<%@ Page Title="Chỉnh sửa hóa đơn" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit-cthd.aspx.cs" Inherits="badmin_Default" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class="mb-10">
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
            </Triggers>
            <ContentTemplate>
                <div>
                    <ul data-role="tabs" data-expand="true">
                        <li <%=active_dv %>><a href="#themdichvu">Dịch vụ</a></li>
                        <li <%=active_sp %>><a href="#themsanpham">Sản phẩm</a></li>
                        <li <%=active_tdv %>><a href="#thedichvu">Thẻ dịch vụ</a></li>
                    </ul>

                    <div class="border bd-default no-border-top p-2 pl-4 pr-4">
                        <div id="themdichvu">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel1" runat="server" DefaultButton="but_form_themdichvu">
                                        <div class="row ">
                                            <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                                <div class="">
                                                    <label class="fw-600">Ngày bán</label>
                                                    <asp:TextBox ID="txt_ngayban" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>

                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Dịch vụ</label>
                                                    <%--<asp:DropDownList ID="ddl_dichvu" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_dichvu_SelectedIndexChanged"></asp:DropDownList>--%>
                                                    <asp:TextBox ID="txt_tendichvu" runat="server" data-role="input" placeholder="Nhập và chọn tên dịch vụ" OnTextChanged="txt_tendichvu_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Giá</label>
                                                    <asp:TextBox ID="txt_gia" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Số lượng</label>
                                                    <asp:TextBox ID="txt_soluong" data-role="input" runat="server" MaxLength="5" Text="1"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Chiết khấu dịch vụ</label>
                                                    <span class="place-right">
                                                        <asp:RadioButton ID="ck_dv_phantram" runat="server" Text="%" GroupName="ck_dv" Checked="true" />
                                                        <asp:RadioButton ID="ck_dv_tienmat" runat="server" Text="Tiền" GroupName="ck_dv" />
                                                    </span>
                                                    <asp:TextBox ID="txt_chietkhau" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                            </div>
                                            <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                                <div>
                                                    <label class="fw-600">Nhân viên chốt sale</label>
                                                    <asp:DropDownList ID="ddl_nhanvien_chotsale" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Chiết khấu chốt sale</label>
                                                    <span class="place-right">
                                                        <asp:RadioButton ID="ck_dv_phantram_chotsale" runat="server" Text="%" GroupName="ck_dv_chotsale" Checked="true" />
                                                        <asp:RadioButton ID="ck_dv_tienmat_chotsale" runat="server" Text="Tiền" GroupName="ck_dv_chotsale" />
                                                    </span>
                                                    <asp:TextBox ID="txt_chietkhau_chotsale" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Nhân viên làm dịch vụ</label>
                                                    <asp:DropDownList ID="ddl_nhanvien_lamdichvu" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Chiết khấu làm dịch vụ</label>
                                                    <span class="place-right">
                                                        <asp:RadioButton ID="ck_dv_phantram_chotsale_lamdv" runat="server" Text="%" GroupName="ck_dv_lamdv" Checked="true" />
                                                        <asp:RadioButton ID="ck_dv_tienmat_chotsale_lamdv" runat="server" Text="Tiền" GroupName="ck_dv_lamdv" />
                                                    </span>
                                                    <asp:TextBox ID="txt_chietkhau_lamdichvu" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Đánh giá nhân viên làm dịch vụ</label>
                                                    <asp:TextBox ID="txt_danhgia_dichvu" data-role="input" runat="server"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <input data-role="rating" data-value="<%=danhgia_5sao_dv %>" name="danhgia_5sao_nhanvien_dv">
                                            </div>
                                        </div>

                                        <div class="mt-6 mb-6 text-right">
                                            <div style="float: left">
                                                <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                            </div>
                                            <div style="float: right">
                                                <asp:Button ID="but_form_themdichvu" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_form_themdichvu_Click" />
                                            </div>
                                            <div style="clear: both"></div>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel2">
                                <ProgressTemplate>
                                    <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                        <div style="padding-top: 50vh;">
                                            <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                        </div>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                        <div id="themsanpham">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel2" runat="server" DefaultButton="but_form_themsanpham">
                                        <div class="row ">
                                            <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                                <div class="">
                                                    <label class="fw-600">Ngày bán</label>

                                                    <asp:TextBox ID="txt_ngayban_sanpham" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Sản phẩm</label>
                                                    <%--<asp:DropDownList ID="ddl_sanpham" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_sanpham_SelectedIndexChanged"></asp:DropDownList>--%>
                                                    <asp:TextBox ID="txt_tensanpham" runat="server" data-role="input" placeholder="Nhập và chọn tên sản phẩm" OnTextChanged="txt_tensanpham_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Giá</label>
                                                    <asp:TextBox ID="txt_gia_sanpham" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Số lượng</label>
                                                    <asp:TextBox ID="txt_soluong_sanpham" data-role="input" runat="server" MaxLength="5" Text="1"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Chiết khấu sản phẩm</label>
                                                    <span class="place-right">
                                                        <asp:RadioButton ID="ck_sp_phantram" runat="server" Text="%" GroupName="ck_sp" Checked="true" />
                                                        <asp:RadioButton ID="ck_sp_tienmat" runat="server" Text="Tiền" GroupName="ck_sp" />
                                                    </span>
                                                    <asp:TextBox ID="txt_chietkhau_sanpham" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                            </div>
                                            <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                                <div>
                                                    <label class="fw-600">Nhân viên chốt sale</label>
                                                    <asp:DropDownList ID="ddl_nhanvien_chotsale_sanpham" data-role="select" data-filter="false" runat="server"></asp:DropDownList>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Chiết khấu chốt sale</label>
                                                    <span class="place-right">
                                                        <asp:RadioButton ID="ck_sp_phantram_chotsale" runat="server" Text="%" GroupName="ck_sp_chotsale" Checked="true" />
                                                        <asp:RadioButton ID="ck_sp_tienmat_chotsale" runat="server" Text="Tiền" GroupName="ck_sp_chotsale" />
                                                    </span>
                                                    <asp:TextBox ID="txt_chietkhau_chotsale_sanpham" data-role="input" runat="server" MaxLength="10" onchange="format_sotien(this);" Text="0"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="mt-6 mb-6 text-right">
                                            <div style="float: left">
                                                <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                            </div>
                                            <div style="float: right">
                                                <asp:Button ID="but_form_themsanpham" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_form_themsanpham_Click" />
                                            </div>
                                            <div style="clear: both"></div>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                                <ProgressTemplate>
                                    <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                        <div style="padding-top: 50vh;">
                                            <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                        </div>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                        <div id="thedichvu">
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel3" runat="server" DefaultButton="Button1">
                                    <div class="row ">
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Ngày bán</label>
                                                <asp:TextBox ID="txt_ngayban_thedv" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Nhân viên làm dịch vụ</label>
                                                <asp:DropDownList ID="ddl_nhanvien_lamdichvu_thedv" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Chiết khấu làm dịch vụ</label>
                                                <span class="place-right">
                                                    <asp:RadioButton ID="ck_dv_phantram_lamdv_thedv" runat="server" Text="%" GroupName="ck_dv_lamdv_thedv" Checked="true" />
                                                    <asp:RadioButton ID="ck_dv_tienmat_lamdv_thedv" runat="server" Text="Tiền" GroupName="ck_dv_lamdv_thedv" />
                                                </span>
                                                <asp:TextBox ID="txt_chietkhau_lamdichvu_thedv" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Đánh giá nhân viên làm dịch vụ</label>
                                                <asp:TextBox ID="txt_danhgia_dichvu_lamdv" data-role="input" runat="server"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <input data-role="rating" data-value="<%=danhgia_5sao_dv_thedv %>" name="danhgia_5sao_nhanvien_dv_lamdv">
                                        </div>

                                    </div>

                                    <div class="mt-3">
                                                <label class="fw-600">Chọn 1 thẻ dịch vụ</label>
                                              
                                            </div>
                                    <div style="overflow: auto" >
                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                            <thead>
                                                <tr style="background-color: #f5f5f5">
                                                    <%--<td style="width: 1px;" class=" text-bold text-center">#</td>--%>
                                                    <td style="width: 1px;"></td>
                                                    <td class="text-bold" style="min-width: 1px; width: 1px">Mã</td>
                                                    <td class="text-bold" style="min-width: 90px; width: 90px">HSD</td>
                                                    <td class="text-bold" style="min-width: 130px">Người bán/CK</td>
                                                    <td class="text-bold" style="min-width: 120px;">Tên thẻ/DV</td>
                                                    <td class="text-bold" style="min-width: 100px;">Số buổi</td>
                                                    <td class="text-bold" style="min-width: 140px">Khách hàng</td>
                                                    <td class="text-bold" style="min-width: 130px">Tổng tiền/CK</td>

                                                    <td class="text-bold" style="min-width: 100px">Sau CK</td>
                                                    <td class="text-bold" style="min-width: 110px">T.Toán/C.Nợ</td>

                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="Repeater3" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <%--<td class="text-right"><%=stt %></td>--%>
                                                            <td class="checkbox-table-tdv">
                                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_thedv_<%#Eval("id").ToString() %>">
                                                            </td>
                                                            <td class="text-center">
                                                                <a data-role="hint" data-hint-position="top" data-hint-text="Chi tiết thẻ dịch vụ" href="/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=<%#Eval("id").ToString() %>">
                                                                    <b><%#Eval("id").ToString() %></b>
                                                                </a>
                                                            </td>
                                                            <td class="text-right">
                                                                <small>
                                                                    <div><%#Eval("hsd","{0:dd/MM/yyyy}").ToString() %></div>

                                                                </small>
                                                            </td>
                                                            <td>
                                                                <small>
                                                                    <%#Eval("tennguoichot").ToString() %>
                                                                    <div class="text-bold">
                                                                        <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("phantramchot").ToString()=="0" %>'>
                                                                            <div class="">CK: <%#Eval("tongtien_chot","{0:#,##0}").ToString() %></div>
                                                                        </asp:PlaceHolder>
                                                                        <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("phantramchot").ToString()!="0" %>'>
                                                                            <div class="">CK: <%#Eval("phantramchot")%>%</div>
                                                                        </asp:PlaceHolder>

                                                                    </div>
                                                                </small>
                                                            </td>

                                                            <td><small>
                                                                <%#Eval("tenthe").ToString() %>
                                                                <div class="fg-green"><%#Eval("tendv").ToString() %></div>
                                                            </small>
                                                            </td>
                                                            <td>
                                                                <small>
                                                                    <div>Số buổi: <%#Eval("sobuoi").ToString() %></div>
                                                                    <div>Đã làm: <%#Eval("sl_dalam").ToString() %></div>
                                                                    <div>Còn lại: <%#Eval("sl_conlai").ToString() %></div>
                                                                </small>
                                                            </td>
                                                            <td><%#Eval("tenkhachhang").ToString() %>
                                                                <div><a class="fg-orange" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></div>
                                                            </td>


                                                            <td class="text-right">
                                                                <%#Eval("tongtien","{0:#,##0}").ToString() %>
                                                                <div class="text-bold">
                                                                    <small>
                                                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("ck_hoadon").ToString()=="0" %>'>CK: <%#Eval("tongtien_ck","{0:#,##0}").ToString() %>
                                                                        </asp:PlaceHolder>
                                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("ck_hoadon").ToString()!="0" %>'>CK: <%#Eval("ck_hoadon")%>%
                                                                        </asp:PlaceHolder>
                                                                    </small>
                                                                </div>
                                                            </td>


                                                            <td class="text-right text-bold">

                                                                <div><%#Eval("tongsauchietkhau","{0:#,##0}").ToString() %></div>
                                                            </td>
                                                            <td class="text-right">
                                                                <div>
                                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                                        <span class="data-wrapper"><code class="bg-red  fg-white">Đã thanh toán</code></span>
                                                                    </asp:PlaceHolder>
                                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                                        <%#Eval("sotien_dathanhtoan","{0:#,##0}").ToString() %>
                                                                    </asp:PlaceHolder>
                                                                </div>
                                                                <div>
                                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                                        <%#Eval("sotien_conlai","{0:#,##0}").ToString() %>
                                                                    </asp:PlaceHolder>
                                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                                        <span class="data-wrapper"><code class="bg-orange fg-white"><%#Eval("sotien_conlai","{0:#,##0}").ToString() %></code></span>
                                                                    </asp:PlaceHolder>
                                                                </div>
                                                            </td>

                                                        </tr>

                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                    <div class="mt-6 mb-6 text-right">
                                        <div style="float: left">
                                            <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                        </div>
                                        <div style="float: right">
                                            <asp:Button ID="Button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="Button1_Click" />
                                        </div>
                                        <div style="clear: both"></div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanel6">
                            <ProgressTemplate>
                                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                    <div style="padding-top: 50vh;">
                                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        function show_saochep() {
            Metro.notify.create("Sao chép link hóa đơn thành công.", "Thông báo", {});
        }
    </script>
    <%--<%=notifi %>--%>
</asp:Content>

