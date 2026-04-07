<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Tạo đơn gian hàng</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-gianhang">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-shell="gianhang" data-app-nav-active="post">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Aha Shop</span>
                        <h1 class="app-page-title">Tạo đơn</h1>
                        <p class="app-page-copy">Khởi tạo đơn bán và chuyển sang bước chờ thanh toán ngay trên app.</p>
                    </article>

                    <article class="app-page-card">
                        <h2>Thao tác nhanh</h2>
                        <div class="app-page-actions">
                            <a class="app-page-btn is-primary" href="/app-ui/gianhang/orders.aspx?ui_mode=app&open=create">Bắt đầu tạo đơn</a>
                            <a class="app-page-btn" href="/app-ui/gianhang/orders.aspx?ui_mode=app">Danh sách đơn bán</a>
                            <a class="app-page-btn" href="/app-ui/gianhang/pending-payment.aspx?ui_mode=app">Đi tới chờ thanh toán</a>
                        </div>
                    </article>

                    <section data-role="gh-create-order-runtime"></section>
                </section>
            </main>
        </div>
    </form>

    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260404f"></script>
    <script src="/app-ui/shared/adapters/app-ui-seller-adapter.js"></script>
    <script src="/app-ui/shared/app-ui-shell.js?v=20260406g"></script>
    <script src="/app-ui/shared/app-ui-runtime-page-bridge.js?v=20260404d"></script>
    <script src="/app-ui/gianhang/app-gianhang-create-order.js?v=20260404b"></script>
</body>
</html>
