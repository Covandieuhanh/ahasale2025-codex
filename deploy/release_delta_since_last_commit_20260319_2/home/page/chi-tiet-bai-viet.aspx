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
            height: 460px;
            background: #f6f8fb;
            border-radius: 12px;
            overflow: hidden;
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
        }
        .gallery-main img,
        .gallery-main video { max-width: 100%; max-height: 100%; object-fit: contain; }
        .gallery-main img.zoomable {
            transition: transform .25s ease;
            cursor: zoom-in;
        }
        .gallery-main:hover img.zoomable {
            transform: scale(1.04);
        }

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

        .product-sidebar {
            position: sticky;
            top: 90px;
        }

        .product-quick-info {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-bottom: 10px;
        }

        .product-chip {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 10px;
            border-radius: 999px;
            font-size: 12px;
            background: #f1f5f9;
            border: 1px solid #e2e8f0;
            color: #334e68;
        }

        .related-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 16px;
        }

        .related-card {
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            background: #fff;
            overflow: hidden;
            box-shadow: 0 10px 24px rgba(15, 23, 42, .06);
        }

        .related-card img {
            width: 100%;
            height: 160px;
            object-fit: cover;
            display: block;
        }

        .related-body {
            padding: 10px 12px 12px;
        }

        .related-title {
            font-weight: 700;
            color: #0f172a;
            line-height: 1.3;
            max-height: 38px;
            overflow: hidden;
        }

        .related-price {
            margin-top: 6px;
            color: #dc2626;
            font-weight: 700;
        }

        .mobile-cta {
            position: fixed;
            left: 0;
            right: 0;
            bottom: 0;
            z-index: 2000;
            background: #ffffff;
            border-top: 1px solid #e2e8f0;
            box-shadow: 0 -10px 24px rgba(15, 23, 42, .12);
            padding: 10px 14px;
            display: none;
        }

        .mobile-cta .cta-wrap {
            max-width: 1100px;
            margin: 0 auto;
            display: flex;
            gap: 10px;
        }

        .mobile-cta .btn {
            flex: 1;
            min-height: 44px;
            font-weight: 700;
        }

        body.mobile-cta-enabled {
            padding-bottom: 84px;
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

        html[data-bs-theme="dark"] .gallery-main {
            background: #111d34;
        }

        html[data-bs-theme="dark"] .thumbs img,
        html[data-bs-theme="dark"] .thumbs video {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .pagination .current-page {
            background-color: rgba(34, 197, 94, 0.18);
            color: #86efac;
            border-color: rgba(34, 197, 94, 0.45);
        }

        html[data-bs-theme="dark"] .product-chip {
            background: rgba(15, 23, 42, .7);
            border-color: #223246;
            color: #cbd5f5;
        }

        html[data-bs-theme="dark"] .related-card {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .related-title {
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .mobile-cta {
            background: #0f172a;
            border-color: #223246;
        }

        @media (max-width: 991px) {
            .gallery-main { height: 340px; }
            .product-sidebar { position: static; }
            .related-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .mobile-cta { display: block; }
        }

        @media (max-width: 640px) {
            .gallery-main { height: 280px; }
            .related-grid { grid-template-columns: 1fr; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:HiddenField ID="hf_anhphu" runat="server" ClientIDMode="Static" />

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
                        <div class="card shadow-sm product-sidebar">
                            <div class="card-body">

                                <div class="h3 mb-1">
                                    <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                </div>

                                <asp:Label ID="lb_quick_info" runat="server" CssClass="product-quick-info" />

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
                                <asp:PlaceHolder ID="ph_map_link" runat="server" Visible="false">
                                    <div class="mb-3 d-grid">
                                        <asp:HyperLink ID="lnk_map" runat="server" CssClass="btn btn-outline-primary w-100" Target="_blank">
                                            <i class="ti ti-map-pin"></i> Xem bản đồ
                                        </asp:HyperLink>
                                    </div>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                                    <asp:PlaceHolder ID="ph_qty_block" runat="server" Visible="true">
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
                                    </asp:PlaceHolder>

                                    <div class="d-grid gap-2">
                                        <div class="row g-2">
                                            <div class="col-6 d-none">
                                                <asp:Button ID="but_bansanphamnay" OnClick="but_bansanphamnay_Click" runat="server"
                                                    Text="Bán chéo" CssClass="btn btn-outline-warning w-100" Visible="false" />
                                            </div>
                                            <div class="col-12">
                                                <asp:Button ID="but_traodoi" OnClick="but_traodoi_Click" runat="server"
                                                    Text="Trao đổi" CssClass="btn btn-outline-info w-100"
                                                    OnClientClick="return goToTraoDoiPageSafe();" />
                                            </div>
                                            <asp:PlaceHolder ID="ph_booking" runat="server" Visible="false">
                                                <div class="col-12">
                                                    <asp:HyperLink ID="lnk_datlich" runat="server" CssClass="btn btn-success w-100">Đặt lịch</asp:HyperLink>
                                                </div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_add_to_cart" runat="server" Visible="true">
                                                <div class="col-12">
                                                    <asp:Button ID="but_themvaogio" OnClick="but_themvaogio_Click" runat="server"
                                                        Text="Thêm vào giỏ" CssClass="btn btn-outline-success w-100" />
                                                </div>
                                            </asp:PlaceHolder>
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

    <asp:PlaceHolder ID="ph_related_products" runat="server" Visible="false">
        <div class="container-xl product-wrap pb-3">
            <div class="card shadow-sm">
                <div class="card-header">
                    <div class="card-title fw-bold">Sản phẩm liên quan</div>
                </div>
                <div class="card-body">
                    <div class="related-grid">
                        <asp:Repeater ID="rpt_related_products" runat="server">
                            <ItemTemplate>
                                <a class="related-card" href="<%# Eval("Url") %>">
                                    <img src="<%# Eval("Image") %>" alt="<%# HttpUtility.HtmlEncode((Eval("Name") ?? "").ToString()) %>" loading="lazy" decoding="async" />
                                    <div class="related-body">
                                        <div class="related-title"><%# Eval("Name") %></div>
                                        <div class="related-price"><%# Eval("Price") %> đ</div>
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <!-- ====== REVIEWS ====== -->
    <div id="review-section"></div>
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

            <asp:PlaceHolder ID="phReviewGate" runat="server" Visible="false">
                <div class="container-xl product-wrap pb-3">
                    <div class="alert alert-warning text-center mb-0">
                        Bạn cần hoàn tất trao đổi sản phẩm hoặc dịch vụ này trước khi có thể đánh giá.
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ListReview" runat="server" Visible="true">
                <div class="container-xl product-wrap pb-3">
                    <asp:Repeater ID="rptDanhGia" runat="server">
                        <ItemTemplate>
                            <div class="card shadow-sm mb-3">
                                <div class="card-body d-flex align-items-start gap-3">
                                    <asp:Image ID="imgAvatar" runat="server" ImageUrl='<%# ResolveSafeImage(Eval("AnhDaiDien")) %>' Width="52" Height="52" CssClass="rounded-circle border" AlternateText="Ảnh đại diện" />

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
                                                ImageUrl='<%# ResolveSafeImage(Eval("UrlAnh")) %>' Width="110"
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

    <asp:PlaceHolder ID="phMobileCta" runat="server" Visible="false">
        <div class="mobile-cta" id="mobileCta">
            <div class="cta-wrap">
                <button type="button" class="btn btn-outline-info" onclick="triggerTraoDoi()"><asp:Literal ID="lit_mobile_traodoi" runat="server" Text="Trao đổi" /></button>
                <asp:PlaceHolder ID="ph_mobile_booking" runat="server" Visible="false">
                    <a id="lnk_mobile_datlich" runat="server" class="btn btn-success">Đặt lịch</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_mobile_addcart" runat="server" Visible="true">
                    <button type="button" class="btn btn-outline-success" onclick="triggerAddToCart()">Thêm vào giỏ</button>
                </asp:PlaceHolder>
            </div>
        </div>
    </asp:PlaceHolder>

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

    function goToTraoDoiPageSafe() {
        try {
            return goToTraoDoiPage();
        } catch (e) {
            // fallback to server postback if client script fails
            return true;
        }
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

        document.addEventListener("click", function (ev) {
            var target = ev.target;
            if (target && target.classList && target.classList.contains("product-main-media")) {
                openMediaModal(target.src);
            }
        });

        closeBtn.onclick = function () {
            modal.style.display = "none";
        };

        modal.onclick = function (e) {
            if (e.target === modal) {
                modal.style.display = "none";
            }
        };

        var mobileCta = document.getElementById("mobileCta");
        if (mobileCta) {
            document.body.classList.add("mobile-cta-enabled");
        }
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
            host.innerHTML = "<img src='" + src + "' alt='media' class='product-main-media zoomable' style='max-width:100%;max-height:100%;object-fit:contain' />";
        }
    }

    function openMediaModal(src) {
        var modal = document.getElementById("imgModal");
        var modalImg = document.getElementById("modalImage");
        if (!modal || !modalImg) return;
        modal.style.display = "block";
        modalImg.src = src;
    }

    function triggerTraoDoi() {
        var btn = document.getElementById('<%= but_traodoi.ClientID %>');
        if (btn) btn.click();
    }

    function triggerAddToCart() {
        var btn = document.getElementById('<%= but_themvaogio.ClientID %>');
        if (btn) btn.click();
    }
</script>
</asp:Content>
