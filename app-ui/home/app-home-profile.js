(function () {
    function escapeHtml(value) {
        return String(value || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function formatCount(value) {
        var num = Number(value || 0);
        if (!isFinite(num)) num = 0;
        return num.toLocaleString("vi-VN");
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
                phone: "",
                email: "",
                role_label: "Khách hàng",
                avatar_url: "",
                avatar_fallback: "KH"
            }
        };
    }

    function hasCapability(name) {
        if (!name) return true;
        if (window.AhaAppUi && typeof window.AhaAppUi.hasCapability === "function") {
            return window.AhaAppUi.hasCapability(name);
        }
        return true;
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
        if (!safeUrl) return safeUrl;
        if (safeUrl.indexOf("javascript:") === 0) return safeUrl;
        if (safeUrl.indexOf("/app-ui/") === 0) {
            if (safeUrl.indexOf("/app-ui/auth/") === 0) {
                return appendQuery(safeUrl, "return_url", getCurrentAppHref());
            }
            return appendQuery(safeUrl, "back_href", getCurrentAppHref());
        }
        if (safeUrl.indexOf("/home/") === 0
            || safeUrl.indexOf("/gianhang/") === 0
            || safeUrl.indexOf("/bat-dong-san/") === 0
            || safeUrl.indexOf("/xe/") === 0
            || safeUrl.indexOf("/shop/") === 0) {
            return appendQuery(safeUrl, "return_url", getCurrentAppHref());
        }
        return safeUrl;
    }

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

    function resolveReferralLink(model, session) {
        var profile = model && model.profile ? model.profile : {};
        if (profile.account_key) {
            return "https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + encodeURIComponent(profile.account_key);
        }
        var user = session && session.user_summary ? session.user_summary : {};
        var account = (user.phone || "").trim();
        if (!account) {
            account = (user.username || "").replace(/^home\s+/i, "").trim();
        }
        if (!account) return "";
        return "https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + encodeURIComponent(account);
    }

    function showNotice(node, message, tone) {
        if (!node) return;
        node.textContent = message || "";
        node.classList.remove("is-error");
        if (tone === "error") {
            node.classList.add("is-error");
        }
        node.hidden = false;
        clearTimeout(showNotice._timer);
        showNotice._timer = setTimeout(function () {
            node.hidden = true;
        }, 2400);
    }

    function resolveModel(session, payload) {
        var user = session && session.user_summary ? session.user_summary : {};
        var emptyProfile = {
            account_key: "",
            display_name: user.display_name || "Tài khoản Home",
            username: user.username || "",
            role_label: user.role_label || "Khách hàng",
            intro: "Chưa cập nhật giới thiệu.",
            avatar_url: user.avatar_url || "",
            avatar_fallback: user.avatar_fallback || "AH",
            phone: user.phone || "",
            phone_href: user.phone ? ("tel:" + user.phone) : "",
            email: user.email || "",
            address: "",
            public_profile_url: "",
            save_contact_url: "",
            edit_url: "/app-ui/home/settings.aspx?ui_mode=app"
        };

        if (!payload || payload.ok !== true || !payload.profile) {
            return {
                ok: false,
                profile: emptyProfile,
                settings: { show_contact: true, show_social: true },
                stats: { review_count: 0, post_count: 0, social_count: 0 },
                social_links: []
            };
        }

        return {
            ok: true,
            profile: payload.profile || emptyProfile,
            settings: payload.settings || { show_contact: true, show_social: true },
            stats: payload.stats || { review_count: 0, post_count: 0, social_count: 0 },
            social_links: Array.isArray(payload.social_links) ? payload.social_links : []
        };
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

    function renderProfileHead(host, model, session) {
        if (!host) return;
        var profile = model && model.profile ? model.profile : {};
        var stats = model && model.stats ? model.stats : {};
        var avatarHtml = profile.avatar_url
            ? '<img class="app-account-avatar-image" src="' + escapeHtml(profile.avatar_url) + '" alt="' + escapeHtml(profile.display_name || "Tài khoản") + '" />'
            : escapeHtml(profile.avatar_fallback || "AH");
        var intro = (profile.intro || "").trim() || "Chưa cập nhật giới thiệu.";

        var actions = [];
        if (profile.phone_href) {
            actions.push('<a class="app-home-profile-action is-primary" href="' + escapeHtml(profile.phone_href) + '">Gọi nhanh</a>');
        }
        actions.push('<a class="app-home-profile-action" href="' + escapeHtml(decorateAppHref(profile.edit_url || "/app-ui/home/settings.aspx?ui_mode=app")) + '">Sửa hồ sơ</a>');
        actions.push('<button class="app-home-profile-action" type="button" data-app-account-action="copy_profile_link">Sao chép Bio Link</button>');
        if (profile.save_contact_url) {
            actions.push('<a class="app-home-profile-action" href="' + escapeHtml(profile.save_contact_url) + '">Lưu danh bạ</a>');
        }
        if (profile.public_profile_url) {
            actions.push('<a class="app-home-profile-action" href="' + escapeHtml(profile.public_profile_url) + '" target="_blank" rel="noopener noreferrer">Mở hồ sơ công khai</a>');
        }

        host.innerHTML = '' +
            '<div class="app-home-profile-hero">' +
            '  <div class="app-home-profile-hero-head">' +
            '    <span class="app-account-avatar app-home-profile-avatar">' + avatarHtml + '</span>' +
            '    <div class="app-home-profile-meta">' +
            '      <strong class="app-home-profile-name">' + escapeHtml(profile.display_name || "Tài khoản Home") + '</strong>' +
            '      <span class="app-home-profile-sub">' + escapeHtml(profile.username || "Home user") + '</span>' +
            '      <span class="app-account-role">' + escapeHtml(profile.role_label || "Khách hàng") + '</span>' +
            '    </div>' +
            '  </div>' +
            '  <p class="app-home-profile-intro">' + escapeHtml(intro) + '</p>' +
            '  <div class="app-home-profile-stats">' +
            '    <div class="app-home-profile-stat"><strong>' + formatCount(stats.review_count) + '</strong><span>Đánh giá</span></div>' +
            '    <div class="app-home-profile-stat"><strong>' + formatCount(stats.post_count) + '</strong><span>Tin đăng</span></div>' +
            '    <div class="app-home-profile-stat"><strong>' + formatCount(stats.social_count) + '</strong><span>Bio Link</span></div>' +
            '  </div>' +
            '  <div class="app-home-profile-actions">' + actions.join("") + "</div>" +
            "</div>";

        host.setAttribute("data-profile-public-link", profile.public_profile_url || "");
        host.setAttribute("data-profile-ref-link", resolveReferralLink(model, session) || "");
    }

    function renderInfoCard(host, model) {
        if (!host) return;
        var profile = model && model.profile ? model.profile : {};
        var settings = model && model.settings ? model.settings : {};
        var showContact = settings.show_contact !== false;

        var contactRows = [];
        contactRows.push(renderInfoItem("Vai trò", profile.role_label || "Khách hàng"));
        contactRows.push(renderInfoItem("Tên tài khoản", profile.username || "Chưa cập nhật"));
        if (showContact) {
            contactRows.push(renderInfoItem("Điện thoại", profile.phone || "Chưa cập nhật"));
            contactRows.push(renderInfoItem("Email", profile.email || "Chưa cập nhật"));
            contactRows.push(renderInfoItem("Địa chỉ", profile.address || "Chưa cập nhật", true));
        }

        host.innerHTML = '' +
            "<h2>Thông tin cá nhân</h2>" +
            '<p>Dữ liệu hồ sơ đồng bộ từ Home desktop và hiển thị tối ưu cho app.</p>' +
            '<div class="app-home-profile-info-grid">' + contactRows.join("") + "</div>";
    }

    function renderInfoItem(label, value, full) {
        return '' +
            '<article class="app-home-profile-info-item' + (full ? " is-full" : "") + '">' +
            '  <span class="app-home-profile-info-label">' + escapeHtml(label || "") + "</span>" +
            '  <strong class="app-home-profile-info-value">' + escapeHtml(value || "") + "</strong>" +
            "</article>";
    }

    function renderLinksCard(host, model) {
        if (!host) return;
        var settings = model && model.settings ? model.settings : {};
        var links = model && Array.isArray(model.social_links) ? model.social_links : [];
        var content = "";

        if (settings.show_social === false) {
            content = '<div class="app-home-profile-empty">Chủ tài khoản đang ẩn liên kết cá nhân.</div>';
        } else if (!links.length) {
            content = '<div class="app-home-profile-empty">Chưa có bio link. Bạn có thể thêm trong phần cài đặt hồ sơ.</div>';
        } else {
            content = '<div class="app-home-profile-bio-list">' + links.map(function (item) {
                var icon = item.icon
                    ? '<img src="' + escapeHtml(item.icon) + '" alt="' + escapeHtml(item.title || "link") + '" />'
                    : '<span class="app-home-profile-bio-fallback">🌐</span>';
                return '' +
                    '<a class="app-home-profile-bio-item" href="' + escapeHtml(item.href || "#") + '" target="_blank" rel="noopener noreferrer">' +
                    '  <span class="app-home-profile-bio-icon">' + icon + "</span>" +
                    '  <span class="app-home-profile-bio-copy">' +
                    '    <strong>' + escapeHtml(item.title || "Liên kết") + "</strong>" +
                    '    <span>' + escapeHtml(item.host || "Mở liên kết") + "</span>" +
                    "  </span>" +
                    '  <span class="app-home-profile-bio-arrow">&rsaquo;</span>' +
                    "</a>";
            }).join("") + "</div>";
        }

        host.innerHTML = '' +
            "<h2>Bio Link</h2>" +
            "<p>Các liên kết cá nhân hiển thị giống phần hồ sơ desktop.</p>" +
            content;
    }

    function bindActions(root, session, noticeNode) {
        if (!root || root.getAttribute("data-profile-action-bound") === "1") return;
        root.setAttribute("data-profile-action-bound", "1");

        root.addEventListener("click", function (event) {
            var node = event.target;
            while (node && node !== root) {
                if (node.getAttribute && node.getAttribute("data-app-account-action")) {
                    break;
                }
                node = node.parentNode;
            }
            if (!node || node === root) return;

            var action = node.getAttribute("data-app-account-action");
            if (!action) return;

            event.preventDefault();

            var model = root._profileModel || {};
            if (action === "copy_referral_link") {
                var referral = resolveReferralLink(model, session);
                if (!referral) {
                    showNotice(noticeNode, "Không có link giới thiệu để sao chép.", "error");
                    return;
                }
                copyText(referral).then(function () {
                    showNotice(noticeNode, "Đã sao chép link giới thiệu.");
                }).catch(function () {
                    showNotice(noticeNode, "Không thể sao chép link, vui lòng thử lại.", "error");
                });
                return;
            }

            if (action === "copy_profile_link") {
                var publicLink = model.profile && model.profile.public_profile_url
                    ? String(model.profile.public_profile_url)
                    : "";
                if (!publicLink) {
                    showNotice(noticeNode, "Chưa có link hồ sơ công khai để sao chép.", "error");
                    return;
                }
                copyText(publicLink).then(function () {
                    showNotice(noticeNode, "Đã sao chép bio link.");
                }).catch(function () {
                    showNotice(noticeNode, "Không thể sao chép bio link, vui lòng thử lại.", "error");
                });
            }
        });
    }

    function render() {
        var page = document.querySelector("[data-home-profile-page='1']");
        if (!page) return;

        var headNode = page.querySelector("[data-home-profile-head='1']");
        var infoNode = page.querySelector("[data-home-profile-info='1']");
        var linksNode = page.querySelector("[data-home-profile-links='1']");
        var noticeNode = page.querySelector("[data-home-profile-notice='1']");
        if (!headNode || !infoNode || !linksNode) return;

        var session = getSession();
        bindActions(page, session, noticeNode);

        var fallbackModel = resolveModel(session, null);
        page._profileModel = fallbackModel;
        renderProfileHead(headNode, fallbackModel, session);
        renderInfoCard(infoNode, fallbackModel);
        renderLinksCard(linksNode, fallbackModel);

        loadRuntimeProfile().then(function (payload) {
            var runtimeModel = resolveModel(session, payload);
            page._profileModel = runtimeModel;
            renderProfileHead(headNode, runtimeModel, session);
            renderInfoCard(infoNode, runtimeModel);
            renderLinksCard(linksNode, runtimeModel);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", render);
    } else {
        render();
    }
})();
