<%@ Page Title="Sửa Chuyên gia" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="taikhoan_add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-giang-vien/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div class="row">
            <div class="cell-12">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">

                    <div class="row">
                        <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                            <h5>Thông tin Chuyên gia</h5>
                            <div class="mt-3 fg-red">
                                <label class="fw-600">Họ tên</label>
                                <div>
                                    <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh đại diện</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 500x500 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                        <div>
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div style='position: absolute; bottom: 0px; left: 100px'>
                                            <asp:Button ID="Button2" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
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
                            <div class="mt-3">
                                <label class="fw-600">Ngày sinh</label>
                                <div>
                                    <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mt-3">
                                <div class="fw-600">Trạng thái</div>
                                <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Đang giảng dạy" Value="Đang giảng dạy"></asp:ListItem>
                                    <asp:ListItem Text="Ngưng giảng dạy" Value="Ngưng giảng dạy"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                            <h5>Liên hệ</h5>
                            <div class="mt-3">
                                <label class="fw-600">Email</label>
                                <div>
                                    <asp:TextBox ID="txt_email" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Điện thoại</label>
                                <div>
                                    <asp:TextBox ID="txt_dienthoai" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Zalo</label>
                                <div>
                                    <asp:TextBox ID="txt_zalo" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Facebook</label>
                                <div>
                                    <asp:TextBox ID="txt_facebook" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                            <h5>Chuyên môn</h5>
                            <%--<div class="mt-3">
                                <label class="fw-600">Chuyên môn</label>
                                <div>
                                    <asp:TextBox ID="txt_chuyenmon" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>--%>
                            <div class="mt-3">
                                        <label class="fw-600">Ngành</label>
                                         <asp:DropDownList ID="DropDownList5" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
       
                                    </div>
                            <div class="mt-3">
                                <label class="fw-600">Gói đào tạo</label>
                                <div>
                                    <asp:DropDownList ID="DropDownList2" runat="server" data-role="select" data-filter="false">
                                        <asp:ListItem Value="Cơ bản" Text="Cơ bản"></asp:ListItem>
                                        <asp:ListItem Value="Nâng cao" Text="Nâng cao"></asp:ListItem>
                                        <asp:ListItem Value="Combo" Text="Combo"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Số buổi lý thuyết</label>
                                <div>
                                    <asp:TextBox ID="txt_sobuoi_lythuyet" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Số buổi thực hành</label>
                                <div>
                                    <asp:TextBox ID="txt_sobuoi_thuchanh" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Số buổi trợ giảng</label>
                                <div>
                                    <asp:TextBox ID="txt_sobuoi_trogiang" runat="server" data-role="input"></asp:TextBox>
                                </div>
                            </div>
                            
                            <div class="mt-3">
                                <label class="fw-600">Đánh giá Chuyên gia</label>
                                <div>
                                    <asp:TextBox ID="txt_danhgia" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="text-center mt-10">
                        <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="button1_Click1" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

