(function () {
    var uiScaffoldRoot = (window.AhaAppUiData && window.AhaAppUiData.gianhang) || {
        statusTabs: [],
        listings: []
    };
    var tabHost = document.querySelector('[data-role="gh-status-tabs"]');
    var listHost = document.querySelector('[data-role="gh-listing-page"]');

    function renderAll(dataRoot) {
        if (tabHost) {
            tabHost.innerHTML = (dataRoot.statusTabs || []).map(function (item, index) {
                return '<button class="app-feed-tab' + (index === 0 ? ' is-active' : '') + '" type="button">' + item + '</button>';
            }).join('');
        }
        if (!listHost) return;
        var listings = dataRoot.listings || [];
        if (!listings.length) {
            listHost.innerHTML = '' +
                '<section class="app-empty-state">' +
                '  <div class="app-empty-icon">!</div>' +
                '  <h3 class="app-empty-title">Chưa có tin đăng gian hàng</h3>' +
                '  <p class="app-empty-copy">Tài khoản chưa có dữ liệu seller hoạt động hoặc chưa được duyệt gian hàng.</p>' +
                '</section>';
            return;
        }
        listHost.innerHTML = listings.map(function (item) {
            var href = window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing
                ? window.AhaAppUiAdapters.listing.buildDetailHref("gianhang", item.id)
                : '/app-ui/gianhang/detail.aspx?id=' + encodeURIComponent(item.id) + '&ui_mode=app';
            return '' +
                '<article class="app-ops-card">' +
                '  <a class="app-card-link" href="' + href + '">' +
                '    <div class="app-ops-row">' +
                '      <div><h3 class="app-ops-title">' + item.title + '</h3><p class="app-ops-meta">' + item.category + '</p></div>' +
                '      <span class="app-status-pill is-' + item.statusTone + '">' + item.status + '</span>' +
                '    </div>' +
                '    <div class="app-ops-foot"><span class="app-ops-stat">' + item.stat + '</span><span class="app-ops-time">' + item.updatedAt + '</span></div>' +
                '  </a>' +
                '</article>';
        }).join('');
    }

    function bootstrap() {
        renderAll({
            statusTabs: uiScaffoldRoot.statusTabs || [],
            listings: []
        });
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
        if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
        adapter.getSnapshotAsync().then(function (snapshot) {
            if (!snapshot) return;
            renderAll(snapshot);
        });
    }

    bootstrap();
})();
