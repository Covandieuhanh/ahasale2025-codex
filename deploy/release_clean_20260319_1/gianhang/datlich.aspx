<%@ Page Title="Đặt lịch hẹn" Language="C#" MasterPageFile="~/gianhang/mp-home.master" AutoEventWireup="true" CodeFile="datlich.aspx.cs" Inherits="chitiettintuc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta property="og:type" content="website" />
    <title><%=title_web %></title>
    <asp:PlaceHolder runat="server"><%=meta %></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="container-fluid pt-10 pb-20">
        <div class="container">
            <div style="border-left: 4px solid #008a00;" class="fw-600">
                <div class="pl-3 fs-bc-34-28 pt-1 text-upper">ĐẶT LỊCH HẸN</div>
                <div class="pl-3 title-sub-home-bc mt-2-minus pb-1">Vui lòng điền đầy đủ thông tin bên dưới</div>
            </div>

            <asp:Panel ID="Panel1" runat="server" DefaultButton="but_dathang">
                <div class="example bg-light p-4 mt-6">
                    <div class="row">

                        <div class="cell-lg-6 mt-3 pr-2-lg">
                            <label class="fw-600">Chọn ngày</label>
                            <asp:TextBox ID="txt_ngay" runat="server" MaxLength="10"
                                data-role="calendar-picker" data-outside="true" data-dialog-mode="true"
                                data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY"
                                data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                        </div>

                        <div class="cell-lg-6 mt-3 pl-2-lg">
                            <label class="fw-600">Chọn giờ</label>
                            <div class="d-flex">
                                <asp:DropDownList ID="ddl_giobatdau" runat="server" data-role="select" data-filter="flase" CssClass="mr-1"></asp:DropDownList>
                                <asp:DropDownList ID="ddl_phutbatdau" runat="server" data-role="select" data-filter="flase" CssClass="ml-1"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="cell-lg-6 mt-3 pr-2-lg">
                            <span class="place-left fw-600">Tên khách hàng</span>
                            <span class="ani-float place-left pl-2">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ErrorMessage="!" ForeColor="#CE352C" ControlToValidate="txt_hoten"
                                    ValidationGroup="val_dathang"></asp:RequiredFieldValidator>
                            </span>
                            <div class="input">
                                <asp:TextBox ID="txt_hoten" runat="server" ValidationGroup="val_dathang" MaxLength="50"></asp:TextBox>
                            </div>
                        </div>

                        <div class="cell-lg-6 mt-3 pl-2-lg">
                            <span class="place-left fw-600">Điện thoại</span>
                            <span class="ani-float place-left pl-2">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                    ErrorMessage="!" ForeColor="#CE352C" ControlToValidate="txt_sdt"
                                    ValidationGroup="val_dathang"></asp:RequiredFieldValidator>
                            </span>
                            <div class="input">
                                <asp:TextBox ID="txt_sdt" runat="server" ValidationGroup="val_dathang"></asp:TextBox>
                            </div>
                        </div>

                        <div class="cell-lg-6 mt-3 pr-2-lg">
                            <span class="place-left fw-600">Chọn dịch vụ muốn đặt</span>
                            <span class="ani-float place-left pl-2">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                    ErrorMessage="!" ForeColor="#CE352C" ControlToValidate="DropDownList1"
                                    InitialValue="" ValidationGroup="val_dathang"></asp:RequiredFieldValidator>
                            </span>
                            <div>
                                <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="true"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="cell-lg-6 mt-3 pl-2-lg">
                            <label class="fw-600">Ghi chú</label>
                            <asp:TextBox ID="txt_ghichu" data-role="input" runat="server"></asp:TextBox>
                        </div>

                        <div class="cell-lg-12 text-center mt-8 mb-6">
                            <asp:Button ID="but_dathang" runat="server" CssClass="button info large primary"
                                Text="ĐẶT LỊCH HẸN" ValidationGroup="val_dathang" OnClick="but_dathang_Click" />
                        </div>

                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
