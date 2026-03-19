<%@ Page Title="Sửa chi nhánh" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit-chi-nhanh.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">

        <asp:Panel ID="Panel1" runat="server" DefaultButton="but_form_themthuchi">
            <div class="row">
                <div class="cell-lg-6 pr-2-lg">
                    <div class="mt-3">
                        <label class="fw-600">Tên chi nhánh</label>
                        <asp:TextBox ID="txt_tenchinhanh" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Slogan</label>
                        <asp:TextBox ID="TextBox1" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Địa chỉ</label>
                        <asp:TextBox ID="TextBox2" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Email</label>
                        <asp:TextBox ID="TextBox3" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Điện thoại</label>
                        <asp:TextBox ID="TextBox4" runat="server" data-role="input"></asp:TextBox>
                    </div>
                </div>
                <div class="cell-lg-6 pl-2-lg">
                    <div class="mt-3">
                        <label>Logo chân trang</label>
                        <asp:FileUpload ID="FileUpload1" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                        <div>
                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                        </div>
                        <div style='position: absolute; bottom: 0px; left: 150px'>
                            <asp:Button ID="Button1" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button1_Click" />
                        </div>
                    </div>
                    <div class="mt-3">
                        <label>Logo hóa đơn</label>
                        <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                        <div>
                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                        </div>
                        <div style='position: absolute; bottom: 0px; left: 150px'>
                            <asp:Button ID="Button2" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                        </div>
                    </div>
                </div>

            </div>

            <div class="mt-6 mb-10 text-center">
                <asp:Button ID="but_form_themthuchi" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_form_themthuchi_Click" />
            </div>
        </asp:Panel>
    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

