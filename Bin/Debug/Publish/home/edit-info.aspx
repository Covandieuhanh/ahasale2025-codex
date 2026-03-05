<%@ page title="Chỉnh sửa thông tin" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="home_edit_info, App_Web_i5twubwd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:TextBox ID="txtKieu" runat="server" style="visibility:hidden; height:0; width:0;" />

    <asp:UpdatePanel ID="up_themlink" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_themlink" runat="server" Visible="false" DefaultButton="btnLuuLink">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style="top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;">
                        <div style="position: absolute; right: 18px; top: 14px; z-index: 1040!important">
                            <a href="#" class="fg-white d-inline" id="btnCloseLink" runat="server" onserverclick="btnDong_Click" title="Đóng">
                                <span class="mif mif-cross mif-2x fg-red fg-lightRed-hover"></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                Thêm/sửa link mạng xã hội
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style="top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;">
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row">
                                <div class="cell-lg-12">
                                    <asp:HiddenField ID="hfIdLink" runat="server" />
                                    <div class="mt-3">
                                        <label class="fw-600"><small>Tên</small></label>
                                        <div>
                                            <asp:TextBox ID="txtTen" runat="server" CssClass="input-small" placeholder="Tên" data-role="input"/>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red"><small>Link</small></label>
                                        <div>
                                            <asp:TextBox ID="txtLink" runat="server" CssClass="input-small" placeholder="Ví dụ: https://www.facebook.com" data-role="input"/>
                                            <asp:Label ID="lblLinkError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600"><small>Icon</small></label>
                                        <div>
                                            <input class="input-small" type="file" id="fileInput3" onchange="uploadFile3()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                            <div id="message3"></div>
                                            <div id="uploadedFilePath3"></div>
                                            <div style="display: none">
                                                <asp:TextBox ID="TxtIcon" runat="server" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div>
                                                <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                                            </div>

                                            <div id="uploadedFileContainer" runat="server" visible="false">
                                                <div>
                                                    <small>Ảnh hiện tại:</small><br />
                                                    <asp:Image ID="previewImage" runat="server" Width="100" />
                                                </div>
                                                <asp:Button ID="btnRemoveImage" runat="server" Text="Xoá ảnh" CssClass="button alert small mt-2" OnClick="removeUploadedImage" />
                                            </div>

                                        </div>
                                    </div>
                                    <div class="mt-6 mb-20 text-right">
                                        <asp:Button ID="btnLuuLink" runat="server" Text="Lưu" CssClass="button dark" OnClick="btnLuuLink_Click" />
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" DefaultButton="but_capnhat">
                <div class="container pt-10-lg pt-3 pb-10-lg pb-3">
                    <div class="row">
                        <div class="cell-lg-4 pr-4-lg fg-ahasale">
                            <div class=" bg-ahasale1 p-4">
                                <%--<div class="fg-ahasale text-center mb-3 fw-600">Hồ sơ</div>--%>
                                <div class="text-center">
                                    <img src="<%= ViewState["avt_query"] %>" width="100" height="100" class="img-cover-vuongtron border bg-white border-size-2" />
                                </div>
                                <div class="text-bold text-center fg-ahasale"><small><%= ViewState["hoten_query"] %></small></div>
                                <div class="p-4 bg-ahasale fg-ahasale mt-3 text-center">
                                    <small>
                                        <asp:Literal ID="Literal4" runat="server"></asp:Literal></small>
                                </div>
                                <div class="text-center mt-3"><a class="fg-ahasale fg-yellow-hover" href="tel:<%= ViewState["sdt_query"] %>"><small><span class="mif-phone pr-1"></span><%= ViewState["sdt_query"] %></small></a></div>
                                <div class="text-center">
                                    <span class="mif-location"></span>
                                    <small>
                                        <asp:Literal ID="Literal5" runat="server"></asp:Literal></small>
                                </div>
                                <div class="bg-ahasale1 d-flex flex-justify-between flex-equal-items p-2">
                                    <%=ViewState["phanloai_query"] %>
                                    <div class="button flat-button bg-gray">
                                        <img src="/uploads/images/dong-a.png" />
                                        <span class="text-bold"><%=ViewState["DongA_query"] %></span>
                                    </div>
                                </div>
                                <div class="d-flex flex-justify-between flex-equal-items p-2 " style="background-color: #1e2329">
                                    <asp:Button ID="but_show_form_naptien" runat="server" Text="Nạp Quyền tiêu dùng" CssClass="button mr-1  bg-yellow fg-black bg-amber-hover  flat-button rounded" />
                                    <asp:Button ID="Button4" runat="server" Text="Lịch sử Trao đổi" CssClass="button ml-1  dark  flat-button rounded" />
                                </div>
                            </div>
                        </div>
                        <div class="cell-lg-8 pl-4-lg mt-3 mt-0-lg">
                            <div class=" bg-ahasale1 p-4">
                                <ul data-role="tabs" data-expand="true" data-tabs-type="group">
                                    <li><a href="#_target_1"><span class="mif-user pr-1"></span>Cá nhân</a></li>
                                    <li><a href="#_target_2"><span class="mif-shop pr-1"></span>Cửa hàng</a></li>
                                </ul>
                                <div class="">
                                    <div id="_target_1">
                                        <div class="row">
                                            <div class="cell-lg-12">
                                                <div class="mt-2 fg-ahasale">
                                                    <div class="fw-600 fg-ahasale"><small>Ảnh đại diện</small></div>
                                                    <input class="input-small" type="file" id="fileInput" onchange="uploadFile()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                                    <div id="message" runat="server"></div>
                                                    <div id="uploadedFilePath"></div>
                                                    <div style="display: none">
                                                        <asp:TextBox ID="txt_link_fileupload" runat="server" CssClass="input-small"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div style='position: absolute; bottom: 0px; left: 100px'>
                                                        <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="fw-600 mt-2 fg-red"><small>Họ tên</small></div>
                                                <asp:TextBox ID="txt_hoten" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>

                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="fw-600 mt-2 fg-red"><small>Điện thoại</small></div>
                                                <asp:TextBox ID="txt_sdt" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="fw-600 mt-2 fg-red"><small>Email</small></div>
                                                <asp:TextBox ID="TextBox9" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>

                                            </div>
                                            <div class="cell-lg-6 pl-3-lg">
                                                <%--<div class="fw-600 mt-2 fg-ahasale"><small>Số Zalo</small></div>
                                                <asp:TextBox ID="TextBox8" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>--%>
                                            </div>
                                            <div class="cell-lg-12">
                                                <div class="fw-600 mt-2 fg-red"><small>Địa chỉ</small></div>
                                                <asp:TextBox ID="txt_diachi" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-12">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Giới thiệu</small></div>
                                                <asp:TextBox ID="txt_gioithieu" runat="server" data-role="textarea" TextMode="MultiLine" placeholder="Tối đa 60 ký tự"></asp:TextBox>
                                            </div>

                                            <asp:Repeater ID="rptMangXaHoiCN" runat="server" OnItemCommand="rptMangXaHoi_ItemCommand">
                                                <ItemTemplate>
                                                    <div style="justify-content: space-between; align-items: center;" class="cell-lg-12 d-flex mt-2 mb-3">
                                                        <a href='<%# Eval("Link") %>' target="_blank" style="text-decoration: none; color: inherit;">
                                                            <div class="d-flex align-items-center">
                                                                <asp:Image ID="imgIcon" runat="server"
                                                                    ImageUrl='<%# Eval("Icon") %>'
                                                                    Width="50" Height="50"
                                                                    Style="object-fit: cover; border-radius: 5px; margin-right: 10px;"
                                                                    Visible='<%# !string.IsNullOrEmpty(Eval("Icon") as string) %>' />

                                                                <div style='<%# string.IsNullOrEmpty(Eval("Icon") as string) ? "margin-left:60px;" : "" %>'>
                                                                    <div class="fw-600 fg-ahasale">
                                                                        <small><%# Eval("Ten") %></small>
                                                                    </div>
                                                                    <div style="font-style: italic; color: gray; font-size: 0.9em;">
                                                                        <%# Eval("Link") %>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </a>
                                                        <div class="d-flex align-items-center">
                                                            <asp:Button 
                                                                ID="btnSua" 
                                                                runat="server" 
                                                                Text="Sửa" 
                                                                ToolTip="Sửa"
                                                                CssClass="btn btn-sm fw-bold"
                                                                Style="background: #007bff;
                                                                        color: white;
                                                                        border: 1px solid #007bff;
                                                                        margin-right: 5px;"
                                                                CommandName="EditItem"
                                                                CommandArgument='<%# Eval("ID") %>' />

                                                            <asp:Button 
                                                                ID="btnXoa" 
                                                                runat="server" 
                                                                Text="X" 
                                                                ToolTip="Xóa"
                                                                CssClass="btn btn-sm fw-bold"
                                                                Style="background: red;
                                                                        color: white;
                                                                        border: 1px solid red;"
                                                                CommandName="DeleteItem"
                                                                    CommandArgument='<%# Eval("ID") %>' />
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <%--
                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Liên kết Zalo cá nhân</small></div>
                                                <asp:TextBox ID="TextBox18" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Tên Facebook cá nhân</small></div>
                                                <asp:TextBox ID="TextBox14" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Liên kết Facebook cá nhân</small></div>
                                                <asp:TextBox ID="txt_linkfb" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Tên TikTok cá nhân</small></div>
                                                <asp:TextBox ID="TextBox15" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Liên kết TikTok cá nhân</small></div>
                                                <asp:TextBox ID="TextBox1" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Tên kênh Youtube cá nhân</small></div>
                                                <asp:TextBox ID="TextBox16" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Liên kết Youtube cá nhân</small></div>
                                                <asp:TextBox ID="TextBox7" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>--%>

                                        </div>
                                    </div>
                                    <div id="_target_2">
                                        <div class="row">
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="mt-2">
                                                    <label class="fw-600 fg-red"><small>Logo</small></label>
                                                    <input type="file" id="fileInput1" onchange="uploadFile1()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                                    <div id="message1" runat="server"></div>
                                                    <div id="uploadedFilePath1"></div>
                                                    <div style="display: none">
                                                        <asp:TextBox ID="txt_link_fileupload1" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div style='position: absolute; bottom: 0px; left: 100px'>
                                                        <asp:Button ID="Button1" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button1_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="mt-2">
                                                    <label class="fw-600 fg-red"><small>Ảnh bìa</small></label>
                                                    <input type="file" id="fileInput2" onchange="uploadFile2()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                                    <div id="message2" runat="server"></div>
                                                    <div id="uploadedFilePath2"></div>
                                                    <div style="display: none">
                                                        <asp:TextBox ID="txt_link_fileupload2" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div style='position: absolute; bottom: 0px; left: 100px'>
                                                        <asp:Button ID="Button3" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button3_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="fw-600 mt-2 fg-red"><small>Tên cửa hàng</small></div>
                                                <asp:TextBox ID="TextBox2" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>

                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="fw-600 mt-2 fg-red"><small>Điện thoại</small></div>
                                                <asp:TextBox ID="TextBox3" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>
                                            <div class="cell-lg-6 pr-3-lg">
                                                <div class="fw-600 mt-2 fg-red"><small>Email</small></div>
                                                <asp:TextBox ID="TextBox10" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>

                                            </div>
                                            <div class="cell-lg-6 pl-3-lg">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Số Zalo</small></div>
                                                <asp:TextBox ID="TextBox11" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>

                                            <div class="cell-lg-12">
                                                <div class="fw-600 mt-2 fg-ahasale"><small>Mô tả ngắn</small></div>
                                                <asp:TextBox ID="TextBox5" runat="server" data-role="textarea" TextMode="MultiLine" placeholder="Tối đa 60 ký tự"></asp:TextBox>
                                            </div>


                                            <div class="cell-lg-12">
                                                <div class="fw-600 mt-2 fg-red"><small>Địa chỉ</small></div>
                                                <asp:TextBox ID="TextBox4" runat="server" data-role="input" CssClass="input-small"></asp:TextBox>
                                            </div>

                                            <asp:Repeater ID="rptMangXaHoiCH" runat="server" OnItemCommand="rptMangXaHoi_ItemCommand">
                                                <ItemTemplate>
                                                    <div style="justify-content: space-between; align-items: center;" class="cell-lg-12 d-flex mt-2 mb-3">
                                                        <a href='<%# Eval("Link") %>' target="_blank" style="text-decoration: none; color: inherit;">
                                                            <div class="d-flex align-items-center">
                                                                <asp:Image ID="imgIcon" runat="server"
                                                                    ImageUrl='<%# Eval("Icon") %>'
                                                                    Width="50" Height="50"
                                                                    Style="object-fit: cover; border-radius: 5px; margin-right: 10px;"
                                                                    Visible='<%# !string.IsNullOrEmpty(Eval("Icon") as string) %>' />

                                                                <div style='<%# string.IsNullOrEmpty(Eval("Icon") as string) ? "margin-left:60px;" : "" %>'>
                                                                    <div class="fw-600 fg-ahasale">
                                                                        <small><%# Eval("Ten") %></small>
                                                                    </div>
                                                                    <div style="font-style: italic; color: gray; font-size: 0.9em;">
                                                                        <%# Eval("Link") %>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </a>
                                                      <div class="d-flex align-items-center">
                                                            <asp:Button 
                                                                ID="btnSua" 
                                                                runat="server" 
                                                                Text="Sửa" 
                                                                ToolTip="Sửa"
                                                                CssClass="btn btn-sm fw-bold"
                                                                Style="background: #007bff;
                                                                       color: white;
                                                                       border: 1px solid #007bff;
                                                                       margin-right: 5px;"
                                                                CommandName="EditItem"
                                                                CommandArgument='<%# Eval("ID") %>' />

                                                            <asp:Button 
                                                                ID="btnXoa" 
                                                                runat="server" 
                                                                Text="X" 
                                                                ToolTip="Xóa"
                                                                CssClass="btn btn-sm fw-bold"
                                                                Style="background: red;
                                                                       color: white;
                                                                       border: 1px solid red;"
                                                                CommandName="DeleteItem"
                                                                CommandArgument='<%# Eval("ID") %>' />
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>
                                <div class="text-right mt-4">
                                    <asp:Button ID="but_themlink" runat="server" Text="+ Thêm link" CssClass="rounded small bg-yellow" OnClick="but_themlink_Click" />
                                    <asp:Button ID="but_capnhat" OnClick="but_capnhat_Click" runat="server" Text="Cập nhật" CssClass="rounded small bg-yellow" />
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
        $(document).ready(function () {
            $("ul[data-role='tabs'] a").on("click", function () {
                setTimeout(function () {
                    if ($("#_target_1").is(":visible")) {
                        $("#<%= txtKieu.ClientID %>").val("Cá nhân");
                    } else {
                        $("#<%= txtKieu.ClientID %>").val("Cửa hàng");
                    }
                }, 100);
            });
            if ($("#_target_1").is(":visible")) {
                $("#<%= txtKieu.ClientID %>").val("Cá nhân");
            } else {
                $("#<%= txtKieu.ClientID %>").val("Cửa hàng");
            }
        });

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
        function uploadFile1() {
            var fileInput = document.getElementById("fileInput1");
            var messageDiv = document.getElementById("message1");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath1");

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
                        document.getElementById('<%= txt_link_fileupload1.ClientID %>').value = xhr.responseText;// Hiển thị đường dẫn
                    } else {
                        messageDiv.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }

        function uploadFile2() {
            var fileInput = document.getElementById("fileInput2");
            var messageDiv = document.getElementById("message2");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath2");

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
                        document.getElementById('<%= txt_link_fileupload2.ClientID %>').value = xhr.responseText;// Hiển thị đường dẫn
                    } else {
                        messageDiv.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }
        function uploadFile3() {
            var fileInput = document.getElementById("fileInput3");
            var messageDiv = document.getElementById("message3");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath3");

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
                        console.log(xhr.responseText);
                        uploadedFilePathDiv.innerHTML = "<div><small>Ảnh mới chọn<small></div><img width='100' src='" + xhr.responseText + "' />"; // Hiển thị ảnh
                        document.getElementById('<%= TxtIcon.ClientID %>').value = xhr.responseText;// Hiển thị đường dẫn
                    } else {
                        messageDiv.innerHTML = "Lỗi upload." + xhr.responseText;
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }
    </script>
</asp:Content>

