<%@ Page Title="Chỉnh sửa thông tin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="edit-info.aspx.cs" Inherits="home_edit_info" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
    <style>
        .wrap { max-width: 1200px; }
        .overlay-loading{
            position:fixed; inset:0; background:rgba(0,0,0,.6);
            z-index:99999; display:flex; align-items:center; justify-content:center;
        }
        .overlay-panel{
            position:fixed; inset:0; z-index:2000;
            background:rgba(0,0,0,.55);
            overflow:auto; padding:16px 12px;
        }
        .overlay-dialog{ max-width: 720px; margin:0 auto; }
        .avatar-100{ width:100px; height:100px; border-radius:999px; object-fit:cover; }
        .img-100{ width:100px; height:100px; object-fit:cover; border-radius:10px; border:1px solid rgba(98,105,118,.18); background:#fff; }
        .card-soft{ border:1px solid rgba(98,105,118,.18); }
        .muted{ color:var(--tblr-muted); }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:TextBox ID="txtKieu" runat="server" style="visibility:hidden; height:0; width:0;" />

    <!-- ====== PANEL ADD/EDIT LINK (overlay) ====== -->
    <asp:UpdatePanel ID="up_themlink" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_themlink" runat="server" Visible="false" DefaultButton="btnLuuLink">
                <div class="overlay-panel">
                    <div class="overlay-dialog">
                        <div class="card shadow-sm">
                            <div class="card-header d-flex align-items-center justify-content-between">
                                <div class="card-title fw-bold">Thêm/sửa link mạng xã hội</div>
                                <a href="#" id="btnCloseLink" runat="server" onserverclick="btnDong_Click" title="Đóng"
                                   class="btn btn-ghost-danger btn-icon">
                                    <i class="ti ti-x"></i>
                                </a>
                            </div>

                            <div class="card-body">
                                <asp:HiddenField ID="hfIdLink" runat="server" />

                                <div class="mb-3">
                                    <label class="form-label">Tên</label>
                                    <asp:TextBox ID="txtTen" runat="server" CssClass="form-control" placeholder="Tên" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Link</label>
                                    <asp:TextBox ID="txtLink" runat="server" CssClass="form-control" placeholder="Ví dụ: https://www.facebook.com" />
                                    <asp:Label ID="lblLinkError" runat="server" CssClass="text-danger small" Visible="false"></asp:Label>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Icon</label>
                                    <input type="file" id="fileInput3" onchange="uploadFile3()" class="form-control" />
                                    <div id="message3" class="text-danger small mt-1"></div>
                                    <div id="uploadedFilePath3" class="mt-2"></div>

                                    <div style="display:none">
                                        <asp:TextBox ID="TxtIcon" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="mt-2">
                                        <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                                    </div>

                                    <div id="uploadedFileContainer" runat="server" visible="false" class="mt-3">
                                        <div class="small text-muted mb-1">Ảnh hiện tại:</div>
                                        <asp:Image ID="previewImage" runat="server" CssClass="img-100" />
                                        <div class="mt-2">
                                            <asp:Button ID="btnRemoveImage" runat="server" Text="Xoá ảnh"
                                                CssClass="btn btn-outline-danger btn-sm"
                                                OnClick="removeUploadedImage" />
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div class="card-footer d-flex align-items-center justify-content-between">
                                <a href="#" id="btnCloseLink2" runat="server" onserverclick="btnDong_Click"
                                   class="btn btn-link text-decoration-none">Hủy</a>
                                <asp:Button ID="btnLuuLink" runat="server" Text="Lưu" CssClass="btn btn-primary px-4" OnClick="btnLuuLink_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ====== MAIN ====== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" DefaultButton="but_capnhat">

                <div class="container-xl wrap py-4">
                    <div class="row g-3">

                        <!-- left profile card -->
                        <div class="col-lg-4">
                            <div class="card card-soft shadow-sm">
                                <div class="card-body">
                                    <div class="text-center">
                                        <img src="<%= ViewState["avt_query"] %>" class="avatar-100" alt="avatar" />
                                    </div>

                                    <div class="text-center fw-bold mt-2">
                                        <small><%= ViewState["hoten_query"] %></small>
                                    </div>

                                    <div class="mt-3 p-3 rounded bg-muted-lt text-center">
                                        <small><asp:Literal ID="Literal4" runat="server"></asp:Literal></small>
                                    </div>

                                    <div class="text-center mt-3">
                                        <a class="text-decoration-none" href="tel:<%= ViewState["sdt_query"] %>">
                                            <small><i class="ti ti-phone me-1"></i><%= ViewState["sdt_query"] %></small>
                                        </a>
                                    </div>

                                    <div class="text-center mt-2">
                                        <i class="ti ti-map-pin"></i>
                                        <small><asp:Literal ID="Literal5" runat="server"></asp:Literal></small>
                                    </div>

                                    <div class="d-flex align-items-center justify-content-between gap-2 mt-3 p-2 rounded bg-dark-lt">
                                        <div><%= ViewState["phanloai_query"] %></div>
                                        <div class="btn btn-outline-secondary btn-sm">
                                            <img src="/uploads/images/dong-a.png" alt="A" style="width:18px;height:18px;object-fit:contain" />
                                            <span class="fw-bold ms-1"><%= ViewState["DongA_query"] %></span>
                                        </div>
                                    </div>

                                    <div class="d-flex gap-2 mt-3">
                                        <asp:Button ID="but_show_form_naptien" runat="server" Text="Nạp Quyền tiêu dùng"
                                            CssClass="btn btn-warning w-100" />
                                        <asp:Button ID="Button4" runat="server" Text="Lịch sử Trao đổi"
                                            CssClass="btn btn-outline-dark w-100" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- right edit tabs -->
                        <div class="col-lg-8">
                            <div class="card card-soft shadow-sm">
                                <div class="card-body">

                                    <!-- Tabler/Bootstrap tabs -->
                                    <ul class="nav nav-tabs" data-bs-toggle="tabs" role="tablist" id="tab-kieu">
                                        <li class="nav-item" role="presentation">
                                            <a class="nav-link active" href="#target_cn" data-bs-toggle="tab" role="tab" aria-selected="true">
                                                <i class="ti ti-user me-1"></i>Cá nhân
                                            </a>
                                        </li>

                                        <!-- ✅ TAB CỬA HÀNG: CHỈ HIỆN KHI KHÔNG BỊ KHÓA -->
                                        <asp:PlaceHolder ID="phTabCuaHang" runat="server" Visible="true">
                                            <li class="nav-item" role="presentation">
                                                <a class="nav-link" href="#target_ch" data-bs-toggle="tab" role="tab" aria-selected="false">
                                                    <i class="ti ti-building-store me-1"></i>Cửa hàng
                                                </a>
                                            </li>
                                        </asp:PlaceHolder>
                                    </ul>

                                    <div class="tab-content pt-3">

                                        <!-- ===== Cá nhân ===== -->
                                        <div class="tab-pane active show" id="target_cn" role="tabpanel">
                                            <div class="row g-3">

                                                <div class="col-12">
                                                    <div class="fw-bold"><small>Ảnh đại diện</small></div>
                                                    <input type="file" id="fileInput" onchange="uploadFile()" class="form-control" />
                                                    <div id="message" runat="server" class="text-danger small mt-1"></div>
                                                    <div id="uploadedFilePath" class="mt-2"></div>
                                                    <div style="display:none">
                                                        <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="mt-2">
                                                        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div class="mt-2">
                                                        <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ"
                                                            CssClass="btn btn-outline-danger btn-sm"
                                                            Visible="false" OnClick="Button2_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-lg-6">
                                                    <label class="form-label">Họ tên</label>
                                                    <asp:TextBox ID="txt_hoten" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-6">
                                                    <label class="form-label">Điện thoại</label>
                                                    <asp:TextBox ID="txt_sdt" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-6">
                                                    <label class="form-label">Email</label>
                                                    <asp:TextBox ID="TextBox9" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-6"></div>

                                                <div class="col-12">
                                                    <label class="form-label">Địa chỉ</label>
                                                    <asp:TextBox ID="txt_diachi" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-12">
                                                    <label class="form-label">Giới thiệu</label>
                                                    <asp:TextBox ID="txt_gioithieu" runat="server" TextMode="MultiLine" Rows="3"
                                                        CssClass="form-control" placeholder="Tối đa 60 ký tự"></asp:TextBox>
                                                </div>

                                                <!-- social links cá nhân -->
                                                <div class="col-12">
                                                    <asp:Repeater ID="rptMangXaHoiCN" runat="server" OnItemCommand="rptMangXaHoi_ItemCommand">
                                                        <ItemTemplate>
                                                            <div class="d-flex align-items-center justify-content-between border rounded p-2 mb-2">
                                                                <a href='<%# Eval("Link") %>' target="_blank" class="text-decoration-none text-reset">
                                                                    <div class="d-flex align-items-center">
                                                                        <asp:Image ID="imgIcon" runat="server"
                                                                            ImageUrl='<%# Eval("Icon") %>'
                                                                            Width="50" Height="50"
                                                                            Style="object-fit: cover; border-radius: 8px; margin-right: 10px;"
                                                                            Visible='<%# !string.IsNullOrEmpty(Eval("Icon") as string) %>' />
                                                                        <div style='<%# string.IsNullOrEmpty(Eval("Icon") as string) ? "margin-left:60px;" : "" %>'>
                                                                            <div class="fw-bold"><small><%# Eval("Ten") %></small></div>
                                                                            <div class="small text-muted" style="font-style: italic;"><%# Eval("Link") %></div>
                                                                        </div>
                                                                    </div>
                                                                </a>

                                                                <div class="d-flex align-items-center gap-2">
                                                                    <asp:Button ID="btnSua" runat="server" Text="Sửa" ToolTip="Sửa"
                                                                        CssClass="btn btn-sm btn-primary"
                                                                        CommandName="EditItem"
                                                                        CommandArgument='<%# Eval("ID") %>' />
                                                                    <asp:Button ID="btnXoa" runat="server" Text="X" ToolTip="Xóa"
                                                                        CssClass="btn btn-sm btn-danger"
                                                                        CommandName="DeleteItem"
                                                                        CommandArgument='<%# Eval("ID") %>' />
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>

                                            </div>
                                        </div>

                                        <!-- ✅ THÔNG BÁO KHI SHOP BỊ KHÓA -->
                                        <asp:PlaceHolder ID="phShopLockedNote" runat="server" Visible="false">
                                            <div class="tab-pane" id="target_ch" role="tabpanel">
                                                <div class="alert alert-warning">
                                                    Bạn đã đăng ký/đang là <b>Gian hàng đối tác</b> nên <b>không thể chỉnh sửa thông tin cửa hàng</b> ở đây.
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>

                                        <!-- ===== Cửa hàng (chỉ hiển thị khi không bị khóa) ===== -->
                                        <asp:PlaceHolder ID="phShopEdit" runat="server" Visible="true">
                                            <div class="tab-pane" id="target_ch" role="tabpanel">
                                                <div class="row g-3">

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Logo</label>
                                                        <input type="file" id="fileInput1" onchange="uploadFile1()" class="form-control" />
                                                        <div id="message1" runat="server" class="text-danger small mt-1"></div>
                                                        <div id="uploadedFilePath1" class="mt-2"></div>
                                                        <div style="display:none">
                                                            <asp:TextBox ID="txt_link_fileupload1" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="mt-2"><asp:Label ID="Label3" runat="server" Text=""></asp:Label></div>
                                                        <div class="mt-2">
                                                            <asp:Button ID="Button1" runat="server" Text="Xóa ảnh cũ"
                                                                CssClass="btn btn-outline-danger btn-sm"
                                                                Visible="false" OnClick="Button1_Click" />
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Ảnh bìa</label>
                                                        <input type="file" id="fileInput2" onchange="uploadFile2()" class="form-control" />
                                                        <div id="message2" runat="server" class="text-danger small mt-1"></div>
                                                        <div id="uploadedFilePath2" class="mt-2"></div>
                                                        <div style="display:none">
                                                            <asp:TextBox ID="txt_link_fileupload2" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="mt-2"><asp:Label ID="Label4" runat="server" Text=""></asp:Label></div>
                                                        <div class="mt-2">
                                                            <asp:Button ID="Button3" runat="server" Text="Xóa ảnh cũ"
                                                                CssClass="btn btn-outline-danger btn-sm"
                                                                Visible="false" OnClick="Button3_Click" />
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Tên cửa hàng</label>
                                                        <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Điện thoại</label>
                                                        <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Email</label>
                                                        <asp:TextBox ID="TextBox10" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Số Zalo</label>
                                                        <asp:TextBox ID="TextBox11" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-12">
                                                        <label class="form-label">Mô tả ngắn</label>
                                                        <asp:TextBox ID="TextBox5" runat="server" TextMode="MultiLine" Rows="3"
                                                            CssClass="form-control" placeholder="Tối đa 60 ký tự"></asp:TextBox>
                                                    </div>

                                                    <div class="col-12">
                                                        <label class="form-label">Địa chỉ</label>
                                                        <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <!-- social links cửa hàng -->
                                                    <div class="col-12">
                                                        <asp:Repeater ID="rptMangXaHoiCH" runat="server" OnItemCommand="rptMangXaHoi_ItemCommand">
                                                            <ItemTemplate>
                                                                <div class="d-flex align-items-center justify-content-between border rounded p-2 mb-2">
                                                                    <a href='<%# Eval("Link") %>' target="_blank" class="text-decoration-none text-reset">
                                                                        <div class="d-flex align-items-center">
                                                                            <asp:Image ID="imgIcon" runat="server"
                                                                                ImageUrl='<%# Eval("Icon") %>'
                                                                                Width="50" Height="50"
                                                                                Style="object-fit: cover; border-radius: 8px; margin-right: 10px;"
                                                                                Visible='<%# !string.IsNullOrEmpty(Eval("Icon") as string) %>' />

                                                                            <div style='<%# string.IsNullOrEmpty(Eval("Icon") as string) ? "margin-left:60px;" : "" %>'>
                                                                                <div class="fw-bold"><small><%# Eval("Ten") %></small></div>
                                                                                <div class="small text-muted" style="font-style: italic;"><%# Eval("Link") %></div>
                                                                            </div>
                                                                        </div>
                                                                    </a>

                                                                    <div class="d-flex align-items-center gap-2">
                                                                        <asp:Button ID="btnSua" runat="server" Text="Sửa" ToolTip="Sửa"
                                                                            CssClass="btn btn-sm btn-primary"
                                                                            CommandName="EditItem"
                                                                            CommandArgument='<%# Eval("ID") %>' />
                                                                        <asp:Button ID="btnXoa" runat="server" Text="X" ToolTip="Xóa"
                                                                            CssClass="btn btn-sm btn-danger"
                                                                            CommandName="DeleteItem"
                                                                            CommandArgument='<%# Eval("ID") %>' />
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>

                                                </div>
                                            </div>
                                        </asp:PlaceHolder>

                                    </div>

                                    <div class="d-flex justify-content-end gap-2 mt-4">
                                        <asp:Button ID="but_themlink" runat="server" Text="+ Thêm link"
                                            CssClass="btn btn-outline-primary"
                                            OnClick="but_themlink_Click" />
                                        <asp:Button ID="but_capnhat" OnClick="but_capnhat_Click" runat="server" Text="Cập nhật"
                                            CssClass="btn btn-primary px-4" />
                                    </div>

                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="overlay-loading">
                <div class="spinner-border" role="status" aria-label="loading"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
    <script>
        function setKieuFromTab() {
            var isCN = document.getElementById('target_cn')?.classList.contains('active');
            document.getElementById('<%= txtKieu.ClientID %>').value = isCN ? "Cá nhân" : "Cửa hàng";
        }

        document.addEventListener('shown.bs.tab', function () {
            setTimeout(setKieuFromTab, 50);
        });

        document.addEventListener('DOMContentLoaded', function () {
            setKieuFromTab();
        });

        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload.";
                };
                xhr.send(formData);
            } else messageDiv.innerHTML = "Vui lòng chọn file.";
        }

        function uploadFile1() {
            var fileInput = document.getElementById("fileInput1");
            var messageDiv = document.getElementById("message1");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath1");

            if (fileInput && fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload1.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload.";
                };
                xhr.send(formData);
            } else if (messageDiv) messageDiv.innerHTML = "Vui lòng chọn file.";
        }

        function uploadFile2() {
            var fileInput = document.getElementById("fileInput2");
            var messageDiv = document.getElementById("message2");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath2");

            if (fileInput && fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload2.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload.";
                };
                xhr.send(formData);
            } else if (messageDiv) messageDiv.innerHTML = "Vui lòng chọn file.";
        }

        function uploadFile3() {
            var fileInput = document.getElementById("fileInput3");
            var messageDiv = document.getElementById("message3");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath3");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= TxtIcon.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload." + xhr.responseText;
                };
                xhr.send(formData);
            } else messageDiv.innerHTML = "Vui lòng chọn file.";
        }
    </script>
</asp:Content>
