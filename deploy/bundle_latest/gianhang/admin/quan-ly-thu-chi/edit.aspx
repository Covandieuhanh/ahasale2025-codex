<%@ Page Title="Sửa thu chi" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-thu-chi/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <asp:Panel ID="Panel1" runat="server" DefaultButton="but_form_themthuchi">
            <div class="row">


                <div class="cell-lg-6 pr-2-lg">
                    <div class="mt-3">
                            <label class="fw-600">Ngành</label>
                            <asp:DropDownList ID="DropDownList3" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                        </div>
                    <div class="mt-3">
                        <label class="fw-600">Ngày</label>
                        <asp:TextBox ID="txt_ngaylap" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Thu/Chi</label>
                        <asp:DropDownList ID="ddl_loaiphieu" runat="server" data-role="select" data-filter="false">
                            <asp:ListItem Text="Phiếu thu" Value="Thu"></asp:ListItem>
                            <asp:ListItem Text="Phiếu chi" Value="Chi"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Loại thu chi</label>
                        <a class="place-right fg-red fg-orange-hover" href="/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx"><small>Quản lý nhóm thu chi</small></a>
                        <asp:DropDownList ID="ddl_nhomtc" runat="server" data-role="select" data-filter="false"></asp:DropDownList>
                    </div>

                    <div class="mt-3">
                        <label class="fw-600">Số tiền</label>
                        <asp:TextBox ID="txt_sotien" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                    </div>
                    <div class="mt-3">
                            <label class="fw-600">Người nhận tiền</label>
                            <asp:DropDownList ID="ddl_nhanvien_nhantien" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                        </div>
                    <div class="mt-3">
                        <label class="fw-600">Nội dung</label>
                        <asp:TextBox ID="txt_noidung" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox><%--autocomplete="off" --%>
                    </div>


                </div>
                <div class="cell-lg-6 pl-2-lg">
                    <div class="mt-3">
                        <label>Ảnh đính kèm</label>
                        <asp:FileUpload ID="FileUpload1" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="true" />
                    </div>
                    <div class="mt-3">

                        <div data-role="lightbox" style="cursor: pointer">
                            <div class="row">
                                <asp:Repeater ID="Repeater2" runat="server">
                                    <ItemTemplate>
                                        <div class="cell-4">
                                            <img src="<%#Eval("hinhanh").ToString() %>" class="img-cover-vuong w-h-100" />
                                            <small><a href="/gianhang/admin/quan-ly-thu-chi/xoa-anh.aspx?id=<%#Eval("id").ToString() %>&link=<%#Eval("hinhanh").ToString() %>" class="fg-red">Xóa</a></small>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
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

