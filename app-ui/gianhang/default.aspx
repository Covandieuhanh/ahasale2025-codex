<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <meta name="format-detection" content="telephone=no" />
    <meta name="format-detection" content="date=no" />
    <meta name="format-detection" content="address=no" />
    <meta name="format-detection" content="email=no" />
    <title>AhaSale App UI - Gian hàng</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/gianhang/app-gianhang.css?v=20260404d" />
</head>
<body class="app-ui app-ui-gianhang">
    <form id="form1" runat="server">
        <div class="app-shell app-shell-gianhang" data-app-shell="gianhang" data-app-nav-active="home" data-app-topbar-mode="reveal-on-scroll">
            <header class="app-topbar app-topbar-gianhang">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span>
                        <span></span>
                        <span></span>
                    </button>
                    <div class="app-search-pill app-search-pill-gianhang" role="button" tabindex="0" data-app-search-link="/app-ui/gianhang/listing.aspx?ui_mode=app">
                        <span class="app-search-icon"></span>
                        <span class="app-search-placeholder">Tìm thao tác gian hàng</span>
                    </div>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo">
                        <span class="app-badge">0</span>
                    </button>
                </div>
            </header>

            <main class="app-main">
                <section class="app-section app-homepage-hero-stack">
                    <a class="app-homepage-brandbar" href="/app-ui/gianhang/default.aspx?ui_mode=app" aria-label="Trang chủ Aha Shop">
                        <span class="app-homepage-brand-logo">
                            <img src="/app-ui/assets/brand/logo-aha-trang.png" alt="Aha Shop" loading="eager" />
                        </span>
                        <span class="app-homepage-brand-copy">
                            <span class="app-homepage-brand-title">AHA SHOP</span>
                            <span class="app-homepage-brand-subtitle">Không gian gian hàng</span>
                        </span>
                    </a>
                    <a class="app-homepage-banner-link app-homepage-banner-link-stacked" href="/app-ui/gianhang/listing.aspx?ui_mode=app">
                        <img class="app-homepage-banner-media" src="/app-ui/assets/banners/gianhang-banner-home-style.png" alt="Banner Aha Shop" loading="eager" />
                    </a>
                </section>

                <section class="app-section app-homepage-search">
                    <div class="app-search-pill app-search-pill-gianhang" role="search" data-app-search-form="1" data-app-search-space="gianhang">
                        <span class="app-search-icon"></span>
                        <input type="search" placeholder="Tìm tin đăng, lead, khách hàng..." data-app-search-input="1" />
                        <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>
                    </div>
                </section>

                <section class="app-section">
                    <div class="app-quick-action-grid" data-role="gh-quick-actions"></div>
                </section>

                <div class="app-topbar-reveal-anchor" data-app-topbar-reveal-anchor="1"></div>

                <section class="app-section">
                    <div class="app-hero-stat-grid" data-role="gh-hero-stats"></div>
                </section>

                <section class="app-section">
                    <div class="app-gianhang-focus-strip" data-role="gh-focus-strip"></div>
                </section>

                <section class="app-section">
                    <div class="app-metric-row" data-role="gh-metrics"></div>
                </section>

                <section class="app-section">
                    <div class="app-gianhang-priority-strip" data-role="gh-priority-strip"></div>
                </section>

                <section class="app-section app-feed-tabs-wrap">
                    <div class="app-feed-tabs" data-role="gh-status-tabs"></div>
                </section>

                <section class="app-section">
                    <div class="app-ops-list" data-role="gh-listings"></div>
                </section>

                <section class="app-section">
                    <div class="app-section-head">
                        <div>
                            <h2 class="app-section-title">Khách hàng cần xử lý</h2>
                            <p class="app-section-subtitle">Nhóm khách và trao đổi ưu tiên cao trong seller flow</p>
                        </div>
                    </div>
                    <div class="app-conversation-list" data-role="gh-conversations"></div>
                </section>
            </main>

            <nav class="app-bottom-nav" aria-label="Điều hướng gian hàng">
                <a class="app-bottom-item is-active" href="/app-ui/gianhang/default.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-home"></span>
                    <span>Tổng quan</span>
                </a>
                <a class="app-bottom-item" href="/app-ui/gianhang/listing.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-tag"></span>
                    <span>Quản lý tin</span>
                </a>
                <a class="app-post-cta app-post-cta-gianhang" href="/app-ui/gianhang/actions.aspx?ui_mode=app" aria-label="Mở công cụ seller">+</a>
                <a class="app-bottom-item" href="/app-ui/gianhang/conversations.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-chat"></span>
                    <span>Khách hàng</span>
                </a>
                <button class="app-bottom-item" type="button" data-app-trigger="account">
                    <span class="app-bottom-icon app-bottom-user"></span>
                    <span>Tài khoản</span>
                </button>
            </nav>
        </div>
    </form>

    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260404f"></script>
    <script src="/app-ui/shared/adapters/app-ui-listing-adapter.js?v=20260403d"></script>
    <script src="/app-ui/shared/adapters/app-ui-seller-adapter.js"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
    <script src="/app-ui/gianhang/app-gianhang.js?v=20260404e"></script>
</body>
</html>
