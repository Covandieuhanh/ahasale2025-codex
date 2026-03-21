<%@ Page Title="Yêu cầu tư vấn" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_yeu_cau_tu_van_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">


    <asp:UpdatePanel ID="up_loc" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_loc" runat="server" Visible="false" DefaultButton="but_loc">
                <div class="admin-fullpage-head admin-route-panel-head">
                    <div class='admin-fullpage-shell admin-fullpage-head-shell admin-route-panel-shell admin-route-panel-head-shell'>
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_loc" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                LỌC DỮ LIỆU
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div class="admin-fullpage-body-wrap admin-route-panel-body-wrap">
                    <div class='admin-fullpage-shell admin-fullpage-dialog admin-route-panel-shell admin-route-panel-dialog'>
                        <div class="bg-white border bd-transparent admin-fullpage-body admin-route-panel-body pl-4 pl-8-md pr-8-md pr-4">
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="fw-600 mt-3">Số lượng hiển thị mỗi trang</div>
                                    <asp:TextBox ID="txt_show" MaxLength="7" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                                    <%--<div class=" mt-3">
                                    <label class="fw-600">Lọc ra menu con của</label></div>
                                    <asp:ListBox ID="multiSelectList" runat="server" SelectionMode="Multiple" data-role="select">
                                        <asp:ListItem Text="Không chọn" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Dịch vụ" Value="96"></asp:ListItem>
                                        <asp:ListItem Text="Sản phẩm" Value="97"></asp:ListItem>
                                    </asp:ListBox>--%>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Lọc theo thời gian</label>
                                        <asp:DropDownList ID="ddl_thoigian" runat="server" data-role="select">
                                            <asp:ListItem Text="Dựa vào ngày gửi" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 mt-3">Từ ngày</label>
                                        <asp:TextBox ID="txt_tungay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>
                                    <div class=" mt-3">
                                        <label class="fw-600 mt-3">Đến ngày</label>
                                        <asp:TextBox ID="txt_denngay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>
                                    <div class="mt-1">
                                        <asp:Button ID="but_homqua" runat="server" Text="Hôm qua" Width="92" OnClick="but_homqua_Click" />
                                        <asp:Button ID="but_homnay" runat="server" Text="Hôm nay" Width="92" OnClick="but_homnay_Click" />
                                        <asp:Button ID="but_tuantruoc" runat="server" Text="Tuần trước" Width="92" OnClick="but_tuantruoc_Click" />
                                        <asp:Button ID="but_tuannay" runat="server" Text="Tuần này" Width="92" OnClick="but_tuannay_Click" />
                                        <asp:Button ID="but_thangtruoc" runat="server" Text="Tháng trước" Width="92" OnClick="but_thangtruoc_Click" />
                                        <asp:Button ID="but_thangnay" runat="server" Text="Tháng này" Width="92" OnClick="but_thangnay_Click" />
                                        <asp:Button ID="but_quytruoc" runat="server" Text="Quý trước" Width="92" OnClick="but_quytruoc_Click" />
                                        <asp:Button ID="but_quynay" runat="server" Text="Quý này" Width="92" OnClick="but_quynay_Click" />
                                        <asp:Button ID="but_namtruoc" runat="server" Text="Năm trước" Width="92" OnClick="but_namtruoc_Click" />
                                        <asp:Button ID="but_namnay" runat="server" Text="Năm này" Width="92" OnClick="but_namnay_Click" />
                                    </div>

                                </div>
                            </div>
                            <div class="mt-6 mb-20">
                                <div style="float: left">
                                    <asp:Button ID="but_huy_loc" OnClick="but_huy_loc_Click" runat="server" Text="Đặt lại mặc định" CssClass="button warning small" />
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="but_loc" OnClick="but_loc_Click" runat="server" Text="THỰC HIỆN LỌC" CssClass="button success" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                            <div class="mb-20">
                                <div class="mt-3">
                                    <div class="fw-600 fg-red"><i>Lọc theo nhu cầu của bạn. Liên hệ: 0842 359 155</i></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="up_loc">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_xuat" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_xuat" runat="server" Visible="false" DefaultButton="but_xuat_excel">
                <div class="admin-fullpage-head admin-route-panel-head">
                    <div class='admin-fullpage-shell admin-fullpage-head-shell admin-route-panel-shell admin-route-panel-head-shell'>
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_xuat" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                XUẤT EXCEL
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div class="admin-fullpage-body-wrap admin-route-panel-body-wrap">
                    <div class='admin-fullpage-shell admin-fullpage-dialog admin-route-panel-shell admin-route-panel-dialog'>
                        <div class="bg-white border bd-transparent admin-fullpage-body admin-route-panel-body pl-4 pl-8-md pr-8-md pr-4">
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <div class="fw-600">Chọn mục muốn xuất</div>
                                        <div class="mt-1">
                                            <asp:CheckBox Checked="true" ID="check_all_excel" runat="server" CssClass="text-bold" Text="Tất cả các mục" OnCheckedChanged="check_all_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_excel" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_excel_SelectedIndexChanged">
                                            <asp:ListItem Text="ID" Value="id" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Tên khách" Value="ten" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Ngày gửi" Value="ngay" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Điện thoại" Value="sdt" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Nội dung" Value="noidung" Selected="true"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <div class="fw-600">Chọn trang</div>
                                        <div class="mt-1">
                                            <asp:CheckBox ID="check_all_page" Checked="true" runat="server" CssClass="text-bold" Text="Tất cả các trang" OnCheckedChanged="check_all_page_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_page" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_page_SelectedIndexChanged"></asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>

                            <div class="cell-12">
                                <div class="mt-3">
                                    <small><b>Lưu ý:</b> Nhấn nút <b class="fg-green">"Xuất Excel"</b> 1 lần và chờ cho đến khi File được tải xuống.</small>
                                </div>
                            </div>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_xuat_excel" runat="server" CssClass="success" Text="Xuất Excel" OnClick="but_xuat_excel_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--<asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="up_xuat">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>

    <asp:UpdatePanel ID="up_in" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_in" runat="server" Visible="false">
                <div class="admin-fullpage-head admin-route-panel-head">
                    <div class='admin-fullpage-shell admin-fullpage-head-shell admin-route-panel-shell admin-route-panel-head-shell'>
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_in" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                IN THEO NHU CẦU CỦA BẠN
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div class="admin-fullpage-body-wrap admin-route-panel-body-wrap">
                    <div class='admin-fullpage-shell admin-fullpage-dialog admin-route-panel-shell admin-route-panel-dialog'>
                        <div class="bg-white border bd-transparent admin-fullpage-body admin-route-panel-body pl-4 pl-8-md pr-8-md pr-4">
                            <div class=" pt-10 pb-20">
                                Liên hệ: 0842 359 155 (Bôn Bắp)
                                <div>
                                    <asp:Button ID="but_in" runat="server" Text="Đến trang in" OnClick="but_in_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_in">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" class="admin-fullpage-toolbar admin-route-toolbar">
                    <ul class="h-menu bg-white">


                        <li data-role="hint" data-hint-position="top" data-hint-text="Về trang chính">
                            <asp:HyperLink ID="but_quayve_trangchu" runat="server" Visible="false"><span class="mif-undo"></span></asp:HyperLink>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Di chuyển vào thùng rác">
                            <asp:LinkButton ID="but_remove_bin" OnClick="but_remove_bin_Click" runat="server"><span class="mif-bin"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:HyperLink ID="but_show_form_loc" runat="server"><span class="mif-filter"></span></asp:HyperLink>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel">
                            <asp:HyperLink ID="but_show_form_xuat" runat="server"><span class="mif-file-excel"></span></asp:HyperLink>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="In">
                            <asp:HyperLink ID="but_show_form_in" runat="server"><span class="mif-print"></span></asp:HyperLink>
                        </li>
                        <li>
                            <%--class="dropdown-toggle"--%>
                            <a href="#"><span class="mif-more-vert"></span></a>
                            <ul class="d-menu place-right" data-role="dropdown">
                                <li>
                                    <asp:HyperLink ID="but_show_thungrac" runat="server" Text="Xem thùng rác" />
                                </li>
                                <li>
                                    <asp:HyperLink ID="but_show_main" runat="server" Text="Về trang chính" Visible="false" />
                                </li>
                                <li class="divider"></li>
                                <li>
                                    <asp:LinkButton ID="but_khoiphuc" OnClick="but_khoiphuc_Click" runat="server" Text='Khôi phục' Visible="false" />
                                </li>
                                <li>
                                    <asp:LinkButton ID="but_xoa_vinh_vien" OnClick="but_xoa_vinh_vien_Click" runat="server" Text='Xóa vĩnh viễn' CssClass="fg-red" OnClientClick="return confirm('Dữ liệu sẽ không thể khôi phục! Bạn đã chắc chắn chưa?');" />
                                </li>
                                <%--<li class="divider"></li>
                        <li><a href="#">Office</a></li>
                        <li>
                            <a href="#" class="dropdown-toggle">Windows</a>
                            <ul class="d-menu" data-role="dropdown">
                                <li><a href="#">Windows 10</a></li>
                                <li><a href="#">Windows Server</a></li>
                                <li class="divider"></li>
                                <li><a href="#">MS-DOS</a></li>
                            </ul>
                        </li>--%>
                            </ul>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small></a>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                            <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Tới">
                            <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                        </li>
                    </ul>
                </div>
                <div id="timkiem-fixtop-bc" class="aha-admin-toolbar-search admin-fullpage-searchbar admin-route-searchbar d-none d-block-sm">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem" runat="server" placeholder="Nhập từ khóa" data-role="input" CssClass="input-small" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
            </div>

            <div class="<%--border-top bd-lightGray--%> <%--pt-3 pl-3-lg pl-0 pr-3-lg pr-0 pb-3--%>p-3">
                <div class="d-none-sm d-block">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Nhập từ khóa" data-role="input" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                    <div class="place-left">
                        <%--<b><%=ViewState["title"] %></b> Nó k kịp lưu vì nó tải trang này trước khi load menu-left--%>
                    </div>
                    <div class="place-right text-right">

                        <small class="pr-1">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label></small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>


                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container aha-admin-grid">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th class="text-left" style="width: 130px; min-width: 130px">Ngày gửi</th>
                                        <th class="text-left" style="min-width: 130px;">Tên khách</th>
                                        <th class="text-left" style="min-width: 130px;">Điện thoại</th>
                                        <th class="text-left" style="min-width: 300px;">Nội dung</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center">
                                                    <%# Eval("id") %>
                                                </td>
                                                <td class="checkbox-table">
                                                    <%--data-role="checkbox" data-style="2"--%>
                                                    <%--<input type="checkbox" onkeypress="if (event.keyCode==13) return false;" name="check_<%#Eval("id").ToString() %>">--%>
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td><small><%#Eval("ngay","{0:dd/MM/yyyy HH:mm}") %>'</small></td>

                                                <td>

                                                    <%#Eval("ten").ToString() %>

                                                </td>
                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                        <span class="fg-red"><%#Eval("sdt") %></span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                        <a class="fg-violet" href="tel:<%#Eval("sdt") %>"><%#Eval("sdt") %></a>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td style="word-break: break-all;">


                                                    <%#Eval("noidung") %>
                                                </td>

                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>


            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
