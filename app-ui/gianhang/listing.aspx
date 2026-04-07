<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Danh sách tin gian hàng</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/gianhang/app-gianhang.css?v=20260404d" />
</head>
<body class="app-ui app-ui-gianhang-listing">
<form id="form1" runat="server">
<div class="app-shell app-shell-gianhang" data-app-shell="gianhang" data-app-nav-active="listing">
<header class="app-topbar app-topbar-gianhang">
<div class="app-safe-top"></div>
<div class="app-topbar-row">
<button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu"><span></span><span></span><span></span></button>
<div class="app-search-pill app-search-pill-gianhang"><span class="app-search-icon"></span><span class="app-search-placeholder">Danh sách tin đăng gian hàng</span></div>
<button class="app-icon-btn" type="button" aria-label="Công việc"><span class="app-gianhang-dot"></span></button>
<button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">9</span></button>
<button class="app-icon-btn app-icon-user" type="button" aria-label="Tài khoản" data-app-trigger="account"></button>
</div>
</header>
<main class="app-main">
<section class="app-section">
<div class="app-results-head">
<h1 class="app-results-title">Danh sách tin đăng</h1>
<p class="app-results-copy">Tập trung toàn bộ listing đang bán, chờ duyệt và cần sửa trong seller app.</p>
</div>
</section>
<section class="app-section app-feed-tabs-wrap"><div class="app-feed-tabs" data-role="gh-status-tabs"></div></section>
<section class="app-section"><div class="app-ops-list" data-role="gh-listing-page"></div></section>
</main>
<nav class="app-bottom-nav" aria-label="Điều hướng gian hàng">
<a class="app-bottom-item" href="/app-ui/gianhang/default.aspx?ui_mode=app"><span class="app-bottom-icon app-bottom-home"></span><span>Tổng quan</span></a>
<a class="app-bottom-item is-active" href="/app-ui/gianhang/listing.aspx?ui_mode=app"><span class="app-bottom-icon app-bottom-tag"></span><span>Quản lý tin</span></a>
<a class="app-post-cta app-post-cta-gianhang" href="/app-ui/gianhang/actions.aspx?ui_mode=app" aria-label="Mở công cụ seller">+</a>
<a class="app-bottom-item" href="/app-ui/gianhang/conversations.aspx?ui_mode=app"><span class="app-bottom-icon app-bottom-chat"></span><span>Khách hàng</span></a>
<button class="app-bottom-item" type="button" data-app-trigger="account"><span class="app-bottom-icon app-bottom-user"></span><span>Tài khoản</span></button>
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
<script src="/app-ui/gianhang/app-gianhang-listing.js"></script>
</body>
</html>
