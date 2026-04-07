<%@ Page Language="C#" AutoEventWireup="true" CodeFile="datlich.aspx.cs" Inherits="gianhang_datlich_public" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Äáº·t lá»‹ch</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body { margin: 0; font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif; background: #fff7ed; color: #17233b; }
        a { text-decoration: none; color: inherit; }
        .shell { width: min(980px, calc(100% - 24px)); margin: 18px auto 28px; }
        .card { background: #fff; border: 1px solid #fed7aa; border-radius: 22px; box-shadow: 0 16px 32px rgba(15,23,42,.08); overflow: hidden; }
        .head {
            padding: 18px 20px;
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
        .title { margin: 0; font-size: 30px; line-height: 1.1; font-weight: 900; color: #fff !important; }
        .sub { margin-top: 6px; color: rgba(255,255,255,.92) !important; font-size: 14px; }
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
        .body { padding: 18px 20px 22px; }
        .trust-strip { margin-bottom:16px; display:flex; gap:10px; flex-wrap:wrap; }
        .trust-pill { min-height:32px; display:inline-flex; align-items:center; padding:0 12px; border-radius:999px; background:#fff7ed; border:1px solid #fed7aa; color:#9a3412; font-size:12px; font-weight:800; }
        .message { border-radius: 14px; padding: 14px 16px; margin-bottom: 16px; font-weight: 700; }
        .message-success { background: #ecfdf3; border: 1px solid #86efac; color: #166534; }
        .message-warning { background: #fff7ed; border: 1px solid #fdba74; color: #9a3412; }
        .grid { display:grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 14px; }
        .field { display:flex; flex-direction:column; gap: 6px; }
        .field label { font-size: 13px; font-weight: 800; color: #475569; }
        .field input, .field textarea, .field select {
            width: 100%;
            min-height: 42px;
            border-radius: 12px;
            border: 1px solid #fdba74;
            padding: 0 12px;
            font-size: 14px;
            font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif !important;
            outline: none;
            background: #fff;
        }
        .field textarea { min-height: 130px; padding: 12px; resize: vertical; }
        .actions { margin-top: 18px; display:flex; gap:10px; flex-wrap:wrap; }
        .not-found { padding: 36px 24px; text-align:center; color:#64748b; }
        @media (max-width: 767.98px) {
            .grid { grid-template-columns: 1fr; }
            .title { font-size: 26px; }
            .actions { flex-direction:column; }
            .actions .btn { width:100%; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shell">
            <asp:PlaceHolder ID="pn_form" runat="server" Visible="false">
                <div class="card">
                    <div class="head">
                        <div>
                            <h1 class="title">Äáº·t lá»‹ch vá»›i <asp:Label ID="lb_store_name" runat="server" /></h1>
                            <div class="sub"><asp:Label ID="lb_store_desc" runat="server" /></div>
                        </div>
                        <asp:HyperLink ID="lnk_back" runat="server" CssClass="btn btn-soft">Quay láº¡i</asp:HyperLink>
                    </div>
                    <div class="body">
                        <div class="trust-strip">
                            <span class="trust-pill">Gá»­i yÃªu cáº§u Ä‘áº·t lá»‹ch trá»±c tiáº¿p</span>
                            <span class="trust-pill">ThÃ´ng tin hiá»ƒn thá»‹ giá»‘ng nhau trÃªn desktop vÃ  mobile</span>
                        </div>
                        <asp:PlaceHolder ID="pn_message" runat="server" Visible="false">
                            <div id="box_message" runat="server" class="message message-warning">
                                <asp:Literal ID="lit_message" runat="server" />
                            </div>
                        </asp:PlaceHolder>

                        <div class="grid">
                            <div class="field">
                                <label for="<%= txt_customer_name.ClientID %>">Há» tÃªn</label>
                                <asp:TextBox ID="txt_customer_name" runat="server" MaxLength="120" />
                            </div>
                            <div class="field">
                                <label for="<%= txt_customer_phone.ClientID %>">Sá»‘ Ä‘iá»‡n thoáº¡i</label>
                                <asp:TextBox ID="txt_customer_phone" runat="server" MaxLength="30" />
                            </div>
                            <div class="field">
                                <label for="<%= ddl_service.ClientID %>">Dá»‹ch vá»¥</label>
                                <asp:DropDownList ID="ddl_service" runat="server" />
                            </div>
                            <div class="field">
                                <label for="<%= txt_booking_date.ClientID %>">NgÃ y háº¹n</label>
                                <asp:TextBox ID="txt_booking_date" runat="server" TextMode="Date" />
                            </div>
                            <div class="field">
                                <label for="<%= txt_booking_time.ClientID %>">Giá» háº¹n</label>
                                <asp:TextBox ID="txt_booking_time" runat="server" TextMode="Time" />
                            </div>
                            <div class="field">
                                <label for="<%= txt_notes.ClientID %>">Ghi chÃº</label>
                                <asp:TextBox ID="txt_notes" runat="server" TextMode="MultiLine" />
                            </div>
                        </div>

                        <div class="actions">
                            <asp:Button ID="btn_submit" runat="server" CssClass="btn btn-primary" Text="Gá»­i lá»‹ch háº¹n" OnClick="btn_submit_Click" />
                            <asp:HyperLink ID="lnk_back_2" runat="server" CssClass="btn btn-soft" NavigateUrl="#">Quay láº¡i</asp:HyperLink>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="pn_not_found" runat="server" Visible="false">
                <div class="card not-found">
                    <h1>KhÃ´ng tÃ¬m tháº¥y dá»‹ch vá»¥ cáº§n Ä‘áº·t lá»‹ch</h1>
                    <p>KhÃ´ng tÃ¬m tháº¥y dá»‹ch vá»¥ phÃ¹ há»£p Ä‘á»ƒ Ä‘áº·t lá»‹ch.</p>
                    <a href="/gianhang/public.aspx" class="btn btn-soft">Vá» trang cÃ´ng khai</a>
                </div>
            </asp:PlaceHolder>
        </div>
    </form>
    <script>
        (function () {
            var topBack = document.getElementById('<%= lnk_back.ClientID %>');
            var bottomBack = document.getElementById('<%= lnk_back_2.ClientID %>');
            if (topBack && bottomBack) {
                bottomBack.setAttribute('href', topBack.getAttribute('href') || '/gianhang/public.aspx');
            }
        })();
    </script>
</body>
</html>
