(function () {
    "use strict";

    var VIEW_PARAM = "view";
    var adminBusyCounter = 0;
    var adminNotifyConfig = {
        timeout: 4000
    };

    function onReady(fn) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", fn);
            return;
        }
        fn();
    }

    function parseQuery(search) {
        var out = {};
        var src = search || "";

        if (src.charAt(0) === "?") {
            src = src.substring(1);
        }
        if (!src) {
            return out;
        }

        var parts = src.split("&");
        for (var i = 0; i < parts.length; i += 1) {
            var pair = parts[i];
            if (!pair) {
                continue;
            }
            var idx = pair.indexOf("=");
            var key = idx >= 0 ? pair.substring(0, idx) : pair;
            var val = idx >= 0 ? pair.substring(idx + 1) : "";

            try {
                key = decodeURIComponent(key.replace(/\+/g, " "));
                val = decodeURIComponent(val.replace(/\+/g, " "));
            } catch (e) {
                continue;
            }

            if (key) {
                out[key] = val;
            }
        }

        return out;
    }

    function stringifyQuery(params) {
        var pairs = [];
        for (var key in params) {
            if (!Object.prototype.hasOwnProperty.call(params, key)) {
                continue;
            }
            var value = params[key];
            if (value === undefined || value === null || value === "") {
                continue;
            }
            pairs.push(encodeURIComponent(key) + "=" + encodeURIComponent(value));
        }
        return pairs.join("&");
    }

    function getParam(name) {
        var params = parseQuery(window.location.search);
        return params[name] || "";
    }

    function setViewParam(value) {
        var params = parseQuery(window.location.search);
        if (value) {
            params[VIEW_PARAM] = value;
        } else {
            delete params[VIEW_PARAM];
        }

        var query = stringifyQuery(params);
        var target = window.location.pathname + (query ? "?" + query : "");
        window.location.assign(target);
    }

    function setFormActionWithoutViewParam() {
        var mainForm = document.querySelector("form");
        if (!mainForm) {
            return;
        }

        var params = parseQuery(window.location.search);
        if (!params[VIEW_PARAM]) {
            return;
        }

        delete params[VIEW_PARAM];
        var query = stringifyQuery(params);
        mainForm.action = window.location.pathname + (query ? "?" + query : "");
    }

    function toElement(node) {
        if (!node) {
            return null;
        }
        if (node.nodeType === 1) {
            return node;
        }
        return node.parentElement || null;
    }

    function escapeHtml(text) {
        return String(text || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/\"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function cleanPath(path) {
        var src = (path || "").toLowerCase();
        if (!src) {
            return "";
        }
        src = src.replace(/\/+$/, "");
        return src || "/";
    }

    function getPathFromHref(href) {
        if (!href) {
            return "";
        }
        var anchor = document.createElement("a");
        anchor.href = href;
        return cleanPath(anchor.pathname);
    }

    function isNavigableHref(href) {
        if (!href) {
            return false;
        }
        var normalized = href.trim();
        if (!normalized || normalized === "#") {
            return false;
        }
        return !/^javascript:/i.test(normalized);
    }

    function normalizeMenuHref(href) {
        var src = (href || "").trim();
        if (!src) {
            return "";
        }
        if (/^(?:[a-z]+:)?\/\//i.test(src) || src.charAt(0) === "#") {
            return src;
        }
        if (src.charAt(0) !== "/") {
            src = "/" + src;
        }
        return src;
    }

    function directChild(node, tagName, requiredClass) {
        if (!node || !node.children) {
            return null;
        }

        var desired = tagName.toUpperCase();
        for (var i = 0; i < node.children.length; i += 1) {
            var child = node.children[i];
            if (child.tagName !== desired) {
                continue;
            }
            if (!requiredClass || child.classList.contains(requiredClass)) {
                return child;
            }
        }
        return null;
    }

    function createMenuLink(linkNode, level, currentPath, variant) {
        if (!linkNode) {
            return null;
        }

        var href = normalizeMenuHref(linkNode.getAttribute("href") || "");
        if (!isNavigableHref(href)) {
            return null;
        }

        var captionNode = linkNode.querySelector(".caption");
        var iconNode = linkNode.querySelector(".icon");
        var caption = captionNode ? captionNode.textContent : linkNode.textContent;
        caption = (caption || "").replace(/\s+/g, " ").trim();

        if (!caption) {
            return null;
        }

        var path = getPathFromHref(href);
        var row = document.createElement("a");
        if (variant === "shop") {
            row.className = "admin-shop-menu-item admin-avatar-menu-item level-" + level;
        } else {
            row.className = "avatar-nav-link level-" + level;
        }
        row.href = href;
        row.innerHTML = (iconNode ? "<span class='icon'>" + iconNode.innerHTML + "</span>" : "")
            + "<span class='caption'>" + escapeHtml(caption) + "</span>";

        if (path && currentPath === path) {
            row.classList.add("active");
        }

        return row;
    }

    function initSideMenuDropdowns() {
        var sideMenu = document.getElementById("side-menu");
        if (!sideMenu) {
            return;
        }

        for (var i = 0; i < sideMenu.children.length; i += 1) {
            var item = sideMenu.children[i];
            var trigger = directChild(item, "a");
            var nested = directChild(item, "ul", "navview-menu");

            if (!trigger || !nested) {
                continue;
            }

            item.classList.add("has-submenu");

            if (item.classList.contains("active") || nested.querySelector("li.active")) {
                item.classList.add("open");
                item.classList.add("active-branch");
            }

            if (trigger.getAttribute("data-admin-submenu-bound") === "1") {
                continue;
            }

            trigger.addEventListener("click", function (event) {
                var currentTrigger = event.currentTarget;
                var currentItem = currentTrigger ? currentTrigger.parentElement : null;
                if (!currentItem) {
                    return;
                }

                if (!isNavigableHref(currentTrigger.getAttribute("href") || "")) {
                    event.preventDefault();
                }

                currentItem.classList.toggle("open");
            }, true);

            trigger.setAttribute("data-admin-submenu-bound", "1");
        }
    }

    function buildAvatarMenu() {
        var host = document.getElementById("admin-avatar-nav-menu") || document.getElementById("avatar-nav-menu");
        var sideMenu = document.getElementById("side-menu");

        if (!host || !sideMenu) {
            return;
        }

        var currentPath = cleanPath(window.location.pathname);
        var frag = document.createDocumentFragment();
        var hasItems = false;
        var variant = host.id === "admin-avatar-nav-menu" ? "shop" : "avatar";

        for (var i = 0; i < sideMenu.children.length; i += 1) {
            var item = sideMenu.children[i];
            if (item.classList.contains("item-header")) {
                var headerText = (item.textContent || "").replace(/\s+/g, " ").trim();
                if (headerText) {
                    var header = document.createElement("div");
                    header.className = variant === "shop" ? "admin-shop-menu-group-title" : "avatar-nav-group";
                    header.textContent = headerText;
                    frag.appendChild(header);
                }
                continue;
            }

            var topLink = directChild(item, "a");
            var renderedTop = createMenuLink(topLink, 0, currentPath, variant);
            if (renderedTop) {
                frag.appendChild(renderedTop);
                hasItems = true;
            }

            var nested = directChild(item, "ul", "navview-menu");
            if (nested) {
                for (var j = 0; j < nested.children.length; j += 1) {
                    var nestedLi = nested.children[j];
                    var nestedLink = directChild(nestedLi, "a");
                    var renderedNested = createMenuLink(nestedLink, 1, currentPath, variant);
                    if (renderedNested) {
                        frag.appendChild(renderedNested);
                        hasItems = true;
                    }
                }
            }
        }

        host.innerHTML = "";
        if (!hasItems) {
            host.innerHTML = "<div class='avatar-nav-empty'>Không có menu.</div>";
            return;
        }
        host.appendChild(frag);
    }

    function initAvatarMenuToggle() {
        var shell = document.getElementById("admin-avatar-shell");
        var toggle = document.getElementById("admin-avatar-toggle");
        var dropdown = document.getElementById("admin-avatar-dropdown");
        var navHost = document.getElementById("admin-avatar-nav-menu");

        if (shell && toggle && dropdown) {
            shell.style.visibility = "visible";
            shell.style.opacity = "1";
            shell.classList.remove("open");

            if (toggle.getAttribute("data-admin-avatar-bound") === "1") {
                return;
            }

            function closeShell() {
                shell.classList.remove("open");
                toggle.setAttribute("aria-expanded", "false");
            }

            function openShell() {
                shell.classList.add("open");
                toggle.setAttribute("aria-expanded", "true");
            }

            toggle.addEventListener("click", function (event) {
                event.preventDefault();
                event.stopPropagation();
                if (shell.classList.contains("open")) {
                    closeShell();
                } else {
                    openShell();
                }
            }, true);

            dropdown.addEventListener("click", function (event) {
                event.stopPropagation();
            }, true);

            if (!window.__adminAvatarMenuDocBound) {
                document.addEventListener("click", function (event) {
                    var currentShell = document.getElementById("admin-avatar-shell");
                    if (!currentShell) {
                        return;
                    }

                    if (currentShell.contains(event.target)) {
                        return;
                    }

                    currentShell.classList.remove("open");
                    var currentToggle = document.getElementById("admin-avatar-toggle");
                    if (currentToggle) {
                        currentToggle.setAttribute("aria-expanded", "false");
                    }
                }, true);

                document.addEventListener("keydown", function (event) {
                    if (event.key !== "Escape") {
                        return;
                    }

                    var currentShell = document.getElementById("admin-avatar-shell");
                    if (currentShell) {
                        currentShell.classList.remove("open");
                    }

                    var currentToggle = document.getElementById("admin-avatar-toggle");
                    if (currentToggle) {
                        currentToggle.setAttribute("aria-expanded", "false");
                    }
                }, true);

                window.addEventListener("pageshow", function () {
                    var currentShell = document.getElementById("admin-avatar-shell");
                    if (currentShell) {
                        currentShell.classList.remove("open");
                    }
                    var currentToggle = document.getElementById("admin-avatar-toggle");
                    if (currentToggle) {
                        currentToggle.setAttribute("aria-expanded", "false");
                    }
                });

                window.__adminAvatarMenuDocBound = true;
            }

            if (navHost && navHost.getAttribute("data-admin-avatar-links-bound") !== "1") {
                navHost.addEventListener("click", function (event) {
                    var targetEl = toElement(event.target);
                    var link = targetEl && targetEl.closest ? targetEl.closest("a.admin-shop-menu-item[href], a.avatar-nav-link[href]") : null;
                    if (!link) {
                        return;
                    }

                    closeShell();

                    var href = link.getAttribute("href") || "";
                    if (!isNavigableHref(href)) {
                        event.preventDefault();
                    }
                }, true);

                navHost.setAttribute("data-admin-avatar-links-bound", "1");
            }

            toggle.setAttribute("data-admin-avatar-bound", "1");
            return;
        }

        toggle = document.getElementById("avatar-menu-toggle");
        var panel = document.getElementById("avatar-menu-panel");
        navHost = document.getElementById("avatar-nav-menu");

        if (!toggle || !panel) {
            return;
        }

        panel.classList.remove("open");

        if (!window.__adminAvatarMenuDocBound) {
            document.addEventListener("click", function (event) {
                var currentPanel = document.getElementById("avatar-menu-panel");
                var currentToggle = document.getElementById("avatar-menu-toggle");

                if (!currentPanel || !currentToggle) {
                    return;
                }

                var inPanel = event.target && currentPanel.contains(event.target);
                var inToggle = event.target && currentToggle.contains(event.target);
                if (!inPanel && !inToggle) {
                    currentPanel.classList.remove("open");
                }
            }, true);

            window.addEventListener("pageshow", function () {
                var currentPanel = document.getElementById("avatar-menu-panel");
                if (currentPanel) {
                    currentPanel.classList.remove("open");
                }
            });
            window.__adminAvatarMenuDocBound = true;
        }

        if (toggle.getAttribute("data-admin-avatar-bound") === "1") {
            return;
        }

        toggle.addEventListener("click", function (event) {
            event.preventDefault();
            event.stopPropagation();
            panel.classList.toggle("open");
        }, true);

        if (navHost && navHost.getAttribute("data-admin-avatar-links-bound") !== "1") {
            navHost.addEventListener("click", function (event) {
                var targetEl = toElement(event.target);
                var link = targetEl && targetEl.closest ? targetEl.closest("a.avatar-nav-link[href]") : null;
                if (!link) {
                    return;
                }

                panel.classList.remove("open");

                var href = link.getAttribute("href") || "";
                if (!isNavigableHref(href)) {
                    event.preventDefault();
                }
            }, true);

            navHost.setAttribute("data-admin-avatar-links-bound", "1");
        }

        toggle.setAttribute("data-admin-avatar-bound", "1");
    }

    function toNumber(value, fallback) {
        var num = parseInt(value, 10);
        if (isNaN(num)) {
            return fallback;
        }
        return num;
    }

    function ensureInlineFeedbackHost() {
        var root = document.querySelector(".admin-page-surface") || document.body;
        if (!root) {
            return null;
        }

        var host = document.getElementById("admin-inline-feedback");
        if (!host) {
            host = document.createElement("div");
            host.id = "admin-inline-feedback";
            host.className = "admin-inline-feedback";
            root.insertBefore(host, root.firstChild);
        }

        return host;
    }

    function ensureInlineBusyBadge() {
        var badge = document.getElementById("admin-inline-busy");
        if (badge) {
            return badge;
        }

        badge = document.createElement("div");
        badge.id = "admin-inline-busy";
        badge.className = "admin-inline-busy";
        badge.textContent = "Đang xử lý...";
        document.body.appendChild(badge);
        return badge;
    }

    function showInlineBusy(text) {
        var badge = ensureInlineBusyBadge();
        if (!badge) {
            return;
        }
        badge.textContent = text || "Đang xử lý...";
        badge.classList.add("show");
    }

    function hideInlineBusy() {
        var badge = ensureInlineBusyBadge();
        if (!badge) {
            return;
        }
        badge.classList.remove("show");
    }

    function toneFromClass(clsText) {
        var cls = (clsText || "").toLowerCase();
        if (cls.indexOf("alert") >= 0 || cls.indexOf("error") >= 0 || cls.indexOf("danger") >= 0) {
            return "tone-alert";
        }
        if (cls.indexOf("success") >= 0) {
            return "tone-success";
        }
        if (cls.indexOf("warning") >= 0 || cls.indexOf("yellow") >= 0) {
            return "tone-warning";
        }
        return "tone-info";
    }

    function removeInlineItem(node) {
        if (node && node.parentNode) {
            node.parentNode.removeChild(node);
        }
    }

    function buildInlineItem(title, htmlContent, tone, allowClose) {
        var item = document.createElement("section");
        item.className = "admin-inline-item " + (tone || "tone-info");

        var head = document.createElement("div");
        head.className = "admin-inline-head";

        var titleEl = document.createElement("div");
        titleEl.className = "admin-inline-title";
        titleEl.textContent = title || "Thông báo";
        head.appendChild(titleEl);

        if (allowClose) {
            var closer = document.createElement("button");
            closer.type = "button";
            closer.className = "admin-inline-close";
            closer.innerHTML = "&times;";
            closer.onclick = function () {
                removeInlineItem(item);
            };
            head.appendChild(closer);
        }

        var body = document.createElement("div");
        body.className = "admin-inline-body";
        body.innerHTML = htmlContent || "";

        item.appendChild(head);
        item.appendChild(body);
        return item;
    }

    function renderInlineNotify(title, message, options) {
        var host = ensureInlineFeedbackHost();
        if (!host) {
            return null;
        }

        var opts = options || {};
        var item = buildInlineItem(title || "Thông báo", message || "", toneFromClass(opts.cls), true);
        host.insertBefore(item, host.firstChild);

        var timeout = toNumber(opts.timeout, toNumber(adminNotifyConfig.timeout, 4000));
        if (timeout > 0) {
            window.setTimeout(function () {
                removeInlineItem(item);
            }, timeout);
        }
        return item;
    }

    function renderInlineDialog(options) {
        var host = ensureInlineFeedbackHost();
        if (!host) {
            return null;
        }

        var opts = options || {};
        var actions = Array.isArray(opts.actions) ? opts.actions : [];

        var tone = toneFromClass(opts.cls);
        if (tone === "tone-info" && actions.length) {
            tone = toneFromClass(actions[0].cls || "");
        }

        var item = buildInlineItem(opts.title || "Thông báo", opts.content || "", tone, opts.closeButton !== false);

        var actionWrap = document.createElement("div");
        actionWrap.className = "admin-inline-actions";

        if (!actions.length) {
            actions.push({ caption: "Đóng", cls: "js-dialog-close" });
        }

        for (var i = 0; i < actions.length; i += 1) {
            (function (action) {
                var btn = document.createElement("button");
                btn.type = "button";
                btn.className = "admin-inline-btn";

                var actCls = (action && action.cls) ? action.cls : "";
                if (/\balert\b/i.test(actCls)) {
                    btn.classList.add("alert");
                } else if (/\bsuccess\b/i.test(actCls) || /\bwarning\b/i.test(actCls)) {
                    btn.classList.add("success");
                }

                btn.textContent = (action && action.caption) ? action.caption : "Xác nhận";
                btn.onclick = function (e) {
                    if (action && typeof action.onclick === "function") {
                        try {
                            action.onclick.call(btn, e);
                        } catch (err) {
                            // Keep UI responsive even when action callback throws.
                        }
                    }

                    if (!action || /\bjs-dialog-close\b/i.test(actCls) || typeof action.onclick !== "function") {
                        removeInlineItem(item);
                    }
                };
                actionWrap.appendChild(btn);
            })(actions[i]);
        }

        item.appendChild(actionWrap);
        host.insertBefore(item, host.firstChild);

        window.scrollTo(0, 0);
        return item;
    }

    function installMetroAdapters() {
        if (!window.Metro) {
            return;
        }

        if (Metro.notify && !window.__adminNotifyPatched) {
            Metro.notify.setup = function (cfg) {
                adminNotifyConfig = cfg || {};
                return Metro.notify;
            };

            Metro.notify.create = function (message, title, options) {
                var opts = options || {};
                if (opts.timeout === undefined || opts.timeout === null) {
                    opts.timeout = adminNotifyConfig.timeout;
                }
                return renderInlineNotify(title, message, opts);
            };
            window.__adminNotifyPatched = true;
        }

        if (Metro.dialog && !window.__adminDialogPatched) {
            Metro.dialog.create = function (options) {
                return renderInlineDialog(options);
            };
            window.__adminDialogPatched = true;
        }

        if (Metro.activity && !window.__adminActivityPatched) {
            Metro.activity.open = function () {
                adminBusyCounter += 1;
                showInlineBusy("Đang xử lý...");
                return true;
            };

            Metro.activity.close = function () {
                adminBusyCounter = Math.max(0, adminBusyCounter - 1);
                if (!adminBusyCounter) {
                    hideInlineBusy();
                }
                return true;
            };
            window.__adminActivityPatched = true;
        }
    }

    function extractConfirmMessage(attr) {
        if (!attr) {
            return "";
        }

        var match = attr.match(/confirm\((['"])((?:\\.|(?!\1).)*)\1\)/i);
        if (!match) {
            return "";
        }

        return match[2]
            .replace(/\\'/g, "'")
            .replace(/\\"/g, "\"")
            .replace(/\\\\/g, "\\");
    }

    function installInlineConfirmAdapter() {
        if (!window.adminInlineConfirm) {
            window.adminInlineConfirm = function (triggerEl) {
                if (!triggerEl) {
                    return false;
                }

                if (triggerEl.getAttribute("data-admin-confirm-approved") === "1") {
                    triggerEl.removeAttribute("data-admin-confirm-approved");
                    return true;
                }

                var message = triggerEl.getAttribute("data-admin-confirm-message") || "Bạn đã chắc chắn chưa?";
                renderInlineDialog({
                    title: "Xác nhận thao tác",
                    content: "<div>" + escapeHtml(message) + "</div>",
                    closeButton: true,
                    actions: [
                        { caption: "Hủy", cls: "js-dialog-close" },
                        {
                            caption: "Xác nhận",
                            cls: "js-dialog-close alert",
                            onclick: function () {
                                triggerEl.setAttribute("data-admin-confirm-approved", "1");
                                triggerEl.click();
                            }
                        }
                    ]
                });
                return false;
            };
        }

        var elements = document.querySelectorAll("[onclick*='confirm(']");
        for (var i = 0; i < elements.length; i += 1) {
            var el = elements[i];
            if (el.getAttribute("data-admin-confirm-routed") === "1") {
                continue;
            }

            var attr = el.getAttribute("onclick") || "";
            var message = extractConfirmMessage(attr);
            if (!message) {
                continue;
            }

            var rewritten = attr.replace(/return\s*confirm\((['"])(?:\\.|(?!\1).)*\1\)\s*;?/i, "return window.adminInlineConfirm(this);");
            if (rewritten === attr) {
                continue;
            }

            el.setAttribute("data-admin-confirm-message", message);
            el.setAttribute("onclick", rewritten);
            el.setAttribute("data-admin-confirm-routed", "1");
        }
    }

    function collectPopupForms() {
        return Array.prototype.slice.call(document.querySelectorAll("[id^='form_']"));
    }

    function rememberDefaultDisplay(forms) {
        for (var i = 0; i < forms.length; i += 1) {
            var form = forms[i];
            if (!form.hasAttribute("data-admin-original-display")) {
                form.setAttribute("data-admin-original-display", form.style.display || "");
            }
        }
    }

    function restoreHiddenSiblings() {
        var nodes = document.querySelectorAll("[data-admin-hidden-by-view='1']");
        for (var i = 0; i < nodes.length; i += 1) {
            var node = nodes[i];
            var original = node.getAttribute("data-admin-original-display") || "";
            node.style.display = original;
            node.removeAttribute("data-admin-hidden-by-view");
        }
    }

    function ensureBackBar(formEl) {
        if (!formEl || formEl.querySelector(".admin-form-backbar")) {
            return;
        }

        var titleNode = formEl.querySelector("h5, .h5, h4, .h4");
        var title = titleNode ? titleNode.textContent.replace(/\s+/g, " ").trim() : "Màn hình thao tác";

        var bar = document.createElement("div");
        bar.className = "admin-form-backbar";

        var label = document.createElement("div");
        label.textContent = title;

        var btn = document.createElement("button");
        btn.type = "button";
        btn.className = "admin-back-btn";
        btn.textContent = "Quay lại";
        btn.onclick = function () {
            setViewParam("");
        };

        bar.appendChild(label);
        bar.appendChild(btn);
        formEl.insertBefore(bar, formEl.firstChild);
    }

    function applyFormView() {
        var forms = collectPopupForms();
        if (!forms.length) {
            return;
        }

        rememberDefaultDisplay(forms);
        restoreHiddenSiblings();

        for (var i = 0; i < forms.length; i += 1) {
            var form = forms[i];
            form.classList.remove("admin-form-as-page");
            form.style.display = form.getAttribute("data-admin-original-display") || "";
        }

        document.body.classList.remove("admin-has-form-view");

        var activeId = getParam(VIEW_PARAM);
        if (!activeId) {
            return;
        }

        var activeForm = document.getElementById(activeId);
        if (!activeForm) {
            return;
        }

        var parent = activeForm.parentElement;
        if (parent && parent.children) {
            for (var j = 0; j < parent.children.length; j += 1) {
                var child = parent.children[j];
                if (child === activeForm) {
                    continue;
                }
                if (!child.hasAttribute("data-admin-original-display")) {
                    child.setAttribute("data-admin-original-display", child.style.display || "");
                }
                child.style.display = "none";
                child.setAttribute("data-admin-hidden-by-view", "1");
            }
        }

        activeForm.classList.add("admin-form-as-page");
        activeForm.style.display = "block";
        ensureBackBar(activeForm);

        document.body.classList.add("admin-has-form-view");
        window.scrollTo(0, 0);
    }

    function extractFormIdFromOnclickAttr(el) {
        if (!el || !el.getAttribute) {
            return "";
        }

        var attr = el.getAttribute("onclick") || "";
        if (!attr) {
            return "";
        }

        var match = attr.match(/show_hide_id_(form_[a-z0-9_]+)\s*\(/i);
        return match ? match[1] : "";
    }

    function isSubmitLikeElement(el) {
        if (!el || !el.tagName) {
            return false;
        }

        var tag = el.tagName.toLowerCase();
        if (tag === "button") {
            return true;
        }

        if (tag === "input") {
            var type = (el.getAttribute("type") || "").toLowerCase();
            return type === "submit" || type === "button" || type === "image";
        }

        return false;
    }

    function installPopupRouteHandler() {
        if (window.__adminPopupRouteBound) {
            return;
        }

        document.addEventListener("click", function (e) {
            var target = e.target;
            if (!target || !target.closest) {
                return;
            }

            var opener = target.closest("[onclick*='show_hide_id_form_']");
            if (!opener) {
                return;
            }

            var formId = extractFormIdFromOnclickAttr(opener);
            if (!formId) {
                return;
            }

            // Keep server submit flow untouched.
            if (isSubmitLikeElement(opener)) {
                if (getParam(VIEW_PARAM) === formId) {
                    setFormActionWithoutViewParam();
                }
                return;
            }

            e.preventDefault();
            e.stopPropagation();

            var current = getParam(VIEW_PARAM);
            if (current === formId) {
                setViewParam("");
            } else {
                setViewParam(formId);
            }
        }, true);

        window.__adminPopupRouteBound = true;
    }

    function initAdminModern() {
        installMetroAdapters();
        installInlineConfirmAdapter();
        initSideMenuDropdowns();
        buildAvatarMenu();
        initAvatarMenuToggle();
        installPopupRouteHandler();
        applyFormView();
    }

    onReady(function () {
        initAdminModern();

        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            if (prm && !window.__adminModernPRMHooked) {
                prm.add_beginRequest(function () {
                    adminBusyCounter += 1;
                    showInlineBusy("Đang xử lý...");
                });
                prm.add_endRequest(function () {
                    adminBusyCounter = 0;
                    hideInlineBusy();
                    initAdminModern();
                });
                window.__adminModernPRMHooked = true;
            }
        }
    });
})();
