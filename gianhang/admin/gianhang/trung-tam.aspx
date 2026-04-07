<%@ Page Title="Trung tâm /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="trung-tam.aspx.cs" Inherits="gianhang_admin_gianhang_trung_tam" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-center-shell {
            display: grid;
            gap: 18px;
        }

        .gh-center-hero {
            border: 1px solid #f6d4c5;
            border-radius: 24px;
            padding: 22px;
            background:
                radial-gradient(760px 240px at 0% 0%, rgba(255, 110, 53, .18), transparent 60%),
                linear-gradient(180deg, #fffdfc 0%, #ffffff 100%);
            box-shadow: 0 20px 36px rgba(127, 29, 29, 0.08);
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 18px;
            flex-wrap: wrap;
        }

        .gh-center-kicker {
            display: inline-flex;
            align-items: center;
            min-height: 30px;
            padding: 0 12px;
            border-radius: 999px;
            background: #fff3ee;
            border: 1px solid #ffd4c5;
            color: #b93815;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .03em;
            text-transform: uppercase;
        }

        .gh-center-title {
            margin: 10px 0 6px;
            color: #7f1d1d;
            font-size: 30px;
            line-height: 1.12;
            font-weight: 900;
        }

        .gh-center-sub {
            margin: 0;
            color: #8d5d5d;
            font-size: 14px;
            line-height: 1.6;
            max-width: 760px;
        }

        .gh-center-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gh-center-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 14px;
            text-decoration: none !important;
            font-weight: 800;
            font-size: 13px;
            border: 1px solid transparent;
        }

        .gh-center-btn--primary {
            background: linear-gradient(135deg, #d73a31, #ef6b41);
            color: #fff !important;
            box-shadow: 0 14px 30px rgba(215, 58, 49, .18);
        }

        .gh-center-btn--soft {
            background: #fff;
            color: #7f1d1d !important;
            border-color: #f3cccc;
        }

        .gh-center-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 12px;
        }

        .gh-center-stat {
            border: 1px solid #f0dfe0;
            border-radius: 18px;
            padding: 16px;
            background: #fff;
            box-shadow: 0 14px 28px rgba(15, 23, 42, 0.05);
        }

        .gh-center-stat small {
            display: block;
            color: #8d5d5d;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .gh-center-stat strong {
            display: block;
            margin-top: 8px;
            color: #1f2937;
            font-size: 28px;
            line-height: 1.05;
            font-weight: 900;
        }

        .gh-center-stat span {
            display: block;
            margin-top: 6px;
            color: #6b7280;
            font-size: 13px;
        }

        .gh-center-grid {
            display: grid;
            grid-template-columns: minmax(0, 1.2fr) minmax(320px, .95fr);
            gap: 16px;
            align-items: start;
        }

        .gh-center-card {
            border: 1px solid #ebeff5;
            border-radius: 18px;
            background: #fff;
            padding: 18px;
            box-shadow: 0 16px 30px rgba(15, 23, 42, 0.05);
        }

        .gh-center-card h3 {
            margin: 0 0 6px;
            font-size: 20px;
            line-height: 1.2;
            color: #1f2937;
        }

        .gh-center-card p {
            margin: 0;
            color: #6b7280;
            line-height: 1.6;
            font-size: 14px;
        }

        .gh-center-kv {
            margin-top: 16px;
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 12px;
        }

        .gh-center-kv-item {
            padding: 14px;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            background: #f8fafc;
        }

        .gh-center-kv-item small {
            display: block;
            color: #64748b;
            font-size: 12px;
            margin-bottom: 6px;
            text-transform: uppercase;
            font-weight: 700;
        }

        .gh-center-kv-item strong,
        .gh-center-kv-item a {
            color: #0f172a;
            font-size: 15px;
            line-height: 1.45;
            font-weight: 800;
            text-decoration: none;
            word-break: break-word;
        }

        .gh-center-note {
            margin-top: 16px;
            padding: 14px 16px;
            border-radius: 16px;
            background: #fff7ed;
            border: 1px solid #fed7aa;
            color: #9a3412;
            line-height: 1.6;
            font-size: 14px;
        }

        .gh-center-links {
            display: grid;
            gap: 10px;
            margin-top: 16px;
        }

        .gh-center-link {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            padding: 12px 14px;
            border-radius: 14px;
            background: #f8fafc;
            border: 1px solid #edf2f7;
            text-decoration: none !important;
            color: #1f2937;
        }

        .gh-center-link small {
            display: block;
            color: #8d5d5d;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            margin-bottom: 2px;
        }

        .gh-center-link strong {
            display: block;
            font-size: 15px;
            line-height: 1.35;
        }

        .gh-center-link span {
            display: block;
            color: #6b7280;
            font-size: 13px;
            line-height: 1.5;
            margin-top: 3px;
        }

        .gh-center-link em {
            font-style: normal;
            font-weight: 900;
            color: #d73a31;
        }

        @media (max-width: 767px) {
            .gh-center-title {
                font-size: 26px;
            }

            .gh-center-grid {
                grid-template-columns: 1fr;
            }

            .gh-center-kv {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-center-shell">
        <section class="gh-center-hero">
            <div>
                <div class="gh-center-kicker">Trung tâm /gianhang</div>
                <h1 class="gh-center-title"><%=WorkspaceDisplayName %></h1>
                <p class="gh-center-sub">
                    Đây là bảng điều khiển tài khoản gốc của không gian <code>/gianhang</code> ngay trong <code>/gianhang/admin</code>.
                    Chủ Home có thể nhìn một chỗ để thấy thông tin storefront, trạng thái quyền level 2 và các lối vào vận hành quan trọng.
                </p>
            </div>
            <div class="gh-center-actions">
                <a class="gh-center-btn gh-center-btn--primary" href="<%=HubUrl %>">Hub /gianhang</a>
                <a class="gh-center-btn gh-center-btn--soft" href="<%=NativeCenterUrl %>">Trung tâm native</a>
                <a class="gh-center-btn gh-center-btn--soft" href="<%=PublicUrl %>" target="_blank">Trang công khai</a>
                <a class="gh-center-btn gh-center-btn--soft" href="<%=HomeProfileUrl %>">Sửa hồ sơ Home</a>
                <a class="gh-center-btn gh-center-btn--soft" href="<%=AdminSettingsUrl %>">Cài đặt admin</a>
                <a class="gh-center-btn gh-center-btn--soft" href="<%=ProfileSettingsUrl %>">Thông tin thương hiệu</a>
            </div>
        </section>

        <div class="gh-center-stats">
            <div class="gh-center-stat">
                <small>Sản phẩm</small>
                <strong><%=ProductCount.ToString("#,##0") %></strong>
                <span>Nguồn hiển thị public và bán hàng.</span>
            </div>
            <div class="gh-center-stat">
                <small>Dịch vụ</small>
                <strong><%=ServiceCount.ToString("#,##0") %></strong>
                <span>Phục vụ đặt lịch và hóa đơn vận hành.</span>
            </div>
            <div class="gh-center-stat">
                <small>Đơn chờ xử lý</small>
                <strong><%=PendingOrderCount.ToString("#,##0") %></strong>
                <span>Đơn buyer còn đang trong queue native.</span>
            </div>
            <div class="gh-center-stat">
                <small>Lịch hẹn</small>
                <strong><%=BookingTotal.ToString("#,##0") %></strong>
                <span>Tổng booking hiện có trong workspace.</span>
            </div>
            <div class="gh-center-stat">
                <small>Doanh thu gộp</small>
                <strong><%=RevenueText %></strong>
                <span>Tổng từ lõi đơn hàng /gianhang.</span>
            </div>
            <div class="gh-center-stat">
                <small>Truy cập admin</small>
                <strong><%=AdminAccessText %></strong>
                <span>Vai trò hiện tại: <%=RoleLabel %></span>
            </div>
        </div>

        <div class="gh-center-grid">
            <section class="gh-center-card">
                <h3>Thông tin chủ không gian</h3>
                <p>Đây là lớp định danh gốc của workspace. Sau khi được duyệt level 2, chính tài khoản Home này dùng thêm công cụ <code>/gianhang/admin</code>.</p>
                <div class="gh-center-kv">
                    <div class="gh-center-kv-item">
                        <small>Tài khoản Home</small>
                        <strong><%=HomeAccountKey %></strong>
                    </div>
                    <div class="gh-center-kv-item">
                        <small>Chủ sở hữu</small>
                        <strong><%=OwnerDisplayName %></strong>
                    </div>
                    <div class="gh-center-kv-item">
                        <small>Email liên hệ</small>
                        <strong><%=ContactEmail %></strong>
                    </div>
                    <div class="gh-center-kv-item">
                        <small>Loại tài khoản</small>
                        <strong><%=AccountTypeLabel %></strong>
                    </div>
                    <div class="gh-center-kv-item">
                        <small>Trang công khai</small>
                        <a href="<%=PublicUrl %>" target="_blank"><%=PublicUrl %></a>
                    </div>
                    <div class="gh-center-kv-item">
                        <small>URL tuyệt đối</small>
                        <a href="<%=PublicAbsoluteUrl %>" target="_blank"><%=PublicAbsoluteUrl %></a>
                    </div>
                </div>
                <div class="gh-center-note">
                    Logic mới đang dùng là <strong>1 workspace - 2 mặt sử dụng</strong>:
                    <code>/gianhang</code> là mặt public/front office,
                    còn <code>/gianhang/admin</code> là mặt vận hành/back office trên cùng dữ liệu gốc.
                </div>
            </section>

            <section class="gh-center-card">
                <h3>Đi nhanh theo ngữ cảnh</h3>
                <p>Các lối vào dưới đây giúp đi thẳng từ trung tâm tài khoản sang đúng vùng thao tác mà không phải quay lại nhiều menu.</p>
                <div class="gh-center-links">
                    <a class="gh-center-link" href="<%=StorefrontPreviewUrl %>">
                        <div>
                            <small>Preview trong admin</small>
                            <strong>Trang công khai</strong>
                            <span>Xem storefront public ngay trong không gian quản trị.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-center-link" href="<%=ContentUrl %>">
                        <div>
                            <small>Nội dung nguồn</small>
                            <strong>Nội dung /gianhang</strong>
                            <span>Gom sản phẩm, dịch vụ và bài viết native vào lớp vận hành.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-center-link" href="<%=OrdersUrl %>">
                        <div>
                            <small>Đơn · checkout</small>
                            <strong>Đơn gian hàng</strong>
                            <span>Theo dõi buyer-flow và bridge sang hóa đơn admin.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-center-link" href="<%=BookingsUrl %>">
                        <div>
                            <small>Khách và lịch</small>
                            <strong>Khách hàng / Lịch hẹn</strong>
                            <span>Đi thẳng vào lớp CRM native của workspace.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-center-link" href="<%=ReportUrl %>">
                        <div>
                            <small>Báo cáo native</small>
                            <strong>Báo cáo /gianhang</strong>
                            <span>Xem dashboard native mà không rời khỏi admin.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-center-link" href="<%=ChangePasswordUrl %>">
                        <div>
                            <small>Bảo mật tài khoản</small>
                            <strong>Đổi mật khẩu Home</strong>
                            <span>Thao tác trên định danh gốc, xong quay lại đúng trung tâm này.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
