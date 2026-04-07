<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Danh mục điện thoại máy tính</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/dienthoai-maytinh/app-tech.css" />
</head>
<body class="app-ui app-ui-tech-hub">
    <form id="form1" runat="server">
        <div class="app-shell app-shell-tech" data-app-shell="dienthoai-maytinh" data-app-nav-active="categories">
            <header class="app-topbar app-topbar-tech">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu"><span></span><span></span><span></span></button>
                    <div class="app-search-pill app-search-pill-tech" role="search" data-app-search-form="1" data-app-search-space="dienthoai-maytinh">
                        <span class="app-search-icon"></span>
                        <input type="search" placeholder="Tìm theo danh mục công nghệ..." data-app-search-input="1" />
                        <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>
                    </div>
                    <button class="app-icon-btn" type="button" aria-label="Tin đã lưu"><span class="app-tech-chip"></span></button>
                    <button class="app-icon-btn app-icon-user" type="button" aria-label="Tài khoản" data-app-trigger="account"></button>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">6</span></button>
                </div>
            </header>
            <main class="app-main">
                <section class="app-section">
                    <div class="app-results-head">
                        <h1 class="app-results-title">Danh mục công nghệ</h1>
                        <p class="app-results-copy">Đi vào từng nhóm máy và phụ kiện để quét nhanh model, cấu hình và giá.</p>
                    </div>
                </section>
                <section class="app-section">
                    <div class="app-filter-groups" data-role="tech-hub-groups"></div>
                </section>
            </main>
        </div>
    </form>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
    <script src="/app-ui/dienthoai-maytinh/app-tech-hub.js?v=20260404a"></script>
</body>
</html>
