<%@ Page Title="Đổi mật khẩu" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_doi_mat_khau_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .admin-password-page {
            max-width: 760px;
            margin: 0 auto;
            padding: 12px 16px 48px;
        }

        .admin-password-hero {
            border: 1px solid #dce7f2;
            border-radius: 24px;
            padding: 20px 22px;
            background: linear-gradient(135deg, #fff4f4 0%, #ffffff 52%, #f7fbff 100%);
            box-shadow: 0 18px 40px rgba(15, 41, 64, 0.08);
        }

        .admin-password-eyebrow {
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .14em;
            color: #c81e1e;
            text-transform: uppercase;
        }

        .admin-password-title {
            margin: 10px 0 8px;
            font-size: 34px;
            line-height: 1.08;
            color: #123148;
        }

        .admin-password-desc {
            margin: 0;
            color: #657c90;
            font-size: 15px;
            line-height: 1.7;
        }

        .admin-password-card {
            margin-top: 18px;
            border: 1px solid #dce7f2;
            border-radius: 22px;
            background: #fff;
            box-shadow: 0 14px 36px rgba(15, 41, 64, 0.08);
            overflow: hidden;
        }

        .admin-password-card-head {
            display: flex;
            justify-content: space-between;
            gap: 16px;
            align-items: flex-start;
            padding: 18px 20px;
            border-bottom: 1px solid #e7eef5;
            background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
        }

        .admin-password-card-title {
            margin: 0;
            font-size: 24px;
            color: #123148;
        }

        .admin-password-card-note {
            margin: 6px 0 0;
            color: #6a8194;
            font-size: 13px;
            line-height: 1.6;
        }

        .admin-password-card-body {
            padding: 20px;
        }

        .admin-password-message {
            margin-bottom: 16px;
            padding: 14px 16px;
            border-radius: 18px;
            border: 1px solid #dce7f2;
            background: #f7fbff;
            color: #23435d;
            font-weight: 700;
        }

        .admin-password-message.error {
            border-color: #fecaca;
            background: #fff1f2;
            color: #b91c1c;
        }

        .admin-password-message.success {
            border-color: #bbf7d0;
            background: #f0fdf4;
            color: #166534;
        }

        .admin-password-field-wrap {
            margin-bottom: 16px;
        }

        .admin-password-field-wrap label {
            display: block;
            margin-bottom: 8px;
            font-size: 14px;
            font-weight: 800;
            color: #123148;
        }

        .admin-password-hint {
            margin: 12px 0 0;
            color: #7b8da0;
            font-size: 13px;
            line-height: 1.6;
        }

        .admin-password-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            justify-content: flex-end;
            align-items: center;
            margin-top: 24px;
        }

        .admin-password-back-link {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 18px;
            border-radius: 999px;
            border: 1px solid #dce7f2;
            background: #fff;
            color: #123148 !important;
            font-weight: 800;
            text-decoration: none !important;
        }

        .admin-password-back-link:hover {
            background: #f8fbff;
            border-color: #c4d7e7;
        }

        .admin-password-submit.button {
            min-width: 190px;
            min-height: 46px;
            padding: 0 22px;
            border-radius: 999px !important;
            font-weight: 800 !important;
        }

        @media (max-width: 640px) {
            .admin-password-page {
                padding: 8px 10px 32px;
            }

            .admin-password-title {
                font-size: 28px;
            }

            .admin-password-card-head,
            .admin-password-card-body {
                padding: 16px;
            }

            .admin-password-actions {
                justify-content: stretch;
            }

            .admin-password-actions > * {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="admin-password-page">
        <section class="admin-password-hero">
            <div class="admin-password-eyebrow">TÀI KHOẢN ADMIN</div>
            <h1 class="admin-password-title">Đổi mật khẩu đăng nhập</h1>
            <p class="admin-password-desc">
                Màn này được tách riêng thành URL độc lập để thao tác mượt và rõ ràng hơn. Sau khi đổi mật khẩu thành công, hệ thống sẽ yêu cầu đăng nhập lại để bảo mật phiên làm việc.
            </p>
        </section>

        <section class="admin-password-card">
            <div class="admin-password-card-head">
                <div>
                    <h2 class="admin-password-card-title">Cập nhật bảo mật</h2>
                    <p class="admin-password-card-note">
                        Tài khoản: <strong><asp:Label ID="lbTaiKhoan" runat="server" /></strong>
                    </p>
                </div>
            </div>

            <div class="admin-password-card-body">
                <asp:Panel ID="pnMessage" runat="server" Visible="false" CssClass="admin-password-message">
                    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="admin-password-field-wrap">
                    <label for="<%= txtPasswordOld.ClientID %>">Mật khẩu hiện tại</label>
                    <div class="aha-password-field">
                        <asp:TextBox ID="txtPasswordOld" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                        <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu hiện tại">
                            <span class="aha-password-toggle-label">Hiện</span>
                        </button>
                    </div>
                </div>

                <div class="admin-password-field-wrap">
                    <label for="<%= txtPasswordNew.ClientID %>">Mật khẩu mới</label>
                    <div class="aha-password-field">
                        <asp:TextBox ID="txtPasswordNew" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                        <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu mới">
                            <span class="aha-password-toggle-label">Hiện</span>
                        </button>
                    </div>
                </div>

                <div class="admin-password-field-wrap">
                    <label for="<%= txtPasswordConfirm.ClientID %>">Nhập lại mật khẩu mới</label>
                    <div class="aha-password-field">
                        <asp:TextBox ID="txtPasswordConfirm" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                        <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện xác nhận mật khẩu mới">
                            <span class="aha-password-toggle-label">Hiện</span>
                        </button>
                    </div>
                </div>

                <p class="admin-password-hint">
                    Lưu ý: tài khoản admin sau khi đổi mật khẩu sẽ đăng xuất ngay, vì vậy chỉ nên thao tác khi bạn sẵn sàng đăng nhập lại.
                </p>

                <div class="admin-password-actions">
                    <asp:HyperLink ID="hlBack" runat="server" CssClass="admin-password-back-link">Quay lại</asp:HyperLink>
                    <asp:Button ID="butSave" runat="server" Text="Đổi mật khẩu" CssClass="button success admin-password-submit" OnClick="butSave_Click" />
                </div>
            </div>
        </section>
    </div>
</asp:Content>
