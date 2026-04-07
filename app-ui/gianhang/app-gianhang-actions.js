(function () {
    var uiScaffoldRoot = (window.AhaAppUiData && window.AhaAppUiData.gianhang) || { quickActions: [] };
    var actionHost = document.querySelector('[data-role="gh-actions-grid"]');
    var targetHost = document.querySelector('[data-role="gh-actions-targets"]');

    function getCurrentAppHref() {
        return (window.location.pathname || "/") + (window.location.search || "");
    }

    function appendQuery(url, key, value) {
        if (!url || !key || !value) return url || "";
        if (url.indexOf(key + "=") >= 0) return url;
        return url + (url.indexOf("?") >= 0 ? "&" : "?") + key + "=" + encodeURIComponent(value);
    }

    function buildAppHref(url) {
        if (!url) return "/app-ui/gianhang/default.aspx?ui_mode=app";
        if (url.indexOf("/app-ui/") === 0) return appendQuery(url, "back_href", getCurrentAppHref());
        if (url.indexOf("/") === 0) return appendQuery(url, "return_url", getCurrentAppHref());
        return url;
    }

    function buildActionHref(item) {
        var label = ((item && item.label) || "").toLowerCase();
        if (label.indexOf("quan ly tin") >= 0 || label.indexOf("quản lý tin") >= 0) {
            return buildAppHref("/app-ui/gianhang/listing.aspx?ui_mode=app");
        }
        if (label.indexOf("tao don") >= 0 || label.indexOf("tạo đơn") >= 0) {
            return buildAppHref("/app-ui/gianhang/create-order.aspx?ui_mode=app");
        }
        if (label.indexOf("cho thanh toan") >= 0 || label.indexOf("chờ thanh toán") >= 0
            || label.indexOf("cho trao doi") >= 0 || label.indexOf("chờ trao đổi") >= 0) {
            return buildAppHref("/app-ui/gianhang/pending-payment.aspx?ui_mode=app");
        }
        if (label.indexOf("lich hen") >= 0 || label.indexOf("lịch hẹn") >= 0) {
            return buildAppHref("/app-ui/gianhang/appointments.aspx?ui_mode=app");
        }
        if (label.indexOf("khach hang") >= 0 || label.indexOf("khách hàng") >= 0) {
            return buildAppHref("/app-ui/gianhang/conversations.aspx?ui_mode=app");
        }
        if (label.indexOf("mo quan tri") >= 0 || label.indexOf("mở quản trị") >= 0) {
            return buildAppHref("/app-ui/gianhang/status.aspx?ui_mode=app");
        }
        if (label.indexOf("dang tin") >= 0 || label.indexOf("đăng tin") >= 0) {
            return buildAppHref("/app-ui/gianhang/post.aspx?ui_mode=app");
        }
        return buildAppHref(item && item.href ? item.href : "/app-ui/gianhang/default.aspx?ui_mode=app");
    }

    function renderAll(dataRoot) {
        if (actionHost) {
            actionHost.innerHTML = (dataRoot.quickActions || []).map(function (item) {
            var tone = item.tone === 'primary' ? ' is-primary' : '';
            var href = buildActionHref(item);
            return '<a class="app-quick-action-card' + tone + '" href="' + href + '">' + item.label + '</a>';
        }).join('');
        }
        if (targetHost) {
            targetHost.innerHTML = '' +
            '<a class="app-gianhang-priority-card app-card-link" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '"><span class="app-gianhang-priority-eyebrow">Quản lý tin</span><h3 class="app-gianhang-priority-title">Đi vào luồng listing seller</h3><p class="app-gianhang-priority-copy">Dùng cho các thao tác đăng mới, cập nhật nội dung và theo dõi trạng thái tin.</p></a>' +
            '<a class="app-gianhang-priority-card app-card-link" href="' + buildAppHref('/app-ui/gianhang/create-order.aspx?ui_mode=app') + '"><span class="app-gianhang-priority-eyebrow">Đơn hàng</span><h3 class="app-gianhang-priority-title">Tạo đơn và chờ thanh toán</h3><p class="app-gianhang-priority-copy">Kích hoạt nhanh luồng tạo đơn, sau đó chuyển qua màn chờ thanh toán để xử lý tiếp.</p></a>';
        }
    }

    function bootstrap() {
        renderAll({ quickActions: uiScaffoldRoot.quickActions || [] });
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
        if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
        adapter.getSnapshotAsync().then(function (snapshot) {
            if (!snapshot) return;
            renderAll(snapshot);
        });
    }

    bootstrap();
})();
