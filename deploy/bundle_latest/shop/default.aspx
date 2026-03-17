<%@ Page Title="Trang chủ gian hàng đối tác" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="shop_default" %>

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
            font-family: "Nunito Sans", "Segoe UI", Arial, sans-serif;
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

        .brand {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 0;
        }

        .brand-logo {
            width: 44px;
            height: 44px;
            border-radius: 14px;
            background: linear-gradient(145deg, var(--shop-green-700), var(--shop-green-600));
            display: inline-flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 10px 24px rgba(238, 77, 45, .25);
            overflow: hidden;
        }

        .brand-logo img {
            width: 26px;
            height: 26px;
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

        .level2-banner {
            margin: 14px 0 0;
            padding: 12px 16px;
            border-radius: 14px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            font-size: 14px;
        }

        .level2-banner.pending {
            background: #fff7ed;
            border: 1px solid #fed7aa;
            color: #9a3412;
        }

        .level2-banner.rejected {
            background: #fff1f2;
            border: 1px solid #fecdd3;
            color: #9f1239;
        }

        .level2-banner a {
            color: inherit;
            font-weight: 700;
            text-decoration: underline;
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
            font-weight: 700;
        }

        .menu-footer {
            padding: 10px 16px 14px;
            border-top: 1px solid #e6eef7;
            background: #fcfdff;
        }

        .btn-logout {
            width: 100%;
            min-height: 38px;
            border-radius: 10px;
            border: 0;
            background: #db2f2f;
            color: #fff;
            font-weight: 800;
            cursor: pointer;
        }

        .shop-main {
            max-width: 1220px;
            margin: 0 auto;
            padding: 18px 16px 22px;
        }

        .hero {
            background: linear-gradient(130deg, #ff9a3d, #ee4d2d 56%, #ff7a45);
            color: #fff;
            border-radius: var(--radius-lg);
            box-shadow: var(--shop-shadow);
            padding: 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 14px;
            flex-wrap: wrap;
        }

        .hero h1 {
            margin: 0 0 5px;
            font-size: 26px;
            line-height: 1.15;
        }

        .hero p {
            margin: 0;
            font-size: 14px;
            opacity: .95;
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

        .stats {
            margin-top: 14px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
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
            font-size: 24px;
            line-height: 1.1;
            font-weight: 800;
            color: var(--shop-ink-900);
        }

        .top-products {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 12px;
        }

        .top-product {
            display: flex;
            gap: 10px;
            align-items: center;
            padding: 10px;
            border-radius: var(--radius-sm);
            border: 1px solid var(--shop-line);
            background: #fff;
        }

        .top-product img {
            width: 56px;
            height: 56px;
            border-radius: 12px;
            object-fit: cover;
            border: 1px solid #e1e8f0;
            background: #f3f6fa;
        }

        .top-product-title {
            font-weight: 800;
            color: var(--shop-ink-900);
            line-height: 1.2;
        }

        .top-product-meta {
            font-size: 12px;
            color: var(--shop-ink-500);
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

        .product-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(180px, 1fr));
            gap: 14px;
        }

        .product-card {
            border: none;
            border-radius: var(--radius-md);
            overflow: hidden;
            background: #fff;
            display: flex;
            flex-direction: column;
            min-height: 100%;
            box-shadow: 0 2px 14px rgba(0,0,0,.06);
            transition: transform .18s ease, box-shadow .18s ease;
        }

        .product-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 12px 24px rgba(16, 42, 67, .16);
        }

        .product-thumb {
            position: relative;
            width: 100%;
            padding-top: 100%;
            background: #f4f8fd;
            overflow: hidden;
            display: block;
        }

        .product-thumb img {
            position: absolute;
            inset: 0;
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .product-body {
            padding: 12px;
            display: flex;
            flex-direction: column;
            gap: 8px;
            flex: 1;
        }

        .product-name {
            font-size: 15px;
            font-weight: 700;
            line-height: 1.35;
            color: var(--shop-ink-900);
            min-height: 40px;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .product-price {
            color: #d63939;
            font-size: 16px;
            font-weight: 800;
        }

        .product-badges {
            display: flex;
            align-items: center;
            gap: 6px;
            flex-wrap: wrap;
        }

        .product-badge {
            display: inline-flex;
            align-items: center;
            min-height: 22px;
            border-radius: 999px;
            padding: 0 8px;
            font-size: 11px;
            font-weight: 800;
        }

        .product-badge-public {
            color: #c2410c;
            background: #fff3ed;
            border: 1px solid #ffd4c4;
        }

        .product-badge-internal {
            color: #9a3412;
            background: #fff3e8;
            border: 1px solid #ffd9bf;
        }

        .product-meta {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
            font-size: 12px;
            color: var(--shop-ink-500);
        }

        .product-actions {
            margin-top: auto;
            display: grid;
            grid-template-columns: 1fr;
            gap: 8px;
        }

        .btn-strong {
            min-height: 34px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 800;
        }

        .btn-strong {
            border: 1px solid #ee4d2d;
            color: #fff;
            background: #ee4d2d;
        }

        .btn-strong:hover { background: #d94327; }

        .btn-soft {
            min-height: 34px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 800;
            border: 1px solid #ee4d2d;
            color: #ee4d2d;
            background: #fff1ea;
        }

        .btn-soft:hover { background: #ffe4d6; }

        .empty-state {
            border: 1px dashed #c3d3e5;
            border-radius: var(--radius-md);
            background: #fbfdff;
            padding: 22px;
            text-align: center;
            color: var(--shop-ink-700);
        }

        .empty-state h3 {
            margin: 0 0 8px;
            font-size: 20px;
            color: var(--shop-ink-900);
        }

        .empty-state p {
            margin: 0 0 12px;
            font-size: 14px;
            color: var(--shop-ink-500);
        }

        .empty-menu-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 36px;
            padding: 0 14px;
            border-radius: 10px;
            border: 1px solid #ee4d2d;
            background: #ee4d2d;
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            cursor: pointer;
        }

        @media (max-width: 1140px) {
            .product-grid { grid-template-columns: repeat(3, minmax(180px, 1fr)); }
        }

        @media (max-width: 900px) {
            .stats { grid-template-columns: repeat(2, minmax(120px, 1fr)); }
            .product-grid { grid-template-columns: repeat(2, minmax(150px, 1fr)); }
        }

        @media (max-width: 640px) {
            .shop-topbar { padding: 12px; }
            .shop-main { padding: 12px 12px 18px; }
            .brand-title { font-size: 17px; }
            .brand-sub { max-width: 240px; }
            .hero h1 { font-size: 22px; }
            .product-grid { grid-template-columns: 1fr; }
            .avatar-name { max-width: 82px; }
            .menu { width: min(360px, calc(100vw - 24px)); }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
        <div class="shop-shell">
            <asp:PlaceHolder ID="ph_shop_topbar" runat="server" Visible="false">
            <header class="shop-topbar">
                <div class="brand">
                    <a href="/shop/default.aspx" class="brand-logo" aria-label="Trang chủ gian hàng đối tác">
                        <img src="/uploads/images/favicon.png" alt="AHA" />
                    </a>
                    <div>
                        <div class="brand-title"><asp:Label ID="lb_shop_brand_title" runat="server" Text="AHA Shop Portal"></asp:Label></div>
                        <div class="brand-sub">
                            Tài khoản: <asp:Label ID="lb_taikhoan" runat="server"></asp:Label>
                            • Link: <asp:Label ID="lb_public_path" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="top-actions">
                    <div class="mode-pill"><asp:Label ID="lb_shop_level" runat="server" Text="Level 1"></asp:Label> • <asp:Label ID="lb_shop_level_hint" runat="server" Text="Đang dùng bộ công cụ cơ bản trong /shop"></asp:Label></div>
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
                                <a class="menu-item" href="/shop/bao-cao.aspx"><span>Báo cáo</span><span class="menu-badge">Report</span></a>
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
                                <a class="menu-item" href="/shop/quan-ly-lich-hen.aspx"><span>Lịch hẹn cơ bản</span><span class="menu-badge">Basic</span></a>
                                <asp:PlaceHolder ID="ph_menu_ban_san_pham" runat="server" Visible="false">
                                    <a class="menu-item" href="/shop/noi-bo/ban-san-pham"><span>Bán sản phẩm nội bộ</span><span class="menu-badge">Không gian 2</span></a>
                                </asp:PlaceHolder>
                                <a class="menu-item" href="/shop/don-ban"><span>Đơn bán</span><span class="menu-badge">Theo dõi</span></a>
                                <a class="menu-item" href="/shop/cho-thanh-toan"><span>Chờ trao đổi</span><span class="menu-badge">Xử lý</span></a>
                                <a class="menu-item" href="/shop/khach-hang"><span>Khách hàng</span><span class="menu-badge">Danh sách</span></a>

                                <asp:PlaceHolder ID="ph_menu_advanced_tools" runat="server" Visible="false">
                                    <div class="menu-group-title">Bộ công cụ nâng cao</div>
                                    <a class="menu-item" href="/shop/nang-cap-level2.aspx"><span>Thiết lập Level 2</span><span class="menu-badge">Quản trị</span></a>
                                    <a class="menu-item" href="/gianhang/admin/login.aspx"><span>Vào /gianhang/admin</span><span class="menu-badge">Level 2</span></a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="ph_menu_advanced_upgrade" runat="server" Visible="false">
                                    <div class="menu-group-title">Bộ công cụ nâng cao</div>
                                    <a class="menu-item" href="/shop/nang-cap-level2.aspx"><span>Chưa kích hoạt /gianhang/admin</span><span class="menu-badge">Yêu cầu Level 2</span></a>
                                </asp:PlaceHolder>

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

            <asp:PlaceHolder ID="ph_level2_pending_notice" runat="server" Visible="false">
                <div class="level2-banner pending">
                    <div>
                        Yêu cầu nâng cấp Level 2 đang chờ duyệt.
                        <span>Gửi lúc <asp:Label ID="lb_level2_pending_time" runat="server" Text="--"></asp:Label></span>
                    </div>
                    <a href="/shop/nang-cap-level2.aspx">Xem chi tiết</a>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="ph_level2_reject_notice" runat="server" Visible="false">
                <div class="level2-banner rejected">
                    <div>
                        Yêu cầu nâng cấp Level 2 đã bị từ chối.
                        <span>Lý do: <asp:Label ID="lb_level2_reject_note" runat="server" Text="Admin chưa ghi chú."></asp:Label></span>
                    </div>
                    <a href="/shop/nang-cap-level2.aspx">Gửi lại yêu cầu</a>
                </div>
            </asp:PlaceHolder>

            <main class="shop-main">
                <section class="hero">
                    <div>
                        <h1><asp:Label ID="lb_space_hero_title" runat="server" Text="Không gian 1: Gian hàng công khai"></asp:Label></h1>
                        <p><asp:Label ID="lb_space_hero_desc" runat="server" Text="Giống gian hàng đối tác thường: quản lý sản phẩm công khai, đơn bán, khách hàng và trao đổi."></asp:Label></p>
                        <div class="product-actions mt-3">
                            <asp:HyperLink ID="lnk_shop_primary_cta" runat="server" CssClass="btn-strong" Visible="false"></asp:HyperLink>
                            <asp:HyperLink ID="lnk_shop_secondary_cta" runat="server" CssClass="btn-soft" Visible="false"></asp:HyperLink>
                        </div>
                    </div>
                    <asp:HyperLink ID="lnk_public_shop_top" runat="server" CssClass="hero-link" Target="_blank"></asp:HyperLink>
                </section>

                <section class="shop-card">
                    <div class="shop-card-head">
                        <div>
                            <h2 class="shop-card-title"><asp:Label ID="lb_top_products_title" runat="server" Text="Top sản phẩm"></asp:Label></h2>
                            <div class="shop-card-sub"><asp:Label ID="lb_top_products_desc" runat="server" Text="Ưu tiên theo bán chạy, nếu chưa có sẽ theo lượt xem."></asp:Label></div>
                        </div>
                    </div>
                    <div class="shop-card-body">
                        <asp:PlaceHolder ID="ph_top_products" runat="server" Visible="false">
                            <div class="top-products">
                                <asp:Repeater ID="rpt_top_products" runat="server">
                                    <ItemTemplate>
                                        <a class="top-product" href="/shop/san-pham/<%# Eval("Id") %>">
                                            <img src="<%# Eval("Image") %>" alt="<%# Eval("Name") %>" loading="lazy" decoding="async" />
                                            <div>
                                                <div class="top-product-title"><%# Eval("Name") %></div>
                                                <div class="top-product-meta">Đã bán: <%# Eval("Sold") %> · Lượt xem: <%# Eval("Views") %></div>
                                            </div>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_empty_top_products" runat="server" Visible="false">
                            <div class="empty-state">
                                <h3>Chưa có dữ liệu top sản phẩm</h3>
                                <p>Hãy đăng bán sản phẩm và có giao dịch để hệ thống tự tổng hợp.</p>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </section>

                <section class="shop-card">
                    <div class="shop-card-head">
                        <div>
                            <h2 class="shop-card-title"><asp:Label ID="lb_space_product_title" runat="server" Text="Sản phẩm của gian hàng đối tác"></asp:Label></h2>
                            <div class="shop-card-sub"><asp:Label ID="lb_space_product_desc" runat="server" Text="Trang chủ gian hàng đối tác chỉ hiển thị sản phẩm do chính tài khoản gian hàng đối tác này đăng."></asp:Label></div>
                        </div>
                    </div>

                    <div class="shop-card-body">
                        <asp:PlaceHolder ID="ph_empty_products" runat="server" Visible="false">
                            <div class="empty-state">
                                <h3>Chưa có sản phẩm nào</h3>
                                <p>Hãy tạo sản phẩm đầu tiên để bắt đầu vận hành gian hàng đối tác. Tất cả thao tác nằm trong menu avatar góc phải.</p>
                                <button type="button" class="empty-menu-btn" id="btnOpenMenu">Mở menu tài khoản</button>
                            </div>
                        </asp:PlaceHolder>

                        <div class="product-grid">
                            <asp:Repeater ID="rp_products" runat="server">
                                <ItemTemplate>
                                    <article class="product-card">
                                        <a class="product-thumb" href="/shop/san-pham/<%# Eval("id") %>">
                                            <img src="<%# ResolveProductImage(Eval("image")) %>" alt="<%# Eval("name") %>" loading="lazy" decoding="async" />
                                        </a>

                                        <div class="product-body">
                                            <a class="product-name" href="/shop/san-pham/<%# Eval("id") %>"><%# Eval("name") %></a>
                                            <div class="product-price"><%# FormatCurrency(Eval("giaban")) %> đ</div>
                                            <div class="product-badges">
                                                <span class="<%# BuildProductChannelCss(Eval("KenhRaw")) %>"><%# BuildProductChannelLabel(Eval("KenhRaw")) %></span>
                                            </div>
                                            <div class="product-meta">
                                                <span>Ngày đăng: <%# Eval("ngaytao", "{0:dd/MM/yyyy}") %></span>
                                                <span>Lượt xem: <%# Eval("LuotTruyCap") %></span>
                                            </div>
                                            <div class="product-actions">
                                                <a class="btn-strong" href="/shop/san-pham/<%# Eval("id") %>">Xem chi tiết</a>
                                                <a class="btn-soft" href="<%# BuildSellActionUrl(Eval("id"), Eval("name_en"), Eval("name"), Eval("KenhRaw")) %>"><%# BuildSellActionText(Eval("KenhRaw")) %></a>
                                            </div>
                                        </div>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
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
            if (!wrap || !toggle) return;

            toggle.addEventListener('click', function (ev) {
                ev.preventDefault();
                ev.stopPropagation();
                wrap.classList.toggle('open');
            });

            document.addEventListener('click', function (ev) {
                if (!wrap.contains(ev.target)) {
                    wrap.classList.remove('open');
                }
            });

            document.addEventListener('keydown', function (ev) {
                if (ev.key === 'Escape') {
                    wrap.classList.remove('open');
                }
            });

            var btnOpenMenu = document.getElementById('btnOpenMenu');
            if (btnOpenMenu) {
                btnOpenMenu.addEventListener('click', function () {
                    wrap.classList.add('open');
                });
            }
        })();
    </script>
</asp:Content>
