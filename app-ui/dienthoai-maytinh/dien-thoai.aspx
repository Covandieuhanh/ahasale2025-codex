<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Điện thoại</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-dienthoai-maytinh">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-shell="dienthoai-maytinh" data-app-nav-active="categories">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Aha Tech</span>
                        <h1 class="app-page-title">Điện thoại</h1>
                        <p class="app-page-copy">Nhóm tin điện thoại theo model, tình trạng và giá bán.</p>
                    </article><article class="app-page-card">
                        <h2>Điểm vào liên quan</h2>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/dienthoai-maytinh/default.aspx?ui_mode=app"><span>Aha Tech<small>Trang chính Công nghệ</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/dienthoai-maytinh/hub.aspx?ui_mode=app"><span>Danh mục Tech<small>Điện thoại, máy tính, phụ kiện</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app"><span>Tìm kiếm Tech<small>Kết quả theo model</small></span><span>&rsaquo;</span></a>
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
