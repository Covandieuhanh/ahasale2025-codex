<%@ Page Title="Báº¥t Ä‘á»™ng sáº£n AhaSale" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="bat_dong_san_default" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <link rel="stylesheet" type="text/css" href="<%= Helper_cl.VersionedUrl("~/bat-dong-san/bds-home.css") %>" />
    <script src="<%= Helper_cl.VersionedUrl("~/bat-dong-san/bds-home.js") %>" defer></script>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-body bds-page">
        <div class="container-xl d-flex flex-column gap-3">
            <uc:BdsNav ID="BdsBannerMain" runat="server" Current="overview" ShowTopbar="false" IncludeAssets="false" ShowBanner="true" ShowNav="false" />

            <section class="bds-landing-hero">
                <div class="bds-page-title-block">
                    <h1 class="bds-page-title-main">Mua bÃ¡n, cho thuÃª báº¥t Ä‘á»™ng sáº£n</h1>
                    <div class="page-pretitle">AhaSale / Báº¥t Ä‘á»™ng sáº£n</div>
                </div>

                <div id="bdsSearchWrapSpacer" class="bds-search-wrap-spacer"></div>
                <section id="bdsSearchWrap" class="bds-search-wrap">
                    <div class="bds-search-shell">
                        <div class="bds-search-shell-inner">
                            <asp:HiddenField ID="hfSearchMode" runat="server" Value="sale" ClientIDMode="Static" />
                            <div class="bds-search-bar">
                                <asp:TextBox ID="txtKeyword" runat="server" ClientIDMode="Static" CssClass="form-control bds-search-input" placeholder="VD: BÃ¡n nhÃ  BÃ¬nh Tháº¡nh dÆ°á»›i 5 tá»·" />
                                <div class="bds-search-mode-wrap">
                                    <span class="bds-search-mode-prefix">Loáº¡i hÃ¬nh BÄS</span>
                                    <select id="bdsSearchModeSelect" class="form-select bds-search-mode-select" aria-label="Loáº¡i hÃ¬nh báº¥t Ä‘á»™ng sáº£n">
                                        <option value="sale" selected="selected">Mua bÃ¡n</option>
                                        <option value="rent">Cho thuÃª</option>
                                        <option value="project">Dá»± Ã¡n</option>
                                    </select>
                                </div>
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-success bds-search-submit" OnClick="btnSearch_Click"><span class="bds-search-submit-text">TÃ¬m kiáº¿m</span></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </section>
            </section>

            <section id="regionOverviewBlock" class="region-overview">
                <div class="region-overview-head">
                    <div>
                        <div class="region-overview-kicker">KhÃ¡m phÃ¡ nhanh</div>
                        <h3 class="region-overview-title">Báº¥t Ä‘á»™ng sáº£n theo khu vá»±c</h3>
                    </div>
                    <div class="region-overview-note">Chá»n khu vá»±c Ä‘á»ƒ má»Ÿ listing tÆ°Æ¡ng á»©ng</div>
                </div>

                <div class="region-type-tabs">
                    <button type="button" class="region-type-btn active" data-type-tab="all">Táº¥t cáº£</button>
                    <button type="button" class="region-type-btn" data-type-tab="apartment">CÄƒn há»™/Chung cÆ°</button>
                    <button type="button" class="region-type-btn" data-type-tab="house">NhÃ  á»Ÿ</button>
                    <button type="button" class="region-type-btn" data-type-tab="business">VÄƒn phÃ²ng, Máº·t báº±ng kinh doanh</button>
                    <button type="button" class="region-type-btn" data-type-tab="land">Äáº¥t</button>
                </div>

                <div class="region-grid">
                    <asp:Repeater ID="rptRegionOverview" runat="server">
                        <ItemTemplate>
                            <a
                                class="<%# BuildRegionCardCss(Container.DataItem) %>"
                                data-region-card="1"
                                data-province="<%# Eval("ProvinceKey") %>"
                                data-total-all="<%# Convert.ToInt32(Eval("SaleAll")) + Convert.ToInt32(Eval("RentAll")) %>"
                                data-total-apartment="<%# Convert.ToInt32(Eval("SaleApartment")) + Convert.ToInt32(Eval("RentApartment")) %>"
                                data-total-house="<%# Convert.ToInt32(Eval("SaleHouse")) + Convert.ToInt32(Eval("RentHouse")) %>"
                                data-total-business="<%# Convert.ToInt32(Eval("SaleBusiness")) + Convert.ToInt32(Eval("RentBusiness")) %>"
                                data-total-land="<%# Convert.ToInt32(Eval("SaleLand")) + Convert.ToInt32(Eval("RentLand")) %>"
                                data-sale-all="<%# Eval("SaleAll") %>"
                                data-sale-apartment="<%# Eval("SaleApartment") %>"
                                data-sale-house="<%# Eval("SaleHouse") %>"
                                data-sale-business="<%# Eval("SaleBusiness") %>"
                                data-sale-land="<%# Eval("SaleLand") %>"
                                data-rent-all="<%# Eval("RentAll") %>"
                                data-rent-apartment="<%# Eval("RentApartment") %>"
                                data-rent-house="<%# Eval("RentHouse") %>"
                                data-rent-business="<%# Eval("RentBusiness") %>"
                                data-rent-land="<%# Eval("RentLand") %>"
                                style="<%# BuildRegionCardStyle(Container.DataItem) %>"
                                href="/bat-dong-san/mua-ban.aspx?province=<%# Eval("ProvinceKey") %>">
                                <div>
                                    <div class="region-card-name"><%# Eval("ProvinceName") %></div>
                                    <div class="region-card-count"><span data-region-count="1"><%# GetRegionDisplayCount(Container.DataItem) %></span> <span class="region-card-count-label">tin Ä‘Äƒng</span></div>
                                </div>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>

                <section class="card bds-section-card">
                    <div class="card-header d-flex align-items-center bds-section-head">
                        <div>
                            <div class="bds-section-kicker">Nguá»“n ná»™i bá»™</div>
                            <h3 class="card-title">Tin má»›i tá»« AhaLand</h3>
                        </div>
                    </div>
                    <div class="card-body">
                    <div class="row row-cards">
                        <asp:Repeater ID="rptFeaturedListings" runat="server">
                            <ItemTemplate>
                                <div class="col-12 col-sm-6 col-lg-4 bds-col-preview">
                                    <article class="card bds-listing-card">
                                        <div class="card-body d-flex flex-column gap-3">
                                            <a class="bds-listing-thumb" href="<%# BuildListingUrl(Container.DataItem) %>">
                                                <img loading="lazy" decoding="async" src='<%# Eval("ThumbnailUrl") %>' alt='<%# Eval("Title") %>' data-fallback="/uploads/images/bat-dong-san/bds-placeholder.jpg" onerror="this.onerror=null;this.src='/uploads/images/bat-dong-san/bds-placeholder.jpg';" />
                                            </a>
                                            <div class="d-flex flex-wrap gap-1">
                                                <span class="badge bg-azure-lt text-azure"><%# Eval("PropertyTypeLabel") %></span>
                                            </div>
                                            <a class="bds-card-title text-reset text-decoration-none" href="<%# BuildListingUrl(Container.DataItem) %>"><%# Eval("Title") %></a>
                                            <div class="bds-price"><%# Eval("PriceText") %></div>
                                            <div class="bds-meta-line"><%# BuildListingMeta(Container.DataItem) %></div>
                                            <div class="bds-meta-line"><%# Eval("District") %>, <%# Eval("Province") %></div>
                                            <div class="d-flex justify-content-between align-items-center mt-auto pt-1">
                                                <div class="text-muted small"><%# Eval("PostedAgoText") %></div>
                                                <a class="btn btn-sm bds-card-cta" href="<%# BuildListingUrl(Container.DataItem) %>">Chi tiáº¿t</a>
                                            </div>
                                        </div>
                                    </article>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </section>

            <asp:PlaceHolder ID="phLinked" runat="server" Visible="false">
                <section class="card bds-section-card">
                    <div class="card-header d-flex align-items-center bds-section-head">
                        <div>
                            <div class="bds-section-kicker">Bá»• sung nguá»“n ngoÃ i</div>
                            <h3 class="card-title">Tin liÃªn káº¿t</h3>
                        </div>
                        <div class="card-actions">
                            <span class="badge bg-azure-lt text-azure">Chá»‰ xem táº¡i nguá»“n</span>
                        </div>
                    </div>
                    <div class="card-body">
                        <asp:PlaceHolder ID="phLinkedDebug" runat="server" Visible="false">
                            <div class="alert alert-warning py-2 px-3 mb-3 small"><%= LinkedDebugInfo %></div>
                        </asp:PlaceHolder>
                        <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
                            <div class="text-muted small">Tá»•ng tin liÃªn káº¿t: <strong><%= LinkedTotalItems.ToString("#,##0") %></strong></div>
                            <% if (HasLinkedPager()) { %>
                                <div class="text-muted small">Trang <strong><%= LinkedCurrentPage %></strong>/<strong><%= LinkedTotalPages %></strong></div>
                            <% } %>
                        </div>
                        <div class="row row-cards">
                            <asp:Repeater ID="rptLinked" runat="server">
                                <ItemTemplate>
                                    <div class="col-12 col-sm-6 col-lg-4 bds-col-preview">
                                        <article class="card bds-link-card h-100">
                                            <div class="card-body d-flex flex-column gap-2">
                                                <a class="bds-listing-thumb" href="/bat-dong-san/chi-tiet.aspx?linkedId=<%# Eval("Id") %>">
                                                    <img data-role="linked-thumb"
                                                        data-gallery='<%# BuildLinkedGalleryData(Container.DataItem) %>'
                                                        data-fallback="/uploads/images/bat-dong-san/bds-placeholder.jpg"
                                                        loading="lazy"
                                                        decoding="async"
                                                        src='<%# ResolveLinkedThumb(Container.DataItem) %>'
                                                        alt='<%# Eval("Title") %>'
                                                        onerror="this.onerror=null;this.src='/uploads/images/bat-dong-san/bds-placeholder.jpg';" />
                                                </a>
                                                <div class="d-flex flex-wrap gap-1">
                                                    <span class="badge bg-gray-lt text-gray"><%# RenderLinkedSourceLabel(Eval("Source")) %></span>
                                                    <span class='badge <%# string.Equals((Eval("Purpose") ?? "").ToString(), "rent", StringComparison.OrdinalIgnoreCase) ? "bg-azure-lt text-azure" : "bg-green-lt text-green" %>'>
                                                        <%# string.Equals((Eval("Purpose") ?? "").ToString(), "rent", StringComparison.OrdinalIgnoreCase) ? "Cho thuÃª" : "Mua bÃ¡n" %>
                                                    </span>
                                                </div>
                                                <a class="bds-linked-title text-reset text-decoration-none" href="/bat-dong-san/chi-tiet.aspx?linkedId=<%# Eval("Id") %>"><%# Eval("Title") %></a>
                                                <div class="text-muted small"><%# Eval("PublishedAt","{0:dd/MM HH:mm}") %></div>
                                                <div class="bds-linked-price"><%# Eval("PriceText") %></div>
                                                <div class="text-muted small"><%# Eval("AreaText") %></div>
                                                <div class="text-muted small"><%# BuildLinkedLocation(Eval("District"), Eval("Province")) %></div>
                                                <div class="bds-linked-summary small mb-0"><%# RenderLinkedSummary(Eval("Summary")) %></div>
                                                <a class="btn btn-sm bds-card-cta mt-auto" href="/bat-dong-san/chi-tiet.aspx?linkedId=<%# Eval("Id") %>">Xem</a>
                                            </div>
                                        </article>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <% if (HasLinkedPager()) { %>
                            <div class="d-flex justify-content-center mt-3">
                                <div class="bds-pager">
                                    <% foreach (int p in GetLinkedPageNumbers()) { %>
                                        <a class="bds-page-btn <%= p == LinkedCurrentPage ? "active" : "" %>" href="<%= BuildLinkedPageUrl(p) %>"><%= p %></a>
                                    <% } %>
                                    <a class="bds-page-btn <%= LinkedCurrentPage >= LinkedTotalPages ? "disabled" : "" %>" href="<%= BuildLinkedPageUrl(LinkedCurrentPage + 1) %>">Sau Â»</a>
                                </div>
                            </div>
                        <% } %>
                    </div>
                </section>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
