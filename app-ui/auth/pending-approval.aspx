<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Chờ duyệt gian hàng</title>
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
                    <h1 class="app-auth-title">Yêu cầu gian hàng đang chờ duyệt</h1>
                    <p class="app-auth-copy">Bạn đã gửi yêu cầu mở gian hàng. Khi được duyệt, nút Đăng tin sẽ mở thẳng màn tạo tin seller.</p>
                </section>

                <section class="app-auth-form-card">
                    <div class="app-auth-form-head">
                        <h2 class="app-auth-form-title">Tiến độ duyệt gian hàng</h2>
                        <p class="app-auth-form-copy">Theo dõi tiến độ trực tiếp trong app trước khi chuyển sang runtime để thao tác chi tiết.</p>
                    </div>
                    <div class="app-auth-stepper app-auth-stepper-pending">
                        <div class="app-auth-step is-done"><span>1</span><strong>Đã gửi yêu cầu</strong></div>
                        <div class="app-auth-step is-active"><span>2</span><strong>Đang thẩm định</strong></div>
                        <div class="app-auth-step"><span>3</span><strong>Kích hoạt seller</strong></div>
                    </div>
                    <div class="app-auth-mini-grid">
                        <div class="app-auth-mini-item">
                            <span>Thời gian dự kiến</span>
                            <strong>24-72 giờ làm việc</strong>
                        </div>
                        <div class="app-auth-mini-item">
                            <span>Cập nhật gần nhất</span>
                            <strong data-role="auth-pending-updated">Đang đồng bộ...</strong>
                        </div>
                    </div>
                    <p class="app-auth-form-note">Trong lúc chờ duyệt, bạn vẫn dùng đầy đủ các luồng xem tin, tìm kiếm và lưu tin trong app.</p>
                </section>

                <section class="app-auth-card" data-role="auth-pending-session-card">
                    <h2 class="app-auth-form-title">Trạng thái tài khoản hiện tại</h2>
                    <p class="app-auth-form-copy" data-role="auth-pending-session-copy">Đang kiểm tra phiên đăng nhập...</p>
                    <div class="app-auth-mini-grid">
                        <div class="app-auth-mini-item">
                            <span>Tài khoản</span>
                            <strong data-role="auth-pending-user">Khách</strong>
                        </div>
                        <div class="app-auth-mini-item">
                            <span>Gian hàng</span>
                            <strong data-role="auth-pending-status">Đang chờ duyệt</strong>
                        </div>
                    </div>
                </section>

                <section class="app-auth-card">
                    <ul class="app-auth-list">
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Theo dõi lịch sử duyệt và ghi chú tại màn hồ sơ chỉ số gắn kết.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Nếu cần bổ sung hồ sơ, bạn có thể gửi lại từ màn mở gian hàng.</span></li>
                        <li class="app-auth-list-item"><span class="app-auth-dot"></span><span>Trong thời gian chờ duyệt, bạn vẫn dùng đầy đủ các không gian xem tin.</span></li>
                    </ul>
                </section>

                <section class="app-auth-actions">
                    <a class="app-auth-btn app-auth-btn-primary" data-role="auth-pending-main" href="/app-ui/gianhang/status.aspx?ui_mode=app">Xem trạng thái duyệt</a>
                    <a class="app-auth-btn app-auth-btn-secondary" data-role="auth-open-shop-link" href="/app-ui/auth/open-shop.aspx?ui_mode=app">Chỉnh sửa / gửi lại yêu cầu</a>
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
