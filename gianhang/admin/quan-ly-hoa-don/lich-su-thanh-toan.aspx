<%@ Page Title="Lịch sử thanh toán" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="lich-su-thanh-toan.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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
                                <label class="fw-600">Ngành</label>
                                <asp:DropDownList ID="DropDownList5" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Hình thức giao dịch</label>
                                <asp:DropDownList ID="ddl_loc_thanhtoan" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Tiền mặt" Value="Tiền mặt"></asp:ListItem>
                                    <asp:ListItem Text="Chuyển khoản" Value="Chuyển khoản"></asp:ListItem>
                                    <asp:ListItem Text="Quẹt thẻ" Value="Quẹt thẻ"></asp:ListItem>
                                 
                                    <asp:ListItem Text="Voucher giấy" Value="Voucher giấy"></asp:ListItem>
                                    <asp:ListItem Text="E-Voucher (điểm)" Value="E-Voucher (điểm)"></asp:ListItem>
                                    <asp:ListItem Text="Ví điện tử" Value="Ví điện tử"></asp:ListItem>
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
                                    <asp:ListItem Text="Ngày thanh toán (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Ngày thanh toán (Giảm dần)" Value="1"></asp:ListItem>
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

    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <%--<div data-role="panel"
                    data-title-caption="Thống kê thanh toán"
                    data-title-icon="<span class='mif-calendar'></span>"
                    data-collapsible="true">
                    <div class="bg-white">
                       

                    </div>
                </div>--%>
                <div class="row">
                    <div class="cell-lg-4 cell-sm-6 text-center bg-red fg-white p-4">
                        <div class="text-bold"><%=tongcong.ToString("#,##0") %></div>
                        <div class="text-upper"><b>TỔNG THANH TOÁN</b></div>
                    </div>
                    <div class="cell-lg-4 cell-sm-6 text-center bg-orange	fg-white   p-4">
                        <div class="text-bold"><%=tienmat.ToString("#,##0") %></div>
                        <div class="text-upper">TIỀN MẶT</div>
                    </div>
                    <div class="cell-lg-4 cell-sm-6 text-center  bg-amber fg-white  p-4">
                        <div class="text-bold"><%=chuyenkhoan.ToString("#,##0") %></div>
                        <div class="text-upper">CHUYỂN KHOẢN</div>
                    </div>
                    <div class="cell-lg-4 cell-sm-6 text-center  bg-brown fg-white  p-4">
                        <div class="text-bold"><%=quetthe.ToString("#,##0") %></div>
                        <div class="text-upper">QUẸT THẺ</div>
                    </div>
                  
                    <div class="cell-lg-4 cell-sm-6 text-center  bg-steel fg-white  p-4">
                        <div class="text-bold"><%=vouchergiay.ToString("#,##0") %></div>
                        <div class="text-upper">Voucher giấy</div>
                    </div>
                    <div class="cell-lg-4 cell-sm-6 text-center  bg-pink fg-white  p-4">
                        <div class="text-bold"><%=voucherdiem.ToString("#,##0") %></div>
                        <div class="text-upper">E-Voucher (điểm)</div>
                    </div>
                    <div class="cell-lg-4 cell-sm-6 text-center  bg-mauve fg-white  p-4">
                        <div class="text-bold"><%=vidientu.ToString("#,##0") %></div>
                        <div class="text-upper">VÍ ĐIỆN TỬ</div>
                    </div>
                </div>

                <%-- <div class="row">
                    <div class="cell-lg-12 mt-2">
                        <div class="icon-box border bd-red">
                            <div class="icon bg-red fg-white"><span class="mif-paypal"></span></div>
                            <div class="content p-3">
                                <div class="mt-1">
                                    <small class="d-inline fg-darkGray">Tièn mặt<span class="place-right">100.000</span></small>
                                </div>
                                <div class="mt-0">
                                    <small class="d-inline fg-darkGray">Chuyển khoản<span class="place-right">100.000</span></small>
                                </div>
                               <div class="mt-0">
                                    <small class="d-inline fg-darkGray">Quẹt thẻ<span class="place-right">100.000</span></small>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>--%>

                <div class="row mt-4<%--mt-0-lg-minus mt-12-minus--%>">
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
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <%if (bcorn_class.check_quyen(user, "q7_6") == ""||bcorn_class.check_quyen(user, "n7_6") == "")
                                    { %>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li>
                                    <asp:ImageButton ID="but_xoa_thanhtoan" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_thanhtoan_Click" /><%--OnClientClick="return confirm('Bạn đã chắc chắn chưa?');"--%>
                                </li>
                                <%} %>
                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                </div>

                <div id="table-main">
                    <div style="overflow: auto" class=" mt-4">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                            <tbody>

                                <tr style="background-color: #ecf0f5">

                                    <td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>
                                    <td class="text-bold" style="width: 1px;">Thời gian</td>
                                    <td class="text-bold text-center" style="min-width: 90px; width: 90px">Mã đơn</td>
                                    <td class="text-bold" style="min-width: 140px">Khách hàng</td>
                                    <%--<td class="text-bold" style="min-width: 140px">Địa chỉ</td>--%>
                                    <td class="text-bold" style="min-width: 1px; width: 1px">Điện thoại</td>
                                    <td class="text-bold" style="width: 124px; min-width: 124px">Hình thức</td>
                                    <td class="text-bold text-right" style="width: 80px; min-width: 80px">Số tiền</td>
                                    <td class="text-bold" style="min-width: 190px">Nhân viên xác nhận</td>
                                </tr>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>

                                            <td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                            </td>
                                            <td class="text-right">
                                                <%#Eval("thoigian","{0:dd/MM/yyyy HH:mm}").ToString() %>
                                            </td>
                                            <td class="text-bold text-center"><a data-role="hint" data-hint-position="top" data-hint-text="Xem hóa đơn" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id_hoadon") %>"><%#Eval("id_hoadon") %></a></td>
                                            <td><%#Eval("khachhang") %></td>
                                            <%--<td><%#Eval("diachi") %></td>--%>
                                            <td><a class="fg-black" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></td>
                                            <td><%#Eval("hinhthuc") %></td>
                                            <td class="text-bold text-right">
                                                <%#Eval("sotien","{0:#,##0}") %>
                                            </td>
                                            <td>
                                                <a class="fg-black" title="Xem tài khoản" href="/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=<%#Eval("nguoithanhtoan") %>"><%#Eval("nguoithanhtoan") %></a>
                                            </td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>

                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="6" class="text-right text-leader text-bold fg-red text-size-20">Tổng cộng</td>
                                    <td class="text-leader text-right text-bold fg-red text-size-20"><%=tongcong.ToString("#,##0") %></td>
                                    <td></td>
                                </tr>
                                <tr class="text-bold">
                                    <td colspan="7" class="text-right fg-red">Số tiền bằng chữ: <%=tienbangchu %> đồng.</td>
                                    <td></td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                    <div class="text-center mt-8 mb-20" <%--style="margin-top: -70px!important"--%>>
                        <asp:Button ID="but_quaylai" runat="server" Text="Lùi" CssClass="" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Tiếp" CssClass="" OnClick="but_xemtiep_Click" />
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
    <%--<%=notifi %>--%>

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
