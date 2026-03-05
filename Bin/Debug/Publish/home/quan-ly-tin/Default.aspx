<%@ page title="Quản lý tin" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="home_quan_ly_bai_Default, App_Web_15a4ezk0" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:HiddenField ID="hf_anhphu" runat="server" />
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 1100px; opacity: 1;'>
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
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1106px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <label class="fg-red fw-600">Tên bài viết</label>
                                        <asp:TextBox ID="txt_name" runat="server" data-role="input" MaxLength="100"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh chính</label>
                                        <input type="file" id="fileInput" onchange="uploadFile()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message"></div>
                                        <div id="uploadedFilePath"></div>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                        </div>

                                    </div>
                                </div>
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <label class="fg-red fw-600">Danh mục</label>
                                        <asp:DropDownList ID="ddl_DanhMuc" runat="server" data-role="select"></asp:DropDownList>
                                    </div>
                                </div>
                               <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Danh sách ảnh</label>
                                        <input type="file" id="fileListInput" multiple onchange="uploadMultipleFiles()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="multiMessage"></div>
                                        <div id="uploadedFileList"></div>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pr-4-lg">

                                    <div class="mt-3">
                                        <label class="fw-600">Giá bán (VNĐ)</label>
                                        <asp:TextBox ID="txt_giaban" onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)" runat="server" data-role="input" Text="0"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Thành phố</label>
                                        <asp:DropDownList ID="DanhSachTP" runat="server" data-role="select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pr-4-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Link google map</label>
                                        <asp:TextBox ID="LinkMap"  runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-12">

                                    <div class="mt-3">
                                        <label class="fw-600">Mô tả ngắn</label>
                                        <asp:TextBox ID="txt_description" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">Nội dung</label>
                                        <CKEditor:CKEditorControl ID="txt_noidung" runat="server" Height="300px" Width="100%" CustomConfig="/ckeditor/config-basic.js"></CKEditor:CKEditorControl>

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
        <ContentTemplate>
            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">
                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm">
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><span class="mif-plus"></span></asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" runat="server"><small>Đang bán</small></asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton ID="LinkButton2" OnClick="LinkButton2_Click" runat="server"><small>Ngưng bán</small></asp:LinkButton>
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

            <div class="container">
                <div class="d-none-sm d-block">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Nhập từ khóa" data-role="input" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                    <div class="place-left">
                        <%--<b><%=ViewState["title"] %></b> Nó k kịp lưu vì nó tải trang này trước khi load menu-left--%>
                    </div>
                    <div class="place-right text-right">

                        <small class="pr-1">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                        </small>
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
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th style="width: 1px;">Ảnh</th>
                                        <th class="text-left" style="min-width: 180px;">Tên sản phẩm</th>
                                        <th class="text-left" style="min-width: 180px">Mô tả</th>
                                        <th style="min-width: 90px;">Giá (VNĐ)</th>
                                        <th class="text-left" style="min-width: 180px">Danh mục</th>
                                        <th style="width: 86px; min-width: 86px">Ngày tạo</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center">
                                                    <%# Eval("id") %>
                                                </td>
                                                <td class="checkbox-table">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td class="text-center">
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src="<%# Eval("image") %>" class="img-cover-vuong" width="60" height="60" />
                                                    </div>
                                                </td>
                                                <td class="text-left">
                                                    <%#Eval("name") %>
                                                    <div>
                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'><span class="button mini rounded alert">Ngưng bán</span></asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'><span class="button mini rounded info">Đang bán</span></asp:PlaceHolder>
                                                    </div>
                                                </td>
                                                <td class="text-left">
                                                    <%#Eval("description") %>
                                                </td>
                                                <td class="text-right"><%#Eval("giaban","{0:#,##0}") %></td>
                                                <td class="text-left">
                                                    <%#Eval("TenMenu") %>
                                                    <br />
                                                    <%#Eval("TenMenu2") %>
                                                </td>

                                                <td><%#Eval("ngaytao","{0:dd/MM/yyyy}") %></td>
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

        function uploadMultipleFiles() {
            var fileInput = document.getElementById("fileListInput");
            var messageDiv = document.getElementById("multiMessage");
            var uploadedListDiv = document.getElementById("uploadedFileList");
            uploadedListDiv.innerHTML = ""; 
            var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
            var maxFileSize = 10 * 1024 * 1024; 
            if (fileInput.files.length === 0) {
                messageDiv.innerHTML = "Vui lòng chọn ít nhất một file.";
                return;
            }

            for (let i = 0; i < fileInput.files.length; i++) {
                let file = fileInput.files[i];
                let fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();

                if (!allowedExtensions.includes(fileExtension)) {
                    messageDiv.innerHTML += `<div>File "${file.name}" có định dạng không hợp lệ.</div>`;
                    continue;
                }

                if (file.size > maxFileSize) {
                    messageDiv.innerHTML += `<div>File "${file.name}" vượt quá dung lượng cho phép.</div>`;
                    continue;
                }

                let formData = new FormData();
                formData.append("file", file);

                let xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedListDiv.innerHTML += "<div style='display:inline-block;margin:5px'><img width='100' src='" + xhr.responseText + "' /></div>";
                        document.getElementById('<%= hf_anhphu.ClientID %>').value += xhr.responseText + "|";
                        console.log("With ClientID:", document.getElementById('<%= hf_anhphu.ClientID %>')); // OK?
                    } else {
                        messageDiv.innerHTML += `<div>Lỗi upload ảnh "${file.name}".</div>`;
                    }
                };
                xhr.send(formData);
            }
        }

    </script>
</asp:Content>

