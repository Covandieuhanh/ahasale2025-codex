<%@ Control Language="C#" AutoEventWireup="true" CodeFile="baiviet_new.ascx.cs" Inherits="uc_home_baiviet_new" %>
<section class="home-module">
    <div class="home-panel">
        <div class="home-section-head">
            <div class="home-section-head__copy">
                <span class="home-section-label"><%=SectionLabel %></span>
                <h2 class="home-section-title"><%=SectionTitle %></h2>
                <div class="home-section-desc"><%=SectionDescription %></div>
            </div>
            <div class="home-section-chip"><%=SectionBadgeText %></div>
        </div>

        <div class="article-layout">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <article class="article-lead">
                        <a class="article-lead__media" href="/gianhang/page/chi-tiet-bai-viet.aspx?idbv=<%#Eval("id") %>" data-fallback="Hinh anh dang cap nhat">
                            <img src="<%#Eval("image") %>" alt="<%#Eval("name") %>" onerror="this.onerror=null; this.style.display='none'; this.parentNode.className += ' article-lead__media--missing'; this.parentNode.setAttribute('data-fallback', 'Hinh anh dang cap nhat');" />
                        </a>
                        <div class="article-lead__body">
                            <span class="article-lead__meta"><%=ItemLabel %></span>
                            <h3 class="article-lead__title ellipsis-2">
                                <a href="/gianhang/page/chi-tiet-bai-viet.aspx?idbv=<%#Eval("id") %>"><%#Eval("name") %></a>
                            </h3>
                            <div class="article-lead__desc"><%#Eval("description") %></div>
                            <div class="service-card__actions">
                                <% if (PrimaryButtonText != "") { %>
                                <a class="service-card__button service-card__button--primary" href="/gianhang/page/chi-tiet-bai-viet.aspx?idbv=<%#Eval("id") %>"><%=PrimaryButtonText %></a>
                                <% } %>
                            </div>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>

            <div class="article-side-list">
                <asp:Repeater ID="Repeater2" runat="server">
                    <ItemTemplate>
                        <article class="article-mini">
                            <a class="article-mini__media" href="/gianhang/page/chi-tiet-bai-viet.aspx?idbv=<%#Eval("id") %>" data-fallback="Hinh anh dang cap nhat">
                                <img src="<%#Eval("image") %>" alt="<%#Eval("name") %>" onerror="this.onerror=null; this.style.display='none'; this.parentNode.className += ' article-mini__media--missing'; this.parentNode.setAttribute('data-fallback', 'Hinh anh dang cap nhat');" />
                            </a>
                            <div class="article-mini__body">
                                <span class="article-mini__tag"><%=ItemLabel %></span>
                                <h3 class="article-mini__title ellipsis-2">
                                    <a href="/gianhang/page/chi-tiet-bai-viet.aspx?idbv=<%#Eval("id") %>"><%#Eval("name") %></a>
                                </h3>
                                <div class="article-mini__desc ellipsis-3"><%#Eval("description") %></div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</section>
