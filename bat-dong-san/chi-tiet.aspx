<%@ Page Title="Chi tiáº¿t tin báº¥t Ä‘á»™ng sáº£n" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="chi-tiet.aspx.cs" Inherits="bat_dong_san_chi_tiet" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-body bds-detail-page">
        <div class="container-xl">
            <section class="bds-page-topbar bds-page-topbar-detail">
                <div>
                    <div class="bds-page-kicker">AhaSale / Báº¥t Ä‘á»™ng sáº£n / Chi tiáº¿t tin</div>
                    <h1 class="bds-page-heading">Chi tiáº¿t báº¥t Ä‘á»™ng sáº£n</h1>
                </div>
                <a class="btn btn-outline-secondary btn-sm" href="<%= ResolveBackToListingUrl() %>">Quay láº¡i káº¿t quáº£</a>
            </section>

            <asp:PlaceHolder ID="phDetail" runat="server">
                <div class="row g-3">
                    <div class="col-lg-8">
                        <div class="card bds-detail-main-card bds-detail-overview-card">
                            <div class="card-body d-flex flex-column gap-3">
                                <div class="d-flex flex-wrap gap-1">
                                    <span class='badge <%= BuildPurposeBadgeCss() %>'><%= BuildPurposeLabel() %></span>
                                    <span class="badge bg-azure-lt text-azure"><%= Listing == null ? "" : Server.HtmlEncode(Listing.PropertyTypeLabel) %></span>
                                    <%= RenderSourceChip() %>
                                </div>
                                <div class="bds-detail-title"><%= Listing == null ? "Chi tiáº¿t tin BÄS" : Server.HtmlEncode(Listing.Title) %></div>
                                <div class="bds-detail-price"><%= Listing == null ? "-" : Server.HtmlEncode(Listing.PriceText) %></div>
                                <div class="bds-detail-location"><%= Listing == null ? "" : Server.HtmlEncode(Listing.AddressText) %></div>
                                <div class="bds-detail-quick-row"><%= RenderQuickHighlights() %></div>
                            </div>
                        </div>

                        <div class="card bds-detail-main-card">
                            <div class="card-body d-flex flex-column gap-3">
                                <div class="bds-gallery-main">
                                    <img decoding="async" src="<%= ResolveMainImage() %>" alt="<%= Listing == null ? "Báº¥t Ä‘á»™ng sáº£n" : Server.HtmlEncode(Listing.Title) %>" onerror="this.onerror=null;this.src='/uploads/images/bat-dong-san/bds-placeholder.jpg';" />
                                </div>
                                <div class="row g-2">
                                    <asp:Repeater ID="rptGallery" runat="server">
                                        <ItemTemplate>
                                            <div class="col-4 col-md-3">
                                                <div class="bds-gallery-thumb">
                                                    <img loading="lazy" decoding="async" src="<%# Container.DataItem %>" alt="áº¢nh báº¥t Ä‘á»™ng sáº£n" onerror="this.onerror=null;this.src='/uploads/images/bat-dong-san/bds-placeholder.jpg';" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>

                        <asp:PlaceHolder ID="phProject" runat="server" Visible="false">
                            <div class="card mt-3 bds-detail-subcard">
                                <div class="card-body d-flex flex-column gap-2">
                                    <div class="text-uppercase text-muted fw-bold small">ThÃ´ng tin dá»± Ã¡n</div>
                                    <div class="bds-detail-highlight"><%= Listing == null ? "" : Server.HtmlEncode(Listing.ProjectName) %></div>
                                    <div class="text-muted"><i class="ti ti-map-pin"></i> <%= Listing == null ? "" : Server.HtmlEncode(Listing.AddressText) %></div>
                                    <div class="d-flex flex-wrap gap-2">
                                        <a class="btn btn-outline-success btn-sm" href="/bat-dong-san/du-an.aspx">Xem danh sÃ¡ch dá»± Ã¡n</a>
                                        <a class="btn btn-outline-secondary btn-sm" href="/bat-dong-san/tham-khao-gia.aspx">Xem lá»‹ch sá»­ giÃ¡</a>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <div class="card mt-3 bds-detail-subcard">
                            <div class="card-header">
                                <h3 class="card-title">MÃ´ táº£ chi tiáº¿t</h3>
                            </div>
                            <div class="card-body">
                                <div class="bds-detail-description"><%= RenderDescription() %></div>
                            </div>
                        </div>

                        <asp:PlaceHolder ID="phSimilar" runat="server" Visible="false">
                            <div class="card mt-3 bds-detail-subcard">
                                <div class="card-header">
                                    <h3 class="card-title">Tin tÆ°Æ¡ng tá»± (cÃ¹ng loáº¡i & khu vá»±c)</h3>
                                </div>
                                <div class="card-body">
                                    <div class="row row-cards">
                                        <asp:Repeater ID="rptSimilar" runat="server">
                                            <ItemTemplate>
                                            <div class="col-md-6">
                                                    <div class="card bds-related-card">
                                                        <div class="card-body d-flex flex-column gap-2">
                                                            <a class="bds-card-title text-reset text-decoration-none" href="<%# BuildListingUrl(Container.DataItem) %>"><%# Eval("Title") %></a>
                                                            <div class="bds-related-price"><%# Eval("PriceText") %></div>
                                                            <div class="text-muted small"><%# Eval("AddressText") %></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phProjectListings" runat="server" Visible="false">
                            <div class="card mt-3 bds-detail-subcard">
                                <div class="card-header">
                                    <h3 class="card-title">Tin khÃ¡c trong dá»± Ã¡n</h3>
                                </div>
                                <div class="card-body">
                                    <div class="row row-cards">
                                        <asp:Repeater ID="rptProjectListings" runat="server">
                                            <ItemTemplate>
                                            <div class="col-md-6">
                                                    <div class="card bds-related-card">
                                                        <div class="card-body d-flex flex-column gap-2">
                                                            <a class="bds-card-title text-reset text-decoration-none" href="<%# BuildListingUrl(Container.DataItem) %>"><%# Eval("Title") %></a>
                                                            <div class="bds-related-price"><%# Eval("PriceText") %></div>
                                                            <div class="text-muted small"><%# Eval("AddressText") %></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <div class="card mt-3 bds-detail-subcard">
                            <div class="card-header">
                                <h3 class="card-title">Tin gá»£i Ã½ khÃ¡c</h3>
                            </div>
                            <div class="card-body">
                                <div class="row row-cards">
                                    <asp:Repeater ID="rptRelated" runat="server">
                                        <ItemTemplate>
                                            <div class="col-md-6">
                                                <div class="card bds-related-card">
                                                    <div class="card-body d-flex flex-column gap-2">
                                                        <a class="bds-card-title text-reset text-decoration-none" href="<%# BuildListingUrl(Container.DataItem) %>"><%# Eval("Title") %></a>
                                                        <div class="bds-related-price"><%# Eval("PriceText") %></div>
                                                        <div class="text-muted small"><%# Eval("AddressText") %></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-4">
                        <div class="card sticky-top bds-detail-sidebar-sticky bds-detail-sidebar-card">
                            <div class="card-body d-flex flex-column gap-3">
                                <div class="row g-2">
                                    <asp:PlaceHolder ID="phFactArea" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">Diá»‡n tÃ­ch</div>
                                            <div class="fw-semibold"><%= Listing == null ? "-" : BatDongSanService_cl.FormatArea(Listing.AreaValue) %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phFactUnitPrice" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">ÄÆ¡n giÃ¡</div>
                                            <div class="fw-semibold"><%= Listing == null ? "-" : BatDongSanService_cl.FormatPricePerSquareMeter(Listing) %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phFactBedrooms" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">PhÃ²ng ngá»§</div>
                                            <div class="fw-semibold"><%= Listing == null ? "-" : Listing.BedroomCount.ToString() %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phFactBathrooms" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">PhÃ²ng táº¯m</div>
                                            <div class="fw-semibold"><%= Listing == null ? "-" : Listing.BathroomCount.ToString() %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phFactLegal" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">PhÃ¡p lÃ½</div>
                                            <div class="fw-semibold"><%= Listing == null ? "-" : Server.HtmlEncode(Listing.LegalStatus) %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phFactFurnishing" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">Ná»™i tháº¥t</div>
                                            <div class="fw-semibold"><%= Listing == null ? "-" : Server.HtmlEncode(Listing.FurnishingStatus) %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phFactProject" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">Dá»± Ã¡n</div>
                                            <div class="fw-semibold"><%= Listing == null || string.IsNullOrWhiteSpace(Listing.ProjectName) ? "KhÃ´ng thuá»™c dá»± Ã¡n" : Server.HtmlEncode(Listing.ProjectName) %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phFactDirection" runat="server" Visible="false">
                                    <div class="col-6">
                                        <div class="bds-detail-fact">
                                            <div class="text-muted small">HÆ°á»›ng</div>
                                            <div class="fw-semibold"><%= Listing == null || string.IsNullOrWhiteSpace(Listing.HouseDirection) ? "ChÆ°a cáº­p nháº­t" : Server.HtmlEncode(Listing.HouseDirection) %></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phRentFacts" runat="server" Visible="false">
                                        <div class="col-6">
                                            <div class="bds-detail-fact">
                                                <div class="text-muted small">Tiá»n cá»c</div>
                                                <div class="fw-semibold"><%= FormatDepositAmount() %></div>
                                            </div>
                                        </div>
                                        <div class="col-6">
                                            <div class="bds-detail-fact">
                                                <div class="text-muted small">Ká»³ háº¡n thuÃª</div>
                                                <div class="fw-semibold"><%= FormatRentalTerm() %></div>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phHouseFacts" runat="server" Visible="false">
                                        <div class="col-6">
                                            <div class="bds-detail-fact">
                                                <div class="text-muted small">Sá»‘ táº§ng</div>
                                                <div class="fw-semibold"><%= FormatFloorCount() %></div>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phLandFacts" runat="server" Visible="false">
                                        <div class="col-6">
                                            <div class="bds-detail-fact">
                                                <div class="text-muted small">KÃ­ch thÆ°á»›c</div>
                                                <div class="fw-semibold"><%= FormatLandSize() %></div>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                                <div class="card bds-detail-seller-card border-0">
                                    <div class="card-body">
                                        <div class="text-uppercase text-muted fw-bold small mb-2">Nguá»“n liÃªn há»‡</div>
                                        <div class="text-muted small">NgÆ°á»i Ä‘Äƒng</div>
                                        <div class="fw-semibold"><%= Listing == null ? "-" : Server.HtmlEncode(Listing.SellerName) %></div>
                                        <asp:PlaceHolder ID="phSellerRole" runat="server" Visible="false">
                                            <div class="text-muted small"><%= Listing == null ? "" : Server.HtmlEncode(Listing.SellerRole) %></div>
                                        </asp:PlaceHolder>
                                        <div class="text-muted small mt-2">Cáº­p nháº­t: <%= Listing == null ? "-" : Server.HtmlEncode(Listing.PostedAgoText) %></div>
                                    </div>
                                </div>
                                <div class="btn-list">
                                    <asp:PlaceHolder ID="phCallAction" runat="server" Visible="false">
                                        <a class="btn btn-success w-100 bds-detail-primary-cta" href="<%= BuildCallHref() %>">Gá»i ngÆ°á»i Ä‘Äƒng</a>
                                    </asp:PlaceHolder>
                                    <a class="btn btn-outline-success w-100 bds-detail-secondary-cta" href="<%= ResolveBackToListingUrl() %>">Vá» danh sÃ¡ch káº¿t quáº£</a>
                                </div>
                                <asp:PlaceHolder ID="phSourceAction" runat="server" Visible="false">
                                    <a class="btn btn-outline-azure w-100 bds-detail-source-cta" href="<%= ResolveSourceUrl() %>" target="_blank" rel="noopener">Xem táº¡i nguá»“n</a>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="card">
                    <div class="card-body text-center text-muted">KhÃ´ng tÃ¬m tháº¥y tin báº¥t Ä‘á»™ng sáº£n phÃ¹ há»£p.</div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
