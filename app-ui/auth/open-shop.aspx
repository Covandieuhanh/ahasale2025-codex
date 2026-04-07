<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Mở gian hàng</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/auth/app-auth.css" />
</head>
<body class="app-ui app-ui-gianhang">
    <form id="form1" runat="server">
        <div class="app-shell app-shell-gianhang" data-app-shell="gianhang" data-app-nav-active="post">
            <main class="app-main app-auth-main">
                <section class="app-auth-hero">
                    <span class="app-auth-kicker">Aha Shop</span>
                    <h1 class="app-auth-title">Mở gian hàng để đăng tin</h1>
                    <p class="app-auth-copy">Tài khoản của bạn chưa có quyền gian hàng. Hãy gửi yêu cầu mở gian hàng để bật luồng đăng tin seller.</p>
                </section>

                <section class="app-auth-form-card">
                    <div class="app-auth-form-head">
                        <h2 class="app-auth-form-title">Thông tin gian hàng</h2>
                        <p class="app-auth-form-copy">Màn app chuẩn bị thông tin, còn quy trình duyệt và tạo gian hàng xử lý tại runtime.</p>
                    </div>
                    <div class="app-auth-field-grid">
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Tên gian hàng dự kiến</span>
                            <input class="app-auth-input" data-role="auth-open-shop-name" type="text" placeholder="VD: Aha Land Premium" />
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Nhóm ngành chính</span>
                            <input class="app-auth-input" data-role="auth-open-shop-category" type="text" placeholder="VD: Bất động sản / Xe / Công nghệ" />
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Khu vực hoạt động</span>
                            <input class="app-auth-input" data-role="auth-open-shop-location" type="text" placeholder="VD: TP.HCM, Hà Nội..." />
                        </label>
                        <label class="app-auth-field">
                            <span class="app-auth-field-label">Mô tả ngắn</span>
                            <textarea class="app-auth-input app-auth-textarea" data-role="auth-open-shop-note" placeholder="Giới thiệu ngắn về gian hàng và sản phẩm/dịch vụ chính..."></textarea>
                        </label>
                    </div>
                    <div class="app-auth-stepper">
                        <div class="app-auth-step is-active"><span>1</span><strong>Gửi yêu cầu</strong></div>
                        <div class="app-auth-step"><span>2</span><strong>Chờ duyệt</strong></div>
                        <div class="app-auth-step"><span>3</span><strong>Kích hoạt đăng tin</strong></div>
                    </div>
                    <p class="app-auth-form-note">Khi duyệt thành công, nút Đăng tin ở mọi không gian sẽ mở thẳng luồng seller runtime.</p>
                </section>

                <section class="app-auth-card" data-role="auth-open-shop-session-card">
                    <h2 class="app-auth-form-title">Trạng thái tài khoản hiện tại</h2>
                    <p class="app-auth-form-copy" data-role="auth-open-shop-session-copy">Đang kiểm tra phiên đăng nhập...</p>
                    <div class="app-auth-mini-grid">
                        <div class="app-auth-mini-item">
                            <span>Tài khoản</span>
                            <strong data-role="auth-open-shop-user">Khách</strong>
                        </div>
                        <div class="app-auth-mini-item">
                            <span>Gian hàng</span>
                            <strong data-role="auth-open-shop-status">Chưa đăng ký</strong>
                        </div>
                    </div>
                </section>

                <section class="app-auth-card">
                    <ul class="app-auth-list">
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Điền thông tin gian hàng theo form runtime hiện tại.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Sau khi admin duyệt, nút Đăng tin sẽ vào thẳng màn tạo tin.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Trạng thái duyệt có thể theo dõi ngay trong app.</span></li>
                    </ul>
                </section>

                <section class="app-auth-actions">
                    <a class="app-auth-btn app-auth-btn-primary" data-role="auth-open-shop-main" href="/app-ui/auth/pending-approval.aspx?ui_mode=app">Gửi yêu cầu mở gian hàng</a>
                    <a class="app-auth-btn app-auth-btn-secondary" data-role="auth-pending-link" href="/app-ui/auth/pending-approval.aspx?ui_mode=app">Tôi đã gửi, xem trạng thái duyệt</a>
                    <div class="app-auth-links">
                        <a class="app-auth-link" data-role="auth-login-link" href="/app-ui/auth/login.aspx?ui_mode=app&intent=post">Đổi tài khoản</a>
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
