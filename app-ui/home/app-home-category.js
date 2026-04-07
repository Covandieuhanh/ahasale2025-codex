(function () {
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

    function escapeHtml(text) {
        return String(text || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
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
        var slug = slugifyCategoryLabel(item.slug || item.label || "");
        if (CATEGORY_TILE_CODES[slug]) return CATEGORY_TILE_CODES[slug];
        var words = String(item.label || "").trim().split(/\s+/).filter(Boolean);
        if (!words.length) return "DM";
        if (words.length === 1) return words[0].slice(0, 3).toUpperCase();
        return words.slice(0, 2).map(function (word) { return word.charAt(0); }).join("").toUpperCase();
    }

    function getCategoryTileImage(item) {
        var slug = slugifyCategoryLabel(item.slug || item.label || "");
        return CATEGORY_TILE_IMAGES[slug] || "";
    }

    function getCurrentAppHref() {
        return (window.location.pathname || "/") + (window.location.search || "");
    }

    function buildCategorySearchHref(label) {
        return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent(label || "");
    }

    function buildCategoriesHref() {
        return "/app-ui/home/categories.aspx?ui_mode=app";
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
            return window.location.pathname + "?page=" + safePage;
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
        var host = document.querySelector("[data-role='category-results']");
        if (!host || !host.parentNode) return;

        var paginationHost = host.parentNode.querySelector("[data-role='category-pagination']");
        if (!paginationHost) {
            paginationHost = document.createElement("div");
            paginationHost.setAttribute("data-role", "category-pagination");
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
            '<nav class="app-results-pagination" aria-label="Phân trang danh mục Home">' +
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

    function resolveSpaceCode(item) {
        if (!item) return "home";
        if (item.runtime_source === "batdongsan") return "batdongsan";
        if (item.runtime_source === "choxe") return "choxe";
        if (item.runtime_source === "dienthoai-maytinh") return "dienthoai-maytinh";
        if ((item.category || "").toLowerCase() === "bất động sản") return "batdongsan";
        return "home";
    }

    function renderHero(category) {
        var titleNode = document.querySelector("[data-role='category-title']");
        var copyNode = document.querySelector("[data-role='category-copy']");
        var codeNode = document.querySelector("[data-role='category-code']");
        var thumbNode = document.querySelector("[data-role='category-thumb']");
        var summaryNode = document.querySelector("[data-role='category-summary']");
        var actionsNode = document.querySelector("[data-role='category-actions']");

        var label = category.label || "Danh mục Home";
        if (titleNode) titleNode.textContent = label;
        if (copyNode) {
            copyNode.textContent = "Màn hình app gom tin mới và điểm vào liên quan cho danh mục " + label + ".";
        }
        if (summaryNode) {
            summaryNode.textContent = "Đang tải tin mới cho danh mục " + label + "...";
        }
        if (codeNode) codeNode.textContent = getCategoryTileCode(category);
        if (thumbNode) {
            var image = getCategoryTileImage(category);
            if (image) {
                thumbNode.classList.add("has-image");
                thumbNode.style.backgroundImage = "url('" + encodeURI(image) + "')";
            } else {
                thumbNode.classList.remove("has-image");
                thumbNode.style.backgroundImage = "";
            }
        }
        if (actionsNode) {
            actionsNode.innerHTML = '' +
                '<a class="app-page-btn is-primary" href="' + buildCategorySearchHref(label) + '">Tìm trong Home</a>' +
                '<a class="app-page-btn" href="' + buildCategoriesHref() + '">Toàn bộ danh mục</a>' +
                '<a class="app-page-btn" href="/app-ui/home/default.aspx?ui_mode=app">Về trang chủ</a>';
        }
        document.title = "AhaSale App UI - " + label;
    }

    function renderListings(items) {
        var host = document.querySelector("[data-role='category-results']");
        if (!host) return;
        host.innerHTML = items.map(function (item, index) {
            var spaceCode = resolveSpaceCode(item);
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref(spaceCode, item.id, { backHref: getCurrentAppHref() })
                : "/app-ui/" + spaceCode + "/detail.aspx?id=" + encodeURIComponent(item.id) + "&ui_mode=app&back_href=" + encodeURIComponent(getCurrentAppHref());
            return '' +
                '<article class="app-listing-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-listing-media" style="background-image:url(\'' + escapeHtml((item.image || "").replace(/'/g, "%27")) + '\');background-size:cover;background-position:center;">' +
                '      <div class="app-listing-media-meta">' +
                '        <span>' + (index + 1) + ' gợi ý</span>' +
                '        <span>Chi tiết</span>' +
                '      </div>' +
                '    </div>' +
                '    <div class="app-listing-body">' +
                '      <div class="app-listing-category">' + escapeHtml(item.category || "Home") + '</div>' +
                '      <div class="app-listing-title">' + escapeHtml(item.title || "Tin đăng") + '</div>' +
                '      <div class="app-listing-price">' + escapeHtml(item.price || "Liên hệ") + '</div>' +
                '      <div class="app-listing-meta">' + escapeHtml(item.meta || "Đang cập nhật") + '</div>' +
                '      <div class="app-listing-location">' + escapeHtml(item.location || "Toàn quốc") + '</div>' +
                '      <div class="app-listing-badge">' + escapeHtml(item.badge || "Danh mục Home") + '</div>' +
                '    </div>' +
                '  </a>' +
                '</article>';
        }).join("");
    }

    function renderEmptyState(category) {
        var host = document.querySelector("[data-role='category-results']");
        var summaryNode = document.querySelector("[data-role='category-summary']");
        if (summaryNode) {
            summaryNode.textContent = "Chưa có tin runtime phù hợp cho danh mục " + (category.label || "đã chọn") + ".";
        }
        if (!host) return;
        host.innerHTML = '' +
            '<article class="app-listing-card">' +
            '  <div class="app-listing-media" style="background:linear-gradient(135deg,#dbeafe,#bfdbfe 40%,#f8fafc)">' +
            '    <div class="app-listing-media-meta"><span>Danh mục Home</span><span>Chờ dữ liệu</span></div>' +
            '  </div>' +
            '  <div class="app-listing-body">' +
            '    <div class="app-listing-category">' + escapeHtml(category.label || "Danh mục") + '</div>' +
            '    <div class="app-listing-title">Chưa có tin phù hợp để hiển thị trên app</div>' +
            '    <div class="app-listing-meta">Bạn vẫn có thể chuyển sang màn tìm kiếm Home hoặc quay lại danh mục tổng.</div>' +
            '    <div class="app-chip-lane">' +
            '      <a class="app-chip" href="' + buildCategorySearchHref(category.label || "") + '">Mở tìm kiếm</a>' +
            '      <a class="app-chip" href="' + buildCategoriesHref() + '">Xem danh mục khác</a>' +
            '    </div>' +
            '  </div>' +
            '</article>';
    }

    function buildCategoryFromQuery() {
        var label = getQuery("label");
        var slug = getQuery("slug");
        return {
            id: getQuery("id"),
            label: label || "Danh mục Home",
            slug: slug || slugifyCategoryLabel(label)
        };
    }

    function findCurrentCategory(items) {
        var id = getQuery("id");
        var slug = getQuery("slug");
        var label = getQuery("label");
        var safeItems = Array.isArray(items) ? items : [];
        for (var i = 0; i < safeItems.length; i += 1) {
            var item = safeItems[i] || {};
            if (id && item.id && String(item.id) === String(id)) return item;
            if (slug && item.slug && item.slug === slug) return item;
            if (label && item.label && item.label === label) return item;
        }
        return buildCategoryFromQuery();
    }

    function fetchCategoriesAsync() {
        if (!window.fetch) return Promise.resolve([]);
        return fetch("/app-ui/shared/runtime-categories.ashx?space=home", { credentials: "same-origin" })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (payload) {
                return payload && payload.ok && Array.isArray(payload.items) ? payload.items : [];
            })
            .catch(function () { return []; });
    }

    function loadRuntimeListings(category, page) {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.search;
        if (!adapter || typeof adapter.searchAsync !== "function") {
            return Promise.resolve({ items: [], page: parsePage(page), total_pages: 0, total_items: 0 });
        }
        return adapter.searchAsync({
            q: category.label || "",
            space_code: "home",
            sort: "newest",
            page: parsePage(page),
            page_size: 8,
            filters: {}
        });
    }

    async function bootstrap() {
        var categoryItems = await fetchCategoriesAsync();
        var category = findCurrentCategory(categoryItems);
        renderHero(category);

        var currentPage = parsePage(getQuery("page"));
        var runtimeResult = await loadRuntimeListings(category, currentPage);
        var runtimeItems = runtimeResult && Array.isArray(runtimeResult.items) ? runtimeResult.items : [];
        var summaryNode = document.querySelector("[data-role='category-summary']");
        if (runtimeItems.length) {
            if (summaryNode) {
                summaryNode.textContent = "Đang hiển thị " + runtimeItems.length + " tin gợi ý mới nhất cho danh mục " + (category.label || "đã chọn") + ".";
            }
            renderListings(runtimeItems);
            renderPagination(runtimeResult.page || currentPage, runtimeResult.total_pages || 1, runtimeResult.total_items || runtimeItems.length);
            return;
        }
        renderEmptyState(category);
        renderPagination(currentPage, 0, 0);
    }

    bootstrap();
})();
