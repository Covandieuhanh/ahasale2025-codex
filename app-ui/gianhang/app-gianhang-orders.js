(function () {
    var host = document.querySelector("[data-role='gh-orders-runtime']");
    if (!host) return;

    function buildAppHref(pathname) {
        var returnUrl = (window.location.pathname || "/") + (window.location.search || "");
        return pathname + (pathname.indexOf("?") >= 0 ? "&" : "?") + "back_href=" + encodeURIComponent(returnUrl);
    }

    function normalizeText(value) {
        return String(value || "")
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    }

    function isPendingOrder(row) {
        var normalized = normalizeText(row && row.status);
        if (!normalized) return false;
        return normalized.indexOf("cho") >= 0
            || normalized.indexOf("trao doi") >= 0
            || normalized.indexOf("thanh toan") >= 0
            || normalized.indexOf("dang xu ly") >= 0;
    }

    function buildOrderHref(row) {
        return isPendingOrder(row)
            ? buildAppHref("/app-ui/gianhang/pending-payment.aspx?ui_mode=app")
            : buildAppHref("/app-ui/gianhang/orders.aspx?ui_mode=app");
    }

    function render(snapshot) {
        var rows = snapshot && snapshot.orders ? snapshot.orders : [];
        var pendingRows = rows.filter(isPendingOrder);
        var summaryHtml = '' +
            '<article class="app-page-card">' +
            '  <h2>Tác vụ đơn hàng</h2>' +
            '  <div class="app-page-actions">' +
            '    <a class="app-page-btn is-primary" href="' + buildAppHref('/app-ui/gianhang/create-order.aspx?ui_mode=app') + '">Tạo đơn</a>' +
            '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/pending-payment.aspx?ui_mode=app') + '">Chờ thanh toán (' + pendingRows.length + ')</a>' +
            '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/orders.aspx?ui_mode=app') + '">Đơn bán</a>' +
            '  </div>' +
            '</article>';

        if (!rows.length) {
            host.innerHTML = summaryHtml + '' +
                '<article class="app-page-card">' +
                '  <h2>Chưa có đơn hàng</h2>' +
                '  <p>Tài khoản chưa có đơn bán đang hoạt động hoặc chưa được duyệt gian hàng.</p>' +
                '  <div class="app-page-actions">' +
                '    <a class="app-page-btn is-primary" href="' + buildAppHref('/app-ui/gianhang/create-order.aspx?ui_mode=app') + '">Mở màn tạo đơn</a>' +
                '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/pending-payment.aspx?ui_mode=app') + '">Mở chờ thanh toán</a>' +
                '  </div>' +
                '</article>';
            return;
        }

        host.innerHTML = summaryHtml + '' +
            '<article class="app-page-card">' +
            '  <h2>Đơn gần đây</h2>' +
            '  <div class="app-page-links">' +
            rows.map(function (row) {
                return '' +
                    '<a class="app-page-link" href="' + buildOrderHref(row) + '">' +
                    '  <span>#' + row.id + ' • ' + row.buyer + '<small>' + row.ordered_at + ' • ' + row.status + '</small></span>' +
                    '  <span>' + row.amount + '</span>' +
                    '</a>';
            }).join("") +
            '  </div>' +
            '</article>';
    }

    var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
    if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
    adapter.getSnapshotAsync().then(render);
})();
