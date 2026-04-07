<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Danh mục bất động sản</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-results.css" />
    <link rel="stylesheet" href="/app-ui/batdongsan/app-bds.css" />
</head>
<body class="app-ui app-ui-bds-hub">
<form id="form1" runat="server">
<div class="app-shell app-shell-bds" data-app-shell="batdongsan" data-app-nav-active="categories">
<header class="app-topbar app-topbar-bds">
<div class="app-safe-top"></div>
<div class="app-topbar-row">
<button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu"><span></span><span></span><span></span></button>
<div class="app-search-pill app-search-pill-bds" role="search" data-app-search-form="1" data-app-search-space="batdongsan"><span class="app-search-icon"></span><input type="search" placeholder="Tìm theo nhánh bất động sản..." data-app-search-input="1" /><button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button></div>
<button class="app-icon-btn" type="button" aria-label="Lưu tin"><span class="app-bds-heart"></span></button>
<button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">3</span></button>
<button class="app-icon-btn app-icon-user" type="button" aria-label="Tài khoản" data-app-trigger="account"></button>
</div>
</header>
<main class="app-main">
<section class="app-section"><div class="app-results-head"><h1 class="app-results-title">Danh mục bất động sản</h1><p class="app-results-copy">Đi vào từng nhánh lớn của Aha Land để xem đúng loại tin và đúng nhu cầu.</p></div></section>
<section class="app-section"><div class="app-filter-groups" data-role="bds-hub-groups"></div></section>
</main>
</div>
</form>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
<script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
<script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
<script src="/app-ui/shared/app-ui-registry.js"></script>
<script src="/app-ui/shared/app-ui-mode.js"></script>
<script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
<script src="/app-ui/batdongsan/app-bds-hub.js?v=20260404a"></script>
</body>
</html>
