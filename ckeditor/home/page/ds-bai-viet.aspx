<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="ds-bai-viet.aspx.cs" Inherits="home_page_ds_bai_viet" %>

<%@ Register Src="~/UC/Home/DanhChoBan_MoiNhat_UC.ascx" TagPrefix="uc" TagName="DanhChoBanMoiNhat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head_sau" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="main" runat="Server">
    <asp:HiddenField ID="hdnIdmn" runat="server" />
    <uc:DanhChoBanMoiNhat ID="UcDanhChoBanMoiNhat" runat="server" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="foot_sau" runat="Server"></asp:Content>
