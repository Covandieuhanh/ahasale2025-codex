(function () {
    function getCapabilities() {
        var defaults = {
            show_home_saved: true,
            show_home_inbox: true,
            show_home_future_tabs: false,
            show_future_account_items: false,
            show_future_vertical_actions: false,
            show_bds_future_actions: false,
            show_xe_future_actions: false,
            show_tech_future_actions: false,
            show_gianhang_future_ops: false
        };
        var external = window.AhaAppUiCapabilities || {};
        var merged = {};
        Object.keys(defaults).forEach(function (key) {
            merged[key] = Object.prototype.hasOwnProperty.call(external, key) ? !!external[key] : defaults[key];
        });
        return merged;
    }

    function hasCapability(name) {
        if (!name) return true;
        return !!getCapabilities()[name];
    }

    function applyCapabilityVisibility() {
        var nodes = document.querySelectorAll("[data-app-capability]");
        nodes.forEach(function (node) {
            var name = node.getAttribute("data-app-capability");
            if (!hasCapability(name)) {
                node.remove();
            }
        });
    }

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

    function getCurrentUrl() {
        return sanitizeAppHref(window.location.pathname + window.location.search + window.location.hash);
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

    function decorateAppHref(url) {
        var safeUrl = sanitizeAppHref(url);
        if (!safeUrl || safeUrl.indexOf("/app-ui/") !== 0) return safeUrl;
        if (safeUrl.indexOf("/app-ui/auth/") === 0) return appendQuery(safeUrl, "return_url", getCurrentUrl());
        return appendQuery(safeUrl, "back_href", getCurrentUrl());
    }

    function getCurrentPath() {
        return window.location.pathname || "";
    }

    function normalizeComparableHref(value) {
        if (!value) return "";
        try {
            var url = new URL(value, window.location.origin);
            return sanitizeAppHref(url.pathname + (url.search || "") + (url.hash || ""));
        } catch (e) {
            return sanitizeAppHref(String(value));
        }
    }

    function navigateIfChanged(target) {
        if (!target) return;
        var next = normalizeComparableHref(target);
        var current = normalizeComparableHref(window.location.pathname + window.location.search + window.location.hash);
        if (!next || next === current) return;
        window.location.href = target;
    }

    function readSession() {
        return window.AhaAppUiAdapters && window.AhaAppUiAdapters.session
            ? window.AhaAppUiAdapters.session.getSession()
            : {
                auth_state: "authenticated",
                seller_status: "not_registered",
                user_summary: {
                    display_name: "Home 0326824915",
                    username: "home home home",
                    phone: "0326824915",
                    role_label: "Khách hàng",
                    avatar_fallback: "HM"
                }
            };
    }

    function getSearchRoute(space) {
        if (space === "batdongsan") return decorateAppHref("/app-ui/batdongsan/search.aspx?ui_mode=app");
        if (space === "choxe") return decorateAppHref("/app-ui/choxe/search.aspx?ui_mode=app");
        if (space === "dienthoai-maytinh") return decorateAppHref("/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app");
        if (space === "gianhang") return decorateAppHref("/app-ui/gianhang/listing.aspx?ui_mode=app");
        return decorateAppHref("/app-ui/home/search.aspx?ui_mode=app");
    }

    function getCategoryRoute(space) {
        if (space === "home") return decorateAppHref("/app-ui/home/categories.aspx?ui_mode=app");
        if (space === "batdongsan") return decorateAppHref("/app-ui/batdongsan/hub.aspx?ui_mode=app");
        if (space === "choxe") return decorateAppHref("/app-ui/choxe/hub.aspx?ui_mode=app");
        if (space === "dienthoai-maytinh") return decorateAppHref("/app-ui/dienthoai-maytinh/hub.aspx?ui_mode=app");
        return decorateAppHref("/app-ui/gianhang/listing.aspx?ui_mode=app");
    }

    function getNotificationRoute(space) {
        if (space === "batdongsan") return decorateAppHref("/app-ui/batdongsan/notifications.aspx?ui_mode=app");
        if (space === "choxe") return decorateAppHref("/app-ui/choxe/notifications.aspx?ui_mode=app");
        if (space === "dienthoai-maytinh") return decorateAppHref("/app-ui/dienthoai-maytinh/notifications.aspx?ui_mode=app");
        if (space === "gianhang") return decorateAppHref("/app-ui/gianhang/notifications.aspx?ui_mode=app");
        return decorateAppHref("/app-ui/home/notifications.aspx?ui_mode=app");
    }

    function getHomeRoute(space) {
        var registry = window.AhaAppUiRegistry || [];
        for (var i = 0; i < registry.length; i += 1) {
            if (registry[i].code === space) return decorateAppHref(registry[i].href + "?ui_mode=app");
        }
        return decorateAppHref("/app-ui/home/default.aspx?ui_mode=app");
    }

    function getAccountRoute() {
        return "#app-account";
    }

    function getPostTarget(session) {
        var currentUrl = getCurrentUrl();
        if (!session || session.auth_state !== "authenticated") {
            return {
                href: appendQuery("/app-ui/auth/login.aspx?ui_mode=app&intent=post", "return_url", currentUrl),
                label: "Đăng nhập để đăng tin"
            };
        }
        var sellerStatus = (session && session.seller_status) || "not_registered";
        if (sellerStatus === "approved") {
            return {
                href: appendQuery("/app-ui/gianhang/post.aspx?ui_mode=app", "back_href", currentUrl),
                label: "Đăng tin"
            };
        }
        if (sellerStatus === "pending") {
            return {
                href: appendQuery("/app-ui/auth/pending-approval.aspx?ui_mode=app&intent=post", "return_url", currentUrl),
                label: "Chờ duyệt gian hàng"
            };
        }
        return {
            href: appendQuery("/app-ui/auth/open-shop.aspx?ui_mode=app&intent=post", "return_url", currentUrl),
            label: "Đăng ký gian hàng"
        };
    }

    async function hydrateNotificationBadge(currentSpace) {
        var badge = document.querySelector(".app-icon-bell .app-badge");
        if (!badge) return;

        try {
            if (currentSpace === "gianhang") {
                var sellerAdapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.seller;
                if (!sellerAdapter || typeof sellerAdapter.getSnapshotAsync !== "function") {
                    badge.textContent = "0";
                    return;
                }
                var snapshot = await sellerAdapter.getSnapshotAsync();
                var report = snapshot && snapshot.reportSummary ? snapshot.reportSummary : null;
                var pendingOrders = report && report.pending_orders ? Number(report.pending_orders) : 0;
                var pendingBookings = report && report.booking_pending ? Number(report.booking_pending) : 0;
                var conversationCount = snapshot && snapshot.conversations ? snapshot.conversations.length : 0;
                badge.textContent = String(Math.max(0, pendingOrders + pendingBookings + conversationCount));
                return;
            }

            var searchAdapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.search;
            if (!searchAdapter || typeof searchAdapter.searchAsync !== "function") {
                badge.textContent = "0";
                return;
            }

            var result = await searchAdapter.searchAsync({
                q: "",
                space_code: currentSpace,
                sort: "newest",
                page: 1,
                page_size: 3,
                filters: {}
            });
            var items = result && result.items ? result.items : [];
            badge.textContent = String(items.length);
        } catch (e) {
            badge.textContent = "0";
        }
    }

    function renderChipLane(selector, items) {
        var host = document.querySelector(selector);
        if (!host || !items || !items.length) return;
        host.innerHTML = items.map(function (item) {
            return '<button class="app-chip" type="button">' + item + '</button>';
        }).join("");
    }

    function createOverlay() {
        var overlay = document.createElement("div");
        overlay.className = "app-overlay";
        document.body.appendChild(overlay);
        return overlay;
    }

    function createSheet(side, title) {
        var sheet = document.createElement("aside");
        sheet.className = "app-sheet app-sheet-" + side;
        sheet.setAttribute("aria-hidden", "true");
        sheet.innerHTML = '' +
            '<div class="app-sheet-head">' +
            '  <h2 class="app-sheet-title">' + title + '</h2>' +
            '  <button class="app-sheet-close" type="button" aria-label="Đóng">×</button>' +
            '</div>' +
            '<div class="app-sheet-body"></div>';
        document.body.appendChild(sheet);
        return sheet;
    }

    function renderSpaceLauncher(host, currentSpace) {
        var registry = window.AhaAppUiRegistry || [];
        host.innerHTML = '' +
            '<div class="app-space-switch-list">' +
            registry.map(function (item) {
                var active = item.code === currentSpace ? ' is-active' : '';
                return '' +
                    '<a class="app-space-switch-item' + active + '" href="' + decorateAppHref(item.href + '?ui_mode=app') + '">' +
                    '  <span class="app-space-switch-thumb">' + item.emoji + '</span>' +
                    '  <span class="app-space-switch-copy">' +
                    '    <span class="app-space-switch-label">' + item.label + '</span>' +
                    '    <span class="app-space-switch-desc">' + item.description + '</span>' +
                    '  </span>' +
                    '  <span>&rsaquo;</span>' +
                    '</a>';
            }).join("") +
            '</div>';
    }

    function configureTopbarRevealMode(shell) {
        if (!shell) return;
        var topbar = shell.querySelector(".app-topbar");
        if (!topbar) return;
        // Force persistent topbar on all app screens.
        shell.classList.remove("app-shell-topbar-reveal");
        topbar.classList.remove("app-topbar-reveal");
        topbar.classList.add("is-visible");
    }

    function renderAccountMenu(host, currentSpace) {
        var session = readSession();
        var accountMenu = window.AhaAppUiAdapters && window.AhaAppUiAdapters.account
            ? window.AhaAppUiAdapters.account.getAccountMenu(currentSpace, session)
            : { main_menu_items: [], utility_items: [], promo_items: [], misc_items: [], scope_note: "Home" };
        var user = session.user_summary || {
            display_name: "Khách",
            username: "",
            phone: "",
            role_label: "Khách",
            avatar_fallback: "KH",
            avatar_url: ""
        };

        function copyText(text) {
            return new Promise(function (resolve, reject) {
                if (!text) {
                    reject(new Error("empty"));
                    return;
                }
                if (navigator.clipboard && typeof navigator.clipboard.writeText === "function") {
                    navigator.clipboard.writeText(text).then(resolve).catch(function () {
                        reject(new Error("clipboard"));
                    });
                    return;
                }
                var textarea = document.createElement("textarea");
                textarea.value = text;
                textarea.setAttribute("readonly", "");
                textarea.style.position = "fixed";
                textarea.style.top = "-1000px";
                document.body.appendChild(textarea);
                textarea.select();
                try {
                    var copied = document.execCommand("copy");
                    document.body.removeChild(textarea);
                    if (copied) resolve();
                    else reject(new Error("exec"));
                } catch (error) {
                    document.body.removeChild(textarea);
                    reject(error);
                }
            });
        }

        function resolveReferralLink() {
            var summary = session && session.user_summary ? session.user_summary : {};
            var account = (summary.phone || "").trim();
            if (!account) {
                account = (summary.username || "").replace(/^home\s+/i, "").trim();
            }
            if (!account) return "";
            return "https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + encodeURIComponent(account);
        }

        function renderItems(items) {
            return (items || []).filter(function (item) {
                return !item.capability || hasCapability(item.capability);
            }).map(function (item) {
                var href = item.href || "javascript:void(0)";
                if (item.action) {
                    return '  <button class="app-account-menu-item app-account-menu-button" type="button" data-app-account-action="' + item.action + '"><span><span class="app-account-menu-label">' + item.label + '</span><span class="app-account-menu-sub">' + item.sub + '</span></span><span>&rsaquo;</span></button>';
                }
                return '  <a class="app-account-menu-item" href="' + decorateAppHref(href) + '"><span><span class="app-account-menu-label">' + item.label + '</span><span class="app-account-menu-sub">' + item.sub + '</span></span><span>&rsaquo;</span></a>';
            }).join("");
        }
        var promoItemsHtml = renderItems(accountMenu.promo_items);
        var avatarHtml = user.avatar_url
            ? '<img class="app-account-avatar-image" src="' + user.avatar_url + '" alt="' + user.display_name + '" />'
            : user.avatar_fallback;
        var metaParts = [];
        if (user.username) metaParts.push(user.username);
        if (user.phone) metaParts.push(user.phone);
        var metaHtml = metaParts.length
            ? '<span class="app-account-meta">' + metaParts.join(' • ') + '</span>'
            : '';
        host.innerHTML = '' +
            '<div class="app-account-inline-notice" data-app-account-notice hidden></div>' +
            '<section class="app-account-card">' +
            '  <div class="app-account-head">' +
            '    <span class="app-account-avatar">' + avatarHtml + '</span>' +
            '    <span>' +
            '      <span class="app-account-name">' + user.display_name + '</span>' +
            metaHtml +
            '    </span>' +
            '  </div>' +
            '  <span class="app-account-role">' + user.role_label + '</span>' +
            '</section>' +
            '<span class="app-section-label">Chức năng chính</span>' +
            '<div class="app-account-menu-list">' + renderItems(accountMenu.main_menu_items) + '</div>' +
            '<span class="app-section-label">Tiện ích</span>' +
            '<div class="app-account-menu-list">' + renderItems(accountMenu.utility_items) + '</div>' +
            (promoItemsHtml ? '<span class="app-section-label">Hồ sơ quyền &amp; lịch sử</span><div class="app-account-menu-list">' + promoItemsHtml + '</div>' : '') +
            '<span class="app-section-label">Khác</span>' +
            '<div class="app-account-menu-list">' + renderItems(accountMenu.misc_items) + '</div>';

        var noticeNode = host.querySelector("[data-app-account-notice]");
        function showNotice(message, tone) {
            if (!noticeNode) return;
            noticeNode.textContent = message || "";
            noticeNode.classList.remove("is-error");
            if (tone === "error") {
                noticeNode.classList.add("is-error");
            }
            noticeNode.hidden = false;
            clearTimeout(showNotice._timer);
            showNotice._timer = setTimeout(function () {
                noticeNode.hidden = true;
            }, 2200);
        }

        host.querySelectorAll("[data-app-account-action='copy_referral_link']").forEach(function (button) {
            button.addEventListener("click", function () {
                var link = resolveReferralLink();
                if (!link) {
                    showNotice("Không có link giới thiệu để sao chép.", "error");
                    return;
                }
                copyText(link).then(function () {
                    showNotice("Đã sao chép link giới thiệu.");
                }).catch(function () {
                    showNotice("Không thể sao chép link, vui lòng thử lại.", "error");
                });
            });
        });
    }

    function ensureTopbarAnchorTargets(shell, currentSpace) {
        if (currentSpace === "home") {
            var categoriesSection = shell.querySelector(".app-space-rail");
            if (categoriesSection && !categoriesSection.id) {
                categoriesSection.id = "danh-muc-home";
            }
            if (categoriesSection) {
                categoriesSection.classList.add("app-anchor-target");
            }
        }
    }

    function ensureTopbar(shell) {
        var topbar = shell.querySelector(".app-topbar");
        if (topbar) {
            if (!topbar.querySelector(".app-safe-top")) {
                var safeTop = document.createElement("div");
                safeTop.className = "app-safe-top";
                topbar.insertBefore(safeTop, topbar.firstChild || null);
            }
            return topbar;
        }

        topbar = document.createElement("header");
        topbar.className = "app-topbar";
        topbar.innerHTML = '' +
            '<div class="app-safe-top"></div>' +
            '<div class="app-topbar-row">' +
            '  <button class="app-icon-btn" type="button" aria-label="Chuyển không gian" title="Chuyển không gian" data-app-trigger="menu">' +
            '    <span></span><span></span><span></span>' +
            '  </button>' +
            '  <div class="app-search-pill" role="search" data-app-search-form="1">' +
            '    <span class="app-search-icon"></span>' +
            '    <input type="search" placeholder="Tìm kiếm nhanh" data-app-search-input="1" />' +
            '    <button class="app-search-submit" type="button" aria-label="Tìm" data-app-search-submit="1">→</button>' +
            '  </div>' +
            '  <button class="app-icon-btn app-icon-bell" type="button" aria-label="Thông báo" title="Thông báo">' +
            '    <span class="app-badge">0</span>' +
            '  </button>' +
            '</div>';

        var main = shell.querySelector(".app-main");
        if (main) {
            shell.insertBefore(topbar, main);
        } else {
            shell.prepend(topbar);
        }
        return topbar;
    }

    function ensureSearchPillInput(shell, searchPill, currentSpace) {
        if (!searchPill) return;
        searchPill.className = "app-search-pill";
        searchPill.setAttribute("role", "search");
        searchPill.setAttribute("data-app-search-form", "1");
        searchPill.setAttribute("data-app-search-space", currentSpace || "home");
        searchPill.removeAttribute("data-app-search-link");
        searchPill.removeAttribute("tabindex");
        searchPill.setAttribute("title", "Tìm kiếm nhanh");

        var input = searchPill.querySelector("[data-app-search-input]") || searchPill.querySelector("input[type='search']");
        if (!input) {
            input = document.createElement("input");
            input.type = "search";
            input.setAttribute("data-app-search-input", "1");
        }
        input.placeholder = "Tìm kiếm nhanh";

        var submit = searchPill.querySelector("[data-app-search-submit]") || searchPill.querySelector(".app-search-submit");
        if (!submit) {
            submit = document.createElement("button");
            submit.type = "button";
            submit.className = "app-search-submit";
            submit.setAttribute("aria-label", "Tìm");
            submit.setAttribute("data-app-search-submit", "1");
            submit.textContent = "→";
        }

        searchPill.innerHTML = "";
        var icon = document.createElement("span");
        icon.className = "app-search-icon";
        searchPill.appendChild(icon);
        searchPill.appendChild(input);
        searchPill.appendChild(submit);
    }

    function normalizeTopbar(shell, currentSpace) {
        var topbar = ensureTopbar(shell);
        if (!topbar) return;
        topbar.className = "app-topbar";

        var row = topbar.querySelector(".app-topbar-row");
        if (!row) return;
        var isNotificationPage = /\/notifications\.aspx$/i.test(getCurrentPath());
        var menuButton = row.querySelector("[data-app-trigger='menu']") || document.createElement("button");
        menuButton.className = "app-icon-btn";
        menuButton.type = "button";
        menuButton.setAttribute("aria-label", "Chuyển không gian");
        menuButton.setAttribute("title", "Chuyển không gian");
        menuButton.setAttribute("data-app-trigger", "menu");
        if (!menuButton.innerHTML.trim()) {
            menuButton.innerHTML = "<span></span><span></span><span></span>";
        }

        var searchPill = row.querySelector(".app-search-pill") || document.createElement("div");
        ensureSearchPillInput(shell, searchPill, currentSpace);

        var bellButton = row.querySelector(".app-icon-bell") || document.createElement("button");
        bellButton.className = "app-icon-btn app-icon-bell";
        bellButton.type = "button";
        bellButton.setAttribute("aria-label", "Thông báo");
        bellButton.setAttribute("title", "Thông báo");
        if (isNotificationPage) {
            bellButton.removeAttribute("data-app-open-link");
            bellButton.setAttribute("aria-current", "page");
            bellButton.classList.add("is-active");
        } else {
            bellButton.setAttribute("data-app-open-link", getNotificationRoute(currentSpace));
            bellButton.removeAttribute("aria-current");
            bellButton.classList.remove("is-active");
        }
        if (!bellButton.querySelector(".app-badge")) {
            bellButton.innerHTML = '<span class="app-badge">0</span>';
        }

        row.innerHTML = "";
        row.appendChild(menuButton);
        row.appendChild(searchPill);
        row.appendChild(bellButton);
    }

    function readNavHistory() {
        try {
            var raw = sessionStorage.getItem("aha_app_ui_nav_history");
            return raw ? JSON.parse(raw) : { stack: [], index: -1 };
        } catch (e) {
            return { stack: [], index: -1 };
        }
    }

    function writeNavHistory(state) {
        try {
            sessionStorage.setItem("aha_app_ui_nav_history", JSON.stringify(state));
        } catch (e) { }
    }

    function isTransientHistoryRoute(href) {
        return /\/notifications\.aspx(?:[?#]|$)/i.test(href || "");
    }

    function normalizeNavHistoryState(state) {
        var safeState = state && typeof state === "object" ? state : {};
        var sourceStack = Array.isArray(safeState.stack) ? safeState.stack : [];
        var sourceIndex = typeof safeState.index === "number" ? safeState.index : -1;
        var sourcePendingIndex = typeof safeState.pendingIndex === "number" ? safeState.pendingIndex : -1;
        var stack = [];
        var index = -1;
        var pendingIndex = -1;

        sourceStack.forEach(function (item, sourcePosition) {
            if (!item || isTransientHistoryRoute(item)) return;
            stack.push(item);
            var normalizedPosition = stack.length - 1;
            if (sourcePosition <= sourceIndex) index = normalizedPosition;
            if (sourcePosition === sourcePendingIndex) pendingIndex = normalizedPosition;
        });

        if (!stack.length) {
            index = -1;
            pendingIndex = -1;
        } else if (index >= stack.length) {
            index = stack.length - 1;
        }

        var normalized = {
            stack: stack,
            index: index
        };
        if (pendingIndex >= 0 && pendingIndex < stack.length) {
            normalized.pendingIndex = pendingIndex;
        }
        return normalized;
    }

    function syncNavHistory() {
        var state = normalizeNavHistoryState(readNavHistory());
        var current = getCurrentUrl();
        if (isTransientHistoryRoute(current)) {
            writeNavHistory(state);
            return state;
        }
        var pendingIndex = typeof state.pendingIndex === "number" ? state.pendingIndex : null;
        if (pendingIndex !== null && state.stack[pendingIndex] === current) {
            state.index = pendingIndex;
            delete state.pendingIndex;
            writeNavHistory(state);
            return state;
        }

        if (state.index >= 0 && state.stack[state.index] === current) {
            return state;
        }

        var existingIndex = state.stack.indexOf(current);
        if (existingIndex !== -1) {
            state.index = existingIndex;
            delete state.pendingIndex;
            writeNavHistory(state);
            return state;
        }

        var baseIndex = state.index >= 0 ? state.index : state.stack.length - 1;
        state.stack = state.stack.slice(0, baseIndex + 1);
        state.stack.push(current);
        if (state.stack.length > 40) {
            state.stack = state.stack.slice(state.stack.length - 40);
        }
        state.index = state.stack.length - 1;
        delete state.pendingIndex;
        writeNavHistory(state);
        return state;
    }

    function renderHistoryStrip(shell) {
        if (!shell) return;
        shell.classList.remove("app-shell-has-history-strip");
        if (shell && (shell.getAttribute("data-app-topbar-mode") || "").trim().toLowerCase() === "reveal-on-scroll") {
            return;
        }
        var topbar = shell.querySelector(".app-topbar");
        if (!topbar) return;
        var state = syncNavHistory();
        var canBack = state.index > 0;
        var canForward = state.index >= 0 && state.index < state.stack.length - 1;
        var existing = shell.querySelector(".app-history-strip");
        if (existing) existing.remove();
        if (!canBack && !canForward) return;

        var strip = document.createElement("div");
        strip.className = "app-history-strip";
        strip.innerHTML = '' +
            (canBack ? '<button class="app-history-btn" type="button" data-app-history="back">&larr; Quay lại</button>' : '') +
            (canForward ? '<button class="app-history-btn" type="button" data-app-history="forward">Chuyển tiếp &rarr;</button>' : '');
        topbar.insertAdjacentElement("afterend", strip);
        shell.classList.add("app-shell-has-history-strip");

        strip.addEventListener("click", function (event) {
            var button = event.target.closest("[data-app-history]");
            if (!button) return;
            var direction = button.getAttribute("data-app-history");
            var currentState = normalizeNavHistoryState(readNavHistory());
            var targetIndex = direction === "back" ? currentState.index - 1 : currentState.index + 1;
            if (targetIndex < 0 || targetIndex >= currentState.stack.length) return;
            currentState.pendingIndex = targetIndex;
            writeNavHistory(currentState);
            window.location.href = currentState.stack[targetIndex];
        });
    }

    function getBottomNavConfig(currentSpace) {
        var session = readSession();
        var postTarget = getPostTarget(session);
        if (currentSpace === "gianhang") {
            return {
                isSeller: true,
                centerHref: postTarget.href,
                centerLabel: "Đăng tin",
                centerAriaLabel: postTarget.label,
                items: [
                    { key: "home", label: "Trang chủ", href: getHomeRoute("gianhang"), icon: "app-bottom-home" },
                    { key: "listing", label: "Quản lý tin", href: "/app-ui/gianhang/listing.aspx?ui_mode=app", icon: "app-bottom-category" },
                    { key: "customers", label: "Khách hàng", href: "/app-ui/gianhang/conversations.aspx?ui_mode=app", icon: "app-bottom-chat" },
                    { key: "account", label: "Tài khoản", href: getAccountRoute(), icon: "app-bottom-user", trigger: "account" }
                ]
            };
        }

        return {
            isSeller: false,
            centerHref: getSearchRoute(currentSpace),
            centerLabel: "Tìm kiếm",
            centerAriaLabel: "Tìm kiếm",
            items: [
                { key: "home", label: "Trang chủ", href: getHomeRoute(currentSpace), icon: "app-bottom-home" },
                { key: "categories", label: "Danh mục", href: getCategoryRoute(currentSpace), icon: "app-bottom-category" },
                { key: "post", label: "Đăng tin", href: postTarget.href, icon: "app-bottom-post" },
                { key: "account", label: "Tài khoản", href: getAccountRoute(), icon: "app-bottom-user", trigger: "account" }
            ]
        };
    }

    function getActiveBottomNavKey(currentSpace) {
        var shell = document.querySelector("[data-app-shell]");
        var explicit = shell ? shell.getAttribute("data-app-nav-active") : "";
        if (explicit) return explicit;
        var path = getCurrentPath();
        if (currentSpace === "gianhang") {
            if (/\/app-ui\/gianhang\/listing\.aspx$/i.test(path)) return "listing";
            if (/\/app-ui\/gianhang\/detail\.aspx$/i.test(path)) return "listing";
            if (/\/app-ui\/gianhang\/status\.aspx$/i.test(path)) return "listing";
            if (/\/app-ui\/gianhang\/actions\.aspx$/i.test(path)) return "post";
            if (/\/app-ui\/gianhang\/conversations\.aspx$/i.test(path)) return "customers";
            if (/\/app-ui\/gianhang\/default\.aspx$/i.test(path)) return "home";
            return "";
        }

        if (/\/search\.aspx$/i.test(path)) return "search";
        if (currentSpace !== "home" && /\/hub\.aspx$/i.test(path)) return "categories";
        if (currentSpace === "home" && /\/home\/categories\.aspx$/i.test(path)) return "categories";
        if (currentSpace === "home" && window.location.hash === "#danh-muc-home") return "categories";
        if (/\/default\.aspx$/i.test(path)) return "home";
        return "";
    }

    function renderBottomNav(shell, currentSpace) {
        var config = getBottomNavConfig(currentSpace);
        var activeKey = getActiveBottomNavKey(currentSpace);
        var existing = shell.querySelector(".app-bottom-nav");
        if (existing) existing.remove();

        var nav = document.createElement("nav");
        nav.className = "app-bottom-nav" + (config.isSeller ? " app-bottom-nav-seller" : " app-bottom-nav-consumer");
        nav.setAttribute("aria-label", currentSpace === "gianhang" ? "Điều hướng gian hàng" : "Điều hướng không gian");

        function buildItem(item) {
            var active = activeKey === item.key ? " is-active" : "";
            if (item.trigger) {
                return '<button class="app-bottom-item' + active + '" type="button" data-app-trigger="' + item.trigger + '" title="' + item.label + '"' + (active ? ' aria-current="page"' : '') + '>' +
                    '<span class="app-bottom-icon ' + item.icon + '"></span>' +
                    '<span>' + item.label + '</span>' +
                    '</button>';
            }
            return '<a class="app-bottom-item' + active + '" href="' + decorateAppHref(item.href) + '" title="' + item.label + '"' + (active ? ' aria-current="page"' : '') + '>' +
                '<span class="app-bottom-icon ' + item.icon + '"></span>' +
                '<span>' + item.label + '</span>' +
                '</a>';
        }

        if (config.isSeller) {
            nav.innerHTML = '' +
                buildItem(config.items[0]) +
                buildItem(config.items[1]) +
                '<a class="app-post-cta' + (activeKey === "post" ? ' is-active' : '') + '" href="' + config.centerHref + '" aria-label="' + config.centerAriaLabel + '" title="' + config.centerLabel + '"' + (activeKey === "post" ? ' aria-current="page"' : '') + '>+</a>' +
                buildItem(config.items[2]) +
                buildItem(config.items[3]);
        } else {
            nav.innerHTML = '' +
                buildItem(config.items[0]) +
                buildItem(config.items[1]) +
                '<a class="app-search-cta' + (activeKey === "search" ? ' is-active' : '') + '" href="' + config.centerHref + '" aria-label="' + config.centerAriaLabel + '" title="' + config.centerLabel + '"' + (activeKey === "search" ? ' aria-current="page"' : '') + '>' +
                '  <span class="app-search-cta-icon"></span>' +
                '</a>' +
                buildItem(config.items[2]) +
                buildItem(config.items[3]);
        }

        shell.appendChild(nav);
    }

    function bindSheets() {
        var shell = document.querySelector("[data-app-shell]");
        if (!shell) return;

        var currentSpace = shell.getAttribute("data-app-shell") || "home";
        if (currentSpace === "launcher") return;
        ensureTopbar(shell);
        ensureTopbarAnchorTargets(shell, currentSpace);
        normalizeTopbar(shell, currentSpace);
        configureTopbarRevealMode(shell);
        renderHistoryStrip(shell);
        renderBottomNav(shell, currentSpace);
        hydrateNotificationBadge(currentSpace);

        var menuTrigger = document.querySelector("[data-app-trigger='menu']");
        var accountTriggers = document.querySelectorAll("[data-app-trigger='account']");
        if (!menuTrigger && !accountTriggers.length) return;

        var overlay = createOverlay();
        var leftSheet = createSheet("left", "Không gian");
        var rightSheet = createSheet("right", "Tài khoản");
        renderSpaceLauncher(leftSheet.querySelector(".app-sheet-body"), currentSpace);
        renderAccountMenu(rightSheet.querySelector(".app-sheet-body"), currentSpace);

        function closeAll() {
            overlay.classList.remove("is-open");
            leftSheet.classList.remove("is-open");
            rightSheet.classList.remove("is-open");
            leftSheet.setAttribute("aria-hidden", "true");
            rightSheet.setAttribute("aria-hidden", "true");
        }

        function openSheet(side) {
            closeAll();
            overlay.classList.add("is-open");
            if (side === "left") {
                leftSheet.classList.add("is-open");
                leftSheet.setAttribute("aria-hidden", "false");
            } else {
                rightSheet.classList.add("is-open");
                rightSheet.setAttribute("aria-hidden", "false");
            }
        }

        overlay.addEventListener("click", closeAll);
        leftSheet.querySelector(".app-sheet-close").addEventListener("click", closeAll);
        rightSheet.querySelector(".app-sheet-close").addEventListener("click", closeAll);

        if (menuTrigger) {
            menuTrigger.addEventListener("click", function () {
                openSheet("left");
            });
        }
        accountTriggers.forEach(function (trigger) {
            trigger.addEventListener("click", function () {
                openSheet("right");
            });
        });
    }

    function alignAnchorTargets() {
        if (!window.location.hash) return;
        var id = window.location.hash.replace(/^#/, "");
        if (!id) return;
        var target = document.getElementById(id);
        if (!target) return;
        window.requestAnimationFrame(function () {
            target.scrollIntoView({ block: "start", behavior: "smooth" });
        });
    }

    function bindSearchForms() {
        function submitHost(host) {
            var input = host.querySelector("[data-app-search-input]");
            var space = host.getAttribute("data-app-search-space") || "home";
            var value = input ? (input.value || "").trim() : "";
            var target = getSearchRoute(space);
            if (value) {
                target += "&q=" + encodeURIComponent(value);
            }
            navigateIfChanged(decorateAppHref(target));
        }

        var hosts = document.querySelectorAll("[data-app-search-form]");
        hosts.forEach(function (host) {
            if (host.dataset.bound === "1") return;
            host.dataset.bound = "1";
            var input = host.querySelector("[data-app-search-input]");
            var button = host.querySelector("[data-app-search-submit]");
            if (button) {
                button.addEventListener("click", function () {
                    submitHost(host);
                });
            }
            if (input) {
                input.addEventListener("keydown", function (event) {
                    if (event.key === "Enter") {
                        event.preventDefault();
                        submitHost(host);
                    }
                });
            }
        });

        var links = document.querySelectorAll("[data-app-search-link]");
        links.forEach(function (host) {
            if (host.dataset.searchLinkBound === "1") return;
            host.dataset.searchLinkBound = "1";
            host.addEventListener("click", function () {
                navigateIfChanged(decorateAppHref(host.getAttribute("data-app-search-link")));
            });
            host.addEventListener("keydown", function (event) {
                if (event.key === "Enter" || event.key === " ") {
                    event.preventDefault();
                    navigateIfChanged(decorateAppHref(host.getAttribute("data-app-search-link")));
                }
            });
        });

        var openLinks = document.querySelectorAll("[data-app-open-link]");
        openLinks.forEach(function (node) {
            if (node.dataset.openLinkBound === "1") return;
            node.dataset.openLinkBound = "1";
            node.addEventListener("click", function () {
                if (node.disabled) return;
                var href = node.getAttribute("data-app-open-link");
                if (!href) return;
                navigateIfChanged(decorateAppHref(href));
            });
        });
    }

    function bindSamePageAnchorGuard() {
        if (document.body && document.body.dataset.samePageGuardBound === "1") return;
        if (document.body) {
            document.body.dataset.samePageGuardBound = "1";
        }

        document.addEventListener("click", function (event) {
            var anchor = event.target && event.target.closest ? event.target.closest("a[href]") : null;
            if (!anchor) return;
            if (anchor.hasAttribute("download")) return;
            if (anchor.target && anchor.target !== "_self") return;
            if (event.metaKey || event.ctrlKey || event.shiftKey || event.altKey || event.button !== 0) return;

            var rawHref = anchor.getAttribute("href") || "";
            if (!rawHref || rawHref === "javascript:void(0)" || rawHref.indexOf("javascript:") === 0) return;

            var next = normalizeComparableHref(rawHref);
            var current = normalizeComparableHref(window.location.pathname + window.location.search + window.location.hash);
            if (!next || !current || next !== current) return;

            event.preventDefault();
            window.scrollTo({ top: 0, behavior: "smooth" });
        });
    }

    window.AhaAppUi = window.AhaAppUi || {};
    window.AhaAppUi.renderChipLane = renderChipLane;
    window.AhaAppUi.bindSheets = bindSheets;
    window.AhaAppUi.bindSearchForms = bindSearchForms;
    window.AhaAppUi.hasCapability = hasCapability;
    window.AhaAppUi.applyCapabilityVisibility = applyCapabilityVisibility;
    window.AhaAppUi.getPostTarget = getPostTarget;

    function runWhenReady(handler) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", handler);
            return;
        }
        handler();
    }

    runWhenReady(bindSheets);
    runWhenReady(bindSearchForms);
    runWhenReady(bindSamePageAnchorGuard);
    runWhenReady(applyCapabilityVisibility);
    runWhenReady(alignAnchorTargets);
})();
