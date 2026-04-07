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
    <title>AhaSale App UI - Chợ xe tốt</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/choxe/app-choxe.css" />
</head>
<body class="app-ui app-ui-choxe">
    <form id="form1" runat="server">
        <div class="app-shell app-shell-choxe" data-app-shell="choxe" data-app-nav-active="home" data-app-topbar-mode="reveal-on-scroll">
            <header class="app-topbar app-topbar-choxe">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span><span></span><span></span>
                    </button>
                    <div class="app-search-pill app-search-pill-choxe" role="button" tabindex="0" data-app-search-link="/app-ui/choxe/search.aspx?ui_mode=app">
                        <span class="app-search-icon"></span>
                        <span class="app-search-placeholder">Tìm xe</span>
                    </div>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">0</span></button>
                </div>
            </header>

            <main class="app-main">
                <section class="app-section app-homepage-hero-stack">
                    <a class="app-homepage-brandbar" href="/app-ui/choxe/default.aspx?ui_mode=app" aria-label="Trang chủ Aha Xe">
                        <span class="app-homepage-brand-logo">
                            <img src="/app-ui/assets/brand/logo-aha-trang.png" alt="Aha Xe" loading="eager" />
                        </span>
                        <span class="app-homepage-brand-copy">
                            <span class="app-homepage-brand-title">AHA XE</span>
                            <span class="app-homepage-brand-subtitle">Không gian xe</span>
                        </span>
                    </a>
                    <a class="app-homepage-banner-link app-homepage-banner-link-stacked" href="/app-ui/choxe/search.aspx?ui_mode=app">
                        <img class="app-homepage-banner-media" src="/app-ui/assets/banners/home-banner-20260328.png" alt="Banner Aha Xe" loading="eager" />
                    </a>
                </section>

                <section class="app-section app-homepage-search">
                    <div class="app-search-pill app-search-pill-choxe" role="search" data-app-search-form="1" data-app-search-space="choxe">
                        <span class="app-search-icon"></span>
                        <input type="search" placeholder="Tìm ô tô, xe máy, đời xe, hãng xe..." data-app-search-input="1" />
                        <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>
                    </div>
                    <div class="app-chip-lane app-chip-lane-inline" data-role="choxe-quick-filters"></div>
                </section>

                <section class="app-section">
                    <div class="app-space-rail-track app-choxe-type-track" data-role="choxe-categories"></div>
                </section>

                <div class="app-topbar-reveal-anchor" data-app-topbar-reveal-anchor="1"></div>

                <section class="app-section">
                    <div class="app-space-quick-actions" data-role="choxe-quick-actions"></div>
                </section>

                <section class="app-section">
                    <div class="app-choxe-market-strip">
                        <article class="app-choxe-market-card">
                            <span class="app-choxe-market-value">Ô tô</span>
                            <span class="app-choxe-market-label">Nhóm xe gia đình và SUV đang được xem nhiều</span>
                        </article>
                        <article class="app-choxe-market-card">
                            <span class="app-choxe-market-value">Xe máy</span>
                            <span class="app-choxe-market-label">Xe tay ga và xe số cũ giao dịch nhanh</span>
                        </article>
                        <article class="app-choxe-market-card">
                            <span class="app-choxe-market-value">Biên Hòa</span>
                            <span class="app-choxe-market-label">Nguồn xe cũ đời 2020-2023 tăng mạnh</span>
                        </article>
                    </div>
                </section>

                <section class="app-section">
                    <div class="app-section-head">
                        <div>
                            <h2 class="app-section-title">Lối tắt đang có</h2>
                            <p class="app-section-subtitle">Các điểm vào chính của không gian xe trên nền tảng hiện tại</p>
                        </div>
                        <a class="app-ghost-btn" href="/app-ui/choxe/hub.aspx?ui_mode=app">Xem danh mục</a>
                    </div>
                    <div class="app-card-strip" data-role="choxe-highlights"></div>
                </section>

                <section class="app-section app-feed-tabs-wrap">
                    <div class="app-feed-tabs" data-role="choxe-feed-tabs"></div>
                </section>

                <section class="app-section app-feed-grid" data-role="choxe-listing-feed"></section>
            </main>

            <nav class="app-bottom-nav" aria-label="Điều hướng chợ xe tốt">
                <a class="app-bottom-item is-active" href="/app-ui/choxe/default.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-home"></span>
                    <span>Tổng quan</span>
                </a>
                <a class="app-bottom-item" href="/app-ui/choxe/hub.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-tag"></span>
                    <span>Danh mục</span>
                </a>
                <a class="app-post-cta app-post-cta-choxe" href="/app-ui/gianhang/default.aspx?ui_mode=app" aria-label="Đăng tin xe">+</a>
                <a class="app-bottom-item" href="/app-ui/gianhang/listing.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-chat"></span>
                    <span>Quản lý tin</span>
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
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/adapters/app-ui-listing-adapter.js?v=20260403d"></script>
    <script src="/app-ui/shared/adapters/app-ui-search-adapter.js"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
    <script src="/app-ui/choxe/app-choxe.js?v=20260403b"></script>
</body>
</html>
