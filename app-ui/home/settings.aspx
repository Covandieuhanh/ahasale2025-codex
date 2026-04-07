<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Cài đặt tài khoản</title>
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
                <section class="app-page-wrap app-home-settings-layout" data-home-settings-page="1">
                    <article class="app-page-card app-home-settings-card">
                        <h1 class="app-page-title">Cài đặt tài khoản</h1>
                        <p class="app-page-copy">Chỉnh sửa thông tin hồ sơ Home cho giao diện app.</p>
                        <div class="app-home-settings-user" data-home-settings-user="1">
                            <span class="app-account-avatar">AH</span>
                            <div class="app-home-settings-user-meta">
                                <strong>Tài khoản Home</strong>
                                <span>Đang tải thông tin...</span>
                            </div>
                        </div>
                    </article>

                    <div class="app-home-settings-notice" data-home-settings-notice="1" hidden></div>

                    <article class="app-page-card app-home-settings-form-card">
                        <div class="app-home-settings-field-grid">
                            <label class="app-home-settings-field">
                                <span class="app-home-settings-label">Họ và tên</span>
                                <input class="app-home-settings-input" type="text" maxlength="120" data-home-settings-input="display_name" placeholder="Nhập họ và tên" />
                            </label>
                            <label class="app-home-settings-field">
                                <span class="app-home-settings-label">Số điện thoại liên hệ</span>
                                <input class="app-home-settings-input" type="text" maxlength="30" data-home-settings-input="phone" placeholder="Nhập số điện thoại" />
                            </label>
                            <label class="app-home-settings-field">
                                <span class="app-home-settings-label">Email</span>
                                <input class="app-home-settings-input" type="email" maxlength="120" data-home-settings-input="email" placeholder="Nhập email" />
                            </label>
                            <label class="app-home-settings-field">
                                <span class="app-home-settings-label">Địa chỉ</span>
                                <input class="app-home-settings-input" type="text" maxlength="220" data-home-settings-input="address" placeholder="Nhập địa chỉ" />
                            </label>
                            <label class="app-home-settings-field">
                                <span class="app-home-settings-label">Giới thiệu ngắn</span>
                                <textarea class="app-home-settings-input app-home-settings-textarea" maxlength="500" data-home-settings-input="intro" placeholder="Giới thiệu ngắn về bạn"></textarea>
                            </label>
                        </div>
                        <div class="app-home-settings-actions">
                            <button class="app-page-btn is-primary" type="button" data-home-settings-action="save">Lưu thay đổi</button>
                            <a class="app-page-btn" href="/app-ui/home/profile.aspx?ui_mode=app">Quay lại trang cá nhân</a>
                        </div>
                    </article>

                    <article class="app-page-card app-home-settings-links-card">
                        <h2>Bio Link</h2>
                        <p>Thêm liên kết cá nhân để hiển thị trên trang cá nhân app.</p>
                        <div class="app-home-settings-link-form">
                            <label class="app-home-settings-field">
                                <span class="app-home-settings-label">Tên liên kết</span>
                                <input class="app-home-settings-input" type="text" maxlength="80" data-home-settings-link-input="title" placeholder="Ví dụ: Facebook cá nhân" />
                            </label>
                            <label class="app-home-settings-field">
                                <span class="app-home-settings-label">URL</span>
                                <input class="app-home-settings-input" type="url" maxlength="500" data-home-settings-link-input="url" placeholder="https://..." />
                            </label>
                            <button class="app-page-btn is-primary" type="button" data-home-settings-action="add-link">Thêm link</button>
                        </div>
                        <div class="app-home-settings-link-list" data-home-settings-link-list="1"></div>
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
    <script src="/app-ui/home/app-home-settings.js?v=20260405b"></script>
</body>
</html>
