(function () {
    var path = (window.location && window.location.pathname || "").toLowerCase();
    if (path.indexOf("/bat-dong-san") !== 0) return;

    var docEl = document.documentElement;
    var body = document.body;
    var cachedHeaderHeight = -1;
    var resizeRafId = 0;

    if (docEl) docEl.classList.add("aha-bds-theme");
    if (body) body.classList.add("aha-bds-theme");

    function syncBdsHeaderVars() {
        var header = document.querySelector(".bds-topbar");
        var headerInner = header ? header.querySelector(".bds-topbar-inner") : null;
        var headerHeight = headerInner
            ? Math.round(headerInner.getBoundingClientRect().height)
            : (header ? Math.round(header.clientHeight || 78) : 78);
        if (headerHeight <= 0) headerHeight = 78;
        // Guardrail: tránh loop phình chiều cao do đo nhầm box model.
        if (headerHeight < 56) headerHeight = 56;
        if (headerHeight > 96) headerHeight = 96;
        if (headerHeight === cachedHeaderHeight) return;
        cachedHeaderHeight = headerHeight;
        if (docEl) {
            docEl.style.setProperty("--bds-topbar-height", headerHeight + "px");
            docEl.style.setProperty("--aha-header-offset", headerHeight + "px");
            docEl.style.setProperty("--aha-home-sticky-top", (headerHeight + 6) + "px");
        }
    }

    function queueHeaderVarSync() {
        if (resizeRafId) return;
        resizeRafId = window.requestAnimationFrame(function () {
            resizeRafId = 0;
            syncBdsHeaderVars();
        });
    }

    function initBdsHeaderAutoHeight() {
        var header = document.querySelector(".bds-topbar");
        if (!header) return;
        if ("ResizeObserver" in window) {
            var observer = new ResizeObserver(function () {
                queueHeaderVarSync();
            });
            observer.observe(header);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", function () {
            syncBdsHeaderVars();
            initBdsHeaderAutoHeight();
        });
    } else {
        syncBdsHeaderVars();
        initBdsHeaderAutoHeight();
    }

    window.addEventListener("resize", queueHeaderVarSync);
    window.addEventListener("load", syncBdsHeaderVars);

    if (window.Sys && window.Sys.WebForms && window.Sys.WebForms.PageRequestManager) {
        window.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            syncBdsHeaderVars();
        });
    }
})();
