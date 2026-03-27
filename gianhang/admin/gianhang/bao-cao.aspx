<%@ Page Title="Báo cáo gian hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="bao-cao.aspx.cs" Inherits="gianhang_admin_gianhang_bao_cao" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-report {
            display: grid;
            gap: 18px;
        }

        .gh-admin-report__hero,
        .gh-admin-report__filters,
        .gh-admin-report__card,
        .gh-admin-report__table-card {
            border: 1px solid #eadfe2;
            border-radius: 20px;
            background: #fff;
            box-shadow: 0 18px 34px rgba(15, 23, 42, 0.06);
        }

        .gh-admin-report__hero {
            padding: 20px 22px;
            background:
                radial-gradient(820px 240px at 100% 0%, rgba(255, 132, 84, .14), transparent 64%),
                linear-gradient(180deg, #fff9f7 0%, #ffffff 100%);
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 16px;
            flex-wrap: wrap;
        }

        .gh-admin-report__kicker {
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

        .gh-admin-report__title {
            margin: 10px 0 6px;
            color: #7f1d1d;
            font-size: 30px;
            line-height: 1.1;
            font-weight: 900;
        }

        .gh-admin-report__sub,
        .gh-admin-report__range {
            margin: 0;
            color: #8d5d5d;
            font-size: 14px;
            line-height: 1.6;
        }

        .gh-admin-report__actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gh-admin-report__btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 14px;
            text-decoration: none !important;
            font-size: 13px;
            font-weight: 800;
            border: 1px solid transparent;
        }

        .gh-admin-report__btn--primary {
            color: #fff !important;
            background: linear-gradient(135deg, #d73a31, #ef6b41);
            box-shadow: 0 14px 28px rgba(215, 58, 49, .18);
        }

        .gh-admin-report__btn--soft {
            color: #7f1d1d !important;
            background: #fff;
            border-color: #f0d7d9;
        }

        .gh-admin-report__filters {
            padding: 16px;
            display: grid;
            grid-template-columns: repeat(5, minmax(0, 1fr));
            gap: 12px;
            align-items: end;
        }

        .gh-admin-report__field {
            display: grid;
            gap: 6px;
        }

        .gh-admin-report__field label {
            color: #8d5d5d;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        .gh-admin-report__input {
            min-height: 42px;
            border-radius: 14px;
            border: 1px solid #d9dee7;
            background: #fff;
            padding: 0 14px;
            color: #1f2937;
            font-size: 14px;
            outline: none;
        }

        .gh-admin-report__input:focus {
            border-color: #ef6b41;
            box-shadow: 0 0 0 3px rgba(239, 107, 65, .12);
        }

        .gh-admin-report__submit,
        .gh-admin-report__reset {
            min-height: 42px;
            border-radius: 14px;
            padding: 0 16px;
            font-size: 13px;
            font-weight: 800;
        }

        .gh-admin-report__submit {
            border: 0;
            background: linear-gradient(135deg, #d73a31, #ef6b41);
            color: #fff;
        }

        .gh-admin-report__reset {
            border: 1px solid #eadfe2;
            background: #fff;
            color: #7f1d1d;
        }

        .gh-admin-report__stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 12px;
        }

        .gh-admin-report__stat {
            border: 1px solid #ebeff5;
            border-radius: 18px;
            background: #fff;
            padding: 16px;
        }

        .gh-admin-report__stat small {
            display: block;
            color: #8d5d5d;
            font-size: 12px;
            text-transform: uppercase;
            font-weight: 700;
        }

        .gh-admin-report__stat strong {
            display: block;
            margin-top: 8px;
            color: #1f2937;
            font-size: 28px;
            line-height: 1.05;
            font-weight: 900;
        }

        .gh-admin-report__grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }

        .gh-admin-report__link-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 12px;
        }

        .gh-admin-report__link-card {
            display: block;
            padding: 14px 16px;
            border: 1px solid #ebeff5;
            border-radius: 18px;
            background: #fff;
            color: #1f2937;
            text-decoration: none !important;
            box-shadow: 0 16px 30px rgba(15, 23, 42, 0.05);
        }

        .gh-admin-report__link-card small {
            display: block;
            color: #8d5d5d;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .gh-admin-report__link-card strong {
            display: block;
            margin-top: 6px;
            font-size: 17px;
            color: #111827;
        }

        .gh-admin-report__link-card span {
            display: block;
            margin-top: 5px;
            color: #6b7280;
            font-size: 13px;
            line-height: 1.6;
        }

        .gh-admin-report__table-card {
            overflow: hidden;
        }

        .gh-admin-report__head {
            padding: 16px 18px;
            border-bottom: 1px solid #eef2f7;
        }

        .gh-admin-report__head h3 {
            margin: 0;
            color: #1f2937;
            font-size: 18px;
        }

        .gh-admin-report__head p {
            margin: 5px 0 0;
            color: #6b7280;
            font-size: 13px;
        }

        .gh-admin-report__table-wrap {
            overflow: auto;
        }

        .gh-admin-report__table {
            width: 100%;
            border-collapse: collapse;
        }

        .gh-admin-report__table th,
        .gh-admin-report__table td {
            padding: 12px 10px;
            border-bottom: 1px solid #edf1f6;
            text-align: left;
            vertical-align: top;
        }

        .gh-admin-report__table th {
            color: #8d5d5d;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        .gh-admin-report__badge {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
            white-space: nowrap;
        }

        .gh-admin-report__badge--pending { background: #eff6ff; color: #2563eb; }
        .gh-admin-report__badge--waiting { background: #fff1f2; color: #be123c; }
        .gh-admin-report__badge--success { background: #ecfdf3; color: #15803d; }
        .gh-admin-report__badge--done { background: #e0f2fe; color: #0369a1; }
        .gh-admin-report__badge--cancelled { background: #f3f4f6; color: #6b7280; }
        .gh-admin-report__badge--booking-pending { background: #fff7ed; color: #c2410c; }
        .gh-admin-report__badge--booking-confirmed { background: #eff6ff; color: #2563eb; }
        .gh-admin-report__badge--booking-done { background: #ecfdf3; color: #15803d; }

        .gh-admin-report__empty {
            padding: 20px 18px;
            color: #6b7280;
        }

        @media (max-width: 1100px) {
            .gh-admin-report__filters {
                grid-template-columns: repeat(3, minmax(0, 1fr));
            }

            .gh-admin-report__grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 767px) {
            .gh-admin-report__title {
                font-size: 26px;
            }

            .gh-admin-report__filters {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-report">
        <section class="gh-admin-report__hero">
            <div>
                <div class="gh-admin-report__kicker">Dashboard native /gianhang</div>
                <h1 class="gh-admin-report__title">Báo cáo gian hàng</h1>
                <p class="gh-admin-report__sub">Dùng chung lõi báo cáo của <code>/gianhang</code>, nhưng hiển thị ngay trong không gian quản trị level 2.</p>
                <p class="gh-admin-report__range"><%=RangeLabel %></p>
            </div>
            <div class="gh-admin-report__actions">
                <a class="gh-admin-report__btn gh-admin-report__btn--primary" href="<%=OrdersUrl %>">Mở đơn gian hàng</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=ContentUrl %>">Nội dung /gianhang</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=BookingsUrl %>">Lịch hẹn /gianhang</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=UtilityUrl %>">Tiện ích /gianhang</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=WaitUrl %>">Chờ thanh toán</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=BuyerFlowUrl %>">Buyer-flow / Đơn mua</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=PublicUrl %>" target="_blank">Trang công khai</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=LegacyInvoiceUrl %>">Hóa đơn admin</a>
                <a class="gh-admin-report__btn gh-admin-report__btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <section class="gh-admin-report__filters">
            <div class="gh-admin-report__field">
                <label for="<%= txt_month.ClientID %>">Tháng</label>
                <asp:TextBox ID="txt_month" runat="server" CssClass="gh-admin-report__input" TextMode="Month"></asp:TextBox>
            </div>
            <div class="gh-admin-report__field">
                <label for="<%= txt_from.ClientID %>">Từ ngày</label>
                <asp:TextBox ID="txt_from" runat="server" CssClass="gh-admin-report__input" TextMode="Date"></asp:TextBox>
            </div>
            <div class="gh-admin-report__field">
                <label for="<%= txt_to.ClientID %>">Đến ngày</label>
                <asp:TextBox ID="txt_to" runat="server" CssClass="gh-admin-report__input" TextMode="Date"></asp:TextBox>
            </div>
            <asp:Button ID="btn_filter" runat="server" Text="Lọc báo cáo" CssClass="gh-admin-report__submit" OnClick="btn_filter_Click" />
            <asp:Button ID="btn_clear" runat="server" Text="Toàn thời gian" CssClass="gh-admin-report__reset" CausesValidation="false" OnClick="btn_clear_Click" />
        </section>

        <div class="gh-admin-report__stats">
            <div class="gh-admin-report__stat">
                <small>Doanh thu gộp</small>
                <strong><%=GianHangReport_cl.FormatCurrency(RevenueGross) %> đ</strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Tổng đơn</small>
                <strong><%=TotalOrders.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Chờ trao đổi</small>
                <strong><%=WaitingExchange.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Đã giao</small>
                <strong><%=DeliveredOrders.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Sản phẩm</small>
                <strong><%=ProductCount.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Dịch vụ</small>
                <strong><%=ServiceCount.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Lượt xem</small>
                <strong><%=TotalViews.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Lịch hẹn</small>
                <strong><%=BookingTotal.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Hóa đơn admin</small>
                <strong><%=LegacyInvoiceCount.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Doanh thu admin</small>
                <strong><%=GianHangReport_cl.FormatCurrency(LegacyRevenueGross) %> đ</strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Hóa đơn mirror</small>
                <strong><%=LegacyMirroredInvoiceCount.ToString("#,##0") %></strong>
            </div>
            <div class="gh-admin-report__stat">
                <small>Doanh thu mirror</small>
                <strong><%=GianHangReport_cl.FormatCurrency(LegacyMirroredRevenueGross) %> đ</strong>
            </div>
        </div>

        <div class="gh-admin-report__link-grid">
            <a class="gh-admin-report__link-card" href="<%=ContentUrl %>">
                <small>Lớp native</small>
                <strong>Nội dung /gianhang</strong>
                <span>Xem sản phẩm, dịch vụ native và đối chiếu những bản ghi đã mirror sang quản lý bài viết admin.</span>
            </a>
            <a class="gh-admin-report__link-card" href="<%=CustomersUrl %>">
                <small>Lớp native</small>
                <strong>Khách hàng /gianhang</strong>
                <span>Đi sang lớp khách native để nhìn ai đã phát sinh đơn, booking và đã có CRM admin tương ứng hay chưa.</span>
            </a>
            <a class="gh-admin-report__link-card" href="<%=BookingsUrl %>">
                <small>Lớp native</small>
                <strong>Lịch hẹn /gianhang</strong>
                <span>Mở lớp booking native rồi chuyển tiếp sang lịch hẹn admin khi cần thao tác vận hành sâu hơn.</span>
            </a>
            <a class="gh-admin-report__link-card" href="<%=LegacyInvoiceUrl %>">
                <small>Vận hành admin</small>
                <strong>Hóa đơn admin đã mirror</strong>
                <span>Mở ngay lớp hóa đơn legacy đã nhận dữ liệu từ không gian /gianhang để tiếp tục xử lý nghiệp vụ.</span>
            </a>
            <a class="gh-admin-report__link-card" href="<%=LegacyProductStatsUrl %>">
                <small>Phân tích</small>
                <strong>Thống kê sản phẩm</strong>
                <span>Xem lớp thống kê sản phẩm phía admin dựa trên dữ liệu đã bridge sang hóa đơn vận hành.</span>
            </a>
            <a class="gh-admin-report__link-card" href="<%=LegacyServiceStatsUrl %>">
                <small>Phân tích</small>
                <strong>Thống kê dịch vụ</strong>
                <span>Theo dõi hiệu quả dịch vụ giữa storefront native và công cụ vận hành.</span>
            </a>
        </div>

        <div class="gh-admin-report__grid">
            <section class="gh-admin-report__table-card">
                <div class="gh-admin-report__head">
                    <h3>Đơn gần đây từ /gianhang</h3>
                    <p>Ưu tiên mở vào hóa đơn admin nếu mirror đã sẵn sàng.</p>
                </div>
                <div class="gh-admin-report__table-wrap">
                    <table class="gh-admin-report__table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Khách</th>
                                <th>Trạng thái</th>
                                <th>Tổng tiền</th>
                                <th>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_recent_orders" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <strong>#<%# Eval("DisplayId") %></strong><br />
                                            <span class="fg-gray"><%# Eval("OrderedAtText") %></span>
                                        </td>
                                        <td><%# Eval("BuyerDisplay") %></td>
                                        <td><span class='gh-admin-report__badge <%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                        <td><%# Eval("TotalAmountText") %> đ</td>
                                        <td><a href="<%# Eval("DetailUrl") %>">Xem</a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <asp:PlaceHolder ID="ph_empty_orders" runat="server" Visible="false">
                    <div class="gh-admin-report__empty">Chưa có đơn phù hợp trong khoảng thời gian này.</div>
                </asp:PlaceHolder>
            </section>

            <section class="gh-admin-report__table-card">
                <div class="gh-admin-report__head">
                    <h3>Lịch hẹn gần đây từ /gianhang</h3>
                    <p>Booking public được bridge sang admin để xử lý tiếp tại CRM/lịch hẹn.</p>
                </div>
                <div class="gh-admin-report__table-wrap">
                    <table class="gh-admin-report__table">
                        <thead>
                            <tr>
                                <th>Khách</th>
                                <th>Dịch vụ</th>
                                <th>Thời gian</th>
                                <th>Trạng thái</th>
                                <th>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_recent_bookings" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <strong><%# Eval("CustomerName") %></strong><br />
                                            <span class="fg-gray"><%# Eval("Phone") %></span>
                                        </td>
                                        <td><%# Eval("ServiceName") %></td>
                                        <td><%# Eval("BookingTimeText") %></td>
                                        <td><span class='gh-admin-report__badge <%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                        <td><a href="<%# Eval("DetailUrl") %>">Xem</a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <asp:PlaceHolder ID="ph_empty_bookings" runat="server" Visible="false">
                    <div class="gh-admin-report__empty">Chưa có lịch hẹn phù hợp trong khoảng thời gian này.</div>
                </asp:PlaceHolder>
            </section>
        </div>
    </div>
</asp:Content>
