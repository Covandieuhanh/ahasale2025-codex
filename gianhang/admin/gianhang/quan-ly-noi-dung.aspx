<%@ Page Title="Nội dung /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="quan-ly-noi-dung.aspx.cs" Inherits="gianhang_admin_gianhang_quan_ly_noi_dung" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-content{display:grid;gap:18px}.gh-admin-content__hero,.gh-admin-content__card{border:1px solid #ecdcdc;border-radius:20px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-admin-content__hero{padding:20px 22px;background:radial-gradient(720px 220px at 0% 0%, rgba(82,163,91,.12), transparent 60%),linear-gradient(180deg,#f7fff8 0%,#fff 100%);display:flex;justify-content:space-between;gap:16px;align-items:flex-start;flex-wrap:wrap}
        .gh-admin-content__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#eefbf0;border:1px solid #cfead3;color:#166534;font-size:12px;font-weight:800;letter-spacing:.03em;text-transform:uppercase}
        .gh-admin-content__title{margin:10px 0 6px;color:#16331d;font-size:30px;line-height:1.1;font-weight:900}.gh-admin-content__sub{margin:0;color:#55705b;font-size:14px;line-height:1.6;max-width:760px}
        .gh-admin-content__actions{display:flex;gap:10px;flex-wrap:wrap}.gh-admin-content__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;border:1px solid transparent;text-decoration:none!important;font-size:13px;font-weight:800}
        .gh-admin-content__btn--primary{color:#fff!important;background:linear-gradient(135deg,#2f7a35,#52a35b);box-shadow:0 14px 28px rgba(47,122,53,.16)}.gh-admin-content__btn--soft{color:#16331d!important;background:#fff;border-color:#d9e6dc}
        .gh-admin-content__filters{padding:16px;display:grid;grid-template-columns:minmax(0,1fr) 190px auto auto;gap:12px;align-items:end}.gh-admin-content__field{display:grid;gap:6px}.gh-admin-content__field label{color:#55705b;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.03em}
        .gh-admin-content__input{min-height:42px;border-radius:14px;border:1px solid #d9dee7;background:#fff;padding:0 14px;color:#1f2937;font-size:14px;outline:none}.gh-admin-content__input:focus{border-color:#52a35b;box-shadow:0 0 0 3px rgba(82,163,91,.12)}
        .gh-admin-content__submit,.gh-admin-content__reset{min-height:42px;border-radius:14px;padding:0 16px;font-size:13px;font-weight:800}.gh-admin-content__submit{border:0;background:linear-gradient(135deg,#2f7a35,#52a35b);color:#fff}.gh-admin-content__reset{border:1px solid #e2e8f0;background:#fff;color:#16331d}
        .gh-admin-content__summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:12px}.gh-admin-content__summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}
        .gh-admin-content__summary-card small{display:block;color:#55705b;font-size:12px;text-transform:uppercase;font-weight:700}.gh-admin-content__summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}.gh-admin-content__summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-admin-content__table-wrap{overflow:auto}.gh-admin-content__table{width:100%;border-collapse:collapse}.gh-admin-content__table th,.gh-admin-content__table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top}.gh-admin-content__table th{color:#55705b;font-size:12px;text-transform:uppercase;letter-spacing:.03em}
        .gh-admin-content__media{display:flex;align-items:flex-start;gap:12px}.gh-admin-content__thumb{width:64px;height:64px;border-radius:16px;overflow:hidden;border:1px solid #e5e7eb;background:#f8fafc;flex:0 0 auto}.gh-admin-content__thumb img{width:100%;height:100%;object-fit:cover;display:block}
        .gh-admin-content__meta strong{display:block;color:#111827;font-size:15px;line-height:1.4}.gh-admin-content__meta span{display:block;margin-top:4px;color:#6b7280;font-size:13px}
        .gh-admin-content__badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}.gh-admin-content__badge--product{background:#e8f3ff;color:#1d4ed8}.gh-admin-content__badge--service{background:#eefcf4;color:#15803d}.gh-admin-content__badge--visible{background:#ecfdf3;color:#15803d}.gh-admin-content__badge--hidden{background:#f3f4f6;color:#6b7280}
        .gh-admin-content__actions-list{display:flex;gap:8px;flex-wrap:wrap}.gh-admin-content__action-link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #dbe2ea;background:#fff;color:#16331d!important;font-size:12px;font-weight:800;text-decoration:none!important}.gh-admin-content__action-link--primary{border-color:#cfead3;background:#f5fff6}
        .gh-admin-content__empty{padding:22px 12px;color:#6b7280;text-align:center}
        @media (max-width:991px){.gh-admin-content__filters{grid-template-columns:1fr}.gh-admin-content__title{font-size:26px}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-content">
        <section class="gh-admin-content__hero">
            <div>
                <div class="gh-admin-content__kicker">Không gian /gianhang</div>
                <h1 class="gh-admin-content__title">Nội dung /gianhang</h1>
                <p class="gh-admin-content__sub">Từ đây mình nhìn được toàn bộ sản phẩm và dịch vụ native của gian hàng, đồng thời biết ngay bản ghi nào đã bridge sang lớp quản lý bài viết trong <code>/gianhang/admin</code>.</p>
            </div>
            <div class="gh-admin-content__actions">
                <a class="gh-admin-content__btn gh-admin-content__btn--primary" href="<%=AdminContentUrl %>">Mở quản lý admin</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=ProductHubUrl %>">Sản phẩm /gianhang</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=ServiceHubUrl %>">Dịch vụ /gianhang</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=ArticleHubUrl %>">Bài viết public</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=NativeManageUrl %>">Mở lớp native</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=NativeCreateUrl %>">Tạo nội dung mới</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=PublicProductsUrl %>" target="_blank">Xem sản phẩm công khai</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=PublicServicesUrl %>" target="_blank">Xem dịch vụ công khai</a>
                <a class="gh-admin-content__btn gh-admin-content__btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <section class="gh-admin-content__card">
            <div class="gh-admin-content__filters">
                <div class="gh-admin-content__field">
                    <label for="<%= txt_search.ClientID %>">Tìm nội dung</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-admin-content__input" MaxLength="120" placeholder="Tên, mô tả hoặc ID native"></asp:TextBox>
                </div>
                <div class="gh-admin-content__field">
                    <label for="<%= ddl_type.ClientID %>">Loại hiển thị</label>
                    <asp:DropDownList ID="ddl_type" runat="server" CssClass="gh-admin-content__input">
                        <asp:ListItem Value="all" Text="Tất cả"></asp:ListItem>
                        <asp:ListItem Value="sanpham" Text="Sản phẩm"></asp:ListItem>
                        <asp:ListItem Value="dichvu" Text="Dịch vụ"></asp:ListItem>
                        <asp:ListItem Value="active" Text="Đang hiển thị"></asp:ListItem>
                        <asp:ListItem Value="hidden" Text="Đang ẩn"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc nội dung" CssClass="gh-admin-content__submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-admin-content__reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-admin-content__summary">
            <div class="gh-admin-content__summary-card"><small>Tổng nội dung</small><strong><%=TotalItems.ToString("#,##0") %></strong><span>Số bản ghi native đang có trong /gianhang.</span></div>
            <div class="gh-admin-content__summary-card"><small>Sản phẩm</small><strong><%=ProductCount.ToString("#,##0") %></strong><span>Nhóm bán hàng vật lý đang hiển thị cho storefront.</span></div>
            <div class="gh-admin-content__summary-card"><small>Dịch vụ</small><strong><%=ServiceCount.ToString("#,##0") %></strong><span>Nhóm dịch vụ, booking và chăm sóc khách.</span></div>
            <div class="gh-admin-content__summary-card"><small>Đang ẩn</small><strong><%=HiddenCount.ToString("#,##0") %></strong><span>Nội dung đã tắt hiển thị ở mặt public.</span></div>
            <div class="gh-admin-content__summary-card"><small>Đã mirror</small><strong><%=MirroredCount.ToString("#,##0") %></strong><span>Bản ghi đã hiện trong công cụ nội dung admin.</span></div>
        </div>

        <section class="gh-admin-content__card">
            <div class="gh-admin-content__table-wrap">
                <table class="gh-admin-content__table">
                    <thead>
                        <tr>
                            <th style="min-width:280px;">Nội dung</th>
                            <th style="min-width:120px;">Loại</th>
                            <th style="min-width:120px;">Giá</th>
                            <th style="min-width:120px;">Hiển thị</th>
                            <th style="min-width:150px;">Bridge admin</th>
                            <th style="min-width:140px;">Cập nhật</th>
                            <th style="min-width:260px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_rows" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="gh-admin-content__media">
                                            <div class="gh-admin-content__thumb">
                                                <img src="<%# Eval("ImageUrl") %>" alt="<%# Eval("Name") %>" />
                                            </div>
                                            <div class="gh-admin-content__meta">
                                                <strong>#<%# Eval("NativeId") %> · <%# Eval("Name") %></strong>
                                                <span><%# Eval("LegacyMirrorText") %></span>
                                            </div>
                                        </div>
                                    </td>
                                    <td><span class='<%# Eval("TypeCss") %>'><%# Eval("TypeLabel") %></span></td>
                                    <td><strong><%# Eval("PriceText") %></strong></td>
                                    <td><span class='<%# Eval("VisibilityCss") %>'><%# Eval("VisibilityText") %></span></td>
                                    <td><%# Eval("LegacyMirrorText") %></td>
                                    <td><%# Eval("UpdatedAtText") %></td>
                                    <td>
                                        <div class="gh-admin-content__actions-list">
                                            <a class="gh-admin-content__action-link gh-admin-content__action-link--primary" href="<%# Eval("LegacyEditUrl") %>">Mở admin</a>
                                            <a class="gh-admin-content__action-link" href="<%# Eval("NativeEditUrl") %>">Mở native</a>
                                            <a class="gh-admin-content__action-link" href="<%# Eval("PublicUrl") %>" target="_blank">Xem công khai</a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <tr><td colspan="7" class="gh-admin-content__empty">Chưa có sản phẩm hoặc dịch vụ native nào khớp với bộ lọc hiện tại.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
