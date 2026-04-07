<%@ Page Title="Dự án bất động sản" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="du-an.aspx.cs" Inherits="bat_dong_san_du_an" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Bất động sản / Dự án</div>
                    <h2 class="page-title">Dự án</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body bds-project-page">
        <div class="container-xl d-flex flex-column gap-3">
            <uc:BdsNav ID="BdsNavMain" runat="server" Current="project" ShowTopbar="false" IncludeAssets="false" ShowBanner="false" ShowNav="false" />

            <div class="d-flex justify-content-between align-items-center gap-2">
                <div class="fw-semibold"><%= Projects.Count.ToString("#,##0") %> dự án</div>
                <a class="btn btn-outline-success btn-sm" href="/bat-dong-san">Về bất động sản</a>
            </div>

            <div class="row row-cards">
                <asp:Repeater ID="rptProjects" runat="server">
                    <ItemTemplate>
                        <div class="col-md-6 col-xl-4">
                            <a class="card bds-project-list-card text-decoration-none text-reset" href="<%# BuildProjectUrl(Container.DataItem) %>">
                                <div class="card-body d-flex flex-column gap-2">
                                    <span class="badge bg-azure-lt text-azure"><%# Eval("TypeLabel") %></span>
                                    <div class="fw-semibold"><%# Eval("Name") %></div>
                                    <div class="text-muted small"><%# Eval("Location") %></div>
                                    <div class="text-success fw-bold"><%# Eval("PriceHint") %></div>
                                    <div class="text-muted small"><%# Eval("ListingCount") %> tin</div>
                                </div>
                            </a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
