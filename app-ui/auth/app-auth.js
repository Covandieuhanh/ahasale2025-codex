(function () {
    function getQuery(name) {
        try {
            return new URLSearchParams(window.location.search || "").get(name) || "";
        } catch (e) {
            return "";
        }
    }

    function getSession() {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        return adapter && typeof adapter.getSession === "function"
            ? adapter.getSession()
            : { auth_state: "guest", seller_status: "not_registered", user_summary: null };
    }

    function getIntent() {
        return (getQuery("intent") || "").trim().toLowerCase();
    }

    function buildAppReturnUrl() {
        var returnUrl = getQuery("return_url");
        if (returnUrl && returnUrl.charAt(0) === "/") return returnUrl;
        return "/app-ui/home/default.aspx?ui_mode=app";
    }

    function buildAppUiHref(pathname, extras) {
        var params = new URLSearchParams();
        params.set("ui_mode", "app");
        var returnUrl = buildAppReturnUrl();
        if (returnUrl) params.set("return_url", returnUrl);
        var intent = getIntent();
        if (intent) params.set("intent", intent);
        Object.keys(extras || {}).forEach(function (key) {
            if (extras[key]) params.set(key, extras[key]);
        });
        return pathname + "?" + params.toString();
    }

    function getPostTarget(session) {
        var back = buildAppReturnUrl();
        if (!session || session.auth_state !== "authenticated") {
            return {
                href: buildAppUiHref("/app-ui/auth/login.aspx", { intent: "post" }),
                label: "Đăng nhập để đăng tin"
            };
        }
        if ((session.seller_status || "") === "approved") {
            return {
                href: buildAppUiHref("/app-ui/gianhang/post.aspx"),
                label: "Đăng tin"
            };
        }
        if ((session.seller_status || "") === "pending") {
            return {
                href: buildAppUiHref("/app-ui/auth/pending-approval.aspx", { intent: "post" }),
                label: "Chờ duyệt gian hàng"
            };
        }
        return {
            href: buildAppUiHref("/app-ui/auth/open-shop.aspx", { intent: "post" }),
            label: "Đăng ký gian hàng"
        };
    }

    function wireHref(selector, href) {
        var node = document.querySelector(selector);
        if (node) node.setAttribute("href", href);
    }

    function wireText(selector, text) {
        var node = document.querySelector(selector);
        if (node) node.textContent = text;
    }

    function wireValue(selector, value) {
        var node = document.querySelector(selector);
        if (node) node.value = value || "";
    }

    function setLoginFeedback(text, success) {
        var node = document.querySelector("[data-role='auth-login-feedback']");
        if (!node) return;
        node.textContent = text || "";
        node.classList.toggle("is-success", !!success);
    }

    function resolveSellerStatusLabel(status) {
        var normalized = (status || "not_registered").toLowerCase();
        if (normalized === "approved") return "Đã duyệt gian hàng";
        if (normalized === "pending") return "Đang chờ duyệt";
        return "Chưa đăng ký gian hàng";
    }

    function formatLocalNow() {
        var now = new Date();
        var dd = String(now.getDate()).padStart(2, "0");
        var mm = String(now.getMonth() + 1).padStart(2, "0");
        var yyyy = String(now.getFullYear());
        var hh = String(now.getHours()).padStart(2, "0");
        var min = String(now.getMinutes()).padStart(2, "0");
        return dd + "/" + mm + "/" + yyyy + " " + hh + ":" + min;
    }

    function bindPasswordToggles() {
        var toggles = document.querySelectorAll("[data-role='auth-password-toggle']");
        toggles.forEach(function (toggle) {
            if (toggle.dataset.bound === "1") return;
            toggle.dataset.bound = "1";
            toggle.addEventListener("click", function () {
                var wrap = toggle.closest(".app-auth-password-wrap");
                var input = wrap ? wrap.querySelector(".app-auth-input-password") : null;
                if (!input) return;
                var show = input.type === "password";
                input.type = show ? "text" : "password";
                toggle.textContent = show ? "Ẩn" : "Hiện";
                toggle.setAttribute("aria-label", show ? "Ẩn mật khẩu" : "Hiện mật khẩu");
            });
        });
    }

    function pruneAuthExplainers() {
        var cards = document.querySelectorAll(".app-auth-card");
        cards.forEach(function (card) {
            if (card.querySelector(".app-auth-list")) {
                card.remove();
            }
        });
    }

    function bindAppLoginSubmit() {
        var pathname = (window.location.pathname || "").toLowerCase();
        if (pathname.indexOf("/app-ui/auth/login.aspx") < 0) return;

        var session = getSession();
        if (session && session.auth_state === "authenticated") return;

        var submitBtn = document.querySelector("[data-role='auth-login-main']");
        var idInput = document.querySelector("[data-role='auth-login-identifier']");
        var passInput = document.querySelector("[data-role='auth-login-password']");
        if (!submitBtn || !idInput || !passInput || !window.fetch) return;

        if (submitBtn.dataset.boundAppLogin === "1") return;
        submitBtn.dataset.boundAppLogin = "1";

        submitBtn.addEventListener("click", function (event) {
            event.preventDefault();
            setLoginFeedback("", false);

            var loginId = (idInput.value || "").trim();
            var password = passInput.value || "";
            if (!loginId) {
                setLoginFeedback("Vui lòng nhập tài khoản / số điện thoại / email.", false);
                idInput.focus();
                return;
            }
            if (!password) {
                setLoginFeedback("Vui lòng nhập mật khẩu.", false);
                passInput.focus();
                return;
            }

            var originalText = submitBtn.textContent || "Đăng nhập ngay";
            submitBtn.textContent = "Đang đăng nhập...";
            submitBtn.classList.add("is-loading");
            submitBtn.style.pointerEvents = "none";

            var body = new URLSearchParams();
            body.set("login_id", loginId);
            body.set("password", password);
            body.set("return_url", buildAppReturnUrl());
            var intent = getIntent();
            if (intent) body.set("intent", intent);

            fetch("/app-ui/shared/runtime-auth-login.ashx", {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
                },
                body: body.toString()
            })
                .then(function (response) {
                    return response.text().then(function (raw) {
                        var payload = null;
                        try { payload = raw ? JSON.parse(raw) : null; } catch (e) { payload = null; }
                        return { ok: response.ok, payload: payload };
                    });
                })
                .then(function (result) {
                    var payload = result.payload || {};
                    if (!result.ok || !payload.ok) {
                        setLoginFeedback(payload.message || "Đăng nhập không thành công. Vui lòng thử lại.", false);
                        submitBtn.textContent = originalText;
                        submitBtn.classList.remove("is-loading");
                        submitBtn.style.pointerEvents = "";
                        return;
                    }
                    setLoginFeedback(payload.message || "Đăng nhập thành công.", true);
                    var redirectUrl = payload.redirect_url || buildAppReturnUrl();
                    window.location.href = redirectUrl;
                })
                .catch(function () {
                    setLoginFeedback("Lỗi kết nối, vui lòng thử lại.", false);
                    submitBtn.textContent = originalText;
                    submitBtn.classList.remove("is-loading");
                    submitBtn.style.pointerEvents = "";
                });
        });
    }

    function decorateAuthenticatedConsumerPage(mainSelector, mainDefaultText, secondarySelector, secondaryText) {
        var session = getSession();
        if (!session || session.auth_state !== "authenticated") return;
        var mainHref = getIntent() === "post"
            ? getPostTarget(session).href
            : buildAppReturnUrl();
        var mainText = getIntent() === "post"
            ? getPostTarget(session).label
            : "Tiếp tục vào app";
        wireHref(mainSelector, mainHref);
        wireText(mainSelector, mainText || mainDefaultText);
        if (secondarySelector) {
            wireHref(secondarySelector, "/app-ui/home/profile.aspx?ui_mode=app");
            wireText(secondarySelector, secondaryText || "Mở tài khoản");
        }
    }

    function initLoginPage() {
        var back = buildAppReturnUrl();
        wireHref("[data-role='auth-login-main']", buildAppUiHref("/app-ui/auth/login.aspx"));
        wireHref("[data-role='auth-register-link']", buildAppUiHref("/app-ui/auth/register.aspx"));
        wireHref("[data-role='auth-forgot-link']", buildAppUiHref("/app-ui/auth/forgot-password.aspx"));
        wireHref("[data-role='auth-forgot-link-inline']", buildAppUiHref("/app-ui/auth/forgot-password.aspx"));
        wireHref("[data-role='auth-open-shop-link']", buildAppUiHref("/app-ui/auth/open-shop.aspx", { intent: "post" }));
        wireHref("[data-role='auth-app-back']", back);
        decorateAuthenticatedConsumerPage("[data-role='auth-login-main']", "Đăng nhập ngay", "[data-role='auth-register-link']", "Mở tài khoản");
    }

    function initRegisterPage() {
        var back = buildAppReturnUrl();
        wireHref("[data-role='auth-register-main']", buildAppUiHref("/app-ui/auth/register.aspx"));
        wireHref("[data-role='auth-login-link']", buildAppUiHref("/app-ui/auth/login.aspx"));
        wireHref("[data-role='auth-app-back']", back);
        decorateAuthenticatedConsumerPage("[data-role='auth-register-main']", "Đăng ký tài khoản", "[data-role='auth-login-link']", "Mở tài khoản");
    }

    function initOpenShopPage() {
        var target = buildAppUiHref("/app-ui/auth/pending-approval.aspx", { intent: "post" });
        wireHref("[data-role='auth-open-shop-main']", target);
        wireHref("[data-role='auth-pending-link']", buildAppUiHref("/app-ui/auth/pending-approval.aspx", { intent: "post" }));
        wireHref("[data-role='auth-login-link']", buildAppUiHref("/app-ui/auth/login.aspx", { intent: "post" }));
        wireHref("[data-role='auth-app-back']", buildAppReturnUrl());

        var session = getSession();
        var user = session && session.user_summary ? session.user_summary : null;
        wireText("[data-role='auth-open-shop-user']", user && user.display_name ? user.display_name : "Khách");
        wireText("[data-role='auth-open-shop-status']", resolveSellerStatusLabel(session ? session.seller_status : "not_registered"));
        wireText(
            "[data-role='auth-open-shop-session-copy']",
            session && session.auth_state === "authenticated"
                ? "Bạn đang đăng nhập. Có thể gửi yêu cầu mở gian hàng ngay."
                : "Bạn chưa đăng nhập. Cần đăng nhập trước khi gửi yêu cầu mở gian hàng."
        );
        wireValue("[data-role='auth-open-shop-name']", user && user.display_name ? (user.display_name + " Shop") : "");

        if (session && session.auth_state === "authenticated" && (session.seller_status || "") === "approved") {
            var postTarget = getPostTarget(session);
            wireHref("[data-role='auth-open-shop-main']", postTarget.href);
            wireText("[data-role='auth-open-shop-main']", postTarget.label);
            wireHref("[data-role='auth-pending-link']", "/app-ui/gianhang/default.aspx?ui_mode=app");
            wireText("[data-role='auth-pending-link']", "Vào Aha Shop");
            wireText("[data-role='auth-open-shop-session-copy']", "Gian hàng đã được duyệt, bạn có thể đăng tin ngay.");
            wireText("[data-role='auth-open-shop-status']", "Đã duyệt gian hàng");
        }
    }

    function initPendingPage() {
        var target = buildAppUiHref("/app-ui/gianhang/status.aspx");
        wireHref("[data-role='auth-pending-main']", target);
        wireHref("[data-role='auth-open-shop-link']", buildAppUiHref("/app-ui/auth/open-shop.aspx", { intent: "post" }));
        wireHref("[data-role='auth-app-back']", buildAppReturnUrl());
        wireText("[data-role='auth-pending-updated']", formatLocalNow());

        var session = getSession();
        var user = session && session.user_summary ? session.user_summary : null;
        wireText("[data-role='auth-pending-user']", user && user.display_name ? user.display_name : "Khách");
        wireText("[data-role='auth-pending-status']", resolveSellerStatusLabel(session ? session.seller_status : "pending"));
        wireText(
            "[data-role='auth-pending-session-copy']",
            session && session.auth_state === "authenticated"
                ? "Phiên đăng nhập đã sẵn sàng. Bạn sẽ được mở quyền seller ngay khi duyệt xong."
                : "Bạn chưa đăng nhập. Hãy đăng nhập để theo dõi đầy đủ trạng thái duyệt."
        );

        if (session && session.auth_state === "authenticated" && (session.seller_status || "") === "approved") {
            var postTarget = getPostTarget(session);
            wireHref("[data-role='auth-pending-main']", postTarget.href);
            wireText("[data-role='auth-pending-main']", postTarget.label);
            wireHref("[data-role='auth-open-shop-link']", "/app-ui/gianhang/default.aspx?ui_mode=app");
            wireText("[data-role='auth-open-shop-link']", "Vào Aha Shop");
            wireText("[data-role='auth-pending-status']", "Đã duyệt gian hàng");
            wireText("[data-role='auth-pending-session-copy']", "Gian hàng đã được duyệt, có thể vào Aha Shop hoặc đăng tin ngay.");
        }
    }

    function initForgotPage() {
        var back = buildAppReturnUrl();
        wireHref("[data-role='auth-forgot-main']", buildAppUiHref("/app-ui/auth/forgot-password.aspx"));
        wireHref("[data-role='auth-login-link']", buildAppUiHref("/app-ui/auth/login.aspx"));
        wireHref("[data-role='auth-register-link']", buildAppUiHref("/app-ui/auth/register.aspx"));
        wireHref("[data-role='auth-app-back']", back);
        decorateAuthenticatedConsumerPage("[data-role='auth-forgot-main']", "Đến trang khôi phục", "[data-role='auth-login-link']", "Mở tài khoản");
    }

    function init() {
        pruneAuthExplainers();
        bindPasswordToggles();
        bindAppLoginSubmit();
        var pathname = window.location.pathname || "";
        if (pathname.indexOf("/app-ui/auth/login.aspx") >= 0) {
            initLoginPage();
            return;
        }
        if (pathname.indexOf("/app-ui/auth/register.aspx") >= 0) {
            initRegisterPage();
            return;
        }
        if (pathname.indexOf("/app-ui/auth/open-shop.aspx") >= 0) {
            initOpenShopPage();
            return;
        }
        if (pathname.indexOf("/app-ui/auth/pending-approval.aspx") >= 0) {
            initPendingPage();
            return;
        }
        if (pathname.indexOf("/app-ui/auth/forgot-password.aspx") >= 0) {
            initForgotPage();
            return;
        }
        wireText("[data-role='auth-page-title']", "Tài khoản AhaSale");
    }

    init();
})();
