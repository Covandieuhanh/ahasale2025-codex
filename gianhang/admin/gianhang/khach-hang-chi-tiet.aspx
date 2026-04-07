<%@ Page Title="Chi tiết khách hàng /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="khach-hang-chi-tiet.aspx.cs" Inherits="gianhang_admin_gianhang_khach_hang_chi_tiet" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-wc-shell{display:grid;gap:18px}
        .gh-wc-hero,.gh-wc-card{border:1px solid #d7e8f0;border-radius:22px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-wc-hero{padding:22px;background:radial-gradient(760px 240px at 0% 0%, rgba(14,165,233,.12), transparent 60%),linear-gradient(180deg,#f7fdff 0%,#fff 100%);display:flex;justify-content:space-between;align-items:flex-start;gap:18px;flex-wrap:wrap}
        .gh-wc-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#ecfeff;border:1px solid #bae6fd;color:#0f766e;font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.03em}
        .gh-wc-title{margin:10px 0 6px;color:#123047;font-size:30px;line-height:1.12;font-weight:900}
        .gh-wc-sub{margin:0;color:#4b6576;font-size:14px;line-height:1.6;max-width:760px}
        .gh-wc-actions{display:flex;gap:10px;flex-wrap:wrap}
        .gh-wc-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none!important;font-size:13px;font-weight:800;border:1px solid transparent}
        .gh-wc-btn--primary{background:linear-gradient(135deg,#0284c7,#0ea5e9);color:#fff!important;box-shadow:0 14px 28px rgba(14,165,233,.18)}
        .gh-wc-btn--soft{background:#fff;color:#123047!important;border-color:#d7e8f0}
        .gh-wc-summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px}
        .gh-wc-summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}
        .gh-wc-summary-card small{display:block;color:#4b6576;font-size:12px;text-transform:uppercase;font-weight:700}
        .gh-wc-summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}
        .gh-wc-summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-wc-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(280px,1fr));gap:16px}
        .gh-wc-meta{display:grid;gap:10px}
        .gh-wc-meta-item strong{display:block;color:#111827;font-size:14px}
        .gh-wc-meta-item span{display:block;margin-top:4px;color:#6b7280;font-size:13px}
        .gh-wc-table{width:100%;border-collapse:collapse}
        .gh-wc-table th,.gh-wc-table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top}
        .gh-wc-table th{color:#4b6576;font-size:12px;text-transform:uppercase;letter-spacing:.03em}
        .gh-wc-badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}
        .gh-wc-badge--active{background:#eff6ff;color:#1d4ed8}
        .gh-wc-badge--waiting{background:#fff7ed;color:#c2410c}
        .gh-wc-badge--success{background:#ecfdf3;color:#15803d}
        .gh-wc-badge--muted{background:#f3f4f6;color:#6b7280}
        .gh-wc-link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #dbe2ea;background:#fff;color:#123047!important;font-size:12px;font-weight:800;text-decoration:none!important}
        .gh-wc-empty{padding:28px 18px;text-align:center;color:#6b7280}
        @media (max-width:767px){.gh-wc-title{font-size:26px}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-wc-shell">
        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
            <section class="gh-wc-card">
                <div class="gh-wc-empty">
                    Chưa chọn đúng khách hàng workspace để xem chi tiết. Mình quay lại danh sách khách hàng /gianhang rồi mở lại từ đúng dòng dữ liệu nhé.
                    <div style="margin-top:14px;">
                        <a class="gh-wc-btn gh-wc-btn--primary" href="<%=CustomersUrl %>">Về khách hàng /gianhang</a>
                    </div>
                </div>
            </section>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="ph_content" runat="server" Visible="false">
            <section class="gh-wc-hero">
                <div>
                    <div class="gh-wc-kicker">Khách hàng native /gianhang</div>
                    <h1 class="gh-wc-title"><%=CustomerTitle %></h1>
                    <p class="gh-wc-sub">Từ đây mình nhìn được một khách hàng xuyên suốt giữa buyer-flow, booking native, CRM admin và hồ sơ người trong cùng workspace.</p>
                </div>
                <div class="gh-wc-actions">
                    <a class="gh-wc-btn gh-wc-btn--primary" href="<%=AdminDetailUrl %>">Mở CRM admin</a>
                    <a class="gh-wc-btn gh-wc-btn--soft" href="<%=CustomersUrl %>">Danh sách khách /gianhang</a>
                    <a class="gh-wc-btn gh-wc-btn--soft" href="<%=OrdersUrl %>">Đơn gian hàng</a>
                    <a class="gh-wc-btn gh-wc-btn--soft" href="<%=BookingsUrl %>">Lịch hẹn /gianhang</a>
                    <a class="gh-wc-btn gh-wc-btn--soft" href="<%=PersonHubUrl %>">Hồ sơ người</a>
                    <a class="gh-wc-btn gh-wc-btn--soft" href="<%=NativeDetailUrl %>">Mở native</a>
                </div>
            </section>

            <div class="gh-wc-summary">
                <div class="gh-wc-summary-card"><small>Số điện thoại</small><strong><%=PhoneText %></strong><span>Dùng để bridge sang CRM admin và hồ sơ người.</span></div>
                <div class="gh-wc-summary-card"><small>Buyer account</small><strong><%=BuyerAccountText %></strong><span>Tài khoản Home/buyer đi cùng các đơn native.</span></div>
                <div class="gh-wc-summary-card"><small>Tổng đơn</small><strong><%=OrderCount.ToString("#,##0") %></strong><span>Số đơn native /gianhang đang gắn với khách này.</span></div>
                <div class="gh-wc-summary-card"><small>Tổng lịch hẹn</small><strong><%=BookingCount.ToString("#,##0") %></strong><span>Số booking storefront đã gắn vào cùng khách này.</span></div>
                <div class="gh-wc-summary-card"><small>Doanh thu gộp</small><strong><%=RevenueText %></strong><span>Ước lượng theo đơn native đã tạo.</span></div>
                <div class="gh-wc-summary-card"><small>Tương tác gần nhất</small><strong><%=LastInteractionText %></strong><span><%=MirrorText %></span></div>
            </div>

            <div class="gh-wc-grid">
                <section class="gh-wc-card">
                    <h3 style="margin:0 0 14px;color:#123047;">Tổng quan khách hàng</h3>
                    <div class="gh-wc-meta">
                        <div class="gh-wc-meta-item"><strong>Tên hiển thị</strong><span><%=CustomerTitle %></span></div>
                        <div class="gh-wc-meta-item"><strong>Số điện thoại</strong><span><%=PhoneText %></span></div>
                        <div class="gh-wc-meta-item"><strong>Buyer / Home</strong><span><%=BuyerAccountText %></span></div>
                        <div class="gh-wc-meta-item"><strong>Bridge admin</strong><span><%=MirrorText %></span></div>
                        <div class="gh-wc-meta-item"><strong>Lớp native</strong><span><a class="gh-wc-link" href="<%=NativeCustomersUrl %>">Mở khách hàng native</a></span></div>
                    </div>
                </section>

                <section class="gh-wc-card">
                    <h3 style="margin:0 0 14px;color:#123047;">Việc nên làm tiếp</h3>
                    <div class="gh-wc-meta">
                        <div class="gh-wc-meta-item"><strong>Chăm sóc nội bộ</strong><span>Mở CRM admin để ghi nhận lịch sử, nhóm khách và thao tác bán hàng.</span></div>
                        <div class="gh-wc-meta-item"><strong>Liên kết định danh</strong><span>Mở Hồ sơ người nếu cần gắn Home hoặc đồng bộ vai trò trong workspace.</span></div>
                        <div class="gh-wc-meta-item"><strong>Kiểm tra buyer-flow</strong><span>Xem đơn và lịch hẹn gần đây để biết khách đến từ đâu và đang ở bước nào.</span></div>
                    </div>
                </section>
            </div>

            <section class="gh-wc-card">
                <h3 style="margin:0 0 14px;color:#123047;">Đơn native gần đây</h3>
                <table class="gh-wc-table">
                    <thead>
                        <tr>
                            <th>Mã đơn</th>
                            <th>Buyer</th>
                            <th>SĐT</th>
                            <th>Giá trị</th>
                            <th>Thời gian</th>
                            <th>Trạng thái</th>
                            <th>Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_orders" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><strong><%# Eval("OrderIdText") %></strong></td>
                                    <td><%# Eval("BuyerDisplay") %></td>
                                    <td><%# Eval("Phone") %></td>
                                    <td><strong><%# Eval("AmountText") %></strong></td>
                                    <td><%# Eval("OrderedAtText") %></td>
                                    <td><span class='<%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                    <td><a class="gh-wc-link" href="<%# Eval("WorkspaceOrdersUrl") %>">Mở danh sách đơn</a></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_orders_empty" runat="server" Visible="false">
                            <tr><td colspan="7" class="gh-wc-empty">Khách này chưa có đơn native nào trong workspace hiện tại.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </section>

            <section class="gh-wc-card">
                <h3 style="margin:0 0 14px;color:#123047;">Lịch hẹn gần đây</h3>
                <table class="gh-wc-table">
                    <thead>
                        <tr>
                            <th>Mã lịch</th>
                            <th>Dịch vụ</th>
                            <th>Hẹn lúc</th>
                            <th>Tạo lúc</th>
                            <th>Trạng thái</th>
                            <th>Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_bookings" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><strong><%# Eval("BookingIdText") %></strong></td>
                                    <td><%# Eval("ServiceName") %></td>
                                    <td><%# Eval("ScheduleText") %></td>
                                    <td><%# Eval("CreatedAtText") %></td>
                                    <td><span class='<%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                    <td><a class="gh-wc-link" href="<%# Eval("WorkspaceDetailUrl") %>">Mở chi tiết lịch</a></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_bookings_empty" runat="server" Visible="false">
                            <tr><td colspan="6" class="gh-wc-empty">Khách này chưa có lịch hẹn native nào trong workspace hiện tại.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </section>
        </asp:PlaceHolder>
    </div>
</asp:Content>
