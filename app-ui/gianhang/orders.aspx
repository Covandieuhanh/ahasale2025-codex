<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>AhaSale App UI - Đơn bán / Đơn mua</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-pages.css" />
</head>
<body class="app-ui app-ui-gianhang">
    <form id="form1" runat="server">
        <div class="app-shell" data-app-shell="gianhang" data-app-nav-active="listing">
            <main class="app-main">
                <section class="app-page-wrap">
                    <article class="app-page-hero">
                        <span class="app-page-kicker">Aha Shop</span>
                        <h1 class="app-page-title">Đơn bán</h1>
                        <p class="app-page-copy">Theo dõi đơn, tạo đơn mới và chuyển nhanh sang luồng chờ thanh toán.</p>
                    </article><article class="app-page-card">
                        <h2>Điểm vào liên quan</h2>
                        <div class="app-page-links">
                            <a class="app-page-link" href="/app-ui/gianhang/default.aspx?ui_mode=app"><span>Aha Shop<small>Tổng quan người bán</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/gianhang/create-order.aspx?ui_mode=app"><span>Tạo đơn<small>Mở luồng tạo đơn</small></span><span>&rsaquo;</span></a><a class="app-page-link" href="/app-ui/gianhang/pending-payment.aspx?ui_mode=app"><span>Chờ thanh toán<small>Theo dõi đơn chờ xử lý</small></span><span>&rsaquo;</span></a>
                        </div>
                    </article>

                    <section data-role="gh-orders-runtime"></section>
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
    <script src="/app-ui/gianhang/app-gianhang-orders.js?v=20260404d"></script>
</body>
</html>
