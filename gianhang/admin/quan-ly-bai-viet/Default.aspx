<%@ Page Title="Quản lý bài viết/Sản phẩm/Dịch vụ" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="badmin_quan_ly_menu_Default" %>

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
                                    <asp:ListItem Text="Bình thường" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Đã xóa" Value="1"></asp:ListItem>
                                    <%--<asp:ListItem Text="Tất cả" Value="2"></asp:ListItem>--%>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <div class="fw-600">Phân loại</div>
                                <asp:DropDownList ID="DropDownList3" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Tin tức" Value="ctbv"></asp:ListItem>
                                    <asp:ListItem Text="Sản phẩm" Value="ctsp"></asp:ListItem>
                                    <asp:ListItem Text="Dịch vụ" Value="ctdv"></asp:ListItem>
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
                                    <asp:ListItem Text="Tên bài viết (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Tên bài viết (Giảm dần)" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Tên menu (Tăng dần)" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Tên menu (Giảm dần)" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Tăng dần)" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Giảm dần)" Value="5"></asp:ListItem>
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

    <div id='form_2' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_2()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Xuất excel</h5>
                <hr />
                <asp:Panel ID="Panel2" runat="server" DefaultButton="Button4">
                    <div class="mt-3">
                        <div class="fw-600">Chọn mục muốn xuất</div>
                        <asp:CheckBoxList ID="check_list_excel" runat="server">
                            <asp:ListItem Text="Tên bài viết" Value="name"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                    <div class="mt-3 fg-red">
                        <small>Các mục khác đang được cập nhật</small>
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
        function show_hide_id_form_2() {
            var x = document.getElementById("form_2");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <%-- <div id='form_3' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_3()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Tùy chọn xóa</h5>
                <hr />
                <asp:Panel ID="Panel3" runat="server" DefaultButton="Button4">
                    <div class="mt-3 fg-red">
                        <small>Chức năng tùy chọn xóa đang được cập nhật.<br /> Hãy nhấn nút XÓA để xóa.</small>
                    </div>
                    <hr />
                    <div class="mt-6 mb-10">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button3" runat="server" Text="XÓA" CssClass="button alert" OnClick="but_del_Click" OnClientClick="show_hide_id_form_3()" />
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
    </script>--%>

    <div id='form_4' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_4()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Tùy chọn xóa vĩnh viễn</h5>
                <hr />
                <asp:Panel ID="Panel4" runat="server" DefaultButton="Button5">
                    <%--<div class="mt-3">
                        <div class="fw-600">Chọn mục muốn xuất</div>
                    </div>--%>
                    <div class="mt-3 fg-red">
                        <small>Dữ liệu sẽ không thể khôi phục sau khi xóa vĩnh viễn.<br />
                            Bạn đã chắc chắn chưa?
                        </small>
                    </div>
                    <hr />
                    <div class="mt-6 mb-10">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button5" runat="server" Text="XÁC NHẬN XÓA VĨNH VIỄN" CssClass="button alert" OnClick="but_xoavinhvien_Click" OnClientClick="show_hide_id_form_4()" />
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
        function show_hide_id_form_4() {
            var x = document.getElementById("form_4");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                <%--<asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />--%>
                <asp:AsyncPostBackTrigger ControlID="Button5" EventName="Click" />
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
                                <li data-role="hint" data-hint-position="top" data-hint-text="Thêm">
                                    <a class="button" href="/gianhang/admin/quan-ly-bai-viet/add.aspx"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lưu">
                                    <asp:ImageButton ID="but_luu" runat="server" ImageUrl="~/uploads/images/icon-button/but-save.png" OnClick="but_luu_Click" /></li>

                                <%if (Session["index_trangthai_baiviet"].ToString() != "1")
                                    { %>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xóa" <%--onclick="show_hide_id_form_3()"--%>>
                                    <%--<a class="button"><span class="mif mif-bin"></span></a>--%>
                                    <asp:ImageButton ID="but_del" runat="server" ImageUrl="~/uploads/images/icon-button/but-bin.png" OnClick="but_del_Click" />
                                </li>
                                <%} %>

                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                                <li data-role="hint" data-hint-position="top" data-hint-text="In">
                                    <a class="button"><span class="mif mif-print"></span></a>
                                    <ul class="d-menu place-right" data-role="dropdown">
                                        <li><a target="_blank" href="/gianhang/admin/quan-ly-bai-viet/in/a4.aspx">In A4</a></li>
                                    </ul>
                                </li>

                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel" onclick="show_hide_id_form_2()">
                                    <a class="button"><span class="mif mif-file-excel"></span></a></li>

                                <%if (Session["index_trangthai_baiviet"].ToString() == "1")
                                    { %><%--=1 là list trong thùng rác--%>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li>
                                    <a href="#" <%--class="dropdown-toggle"--%>><span class="mif mif-more-vert"></span></a>
                                    <ul class="d-menu place-right" data-role="dropdown">

                                        <li><a onclick="show_hide_id_form_4()">Xóa vĩnh viễn</a></li>
                                        <li><a runat="server" onserverclick="but_khoiphuc_Click">Khôi phục</a></li>
                                        <li class="divider"></li>

                                    </ul>
                                </li>
                                <%} %>
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

                                    <td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>
                                    <td style="width: 1px;" class=" text-bold text-center">ID</td>
                                    <td style="width: 50px;" class=" text-bold text-center">Ảnh</td>
                                    <td class=" text-bold " style="min-width: 200px">Tên
                                        
                                    </td>
                                    <td class=" text-bold" style="min-width: 100px">Menu
                                        
                                    </td>
                                    <td class=" text-bold" style="width: 92px">Phân loại</td>
                                    <td class=" text-bold" style="min-width: 120px">Chi tiết</td>
                                    <td class=" text-bold text-center" style="width: 80px">Nổi bật</td>
                                    <td class=" text-bold " style="width: 94px;">Ngày tạo</td>
                                    <td class=" text-bold text-center" style="width: 100px">Hiển thị trang chủ</td>
                                    <td class=" text-bold " style="width: 1px;">Ngành</td>
                                    <%--<td style="width: 1px;"></td>--%>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                            </td>

                                            <td class="text-center text-bold">
                                                <a href="/gianhang/admin/quan-ly-bai-viet/edit.aspx?id=<%#Eval("id") %>" data-role="hint" data-hint-position="right" data-hint-text="Nhấn để sửa"><%#Eval("id") %></a></td>
                                            <td>
                                                <a href="/gianhang/admin/quan-ly-bai-viet/edit.aspx?id=<%#Eval("id") %>">

                                                    <img src="<%#Eval("image") %>" class="img-cover-vuong w-h-50" style="max-width: none!important">
                                                </a>
                                            </td>
                                            <td>
                                                <a href="/gianhang/admin/quan-ly-bai-viet/edit.aspx?id=<%#Eval("id") %>" class="fg-black">
                                                    <asp:PlaceHolder ID="PlaceHolder14" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                        <span><%#Eval("name") %></span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                        <span class="fg-red"><%#Eval("name") %></span>
                                                    </asp:PlaceHolder>
                                                </a>
                                            </td>
                                            <td><%#Eval("tenmn") %></td>
                                            <td class="text-center">
                                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("phanloai").ToString() == "ctbv" %>'>
                                                    <span class="data-wrapper"><code class="bg-grayMouse fg-white">Tin tức</code></span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("phanloai").ToString() == "ctsp" %>'>
                                                    <span class="data-wrapper"><code class="bg-green fg-white">Sản phẩm</code></span>
                                                    <div><%#Eval("giasp","{0:#,##0}").ToString() %></div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("phanloai").ToString() == "ctdv" %>'>
                                                    <span class="data-wrapper"><code class="bg-cyan fg-white">Dịch vụ</code></span>
                                                    <div><%#Eval("giadv","{0:#,##0}").ToString() %></div>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td>
                                                <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("phanloai").ToString() == "ctsp" %>'>
                                                    <div><small>Chốt sale: <%#Eval("phantram_chotsale_sanpham").ToString() %>%</small></div>
                                                    <%--<div><small>SL tồn: <%#Eval("soluong_ton_sanpham","{0:#,##0}").ToString() %></small></div>--%>
                                                    <div><small>Giá vốn: <%#Eval("giavon_sp","{0:#,##0}").ToString() %></small></div>
                                                    <div><small>ĐVT: <%#Eval("dvt_sp","{0:#,##0}").ToString() %></small></div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("phanloai").ToString() == "ctdv" %>'>
                                                    <div><small>Chốt sale: <%#Eval("phantram_chotsale_dichvu").ToString() %>%</small></div>
                                                    <div><small>Thực hiện: <%#Eval("phantram_lamdichvu").ToString() %>%</small></div>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td class="text-center">
                                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("noibat").ToString() == "False" %>'>
                                                    <input type="checkbox" data-role="switch" data-material="true" name="noibat_<%#Eval("id").ToString() %>">
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("noibat").ToString() == "True" %>'>
                                                    <input type="checkbox" data-role="switch" checked data-material="true" name="noibat_<%#Eval("id").ToString() %>">
                                                </asp:PlaceHolder>
                                            </td>
                                            <td class="text-right">
                                                <%#Eval("ngaytao","{0:dd/MM/yyyy}").ToString() %>
                                                <div><%#Eval("ngaytao","{0:HH:mm}").ToString() %>'</div>
                                            </td>
                                            <td class="text-center">
                                                <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#Eval("hienthi").ToString() == "False" %>'>
                                                    <input type="checkbox" data-role="switch" data-material="true" name="hienthi_<%#Eval("id").ToString() %>">
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#Eval("hienthi").ToString() == "True" %>'>
                                                    <input type="checkbox" data-role="switch" checked data-material="true" name="hienthi_<%#Eval("id").ToString() %>">
                                                </asp:PlaceHolder>
                                            </td>
                                            <td><%#Eval("tennganh").ToString()%></td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
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

