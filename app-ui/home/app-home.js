(function () {
    var RECENT_SEARCHES = ["nhà phố", "xe gia đình", "iphone", "gian hàng gần bạn"];
    var HOME_CATEGORY_FALLBACK = [
        { label: "Bất động sản", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/batdongsan/default.aspx?ui_mode=app" },
        { label: "Sức khoẻ & Làm đẹp", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Thời trang & Phụ kiện", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Đồ gia dụng & Nhà cửa", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Công nghệ & Thiết bị số", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/dienthoai-maytinh/default.aspx?ui_mode=app" },
        { label: "Mẹ & Bé", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Thực phẩm & Đồ uống", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Giáo dục & Phát triển bản thân", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Du lịch & Trải nghiệm", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Phương tiện & Phụ kiện", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/choxe/default.aspx?ui_mode=app" },
        { label: "Tài chính & Dịch vụ", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Văn phòng & Công cụ làm việc", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Thể thao & Giải trí", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Tâm linh - Phong thủy - Tâm lý", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Nông nghiệp & Vật nuôi", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Cho tặng miễn phí", image: "/app-ui/assets/placeholder-media.svg", href: "/app-ui/home/categories.aspx?ui_mode=app" }
    ];
    var QUICK_ACTIONS = [
        { label: "Danh mục", href: "/app-ui/home/categories.aspx?ui_mode=app" },
        { label: "Đăng tin", href: "/app-ui/gianhang/default.aspx?ui_mode=app", action: "post" },
        { label: "Đã lưu", href: "/app-ui/home/saved.aspx?ui_mode=app" },
        { label: "Liên hệ", href: "/app-ui/home/inbox.aspx?ui_mode=app" }
    ];
    var FEED_TABS = ["Dành cho bạn", "Gần bạn", "Mới nhất", "Video"];
    var CATEGORY_TILE_CODES = {
        "bat-dong-san": "BDS",
        "nha-dat": "BDS",
        "suc-khoe-lam-dep": "SK",
        "thoi-trang-phu-kien": "TT",
        "do-gia-dung-nha-cua": "GD",
        "cong-nghe-thiet-bi-so": "TEC",
        "me-be": "MB",
        "thuc-pham-do-uong": "TP",
        "giao-duc-phat-trien-ban-than": "EDU",
        "du-lich-trai-nghiem": "DL",
        "phuong-tien-phu-kien": "XE",
        "tai-chinh-dich-vu": "DV",
        "van-phong-cong-cu-lam-viec": "VP",
        "the-thao-giai-tri": "SP",
        "tam-linh-phong-thuy-tam-ly": "TL",
        "nong-nghiep-vat-nuoi": "NN",
        "cho-tang-mien-phi": "CHO"
    };
    var CATEGORY_TILE_IMAGES = {
        "bat-dong-san": "/app-ui/assets/home-categories/bat-dong-san.jpg",
        "nha-dat": "/app-ui/assets/home-categories/bat-dong-san.jpg",
        "suc-khoe-lam-dep": "/app-ui/assets/home-categories/suc-khoe-lam-dep.jpg",
        "thoi-trang-phu-kien": "/app-ui/assets/home-categories/thoi-trang-phu-kien.jpg",
        "do-gia-dung-nha-cua": "/app-ui/assets/home-categories/do-gia-dung-nha-cua.jpg",
        "cong-nghe-thiet-bi-so": "/app-ui/assets/home-categories/cong-nghe-thiet-bi-so.jpg",
        "me-be": "/app-ui/assets/home-categories/me-be.jpg",
        "thuc-pham-do-uong": "/app-ui/assets/home-categories/thuc-pham-do-uong.jpg",
        "giao-duc-phat-trien-ban-than": "/app-ui/assets/home-categories/giao-duc-phat-trien-ban-than.jpg",
        "du-lich-trai-nghiem": "/app-ui/assets/home-categories/du-lich-trai-nghiem.jpg",
        "phuong-tien-phu-kien": "/app-ui/assets/home-categories/phuong-tien-phu-kien.jpg",
        "tai-chinh-dich-vu": "/app-ui/assets/home-categories/tai-chinh-dich-vu.jpg",
        "van-phong-cong-cu-lam-viec": "/app-ui/assets/home-categories/van-phong-cong-cu-lam-viec.jpg",
        "the-thao-giai-tri": "/app-ui/assets/home-categories/the-thao-giai-tri.jpg",
        "tam-linh-phong-thuy-tam-ly": "/app-ui/assets/home-categories/tam-linh-phong-thuy-tam-ly.png",
        "nong-nghiep-vat-nuoi": "/app-ui/assets/home-categories/nong-nghiep-vat-nuoi.jpg",
        "cho-tang-mien-phi": "/app-ui/assets/home-categories/cho-tang-mien-phi.jpg"
    };

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

    function escapeHtml(text) {
        return String(text || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function getCurrentAppHref() {
        return (window.location.pathname || "/") + (window.location.search || "");
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

    function buildGenericCategoryHref(raw) {
        var slug = slugifyCategoryLabel(raw);
        return "/app-ui/home/category.aspx?ui_mode=app" + (slug ? "&slug=" + encodeURIComponent(slug) : "");
    }

    function decorateCategoryHref(url) {
        var href = (url || "").trim();
        if (!href) return "/app-ui/home/categories.aspx?ui_mode=app";
        if (href.indexOf("/app-ui/") === 0) return href;
        if (href.indexOf("/") === 0) return appendQuery(href, "return_url", getCurrentAppHref());
        return href;
    }

    function getHomeCategoryFallback() {
        return HOME_CATEGORY_FALLBACK.slice().map(function (item) {
            var next = Object.assign({}, item);
            if (!next.href || next.href.indexOf("/app-ui/home/categories.aspx") === 0) {
                next.href = buildGenericCategoryHref(next.slug || next.label || "");
            }
            return next;
        });
    }

    function arrangeCategoriesForRail(items, rowCount) {
        var source = Array.isArray(items) ? items.slice() : [];
        var rows = rowCount || 2;
        if (!source.length || rows < 2) return source;

        var perRow = Math.ceil(source.length / rows);
        var ordered = [];
        for (var column = 0; column < perRow; column += 1) {
            for (var row = 0; row < rows; row += 1) {
                var index = row * perRow + column;
                if (index < source.length) ordered.push(source[index]);
            }
        }
        return ordered;
    }

    function slugifyCategoryLabel(label) {
        var source = String(label || "").trim().toLowerCase();
        if (!source) return "";
        var map = {
            a: /[aáàảãạăắằẳẵặâấầẩẫậ]/g,
            d: /đ/g,
            e: /[eéèẻẽẹêếềểễệ]/g,
            i: /[iíìỉĩị]/g,
            o: /[oóòỏõọôốồổỗộơớờởỡợ]/g,
            u: /[uúùủũụưứừửữự]/g,
            y: /[yýỳỷỹỵ]/g
        };
        Object.keys(map).forEach(function (key) {
            source = source.replace(map[key], key);
        });
        return source
            .replace(/&/g, " ")
            .replace(/[^a-z0-9]+/g, "-")
            .replace(/^-+|-+$/g, "");
    }

    function getCategoryTileCode(item) {
        var slug = slugifyCategoryLabel(item.slug || item.label || item.name || "");
        if (CATEGORY_TILE_CODES[slug]) return CATEGORY_TILE_CODES[slug];

        var words = String(item.label || item.name || "")
            .trim()
            .split(/\s+/)
            .filter(Boolean);
        if (!words.length) return "DM";
        if (words.length === 1) return words[0].slice(0, 3).toUpperCase();
        return words.slice(0, 2).map(function (word) { return word.charAt(0); }).join("").toUpperCase();
    }

    function getCategoryTileImage(item) {
        var slug = slugifyCategoryLabel(item.slug || item.label || item.name || "");
        return CATEGORY_TILE_IMAGES[slug] || (item.image || "").trim();
    }

    function resolvePostHref(fallbackHref) {
        var sessionAdapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        if (!sessionAdapter || !window.AhaAppUi || typeof window.AhaAppUi.getPostTarget !== "function") {
            return fallbackHref;
        }
        var target = window.AhaAppUi.getPostTarget(sessionAdapter.getSession());
        return target && target.href ? target.href : fallbackHref;
    }

    function resolveSpaceCode(item) {
        if (!item) return "home";
        if (item.runtime_source === "batdongsan") return "batdongsan";
        if (item.runtime_source === "choxe") return "choxe";
        if (item.runtime_source === "dienthoai-maytinh") return "dienthoai-maytinh";
        if ((item.category || "").toLowerCase() === "bất động sản") return "batdongsan";
        return "home";
    }

    function isCategoriesDirectoryPage() {
        return /\/app-ui\/home\/categories\.aspx$/i.test(window.location.pathname || "");
    }

    function renderSpaces(items) {
        var host = document.querySelector('[data-role="space-categories"]');
        if (!host) return;
        var orderedItems = isCategoriesDirectoryPage()
            ? (Array.isArray(items) ? items.slice() : [])
            : arrangeCategoriesForRail(items, 2);
        host.classList.remove("app-home-category-track", "app-home-category-grid");
        host.classList.add(isCategoriesDirectoryPage() ? "app-home-category-grid" : "app-home-category-track");
        host.innerHTML = orderedItems.map(function (item) {
            var label = escapeHtml(item.label || item.name || "");
            var href = decorateCategoryHref(item.href || buildGenericCategoryHref(item.slug || item.label || item.name || ""));
            var tileCode = escapeHtml(getCategoryTileCode(item));
            var tileImage = getCategoryTileImage(item);
            var thumbStyle = tileImage ? ' style="background-image:url(' + escapeHtml(encodeURI(tileImage)) + ');"' : "";
            return '' +
                '<a class="app-space-card app-home-category-card" href="' + href + '">' +
                '  <span class="app-space-thumb app-home-category-thumb' + (tileImage ? ' has-image' : '') + '"' + thumbStyle + '>' +
                '    <span class="app-home-category-code">' + tileCode + '</span>' +
                '  </span>' +
                '  <span class="app-space-label app-home-category-label">' + label + '</span>' +
                '</a>';
        }).join("");
    }

    function fetchHomeCategoriesAsync() {
        if (!window.fetch) return Promise.resolve(getHomeCategoryFallback());

        return fetch("/app-ui/shared/runtime-categories.ashx?space=home", { credentials: "same-origin" })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (payload) {
                if (!payload || !payload.ok || !Array.isArray(payload.items) || !payload.items.length) {
                    return getHomeCategoryFallback();
                }
                return payload.items.map(function (item) {
                    return {
                        label: item.label || "",
                        slug: item.slug || "",
                        image: item.image || "",
                        href: item.href || "/app-ui/home/categories.aspx?ui_mode=app"
                    };
                });
            })
            .catch(function () {
                return getHomeCategoryFallback();
            });
    }


    function renderFeedTabs(items) {
        var host = document.querySelector('[data-role="feed-tabs"]');
        if (!host) return;
        host.innerHTML = items.map(function (item, index) {
            return '<a class="app-feed-tab' + (index === 0 ? ' is-active' : '') + '" href="/app-ui/home/search.aspx?ui_mode=app&tab=' + encodeURIComponent(item) + '">' + item + '</a>';
        }).join("");
    }

    function renderQuickActions(items) {
        var host = document.querySelector('[data-role="home-quick-actions"]');
        if (!host || !items || !items.length) return;
        host.innerHTML = items.map(function (item) {
            var href = item.action === "post" ? resolvePostHref(item.href) : item.href;
            return '<a class="app-space-quick-action" href="' + href + '">' + item.label + '</a>';
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
        var host = document.querySelector('[data-role="listing-feed"]');
        if (!host || !host.parentNode) return;

        var paginationHost = host.parentNode.querySelector('[data-role="home-feed-pagination"]');
        if (!paginationHost) {
            paginationHost = document.createElement("div");
            paginationHost.setAttribute("data-role", "home-feed-pagination");
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
            '<nav class="app-results-pagination" aria-label="Phân trang danh sách tin Home">' +
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
        var host = document.querySelector('[data-role="listing-feed"]');
        if (!host) return;
        host.innerHTML = items.map(function (item, index) {
            var spaceCode = resolveSpaceCode(item);
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref(spaceCode, item.id)
                : '/app-ui/' + spaceCode + '/detail.aspx?id=' + encodeURIComponent(item.id) + '&ui_mode=app';
            return '' +
                '<article class="app-listing-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-listing-media" style="' + resolveMediaStyle(item.image) + '">' +
                '      <button class="app-listing-bookmark" type="button" aria-label="Lưu tin"></button>' +
                '      <div class="app-listing-media-meta">' +
                '        <span>' + (index + 2) + ' giờ trước</span>' +
                '        <span>' + (index + 6) + ' ảnh</span>' +
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

    function renderEmptyListingState() {
        var host = document.querySelector('[data-role="listing-feed"]');
        if (!host) return;
        host.innerHTML = '' +
            '<article class="app-listing-card">' +
            '  <div class="app-listing-media" style="background:linear-gradient(135deg,#dbeafe,#bfdbfe 40%,#f8fafc)">' +
            '    <div class="app-listing-media-meta"><span>Dữ liệu hệ thống</span><span>Chờ dữ liệu</span></div>' +
            '  </div>' +
            '  <div class="app-listing-body">' +
            '    <div class="app-listing-category">Aha Sale</div>' +
            '    <div class="app-listing-title">Chưa có tin mới để hiển thị trên trang chủ app</div>' +
            '    <div class="app-listing-meta">Hiện chưa có dữ liệu từ Home, Bất động sản, Xe hoặc Công nghệ. Vui lòng thử lại sau ít phút.</div>' +
            '    <div class="app-chip-lane">' +
            '      <a class="app-chip" href="/app-ui/home/search.aspx?ui_mode=app">Mở tìm kiếm</a>' +
            '      <a class="app-chip" href="/app-ui/home/categories.aspx?ui_mode=app">Xem danh mục</a>' +
            '    </div>' +
            '  </div>' +
            '</article>';
    }

    async function loadRuntimeListings(page) {
        try {
            var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.search;
            if (!adapter || typeof adapter.searchAsync !== "function") return { items: [], page: 1, total_pages: 0, total_items: 0 };
            var result = await adapter.searchAsync({
                q: "",
                space_code: "home",
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
        if (window.AhaAppUi && typeof window.AhaAppUi.renderChipLane === "function") {
            window.AhaAppUi.renderChipLane('[data-role="recent-searches"]', RECENT_SEARCHES);
        }
        renderSpaces(await fetchHomeCategoriesAsync());
        renderQuickActions(QUICK_ACTIONS);
        renderFeedTabs(FEED_TABS);

        var currentPage = getCurrentPage();
        var runtimeResult = await loadRuntimeListings(currentPage);
        var runtimeItems = runtimeResult && runtimeResult.items ? runtimeResult.items : [];
        if (runtimeItems.length) {
            renderListings(runtimeItems);
            renderPagination(runtimeResult.page || currentPage, runtimeResult.total_pages || 1, runtimeResult.total_items || runtimeItems.length);
            return;
        }
        renderEmptyListingState();
        renderPagination(currentPage, 0, 0);
    }

    bootstrap();
})();
