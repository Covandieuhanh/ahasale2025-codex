<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu_left.ascx.cs" Inherits="admin_uc_menu_left_uc" %>
<div class="navview-pane" style="z-index: 6">
    <div class="d-flex flex-align-center bg-navview-left-bc">
        <span class="pull-button m-0 bg-navview-left-bc-hover">
            <span class="mif-menu fg-white"></span>
        </span>
        <div class="app-title h4 text-light m-0 fg-white pl-2" style="line-height: 1.2; padding-top: 10px; padding-bottom: 10px;">
            <div>AHASALE.VN</div>
            <div class="admin-left-subtitle">Trang quản trị</div>
        </div>
    </div>

    <asp:PlaceHolder ID="ph_space_access_summary" runat="server" Visible="false">
        <div class="admin-nav-space-summary">
            <div class="admin-nav-space-summary-title">Đây là lối tắt truy cập các không gian tại Aha Sale</div>
            <div class="admin-nav-space-summary-list">
                <asp:Literal ID="lit_space_access_summary" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>

    <ul class="navview-menu" id="side-menu" style="display: none;">
        <% if (ShowLeftGroupAdmin()) { %>
        <li class="item-header">QUẢN LÝ ADMIN</li>
        <% if (ShowAdminDashboardTab()) { %>
        <li class="<%=MenuActive("/gianhang/admin/default.aspx") %>">
            <a href="/gianhang/admin/default.aspx">
                <span class="icon"><span class="mif-home"></span></span>
                <span class="caption">Trang chủ admin</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminAccount()) { %>
        <li class="<%=MenuActiveTaiKhoanScope("admin") %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/Default.aspx?scope=admin">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản admin</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupHome()) { %>
        <li class="item-header">QUẢN LÝ HOME</li>
        <% if (ShowLeftHomeAccount()) { %>
        <li class="<%=MenuActiveTaiKhoanScope("home") %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/Default.aspx?scope=home">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản home</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupShop()) { %>
        <li class="item-header">QUẢN LÝ GIAN HÀNG ĐỐI TÁC</li>
        <% if (ShowLeftShopAccount()) { %>
        <li class="<%=MenuActiveTaiKhoanScope("shop") %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/Default.aspx?scope=shop">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản gian hàng đối tác</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupContent()) { %>
        <li class="item-header">QUẢN LÝ NỘI DUNG</li>
        <% if (ShowLeftContentMenu()) { %>
        <li class="<%=MenuActive("/gianhang/admin/quan-ly-menu/default.aspx") %>">
            <a href="/gianhang/admin/quan-ly-menu/default.aspx">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Quản lý menu</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentBaiViet()) { %>
        <li class="<%=MenuActive("/gianhang/admin/quan-ly-bai-viet/default.aspx") %>">
            <a href="/gianhang/admin/quan-ly-bai-viet/default.aspx">
                <span class="icon"><span class="mif-news"></span></span>
                <span class="caption">Quản lý bài viết</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentThongBao()) { %>
        <li class="<%=MenuActive("/gianhang/admin/quan-ly-thong-bao/default.aspx") %>">
            <a href="/gianhang/admin/quan-ly-thong-bao/default.aspx">
                <span class="icon"><span class="mif-bell"></span></span>
                <span class="caption">Quản lý thông báo</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentTuVan()) { %>
        <li class="<%=MenuActive("/gianhang/admin/yeu-cau-tu-van/default.aspx") %>">
            <a href="/gianhang/admin/yeu-cau-tu-van/default.aspx">
                <span class="icon"><span class="mif-bubbles"></span></span>
                <span class="caption">Yêu cầu tư vấn</span>
            </a>
        </li>
        <% } %>
        <% } %>
    </ul>

    <div class="w-100 text-center text-small data-box p-2 border-top bd-darkGreen" style="position: absolute; bottom: 0">
        <%--bg-navview-foot-bc--%>
        <div>Sản phẩm của <a href="/" class="text-muted fg-white-hover no-decor">AhaSale.vn</a></div>
    </div>

</div>
