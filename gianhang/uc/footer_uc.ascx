<%@ Control Language="C#" AutoEventWireup="true" CodeFile="footer_uc.ascx.cs" Inherits="uc_footer_uc" %>
<footer class="storefront-footer" id="storefront-footer">
    <div class="container storefront-footer__inner">
        <div class="storefront-footer__grid">
            <section class="storefront-footer__brand">
                <a href="<%=HomeUrl %>" class="storefront-footer__logo<%=logo == "" ? " storefront-footer__logo--empty" : "" %>">
                    <% if (logo != "") { %>
                    <img src="<%=logo %>" alt="<%=tencongty %>" onerror="this.style.display='none';" />
                    <% } else { %>
                    <span><%=(tencongty == "" ? "G" : tencongty.Substring(0, 1).ToUpperInvariant()) %></span>
                    <% } %>
                </a>
                <div class="storefront-footer__title"><%=tencongty == "" ? "Gian hàng đối tác" : tencongty %></div>
                <div class="storefront-footer__desc"><%=FooterDescription %></div>

                <div class="storefront-footer__chips">
                    <% if (FooterChip1 != "") { %><span><%=FooterChip1 %></span><% } %>
                    <% if (FooterChip2 != "") { %><span><%=FooterChip2 %></span><% } %>
                    <% if (FooterChip3 != "") { %><span><%=FooterChip3 %></span><% } %>
                    <% if (FooterChip4 != "") { %><span><%=FooterChip4 %></span><% } %>
                </div>
            </section>

            <section class="storefront-footer__nav">
                <div class="storefront-footer__heading"><%=FooterNavTitle %></div>
                <div class="storefront-footer__links">
                    <a href="<%=HomeUrl %>"><%=NavHomeText %></a>
                    <a href="<%=ServicesUrl %>"><%=QuickServiceText %></a>
                    <a href="<%=ProductsUrl %>"><%=QuickProductText %></a>
                    <a href="<%=ArticlesUrl %>"><%=QuickArticleText %></a>
                    <a href="<%=BookingUrl %>"><%=NavBookingText %></a>
                    <a href="<%=CartUrl %>">Giỏ hàng</a>
                </div>
            </section>

            <section class="storefront-footer__nav">
                <div class="storefront-footer__heading"><%=FooterCategoryTitle %></div>
                <div class="storefront-footer__links">
                    <asp:Repeater ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <a href="<%# ResolveMenuUrl(Eval("id"), Eval("phanloai"), Eval("url_other")) %>"><%#Eval("name") %></a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>

            <section class="storefront-footer__contact">
                <div class="storefront-footer__heading"><%=FooterContactTitle %></div>
                <% if (diachi != "") { %>
                <div class="storefront-footer__contact-item"><span class="mif-location"></span><span><%=diachi %></span></div>
                <% } %>
                <% if (hotline != "") { %>
                <div class="storefront-footer__contact-item"><span class="mif-phone"></span><a href="tel:<%=hotline %>"><%=hotline %></a></div>
                <% } %>
                <% if (email != "") { %>
                <div class="storefront-footer__contact-item"><span class="mif-mail"></span><a href="mailto:<%=email %>"><%=email %></a></div>
                <% } %>
                <div class="storefront-footer__social">
                    <% if (fb != "") { %><a href="<%=fb %>" target="_blank" rel="noopener">Facebook</a><% } %>
                    <% if (zalo != "") { %><a href="<%=zalo %>" target="_blank" rel="noopener">Zalo</a><% } %>
                    <% if (ytb != "") { %><a href="<%=ytb %>" target="_blank" rel="noopener">Youtube</a><% } %>
                    <% if (ins != "") { %><a href="<%=ins %>" target="_blank" rel="noopener">Instagram</a><% } %>
                    <% if (tiktok != "") { %><a href="<%=tiktok %>" target="_blank" rel="noopener">TikTok</a><% } %>
                    <% if (linkedin != "") { %><a href="<%=linkedin %>" target="_blank" rel="noopener">LinkedIn</a><% } %>
                    <% if (whatsapp != "") { %><a href="<%=whatsapp %>" target="_blank" rel="noopener">WhatsApp</a><% } %>
                    <% if (twitter != "") { %><a href="<%=twitter %>" target="_blank" rel="noopener">Twitter</a><% } %>
                    <% if (wechat != "") { %><a href="<%=wechat %>" target="_blank" rel="noopener">WeChat</a><% } %>
                </div>
            </section>
        </div>

        <div class="storefront-footer__bottom">
            <div>Copyright © <%=CurrentYear %> <%=tencongty == "" ? HttpContext.Current.Request.Url.Host : tencongty %>.</div>
            <div class="storefront-footer__bottom-links">
                <% if (FooterBottomPrimaryText != "") { %><a href="<%=FooterBottomPrimaryUrl %>" <%=FooterBottomPrimaryAttr %>><%=FooterBottomPrimaryText %></a><% } %>
                <% if (FooterBottomSecondaryText != "") { %><a href="<%=FooterBottomSecondaryUrl %>"><%=FooterBottomSecondaryText %></a><% } %>
            </div>
        </div>
    </div>
</footer>
