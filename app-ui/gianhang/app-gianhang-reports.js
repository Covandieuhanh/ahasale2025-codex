(function () {
    var host = document.querySelector("[data-role='gh-reports-runtime']");
    if (!host) return;

    function buildAppHref(pathname) {
        var returnUrl = (window.location.pathname || "/") + (window.location.search || "");
        return pathname + (pathname.indexOf("?") >= 0 ? "&" : "?") + "back_href=" + encodeURIComponent(returnUrl);
    }

    function render(snapshot) {
        var report = snapshot && snapshot.reportSummary ? snapshot.reportSummary : null;
        if (!report) {
            host.innerHTML = '' +
                '<article class="app-page-card">' +
                '  <h2>Chưa có dữ liệu báo cáo</h2>' +
                '  <p>Dữ liệu báo cáo sẽ hiển thị khi tài khoản seller có hoạt động đơn hàng/lịch hẹn.</p>' +
                '</article>';
            return;
        }

        host.innerHTML = '' +
            '<article class="app-page-card">' +
            '  <h2>Tổng quan hiệu suất</h2>' +
            '  <div class="app-page-links">' +
            '    <a class="app-page-link" href="' + buildAppHref("/app-ui/gianhang/reports.aspx?ui_mode=app") + '"><span>Doanh thu gộp<small>Từ gian hàng</small></span><span>' + report.revenue_gross + '</span></a>' +
            '    <a class="app-page-link" href="' + buildAppHref("/app-ui/gianhang/reports.aspx?ui_mode=app") + '"><span>Tổng đơn hàng<small>Đơn đã phát sinh</small></span><span>' + report.total_orders + '</span></a>' +
            '    <a class="app-page-link" href="' + buildAppHref("/app-ui/gianhang/reports.aspx?ui_mode=app") + '"><span>Đơn chờ xử lý<small>Cần phản hồi nhanh</small></span><span>' + report.pending_orders + '</span></a>' +
            '    <a class="app-page-link" href="' + buildAppHref("/app-ui/gianhang/reports.aspx?ui_mode=app") + '"><span>Tổng sản phẩm bán<small>Số lượng đã ghi nhận</small></span><span>' + report.total_sold + '</span></a>' +
            '    <a class="app-page-link" href="' + buildAppHref("/app-ui/gianhang/reports.aspx?ui_mode=app") + '"><span>Lịch hẹn tổng<small>Tất cả booking</small></span><span>' + report.booking_total + '</span></a>' +
            '    <a class="app-page-link" href="' + buildAppHref("/app-ui/gianhang/reports.aspx?ui_mode=app") + '"><span>Lịch hẹn chờ xác nhận<small>Cần xử lý</small></span><span>' + report.booking_pending + '</span></a>' +
            '  </div>' +
            '</article>';
    }

    var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
    if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
    adapter.getSnapshotAsync().then(render);
})();
