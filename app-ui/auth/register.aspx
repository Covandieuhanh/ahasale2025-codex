<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Đăng ký</title>
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
                    <h1 class="app-auth-title">Tạo tài khoản AhaSale</h1>
                    <p class="app-auth-copy">Đăng ký tài khoản Home để mở đầy đủ tính năng mua bán, tìm kiếm và vận hành gian hàng.</p>
                </section>

                <section class="app-auth-form-card">
                    <div class="app-auth-form-head">
                        <h2 class="app-auth-form-title">Thông tin đăng ký</h2>
                        <p class="app-auth-form-copy">Màn app thu thập thông tin cơ bản, xử lý tạo tài khoản thực hiện tại runtime.</p>
                    </div>
                    <div class="app-auth-field-grid">
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Họ và tên</span>
                            <input class="app-auth-input" data-role="auth-register-name" type="text" placeholder="VD: Nguyễn Văn A" autocomplete="name" />
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Số điện thoại</span>
                            <input class="app-auth-input" data-role="auth-register-phone" type="tel" placeholder="VD: 0326824915" autocomplete="tel" />
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Email (tuỳ chọn)</span>
                            <input class="app-auth-input" data-role="auth-register-email" type="email" placeholder="VD: ten@email.com" autocomplete="email" />
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Mật khẩu</span>
                            <span class="app-auth-password-wrap">
                                <input class="app-auth-input app-auth-input-password" data-role="auth-register-password" type="password" placeholder="Tạo mật khẩu" autocomplete="new-password" />
                                <button class="app-auth-password-toggle" data-role="auth-password-toggle" type="button" aria-label="Hiện mật khẩu">Hiện</button>
                            </span>
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Nhập lại mật khẩu</span>
                            <span class="app-auth-password-wrap">
                                <input class="app-auth-input app-auth-input-password" data-role="auth-register-password-confirm" type="password" placeholder="Nhập lại mật khẩu" autocomplete="new-password" />
                                <button class="app-auth-password-toggle" data-role="auth-password-toggle" type="button" aria-label="Hiện mật khẩu">Hiện</button>
                            </span>
                        </label>
                    </div>
                    <p class="app-auth-form-note">Bằng việc tiếp tục, bạn đồng ý dùng tài khoản chung cho Home, Aha Land, Aha Xe, Aha Tech và Aha Shop.</p>
                </section>

                <section class="app-auth-card">
                    <ul class="app-auth-list">
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Một tài khoản dùng chung cho Home, Aha Land, Aha Xe, Aha Tech và Gian hàng.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Sau khi đăng ký, bạn có thể gửi yêu cầu mở gian hàng ngay trong app.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Luồng đăng ký dùng trực tiếp runtime hiện tại để đảm bảo dữ liệu thật.</span></li>
                    </ul>
                </section>

                <section class="app-auth-actions">
                    <a class="app-auth-btn app-auth-btn-primary" data-role="auth-register-main" href="/app-ui/auth/register.aspx?ui_mode=app">Đăng ký tài khoản</a>
                    <a class="app-auth-btn app-auth-btn-secondary" data-role="auth-login-link" href="/app-ui/auth/login.aspx?ui_mode=app">Đã có tài khoản? Đăng nhập</a>
                    <div class="app-auth-links">
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
