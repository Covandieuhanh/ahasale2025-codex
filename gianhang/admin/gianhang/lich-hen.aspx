<%@ Page Title="Lịch hẹn /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="lich-hen.aspx.cs" Inherits="gianhang_admin_gianhang_lich_hen" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-bookings{display:grid;gap:18px}.gh-admin-bookings__hero,.gh-admin-bookings__card{border:1px solid #ecdcdc;border-radius:20px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-admin-bookings__hero{padding:20px 22px;background:radial-gradient(720px 220px at 0% 0%, rgba(168,85,247,.12), transparent 60%),linear-gradient(180deg,#fbf7ff 0%,#fff 100%);display:flex;justify-content:space-between;gap:16px;align-items:flex-start;flex-wrap:wrap}
        .gh-admin-bookings__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#f3e8ff;border:1px solid #e9d5ff;color:#7e22ce;font-size:12px;font-weight:800;letter-spacing:.03em;text-transform:uppercase}
        .gh-admin-bookings__title{margin:10px 0 6px;color:#3b0764;font-size:30px;line-height:1.1;font-weight:900}.gh-admin-bookings__sub{margin:0;color:#6b4f7b;font-size:14px;line-height:1.6;max-width:760px}
        .gh-admin-bookings__actions{display:flex;gap:10px;flex-wrap:wrap}.gh-admin-bookings__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;border:1px solid transparent;text-decoration:none!important;font-size:13px;font-weight:800}.gh-admin-bookings__btn--primary{color:#fff!important;background:linear-gradient(135deg,#9333ea,#a855f7);box-shadow:0 14px 28px rgba(168,85,247,.18)}.gh-admin-bookings__btn--soft{color:#3b0764!important;background:#fff;border-color:#e9d5ff}
        .gh-admin-bookings__filters{padding:16px;display:grid;grid-template-columns:minmax(0,1fr) 210px auto auto;gap:12px;align-items:end}.gh-admin-bookings__field{display:grid;gap:6px}.gh-admin-bookings__field label{color:#6b4f7b;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.03em}.gh-admin-bookings__input{min-height:42px;border-radius:14px;border:1px solid #d9dee7;background:#fff;padding:0 14px;color:#1f2937;font-size:14px;outline:none}.gh-admin-bookings__input:focus{border-color:#a855f7;box-shadow:0 0 0 3px rgba(168,85,247,.12)}
        .gh-admin-bookings__submit,.gh-admin-bookings__reset{min-height:42px;border-radius:14px;padding:0 16px;font-size:13px;font-weight:800}.gh-admin-bookings__submit{border:0;background:linear-gradient(135deg,#9333ea,#a855f7);color:#fff}.gh-admin-bookings__reset{border:1px solid #e2e8f0;background:#fff;color:#3b0764}
        .gh-admin-bookings__summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:12px}.gh-admin-bookings__summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}.gh-admin-bookings__summary-card small{display:block;color:#6b4f7b;font-size:12px;text-transform:uppercase;font-weight:700}.gh-admin-bookings__summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}.gh-admin-bookings__summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-admin-bookings__table-wrap{overflow:auto}.gh-admin-bookings__table{width:100%;border-collapse:collapse}.gh-admin-bookings__table th,.gh-admin-bookings__table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top}.gh-admin-bookings__table th{color:#6b4f7b;font-size:12px;text-transform:uppercase;letter-spacing:.03em}
        .gh-admin-bookings__badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}.gh-admin-bookings__badge--pending{background:#fff7ed;color:#c2410c}.gh-admin-bookings__badge--confirmed{background:#eff6ff;color:#1d4ed8}.gh-admin-bookings__badge--done{background:#ecfdf3;color:#15803d}.gh-admin-bookings__badge--cancelled{background:#f3f4f6;color:#6b7280}
        .gh-admin-bookings__actions-list{display:flex;gap:8px;flex-wrap:wrap}.gh-admin-bookings__action-link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #e5d5ff;background:#fff;color:#3b0764!important;font-size:12px;font-weight:800;text-decoration:none!important}.gh-admin-bookings__action-link--primary{background:#faf5ff}
        .gh-admin-bookings__empty{padding:22px 12px;color:#6b7280;text-align:center}
        @media (max-width:991px){.gh-admin-bookings__filters{grid-template-columns:1fr}.gh-admin-bookings__title{font-size:26px}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-bookings">
        <section class="gh-admin-bookings__hero">
            <div>
                <div class="gh-admin-bookings__kicker">Booking native /gianhang</div>
                <h1 class="gh-admin-bookings__title">Lịch hẹn /gianhang</h1>
                <p class="gh-admin-bookings__sub">Trang này gom booking native của gian hàng để đội vận hành level 2 nhìn rõ lịch nào đã bridge sang mô-đun lịch hẹn admin, lịch nào còn đang ở lớp native của storefront.</p>
            </div>
            <div class="gh-admin-bookings__actions">
                <a class="gh-admin-bookings__btn gh-admin-bookings__btn--primary" href="<%=AdminBookingsUrl %>">Mở lịch hẹn admin</a>
                <a class="gh-admin-bookings__btn gh-admin-bookings__btn--soft" href="<%=NativeBookingsUrl %>">Mở lớp native</a>
                <a class="gh-admin-bookings__btn gh-admin-bookings__btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                <a class="gh-admin-bookings__btn gh-admin-bookings__btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <section class="gh-admin-bookings__card">
            <div class="gh-admin-bookings__filters">
                <div class="gh-admin-bookings__field">
                    <label for="<%= txt_search.ClientID %>">Tìm lịch hẹn</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-admin-bookings__input" MaxLength="120" placeholder="Tên khách, số điện thoại, dịch vụ hoặc ID"></asp:TextBox>
                </div>
                <div class="gh-admin-bookings__field">
                    <label for="<%= ddl_status.ClientID %>">Trạng thái</label>
                    <asp:DropDownList ID="ddl_status" runat="server" CssClass="gh-admin-bookings__input">
                        <asp:ListItem Value="all" Text="Tất cả"></asp:ListItem>
                        <asp:ListItem Value="Chưa xác nhận" Text="Chưa xác nhận"></asp:ListItem>
                        <asp:ListItem Value="Đã xác nhận" Text="Đã xác nhận"></asp:ListItem>
                        <asp:ListItem Value="Đã hoàn thành" Text="Đã hoàn thành"></asp:ListItem>
                        <asp:ListItem Value="Đã hủy" Text="Đã hủy"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc lịch" CssClass="gh-admin-bookings__submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-admin-bookings__reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-admin-bookings__summary">
            <div class="gh-admin-bookings__summary-card"><small>Tổng lịch</small><strong><%=TotalBookings.ToString("#,##0") %></strong><span>Tất cả booking native đang được hiển thị theo bộ lọc.</span></div>
            <div class="gh-admin-bookings__summary-card"><small>Chưa xác nhận</small><strong><%=PendingCount.ToString("#,##0") %></strong><span>Các lịch vừa phát sinh từ storefront hoặc buyer-flow.</span></div>
            <div class="gh-admin-bookings__summary-card"><small>Đã xác nhận</small><strong><%=ConfirmedCount.ToString("#,##0") %></strong><span>Lịch đã có xử lý bước đầu từ vận hành.</span></div>
            <div class="gh-admin-bookings__summary-card"><small>Hoàn thành</small><strong><%=DoneCount.ToString("#,##0") %></strong><span>Lịch đã kết thúc tại cơ sở.</span></div>
            <div class="gh-admin-bookings__summary-card"><small>Đã hủy</small><strong><%=CancelledCount.ToString("#,##0") %></strong><span>Lịch bị hủy từ buyer hoặc nội bộ.</span></div>
            <div class="gh-admin-bookings__summary-card"><small>Đã mirror</small><strong><%=MirroredCount.ToString("#,##0") %></strong><span>Có thể mở thẳng sang màn sửa lịch hẹn admin.</span></div>
        </div>

        <section class="gh-admin-bookings__card">
            <div class="gh-admin-bookings__table-wrap">
                <table class="gh-admin-bookings__table">
                    <thead>
                        <tr>
                            <th style="width:90px;">ID</th>
                            <th style="min-width:220px;">Khách</th>
                            <th style="min-width:180px;">Dịch vụ</th>
                            <th style="min-width:150px;">Tạo lúc</th>
                            <th style="min-width:150px;">Hẹn lúc</th>
                            <th style="min-width:130px;">Trạng thái</th>
                            <th style="min-width:180px;">Bridge admin</th>
                            <th style="min-width:220px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_rows" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><strong>#<%# Eval("NativeId") %></strong></td>
                                    <td><strong><%# Eval("CustomerName") %></strong><br /><span class="fg-gray"><%# Eval("Phone") %></span></td>
                                    <td><%# Eval("ServiceName") %></td>
                                    <td><%# Eval("CreatedAtText") %></td>
                                    <td><%# Eval("ScheduleText") %></td>
                                    <td><span class='<%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                    <td><%# Eval("AdminMirrorText") %></td>
                                    <td>
                                        <div class="gh-admin-bookings__actions-list">
                                            <a class="gh-admin-bookings__action-link" href="<%# Eval("WorkspaceDetailUrl") %>">Chi tiết workspace</a>
                                            <a class="gh-admin-bookings__action-link gh-admin-bookings__action-link--primary" href="<%# Eval("AdminDetailUrl") %>">Mở lịch admin</a>
                                            <a class="gh-admin-bookings__action-link" href="<%=NativeBookingsUrl %>">Mở lớp native</a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <tr><td colspan="8" class="gh-admin-bookings__empty">Hiện chưa có lịch native nào khớp với bộ lọc hiện tại.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
