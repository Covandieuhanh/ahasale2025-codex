<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true"
    CodeFile="dangky.aspx.cs" Inherits="home_dangky" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>

    <style>
        /* ===== WRAP BACKGROUND ===== */
        .aha-reg-wrap {
            min-height: calc(100vh - 56px);
            background: linear-gradient(
                180deg,
                rgba(32, 201, 151, .14) 0%,
                rgba(32, 201, 151, .05) 35%,
                rgba(255,255,255,1) 100%
            );
            position: relative;
            overflow: hidden;
        }

        .aha-reg-wrap:before {
            content: "";
            position: absolute;
            inset: -140px -140px auto -140px;
            height: 360px;
            background:
                radial-gradient(circle at 30% 30%, rgba(32,201,151,.32), transparent 55%),
                radial-gradient(circle at 70% 40%, rgba(34,197,94,.22), transparent 60%);
            pointer-events: none;
        }

        /* ===== CONTAINER ===== */
        .aha-reg-container {
            padding-left: 16px;
            padding-right: 16px;
        }

        /* ===== CARD ===== */
        .aha-reg-card {
            max-width: 620px;
            margin: 0 auto;
            border: 1px solid rgba(98, 105, 118, .16);
            box-shadow: 0 12px 36px rgba(0,0,0,.08);
            border-radius: 12px;
        }

        .aha-reg-card .card-body {
            padding: 24px;
        }

        /* ===== REF BOX ===== */
        .aha-refbox {
            background: rgba(248,249,250,1);
            border: 1px solid rgba(98, 105, 118, .16);
            border-radius: 12px;
            padding: 16px;
        }

        /* ===== NOTE ===== */
        .aha-note {
            font-size: 12px;
            color: #626976;
        }

        /* ===== AVATAR PREVIEW ===== */
        .aha-preview img {
            max-width: 120px;
            border-radius: 10px;
            border: 1px solid rgba(98,105,118,.16);
        }
    </style>
</asp:Content>

<asp:Content ID="cHeadSau" ContentPlaceHolderID="head_sau" runat="Server"></asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_reg" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="aha-reg-wrap">
                <div class="page page-center">
                    <div class="container py-5 aha-reg-container">

                        <div class="card card-md aha-reg-card">
                            <div class="card-body">

                                <h2 class="h2 text-center mb-1">Đăng ký tài khoản mới</h2>
                                <div class="text-secondary text-center mb-4">Tạo tài khoản AHASALE.VN trong vài bước</div>

                                <!-- Người giới thiệu + loại tk -->
                                <div class="aha-refbox mb-4">
                                    <div class="row g-3">
                                        <div class="col-12">
                                            <label class="form-label">Người giới thiệu (không bắt buộc)</label>

                                            <asp:TextBox ID="txt_ref_taikhoan" runat="server"
                                                CssClass="form-control"
                                                AutoPostBack="true"
                                                OnTextChanged="txt_ref_taikhoan_TextChanged"
                                                placeholder="Nhập tài khoản người giới thiệu (có thể để trống)"></asp:TextBox>

                                            <div class="aha-note mt-2">Nếu nhập đúng tài khoản, hệ thống sẽ hiển thị họ tên bên dưới.</div>

                                            <div class="mt-2">
                                                <asp:Label ID="lb_ref_hoten" runat="server" CssClass="small"></asp:Label>
                                            </div>

                                            <!-- lưu trạng thái ref hợp lệ -->
                                            <asp:HiddenField ID="hf_ref_valid" runat="server" Value="" />
                                        </div>

                                        <div class="col-12">
                                            <label class="form-label">Loại tài khoản mặc định</label>
                                            <div class="form-control-plaintext fw-bold">
                                                <asp:Label ID="lb_phanloai_display" runat="server" Text="Khách hàng"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Số điện thoại đăng nhập -->
                                <div class="mb-3">
                                    <label class="form-label text-danger fw-bold">Số điện thoại đăng nhập</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text"><i class="ti ti-phone"></i></span>
                                        <asp:TextBox ID="txt_taikhoan" runat="server"
                                        CssClass="form-control"
                                        MaxLength="20"
                                        placeholder="Nhập số điện thoại"
                                        autocomplete="tel"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- Mật khẩu -->
                                <div class="mb-3">
                                    <label class="form-label text-danger fw-bold">Mật khẩu</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text"><i class="ti ti-key"></i></span>
                                        <asp:TextBox ID="txt_matkhau" runat="server"
                                            CssClass="form-control js-password"
                                            TextMode="Password"
                                            placeholder="Nhập mật khẩu"
                                            autocomplete="new-password"></asp:TextBox>
                                        <span class="input-group-text">
                                            <a href="javascript:void(0);" class="link-secondary js-toggle-password"
                                                aria-label="Hiện mật khẩu" data-bs-toggle="tooltip" data-bs-original-title="Hiện mật khẩu">
                                                <i class="ti ti-eye"></i>
                                            </a>
                                        </span>
                                    </div>
                                </div>

                                <!-- Email -->
                                <div class="mb-3">
                                    <label class="form-label text-danger fw-bold">Email</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text"><i class="ti ti-mail"></i></span>
                                        <asp:TextBox ID="txt_email" runat="server"
                                            CssClass="form-control"
                                            placeholder="email@domain.com"
                                            autocomplete="email"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- Ảnh -->
                                <div class="mb-3">
                                    <label class="form-label">Ảnh đại diện</label>
                                    <input type="file" id="fileInput" class="form-control"
                                        accept=".jpg,.jpeg,.png,.gif,.webp,.svg,.heic" onchange="uploadFile()" />
                                    <div id="message" runat="server" class="text-danger small mt-2"></div>
                                    <div id="uploadedFilePath" class="aha-preview mt-2"></div>
                                    <div style="display: none">
                                        <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- Họ tên -->
                                <div class="mb-3">
                                    <label class="form-label">Họ tên</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text"><i class="ti ti-id"></i></span>
                                        <asp:TextBox ID="txt_hoten" runat="server"
                                            CssClass="form-control"
                                            placeholder="Nhập họ tên"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- Ngày sinh -->
                                <div class="mb-3">
                                    <label class="form-label">Ngày sinh</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text"><i class="ti ti-calendar"></i></span>
                                        <asp:TextBox ID="txt_ngaysinh" runat="server"
                                            CssClass="form-control"
                                            MaxLength="10"
                                            placeholder="dd/MM/yyyy"></asp:TextBox>
                                    </div>
                                    <div class="aha-note mt-2">Định dạng: dd/MM/yyyy</div>
                                </div>

                                <!-- ✅ Checkbox đồng ý điều khoản -->
                                <div class="mb-4">
                                    <div class="form-check">
                                        <asp:CheckBox ID="cb_dongy" runat="server" CssClass="form-check-input" />
                                        <label class="form-check-label" for="<%= cb_dongy.ClientID %>">
                                            Bằng việc Đăng ký, bạn đã đọc và đồng ý với
                                            <a href="javascript:void(0);" class="link-primary"
                                               data-bs-toggle="modal" data-bs-target="#modal-dieukhoan">Điều khoản sử dụng</a>
                                            và
                                            <a href="javascript:void(0);" class="link-primary"
                                               data-bs-toggle="modal" data-bs-target="#modal-chinhsach">Chính sách bảo mật</a>
                                            của AhaSale
                                        </label>
                                    </div>
                                </div>

                                <div class="form-footer">
                                    <asp:Button ID="but_dangky" runat="server"
                                        Text="ĐĂNG KÝ"
                                        CssClass="btn btn-success w-100"
                                        OnClick="but_dangky_Click" />
                                </div>

                            </div>
                        </div>

                        <div class="text-center text-secondary mt-4">
                            <small>© AHASALE.VN</small>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ✅ MODAL: Điều khoản sử dụng -->
            <div class="modal modal-blur fade" id="modal-dieukhoan" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Điều khoản sử dụng</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            ...
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ✅ MODAL: Chính sách bảo mật -->
            <div class="modal modal-blur fade" id="modal-chinhsach" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Chính sách bảo mật</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            ...
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="txt_ref_taikhoan" EventName="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_reg" DisplayAfter="0">
        <ProgressTemplate>
            <div class="page-loading active">
                <div class="page-loading-card">
                    <div class="mb-2">
                        <span class="spinner-border" role="status" aria-hidden="true"></span>
                    </div>
                    <div class="text-secondary">Loading...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="cFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script type="text/javascript">
        // Upload handler GIỮ nguyên logic của bạn
        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            messageDiv.innerHTML = "";
            uploadedFilePathDiv.innerHTML = "";

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
                        uploadedFilePathDiv.innerHTML =
                            "<div class='text-secondary mb-2'><small>Ảnh mới chọn</small></div>" +
                            "<img width='120' src='" + xhr.responseText + "' />";
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
