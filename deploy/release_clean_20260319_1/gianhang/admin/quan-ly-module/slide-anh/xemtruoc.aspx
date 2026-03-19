<%@ Page Title="Xem trước Slide ảnh" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="xemtruoc.aspx.cs" Inherits="badmin_quan_ly_website_quan_ly_menu_xemtruoc" %>

<%@ Register Src="~/gianhang/admin/quan-ly-module/slide-anh/uc/slider.ascx" TagPrefix="uc1" TagName="slider" %>





<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-module/slide-anh/default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div>
            <uc1:slider runat="server" ID="slider" />
        </div>
    </div>


    


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

