<%@ Page Title="Giỏ hàng" Language="C#" MasterPageFile="~/gianhang/mp-home.master" AutoEventWireup="true" CodeFile="giohang.aspx.cs" Inherits="gianhang_giohang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .gh-cart-shell {
            width: min(1240px, calc(100% - 28px));
            margin: 24px auto 40px;
        }
        .gh-cart-notice {
            border-radius: 16px;
            padding: 14px 18px;
            margin-bottom: 16px;
            border: 1px solid transparent;
            font-weight: 700;
        }
        .gh-cart-notice--success {
            background: #ecfdf3;
            border-color: #a7f3d0;
            color: #166534;
        }
        .gh-cart-notice--warning {
            background: #fff7ed;
            border-color: #fdba74;
            color: #9a3412;
        }
        .gh-cart-card {
            background: #fff;
            border: 1px solid #fed7aa;
            border-radius: 24px;
            box-shadow: 0 18px 40px rgba(15, 23, 42, .08);
        }
        .gh-cart-hero {
            display: grid;
            grid-template-columns: 92px 1fr;
            gap: 18px;
            align-items: center;
            padding: 22px;
            background: linear-gradient(135deg, rgba(249,115,22,.12), rgba(251,146,60,.08));
        }
        .gh-cart-avatar {
            width: 92px;
            height: 92px;
            border-radius: 24px;
            object-fit: cover;
            border: 1px solid #fdba74;
            background: #fff7ed;
        }
        .gh-cart-title {
            margin: 0;
            font-size: clamp(30px, 4vw, 48px);
            line-height: 1.04;
            font-weight: 900;
            color: #17233b;
        }
        .gh-cart-sub {
            margin-top: 8px;
            color: #64748b;
            font-size: 15px;
            font-weight: 700;
        }
        .gh-cart-pills {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 12px;
        }
        .gh-cart-pill {
            display: inline-flex;
            align-items: center;
            min-height: 38px;
            padding: 0 14px;
            border-radius: 999px;
            border: 1px solid #fdba74;
            background: #fff7ed;
            color: #9a3412;
            font-size: 13px;
            font-weight: 800;
        }
        .gh-cart-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 14px;
        }
        .gh-btn,
        .gh-btn:visited {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 44px;
            padding: 0 16px;
            border-radius: 14px;
            border: 1px solid transparent;
            text-decoration: none;
            font-size: 14px;
            font-weight: 900;
            cursor: pointer;
        }
        .gh-btn--primary {
            background: linear-gradient(135deg, #f97316, #fb923c);
            color: #fff !important;
            box-shadow: 0 12px 24px rgba(249,115,22,.22);
        }
        .gh-btn--soft {
            background: #fff7ed;
            border-color: #fdba74;
            color: #9a3412 !important;
        }
        .gh-btn--ghost {
            background: #fff;
            border-color: #fed7aa;
            color: #475569 !important;
        }
        .gh-cart-layout {
            display: grid;
            grid-template-columns: minmax(0, 1.4fr) minmax(320px, .9fr);
            gap: 18px;
            margin-top: 18px;
        }
        .gh-panel {
            background: #fff;
            border: 1px solid #fed7aa;
            border-radius: 24px;
            overflow: hidden;
            box-shadow: 0 14px 28px rgba(15, 23, 42, .06);
        }
        .gh-panel-head {
            padding: 18px 20px;
            border-bottom: 1px solid #ffedd5;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }
        .gh-panel-title {
            margin: 0;
            font-size: 28px;
            line-height: 1.08;
            font-weight: 900;
            color: #17233b;
        }
        .gh-panel-sub {
            margin-top: 6px;
            color: #64748b;
            font-size: 14px;
            font-weight: 700;
        }
        .gh-panel-body {
            padding: 18px 20px 20px;
        }
        .gh-toolbar {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }
        .gh-empty {
            border: 1px dashed #fdba74;
            border-radius: 18px;
            padding: 26px 18px;
            text-align: center;
            background: #fffaf5;
            color: #64748b;
            font-weight: 700;
        }
        .gh-items {
            display: flex;
            flex-direction: column;
            gap: 14px;
        }
        .gh-item {
            display: grid;
            grid-template-columns: 110px 1fr auto;
            gap: 16px;
            align-items: center;
            padding: 16px;
            border: 1px solid #ffedd5;
            border-radius: 20px;
            background: #fffaf6;
        }
        .gh-item__image {
            width: 110px;
            height: 110px;
            border-radius: 18px;
            object-fit: cover;
            background: #fff;
            border: 1px solid #fed7aa;
        }
        .gh-item__name {
            font-size: 24px;
            line-height: 1.12;
            font-weight: 900;
            color: #17233b;
        }
        .gh-item__meta {
            margin-top: 6px;
            color: #64748b;
            font-size: 13px;
            font-weight: 700;
        }
        .gh-item__price {
            margin-top: 10px;
            font-size: 28px;
            line-height: 1.05;
            font-weight: 900;
            color: #17233b;
        }
        .gh-item__quyen {
            margin-top: 6px;
            color: #0f766e;
            font-size: 14px;
            font-weight: 900;
        }
        .gh-item__side {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            gap: 12px;
            min-width: 220px;
        }
        .gh-qty {
            width: 126px;
            height: 48px;
            border-radius: 14px;
            border: 1px solid #fdba74;
            padding: 0 14px;
            font-size: 22px;
            font-weight: 900;
            text-align: center;
            outline: none;
            background: #fff;
        }
        .gh-item__subtotal-label {
            font-size: 11px;
            color: #64748b;
            font-weight: 800;
            text-transform: uppercase;
            letter-spacing: .08em;
        }
        .gh-item__subtotal {
            font-size: 24px;
            line-height: 1.05;
            font-weight: 900;
            color: #9a3412;
        }
        .gh-item__remove,
        .gh-item__remove:visited {
            color: #ef4444;
            font-size: 13px;
            font-weight: 800;
            text-decoration: none;
        }
        .gh-summary-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 12px;
        }
        .gh-summary-box {
            border: 1px solid #fed7aa;
            border-radius: 18px;
            background: #fffaf5;
            padding: 14px 16px;
        }
        .gh-summary-label {
            font-size: 12px;
            color: #64748b;
            font-weight: 800;
            text-transform: uppercase;
            letter-spacing: .06em;
        }
        .gh-summary-value {
            margin-top: 8px;
            font-size: 30px;
            line-height: 1.04;
            font-weight: 900;
            color: #17233b;
            word-break: break-word;
        }
        .gh-checkout-note {
            margin-top: 14px;
            padding: 14px 16px;
            border: 1px solid #fed7aa;
            border-radius: 18px;
            background: #fff7ed;
            color: #9a3412;
            font-weight: 700;
        }
        .gh-form {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 12px;
            margin-top: 16px;
        }
        .gh-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }
        .gh-field--full { grid-column: 1 / -1; }
        .gh-field label {
            font-size: 12px;
            color: #64748b;
            font-weight: 800;
            text-transform: uppercase;
            letter-spacing: .06em;
        }
        .gh-input,
        .gh-textarea {
            width: 100%;
            border-radius: 14px;
            border: 1px solid #fdba74;
            padding: 12px 14px;
            font-size: 16px;
            outline: none;
            background: #fff;
        }
        .gh-textarea {
            min-height: 120px;
            resize: vertical;
        }
        .gh-submit {
            width: 100%;
            min-height: 50px;
            border: none;
            border-radius: 16px;
            background: linear-gradient(135deg, #f97316, #fb923c);
            color: #fff;
            font-size: 16px;
            font-weight: 900;
            cursor: pointer;
            box-shadow: 0 14px 26px rgba(249,115,22,.24);
        }
        .gh-invalid {
            max-width: 780px;
            margin: 26px auto;
            padding: 22px;
            text-align: center;
        }
        .gh-invalid h1 {
            margin: 0;
            font-size: 34px;
            line-height: 1.08;
            color: #17233b;
        }
        .gh-invalid p {
            margin: 10px 0 0;
            color: #64748b;
            font-size: 15px;
            font-weight: 700;
        }
        @media (max-width: 767.98px) {
            .gh-cart-layout {
                grid-template-columns: 1fr;
            }
            .gh-cart-hero {
                grid-template-columns: 1fr;
                text-align: center;
            }
            .gh-cart-pills,
            .gh-cart-actions {
                justify-content: center;
            }
        }
        @media (max-width: 767.98px) {
            .gh-cart-shell { width: calc(100% - 18px); margin: 16px auto 28px; }
            .gh-panel-head,
            .gh-panel-body,
            .gh-cart-hero { padding: 16px; }
            .gh-panel-title { font-size: 24px; }
            .gh-item {
                grid-template-columns: 1fr;
            }
            .gh-item__image { width: 100%; height: auto; aspect-ratio: 1 / 1; }
            .gh-item__side {
                align-items: stretch;
                min-width: 0;
            }
            .gh-summary-grid,
            .gh-form {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="gh-cart-shell">
        <asp:PlaceHolder ID="ph_notice" runat="server" Visible="false">
            <div id="box_notice" runat="server" class="gh-cart-notice gh-cart-notice--warning">
                <asp:Literal ID="lit_notice" runat="server"></asp:Literal>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="ph_invalid" runat="server" Visible="false">
            <div class="gh-cart-card gh-invalid">
                <h1>Không mở được giỏ hàng</h1>
                <p><asp:Literal ID="lit_invalid_message" runat="server"></asp:Literal></p>
                <div class="gh-cart-actions" style="justify-content:center; margin-top: 18px;">
                    <asp:HyperLink ID="lnk_invalid_back" runat="server" CssClass="gh-btn gh-btn--primary">Quay lại gian hàng</asp:HyperLink>
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="ph_cart_page" runat="server" Visible="false">
            <section class="gh-cart-card">
                <div class="gh-cart-hero">
                    <asp:Image ID="img_store_avatar" runat="server" CssClass="gh-cart-avatar" AlternateText="store avatar" />
                    <div>
                        <h1 class="gh-cart-title">Giỏ hàng</h1>
                        <div class="gh-cart-sub">Bạn đang đặt hàng tại gian hàng đối tác <asp:Label ID="lb_store_name" runat="server"></asp:Label>.</div>
                        <div class="gh-cart-pills">
                            <span class="gh-cart-pill">Đơn tạo tại đây sẽ được ghi nhận ngay vào danh sách đơn bán</span>
                            <span class="gh-cart-pill">1 Quyền = 1.000đ (quy đổi nội bộ)</span>
                        </div>
                        <div class="gh-cart-actions">
                            <asp:HyperLink ID="lnk_storefront" runat="server" CssClass="gh-btn gh-btn--primary">Xem trang công khai</asp:HyperLink>
                            <asp:HyperLink ID="lnk_continue" runat="server" CssClass="gh-btn gh-btn--soft">Tiếp tục mua sắm</asp:HyperLink>
                        </div>
                    </div>
                </div>
            </section>

            <div class="gh-cart-layout">
                <section class="gh-panel">
                    <div class="gh-panel-head">
                        <div>
                            <h2 class="gh-panel-title">Mặt hàng trong giỏ</h2>
                            <div class="gh-panel-sub">Giỏ hàng này dùng riêng cho gian hàng hiện tại.</div>
                        </div>
                        <div class="gh-toolbar">
                            <asp:Button ID="btn_update_cart" runat="server" Text="Cập nhật giỏ" CssClass="gh-btn gh-btn--soft" OnClick="btn_update_cart_Click" CausesValidation="false" />
                            <asp:Button ID="btn_clear_cart" runat="server" Text="Xóa giỏ" CssClass="gh-btn gh-btn--ghost" OnClick="btn_clear_cart_Click" CausesValidation="false" />
                        </div>
                    </div>
                    <div class="gh-panel-body">
                        <asp:PlaceHolder ID="ph_empty_cart" runat="server" Visible="false">
                            <div class="gh-empty">
                                Giỏ hàng đang trống. Bạn có thể quay lại trang công khai để chọn thêm sản phẩm trước khi tạo đơn.
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="ph_cart_list" runat="server" Visible="false">
                            <div class="gh-items">
                                <asp:Repeater ID="rpt_cart" runat="server">
                                    <ItemTemplate>
                                        <article class="gh-item">
                                            <img class="gh-item__image" src="<%# ResolveImageUrl(Eval("img")) %>" alt="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Name"))) %>" />
                                            <div>
                                                <div class="gh-item__name"><%# HttpUtility.HtmlEncode(Convert.ToString(Eval("Name"))) %></div>
                                                <div class="gh-item__meta">ID: <%# HttpUtility.HtmlEncode(Convert.ToString(Eval("ID"))) %></div>
                                                <div class="gh-item__price"><%# FormatMoney(Eval("Price")) %></div>
                                                <div class="gh-item__quyen">~ <%# FormatQuyen(Eval("Price")) %></div>
                                            </div>
                                            <div class="gh-item__side">
                                                <input class="gh-qty" type="number" min="1" max="9999" name="sl_<%# Eval("ID") %>" value="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("soluong"))) %>" />
                                                <div>
                                                    <div class="gh-item__subtotal-label">Thành tiền</div>
                                                    <div class="gh-item__subtotal"><%# FormatMoney(Eval("thanhtien")) %></div>
                                                </div>
                                                <a class="gh-item__remove" href="<%# BuildRemoveUrl(Eval("ID")) %>">Xóa khỏi giỏ</a>
                                            </div>
                                        </article>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </section>

                <section class="gh-panel" id="checkout-section">
                    <div class="gh-panel-head">
                        <div>
                            <h2 class="gh-panel-title">Xác nhận đặt đơn</h2>
                            <div class="gh-panel-sub">Hoàn tất thông tin để hệ thống tạo đơn bán cho gian hàng này.</div>
                        </div>
                    </div>
                    <div class="gh-panel-body">
                        <div class="gh-summary-grid">
                            <div class="gh-summary-box">
                                <div class="gh-summary-label">Số dòng hàng</div>
                                <div class="gh-summary-value"><asp:Label ID="lb_total_lines" runat="server"></asp:Label></div>
                            </div>
                            <div class="gh-summary-box">
                                <div class="gh-summary-label">Tổng số lượng</div>
                                <div class="gh-summary-value"><asp:Label ID="lb_total_quantity" runat="server"></asp:Label></div>
                            </div>
                            <div class="gh-summary-box">
                                <div class="gh-summary-label">Tổng tiền</div>
                                <div class="gh-summary-value"><asp:Label ID="lb_total_amount" runat="server"></asp:Label></div>
                            </div>
                            <div class="gh-summary-box">
                                <div class="gh-summary-label">Quy đổi nội bộ</div>
                                <div class="gh-summary-value"><asp:Label ID="lb_total_quyen" runat="server"></asp:Label></div>
                            </div>
                        </div>

                        <div class="gh-checkout-note">
                            Sau khi xác nhận, đơn sẽ xuất hiện ngay trong danh sách đơn bán để tiếp tục xử lý.
                        </div>

                        <asp:PlaceHolder ID="ph_checkout" runat="server" Visible="false">
                            <div class="gh-form">
                                <div class="gh-field">
                                    <label>Họ tên người nhận</label>
                                    <asp:TextBox ID="txt_customer_name" runat="server" CssClass="gh-input" MaxLength="80"></asp:TextBox>
                                </div>
                                <div class="gh-field">
                                    <label>Số điện thoại</label>
                                    <asp:TextBox ID="txt_customer_phone" runat="server" CssClass="gh-input" MaxLength="20"></asp:TextBox>
                                </div>
                                <div class="gh-field gh-field--full">
                                    <label>Địa chỉ nhận hàng</label>
                                    <asp:TextBox ID="txt_customer_address" runat="server" CssClass="gh-input" MaxLength="250"></asp:TextBox>
                                </div>
                                <div class="gh-field gh-field--full">
                                    <label>Ghi chú</label>
                                    <asp:TextBox ID="txt_note" runat="server" CssClass="gh-textarea" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>

                            <div class="gh-summary-box" style="margin-top:16px;">
                                <div class="gh-summary-label">Thanh toán đơn này</div>
                                <div class="gh-summary-value"><asp:Label ID="lb_checkout_amount" runat="server"></asp:Label></div>
                                <div class="gh-item__quyen" style="margin-top:8px;">~ <asp:Label ID="lb_checkout_quyen" runat="server"></asp:Label></div>
                            </div>

                            <div class="gh-cart-actions" style="margin-top:18px;">
                                <asp:Button ID="btn_checkout" runat="server" Text="XÁC NHẬN ĐẶT ĐƠN" CssClass="gh-submit" OnClick="btn_checkout_Click" />
                                <asp:HyperLink ID="lnk_back_to_store" runat="server" CssClass="gh-btn gh-btn--ghost">Quay lại gian hàng</asp:HyperLink>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </section>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="server"></asp:Content>
