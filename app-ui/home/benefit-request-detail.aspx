<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Chi tiết yêu cầu</title>
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
                <section class="app-page-wrap app-benefit-page" data-benefit-request-detail-page="1">
                    <article class="app-page-hero app-benefit-hero">
                        <div class="app-benefit-hero-grid">
                            <div class="app-benefit-hero-copy">
                                <span class="app-page-kicker">Home Request</span>
                                <h1 class="app-page-title" data-benefit-detail-title="1">Chi tiết yêu cầu</h1>
                                <p class="app-page-copy" data-benefit-detail-copy="1">Đang tải chi tiết yêu cầu ghi nhận trên app.</p>
                            </div>
                            <div class="app-benefit-balance">
                                <span class="app-benefit-balance-label">Trạng thái hiện tại</span>
                                <strong class="app-benefit-balance-value" data-benefit-detail-status="1">...</strong>
                                <span class="app-benefit-balance-note" data-benefit-detail-short-id="1">Đang tải mã yêu cầu...</span>
                            </div>
                        </div>
                    </article>

                    <div class="app-benefit-notice" data-benefit-detail-notice="1" hidden></div>

                    <section class="app-benefit-summary-grid" data-benefit-detail-summary="1">
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                    </section>

                    <article class="app-page-card app-benefit-section" data-benefit-detail-main="1">
                        <div class="app-benefit-section-head">
                            <h2>Thông tin yêu cầu</h2>
                        </div>
                        <div class="app-benefit-request-form" data-benefit-detail-fields="1"></div>
                        <div class="app-page-actions" data-benefit-detail-actions="1"></div>
                    </article>

                    <article class="app-page-card app-benefit-section" data-benefit-detail-timeline-section="1">
                        <div class="app-benefit-section-head">
                            <h2>Diễn biến xử lý</h2>
                        </div>
                        <div class="app-benefit-stack" data-benefit-detail-timeline="1"></div>
                    </article>

                    <article class="app-page-card app-benefit-section" data-benefit-detail-history-section="1">
                        <div class="app-benefit-section-head">
                            <h2>Biến động liên quan</h2>
                        </div>
                        <div class="app-benefit-stack" data-benefit-detail-history="1"></div>
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
    <script src="/app-ui/home/app-home-benefit-request-detail.js?v=20260406a"></script>
</body>
</html>
