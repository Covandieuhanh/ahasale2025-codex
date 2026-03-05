<%@ page title="Quản lý thông báo" language="C#" masterpagefile="~/admin/MasterPageAdmin.master" autoeventwireup="true" inherits="admin_quan_ly_thong_bao_Default, App_Web_tqosnw1a" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">


    <asp:UpdatePanel ID="up_loc" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
        <asp:AsyncPostBackTrigger ControlID="but_show_form_loc" EventName="Click" />
    </Triggers>--%>
        <ContentTemplate>
            <asp:Panel ID="pn_loc" runat="server" Visible="false" DefaultButton="but_loc">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 900px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_loc" onserverclick="but_show_form_loc_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                LỌC DỮ LIỆU
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 906px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
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
                                            <asp:ListItem Text="Dựa vào ngày thông báo" Value="1"></asp:ListItem>
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
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_xuat" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
            <asp:AsyncPostBackTrigger ControlID="but_show_form_xuat" EventName="Click" />
        </Triggers>--%>
        <ContentTemplate>
            <asp:Panel ID="pn_xuat" runat="server" Visible="false" DefaultButton="but_xuat_excel">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 700px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_xuat" onserverclick="but_show_form_xuat_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                XUẤT EXCEL
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 706px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <div class="fw-600">Chọn mục muốn xuất</div>
                                        <div class="mt-1">
                                            <asp:CheckBox Checked="true" ID="check_all_excel" runat="server" CssClass="text-bold" Text="Tất cả các mục" OnCheckedChanged="check_all_CheckedChanged" AutoPostBack="true" />
                                        </div>
                                        <asp:CheckBoxList ID="check_list_excel" runat="server" AutoPostBack="true" OnSelectedIndexChanged="check_list_excel_SelectedIndexChanged">
                                            <asp:ListItem Text="ID" Value="id" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Nội dung" Value="noidung" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Thời gian" Value="thoigian" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Người thông báo" Value="ten_nguoithongbao" Selected="true"></asp:ListItem>
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
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>

    <asp:UpdatePanel ID="up_in" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_in" runat="server" Visible="false">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 700px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="close_in" runat="server" onserverclick="but_close_form_in_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                IN THEO NHU CẦU CỦA BẠN
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 706px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
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
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:AsyncPostBackTrigger ControlID="but_add" EventName="Click" />--%>
        </Triggers>
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">
                        <li data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" class="pl-4 pr-4">
                            <input data-role="checkbox" data-style="2" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Đánh dấu">
                            <%--class="dropdown-toggle"--%>
                            <a href="#"><span class="mif-checkmark"></span></a>
                            <ul class="d-menu" data-role="dropdown">
                                <li>
                                    <asp:LinkButton ID="but_danhdau_dadoc_nhieumuc" runat="server" Text='Đánh dấu đã đọc' OnClick="but_danhdau_dadoc_nhieumuc_Click" />
                                </li>
                                <li>
                                    <asp:LinkButton ID="but_danhdau_chuadoc_nhieumuc" runat="server" Text='Đánh dấu chưa đọc' OnClick="but_danhdau_chuadoc_nhieumuc_Click" />
                                </li>
                            </ul>
                        </li>


                        <li data-role="hint" data-hint-position="top" data-hint-text="Về trang chính">
                            <asp:LinkButton ID="but_quayve_trangchu" OnClick="but_quayve_trangchu_Click" runat="server" Visible="false"><span class="mif-undo"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Di chuyển vào thùng rác">
                            <asp:LinkButton ID="but_remove_bin" OnClick="but_remove_bin_Click" runat="server"><span class="mif-bin"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:LinkButton ID="but_show_form_loc" runat="server" OnClick="but_show_form_loc_Click"><span class="mif-filter"></span></asp:LinkButton>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel">
                            <asp:LinkButton ID="but_show_form_xuat" runat="server" OnClick="but_show_form_xuat_Click"><span class="mif-file-excel"></span></asp:LinkButton>
                        </li>
                        <li data-role="hint" data-hint-position="top" data-hint-text="In">
                            <asp:LinkButton ID="but_show_form_in" runat="server" OnClick="but_show_form_in_Click"><span class="mif-print"></span></asp:LinkButton>
                        </li>
                        <li>
                            <%--class="dropdown-toggle"--%>
                            <a href="#"><span class="mif-more-vert"></span></a>
                            <ul class="d-menu place-right" data-role="dropdown">
                                <li>
                                    <asp:LinkButton ID="but_show_thungrac" OnClick="but_show_thungrac_Click" runat="server" Text='Xem thùng rác' />
                                </li>
                                <li>
                                    <asp:LinkButton ID="but_show_main" OnClick="but_show_main_Click" runat="server" Text='Về trang chính' Visible="false" />
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
                <div id="timkiem-fixtop-bc" style="position: fixed; right: 10px; top: 58px; width: 240px; z-index: 4" class="d-none d-block-sm">
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


                <div class="text-left p-3">

                    <asp:Button ID="but_sapxep_moinhat" OnClick="but_sapxep_moinhat_Click" runat="server" Text="Mới nhất" CssClass="light small rounded" />
                    <asp:Button ID="but_sapxep_chuadoc" OnClick="but_sapxep_chuadoc_Click" runat="server" Text="Chưa đọc" CssClass="light small rounded" />
                </div>



                <div class="bg-white" style="max-width: 344px">
                    <%-- +24 của ô checkbox--%>
                    <asp:Repeater ID="Repeater1" runat="server"  OnItemDataBound="Repeater1_ItemDataBound">
                        <ItemTemplate>
                            <span style="display: none">
    <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
</span>
                            <div class="thongbao-div pt-2 pb-2 pl-3 pr-3">
                                <div class="checkbox-table" style="float: left; width: 24px">
                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                </div>
                                <a href="<%#Eval("link").ToString()%>idtb=<%#Eval("id").ToString() %>">
                                    <div class="thongbao-avt">
                                        <img src="<%#Eval("avt_nguoithongbao") %>" width="50" height="50" />
                                    </div>
                                    <div class="thongbao-noidung">
                                        <div class="thongbao-noidungchinh">
                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("daxem").ToString()=="True" %>'>
                                                <%#Eval("noidung").ToString() %>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("daxem").ToString()=="False" %>'>
                                                <span class="fg-orange"><%#Eval("noidung").ToString() %></span>
                                            </asp:PlaceHolder>
                                        </div>
                                        <div class="thongbao-thoigian"><%#Eval("thoigian","{0:dd/MM/yyyy HH:mm}") %>'</div>
                                    </div>
                                </a>
                                <div class="thongbao-hanhdong">
                                    <div class="dropdown-button bg-transparent">
                                        <span class="button <%--dropdown-toggle--%> bg-transparent"><span class="text-bold" style="font-size: 18px">...</span></span>
                                    </div>
                                    <ul class="d-menu place-right" data-role="dropdown">
                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("daxem").ToString()=="False" %>'>
                                            <li>
                                                <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_dadoc" OnClick="but_dadoc_Click" runat="server" Text="Đánh dấu đã đọc"></asp:LinkButton>
                                            </li>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("daxem").ToString()=="True" %>'>
                                            <li>
                                                <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_chuadoc" OnClick="but_chuadoc_Click" runat="server" Text="Đánh dấu chưa đọc"></asp:LinkButton>
                                            </li>
                                        </asp:PlaceHolder>
                                        <li>
                                            <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_xoathongbao" OnClick="but_xoathongbao_Click" runat="server" Text="Xóa thông báo này"></asp:LinkButton>
                                        </li>
                                    </ul>
                                </div>

                                <div class="clr-bc"></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
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
</asp:Content>


