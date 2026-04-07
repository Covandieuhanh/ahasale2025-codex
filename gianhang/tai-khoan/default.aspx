<%@ Page Title="Trung tâm gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerGianHang.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="gianhang_tai_khoan_default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .gh-account-shell {
            min-height: 100vh;
            padding: 20px 0 32px;
            background:
                radial-gradient(1000px 280px at 12% -4%, rgba(249,115,22,.16), transparent 64%),
                radial-gradient(900px 260px at 88% -8%, rgba(251,146,60,.12), transparent 58%),
                #f7fafc;
        }

        .gh-account-wrap {
            max-width: 1180px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .gh-account-hero {
            background: linear-gradient(135deg, #ffb169, #f97316 58%, #fb923c);
            color: #fff;
            border-radius: 22px;
            box-shadow: 0 18px 38px rgba(15, 23, 42, .12);
            padding: 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 18px;
            flex-wrap: wrap;
        }

        .gh-account-head {
            display: flex;
            align-items: center;
            gap: 16px;
            min-width: 0;
        }

        .gh-account-avatar {
            width: 88px;
            height: 88px;
            border-radius: 22px;
            object-fit: cover;
            border: 3px solid rgba(255,255,255,.45);
            background: rgba(255,255,255,.18);
            box-shadow: 0 14px 28px rgba(15, 23, 42, .12);
        }

        .gh-account-eyebrow {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.18);
            border: 1px solid rgba(255,255,255,.34);
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .02em;
            margin-bottom: 10px;
        }

        .gh-account-title {
            margin: 0;
            font-size: 30px;
            line-height: 1.08;
            font-weight: 800;
            color: #fff !important;
        }

        .gh-account-sub {
            margin-top: 8px;
            font-size: 14px;
            opacity: .96;
            color: rgba(255,255,255,.94) !important;
        }

        .gh-account-meta {
            margin-top: 12px;
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .gh-account-chip {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.14);
            border: 1px solid rgba(255,255,255,.28);
            font-size: 13px;
            font-weight: 700;
            color: #fff;
        }

        .gh-account-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            justify-content: flex-end;
        }

        .gh-account-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 999px;
            font-size: 13px;
            font-weight: 800;
            border: 1px solid rgba(255,255,255,.36);
            background: rgba(255,255,255,.16);
            color: #fff !important;
            text-decoration: none;
        }

        .gh-account-btn:hover {
            color: #fff !important;
            background: rgba(255,255,255,.24);
        }

        .gh-account-btn--solid {
            background: #fff;
            border-color: #fff;
            color: #c2410c !important;
        }

        .gh-account-btn--solid:hover {
            color: #9a3412 !important;
            background: #ffedd5;
            border-color: #ffedd5;
        }

        .gh-account-grid {
            margin-top: 16px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
        }

        .gh-account-stat {
            background: #fff;
            border-radius: 18px;
            border: 1px solid #dbe4ee;
            box-shadow: 0 12px 28px rgba(15, 23, 42, .06);
            padding: 16px;
        }

        .gh-account-stat-label {
            font-size: 12px;
            color: #64748b;
            margin-bottom: 6px;
        }

        .gh-account-stat-value {
            font-size: 26px;
            line-height: 1.05;
            font-weight: 800;
            color: #0f172a;
        }

        .gh-account-stat-sub {
            margin-top: 6px;
            font-size: 12px;
            color: #64748b;
        }

        .gh-account-layout {
            margin-top: 16px;
            display: grid;
            grid-template-columns: minmax(0, 1.35fr) minmax(320px, .95fr);
            gap: 16px;
            align-items: start;
        }

        .gh-account-card {
            background: #fff;
            border: 1px solid #dbe4ee;
            border-radius: 22px;
            box-shadow: 0 14px 30px rgba(15, 23, 42, .06);
            overflow: hidden;
        }

        .gh-account-card-head {
            padding: 16px 18px;
            border-bottom: 1px solid #e8eef5;
        }

        .gh-account-card-title {
            margin: 0;
            font-size: 20px;
            line-height: 1.15;
            font-weight: 800;
            color: #0f172a;
        }

        .gh-account-card-sub {
            margin-top: 5px;
            font-size: 13px;
            color: #64748b;
        }

        .gh-account-card-body {
            padding: 18px;
        }

        .gh-account-kv {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 12px;
        }

        .gh-account-kv-item {
            padding: 14px;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            background: #f8fafc;
        }

        .gh-account-kv-label {
            font-size: 12px;
            color: #64748b;
            margin-bottom: 4px;
        }

        .gh-account-kv-value {
            font-size: 15px;
            color: #0f172a;
            font-weight: 700;
            word-break: break-word;
        }

        .gh-account-note {
            margin-top: 14px;
            border-radius: 16px;
            border: 1px solid #fed7aa;
            background: #fff7ed;
            color: #9a3412;
            padding: 14px 15px;
            font-size: 13px;
            line-height: 1.6;
        }

        .gh-account-links {
            display: grid;
            gap: 10px;
        }

        .gh-account-form {
            display: grid;
            gap: 14px;
        }

        .gh-account-form-grid {
            display: grid;
            grid-template-columns: 1fr;
            gap: 14px;
        }

        .gh-account-field-label {
            display: block;
            margin-bottom: 6px;
            font-size: 12px;
            font-weight: 800;
            color: #475569;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        .gh-account-input,
        .gh-account-textarea {
            width: 100%;
            border: 1px solid #dbe4ee;
            border-radius: 16px;
            background: #fff;
            color: #0f172a;
            padding: 13px 14px;
            font-size: 14px;
            line-height: 1.5;
            box-shadow: inset 0 1px 2px rgba(15, 23, 42, .03);
        }

        .gh-account-input:focus,
        .gh-account-textarea:focus {
            outline: none;
            border-color: #fb923c;
            box-shadow: 0 0 0 4px rgba(249, 115, 22, .12);
        }

        .gh-account-textarea {
            min-height: 132px;
            resize: vertical;
        }

        .gh-account-avatar-upload {
            display: grid;
            grid-template-columns: 110px minmax(0, 1fr);
            gap: 14px;
            align-items: start;
        }

        .gh-account-avatar-preview img {
            width: 110px;
            height: 110px;
            border-radius: 22px;
            object-fit: cover;
            border: 1px solid #dbe4ee;
            background: #f8fafc;
            box-shadow: 0 8px 18px rgba(15, 23, 42, .06);
        }

        .gh-account-upload-note {
            margin-top: 8px;
            font-size: 12px;
            color: #64748b;
            line-height: 1.6;
        }

        .gh-account-form-actions {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .gh-account-form-note {
            font-size: 13px;
            color: #64748b;
            line-height: 1.6;
        }

        .gh-account-submit {
            min-height: 44px;
            padding: 0 18px;
            border-radius: 999px;
            border: 0;
            background: linear-gradient(135deg, #fb923c, #f97316);
            color: #fff;
            font-size: 14px;
            font-weight: 800;
            box-shadow: 0 14px 28px rgba(249, 115, 22, .2);
        }

        .gh-account-submit:hover {
            filter: brightness(1.02);
        }

        .gh-account-alert {
            margin-bottom: 14px;
            border-radius: 16px;
            padding: 13px 15px;
            font-size: 13px;
            line-height: 1.6;
            border: 1px solid transparent;
        }

        .gh-account-alert--success {
            background: #ecfdf3;
            border-color: #bbf7d0;
            color: #166534;
        }

        .gh-account-alert--warning {
            background: #fff7ed;
            border-color: #fed7aa;
            color: #9a3412;
        }

        .gh-account-link {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            padding: 13px 14px;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            background: #fff;
            color: #0f172a;
            font-weight: 700;
            text-decoration: none;
        }

        .gh-account-link:hover {
            border-color: #fdba74;
            background: #fffaf5;
            color: #c2410c;
        }

        .gh-account-link small {
            display: block;
            font-weight: 500;
            color: #64748b;
            margin-top: 2px;
        }

        .gh-account-row-list {
            display: grid;
            gap: 10px;
        }

        .gh-account-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            padding: 12px 14px;
            border-radius: 15px;
            border: 1px solid #e2e8f0;
            background: #f8fafc;
        }

        .gh-account-row-title {
            font-weight: 700;
            color: #0f172a;
        }

        .gh-account-row-sub {
            margin-top: 2px;
            font-size: 12px;
            color: #64748b;
        }

        .gh-account-badge {
            display: inline-flex;
            align-items: center;
            min-height: 30px;
            padding: 0 10px;
            border-radius: 999px;
            background: #ffedd5;
            color: #c2410c;
            font-size: 12px;
            font-weight: 800;
            white-space: nowrap;
        }

        @media (max-width: 767.98px) {
            .gh-account-layout {
                grid-template-columns: 1fr;
            }

            .gh-account-kv {
                grid-template-columns: 1fr;
            }

            .gh-account-avatar-upload {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="gh-account-shell">
        <div class="gh-account-wrap">
            <div class="gh-account-hero">
                <div class="gh-account-head">
                    <asp:Image ID="img_avatar" runat="server" CssClass="gh-account-avatar" />
                    <div>
                        <div class="gh-account-eyebrow">Trung tâm vận hành</div>
                        <h1 class="gh-account-title"><asp:Literal ID="lit_store_name" runat="server" /></h1>
                        <div class="gh-account-sub">Dùng chung đăng nhập với tài khoản gốc để vận hành gian hàng.</div>
                        <div class="gh-account-meta">
                            <span class="gh-account-chip">@<asp:Literal ID="lit_account_key" runat="server" /></span>
                            <span class="gh-account-chip"><asp:Literal ID="lit_space_status" runat="server" /></span>
                            <span class="gh-account-chip"><asp:Literal ID="lit_contact_email" runat="server" /></span>
                        </div>
                    </div>
                </div>

                <div class="gh-account-actions">
                    <asp:HyperLink ID="lnk_storefront" runat="server" CssClass="gh-account-btn gh-account-btn--solid">Trang công khai</asp:HyperLink>
                    <asp:HyperLink ID="lnk_edit_home_profile" runat="server" CssClass="gh-account-btn">Sửa hồ sơ gốc</asp:HyperLink>
                    <asp:HyperLink ID="lnk_change_password" runat="server" CssClass="gh-account-btn">Đổi mật khẩu</asp:HyperLink>
                </div>
            </div>

            <div class="gh-account-grid">
                <div class="gh-account-stat">
                    <div class="gh-account-stat-label">Sản phẩm công khai</div>
                    <div class="gh-account-stat-value"><asp:Literal ID="lit_product_count" runat="server" /></div>
                </div>
                <div class="gh-account-stat">
                    <div class="gh-account-stat-label">Dịch vụ công khai</div>
                    <div class="gh-account-stat-value"><asp:Literal ID="lit_service_count" runat="server" /></div>
                </div>
                <div class="gh-account-stat">
                    <div class="gh-account-stat-label">Đơn chờ xử lý</div>
                    <div class="gh-account-stat-value"><asp:Literal ID="lit_pending_orders" runat="server" /></div>
                </div>
                <div class="gh-account-stat">
                    <div class="gh-account-stat-label">Lịch hẹn trong kỳ</div>
                    <div class="gh-account-stat-value"><asp:Literal ID="lit_booking_total" runat="server" /></div>
                </div>
                <div class="gh-account-stat">
                    <div class="gh-account-stat-label">Hồ sơ quyền tiêu dùng</div>
                    <div class="gh-account-stat-value"><asp:Literal ID="lit_shop_tieudung_balance" runat="server" /></div>
                    <div class="gh-account-stat-sub">≈ <asp:Literal ID="lit_shop_tieudung_vnd" runat="server" /> đ</div>
                </div>
                <div class="gh-account-stat">
                    <div class="gh-account-stat-label">Hồ sơ quyền ưu đãi</div>
                    <div class="gh-account-stat-value"><asp:Literal ID="lit_shop_uudai_balance" runat="server" /></div>
                    <div class="gh-account-stat-sub">≈ <asp:Literal ID="lit_shop_uudai_vnd" runat="server" /> đ</div>
                </div>
            </div>

            <div class="gh-account-layout">
                <div class="gh-account-card">
                    <div class="gh-account-card-head">
                        <h2 class="gh-account-card-title">Thông tin tài khoản gốc</h2>
                        <div class="gh-account-card-sub">Gian hàng dùng chung danh tính với tài khoản gốc, nhưng vận hành bán hàng trên dữ liệu riêng của không gian này.</div>
                    </div>
                    <div class="gh-account-card-body">
                        <div class="gh-account-kv">
                            <div class="gh-account-kv-item">
                                <div class="gh-account-kv-label">Họ tên</div>
                                <div class="gh-account-kv-value"><asp:Literal ID="lit_full_name" runat="server" /></div>
                            </div>
                            <div class="gh-account-kv-item">
                                <div class="gh-account-kv-label">Loại tài khoản</div>
                                <div class="gh-account-kv-value"><asp:Literal ID="lit_account_type" runat="server" /></div>
                            </div>
                            <div class="gh-account-kv-item">
                                <div class="gh-account-kv-label">Email liên hệ</div>
                                <div class="gh-account-kv-value"><asp:Literal ID="lit_email" runat="server" /></div>
                            </div>
                            <div class="gh-account-kv-item">
                                <div class="gh-account-kv-label">Email gian hàng</div>
                                <div class="gh-account-kv-value"><asp:Literal ID="lit_gianhang_email" runat="server" /></div>
                            </div>
                            <div class="gh-account-kv-item">
                                <div class="gh-account-kv-label">Trang công khai</div>
                                <div class="gh-account-kv-value"><asp:Literal ID="lit_profile_url" runat="server" /></div>
                            </div>
                            <div class="gh-account-kv-item">
                                <div class="gh-account-kv-label">Quản trị gian hàng</div>
                                <div class="gh-account-kv-value"><asp:Literal ID="lit_admin_access" runat="server" /></div>
                            </div>
                        </div>

                        <div class="gh-account-note">
                            Mật khẩu, phiên đăng nhập và hồ sơ chính của gian hàng dùng chung với tài khoản gốc. Khi cần đổi mật khẩu, hệ thống sẽ chuyển về đúng khu vực tài khoản gốc để cập nhật.
                        </div>
                    </div>
                </div>

                <div class="gh-account-card">
                    <div class="gh-account-card-head">
                        <h2 class="gh-account-card-title">Thông tin gian hàng</h2>
                        <div class="gh-account-card-sub">Tên gian hàng và mô tả hiển thị cho storefront được chỉnh riêng, không đè lên tên tài khoản gốc.</div>
                    </div>
                    <div class="gh-account-card-body">
                        <asp:PlaceHolder ID="ph_storefront_notice" runat="server" Visible="false">
                            <div class='gh-account-alert <%= StorefrontNoticeCssClass %>'>
                                <asp:Literal ID="lit_storefront_notice" runat="server" />
                            </div>
                        </asp:PlaceHolder>

                        <div class="gh-account-form">
                            <div class="gh-account-form-grid">
                                <div>
                                    <label class="gh-account-field-label">Ảnh đại diện gian hàng</label>
                                    <div class="gh-account-avatar-upload">
                                        <div class="gh-account-avatar-preview" id="storeAvatarPreview">
                                            <asp:Literal ID="lit_store_avatar_preview" runat="server" />
                                        </div>
                                        <div>
                                            <input type="file" id="storeAvatarFileInput" onchange="uploadStoreAvatar()" class="gh-account-input" accept="image/*" />
                                            <div id="storeAvatarUploadMessage" class="text-danger small mt-2"></div>
                                            <div class="gh-account-upload-note">
                                                Ảnh này chỉ dùng cho storefront của gian hàng. Nếu chưa tải lên, hệ thống sẽ dùng ảnh hồ sơ tài khoản gốc làm mặc định.
                                            </div>
                                            <div style="display:none">
                                                <asp:TextBox ID="txt_store_avatar" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div>
                                    <label class="gh-account-field-label" for="<%= txt_store_name.ClientID %>">Tên gian hàng</label>
                                    <asp:TextBox ID="txt_store_name" runat="server" CssClass="gh-account-input" MaxLength="150" />
                                </div>
                                <div>
                                    <label class="gh-account-field-label" for="<%= txt_store_description.ClientID %>">Mô tả gian hàng</label>
                                    <asp:TextBox ID="txt_store_description" runat="server" CssClass="gh-account-textarea" TextMode="MultiLine" Rows="6" MaxLength="1200" />
                                </div>
                            </div>
                            <div class="gh-account-form-actions">
                                <div class="gh-account-form-note">
                                    Tên gian hàng sẽ hiển thị ở trang công khai `/gianhang`. Mô tả gian hàng dùng cho phần giới thiệu và giúp storefront rõ ràng hơn với khách truy cập.
                                </div>
                                <asp:Button ID="btn_save_storefront" runat="server" CssClass="gh-account-submit" Text="Lưu thông tin gian hàng" OnClick="btn_save_storefront_Click" UseSubmitBehavior="false" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="gh-account-card">
                    <div class="gh-account-card-head">
                        <h2 class="gh-account-card-title">Truy cập nhanh</h2>
                        <div class="gh-account-card-sub">Các nhóm thao tác chính để vận hành gian hàng hằng ngày.</div>
                    </div>
                    <div class="gh-account-card-body">
                        <div class="gh-account-links">
                            <asp:HyperLink ID="lnk_shop_tieudung_profile" runat="server" CssClass="gh-account-link">
                                <span>
                                    Hồ sơ quyền tiêu dùng
                                    <small>Theo dõi điểm A tiêu dùng mà gian hàng nhận được từ đơn bán.</small>
                                </span>
                                <strong><asp:Literal ID="lit_shop_tieudung_link_balance" runat="server" /></strong>
                            </asp:HyperLink>
                            <asp:HyperLink ID="lnk_shop_uudai_profile" runat="server" CssClass="gh-account-link">
                                <span>
                                    Hồ sơ quyền ưu đãi
                                    <small>Theo dõi điểm A ưu đãi mà gian hàng tích lũy từ giao dịch bán.</small>
                                </span>
                                <strong><asp:Literal ID="lit_shop_uudai_link_balance" runat="server" /></strong>
                            </asp:HyperLink>
                            <asp:HyperLink ID="lnk_manage_posts" runat="server" CssClass="gh-account-link">
                                <span>
                                    Quản lý tin
                                    <small>Đăng mới, cập nhật, ẩn/hiện sản phẩm và dịch vụ.</small>
                                </span>
                                <span>›</span>
                            </asp:HyperLink>
                            <asp:HyperLink ID="lnk_orders" runat="server" CssClass="gh-account-link">
                                <span>
                                    Đơn bán / POS
                                    <small>Tạo đơn offline, xử lý chờ trao đổi và theo dõi đơn hàng.</small>
                                </span>
                                <span>›</span>
                            </asp:HyperLink>
                            <asp:HyperLink ID="lnk_bookings" runat="server" CssClass="gh-account-link">
                                <span>
                                    Lịch hẹn
                                    <small>Xem lịch khách đặt, xác nhận, hoàn thành hoặc hủy lịch.</small>
                                </span>
                                <span>›</span>
                            </asp:HyperLink>
                            <asp:HyperLink ID="lnk_customers" runat="server" CssClass="gh-account-link">
                                <span>
                                    Khách hàng
                                    <small>Quản lý tệp khách, xem chi tiết đơn và lịch hẹn theo khách.</small>
                                </span>
                                <span>›</span>
                            </asp:HyperLink>
                            <asp:HyperLink ID="lnk_report" runat="server" CssClass="gh-account-link">
                                <span>
                                    Báo cáo gian hàng
                                    <small>Theo dõi doanh thu, đơn, lịch hẹn và hiệu quả vận hành.</small>
                                </span>
                                <span>›</span>
                            </asp:HyperLink>
                            <asp:PlaceHolder ID="ph_admin_link" runat="server" Visible="false">
                                <asp:HyperLink ID="lnk_admin" runat="server" CssClass="gh-account-link">
                                    <span>
                                        Gian hàng Admin
                                        <small>Đi tới khu quản trị riêng của gian hàng khi tài khoản đã được mở quyền.</small>
                                    </span>
                                    <span>›</span>
                                </asp:HyperLink>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_admin_request_link" runat="server" Visible="false">
                                <asp:HyperLink ID="lnk_request_admin" runat="server" CssClass="gh-account-link">
                                    <span>
                                        Mở công cụ quản trị gian hàng
                                        <small>Gửi yêu cầu mở quyền quản trị nâng cao cho tài khoản đang vận hành gian hàng.</small>
                                    </span>
                                    <span>›</span>
                                </asp:HyperLink>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>

            <div class="gh-account-card mt-3">
                <div class="gh-account-card-head">
                    <h2 class="gh-account-card-title">Tổng quan vận hành nhanh</h2>
                    <div class="gh-account-card-sub">Những chỉ số gần nhất để kiểm tra trạng thái hoạt động hiện tại của gian hàng.</div>
                </div>
                <div class="gh-account-card-body">
                    <div class="gh-account-row-list">
                        <div class="gh-account-row">
                            <div>
                                <div class="gh-account-row-title">Doanh thu gộp trong kỳ</div>
                                <div class="gh-account-row-sub">Tính theo chi tiết đơn của gian hàng trong kỳ hiện tại.</div>
                            </div>
                            <div class="gh-account-badge"><asp:Literal ID="lit_revenue_gross" runat="server" /> đ</div>
                        </div>
                        <div class="gh-account-row">
                            <div>
                                <div class="gh-account-row-title">Tổng lượt xem trang công khai</div>
                                <div class="gh-account-row-sub">Tổng lượt truy cập hiện có trên sản phẩm và dịch vụ công khai.</div>
                            </div>
                            <div class="gh-account-badge"><asp:Literal ID="lit_total_views" runat="server" /></div>
                        </div>
                        <div class="gh-account-row">
                            <div>
                                <div class="gh-account-row-title">Đơn đã Trao đổi / đã giao</div>
                                <div class="gh-account-row-sub">Theo tiến độ xử lý đơn hiện tại của gian hàng.</div>
                            </div>
                            <div class="gh-account-badge"><asp:Literal ID="lit_exchange_delivery" runat="server" /></div>
                        </div>
                        <div class="gh-account-row">
                            <div>
                                <div class="gh-account-row-title">Lịch hẹn đã xác nhận / hoàn thành</div>
                                <div class="gh-account-row-sub">Để theo dõi tiến độ phục vụ khách đặt dịch vụ.</div>
                            </div>
                            <div class="gh-account-badge"><asp:Literal ID="lit_booking_progress" runat="server" /></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script>
        function uploadStoreAvatar() {
            var fileInput = document.getElementById("storeAvatarFileInput");
            var messageDiv = document.getElementById("storeAvatarUploadMessage");
            var previewDiv = document.getElementById("storeAvatarPreview");
            messageDiv.innerHTML = "";

            if (!fileInput || fileInput.files.length === 0) {
                messageDiv.innerHTML = "Vui lòng chọn ảnh.";
                return;
            }

            var file = fileInput.files[0];
            var ext = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
            var allowed = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg", ".avif", ".jfif"];
            if (allowed.indexOf(ext) === -1) {
                messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                return;
            }

            if (file.size > 20 * 1024 * 1024) {
                messageDiv.innerHTML = "Vui lòng chọn ảnh nhỏ hơn 20 MB.";
                return;
            }

            var formData = new FormData();
            formData.append("file", file);
            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
            xhr.onload = function () {
                if (xhr.status === 200) {
                    var url = (xhr.responseText || "").trim();
                    previewDiv.innerHTML = "<img src='" + url + "' alt='Ảnh đại diện gian hàng' />";
                    document.getElementById('<%= txt_store_avatar.ClientID %>').value = url;
                } else {
                    messageDiv.innerHTML = "Lỗi upload: " + (xhr.responseText || "Không xác định");
                }
            };
            xhr.send(formData);
        }
    </script>
</asp:Content>
