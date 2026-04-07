<%@ Page Title="Khách hàng /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="khach-hang.aspx.cs" Inherits="gianhang_admin_gianhang_khach_hang" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-customers{display:grid;gap:18px}.gh-admin-customers__hero,.gh-admin-customers__card{border:1px solid #ecdcdc;border-radius:20px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-admin-customers__hero{padding:20px 22px;background:radial-gradient(720px 220px at 0% 0%, rgba(56,189,248,.14), transparent 60%),linear-gradient(180deg,#f7fdff 0%,#fff 100%);display:flex;justify-content:space-between;gap:16px;align-items:flex-start;flex-wrap:wrap}
        .gh-admin-customers__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#ecfeff;border:1px solid #bae6fd;color:#0f766e;font-size:12px;font-weight:800;letter-spacing:.03em;text-transform:uppercase}
        .gh-admin-customers__title{margin:10px 0 6px;color:#123047;font-size:30px;line-height:1.1;font-weight:900}.gh-admin-customers__sub{margin:0;color:#4b6576;font-size:14px;line-height:1.6;max-width:760px}
        .gh-admin-customers__actions{display:flex;gap:10px;flex-wrap:wrap}.gh-admin-customers__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;border:1px solid transparent;text-decoration:none!important;font-size:13px;font-weight:800}.gh-admin-customers__btn--primary{color:#fff!important;background:linear-gradient(135deg,#0284c7,#0ea5e9);box-shadow:0 14px 28px rgba(14,165,233,.18)}.gh-admin-customers__btn--soft{color:#123047!important;background:#fff;border-color:#d7e8f0}
        .gh-admin-customers__filters{padding:16px;display:grid;grid-template-columns:minmax(0,1fr) auto auto;gap:12px;align-items:end}.gh-admin-customers__field{display:grid;gap:6px}.gh-admin-customers__field label{color:#4b6576;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.03em}.gh-admin-customers__input{min-height:42px;border-radius:14px;border:1px solid #d9dee7;background:#fff;padding:0 14px;color:#1f2937;font-size:14px;outline:none}.gh-admin-customers__input:focus{border-color:#0ea5e9;box-shadow:0 0 0 3px rgba(14,165,233,.12)}
        .gh-admin-customers__submit,.gh-admin-customers__reset{min-height:42px;border-radius:14px;padding:0 16px;font-size:13px;font-weight:800}.gh-admin-customers__submit{border:0;background:linear-gradient(135deg,#0284c7,#0ea5e9);color:#fff}.gh-admin-customers__reset{border:1px solid #e2e8f0;background:#fff;color:#123047}
        .gh-admin-customers__summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:12px}.gh-admin-customers__summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}.gh-admin-customers__summary-card small{display:block;color:#4b6576;font-size:12px;text-transform:uppercase;font-weight:700}.gh-admin-customers__summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}.gh-admin-customers__summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-admin-customers__table-wrap{overflow:auto}.gh-admin-customers__table{width:100%;border-collapse:collapse}.gh-admin-customers__table th,.gh-admin-customers__table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top}.gh-admin-customers__table th{color:#4b6576;font-size:12px;text-transform:uppercase;letter-spacing:.03em}.gh-admin-customers__meta strong{display:block;color:#111827;font-size:15px;line-height:1.4}.gh-admin-customers__meta span{display:block;margin-top:4px;color:#6b7280;font-size:13px}.gh-admin-customers__actions-list{display:flex;gap:8px;flex-wrap:wrap}.gh-admin-customers__action-link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #dbe2ea;background:#fff;color:#123047!important;font-size:12px;font-weight:800;text-decoration:none!important}.gh-admin-customers__action-link--primary{border-color:#bae6fd;background:#f3fbff}.gh-admin-customers__empty{padding:22px 12px;color:#6b7280;text-align:center}
        @media (max-width:767px){.gh-admin-customers__filters{grid-template-columns:1fr}.gh-admin-customers__title{font-size:26px}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-customers">
        <section class="gh-admin-customers__hero">
            <div>
                <div class="gh-admin-customers__kicker">CRM native /gianhang</div>
                <h1 class="gh-admin-customers__title">Khách hàng /gianhang</h1>
                <p class="gh-admin-customers__sub">Trang này gom lớp khách native của gian hàng để mình nhìn rõ khách nào đã được bridge sang CRM admin, khách nào mới chỉ xuất hiện trong đơn native hoặc booking native.</p>
            </div>
            <div class="gh-admin-customers__actions">
                <a class="gh-admin-customers__btn gh-admin-customers__btn--primary" href="<%=AdminCustomersUrl %>">Mở CRM admin</a>
                <a class="gh-admin-customers__btn gh-admin-customers__btn--soft" href="<%=NativeCustomersUrl %>">Mở lớp native</a>
                <a class="gh-admin-customers__btn gh-admin-customers__btn--soft" href="<%=PersonHubUrl %>">Hồ sơ người</a>
                <a class="gh-admin-customers__btn gh-admin-customers__btn--soft" href="<%=OrdersUrl %>">Đơn gian hàng</a>
                <a class="gh-admin-customers__btn gh-admin-customers__btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <section class="gh-admin-customers__card">
            <div class="gh-admin-customers__filters">
                <div class="gh-admin-customers__field">
                    <label for="<%= txt_search.ClientID %>">Tìm khách</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-admin-customers__input" MaxLength="120" placeholder="Tên khách, số điện thoại hoặc tài khoản buyer"></asp:TextBox>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc khách" CssClass="gh-admin-customers__submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-admin-customers__reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-admin-customers__summary">
            <div class="gh-admin-customers__summary-card"><small>Tổng khách</small><strong><%=TotalCustomers.ToString("#,##0") %></strong><span>Các khách native gom từ đơn và lịch hẹn.</span></div>
            <div class="gh-admin-customers__summary-card"><small>Có số điện thoại</small><strong><%=WithPhoneCount.ToString("#,##0") %></strong><span>Dễ bridge sang CRM admin và hồ sơ người.</span></div>
            <div class="gh-admin-customers__summary-card"><small>Đã vào CRM admin</small><strong><%=AdminMirrorCount.ToString("#,##0") %></strong><span>Khách có điểm mở thẳng sang chi tiết CRM admin.</span></div>
            <div class="gh-admin-customers__summary-card"><small>Tổng đơn</small><strong><%=TotalOrderCount.ToString("#,##0") %></strong><span>Số lượt mua phát sinh trên buyer-flow/native order.</span></div>
            <div class="gh-admin-customers__summary-card"><small>Tổng lịch hẹn</small><strong><%=TotalBookingCount.ToString("#,##0") %></strong><span>Booking public đã đổ vào cùng lớp khách này.</span></div>
            <div class="gh-admin-customers__summary-card"><small>Doanh thu gộp</small><strong><%=GianHangReport_cl.FormatCurrency(RevenueTotal) %> đ</strong><span>Ước lượng theo đơn native /gianhang.</span></div>
        </div>

        <section class="gh-admin-customers__card">
            <div class="gh-admin-customers__table-wrap">
                <table class="gh-admin-customers__table">
                    <thead>
                        <tr>
                            <th style="min-width:230px;">Khách</th>
                            <th style="min-width:140px;">Buyer</th>
                            <th style="min-width:90px;">Đơn</th>
                            <th style="min-width:90px;">Lịch</th>
                            <th style="min-width:140px;">Doanh thu</th>
                            <th style="min-width:150px;">Tương tác gần nhất</th>
                            <th style="min-width:190px;">Bridge admin</th>
                            <th style="min-width:260px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_rows" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="gh-admin-customers__meta">
                                            <strong><%# Eval("DisplayName") %></strong>
                                            <span>SĐT: <%# Eval("Phone") %></span>
                                        </div>
                                    </td>
                                    <td><%# Eval("BuyerAccount") %></td>
                                    <td><strong><%# Eval("OrderCountText") %></strong></td>
                                    <td><strong><%# Eval("BookingCountText") %></strong></td>
                                    <td><strong><%# Eval("RevenueText") %></strong></td>
                                    <td><%# Eval("LastInteractionText") %></td>
                                    <td><%# Eval("MirrorText") %></td>
                                    <td>
                                        <div class="gh-admin-customers__actions-list">
                                            <a class="gh-admin-customers__action-link" href="<%# Eval("NativeDetailUrl") %>">Chi tiết workspace</a>
                                            <a class="gh-admin-customers__action-link gh-admin-customers__action-link--primary" href="<%# Eval("AdminDetailUrl") %>">Mở CRM admin</a>
                                            <a class="gh-admin-customers__action-link" href="<%# Eval("PersonHubUrl") %>">Hồ sơ người</a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <tr><td colspan="8" class="gh-admin-customers__empty">Hiện chưa có khách native nào khớp với bộ lọc hiện tại.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
