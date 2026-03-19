<%@ Control Language="C#" AutoEventWireup="true" CodeFile="xemtruoc_1cap.ascx.cs" Inherits="badmin_quan_ly_website_quan_ly_menu_uc_xemtruoc_1cap" %>
<div class="pos-relative bg-indigo fg-white" data-role="appbar" data-expand-point="md" style="z-index: 1!important">
    <a href="#" class="brand text-bold ">Home</a>
    <ul class="app-bar-menu ml-auto">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <li><a href="#"><%#Eval("name") %></a></li>
            </ItemTemplate>
        </asp:Repeater>        
    </ul>
</div>
