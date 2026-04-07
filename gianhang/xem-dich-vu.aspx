<%@ Page Language="C#" AutoEventWireup="true" CodeFile="xem-dich-vu.aspx.cs" Inherits="gianhang_xem_dich_vu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chi tiáº¿t dá»‹ch vá»¥</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body { margin: 0; font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif; background: #fff7ed; color: #17233b; }
        a { text-decoration: none; color: inherit; }
        .shell { width: min(1120px, calc(100% - 24px)); margin: 18px auto 28px; }
        .card { background: #fff; border: 1px solid #fed7aa; border-radius: 22px; box-shadow: 0 16px 32px rgba(15,23,42,.08); overflow: hidden; }
        .head {
            position: relative;
            padding: 18px;
            border-bottom: 1px solid #ffedd5;
            display:flex;
            align-items:center;
            justify-content:space-between;
            gap:10px;
            flex-wrap:wrap;
            background:
                linear-gradient(125deg, rgba(249,115,22,.92), rgba(251,146,60,.84)),
                url('<%= Helper_cl.VersionedUrl("~/uploads/images/gianhang-banner-home-style.png") %>') center center / cover no-repeat;
        }
        .btn { display:inline-flex; align-items:center; justify-content:center; min-height:40px; padding:0 16px; border-radius:999px; font-weight:800; border:1px solid transparent; }
        .btn-soft,
        .btn-soft:link,
        .btn-soft:visited { background:#fff7ed; color:#9a3412 !important; border-color:#fed7aa; text-decoration:none !important; }
        .btn-primary,
        .btn-primary:link,
        .btn-primary:visited { background:#f97316; color:#fff !important; border-color:#f97316; text-decoration:none !important; }
        .btn-soft:hover,
        .btn-soft:active,
        .btn-soft:focus { color:#7c2d12 !important; background:#ffedd5; text-decoration:none !important; }
        .btn-primary:hover,
        .btn-primary:active,
        .btn-primary:focus { color:#fff !important; background:#ea580c; text-decoration:none !important; }
        .layout { display:grid; grid-template-columns: minmax(280px, 420px) 1fr; gap: 20px; padding: 20px; }
        .image { width:100%; aspect-ratio:1 / 1; object-fit:cover; border-radius:18px; background:#fff7ed; }
        .name { margin:0; font-size:34px; line-height:1.1; font-weight:900; color:#17233b; }
        .sub { margin-top:8px; color:#64748b; font-size:14px; }
        .price { margin-top:14px; color:#dc2626; font-size:28px; font-weight:900; }
        .trust-strip { margin-top:14px; display:flex; gap:10px; flex-wrap:wrap; }
        .trust-pill { min-height:32px; display:inline-flex; align-items:center; padding:0 12px; border-radius:999px; background:#fff7ed; border:1px solid #fed7aa; color:#9a3412; font-size:12px; font-weight:800; }
        .stats { margin-top:14px; display:flex; gap:10px; flex-wrap:wrap; }
        .chip { min-height:32px; display:inline-flex; align-items:center; padding:0 12px; border-radius:999px; background:#fff7ed; border:1px solid #fed7aa; color:#9a3412; font-size:12px; font-weight:800; }
        .actions { margin-top:18px; display:flex; gap:10px; flex-wrap:wrap; }
        .content { margin-top:18px; padding:18px 20px 24px; border-top:1px solid #ffedd5; }
        .content h2 { margin:0 0 10px; font-size:22px; }
        .not-found { padding:36px 24px; text-align:center; color:#64748b; }
        @media (max-width: 767.98px) {
            .layout { grid-template-columns: 1fr; }
            .name { font-size: 28px; }
            .actions { flex-direction:column; }
            .actions .btn { width:100%; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shell">
            <asp:PlaceHolder ID="pn_content" runat="server" Visible="false">
                <div class="card">
                    <div class="head">
                        <asp:HyperLink ID="lnk_back" runat="server" CssClass="btn btn-soft">Quay láº¡i</asp:HyperLink>
                        <asp:HyperLink ID="lnk_store" runat="server" CssClass="btn btn-soft">Trang cÃ´ng khai</asp:HyperLink>
                    </div>
                    <div class="layout">
                        <div>
                            <asp:Image ID="img_service" runat="server" CssClass="image" />
                        </div>
                        <div>
                            <h1 class="name"><asp:Label ID="lb_name" runat="server" /></h1>
                            <div class="sub">Tá»« gian hÃ ng: <asp:Label ID="lb_store_name" runat="server" /></div>
                            <div class="sub"><asp:Label ID="lb_desc" runat="server" /></div>
                            <div class="price"><asp:Label ID="lb_price" runat="server" /></div>
                            <div class="trust-strip">
                                <span class="trust-pill">CÃ³ thá»ƒ Ä‘áº·t lá»‹ch trá»±c tiáº¿p</span>
                                <span class="trust-pill">Hiá»ƒn thá»‹ cÃ´ng khai trong gian hÃ ng</span>
                            </div>
                            <div class="stats">
                                <span class="chip">LÆ°á»£t xem: <asp:Label ID="lb_views" runat="server" Text="0" /></span>
                                <span class="chip">ÄÃ£ bÃ¡n: <asp:Label ID="lb_sold" runat="server" Text="0" /></span>
                            </div>
                            <div class="actions">
                                <asp:HyperLink ID="lnk_booking" runat="server" CssClass="btn btn-primary">Äáº·t lá»‹ch</asp:HyperLink>
                            </div>
                        </div>
                    </div>
                    <div class="content">
                        <h2>Ná»™i dung chi tiáº¿t</h2>
                        <asp:Literal ID="lit_content" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="pn_not_found" runat="server" Visible="false">
                <div class="card not-found">
                    <h1>KhÃ´ng tÃ¬m tháº¥y dá»‹ch vá»¥</h1>
                    <p>Dá»‹ch vá»¥ nÃ y hiá»‡n khÃ´ng cÃ²n hiá»ƒn thá»‹ hoáº·c Ä‘Ã£ Ä‘Æ°á»£c gá»¡ khá»i gian hÃ ng.</p>
                    <a href="/gianhang/public.aspx" class="btn btn-soft">Vá» trang cÃ´ng khai</a>
                </div>
            </asp:PlaceHolder>
        </div>
    </form>
</body>
</html>
