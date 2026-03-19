<%@ Page Title="Doanh số nhân viên" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="doanh-so-nhan-vien.aspx.cs" Inherits="badmin_quan_ly_menu_Default" %>

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
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
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
                                <div class="fw-600">Trạng thái</div>
                                <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Đang hoạt động" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Đã bị khóa" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Tất cả" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Ngành</label>
                                <asp:DropDownList ID="DropDownList5" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
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
                                    <asp:ListItem Text="Tiền chốt sale (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Tiền chốt sale (Giảm dần)" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Tiền làm dịch vụ (Tăng dần)" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Tiền làm dịch vụ (Giảm dần)" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Tổng doanh số (Tăng dần)" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Tổng doanh số (Giảm dần)" Value="5"></asp:ListItem>
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

    <div id='form_excel' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_excel()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>
            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Xuất excel</h5>
                <hr />
                <asp:Panel ID="Panel4" runat="server" DefaultButton="Button5">
                    
                    <script type="text/javascript">
                        function checkAllCheckBoxes() {
                            var checkBoxList = document.getElementById('<%= check_list_excel.ClientID %>');
                            var checkBoxes = checkBoxList.getElementsByTagName('input');

                            for (var i = 0; i < checkBoxes.length; i++) {
                                if (checkBoxes[i].type == 'checkbox') {
                                    checkBoxes[i].checked = true;
                                }
                            }
                        }
                    </script>
                    <div class="mt-3">
                        <div class="fw-600">Chọn mục muốn xuất</div>
                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checklist_excel input[type=checkbox]').prop('checked', this.checked)" />Chọn tất cả
                        <asp:CheckBoxList ID="check_list_excel" runat="server" CssClass="checklist_excel">
                            <asp:ListItem Text="Tài khoản" Value="taikhoan"></asp:ListItem>
                            <asp:ListItem Text="Họ tên" Value="hoten"></asp:ListItem>
                            <asp:ListItem Text="Ngành" Value="tennganh"></asp:ListItem>
                            <asp:ListItem Text="Trạng thái" Value="trangthai"></asp:ListItem>
                            <asp:ListItem Text="Điện thoại" Value="sdt"></asp:ListItem>
                            <asp:ListItem Text="Doanh số chốt sale" Value="tongchot"></asp:ListItem>
                            <asp:ListItem Text="Doanh số làm dịch vụ" Value="tonglam"></asp:ListItem>
                            <asp:ListItem Text="Doanh số bán thẻ" Value="tongbanthe"></asp:ListItem>
                            <asp:ListItem Text="Tổng doanh số" Value="tongcong"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                    <div class="mt-3 fg-red">
                        <small>Nhấn nút XUẤT EXCEL 1 lần và chờ đến khi hệ thống xử lý xong.</small>
                    </div>
                    <hr />
                    <div class="mt-6 mb-10">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button5" runat="server" Text="XUẤT EXCEL" CssClass="button success" OnClick="Button5_Click" />
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
        function show_hide_id_form_excel() {
            var x = document.getElementById("form_excel");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>



    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />

            </Triggers>
            <ContentTemplate>
                <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
                    <div class="cell-md-6 order-2 order-md-1 mt-0">
                        <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm" OnTextChanged="txt_search_TextChanged" AutoPostBack="true"></asp:TextBox>
                    </div>
                    <div class="cell-md-6 order-1 order-md-2 mt-0">

                        <div class="place-right">
                            <ul class="h-menu">
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel" onclick="show_hide_id_form_excel()">
                                    <a class="button"><span class="mif mif-file-excel"></span></a></li>
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

                                    <td class=" text-bold" style="width: 50px;">Ảnh</td>
                                    <td class=" text-bold " style="min-width: 120px;">Họ tên</td>
                                    <td class=" text-bold " style="width: 120px;">Ngành</td>
                                    <td class=" text-bold " style="width: 1px; min-width: 1px;">Liên hệ</td>
                                    <td class="text-bold" style="width: 112px; min-width: 112px">Chốt sale</td>
                                    <td class="text-bold" style="width: 112px; min-width: 112px">Làm dịch vụ</td>
                                    <td class="text-bold" style="width: 112px; min-width: 112px">Bán thẻ DV</td>
                                    <td class="text-bold" style="width: 112px; min-width: 112px">Tổng cộng</td>
                                    <td class=" text-bold " style="width: 1px; min-width: 1px;"></td>
                                    <%--<td style="width: 1px;"></td>--%>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <a href="/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=<%#Eval("taikhoan").ToString() %>" data-role="hint" data-hint-position="top" data-hint-text="Xem tài khoản">
                                                    <img src="<%#Eval("avt") %>" class="img-cover-vuongtron w-h-50" style="max-width: none!important" />
                                                </a>
                                            </td>
                                            <td>
                                                <asp:PlaceHolder ID="PlaceHolder14" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đang hoạt động" %>'>
                                                    <a class="fg-black fg-blue-hover" href="/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=<%#Eval("taikhoan").ToString() %>" data-role="hint" data-hint-position="top" data-hint-text="<%#Eval("taikhoan") %>">
                                                        <%#Eval("hoten") %>
                                                    </a>
                                                    <div>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đang hoạt động" %>'>
                                                            <span class="data-wrapper"><code class="bg-cyan fg-white">Đang hoạt động</code></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã bị khóa" %>'>
                                                            <span class="data-wrapper"><code class="bg-red fg-white">Đã bị khóa</code></span>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã bị khóa" %>'>
                                                    <a class="fg-red fg-darkRed-hover" href="/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=<%#Eval("taikhoan").ToString() %>">
                                                        <span class="fg-red"><%#Eval("taikhoan") %></span>
                                                        <div class="fg-red"><%#Eval("hoten") %></div>
                                                    </a>
                                                </asp:PlaceHolder>

                                            </td>
                                            <td><%#Eval("tennganh").ToString()%></td>
                                            <td>
                                                <div><a class="fg-black" title="Nhấn để gọi" href="tel:<%#Eval("sdt") %>"><%#Eval("sdt") %></a></div>

                                            </td>
                                            <td class="text-right  fg-cyan">
                                                <%#Eval("tongchot","{0:#,##0}") %>
                                            </td>
                                            <td class="text-right fg-green">
                                                <%#Eval("tonglam","{0:#,##0}") %>
                                            </td>
                                            <td class="text-right fg-orange">
                                                <%#Eval("tongbanthe","{0:#,##0}") %>
                                            </td>

                                            <td class="text-right fg-red">
                                                <b><%#Eval("tongcong","{0:#,##0}") %></b>
                                            </td>
                                            <td>
                                                <a href="/gianhang/admin/quan-ly-tai-khoan/chi-tiet-doanh-so.aspx?user=<%#Eval("taikhoan").ToString() %>" class="button success small">Chi tiết
                                                </a>
                                            </td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="4"></td>
                                    <td class="text-right text-bold fg-cyan"><%=tongchot.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-green"><%=tonglam.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-orange"><%=tongbanthe.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-red"><%=tongcong.ToString("#,##0") %></td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                    <div class="text-center mt-8 mb-20">
                        <asp:Button ID="but_quaylai" runat="server" Text="Lùi" CssClass="" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Tiếp" CssClass="" OnClick="but_xemtiep_Click" />
                    </div>
                </div>
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



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

