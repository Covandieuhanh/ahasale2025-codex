<%@ Page Title="Sản phẩm /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="san-pham.aspx.cs" Inherits="gianhang_admin_gianhang_san_pham" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-catalog-shell{display:grid;gap:18px}
        .gh-catalog-hero,.gh-catalog-card{border:1px solid #d8e7ff;border-radius:22px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06)}
        .gh-catalog-hero{padding:22px;background:radial-gradient(760px 240px at 0% 0%, rgba(59,130,246,.12), transparent 60%),linear-gradient(180deg,#f8fbff 0%,#fff 100%);display:flex;justify-content:space-between;align-items:flex-start;gap:18px;flex-wrap:wrap}
        .gh-catalog-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#eef5ff;border:1px solid #cfe0ff;color:#1d4ed8;font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.03em}
        .gh-catalog-title{margin:10px 0 6px;color:#1e3a8a;font-size:30px;line-height:1.12;font-weight:900}
        .gh-catalog-sub{margin:0;color:#64748b;font-size:14px;line-height:1.6;max-width:760px}
        .gh-catalog-actions{display:flex;gap:10px;flex-wrap:wrap}
        .gh-catalog-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none!important;font-size:13px;font-weight:800;border:1px solid transparent}
        .gh-catalog-btn--primary{background:linear-gradient(135deg,#2563eb,#3b82f6);color:#fff!important;box-shadow:0 14px 28px rgba(37,99,235,.18)}
        .gh-catalog-btn--soft{background:#fff;color:#1e3a8a!important;border-color:#d8e7ff}
        .gh-catalog-filters{padding:16px;display:grid;grid-template-columns:minmax(0,1fr) 190px auto auto;gap:12px;align-items:end}
        .gh-catalog-field{display:grid;gap:6px}
        .gh-catalog-field label{color:#64748b;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.03em}
        .gh-catalog-input{min-height:42px;border-radius:14px;border:1px solid #d9dee7;background:#fff;padding:0 14px;color:#1f2937;font-size:14px;outline:none}
        .gh-catalog-input:focus{border-color:#3b82f6;box-shadow:0 0 0 3px rgba(59,130,246,.12)}
        .gh-catalog-submit,.gh-catalog-reset{min-height:42px;border-radius:14px;padding:0 16px;font-size:13px;font-weight:800}
        .gh-catalog-submit{border:0;background:linear-gradient(135deg,#2563eb,#3b82f6);color:#fff}
        .gh-catalog-reset{border:1px solid #e2e8f0;background:#fff;color:#16331d}
        .gh-catalog-summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:12px}
        .gh-catalog-summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px}
        .gh-catalog-summary-card small{display:block;color:#64748b;font-size:12px;text-transform:uppercase;font-weight:700}
        .gh-catalog-summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900}
        .gh-catalog-summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px}
        .gh-catalog-table-wrap{overflow:auto}
        .gh-catalog-table{width:100%;border-collapse:collapse}
        .gh-catalog-table th,.gh-catalog-table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top}
        .gh-catalog-table th{color:#64748b;font-size:12px;text-transform:uppercase;letter-spacing:.03em}
        .gh-catalog-media{display:flex;align-items:flex-start;gap:12px}
        .gh-catalog-thumb{width:72px;height:72px;border-radius:18px;overflow:hidden;border:1px solid #e5e7eb;background:#f8fafc;flex:0 0 auto}
        .gh-catalog-thumb img{width:100%;height:100%;object-fit:cover;display:block}
        .gh-catalog-meta strong{display:block;color:#111827;font-size:15px;line-height:1.4}
        .gh-catalog-meta span{display:block;margin-top:4px;color:#6b7280;font-size:13px}
        .gh-catalog-badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap}
        .gh-catalog-badge--visible{background:#ecfdf3;color:#15803d}
        .gh-catalog-badge--hidden{background:#f3f4f6;color:#6b7280}
        .gh-catalog-actions-list{display:flex;gap:8px;flex-wrap:wrap}
        .gh-catalog-link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #dbe2ea;background:#fff;color:#16331d!important;font-size:12px;font-weight:800;text-decoration:none!important}
        .gh-catalog-link--primary{border-color:#cfe0ff;background:#eef5ff}
        .gh-catalog-empty{padding:22px 12px;color:#6b7280;text-align:center}
        @media (max-width:991px){.gh-catalog-title{font-size:26px}.gh-catalog-filters{grid-template-columns:1fr}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-catalog-shell">
        <section class="gh-catalog-hero">
            <div>
                <div class="gh-catalog-kicker">Không gian /gianhang</div>
                <h1 class="gh-catalog-title">Sản phẩm /gianhang</h1>
                <p class="gh-catalog-sub">Từ đây mình quản lý riêng lớp sản phẩm native của workspace /gianhang, nhìn rõ trạng thái public, bridge sang admin và mở nhanh sang cả native lẫn legacy.</p>
            </div>
            <div class="gh-catalog-actions">
                <a class="gh-catalog-btn gh-catalog-btn--primary" href="<%=AdminLegacyUrl %>">Mở quản lý admin</a>
                <a class="gh-catalog-btn gh-catalog-btn--soft" href="<%=ContentUrl %>">Nội dung /gianhang</a>
                <a class="gh-catalog-btn gh-catalog-btn--soft" href="<%=CompanionUrl %>">Dịch vụ /gianhang</a>
                <a class="gh-catalog-btn gh-catalog-btn--soft" href="<%=PublicListUrl %>" target="_blank">Xem public</a>
                <a class="gh-catalog-btn gh-catalog-btn--soft" href="<%=NativeManageUrl %>">Lớp native</a>
                <a class="gh-catalog-btn gh-catalog-btn--soft" href="<%=HubUrl %>">Về hub /gianhang</a>
            </div>
        </section>

        <section class="gh-catalog-card">
            <div class="gh-catalog-filters">
                <div class="gh-catalog-field">
                    <label for="<%= txt_search.ClientID %>">Tìm sản phẩm</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-catalog-input" MaxLength="120" placeholder="Tên, mô tả hoặc ID native"></asp:TextBox>
                </div>
                <div class="gh-catalog-field">
                    <label for="<%= ddl_visibility.ClientID %>">Hiển thị</label>
                    <asp:DropDownList ID="ddl_visibility" runat="server" CssClass="gh-catalog-input">
                        <asp:ListItem Value="all" Text="Tất cả"></asp:ListItem>
                        <asp:ListItem Value="visible" Text="Đang hiển thị"></asp:ListItem>
                        <asp:ListItem Value="hidden" Text="Đang ẩn"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc dữ liệu" CssClass="gh-catalog-submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-catalog-reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-catalog-summary">
            <div class="gh-catalog-summary-card"><small>Tổng sản phẩm</small><strong><%=TotalCount.ToString("#,##0") %></strong><span>Tổng sản phẩm native đang thuộc storefront này.</span></div>
            <div class="gh-catalog-summary-card"><small>Đang hiển thị</small><strong><%=VisibleCount.ToString("#,##0") %></strong><span>Sản phẩm đang hiển thị cho khách trên storefront.</span></div>
            <div class="gh-catalog-summary-card"><small>Đang ẩn</small><strong><%=HiddenCount.ToString("#,##0") %></strong><span>Sản phẩm đã tắt hoặc đang ẩn khỏi mặt public.</span></div>
            <div class="gh-catalog-summary-card"><small>Đã bridge</small><strong><%=MirroredCount.ToString("#,##0") %></strong><span>Sản phẩm đã có bản ghi bridge sang quản lý admin.</span></div>
            <div class="gh-catalog-summary-card"><small>Đang ưu đãi</small><strong><%=DiscountedCount.ToString("#,##0") %></strong><span>Sản phẩm đang có ưu đãi để phục vụ bán hàng nhanh.</span></div>
        </div>

        <section class="gh-catalog-card">
            <div class="gh-catalog-table-wrap">
                <table class="gh-catalog-table">
                    <thead>
                        <tr>
                            <th style="min-width:320px;">Sản phẩm</th>
                            <th style="min-width:130px;">Danh mục</th>
                            <th style="min-width:120px;">Giá</th>
                            <th style="min-width:120px;">Hiển thị</th>
                            <th style="min-width:150px;">Bridge admin</th>
                            <th style="min-width:150px;">Theo dõi</th>
                            <th style="min-width:150px;">Cập nhật</th>
                            <th style="min-width:260px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_rows" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="gh-catalog-media">
                                            <div class="gh-catalog-thumb">
                                                <img src="<%# Eval("ImageUrl") %>" alt="<%# Eval("Name") %>" />
                                            </div>
                                            <div class="gh-catalog-meta">
                                                <strong>#<%# Eval("NativeId") %> · <%# Eval("Name") %></strong>
                                                <span><%# Eval("Description") %></span>
                                            </div>
                                        </div>
                                    </td>
                                    <td><%# Eval("CategoryText") %></td>
                                    <td><strong><%# Eval("PriceText") %></strong></td>
                                    <td><span class='<%# Eval("VisibilityCss") %>'><%# Eval("VisibilityText") %></span></td>
                                    <td><%# Eval("BridgeText") %></td>
                                    <td>
                                        <div><%# Eval("StockText") %></div>
                                        <div class="text-muted"><%# Eval("DiscountText") %></div>
                                    </td>
                                    <td><%# Eval("UpdatedAtText") %></td>
                                    <td>
                                        <div class="gh-catalog-actions-list">
                                            <a class="gh-catalog-link" href="<%# Eval("WorkspaceDetailUrl") %>">Chi tiết workspace</a>
                                            <a class="gh-catalog-link gh-catalog-link--primary" href="<%# Eval("AdminEditUrl") %>">Mở admin</a>
                                            <a class="gh-catalog-link" href="<%# Eval("NativeEditUrl") %>">Mở native</a>
                                            <a class="gh-catalog-link" href="<%# Eval("PublicUrl") %>" target="_blank">Xem public</a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <tr><td colspan="8" class="gh-catalog-empty">Chưa có sản phẩm native nào khớp với bộ lọc hiện tại.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
