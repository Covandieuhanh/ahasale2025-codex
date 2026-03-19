<%@ Page Title="Quản lý tin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="tin-da-luu.aspx.cs" Inherits="tin_da_luu" %>
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
