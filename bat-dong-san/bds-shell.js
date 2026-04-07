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

    function cleanupBdsOffcanvasState() {
        try {
            var selector = [
                "#accountMenuCanvas.offcanvas",
                "#bdsMobileMenuCanvas.offcanvas",
                "#bdsNotifCanvas.offcanvas",
                ".bds-filter-canvas.offcanvas"
            ].join(", ");
            var nodes = document.querySelectorAll(selector);
            for (var i = 0; i < nodes.length; i++) {
                var node = nodes[i];
                try {
                    if (window.bootstrap && window.bootstrap.Offcanvas) {
                        var instance = window.bootstrap.Offcanvas.getInstance(node);
                        if (instance) instance.hide();
                    }
                } catch (e) { }

                node.classList.remove("show", "showing", "hiding");
                node.removeAttribute("aria-modal");
                node.removeAttribute("role");
                node.style.removeProperty("visibility");
            }

            var backdrops = document.querySelectorAll(".offcanvas-backdrop, .modal-backdrop");
            for (var j = 0; j < backdrops.length; j++) {
                var backdrop = backdrops[j];
                if (backdrop && backdrop.parentNode) backdrop.parentNode.removeChild(backdrop);
            }

            if (document.documentElement) {
                document.documentElement.classList.remove("offcanvas-open", "modal-open");
            }

            if (document.body) {
                document.body.classList.remove("offcanvas-open", "modal-open");
                document.body.style.removeProperty("overflow");
                document.body.style.removeProperty("padding-right");
                document.body.style.removeProperty("padding-left");
            }
        } catch (e) { }
    }

    function scheduleBdsOffcanvasCleanup() {
        if (!window.requestAnimationFrame) {
            setTimeout(cleanupBdsOffcanvasState, 0);
            return;
        }

        window.requestAnimationFrame(function () {
            window.requestAnimationFrame(cleanupBdsOffcanvasState);
        });
    }

    if (!document.body) return;
    document.body.classList.add("aha-bds-shell");

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", scheduleBdsOffcanvasCleanup);
    } else {
        scheduleBdsOffcanvasCleanup();
    }

    window.addEventListener("load", scheduleBdsOffcanvasCleanup);
    window.addEventListener("pageshow", scheduleBdsOffcanvasCleanup);
})();
