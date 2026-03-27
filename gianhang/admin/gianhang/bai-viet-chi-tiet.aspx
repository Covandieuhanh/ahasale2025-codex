<%@ Page Title="Chi tiết bài viết /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="bai-viet-chi-tiet.aspx.cs" Inherits="gianhang_admin_gianhang_bai_viet_chi_tiet" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-article-detail-shell{display:grid;gap:18px}
        .gh-article-detail-hero,.gh-article-detail-card{border:1px solid #d9efe0;border-radius:22px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-article-detail-hero{padding:22px;background:radial-gradient(760px 240px at 0% 0%, rgba(34,197,94,.10), transparent 60%),linear-gradient(180deg,#f7fff8 0%,#fff 100%);display:flex;justify-content:space-between;align-items:flex-start;gap:18px;flex-wrap:wrap}
        .gh-article-detail-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#eefbf0;border:1px solid #cfead3;color:#166534;font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.03em}
        .gh-article-detail-title{margin:10px 0 6px;color:#16331d;font-size:30px;line-height:1.12;font-weight:900}
        .gh-article-detail-sub{margin:0;color:#55705b;font-size:14px;line-height:1.6;max-width:780px}
        .gh-article-detail-actions{display:flex;gap:10px;flex-wrap:wrap}
        .gh-article-detail-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none!important;font-size:13px;font-weight:800;border:1px solid transparent}
        .gh-article-detail-btn--primary{background:linear-gradient(135deg,#2f7a35,#52a35b);color:#fff!important;box-shadow:0 14px 28px rgba(47,122,53,.16)}
        .gh-article-detail-btn--soft{background:#fff;color:#16331d!important;border-color:#d9e6dc}
        .gh-article-detail-summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px}
        .gh-article-detail-summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}
        .gh-article-detail-summary-card small{display:block;color:#55705b;font-size:12px;text-transform:uppercase;font-weight:700}
        .gh-article-detail-summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}
        .gh-article-detail-summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-article-detail-grid{display:grid;grid-template-columns:minmax(280px,340px) minmax(0,1fr);gap:16px}
        .gh-article-detail-thumb{border-radius:24px;overflow:hidden;border:1px solid #d9efe0;background:#f5fff6;aspect-ratio:1/1}
        .gh-article-detail-thumb img{width:100%;height:100%;object-fit:cover;display:block}
        .gh-article-detail-meta{display:grid;gap:10px}
        .gh-article-detail-meta strong{display:block;color:#111827;font-size:14px}
        .gh-article-detail-meta span{display:block;margin-top:4px;color:#6b7280;font-size:13px;line-height:1.6}
        .gh-article-detail-badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}
        .gh-article-detail-badge--visible{background:#ecfdf3;color:#15803d}
        .gh-article-detail-badge--hidden{background:#f3f4f6;color:#6b7280}
        .gh-article-detail-content{padding:18px 20px;border-radius:18px;background:#f7fff8;border:1px solid #d9efe0;color:#1f2937;line-height:1.75}
        .gh-article-detail-empty{padding:28px 18px;text-align:center;color:#6b7280}
        @media (max-width:991px){.gh-article-detail-title{font-size:26px}.gh-article-detail-grid{grid-template-columns:1fr}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-article-detail-shell">
        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
            <section class="gh-article-detail-card">
                <div class="gh-article-detail-empty">
                    Chưa xác định đúng bài viết workspace để xem chi tiết. Mình quay về danh sách bài viết /gianhang rồi mở lại từ đúng dòng nhé.
                    <div style="margin-top:14px;">
                        <a class="gh-article-detail-btn gh-article-detail-btn--primary" href="<%=ArticlesUrl %>">Về bài viết /gianhang</a>
                    </div>
                </div>
            </section>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="ph_content" runat="server" Visible="false">
            <section class="gh-article-detail-hero">
                <div>
                    <div class="gh-article-detail-kicker">Bài viết native /gianhang</div>
                    <h1 class="gh-article-detail-title"><%=ArticleTitle %></h1>
                    <p class="gh-article-detail-sub">Mình gom luôn lớp chi tiết bài viết public vào <code>/gianhang/admin</code> để kiểm tra content, menu, bridge admin và storefront preview ngay trong cùng workspace.</p>
                </div>
                <div class="gh-article-detail-actions">
                    <a class="gh-article-detail-btn gh-article-detail-btn--primary" href="<%=AdminEditUrl %>">Mở admin</a>
                    <a class="gh-article-detail-btn gh-article-detail-btn--soft" href="<%=ArticlesUrl %>">Danh sách bài viết</a>
                    <a class="gh-article-detail-btn gh-article-detail-btn--soft" href="<%=PublicDetailUrl %>" target="_blank">Xem public</a>
                    <a class="gh-article-detail-btn gh-article-detail-btn--soft" href="<%=PublicListUrl %>" target="_blank">Mở danh sách public</a>
                    <a class="gh-article-detail-btn gh-article-detail-btn--soft" href="<%=StorefrontUrl %>">Trang công khai</a>
                </div>
            </section>

            <div class="gh-article-detail-summary">
                <div class="gh-article-detail-summary-card"><small>Hiển thị</small><strong><span class="<%=VisibilityCss %>"><%=VisibilityText %></span></strong><span>Trạng thái bài viết đang ra ngoài storefront.</span></div>
                <div class="gh-article-detail-summary-card"><small>Mirror admin</small><strong><%=MirrorText %></strong><span>Cho biết bài đã có bản edit legacy trong admin hay chưa.</span></div>
                <div class="gh-article-detail-summary-card"><small>Danh mục</small><strong><%=MenuTitle %></strong><span>Menu/list mà bài viết này đang gắn vào.</span></div>
                <div class="gh-article-detail-summary-card"><small>Cập nhật</small><strong><%=UpdatedAtText %></strong><span>Thời điểm đồng bộ gần nhất của record bài viết.</span></div>
            </div>

            <div class="gh-article-detail-grid">
                <section class="gh-article-detail-card" style="padding:18px;">
                    <div class="gh-article-detail-thumb">
                        <img src="<%=ImageUrl %>" alt="<%=ArticleTitle %>" />
                    </div>
                </section>

                <section class="gh-article-detail-card" style="padding:18px;">
                    <div class="gh-article-detail-meta">
                        <div><strong>Mô tả ngắn</strong><span><%=DescriptionText %></span></div>
                        <div><strong>Menu public</strong><span><%=MenuTitle %></span></div>
                        <div><strong>Điều hướng</strong><span>Mở admin để chỉnh content, mở public để rà SEO-facing content, hoặc quay về hub storefront để kiểm tra toàn bộ bố cục hiển thị.</span></div>
                    </div>
                </section>
            </div>

            <section class="gh-article-detail-card" style="padding:18px;">
                <h3 style="margin:0 0 14px;color:#16331d;">Nội dung bài viết</h3>
                <div class="gh-article-detail-content"><%=ContentHtml %></div>
            </section>
        </asp:PlaceHolder>
    </div>
</asp:Content>
