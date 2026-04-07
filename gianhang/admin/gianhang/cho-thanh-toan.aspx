<%@ Page Title="Chờ thanh toán gian hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="cho-thanh-toan.aspx.cs" Inherits="gianhang_admin_gianhang_cho_thanh_toan" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-wait { display:grid; gap:18px; }
        .gh-admin-wait__hero,
        .gh-admin-wait__card {
            border:1px solid #ecdcdc;
            border-radius:20px;
            background:#fff;
            box-shadow:0 18px 34px rgba(15,23,42,.06);
        }
        .gh-admin-wait__hero {
            padding:20px 22px;
            background:radial-gradient(720px 220px at 0% 0%, rgba(255,104,62,.16), transparent 60%), linear-gradient(180deg, #fff9f8 0%, #ffffff 100%);
            display:flex;
            justify-content:space-between;
            gap:16px;
            align-items:flex-start;
            flex-wrap:wrap;
        }
        .gh-admin-wait__kicker { display:inline-flex; align-items:center; min-height:30px; padding:0 12px; border-radius:999px; background:#fff1ea; border:1px solid #ffd3c4; color:#c2410c; font-size:12px; font-weight:800; letter-spacing:.03em; text-transform:uppercase; }
        .gh-admin-wait__title { margin:10px 0 6px; color:#7f1d1d; font-size:30px; line-height:1.1; font-weight:900; }
        .gh-admin-wait__sub { margin:0; color:#8d5d5d; font-size:14px; line-height:1.6; max-width:760px; }
        .gh-admin-wait__actions { display:flex; gap:10px; flex-wrap:wrap; }
        .gh-admin-wait__btn { display:inline-flex; align-items:center; justify-content:center; min-height:42px; padding:0 16px; border-radius:14px; border:1px solid transparent; text-decoration:none !important; font-size:13px; font-weight:800; }
        .gh-admin-wait__btn--primary { color:#fff !important; background:linear-gradient(135deg, #d73a31, #ef6b41); box-shadow:0 14px 28px rgba(215,58,49,.18); }
        .gh-admin-wait__btn--soft { color:#7f1d1d !important; background:#fff; border-color:#f0d7d9; }
        .gh-admin-wait__filters { padding:16px; display:grid; grid-template-columns:minmax(0, 1fr) auto auto; gap:12px; align-items:end; }
        .gh-admin-wait__field { display:grid; gap:6px; }
        .gh-admin-wait__field label { color:#8d5d5d; font-size:12px; font-weight:700; text-transform:uppercase; letter-spacing:.03em; }
        .gh-admin-wait__input { min-height:42px; border-radius:14px; border:1px solid #d9dee7; background:#fff; padding:0 14px; color:#1f2937; font-size:14px; outline:none; }
        .gh-admin-wait__input:focus { border-color:#ef6b41; box-shadow:0 0 0 3px rgba(239,107,65,.12); }
        .gh-admin-wait__submit, .gh-admin-wait__reset { min-height:42px; border-radius:14px; padding:0 16px; font-size:13px; font-weight:800; }
        .gh-admin-wait__submit { border:0; background:linear-gradient(135deg, #d73a31, #ef6b41); color:#fff; }
        .gh-admin-wait__reset { border:1px solid #eadfe2; background:#fff; color:#7f1d1d; }
        .gh-admin-wait__summary { display:grid; grid-template-columns:repeat(auto-fit, minmax(170px, 1fr)); gap:12px; }
        .gh-admin-wait__summary-card { border:1px solid #ebeff5; border-radius:18px; background:#fff; padding:16px; }
        .gh-admin-wait__summary-card small { display:block; color:#8d5d5d; font-size:12px; text-transform:uppercase; font-weight:700; }
        .gh-admin-wait__summary-card strong { display:block; margin-top:8px; color:#1f2937; font-size:28px; line-height:1.05; font-weight:900; }
        .gh-admin-wait__summary-card span { display:block; margin-top:6px; color:#6b7280; font-size:13px; }
        .gh-admin-wait__table-wrap { overflow:auto; }
        .gh-admin-wait__table { width:100%; border-collapse:collapse; }
        .gh-admin-wait__table th, .gh-admin-wait__table td { padding:12px 10px; border-bottom:1px solid #edf1f6; text-align:left; vertical-align:top; }
        .gh-admin-wait__table th { color:#8d5d5d; font-size:12px; text-transform:uppercase; letter-spacing:.03em; }
        .gh-admin-wait__badge { display:inline-flex; align-items:center; min-height:28px; padding:0 10px; border-radius:999px; font-size:12px; font-weight:800; white-space:nowrap; }
        .gh-admin-wait__badge--pending { background:#eff6ff; color:#2563eb; }
        .gh-admin-wait__badge--waiting { background:#fff1f2; color:#be123c; }
        .gh-admin-wait__badge--success { background:#eff6ff; color:#1d4ed8; }
        .gh-admin-wait__badge--done { background:#ecfdf3; color:#15803d; }
        .gh-admin-wait__badge--cancelled { background:#f3f4f6; color:#6b7280; }
        .gh-admin-wait__actions-list { display:flex; gap:8px; flex-wrap:wrap; }
        .gh-admin-wait__action-btn, .gh-admin-wait__action-link { display:inline-flex; align-items:center; justify-content:center; min-height:34px; padding:0 12px; border-radius:12px; border:1px solid #eadfe2; background:#fff; color:#7f1d1d !important; font-size:12px; font-weight:800; text-decoration:none !important; }
        .gh-admin-wait__action-btn { box-shadow:none; }
        .gh-admin-wait__empty { padding:22px 12px; color:#6b7280; text-align:center; }
        @media (max-width: 767px) { .gh-admin-wait__filters { grid-template-columns:1fr; } .gh-admin-wait__title { font-size:26px; } }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-wait">
        <section class="gh-admin-wait__hero">
            <div>
                <div class="gh-admin-wait__kicker">Buyer-flow /gianhang</div>
                <h1 class="gh-admin-wait__title">Chờ thanh toán · trao đổi</h1>
                <p class="gh-admin-wait__sub">
                    Đây là lớp admin để theo dõi riêng các đơn buyer đã vào bước <code>chờ thanh toán / trao đổi</code>.
                    Từ đây mình nhìn thấy rõ đơn nào đang chờ chốt, đơn nào đã mirror sang hóa đơn admin và thao tác nào còn cần xử lý.
                </p>
            </div>
            <div class="gh-admin-wait__actions">
                <a class="gh-admin-wait__btn gh-admin-wait__btn--primary" href="<%=OrdersUrl %>">Đơn gian hàng</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=ContentUrl %>">Nội dung /gianhang</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=BookingsUrl %>">Lịch hẹn /gianhang</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=UtilityUrl %>">Tiện ích /gianhang</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=BuyerFlowUrl %>">Buyer-flow / Đơn mua</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=NativeWaitUrl %>">Mở native /gianhang</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=LegacyInvoiceUrl %>">Hóa đơn admin</a>
                <a class="gh-admin-wait__btn gh-admin-wait__btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <section class="gh-admin-wait__card">
            <div class="gh-admin-wait__filters">
                <div class="gh-admin-wait__field">
                    <label for="<%= txt_search.ClientID %>">Tìm đơn chờ thanh toán</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-admin-wait__input" MaxLength="100" placeholder="ID đơn, tên khách, số điện thoại, tài khoản buyer"></asp:TextBox>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc đơn" CssClass="gh-admin-wait__submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-admin-wait__reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-admin-wait__summary">
            <div class="gh-admin-wait__summary-card">
                <small>Đơn đang chờ</small>
                <strong><%=SummaryWaiting.ToString("#,##0") %></strong>
                <span>Buyer đã đi tới bước trao đổi / xác nhận thanh toán.</span>
            </div>
            <div class="gh-admin-wait__summary-card">
                <small>Đã mirror sang admin</small>
                <strong><%=SummaryMirrored.ToString("#,##0") %></strong>
                <span>Mở tiếp bằng mô-đun hóa đơn admin nếu cần vận hành sâu hơn.</span>
            </div>
            <div class="gh-admin-wait__summary-card">
                <small>Có thể hủy chờ</small>
                <strong><%=SummaryCanCancel.ToString("#,##0") %></strong>
                <span>Dùng khi buyer đổi ý hoặc cần đóng phiên trao đổi hiện tại.</span>
            </div>
            <div class="gh-admin-wait__summary-card">
                <small>Có thể chốt đã giao</small>
                <strong><%=SummaryCanDeliver.ToString("#,##0") %></strong>
                <span>Những đơn đã xong trao đổi và có thể đóng vòng buyer-flow.</span>
            </div>
        </div>

        <section class="gh-admin-wait__card">
            <div class="gh-admin-wait__table-wrap">
                <table class="gh-admin-wait__table">
                    <thead>
                        <tr>
                            <th style="width:110px;">ID đơn</th>
                            <th style="min-width:120px;">Ngày tạo</th>
                            <th style="min-width:220px;">Buyer</th>
                            <th style="min-width:140px;">Trạng thái</th>
                            <th class="text-right" style="min-width:140px;">Tổng tiền</th>
                            <th style="min-width:140px;">Bridge admin</th>
                            <th style="min-width:280px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_orders" runat="server" OnItemCommand="rp_orders_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <strong>#<%# Eval("DisplayId") %></strong><br />
                                        <span class="fg-gray">GH: <%# Eval("SourceInvoiceText") %></span>
                                    </td>
                                    <td><%# Eval("OrderedAtText") %></td>
                                    <td>
                                        <strong><%# Eval("BuyerDisplay") %></strong><br />
                                        <span class="fg-gray"><%# Eval("BuyerHint") %></span>
                                    </td>
                                    <td><span class='gh-admin-wait__badge <%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                    <td class="text-right"><strong><%# Eval("TotalAmountText") %> đ</strong></td>
                                    <td>
                                        <asp:PlaceHolder ID="ph_has_legacy" runat="server" Visible='<%# Convert.ToBoolean(Eval("HasLegacyMirror")) %>'>
                                            <a class="gh-admin-wait__action-link" href="<%# Eval("LegacyDetailUrl") %>">Hóa đơn admin</a>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="ph_no_legacy" runat="server" Visible='<%# !Convert.ToBoolean(Eval("HasLegacyMirror")) %>'>
                                            <span class="fg-gray">Chưa mirror</span>
                                        </asp:PlaceHolder>
                                    </td>
                                    <td>
                                        <div class="gh-admin-wait__actions-list">
                                            <a class="gh-admin-wait__action-link" href="<%=NativeWaitUrl %>">Mở native wait</a>
                                            <asp:PlaceHolder ID="ph_cancel_wait" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanCancelWait")) %>'>
                                                <asp:LinkButton ID="btn_cancel_wait" runat="server" CssClass="gh-admin-wait__action-btn" CommandName="cancelwait" CommandArgument='<%# Eval("ActionOrderId") %>'>Hủy chờ</asp:LinkButton>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_deliver" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanMarkDelivered")) %>'>
                                                <asp:LinkButton ID="btn_deliver" runat="server" CssClass="gh-admin-wait__action-btn" CommandName="deliver" CommandArgument='<%# Eval("ActionOrderId") %>'>Đã giao</asp:LinkButton>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_cancel_order" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanCancelOrder")) %>'>
                                                <asp:LinkButton ID="btn_cancel_order" runat="server" CssClass="gh-admin-wait__action-btn" CommandName="cancelorder" CommandArgument='<%# Eval("ActionOrderId") %>' OnClientClick="return confirm('Hủy đơn này?');">Hủy đơn</asp:LinkButton>
                                            </asp:PlaceHolder>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_no_orders" runat="server" Visible="false">
                            <tr>
                                <td colspan="7" class="gh-admin-wait__empty">Hiện chưa có đơn nào ở bước <code>chờ thanh toán / trao đổi</code>.</td>
                            </tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
