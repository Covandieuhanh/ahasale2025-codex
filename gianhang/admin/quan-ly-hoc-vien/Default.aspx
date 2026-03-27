<%@ Page Title="Quản lý thành viên" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="badmin_quan_ly_menu_Default" %>

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
                                <div class="fw-600">Cấp bằng</div>
                                <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Chưa cấp bằng" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Đã cấp bằng" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Thanh toán</label>
                                <asp:DropDownList ID="ddl_locdulieu" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Lọc thành viên còn nợ" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Lọc thành viên đã thanh toán" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Ngành</label>
                                <asp:DropDownList ID="DropDownList5" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Trạng thái nguồn</label>
                                <asp:DropDownList ID="ddl_trangthai_nguon" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Đang dùng thành viên" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Đã ngừng dùng thành viên" Value="2"></asp:ListItem>
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




    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />

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

                                <li data-role="hint" data-hint-position="top" data-hint-text="Thêm mới">
                                    <a class="button" href="/gianhang/admin/quan-ly-hoc-vien/add.aspx"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <%if (bcorn_class.check_quyen(user, "q14_3") == "" || bcorn_class.check_quyen(user, "n14_3") == "")
                                    { %>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Ngừng dùng">
                                    <asp:Button ID="but_ngung" runat="server" Text="Ngừng dùng" CssClass="button warning" OnClick="but_ngung_Click" />
                                </li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Mở lại">
                                    <asp:Button ID="but_molai" runat="server" Text="Mở lại" CssClass="button success" OnClick="but_molai_Click" />
                                </li>
                                <%} %>
                                <%if (bcorn_class.check_quyen(user, "q14_4") == ""||bcorn_class.check_quyen(user, "n14_4") == "")
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
                <div id="table-main">
                    <div style="overflow: auto" class=" mt-3">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                            <thead>
                                <tr style="background-color: #f5f5f5">
                                    <td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>


                                    <td class=" text-bold text-center" style="width: 50px;">Ảnh</td>
                                    <td class=" text-bold " style="min-width: 120px;">Họ tên</td>
                                    <td class=" text-bold " style="min-width: 220px;">Liên kết Home</td>
                                    <td class=" text-bold " style="min-width: 150px;">Trạng thái nguồn</td>
                                    <td class=" text-bold " style="min-width: 120px;">Ngành/Gói</td>

                                    <td class=" text-bold " style="width: 120px;">Số buổi</td>
                                    <td class=" text-bold " style="width: 100px;">Cấp bằng</td>
                                    <td class=" text-bold " style="width: 100px;">Ngày tạo</td>
                                    <td class=" text-bold " style="width: 100px;">Học phí</td>
                                    <td class="text-bold" style="width: 80px">T.Toán/C.Nợ</td>

                                    <%--<td style="width: 1px;"></td>--%>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
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


                                            <td>
                                                <a href="/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=<%#Eval("id").ToString() %>" data-role="hint" data-hint-position="top" data-hint-text="Chi tiết">
                                                    <img src="<%#Eval("avt") %>" class="img-cover-vuongtron w-h-50" style="max-width: none!important" />
                                                </a>
                                            </td>
                                            <td>
                                                <a class="fg-black fg-blue-hover" href="/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=<%#Eval("id").ToString() %>">
                                                    <span><%#Eval("hoten") %></span></a>
                                                <div><a class="fg-blue" title="Nhấn để gọi" href="tel:<%#Eval("sdt") %>"><%#Eval("sdt") %></a></div>
                                                <div><small><%#Eval("ngaysinh","{0:dd/MM/yyyy}") %></small></div>
                                            </td>
                                            <td>
                                                <asp:Literal ID="litPersonHub" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="litLifecycle" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <div>
                                                    <%#Eval("nganhhoc") %>
                                                </div>
                                                <div><%#Eval("goidaotao") %></div>
                                                <div>
                                                    <small>GV: <%#Eval("ten_gv") %></small>
                                                </div>

                                            </td>




                                            <td>
                                                <small>
                                                    <div>Lý thuyết: <%#Eval("sobuoi_lt") %></div>
                                                    <div>Thực hành: <%#Eval("sobuoi_th") %></div>
                                                    <div>Trợ giảng: <%#Eval("sobuoi_tg") %></div>
                                                </small>
                                            </td>
                                            <td>
                                                <div>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("capbang").ToString()=="Chưa cấp bằng" %>'>
                                                        <span class="data-wrapper"><code class="bg-grayBlue fg-white">Chưa cấp bằng</code></span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("capbang").ToString()=="Đã cấp bằng" %>'>
                                                        <span class="data-wrapper"><code class="bg-green fg-white">Đã cấp bằng</code></span>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("ngaycapbang")!=null %>'>
                                                            <small><%#Eval("ngaycapbang","{0:dd/MM/yyyy}") %></small>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>
                                                    <div><small><%#xuly_xeploai(Eval("xeploai").ToString()) %></small></div>
                                                </div>
                                            </td>
                                            <td>
                                                <small>
                                                    <div><%#Eval("ngaytao","{0:dd/MM/yyyy}") %></div>
                                                    <div><%#Eval("nguoitao") %></div>
                                                </small>
                                            </td>
                                            <td class="text-bold text-right">
                                                <%#Eval("hocphi","{0:#,##0}").ToString() %>
                                            </td>
                                            <td class="text-right">
                                                <div>
                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                        <span class="data-wrapper"><code class="bg-red  fg-white">Đã thanh toán</code></span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                        <%#Eval("sotien_dathanhtoan","{0:#,##0}").ToString() %>
                                                    </asp:PlaceHolder>
                                                </div>
                                                <div>
                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                        <%#Eval("sotien_conlai","{0:#,##0}").ToString() %>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                        <span class="data-wrapper"><code class="bg-orange fg-white"><%#Eval("sotien_conlai","{0:#,##0}").ToString() %></code></span>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="7"></td>
                                    <td class="text-right text-bold"><%=tong_hocphi.ToString("#,##0") %></td>
                                    <td class="text-right text-bold">
                                        <div><%=tong_thanhtoan.ToString("#,##0") %></div>
                                        <div>
                                            <span class="data-wrapper"><code class="bg-orange fg-white"><%=tong_congno.ToString("#,##0") %></code></span>
                                        </div>
                                    </td>

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