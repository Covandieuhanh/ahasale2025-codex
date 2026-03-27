<%@ Page Title="Nhập vật tư" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="nhap-vat-tu.aspx.cs" Inherits="badmin_quan_ly_menu_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">



    <div id="main-content" class="mb-10">

        <ul data-role="tabs" data-expand="true">
            <li><a href="#_donnhaphang">Đơn nhập vật tư</a></li>
            <li class="<%=act_nhaphang %>"><a href="#_nhaphang">Nhập vật tư</a></li>
        </ul>
        <div class="">
            <div id="_donnhaphang">
                <div>
                    <div class="row">
                        <div class="cell-lg-6 pr-2-lg">
                            <div class="mt-3">
                                <label class="fw-600">Ngày nhập</label>
                                <asp:TextBox ID="txt_ngaynhap" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                            </div>
                        </div>
                        <div class="cell-lg-6 pl-2-lg">
                            <div class="mt-3">
                                <label class="fw-600">Nhà cung cấp</label>
                                <asp:DropDownList ID="DropDownList3" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="cell-lg-12">
                            <div class="mt-3">
                                <label class="fw-600">Ghi chú</label>
                                <asp:TextBox ID="txt_ghichu" runat="server" data-role="input"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                        </div>
                    </div>


                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mt-5 mb-5 text-right">
                                <asp:Button ID="but_save" runat="server" Text="Lưu chỉnh sửa" Visible="false" CssClass="primary mr-2" OnClick="but_capnhat_Click" />
                                <asp:Button ID="but_huygiohang" runat="server" Text="Hủy nhập" Visible="false" CssClass="alert" OnClick="but_huygiohang_Click" />
                            </div>
                            <div class="mt-2" style="overflow: auto">
                                <table class="table row-hover table-border cell-border compact normal-lg bg-white <%--striped--%> <%--compact normal-lg--%>">
                                    <thead>
                                        <tr style="background-color: #60a917">
                                            <th style="width: 1px"></th>
                                            <td class="text-bold text-center fg-white" style="width: 50px; min-width: 50px">Ảnh</td>
                                            <td class="text-bold fg-white" style="min-width: 150px">Vật tư</td>
                                            <td class="text-bold fg-white" style="width: 100px">ĐVT/Số lô</td>
                                            <td class="text-bold fg-white" style="width: 157px">Đơn giá/SL nhập</td>
                                            <td class=" text-bold fg-white" style="width: 121px">Chiết khấu</td>

                                            <td class="text-bold text-right fg-white" style="width: 110px;">Thành tiền</td>
                                            <td class=" text-bold fg-white" style="width: 1px">Bảo hành</td>


                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="Repeater2" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <a href="/gianhang/admin/quan-ly-vat-tu/xoa_chitiet_giohang.aspx?id=<%#Eval("ID").ToString() %>" data-role="hint" data-hint-text="Xóa" data-hint-position="top">
                                                            <span class="mif mif-2x mif-cancel fg-red"></span>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <img src="<%#Eval("img") %>" class="img-cover-vuong w-h-50" style="max-width: none!important">
                                                    </td>
                                                    <td><%#Eval("Name").ToString() %></td>


                                                    <td>
                                                        <div><small>Đơn vị tính</small></div>
                                                        <input style="width: 60px" data-role="input" type="text" maxlength="4" value="<%#Eval("dvt").ToString() %>" name="dvt_gh_<%#Eval("ID").ToString() %>" data-clear-button="false" onkeypress="if (event.keyCode==13) return false;">

                                                        <div><small>Số lô</small></div>
                                                        <input style="width: 60px" data-role="input" type="text" value="<%#Eval("solo").ToString() %>" name="solo_gh_<%#Eval("ID").ToString() %>" onkeypress="if (event.keyCode==13) return false;" data-clear-button="false">
                                                    </td>

                                                    <td>
                                                        <div><small>Đơn giá</small></div>
                                                        <input data-role="input" data-min-value="1" data-max-value="9999" maxlength="13" data-clear-button="false" name="gianhap_<%#Eval("ID").ToString() %>" type="text" onchange="format_sotien(this);" value="<%#tien(Eval("Price").ToString()) %>" onkeypress="if (event.keyCode==13) return false;">
                                                        <div><small>Số lượng nhập</small></div>
                                                        <input data-role="spinner" data-min-value="1" data-max-value="9999" maxlength="4" data-clear-button="false" name="sl_<%#Eval("ID").ToString() %>" type="text" value="<%#Eval("soluong").ToString() %>" onkeypress="if (event.keyCode==13) return false;">
                                                    </td>

                                                    <td>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("loai_chietkhau").ToString()=="phantram" %>'>
                                                            <input type="radio" name="loai_ck_gh_<%#Eval("ID").ToString() %>" value="phantram" checked />%
                                                                <input type="radio" name="loai_ck_gh_<%#Eval("ID").ToString() %>" value="tienmat" />Tiền
                                                            <input style="width: 60px" data-role="input" type="text" value="<%#Eval("chietkhau").ToString() %>" name="ck_gh_<%#Eval("ID").ToString() %>" onkeypress="if (event.keyCode==13) return false;" onblur="format_sotien_new(this)" data-clear-button="false">
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("loai_chietkhau").ToString()=="tienmat" %>'>
                                                            <input type="radio" name="loai_ck_gh_<%#Eval("ID").ToString() %>" value="phantram" />%
                                                                <input type="radio" name="loai_ck_gh_<%#Eval("ID").ToString() %>" value="tienmat" checked />Tiền
                                                            <input style="width: 60px" data-role="input" type="text" value="<%#Eval("tongtien_ck_hoadon").ToString() %>" name="ck_gh_<%#Eval("ID").ToString() %>" onkeypress="if (event.keyCode==13) return false;" onblur="format_sotien_new(this)" data-clear-button="false">
                                                        </asp:PlaceHolder>

                                                    </td>
                                                    <td class="text-right text-bold"><%#tien(Eval("sauck").ToString()) %></td>
                                                    <td>
                                                        <div><small>Ngày nhập</small></div>
                                                        <input style="width: 106px" type="text" maxlength="10" value="<%#Eval("nsx").ToString() %>" name="nsx_gh_<%#Eval("ID").ToString() %>" onkeypress="if (event.keyCode==13) return false;" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false">
                                                        <div><small>Hạn bảo hành</small></div>
                                                        <input style="width: 106px" type="text" maxlength="10" value="<%#Eval("hsd").ToString() %>" name="hsd_gh_<%#Eval("ID").ToString() %>" onkeypress="if (event.keyCode==13) return false;" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false">
                                                    </td>


                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                    <%if (sl_hangtronggio > 0)
                                        { %>
                                    <tfoot>
                                        <tr class=" text-bold">
                                            <td colspan="6" class="text-right">Tổng cộng</td>
                                            <td class="text-right"><%=tongtien.ToString("#,##0") %></td>

                                        </tr>
                                        <tr class=" ">
                                            <td colspan="6" class="text-right "><span class="text-bold">Chiết khấu hóa đơn</span>
                                                <div>
                                                    <asp:RadioButton ID="ck_hd_phantram" runat="server" Text="%" GroupName="ck_hd" />
                                                    <asp:RadioButton ID="ck_hd_tienmat" runat="server" Text="Tiền" GroupName="ck_hd" />
                                                </div>
                                            </td>
                                            <td class="text-right">
                                                <asp:TextBox onkeypress="if (event.keyCode==13) return false;" Style="text-align: right;" ID="txt_chietkhau" runat="server" data-role="input" MaxLength="10" Text="0" onchange="format_sotien(this);" data-clear-button="false"></asp:TextBox>
                                                <%--<%=chietkhau %>%--%>
                                            </td>
                                        </tr>
                                        <tr class=" text-bold">
                                            <td colspan="6" class="text-right text-size-20">Sau chiết khấu</td>
                                            <td class="text-right text-size-20"><%=sauchietkhau.ToString("#,##0") %></td>

                                        </tr>
                                        <tr class="text-bold">
                                            <td colspan="7" class="text-right fg-red">Số tiền bằng chữ: <%=number_class.number_to_text_unlimit(sauchietkhau.ToString()) %> đồng.
                                            </td>
                                        </tr>
                                    </tfoot>
                                    <%}
                                        else
                                        { %>
                                    <h5>Đơn chưa có gì.</h5>
                                    <%} %>
                                </table>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>


                </div>
                <div class="text-center mt-8 mb-20">
                    <asp:Button ID="Button1" runat="server" Text="XÁC NHẬN NHẬP VẬT TƯ" CssClass="button success" OnClick="Button1_Click" onclientclick="return confirm('Bạn đã lưu chỉnh sửa gần nhất chưa?');" />
                </div>
            </div>
            <div id="_nhaphang">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="row mt-3 <%--mt-0-lg-minus mt-12-minus--%>">
                            <div class="cell-md-6 order-2 order-md-1 mt-0">

                                <div class="d-flex flex-align-center gap-2">
                                    <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm"></asp:TextBox>
                                    <asp:LinkButton ID="but_search" runat="server" CssClass="button" OnClick="but_search_Click" CausesValidation="false">
                                        <span class="mif mif-search"></span>
                                    </asp:LinkButton>
                                </div>

                            </div>
                            <div class="cell-md-6 order-1 order-md-2 mt-0">

                                <div class="place-right">
                                    <ul class="h-menu">
                                        <%--<li data-role="hint" data-hint-position="top" data-hint-text="Nhập hàng" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-plus"></span></a></li>--%>
                                        <%--<li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>--%>
                                    </ul>
                                </div>
                                <div class="clr-float"></div>

                            </div>
                        </div>
                        <div id="table-main">
                            <div style="overflow: auto" class=" mt-3">
                                <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                    <thead>
                                        <tr style="background-color: #f5f5f5">
                                            <%--<td style="width: 1px;" class=" text-bold text-center">#</td>--%>
                                            <td style="width: 50px;" class=" text-bold text-center">Ảnh</td>
                                            <td class=" text-bold " style="min-width: 120px">Tên sản phẩm</td>
                                            <td class=" text-bold" style="width: 100px">ĐVT/Số lô</td>
                                            <td class=" text-bold" style="width: 157px">Đơn giá/SL nhập</td>

                                            <td class=" text-bold" style="width: 121px">Chiết khấu</td>
                                            <td class=" text-bold" style="width: 1px">Bảo hành</td>

                                            <%-- <td class=" text-bold " style="width: 94px;">Ngày tạo</td>--%>

                                            <%--<td style="width: 1px;"></td>--%>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="Repeater1" runat="server">
                                            <ItemTemplate>
                                                <tr>

                                                    <%-- <td class="text-center text-bold">
                                                        <%=stt %></td>--%>
                                                    <td>
                                                        <img src="<%#Eval("image") %>" class="img-cover-vuong w-h-50" style="max-width: none!important">
                                                    </td>
                                                    <td>

                                                        <span><%#Eval("tenvattu") %></span>

                                                    </td>
                                                    <td>
                                                        <div><small>Đơn vị tính</small></div>
                                                        <input style="width: 60px" data-role="input" type="text" maxlength="4" value="<%#Eval("donvitinh_sp").ToString() %>" name="dvt_<%#Eval("id").ToString() %>" data-clear-button="false" onkeypress="if (event.keyCode==13) return false;">

                                                        <div><small>Số lô</small></div>
                                                        <input style="width: 60px" data-role="input" type="text" value="<%=nhapvattu_class.return_maxid() %>" name="solo_<%#Eval("id").ToString() %>" onkeypress="if (event.keyCode==13) return false;" data-clear-button="false">
                                                    </td>
                                                    <%--<td><%#return_tenmn(Eval("id_category").ToString()) %></td>--%>

                                                    <td>
                                                        <div><small>Đơn giá</small></div>
                                                        <input style="width: 100px" data-role="input" type="text" value="<%#Eval("gianhap","{0:#,##0}").ToString() %>" onblur="format_sotien_new(this)" name="giavon_<%#Eval("id").ToString() %>" onkeypress="if (event.keyCode==13) return false;" data-clear-button="false">
                                                        <div><small>Số lượng nhập</small></div>
                                                        <input style="width: 60px" data-role="spinner" data-min-value="0" data-max-value="9999" type="text" maxlength="4" value="0" name="slsp_<%#Eval("id").ToString() %>" onkeypress="if (event.keyCode==13) return false;">
                                                    </td>

                                                    <td>
                                                        <input type="radio" name="loai_ck_<%#Eval("id").ToString() %>" value="phantram" checked />%
                                                        <input type="radio" name="loai_ck_<%#Eval("id").ToString() %>" value="tienmat" />Tiền
                                                        <input style="width: 60px" data-role="input" type="text" value="0" name="ck_<%#Eval("id").ToString() %>" onkeypress="if (event.keyCode==13) return false;" onblur="format_sotien_new(this)" data-clear-button="false">
                                                    </td>

                                                    <td>
                                                        <div><small>Ngày nhập</small></div>
                                                        <input style="width: 106px" type="text" maxlength="10" value="<%=DateTime.Now.ToString("dd/MM/yyyy") %>" name="nsx_<%#Eval("id").ToString() %>" onkeypress="if (event.keyCode==13) return false;" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false">
                                                        <div><small>Hạn bảo hành</small></div>
                                                        <input style="width: 106px" type="text" maxlength="10" value="<%=DateTime.Now.AddYears(1).ToString("dd/MM/yyyy") %>" name="hsd_<%#Eval("id").ToString() %>" onkeypress="if (event.keyCode==13) return false;" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false">
                                                    </td>

                                                </tr>
                                                <%--<%stt = stt + 1; %>--%>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                            <div class="text-center mt-8 mb-20">
                                <asp:Button ID="but_themvaogio" runat="server" Text="THÊM VÀO ĐƠN NHẬP VẬT TƯ" CssClass="button success" OnClick="but_themvaogio_Click"  />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>


    </div>



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>

    <script src="/js/gianhang-invoice-fast.js?v=20260326a"></script>
    <script>
        (function () {
            function bindFastUi() {
                if (!window.ahaInvoiceFast) return;
                window.ahaInvoiceFast.initSearchSubmit({
                    inputId: "<%=txt_search.ClientID %>",
                    buttonId: "<%=but_search.ClientID %>"
                });
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
</asp:Content>
