#!/usr/bin/env python3
import os
import ssl
import sys
import time
import urllib.parse
import urllib.request
import urllib.error
import re
import http.cookiejar
from html.parser import HTMLParser

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
                value = attr.get("value", "")
                label = attr.get("label", "")
                self._current_option = (value, label)
                self._current_option_text = ""

    def handle_endtag(self, tag):
        if tag == "textarea":
            self._current_textarea = None
        elif tag == "select":
            self._current_select = None
        elif tag == "option":
            if self._current_select and self._current_option:
                value, label = self._current_option
                text = label if label else self._current_option_text.strip()
                self.selects[self._current_select].append((value, text))
            self._current_option = None
            self._current_option_text = ""

    def handle_data(self, data):
        if self._current_textarea is not None:
            self.textareas[self._current_textarea] += data
        if self._current_option is not None:
            self._current_option_text += data


def find_name_by_id_suffix(parser, suffix):
    suffix = suffix.lower()
    for input_id, name in parser.id_to_name.items():
        if input_id.lower().endswith(suffix):
            return name
    return ""


def summarize_error(body):
    if not body:
        return ""
    match = re.search(r"Exception Details:\s*(.*?)\s*\r?\n", body)
    if match:
        return match.group(1).strip()
    match = re.search(r"<h2 class=\"exceptionMessage\">\s*(.*?)\s*</h2>", body, re.S)
    if match:
        return re.sub(r"\s+", " ", match.group(1)).strip()
    return ""


def fetch_form(opener, url):
    try:
        resp = opener.open(url)
        html = resp.read().decode("utf-8", "ignore")
        parser = FormParser()
        parser.feed(html)
        return parser, html, resp.geturl()
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", "ignore")
        summary = summarize_error(body)
        head = body[:1200]
        tail = body[-1200:] if len(body) > 1200 else ""
        prefix = (summary + " | ") if summary else ""
        raise RuntimeError("HTTP %s for %s: %s%s%s" % (exc.code, url, prefix, head, tail))


def choose_option(selects, name, match_text=None):
    options = selects.get(name, [])
    if match_text:
        target = match_text.lower()
        for value, text in options:
            if text and target in text.lower():
                return value
    for value, _ in options:
        if value:
            return value
    return ""


def post_form(opener, url, parser, overrides, submit_name=None):
    fields = dict(parser.fields)
    fields.update(parser.textareas)
    fields.update(overrides)
    if submit_name:
        for name, typ in parser.input_types.items():
            if typ == "submit" and name != submit_name:
                fields.pop(name, None)
        fields[submit_name] = fields.get(submit_name, "Submit")
    data = urllib.parse.urlencode(fields).encode("utf-8")
    req = urllib.request.Request(url, data=data, method="POST")
    try:
        resp = opener.open(req)
        body = resp.read().decode("utf-8", "ignore")
        return body, resp.geturl()
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", "ignore")
        summary = summarize_error(body)
        head = body[:1200]
        tail = body[-1200:] if len(body) > 1200 else ""
        prefix = (summary + " | ") if summary else ""
        raise RuntimeError("HTTP %s for %s: %s%s%s" % (exc.code, url, prefix, head, tail))


def main():
    base_url = os.environ.get("BASE_URL", "https://ahasale.local").rstrip("/")
    shop_email = os.environ.get("SHOP_EMAIL", "demo@ahasale.local")
    shop_pass = os.environ.get("SHOP_PASS", "123123")
    product_name = os.environ.get("PRODUCT_NAME", "Test SP %d" % int(time.time()))
    product_desc = os.environ.get("PRODUCT_DESC", "Mô tả test")
    product_content = os.environ.get("PRODUCT_CONTENT", "Nội dung test")
    product_image = os.environ.get("PRODUCT_IMAGE", "/uploads/images/macdinh.jpg")
    product_price = os.environ.get("PRODUCT_PRICE", "150000")
    product_cost = os.environ.get("PRODUCT_COST", "90000")
    product_stock = os.environ.get("PRODUCT_STOCK", "7")

    invoice_name = os.environ.get("INVOICE_NAME", "Khach test")
    invoice_phone = os.environ.get("INVOICE_PHONE", "0900000001")
    invoice_address = os.environ.get("INVOICE_ADDRESS", "AhaSale local")
    invoice_note = os.environ.get("INVOICE_NOTE", "Test hoa don")
    invoice_qty = os.environ.get("INVOICE_QTY", "1")

    ctx = ssl._create_unverified_context()
    cj = http.cookiejar.CookieJar()
    opener = urllib.request.build_opener(
        urllib.request.HTTPCookieProcessor(cj),
        urllib.request.HTTPSHandler(context=ctx)
    )
    opener.addheaders = [("User-Agent", "AhaSaleTest/1.0")]

    print("Step: login")
    login_url = f"{base_url}/shop/login.aspx"
    parser, _, _ = fetch_form(opener, login_url)
    name_user = find_name_by_id_suffix(parser, "txt_user")
    name_pass = find_name_by_id_suffix(parser, "txt_pass")
    name_submit = find_name_by_id_suffix(parser, "but_login")
    if not name_user or not name_pass or not name_submit:
        print("Missing login fields", file=sys.stderr)
        sys.exit(2)

    body, final_url = post_form(opener, login_url, parser, {
        name_user: shop_email,
        name_pass: shop_pass
    }, submit_name=name_submit)

    if "/shop/login.aspx" in final_url:
        print("Login failed (still at login page)", file=sys.stderr)
        sys.exit(3)



    print("Step: check admin access")
    admin_default = f"{base_url}/gianhang/admin/default.aspx"
    _, _, admin_final = fetch_form(opener, admin_default)
    if "/shop/login.aspx" in admin_final:
        print("Not authenticated for shop admin", file=sys.stderr)
        sys.exit(4)

    print("Step: create product")
    product_url = f"{base_url}/gianhang/admin/san-pham.aspx"
    parser, _, _ = fetch_form(opener, product_url)

    name_txt_name = find_name_by_id_suffix(parser, "txt_name")
    name_txt_desc = find_name_by_id_suffix(parser, "txt_description")
    name_txt_content = find_name_by_id_suffix(parser, "txt_noidung")
    name_txt_image = find_name_by_id_suffix(parser, "txt_link_fileupload")
    name_txt_price = find_name_by_id_suffix(parser, "txt_giaban")
    name_txt_cost = find_name_by_id_suffix(parser, "txt_giavon")
    name_txt_stock = find_name_by_id_suffix(parser, "txt_ton")
    name_loai = find_name_by_id_suffix(parser, "ddl_loai")
    name_danhmuc = find_name_by_id_suffix(parser, "ddl_danhmuc")
    name_submit_save = find_name_by_id_suffix(parser, "but_save")

    if not name_txt_name or not name_submit_save:
        print("Missing product form fields", file=sys.stderr)
        sys.exit(5)

    overrides = {
        name_txt_name: product_name,
        name_txt_desc: product_desc,
        name_txt_content: product_content,
        name_txt_image: product_image,
        name_txt_price: product_price,
        name_txt_cost: product_cost,
        name_txt_stock: product_stock
    }
    if name_loai:
        overrides[name_loai] = "sanpham"
    if name_danhmuc:
        overrides[name_danhmuc] = choose_option(parser.selects, name_danhmuc)

    try:
        debug_post_path = "/tmp/gh_admin_product_post.txt"
        with open(debug_post_path, "w", encoding="utf-8") as fh:
            for key in sorted(overrides.keys()):
                fh.write("%s=%s\n" % (key, overrides[key]))
    except Exception:
        debug_post_path = ""

    if "__VIEWSTATE" not in parser.fields or "__EVENTVALIDATION" not in parser.fields:
        print("Warning: missing __VIEWSTATE or __EVENTVALIDATION in product form.")

    body, _ = post_form(opener, product_url, parser, overrides, submit_name=name_submit_save)
    if "Đã lưu" not in body:
        debug_path = "/tmp/gh_admin_product_response.html"
        try:
            with open(debug_path, "w", encoding="utf-8") as fh:
                fh.write(body)
            if debug_post_path:
                print("Product create response did not contain success message. Saved to %s (post %s)" % (debug_path, debug_post_path))
            else:
                print("Product create response did not contain success message. Saved to %s" % debug_path)
        except Exception:
            print("Product create response did not contain success message.")

    print("Step: create invoice")
    invoice_url = f"{base_url}/gianhang/admin/hoa-don.aspx"
    parser, _, _ = fetch_form(opener, invoice_url)
    name_sanpham = find_name_by_id_suffix(parser, "ddl_sanpham")
    name_ten = find_name_by_id_suffix(parser, "txt_ten")
    name_sdt = find_name_by_id_suffix(parser, "txt_sdt")
    name_diachi = find_name_by_id_suffix(parser, "txt_diachi")
    name_ghichu = find_name_by_id_suffix(parser, "txt_ghichu")
    name_soluong = find_name_by_id_suffix(parser, "txt_soluong")
    name_submit_invoice = find_name_by_id_suffix(parser, "but_create")

    if not name_sanpham or not name_submit_invoice:
        print("Missing invoice form fields", file=sys.stderr)
        sys.exit(6)

    product_option = choose_option(parser.selects, name_sanpham, product_name)
    if not product_option:
        print("No product option available for invoice", file=sys.stderr)
        sys.exit(7)

    overrides = {
        name_sanpham: product_option,
        name_ten: invoice_name,
        name_sdt: invoice_phone,
        name_diachi: invoice_address,
        name_ghichu: invoice_note,
        name_soluong: invoice_qty
    }

    body, _ = post_form(opener, invoice_url, parser, overrides, submit_name=name_submit_invoice)
    if "Đã tạo hóa đơn" not in body:
        debug_path = "/tmp/gh_admin_invoice_response.html"
        try:
            with open(debug_path, "w", encoding="utf-8") as fh:
                fh.write(body)
            print("Invoice create response did not contain success message. Saved to %s" % debug_path)
        except Exception:
            print("Invoice create response did not contain success message.")

    print("PRODUCT_NAME=%s" % product_name)
    print("INVOICE_NAME=%s" % invoice_name)

if __name__ == "__main__":
    try:
        main()
    except Exception as exc:
        print("Admin flow HTTP test failed:", exc, file=sys.stderr)
        sys.exit(1)
