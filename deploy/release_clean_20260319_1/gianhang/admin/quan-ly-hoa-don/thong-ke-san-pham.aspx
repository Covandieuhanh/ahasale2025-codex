<%@ Page Title="Thống kê sản phẩm" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="thong-ke-san-pham.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .report-kpi {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
            margin: 14px 0 16px;
        }
        .report-kpi__card {
            border: 1px solid #ead6d6;
            background: #fff;
            border-radius: 14px;
            padding: 12px 14px;
            box-shadow: 0 8px 18px rgba(178, 38, 38, 0.08);
        }
        .report-kpi__card small {
            display: block;
            color: #7b5a5a;
            font-weight: 600;
        }
        .report-kpi__card strong {
            display: block;
            font-size: 18px;
            color: #7f1d1d;
            margin-top: 6px;
        }
        .report-kpi__hint {
            color: #7b5a5a;
            font-size: 12px;
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
                                    <asp:ListItem Text="Số lượng (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Số lượng (Giảm dần)" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Tổng tiền (Tăng dần)" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Tổng tiền (Giảm dần)" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Sau chiết khấu (Tăng dần)" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Sau chiết khấu (Giảm dần)" Value="5"></asp:ListItem>
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
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
                                <%--<li class="bd-gray border bd-default mt-1" style="height: 28px"></li>--%>
                            </ul>
                        </div>
                        <div class="clr-float"></div>

                    </div>
                </div>




                <div class="report-kpi">
                    <div class="report-kpi__card">
                        <small>Doanh thu sản phẩm (sau CK)</small>
                        <strong><%=tongsauck.ToString("#,##0") %> đ</strong>
                    </div>
                    <div class="report-kpi__card">
                        <small>Thực thu (Thu/Chi)</small>
                        <strong><%=tongthu.ToString("#,##0") %> đ</strong>
                        <div class="report-kpi__hint">Tổng các phiếu Thu trong kỳ</div>
                    </div>
                    <div class="report-kpi__card">
                        <small>Tổng chi</small>
                        <strong><%=tongchi.ToString("#,##0") %> đ</strong>
                        <div class="report-kpi__hint">Bao gồm chi nhập kho & chi khác</div>
                    </div>
                    <div class="report-kpi__card">
                        <small>Lợi nhuận tạm tính</small>
                        <strong class="<%=loinhuan_class %>"><%=loinhuan.ToString("#,##0") %> đ</strong>
                        <div class="report-kpi__hint">Thu - Chi</div>
                    </div>
                </div>

                <div id="table-main">
                    <div style="overflow: auto" class=" mt-4">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                            <thead>
                                <tr style="background-color: #f5f5f5">
                                    <td class="text-bold" style="min-width: 110px;">Tên sản phẩm</td>
                                    <td class="text-bold" style="width: 110px; min-width: 110px">Số lượng</td>
                                     <td class="text-bold" style="width: 100px; min-width: 100px">Tổng tiền</td>
                                    <td class="text-bold" style="width: 100px; min-width: 100px">Sau CK</td>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%#Eval("tensp").ToString() %>
                                            </td>
                                            <td class="text-center"><%#Eval("tongsl","{0:#,##0}").ToString() %></td>
                                            <td class="text-right">
                                                <%#Eval("tongtien","{0:#,##0}").ToString() %>
                                            </td>
                                            <td class="text-right text-bold">
                                                <%#Eval("sauck","{0:#,##0}").ToString() %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td></td>
                                    <td class="text-center text-bold"><%=tongsl.ToString("#,##0") %></td>
                                    <td class="text-right text-bold"><%=tongtien.ToString("#,##0") %></td>
                                    <td class="text-right text-bold fg-red"><%=tongsauck.ToString("#,##0") %></td>
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
    <%--<%=notifi %>--%>
</asp:Content>

