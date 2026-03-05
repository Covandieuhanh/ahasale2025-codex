<%@ page title="Quản lý tài khoản" language="C#" masterpagefile="~/admin/MasterPageAdmin.master" autoeventwireup="true" inherits="admin_Default, App_Web_5jqmh4il" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_phanquyen" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
       <asp:AsyncPostBackTrigger ControlID="but_show_form_xuat" EventName="Click" />
   </Triggers>--%>
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
                                                <asp:ListItem Text="Chuyển điểm đến các tài khoản ví tổng" Value="q1_1" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ ví tổng Khách hàng đến các tài khoản Khách hàng" Value="q1_2" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ ví tổng Gian hàng đối tác đến các tài khoản Gian hàng đối tác" Value="q1_3" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ ví tổng Đồng hành hệ sinh thái đến các tài khoản Đồng hành hệ sinh thái" Value="q1_4" Selected="false"></asp:ListItem>
                                                <asp:ListItem Text="Chuyển điểm từ ví tổng Cộng tác phát triển đến các tài khoản Cộng tác phát triển" Value="q1_5" Selected="false"></asp:ListItem>
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


    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
            <asp:AsyncPostBackTrigger ControlID="but_show_form_add" EventName="Click" />
        </Triggers>--%>
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
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
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
                                                <asp:ListItem Value="Cộng tác phát triển" Text="Cộng tác phát triển"></asp:ListItem>
                                                <asp:ListItem Value="Đồng hành hệ sinh thái" Text="Đồng hành hệ sinh thái"></asp:ListItem>
                                                <asp:ListItem Value="Gian hàng đối tác" Text="Gian hàng đối tác"></asp:ListItem>
                                                <asp:ListItem Value="Khách hàng" Text="Khách hàng"></asp:ListItem>
                                                <asp:ListItem Value="Ví tổng" Text="Ví tổng"></asp:ListItem>
                                            </asp:DropDownList>
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


    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:AsyncPostBackTrigger ControlID="but_add" EventName="Click" />--%>
        </Triggers>
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm">
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><span class="mif-plus"></span></asp:LinkButton>
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

                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container">
                            <%--style="padding-bottom: 300px"--%>
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>

                                        <th class="text-center" style="width: 60px; min-width: 60px;">Ảnh</th>
                                        <th class="text-center" style="min-width: 1px;">Tài khoản</th>
                                        <th class="text-center" style="min-width: 50px;">Quyền tiêu dùng</th>
                                        <th class="text-center" style="min-width: 140px;">Họ tên</th>
                                        <th class="text-center" style="width: 60px; min-width: 60px;">Ngày sinh</th>
                                        <th class="text-center" style="width: 60px; min-width: 60px;">Điện thoại</th>
                                        <th class="text-center" style="width: 60px; min-width: 60px;">Email</th>
                                        <th class="text-center" style="min-width: 1px;"></th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <%--OnItemDataBound="Repeater1_ItemDataBound"--%>
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("taikhoan") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center">
                                                    <%# Eval("id") %>
                                                </td>
                                                <%--<td class="text-center"><%# Container.ItemIndex + 1 %></td>--%>
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
                                                        <%--<div>Mật khẩu:  <%# Eval("matkhau") %></div>--%>
                                                    </div>

                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("phanloai").ToString()=="Cộng tác phát triển" %>'>
                                                        <div class="button mini dark rounded">Cộng tác phát triển</div>
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
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("phanloai").ToString()=="Ví tổng" %>'>
                                                        <div class="button mini bg-violet fg-white rounded">Ví tổng</div>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td>
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%#Eval("DongA","{0:#,##0}") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <div class="fw-600"><%#Eval("hoten") %></div>
                                                </td>
                                                <td>
                                                    <%#Eval("ngaysinh","{0:dd/MM/yyyy}") %>
                                                </td>
                                                <td>
                                                    <div><a title="Gọi" href="tel:<%#Eval("dienthoai") %>"><%#Eval("dienthoai") %></a></div>
                                                </td>
                                                <td class="text-left"><%#Eval("email") %></td>
                                                <td style="vertical-align: middle">
                                                    <div class="dropdown-button place-right">
                                                        <button class="button small bg-transparent">
                                                            <span class="mif mif-more-horiz"></span>
                                                        </button>
                                                        <ul class="d-menu place-right" data-role="dropdown">
                                                            <%--<li><a href="#">Chỉnh sửa</a></li>
            <li><a href="#">Đổi mật khẩu</a></li>--%>
                                                            <li>
                                                                <asp:LinkButton ID="but_show_form_phanquyen" OnClick="but_show_form_phanquyen_Click" CommandArgument='<%#Eval("taikhoan") %>' runat="server">Phân quyền</asp:LinkButton></li>
                                                            <%-- <li class="divider"></li>
            <li><a href="#">Sao chép thông tin đăng nhập</a></li>
            <li class="divider"></li>
            <li><a href="#">Đang làm việc</a></li>
            <li><a href="#">Đang nghỉ phép</a></li>
            <li><a href="#">Đã nghỉ việc</a></li>--%>
                                                            <%--<li class="divider"></li>--%>
                                                        </ul>
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

