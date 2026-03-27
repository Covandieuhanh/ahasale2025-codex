<%@ Page Title="Đóng góp ý kiến"
    Language="C#"
    MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true"
    CodeFile="dong-gop-y-kien.aspx.cs"
    Inherits="home_dong_gop_y_kien" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="head_sau" runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="page page-center">
                <div class="container container-tight py-4">
                    <div class="card card-md">
                        <div class="card-body">

                            <h2 class="h2 text-center mb-4">Đóng góp ý kiến</h2>

                            <!-- Loại vấn đề -->
                            <div class="mb-3">
                                <label class="form-label fw-bold">
                                    Loại vấn đề bạn muốn góp ý
                                </label>

                                <asp:RadioButtonList
                                    ID="rdLoaiVanDe"
                                    runat="server"
                                    CssClass="form-check"
                                    RepeatDirection="Vertical">
                                    <asp:ListItem Text="Tính năng bị lỗi" Value="bug" />
                                    <asp:ListItem Text="Yêu cầu tính năng mới" Value="feature" />
                                    <asp:ListItem Text="Góp ý về giao diện" Value="ui" />
                                    <asp:ListItem Text="Vấn đề khác" Value="other" />
                                </asp:RadioButtonList>
                            </div>

                            <!-- Nội dung góp ý -->
                            <div class="mb-3">
                                <label class="form-label fw-bold">
                                    Nêu chi tiết ý kiến của bạn
                                </label>
                                <asp:TextBox
                                    ID="txtNoiDung"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="MultiLine"
                                    Rows="5"
                                    placeholder="Nhập nội dung góp ý chi tiết..." />
                            </div>

                            <!-- Upload file -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">
                                Tải lên hình ảnh / video (tối đa 5 file)
                            </label>

                            <input type="file"
                                id="fileInput"
                                multiple
                                onchange="uploadFiles()"
                                class="form-control" />

                            <div id="message" class="text-danger mt-1"></div>
                            <div id="uploadedFilePath" class="mt-2"></div>

                            <!-- Lưu danh sách link file -->
                            <asp:TextBox
                                ID="txt_ListMedia"
                                runat="server"
                                Style="display: none;" />
                        </div>


                            <!-- Số điện thoại -->
                            <div class="mb-3">
                                <label class="form-label fw-bold">
                                    Số điện thoại của bạn
                                </label>
                                <asp:TextBox
                                    ID="txtSoDienThoai"
                                    runat="server"
                                    CssClass="form-control"
                                    placeholder="Nhập số điện thoại liên hệ" />
                            </div>

                            <!-- Submit -->
                            <div class="form-footer">
                                <asp:Button
                                    ID="btnGuiGopY"
                                    runat="server"
                                    Text="Gửi ý kiến"
                                    CssClass="btn btn-primary w-100"
                                    OnClick="btnGuiGopY_Click" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>
            <script type="text/javascript">
                let uploadedFiles = [];

                function uploadFiles() {
                    var fileInput = document.getElementById("fileInput");
                    var messageDiv = document.getElementById("message");
                    var previewDiv = document.getElementById("uploadedFilePath");

                    if (!fileInput.files || fileInput.files.length === 0) {
                        messageDiv.innerHTML = "Vui lòng chọn file.";
                        return;
                    }

                    if (uploadedFiles.length + fileInput.files.length > 5) {
                        messageDiv.innerHTML = "Chỉ được tải tối đa 5 file.";
                        return;
                    }

                    var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic", ".mp4"];
                    var maxFileSize = 10 * 1024 * 1024; // 10MB

                    Array.from(fileInput.files).forEach(file => {
                        var ext = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                        if (allowedExtensions.indexOf(ext) === -1) {
                            messageDiv.innerHTML = "Định dạng file không hợp lệ.";
                            return;
                        }

                        if (file.size > maxFileSize) {
                            messageDiv.innerHTML = "File vượt quá 10MB.";
                            return;
                        }

                        var formData = new FormData();
                        formData.append("file", file);

                        var xhr = new XMLHttpRequest();
                        xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                        xhr.onload = function () {
                            if (xhr.status === 200) {
                                let link = xhr.responseText;
                                uploadedFiles.push(link);

                                previewDiv.innerHTML +=
                                    "<div class='d-inline-block me-2 mb-2'>" +
                                    "<img src='" + link + "' width='80' class='rounded border' />" +
                                    "</div>";

                                document.getElementById('<%= txt_ListMedia.ClientID %>').value =
                                    uploadedFiles.join(",");
                            } else {
                                messageDiv.innerHTML = "Lỗi upload.";
                            }
                        };
                        xhr.send(formData);
                    });

                    fileInput.value = "";
                }
            </script>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="0">
        <ProgressTemplate>
            <div class="page-loading active">
                <div class="page-loading-card">
                    <span class="spinner-border"></span>
                    <div class="text-secondary">Loading...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
