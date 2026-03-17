#!/usr/bin/env python3
import os
import ssl
import urllib.request
import urllib.parse
import urllib.error
import http.cookiejar
import re
from html.parser import HTMLParser

class FormParser(HTMLParser):
    def __init__(self):
        super().__init__()
        self.fields = {}
        self.id_to_name = {}

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
        elif tag == "select":
            name = attr.get("name", "")
            select_id = attr.get("id", "")
            if select_id and name:
                self.id_to_name[select_id] = name


def find_name_by_id_suffix(parser, suffix):
    suffix = suffix.lower()
    for input_id, name in parser.id_to_name.items():
        if input_id.lower().endswith(suffix):
            return name
    return ""


def fetch(opener, url):
    resp = opener.open(url)
    return resp.read().decode("utf-8", "ignore")


def fetch_form(opener, url):
    html = fetch(opener, url)
    parser = FormParser()
    parser.feed(html)
    return parser, html


def main():
    base_url = os.environ.get("BASE_URL", "https://ahasale.local")
    shop = os.environ.get("SHOP_TK", "gh_demo")
    order_name = os.environ.get("ORDER_NAME", "Khach test cart")
    order_phone = os.environ.get("ORDER_PHONE", "0900000002")
    order_address = os.environ.get("ORDER_ADDRESS", "AhaSale local")
    order_note = os.environ.get("ORDER_NOTE", "Test cart")
    order_qty = int(os.environ.get("ORDER_QTY", "1"))

    cj = http.cookiejar.CookieJar()
    ctx = ssl.create_default_context()
    ctx.check_hostname = False
    ctx.verify_mode = ssl.CERT_NONE

    opener = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(cj))
    opener.addheaders = [("User-Agent", "AhaSaleTest/1.0")]

    list_url = f"{base_url}/gianhang/default.aspx?user={urllib.parse.quote(shop)}"
    html = fetch(opener, list_url)
    match = re.search(r'data-product-id="(\d+)"[^>]*data-loai="sanpham"', html)
    if not match:
        raise RuntimeError("No sanpham found in gianhang listing")
    product_id = match.group(1)

    add_url = f"{base_url}/gianhang/giohang.aspx?user={urllib.parse.quote(shop)}&id={product_id}&sl={order_qty}"
    fetch(opener, add_url)

    cart_url = f"{base_url}/gianhang/giohang.aspx?user={urllib.parse.quote(shop)}"
    parser, html = fetch_form(opener, cart_url)

    data = dict(parser.fields)

    name_field = find_name_by_id_suffix(parser, "txt_ten")
    phone_field = find_name_by_id_suffix(parser, "txt_sdt")
    address_field = find_name_by_id_suffix(parser, "txt_diachi")
    note_field = find_name_by_id_suffix(parser, "txt_ghichu")
    submit_field = find_name_by_id_suffix(parser, "but_dathang")

    if not name_field or not phone_field or not address_field or not submit_field:
        raise RuntimeError("Cannot locate checkout form fields")

    data[name_field] = order_name
    data[phone_field] = order_phone
    data[address_field] = order_address
    if note_field:
        data[note_field] = order_note

    # ensure quantity field exists
    qty_field = None
    for key in data.keys():
        if key.startswith("sl_"):
            qty_field = key
            break
    if qty_field:
        data[qty_field] = str(order_qty)

    data[submit_field] = data.get(submit_field, "Xác nhận đặt hàng")

    payload = urllib.parse.urlencode(data).encode("utf-8")
    req = urllib.request.Request(cart_url, data=payload, method="POST")
    opener.open(req).read()


if __name__ == "__main__":
    main()
