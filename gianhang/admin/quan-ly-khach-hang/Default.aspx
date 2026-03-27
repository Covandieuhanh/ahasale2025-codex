<%@ Page Title="Data khách hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="badmin_Default" %>


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
                                <label class="fw-600">Nhóm khách hàng</label>
                                <asp:DropDownList ID="DropDownList3" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Trạng thái nguồn</label>
                                <asp:DropDownList ID="ddl_trangthai_nguon" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Đang dùng khách hàng" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Đã ngừng dùng khách hàng" Value="2"></asp:ListItem>
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

    <%if (bcorn_class.check_quyen(user, "q8_2") == "")
        { %>
    <div id='form_2' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: <%=show_add%>; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 1200px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_2()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thêm khách hàng</h5>
                <%--<hr />--%>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="Button3">
                            <%--<ul data-role="tabs" data-expand="true">
                                <li><a href="#_chung">Chung</a></li>
                                <li><a href="#_benhly">Bệnh lý</a></li>
                                <li><a href="#_khac">Khác</a></li>
                            </ul>
                            <div class="">
                                <div id="_chung">
                                    
                                </div>
                            </div>--%>

                            <div class="row">
                                <div class="cell-lg-6 pr-2-lg">

                                    <div class="mt-3">
                                        <label class="fw-600">Tên khách hàng</label>
                                        <asp:TextBox ID="txt_tenkhachhang" runat="server" data-role="input"></asp:TextBox>

                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <%--<asp:TextBox ID="txt_sdt" runat="server" data-role="input" OnTextChanged="txt_sdt_TextChanged" AutoPostBack="true"></asp:TextBox>--%>

                                        <asp:TextBox ID="txt_sdt" runat="server" data-role="input"></asp:TextBox>
                                        <div class="mt-2 p-3 border bd-cyan bg-light" style="border-radius: 14px;">
                                            <div class="fw-700">Tự đưa vào Hồ sơ người</div>
                                            <div class="mt-1 fg-gray">
                                                Nếu khách hàng này có số điện thoại, sau khi lưu hệ thống sẽ tự nhận diện vào module <strong>Hồ sơ người</strong>. Việc liên kết tài khoản Home được thực hiện tập trung tại đó, không gắn trực tiếp ở form tạo khách hàng.
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày sinh</label>
                                        <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Địa chỉ</label>
                                        <asp:TextBox ID="txt_diachi" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox><%--autocomplete="off" --%>
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

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button3" runat="server" Text="THÊM KHÁCH HÀNG" CssClass="button success" OnClick="Button3_Click" />
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

    <div id='form_3' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_3()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>
            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Xuất excel</h5>
                <hr />
                <asp:Panel ID="Panel3" runat="server" DefaultButton="Button4">
                    <div class="mt-3">
                        <div class="fw-600">Chọn mục muốn xuất</div>
                        <asp:CheckBoxList ID="check_list_excel" runat="server">
                            <asp:ListItem Text="Tên khách hàng" Value="tenkhachhang"></asp:ListItem>
                            <asp:ListItem Text="Địa chỉ" Value="diachi"></asp:ListItem>
                            <asp:ListItem Text="Điện thoại" Value="sdt"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                    <hr />
                    <div class="mt-6 mb-10">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button4" runat="server" Text="XUẤT EXCEL" CssClass="button success" OnClick="Button4_Click" />
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


    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
                <div class="cell-md-6 order-2 order-md-1 mt-0">
                    <div class="d-flex flex-align-center">
                        <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm"></asp:TextBox>
                        <asp:LinkButton ID="but_search" runat="server" CssClass="button ml-2" OnClick="but_search_Click">
                            <span class="mif mif-search"></span>
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="cell-md-6 order-1 order-md-2 mt-0">

                    <div class="place-right">
                        <ul class="h-menu">
                            <%if (bcorn_class.check_quyen(user, "q8_2") == "")
                                { %>
                            <li data-role="hint" data-hint-position="top" data-hint-text="Thêm" onclick="show_hide_id_form_2()">
                                <a class="button"><span class="mif mif-plus"></span></a></li>
                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                            <%} %>
                            <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                <a class="button"><span class="mif mif-filter"></span></a></li>
                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                            <%if (bcorn_class.check_quyen(user, "q8_3") == "")
                                { %>
                            <li data-role="hint" data-hint-position="top" data-hint-text="Ngừng dùng">
                                <asp:Button ID="but_ngung" runat="server" Text="Ngừng dùng" CssClass="button warning" OnClick="but_ngung_Click" />
                            </li>
                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                            <li data-role="hint" data-hint-position="top" data-hint-text="Mở lại">
                                <asp:Button ID="but_molai" runat="server" Text="Mở lại" CssClass="button success" OnClick="but_molai_Click" />
                            </li>
                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                            <%} %>

                            <%if (bcorn_class.check_quyen(user, "q8_4") == "")
                                { %>
                            <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                <asp:ImageButton ID="but_xoa" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_Click" OnClientClick="return confirm('Bạn đã chắc chắn chưa?');" />
                            </li>
                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                            <%} %>
                            <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel" onclick="show_hide_id_form_3()">
                                <a class="button"><span class="mif mif-file-excel"></span></a></li>
                        </ul>
                    </div>
                    <div class="clr-float"></div>

                </div>
            </div>


            <div id="table-main">

                <div style="overflow: auto" class=" mt-4">
                    <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                        <tbody>
                            <tr style="background-color: #f5f5f5">

                                <td style="width: 1px;">
                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                </td>
                                <td class=" text-bold text-center" style="width: 50px;">Ảnh</td>
                                <td class="text-bold" style="min-width: 140px">Khách hàng</td>
                                <td class="text-bold" style="min-width: 220px">Liên kết Home</td>
                                <td class="text-bold" style="min-width: 150px">Trạng thái nguồn</td>
                                <td class="text-bold" style="width: 1px">Nhóm</td>
                                <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                <td style="width: 1px;"></td>
                                <%--<td class="text-bold text-center" style="min-width: 1px; width: 1px">Đơn</td>
                                <td class="text-bold text-center" style="min-width: 1px; width: 1px"><span data-role="hint" data-hint-position="top" data-hint-text="Dịch vụ">DV</span></td>
                                <td class="text-bold text-center" style="min-width: 1px; width: 1px"><span data-role="hint" data-hint-position="top" data-hint-text="Sản phẩm">SP</span></td>

                                <td class="text-bold text-right" style="width: 120px; min-width: 120px">Tổng chi tiêu</td>--%>
                                <%--<td class="text-bold text-right" style="width: 108px; min-width: 108px">Thanh toán</td>--%>
                                <%--<td class="text-bold text-right" style="width: 108px; min-width: 108px">Công nợ</td>--%>
                            </tr>
                            <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                                <ItemTemplate>
                                    <tr>

                                        <td class="checkbox-table">
                                            <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                        </td>
                                        <td>
                                            <a href="/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=<%#Eval("id").ToString() %>" data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết">
                                                <img src="<%#Eval("avt") %>" class="img-cover-vuongtron w-h-50" style="max-width: none!important" />
                                            </a>
                                        </td>
                                        <td>
                                            <a class="fg-black" href="/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=<%#Eval("id").ToString() %>" data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết">
                                                <%#Eval("tenkhachhang").ToString() %>
                                            </a>
                                            <div>
                                                <a data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a>
                                            </div>
                                            <div>
                                                <small><%#Eval("diachi").ToString() %></small>
                                            </div>
                                        </td>
                                        <td>
                                            <asp:Literal ID="litPersonHub" runat="server"></asp:Literal>
                                        </td>
                                        <td>
                                            <asp:Literal ID="litLifecycle" runat="server"></asp:Literal>
                                        </td>
                                        <td><%#Eval("tennhom").ToString() %></td>
                                        <td>
                                            <%#Eval("nguoitao").ToString() %>
                                            <div>
                                                <small><%#Eval("ngaytao","{0:dd/MM/yyyy HH:hh}").ToString() %></small>
                                            </div>
                                        </td>
                                        <td>

                                            <span class="data-wrapper"><code class="bg-green fg-white">
                                                <a class="fg-white" href="/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=<%#Eval("id").ToString() %>">Xem chi tiết </a></code></span>

                                        </td>
                                        <%--<td class="text-center text-bold">
                                            <%#Eval("sl_hoadon").ToString() %>
                                        </td>
                                        <td class="text-center">
                                            <span class="data-wrapper"><code class="bg-cyan fg-white"><%#Eval("sl_dv","{0:#,##0}").ToString() %></code></span>
                                        </td>
                                        <td class="text-center">
                                            <span class="data-wrapper"><code class="bg-green fg-white"><%#Eval("sl_sp","{0:#,##0}").ToString() %></code></span>
                                        </td>

                                        <td class="text-right">
                                            <%#Eval("sauck","{0:#,##0}").ToString() %>
                                        </td>--%>
                                        <%--<td class="text-right">
                                            <%#Eval("dathanhtoan","{0:#,##0}").ToString() %>
                                        </td>--%>
                                        <%--<td class="text-right">
                                            <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                <%#Eval("sotien_conlai","{0:#,##0}").ToString() %>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                <span class="data-wrapper"><code class="bg-orange fg-white"><%#Eval("sotien_conlai","{0:#,##0}").ToString() %></code></span>
                                            </asp:PlaceHolder>
                                        </td>--%>
                                    </tr>

                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                        <%-- <tfoot>
                            <tr class="">
                                <td colspan="3"></td>
                                <td class="text-center text-bold"><%=tongdon.ToString("#,##0") %></td>
                                <td class="text-center text-bold fg-cyan"><%=tong_sl_dv.ToString("#,##0") %></td>
                                <td class="text-center text-bold fg-green"><%=tong_sl_sp.ToString("#,##0") %></td>

                                <td class="text-right text-bold fg-red"><%=sauck.ToString("#,##0") %></td>
                              
                                <td class="text-right text-bold fg-orange"><%=tong_congno.ToString("#,##0") %></td>

                            </tr>
                        </tfoot>--%>
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
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div style="position: fixed; top: 92px; right: 18px; z-index: 99999!important; display: inline-flex; align-items: center; gap: 12px; padding: 10px 16px; border-radius: 999px; background: rgba(15, 23, 42, 0.92); box-shadow: 0 12px 28px rgba(15, 23, 42, 0.24);">
                <div class="color-style activity-ring" data-role="activity" data-type="ring" data-style="color" data-small="true" data-role-activity="true"></div>
                <span class="fg-white">Đang tải khách hàng...</span>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
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
