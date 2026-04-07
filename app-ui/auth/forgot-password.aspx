<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Quên mật khẩu</title>
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
                    <h1 class="app-auth-title">Khôi phục mật khẩu</h1>
                    <p class="app-auth-copy">Màn này giữ luồng app liên tục, sau đó chuyển về đúng màn khôi phục của runtime hiện tại.</p>
                </section>

                <section class="app-auth-form-card">
                    <div class="app-auth-form-head">
                        <h2 class="app-auth-form-title">Xác minh tài khoản</h2>
                        <p class="app-auth-form-copy">Nhập số điện thoại hoặc email đã đăng ký để tiếp tục sang bước khôi phục runtime.</p>
                    </div>
                    <div class="app-auth-field-grid">
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Số điện thoại hoặc email</span>
                            <input class="app-auth-input" data-role="auth-forgot-identifier" type="text" placeholder="VD: 0326824915 hoặc ten@email.com" autocomplete="username" />
                        </label>
                    </div>
                    <p class="app-auth-form-note">Sau khi chuyển sang runtime, bạn sẽ nhận OTP/đường dẫn khôi phục theo quy trình bảo mật hiện tại.</p>
                </section>

                <section class="app-auth-card">
                    <ul class="app-auth-list">
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Dùng email/số điện thoại đã đăng ký tài khoản Home.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Sau khi đặt lại mật khẩu thành công, quay lại đăng nhập để tiếp tục trong app.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Không thay đổi logic xác thực của nền tảng hiện tại.</span></li>
                    </ul>
                </section>

                <section class="app-auth-actions">
                    <a class="app-auth-btn app-auth-btn-primary" data-role="auth-forgot-main" href="/app-ui/auth/forgot-password.aspx?ui_mode=app">Đến trang khôi phục</a>
                    <a class="app-auth-btn app-auth-btn-secondary" data-role="auth-login-link" href="/app-ui/auth/login.aspx?ui_mode=app">Quay lại đăng nhập</a>
                    <div class="app-auth-links">
                        <a class="app-auth-link" data-role="auth-register-link" href="/app-ui/auth/register.aspx?ui_mode=app">Chưa có tài khoản? Đăng ký</a>
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
