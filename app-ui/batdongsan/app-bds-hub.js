(function () {
    var groups = [
        { label: 'Mua bán', stat: 'Nhà phố, căn hộ, đất nền', href: '/app-ui/batdongsan/mua-ban.aspx?ui_mode=app', searchHref: '/app-ui/batdongsan/search.aspx?ui_mode=app&q=mua+ban' },
        { label: 'Cho thuê', stat: 'Căn hộ, phòng trọ, văn phòng', href: '/app-ui/batdongsan/cho-thue.aspx?ui_mode=app', searchHref: '/app-ui/batdongsan/search.aspx?ui_mode=app&q=cho+thue' },
        { label: 'Dự án', stat: 'Nhóm khám phá dự án nổi bật', href: '/app-ui/batdongsan/du-an.aspx?ui_mode=app', searchHref: '/app-ui/batdongsan/search.aspx?ui_mode=app&q=du+an+bat+dong+san' },
        { label: 'Tham khảo giá', stat: 'Xem nhanh mặt bằng giá theo khu vực', href: '/app-ui/batdongsan/tham-khao-gia.aspx?ui_mode=app', searchHref: '/app-ui/batdongsan/search.aspx?ui_mode=app&q=tham+khao+gia+bat+dong+san' },
        { label: 'Vay mua nhà', stat: 'Nhóm nội dung tài chính nhà ở', href: '/app-ui/batdongsan/vay-mua-nha.aspx?ui_mode=app', searchHref: '/app-ui/batdongsan/search.aspx?ui_mode=app&q=vay+mua+nha' },
        { label: 'Kinh nghiệm', stat: 'Nội dung kinh nghiệm mua bán và thuê nhà', href: '/app-ui/batdongsan/kinh-nghiem.aspx?ui_mode=app', searchHref: '/app-ui/batdongsan/search.aspx?ui_mode=app&q=kinh+nghiem+bat+dong+san' }
    ];
    var host = document.querySelector('[data-role="bds-hub-groups"]');
    if (!host) return;
    host.innerHTML = groups.map(function (item) {
        return '' +
            '<div class="app-filter-group">' +
            '  <span class="app-filter-label">Nhánh chính</span>' +
            '  <span class="app-bds-market-value">' + item.label + '</span>' +
            '  <span class="app-bds-market-label">' + item.stat + '</span>' +
            '  <div class="app-chip-lane">' +
            '    <a class="app-ghost-btn" href="' + item.href + '">Xem trong app</a>' +
            '    <a class="app-chip" href="' + item.searchHref + '">Xem kết quả</a>' +
            '  </div>' +
            '</div>';
    }).join('');
})();
