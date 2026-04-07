<%@ Page Title="Cài đặt trang chủ" ValidateRequest="false" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_cai_dat_trang_chu_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="<%--border-top bd-lightGray--%> pt-3 pl-3 pr-3 pb-20 admin-settings-grid">
                <div class="row">
                    <div class="cell-lg-6 pr-4-lg mt-8">
                        <div data-role="panel"
                            data-title-caption="Liên kết chia sẻ"
                            data-title-icon="<span class='mif-link'></span>"
                            data-collapsed="false" <%--mặc định đóng lại--%>
                            data-collapsible="true">
                            <div class="pl-3 pr-3">
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                                    <div class="mt-3">
                                        <label class="fw-600">Tiêu đề</label>
                                        <asp:TextBox ID="txt_title" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Mô tả ngắn</label>
                                        <asp:TextBox ID="txt_description" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh thu nhỏ</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Đây là ảnh sẽ hiển thị khi bạn chia sẻ link Website của bạn qua mạng xã hội.<br />Kích thước chuẩn: 1200x628 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <input type="file" id="fileInput1" onchange="uploadFile1()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message1" runat="server"></div>
                                        <div id="uploadedFilePath1"></div>
                                        <button type="button" id="cancelButton1" class="button alert mini rounded mb-2 mt-1" onclick="cancelUpload1()" style="display: none; width: 100px">Hủy ảnh mới chọn</button>
                                        <div style="display: none">
                                            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button ID="Button11" runat="server" Text="Xóa ảnh cũ" CssClass="alert mini rounded" Visible="false" OnClick="Button11_Click" Width="100" />
                                        </div>
                                    </div>
                                    <div class="mt-6 mb-3 text-right">
                                        <asp:Button ID="Button1" OnClick="Button1_Click" runat="server" Text="CẬP NHẬT" CssClass="success rounded small" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div class="cell-lg-6 pl-4-lg mt-8">
                        <div data-role="panel"
                            data-title-caption="Hẹn giờ bảo trì"
                            data-title-icon="<span class='mif-tools'></span>"
                            data-collapsed="false" <%--mặc định đóng lại--%>
                            data-collapsible="true">
                            <div class="pl-3 pr-3">
                                <asp:Panel ID="Panel2" runat="server" DefaultButton="Button2">
                                    <div class="mt-3">
                                        <label class="fw-600">Trạng thái</label>
                                        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                                            <asp:ListItem Text="Không bảo trì" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Bảo trì" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                                        <div class="mt-3">
                                            <label class="fw-600">Ngày bắt đầu</label>
                                            <asp:TextBox ID="txt_ngay_batdau" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Thời gian bắt đầu</label>
                                            <div class="d-flex">
                                                <asp:DropDownList ID="ddl_giobatdau" runat="server" data-role="select" data-filter="flase" CssClass="mr-2"></asp:DropDownList>
                                                <asp:DropDownList ID="ddl_phutbatdau" runat="server" data-role="select" data-filter="flase" CssClass="ml-2"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Ngày kết thúc</label>
                                            <asp:TextBox ID="txt_ngay_ketthuc" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                        <div class="mt-3">
                                            <label class="fw-600">Thời gian kết thúc</label>
                                            <div class="d-flex">
                                                <asp:DropDownList ID="ddl_gioketthuc" runat="server" data-role="select" data-filter="flase" CssClass="mr-2"></asp:DropDownList>
                                                <asp:DropDownList ID="ddl_phutketthuc" runat="server" data-role="select" data-filter="flase" CssClass="ml-2"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="mt-6 mb-3 text-right">
                                        <asp:Button ID="Button2" runat="server" Text="CẬP NHẬT" CssClass="success rounded small" OnClick="Button2_Click" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div class="cell-lg-6 pr-4-lg mt-8">
                        <div data-role="panel"
                            data-title-caption="Nhúng mã"
                            data-title-icon="<span class='mif-embed2'></span>"
                            data-collapsed="false" <%--mặc định đóng lại--%>
                            data-collapsible="true">
                            <div class="pl-3 pr-3">
                                <asp:Panel ID="Panel3" runat="server" DefaultButton="Button3">
                                    <div class="mt-3">
                                        <label class="fw-600">Nhúng mã vào ngay sau thẻ &lt;head&gt; </label>
                                        <asp:TextBox ID="txt_code_head1" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhúng mã vào ngay trước thẻ &lt;/head&gt; </label>
                                        <asp:TextBox ID="txt_code_head2" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhúng mã vào ngay sau thẻ &lt;body&gt; </label>
                                        <asp:TextBox ID="txt_code_body1" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhúng mã vào ngay trước thẻ &lt;/body&gt; </label>
                                        <asp:TextBox ID="txt_code_body2" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhúng mã Plugin Fanpage Facebook</label>
                                        <asp:TextBox ID="txt_code_page" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhúng mã Google Maps</label>
                                        <asp:TextBox ID="txt_code_maps" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                                    </div>
                                    <div class="mt-6 mb-3 text-right">
                                        <asp:Button ID="Button3" OnClick="Button3_Click" runat="server" Text="CẬP NHẬT" CssClass="success rounded small" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div class="cell-lg-6 pl-4-lg mt-8">
                        <div data-role="panel"
                            data-title-caption="Liên kết ngoài"
                            data-title-icon="<span class='mif-earth'></span>"
                            data-collapsed="false" <%--mặc định đóng lại--%>
                            data-collapsible="true">
                            <div class="pl-3 pr-3">
                                <asp:Panel ID="Panel4" runat="server" DefaultButton="Button4">
                                    <div class="mt-0">
                                        <label class="fw-600">Facebook</label>
                                        <asp:TextBox ID="TextBox2" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Zalo</label>
                                        <asp:TextBox ID="TextBox3" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Youtube</label>
                                        <asp:TextBox ID="TextBox4" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Instagram</label>
                                        <asp:TextBox ID="TextBox5" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Twitter</label>
                                        <asp:TextBox ID="TextBox6" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">TikTok</label>
                                        <asp:TextBox ID="TextBox7" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">WeChat</label>
                                        <asp:TextBox ID="TextBox8" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">linkedIn</label>
                                        <asp:TextBox ID="TextBox9" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">WhatsApp</label>
                                        <asp:TextBox ID="TextBox10" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-6 mb-3 text-right">
                                        <asp:Button ID="Button4" OnClick="Button4_Click" runat="server" Text="CẬP NHẬT" CssClass="success rounded small" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div class="cell-lg-6 pr-4-lg mt-8">
                        <div data-role="panel"
                            data-title-caption="Thông tin khác"
                            data-title-icon="<span class='mif-info'></span>"
                            data-collapsed="false" <%--mặc định đóng lại--%>
                            data-collapsible="true">
                            <div class="pl-3 pr-3">
                                <asp:Panel ID="Panel5" runat="server" DefaultButton="Button5">
                                    <div class="mt-3">
                                        <label class="fw-600">Favicon</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 48x48 pixel. Favicon là biểu tượng nhỏ hiển thị trên tab trình duyệt và các bookmark.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <input type="file" id="fileInput2" onchange="uploadFile2()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message2" runat="server"></div>
                                        <div id="uploadedFilePath2"></div>
                                        <button type="button" id="cancelButton2" class="button alert mini rounded mb-2 mt-1" onclick="cancelUpload2()" style="display: none; width: 100px">Hủy ảnh mới chọn</button>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_upload_2" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button ID="Button22" runat="server" Text="Xóa ảnh cũ" CssClass="alert mini rounded" Visible="false" OnClick="Button22_Click" Width="100" />
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Icon mobile + logo top bar (Home)</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 180x180 pixel. Ảnh này dùng cho icon mobile (khi thêm vào màn hình chính) và logo ở giữa thanh top bar của Home.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <input type="file" id="fileInput3" onchange="uploadFile3()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message3" runat="server"></div>
                                        <div id="uploadedFilePath3"></div>
                                        <button type="button" id="cancelButton3" class="button alert mini rounded mb-2 mt-1" onclick="cancelUpload3()" style="display: none; width: 100px">Hủy ảnh mới chọn</button>
                                        <div class="mt-2">
                                            <label class="fw-600">Hoặc dán link icon (https://... hoặc /uploads/...)</label>
                                            <asp:TextBox ID="txt_link_upload_3" runat="server" data-role="input" placeholder="Ví dụ: https://cdn.domain.com/icon.png"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button ID="Button33" runat="server" Text="Xóa ảnh cũ" CssClass="alert mini rounded" Visible="false" OnClick="Button33_Click" Width="100" />
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Icon mobile + logo top bar (Gian hàng đối tác)</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Ảnh này dùng cho icon mobile khi thêm link /shop vào màn hình chính và logo ở giữa thanh top bar trong không gian Shop.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <input type="file" id="fileInput4" onchange="uploadFile4()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message4" runat="server"></div>
                                        <div id="uploadedFilePath4"></div>
                                        <button type="button" id="cancelButton4" class="button alert mini rounded mb-2 mt-1" onclick="cancelUpload4()" style="display: none; width: 100px">Hủy ảnh mới chọn</button>
                                        <div class="mt-2">
                                            <label class="fw-600">Hoặc dán link icon (https://... hoặc /uploads/...)</label>
                                            <asp:TextBox ID="txt_link_upload_shop" runat="server" data-role="input" placeholder="Ví dụ: https://cdn.domain.com/icon-shop.png"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button ID="Button44" runat="server" Text="Xóa ảnh cũ" CssClass="alert mini rounded" Visible="false" OnClick="Button44_Click" Width="100" />
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Icon mobile (Admin)</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Icon hiển thị khi người dùng thêm link /admin vào màn hình chính.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <input type="file" id="fileInput5" onchange="uploadFile5()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message5" runat="server"></div>
                                        <div id="uploadedFilePath5"></div>
                                        <button type="button" id="cancelButton5" class="button alert mini rounded mb-2 mt-1" onclick="cancelUpload5()" style="display: none; width: 100px">Hủy ảnh mới chọn</button>
                                        <div class="mt-2">
                                            <label class="fw-600">Hoặc dán link icon (https://... hoặc /uploads/...)</label>
                                            <asp:TextBox ID="txt_link_upload_admin" runat="server" data-role="input" placeholder="Ví dụ: https://cdn.domain.com/icon-admin.png"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button ID="Button55" runat="server" Text="Xóa ảnh cũ" CssClass="alert mini rounded" Visible="false" OnClick="Button55_Click" Width="100" />
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Icon mobile + logo top bar (Bất động sản)</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Ảnh này dùng cho icon mobile khi thêm link /bat-dong-san vào màn hình chính và logo nằm giữa thanh top bar của không gian Bất động sản.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <input type="file" id="fileInput6" onchange="uploadFile6()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message6" runat="server"></div>
                                        <div id="uploadedFilePath6"></div>
                                        <button type="button" id="cancelButton6" class="button alert mini rounded mb-2 mt-1" onclick="cancelUpload6()" style="display: none; width: 100px">Hủy ảnh mới chọn</button>
                                        <div class="mt-2">
                                            <label class="fw-600">Hoặc dán link icon (https://... hoặc /uploads/...)</label>
                                            <asp:TextBox ID="txt_link_upload_batdongsan" runat="server" data-role="input" placeholder="Ví dụ: https://cdn.domain.com/icon-batdongsan.png"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button ID="Button66" runat="server" Text="Xóa ảnh cũ" CssClass="alert mini rounded" Visible="false" OnClick="Button66_Click" Width="100" />
                                        </div>
                                    </div>
                                    <div class="mt-6 mb-3 text-right">
                                        <asp:Button ID="Button5" OnClick="Button5_Click" runat="server" Text="CẬP NHẬT" CssClass="success rounded small" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="Button5" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        function uploadFile1() {
            var fileInput1 = document.getElementById("fileInput1");
            var message1Div = document.getElementById("message1");
            var uploadedFilePathDiv1 = document.getElementById("uploadedFilePath1");
            var cancelButton1 = document.getElementById("cancelButton1");

            if (fileInput1.files.length > 0) {
                var file = fileInput1.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    message1Div.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    message1Div.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        // Hiển thị ảnh và đường dẫn ảnh
                        uploadedFilePathDiv1.innerHTML = "<div><img style='max-width:100px;max-height:100px' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= TextBox1.ClientID %>').value = xhr.responseText;

                        // Hiển thị nút Hủy
                        cancelButton1.style.display = "inline-block";
                    } else {
                        message1Div.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                message1Div.innerHTML = "Vui lòng chọn file.";
            }
        }

        function cancelUpload1() {
            var uploadedFilePathDiv1 = document.getElementById("uploadedFilePath1");
            var textBox1 = document.getElementById('<%= TextBox1.ClientID %>');
            var cancelButton1 = document.getElementById("cancelButton1");

            // Xóa nội dung trong TextBox1
            textBox1.value = "";

            // Xóa nội dung hiển thị ảnh
            uploadedFilePathDiv1.innerHTML = "";

            // Ẩn nút Hủy
            cancelButton1.style.display = "none";

            // Hiển thị thông báo nếu cần
            var message1Div = document.getElementById("message1");
            message1Div.innerHTML = "Đã hủy upload ảnh.";
        }
    </script>
    <script>
        function uploadFile2() {
            var fileInput2 = document.getElementById("fileInput2");
            var message2Div = document.getElementById("message2");
            var uploadedFilePathDiv2 = document.getElementById("uploadedFilePath2");
            var cancelButton2 = document.getElementById("cancelButton2");

            if (fileInput2.files.length > 0) {
                var file = fileInput2.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    message2Div.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    message2Div.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        // Hiển thị ảnh và đường dẫn ảnh
                        uploadedFilePathDiv2.innerHTML = "<div><img style='max-width:100px;max-height:100px' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_upload_2.ClientID %>').value = xhr.responseText;

                        // Hiển thị nút Hủy
                        cancelButton2.style.display = "inline-block";
                    } else {
                        message2Div.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                message2Div.innerHTML = "Vui lòng chọn file.";
            }
        }

        function cancelUpload2() {
            var uploadedFilePathDiv2 = document.getElementById("uploadedFilePath2");
            var textBox2 = document.getElementById('<%= txt_link_upload_2.ClientID %>');
            var cancelButton2 = document.getElementById("cancelButton2");

            // Xóa nội dung trong TextBox1
            textBox2.value = "";

            // Xóa nội dung hiển thị ảnh
            uploadedFilePathDiv2.innerHTML = "";

            // Ẩn nút Hủy
            cancelButton2.style.display = "none";

            // Hiển thị thông báo nếu cần
            var message2Div = document.getElementById("message2");
            message2Div.innerHTML = "Đã hủy upload ảnh.";
        }
</script>
    <script>
        function uploadFile3() {
            var fileInput3 = document.getElementById("fileInput3");
            var message3Div = document.getElementById("message3");
            var uploadedFilePathDiv3 = document.getElementById("uploadedFilePath3");
            var cancelButton3 = document.getElementById("cancelButton3");

            if (fileInput3.files.length > 0) {
                var file = fileInput3.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    message3Div.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    message3Div.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        // Hiển thị ảnh và đường dẫn ảnh
                        uploadedFilePathDiv3.innerHTML = "<div><img style='max-width:100px;max-height:100px' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_upload_3.ClientID %>').value = xhr.responseText;

                        // Hiển thị nút Hủy
                        cancelButton3.style.display = "inline-block";
                    } else {
                        message3Div.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                message3Div.innerHTML = "Vui lòng chọn file.";
            }
        }

        function cancelUpload3() {
            var uploadedFilePathDiv3 = document.getElementById("uploadedFilePath3");
            var textBox3 = document.getElementById('<%= txt_link_upload_3.ClientID %>');
            var cancelButton3 = document.getElementById("cancelButton3");

            // Xóa nội dung trong TextBox1
            textBox3.value = "";

            // Xóa nội dung hiển thị ảnh
            uploadedFilePathDiv3.innerHTML = "";

            // Ẩn nút Hủy
            cancelButton3.style.display = "none";

            // Hiển thị thông báo nếu cần
            var message3Div = document.getElementById("message3");
            message3Div.innerHTML = "Đã hủy upload ảnh.";
        }
</script>
    <script>
        function uploadFile4() {
            var fileInput4 = document.getElementById("fileInput4");
            var message4Div = document.getElementById("message4");
            var uploadedFilePathDiv4 = document.getElementById("uploadedFilePath4");
            var cancelButton4 = document.getElementById("cancelButton4");

            if (fileInput4.files.length > 0) {
                var file = fileInput4.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    message4Div.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    message4Div.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        // Hiển thị ảnh và đường dẫn ảnh
                        uploadedFilePathDiv4.innerHTML = "<div><img style='max-width:100px;max-height:100px' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_upload_shop.ClientID %>').value = xhr.responseText;

                        // Hiển thị nút Hủy
                        cancelButton4.style.display = "inline-block";
                    } else {
                        message4Div.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                message4Div.innerHTML = "Vui lòng chọn file.";
            }
        }

        function cancelUpload4() {
            var uploadedFilePathDiv4 = document.getElementById("uploadedFilePath4");
            var textBox4 = document.getElementById('<%= txt_link_upload_shop.ClientID %>');
            var cancelButton4 = document.getElementById("cancelButton4");

            // Xóa nội dung trong TextBox
            textBox4.value = "";

            // Xóa nội dung hiển thị ảnh
            uploadedFilePathDiv4.innerHTML = "";

            // Ẩn nút Hủy
            cancelButton4.style.display = "none";

            // Hiển thị thông báo nếu cần
            var message4Div = document.getElementById("message4");
            message4Div.innerHTML = "Đã hủy upload ảnh.";
        }
</script>
    <script>
        function uploadFile5() {
            var fileInput5 = document.getElementById("fileInput5");
            var message5Div = document.getElementById("message5");
            var uploadedFilePathDiv5 = document.getElementById("uploadedFilePath5");
            var cancelButton5 = document.getElementById("cancelButton5");

            if (fileInput5.files.length > 0) {
                var file = fileInput5.files[0];

                // Kiểm tra loại tệp
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    message5Div.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                // Kiểm tra kích thước tệp
                var maxFileSize = 10 * 1024 * 1024; // MB
                if (file.size > maxFileSize) {
                    message5Div.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        // Hiển thị ảnh và đường dẫn ảnh
                        uploadedFilePathDiv5.innerHTML = "<div><img style='max-width:100px;max-height:100px' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_upload_admin.ClientID %>').value = xhr.responseText;

                        // Hiển thị nút Hủy
                        cancelButton5.style.display = "inline-block";
                    } else {
                        message5Div.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                message5Div.innerHTML = "Vui lòng chọn file.";
            }
        }

        function cancelUpload5() {
            var uploadedFilePathDiv5 = document.getElementById("uploadedFilePath5");
            var textBox5 = document.getElementById('<%= txt_link_upload_admin.ClientID %>');
            var cancelButton5 = document.getElementById("cancelButton5");

            // Xóa nội dung trong TextBox
            textBox5.value = "";

            // Xóa nội dung hiển thị ảnh
            uploadedFilePathDiv5.innerHTML = "";

            // Ẩn nút Hủy
            cancelButton5.style.display = "none";

            // Hiển thị thông báo nếu cần
            var message5Div = document.getElementById("message5");
            message5Div.innerHTML = "Đã hủy upload ảnh.";
        }
</script>
    <script>
        function uploadFile6() {
            var fileInput6 = document.getElementById("fileInput6");
            var message6Div = document.getElementById("message6");
            var uploadedFilePathDiv6 = document.getElementById("uploadedFilePath6");
            var cancelButton6 = document.getElementById("cancelButton6");

            if (fileInput6.files.length > 0) {
                var file = fileInput6.files[0];

                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    message6Div.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) {
                    message6Div.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv6.innerHTML = "<div><img style='max-width:100px;max-height:100px' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_upload_batdongsan.ClientID %>').value = xhr.responseText;
                        cancelButton6.style.display = "inline-block";
                    } else {
                        message6Div.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                message6Div.innerHTML = "Vui lòng chọn file.";
            }
        }

        function cancelUpload6() {
            var uploadedFilePathDiv6 = document.getElementById("uploadedFilePath6");
            var textBox6 = document.getElementById('<%= txt_link_upload_batdongsan.ClientID %>');
            var cancelButton6 = document.getElementById("cancelButton6");

            textBox6.value = "";
            uploadedFilePathDiv6.innerHTML = "";
            cancelButton6.style.display = "none";

            var message6Div = document.getElementById("message6");
            message6Div.innerHTML = "Đã hủy upload ảnh.";
        }
</script>
</asp:Content>
