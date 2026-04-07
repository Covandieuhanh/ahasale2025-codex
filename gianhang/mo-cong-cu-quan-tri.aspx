<%@ Page Title="Mở công cụ quản trị gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerGianHang.master" AutoEventWireup="true" CodeFile="mo-cong-cu-quan-tri.aspx.cs" Inherits="gianhang_mo_cong_cu_quan_tri" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .gh-admin-open-shell {
            padding: 24px 0 48px;
            background:
                radial-gradient(920px 240px at 10% -6%, rgba(249,115,22,.14), transparent 62%),
                radial-gradient(820px 220px at 88% -8%, rgba(251,146,60,.12), transparent 58%),
                #f8fafc;
            min-height: 100vh;
        }

        .gh-admin-open-card {
            max-width: 980px;
            margin: 0 auto;
            background: #fff;
            border: 1px solid #e2e8f0;
            border-radius: 24px;
            box-shadow: 0 22px 54px rgba(15, 23, 42, .08);
            overflow: hidden;
        }

        .gh-admin-open-hero {
            padding: 30px 32px;
            background: linear-gradient(135deg, #fff7ed 0%, #ffedd5 48%, #fff 100%);
            border-bottom: 1px solid #e2e8f0;
        }

        .gh-admin-open-badge {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            min-height: 34px;
            padding: 0 14px;
            border-radius: 999px;
            background: rgba(249,115,22,.12);
            color: #c2410c;
            font-size: 13px;
            font-weight: 800;
        }

        .gh-admin-open-title {
            margin: 14px 0 6px;
            font-size: 34px;
            line-height: 1.08;
            font-weight: 800;
            color: #0f172a;
        }

        .gh-admin-open-summary {
            margin: 0;
            color: #475569;
            font-size: 15px;
            line-height: 1.7;
        }

        .gh-admin-open-actions {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
            margin-top: 18px;
        }

        .gh-admin-open-status {
            display: inline-flex;
            align-items: center;
            min-height: 38px;
            padding: 0 14px;
            border-radius: 999px;
            background: #fff7ed;
            color: #c2410c;
            font-size: 14px;
            font-weight: 700;
        }

        .gh-admin-open-status--ok {
            background: #ecfdf5;
            color: #047857;
        }

        .gh-admin-open-status--warn {
            background: #fff7ed;
            color: #c2410c;
        }

        .gh-admin-open-status--danger {
            background: #fef2f2;
            color: #b91c1c;
        }

        .gh-admin-open-body {
            padding: 24px 32px 30px;
        }

        .gh-admin-open-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 14px;
        }

        .gh-admin-open-panel {
            border: 1px solid #e2e8f0;
            border-radius: 18px;
            padding: 18px;
            background: #fff;
        }

        .gh-admin-open-panel h3 {
            margin: 0 0 10px;
            font-size: 18px;
            line-height: 1.2;
            color: #0f172a;
        }

        .gh-admin-open-panel p,
        .gh-admin-open-panel li {
            color: #475569;
            font-size: 14px;
            line-height: 1.7;
        }

        .gh-admin-open-panel ul {
            margin: 0;
            padding-left: 18px;
        }

        .gh-admin-open-meta {
            margin-top: 18px;
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 12px;
        }

        .gh-admin-open-meta-item {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 14px 16px;
        }

        .gh-admin-open-meta-label {
            font-size: 12px;
            color: #64748b;
            margin-bottom: 6px;
        }

        .gh-admin-open-meta-value {
            font-size: 16px;
            line-height: 1.5;
            font-weight: 700;
            color: #0f172a;
            word-break: break-word;
        }

        .gh-admin-open-note {
            margin-top: 14px;
            padding: 14px 16px;
            border-radius: 16px;
            border: 1px solid #fed7aa;
            background: #fff7ed;
            color: #9a3412;
            font-size: 14px;
            line-height: 1.7;
        }

        @media (max-width: 767px) {
            .gh-admin-open-grid,
            .gh-admin-open-meta {
                grid-template-columns: 1fr;
            }

            .gh-admin-open-title {
                font-size: 28px;
            }

            .gh-admin-open-hero,
            .gh-admin-open-body {
                padding: 20px 18px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl gh-admin-open-shell">
        <div class="gh-admin-open-card">
            <div class="gh-admin-open-hero">
                <div class="gh-admin-open-badge">Khu quản trị nâng cao</div>
                <h1 class="gh-admin-open-title">Mở công cụ quản trị gian hàng</h1>
                <p class="gh-admin-open-summary">
                    Đây là khu quản trị nâng cao dành riêng cho gian hàng. Tài khoản vẫn dùng chung với tài khoản gốc, nhưng quyền truy cập khu quản trị cần được mở riêng để bảo đảm an toàn vận hành.
                </p>
                <div class="gh-admin-open-actions">
                    <span id="statusBadge" runat="server" class="gh-admin-open-status">
                        <asp:Literal ID="lit_access_status" runat="server" />
                    </span>
                    <asp:HyperLink ID="lnk_open_admin" runat="server" CssClass="btn btn-success" Visible="false">Mở khu quản trị</asp:HyperLink>
                    <asp:Button ID="btn_request_access" runat="server" CssClass="btn btn-primary" Text="Gửi yêu cầu mở quyền" OnClick="btn_request_access_Click" Visible="false" />
                    <asp:HyperLink ID="lnk_back_account" runat="server" CssClass="btn btn-outline-secondary" NavigateUrl="/gianhang/tai-khoan/default.aspx">Về trung tâm gian hàng</asp:HyperLink>
                </div>
            </div>

            <div class="gh-admin-open-body">
                <div class="gh-admin-open-grid">
                    <div class="gh-admin-open-panel">
                        <h3>Luồng truy cập chuẩn</h3>
                        <ul>
                            <li>Đăng nhập bằng chính tài khoản đã được dùng để vận hành gian hàng.</li>
                            <li>Gửi yêu cầu mở công cụ quản trị ngay tại màn hình này.</li>
                            <li>Admin xác nhận và bật quyền truy cập khu quản trị cho đúng tài khoản đó.</li>
                            <li>Sau khi được mở quyền, vào thẳng app quản trị mà không cần đăng nhập riêng.</li>
                        </ul>
                    </div>
                    <div class="gh-admin-open-panel">
                        <h3>Khi nào cần mở quyền này?</h3>
                        <ul>
                            <li>Khi bạn cần dùng app quản trị riêng của gian hàng.</li>
                            <li>Khi cần các công cụ cấu hình, quản trị chuyên sâu hoặc phân hệ vận hành nâng cao.</li>
                            <li>Khi muốn tách rõ gian hàng công khai và app quản trị nội bộ.</li>
                        </ul>
                    </div>
                    <div class="gh-admin-open-panel">
                        <h3>Admin xử lý ở đâu?</h3>
                        <p class="mb-0">
                            Admin duyệt trong khu quản trị chung hoặc cấp trực tiếp trong phần quản lý tài khoản theo đúng không gian hoạt động.
                        </p>
                    </div>
                    <div class="gh-admin-open-panel">
                        <h3>Quyền này mở ra điều gì?</h3>
                        <ul>
                            <li>Vào khu quản trị riêng của gian hàng.</li>
                            <li>Khởi tạo phiên quản trị bằng chính tài khoản đã được cấp quyền.</li>
                            <li>Không tạo tài khoản đăng nhập riêng, không tách khỏi dữ liệu gốc.</li>
                        </ul>
                    </div>
                </div>

                <div class="gh-admin-open-meta">
                    <div class="gh-admin-open-meta-item">
                        <div class="gh-admin-open-meta-label">Tài khoản chính</div>
                        <div class="gh-admin-open-meta-value"><asp:Literal ID="lit_account_key" runat="server" /></div>
                    </div>
                    <div class="gh-admin-open-meta-item">
                        <div class="gh-admin-open-meta-label">Gian hàng hiện tại</div>
                        <div class="gh-admin-open-meta-value"><asp:Literal ID="lit_store_name" runat="server" /></div>
                    </div>
                    <div class="gh-admin-open-meta-item">
                        <div class="gh-admin-open-meta-label">Yêu cầu gần nhất</div>
                        <div class="gh-admin-open-meta-value"><asp:Literal ID="lit_request_status" runat="server" /></div>
                    </div>
                </div>

                <asp:PlaceHolder ID="ph_request_time" runat="server" Visible="false">
                    <div class="gh-admin-open-note">
                        Ngày gửi yêu cầu gần nhất: <strong><asp:Literal ID="lit_requested_at" runat="server" /></strong>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_review_note" runat="server" Visible="false">
                    <div class="gh-admin-open-note">
                        Ghi chú xử lý: <asp:Literal ID="lit_review_note" runat="server" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_lock_reason" runat="server" Visible="false">
                    <div class="gh-admin-open-note">
                        Lý do khóa quyền hiện tại: <asp:Literal ID="lit_lock_reason" runat="server" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:Content>
