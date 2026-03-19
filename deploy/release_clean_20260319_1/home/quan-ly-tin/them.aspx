<%@ Page Title="Đăng tin mới" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="them.aspx.cs" Inherits="home_quan_ly_bai_Them" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        .wrap { max-width: 1200px; }
        .field-note { font-size: 13px; color: #6c7a89; margin-top: 6px; }
        .preview-grid img {
            width: 100px;
            height: 100px;
            object-fit: cover;
            border-radius: 8px;
            border: 1px solid rgba(98,105,118,.18);
            margin: 6px 6px 0 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl wrap py-4">
        <asp:HiddenField ID="hf_anhphu" runat="server" />
        <div style="display: none">
            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
        </div>

        <div class="card shadow-sm">
            <div class="card-header d-flex align-items-center justify-content-between">
                <div class="card-title fw-bold">ĐĂNG TIN MỚI</div>
                <asp:HyperLink ID="lnk_back_top" runat="server" CssClass="btn btn-outline-secondary">
                    <i class="ti ti-arrow-left me-1"></i>Quay lại danh sách
                </asp:HyperLink>
            </div>

            <div class="card-body">
                <div class="row g-3">
                    <div class="col-lg-6">
                        <label class="form-label">Tên bài viết</label>
                        <asp:TextBox ID="txt_name" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                    </div>

                    <div class="col-lg-6">
                        <label class="form-label">Ảnh chính / Video chính</label>
                        <input type="file" id="fileInput" onchange="uploadFile()" class="form-control"
                            accept="image/*,video/mp4,video/webm,video/ogg,video/quicktime,video/x-m4v,video/3gpp,video/avi,video/x-msvideo,video/x-matroska" />
                        <div id="message" class="text-danger small mt-1"></div>
                        <div id="uploadedFilePath" class="mt-2"></div>
                    </div>

                    <div class="col-lg-6">
                        <label class="form-label">Danh mục</label>
                        <asp:DropDownList ID="ddl_DanhMuc" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <div class="col-lg-6">
                        <label class="form-label">Loại tin</label>
                        <asp:DropDownList ID="ddl_loai_tin" runat="server" CssClass="form-select">
                            <asp:ListItem Value="sanpham">Sản phẩm</asp:ListItem>
                            <asp:ListItem Value="dichvu">Dịch vụ</asp:ListItem>
                        </asp:DropDownList>
                        <div class="field-note">Dịch vụ sẽ hiển thị nút Đặt lịch và thanh toán online.</div>
                    </div>

                    <div class="col-lg-6">
                        <label class="form-label">Danh sách media (ảnh/video)</label>
                        <input type="file" id="fileListInput" multiple onchange="uploadMultipleFiles()" class="form-control"
                            accept="image/*,video/mp4,video/webm,video/ogg,video/quicktime,video/x-m4v,video/3gpp,video/avi,video/x-msvideo,video/x-matroska" />
                        <div id="multiMessage" class="text-danger small mt-1"></div>
                        <div id="uploadedFileList" class="preview-grid mt-2"></div>
                    </div>

                    <div class="col-lg-6">
                        <label class="form-label">Giá bán (VNĐ)</label>
                        <asp:TextBox ID="txt_giaban" runat="server"
                            onfocus="AutoSelect(this)"
                            oninput="format_sotien_new(this)"
                            CssClass="form-control" Text="0" MaxLength="14"></asp:TextBox>
                    </div>

                    <div class="col-lg-6">
                        <label class="form-label">Phần trăm ưu đãi (0 - 50%)</label>
                        <asp:TextBox ID="txt_phantram_uu_dai" runat="server"
                            CssClass="form-control" Text="0" MaxLength="2"
                            oninput="clamp_percent_0_50(this)"></asp:TextBox>
                        <div class="field-note">Mặc định 0%. Sẽ được trừ tại hồ sơ ưu đãi của người mua.</div>
                    </div>

                    <asp:PlaceHolder ID="ph_company_shop_options" runat="server" Visible="false">
                        <div class="col-lg-6">
                            <label class="form-label">Kênh hiển thị sản phẩm</label>
                            <asp:DropDownList ID="ddl_kenh_hienthi" runat="server" CssClass="form-select">
                                <asp:ListItem Value="public">Công khai (hiển thị ngoài Home)</asp:ListItem>
                                <asp:ListItem Value="internal">Nội bộ (chỉ shop công ty tự bán)</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-lg-6">
                            <label class="form-label">% chiết khấu cho sàn (0 - 100%)</label>
                            <asp:TextBox ID="txt_phantram_san" runat="server"
                                CssClass="form-control" Text="0" MaxLength="3"
                                oninput="clamp_percent_0_100(this)"></asp:TextBox>
                            <div class="field-note">Áp dụng cho luồng bán sản phẩm của shop công ty.</div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="col-lg-12">
                        <label class="form-label">Tỉnh/Thành - Quận/Huyện - Phường/Xã</label>
                        <div class="row g-2">
                            <div class="col-md-4">
                                <select id="post_tinh" class="form-select"></select>
                            </div>
                            <div class="col-md-4">
                                <select id="post_quan" class="form-select"></select>
                            </div>
                            <div class="col-md-4">
                                <select id="post_phuong" class="form-select"></select>
                            </div>
                        </div>
                        <div class="field-note">Chọn đủ 3 cấp để hệ thống hiển thị đúng vị trí.</div>
                    </div>

                    <div class="col-lg-12">
                        <label class="form-label">Địa chỉ chi tiết</label>
                        <asp:TextBox ID="txt_diachi_chitiet" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control"></asp:TextBox>
                    </div>

                    <div class="col-lg-12">
                        <label class="form-label">Link google map</label>
                        <asp:TextBox ID="LinkMap" runat="server" CssClass="form-control"></asp:TextBox>
                        <div class="field-note">Nếu để trống, hệ thống sẽ tự tạo link từ địa chỉ đã chọn.</div>
                    </div>

                    <asp:HiddenField ID="hf_tinh" runat="server" />
                    <asp:HiddenField ID="hf_quan" runat="server" />
                    <asp:HiddenField ID="hf_phuong" runat="server" />
                    <asp:HiddenField ID="hf_address_raw" runat="server" />

                    <div class="col-lg-12">
                        <label class="form-label">Mô tả ngắn</label>
                        <asp:TextBox ID="txt_description" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    </div>

                    <div class="col-lg-12">
                        <label class="form-label">Nội dung</label>
                        <CKEditor:CKEditorControl ID="txt_noidung" runat="server" Height="340px" Width="100%" CustomConfig="/ckeditor/config-basic.js"></CKEditor:CKEditorControl>
                    </div>
                </div>
            </div>

            <div class="card-footer d-flex align-items-center justify-content-between">
                <asp:HyperLink ID="lnk_cancel" runat="server" CssClass="btn btn-link text-decoration-none">Hủy</asp:HyperLink>
                <asp:Button ID="but_submit" runat="server" Text="ĐĂNG TIN" CssClass="btn btn-success px-4" OnClick="but_submit_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
    <script src="<%= Helper_cl.VersionedUrl("~/js/aha-address-picker.js") %>"></script>
    <script>
        function clamp_percent_0_50(el) {
            el.value = (el.value || "").replace(/[^\d]/g, "");
            if (el.value === "") return;
            var v = parseInt(el.value, 10);
            if (isNaN(v)) { el.value = ""; return; }
            if (v < 0) v = 0;
            if (v > 50) v = 50;
            el.value = v.toString();
        }

        function clamp_percent_0_100(el) {
            el.value = (el.value || "").replace(/[^\d]/g, "");
            if (el.value === "") return;
            var v = parseInt(el.value, 10);
            if (isNaN(v)) { el.value = ""; return; }
            if (v < 0) v = 0;
            if (v > 100) v = 100;
            el.value = v.toString();
        }

        function isVideoExt(ext) {
            var videos = [".mp4", ".mov", ".webm", ".m4v", ".ogv", ".3gp", ".avi", ".mkv"];
            return videos.indexOf((ext || "").toLowerCase()) !== -1;
        }

        function guessVideoMime(fileNameOrExt) {
            var value = (fileNameOrExt || "").toLowerCase();
            if (value.indexOf(".") >= 0) value = value.substring(value.lastIndexOf("."));
            if (value === ".webm") return "video/webm";
            if (value === ".ogv") return "video/ogg";
            if (value === ".mov") return "video/quicktime";
            if (value === ".m4v") return "video/x-m4v";
            if (value === ".3gp") return "video/3gpp";
            if (value === ".avi") return "video/x-msvideo";
            if (value === ".mkv") return "video/x-matroska";
            return "video/mp4";
        }

        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            messageDiv.innerHTML = "";
            if (fileInput.files.length === 0) {
                messageDiv.innerHTML = "Vui lòng chọn file.";
                return;
            }

            var file = fileInput.files[0];
            var allowedExtensions = [
                ".jpg", ".jpeg", ".jpe", ".png", ".gif", ".webp", ".svg", ".heic", ".heif",
                ".bmp", ".tif", ".tiff", ".avif", ".jfif",
                ".mp4", ".mov", ".webm", ".m4v", ".ogv", ".3gp", ".avi", ".mkv"
            ];
            var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
            if (allowedExtensions.indexOf(fileExtension) === -1) {
                messageDiv.innerHTML = "Định dạng không hợp lệ. Hỗ trợ ảnh và video ngắn.";
                return;
            }

            var maxFileSize = isVideoExt(fileExtension) ? (80 * 1024 * 1024) : (20 * 1024 * 1024);
            if (file.size > maxFileSize) {
                messageDiv.innerHTML = isVideoExt(fileExtension)
                    ? "Vui lòng chọn video nhỏ hơn 80 MB."
                    : "Vui lòng chọn ảnh nhỏ hơn 20 MB.";
                return;
            }

            var formData = new FormData();
            formData.append("file", file);
            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
            xhr.onload = function () {
                if (xhr.status === 200) {
                    var url = (xhr.responseText || "").trim();
                    if (isVideoExt(fileExtension)) {
                        uploadedFilePathDiv.innerHTML =
                            "<div class='small text-muted mb-1'>Video mới chọn</div>" +
                            "<video class='img-fluid rounded' style='max-width:220px;aspect-ratio:1/1;object-fit:cover' controls muted playsinline preload='metadata'>" +
                            "<source src='" + url + "' type='" + guessVideoMime(fileExtension) + "'></video>";
                    } else {
                        uploadedFilePathDiv.innerHTML =
                            "<div class='small text-muted mb-1'>Ảnh mới chọn</div>" +
                            "<img class='img-fluid rounded' style='max-width:220px;aspect-ratio:1/1;object-fit:cover' src='" + url + "' />";
                    }
                    document.getElementById('<%= txt_link_fileupload.ClientID %>').value = url;
                } else {
                    messageDiv.innerHTML = "Lỗi upload: " + (xhr.responseText || "Không xác định");
                }
            };
            xhr.send(formData);
        }

        function uploadMultipleFiles() {
            var fileInput = document.getElementById("fileListInput");
            var messageDiv = document.getElementById("multiMessage");
            var uploadedListDiv = document.getElementById("uploadedFileList");
            messageDiv.innerHTML = "";
            uploadedListDiv.innerHTML = "";

            var allowedExtensions = [
                ".jpg", ".jpeg", ".jpe", ".png", ".gif", ".webp", ".svg", ".heic", ".heif",
                ".bmp", ".tif", ".tiff", ".avif", ".jfif",
                ".mp4", ".mov", ".webm", ".m4v", ".ogv", ".3gp", ".avi", ".mkv"
            ];
            var maxFileSizeImage = 20 * 1024 * 1024;
            var maxFileSizeVideo = 80 * 1024 * 1024;
            if (fileInput.files.length === 0) {
                messageDiv.innerHTML = "Vui lòng chọn ít nhất một file.";
                return;
            }

            for (let i = 0; i < fileInput.files.length; i++) {
                let file = fileInput.files[i];
                let fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                let isVideo = isVideoExt(fileExtension);
                if (!allowedExtensions.includes(fileExtension)) {
                    messageDiv.innerHTML += "<div>File \"" + file.name + "\" có định dạng không hợp lệ.</div>";
                    continue;
                }
                if (file.size > (isVideo ? maxFileSizeVideo : maxFileSizeImage)) {
                    messageDiv.innerHTML += isVideo
                        ? "<div>Video \"" + file.name + "\" vượt quá 80 MB.</div>"
                        : "<div>Ảnh \"" + file.name + "\" vượt quá 20 MB.</div>";
                    continue;
                }

                let formData = new FormData();
                formData.append("file", file);
                let xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        var url = (xhr.responseText || "").trim();
                        if (isVideo) {
                            uploadedListDiv.innerHTML +=
                                "<video style='width:100px;height:100px;object-fit:cover;border-radius:8px;border:1px solid rgba(98,105,118,.18);margin:6px 6px 0 0' controls muted playsinline preload='metadata'>" +
                                "<source src='" + url + "' type='" + guessVideoMime(fileExtension) + "'></video>";
                        } else {
                            uploadedListDiv.innerHTML += "<img src='" + url + "' />";
                        }
                        document.getElementById('<%= hf_anhphu.ClientID %>').value += url + "|";
                    } else {
                        messageDiv.innerHTML += "<div>Lỗi upload \"" + file.name + "\": " + (xhr.responseText || "Không xác định") + "</div>";
                    }
                };
                xhr.send(formData);
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            if (!window.AhaAddressPicker) return;
            AhaAddressPicker.init({
                provinceSelectId: "post_tinh",
                districtSelectId: "post_quan",
                wardSelectId: "post_phuong",
                detailInputId: "<%= txt_diachi_chitiet.ClientID %>",
                hiddenAddressId: "<%= hf_address_raw.ClientID %>",
                hiddenProvinceId: "<%= hf_tinh.ClientID %>",
                hiddenDistrictId: "<%= hf_quan.ClientID %>",
                hiddenWardId: "<%= hf_phuong.ClientID %>",
                rawAddressId: "<%= hf_address_raw.ClientID %>"
            });
        });
    </script>
</asp:Content>
