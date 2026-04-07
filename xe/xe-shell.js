(function () {
    function updateHeaderVars() {
        var header = document.querySelector(".site-header");
        var headerHeight = header ? Math.round(header.getBoundingClientRect().height) : 56;
        document.documentElement.style.setProperty("--aha-header-offset", headerHeight + "px");
        var stickyTop = Math.max(48, headerHeight + 6);
        document.documentElement.style.setProperty("--aha-home-sticky-top", stickyTop + "px");
    }

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

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", updateHeaderVars);
    } else {
        updateHeaderVars();
    }
    window.addEventListener("resize", updateHeaderVars);
})();
