<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="chi-tiet-bai-viet.aspx.cs" Inherits="home_page_chi_tiet_bai_viet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <meta property="og:type" content="article" />
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>

    <style>
        /* ====== Page layout ====== */
        .product-wrap { max-width: 1100px; }

        /* ====== Gallery ====== */
        .gallery-main {
            width: 100%;
            height: 360px;
            background: #f6f8fb;
            border-radius: 12px;
            overflow: hidden;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .gallery-main img,
        .gallery-main video { max-width: 100%; max-height: 100%; object-fit: contain; }

        .thumbs {
            display: flex;
            gap: 10px;
            overflow-x: auto;
            padding-bottom: 6px;
            margin-top: 12px;
        }
        .thumbs img,
        .thumbs video {
            width: 80px;
            height: 80px;
            object-fit: cover;
            cursor: pointer;
            border-radius: 10px;
            border: 1px solid rgba(98,105,118,.18);
            background: #fff;
        }

        /* ====== Overlay form (giữ cơ chế render khi pn_dathang.Visible = true) ====== */
        .order-overlay {
            position: fixed;
            inset: 0;
            z-index: 2000;
            background: rgba(0,0,0,.55);
            overflow: auto;
            padding: 18px 12px;
        }
        .order-dialog {
            max-width: 560px;
            margin: 0 auto;
        }

        /* ===== Reviews image modal (1 modal dùng chung) ===== */
        #imgModal {
            position: fixed;
            z-index: 3000;
            padding-top: 60px;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0,0,0,0.85);
            display: none;
        }
        #imgModal .modal-content {
            margin: auto;
            display: block;
            max-width: 86%;
            max-height: 86%;
            border-radius: 10px;
        }
        #imgModal .close {
            position: absolute;
            top: 14px;
            right: 18px;
            color: #fff;
            font-size: 38px;
            font-weight: bold;
            cursor: pointer;
            user-select: none;
        }
        .review-img { cursor: pointer; border-radius: 10px; border: 1px solid rgba(98,105,118,.18); }

        .rounded-circle {
            object-fit: cover;
            object-position: 50% 50%;
            border-radius: 50%;
        }

        /* Paging giữ cơ chế cũ */
        .pagination a, .pagination .current-page {
            padding: 6px 10px;
            margin: 2px;
            border: 1px solid rgba(98,105,118,.25);
            cursor: pointer;
            text-decoration: none;
            color: inherit;
            border-radius: 8px;
            display: inline-block;
        }
        .pagination .current-page {
            font-weight: 700;
            background-color: #fff;
            color: #1d273b;
            cursor: default;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:HiddenField ID="hf_anhphu" runat="server" ClientIDMode="Static" />

    <!-- ====== FORM TRAO ĐỔI (GIỮ LOGIC: toggle pn_dathang.Visible) ====== -->
    <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_dathang" runat="server" Visible="false" DefaultButton="but_dathang">
                <div class="order-overlay">
                    <div class="order-dialog">
                        <div class="card shadow-sm">
                            <div class="card-header d-flex align-items-center justify-content-between">
                                <div class="card-title fw-bold">Xác nhận trao đổi</div>
                                <a href="#" id="A1" runat="server" onserverclick="but_close_form_dathang_Click" title="Đóng"
                                    class="btn btn-ghost-danger btn-icon">
                                    <i class="ti ti-x"></i>
                                </a>
                            </div>

                            <div class="card-body">
                                <div class="mb-2">
                                    <div class="text-muted small">Sản phẩm</div>
                                    <div class="fw-bold">
                                        <asp:Literal ID="Literal9" runat="server"></asp:Literal>
                                    </div>
                                </div>

                                <div class="row g-2 mb-3">
                                    <div class="col-6">
                                        <div class="text-muted small">Giá</div>
                                        <div class="fw-bold text-danger">
                                            <asp:Literal ID="Literal10" runat="server"></asp:Literal> đ
                                        </div>
                                    </div>
                                    <div class="col-6">
                                        <div class="text-muted small">Tổng (VNĐ)</div>
                                        <div class="fw-bold text-success">
                                            <asp:Literal ID="Literal11" runat="server"></asp:Literal> đ
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-2">
    <div class="text-muted small">Ưu đãi</div>
    <div class="fw-bold text-warning">
        <asp:Literal ID="LiteralUuDaiPercent" runat="server"></asp:Literal>%
    </div>
</div>


                                <div class="mb-2">
                                    <label class="form-label">Số lượng</label>
                                    <asp:TextBox ID="txt_soluong2"
                                        AutoPostBack="true"
                                        OnTextChanged="txt_soluong2_TextChanged"
                                        runat="server"
                                        CssClass="form-control"
                                        MaxLength="3"
                                        onfocus="AutoSelect(this)"
                                        oninput="format_sotien_new(this)"></asp:TextBox>
                                </div>

                                <div class="row g-2">
                                    <div class="col-12">
                                        <label class="form-label">Người nhận</label>
                                        <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Điện thoại</label>
                                        <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Địa chỉ</label>
                                        <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="card-footer d-flex align-items-center justify-content-between">
                                <a href="#" id="A2" runat="server" onserverclick="but_close_form_dathang_Click"
                                   class="btn btn-link text-decoration-none">Hủy</a>

                                <asp:Button ID="but_dathang"
                                    OnClick="but_dathang_Click"
                                    runat="server"
                                    Text="Xác nhận trao đổi"
                                    CssClass="btn btn-success px-4" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_dathang">
        <ProgressTemplate>
            <div class="position-fixed top-0 start-0 w-100 h-100" style="background:rgba(0,0,0,.6); z-index: 99999;">
                <div class="h-100 d-flex align-items-center justify-content-center">
                    <div class="spinner-border" role="status" aria-label="loading"></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ====== MAIN ====== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container-xl py-4 product-wrap">
                <div class="row g-4">

                    <!-- LEFT: GALLERY + DESC -->
                    <div class="col-lg-8">
                        <div class="card shadow-sm">
                            <div class="card-body">
                                <div class="gallery-main">
                                    <asp:Literal ID="litMainMedia" runat="server"></asp:Literal>
                                </div>

                                <div id="listAnhPhu" runat="server" class="thumbs"></div>
                            </div>
                        </div>

                        <div class="card shadow-sm mt-4">
                            <div class="card-header">
                                <div class="card-title fw-bold">Mô tả chi tiết</div>
                            </div>
                            <div class="card-body chitiet-baiviet-bc">
                                <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- RIGHT: PRODUCT + SELLER -->
                    <div class="col-lg-4">

                        <!-- PRODUCT INFO / ACTIONS -->
                        <div class="card shadow-sm">
                            <div class="card-body">

                                <div class="h3 mb-1">
                                    <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                </div>

                                <div class="text-muted mb-2">
                                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                </div>

                                <div class="p-3 rounded-3 border bg-light mb-2">
                                    <div class="text-muted small">Giá</div>
                                    <div class="h2 text-danger mb-0">
                                        <asp:Label ID="Label5" runat="server" Text=""></asp:Label> đ
                                    </div>
                                </div>

                                <div class="text-muted mb-1">
                                    <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="text-muted mb-3">
                                    <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                                </div>

                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                                    <div class="mb-3">
                                        <label class="form-label">Số lượng</label>
                                        <asp:TextBox ID="txt_soluong1"
                                            Text="1"
                                            runat="server"
                                            CssClass="form-control"
                                            MaxLength="3"
                                            onfocus="AutoSelect(this)"
                                            oninput="format_sotien_new(this)"></asp:TextBox>
                                    </div>

                                    <div class="d-grid gap-2">
                                        <div class="row g-2">
                                            <div class="col-6 d-none">
                                                <asp:Button ID="but_bansanphamnay" OnClick="but_bansanphamnay_Click" runat="server"
                                                    Text="Bán chéo" CssClass="btn btn-outline-warning w-100" Visible="false" />
                                            </div>
                                            <div class="col-12">
                                                <asp:Button ID="but_traodoi" OnClick="but_traodoi_Click" runat="server"
                                                    Text="Trao đổi" CssClass="btn btn-outline-info w-100"
                                                    OnClientClick="return goToTraoDoiPage();" />
                                            </div>
                                            <div class="col-12">
                                                <asp:Button ID="but_themvaogio" OnClick="but_themvaogio_Click" runat="server"
                                                    Text="Thêm vào giỏ" CssClass="btn btn-outline-success w-100" />
                                            </div>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>

                            </div>
                        </div>

                        <!-- SELLER INFO -->
                        <div class="card shadow-sm mt-4">
                            <div class="card-header">
                                <div class="card-title fw-bold">Thông tin người bán</div>
                            </div>
                            <div class="card-body">
                                <div class="d-flex align-items-center mb-3">
                                   <a href="<%=ViewState["link_nguoiban"] %>"><img src="<%= ViewState["avt_query"] %>" width="52" height="52"
                                        class="rounded-circle border bg-white me-2" /></a>
                                    <div class="min-w-0">
                                        <div class="fw-bold text-truncate"><a href="<%=ViewState["link_nguoiban"] %>"><%= ViewState["hoten_query"] %></a></div>
                                        <div class="text-muted small">
                                            <i class="ti ti-star-filled text-warning"></i>
                                            <span class="ms-1">5</span>
                                            <span class="ms-2">· 39 đã bán</span>
                                            <span class="ms-2">· 6 đang bán</span>
                                        </div>
                                        <div class="text-muted small mt-1">Hoạt động 3 giờ trước</div>
                                        <div class="text-muted small">Phản hồi: 98%</div>
                                    </div>
                                </div>

                                <%--<button type="button" class="btn btn-danger w-100 mb-2 fw-bold">
                                    <i class="ti ti-phone me-2"></i>Gọi điện
                                </button>
                                <button type="button" class="btn btn-info w-100 fw-bold">
                                    <i class="ti ti-messages me-2"></i>Chat
                                </button>--%>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ====== REVIEWS ====== -->
    <asp:UpdatePanel ID="Review" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="true">
                <div class="container-xl product-wrap pb-3">
                    <div class="card shadow-sm">
                        <div class="card-body text-center">
                            <div class="h2 mb-0">Đánh giá sản phẩm</div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="InputReview" runat="server" Visible="false">
                <div class="container-xl product-wrap pb-3">
                    <div class="card shadow-sm">
                        <div class="card-header">
                            <div class="card-title fw-bold">Gửi đánh giá</div>
                        </div>
                        <div class="card-body">
                            <asp:Label ID="lblNhapDanhGia" runat="server" Text="Nội dung đánh giá:" />
                            <br />
                            <asp:HiddenField ID="hdDiem" runat="server" />

                            <div id="starRating" class="mt-2" style="font-size: 26px; color: gray; cursor: pointer;">
                                <span data-value="1">&#9733;</span>
                                <span data-value="2">&#9733;</span>
                                <span data-value="3">&#9733;</span>
                                <span data-value="4">&#9733;</span>
                                <span data-value="5">&#9733;</span>
                            </div>

                            <div class="mt-3">
                                <asp:TextBox ID="txtNoiDung" runat="server" TextMode="MultiLine" CssClass="form-control" Width="100%" Rows="4" />
                            </div>

                            <div class="mt-3">
                                <input class="form-control" type="file" id="fileInput3" onchange="uploadFile()" />
                                <div id="message" class="text-danger small mt-2"></div>
                                <div id="uploadedFilePath3" class="mt-2"></div>

                                <asp:TextBox ID="TxtIcon" runat="server" CssClass="form-control" Style="display:none;"></asp:TextBox>

                                <button type="button" id="btnRemoveImage" class="btn btn-outline-danger btn-sm mt-2" style="display:none;" onclick="removeUploadedImage()">Xoá ảnh</button>
                            </div>

                            <div class="mt-3">
                                <asp:Button ID="btnGuiDanhGia" runat="server" Text="Gửi đánh giá" CssClass="btn btn-primary" OnClick="btnGuiDanhGia_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ListReview" runat="server" Visible="true">
                <div class="container-xl product-wrap pb-3">
                    <asp:Repeater ID="rptDanhGia" runat="server">
                        <ItemTemplate>
                            <div class="card shadow-sm mb-3">
                                <div class="card-body d-flex align-items-start gap-3">
                                    <asp:Image ID="imgAvatar" runat="server" ImageUrl='<%# Eval("AnhDaiDien") %>' Width="52" Height="52" CssClass="rounded-circle border" AlternateText="Ảnh đại diện" />

                                    <div class="flex-grow-1 min-w-0">
                                        <a href="<%#Eval("HoSoUrl")%>" class="text-decoration-none">
                                            <b><%# Eval("HoTen") %></b>
                                        </a>
                                        <div class="mt-1" style="color:#ffc107; font-size: 18px;">
                                            <%# new string('★', Convert.ToInt32(Eval("Diem"))) %>
                                        </div>
                                        <div class="text-muted small fst-italic">
                                            <%# Eval("NgayDang", "{0:dd/MM/yyyy HH:mm}") %>
                                        </div>
                                        <div class="mt-2"><%# Eval("NoiDung") %></div>

                                        <div class="mt-2">
                                            <asp:Image ID="imgReview" runat="server"
                                                ImageUrl='<%# Eval("UrlAnh") %>' Width="110"
                                                CssClass="review-img"
                                                Visible='<%# !string.IsNullOrEmpty(Eval("UrlAnh") as string) %>' />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="card shadow-sm">
                        <div class="card-body">
                            <asp:Panel ID="pnlPaging" runat="server" CssClass="pagination"></asp:Panel>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!-- 1 modal dùng chung cho ảnh review -->
            <div id="imgModal">
                <span class="close">&times;</span>
                <img class="modal-content" id="modalImage" />
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="position-fixed top-0 start-0 w-100 h-100" style="background:rgba(0,0,0,.6); z-index: 99999;">
                <div class="h-100 d-flex align-items-center justify-content-center">
                    <div class="spinner-border" role="status" aria-label="loading"></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
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

    if (stars && stars.length) {
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
    }

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
        if (hiddenInput) updateStarDisplay(hiddenInput.value || "1");
        if (typeof btnGuiDanhGia !== "undefined" && btnGuiDanhGia) btnGuiDanhGia.disabled = false;
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
                        "<div class='small text-muted mb-1'>Ảnh mới chọn:</div><img id='previewImage' width='110' class='review-img' src='" + url + "' />";
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

    function goToTraoDoiPage() {
        var qty = 1;
        var qtyInput = document.getElementById('<%= txt_soluong1.ClientID %>');
        if (qtyInput) {
            var parsed = parseInt((qtyInput.value || "1").replace(/[^\d-]/g, ""), 10);
            if (!isNaN(parsed) && parsed > 0) qty = parsed;
        }

        var baseUrl = "<%= System.Web.HttpUtility.JavaScriptStringEncode(BuildExchangePageUrl((ViewState["idsp"] ?? "").ToString(), 1, (ViewState["user_bancheo"] ?? "").ToString())) %>";
        try {
            var url = new URL(baseUrl, window.location.origin);
            url.searchParams.set("qty", String(qty));
            window.location.href = url.pathname + url.search;
        } catch (e) {
            window.location.href = baseUrl;
        }
        return false;
    }

    document.addEventListener("DOMContentLoaded", function () {
        var images = document.querySelectorAll('.review-img');
        var modal = document.getElementById("imgModal");
        var modalImg = document.getElementById("modalImage");
        var closeBtn = document.querySelector("#imgModal .close");

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

    function guessVideoMimeByUrl(url) {
        var value = (url || "").toLowerCase();
        var dot = value.lastIndexOf(".");
        var ext = dot >= 0 ? value.substring(dot) : "";
        if (ext === ".webm") return "video/webm";
        if (ext === ".ogv") return "video/ogg";
        if (ext === ".mov") return "video/quicktime";
        if (ext === ".m4v") return "video/x-m4v";
        if (ext === ".3gp") return "video/3gpp";
        if (ext === ".avi") return "video/x-msvideo";
        if (ext === ".mkv") return "video/x-matroska";
        return "video/mp4";
    }

    function changeMainMedia(src, type) {
        var host = document.querySelector(".gallery-main");
        if (!host) return;

        if ((type || "").toLowerCase() === "video") {
            host.innerHTML =
                "<video controls playsinline preload='metadata' style='max-width:100%;max-height:100%;object-fit:contain'>" +
                "<source src='" + src + "' type='" + guessVideoMimeByUrl(src) + "'></video>";
        } else {
            host.innerHTML = "<img src='" + src + "' alt='media' style='max-width:100%;max-height:100%;object-fit:contain' />";
        }
    }
</script>
</asp:Content>
