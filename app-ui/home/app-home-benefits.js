(function () {
    var PROFILE_META = {
        consumer: {
            title: "Hồ sơ quyền tiêu dùng",
            copy: "Lịch sử quyền tiêu dùng trên app được trình bày lại theo ngữ cảnh làm việc nhanh và đọc nhanh trên di động.",
            balanceLabel: "Số dư quyền tiêu dùng"
        },
        offers: {
            title: "Hồ sơ quyền ưu đãi",
            copy: "Theo dõi số dư ưu đãi, hành vi đủ điều kiện, các yêu cầu chờ xử lý và lịch sử ghi nhận trong một luồng app thống nhất.",
            balanceLabel: "Số dư quyền ưu đãi"
        },
        labor: {
            title: "Hồ sơ hành vi lao động",
            copy: "Màn hình app-native tập trung vào hành vi lao động, trạng thái hợp lệ và lịch sử chi trả liên quan.",
            balanceLabel: "Số dư hồ sơ lao động"
        },
        engagement: {
            title: "Hồ sơ chỉ số gắn kết",
            copy: "Xem nhanh trạng thái gắn kết, yêu cầu ghi nhận và lịch sử đã phát sinh theo bố cục gọn của app.",
            balanceLabel: "Số dư hồ sơ gắn kết"
        }
    };

    function escapeHtml(text) {
        return String(text || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function getPage() {
        return document.querySelector("[data-app-benefit-page='1']");
    }

    function getProfileKey(page) {
        var key = (page && page.getAttribute("data-benefit-profile") || "").trim().toLowerCase();
        return PROFILE_META[key] ? key : "consumer";
    }

    function getMeta(key) {
        return PROFILE_META[key] || PROFILE_META.consumer;
    }

    function buildCurrentUrl(pageNumber) {
        var url = new URL(window.location.href);
        url.searchParams.set("ui_mode", "app");
        if (pageNumber > 1) {
            url.searchParams.set("page", String(pageNumber));
        } else {
            url.searchParams.delete("page");
        }
        return url.pathname + url.search + url.hash;
    }

    function buildFilteredUrl(periodKey, pageNumber) {
        var url = new URL(window.location.href);
        url.searchParams.set("ui_mode", "app");
        if (periodKey && periodKey !== "all") {
            url.searchParams.set("period", periodKey);
        } else {
            url.searchParams.delete("period");
        }
        if (pageNumber > 1) {
            url.searchParams.set("page", String(pageNumber));
        } else {
            url.searchParams.delete("page");
        }
        return url.pathname + url.search + url.hash;
    }

    function buildLoginUrl() {
        var returnUrl = window.location.pathname + window.location.search;
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + encodeURIComponent(returnUrl);
    }

    function renderHero(page, payload, fallbackMeta) {
        var meta = payload && payload.meta ? payload.meta : {};
        var titleNode = page.querySelector("[data-benefit-title='1']");
        var copyNode = page.querySelector("[data-benefit-copy='1']");
        var balanceLabelNode = page.querySelector("[data-benefit-balance-label='1']");
        var balanceValueNode = page.querySelector("[data-benefit-balance-value='1']");
        var balanceNoteNode = page.querySelector("[data-benefit-balance-note='1']");
        var hero = payload && payload.hero ? payload.hero : null;

        titleNode.textContent = meta.title || fallbackMeta.title;
        copyNode.textContent = meta.description || fallbackMeta.copy;
        balanceLabelNode.textContent = hero && hero.balance_label ? hero.balance_label : fallbackMeta.balanceLabel;
        balanceValueNode.textContent = hero && hero.balance_value ? (hero.balance_value + (hero.balance_suffix ? " " + hero.balance_suffix : "")) : "...";
        balanceNoteNode.textContent = hero && hero.status_text ? hero.status_text : "Đang tải trạng thái hồ sơ...";
    }

    function renderSummary(page, summary) {
        var host = page.querySelector("[data-benefit-summary='1']");
        if (!host) return;
        if (!Array.isArray(summary) || !summary.length) {
            host.innerHTML = "";
            host.hidden = true;
            return;
        }

        host.hidden = false;
        host.innerHTML = summary.map(function (item) {
            return '' +
                '<article class="app-benefit-stat-card" data-tone="' + escapeHtml(item.tone || "default") + '">' +
                '  <span class="app-benefit-stat-label">' + escapeHtml(item.label || "") + '</span>' +
                '  <strong class="app-benefit-stat-value">' + escapeHtml(item.value || "") + '</strong>' +
                '</article>';
        }).join("");
    }

    function renderNotice(page, payload, profileKey) {
        var host = page.querySelector("[data-benefit-notice='1']");
        if (!host) return;

        if (!payload || payload.state === "ready") {
            host.hidden = true;
            host.innerHTML = "";
            return;
        }

        var notice = payload.notice || {};
        var title = "Chưa thể mở hồ sơ";
        var copy = "Hồ sơ này hiện chưa sẵn sàng trên app.";
        var actions = [];

        if (payload.state === "guest") {
            title = "Cần đăng nhập để xem hồ sơ";
            copy = "App cần phiên đăng nhập Home để tải lịch sử và số dư của hồ sơ này.";
            actions = [
                '<a class="app-page-btn is-primary" href="' + escapeHtml(buildLoginUrl()) + '">Đăng nhập</a>',
                '<a class="app-page-btn" href="/app-ui/home/benefits.aspx?ui_mode=app">Xem các hồ sơ khác</a>'
            ];
        } else if (payload.state === "blocked") {
            title = notice.title || "Hồ sơ chưa mở quyền truy cập";
            copy = notice.message || "Tài khoản hiện chưa đạt điều kiện để xem hồ sơ này.";
            actions = [
                '<a class="app-page-btn is-primary" href="/app-ui/home/benefit-offers.aspx?ui_mode=app">Mở hồ sơ quyền ưu đãi</a>',
                '<a class="app-page-btn" href="/app-ui/home/benefits.aspx?ui_mode=app">Quay lại danh sách hồ sơ</a>'
            ];
        } else if (payload.state === "error") {
            title = "Không tải được dữ liệu";
            copy = "Hệ thống chưa trả dữ liệu cho hồ sơ này. Vui lòng thử tải lại sau.";
            actions = [
                '<a class="app-page-btn is-primary" href="' + escapeHtml(buildCurrentUrl(1)) + '">Tải lại</a>'
            ];
        }

        host.className = "app-benefit-notice" + (payload.state === "error" ? " is-danger" : "");
        host.hidden = false;
        host.innerHTML = '' +
            '<div class="app-benefit-notice-head">' +
            '  <h2 class="app-benefit-notice-title">' + escapeHtml(title) + '</h2>' +
            '  <p class="app-benefit-notice-copy">' + escapeHtml(copy) + '</p>' +
            (notice.tier_text ? '<p class="app-benefit-notice-copy">' + escapeHtml(notice.tier_text) + '</p>' : '') +
            '</div>' +
            '<div class="app-page-actions">' + actions.join("") + '</div>';

        if (payload.state !== "ready") {
            page.querySelector("[data-benefit-note-section='1']").hidden = true;
            page.querySelector("[data-benefit-behaviors-section='1']").hidden = true;
            page.querySelector("[data-benefit-requests-section='1']").hidden = true;
            if (payload.state !== "guest") {
                page.querySelector("[data-benefit-history='1']").innerHTML = '<div class="app-benefit-empty">Hồ sơ này chưa có dữ liệu để hiển thị trên app.</div>';
                page.querySelector("[data-benefit-history-meta='1']").textContent = "Chưa khả dụng";
            }
        }
    }

    function renderFilters(page, payload) {
        var host = page.querySelector("[data-benefit-filters='1']");
        if (!host) return;
        var items = payload && Array.isArray(payload.filters) ? payload.filters : [];
        if (!items.length) {
            host.hidden = true;
            host.innerHTML = "";
            return;
        }

        host.hidden = false;
        host.innerHTML = items.map(function (item) {
            var className = "app-benefit-filter-chip" + (item.active ? " is-active" : "");
            return '<a class="' + className + '" href="' + escapeHtml(buildFilteredUrl(item.key || "all", 1)) + '">' + escapeHtml(item.label || "") + '</a>';
        }).join("");
    }

    function renderNote(page, text) {
        var section = page.querySelector("[data-benefit-note-section='1']");
        var node = page.querySelector("[data-benefit-note='1']");
        if (!section || !node) return;
        if (!text) {
            section.hidden = true;
            node.textContent = "";
            return;
        }
        section.hidden = false;
        node.textContent = text;
    }

    function buildRequestHref(profileKey, behaviorCode) {
        return "/app-ui/home/benefit-request.aspx?ui_mode=app&profile=" + encodeURIComponent(profileKey || "") + "&behavior=" + encodeURIComponent(behaviorCode || "");
    }

    function renderBehaviors(page, items, profileKey, activeLabel) {
        var section = page.querySelector("[data-benefit-behaviors-section='1']");
        var host = page.querySelector("[data-benefit-behaviors='1']");
        var meta = page.querySelector("[data-benefit-behaviors-meta='1']");
        if (!section || !host || !meta) return;

        if (!Array.isArray(items) || !items.length) {
            section.hidden = true;
            host.innerHTML = "";
            meta.textContent = "";
            return;
        }

        section.hidden = false;
        meta.textContent = items.length + " nhóm hành vi" + (activeLabel ? " · " + activeLabel : "");
        host.innerHTML = items.map(function (item) {
            return '' +
                '<article class="app-benefit-behavior-card">' +
                '  <div class="app-benefit-row-head">' +
                '    <div class="app-benefit-row-copy">' +
                '      <div class="app-benefit-row-title">' + escapeHtml(item.title || "") + '</div>' +
                '      <div class="app-benefit-row-subtitle">' + escapeHtml(item.rule_text || "") + '</div>' +
                '    </div>' +
                '    <span class="app-benefit-chip">' + escapeHtml(item.code || "") + '</span>' +
                '  </div>' +
                '  <div class="app-benefit-behavior-grid">' +
                '    <div class="app-benefit-mini-stat"><span class="app-benefit-mini-stat-label">Điểm nhận</span><strong class="app-benefit-mini-stat-value">' + escapeHtml(item.earned_text || "") + '</strong></div>' +
                '    <div class="app-benefit-mini-stat"><span class="app-benefit-mini-stat-label">Đủ điều kiện</span><strong class="app-benefit-mini-stat-value">' + escapeHtml(item.eligible_text || "") + '</strong></div>' +
                '    <div class="app-benefit-mini-stat"><span class="app-benefit-mini-stat-label">Chờ duyệt</span><strong class="app-benefit-mini-stat-value">' + escapeHtml(item.pending_text || "") + '</strong></div>' +
                '    <div class="app-benefit-mini-stat"><span class="app-benefit-mini-stat-label">Đã ghi nhận</span><strong class="app-benefit-mini-stat-value">' + escapeHtml(item.recorded_text || "") + '</strong></div>' +
                '  </div>' +
                '  <div class="app-page-actions">' +
                (item.can_submit
                    ? ('<a class="app-page-btn is-primary" href="' + escapeHtml(buildRequestHref(profileKey, item.behavior_code || "")) + '">Gửi yêu cầu</a>')
                    : '<span class="app-page-btn" aria-disabled="true">Chưa đủ điều kiện</span>') +
                '  </div>' +
                '</article>';
        }).join("");
    }

    function renderRequests(page, items, activeLabel) {
        var section = page.querySelector("[data-benefit-requests-section='1']");
        var host = page.querySelector("[data-benefit-requests='1']");
        var meta = page.querySelector("[data-benefit-requests-meta='1']");
        if (!section || !host || !meta) return;

        if (!Array.isArray(items) || !items.length) {
            section.hidden = true;
            host.innerHTML = "";
            meta.textContent = "";
            return;
        }

        section.hidden = false;
        meta.textContent = items.length + " yêu cầu gần nhất" + (activeLabel ? " · " + activeLabel : "");
        host.innerHTML = items.map(function (item) {
            return '' +
                '<article class="app-benefit-request-card">' +
                '  <div class="app-benefit-row-head">' +
                '    <div class="app-benefit-row-copy">' +
                '      <div class="app-benefit-row-title">' + escapeHtml(item.title || "") + '</div>' +
                '      <div class="app-benefit-row-subtitle">Tạo lúc ' + escapeHtml(item.created_at || "") + '</div>' +
                '    </div>' +
                '    <div class="app-benefit-row-copy" style="text-align:right">' +
                '      <div class="app-benefit-row-title">' + escapeHtml(item.amount_text || "") + '</div>' +
                '      <span class="app-benefit-status" data-status="' + escapeHtml(item.status_code || "") + '">' + escapeHtml(item.status_text || "") + '</span>' +
                '    </div>' +
                '  </div>' +
                '  <div class="app-benefit-meta-row">' +
                '    <span class="app-benefit-chip">Cập nhật ' + escapeHtml(item.updated_at || "") + '</span>' +
                '    <span class="app-benefit-chip">' + escapeHtml(item.reviewer || "") + '</span>' +
                '  </div>' +
                (item.note ? '<div class="app-benefit-row-subtitle">' + escapeHtml(item.note) + '</div>' : '') +
                '  <div class="app-page-actions">' +
                (item.detail_url ? '<a class="app-page-btn" href="' + escapeHtml(item.detail_url) + '">Chi tiết yêu cầu</a>' : '') +
                '  </div>' +
                '</article>';
        }).join("");
    }

    function renderHistory(page, items, pagination, activeLabel) {
        var host = page.querySelector("[data-benefit-history='1']");
        var meta = page.querySelector("[data-benefit-history-meta='1']");
        var pager = page.querySelector("[data-benefit-history-pager='1']");
        if (!host || !meta || !pager) return;

        if (!Array.isArray(items) || !items.length) {
            host.innerHTML = '<div class="app-benefit-empty">Chưa có lịch sử nào trong hồ sơ này.</div>';
            meta.textContent = "Không có bản ghi";
            pager.hidden = true;
            pager.innerHTML = "";
            return;
        }

        meta.textContent = (pagination && pagination.label ? pagination.label : (items.length + " mục")) + (activeLabel ? " · " + activeLabel : "");
        host.innerHTML = items.map(function (item) {
            return '' +
                '<article class="app-benefit-history-card">' +
                '  <div class="app-benefit-row-head">' +
                '    <div class="app-benefit-row-copy">' +
                '      <div class="app-benefit-row-title">' + escapeHtml(item.title || "") + '</div>' +
                '      <div class="app-benefit-row-subtitle">' + escapeHtml(item.date_text || "") + '</div>' +
                '    </div>' +
                '    <span class="app-benefit-amount" data-direction="' + escapeHtml(item.direction_code || "") + '">' + escapeHtml(item.amount_text || "") + '</span>' +
                '  </div>' +
                '  <div class="app-benefit-row-subtitle">' + escapeHtml(item.note || "") + '</div>' +
                '  <div class="app-benefit-meta-row">' +
                '    <span class="app-benefit-status" data-status="' + escapeHtml(item.direction_code === "debit" ? "2" : "1") + '">' + escapeHtml(item.direction_text || "") + '</span>' +
                (item.tag ? '<span class="app-benefit-chip">' + escapeHtml(item.tag) + '</span>' : '') +
                (item.order_code ? '<span class="app-benefit-chip">ĐH ' + escapeHtml(item.order_code) + '</span>' : '') +
                '  </div>' +
                '</article>';
        }).join("");

        if (!pagination || pagination.total_pages <= 1) {
            pager.hidden = true;
            pager.innerHTML = "";
            return;
        }

        var prevLink = pagination.can_prev
            ? '<a class="app-page-btn app-benefit-pager-btn" href="' + escapeHtml(buildCurrentUrl(pagination.current_page - 1)) + '">Trang trước</a>'
            : '<span class="app-page-btn app-benefit-pager-btn" aria-disabled="true">Trang trước</span>';
        var nextLink = pagination.can_next
            ? '<a class="app-page-btn is-primary app-benefit-pager-btn" href="' + escapeHtml(buildCurrentUrl(pagination.current_page + 1)) + '">Trang sau</a>'
            : '<span class="app-page-btn app-benefit-pager-btn" aria-disabled="true">Trang sau</span>';

        pager.hidden = false;
        pager.innerHTML = '' +
            '<span class="app-benefit-pager-label">' + escapeHtml(pagination.label || "") + '</span>' +
            '<div class="app-benefit-pager-actions">' + prevLink + nextLink + '</div>';
    }

    function renderFallback(page, key) {
        renderHero(page, null, getMeta(key));
        renderSummary(page, []);
        renderNote(page, "");
        renderBehaviors(page, []);
        renderRequests(page, []);
        renderHistory(page, [], null);
    }

    function hydrate(page, payload, profileKey) {
        var fallbackMeta = getMeta(profileKey);
        renderHero(page, payload, fallbackMeta);
        renderNotice(page, payload, profileKey);
        renderFilters(page, payload);

        if (!payload || payload.state !== "ready") {
            if (payload && payload.state === "guest") {
                renderSummary(page, []);
                renderNote(page, "");
                renderBehaviors(page, [], profileKey, "");
                renderRequests(page, [], "");
                renderHistory(page, [], null, "");
            }
            return;
        }

        renderSummary(page, payload.summary || []);
        renderNote(page, payload.note || "");
        renderBehaviors(page, payload.behaviors || [], profileKey, payload.active_filter_label || "");
        renderRequests(page, payload.requests || [], payload.active_filter_label || "");
        renderHistory(page, payload.history || [], payload.history_pagination || null, payload.active_filter_label || "");
    }

    function load(page) {
        var profileKey = getProfileKey(page);
        renderFallback(page, profileKey);

        if (!window.fetch) {
            return;
        }

        var urlState = new URL(window.location.href);
        var url = "/app-ui/shared/runtime-benefit-history.ashx?profile=" + encodeURIComponent(profileKey);
        var currentPage = new URL(window.location.href).searchParams.get("page");
        var currentPeriod = urlState.searchParams.get("period");
        if (currentPage) {
            url += "&page=" + encodeURIComponent(currentPage);
        }
        if (currentPeriod) {
            url += "&period=" + encodeURIComponent(currentPeriod);
        }

        fetch(url, { credentials: "same-origin" })
            .then(function (response) {
                if (!response.ok) {
                    throw new Error("runtime_failed");
                }
                return response.json();
            })
            .then(function (payload) {
                hydrate(page, payload, profileKey);
            })
            .catch(function () {
                hydrate(page, { state: "error" }, profileKey);
            });
    }

    var page = getPage();
    if (!page) return;
    load(page);
})();
