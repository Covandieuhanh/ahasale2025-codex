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

        if (lower.indexOf("/home/quan-ly-tin/tin-da-luu.aspx") === 0) {
            return "/app-ui/home/saved-manager.aspx?ui_mode=app";
        }
        if (lower.indexOf("/home/trao-doi.aspx") === 0) {
            return "/app-ui/home/exchange.aspx?ui_mode=app";
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

    function buildSavedManagerHref() {
        return decorateTargetHref("/app-ui/home/saved-manager.aspx?ui_mode=app");
    }

    function buildLoginHref() {
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + encodeURIComponent(getCurrentAppHref());
    }

    function getSession() {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        return adapter && typeof adapter.getSession === "function"
            ? adapter.getSession()
            : { auth_state: "guest" };
    }

    function resolveDetailHref(item) {
        if (!item) return "/app-ui/home/default.aspx?ui_mode=app";
        var source = (item.runtime_source || "").toLowerCase();
        var spaceCode = source === "batdongsan" || source === "choxe" || source === "dienthoai-maytinh"
            ? source
            : "home";
        if (window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing) {
            return window.AhaAppUiAdapters.listing.buildDetailHref(spaceCode, item.id);
        }
        return "/app-ui/" + spaceCode + "/detail.aspx?id=" + encodeURIComponent(item.id) + "&ui_mode=app";
    }

    function renderCard(item, index) {
        var href = resolveDetailHref(item);
        var mediaStyle = (item.image || "").indexOf("http") === 0 || (item.image || "").indexOf("/") === 0
            ? "background-image:url('" + item.image + "');background-size:cover;background-position:center;"
            : "background:" + (item.image || "linear-gradient(135deg,#cbd5e1,#94a3b8)");
        return '' +
            '<article class="app-listing-card">' +
            '  <a class="app-card-link" href="' + href + '">' +
            '    <div class="app-listing-media" style="' + mediaStyle + '">' +
            '      <div class="app-listing-media-meta"><span>Từ dữ liệu hệ thống</span><span>' + (index + 1) + ' ảnh</span></div>' +
            '    </div>' +
            '    <div class="app-listing-body">' +
            '      <div class="app-listing-category">' + (item.category || "Tin đăng") + '</div>' +
            '      <div class="app-listing-title">' + (item.title || "Tin đăng") + '</div>' +
            '      <div class="app-listing-price">' + (item.price || "Liên hệ") + '</div>' +
            '      <div class="app-listing-meta">' + (item.meta || "") + '</div>' +
            '      <div class="app-listing-location">' + (item.location || "") + '</div>' +
            '      <div class="app-listing-badge">' + (item.badge || "Mới") + '</div>' +
            '    </div>' +
            '  </a>' +
            '</article>';
    }

    function renderGuestState(host) {
        host.innerHTML = '' +
            '<section class="app-empty-state">' +
            '  <div class="app-empty-icon">i</div>' +
            '  <h2 class="app-empty-title">Đăng nhập để xem tin đã lưu</h2>' +
            '  <p class="app-empty-copy">Tính năng lưu tin dùng dữ liệu tài khoản thật trên nền tảng hiện tại.</p>' +
            '  <div class="app-empty-actions"><a class="app-ghost-btn" href="' + buildLoginHref() + '">Đăng nhập</a></div>' +
            '</section>';
    }

    function renderEmptyState(host) {
        host.innerHTML = '' +
            '<section class="app-empty-state">' +
            '  <div class="app-empty-icon">i</div>' +
            '  <h2 class="app-empty-title">Chưa có dữ liệu lưu khả dụng</h2>' +
            '  <p class="app-empty-copy">Bạn có thể mở màn quản lý tin đã lưu để theo dõi đầy đủ, hoặc tiếp tục tìm kiếm để cập nhật dữ liệu mới.</p>' +
            '  <div class="app-empty-actions"><a class="app-ghost-btn" href="' + buildSavedManagerHref() + '">Mở quản lý đã lưu</a></div>' +
            '</section>';
    }

    async function fetchRuntimeItems() {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.search;
        if (!adapter || typeof adapter.searchAsync !== "function") return [];
        var spaces = ["home", "batdongsan", "choxe", "dienthoai-maytinh"];
        var all = [];
        for (var i = 0; i < spaces.length; i += 1) {
            try {
                var result = await adapter.searchAsync({
                    q: "",
                    space_code: spaces[i],
                    sort: "newest",
                    page: 1,
                    page_size: 3,
                    filters: {}
                });
                if (result && result.items && result.items.length) {
                    all = all.concat(result.items);
                }
            } catch (e) { }
        }
        var seen = {};
        return all.filter(function (item) {
            if (!item || !item.id || seen[item.id]) return false;
            seen[item.id] = true;
            return true;
        }).slice(0, 12);
    }

    async function bootstrap() {
        var summary = document.querySelector('[data-role="saved-summary"]');
        var host = document.querySelector('[data-role="saved-feed"]');
        if (!host) return;
        var managerLink = document.querySelector('[data-role="saved-manager-link"]');
        if (managerLink) {
            managerLink.setAttribute("href", buildSavedManagerHref());
        }

        var session = getSession();
        if (session.auth_state !== "authenticated") {
            if (summary) summary.textContent = "Bạn chưa đăng nhập.";
            renderGuestState(host);
            return;
        }

        if (summary) {
            summary.textContent = "Danh sách xem nhanh trong app, đồng bộ từ dữ liệu của các không gian đang bật.";
        }

        var runtimeItems = await fetchRuntimeItems();
        if (!runtimeItems.length) {
            renderEmptyState(host);
            return;
        }

        if (summary) {
            summary.textContent = "Đang hiển thị " + runtimeItems.length + " tin mới nhất để theo dõi nhanh.";
        }
        host.innerHTML = runtimeItems.map(renderCard).join("");
    }

    bootstrap();
})();
