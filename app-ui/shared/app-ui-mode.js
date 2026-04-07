(function () {
    function parseMode() {
        try {
            var params = new URLSearchParams(window.location.search || "");
            var queryMode = params.get("ui_mode");
            if (queryMode) {
                localStorage.setItem("aha_app_ui_mode", queryMode);
                return queryMode;
            }
        } catch (e) { }

        try {
            return localStorage.getItem("aha_app_ui_mode") || "app";
        } catch (error) {
            return "app";
        }
    }

    var mode = parseMode();
    window.AhaUiMode = {
        current: mode,
        isApp: mode === "app"
    };

    document.documentElement.setAttribute("data-ui-mode", mode);
})();
