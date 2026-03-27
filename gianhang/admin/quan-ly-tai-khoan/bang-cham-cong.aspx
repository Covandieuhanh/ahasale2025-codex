<%@ Page Title="Bảng chấm công" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="bang-cham-cong.aspx.cs" Inherits="badmin_Default" %>


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


    <div id="main-content" class=" mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
            </Triggers>
            <ContentTemplate>

                <div>
                    <%--TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>
                    <div>
                        <div style="float: left">
                            <%--<h4>Danh sách dịch vụ</h4>--%>
                            <ul class="h-menu ">
                                <li><a class="button" href="#"><span class='mif mif-checkmark fg-green  c-pointer'></span>
                                    <span class="fg-green">Làm đủ ngày</span>
                                </a></li>
                                <li><a class="button" href="#"><span class='mif mif-checkmark fg-blue  text-linethrough  c-pointer'></span>
                                    <span class="fg-blue">Làm nữa ngày</span>
                                </a></li>
                                <li><a class="button" href="#"><span class='mif mif-cross fg-taupe  c-pointer'></span>
                                    <span class="fg-taupe">Nghỉ phép</span>
                                </a></li>
                                <li><a class="button" href="#"><span class='mif mif-checkmark fg-orange  c-pointer'></span>
                                    <span class="fg-orange">Nghỉ có lương</span>
                                </a></li>
                                <li><a class="button" href="#"><span class='mif mif-cross fg-red  c-pointer'></span>
                                    <span class="fg-red">Nghỉ không lương</span>
                                </a></li>
                                <li><a class="button" href="#" data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <span class="mif mif-filter"></span>
                                </a></li>
                                <%--<li class="bd-gray border bd-default mt-1" style="height:28px"></li>--%>
                            </ul>
                        </div>
                        <div style="float: right" class="mt-3">
                        </div>
                        <div class="clr-float"></div>
                    </div>
                    <%--END TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>

                    <%--BOX TÌM KIẾM VÀ HIỂN THỊ--%>
                    <div class="row">
                        <div class="cell-md-6 mt-3 pr-2-lg">
                            <div class="d-flex">
                                <div class="d-flex flex-align-center gap-2">
                                    <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm nhân viên"></asp:TextBox>
                                    <asp:LinkButton ID="but_search" runat="server" CssClass="button" OnClick="but_search_Click" CausesValidation="false">
                                        <span class="mif mif-search"></span>
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div class="cell-md-6 mt-3 pl-2-lg">
                            <div class="d-flex">
                                <asp:DropDownList ID="ddl_thang" runat="server" data-role="select" data-filter="false">
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddl_nam" runat="server" data-role="select" data-filter="false"></asp:DropDownList>
                                <asp:Button ID="but_xem" runat="server" Text="XEM" OnClick="but_xem_Click" CssClass="warning" />
                            </div>
                        </div>
                    </div>
                    <%--END BOX TÌM KIẾM VÀ HIỂN THỊ--%>

                    <%--TABLE CHÍNH--%>
                    <div class="grid-bcorn-container mt-3">
                        <div class="grid-bcorn ">
                            <div class="grid-bcorn-col grid-bcorn-colum-fixed-left">
                                <div class="grid-bcorn-item grid-bcorn-item--header" style="min-width: 240px">
                                    <p>Nhân viên</p>
                                </div>
                                <asp:Repeater ID="Repeater2" runat="server">
                                    <ItemTemplate>
                                        <div class="grid-bcorn-item">
                                            <div class="ellipsis-1-grid">
                                                <span data-role='hint' data-hint-position='top' data-hint-text='<%#Eval("username") %>' class="c-pointer"><%#Eval("fullname") %></span>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>

                            <%for (int i = 1; i <= tongsongay_trongthang; i++)
                                { %>
                            <div class="grid-bcorn-col">
                                <div class="grid-bcorn-item grid-bcorn-item--header">
                                    <p class="text-center"><%=i %></p>
                                </div>

                                <%for (int j = 0; j < arr_nhanvien.Length; j++)
                                    {%>
                                <div class="grid-bcorn-item">
                                    <p class="text-center">
                                        <%=return_chamcong(arr_nhanvien[j],i) %>
                                    </p>
                                </div>
                                <%} %>
                            </div>
                            <%} %>

                            <div class="grid-bcorn-col <%--grid-bcorn-colum-fixed-right--%>">
                                <div class="grid-bcorn-item grid-bcorn-item--header">
                                    <p>Công</p>
                                </div>
                                <%for (int j = 0; j < arr_nhanvien.Length; j++)
                                    {%>
                                <div class="grid-bcorn-item">
                                    <p class="text-center text-bold"><%=return_ngaycong(arr_nhanvien[j]) %></p>
                                </div>
                                <%} %>
                            </div>

                            <div class="grid-bcorn-col <%--grid-bcorn-colum-fixed-right--%>">
                                <div class="grid-bcorn-item grid-bcorn-item--header">
                                    <p>Lương</p>
                                </div>
                                <%for (int j = 0; j < arr_nhanvien.Length; j++)
                                    {%>
                                <div class="grid-bcorn-item">
                                    <p class="text-right  text-bold"><%=return_luongcong(arr_nhanvien[j]) %></p>
                                </div>
                                <%} %>
                            </div>

                        </div>
                    </div>

                    <%--NÚT TỚI LUI PHÂN TRANG--%>
                    <div class="text-center mt-4 mb-20">
                        <asp:Button ID="but_quaylai" runat="server" Text="Lùi" CssClass="" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Tiếp" CssClass="" OnClick="but_xemtiep_Click" />
                    </div>
                    <%--END NÚT TỚI LUI PHÂN TRANG--%>
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
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
</asp:Content>
