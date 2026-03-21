<%@ Page Title="Quản lý menu" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
     <style>
 
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div class="admin-fullpage-head admin-route-panel-head">
                    <div class='admin-fullpage-shell admin-fullpage-head-shell admin-route-panel-shell admin-route-panel-head-shell'>
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_add" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div class="admin-fullpage-body-wrap admin-route-panel-body-wrap">
                    <div class='admin-fullpage-shell admin-fullpage-dialog admin-route-panel-shell admin-route-panel-dialog'>
                        <div class="bg-white border bd-transparent admin-fullpage-body admin-route-panel-body pl-4 pl-8-md pr-8-md pr-4">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <label class="fg-red fw-600">Tên menu</label>
                                        <asp:TextBox ID="txt_name" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Menu cha</label>
                                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Nếu Menu bạn sắp thêm là Menu cấp 1 thì không chọn Menu cha.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <asp:DropDownList ID="ddl_DanhMuc" runat="server" data-role="select"></asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Thứ tự hiển thị</label>
                                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Menu nào có thứ tự hiển thị <b>nhỏ hơn</b> sẽ ưu tiên <b>hiển thị trước</b>. Chỉ chấp nhận số nguyên dương.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <asp:TextBox ID="txt_rank" MaxLength="4" oninput="format_sotien_new(this)" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Mô tả ngắn</label>
                                        <asp:TextBox ID="txt_description" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Đường dẫn khác</label>
                                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Nếu bạn muốn Menu này liên kết với một trang khác thì hãy điền URL đó vào. Nếu không, hãy để trống ô này.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <asp:TextBox ID="txt_url" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh thu nhỏ</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Đây là ảnh sẽ hiển thị khi bạn chia sẻ link Website của bạn qua mạng xã hội.<br />Kích thước chuẩn: 1200x628 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <input type="file" id="fileInput" onchange="uploadFile()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message" runat="server"></div>
                                        <div id="uploadedFilePath"></div>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div style='position: absolute; bottom: 0px; left: 100px'>
                                            <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="button success" OnClick="but_add_edit_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

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
                                            <asp:ListItem Text="Dựa vào ngày tạo" Value="1"></asp:ListItem>
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
                                            <asp:ListItem Text="Tên menu" Value="name" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Menu cha" Value="name_parent" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Cấp" Value="id_level" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Thứ tự" Value="rank" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Ngày tạo" Value="ngaytao" Selected="true"></asp:ListItem>
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
                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm">
                            <asp:HyperLink ID="but_show_form_add" runat="server"><span class="mif-plus"></span></asp:HyperLink>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Lưu">
                            <asp:LinkButton ID="but_save" OnClick="but_save_Click" runat="server"><span class="mif-floppy-disk"></span></asp:LinkButton>
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
                    <div class="cell-lg-8 pr-3-lg">
                        <div class="bcorn-fix-title-table-container aha-admin-grid">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>

                                        <th class="text-left" style="min-width: 150px;">Tên menu</th>
                                        <th class="text-left" style="min-width: 150px">Menu cha</th>
                                        <th style="width: 1px;">Cấp</th>
                                        <%--<th class="text-center" style="min-width: 100px; width: 100px;">Menu con</th>--%>
                                        <th class="text-center" style="width: 62px; min-width: 62px">Thứ tự</th>
                                        <th style="width: 86px; min-width: 86px">Ngày tạo</th>
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
                                                <%--<td class="text-center"><%# Container.ItemIndex + 1 %></td>--%>
                                                <td class="checkbox-table">
                                                    <%--data-role="checkbox" data-style="2"--%>
                                                    <%--<input type="checkbox" onkeypress="if (event.keyCode==13) return false;" name="check_<%#Eval("id").ToString() %>">--%>
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td class="text-left">
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                        <asp:HyperLink CssClass="fg-red  fg-darkRed-hover" data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" ID="but_name_1" NavigateUrl='<%# BuildEditUrl(Eval("id")) %>' runat="server">
                                                   <%#Eval("name") %>
                                                        </asp:HyperLink>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                        <asp:HyperLink CssClass="fg-green fg-darkGreen-hover" data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" ID="but_name_2" NavigateUrl='<%# BuildEditUrl(Eval("id")) %>' runat="server">
                                               <%#Eval("name") %>
                                                        </asp:HyperLink>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td>
                                                    <%#Eval("name_parent") %>
                                                </td>
                                                <td class="text-center"><%#Eval("id_level") %></td>
                                                <td><%--CssClass="textbox-style-bc"--%>
                                                    <asp:TextBox data-role="input" CssClass="input-small" data-clear-button="false" oninput="format_sotien_new(this)" ID="txt_rank_1" Width="50" MaxLength="4" runat="server" Text='<%#Eval("rank") %>' onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>
                                                </td>
                                                <td><%#Eval("ngaytao","{0:dd/MM/yyyy}") %></td>
                                                <%-- <td>
                                                <a href="#" class="mif mif-more-horiz fg-black"></a>
                                                <ul class="d-menu place-right" data-role="dropdown">
                                                    <li>
                                                        <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_show_form_sua" runat="server" Text='Chỉnh sửa' />
                                                    </li>
                                                    <li class="divider"></li>
                                                    <asp:PlaceHolder ID="PlaceHolder14" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                        <li>
                                                            <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_remove_bin_only" OnClick="but_remove_bin_only_Click" runat="server" Text='Di chuyển vào thùng rác' />
                                                        </li>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                        <li class="divider"></li>
                                                        <li>
                                                            <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_khoi_phuc_only" OnClick="but_khoi_phuc_only_Click" runat="server" Text='Khôi phục' />
                                                        </li>
                                                        <li>
                                                            <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_xoa_vinh_vien_only" OnClick="but_xoa_vinh_vien_only_Click" runat="server" Text='Xóa vĩnh viễn' CssClass="fg-red" OnClientClick="return confirm('Dữ liệu sẽ không thể khôi phục! Bạn đã chắc chắn chưa?');" />
                                                        </li>
                                                    </asp:PlaceHolder>
                                                </ul>
                                                <div class="dropdown-button place-right">
                                                    <button class="button small bg-transparent">
                                                        <span class="mif mif-more-horiz"></span>
                                                    </button>
                                                    <ul class="d-menu place-right" data-role="dropdown">
                                                        <li>
                                                            <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="LinkButton1" runat="server" Text='Chỉnh sửa' />
                                                        </li>
                                                        <li class="divider"></li>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                            <li>
                                                                <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="LinkButton3" OnClick="but_remove_bin_only_Click" runat="server" Text='Di chuyển vào thùng rác' />
                                                            </li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                            <li class="divider"></li>
                                                            <li>
                                                                <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="LinkButton4" OnClick="but_khoi_phuc_only_Click" runat="server" Text='Khôi phục' />
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="LinkButton5" OnClick="but_xoa_vinh_vien_only_Click" runat="server" Text='Xóa vĩnh viễn' CssClass="fg-red" OnClientClick="return confirm('Dữ liệu sẽ không thể khôi phục! Bạn đã chắc chắn chưa?');" />
                                                            </li>
                                                        </asp:PlaceHolder>
                                                    </ul>
                                                </div>
                                            </td>--%>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="cell-lg-4 pl-3-lg mt-0-lg mt-8">
                        <div class=" p-4 bg-white">
                            <b>Cây menu</b>
                            <ul style='font-size: 13px' data-role='treeview'>
                                <asp:Label ID="lb_tree_view" runat="server" Text=""></asp:Label>
                            </ul>
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
    <%--ảnh opengraph của menu--%>
    <script>
        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        //messageDiv.innerHTML = "File uploaded successfully!";
                        uploadedFilePathDiv.innerHTML = "<div><small>Ảnh mới chọn<small></div><img width='100' src='" + xhr.responseText + "' />"; // Hiển thị ảnh
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText;// Hiển thị đường dẫn
                    } else {
                        messageDiv.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }
    </script>
</asp:Content>
