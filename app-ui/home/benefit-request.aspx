<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Gửi yêu cầu ghi nhận</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406i" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-benefits.css?v=20260406b" />
</head>
<body class="app-ui app-ui-home">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="account">
            <main class="app-main">
                <section class="app-page-wrap app-benefit-page app-benefit-request-page" data-benefit-request-page="1">
                    <article class="app-page-hero app-benefit-hero">
                        <div class="app-benefit-hero-grid">
                            <div class="app-benefit-hero-copy">
                                <span class="app-page-kicker" data-benefit-request-kicker="1">Home Request</span>
                                <h1 class="app-page-title" data-benefit-request-title="1">Gửi yêu cầu ghi nhận</h1>
                                <p class="app-page-copy" data-benefit-request-copy="1">Màn hình app đang kiểm tra điều kiện hành vi trước khi gửi yêu cầu duyệt điểm.</p>
                            </div>
                            <div class="app-benefit-balance">
                                <span class="app-benefit-balance-label" data-benefit-request-balance-label="1">Số dư có thể gửi</span>
                                <strong class="app-benefit-balance-value" data-benefit-request-balance-value="1">...</strong>
                                <span class="app-benefit-balance-note" data-benefit-request-balance-note="1">Đang tải thông tin hành vi...</span>
                            </div>
                        </div>
                    </article>

                    <div class="app-benefit-notice" data-benefit-request-notice="1" hidden></div>

                    <section class="app-benefit-summary-grid" data-benefit-request-summary="1">
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                    </section>

                    <article class="app-page-card app-benefit-section" data-benefit-request-form-section="1">
                        <div class="app-benefit-section-head">
                            <h2>Biểu mẫu gửi yêu cầu</h2>
                            <span class="app-benefit-section-meta" data-benefit-request-form-meta="1">Đang tải...</span>
                        </div>
                        <div class="app-benefit-request-form" data-benefit-request-form="1">
                            <div class="app-benefit-request-field">
                                <span class="app-benefit-request-label">Hồ sơ</span>
                                <strong class="app-benefit-request-value" data-benefit-request-profile="1">...</strong>
                            </div>
                            <div class="app-benefit-request-field">
                                <span class="app-benefit-request-label">Hành vi</span>
                                <strong class="app-benefit-request-value" data-benefit-request-behavior="1">...</strong>
                            </div>
                            <div class="app-benefit-request-field">
                                <span class="app-benefit-request-label">Điều kiện</span>
                                <strong class="app-benefit-request-value" data-benefit-request-rule="1">...</strong>
                            </div>
                            <label class="app-benefit-request-input-group">
                                <span class="app-benefit-request-label">Số điểm muốn ghi nhận</span>
                                <input class="app-benefit-request-input" data-benefit-request-amount="1" type="text" inputmode="decimal" placeholder="Nhập số điểm" autocomplete="off" />
                            </label>
                            <div class="app-benefit-request-quick-actions" data-benefit-request-quick-actions="1"></div>
                            <p class="app-benefit-request-help" data-benefit-request-help="1">Điểm hợp lệ sẽ được đối chiếu theo mốc ngày, tuần hoặc tháng tương ứng với hành vi.</p>
                            <div class="app-page-actions">
                                <button class="app-page-btn is-primary" type="button" data-benefit-request-submit="1">Gửi yêu cầu</button>
                                <a class="app-page-btn" href="/app-ui/home/benefits.aspx?ui_mode=app" data-benefit-request-back="1">Quay lại hồ sơ</a>
                            </div>
                        </div>
                    </article>

                    <article class="app-page-card app-benefit-section" data-benefit-request-recent-section="1">
                        <div class="app-benefit-section-head">
                            <h2>Yêu cầu gần đây của hành vi này</h2>
                            <span class="app-benefit-section-meta" data-benefit-request-recent-meta="1">Đang tải...</span>
                        </div>
                        <div class="app-benefit-stack" data-benefit-request-recent="1">
                            <div class="app-benefit-history-card is-loading"></div>
                            <div class="app-benefit-history-card is-loading"></div>
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
    <script src="/app-ui/home/app-home-benefit-request.js?v=20260406a"></script>
</body>
</html>
