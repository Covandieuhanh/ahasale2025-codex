<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Chi tiết gian hàng</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/gianhang/app-gianhang.css?v=20260404d" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-detail.css" />
</head>
<body class="app-ui app-ui-gianhang-detail">
    <form id="form1" runat="server">
        <div class="app-shell app-shell-gianhang" data-app-shell="gianhang" data-app-nav-active="listing">
            <header class="app-topbar app-topbar-gianhang">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span><span></span><span></span>
                    </button>
                    <div class="app-search-pill"><span class="app-search-icon"></span><span class="app-search-placeholder">Chi tiết listing gian hàng</span></div>
                    <button class="app-icon-btn" type="button" aria-label="Công việc"><span class="app-gianhang-dot"></span></button>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">9</span></button>
                    <button class="app-icon-btn app-icon-user" type="button" aria-label="Tài khoản" data-app-trigger="account"></button>
                </div>
            </header>
            <main class="app-main" data-role="gianhang-detail-host">
                <section class="app-section">
                    <div class="app-skeleton-block app-detail-hero"></div>
                    <div class="app-detail-body">
                        <div class="app-skeleton-pill" style="width:92px;"></div>
                        <div class="app-skeleton-line" style="width:84%;height:18px;margin-top:14px;"></div>
                        <div class="app-skeleton-line" style="width:52%;height:18px;margin-top:10px;"></div>
                        <div class="app-skeleton-line" style="width:90%;margin-top:12px;"></div>
                        <div class="app-skeleton-line" style="width:78%;margin-top:8px;"></div>
                    </div>
                </section>
            </main>
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
    <script src="/app-ui/gianhang/app-gianhang-detail.js?v=20260404e"></script>
</body>
</html>
