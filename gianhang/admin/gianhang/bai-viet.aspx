<%@ Page Title="Bài viết /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="bai-viet.aspx.cs" Inherits="gianhang_admin_gianhang_bai_viet" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-article-shell{display:grid;gap:18px}
        .gh-article-hero,.gh-article-card{border:1px solid #d9efe0;border-radius:22px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-article-hero{padding:22px;background:radial-gradient(760px 240px at 0% 0%, rgba(34,197,94,.10), transparent 60%),linear-gradient(180deg,#f7fff8 0%,#fff 100%);display:flex;justify-content:space-between;align-items:flex-start;gap:18px;flex-wrap:wrap}
        .gh-article-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#eefbf0;border:1px solid #cfead3;color:#166534;font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.03em}
        .gh-article-title{margin:10px 0 6px;color:#16331d;font-size:30px;line-height:1.12;font-weight:900}
        .gh-article-sub{margin:0;color:#55705b;font-size:14px;line-height:1.6;max-width:760px}
        .gh-article-actions{display:flex;gap:10px;flex-wrap:wrap}
        .gh-article-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none!important;font-size:13px;font-weight:800;border:1px solid transparent}
        .gh-article-btn--primary{background:linear-gradient(135deg,#2f7a35,#52a35b);color:#fff!important;box-shadow:0 14px 28px rgba(47,122,53,.16)}
        .gh-article-btn--soft{background:#fff;color:#16331d!important;border-color:#d9e6dc}
        .gh-article-filters{padding:16px;display:grid;grid-template-columns:minmax(0,1fr) auto auto;gap:12px;align-items:end}
        .gh-article-input{min-height:42px;border-radius:14px;border:1px solid #d9dee7;background:#fff;padding:0 14px;color:#1f2937;font-size:14px;outline:none}
        .gh-article-input:focus{border-color:#52a35b;box-shadow:0 0 0 3px rgba(82,163,91,.12)}
        .gh-article-submit,.gh-article-reset{min-height:42px;border-radius:14px;padding:0 16px;font-size:13px;font-weight:800}
        .gh-article-submit{border:0;background:linear-gradient(135deg,#2f7a35,#52a35b);color:#fff}
        .gh-article-reset{border:1px solid #e2e8f0;background:#fff;color:#16331d}
        .gh-article-summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:12px}
        .gh-article-summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}
        .gh-article-summary-card small{display:block;color:#55705b;font-size:12px;text-transform:uppercase;font-weight:700}
        .gh-article-summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}
        .gh-article-summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-article-table-wrap{overflow:auto}
        .gh-article-table{width:100%;border-collapse:collapse}
        .gh-article-table th,.gh-article-table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top}
        .gh-article-table th{color:#55705b;font-size:12px;text-transform:uppercase;letter-spacing:.03em}
        .gh-article-media{display:flex;align-items:flex-start;gap:12px}
        .gh-article-thumb{width:64px;height:64px;border-radius:16px;overflow:hidden;border:1px solid #e5e7eb;background:#f8fafc;flex:0 0 auto}
        .gh-article-thumb img{width:100%;height:100%;object-fit:cover;display:block}
        .gh-article-meta strong{display:block;color:#111827;font-size:15px;line-height:1.4}
        .gh-article-meta span{display:block;margin-top:4px;color:#6b7280;font-size:13px}
        .gh-article-badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}
        .gh-article-badge--visible{background:#ecfdf3;color:#15803d}
        .gh-article-badge--hidden{background:#f3f4f6;color:#6b7280}
        .gh-article-actions-list{display:flex;gap:8px;flex-wrap:wrap}
        .gh-article-link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #dbe2ea;background:#fff;color:#16331d!important;font-size:12px;font-weight:800;text-decoration:none!important}
        .gh-article-link--primary{border-color:#cfead3;background:#f5fff6}
        .gh-article-empty{padding:22px 12px;color:#6b7280;text-align:center}
        @media (max-width:991px){.gh-article-title{font-size:26px}.gh-article-filters{grid-template-columns:1fr}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-article-shell">
        <section class="gh-article-hero">
            <div>
                <div class="gh-article-kicker">Không gian /gianhang</div>
                <h1 class="gh-article-title">Bài viết /gianhang</h1>
                <p class="gh-article-sub">
                    Khu này gom lớp bài viết public của storefront vào đúng workspace level 2. Mình nhìn được bài nào đang hiển thị, bài nào đã mirror từ legacy và mở thẳng sang public hoặc admin edit.
                </p>
            </div>
            <div class="gh-article-actions">
                <a class="gh-article-btn gh-article-btn--primary" href="<%=AdminArticleUrl %>">Mở quản lý bài viết admin</a>
                <a class="gh-article-btn gh-article-btn--soft" href="<%=PublicArticlesUrl %>" target="_blank">Danh sách bài viết public</a>
                <a class="gh-article-btn gh-article-btn--soft" href="<%=StorefrontPreviewUrl %>">Preview storefront</a>
            </div>
        </section>

        <section class="gh-article-card">
            <div class="gh-article-filters">
                <asp:TextBox ID="txt_search" runat="server" CssClass="gh-article-input" MaxLength="120" placeholder="Tên bài viết, mô tả hoặc ID"></asp:TextBox>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc bài viết" CssClass="gh-article-submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-article-reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-article-summary">
            <div class="gh-article-summary-card"><small>Tổng bài viết</small><strong><%=TotalArticles.ToString("#,##0") %></strong><span>Tổng bài public đang có của storefront.</span></div>
            <div class="gh-article-summary-card"><small>Đang hiển thị</small><strong><%=VisibleArticles.ToString("#,##0") %></strong><span>Bài đang hiển thị trên storefront.</span></div>
            <div class="gh-article-summary-card"><small>Đang ẩn</small><strong><%=HiddenArticles.ToString("#,##0") %></strong><span>Bài đã tắt hoặc chuyển ẩn khỏi public.</span></div>
            <div class="gh-article-summary-card"><small>Đã bridge</small><strong><%=MirroredArticles.ToString("#,##0") %></strong><span>Bài đã có legacy mirror để mở trong admin.</span></div>
        </div>

        <section class="gh-article-card">
            <div class="gh-article-table-wrap">
                <table class="gh-article-table">
                    <thead>
                        <tr>
                            <th style="min-width:300px;">Bài viết</th>
                            <th style="min-width:130px;">Hiển thị</th>
                            <th style="min-width:150px;">Mirror admin</th>
                            <th style="min-width:150px;">Cập nhật</th>
                            <th style="min-width:260px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_rows" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="gh-article-media">
                                            <div class="gh-article-thumb">
                                                <img src="<%# Eval("ImageUrl") %>" alt="<%# Eval("Name") %>" />
                                            </div>
                                            <div class="gh-article-meta">
                                                <strong>#<%# Eval("ArticleId") %> · <%# Eval("Name") %></strong>
                                                <span><%# Eval("Description") %></span>
                                            </div>
                                        </div>
                                    </td>
                                    <td><span class='<%# Eval("VisibilityCss") %>'><%# Eval("VisibilityText") %></span></td>
                                    <td><%# Eval("MirrorText") %></td>
                                    <td><%# Eval("UpdatedAtText") %></td>
                                    <td>
                                        <div class="gh-article-actions-list">
                                            <a class="gh-article-link" href="<%# Eval("WorkspaceDetailUrl") %>">Chi tiết workspace</a>
                                            <a class="gh-article-link gh-article-link--primary" href="<%# Eval("AdminEditUrl") %>">Mở admin</a>
                                            <a class="gh-article-link" href="<%# Eval("PublicDetailUrl") %>" target="_blank">Xem public</a>
                                            <a class="gh-article-link" href="<%# Eval("PublicListUrl") %>" target="_blank">Về danh sách</a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <tr><td colspan="5" class="gh-article-empty">Chưa có bài viết public nào khớp với bộ lọc hiện tại.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
