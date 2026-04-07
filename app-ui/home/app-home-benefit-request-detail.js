(function () {
    function escapeHtml(text) {
        return String(text || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function getPage() {
        return document.querySelector("[data-benefit-request-detail-page='1']");
    }

    function getParams() {
        var url = new URL(window.location.href);
        return {
            profile: (url.searchParams.get("profile") || "").trim().toLowerCase(),
            requestId: (url.searchParams.get("request_id") || "").trim()
        };
    }

    function buildRuntimeUrl(params) {
        return "/app-ui/shared/runtime-benefit-request-detail.ashx?profile=" + encodeURIComponent(params.profile || "") + "&request_id=" + encodeURIComponent(params.requestId || "");
    }

    function buildLoginUrl() {
        var returnUrl = window.location.pathname + window.location.search;
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + encodeURIComponent(returnUrl);
    }

    function renderSummary(page, items) {
        var host = page.querySelector("[data-benefit-detail-summary='1']");
        if (!host) return;
        if (!Array.isArray(items) || !items.length) {
            host.hidden = true;
            host.innerHTML = "";
            return;
        }
        host.hidden = false;
        host.innerHTML = items.map(function (item) {
            return '' +
                '<article class="app-benefit-stat-card" data-tone="' + escapeHtml(item.tone || "default") + '">' +
                '  <span class="app-benefit-stat-label">' + escapeHtml(item.label || "") + '</span>' +
                '  <strong class="app-benefit-stat-value">' + escapeHtml(item.value || "") + '</strong>' +
                '</article>';
        }).join("");
    }

    function renderNotice(page, state, title, message, actions) {
        var host = page.querySelector("[data-benefit-detail-notice='1']");
        if (!host) return;
        if (!title) {
            host.hidden = true;
            host.innerHTML = "";
            return;
        }
        host.className = "app-benefit-notice" + (state === "error" || state === "invalid" ? " is-danger" : "");
        host.hidden = false;
        host.innerHTML = '' +
            '<div class="app-benefit-notice-head">' +
            '  <h2 class="app-benefit-notice-title">' + escapeHtml(title) + '</h2>' +
            '  <p class="app-benefit-notice-copy">' + escapeHtml(message || "") + '</p>' +
            '</div>' +
            '<div class="app-page-actions">' + (actions || []).join("") + '</div>';
    }

    function renderReady(page, payload) {
        var request = payload.request || {};
        var profile = payload.profile || {};
        page.querySelector("[data-benefit-detail-title='1']").textContent = request.title || "Chi tiết yêu cầu";
        page.querySelector("[data-benefit-detail-copy='1']").textContent = (profile.title || "Hồ sơ") + " · chu kỳ " + (request.period_text || "đang cập nhật") + ".";
        page.querySelector("[data-benefit-detail-status='1']").textContent = request.status_text || "...";
        page.querySelector("[data-benefit-detail-short-id='1']").textContent = "Mã yêu cầu #" + (request.short_id || "...");

        renderSummary(page, payload.stats || []);
        renderNotice(page, "", "", "", []);

        var fields = page.querySelector("[data-benefit-detail-fields='1']");
        fields.innerHTML = '' +
            '<div class="app-benefit-request-field"><span class="app-benefit-request-label">Hồ sơ</span><strong class="app-benefit-request-value">' + escapeHtml(profile.title || "") + '</strong></div>' +
            '<div class="app-benefit-request-field"><span class="app-benefit-request-label">Hành vi</span><strong class="app-benefit-request-value">' + escapeHtml(request.title || "") + (request.behavior_code ? ' · HV' + escapeHtml(request.behavior_code) : '') + '</strong></div>' +
            '<div class="app-benefit-request-field"><span class="app-benefit-request-label">Số điểm yêu cầu</span><strong class="app-benefit-request-value">' + escapeHtml(request.amount_text || "") + '</strong></div>' +
            '<div class="app-benefit-request-field"><span class="app-benefit-request-label">Người duyệt</span><strong class="app-benefit-request-value">' + escapeHtml(request.reviewer || "") + '</strong></div>' +
            '<div class="app-benefit-request-field"><span class="app-benefit-request-label">Thời gian tạo</span><strong class="app-benefit-request-value">' + escapeHtml(request.created_at || "") + '</strong></div>' +
            '<div class="app-benefit-request-field"><span class="app-benefit-request-label">Cập nhật gần nhất</span><strong class="app-benefit-request-value">' + escapeHtml(request.updated_at || "") + '</strong></div>' +
            '<div class="app-benefit-request-field"><span class="app-benefit-request-label">Ghi chú</span><strong class="app-benefit-request-value">' + escapeHtml(request.note || "") + '</strong></div>';

        page.querySelector("[data-benefit-detail-actions='1']").innerHTML = '' +
            '<a class="app-page-btn is-primary" href="' + escapeHtml(profile.history_url || "/app-ui/home/benefits.aspx?ui_mode=app") + '">Về hồ sơ</a>' +
            '<a class="app-page-btn" href="/app-ui/home/benefit-request.aspx?ui_mode=app&profile=' + escapeHtml(profile.key || "") + '&behavior=' + escapeHtml(request.behavior_code || "") + '">Tạo yêu cầu cùng hành vi</a>';

        var timeline = page.querySelector("[data-benefit-detail-timeline='1']");
        timeline.innerHTML = (payload.timeline || []).map(function (item) {
            return '' +
                '<article class="app-benefit-history-card">' +
                '  <div class="app-benefit-row-title">' + escapeHtml(item.title || "") + '</div>' +
                '  <div class="app-benefit-row-subtitle">' + escapeHtml(item.text || "") + '</div>' +
                '  <div class="app-benefit-meta-row"><span class="app-benefit-status" data-status="' + escapeHtml(item.tone || "default") + '">' + escapeHtml(item.text || "") + '</span></div>' +
                '</article>';
        }).join("") || '<div class="app-benefit-empty">Chưa có diễn biến nào để hiển thị.</div>';

        var history = page.querySelector("[data-benefit-detail-history='1']");
        history.innerHTML = (payload.related_history || []).map(function (item) {
            return '' +
                '<article class="app-benefit-history-card">' +
                '  <div class="app-benefit-row-head">' +
                '    <div class="app-benefit-row-copy">' +
                '      <div class="app-benefit-row-title">' + escapeHtml(item.title || "") + '</div>' +
                '      <div class="app-benefit-row-subtitle">' + escapeHtml(item.date_text || "") + '</div>' +
                '    </div>' +
                '    <span class="app-benefit-amount" data-direction="' + ((item.amount_text || "").indexOf("-") === 0 ? "debit" : "credit") + '">' + escapeHtml(item.amount_text || "") + '</span>' +
                '  </div>' +
                (item.note ? '<div class="app-benefit-row-subtitle">' + escapeHtml(item.note) + '</div>' : '') +
                '</article>';
        }).join("") || '<div class="app-benefit-empty">Chưa có biến động liên quan nào.</div>';
    }

    function load(page) {
        var params = getParams();
        var mainSection = page.querySelector("[data-benefit-detail-main='1']");
        var timelineSection = page.querySelector("[data-benefit-detail-timeline-section='1']");
        var historySection = page.querySelector("[data-benefit-detail-history-section='1']");
        fetch(buildRuntimeUrl(params), {
            method: "GET",
            credentials: "same-origin",
            cache: "no-store"
        }).then(function (response) {
            if (!response.ok) throw new Error("runtime_detail_http");
            return response.json();
        }).then(function (payload) {
            if (payload && payload.state === "ready") {
                mainSection.hidden = false;
                timelineSection.hidden = false;
                historySection.hidden = false;
                renderReady(page, payload);
                return;
            }

            renderSummary(page, []);
            mainSection.hidden = true;
            timelineSection.hidden = true;
            historySection.hidden = true;
            renderNotice(page, payload && payload.state || "error",
                payload && payload.state === "guest" ? "Cần đăng nhập để xem chi tiết" : "Không mở được chi tiết yêu cầu",
                payload && payload.message ? payload.message : "Chi tiết yêu cầu chưa khả dụng.",
                [
                    payload && payload.state === "guest"
                        ? '<a class="app-page-btn is-primary" href="' + escapeHtml(buildLoginUrl()) + '">Đăng nhập</a>'
                        : '<a class="app-page-btn" href="/app-ui/home/benefits.aspx?ui_mode=app">Quay lại hồ sơ</a>'
                ]);
        }).catch(function () {
            renderSummary(page, []);
            mainSection.hidden = true;
            timelineSection.hidden = true;
            historySection.hidden = true;
            renderNotice(page, "error", "Không tải được chi tiết yêu cầu", "Hệ thống chưa trả được dữ liệu cho màn này.", [
                '<a class="app-page-btn is-primary" href="' + escapeHtml(window.location.pathname + window.location.search) + '">Tải lại</a>'
            ]);
        });
    }

    var page = getPage();
    if (!page) return;
    load(page);
})();
