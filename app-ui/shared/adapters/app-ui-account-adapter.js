(function () {
    function resolveScopeNote(currentSpace) {
        if (currentSpace === "batdongsan") return "Aha Land";
        if (currentSpace === "gianhang") return "Aha Shop";
        if (currentSpace === "choxe") return "Aha Xe";
        if (currentSpace === "dienthoai-maytinh") return "Aha Tech";
        return "Aha Sale";
    }

    function buildLoginHref() {
        var returnUrl = encodeURIComponent(getCurrentAppHref());
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + returnUrl;
    }

    function getCurrentAppHref() {
        return sanitizeAppHref((window.location.pathname || "/") + (window.location.search || ""));
    }

    function sanitizeAppHref(value) {
        if (!value) return "";
        var raw = String(value);
        var hashIndex = raw.indexOf("#");
        var hash = "";
        var base = raw;
        if (hashIndex >= 0) {
            hash = raw.slice(hashIndex);
            base = raw.slice(0, hashIndex);
        }
        var queryIndex = base.indexOf("?");
        if (queryIndex < 0) return base + hash;
        var path = base.slice(0, queryIndex);
        var query = base.slice(queryIndex + 1);
        var params = new URLSearchParams(query);
        params.delete("back_href");
        params.delete("return_url");
        var nextQuery = params.toString();
        return path + (nextQuery ? "?" + nextQuery : "") + hash;
    }

    function appendQuery(url, key, value) {
        if (!url || !key || !value) return url || "";
        if (url.indexOf(key + "=") >= 0) return url;

        var hashIndex = url.indexOf("#");
        var hash = "";
        if (hashIndex >= 0) {
            hash = url.slice(hashIndex);
            url = url.slice(0, hashIndex);
        }

        return url + (url.indexOf("?") >= 0 ? "&" : "?") + key + "=" + encodeURIComponent(value) + hash;
    }

    function buildRuntimeAwareHref(url) {
        return appendQuery(sanitizeAppHref(url), "return_url", getCurrentAppHref());
    }

    function buildAppAwareHref(url) {
        if (!url) return "";
        var rawUrl = String(url);
        if (rawUrl.indexOf("/app-ui/") !== 0) return rawUrl;
        var safeUrl = sanitizeAppHref(rawUrl);
        if (!safeUrl) return safeUrl;
        if (safeUrl.indexOf("/app-ui/auth/") === 0) return buildRuntimeAwareHref(safeUrl);
        return appendQuery(safeUrl, "back_href", getCurrentAppHref());
    }

    function buildGuestMenu(currentSpace) {
        return {
            main_menu_items: [
                { label: "Đăng nhập", sub: "Đăng nhập tài khoản Home để tiếp tục", href: buildLoginHref() },
                { label: "Đăng ký", sub: "Tạo tài khoản mới để dùng app", href: buildRuntimeAwareHref("/app-ui/auth/register.aspx?ui_mode=app") },
                { label: "Mở gian hàng", sub: "Gửi yêu cầu mở gian hàng đối tác", href: buildRuntimeAwareHref("/app-ui/auth/open-shop.aspx?ui_mode=app") }
            ],
            utility_items: [],
            promo_items: [],
            misc_items: [],
            scope_note: resolveScopeNote(currentSpace)
        };
    }

    function getAccountMenu(currentSpace, session) {
        if (session && session.auth_state === "guest") {
            return buildGuestMenu(currentSpace);
        }
        if (currentSpace === "gianhang") {
            var gianhangMenu = {
                main_menu_items: [
                    { label: "Trang chủ gian hàng", sub: "Tổng quan seller dashboard", href: "/app-ui/gianhang/default.aspx?ui_mode=app" },
                    { label: "Quản lý tin", sub: "Danh sách và trạng thái tin đăng", href: "/app-ui/gianhang/listing.aspx?ui_mode=app" },
                    { label: "Đăng tin", sub: "Tạo mới hoặc tiếp tục chỉnh sửa bản nháp", href: "/app-ui/gianhang/post.aspx?ui_mode=app" },
                    { label: "Tạo đơn", sub: "Khởi tạo đơn bán từ gian hàng", href: "/app-ui/gianhang/create-order.aspx?ui_mode=app" },
                    { label: "Chờ thanh toán", sub: "Theo dõi luồng chờ thanh toán", href: "/app-ui/gianhang/pending-payment.aspx?ui_mode=app" },
                    { label: "Đơn hàng", sub: "Theo dõi đơn bán / đơn mua", href: "/app-ui/gianhang/orders.aspx?ui_mode=app" },
                    { label: "Khách hàng", sub: "Hội thoại và lịch sử liên hệ", href: "/app-ui/gianhang/conversations.aspx?ui_mode=app" },
                    { label: "Lịch hẹn", sub: "Lịch hẹn liên quan đơn và khách", href: "/app-ui/gianhang/appointments.aspx?ui_mode=app" }
                ],
                utility_items: [
                    { label: "Báo cáo", sub: "Hiệu suất và thống kê gian hàng", href: "/app-ui/gianhang/reports.aspx?ui_mode=app" },
                    { label: "Trang công khai", sub: "Giao diện public gian hàng", href: "/app-ui/gianhang/storefront.aspx?ui_mode=app" },
                    { label: "Tài khoản gian hàng", sub: "Cài đặt hồ sơ vận hành", href: "/app-ui/gianhang/account.aspx?ui_mode=app" }
                ],
                promo_items: [],
                misc_items: [
                    { label: "Đổi mật khẩu tài khoản", sub: "Bảo mật tài khoản", href: "/app-ui/home/change-password.aspx?ui_mode=app" },
                    { label: "Đăng xuất", sub: "Thoát tài khoản hiện tại", href: "/app-ui/auth/logout.aspx?ui_mode=app" }
                ],
                scope_note: resolveScopeNote(currentSpace)
            };
            ["main_menu_items", "utility_items", "promo_items", "misc_items"].forEach(function (key) {
                (gianhangMenu[key] || []).forEach(function (item) {
                    item.href = buildAppAwareHref(item.href);
                });
            });
            return gianhangMenu;
        }
        var homeMenu = {
            main_menu_items: [
                { label: "Trang cá nhân", sub: "Mở hồ sơ cá nhân trong app", href: "/app-ui/home/profile.aspx?ui_mode=app" },
                { label: "Tạo đề xuất", sub: "Gửi yêu cầu / đề xuất từ hệ Home", href: "/app-ui/home/proposals.aspx?ui_mode=app" },
                { label: "Sao chép Link giới thiệu", sub: "Sao chép nhanh link giới thiệu của bạn", href: "javascript:void(0)", action: "copy_referral_link" },
                { label: "Đổi mã pin thẻ", sub: "Bảo mật mã pin", href: "/app-ui/home/change-pin.aspx?ui_mode=app" },
                { label: "Đổi mật khẩu tài khoản", sub: "Bảo mật tài khoản", href: "/app-ui/home/change-password.aspx?ui_mode=app" },
                { label: "Đơn mua", sub: "Theo dõi các đơn đang có", href: "/app-ui/home/orders.aspx?ui_mode=app" },
                { label: "Lịch hẹn của tôi", sub: "Lịch hẹn và nhắc việc", href: "/app-ui/home/appointments.aspx?ui_mode=app" },
                { label: "Lịch sử Trao đổi", sub: "Theo dõi lịch sử tương tác", href: "/app-ui/home/exchange-history.aspx?ui_mode=app" }
            ],
            utility_items: [
                { label: "Tin đăng đã lưu", sub: "Các tin bạn đã lưu", href: "/app-ui/home/saved.aspx?ui_mode=app" },
                { label: "Lịch sử xem tin", sub: "Nội dung đã xem gần đây", href: "/app-ui/home/view-history.aspx?ui_mode=app" },
                { label: "Đánh giá từ tôi", sub: "Những đánh giá đã gửi", href: "/app-ui/home/my-reviews.aspx?ui_mode=app" }
            ],
            promo_items: [
                { label: "Hồ sơ quyền tiêu dùng", sub: "Lịch sử trao đổi quyền tiêu dùng", href: "/app-ui/home/benefit-consumer.aspx?ui_mode=app" },
                { label: "Hồ sơ quyền ưu đãi", sub: "Theo dõi quyền ưu đãi", href: "/app-ui/home/benefit-offers.aspx?ui_mode=app" },
                { label: "Hồ sơ hành vi lao động", sub: "Theo dõi hành vi lao động", href: "/app-ui/home/benefit-labor.aspx?ui_mode=app" },
                { label: "Hồ sơ chỉ số gắn kết", sub: "Theo dõi chỉ số gắn kết", href: "/app-ui/home/benefit-engagement.aspx?ui_mode=app" }
            ],
            misc_items: [
                { label: "Cài đặt tài khoản", sub: "Cập nhật thông tin cá nhân", href: "/app-ui/home/settings.aspx?ui_mode=app" },
                { label: "Trợ giúp", sub: "Hướng dẫn và hỗ trợ người dùng", href: "/app-ui/home/help.aspx?ui_mode=app" },
                { label: "Đóng góp ý kiến", sub: "Gửi góp ý cho nền tảng", href: "/app-ui/home/feedback.aspx?ui_mode=app" },
                { label: "Đăng xuất", sub: "Thoát tài khoản hiện tại", href: "/app-ui/auth/logout.aspx?ui_mode=app" }
            ],
            scope_note: resolveScopeNote(currentSpace)
        };
        ["main_menu_items", "utility_items", "promo_items", "misc_items"].forEach(function (key) {
            (homeMenu[key] || []).forEach(function (item) {
                item.href = buildAppAwareHref(item.href);
            });
        });
        return homeMenu;
    }

    window.AhaAppUiAdapters = window.AhaAppUiAdapters || {};
    window.AhaAppUiAdapters.account = {
        getAccountMenu: getAccountMenu
    };
})();
