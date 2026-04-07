<%@ Page Title="Cho thuÃª báº¥t Ä‘á»™ng sáº£n" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="cho-thue.aspx.cs" Inherits="bat_dong_san_cho_thue" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-body bds-rent-page">
        <div class="container-xl d-flex flex-column gap-3">
            <section class="bds-page-topbar">
                <div>
                    <div class="bds-page-kicker">AhaSale / Báº¥t Ä‘á»™ng sáº£n / Cho thuÃª</div>
                    <h1 class="bds-page-heading">Cho thuÃª báº¥t Ä‘á»™ng sáº£n</h1>
                </div>
            </section>

            <section class="bds-results-search sticky-top">
                <div class="bds-results-search-bar">
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control bds-results-search-input" placeholder="VD: ThuÃª nhÃ  BiÃªn HÃ²a dÆ°á»›i 10 triá»‡u" />
                    <asp:LinkButton ID="btnFilter" runat="server" CssClass="btn bds-results-search-submit" OnClick="btnFilter_Click">TÃ¬m kiáº¿m</asp:LinkButton>
                    <button type="button" class="btn bds-results-filter-toggle" data-bs-toggle="offcanvas" data-bs-target="#bdsRentFilterCanvas" aria-controls="bdsRentFilterCanvas">
                        Bá»™ lá»c
                    </button>
                </div>
                <div class="bds-filter-chip-row">
                    <%= RenderActiveFilterChips() %>
                </div>
            </section>

            <div class="bds-list-summary-bar d-flex flex-wrap justify-content-between align-items-center gap-2">
                <div>
                    <div class="bds-list-total"><%= TotalCount.ToString("#,##0") %> tin cho thuÃª phÃ¹ há»£p</div>
                    <div class="text-muted small">
                        <asp:Label ID="lbPageSummary" runat="server" Text="0-0 / 0"></asp:Label>
                    </div>
                </div>
            </div>

            <asp:PlaceHolder ID="phList" runat="server">
                <div class="row row-cards">
                    <asp:Repeater ID="rptRentListings" runat="server">
                        <ItemTemplate>
                            <div class="col-12 col-sm-6 col-lg-4 bds-col-market">
                                <article class="card bds-rent-card h-100">
                                    <div class="card-body d-flex flex-column gap-2">
                                        <a class="bds-rent-thumb" href="<%# BuildListingUrl(Container.DataItem) %>">
                                            <img loading="lazy" decoding="async" src='<%# Eval("ThumbnailUrl") %>' alt='<%# Eval("Title") %>' data-fallback="/uploads/images/bat-dong-san/bds-placeholder.jpg" onerror="this.onerror=null;this.src='/uploads/images/bat-dong-san/bds-placeholder.jpg';" />
                                        </a>
                                        <div class="bds-card-chip-row">
                                            <%# RenderSourceChip(Container.DataItem) %>
                                            <span class="bds-card-chip bds-card-chip-soft"><%# Eval("PropertyTypeLabel") %></span>
                                        </div>
                                        <a class="bds-rent-title" href="<%# BuildListingUrl(Container.DataItem) %>"><%# Eval("Title") %></a>
                                        <div class="bds-rent-price"><%# Eval("PriceText") %></div>
                                        <div class="bds-intent-pill-row"><%# RenderIntentHighlights(Container.DataItem) %></div>
                                        <div class="bds-card-location"><%# Eval("AddressText") %></div>
                                        <div class="bds-card-support-line"><%# RenderSupportLine(Container.DataItem) %></div>
                                        <div class="bds-rent-summary small mb-0"><%# RenderListingSummary(Container.DataItem) %></div>
                                        <div class="mt-auto d-flex justify-content-between align-items-center gap-2 pt-2">
                                            <div class="text-muted small"><%# Eval("PostedAgoText") %></div>
                                            <a class="btn btn-sm btn-outline-primary bds-list-cta" href="<%# BuildListingUrl(Container.DataItem) %>">Chi tiáº¿t</a>
                                        </div>
                                    </div>
                                </article>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="bds-rent-empty">
                    KhÃ´ng cÃ³ tin cho thuÃª nÃ o phÃ¹ há»£p bá»™ lá»c hiá»‡n táº¡i.
                </div>
            </asp:PlaceHolder>

            <div class="d-flex justify-content-center">
                <asp:Literal ID="litPager" runat="server"></asp:Literal>
            </div>
        </div>
    </div>

    <div class="offcanvas offcanvas-end bds-filter-canvas" tabindex="-1" id="bdsRentFilterCanvas" aria-labelledby="bdsRentFilterCanvasLabel" data-bs-scroll="true" data-bs-backdrop="false">
        <div class="offcanvas-header">
            <div>
                <div class="bds-section-kicker">Tinh chá»‰nh káº¿t quáº£</div>
                <h5 class="offcanvas-title" id="bdsRentFilterCanvasLabel">Bá»™ lá»c cho thuÃª</h5>
            </div>
            <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <div class="bds-filter-core-note">
                Giá»¯ bá»™ lá»c á»Ÿ má»©c tá»‘i giáº£n trÆ°á»›c Ä‘á»ƒ há»‡ thá»‘ng tráº£ káº¿t quáº£ nhanh vÃ  chÃ­nh xÃ¡c hÆ¡n trÃªn cáº£ tin liÃªn káº¿t.
            </div>
            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label">Khu vá»±c</label>
                    <asp:DropDownList ID="ddlProvince" runat="server" CssClass="form-select" />
                </div>
                <div class="col-12">
                    <label class="form-label">Quáº­n/Huyá»‡n</label>
                    <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="form-select" />
                </div>
                <div class="col-12">
                    <label class="form-label">Loáº¡i hÃ¬nh</label>
                    <asp:DropDownList ID="ddlPropertyType" runat="server" CssClass="form-select" />
                </div>
                <div class="col-12 col-sm-6">
                    <label class="form-label">PhÃ²ng ngá»§</label>
                    <asp:DropDownList ID="ddlBedrooms" runat="server" CssClass="form-select" />
                </div>
                <div class="col-12 col-sm-6">
                    <label class="form-label">Khoáº£ng giÃ¡</label>
                    <asp:DropDownList ID="ddlPrice" runat="server" CssClass="form-select" />
                </div>
                <div class="col-12 col-sm-6">
                    <label class="form-label">Diá»‡n tÃ­ch</label>
                    <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-select" />
                </div>
                <div class="col-12 col-sm-6">
                    <label class="form-label">Sáº¯p xáº¿p</label>
                    <asp:DropDownList ID="ddlSort" runat="server" CssClass="form-select" />
                </div>
                <div class="col-12">
                    <details class="bds-advanced-filter">
                        <summary>TÃ¹y chá»n nÃ¢ng cao</summary>
                        <div class="bds-advanced-filter-note">
                            Ná»™i tháº¥t vÃ  dá»± Ã¡n thÆ°á»ng khÃ´ng Ä‘áº§y Ä‘á»§ trÃªn má»i nguá»“n liÃªn káº¿t. Chá»‰ báº­t khi báº¡n thá»±c sá»± cáº§n lá»c sÃ¢u hÆ¡n.
                        </div>
                        <div class="row g-3">
                            <div class="col-12 col-sm-6">
                                <label class="form-label">Ná»™i tháº¥t</label>
                                <asp:DropDownList ID="ddlFurnishing" runat="server" CssClass="form-select" />
                            </div>
                            <div class="col-12 col-sm-6">
                                <label class="form-label">Dá»± Ã¡n</label>
                                <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-select" />
                            </div>
                        </div>
                    </details>
                </div>
            </div>
        </div>
        <div class="offcanvas-footer">
            <a class="btn btn-outline-secondary" href="/bat-dong-san/cho-thue.aspx">Reset</a>
            <button type="button" class="btn btn-success" onclick="document.getElementById('<%= btnFilter.ClientID %>').click();">Ãp dá»¥ng</button>
        </div>
    </div>
</asp:Content>
