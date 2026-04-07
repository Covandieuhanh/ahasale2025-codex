(function () {
    var summaryHost = document.querySelector('[data-role="gh-status-summary"]');
    var listHost = document.querySelector('[data-role="gh-status-list"]');

    function getCurrentAppHref() {
        return (window.location.pathname || "/") + (window.location.search || "");
    }

    function appendQuery(url, key, value) {
        if (!url || !key || !value) return url || "";
        if (url.indexOf(key + "=") >= 0) return url;
        return url + (url.indexOf("?") >= 0 ? "&" : "?") + key + "=" + encodeURIComponent(value);
    }

    function buildAppHref(url) {
        if (!url || url.indexOf("/app-ui/") !== 0) return url;
        return appendQuery(url, "back_href", getCurrentAppHref());
    }

    function renderAll(dataRoot) {
        var listings = dataRoot.listings || [];
        if (summaryHost) {
            var live = listings.filter(function (item) { return item.statusTone === 'live'; }).length;
            var pending = listings.filter(function (item) { return item.statusTone === 'pending'; }).length;
            summaryHost.innerHTML = '' +
                '<a class="app-gianhang-priority-card app-card-link" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '"><span class="app-gianhang-priority-eyebrow">Đang bán</span><h3 class="app-gianhang-priority-title">' + live + ' listing</h3><p class="app-gianhang-priority-copy">Các tin đang hoạt động và tiếp tục nhận lead.</p></a>' +
                '<a class="app-gianhang-priority-card app-card-link" href="' + buildAppHref('/app-ui/gianhang/appointments.aspx?ui_mode=app') + '"><span class="app-gianhang-priority-eyebrow">Chờ xử lý</span><h3 class="app-gianhang-priority-title">' + pending + ' listing</h3><p class="app-gianhang-priority-copy">Các tin chờ duyệt hoặc cần chỉnh sửa theo ghi chú.</p></a>';
        }
        if (!listHost) return;
        if (!listings.length) {
            listHost.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">!</div>' +
                '  <h3 class="app-empty-title">Chưa có trạng thái tin</h3>' +
                '  <p class="app-empty-copy">Dữ liệu trạng thái sẽ xuất hiện khi tài khoản có tin seller hoạt động.</p>' +
                '</section>';
            return;
        }
        listHost.innerHTML = listings.map(function (item) {
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref("gianhang", item.id)
                : '/app-ui/gianhang/detail.aspx?id=' + encodeURIComponent(item.id) + '&ui_mode=app&back_href=' + encodeURIComponent(getCurrentAppHref());
            return '' +
                '<article class="app-ops-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-ops-row">' +
                '      <div><h3 class="app-ops-title">' + item.title + '</h3><p class="app-ops-meta">' + item.summary + '</p></div>' +
                '      <span class="app-status-pill is-' + item.statusTone + '">' + item.status + '</span>' +
                '    </div>' +
                '    <div class="app-ops-foot"><span class="app-ops-stat">' + item.category + '</span><span class="app-ops-time">' + item.updatedAt + '</span></div>' +
                '  </a>' +
                '</article>';
        }).join('');
    }

    function bootstrap() {
        renderAll({ listings: [] });
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
        if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
        adapter.getSnapshotAsync().then(function (snapshot) {
            if (!snapshot) return;
            renderAll(snapshot);
        });
    }

    bootstrap();
})();
