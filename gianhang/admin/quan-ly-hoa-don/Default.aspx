<%@ Page Title="Quản lý hóa đơn" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="badmin_Default" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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
                                <label class="fw-600">Ngành</label>
                                <asp:DropDownList ID="DropDownList5" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Thanh toán</label>
                                <asp:DropDownList ID="ddl_locdulieu" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn còn nợ" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn đã thanh toán" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Dịch vụ - Sản phẩm</label>
                                <asp:DropDownList ID="ddl_loc2" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn chỉ riêng dịch vụ" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn có chứa dịch vụ" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn chỉ riêng sản phẩm" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Lọc ra những đơn có chứa sản phẩm" Value="4"></asp:ListItem>
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
                                    <asp:ListItem Text="Ngày tạo (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Giảm dần)" Value="1"></asp:ListItem>
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


    <%if (bcorn_class.check_quyen(user, "q7_2") == "" || bcorn_class.check_quyen(user, "n7_2") == "")
        { %>
    <div id='form_2' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: <%=show_add%>; background-image: url('/uploads/images/bg1.png');">
        <div class='admin-form-shell-wide' style='top: 0; left: 0; margin: 0 auto; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_2()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Tạo hóa đơn</h5>
                <% if (!string.IsNullOrWhiteSpace(Request.QueryString["id_datlich"])) { %>
                <div class="mb-2 p-2 border bd-info bg-light">
                    Đang nhận dữ liệu từ lịch hẹn #<%=Request.QueryString["id_datlich"] %>.
                    <a class="fg-blue" href="/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=<%=Request.QueryString["id_datlich"] %>">Quay lại lịch hẹn</a>
                </div>
                <% } %>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="Button3">
                            <div class="row">
                                <div class="cell-lg-6 pr-2-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Ngành</label>
                                        <asp:DropDownList ID="DropDownList3" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày tạo</label>
                                        <asp:TextBox ID="txt_ngaytao" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Tên khách hàng</label>
                                        <asp:TextBox ID="txt_tenkhachhang" runat="server" data-role="input"></asp:TextBox></div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <%--<asp:TextBox ID="txt_sdt" runat="server" data-role="input" OnTextChanged="txt_sdt_TextChanged" AutoPostBack="true"></asp:TextBox>--%>
                                        <asp:TextBox ID="txt_sdt" runat="server" data-role="input"></asp:TextBox></div>
                                    <%--<div class="mt-3">
                                        <label class="fw-600">Ngày sinh</label>
                                        <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>--%>
                                    <div class="mt-3">
                                        <label class="fw-600">Địa chỉ</label>
                                        <asp:TextBox ID="txt_diachi" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox><%--autocomplete="off" --%>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ghi chú</label>
                                        <asp:TextBox ID="txt_ghichu" runat="server" data-role="input"></asp:TextBox><%--autocomplete="off" --%>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-2-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Nhân viên chăm sóc</label>
                                        <asp:DropDownList ID="ddl_nhanvien_chamsoc" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Mã giới thiệu</label>
                                        <asp:TextBox ID="txt_magioithieu" runat="server" data-role="input"></asp:TextBox><%--autocomplete="off" --%>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Nhóm khách hàng</label>
                                        <asp:DropDownList ID="DropDownList1" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <%--<div class="mt-3">
                                <label>Chiết khấu hóa đơn</label>
                                <span class="place-right">
                                    <asp:RadioButton ID="ck_hd_phantram" runat="server" Text="%" GroupName="ck_hd" Checked="true" />
                                    <asp:RadioButton ID="ck_hd_tienmat" runat="server" Text="Tiền" GroupName="ck_hd" />
                                </span>
                                <asp:TextBox ID="txt_chietkhau_hoadon" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox>
                            </div>--%>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button3" runat="server" Text="TẠO HÓA ĐƠN" CssClass="button success" OnClick="Button3_Click" />
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

        </div>
    </div>
    <script>
        function show_hide_id_form_2() {
            var x = document.getElementById("form_2");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>

    <%if (bcorn_class.check_quyen(user, "q7_4") == "" || bcorn_class.check_quyen(user, "n7_4") == "")
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
                        Các dữ liệu liên quan đến hóa đơn này sẽ bị xóa theo.<br />
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
                <asp:Panel ID="Panel4" runat="server" DefaultButton="Button4">
                    <div class="mt-3">
                        <div class="fw-600">Chọn mục muốn xuất</div>
                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checklist_excel input[type=checkbox]').prop('checked', this.checked)" />Chọn tất cả
                        <asp:CheckBoxList ID="check_list_excel" runat="server" CssClass="checklist_excel">
                            <asp:ListItem Text="Mã đơn" Value="id"></asp:ListItem>
                            <asp:ListItem Text="Ngày" Value="ngaytao"></asp:ListItem>
                            <asp:ListItem Text="Nguồn" Value="nguongoc"></asp:ListItem>
                            <asp:ListItem Text="Tên khách hàng" Value="tenkhachhang"></asp:ListItem>
                            <asp:ListItem Text="Số điện thoại" Value="sdt"></asp:ListItem>
                            <asp:ListItem Text="Số lượng dịch vụ" Value="sl_dv"></asp:ListItem>
                            <asp:ListItem Text="Doanh số dịch vụ" Value="tongtien_dichvu"></asp:ListItem>
                            <asp:ListItem Text="Số lượng sản phẩm" Value="sl_sp"></asp:ListItem>
                            <asp:ListItem Text="Doanh số sản phẩm" Value="tongtien_sanpham"></asp:ListItem>
                            <asp:ListItem Text="Tổng tiền" Value="tongtien"></asp:ListItem>
                            <asp:ListItem Text="Chiết khấu" Value="ck_hoadon"></asp:ListItem>
                            <asp:ListItem Text="Sau chiết khấu" Value="tongsauchietkhau"></asp:ListItem>
                            <asp:ListItem Text="Đã thanh toán" Value="sotien_dathanhtoan"></asp:ListItem>
                            <asp:ListItem Text="Công nợ" Value="sotien_conlai"></asp:ListItem>
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
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
                                <%if (bcorn_class.check_quyen(user, "q7_2") == "" || bcorn_class.check_quyen(user, "n7_2") == "")
                                    { %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Tạo hóa đơn" onclick="show_hide_id_form_2()">
                                    <a class="button"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <%} %>

                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                                <%if (bcorn_class.check_quyen(user, "q7_4") == "" || bcorn_class.check_quyen(user, "n7_4") == "")
                                    { %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                    <a class="button" onclick="show_hide_id_form_3()"><span class="mif mif-bin"></span></a>
                                </li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <%} %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel" onclick="show_hide_id_form_excel()">
                                    <a class="button"><span class="mif mif-file-excel"></span></a></li>
                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                </div>

                <div class="row mt-2">
                    <div class="cell-xxl-3 cell-lg-6 mt-2 pl-1-lg pr-1-lg">
                        <div class="icon-box border bd-cyan">
                            <div class="icon bg-cyan fg-white"><span class="mif-list"></span></div>
                            <div class="content p-3">
                                <div class="text-upper text-bold">Dịch vụ </div>
                                <div class="mt-1">
                                    <small class="d-inline fg-darkGray">Số lượng <span class="place-right"><%=dichvu_soluong.ToString("#,##0") %></span></small>
                                </div>
                                <div class="">
                                    <small class="d-inline fg-darkGray">Doanh số <span class="place-right"><%=doanhso_dichvu.ToString("#,##0") %></span></small>
                                </div>
                                <div class="">
                                    <small class="d-inline fg-darkGray">Sau chiết khấu <span class="place-right"><%=dichvu_sauck.ToString("#,##0") %></span></small>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="cell-xxl-3 cell-lg-6 mt-2  pl-1-lg pr-1-lg">
                        <div class="icon-box border bd-green">
                            <div class="icon bg-green fg-white"><span class="mif-shop"></span></div>
                            <div class="content p-3">
                                <div class="text-upper text-bold">Sản phẩm</div>
                                <div class="mt-1">
                                    <small class="d-inline fg-darkGray">Số lượng <span class="place-right"><%=sanpham_soluong.ToString("#,##0") %></span></small>
                                </div>
                                <div class="">
                                    <small class="d-inline fg-darkGray">Doanh số <span class="place-right"><%=doanhso_sanpham.ToString("#,##0") %></span></small>
                                </div>
                                <div class="">
                                    <small class="d-inline fg-darkGray">Sau chiết khấu <span class="place-right"><%=sanpham_sauck.ToString("#,##0") %></span></small>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="cell-xxl-3 cell-lg-6 mt-2 pl-1-lg pr-1-lg">
                        <div class="icon-box border bd-orange">
                            <div class="icon bg-orange fg-white"><span class="mif-open-book"></span></div>
                            <div class="content p-3">
                                <div class="text-upper">
                                    <span class=" text-bold">Hóa đơn</span>
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
                    <div class="cell-xxl-3 cell-lg-6 mt-2 pl-1-lg pr-1-lg">
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
                                <div>
                                    <a href="/gianhang/admin/quan-ly-hoa-don/lich-su-thanh-toan.aspx">
                                        <small class="d-inline fg-red">Xem lịch sử thanh toán</small>
                                    </a>
                                </div>
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
                                    <td class="text-bold" style="width: 1px;">Ngày</td>
                                    <td class="text-bold" style="min-width: 120px">Ngành</td>
                                    <td class="text-bold" style="min-width: 140px">Khách hàng</td>
                                    <td class="text-bold" style="width: 100px; min-width: 100px">Dịch vụ</td>
                                    <td class="text-bold" style="width: 100px; min-width: 100px">Sản phẩm</td>
                                    <td class="text-bold" style="width: 100px; min-width: 100px">Tổng tiền</td>
                                    <td class="text-bold" style="width: 1px; min-width: 1px">CK</td>
                                    <td class="text-bold" style="width: 100px; min-width: 100px">Sau CK</td>
                                    <td class="text-bold" style="width: 110px; min-width: 110px">Thanh toán</td>
                                    <td class="text-bold" style="width: 86px; min-width: 86px">Công nợ</td>
                                    <td style="width: 1px;"></td>
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
                                                <a class="fg-black" data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id").ToString() %>">
                                                    <b><%#Eval("id").ToString() %></b>
                                                </a>
                                                <div>
                                                    <span class="data-wrapper">
                                                        <code class="<%#Eval("source_badge_css") %>"><%#Eval("source_display") %></code>
                                                    </span>
                                                    <div class="fg-gray"><small><%#Eval("source_hint") %></small></div>
                                                    <asp:PlaceHolder ID="ph_workspace_order" runat="server" Visible='<%#Convert.ToBoolean(Eval("is_workspace_mirror")) %>'>
                                                        <div>
                                                            <a class="fg-orange" href="<%#Eval("native_order_url") %>">Mở đơn /gianhang</a>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </div>

                                            </td>
                                            <td class="text-right"><small><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></small></td>
                                            <td><%#Eval("tennganh").ToString() %></td>

                                            <td><%#Eval("tenkhachhang").ToString() %>
                                                <div><a class="fg-lightOrange" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></div>
                                            </td>
                                            
                                            <td class="text-center">
                                                <span class="data-wrapper"><code class="bg-cyan fg-white" style="min-width: 40px"><%#Eval("sl_dv","{0:#,##0}").ToString() %></code></span>
                                                <div><%#Eval("tongtien_dichvu","{0:#,##0}").ToString() %></div>
                                            </td>
                                            <td class="text-center">
                                                <span class="data-wrapper"><code class="bg-green fg-white" style="min-width: 40px"><%#Eval("sl_sp","{0:#,##0}").ToString() %></code></span>
                                                <div><%#Eval("tongtien_sanpham","{0:#,##0}").ToString() %></div>
                                            </td>
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
                                            <td>
                                                <ul class="h-menu bg-transparent">
                                                    <li>
                                                        <a href="#" <%-- class="dropdown-toggle"--%>><span class="mif mif-more-horiz"></span></a>
                                                        <ul class="d-menu place-right " data-role="dropdown">
                                                            <%--<li class="divider"></li>--%>
                                                            <li>
                                                                <a class="fg-black" target="_blank" href="/gianhang/admin/quan-ly-hoa-don/in-bill.aspx?id=<%#Eval("id").ToString() %>">In bill
                                                                </a>
                                                            </li>
                                                            <li>
                                                                <a class="fg-black" target="_blank" href="/gianhang/admin/quan-ly-hoa-don/in-a5.aspx?id=<%#Eval("id").ToString() %>">In A5
                                                                </a>
                                                            </li>
                                                            <li class="divider"></li>
                                                            <li>
                                                                <a class="js-textareacopybtn<%#Eval("id_guide") %>" onclick="show_saochep();">Sao chép link hóa đơn
                                                                </a>
                                                            </li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </td>
                                        </tr>
                                        <div style="opacity: 0; z-index: -999; position: absolute; bottom: 0px; left: 0">
                                            <input type="text" value="https://ahashine.vn/hoa-don-dien-tu.aspx?id=<%#Eval("id_guide") %>" class="js-copytextarea<%#Eval("id_guide") %>">
                                        </div>
                                        <script>        
                                            var copyTextareaBtn = document.querySelector('.js-textareacopybtn<%#Eval("id_guide") %>');

                                            copyTextareaBtn.addEventListener('click', function (event) {
                                                var copyTextarea = document.querySelector('.js-copytextarea<%#Eval("id_guide") %>');
                                                copyTextarea.focus();
                                                copyTextarea.select();
                                                try {
                                                    var successful = document.execCommand('copy');
                                                    var msg = successful ? 'successful' : 'unsuccessful';
                                                    console.log('Copying text command was ' + msg);
                                                } catch (err) {
                                                    console.log('Oops, unable to copy');
                                                }
                                            });
                                        </script>
                                        <%--<%stt = stt + 1; %>--%>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="5"></td>
                                    <td class="text-right text-bold fg-cyan"><%=tongrieng_dichvu.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-green"><%=tongrieng_sanpham.ToString("#,##0") %></td>
                                    <td class="text-right text-bold"><%=doanhso_hoadon.ToString("#,##0") %></td>
                                    <td></td>
                                    <td class="text-right text-bold"><%=doanhso_hoadon_sauck.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-red"><%=tongtien_dathanhtoan.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-orange"><%=tong_congno.ToString("#,##0") %></td>
                                    <td></td>
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
            <div style="position: fixed; top: 92px; right: 18px; z-index: 99999!important; display: inline-flex; align-items: center; gap: 12px; padding: 10px 16px; border-radius: 999px; background: rgba(15, 23, 42, 0.92); box-shadow: 0 12px 28px rgba(15, 23, 42, 0.24);">
                <div class="color-style activity-ring" data-role="activity" data-type="ring" data-style="color" data-small="true" data-role-activity="true"></div>
                <span class="fg-white">Đang tải dữ liệu...</span>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    </div>
</asp:Content>
<asp:Content ID="ContentFast" ContentPlaceHolderID="foot" runat="Server">
    <script src="/js/gianhang-invoice-fast.js?v=2026-03-26.2"></script>
    <script>
        (function () {
            function bindFastUi() {
                if (!window.ahaInvoiceFast) return;
                window.ahaInvoiceFast.initSearchSubmit({
                    inputId: "<%=txt_search.ClientID %>",
                    buttonId: "<%=but_search.ClientID %>"
                });
                window.ahaInvoiceFast.initCustomerLookup({
                    endpoint: "/gianhang/admin/quan-ly-hoa-don/lookup-data.ashx",
                    phoneId: "<%=txt_sdt.ClientID %>",
                    nameId: "<%=txt_tenkhachhang.ClientID %>",
                    addressId: "<%=txt_diachi.ClientID %>",
                    careId: "<%=ddl_nhanvien_chamsoc.ClientID %>",
                    groupId: "<%=DropDownList1.ClientID %>"
                });
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        function show_saochep() {
            Metro.notify.create("Sao chép link hóa đơn thành công.", "Thông báo", {});
        }
    </script>
    <%--<%=notifi %>--%>
</asp:Content>
