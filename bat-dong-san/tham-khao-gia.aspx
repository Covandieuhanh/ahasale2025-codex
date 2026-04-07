<%@ Page Title="Tham khảo giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="tham-khao-gia.aspx.cs" Inherits="bat_dong_san_tham_khao_gia" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Bất động sản / Tham khảo giá</div>
                    <h2 class="page-title">Tham khảo giá</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="mb-3">
                <uc:BdsNav ID="BdsNavMain" runat="server" Current="price" ShowTopbar="false" IncludeAssets="false" ShowBanner="false" ShowNav="false" />
            </div>

            <div class="card mb-3">
                <div class="card-body">
                    <div class="row g-3 align-items-end">
                        <div class="col-md-4">
                            <label class="form-label">Khu vực</label>
                            <asp:DropDownList ID="ddlProvince" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Loại hình</label>
                            <asp:DropDownList ID="ddlPropertyType" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-4">
                            <asp:LinkButton ID="btnView" runat="server" CssClass="btn btn-success w-100" OnClick="btnView_Click">Xem tin phù hợp</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row row-cards mb-3">
                <div class="col-sm-6 col-lg-3">
                    <div class="card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Số tin</div>
                            <div class="h2 fw-bold mb-0"><asp:Label ID="lbCount" runat="server" Text="0"></asp:Label></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Giá TB</div>
                            <div class="h2 fw-bold mb-0"><asp:Label ID="lbAvgPrice" runat="server" Text="-"></asp:Label></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đơn giá TB</div>
                            <div class="h2 fw-bold mb-0"><asp:Label ID="lbAvgPpsm" runat="server" Text="-"></asp:Label></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Giá min / max</div>
                            <div class="h2 fw-bold mb-0"><asp:Label ID="lbMinMax" runat="server" Text="-"></asp:Label></div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-header d-flex align-items-center">
                    <h3 class="card-title">Top tin gần nhất</h3>
                    <div class="card-actions">
                        <asp:HyperLink ID="lnkViewAll" runat="server" CssClass="btn btn-outline-success btn-sm" NavigateUrl="/bat-dong-san/mua-ban.aspx">Xem tất cả</asp:HyperLink>
                    </div>
                </div>
                <div class="card-body">
                    <asp:Repeater ID="rptListings" runat="server">
                        <HeaderTemplate><div class="row row-cards"></HeaderTemplate>
                        <ItemTemplate>
                            <div class="col-md-6 col-xl-4">
                                <a class="card text-decoration-none text-reset" href="<%# BuildListingUrl(Container.DataItem) %>">
                                    <div class="card-body d-flex flex-column gap-2">
                                        <div class="fw-semibold"><%# Eval("Title") %></div>
                                        <div class="text-success fw-bold"><%# Eval("PriceText") %></div>
                                        <div class="text-muted small"><%# Eval("AddressText") %></div>
                                        <div class="text-muted small"><%# BatDongSanService_cl.FormatPricePerSquareMeter(Container.DataItem as BatDongSanService_cl.ListingItem) %></div>
                                    </div>
                                </a>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate></div></FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
