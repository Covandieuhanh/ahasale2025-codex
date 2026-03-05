<%@ Page Language="C#" AutoEventWireup="true" CodeFile="public.aspx.cs" Inherits="shop_public" %>
<%@ Import Namespace="System.Web" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trang công khai gian hàng</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root {
            --shop-bg: #f4f8fc;
            --shop-card: #ffffff;
            --shop-line: #d8e3f0;
            --shop-text: #15314f;
            --shop-muted: #5b738d;
            --shop-brand: #0f4c81;
            --shop-brand-2: #0e8e5b;
        }

        * { box-sizing: border-box; }

        body {
            margin: 0;
            font-family: "Nunito Sans", "Segoe UI", Arial, sans-serif;
            color: var(--shop-text);
            background:
                radial-gradient(920px 280px at 18% -4%, rgba(15, 76, 129, .12), transparent 62%),
                radial-gradient(920px 320px at 88% -6%, rgba(14, 142, 91, .16), transparent 62%),
                var(--shop-bg);
        }

        a { text-decoration: none; color: inherit; }

        .shell {
            width: min(1220px, 100% - 28px);
            margin: 18px auto 28px;
        }

        .hero {
            border: 1px solid var(--shop-line);
            border-radius: 20px;
            overflow: hidden;
            background: var(--shop-card);
            box-shadow: 0 18px 44px rgba(16, 42, 67, .10);
        }

        .hero-top {
            background: linear-gradient(130deg, var(--shop-brand), var(--shop-brand-2));
            color: #fff;
            padding: 16px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .hero-title {
            margin: 0;
            font-size: 24px;
            line-height: 1.2;
            font-weight: 900;
        }

        .hero-sub {
            margin-top: 4px;
            font-size: 13px;
            color: rgba(255, 255, 255, .88);
        }

        .btn-login {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            border-radius: 12px;
            padding: 0 14px;
            font-weight: 800;
            background: #fff;
            color: var(--shop-brand);
            border: 1px solid rgba(255, 255, 255, .65);
        }

        .hero-main {
            padding: 18px;
            display: grid;
            grid-template-columns: 98px 1fr;
            gap: 16px;
            align-items: center;
            border-bottom: 1px solid #edf2f8;
        }

        .shop-avatar {
            width: 98px;
            height: 98px;
            border-radius: 50%;
            object-fit: cover;
            border: 3px solid #dbe8f6;
            background: #ecf3fb;
        }

        .meta-title {
            margin: 0;
            font-size: 30px;
            line-height: 1.12;
            font-weight: 900;
        }

        .meta-line {
            margin-top: 6px;
            color: var(--shop-muted);
            font-size: 14px;
        }

        .stats {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            padding: 16px 18px 18px;
        }

        .stat {
            border: 1px solid var(--shop-line);
            border-radius: 14px;
            background: #f9fbfe;
            padding: 12px 14px;
            min-height: 82px;
        }

        .stat-label {
            font-size: 13px;
            color: var(--shop-muted);
        }

        .stat-value {
            margin-top: 4px;
            font-size: 30px;
            line-height: 1.1;
            font-weight: 900;
            color: var(--shop-text);
        }

        .products {
            margin-top: 16px;
            border: 1px solid var(--shop-line);
            border-radius: 20px;
            background: var(--shop-card);
            box-shadow: 0 16px 32px rgba(16, 42, 67, .08);
            overflow: hidden;
        }

        .products-head {
            padding: 16px 18px;
            border-bottom: 1px solid #edf2f8;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .products-title {
            margin: 0;
            font-size: 24px;
            line-height: 1.2;
            font-weight: 900;
        }

        .products-sub {
            color: var(--shop-muted);
            font-size: 13px;
            margin-top: 4px;
        }

        .public-url {
            color: var(--shop-muted);
            font-size: 12px;
            font-weight: 700;
            background: #f7fbff;
            border: 1px solid var(--shop-line);
            border-radius: 999px;
            padding: 7px 12px;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 14px;
            padding: 14px;
        }

        .card {
            display: block;
            border: 1px solid var(--shop-line);
            border-radius: 14px;
            overflow: hidden;
            background: #fff;
            transition: transform .18s ease, box-shadow .18s ease;
        }

        .card:hover {
            transform: translateY(-2px);
            box-shadow: 0 12px 24px rgba(16, 42, 67, .16);
        }

        .card-image {
            width: 100%;
            aspect-ratio: 16/10;
            object-fit: cover;
            background: #edf4fc;
            display: block;
        }

        .card-body {
            padding: 11px 12px 12px;
        }

        .card-name {
            margin: 0;
            font-size: 16px;
            line-height: 1.28;
            font-weight: 800;
            color: var(--shop-text);
            min-height: 42px;
        }

        .card-meta {
            margin-top: 6px;
            color: var(--shop-muted);
            font-size: 13px;
        }

        .card-price {
            margin-top: 8px;
            color: #0b7a4d;
            font-size: 20px;
            font-weight: 900;
            line-height: 1.2;
        }

        .empty {
            padding: 26px 16px 30px;
            text-align: center;
            color: var(--shop-muted);
            font-weight: 700;
        }

        @media (max-width: 960px) {
            .stats { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .meta-title { font-size: 26px; }
        }

        @media (max-width: 680px) {
            .shell { width: min(1220px, 100% - 16px); margin-top: 10px; }
            .hero-main { grid-template-columns: 72px 1fr; padding: 14px; }
            .shop-avatar { width: 72px; height: 72px; }
            .meta-title { font-size: 22px; }
            .stats { grid-template-columns: 1fr; padding: 12px 14px 14px; }
            .products-head { padding: 14px; }
            .products-title { font-size: 21px; }
            .grid { padding: 12px; gap: 12px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shell">
            <section class="hero">
                <div class="hero-top">
                    <div>
                        <h1 class="hero-title">Trang công khai gian hàng</h1>
                        <div class="hero-sub">Trang công khai cho phép xem tự do. Nếu muốn thao tác trao đổi, vui lòng đăng nhập tài khoản Home.</div>
                    </div>
                    <div style="display:flex; gap:8px; flex-wrap:wrap;">
                        <asp:HyperLink ID="lnk_home_login" runat="server" CssClass="btn-login" />
                        <a class="btn-login" href="/shop/default.aspx">Trang chủ shop</a>
                        <a class="btn-login" href="/shop/dang-nhap">Đăng nhập shop</a>
                    </div>
                </div>

                <div class="hero-main">
                    <asp:Image ID="img_avatar" runat="server" CssClass="shop-avatar" />
                    <div>
                        <h2 class="meta-title"><asp:Label ID="lb_shop_name" runat="server" /></h2>
                        <div class="meta-line">Mã gian hàng: <asp:Label ID="lb_shop_slug" runat="server" /></div>
                        <div class="meta-line">Chủ gian hàng: <asp:Label ID="lb_owner_name" runat="server" /></div>
                    </div>
                </div>

                <div class="stats">
                    <article class="stat">
                        <div class="stat-label">Sản phẩm đang hiển thị</div>
                        <div class="stat-value"><asp:Label ID="lb_total_products" runat="server" /></div>
                    </article>
                    <article class="stat">
                        <div class="stat-label">Tổng lượt xem</div>
                        <div class="stat-value"><asp:Label ID="lb_total_views" runat="server" /></div>
                    </article>
                    <article class="stat">
                        <div class="stat-label">Đã bán</div>
                        <div class="stat-value"><asp:Label ID="lb_total_sold" runat="server" /></div>
                    </article>
                    <article class="stat">
                        <div class="stat-label">Đơn chờ trao đổi</div>
                        <div class="stat-value"><asp:Label ID="lb_pending_orders" runat="server" /></div>
                    </article>
                </div>
            </section>

            <section class="products">
                <div class="products-head">
                    <div>
                        <h3 class="products-title">Sản phẩm của gian hàng</h3>
                        <div class="products-sub">Danh sách này là public, lấy trực tiếp từ tài khoản shop hiện tại.</div>
                    </div>
                    <div class="public-url">
                        URL: <asp:Label ID="lb_public_url" runat="server" />
                    </div>
                </div>

                <asp:PlaceHolder ID="ph_empty_products" runat="server" Visible="false">
                    <div class="empty">Gian hàng chưa có sản phẩm công khai.</div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_products" runat="server" Visible="true">
                    <div class="grid">
                        <asp:Repeater ID="rp_products" runat="server">
                            <ItemTemplate>
                                <a class="card" href="<%# ResolveProductUrl(Eval("id"), Eval("name_en")) %>">
                                    <img class="card-image" src="<%# ResolveProductImage(Eval("image")) %>" alt="<%# HttpUtility.HtmlEncode((Eval("name") ?? "").ToString()) %>" />
                                    <div class="card-body">
                                        <h4 class="card-name"><%# HttpUtility.HtmlEncode((Eval("name") ?? "").ToString()) %></h4>
                                        <div class="card-meta">Ngày đăng: <%# FormatDate(Eval("ngaytao")) %></div>
                                        <div class="card-price"><%# FormatCurrency(Eval("giaban")) %> Quyền</div>
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:PlaceHolder>
            </section>
        </div>
    </form>
</body>
</html>
