<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Danh mục Home</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406compact3" />
    <link rel="stylesheet" href="/app-ui/home/app-home.css?v=20260406compact9" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="categories">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Home Space</span>
                        <h1 class="app-page-title">Danh mục Home</h1>
                        <p class="app-page-copy">Hiển thị đầy đủ các danh mục đang có trên desktop để người dùng app vào nhanh theo cùng hệ thống phân loại.</p>
                    </article>
                    <section class="app-section app-space-rail">
                        <div class="app-space-rail-track" data-role="space-categories"></div>
                    </section>
                    <article class="app-page-card">
                        <h2>Điểm vào liên quan</h2>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/home/default.aspx?ui_mode=app"><span>Trang chủ Home<small>Discovery tổng hợp</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/home/search.aspx?ui_mode=app"><span>Tìm kiếm Home<small>Feed và bộ lọc</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/default.aspx?ui_mode=app"><span>Chuyển không gian<small>Mở launcher app</small></span><span>&rsaquo;</span></a>
                        </div>
                    </article>
                </section>
            </main>
        </div>
    </form>

    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260406a"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/shared/app-ui-runtime-page-bridge.js?v=20260406a"></script>
    <script src="/app-ui/home/app-home.js?v=20260406compact12"></script>
</body>
</html>
