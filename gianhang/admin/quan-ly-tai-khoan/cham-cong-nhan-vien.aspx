<%@ Page Title="Chấm công nhân viên" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="cham-cong-nhan-vien.aspx.cs" Inherits="badmin_Default" %>


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
                        <%--<li><a href="#_time">Thời gian</a></li>
                        <li><a href="#_sort">Sắp xếp</a></li>--%>
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

    <div id='form_3' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_3()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Chấm công nhanh</h5>
                <hr />
                <asp:Panel ID="Panel3" runat="server" DefaultButton="Button4">
                    <div class="mt-3">
                        <div class="fw-600 fg-red">Hướng dẫn</div>
                        <div>- Bước 1: Chọn ngày muốn chấm công.</div>
                        <div>- Bước 2: Tích chọn vào nhân viên muốn chấm công.</div>
                        <div>- Bước 3: Chọn mục muốn chấm công</div>
                        <div>- Bước 4: Nhấn nút LƯU CHẤM CÔNG</div>
                        <div class="fw-600 fg-red">Lưu ý</div>
                        <div>- Các giá trị cũ trước đó sẽ được thay thế bằng giá trị mới nhất.</div>
                        <div>- Các mục không được chọn mặc định sẽ là Nghỉ không lương.</div>
                    </div>
                    <div class="mt-3">
                        <div class="fw-600">Mục chấm công</div>
                        <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                            <asp:ListItem Text="Làm đủ ngày" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Làm nữa ngày" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Nghỉ phép" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Nghỉ có lương" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Nghỉ không lương" Value="4"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="mt-6 mb-10">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button4" runat="server" Text="LƯU CHẤM CÔNG" CssClass="button success" OnClick="Button4_Click" OnClientClick="show_hide_id_form_3()" />
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

    <div id="main-content" class=" mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
                    <div class="cell-md-6 order-2 order-md-1 mt-0">
                        <div class="d-flex flex-align-center gap-2">
                            <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm nhân viên"></asp:TextBox>
                            <asp:LinkButton ID="but_search" runat="server" CssClass="button" OnClick="but_search_Click" CausesValidation="false">
                                <span class="mif mif-search"></span>
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="cell-md-6 order-1 order-md-2 mt-0">
                        <div class="place-right">
                            <ul class="h-menu">
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lưu chấm công">
                                    <asp:ImageButton ID="but_save" runat="server" ImageUrl="/uploads/images/icon-button/but-save.png" Height="32" OnClick="but_save_Click" />
                                </li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li onclick="show_hide_id_form_3()">
                                    <a class="button">Chấm nhanh</a></li>
                                 <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li>
                                    <a class="button" href="#" data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                        <span class="mif mif-filter"></span>
                                    </a></li>
                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                </div>


                <div class="fg-red text-bold mt-3"><%=thongbao_chamcong_homnay %></div>
                <%--NỘI DUNG CHÍNH--%>

                <div id="table-main">
                    <div style="overflow: auto" class=" mt-4">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                            <thead>
                                <tr style="background-color: #f5f5f5">
                                    <td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>
                                    <td class=" text-bold " style="min-width: 60px; width: 60px;"></td>
                                    <td class="text-bold" style="min-width: 220px; width: 220px">Nhân viên</td>
                                    <td class="" style="min-width: 220px; width: 220px;">
                                        <asp:TextBox ID="txt_ngaychamcong" data-clear-button="false" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY"></asp:TextBox>
                                    </td>
                                    <td></td>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("username") %>">
                                            </td>
                                            <td>
                                                <div data-role="lightbox" style="cursor: pointer">
                                                    <img src='<%#Eval("avt") %>' data-original='<%#Eval("avt") %>' class="img-cover-60-vuongtron" style="max-width: none!important" />

                                                </div>
                                                <%--<img src="<%#Eval("avt") %>" class="img-cover-60-vuongtron" />--%>
                                            </td>
                                            <td>
                                                <div>
                                                    <%#Eval("fullname") %>
                                                </div>
                                                <div>
                                                    <small>
                                                        <%#Eval("username") %>
                                                    </small>
                                                </div>
                                            </td>
                                            <td>
                                                <select name="chamcong_<%#Eval("username").ToString() %>" data-role='select' data-filter='false'>
                                                    <%--<option value='0'>Làm đủ ngày</option>
                                                    <option value='1'>Làm nữa ngày</option>
                                                    <option value='2'>Nghỉ phép</option>
                                                    <option value='3'>Nghỉ có lương</option>
                                                    <option value='4'>Nghỉ không lương</option>
                                                    <option value='5'>Làm không lương</option>--%>
                                                    <%#return_chamcong(Eval("username").ToString()) %>
                                                </select>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <%stt = stt + 1; %>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <div class="text-center mt-4 mb-20">
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
    <%=notifi %>

    <script src="/js/gianhang-invoice-fast.js?v=20260326a"></script>
    <script>
        (function () {
            function bindFastUi() {
                if (!window.ahaInvoiceFast) return;
                window.ahaInvoiceFast.initSearchSubmit({
                    inputId: "<%=txt_search.ClientID %>",
                    buttonId: "<%=but_search.ClientID %>"
                });
                var dateInput = document.getElementById("<%=txt_ngaychamcong.ClientID %>");
                if (dateInput && !dateInput.dataset.ahaDateBound) {
                    dateInput.dataset.ahaDateBound = "1";
                    dateInput.addEventListener("change", function () {
                        var value = (dateInput.value || "").trim();
                        if (!value) return;
                        var url = new URL(window.location.href);
                        url.searchParams.set("ngay", value);
                        window.location.href = url.toString();
                    });
                }
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
</asp:Content>
