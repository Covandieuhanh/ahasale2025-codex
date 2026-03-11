<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header_uc.ascx.cs" Inherits="Uc_Home_Header_uc" %>
<style>
    :root {
        --aha-top-icon-bg: #ffffff;
        --aha-top-icon-border: #d9e2ec;
        --aha-top-icon-text: #111111;
    }

    html[data-bs-theme="dark"],
    body[data-bs-theme="dark"],
    body.theme-dark,
    body.dark,
    body.dark-mode {
        --aha-top-icon-bg: #1f2937;
        --aha-top-icon-border: #334155;
        --aha-top-icon-text: #f3f4f6;
    }

    .notif-item {
        position: relative;
    }

    .home-top-circle-btn {
        width: 46px;
        height: 46px;
        min-width: 46px;
        min-height: 46px;
        border-radius: 999px;
        border: 1px solid var(--aha-top-icon-border);
        background: var(--aha-top-icon-bg);
        color: var(--aha-top-icon-text);
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: all .2s ease;
    }

    .site-header .home-top-circle-btn i,
    .site-header .home-top-circle-btn .ti,
    .site-header .d-lg-none .btn.btn-icon.bg-white i,
    .site-header .d-lg-none .btn.btn-icon.bg-white .ti {
        font-size: 22px;
        color: var(--aha-top-icon-text) !important;
    }

    .site-header .d-lg-none .btn.btn-icon.bg-white {
        border: 1px solid var(--aha-top-icon-border) !important;
        background: var(--aha-top-icon-bg) !important;
    }

    .home-top-circle-btn:hover {
        border-color: #c8d4e1;
        background: #f8fafc;
        transform: translateY(-1px);
    }

    #notifPopup {
        background: var(--aha-home-dd-bg, #ffffff) !important;
        border: 1px solid var(--aha-home-dd-border, #dbe6ef);
        color: var(--aha-home-dd-text, #102a43);
    }

    .home-dropdown-avatar {
        width: 88px !important;
        height: 88px !important;
        min-width: 88px !important;
        min-height: 88px !important;
        max-width: 88px !important;
        max-height: 88px !important;
        aspect-ratio: 1 / 1;
        border-radius: 50% !important;
        display: inline-block;
        overflow: hidden;
        background-size: cover !important;
        background-position: center center !important;
        background-repeat: no-repeat !important;
        flex: 0 0 88px;
    }

    .notif-kebab {
        position: absolute;
        right: 12px;
        bottom: 12px;
    }

    @media (min-width: 992px) {
        .site-header .brand-center-mobile {
            display: none !important;
        }

        .site-header #navbar-menu {
            width: 100%;
            display: grid !important;
            grid-template-columns: minmax(0, 1fr) auto minmax(0, 1fr);
            align-items: center;
            column-gap: 16px;
        }

        .site-header .header-left-nav {
            justify-self: start;
            min-width: 0;
        }

        .site-header .header-right-nav {
            justify-self: end;
            min-width: 0;
        }

        .site-header .brand-center-desktop {
            justify-self: center;
            display: inline-flex;
            align-items: center;
            font-weight: 700;
            color: #ffffff !important;
            text-decoration: none;
            white-space: nowrap;
        }

        .site-header .brand-center-desktop img {
            height: 30px !important;
            width: auto;
        }
    }

    .site-header .brand-center-mobile,
    .site-header .brand-center-mobile span,
    .site-header .brand-center-desktop,
    .site-header .brand-center-desktop span {
        color: #ffffff !important;
    }

    @media (max-width: 575.98px) {
        .site-header .container-fluid {
            padding-left: 6px;
            padding-right: 6px;
        }

        .site-header .brand-center-mobile {
            position: absolute;
            left: 50%;
            transform: translateX(-50%);
            max-width: 132px;
            z-index: 1;
            pointer-events: auto;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .site-header .brand-center-mobile img {
            height: 24px !important;
            width: auto;
        }

        .site-header .brand-center-mobile span {
            display: inline-block;
            max-width: 100%;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            font-size: 1.05rem;
            font-weight: 700;
        }

        .site-header .d-lg-none.ms-auto.d-flex {
            gap: 4px !important;
        }

        .site-header .d-lg-none.ms-auto.d-flex .btn,
        .site-header .d-lg-none.ms-auto.d-flex .home-top-circle-btn {
            width: 34px;
            height: 34px;
            min-width: 34px;
            min-height: 34px;
            padding: 0;
        }

        .site-header .d-lg-none a[title="Tin đã lưu"] {
            display: none !important;
        }

        .site-header .d-lg-none.ms-auto.d-flex .btn .ti,
        .site-header .d-lg-none.ms-auto.d-flex .home-top-circle-btn .ti {
            font-size: 18px !important;
        }
    }

    @media (max-width: 389.98px) {
        .site-header .brand-center-mobile span {
            font-size: .96rem;
        }
    }
</style>
<header class="navbar navbar-expand-lg fixed-top d-print-none site-header">
    <div class="container-fluid position-relative">

        <!-- MOBILE: Hamburger -->
        <button class="btn btn-icon home-top-circle-btn bg-white d-lg-none" type="button"
            data-bs-toggle="offcanvas" data-bs-target="#mobileMenu"
            aria-controls="mobileMenu" aria-label="Menu">
            <i class="ti ti-menu-2"></i>
        </button>

        <!-- BRAND MOBILE -->
        <a class="navbar-brand fw-bold d-flex align-items-center brand-center-mobile d-lg-none"
            href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/" %>">
            <img src="/uploads/images/logo-aha-trang.png" class="me-2" style="height: 28px" alt="AhaSale" />
            <span>AhaSale</span>
        </a>

        <!-- DESKTOP NAV -->
        <div class="collapse navbar-collapse d-none d-lg-flex" id="navbar-menu">
            <ul class="navbar-nav align-items-lg-center gap-lg-2 header-left-nav">
                <% if (PortalRequest_cl.IsShopPortalRequest()) { %>
                <li class="nav-item"><a class="nav-link fw-semibold" href="/dang-nhap?switch=home">Chuyển sang home (cá nhân)</a></li>
                <% } else { %>
                <%=show_danhmuc_nav %>
                <li class="nav-item"><a class="nav-link fw-semibold" href="/shop/login.aspx?switch=shop">Chuyển sang shop</a></li>
                <% } %>
                <asp:PlaceHolder ID="phDonBan" runat="server" Visible="false"></asp:PlaceHolder>
               <%-- <li class="nav-item"><a class="nav-link fw-semibold" href="#">Bất động sản</a></li>
                <li class="nav-item"><a class="nav-link fw-semibold" href="#">Việc làm</a></li>--%>
            </ul>

            <a class="brand-center-desktop"
                href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/" %>">
                <img src="/uploads/images/logo-aha-trang.png" class="me-2" alt="AhaSale" />
                <span>AhaSale</span>
            </a>

            <!-- DESKTOP RIGHT -->
            <div class="navbar-nav flex-row align-items-center gap-2 header-right-nav">
                <asp:PlaceHolder ID="phTopDesktopHomeUtilities" runat="server" Visible="false">
                    <div class="position-relative d-none d-lg-block">
                        <a href="/home/quan-ly-tin/tin-da-luu.aspx"
                            class="home-top-circle-btn"
                            title="Tin đã lưu">
                            <i class="ti ti-heart"></i>
                        </a>
                    </div>
                    <div class="position-relative d-none d-lg-block">
                        <a href="/home/gio-hang.aspx"
                            class="home-top-circle-btn position-relative"
                            title="Giỏ hàng">
                            <i class="ti ti-shopping-cart"></i>
                            <span id="badgeGioHangDesktop" runat="server" class="badge position-absolute top-0 end-0"
                                style="background: #FFF3CD; color: #856404; font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                <asp:Label ID="lb_sl_giohang_desktop" runat="server" Text="0"></asp:Label>
                            </span>
                        </a>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phTopNotificationDesktop" runat="server" Visible="false">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="nav-item position-relative">
                                <asp:LinkButton ID="but_show_form_thongbao"
                                    runat="server"
                                    CssClass="home-top-circle-btn position-relative"
                                    OnClick="but_show_form_thongbao_Click"
                                    OnClientClick="showNotif();">
                                    <i class="ti ti-bell"></i>
                                    <span id="badgeThongBaoDesktop" runat="server" class="badge bg-red position-absolute top-0 end-0"
                                        style="font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                        <asp:Label ID="lb_sl_thongbao_desktop" runat="server" Text="0"></asp:Label>
                                    </span>
                                </asp:LinkButton>
                                <div class="dropdown-menu dropdown-menu-end p-0 notif-dropdown"
                                    id="notifHostDesktop"
                                    style="width: 380px; border-radius: 16px;">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                    <% if (!PortalRequest_cl.IsShopPortalRequest() && string.IsNullOrEmpty(PortalRequest_cl.GetCurrentAccountEncrypted())) { %>
                    <a href="/dang-nhap" class="btn btn-dark rounded-pill  px-3">Đăng nhập</a>
                    <a href="/home/dangky.aspx"
                        class="btn flex-fill"
                        style="border-radius: 999px; background: #e9f7ef; color: #146c43; border: 1px solid rgba(25,135,84,.45);">Đăng ký
                    </a>
                    <% } %>
                </asp:PlaceHolder>

                <!-- USER DROPDOWN -->
                <asp:PlaceHolder ID="phTopDesktopAccount" runat="server" Visible="true">
                    <% if (!string.IsNullOrEmpty(PortalRequest_cl.GetCurrentAccountEncrypted())) { %>
                    <div class="nav-item aha-account">
                        <a id="userMenuToggle" href="#" class="home-account-toggle"
                            aria-expanded="false">
                            <span class="home-account-avatar" style="background-image: url('<%= ViewState["anhdaidien"] %>')"></span>
                            <span class="home-account-name"><%: (ViewState["hoten"] ?? "Tài khoản").ToString() %></span>
                            <i class="ti ti-chevron-down home-account-caret"></i>
                        </a>

                        <div class="account-dropdown account-menu-host"
                            id="accountMenuHostDesktop"
                            style="width: 360px; border-radius: 16px;">
                        </div>
                    </div>
                    <% } %>
                </asp:PlaceHolder>
            </div>
        </div>

        <!-- MOBILE RIGHT: notif + account offcanvas -->
        <div class="d-lg-none ms-auto d-flex align-items-center gap-2">
            <asp:PlaceHolder ID="phTopMobileHomeUtilities" runat="server" Visible="true">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <a class="btn btn-icon home-top-circle-btn bg-white position-relative"
                            href="/home/gio-hang.aspx"
                            title="Giỏ hàng">
                            <i class="ti ti-shopping-cart text-success"></i>
                            <span id="badgeGioHangMobile" runat="server" class="position-absolute top-0 start-100 translate-middle badge rounded-pill"
                                style="background: #FFF3CD; color: #856404; font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                <asp:Label ID="lb_sl_giohang_mobile" runat="server" Text="0"></asp:Label>
                            </span>
                        </a>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:PlaceHolder>
            <!-- ✅ NOTIF OFFCANVAS BUTTON (NEW) -->
            <asp:PlaceHolder ID="phTopNotificationMobile" runat="server" Visible="true">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:LinkButton ID="LinkButton1"
                            runat="server"
                            CssClass="btn btn-icon home-top-circle-btn bg-white position-relative"
                            OnClick="but_show_form_thongbao_Click">
                            <i class="ti ti-bell"></i>
                            <span id="badgeThongBaoMobile" runat="server" class="badge bg-red position-absolute top-0 end-0"
                                style="font-size: 11px; min-width: 18px; height: 18px; line-height: 18px; padding: 0 4px;">
                                <asp:Label ID="lb_sl_thongbao_mobile" runat="server" Text="0"></asp:Label>
                            </span>
                        </asp:LinkButton>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:PlaceHolder>
            <!--FAVORITE -->
            <asp:PlaceHolder ID="phTopMobileFavorite" runat="server" Visible="true">
                <a href="/home/quan-ly-tin/tin-da-luu.aspx"
                    class="btn btn-icon home-top-circle-btn bg-white"
                    title="Tin đã lưu">
                    <i class="ti ti-heart text-success"></i>
                </a>
            </asp:PlaceHolder>

            <!-- account -->
            <asp:PlaceHolder ID="phTopMobileAccount" runat="server" Visible="true">
                <button type="button"
                    class="btn btn-icon home-top-circle-btn bg-white position-relative p-0"
                    data-bs-toggle="offcanvas"
                    data-bs-target="#accountMenuCanvas"
                    aria-controls="accountMenuCanvas"
                    aria-label="Tài khoản"
                    style="overflow: hidden;">
                    <span class="d-block w-100 h-100"
                        style="background-image: url('<%= ViewState["anhdaidien"] %>'); background-size: cover; background-position: center; border-radius: 50%;"></span>
                </button>
            </asp:PlaceHolder>

        </div>

    </div>
</header>

<!-- MOBILE MAIN MENU OFFCANVAS -->
<div class="offcanvas offcanvas-start" tabindex="-1" id="mobileMenu" aria-labelledby="mobileMenuLabel">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="mobileMenuLabel">Menu</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>

    <div class="offcanvas-body">
        <div class="list-group list-group-flush">
            <%= show_danhmuc_mobile %>
           <%-- <a href="#" class="list-group-item list-group-item-action fw-semibold">Bất động sản</a>
            <a href="#" class="list-group-item list-group-item-action fw-semibold">Việc làm</a>--%>
        </div>

        <div class="mt-3 d-flex gap-2">
            <asp:PlaceHolder ID="phDangNhap" runat="server">
                <a href="/dang-nhap" class="btn btn-outline-secondary flex-fill">Đăng nhập</a>
            </asp:PlaceHolder>
            <%--<a href="/home/quan-ly-tin/default.aspx" class="btn btn-primary flex-fill">Đăng tin</a>--%>
        </div>
    </div>
</div>


<!-- MOBILE ACCOUNT OFFCANVAS -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="accountMenuCanvas" aria-labelledby="accountMenuCanvasLabel">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="accountMenuCanvasLabel">Tài khoản</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>

    <div class="offcanvas-body p-3">
        <div id="accountMenuHostMobile" class="account-dropdown"></div>
    </div>
</div>



<!-- ✅ TEMPLATE THÔNG BÁO (NEW) -->
<div id="notifPopup"
    style="display: none; position: fixed; right: 0; top: 45px; width: 380px; background: #fff; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,.15); z-index: 9999;">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="notif-head p-3">
                <div class="d-flex align-items-center justify-content-between">
                    <div class="fw-bold" style="font-size: 1.05rem;">Thông báo</div>

                    <div class="d-flex gap-2">
                        <a href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/home/quan-ly-tin/default.aspx" %>" class="btn btn-sm btn-success" style="border-radius: 10px;">Xem tất cả
                        </a>
                    </div>
                </div>

                <div class="mt-3 d-flex gap-2">
                    <asp:Button ID="but_sapxep_moinhat_desk" OnClick="but_sapxep_moinhat_Click" type="button" runat="server" class="btn btn-sm btn-outline-secondary active" Style="border-radius: 999px;" Text="Tất cả" />
                    <asp:Button ID="but_sapxep_chuadoc_desk" OnClick="but_sapxep_chuadoc_Click" runat="server" Text="Chưa đọc" CssClass="btn btn-sm btn-outline-secondary" Style="border-radius: 999px;" />
                </div>
            </div>
            <div style="max-height: 75vh; overflow-y: auto;">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>

                        <div class="list-group list-group-flush">
                            <div class="list-group-item notif-item position-relative">

                                <!-- LINK nội dung -->
                                <a href="<%# Eval("link") %>idtb=<%# Eval("id") %>"
                                    class="list-group-item-action d-block text-decoration-none">

                                    <div class="d-flex align-items-start gap-3">
                                        <img class="avatar avatar-sm rounded"
                                            src="<%# Eval("avt_nguoithongbao") %>" />

                                        <div class="flex-fill">
                                            <!-- nội dung -->
                                            <div class="d-flex align-items-center gap-2">
                                                <asp:PlaceHolder runat="server"
                                                    Visible='<%# Eval("daxem").ToString() == "True" %>'>
                                                    <div>
                                                        <%# Eval("noidung") %>
                                                    </div>
                                                </asp:PlaceHolder>

                                                <asp:PlaceHolder runat="server"
                                                    Visible='<%# Eval("daxem").ToString() == "False" %>'>
                                                    <div class="fw-bold text-warning">
                                                        <%# Eval("noidung") %>
                                                    </div>
                                                    <span class="notif-dot" title="Chưa đọc"></span>
                                                </asp:PlaceHolder>
                                            </div>

                                            <!-- thời gian -->
                                            <div class="notif-time text-muted mt-1">
                                                <i class="ti ti-clock"></i>
                                                <%# Eval("thoigian", "{0:dd/MM/yyyy HH:mm}") %>
                                            </div>
                                        </div>
                                    </div>
                                </a>

                                <!-- DROPDOWN -->
                                <div class="dropdown notif-kebab">
                                    <button type="button" style="border: none; background: none; box-shadow: none;"
                                        class="btn text-muted notif-kebab-btn"
                                        data-bs-toggle="dropdown"
                                        aria-expanded="false">
                                        ...
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <asp:PlaceHolder runat="server"
                                            Visible='<%# Eval("daxem").ToString()=="False" %>'>
                                            <li>
                                                <asp:LinkButton runat="server"
                                                    CssClass="dropdown-item"
                                                    CommandArgument='<%# Eval("id") %>'
                                                    OnClick="but_dadoc_Click">
                                                    Đánh dấu đã đọc
                                                </asp:LinkButton>
                                            </li>
                                        </asp:PlaceHolder>

                                        <asp:PlaceHolder runat="server"
                                            Visible='<%# Eval("daxem").ToString()=="True" %>'>
                                            <li>
                                                <asp:LinkButton runat="server"
                                                    CssClass="dropdown-item"
                                                    CommandArgument='<%# Eval("id") %>'
                                                    OnClick="but_chuadoc_Click">
                                                    Đánh dấu chưa đọc
                                                </asp:LinkButton>
                                            </li>
                                        </asp:PlaceHolder>

                                        <li>
                                            <asp:LinkButton runat="server"
                                                CssClass="dropdown-item"
                                                CommandArgument='<%# Eval("id") %>'
                                                OnClick="but_xoathongbao_Click">
                                                Xóa thông báo
                                            </asp:LinkButton>
                                        </li>
                                    </ul>
                                </div>

                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="ph_empty_thongbao" runat="server" Visible="false">
                    <div class="p-3 text-muted">Không có thông báo nào.</div>
                </asp:PlaceHolder>

            </div>
            <div class="notif-foot p-3">
                <a href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/default.aspx" : "/home/quan-ly-tin/default.aspx" %>" class="btn btn-outline-secondary w-100" style="border-radius: 12px;">Quản lý thông báo
                </a>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

<!-- ✅ CHÈN MENU USER THẬT Ở ĐÂY -->
<div id="accountMenuContent" style="display: none;">
    <div class="account-menu-shell">
        <div class="account-menu-body">
    <asp:UpdatePanel ID="UpdatePanelGuestCard" runat="server" Visible="false" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card border-0 shadow-sm mb-3" style="border-radius: 14px;">
                <div class="card-body p-3">
                    <div class="fw-bold" style="font-size: 1.05rem;">
                        Mua thì hời, bán thì lời.
                    </div>
                    <div class="text-secondary">
                        Đăng nhập cái đã!
                    </div>

                    <!-- 2 cột -->
                    <div class="row g-2 mt-3">
                        <div class="col-6">
                            <a href="/dang-nhap"
                                class="btn btn-success w-100"
                                style="border-radius: 12px;">Đăng nhập
                            </a>
                        </div>

                        <div class="col-6">
                            <a href="/home/dangky.aspx"
                                class="btn btn-outline-success w-100"
                                style="border-radius: 12px; border-color: #35b36a; color: #2f9e5f;">Đăng ký
                            </a>
                        </div>
                    </div>
                    <asp:PlaceHolder ID="phGuestSwitchHome" runat="server" Visible="false">
                        <div class="mt-2">
                            <a href="/dang-nhap?switch=home"
                                class="btn btn-warning w-100"
                                style="border-radius: 12px;">Chuyển sang tài khoản cá nhân</a>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ================= MENU CŨ ================= -->
    <!-- User header -->
    <asp:UpdatePanel ID="UpdatePanelUser" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderLogged" runat="server" Visible="false">
                <div class="card border-0 bg-light mb-3" style="border-radius: 14px; overflow: hidden;">
                    <div class="p-3">
                        <div class="d-flex align-items-center gap-3">
                            <span class="avatar avatar-lg rounded-circle home-dropdown-avatar"
                                style="background-image: url('<%= ViewState["anhdaidien"] %>')"></span>

                            <div class="flex-fill">
                                <div class="fw-bold" style="font-size: 1.05rem;">
                                    <%= ViewState["hoten"] %>
                                </div>
                                <div class="text-secondary small">
                                    <%= ViewState["taikhoan"] %>
                                </div>
                                <div class="mt-1">
                                    <%= ViewState["phanloai"] %>
                                </div>
                            </div>

                            <a href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/cap-nhat-ho-so" : "/home/edit-info.aspx" %>"
                                class="btn btn-sm btn-outline-secondary"
                                style="border-radius: 10px;">
                                <i class="ti ti-pencil"></i>
                            </a>
                        </div>

                        <!-- ✅ 2 hồ sơ shop (chỉ hiện khi là gian hàng đối tác) -->
                        <asp:PlaceHolder ID="phHoSoShopOnly" runat="server" Visible="false">
                            <div class="d-flex flex-column gap-2 mt-3">
                                <!-- 5) Hồ sơ tiêu dùng shop -->
                                <a href="/shop/ho-so-tieu-dung"
                                    class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                                    style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                                    <span class="avatar avatar-xs rounded-circle bg-white border d-flex align-items-center justify-content-center"
                                        style="width: 22px; height: 22px;">
                                        <img src="/uploads/images/dong-a.png" alt="Hồ sơ tiêu dùng shop"
                                            style="width: 14px; height: 14px; object-fit: contain;">
                                    </span>

                                    <span class="flex-fill" style="min-width: 0;">
                                        <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ tiêu dùng shop
                                        </span>
                                        <span class="d-block fw-semibold" style="line-height: 1.2;">
                                            <%= ViewState["HoSo_TieuDung_ShopOnly"] %>
                                        </span>
                                    </span>

                                    <i class="ti ti-chevron-right text-muted"></i>
                                </a>

                                <!-- 6) Hồ sơ ưu đãi shop -->
                                <a href="/shop/ho-so-uu-dai"
                                    class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                                    style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                                    <span class="avatar avatar-xs rounded-circle bg-success-lt text-success d-flex align-items-center justify-content-center"
                                        style="width: 22px; height: 22px;">
                                        <i class="ti ti-ticket"></i>
                                    </span>

                                    <span class="flex-fill" style="min-width: 0;">
                                        <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ ưu đãi shop
                                        </span>
                                        <span class="d-block fw-semibold" style="line-height: 1.2;">
                                            <%= ViewState["HoSo_UuDai_ShopOnly"] %>
                                        </span>
                                    </span>

                                    <i class="ti ti-chevron-right text-muted"></i>
                                </a>
                            </div>
                        </asp:PlaceHolder>

                    </div>

                    <!-- Menu list (giống vùng khoanh đỏ) -->
                    <div class="list-group list-group-flush bg-white" style="border-top: 1px solid rgba(0,0,0,.06);">

                        <!-- Hồ sơ -->
                        <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? (ViewState["public_profile_link"] ?? "/shop/default.aspx").ToString() : ("/" + (ViewState["taikhoan"] ?? "").ToString() + ".info") %>">
                            <div class="d-flex align-items-center gap-2">
                                <i class="ti ti-user-circle text-secondary"></i>
                                <span class="fw-medium">Trang cá nhân</span>
                            </div>
                            <i class="ti ti-chevron-right text-secondary"></i>
                        </a>
                        <asp:PlaceHolder ID="phMenuHomeYeuCau" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/tao-yeu-cau.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-file-plus text-secondary"></i>
                                    <span class="fw-medium">Tạo yêu cầu</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeCopyLink" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3"
                               href="javascript:void(0);"
                               data-copy-ref="1"
                               data-ref-link="<%= System.Web.HttpUtility.HtmlAttributeEncode("https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + (ViewState["taikhoan"] ?? "").ToString()) %>"
                               onclick="return copyReferralLink(this);">
                                <span class="d-flex align-items-center gap-2">
                                    <i class="ti ti-link text-secondary"></i>
                                    <span class="fw-medium">Sao chép Link giới thiệu</span>
                                </span>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeDoiPin" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/DoiPin.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-key text-secondary"></i>
                                <span class="fw-medium">Đổi mã pin thẻ</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="<%= ((ViewState["portal_scope"] ?? "").ToString() == PortalScope_cl.ScopeShop) ? "/shop/doi-mat-khau" : "/home/DoiMatKhau.aspx" %>">
                            <div class="d-flex align-items-center gap-2">
                                <i class="ti ti-lock text-secondary"></i>
                            <span class="fw-medium">Đổi mật khẩu tài khoản</span>
                            </div>
                            <i class="ti ti-chevron-right text-secondary"></i>
                        </a>

                        <%-- Ẩn: Mã QR của tôi --%>

                        <asp:PlaceHolder ID="phMenuHomeKhachHang" runat="server" Visible="false"></asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeDonMua" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/don-mua.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-shopping-bag text-secondary"></i>
                                    <span class="fw-medium">Đơn mua</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuHomeLichSuTraoDoi" runat="server" Visible="true">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-giao-dich.aspx">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-clock text-secondary"></i>
                                    <span class="fw-medium">Lịch sử Trao đổi</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phMenuShopTinhNang" runat="server" Visible="false">
                            <div class="px-3 pt-2 pb-1 text-secondary small fw-semibold border-top">Tính năng gian hàng</div>

                            <div class="px-3 pt-2 pb-1 text-secondary small">Hồ sơ shop</div>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/ho-so-tieu-dung">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-coin text-secondary"></i>
                                    <span class="fw-medium">Hồ sơ tiêu dùng shop</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/ho-so-uu-dai">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-ticket text-secondary"></i>
                                    <span class="fw-medium">Hồ sơ ưu đãi shop</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <div class="px-3 pt-2 pb-1 text-secondary small">Vận hành gian hàng</div>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/quan-ly-tin">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-news text-secondary"></i>
                                    <span class="fw-medium">Quản lý tin shop</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/don-ban">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-receipt text-secondary"></i>
                                    <span class="fw-medium">Đơn bán</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/cho-thanh-toan">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-credit-card text-secondary"></i>
                                    <span class="fw-medium">Chờ trao đổi</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/khach-hang">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-users text-secondary"></i>
                                    <span class="fw-medium">Khách hàng</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>

                            <div class="px-3 pt-2 pb-1 text-secondary small">Thiết lập shop</div>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/cap-nhat-ho-so">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-pencil text-secondary"></i>
                                    <span class="fw-medium">Cập nhật hồ sơ</span>
                                </div>
                                <i class="ti ti-chevron-right text-secondary"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phSwitchToShop" runat="server" Visible="false">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/shop/login.aspx?switch=shop">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-building-store text-success"></i>
                                    <span class="fw-semibold text-success">Chuyển sang gian hàng đối tác</span>
                                </div>
                                <i class="ti ti-chevron-right text-success"></i>
                            </a>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phSwitchToHome" runat="server" Visible="false">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/dang-nhap?switch=home">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="ti ti-user-circle text-warning"></i>
                                    <span class="fw-semibold text-warning">Chuyển sang tài khoản cá nhân</span>
                                </div>
                                <i class="ti ti-chevron-right text-warning"></i>
                            </a>
                        </asp:PlaceHolder>


                    </div>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ================= TIỆN ÍCH ================= -->
    <asp:PlaceHolder ID="phUtilityHome" runat="server" Visible="true">
        <div class="text-secondary small fw-semibold px-1 mb-2">Tiện ích</div>
        <div class="card border-0 bg-light mb-3" style="border-radius: 14px;">
            <div class="list-group list-group-flush" style="border-radius: 14px; overflow: hidden;">
                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/tin-da-luu.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-heart text-secondary"></i>
                        <span class="fw-medium">Tin đăng đã lưu</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/lich-su-xem-tin.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-history text-secondary"></i>
                        <span class="fw-medium">Lịch sử xem tin</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/danh-gia-tu-toi.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-star text-secondary"></i>
                        <span class="fw-medium">Đánh giá từ tôi</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>
            </div>
        </div>
    </asp:PlaceHolder>

    <!-- ================= DỊCH VỤ TRẢ PHÍ ================= -->
    <%-- <div class="text-secondary small fw-semibold px-1 mb-2">Gian hàng đối tác</div>
    <div class="card border-0 bg-light mb-3" style="border-radius: 14px;">
        <div class="list-group list-group-flush" style="border-radius: 14px; overflow: hidden;">
            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-giao-dich.aspx">
                <div class="d-flex align-items-center gap-2">
                    <span class="avatar avatar-xs rounded-circle bg-white border d-flex align-items-center justify-content-center">
                        <img src="/uploads/images/dong-a.png" alt="Quyền tiêu dùngha" style="width: 14px; height: 14px; object-fit: contain;">
                    </span>
                    <span class="fw-medium">Quyền tiêu dùngha</span>
                </div>
                <i class="ti ti-chevron-right text-secondary"></i>
            </a>

            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="#">
                <div class="d-flex align-items-center gap-2">
                    <span class="avatar avatar-xs rounded-circle bg-dark text-white d-flex align-items-center justify-content-center">
                        <span class="fw-bold" style="font-size: 10px;">PRO</span>
                    </span>
                    <span class="fw-medium">Gói PRO</span>
                </div>
                <i class="ti ti-chevron-right text-secondary"></i>
            </a>

            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/dang-ky-gian-hang-doi-tac.aspx">
                <div class="d-flex align-items-center gap-2">
                    <span class="avatar avatar-xs rounded-circle bg-orange-lt text-orange d-flex align-items-center justify-content-center">
                        <i class="ti ti-check"></i>
                    </span>
                    <span class="fw-medium">Tạo cửa hàng</span>
                </div>
                <i class="ti ti-chevron-right text-secondary"></i>
            </a>
       
 <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/default.aspx">
     <div class="d-flex align-items-center gap-2">
         <i class="ti ti-news text-success"></i>
         <span class="fw-semibold text-success">Quản lý tin</span>
     </div>
     <i class="ti ti-chevron-right text-success"></i>
 </a>
           
           <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/edit-info.aspx">
                <div class="d-flex align-items-center gap-2">
                    <i class="ti ti-building-store text-secondary"></i>
                    <span class="fw-medium">Cửa hàng</span>
                </div>
                <span class="badge bg-white text-dark border rounded-pill">Tạo ngay</span>
            </a>
</div>
</div>--%>

    <asp:PlaceHolder ID="phMenuHomeExtra" runat="server" Visible="true">
        <!-- ================= ƯU ĐÃI ================= -->
        <div class="text-secondary small fw-semibold px-1 mb-2">Ưu đãi, khuyến mãi</div>
        <div class="card border-0 bg-light mb-3" style="border-radius: 14px;">
            <div class="p-3">
                <div class="d-flex flex-column gap-2">
                    <asp:PlaceHolder ID="phHoSoHomeMacDinh" runat="server" Visible="false">
                        <!-- 1) Hồ sơ quyền tiêu dùng -->
                        <a href="/home/lich-su-giao-dich.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-white border d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <img src="/uploads/images/dong-a.png" alt="Quyền tiêu dùng"
                                    style="width: 14px; height: 14px; object-fit: contain;">
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ quyền tiêu dùng
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["DongA"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>

                        <!-- 2) Hồ sơ quyền ưu đãi -->
                        <a href="/home/lich-su-quyen-uu-dai.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-success-lt text-success d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <i class="ti ti-ticket"></i>
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ quyền ưu đãi
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["HoSo_UuDai_Real"] ?? ViewState["DuVi1_Evocher_30PhanTram"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHoSoLaoDong" runat="server" Visible="false">
                        <!-- 3) Hồ sơ lao động -->
                        <a href="/home/lich-su-quyen-lao-dong.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-warning-lt text-warning d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <i class="ti ti-briefcase"></i>
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ hành vi lao động
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["HoSo_LaoDong_Real"] ?? ViewState["DuVi2_LaoDong_50PhanTram"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHoSoGanKet" runat="server" Visible="false">
                        <!-- 4) Hồ sơ gắn kết -->
                        <a href="/home/lich-su-quyen-gan-ket.aspx"
                            class="badge bg-white text-dark border w-100 text-start px-3 py-2"
                            style="border-radius: 999px; display: flex; align-items: center; gap: 10px;">
                            <span class="avatar avatar-xs rounded-circle bg-primary-lt text-primary d-flex align-items-center justify-content-center"
                                style="width: 22px; height: 22px;">
                                <i class="ti ti-heart-handshake"></i>
                            </span>

                            <span class="flex-fill" style="min-width: 0;">
                                <span class="d-block text-muted" style="font-size: 11px; line-height: 1.1; white-space: normal;">Hồ sơ chỉ số gắn kết
                                </span>
                                <span class="d-block fw-semibold" style="line-height: 1.2;">
                                    <%= ViewState["HoSo_GanKet_Real"] ?? ViewState["DuVi3_GanKet_20PhanTram"] %>
                                </span>
                            </span>

                            <i class="ti ti-chevron-right text-muted"></i>
                        </a>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>

        <!-- ================= KHÁC ================= -->
        <div class="text-secondary small fw-semibold px-1 mb-2">Khác</div>
        <div class="card border-0 bg-light" style="border-radius: 14px;">
            <div class="list-group list-group-flush" style="border-radius: 14px; overflow: hidden;">

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/edit-info.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-settings text-secondary"></i>
                        <span class="fw-medium">Cài đặt tài khoản</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="#">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-help-circle text-secondary"></i>
                        <span class="fw-medium">Trợ giúp</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

                <!-- ✅ NEW: Đóng góp ý kiến -->
                <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/dong-gop-y-kien.aspx">
                    <div class="d-flex align-items-center gap-2">
                        <i class="ti ti-message-report text-secondary"></i>
                        <span class="fw-medium">Đóng góp ý kiến</span>
                    </div>
                    <i class="ti ti-chevron-right text-secondary"></i>
                </a>

            </div>
        </div>
    </asp:PlaceHolder>
        </div>
        <asp:PlaceHolder ID="phAccountLogoutFooter" runat="server" Visible="false">
            <div class="account-logout-sticky">
                <asp:LinkButton
                    ID="but_dangxuat"
                    runat="server"
                    CssClass="account-logout-btn"
                    OnClick="dangxuat_Click">
                    <i class="ti ti-logout"></i>
                    <span>Đăng xuất</span>
                </asp:LinkButton>
            </div>
        </asp:PlaceHolder>
    </div>
</div>

<script>
    function showNotif() {
        var p = document.getElementById('notifPopup');
        if (!p) return;
        p.style.display = 'block';
    }

    function hideNotif() {
        var p = document.getElementById('notifPopup');
        if (!p) return;
        p.style.display = 'none';
    }

    document.addEventListener('click', function (e) {
        var popup = document.getElementById('notifPopup');
        var btn = document.getElementById('<%= but_show_form_thongbao.ClientID %>');

            if (!popup || !btn) return;
            if (!popup.contains(e.target) && !btn.contains(e.target)) {
                hideNotif();
            }
        });
</script>

<script>
    function ensureCenterNotice() {
        var existing = document.getElementById('ahaCenterNotice');
        if (existing) return existing;

        var wrap = document.createElement('div');
        wrap.id = 'ahaCenterNotice';
        wrap.style.cssText = 'position:fixed;inset:0;display:none;align-items:center;justify-content:center;z-index:99999;background:rgba(0,0,0,0.35);';

        var card = document.createElement('div');
        card.id = 'ahaCenterNoticeCard';
        card.style.cssText = 'min-width:240px;max-width:85vw;background:#fff;border-radius:14px;padding:16px 18px;box-shadow:0 12px 30px rgba(0,0,0,.2);text-align:center;font-weight:600;';

        wrap.appendChild(card);
        document.body.appendChild(wrap);
        return wrap;
    }

    function showCenterNotice(message, type) {
        var wrap = ensureCenterNotice();
        var card = document.getElementById('ahaCenterNoticeCard');
        if (!wrap || !card) return;

        card.style.color = (type === 'success') ? '#0f5132' : '#664d03';
        card.style.background = (type === 'success') ? '#d1e7dd' : '#fff3cd';
        card.style.border = (type === 'success') ? '1px solid #badbcc' : '1px solid #ffecb5';
        card.textContent = message || '';

        wrap.style.display = 'flex';
        clearTimeout(wrap._hideTimer);
        wrap._hideTimer = setTimeout(function () {
            wrap.style.display = 'none';
        }, 2200);
    }

    function copyReferralLink(el) {
        try {
            var link = (el && el.getAttribute('data-ref-link')) ? el.getAttribute('data-ref-link') : '';
            if (!link || link.slice(-2) === 'u=') {
                showCenterNotice('Không có link giới thiệu để sao chép.', 'warning');
                return false;
            }

            function showSuccess() {
                showCenterNotice('Đã sao chép link giới thiệu thành công.', 'success');
            }

            function showFail() {
                showCenterNotice('Không thể sao chép link. Vui lòng thử lại.', 'warning');
            }

            function fallbackCopy(text) {
                var ta = document.createElement('textarea');
                ta.value = text;
                ta.setAttribute('readonly', '');
                ta.style.position = 'fixed';
                ta.style.top = '-1000px';
                document.body.appendChild(ta);
                ta.select();
                try {
                    var ok = document.execCommand('copy');
                    if (ok) showSuccess();
                    else showFail();
                } catch (e) {
                    showFail();
                }
                document.body.removeChild(ta);
            }

            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(link).then(showSuccess).catch(function () {
                    fallbackCopy(link);
                });
            } else {
                fallbackCopy(link);
            }
        } catch (e) { }

        return false;
    }

    function bindCopyReferralHandler() {
        document.addEventListener('click', function (ev) {
            if (ev.defaultPrevented) return;
            var target = ev.target;
            if (!target) return;
            var el = target.closest('[data-copy-ref="1"]');
            if (!el) return;
            ev.preventDefault();
            copyReferralLink(el);
        });
    }

    document.addEventListener('DOMContentLoaded', bindCopyReferralHandler);
</script>

<script>
    function cloneAccountNode(source) {
        if (!source) return null;
        var node = source.cloneNode(true);
        node.removeAttribute('id');
        node.querySelectorAll('[id]').forEach(function (el) {
            el.removeAttribute('id');
        });
        node.querySelectorAll('script').forEach(function (el) {
            el.remove();
        });
        return node;
    }

    function mountAccountMenuOnce() {
        var tpl = document.getElementById('accountMenuContent');
        var hostDesktop = document.getElementById('accountMenuHostDesktop');
        if (!tpl || !hostDesktop) return;
        if (hostDesktop.children && hostDesktop.children.length > 0) return;
        hostDesktop.innerHTML = tpl.innerHTML;
        tpl.style.display = 'none';
    }

    function syncAccountMenuToMobile() {
        var hostDesktop = document.getElementById('accountMenuHostDesktop');
        var hostMobile = document.getElementById('accountMenuHostMobile');
        if (!hostDesktop || !hostMobile) return;
        var clone = cloneAccountNode(hostDesktop);
        if (!clone) return;
        hostMobile.innerHTML = clone.innerHTML;
    }

    function setupAccountDropdown() {
        var toggle = document.getElementById('userMenuToggle');
        var wrapper = toggle ? toggle.closest('.aha-account') : null;
        if (!toggle || !wrapper) return;
        if (toggle.getAttribute('data-dropdown-bound') === '1') return;
        toggle.setAttribute('data-dropdown-bound', '1');
        wrapper.classList.remove('is-open');
        toggle.setAttribute('aria-expanded', 'false');

        function closeMenu() {
            wrapper.classList.remove('is-open');
            toggle.setAttribute('aria-expanded', 'false');
        }

        function openMenu() {
            wrapper.classList.add('is-open');
            toggle.setAttribute('aria-expanded', 'true');
        }

        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            if (wrapper.classList.contains('is-open')) {
                closeMenu();
            } else {
                openMenu();
            }
        });

        document.addEventListener('click', function (e) {
            if (!wrapper.contains(e.target)) {
                closeMenu();
            }
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') closeMenu();
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        mountAccountMenuOnce();
        setupAccountDropdown();
    });

    if (window.Sys && window.Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            mountAccountMenuOnce();
            setupAccountDropdown();
        });
    }

    document.addEventListener('shown.bs.offcanvas', function (e) {
        if (e.target && e.target.id === 'accountMenuCanvas') {
            syncAccountMenuToMobile();
        }
    });
</script>

<script>
    function showFavorite() {
        var p = document.getElementById('favoritePopup');
        if (!p) return;
        p.style.display = 'block';
    }

    function hideFavorite() {
        var p = document.getElementById('favoritePopup');
        if (!p) return;
        p.style.display = 'none';
    }


</script>
