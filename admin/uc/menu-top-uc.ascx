<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu-top-uc.ascx.cs" Inherits="admin_uc_menu_top_uc" %>

<asp:Panel ID="pn_thongbao_legacy_cache" runat="server" Style="display:none;">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button ID="but_sapxep_moinhat" runat="server" Text="Mới nhất" CssClass="light small rounded" OnClick="but_sapxep_moinhat_Click" Style="display:none;" />
            <asp:Button ID="but_sapxep_chuadoc" runat="server" Text="Chưa đọc" CssClass="light small rounded" OnClick="but_sapxep_chuadoc_Click" Style="display:none;" />
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <asp:LinkButton ID="but_dadoc" runat="server" CommandArgument='<%# Eval("id") %>' OnClick="but_dadoc_Click" Style="display:none;">Đánh dấu đã đọc</asp:LinkButton>
                    <asp:LinkButton ID="but_chuadoc" runat="server" CommandArgument='<%# Eval("id") %>' OnClick="but_chuadoc_Click" Style="display:none;">Đánh dấu chưa đọc</asp:LinkButton>
                    <asp:LinkButton ID="but_xoathongbao" runat="server" CommandArgument='<%# Eval("id") %>' OnClick="but_xoathongbao_Click" Style="display:none;">Xóa</asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="ph_empty_thongbao" runat="server" Visible="false"></asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>

<div data-role="appbar" class="fg-white bg-nmenutop-bc admin-topbar" data-expand-point="lg" style="position: fixed; top: 0; z-index: 3">

    <a href="#" class="app-bar-item d-block d-none-lg" id="paneToggle"><span class="mif-menu"></span></a>

    <a class="app-bar-item fg-white admin-topbar-home" href="<%=GetAdminHomeUrl() %>"><span class="mif mif-home"></span></a>
    <a class="app-bar-item fg-white d-block-lg d-none fw-600 admin-topbar-title" style="z-index: 10!important" href="<%=GetAdminHomeUrl() %>"><%=ViewState["title"] %></a>
    <a class="app-bar-item d-block-lg d-none" href="#" id="admin-ui-density-toggle" title="Thu gọn menu">
        <span class="mif mif-list"></span>
    </a>
    <div class="app-bar-container ml-auto">

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HyperLink ID="but_show_form_thongbao" CssClass="app-bar-item" runat="server">
                    <span class="mif-notifications mif-lg"></span>
                    <span class="badge bg-orange fg-white mt-2 mr-1">
                        <asp:Label ID="lb_sl_thongbao" runat="server" Text="0"></asp:Label>
                    </span>
                </asp:HyperLink>
            </ContentTemplate>
        </asp:UpdatePanel>

        <div class="app-bar-container admin-avatar-wrap admin-shop-dd" id="admin-avatar-shell" style="visibility:hidden; opacity:0;">
            <button type="button" class="app-bar-item admin-shop-avatar-btn" id="admin-avatar-toggle" aria-haspopup="true" aria-expanded="false">
                <img src="<%=ViewState["anhdaidien"] %>" class="admin-shop-avatar-image" alt="avatar" />
                <span class="app-bar-name admin-shop-avatar-name"><%=ViewState["taikhoan"] %></span>
                <span class="admin-shop-avatar-caret">&#9660;</span>
            </button>
            <div class="admin-shop-menu" id="admin-avatar-dropdown">
                <div class="admin-shop-menu-head">
                    <p class="admin-shop-menu-title"><%=ViewState["hoten"] %></p>
                    <div class="admin-shop-menu-sub"><%=Server.HtmlEncode(ViewState["phanloai"] as string ?? "Nhân viên admin") %> • <%=Server.HtmlEncode(ViewState["taikhoan"] as string ?? "") %></div>
                    <div class="admin-shop-menu-role-row">
                        <span class="admin-shop-role-pill admin-shop-role-pill-primary"><%=Server.HtmlEncode(ViewState["admin_role_label"] as string ?? "Tài khoản quản trị") %></span>
                        <span class="admin-shop-role-pill admin-shop-role-pill-scope"><%=Server.HtmlEncode(ViewState["admin_scope_label"] as string ?? "Cổng Admin") %></span>
                    </div>
                    <div class="admin-shop-menu-help"><%=Server.HtmlEncode(ViewState["admin_role_description"] as string ?? "") %></div>
                </div>
                <div class="admin-shop-menu-body">
                    <% if (ShowMenuDashboard()) { %>
                    <div class="admin-shop-menu-group-title">Trang chính</div>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/default.aspx") %>' href="/admin/default.aspx">
                        <span>Trang chủ admin</span>
                        <span class="admin-shop-menu-badge">Dashboard</span>
                    </a>
                    <% } %>

                    <% if (ShowMenuGroupAdmin()) { %>
                    <div class="admin-shop-menu-group-title">Quản lý admin</div>
                    <% if (ShowMenuAdminAccount()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("admin") %>' href="<%= GetAdminAccountManagementUrl() %>">
                        <span>Quản lý tài khoản admin</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuOtp()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/otp/default.aspx") %>' href="/admin/otp/default.aspx?scope=home">
                        <span>Quản lý OTP</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuTransferHistory()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTransferHistory() %>' href="/admin/lich-su-chuyen-diem/default.aspx">
                        <span>Lịch sử chuyển điểm</span>
                        <span class="admin-shop-menu-badge"><%=ViewState["DongA"] %></span>
                    </a>
                    <% } %>
                    <% if (ShowMenuTokenWallet()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTokenWallet() %>' href="/admin/vi-token-diem/default.aspx">
                        <span>Ví token điểm</span>
                        <span class="admin-shop-menu-badge">Super</span>
                    </a>
                    <% } %>
                    <% } %>

                    <% if (ShowMenuGroupHome()) { %>
                    <div class="admin-shop-menu-group-title">Quản lý home</div>
                    <% if (ShowMenuHomeAccount()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("home") %>' href="<%= GetHomeAccountManagementUrl() %>">
                        <span>Quản lý tài khoản home</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuApproveHanhVi()) { %>
                    <a class='admin-shop-menu-item <%= MenuActivePointApproval() %>' href="<%= GetApproveHomePointUrl() %>">
                        <span>Duyệt yêu cầu điểm / hành vi</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuIssueCard()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/phat-hanh-the.aspx", "/admin/phat-hanh-the/them-moi.aspx") %>' href="/admin/phat-hanh-the/them-moi.aspx">
                        <span>Phát hành thẻ</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuTierDescription()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/motacapbac.aspx", "/admin/MoTaCapBac.aspx") %>' href="/admin/MoTaCapBac.aspx">
                        <span>Mô tả cấp bậc</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuSellProduct()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/he-thong-san-pham/ban-san-pham.aspx", "/admin/he-thong-san-pham/ban-the.aspx", "/admin/he-thong-san-pham/chi-tiet-giao-dich.aspx") %>' href="/admin/he-thong-san-pham/ban-the.aspx">
                        <span>Bán sản phẩm</span>
                    </a>
                    <% } %>
                    <% } %>

                    <% if (ShowMenuGroupShop()) { %>
                    <div class="admin-shop-menu-group-title">Quản lý gian hàng đối tác</div>
                    <% if (ShowMenuShopAccount()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("shop") %>' href="<%= GetShopAccountManagementUrl() %>">
                        <span>Quản lý tài khoản gian hàng đối tác</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopPointApproval()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveShopPointApproval() %>' href="/admin/lich-su-chuyen-diem/default.aspx?tab=shop-only">
                        <span>Duyệt điểm / nghiệp vụ shop</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopEmailTemplate()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-email-shop/default.aspx") %>' href="/admin/quan-ly-email-shop/default.aspx">
                        <span>Nội dung email gian hàng đối tác</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopApprove()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/duyet-gian-hang-doi-tac.aspx") %>' href="/admin/duyet-gian-hang-doi-tac.aspx">
                        <span>Duyệt gian hàng đối tác</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/duyet-nang-cap-level2.aspx") %>' href="/admin/duyet-nang-cap-level2.aspx">
                        <span>Duyệt nâng cấp Level 2</span>
                    </a>
                    <% } %>
                    <% } %>

                    <% if (ShowMenuGroupContent()) { %>
                    <div class="admin-shop-menu-group-title">Quản lý nội dung</div>
                    <% if (ShowMenuHomeContent()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/cai-dat-trang-chu/default.aspx") %>' href="/admin/cai-dat-trang-chu/default.aspx">
                        <span>Cài đặt trang chủ</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuHomeTextContent()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-noi-dung-home/default.aspx") %>' href="/admin/quan-ly-noi-dung-home/default.aspx">
                        <span>Nội dung trang chủ Home</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentMenu()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-menu/default.aspx", "/admin/quan-ly-menu/them-moi.aspx", "/admin/quan-ly-menu/bo-loc.aspx", "/admin/quan-ly-menu/chinh-sua.aspx", "/admin/quan-ly-menu/xuat-du-lieu.aspx", "/admin/quan-ly-menu/ban-in.aspx") %>' href="/admin/quan-ly-menu/default.aspx">
                        <span>Quản lý menu</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentBaiViet()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-bai-viet/default.aspx", "/admin/quan-ly-bai-viet/in.aspx", "/admin/quan-ly-bai-viet/them-moi.aspx", "/admin/quan-ly-bai-viet/bo-loc.aspx", "/admin/quan-ly-bai-viet/chinh-sua.aspx", "/admin/quan-ly-bai-viet/xuat-du-lieu.aspx", "/admin/quan-ly-bai-viet/ban-in.aspx") %>' href="/admin/quan-ly-bai-viet/default.aspx">
                        <span>Quản lý bài viết</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentBanner()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-banner/default.aspx", "/admin/quan-ly-banner/them-moi.aspx") %>' href="/admin/quan-ly-banner/default.aspx">
                        <span>Quản lý banner</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentGopY()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-gop-y/default.aspx") %>' href="/admin/quan-ly-gop-y/default.aspx">
                        <span>Quản lý góp ý</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentThongBao()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-thong-bao/default.aspx", "/admin/quan-ly-thong-bao/in.aspx", "/admin/quan-ly-thong-bao/bo-loc.aspx", "/admin/quan-ly-thong-bao/xuat-du-lieu.aspx", "/admin/quan-ly-thong-bao/ban-in.aspx") %>' href="/admin/quan-ly-thong-bao/default.aspx">
                        <span>Quản lý thông báo</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentTuVan()) { %>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/yeu-cau-tu-van/default.aspx", "/admin/yeu-cau-tu-van/bo-loc.aspx", "/admin/yeu-cau-tu-van/xuat-du-lieu.aspx", "/admin/yeu-cau-tu-van/ban-in.aspx") %>' href="/admin/yeu-cau-tu-van/default.aspx">
                        <span>Yêu cầu tư vấn</span>
                    </a>
                    <% } %>
                    <% } %>
                </div>
                <div class="admin-shop-menu-footer">
                    <asp:HyperLink ID="but_show_form_doimatkhau" runat="server" CssClass="admin-shop-btn admin-shop-btn-light">Đổi mật khẩu</asp:HyperLink>
                    <asp:LinkButton ID="but_dangxuat" runat="server" CssClass="admin-shop-btn admin-shop-btn-danger" OnClick="but_dangxuat_Click">Đăng xuất</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</div>
