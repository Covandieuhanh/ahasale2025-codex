(function () {
    var host = document.querySelector('[data-role="gh-conversation-page"]');
    if (!host) return;

    function getCurrentAppHref() {
        return (window.location.pathname || "/") + (window.location.search || "");
    }

    function buildRuntimeHref(pathname) {
        if (!pathname) return "";
        return pathname + (pathname.indexOf("?") >= 0 ? "&" : "?") + "return_url=" + encodeURIComponent(getCurrentAppHref());
    }

    function buildCustomerRuntimeHref(item) {
        var topic = ((item && item.topic) || "").toLowerCase();
        if (topic.indexOf("lịch hẹn") >= 0) {
            return "/app-ui/gianhang/appointments.aspx?ui_mode=app";
        }
        return "/app-ui/gianhang/conversations.aspx?ui_mode=app";
    }

    function renderConversations(dataRoot) {
        var conversations = dataRoot.conversations || [];
        if (!conversations.length) {
            host.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">?</div>' +
                '  <h3 class="app-empty-title">Chưa có khách hàng cần xử lý</h3>' +
                '  <p class="app-empty-copy">Khi tài khoản có đơn hoặc lịch hẹn, danh sách trao đổi sẽ hiện ở đây.</p>' +
                '</section>';
            return;
        }
        host.innerHTML = conversations.map(function (item) {
        var href = buildCustomerRuntimeHref(item);
        return '' +
            '<article class="app-conversation-card">' +
            '  <a class="app-card-link" href="' + href + '">' +
            '    <div class="app-ops-row">' +
            '      <div><h3 class="app-conversation-name">' + item.name + '</h3><p class="app-conversation-topic">' + item.topic + '</p></div>' +
            '      <span class="app-status-pill is-warn">' + item.state + '</span>' +
            '    </div>' +
            '    <div class="app-conversation-foot"><span class="app-conversation-state">Ưu tiên phản hồi</span><span class="app-conversation-time">' + item.time + '</span></div>' +
            '  </a>' +
            '</article>';
        }).join('');
    }

    function bootstrap() {
        renderConversations({ conversations: [] });
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
        if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
        adapter.getSnapshotAsync().then(function (snapshot) {
            if (!snapshot) return;
            renderConversations(snapshot);
        });
    }

    bootstrap();
})();
