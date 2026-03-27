<%@ Page Title="Đăng tin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="Them.aspx.cs" Inherits="gianhang_quan_ly_tin_Them" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        .gh-form-shell { max-width: 1100px; margin: 0 auto; }
        .gh-field-note { font-size: 13px; color: #6b7280; margin-top: 6px; }
        .gh-main-preview img {
            width: 180px;
            height: 180px;
            object-fit: cover;
            border-radius: 14px;
            border: 1px solid rgba(98,105,118,.18);
            background: #f8fafc;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl gh-form-shell py-4">
        <div style="display:none">
            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
        </div>

        <div class="card shadow-sm">
            <div class="card-header d-flex align-items-center justify-content-between">
                <div>
                    <div class="page-pretitle">Gian hàng</div>
                    <div class="card-title fw-bold"><asp:Label ID="lb_form_title" runat="server" Text="Đăng tin mới"></asp:Label></div>
                </div>
                <asp:HyperLink ID="lnk_back" runat="server" NavigateUrl="/gianhang/quan-ly-tin/Default.aspx" CssClass="btn btn-outline-secondary">Quay lại danh sách</asp:HyperLink>
            </div>

            <div class="card-body">
                <div class="row g-3">
                    <div class="col-lg-6">
                        <label class="form-label">Tên tin</label>
                        <asp:TextBox ID="txt_name" runat="server" CssClass="form-control" MaxLength="120"></asp:TextBox>
                    </div>

                    <div class="col-lg-3">
                        <label class="form-label">Loại tin</label>
                        <asp:DropDownList ID="ddl_loai" runat="server" CssClass="form-select">
                            <asp:ListItem Value="sanpham">Sản phẩm</asp:ListItem>
                            <asp:ListItem Value="dichvu">Dịch vụ</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-lg-3">
                        <label class="form-label">Danh mục</label>
                        <asp:DropDownList ID="ddl_danhmuc" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <div class="col-lg-6">
                        <label class="form-label">Ảnh chính</label>
                        <input type="file" id="fileInput" onchange="uploadFile()" class="form-control" accept="image/*" />
                        <div id="message" class="text-danger small mt-1"></div>
                        <div id="uploadedFilePath" class="gh-main-preview mt-2">
                            <asp:Literal ID="lit_uploaded_main" runat="server"></asp:Literal>
                        </div>
                    </div>

                    <div class="col-lg-3">
                        <label class="form-label">Giá bán (VNĐ)</label>
                        <asp:TextBox ID="txt_giaban" runat="server" CssClass="form-control" Text="0" MaxLength="14" onfocus="AutoSelect(this)" oninput="format_sotien_new(this)"></asp:TextBox>
                    </div>

                    <div class="col-lg-3">
                        <label class="form-label">Ưu đãi thanh toán (%)</label>
                        <asp:TextBox ID="txt_phantram_uu_dai" runat="server" CssClass="form-control" Text="0" MaxLength="2"></asp:TextBox>
                        <div class="gh-field-note">Từ 0 đến 50%. Mức ưu đãi sẽ áp dụng khi thanh toán.</div>
                    </div>

                    <div class="col-lg-3">
                        <label class="form-label">Trạng thái hiển thị</label>
                        <div class="form-check form-switch mt-2">
                            <asp:CheckBox ID="chk_hidden" runat="server" CssClass="form-check-input" />
                            <label class="form-check-label" for="<%= chk_hidden.ClientID %>">Ẩn tin khỏi trang công khai</label>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <label class="form-label">Mô tả ngắn</label>
                        <asp:TextBox ID="txt_description" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                    <div class="col-lg-12">
                        <label class="form-label">Nội dung</label>
                        <asp:TextBox ID="txt_noidung" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="10"></asp:TextBox>
                        <div class="gh-field-note">Nội dung sẽ được lưu đầy đủ và hiển thị tại trang chi tiết.</div>
                    </div>
                </div>
            </div>

            <div class="card-footer d-flex align-items-center justify-content-between">
                <asp:HyperLink ID="lnk_cancel" runat="server" NavigateUrl="/gianhang/quan-ly-tin/Default.aspx" CssClass="btn btn-link text-decoration-none">Hủy</asp:HyperLink>
                <asp:Button ID="but_submit" runat="server" CssClass="btn btn-primary px-4" Text="Lưu tin" OnClick="but_submit_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
    <script>
        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");
            messageDiv.innerHTML = "";

            if (!fileInput || fileInput.files.length === 0) {
                messageDiv.innerHTML = "Vui lòng chọn ảnh.";
                return;
            }

            var file = fileInput.files[0];
            var ext = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
            var allowed = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg", ".avif", ".jfif"];
            if (allowed.indexOf(ext) === -1) {
                messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                return;
            }

            if (file.size > 20 * 1024 * 1024) {
                messageDiv.innerHTML = "Vui lòng chọn ảnh nhỏ hơn 20 MB.";
                return;
            }

            var formData = new FormData();
            formData.append("file", file);
            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
            xhr.onload = function () {
                if (xhr.status === 200) {
                    var url = (xhr.responseText || "").trim();
                    uploadedFilePathDiv.innerHTML =
                        "<div class='small text-muted mb-1'>Ảnh đã tải lên</div>" +
                        "<img src='" + url + "' alt='' />";
                    document.getElementById('<%= txt_link_fileupload.ClientID %>').value = url;
                } else {
                    messageDiv.innerHTML = "Lỗi upload: " + (xhr.responseText || "Không xác định");
                }
            };
            xhr.send(formData);
        }
    </script>
</asp:Content>
