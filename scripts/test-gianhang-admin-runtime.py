#!/usr/bin/env python3
import datetime
import http.cookiejar
import json
import os
import re
import ssl
import sys
import time
import urllib.error
import urllib.parse
import urllib.request
from html.parser import HTMLParser


ERROR_MARKERS = [
    "application exception",
    "compilationexception",
    "full error source code",
    "server error in '/' application",
]
REQUEST_TIMEOUT = int(os.environ.get("RUNTIME_HTTP_TIMEOUT", "60"))


class FormParser(HTMLParser):
    def __init__(self):
        super().__init__()
        self.fields = {}
        self.textareas = {}
        self.id_to_name = {}
        self.selects = {}
        self.input_types = {}
        self._current_textarea = None
        self._current_select = None
        self._current_option = None
        self._current_option_text = ""

    def handle_starttag(self, tag, attrs):
        attr = {k.lower(): (v or "") for k, v in attrs}
        if tag == "input":
            name = attr.get("name", "")
            input_type = (attr.get("type", "") or "text").lower()
            if name:
                existing_type = self.input_types.get(name, "")
                if input_type in ("radio", "checkbox"):
                    self.input_types[name] = input_type
                    if "checked" in attr:
                        self.fields[name] = attr.get("value", "on")
                else:
                    if existing_type not in ("radio", "checkbox"):
                        self.fields[name] = attr.get("value", "")
                        self.input_types[name] = input_type
            input_id = attr.get("id", "")
            if input_id and name:
                self.id_to_name[input_id] = name
        elif tag == "textarea":
            name = attr.get("name", "")
            textarea_id = attr.get("id", "")
            if textarea_id and name:
                self.id_to_name[textarea_id] = name
            if name:
                self.textareas[name] = ""
                self._current_textarea = name
        elif tag == "select":
            name = attr.get("name", "")
            select_id = attr.get("id", "")
            if select_id and name:
                self.id_to_name[select_id] = name
            if name:
                self.selects[name] = []
                self._current_select = name
        elif tag == "option":
            if self._current_select:
                self._current_option = attr.get("value", "")
                self._current_option_text = ""

    def handle_endtag(self, tag):
        if tag == "textarea":
            self._current_textarea = None
        elif tag == "select":
            self._current_select = None
        elif tag == "option":
            if self._current_select is not None and self._current_option is not None:
                self.selects[self._current_select].append((self._current_option, self._current_option_text.strip()))
            self._current_option = None
            self._current_option_text = ""

    def handle_data(self, data):
        if self._current_textarea is not None:
            self.textareas[self._current_textarea] += data
        if self._current_option is not None:
            self._current_option_text += data


def summarize_error(body):
    if not body:
        return ""
    match = re.search(r"Exception Details:\s*(.*?)\s*\r?\n", body)
    if match:
        return match.group(1).strip()
    match = re.search(r"Details:\s*(.*?)\s*<br", body, re.I | re.S)
    if match:
        return re.sub(r"\s+", " ", match.group(1)).strip()
    match = re.search(r"<title>\s*(.*?)\s*</title>", body, re.I | re.S)
    if match:
        return re.sub(r"\s+", " ", match.group(1)).strip()
    return ""


def assert_ok(body, current_url, final_url, allow_login_page=False):
    lower = (body or "").lower()
    for marker in ERROR_MARKERS:
        if marker in lower:
            raise RuntimeError("Runtime error at %s (%s): %s" % (current_url, final_url, marker))
    if (not allow_login_page) and "/gianhang/admin/login.aspx" in (final_url or "").lower():
        raise RuntimeError("Unexpected redirect to login at %s" % current_url)
    if "/gianhang/admin/f5_ss_admin.aspx" in (final_url or "").lower():
        raise RuntimeError("Unexpected redirect to session refresh at %s" % current_url)


def find_name_by_id_suffix(parser, suffix):
    suffix = suffix.lower()
    for input_id, name in parser.id_to_name.items():
        if input_id.lower().endswith(suffix):
            return name
    return ""


def choose_option(selects, name, prefer_text=None):
    options = selects.get(name, [])
    if prefer_text:
        target = prefer_text.lower()
        for value, text in options:
            if value and (target in (text or "").lower() or target in (value or "").lower()):
                return value
    for value, _ in options:
        if value:
            return value
    return ""


def choose_option_by_index(selects, name, index):
    options = [value for value, _ in selects.get(name, []) if value]
    if not options:
        return ""
    return options[index % len(options)]


def require_name(parser, suffix, label):
    name = find_name_by_id_suffix(parser, suffix)
    if not name:
        raise RuntimeError("Cannot find field %s (%s)" % (label, suffix))
    return name


def fetch_form(opener, url):
    attempts = 2
    last_error = None
    for index in range(attempts):
        try:
            response = opener.open(url, timeout=REQUEST_TIMEOUT)
            html = response.read().decode("utf-8", "ignore")
            parser = FormParser()
            parser.feed(html)
            return parser, html, response.geturl()
        except urllib.error.HTTPError as exc:
            body = exc.read().decode("utf-8", "ignore")
            last_error = (exc.code, body)
            lower = body.lower()
            can_retry = exc.code == 500 and index + 1 < attempts and (
                "threadabortexception" in lower or "compilationexception" in lower or "parseexception" in lower
            )
            if can_retry:
                time.sleep(1)
                continue

            summary = summarize_error(body)
            dump_path = "/tmp/gianhang_admin_runtime_http_%s.html" % int(time.time())
            try:
                with open(dump_path, "w", encoding="utf-8") as fh:
                    fh.write(body)
            except Exception:
                dump_path = ""
            suffix = (" dump=" + dump_path) if dump_path else ""
            raise RuntimeError("HTTP %s for %s: %s%s" % (exc.code, url, summary or body[:500], suffix))

    if last_error is not None:
        raise RuntimeError("HTTP %s for %s after retry" % (last_error[0], url))
    raise RuntimeError("Failed to fetch form for %s" % url)


def build_form_fields(
    parser,
    overrides,
    submit_name=None,
    event_target=None,
    exclude_input_types=None,
    exclude_names=None,
    base_mode="all",
):
    if base_mode == "hidden_only":
        fields = {}
        for name, value in parser.fields.items():
            if parser.input_types.get(name) == "hidden":
                fields[name] = value
    else:
        fields = dict(parser.fields)
        fields.update(parser.textareas)
    exclude_input_types = set(exclude_input_types or [])
    exclude_names = set(exclude_names or [])
    if exclude_input_types or exclude_names:
        for name, input_type in list(parser.input_types.items()):
            if input_type in exclude_input_types or name in exclude_names:
                fields.pop(name, None)
    fields.update(overrides)
    if event_target is not None:
        fields["__EVENTTARGET"] = event_target
        fields["__EVENTARGUMENT"] = ""
    if submit_name:
        for name, input_type in parser.input_types.items():
            if input_type == "submit" and name != submit_name:
                fields.pop(name, None)
        fields[submit_name] = fields.get(submit_name, "Submit")

    return fields


def post_form(
    opener,
    url,
    parser,
    overrides,
    submit_name=None,
    event_target=None,
    exclude_input_types=None,
    exclude_names=None,
    base_mode="all",
):
    fields = build_form_fields(
        parser,
        overrides,
        submit_name=submit_name,
        event_target=event_target,
        exclude_input_types=exclude_input_types,
        exclude_names=exclude_names,
        base_mode=base_mode,
    )

    data = urllib.parse.urlencode(fields).encode("utf-8")
    request = urllib.request.Request(url, data=data, method="POST")
    try:
        response = opener.open(request, timeout=REQUEST_TIMEOUT)
        body = response.read().decode("utf-8", "ignore")
        return body, response.geturl()
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", "ignore")
        summary = summarize_error(body)
        dump_path = "/tmp/gianhang_admin_runtime_http_%s.html" % int(time.time())
        try:
            with open(dump_path, "w", encoding="utf-8") as fh:
                fh.write(body)
        except Exception:
            dump_path = ""
        suffix = (" dump=" + dump_path) if dump_path else ""
        raise RuntimeError("HTTP %s for %s: %s%s" % (exc.code, url, summary or body[:500], suffix))


def extract_invoice_id(url):
    match = re.search(r"[?&]id=(\d+)", url or "", re.I)
    if not match:
        raise RuntimeError("Cannot extract invoice id from url: %s" % url)
    return match.group(1)


def find_product_id_in_import_page(html, product_name):
    pattern = re.compile(r"<tr[^>]*>.*?%s.*?name=[\"']slsp_(\d+)[\"']" % re.escape(product_name), re.I | re.S)
    match = pattern.search(html or "")
    if not match:
        raise RuntimeError("Cannot find product row for %s on import page." % product_name)
    return match.group(1)


def login_admin(opener, base_url, admin_user, admin_pass):
    login_url = "%s/gianhang/admin/login.aspx" % base_url
    parser, body, final_url = fetch_form(opener, login_url)
    assert_ok(body, login_url, final_url, allow_login_page=True)

    name_user = find_name_by_id_suffix(parser, "txt_user") or find_name_by_id_suffix(parser, "txt_taikhoan")
    name_pass = find_name_by_id_suffix(parser, "txt_pass") or find_name_by_id_suffix(parser, "txt_matkhau")
    name_branch = find_name_by_id_suffix(parser, "ddl_chinhanh")
    name_submit = find_name_by_id_suffix(parser, "but_login") or find_name_by_id_suffix(parser, "Button1")
    branch_value = choose_option(parser.selects, name_branch) or "1"

    if not all([name_user, name_pass, name_submit]):
        raise RuntimeError("Admin login form is incomplete.")

    payload = {
        name_user: admin_user,
        name_pass: admin_pass,
    }
    if name_branch:
        payload[name_branch] = branch_value

    body, final_url = post_form(
        opener,
        login_url,
        parser,
        payload,
        submit_name=name_submit,
    )
    assert_ok(body, login_url, final_url)
    if "/gianhang/admin/login.aspx" in final_url.lower():
        raise RuntimeError("Admin login failed for %s" % admin_user)


def get_authenticated_page(opener, base_url, path):
    url = "%s%s" % (base_url, path)
    parser, body, final_url = fetch_form(opener, url)
    assert_ok(body, url, final_url)
    return parser, body, final_url


def page_contains_text(opener, url, expected_text):
    try:
        _, body, final_url = fetch_form(opener, url)
        assert_ok(body, url, final_url)
        return expected_text.lower() in body.lower()
    except Exception:
        return False


def extract_customer_detail_path_from_body(body, customer_phone):
    phone_pattern = re.escape(customer_phone)
    patterns = [
        r'href="(?P<path>/gianhang/admin/quan-ly-khach-hang/chi-tiet\.aspx\?id=[^"]+)".{0,2000}%s' % phone_pattern,
        r'%s.{0,2000}href="(?P<path>/gianhang/admin/quan-ly-khach-hang/chi-tiet\.aspx\?id=[^"]+)"' % phone_pattern,
    ]
    for pattern in patterns:
        match = re.search(pattern, body, re.I | re.S)
        if match:
            return match.group("path")
    return ""


def find_customer_detail_path(opener, base_url, customer_phone):
    page_url = "%s/gianhang/admin/quan-ly-khach-hang/default.aspx" % base_url
    parser, body, final_url = fetch_form(opener, page_url)
    assert_ok(body, page_url, final_url)

    detail_path = extract_customer_detail_path_from_body(body, customer_phone)
    if detail_path:
        return detail_path

    search_name = find_name_by_id_suffix(parser, "txt_search")
    if not search_name:
        return ""

    body, final_url = post_form(
        opener,
        page_url,
        parser,
        {search_name: customer_phone},
        event_target=search_name,
    )
    assert_ok(body, page_url, final_url)
    return extract_customer_detail_path_from_body(body, customer_phone)


def create_trade_post(opener, base_url, post_type, post_name):
    add_path = "/gianhang/admin/quan-ly-bai-viet/add.aspx"
    add_url = "%s%s" % (base_url, add_path)

    parser, body, final_url = fetch_form(opener, add_url)
    assert_ok(body, add_url, final_url)

    drop_name = find_name_by_id_suffix(parser, "DropDownList1")
    if not drop_name:
        raise RuntimeError(
            "Cannot find trade type dropdown on add post page. final_url=%s ids=%s selects=%s"
            % (final_url, sorted(parser.id_to_name.keys())[:30], sorted(parser.selects.keys())[:30])
        )

    body, final_url = post_form(
        opener,
        add_url,
        parser,
        {drop_name: post_type},
        event_target=drop_name,
    )
    assert_ok(body, add_url, final_url)
    parser = FormParser()
    parser.feed(body)

    name_title = find_name_by_id_suffix(parser, "txt_name")
    name_description = find_name_by_id_suffix(parser, "txt_description")
    name_content = find_name_by_id_suffix(parser, "txt_content")
    name_submit = find_name_by_id_suffix(parser, "button1")
    menu_name = "listmenu" if "listmenu" in parser.selects else ""

    if not all([name_title, name_description, name_content, name_submit, menu_name]):
        raise RuntimeError("Add post form missing required fields after postback for %s" % post_type)

    overrides = {
        drop_name: post_type,
        name_title: post_name,
        name_description: "Runtime test %s" % post_name,
        name_content: "Runtime test %s" % post_name,
        menu_name: choose_option(parser.selects, menu_name),
    }

    if post_type == "ctdv":
        name_price = find_name_by_id_suffix(parser, "txt_giadichvu")
        name_sale = find_name_by_id_suffix(parser, "txt_chotsale_dichvu")
        name_do = find_name_by_id_suffix(parser, "txt_lam_dichvu")
        name_duration = find_name_by_id_suffix(parser, "txt_thoiluong_dichvu")
        name_industry = find_name_by_id_suffix(parser, "DropDownList5")
        overrides.update({
            name_price: "250000",
            name_sale: "10",
            name_do: "20",
            name_duration: "90",
            name_industry: choose_option(parser.selects, name_industry),
        })
    else:
        name_price = find_name_by_id_suffix(parser, "txt_giasanpham")
        name_cost = find_name_by_id_suffix(parser, "txt_giavonsanpham")
        name_sale = find_name_by_id_suffix(parser, "txt_chotsale_sanpham")
        name_unit = find_name_by_id_suffix(parser, "txt_dvt_sp")
        name_industry = find_name_by_id_suffix(parser, "DropDownList6")
        overrides.update({
            name_price: "350000",
            name_cost: "200000",
            name_sale: "15",
            name_unit: "chai",
            name_industry: choose_option(parser.selects, name_industry),
        })

    body, final_url = post_form(opener, add_url, parser, overrides, submit_name=name_submit)
    assert_ok(body, add_url, final_url)

    if "/quan-ly-bai-viet/default.aspx" not in final_url.lower():
        raise RuntimeError("Unexpected redirect after saving %s: %s" % (post_type, final_url))


def create_staff_account(opener, base_url, account_user):
    add_path = "/gianhang/admin/quan-ly-tai-khoan/add.aspx"
    add_url = "%s%s" % (base_url, add_path)

    parser, body, final_url = fetch_form(opener, add_url)
    assert_ok(body, add_url, final_url)

    name_user = require_name(parser, "txt_taikhoan", "account username")
    name_pass = require_name(parser, "txt_matkhau", "account password")
    name_fullname = require_name(parser, "txt_hoten", "account fullname")
    name_email = require_name(parser, "txt_email", "account email")
    name_phone = require_name(parser, "txt_dienthoai", "account phone")
    name_salary = require_name(parser, "txt_luong", "salary")
    name_workdays = require_name(parser, "txt_songaycong", "workdays")
    name_status = require_name(parser, "DropDownList1", "status")
    name_industry = require_name(parser, "DropDownList5", "industry")
    name_submit = require_name(parser, "button1", "submit button")

    industry_value = choose_option(parser.selects, name_industry)
    if not industry_value:
        raise RuntimeError("Industry dropdown has no selectable option on staff account form.")

    body, final_url = post_form(
        opener,
        add_url,
        parser,
        {
            name_user: account_user,
            name_pass: "123456",
            name_fullname: "Runtime Staff %s" % account_user[-6:],
            name_email: "%s@example.test" % account_user,
            name_phone: "0900000000",
            name_salary: "12000000",
            name_workdays: "26",
            name_status: "Đang hoạt động",
            name_industry: industry_value,
        },
        submit_name=name_submit,
    )
    assert_ok(body, add_url, final_url)

    expected = "/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=%s" % account_user
    if expected.lower() not in final_url.lower():
        expected_url = "%s%s" % (base_url, expected)
        if page_contains_text(opener, expected_url, account_user):
            return
        raise RuntimeError("Unexpected redirect after saving account %s: %s" % (account_user, final_url))


def create_supply_item(opener, base_url, supply_name):
    add_path = "/gianhang/admin/quan-ly-vat-tu/add.aspx"
    add_url = "%s%s" % (base_url, add_path)

    parser, body, final_url = fetch_form(opener, add_url)
    assert_ok(body, add_url, final_url)

    name_title = require_name(parser, "txt_name", "supply name")
    name_group = require_name(parser, "ddl_nhom", "supply group")
    name_supplier = require_name(parser, "DropDownList3", "supplier")
    name_price = require_name(parser, "txt_giasanpham", "sale price")
    name_cost = require_name(parser, "txt_giavonsanpham", "cost price")
    name_unit = require_name(parser, "txt_dvt_sp", "unit")
    name_note = require_name(parser, "txt_ghichu", "note")
    name_condition = require_name(parser, "DropDownList1", "condition")
    name_department = require_name(parser, "DropDownList2", "department")
    name_submit = require_name(parser, "button1", "submit button")

    body, final_url = post_form(
        opener,
        add_url,
        parser,
        {
            name_title: supply_name,
            name_group: choose_option(parser.selects, name_group) or "0",
            name_supplier: choose_option(parser.selects, name_supplier),
            name_price: "180000",
            name_cost: "90000",
            name_unit: "hop",
            name_note: "Runtime vat tu %s" % supply_name,
            name_condition: "Mua",
            name_department: choose_option(parser.selects, name_department),
        },
        submit_name=name_submit,
    )
    assert_ok(body, add_url, final_url)

    if "/gianhang/admin/quan-ly-vat-tu/add.aspx" not in final_url.lower():
        raise RuntimeError("Unexpected redirect after saving supply item %s: %s" % (supply_name, final_url))


def create_customer(opener, base_url, customer_name, customer_phone):
    page_path = "/gianhang/admin/quan-ly-khach-hang/default.aspx"
    page_url = "%s%s" % (base_url, page_path)

    parser, body, final_url = fetch_form(opener, page_url)
    assert_ok(body, page_url, final_url)

    name_customer = require_name(parser, "txt_tenkhachhang", "customer name")
    name_phone = require_name(parser, "txt_sdt", "customer phone")
    name_birth = require_name(parser, "txt_ngaysinh", "customer birth date")
    name_address = require_name(parser, "txt_diachi", "customer address")
    name_staff = require_name(parser, "ddl_nhanvien_chamsoc", "customer care staff")
    name_ref = require_name(parser, "txt_magioithieu", "referral code")
    name_group = require_name(parser, "DropDownList1", "customer group")
    name_submit = require_name(parser, "Button3", "customer submit button")

    body, final_url = post_form(
        opener,
        page_url,
        parser,
        {
            name_customer: customer_name,
            name_phone: customer_phone,
            name_birth: "",
            name_address: "Runtime customer address",
            name_staff: choose_option(parser.selects, name_staff),
            name_ref: "",
            name_group: choose_option(parser.selects, name_group),
        },
        submit_name=name_submit,
    )
    assert_ok(body, page_url, final_url)


def assert_prefill_customer_fields(opener, base_url, page_path, customer_name, customer_phone):
    page_url = "%s%s" % (base_url, page_path)
    parser, body, final_url = fetch_form(opener, page_url)
    assert_ok(body, page_url, final_url)

    name_customer = require_name(parser, "txt_tenkhachhang", "prefill customer name")
    name_phone = require_name(parser, "txt_sdt", "prefill customer phone")

    actual_customer = (parser.fields.get(name_customer) or "").strip()
    actual_phone = (parser.fields.get(name_phone) or "").strip()
    if actual_customer != customer_name:
        raise RuntimeError("Prefill customer name mismatch for %s: %s" % (page_path, actual_customer))
    if actual_phone != customer_phone:
        raise RuntimeError("Prefill customer phone mismatch for %s: %s" % (page_path, actual_phone))


def assert_customer_profile_quick_actions(opener, base_url, customer_name, customer_phone):
    detail_path = find_customer_detail_path(opener, base_url, customer_phone)
    if not detail_path:
        raise RuntimeError("Cannot find customer detail path for %s" % customer_phone)

    detail_url = "%s%s" % (base_url, detail_path)
    _, detail_body, detail_final = fetch_form(opener, detail_url)
    assert_ok(detail_body, detail_url, detail_final)

    encoded_phone = urllib.parse.quote_plus(customer_phone, safe="")
    encoded_name = urllib.parse.quote_plus(customer_name, safe="")
    expected_patterns = [
        r"/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen\.aspx\?q=add(?:&|&amp;)sdt=%s(?:&|&amp;)tenkh=%s" % (re.escape(encoded_phone), re.escape(encoded_name)),
        r"/gianhang/admin/quan-ly-hoa-don/Default\.aspx\?q=add(?:&|&amp;)sdt=%s(?:&|&amp;)tenkh=%s" % (re.escape(encoded_phone), re.escape(encoded_name)),
        r"/gianhang/admin/quan-ly-the-dich-vu/Default\.aspx\?q=add(?:&|&amp;)sdt=%s(?:&|&amp;)tenkh=%s" % (re.escape(encoded_phone), re.escape(encoded_name)),
    ]
    for pattern in expected_patterns:
        if not re.search(pattern, detail_body, re.I):
            raise RuntimeError("Missing quick action on customer detail: %s" % pattern)


def create_invoice(opener, base_url, customer_name, customer_phone):
    page_path = "/gianhang/admin/quan-ly-hoa-don/default.aspx"
    page_url = "%s%s" % (base_url, page_path)

    parser, body, final_url = fetch_form(opener, page_url)
    assert_ok(body, page_url, final_url)

    name_industry = require_name(parser, "DropDownList3", "invoice industry")
    name_created = require_name(parser, "txt_ngaytao", "invoice created date")
    name_customer = require_name(parser, "txt_tenkhachhang", "invoice customer name")
    name_phone = require_name(parser, "txt_sdt", "invoice customer phone")
    name_address = require_name(parser, "txt_diachi", "invoice customer address")
    name_note = require_name(parser, "txt_ghichu", "invoice note")
    name_staff = require_name(parser, "ddl_nhanvien_chamsoc", "invoice care staff")
    name_ref = require_name(parser, "txt_magioithieu", "invoice referral code")
    name_group = require_name(parser, "DropDownList1", "invoice customer group")
    name_submit = require_name(parser, "Button3", "invoice submit button")

    industry_value = choose_option(parser.selects, name_industry)
    if not industry_value:
        raise RuntimeError("Invoice industry dropdown has no selectable option.")

    body, final_url = post_form(
        opener,
        page_url,
        parser,
        {
            name_industry: industry_value,
            name_created: time.strftime("%d/%m/%Y"),
            name_customer: customer_name,
            name_phone: customer_phone,
            name_address: "Runtime invoice address",
            name_note: "Runtime invoice %s" % customer_phone,
            name_staff: choose_option(parser.selects, name_staff),
            name_ref: "",
            name_group: choose_option(parser.selects, name_group),
        },
        submit_name=name_submit,
    )
    assert_ok(body, page_url, final_url)

    if "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" not in final_url.lower():
        raise RuntimeError("Unexpected redirect after creating invoice for %s: %s" % (customer_phone, final_url))

    return extract_invoice_id(final_url)


def create_appointment(opener, base_url, customer_name, customer_phone, service_name, appointment_note):
    page_path = "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx?q=add"
    page_url = "%s%s" % (base_url, page_path)
    last_final_url = ""
    for attempt in range(2):
        parser, body, final_url = fetch_form(opener, page_url)
        assert_ok(body, page_url, final_url)

        name_date = require_name(parser, "txt_ngaydat", "appointment date")
        name_hour = require_name(parser, "ddl_giobatdau", "appointment hour")
        name_minute = require_name(parser, "ddl_phutbatdau", "appointment minute")
        name_customer = require_name(parser, "txt_tenkhachhang", "appointment customer")
        name_phone = require_name(parser, "txt_sdt", "appointment phone")
        name_note = require_name(parser, "txt_ghichu", "appointment note")
        name_source = require_name(parser, "txt_nguon", "appointment source")
        name_service = require_name(parser, "ddl_dichvu", "appointment service")
        name_staff = require_name(parser, "ddl_nhanvien", "appointment staff")
        name_status = require_name(parser, "ddl_trangthai", "appointment status")
        name_submit = require_name(parser, "Button3", "appointment submit button")

        service_value = choose_option(parser.selects, name_service, service_name)
        if not service_value:
            raise RuntimeError("Cannot find service %s on appointment page." % service_name)

        slot_seed = int(re.sub(r"\D", "", customer_phone or "0")[-4:] or "0")
        appointment_date = (datetime.date.today() + datetime.timedelta(days=1 + (slot_seed % 9))).strftime("%d/%m/%Y")
        hour_value = choose_option_by_index(parser.selects, name_hour, slot_seed)
        minute_value = choose_option_by_index(parser.selects, name_minute, slot_seed // 3)
        if not hour_value or not minute_value:
            raise RuntimeError("Appointment time dropdowns have no selectable options.")

        body, final_url = post_form(
            opener,
            page_url,
            parser,
            {
                name_date: appointment_date,
                name_hour: hour_value,
                name_minute: minute_value,
                name_customer: customer_name,
                name_phone: customer_phone,
                name_note: appointment_note,
                name_source: "Runtime",
                name_service: service_value,
                name_staff: choose_option(parser.selects, name_staff),
                name_status: choose_option(parser.selects, name_status, "chưa") or choose_option(parser.selects, name_status),
            },
            submit_name=name_submit,
        )
        assert_ok(body, page_url, final_url)

        if "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx" in final_url.lower():
            return

        last_final_url = final_url
        list_url = "%s/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx" % base_url
        if page_contains_text(opener, list_url, appointment_note):
            return
        if "/gianhang/admin/" in final_url.lower() and attempt == 0:
            time.sleep(1)
            continue

        break

    raise RuntimeError("Unexpected redirect after creating appointment for %s: %s" % (customer_phone, last_final_url))


def import_product_stock(opener, base_url, product_name, import_note):
    page_path = "/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx?q=nh"
    page_url = "%s%s" % (base_url, page_path)

    parser, body, final_url = fetch_form(opener, page_url)
    assert_ok(body, page_url, final_url)
    try:
        with open("/tmp/gianhang_import_initial.html", "w", encoding="utf-8") as fh:
            fh.write(body)
    except Exception:
        pass

    product_id = find_product_id_in_import_page(body, product_name)
    add_submit = require_name(parser, "but_themvaogio", "import add-to-cart button")
    today = datetime.date.today().strftime("%d/%m/%Y")
    next_year = (datetime.date.today() + datetime.timedelta(days=365)).strftime("%d/%m/%Y")
    lot_code = "RT%s" % product_id
    exclude_add_names = {
        name for name in parser.input_types
        if "ck_hd" in name.lower()
    }
    exclude_add_names.update(
        name for input_id, name in parser.id_to_name.items()
        if "ck_hd" in input_id.lower()
    )

    body, final_url = post_form(
        opener,
        page_url,
        parser,
        {
            "slsp_%s" % product_id: "3",
            "giavon_%s" % product_id: "200000",
            "ck_%s" % product_id: "0",
            "solo_%s" % product_id: lot_code,
            "dvt_%s" % product_id: "chai",
            "nsx_%s" % product_id: today,
            "hsd_%s" % product_id: next_year,
        },
        submit_name=add_submit,
        exclude_input_types={"radio"},
        exclude_names=exclude_add_names,
        base_mode="hidden_only",
    )
    assert_ok(body, page_url, final_url)

    parser = FormParser()
    parser.feed(body)
    try:
        with open("/tmp/gianhang_import_after_add.html", "w", encoding="utf-8") as fh:
            fh.write(body)
    except Exception:
        pass

    name_date = require_name(parser, "txt_ngaynhap", "import date")
    name_supplier = require_name(parser, "DropDownList3", "import supplier")
    name_note = require_name(parser, "txt_ghichu", "import note")
    name_discount = require_name(parser, "txt_chietkhau", "import discount")
    confirm_submit = require_name(parser, "Button1", "confirm import button")
    exclude_confirm_names = {
        name for name in parser.input_types
        if "ck_hd" in name.lower()
    }
    exclude_confirm_names.update(
        name for input_id, name in parser.id_to_name.items()
        if "ck_hd" in input_id.lower()
    )

    body, final_url = post_form(
        opener,
        page_url,
        parser,
        {
            name_date: today,
            name_supplier: choose_option(parser.selects, name_supplier),
            name_note: import_note,
            name_discount: "0",
        },
        submit_name=confirm_submit,
        exclude_input_types={"radio"},
        exclude_names=exclude_confirm_names,
        base_mode="hidden_only",
    )
    assert_ok(body, page_url, final_url)

    if "/gianhang/admin/quan-ly-kho-hang/don-nhap-hang.aspx" not in final_url.lower():
        raise RuntimeError("Unexpected redirect after importing stock for %s: %s" % (product_name, final_url))


def add_invoice_service(opener, base_url, invoice_id, service_name):
    page_path = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=%s&q=add" % invoice_id
    page_url = "%s%s" % (base_url, page_path)

    parser, body, final_url = fetch_form(opener, page_url)
    assert_ok(body, page_url, final_url)

    name_date = require_name(parser, "txt_ngayban", "invoice service date")
    name_service = require_name(parser, "txt_tendichvu", "invoice service name")
    name_price = require_name(parser, "txt_gia", "invoice service price")
    name_quantity = require_name(parser, "txt_soluong", "invoice service quantity")
    name_discount = require_name(parser, "txt_chietkhau", "invoice service discount")
    name_sale_staff = require_name(parser, "ddl_nhanvien_chotsale", "invoice sale staff")
    name_sale_discount = require_name(parser, "txt_chietkhau_chotsale", "invoice sale discount")
    name_do_staff = require_name(parser, "ddl_nhanvien_lamdichvu", "invoice service staff")
    name_do_discount = require_name(parser, "txt_chietkhau_lamdichvu", "invoice service staff discount")
    name_review = require_name(parser, "txt_danhgia_dichvu", "invoice service review")
    name_submit = require_name(parser, "but_form_themdichvu", "invoice add service button")
    exclude_service_names = {
        name for name in parser.input_types
        if "ck_dv" in name.lower() or "ck_hd" in name.lower()
    }
    exclude_service_names.update(
        name for input_id, name in parser.id_to_name.items()
        if "ck_dv" in input_id.lower() or "ck_hd" in input_id.lower()
    )

    body, final_url = post_form(
        opener,
        page_url,
        parser,
        {
            name_date: datetime.date.today().strftime("%d/%m/%Y"),
            name_service: service_name,
            name_price: "250000",
            name_quantity: "1",
            name_discount: "0",
            name_sale_staff: choose_option(parser.selects, name_sale_staff),
            name_sale_discount: "0",
            name_do_staff: choose_option(parser.selects, name_do_staff),
            name_do_discount: "0",
            name_review: "Runtime service flow",
            "danhgia_5sao_nhanvien_dv": "5",
        },
        submit_name=name_submit,
        exclude_input_types={"radio"},
        exclude_names=exclude_service_names,
        base_mode="hidden_only",
    )
    assert_ok(body, page_url, final_url)

    if ("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" not in final_url.lower()) or (invoice_id not in final_url):
        raise RuntimeError("Unexpected redirect after adding service to invoice %s: %s" % (invoice_id, final_url))


def add_invoice_product(opener, base_url, invoice_id, product_name):
    page_path = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=%s&q=add" % invoice_id
    page_url = "%s%s" % (base_url, page_path)

    parser, body, final_url = fetch_form(opener, page_url)
    assert_ok(body, page_url, final_url)

    name_date = require_name(parser, "txt_ngayban_sanpham", "invoice product date")
    name_product = require_name(parser, "txt_tensanpham", "invoice product name")
    name_price = require_name(parser, "txt_gia_sanpham", "invoice product price")
    name_quantity = require_name(parser, "txt_soluong_sanpham", "invoice product quantity")
    name_discount = require_name(parser, "txt_chietkhau_sanpham", "invoice product discount")
    name_sale_staff = require_name(parser, "ddl_nhanvien_chotsale_sanpham", "invoice product sale staff")
    name_sale_discount = require_name(parser, "txt_chietkhau_chotsale_sanpham", "invoice product sale discount")
    name_submit = require_name(parser, "but_form_themsanpham", "invoice add product button")
    exclude_product_names = {
        name for name in parser.input_types
        if "ck_sp" in name.lower() or "ck_hd" in name.lower()
    }
    exclude_product_names.update(
        name for input_id, name in parser.id_to_name.items()
        if "ck_sp" in input_id.lower() or "ck_hd" in input_id.lower()
    )

    body, final_url = post_form(
        opener,
        page_url,
        parser,
        {
            name_date: datetime.date.today().strftime("%d/%m/%Y"),
            name_product: product_name,
            name_price: "350000",
            name_quantity: "1",
            name_discount: "0",
            name_sale_staff: choose_option(parser.selects, name_sale_staff),
            name_sale_discount: "0",
        },
        submit_name=name_submit,
        exclude_input_types={"radio"},
        exclude_names=exclude_product_names,
        base_mode="hidden_only",
    )
    assert_ok(body, page_url, final_url)

    if ("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" not in final_url.lower()) or (invoice_id not in final_url):
        raise RuntimeError("Unexpected redirect after adding product to invoice %s: %s" % (invoice_id, final_url))


def main():
    base_url = os.environ.get("BASE_URL", "https://ahasale.local").rstrip("/")
    admin_user = os.environ.get("ADMIN_USER", "admin")
    admin_pass = os.environ.get("ADMIN_PASS", "123456")
    service_name = os.environ.get("SERVICE_NAME", "runtime_service_%d" % int(time.time()))
    product_name = os.environ.get("PRODUCT_NAME", "runtime_product_%d" % int(time.time()))
    runtime_user_account = os.environ.get("RUNTIME_USER_ACCOUNT", "rtu%d" % int(time.time()))
    runtime_vattu_name = os.environ.get("RUNTIME_VATTU_NAME", "runtime_vattu_%d" % int(time.time()))
    phone_seed = str(int(time.time()) % 100000000).zfill(8)
    runtime_customer_name = os.environ.get("RUNTIME_CUSTOMER_NAME", "Runtime Customer %s" % phone_seed[-4:])
    runtime_customer_phone = os.environ.get("RUNTIME_CUSTOMER_PHONE", "09%s" % phone_seed)
    runtime_invoice_customer_name = os.environ.get("RUNTIME_INVOICE_CUSTOMER_NAME", "Runtime Invoice %s" % phone_seed[-4:])
    runtime_invoice_phone = os.environ.get("RUNTIME_INVOICE_PHONE", "08%s" % phone_seed)
    runtime_appointment_note = os.environ.get("RUNTIME_APPOINTMENT_NOTE", "Runtime appointment %s" % runtime_customer_phone)
    runtime_import_note = os.environ.get("RUNTIME_IMPORT_NOTE", "Runtime import %s" % product_name)
    runtime_include_invoice_detail = os.environ.get("RUNTIME_INCLUDE_INVOICE_DETAIL", "0") == "1"
    runtime_include_warehouse = os.environ.get("RUNTIME_INCLUDE_WAREHOUSE", "0") == "1"
    create_only = os.environ.get("RUNTIME_CREATE_ONLY", "") == "1"

    context = ssl._create_unverified_context()
    cookie_jar = http.cookiejar.CookieJar()
    opener = urllib.request.build_opener(
        urllib.request.HTTPCookieProcessor(cookie_jar),
        urllib.request.HTTPSHandler(context=context),
        urllib.request.HTTPHandler(),
    )
    opener.addheaders = [("User-Agent", "AhaSaleAdminRuntime/1.0")]

    print("Step: login admin")
    login_admin(opener, base_url, admin_user, admin_pass)

    pages = [
        "/gianhang/admin/default.aspx",
        "/gianhang/admin/cai-dat-chung/default.aspx",
        "/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx",
        "/gianhang/admin/cau-hinh-storefront/default.aspx",
        "/gianhang/admin/cau-hinh-storefront/edit-section.aspx?id=1",
        "/gianhang/admin/quan-ly-bai-viet/default.aspx",
        "/gianhang/admin/quan-ly-hoa-don/default.aspx",
        "/gianhang/admin/quan-ly-khach-hang/default.aspx",
        "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx",
        "/gianhang/admin/quan-ly-kho-hang/default.aspx",
        "/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx?q=nh",
        "/gianhang/admin/quan-ly-tai-khoan/default.aspx",
        "/gianhang/admin/quan-ly-vat-tu/default.aspx",
    ]

    if not create_only:
        print("Step: verify authenticated module access")
        for path in pages:
            get_authenticated_page(opener, base_url, path)
            print("OK", path)

    print("Step: create service post")
    create_trade_post(opener, base_url, "ctdv", service_name)

    print("Step: create product post")
    create_trade_post(opener, base_url, "ctsp", product_name)

    print("Step: create staff account")
    create_staff_account(opener, base_url, runtime_user_account)

    print("Step: create supply item")
    create_supply_item(opener, base_url, runtime_vattu_name)

    print("Step: create customer")
    create_customer(opener, base_url, runtime_customer_name, runtime_customer_phone)

    print("Step: verify customer quick actions")
    assert_customer_profile_quick_actions(opener, base_url, runtime_customer_name, runtime_customer_phone)

    print("Step: verify appointment prefill")
    assert_prefill_customer_fields(
        opener,
        base_url,
        "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx?q=add&sdt=%s&tenkh=%s" % (
            urllib.parse.quote(runtime_customer_phone, safe=""),
            urllib.parse.quote(runtime_customer_name, safe=""),
        ),
        runtime_customer_name,
        runtime_customer_phone,
    )

    print("Step: verify invoice prefill")
    assert_prefill_customer_fields(
        opener,
        base_url,
        "/gianhang/admin/quan-ly-hoa-don/Default.aspx?q=add&sdt=%s&tenkh=%s" % (
            urllib.parse.quote(runtime_customer_phone, safe=""),
            urllib.parse.quote(runtime_customer_name, safe=""),
        ),
        runtime_customer_name,
        runtime_customer_phone,
    )

    print("Step: verify service-card prefill")
    assert_prefill_customer_fields(
        opener,
        base_url,
        "/gianhang/admin/quan-ly-the-dich-vu/Default.aspx?q=add&sdt=%s&tenkh=%s" % (
            urllib.parse.quote(runtime_customer_phone, safe=""),
            urllib.parse.quote(runtime_customer_name, safe=""),
        ),
        runtime_customer_name,
        runtime_customer_phone,
    )

    print("Step: create appointment")
    create_appointment(opener, base_url, runtime_customer_name, runtime_customer_phone, service_name, runtime_appointment_note)

    if runtime_include_warehouse:
        print("Step: import product stock")
        import_product_stock(opener, base_url, product_name, runtime_import_note)

    print("Step: create invoice")
    invoice_id = create_invoice(opener, base_url, runtime_invoice_customer_name, runtime_invoice_phone)

    if runtime_include_invoice_detail:
        print("Step: add service to invoice")
        add_invoice_service(opener, base_url, invoice_id, service_name)

        if runtime_include_warehouse:
            print("Step: add product to invoice")
            add_invoice_product(opener, base_url, invoice_id, product_name)

    output = {
        "SERVICE_NAME": service_name,
        "PRODUCT_NAME": product_name,
        "RUNTIME_USER_ACCOUNT": runtime_user_account,
        "RUNTIME_VATTU_NAME": runtime_vattu_name,
        "RUNTIME_CUSTOMER_NAME": runtime_customer_name,
        "RUNTIME_CUSTOMER_PHONE": runtime_customer_phone,
        "RUNTIME_APPOINTMENT_NOTE": runtime_appointment_note,
        "RUNTIME_INVOICE_CUSTOMER_NAME": runtime_invoice_customer_name,
        "RUNTIME_INVOICE_PHONE": runtime_invoice_phone,
        "RUNTIME_IMPORT_NOTE": runtime_import_note,
        "RUNTIME_INCLUDE_INVOICE_DETAIL": "1" if runtime_include_invoice_detail else "0",
        "RUNTIME_INCLUDE_WAREHOUSE": "1" if runtime_include_warehouse else "0",
        "RUNTIME_INVOICE_ID": invoice_id,
    }

    output_json_path = os.environ.get("RUNTIME_OUTPUT_JSON_PATH", "").strip()
    if output_json_path:
        with open(output_json_path, "w", encoding="utf-8") as handle:
            json.dump(output, handle, ensure_ascii=False, indent=2)

    for key, value in output.items():
        print("%s=%s" % (key, value))


if __name__ == "__main__":
    try:
        main()
    except Exception as exc:
        print("Admin runtime test failed:", exc, file=sys.stderr)
        sys.exit(1)
