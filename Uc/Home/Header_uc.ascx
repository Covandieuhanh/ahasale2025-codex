<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header_uc.ascx.cs" Inherits="Uc_Home_Header_uc" %>
<%@ Register Src="~/Uc/Shared/SpaceLauncher_uc.ascx" TagPrefix="uc1" TagName="SpaceLauncher" %>
<style>
    :root {
        --aha-top-icon-bg: #ffffff;
        --aha-top-icon-border: #d9e2ec;
        --aha-top-icon-text: #111111;
        --aha-topbar-height: 54px;
        --aha-logo-available-width: calc(100vw - 148px);
    }

    html[data-bs-theme="dark"],
    body[data-bs-theme="dark"],
    body.theme-dark,
    body.dark,
    body.dark-mode {
        --aha-top-icon-bg: #1f2937;
        --aha-top-icon-border: #334155;
        --aha-top-icon-text: #f3f4f6;
    }

    .notif-item {
        position: relative;
    }

    .mode-badge {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        padding: 6px 10px;
        border-radius: 999px;
        font-size: 12px;
        font-weight: 600;
        border: 1px solid transparent;
        white-space: nowrap;
    }

    .mode-badge.mode-home {
        background: #e7f5ff;
        border-color: #a5d8ff;
        color: #1c7ed6;
    }

    .mode-badge.mode-shop {
        background: #fff4e6;
        border-color: #ffd8a8;
        color: #d9480f;
    }

    .mode-badge.mode-admin {
        background: #f3f0ff;
        border-color: #d0bfff;
        color: #6741d9;
    }

    .mode-badge.mode-gianhang-admin {
        background: #eef2ff;
        border-color: #c7d2fe;
        color: #4338ca;
    }

    .mode-badge.mode-daugia {
        background: #fdf2f8;
        border-color: #fbcfe8;
        color: #be185d;
    }

    .mode-badge.mode-event {
        background: #fff7ed;
        border-color: #fdba74;
        color: #c2410c;
    }

    .mode-badge.mode-mobile {
        font-size: 10px;
        padding: 4px 8px;
    }

    .mobile-left-icons .mode-badge.mode-mobile {
        display: inline-flex !important;
        max-width: 148px;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
        line-height: 1.2;
    }

    .mobile-auth-link {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-height: 34px;
        padding: 0 12px;
        border-radius: 999px;
        font-size: 12px;
        font-weight: 700;
        text-decoration: none;
        white-space: nowrap;
    }

    .mobile-auth-link.login {
        background: #0f172a;
        color: #ffffff;
    }

    .mobile-auth-link.signup {
        background: #e9f7ef;
        color: #146c43;
        border: 1px solid rgba(25,135,84,.45);
    }

    .home-top-circle-btn {
        width: 46px;
        height: 46px;
        min-width: 46px;
        min-height: 46px;
        border-radius: 999px;
        border: 1px solid var(--aha-top-icon-border);
        background: var(--aha-top-icon-bg);
        color: var(--aha-top-icon-text);
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: all .2s ease;
    }

    .site-header .home-top-circle-btn i,
    .site-header .home-top-circle-btn .ti,
    .site-header .d-lg-none .btn.btn-icon.bg-white i,
    .site-header .d-lg-none .btn.btn-icon.bg-white .ti {
        font-size: 22px;
        color: var(--aha-top-icon-text) !important;
    }

    .site-header .d-lg-none .btn.btn-icon.bg-white {
        border: 1px solid var(--aha-top-icon-border) !important;
        background: var(--aha-top-icon-bg) !important;
    }

    .home-top-circle-btn:hover {
        border-color: #c8d4e1;
        background: #f8fafc;
        transform: translateY(-1px);
    }

    #notifPopup {
        background: var(--aha-home-dd-bg, #ffffff) !important;
        border: 1px solid var(--aha-home-dd-border, #dbe6ef);
        color: var(--aha-home-dd-text, #102a43);
    }

    .home-dropdown-avatar {
        width: 88px !important;
        height: 88px !important;
        min-width: 88px !important;
        min-height: 88px !important;
        max-width: 88px !important;
        max-height: 88px !important;
        aspect-ratio: 1 / 1;
        border-radius: 50% !important;
        display: inline-block;
        overflow: hidden;
        background-size: cover !important;
        background-position: center center !important;
        background-repeat: no-repeat !important;
        flex: 0 0 88px;
    }

    .home-space-access-card {
        margin: 0 8px 16px;
        border: 0;
        background: transparent;
        border-radius: 0;
        padding: 0;
        box-shadow: none;
    }

    .home-space-access-title {
        margin: 0 0 10px;
        padding: 0 4px;
        font-size: 12px;
        font-weight: 800;
        letter-spacing: .04em;
        text-transform: uppercase;
        color: #64748b;
    }

    .home-space-access-list {
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .home-space-access-link {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 10px;
        text-decoration: none;
        color: #c2410c;
        background: #fff7ed;
        border: 1px solid rgba(249, 115, 22, .28);
        border-radius: 999px;
        padding: 12px 16px;
        box-shadow: none;
        transition: background-color .2s ease, border-color .2s ease, color .2s ease, transform .2s ease;
    }

    .home-space-access-link:hover {
        background: #ffedd5;
        border-color: rgba(234, 88, 12, .42);
        color: #9a3412;
        transform: translateY(-1px);
    }

    .home-space-access-card.site-header-space-home .home-space-access-link {
        color: #166534;
        background: #f0fdf4;
        border-color: rgba(34, 197, 94, .26);
    }

    .home-space-access-card.site-header-space-home .home-space-access-link:hover {
        background: #dcfce7;
        border-color: rgba(22, 163, 74, .38);
        color: #14532d;
    }

    .gianhang-space-menu-card {
        margin-bottom: 16px;
        border-radius: 16px;
        overflow: hidden;
        border: 1px solid #fed7aa;
        background: #fff7ed;
    }

    .gianhang-space-menu-card .list-group-item {
        border-color: rgba(249, 115, 22, .14) !important;
    }

    .gianhang-space-menu-title {
        padding: 14px 16px 10px;
        font-size: 12px;
        font-weight: 800;
        letter-spacing: .04em;
        text-transform: uppercase;
        color: #9a3412;
        background: rgba(255,255,255,.55);
    }

    .gianhang-space-link {
        background: #fff7ed !important;
        transition: background-color .2s ease, border-color .2s ease, color .2s ease;
    }

    .gianhang-space-link:hover {
        background: #ffedd5 !important;
    }

    .gianhang-space-link.is-active {
        background: linear-gradient(135deg, #fb923c, #f97316) !important;
        color: #ffffff !important;
    }

    .gianhang-space-link.is-active .text-secondary,
    .gianhang-space-link.is-active .text-muted,
    .gianhang-space-link.is-active .fw-medium,
    .gianhang-space-link.is-active .ti {
        color: #ffffff !important;
    }

    .home-space-access-copy {
        display: flex;
        flex-direction: column;
        min-width: 0;
    }

    .home-space-access-copy strong {
        font-size: 14px;
        color: #9a3412;
        line-height: 1.3;
        font-weight: 800;
    }

    .home-space-access-copy span {
        font-size: 11px;
        color: #c2410c;
        opacity: .82;
        line-height: 1.4;
    }

    .home-space-access-card.site-header-space-home .home-space-access-copy strong {
        color: #166534;
    }

    .home-space-access-card.site-header-space-home .home-space-access-copy span {
        color: #15803d;
    }

    html[data-bs-theme="dark"] .home-space-access-card,
    body[data-bs-theme="dark"] .home-space-access-card,
    body.theme-dark .home-space-access-card,
    body.dark .home-space-access-card,
    body.dark-mode .home-space-access-card {
        background: transparent;
        border-color: transparent;
    }

    html[data-bs-theme="dark"] .home-space-access-title,
    body[data-bs-theme="dark"] .home-space-access-title,
    body.theme-dark .home-space-access-title,
    body.dark .home-space-access-title,
    body.dark-mode .home-space-access-title {
        color: #94a3b8;
    }

    html[data-bs-theme="dark"] .home-space-access-link,
    body[data-bs-theme="dark"] .home-space-access-link,
    body.theme-dark .home-space-access-link,
    body.dark .home-space-access-link,
    body.dark-mode .home-space-access-link {
        background: rgba(249, 115, 22, .14);
        border-color: rgba(251, 146, 60, .34);
        color: #fdba74;
        box-shadow: none;
    }

    html[data-bs-theme="dark"] .home-space-access-card.site-header-space-home .home-space-access-link,
    body[data-bs-theme="dark"] .home-space-access-card.site-header-space-home .home-space-access-link,
    body.theme-dark .home-space-access-card.site-header-space-home .home-space-access-link,
    body.dark .home-space-access-card.site-header-space-home .home-space-access-link,
    body.dark-mode .home-space-access-card.site-header-space-home .home-space-access-link {
        background: rgba(34, 197, 94, .16);
        border-color: rgba(74, 222, 128, .34);
        color: #86efac;
    }

    html[data-bs-theme="dark"] .home-space-access-copy strong,
    body[data-bs-theme="dark"] .home-space-access-copy strong,
    body.theme-dark .home-space-access-copy strong,
    body.dark .home-space-access-copy strong,
    body.dark-mode .home-space-access-copy strong {
        color: #fdba74;
    }

    html[data-bs-theme="dark"] .home-space-access-card.site-header-space-home .home-space-access-copy strong,
    body[data-bs-theme="dark"] .home-space-access-card.site-header-space-home .home-space-access-copy strong,
    body.theme-dark .home-space-access-card.site-header-space-home .home-space-access-copy strong,
    body.dark .home-space-access-card.site-header-space-home .home-space-access-copy strong,
    body.dark-mode .home-space-access-card.site-header-space-home .home-space-access-copy strong {
        color: #bbf7d0;
    }

    html[data-bs-theme="dark"] .home-space-access-copy span,
    body[data-bs-theme="dark"] .home-space-access-copy span,
    body.theme-dark .home-space-access-copy span,
    body.dark .home-space-access-copy span,
    body.dark-mode .home-space-access-copy span {
        color: #fed7aa;
        opacity: .86;
    }

    html[data-bs-theme="dark"] .home-space-access-card.site-header-space-home .home-space-access-copy span,
    body[data-bs-theme="dark"] .home-space-access-card.site-header-space-home .home-space-access-copy span,
    body.theme-dark .home-space-access-card.site-header-space-home .home-space-access-copy span,
    body.dark .home-space-access-card.site-header-space-home .home-space-access-copy span,
    body.dark-mode .home-space-access-card.site-header-space-home .home-space-access-copy span {
        color: #86efac;
        opacity: .88;
    }

    .site-header.site-header-space-home {
        background: linear-gradient(90deg, #176a4a 0%, #1f7a52 56%, #39a86e 100%);
        box-shadow: 0 10px 28px rgba(12, 46, 30, .18);
    }

    .site-header.site-header-space-home .mode-badge.mode-home,
    .site-header.site-header-space-home .mode-badge.mode-mobile {
        background: #ffffff;
        border-color: rgba(255,255,255,.35);
        color: #176a4a;
        font-weight: 800;
        box-shadow: 0 8px 18px rgba(12, 46, 30, .12);
    }

    .site-header.site-header-space-home .home-top-circle-btn {
        border-color: rgba(15, 23, 42, .08);
        background: #ffffff;
        box-shadow: 0 10px 22px rgba(12, 46, 30, .12);
    }

    .site-header.site-header-space-home .home-top-circle-btn:hover {
        border-color: rgba(22, 106, 74, .18);
        background: #f8fffb;
    }

    .site-header.site-header-space-home .home-account-toggle {
        display: inline-flex;
        align-items: center;
        gap: 10px;
        min-height: 50px;
        padding: 6px 14px 6px 6px;
        border-radius: 999px;
        border: 1px solid rgba(15, 23, 42, .08);
        background: #ffffff;
        color: #102a43;
        text-decoration: none;
        box-shadow: 0 12px 26px rgba(12, 46, 30, .14);
    }

    .site-header.site-header-space-home .home-account-toggle:hover {
        background: #f8fffb;
        border-color: rgba(22, 106, 74, .16);
    }

    .site-header.site-header-space-home .home-account-avatar {
        width: 38px;
        height: 38px;
        min-width: 38px;
        min-height: 38px;
        border-radius: 50%;
        border: 1px solid #dbe6ef;
        background-color: #f8fafc;
        background-size: cover;
        background-position: center;
        background-repeat: no-repeat;
        display: inline-block;
        flex: 0 0 38px;
    }

    .site-header.site-header-space-home .home-account-name {
        font-size: 15px;
        font-weight: 800;
        line-height: 1.2;
        color: #102a43;
    }

    .site-header.site-header-space-home .home-account-caret {
        color: #64748b;
        font-size: 18px;
    }

    .site-header.site-header-space-home .mobile-auth-link.login {
        background: #ffffff;
        color: #176a4a;
        border: 1px solid rgba(15, 23, 42, .08);
        box-shadow: 0 10px 22px rgba(12, 46, 30, .12);
    }

    .site-header.site-header-space-home .mobile-auth-link.signup {
        background: #e8f8ef;
        color: #176a4a;
        border: 1px solid rgba(23, 106, 74, .18);
        box-shadow: 0 10px 22px rgba(12, 46, 30, .08);
    }

    .site-header.site-header-shop,
    .site-header.site-header-space-shop {
        background: linear-gradient(90deg, #ee4d2d 0%, #ff7a45 100%);
    }

    .site-header.site-header-space-admin {
        background: linear-gradient(90deg, #b91c1c 0%, #dc2626 60%, #ef4444 100%);
    }

    .site-header.site-header-space-gianhang {
        background: linear-gradient(90deg, #f97316 0%, #fb923c 60%, #fdba74 100%);
    }

    .site-header.site-header-space-gianhang-admin {
        background: linear-gradient(90deg, #3730a3 0%, #4f46e5 60%, #818cf8 100%);
    }

    .site-header.site-header-space-daugia {
        background: linear-gradient(90deg, #be185d 0%, #db2777 55%, #f472b6 100%);
    }

    .site-header.site-header-space-event {
        background: linear-gradient(90deg, #0e7490 0%, #0891b2 60%, #22d3ee 100%);
    }

    .notif-kebab {
        position: absolute;
        right: 12px;
        bottom: 12px;
    }

    .site-header .container-fluid {
        min-height: var(--aha-topbar-height);
    }

    .site-header .mobile-menu-btn {
        display: inline-flex !important;
        align-items: center;
        justify-content: center;
        flex: 0 0 auto;
        margin-right: 8px;
        z-index: 3;
    }

    @media (min-width: 992px) {
        .site-header .mobile-menu-btn {
            display: none !important;
        }

        .site-header .brand-center-mobile {
            display: none !important;
        }

        .site-header #navbar-menu {
            width: 100%;
            display: grid !important;
            grid-template-columns: minmax(0, 1fr) auto minmax(0, 1fr);
            align-items: center;
            column-gap: 16px;
        }

        .site-header .header-left-nav {
            justify-self: start;
            min-width: 0;
        }

        .site-header .header-right-nav {
            justify-self: end;
            min-width: 0;
        }

        .site-header .brand-center-desktop {
            justify-self: center;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: auto;
            height: var(--aha-topbar-height);
            min-width: 0;
            min-height: var(--aha-topbar-height);
            text-decoration: none;
            line-height: 0;
        }

        .site-header .brand-center-desktop img {
            height: var(--aha-topbar-height) !important;
            width: auto !important;
            max-width: 240px;
            object-fit: contain;
        }
    }

    @media (max-width: 991.98px) {
        .site-header .mobile-menu-btn {
            margin-left: 0;
            margin-right: 6px;
        }

        .site-header .d-lg-none.ms-auto {
            display: none !important;
        }

        .home-mobile-search-host {
            display: none !important;
        }

        .site-header .brand-center-mobile {
            display: inline-flex !important;
            position: absolute;
            left: 50%;
            top: 50%;
            transform: translate(-50%, -50%);
            width: auto;
            height: var(--aha-topbar-height);
            min-width: 0;
            min-height: var(--aha-topbar-height);
            margin: 0;
            padding: 0;
            align-items: center;
            justify-content: center;
            max-width: calc(100vw - 160px);
            line-height: 0;
        }

        .mobile-left-icons {
            display: flex;
            align-items: center;
            gap: 6px;
            margin-left: 6px;
            z-index: 2;
        }

        .mobile-right-account {
            display: flex;
            align-items: center;
            margin-left: auto;
            z-index: 2;
            gap: 6px;
            flex-wrap: nowrap;
        }

        .site-header .mode-badge.mode-mobile {
            display: inline-flex !important;
            font-size: 10px;
            padding: 4px 8px;
        }
    }

    @media (max-width: 575.98px) {
        .site-header .container-fluid {
            padding-left: 6px;
            padding-right: 6px;
            min-height: 54px;
        }

        .site-header.site-header-space-home {
            background: linear-gradient(90deg, #176a4a 0%, #1f7a52 100%) !important;
            border-bottom: 0;
        }

        .site-header.site-header-space-home .home-top-circle-btn {
            border-color: rgba(0, 0, 0, .06);
            background: #ffffff;
        }

        .site-header .brand-center-mobile {
            display: inline-flex !important;
            position: absolute;
            left: 50%;
            top: 50%;
            transform: translate(-50%, -50%);
            width: auto;
            height: var(--aha-topbar-height);
            min-width: 0;
            min-height: var(--aha-topbar-height);
            margin: 0;
            padding: 0;
            align-items: center;
            justify-content: center;
            max-width: calc(100vw - 148px);
        }

        .site-header .mobile-hide-ct {
            display: none !important;
        }

        .home-mobile-search-host {
            display: none !important;
        }

        .site-header .mode-badge.mode-mobile {
            display: inline-flex !important;
            font-size: 10px;
            padding: 4px 8px;
        }

        .site-header .d-lg-none.ms-auto {
            display: none !important;
        }

        .mobile-left-icons {
            display: flex;
            align-items: center;
            gap: 6px;
            margin-left: 6px;
            z-index: 2;
        }

        .mobile-right-account {
            display: flex;
            align-items: center;
            margin-left: auto;
            z-index: 2;
            gap: 6px;
            flex-wrap: nowrap;
        }

        .site-header .d-lg-none .mobile-icon-heart,
        .site-header .d-lg-none .mobile-icon-account,
        .site-header .d-lg-none .mobile-icon-bell {
            display: inline-flex !important;
        }

        .site-header .d-lg-none .mobile-icon-bell,
        .site-header .d-lg-none .mobile-icon-cart,
        .site-header .d-lg-none .mobile-icon-heart,
        .site-header .d-lg-none .mobile-icon-account {
            order: initial;
        }

        .site-header .brand-center-mobile img {
            height: var(--aha-topbar-height) !important;
            width: auto !important;
            max-width: calc(100vw - 148px);
            object-fit: contain;
            background: transparent !important;
        }

        .site-header .d-lg-none.ms-auto.d-flex {
            display: none !important;
        }

        .site-header .d-lg-none.ms-auto.d-flex .btn,
        .site-header .d-lg-none.ms-auto.d-flex .home-top-circle-btn {
            width: 32px;
            height: 32px;
            min-width: 32px;
            min-height: 32px;
            padding: 0;
        }

        .site-header .d-lg-none a[title="Tin đã lưu"] {
            display: none !important;
        }

        .site-header .d-lg-none.ms-auto.d-flex .btn .ti,
        .site-header .d-lg-none.ms-auto.d-flex .home-top-circle-btn .ti {
            font-size: 17px !important;
        }
    }

    @media (max-width: 389.98px) {
        .mobile-left-icons .mode-badge.mode-mobile {
            max-width: 108px;
            font-size: 9px;
            padding: 4px 6px;
        }

        .mobile-auth-link {
            min-height: 30px;
            padding: 0 10px;
            font-size: 11px;
        }
    }

    .site-header .brand-center-logo-link {
        text-decoration: none !important;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: auto;
        height: var(--aha-topbar-height);
        min-width: 0;
        min-height: var(--aha-topbar-height);
        background: transparent;
        line-height: 0;
    }

    .site-header .brand-center-logo-link .brand-center-logo-img {
        width: auto !important;
        height: var(--aha-topbar-height) !important;
        max-width: min(240px, var(--aha-logo-available-width));
        object-fit: contain;
        object-position: center;
        display: block;
        background: transparent !important;
    }

    .site-header .brand-center-logo-link .brand-center-logo-img.logo-fit-by-width {
        width: min(100%, var(--aha-logo-available-width)) !important;
        height: auto !important;
        max-height: var(--aha-topbar-height) !important;
    }

    @media (max-width: 991.98px) {
        .site-header .brand-center-logo-link .brand-center-logo-img {
            height: var(--aha-topbar-height) !important;
            width: auto !important;
            max-width: min(220px, var(--aha-logo-available-width));
        }
    }

    @media (max-width: 575.98px) {
        .site-header .brand-center-logo-link .brand-center-logo-img {
            height: var(--aha-topbar-height) !important;
            width: auto !important;
            max-width: min(200px, var(--aha-logo-available-width));
        }
    }

    .site-header {
        color: #ffffff;
        backdrop-filter: blur(14px);
    }

    .site-header .container-fluid {
        min-height: var(--aha-topbar-height);
        gap: 10px;
    }

    .site-header .nav-link,
    .site-header .navbar-brand,
    .site-header .navbar-brand:hover,
    .site-header .fw-semibold,
    .site-header .text-secondary,
    .site-header .text-muted {
        color: #ffffff !important;
    }

    .site-header .mode-badge {
        background: rgba(255, 255, 255, 0.16);
        border-color: rgba(255, 255, 255, 0.28);
        color: #ffffff;
        box-shadow: inset 0 1px 0 rgba(255,255,255,0.12);
    }

    .site-header .topbar-space-toggle,
    .site-header .home-top-circle-btn,
    .site-header .topbar-auth-btn,
    .site-header .home-account-toggle {
        border: 1px solid rgba(255, 255, 255, 0.22) !important;
        box-shadow: 0 12px 28px rgba(15, 23, 42, 0.16);
    }

    .site-header .topbar-space-toggle,
    .site-header .home-top-circle-btn {
        background: rgba(255, 255, 255, 0.96) !important;
        color: #102a43 !important;
    }

    .site-header .topbar-space-toggle:hover,
    .site-header .home-top-circle-btn:hover {
        background: #ffffff !important;
        border-color: rgba(255, 255, 255, 0.34) !important;
        transform: translateY(-1px);
    }

    .site-header .topbar-space-toggle,
    .site-header .topbar-space-toggle * {
        color: #102a43 !important;
    }

    .site-header .home-account-toggle {
        min-height: 46px;
        padding: 4px 12px 4px 4px;
        border-radius: 999px;
        background: rgba(255, 255, 255, 0.96);
        color: #102a43 !important;
        gap: 10px;
        text-decoration: none;
        transition: transform .2s ease, box-shadow .2s ease, background-color .2s ease;
    }

    .site-header .home-account-toggle:hover {
        background: #ffffff;
        color: #102a43 !important;
        transform: translateY(-1px);
    }

    .site-header .home-account-avatar {
        width: 38px !important;
        height: 38px !important;
        min-width: 38px !important;
        min-height: 38px !important;
        max-width: 38px !important;
        max-height: 38px !important;
        border: 2px solid rgba(255, 255, 255, 0.9);
        box-shadow: 0 8px 18px rgba(15, 23, 42, 0.18);
    }

    .site-header .home-account-name,
    .site-header .home-account-caret {
        color: #102a43 !important;
    }

    .site-header .home-account-name {
        max-width: 180px;
        font-weight: 800;
    }

    .site-header .home-account-caret {
        opacity: .76;
    }

    .site-header .topbar-auth-btn {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-height: 44px;
        padding: 0 16px;
        border-radius: 999px;
        font-size: 13px;
        font-weight: 800;
        text-decoration: none;
        transition: transform .2s ease, opacity .2s ease, background-color .2s ease;
    }

    .site-header .topbar-auth-btn:hover {
        transform: translateY(-1px);
    }

    .site-header .topbar-auth-login {
        background: #0f172a;
        color: #ffffff !important;
    }

    .site-header .topbar-auth-signup {
        background: rgba(255, 255, 255, 0.94);
        color: #9f1239 !important;
    }

    .site-header .aha-header-quick-shell {
        position: relative;
    }

    .site-header .aha-header-quick-btn {
        position: relative;
    }

    .site-header .aha-header-quick-shell.is-open .aha-header-quick-btn {
        background: #ffffff !important;
        border-color: rgba(255, 255, 255, 0.34) !important;
    }

    .site-header .aha-header-quick-menu {
        position: absolute;
        top: calc(100% + 12px);
        right: 0;
        width: min(360px, calc(100vw - 24px));
        display: none;
        flex-direction: column;
        gap: 8px;
        padding: 12px;
        border-radius: 22px;
        background: rgba(255, 255, 255, 0.98);
        border: 1px solid rgba(226, 232, 240, 0.94);
        box-shadow: 0 24px 60px rgba(15, 23, 42, 0.22);
        z-index: 1060;
    }

    .site-header .aha-header-quick-shell.is-open .aha-header-quick-menu {
        display: flex;
    }

    .site-header .aha-header-quick-title {
        margin: 0 2px 4px;
        font-size: 12px;
        font-weight: 800;
        letter-spacing: .06em;
        text-transform: uppercase;
        color: #64748b;
    }

    .site-header .aha-header-quick-links {
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .site-header .aha-header-quick-link {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        padding: 12px 14px;
        border-radius: 18px;
        text-decoration: none;
        background: linear-gradient(135deg, rgba(255,255,255,0.98), rgba(248,250,252,0.96));
        border: 1px solid rgba(148, 163, 184, 0.18);
        color: #0f2940 !important;
        transition: transform .2s ease, border-color .2s ease, box-shadow .2s ease;
    }

    .site-header .aha-header-quick-link:hover {
        transform: translateY(-1px);
        border-color: rgba(148, 163, 184, 0.3);
        box-shadow: 0 16px 36px rgba(15, 23, 42, 0.12);
    }

    .site-header .aha-header-quick-copy {
        display: flex;
        flex-direction: column;
        min-width: 0;
    }

    .site-header .aha-header-quick-copy strong {
        font-size: 14px;
        font-weight: 800;
        color: #102a43;
        line-height: 1.3;
    }

    .site-header .aha-header-quick-copy small {
        margin-top: 2px;
        font-size: 11px;
        color: #64748b;
        line-height: 1.35;
    }

    .site-header .aha-header-quick-badge {
        flex: 0 0 auto;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 6px 10px;
        border-radius: 999px;
        background: rgba(15, 23, 42, 0.08);
        color: #102a43;
        font-size: 11px;
        font-weight: 800;
        white-space: nowrap;
    }

    .site-header .brand-center-desktop,
    .site-header .brand-center-mobile {
        filter: drop-shadow(0 10px 22px rgba(15, 23, 42, 0.14));
    }

    .site-header .mobile-right-account {
        gap: 8px;
    }

    .site-header .mobile-auth-link.login {
        background: #0f172a;
        color: #ffffff !important;
        border: 1px solid rgba(255,255,255,0.18);
    }

    .site-header .mobile-auth-link.signup {
        background: rgba(255,255,255,0.94);
        color: #9f1239 !important;
        border: 1px solid rgba(255,255,255,0.2);
    }

    @media (max-width: 991.98px) {
        .site-header .container-fluid {
            min-height: 56px;
        }

        .site-header .aha-header-quick-menu {
            display: none !important;
        }
    }
</style>
<header class="navbar navbar-expand-lg fixed-top d-print-none site-header <%= ResolveHeaderSpaceCssClass() %>"
    style="<%= ResolveHeaderInlineStyle() %>">
    <div class="container-fluid position-relative">

        <!-- Hamburger menu -->
        <button class="btn btn-icon home-top-circle-btn bg-white mobile-menu-btn d-lg-none" type="button"
            data-bs-toggle="offcanvas" data-bs-target="#mobileMenu"
            aria-controls="mobileMenu" aria-label="Menu">
            <i class="ti ti-menu-2"></i>
        </button>

        <div class="d-lg-none mobile-left-icons">
            <asp:PlaceHolder ID="phModeBadgeMobile" runat="server" Visible="false">
                <asp:Label ID="lb_mode_badge_mobile" runat="server" CssClass="mode-badge mode-mobile mode-home"></asp:Label>
            </asp:PlaceHolder>
        </div>

        <div id="homeMobileSearchHost" class="d-lg-none home-mobile-search-host" data-use-search-host="0"></div>

        <!-- BRAND MOBILE -->
        <a class="navbar-brand fw-bold d-flex align-items-center brand-center-mobile d-lg-none brand-center-logo-link"
            href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/" %>">
            <img src="<%= ResolveHeaderCenterLogoUrl() %>" class="brand-center-logo-img" alt="AhaSale" />
        </a>

        <!-- DESKTOP NAV -->
        <div class="collapse navbar-collapse d-none d-lg-flex" id="navbar-menu">
            <ul class="navbar-nav align-items-lg-center gap-lg-2 header-left-nav">
                <li class="nav-item d-none d-lg-flex align-items-center">
                    <uc1:SpaceLauncher runat="server" ID="spaceLauncher" ButtonCssClass="home-top-circle-btn topbar-space-toggle" />
                </li>
                <asp:PlaceHolder ID="phDesktopSpaceBadge" runat="server" Visible="true">
                    <li class="nav-item">
                        <asp:Label ID="lb_desktop_space_badge" runat="server" CssClass="mode-badge mode-home"></asp:Label>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phDonBan" runat="server" Visible="false"></asp:PlaceHolder>
               <%-- <li class="nav-item"><a class="nav-link fw-semibold" href="#">Bất động sản</a></li>
                <li class="nav-item"><a class="nav-link fw-semibold" href="#">Việc làm</a></li>--%>
            </ul>

            <a class="brand-center-desktop brand-center-logo-link"
                href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/" %>">
                <img src="<%= ResolveHeaderCenterLogoUrl() %>" class="brand-center-logo-img" alt="AhaSale" />
            </a>

            <!-- DESKTOP RIGHT -->
            <div class="navbar-nav flex-row align-items-center gap-2 header-right-nav">
                <asp:PlaceHolder ID="phModeBadge" runat="server" Visible="false">
                    <asp:Label ID="lb_mode_badge" runat="server" CssClass="mode-badge mode-home"></asp:Label>
                </asp:PlaceHolder>
                <% if (ShowHeaderQuickActions()) { %>
                <div class="position-relative d-none d-lg-block aha-header-quick-shell" id="headerQuickShell">
                    <button type="button" class="home-top-circle-btn aha-header-quick-btn" id="headerQuickToggle" aria-haspopup="true" aria-expanded="false" title="<%: ResolveHeaderQuickActionsTitle() %>">
                        <i class="ti ti-plus"></i>
                    </button>
                    <div class="aha-header-quick-menu" id="headerQuickMenu">
                        <div class="aha-header-quick-title"><%: ResolveHeaderQuickActionsTitle() %></div>
                        <div class="aha-header-quick-links">
                            <%= RenderHeaderQuickActionsHtml() %>
                        </div>
                    </div>
                </div>
                <% } %>
                <asp:PlaceHolder ID="phTopDesktopHomeUtilities" runat="server" Visible="false">
                    <div class="position-relative d-none d-lg-block">
                        <a href="/home/quan-ly-tin/tin-da-luu.aspx"
                            class="home-top-circle-btn"
                            title="Tin đã lưu">
                            <i class="ti ti-heart"></i>
                        </a>
                    </div>
                    <div class="position-relative d-none d-lg-block">
                        <a href="/home/gio-hang.aspx"
                            class="home-top-circle-btn position-relative"
                            title="Giỏ hàng">
                            <i class="ti ti-shopping-cart"></i>
                            <span id="badgeGioHangDesktop" runat="server" class="badge position-absolute top-0 end-0"
                                style="background: #FFF3CD; color: #856404; font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                <asp:Label ID="lb_sl_giohang_desktop" runat="server" Text="0"></asp:Label>
                            </span>
                        </a>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phTopNotificationDesktop" runat="server" Visible="false">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="nav-item position-relative">
                                <asp:LinkButton ID="but_show_form_thongbao"
                                    runat="server"
                                    CssClass="home-top-circle-btn position-relative"
                                    OnClick="but_show_form_thongbao_Click"
                                    OnClientClick="showNotif();">
                                    <i class="ti ti-bell"></i>
                                    <span id="badgeThongBaoDesktop" runat="server" class="badge bg-red position-absolute top-0 end-0"
                                        style="font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                        <asp:Label ID="lb_sl_thongbao_desktop" runat="server" Text="0"></asp:Label>
                                    </span>
                                </asp:LinkButton>
                                <div class="dropdown-menu dropdown-menu-end p-0 notif-dropdown"
                                    id="notifHostDesktop"
                                    style="width: 380px; border-radius: 16px;">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phTopNotificationDesktopShop" runat="server" Visible="false">
                    <asp:UpdatePanel ID="UpdatePanel1Shop" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="nav-item position-relative">
                                <asp:LinkButton ID="but_show_form_thongbao_shop"
                                    runat="server"
                                    CssClass="home-top-circle-btn position-relative"
                                    OnClick="but_show_form_thongbao_Click"
                                    OnClientClick="showNotif();">
                                    <i class="ti ti-bell"></i>
                                    <span id="badgeThongBaoDesktopShop" runat="server" class="badge bg-red position-absolute top-0 end-0"
                                        style="font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                        <asp:Label ID="lb_sl_thongbao_desktop_shop" runat="server" Text="0"></asp:Label>
                                    </span>
                                </asp:LinkButton>
                                <div class="dropdown-menu dropdown-menu-end p-0 notif-dropdown"
                                    id="notifHostDesktopShop"
                                    style="width: 380px; border-radius: 16px;">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                    <% if (!PortalRequest_cl.IsShopPortalRequest() && string.IsNullOrEmpty(PortalRequest_cl.GetCurrentAccountEncrypted())) { %>
                    <a href="/dang-nhap" class="topbar-auth-btn topbar-auth-login">Đăng nhập</a>
                    <a href="/home/dangky.aspx"
                        class="topbar-auth-btn topbar-auth-signup">Đăng ký
                    </a>
                    <% } %>
                </asp:PlaceHolder>

                <!-- USER DROPDOWN -->
                <asp:PlaceHolder ID="phTopDesktopAccount" runat="server" Visible="true">
                    <% if (!string.IsNullOrEmpty(PortalRequest_cl.GetCurrentAccountEncrypted())) { %>
                    <div class="nav-item aha-account">
                        <a id="userMenuToggle" href="#" class="home-account-toggle"
                            aria-expanded="false">
                            <span class="home-account-avatar" style="background-image: url('<%= ViewState["anhdaidien"] %>')"></span>
                            <span class="home-account-name"><%: ResolveTopAccountLabel() %></span>
                            <i class="ti ti-chevron-down home-account-caret"></i>
                        </a>

                        <div class="account-dropdown account-menu-host"
                            id="accountMenuHostDesktop"
                            style="width: 360px; border-radius: 16px;">
                        </div>
                    </div>
                    <% } %>
                </asp:PlaceHolder>
            </div>
        </div>

        <!-- MOBILE RIGHT: account only -->
        <div class="d-lg-none mobile-right-account">
            <asp:PlaceHolder ID="phTopMobileGuestAuth" runat="server" Visible="false">
                <a href="/dang-nhap" class="mobile-auth-link login">Đăng nhập</a>
                <a href="/home/dangky.aspx" class="mobile-auth-link signup">Đăng ký</a>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phTopMobileFavorite" runat="server" Visible="true">
                <a href="/home/quan-ly-tin/tin-da-luu.aspx"
                    class="btn btn-icon home-top-circle-btn bg-white mobile-icon-heart"
                    title="Tin đã lưu">
                    <i class="ti ti-heart text-success"></i>
                </a>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phTopMobileHomeUtilities" runat="server" Visible="true">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <a class="btn btn-icon home-top-circle-btn bg-white position-relative mobile-icon-cart"
                            href="/home/gio-hang.aspx"
                            title="Giỏ hàng">
                            <i class="ti ti-shopping-cart text-success"></i>
                            <span id="badgeGioHangMobile" runat="server" class="position-absolute top-0 start-100 translate-middle badge rounded-pill"
                                style="background: #FFF3CD; color: #856404; font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                <asp:Label ID="lb_sl_giohang_mobile" runat="server" Text="0"></asp:Label>
                            </span>
                        </a>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phTopNotificationMobile" runat="server" Visible="true">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:LinkButton ID="LinkButton1"
                            runat="server"
                            CssClass="btn btn-icon home-top-circle-btn bg-white position-relative mobile-icon-bell"
                            OnClick="but_show_form_thongbao_Click">
                            <i class="ti ti-bell"></i>
                            <span id="badgeThongBaoMobile" runat="server" class="badge bg-red position-absolute top-0 end-0"
                                style="font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                <asp:Label ID="lb_sl_thongbao_mobile" runat="server" Text="0"></asp:Label>
                            </span>
                        </asp:LinkButton>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:PlaceHolder>
            <% if (ShowHeaderQuickActions()) { %>
            <button type="button"
                class="btn btn-icon home-top-circle-btn bg-white mobile-icon-quick"
                data-bs-toggle="offcanvas"
                data-bs-target="#quickActionsCanvas"
                aria-controls="quickActionsCanvas"
                aria-label="<%: ResolveHeaderQuickActionsTitle() %>">
                <i class="ti ti-plus"></i>
            </button>
            <% } %>
            <asp:PlaceHolder ID="phTopMobileAccount" runat="server" Visible="true">
                <button type="button"
                    class="btn btn-icon home-top-circle-btn bg-white position-relative p-0 mobile-icon-account"
                    data-bs-toggle="offcanvas"
                    data-bs-target="#accountMenuCanvas"
                    aria-controls="accountMenuCanvas"
                    aria-label="Tài khoản"
                    style="overflow: hidden;">
                    <span class="d-block w-100 h-100"
                        style="background-image: url('<%= ViewState["anhdaidien"] %>'); background-size: cover; background-position: center; border-radius: 50%;"></span>
                </button>
            </asp:PlaceHolder>
        </div>

    </div>
</header>

<!-- MOBILE MAIN MENU OFFCANVAS -->
<div class="offcanvas offcanvas-start" tabindex="-1" id="mobileMenu" aria-labelledby="mobileMenuLabel">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="mobileMenuLabel">Menu</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>

    <div class="offcanvas-body">
        <asp:PlaceHolder ID="phHomeSpaceAccessSummaryMobile" runat="server" Visible="false">
            <div class="home-space-access-card <%= ResolveHeaderSpaceCssClass() %> mt-1 mb-3">
                <div class="home-space-access-title">Không gian hoạt động:</div>
                <div class="home-space-access-list">
                    <asp:Literal ID="litHomeSpaceAccessSummaryMobile" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>

        <div class="list-group list-group-flush">
            <%= show_danhmuc_mobile %>
           <%-- <a href="#" class="list-group-item list-group-item-action fw-semibold">Bất động sản</a>
            <a href="#" class="list-group-item list-group-item-action fw-semibold">Việc làm</a>--%>
        </div>

        <div class="mt-3 d-flex gap-2">
            <asp:PlaceHolder ID="phDangNhap" runat="server">
                <a href="/dang-nhap" class="btn btn-outline-secondary flex-fill">Đăng nhập</a>
            </asp:PlaceHolder>
            <%--<a href="/home/quan-ly-tin/default.aspx" class="btn btn-primary flex-fill">Đăng tin</a>--%>
        </div>
    </div>
</div>


<!-- MOBILE ACCOUNT OFFCANVAS -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="accountMenuCanvas" aria-labelledby="accountMenuCanvasLabel">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="accountMenuCanvasLabel">Tài khoản</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>

    <div class="offcanvas-body p-3">
        <div id="accountMenuHostMobile" class="account-dropdown"></div>
    </div>
</div>

<% if (ShowHeaderQuickActions()) { %>
<div class="offcanvas offcanvas-end" tabindex="-1" id="quickActionsCanvas" aria-labelledby="quickActionsCanvasLabel">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="quickActionsCanvasLabel"><%: ResolveHeaderQuickActionsTitle() %></h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body p-3">
        <div class="aha-header-quick-links">
            <%= RenderHeaderQuickActionsHtml() %>
        </div>
    </div>
</div>
<% } %>



<!-- ✅ TEMPLATE THÔNG BÁO (NEW) -->
<div id="notifPopup"
    style="display: none; position: fixed; right: 0; top: 45px; width: 380px; background: #fff; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,.15); z-index: 9999;">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="notif-head p-3">
                <div class="d-flex align-items-center justify-content-between">
                    <div class="fw-bold" style="font-size: 1.05rem;">Thông báo</div>

                    <div class="d-flex gap-2">
                        <a href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/home/quan-ly-tin/default.aspx" %>" class="btn btn-sm btn-success" style="border-radius: 10px;">Xem tất cả
                        </a>
                    </div>
                </div>

                <div class="mt-3 d-flex gap-2">
                    <asp:Button ID="but_sapxep_moinhat_desk" OnClick="but_sapxep_moinhat_Click" type="button" runat="server" class="btn btn-sm btn-outline-secondary active" Style="border-radius: 999px;" Text="Tất cả" />
                    <asp:Button ID="but_sapxep_chuadoc_desk" OnClick="but_sapxep_chuadoc_Click" runat="server" Text="Chưa đọc" CssClass="btn btn-sm btn-outline-secondary" Style="border-radius: 999px;" />
                </div>
            </div>
            <div style="max-height: 75vh; overflow-y: auto;">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>

                        <div class="list-group list-group-flush">
                            <div class="list-group-item notif-item position-relative">

                                <!-- LINK nội dung -->
                                <a href="<%# Eval("link") %>idtb=<%# Eval("id") %>"
                                    class="list-group-item-action d-block text-decoration-none">

                                    <div class="d-flex align-items-start gap-3">
                                        <img class="avatar avatar-sm rounded"
                                            src="<%# Eval("avt_nguoithongbao") %>" />

                                        <div class="flex-fill">
                                            <!-- nội dung -->
                                            <div class="d-flex align-items-center gap-2">
                                                <asp:PlaceHolder runat="server"
                                                    Visible='<%# Eval("daxem").ToString() == "True" %>'>
                                                    <div>
                                                        <%# Eval("noidung") %>
                                                    </div>
                                                </asp:PlaceHolder>

                                                <asp:PlaceHolder runat="server"
                                                    Visible='<%# Eval("daxem").ToString() == "False" %>'>
                                                    <div class="fw-bold text-warning">
                                                        <%# Eval("noidung") %>
                                                    </div>
                                                    <span class="notif-dot" title="Chưa đọc"></span>
                                                </asp:PlaceHolder>
                                            </div>

                                            <!-- thời gian -->
                                            <div class="notif-time text-muted mt-1">
                                                <i class="ti ti-clock"></i>
                                                <%# Eval("thoigian", "{0:dd/MM/yyyy HH:mm}") %>
                                            </div>
                                        </div>
                                    </div>
                                </a>

                                <!-- DROPDOWN -->
                                <div class="dropdown notif-kebab">
                                    <button type="button" style="border: none; background: none; box-shadow: none;"
                                        class="btn text-muted notif-kebab-btn"
                                        data-bs-toggle="dropdown"
                                        aria-expanded="false">
                                        ...
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <asp:PlaceHolder runat="server"
                                            Visible='<%# Eval("daxem").ToString()=="False" %>'>
                                            <li>
                                                <asp:LinkButton runat="server"
                                                    CssClass="dropdown-item"
                                                    CommandArgument='<%# Eval("id") %>'
                                                    OnClick="but_dadoc_Click">
                                                    Đánh dấu đã đọc
                                                </asp:LinkButton>
                                            </li>
                                        </asp:PlaceHolder>

                                        <asp:PlaceHolder runat="server"
                                            Visible='<%# Eval("daxem").ToString()=="True" %>'>
                                            <li>
                                                <asp:LinkButton runat="server"
                                                    CssClass="dropdown-item"
                                                    CommandArgument='<%# Eval("id") %>'
                                                    OnClick="but_chuadoc_Click">
                                                    Đánh dấu chưa đọc
                                                </asp:LinkButton>
                                            </li>
                                        </asp:PlaceHolder>

                                        <li>
                                            <asp:LinkButton runat="server"
                                                CssClass="dropdown-item"
                                                CommandArgument='<%# Eval("id") %>'
                                                OnClick="but_xoathongbao_Click">
                                                Xóa thông báo
                                            </asp:LinkButton>
                                        </li>
                                    </ul>
                                </div>

                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="ph_empty_thongbao" runat="server" Visible="false">
                    <div class="p-3 text-muted">Không có thông báo nào.</div>
                </asp:PlaceHolder>

            </div>
            <div class="notif-foot p-3">
                <a href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/home/quan-ly-tin/default.aspx" %>" class="btn btn-outline-secondary w-100" style="border-radius: 12px;">Quản lý thông báo
                </a>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

<!-- ✅ CHÈN MENU USER THẬT Ở ĐÂY -->
<div id="accountMenuContent" style="display: none;">
    <div class="account-menu-shell">
        <div class="account-menu-body">
    <asp:UpdatePanel ID="UpdatePanelGuestCard" runat="server" Visible="false" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card border-0 shadow-sm mb-3" style="border-radius: 14px;">
                <div class="card-body p-3">
                    <div class="fw-bold" style="font-size: 1.05rem;">
                        Mua thì hời, bán thì lời.
                    </div>
                    <div class="text-secondary">
                        Đăng nhập cái đã!
                    </div>

                    <!-- 2 cột -->
                    <div class="row g-2 mt-3">
                        <div class="col-6">
                            <a href="/dang-nhap"
                                class="btn btn-success w-100"
                                style="border-radius: 12px;">Đăng nhập
                            </a>
                        </div>

                        <div class="col-6">
                            <a href="/home/dangky.aspx"
                                class="btn btn-outline-success w-100"
                                style="border-radius: 12px; border-color: #35b36a; color: #2f9e5f;">Đăng ký
                            </a>
                        </div>
                    </div>
                    <asp:PlaceHolder ID="phGuestSwitchHome" runat="server" Visible="false">
                        <div class="mt-2">
                            <a href="/dang-nhap?switch=home"
                                class="btn btn-warning w-100"
                                style="border-radius: 12px;">Chuyển sang tài khoản cá nhân</a>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ================= MENU CŨ ================= -->
    <!-- User header -->
    <asp:UpdatePanel ID="UpdatePanelUser" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderLogged" runat="server" Visible="false">
                <div class="card border-0 bg-light mb-3" style="border-radius: 14px; overflow: hidden;">
                    <div class="p-3">
                        <div class="d-flex align-items-center gap-3">
                            <span class="avatar avatar-lg rounded-circle home-dropdown-avatar"
                                style="background-image: url('<%= ViewState["anhdaidien"] %>')"></span>

                            <div class="flex-fill">
                                <div class="fw-bold" style="font-size: 1.05rem;">
                                    <%= ResolveDropdownProfileName() %>
                                </div>
                                <div class="text-secondary small">
                                    <%= ViewState["taikhoan"] %>
                                </div>
                                <div class="mt-1">
                                    <%= ViewState["phanloai"] %>
                                </div>
                            </div>

                            <a href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/cap-nhat-ho-so" : "/home/edit-info.aspx" %>"
                                class="btn btn-sm btn-outline-secondary"
                                style="border-radius: 10px;">
                                <i class="ti ti-pencil"></i>
                            </a>
                        </div>

                        <!-- ✅ 2 hồ sơ shop (chỉ hiện khi là gian hàng đối tác) -->
                        <asp:PlaceHolder ID="phHoSoShopOnly" runat="server" Visible="false">
                            <div class="d-flex flex-column gap-2 mt-3">
                                <!-- 5) Hồ sơ tiêu dùng shop -->
                                <a href="/shop/ho-so-tieu-dung"
                                    class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                                    style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                                    <span class="avatar avatar-xs rounded-circle bg-white border d-flex align-items-center justify-content-center"
                                        style="width: 22px; height: 22px;">
                                        <img src="/uploads/images/dong-a.png" alt="Hồ sơ tiêu dùng gian hàng đối tác"
                                            style="width: 14px; height: 14px; object-fit: contain;">
                                    </span>

                                    <span class="flex-fill" style="min-width: 0;">
                                        <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ tiêu dùng gian hàng đối tác
                                        </span>
                                        <span class="d-block fw-semibold" style="line-height: 1.2;">
                                            <%= ViewState["HoSo_TieuDung_ShopOnly"] %>
                                        </span>
                                    </span>

                                    <i class="ti ti-chevron-right text-muted"></i>
                                </a>

                                <!-- 6) Hồ sơ ưu đãi shop -->
                                <a href="/shop/ho-so-uu-dai"
                                    class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                                    style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                                    <span class="avatar avatar-xs rounded-circle bg-success-lt text-success d-flex align-items-center justify-content-center"
                                        style="width: 22px; height: 22px;">
                                        <i class="ti ti-ticket"></i>
                                    </span>

                                    <span class="flex-fill" style="min-width: 0;">
                                        <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ ưu đãi gian hàng đối tác
                                        </span>
                                        <span class="d-block fw-semibold" style="line-height: 1.2;">
                                            <%= ViewState["HoSo_UuDai_ShopOnly"] %>
                                        </span>
                                    </span>

                                    <i class="ti ti-chevron-right text-muted"></i>
                                </a>
                            </div>
                        </asp:PlaceHolder>

                    </div>

                    <asp:PlaceHolder ID="phHomeSpaceAccessSummary" runat="server" Visible="false">
                        <div class="home-space-access-card <%= ResolveHeaderSpaceCssClass() %>">
                            <div class="home-space-access-title">Không gian hoạt động:</div>
                            <div class="home-space-access-list">
                                <asp:Literal ID="litHomeSpaceAccessSummary" runat="server" />
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phGianHangCompactMenu" runat="server" Visible="false">
                        <div class="gianhang-space-menu-card">
                            <div class="gianhang-space-menu-title">Không gian gian hàng</div>
                            <div class="list-group list-group-flush bg-white">
                                <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("trang-cong-khai") %>' href="/gianhang/">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-world text-secondary"></i>
                                        <span class="fw-medium">Trang công khai</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("quan-ly-tin") %>' href="/gianhang/quan-ly-tin/Default.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-news text-secondary"></i>
                                        <span class="fw-medium">Quản lý tin</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("don-ban") %>' href="/gianhang/don-ban.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-receipt text-secondary"></i>
                                        <span class="fw-medium">Đơn bán</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("lich-hen") %>' href="/gianhang/quan-ly-lich-hen.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-calendar-event text-secondary"></i>
                                        <span class="fw-medium">Lịch hẹn</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("khach-hang") %>' href="/gianhang/khach-hang.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-users text-secondary"></i>
                                        <span class="fw-medium">Khách hàng</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("bao-cao") %>' href="/gianhang/bao-cao.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-chart-bar text-secondary"></i>
                                        <span class="fw-medium">Báo cáo</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("nang-cap-level2") %>' href="/gianhang/nang-cap-level2.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-rocket text-secondary"></i>
                                        <span class="fw-medium">Nâng cấp Level 2</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <asp:PlaceHolder ID="phGianHangCompactAdmin" runat="server" Visible="false">
                                    <a class='list-group-item d-flex align-items-center justify-content-between py-3 gianhang-space-link <%= ResolveGianHangMenuItemClass("quan-tri") %>' href="/gianhang/admin/">
                                        <div class="d-flex align-items-center gap-2">
                                            <i class="ti ti-settings text-secondary"></i>
                                            <span class="fw-medium">Quản trị gian hàng</span>
                                        </div>
                                        <i class="ti ti-chevron-right text-secondary"></i>
                                    </a>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phDefaultAccountMenu" runat="server" Visible="true">
                    <!-- Menu list (giống vùng khoanh đỏ) -->
                    <div class="list-group list-group-flush bg-white" style="border-top: 1px solid rgba(0,0,0,.06);">

                        <!-- Hồ sơ -->
                        <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? (ViewState["public_profile_link"] ?? "/shop/default.aspx").ToString() : ("/" + (ViewState["taikhoan"] ?? "").ToString() + ".info") %>">
                            <div class="d-flex align-items-center gap-2">
                                <i class="ti ti-user-circle text-secondary"></i>
                                <span class="fw-medium">Trang cá nhân</span>
                            </div>
                            <i class="ti ti-chevron-right text-secondary"></i>
                        </a>
                        <asp:PlaceHolder ID="phMenuHomeYeuCau" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/tao-yeu-cau.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-file-plus text-secondary"></i>
                                    <span class="fw-medium">Tạo đề xuất</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeCopyLink" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3"
                               href="javascript:void(0);"
                               data-copy-ref="1"
                               data-ref-link="<%= System.Web.HttpUtility.HtmlAttributeEncode("https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + (ViewState["taikhoan"] ?? "").ToString()) %>"
                               onclick="return copyReferralLink(this);">
                                <span class="d-flex align-items-center gap-2">
                                    <i class="ti ti-link text-secondary"></i>
                                    <span class="fw-medium">Sao chép Link giới thiệu</span>
                                </span>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeDoiPin" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/DoiPin.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-key text-secondary"></i>
                                <span class="fw-medium">Đổi mã pin thẻ</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/doi-mat-khau" : "/home/DoiMatKhau.aspx" %>">
                            <div class="d-flex align-items-center gap-2">
                                <i class="ti ti-lock text-secondary"></i>
                            <span class="fw-medium">Đổi mật khẩu tài khoản</span>
                            </div>
                            <i class="ti ti-chevron-right text-secondary"></i>
                        </a>

                        <%-- Ẩn: Mã QR của tôi --%>

                        <asp:PlaceHolder ID="phMenuHomeKhachHang" runat="server" Visible="false"></asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeDonMua" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/don-mua.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-shopping-bag text-secondary"></i>
                                    <span class="fw-medium">Đơn mua</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeLichHen" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-hen.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-calendar-event text-secondary"></i>
                                    <span class="fw-medium">Lịch hẹn của tôi</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeLichSuTraoDoi" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-giao-dich.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-clock text-secondary"></i>
                                    <span class="fw-medium">Lịch sử Trao đổi</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuShopTinhNang" runat="server" Visible="false">
                            <div class="px-3 pt-2 pb-1 text-secondary small fw-semibold border-top">Tính năng gian hàng đối tác</div>

                            <div class="px-3 pt-2 pb-1 text-secondary small">Hồ sơ gian hàng đối tác</div>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/ho-so-tieu-dung">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-coin text-secondary"></i>
                                    <span class="fw-medium">Hồ sơ tiêu dùng gian hàng đối tác</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/ho-so-uu-dai">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-ticket text-secondary"></i>
                                    <span class="fw-medium">Hồ sơ ưu đãi gian hàng đối tác</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <div class="px-3 pt-2 pb-1 text-secondary small">Vận hành gian hàng đối tác</div>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/quan-ly-tin">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-news text-secondary"></i>
                                    <span class="fw-medium">Quản lý tin gian hàng đối tác</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/quan-ly-lich-hen.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-calendar-event text-secondary"></i>
                                    <span class="fw-medium">Lịch hẹn khách đặt</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/don-ban">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-receipt text-secondary"></i>
                                    <span class="fw-medium">Đơn bán</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/bao-cao.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-chart-bar text-secondary"></i>
                                    <span class="fw-medium">Báo cáo</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <asp:PlaceHolder ID="phMenuShopChoTraoDoi" runat="server" Visible="false">
                                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/cho-thanh-toan">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-credit-card text-secondary"></i>
                                        <span class="fw-medium">Chờ trao đổi</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                            </asp:PlaceHolder>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/khach-hang">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-users text-secondary"></i>
                                    <span class="fw-medium">Khách hàng</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <div class="px-3 pt-2 pb-1 text-secondary small">Bộ công cụ nâng cao</div>
                            <asp:PlaceHolder ID="phShopLevel2Tools" runat="server" Visible="false">
                                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/gianhang/admin">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-settings text-secondary"></i>
                                        <span class="fw-medium">Vào /gianhang/admin</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/nang-cap-level2.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-shield-check text-secondary"></i>
                                        <span class="fw-medium">Quản lý Level 2</span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phShopLevel2Upgrade" runat="server" Visible="false">
                                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/nang-cap-level2.aspx">
                                    <div class="d-flex align-items-center gap-2">
                                        <i class="ti ti-rocket text-secondary"></i>
                                        <span class="fw-medium"><asp:Literal ID="lb_shop_level2_cta" runat="server" Text="Yêu cầu nâng cấp Level 2"></asp:Literal></span>
                                    </div>
                                    <i class="ti ti-chevron-right text-secondary"></i>
                                </a>
                            </asp:PlaceHolder>

                            <div class="px-3 pt-2 pb-1 text-secondary small">Thiết lập gian hàng đối tác</div>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/cap-nhat-ho-so">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-pencil text-secondary"></i>
                                    <span class="fw-medium">Cập nhật hồ sơ</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phSwitchToShop" runat="server" Visible="false">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/login.aspx?switch=shop">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-building-store text-success"></i>
                                    <span class="fw-semibold text-success">Chuyển sang gian hàng đối tác</span>
                                </div>
                                <i class="ti ti-chevron-right text-success"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phSwitchToHome" runat="server" Visible="false">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/dang-nhap?switch=home">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-user-circle text-warning"></i>
                                    <span class="fw-semibold text-warning">Chuyển sang tài khoản cá nhân</span>
                                </div>
                                <i class="ti ti-chevron-right text-warning"></i>
                            </a>
                        </asp:PlaceHolder>

                    </div>
                    </asp:PlaceHolder>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ================= TIỆN ÍCH ================= -->
    <asp:PlaceHolder ID="phUtilityHome" runat="server" Visible="true">
        <div class="text-secondary small fw-semibold px-1 mb-2">Tiện ích</div>
        <div class="card border-0 bg-light mb-3" style="border-radius: 14px;">
            <div class="list-group list-group-flush" style="border-radius: 14px; overflow: hidden;">
                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/tin-da-luu.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-heart text-secondary"></i>
                        <span class="fw-medium">Tin đăng đã lưu</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/lich-su-xem-tin.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-history text-secondary"></i>
                        <span class="fw-medium">Lịch sử xem tin</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/danh-gia-tu-toi.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-star text-secondary"></i>
                        <span class="fw-medium">Đánh giá từ tôi</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>
            </div>
        </div>
    </asp:PlaceHolder>

    <!-- ================= DỊCH VỤ TRẢ PHÍ ================= -->
    <%-- <div class="text-secondary small fw-semibold px-1 mb-2">Gian hàng đối tác</div>
    <div class="card border-0 bg-light mb-3" style="border-radius: 14px;">
        <div class="list-group list-group-flush" style="border-radius: 14px; overflow: hidden;">
            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-giao-dich.aspx">
                <div class="d-flex align-items-center gap-2">
                    <span class="avatar avatar-xs rounded-circle bg-white border d-flex align-items-center justify-content-center">
                        <img src="/uploads/images/dong-a.png" alt="Quyền tiêu dùngha" style="width: 14px; height: 14px; object-fit: contain;">
                    </span>
                    <span class="fw-medium">Quyền tiêu dùngha</span>
                </div>
                <i class="ti ti-chevron-right text-secondary"></i>
            </a>

            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="#">
                <div class="d-flex align-items-center gap-2">
                    <span class="avatar avatar-xs rounded-circle bg-dark text-white d-flex align-items-center justify-content-center">
                        <span class="fw-bold" style="font-size: 10px;">PRO</span>
                    </span>
                    <span class="fw-medium">Gói PRO</span>
                </div>
                <i class="ti ti-chevron-right text-secondary"></i>
            </a>

            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/dang-ky-gian-hang-doi-tac.aspx?return_url=%2Fgianhang%2F">
                <div class="d-flex align-items-center gap-2">
                    <span class="avatar avatar-xs rounded-circle bg-orange-lt text-orange d-flex align-items-center justify-content-center">
                        <i class="ti ti-check"></i>
                    </span>
                    <span class="fw-medium">Mở gian hàng đối tác</span>
                </div>
                <i class="ti ti-chevron-right text-secondary"></i>
            </a>
       
 <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/default.aspx">
     <div class="d-flex align-items-center gap-2">
         <i class="ti ti-news text-success"></i>
         <span class="fw-semibold text-success">Quản lý tin</span>
     </div>
     <i class="ti ti-chevron-right text-success"></i>
 </a>
           
           <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/edit-info.aspx">
                <div class="d-flex align-items-center gap-2">
                    <i class="ti ti-building-store text-secondary"></i>
                    <span class="fw-medium">Cửa hàng</span>
                </div>
                <span class="badge bg-white text-dark border rounded-pill">Tạo ngay</span>
            </a>
</div>
</div>--%>

    <asp:PlaceHolder ID="phMenuHomeExtra" runat="server" Visible="true">
        <!-- ================= ƯU ĐÃI ================= -->
        <div class="text-secondary small fw-semibold px-1 mb-2">Ưu đãi, khuyến mãi</div>
        <div class="card border-0 bg-light mb-3" style="border-radius: 14px;">
            <div class="p-3">
                <div class="d-flex flex-column gap-2">
                    <asp:PlaceHolder ID="phHoSoHomeMacDinh" runat="server" Visible="false">
                        <!-- 1) Hồ sơ quyền tiêu dùng -->
                        <a href="/home/lich-su-giao-dich.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-white border d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <img src="/uploads/images/dong-a.png" alt="Quyền tiêu dùng"
                                    style="width: 14px; height: 14px; object-fit: contain;">
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ quyền tiêu dùng
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["DongA"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>

                        <!-- 2) Hồ sơ quyền ưu đãi -->
                        <a href="/home/lich-su-quyen-uu-dai.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-success-lt text-success d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <i class="ti ti-ticket"></i>
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ quyền ưu đãi
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["HoSo_UuDai_Real"] ?? ViewState["DuVi1_Evocher_30PhanTram"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHoSoLaoDong" runat="server" Visible="false">
                        <!-- 3) Hồ sơ lao động -->
                        <a href="/home/lich-su-quyen-lao-dong.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-warning-lt text-warning d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <i class="ti ti-briefcase"></i>
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ hành vi lao động
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["HoSo_LaoDong_Real"] ?? ViewState["DuVi2_LaoDong_50PhanTram"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHoSoGanKet" runat="server" Visible="false">
                        <!-- 4) Hồ sơ gắn kết -->
                        <a href="/home/lich-su-quyen-gan-ket.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-primary-lt text-primary d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <i class="ti ti-heart-handshake"></i>
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ chỉ số gắn kết
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["HoSo_GanKet_Real"] ?? ViewState["DuVi3_GanKet_20PhanTram"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>

        <!-- ================= KHÁC ================= -->
        <div class="text-secondary small fw-semibold px-1 mb-2">Khác</div>
        <div class="card border-0 bg-light" style="border-radius: 14px;">
            <div class="list-group list-group-flush" style="border-radius: 14px; overflow: hidden;">

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/edit-info.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-settings text-secondary"></i>
                        <span class="fw-medium">Cài đặt tài khoản</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="#">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-help-circle text-secondary"></i>
                        <span class="fw-medium">Trợ giúp</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <!-- ✅ NEW: Đóng góp ý kiến -->
                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/dong-gop-y-kien.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-message-report text-secondary"></i>
                        <span class="fw-medium">Đóng góp ý kiến</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

            </div>
        </div>
    </asp:PlaceHolder>
        </div>
        <asp:PlaceHolder ID="phAccountLogoutFooter" runat="server" Visible="false">
            <div class="account-logout-sticky">
                <asp:LinkButton
                    ID="but_dangxuat"
                    runat="server"
                    CssClass="account-logout-btn"
                    OnClick="dangxuat_Click">
                    <i class="ti ti-logout"></i>
                    <span>Đăng xuất</span>
                </asp:LinkButton>
            </div>
        </asp:PlaceHolder>
    </div>
</div>

<script>
    function showNotif() {
        var p = document.getElementById('notifPopup');
        if (!p) return;
        p.style.display = 'block';
    }

    function hideNotif() {
        var p = document.getElementById('notifPopup');
        if (!p) return;
        p.style.display = 'none';
    }

    document.addEventListener('click', function (e) {
        var popup = document.getElementById('notifPopup');
        var btn = document.getElementById('<%= but_show_form_thongbao.ClientID %>');
        var btnShop = document.getElementById('<%= but_show_form_thongbao_shop.ClientID %>');

        if (!popup) return;
        if ((btn && btn.contains(e.target)) || (btnShop && btnShop.contains(e.target))) return;
        if (!popup.contains(e.target)) {
            hideNotif();
        }
    });
</script>

<script>
    function ensureCenterNotice() {
        var existing = document.getElementById('ahaCenterNotice');
        if (existing) return existing;

        var wrap = document.createElement('div');
        wrap.id = 'ahaCenterNotice';
        wrap.style.cssText = 'position:fixed;inset:0;display:none;align-items:center;justify-content:center;z-index:99999;background:rgba(0,0,0,0.35);';

        var card = document.createElement('div');
        card.id = 'ahaCenterNoticeCard';
        card.style.cssText = 'min-width:240px;max-width:85vw;background:#fff;border-radius:14px;padding:16px 18px;box-shadow:0 12px 30px rgba(0,0,0,.2);text-align:center;font-weight:600;';

        wrap.appendChild(card);
        document.body.appendChild(wrap);
        return wrap;
    }

    function showCenterNotice(message, type) {
        var wrap = ensureCenterNotice();
        var card = document.getElementById('ahaCenterNoticeCard');
        if (!wrap || !card) return;

        card.style.color = (type === 'success') ? '#0f5132' : '#664d03';
        card.style.background = (type === 'success') ? '#d1e7dd' : '#fff3cd';
        card.style.border = (type === 'success') ? '1px solid #badbcc' : '1px solid #ffecb5';
        card.textContent = message || '';

        wrap.style.display = 'flex';
        clearTimeout(wrap._hideTimer);
        wrap._hideTimer = setTimeout(function () {
            wrap.style.display = 'none';
        }, 2200);
    }

    function copyReferralLink(el) {
        try {
            var link = (el && el.getAttribute('data-ref-link')) ? el.getAttribute('data-ref-link') : '';
            if (!link || link.slice(-2) === 'u=') {
                showCenterNotice('Không có link giới thiệu để sao chép.', 'warning');
                return false;
            }

            function showSuccess() {
                showCenterNotice('Đã sao chép link giới thiệu thành công.', 'success');
            }

            function showFail() {
                showCenterNotice('Không thể sao chép link. Vui lòng thử lại.', 'warning');
            }

            function fallbackCopy(text) {
                var ta = document.createElement('textarea');
                ta.value = text;
                ta.setAttribute('readonly', '');
                ta.style.position = 'fixed';
                ta.style.top = '-1000px';
                document.body.appendChild(ta);
                ta.select();
                try {
                    var ok = document.execCommand('copy');
                    if (ok) showSuccess();
                    else showFail();
                } catch (e) {
                    showFail();
                }
                document.body.removeChild(ta);
            }

            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(link).then(showSuccess).catch(function () {
                    fallbackCopy(link);
                });
            } else {
                fallbackCopy(link);
            }
        } catch (e) { }

        return false;
    }

    function bindCopyReferralHandler() {
        document.addEventListener('click', function (ev) {
            if (ev.defaultPrevented) return;
            var target = ev.target;
            if (!target) return;
            var el = target.closest('[data-copy-ref="1"]');
            if (!el) return;
            ev.preventDefault();
            copyReferralLink(el);
        });
    }

    document.addEventListener('DOMContentLoaded', bindCopyReferralHandler);
</script>

<script>
    function refreshCenterLogoLayout() {
        var header = document.querySelector('.site-header');
        if (!header) return;

        var left = header.querySelector('.mobile-left-icons');
        var right = header.querySelector('.mobile-right-account');
        var logos = header.querySelectorAll('.brand-center-logo-img');
        if (!logos || logos.length === 0) return;

        var viewportWidth = Math.max(document.documentElement ? document.documentElement.clientWidth : 0, window.innerWidth || 0);
        var leftWidth = left ? Math.ceil(left.getBoundingClientRect().width) : 0;
        var rightWidth = right ? Math.ceil(right.getBoundingClientRect().width) : 0;
        var sideMax = Math.max(leftWidth, rightWidth);
        var availableWidth = Math.max(92, viewportWidth - (sideMax * 2) - 18);
        document.documentElement.style.setProperty('--aha-logo-available-width', availableWidth + 'px');

        var topbarHeight = Math.max(40, Math.ceil(header.getBoundingClientRect().height));
        for (var i = 0; i < logos.length; i++) {
            var logo = logos[i];
            if (!logo) continue;

            logo.classList.remove('logo-fit-by-width');
            var naturalWidth = parseFloat(logo.naturalWidth || 0);
            var naturalHeight = parseFloat(logo.naturalHeight || 0);
            if (naturalWidth <= 0 || naturalHeight <= 0) continue;

            var projectedWidth = (naturalWidth / naturalHeight) * topbarHeight;
            if (projectedWidth > availableWidth) {
                logo.classList.add('logo-fit-by-width');
            }
        }
    }

    function syncCenterLogoFromHead() {
        var iconUrl = "";
        var apple = document.querySelector("link[rel='apple-touch-icon'][sizes='180x180']")
            || document.querySelector("link[rel='apple-touch-icon']");
        if (apple) iconUrl = (apple.getAttribute('href') || "").trim();
        if (!iconUrl) {
            var tile = document.querySelector("meta[name='msapplication-TileImage']");
            if (tile) iconUrl = (tile.getAttribute('content') || "").trim();
        }
        if (!iconUrl) return;

        var logos = document.querySelectorAll('.brand-center-logo-img');
        if (!logos || logos.length === 0) return;
        for (var i = 0; i < logos.length; i++) {
            if (!logos[i]) continue;
            logos[i].setAttribute('src', iconUrl);
            if (!logos[i].complete) {
                logos[i].addEventListener('load', refreshCenterLogoLayout, { once: true });
            }
        }
        refreshCenterLogoLayout();
    }

    function cloneAccountNode(source) {
        if (!source) return null;
        var node = source.cloneNode(true);
        node.removeAttribute('id');
        node.querySelectorAll('[id]').forEach(function (el) {
            el.removeAttribute('id');
        });
        node.querySelectorAll('script').forEach(function (el) {
            el.remove();
        });
        return node;
    }

    function mountAccountMenuOnce() {
        var tpl = document.getElementById('accountMenuContent');
        var hostDesktop = document.getElementById('accountMenuHostDesktop');
        if (!tpl || !hostDesktop) return;
        if (hostDesktop.children && hostDesktop.children.length > 0) return;
        hostDesktop.innerHTML = tpl.innerHTML;
        tpl.style.display = 'none';
    }

    function syncAccountMenuToMobile() {
        var hostDesktop = document.getElementById('accountMenuHostDesktop');
        var hostMobile = document.getElementById('accountMenuHostMobile');
        if (!hostDesktop || !hostMobile) return;
        var clone = cloneAccountNode(hostDesktop);
        if (!clone) return;
        hostMobile.innerHTML = clone.innerHTML;
    }

    function setupAccountDropdown() {
        var toggle = document.getElementById('userMenuToggle');
        var wrapper = toggle ? toggle.closest('.aha-account') : null;
        if (!toggle || !wrapper) return;
        if (toggle.getAttribute('data-dropdown-bound') === '1') return;
        toggle.setAttribute('data-dropdown-bound', '1');
        wrapper.classList.remove('is-open');
        toggle.setAttribute('aria-expanded', 'false');

        function closeMenu() {
            wrapper.classList.remove('is-open');
            toggle.setAttribute('aria-expanded', 'false');
        }

        function openMenu() {
            wrapper.classList.add('is-open');
            toggle.setAttribute('aria-expanded', 'true');
        }

        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            if (wrapper.classList.contains('is-open')) {
                closeMenu();
            } else {
                openMenu();
            }
        });

        document.addEventListener('click', function (e) {
            if (!wrapper.contains(e.target)) {
                closeMenu();
            }
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') closeMenu();
        });
    }

    function setupQuickDropdown() {
        var shell = document.getElementById('headerQuickShell');
        var toggle = document.getElementById('headerQuickToggle');
        var menu = document.getElementById('headerQuickMenu');
        if (!shell || !toggle || !menu) return;
        if (toggle.getAttribute('data-quick-bound') === '1') return;
        toggle.setAttribute('data-quick-bound', '1');
        shell.classList.remove('is-open');
        toggle.setAttribute('aria-expanded', 'false');

        function closeMenu() {
            shell.classList.remove('is-open');
            toggle.setAttribute('aria-expanded', 'false');
        }

        function openMenu() {
            shell.classList.add('is-open');
            toggle.setAttribute('aria-expanded', 'true');
        }

        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (shell.classList.contains('is-open')) {
                closeMenu();
            } else {
                openMenu();
            }
        });

        menu.addEventListener('click', function (e) {
            e.stopPropagation();
        });

        document.addEventListener('click', function (e) {
            if (!shell.contains(e.target)) {
                closeMenu();
            }
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') closeMenu();
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        syncCenterLogoFromHead();
        refreshCenterLogoLayout();
        mountAccountMenuOnce();
        setupAccountDropdown();
        setupQuickDropdown();
    });

    window.addEventListener('load', function () {
        syncCenterLogoFromHead();
        refreshCenterLogoLayout();
    });

    window.addEventListener('resize', refreshCenterLogoLayout);

    if (window.Sys && window.Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            syncCenterLogoFromHead();
            refreshCenterLogoLayout();
            mountAccountMenuOnce();
            setupAccountDropdown();
            setupQuickDropdown();
        });
    }

    document.addEventListener('shown.bs.offcanvas', function (e) {
        if (e.target && e.target.id === 'accountMenuCanvas') {
            syncAccountMenuToMobile();
        }
    });
</script>

<script>
    function showFavorite() {
        var p = document.getElementById('favoritePopup');
        if (!p) return;
        p.style.display = 'block';
    }

    function hideFavorite() {
        var p = document.getElementById('favoritePopup');
        if (!p) return;
        p.style.display = 'none';
    }


</script>
