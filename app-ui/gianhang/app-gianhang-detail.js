(function () {
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
        return url + (url.indexOf("?") >= 0 ? "&" : "?") + key + "=" + encodeURIComponent(value);
    }

    function buildAppHref(pathname) {
        if (!pathname) return "/app-ui/gianhang/default.aspx?ui_mode=app";
        return appendQuery(pathname, "back_href", getCurrentAppHref());
    }

    function parseRuntimeId(item) {
        if (!item) return 0;
        if (item.runtime_id && !isNaN(Number(item.runtime_id))) return Number(item.runtime_id);
        var raw = (item.id || "").toString();
        raw = raw.replace(/^runtime-gh-/i, "");
        var num = Number(raw);
        return isNaN(num) ? 0 : num;
    }

    function getManageHref(runtimeId) {
        var base = buildAppHref("/app-ui/gianhang/post.aspx?ui_mode=app");
        if (runtimeId > 0) {
            return appendQuery(base, "runtime_id", String(runtimeId));
        }
        return base;
    }

    function resolveQuickActionHref(label, runtimeId) {
        var text = (label || "").toLowerCase();
        if (text.indexOf("sửa") >= 0 || text.indexOf("noi dung") >= 0 || text.indexOf("nội dung") >= 0) {
            return getManageHref(runtimeId);
        }
        if (text.indexOf("đẩy tin") >= 0 || text.indexOf("day tin") >= 0) {
            return buildAppHref("/app-ui/gianhang/listing.aspx?ui_mode=app");
        }
        if (text.indexOf("ẩn") >= 0 || text.indexOf("hien thi lai") >= 0 || text.indexOf("hiển thị lại") >= 0) {
            return buildAppHref("/app-ui/gianhang/listing.aspx?ui_mode=app");
        }
        return buildAppHref("/app-ui/gianhang/default.aspx?ui_mode=app");
    }

    async function run() {
        var id = getQuery("id");
        var host = document.querySelector("[data-role='gianhang-detail-host']");
        if (!host) return;
        var item = await window.AhaAppUiAdapters.listing.getByIdAsync(id, "gianhang");

        if (!item) {
            host.innerHTML = '' +
                '<section class="app-section">' +
                '  <div class="app-empty-state">' +
                '    <div class="app-empty-icon">!</div>' +
                '    <h1 class="app-empty-title">Không tìm thấy listing</h1>' +
                '    <p class="app-empty-copy">Listing không còn khả dụng hoặc tài khoản chưa có quyền xem chi tiết tin này.</p>' +
                '    <div class="app-empty-actions">' +
                '      <a class="app-ghost-btn" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '">Mở quản lý tin</a>' +
                '      <a class="app-ghost-btn" href="' + buildAppHref('/app-ui/gianhang/default.aspx?ui_mode=app') + '">Về gian hàng</a>' +
                '    </div>' +
                '  </div>' +
                '</section>';
            return;
        }

        var runtimeId = parseRuntimeId(item);
        var manageHref = getManageHref(runtimeId);
        var listingRuntimeHref = buildAppHref("/app-ui/gianhang/listing.aspx?ui_mode=app");
        var ordersHref = buildAppHref("/app-ui/gianhang/orders.aspx?ui_mode=app");
        var createOrderHref = buildAppHref("/app-ui/gianhang/create-order.aspx?ui_mode=app");
        var pendingPaymentHref = buildAppHref("/app-ui/gianhang/pending-payment.aspx?ui_mode=app");
        var customerHref = buildAppHref("/app-ui/gianhang/conversations.aspx?ui_mode=app");
        var storefrontHref = buildAppHref("/app-ui/gianhang/storefront.aspx?ui_mode=app");

    function renderLines(items, emptyText) {
        if (!items || !items.length) {
            return '<div class="app-detail-summary">' + emptyText + '</div>';
        }
        return '<div class="app-detail-stack">' + items.map(function (row) {
            return '<div class="app-detail-line-item">' + row + '</div>';
        }).join("") + '</div>';
    }

        host.innerHTML = '' +
        '<section class="app-section">' +
        '  <div class="app-detail-hero" style="background:linear-gradient(135deg,var(--app-hero-start),var(--app-hero-mid) 58%,var(--app-hero-end) 115%)"></div>' +
        '  <div class="app-detail-body">' +
        '    <div class="app-detail-category">' + item.category + '</div>' +
        '    <h1 class="app-detail-title">' + item.title + '</h1>' +
        '    <div class="app-detail-meta">' + item.status + ' • ' + item.updatedAt + '</div>' +
        '    <div class="app-detail-summary">' + (item.summary || "Listing đang được quản lý trong seller cockpit.") + '</div>' +
        '    <div class="app-detail-badge-row"><span class="app-detail-pill">' + item.status + '</span><span class="app-detail-pill">' + item.category + '</span></div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Kênh publish</h2>' +
        '      <div class="app-chip-lane">' +
                (item.publishTargets || []).map(function (target) {
                    return '<span class="app-chip">' + target + '</span>';
                }).join("") +
        '      </div>' +
        '    </div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Tóm tắt điều hành</h2>' +
        '      <div class="app-detail-dual-grid">' +
        '        <div class="app-detail-mini-card"><span class="app-detail-mini-label">Trạng thái</span><span class="app-detail-mini-value">' + item.status + '</span></div>' +
        '        <div class="app-detail-mini-card"><span class="app-detail-mini-label">Cập nhật</span><span class="app-detail-mini-value">' + item.updatedAt + '</span></div>' +
        '      </div>' +
        '    </div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Vận hành nhanh</h2>' +
        '      <div class="app-detail-summary">Dùng các nút bên dưới để mở trực tiếp luồng xử lý: sửa tin, quản lý đơn và xử lý khách hàng.</div>' +
        '    </div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Ghi chú duyệt</h2>' +
        '      <div class="app-chip-lane">' +
                (item.reviewNotes || []).map(function (note) {
                    return '<span class="app-chip">' + note + '</span>';
                }).join("") +
        '      </div>' +
        '    </div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Lead cần xử lý</h2>' +
                renderLines(item.leads || [], 'Chưa có lead mới trong listing này.') +
        '    </div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Thao tác nhanh</h2>' +
        '      <div class="app-chip-lane">' +
                (item.quickActions || []).map(function (action) {
                    return '<a class="app-chip" href="' + resolveQuickActionHref(action, runtimeId) + '">' + action + '</a>';
                }).join("") +
        '      </div>' +
        '    </div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Lịch sử publish</h2>' +
                renderLines(item.publishHistory || [], 'Chưa có lịch sử publish.') +
        '    </div>' +
        '    <div class="app-detail-panel">' +
        '      <h2 class="app-detail-panel-title">Checklist hôm nay</h2>' +
                renderLines(item.checklist || [], 'Không có việc đang mở.') +
        '    </div>' +
        '    <div class="app-detail-cta-row">' +
        '      <a class="app-detail-btn is-primary" href="' + manageHref + '">Sửa tin</a>' +
        '      <a class="app-detail-btn is-secondary" href="' + listingRuntimeHref + '">Quản lý tin</a>' +
        '    </div>' +
        '    <div class="app-detail-cta-row">' +
        '      <a class="app-detail-btn is-secondary" href="' + createOrderHref + '">Tạo đơn</a>' +
        '      <a class="app-detail-btn is-secondary" href="' + pendingPaymentHref + '">Chờ trao đổi</a>' +
        '    </div>' +
        '    <div class="app-detail-cta-row">' +
        '      <a class="app-detail-btn is-secondary" href="' + ordersHref + '">Đơn bán</a>' +
        '      <a class="app-detail-btn is-secondary" href="' + customerHref + '">Khách hàng</a>' +
        '    </div>' +
        '    <div class="app-detail-cta-row">' +
        '      <a class="app-detail-btn is-secondary" href="' + storefrontHref + '">Trang công khai</a>' +
        '    </div>' +
        '  </div>' +
        '</section>';
    }

    run();
})();
