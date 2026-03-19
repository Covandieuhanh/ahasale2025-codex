<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dang-ky.aspx.cs" Inherits="shop_dang_ky" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Đăng Ký Shop</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body { font-family: Arial, sans-serif; background: #f4f7fb; margin: 0; }
        .wrap { max-width: 480px; margin: 42px auto; background: #fff; border: 1px solid #dbe4ef; border-radius: 12px; padding: 20px; }
        h2 { margin: 0 0 14px; color: #1f3d5b; }
        label { display: block; margin: 10px 0 6px; color: #2f4c69; font-weight: 600; }
        .input { width: 100%; box-sizing: border-box; min-height: 42px; border: 1px solid #c8d7e8; border-radius: 10px; padding: 0 12px; }
        .btn { margin-top: 16px; width: 100%; min-height: 42px; border: 0; border-radius: 10px; background: #ee4d2d; color: #fff; font-weight: 700; cursor: pointer; }
        .msg { margin-top: 10px; color: #b42318; min-height: 22px; }
        .foot { margin-top: 16px; text-align: center; }
        .foot a { color: #ee4d2d; font-weight: 600; text-decoration: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h2>Đăng ký tài khoản gian hàng đối tác</h2>
            <label for="txt_email">Email (dùng để đăng nhập)</label>
            <asp:TextBox ID="txt_email" runat="server" CssClass="input" MaxLength="120"></asp:TextBox>
            <label for="txt_matkhau">Mật khẩu</label>
            <div class="aha-password-field">
                <asp:TextBox ID="txt_matkhau" runat="server" CssClass="input js-password" TextMode="Password" MaxLength="120"></asp:TextBox>
                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu">
                    <span class="aha-password-toggle-label">Hiện</span>
                </button>
            </div>
            <label for="txt_hoten">Họ tên / tên gian hàng đối tác</label>
            <asp:TextBox ID="txt_hoten" runat="server" CssClass="input" MaxLength="120"></asp:TextBox>
            <label for="txt_dienthoai">Số điện thoại</label>
            <asp:TextBox ID="txt_dienthoai" runat="server" CssClass="input" MaxLength="20"></asp:TextBox>
            <asp:Button ID="but_dangky" runat="server" CssClass="btn" Text="Gửi đăng ký gian hàng đối tác" OnClientClick="return AhaPreventDoubleClick(this);" OnClick="but_dangky_Click" />
            <div class="msg">
                <asp:Label ID="lb_msg" runat="server"></asp:Label>
            </div>
            <div class="foot">
                <a href="/shop/login.aspx">Đã có tài khoản gian hàng đối tác? Đăng nhập</a>
            </div>
        </div>
    </form>
    <script src="/js/aha-ui-refresh.js?v=2026-03-10.1"></script>
    <script>
        window.AhaPreventDoubleClick = window.AhaPreventDoubleClick || function (btn) {
            try {
                if (!btn) return true;
                if (btn.dataset && btn.dataset.locked === "1") return false;
                if (btn.dataset) btn.dataset.locked = "1";
                btn.disabled = true;
                btn.classList.add("aha-btn-loading");
                return true;
            } catch (e) { return true; }
        };
    </script>
</body>
</html>
