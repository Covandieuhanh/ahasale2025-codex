<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dang-ky.aspx.cs" Inherits="shop_dang_ky" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Đang chuyển hướng đăng ký Shop</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body { margin: 0; font-family: Arial, sans-serif; background: #f8fafc; color: #0f172a; }
        .wrap { max-width: 680px; margin: 44px auto; background: #fff; border: 1px solid #e2e8f0; border-radius: 14px; padding: 24px; }
        h1 { margin: 0 0 12px; font-size: 24px; }
        p { margin: 0 0 14px; line-height: 1.6; color: #475569; }
        a { color: #0ea5e9; text-decoration: none; font-weight: 700; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h1>Đang chuyển sang luồng mở không gian Shop</h1>
            <p>Hệ thống đang dùng luồng đăng ký mới: tài khoản Home mở quyền <strong>/shop</strong> kèm % chiết khấu tại màn hình quản lý không gian.</p>
            <p>Nếu chưa tự chuyển, vui lòng vào: <a href="/home/mo-khong-gian.aspx?space=shop&return_url=%2Fshop%2Fdefault.aspx">/home/mo-khong-gian.aspx?space=shop</a></p>
        </div>
    </form>
</body>
</html>
