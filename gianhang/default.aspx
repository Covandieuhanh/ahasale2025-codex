<%@ Page Title="Gian hÃ ng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerGianHang.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="gianhang_Default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        :root {
            --gianhang-primary-700: #f97316;
            --gianhang-primary-600: #fb923c;
            --gianhang-primary-50: #fff7ed;
            --gianhang-ink-900: #102a43;
            --gianhang-ink-700: #334e68;
            --gianhang-ink-500: #627d98;
            --gianhang-line: #d9e2ec;
            --gianhang-bg: #f5f8fb;
            --gianhang-card: #ffffff;
            --gianhang-shadow: 0 16px 36px rgba(16, 42, 67, .08);
            --radius-lg: 18px;
            --radius-md: 14px;
            --radius-sm: 10px;
        }

        .gianhang-shell {
            min-height: 100vh;
            padding: 18px 0 28px;
            color: var(--gianhang-ink-900);
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(249,115,22,.18), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(251,146,60,.14), transparent 62%),
                var(--gianhang-bg);
        }

        .gianhang-inner {
            max-width: 1220px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .gianhang-shell a {
            color: inherit;
            text-decoration: none;
        }

        .gianhang-topbar {
            background: #fff;
            border: 1px solid var(--gianhang-line);
            border-radius: var(--radius-lg);
            box-shadow: var(--gianhang-shadow);
            padding: 16px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            flex-wrap: wrap;
        }

        .gianhang-brand {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 0;
        }

        .gianhang-brand-logo {
            width: 48px;
            height: 48px;
            border-radius: 15px;
            background: linear-gradient(145deg, var(--gianhang-primary-700), var(--gianhang-primary-600));
            display: inline-flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 10px 24px rgba(249, 115, 22, .22);
            overflow: hidden;
        }

        .gianhang-brand-logo img {
            width: 28px;
            height: 28px;
            object-fit: contain;
        }

        .gianhang-brand-title {
            font-size: 22px;
            font-weight: 800;
            line-height: 1.1;
        }

        .gianhang-brand-sub {
            margin-top: 3px;
            color: var(--gianhang-ink-500);
            font-size: 13px;
        }

        .gianhang-top-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gianhang-mode-pill {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            background: var(--gianhang-primary-50);
            border: 1px solid rgba(249,115,22,.18);
            color: #c2410c;
            font-size: 12px;
            font-weight: 800;
        }

        .gianhang-avatar {
            width: 42px;
            height: 42px;
            border-radius: 50%;
            object-fit: cover;
            border: 1px solid #dce7f2;
            background: #f3f6fa;
        }

        .gianhang-btn {
            min-height: 38px;
            border-radius: 999px;
            padding: 0 16px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 800;
            border: 1px solid var(--gianhang-primary-700);
            background: var(--gianhang-primary-700);
            color: #fff;
        }

        .gianhang-btn:hover {
            background: #ea580c;
            border-color: #ea580c;
            color: #fff;
        }

        .gianhang-btn--soft {
            background: var(--gianhang-primary-50);
            color: #c2410c;
        }

        .gianhang-btn--soft:hover {
            background: #ffedd5;
            color: #9a3412;
            border-color: var(--gianhang-primary-700);
        }

        .gianhang-hero {
            margin-top: 16px;
            position: relative;
            overflow: hidden;
            color: #fff;
            border-radius: var(--radius-lg);
            box-shadow: var(--gianhang-shadow);
            padding: 18px;
            display: grid;
            grid-template-columns: minmax(0, 1.25fr) minmax(320px, .9fr);
            gap: 18px;
            background:
                radial-gradient(circle at top right, rgba(255,255,255,.24), transparent 34%),
                linear-gradient(135deg, #ffb26c 0%, #f97316 56%, #fb923c 100%);
        }

        .gianhang-hero::after {
            content: "";
            position: absolute;
            inset: auto -10% -35% auto;
            width: 320px;
            height: 320px;
            border-radius: 50%;
            background: rgba(255,255,255,.12);
            filter: blur(8px);
            pointer-events: none;
        }

        .gianhang-hero-copy,
        .gianhang-hero-visual {
            position: relative;
            z-index: 1;
        }

        .gianhang-hero-copy {
            display: flex;
            flex-direction: column;
            justify-content: center;
            min-width: 0;
            padding: 14px 6px 14px 10px;
        }

        .gianhang-hero h1 {
            margin: 0 0 6px;
            font-size: 30px;
            line-height: 1.12;
            color: #fff !important;
        }

        .gianhang-hero p {
            margin: 0;
            max-width: 560px;
            font-size: 14px;
            line-height: 1.6;
            opacity: .96;
            color: rgba(255,255,255,.94) !important;
        }

        .gianhang-hero-actions {
            margin-top: 14px;
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gianhang-hero-link {
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
            color: #fff !important;
        }

        .gianhang-hero a.gianhang-btn,
        .gianhang-hero a.gianhang-btn:link,
        .gianhang-hero a.gianhang-btn:visited,
        .gianhang-hero a.gianhang-btn:hover,
        .gianhang-hero a.gianhang-btn:focus {
            color: #fff !important;
            text-decoration: none !important;
        }

        .gianhang-hero a.gianhang-btn.gianhang-btn--soft,
        .gianhang-hero a.gianhang-btn.gianhang-btn--soft:link,
        .gianhang-hero a.gianhang-btn.gianhang-btn--soft:visited,
        .gianhang-hero a.gianhang-btn.gianhang-btn--soft:focus {
            color: #9a3412 !important;
            background: #fff7ed !important;
            border-color: rgba(249,115,22,.24) !important;
            text-decoration: none !important;
            text-shadow: none !important;
        }

        .gianhang-hero a.gianhang-btn.gianhang-btn--soft:hover,
        .gianhang-hero a.gianhang-btn.gianhang-btn--soft:active {
            color: #7c2d12 !important;
            background: #ffedd5 !important;
            border-color: rgba(249,115,22,.42) !important;
            text-decoration: none !important;
        }

        .gianhang-hero-visual {
            min-height: 240px;
            border-radius: 22px;
            overflow: hidden;
            border: 1px solid rgba(255,255,255,.28);
            box-shadow: 0 22px 38px rgba(124, 45, 18, .22);
            background:
                linear-gradient(180deg, rgba(255,255,255,.12), rgba(255,255,255,0)),
                url('<%= Helper_cl.VersionedUrl("~/uploads/images/gianhang-banner-home-style.png") %>') center center / cover no-repeat;
        }

        .gianhang-stats {
            margin-top: 14px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
        }

        .gianhang-stat-card {
            background: var(--gianhang-card);
            border: 1px solid var(--gianhang-line);
            border-radius: var(--radius-md);
            padding: 14px;
            box-shadow: var(--gianhang-shadow);
        }

        .gianhang-stat-label {
            font-size: 12px;
            color: var(--gianhang-ink-500);
            margin-bottom: 4px;
        }

        .gianhang-stat-value {
            font-size: 24px;
            line-height: 1.1;
            font-weight: 800;
            color: var(--gianhang-ink-900);
        }

        .gianhang-card {
            margin-top: 14px;
            background: var(--gianhang-card);
            border: 1px solid var(--gianhang-line);
            border-radius: var(--radius-lg);
            box-shadow: var(--gianhang-shadow);
            overflow: hidden;
        }

        .gianhang-card-head {
            padding: 14px 16px;
            border-bottom: 1px solid #e7edf4;
        }

        .gianhang-card-title {
            margin: 0;
            font-size: 20px;
            line-height: 1.2;
            color: var(--gianhang-ink-900);
        }

        .gianhang-card-sub {
            margin-top: 3px;
            font-size: 13px;
            color: var(--gianhang-ink-500);
        }

        .gianhang-card-body {
            padding: 16px;
        }

        .gianhang-wallet-grid {
            margin-top: 14px;
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 14px;
        }

        .gianhang-wallet-card {
            position: relative;
            overflow: hidden;
            border-radius: var(--radius-lg);
            padding: 18px;
            color: #fff;
            box-shadow: var(--gianhang-shadow);
            display: flex;
            flex-direction: column;
            gap: 10px;
            min-height: 190px;
        }

        .gianhang-wallet-card::after {
            content: "";
            position: absolute;
            right: -36px;
            top: -36px;
            width: 140px;
            height: 140px;
            border-radius: 50%;
            background: rgba(255,255,255,.12);
        }

        .gianhang-wallet-card--consumer {
            background: linear-gradient(135deg, #0f766e 0%, #14b8a6 100%);
        }

        .gianhang-wallet-card--discount {
            background: linear-gradient(135deg, #7c3aed 0%, #a855f7 100%);
        }

        .gianhang-wallet-card > * {
            position: relative;
            z-index: 1;
        }

        .gianhang-wallet-eyebrow {
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .08em;
            text-transform: uppercase;
            opacity: .88;
        }

        .gianhang-wallet-title {
            margin: 0;
            font-size: 24px;
            line-height: 1.2;
            color: #fff;
        }

        .gianhang-wallet-value {
            font-size: 34px;
            line-height: 1;
            font-weight: 900;
        }

        .gianhang-wallet-note {
            font-size: 13px;
            line-height: 1.6;
            color: rgba(255,255,255,.9);
        }

        .gianhang-wallet-action {
            margin-top: auto;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 38px;
            border-radius: 999px;
            padding: 0 16px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.32);
            color: #fff !important;
            font-size: 13px;
            font-weight: 800;
            width: fit-content;
        }

        .gianhang-top-products {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 12px;
        }

        .gianhang-top-product {
            display: flex;
            gap: 10px;
            align-items: center;
            padding: 10px;
            border-radius: var(--radius-sm);
            border: 1px solid var(--gianhang-line);
            background: #fff;
        }

        .gianhang-top-product img {
            width: 56px;
            height: 56px;
            border-radius: 12px;
            object-fit: cover;
            border: 1px solid #e1e8f0;
            background: #f3f6fa;
        }

        .gianhang-top-product-title {
            font-weight: 800;
            color: var(--gianhang-ink-900);
            line-height: 1.2;
        }

        .gianhang-top-product-meta {
            font-size: 12px;
            color: var(--gianhang-ink-500);
        }

        .gianhang-product-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(180px, 1fr));
            gap: 14px;
        }

        .gianhang-product-card {
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

        .gianhang-product-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 12px 24px rgba(16, 42, 67, .16);
        }

        .gianhang-product-thumb {
            position: relative;
            width: 100%;
            padding-top: 100%;
            background: #f4f8fd;
            overflow: hidden;
            display: block;
        }

        .gianhang-product-thumb img {
            position: absolute;
            inset: 0;
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .gianhang-product-body {
            padding: 12px;
            display: flex;
            flex-direction: column;
            gap: 8px;
            flex: 1;
        }

        .gianhang-product-name {
            font-size: 15px;
            font-weight: 700;
            line-height: 1.35;
            color: var(--gianhang-ink-900);
            min-height: 40px;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .gianhang-product-price {
            color: #d63939;
            font-size: 16px;
            font-weight: 800;
        }

        .gianhang-product-badges {
            display: flex;
            align-items: center;
            gap: 6px;
            flex-wrap: wrap;
        }

        .gianhang-product-badge {
            display: inline-flex;
            align-items: center;
            min-height: 22px;
            border-radius: 999px;
            padding: 0 8px;
            font-size: 11px;
            font-weight: 800;
        }

        .gianhang-product-badge-product {
            color: #c2410c;
            background: #fff3ed;
            border: 1px solid #ffd4c4;
        }

        .gianhang-product-badge-service {
            color: #0f766e;
            background: #ecfeff;
            border: 1px solid #a5f3fc;
        }

        .gianhang-product-meta {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
            font-size: 12px;
            color: var(--gianhang-ink-500);
        }

        .gianhang-product-actions {
            margin-top: auto;
            display: grid;
            grid-template-columns: 1fr;
            gap: 8px;
        }

        .gianhang-btn-strong,
        .gianhang-btn-soft {
            min-height: 34px;
            border-radius: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 800;
        }

        .gianhang-btn-strong {
            border: 1px solid var(--gianhang-primary-700);
            color: #fff;
            background: var(--gianhang-primary-700);
        }

        .gianhang-btn-soft {
            border: 1px solid var(--gianhang-primary-700);
            color: #c2410c;
            background: var(--gianhang-primary-50);
        }

        .gianhang-empty-state {
            border: 1px dashed #c3d3e5;
            border-radius: var(--radius-md);
            background: #fbfdff;
            padding: 22px;
            text-align: center;
            color: var(--gianhang-ink-700);
        }

        .gianhang-empty-state h3 {
            margin: 0 0 8px;
            font-size: 20px;
            color: var(--gianhang-ink-900);
        }

        .gianhang-state {
            padding: 18px 20px;
            border-radius: 20px;
            margin-top: 18px;
        }

        .gianhang-state--success {
            background: #ecfdf3;
            border: 1px solid rgba(34,197,94,.2);
            color: #166534;
        }

        .gianhang-state--warning {
            background: var(--gianhang-primary-50);
            border: 1px solid rgba(249,115,22,.2);
            color: #9a3412;
        }

        .gianhang-state--danger {
            background: #fef2f2;
            border: 1px solid rgba(239,68,68,.2);
            color: #991b1b;
        }

        .gianhang-access-grid {
            display: grid;
            grid-template-columns: 1.15fr .85fr;
            gap: 20px;
            margin-top: 16px;
        }

        .gianhang-roadmap {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 14px;
            margin-top: 20px;
        }

        .gianhang-roadmap__item {
            padding: 18px;
            border-radius: 18px;
            background: #fff7ed;
            border: 1px solid rgba(249,115,22,.16);
        }

        .gianhang-roadmap__item h3 {
            margin: 0 0 8px;
            font-size: 18px;
            color: var(--gianhang-ink-900);
        }

        @media (max-width: 1140px) {
            .gianhang-product-grid { grid-template-columns: repeat(3, minmax(180px, 1fr)); }
        }

        @media (max-width: 767px) {
            .gianhang-access-grid,
            .gianhang-roadmap,
            .gianhang-wallet-grid {
                grid-template-columns: 1fr;
            }

            .gianhang-hero {
                grid-template-columns: 1fr;
            }

            .gianhang-hero-copy {
                padding: 4px 2px 0;
            }

            .gianhang-hero-visual {
                min-height: 200px;
                order: -1;
            }
        }

        @media (max-width: 767px) {
            .gianhang-stats { grid-template-columns: repeat(2, minmax(120px, 1fr)); }
            .gianhang-product-grid { grid-template-columns: repeat(2, minmax(150px, 1fr)); }
        }

        @media (max-width: 640px) {
            .gianhang-inner { padding: 0 12px; }
            .gianhang-shell { padding-top: 12px; }
            .gianhang-brand-title { font-size: 18px; }
            .gianhang-hero h1 { font-size: 22px; }
            .gianhang-hero { padding: 14px; gap: 14px; }
            .gianhang-hero-actions { flex-direction: column; align-items: stretch; }
            .gianhang-hero-actions .gianhang-btn,
            .gianhang-hero-actions .gianhang-btn--soft { width: 100%; justify-content: center; }
            .gianhang-hero-link { width: 100%; justify-content: center; }
            .gianhang-hero-visual { min-height: 160px; border-radius: 18px; }
            .gianhang-product-grid { grid-template-columns: 1fr; }
            .gianhang-top-products { grid-template-columns: 1fr; }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="gianhang-shell">
        <div class="gianhang-inner">
            <asp:PlaceHolder ID="ph_message" runat="server" Visible="false">
                <div id="box_message" runat="server" class="gianhang-state gianhang-state--warning">
                    <asp:Literal ID="lit_message" runat="server" />
                </div>
            </asp:PlaceHolder>

            <section class="gianhang-hero">
                <div class="gianhang-hero-copy">
                    <h1><asp:Label ID="lb_hero_title" runat="server" Text="Gian hÃ ng Ä‘á»‘i tÃ¡c"></asp:Label></h1>
                    <p><asp:Label ID="lb_hero_desc" runat="server" Text="KhÃ´ng gian Ä‘á»™c láº­p Ä‘á»ƒ Ä‘Äƒng tin, nháº­n Ä‘Æ¡n vÃ  váº­n hÃ nh bÃ¡n hÃ ng trÃªn AhaSale."></asp:Label></p>
                    <div class="gianhang-hero-actions">
                        <asp:HyperLink ID="lnk_storefront_primary_cta" runat="server" CssClass="gianhang-btn" Visible="false"></asp:HyperLink>
                        <asp:HyperLink ID="lnk_storefront_secondary_cta" runat="server" CssClass="gianhang-btn gianhang-btn--soft" Visible="false"></asp:HyperLink>
                        <asp:HyperLink ID="lnk_storefront_tertiary_cta" runat="server" CssClass="gianhang-btn gianhang-btn--soft" Visible="false"></asp:HyperLink>
                    </div>
                    <div class="gianhang-hero-actions">
                        <asp:HyperLink ID="lnk_storefront_profile_top" runat="server" CssClass="gianhang-hero-link" Target="_blank" Visible="false"></asp:HyperLink>
                    </div>
                </div>
                <div class="gianhang-hero-visual" aria-hidden="true"></div>
            </section>

            <asp:PlaceHolder ID="ph_storefront_active" runat="server" Visible="false">
                <section class="gianhang-stats">
                    <article class="gianhang-stat-card">
                        <div class="gianhang-stat-label"><asp:Label ID="lb_stat_products_label" runat="server" Text="Sáº£n pháº©m"></asp:Label></div>
                        <div class="gianhang-stat-value"><asp:Label ID="lb_stat_products" runat="server" Text="0"></asp:Label></div>
                    </article>
                    <article class="gianhang-stat-card">
                        <div class="gianhang-stat-label"><asp:Label ID="lb_stat_services_label" runat="server" Text="Dá»‹ch vá»¥"></asp:Label></div>
                        <div class="gianhang-stat-value"><asp:Label ID="lb_stat_services" runat="server" Text="0"></asp:Label></div>
                    </article>
                    <article class="gianhang-stat-card">
                        <div class="gianhang-stat-label"><asp:Label ID="lb_stat_pending_orders_label" runat="server" Text="ÄÆ¡n Ä‘ang chá» trao Ä‘á»•i"></asp:Label></div>
                        <div class="gianhang-stat-value"><asp:Label ID="lb_stat_pending_orders" runat="server" Text="0"></asp:Label></div>
                    </article>
                </section>

                <section class="gianhang-wallet-grid">
                    <article class="gianhang-wallet-card gianhang-wallet-card--consumer">
                        <div class="gianhang-wallet-eyebrow">Há»“ sÆ¡ gian hÃ ng</div>
                        <h2 class="gianhang-wallet-title">Há»“ sÆ¡ quyá»n tiÃªu dÃ¹ng</h2>
                        <div class="gianhang-wallet-value"><asp:Literal ID="lit_wallet_tieudung" runat="server" Text="0 A"></asp:Literal></div>
                        <div class="gianhang-wallet-note">Theo dÃµi sá»‘ dÆ° quyá»n tiÃªu dÃ¹ng vÃ  Ä‘i vÃ o lá»‹ch sá»­ phÃ¡t sinh cá»§a gian hÃ ng.</div>
                        <asp:HyperLink ID="lnk_wallet_tieudung" runat="server" CssClass="gianhang-wallet-action" NavigateUrl="/gianhang/ho-so-tieu-dung.aspx">Má»Ÿ há»“ sÆ¡ quyá»n tiÃªu dÃ¹ng</asp:HyperLink>
                    </article>
                    <article class="gianhang-wallet-card gianhang-wallet-card--discount">
                        <div class="gianhang-wallet-eyebrow">Há»“ sÆ¡ gian hÃ ng</div>
                        <h2 class="gianhang-wallet-title">Há»“ sÆ¡ quyá»n Æ°u Ä‘Ã£i</h2>
                        <div class="gianhang-wallet-value"><asp:Literal ID="lit_wallet_uudai" runat="server" Text="0 A"></asp:Literal></div>
                        <div class="gianhang-wallet-note">Xem nhanh quá»¹ quyá»n Æ°u Ä‘Ã£i hiá»‡n cÃ³ vÃ  má»Ÿ há»“ sÆ¡ chi tiáº¿t chá»‰ vá»›i má»™t cháº¡m.</div>
                        <asp:HyperLink ID="lnk_wallet_uudai" runat="server" CssClass="gianhang-wallet-action" NavigateUrl="/gianhang/ho-so-uu-dai.aspx">Má»Ÿ há»“ sÆ¡ quyá»n Æ°u Ä‘Ã£i</asp:HyperLink>
                    </article>
                </section>

                <section class="gianhang-card">
                    <div class="gianhang-card-head">
                        <div>
                            <h2 class="gianhang-card-title"><asp:Label ID="lb_top_products_title" runat="server" Text="Top tin ná»•i báº­t"></asp:Label></h2>
                            <div class="gianhang-card-sub"><asp:Label ID="lb_top_products_sub" runat="server" Text="Æ¯u tiÃªn hiá»ƒn thá»‹ cÃ¡c tin ná»•i báº­t cá»§a gian hÃ ng."></asp:Label></div>
                        </div>
                    </div>
                    <div class="gianhang-card-body">
                        <asp:PlaceHolder ID="ph_top_products" runat="server" Visible="false">
                            <div class="gianhang-top-products">
                                <asp:Repeater ID="rpt_top_products" runat="server">
                                    <ItemTemplate>
                                        <a class='gianhang-top-product' href='<%# BuildProductDetailUrl(Container.DataItem) %>'>
                                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' loading='lazy' decoding='async' />
                                            <div>
                                                <div class="gianhang-top-product-title"><%# Eval("Name") %></div>
                                                <div class="gianhang-top-product-meta">ÄÃ£ bÃ¡n: <%# Eval("SoldCount") %> Â· LÆ°á»£t xem: <%# Eval("Views") %></div>
                                            </div>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_empty_top_products" runat="server" Visible="false">
                            <div class="gianhang-empty-state">
                                <h3>ChÆ°a cÃ³ dá»¯ liá»‡u hiá»ƒn thá»‹</h3>
                                <p>CÃ¡c tin ná»•i báº­t sáº½ xuáº¥t hiá»‡n khi gian hÃ ng báº¯t Ä‘áº§u cÃ³ dá»¯ liá»‡u hoáº¡t Ä‘á»™ng.</p>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </section>

                <section class="gianhang-card">
                    <div class="gianhang-card-head">
                        <div>
                            <h2 class="gianhang-card-title"><asp:Label ID="lb_products_title" runat="server" Text="Danh sÃ¡ch sáº£n pháº©m vÃ  dá»‹ch vá»¥"></asp:Label></h2>
                            <div class="gianhang-card-sub"><asp:Label ID="lb_products_sub" runat="server" Text="Danh sÃ¡ch sáº£n pháº©m vÃ  dá»‹ch vá»¥ Ä‘ang hoáº¡t Ä‘á»™ng."></asp:Label></div>
                        </div>
                    </div>
                    <div class="gianhang-card-body">
                        <asp:PlaceHolder ID="ph_empty_products" runat="server" Visible="false">
                            <div class="gianhang-empty-state">
                                <h3>ChÆ°a cÃ³ sáº£n pháº©m hoáº·c dá»‹ch vá»¥</h3>
                                <p>HÃ£y thÃªm tin Ä‘áº§u tiÃªn Ä‘á»ƒ báº¯t Ä‘áº§u váº­n hÃ nh gian hÃ ng.</p>
                                <a class="gianhang-btn" href="/gianhang/quan-ly-tin/Default.aspx">Má»Ÿ quáº£n lÃ½ tin</a>
                            </div>
                        </asp:PlaceHolder>

                        <div class="gianhang-product-grid">
                            <asp:Repeater ID="rp_products" runat="server">
                                <ItemTemplate>
                                    <article class="gianhang-product-card">
                                        <a class='gianhang-product-thumb' href='<%# BuildProductDetailUrl(Container.DataItem) %>'>
                                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' loading='lazy' decoding='async' />
                                        </a>
                                        <div class="gianhang-product-body">
                                            <a class='gianhang-product-name' href='<%# BuildProductDetailUrl(Container.DataItem) %>'><%# Eval("Name") %></a>
                                            <div class="gianhang-product-price"><%# FormatCurrency(Eval("Price")) %> Ä‘</div>
                                            <div class="gianhang-product-badges">
                                                <span class='<%# BuildProductTypeCss(Container.DataItem) %>'><%# BuildProductTypeLabel(Container.DataItem) %></span>
                                            </div>
                                            <div class="gianhang-product-meta">
                                                <span>NgÃ y Ä‘Äƒng: <%# Eval("CreatedAt", "{0:dd/MM/yyyy}") %></span>
                                                <span>LÆ°á»£t xem: <%# Eval("Views") %></span>
                                            </div>
                                            <div class="gianhang-product-actions">
                                                <a class='gianhang-btn-strong' href='<%# BuildProductDetailUrl(Container.DataItem) %>'>Xem chi tiáº¿t</a>
                                                <a class='gianhang-btn-soft' href='<%# BuildProductActionUrl(Container.DataItem) %>'><%# BuildProductActionText(Container.DataItem) %></a>
                                            </div>
                                        </div>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </section>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_access_shell" runat="server" Visible="false">
                <div class="gianhang-card">
                    <div class="gianhang-card-body">
                        <span class="gianhang-mode-pill">KhÃ´ng gian gian hÃ ng</span>
                        <h1 class="mt-3 mb-2">KhÃ´ng gian gian hÃ ng</h1>
                        <p class="text-muted mb-0">KÃ­ch hoáº¡t gian hÃ ng Ä‘á»ƒ báº¯t Ä‘áº§u Ä‘Äƒng tin, nháº­n Ä‘Æ¡n vÃ  váº­n hÃ nh bÃ¡n hÃ ng.</p>

                        <div class="gianhang-stats">
                            <article class="gianhang-stat-card">
                                <div class="gianhang-stat-label">TÃ i khoáº£n gá»‘c</div>
                                <div class="gianhang-stat-value"><asp:Literal ID="lit_account_key" runat="server" /></div>
                            </article>
                            <article class="gianhang-stat-card">
                                <div class="gianhang-stat-label">Tráº¡ng thÃ¡i</div>
                                <div class="gianhang-stat-value"><asp:Literal ID="lit_space_status" runat="server" /></div>
                            </article>
                            <article class="gianhang-stat-card">
                                <div class="gianhang-stat-label">Loáº¡i khÃ´ng gian</div>
                                <div class="gianhang-stat-value">Gian hÃ ng</div>
                            </article>
                        </div>
                    </div>
                </div>

                <div class="gianhang-access-grid">
                    <div class="gianhang-card">
                        <div class="gianhang-card-body">
                            <h2 class="mb-2">Tráº¡ng thÃ¡i má»Ÿ khÃ´ng gian</h2>
                            <p class="text-muted mt-0">Theo dÃµi tÃ¬nh tráº¡ng má»Ÿ quyá»n vÃ  gá»­i yÃªu cáº§u khi cáº§n.</p>

                            <asp:PlaceHolder ID="ph_state_pending" runat="server" Visible="false">
                                <div class="gianhang-state gianhang-state--warning">
                                    <div class="fw-bold mb-1">Äang chá» admin duyá»‡t</div>
                                    <div class="mb-1">YÃªu cáº§u má»Ÿ gian hÃ ng Ä‘Ã£ Ä‘Æ°á»£c gá»­i. Báº¡n chÆ°a cáº§n thao tÃ¡c láº¡i.</div>
                                    <div class="small">Láº§n gá»­i gáº§n nháº¥t: <asp:Literal ID="lit_requested_at" runat="server" />.</div>
                                    <asp:PlaceHolder ID="ph_review_note_pending" runat="server" Visible="false">
                                        <div class="small mt-2">Ghi chÃº admin: <asp:Literal ID="lit_review_note_pending" runat="server" /></div>
                                    </asp:PlaceHolder>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="ph_state_request" runat="server" Visible="false">
                                <div class="gianhang-state gianhang-state--warning">
                                    <div class="fw-bold mb-1">TÃ i khoáº£n chÆ°a má»Ÿ khÃ´ng gian gian hÃ ng</div>
                                    <div>HÃ£y gá»­i yÃªu cáº§u Ä‘á»ƒ Ä‘Æ°á»£c má»Ÿ quyá»n sá»­ dá»¥ng khÃ´ng gian nÃ y.</div>
                                    <div class="mt-3 d-flex flex-wrap gap-2">
                                        <asp:Button ID="btn_request_open" runat="server" CssClass="gianhang-btn" Text="Gá»­i yÃªu cáº§u má»Ÿ gian hÃ ng" OnClick="btn_request_open_Click" />
                                        <a class="gianhang-btn gianhang-btn--soft" href="/gianhang/tai-khoan/default.aspx">Trung tÃ¢m tÃ i khoáº£n</a>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="ph_state_blocked" runat="server" Visible="false">
                                <div class="gianhang-state gianhang-state--danger">
                                    <div class="fw-bold mb-1">KhÃ´ng gian gian hÃ ng Ä‘ang bá»‹ khÃ³a hoáº·c Ä‘Ã£ bá»‹ thu há»“i</div>
                                    <div class="mb-1">TÃ i khoáº£n nÃ y chÆ°a thá»ƒ tiáº¿p tá»¥c vÃ o gian hÃ ng. HÃ£y kiá»ƒm tra ghi chÃº duyá»‡t hoáº·c liÃªn há»‡ admin.</div>
                                    <asp:PlaceHolder ID="ph_review_note_blocked" runat="server" Visible="false">
                                        <div class="small">Ghi chÃº admin: <asp:Literal ID="lit_review_note_blocked" runat="server" /></div>
                                    </asp:PlaceHolder>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="gianhang-card">
                        <div class="gianhang-card__body">
                            <h2 class="mb-2">TÃ­nh nÄƒng sáºµn cÃ³</h2>
                            <div class="gianhang-roadmap">
                                <div class="gianhang-roadmap__item">
                                    <h3>Trang cÃ´ng khai</h3>
                                    <div class="text-muted">Hiá»ƒn thá»‹ sáº£n pháº©m, dá»‹ch vá»¥ vÃ  thÃ´ng tin gian hÃ ng cho khÃ¡ch truy cáº­p.</div>
                                </div>
                                <div class="gianhang-roadmap__item">
                                    <h3>ÄÆ¡n bÃ¡n vÃ  POS</h3>
                                    <div class="text-muted">Táº¡o Ä‘Æ¡n, theo dÃµi thanh toÃ¡n vÃ  xá»­ lÃ½ bÃ¡n hÃ ng ngay trong cÃ¹ng má»™t khÃ´ng gian.</div>
                                </div>
                                <div class="gianhang-roadmap__item">
                                    <h3>KhÃ¡ch hÃ ng vÃ  lá»‹ch háº¹n</h3>
                                    <div class="text-muted">Quáº£n lÃ½ khÃ¡ch quan tÃ¢m, lá»‹ch háº¹n vÃ  cÃ¡c tÆ°Æ¡ng tÃ¡c phÃ¡t sinh tá»« gian hÃ ng.</div>
                                </div>
                                <div class="gianhang-roadmap__item">
                                    <h3>BÃ¡o cÃ¡o váº­n hÃ nh</h3>
                                    <div class="text-muted">Theo dÃµi chá»‰ sá»‘ bÃ¡n hÃ ng, lá»‹ch háº¹n vÃ  hiá»‡u quáº£ hoáº¡t Ä‘á»™ng theo thá»i gian.</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
