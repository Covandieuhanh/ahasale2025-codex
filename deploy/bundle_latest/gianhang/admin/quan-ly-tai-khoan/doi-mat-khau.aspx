<%@ Page Title="Đổi mật khẩu" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="doi-mat-khau.aspx.cs" Inherits="taikhoan_add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="<%=url_back %>"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div class="row">
            <div class="cell-lg-6">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">
                    <div class="mt-3">
                        Đổi mật khẩu cho tài khoản:
                        <label class="fw-600"><%=user %></label>
                    </div>
                    <%--<div class="mt-3">
                        <label class="fw-600">Mật khẩu hiện tại</label>
                        <div>
                            <input type="password" data-role="input" name="txt_oldpass" value="<%=txt_oldpass %>" />
                        </div>
                    </div>--%>
                    <div class="mt-3">

                        <label class="fw-600">Mật khẩu mới</label>
                        <div>
                            <input type="password" data-role="input" name="txt_pass1" value="<%=txt_pass1%>" />
                        </div>

                    </div>
                    <div class="mt-3">

                        <label class="fw-600">Nhập lại mật khẩu mới</label>
                        <div>
                            <input type="password" data-role="input" name="txt_pass2" value="<%=txt_pass2%>" />
                        </div>

                    </div>
                    <div class="text-right-lg text-center mt-10">
                        <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="button1_Click1" />
                    </div>
                </asp:Panel>
                <div class="cell-lg-6">
                </div>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

