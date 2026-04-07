<%@ Page Title="Trang cÃ´ng khai gian hÃ ng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerGianHang.master" AutoEventWireup="true" CodeFile="public.aspx.cs" Inherits="gianhang_public" %>
<%@ Import Namespace="System.Web" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <link href="<%= Helper_cl.VersionedUrl("~/Metro-UI-CSS-master/tests/metro/css/metro-all.min.css") %>" rel="stylesheet" />
    <link href="<%= Helper_cl.VersionedUrl("~/css/fix-metro.css") %>" rel="stylesheet" />
    <link href="<%= Helper_cl.VersionedUrl("~/css/bcorn-with-metro.css") %>" rel="stylesheet" />
    <link href="<%= Helper_cl.VersionedUrl("~/css/gianhang-home.css") %>" rel="stylesheet" />
    <link href="<%= Helper_cl.VersionedUrl("~/css/gianhang-storefront.css") %>" rel="stylesheet" />
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

        body {
            background:
                radial-gradient(920px 320px at 18% -4%, rgba(249, 115, 22, .14), transparent 62%),
                radial-gradient(920px 320px at 88% -6%, rgba(251, 146, 60, .18), transparent 62%),
                var(--gh-bg);
        }

        .gh-public-shell {
            width: min(1220px, calc(100% - 28px));
            margin: 18px auto 28px;
        }

        .gh-public-shell a {
            text-decoration: none;
            color: inherit;
        }

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
            inset: 0;
            background-size: cover;
            background-position: center;
            opacity: .08;
        }

        .hero.has-cover .hero-cover {
            opacity: .14;
        }

        .hero-inner {
            position: relative;
            z-index: 1;
        }

        .hero-shell {
            padding: 18px 18px 0;
        }

        .btn-login,
        .btn-soft,
        .btn-primary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            border-radius: 12px;
            padding: 0 14px;
            font-weight: 800;
            border: 1px solid transparent;
        }

        .btn-login,
        .btn-login:visited {
            background: #fff;
            color: var(--gh-brand) !important;
            border-color: rgba(255,255,255,.65);
        }

        .btn-primary,
        .btn-primary:visited {
            background: var(--gh-brand);
            color: #fff !important;
            border-color: rgba(249,115,22,.42);
        }

        .btn-soft,
        .btn-soft:visited {
            background: var(--gh-soft);
            color: #9a3412 !important;
            border-color: rgba(249,115,22,.18);
        }

        .btn-login:hover {
            color: #c2410c !important;
            background: #fff7ed;
        }

        .btn-soft:hover {
            color: #7c2d12 !important;
            background: #ffedd5;
            border-color: #fdba74;
        }

        .btn-primary:hover {
            color: #fff !important;
            background: #ea580c;
        }

        .hero-showcase {
            position: relative;
            min-height: 320px;
            border-radius: 20px;
            overflow: hidden;
            border: 1px solid rgba(249,115,22,.18);
            background:
                linear-gradient(180deg, rgba(255,255,255,.16), rgba(255,255,255,0)),
                url('<%= Helper_cl.VersionedUrl("~/uploads/images/gianhang-banner-home-style.png") %>') center center / cover no-repeat;
            box-shadow: 0 20px 42px rgba(15, 23, 42, .12);
        }

        .trust-strip {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            padding: 0 18px 18px;
            background: rgba(255,255,255,.92);
        }

        .trust-pill {
            display: inline-flex;
            align-items: center;
            min-height: 36px;
            padding: 0 14px;
            border-radius: 999px;
            background: #fff7ed;
            border: 1px solid #fed7aa;
            color: #9a3412;
            font-size: 13px;
            font-weight: 800;
        }

        .hero-main {
            margin: 14px 18px 18px;
            padding: 20px;
            display: grid;
            grid-template-columns: 98px 1fr;
            gap: 16px;
            align-items: center;
            background: linear-gradient(130deg, var(--gh-brand), var(--gh-brand-2));
            color: #fff;
            border-radius: 20px;
            box-shadow: 0 18px 38px rgba(154, 52, 18, .18);
        }

        .store-avatar {
            width: 98px;
            height: 98px;
            border-radius: 50%;
            object-fit: cover;
            border: 3px solid rgba(255,255,255,.42);
            background: #fff7ed;
        }

        .meta-kicker {
            display: inline-flex;
            align-items: center;
            min-height: 32px;
            padding: 0 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.28);
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .03em;
            text-transform: uppercase;
        }

        .meta-title {
            margin: 12px 0 0;
            font-size: 30px;
            line-height: 1.12;
            font-weight: 900;
            color: #fff;
        }

        .meta-desc {
            margin-top: 8px;
            font-size: 15px;
            color: rgba(255,255,255,.94);
            font-weight: 700;
            line-height: 1.6;
        }

        .meta-line {
            margin-top: 8px;
            color: rgba(255,255,255,.84);
            font-size: 14px;
            line-height: 1.55;
        }

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

        .section-title {
            margin: 0;
            font-size: 24px;
            line-height: 1.2;
            font-weight: 900;
        }

        .section-sub {
            color: var(--gh-muted);
            font-size: 13px;
            margin-top: 4px;
        }

        .public-url {
            color: var(--gh-muted);
            font-size: 12px;
            font-weight: 700;
            background: #fffaf5;
            border: 1px solid var(--gh-line);
            border-radius: 999px;
            padding: 7px 12px;
        }

        .product-toolbar {
            padding: 14px 18px 16px;
            border-bottom: 1px solid #f7e8d8;
            background: linear-gradient(180deg, #fffdfb, #fff8f2);
            position: sticky;
            top: 10px;
            z-index: 5;
        }

        .filters {
            width: 100%;
            display: grid;
            grid-template-columns: minmax(220px, 1.4fr) minmax(160px, .7fr) minmax(180px, .8fr) auto;
            gap: 10px;
        }

        .filter-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .filter-field label {
            font-size: 11px;
            font-weight: 700;
            color: var(--gh-muted);
            text-transform: uppercase;
            letter-spacing: .04em;
        }

        .filter-input,
        .filter-select {
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
            height: 40px;
            border-radius: 999px;
            border: 1px solid #fdba74;
            background: #fff7ed;
            color: #9a3412;
            font-weight: 800;
            cursor: pointer;
            padding: 0 14px;
        }

        .filter-count {
            font-size: 12px;
            color: var(--gh-muted);
            margin-top: 10px;
        }

        .filter-summary {
            margin-top: 10px;
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .filter-summary__chip {
            display: inline-flex;
            align-items: center;
            min-height: 32px;
            padding: 0 12px;
            border-radius: 999px;
            background: #fff;
            border: 1px solid #fde3c5;
            color: #9a3412;
            font-size: 12px;
            font-weight: 700;
        }

        .product-grid-wrap {
            padding: 14px;
        }

        .grid {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
        }

        .grid-item,
        .featured-grid .grid-item {
            width: calc((100% - 12px) / 2);
        }

        .featured-grid {
            padding: 18px;
            gap: 16px;
        }

        .featured-grid .grid-item {
            width: calc((100% - 16px) / 2);
        }

        @media (min-width: 768px) {
            .grid-item,
            .featured-grid .grid-item {
                flex: 1 1 220px;
                width: calc((100% - 12px * 2) / 3);
                max-width: calc((100% - 12px * 2) / 3);
            }

            .featured-grid .grid-item {
                flex: 1 1 260px;
                width: calc((100% - 16px) / 2);
                max-width: calc((100% - 16px) / 2);
            }
        }

        @media (min-width: 1140px) {
            .grid-item {
                flex: 1 1 220px;
                width: calc((100% - 12px * 3) / 4);
                max-width: calc((100% - 12px * 3) / 4);
            }
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

        .card:hover {
            transform: translateY(-2px);
            box-shadow: 0 16px 28px rgba(15,23,42,.14);
        }

        .card-link {
            display: block;
            color: inherit;
        }

        .card-image {
            width: 100%;
            aspect-ratio: 4 / 5;
            object-fit: cover;
            background: #fff7ed;
            display: block;
        }

        .card-body {
            padding: 12px 12px 10px;
            display: flex;
            flex-direction: column;
            min-height: 126px;
        }

        .card-name {
            margin: 0;
            font-size: 15px;
            line-height: 1.35;
            font-weight: 800;
            min-height: 40px;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .card-desc {
            margin-top: 8px;
            color: var(--gh-muted);
            font-size: 12px;
            line-height: 1.5;
            min-height: 36px;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
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

        .card-badge-service {
            color: #9a3412;
            background: #ffedd5;
            border: 1px solid #fdba74;
        }

        .card-badge-product {
            color: #c2410c;
            background: #fff7ed;
            border: 1px solid #fed7aa;
        }

        .card-badge-category {
            color: #9a3412;
            background: #fff1ea;
            border: 1px solid #ffd7c2;
        }

        .card-price {
            margin-top: 10px;
            color: #dc2626;
            font-size: 18px;
            font-weight: 900;
            line-height: 1.2;
        }

        .card-actions {
            margin-top: auto;
            padding: 0 12px 12px;
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 6px;
            justify-content: center;
            align-items: center;
            justify-items: center;
        }

        .card-btn {
            min-height: 38px;
            border-radius: 12px;
            border: 1px solid transparent;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 800;
            text-decoration: none;
            width: 100%;
            max-width: 156px;
        }

        .card-btn-trade,
        .card-btn-trade:visited {
            background: var(--gh-brand);
            border-color: var(--gh-brand);
            color: #fff;
        }

        .card-btn-cart,
        .card-btn-cart:visited {
            background: #fff7ed;
            border-color: #fdba74;
            color: #c2410c;
        }

        .card-btn-trade:hover {
            color: #fff;
            background: #ea580c;
        }

        .card-btn-cart:hover {
            color: #9a3412;
            background: #ffedd5;
        }

        .featured-card {
            border-radius: 20px;
            box-shadow: 0 14px 28px rgba(15,23,42,.10);
        }

        .featured-card .card-body {
            padding: 14px 14px 16px;
            min-height: 138px;
        }

        .featured-card .card-name {
            font-size: 17px;
            line-height: 1.35;
            min-height: 46px;
        }

        .featured-card .card-price {
            font-size: 20px;
            margin-top: 10px;
        }

        .featured-card .card-actions {
            grid-template-columns: repeat(2, minmax(0, 1fr));
        }

        .discovery-strip {
            margin-top: 16px;
            border: 1px solid var(--gh-line);
            border-radius: 22px;
            background: linear-gradient(180deg, #fffdfb, #fff8f2);
            box-shadow: 0 16px 32px rgba(15, 23, 42, .08);
            padding: 18px;
        }

        .discovery-strip__head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .discovery-strip__title {
            margin: 0;
            font-size: 22px;
            line-height: 1.2;
            font-weight: 900;
            color: var(--gh-text);
        }

        .discovery-strip__sub {
            margin-top: 4px;
            color: var(--gh-muted);
            font-size: 13px;
        }

        .discovery-strip__chips {
            margin-top: 14px;
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .discovery-chip {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            padding: 0 16px;
            border-radius: 999px;
            background: #fff;
            border: 1px solid #fde3c5;
            color: #9a3412 !important;
            font-size: 13px;
            font-weight: 800;
            text-decoration: none !important;
            cursor: pointer;
        }

        .discovery-chip:hover,
        .discovery-chip:focus {
            color: #7c2d12 !important;
            background: #fff7ed;
            border-color: #fdba74;
        }

        .empty-state {
            padding: 24px 18px 28px;
            text-align: center;
            color: var(--gh-muted);
        }

        .empty-state h3 {
            margin: 0 0 8px;
            color: var(--gh-text);
            font-size: 22px;
        }

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

        .pending-review-meta,
        .pending-review-trust,
        .pending-review-actions {
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

        @media (max-width: 767.98px) {
            .gh-public-shell {
                width: min(100% - 18px, 100%);
                margin: 12px auto 20px;
            }

            .trust-strip {
                padding: 0 14px 14px;
            }

            .hero-main {
                grid-template-columns: 72px 1fr;
                margin: 12px 14px 14px;
                padding: 16px;
            }

            .store-avatar {
                width: 72px;
                height: 72px;
            }

            .meta-title {
                font-size: 24px;
            }

            .section-title {
                font-size: 22px;
            }

            .discovery-strip__title {
                font-size: 20px;
            }

            .filters {
                grid-template-columns: 1fr;
            }

            .hero-showcase {
                min-height: 220px;
            }

            .featured-grid {
                padding: 14px;
            }

            .featured-grid .grid-item {
                width: 100%;
                max-width: 100%;
            }

            .product-toolbar,
            .product-grid-wrap {
                padding: 14px;
            }

            .grid-item {
                width: calc((100% - 12px) / 2);
                max-width: calc((100% - 12px) / 2);
            }

            .card-actions {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 520px) {
            .grid-item {
                width: 100%;
                max-width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="gh-public-shell">
        <asp:PlaceHolder ID="ph_pending_review" runat="server" Visible="false">
            <section class="pending-review">
                <div class="pending-review-top">
                    <h1 class="pending-review-title">Gian hÃ ng Ä‘ang xÃ©t duyá»‡t</h1>
                    <div class="pending-review-sub">Gian hÃ ng nÃ y Ä‘ang chá» xÃ©t duyá»‡t.</div>
                </div>
                <div class="pending-review-body">
                    <div class="pending-review-box">
                        <h2>Gian hÃ ng nÃ y Ä‘ang trong quÃ¡ trÃ¬nh xÃ©t duyá»‡t</h2>
                        <p>Ngay khi Ä‘Æ°á»£c phÃª duyá»‡t, toÃ n bá»™ sáº£n pháº©m, dá»‹ch vá»¥ vÃ  ná»™i dung cÃ´ng khai cá»§a gian hÃ ng sáº½ hiá»ƒn thá»‹ táº¡i Ä‘Ã¢y.</p>
                        <div class="pending-review-trust">
                            <span class="pending-review-pill">Storefront cÃ´ng khai sáº½ má»Ÿ sau khi duyá»‡t</span>
                            <span class="pending-review-pill">Giá»¯ cÃ¹ng logic hiá»ƒn thá»‹ trÃªn desktop vÃ  mobile</span>
                        </div>
                        <div class="pending-review-meta">
                            <span class="pending-review-pill">MÃ£ gian hÃ ng: <asp:Label ID="lb_pending_store_account" runat="server" /></span>
                            <span class="pending-review-pill">TÃªn hiá»ƒn thá»‹: <asp:Label ID="lb_pending_store_name" runat="server" /></span>
                        </div>
                        <div class="pending-review-actions">
                            <asp:HyperLink ID="lnk_pending_home" runat="server" CssClass="btn-login">Trung tÃ¢m tÃ i khoáº£n</asp:HyperLink>
                            <a href="#storefront-footer" class="btn-soft">Xem thÃ´ng tin liÃªn há»‡</a>
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
                    <div class="hero-shell">
                        <div class="hero-showcase" aria-hidden="true"></div>
                    </div>

                    <div class="hero-main">
                        <asp:Image ID="img_avatar" runat="server" CssClass="store-avatar" />
                        <div>
                            <div class="meta-kicker"><asp:Label ID="lb_public_hero_title" runat="server" /></div>
                            <h2 class="meta-title"><asp:Label ID="lb_store_name" runat="server" /></h2>
                            <div class="meta-desc"><asp:Label ID="lb_public_hero_sub" runat="server" /></div>
                            <asp:PlaceHolder ID="ph_store_desc" runat="server" Visible="false">
                                <div class="meta-line"><asp:Label ID="lb_store_desc" runat="server" /></div>
                            </asp:PlaceHolder>
                            <div class="meta-line">KhÃ´ng gian trao Ä‘á»•i sáº£n pháº©m vÃ  dá»‹ch vá»¥ Ä‘Æ°á»£c trÃ¬nh bÃ y cÃ´ng khai Ä‘á»ƒ khÃ¡ch hÃ ng dá»… khÃ¡m phÃ¡ vÃ  káº¿t ná»‘i.</div>
                        </div>
                    </div>

                    <asp:PlaceHolder ID="ph_public_actions_legacy" runat="server" Visible="false">
                        <asp:HyperLink ID="lnk_public_primary_cta" runat="server" CssClass="btn-login" Visible="false" />
                        <asp:HyperLink ID="lnk_home_login" runat="server" CssClass="btn-soft" />
                        <asp:PlaceHolder ID="ph_owner_actions" runat="server" Visible="false">
                            <asp:HyperLink ID="lnk_owner_dashboard" runat="server" CssClass="btn-soft">Vá» trung tÃ¢m gian hÃ ng</asp:HyperLink>
                            <asp:HyperLink ID="lnk_owner_manage" runat="server" CssClass="btn-primary">Quáº£n lÃ½ tin Ä‘Äƒng</asp:HyperLink>
                        </asp:PlaceHolder>
                        <asp:Label ID="lb_store_name_banner" runat="server" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="ph_public_metrics_legacy" runat="server" Visible="false">
                        <asp:Label ID="lb_store_account" runat="server" />
                        <asp:Label ID="lb_owner_name" runat="server" />
                        <asp:Label ID="lb_total_products" runat="server" Text="0" />
                        <asp:Label ID="lb_total_services" runat="server" Text="0" />
                        <asp:Label ID="lb_total_views" runat="server" Text="0" />
                        <asp:Label ID="lb_total_sold" runat="server" Text="0" />
                    </asp:PlaceHolder>
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
                                    <article class="card featured-card">
                                        <a class="card-link" href="<%# BuildDetailUrl(Container.DataItem) %>">
                                            <img class="card-image" src="<%# Eval("ImageUrl") %>" alt="<%# EncodeAttr(Eval("Name")) %>" />
                                        </a>
                                        <div class="card-body">
                                            <a class="card-link" href="<%# BuildDetailUrl(Container.DataItem) %>">
                                                <h3 class="card-name"><%# Eval("Name") %></h3>
                                            </a>
                                            <div class="card-price"><%# FormatCurrency(Eval("Price")) %> Ä‘</div>
                                            <div class="card-badges">
                                                <span class="<%# BuildTypeCss(Eval("ProductType")) %>"><%# BuildTypeLabel(Eval("ProductType")) %></span>
                                                <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("CategoryName"))) %>'>
                                                    <span class="card-badge card-badge-category"><%# Eval("CategoryName") %></span>
                                                </asp:PlaceHolder>
                                            </div>
                                        </div>
                                        <div class="card-actions">
                                            <a class="card-btn card-btn-trade" href="<%# BuildDetailUrl(Container.DataItem) %>">Xem chi tiet</a>
                                            <asp:PlaceHolder runat="server" Visible='<%# ShowAddCart(Container.DataItem) %>'>
                                                <a class="card-btn card-btn-cart" href="<%# BuildAddCartActionUrl(Container.DataItem) %>">ThÃªm vÃ o giá»</a>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder runat="server" Visible='<%# !ShowAddCart(Container.DataItem) %>'>
                                                <a class="card-btn card-btn-cart" href="<%# BuildExchangeActionUrl(Container.DataItem) %>"><%# Eval("ActionLabel") %></a>
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
                        <h3>ChÆ°a cÃ³ top tin ná»•i báº­t</h3>
                        <p>Khi gian hÃ ng báº¯t Ä‘áº§u cÃ³ lÆ°á»£t xem vÃ  bÃ¡n hÃ ng, khá»‘i nÃ y sáº½ tá»± cáº­p nháº­t.</p>
                    </div>
                </asp:PlaceHolder>
            </section>

            <section class="discovery-strip">
                <div class="discovery-strip__head">
                    <div>
                        <h2 class="discovery-strip__title">KhÃ¡m phÃ¡ nhanh</h2>
                        <div class="discovery-strip__sub">Chá»n nhanh loáº¡i tin hoáº·c danh má»¥c ná»•i báº­t Ä‘á»ƒ Ä‘i tháº³ng vÃ o pháº§n báº¡n Ä‘ang quan tÃ¢m.</div>
                    </div>
                    <a href="#gianhang-products" class="btn-soft">Xem toÃ n bá»™</a>
                </div>
                <div class="discovery-strip__chips">
                    <a href="#gianhang-products" class="discovery-chip" data-discovery-type="">Táº¥t cáº£</a>
                    <a href="#gianhang-products" class="discovery-chip" data-discovery-type="product">Sáº£n pháº©m</a>
                    <a href="#gianhang-products" class="discovery-chip" data-discovery-type="service">Dá»‹ch vá»¥</a>
                    <asp:Literal ID="lit_category_discovery" runat="server" />
                </div>
            </section>

            <section class="section" id="gianhang-products">
                <div class="section-head">
                    <div>
                        <h2 class="section-title"><asp:Label ID="lb_public_products_title" runat="server" /></h2>
                        <div class="section-sub"><asp:Label ID="lb_public_products_sub" runat="server" /></div>
                    </div>
                    <span class="public-url">Äang hiá»ƒn thá»‹ <asp:Literal ID="lit_product_count" runat="server" /></span>
                </div>
                <div class="product-toolbar">
                    <div class="filters">
                        <div class="filter-field">
                            <label for="ghSearch">TÃ¬m kiáº¿m</label>
                            <input id="ghSearch" class="filter-input" type="text" placeholder="TÃªn sáº£n pháº©m hoáº·c dá»‹ch vá»¥..." />
                        </div>
                        <div class="filter-field">
                            <label for="ghType">Loáº¡i tin</label>
                            <select id="ghType" class="filter-select">
                                <option value="">Táº¥t cáº£ loáº¡i tin</option>
                                <option value="product">Chá»‰ sáº£n pháº©m</option>
                                <option value="service">Chá»‰ dá»‹ch vá»¥</option>
                            </select>
                        </div>
                        <div class="filter-field">
                            <label for="ghCategory">Danh má»¥c</label>
                            <select id="ghCategory" class="filter-select">
                                <asp:Literal ID="lit_category_options" runat="server" />
                            </select>
                        </div>
                        <div class="filter-actions">
                            <button type="button" class="filter-btn" id="ghReset">LÃ m má»›i</button>
                        </div>
                    </div>
                    <div class="filter-count" id="ghCountLabel">LÆ°á»›t toÃ n bá»™ sáº£n pháº©m vÃ  dá»‹ch vá»¥ Ä‘ang hoáº¡t Ä‘á»™ng.</div>
                    <div class="filter-summary">
                        <span class="filter-summary__chip">TÃ¬m theo tÃªn máº·t hÃ ng</span>
                        <span class="filter-summary__chip">Lá»c nhanh sáº£n pháº©m hoáº·c dá»‹ch vá»¥</span>
                        <span class="filter-summary__chip">Hiá»ƒn thá»‹ giá»‘ng nhau trÃªn desktop vÃ  mobile</span>
                    </div>
                </div>

                <asp:PlaceHolder ID="ph_products" runat="server" Visible="false">
                    <div class="product-grid-wrap">
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
                                                <div class="card-desc"><%# Eval("Description") %></div>
                                                <div class="card-price"><%# FormatCurrency(Eval("Price")) %> Ä‘</div>
                                                <div class="card-badges">
                                                    <span class="<%# BuildTypeCss(Eval("ProductType")) %>"><%# BuildTypeLabel(Eval("ProductType")) %></span>
                                                    <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("CategoryName"))) %>'>
                                                        <span class="card-badge card-badge-category"><%# Eval("CategoryName") %></span>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </div>
                                            <div class="card-actions">
                                                <a class="card-btn card-btn-trade" href="<%# BuildDetailUrl(Container.DataItem) %>">Xem chi tiet</a>
                                                <asp:PlaceHolder runat="server" Visible='<%# ShowAddCart(Container.DataItem) %>'>
                                                    <a class="card-btn card-btn-cart" href="<%# BuildAddCartActionUrl(Container.DataItem) %>">ThÃªm vÃ o giá»</a>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# !ShowAddCart(Container.DataItem) %>'>
                                                    <a class="card-btn card-btn-cart" href="<%# BuildExchangeActionUrl(Container.DataItem) %>"><%# Eval("ActionLabel") %></a>
                                                </asp:PlaceHolder>
                                            </div>
                                        </article>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_empty_products" runat="server" Visible="false">
                    <div class="empty-state">
                        <h3>ChÆ°a cÃ³ sáº£n pháº©m hoáº·c dá»‹ch vá»¥ cÃ´ng khai</h3>
                        <p>HÃ£y thÃªm tin Ä‘áº§u tiÃªn Ä‘á»ƒ báº¯t Ä‘áº§u váº­n hÃ nh gian hÃ ng.</p>
                    </div>
                </asp:PlaceHolder>
            </section>

            <section class="section" id="storefront-footer">
                <div class="section-head">
                    <div>
                        <h2 class="section-title">LiÃªn há»‡ vÃ  trao Ä‘á»•i</h2>
                        <div class="section-sub">Khi quan tÃ¢m má»™t sáº£n pháº©m hoáº·c dá»‹ch vá»¥, hÃ£y Ä‘i vÃ o chi tiáº¿t Ä‘á»ƒ trao Ä‘á»•i hoáº·c Ä‘áº·t lá»‹ch theo Ä‘Ãºng luá»“ng hiá»‡n cÃ³.</div>
                    </div>
                </div>
                <div class="empty-state" style="text-align:left;">
                    <p>Trang cÃ´ng khai cá»§a gian hÃ ng Ä‘Ã£ Ä‘Æ°á»£c giáº£n lÆ°á»£c theo hÆ°á»›ng storefront: Æ°u tiÃªn xem hÃ ng, xem chi tiáº¿t vÃ  báº¯t Ä‘áº§u trao Ä‘á»•i. Desktop vÃ  mobile giá»¯ cÃ¹ng ná»™i dung, chá»‰ khÃ¡c cÃ¡ch bá»‘ trÃ­ Ä‘á»ƒ phÃ¹ há»£p mÃ n hÃ¬nh.</p>
                </div>
            </section>
        </asp:PlaceHolder>
    </div>
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script src="/Metro-UI-CSS-master/tests/metro/js/metro.min.js"></script>
    <script src="/js/bcorn.js"></script>
    <script>
        (function () {
            var search = document.getElementById('ghSearch');
            var type = document.getElementById('ghType');
            var category = document.getElementById('ghCategory');
            var reset = document.getElementById('ghReset');
            var countLabel = document.getElementById('ghCountLabel');
            var items = Array.prototype.slice.call(document.querySelectorAll('[data-filter-item="1"]'));
            var discoveryLinks = Array.prototype.slice.call(document.querySelectorAll('[data-discovery-type], [data-discovery-category]'));

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
                    countLabel.textContent = 'Hiá»ƒn thá»‹ ' + visible + ' tin phÃ¹ há»£p.';
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

            discoveryLinks.forEach(function (link) {
                link.addEventListener('click', function () {
                    var typeValue = link.getAttribute('data-discovery-type');
                    var categoryValue = link.getAttribute('data-discovery-category');
                    if (type && typeValue !== null) type.value = typeValue;
                    if (category && categoryValue !== null) category.value = categoryValue;
                    applyFilter();
                });
            });

            applyFilter();
        })();
    </script>
</asp:Content>
