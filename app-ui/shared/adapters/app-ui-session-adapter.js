(function () {
    function normalizeSession(raw) {
        var data = raw || {};
        var user = data.user_summary || {};
        var displayName = user.display_name || "Khách";
        var phone = user.phone || "";
        var fallback = user.avatar_fallback || (displayName || "KH").slice(0, 2).toUpperCase();

        return {
            auth_state: data.auth_state || "guest",
            seller_status: data.seller_status || "not_registered",
            available_spaces: Array.isArray(data.available_spaces) ? data.available_spaces : [],
            user_summary: {
                display_name: displayName,
                username: user.username || "",
                phone: phone,
                email: user.email || "",
                role_label: user.role_label || "Khách",
                avatar_url: user.avatar_url || "",
                avatar_fallback: fallback
            }
        };
    }

    function readOverride() {
        try {
            var raw = localStorage.getItem("aha_app_ui_session_override");
            return raw ? normalizeSession(JSON.parse(raw)) : null;
        } catch (e) {
            return null;
        }
    }

    function getSession() {
        var override = readOverride();
        if (override) return override;
        if (window.AhaAppUiRuntimeSession) {
            return normalizeSession(window.AhaAppUiRuntimeSession);
        }
        return normalizeSession({
            auth_state: "authenticated",
            seller_status: "not_registered",
            user_summary: {
                display_name: "Home 0326824915",
                username: "home home home",
                phone: "0326824915",
                role_label: "Khách hàng",
                avatar_fallback: "HM"
            }
        });
    }

    window.AhaAppUiAdapters = window.AhaAppUiAdapters || {};
    window.AhaAppUiAdapters.session = {
        getSession: getSession
    };
})();
