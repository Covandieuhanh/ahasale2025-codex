(function () {
    function getSession() {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        return adapter && typeof adapter.getSession === "function"
            ? adapter.getSession()
            : { auth_state: "guest", seller_status: "not_registered", user_summary: null };
    }

    function getSpaceCode() {
        var shell = document.querySelector(".app-shell[data-app-shell]");
        return shell ? (shell.getAttribute("data-app-shell") || "home") : "home";
    }

    function getTitleBySpace(spaceCode) {
        if (spaceCode === "batdongsan") return "Aha Land";
        if (spaceCode === "choxe") return "Aha Xe";
        if (spaceCode === "dienthoai-maytinh") return "Aha Tech";
        if (spaceCode === "gianhang") return "Aha Shop";
        return "Aha Sale";
    }

    function buildDetailHref(spaceCode, id) {
        if (window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing && typeof window.AhaAppUiAdapters.listing.buildDetailHref === "function") {
            return window.AhaAppUiAdapters.listing.buildDetailHref(spaceCode, id);
        }
        return "/app-ui/" + spaceCode + "/detail.aspx?id=" + encodeURIComponent(id) + "&ui_mode=app";
    }

    function normalizeText(value) {
        return String(value || "")
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    }

    function isPendingOrderStatus(statusText) {
        var normalized = normalizeText(statusText);
        return normalized.indexOf("cho") >= 0
            || normalized.indexOf("trao doi") >= 0
            || normalized.indexOf("thanh toan") >= 0
            || normalized.indexOf("dang xu ly") >= 0;
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

    function decorateRuntimeHref(href) {
        if (!href) return href;
        var safeHref = sanitizeAppHref(href);
        if (safeHref.indexOf("/home/") === 0 || safeHref.indexOf("/gianhang/") === 0 || safeHref.indexOf("/xe/") === 0 || safeHref.indexOf("/bat-dong-san/") === 0) {
            return appendQuery(safeHref, "return_url", getCurrentAppHref());
        }
        return safeHref;
    }

    function renderCards(host, items) {
        if (!host) return;
        if (!items || !items.length) {
            host.innerHTML = '' +
                '<article class="app-page-card">' +
                '  <h2>Chưa có thông báo mới</h2>' +
                '  <p>Thông báo sẽ xuất hiện ở đây khi có dữ liệu mới trong không gian hiện tại.</p>' +
                '</article>';
            return;
        }
        host.innerHTML = '' +
            '<article class="app-page-card">' +
            '  <h2>Feed thông báo</h2>' +
            '  <div class="app-page-links app-notification-list">' +
            items.map(function (item) {
                return '' +
                    '<a class="app-page-link app-notification-item" href="' + item.href + '">' +
                    '  <span>' +
                    '    <strong class="app-notification-label">' + item.label + '</strong>' +
                    '    <small>' + item.sub + '</small>' +
                    '  </span>' +
                    '  <span class="app-notification-tone">' + item.meta + '</span>' +
                    '</a>';
            }).join("") +
            '  </div>' +
            '</article>';
    }

    function buildSessionCard(spaceCode, session) {
        var user = session && session.user_summary ? session.user_summary : null;
        var userName = user ? (user.display_name || user.phone || "Tài khoản") : "Khách";
        return {
            href: "/app-ui/" + (spaceCode === "gianhang" ? "gianhang/default.aspx" : spaceCode + "/default.aspx") + "?ui_mode=app",
            label: session && session.auth_state === "authenticated" ? ("Phiên đang hoạt động: " + userName) : "Bạn đang ở chế độ khách",
            sub: session && session.auth_state === "authenticated"
                ? "App đang nối theo phiên thật của tài khoản hiện tại."
                : "Đăng nhập để mở đầy đủ dữ liệu cá nhân.",
            meta: session && session.auth_state === "authenticated" ? "Hoạt động" : "Khách"
        };
    }

    async function buildMarketplaceNotifications(spaceCode, session) {
        var items = [buildSessionCard(spaceCode, session)];
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.search;
        if (!adapter || typeof adapter.searchAsync !== "function") return items;
        try {
            var result = await adapter.searchAsync({
                q: "",
                space_code: spaceCode,
                sort: "newest",
                page: 1,
                page_size: 4,
                filters: {}
            });
            var runtimeItems = result && result.items ? result.items : [];
            runtimeItems.forEach(function (item) {
                items.push({
                    href: buildDetailHref(spaceCode, item.id),
                    label: item.title || ("Tin mới trên " + getTitleBySpace(spaceCode)),
                    sub: [item.category || "", item.location || "", item.meta || ""].filter(Boolean).join(" • "),
                    meta: item.badge || "Mới"
                });
            });
        } catch (e) { }
        return items;
    }

    async function buildSellerNotifications(session) {
        var items = [buildSessionCard("gianhang", session)];
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
        if (!adapter || typeof adapter.getSnapshotAsync !== "function") return items;
        try {
            var snapshot = await adapter.getSnapshotAsync();
            var listings = snapshot && snapshot.listings ? snapshot.listings.slice(0, 2) : [];
            var conversations = snapshot && snapshot.conversations ? snapshot.conversations.slice(0, 2) : [];
            var orders = snapshot && snapshot.orders ? snapshot.orders.slice(0, 2) : [];
            var report = snapshot && snapshot.reportSummary ? snapshot.reportSummary : null;
            items.push({
                href: "/app-ui/gianhang/create-order.aspx?ui_mode=app",
                label: "Tạo đơn mới",
                sub: "Khởi tạo đơn bán ngay trong không gian gian hàng.",
                meta: "Tạo đơn"
            });
            items.push({
                href: "/app-ui/gianhang/pending-payment.aspx?ui_mode=app",
                label: "Chờ thanh toán",
                sub: "Đơn đang ở bước chờ xử lý thanh toán / trao đổi.",
                meta: report && report.pending_orders ? report.pending_orders : "0"
            });
            listings.forEach(function (item) {
                items.push({
                    href: buildDetailHref("gianhang", item.id),
                    label: item.title || "Tin gian hàng",
                    sub: [item.category || "", item.status || "", item.updatedAt || ""].filter(Boolean).join(" • "),
                    meta: item.stat || "Tin"
                });
            });
            conversations.forEach(function (item) {
                items.push({
                    href: "/app-ui/gianhang/conversations.aspx?ui_mode=app",
                    label: item.name || "Khách hàng mới",
                    sub: [item.topic || "", item.state || ""].filter(Boolean).join(" • "),
                    meta: item.time || "Lead"
                });
            });
            orders.forEach(function (item) {
                var pending = isPendingOrderStatus(item.status);
                items.push({
                    href: pending
                        ? "/app-ui/gianhang/pending-payment.aspx?ui_mode=app"
                        : "/app-ui/gianhang/orders.aspx?ui_mode=app",
                    label: "Đơn #" + item.id,
                    sub: [item.buyer || "", item.status || ""].filter(Boolean).join(" • "),
                    meta: item.amount || "Đơn"
                });
            });
        } catch (e) { }
        return items;
    }

    async function run() {
        var host = document.querySelector("[data-role='app-notification-feed']");
        if (!host) return;
        var session = getSession();
        var spaceCode = getSpaceCode();
        var items = spaceCode === "gianhang"
            ? await buildSellerNotifications(session)
            : await buildMarketplaceNotifications(spaceCode, session);
        renderCards(host, items);
    }

    run();
})();
