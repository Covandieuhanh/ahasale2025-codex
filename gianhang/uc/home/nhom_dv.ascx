<%@ Control Language="C#" AutoEventWireup="true" CodeFile="nhom_dv.ascx.cs" Inherits="uc_home_nhom_dv" %>
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
        <div class="feature-track feature-track--grid">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <article class="feature-card feature-card--group">
                        <a class="feature-card__media" href="<%# ResolveGroupUrl(Eval("id"), Eval("url_other")) %>" data-fallback="Hinh anh dang cap nhat">
                            <img src="<%#Eval("image") %>" alt="<%#Eval("name") %>" onerror="this.onerror=null; this.style.display='none'; this.parentNode.className += ' feature-card__media--missing'; this.parentNode.setAttribute('data-fallback', 'Hinh anh dang cap nhat');" />
                        </a>
                        <div class="feature-card__body feature-card__body--group">
                            <span class="feature-card__eyebrow"><%=ItemLabel %></span>
                            <h3 class="feature-card__title ellipsis-2">
                                <a href="<%# ResolveGroupUrl(Eval("id"), Eval("url_other")) %>"><%#Eval("name") %></a>
                            </h3>
                            <div class="feature-card__desc"><%#Eval("description") %></div>
                            <div class="feature-card__actions">
                                <% if (SecondaryButtonText != "") { %>
                                <a class="feature-card__button feature-card__button--ghost" href="<%=BookingUrl %>"><%=SecondaryButtonText %></a>
                                <% } %>
                                <% if (PrimaryButtonText != "") { %>
                                <a class="feature-card__button feature-card__button--primary" href="<%# ResolveGroupUrl(Eval("id"), Eval("url_other")) %>"><%=PrimaryButtonText %></a>
                                <% } %>
                            </div>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</section>
