<%@ Control Language="C#" AutoEventWireup="true" CodeFile="space_nav.ascx.cs" Inherits="gianhang_uc_space_nav" %>

<asp:PlaceHolder ID="ph_nav" runat="server" Visible="false">
    <style>
        .gh-space-nav-shell {
            max-width: 1220px;
            margin: 14px auto 0;
            padding: 0 16px;
        }

        .gh-space-nav-card {
            background: #fff;
            border: 1px solid #d9e2ec;
            border-radius: 22px;
            box-shadow: 0 14px 32px rgba(16, 42, 67, .08);
            padding: 16px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            flex-wrap: wrap;
        }

        .gh-space-nav-brand {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 0;
        }

        .gh-space-nav-logo {
            width: 48px;
            height: 48px;
            border-radius: 15px;
            background: linear-gradient(145deg, #f97316, #fb923c);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 10px 24px rgba(249, 115, 22, .22);
            overflow: hidden;
            flex: 0 0 auto;
        }

        .gh-space-nav-logo img {
            width: 28px;
            height: 28px;
            object-fit: contain;
        }

        .gh-space-nav-title {
            font-size: 22px;
            line-height: 1.1;
            font-weight: 800;
            color: #102a43;
        }

        .gh-space-nav-sub {
            margin-top: 3px;
            color: #627d98;
            font-size: 13px;
        }

        .gh-space-nav-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
            justify-content: flex-end;
        }

        .gh-space-nav-pill {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            background: #fff7ed;
            border: 1px solid rgba(249,115,22,.18);
            color: #c2410c;
            font-size: 12px;
            font-weight: 800;
        }

        .gh-space-nav-avatar {
            width: 42px;
            height: 42px;
            border-radius: 50%;
            object-fit: cover;
            border: 1px solid #dce7f2;
            background: #f3f6fa;
            flex: 0 0 auto;
        }

        .gh-space-nav-link {
            min-height: 38px;
            border-radius: 999px;
            padding: 0 16px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 800;
            border: 1px solid #f97316;
            background: #fff7ed;
            color: #c2410c;
            text-decoration: none;
            transition: .18s ease;
        }

        .gh-space-nav-link:hover {
            background: #ffedd5;
            border-color: #ea580c;
            color: #9a3412;
        }

        .gh-space-nav-link.is-active {
            background: #f97316;
            border-color: #f97316;
            color: #fff;
            box-shadow: 0 8px 18px rgba(249, 115, 22, .18);
        }

        .gh-space-nav-link.is-solid {
            background: #f97316;
            border-color: #f97316;
            color: #fff;
        }

        .gh-space-nav-link.is-solid:hover {
            background: #ea580c;
            border-color: #ea580c;
            color: #fff;
        }

        @media (max-width: 767px) {
            .gh-space-nav-shell {
                margin-top: 10px;
                padding: 0 12px;
            }

            .gh-space-nav-card {
                padding: 14px;
                gap: 14px;
            }

            .gh-space-nav-title {
                font-size: 18px;
            }

            .gh-space-nav-actions {
                justify-content: flex-start;
            }
        }
    </style>

    <div class="gh-space-nav-shell">
        <div class="gh-space-nav-card">
            <div class="gh-space-nav-brand">
                <span class="gh-space-nav-logo">
                    <img src="/uploads/images/favicon.png" alt="AhaSale" />
                </span>
                <div>
                    <div class="gh-space-nav-title">
                        <asp:Literal ID="lit_store_name" runat="server" />
                    </div>
                    <div class="gh-space-nav-sub">
                        Tài khoản gốc: <asp:Literal ID="lit_account_key" runat="server" />
                        • Trạng thái: <asp:Literal ID="lit_status" runat="server" />
                    </div>
                </div>
            </div>

            <div class="gh-space-nav-actions">
                <span class="gh-space-nav-pill">Không gian /gianhang mở rộng từ /home</span>
                <asp:Image ID="img_avatar" runat="server" CssClass="gh-space-nav-avatar" />
                <asp:HyperLink ID="lnk_public" runat="server" CssClass="gh-space-nav-link" Target="_blank">Trang công khai</asp:HyperLink>
                <asp:HyperLink ID="lnk_home" runat="server" CssClass="gh-space-nav-link">Về Home</asp:HyperLink>
                <asp:HyperLink ID="lnk_manage" runat="server" CssClass="gh-space-nav-link">Quản lý tin</asp:HyperLink>
                <asp:HyperLink ID="lnk_orders" runat="server" CssClass="gh-space-nav-link">Đơn bán</asp:HyperLink>
                <asp:HyperLink ID="lnk_booking" runat="server" CssClass="gh-space-nav-link">Lịch hẹn</asp:HyperLink>
                <asp:HyperLink ID="lnk_customers" runat="server" CssClass="gh-space-nav-link">Khách hàng</asp:HyperLink>
                <asp:HyperLink ID="lnk_report" runat="server" CssClass="gh-space-nav-link">Báo cáo</asp:HyperLink>
                <asp:HyperLink ID="lnk_level2" runat="server" CssClass="gh-space-nav-link">Level 2</asp:HyperLink>
                <asp:PlaceHolder ID="ph_admin" runat="server" Visible="false">
                    <asp:HyperLink ID="lnk_admin" runat="server" CssClass="gh-space-nav-link is-solid">Mở /gianhang/admin</asp:HyperLink>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
