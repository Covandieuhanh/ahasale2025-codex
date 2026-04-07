<%@ Page Title="Chi tiết dự án" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="du-an-chi-tiet.aspx.cs" Inherits="bat_dong_san_du_an_chi_tiet" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Bất động sản / Dự án</div>
                    <h2 class="page-title"><%= Project == null ? "Chi tiết dự án" : Server.HtmlEncode(Project.Name) %></h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body bds-project-detail-page">
        <div class="container-xl">
            <div class="mb-3">
                <uc:BdsNav ID="BdsNavMain" runat="server" Current="project" ShowTopbar="false" IncludeAssets="false" ShowBanner="false" ShowNav="false" />
            </div>

            <asp:PlaceHolder ID="phProject" runat="server">
                <div class="row g-3">
                    <div class="col-lg-4">
                        <div class="card">
                            <div class="card-body d-flex flex-column gap-2">
                                <span class="badge bg-azure-lt text-azure"><%= Project == null ? "" : Server.HtmlEncode(Project.TypeLabel) %></span>
                                <div class="text-muted small">Vị trí</div>
                                <div class="fw-semibold"><%= Project == null ? "" : Server.HtmlEncode(Project.Location) %></div>
                                <div class="text-muted small">Giá tham khảo</div>
                                <div class="fw-semibold text-success"><%= Project == null ? "" : Server.HtmlEncode(Project.PriceHint) %></div>
                                <div class="text-muted small">Số tin</div>
                                <div class="fw-semibold"><%= Project == null ? "0" : Project.ListingCount.ToString("#,##0") %></div>
                                <a class="btn btn-outline-success btn-sm mt-2" href="/bat-dong-san/du-an.aspx">Về danh sách dự án</a>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-8">
                        <div class="card">
                            <div class="card-header">
                                <h3 class="card-title">Tin trong dự án</h3>
                            </div>
                            <div class="card-body">
                                <div class="row row-cards">
                                    <asp:Repeater ID="rptListings" runat="server">
                                        <ItemTemplate>
                                            <div class="col-md-6">
                                                <a class="card text-decoration-none text-reset" href="<%# BuildListingUrl(Container.DataItem) %>">
                                                    <div class="card-body d-flex flex-column gap-2">
                                                        <div class="fw-semibold"><%# Eval("Title") %></div>
                                                        <div class="text-success fw-bold"><%# Eval("PriceText") %></div>
                                                        <div class="text-muted small"><%# Eval("AddressText") %></div>
                                                    </div>
                                                </a>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="card">
                    <div class="card-body text-center text-muted">Không tìm thấy dự án.</div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
