(function () {
    function toSearchHref(query) {
        return '/app-ui/choxe/search.aspx?ui_mode=app&q=' + encodeURIComponent(query);
    }

    var dataRoot = {
        quickActions: [
            { label: "Ô tô", href: toSearchHref("Ô tô") },
            { label: "Xe máy", href: toSearchHref("Xe máy") },
            { label: "Đăng tin", href: "/app-ui/auth/open-shop.aspx?ui_mode=app", action: "post" }
        ],
        categories: [
            { label: "Ô tô", stat: "Xe mới, xe cũ, theo hãng và đời xe" },
            { label: "Xe máy", stat: "Xe tay ga, xe số, xe phân khối" },
            { label: "Phụ tùng", stat: "Linh kiện, đồ chơi và phụ kiện" },
            { label: "Dịch vụ xe", stat: "Rửa xe, bảo dưỡng, sửa chữa" }
        ],
        highlights: [
            { series: "Ô tô", summary: "Nhóm xe gia đình và SUV đang được xem nhiều", accent: "amber", href: toSearchHref("Ô tô") },
            { series: "Xe máy", summary: "Xe tay ga và xe số cũ giao dịch nhanh", accent: "sky", href: toSearchHref("Xe máy") },
            { series: "Biên Hòa", summary: "Nguồn xe cũ đời 2020-2023 tăng mạnh", accent: "slate", href: toSearchHref("Biên Hòa") }
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

    function resolvePostHref(fallbackHref) {
        var sessionAdapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        if (!sessionAdapter || !window.AhaAppUi || typeof window.AhaAppUi.getPostTarget !== "function") {
            return fallbackHref;
        }
        var target = window.AhaAppUi.getPostTarget(sessionAdapter.getSession());
        return target && target.href ? target.href : fallbackHref;
    }

    function renderChipLane() {
        var host = document.querySelector('[data-role="choxe-quick-filters"]');
        if (!host) return;
        var items = [
            { label: 'Dành cho bạn', href: '/app-ui/choxe/default.aspx?ui_mode=app' },
            { label: 'Gần bạn', href: toSearchHref('Gần bạn') },
            { label: 'Mới nhất', href: toSearchHref('Mới đăng') },
            { label: 'Xem thêm', href: '/app-ui/choxe/hub.aspx?ui_mode=app' }
        ];
        host.innerHTML = items.map(function (item, index) {
            return '<a class="app-chip' + (index === 0 ? ' is-active' : '') + '" href="' + item.href + '">' + item.label + '</a>';
        }).join('');
    }

    function renderQuickActions(items) {
        var host = document.querySelector('[data-role="choxe-quick-actions"]');
        if (!host || !items || !items.length) return;
        host.innerHTML = items.map(function (item) {
            var href = item.action === "post" ? resolvePostHref(item.href) : item.href;
            return '<a class="app-space-quick-action" href="' + href + '">' + item.label + '</a>';
        }).join("");
    }

    function renderCategories(items) {
        var host = document.querySelector('[data-role="choxe-categories"]');
        if (!host) return;
        var routeMap = {
            "Ô tô": toSearchHref("Ô tô"),
            "Xe máy": toSearchHref("Xe máy"),
            "Phụ tùng": toSearchHref("Phụ tùng xe"),
            "Dịch vụ xe": toSearchHref("Dịch vụ xe")
        };
        host.innerHTML = items.map(function (item) {
            var href = routeMap[item.label] || item.href || toSearchHref(item.label);
            return '' +
                '<a class="app-choxe-type-card app-card-link" href="' + href + '">' +
                '  <span class="app-choxe-type-label">' + item.label + '</span>' +
                '  <span class="app-choxe-type-stat">' + item.stat + '</span>' +
                '</a>';
        }).join("");
    }

    function renderHighlights(items) {
        var host = document.querySelector('[data-role="choxe-highlights"]');
        if (!host) return;
        host.innerHTML = items.map(function (item) {
            var href = item.href || toSearchHref(item.series);
            return '' +
                '<a class="app-choxe-highlight-card app-card-link is-' + item.accent + '" href="' + href + '">' +
                '  <span class="app-choxe-highlight-series">' + item.series + '</span>' +
                '  <p class="app-choxe-highlight-summary">' + item.summary + '</p>' +
                '</a>';
        }).join("");
    }

    function renderTabs(items) {
        var host = document.querySelector('[data-role="choxe-feed-tabs"]');
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
        var host = document.querySelector('[data-role="choxe-listing-feed"]');
        if (!host || !host.parentNode) return;

        var paginationHost = host.parentNode.querySelector('[data-role="choxe-feed-pagination"]');
        if (!paginationHost) {
            paginationHost = document.createElement("div");
            paginationHost.setAttribute("data-role", "choxe-feed-pagination");
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
            '<nav class="app-results-pagination" aria-label="Phân trang danh sách tin xe">' +
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
        var host = document.querySelector('[data-role="choxe-listing-feed"]');
        if (!host) return;
        if (!items || !items.length) {
            host.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">i</div>' +
                '  <h2 class="app-empty-title">Chưa có tin xe mới</h2>' +
                '  <p class="app-empty-copy">Feed xe sẽ hiện ở đây khi có dữ liệu phù hợp.</p>' +
                '</section>';
            return;
        }
        host.innerHTML = items.map(function (item, index) {
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref("choxe", item.id)
                : '/app-ui/choxe/detail.aspx?id=' + encodeURIComponent(item.id) + '&ui_mode=app';
            return '' +
                '<article class="app-listing-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-listing-media" style="' + mediaStyle(item.image, "linear-gradient(135deg,#0f172a,#334155)") + '">' +
                '      <button class="app-listing-bookmark" type="button" aria-label="Lưu tin"></button>' +
                '      <div class="app-listing-media-meta">' +
                '        <span>' + (index + 1) + ' giờ trước</span>' +
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
                space_code: "choxe",
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
        renderChipLane();
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
