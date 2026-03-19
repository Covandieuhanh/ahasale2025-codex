<%@ Page Title="Lịch hẹn khách đặt" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="quan-ly-lich-hen.aspx.cs" Inherits="shop_quan_ly_lich_hen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --shop-orange: #ee4d2d;
            --shop-orange-soft: #fff1ea;
            --shop-ink: #102a43;
            --shop-muted: #627d98;
            --shop-line: #d9e2ec;
            --shop-bg: #f5f8fb;
            --shop-card: #ffffff;
            --shop-radius: 18px;
        }

        .booking-admin-shell {
            min-height: 100vh;
            padding: 24px 16px 40px;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(238,77,45,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(255,122,69,.18), transparent 62%),
                var(--shop-bg);
        }

        .booking-admin-wrap {
            max-width: 1180px;
            margin: 0 auto;
        }

        .booking-admin-card,
        .booking-admin-hero {
            background: var(--shop-card);
            border: 1px solid var(--shop-line);
            border-radius: var(--shop-radius);
            box-shadow: 0 16px 36px rgba(16, 42, 67, .08);
        }

        .booking-admin-hero {
            padding: 24px;
            margin-bottom: 18px;
        }

        .booking-admin-eyebrow {
            color: var(--shop-orange);
            font-weight: 800;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
        }

        .booking-admin-title {
            margin: 10px 0 8px;
            color: var(--shop-ink);
            font-size: 32px;
            line-height: 1.15;
            font-weight: 900;
        }

        .booking-admin-sub {
            color: var(--shop-muted);
            font-size: 15px;
            line-height: 1.6;
            max-width: 760px;
        }

        .booking-kpi-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            margin-top: 18px;
        }

        .booking-kpi {
            border: 1px solid var(--shop-line);
            border-radius: 14px;
            padding: 14px 16px;
            background: #fcfdff;
        }

        .booking-kpi-label {
            color: var(--shop-muted);
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .booking-kpi-value {
            margin-top: 6px;
            color: var(--shop-ink);
            font-size: 24px;
            font-weight: 900;
        }

        .booking-admin-card {
            padding: 18px;
        }

        .booking-admin-toolbar {
            display: flex;
            flex-wrap: wrap;
            justify-content: space-between;
            gap: 12px;
            align-items: center;
            margin-bottom: 14px;
        }

        .booking-admin-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .booking-link-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            padding: 0 16px;
            border-radius: 12px;
            border: 1px solid var(--shop-line);
            color: var(--shop-ink);
            text-decoration: none;
            font-weight: 800;
            background: #fff;
        }

        .booking-link-btn-primary {
            background: linear-gradient(135deg, #ff9a3d, #ee4d2d);
            color: #fff;
            border-color: transparent;
        }

        .booking-table-wrap {
            overflow: auto;
        }

        .booking-table {
            width: 100%;
            border-collapse: collapse;
        }

        .booking-table th,
        .booking-table td {
            padding: 12px 10px;
            border-bottom: 1px solid #e6eef7;
            text-align: left;
            vertical-align: top;
            color: var(--shop-ink);
        }

        .booking-table th {
            font-size: 12px;
            text-transform: uppercase;
            color: var(--shop-muted);
        }

        .booking-status {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
        }

        .booking-status-pending { background: #fff8e1; color: #8a6116; }
        .booking-status-confirmed { background: #edf4ff; color: #175cd3; }
        .booking-status-done { background: #edfdf4; color: #137333; }
        .booking-status-cancelled { background: #ffecec; color: #b42318; }

        .booking-row-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .booking-row-action {
            display: inline-flex;
            align-items: center;
            min-height: 32px;
            padding: 0 10px;
            border-radius: 10px;
            border: 1px solid var(--shop-line);
            font-size: 12px;
            font-weight: 800;
            text-decoration: none;
            color: var(--shop-ink);
            background: #fff;
        }

        .booking-empty {
            padding: 28px 18px;
            text-align: center;
            color: var(--shop-muted);
            font-weight: 700;
        }

        .booking-alert {
            border-radius: 14px;
            padding: 14px 16px;
            margin-bottom: 16px;
            font-weight: 700;
        }

        .booking-alert-success {
            background: #edfdf4;
            border: 1px solid #b7ebc6;
            color: #137333;
        }

        @media (max-width: 900px) {
            .booking-kpi-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 640px) {
            .booking-kpi-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="booking-admin-shell">
        <div class="booking-admin-wrap">
            <asp:PlaceHolder ID="ph_notice" runat="server" Visible="false">
                <div class="booking-alert booking-alert-success">
                    <asp:Label ID="lb_notice" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <section class="booking-admin-hero">
                <div class="booking-admin-eyebrow">Shop Level 1</div>
                <h1 class="booking-admin-title">Lịch hẹn khách đặt</h1>
                <div class="booking-admin-sub">
                    Theo dõi các lịch hẹn khách hàng đặt từ <b>/home</b>. Tại đây bạn có thể duyệt, xác nhận, đổi trạng thái và ghi nhận lịch sử. Khi nâng cấp lên Level 2, toàn bộ lịch hẹn này tiếp tục được dùng lại trong <b>/gianhang/admin</b>.
                </div>

                <div class="booking-kpi-grid">
                    <div class="booking-kpi">
                        <div class="booking-kpi-label">Tổng lịch</div>
                        <div class="booking-kpi-value"><asp:Label ID="lb_total" runat="server" Text="0"></asp:Label></div>
                    </div>
                    <div class="booking-kpi">
                        <div class="booking-kpi-label">Chưa xác nhận</div>
                        <div class="booking-kpi-value"><asp:Label ID="lb_pending" runat="server" Text="0"></asp:Label></div>
                    </div>
                    <div class="booking-kpi">
                        <div class="booking-kpi-label">Đã xác nhận</div>
                        <div class="booking-kpi-value"><asp:Label ID="lb_confirmed" runat="server" Text="0"></asp:Label></div>
                    </div>
                    <div class="booking-kpi">
                        <div class="booking-kpi-label">Đã đến</div>
                        <div class="booking-kpi-value"><asp:Label ID="lb_done" runat="server" Text="0"></asp:Label></div>
                    </div>
                </div>
            </section>

            <section class="booking-admin-card">
                <div class="booking-admin-toolbar">
                    <div>
                        <strong><asp:Label ID="lb_shop_name" runat="server" Text="Shop"></asp:Label></strong>
                        <div class="text-muted">Quản lý lịch hẹn khách đặt từ /home trong /shop</div>
                    </div>
                    <div class="booking-admin-actions">
                        <span class="booking-link-btn booking-link-btn-primary">Đặt lịch tại /home</span>
                        <asp:PlaceHolder ID="ph_upgrade_link" runat="server" Visible="false">
                            <a href="/shop/nang-cap-level2.aspx" class="booking-link-btn">Nâng cấp Level 2</a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_advanced_link" runat="server" Visible="false">
                            <a href="/gianhang/admin/login.aspx" class="booking-link-btn">Mở /gianhang/admin</a>
                        </asp:PlaceHolder>
                    </div>
                </div>

                <div class="booking-table-wrap">
                    <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                        <div class="booking-empty">Chưa có lịch hẹn nào.</div>
                    </asp:PlaceHolder>

                    <asp:Repeater ID="rp_bookings" runat="server">
                        <HeaderTemplate>
                            <table class="booking-table">
                                <thead>
                                    <tr>
                                        <th>Mã</th>
                                        <th>Khách hàng</th>
                                        <th>Dịch vụ</th>
                                        <th>Thời gian</th>
                                        <th>Trạng thái</th>
                                        <th>Nguồn</th>
                                        <th>Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>#<%# Eval("id") %></td>
                                <td>
                                    <strong><%# Eval("tenkhachhang") %></strong><br />
                                    <span class="text-muted"><%# Eval("sdt") %></span>
                                </td>
                                <td><%# Eval("tendichvu_taithoidiemnay") %></td>
                                <td><%# Eval("ngaydat_text") %></td>
                                <td><span class='<%# Eval("status_css") %>'><%# Eval("trangthai") %></span></td>
                                <td><%# Eval("nguongoc") %></td>
                                <td>
                                    <div class="booking-row-actions">
                                        <asp:PlaceHolder ID="phConfirm" runat="server" Visible='<%# Eval("show_confirm") %>'>
                                            <a class="booking-row-action" href="<%# Eval("url_confirm") %>">Xác nhận</a>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phDone" runat="server" Visible='<%# Eval("show_done") %>'>
                                            <a class="booking-row-action" href="<%# Eval("url_done") %>">Đã đến</a>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phCancel" runat="server" Visible='<%# Eval("show_cancel") %>'>
                                            <a class="booking-row-action" href="<%# Eval("url_cancel") %>">Hủy</a>
                                        </asp:PlaceHolder>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
