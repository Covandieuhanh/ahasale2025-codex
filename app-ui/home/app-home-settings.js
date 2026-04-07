(function () {
    function escapeHtml(value) {
        return String(value || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function getSession() {
        var adapter = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session;
        if (adapter && typeof adapter.getSession === "function") {
            return adapter.getSession();
        }
        return {
            auth_state: "guest",
            user_summary: {
                display_name: "Khách",
                username: "",
                role_label: "Khách hàng",
                avatar_url: "",
                avatar_fallback: "KH"
            }
        };
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

    function getCurrentAppHref() {
        return sanitizeAppHref((window.location.pathname || "/") + (window.location.search || ""));
    }

    function buildLoginHref() {
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + encodeURIComponent(getCurrentAppHref());
    }

    function showNotice(node, message, tone) {
        if (!node) return;
        node.textContent = message || "";
        node.classList.remove("is-error");
        if (tone === "error") node.classList.add("is-error");
        node.hidden = false;
        clearTimeout(showNotice._timer);
        showNotice._timer = setTimeout(function () {
            node.hidden = true;
        }, 2600);
    }

    function isValidEmail(value) {
        if (!value) return true;
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
    }

    function isValidLink(value) {
        var raw = (value || "").trim();
        if (!raw) return false;
        if (/^javascript:/i.test(raw)) return false;
        var normalized = /^https?:\/\//i.test(raw) ? raw : ("https://" + raw.replace(/^\/+/, ""));
        try {
            var url = new URL(normalized);
            return url.protocol === "http:" || url.protocol === "https:";
        } catch (e) {
            return false;
        }
    }

    function normalizeLinkForSubmit(value) {
        var raw = (value || "").trim();
        if (!raw) return "";
        return /^https?:\/\//i.test(raw) ? raw : ("https://" + raw.replace(/^\/+/, ""));
    }

    function loadRuntimeProfile() {
        return fetch("/app-ui/shared/runtime-profile.ashx?ui_mode=app", {
            method: "GET",
            credentials: "same-origin",
            cache: "no-store"
        })
            .then(function (response) {
                if (!response.ok) throw new Error("runtime_profile_http");
                return response.json();
            })
            .catch(function () {
                return null;
            });
    }

    function postProfileUpdate(payload) {
        var body = new URLSearchParams();
        Object.keys(payload || {}).forEach(function (key) {
            body.set(key, payload[key] || "");
        });
        return fetch("/app-ui/shared/runtime-profile-update.ashx?ui_mode=app", {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
            },
            body: body.toString()
        }).then(function (response) {
            return response.text().then(function (raw) {
                var data = null;
                try { data = raw ? JSON.parse(raw) : null; } catch (e) { data = null; }
                return { ok: response.ok, data: data || {} };
            });
        });
    }

    function postLinkAction(action, payload) {
        var body = new URLSearchParams();
        body.set("action", action || "");
        Object.keys(payload || {}).forEach(function (key) {
            body.set(key, payload[key] || "");
        });
        return fetch("/app-ui/shared/runtime-profile-links.ashx?ui_mode=app", {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
            },
            body: body.toString()
        }).then(function (response) {
            return response.text().then(function (raw) {
                var data = null;
                try { data = raw ? JSON.parse(raw) : null; } catch (e) { data = null; }
                return { ok: response.ok, data: data || {} };
            });
        });
    }

    function setButtonState(button, active, loadingText, defaultText) {
        if (!button) return;
        if (active) {
            button.dataset.prevText = button.textContent || defaultText || "";
            button.textContent = loadingText || "Đang xử lý...";
            button.disabled = true;
            return;
        }
        button.textContent = button.dataset.prevText || defaultText || "";
        button.disabled = false;
    }

    function setReadonly(inputs, value) {
        Object.keys(inputs || {}).forEach(function (key) {
            if (inputs[key]) inputs[key].disabled = !!value;
        });
    }

    function renderUserCard(node, profile, session) {
        if (!node) return;
        var user = session && session.user_summary ? session.user_summary : {};
        var current = profile || {};
        var displayName = (current.display_name || user.display_name || "Tài khoản Home").trim();
        var role = (current.role_label || user.role_label || "Khách hàng").trim();
        var username = (current.username || user.username || "").trim();
        var avatarHtml = current.avatar_url
            ? '<img class="app-account-avatar-image" src="' + escapeHtml(current.avatar_url) + '" alt="' + escapeHtml(displayName) + '" />'
            : escapeHtml(current.avatar_fallback || user.avatar_fallback || "AH");

        node.innerHTML = '' +
            '<span class="app-account-avatar">' + avatarHtml + "</span>" +
            '<div class="app-home-settings-user-meta">' +
            '  <strong>' + escapeHtml(displayName) + "</strong>" +
            '  <span>' + escapeHtml(username || role) + "</span>" +
            "</div>";
    }

    function renderLinkList(node, links) {
        if (!node) return;
        var safeLinks = Array.isArray(links) ? links : [];
        if (!safeLinks.length) {
            node.innerHTML = '<div class="app-home-settings-link-empty">Chưa có Bio Link. Thêm link đầu tiên của bạn ở trên.</div>';
            return;
        }
        node.innerHTML = safeLinks.map(function (item) {
            var icon = item && item.icon
                ? '<img src="' + escapeHtml(item.icon) + '" alt="' + escapeHtml(item.title || "link") + '" />'
                : "<span>🌐</span>";
            return '' +
                '<article class="app-home-settings-link-item">' +
                '  <span class="app-home-settings-link-icon">' + icon + "</span>" +
                '  <a class="app-home-settings-link-copy" href="' + escapeHtml(item.href || "#") + '" target="_blank" rel="noopener noreferrer">' +
                '    <strong>' + escapeHtml(item.title || "Liên kết") + "</strong>" +
                '    <span>' + escapeHtml(item.host || "") + "</span>" +
                "  </a>" +
                '  <button type="button" class="app-home-settings-link-delete" data-home-settings-action="delete-link" data-link-id="' + escapeHtml(item.id) + '">Xóa</button>' +
                "</article>";
        }).join("");
    }

    function renderGuestMode(state) {
        renderUserCard(state.userNode, null, state.session);
        setReadonly(state.inputs, true);
        if (state.saveButton) {
            state.saveButton.textContent = "Đăng nhập để chỉnh sửa";
            state.saveButton.disabled = false;
            if (state.saveButton.dataset.boundGuest !== "1") {
                state.saveButton.dataset.boundGuest = "1";
                state.saveButton.addEventListener("click", function () {
                    window.location.href = buildLoginHref();
                });
            }
        }
        if (state.addLinkButton) {
            state.addLinkButton.textContent = "Đăng nhập để thêm link";
            state.addLinkButton.disabled = false;
            if (state.addLinkButton.dataset.boundGuest !== "1") {
                state.addLinkButton.dataset.boundGuest = "1";
                state.addLinkButton.addEventListener("click", function () {
                    window.location.href = buildLoginHref();
                });
            }
        }
        if (state.linkInputs.title) state.linkInputs.title.disabled = true;
        if (state.linkInputs.url) state.linkInputs.url.disabled = true;
        renderLinkList(state.linkListNode, []);
        showNotice(state.noticeNode, "Bạn cần đăng nhập để chỉnh sửa hồ sơ và Bio Link.", "error");
    }

    function bindDeleteLink(state) {
        if (!state.linkListNode || state.linkListNode.dataset.boundDelete === "1") return;
        state.linkListNode.dataset.boundDelete = "1";
        state.linkListNode.addEventListener("click", function (event) {
            var target = event.target;
            if (!target || target.getAttribute("data-home-settings-action") !== "delete-link") return;
            event.preventDefault();
            var id = (target.getAttribute("data-link-id") || "").trim();
            if (!id) return;

            setButtonState(target, true, "Đang xóa...", "Xóa");
            postLinkAction("delete", { id: id })
                .then(function (result) {
                    if (!result.ok || !result.data || result.data.ok !== true) {
                        showNotice(state.noticeNode, (result.data && result.data.message) || "Không thể xóa link.", "error");
                        return;
                    }
                    state.links = Array.isArray(result.data.links) ? result.data.links : [];
                    renderLinkList(state.linkListNode, state.links);
                    showNotice(state.noticeNode, result.data.message || "Đã xóa link.");
                })
                .catch(function () {
                    showNotice(state.noticeNode, "Lỗi kết nối, vui lòng thử lại.", "error");
                })
                .finally(function () {
                    setButtonState(target, false, "Đang xóa...", "Xóa");
                });
        });
    }

    function bindAddLink(state) {
        if (!state.addLinkButton || state.addLinkButton.dataset.boundAdd === "1") return;
        state.addLinkButton.dataset.boundAdd = "1";
        state.addLinkButton.addEventListener("click", function () {
            var title = (state.linkInputs.title && state.linkInputs.title.value || "").trim();
            var urlRaw = (state.linkInputs.url && state.linkInputs.url.value || "").trim();
            if (!urlRaw) {
                showNotice(state.noticeNode, "Vui lòng nhập URL Bio Link.", "error");
                if (state.linkInputs.url) state.linkInputs.url.focus();
                return;
            }
            if (!isValidLink(urlRaw)) {
                showNotice(state.noticeNode, "URL chưa hợp lệ. Vui lòng nhập dạng https://...", "error");
                if (state.linkInputs.url) state.linkInputs.url.focus();
                return;
            }

            setButtonState(state.addLinkButton, true, "Đang thêm...", "Thêm link");
            postLinkAction("add", { title: title, url: normalizeLinkForSubmit(urlRaw) })
                .then(function (result) {
                    if (!result.ok || !result.data || result.data.ok !== true) {
                        showNotice(state.noticeNode, (result.data && result.data.message) || "Không thể thêm link.", "error");
                        return;
                    }
                    state.links = Array.isArray(result.data.links) ? result.data.links : [];
                    renderLinkList(state.linkListNode, state.links);
                    if (state.linkInputs.title) state.linkInputs.title.value = "";
                    if (state.linkInputs.url) state.linkInputs.url.value = "";
                    showNotice(state.noticeNode, result.data.message || "Đã thêm Bio Link.");
                })
                .catch(function () {
                    showNotice(state.noticeNode, "Lỗi kết nối, vui lòng thử lại.", "error");
                })
                .finally(function () {
                    setButtonState(state.addLinkButton, false, "Đang thêm...", "Thêm link");
                });
        });
    }

    function bindSaveProfile(state) {
        if (!state.saveButton || state.saveButton.dataset.boundSave === "1") return;
        state.saveButton.dataset.boundSave = "1";
        state.saveButton.addEventListener("click", function () {
            var displayName = (state.inputs.display_name.value || "").trim();
            var phone = (state.inputs.phone.value || "").trim();
            var email = (state.inputs.email.value || "").trim();
            var address = (state.inputs.address.value || "").trim();
            var intro = (state.inputs.intro.value || "").trim();

            if (!displayName) {
                showNotice(state.noticeNode, "Vui lòng nhập họ và tên.", "error");
                state.inputs.display_name.focus();
                return;
            }
            if (!isValidEmail(email)) {
                showNotice(state.noticeNode, "Email chưa đúng định dạng.", "error");
                state.inputs.email.focus();
                return;
            }

            setButtonState(state.saveButton, true, "Đang lưu...", "Lưu thay đổi");
            postProfileUpdate({
                display_name: displayName,
                phone: phone,
                email: email,
                address: address,
                intro: intro
            })
                .then(function (result) {
                    if (!result.ok || !result.data || result.data.ok !== true) {
                        showNotice(state.noticeNode, (result.data && result.data.message) || "Lỗi cập nhật tài khoản.", "error");
                        return;
                    }
                    renderUserCard(state.userNode, {
                        display_name: displayName,
                        username: (state.session.user_summary && state.session.user_summary.username) || "",
                        role_label: (state.session.user_summary && state.session.user_summary.role_label) || "Khách hàng",
                        avatar_url: (state.session.user_summary && state.session.user_summary.avatar_url) || "",
                        avatar_fallback: (state.session.user_summary && state.session.user_summary.avatar_fallback) || "AH"
                    }, state.session);
                    showNotice(state.noticeNode, result.data.message || "Đã lưu thông tin tài khoản.");
                })
                .catch(function () {
                    showNotice(state.noticeNode, "Lỗi kết nối, vui lòng thử lại.", "error");
                })
                .finally(function () {
                    setButtonState(state.saveButton, false, "Đang lưu...", "Lưu thay đổi");
                });
        });
    }

    function render() {
        var page = document.querySelector("[data-home-settings-page='1']");
        if (!page) return;

        var state = {
            page: page,
            noticeNode: page.querySelector("[data-home-settings-notice='1']"),
            userNode: page.querySelector("[data-home-settings-user='1']"),
            saveButton: page.querySelector("[data-home-settings-action='save']"),
            addLinkButton: page.querySelector("[data-home-settings-action='add-link']"),
            linkListNode: page.querySelector("[data-home-settings-link-list='1']"),
            inputs: {
                display_name: page.querySelector("[data-home-settings-input='display_name']"),
                phone: page.querySelector("[data-home-settings-input='phone']"),
                email: page.querySelector("[data-home-settings-input='email']"),
                address: page.querySelector("[data-home-settings-input='address']"),
                intro: page.querySelector("[data-home-settings-input='intro']")
            },
            linkInputs: {
                title: page.querySelector("[data-home-settings-link-input='title']"),
                url: page.querySelector("[data-home-settings-link-input='url']")
            },
            session: getSession(),
            links: []
        };

        if (!state.noticeNode || !state.userNode || !state.saveButton || !state.addLinkButton || !state.linkListNode) return;
        if (!state.inputs.display_name || !state.inputs.phone || !state.inputs.email || !state.inputs.address || !state.inputs.intro) return;
        if (!state.linkInputs.title || !state.linkInputs.url) return;

        bindDeleteLink(state);

        if (!state.session || state.session.auth_state !== "authenticated") {
            renderGuestMode(state);
            return;
        }

        setReadonly(state.inputs, false);
        state.linkInputs.title.disabled = false;
        state.linkInputs.url.disabled = false;

        loadRuntimeProfile().then(function (payload) {
            var profile = payload && payload.ok && payload.profile ? payload.profile : {};
            state.links = payload && payload.ok && Array.isArray(payload.social_links) ? payload.social_links : [];

            renderUserCard(state.userNode, profile, state.session);
            renderLinkList(state.linkListNode, state.links);

            state.inputs.display_name.value = profile.display_name || "";
            state.inputs.phone.value = profile.phone || "";
            state.inputs.email.value = profile.email || "";
            state.inputs.address.value = profile.address || "";
            state.inputs.intro.value = profile.intro && profile.intro !== "Chưa cập nhật giới thiệu." ? profile.intro : "";
        });

        bindSaveProfile(state);
        bindAddLink(state);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", render);
    } else {
        render();
    }
})();
