<%@ Page Title="Quản lý thu chi" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .icon-box {
            height: 103px !important;
        }

            .icon-box .icon {
                height: 102px !important;
                width: 80px;
            }

        .thuchi-auto-row {
            background: #fff7f7;
        }

        .thuchi-auto-tag {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 2px 8px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            color: #7f1d1d;
            background: #ffe4e6;
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

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8 pb-8">
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
                            <div class="mt-3">
                                <label class="fw-600">Loại thu chi</label>
                                <asp:DropDownList ID="ddl_loc1" runat="server" data-role="select" data-filter="false"></asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Duyệt phiếu</label>
                                <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Đã duyệt" Value="Đã duyệt"></asp:ListItem>
                                    <asp:ListItem Text="Chưa duyệt" Value="Chưa duyệt"></asp:ListItem>
                                    <asp:ListItem Text="Hủy duyệt" Value="Hủy duyệt"></asp:ListItem>
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
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
                            <ul class="h-menu ">
                                <%if (bcorn_class.check_quyen(user, "q9_2") == ""||bcorn_class.check_quyen(user, "n9_2") == "")
                                    { %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Tạo thu chi"><a class="button" href="/gianhang/admin/quan-ly-thu-chi/add.aspx"><span class="mif mif-plus"></span></a></li>
                                <%} %>
                                <%--<li class="bd-gray border bd-default" style="height:38px"></li>--%>
                                <%--<li class="bd-gray border bd-default mt-1" style="height:28px"></li>--%>

                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>

                                <%if (bcorn_class.check_quyen(user, "q9_6") == ""||bcorn_class.check_quyen(user, "n9_6") == "")
                                    { %>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Duyệt phiếu chi">
                                    <a class="button" runat="server" onserverclick="but_duyetchi_Click1">
                                        <span class="mif mif-checkmark fg-blue"></span></a>
                                </li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Hủy duyệt chi">
                                    <a class="button" runat="server" onserverclick="but_huyduyetchi_Click">
                                        <span class="mif mif-cross-light fg-red"></span></a>
                                </li>
                                <%} %>
                                <%if (bcorn_class.check_quyen(user, "q9_4") == ""||bcorn_class.check_quyen(user, "n9_4") == "")
                                    { %>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                    <asp:ImageButton ID="but_xoa" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_Click" />
                                </li>

                                <%} %>

                                <%--
                                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                    <li>
                                    <a href="#"><span class="mif mif-more-vert"></span></a>
                                    <ul class="d-menu place-right" data-role="dropdown">
                                        <li><a href="/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx">Quản lý nhóm thu chi</a></li>
                                        <li><a href="/gianhang/admin/quan-ly-thu-chi/so-quy-tien-mat.aspx">Sổ quỹ tiền mặt</a></li>
                                       
                                    </ul>
                                </li>--%>
                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                </div>


                <div class="row">
                    <div class="cell-md-5">
                    </div>
                    <div class="cell-md-7 text-right mt-3">
                        <span class="fg-green text-bold">Thu: <%=_tongthu_giuaky.ToString("#,##0") %></span>
                        <span class="ml-5 fg-orange text-bold">Chi: <%=_tongchi_giuaky.ToString("#,##0") %></span>
                        <span class="ml-5 text-bold">Tổng: <%=_tongthuchi.ToString("#,##0") %></span>
                    </div>
                </div>



                <div id="table-main">
                    <div style="overflow: auto" class=" mt-4">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">

                            <thead>
                                <tr class="border-bottom bd-default ">
                                    <td colspan="8" class="text-right text-leader2 text-bold">Tồn đầu kỳ</td>
                                    <td class="text-leader2 text-right text-bold"><%=tondauky %></td>
                                    <td colspan="2"></td>
                                </tr>
                                <tr style="background-color: #f5f5f5">
                                    <td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>
                                    <td style="width: 1px;" class="text-center text-bold">Mã</td>
                                    <td class="text-bold" style="min-width: 1px; width: 1px">Ngày</td>

                                    <td class="text-bold" style="min-width: 120px;">Loại</td>
                                    <td class="text-bold" style="min-width: 120px">Người tạo</td>
                                    <td class="text-bold" style="min-width: 120px">Người nhận</td>
                                    <td class="text-bold" style="min-width: 160px">Nội dung</td>
                                    <td style="width: 50px; min-width: 50px" class=" text-bold">Ảnh</td>
                                    <td class="text-bold text-right" style="width: 80px; min-width: 80px">Số tiền</td>
                                    <td class="text-bold" style="min-width: 130px">Duyệt/Hủy</td>
                                    <td></td>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr class="<%#Eval("row_class") %>">

                                            <td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>" <%#Eval("check_attr") %>>
                                            </td>
                                            <td class="text-center">
                                                <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#(bool)Eval("is_auto")==false %>'>
                                                    <a href="/gianhang/admin/quan-ly-thu-chi/edit.aspx?id=<%#Eval("id").ToString() %>" data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa">
                                                        <b><%#Eval("id").ToString() %></b>
                                                    </a>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#(bool)Eval("is_auto")==true %>'>
                                                    <b><%#Eval("id").ToString() %></b>
                                                </asp:PlaceHolder>
                                                <div>
                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("thuchi").ToString()=="Thu" %>'>
                                                        <div class="data-wrapper"><code class="bg-green fg-white">Thu</code></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("thuchi").ToString()=="Chi" %>'>
                                                        <div class="data-wrapper"><code class="bg-orange fg-white">Chi</code></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#(bool)Eval("is_auto")==true %>'>
                                                        <div class="thuchi-auto-tag">Tự động · <%#Eval("auto_source") %></div>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </td>
                                            <td class="text-right">
                                                <small>
                                                    <%#Eval("ngay","{0:dd/MM/yyyy}").ToString() %>
                                                    <div>
                                                        <%#Eval("ngay","{0:HH:mm}").ToString() %>
                                                    </div>
                                                </small>
                                            </td>


                                            <td>
                                                <%#Eval("nhom").ToString() %>
                                                <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#Eval("auto_link")!=null && Eval("auto_link").ToString()!="" %>'>
                                                    <div><a class="fg-crimson" href="<%#Eval("auto_link") %>">Mở chứng từ</a></div>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td><%#Eval("nguoilapphieu").ToString() %></td>
                                            <td><%#Eval("nguoinhan").ToString() %></td>
                                            <td><%#Eval("noidung").ToString() %></td>
                                            <td>
                                                <div data-role="lightbox" style="cursor: pointer">
                                                    <%#get_hinhanh(Eval("id").ToString()) %>
                                                    <%--<img src='<%#Eval("avt") %>' data-original='<%#Eval("avt") %>' class='img-cover-60-vuongtron' style="max-width: none!important' />
                                                    <img class='d-none' src='link'>--%>
                                                </div>
                                            </td>
                                            <td class="text-bold text-right">
                                                <%#Eval("sotien","{0:#,##0}") %>
                                                <div>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("duyet_chi").ToString()=="Đã duyệt" %>'>
                                                        <div class="data-wrapper"><code class="bg-cyan fg-white">Đã duyệt</code></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("duyet_chi").ToString()=="Chưa duyệt" %>'>
                                                        <div class="data-wrapper"><code class="bg-orange fg-white">Chưa duyệt</code></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("duyet_chi").ToString()=="Hủy duyệt" %>'>
                                                        <div class="data-wrapper"><code class="bg-red fg-white">Hủy duyệt</code></div>
                                                    </asp:PlaceHolder>


                                                </div>
                                            </td>
                                            <td>
                                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("nguoi_duyet_huy")!=null %>'>
                                                    <small><%#Eval("nguoi_duyet_huy") %>
                                                        <div><%#Eval("thoigian_duyet_huy","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                    </small>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td>
                                                <ul class="h-menu bg-transparent">
                                                    <li>
                                                        <a href="#" <%-- class="dropdown-toggle"--%>><span class="mif mif-more-horiz"></span></a>
                                                        <ul class="d-menu place-right " data-role="dropdown">
                                                            <%--<li class="divider"></li>--%>
                                                            <li>
                                                                <asp:PlaceHolder ID="PlaceHolder11" runat="server" Visible='<%#(bool)Eval("is_auto")==false %>'>
                                                                    <a class="fg-black" href="/gianhang/admin/quan-ly-thu-chi/edit.aspx?id=<%#Eval("id").ToString() %>">Chỉnh sửa</a>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="PlaceHolder12" runat="server" Visible='<%#(bool)Eval("is_auto")==true %>'>
                                                                    <span class="fg-gray">Phiếu tự động</span>
                                                                </asp:PlaceHolder>
                                                            </li>
                                                            <li>
                                                                <a class="fg-black" target="_blank" href="/gianhang/admin/quan-ly-thu-chi/inthuchi.aspx?id=<%#Eval("id").ToString() %>">In A5</a>
                                                            </li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr class="border-top bd-default ">
                                    <td colspan="8" class="text-right text-bold fg-red">Tổng chưa duyệt</td>
                                    <td class="text-right text-bold fg-red"><%=_tongchuaduyet.ToString("#,##0") %></td>
                                    <td colspan="2"></td>
                                </tr>
                                <tr class="border-top bd-default ">
                                    <td colspan="8" class="text-right text-bold fg-blue">Tổng đã duyệt</td>
                                    <td class="text-right text-bold fg-blue"><%=_tongthuchi.ToString("#,##0") %></td>
                                    <td colspan="2"></td>
                                </tr>
                                <tr class="border-top bd-default ">
                                    <td colspan="8" class="text-right text-leader2 text-bold">Tồn cuối kỳ</td>
                                    <td class="text-leader2 text-right text-bold"><%=toncuoiky %></td>
                                    <td colspan="2"></td>
                                </tr>
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