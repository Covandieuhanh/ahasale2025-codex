<%@ Page Title="Mở quyền không gian" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="mo-khong-gian.aspx.cs" Inherits="home_mo_khong_gian" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .space-access-shell {
            padding: 22px 0 44px;
        }

        .space-access-card {
            max-width: 920px;
            margin: 0 auto;
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 24px;
            box-shadow: 0 20px 52px rgba(15, 23, 42, .08);
            overflow: hidden;
        }

        .space-access-hero {
            padding: 28px;
            background: linear-gradient(135deg, #ecfeff 0%, #f0fdf4 52%, #fff7ed 100%);
            border-bottom: 1px solid rgba(15, 23, 42, .08);
        }

        .space-access-badge {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            min-height: 34px;
            padding: 0 14px;
            border-radius: 999px;
            background: rgba(37, 99, 235, .08);
            color: #1d4ed8;
            font-size: 13px;
            font-weight: 700;
        }

        .space-access-title {
            margin: 14px 0 6px;
            font-size: 34px;
            line-height: 1.08;
            font-weight: 800;
            color: #0f172a;
        }

        .space-access-summary {
            margin: 0;
            font-size: 15px;
            line-height: 1.7;
            color: #475569;
        }

        .space-access-body {
            padding: 24px 28px 28px;
        }

        .space-access-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 14px;
            margin-bottom: 18px;
        }

        .space-access-panel {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            background: #fff;
            padding: 18px;
        }

        .space-access-panel h3 {
            margin: 0 0 10px;
            font-size: 18px;
            line-height: 1.25;
            color: #0f172a;
        }

        .space-access-panel p,
        .space-access-panel li {
            color: #475569;
            font-size: 14px;
            line-height: 1.7;
        }

        .space-access-panel ul {
            margin: 0;
            padding-left: 18px;
        }

        .space-access-status {
            display: inline-flex;
            align-items: center;
            min-height: 38px;
            padding: 0 14px;
            border-radius: 999px;
            background: #eff6ff;
            color: #1d4ed8;
            font-size: 14px;
            font-weight: 700;
        }

        .space-access-status--ok {
            background: #ecfdf5;
            color: #047857;
        }

        .space-access-status--warn {
            background: #fff7ed;
            color: #c2410c;
        }

        .space-access-status--danger {
            background: #fef2f2;
            color: #b91c1c;
        }

        .space-access-actions {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
            margin-top: 18px;
        }

        .space-access-note {
            margin-top: 12px;
            padding: 14px 16px;
            border-radius: 16px;
            background: #f8fafc;
            border: 1px solid rgba(15, 23, 42, .08);
            color: #475569;
            font-size: 14px;
            line-height: 1.7;
        }

        .space-access-inline-form {
            margin-top: 14px;
            padding: 14px;
            border-radius: 16px;
            border: 1px solid rgba(15, 23, 42, .08);
            background: #f8fafc;
        }

        .space-access-inline-form label {
            margin: 0 0 8px;
            font-size: 13px;
            color: #334155;
            font-weight: 700;
            display: block;
        }

        .space-access-inline-form .form-text {
            margin-top: 8px;
            font-size: 12px;
            color: #64748b;
        }

        .space-workspace-list {
            margin-top: 20px;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            background: #fff;
            overflow: hidden;
        }

        .space-workspace-list__head {
            padding: 18px 20px;
            border-bottom: 1px solid rgba(15, 23, 42, .08);
            background: #f8fafc;
        }

        .space-workspace-list__title {
            margin: 0;
            font-size: 18px;
            line-height: 1.25;
            font-weight: 800;
            color: #0f172a;
        }

        .space-workspace-list__subtitle {
            margin: 8px 0 0;
            font-size: 14px;
            line-height: 1.7;
            color: #475569;
        }

        .space-workspace-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 14px;
            padding: 18px;
        }

        .space-workspace-card {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            background: #ffffff;
            padding: 18px;
            box-shadow: 0 8px 24px rgba(15, 23, 42, .04);
        }

        .space-workspace-card__badges {
            display: flex;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
            margin-bottom: 10px;
        }

        .space-workspace-card__badge {
            display: inline-flex;
            align-items: center;
            min-height: 30px;
            padding: 0 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            background: #ecfdf5;
            color: #047857;
        }

        .space-workspace-card__badge--member {
            background: #eff6ff;
            color: #1d4ed8;
        }

        .space-workspace-card__title {
            margin: 0;
            font-size: 18px;
            line-height: 1.35;
            font-weight: 800;
            color: #0f172a;
        }

        .space-workspace-card__meta {
            margin-top: 10px;
            font-size: 14px;
            line-height: 1.8;
            color: #475569;
        }

        .space-workspace-card__actions {
            margin-top: 16px;
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
        }

        .space-access-meta {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 12px;
            margin-top: 16px;
        }

        .space-access-meta-item {
            border-radius: 16px;
            background: #f8fafc;
            border: 1px solid rgba(15, 23, 42, .08);
            padding: 14px 16px;
        }

        .space-access-meta-label {
            font-size: 12px;
            color: #64748b;
            margin-bottom: 6px;
        }

        .space-access-meta-value {
            font-size: 16px;
            font-weight: 700;
            color: #0f172a;
            line-height: 1.5;
        }

        .space-access-usage-list {
            margin: 0;
            padding-left: 18px;
        }

        .space-access-usage-list li {
            color: #475569;
            font-size: 14px;
            line-height: 1.7;
            margin-bottom: 6px;
        }

        @media (max-width: 900px) {
            .space-access-grid,
            .space-access-meta,
            .space-workspace-grid {
                grid-template-columns: 1fr;
            }

            .space-access-title {
                font-size: 28px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl space-access-shell">
        <div class="space-access-card">
            <div class="space-access-hero">
                <div class="space-access-badge">Quyền không gian Home</div>
                <h1 class="space-access-title"><asp:Literal ID="lit_title" runat="server" /></h1>
                <p class="space-access-summary"><asp:Literal ID="lit_summary" runat="server" /></p>
                <div class="space-access-actions">
                    <span id="statusBadge" runat="server" class="space-access-status">
                        <asp:Literal ID="lit_access_status" runat="server" />
                    </span>
                    <asp:HyperLink ID="lnk_open_space_top" runat="server" CssClass="btn btn-success" Visible="false">Mở không gian</asp:HyperLink>
                    <asp:Button ID="btn_request_open_top" runat="server" CssClass="btn btn-primary" Text="Gửi yêu cầu mở quyền" OnClick="btn_request_open_Click" Visible="false" />
                    <asp:HyperLink ID="lnk_back_home_top" runat="server" CssClass="btn btn-outline-secondary" NavigateUrl="/home/default.aspx">Về trang chủ Home</asp:HyperLink>
                </div>
            </div>

            <div class="space-access-body">
                <div class="space-access-grid">
                    <div class="space-access-panel">
                        <h3>Luồng truy cập chuẩn</h3>
                        <ul>
                            <li>Đăng nhập bằng chính tài khoản Home của bạn.</li>
                            <li>Nếu chưa có quyền, gửi yêu cầu mở quyền tại màn hình này.</li>
                            <li>Admin xác nhận và bật đúng quyền không gian cho tài khoản Home.</li>
                            <li>Sau khi được mở quyền, quay lại đúng route để sử dụng ngay.</li>
                        </ul>
                    </div>
                    <div class="space-access-panel">
                        <h3>Admin mở ở đâu?</h3>
                        <p><asp:Literal ID="lit_admin_hint" runat="server" /></p>
                        <p class="mb-0"><asp:Literal ID="lit_request_hint" runat="server" /></p>
                    </div>
                    <div class="space-access-panel">
                        <h3>Quyền này mở ra những gì?</h3>
                        <ul class="space-access-usage-list">
                            <asp:Literal ID="lit_usage_items" runat="server" />
                        </ul>
                    </div>
                    <div class="space-access-panel">
                        <h3>Tên quyền trong admin</h3>
                        <p class="mb-0"><asp:Literal ID="lit_admin_toggle_label" runat="server" /></p>
                    </div>
                </div>

                <div class="space-access-meta">
                    <div class="space-access-meta-item">
                        <div class="space-access-meta-label">Tài khoản Home</div>
                        <div class="space-access-meta-value"><asp:Literal ID="lit_account" runat="server" /></div>
                    </div>
                    <div class="space-access-meta-item">
                        <div class="space-access-meta-label">Route truy cập</div>
                        <div class="space-access-meta-value"><asp:Literal ID="lit_route" runat="server" /></div>
                    </div>
                    <div class="space-access-meta-item">
                        <div class="space-access-meta-label">Yêu cầu gần nhất</div>
                        <div class="space-access-meta-value"><asp:Literal ID="lit_request_status" runat="server" /></div>
                    </div>
                </div>

                <asp:PlaceHolder ID="ph_shop_commission" runat="server" Visible="false">
                    <div class="space-access-inline-form">
                        <label for="<%= txt_shop_commission.ClientID %>">% chiết khấu cho sàn (0 - 100)</label>
                        <asp:TextBox ID="txt_shop_commission" runat="server" CssClass="form-control" TextMode="Number" Min="0" Max="100"></asp:TextBox>
                        <div class="form-text">
                            Đây là mức % mặc định sẽ áp dụng cho toàn bộ tin đăng trong không gian <strong>/shop</strong> sau khi admin duyệt.
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_workspace_list" runat="server" Visible="false">
                    <div class="space-workspace-list">
                        <div class="space-workspace-list__head">
                            <h3 class="space-workspace-list__title">Không gian bạn có thể truy cập</h3>
                            <p class="space-workspace-list__subtitle">
                                Đây là danh sách không gian <strong>/gianhang/admin</strong> mà tài khoản Home hiện đang làm chủ hoặc đang tham gia với vai trò nhân viên.
                            </p>
                        </div>
                        <div class="space-workspace-grid">
                            <asp:Literal ID="lit_workspace_cards" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_workspace_hint" runat="server" Visible="false">
                    <div class="space-access-note">
                        <asp:Literal ID="lit_workspace_hint" runat="server" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_request_time" runat="server" Visible="false">
                    <div class="space-access-note">
                        Ngày gửi yêu cầu gần nhất: <strong><asp:Literal ID="lit_requested_at" runat="server" /></strong>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_review_note" runat="server" Visible="false">
                    <div class="space-access-note">
                        Ghi chú xử lý: <asp:Literal ID="lit_review_note" runat="server" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_lock_reason" runat="server" Visible="false">
                    <div class="space-access-note">
                        Lý do khóa / thu hồi: <asp:Literal ID="lit_lock_reason" runat="server" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:Content>
