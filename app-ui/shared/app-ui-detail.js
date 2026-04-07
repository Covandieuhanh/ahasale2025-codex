(function () {
    function escapeHtml(value) {
        return String(value || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function cleanDisplayText(value) {
        var text = String(value || "").replace(/\r\n/g, "\n").replace(/\r/g, "\n");
        text = text.replace(/\s+/g, " ").trim();
        if (!text) return "";

        var lower = text.toLowerCase();
        var cutTokens = [
            '"@context"',
            "@context",
            "schema.org",
            "realestatelisting",
            "breadcrumblist",
            "itemlistelement",
            "tài khoản xin chào",
            "tai khoan xin chao",
            "xin chào, khách",
            "xin chao, khach",
            "đăng nhập",
            "dang nhap",
            "cookie"
        ];
        var cutIndex = -1;
        for (var i = 0; i < cutTokens.length; i += 1) {
            var idx = lower.indexOf(cutTokens[i]);
            if (idx >= 0 && (cutIndex < 0 || idx < cutIndex)) cutIndex = idx;
        }
        if (cutIndex >= 0) {
            text = text.slice(0, cutIndex).trim();
        }

        text = text.replace(/[",;:|\-\s]+$/g, "").trim();
        return text;
    }

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

    function decorateRuntimeHref(href) {
        var value = (href || "").trim();
        if (!value) return value;
        if (/^https?:/i.test(value)) return value;
        if (value.indexOf("/app-ui/") === 0) return value;
        if (value.charAt(0) === "/") {
            return appendQuery(value, "return_url", getCurrentAppHref());
        }
        return value;
    }

    function buildRelatedSearchHref(spaceCode, item) {
        var keyword = item && item.title ? cleanDisplayText(item.title) : "";
        if (spaceCode === "batdongsan") {
            return appendQuery("/app-ui/batdongsan/search.aspx?ui_mode=app", "q", keyword || "bat dong san");
        }
        if (spaceCode === "choxe") {
            return appendQuery("/app-ui/choxe/search.aspx?ui_mode=app", "q", keyword || "xe");
        }
        if (spaceCode === "dienthoai-maytinh") {
            return appendQuery("/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app", "q", keyword || "dien thoai may tinh");
        }
        return appendQuery("/app-ui/home/search.aspx?ui_mode=app", "q", keyword || "tin moi");
    }

    function isAppHref(value) {
        return !!value && value.indexOf("/app-ui/") === 0;
    }

    function resolveBackContext(defaultHref, searchHref) {
        var backHref = sanitizeAppHref(getQuery("back_href"));
        if (!isAppHref(backHref)) {
            return {
                href: searchHref,
                label: "Quay lại kết quả",
                secondaryHref: searchHref
            };
        }

        var label = "Quay lại màn trước";
        if (/\/search\.aspx/i.test(backHref)) label = "Quay lại kết quả";
        else if (/\/saved\.aspx/i.test(backHref)) label = "Quay lại tin đã lưu";
        else if (/\/notifications\.aspx/i.test(backHref)) label = "Quay lại thông báo";
        else if (/\/inbox\.aspx/i.test(backHref)) label = "Quay lại liên hệ";
        else if (/\/categories\.aspx/i.test(backHref) || /\/hub\.aspx/i.test(backHref)) label = "Quay lại danh mục";
        else if (/\/listing\.aspx/i.test(backHref)) label = "Quay lại quản lý tin";
        else if (/\/default\.aspx/i.test(backHref)) label = "Quay lại trang chủ";

        return {
            href: backHref,
            label: label,
            secondaryHref: /\/search\.aspx/i.test(backHref) || /\/listing\.aspx/i.test(backHref) ? backHref : searchHref
        };
    }

    async function run() {
        var id = getQuery("id");
        var host = document.querySelector("[data-role='detail-host']");
        var isBds = window.location.pathname.indexOf("/batdongsan/") >= 0;
        var isHome = window.location.pathname.indexOf("/home/") >= 0;
        var isXe = window.location.pathname.indexOf("/choxe/") >= 0;
        var isTech = window.location.pathname.indexOf("/dienthoai-maytinh/") >= 0;
        if (!host) return;

        var spaceCode = isBds ? "batdongsan" : isXe ? "choxe" : isTech ? "dienthoai-maytinh" : "home";
        var item = await window.AhaAppUiAdapters.listing.getByIdAsync(id, spaceCode);
        var defaultHref = isBds ? "/app-ui/batdongsan/default.aspx?ui_mode=app"
            : isXe ? "/app-ui/choxe/default.aspx?ui_mode=app"
            : isTech ? "/app-ui/dienthoai-maytinh/default.aspx?ui_mode=app"
            : "/app-ui/home/default.aspx?ui_mode=app";
        var searchHref = isBds ? "/app-ui/batdongsan/search.aspx?ui_mode=app"
            : isXe ? "/app-ui/choxe/search.aspx?ui_mode=app"
            : isTech ? "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app"
            : "/app-ui/home/search.aspx?ui_mode=app";
        var backContext = resolveBackContext(defaultHref, searchHref);
        var relatedSearchHref = buildRelatedSearchHref(spaceCode, item);
        var session = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session
            ? window.AhaAppUiAdapters.session.getSession()
            : null;
        var postHref = window.AhaAppUi && typeof window.AhaAppUi.getPostTarget === "function"
            ? window.AhaAppUi.getPostTarget(session).href
            : "/app-ui/gianhang/default.aspx?ui_mode=app";
        var sourceHref = item && item.detail_url ? decorateRuntimeHref(item.detail_url) : relatedSearchHref;
        var sourceActionLabel = isBds
            ? "Mở nguồn bất động sản"
            : isXe
                ? "Mở trang xe gốc"
                : isTech
                    ? "Mở nguồn thiết bị"
                    : "Mở trang nguồn";
        var internalActionLabel = isBds
            ? "Mở kết quả BĐS"
            : isXe
                ? "Mở kết quả xe"
                : isTech
                    ? "Mở kết quả thiết bị"
                    : "Mở kết quả trong app";
        var primaryActionLabel = item && item.detail_url ? sourceActionLabel : internalActionLabel;
        var sourceChipLabel = item && item.detail_url ? "Mở trang nguồn" : "Mở kết quả liên quan";
        var sourceChipHref = sourceHref;

        if (!item) {
            host.innerHTML = '' +
                '<section class="app-section">' +
                '  <div class="app-empty-state">' +
                '    <div class="app-empty-icon">!</div>' +
                '    <h1 class="app-empty-title">Không tìm thấy tin</h1>' +
                '    <p class="app-empty-copy">Tin không còn khả dụng hoặc liên kết chi tiết đã thay đổi trên hệ thống.</p>' +
                '    <div class="app-empty-actions">' +
                '      <a class="app-ghost-btn" href="' + backContext.href + '">' + backContext.label + '</a>' +
                '      <a class="app-ghost-btn" href="' + defaultHref + '">Về màn chính</a>' +
                '    </div>' +
                '  </div>' +
                '</section>';
            return;
        }

        var safeCategory = escapeHtml(cleanDisplayText(item.category));
        var safeTitle = escapeHtml(cleanDisplayText(item.title));
        var safePrice = escapeHtml(cleanDisplayText(item.price));
        var safeMeta = escapeHtml(cleanDisplayText(item.meta));
        var safeLocation = escapeHtml(cleanDisplayText(item.location));
        var safeSummary = escapeHtml(cleanDisplayText(item.summary || "Nội dung chi tiết sẽ được nối vào listing core và listing projection về sau."));
        var safeBadge = escapeHtml(cleanDisplayText(item.badge || "Đề xuất"));

        var specHtml = "";
        if (isBds && item.specs && item.specs.length) {
        var highlightSpecs = item.specs.slice(0, 3).map(function (spec) {
            return '' +
                '<div class="app-detail-highlight-card">' +
                '  <span class="app-detail-highlight-value">' + escapeHtml(cleanDisplayText(spec.value)) + '</span>' +
                '  <span class="app-detail-highlight-label">' + escapeHtml(cleanDisplayText(spec.label)) + '</span>' +
                '</div>';
        }).join("");
        specHtml = '' +
            '<div class="app-detail-highlight-strip">' + highlightSpecs + '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Thông số bất động sản</h2>' +
            '  <div class="app-detail-spec-grid">' +
            item.specs.map(function (spec) {
                return '' +
                    '<div class="app-detail-spec-item">' +
                    '  <span class="app-detail-spec-label">' + escapeHtml(cleanDisplayText(spec.label)) + '</span>' +
                    '  <span class="app-detail-spec-value">' + escapeHtml(cleanDisplayText(spec.value)) + '</span>' +
                    '</div>';
            }).join("") +
            '  </div>' +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Vị trí và bản đồ</h2>' +
            '  <div class="app-detail-map-box">Vùng bản đồ dành riêng cho app-ui bất động sản<br />' + escapeHtml(cleanDisplayText(item.mapLabel || item.location)) + '</div>' +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Người đăng</h2>' +
            '  <div class="app-detail-seller-card">' +
            '    <div>' +
            '      <span class="app-detail-seller-name">' + escapeHtml(cleanDisplayText((item.seller && item.seller.name) || "Tài khoản đăng tin")) + '</span>' +
            '      <span class="app-detail-seller-role">' + escapeHtml(cleanDisplayText((item.seller && item.seller.role) || (item.detail_url ? "Mở nguồn gốc để xem thông tin liên hệ hiện tại." : "Nguồn liên hệ sẽ nối qua adapter sau"))) + '</span>' +
            '    </div>' +
            '    <a class="app-detail-btn is-secondary" href="' + sourceChipHref + '">' + sourceChipLabel + '</a>' +
            '  </div>' +
            '</div>';
        } else if (isHome) {
        specHtml = '' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Điểm nổi bật</h2>' +
            '  <div class="app-detail-dual-grid">' +
            '    <div class="app-detail-mini-card">' +
            '      <span class="app-detail-mini-label">Danh mục</span>' +
            '      <span class="app-detail-mini-value">' + item.category + '</span>' +
            '    </div>' +
            '    <div class="app-detail-mini-card">' +
            '      <span class="app-detail-mini-label">Nhịp hiển thị</span>' +
            '      <span class="app-detail-mini-value">' + (item.badge || "Ưu tiên") + '</span>' +
            '    </div>' +
            '  </div>' +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Mô tả nhanh</h2>' +
            '  <div class="app-detail-summary">Màn detail app giữ bố cục tối giản để ưu tiên thao tác: đọc thông tin nhanh, sau đó mở sang đúng trang dữ liệu.</div>' +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Thao tác đề xuất</h2>' +
            '  <div class="app-chip-lane">' +
            '    <a class="app-chip" href="/app-ui/home/search.aspx?ui_mode=app">Xem thêm tin liên quan</a>' +
            '    <a class="app-chip" href="' + sourceChipHref + '">' + sourceChipLabel + '</a>' +
            '    <a class="app-chip" href="/app-ui/default.aspx?ui_mode=app">Đổi không gian</a>' +
            '  </div>' +
            '</div>';
        } else if (isXe) {
        specHtml = '' +
            '<div class="app-detail-highlight-strip">' +
            (item.specs || []).slice(0, 3).map(function (spec) {
                return '' +
                    '<div class="app-detail-highlight-card">' +
                    '  <span class="app-detail-highlight-value">' + escapeHtml(cleanDisplayText(spec.value)) + '</span>' +
                    '  <span class="app-detail-highlight-label">' + escapeHtml(cleanDisplayText(spec.label)) + '</span>' +
                    '</div>';
            }).join("") +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Thông tin xe</h2>' +
            '  <div class="app-detail-spec-grid">' +
            (item.specs || []).map(function (spec) {
                return '' +
                    '<div class="app-detail-spec-item">' +
                    '  <span class="app-detail-spec-label">' + escapeHtml(cleanDisplayText(spec.label)) + '</span>' +
                    '  <span class="app-detail-spec-value">' + escapeHtml(cleanDisplayText(spec.value)) + '</span>' +
                    '</div>';
            }).join("") +
            '  </div>' +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Gợi ý thao tác</h2>' +
            '  <div class="app-chip-lane">' +
            '    <a class="app-chip" href="/app-ui/choxe/search.aspx?ui_mode=app">Xem thêm xe cùng nhóm</a>' +
            '    <a class="app-chip" href="' + sourceChipHref + '">' + sourceChipLabel + '</a>' +
            '    <a class="app-chip" href="/app-ui/default.aspx?ui_mode=app">Đổi không gian</a>' +
            '  </div>' +
            '</div>';
        } else if (isTech) {
        specHtml = '' +
            '<div class="app-detail-highlight-strip">' +
            (item.specs || []).slice(0, 3).map(function (spec) {
                return '' +
                    '<div class="app-detail-highlight-card">' +
                    '  <span class="app-detail-highlight-value">' + escapeHtml(cleanDisplayText(spec.value)) + '</span>' +
                    '  <span class="app-detail-highlight-label">' + escapeHtml(cleanDisplayText(spec.label)) + '</span>' +
                    '</div>';
            }).join("") +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Thông tin thiết bị</h2>' +
            '  <div class="app-detail-spec-grid">' +
            (item.specs || []).map(function (spec) {
                return '' +
                    '<div class="app-detail-spec-item">' +
                    '  <span class="app-detail-spec-label">' + escapeHtml(cleanDisplayText(spec.label)) + '</span>' +
                    '  <span class="app-detail-spec-value">' + escapeHtml(cleanDisplayText(spec.value)) + '</span>' +
                    '</div>';
            }).join("") +
            '  </div>' +
            '</div>' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Gợi ý thao tác</h2>' +
            '  <div class="app-chip-lane">' +
            '    <a class="app-chip" href="/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app">Xem thêm máy liên quan</a>' +
            '    <a class="app-chip" href="' + sourceChipHref + '">' + sourceChipLabel + '</a>' +
            '    <a class="app-chip" href="/app-ui/default.aspx?ui_mode=app">Đổi không gian</a>' +
            '  </div>' +
            '</div>';
        } else {
        specHtml = '' +
            '<div class="app-detail-panel">' +
            '  <h2 class="app-detail-panel-title">Khung chi tiết app-ui</h2>' +
            '  <p class="app-detail-summary">Trang này là host độc lập cho flow detail. Dữ liệu chi tiết sẽ tiếp tục được nối qua listing adapter theo từng không gian.</p>' +
            '</div>';
    }

        host.innerHTML = '' +
            '<section class="app-section">' +
            '  <div class="app-detail-hero" style="' + resolveMediaStyle(item.image) + '"></div>' +
            '  <div class="app-detail-body">' +
            '    <div class="app-detail-category">' + safeCategory + '</div>' +
            '    <h1 class="app-detail-title">' + safeTitle + '</h1>' +
            '    <div class="app-detail-price">' + safePrice + '</div>' +
            '    <div class="app-detail-meta">' + safeMeta + '</div>' +
            '    <div class="app-detail-location">' + safeLocation + '</div>' +
            '    <div class="app-detail-summary">' + safeSummary + '</div>' +
            '    <div class="app-detail-badge-row"><span class="app-detail-pill">' + safeBadge + '</span><span class="app-detail-pill">' + safeCategory + '</span></div>' +
            '    <div class="app-detail-flow-row">' +
            '      <a class="app-detail-flow-chip" href="' + backContext.href + '">' + backContext.label + '</a>' +
            '      <a class="app-detail-flow-chip" href="' + defaultHref + '">Về trang chính</a>' +
            '      <a class="app-detail-flow-chip is-primary" href="' + postHref + '">Đăng tin</a>' +
            '    </div>' +
                  specHtml +
            '    <div class="app-detail-cta-row app-detail-cta-row-main">' +
            '      <a class="app-detail-btn is-primary" href="' + sourceHref + '">' + primaryActionLabel + '</a>' +
            '      <a class="app-detail-btn is-secondary" href="' + backContext.secondaryHref + '">' + backContext.label + '</a>' +
            '    </div>' +
            (item.detail_url ? '<div class="app-detail-cta-row"><a class="app-detail-btn is-secondary" href="' + decorateRuntimeHref(item.detail_url) + '">Mở trang gốc</a></div>' : '') +
            '  </div>' +
            '</section>';
        document.title = cleanDisplayText(item.title) + " - App UI";
    }

    run();
})();
