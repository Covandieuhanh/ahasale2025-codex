<%@ Page Language="C#" AutoEventWireup="true" CodeFile="public.aspx.cs" Inherits="shop_public" %>
<%@ Import Namespace="System.Web" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trang công khai gian hàng đối tác</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root {
            --shop-bg: #f4f8fc;
            --shop-card: #ffffff;
            --shop-line: #d8e3f0;
            --shop-text: #15314f;
            --shop-muted: #5b738d;
            --shop-brand: #ee4d2d;
            --shop-brand-2: #ff7a45;
        }

        * { box-sizing: border-box; }

        body {
            margin: 0;
            font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif;
            color: var(--shop-text);
            background:
                radial-gradient(920px 280px at 18% -4%, rgba(238, 77, 45, .12), transparent 62%),
                radial-gradient(920px 320px at 88% -6%, rgba(255, 122, 69, .16), transparent 62%),
                var(--shop-bg);
        }

        a { text-decoration: none; color: inherit; }

        .shell {
            width: min(1220px, 100% - 28px);
            margin: 18px auto 28px;
        }

        .hero {
            position: relative;
            border: 1px solid var(--shop-line);
            border-radius: 20px;
            overflow: hidden;
            background: var(--shop-card);
            box-shadow: 0 18px 44px rgba(16, 42, 67, .10);
        }

        .hero-cover {
            position: absolute;
            left: 0;
            right: 0;
            top: 0;
            height: 160px;
            background-size: cover;
            background-position: center;
            opacity: .22;
            filter: saturate(1.05);
        }

        .hero.has-cover .hero-cover {
            opacity: .42;
        }

        .hero.has-cover {
            padding-top: 160px;
        }

        .hero-inner {
            position: relative;
            z-index: 1;
        }

        .hero-top {
            background: linear-gradient(130deg, var(--shop-brand), var(--shop-brand-2));
            color: #fff;
            padding: 16px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .hero.has-cover .hero-top {
            background: linear-gradient(130deg, rgba(238, 77, 45, .78), rgba(255, 122, 69, .78));
        }
        .hero-title {
            margin: 0;
            font-size: 24px;
            line-height: 1.2;
            font-weight: 900;
        }

        .hero-sub {
            margin-top: 4px;
            font-size: 13px;
            color: rgba(255, 255, 255, .88);
        }

        .btn-login {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            border-radius: 12px;
            padding: 0 14px;
            font-weight: 800;
            background: #fff;
            color: var(--shop-brand);
            border: 1px solid rgba(255, 255, 255, .65);
        }

        .hero-main {
            padding: 18px;
            display: grid;
            grid-template-columns: 98px 1fr;
            gap: 16px;
            align-items: center;
            border-bottom: 1px solid #edf2f8;
            background: rgba(255, 255, 255, .9);
        }

        .shop-avatar {
            width: 98px;
            height: 98px;
            border-radius: 50%;
            object-fit: cover;
            border: 3px solid #dbe8f6;
            background: #ecf3fb;
        }

        .meta-title {
            margin: 0;
            font-size: 30px;
            line-height: 1.12;
            font-weight: 900;
        }

        .meta-desc {
            margin-top: 8px;
            font-size: 14px;
            color: rgba(21, 49, 79, .82);
            font-weight: 600;
        }

        .meta-line {
            margin-top: 6px;
            color: var(--shop-muted);
            font-size: 14px;
        }

        .meta-actions {
            margin-top: 12px;
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 38px;
            border-radius: 10px;
            padding: 0 14px;
            font-weight: 800;
            background: var(--shop-brand);
            color: #fff;
            border: 1px solid rgba(238, 77, 45, .45);
        }

        .stats {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            padding: 16px 18px 18px;
        }

        .stat {
            border: 1px solid var(--shop-line);
            border-radius: 14px;
            background: #f9fbfe;
            padding: 12px 14px;
            min-height: 82px;
        }

        .stat-label {
            font-size: 13px;
            color: var(--shop-muted);
        }

        .stat-value {
            margin-top: 4px;
            font-size: 30px;
            line-height: 1.1;
            font-weight: 900;
            color: var(--shop-text);
        }

        .featured {
            margin-top: 16px;
            border: 1px solid var(--shop-line);
            border-radius: 20px;
            background: var(--shop-card);
            box-shadow: 0 16px 32px rgba(16, 42, 67, .08);
            overflow: hidden;
        }

        .featured-head {
            padding: 16px 18px;
            border-bottom: 1px solid #edf2f8;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .featured-title {
            margin: 0;
            font-size: 22px;
            line-height: 1.2;
            font-weight: 900;
        }

        .featured-sub {
            color: var(--shop-muted);
            font-size: 13px;
            margin-top: 4px;
        }

        .featured-grid .grid-item { width: 100%; }

        @media (max-width: 767.98px) {
            .featured-grid .grid-item { width: calc((100% - 12px) / 2); }
        }

        @media (min-width: 768px) and (max-width: 991.98px) {
            .featured-grid .grid-item { width: calc((100% - 12px*2) / 3); }
        }

        @media (min-width: 992px) {
            .featured-grid .grid-item { width: calc((100% - 12px*3) / 4); }
        }

        .products {
            margin-top: 16px;
            border: 1px solid var(--shop-line);
            border-radius: 20px;
            background: var(--shop-card);
            box-shadow: 0 16px 32px rgba(16, 42, 67, .08);
            overflow: hidden;
        }

        .products-head {
            padding: 16px 18px;
            border-bottom: 1px solid #edf2f8;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .products-title {
            margin: 0;
            font-size: 24px;
            line-height: 1.2;
            font-weight: 900;
        }

        .products-sub {
            color: var(--shop-muted);
            font-size: 13px;
            margin-top: 4px;
        }

        .public-url {
            color: var(--shop-muted);
            font-size: 12px;
            font-weight: 700;
            background: #f7fbff;
            border: 1px solid var(--shop-line);
            border-radius: 999px;
            padding: 7px 12px;
        }

        .filters {
            width: 100%;
            display: grid;
            grid-template-columns: repeat(6, minmax(0, 1fr));
            gap: 10px;
            margin-top: 14px;
        }

        .filter-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .filter-field label {
            font-size: 11px;
            font-weight: 700;
            color: var(--shop-muted);
            text-transform: uppercase;
            letter-spacing: .04em;
        }

        .filter-input,
        .filter-select {
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
            align-items: flex-end;
            gap: 8px;
        }

        .filter-btn {
            height: 38px;
            border-radius: 10px;
            border: 1px solid var(--shop-line);
            background: #f7fbff;
            font-weight: 700;
            cursor: pointer;
            padding: 0 12px;
        }

        .filter-count {
            font-size: 12px;
            color: var(--shop-muted);
            margin-top: 6px;
        }

        .grid {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            padding: 14px;
        }

        .grid-item {
            width: 100%;
        }

        @media (max-width: 575.98px) {
            .grid-item { width: calc((100% - 12px) / 2); }
        }

        @media (min-width: 576px) and (max-width: 767.98px) {
            .grid-item { width: calc((100% - 12px) / 2); }
        }

        @media (min-width: 768px) and (max-width: 991.98px) {
            .grid-item { width: calc((100% - 12px*2) / 3); }
        }

        @media (min-width: 992px) and (max-width: 1199.98px) {
            .grid-item { width: calc((100% - 12px*3) / 4); }
        }

        @media (min-width: 1200px) {
            .grid-item { width: calc((100% - 12px*5) / 6); }
        }

        .card {
            border: none;
            border-radius: 12px;
            overflow: hidden;
            background: #fff;
            transition: transform .18s ease, box-shadow .18s ease;
            display: flex;
            flex-direction: column;
            min-height: 100%;
            box-shadow: 0 2px 14px rgba(0,0,0,.06);
        }

        .card:hover {
            transform: translateY(-2px);
            box-shadow: 0 12px 24px rgba(16, 42, 67, .16);
        }

        .card-link {
            display: block;
            color: inherit;
        }

        .card-image {
            width: 100%;
            aspect-ratio: 1 / 1;
            object-fit: cover;
            background: #edf4fc;
            display: block;
            transition: transform .25s ease;
        }

        .card:hover .card-image {
            transform: scale(1.04);
        }

        .card-body {
            padding: 10px 10px 12px;
            display: flex;
            flex-direction: column;
            min-height: 100px;
        }

        .card-name {
            margin: 0;
            font-size: 14px;
            line-height: 1.15rem;
            font-weight: 700;
            color: var(--shop-text);
            min-height: 2.3rem;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .card-meta {
            color: var(--shop-muted);
            font-size: 12px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .card-meta-row {
            margin-top: 8px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
        }

        .card-badges {
            margin-top: 8px;
            display: flex;
            align-items: center;
            gap: 6px;
            flex-wrap: wrap;
        }

        .card-badge {
            display: inline-flex;
            align-items: center;
            min-height: 22px;
            border-radius: 999px;
            padding: 0 8px;
            font-size: 11px;
            font-weight: 800;
        }

        .card-badge-public {
            color: #c2410c;
            background: #fff3ed;
            border: 1px solid #ffd4c4;
        }

        .card-badge-service {
            color: #9a3412;
            background: #ffedd5;
            border: 1px solid #fed7aa;
        }

        .card-badge-category {
            color: #c2410c;
            background: #fff1ea;
            border: 1px solid #ffd4c4;
        }

        .card-price {
            margin-top: 8px;
            color: #d63939;
            font-size: 16px;
            font-weight: 800;
            line-height: 1.2;
        }

        .card-actions {
            margin-top: auto;
            padding: 0 12px 12px;
            display: grid;
            grid-template-columns: 1fr;
            gap: 8px;
        }

        .card-btn {
            min-height: 34px;
            border-radius: 10px;
            border: 1px solid transparent;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
        }

        .card-btn-trade {
            background: #ee4d2d;
            border-color: #ee4d2d;
            color: #fff;
        }

        .card-btn-trade:hover {
            color: #fff;
            background: #d94327;
            border-color: #d94327;
        }

        @media (min-width: 1200px) {
            .card-body { padding: 8px 8px 10px; min-height: 94px; }
            .card-meta { font-size: 11px; }
            .card-badge { font-size: 10px; min-height: 20px; }
            .card-price { font-size: 14px; margin-top: 6px; }
            .card-actions { padding: 0 8px 10px; }
            .card-btn { min-height: 30px; font-size: 12px; }
        }

        .card-btn-cart {
            background: #fff1ea;
            border-color: #ffd4c4;
            color: #ee4d2d;
        }

        .card-btn-cart:hover {
            color: #c2410c;
            background: #ffe4d6;
            border-color: #ffc2ad;
        }

        .empty {
            padding: 26px 16px 30px;
            text-align: center;
            color: var(--shop-muted);
            font-weight: 700;
        }

        @media (max-width: 960px) {
            .stats { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .meta-title { font-size: 26px; }
            .featured-title { font-size: 20px; }
        }

        @media (max-width: 680px) {
            .shell { width: min(1220px, 100% - 16px); margin-top: 10px; }
            .hero-main { grid-template-columns: 72px 1fr; padding: 14px; }
            .shop-avatar { width: 72px; height: 72px; }
            .meta-title { font-size: 22px; }
            .stats { grid-template-columns: 1fr; padding: 12px 14px 14px; }
            .products-head { padding: 14px; }
            .products-title { font-size: 21px; }
            .grid { padding: 12px; gap: 12px; }
            .filters { grid-template-columns: 1fr 1fr; }
            .featured-head { padding: 14px; }
        }

        @media (max-width: 420px) {
            .filters { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shell">
            <section id="hero_section" runat="server" class="hero">
                <asp:Panel ID="pn_cover" runat="server" CssClass="hero-cover" Visible="false"></asp:Panel>
                <div class="hero-inner">
                    <div class="hero-top">
                        <div>
                            <h1 class="hero-title"><asp:Label ID="lb_public_hero_title" runat="server" Text="Trang công khai gian hàng đối tác"></asp:Label></h1>
                            <div class="hero-sub"><asp:Label ID="lb_public_hero_sub" runat="server" Text="Trang công khai cho phép xem tự do. Nếu muốn thao tác trao đổi, vui lòng đăng nhập tài khoản Home."></asp:Label></div>
                        </div>
                        <div style="display:flex; gap:8px; flex-wrap:wrap;">
                            <asp:HyperLink ID="lnk_home_login" runat="server" CssClass="btn-login" />
                            <asp:HyperLink ID="lnk_public_primary_cta" runat="server" CssClass="btn-login" Visible="false" />
                            <a class="btn-login" href="/shop/default.aspx">Trang chủ gian hàng đối tác</a>
                            <a class="btn-login" href="/shop/dang-nhap">Đăng nhập gian hàng đối tác</a>
                        </div>
                    </div>

                    <div class="hero-main">
                        <asp:Image ID="img_avatar" runat="server" CssClass="shop-avatar" />
                        <div>
                            <h2 class="meta-title"><asp:Label ID="lb_shop_name" runat="server" /></h2>
                            <asp:PlaceHolder ID="ph_shop_desc" runat="server" Visible="false">
                                <div class="meta-desc"><asp:Label ID="lb_shop_desc" runat="server" /></div>
                            </asp:PlaceHolder>
                            <div class="meta-line">Mã gian hàng đối tác: <asp:Label ID="lb_shop_slug" runat="server" /></div>
                            <div class="meta-line">Chủ gian hàng đối tác: <asp:Label ID="lb_owner_name" runat="server" /></div>
                            <div class="meta-actions">
                                <asp:HyperLink ID="lnk_public_scroll_cta" runat="server" CssClass="btn-primary" NavigateUrl="#shop-products" Text="Xem sản phẩm"></asp:HyperLink>
                            </div>
                        </div>
                    </div>

                    <asp:Panel ID="pn_public_stats" runat="server" Visible="false">
                        <div class="stats">
                            <article class="stat">
                                <div class="stat-label">Sản phẩm đang hiển thị</div>
                                <div class="stat-value"><asp:Label ID="lb_total_products" runat="server" /></div>
                            </article>
                            <article class="stat">
                                <div class="stat-label">Tổng lượt xem</div>
                                <div class="stat-value"><asp:Label ID="lb_total_views" runat="server" /></div>
                            </article>
                            <article class="stat">
                                <div class="stat-label">Đã bán</div>
                                <div class="stat-value"><asp:Label ID="lb_total_sold" runat="server" /></div>
                            </article>
                            <article class="stat">
                                <div class="stat-label">Đơn chờ trao đổi</div>
                                <div class="stat-value"><asp:Label ID="lb_pending_orders" runat="server" /></div>
                            </article>
                        </div>
                    </asp:Panel>
                </div>
            </section>

            <asp:PlaceHolder ID="ph_top_products" runat="server" Visible="false">
                <section class="featured">
                    <div class="featured-head">
                        <div>
                            <h3 class="featured-title"><asp:Label ID="lb_public_top_title" runat="server" Text="Top tin nổi bật"></asp:Label></h3>
                            <div class="featured-sub"><asp:Label ID="lb_public_top_sub" runat="server" Text="Ưu tiên theo bán chạy và lượt xem."></asp:Label></div>
                        </div>
                        <asp:HyperLink ID="lnk_public_top_cta" runat="server" CssClass="btn-primary" NavigateUrl="#shop-products" Text="Xem tất cả"></asp:HyperLink>
                    </div>
                    <div class="grid featured-grid">
                        <asp:Repeater ID="rp_top_products" runat="server">
                            <ItemTemplate>
                                <div class="grid-item">
                                    <article class="card">
                                        <a class="card-link" href="<%# ResolveProductUrl(Eval("id"), Eval("name_en")) %>">
                                            <img class="card-image" src="<%# ResolveProductImage(Eval("image")) %>" alt="<%# HttpUtility.HtmlEncode((Eval("name") ?? "").ToString()) %>" loading="lazy" decoding="async" />
                                            <div class="card-body">
                                                <h4 class="card-name"><%# HttpUtility.HtmlEncode((Eval("name") ?? "").ToString()) %></h4>
                                                <div class="card-badges">
                                                    <asp:PlaceHolder ID="phServiceBadgeTop" runat="server" Visible='<%# (Eval("IsService") != null && Convert.ToBoolean(Eval("IsService"))) %>'>
                                                        <span class="card-badge card-badge-service">Dịch vụ</span>
                                                    </asp:PlaceHolder>
                                                    <span class="card-badge card-badge-public">Nổi bật</span>
                                                </div>
                                                <div class="card-price"><%# FormatCurrency(Eval("giaban")) %> đ</div>
                                                <div class="card-meta-row">
                                                    <div class="card-meta">Đã bán: <%# Eval("SoldCount") %></div>
                                                    <div class="card-meta">Lượt xem: <%# Eval("LuotTruyCap") %></div>
                                                </div>
                                            </div>
                                        </a>
                                        <div class="card-actions">
                                            <a class="card-btn card-btn-trade" href="<%# BuildExchangeActionUrl(Eval("id"), Eval("IsService"), Eval("name_en")) %>"><%# Eval("ExchangeLabel") %></a>
                                            <asp:PlaceHolder ID="phCartTop" runat="server" Visible='<%# !(Eval("IsService") != null && Convert.ToBoolean(Eval("IsService"))) %>'>
                                                <a class="card-btn card-btn-cart" href="<%# BuildAddCartActionUrl(Eval("id")) %>">Thêm vào giỏ</a>
                                            </asp:PlaceHolder>
                                        </div>
                                    </article>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </section>
            </asp:PlaceHolder>

            <section class="products" id="shop-products">
                <div class="products-head">
                    <div>
                        <h3 class="products-title"><asp:Label ID="lb_public_products_title" runat="server" Text="Sản phẩm &amp; dịch vụ của gian hàng đối tác"></asp:Label></h3>
                        <div class="products-sub"><asp:Label ID="lb_public_products_sub" runat="server" Text="Danh sách này là public, lấy trực tiếp từ tài khoản gian hàng đối tác hiện tại."></asp:Label></div>
                    </div>
                    <div class="public-url">
                        URL: <asp:Label ID="lb_public_url" runat="server" />
                    </div>
                    <div class="filters">
                        <div class="filter-field">
                            <label for="shopSearch">Tìm kiếm</label>
                            <input id="shopSearch" class="filter-input" type="text" placeholder="Tìm theo tên sản phẩm" list="shopSuggest" />
                            <asp:Literal ID="lit_shop_suggest" runat="server"></asp:Literal>
                        </div>
                        <div class="filter-field">
                            <label for="shopCategory">Danh mục</label>
                            <select id="shopCategory" class="filter-select">
                                <asp:Literal ID="lit_category_options" runat="server"></asp:Literal>
                            </select>
                        </div>
                        <div class="filter-field">
                            <label for="shopArea">Khu vực</label>
                            <select id="shopArea" class="filter-select">
                                <asp:Literal ID="lit_area_options" runat="server"></asp:Literal>
                            </select>
                        </div>
                        <div class="filter-field">
                            <label for="shopMinPrice">Giá từ</label>
                            <input id="shopMinPrice" class="filter-input" type="number" min="0" placeholder="0" />
                        </div>
                        <div class="filter-field">
                            <label for="shopMaxPrice">Giá đến</label>
                            <input id="shopMaxPrice" class="filter-input" type="number" min="0" placeholder="Không giới hạn" />
                        </div>
                        <div class="filter-field">
                            <label for="shopSort">Sắp xếp</label>
                            <select id="shopSort" class="filter-select">
                                <option value="new">Mới nhất</option>
                                <option value="price_asc">Giá thấp → cao</option>
                                <option value="price_desc">Giá cao → thấp</option>
                                <option value="sold_desc">Bán chạy</option>
                            </select>
                        </div>
                        <div class="filter-actions">
                            <button type="button" class="filter-btn" id="btnClearFilters">Xóa lọc</button>
                            <div class="filter-count" id="shopFilterCount"></div>
                        </div>
                    </div>
                </div>

                <asp:PlaceHolder ID="ph_empty_products" runat="server" Visible="false">
                    <div class="empty">Gian hàng chưa có sản phẩm hoặc dịch vụ công khai.</div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_products" runat="server" Visible="true">
                    <div class="grid" id="shopProductGrid">
                        <asp:Repeater ID="rp_products" runat="server">
                            <ItemTemplate>
                                <div class="grid-item"
                                    data-name="<%# HttpUtility.HtmlAttributeEncode((Eval("name") ?? "").ToString()) %>"
                                    data-price="<%# Eval("giaban") %>"
                                    data-category="<%# Eval("CategoryId") %>"
                                    data-area="<%# Eval("AreaRaw") %>"
                                    data-date="<%# Eval("DateTicks") %>"
                                    data-sold="<%# Eval("SoldCount") %>"
                                    data-index="<%# Container.ItemIndex %>">
                                    <article class="card">
                                        <a class="card-link" href="<%# ResolveProductUrl(Eval("id"), Eval("name_en")) %>">
                                            <img class="card-image" src="<%# ResolveProductImage(Eval("image")) %>" alt="<%# HttpUtility.HtmlEncode((Eval("name") ?? "").ToString()) %>" loading="lazy" decoding="async" />
                                            <div class="card-body">
                                                <h4 class="card-name"><%# HttpUtility.HtmlEncode((Eval("name") ?? "").ToString()) %></h4>
                                                <div class="card-badges">
                                                    <asp:PlaceHolder ID="phServiceBadge" runat="server" Visible='<%# (Eval("IsService") != null && Convert.ToBoolean(Eval("IsService"))) %>'>
                                                        <span class="card-badge card-badge-service">Dịch vụ</span>
                                                    </asp:PlaceHolder>
                                                    <span class="card-badge card-badge-public">Công khai</span>
                                                    <asp:PlaceHolder ID="phCategoryBadge" runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Eval("CategoryName") as string) %>'>
                                                        <span class="card-badge card-badge-category"><%# Eval("CategoryName") %></span>
                                                    </asp:PlaceHolder>
                                                </div>
                                                <div class="card-price"><%# FormatCurrency(Eval("giaban")) %> đ</div>
                                                <div class="card-meta-row">
                                                    <div class="card-meta">Ngày đăng: <%# FormatDate(Eval("ngaytao")) %></div>
                                                    <div class="card-meta">Lượt xem: <%# Eval("LuotTruyCap") %></div>
                                                </div>
                                                <div class="card-meta-row">
                                                    <div class="card-meta">Khu vực: <%# Eval("AreaLabel") %></div>
                                                    <div class="card-meta">Đã bán: <%# Eval("SoldCount") %></div>
                                                </div>
                                            </div>
                                        </a>
                                        <div class="card-actions">
                                            <a class="card-btn card-btn-trade" href="<%# BuildExchangeActionUrl(Eval("id"), Eval("IsService"), Eval("name_en")) %>"><%# Eval("ExchangeLabel") %></a>
                                            <asp:PlaceHolder ID="phCart" runat="server" Visible='<%# !(Eval("IsService") != null && Convert.ToBoolean(Eval("IsService"))) %>'>
                                                <a class="card-btn card-btn-cart" href="<%# BuildAddCartActionUrl(Eval("id")) %>">Thêm vào giỏ</a>
                                            </asp:PlaceHolder>
                                        </div>
                                    </article>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:PlaceHolder>
            </section>
        </div>
    </form>

    <script type="text/javascript">
        (function () {
            var searchInput = document.getElementById("shopSearch");
            var categorySelect = document.getElementById("shopCategory");
            var areaSelect = document.getElementById("shopArea");
            var minPriceInput = document.getElementById("shopMinPrice");
            var maxPriceInput = document.getElementById("shopMaxPrice");
            var sortSelect = document.getElementById("shopSort");
            var clearBtn = document.getElementById("btnClearFilters");
            var grid = document.getElementById("shopProductGrid");
            var countEl = document.getElementById("shopFilterCount");

            if (!grid) return;

            function parseNumber(value) {
                var num = parseFloat(value);
                return isNaN(num) ? 0 : num;
            }

            function getFilterValue(el) {
                return el ? (el.value || "").trim().toLowerCase() : "";
            }

            function applyFilters() {
                var keyword = getFilterValue(searchInput);
                var category = getFilterValue(categorySelect);
                var area = getFilterValue(areaSelect);
                var minPrice = parseNumber(minPriceInput && minPriceInput.value ? minPriceInput.value : "");
                var maxPrice = parseNumber(maxPriceInput && maxPriceInput.value ? maxPriceInput.value : "");
                var sortMode = getFilterValue(sortSelect);

                var items = Array.prototype.slice.call(grid.querySelectorAll(".grid-item"));
                var visible = [];

                items.forEach(function (item) {
                    var name = (item.getAttribute("data-name") || "").toLowerCase();
                    var price = parseNumber(item.getAttribute("data-price"));
                    var cat = (item.getAttribute("data-category") || "").toLowerCase();
                    var areaValue = (item.getAttribute("data-area") || "").toLowerCase();

                    var ok = true;
                    if (keyword && name.indexOf(keyword) < 0) ok = false;
                    if (category && cat !== category) ok = false;
                    if (area && areaValue !== area) ok = false;
                    if (minPrice > 0 && price < minPrice) ok = false;
                    if (maxPrice > 0 && price > maxPrice) ok = false;

                    item.style.display = ok ? "" : "none";
                    if (ok) visible.push(item);
                });

                if (sortMode) {
                    visible.sort(function (a, b) {
                        if (sortMode === "price_asc") {
                            return parseNumber(a.getAttribute("data-price")) - parseNumber(b.getAttribute("data-price"));
                        }
                        if (sortMode === "price_desc") {
                            return parseNumber(b.getAttribute("data-price")) - parseNumber(a.getAttribute("data-price"));
                        }
                        if (sortMode === "sold_desc") {
                            return parseNumber(b.getAttribute("data-sold")) - parseNumber(a.getAttribute("data-sold"));
                        }
                        if (sortMode === "new") {
                            return parseNumber(b.getAttribute("data-date")) - parseNumber(a.getAttribute("data-date"));
                        }
                        return parseNumber(a.getAttribute("data-index")) - parseNumber(b.getAttribute("data-index"));
                    });
                    visible.forEach(function (item) { grid.appendChild(item); });
                }

                if (countEl) {
                    countEl.textContent = "Hiển thị " + visible.length + " / " + items.length;
                }
            }

            if (searchInput) searchInput.addEventListener("input", applyFilters);
            if (categorySelect) categorySelect.addEventListener("change", applyFilters);
            if (areaSelect) areaSelect.addEventListener("change", applyFilters);
            if (minPriceInput) minPriceInput.addEventListener("input", applyFilters);
            if (maxPriceInput) maxPriceInput.addEventListener("input", applyFilters);
            if (sortSelect) sortSelect.addEventListener("change", applyFilters);

            if (clearBtn) {
                clearBtn.addEventListener("click", function () {
                    if (searchInput) searchInput.value = "";
                    if (categorySelect) categorySelect.value = "";
                    if (areaSelect) areaSelect.value = "";
                    if (minPriceInput) minPriceInput.value = "";
                    if (maxPriceInput) maxPriceInput.value = "";
                    if (sortSelect) sortSelect.value = "new";
                    applyFilters();
                });
            }

            applyFilters();
        })();
    </script>
</body>
</html>
