<%@ Page Title="Quản lý tài khoản" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .aff-current-node > a,
        .aff-current-node > span {
            color: #d60000 !important;
            font-weight: 700 !important;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <!-- ======================= POPUP CÂY AFFILIATE (NEW) ======================= -->
    <asp:UpdatePanel ID="up_aff_tree" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_aff_tree" runat="server" Visible="false">

                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 650px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="A_close_aff_tree" onserverclick="but_close_form_aff_tree_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                CÂY AFFILIATE
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 656px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="mt-3 mb-3">
                                <div class="text-bold">
                                    Tài khoản đang xem: 
                                <asp:Label ID="lb_aff_current" runat="server" CssClass="fg-red"></asp:Label>
                                </div>
                            </div>

                            <!-- MetroUI4 TreeView -->
                            <div class="mt-2 mb-4">
                                <div data-role="treeview" data-on-node-click="false">
                                    <asp:Literal ID="lit_aff_tree" runat="server"></asp:Literal>
                                </div>
                            </div>

                            <div class="mb-10"></div>
                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress_aff_tree" runat="server" AssociatedUpdatePanelID="up_aff_tree">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true">
                        <span class="electron"></span><span class="electron"></span><span class="electron"></span>
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ======================= POPUP PHÂN QUYỀN ======================= -->
    <asp:UpdatePanel ID="up_phanquyen" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_phanquyen" runat="server" Visible="false" DefaultButton="but_phanquyen">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 550px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="A1" onserverclick="but_close_form_phanquyen_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                PHÂN QUYỀN TÀI KHOẢN
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 556px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <small>
                                <div class="row">
                                    <div class="cell-lg-12">

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_quanlynhanvien" runat="server" CssClass="text-bold" Text="QUẢN LÝ TÀI KHOẢN" OnCheckedChanged="check_all_quyen_quanlynhanvien_CheckedChanged" AutoPostBack="true" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_quanlynhanvien" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_quanlynhanvien_SelectedIndexChanged">
                                                <asp:ListItem Text="Phân quyền cho tài khoản khác" Value="5" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Các quyền còn lại (tạm thời)" Value="1" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_1" runat="server" CssClass="text-bold" Text="CHUYỂN ĐIỂM" OnCheckedChanged="check_all_quyen_1_CheckedChanged" AutoPostBack="true" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_1_SelectedIndexChanged">
                                                <asp:ListItem Text="Xem lịch sử chuyển điểm (Toàn hệ thống)" Value="q1_6" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Xem lịch sử chuyển điểm (Được phân quyền)" Value="q1_7" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm đến các tài khoản tổng" Value="q1_1" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ tài khoản tổng Khách hàng đến các tài khoản Khách hàng" Value="q1_2" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ tài khoản tổng Gian hàng đối tác đến các tài khoản Gian hàng đối tác" Value="q1_3" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ tài khoản tổng Đồng hành hệ sinh thái đến các tài khoản Đồng hành hệ sinh thái" Value="q1_4" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ tài khoản tổng Cộng tác phát triển đến các tài khoản Cộng tác phát triển" Value="q1_5" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_hoso" runat="server" CssClass="text-bold" Text="QUYỀN THEO 5 HỒ SƠ" OnCheckedChanged="check_all_quyen_hoso_CheckedChanged" AutoPostBack="true" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_hoso" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_quyen_hoso_SelectedIndexChanged">
                                                <asp:ListItem Text="Hồ sơ quyền tiêu dùng (điểm A, bán thẻ/chuyển điểm A)" Value="q2_1" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Hồ sơ quyền ưu đãi (duyệt ghi nhận điểm + đổi tầng Khách hàng)" Value="q2_2" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Hồ sơ hành vi lao động (duyệt ghi nhận điểm + đổi tầng Cộng tác phát triển)" Value="q2_3" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Hồ sơ chỉ số gắn kết (duyệt ghi nhận điểm + đổi tầng Đồng hành hệ sinh thái)" Value="q2_4" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Hồ sơ gian hàng đối tác (duyệt ghi nhận điểm ShopOnly tiêu dùng + ưu đãi)" Value="q2_5" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                </div>
                            </small>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_phanquyen" runat="server" CssClass="success" Text="Phân quyền" OnClick="but_phanquyen_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="up_phanquyen">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= POPUP ADD/EDIT ======================= -->
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="close_add" runat="server" onserverclick="but_close_form_add_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-12">

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Tài khoản</label>
                                        <div>
                                            <asp:TextBox ID="txt_taikhoan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                                        <div class="mt-3">
                                            <label class="fw-600 fg-red">Mật khẩu</label>
                                            <div>
                                                <asp:TextBox ID="txt_matkhau" TextMode="Password" runat="server" data-role="input"></asp:TextBox>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Loại tài khoản</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList1" runat="server" data-role="select">
                                                <asp:ListItem Value="Nhân viên admin" Text="Nhân viên admin"></asp:ListItem>
                                                <asp:ListItem Value="Tài khoản tổng" Text="Tài khoản tổng"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

<div class="mt-3">
    <label class="fw-600">Tầng Home (chỉnh bởi admin có quyền)</label>
    <div>
        <asp:DropDownList ID="ddl_cap_sp" runat="server" data-role="select">
            <asp:ListItem Value="1" Text="Khách hàng"></asp:ListItem>
            <asp:ListItem Value="2" Text="Cộng tác phát triển"></asp:ListItem>
            <asp:ListItem Value="3" Text="Đồng hành hệ sinh thái"></asp:ListItem>
        </asp:DropDownList>
    </div>
</div>


                                    <!-- ✅ NEW: Người giới thiệu -->
                                    <div class="mt-3">
                                        <label class="fw-600">Người giới thiệu</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_nguoi_gioi_thieu" runat="server" data-role="select"></asp:DropDownList>
                                        </div>

                                    </div>

                      




                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Email</label>
                                        <div>
                                            <asp:TextBox ID="txt_email" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh đại diện</label>
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

                                    <div class="mt-3">
                                        <label class="fw-600">Họ tên</label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày sinh</label>
                                        <div>
                                            <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <div>
                                            <asp:TextBox ID="txt_dienthoai" runat="server" data-role="input"></asp:TextBox>
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
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= POPUP LỌC (NEW) ======================= -->
    <asp:UpdatePanel ID="up_filter" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_filter" runat="server" Visible="false" DefaultButton="but_apdung_loc">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 520px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="close_filter" runat="server" onserverclick="but_close_form_filter_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                LỌC DANH SÁCH
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 526px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-12">

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Phân loại tài khoản</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_loc_phanloai" runat="server" data-role="select">
                                                <asp:ListItem Value="" Text="-- Tất cả --"></asp:ListItem>
                                                <asp:ListItem Value="Nhân viên admin" Text="Nhân viên admin"></asp:ListItem>
                                                <asp:ListItem Value="Cộng tác phát triển" Text="Cộng tác phát triển"></asp:ListItem>
                                                <asp:ListItem Value="Đồng hành hệ sinh thái" Text="Đồng hành hệ sinh thái"></asp:ListItem>
                                                <asp:ListItem Value="Gian hàng đối tác" Text="Gian hàng đối tác"></asp:ListItem>
                                                <asp:ListItem Value="Khách hàng" Text="Khách hàng"></asp:ListItem>
                                                <asp:ListItem Value="Tài khoản tổng" Text="Tài khoản tổng"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Phạm vi hệ</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_loc_scope" runat="server" data-role="select">
                                                <asp:ListItem Value="" Text="-- Tất cả hệ --"></asp:ListItem>
                                                <asp:ListItem Value="admin" Text="Hệ admin"></asp:ListItem>
                                                <asp:ListItem Value="home" Text="Hệ home"></asp:ListItem>
                                                <asp:ListItem Value="shop" Text="Hệ shop"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_apdung_loc" runat="server" Text="Áp dụng lọc" CssClass="button success" OnClick="but_apdung_loc_Click" />
                                <asp:Button ID="but_xoa_loc" runat="server" Text="Xóa lọc" CssClass="button alert" OnClick="but_xoa_loc_Click" />
                            </div>

                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress6" runat="server" AssociatedUpdatePanelID="up_filter">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= MAIN ======================= -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <asp:HiddenField ID="hf_selected_taikhoan" runat="server" ClientIDMode="Static" />
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <!-- nút thêm -->
                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm">
                            <asp:HyperLink ID="but_show_form_add" runat="server" NavigateUrl="<%# BuildAddUrl() %>"><span class="mif-plus"></span></asp:HyperLink>
                        </li>

                        <!-- ✅ NEW: icon lọc -->
                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:HyperLink ID="but_show_filter" runat="server" NavigateUrl="<%# BuildFilterUrl() %>"><span class="mif-filter"></span></asp:HyperLink>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Tìm kiếm">
                            <asp:LinkButton ID="but_timkiem" ClientIDMode="Static" OnClick="but_timkiem_Click" runat="server"><span class="mif-search"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Làm mới">
                            <asp:LinkButton ID="but_xoa_timkiem" OnClick="but_xoa_timkiem_Click" runat="server"><span class="mif-loop2"></span></asp:LinkButton>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small>
                            </a>
                        </li>

                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                            <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Tới">
                            <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                        </li>
                    </ul>
                </div>

                <div id="timkiem-fixtop-bc" style="position: fixed; right: 10px; top: 58px; width: 240px; z-index: 4" class="d-none d-block-sm">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem" runat="server" placeholder="Nhập từ khóa" data-role="input" CssClass="input-small" AutoPostBack="false" data-sync-key="qltk-search" data-enter-click="but_timkiem"></asp:TextBox>
                </div>
            </div>

            <div class="p-3">
                <div class="d-none-sm d-block">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Nhập từ khóa" data-role="input" AutoPostBack="false" data-sync-key="qltk-search" data-enter-click="but_timkiem"></asp:TextBox>
                </div>

                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
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
                        <div class="bcorn-fix-title-table-container">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>

                                        <th class="text-center" style="width: 60px; min-width: 60px;">Ảnh</th>
                                        <th class="text-center" style="min-width: 1px;">Tài khoản</th>
                                        <th class="text-center" style="min-width: 50px;">Quyền tiêu dùng</th>

                                        <!-- ✅ NEW -->
                                       <%-- <th class="text-center" style="min-width: 90px;">Ví 1 (20%)</th>
                                        <th class="text-center" style="min-width: 90px;">Ví 2 (30%) EVoucher</th>
                                        <th class="text-center" style="min-width: 90px;">Ví 3 (50%)</th>--%>

                                        <th class="text-center" style="min-width: 140px;">Họ tên</th>
                                        <th class="text-center" style="min-width: 220px;">Hành vi</th>



                                        <!-- ✅ NEW: Người giới thiệu -->
                                        <th class="text-center" style="min-width: 140px;">Người giới thiệu</th>

                                        <th class="text-center" style="width: 90px; min-width: 90px;">% DV Cho sàn</th>

                                        <th class="text-center" style="width: 60px; min-width: 60px;">Ngày sinh</th>
                                        <th class="text-center" style="width: 60px; min-width: 60px;">Điện thoại</th>
                                        <th class="text-center" style="width: 60px; min-width: 60px;">Email</th>
                                        <th class="text-center" style="min-width: 120px;">Thao tác</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server" EnableViewState="false" ViewStateMode="Disabled">
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("taikhoan") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center">
                                                    <%# Eval("id") %>
                                                </td>
                                                <td class="checkbox-table text-center">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td class="text-center">
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src='<%#Eval("anhdaidien") %>' class="img-cover-vuongtron" width="60" height="60" />
                                                    </div>
                                                </td>
                                                <td class="text-left" style="vertical-align: middle">
                                                    <div>
                                                        <%# Eval("taikhoan") %>
                                                    </div>

                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("phanloai").ToString()=="Cộng tác phát triển" %>'>
                                                        <div class="button mini dark rounded">Cộng tác phát triển</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder19b" runat="server" Visible='<%#Eval("phanloai").ToString()=="Nhân viên admin" %>'>
                                                        <div class="button mini dark rounded">Nhân viên admin</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("phanloai").ToString()=="Đồng hành hệ sinh thái" %>'>
                                                        <div class="button mini alert rounded">Đồng hành hệ sinh thái</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("phanloai").ToString()=="Gian hàng đối tác" %>'>
                                                        <div class="button mini yellow rounded">Gian hàng đối tác</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("phanloai").ToString()=="Khách hàng" %>'>
                                                        <div class="button mini success rounded">Khách hàng</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%# AccountType_cl.IsTreasury(Eval("phanloai").ToString()) %>'>
                                                        <div class="button mini bg-violet fg-white rounded">Tài khoản tổng</div>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-left">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("DongA","{0:#,##0.##}") %></div>
                                                </td>

                                                <!-- ✅ NEW: 3 ví (đã được code-behind đổi null => 0) -->
                                               <%-- <td class="text-left">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("Vi3_20PhanTram_ViGanKet","{0:#,##0.##}") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("Vi1_30PhanTram_ViEVoucher","{0:#,##0.##}") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("Vi2_50PhanTram_ViLaoDong","{0:#,##0.##}") %></div>
                                                </td>--%>

                                                <td class="text-left">
                                                    <div class="fw-600"><%#Eval("hoten") %></div>
                                                </td>
                                                <td class="text-center">
                                                    <div class="button mini light rounded">
                                                        <%# Eval("HanhVi_HienThi") %>
                                                    </div>
                                                </td>


                                                <!-- ✅ NEW: Người giới thiệu -->
                                                <td class="text-left">
                                                    <div class="fw-600"><%# Eval("NguoiGioiThieu_HienThi") %></div>

                                                    <div class="mt-1">
                                                        <asp:HyperLink ID="hl_xem_cay_aff" runat="server"
                                                            CssClass="fg-blue fg-cyan-hover"
                                                            Text="Xem chi tiết"
                                                            NavigateUrl='<%# BuildAffiliateTreeUrl(Eval("taikhoan")) %>'>
                                                        </asp:HyperLink>
                                                    </div>
                                                </td>


                                                <td class="text-center">
                                                    <div class="button mini light rounded">
                                                        <%# Eval("ChiPhanTram_BanDichVu_ChoSan") %>%
                                                    </div>
                                                </td>

                                                <td>
                                                    <%#Eval("ngaysinh","{0:dd/MM/yyyy}") %>
                                                </td>
                                                <td>
                                                    <div><a title="Gọi" href="tel:<%#Eval("dienthoai") %>"><%#Eval("dienthoai") %></a></div>
                                                </td>
                                                <td class="text-left"><%#Eval("email") %></td>

                                                <td style="vertical-align: middle" class="text-center">
                                                    <div class="d-flex flex-wrap justify-content-center align-items-center">
                                                        <asp:HyperLink ID="hl_chi_tiet" runat="server" CssClass="button small primary rounded mr-1" NavigateUrl='<%# BuildEditUrl(Eval("taikhoan")) %>' Text="Chi tiết"></asp:HyperLink>
                                                        <asp:HyperLink ID="hl_show_form_phanquyen" runat="server" CssClass="button small warning rounded" NavigateUrl='<%# BuildPermissionUrl(Eval("taikhoan")) %>' Visible='<%# Convert.ToBoolean(Eval("CanShowPhanQuyen")) %>'>Phân quyền</asp:HyperLink>
                                                    </div>
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
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];

                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                var maxFileSize = 10 * 1024 * 1024;
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
                        uploadedFilePathDiv.innerHTML = "<div><small>Ảnh mới chọn<small></div><img width='100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText;
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
