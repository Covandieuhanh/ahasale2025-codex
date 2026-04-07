<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Đăng nhập</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/auth/app-auth.css" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-shell="home" data-app-nav-active="account">
            <main class="app-main app-auth-main">
                <section class="app-auth-hero">
                    <span class="app-auth-kicker">Aha Sale App</span>
                    <h1 class="app-auth-title">Đăng nhập tài khoản Home</h1>
                    <p class="app-auth-copy">Bạn cần đăng nhập để lưu tin, đăng tin và mở luồng gian hàng ngay trong app.</p>
                </section>

                <section class="app-auth-form-card">
                    <div class="app-auth-form-head">
                        <h2 class="app-auth-form-title">Thông tin đăng nhập</h2>
                        <p class="app-auth-form-copy">Điền thông tin để tiếp tục. Luồng xác thực vẫn dùng runtime hiện tại.</p>
                    </div>
                    <div class="app-auth-field-grid">
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Số điện thoại hoặc email</span>
                            <input class="app-auth-input" data-role="auth-login-identifier" type="text" placeholder="VD: 0326824915 hoặc ten@email.com" autocomplete="username" />
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Mật khẩu</span>
                            <span class="app-auth-password-wrap">
                                <input class="app-auth-input app-auth-input-password" data-role="auth-login-password" type="password" placeholder="Nhập mật khẩu" autocomplete="current-password" />
                                <button class="app-auth-password-toggle" data-role="auth-password-toggle" type="button" aria-label="Hiện mật khẩu">Hiện</button>
                            </span>
                        </label>
                    </div>
                    <div class="app-auth-inline-row">
                        <label class="app-auth-check">
                            <input data-role="auth-remember" type="checkbox" />
                            <span>Giữ đăng nhập trên thiết bị này</span>
                        </label>
                        <a class="app-auth-link" data-role="auth-forgot-link-inline" href="/app-ui/auth/forgot-password.aspx?ui_mode=app">Quên mật khẩu?</a>
                    </div>
                    <p class="app-auth-feedback" data-role="auth-login-feedback" aria-live="polite"></p>
                    <p class="app-auth-form-note">Mật khẩu không lưu tại giao diện app. Xác thực và bảo mật do runtime xử lý.</p>
                </section>

                <section class="app-auth-card">
                    <ul class="app-auth-list">
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Giữ nguyên luồng app UI độc lập, không bị đứt nhịp điều hướng.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Sau đăng nhập sẽ quay lại đúng màn bạn đang thao tác.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Dùng chung tài khoản Home của nền tảng hiện tại.</span></li>
                    </ul>
                </section>

                <section class="app-auth-actions">
                    <a class="app-auth-btn app-auth-btn-primary" data-role="auth-login-main" href="/app-ui/auth/login.aspx?ui_mode=app">Đăng nhập ngay</a>
                    <a class="app-auth-btn app-auth-btn-secondary" data-role="auth-register-link" href="/app-ui/auth/register.aspx?ui_mode=app">Chưa có tài khoản? Đăng ký</a>
                    <div class="app-auth-links">
                        <a class="app-auth-link" data-role="auth-forgot-link" href="/app-ui/auth/forgot-password.aspx?ui_mode=app">Quên mật khẩu</a>
                        <a class="app-auth-link" data-role="auth-open-shop-link" href="/app-ui/auth/open-shop.aspx?ui_mode=app">Muốn đăng tin? Mở gian hàng</a>
                        <a class="app-auth-link" data-role="auth-app-back" href="/app-ui/home/default.aspx?ui_mode=app">Quay lại app</a>
                    </div>
                </section>
            </main>
        </div>
    </form>

    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260406a"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
    <script src="/app-ui/auth/app-auth.js?v=20260406a"></script>
</body>
</html>
