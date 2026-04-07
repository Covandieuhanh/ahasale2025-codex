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
        return document.querySelector("[data-benefit-request-page='1']");
    }

    function getParams() {
        var url = new URL(window.location.href);
        return {
            profile: (url.searchParams.get("profile") || "").trim().toLowerCase(),
            behavior: (url.searchParams.get("behavior") || "").trim()
        };
    }

    function buildRuntimeUrl(params) {
        return "/app-ui/shared/runtime-benefit-request.ashx?profile=" + encodeURIComponent(params.profile || "") + "&behavior=" + encodeURIComponent(params.behavior || "");
    }

    function buildPageUrl(params) {
        return "/app-ui/home/benefit-request.aspx?ui_mode=app&profile=" + encodeURIComponent(params.profile || "") + "&behavior=" + encodeURIComponent(params.behavior || "");
    }

    function buildLoginUrl() {
        var returnUrl = window.location.pathname + window.location.search;
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + encodeURIComponent(returnUrl);
    }

    function renderSummary(page, items) {
        var host = page.querySelector("[data-benefit-request-summary='1']");
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
        var host = page.querySelector("[data-benefit-request-notice='1']");
        if (!host) return;
        if (!title && !message) {
            host.hidden = true;
            host.innerHTML = "";
            return;
        }
        host.className = "app-benefit-notice" + (state === "error" || state === "invalid" ? " is-danger" : "");
        host.hidden = false;
        host.innerHTML = '' +
            '<div class="app-benefit-notice-head">' +
            '  <h2 class="app-benefit-notice-title">' + escapeHtml(title || "") + '</h2>' +
            '  <p class="app-benefit-notice-copy">' + escapeHtml(message || "") + '</p>' +
            '</div>' +
            '<div class="app-page-actions">' + (actions || []).join("") + '</div>';
    }

    function renderRecentRequests(page, items) {
        var host = page.querySelector("[data-benefit-request-recent='1']");
        var meta = page.querySelector("[data-benefit-request-recent-meta='1']");
        if (!host || !meta) return;
        if (!Array.isArray(items) || !items.length) {
            host.innerHTML = '<div class="app-benefit-empty">Chưa có yêu cầu nào cho hành vi này.</div>';
            meta.textContent = "Chưa phát sinh";
            return;
        }

        meta.textContent = items.length + " yêu cầu gần nhất";
        host.innerHTML = items.map(function (item) {
            return '' +
                '<article class="app-benefit-request-card">' +
                '  <div class="app-benefit-row-head">' +
                '    <div class="app-benefit-row-copy">' +
                '      <div class="app-benefit-row-title">' + escapeHtml(item.amount_text || "") + '</div>' +
                '      <div class="app-benefit-row-subtitle">Tạo lúc ' + escapeHtml(item.created_at || "") + '</div>' +
                '    </div>' +
                '    <span class="app-benefit-status" data-status="' + escapeHtml(item.status_code || "") + '">' + escapeHtml(item.status_text || "") + '</span>' +
                '  </div>' +
                '  <div class="app-benefit-row-subtitle">Cập nhật ' + escapeHtml(item.updated_at || "") + '</div>' +
                (item.note ? '<div class="app-benefit-row-subtitle">' + escapeHtml(item.note) + '</div>' : '') +
                '  <div class="app-page-actions">' +
                (item.detail_url ? '<a class="app-page-btn" href="' + escapeHtml(item.detail_url) + '">Chi tiết yêu cầu</a>' : '') +
                '  </div>' +
                '</article>';
        }).join("");
    }

    function setLoading(button, active) {
        if (!button) return;
        if (active) {
            button.dataset.prevText = button.textContent || "Gửi yêu cầu";
            button.textContent = "Đang gửi...";
            button.disabled = true;
            return;
        }
        button.textContent = button.dataset.prevText || "Gửi yêu cầu";
        button.disabled = false;
    }

    function renderQuickActions(page, maxValue) {
        var host = page.querySelector("[data-benefit-request-quick-actions='1']");
        var input = page.querySelector("[data-benefit-request-amount='1']");
        if (!host || !input || !(maxValue > 0)) {
            if (host) host.innerHTML = "";
            return;
        }

        var values = [
            { label: "25%", value: Math.max(0, Math.round(maxValue * 25) / 100) },
            { label: "50%", value: Math.max(0, Math.round(maxValue * 50) / 100) },
            { label: "Toàn bộ", value: maxValue }
        ].filter(function (item, index, list) {
            return item.value > 0 && list.findIndex(function (other) { return other.value === item.value; }) === index;
        });

        host.innerHTML = values.map(function (item) {
            return '<button class="app-page-btn" type="button" data-fill-amount="' + escapeHtml(String(item.value)) + '">' + escapeHtml(item.label) + '</button>';
        }).join("");

        Array.prototype.forEach.call(host.querySelectorAll("[data-fill-amount]"), function (button) {
            button.addEventListener("click", function () {
                input.value = button.getAttribute("data-fill-amount") || "";
                input.focus();
            });
        });
    }

    function hydrateReady(page, payload, params) {
        var profile = payload.profile || {};
        var behavior = payload.behavior || {};
        var titleNode = page.querySelector("[data-benefit-request-title='1']");
        var copyNode = page.querySelector("[data-benefit-request-copy='1']");
        var balanceLabelNode = page.querySelector("[data-benefit-request-balance-label='1']");
        var balanceValueNode = page.querySelector("[data-benefit-request-balance-value='1']");
        var balanceNoteNode = page.querySelector("[data-benefit-request-balance-note='1']");
        var metaNode = page.querySelector("[data-benefit-request-form-meta='1']");
        var profileNode = page.querySelector("[data-benefit-request-profile='1']");
        var behaviorNode = page.querySelector("[data-benefit-request-behavior='1']");
        var ruleNode = page.querySelector("[data-benefit-request-rule='1']");
        var helpNode = page.querySelector("[data-benefit-request-help='1']");
        var backLink = page.querySelector("[data-benefit-request-back='1']");
        var submitButton = page.querySelector("[data-benefit-request-submit='1']");
        var input = page.querySelector("[data-benefit-request-amount='1']");

        titleNode.textContent = "Gửi yêu cầu cho " + (behavior.title || "hành vi");
        copyNode.textContent = profile.description || "Điền số điểm cần gửi để hệ thống duyệt theo mốc hành vi.";
        balanceLabelNode.textContent = "Số dư có thể gửi";
        balanceValueNode.textContent = behavior.eligible_text || "...";
        balanceNoteNode.textContent = behavior.rule_text || "Đang kiểm tra điều kiện hành vi...";
        metaNode.textContent = behavior.can_submit ? "Đã sẵn sàng để gửi" : "Chưa đủ điều kiện";
        profileNode.textContent = profile.title || "...";
        behaviorNode.textContent = (behavior.title || "...") + (behavior.code ? " · " + behavior.code : "");
        ruleNode.textContent = behavior.rule_text || "...";
        helpNode.textContent = payload.note || "Hệ thống sẽ duyệt dựa trên số dư hành vi hợp lệ hiện tại.";
        backLink.href = profile.history_url || "/app-ui/home/benefits.aspx?ui_mode=app";

        renderSummary(page, [
            { label: "Hành vi đủ điều kiện", value: behavior.eligible_text || "0 A", tone: "accent" },
            { label: "Điểm nhận", value: behavior.earned_text || "0", tone: "default" },
            { label: "Chờ duyệt", value: behavior.pending_text || "0", tone: "warning" },
            { label: "Đã ghi nhận", value: behavior.recorded_text || "0", tone: "positive" }
        ]);
        renderRecentRequests(page, payload.recent_requests || []);
        renderQuickActions(page, Number(behavior.eligible_value || 0));
        renderNotice(page, "", "", "", []);

        submitButton.disabled = !behavior.can_submit;
        input.disabled = !behavior.can_submit;
        if (!behavior.can_submit) {
            renderNotice(page, "warning", "Hành vi chưa đủ điều kiện", "Hành vi này hiện chưa có số dư hợp lệ để gửi yêu cầu. Khi đủ mốc ngày, tuần hoặc tháng tương ứng, nút gửi sẽ khả dụng.", [
                '<a class="app-page-btn" href="' + escapeHtml(profile.history_url || "/app-ui/home/benefits.aspx?ui_mode=app") + '">Quay lại hồ sơ</a>'
            ]);
        }

        submitButton.onclick = function () {
            var rawAmount = (input.value || "").replace(/,/g, "").trim();
            if (!rawAmount) {
                renderNotice(page, "warning", "Thiếu số điểm", "Vui lòng nhập số điểm muốn gửi yêu cầu ghi nhận.", []);
                return;
            }

            var amount = Number(rawAmount);
            var maxValue = Number(behavior.eligible_value || 0);
            if (!(amount > 0)) {
                renderNotice(page, "warning", "Số điểm không hợp lệ", "Số điểm yêu cầu phải lớn hơn 0.", []);
                return;
            }
            if (maxValue > 0 && amount > maxValue) {
                renderNotice(page, "warning", "Vượt số dư hợp lệ", "Số điểm yêu cầu đang vượt quá số dư hành vi hợp lệ hiện tại.", []);
                return;
            }

            setLoading(submitButton, true);
            var body = new URLSearchParams();
            body.set("profile", params.profile || "");
            body.set("behavior", params.behavior || "");
            body.set("amount", rawAmount);

            fetch("/app-ui/shared/runtime-benefit-request.ashx?ui_mode=app", {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
                },
                body: body.toString()
            }).then(function (response) {
                return response.text().then(function (raw) {
                    var data = null;
                    try { data = raw ? JSON.parse(raw) : null; } catch (e) { data = null; }
                    return { ok: response.ok, data: data || {} };
                });
            }).then(function (result) {
                setLoading(submitButton, false);
                if (!result.ok || !result.data || !result.data.ok) {
                    renderNotice(page, "warning", "Không gửi được yêu cầu", (result.data && result.data.message) || "Yêu cầu chưa được gửi. Vui lòng thử lại.", []);
                    return;
                }

                input.value = "";
                renderNotice(page, "success", "Đã gửi yêu cầu", result.data.message || "Yêu cầu đã được ghi nhận.", [
                    '<a class="app-page-btn is-primary" href="' + escapeHtml((result.data.redirect_url || profile.history_url || "/app-ui/home/benefits.aspx?ui_mode=app")) + '">Về lịch sử hồ sơ</a>',
                    '<a class="app-page-btn" href="' + escapeHtml(buildPageUrl(params)) + '">Tạo yêu cầu khác</a>'
                ]);
                load(page);
            }).catch(function () {
                setLoading(submitButton, false);
                renderNotice(page, "error", "Lỗi gửi yêu cầu", "Không kết nối được tới hệ thống duyệt điểm. Vui lòng thử lại sau.", []);
            });
        };
    }

    function load(page) {
        var params = getParams();
        var recentSection = page.querySelector("[data-benefit-request-recent-section='1']");
        var formSection = page.querySelector("[data-benefit-request-form-section='1']");
        var summarySection = page.querySelector("[data-benefit-request-summary='1']");
        fetch(buildRuntimeUrl(params), {
            method: "GET",
            credentials: "same-origin",
            cache: "no-store"
        }).then(function (response) {
            if (!response.ok) throw new Error("runtime_request_http");
            return response.json();
        }).then(function (payload) {
            if (!payload || payload.state === "guest") {
                renderSummary(page, []);
                renderRecentRequests(page, []);
                renderNotice(page, "warning", "Cần đăng nhập để gửi yêu cầu", "Bạn cần đăng nhập Home để gửi yêu cầu ghi nhận hành vi trên app.", [
                    '<a class="app-page-btn is-primary" href="' + escapeHtml(buildLoginUrl()) + '">Đăng nhập</a>',
                    '<a class="app-page-btn" href="/app-ui/home/benefits.aspx?ui_mode=app">Quay lại hồ sơ</a>'
                ]);
                if (formSection) formSection.hidden = true;
                if (summarySection) summarySection.hidden = true;
                recentSection.hidden = true;
                return;
            }

            if (payload.state === "blocked" || payload.state === "invalid" || payload.state === "error") {
                renderSummary(page, []);
                renderRecentRequests(page, []);
                renderNotice(page, payload.state, payload.state === "blocked" ? "Chưa mở quyền gửi yêu cầu" : "Không mở được màn yêu cầu", (payload.message || "Luồng yêu cầu chưa khả dụng."), [
                    '<a class="app-page-btn" href="/app-ui/home/benefits.aspx?ui_mode=app">Quay lại hồ sơ</a>'
                ]);
                if (formSection) formSection.hidden = true;
                if (summarySection) summarySection.hidden = true;
                recentSection.hidden = true;
                return;
            }

            if (formSection) formSection.hidden = false;
            if (summarySection) summarySection.hidden = false;
            recentSection.hidden = false;
            hydrateReady(page, payload, params);
        }).catch(function () {
            renderSummary(page, []);
            renderRecentRequests(page, []);
            renderNotice(page, "error", "Không tải được dữ liệu", "Hệ thống chưa trả được dữ liệu cho màn gửi yêu cầu này.", [
                '<a class="app-page-btn is-primary" href="' + escapeHtml(window.location.pathname + window.location.search) + '">Tải lại</a>'
            ]);
            if (formSection) formSection.hidden = true;
            if (summarySection) summarySection.hidden = true;
            if (recentSection) recentSection.hidden = true;
        });
    }

    var page = getPage();
    if (!page) return;
    load(page);
})();
