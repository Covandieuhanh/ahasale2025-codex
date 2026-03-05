<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="shop_default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trang chủ Shop</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root {
            --shop-green-700: #0d8f5b;
            --shop-green-600: #12a36a;
            --shop-green-50: #effcf6;
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

        body {
            margin: 0;
            font-family: "Nunito Sans", "Segoe UI", Arial, sans-serif;
            background: var(--shop-bg);
            color: var(--shop-ink-900);
        }

        a { color: inherit; text-decoration: none; }

        .shop-shell {
            min-height: 100vh;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(18,163,106,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(13,143,91,.18), transparent 62%),
                var(--shop-bg);
        }

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
            box-shadow: 0 10px 24px rgba(13, 143, 91, .25);
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
            border-color: #9dc6ad;
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
            background: linear-gradient(135deg, rgba(13,143,91,.12), rgba(18,163,106,.08));
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

        .menu-item:hover { background: #f6fbff; }

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
            padding: 4px 16px 22px;
        }

        .hero {
            background: linear-gradient(130deg, #0f4c81, #0d8f5b 56%, #17b978);
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
            grid-template-columns: repeat(4, minmax(120px, 1fr));
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
            border: 1px solid #dce7f2;
            border-radius: var(--radius-md);
            overflow: hidden;
            background: #fff;
            display: flex;
            flex-direction: column;
            min-height: 100%;
        }

        .product-thumb {
            position: relative;
            width: 100%;
            padding-top: 78%;
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
            font-weight: 800;
            line-height: 1.35;
            color: var(--shop-ink-900);
            min-height: 40px;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .product-price {
            color: #0f8f5d;
            font-size: 18px;
            font-weight: 800;
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
            border: 1px solid #0f8f5d;
            color: #fff;
            background: #0f8f5d;
        }

        .btn-strong:hover { background: #0d7b50; }

        .btn-soft {
            min-height: 34px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 800;
            border: 1px solid #0f4c81;
            color: #0f4c81;
            background: #eef6ff;
        }

        .btn-soft:hover { background: #e2efff; }

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
            border: 1px solid #0f8f5d;
            background: #0f8f5d;
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
            .shop-main { padding: 2px 12px 18px; }
            .brand-title { font-size: 17px; }
            .brand-sub { max-width: 240px; }
            .hero h1 { font-size: 22px; }
            .product-grid { grid-template-columns: 1fr; }
            .avatar-name { max-width: 82px; }
            .menu { width: min(360px, calc(100vw - 24px)); }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shop-shell">
            <header class="shop-topbar">
                <div class="brand">
                    <a href="/shop/default.aspx" class="brand-logo" aria-label="Shop Home">
                        <img src="/uploads/images/favicon.png" alt="AHA" />
                    </a>
                    <div>
                        <div class="brand-title">AHA Shop Portal</div>
                        <div class="brand-sub">
                            Tài khoản: <asp:Label ID="lb_taikhoan" runat="server"></asp:Label>
                            • Link: <asp:Label ID="lb_public_path" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="top-actions">
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
                                <a class="menu-item" href="/shop/default.aspx"><span>Trang chủ shop</span><span class="menu-badge">Dashboard</span></a>
                                <asp:HyperLink ID="lnk_public_shop" runat="server" CssClass="menu-item" Target="_blank"><span>Xem trang công khai</span><span class="menu-badge">Public</span></asp:HyperLink>

                                <div class="menu-group-title">Vận hành gian hàng</div>
                                <a class="menu-item" href="/shop/quan-ly-tin"><span>Quản lý tin shop</span><span class="menu-badge">Đăng/Sửa</span></a>
                                <a class="menu-item" href="/shop/don-ban"><span>Đơn bán</span><span class="menu-badge">Theo dõi</span></a>
                                <a class="menu-item" href="/shop/cho-thanh-toan"><span>Chờ thanh toán</span><span class="menu-badge">Xử lý</span></a>
                                <a class="menu-item" href="/shop/khach-hang"><span>Khách hàng</span><span class="menu-badge">Danh sách</span></a>

                                <div class="menu-group-title">Hồ sơ shop</div>
                                <a class="menu-item" href="/shop/ho-so-tieu-dung"><span>Hồ sơ quyền tiêu dùng shop</span><span class="menu-badge">Shop-only</span></a>
                                <a class="menu-item" href="/shop/ho-so-uu-dai"><span>Hồ sơ quyền ưu đãi shop</span><span class="menu-badge">Shop-only</span></a>

                                <div class="menu-group-title">Thiết lập tài khoản</div>
                                <a class="menu-item" href="/shop/cap-nhat-ho-so"><span>Cập nhật hồ sơ</span><span class="menu-badge">Thông tin</span></a>
                                <a class="menu-item" href="/shop/doi-mat-khau"><span>Đổi mật khẩu</span><span class="menu-badge">Bảo mật</span></a>
                            </div>
                            <div class="menu-footer">
                                <asp:Button ID="but_dangxuat" runat="server" CssClass="btn-logout" Text="Đăng xuất tài khoản shop" OnClick="but_dangxuat_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </header>

            <main class="shop-main">
                <section class="hero">
                    <div>
                        <h1>Trang chủ gian hàng</h1>
                        <p>Hiển thị toàn bộ sản phẩm thuộc tài khoản shop của bạn. Mọi thao tác quản lý gom trong menu avatar góc phải.</p>
                    </div>
                    <asp:HyperLink ID="lnk_public_shop_top" runat="server" CssClass="hero-link" Target="_blank"></asp:HyperLink>
                </section>

                <section class="stats">
                    <article class="stat-card">
                        <div class="stat-label">Sản phẩm đang hiển thị</div>
                        <div class="stat-value"><asp:Label ID="lb_total_products" runat="server"></asp:Label></div>
                    </article>
                    <article class="stat-card">
                        <div class="stat-label">Số lượt xem</div>
                        <div class="stat-value"><asp:Label ID="lb_total_views" runat="server"></asp:Label></div>
                    </article>
                    <article class="stat-card">
                        <div class="stat-label">Số lượng đã bán</div>
                        <div class="stat-value"><asp:Label ID="lb_total_sold" runat="server"></asp:Label></div>
                    </article>
                    <article class="stat-card">
                        <div class="stat-label">Đơn chờ thanh toán</div>
                        <div class="stat-value"><asp:Label ID="lb_pending_orders" runat="server"></asp:Label></div>
                    </article>
                </section>

                <section class="shop-card">
                    <div class="shop-card-head">
                        <div>
                            <h2 class="shop-card-title">Sản phẩm của gian hàng</h2>
                            <div class="shop-card-sub">Trang chủ shop chỉ hiển thị sản phẩm do chính tài khoản shop này đăng.</div>
                        </div>
                    </div>

                    <div class="shop-card-body">
                        <asp:PlaceHolder ID="ph_empty_products" runat="server" Visible="false">
                            <div class="empty-state">
                                <h3>Chưa có sản phẩm nào</h3>
                                <p>Hãy tạo sản phẩm đầu tiên để bắt đầu vận hành gian hàng. Tất cả thao tác nằm trong menu avatar góc phải.</p>
                                <button type="button" class="empty-menu-btn" id="btnOpenMenu">Mở menu tài khoản</button>
                            </div>
                        </asp:PlaceHolder>

                        <div class="product-grid">
                            <asp:Repeater ID="rp_products" runat="server">
                                <ItemTemplate>
                                    <article class="product-card">
                                        <a class="product-thumb" href="/shop/san-pham/<%# Eval("id") %>">
                                            <img src="<%# ResolveProductImage(Eval("image")) %>" alt="<%# Eval("name") %>" />
                                        </a>

                                        <div class="product-body">
                                            <a class="product-name" href="/shop/san-pham/<%# Eval("id") %>"><%# Eval("name") %></a>
                                            <div class="product-price"><%# FormatCurrency(Eval("giaban")) %> đ</div>
                                            <div class="product-meta">
                                                <span><%# Eval("ngaytao", "{0:dd/MM/yyyy}") %></span>
                                                <span>Lượt xem: <%# Eval("LuotTruyCap") %></span>
                                            </div>
                                            <div class="product-actions">
                                                <a class="btn-strong" href="/shop/san-pham/<%# Eval("id") %>">Xem sản phẩm</a>
                                                <a class="btn-soft" href="<%# BuildCreateOrderUrl(Eval("id")) %>">Tạo đơn</a>
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
    </form>

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
</body>
</html>
