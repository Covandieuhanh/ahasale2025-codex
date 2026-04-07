(function () {
    var host = document.querySelector("[data-role='gh-pending-payment-runtime']");
    if (!host) return;

    function getCurrentAppHref() {
        return (window.location.pathname || "/") + (window.location.search || "");
    }

    function appendQuery(url, key, value) {
        if (!url || !key || !value) return url || "";
        if (url.indexOf(key + "=") >= 0) return url;
        return url + (url.indexOf("?") >= 0 ? "&" : "?") + key + "=" + encodeURIComponent(value);
    }

    function buildAppHref(pathname) {
        return appendQuery(pathname, "back_href", getCurrentAppHref());
    }

    function normalizeText(value) {
        return String(value || "")
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    }

    function isPendingExchangeStatus(statusText) {
        var normalized = normalizeText(statusText);
        if (!normalized) return false;
        return normalized.indexOf("cho") >= 0
            || normalized.indexOf("trao doi") >= 0
            || normalized.indexOf("thanh toan") >= 0
            || normalized.indexOf("dang xu ly") >= 0
            || normalized.indexOf("da dat") >= 0;
    }

    function render(snapshot) {
        var rows = snapshot && snapshot.orders ? snapshot.orders : [];
        var pendingRows = rows.filter(function (row) {
            return isPendingExchangeStatus(row && row.status);
        });

        if (!pendingRows.length) {
            host.innerHTML = '' +
                '<article class="app-page-card">' +
                '  <h2>Chưa có đơn chờ trao đổi</h2>' +
                '  <div class="app-page-actions">' +
                '    <a class="app-page-btn is-primary" href="' + buildAppHref('/app-ui/gianhang/orders.aspx?ui_mode=app') + '">Mở đơn bán</a>' +
                '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/create-order.aspx?ui_mode=app') + '">Tạo đơn mới</a>' +
                '  </div>' +
                '</article>';
            return;
        }

        host.innerHTML = '' +
            '<article class="app-page-card">' +
            '  <h2>Đơn đang chờ trao đổi</h2>' +
            '  <div class="app-page-links">' +
            pendingRows.map(function (row) {
                return '' +
                    '<a class="app-page-link" href="' + buildAppHref('/app-ui/gianhang/pending-payment.aspx?ui_mode=app') + '">' +
                    '  <span>#' + row.id + ' • ' + row.buyer + '<small>' + row.ordered_at + ' • ' + row.status + '</small></span>' +
                    '  <span>' + row.amount + '</span>' +
                    '</a>';
            }).join("") +
            '  </div>' +
            '  <div class="app-page-actions">' +
            '    <a class="app-page-btn is-primary" href="' + buildAppHref('/app-ui/gianhang/pending-payment.aspx?ui_mode=app') + '">Xử lý chờ trao đổi</a>' +
            '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/orders.aspx?ui_mode=app') + '">Xem toàn bộ đơn</a>' +
            '  </div>' +
            '</article>';
    }

    var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
    if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
    adapter.getSnapshotAsync().then(render);
})();
