<%@ Page Title="Cây menu" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="cay-menu.aspx.cs" Inherits="admin_quan_ly_menu_cay_menu" %>

<%@ Register Src="~/gianhang/admin/quan-ly-menu/uc/treeview.ascx" TagPrefix="uc1" TagName="treeview" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">
         <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-menu/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
                
            </div>
        </div>
        <div class="mt-3">
            <div class="fw-600">Cây menu</div>
        </div>
        <uc1:treeview runat="server" ID="treeview" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

