(function () {
    function getCurrentAppHref() {
        return (window.location.pathname || "/") + (window.location.search || "");
    }

    function getQueryParamFromHref(url, key) {
        if (!url || !key) return "";
        var queryIndex = url.indexOf("?");
        if (queryIndex < 0) return "";
        var hashIndex = url.indexOf("#", queryIndex);
        var query = hashIndex >= 0 ? url.slice(queryIndex + 1, hashIndex) : url.slice(queryIndex + 1);
        try {
            return new URLSearchParams(query).get(key) || "";
        } catch (e) {
            return "";
        }
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

    function mapLegacyHref(url) {
        var value = (url || "").trim();
        if (!value || value.indexOf("/app-ui/") === 0) return value;
        var lower = value.toLowerCase();

        if (lower.indexOf("/home/trao-doi.aspx") === 0) {
            return "/app-ui/home/exchange.aspx?ui_mode=app";
        }
        if (lower.indexOf("/home/quan-ly-tin/tin-da-luu.aspx") === 0) {
            return "/app-ui/home/saved-manager.aspx?ui_mode=app";
        }
        if (lower.indexOf("/bat-dong-san/mua-ban.aspx") === 0) {
            return "/app-ui/batdongsan/mua-ban.aspx?ui_mode=app";
        }
        if (lower.indexOf("/bat-dong-san/cho-thue.aspx") === 0) {
            return "/app-ui/batdongsan/cho-thue.aspx?ui_mode=app";
        }
        if (lower.indexOf("/bat-dong-san/") === 0) {
            return "/app-ui/batdongsan/search.aspx?ui_mode=app";
        }
        if (lower.indexOf("/xe/mua-ban") === 0) {
            var vehicle = getQueryParamFromHref(value, "vehicle").toLowerCase();
            if (vehicle === "oto") return "/app-ui/choxe/o-to.aspx?ui_mode=app";
            if (vehicle === "xe-may") return "/app-ui/choxe/xe-may.aspx?ui_mode=app";
            return "/app-ui/choxe/search.aspx?ui_mode=app";
        }
        if (lower.indexOf("/xe/") === 0) {
            return "/app-ui/choxe/search.aspx?ui_mode=app";
        }
        if (lower.indexOf("/home/tim-kiem.aspx") === 0) {
            var q = getQueryParamFromHref(value, "q");
            var searchHref = "/app-ui/home/search.aspx?ui_mode=app";
            return q ? appendQuery(searchHref, "q", q) : searchHref;
        }
        if (lower.indexOf("/home/") === 0) {
            return "/app-ui/home/search.aspx?ui_mode=app";
        }
        return value;
    }

    function decorateTargetHref(url) {
        if (!url) return "";
        var mapped = mapLegacyHref(url);
        if (mapped.indexOf("/app-ui/") === 0) {
            if (mapped.indexOf("/app-ui/auth/") === 0) {
                return appendQuery(mapped, "return_url", getCurrentAppHref());
            }
            return appendQuery(mapped, "back_href", getCurrentAppHref());
        }
        if (mapped.indexOf("/home/") === 0
            || mapped.indexOf("/gianhang/") === 0
            || mapped.indexOf("/bat-dong-san/") === 0
            || mapped.indexOf("/xe/") === 0
            || mapped.indexOf("/shop/") === 0) {
            return appendQuery(mapped, "return_url", getCurrentAppHref());
        }
        return mapped;
    }

    function buildBuyerInboxHref() {
        return decorateTargetHref("/app-ui/home/exchange.aspx?ui_mode=app");
    }

    function buildSellerInboxHref() {
        return decorateTargetHref("/app-ui/gianhang/conversations.aspx?ui_mode=app");
    }

    function buildLoginHref() {
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + encodeURIComponent(getCurrentAppHref());
    }

    function getSession() {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        return adapter && typeof adapter.getSession === "function"
            ? adapter.getSession()
            : { auth_state: "guest", seller_status: "not_registered" };
    }

    function renderGuestState(host) {
        host.innerHTML = '' +
            '<section class="app-empty-state">' +
            '  <div class="app-empty-icon">i</div>' +
            '  <h2 class="app-empty-title">Đăng nhập để xem trao đổi</h2>' +
            '  <p class="app-empty-copy">Hộp trao đổi dùng dữ liệu tài khoản thật trong hệ thống hiện tại.</p>' +
            '  <div class="app-empty-actions"><a class="app-ghost-btn" href="' + buildLoginHref() + '">Đăng nhập</a></div>' +
            '</section>';
    }

    function renderNoConversationState(host, sellerStatus) {
        var actionHref = sellerStatus === "approved"
            ? buildSellerInboxHref()
            : buildBuyerInboxHref();
        var actionLabel = sellerStatus === "approved"
            ? "Mở khách hàng gian hàng"
            : "Mở lịch sử trao đổi";
        host.innerHTML = '' +
            '<section class="app-empty-state">' +
            '  <div class="app-empty-icon">i</div>' +
            '  <h2 class="app-empty-title">Chưa có trao đổi khả dụng</h2>' +
            '  <p class="app-empty-copy">Khi phát sinh trao đổi trên nền tảng thật, danh sách sẽ tự đồng bộ về màn app này.</p>' +
            '  <div class="app-empty-actions"><a class="app-ghost-btn" href="' + actionHref + '">' + actionLabel + '</a></div>' +
            '</section>';
    }

    function renderThread(item, defaultHref) {
        var href = decorateTargetHref(item.href || defaultHref);
        return '' +
            '<a class="app-home-thread-card" href="' + href + '">' +
            '  <div class="app-home-thread-row">' +
            '    <div>' +
            '      <span class="app-home-thread-kicker">' + (item.seller || "Khách hàng") + '</span>' +
            '      <h3 class="app-home-thread-title">' + (item.title || "Trao đổi gần đây") + '</h3>' +
            '    </div>' +
            '    <span class="app-home-thread-time">' + (item.time || "") + '</span>' +
            '  </div>' +
            '  <p class="app-home-thread-message">' + (item.message || "") + '</p>' +
            '  <span class="app-home-thread-action">' + (item.action || "Mở chi tiết") + '</span>' +
            '</a>';
    }

    async function fetchRuntimeConversations(session) {
        if (!session || session.seller_status !== "approved") return [];
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
        if (!adapter || typeof adapter.getSnapshotAsync !== "function") return [];
        try {
            var snapshot = await adapter.getSnapshotAsync();
            return snapshot && snapshot.conversations ? snapshot.conversations.slice(0, 8) : [];
        } catch (e) {
            return [];
        }
    }

    async function bootstrap() {
        var summary = document.querySelector('[data-role="inbox-summary"]');
        var host = document.querySelector('[data-role="inbox-list"]');
        if (!host) return;

        var session = getSession();
        if (session.auth_state !== "authenticated") {
            if (summary) summary.textContent = "Bạn chưa đăng nhập.";
            renderGuestState(host);
            return;
        }

        var conversations = await fetchRuntimeConversations(session);
        if (!conversations.length) {
            if (summary) summary.textContent = "Hiện chưa có hội thoại phù hợp.";
            renderNoConversationState(host, session.seller_status || "not_registered");
            return;
        }

        if (summary) {
            summary.textContent = "Bạn có " + conversations.length + " trao đổi đang hoạt động.";
        }
        var defaultHref = (session.seller_status || "not_registered") === "approved"
            ? buildSellerInboxHref()
            : buildBuyerInboxHref();
        host.innerHTML = conversations.map(function (item) {
            return renderThread(item, defaultHref);
        }).join("");
    }

    bootstrap();
})();
