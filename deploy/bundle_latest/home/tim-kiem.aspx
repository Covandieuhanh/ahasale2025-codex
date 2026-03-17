<%@ Page Title="Tìm kiếm" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="tim-kiem.aspx.cs" Inherits="home_tim_kiem" %>
<%@ Register Src="~/Uc/Home/DanhChoBan_MoiNhat_UC.ascx" TagPrefix="uc1" TagName="DanhChoBan_MoiNhat_UC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_sau" runat="server">
    <style>
        .home-search-page .hero-ct-like {
            display: none !important;
        }

        .home-search-page .container-xl.mt-5 {
            margin-top: 12px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="home-search-page">
        <uc1:DanhChoBan_MoiNhat_UC ID="UcSearch" runat="server" TitleText="Kết quả tìm kiếm" />
    </div>
</asp:Content>
