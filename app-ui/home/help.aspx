<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Trợ giúp</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406compact3" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="account">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Hỗ trợ Home</span>
                        <h1 class="app-page-title">Trợ giúp</h1>
                        <p class="app-page-copy">Chọn nhanh mục cần hỗ trợ để tiếp tục thao tác trong app.</p>
                    </article>

                    <article class="app-page-card">
                        <h2>Hướng dẫn nhanh</h2>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/home/profile.aspx?ui_mode=app"><span>Trang cá nhân<small>Cập nhật hồ sơ và thông tin tài khoản</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/change-password.aspx?ui_mode=app"><span>Đổi mật khẩu<small>Bảo mật tài khoản khi cần</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/change-pin.aspx?ui_mode=app"><span>Đổi mã pin<small>Thay mã pin thẻ trong app</small></span><span>&rsaquo;</span></a>
                        </div>
                    </article>

                    <article class="app-page-card">
                        <h2>Liên hệ hỗ trợ</h2>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/home/feedback.aspx?ui_mode=app"><span>Đóng góp ý kiến<small>Gửi phản hồi trực tiếp tới nền tảng</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/default.aspx?ui_mode=app"><span>Quay về Trang chủ Home<small>Tiếp tục hành trình duyệt tin</small></span><span>&rsaquo;</span></a>
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
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/shared/app-ui-runtime-page-bridge.js?v=20260404d"></script>
</body>
</html>
