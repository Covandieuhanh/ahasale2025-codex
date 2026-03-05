<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu-left-uc.ascx.cs" Inherits="admin_uc_menu_left_uc" %>
<div class="navview-pane" style="z-index: 6">
    <div class="d-flex flex-align-center bg-navview-left-bc">
        <span class="pull-button m-0 bg-navview-left-bc-hover">
            <span class="mif-menu fg-white"></span>
        </span>
        <div class="app-title h4 text-light m-0 fg-white pl-2" style="line-height: 1.2; padding-top: 10px; padding-bottom: 10px;">
            <div>AHASALE.VN</div>
            <div class="admin-left-subtitle">Trang quản trị</div>
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
        <li class="item-header">QUẢN LÝ ADMIN</li>
        <li class="<%=MenuActive("/admin/default.aspx") %>">
            <a href="/admin/default.aspx">
                <span class="icon"><span class="mif-home"></span></span>
                <span class="caption">Trang chủ admin</span>
            </a>
        </li>
        <li class="<%=MenuActiveTaiKhoanScope("admin") %>">
            <a href="/admin/quan-ly-tai-khoan/default.aspx?scope=admin">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản admin</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/lich-su-chuyen-diem/default.aspx") %>">
            <a href="/admin/lich-su-chuyen-diem/default.aspx">
                <span class="icon"><span class="mif-tab"></span></span>
                <span class="caption">Lịch sử chuyển điểm</span>
            </a>
        </li>

        <li class="item-header">QUẢN LÝ HOME</li>
        <li class="<%=MenuActiveTaiKhoanScope("home") %>">
            <a href="/admin/quan-ly-tai-khoan/default.aspx?scope=home">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản home</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/duyet-yeu-cau-len-cap.aspx") %>">
            <a href="/admin/duyet-yeu-cau-len-cap.aspx">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt yêu cầu lên cấp</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/phat-hanh-the.aspx") %>">
            <a href="/admin/phat-hanh-the.aspx">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Phát hành thẻ</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/motacapbac.aspx", "/admin/MoTaCapBac.aspx") %>">
            <a href="/admin/MoTaCapBac.aspx">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Mô tả cấp bậc</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/he-thong-san-pham/ban-san-pham.aspx") %>">
            <a href="/admin/he-thong-san-pham/ban-san-pham.aspx">
                <span class="icon"><span class="mif-credit-card"></span></span>
                <span class="caption">Bán sản phẩm</span>
            </a>
        </li>

        <li class="item-header">QUẢN LÝ SHOP</li>
        <li class="<%=MenuActiveTaiKhoanScope("shop") %>">
            <a href="/admin/quan-ly-tai-khoan/default.aspx?scope=shop">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản shop</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/duyet-gian-hang-doi-tac.aspx") %>">
            <a href="/admin/duyet-gian-hang-doi-tac.aspx">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Duyệt gian hàng đối tác</span>
            </a>
        </li>

        <li class="item-header">QUẢN LÝ NỘI DUNG</li>
        <li class="<%=MenuActive("/admin/cai-dat-trang-chu/default.aspx") %>">
            <a href="/admin/cai-dat-trang-chu/default.aspx">
                <span class="icon"><span class="mif-cog"></span></span>
                <span class="caption">Cài đặt trang chủ</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/quan-ly-menu/default.aspx") %>">
            <a href="/admin/quan-ly-menu/default.aspx">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Quản lý menu</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/quan-ly-bai-viet/default.aspx") %>">
            <a href="/admin/quan-ly-bai-viet/default.aspx">
                <span class="icon"><span class="mif-news"></span></span>
                <span class="caption">Quản lý bài viết</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/quan-ly-banner/default.aspx") %>">
            <a href="/admin/quan-ly-banner/default.aspx">
                <span class="icon"><span class="mif-images"></span></span>
                <span class="caption">Quản lý banner</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/quan-ly-gop-y/default.aspx") %>">
            <a href="/admin/quan-ly-gop-y/default.aspx">
                <span class="icon"><span class="mif-list2"></span></span>
                <span class="caption">Quản lý góp ý</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/quan-ly-thong-bao/default.aspx") %>">
            <a href="/admin/quan-ly-thong-bao/default.aspx">
                <span class="icon"><span class="mif-bell"></span></span>
                <span class="caption">Quản lý thông báo</span>
            </a>
        </li>
        <li class="<%=MenuActive("/admin/yeu-cau-tu-van/default.aspx") %>">
            <a href="/admin/yeu-cau-tu-van/default.aspx">
                <span class="icon"><span class="mif-bubbles"></span></span>
                <span class="caption">Yêu cầu tư vấn</span>
            </a>
        </li>
    </ul>

    <div class="w-100 text-center text-small data-box p-2 border-top bd-darkGreen" style="position: absolute; bottom: 0">
        <%--bg-navview-foot-bc--%>
        <div>Sản phẩm của <a href="https:/bcorn.net" class="text-muted fg-white-hover no-decor">Bcorn.net</a></div>
    </div>

</div>
