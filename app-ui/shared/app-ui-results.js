(function () {
    function resolveMediaStyle(image) {
        var value = (image || "").trim();
        if (!value) {
            return "background:linear-gradient(135deg,#dbeafe,#bfdbfe 40%,#f8fafc)";
        }
        if (/^(\/|https?:)/i.test(value)) {
            return "background-image:url('" + value.replace(/'/g, "%27") + "');background-size:cover;background-position:center;";
        }
        return "background:" + value;
    }

    function getQuery(name) {
        try {
            return new URLSearchParams(window.location.search || "").get(name) || "";
        } catch (e) {
            return "";
        }
    }

    function parsePage(raw) {
        var value = parseInt(raw, 10);
        if (!value || value < 1) return 1;
        return value;
    }

    function getCurrentPage() {
        return parsePage(getQuery("page"));
    }

    function getFilters(spaceCode) {
        var filters = {};
        var params;
        try {
            params = new URLSearchParams(window.location.search || "");
        } catch (e) {
            params = null;
        }
        if (!params) return filters;
        if (spaceCode === "batdongsan") {
            var propertyType = params.get("property_type");
            var transactionType = params.get("transaction_type");
            var areaMin = params.get("area_min");
            var priceBucket = params.get("price_bucket");
            var sort = params.get("sort");
            if (propertyType) filters.property_type = propertyType;
            if (transactionType) filters.transaction_type = transactionType;
            if (areaMin) filters.area_min = areaMin;
            if (priceBucket) filters.price_bucket = priceBucket;
            if (sort) filters.sort = sort;
        }
        return filters;
    }

    function inferSpace() {
        var pathname = window.location.pathname || "";
        if (pathname.indexOf("/batdongsan/") >= 0) return "batdongsan";
        if (pathname.indexOf("/choxe/") >= 0) return "choxe";
        if (pathname.indexOf("/dienthoai-maytinh/") >= 0) return "dienthoai-maytinh";
        return "home";
    }

    function getHomeRoute(spaceCode) {
        if (spaceCode === "batdongsan") return "/app-ui/batdongsan/default.aspx?ui_mode=app";
        if (spaceCode === "choxe") return "/app-ui/choxe/default.aspx?ui_mode=app";
        if (spaceCode === "dienthoai-maytinh") return "/app-ui/dienthoai-maytinh/default.aspx?ui_mode=app";
        return "/app-ui/home/default.aspx?ui_mode=app";
    }

    function getCategoryRoute(spaceCode) {
        if (spaceCode === "batdongsan") return "/app-ui/batdongsan/hub.aspx?ui_mode=app";
        if (spaceCode === "choxe") return "/app-ui/choxe/hub.aspx?ui_mode=app";
        if (spaceCode === "dienthoai-maytinh") return "/app-ui/dienthoai-maytinh/hub.aspx?ui_mode=app";
        return "/app-ui/home/categories.aspx?ui_mode=app";
    }

    function getPostRoute() {
        var session = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session
            ? window.AhaAppUiAdapters.session.getSession()
            : null;
        if (window.AhaAppUi && typeof window.AhaAppUi.getPostTarget === "function") {
            return window.AhaAppUi.getPostTarget(session).href;
        }
        return "/app-ui/gianhang/default.aspx?ui_mode=app";
    }

    function renderSearchActions(spaceCode) {
        var host = document.querySelector("[data-role='search-actions']");
        if (!host) return;
        host.innerHTML = '' +
            '<a class="app-action-pill" href="' + getHomeRoute(spaceCode) + '">Về trang chính</a>' +
            '<a class="app-action-pill" href="' + getCategoryRoute(spaceCode) + '">Xem danh mục</a>' +
            '<a class="app-action-pill is-primary" href="' + getPostRoute() + '">Đăng tin</a>';
    }

    function renderBdsFilterLinks(query, currentFilters, items, key) {
        return items.map(function (item) {
            var href = '/app-ui/batdongsan/search.aspx?ui_mode=app';
            if (query) href += '&q=' + encodeURIComponent(query);
            var merged = {
                transaction_type: currentFilters.transaction_type || "",
                property_type: currentFilters.property_type || "",
                area_min: currentFilters.area_min || "",
                price_bucket: currentFilters.price_bucket || "",
                sort: currentFilters.sort || ""
            };
            if (item.value) {
                merged[key] = item.value;
            } else {
                merged[key] = "";
            }
            Object.keys(merged).forEach(function (name) {
                if (merged[name]) href += '&' + name + '=' + encodeURIComponent(merged[name]);
            });
            return '<a class="app-chip app-filter-chip' + (item.active ? ' is-active' : '') + '" href="' + href + '">' + item.label + '</a>';
        }).join("");
    }

    function renderBdsSortToolbar(query, filters) {
        var host = document.querySelector("[data-role='bds-sort-toolbar']");
        if (!host) return;
        var items = [
            { label: "Mới nhất", value: "newest" },
            { label: "Giá tăng dần", value: "price_asc" },
            { label: "Giá giảm dần", value: "price_desc" },
            { label: "Diện tích lớn", value: "area_desc" }
        ];
        host.innerHTML = items.map(function (item) {
            var href = '/app-ui/batdongsan/search.aspx?ui_mode=app';
            if (query) href += '&q=' + encodeURIComponent(query);
            [
                ["transaction_type", filters.transaction_type],
                ["property_type", filters.property_type],
                ["area_min", filters.area_min],
                ["price_bucket", filters.price_bucket],
                ["sort", item.value]
            ].forEach(function (pair) {
                if (pair[1]) href += '&' + pair[0] + '=' + encodeURIComponent(pair[1]);
            });
            return '<a class="app-sort-pill' + ((filters.sort || "newest") === item.value ? ' is-active' : '') + '" href="' + href + '">' + item.label + '</a>';
        }).join("");
    }

    function renderBdsFilters(query, filters) {
        var host = document.querySelector("[data-role='bds-result-filters']");
        if (!host) return;
        var transactionItems = [
            { label: "Mới nhất", active: !filters.transaction_type, value: "" },
            { label: "Mua bán", active: (filters.transaction_type || "") === "mua_ban", value: "mua_ban" },
            { label: "Cho thuê", active: (filters.transaction_type || "") === "cho_thue", value: "cho_thue" }
        ];
        var propertyItems = [
            { label: "Tất cả loại", active: !filters.property_type, value: "" },
            { label: "Căn hộ", active: (filters.property_type || "") === "can ho", value: "can ho" },
            { label: "Nhà phố", active: (filters.property_type || "") === "nha pho", value: "nha pho" },
            { label: "Đất nền", active: (filters.property_type || "") === "dat nen", value: "dat nen" }
        ];
        var priceItems = [
            { label: "Tất cả giá", active: !filters.price_bucket, value: "" },
            { label: "Dưới 3 tỷ", active: (filters.price_bucket || "") === "duoi_3_ty", value: "duoi_3_ty" },
            { label: "Trên 3 tỷ", active: (filters.price_bucket || "") === "tren_3_ty", value: "tren_3_ty" }
        ];
        var areaItems = [
            { label: "Mọi diện tích", active: !filters.area_min, value: "" },
            { label: "Từ 70 m²", active: (filters.area_min || "") === "70", value: "70" },
            { label: "Từ 90 m²", active: (filters.area_min || "") === "90", value: "90" }
        ];
        host.innerHTML = '' +
            '<div class="app-filter-groups">' +
            '  <div class="app-filter-group"><span class="app-filter-label">Giao dịch</span><div class="app-chip-lane">' + renderBdsFilterLinks(query, filters, transactionItems, "transaction_type") + '</div></div>' +
            '  <div class="app-filter-group"><span class="app-filter-label">Loại hình</span><div class="app-chip-lane">' + renderBdsFilterLinks(query, filters, propertyItems, "property_type") + '</div></div>' +
            '  <div class="app-filter-group"><span class="app-filter-label">Giá</span><div class="app-chip-lane">' + renderBdsFilterLinks(query, filters, priceItems, "price_bucket") + '</div></div>' +
            '  <div class="app-filter-group"><span class="app-filter-label">Diện tích</span><div class="app-chip-lane">' + renderBdsFilterLinks(query, filters, areaItems, "area_min") + '</div></div>' +
            '</div>';
    }

    function renderBdsInsights(result, filters) {
        var host = document.querySelector("[data-role='bds-search-insights']");
        if (!host) return;
        var activeFilterCount = 0;
        ["transaction_type", "property_type", "area_min", "price_bucket"].forEach(function (key) {
            if (filters[key]) activeFilterCount += 1;
        });
        var sortLabel = (function () {
            switch (filters.sort || "newest") {
                case "price_asc": return "Giá tăng";
                case "price_desc": return "Giá giảm";
                case "area_desc": return "Diện tích";
                default: return "Mới nhất";
            }
        })();
        host.innerHTML = '' +
            '<article class="app-results-insight-card">' +
            '  <span class="app-results-insight-value">' + result.total_items + '</span>' +
            '  <span class="app-results-insight-label">Tin phù hợp</span>' +
            '</article>' +
            '<article class="app-results-insight-card">' +
            '  <span class="app-results-insight-value">' + activeFilterCount + '</span>' +
            '  <span class="app-results-insight-label">Bộ lọc đang bật</span>' +
            '</article>' +
            '<article class="app-results-insight-card">' +
            '  <span class="app-results-insight-value">' + sortLabel + '</span>' +
            '  <span class="app-results-insight-label">Thứ tự hiển thị</span>' +
            '</article>';
    }

    function renderCard(item, spaceCode) {
        var href = window.AhaAppUiAdapters.listing.buildDetailHref(spaceCode, item.id);
        return '' +
            '<article class="app-listing-card">' +
            '  <a class="app-card-link" href="' + href + '">' +
            '    <div class="app-listing-media" style="' + resolveMediaStyle(item.image) + '">' +
            '      <div class="app-listing-media-meta"><span>Mới cập nhật</span><span>Chi tiết</span></div>' +
            '    </div>' +
            '    <div class="app-listing-body">' +
            '      <div class="app-listing-category">' + item.category + '</div>' +
            '      <div class="app-listing-title">' + item.title + '</div>' +
            '      <div class="app-listing-price">' + item.price + '</div>' +
            '      <div class="app-listing-meta">' + item.meta + '</div>' +
            '      <div class="app-listing-location">' + item.location + '</div>' +
            '      <div class="app-listing-badge">' + item.badge + '</div>' +
            '    </div>' +
            '  </a>' +
            '</article>';
    }

    function buildPageHref(page) {
        var safePage = parsePage(page);
        try {
            var url = new URL(window.location.href);
            url.searchParams.delete("back_href");
            url.searchParams.delete("return_url");
            url.searchParams.set("page", String(safePage));
            return url.pathname + "?" + url.searchParams.toString();
        } catch (e) {
            var params;
            try {
                params = new URLSearchParams(window.location.search || "");
                params.delete("back_href");
                params.delete("return_url");
                params.set("page", String(safePage));
                return window.location.pathname + "?" + params.toString();
            } catch (inner) {
                return window.location.pathname + "?page=" + safePage;
            }
        }
    }

    function buildPageItems(currentPage, totalPages) {
        var safeCurrent = parsePage(currentPage);
        var safeTotal = parsePage(totalPages);
        if (safeCurrent > safeTotal) safeCurrent = safeTotal;

        if (safeTotal <= 7) {
            var full = [];
            for (var p = 1; p <= safeTotal; p += 1) full.push(p);
            return full;
        }

        var pages = [1];
        var start = Math.max(2, safeCurrent - 1);
        var end = Math.min(safeTotal - 1, safeCurrent + 1);

        if (start > 2) pages.push("ellipsis-left");
        for (var page = start; page <= end; page += 1) pages.push(page);
        if (end < safeTotal - 1) pages.push("ellipsis-right");
        pages.push(safeTotal);
        return pages;
    }

    function getResultHost(spaceCode) {
        var resultHostMap = {
            batdongsan: "[data-role='bds-search-results']",
            home: "[data-role='home-search-results']",
            choxe: "[data-role='choxe-search-results']",
            "dienthoai-maytinh": "[data-role='tech-search-results']"
        };
        return document.querySelector(resultHostMap[spaceCode] || resultHostMap.home);
    }

    function renderPagination(spaceCode, currentPage, totalPages, totalItems) {
        var host = getResultHost(spaceCode);
        if (!host || !host.parentNode) return;

        var paginationHost = host.parentNode.querySelector("[data-role='search-pagination']");
        if (!paginationHost) {
            paginationHost = document.createElement("div");
            paginationHost.setAttribute("data-role", "search-pagination");
            host.insertAdjacentElement("afterend", paginationHost);
        }

        var safeCurrent = parsePage(currentPage);
        var safeTotal = parsePage(totalPages);
        var safeItems = parseInt(totalItems, 10) || 0;

        if (safeItems <= 0) {
            paginationHost.innerHTML = "";
            paginationHost.className = "app-results-pagination-wrap is-hidden";
            return;
        }

        if (safeCurrent > safeTotal) safeCurrent = safeTotal;
        var pageItems = buildPageItems(safeCurrent, safeTotal);
        var prevHref = buildPageHref(Math.max(1, safeCurrent - 1));
        var nextHref = buildPageHref(Math.min(safeTotal, safeCurrent + 1));

        paginationHost.className = "app-results-pagination-wrap";
        paginationHost.innerHTML = '' +
            '<div class="app-results-pagination-head">Trang ' + safeCurrent + '/' + safeTotal + ' • ' + safeItems + ' tin</div>' +
            '<nav class="app-results-pagination" aria-label="Phân trang kết quả">' +
            (safeCurrent > 1
                ? '<a class="app-page-number app-page-nav" href="' + prevHref + '">Trước</a>'
                : '<span class="app-page-number app-page-nav is-disabled">Trước</span>') +
            pageItems.map(function (item) {
                if (typeof item !== "number") {
                    return '<span class="app-page-ellipsis">...</span>';
                }
                if (item === safeCurrent) {
                    return '<span class="app-page-number is-active">' + item + '</span>';
                }
                return '<a class="app-page-number" href="' + buildPageHref(item) + '">' + item + '</a>';
            }).join("") +
            (safeCurrent < safeTotal
                ? '<a class="app-page-number app-page-nav" href="' + nextHref + '">Sau</a>'
                : '<span class="app-page-number app-page-nav is-disabled">Sau</span>') +
            '</nav>';
    }

    function renderEmptyResults(spaceCode, query, result) {
        var hostMap = {
            batdongsan: "[data-role='bds-search-results']",
            home: "[data-role='home-search-results']",
            choxe: "[data-role='choxe-search-results']",
            "dienthoai-maytinh": "[data-role='tech-search-results']"
        };
        var host = document.querySelector(hostMap[spaceCode] || hostMap.home);
        if (!host) return;
        var baseSpace = spaceCode === "batdongsan" ? "batdongsan"
            : spaceCode === "choxe" ? "choxe"
            : spaceCode === "dienthoai-maytinh" ? "dienthoai-maytinh"
            : "home";
        var reason = result && result.summary_text
            ? result.summary_text
            : ('Không tìm thấy nội dung cho từ khóa "' + (query || "tất cả") + '". Hãy thử đổi từ khóa hoặc bỏ bớt bộ lọc.');
        host.innerHTML = '' +
            '<section class="app-empty-state">' +
            '  <div class="app-empty-icon">?</div>' +
            '  <h2 class="app-empty-title">Chưa có kết quả phù hợp</h2>' +
            '  <p class="app-empty-copy">' + reason + '</p>' +
            '  <div class="app-empty-actions">' +
            '    <a class="app-ghost-btn" href="/app-ui/' + baseSpace + '/default.aspx?ui_mode=app">Về màn chính</a>' +
            '    <a class="app-ghost-btn" href="/app-ui/' + baseSpace + '/search.aspx?ui_mode=app">Làm mới tìm kiếm</a>' +
            '  </div>' +
            '</section>';
    }

    async function run() {
        var spaceCode = inferSpace();
        var query = getQuery("q");
        var activeFilters = getFilters(spaceCode);
        var currentPage = getCurrentPage();
        var result = await window.AhaAppUiAdapters.search.searchAsync({
            q: query,
            space_code: spaceCode,
            sort: activeFilters.sort || "newest",
            page: currentPage,
            page_size: 20,
            filters: activeFilters
        });

        var title = document.querySelector("[data-role='search-query-label']");
        if (title) {
            var titleMap = {
                batdongsan: "Tìm bất động sản...",
                choxe: "Tìm ô tô, xe máy...",
                "dienthoai-maytinh": "Tìm điện thoại, laptop...",
                home: "Tìm sản phẩm..."
            };
            title.textContent = query || titleMap[spaceCode] || titleMap.home;
        }

        var summary = document.querySelector("[data-role='search-summary']");
        if (summary) {
            var filterNote = activeFilters.transaction_type || activeFilters.property_type
                || activeFilters.area_min || activeFilters.price_bucket
                ? ' với bộ lọc đang áp dụng.'
                : '.';
            summary.textContent = result.summary_text
                ? result.summary_text
                : ('Tìm thấy ' + result.total_items + ' kết quả cho từ khóa "' + (query || "tất cả") + '"' + filterNote);
        }
        if (result.page_title) {
            document.title = result.page_title + " - App UI";
        }
        renderSearchActions(spaceCode);

        var host = getResultHost(spaceCode);
        if (host) {
            if (!result.items.length) {
                renderEmptyResults(spaceCode, query || "", result);
            } else {
                host.innerHTML = result.items.map(function (item) {
                    return renderCard(item, spaceCode);
                }).join("");
            }
        }
        renderPagination(spaceCode, result.page || currentPage, result.total_pages || 1, result.total_items || 0);

        if (spaceCode === "batdongsan") {
            renderBdsInsights(result, activeFilters);
            renderBdsSortToolbar(query || "", activeFilters);
            renderBdsFilters(query || "", activeFilters);
        }

        var searchForms = document.querySelectorAll("[data-app-search-form]");
        searchForms.forEach(function (form) {
            var input = form.querySelector("[data-app-search-input]");
            if (input) input.value = query || "";
        });
    }

    run();
})();
