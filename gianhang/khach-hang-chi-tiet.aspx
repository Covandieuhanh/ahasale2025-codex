<%@ Page Title="Chi tiết khách hàng gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="khach-hang-chi-tiet.aspx.cs" Inherits="gianhang_khach_hang_chi_tiet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --gh-orange: #f97316;
            --gh-orange-soft: #fff7ed;
            --gh-ink: #102a43;
            --gh-muted: #627d98;
            --gh-line: #d9e2ec;
            --gh-bg: #f5f8fb;
            --gh-card: #ffffff;
        }

        .gh-detail-shell {
            min-height: 100vh;
            padding: 24px 16px 40px;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(249,115,22,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(251,146,60,.18), transparent 62%),
                var(--gh-bg);
        }

        .gh-detail-wrap { max-width: 1180px; margin: 0 auto; }
        .gh-card {
            background: var(--gh-card);
            border: 1px solid var(--gh-line);
            border-radius: 20px;
            box-shadow: 0 16px 36px rgba(16, 42, 67, .08);
        }

        .gh-hero {
            padding: 24px;
            margin-bottom: 18px;
            background: linear-gradient(135deg, rgba(255,178,108,.15), rgba(249,115,22,.07));
        }

        .gh-eyebrow {
            color: #ea580c;
            font-weight: 800;
            font-size: 12px;
            letter-spacing: .08em;
            text-transform: uppercase;
        }

        .gh-hero-top {
            display: flex;
            flex-wrap: wrap;
            justify-content: space-between;
            gap: 16px;
            align-items: flex-start;
        }

        .gh-title {
            margin: 10px 0 8px;
            font-size: 34px;
            line-height: 1.1;
            color: var(--gh-ink);
            font-weight: 900;
        }

        .gh-sub {
            color: var(--gh-muted);
            font-size: 15px;
            line-height: 1.6;
            max-width: 760px;
        }

        .gh-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .gh-link-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            padding: 0 16px;
            border-radius: 12px;
            border: 1px solid var(--gh-line);
            color: var(--gh-ink);
            text-decoration: none;
            font-weight: 800;
            background: #fff;
        }

        .gh-link-btn-primary {
            background: linear-gradient(135deg, #ffb26c, #f97316);
            border-color: transparent;
            color: #fff;
        }

        .gh-kpi-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            margin-top: 18px;
        }

        .gh-kpi {
            padding: 16px;
            border-radius: 16px;
            border: 1px solid #fed7aa;
            background: #fff;
        }

        .gh-kpi-label {
            color: var(--gh-muted);
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .06em;
            font-weight: 800;
        }

        .gh-kpi-value {
            margin-top: 8px;
            color: var(--gh-ink);
            font-size: 28px;
            line-height: 1.1;
            font-weight: 900;
        }

        .gh-grid {
            display: grid;
            grid-template-columns: minmax(0, 1.2fr) minmax(0, .8fr);
            gap: 18px;
            margin-top: 18px;
        }

        .gh-section {
            padding: 20px;
        }

        .gh-section-title {
            margin: 0 0 6px;
            color: var(--gh-ink);
            font-size: 28px;
            line-height: 1.1;
            font-weight: 900;
        }

        .gh-section-sub {
            color: var(--gh-muted);
            font-size: 14px;
            margin-bottom: 14px;
        }

        .gh-list {
            display: grid;
            gap: 12px;
        }

        .gh-row {
            display: grid;
            grid-template-columns: minmax(0, 1.5fr) minmax(0, .8fr) auto;
            gap: 12px;
            align-items: center;
            padding: 14px;
            border: 1px solid #e6eef7;
            border-radius: 16px;
            background: #fff;
        }

        .gh-row-name {
            color: var(--gh-ink);
            font-size: 20px;
            line-height: 1.2;
            font-weight: 900;
        }

        .gh-row-sub {
            color: var(--gh-muted);
            font-size: 14px;
            line-height: 1.5;
            margin-top: 4px;
        }

        .gh-row-amount {
            text-align: right;
            color: var(--gh-ink);
            font-weight: 900;
            font-size: 20px;
        }

        .gh-chip {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 32px;
            padding: 0 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
            white-space: nowrap;
        }

        .gh-chip-active { background: #eff6ff; color: #1d4ed8; }
        .gh-chip-info { background: #ecfeff; color: #0f766e; }
        .gh-chip-success { background: #ecfdf5; color: #047857; }
        .gh-chip-warning { background: #fff7ed; color: #c2410c; }
        .gh-chip-muted { background: #f1f5f9; color: #475569; }

        .gh-empty {
            padding: 28px 18px;
            text-align: center;
            color: var(--gh-muted);
            font-weight: 700;
            border: 1px dashed #cbd5e1;
            border-radius: 16px;
            background: #fff;
        }

        .gh-notfound {
            padding: 36px 24px;
            text-align: center;
        }

        @media (max-width: 980px) {
            .gh-kpi-grid,
            .gh-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 760px) {
            .gh-row {
                grid-template-columns: 1fr;
            }

            .gh-row-amount {
                text-align: left;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="gh-detail-shell">
        <div class="gh-detail-wrap">
            <asp:PlaceHolder ID="ph_not_found" runat="server" Visible="false">
                <section class="gh-card gh-notfound">
                    <div class="gh-eyebrow">Khách hàng</div>
                    <h1 class="gh-title">Không tìm thấy khách hàng</h1>
                    <div class="gh-sub">Khách hàng này không còn tồn tại hoặc đường dẫn không hợp lệ.</div>
                    <div class="gh-actions" style="justify-content:center;margin-top:18px;">
                        <asp:HyperLink ID="lnk_not_found" runat="server" CssClass="gh-link-btn gh-link-btn-primary" Text="Quay lại danh sách khách hàng" />
                    </div>
                </section>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_detail" runat="server" Visible="false">
                <section class="gh-card gh-hero">
                    <div class="gh-eyebrow">Chi tiết khách hàng</div>
                    <div class="gh-hero-top">
                        <div>
                            <h1 class="gh-title"><asp:Literal ID="lb_name" runat="server" /></h1>
                            <div class="gh-sub">
                                Số điện thoại: <b><asp:Literal ID="lb_phone" runat="server" /></b>
                                <br />
                                Tài khoản mua: <b><asp:Literal ID="lb_buyer_account" runat="server" /></b>
                                <br />
                                Tương tác gần nhất: <b><asp:Literal ID="lb_last_interaction" runat="server" /></b>
                            </div>
                        </div>
                        <div class="gh-actions">
                            <asp:HyperLink ID="lnk_back_customers" runat="server" CssClass="gh-link-btn" Text="Danh sách khách hàng" />
                            <asp:HyperLink ID="lnk_back_orders" runat="server" CssClass="gh-link-btn" Text="Đơn bán" />
                            <asp:HyperLink ID="lnk_back_bookings" runat="server" CssClass="gh-link-btn" Text="Lịch hẹn" />
                            <asp:PlaceHolder ID="ph_call" runat="server" Visible="false">
                                <asp:HyperLink ID="lnk_call" runat="server" CssClass="gh-link-btn gh-link-btn-primary" Text="Gọi ngay" />
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="gh-kpi-grid">
                        <div class="gh-kpi">
                            <div class="gh-kpi-label">Đơn bán</div>
                            <div class="gh-kpi-value"><asp:Literal ID="lb_order_count" runat="server" /></div>
                        </div>
                        <div class="gh-kpi">
                            <div class="gh-kpi-label">Lịch hẹn</div>
                            <div class="gh-kpi-value"><asp:Literal ID="lb_booking_count" runat="server" /></div>
                        </div>
                        <div class="gh-kpi">
                            <div class="gh-kpi-label">Doanh thu</div>
                            <div class="gh-kpi-value"><asp:Literal ID="lb_revenue_total" runat="server" /></div>
                        </div>
                        <div class="gh-kpi">
                            <div class="gh-kpi-label">Nhịp vận hành</div>
                            <div class="gh-kpi-value"><asp:Literal ID="lb_last_interaction_kpi" runat="server" /></div>
                        </div>
                    </div>
                </section>

                <div class="gh-grid">
                    <section class="gh-card gh-section">
                        <h2 class="gh-section-title">Đơn gần nhất</h2>
                        <div class="gh-section-sub">Danh sách đơn bán gắn với khách hàng này.</div>

                        <asp:PlaceHolder ID="ph_empty_orders" runat="server" Visible="false">
                            <div class="gh-empty">Khách này chưa có đơn bán nào.</div>
                        </asp:PlaceHolder>

                        <div class="gh-list">
                            <asp:Repeater ID="rp_orders" runat="server">
                                <ItemTemplate>
                                    <div class="gh-row">
                                        <div>
                                            <div class="gh-row-name">Đơn #<%# Eval("OrderId") %></div>
                                            <div class="gh-row-sub">
                                                <%# Eval("BuyerDisplay") %> • <%# Eval("Phone") %><br />
                                                <%# GianHangReport_cl.FormatDateTime((DateTime?)Eval("OrderedAt")) %>
                                            </div>
                                        </div>
                                        <div class="gh-row-amount"><%# GianHangReport_cl.FormatCurrency(Convert.ToDecimal(Eval("TotalAmount"))) %> đ</div>
                                        <div><span class="<%# ResolveOrderStatusCss(Eval("StatusGroup")) %>"><%# Eval("StatusText") %></span></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </section>

                    <section class="gh-card gh-section">
                        <h2 class="gh-section-title">Lịch hẹn gần nhất</h2>
                        <div class="gh-section-sub">Danh sách lịch hẹn của khách hàng này.</div>

                        <asp:PlaceHolder ID="ph_empty_bookings" runat="server" Visible="false">
                            <div class="gh-empty">Khách này chưa có lịch hẹn nào.</div>
                        </asp:PlaceHolder>

                        <div class="gh-list">
                            <asp:Repeater ID="rp_bookings" runat="server">
                                <ItemTemplate>
                                    <div class="gh-row">
                                        <div>
                                            <div class="gh-row-name">Lịch #<%# Eval("BookingId") %></div>
                                            <div class="gh-row-sub">
                                                <%# Eval("ServiceName") %><br />
                                                <%# GianHangReport_cl.FormatDateTime((DateTime?)Eval("ScheduleAt")) %>
                                            </div>
                                        </div>
                                        <div class="gh-row-sub">
                                            Tạo lúc: <%# GianHangReport_cl.FormatDateTime((DateTime?)Eval("CreatedAt")) %><br />
                                            SĐT: <%# Eval("Phone") %>
                                        </div>
                                        <div><span class="<%# ResolveBookingStatusCss(Eval("StatusText")) %>"><%# Eval("StatusText") %></span></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </section>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
