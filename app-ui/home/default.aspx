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
    <title>AhaSale App UI - Home</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406compact3" />
    <link rel="stylesheet" href="/app-ui/home/app-home.css?v=20260406compact9" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="home" data-app-topbar-mode="reveal-on-scroll">
            <header class="app-topbar">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span>
                        <span></span>
                        <span></span>
                    </button>
                    <div class="app-search-pill" role="search" data-app-search-form="1" data-app-search-space="home">
                        <span class="app-search-icon"></span>
                        <input type="search" placeholder="Tìm kiếm nhanh" data-app-search-input="1" />
                        <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>
                    </div>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo">
                        <span class="app-badge">0</span>
                    </button>
                </div>
            </header>

            <main class="app-main">
                <section class="app-section app-homepage-hero-stack">
                    <a class="app-homepage-brandbar" href="/app-ui/home/default.aspx?ui_mode=app" aria-label="Trang chủ Aha Sale">
                        <span class="app-homepage-brand-logo">
                            <img src="/app-ui/assets/brand/logo-aha-trang.png" alt="Aha Sale" loading="eager" />
                        </span>
                        <span class="app-homepage-brand-copy">
                            <span class="app-homepage-brand-title">AHA SALE</span>
                            <span class="app-homepage-brand-subtitle">Trang chủ tổng hợp</span>
                        </span>
                    </a>
                    <a class="app-homepage-banner-link app-homepage-banner-link-stacked" href="/app-ui/home/search.aspx?ui_mode=app">
                        <img class="app-homepage-banner-media" src="/app-ui/assets/banners/home-banner-20260328.png" alt="Banner trang chủ Aha Sale" loading="eager" />
                    </a>
                </section>

                <section class="app-section app-homepage-search">
                    <div class="app-search-pill" role="search" data-app-search-form="1" data-app-search-space="home">
                        <span class="app-search-icon"></span>
                        <input type="search" placeholder="Tìm sản phẩm, tin đăng, không gian..." data-app-search-input="1" />
                        <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>
                    </div>
                    <div class="app-chip-lane app-chip-lane-inline" data-role="recent-searches"></div>
                </section>

                <section class="app-section app-space-rail">
                    <div class="app-space-rail-track" data-role="space-categories"></div>
                </section>

                <div class="app-topbar-reveal-anchor" data-app-topbar-reveal-anchor="1"></div>

                <section class="app-section">
                    <div class="app-space-quick-actions" data-role="home-quick-actions"></div>
                </section>

                <section class="app-section app-feed-tabs-wrap">
                    <div class="app-feed-tabs" data-role="feed-tabs"></div>
                </section>

                <section class="app-section app-feed-grid" data-role="listing-feed"></section>
            </main>

            <nav class="app-bottom-nav" aria-label="Điều hướng chính">
                <a class="app-bottom-item is-active" href="/app-ui/home/default.aspx">
                    <span class="app-bottom-icon app-bottom-home"></span>
                    <span>Trang chủ</span>
                </a>
                <a class="app-bottom-item" href="/app-ui/home/search.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-tag"></span>
                    <span>Tìm kiếm</span>
                </a>
                <a class="app-post-cta" href="/app-ui/gianhang/default.aspx?ui_mode=app" aria-label="Đăng tin">+</a>
                <a class="app-bottom-item" href="/app-ui/batdongsan/default.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-chat"></span>
                    <span>Aha Land</span>
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
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260406a"></script>
    <script src="/app-ui/shared/adapters/app-ui-search-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-listing-adapter.js?v=20260403d"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/home/app-home.js?v=20260406compact12"></script>
</body>
</html>
