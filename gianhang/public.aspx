<%@ Page Language="C#" AutoEventWireup="true" CodeFile="public.aspx.cs" Inherits="gianhang_public" %>
<%@ Import Namespace="System.Web" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trang công khai</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root {
            --gh-bg: #fff7ed;
            --gh-card: #ffffff;
            --gh-line: #fed7aa;
            --gh-text: #17233b;
            --gh-muted: #64748b;
            --gh-brand: #f97316;
            --gh-brand-2: #fb923c;
            --gh-soft: #ffedd5;
        }

        * { box-sizing: border-box; }
        body {
            margin: 0;
            font-family: "Nunito Sans", "Segoe UI", Arial, sans-serif;
            color: var(--gh-text);
            background:
                radial-gradient(920px 320px at 18% -4%, rgba(249, 115, 22, .14), transparent 62%),
                radial-gradient(920px 320px at 88% -6%, rgba(251, 146, 60, .18), transparent 62%),
                var(--gh-bg);
        }
        a { text-decoration: none; color: inherit; }
        .shell { width: min(1220px, calc(100% - 28px)); margin: 18px auto 28px; }
        .hero {
            position: relative;
            border: 1px solid var(--gh-line);
            border-radius: 22px;
            overflow: hidden;
            background: var(--gh-card);
            box-shadow: 0 18px 44px rgba(15, 23, 42, .10);
        }
        .hero-cover {
            position: absolute;
            inset: 0 0 auto 0;
            height: 170px;
            background-size: cover;
            background-position: center;
            opacity: .18;
        }
        .hero.has-cover .hero-cover { opacity: .36; }
        .hero.has-cover { padding-top: 170px; }
        .hero-inner { position: relative; z-index: 1; }
        .hero-top {
            background: linear-gradient(130deg, var(--gh-brand), var(--gh-brand-2));
            color: #fff;
            padding: 16px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }
        .hero.has-cover .hero-top {
            background: linear-gradient(130deg, rgba(249,115,22,.82), rgba(251,146,60,.82));
        }
        .hero-title { margin: 0; font-size: 24px; line-height: 1.2; font-weight: 900; }
        .hero-sub { margin-top: 4px; font-size: 13px; color: rgba(255,255,255,.9); }
        .btn-login, .btn-soft, .btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            border-radius: 12px;
            padding: 0 14px;
            font-weight: 800;
            border: 1px solid transparent;
        }
        .btn-login { background: #fff; color: var(--gh-brand); border-color: rgba(255,255,255,.65); }
        .btn-primary { background: var(--gh-brand); color: #fff; border-color: rgba(249,115,22,.42); }
        .btn-soft { background: var(--gh-soft); color: #9a3412; border-color: rgba(249,115,22,.18); }
        .hero-main {
            padding: 18px;
            display: grid;
            grid-template-columns: 98px 1fr;
            gap: 16px;
            align-items: center;
            border-bottom: 1px solid #f3e8d8;
            background: rgba(255,255,255,.92);
        }
        .store-avatar {
            width: 98px; height: 98px; border-radius: 50%;
            object-fit: cover; border: 3px solid #fdedd8; background: #fff7ed;
        }
        .meta-title { margin: 0; font-size: 30px; line-height: 1.12; font-weight: 900; }
        .meta-desc { margin-top: 8px; font-size: 14px; color: rgba(23,35,59,.84); font-weight: 600; }
        .meta-line { margin-top: 6px; color: var(--gh-muted); font-size: 14px; }
        .meta-actions { margin-top: 12px; display: flex; flex-wrap: wrap; gap: 8px; }
        .stats {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            padding: 16px 18px 18px;
        }
        .stat {
            border: 1px solid #fde3c5;
            border-radius: 14px;
            background: #fffaf5;
            padding: 12px 14px;
            min-height: 82px;
        }
        .stat-label { font-size: 13px; color: var(--gh-muted); }
        .stat-value { margin-top: 4px; font-size: 30px; line-height: 1.1; font-weight: 900; }
        .section {
            margin-top: 16px;
            border: 1px solid var(--gh-line);
            border-radius: 22px;
            background: var(--gh-card);
            box-shadow: 0 16px 32px rgba(15, 23, 42, .08);
            overflow: hidden;
        }
        .section-head {
            padding: 16px 18px;
            border-bottom: 1px solid #f7e8d8;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }
        .section-title { margin: 0; font-size: 24px; line-height: 1.2; font-weight: 900; }
        .section-sub { color: var(--gh-muted); font-size: 13px; margin-top: 4px; }
        .public-url {
            color: var(--gh-muted);
            font-size: 12px;
            font-weight: 700;
            background: #fffaf5;
            border: 1px solid var(--gh-line);
            border-radius: 999px;
            padding: 7px 12px;
        }
        .filters {
            width: 100%;
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 10px;
            margin-top: 14px;
        }
        .filter-field { display: flex; flex-direction: column; gap: 6px; }
        .filter-field label {
            font-size: 11px; font-weight: 700; color: var(--gh-muted);
            text-transform: uppercase; letter-spacing: .04em;
        }
        .filter-input, .filter-select {
            height: 38px;
            border-radius: 10px;
            border: 1px solid var(--gh-line);
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
            border: 1px solid var(--gh-line);
            background: #fffaf5;
            font-weight: 700;
            cursor: pointer;
            padding: 0 12px;
        }
        .filter-count { font-size: 12px; color: var(--gh-muted); margin-top: 6px; }
        .grid {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            padding: 14px;
        }
        .grid-item { width: 100%; }
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
            .grid-item { width: calc((100% - 12px*4) / 5); }
        }
        .card {
            border: none;
            border-radius: 14px;
            overflow: hidden;
            background: #fff;
            transition: transform .18s ease, box-shadow .18s ease;
            display: flex;
            flex-direction: column;
            min-height: 100%;
            box-shadow: 0 4px 16px rgba(15,23,42,.08);
        }
        .card:hover { transform: translateY(-2px); box-shadow: 0 16px 28px rgba(15,23,42,.14); }
        .card-link { display: block; color: inherit; }
        .card-image {
            width: 100%;
            aspect-ratio: 1 / 1;
            object-fit: cover;
            background: #fff7ed;
            display: block;
        }
        .card-body {
            padding: 10px 10px 12px;
            display: flex;
            flex-direction: column;
            min-height: 110px;
        }
        .card-name {
            margin: 0;
            font-size: 15px;
            line-height: 1.2rem;
            font-weight: 800;
            min-height: 2.4rem;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }
        .card-meta { color: var(--gh-muted); font-size: 12px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        .card-meta-row {
            margin-top: 8px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
        }
        .card-badges { margin-top: 8px; display: flex; align-items: center; gap: 6px; flex-wrap: wrap; }
        .card-badge {
            display: inline-flex;
            align-items: center;
            min-height: 22px;
            border-radius: 999px;
            padding: 0 8px;
            font-size: 11px;
            font-weight: 800;
        }
        .card-badge-service { color: #9a3412; background: #ffedd5; border: 1px solid #fdba74; }
        .card-badge-product { color: #c2410c; background: #fff7ed; border: 1px solid #fed7aa; }
        .card-badge-category { color: #9a3412; background: #fff1ea; border: 1px solid #ffd7c2; }
        .card-price { margin-top: 8px; color: #dc2626; font-size: 16px; font-weight: 800; line-height: 1.2; }
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
        .card-btn-trade { background: var(--gh-brand); border-color: var(--gh-brand); color: #fff; }
        .card-btn-cart { background: #fff7ed; border-color: #fdba74; color: #c2410c; }
        .empty-state {
            padding: 24px 18px 28px;
            text-align: center;
            color: var(--gh-muted);
        }
        .empty-state h3 { margin: 0 0 8px; color: var(--gh-text); font-size: 22px; }
        .pending-review {
            border: 1px solid var(--gh-line);
            border-radius: 22px;
            background: var(--gh-card);
            box-shadow: 0 18px 44px rgba(15, 23, 42, .10);
            overflow: hidden;
        }
        .pending-review-top {
            background: linear-gradient(130deg, var(--gh-brand), var(--gh-brand-2));
            color: #fff;
            padding: 18px 20px;
        }
        .pending-review-title {
            margin: 0;
            font-size: 28px;
            line-height: 1.15;
            font-weight: 900;
        }
        .pending-review-sub {
            margin-top: 6px;
            font-size: 14px;
            color: rgba(255,255,255,.92);
        }
        .pending-review-body {
            padding: 22px 20px 24px;
            background: rgba(255,255,255,.95);
        }
        .pending-review-box {
            border: 1px solid #fde3c5;
            border-radius: 16px;
            background: #fffaf5;
            padding: 18px;
            color: var(--gh-text);
        }
        .pending-review-box h2 {
            margin: 0 0 10px;
            font-size: 22px;
            line-height: 1.2;
            font-weight: 900;
        }
        .pending-review-box p {
            margin: 0;
            color: var(--gh-muted);
            font-size: 15px;
            line-height: 1.6;
        }
        .pending-review-meta {
            margin-top: 16px;
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }
        .pending-review-pill {
            display: inline-flex;
            align-items: center;
            min-height: 36px;
            border-radius: 999px;
            padding: 0 14px;
            background: var(--gh-soft);
            border: 1px solid rgba(249,115,22,.18);
            color: #9a3412;
            font-size: 13px;
            font-weight: 800;
        }
        .pending-review-actions {
            margin-top: 18px;
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }
        @media (max-width: 991.98px) {
            .stats { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .filters { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        }
        @media (max-width: 767.98px) {
            .shell { width: min(100% - 18px, 100%); margin: 12px auto 20px; }
            .hero-main { grid-template-columns: 72px 1fr; padding: 14px; }
            .store-avatar { width: 72px; height: 72px; }
            .meta-title { font-size: 24px; }
            .section-title { font-size: 22px; }
            .filters { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shell">
            <asp:PlaceHolder ID="ph_pending_review" runat="server" Visible="false">
                <section class="pending-review">
                    <div class="pending-review-top">
                        <h1 class="pending-review-title">Gian hàng đang xét duyệt</h1>
                        <div class="pending-review-sub">Gian hàng này đang chờ xét duyệt.</div>
                    </div>
                    <div class="pending-review-body">
                        <div class="pending-review-box">
                            <h2>Gian hàng này đang trong quá trình xét duyệt bởi AHA SALE</h2>
                            <p>Ngay khi gian hàng được phê duyệt, trang công khai sẽ tự động hiển thị tại đây.</p>
                            <div class="pending-review-meta">
                                <span class="pending-review-pill">Tài khoản Home: <asp:Label ID="lb_pending_store_account" runat="server" /></span>
                                <span class="pending-review-pill">Chủ gian hàng: <asp:Label ID="lb_pending_store_name" runat="server" /></span>
                            </div>
                            <div class="pending-review-actions">
                                <asp:HyperLink ID="lnk_pending_home" runat="server" CssClass="btn-login">Về trang Home</asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </section>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_public_active" runat="server">
            <div id="hero_section" runat="server" class="hero">
                <asp:PlaceHolder ID="pn_cover" runat="server" Visible="false">
                    <div class="hero-cover" id="hero_cover" runat="server"></div>
                </asp:PlaceHolder>
                <div class="hero-inner">
                    <div class="hero-top">
                        <div>
                            <h1 class="hero-title"><asp:Label ID="lb_public_hero_title" runat="server" /></h1>
                            <div class="hero-sub"><asp:Label ID="lb_public_hero_sub" runat="server" /></div>
                        </div>
                        <div style="display:flex; gap:8px; flex-wrap:wrap;">
                            <asp:HyperLink ID="lnk_home_login" runat="server" CssClass="btn-login" />
                            <asp:HyperLink ID="lnk_public_primary_cta" runat="server" CssClass="btn-login" Visible="false" />
                            <asp:PlaceHolder ID="ph_owner_actions" runat="server" Visible="false">
                                <asp:HyperLink ID="lnk_owner_dashboard" runat="server" CssClass="btn-soft">Vào gian hàng</asp:HyperLink>
                                <asp:HyperLink ID="lnk_owner_manage" runat="server" CssClass="btn-soft">Quản lý tin</asp:HyperLink>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="hero-main">
                        <asp:Image ID="img_avatar" runat="server" CssClass="store-avatar" />
                        <div>
                            <h2 class="meta-title"><asp:Label ID="lb_store_name" runat="server" /></h2>
                            <div class="meta-line">Tài khoản gốc: <asp:Label ID="lb_store_account" runat="server" /></div>
                            <div class="meta-line">Chủ không gian: <asp:Label ID="lb_owner_name" runat="server" /></div>
                            <asp:PlaceHolder ID="ph_store_desc" runat="server" Visible="false">
                                <div class="meta-desc"><asp:Label ID="lb_store_desc" runat="server" /></div>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="stats">
                        <div class="stat">
                            <div class="stat-label">Sản phẩm</div>
                            <div class="stat-value"><asp:Label ID="lb_total_products" runat="server" Text="0" /></div>
                        </div>
                        <div class="stat">
                            <div class="stat-label">Dịch vụ</div>
                            <div class="stat-value"><asp:Label ID="lb_total_services" runat="server" Text="0" /></div>
                        </div>
                        <div class="stat">
                            <div class="stat-label">Lượt xem</div>
                            <div class="stat-value"><asp:Label ID="lb_total_views" runat="server" Text="0" /></div>
                        </div>
                        <div class="stat">
                            <div class="stat-label">Đã bán</div>
                            <div class="stat-value"><asp:Label ID="lb_total_sold" runat="server" Text="0" /></div>
                        </div>
                    </div>
                </div>
            </div>

            <section class="section">
                <div class="section-head">
                    <div>
                        <h2 class="section-title"><asp:Label ID="lb_public_top_title" runat="server" /></h2>
                        <div class="section-sub"><asp:Label ID="lb_public_top_sub" runat="server" /></div>
                    </div>
                    <asp:HyperLink ID="lnk_public_top_cta" runat="server" CssClass="btn-soft" />
                </div>
                <asp:PlaceHolder ID="ph_top_products" runat="server" Visible="false">
                    <div class="grid featured-grid">
                        <asp:Repeater ID="rp_top_products" runat="server">
                            <ItemTemplate>
                                <div class="grid-item">
                                    <article class="card">
                                        <a class="card-link" href="<%# BuildDetailUrl(Container.DataItem) %>">
                                            <img class="card-image" src="<%# Eval("ImageUrl") %>" alt="<%# EncodeAttr(Eval("Name")) %>" />
                                        </a>
                                        <div class="card-body">
                                            <a class="card-link" href="<%# BuildDetailUrl(Container.DataItem) %>">
                                                <h3 class="card-name"><%# Eval("Name") %></h3>
                                            </a>
                                            <div class="card-price"><%# FormatCurrency(Eval("Price")) %> đ</div>
                                            <div class="card-badges">
                                                <span class="<%# BuildTypeCss(Eval("ProductType")) %>"><%# BuildTypeLabel(Eval("ProductType")) %></span>
                                                <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("CategoryName"))) %>'>
                                                    <span class="card-badge card-badge-category"><%# Eval("CategoryName") %></span>
                                                </asp:PlaceHolder>
                                            </div>
                                            <div class="card-meta-row">
                                                <span class="card-meta">Đã bán: <%# Eval("SoldCount") %></span>
                                                <span class="card-meta">Lượt xem: <%# Eval("Views") %></span>
                                            </div>
                                        </div>
                                        <div class="card-actions">
                                            <a class="card-btn card-btn-trade" href="<%# BuildExchangeActionUrl(Container.DataItem) %>"><%# Eval("ActionLabel") %></a>
                                            <asp:PlaceHolder runat="server" Visible='<%# ShowAddCart(Container.DataItem) %>'>
                                                <a class="card-btn card-btn-cart" href="<%# BuildAddCartActionUrl(Container.DataItem) %>">Thêm vào giỏ</a>
                                            </asp:PlaceHolder>
                                        </div>
                                    </article>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_empty_top_products" runat="server" Visible="false">
                    <div class="empty-state">
                        <h3>Chưa có top tin nổi bật</h3>
                        <p>Khi gian hàng bắt đầu có lượt xem và bán hàng, khối này sẽ tự cập nhật.</p>
                    </div>
                </asp:PlaceHolder>
            </section>

            <section class="section" id="gianhang-products">
                <div class="section-head">
                    <div>
                        <h2 class="section-title"><asp:Label ID="lb_public_products_title" runat="server" /></h2>
                        <div class="section-sub"><asp:Label ID="lb_public_products_sub" runat="server" /></div>
                    </div>
                    <span class="public-url">Đang hiển thị <asp:Literal ID="lit_product_count" runat="server" /></span>
                </div>
                <div style="padding:0 18px 18px;">
                    <div class="filters">
                        <div class="filter-field">
                            <label for="ghSearch">Tìm kiếm</label>
                            <input id="ghSearch" class="filter-input" type="text" placeholder="Tên sản phẩm hoặc dịch vụ..." />
                        </div>
                        <div class="filter-field">
                            <label for="ghType">Loại tin</label>
                            <select id="ghType" class="filter-select">
                                <option value="">Tất cả loại tin</option>
                                <option value="product">Chỉ sản phẩm</option>
                                <option value="service">Chỉ dịch vụ</option>
                            </select>
                        </div>
                        <div class="filter-field">
                            <label for="ghCategory">Danh mục</label>
                            <select id="ghCategory" class="filter-select">
                                <asp:Literal ID="lit_category_options" runat="server" />
                            </select>
                        </div>
                        <div class="filter-actions">
                            <button type="button" class="filter-btn" id="ghReset">Làm mới</button>
                        </div>
                    </div>
                    <div class="filter-count" id="ghCountLabel">Hiển thị danh sách tin đang hoạt động.</div>
                </div>

                <asp:PlaceHolder ID="ph_products" runat="server" Visible="false">
                    <div class="grid" id="ghProductGrid">
                        <asp:Repeater ID="rp_products" runat="server">
                            <ItemTemplate>
                                <div class="grid-item"
                                     data-filter-item="1"
                                     data-name="<%# EncodeAttr(Eval("Name")) %>"
                                     data-type="<%# BuildTypeFilterValue(Eval("ProductType")) %>"
                                     data-category="<%# EncodeAttr(Eval("CategoryId")) %>">
                                    <article class="card">
                                        <a class="card-link" href="<%# BuildDetailUrl(Container.DataItem) %>">
                                            <img class="card-image" src="<%# Eval("ImageUrl") %>" alt="<%# EncodeAttr(Eval("Name")) %>" />
                                        </a>
                                        <div class="card-body">
                                            <a class="card-link" href="<%# BuildDetailUrl(Container.DataItem) %>">
                                                <h3 class="card-name"><%# Eval("Name") %></h3>
                                            </a>
                                            <div class="card-meta"><%# Eval("Description") %></div>
                                            <div class="card-price"><%# FormatCurrency(Eval("Price")) %> đ</div>
                                            <div class="card-badges">
                                                <span class="<%# BuildTypeCss(Eval("ProductType")) %>"><%# BuildTypeLabel(Eval("ProductType")) %></span>
                                                <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("CategoryName"))) %>'>
                                                    <span class="card-badge card-badge-category"><%# Eval("CategoryName") %></span>
                                                </asp:PlaceHolder>
                                            </div>
                                            <div class="card-meta-row">
                                                <span class="card-meta"><%# FormatDate(Eval("CreatedAt")) %></span>
                                                <span class="card-meta">Lượt xem: <%# Eval("Views") %></span>
                                            </div>
                                        </div>
                                        <div class="card-actions">
                                            <a class="card-btn card-btn-trade" href="<%# BuildExchangeActionUrl(Container.DataItem) %>"><%# Eval("ActionLabel") %></a>
                                            <asp:PlaceHolder runat="server" Visible='<%# ShowAddCart(Container.DataItem) %>'>
                                                <a class="card-btn card-btn-cart" href="<%# BuildAddCartActionUrl(Container.DataItem) %>">Thêm vào giỏ</a>
                                            </asp:PlaceHolder>
                                        </div>
                                    </article>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_empty_products" runat="server" Visible="false">
                    <div class="empty-state">
                        <h3>Chưa có sản phẩm hoặc dịch vụ công khai</h3>
                        <p>Hãy thêm tin đầu tiên để bắt đầu vận hành gian hàng.</p>
                    </div>
                </asp:PlaceHolder>
            </section>
            </asp:PlaceHolder>
        </div>
    </form>

    <script>
        (function () {
            var search = document.getElementById('ghSearch');
            var type = document.getElementById('ghType');
            var category = document.getElementById('ghCategory');
            var reset = document.getElementById('ghReset');
            var countLabel = document.getElementById('ghCountLabel');
            var items = Array.prototype.slice.call(document.querySelectorAll('[data-filter-item="1"]'));

            function normalize(value) {
                return (value || '').toString().trim().toLowerCase();
            }

            function applyFilter() {
                var q = normalize(search && search.value);
                var typeValue = normalize(type && type.value);
                var categoryValue = normalize(category && category.value);
                var visible = 0;

                items.forEach(function (item) {
                    var itemName = normalize(item.getAttribute('data-name'));
                    var itemType = normalize(item.getAttribute('data-type'));
                    var itemCategory = normalize(item.getAttribute('data-category'));
                    var match = true;

                    if (q && itemName.indexOf(q) === -1) match = false;
                    if (match && typeValue && itemType !== typeValue) match = false;
                    if (match && categoryValue && itemCategory !== categoryValue) match = false;

                    item.style.display = match ? '' : 'none';
                    if (match) visible++;
                });

                if (countLabel) {
                    countLabel.textContent = 'Hiển thị ' + visible + ' tin phù hợp.';
                }
            }

            if (search) search.addEventListener('input', applyFilter);
            if (type) type.addEventListener('change', applyFilter);
            if (category) category.addEventListener('change', applyFilter);
            if (reset) {
                reset.addEventListener('click', function () {
                    if (search) search.value = '';
                    if (type) type.value = '';
                    if (category) category.value = '';
                    applyFilter();
                });
            }

            applyFilter();
        })();
    </script>
</body>
</html>
