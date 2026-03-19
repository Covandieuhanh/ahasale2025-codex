<%@ Page Title="Đơn nhập hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="don-nhap-hang.aspx.cs" Inherits="badmin_Default" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .icon-box {
            height: 103px !important;
        }

            .icon-box .icon {
                height: 102px !important;
                width: 80px;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id='form_1' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_1()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Lọc dữ liệu</h5>
                <asp:Panel ID="Panel2" runat="server" DefaultButton="Button1">
                    <ul data-role="tabs" data-expand="true">
                        <li><a href="#_data">Dữ liệu</a></li>
                        <li><a href="#_time">Thời gian</a></li>
                        <li><a href="#_sort">Sắp xếp</a></li>
                    </ul>
                    <div class="">
                        <div id="_data">
                            <div class="mt-3">
                                <div class="fw-600">Số lượng hiển thị mỗi trang</div>
                                <asp:TextBox ID="txt_show" MaxLength="6" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Thanh toán</label>
                                <asp:DropDownList ID="ddl_locdulieu" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn còn nợ" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn đã thanh toán" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                           
                        </div>
                        <div id="_time">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="mt-3">
                                        <div class="row">
                                            <div class="cell-6 pr-1">
                                                <div class="fw-600">Từ ngày</div>
                                                <asp:TextBox ID="txt_tungay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>

                                            </div>
                                            <div class="cell-6 pl-1">
                                                <div class="fw-600">Đến ngày</div>
                                                <asp:TextBox ID="txt_denngay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="cell-12">
                                                <div class="fw-600">Chọn nhanh</div>
                                            </div>
                                            <div class="cell-6 pr-1">
                                                <div class="mt-1">
                                                    <asp:Button ID="but_homqua" runat="server" Text="Hôm qua" CssClass="mt-1 light" Width="100%" OnClick="but_homqua_Click" />
                                                    <asp:Button ID="but_tuantruoc" runat="server" Text="Tuần trước" CssClass="mt-1 light" Width="100%" OnClick="but_tuantruoc_Click" />
                                                    <asp:Button ID="but_thangtruoc" runat="server" Text="Tháng trước" CssClass="mt-1 light" Width="100%" OnClick="but_thangtruoc_Click" />
                                                    <asp:Button ID="but_quytruoc" runat="server" Text="Quý trước" CssClass="mt-1 light" Width="100%" OnClick="but_quytruoc_Click" />
                                                    <asp:Button ID="but_namtruoc" runat="server" Text="Năm trước" CssClass="mt-1 light" Width="100%" OnClick="but_namtruoc_Click" />
                                                </div>
                                            </div>
                                            <div class="cell-6 pl-1">
                                                <div class="mt-1">
                                                    <asp:Button ID="but_homnay" runat="server" Text="Hôm nay" CssClass="mt-1 light" Width="100%" OnClick="but_homnay_Click" />
                                                    <asp:Button ID="but_tuannay" runat="server" Text="Tuần này" CssClass="mt-1 light" Width="100%" OnClick="but_tuannay_Click" />
                                                    <asp:Button ID="but_thangnay" runat="server" Text="Tháng này" CssClass="mt-1 light" Width="100%" OnClick="but_thangnay_Click" />
                                                    <asp:Button ID="but_quynay" runat="server" Text="Quý này" CssClass="mt-1 light" Width="100%" OnClick="but_quynay_Click" />
                                                    <asp:Button ID="but_namnay" runat="server" Text="Năm này" CssClass="mt-1 light" Width="100%" OnClick="but_namnay_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                                <ProgressTemplate>
                                    <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                        <div style="padding-top: 45vh;">
                                            <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                        </div>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>

                        </div>
                        <div id="_sort">
                            <div class="mt-3">
                                <div class="fw-600">Sắp xếp theo</div>
                                <asp:DropDownList ID="DropDownList2" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Ngày nhập (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Ngày nhập (Giảm dần)" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="mt-6 mb-10">

                        <div style="float: left">
                            <asp:Button ID="Button2" runat="server" Text="Đặt lại mặc định" CssClass="button link small fg-orange-hover" OnClick="Button2_Click" />
                        </div>
                        <div style="float: right">
                            <asp:Button ID="Button1" runat="server" Text="BẮT ĐẦU LỌC" CssClass="button success" OnClick="Button1_Click" OnClientClick="show_hide_id_form_1()" />
                        </div>
                        <div style="clear: both"></div>

                    </div>
                </asp:Panel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_1() {
            var x = document.getElementById("form_1");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%if (bcorn_class.check_quyen(user, "q11_6") == "")
        { %>
    <div id='form_3' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_3()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Xóa các mục đã chọn</h5>
                <hr />
                <asp:Panel ID="Panel3" runat="server" DefaultButton="Button4">
                    <%--<div class="mt-3">
                        <div class="fw-600">Chọn mục muốn xuất</div>
                    </div>--%>
                    <div class="mt-3 fg-red">
                        Dữ liệu sẽ không thể khôi phục sau khi xóa.<br />
                        Các dữ liệu liên quan đến đơn nhập hàng này sẽ bị xóa theo.<br />
                        <b>Bạn có chắc chắn muốn xóa?</b>
                    </div>
                    <hr />
                    <div class="mt-6 mb-10">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button4" runat="server" Text="TÔI XÁC NHẬN MUỐN XÓA" CssClass="button alert" OnClick="Button4_Click" OnClientClick="show_hide_id_form_3()" />
                                </div>
                                <div style="clear: both"></div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </asp:Panel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_3() {
            var x = document.getElementById("form_3");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>


 

    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
              
                <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
                    <div class="cell-md-6 order-2 order-md-1 mt-0">
                        <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm" OnTextChanged="txt_search_TextChanged" AutoPostBack="true"></asp:TextBox>
                    </div>
                    <div class="cell-md-6 order-1 order-md-2 mt-0">
                        <div class="place-right">
                            <ul class="h-menu">
                                <%if (bcorn_class.check_quyen(user, "q11_2") == "")
                                    { %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Nhập hàng">
                                    <a class="button" href="/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx?q=nh"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <%} %>

                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                                <%if (bcorn_class.check_quyen(user, "q11_6") == "")
                                    { %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                    <a class="button" onclick="show_hide_id_form_3()"><span class="mif mif-bin"></span></a>
                                </li>
                                <%} %>
                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                </div>

                <div class="row mt-2">
                  
                    <div class="cell-xxl-3 cell-lg-6 mt-2 pr2-lg">
                        <div class="icon-box border bd-orange">
                            <div class="icon bg-orange fg-white"><span class="mif-open-book"></span></div>
                            <div class="content p-3">
                                <div class="text-upper">
                                    <span class=" text-bold">Đơn nhập hàng</span>
                                    <%--<span class="place-right"><span data-role="hint" data-hint-position="top" data-hint-text="Số lượng" class="badge bg-orange fg-white mr-3 mt-2"><%=hoadon_sl.ToString("#,##0") %></span></span>--%>
                                </div>
                                <%--<div class="text-upper text-bold text-lead"></div>--%>
                                <div class="mt-1">
                                    <small class="d-inline fg-darkGray">Số lượng <span class="place-right"><%=hoadon_sl.ToString("#,##0") %></span></small>
                                </div>
                                <div class="">
                                    <small class="d-inline fg-darkGray">Tổng tiền <span class="place-right"><%=doanhso_hoadon.ToString("#,##0") %></span></small>
                                </div>
                                <div class="">
                                    <small class="d-inline fg-darkGray">Sau chiết khấu <span class="place-right"><%=doanhso_hoadon_sauck.ToString("#,##0") %></span></small>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="cell-xxl-3 cell-lg-6 mt-2 pl-2-lg">
                        <div class="icon-box border bd-red">

                            <div class="icon bg-red fg-white"><span class="mif-paypal"></span></div>

                            <div class="content p-3">
                                <div class="text-upper">
                                    <span class=" text-bold">Thanh toán</span>
                                    <%--<span class="place-right fg-red"><small><%=tongtien_dathanhtoan.ToString("#,##0") %></small></span>--%>
                                </div>
                                <div class="mt-1">
                                    <small class="d-inline fg-darkGray">Đã thanh toán <span class="place-right"><%=tongtien_dathanhtoan.ToString("#,##0") %></span></small>
                                </div>
                                <%--<div class="">
                                    <small class="d-inline fg-darkGray">Chuyển khoản <span class="place-right"><%=tong_chuyenkhoan.ToString("#,##0") %></span></small>
                                </div>--%>
                                <div class="">
                                    <small class="d-inline fg-darkGray">Công nợ
                                        <%if (tong_congno > 0)
                                            { %>
                                        <span class="place-right ani-flash fg-red"><%=tong_congno.ToString("#,##0") %></span>
                                        <%}
                                            else
                                            { %>
                                        <span class="place-right fg-red"><%=tong_congno.ToString("#,##0") %></span><%} %>
                                    </small>
                                </div>
                                <%--<div>
                                    <a href="/gianhang/admin/quan-ly-hoa-don/lich-su-thanh-toan.aspx">
                                        <small class="d-inline fg-red">Xem lịch sử thanh toán</small>
                                    </a>
                                </div>--%>
                            </div>
                        </div>
                    </div>

                </div>

                <%--TABLE CHÍNH--%>
                <div id="table-main">
                    <div style="overflow: auto" class=" mt-4">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                            <thead>
                                <tr style="background-color: #f5f5f5">
                                    <%--<td style="width: 1px;" class=" text-bold text-center">#</td>--%>
                                    <td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>
                                    <td class="text-bold" style="min-width: 1px; width: 1px">Mã</td>
                                    <td class="text-bold" style="width: 110px">Ngày nhập</td>
                                    <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                   <td class="text-bold" style="min-width: 140px">Nhà cung cấp</td>
                                   
                                    <td class="text-bold" style="width: 100px; min-width: 100px">Tổng tiền</td>
                                    <td class="text-bold" style="width: 1px; min-width: 1px">CK</td>
                                    <td class="text-bold" style="width: 100px; min-width: 100px">Sau CK</td>
                                    <td class="text-bold" style="width: 110px; min-width: 110px">Thanh toán</td>
                                    <td class="text-bold" style="width: 86px; min-width: 86px">Công nợ</td>
                                
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <%--<td class="text-right"><%=stt %></td>--%>
                                            <td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                            </td>
                                            <td class="text-center">
                                               <a class="fg-black" data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết" href="/gianhang/admin/quan-ly-kho-hang/chi-tiet-nhap-hang.aspx?id=<%#Eval("id").ToString() %>">
                                                    <b><%#Eval("id").ToString() %></b>
                                                </a>
                                            </td>
                                            <td class="text-right"><%#Eval("ngaytao","{0:dd/MM/yyyy}").ToString() %></td>


                                            <td><%#Eval("nguoitao").ToString() %></td>
                                            <td><%#return_ten_nhacungcap(Eval("nhacungcap").ToString()) %></td>
                                            
                                            <td class="text-right"><%#Eval("tongtien","{0:#,##0}").ToString() %></td>

                                            <td class="text-center">
                                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("ck_hoadon").ToString()=="0" %>'>
                                                    <%#Eval("tongtien_ck","{0:#,##0}").ToString() %>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("ck_hoadon").ToString()!="0" %>'>
                                                    <%#Eval("ck_hoadon")%>%
                                                </asp:PlaceHolder>
                                            </td>
                                            <td class="text-right text-bold">

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
                                       
                                        <%--<%stt = stt + 1; %>--%>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="5"></td>
                              
                                    <td class="text-right text-bold"><%=doanhso_hoadon.ToString("#,##0") %></td>
                                    <td></td>
                                    <td class="text-right text-bold"><%=doanhso_hoadon_sauck.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-red"><%=tongtien_dathanhtoan.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-orange"><%=tong_congno.ToString("#,##0") %></td>
                                  
                                </tr>
                            </tfoot>

                        </table>
                        <div style="height: 90px">&nbsp;</div>
                    </div>

                    <div class="text-center mt-8 mb-20" style="margin-top: -70px!important">
                        <asp:Button ID="but_quaylai" runat="server" Text="Lùi" CssClass="" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Tiếp" CssClass="" OnClick="but_xemtiep_Click" />
                    </div>
                </div>
                <%--END TABLE CHÍNH--%>
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

