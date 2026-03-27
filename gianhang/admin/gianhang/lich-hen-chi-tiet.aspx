<%@ Page Title="Chi tiết lịch hẹn /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="lich-hen-chi-tiet.aspx.cs" Inherits="gianhang_admin_gianhang_lich_hen_chi_tiet" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-wb-shell{display:grid;gap:18px}
        .gh-wb-hero,.gh-wb-card{border:1px solid #ead9ff;border-radius:22px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-wb-hero{padding:22px;background:radial-gradient(760px 240px at 0% 0%, rgba(168,85,247,.12), transparent 60%),linear-gradient(180deg,#fbf7ff 0%,#fff 100%);display:flex;justify-content:space-between;align-items:flex-start;gap:18px;flex-wrap:wrap}
        .gh-wb-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#f3e8ff;border:1px solid #e9d5ff;color:#7e22ce;font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.03em}
        .gh-wb-title{margin:10px 0 6px;color:#3b0764;font-size:30px;line-height:1.12;font-weight:900}
        .gh-wb-sub{margin:0;color:#6b4f7b;font-size:14px;line-height:1.6;max-width:760px}
        .gh-wb-actions{display:flex;gap:10px;flex-wrap:wrap}
        .gh-wb-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none!important;font-size:13px;font-weight:800;border:1px solid transparent}
        .gh-wb-btn--primary{background:linear-gradient(135deg,#9333ea,#a855f7);color:#fff!important;box-shadow:0 14px 28px rgba(168,85,247,.18)}
        .gh-wb-btn--soft{background:#fff;color:#3b0764!important;border-color:#ead9ff}
        .gh-wb-summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px}
        .gh-wb-summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}
        .gh-wb-summary-card small{display:block;color:#6b4f7b;font-size:12px;text-transform:uppercase;font-weight:700}
        .gh-wb-summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}
        .gh-wb-summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-wb-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(280px,1fr));gap:16px}
        .gh-wb-meta{display:grid;gap:10px}
        .gh-wb-meta-item strong{display:block;color:#111827;font-size:14px}
        .gh-wb-meta-item span{display:block;margin-top:4px;color:#6b7280;font-size:13px}
        .gh-wb-badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}
        .gh-wb-badge--active{background:#eff6ff;color:#1d4ed8}
        .gh-wb-badge--waiting{background:#fff7ed;color:#c2410c}
        .gh-wb-badge--success{background:#ecfdf3;color:#15803d}
        .gh-wb-badge--muted{background:#f3f4f6;color:#6b7280}
        .gh-wb-empty{padding:28px 18px;text-align:center;color:#6b7280}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-wb-shell">
        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
            <section class="gh-wb-card">
                <div class="gh-wb-empty">
                    Chưa chọn đúng lịch hẹn workspace để xem chi tiết. Mình quay lại danh sách lịch hẹn /gianhang rồi mở lại từ đúng dòng nhé.
                    <div style="margin-top:14px;">
                        <a class="gh-wb-btn gh-wb-btn--primary" href="<%=BookingsUrl %>">Về lịch hẹn /gianhang</a>
                    </div>
                </div>
            </section>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="ph_content" runat="server" Visible="false">
            <section class="gh-wb-hero">
                <div>
                    <div class="gh-wb-kicker">Booking native /gianhang</div>
                    <h1 class="gh-wb-title"><%=CustomerName %></h1>
                    <p class="gh-wb-sub">Màn này giúp đội vận hành level 2 nhìn rõ một lịch hẹn native của workspace, trạng thái bridge sang admin và các lối mở tiếp sang khách hàng, hồ sơ người và dịch vụ public.</p>
                </div>
                <div class="gh-wb-actions">
                    <a class="gh-wb-btn gh-wb-btn--primary" href="<%=AdminDetailUrl %>">Mở lịch admin</a>
                    <a class="gh-wb-btn gh-wb-btn--soft" href="<%=BookingsUrl %>">Danh sách lịch /gianhang</a>
                    <a class="gh-wb-btn gh-wb-btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                    <a class="gh-wb-btn gh-wb-btn--soft" href="<%=PersonHubUrl %>">Hồ sơ người</a>
                    <a class="gh-wb-btn gh-wb-btn--soft" href="<%=PublicServiceUrl %>" target="_blank">Mở dịch vụ public</a>
                </div>
            </section>

            <div class="gh-wb-summary">
                <div class="gh-wb-summary-card"><small>Khách hàng</small><strong><%=CustomerName %></strong><span>Số điện thoại: <%=PhoneText %></span></div>
                <div class="gh-wb-summary-card"><small>Dịch vụ</small><strong><%=ServiceName %></strong><span>Dữ liệu dịch vụ hiện đang gắn với lịch này.</span></div>
                <div class="gh-wb-summary-card"><small>Hẹn lúc</small><strong><%=ScheduleText %></strong><span>Thời gian mà storefront hoặc nội bộ đã đặt.</span></div>
                <div class="gh-wb-summary-card"><small>Tạo lúc</small><strong><%=CreatedAtText %></strong><span>Thời điểm bản ghi native được tạo ra.</span></div>
                <div class="gh-wb-summary-card"><small>Trạng thái</small><strong><span class="<%=StatusCss %>"><%=StatusText %></span></strong><span><%=MirrorText %></span></div>
            </div>

            <div class="gh-wb-grid">
                <section class="gh-wb-card">
                    <h3 style="margin:0 0 14px;color:#3b0764;">Thông tin lịch hẹn</h3>
                    <div class="gh-wb-meta">
                        <div class="gh-wb-meta-item"><strong>Khách hàng</strong><span><%=CustomerName %></span></div>
                        <div class="gh-wb-meta-item"><strong>Số điện thoại</strong><span><%=PhoneText %></span></div>
                        <div class="gh-wb-meta-item"><strong>Dịch vụ</strong><span><%=ServiceName %></span></div>
                        <div class="gh-wb-meta-item"><strong>Bridge admin</strong><span><%=MirrorText %></span></div>
                    </div>
                </section>

                <section class="gh-wb-card">
                    <h3 style="margin:0 0 14px;color:#3b0764;">Ghi chú vận hành</h3>
                    <div class="gh-wb-meta">
                        <div class="gh-wb-meta-item"><strong>Ghi chú</strong><span><%=NoteText %></span></div>
                        <div class="gh-wb-meta-item"><strong>Lớp native</strong><span><a class="gh-wb-btn gh-wb-btn--soft" href="<%=NativeBookingsUrl %>">Mở quản lý lịch native</a></span></div>
                        <div class="gh-wb-meta-item"><strong>Khách liên quan</strong><span><a class="gh-wb-btn gh-wb-btn--soft" href="<%=CustomersUrl %>">Mở khách hàng /gianhang</a></span></div>
                    </div>
                </section>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
