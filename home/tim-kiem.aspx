<%@ Page Title="Tìm kiếm" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="tim-kiem.aspx.cs" Inherits="home_tim_kiem" %>
<%@ Register Src="~/Uc/Home/DanhChoBan_MoiNhat_UC.ascx" TagPrefix="uc1" TagName="DanhChoBan_MoiNhat_UC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_sau" runat="server">
    <asp:Literal ID="litSearchMeta" runat="server" />
    <style>
        .home-search-page .hero-ct-like {
            display: none !important;
        }

        .home-search-page .container-xl.mt-5 {
            margin-top: 12px !important;
        }

        .search-v2-grid .card {
            border-radius: 12px;
        }

        .search-v2-grid .col-12 {
            content-visibility: auto;
            contain-intrinsic-size: 1px 320px;
        }

        .search-v2-thumb {
            width: 100%;
            aspect-ratio: 16 / 10;
            object-fit: cover;
            border-radius: 10px;
            background: #f8f9fa;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="home-search-page">
        <asp:PlaceHolder ID="phSearchSummary" runat="server" Visible="false">
            <div class="container-xl mt-3">
                <div class="alert alert-light border shadow-sm mb-3">
                    <div class="fw-bold">Kết quả tìm kiếm</div>
                    <div class="small text-secondary">
                        <asp:Literal ID="litSearchSummary" runat="server" />
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phSearchV2List" runat="server" Visible="false">
            <div class="container-xl mt-3 search-v2-grid">
                <div class="row g-3">
                    <asp:Repeater ID="rptSearchV2" runat="server">
                        <ItemTemplate>
                            <div class="col-12 col-md-6 col-xl-4">
                                <div class="card h-100 border shadow-sm">
                                    <img loading="lazy" decoding="async" class="search-v2-thumb card-img-top" src="<%# ResolveSearchItemImage(Eval("ImageUrl")) %>" alt="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Title"))) %>" />
                                    <div class="card-body">
                                        <a class="fw-semibold text-decoration-none d-block mb-1" href="<%# Eval("DetailUrl") %>"><%# HttpUtility.HtmlEncode(Convert.ToString(Eval("Title"))) %></a>
                                        <div class="small text-secondary mb-2"><%# HttpUtility.HtmlEncode(ResolveSearchMetaLine(Eval("CategoryName"), Eval("LocationName"))) %></div>
                                        <div class="small text-muted"><%# HttpUtility.HtmlEncode(ResolveSearchSummaryLine(Eval("Summary"))) %></div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:PlaceHolder ID="phSearchV2Empty" runat="server" Visible="false">
                    <div class="alert alert-light border shadow-sm mt-3 mb-0">
                        <div class="fw-semibold mb-2">Không có kết quả phù hợp.</div>
                        <asp:PlaceHolder ID="phSearchV2EmptySuggest" runat="server" Visible="false">
                            <div class="small text-secondary mb-2">Bạn có thể thử các gợi ý:</div>
                            <asp:Literal ID="litSearchV2EmptySuggest" runat="server" />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phSearchV2Recommend" runat="server" Visible="false">
                            <div class="small text-secondary mt-3 mb-2">Gợi ý cho bạn:</div>
                            <div class="row g-2">
                                <asp:Repeater ID="rptSearchV2Recommend" runat="server">
                                    <ItemTemplate>
                                        <div class="col-12 col-md-6">
                                            <a class="d-block text-decoration-none border rounded-2 p-2 bg-white"
                                               href="<%# Eval("DetailUrl") %>"
                                               data-aha-search-empty="recommend"
                                               data-aha-search-label="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Title"))) %>">
                                                <div class="fw-semibold text-dark"><%# HttpUtility.HtmlEncode(Convert.ToString(Eval("Title"))) %></div>
                                                <div class="small text-secondary"><%# HttpUtility.HtmlEncode(ResolveSearchMetaLine(Eval("CategoryName"), Eval("LocationName"))) %></div>
                                            </a>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </asp:PlaceHolder>
            </div>
        </asp:PlaceHolder>
        <uc1:DanhChoBan_MoiNhat_UC ID="UcSearch" runat="server" TitleText="Kết quả tìm kiếm" />
    </div>
</asp:Content>
