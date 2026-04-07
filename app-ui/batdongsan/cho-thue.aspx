<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Cho thuê bất động sản</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-batdongsan">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-shell="batdongsan" data-app-nav-active="categories">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Aha Land</span>
                        <h1 class="app-page-title">Cho thuê bất động sản</h1>
                        <p class="app-page-copy">Nhóm tin thuê nhà, căn hộ, mặt bằng theo khu vực.</p>
                    </article><article class="app-page-card">
                        <h2>Điểm vào liên quan</h2>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/batdongsan/default.aspx?ui_mode=app"><span>Aha Land<small>Trang chính Bất động sản</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/batdongsan/hub.aspx?ui_mode=app"><span>Danh mục BDS<small>Các nhánh mua bán/cho thuê</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/batdongsan/search.aspx?ui_mode=app"><span>Tìm kiếm BDS<small>Kết quả theo bộ lọc</small></span><span>&rsaquo;</span></a>
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
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
    <script src="/app-ui/shared/app-ui-runtime-page-bridge.js?v=20260404d"></script>
</body>
</html>
