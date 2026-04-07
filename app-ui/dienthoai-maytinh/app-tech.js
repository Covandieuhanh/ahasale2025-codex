(function () {
    function toSearchHref(query) {
        return '/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app&q=' + encodeURIComponent(query);
    }

    var dataRoot = {
        quickFilters: ["iPhone", "Laptop", "MacBook", "Phụ kiện"],
        quickActions: [
            { label: "Điện thoại", href: toSearchHref("Điện thoại") },
            { label: "Máy tính", href: toSearchHref("Máy tính") },
            { label: "Đăng tin", href: "/app-ui/auth/open-shop.aspx?ui_mode=app", action: "post" }
        ],
        categories: [
            { label: "Điện thoại", stat: "Smartphone, máy cũ, theo dòng máy" },
            { label: "Máy tính", stat: "Laptop, PC, màn hình, linh kiện" },
            { label: "Phụ kiện", stat: "Tai nghe, sạc, bàn phím, chuột" }
        ],
        highlights: [
            { model: "iPhone", summary: "Máy cũ và máy like new đang có lượt xem cao", accent: "blue" },
            { model: "Laptop", summary: "Máy văn phòng và gaming giao dịch nhanh", accent: "slate" },
            { model: "Phụ kiện", summary: "Tai nghe, sạc, bàn phím đang được hỏi nhiều", accent: "sky" }
        ],
        feedTabs: ["Dành cho bạn", "Mới nhất", "Xem nhiều"],
        listings: []
    };

    function mediaStyle(image, fallback) {
        var value = image || "";
        if (value.indexOf("http") === 0 || value.indexOf("/") === 0) {
            return "background-image:url('" + value + "');background-size:cover;background-position:center;";
        }
        return "background:" + (value || fallback);
    }

    function resolvePostHref(fallbackHref) {
        var sessionAdapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        if (!sessionAdapter || !window.AhaAppUi || typeof window.AhaAppUi.getPostTarget !== "function") {
            return fallbackHref;
        }
        var target = window.AhaAppUi.getPostTarget(sessionAdapter.getSession());
        return target && target.href ? target.href : fallbackHref;
    }

    function renderChipLane(items) {
        var host = document.querySelector('[data-role="tech-quick-filters"]');
        if (!host || !items || !items.length) return;
        host.innerHTML = items.map(function (item, index) {
            return '<a class="app-chip' + (index === 0 ? ' is-active' : '') + '" href="' + toSearchHref(item) + '">' + item + '</a>';
        }).join("");
    }

    function renderQuickActions(items) {
        var host = document.querySelector('[data-role="tech-quick-actions"]');
        if (!host || !items || !items.length) return;
        host.innerHTML = items.map(function (item) {
            var href = item.action === "post" ? resolvePostHref(item.href) : item.href;
            return '<a class="app-space-quick-action" href="' + href + '">' + item.label + '</a>';
        }).join("");
    }

    function renderCategories(items) {
        var host = document.querySelector('[data-role="tech-categories"]');
        if (!host) return;
        var routeMap = {
            "Điện thoại": toSearchHref("Điện thoại"),
            "Máy tính": toSearchHref("Máy tính"),
            "Phụ kiện": toSearchHref("Phụ kiện công nghệ")
        };
        host.innerHTML = items.map(function (item) {
            var href = routeMap[item.label] || ('/app-ui/dienthoai-maytinh/hub.aspx?ui_mode=app&q=' + encodeURIComponent(item.label));
            return '' +
                '<a class="app-tech-type-card app-card-link" href="' + href + '">' +
                '  <span class="app-tech-type-label">' + item.label + '</span>' +
                '  <span class="app-tech-type-stat">' + item.stat + '</span>' +
                '</a>';
        }).join("");
    }

    function renderHighlights(items) {
        var host = document.querySelector('[data-role="tech-highlights"]');
        if (!host) return;
        host.innerHTML = items.map(function (item) {
            var href = toSearchHref(item.model);
            return '' +
                '<a class="app-tech-highlight-card app-card-link is-' + item.accent + '" href="' + href + '">' +
                '  <span class="app-tech-highlight-model">' + item.model + '</span>' +
                '  <p class="app-tech-highlight-summary">' + item.summary + '</p>' +
                '</a>';
        }).join("");
    }

    function renderTabs(items) {
        var host = document.querySelector('[data-role="tech-feed-tabs"]');
        if (!host) return;
        host.innerHTML = items.map(function (item, index) {
            return '<a class="app-feed-tab' + (index === 0 ? ' is-active' : '') + '" href="' + toSearchHref(item) + '">' + item + '</a>';
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
        var host = document.querySelector('[data-role="tech-listing-feed"]');
        if (!host || !host.parentNode) return;

        var paginationHost = host.parentNode.querySelector('[data-role="tech-feed-pagination"]');
        if (!paginationHost) {
            paginationHost = document.createElement("div");
            paginationHost.setAttribute("data-role", "tech-feed-pagination");
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
            '<nav class="app-results-pagination" aria-label="Phân trang danh sách tin công nghệ">' +
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
        var host = document.querySelector('[data-role="tech-listing-feed"]');
        if (!host) return;
        if (!items || !items.length) {
            host.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">i</div>' +
                '  <h2 class="app-empty-title">Chưa có tin công nghệ mới</h2>' +
                '  <p class="app-empty-copy">Feed công nghệ sẽ tự hiện khi có dữ liệu phù hợp.</p>' +
                '</section>';
            return;
        }
        host.innerHTML = items.map(function (item, index) {
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref("dienthoai-maytinh", item.id)
                : '/app-ui/dienthoai-maytinh/detail.aspx?id=' + encodeURIComponent(item.id) + '&ui_mode=app';
            return '' +
                '<article class="app-listing-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-listing-media" style="' + mediaStyle(item.image, "linear-gradient(135deg,#2563eb,#1e3a8a)") + '">' +
                '      <button class="app-listing-bookmark" type="button" aria-label="Lưu tin"></button>' +
                '      <div class="app-listing-media-meta">' +
                '        <span>' + (index + 2) + ' giờ trước</span>' +
                '        <span>' + (index + 4) + ' ảnh</span>' +
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
                space_code: "dienthoai-maytinh",
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
        renderCategories(dataRoot.categories);
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
