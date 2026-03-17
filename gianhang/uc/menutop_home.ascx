<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menutop_home.ascx.cs" Inherits="uc_menu_top" %>
<header class="storefront-header">
    <div class="storefront-header__bar container-fluid">
        <div class="container storefront-header__inner">
            <a href="<%=AhaShineHomeRoutes_cl.HomeUrl %>" class="storefront-brand">
                <span class="storefront-brand__logo<%=logo == "" ? " storefront-brand__logo--empty" : "" %>">
                    <% if (logo != "") { %>
                    <img src="<%=logo %>" alt="<%=brandName %>" onerror="this.style.display='none';" />
                    <% } else { %>
                    <span><%=brandName.Substring(0, 1).ToUpperInvariant() %></span>
                    <% } %>
                </span>
                <span class="storefront-brand__copy">
                    <span class="storefront-brand__title"><%=brandName %></span>
                    <span class="storefront-brand__note"><%=brandNote == "" ? "Storefront duoc dong bo tu admin" : brandNote %></span>
                </span>
            </a>

            <nav class="storefront-nav" aria-label="Dieu huong gian hang">
                <ul class="storefront-nav__list">
                    <li class="storefront-nav__item"><a class="storefront-nav__link" href="<%=AhaShineHomeRoutes_cl.HomeUrl %>"><%=NavHomeText %></a></li>
                    <%=DesktopMenuHtml %>
                    <li class="storefront-nav__item"><a class="storefront-nav__link" href="<%=AhaShineHomeRoutes_cl.BookingUrl %>"><%=NavBookingText %></a></li>
                </ul>
            </nav>

            <div class="storefront-header__actions">
                <% if (hotline != "") { %>
                <a class="storefront-pill storefront-pill--ghost d-none d-inline-flex-xl" href="tel:<%=hotline %>">
                    <span class="mif-phone"></span>
                    <span><%=hotline %></span>
                </a>
                <% } %>

                <a class="storefront-icon-button" href="<%=AhaShineHomeRoutes_cl.BookingUrl %>" title="<%=NavBookingText %>">
                    <span class="mif-calendar"></span>
                </a>

                <a class="storefront-icon-button storefront-icon-button--cart" href="<%=AhaShineHomeRoutes_cl.CartUrl %>" title="Gio hang">
                    <span class="mif-cart"></span>
                    <span class="storefront-icon-button__badge"><%=sl_hangtronggio %></span>
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
                        <span class="storefront-account__meta d-none d-inline-flex-xl">
                            <strong><%=hoten %></strong>
                            <small>E-AHA <%=sodiem_eaha %></small>
                        </span>
                    </summary>
                    <div class="storefront-account__menu">
                        <a href="<%=AhaShineHomeRoutes_cl.AccountUrl %>">Ho so tai khoan</a>
                        <a href="<%=AhaShineHomeRoutes_cl.ChangePasswordUrl %>">Doi mat khau</a>
                        <a id="linkDangXuatDesktop" runat="server" onserverclick="but_dangxuat_Click">Dang xuat</a>
                    </div>
                </details>
                <% } else { %>
                <div class="storefront-auth d-none d-inline-flex-xl">
                    <a class="storefront-pill storefront-pill--ghost" onclick="show_hide_id_form_dangnhap()">Dang nhap</a>
                    <a class="storefront-pill storefront-pill--primary" onclick="show_hide_id_form_dangky()">Dang ky</a>
                </div>
                <% } %>

                <button type="button" class="storefront-mobile-toggle" aria-expanded="false" aria-controls="storefront-mobile-drawer" data-storefront-mobile-toggle>
                    <span></span>
                    <span></span>
                    <span></span>
                </button>
            </div>
        </div>
    </div>

    <% if (ShowQuickstrip) { %>
    <div class="storefront-quickstrip">
        <div class="container storefront-quickstrip__inner">
            <% if (QuickServiceText != "") { %><a href="<%=ServicesUrl %>"><%=QuickServiceText %></a><% } %>
            <% if (QuickProductText != "") { %><a href="<%=ProductsUrl %>"><%=QuickProductText %></a><% } %>
            <% if (QuickArticleText != "") { %><a href="<%=ArticlesUrl %>"><%=QuickArticleText %></a><% } %>
            <% if (QuickConsultText != "") { %><a href="javascript:void(0)" onclick="show_hide_id_form_dangkytuvan()"><%=QuickConsultText %></a><% } %>
            <% if (QuickBookingText != "") { %><a href="<%=AhaShineHomeRoutes_cl.BookingUrl %>"><%=QuickBookingText %></a><% } %>
        </div>
    </div>
    <% } %>

    <aside id="storefront-mobile-drawer" class="storefront-mobile-drawer" aria-hidden="true">
        <div class="storefront-mobile-drawer__head">
            <div>
                <div class="storefront-mobile-drawer__title"><%=brandName %></div>
                <div class="storefront-mobile-drawer__note"><%=brandNote == "" ? "Storefront duoc dong bo tu admin" : brandNote %></div>
            </div>
            <button type="button" class="storefront-mobile-drawer__close" data-storefront-mobile-close>
                <span class="mif-cross"></span>
            </button>
        </div>

        <div class="storefront-mobile-drawer__actions">
            <a class="storefront-pill storefront-pill--primary" href="<%=AhaShineHomeRoutes_cl.BookingUrl %>"><%=NavBookingText %></a>
            <a class="storefront-pill storefront-pill--ghost" href="<%=AhaShineHomeRoutes_cl.CartUrl %>">Gio hang (<%=sl_hangtronggio %>)</a>
            <% if (hotline != "") { %>
            <a class="storefront-pill storefront-pill--ghost" href="tel:<%=hotline %>">Goi <%=hotline %></a>
            <% } %>
        </div>

        <% if (!IsSignedIn) { %>
        <div class="storefront-mobile-drawer__actions storefront-mobile-drawer__actions--auth">
            <a class="storefront-pill storefront-pill--ghost" onclick="show_hide_id_form_dangnhap()">Dang nhap</a>
            <a class="storefront-pill storefront-pill--primary" onclick="show_hide_id_form_dangky()">Dang ky</a>
        </div>
        <% } %>

        <ul class="storefront-mobile-nav__list">
            <li class="storefront-mobile-nav__item storefront-mobile-nav__item--level-0"><a class="storefront-mobile-nav__link" href="<%=AhaShineHomeRoutes_cl.HomeUrl %>"><%=NavHomeText %></a></li>
            <% if (QuickServiceText != "") { %><li class="storefront-mobile-nav__item storefront-mobile-nav__item--level-0"><a class="storefront-mobile-nav__link" href="<%=ServicesUrl %>"><%=QuickServiceText %></a></li><% } %>
            <% if (QuickProductText != "") { %><li class="storefront-mobile-nav__item storefront-mobile-nav__item--level-0"><a class="storefront-mobile-nav__link" href="<%=ProductsUrl %>"><%=QuickProductText %></a></li><% } %>
            <% if (QuickArticleText != "") { %><li class="storefront-mobile-nav__item storefront-mobile-nav__item--level-0"><a class="storefront-mobile-nav__link" href="<%=ArticlesUrl %>"><%=QuickArticleText %></a></li><% } %>
            <%=MobileMenuHtml %>
        </ul>

        <% if (IsSignedIn) { %>
        <div class="storefront-mobile-drawer__account">
            <a href="<%=AhaShineHomeRoutes_cl.AccountUrl %>">Ho so tai khoan</a>
            <a href="<%=AhaShineHomeRoutes_cl.ChangePasswordUrl %>">Doi mat khau</a>
            <a id="linkDangXuatMobile" runat="server" onserverclick="but_dangxuat_Click">Dang xuat</a>
        </div>
        <% } %>
    </aside>
    <div class="storefront-mobile-backdrop" data-storefront-mobile-close></div>
</header>
