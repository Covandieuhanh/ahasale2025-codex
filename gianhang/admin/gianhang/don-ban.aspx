<%@ Page Title="Đơn gian hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="don-ban.aspx.cs" Inherits="gianhang_admin_gianhang_don_ban" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-orders {
            display: grid;
            gap: 18px;
        }

        .gh-admin-orders__hero,
        .gh-admin-orders__card {
            border: 1px solid #ecdcdc;
            border-radius: 20px;
            background: #fff;
            box-shadow: 0 18px 34px rgba(15, 23, 42, 0.06);
        }

        .gh-admin-orders__hero {
            padding: 20px 22px;
            background:
                radial-gradient(720px 220px at 0% 0%, rgba(255, 104, 62, .16), transparent 60%),
                linear-gradient(180deg, #fff9f8 0%, #ffffff 100%);
            display: flex;
            justify-content: space-between;
            gap: 16px;
            align-items: flex-start;
            flex-wrap: wrap;
        }

        .gh-admin-orders__kicker {
            display: inline-flex;
            align-items: center;
            min-height: 30px;
            padding: 0 12px;
            border-radius: 999px;
            background: #fff1ea;
            border: 1px solid #ffd3c4;
            color: #c2410c;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .03em;
            text-transform: uppercase;
        }

        .gh-admin-orders__title {
            margin: 10px 0 6px;
            color: #7f1d1d;
            font-size: 30px;
            line-height: 1.1;
            font-weight: 900;
        }

        .gh-admin-orders__sub {
            margin: 0;
            color: #8d5d5d;
            font-size: 14px;
            line-height: 1.6;
            max-width: 760px;
        }

        .gh-admin-orders__actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gh-admin-orders__btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 14px;
            border: 1px solid transparent;
            text-decoration: none !important;
            font-size: 13px;
            font-weight: 800;
        }

        .gh-admin-orders__btn--primary {
            color: #fff !important;
            background: linear-gradient(135deg, #d73a31, #ef6b41);
            box-shadow: 0 14px 28px rgba(215, 58, 49, .18);
        }

        .gh-admin-orders__btn--soft {
            color: #7f1d1d !important;
            background: #fff;
            border-color: #f0d7d9;
        }

        .gh-admin-orders__filters {
            padding: 16px;
            display: grid;
            grid-template-columns: minmax(0, 1.4fr) minmax(220px, .7fr) auto auto;
            gap: 12px;
            align-items: end;
        }

        .gh-admin-field {
            display: grid;
            gap: 6px;
        }

        .gh-admin-field label {
            color: #8d5d5d;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        .gh-admin-input,
        .gh-admin-select {
            min-height: 42px;
            border-radius: 14px;
            border: 1px solid #d9dee7;
            background: #fff;
            padding: 0 14px;
            color: #1f2937;
            font-size: 14px;
            outline: none;
        }

        .gh-admin-input:focus,
        .gh-admin-select:focus {
            border-color: #ef6b41;
            box-shadow: 0 0 0 3px rgba(239, 107, 65, .12);
        }

        .gh-admin-submit {
            min-height: 42px;
            border-radius: 14px;
            border: 0;
            padding: 0 16px;
            background: linear-gradient(135deg, #d73a31, #ef6b41);
            color: #fff;
            font-size: 13px;
            font-weight: 800;
        }

        .gh-admin-reset {
            min-height: 42px;
            border-radius: 14px;
            border: 1px solid #eadfe2;
            padding: 0 16px;
            background: #fff;
            color: #7f1d1d;
            font-size: 13px;
            font-weight: 800;
        }

        .gh-admin-orders__summary {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
            gap: 12px;
        }

        .gh-admin-orders__summary-card {
            border: 1px solid #ebeff5;
            border-radius: 18px;
            background: #fff;
            padding: 16px;
        }

        .gh-admin-orders__summary-card small {
            display: block;
            color: #8d5d5d;
            font-size: 12px;
            text-transform: uppercase;
            font-weight: 700;
        }

        .gh-admin-orders__summary-card strong {
            display: block;
            margin-top: 8px;
            color: #1f2937;
            font-size: 28px;
            line-height: 1.05;
            font-weight: 900;
        }

        .gh-admin-orders__table-wrap {
            overflow: auto;
        }

        .gh-admin-orders__table {
            width: 100%;
            border-collapse: collapse;
        }

        .gh-admin-orders__table th,
        .gh-admin-orders__table td {
            padding: 12px 10px;
            border-bottom: 1px solid #edf1f6;
            text-align: left;
            vertical-align: top;
        }

        .gh-admin-orders__table th {
            color: #8d5d5d;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        .gh-admin-badge {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
            white-space: nowrap;
        }

        .gh-admin-badge--online {
            background: #ecfdf3;
            color: #157347;
        }

        .gh-admin-badge--offline {
            background: #eff6ff;
            color: #1d4ed8;
        }

        .gh-admin-badge--pending {
            background: #eff6ff;
            color: #2563eb;
        }

        .gh-admin-badge--waiting {
            background: #fff1f2;
            color: #be123c;
        }

        .gh-admin-badge--exchanged {
            background: #eff6ff;
            color: #1d4ed8;
        }

        .gh-admin-badge--delivered {
            background: #ecfdf3;
            color: #15803d;
        }

        .gh-admin-badge--cancelled {
            background: #f3f4f6;
            color: #6b7280;
        }

        .gh-admin-orders__actions-list {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        .gh-admin-orders__action-btn,
        .gh-admin-orders__action-link {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 800;
            text-decoration: none !important;
        }

        .gh-admin-orders__action-btn {
            border: 1px solid #ebeff5;
            background: #fff;
            color: #374151;
        }

        .gh-admin-orders__action-link {
            border: 1px solid #ebeff5;
            background: #f8fafc;
            color: #334155;
        }

        .gh-admin-orders__empty {
            padding: 20px 10px;
            color: #6b7280;
        }

        @media (max-width: 1080px) {
            .gh-admin-orders__filters {
                grid-template-columns: 1fr 1fr;
            }
        }

        @media (max-width: 767px) {
            .gh-admin-orders__title {
                font-size: 26px;
            }

            .gh-admin-orders__filters {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-orders">
        <section class="gh-admin-orders__hero">
            <div>
                <div class="gh-admin-orders__kicker">Đồng bộ từ /gianhang</div>
                <h1 class="gh-admin-orders__title">Đơn gian hàng</h1>
                <p class="gh-admin-orders__sub">
                    Đây là lớp seller-order dùng lại dữ liệu native của <code>/gianhang</code> để theo dõi đơn public,
                    checkout và các trạng thái chờ trao đổi ngay trong <code>/gianhang/admin</code>.
                </p>
            </div>
            <div class="gh-admin-orders__actions">
                <a class="gh-admin-orders__btn gh-admin-orders__btn--primary" href="<%=CreateFlowUrl %>">Tạo giao dịch</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=ContentUrl %>">Nội dung /gianhang</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=BookingsUrl %>">Lịch hẹn /gianhang</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=CreateOrderUrl %>">Tạo đơn /gianhang</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=WaitExchangeUrl %>">Chờ thanh toán</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=BuyerFlowUrl %>">Buyer-flow / Đơn mua</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=LegacyInvoiceUrl %>">Hóa đơn admin</a>
                <a class="gh-admin-orders__btn gh-admin-orders__btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <section class="gh-admin-orders__card">
            <div class="gh-admin-orders__filters">
                <div class="gh-admin-field">
                    <label for="<%= txt_search.ClientID %>">Tìm đơn</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-admin-input" MaxLength="100" placeholder="ID đơn, tên khách, tài khoản, số điện thoại"></asp:TextBox>
                </div>
                <div class="gh-admin-field">
                    <label for="<%= ddl_status.ClientID %>">Lọc trạng thái</label>
                    <asp:DropDownList ID="ddl_status" runat="server" CssClass="gh-admin-select">
                        <asp:ListItem Text="Tất cả trạng thái" Value="all"></asp:ListItem>
                        <asp:ListItem Text="Đã đặt" Value="da-dat"></asp:ListItem>
                        <asp:ListItem Text="Chờ trao đổi" Value="cho-trao-doi"></asp:ListItem>
                        <asp:ListItem Text="Đã trao đổi" Value="da-trao-doi"></asp:ListItem>
                        <asp:ListItem Text="Đã giao" Value="da-giao"></asp:ListItem>
                        <asp:ListItem Text="Đã hủy" Value="da-huy"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc đơn" CssClass="gh-admin-submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-admin-reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-admin-orders__summary">
            <div class="gh-admin-orders__summary-card">
                <small>Tổng đơn</small>
                <strong><%=SummaryTotal.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-orders__summary-card">
                <small>Đã đặt</small>
                <strong><%=SummaryPending.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-orders__summary-card">
                <small>Chờ trao đổi</small>
                <strong><%=SummaryWaiting.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-orders__summary-card">
                <small>Đã trao đổi</small>
                <strong><%=SummaryExchanged.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-orders__summary-card">
                <small>Đã giao</small>
                <strong><%=SummaryDelivered.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-orders__summary-card">
                <small>Đã hủy</small>
                <strong><%=SummaryCancelled.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-orders__summary-card">
                <small>Đã mirror sang admin</small>
                <strong><%=SummaryMirrored.ToString("#,##0") %></strong>
            </div>
        </div>

        <section class="gh-admin-orders__card">
            <div class="gh-admin-orders__table-wrap">
                <table class="gh-admin-orders__table">
                    <thead>
                        <tr>
                            <th style="width:110px;">ID đơn</th>
                            <th style="min-width:120px;">Ngày tạo</th>
                            <th style="min-width:96px;">Loại</th>
                            <th style="min-width:200px;">Khách hàng</th>
                            <th style="min-width:140px;">Trạng thái</th>
                            <th class="text-right" style="min-width:140px;">Tổng tiền</th>
                            <th style="min-width:140px;">Cầu nối admin</th>
                            <th style="min-width:280px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_orders" runat="server" OnItemCommand="rp_orders_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <strong>#<%# Eval("DisplayId") %></strong><br />
                                        <span class="fg-gray">GH: <%# Eval("SourceInvoiceId") %></span>
                                    </td>
                                    <td>
                                        <%# Eval("OrderedAtText") %>
                                    </td>
                                    <td>
                                        <span class='gh-admin-badge <%# Eval("TypeCss") %>'><%# Eval("TypeLabel") %></span>
                                    </td>
                                    <td>
                                        <strong><%# Eval("BuyerDisplay") %></strong><br />
                                        <span class="fg-gray"><%# Eval("BuyerHint") %></span>
                                    </td>
                                    <td>
                                        <span class='gh-admin-badge <%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span>
                                    </td>
                                    <td class="text-right">
                                        <strong><%# Eval("TotalAmountText") %> đ</strong>
                                    </td>
                                    <td>
                                        <asp:PlaceHolder ID="ph_has_legacy" runat="server" Visible='<%# Convert.ToBoolean(Eval("HasLegacyMirror")) %>'>
                                            <a class="gh-admin-orders__action-link" href="<%# Eval("LegacyDetailUrl") %>">Hóa đơn admin</a>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="ph_no_legacy" runat="server" Visible='<%# !Convert.ToBoolean(Eval("HasLegacyMirror")) %>'>
                                            <span class="fg-gray">Đang chờ mirror</span>
                                        </asp:PlaceHolder>
                                    </td>
                                    <td>
                                        <div class="gh-admin-orders__actions-list">
                                            <asp:PlaceHolder ID="ph_open_wait" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanOpenWait")) %>'>
                                                <asp:LinkButton ID="btn_open_wait" runat="server" CssClass="gh-admin-orders__action-btn" CommandName="openwait" CommandArgument='<%# Eval("ActionOrderId") %>'>Kích hoạt chờ</asp:LinkButton>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_view_wait" runat="server" Visible='<%# Convert.ToBoolean(Eval("IsWaitingExchange")) %>'>
                                                <a class="gh-admin-orders__action-link" href="<%=WaitExchangeUrl %>">Mở chờ thanh toán</a>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_cancel_wait" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanCancelWait")) %>'>
                                                <asp:LinkButton ID="btn_cancel_wait" runat="server" CssClass="gh-admin-orders__action-btn" CommandName="cancelwait" CommandArgument='<%# Eval("ActionOrderId") %>'>Hủy chờ</asp:LinkButton>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_deliver" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanMarkDelivered")) %>'>
                                                <asp:LinkButton ID="btn_deliver" runat="server" CssClass="gh-admin-orders__action-btn" CommandName="deliver" CommandArgument='<%# Eval("ActionOrderId") %>'>Đã giao</asp:LinkButton>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_cancel_order" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanCancelOrder")) %>'>
                                                <asp:LinkButton ID="btn_cancel_order" runat="server" CssClass="gh-admin-orders__action-btn" CommandName="cancelorder" CommandArgument='<%# Eval("ActionOrderId") %>' OnClientClick="return confirm('Hủy đơn này?');">Hủy đơn</asp:LinkButton>
                                            </asp:PlaceHolder>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_no_orders" runat="server" Visible="false">
                            <tr>
                                <td colspan="8" class="gh-admin-orders__empty">Chưa có đơn phù hợp trong lớp dữ liệu <code>/gianhang</code>.</td>
                            </tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
