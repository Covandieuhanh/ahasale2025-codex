<%@ Page Title="Đổi mật khẩu" Language="C#" MasterPageFile="~/gianhang/webcon/mp-webcon.master" AutoEventWireup="true" CodeFile="doi-mat-khau.aspx.cs" Inherits="tai_khoan_doi_mat_khau" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="container-fluid pt-10 pb-20">
        <div class="container">
             <div class="row">
            <div class="cell-lg-6">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">
                    <div class="mt-3">
                        <h5>ĐỔI MẬT KHẨU</h5>
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
                        <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="button1" runat="server" Text="ĐỔI MẬT KHẨU" CssClass="button success" OnClick="button1_Click" />
                    </div>
                </asp:Panel>
                <div class="cell-lg-6">
                </div>
            </div>
        </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

