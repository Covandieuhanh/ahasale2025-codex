<%@ Page Title="" Language="C#" MasterPageFile="~/gianhang/mp-home.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="gianhang_Default" %>

<%@ Register Src="~/gianhang/uc/slider.ascx" TagPrefix="uc1" TagName="slider" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta property="og:type" content="website" />
    <title><%=title_web %></title>
    <asp:PlaceHolder runat="server"><%=meta %></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="home-page">
        <div class="container">
            <section class="storefront-hero home-fade-up">
                <div class="storefront-hero__copy">
                    <span class="storefront-eyebrow"><%=HeroEyebrow %></span>
                    <h1 class="storefront-hero__title"><%=HeroTitle %></h1>
                    <p class="storefront-hero__desc"><%=HeroDescription %></p>

                    <div class="storefront-hero__actions">
                        <% if (HeroPrimaryText != "") { %>
                        <a class="storefront-hero__action storefront-hero__action--primary" href="<%=HeroPrimaryUrl %>"><%=HeroPrimaryText %></a>
                        <% } %>
                        <% if (HeroSecondaryText != "") { %>
                        <a class="storefront-hero__action storefront-hero__action--ghost" href="<%=HeroSecondaryUrl %>"><%=HeroSecondaryText %></a>
                        <% } %>
                        <% if (HeroTertiaryText != "") { %>
                        <a class="storefront-hero__action storefront-hero__action--ghost" href="<%=HeroTertiaryUrl %>"><%=HeroTertiaryText %></a>
                        <% } %>
                    </div>

                    <div class="storefront-hero__metrics">
                        <article class="storefront-metric">
                            <strong><%=ServiceCount.ToString("#,##0") %></strong>
                            <span><%=MetricServiceText %></span>
                        </article>
                        <article class="storefront-metric">
                            <strong><%=ProductCount.ToString("#,##0") %></strong>
                            <span><%=MetricProductText %></span>
                        </article>
                        <article class="storefront-metric">
                            <strong><%=ArticleCount.ToString("#,##0") %></strong>
                            <span><%=MetricArticleText %></span>
                        </article>
                    </div>
                </div>

                <div class="storefront-hero__stage">
                    <div class="storefront-stage-card">
                        <article class="storefront-stage-chip">
                            <strong><%=StagePrimaryTitle %></strong>
                            <span><%=StagePrimaryDescription %></span>
                        </article>
                        <article class="storefront-stage-chip">
                            <strong><%=StageSecondaryTitle %></strong>
                            <span><%=StageSecondaryDescription %></span>
                        </article>
                    </div>
                    <uc1:slider runat="server" ID="slider" />
                </div>
            </section>

            <asp:PlaceHolder ID="plHomeSections" runat="server" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
