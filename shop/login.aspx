<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="shop_login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Đăng Nhập Shop</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body { font-family: Arial, sans-serif; background: #f4f7fb; margin: 0; }
        .wrap { max-width: 420px; margin: 48px auto; background: #fff; border: 1px solid #dbe4ef; border-radius: 12px; padding: 20px; }
        h2 { margin: 0 0 14px; color: #1f3d5b; }
        label { display: block; margin: 10px 0 6px; color: #2f4c69; font-weight: 600; }
        .input { width: 100%; box-sizing: border-box; min-height: 42px; border: 1px solid #c8d7e8; border-radius: 10px; padding: 0 12px; }
        .btn { margin-top: 14px; width: 100%; min-height: 42px; border: 0; border-radius: 10px; background: #0f4c81; color: #fff; font-weight: 700; cursor: pointer; }
        .msg { margin-top: 10px; color: #b42318; min-height: 22px; }
        .foot { margin-top: 16px; text-align: center; }
        .foot a { color: #0f4c81; font-weight: 600; text-decoration: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h2>Đăng nhập trang Shop</h2>
            <label for="txt_user">Email đăng nhập</label>
            <asp:TextBox ID="txt_user" runat="server" CssClass="input" MaxLength="120"></asp:TextBox>
            <label for="txt_pass">Mật khẩu</label>
            <asp:TextBox ID="txt_pass" runat="server" CssClass="input" TextMode="Password" MaxLength="120"></asp:TextBox>
            <asp:Button ID="but_login" runat="server" CssClass="btn" Text="Đăng nhập" OnClick="but_login_Click" />
            <div class="msg">
                <asp:Label ID="lb_msg" runat="server" Text=""></asp:Label>
            </div>
            <div class="foot">
                <a href="/shop/dang-ky.aspx">Đăng ký tài khoản shop</a>
            </div>
        </div>
    </form>
</body>
</html>
