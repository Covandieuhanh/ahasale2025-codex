(function () {
    var groups = [
        { label: "Điện thoại", stat: "Smartphone, máy cũ, theo dòng máy", href: "/app-ui/dienthoai-maytinh/dien-thoai.aspx?ui_mode=app", searchHref: "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app&q=dien+thoai" },
        { label: "Máy tính", stat: "Laptop, PC, màn hình, linh kiện", href: "/app-ui/dienthoai-maytinh/may-tinh.aspx?ui_mode=app", searchHref: "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app&q=may+tinh" },
        { label: "Phụ kiện", stat: "Tai nghe, sạc, bàn phím, chuột", href: "/app-ui/dienthoai-maytinh/phu-kien.aspx?ui_mode=app", searchHref: "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app&q=phu+kien+cong+nghe" }
    ];
    var host = document.querySelector('[data-role="tech-hub-groups"]');
    if (!host) return;

    host.innerHTML = groups.map(function (item) {
        return '' +
            '<div class="app-filter-group">' +
            '  <span class="app-filter-label">Danh mục công nghệ</span>' +
            '  <span class="app-tech-type-label">' + item.label + '</span>' +
            '  <span class="app-tech-type-stat">' + item.stat + '</span>' +
            '  <div class="app-chip-lane">' +
            '    <a class="app-ghost-btn" href="' + item.href + '">Xem trong app</a>' +
            '    <a class="app-chip" href="' + item.searchHref + '">Xem kết quả</a>' +
            '  </div>' +
            '</div>';
    }).join('');
})();
