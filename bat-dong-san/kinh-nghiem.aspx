<%@ Page Title="Kinh nghiệm bất động sản" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="kinh-nghiem.aspx.cs" Inherits="bat_dong_san_kinh_nghiem" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Bất động sản / Kinh nghiệm</div>
                    <h2 class="page-title">Kinh nghiệm</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="mb-3">
                <uc:BdsNav ID="BdsNavMain" runat="server" Current="guide" ShowTopbar="false" IncludeAssets="false" ShowBanner="false" ShowNav="false" />
            </div>

            <div class="row row-cards">
                <div class="col-md-4">
                    <a class="card text-decoration-none text-reset" href="/bat-dong-san/mua-ban.aspx">
                        <div class="card-body">
                            <div class="fw-semibold">Mua nhà</div>
                        </div>
                    </a>
                </div>
                <div class="col-md-4">
                    <a class="card text-decoration-none text-reset" href="/bat-dong-san/cho-thue.aspx">
                        <div class="card-body">
                            <div class="fw-semibold">Thuê nhà</div>
                        </div>
                    </a>
                </div>
                <div class="col-md-4">
                    <a class="card text-decoration-none text-reset" href="/bat-dong-san/du-an.aspx">
                        <div class="card-body">
                            <div class="fw-semibold">Dự án</div>
                        </div>
                    </a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
