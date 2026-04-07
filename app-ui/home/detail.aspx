<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Chi tiết Home</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406compact3" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-detail.css?v=20260406compact2" />
    <link rel="stylesheet" href="/app-ui/home/app-home.css?v=20260406compact7" />
</head>
<body class="app-ui app-ui-home-detail">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="search">
            <header class="app-topbar">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span><span></span><span></span>
                    </button>
                    <div class="app-search-pill"><span class="app-search-icon"></span><span class="app-search-placeholder">Chi tiết tin đăng</span></div>
                    <button class="app-icon-btn app-icon-heart" type="button" aria-label="Lưu tin"></button>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">5</span></button>
                    <button class="app-icon-btn app-icon-user" type="button" aria-label="Tài khoản" data-app-trigger="account"></button>
                </div>
            </header>
            <main class="app-main" data-role="detail-host">
                <section class="app-section">
                    <div class="app-skeleton-block app-detail-hero"></div>
                    <div class="app-detail-body">
                        <div class="app-skeleton-pill" style="width:88px;"></div>
                        <div class="app-skeleton-line" style="width:82%;height:18px;margin-top:14px;"></div>
                        <div class="app-skeleton-line" style="width:48%;height:18px;margin-top:10px;"></div>
                        <div class="app-skeleton-line" style="width:68%;margin-top:12px;"></div>
                        <div class="app-skeleton-line" style="width:92%;margin-top:8px;"></div>
                        <div class="app-skeleton-line" style="width:76%;margin-top:8px;"></div>
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
    <script src="/app-ui/shared/adapters/app-ui-listing-adapter.js?v=20260403d"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/shared/app-ui-detail.js?v=20260404a"></script>
</body>
</html>
