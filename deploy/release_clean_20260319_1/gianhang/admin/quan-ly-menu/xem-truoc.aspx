<%@ Page Title="Xem trước menu" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="xem-truoc.aspx.cs" Inherits="admin_quan_ly_menu_cay_menu" %>

<%@ Register Src="~/gianhang/admin/quan-ly-menu/uc/treeview.ascx" TagPrefix="uc1" TagName="treeview" %>
<%@ Register Src="~/gianhang/admin/quan-ly-menu/uc/xemtruoc_1cap.ascx" TagPrefix="uc1" TagName="xemtruoc_1cap" %>
<%@ Register Src="~/gianhang/admin/quan-ly-menu/uc/xemtruoc_dacap.ascx" TagPrefix="uc1" TagName="xemtruoc_dacap" %>




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
        
        <div class="mt-6">
            <div class="fw-600">Menu 1 cấp</div>
        </div>
        <uc1:xemtruoc_1cap runat="server" ID="xemtruoc_1cap" />

        <div class="mt-6">
            <div class="fw-600">Menu đa cấp</div>
        </div>
        <uc1:xemtruoc_dacap runat="server" ID="xemtruoc_dacap" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

