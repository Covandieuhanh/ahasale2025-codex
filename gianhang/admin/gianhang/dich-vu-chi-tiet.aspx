<%@ Page Title="Chi tiết dịch vụ /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="dich-vu-chi-tiet.aspx.cs" Inherits="gianhang_admin_gianhang_dich_vu_chi_tiet" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-item-shell{display:grid;gap:18px}
        .gh-item-hero,.gh-item-card{border:1px solid #d9efe0;border-radius:22px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-item-hero{padding:22px;background:radial-gradient(760px 240px at 0% 0%, rgba(34,197,94,.10), transparent 60%),linear-gradient(180deg,#f7fff8 0%,#fff 100%);display:flex;justify-content:space-between;align-items:flex-start;gap:18px;flex-wrap:wrap}
        .gh-item-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#eefbf0;border:1px solid #cfead3;color:#166534;font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.03em}
        .gh-item-title{margin:10px 0 6px;color:#16331d;font-size:30px;line-height:1.12;font-weight:900}
        .gh-item-sub{margin:0;color:#64748b;font-size:14px;line-height:1.6;max-width:780px}
        .gh-item-actions{display:flex;gap:10px;flex-wrap:wrap}
        .gh-item-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none!important;font-size:13px;font-weight:800;border:1px solid transparent}
        .gh-item-btn--primary{background:linear-gradient(135deg,#2f7a35,#52a35b);color:#fff!important;box-shadow:0 14px 28px rgba(47,122,53,.16)}
        .gh-item-btn--soft{background:#fff;color:#16331d!important;border-color:#d9efe0}
        .gh-item-summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px}
        .gh-item-summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}
        .gh-item-summary-card small{display:block;color:#64748b;font-size:12px;text-transform:uppercase;font-weight:700}
        .gh-item-summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}
        .gh-item-summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-item-grid{display:grid;grid-template-columns:minmax(280px,340px) minmax(0,1fr);gap:16px}
        .gh-item-thumb{border-radius:24px;overflow:hidden;border:1px solid #d9efe0;background:#f5fff6;aspect-ratio:1/1}
        .gh-item-thumb img{width:100%;height:100%;object-fit:cover;display:block}
        .gh-item-meta{display:grid;gap:10px}
        .gh-item-meta strong{display:block;color:#111827;font-size:14px}
        .gh-item-meta span{display:block;margin-top:4px;color:#6b7280;font-size:13px;line-height:1.6}
        .gh-item-badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}
        .gh-item-badge--visible{background:#ecfdf3;color:#15803d}
        .gh-item-badge--hidden{background:#f3f4f6;color:#6b7280}
        .gh-item-content{padding:18px 20px;border-radius:18px;background:#f7fff8;border:1px solid #d9efe0;color:#1f2937;line-height:1.75}
        .gh-item-empty{padding:28px 18px;text-align:center;color:#6b7280}
        @media (max-width:767px){.gh-item-title{font-size:26px}.gh-item-grid{grid-template-columns:1fr}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-item-shell">
        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
            <section class="gh-item-card">
                <div class="gh-item-empty">
                    Chưa xác định đúng dịch vụ workspace để xem chi tiết. Mình quay về danh sách dịch vụ /gianhang rồi mở lại từ đúng dòng nhé.
                    <div style="margin-top:14px;">
                        <a class="gh-item-btn gh-item-btn--primary" href="<%=ServicesUrl %>">Về dịch vụ /gianhang</a>
                    </div>
                </div>
            </section>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="ph_content" runat="server" Visible="false">
            <section class="gh-item-hero">
                <div>
                    <div class="gh-item-kicker">Dịch vụ native /gianhang</div>
                    <h1 class="gh-item-title"><%=ItemName %></h1>
                    <p class="gh-item-sub">Mình dùng trang này để nhìn chi tiết dịch vụ native ngay trong <code>/gianhang/admin</code>, kèm bridge admin, trạng thái public, booking entry và các liên kết vận hành liên quan.</p>
                </div>
                <div class="gh-item-actions">
                    <a class="gh-item-btn gh-item-btn--primary" href="<%=AdminEditUrl %>">Mở admin</a>
                    <a class="gh-item-btn gh-item-btn--soft" href="<%=ServicesUrl %>">Danh sách dịch vụ</a>
                    <a class="gh-item-btn gh-item-btn--soft" href="<%=NativeEditUrl %>">Mở native</a>
                    <a class="gh-item-btn gh-item-btn--soft" href="<%=PublicDetailUrl %>" target="_blank">Xem public</a>
                    <a class="gh-item-btn gh-item-btn--soft" href="<%=BookingUrl %>">Đặt lịch native</a>
                </div>
            </section>

            <div class="gh-item-summary">
                <div class="gh-item-summary-card"><small>Giá dịch vụ</small><strong><%=PriceText %></strong><span>Giá hiện tại dùng cho booking và storefront.</span></div>
                <div class="gh-item-summary-card"><small>Danh mục</small><strong><%=CategoryText %></strong><span>Dùng để map menu và các list public.</span></div>
                <div class="gh-item-summary-card"><small>Hiển thị</small><strong><span class="<%=VisibilityCss %>"><%=VisibilityText %></span></strong><span>Trạng thái mở cho khách xem và đặt lịch.</span></div>
                <div class="gh-item-summary-card"><small>Bridge admin</small><strong><%=BridgeText %></strong><span>Cho biết dịch vụ này đã có bản ghi legacy trong admin hay chưa.</span></div>
                <div class="gh-item-summary-card"><small>Ưu đãi</small><strong><%=DiscountText %></strong><span><%=StockText %></span></div>
                <div class="gh-item-summary-card"><small>Cập nhật</small><strong><%=UpdatedAtText %></strong><span>Thời điểm sync gần nhất từ lớp native.</span></div>
            </div>

            <div class="gh-item-grid">
                <section class="gh-item-card" style="padding:18px;">
                    <div class="gh-item-thumb">
                        <img src="<%=ImageUrl %>" alt="<%=ItemName %>" />
                    </div>
                </section>

                <section class="gh-item-card" style="padding:18px;">
                    <div class="gh-item-meta">
                        <div><strong>Mô tả ngắn</strong><span><%=DescriptionText %></span></div>
                        <div><strong>Điều hướng</strong><span>Mở admin để vận hành sâu hơn, mở native để chỉnh nhanh, hoặc mở flow đặt lịch để kiểm thử trải nghiệm khách hàng.</span></div>
                        <div><strong>Workspace</strong><span><a class="gh-item-btn gh-item-btn--soft" href="<%=HubUrl %>">Về hub /gianhang</a> <a class="gh-item-btn gh-item-btn--soft" href="<%=BookingsUrl %>">Lịch hẹn</a> <a class="gh-item-btn gh-item-btn--soft" href="<%=ContentUrl %>">Nội dung</a></span></div>
                    </div>
                </section>
            </div>

            <section class="gh-item-card" style="padding:18px;">
                <h3 style="margin:0 0 14px;color:#16331d;">Nội dung dịch vụ</h3>
                <div class="gh-item-content"><%=ContentHtml %></div>
            </section>
        </asp:PlaceHolder>
    </div>
</asp:Content>
