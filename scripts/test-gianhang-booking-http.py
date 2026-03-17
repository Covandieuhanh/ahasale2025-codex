#!/usr/bin/env python3
import os
import ssl
import sys
import time
import urllib.parse
import urllib.request
import urllib.error
import http.cookiejar
from html.parser import HTMLParser
from datetime import datetime

class FormParser(HTMLParser):
    def __init__(self):
        super().__init__()
        self.fields = {}
        self.textareas = {}
        self.id_to_name = {}
        self.selects = {}
        self._current_textarea = None
        self._current_select = None

    def handle_starttag(self, tag, attrs):
        attr = {k.lower(): (v or "") for k, v in attrs}
        if tag == "input":
            name = attr.get("name", "")
            if name:
                self.fields[name] = attr.get("value", "")
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
                text = attr.get("label", "")
                self.selects[self._current_select].append((value, text))

    def handle_endtag(self, tag):
        if tag == "textarea":
            self._current_textarea = None
        elif tag == "select":
            self._current_select = None

    def handle_data(self, data):
        if self._current_textarea is not None:
            self.textareas[self._current_textarea] += data


def find_name_by_id_suffix(parser, suffix):
    suffix = suffix.lower()
    for input_id, name in parser.id_to_name.items():
        if input_id.lower().endswith(suffix):
            return name
    return ""


def fetch_form(opener, url):
    resp = opener.open(url)
    html = resp.read().decode("utf-8", "ignore")
    parser = FormParser()
    parser.feed(html)
    return parser, html


def main():
    base_url = os.environ.get("BASE_URL", "https://ahasale.local").rstrip("/")
    shop_tk = os.environ.get("SHOP_TK", "gh_demo")
    booking_name = os.environ.get("BOOKING_NAME", "Test Booking %d" % int(time.time()))
    booking_phone = os.environ.get("BOOKING_PHONE", "0909990001")
    booking_service = os.environ.get("BOOKING_SERVICE", "Test Service")
    booking_note = os.environ.get("BOOKING_NOTE", "Test booking from script")
    booking_date = os.environ.get("BOOKING_DATE") or datetime.now().strftime("%Y-%m-%d")
    booking_time = os.environ.get("BOOKING_TIME") or "09:00"

    target_url = f"{base_url}/gianhang/dat-lich.aspx?user={urllib.parse.quote(shop_tk)}"

    cj = http.cookiejar.CookieJar()
    ctx = ssl._create_unverified_context()
    opener = urllib.request.build_opener(
        urllib.request.HTTPCookieProcessor(cj),
        urllib.request.HTTPSHandler(context=ctx)
    )
    opener.addheaders = [("User-Agent", "AhaSaleTest/1.0")]

    parser, _ = fetch_form(opener, target_url)

    name_txt_ten = find_name_by_id_suffix(parser, "txt_ten")
    name_txt_sdt = find_name_by_id_suffix(parser, "txt_sdt")
    name_ddl_dichvu = find_name_by_id_suffix(parser, "ddl_dichvu")
    name_txt_dichvu_khac = find_name_by_id_suffix(parser, "txt_dichvu_khac")
    name_txt_ngay = find_name_by_id_suffix(parser, "txt_ngay")
    name_txt_gio = find_name_by_id_suffix(parser, "txt_gio")
    name_txt_ghichu = find_name_by_id_suffix(parser, "txt_ghichu")
    name_submit = find_name_by_id_suffix(parser, "but_submit")

    if not name_txt_ten or not name_txt_sdt or not name_submit:
        print("Missing form fields for booking.", file=sys.stderr)
        sys.exit(2)

    fields = dict(parser.fields)
    fields.update(parser.textareas)

    fields[name_txt_ten] = booking_name
    fields[name_txt_sdt] = booking_phone
    if name_ddl_dichvu:
        options = parser.selects.get(name_ddl_dichvu, [])
        selected = ""
        for value, _ in options:
            if value:
                selected = value
                break
        if selected:
            fields[name_ddl_dichvu] = selected
        elif name_txt_dichvu_khac:
            fields[name_txt_dichvu_khac] = booking_service
    elif name_txt_dichvu_khac:
        fields[name_txt_dichvu_khac] = booking_service
    if name_txt_ngay:
        fields[name_txt_ngay] = booking_date
    if name_txt_gio:
        fields[name_txt_gio] = booking_time
    if name_txt_ghichu:
        fields[name_txt_ghichu] = booking_note
    fields[name_submit] = fields.get(name_submit, "Gửi lịch hẹn")

    data = urllib.parse.urlencode(fields).encode("utf-8")
    req = urllib.request.Request(target_url, data=data, method="POST")
    try:
        resp = opener.open(req)
        body = resp.read().decode("utf-8", "ignore")
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", "ignore")
        print("HTTP Error %s" % exc.code, file=sys.stderr)
        print(body[:1200], file=sys.stderr)
        raise

    if "Đã gửi lịch hẹn" not in body and "Đã gửi lịch" not in body:
        print("Booking submit response did not contain success message.")
    else:
        print("Booking submit response indicates success.")

    print("BOOKING_NAME=%s" % booking_name)

if __name__ == "__main__":
    try:
        main()
    except Exception as exc:
        print("Booking HTTP test failed:", exc, file=sys.stderr)
        sys.exit(1)
