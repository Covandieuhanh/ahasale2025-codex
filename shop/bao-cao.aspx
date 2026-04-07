<%@ Page Title="Báo cáo gian hàng đối tác" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="bao-cao.aspx.cs" Inherits="shop_bao_cao" %>
<%@ Register Src="~/Uc/Shared/SpaceLauncher_uc.ascx" TagPrefix="uc1" TagName="SpaceLauncher" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --shop-green-700: #ee4d2d;
            --shop-green-600: #ff7a45;
            --shop-green-50: #fff1ea;
            --shop-ink-900: #102a43;
            --shop-ink-700: #334e68;
            --shop-ink-500: #627d98;
            --shop-line: #d9e2ec;
            --shop-bg: #f5f8fb;
            --shop-card: #ffffff;
            --shop-shadow: 0 16px 36px rgba(16, 42, 67, .08);
            --radius-lg: 18px;
            --radius-md: 14px;
            --radius-sm: 10px;
        }

        * { box-sizing: border-box; }
        body { margin: 0; }

        .shop-shell {
            min-height: 100vh;
            font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif;
            color: var(--shop-ink-900);
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(238,77,45,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(255,122,69,.18), transparent 62%),
                var(--shop-bg);
        }

        .shop-shell a { color: inherit; text-decoration: none; }

        .shop-topbar {
            max-width: 1220px;
            margin: 0 auto;
            padding: 16px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
        }

        .shop-topbar-left {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 0;
        }

        .brand {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 0;
        }

        .shop-space-toggle {
            width: 46px;
            min-width: 46px;
            height: 46px;
            border-radius: 999px;
            border: 1px solid var(--shop-line);
            background: #fff;
            color: var(--shop-ink-900);
            box-shadow: 0 10px 24px rgba(16, 42, 67, .08);
        }

        .shop-space-toggle:hover {
            background: #fff7ed;
            color: #d9480f;
            border-color: #ffd8a8;
        }

        .brand-logo {
            min-width: 44px;
            width: auto;
            max-width: 156px;
            height: 44px;
            padding: 0 8px;
            border-radius: 14px;
            background: linear-gradient(145deg, var(--shop-green-700), var(--shop-green-600));
            display: inline-flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 10px 24px rgba(238, 77, 45, .25);
            overflow: hidden;
        }

        .brand-logo img {
            width: auto;
            max-width: 100%;
            height: 32px;
            object-fit: contain;
        }

        .brand-title {
            font-weight: 800;
            font-size: 20px;
            line-height: 1.1;
        }

        .brand-sub {
            margin-top: 2px;
            color: var(--shop-ink-500);
            font-size: 13px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            max-width: 560px;
        }

        .top-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            position: relative;
        }

        .quick-wrap {
            position: relative;
        }

        .quick-btn {
            width: 46px;
            min-width: 46px;
            height: 46px;
            border-radius: 999px;
            border: 1px solid var(--shop-line);
            background: #fff;
            color: var(--shop-ink-900);
            box-shadow: 0 10px 24px rgba(16, 42, 67, .08);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            transition: .2s ease;
        }

        .quick-btn:hover,
        .quick-wrap.open .quick-btn {
            background: #fff7ed;
            color: #d9480f;
            border-color: #ffd8a8;
        }

        .quick-menu {
            position: absolute;
            top: calc(100% + 12px);
            right: 0;
            width: min(360px, calc(100vw - 24px));
            display: none;
            background: rgba(255, 255, 255, .98);
            border: 1px solid rgba(217, 226, 236, .95);
            border-radius: 22px;
            box-shadow: 0 20px 42px rgba(16, 42, 67, .18);
            padding: 12px;
            z-index: 30;
        }

        .quick-wrap.open .quick-menu {
            display: block;
        }

        .quick-title {
            margin: 2px 4px 10px;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .05em;
            text-transform: uppercase;
            color: var(--shop-ink-500);
        }

        .quick-list {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .quick-link {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            padding: 12px 14px;
            border-radius: 18px;
            border: 1px solid rgba(238, 77, 45, .14);
            background: linear-gradient(145deg, #ffffff, #fff7ed);
            color: var(--shop-ink-900);
            text-decoration: none;
            transition: .2s ease;
        }

        .quick-link:hover {
            transform: translateY(-1px);
            border-color: rgba(238, 77, 45, .26);
            box-shadow: 0 14px 28px rgba(238, 77, 45, .12);
        }

        .quick-copy {
            display: flex;
            flex-direction: column;
            min-width: 0;
        }

        .quick-copy strong {
            font-size: 14px;
            font-weight: 800;
            line-height: 1.3;
            color: var(--shop-ink-900);
        }

        .quick-copy small {
            margin-top: 2px;
            font-size: 11px;
            color: var(--shop-ink-500);
            line-height: 1.35;
        }

        .quick-badge {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            padding: 6px 10px;
            border-radius: 999px;
            background: rgba(238, 77, 45, .12);
            color: #d9480f;
            font-size: 11px;
            font-weight: 800;
            white-space: nowrap;
        }

        .mode-pill {
            padding: 6px 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 600;
            border: 1px solid #ffd8a8;
            background: #fff4e6;
            color: #d9480f;
            white-space: nowrap;
        }

        .avatar-wrap { position: relative; }

        .avatar-btn {
            border: 1px solid var(--shop-line);
            background: #fff;
            border-radius: 999px;
            min-height: 46px;
            padding: 4px 12px 4px 4px;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            cursor: pointer;
            transition: .2s ease;
        }

        .avatar-btn:hover,
        .avatar-wrap.open .avatar-btn {
            border-color: #ffb998;
            box-shadow: 0 8px 22px rgba(13, 143, 91, .16);
        }

        .avatar-image {
            width: 36px;
            height: 36px;
            border-radius: 50%;
            object-fit: cover;
            border: 1px solid #dce7f2;
            background: #f3f6fa;
        }

        .avatar-name {
            max-width: 140px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            font-weight: 800;
            font-size: 14px;
            color: var(--shop-ink-900);
        }

        .avatar-caret {
            color: var(--shop-ink-500);
            font-size: 12px;
        }

        .menu {
            display: none;
            position: absolute;
            right: 0;
            top: calc(100% + 8px);
            width: min(390px, 92vw);
            background: #fff;
            border: 1px solid #dce7f2;
            border-radius: var(--radius-md);
            box-shadow: 0 24px 40px rgba(16, 42, 67, .18);
            overflow: hidden;
            z-index: 1000;
        }

        .avatar-wrap.open .menu { display: block; }

        .menu-head {
            padding: 14px 16px;
            background: linear-gradient(135deg, rgba(238,77,45,.12), rgba(255,122,69,.08));
            border-bottom: 1px solid #dce7f2;
        }

        .menu-title {
            font-size: 16px;
            font-weight: 800;
            margin: 0;
        }

        .menu-sub {
            margin-top: 4px;
            font-size: 13px;
            color: var(--shop-ink-700);
        }

        .menu-body {
            max-height: 65vh;
            overflow: auto;
            padding: 6px 0;
        }

        .menu-group-title {
            margin: 10px 16px 6px;
            color: var(--shop-ink-500);
            font-size: 11px;
            font-weight: 800;
            text-transform: uppercase;
            letter-spacing: .08em;
        }

        .menu-item {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            min-height: 40px;
            padding: 0 16px;
            color: var(--shop-ink-900);
            font-size: 14px;
            font-weight: 700;
        }

        .menu-item:hover { background: #fff7f2; }
        .menu-item-active {
            background: #fff1ea;
            color: #c2410c;
        }

        .menu-badge {
            font-size: 11px;
            color: var(--shop-ink-500);
        }

        .menu-footer {
            border-top: 1px solid #e2e8f0;
            padding: 10px 14px;
            background: #f8fafc;
        }

        .btn-logout {
            width: 100%;
            border: none;
            border-radius: 12px;
            padding: 10px 14px;
            font-weight: 800;
            font-size: 14px;
            color: #fff;
            background: #e03131;
            cursor: pointer;
        }

        .shop-main {
            max-width: 1220px;
            margin: 0 auto;
            padding: 18px 16px 28px;
            display: grid;
            gap: 16px;
        }

        .hero {
            background: linear-gradient(135deg, var(--shop-green-700), var(--shop-green-600));
            border-radius: var(--radius-lg);
            color: #fff;
            padding: 16px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 18px;
            box-shadow: var(--shop-shadow);
        }

        .hero h1 {
            margin: 0 0 6px;
            font-size: 26px;
            line-height: 1.15;
        }

        .hero p {
            margin: 0;
            font-size: 14px;
            opacity: .95;
        }

        .hero-meta {
            font-size: 13px;
            margin-top: 6px;
            opacity: .9;
        }

        .hero-link {
            display: inline-flex;
            align-items: center;
            min-height: 38px;
            padding: 0 14px;
            border-radius: 999px;
            border: 1px solid rgba(255,255,255,.48);
            background: rgba(255,255,255,.12);
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            white-space: nowrap;
        }

        .hero-link:hover { background: rgba(255,255,255,.2); }

        .report-filters {
            background: var(--shop-card);
            border: 1px solid var(--shop-line);
            border-radius: var(--radius-md);
            padding: 14px;
            display: grid;
            grid-template-columns: repeat(5, minmax(0, 1fr));
            gap: 12px;
            align-items: end;
        }

        .filter-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .filter-field label {
            font-size: 12px;
            font-weight: 700;
            color: var(--shop-ink-500);
        }

        .filter-input {
            height: 38px;
            border-radius: 10px;
            border: 1px solid var(--shop-line);
            padding: 0 10px;
            font-size: 14px;
            outline: none;
            background: #fff;
        }

        .filter-actions {
            display: flex;
            gap: 8px;
        }

        .filter-actions .btn {
            height: 38px;
            border-radius: 10px;
            font-weight: 700;
            padding: 0 14px;
            white-space: nowrap;
        }

        .report-note {
            font-size: 12px;
            color: var(--shop-ink-500);
            margin-top: -4px;
        }

        .stats {
            margin-top: 6px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
        }

        .stat-card {
            background: var(--shop-card);
            border: 1px solid var(--shop-line);
            border-radius: var(--radius-md);
            padding: 12px;
        }

        .stat-label {
            font-size: 12px;
            color: var(--shop-ink-500);
            margin-bottom: 4px;
        }

        .stat-value {
            font-size: 22px;
            line-height: 1.1;
            font-weight: 800;
            color: var(--shop-ink-900);
        }

        .shop-card {
            margin-top: 14px;
            background: var(--shop-card);
            border: 1px solid var(--shop-line);
            border-radius: var(--radius-lg);
            box-shadow: var(--shop-shadow);
            overflow: hidden;
        }

        .shop-card-head {
            padding: 14px 16px;
            border-bottom: 1px solid #e7edf4;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .shop-card-title {
            margin: 0;
            font-size: 20px;
            line-height: 1.2;
            color: var(--shop-ink-900);
        }

        .shop-card-sub {
            margin-top: 3px;
            font-size: 13px;
            color: var(--shop-ink-500);
        }

        .shop-card-body { padding: 16px; }

        @media (max-width: 990px) {
            .report-filters { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .hero { flex-direction: column; align-items: flex-start; }
        }

        @media (max-width: 640px) {
            .shop-topbar { padding: 12px; }
            .shop-main { padding: 12px 12px 18px; }
            .brand-logo { max-width: 132px; height: 40px; border-radius: 12px; }
            .brand-logo img { height: 28px; }
            .brand-title { font-size: 17px; }
            .brand-sub { max-width: 240px; }
            .hero h1 { font-size: 22px; }
            .avatar-name { max-width: 82px; }
            .menu { width: min(360px, calc(100vw - 24px)); }
            .report-filters { grid-template-columns: 1fr; }
            .filter-actions { flex-direction: column; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="shop-shell">
        <asp:PlaceHolder ID="ph_shop_topbar" runat="server" Visible="true">
            <header class="shop-topbar">
                <div class="shop-topbar-left">
                    <uc1:SpaceLauncher runat="server" ID="spaceLauncher" ButtonCssClass="shop-space-toggle" />
                    <div class="brand">
                        <a href="/shop/default.aspx" class="brand-logo" aria-label="Trang chủ gian hàng đối tác">
                            <img src="<%= ResolveShopBrandLogoUrl() %>" alt="AHA" />
                        </a>
                        <div>
                            <div class="brand-title">AHA Shop Portal</div>
                            <div class="brand-sub">
                                Tài khoản: <asp:Label ID="lb_taikhoan" runat="server"></asp:Label>
                                • Link: <asp:Label ID="lb_public_path" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="top-actions">
                    <div class="mode-pill">Chế độ Shop</div>
                    <div class="quick-wrap" id="quickWrap">
                        <button type="button" class="quick-btn" id="quickToggle" title="Đi nhanh">
                            <i class="ti ti-plus"></i>
                        </button>
                        <div class="quick-menu" id="quickMenu">
                            <div class="quick-title">Đi nhanh gian hàng đối tác</div>
                            <div class="quick-list">
                                <a class="quick-link" href="/shop/default.aspx">
                                    <span class="quick-copy"><strong>Trang chủ shop</strong><small>Dashboard vận hành không gian /shop</small></span>
                                    <span class="quick-badge">Dashboard</span>
                                </a>
                                <a class="quick-link" href="/shop/bao-cao.aspx">
                                    <span class="quick-copy"><strong>Báo cáo</strong><small>Xem nhanh doanh thu và hiệu suất</small></span>
                                    <span class="quick-badge">Report</span>
                                </a>
                                <a class="quick-link" href="/shop/quan-ly-tin">
                                    <span class="quick-copy"><strong>Quản lý tin</strong><small>Đăng và cập nhật tin gian hàng đối tác</small></span>
                                    <span class="quick-badge">Tin</span>
                                </a>
                                <a class="quick-link" href="/shop/don-ban">
                                    <span class="quick-copy"><strong>Đơn bán</strong><small>Theo dõi đơn và giao dịch của shop</small></span>
                                    <span class="quick-badge">Đơn</span>
                                </a>
                                <a class="quick-link" href="/shop/khach-hang">
                                    <span class="quick-copy"><strong>Khách hàng</strong><small>Danh sách khách hàng của shop</small></span>
                                    <span class="quick-badge">CRM</span>
                                </a>
                            </div>
                        </div>
                    </div>
                    <div class="avatar-wrap" id="avatarWrap">
                        <button type="button" class="avatar-btn" id="avatarToggle">
                            <asp:Image ID="img_avatar" runat="server" CssClass="avatar-image" />
                            <span class="avatar-name"><asp:Label ID="lb_hoten_short" runat="server"></asp:Label></span>
                            <span class="avatar-caret">▼</span>
                        </button>

                        <div class="menu" id="avatarMenu">
                            <div class="menu-head">
                                <p class="menu-title"><asp:Label ID="lb_hoten" runat="server"></asp:Label></p>
                                <div class="menu-sub"><asp:Label ID="lb_phanloai" runat="server"></asp:Label> • <asp:Label ID="lb_trangthai" runat="server"></asp:Label></div>
                            </div>
                            <div class="menu-body">
                                <div class="menu-group-title">Trang chính</div>
                                <a class="menu-item" href="/shop/default.aspx"><span>Trang chủ gian hàng đối tác</span><span class="menu-badge">Dashboard</span></a>
                                <a class="menu-item menu-item-active" href="/shop/bao-cao.aspx"><span>Báo cáo</span><span class="menu-badge">Report</span></a>
                                <asp:HyperLink ID="lnk_public_shop" runat="server" CssClass="menu-item" Target="_blank"><span>Xem trang công khai</span><span class="menu-badge">Public</span></asp:HyperLink>
                                <asp:PlaceHolder ID="ph_switch_to_home" runat="server" Visible="false">
                                    <asp:HyperLink ID="lnk_switch_to_home" runat="server" CssClass="menu-item"><span>Chuyển sang tài khoản cá nhân</span><span class="menu-badge">Home</span></asp:HyperLink>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="ph_menu_company_space" runat="server" Visible="false">
                                    <div class="menu-group-title">Không gian gian hàng đối tác công ty</div>
                                    <asp:HyperLink ID="lnk_space_public" runat="server" CssClass="menu-item"><span>Không gian 1: Công khai</span><span class="menu-badge">Shop thường</span></asp:HyperLink>
                                    <asp:HyperLink ID="lnk_space_internal" runat="server" CssClass="menu-item"><span>Không gian 2: Nội bộ</span><span class="menu-badge">Sản phẩm thẻ</span></asp:HyperLink>
                                </asp:PlaceHolder>

                                <div class="menu-group-title">Vận hành gian hàng đối tác</div>
                                <a class="menu-item" href="/shop/quan-ly-tin"><span>Quản lý tin gian hàng đối tác</span><span class="menu-badge">Đăng/Sửa</span></a>
                                <asp:PlaceHolder ID="ph_menu_ban_san_pham" runat="server" Visible="false">
                                    <a class="menu-item" href="/shop/noi-bo/ban-san-pham"><span>Bán sản phẩm nội bộ</span><span class="menu-badge">Không gian 2</span></a>
                                </asp:PlaceHolder>
                                <a class="menu-item" href="/shop/don-ban"><span>Đơn bán</span><span class="menu-badge">Theo dõi</span></a>
                                <a class="menu-item" href="/shop/cho-thanh-toan"><span>Chờ trao đổi</span><span class="menu-badge">Xử lý</span></a>
                                <a class="menu-item" href="/shop/khach-hang"><span>Khách hàng</span><span class="menu-badge">Danh sách</span></a>

                                <div class="menu-group-title">Hồ sơ gian hàng đối tác</div>
                                <a class="menu-item" href="/shop/ho-so-tieu-dung"><span>Hồ sơ quyền tiêu dùng gian hàng đối tác</span><span class="menu-badge">Gian hàng đối tác-only</span></a>
                                <a class="menu-item" href="/shop/ho-so-uu-dai"><span>Hồ sơ quyền ưu đãi gian hàng đối tác</span><span class="menu-badge">Gian hàng đối tác-only</span></a>

                                <div class="menu-group-title">Thiết lập tài khoản</div>
                                <a class="menu-item" href="/shop/cap-nhat-ho-so"><span>Cập nhật hồ sơ</span><span class="menu-badge">Thông tin</span></a>
                                <a class="menu-item" href="/shop/doi-mat-khau"><span>Đổi mật khẩu</span><span class="menu-badge">Bảo mật</span></a>
                            </div>
                            <div class="menu-footer">
                                <asp:Button ID="but_dangxuat" runat="server" CssClass="btn-logout" Text="Đăng xuất tài khoản gian hàng đối tác" OnClick="but_dangxuat_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </header>
        </asp:PlaceHolder>

        <main class="shop-main">
            <section class="hero">
                <div>
                    <h1>Báo cáo gian hàng đối tác</h1>
                    <p>Theo dõi doanh thu, đơn hàng và hiệu suất theo khoảng thời gian.</p>
                    <div class="hero-meta">
                        <asp:Label ID="lb_range_label" runat="server" Text="Khoảng thời gian: Toàn thời gian"></asp:Label>
                    </div>
                </div>
                <asp:HyperLink ID="lnk_public_shop_top" runat="server" CssClass="hero-link" Target="_blank"></asp:HyperLink>
            </section>

            <section class="report-filters">
                <div class="filter-field">
                    <label for="txt_month">Tháng</label>
                    <asp:TextBox ID="txt_month" runat="server" CssClass="filter-input" TextMode="Month" placeholder="mm/yyyy"></asp:TextBox>
                </div>
                <div class="filter-field">
                    <label for="txt_from">Từ ngày</label>
                    <asp:TextBox ID="txt_from" runat="server" CssClass="filter-input" TextMode="Date" placeholder="dd/mm/yyyy"></asp:TextBox>
                </div>
                <div class="filter-field">
                    <label for="txt_to">Đến ngày</label>
                    <asp:TextBox ID="txt_to" runat="server" CssClass="filter-input" TextMode="Date" placeholder="dd/mm/yyyy"></asp:TextBox>
                </div>
                <div class="filter-field">
                    <label>&nbsp;</label>
                    <div class="filter-actions">
                        <asp:Button ID="btn_filter" runat="server" Text="Lọc báo cáo" CssClass="btn btn-primary" OnClick="btn_filter_Click" />
                        <asp:LinkButton ID="btn_clear" runat="server" Text="Toàn thời gian" CssClass="btn btn-outline-secondary" OnClick="btn_clear_Click" />
                    </div>
                </div>
                <div class="filter-field">
                    <label>Ghi chú</label>
                    <div class="report-note">Ưu tiên lọc theo tháng; nếu nhập Từ/Đến thì hệ thống dùng Từ/Đến. Nếu trình duyệt không cho chọn tháng, hãy gõ yyyy-mm hoặc mm/yyyy. Muốn đối chiếu với tab Đơn bán, bấm “Toàn thời gian”.</div>
                </div>
            </section>

            <section class="stats">
                <article class="stat-card">
                    <div class="stat-label">Doanh thu phát sinh (VNĐ)</div>
                    <div class="stat-value"><asp:Label ID="lb_revenue_gross" runat="server"></asp:Label></div>
                </article>
                <article class="stat-card">
                    <div class="stat-label">Điểm A hoàn tất (Hồ sơ tiêu dùng)</div>
                    <div class="stat-value"><asp:Label ID="lb_revenue_completed" runat="server"></asp:Label></div>
                </article>
                <article class="stat-card">
                    <div class="stat-label">Tổng đơn</div>
                    <div class="stat-value"><asp:Label ID="lb_total_orders" runat="server"></asp:Label></div>
                </article>
                <article class="stat-card">
                    <div class="stat-label">Đơn chờ trao đổi</div>
                    <div class="stat-value"><asp:Label ID="lb_pending_orders" runat="server"></asp:Label></div>
                </article>
                <article class="stat-card">
                    <div class="stat-label">Số lượng đã bán</div>
                    <div class="stat-value"><asp:Label ID="lb_total_sold" runat="server"></asp:Label></div>
                </article>
                <article class="stat-card">
                    <div class="stat-label">Tỷ lệ phản hồi</div>
                    <div class="stat-value"><asp:Label ID="lb_response_rate" runat="server"></asp:Label></div>
                </article>
                <article class="stat-card">
                    <div class="stat-label">Sản phẩm đang hiển thị</div>
                    <div class="stat-value"><asp:Label ID="lb_total_products" runat="server"></asp:Label></div>
                </article>
                <article class="stat-card">
                    <div class="stat-label">Lượt xem lũy kế</div>
                    <div class="stat-value"><asp:Label ID="lb_total_views" runat="server"></asp:Label></div>
                </article>
            </section>

            <section class="shop-card">
                <div class="shop-card-head">
                    <div>
                        <h2 class="shop-card-title">Trạng thái đơn bán</h2>
                        <div class="shop-card-sub">Tổng hợp theo trạng thái giao hàng và trao đổi trong khoảng thời gian đã lọc.</div>
                    </div>
                </div>
                <div class="shop-card-body">
                    <div class="stats">
                        <article class="stat-card">
                            <div class="stat-label">Đã đặt/Chưa Trao đổi</div>
                            <div class="stat-value"><asp:Label ID="lb_group_dat" runat="server"></asp:Label></div>
                        </article>
                        <article class="stat-card">
                            <div class="stat-label">Chờ Trao đổi</div>
                            <div class="stat-value"><asp:Label ID="lb_group_cho" runat="server"></asp:Label></div>
                        </article>
                        <article class="stat-card">
                            <div class="stat-label">Đã Trao đổi</div>
                            <div class="stat-value"><asp:Label ID="lb_group_da" runat="server"></asp:Label></div>
                        </article>
                        <article class="stat-card">
                            <div class="stat-label">Đã giao</div>
                            <div class="stat-value"><asp:Label ID="lb_group_giao" runat="server"></asp:Label></div>
                        </article>
                        <article class="stat-card">
                            <div class="stat-label">Đã nhận</div>
                            <div class="stat-value"><asp:Label ID="lb_group_nhan" runat="server"></asp:Label></div>
                        </article>
                        <article class="stat-card">
                            <div class="stat-label">Đã hủy</div>
                            <div class="stat-value"><asp:Label ID="lb_group_huy" runat="server"></asp:Label></div>
                        </article>
                    </div>
                </div>
            </section>
        </main>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="server">
    <script type="text/javascript">
        (function () {
            var wrap = document.getElementById('avatarWrap');
            var toggle = document.getElementById('avatarToggle');
            var quickWrap = document.getElementById('quickWrap');
            var quickToggle = document.getElementById('quickToggle');
            if (!wrap || !toggle) return;

            toggle.addEventListener('click', function (ev) {
                ev.preventDefault();
                ev.stopPropagation();
                wrap.classList.toggle('open');
            });

            if (quickWrap && quickToggle) {
                quickToggle.addEventListener('click', function (ev) {
                    ev.preventDefault();
                    ev.stopPropagation();
                    quickWrap.classList.toggle('open');
                });
            }

            document.addEventListener('click', function (ev) {
                if (!wrap.contains(ev.target)) {
                    wrap.classList.remove('open');
                }
                if (quickWrap && !quickWrap.contains(ev.target)) {
                    quickWrap.classList.remove('open');
                }
            });

            document.addEventListener('keydown', function (ev) {
                if (ev.key === 'Escape') {
                    wrap.classList.remove('open');
                    if (quickWrap) quickWrap.classList.remove('open');
                }
            });
        })();
    </script>
</asp:Content>
