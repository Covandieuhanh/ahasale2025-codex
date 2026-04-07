(function () {
    var items = window.AhaAppUiRegistry || [];
    var host = document.querySelector('[data-role="launcher-grid"]');
    if (!host) return;

    host.innerHTML = items.map(function (item) {
        return '' +
            '<a class="app-launcher-card is-' + item.code + '" href="' + item.href + '?ui_mode=app">' +
            '  <div class="app-launcher-row">' +
            '    <span class="app-launcher-badge">' + item.emoji + '</span>' +
            '    <span class="app-launcher-arrow">&rsaquo;</span>' +
            '  </div>' +
            '  <h2 class="app-launcher-label">' + item.label + '</h2>' +
            '  <span class="app-launcher-sub">' + item.shortLabel + '</span>' +
            '  <p class="app-launcher-desc">' + item.description + '</p>' +
            '  <div class="app-launcher-meta">' +
            '    <span class="app-launcher-chip">App độc lập</span>' +
            '    <span class="app-launcher-chip">UI riêng</span>' +
            '  </div>' +
            '</a>';
    }).join("");
})();
