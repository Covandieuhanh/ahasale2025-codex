<%@ Page Title="Chi tiết nhập vật tư" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="chi-tiet-nhap-hang.aspx.cs" Inherits="badmin_Default" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .info-hoadon {
            font-weight: 600
        }

        .text-size-20 {
            font-size: 20px !important
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">



    <div id='form_thanhtoan' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_thanhtoan()' title='Đóng'>
                    <span class='mif mif-cross mif-lg fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thanh toán</h5>
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel4" runat="server" DefaultButton="but_thanhtoan">
                            <div class="mt-7">
                                Ngày
                            </div>
                            <div>
                                <asp:TextBox ID="txt_ngaythanhtoan" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Hình thức giao dịch</label>
                                <asp:DropDownList ID="ddl_hinhthuc_thanhtoan" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tiền mặt" Value="Tiền mặt"></asp:ListItem>
                                    <asp:ListItem Text="Chuyển khoản" Value="Chuyển khoản"></asp:ListItem>
                                    <asp:ListItem Text="Quẹt thẻ" Value="Quẹt thẻ"></asp:ListItem>
                                    <asp:ListItem Text="Voucher giấy" Value="Voucher giấy"></asp:ListItem>
                                    <asp:ListItem Text="E-Voucher (điểm)" Value="E-Voucher (điểm)"></asp:ListItem>
                                    <asp:ListItem Text="Ví điện tử" Value="Ví điện tử"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Nhập số tiền thanh toán</label>
                                <asp:TextBox ID="txt_sotien_thanhtoan_congno" data-role="input" runat="server" data-clear-button="true" onchange="format_sotien(this);"></asp:TextBox>
                            </div>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right" onclick='show_hide_id_form_thanhtoan()'>
                                    <asp:Button ID="but_thanhtoan" runat="server" Text="THANH TOÁN" CssClass="button success" OnClick="but_thanhtoan_Click" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_thanhtoan() {
            var x = document.getElementById("form_thanhtoan");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <%if (bcorn_class.check_quyen(user, "q13_5") == "")
        { %>
    <div id='form_edithoadon' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_edithoadon()' title='Đóng'>
                    <span class='mif mif-cross mif-lg fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Chỉnh sửa hóa đơn</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel3" runat="server" DefaultButton="but_form_edithoadon">
                            <div class="mt-0 fw-600">
                                Ngày
                            </div>
                            <div>
                                <asp:TextBox ID="txt_ngaytao" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                            </div>


                            <div class="mt-3 fw-600">Nhà cung cấp</div>
                            <div>
                                <asp:DropDownList ID="DropDownList3" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                            </div>
                            <div class="mt-3 fw-600">Ghi chú</div>
                            <div>
                                <asp:TextBox ID="txt_ghichu" runat="server" data-role="input" Width="100%"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Chiết khấu hóa đơn</label>
                                <span class="place-right">
                                    <asp:RadioButton ID="ck_hd_phantram" runat="server" Text="%" GroupName="ck_hd" />
                                    <asp:RadioButton ID="ck_hd_tienmat" runat="server" Text="Tiền" GroupName="ck_hd" />
                                </span>
                                <asp:TextBox ID="txt_chietkhau_hoadon" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="but_form_edithoadon" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_form_edithoadon_Click" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_edithoadon() {
            var x = document.getElementById("form_edithoadon");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>

    <div id='form_themdvsp' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 1110px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_themdvsp()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thêm vật tư</h5>
                <ul data-role="tabs" data-expand="true">
                    <li><a href="#themsanpham">Thông tin vật tư</a></li>
                </ul>

                <div class="border bd-default no-border-top p-2 pl-4 pr-4">

                    <div id="themsanpham">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel2" runat="server" DefaultButton="but_form_themsanpham">
                                    <div class="row ">
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="">
                                                <label class="fw-600">Ngày nhập</label>

                                                <asp:TextBox ID="txt_ngayban_sanpham" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Sản phẩm</label>
                                                <%--<asp:DropDownList ID="ddl_sanpham" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_sanpham_SelectedIndexChanged"></asp:DropDownList>--%>
                                                <asp:TextBox ID="txt_tensanpham" runat="server" data-role="input" placeholder="Nhập và chọn tên sản phẩm"></asp:TextBox></div>
                                            <div class="mt-3">
                                                <label class="fw-600">Đơn giá</label>
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
                                                <label class="fw-600">Ngày nhập</label>
                                                <asp:TextBox ID="txt_nsx" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Bảo hành</label>
                                                <asp:TextBox ID="txt_hsd" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Số lô</label>
                                                <asp:TextBox ID="txt_solo" data-role="input" runat="server" Text=""></asp:TextBox>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Đơn vị tính</label>
                                                <asp:TextBox ID="txt_dvt" data-role="input" runat="server" Text=""></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mt-6 mb-6 text-right">
                                        <div style="float: left">
                                            <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                        </div>
                                        <div style="float: right">
                                            <asp:Button ID="but_form_themsanpham" runat="server" Text="NHẬP THÊM VẬT TƯ" CssClass="button success" OnClick="but_form_themsanpham_Click" />
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
                </div>

            </div>
        </div>
    </div>
    <script>
        function show_hide_id_form_themdvsp() {
            var x = document.getElementById("form_themdvsp");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>

                <asp:AsyncPostBackTrigger ControlID="but_form_themsanpham" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="but_form_edithoadon" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="but_thanhtoan" EventName="Click" />
            </Triggers>
            <ContentTemplate>

                <%--NỘI DUNG CHÍNH--%>

                <div class="row">
                    <div class="cell-lg-6 pr-3-lg mt-3">
                        <div data-role="panel"
                            data-title-caption="Mã đơn: <b><%=id %></b>"
                            data-title-icon="<a data-role='hint' data-hint-position='top' data-hint-text='Quay lại' href='/gianhang/admin/quan-ly-vat-tu/vat-tu-da-nhap.aspx'><span class='mif-arrow-left'></span></a>"
                            data-collapsible="true">
                            <div class="bg-white pl-2 pr-2">
                                <%--TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>
                                <div>
                                    <div style="float: left" class="">
                                        <%--<h4>Danh sách hóa đơn</h4>--%>
                                        <ul class="h-menu ">
                                            <%if (bcorn_class.check_quyen(user, "q13_5") == "")
                                                { %>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa"><a class="button" onclick='show_hide_id_form_edithoadon()'><span class="mif mif-pencil"></span></a></li>
                                            <%} %>
                                            <%--<li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="In bill"><a class="button" target="_blank" href="/gianhang/admin/quan-ly-hoa-don/in-bill.aspx?id=<%=id %>">
                                                <span class="mif mif-printer pr-1"></span>Bill</a></li>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="In A5"><a class="button" target="_blank" href="/gianhang/admin/quan-ly-hoa-don/in-a5.aspx?id=<%=id %>">
                                                <span class="mif mif-printer pr-1"></span>A5</a></li>--%>
                                            <%if (bcorn_class.check_quyen(user, "q13_6") == "")
                                                { %>
                                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="Xóa đơn">
                                                <asp:ImageButton ID="but_xoahoadon" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoahoadon_Click" OnClientClick="return confirm('Bạn đã chắc chắn chưa?');" />
                                            </li>
                                            <%} %>
                                            <%--<li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                            <li>
                                                <a class="c-pointer js-textareacopybtn<%=id_guide %>" onclick="show_saochep();"><span class="mif mif-link" data-role="hint" data-hint-position="top" data-hint-text="Sao chép link hóa đơn"></span></a>

                                            </li>--%>
                                            <%--<li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel"><a href="#"><span class="mif mif-file-excel"></span></a></li>--%>
                                        </ul>
                                       
                                    </div>
                                    <div style="float: right" class="">
                                        <ul class="h-menu ">
                                        </ul>
                                    </div>
                                    <div class="clr-float"></div>
                                </div>
                                <%--END TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>
                                <hr />
                                <div class="mt-3">
                                    <h5 class="pl-2 pr-2 fg-orange"><%=ten_kh %></h5>
                                    <table class="table row-hover <%--striped--%> subcompact mt-3">
                                        <tbody>
                                            <tr>
                                                <td style="width: 110px">Ngày nhập:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=ngaytao %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 110px">Người tạo:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=nguoitao %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 110px">Nhà cung cấp:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=ncc %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 110px">Ghi chú:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=ghichu_kh %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 110px">Tổng tiền:
                                                </td>
                                                <td class="info-hoadon"><%=tongtien %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 110px">Chiết khấu:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=ck_hoadon %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="text-bold" style="width: 110px">Sau CK:
                                                </td>
                                                <td class="text-bold"><%=sauck.ToString("#,##0") %>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <hr />
                                    <div class="pl-2 pr-2">
                                        <small class="fg-red">
                                            <%=km1_ghichu %>
                                        </small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="cell-lg-6 pl-3-lg mt-3">
                        <div data-role="panel"
                            data-title-caption="Thanh toán"
                            data-title-icon="<span class='mif-paypal'></span>"
                            data-collapsible="true">
                            <div class="bg-white pl-2 pr-2">
                                <div>
                                    <div style="float: left" class="">
                                        <ul class="h-menu ">
                                            <%if (bcorn_class.check_quyen(user, "q13_8") == "")
                                                { %>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="Thanh toán"><a class="button" onclick='show_hide_id_form_thanhtoan()'><span class="mif mif-paypal"></span></a></li>
                                            <%} %>
                                            <%if (bcorn_class.check_quyen(user, "q13_8") == "")
                                                { %>
                                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="Xóa thanh toán">
                                                <asp:ImageButton ID="but_xoathanhtoan" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoathanhtoan_Click" />
                                            </li>
                                            <%} %>
                                        </ul>
                                    </div>
                                    <div style="float: right" class="">
                                        <ul class="h-menu ">
                                        </ul>
                                    </div>
                                    <div class="clr-float"></div>
                                </div>
                                <hr />
                                <table class="table row-hover striped subcompact mt-3">
                                    <tbody>
                                        <asp:Repeater ID="Repeater2" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td style="width: 1px">
                                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_lichsu_thanhtoan_<%#Eval("id").ToString() %>">
                                                    </td>

                                                    <%--<td style="width: 50px">Lần <%=stt_tt %></td>--%>
                                                    <td class="text-right" style="width: 118px"><%#Eval("thoigian","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                    <td style="min-width: 70px"><%#Eval("hinhthuc_thanhtoan").ToString() %></td>
                                                    <td class="text-right" style="width: 1px"><%#Eval("sotienthanhtoan","{0:#,##0}").ToString() %></td>
                                                </tr>
                                                <%--<%stt_tt = stt_tt + 1; %>--%>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                                <div class="p-2 text-right">
                                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <div>
                    <h5 class="pt-10">Chi tiết đơn nhập vật tư</h5>

                    <div class="row">
                        <div class="cell-md-6 order-2 order-md-1 mt-0">
                            <div class="d-flex flex-align-center gap-2">
                                <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm vật tư theo tên"></asp:TextBox>
                                <asp:LinkButton ID="but_search" runat="server" CssClass="button" OnClick="but_search_Click" CausesValidation="false">
                                    <span class="mif mif-search"></span>
                                </asp:LinkButton>
                            </div>

                        </div>
                        <div class="cell-md-6 order-1 order-md-2 mt-0">

                            <div class="place-right">
                                <ul class="h-menu">
                                    <%if (bcorn_class.check_quyen(user, "q13_5") == "")
                                        { %>
                                    <li data-role="hint" data-hint-position="top" data-hint-text="Thêm mặt hàng"><a class="button" onclick='show_hide_id_form_themdvsp()'><span class="mif mif-plus"></span></a></li>

                                    <%} %>
                                    <%if (bcorn_class.check_quyen(user, "q13_5") == "")
                                        { %>
                                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                    <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                        <asp:ImageButton ID="but_xoa" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_Click" />
                                    </li>
                                    <%} %>
                                </ul>
                            </div>
                            <div class="clr-float"></div>

                        </div>
                    </div>

                    <%--TABLE CHÍNH--%>
                    <div id="table-main">
                        <div style="overflow: auto" class=" mt-3">
                            <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                <thead>
                                    <tr style="background-color: #f5f5f5">
                                        <%--<td style="width: 1px;" class=" text-bold text-center">#</td>--%>
                                        <td style="width: 1px;">
                                            <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </td>
                                        <td class="text-bold" style="width: 110px;">Ngày nhập</td>
                                        <td class="text-bold" style="width: 60px;">Số lô</td>
                                        <%--<td class="text-bold text-center" style="width: 50px; min-width: 50px">Ảnh</td>--%>
                                        <td class="text-bold" style="min-width: 150px">Mặt hàng</td>

                                        <td class="text-bold text-center" style="width: 100px;">Đơn giá</td>
                                        <td class="text-bold" style="width: 1px;">SL</td>
                                        <td class="text-bold" style="width: 1px;">ĐVT</td>
                                        <td class="text-bold text-right" style="width: 102px; min-width: 102px">Thành tiền</td>
                                        <td class="text-bold text-right" style="width: 1px;">CK</td>
                                        <td class="text-bold text-right" style="width: 60px;">Sau CK</td>
                                        <td class=" text-bold" style="width: 1px">Bảo hành</td>
                                       
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
                                                <td class="text-right">
                                                    <div><%#Eval("ngayban","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                </td>
                                                <td class="text-center">
                                                    <%#Eval("solo").ToString() %>
                                                </td>
                                                <%--<td>
                                                    <a href="/gianhang/admin/quan-ly-hoa-don/edit-cthd.aspx?id=<%#Eval("id") %>">
                                                        <img src="<%#Eval("hinhanh") %>" class="img-cover-vuong w-h-50" style="max-width: none!important">
                                                    </a>
                                                </td>--%>
                                                <td>
                                                    <a href="/gianhang/admin/quan-ly-vat-tu/edit-cthd.aspx?id=<%#Eval("id") %>" data-role='hint' data-hint-position='top' data-hint-text='Nhấn để sửa'>
                                                        <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                            <span class="fg-blue"><%#Eval("ten_dichvu_sanpham").ToString() %></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("kyhieu").ToString()=="sanpham" %>'>
                                                            <span class="fg-green"><%#Eval("ten_dichvu_sanpham").ToString() %></span>
                                                        </asp:PlaceHolder>
                                                    </a>
                                                </td>
                                                <td class="text-right">
                                                    <%#Eval("gia","{0:#,##0}").ToString() %>                                       
                                                </td>
                                                <td class="text-right">
                                                    <%#Eval("soluong","{0:#,##0}").ToString() %>
                                                </td>
                                                <td class="text-center">
                                                    <%#Eval("dvt").ToString() %>
                                                </td>
                                                <td class="text-right">
                                                    <div><%#Eval("thanhtien","{0:#,##0}").ToString() %> </div>
                                                </td>
                                                <td class="text-right">
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("chietkhau").ToString()=="0" %>'>
                                                        <%#Eval("tongtien_ck","{0:#,##0}").ToString() %>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("chietkhau").ToString()!="0" %>'>
                                                        <%#Eval("chietkhau")%>%
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-right">
                                                    <div><b><%#Eval("sauck","{0:#,##0}").ToString() %></b></div>
                                                </td>
                                                <td class="text-right">
                                                    <div><%#Eval("nsx","{0:dd/MM/yyyy}").ToString() %></div>
                                                    <div><%#Eval("hsd","{0:dd/MM/yyyy}").ToString() %></div>
                                                </td>
                                               
                                            </tr>
                                            <%--  <%stt = stt + 1; %>--%>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                                <tfoot>
                                    <tr class=" text-bold">
                                        <td colspan="9" class="text-right">Tổng cộng</td>
                                        <td class="text-right"><%=tongtien %></td>
                                        
                                    </tr>
                                    <tr class="text-bold">
                                        <td colspan="9" class="text-right">Chiết khấu hóa đơn</td>
                                        <td class="text-right">
                                            <%=ck_hoadon %></td>
                                       
                                    </tr>
                                    <tr class="">
                                        <td colspan="9" class="text-right text-leader text-bold fg-red text-size-20">Sau chiết khấu</td>
                                        <td class="text-leader text-right text-bold fg-red text-size-20"><%=sauck.ToString("#,##0") %></td>
                                       
                                    </tr>
                                    <tr class="text-bold">
                                        <td colspan="10" class="text-right fg-red">Số tiền bằng chữ: <%=tienbangchu %> đồng.</td>
                                       
                                    </tr>
                                </tfoot>
                            </table>
                        </div>

                    </div>
                    <%--END TABLE CHÍNH--%>
                </div>


                <%--END DUNG CHÍNH--%>
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
    <%--<script>
        function show_saochep() {
            Metro.notify.create("Sao chép link hóa đơn thành công.", "Thông báo", {});
        }
    </script>--%>
    <script src="/js/gianhang-invoice-fast.js?v=20260326a"></script>
    <script>
        (function () {
            function bindFastUi() {
                if (!window.ahaInvoiceFast) return;
                window.ahaInvoiceFast.initSearchSubmit({
                    inputId: "<%=txt_search.ClientID %>",
                    buttonId: "<%=but_search.ClientID %>"
                });
                window.ahaInvoiceFast.initItemLookup({
                    endpoint: "/gianhang/admin/quan-ly-vat-tu/lookup-data.ashx",
                    mode: "supply-item",
                    inputId: "<%=txt_tensanpham.ClientID %>",
                    priceId: "<%=txt_gia_sanpham.ClientID %>",
                    unitId: "<%=txt_dvt.ClientID %>"
                });
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
</asp:Content>
