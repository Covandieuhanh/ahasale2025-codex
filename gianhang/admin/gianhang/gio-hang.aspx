<%@ Page Title="Giỏ hàng /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="gio-hang.aspx.cs" Inherits="gianhang_admin_gianhang_gio_hang" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-cart{display:grid;gap:18px;}
        .gh-admin-cart__hero,.gh-admin-cart__card{border:1px solid #ecdcdc;border-radius:20px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06);}
        .gh-admin-cart__hero{padding:20px 22px;background:radial-gradient(720px 220px at 0% 0%, rgba(255,104,62,.16), transparent 60%), linear-gradient(180deg,#fff9f8 0%,#ffffff 100%);display:flex;justify-content:space-between;gap:16px;align-items:flex-start;flex-wrap:wrap;}
        .gh-admin-cart__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#fff1ea;border:1px solid #ffd3c4;color:#c2410c;font-size:12px;font-weight:800;letter-spacing:.03em;text-transform:uppercase;}
        .gh-admin-cart__title{margin:10px 0 6px;color:#7f1d1d;font-size:30px;line-height:1.1;font-weight:900;}
        .gh-admin-cart__sub{margin:0;color:#8d5d5d;font-size:14px;line-height:1.65;max-width:760px;}
        .gh-admin-cart__actions{display:flex;gap:10px;flex-wrap:wrap;}
        .gh-admin-cart__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;border:1px solid transparent;text-decoration:none !important;font-size:13px;font-weight:800;}
        .gh-admin-cart__btn--primary{color:#fff !important;background:linear-gradient(135deg,#d73a31,#ef6b41);box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-admin-cart__btn--soft{color:#7f1d1d !important;background:#fff;border-color:#f0d7d9;}
        .gh-admin-cart__summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:12px;}
        .gh-admin-cart__summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px;}
        .gh-admin-cart__summary-card small{display:block;color:#8d5d5d;font-size:12px;text-transform:uppercase;font-weight:700;}
        .gh-admin-cart__summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900;}
        .gh-admin-cart__summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px;}
        .gh-admin-cart__card{padding:18px;}
        .gh-admin-cart__note{padding:14px 16px;border-radius:16px;background:#fff7ed;border:1px solid #fed7aa;color:#9a3412;font-size:13px;line-height:1.7;}
        .gh-admin-cart__table-wrap{overflow:auto;margin-top:14px;}
        .gh-admin-cart__table{width:100%;border-collapse:collapse;}
        .gh-admin-cart__table th,.gh-admin-cart__table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:middle;}
        .gh-admin-cart__table th{color:#8d5d5d;font-size:12px;text-transform:uppercase;letter-spacing:.03em;}
        .gh-admin-cart__product{display:flex;align-items:center;gap:12px;min-width:220px;}
        .gh-admin-cart__thumb{width:56px;height:56px;border-radius:16px;overflow:hidden;border:1px solid #f1d8d9;background:#fff5f5;display:flex;align-items:center;justify-content:center;flex:none;}
        .gh-admin-cart__thumb img{width:100%;height:100%;object-fit:cover;display:block;}
        .gh-admin-cart__thumb span{font-size:12px;color:#9b5b5b;font-weight:800;text-transform:uppercase;}
        .gh-admin-cart__name{display:block;color:#1f2937;font-weight:800;line-height:1.4;}
        .gh-admin-cart__meta{display:block;color:#6b7280;font-size:12px;line-height:1.5;}
        .gh-admin-cart__remove{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #eadfe2;background:#fff;color:#7f1d1d !important;font-size:12px;font-weight:800;text-decoration:none !important;}
        .gh-admin-cart__empty{padding:28px 12px;text-align:center;color:#6b7280;}
        @media (max-width:767px){.gh-admin-cart__title{font-size:26px;}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-cart">
        <section class="gh-admin-cart__hero">
            <div>
                <div class="gh-admin-cart__kicker">Checkout preview</div>
                <h1 class="gh-admin-cart__title">Giỏ hàng /gianhang</h1>
                <p class="gh-admin-cart__sub">
                    Đây là lớp preview giỏ hàng của chính storefront <code>/gianhang</code> ngay trong <code>/gianhang/admin</code>.
                    Vì giỏ hàng là dữ liệu theo browser session của buyer, trang này giúp mình nhìn nhanh phiên checkout hiện tại đang thao tác trên chính trình duyệt này,
                    rồi đi tiếp sang chờ thanh toán, buyer-flow hoặc hóa đơn điện tử mà không phải nhảy vòng ngoài.
                </p>
            </div>
            <div class="gh-admin-cart__actions">
                <a class="gh-admin-cart__btn gh-admin-cart__btn--primary" href="<%=PublicCartUrl %>">Mở native /gianhang</a>
                <a class="gh-admin-cart__btn gh-admin-cart__btn--soft" href="<%=WaitUrl %>">Chờ thanh toán</a>
                <a class="gh-admin-cart__btn gh-admin-cart__btn--soft" href="<%=BuyerFlowUrl %>">Buyer-flow / Đơn mua</a>
                <a class="gh-admin-cart__btn gh-admin-cart__btn--soft" href="<%=ElectronicInvoiceUrl %>">Hóa đơn điện tử</a>
                <a class="gh-admin-cart__btn gh-admin-cart__btn--soft" href="<%=HubUrl %>">Hub /gianhang</a>
            </div>
        </section>

        <div class="gh-admin-cart__summary">
            <div class="gh-admin-cart__summary-card">
                <small>Dòng trong giỏ</small>
                <strong><%=CartLineCount.ToString("#,##0") %></strong>
                <span>Số dòng sản phẩm đang có trong phiên checkout hiện tại.</span>
            </div>
            <div class="gh-admin-cart__summary-card">
                <small>Tổng số lượng</small>
                <strong><%=CartQuantity.ToString("#,##0") %></strong>
                <span>Tổng số lượng của tất cả sản phẩm trong giỏ.</span>
            </div>
            <div class="gh-admin-cart__summary-card">
                <small>Tạm tính</small>
                <strong><%=FormatMoney(CartTotal) %></strong>
                <span>Giá trị checkout tạm thời của session storefront đang mở.</span>
            </div>
            <div class="gh-admin-cart__summary-card">
                <small>Workspace public</small>
                <strong><%=WorkspaceDisplayName %></strong>
                <span>Giỏ hàng được gắn theo storefront owner hiện tại.</span>
            </div>
        </div>

        <section class="gh-admin-cart__card">
            <div class="gh-admin-cart__note">
                Lưu ý: giỏ hàng ở <code>/gianhang</code> là dữ liệu theo session/browser đang mở storefront. Vì vậy màn này dùng để preview nhanh và hỗ trợ thao tác,
                không thay thế hoàn toàn buyer-flow đã tạo đơn. Khi buyer đã đặt đơn, nên chuyển sang <a href="<%=BuyerFlowUrl %>">Buyer-flow / Đơn mua</a> hoặc <a href="<%=WaitUrl %>">Chờ thanh toán</a> để vận hành chính thức.
            </div>
            <div class="gh-admin-cart__table-wrap">
                <table class="gh-admin-cart__table">
                    <thead>
                        <tr>
                            <th style="min-width:280px;">Sản phẩm</th>
                            <th style="width:120px;">Giá bán</th>
                            <th style="width:120px;">Số lượng</th>
                            <th style="width:140px;">Thành tiền</th>
                            <th style="width:140px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_cart" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="gh-admin-cart__product">
                                            <div class="gh-admin-cart__thumb">
                                                <asp:PlaceHolder ID="ph_has_img" runat="server" Visible='<%# Convert.ToString(Eval("ImageUrl")) != "" %>'>
                                                    <img src="<%# Eval("ImageUrl") %>" alt="<%# Eval("Name") %>" />
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_no_img" runat="server" Visible='<%# Convert.ToString(Eval("ImageUrl")) == "" %>'>
                                                    <span>GH</span>
                                                </asp:PlaceHolder>
                                            </div>
                                            <div>
                                                <span class="gh-admin-cart__name"><%# Eval("Name") %></span>
                                                <span class="gh-admin-cart__meta">ID native: <%# Eval("Id") %></span>
                                            </div>
                                        </div>
                                    </td>
                                    <td><strong><%# Eval("PriceText") %></strong></td>
                                    <td><%# Eval("QuantityText") %></td>
                                    <td><strong><%# Eval("LineTotalText") %></strong></td>
                                    <td><a class="gh-admin-cart__remove" href="<%# Eval("RemoveUrl") %>">Bỏ khỏi giỏ</a></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <tr>
                                <td colspan="5" class="gh-admin-cart__empty">Hiện browser session này chưa có sản phẩm nào trong giỏ hàng storefront.</td>
                            </tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
            <div class="mt-4">
                <asp:Button ID="btn_clear_preview" runat="server" CssClass="button alert" Text="Xóa giỏ hàng preview" OnClick="btn_clear_preview_Click" Visible="false" />
            </div>
        </section>
    </div>
</asp:Content>
