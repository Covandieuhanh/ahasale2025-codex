<%@ Page Title="Lịch hẹn gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="quan-ly-lich-hen.aspx.cs" Inherits="gianhang_quan_ly_lich_hen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --gh-orange: #f97316;
            --gh-orange-strong: #ea580c;
            --gh-orange-soft: #fff7ed;
            --gh-ink: #102a43;
            --gh-muted: #627d98;
            --gh-line: #d9e2ec;
            --gh-bg: #f5f8fb;
            --gh-card: #ffffff;
            --gh-radius: 18px;
        }

        .gh-booking-shell {
            min-height: 100vh;
            padding: 24px 16px 40px;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(249,115,22,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(251,146,60,.18), transparent 62%),
                var(--gh-bg);
        }

        .gh-booking-wrap {
            max-width: 1180px;
            margin: 0 auto;
        }

        .gh-booking-card,
        .gh-booking-hero {
            background: var(--gh-card);
            border: 1px solid var(--gh-line);
            border-radius: var(--gh-radius);
            box-shadow: 0 16px 36px rgba(16, 42, 67, .08);
        }

        .gh-booking-hero {
            padding: 24px;
            margin-bottom: 18px;
        }

        .gh-booking-eyebrow {
            color: var(--gh-orange-strong);
            font-weight: 800;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
        }

        .gh-booking-title {
            margin: 10px 0 8px;
            color: var(--gh-ink);
            font-size: 32px;
            line-height: 1.15;
            font-weight: 900;
        }

        .gh-booking-sub {
            color: var(--gh-muted);
            font-size: 15px;
            line-height: 1.6;
            max-width: 780px;
        }

        .gh-booking-kpi-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            margin-top: 18px;
        }

        .gh-booking-kpi {
            border: 1px solid var(--gh-line);
            border-radius: 14px;
            padding: 14px 16px;
            background: #fcfdff;
        }

        .gh-booking-kpi-label {
            color: var(--gh-muted);
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .gh-booking-kpi-value {
            margin-top: 6px;
            color: var(--gh-ink);
            font-size: 24px;
            font-weight: 900;
        }

        .gh-booking-card {
            padding: 18px;
        }

        .gh-booking-toolbar {
            display: flex;
            flex-wrap: wrap;
            justify-content: space-between;
            gap: 12px;
            align-items: center;
            margin-bottom: 14px;
        }

        .gh-booking-actions {
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

        .gh-link-btn:hover {
            color: var(--gh-ink);
        }

        .gh-link-btn-primary {
            background: linear-gradient(135deg, #ffb26c, #f97316);
            color: #fff;
            border-color: transparent;
        }

        .gh-link-btn-primary:hover {
            color: #fff;
            background: linear-gradient(135deg, #fb923c, #ea580c);
        }

        .gh-filter-grid {
            display: grid;
            grid-template-columns: 2fr 1fr auto auto;
            gap: 10px;
            margin: 12px 0 16px;
        }

        .gh-filter-input,
        .gh-filter-select {
            min-height: 42px;
            border-radius: 12px;
            border: 1px solid var(--gh-line);
            padding: 0 14px;
            color: var(--gh-ink);
            background: #fff;
        }

        .gh-filter-btn {
            min-height: 42px;
            border-radius: 12px;
            border: 1px solid transparent;
            padding: 0 16px;
            font-weight: 800;
            background: linear-gradient(135deg, #ffb26c, #f97316);
            color: #fff;
        }

        .gh-filter-btn-soft {
            background: #fff;
            color: var(--gh-ink);
            border-color: var(--gh-line);
        }

        .gh-booking-table-wrap {
            overflow: auto;
        }

        .gh-booking-table {
            width: 100%;
            border-collapse: collapse;
        }

        .gh-booking-table th,
        .gh-booking-table td {
            padding: 12px 10px;
            border-bottom: 1px solid #e6eef7;
            text-align: left;
            vertical-align: top;
            color: var(--gh-ink);
        }

        .gh-booking-table th {
            font-size: 12px;
            text-transform: uppercase;
            color: var(--gh-muted);
        }

        .gh-booking-status {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
        }

        .gh-booking-status-pending { background: #fff8e1; color: #8a6116; }
        .gh-booking-status-confirmed { background: #edf4ff; color: #175cd3; }
        .gh-booking-status-done { background: #edfdf4; color: #137333; }
        .gh-booking-status-cancelled { background: #ffecec; color: #b42318; }

        .gh-booking-row-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .gh-booking-row-action {
            display: inline-flex;
            align-items: center;
            min-height: 32px;
            padding: 0 10px;
            border-radius: 10px;
            border: 1px solid var(--gh-line);
            font-size: 12px;
            font-weight: 800;
            text-decoration: none;
            color: var(--gh-ink);
            background: #fff;
        }

        .gh-booking-empty {
            padding: 28px 18px;
            text-align: center;
            color: var(--gh-muted);
            font-weight: 700;
        }

        .gh-booking-alert {
            border-radius: 14px;
            padding: 14px 16px;
            margin-bottom: 16px;
            font-weight: 700;
        }

        .gh-booking-alert-success {
            background: #edfdf4;
            border: 1px solid #b7ebc6;
            color: #137333;
        }

        .gh-muted {
            color: var(--gh-muted);
        }

        @media (max-width: 960px) {
            .gh-filter-grid {
                grid-template-columns: 1fr 1fr;
            }
        }

        @media (max-width: 900px) {
            .gh-booking-kpi-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 640px) {
            .gh-booking-kpi-grid,
            .gh-filter-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="gh-booking-shell">
        <div class="gh-booking-wrap">
            <asp:PlaceHolder ID="ph_notice" runat="server" Visible="false">
                <div class="gh-booking-alert gh-booking-alert-success">
                    <asp:Label ID="lb_notice" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <section class="gh-booking-hero">
                <div class="gh-booking-eyebrow">Lịch hẹn</div>
                <h1 class="gh-booking-title">Lịch hẹn khách đặt</h1>
                <div class="gh-booking-sub">Theo dõi toàn bộ lịch hẹn phát sinh từ các dịch vụ của gian hàng.</div>

                <div class="gh-booking-kpi-grid">
                    <div class="gh-booking-kpi">
                        <div class="gh-booking-kpi-label">Tổng lịch</div>
                        <div class="gh-booking-kpi-value"><asp:Label ID="lb_total" runat="server" Text="0"></asp:Label></div>
                    </div>
                    <div class="gh-booking-kpi">
                        <div class="gh-booking-kpi-label">Chờ xác nhận</div>
                        <div class="gh-booking-kpi-value"><asp:Label ID="lb_pending" runat="server" Text="0"></asp:Label></div>
                    </div>
                    <div class="gh-booking-kpi">
                        <div class="gh-booking-kpi-label">Đã xác nhận</div>
                        <div class="gh-booking-kpi-value"><asp:Label ID="lb_confirmed" runat="server" Text="0"></asp:Label></div>
                    </div>
                    <div class="gh-booking-kpi">
                        <div class="gh-booking-kpi-label">Hoàn thành</div>
                        <div class="gh-booking-kpi-value"><asp:Label ID="lb_done" runat="server" Text="0"></asp:Label></div>
                    </div>
                </div>
            </section>

            <section class="gh-booking-card">
                <div class="gh-booking-toolbar">
                    <div>
                        <strong><asp:Label ID="lb_gianhang_name" runat="server" Text="Gian hàng"></asp:Label></strong>
                        <div class="gh-muted">Quản lý lịch hẹn khách đặt từ các dịch vụ đang hoạt động.</div>
                    </div>
                    <div class="gh-booking-actions">
                        <a href="/gianhang/default.aspx" class="gh-link-btn">Trang công khai</a>
                        <a href="/gianhang/quan-ly-tin/Default.aspx" class="gh-link-btn">Quản lý tin</a>
                        <a href="/gianhang/dat-lich.aspx" class="gh-link-btn gh-link-btn-primary">Mở form đặt lịch</a>
                    </div>
                </div>

                <div class="gh-filter-grid">
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-filter-input" MaxLength="100" placeholder="Tìm khách hàng, số điện thoại hoặc dịch vụ"></asp:TextBox>
                    <asp:DropDownList ID="ddl_status" runat="server" CssClass="gh-filter-select">
                        <asp:ListItem Text="Tất cả trạng thái" Value=""></asp:ListItem>
                        <asp:ListItem Text="Chưa xác nhận" Value="Chưa xác nhận"></asp:ListItem>
                        <asp:ListItem Text="Đã xác nhận" Value="Đã xác nhận"></asp:ListItem>
                        <asp:ListItem Text="Đã hoàn thành" Value="Đã hoàn thành"></asp:ListItem>
                        <asp:ListItem Text="Đã hủy" Value="Đã hủy"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="btn_filter" runat="server" CssClass="gh-filter-btn" Text="Lọc" OnClick="btn_filter_Click" />
                    <asp:Button ID="btn_reset" runat="server" CssClass="gh-filter-btn gh-filter-btn-soft" Text="Làm mới" OnClick="btn_reset_Click" CausesValidation="false" />
                </div>

                <div class="gh-booking-table-wrap">
                    <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                        <div class="gh-booking-empty">Chưa có lịch hẹn nào phù hợp.</div>
                    </asp:PlaceHolder>

                    <asp:Repeater ID="rp_bookings" runat="server">
                        <HeaderTemplate>
                            <table class="gh-booking-table">
                                <thead>
                                    <tr>
                                        <th>Mã</th>
                                        <th>Khách hàng</th>
                                        <th>Dịch vụ</th>
                                        <th>Thời gian</th>
                                        <th>Trạng thái</th>
                                        <th>Ghi chú</th>
                                        <th>Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>#<%# Eval("id") %></td>
                                <td>
                                    <strong><%# Eval("ten_khach") %></strong><br />
                                    <span class="gh-muted"><%# Eval("sdt") %></span>
                                </td>
                                <td><%# Eval("dich_vu") %></td>
                                <td><%# Eval("thoi_gian_hen_text") %></td>
                                <td><span class='<%# Eval("status_css") %>'><%# Eval("trang_thai") %></span></td>
                                <td><%# Eval("ghi_chu_hien_thi") %></td>
                                <td>
                                    <div class="gh-booking-row-actions">
                                        <asp:PlaceHolder ID="phConfirm" runat="server" Visible='<%# Eval("show_confirm") %>'>
                                            <a class="gh-booking-row-action" href="<%# Eval("url_confirm") %>">Xác nhận</a>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phDone" runat="server" Visible='<%# Eval("show_done") %>'>
                                            <a class="gh-booking-row-action" href="<%# Eval("url_done") %>">Hoàn thành</a>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phCancel" runat="server" Visible='<%# Eval("show_cancel") %>'>
                                            <a class="gh-booking-row-action" href="<%# Eval("url_cancel") %>">Hủy</a>
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
