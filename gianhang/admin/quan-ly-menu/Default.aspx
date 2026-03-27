<%@ Page Title="Quản lý menu" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="badmin_quan_ly_menu_Default" %>

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
                                    <asp:ListItem Text="Tên menu (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Tên menu (Giảm dần)" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Tên menu cha (Tăng dần)" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Tên menu cha (Giảm dần)" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Số lượng menu con (Tăng dần)" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Số lượng menu con (Giảm dần)" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="Thứ tự hiển thị (Tăng dần)" Value="6"></asp:ListItem>
                                    <asp:ListItem Text="Thứ tự hiển thị (Giảm dần)" Value="7"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Tăng dần)" Value="8"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Giảm dần)" Value="9"></asp:ListItem>
                                    <asp:ListItem Text="Cấp bậc (Tăng dần)" Value="10"></asp:ListItem>
                                    <asp:ListItem Text="Cấp bậc (Giảm dần)" Value="11"></asp:ListItem>
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
                            <asp:ListItem Text="Tên menu" Value="name"></asp:ListItem>
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

    <div id='form_3' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_3()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Tùy chọn xóa</h5>
                <hr />
                <asp:Panel ID="Panel3" runat="server" DefaultButton="Button3">
                    <%--<div class="mt-3">
                        <div class="fw-600">Chọn mục muốn xuất</div>
                    </div>--%>
                    <div class="mt-3 fg-red">
                        <small>Chức năng tùy chọn xóa đang được cập nhật.<br />
                            Hãy nhấn nút XÓA để xóa.</small>
                    </div>
                    <hr />
                    <div class="mt-6 mb-10">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button3" runat="server" Text="XÓA" CssClass="button alert" OnClick="Button3_Click" OnClientClick="show_hide_id_form_3()" />
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
                            Bạn đã chắc chắn chưa?</small>
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
                <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
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
                                    <a class="button" href="/gianhang/admin/quan-ly-menu/add.aspx"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lưu">
                                    <asp:ImageButton ID="but_luu" runat="server" ImageUrl="~/uploads/images/icon-button/but-save.png" OnClick="but_luu_Click" /></li>

                                <%if (Session["index_trangthai_menu"].ToString() != "1")
                                    { %>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                    <a class="button" onclick="show_hide_id_form_3()"><span class="mif mif-bin"></span></a>
                                    <%--<asp:ImageButton ID="but_del" runat="server" ImageUrl="~/uploads/images/icon-button/but-bin.png"  />--%>
                                </li>
                                <%} %>

                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                                <li data-role="hint" data-hint-position="top" data-hint-text="In">
                                    <a class="button"><span class="mif mif-print"></span></a>
                                    <ul class="d-menu place-right" data-role="dropdown">
                                        <li><a target="_blank" href="/gianhang/admin/quan-ly-menu/in/a4.aspx">In A4</a></li>
                                        <li><a target="_blank" href="/gianhang/admin/quan-ly-menu/in/bill.aspx">In Bill</a></li>
                                    </ul>
                                </li>

                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel" onclick="show_hide_id_form_2()">
                                    <a class="button"><span class="mif mif-file-excel"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li>
                                    <a href="#" <%--class="dropdown-toggle"--%>><span class="mif mif-more-vert"></span></a>
                                    <ul class="d-menu place-right" data-role="dropdown">
                                        <%if (Session["index_trangthai_menu"].ToString() == "1")
                                            { %><%--=1 là list trong thùng rác--%>
                                        <li><a onclick="show_hide_id_form_4()">Xóa vĩnh viễn</a></li>
                                        <li><a runat="server" onserverclick="but_khoiphuc_Click">Khôi phục</a></li>
                                        <li class="divider"></li>
                                        <%} %>
                                        <li><a href="/gianhang/admin/quan-ly-menu/xem-truoc.aspx">Xem trước</a></li>
                                        <li><a href="/gianhang/admin/quan-ly-menu/cay-menu.aspx">Cây menu</a></li>
                                    </ul>
                                </li>
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
                                    <td class=" text-bold " style="width: 94px;">Ngày tạo</td>
                                    <td class=" text-bold " style="min-width: 100px;">Tên menu</td>
                                    <td class=" text-bold" style="min-width: 100px">Menu cha
                                    </td>
                                    <td class=" text-bold text-center" style="min-width: 100px; width: 100px;">Cấp
                                                
                                    </td>


                                    <td class=" text-bold text-center" style="min-width: 100px; width: 100px;">Menu con
                                                
                                    </td>
                                    <td class=" text-bold  text-center" style="min-width: 100px; width: 100px">Thứ tự
                                                
                                    </td>

                                    <%--<td style="width: 1px;"></td>--%>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                                <%-- <label class="checkbox transition-on style2 mt-1">
                                                            <input type="checkbox" data-role="checkbox" data-style="2" data-role-checkbox="true" name="check_<%#Eval("id").ToString() %>" onkeypress="if (event.keyCode==13) return false;">
                                                            <span class="check"></span>
                                                            <span class="caption"></span>
                                                        </label>--%>
                                            </td>
                                            <td class="text-right text-bold"><a href="/gianhang/admin/quan-ly-menu/edit.aspx?id=<%#Eval("id") %>" data-role="hint" data-hint-position="right" data-hint-text="Nhấn để sửa"><%#Eval("id") %></a></td>
                                            <td class="text-right">
                                                <div><%#Eval("ngaytao","{0:dd/MM/yyyy}") %></div>
                                                <%-- <div><%#Eval("ngaytao","{0:HH:mm}") %></div>--%>
                                            </td>
                                            <td>
                                                <asp:PlaceHolder ID="PlaceHolder14" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                    <span><%#Eval("name") %></span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                    <span class="fg-red"><%#Eval("name") %></span>
                                                </asp:PlaceHolder>

                                            </td>
                                            <td><%#Eval("menucha") %></td>
                                            <td class="text-center"><%#Eval("capbac") %></td>
                                            <%--<td class="text-center">
                                                <a href="#" data-role="hint"
                                                    data-hint-position="right"
                                                    data-hint-text="Xem chi tiết"><%#Eval("sl_menu_con").ToString() %></a>
                                            </td>--%>
                                            <td class="text-center"><%#Eval("sl_menu_con").ToString() %></td>
                                            <td>
                                                <input class="border bd-default pl-2 pr-2" style="width: 100%" maxlength="6" name="rank_<%#Eval("id").ToString() %>" type="text" value="<%#Eval("rank").ToString() %>" onkeypress="if (event.keyCode==13) return false;" autocomplete="off">
                                            </td>

                                            <%--<td>
                                                <ul class="h-menu bg-transparent">
                                                    <li>
                                                        <a href="#"><span class="mif mif-more-vert"></span></a>
                                                        <ul class="d-menu place-right " data-role="dropdown">
                                                            <li><a class="fg-black" href="/gianhang/admin/quan-ly-menu/edit.aspx?id=<%#Eval("id") %>"><span class="icon mif-pencil ml-2"></span><span class="pl-2">Chỉnh sửa</span></a></li>
                                                            <li class="divider"></li>
                                                            <li><a class="fg-black" href="/gianhang/admin/quan-ly-menu/del.aspx?id=<%#Eval("id") %>"><span class="icon mif-bin ml-2"></span><span class="pl-2">Xóa</span></a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </td>--%>
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

