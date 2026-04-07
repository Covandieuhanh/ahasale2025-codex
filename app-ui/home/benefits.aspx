<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Hồ sơ quyền & lịch sử</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406g" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="account">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Home Benefit</span>
                        <h1 class="app-page-title">Hồ sơ quyền & lịch sử</h1>
                        <p class="app-page-copy">Tập trung toàn bộ 4 loại hồ sơ quyền trong Home để theo dõi quyền tiêu dùng, ưu đãi, hành vi lao động và chỉ số gắn kết trên giao diện app.</p>
                    </article>

                    <article class="app-page-card">
                        <h2>Các loại hồ sơ đang có</h2>
                        <p>Mỗi hồ sơ tương ứng với một màn lịch sử riêng trong app để theo dõi trạng thái, biến động và các nội dung liên quan.</p>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/home/benefit-consumer.aspx?ui_mode=app"><span>Hồ sơ quyền tiêu dùng<small>Lịch sử quyền tiêu dùng và trao đổi phát sinh</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/benefit-offers.aspx?ui_mode=app"><span>Hồ sơ quyền ưu đãi<small>Lịch sử quyền ưu đãi và mức kích hoạt</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/benefit-labor.aspx?ui_mode=app"><span>Hồ sơ hành vi lao động<small>Lịch sử hành vi và ghi nhận phát triển</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/benefit-engagement.aspx?ui_mode=app"><span>Hồ sơ chỉ số gắn kết<small>Lịch sử gắn kết và cập nhật theo hoạt động</small></span><span>&rsaquo;</span></a>
                        </div>
                    </article>

                    <article class="app-page-card">
                        <h2>Điểm vào liên quan</h2>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/home/profile.aspx?ui_mode=app"><span>Trang cá nhân<small>Quản lý tài khoản và nhóm hồ sơ quyền</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/exchange-history.aspx?ui_mode=app"><span>Lịch sử Trao đổi<small>Theo dõi lịch sử tương tác tổng hợp</small></span><span>&rsaquo;</span></a>
                            <a class="app-page-link" href="/app-ui/home/default.aspx?ui_mode=app"><span>Trang chủ Home<small>Quay lại không gian làm việc chính</small></span><span>&rsaquo;</span></a>
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
