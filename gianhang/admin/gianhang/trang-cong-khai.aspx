<%@ Page Title="Trang công khai /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="trang-cong-khai.aspx.cs" Inherits="gianhang_admin_gianhang_trang_cong_khai" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-storefront{display:grid;gap:18px}
        .gh-admin-storefront__hero,.gh-admin-storefront__card{border:1px solid #ecdcdc;border-radius:20px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-admin-storefront__hero{padding:20px 22px;background:radial-gradient(720px 220px at 0% 0%, rgba(59,130,246,.12), transparent 60%),linear-gradient(180deg,#f8fbff 0%,#fff 100%);display:flex;justify-content:space-between;gap:16px;align-items:flex-start;flex-wrap:wrap}
        .gh-admin-storefront__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#eff6ff;border:1px solid #dbeafe;color:#1d4ed8;font-size:12px;font-weight:800;letter-spacing:.03em;text-transform:uppercase}
        .gh-admin-storefront__title{margin:10px 0 6px;color:#172554;font-size:30px;line-height:1.1;font-weight:900}
        .gh-admin-storefront__sub{margin:0;color:#4b6483;font-size:14px;line-height:1.65;max-width:760px}
        .gh-admin-storefront__actions{display:flex;gap:10px;flex-wrap:wrap}
        .gh-admin-storefront__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;border:1px solid transparent;text-decoration:none!important;font-size:13px;font-weight:800}
        .gh-admin-storefront__btn--primary{color:#fff!important;background:linear-gradient(135deg,#2563eb,#3b82f6);box-shadow:0 14px 28px rgba(37,99,235,.18)}
        .gh-admin-storefront__btn--soft{color:#1e3a8a!important;background:#fff;border-color:#dbe4f0}
        .gh-admin-storefront__grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px}
        .gh-admin-storefront__kpi{border:1px solid #e8eef6;border-radius:18px;background:#fff;padding:16px}
        .gh-admin-storefront__kpi small{display:block;color:#5f7190;font-size:12px;font-weight:700;text-transform:uppercase}
        .gh-admin-storefront__kpi strong{display:block;margin-top:8px;color:#172554;font-size:26px;font-weight:900}
        .gh-admin-storefront__kpi span{display:block;margin-top:6px;color:#6b7280;font-size:13px;line-height:1.6}
        .gh-admin-storefront__card{padding:18px}
        .gh-admin-storefront__links{display:grid;grid-template-columns:repeat(auto-fit,minmax(260px,1fr));gap:12px}
        .gh-admin-storefront__link{display:flex;align-items:flex-start;justify-content:space-between;gap:12px;padding:14px 16px;border-radius:16px;border:1px solid #e6edf5;background:#f8fbff;color:#0f172a;text-decoration:none!important}
        .gh-admin-storefront__link small{display:block;color:#5f7190;font-size:11px;font-weight:700;text-transform:uppercase}
        .gh-admin-storefront__link strong{display:block;margin-top:2px;font-size:15px;color:#172554}
        .gh-admin-storefront__link span{display:block;margin-top:6px;color:#6b7280;font-size:13px;line-height:1.6}
        .gh-admin-storefront__frame-wrap{border:1px solid #e5e7eb;border-radius:20px;overflow:hidden;background:#fff}
        .gh-admin-storefront__frame-head{display:flex;align-items:center;justify-content:space-between;gap:12px;padding:14px 16px;border-bottom:1px solid #edf2f7;background:#f8fafc;flex-wrap:wrap}
        .gh-admin-storefront__frame-url{display:block;color:#6b7280;font-size:12px;line-height:1.6;word-break:break-all}
        .gh-admin-storefront__frame{width:100%;height:920px;border:0;background:#fff}
        @media (max-width:991px){.gh-admin-storefront__title{font-size:26px}.gh-admin-storefront__frame{height:720px}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-storefront">
        <section class="gh-admin-storefront__hero">
            <div>
                <div class="gh-admin-storefront__kicker">Storefront preview</div>
                <h1 class="gh-admin-storefront__title">Trang công khai /gianhang</h1>
                <p class="gh-admin-storefront__sub">
                    Đây là lớp xem nhanh storefront công khai của cùng workspace ngay bên trong <code>/gianhang/admin</code>.
                    Mình dùng nó để rà bố cục, CTA, nội dung sản phẩm, dịch vụ và hành trình khách mà không cần thoát khỏi lớp vận hành.
                </p>
            </div>
            <div class="gh-admin-storefront__actions">
                <a class="gh-admin-storefront__btn gh-admin-storefront__btn--primary" href="<%=PublicUrl %>" target="_blank">Mở trang công khai</a>
                <a class="gh-admin-storefront__btn gh-admin-storefront__btn--soft" href="<%=ContentUrl %>">Nội dung /gianhang</a>
                <a class="gh-admin-storefront__btn gh-admin-storefront__btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                <a class="gh-admin-storefront__btn gh-admin-storefront__btn--soft" href="<%=BookingsUrl %>">Lịch hẹn /gianhang</a>
                <a class="gh-admin-storefront__btn gh-admin-storefront__btn--soft" href="<%=HubUrl %>">Hub /gianhang</a>
            </div>
        </section>

        <div class="gh-admin-storefront__grid">
            <div class="gh-admin-storefront__kpi">
                <small>Workspace</small>
                <strong><%=WorkspaceDisplayName %></strong>
                <span>Đây là storefront public của đúng owner đã mở quyền level 2.</span>
            </div>
            <div class="gh-admin-storefront__kpi">
                <small>Sản phẩm công khai</small>
                <strong><%=ProductCount.ToString("#,##0") %></strong>
                <span>Số sản phẩm đang hiển thị ở storefront public.</span>
            </div>
            <div class="gh-admin-storefront__kpi">
                <small>Dịch vụ công khai</small>
                <strong><%=ServiceCount.ToString("#,##0") %></strong>
                <span>Số dịch vụ khách có thể xem và đi tới booking.</span>
            </div>
            <div class="gh-admin-storefront__kpi">
                <small>Bài viết public</small>
                <strong><%=ArticleCount.ToString("#,##0") %></strong>
                <span>Lớp nội dung public để kéo traffic và chuyển đổi.</span>
            </div>
        </div>

        <section class="gh-admin-storefront__card">
            <div class="gh-admin-storefront__links">
                <a class="gh-admin-storefront__link" href="<%=ProductsUrl %>" target="_blank">
                    <div>
                        <small>Public catalog</small>
                        <strong>Danh sách sản phẩm</strong>
                        <span>Mở trực tiếp catalog sản phẩm công khai của storefront.</span>
                    </div>
                    <strong>&rsaquo;</strong>
                </a>
                <a class="gh-admin-storefront__link" href="<%=ServicesUrl %>" target="_blank">
                    <div>
                        <small>Booking catalog</small>
                        <strong>Danh sách dịch vụ</strong>
                        <span>Mở danh sách dịch vụ mà khách có thể xem và đặt lịch.</span>
                    </div>
                    <strong>&rsaquo;</strong>
                </a>
                <a class="gh-admin-storefront__link" href="<%=ArticlesUrl %>" target="_blank">
                    <div>
                        <small>Content layer</small>
                        <strong>Danh sách bài viết</strong>
                        <span>Mở lớp bài viết public để kiểm tra menu, CTA và SEO-facing content.</span>
                    </div>
                    <strong>&rsaquo;</strong>
                </a>
                <a class="gh-admin-storefront__link" href="<%=ContentUrl %>">
                    <div>
                        <small>Admin content</small>
                        <strong>Quản lý nội dung /gianhang</strong>
                        <span>Quay lại lớp quản trị nội dung native và bridge admin của cùng workspace.</span>
                    </div>
                    <strong>&rsaquo;</strong>
                </a>
            </div>
        </section>

        <section class="gh-admin-storefront__card">
            <div class="gh-admin-storefront__frame-wrap">
                <div class="gh-admin-storefront__frame-head">
                    <div>
                        <strong style="display:block;color:#172554;">Preview storefront</strong>
                        <span class="gh-admin-storefront__frame-url"><%=PublicUrl %></span>
                    </div>
                    <a class="gh-admin-storefront__btn gh-admin-storefront__btn--soft" href="<%=PublicUrl %>" target="_blank">Mở tab mới</a>
                </div>
                <iframe class="gh-admin-storefront__frame" src="<%=PublicUrl %>" title="Storefront /gianhang preview"></iframe>
            </div>
        </section>
    </div>
</asp:Content>
