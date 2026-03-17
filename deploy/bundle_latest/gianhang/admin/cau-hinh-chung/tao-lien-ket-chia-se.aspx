<%@ Page Title="Tạo liên kết chia sẻ" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="tao-lien-ket-chia-se.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <%--<div id="main-head" class="border bd-light border-top-none border-left-none border-right-none">
        <div class="place-left">
            <h5>Cài đặt bảo trì</h5>
        </div>
        <div class="place-right">
            <ul class="h-menu">
                <li data-role="hint" data-hint-position="top" data-hint-text="Tạo hóa đơn"><a class="button" onclick='show_hide_id_form_themhoadon()'><span class="mif mif-plus"></span></a></li>
                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                <li>
                    <a href="#" class="dropdown-toggle">Products</a>
                    <ul class="d-menu" data-role="dropdown">
                        <li><a href="#">Skype</a></li>
                        <li class="divider"></li>
                    </ul>
                </li>
            </ul>
        </div>
        <div class="clr-float"></div>
    </div>--%>

    <div id="main-content" class="mb-10">
        <div class="row">
            <div class="cell-lg-6">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                    <div class="mt-0">
                        <label class="fw-600">Tiêu đề</label>
                        <asp:TextBox ID="txt_title" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Mô tả ngắn</label>
                        <asp:TextBox ID="txt_description" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mt-3">
                                <label class="fw-600">Ảnh thu nhỏ</label>
                                <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 1200x628 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />

                                <div>
                                    <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                </div>

                                <div style='position: absolute; bottom: 0px; left: 150px'>

                                    <asp:Button ID="Button2" runat="server" Text="Xóa ảnh thu nhỏ" CssClass="alert small" Visible="false" OnClick="Button2_Click" />

                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                        <ProgressTemplate>
                            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                <div style="padding-top: 50vh;">
                                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <div class="text-right-lg text-center mt-10">
                        <asp:Button ID="Button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="Button1_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

