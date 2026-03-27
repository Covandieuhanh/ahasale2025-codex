<%@ Page Title="Báo cáo gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="bao-cao.aspx.cs" Inherits="gianhang_bao_cao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --gh-orange-700: #f97316;
            --gh-orange-600: #fb923c;
            --gh-orange-50: #fff7ed;
            --gh-ink-900: #102a43;
            --gh-ink-700: #334e68;
            --gh-ink-500: #627d98;
            --gh-line: #d9e2ec;
            --gh-bg: #f5f8fb;
            --gh-card: #ffffff;
            --gh-shadow: 0 16px 36px rgba(16, 42, 67, .08);
            --radius-lg: 18px;
            --radius-md: 14px;
            --radius-sm: 10px;
        }

        .gh-report-shell {
            min-height: 100vh;
            color: var(--gh-ink-900);
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(249,115,22,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(251,146,60,.18), transparent 62%),
                var(--gh-bg);
        }

        .gh-report-main {
            max-width: 1220px;
            margin: 0 auto;
            padding: 18px 16px 28px;
            display: grid;
            gap: 16px;
        }

        .gh-report-hero {
            background: linear-gradient(135deg, var(--gh-orange-700), var(--gh-orange-600));
            border-radius: var(--radius-lg);
            color: #fff;
            padding: 16px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 18px;
            box-shadow: var(--gh-shadow);
        }

        .gh-report-hero h1 {
            margin: 0 0 6px;
            font-size: 26px;
            line-height: 1.15;
        }

        .gh-report-hero p {
            margin: 0;
            font-size: 14px;
            opacity: .95;
        }

        .gh-report-meta {
            font-size: 13px;
            margin-top: 6px;
            opacity: .92;
        }

        .gh-report-link {
            display: inline-flex;
            align-items: center;
            min-height: 38px;
            padding: 0 14px;
            border-radius: 999px;
            border: 1px solid rgba(255,255,255,.48);
            background: rgba(255,255,255,.12);
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            white-space: nowrap;
            text-decoration: none;
        }

        .gh-report-link:hover {
            color: #fff;
            background: rgba(255,255,255,.2);
        }

        .gh-report-filters {
            background: var(--gh-card);
            border: 1px solid var(--gh-line);
            border-radius: var(--radius-md);
            padding: 14px;
            display: grid;
            grid-template-columns: repeat(5, minmax(0, 1fr));
            gap: 12px;
            align-items: end;
        }

        .gh-filter-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .gh-filter-field label {
            font-size: 12px;
            font-weight: 700;
            color: var(--gh-ink-500);
        }

        .gh-filter-input {
            height: 38px;
            border-radius: 10px;
            border: 1px solid var(--gh-line);
            background: #fff;
            padding: 0 12px;
            font-size: 14px;
            color: var(--gh-ink-900);
        }

        .gh-filter-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gh-btn {
            min-height: 38px;
            padding: 0 14px;
            border-radius: 12px;
            border: 1px solid transparent;
            font-weight: 800;
            font-size: 13px;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
        }

        .gh-btn-primary {
            color: #fff;
            background: linear-gradient(135deg, var(--gh-orange-700), var(--gh-orange-600));
        }

        .gh-btn-soft {
            color: var(--gh-ink-900);
            background: #fff;
            border-color: var(--gh-line);
        }

        .gh-report-note {
            min-height: 38px;
            padding: 10px 12px;
            border-radius: 12px;
            background: var(--gh-orange-50);
            color: #9a3412;
            font-size: 12px;
            line-height: 1.5;
        }

        .gh-stats {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
        }

        .gh-stat-card,
        .gh-card {
            background: var(--gh-card);
            border: 1px solid var(--gh-line);
            border-radius: var(--radius-md);
            box-shadow: var(--gh-shadow);
        }

        .gh-stat-card {
            padding: 14px 16px;
        }

        .gh-stat-label {
            color: var(--gh-ink-500);
            font-size: 12px;
            text-transform: uppercase;
            font-weight: 700;
        }

        .gh-stat-value {
            margin-top: 6px;
            color: var(--gh-ink-900);
            font-size: 24px;
            font-weight: 900;
            line-height: 1.15;
        }

        .gh-card-head {
            padding: 14px 16px;
            border-bottom: 1px solid #e7edf4;
        }

        .gh-card-title {
            margin: 0;
            font-size: 18px;
            line-height: 1.2;
            color: var(--gh-ink-900);
        }

        .gh-card-sub {
            margin-top: 4px;
            color: var(--gh-ink-500);
            font-size: 13px;
        }

        .gh-card-body {
            padding: 16px;
        }

        .gh-report-grid-2 {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }

        .gh-report-table-wrap {
            overflow: auto;
        }

        .gh-report-table {
            width: 100%;
            border-collapse: collapse;
        }

        .gh-report-table th,
        .gh-report-table td {
            padding: 12px 10px;
            border-bottom: 1px solid #e6eef7;
            text-align: left;
            vertical-align: top;
        }

        .gh-report-table th {
            color: var(--gh-ink-500);
            font-size: 12px;
            text-transform: uppercase;
        }

        .gh-report-status {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
        }

        .gh-report-status-warning { background: #fff8e1; color: #8a6116; }
        .gh-report-status-info { background: #edf4ff; color: #175cd3; }
        .gh-report-status-success { background: #edfdf4; color: #137333; }
        .gh-report-status-danger { background: #ffecec; color: #b42318; }
        .gh-report-status-neutral { background: #f3f6fa; color: #486581; }

        @media (max-width: 1100px) {
            .gh-report-filters {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .gh-stats {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .gh-report-grid-2 {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 640px) {
            .gh-report-filters,
            .gh-stats {
                grid-template-columns: 1fr;
            }

            .gh-report-hero {
                flex-direction: column;
                align-items: flex-start;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="gh-report-shell">
        <main class="gh-report-main">
            <section class="gh-report-hero">
                <div>
                    <h1>Báo cáo gian hàng</h1>
                    <p>Theo dõi đơn bán, lịch hẹn và hiệu suất vận hành của gian hàng.</p>
                    <div class="gh-report-meta">
                        <asp:Label ID="lb_range_label" runat="server" Text="Khoảng thời gian: Toàn thời gian"></asp:Label>
                    </div>
                </div>
                <a href="/gianhang/default.aspx" class="gh-report-link">Trang công khai</a>
            </section>

            <section class="gh-report-filters">
                <div class="gh-filter-field">
                    <label for="txt_month">Tháng</label>
                    <asp:TextBox ID="txt_month" runat="server" CssClass="gh-filter-input" TextMode="Month" placeholder="mm/yyyy"></asp:TextBox>
                </div>
                <div class="gh-filter-field">
                    <label for="txt_from">Từ ngày</label>
                    <asp:TextBox ID="txt_from" runat="server" CssClass="gh-filter-input" TextMode="Date" placeholder="dd/mm/yyyy"></asp:TextBox>
                </div>
                <div class="gh-filter-field">
                    <label for="txt_to">Đến ngày</label>
                    <asp:TextBox ID="txt_to" runat="server" CssClass="gh-filter-input" TextMode="Date" placeholder="dd/mm/yyyy"></asp:TextBox>
                </div>
                <div class="gh-filter-field">
                    <label>&nbsp;</label>
                    <div class="gh-filter-actions">
                        <asp:Button ID="btn_filter" runat="server" Text="Lọc báo cáo" CssClass="gh-btn gh-btn-primary" OnClick="btn_filter_Click" />
                        <asp:LinkButton ID="btn_clear" runat="server" Text="Toàn thời gian" CssClass="gh-btn gh-btn-soft" OnClick="btn_clear_Click" />
                    </div>
                </div>
                <div class="gh-filter-field">
                    <label>Ghi chú</label>
                    <div class="gh-report-note">Ưu tiên lọc theo tháng; nếu nhập Từ/Đến thì hệ thống sẽ dùng khoảng ngày đã chọn.</div>
                </div>
            </section>

            <section class="gh-stats">
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Doanh thu phát sinh (VNĐ)</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_revenue_gross" runat="server"></asp:Label></div>
                </article>
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Tổng đơn</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_total_orders" runat="server"></asp:Label></div>
                </article>
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Chờ Trao đổi</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_waiting_exchange" runat="server"></asp:Label></div>
                </article>
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Số lượng đã bán</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_total_sold" runat="server"></asp:Label></div>
                </article>
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Sản phẩm đang hiển thị</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_total_products" runat="server"></asp:Label></div>
                </article>
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Dịch vụ đang hiển thị</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_total_services" runat="server"></asp:Label></div>
                </article>
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Lượt xem lũy kế</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_total_views" runat="server"></asp:Label></div>
                </article>
                <article class="gh-stat-card">
                    <div class="gh-stat-label">Tổng lịch hẹn</div>
                    <div class="gh-stat-value"><asp:Label ID="lb_booking_total" runat="server"></asp:Label></div>
                </article>
            </section>

            <section class="gh-card">
                <div class="gh-card-head">
                    <h2 class="gh-card-title">Trạng thái đơn bán</h2>
                    <div class="gh-card-sub">Tổng hợp theo các trạng thái đơn bán hiện tại.</div>
                </div>
                <div class="gh-card-body">
                    <div class="gh-stats">
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Đã đặt</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_group_pending" runat="server"></asp:Label></div>
                        </article>
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Chờ Trao đổi</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_group_wait" runat="server"></asp:Label></div>
                        </article>
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Đã Trao đổi</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_group_exchanged" runat="server"></asp:Label></div>
                        </article>
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Đã giao</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_group_delivered" runat="server"></asp:Label></div>
                        </article>
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Đã hủy</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_group_cancelled" runat="server"></asp:Label></div>
                        </article>
                    </div>
                </div>
            </section>

            <section class="gh-card">
                <div class="gh-card-head">
                    <h2 class="gh-card-title">Trạng thái lịch hẹn</h2>
                    <div class="gh-card-sub">Theo dõi tỷ lệ xác nhận và hoàn thành của khách đặt lịch.</div>
                </div>
                <div class="gh-card-body">
                    <div class="gh-stats">
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Chờ xác nhận</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_booking_pending" runat="server"></asp:Label></div>
                        </article>
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Đã xác nhận</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_booking_confirmed" runat="server"></asp:Label></div>
                        </article>
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Hoàn thành</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_booking_done" runat="server"></asp:Label></div>
                        </article>
                        <article class="gh-stat-card">
                            <div class="gh-stat-label">Đã hủy</div>
                            <div class="gh-stat-value"><asp:Label ID="lb_booking_cancelled" runat="server"></asp:Label></div>
                        </article>
                    </div>
                </div>
            </section>

            <section class="gh-report-grid-2">
                <section class="gh-card">
                    <div class="gh-card-head">
                        <h2 class="gh-card-title">Đơn gần nhất</h2>
                        <div class="gh-card-sub">Theo dõi các đơn bán mới nhất.</div>
                    </div>
                    <div class="gh-card-body">
                        <div class="gh-report-table-wrap">
                            <asp:PlaceHolder ID="ph_empty_orders" runat="server" Visible="false">
                                <div class="gh-report-note">Chưa có đơn hàng nào trong khoảng lọc hiện tại.</div>
                            </asp:PlaceHolder>
                            <asp:Repeater ID="rp_recent_orders" runat="server">
                                <HeaderTemplate>
                                    <table class="gh-report-table">
                                        <thead>
                                            <tr>
                                                <th>Mã</th>
                                                <th>Khách</th>
                                                <th>Thời gian</th>
                                                <th>Trạng thái</th>
                                                <th>Tổng</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>#<%# Eval("Id") %></td>
                                        <td><%# Eval("BuyerDisplay") %></td>
                                        <td><%# GianHangReport_cl.FormatDateTime((DateTime?)Eval("OrderedAt")) %></td>
                                        <td><span class='<%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                        <td><%# GianHangReport_cl.FormatCurrency(Convert.ToDecimal(Eval("TotalAmount"))) %> đ</td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </section>

                <section class="gh-card">
                    <div class="gh-card-head">
                        <h2 class="gh-card-title">Lịch hẹn gần nhất</h2>
                        <div class="gh-card-sub">Theo dõi các lịch hẹn mới nhất của gian hàng.</div>
                    </div>
                    <div class="gh-card-body">
                        <div class="gh-report-table-wrap">
                            <asp:PlaceHolder ID="ph_empty_bookings" runat="server" Visible="false">
                                <div class="gh-report-note">Chưa có lịch hẹn nào trong khoảng lọc hiện tại.</div>
                            </asp:PlaceHolder>
                            <asp:Repeater ID="rp_recent_bookings" runat="server">
                                <HeaderTemplate>
                                    <table class="gh-report-table">
                                        <thead>
                                            <tr>
                                                <th>Mã</th>
                                                <th>Khách</th>
                                                <th>Dịch vụ</th>
                                                <th>Thời gian</th>
                                                <th>Trạng thái</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>#<%# Eval("Id") %></td>
                                        <td>
                                            <%# Eval("CustomerName") %><br />
                                            <span style="color:#627d98;"><%# Eval("Phone") %></span>
                                        </td>
                                        <td><%# Eval("ServiceName") %></td>
                                        <td><%# GianHangReport_cl.FormatDateTime((DateTime?)Eval("BookingTime")) %></td>
                                        <td><span class='<%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </section>
            </section>
        </main>
    </div>
</asp:Content>
