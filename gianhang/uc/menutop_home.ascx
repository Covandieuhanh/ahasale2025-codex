<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menutop_home.ascx.cs" Inherits="uc_menu_top" %>
<header class="storefront-header">
    <div class="storefront-header__bar container-fluid">
        <div class="container storefront-header__inner">
            <button type="button" class="storefront-mobile-toggle" data-storefront-mobile-toggle aria-label="Mở menu gian hàng" aria-expanded="false">
                <span></span>
                <span></span>
                <span></span>
            </button>

            <a href="<%=HomeUrl %>" class="storefront-brand">
                <span class="storefront-brand__logo<%=logo == "" ? " storefront-brand__logo--empty" : "" %>">
                    <% if (logo != "") { %>
                    <img src="<%=logo %>" alt="<%=brandName %>" onerror="this.style.display='none';" />
                    <% } else { %>
                    <span><%=brandName.Substring(0, 1).ToUpperInvariant() %></span>
                    <% } %>
                </span>
                <span class="storefront-brand__copy">
                    <span class="storefront-brand__title"><%=brandName %></span>
                </span>
            </a>

            <div class="storefront-header__actions">
                <% if (hotline != "") { %>
                <a class="storefront-pill storefront-pill--ghost d-none d-inline-flex-xl storefront-header__action-desktop" href="tel:<%=hotline %>">
                    <span class="mif-phone"></span>
                    <span><%=hotline %></span>
                </a>
                <% } %>

                <a class="storefront-icon-button storefront-header__action-desktop" href="<%=BookingUrl %>" title="<%=NavBookingText %>">
                    <span class="mif-calendar"></span>
                </a>

                <a class="storefront-icon-button storefront-icon-button--cart storefront-header__action-desktop" href="<%=CartUrl %>" title="Giỏ hàng">
                    <span class="mif-cart"></span>
                    <span class="storefront-icon-button__badge"><%=sl_hangtronggio %></span>
                </a>

                <a class="storefront-icon-button storefront-header__action-mobile" href="<%=BookingUrl %>" title="<%=NavBookingText %>">
                    <span class="mif-bell"></span>
                </a>

                <% if (IsSignedIn) { %>
                <details class="storefront-account">
                    <summary class="storefront-account__summary">
                        <span class="storefront-account__avatar<%=avt == "" ? " storefront-account__avatar--empty" : "" %>">
                            <% if (avt != "") { %>
                            <img src="<%=avt %>" alt="<%=hoten %>" />
                            <% } else { %>
                            <span><%=hoten == "" ? "U" : hoten.Substring(0, 1).ToUpperInvariant() %></span>
                            <% } %>
                        </span>
                        <span class="storefront-account__meta">
                            <strong>Tài khoản <%=accountLogin %></strong>
                            <small><%=hoten %></small>
                        </span>
                    </summary>
                    <div class="storefront-account__menu">
                        <div class="storefront-account__profile">
                            <span class="storefront-account__profile-avatar<%=avt == "" ? " storefront-account__profile-avatar--empty" : "" %>">
                                <% if (avt != "") { %>
                                <img src="<%=avt %>" alt="<%=hoten %>" />
                                <% } else { %>
                                <span><%=hoten == "" ? "U" : hoten.Substring(0, 1).ToUpperInvariant() %></span>
                                <% } %>
                            </span>
                            <div class="storefront-account__profile-copy">
                                <strong><%=hoten %></strong>
                                <small><%=accountLogin %></small>
                                <span class="storefront-account__profile-status"><%=spaceStatus %></span>
                            </div>
                            <a class="storefront-account__profile-edit" href="<%=AhaShineHomeRoutes_cl.AccountUrl %>" title="Hồ sơ">
                                <span class="mif-pencil"></span>
                            </a>
                        </div>

                        <div class="storefront-account__space-list">
                            <a class="<%=GetSpaceLinkCss("public") %>" href="<%=spacePublicUrl %>">Trang công khai</a>
                            <a class="<%=GetSpaceLinkCss("manage") %>" href="<%=spaceManageUrl %>">Quản lý tin</a>
                            <a class="<%=GetSpaceLinkCss("orders") %>" href="<%=spaceOrdersUrl %>">Đơn bán</a>
                            <a class="<%=GetSpaceLinkCss("booking") %>" href="<%=spaceBookingUrl %>">Lịch hẹn</a>
                            <a class="<%=GetSpaceLinkCss("customers") %>" href="<%=spaceCustomersUrl %>">Khách hàng</a>
                            <a class="<%=GetSpaceLinkCss("report") %>" href="<%=spaceReportUrl %>">Báo cáo</a>
                            <a class="<%=GetSpaceLinkCss("level2") %>" href="<%=spaceLevel2Url %>">Nâng cấp quản trị</a>
                            <% if (showSpaceAdmin) { %>
                            <a class="<%=GetSpaceLinkCss("admin") %>" href="<%=spaceAdminUrl %>">Quản trị nâng cao</a>
                            <% } %>
                            <a class="storefront-account-space__link" href="<%=spaceHomeUrl %>">Trung tâm tài khoản</a>
                            <a class="storefront-account-space__link" href="<%=AhaShineHomeRoutes_cl.ChangePasswordUrl %>">Đổi mật khẩu</a>
                        </div>

                        <div class="storefront-account__footer">
                            <a id="linkDangXuatDesktop" runat="server" class="storefront-account__logout" onserverclick="but_dangxuat_Click">
                                <span class="mif-exit"></span>
                                <span>Đăng xuất</span>
                            </a>
                        </div>
                    </div>
                </details>
                <% } else { %>
                <div class="storefront-auth">
                    <a class="storefront-pill storefront-pill--ghost" onclick="show_hide_id_form_dangnhap()">Đăng nhập</a>
                    <a class="storefront-pill storefront-pill--primary" onclick="show_hide_id_form_dangky()">Đăng ký</a>
                </div>
                <% } %>
            </div>
        </div>
    </div>
</header>
<div class="storefront-mobile-backdrop" data-storefront-mobile-close></div>
<aside class="storefront-mobile-drawer" id="storefront-mobile-drawer" aria-hidden="true">
    <div class="storefront-mobile-drawer__head">
        <div>
            <div class="storefront-mobile-drawer__title"><%=brandName %></div>
            <div class="storefront-mobile-drawer__note">Đi nhanh vào các khu chính của gian hàng công khai.</div>
        </div>
        <button type="button" class="storefront-mobile-drawer__close" data-storefront-mobile-close aria-label="Đóng menu">
            <span class="mif-cross"></span>
        </button>
    </div>

    <div class="storefront-mobile-drawer__actions">
        <a class="storefront-pill storefront-pill--primary" href="<%=HomeUrl %>">Trang công khai</a>
        <a class="storefront-pill storefront-pill--ghost" href="<%=BookingUrl %>"><%=NavBookingText %></a>
        <a class="storefront-pill storefront-pill--ghost" href="<%=CartUrl %>">Giỏ hàng</a>
    </div>

    <ul class="storefront-mobile-nav__list">
        <li class="storefront-mobile-nav__item"><a class="storefront-mobile-nav__link" href="<%=HomeUrl %>">Trang công khai</a></li>
        <li class="storefront-mobile-nav__item"><a class="storefront-mobile-nav__link" href="<%=ProductsUrl %>">Sản phẩm</a></li>
        <li class="storefront-mobile-nav__item"><a class="storefront-mobile-nav__link" href="<%=ServicesUrl %>">Dịch vụ</a></li>
        <li class="storefront-mobile-nav__item"><a class="storefront-mobile-nav__link" href="<%=ArticlesUrl %>">Bài viết</a></li>
        <li class="storefront-mobile-nav__item"><a class="storefront-mobile-nav__link" href="<%=spaceManageUrl %>">Quản lý tin</a></li>
        <li class="storefront-mobile-nav__item"><a class="storefront-mobile-nav__link" href="<%=spaceOrdersUrl %>">Đơn bán</a></li>
    </ul>

    <div class="storefront-mobile-drawer__account">
        <% if (IsSignedIn) { %>
        <a href="<%=spaceHomeUrl %>">Trung tâm tài khoản</a>
        <a href="<%=AhaShineHomeRoutes_cl.AccountUrl %>">Hồ sơ tài khoản</a>
        <% } else { %>
        <a href="javascript:void(0);" onclick="show_hide_id_form_dangnhap()">Đăng nhập tài khoản</a>
        <a href="javascript:void(0);" onclick="show_hide_id_form_dangky()">Đăng ký tài khoản</a>
        <% } %>
    </div>
</aside>
