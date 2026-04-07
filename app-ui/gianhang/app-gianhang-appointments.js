(function () {
    var host = document.querySelector("[data-role='gh-appointments-runtime']");
    if (!host) return;

    function buildAppHref(pathname) {
        var returnUrl = (window.location.pathname || "/") + (window.location.search || "");
        return pathname + (pathname.indexOf("?") >= 0 ? "&" : "?") + "back_href=" + encodeURIComponent(returnUrl);
    }

    function render(snapshot) {
        var rows = snapshot && snapshot.appointments ? snapshot.appointments : [];
        if (!rows.length) {
            host.innerHTML = '' +
                '<article class="app-page-card">' +
                '  <h2>Chưa có lịch hẹn</h2>' +
                '  <p>Khi có booking từ khách hàng, dữ liệu lịch hẹn sẽ xuất hiện ở đây.</p>' +
                '</article>';
            return;
        }

        host.innerHTML = '' +
            '<article class="app-page-card">' +
            '  <h2>Lịch hẹn gần đây</h2>' +
            '  <div class="app-page-links">' +
            rows.map(function (row) {
                return '<a class="app-page-link" href="' + buildAppHref("/app-ui/gianhang/appointments.aspx?ui_mode=app") + '"><span>#' + row.id + ' • ' + row.customer + '<small>' + row.service + ' • ' + row.booking_time + '</small></span><span>' + row.status + '</span></a>';
            }).join("") +
            '  </div>' +
            '</article>';
    }

    var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
    if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
    adapter.getSnapshotAsync().then(render);
})();
