(function () {
    var dataRoot = {
        quickFilters: ["Căn hộ", "Nhà phố", "Đất nền", "Cho thuê"],
        quickActions: [
            { label: "Mua bán", href: "/app-ui/batdongsan/mua-ban.aspx?ui_mode=app" },
            { label: "Cho thuê", href: "/app-ui/batdongsan/cho-thue.aspx?ui_mode=app" },
            { label: "Đăng tin", href: "/app-ui/auth/open-shop.aspx?ui_mode=app", action: "post" }
        ],
        propertyTypes: [
            { label: "Mua bán", stat: "Nhà phố, căn hộ, đất nền" },
            { label: "Cho thuê", stat: "Căn hộ, phòng trọ, văn phòng" },
            { label: "Dự án", stat: "Nguồn dự án mới và tiến độ" },
            { label: "Tham khảo giá", stat: "So sánh nhanh mặt bằng giá" },
            { label: "Vay mua nhà", stat: "Nội dung tài chính nhà ở" },
            { label: "Kinh nghiệm", stat: "Góc chia sẻ người mua và thuê" }
        ],
        highlights: [
            { area: "Bình Thạnh", summary: "Nguồn tin đang về căn hộ và nhà phố", accent: "navy" },
            { area: "Quận 9", summary: "Nhóm tìm đất nền và căn hộ tăng nhanh", accent: "sky" },
            { area: "Thủ Đức", summary: "Phù hợp khám phá dự án và cho thuê", accent: "slate" }
        ],
        feedTabs: ["Dành cho bạn", "Gần bạn", "Mới nhất"],
        listings: []
    };

    function mediaStyle(image, fallback) {
        var value = image || "";
        if (value.indexOf("http") === 0 || value.indexOf("/") === 0) {
            return "background-image:url('" + value + "');background-size:cover;background-position:center;";
        }
        return "background:" + (value || fallback);
    }

    function renderChipLane(items) {
        var host = document.querySelector('[data-role="bds-quick-filters"]');
        if (!host || !items || !items.length) return;
        host.innerHTML = items.map(function (item) {
            var href = '/app-ui/batdongsan/search.aspx?ui_mode=app&q=' + encodeURIComponent(item);
            return '<a class="app-chip" href="' + href + '">' + item + '</a>';
        }).join("");
    }

    function resolvePostHref(fallbackHref) {
        var sessionAdapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        if (!sessionAdapter || !window.AhaAppUi || typeof window.AhaAppUi.getPostTarget !== "function") {
            return fallbackHref;
        }
        var target = window.AhaAppUi.getPostTarget(sessionAdapter.getSession());
        return target && target.href ? target.href : fallbackHref;
    }

    function renderQuickActions(items) {
        var host = document.querySelector('[data-role="bds-quick-actions"]');
        if (!host || !items || !items.length) return;
        host.innerHTML = items.map(function (item) {
            var href = item.action === "post" ? resolvePostHref(item.href) : item.href;
            return '<a class="app-space-quick-action" href="' + href + '">' + item.label + '</a>';
        }).join("");
    }

    function renderTypes(items) {
        var host = document.querySelector('[data-role="bds-property-types"]');
        if (!host) return;
        var routeMap = {
            "Mua bán": "/app-ui/batdongsan/mua-ban.aspx?ui_mode=app",
            "Cho thuê": "/app-ui/batdongsan/cho-thue.aspx?ui_mode=app",
            "Dự án": "/app-ui/batdongsan/du-an.aspx?ui_mode=app",
            "Tham khảo giá": "/app-ui/batdongsan/tham-khao-gia.aspx?ui_mode=app",
            "Vay mua nhà": "/app-ui/batdongsan/vay-mua-nha.aspx?ui_mode=app",
            "Kinh nghiệm": "/app-ui/batdongsan/kinh-nghiem.aspx?ui_mode=app"
        };
        host.innerHTML = items.map(function (item) {
            var href = routeMap[item.label] || ('/app-ui/batdongsan/hub.aspx?ui_mode=app&q=' + encodeURIComponent(item.label));
            return '' +
                '<a class="app-bds-type-card app-card-link" href="' + href + '">' +
                '  <span class="app-bds-type-label">' + item.label + '</span>' +
                '  <span class="app-bds-type-stat">' + item.stat + '</span>' +
                '</a>';
        }).join("");
    }

    function renderHighlights(items) {
        var host = document.querySelector('[data-role="bds-highlights"]');
        if (!host) return;
        host.innerHTML = items.map(function (item) {
            var href = '/app-ui/batdongsan/search.aspx?ui_mode=app&q=' + encodeURIComponent(item.area);
            return '' +
                '<a class="app-bds-highlight-card is-' + item.accent + ' app-card-link" href="' + href + '">' +
                '  <span class="app-bds-highlight-area">' + item.area + '</span>' +
                '  <p class="app-bds-highlight-summary">' + item.summary + '</p>' +
                '</a>';
        }).join("");
    }

    function renderTabs(items) {
        var host = document.querySelector('[data-role="bds-feed-tabs"]');
        if (!host) return;
        host.innerHTML = items.map(function (item, index) {
            var href = '/app-ui/batdongsan/search.aspx?ui_mode=app&tab=' + encodeURIComponent(item);
            return '<a class="app-feed-tab' + (index === 0 ? ' is-active' : '') + '" href="' + href + '">' + item + '</a>';
        }).join("");
    }

    function parsePage(raw) {
        var value = parseInt(raw, 10);
        if (!value || value < 1) return 1;
        return value;
    }

    function getCurrentPage() {
        try {
            var params = new URLSearchParams(window.location.search || "");
            return parsePage(params.get("page"));
        } catch (e) {
            return 1;
        }
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
            try {
                var params = new URLSearchParams(window.location.search || "");
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
        for (var i = start; i <= end; i += 1) pages.push(i);
        if (end < safeTotal - 1) pages.push("ellipsis-right");
        pages.push(safeTotal);
        return pages;
    }

    function renderPagination(currentPage, totalPages, totalItems) {
        var host = document.querySelector('[data-role="bds-listing-feed"]');
        if (!host || !host.parentNode) return;

        var paginationHost = host.parentNode.querySelector('[data-role="bds-feed-pagination"]');
        if (!paginationHost) {
            paginationHost = document.createElement("div");
            paginationHost.setAttribute("data-role", "bds-feed-pagination");
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
            '<nav class="app-results-pagination" aria-label="Phân trang danh sách tin bất động sản">' +
            (safeCurrent > 1
                ? '<a class="app-page-number app-page-nav" href="' + prevHref + '">Trước</a>'
                : '<span class="app-page-number app-page-nav is-disabled">Trước</span>') +
            pageItems.map(function (item) {
                if (typeof item !== "number") return '<span class="app-page-ellipsis">...</span>';
                if (item === safeCurrent) return '<span class="app-page-number is-active">' + item + '</span>';
                return '<a class="app-page-number" href="' + buildPageHref(item) + '">' + item + '</a>';
            }).join("") +
            (safeCurrent < safeTotal
                ? '<a class="app-page-number app-page-nav" href="' + nextHref + '">Sau</a>'
                : '<span class="app-page-number app-page-nav is-disabled">Sau</span>') +
            '</nav>';
    }

    function renderListings(items) {
        var host = document.querySelector('[data-role="bds-listing-feed"]');
        if (!host) return;
        if (!items || !items.length) {
            host.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">i</div>' +
                '  <h2 class="app-empty-title">Chưa có tin bất động sản mới</h2>' +
                '  <p class="app-empty-copy">Feed sẽ tự cập nhật khi dữ liệu bất động sản có nội dung phù hợp.</p>' +
                '</section>';
            return;
        }
        host.innerHTML = items.map(function (item, index) {
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref("batdongsan", item.id)
                : '/app-ui/batdongsan/detail.aspx?id=' + encodeURIComponent(item.id) + '&ui_mode=app';
            return '' +
                '<article class="app-listing-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-listing-media" style="' + mediaStyle(item.image, "linear-gradient(135deg,#1d4ed8,#0f172a)") + '">' +
                '      <button class="app-listing-bookmark" type="button" aria-label="Lưu tin"></button>' +
                '      <div class="app-listing-media-meta">' +
                '        <span>' + (index + 2) + ' giờ trước</span>' +
                '        <span>' + (index + 8) + ' ảnh</span>' +
                '      </div>' +
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
        }).join("");
    }

    async function loadRuntimeListings(page) {
        try {
            var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.search;
            if (!adapter || typeof adapter.searchAsync !== "function") return { items: [], page: 1, total_pages: 0, total_items: 0 };
            var result = await adapter.searchAsync({
                q: "",
                space_code: "batdongsan",
                sort: "newest",
                page: parsePage(page),
                page_size: 8,
                filters: {}
            });
            if (!result || !Array.isArray(result.items)) {
                return { items: [], page: parsePage(page), total_pages: 0, total_items: 0 };
            }
            return result;
        } catch (e) {
            return { items: [], page: parsePage(page), total_pages: 0, total_items: 0 };
        }
    }

    async function bootstrap() {
        renderChipLane(dataRoot.quickFilters);
        renderQuickActions(dataRoot.quickActions);
        renderTypes(dataRoot.propertyTypes);
        renderHighlights(dataRoot.highlights);
        renderTabs(dataRoot.feedTabs);
        renderListings([]);

        var currentPage = getCurrentPage();
        var runtimeResult = await loadRuntimeListings(currentPage);
        var runtimeItems = runtimeResult && runtimeResult.items ? runtimeResult.items : [];
        renderListings(runtimeItems);
        renderPagination(runtimeResult.page || currentPage, runtimeResult.total_pages || 1, runtimeResult.total_items || runtimeItems.length);
    }

    bootstrap();
})();
