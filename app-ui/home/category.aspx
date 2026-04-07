<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Danh mục Home</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406compact3" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-results.css?v=20260406compact2" />
    <link rel="stylesheet" href="/app-ui/home/app-home.css?v=20260406compact8" />
</head>
<body class="app-ui app-ui-home app-ui-home-category">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-density="compact" data-app-shell="home" data-app-nav-active="categories">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero app-category-hero">
                        <div class="app-category-hero-grid">
                            <div class="app-category-hero-copy">
                                <span class="app-page-kicker" data-role="category-kicker">Danh mục Home</span>
                                <h1 class="app-page-title" data-role="category-title">Đang tải danh mục...</h1>
                                <p class="app-page-copy" data-role="category-copy">Màn hình app đang đồng bộ nội dung theo danh mục đã chọn.</p>
                            </div>
                            <span class="app-space-thumb app-home-category-thumb app-category-hero-thumb" data-role="category-thumb">
                                <span class="app-home-category-code" data-role="category-code">DM</span>
                            </span>
                        </div>
                        <div class="app-page-actions" data-role="category-actions">
                            <a class="app-page-btn is-primary" href="/app-ui/home/search.aspx?ui_mode=app">Mở tìm kiếm Home</a>
                            <a class="app-page-btn" href="/app-ui/home/categories.aspx?ui_mode=app">Toàn bộ danh mục</a>
                        </div>
                    </article>

                    <article class="app-page-card">
                        <h2>Tin mới trong danh mục</h2>
                        <p class="app-category-summary" data-role="category-summary">Đang tải dữ liệu danh mục...</p>
                        <div class="app-feed-grid app-category-result-grid" data-role="category-results">
                            <article class="app-skeleton-card">
                                <div class="app-skeleton-block app-skeleton-card-media"></div>
                                <div class="app-skeleton-card-body">
                                    <div class="app-skeleton-line"></div>
                                    <div class="app-skeleton-line"></div>
                                    <div class="app-skeleton-line"></div>
                                    <div class="app-skeleton-line"></div>
                                </div>
                            </article>
                            <article class="app-skeleton-card">
                                <div class="app-skeleton-block app-skeleton-card-media"></div>
                                <div class="app-skeleton-card-body">
                                    <div class="app-skeleton-line"></div>
                                    <div class="app-skeleton-line"></div>
                                    <div class="app-skeleton-line"></div>
                                    <div class="app-skeleton-line"></div>
                                </div>
                            </article>
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
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260406a"></script>
    <script src="/app-ui/shared/adapters/app-ui-search-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-listing-adapter.js?v=20260403d"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406i"></script>
    <script src="/app-ui/home/app-home-category.js?v=20260406b"></script>
</body>
</html>
