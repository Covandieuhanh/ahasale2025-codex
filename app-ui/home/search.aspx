<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Tìm kiếm Home</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406compact3" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-results.css?v=20260406compact2" />
    <link rel="stylesheet" href="/app-ui/home/app-home.css?v=20260406compact7" />
</head>
<body class="app-ui app-ui-home-search">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="search">
            <header class="app-topbar">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span><span></span><span></span>
                    </button>
                    <div class="app-search-pill" role="search" data-app-search-form="1" data-app-search-space="home">
                        <span class="app-search-icon"></span>
                        <input type="search" placeholder="Tìm sản phẩm..." data-app-search-input="1" />
                        <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>
                    </div>
                    <button class="app-icon-btn app-icon-heart" type="button" aria-label="Tin đã lưu"></button>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">5</span></button>
                    <button class="app-icon-btn app-icon-user" type="button" aria-label="Tài khoản" data-app-trigger="account"></button>
                </div>
            </header>
            <main class="app-main">
                <section class="app-section">
                    <div class="app-results-head">
                        <h1 class="app-results-title">Kết quả Home</h1>
                        <p class="app-results-copy" data-role="search-summary"></p>
                        <div class="app-results-actions" data-role="search-actions"></div>
                    </div>
                    <div class="app-feed-grid" data-role="home-search-results">
                        <article class="app-skeleton-card">
                            <div class="app-skeleton-block app-skeleton-card-media"></div>
                            <div class="app-skeleton-card-body">
                                <div class="app-skeleton-line"></div>
                                <div class="app-skeleton-line"></div>
                                <div class="app-skeleton-line"></div>
                                <div class="app-skeleton-line"></div>
                            </div>
                        </article>
                        <article class="app-skeleton-card">
                            <div class="app-skeleton-block app-skeleton-card-media"></div>
                            <div class="app-skeleton-card-body">
                                <div class="app-skeleton-line"></div>
                                <div class="app-skeleton-line"></div>
                                <div class="app-skeleton-line"></div>
                                <div class="app-skeleton-line"></div>
                            </div>
                        </article>
                    </div>
                </section>
            </main>
        </div>
    </form>
    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/adapters/app-ui-search-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-listing-adapter.js?v=20260403d"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/shared/app-ui-results.js?v=20260403b"></script>
</body>
</html>
