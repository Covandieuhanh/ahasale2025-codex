(function () {
    var RUNTIME_CACHE_KEY = "aha_app_ui_runtime_listing_cache_v1";
    var APP_UI_PREFIX = "/app-ui/";

    function readRuntimeCache() {
        try {
            var raw = sessionStorage.getItem(RUNTIME_CACHE_KEY);
            return raw ? JSON.parse(raw) : {};
        } catch (e) {
            return {};
        }
    }

    function writeRuntimeCache(cache) {
        try {
            sessionStorage.setItem(RUNTIME_CACHE_KEY, JSON.stringify(cache || {}));
        } catch (e) { }
    }

    function rememberItems(items) {
        if (!items || !items.length) return;
        var cache = readRuntimeCache();
        items.forEach(function (item) {
            if (!item || !item.id) return;
            cache[item.id] = item;
        });
        writeRuntimeCache(cache);
    }

    function getAll() {
        var homeItems = (window.AhaAppUiData && window.AhaAppUiData.home && window.AhaAppUiData.home.listings) || [];
        var bdsItems = (window.AhaAppUiData && window.AhaAppUiData.batdongsan && window.AhaAppUiData.batdongsan.listings) || [];
        var xeItems = (window.AhaAppUiData && window.AhaAppUiData.choxe && window.AhaAppUiData.choxe.listings) || [];
        var techItems = (window.AhaAppUiData && window.AhaAppUiData.dienthoaiMayTinh && window.AhaAppUiData.dienthoaiMayTinh.listings) || [];
        var gianHangItems = (window.AhaAppUiData && window.AhaAppUiData.gianhang && window.AhaAppUiData.gianhang.listings) || [];
        return homeItems.concat(bdsItems).concat(xeItems).concat(techItems).concat(gianHangItems);
    }

    function getById(id) {
        var items = getAll();
        for (var i = 0; i < items.length; i += 1) {
            if (items[i].id === id) return items[i];
        }
        var runtimeCache = readRuntimeCache();
        if (runtimeCache[id]) return runtimeCache[id];
        return null;
    }

    function getByIdAsync(id, spaceCode) {
        var existing = getById(id);
        if (existing) return Promise.resolve(existing);
        if (!id || !/^runtime-/.test(id || "")) return Promise.resolve(null);

        var url = "/app-ui/shared/runtime-detail.ashx?space=" + encodeURIComponent(spaceCode || "home") + "&id=" + encodeURIComponent(id);
        return fetch(url, { credentials: "same-origin" })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (payload) {
                if (!payload || !payload.ok || !payload.item) return null;
                rememberItems([payload.item]);
                return payload.item;
            })
            .catch(function () { return null; });
    }

    function getCurrentAppHref() {
        return sanitizeAppHref((window.location.pathname || "/") + (window.location.search || "") + (window.location.hash || ""));
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

    function isAppHref(value) {
        return !!value && value.indexOf(APP_UI_PREFIX) === 0;
    }

    function buildDetailHref(spaceCode, id, options) {
        var href = "/app-ui/" + spaceCode + "/detail.aspx?id=" + encodeURIComponent(id) + "&ui_mode=app";
        var backHref = options && options.backHref;
        if (!backHref && isAppHref(window.location.pathname || "")) {
            backHref = getCurrentAppHref();
        }
        backHref = sanitizeAppHref(backHref);
        if (isAppHref(backHref)) {
            href = appendQuery(href, "back_href", backHref);
        }
        return href;
    }

    window.AhaAppUiAdapters = window.AhaAppUiAdapters || {};
    window.AhaAppUiAdapters.listing = {
        getById: getById,
        getByIdAsync: getByIdAsync,
        rememberItems: rememberItems,
        buildDetailHref: buildDetailHref
    };
})();
