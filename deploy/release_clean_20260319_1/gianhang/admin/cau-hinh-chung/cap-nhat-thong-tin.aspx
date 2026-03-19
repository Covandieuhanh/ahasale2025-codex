<%@ Page Title="Cập nhật thông tin" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="cap-nhat-thong-tin.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class="mb-10">
        <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
            <div class="row">
                <div class="cell-lg-6 pr-3-lg pr-0">

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mt-0">
                                <label class="fw-600">Favicon</label>
                                <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 200x200 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                <div>
                                    <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                </div>
                                <div style='position: absolute; bottom: 0px; left: 80px'>
                                    <asp:Button ID="Button2" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Icon mobile</label>
                                <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 200x200 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                <asp:FileUpload ID="FileUpload3" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                <div>
                                    <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                </div>
                                <div style='position: absolute; bottom: 0px; left: 80px'>
                                    <asp:Button ID="Button3" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button3_Click" />
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Logo menutop</label>
                                <asp:FileUpload ID="FileUpload5" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                <div>
                                    <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                                </div>
                                <div style='position: absolute; bottom: 0px; left: 80px'>
                                    <asp:Button ID="Button5" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button5_Click" />
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Logo chân trang</label>
                                <asp:FileUpload ID="FileUpload4" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                <div>
                                    <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                                </div>
                                <div style='position: absolute; bottom: 0px; left: 80px'>
                                    <asp:Button ID="Button4" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button4_Click" />
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Logo in hóa đơn</label>
                                <asp:FileUpload ID="FileUpload6" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                <div>
                                    <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                                </div>
                                <div style='position: absolute; bottom: 0px; left: 80px'>
                                    <asp:Button ID="Button6" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button6_Click" />
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
                    
                    

                </div>
                <div class="cell-lg-6 pl-3-lg pl-0">
                    
                    <div class="mt-3">
                        <label class="fw-600">Tên công ty</label>
                        <asp:TextBox ID="txt_tencongty" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Mã số thuế</label>
                        <asp:TextBox ID="txt_masothue" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Slogan</label>
                        <asp:TextBox ID="txt_slogan" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Địa chỉ</label>
                        <asp:TextBox ID="txt_diachi" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Link google map</label>
                        <asp:TextBox ID="txt_link_googlemap" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Hotline</label>
                        <asp:TextBox ID="txt_hotline" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Zalo</label>
                        <asp:TextBox ID="txt_zalo" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Email</label>
                        <asp:TextBox ID="txt_email" runat="server" data-role="input"></asp:TextBox>
                    </div>

                </div>
            </div>
            <div class="text-center mt-10">
                        <asp:Button ID="Button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="Button1_Click" />
                    </div>
        </asp:Panel>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

