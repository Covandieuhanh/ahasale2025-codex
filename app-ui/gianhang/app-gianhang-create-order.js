(function () {
    var host = document.querySelector("[data-role='gh-create-order-runtime']");
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

    function render(snapshot) {
        var rows = snapshot && snapshot.orders ? snapshot.orders.slice(0, 5) : [];
        if (!rows.length) {
            host.innerHTML = '' +
                '<article class="app-page-card">' +
                '  <h2>Chưa có đơn gần đây</h2>' +
                '  <div class="app-page-actions">' +
                '    <a class="app-page-btn is-primary" href="' + buildAppHref('/app-ui/gianhang/orders.aspx?ui_mode=app&open=create') + '">Tạo đơn ngay</a>' +
                '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/orders.aspx?ui_mode=app') + '">Mở đơn bán</a>' +
                '  </div>' +
                '</article>';
            return;
        }

        host.innerHTML = '' +
            '<article class="app-page-card">' +
            '  <h2>Đơn gần đây</h2>' +
            '  <div class="app-page-links">' +
            rows.map(function (row) {
                return '' +
                    '<a class="app-page-link" href="' + buildAppHref('/app-ui/gianhang/orders.aspx?ui_mode=app') + '">' +
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
