<%@ Page Language="C#" AutoEventWireup="true" CodeFile="xem-dich-vu.aspx.cs" Inherits="gianhang_xem_dich_vu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chi tiết dịch vụ</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body { margin: 0; font-family: "Nunito Sans", "Segoe UI", Arial, sans-serif; background: #fff7ed; color: #17233b; }
        a { text-decoration: none; color: inherit; }
        .shell { width: min(1120px, calc(100% - 24px)); margin: 18px auto 28px; }
        .card { background: #fff; border: 1px solid #fed7aa; border-radius: 22px; box-shadow: 0 16px 32px rgba(15,23,42,.08); overflow: hidden; }
        .head { padding: 16px 18px; border-bottom: 1px solid #ffedd5; display:flex; align-items:center; justify-content:space-between; gap:10px; flex-wrap:wrap; }
        .btn { display:inline-flex; align-items:center; justify-content:center; min-height:40px; padding:0 16px; border-radius:999px; font-weight:800; border:1px solid transparent; }
        .btn-soft { background:#ffedd5; color:#9a3412; border-color:#fed7aa; }
        .btn-primary { background:#f97316; color:#fff; border-color:#f97316; }
        .layout { display:grid; grid-template-columns: minmax(280px, 420px) 1fr; gap: 20px; padding: 20px; }
        .image { width:100%; aspect-ratio:1 / 1; object-fit:cover; border-radius:18px; background:#fff7ed; }
        .name { margin:0; font-size:34px; line-height:1.1; font-weight:900; }
        .sub { margin-top:8px; color:#64748b; font-size:14px; }
        .price { margin-top:14px; color:#dc2626; font-size:28px; font-weight:900; }
        .stats { margin-top:14px; display:flex; gap:10px; flex-wrap:wrap; }
        .chip { min-height:32px; display:inline-flex; align-items:center; padding:0 12px; border-radius:999px; background:#fff7ed; border:1px solid #fed7aa; color:#9a3412; font-size:12px; font-weight:800; }
        .actions { margin-top:18px; display:flex; gap:10px; flex-wrap:wrap; }
        .content { margin-top:18px; padding:18px 20px 24px; border-top:1px solid #ffedd5; }
        .content h2 { margin:0 0 10px; font-size:22px; }
        .not-found { padding:36px 24px; text-align:center; color:#64748b; }
        @media (max-width: 767.98px) {
            .layout { grid-template-columns: 1fr; }
            .name { font-size: 28px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shell">
            <asp:PlaceHolder ID="pn_content" runat="server" Visible="false">
                <div class="card">
                    <div class="head">
                        <asp:HyperLink ID="lnk_back" runat="server" CssClass="btn btn-soft">Quay lại</asp:HyperLink>
                        <asp:HyperLink ID="lnk_store" runat="server" CssClass="btn btn-soft">Trang công khai</asp:HyperLink>
                    </div>
                    <div class="layout">
                        <div>
                            <asp:Image ID="img_service" runat="server" CssClass="image" />
                        </div>
                        <div>
                            <h1 class="name"><asp:Label ID="lb_name" runat="server" /></h1>
                            <div class="sub">Từ gian hàng: <asp:Label ID="lb_store_name" runat="server" /></div>
                            <div class="sub"><asp:Label ID="lb_desc" runat="server" /></div>
                            <div class="price"><asp:Label ID="lb_price" runat="server" /></div>
                            <div class="stats">
                                <span class="chip">Lượt xem: <asp:Label ID="lb_views" runat="server" Text="0" /></span>
                                <span class="chip">Đã bán: <asp:Label ID="lb_sold" runat="server" Text="0" /></span>
                            </div>
                            <div class="actions">
                                <asp:HyperLink ID="lnk_booking" runat="server" CssClass="btn btn-primary">Đặt lịch</asp:HyperLink>
                            </div>
                        </div>
                    </div>
                    <div class="content">
                        <h2>Nội dung chi tiết</h2>
                        <asp:Literal ID="lit_content" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="pn_not_found" runat="server" Visible="false">
                <div class="card not-found">
                    <h1>Không tìm thấy dịch vụ</h1>
                    <p>Dịch vụ này hiện không còn hiển thị hoặc đã được gỡ khỏi gian hàng.</p>
                    <a href="/gianhang/public.aspx" class="btn btn-soft">Về trang công khai</a>
                </div>
            </asp:PlaceHolder>
        </div>
    </form>
</body>
</html>
