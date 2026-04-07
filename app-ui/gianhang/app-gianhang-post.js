(function () {
    var host = document.querySelector("[data-role='gh-post-runtime']");
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

    function getQuery(name) {
        try {
            return new URLSearchParams(window.location.search || "").get(name) || "";
        } catch (e) {
            return "";
        }
    }

    function parseRuntimeId(item) {
        if (!item) return "";
        if (item.runtime_id) return String(item.runtime_id);
        var id = String(item.id || "");
        return id.replace(/^runtime-gh-/i, "");
    }

    function render(snapshot) {
        var listings = snapshot && snapshot.listings ? snapshot.listings : [];
        var runtimeId = getQuery("runtime_id");
        var editing = null;
        if (runtimeId) {
            editing = listings.find(function (item) {
                return parseRuntimeId(item) === String(runtimeId);
            }) || null;
        }

        if (!listings.length) {
            host.innerHTML = '' +
                '<article class="app-page-card">' +
                '  <h2>Chưa có dữ liệu gian hàng</h2>' +
                '  <p>Tài khoản hiện chưa có tin để chỉnh sửa. Bạn có thể bắt đầu từ quản lý tin khi có dữ liệu seller.</p>' +
                '  <div class="app-page-actions">' +
                '    <a class="app-page-btn is-primary" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '">Mở quản lý tin</a>' +
                '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/default.aspx?ui_mode=app') + '">Về Aha Shop</a>' +
                '  </div>' +
                '</article>';
            return;
        }

        var title = editing ? ("Sửa tin: " + editing.title) : "Bản nháp đăng tin";
        var category = editing ? (editing.category || "Danh mục") : (listings[0].category || "Danh mục");
        var summary = editing ? (editing.summary || "") : "Nhập tiêu đề, mô tả ngắn và chọn danh mục phù hợp trước khi đưa tin vào danh sách quản lý.";

        host.innerHTML = '' +
            '<article class="app-page-card">' +
            '  <h2>' + title + '</h2>' +
            '  <div class="app-page-links">' +
            '    <a class="app-page-link" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '"><span>Tiêu đề<small>' + (editing ? editing.title : "Tin mới từ gian hàng") + '</small></span><span>&rsaquo;</span></a>' +
            '    <a class="app-page-link" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '"><span>Danh mục<small>' + category + '</small></span><span>&rsaquo;</span></a>' +
            '    <a class="app-page-link" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '"><span>Mô tả<small>' + summary + '</small></span><span>&rsaquo;</span></a>' +
            '  </div>' +
            '  <div class="app-page-actions">' +
            '    <a class="app-page-btn is-primary" href="' + buildAppHref('/app-ui/gianhang/listing.aspx?ui_mode=app') + '">Lưu vào quản lý tin</a>' +
            '    <a class="app-page-btn" href="' + buildAppHref('/app-ui/gianhang/orders.aspx?ui_mode=app') + '">Qua đơn hàng</a>' +
            '  </div>' +
            '</article>' +
            '<article class="app-page-card">' +
            '  <h2>Tin gần đây</h2>' +
            '  <div class="app-page-links">' +
            listings.slice(0, 5).map(function (item) {
                var id = parseRuntimeId(item);
                return '' +
                    '<a class="app-page-link" href="' + buildAppHref('/app-ui/gianhang/post.aspx?ui_mode=app&runtime_id=' + encodeURIComponent(id)) + '">' +
                    '  <span>' + item.title + '<small>' + (item.status || "") + ' • ' + (item.updatedAt || "") + '</small></span>' +
                    '  <span>&rsaquo;</span>' +
                    '</a>';
            }).join("") +
            '  </div>' +
            '</article>';
    }

    var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
    if (!adapter || typeof adapter.getSnapshotAsync !== "function") return;
    adapter.getSnapshotAsync().then(render);
})();
