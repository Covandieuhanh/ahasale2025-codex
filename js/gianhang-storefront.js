(function () {
    function closeAllPartnerAccounts(exceptNode) {
        var nodes = document.querySelectorAll('[data-gh-partner-account]');
        if (!nodes || !nodes.length) {
            return;
        }

        nodes.forEach(function (node) {
            if (!node || node === exceptNode) {
                return;
            }

            if (node.hasAttribute('open')) {
                node.removeAttribute('open');
            }
        });
    }

    function bindPartnerAccountDropdown() {
        var nodes = document.querySelectorAll('[data-gh-partner-account]');
        if (!nodes || !nodes.length) {
            return;
        }

        nodes.forEach(function (node) {
            if (!node || node.getAttribute('data-gh-partner-account-bound') === '1') {
                return;
            }

            var summary = node.querySelector('summary');
            if (!summary) {
                return;
            }

            node.setAttribute('data-gh-partner-account-bound', '1');
            summary.addEventListener('click', function () {
                window.setTimeout(function () {
                    if (node.hasAttribute('open')) {
                        closeAllPartnerAccounts(node);
                    }
                }, 0);
            });
        });
    }

    function setMobileDrawer(open) {
        var body = document.body;
        var drawer = document.getElementById('storefront-mobile-drawer');
        var toggle = document.querySelector('[data-storefront-mobile-toggle]');
        if (!drawer || !body) {
            return;
        }

        if (open) {
            body.classList.add('storefront-mobile-open');
            drawer.setAttribute('aria-hidden', 'false');
            if (toggle) {
                toggle.setAttribute('aria-expanded', 'true');
            }
        } else {
            body.classList.remove('storefront-mobile-open');
            drawer.setAttribute('aria-hidden', 'true');
            if (toggle) {
                toggle.setAttribute('aria-expanded', 'false');
            }
        }
    }

    document.addEventListener('click', function (event) {
        var accountNode = event.target.closest('[data-gh-partner-account]');
        if (!accountNode) {
            closeAllPartnerAccounts();
        } else if (event.target.closest('.gh-partner-account__link, .gh-partner-account__logout')) {
            closeAllPartnerAccounts();
        }

        var toggle = event.target.closest('[data-storefront-mobile-toggle]');
        if (toggle) {
            event.preventDefault();
            setMobileDrawer(!document.body.classList.contains('storefront-mobile-open'));
            return;
        }

        var close = event.target.closest('[data-storefront-mobile-close]');
        if (close) {
            event.preventDefault();
            setMobileDrawer(false);
            return;
        }

        if (document.body.classList.contains('storefront-mobile-open')) {
            var drawer = document.getElementById('storefront-mobile-drawer');
            if (drawer && !drawer.contains(event.target)) {
                setMobileDrawer(false);
            }
        }
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') {
            setMobileDrawer(false);
            closeAllPartnerAccounts();
        }
    });

    function boot() {
        bindPartnerAccountDropdown();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', boot);
    } else {
        boot();
    }

    if (window.Sys && Sys.WebForms) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            boot();
        });
    }
})();
