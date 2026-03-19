<%@ Page Title="" Language="C#" MasterPageFile="~/gianhang/webcon/mp-webcon.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="~/gianhang/webcon/uc/dv_noibat.ascx" TagPrefix="uc1" TagName="dv_noibat" %>
<%@ Register Src="~/gianhang/webcon/uc/sp_noibat.ascx" TagPrefix="uc1" TagName="sp_noibat" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta property="og:type" content="website" />
    <title><%=title_web %></title>
    <asp:PlaceHolder runat="server"><%=meta %></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <uc1:dv_noibat runat="server" ID="dv_noibat" />
    <uc1:sp_noibat runat="server" ID="sp_noibat" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

