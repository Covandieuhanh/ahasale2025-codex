<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Trang cá nhân</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="account">
            <main class="app-main">
                <section class="app-page-wrap app-home-profile-layout" data-home-profile-page="1">
                    <article class="app-page-card app-home-profile-card" data-home-profile-card="1">
                        <div class="app-home-profile-head" data-home-profile-head="1">
                            <span class="app-account-avatar">AH</span>
                            <div class="app-home-profile-meta">
                                <strong class="app-home-profile-name">Tài khoản Home</strong>
                                <span class="app-home-profile-sub">Đang tải hồ sơ...</span>
                            </div>
                        </div>
                    </article>
                    <div class="app-home-profile-notice" data-home-profile-notice="1" hidden></div>
                    <article class="app-page-card app-home-profile-info-card" data-home-profile-info="1"></article>
                    <article class="app-page-card app-home-profile-links-card" data-home-profile-links="1"></article>
                </section>
            </main>
        </div>
    </form>

    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/home/app-home-profile.js?v=20260406i"></script>
</body>
</html>
