<%@ Page Title="Thông tin khách hàng" Language="C#" MasterPageFile="~/gianhang/webcon/mp-webcon.master" AutoEventWireup="true" CodeFile="ho-so.aspx.cs" Inherits="taikhoan_add" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div class="container-fluid pt-10 pb-20">
        <div class="container">
            <div class="row">
                <div class="cell-lg-12 mt-5">
                    <div class="text-center">
                        <div style="">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="text-bold mt-1">
                            <%=hoten %>
                        </div>
                        <div class="text-bold">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%=sdt %>">
                                <span class="mif mif-phone"></span><%=sdt %>
                            </a>
                        </div>
                    </div>
                </div>
                <div class="cell-lg-12 mt-5">
                    <ul data-role="tabs" data-expand="true">
                        <li><a href="#_khachhang">Khách hàng</a></li>
                        <li><a href="#_hoadon">Hóa đơn</a></li>
                        <li><a href="#_dathen">Đặt hẹn</a></li>
                        <li><a href="#_ghichu">Ghi chú</a></li>
                        <li class="<%=act_hinhanh %>"><a href="#_hinhanh">Hình ảnh</a></li>
                        <li><a href="#_donthuoc">Đơn thuốc</a></li>
                        <li><a href="#_thedichvu">Thẻ dịch vụ</a></li>

                    </ul>
                    <div class="">
                        <div id="_khachhang">
                            <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">

                                <div class="row">

                                    <div class="cell-lg-6 pr-3-lg">

                                        <div class="mt-3">
                                            <label class="fw-600">Tên khách hàng</label>
                                            <div>
                                                <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Điện thoại</label>
                                            <div>
                                                <asp:TextBox ID="txt_dienthoai" runat="server" data-role="input" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Ngày sinh</label>
                                            <div>
                                                <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Địa chỉ</label>
                                            <asp:TextBox ID="txt_diachi" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox><%--autocomplete="off" --%>
                                        </div>
                                    </div>
                                    <div class="cell-lg-6 pl-3-lg">
                                        <div class="mt-3">
                                            <label class="fw-600">Ảnh đại diện</label>

                                            <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 500x500 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>

                                            <div class="place-right">
                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:Button ID="Button2" runat="server" Text="Xóa ảnh hiện tại" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />


                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Nhân viên chăm sóc</label>
                                            <%--<asp:DropDownList ID="ddl_nhanvien_chamsoc" data-role="select" data-filter="true" runat="server"></asp:DropDownList>--%>
                                            <div>
                                                <asp:Label ID="lb_nv_chamsoc" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>

                                        <div class="mt-3">
                                            <label class="fw-600">Mã giới thiệu</label>
                                            <%--<asp:TextBox ID="txt_magioithieu" runat="server" data-role="input"></asp:TextBox>--%>
                                            <div>
                                                <asp:Label ID="txt_magioithieu" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                        <%--<div class="mt-3">
                                        <label class="fw-600">Nhóm khách hàng</label>
                                        <asp:DropDownList ID="DropDownList1" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>--%>
                                    </div>

                                </div>

                                <div class="text-center mt-6 mb-6">
                                    <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="button1_Click" />
                                </div>
                            </asp:Panel>
                        </div>

                        <div id="_hoadon">
                            <div class="row mt-2">
                                <div class="cell-lg-4 mt-2">
                                    <div class="icon-box border bd-cyan">
                                        <div class="icon bg-cyan fg-white"><span class="mif-open-book"></span></div>
                                        <div class="content p-4">
                                            <div class="text-upper">Số hóa đơn: <span class="text-bold"><%=tong_hoadon.ToString("#,##0") %></span></div>
                                            <%--<div class="text-upper text-bold text-lead"></div>--%>
                                            <div class="mt-3">
                                                <small>
                                                    <span class="fg-cyan pr-3 text-bold"><%=tong_dv.ToString("#,##0") %> dịch vụ</span>
                                                    <span class="fg-green pr-3 text-bold"><%=tong_sp.ToString("#,##0") %> sản phẩm</span>
                                                </small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="cell-lg-4 mt-2">
                                    <div class="icon-box border bd-red">
                                        <div class="icon bg-red fg-white"><span class="mif-dollars"></span></div>
                                        <div class="content p-4">
                                            <div class="text-upper">Tổng chi tiêu</div>
                                            <div class="text-upper text-bold text-lead"><%=tong_chitieu.ToString("#,##0") %></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="cell-lg-4 mt-2">
                                    <div class="icon-box border bd-orange">
                                        <div class="icon bg-orange fg-white"><span class="mif-money"></span></div>
                                        <div class="content p-4">
                                            <div class="text-upper">Tổng công nợ</div>
                                            <div class="text-upper text-bold text-lead"><%=tong_congno.ToString("#,##0") %></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <div class="text-bold mt-10 ">
                                            <a data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết" class="fg-orange" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id") %>">Hóa đơn: <%#Eval("id") %></a>
                                        </div>
                                        <div style="overflow: auto" class=" mt-2">
                                            <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                                <thead>
                                                    <tr class="bg-green">
                                                        <td class="text-bold fg-white" style="width: 1px; min-width: 1px;">Ngày tạo</td>
                                                        <td class="text-bold fg-white" style="min-width: 120px;">Người tạo</td>
                                                        <td class="text-bold fg-white" style="width: 100px; min-width: 100px">Tổng tiền</td>
                                                        <td class="text-bold fg-white" style="width: 1px; min-width: 1px">CK</td>
                                                        <td class="text-bold fg-white" style="width: 100px; min-width: 100px">Sau CK</td>
                                                        <td class="text-bold fg-white" style="width: 110px; min-width: 110px">Thanh toán</td>
                                                        <td class="text-bold fg-white" style="width: 86px; min-width: 86px">Công nợ</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td class="text-right">
                                                            <div><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                        </td>
                                                        <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>
                                                        <td class="text-right"><%#Eval("tongtien","{0:#,##0}").ToString() %></td>
                                                        <td class="text-right">
                                                            <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("chietkhau").ToString()=="0" %>'>
                                                                <%#Eval("tongtien_ck_hoadon","{0:#,##0}").ToString() %>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("chietkhau").ToString()!="0" %>'>
                                                                <%#Eval("chietkhau")%>%
                                                            </asp:PlaceHolder>
                                                        </td>
                                                        <td class="text-right">
                                                            <div><%#Eval("tongsauchietkhau","{0:#,##0}").ToString() %></div>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                                <span class="data-wrapper"><code class="bg-red  fg-white">Đã thanh toán</code></span>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                                <%#Eval("sotien_dathanhtoan","{0:#,##0}").ToString() %>
                                                            </asp:PlaceHolder>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                                <%#Eval("sotien_conlai","{0:#,##0}").ToString() %>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                                <span class="data-wrapper"><code class="bg-orange fg-white"><%#Eval("sotien_conlai","{0:#,##0}").ToString() %></code></span>
                                                            </asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>

                                        <div data-role="accordion"
                                            data-one-frame="false"
                                            data-show-active="true"
                                            data-on-frame-open="console.log('frame was opened!', arguments[0])"
                                            data-on-frame-close="console.log('frame was closed!', arguments[0])">
                                            <div class="frame">
                                                <div class="heading">Dịch vụ - Sản phẩm</div>
                                                <div class="content">
                                                    <div style="overflow: auto" class=" mt-2">
                                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                                            <thead>
                                                                <tr style="background-color: #f5f5f5">

                                                                    <td class="text-bold" style="width: 1px; min-width: 1px;">Ngày bán</td>
                                                                    <td class="text-bold" style="min-width: 120px;">Người bán</td>
                                                                    <td class="text-bold" style="min-width: 150px">Mặt hàng</td>

                                                                    <td class="text-bold text-center" style="width: 1px;">Giá</td>
                                                                    <td class="text-bold" style="width: 1px;">SL</td>
                                                                    <td class="text-bold" style="width: 102px; min-width: 102px">Thành tiền</td>
                                                                    <td class="text-bold" style="width: 1px;">CK</td>
                                                                    <td class="text-bold" style="width: 100px;">Sau CK</td>


                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="Repeater2" runat="server" DataSource='<%#show_chitiet_hoadon(Eval("id").ToString()) %>'>
                                                                    <ItemTemplate>
                                                                        <tr>

                                                                            <td class="text-right">
                                                                                <div><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                                            </td>
                                                                            <td><%#return_ten_nhanvien(Eval("nguoitao").ToString())%></td>
                                                                            <td>
                                                                                <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                                                    <span class="fg-blue"><%#Eval("ten_dvsp_taithoidiemnay").ToString() %></span>
                                                                                </asp:PlaceHolder>
                                                                                <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("kyhieu").ToString()=="sanpham" %>'>
                                                                                    <span class="fg-green"><%#Eval("ten_dvsp_taithoidiemnay").ToString() %></span>
                                                                                </asp:PlaceHolder>
                                                                                <div>
                                                                                    <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#Eval("id_thedichvu")!=null%>'>
                                                                                        <span class="data-wrapper"><code class="bg-orange fg-white">Thẻ DV số <%#Eval("id_thedichvu")%></code></span>
                                                                                    </asp:PlaceHolder>
                                                                                </div>
                                                                            </td>
                                                                            <td class="text-right">
                                                                                <%#Eval("gia_dvsp_taithoidiemnay","{0:#,##0}").ToString() %>                                       
                                                                            </td>
                                                                            <td class="text-right">
                                                                                <%#Eval("soluong","{0:#,##0}").ToString() %>
                                                                            </td>
                                                                            <td class="text-right">
                                                                                <div><%#Eval("thanhtien","{0:#,##0}").ToString() %> </div>
                                                                            </td>
                                                                            <td class="text-right">
                                                                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("chietkhau").ToString()=="0" %>'>
                                                                                    <%#Eval("tongtien_ck_dvsp","{0:#,##0}").ToString() %>
                                                                                </asp:PlaceHolder>
                                                                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("chietkhau").ToString()!="0" %>'>
                                                                                    <%#Eval("chietkhau")%>%
                                                                                </asp:PlaceHolder>
                                                                            </td>
                                                                            <td class="text-right">
                                                                                <div><b><%#Eval("tongsauchietkhau","{0:#,##0}").ToString() %></b></div>
                                                                            </td>


                                                                        </tr>
                                                                        <%--  <%stt = stt + 1; %>--%>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>

                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                        <div id="_dathen">
                            <div style="overflow: auto" class="mt-3">
                                <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                    <tbody>
                                        <tr style="background-color: #f5f5f5">
                                            <td class="text-bold" style="min-width: 1px; width: 1px">Mã</td>
                                            <td class="text-bold" style="min-width: 1px; width: 1px">Ngày đặt</td>
                                            <td class="text-bold" style="min-width: 140px">Khách hàng</td>

                                            <td class="text-bold" style="min-width: 140px">Dịch vụ</td>
                                            <td class="text-bold" style="min-width: 140px">NV thực hiện</td>
                                            <td class="text-bold" style="min-width: 140px">Ghi chú</td>
                                            <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                            <td class="text-bold" style="min-width: 100px; width: 100px">Trạng thái</td>
                                        </tr>
                                        <asp:Repeater ID="Repeater3" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-center">
                                                        <a data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" href="/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=<%#Eval("id").ToString() %>">
                                                            <b><%#Eval("id").ToString() %></b>
                                                        </a>
                                                    </td>
                                                    <td class="text-right"><%#Eval("ngaydat","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                    <td><%#Eval("tenkhachhang").ToString() %>
                                                        <div><a class="fg-cobalt" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></div>
                                                    </td>

                                                    <td><%#Eval("tendv").ToString() %></td>
                                                    <td><%#return_ten_nhanvien(Eval("nhanvien_thuchien").ToString()) %></td>
                                                    <td><%#Eval("ghichu").ToString() %></td>
                                                    <td>
                                                        <%#return_ten_nhanvien(Eval("nguoitao").ToString()) %>
                                                        <div><small><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></small></div>
                                                    </td>
                                                    <td>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chưa xác nhận" %>'>
                                                            <span class="data-wrapper"><code class="bg-cyan  fg-white">Chưa xác nhận</code></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã xác nhận" %>'>
                                                            <span class="data-wrapper"><code class="bg-green  fg-white">Đã xác nhận</code></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("trangthai").ToString()=="Không đến" %>'>
                                                            <span class="data-wrapper"><code class="bg-orange  fg-white">Không đến</code></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã đến" %>'>
                                                            <span class="data-wrapper"><code class="bg-magenta  fg-white">Đã đến</code></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã hủy" %>'>
                                                            <span class="data-wrapper"><code class="bg-red  fg-white">Đã hủy</code></span>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                </tr>

                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                        <div id="_ghichu">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                </Triggers>
                                <ContentTemplate>

                                    <div style="overflow: auto" class="mt-3">
                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                            <tbody>
                                                <tr style="background-color: #f5f5f5">
                                                    <td style="width: 1px;">
                                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table-ghichu input[type=checkbox]').prop('checked', this.checked)">
                                                    </td>
                                                    <td class="text-bold" style="min-width: 1px; width: 1px">Ngày tạo</td>
                                                    <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                                    <td class="text-bold" style="min-width: 180px">Nội dung ghi chú</td>
                                                    <td class="text-bold" style="min-width: 80px; width: 80px">Mã đơn</td>
                                                </tr>
                                                <asp:Repeater ID="Repeater4" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="checkbox-table-ghichu">
                                                                <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("kyhieu").ToString()=="table" %>'>
                                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="ghichu_<%#Eval("id").ToString() %>">
                                                                </asp:PlaceHolder>
                                                            </td>
                                                            <td class="text-right"><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                            <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>
                                                            <td><%#Eval("ghichu").ToString() %></td>
                                                            <td class="text-bold text-center">
                                                                <a data-role="hint" data-hint-position="top" data-hint-text="Xem hóa đơn" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id_hoadon").ToString() %>">
                                                                    <%#Eval("id_hoadon").ToString() %>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                        <div id="_hinhanh">
                            <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                </Triggers>
                                <ContentTemplate>

                                    <div style="overflow: auto" class="mt-3">
                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                            <tbody>
                                                <tr style="background-color: #f5f5f5">
                                                    <td style="width: 1px;">
                                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table-hinhanh input[type=checkbox]').prop('checked', this.checked)">
                                                    </td>
                                                    <td class="text-bold" style="min-width: 1px; width: 1px">Ngày tạo</td>
                                                    <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                                    <td class="text-bold" style="min-width: 230px; width: 230px">Ảnh trước sau</td>
                                                    <td class="text-bold" style="min-width: 140px">Ghi chú</td>

                                                </tr>
                                                <asp:Repeater ID="Repeater6" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="checkbox-table-hinhanh">

                                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="hinhanh_<%#Eval("id").ToString() %>">
                                                            </td>
                                                            <td class="text-right"><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                            <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>


                                                            <td class="text-center">
                                                                <div data-role="lightbox">
                                                                    <%#Eval("anhtruoc").ToString() %>
                                                                    <%#Eval("anhsau").ToString() %>
                                                                </div>
                                                            </td>



                                                            <td><%#Eval("ghichu").ToString() %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                        <div id="_donthuoc">
                            <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                </Triggers>
                                <ContentTemplate>

                                    <div style="overflow: auto" class="mt-3">
                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                            <tbody>
                                                <tr style="background-color: #f5f5f5">
                                                    <td style="width: 1px;">
                                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table-donthuoc input[type=checkbox]').prop('checked', this.checked)">
                                                    </td>
                                                    <td class="text-bold" style="min-width: 1px; width: 1px">Ngày tạo</td>
                                                    <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                                    <td class="text-bold" style="min-width: 180px">Nội dung đơn thuốc</td>
                                                    <td class="text-bold" style="min-width: 1px; width: 1px">Tái khám</td>
                                                    <td class="text-bold" style="min-width: 140px">Nơi tái khám</td>
                                                    <td class="text-bold" style="min-width: 180px">Lời dặn bác sĩ</td>
                                                </tr>
                                                <asp:Repeater ID="Repeater5" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="checkbox-table-donthuoc">

                                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="donthuoc_<%#Eval("id").ToString() %>">
                                                            </td>
                                                            <td class="text-right"><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                            <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>
                                                            <td><%#Eval("ghichu").ToString() %></td>
                                                            <td class="text-right"><%#Eval("ngaytaikham","{0:dd/MM/yyyy}").ToString() %></td>
                                                            <td><%#Eval("noitaikham").ToString() %></td>
                                                            <td><%#Eval("loidan").ToString() %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                        <div id="_thedichvu">
                            <div class="row mt-2">
                                <div class="cell-lg-4 mt-2">
                                    <div class="icon-box border bd-cyan">
                                        <div class="icon bg-cyan fg-white"><span class="mif-credit-card"></span></div>
                                        <div class="content p-4">
                                            <div class="text-upper">Thẻ đã mua</div>
                                            <div class="text-upper text-bold text-lead"><%=sl_thedv.ToString("#,##0") %></div>
                                        </div>
                                    </div>
                                </div>
                                <div class="cell-lg-4 mt-2">
                                    <div class="icon-box border bd-red">
                                        <div class="icon bg-red fg-white"><span class="mif-dollars"></span></div>
                                        <div class="content p-4">
                                            <div class="text-upper">Tổng giá trị thẻ</div>
                                            <div class="text-upper text-bold text-lead"><%=doanhso_tdv_sauck.ToString("#,##0") %></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="cell-lg-4 mt-2">
                                    <div class="icon-box border bd-orange">
                                        <div class="icon bg-orange fg-white"><span class="mif-money"></span></div>
                                        <div class="content p-4">
                                            <div class="text-upper">Tổng công nợ</div>
                                            <div class="text-upper text-bold text-lead"><%=tong_congno_tdv.ToString("#,##0") %></div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div>
                                <asp:Repeater ID="Repeater7" runat="server">
                                    <ItemTemplate>
                                        <div class="text-bold mt-4 ">
                                            <a data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết" class="fg-orange" href="/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=<%#Eval("id") %>">Thẻ dịch vụ số <%#Eval("id") %></a>
                                        </div>
                                        <div style="overflow: auto" class=" mt-2">
                                            <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                                <thead>
                                                    <tr style="background-color: #f5f5f5">
                                                        <td style="width: 1px;"></td>
                                                        <td class="text-bold" style="min-width: 90px; width: 90px">Thời hạn</td>
                                                        <td class="text-bold" style="min-width: 130px">Người bán/CK</td>
                                                        <td class="text-bold" style="min-width: 120px;">Tên thẻ/DV</td>
                                                        <td class="text-bold" style="min-width: 100px;">Số buổi</td>

                                                        <td class="text-bold" style="min-width: 130px">Tổng tiền/CK</td>

                                                        <td class="text-bold" style="min-width: 100px">Sau CK</td>
                                                        <td class="text-bold" style="min-width: 110px">T.Toán/C.Nợ</td>

                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td class="checkbox-table-tdv">
                                                            <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_thedv_<%#Eval("id").ToString() %>">
                                                        </td>
                                                        <td class="text-right">
                                                            <small>
                                                                <div><%#Eval("ngaytao","{0:dd/MM/yyyy}").ToString() %></div>
                                                                <div><%#check_hsd(Eval("hsd","{0:dd/MM/yyyy}").ToString()) %></div>
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

                                                    <%--<%stt = stt + 1; %>--%>
                                                </tbody>
                                                <tfoot>
                                                    <tr class="">
                                                        <td colspan="4"></td>
                                                        <td class="text-right text-bold"><%=doanhso_tdv.ToString("#,##0") %></td>

                                                        <td class="text-right text-bold fg-red"><%=doanhso_tdv_sauck.ToString("#,##0") %></td>
                                                        <td class="text-right text-bold">
                                                            <%--<div><small>TT: <%=tongtien_dathanhtoan.ToString("#,##0") %></small></div>--%>

                                                            <div class="fg-orange"><%=tong_congno.ToString("#,##0") %></div>
                                                        </td>

                                                    </tr>
                                                </tfoot>

                                            </table>

                                        </div>
                                        <div data-role="accordion"
                                            data-one-frame="false"
                                            data-show-active="true"
                                            data-on-frame-open="console.log('frame was opened!', arguments[0])"
                                            data-on-frame-close="console.log('frame was closed!', arguments[0])">
                                            <div class="frame">
                                                <div class="heading">Nhật ký sử dụng</div>
                                                <div class="content">
                                                    <div style="overflow: auto" class=" mt-3 mb-3">
                                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                                            <thead>
                                                                <tr style="background-color: #f5f5f5">

                                                                    <td class="text-bold" style="width: 1px; min-width: 1px;">STT</td>
                                                                    <td class="text-bold" style="width: 100px;">Ngày SD</td>
                                                                    <%--<td class="text-bold text-center" style="width: 50px; min-width: 50px">Ảnh</td>--%>
                                                                    <td class="text-bold" style="min-width: 150px">Dịch vụ</td>
                                                                    <td class="text-bold" style="width: 1px">Đơn</td>
                                                                    <td class="text-bold" style="min-width: 170px">Nhân viên làm dịch vụ</td>
                                                                    <td class="text-bold" style="width: 170px; min-width: 170px">Đánh giá người làm</td>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="Repeater1" runat="server" DataSource='<%#show_chitiet_thedv(Eval("id").ToString()) %>'>
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td><%=stt_tdv %></td>
                                                                            <td class="text-right">
                                                                                <div><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                                            </td>

                                                                            <td>

                                                                                <span class="fg-blue"><%#Eval("ten_dvsp_taithoidiemnay").ToString() %></span>
                                                                            </td>
                                                                            <td class="text-bold text-center">
                                                                                <a data-role="hint" data-hint-position="top" data-hint-text="Xem hóa đơn" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id_hoadon").ToString()%>">
                                                                                    <%#Eval("id_hoadon").ToString()%>
                                                                                </a>
                                                                            </td>
                                                                            <td>
                                                                                <div><%#Eval("tennguoilam_hientai").ToString()%></div>

                                                                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("phantram_lamdichvu").ToString()=="0" %>'>
                                                                                    <div class="text-right text-bold fg-emerald"><%#Eval("tongtien_lamdichvu","{0:#,##0}").ToString() %></div>
                                                                                </asp:PlaceHolder>
                                                                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("phantram_lamdichvu").ToString()!="0" %>'>
                                                                                    <div class="text-right text-bold fg-emerald"><%#Eval("phantram_lamdichvu")%>%</div>
                                                                                </asp:PlaceHolder>
                                                                            </td>
                                                                            <td>
                                                                                <%#Eval("danhgia_nhanvien_lamdichvu")%>
                                                                                <div>
                                                                                    <input data-role="rating" data-value="<%#Eval("danhgia_5sao_dv")%>" name="danhgia_5sao_dv_<%#Eval("id")%>" data-static="true">
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <%stt_tdv = stt_tdv + 1; %>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>

                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

