<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu-left-uc.ascx.cs" Inherits="admin_uc_menu_left_uc" %>
<div class="navview-pane" style="z-index: 6">
    <div class="d-flex flex-align-center bg-navview-left-bc">
        <span class="pull-button m-0 bg-navview-left-bc-hover">
            <span class="mif-menu fg-white"></span>
        </span>
        <div class="app-title h4 text-light m-0 fg-white pl-2" style="line-height: 1.2; padding-top: 10px; padding-bottom: 10px;">
            <div>AHASALE.VN</div>
            <div class="admin-left-subtitle"><%=Server.HtmlEncode(ViewState["admin_role_label"] as string ?? "Trang quản trị") %></div>
            <div class="admin-left-role-scope"><%=Server.HtmlEncode(ViewState["admin_scope_label"] as string ?? "Cổng Admin") %></div>
        </div>
    </div>

    <%-- <div class="suggest-box">
        <input id="search" list="suggestions" type="text" data-role="input" data-clear-button="true" data-search-button="false" placeholder="Tìm nhanh..." autocomplete="off" onkeypress="if (event.keyCode==13) return false;">
        <datalist id="suggestions">
            <!-- Các suggestion sẽ được thêm từ JavaScript -->
        </datalist>

        <script>
            // Bao gồm đoạn mã trong hàm tự chạyq
            (function () {
                var suggestionsArray = [
                    { label: "Quản lý menu", link: "/admin/quan-ly-menu/default.aspx" },
                    { label: "Quản lý bài viết", link: "/admin/quan-ly-bai-viet/default.aspx" },
                    // Các suggestion khác tương ứng
                ];

                var searchInput = document.getElementById("search");

                // Sự kiện khi người dùng nhập
                searchInput.addEventListener("input", function () {
                    var keyword = searchInput.value.toLowerCase();
                    updateSuggestions(keyword);
                });

                // Sự kiện khi người dùng chọn một suggestion
                searchInput.addEventListener("change", function () {
                    var selectedValue = searchInput.value.toLowerCase();
                    navigateToLink(selectedValue);
                });

                // Hàm cập nhật danh sách gợi ý
                function updateSuggestions(keyword) {
                    var datalist = document.getElementById("suggestions");
                    // Xóa các option hiện tại
                    datalist.innerHTML = "";

                    // Lặp qua mảng và thêm các suggestion thỏa mãn từ khóa
                    suggestionsArray.forEach(function (suggestion) {
                        var lowerSuggestion = suggestion.label.toLowerCase();
                        if (lowerSuggestion.includes(keyword)) {
                            var option = document.createElement("option");
                            option.value = suggestion.label;
                            datalist.appendChild(option);
                        }
                    });
                }

                // Hàm điều hướng đến link tương ứng
                function navigateToLink(selectedValue) {
                    // Tìm suggestion có giá trị trùng khớp
                    var selectedSuggestion = suggestionsArray.find(function (suggestion) {
                        return suggestion.label.toLowerCase() === selectedValue;
                    });

                    // Nếu tìm thấy suggestion, điều hướng đến link
                    if (selectedSuggestion) {
                        window.location.href = selectedSuggestion.link;
                    }
                }
            })();
        </script>

        <button class="holder">
            <span class="mif-search fg-white"></span>
        </button>
    </div>--%>


    <ul class="navview-menu" id="side-menu">
        <% if (ShowLeftGroupAdmin()) { %>
        <li class="item-header">QUẢN TRỊ ADMIN</li>
        <% if (ShowLeftAdminDashboard()) { %>
        <li class="<%=MenuActive("/admin/default.aspx") %>">
            <a href="<%= GetAdminDashboardUrl() %>">
                <span class="icon"><span class="mif-home"></span></span>
                <span class="caption">Trang chủ admin</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminAccount()) { %>
        <li class="<%=MenuActiveTaiKhoanScope("admin") %>">
            <a href="<%= GetAdminAccountManagementUrl() %>">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản admin</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminOtp()) { %>
        <li class="<%=MenuActive("/admin/otp/default.aspx") %>">
            <a href="<%= GetAdminOtpUrl() %>">
                <span class="icon"><span class="mif-mobile"></span></span>
                <span class="caption">Quản lý OTP</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminTokenWallet()) { %>
        <li class="<%=MenuActiveTokenWallet() %>">
            <a href="<%= GetAdminTokenWalletUrl() %>">
                <span class="icon"><span class="mif-diamond"></span></span>
                <span class="caption">Ví token điểm</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminGopY()) { %>
        <li class="<%=MenuActive("/admin/quan-ly-gop-y/default.aspx") %>">
            <a href="<%= GetAdminGopYUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Quản lý góp ý</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminThongBao()) { %>
        <li class="<%=MenuActive("/admin/quan-ly-thong-bao/default.aspx", "/admin/quan-ly-thong-bao/in.aspx", "/admin/quan-ly-thong-bao/bo-loc.aspx", "/admin/quan-ly-thong-bao/xuat-du-lieu.aspx", "/admin/quan-ly-thong-bao/ban-in.aspx") %>">
            <a href="<%= GetAdminThongBaoUrl() %>">
                <span class="icon"><span class="mif-bell"></span></span>
                <span class="caption">Quản lý thông báo</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminTuVan()) { %>
        <li class="<%=MenuActive("/admin/yeu-cau-tu-van/default.aspx", "/admin/yeu-cau-tu-van/bo-loc.aspx", "/admin/yeu-cau-tu-van/xuat-du-lieu.aspx", "/admin/yeu-cau-tu-van/ban-in.aspx") %>">
            <a href="<%= GetAdminTuVanUrl() %>">
                <span class="icon"><span class="mif-bubbles"></span></span>
                <span class="caption">Yêu cầu tư vấn</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminCompanyShopSync()) { %>
        <li class="<%=MenuActive("/admin/tools/company-shop-sync.aspx") %>">
            <a href="<%= GetAdminCompanyShopSyncUrl() %>">
                <span class="icon"><span class="mif-loop2"></span></span>
                <span class="caption">Đồng bộ shop công ty</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminReindexBaiViet()) { %>
        <li class="<%=MenuActive("/admin/tools/reindex-baiviet.aspx") %>">
            <a href="<%= GetAdminReindexBaiVietUrl() %>">
                <span class="icon"><span class="mif-database"></span></span>
                <span class="caption">Reindex bài viết</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupHomeSpace()) { %>
        <li class="item-header">QUẢN TRỊ KHÔNG GIAN HOME</li>
        <% if (ShowLeftTransferHistory()) { %>
        <li class="<%=MenuActiveTransferHistory() %>">
            <a href="<%= GetHomeTransferHistoryUrl() %>">
                <span class="icon"><span class="mif-tab"></span></span>
                <span class="caption">Lịch sử chuyển điểm</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftHomeAccount()) { %>
        <li class="<%=MenuActiveTaiKhoanScope("home") %>">
            <a href="<%= GetHomeAccountManagementUrl() %>">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản home</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftApproveHanhVi()) { %>
        <li class="<%=MenuActivePointApproval() %>">
            <a href="<%= GetApproveHomePointUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt yêu cầu điểm / hành vi</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftIssueCard()) { %>
        <li class="<%=MenuActive("/admin/phat-hanh-the.aspx", "/admin/phat-hanh-the/them-moi.aspx") %>">
            <a href="<%= GetHomeIssueCardUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Phát hành thẻ</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftTierDescription()) { %>
        <li class="<%=MenuActive("/admin/motacapbac.aspx", "/admin/MoTaCapBac.aspx") %>">
            <a href="<%= GetHomeTierDescriptionUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Mô tả cấp bậc</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftSellProduct()) { %>
        <li class="<%=MenuActive("/admin/he-thong-san-pham/ban-san-pham.aspx", "/admin/he-thong-san-pham/ban-the.aspx", "/admin/he-thong-san-pham/chi-tiet-giao-dich.aspx") %>">
            <a href="<%= GetHomeSellProductUrl() %>">
                <span class="icon"><span class="mif-credit-card"></span></span>
                <span class="caption">Bán sản phẩm</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupShop()) { %>
        <li class="item-header">QUẢN TRỊ KHÔNG GIAN GIAN HÀNG ĐỐI TÁC</li>
        <% if (ShowLeftShopWorkspace()) { %>
        <li class="<%=MenuActive("/gianhang/admin", "/gianhang/admin/", "/gianhang/admin/default.aspx") %>">
            <a href="<%= GetShopWorkspaceUrl() %>">
                <span class="icon"><span class="mif-shop"></span></span>
                <span class="caption">Trung tâm quản trị gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopAccount()) { %>
        <li class="<%=MenuActiveTaiKhoanScope("shop") %>">
            <a href="<%= GetShopAccountManagementUrl() %>">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản gian hàng đối tác</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopPointApproval()) { %>
        <li class="<%=MenuActiveShopPointApproval() %>">
            <a href="<%= GetShopPointApprovalUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt điểm / nghiệp vụ shop</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopEmailTemplate()) { %>
        <li class="<%=MenuActive("/admin/quan-ly-email-shop/default.aspx") %>">
            <a href="<%= GetShopEmailTemplateUrl() %>">
                <span class="icon"><span class="mif-mail"></span></span>
                <span class="caption">Nội dung email gian hàng đối tác</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopApprove()) { %>
        <li class="<%=MenuActive("/admin/duyet-gian-hang-doi-tac.aspx") %>">
            <a href="<%= GetGianHangApprovalUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt không gian gian hàng</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/duyet-shop-doi-tac.aspx") %>">
            <a href="<%= GetShopPartnerApprovalUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt gian hàng đối tác (Shop)</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLevel2()) { %>
        <li class="<%=MenuActive("/admin/duyet-nang-cap-level2.aspx") %>">
            <a href="<%= GetShopLevel2ApprovalUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt nâng cấp Level 2</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupDauGia()) { %>
        <li class="item-header">QUẢN TRỊ KHÔNG GIAN ĐẤU GIÁ</li>
        <li class="<%=MenuActive("/daugia/admin", "/daugia/admin/", "/daugia/admin/portal.aspx") %>">
            <a href="<%= GetDauGiaAdminUrl() %>">
                <span class="icon"><span class="mif-hammer"></span></span>
                <span class="caption">Trung tâm quản trị đấu giá</span>
            </a>
        </li>
        <% } %>

        <% if (ShowLeftGroupEvent()) { %>
        <li class="item-header">QUẢN TRỊ KHÔNG GIAN SỰ KIỆN</li>
        <li class="<%=MenuActive("/event/admin", "/event/admin/", "/event/admin/default.aspx") %>">
            <a href="<%= GetEventAdminUrl() %>">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Trung tâm quản trị sự kiện</span>
            </a>
        </li>
        <% } %>

        <% if (ShowLeftGroupContent()) { %>
        <li class="item-header">QUẢN TRỊ NỘI DUNG WEBSITE</li>
        <% if (ShowHomeLandingSettingsTab() || ShowHomeLandingContentTab()) { %>
        <li class="<%=MenuActive("/admin/cai-dat-trang-chu/default.aspx") %>">
            <a href="<%= GetContentSettingsUrl() %>">
                <span class="icon"><span class="mif-cog"></span></span>
                <span class="caption">Cài đặt trang chủ</span>
            </a>
        </li>
        <% } %>
        <% if (ShowHomeLandingContentTab()) { %>
        <li class="<%=MenuActive("/admin/quan-ly-noi-dung-home/default.aspx") %>">
            <a href="<%= GetContentHomeTextUrl() %>">
                <span class="icon"><span class="mif-pencil"></span></span>
                <span class="caption">Nội dung trang chủ Home</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentMenu()) { %>
        <li class="<%=MenuActive("/admin/quan-ly-menu/default.aspx", "/admin/quan-ly-menu/them-moi.aspx", "/admin/quan-ly-menu/bo-loc.aspx", "/admin/quan-ly-menu/chinh-sua.aspx", "/admin/quan-ly-menu/xuat-du-lieu.aspx", "/admin/quan-ly-menu/ban-in.aspx") %>">
            <a href="<%= GetContentMenuUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Quản lý menu</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentBaiViet()) { %>
        <li class="<%=MenuActive("/admin/quan-ly-bai-viet/default.aspx", "/admin/quan-ly-bai-viet/in.aspx", "/admin/quan-ly-bai-viet/them-moi.aspx", "/admin/quan-ly-bai-viet/bo-loc.aspx", "/admin/quan-ly-bai-viet/chinh-sua.aspx", "/admin/quan-ly-bai-viet/xuat-du-lieu.aspx", "/admin/quan-ly-bai-viet/ban-in.aspx") %>">
            <a href="<%= GetContentBaiVietUrl() %>">
                <span class="icon"><span class="mif-news"></span></span>
                <span class="caption">Quản lý bài viết</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentBanner()) { %>
        <li class="<%=MenuActive("/admin/quan-ly-banner/default.aspx", "/admin/quan-ly-banner/them-moi.aspx") %>">
            <a href="<%= GetContentBannerUrl() %>">
                <span class="icon"><span class="mif-images"></span></span>
                <span class="caption">Quản lý banner</span>
            </a>
        </li>
        <% } %>
        <% } %>
    </ul>

    <div class="w-100 text-center text-small data-box p-2 border-top bd-darkGreen" style="position: absolute; bottom: 0">
        <%--bg-navview-foot-bc--%>
        <div>Sản phẩm của <a href="/" class="text-muted fg-white-hover no-decor">AhaSale.vn</a></div>
    </div>

</div>
