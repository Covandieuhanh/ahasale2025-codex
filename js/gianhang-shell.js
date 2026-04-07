(function () {
    if (!window.AhaPreventDoubleClick) {
        window.AhaPreventDoubleClick = function (btn) {
            try {
                if (!btn) return true;
                if (btn.dataset && btn.dataset.locked === "1") return false;
                if (btn.dataset) btn.dataset.locked = "1";
                setTimeout(function () {
                    try {
                        btn.disabled = true;
                        btn.classList.add("aha-btn-loading");
                    } catch (e) { }
                }, 0);
                return true;
            } catch (e) {
                return true;
            }
        };
    }

    function closeAllGianHangAccounts(exceptNode) {
        var nodes = document.querySelectorAll('[data-gh-gianhang-account]');
        if (!nodes || !nodes.length) return;

        nodes.forEach(function (node) {
            if (!node || node === exceptNode) return;
            if (node.hasAttribute('open')) node.removeAttribute('open');
        });
    }

    function bindGianHangAccountDropdown() {
        var nodes = document.querySelectorAll('[data-gh-gianhang-account]');
        if (!nodes || !nodes.length) return;

        nodes.forEach(function (node) {
            if (!node || node.getAttribute('data-gh-gianhang-account-bound') === '1') return;
            var summary = node.querySelector('summary');
            if (!summary) return;

            node.setAttribute('data-gh-gianhang-account-bound', '1');
            summary.addEventListener('click', function () {
                window.setTimeout(function () {
                    if (node.hasAttribute('open')) closeAllGianHangAccounts(node);
                }, 0);
            });
        });
    }

    function bootGianHangShell() {
        bindGianHangAccountDropdown();
    }

    document.addEventListener('click', function (event) {
        var inAccount = event.target && event.target.closest
            ? event.target.closest('[data-gh-gianhang-account]')
            : null;
        if (!inAccount) closeAllGianHangAccounts();
        if (event.target && event.target.closest && event.target.closest('.gh-gianhang-account__link, .gh-gianhang-account__logout')) {
            closeAllGianHangAccounts();
        }
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') closeAllGianHangAccounts();
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', bootGianHangShell);
    } else {
        bootGianHangShell();
    }

    if (window.Sys && Sys.WebForms) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            bootGianHangShell();
        });
    }
})();
