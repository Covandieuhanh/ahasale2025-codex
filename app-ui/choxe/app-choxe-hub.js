(function () {
    var groups = [
        { label: "Ô tô", stat: "Xe mới, xe cũ, theo hãng và đời xe", href: "/app-ui/choxe/o-to.aspx?ui_mode=app", searchHref: "/app-ui/choxe/search.aspx?ui_mode=app&q=o+to" },
        { label: "Xe máy", stat: "Xe số, xe tay ga, xe phân khối", href: "/app-ui/choxe/xe-may.aspx?ui_mode=app", searchHref: "/app-ui/choxe/search.aspx?ui_mode=app&q=xe+may" },
        { label: "Phụ tùng", stat: "Lốp, đồ chơi xe, linh kiện thay thế", href: "/app-ui/choxe/phu-tung.aspx?ui_mode=app", searchHref: "/app-ui/choxe/search.aspx?ui_mode=app&q=phu+tung+xe" },
        { label: "Dịch vụ xe", stat: "Rửa xe, bảo dưỡng, sửa chữa", href: "/app-ui/choxe/dich-vu.aspx?ui_mode=app", searchHref: "/app-ui/choxe/search.aspx?ui_mode=app&q=dich+vu+xe" }
    ];
    var host = document.querySelector('[data-role="choxe-hub-groups"]');
    if (!host) return;

    host.innerHTML = groups.map(function (item) {
        return '' +
            '<div class="app-filter-group">' +
            '  <span class="app-filter-label">Điểm vào hiện có</span>' +
            '  <span class="app-choxe-type-label">' + item.label + '</span>' +
            '  <span class="app-choxe-type-stat">' + item.stat + '</span>' +
            '  <div class="app-chip-lane">' +
            '    <a class="app-ghost-btn" href="' + item.href + '">Xem trong app</a>' +
            '    <a class="app-chip" href="' + item.searchHref + '">Xem kết quả</a>' +
            '  </div>' +
            '</div>';
    }).join('');
})();
