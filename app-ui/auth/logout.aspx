<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Đăng xuất</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-shell="home">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Aha Sale</span>
                        <h1 class="app-page-title">Đang đăng xuất</h1>
                        <p class="app-page-copy">Hệ thống đang thoát phiên và chuyển về màn đăng nhập app.</p>
                    </article>
                </section>
            </main>
        </div>
    </form>

    <script>
        (function () {
            var fallback = "/app-ui/auth/login.aspx?ui_mode=app";
            var target = "/home/logout.aspx?return_url=" + encodeURIComponent(fallback);
            window.location.replace(target);
        })();
    </script>
</body>
</html>
