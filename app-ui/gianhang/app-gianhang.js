(function () {
    var uiScaffoldRoot = (window.AhaAppUiData && window.AhaAppUiData.gianhang) || {
        quickActions: [],
        metrics: [],
        statusTabs: [],
        listings: [],
        conversations: []
    };

    function getSession() {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        return adapter && typeof adapter.getSession === "function"
            ? adapter.getSession()
            : { auth_state: "guest", seller_status: "not_registered" };
    }

    function getPostTargetHref() {
        var target = window.AhaAppUi && typeof window.AhaAppUi.getPostTarget === "function"
            ? window.AhaAppUi.getPostTarget(getSession())
            : null;
        return target && target.href ? target.href : "/app-ui/auth/open-shop.aspx?ui_mode=app";
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

    function resolveQuickActionHref(item) {
        var label = item && item.label ? item.label : "";
        var text = (label || "").toLowerCase();
        if (text.indexOf("quản lý tin") >= 0 || text.indexOf("quan ly tin") >= 0) {
            return buildAppHref("/app-ui/gianhang/listing.aspx?ui_mode=app");
        }
        if (text.indexOf("tạo đơn") >= 0 || text.indexOf("tao don") >= 0) {
            return buildAppHref("/app-ui/gianhang/create-order.aspx?ui_mode=app");
        }
        if (text.indexOf("chờ thanh toán") >= 0 || text.indexOf("cho thanh toan") >= 0
            || text.indexOf("chờ trao đổi") >= 0 || text.indexOf("cho trao doi") >= 0) {
            return buildAppHref("/app-ui/gianhang/pending-payment.aspx?ui_mode=app");
        }
        if (text.indexOf("lịch hẹn") >= 0 || text.indexOf("lich hen") >= 0) {
            return buildAppHref("/app-ui/gianhang/appointments.aspx?ui_mode=app");
        }
        if (text.indexOf("khách hàng") >= 0 || text.indexOf("khach hang") >= 0) {
            return buildAppHref("/app-ui/gianhang/conversations.aspx?ui_mode=app");
        }
        if (text.indexOf("mở quản trị") >= 0 || text.indexOf("mo quan tri") >= 0) {
            return buildAppHref("/app-ui/gianhang/status.aspx?ui_mode=app");
        }
        if (text.indexOf("đăng tin") >= 0 || text.indexOf("dang tin") >= 0) {
            return buildAppHref("/app-ui/gianhang/post.aspx?ui_mode=app");
        }
        if (item && item.href) {
            if (item.href.indexOf("/app-ui/") === 0) return buildAppHref(item.href);
        }
        return buildAppHref("/app-ui/gianhang/default.aspx?ui_mode=app");
    }

    function renderQuickActions(items) {
        var host = document.querySelector('[data-role="gh-quick-actions"]');
        if (!host) return;
        host.innerHTML = items.map(function (item) {
            var tone = item.tone === "primary" ? " is-primary" : "";
            var href = resolveQuickActionHref(item);
            return '<a class="app-quick-action-card' + tone + '" href="' + href + '">' + item.label + '</a>';
        }).join("");
    }

    function renderMetrics(items) {
        var host = document.querySelector('[data-role="gh-metrics"]');
        if (!host) return;
        host.innerHTML = items.map(function (item) {
            return '' +
                '<article class="app-metric-card">' +
                '  <span class="app-metric-value">' + item.value + '</span>' +
                '  <span class="app-metric-label">' + item.label + '</span>' +
                '</article>';
        }).join("");
    }

    function renderHeroMetrics(items) {
        var host = document.querySelector('[data-role="gh-hero-stats"]');
        if (!host) return;
        var session = getSession();
        var sellerStatus = (session.seller_status || "not_registered").toLowerCase();
        var cards = items && items.length ? items.slice(0, 3) : [
            { value: sellerStatus === "approved" ? "0" : "--", label: "Tin đăng hoạt động" },
            { value: sellerStatus === "approved" ? "0" : "--", label: "Khách cần phản hồi" },
            { value: sellerStatus === "approved" ? "0" : "--", label: "Lịch hẹn hôm nay" }
        ];
        host.innerHTML = cards.map(function (item) {
            return '' +
                '<article class="app-hero-stat-card">' +
                '  <span class="app-hero-stat-value">' + item.value + '</span>' +
                '  <span class="app-hero-stat-label">' + item.label + '</span>' +
                '</article>';
        }).join("");
    }

    function renderFocusStrip(items, conversations) {
        var host = document.querySelector('[data-role="gh-focus-strip"]');
        if (!host) return;
        var session = getSession();
        var sellerStatus = (session.seller_status || "not_registered").toLowerCase();
        var firstListing = items && items[0];
        var firstConversation = conversations && conversations[0];
        var statusText = sellerStatus === "approved"
            ? "Gian hàng đang hoạt động theo dữ liệu hiện tại."
            : sellerStatus === "pending"
                ? "Gian hàng đang chờ duyệt, các thao tác sẽ bật đầy đủ sau khi được duyệt."
                : "Tài khoản chưa có gian hàng hoạt động, app đang giữ đúng luồng đăng ký.";
        host.innerHTML = '' +
            '<article class="app-gianhang-focus-card">' +
            '  <span class="app-gianhang-focus-label">Mục tiêu hôm nay</span>' +
            '  <span class="app-gianhang-focus-value">' + (firstListing ? firstListing.title : 'Ưu tiên kích hoạt luồng bán hàng') + '</span>' +
            '</article>' +
            '<article class="app-gianhang-focus-card">' +
            '  <span class="app-gianhang-focus-label">Trạng thái sàn</span>' +
            '  <span class="app-gianhang-focus-value">' + (firstConversation ? firstConversation.topic : statusText) + '</span>' +
            '</article>';
    }

    function renderPriority(items, conversations) {
        var host = document.querySelector('[data-role="gh-priority-strip"]');
        if (!host) return;
        var firstListing = items && items[0];
        var firstConversation = conversations && conversations[0];
        host.innerHTML = '' +
            '<article class="app-gianhang-priority-card">' +
            '  <span class="app-gianhang-priority-eyebrow">Ưu tiên</span>' +
            '  <h3 class="app-gianhang-priority-title">' + (firstListing ? firstListing.title : 'Chưa có listing ưu tiên') + '</h3>' +
            '  <p class="app-gianhang-priority-copy">' + (firstListing ? firstListing.summary : 'Không có việc ưu tiên trong seller cockpit.') + '</p>' +
            '</article>' +
            '<article class="app-gianhang-priority-card">' +
            '  <span class="app-gianhang-priority-eyebrow">Khách hàng</span>' +
            '  <h3 class="app-gianhang-priority-title">' + (firstConversation ? firstConversation.name : 'Chưa có hội thoại mới') + '</h3>' +
            '  <p class="app-gianhang-priority-copy">' + (firstConversation ? firstConversation.topic : 'Luồng trao đổi đang trống.') + '</p>' +
            '</article>';
    }

    function renderTabs(items) {
        var host = document.querySelector('[data-role="gh-status-tabs"]');
        if (!host) return;
        host.innerHTML = items.map(function (item, index) {
            var href = '/app-ui/gianhang/status.aspx?ui_mode=app&tab=' + encodeURIComponent(item);
            return '<a class="app-feed-tab' + (index === 0 ? ' is-active' : '') + '" href="' + href + '">' + item + '</a>';
        }).join("");
    }

    function renderListings(items) {
        var host = document.querySelector('[data-role="gh-listings"]');
        if (!host) return;
        if (!items || !items.length) {
            host.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">!</div>' +
                '  <h3 class="app-empty-title">Chưa có listing cần xử lý</h3>' +
                '  <p class="app-empty-copy">Luồng seller hiện đang trống. Bạn có thể tạo listing mới hoặc mở lại các tin đã ẩn.</p>' +
                '  <div class="app-empty-actions">' +
                '    <a class="app-ghost-btn" href="' + getPostTargetHref() + '">Đi tới tác vụ tiếp theo</a>' +
                '  </div>' +
                '</section>';
            return;
        }
        host.innerHTML = items.map(function (item) {
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref("gianhang", item.id)
                : '/app-ui/gianhang/detail.aspx?id=' + encodeURIComponent(item.id) + '&ui_mode=app';
            return '' +
                '<article class="app-ops-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-ops-row">' +
                '      <div>' +
                '        <h3 class="app-ops-title">' + item.title + '</h3>' +
                '        <p class="app-ops-meta">' + item.category + '</p>' +
                '      </div>' +
                '      <span class="app-status-pill is-' + item.statusTone + '">' + item.status + '</span>' +
                '    </div>' +
                '    <div class="app-ops-foot">' +
                '      <span class="app-ops-stat">' + item.stat + '</span>' +
                '      <span class="app-ops-time">' + item.updatedAt + '</span>' +
                '    </div>' +
                '  </a>' +
                '</article>';
        }).join("");
    }

    function renderConversations(items) {
        var host = document.querySelector('[data-role="gh-conversations"]');
        if (!host) return;
        if (!items || !items.length) {
            host.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">?</div>' +
                '  <h3 class="app-empty-title">Chưa có hội thoại mới</h3>' +
                '  <p class="app-empty-copy">Khi có khách hàng hoặc lead mới, khu vực này sẽ hiện các cuộc trao đổi ưu tiên.</p>' +
                '</section>';
            return;
        }
        host.innerHTML = items.map(function (item) {
            var topic = ((item && item.topic) || "").toLowerCase();
            var href = topic.indexOf("lịch hẹn") >= 0
                ? "/app-ui/gianhang/appointments.aspx?ui_mode=app"
                : "/app-ui/gianhang/conversations.aspx?ui_mode=app";
            return '' +
                '<article class="app-conversation-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-ops-row">' +
                '      <div>' +
                '        <h3 class="app-conversation-name">' + item.name + '</h3>' +
                '        <p class="app-conversation-topic">' + item.topic + '</p>' +
                '      </div>' +
                '    </div>' +
                '    <div class="app-conversation-foot">' +
                '      <span class="app-conversation-state">' + item.state + '</span>' +
                '      <span class="app-conversation-time">' + item.time + '</span>' +
                '    </div>' +
                '  </a>' +
                '</article>';
        }).join("");
    }

    function renderAll(dataRoot) {
        renderHeroMetrics(dataRoot.metrics || []);
        renderFocusStrip(dataRoot.listings || [], dataRoot.conversations || []);
        renderQuickActions(dataRoot.quickActions || []);
        renderMetrics(dataRoot.metrics || []);
        renderPriority(dataRoot.listings || [], dataRoot.conversations || []);
        renderTabs(dataRoot.statusTabs || []);
        renderListings(dataRoot.listings || []);
        renderConversations(dataRoot.conversations || []);
    }

    function buildInitialState() {
        return {
            quickActions: uiScaffoldRoot.quickActions || [],
            metrics: [],
            statusTabs: uiScaffoldRoot.statusTabs || [],
            listings: [],
            conversations: []
        };
    }

    function bootstrap() {
        renderAll(buildInitialState());
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
        if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
        adapter.getSnapshotAsync().then(function (snapshot) {
            if (!snapshot) return;
            renderAll(snapshot);
        });
    }

    bootstrap();
})();
