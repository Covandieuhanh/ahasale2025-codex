<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Trao đổi</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406compact3" />
    <link rel="stylesheet" href="/app-ui/home/app-home.css?v=20260406compact7" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="home">
            <header class="app-topbar">
                <div class="app-safe-top"></div>
                <div class="app-topbar-row">
                    <button class="app-icon-btn" type="button" aria-label="Mở menu" data-app-trigger="menu">
                        <span></span><span></span><span></span>
                    </button>
                    <div class="app-search-pill"><span class="app-search-icon"></span><span class="app-search-placeholder">Trao đổi của bạn</span></div>
                    <button class="app-icon-btn app-icon-heart" type="button" aria-label="Tin đã lưu"></button>
                    <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo"><span class="app-badge">5</span></button>
                    <button class="app-icon-btn app-icon-user" type="button" aria-label="Tài khoản" data-app-trigger="account"></button>
                </div>
                <div class="app-chip-lane" data-role="recent-searches" data-app-capability="show_home_inbox"></div>
            </header>

            <main class="app-main">
                <section class="app-section" data-app-capability="show_home_inbox">
                    <div class="app-home-panel">
                        <span class="app-home-panel-kicker">Trung tâm trao đổi</span>
                        <h1 class="app-home-panel-title">Theo dõi toàn bộ trao đổi theo luồng app</h1>
                        <p class="app-home-panel-copy">Màn này giữ nhịp hội thoại tập trung và chuẩn bị sẵn cho kết nối realtime trong bước tiếp theo.</p>
                    </div>
                </section>

                <section class="app-section" data-app-capability="show_home_inbox">
                    <div class="app-section-head">
                        <div>
                            <h2 class="app-section-title">Danh sách trao đổi</h2>
                            <p class="app-section-subtitle" data-role="inbox-summary"></p>
                        </div>
                        <a class="app-ghost-btn" href="/app-ui/home/saved-manager.aspx?ui_mode=app">Mở quản lý đã lưu</a>
                    </div>
                    <div class="app-home-thread-list" data-role="inbox-list"></div>
                </section>
            </main>

            <nav class="app-bottom-nav" aria-label="Điều hướng chính" data-app-capability="show_home_inbox">
                <a class="app-bottom-item" href="/app-ui/home/default.aspx">
                    <span class="app-bottom-icon app-bottom-home"></span>
                    <span>Trang chủ</span>
                </a>
                <a class="app-bottom-item" href="/app-ui/home/saved.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-tag"></span>
                    <span>Đã lưu</span>
                </a>
                <a class="app-post-cta" href="/app-ui/gianhang/default.aspx?ui_mode=app" aria-label="Đăng tin">+</a>
                <a class="app-bottom-item is-active" href="/app-ui/home/exchange.aspx?ui_mode=app">
                    <span class="app-bottom-icon app-bottom-chat"></span>
                    <span>Liên hệ</span>
                </a>
                <button class="app-bottom-item" type="button" data-app-trigger="account">
                    <span class="app-bottom-icon app-bottom-user"></span>
                    <span>Tài khoản</span>
                </button>
            </nav>
        </div>
    </form>

    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/adapters/app-ui-seller-adapter.js"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/home/app-home-inbox.js?v=20260404a"></script>
</body>
</html>
