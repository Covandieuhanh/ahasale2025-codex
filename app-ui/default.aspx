<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <meta name="format-detection" content="telephone=no" />
    <meta name="format-detection" content="date=no" />
    <meta name="format-detection" content="address=no" />
    <meta name="format-detection" content="email=no" />
    <title>AhaSale App UI - Chọn không gian</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-tokens.css" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-shell.css?v=20260406e" />
    <link rel="stylesheet" href="/app-ui/shared/app-ui-launcher.css" />
</head>
<body class="app-ui app-ui-launcher">
    <form id="form1" runat="server">
        <div class="app-shell app-shell-launcher" data-app-shell="launcher">
            <header class="app-topbar app-topbar-launcher">
                <div class="app-safe-top"></div>
                <div class="app-launcher-head">
                    <span class="app-launcher-kicker">AhaSale App UI</span>
                    <h1 class="app-launcher-title">Chọn không gian app độc lập để vào đúng luồng trải nghiệm</h1>
                    <p class="app-launcher-copy">Nhánh này dùng riêng cho app mode, không thay thế giao diện legacy đang chạy.</p>
                </div>
            </header>
            <main class="app-main">
                <section class="app-section">
                    <div class="app-launcher-insight-strip">
                        <article class="app-launcher-insight-card">
                            <span class="app-launcher-insight-value">5</span>
                            <span class="app-launcher-insight-label">Không gian app độc lập</span>
                        </article>
                        <article class="app-launcher-insight-card">
                            <span class="app-launcher-insight-value">100%</span>
                            <span class="app-launcher-insight-label">Không chạm giao diện legacy</span>
                        </article>
                        <article class="app-launcher-insight-card">
                            <span class="app-launcher-insight-value">1 luồng</span>
                            <span class="app-launcher-insight-label">Tập trung vào trải nghiệm mobile app</span>
                        </article>
                    </div>
                </section>
                <section class="app-section">
                    <div class="app-launcher-grid" data-role="launcher-grid">
                        <article class="app-launcher-card is-home">
                            <div class="app-launcher-row">
                                <span class="app-launcher-badge"><span class="app-skeleton-pill" style="width:24px;"></span></span>
                                <span class="app-launcher-arrow"><span class="app-skeleton-pill" style="width:16px;"></span></span>
                            </div>
                            <div class="app-skeleton-line" style="width:46%;height:18px;margin-top:18px;"></div>
                            <div class="app-skeleton-line" style="width:36%;margin-top:10px;"></div>
                            <div class="app-skeleton-line" style="width:86%;margin-top:14px;"></div>
                            <div class="app-skeleton-line" style="width:72%;margin-top:8px;"></div>
                        </article>
                    </div>
                </section>
            </main>
        </div>
    </form>
    <script src="/app-ui/shared/app-ui-registry.js"></script>
    <script src="/app-ui/shared/app-ui-mode.js"></script>
    <script src="/app-ui/shared/runtime-session.ashx"></script>
    <script src="/app-ui/shared/adapters/app-ui-session-adapter.js"></script>
    <script src="/app-ui/shared/adapters/app-ui-account-adapter.js?v=20260405c"></script>
    <script src="/app-ui/shared/app-ui-launcher.js"></script>
</body>
</html>
