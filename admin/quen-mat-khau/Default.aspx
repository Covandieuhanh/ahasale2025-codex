<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_quen_mat_khau_Default" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <title>Quên mật khẩu admin</title>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/Metro-UI-CSS-master/css/icons.css" rel="stylesheet" />
    <link href="/assetscss/admin-style.css" rel="stylesheet" />
    <link href="/assetscss/bcorn-with-metro.css" rel="stylesheet" />
    <link href="/assetscss/fix-metro.css" rel="stylesheet" />
    <link href="/assetscss/admin-clean.css?v=2026-03-03.12" rel="stylesheet" />
    <link href="/assetscss/admin-home-sync.css?v=2026-03-20.4" rel="stylesheet" />
    <style>
        body.admin-forgot-page {
            min-height: 100vh;
            background:
                radial-gradient(circle at top, rgba(248, 113, 113, 0.18), transparent 34%),
                linear-gradient(180deg, #fff5f5 0%, #eef4f9 55%, #ffffff 100%);
            color: #123148;
        }

        .admin-forgot-shell {
            max-width: 760px;
            margin: 0 auto;
            padding: 28px 16px 48px;
        }

        .admin-forgot-hero {
            padding: 22px 24px;
            border: 1px solid #dce7f2;
            border-radius: 24px;
            background: linear-gradient(135deg, #fff1f2 0%, #fff 58%, #f7fbff 100%);
            box-shadow: 0 18px 38px rgba(15, 41, 64, 0.08);
        }

        .admin-forgot-eyebrow {
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .14em;
            text-transform: uppercase;
            color: #c81e1e;
        }

        .admin-forgot-title {
            margin: 10px 0 8px;
            font-size: 34px;
            line-height: 1.08;
        }

        .admin-forgot-desc {
            margin: 0;
            color: #6b8195;
            font-size: 15px;
            line-height: 1.7;
        }

        .admin-forgot-card {
            margin-top: 18px;
            border: 1px solid #dce7f2;
            border-radius: 24px;
            background: #fff;
            box-shadow: 0 16px 34px rgba(15, 41, 64, 0.08);
            overflow: hidden;
        }

        .admin-forgot-card-head {
            padding: 18px 20px;
            border-bottom: 1px solid #e7eef5;
            background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
        }

        .admin-forgot-card-head h2 {
            margin: 0;
            font-size: 24px;
            color: #123148;
        }

        .admin-forgot-card-head p {
            margin: 6px 0 0;
            color: #6b8195;
            font-size: 13px;
            line-height: 1.6;
        }

        .admin-forgot-card-body {
            padding: 20px;
        }

        .admin-forgot-message {
            margin-bottom: 16px;
            padding: 14px 16px;
            border-radius: 18px;
            border: 1px solid #dce7f2;
            background: #f7fbff;
            color: #23435d;
            font-weight: 700;
        }

        .admin-forgot-message.error {
            border-color: #fecaca;
            background: #fff1f2;
            color: #b91c1c;
        }

        .admin-forgot-message.success {
            border-color: #bbf7d0;
            background: #f0fdf4;
            color: #166534;
        }

        .admin-forgot-field {
            margin-bottom: 16px;
        }

        .admin-forgot-field label {
            display: block;
            margin-bottom: 8px;
            font-size: 14px;
            font-weight: 800;
            color: #123148;
        }

        .admin-forgot-help {
            margin: 14px 0 0;
            color: #788da0;
            font-size: 13px;
            line-height: 1.7;
        }

        .admin-forgot-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            justify-content: flex-end;
            align-items: center;
            margin-top: 24px;
        }

        .admin-forgot-back {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 18px;
            border-radius: 999px;
            border: 1px solid #dce7f2;
            background: #fff;
            color: #123148 !important;
            text-decoration: none !important;
            font-weight: 800;
        }

        .admin-forgot-back:hover {
            background: #f8fbff;
            border-color: #c7d8e8;
        }

        .admin-forgot-submit.button {
            min-width: 220px;
            min-height: 46px;
            padding: 0 22px;
            border-radius: 999px !important;
            font-weight: 800 !important;
        }

        @media (max-width: 640px) {
            .admin-forgot-shell {
                padding: 12px 10px 32px;
            }

            .admin-forgot-title {
                font-size: 28px;
            }

            .admin-forgot-card-head,
            .admin-forgot-card-body,
            .admin-forgot-hero {
                padding: 16px;
            }

            .admin-forgot-actions > * {
                width: 100%;
            }
        }
    </style>
</head>
<body class="admin-forgot-page">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>
        <div class="admin-forgot-shell">
            <section class="admin-forgot-hero">
                <div class="admin-forgot-eyebrow">ADMIN AHASALE</div>
                <h1 class="admin-forgot-title">Gửi yêu cầu khôi phục mật khẩu</h1>
                <p class="admin-forgot-desc">
                    Trang này được tách riêng thành URL độc lập để mọi thao tác quên mật khẩu chạy mượt hơn và đồng nhất với luồng full-page của cổng admin.
                </p>
            </section>

            <section class="admin-forgot-card">
                <div class="admin-forgot-card-head">
                    <h2>Nhận email khôi phục</h2>
                    <p>Nhập đúng email đang gắn với tài khoản admin. Hệ thống sẽ gửi link đặt lại mật khẩu nếu email này tồn tại hợp lệ trong cổng admin.</p>
                </div>
                <div class="admin-forgot-card-body">
                    <asp:Panel ID="pnMessage" runat="server" Visible="false" CssClass="admin-forgot-message">
                        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                    </asp:Panel>

                    <div class="admin-forgot-field">
                        <label for="<%= txtEmail.ClientID %>">Email khôi phục</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" MaxLength="100" placeholder="Nhập email đang dùng cho tài khoản admin"></asp:TextBox>
                    </div>

                    <p class="admin-forgot-help">
                        Nếu một email đang trùng nhiều tài khoản admin, hệ thống sẽ từ chối và yêu cầu bạn liên hệ quản trị để xử lý trước.
                    </p>

                    <div class="admin-forgot-actions">
                        <asp:HyperLink ID="hlBack" runat="server" CssClass="admin-forgot-back">Quay lại đăng nhập</asp:HyperLink>
                        <asp:Button ID="butSend" runat="server" Text="Gửi link khôi phục" CssClass="button success admin-forgot-submit" OnClick="butSend_Click" />
                    </div>
                </div>
            </section>
        </div>
        <%= ViewState["thongbao"] %>
    </form>
    <script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
    <script src="/js/aha-ui-refresh.js?v=2026-03-07.2"></script>
</body>
</html>
