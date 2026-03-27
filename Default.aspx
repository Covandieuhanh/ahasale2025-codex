<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="~/Uc/Home/TuKhoaPhoBien_UC.ascx" TagPrefix="uc1" TagName="TuKhoaPhoBien_UC" %>
<%@ Register Src="~/Uc/Home/GioiThieuDai_UC.ascx" TagPrefix="uc1" TagName="GioiThieuDai_UC" %>
<%@ Register Src="~/Uc/Home/DanhChoBan_MoiNhat_UC.ascx" TagPrefix="uc1" TagName="DanhChoBan_MoiNhat_UC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
      <meta property="og:type" content="website" />
  <%--title & meta--%>
  <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="main" runat="Server">
    <uc1:DanhChoBan_MoiNhat_UC runat="server" ID="DanhChoBan_MoiNhat_UC" />
    <uc1:GioiThieuDai_UC runat="server" ID="GioiThieuDai_UC" />
    <uc1:TuKhoaPhoBien_UC runat="server" ID="TuKhoaPhoBien_UC" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>

