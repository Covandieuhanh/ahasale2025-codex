(function () {
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
        }
    });
})();
