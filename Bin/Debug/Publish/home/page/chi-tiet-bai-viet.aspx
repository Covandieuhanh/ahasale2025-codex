<%@ Page Title="" Language="C#" MasterPageFile="~/home/MasterPageHome.master" AutoEventWireup="true" Inherits="home_page_chi_tiet_bai_viet, App_Web_pq4ccoj1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta property="og:type" content="article" />
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
    <style>
        .bd-style-bc1 {
            border-right: 1px solid #eeeeee
        }

        .bd-style-bc2 {
            border-top: 0px solid #eeeeee
        }

        .modal {
            position: fixed;
            z-index: 9999;
            padding-top: 60px;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0,0,0,0.8);
        }

        .modal-content {
            margin: auto;
            display: block;
            max-width: 80%;
            max-height: 80%;
            border-radius: 4px;
        }

        .close {
            position: absolute;
            top: 20px;
            right: 35px;
            color: #fff;
            font-size: 40px;
            font-weight: bold;
            cursor: pointer;
        }

        .review-img {
            cursor: pointer;
        }

        .rounded-circle {
            object-fit: cover;
            object-position: 50% 50%;
            border-radius: 50%;
        }

        .pagination a, .pagination .current-page {
            padding: 5px 10px;
            margin: 2px;
            border: 1px solid #ccc;
            cursor: pointer;
            text-decoration: none;
            color: white;
        }

        .pagination .current-page {
            font-weight: bold;
            background-color: white;
            color: black;
            cursor: default;
        }

        .rounded-10 {
            border-radius: 10px;
        }

        @media (max-width: 991px) {
            .bd-style-bc1 {
                border-right: 0px solid #eeeeee
            }

            .bd-style-bc2 {
                border-top: 1px solid #eeeeee;
                padding-top: 40px
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:HiddenField ID="hf_anhphu" runat="server" ClientIDMode="Static" />
    <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_dathang" runat="server" Visible="false" DefaultButton="but_dathang">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_form_dathang_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                Xác nhận trao đổi
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
                                        <label class="fw-600">
                                            <asp:Literal ID="Literal9" runat="server"></asp:Literal></label>
                                    </div>
                                    <div class="mt-1">
                                        <img src="/uploads/images/dong-a.png" style="width: 14px!important" /><small class="pl-1">
                                            <asp:Literal ID="Literal10" runat="server"></asp:Literal>
                                        </small>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Số lượng</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_soluong2" AutoPostBack="true" OnTextChanged="txt_soluong2_TextChanged" runat="server" data-role="input" data-buttons-position="right" onfocus="AutoSelect(this)" MaxLength="3" oninput="format_sotien_new(this)"></asp:TextBox><%--data-min-value="1" data-max-value="999"--%>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <small>Tổng phải Trao đổi: </small>
                                        <img src="/uploads/images/dong-a.png" style="width: 14px!important" /><small class="pl-1">
                                            <asp:Literal ID="Literal11" runat="server"></asp:Literal>
                                        </small>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Người nhận</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Điện thoại</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Địa chỉ</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_dathang" OnClick="but_dathang_Click" runat="server" Text="Xác nhận trao đổi" CssClass="button dark" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_dathang">
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
            <div class="container pt-10-lg pt-3 pb-10-lg pb-3" style="max-width: 992px !important">
                <div class="row">
                    <div class="cell-lg-12">
                        <div>
                            <div class="row">
                                <div class="cell-lg-8">
                                    <div class="text-center px-2" style="max-width: 100%; margin: auto;">
                                        <div style="border-radius: 10px; width: 100%; height: 300px; background-color: #f0f0f0; display: flex; justify-content: center; align-items: center; overflow: hidden;">
                                            <asp:Image ID="imgMain" runat="server" Style="max-width: 100%; max-height: 100%; object-fit: contain;" />
                                        </div>
                                        <div id="listAnhPhu" runat="server"
                                            class="mt-3"
                                            style="display: flex; overflow-x: auto; white-space: nowrap; gap: 10px; padding-bottom: 5px;">
                                        </div>
                                    </div>
                                </div>

                                <div style="border-radius: 10px" class="p-4 cell-lg-4 mb-4 bg-ahasale1">
                                    <div class="text-bold mt-1 fs-bc-18-16 fg-ahasale">
                                        <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="fg-lightGray">
                                        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div style="width: 100%; background: #181a20; padding: 15px 10px;" class="text-bold rounded-10">
                                        <small style="font-size: 22px; color: #e5193b; line-height: 24px;" class="pl-1">
                                            <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                                            đ</small>
                                    </div>

                                    <div class="fg-lightGray">
                                        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="fg-lightGray">
                                        <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                                    </div>

                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                                        <div class="mt-3">
                                            <div class="row">
                                                <div class="w-100">
                                                    <asp:TextBox ID="txt_soluong1" Text="1" runat="server" data-min-value="1" data-max-value="999" data-role="spinner" data-buttons-position="right" onfocus="AutoSelect(this)" MaxLength="3" oninput="format_sotien_new(this)"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-2">
                                            <div class="cell-6 pr-1">
                                                <asp:Button ID="but_bansanphamnay" OnClick="but_bansanphamnay_Click" runat="server"
                                                    Text="Bán chéo" CssClass="yellow rounded w-100" />
                                            </div>
                                            <div class="cell-6 pl-1">
                                                <asp:Button ID="but_traodoi" OnClick="but_traodoi_Click" runat="server"
                                                    Text="Trao đổi" CssClass="bg-yellow fg-black bg-amber-hover rounded w-100" />
                                            </div>

                                            <div class="cell-12 mt-2">
                                                <asp:Button ID="but_themvaogio" OnClick="but_themvaogio_Click" runat="server"
                                                    Text="Thêm vào giỏ" CssClass="light rounded w-100" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                                <div class="cell-lg-8">
                                </div>
                                <div style="border-radius: 10px" class="p-4 cell-lg-4 bg-ahasale1">
                                    <div class="fg-ahasale mb-3 fw-600">Thông tin người bán</div>
                                    <div class="d-flex align-items-center mb-2">
                                        <img src="<%= ViewState["avt_query"] %>" width="50" height="50"
                                            class="img-cover-vuongtron border bg-white border-size-2 mr-2" />
                                        <div>
                                            <div class="text-bold fg-white">
                                                <%= ViewState["hoten_query"] %>
                                            </div>
                                            <div class="d-flex mt-1 fg-white small">
                                                <span class="mif-star fg-yellow mr-1"></span>
                                                <span>5</span>
                                                <span class="ml-1 fg-white" style="cursor: pointer;">(2)</span>
                                                <span class="ml-2">· 39 đã bán</span>
                                                <span class="ml-2">· 6 đang bán</span>
                                            </div>
                                            <div class="mt-1 small fg-white">
                                                Hoạt động 3 giờ trước 
                                            </div>
                                            <div class="mt-1 small fg-white">
                                                Phản hồi: 98%
                                            </div>
                                        </div>
                                    </div>
                                    <button class="button alert w-100 mb-2 rounded text-bold">
                                        <span class="mif-phone px-2"></span>Gọi điện
                                    </button>

                                    <button class="button info w-100 text-bold rounded">
                                        <span class="mif-bubbles px-2"></span>Chat
                                    </button>
                                </div>
                            </div>
                            <div style="border-radius: 10px" class="text-just mt-4 p-4 chitiet-baiviet-bc pb-10 fg-ahasale bg-ahasale1">
                                <div class="text-bold fg-white mb-2">Mô tả chi tiết</div>
                                <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="Review" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="true">
                <div class="review-form container bg-ahasale1 pt-5-lg pt-3 pb-5-lg pb-3 fg-ahasale" style="max-width: 992px !important">
                    <h3 class="text-center mt-4 mb-4 text-primary">Đánh giá sản phẩm</h3>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="InputReview" runat="server" Visible="false">
                <div class="review-form container bg-ahasale1 pt-5-lg pt-3 pb-5-lg pb-3 fg-ahasale" style="max-width: 992px !important">
                    <asp:Label ID="lblNhapDanhGia" runat="server" Text="Nội dung đánh giá:" />
                    <br />
                    <asp:HiddenField ID="hdDiem" runat="server" />
                    <div id="starRating" style="font-size: 24px; color: gray; cursor: pointer;">
                        <span data-value="1">&#9733;</span>
                        <span data-value="2">&#9733;</span>
                        <span data-value="3">&#9733;</span>
                        <span data-value="4">&#9733;</span>
                        <span data-value="5">&#9733;</span>
                    </div>
                    <br />
                    <asp:TextBox ID="txtNoiDung" runat="server" TextMode="MultiLine" CssClass="form-control" Width="100%" Rows="4" />
                    <br />
                    <div>
                        <input class="input-small" type="file" id="fileInput3" onchange="uploadFile()" />
                        <div id="message"></div>
                        <div id="uploadedFilePath3"></div>
                        <asp:TextBox ID="TxtIcon" runat="server" CssClass="input-small" Style="display: none;"></asp:TextBox>
                        <button type="button" id="btnRemoveImage" style="display: none;" onclick="removeUploadedImage()">Xoá ảnh</button>
                    </div>
                    <br />
                    <asp:Button ID="btnGuiDanhGia" runat="server" Text="Gửi đánh giá" CssClass="btn light btn-primary" OnClick="btnGuiDanhGia_Click" />
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ListReview" runat="server" Visible="true">
                <asp:Repeater ID="rptDanhGia" runat="server">
                    <ItemTemplate>
                        <div class="review-item bg-ahasale1 container pb-5-lg pb-3 fg-ahasale" style="max-width: 992px !important; display: flex; align-items: center; gap: 15px;">
                            <asp:Image ID="imgAvatar" runat="server" ImageUrl='<%# Eval("AnhDaiDien") %>' Width="50" Height="50" CssClass="rounded-circle" AlternateText="Ảnh đại diện" />

                            <div>
                                <a style="color: white;" href="/<%#Eval("TaiKhoanDanhGia")%>.info"><b>Người dùng: <%# Eval("TaiKhoanDanhGia") %></b></a>
                                <br />
                                <span style="color: #ffc107; font-size: 18px;">
                                    <%# new string('★', Convert.ToInt32(Eval("Diem"))) %>
                                </span>
                                <br />
                                <span style="font-style: italic; font-size: 12px; color: gray;">
                                    <%# Eval("NgayDang", "{0:dd/MM/yyyy HH:mm}") %>
                                </span>
                                <br />
                                <span><%# Eval("NoiDung") %></span>
                                <br />
                                <asp:Image ID="imgReview" runat="server" ImageUrl='<%# Eval("UrlAnh") %>' Width="100" CssClass="review-img" Visible='<%# !string.IsNullOrEmpty(Eval("UrlAnh") as string) %>' />
                            </div>

                            <div id="imgModal" class="modal" style="display: none;">
                                <span class="close">&times;</span>
                                <img class="modal-content" id="modalImage" />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:PlaceHolder>
            <div class="review-item bg-ahasale1 container pb-5-lg pb-3 fg-ahasale" style="max-width: 992px !important; display: flex; align-items: center; gap: 15px;">
                <asp:Panel ID="pnlPaging" runat="server" CssClass="pagination"></asp:Panel>
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
        document.addEventListener("DOMContentLoaded", function () {
            const images = document.querySelectorAll(".chitiet-baiviet-bc img");

            images.forEach(img => {
                img.removeAttribute("width");
                img.removeAttribute("height");
                img.style.maxWidth = "100%";
                img.style.height = "auto";
            });
        });

        const stars = document.querySelectorAll("#starRating span");
        const hiddenInput = document.getElementById("<%= hdDiem.ClientID %>");

        stars.forEach(star => {
            star.addEventListener("click", function () {
                const rating = this.getAttribute("data-value");
                hiddenInput.value = rating;
                updateStarDisplay(rating);
                toggleButton();
            });

            star.addEventListener("mouseover", function () {
                const hoverValue = this.getAttribute("data-value");
                updateStarDisplay(hoverValue);
            });

            star.addEventListener("mouseout", function () {
                updateStarDisplay(hiddenInput.value);
            });
        });

        function updateStarDisplay(value) {
            stars.forEach(star => {
                if (parseInt(star.getAttribute("data-value")) <= value) {
                    star.style.color = "#ffc107";
                } else {
                    star.style.color = "gray";
                }
            });
        }

        window.onload = function () {
            updateStarDisplay(hiddenInput.value || "1");
            btnGuiDanhGia.disabled = false;
        };

        function uploadFile() {
            var fileInput = document.getElementById("fileInput3");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath3");
            var btnRemove = document.getElementById("btnRemoveImage");

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
                    messageDiv.innerHTML = "Vui lòng chọn file nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        var url = xhr.responseText;
                        console.log("Uploaded: " + url);

                        uploadedFilePathDiv.innerHTML =
                            "<div><small>Ảnh mới chọn:</small></div><img id='previewImage' width='100' src='" + url + "' />";
                        document.getElementById('<%= TxtIcon.ClientID %>').value = url;
                        btnRemove.style.display = "inline-block";
                        messageDiv.innerHTML = "";
                    } else {
                        messageDiv.innerHTML = "Lỗi upload: " + xhr.responseText;
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }

        function removeUploadedImage() {
            document.getElementById("uploadedFilePath3").innerHTML = "";
            document.getElementById('<%= TxtIcon.ClientID %>').value = "";
            document.getElementById("btnRemoveImage").style.display = "none";
            document.getElementById("fileInput3").value = "";
        }

        document.addEventListener("DOMContentLoaded", function () {
            var images = document.querySelectorAll('.review-img');
            var modal = document.getElementById("imgModal");
            var modalImg = document.getElementById("modalImage");
            var closeBtn = document.querySelector(".close");

            images.forEach(function (img) {
                img.onclick = function () {
                    modal.style.display = "block";
                    modalImg.src = this.src;
                };
            });

            closeBtn.onclick = function () {
                modal.style.display = "none";
            };

            modal.onclick = function (e) {
                if (e.target === modal) {
                    modal.style.display = "none";
                }
            };
        });
        function changeMainImage(src) {
            document.getElementById('<%= imgMain.ClientID %>').src = src;
        }
    </script>
</asp:Content>

