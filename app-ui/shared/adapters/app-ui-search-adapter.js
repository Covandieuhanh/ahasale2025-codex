(function () {
    function normalize(text) {
        return (text || "")
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .replace(/đ/g, "d")
            .trim();
    }

    function matches(item, q) {
        if (!q) return true;
        var needle = normalize(q);
        var haystack = normalize([
            item.title,
            item.category,
            item.location,
            item.meta,
            item.summary
        ].join(" "));
        return haystack.indexOf(needle) >= 0;
    }

    function parseArea(meta) {
        var match = (meta || "").match(/([0-9]+)\s*(?:m2|m²)/i);
        return match ? parseInt(match[1], 10) : 0;
    }

    function parsePrice(item) {
        var text = normalize(item.price);
        var value = parseFloat((text.match(/([0-9]+(?:\.[0-9]+)?)/) || [0, 0])[1]);
        if (text.indexOf("ty") >= 0) return value * 1000;
        if (text.indexOf("trieu") >= 0) return value;
        return value;
    }

    function matchesBdsFilters(item, filters) {
        if (!filters) return true;
        if (filters.property_type && normalize(item.category).indexOf(normalize(filters.property_type)) < 0) return false;
        if (filters.transaction_type === "cho_thue" && normalize(item.badge).indexOf("cho thue") < 0) return false;
        if (filters.transaction_type === "mua_ban" && normalize(item.badge).indexOf("cho thue") >= 0) return false;
        if (filters.area_min && parseArea(item.meta) < parseInt(filters.area_min, 10)) return false;
        if (filters.price_bucket === "duoi_3_ty" && parsePrice(item) >= 3000) return false;
        if (filters.price_bucket === "tren_3_ty" && parsePrice(item) < 3000) return false;
        return true;
    }

    function sortItems(items, sort) {
        var sorted = items.slice();
        if (sort === "price_asc") {
            sorted.sort(function (a, b) { return parsePrice(a) - parsePrice(b); });
        } else if (sort === "price_desc") {
            sorted.sort(function (a, b) { return parsePrice(b) - parsePrice(a); });
        } else if (sort === "area_desc") {
            sorted.sort(function (a, b) { return parseArea(b.meta) - parseArea(a.meta); });
        }
        return sorted;
    }

    function filterBySpace(spaceCode) {
        if (spaceCode === "batdongsan") {
            return (window.AhaAppUiData && window.AhaAppUiData.batdongsan && window.AhaAppUiData.batdongsan.listings) || [];
        }
        if (spaceCode === "choxe") {
            return (window.AhaAppUiData && window.AhaAppUiData.choxe && window.AhaAppUiData.choxe.listings) || [];
        }
        if (spaceCode === "dienthoai-maytinh") {
            return (window.AhaAppUiData && window.AhaAppUiData.dienthoaiMayTinh && window.AhaAppUiData.dienthoaiMayTinh.listings) || [];
        }
        if (spaceCode === "gianhang") {
            return (window.AhaAppUiData && window.AhaAppUiData.gianhang && window.AhaAppUiData.gianhang.listings) || [];
        }
        return (window.AhaAppUiData && window.AhaAppUiData.home && window.AhaAppUiData.home.listings) || [];
    }

    function searchFallback(payload) {
        payload = payload || {};
        var spaceCode = payload.space_code || "home";
        var items = filterBySpace(spaceCode).filter(function (item) {
            var qMatch = matches(item, payload.q);
            var filterMatch = spaceCode === "batdongsan" ? matchesBdsFilters(item, payload.filters || {}) : true;
            return qMatch && filterMatch;
        });
        items = sortItems(items, payload.sort || "newest");
        return {
            items: items,
            page: 1,
            page_size: items.length,
            total_items: items.length,
            total_pages: 1,
            applied_filters: payload.filters || {},
            sort: payload.sort || "newest",
            source: "demo"
        };
    }

    function buildEmptyRuntimeResult(payload, source, summaryText) {
        payload = payload || {};
        return {
            items: [],
            page: payload.page || 1,
            page_size: payload.page_size || 20,
            total_items: 0,
            total_pages: 0,
            applied_filters: payload.filters || {},
            sort: payload.sort || "newest",
            summary_text: summaryText || "",
            page_title: "",
            source: source || "runtime"
        };
    }

    function canUseRuntime(spaceCode) {
        return spaceCode === "home"
            || spaceCode === "batdongsan"
            || spaceCode === "choxe"
            || spaceCode === "dienthoai-maytinh";
    }

    function buildRuntimeUrl(payload) {
        var params = new URLSearchParams();
        params.set("space", payload.space_code || "home");
        params.set("q", payload.q || "");
        params.set("page", String(payload.page || 1));
        params.set("page_size", String(payload.page_size || 20));
        if (payload.sort) params.set("sort", payload.sort);
        var filters = payload.filters || {};
        Object.keys(filters).forEach(function (key) {
            if (filters[key]) params.set(key, filters[key]);
        });
        return "/app-ui/shared/runtime-search.ashx?" + params.toString();
    }

    function searchAsync(payload) {
        payload = payload || {};
        var spaceCode = payload.space_code || "home";
        var fallback = searchFallback(payload);

        if (!canUseRuntime(spaceCode) || !window.fetch) {
            return Promise.resolve(fallback);
        }

        return fetch(buildRuntimeUrl(payload), { credentials: "same-origin" })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (runtime) {
                if (!runtime) {
                    return buildEmptyRuntimeResult(payload, "runtime-unavailable", "Không thể tải dữ liệu lúc này.");
                }
                if (!runtime.ok && runtime.reason === "unauthorized") {
                    return buildEmptyRuntimeResult(payload, "runtime-unauthorized", "Bạn cần đăng nhập để xem kết quả đầy đủ.");
                }
                if (!runtime.ok || !Array.isArray(runtime.items)) {
                    return buildEmptyRuntimeResult(payload, "runtime-unavailable", "Không thể tải dữ liệu lúc này.");
                }
                if (window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing && runtime.items.length) {
                    window.AhaAppUiAdapters.listing.rememberItems(runtime.items);
                }
                return {
                    items: runtime.items,
                    page: runtime.page || 1,
                    page_size: runtime.page_size || runtime.items.length,
                    total_items: runtime.total_items || runtime.items.length,
                    total_pages: runtime.total_pages || 1,
                    applied_filters: payload.filters || {},
                    sort: payload.sort || "newest",
                    summary_text: runtime.summary_text || "",
                    page_title: runtime.page_title || "",
                    source: runtime.source || "runtime"
                };
            })
            .catch(function () {
                return buildEmptyRuntimeResult(payload, "runtime-error", "Lỗi kết nối dữ liệu, vui lòng thử lại.");
            });
    }

    function search(payload) {
        return searchFallback(payload);
    }

    window.AhaAppUiAdapters = window.AhaAppUiAdapters || {};
    window.AhaAppUiAdapters.search = {
        search: search,
        searchAsync: searchAsync
    };
})();
