<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Hồ sơ quyền tiêu dùng</title>
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
                <section class="app-page-wrap app-benefit-page" data-app-benefit-page="1" data-benefit-profile="consumer">
                    <article class="app-page-hero app-benefit-hero">
                        <div class="app-benefit-hero-grid">
                            <div class="app-benefit-hero-copy">
                                <span class="app-page-kicker" data-benefit-kicker="1">Home Benefit</span>
                                <h1 class="app-page-title" data-benefit-title="1">Hồ sơ quyền tiêu dùng</h1>
                                <p class="app-page-copy" data-benefit-copy="1">Màn hình app đang tải lại lịch sử quyền tiêu dùng theo bố cục gọn hơn để đọc và thao tác thuận tiện trên app.</p>
                            </div>
                            <div class="app-benefit-balance" data-benefit-balance-card="1">
                                <span class="app-benefit-balance-label" data-benefit-balance-label="1">Số dư quyền tiêu dùng</span>
                                <strong class="app-benefit-balance-value" data-benefit-balance-value="1">...</strong>
                                <span class="app-benefit-balance-note" data-benefit-balance-note="1">Đang tải trạng thái hồ sơ...</span>
                            </div>
                        </div>
                    </article>

                    <div class="app-benefit-notice" data-benefit-notice="1" hidden></div>
                    <div class="app-benefit-filter-bar" data-benefit-filters="1" hidden></div>

                    <section class="app-benefit-summary-grid" data-benefit-summary="1">
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                        <article class="app-benefit-stat-card is-loading"></article>
                    </section>

                    <article class="app-page-card app-benefit-section" data-benefit-note-section="1" hidden>
                        <h2>Nguyên tắc ghi nhận</h2>
                        <p data-benefit-note="1"></p>
                    </article>

                    <article class="app-page-card app-benefit-section" data-benefit-behaviors-section="1" hidden>
                        <div class="app-benefit-section-head">
                            <h2>Nhóm hành vi</h2>
                            <span class="app-benefit-section-meta" data-benefit-behaviors-meta="1"></span>
                        </div>
                        <div class="app-benefit-stack" data-benefit-behaviors="1"></div>
                    </article>

                    <article class="app-page-card app-benefit-section" data-benefit-requests-section="1" hidden>
                        <div class="app-benefit-section-head">
                            <h2>Yêu cầu ghi nhận</h2>
                            <span class="app-benefit-section-meta" data-benefit-requests-meta="1"></span>
                        </div>
                        <div class="app-benefit-stack" data-benefit-requests="1"></div>
                    </article>

                    <article class="app-page-card app-benefit-section">
                        <div class="app-benefit-section-head">
                            <h2>Lịch sử hồ sơ</h2>
                            <span class="app-benefit-section-meta" data-benefit-history-meta="1">Đang tải...</span>
                        </div>
                        <div class="app-benefit-stack" data-benefit-history="1">
                            <div class="app-benefit-history-card is-loading"></div>
                            <div class="app-benefit-history-card is-loading"></div>
                        </div>
                        <div class="app-benefit-pager" data-benefit-history-pager="1" hidden></div>
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
    <script src="/app-ui/home/app-home-benefits.js?v=20260406a"></script>
</body>
</html>
