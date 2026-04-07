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
        <li class="<%=MenuActiveFeature("admin_dashboard") %>">
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
        <li class="<%=MenuActiveFeature("admin_otp") %>">
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
        <li class="<%=MenuActiveFeature("admin_feedback") %>">
            <a href="<%= GetAdminGopYUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Quản lý góp ý</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminThongBao()) { %>
        <li class="<%=MenuActiveFeature("notifications") %>">
            <a href="<%= GetAdminThongBaoUrl() %>">
                <span class="icon"><span class="mif-bell"></span></span>
                <span class="caption">Quản lý thông báo</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminTuVan()) { %>
        <li class="<%=MenuActiveFeature("consulting") %>">
            <a href="<%= GetAdminTuVanUrl() %>">
                <span class="icon"><span class="mif-bubbles"></span></span>
                <span class="caption">Yêu cầu tư vấn</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminCompanyShopSync()) { %>
        <li class="<%=MenuActiveFeature("admin_company_shop_sync") %>">
            <a href="<%= GetAdminCompanyShopSyncUrl() %>">
                <span class="icon"><span class="mif-loop2"></span></span>
                <span class="caption">Đồng bộ shop công ty</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftAdminReindexBaiViet()) { %>
        <li class="<%=MenuActiveFeature("admin_reindex_baiviet") %>">
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
        <li class="<%=MenuActiveFeature("issue_cards") %>">
            <a href="<%= GetHomeIssueCardUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Phát hành thẻ</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftTierDescription()) { %>
        <li class="<%=MenuActiveFeature("tier_reference") %>">
            <a href="<%= GetHomeTierDescriptionUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Mô tả cấp bậc</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftSellProduct()) { %>
        <li class="<%=MenuActiveFeature("system_products") %>">
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
        <li class="<%=MenuActiveFeature("shop_workspace") %>">
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
        <li class="<%=MenuActiveFeature("shop_email") %>">
            <a href="<%= GetShopEmailTemplateUrl() %>">
                <span class="icon"><span class="mif-mail"></span></span>
                <span class="caption">Nội dung email gian hàng đối tác</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyInvoices()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_invoices") %>">
            <a href="<%= GetShopLegacyInvoicesUrl() %>">
                <span class="icon"><span class="mif-file-text"></span></span>
                <span class="caption">Vận hành hóa đơn gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyCustomers()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_customers") %>">
            <a href="<%= GetShopLegacyCustomersUrl() %>">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Vận hành khách hàng gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyContent()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_content") %>">
            <a href="<%= GetShopLegacyContentUrl() %>">
                <span class="icon"><span class="mif-pencil"></span></span>
                <span class="caption">Vận hành nội dung gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyFinance()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_finance") %>">
            <a href="<%= GetShopLegacyFinanceUrl() %>">
                <span class="icon"><span class="mif-calculator"></span></span>
                <span class="caption">Vận hành thu chi gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyInventory()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_inventory") %>">
            <a href="<%= GetShopLegacyInventoryUrl() %>">
                <span class="icon"><span class="mif-stack"></span></span>
                <span class="caption">Vận hành kho vật tư gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyAccounts()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_accounts") %>">
            <a href="<%= GetShopLegacyAccountsUrl() %>">
                <span class="icon"><span class="mif-user-check"></span></span>
                <span class="caption">Vận hành tài khoản gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyOrg()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_org") %>">
            <a href="<%= GetShopLegacyOrgUrl() %>">
                <span class="icon"><span class="mif-sitemap"></span></span>
                <span class="caption">Vận hành cơ cấu gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacyTraining()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_training") %>">
            <a href="<%= GetShopLegacyTrainingUrl() %>">
                <span class="icon"><span class="mif-library"></span></span>
                <span class="caption">Vận hành học viên giảng viên</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLegacySupport()) { %>
        <li class="<%=MenuActiveFeature("shop_legacy_support") %>">
            <a href="<%= GetShopLegacySupportUrl() %>">
                <span class="icon"><span class="mif-bubbles"></span></span>
                <span class="caption">Vận hành hỗ trợ gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftGianHangApproval()) { %>
        <li class="<%=MenuActiveFeature("home_gianhang_space") %>">
            <a href="<%= GetGianHangApprovalUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt không gian gian hàng</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopPartnerApproval()) { %>
        <li class="<%=MenuActiveFeature("shop_partner") %>">
            <a href="<%= GetShopPartnerApprovalUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt gian hàng đối tác (Shop)</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftShopLevel2()) { %>
        <li class="<%=MenuActiveFeature("shop_level2") %>">
            <a href="<%= GetShopLevel2ApprovalUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt nâng cấp Level 2</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupBatDongSan()) { %>
        <li class="item-header">QUẢN TRỊ KHÔNG GIAN BẤT ĐỘNG SẢN</li>
        <% if (ShowLeftBatDongSanWorkspace()) { %>
        <li class="<%=MenuActiveBatDongSanWorkspace() %>">
            <a href="<%= GetBatDongSanWorkspaceUrl() %>">
                <span class="icon"><span class="mif-home"></span></span>
                <span class="caption">Trung tâm quản trị bất động sản</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftBatDongSanLinked()) { %>
        <li class="<%=MenuActiveFeature("home_bds_linked") %>">
            <a href="<%= GetBatDongSanLinkedUrl() %>">
                <span class="icon"><span class="mif-earth"></span></span>
                <span class="caption">BĐS - Liên kết tin</span>
            </a>
        </li>
        <% } %>
        <% } %>

        <% if (ShowLeftGroupDauGia()) { %>
        <li class="item-header">QUẢN TRỊ KHÔNG GIAN ĐẤU GIÁ</li>
        <li class="<%=MenuActiveFeature("daugia_workspace") %>">
            <a href="<%= GetDauGiaAdminUrl() %>">
                <span class="icon"><span class="mif-hammer"></span></span>
                <span class="caption">Trung tâm quản trị đấu giá</span>
            </a>
        </li>
        <% } %>

        <% if (ShowLeftGroupEvent()) { %>
        <li class="item-header">QUẢN TRỊ KHÔNG GIAN SỰ KIỆN</li>
        <li class="<%=MenuActiveFeature("event_workspace") %>">
            <a href="<%= GetEventAdminUrl() %>">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Trung tâm quản trị sự kiện</span>
            </a>
        </li>
        <% } %>

        <% if (ShowLeftGroupContent()) { %>
        <li class="item-header">QUẢN TRỊ NỘI DUNG WEBSITE</li>
        <% if (ShowHomeLandingSettingsTab() || ShowHomeLandingContentTab()) { %>
        <li class="<%=MenuActiveFeature("home_config") %>">
            <a href="<%= GetContentSettingsUrl() %>">
                <span class="icon"><span class="mif-cog"></span></span>
                <span class="caption">Cài đặt trang chủ</span>
            </a>
        </li>
        <% } %>
        <% if (ShowHomeLandingContentTab()) { %>
        <li class="<%=MenuActiveFeature("home_content") %>">
            <a href="<%= GetContentHomeTextUrl() %>">
                <span class="icon"><span class="mif-pencil"></span></span>
                <span class="caption">Nội dung trang chủ Home</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentMenu()) { %>
        <li class="<%=MenuActiveFeature("home_menu") %>">
            <a href="<%= GetContentMenuUrl() %>">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Quản lý menu</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentBaiViet()) { %>
        <li class="<%=MenuActiveFeature("home_posts") %>">
            <a href="<%= GetContentBaiVietUrl() %>">
                <span class="icon"><span class="mif-news"></span></span>
                <span class="caption">Quản lý bài viết</span>
            </a>
        </li>
        <% } %>
        <% if (ShowLeftContentBanner()) { %>
        <li class="<%=MenuActiveFeature("home_banner") %>">
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
