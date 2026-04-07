(function () {
    function sanitizeAppHref(value) {
        if (!value) return "";
        var raw = String(value);
        var hashIndex = raw.indexOf("#");
        var hash = "";
        var base = raw;
        if (hashIndex >= 0) {
            hash = raw.slice(hashIndex);
            base = raw.slice(0, hashIndex);
        }
        var queryIndex = base.indexOf("?");
        if (queryIndex < 0) return base + hash;
        var path = base.slice(0, queryIndex);
        var query = base.slice(queryIndex + 1);
        var params = new URLSearchParams(query);
        params.delete("back_href");
        params.delete("return_url");
        var nextQuery = params.toString();
        return path + (nextQuery ? "?" + nextQuery : "") + hash;
    }

    function getCurrentAppHref() {
        return sanitizeAppHref((window.location.pathname || "/") + (window.location.search || ""));
    }

    function appendQuery(url, key, value) {
        if (!url || !key || !value) return url || "";
        if (url.indexOf(key + "=") >= 0) return url;
        var hashIndex = url.indexOf("#");
        var hash = "";
        if (hashIndex >= 0) {
            hash = url.slice(hashIndex);
            url = url.slice(0, hashIndex);
        }
        return url + (url.indexOf("?") >= 0 ? "&" : "?") + key + "=" + encodeURIComponent(value) + hash;
    }

    function buildFrameSrc(src) {
        if (!src) return "";
        if (src.indexOf("/") !== 0) return src;
        return appendQuery(src, "return_url", getCurrentAppHref());
    }

    function applyEmbeddedChrome(doc) {
        if (!doc || !doc.head) return;
        if (doc.getElementById("aha-app-embedded-style")) return;
        var style = doc.createElement("style");
        style.id = "aha-app-embedded-style";
        style.textContent = ""
            + ".site-header{display:none!important;}"
            + ".footer{display:none!important;}"
            + ".gh-space-nav-shell{display:none!important;}"
            + ".page-wrapper{margin-top:0!important;}"
            + ".page{padding-bottom:0!important;min-height:0!important;}"
            + "html,body{overflow-x:hidden!important;background:#ffffff!important;}";
        doc.head.appendChild(style);
    }

    function fitFrame(iframe) {
        if (!iframe) return;
        try {
            var doc = iframe.contentDocument;
            if (!doc) return;
            applyEmbeddedChrome(doc);
            var bodyHeight = doc.body ? doc.body.scrollHeight : 0;
            var rootHeight = doc.documentElement ? doc.documentElement.scrollHeight : 0;
            var nextHeight = Math.max(bodyHeight, rootHeight, 640);
            iframe.style.height = nextHeight + "px";
        } catch (error) {
            iframe.style.height = "calc(100dvh - 220px)";
        }
    }

    function bindEmbeddedPage(root) {
        if (!root || root.getAttribute("data-app-embedded-bound") === "1") return;
        root.setAttribute("data-app-embedded-bound", "1");

        var frame = root.querySelector("[data-app-embedded-frame]");
        var loading = root.querySelector("[data-app-embedded-loading]");
        var openDesktop = root.querySelector("[data-app-embedded-open]");
        var desktopSrc = root.getAttribute("data-desktop-src") || "";
        var frameSrc = buildFrameSrc(desktopSrc);

        if (openDesktop && desktopSrc) {
            openDesktop.setAttribute("href", frameSrc);
        }

        if (!frame || !frameSrc) return;

        frame.addEventListener("load", function () {
            if (loading) loading.hidden = true;
            fitFrame(frame);
            clearInterval(frame._fitTimer);
            frame._fitTimer = setInterval(function () {
                fitFrame(frame);
            }, 1200);
        });

        frame.addEventListener("error", function () {
            if (loading) {
                loading.hidden = false;
                loading.textContent = "Không thể tải màn hình hồ sơ. Bạn có thể mở bản đầy đủ bằng nút bên trên.";
            }
        });

        frame.src = frameSrc;
    }

    function init() {
        var roots = document.querySelectorAll("[data-app-embedded-page]");
        roots.forEach(bindEmbeddedPage);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
