<%@ Page Title="Kho vật tư" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="kho-vat-tu.aspx.cs" Inherits="badmin_quan_ly_menu_Default" %>

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
                                <div class="fw-600">Số lượng tồn</div>
                                <asp:DropDownList ID="DropDownList3" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả các lô" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Các lô hàng còn tồn" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <div class="fw-600">Hạn sử dụng</div>
                                <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Đã hết hạn" Value="1"></asp:ListItem>
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
                                    <asp:ListItem Text="Số lượng tồn (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Số lượng tồn (Giảm dần)" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Hạn bảo hành (Sắp hết lên trước)" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Hạn bảo hành (Lâu hết lên trước)" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Ngày nhập (Tăng dần)" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Ngày nhập (Giảm dần)" Value="5"></asp:ListItem>
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
                                <li data-role="hint" data-hint-position="top" data-hint-text="Nhập">
                                    <a class="button" href="/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>

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
                                     <td style="width: 1px;" class=" text-bold text-center">Ngày nhập</td>
                                    <td style="width: 1px;" class=" text-bold text-center">Hạn bảo hành</td>
                                    <td style="width: 1px;" class=" text-bold text-center">Số lô</td>
                                    <td style="width: 50px;" class=" text-bold text-center">Ảnh</td>
                                    <td class=" text-bold " style="min-width: 200px">Tên vật tư</td>
                                    <td class=" text-bold text-center" style="width: 1px;">ĐVT</td>
                                    <td class=" text-bold text-center" style="width: 1px;">Giá nhập</td>
                                    <td style="width: 1px;" class=" text-bold text-center">SL nhập</td>
                                    <td style="width: 1px;" class=" text-bold text-center">Đã xuất</td>
                                    <td class=" text-bold text-center" style="width: 1px">SL tồn</td>
                                    <td class=" text-bold text-center" style="width: 70px">Giá trị tồn</td>
                                   
                                    <%-- <td class=" text-bold " style="width: 94px;">Ngày tạo</td>--%>

                                    <%--<td style="width: 1px;"></td>--%>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("hsd")!=null %>'>
                                                    <%#Eval("nsx","{0:dd/MM/yyyy}").ToString() %>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td class="text-center">
                                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("hsd")!=null %>'>
                                                    <%#Eval("hsd","{0:dd/MM/yyyy}").ToString() %><br />
                                                    <%#check_hsd(Eval("hsd","{0:dd/MM/yyyy}").ToString()) %>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td class="text-center text-bold">
                                                <%#Eval("solo") %></td>
                                            <td>

                                                <img src="<%#Eval("image") %>" class="img-cover-vuong w-h-50" style="max-width: none!important">
                                            </td>
                                            <td>
                                                <span><%#Eval("name") %></span>
                                            </td>
                                            <td>
                                                <span><%#Eval("dvt") %></span>
                                            </td>
                                            <td class="text-right">
                                                <div><%#Eval("gianhap","{0:#,##0}").ToString() %></div>
                                            </td>
                                            <td class="text-center">
                                                <div><%#Eval("soluong","{0:#,##0}").ToString() %></div>
                                            </td>
                                            <td class="text-center">
                                                <div><%#Eval("soluong_daban","{0:#,##0}").ToString() %></div>
                                            </td>
                                            <td class="text-center text-bold fg-orange">
                                                <div><%#Eval("soluong_ton_sanpham","{0:#,##0}").ToString() %></div>
                                            </td>
                                            <td class="text-right">
                                                <div><%#Eval("giatri_ton","{0:#,##0}").ToString() %></div>
                                            </td>
                                            
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="7"></td>
                                    <td class="text-center text-bold "><%=tong_sl_nhap.ToString("#,##0") %></td>
                                    <td class="text-center text-bold "><%=tong_sl_xuat.ToString("#,##0") %></td>
                                    <td class="text-center text-bold fg-orange"><%=tong_sl_ton.ToString("#,##0") %></td>
                                    <td class="text-right text-bold"><%=tong_giatri_ton.ToString("#,##0") %></td>
                                   
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
