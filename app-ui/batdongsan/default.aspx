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
    <title>AhaSale App UI - Bất động sản</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/batdongsan/app-bds.css" />
</head>
<body class="app-ui app-ui-bds">
    <form id="form1" runat="server">
        <div class="app-shell app-shell-bds" data-app-shell="batdongsan" data-app-nav-active="home" data-app-topbar-mode="reveal-on-scroll">
            <header class="app-topbar app-topbar-bds">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span>
                        <span></span>
                        <span></span>
                    </button>
                    <div class="app-search-pill app-search-pill-bds" role="button" tabindex="0" data-app-search-link="/app-ui/batdongsan/search.aspx?ui_mode=app">
                        <span class="app-search-icon"></span>
                        <span class="app-search-placeholder">Tìm bất động sản</span>
                    </div>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo">
                        <span class="app-badge">0</span>
                    </button>
                </div>
            </header>

            <main class="app-main">
                <section class="app-section app-homepage-hero-stack">
                    <a class="app-homepage-brandbar" href="/app-ui/batdongsan/default.aspx?ui_mode=app" aria-label="Trang chủ Aha Land">
                        <span class="app-homepage-brand-logo">
                            <img src="/app-ui/assets/brand/logo-aha-trang.png" alt="Aha Land" loading="eager" />
                        </span>
                        <span class="app-homepage-brand-copy">
                            <span class="app-homepage-brand-title">AHA LAND</span>
                            <span class="app-homepage-brand-subtitle">Không gian bất động sản</span>
                        </span>
                    </a>
                    <a class="app-homepage-banner-link app-homepage-banner-link-stacked" href="/app-ui/batdongsan/search.aspx?ui_mode=app">
                        <img class="app-homepage-banner-media" src="/app-ui/assets/banners/bds-banner-top.png" alt="Banner Aha Land" loading="eager" />
                    </a>
                </section>

                <section class="app-section app-homepage-search">
                    <div class="app-search-pill app-search-pill-bds" role="search" data-app-search-form="1" data-app-search-space="batdongsan">
                        <span class="app-search-icon"></span>
                        <input type="search" placeholder="Tìm nhà, căn hộ, khu vực, dự án..." data-app-search-input="1" />
                        <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>
                    </div>
                    <div class="app-chip-lane app-chip-lane-inline" data-role="bds-quick-filters"></div>
                </section>

                <section class="app-section">
                    <div class="app-space-rail-track app-bds-type-track" data-role="bds-property-types"></div>
                </section>

                <div class="app-topbar-reveal-anchor" data-app-topbar-reveal-anchor="1"></div>

                <section class="app-section">
                    <div class="app-space-quick-actions" data-role="bds-quick-actions"></div>
                </section>

                <section class="app-section">
                    <div class="app-bds-market-strip">
                        <article class="app-bds-market-card">
                            <span class="app-bds-market-value">Bình Thạnh</span>
                            <span class="app-bds-market-label">Nguồn tin đang về căn hộ và nhà phố</span>
                        </article>
                        <article class="app-bds-market-card">
                            <span class="app-bds-market-value">Quận 9</span>
                            <span class="app-bds-market-label">Nhóm tìm đất nền và căn hộ tăng nhanh</span>
                        </article>
                        <article class="app-bds-market-card">
                            <span class="app-bds-market-value">Thủ Đức</span>
                            <span class="app-bds-market-label">Phù hợp khám phá dự án và cho thuê</span>
                        </article>
                    </div>
                </section>

                <section class="app-section">
                    <div class="app-section-head">
                        <div>
                            <h2 class="app-section-title">Khu vực nổi bật</h2>
                            <p class="app-section-subtitle">Nguồn cung và nhu cầu đang lên nhanh</p>
                        </div>
                    </div>
                    <div class="app-card-strip" data-role="bds-highlights"></div>
                </section>

                <section class="app-section app-feed-tabs-wrap">
                    <div class="app-feed-tabs" data-role="bds-feed-tabs"></div>
                </section>

                <section class="app-section app-feed-grid" data-role="bds-listing-feed"></section>
            </main>

            <nav class="app-bottom-nav" aria-label="Điều hướng bất động sản">
                <a class="app-bottom-item is-active" href="/app-ui/batdongsan/default.aspx">
                    <span class="app-bottom-icon app-bottom-home"></span>
                    <span>Tổng quan</span>
                </a>
                <a class="app-bottom-item" href="/app-ui/batdongsan/hub.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-tag"></span>
                    <span>Danh mục</span>
                </a>
                <a class="app-post-cta app-post-cta-bds" href="/app-ui/gianhang/default.aspx?ui_mode=app" aria-label="Đăng tin">+</a>
                <a class="app-bottom-item" href="/app-ui/batdongsan/search.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-chat"></span>
                    <span>Tìm kiếm</span>
                </a>
                <a class="app-bottom-item" href="/app-ui/home/default.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-user"></span>
                    <span>Aha Sale</span>
                </a>
            </nav>
        </div>
    </form>

    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/adapters/app-ui-listing-adapter.js?v=20260403d"></script>
    <script src="/app-ui/shared/adapters/app-ui-search-adapter.js"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
    <script src="/app-ui/batdongsan/app-bds.js?v=20260403b"></script>
</body>
</html>
