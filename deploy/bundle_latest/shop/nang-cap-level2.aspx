<%@ Page Title="Nâng cấp Level 2" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="nang-cap-level2.aspx.cs" Inherits="shop_nang_cap_level2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --level2-orange: #ee4d2d;
            --level2-orange-soft: #fff1ea;
            --level2-ink: #102a43;
            --level2-muted: #627d98;
            --level2-line: #d9e2ec;
            --level2-bg: #f5f8fb;
            --level2-card: #ffffff;
            --level2-radius: 18px;
        }

        .level2-shell {
            min-height: 100vh;
            padding: 24px 16px 40px;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(238,77,45,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(255,122,69,.18), transparent 62%),
                var(--level2-bg);
        }

        .level2-wrap {
            max-width: 1180px;
            margin: 0 auto;
        }

        .level2-hero,
        .level2-card {
            background: var(--level2-card);
            border: 1px solid var(--level2-line);
            border-radius: var(--level2-radius);
            box-shadow: 0 16px 36px rgba(16, 42, 67, .08);
        }

        .level2-hero {
            padding: 24px;
            margin-bottom: 18px;
        }

        .level2-eyebrow {
            color: var(--level2-orange);
            font-weight: 800;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
        }

        .level2-title {
            margin: 10px 0 8px;
            color: var(--level2-ink);
            font-size: 32px;
            line-height: 1.15;
            font-weight: 900;
        }

        .level2-sub {
            color: var(--level2-muted);
            font-size: 15px;
            line-height: 1.6;
            max-width: 780px;
        }

        .level2-kpi-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            margin-top: 18px;
        }

        .level2-kpi {
            border: 1px solid var(--level2-line);
            border-radius: 14px;
            padding: 14px 16px;
            background: #fcfdff;
        }

        .level2-kpi-label {
            color: var(--level2-muted);
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .level2-kpi-value {
            margin-top: 6px;
            color: var(--level2-ink);
            font-size: 24px;
            font-weight: 900;
        }

        .level2-grid {
            display: grid;
            grid-template-columns: minmax(0, 1.15fr) minmax(300px, .85fr);
            gap: 18px;
        }

        .level2-card {
            padding: 20px;
        }

        .level2-card h2 {
            margin: 0 0 10px;
            color: var(--level2-ink);
            font-size: 22px;
            font-weight: 900;
        }

        .level2-card p,
        .level2-card li {
            color: var(--level2-muted);
            line-height: 1.6;
        }

        .level2-list {
            margin: 0;
            padding-left: 20px;
        }

        .level2-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 16px;
        }

        .level2-btn,
        .level2-btn-secondary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 12px;
            border: 1px solid var(--level2-line);
            font-weight: 800;
            text-decoration: none;
            cursor: pointer;
        }

        .level2-btn {
            background: linear-gradient(135deg, #ff9a3d, #ee4d2d);
            color: #fff;
            border-color: transparent;
        }

        .level2-btn-secondary {
            color: var(--level2-ink);
            background: #fff;
        }

        .level2-note,
        .level2-alert {
            border-radius: 14px;
            padding: 14px 16px;
        }

        .level2-note {
            background: var(--level2-orange-soft);
            border: 1px solid #ffd6bf;
            color: #9a3412;
            font-weight: 700;
        }

        .level2-alert {
            margin-bottom: 16px;
            background: #edfdf4;
            border: 1px solid #b7ebc6;
            color: #137333;
            font-weight: 700;
        }

        .level2-definition {
            display: grid;
            grid-template-columns: 144px 1fr;
            gap: 10px 14px;
            margin-top: 16px;
            font-size: 14px;
        }

        .level2-definition dt {
            color: var(--level2-muted);
            font-weight: 800;
        }

        .level2-definition dd {
            margin: 0;
            color: var(--level2-ink);
            font-weight: 700;
        }

        .level2-staff-meta {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 12px;
            margin-top: 16px;
        }

        .level2-mini {
            border: 1px solid var(--level2-line);
            border-radius: 14px;
            padding: 14px 16px;
            background: #fcfdff;
        }

        .level2-mini-label {
            color: var(--level2-muted);
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .level2-mini-value {
            margin-top: 6px;
            color: var(--level2-ink);
            font-size: 22px;
            font-weight: 900;
        }

        .level2-staff-list {
            display: grid;
            gap: 10px;
            margin-top: 16px;
        }

        .level2-staff-item {
            border: 1px solid var(--level2-line);
            border-radius: 14px;
            padding: 14px 16px;
            background: #fff;
        }

        .level2-staff-head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
        }

        .level2-staff-name {
            color: var(--level2-ink);
            font-weight: 900;
            font-size: 16px;
        }

        .level2-staff-badge {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 10px;
            border-radius: 999px;
            background: #fff1ea;
            color: #c2410c;
            font-size: 12px;
            font-weight: 800;
        }

        .level2-staff-sub,
        .level2-staff-note {
            margin-top: 4px;
            color: var(--level2-muted);
            font-size: 14px;
        }

        @media (max-width: 900px) {
            .level2-kpi-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .level2-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 640px) {
            .level2-kpi-grid {
                grid-template-columns: 1fr;
            }

            .level2-definition {
                grid-template-columns: 1fr;
                gap: 4px;
            }

            .level2-staff-meta {
                grid-template-columns: 1fr;
            }

            .level2-staff-head {
                align-items: flex-start;
                flex-direction: column;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="level2-shell">
        <div class="level2-wrap">
            <asp:PlaceHolder ID="ph_notice" runat="server" Visible="false">
                <div class="level2-alert">
                    <asp:Label ID="lb_notice" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <section class="level2-hero">
                <div class="level2-eyebrow">Shop Level 2</div>
                <h1 class="level2-title">Bật bộ công cụ quản trị nâng cao</h1>
                <div class="level2-sub">
                    Level 1 vẫn vận hành bình thường trong <b>/shop</b>. Khi bật Level 2, shop được mở thêm <b>/gianhang/admin</b> để quản trị khách hàng, lịch hẹn nâng cao, hóa đơn, thu chi, kho và nhân sự.
                </div>

                <div class="level2-kpi-grid">
                    <div class="level2-kpi">
                        <div class="level2-kpi-label">Shop</div>
                        <div class="level2-kpi-value"><asp:Label ID="lb_shop_name" runat="server" Text="Shop"></asp:Label></div>
                    </div>
                    <div class="level2-kpi">
                        <div class="level2-kpi-label">Mức hiện tại</div>
                        <div class="level2-kpi-value"><asp:Label ID="lb_shop_level" runat="server" Text="Level 1"></asp:Label></div>
                    </div>
                    <div class="level2-kpi">
                        <div class="level2-kpi-label">Admin owner</div>
                        <div class="level2-kpi-value"><asp:Label ID="lb_admin_owner" runat="server" Text="--"></asp:Label></div>
                    </div>
                    <div class="level2-kpi">
                        <div class="level2-kpi-label">Chi nhánh</div>
                        <div class="level2-kpi-value"><asp:Label ID="lb_branch" runat="server" Text="--"></asp:Label></div>
                    </div>
                </div>
            </section>

            <div class="level2-grid">
                <section class="level2-card">
                    <h2>Khác nhau giữa Level 1 và Level 2</h2>
                    <ul class="level2-list">
                        <li>Level 1: đăng tin, hiển thị công khai, đơn bán, khách hàng và lịch hẹn cơ bản trong <b>/shop</b>.</li>
                        <li>Level 2: mở thêm <b>/gianhang/admin</b> để điều hành lịch nâng cao, CRM, hóa đơn, thẻ dịch vụ, thu chi, kho và phân quyền.</li>
                        <li>Lịch hẹn khách đặt đã tạo trong <b>/shop</b> vẫn được dùng lại nguyên vẹn khi chuyển sang Level 2.</li>
                    </ul>

                    <dl class="level2-definition">
                        <dt>Tài khoản admin</dt>
                        <dd><asp:Label ID="lb_admin_account_info" runat="server" Text="Admin owner sẽ dùng cùng username với tài khoản shop."></asp:Label></dd>
                        <dt>Mật khẩu khởi tạo</dt>
                        <dd><asp:Label ID="lb_password_info" runat="server" Text="Nếu admin owner được tạo mới, mật khẩu mặc định là 123456."></asp:Label></dd>
                        <dt>Dữ liệu dùng chung</dt>
                        <dd>Lịch hẹn, khách hàng và dịch vụ tiếp tục dùng chung lõi dữ liệu hiện có.</dd>
                    </dl>

                    <asp:PlaceHolder ID="ph_enable_level2" runat="server" Visible="false">
                        <asp:PlaceHolder ID="ph_request_reject" runat="server" Visible="false">
                            <div class="level2-alert">
                                Yêu cầu nâng cấp trước đã bị từ chối.
                                <span>Lý do: <asp:Label ID="lb_request_reject" runat="server" Text=""></asp:Label></span>
                            </div>
                        </asp:PlaceHolder>
                        <div class="level2-note">
                            Gửi yêu cầu nâng cấp để admin duyệt. Khi được duyệt, shop sẽ được mở Level 2 và bootstrap owner admin trong <b>/gianhang/admin</b>.
                        </div>
                        <div class="level2-actions">
                            <asp:Button ID="but_enable_level2" runat="server" CssClass="level2-btn" Text="Gửi yêu cầu nâng cấp Level 2" OnClientClick="return AhaPreventDoubleClick(this);" OnClick="but_enable_level2_Click" />
                            <a href="/shop/quan-ly-lich-hen.aspx" class="level2-btn-secondary">Tiếp tục dùng Level 1</a>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="ph_level2_pending" runat="server" Visible="false">
                        <div class="level2-note">
                            Yêu cầu nâng cấp Level 2 đang chờ admin duyệt. Bạn sẽ nhận thông báo ngay khi được duyệt.
                        </div>
                        <dl class="level2-definition">
                            <dt>Trạng thái</dt>
                            <dd>Chờ duyệt</dd>
                            <dt>Thời gian gửi</dt>
                            <dd><asp:Label ID="lb_request_pending_time" runat="server" Text="--"></asp:Label></dd>
                        </dl>
                        <div class="level2-actions">
                            <a href="/shop/quan-ly-lich-hen.aspx" class="level2-btn-secondary">Tiếp tục dùng Level 1</a>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="ph_level2_ready" runat="server" Visible="false">
                        <div class="level2-note">
                            Shop này đã được bật Level 2. Có thể vào <b>/gianhang/admin</b> để dùng công cụ quản trị nâng cao hoặc tiếp tục vận hành cơ bản ở <b>/shop</b>.
                        </div>
                        <div class="level2-actions">
                            <asp:HyperLink ID="lnk_open_advanced" runat="server" CssClass="level2-btn" Text="Mở /gianhang/admin"></asp:HyperLink>
                            <asp:Button ID="but_reset_owner_admin" runat="server" CssClass="level2-btn-secondary" Text="Đặt lại mật khẩu owner admin" OnClientClick="return AhaPreventDoubleClick(this);" OnClick="but_reset_owner_admin_Click" />
                            <asp:HyperLink ID="lnk_add_staff" runat="server" CssClass="level2-btn-secondary" Text="Tạo nhân sự Level 2"></asp:HyperLink>
                            <asp:HyperLink ID="lnk_owner_permission" runat="server" CssClass="level2-btn-secondary" Text="Mở phân quyền owner"></asp:HyperLink>
                        </div>
                    </asp:PlaceHolder>
                </section>

                <section class="level2-card">
                    <h2>Luồng sau khi nâng cấp</h2>
                    <ul class="level2-list">
                        <li>Owner admin dùng cùng username với tài khoản shop hiện tại.</li>
                        <li>Đăng nhập vào <b>/gianhang/admin</b> rồi tạo nhân sự theo preset: lễ tân, thu ngân, kho, marketing, vận hành.</li>
                        <li>Menu trái trong admin sẽ tự ẩn/hiện theo module được cấp quyền.</li>
                        <li>Các màn cấu hình <b>/shop</b> trong admin chỉ mở cho đúng nhóm quyền storefront.</li>
                    </ul>

                    <asp:PlaceHolder ID="ph_level2_ready_detail" runat="server" Visible="false">
                        <div class="level2-staff-meta">
                            <div class="level2-mini">
                                <div class="level2-mini-label">Nhân sự Level 2</div>
                                <div class="level2-mini-value"><asp:Label ID="lb_level2_staff_total" runat="server" Text="0"></asp:Label></div>
                            </div>
                            <div class="level2-mini">
                                <div class="level2-mini-label">Owner admin</div>
                                <div class="level2-mini-value"><asp:Label ID="lb_admin_owner_duplicate" runat="server" Text="--"></asp:Label></div>
                            </div>
                        </div>

                        <asp:PlaceHolder ID="ph_level2_staff_empty" runat="server" Visible="false">
                            <div class="level2-note" style="margin-top:16px;">
                                Chưa có thêm nhân sự Level 2 nào ngoài owner admin.
                            </div>
                        </asp:PlaceHolder>

                        <asp:Repeater ID="rp_level2_staff" runat="server">
                            <HeaderTemplate>
                                <div class="level2-staff-list">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="level2-staff-item">
                                    <div class="level2-staff-head">
                                        <div>
                                            <div class="level2-staff-name"><%# Eval("HoTen") %></div>
                                            <div class="level2-staff-sub">Tài khoản: <%# Eval("TaiKhoan") %> • <%# Eval("ChiNhanh") %></div>
                                        </div>
                                        <asp:PlaceHolder ID="phOwner" runat="server" Visible='<%# Eval("IsOwner") %>'>
                                            <span class="level2-staff-badge">Owner admin</span>
                                        </asp:PlaceHolder>
                                    </div>
                                    <div class="level2-staff-note">Trạng thái: <%# Eval("TrangThai") %> • <%# Eval("PermissionSummary") %></div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                </div>
                            </FooterTemplate>
                        </asp:Repeater>
                    </asp:PlaceHolder>
                </section>
            </div>
        </div>
    </div>
</asp:Content>
