<%@ Page Title="Chi tiết doanh số" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="chi-tiet-doanh-so.aspx.cs" Inherits="admin_quan_ly_hoa_don_lich_su_ban_hang" %>

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
                                <label class="fw-600">Mặt hàng</label>
                                <asp:DropDownList ID="ddl_loc_mathang" runat="server" data-role="select" data-filter="true">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Dịch vụ" Value="dichvu"></asp:ListItem>
                                    <asp:ListItem Text="Sản phẩm" Value="sanpham"></asp:ListItem>
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
                                    <asp:ListItem Text="Ngày bán (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Ngày bán (Giảm dần)" Value="1"></asp:ListItem>
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
            </Triggers>
            <ContentTemplate>
                <div class="row ">
                    <div class="cell-6 d-flex">
                        <a data-role="hint" data-hint-position="top" data-hint-text="Quay lại" class="button" href="/gianhang/admin/quan-ly-tai-khoan/doanh-so-nhan-vien.aspx"><span class="mif mif-arrow-left"></span></a></li>
                        <div class="pt-2 pl-3"><b><%=hoten %></b></div>
                    </div>
                    <div class="cell-6">
                    </div>
                </div>

                <div class="row mt-2">
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
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <%-- <%if (bcorn_class.check_quyen(user, "q7_9") == "")
                                    { %>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li>
                                    <asp:ImageButton ID="but_xoa_lichsu" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_lichsu_Click" />
                                </li>
                                <%} %>--%>
                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                </div>


                <div id="table-main">
                    <div style="overflow: auto" class=" mt-4">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                            <thead>
                                <tr style="background-color: #ecf0f5">
                                    <%-- <td style="width: 1px;" class=" text-bold text-center">#</td>--%>
                                    <%--<td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>--%>
                                    <td class="text-bold" style="width: 1px;">Ngày</td>
                                    <td class="text-bold text-center" style="min-width: 1px; width: 1px">Đơn/Thẻ</td>
                                    <td class="text-bold" style="min-width: 140px">Khách hàng</td>

                                    <td class="text-bold" style="min-width: 150px">Tên</td>

                                    <td class="text-bold text-center" style="width: 50px;">Giá x SL</td>

                                    <td class="text-bold text-right" style="width: 102px; min-width: 102px">Thành tiền</td>
                                    <td class="text-bold " style="width: 1px;">CK</td>
                                    <td class="text-bold " style="width: 60px;">Sau CK</td>




                                    <td class="text-bold" style="min-width: 140px">DS chốt sale</td>
                                    <td class="text-bold" style="min-width: 140px">DS làm dịch vụ</td>
                                    <td class="text-bold" style="min-width: 140px">
                                    DS bán thẻ</td>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <%--<td class="text-right"><%=stt %></td>--%>
                                            <%--<td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                            </td>--%>
                                            <td class="text-right"><small><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></small></td>
                                            <td class=" text-center">
                                                <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#Eval("kyhieu_list").ToString()=="hoadon" %>'>
                                                    <a data-role="hint" data-hint-position="top" data-hint-text="Xem hóa đơn" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id_hoadon").ToString() %>">
                                                        <b>Đơn<br />
                                                            <%#Eval("id_hoadon").ToString() %></b></a>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder11" runat="server" Visible='<%#Eval("kyhieu_list").ToString()=="thedv" %>'>
                                                    <a class="fg-orange" data-role="hint" data-hint-position="top" data-hint-text="Chi tiết thẻ" href="/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=<%#Eval("id_hoadon").ToString() %>">
                                                        <b>Bán thẻ<br />
                                                            <%#Eval("id_hoadon").ToString() %></b></a>
                                                </asp:PlaceHolder>
                                            </td>

                                            <td><%#Eval("tenkhachhang").ToString() %>
                                                <div><a class="fg-orange" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></div>
                                            </td>


                                            <td class="">

                                                <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                    <span class="fg-blue"><%#Eval("tendvsp").ToString() %></span>
                                                    <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#Eval("id_thedichvu")!=null%>'>
                                                        <asp:PlaceHolder ID="PlaceHolder13" runat="server" Visible='<%#Eval("kyhieu_list").ToString()=="hoadon" %>'>
                                                            <span class="data-wrapper"><code class="bg-orange fg-white">Thẻ DV số <%#Eval("id_thedichvu")%></code></span>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("kyhieu").ToString()=="sanpham" %>'>
                                                    <span class="fg-green"><%#Eval("tendvsp").ToString() %></span>
                                                </asp:PlaceHolder>

                                            </td>

                                            <td class="text-center">
                                                <%#Eval("gia","{0:#,##0}").ToString() %>
                                                <div>x <%#Eval("soluong","{0:#,##0}").ToString() %></div>

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
                                                <asp:PlaceHolder ID="PlaceHolder14" runat="server" Visible='<%#Eval("kyhieu_list").ToString()=="hoadon" %>'>
                                                    <asp:PlaceHolder ID="PlaceHolder15" runat="server" Visible='<%#Eval("userchot").ToString()==user_chitiet %>'>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("phantramchot").ToString()=="0" %>'>
                                                            <div><%#Eval("tongtien_chot","{0:#,##0}").ToString() %></div>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("phantramchot").ToString()!="0" %>'>
                                                            <div><%#Eval("phantramchot")%>%</div>
                                                            <div class="fg-cyan"><%#Eval("tongtien_chot","{0:#,##0}").ToString() %></div>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td class="text-right">
                                                <asp:PlaceHolder ID="PlaceHolder12" runat="server" Visible='<%#Eval("userlam").ToString()==user_chitiet %>'>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("phantramlam").ToString()=="0" %>'>
                                                        <div><%#Eval("tongtien_lam","{0:#,##0}").ToString() %></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("phantramlam").ToString()!="0" %>'>
                                                        <div><%#Eval("phantramlam")%>%</div>
                                                        <div class="fg-green"><%#Eval("tongtien_lam","{0:#,##0}").ToString() %></div>
                                                    </asp:PlaceHolder>
                                                </asp:PlaceHolder>

                                            </td>
                                            <td class="text-right">
                                                <asp:PlaceHolder ID="PlaceHolder16" runat="server" Visible='<%#Eval("kyhieu_list").ToString()=="thedv" %>'>
                                                    <asp:PlaceHolder ID="PlaceHolder17" runat="server" Visible='<%#Eval("userchot").ToString()==user_chitiet %>'>
                                                        <asp:PlaceHolder ID="PlaceHolder18" runat="server" Visible='<%#Eval("phantramchot").ToString()=="0" %>'>
                                                            <div><%#Eval("tongtien_chot","{0:#,##0}").ToString() %></div>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("phantramchot").ToString()!="0" %>'>
                                                            <div><%#Eval("phantramchot")%>%</div>
                                                            <div class="fg-orange"><%#Eval("tongtien_chot","{0:#,##0}").ToString() %></div>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>
                                                </asp:PlaceHolder>
                                            </td>


                                        </tr>
                                        <%-- <%stt = stt + 1; %>--%>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <%--<tr class="border-top bd-gray">                                  
                                </tr>--%>
                            </tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="5" class="text-right text-leader text-bold text-size-20">Tổng cộng</td>

                                    <td class="text-right text-bold "><%=tongtien.ToString("#,##0") %></td>
                                    <td class="text-right text-bold "><%=tongtien_ck.ToString("#,##0") %></td>
                                    <td class="text-right text-bold "><%=tong_sauck.ToString("#,##0") %></td>

                                    <td class="text-right text-bold fg-cyan">
                                        <div><small>Tổng SL chốt: <%=sl_chot.ToString("#,##0") %></small></div>
                                        <div class=" ">
                                            <%=tongtien_chot.ToString("#,##0") %>
                                        </div>
                                    </td>
                                    <td class="text-right text-bold fg-green">
                                        <div><small>Tổng SL làm: <%=sl_lam.ToString("#,##0") %></small></div>
                                        <div class=" ">
                                            <%=tongtien_lam.ToString("#,##0") %>
                                        </div>
                                    </td>
                                    <td class="text-right text-bold fg-orange">
                                        <div><small>Tổng SL bán thẻ: <%=sl_banthe.ToString("#,##0") %></small></div>
                                        <div class=" ">
                                            <%=tongtien_banthe.ToString("#,##0") %>
                                        </div>
                                    </td>
                                </tr>
                                <tr class="">
                                    <td colspan="8" class="text-right text-leader text-bold text-size-20 fg-red">Tổng doanh số</td>

                                    <td colspan="3" class="text-center text-bold fg-red text-size-20"><%=tongds.ToString("#,##0") %></td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                    <div class="text-center mt-8 mb-20" <%--style="margin-top: -70px!important"--%>>
                        <asp:Button ID="but_quaylai" runat="server" Text="Lùi" CssClass="" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Tiếp" CssClass="" OnClick="but_xemtiep_Click" />
                    </div>
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
    <%--<%=notifi %>--%>

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

