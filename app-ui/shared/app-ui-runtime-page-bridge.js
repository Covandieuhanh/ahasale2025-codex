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
        return adapter && typeof adapter.getSession === "function"
            ? adapter.getSession()
            : { auth_state: "guest", seller_status: "not_registered", user_summary: null };
    }

    function getSpaceCode() {
        var shell = document.querySelector(".app-shell[data-app-shell]");
        return shell ? (shell.getAttribute("data-app-shell") || "home") : "home";
    }

    function getRuntimePrimaryButton() {
        return document.querySelector(".app-page-btn.is-primary");
    }

    function getPathname() {
        return (window.location.pathname || "").toLowerCase();
    }

    function getCurrentAppHref() {
        return sanitizeAppHref((window.location.pathname || "/") + (window.location.search || ""));
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

    function decorateRuntimeHref(href) {
        if (!href) return href;
        var safeHref = sanitizeAppHref(href);

        var currentAppHref = getCurrentAppHref();
        if (safeHref.indexOf("/app-ui/auth/") === 0) {
            return appendQuery(safeHref, "return_url", currentAppHref);
        }

        if (safeHref.indexOf("/gianhang/") === 0
            || safeHref.indexOf("/bat-dong-san/") === 0
            || safeHref.indexOf("/home/") === 0
            || safeHref.indexOf("/xe/") === 0
            || safeHref.indexOf("/shop/") === 0) {
            return appendQuery(safeHref, "return_url", currentAppHref);
        }

        return safeHref;
    }

    function decorateAppHref(href) {
        if (!href) return href;
        var safeHref = sanitizeAppHref(href);
        if (safeHref.indexOf("/app-ui/") !== 0) return safeHref;
        return appendQuery(safeHref, "back_href", getCurrentAppHref());
    }

    function getContextualSearchHref(spaceCode) {
        var pathname = (window.location.pathname || "").toLowerCase();
        if (spaceCode === "home") {
            if (pathname.indexOf("/home/orders.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Đơn mua");
            if (pathname.indexOf("/home/appointments.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Lịch hẹn");
            if (pathname.indexOf("/home/notifications.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Thông báo");
            if (pathname.indexOf("/home/profile.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Tài khoản");
            if (pathname.indexOf("/home/settings.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Cài đặt");
            if (pathname.indexOf("/home/exchange-history.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Lịch sử trao đổi");
            if (pathname.indexOf("/home/view-history.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Lịch sử xem");
            if (pathname.indexOf("/home/my-reviews.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Đánh giá");
            if (pathname.indexOf("/home/feedback.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Góp ý");
            if (pathname.indexOf("/home/proposals.aspx") >= 0) return "/app-ui/home/search.aspx?ui_mode=app&q=" + encodeURIComponent("Đề xuất");
            return "/app-ui/home/search.aspx?ui_mode=app";
        }
        if (spaceCode === "batdongsan") {
            if (pathname.indexOf("/batdongsan/mua-ban.aspx") >= 0) return "/app-ui/batdongsan/search.aspx?ui_mode=app&transaction_type=mua_ban";
            if (pathname.indexOf("/batdongsan/cho-thue.aspx") >= 0) return "/app-ui/batdongsan/search.aspx?ui_mode=app&transaction_type=cho_thue";
            if (pathname.indexOf("/batdongsan/du-an.aspx") >= 0) return "/app-ui/batdongsan/search.aspx?ui_mode=app&q=" + encodeURIComponent("Dự án bất động sản");
            if (pathname.indexOf("/batdongsan/tham-khao-gia.aspx") >= 0) return "/app-ui/batdongsan/search.aspx?ui_mode=app&q=" + encodeURIComponent("Tham khảo giá bất động sản");
            if (pathname.indexOf("/batdongsan/vay-mua-nha.aspx") >= 0) return "/app-ui/batdongsan/search.aspx?ui_mode=app&q=" + encodeURIComponent("Vay mua nhà");
            if (pathname.indexOf("/batdongsan/kinh-nghiem.aspx") >= 0) return "/app-ui/batdongsan/search.aspx?ui_mode=app&q=" + encodeURIComponent("Kinh nghiệm bất động sản");
            return "/app-ui/batdongsan/search.aspx?ui_mode=app";
        }
        if (spaceCode === "choxe") {
            if (pathname.indexOf("/choxe/o-to.aspx") >= 0) return "/app-ui/choxe/search.aspx?ui_mode=app&q=" + encodeURIComponent("Ô tô");
            if (pathname.indexOf("/choxe/xe-may.aspx") >= 0) return "/app-ui/choxe/search.aspx?ui_mode=app&q=" + encodeURIComponent("Xe máy");
            if (pathname.indexOf("/choxe/phu-tung.aspx") >= 0) return "/app-ui/choxe/search.aspx?ui_mode=app&q=" + encodeURIComponent("Phụ tùng xe");
            if (pathname.indexOf("/choxe/dich-vu.aspx") >= 0) return "/app-ui/choxe/search.aspx?ui_mode=app&q=" + encodeURIComponent("Dịch vụ xe");
            return "/app-ui/choxe/search.aspx?ui_mode=app";
        }
        if (spaceCode === "dienthoai-maytinh") {
            if (pathname.indexOf("/dienthoai-maytinh/dien-thoai.aspx") >= 0) return "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app&q=" + encodeURIComponent("Điện thoại");
            if (pathname.indexOf("/dienthoai-maytinh/may-tinh.aspx") >= 0) return "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app&q=" + encodeURIComponent("Máy tính");
            if (pathname.indexOf("/dienthoai-maytinh/phu-kien.aspx") >= 0) return "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app&q=" + encodeURIComponent("Phụ kiện công nghệ");
            return "/app-ui/dienthoai-maytinh/search.aspx?ui_mode=app";
        }
        if (spaceCode === "gianhang") {
            return "/app-ui/gianhang/listing.aspx?ui_mode=app";
        }
        return "/app-ui/home/search.aspx?ui_mode=app";
    }

    function getRuntimePreviewContext(spaceCode, pathname) {
        if (spaceCode === "home") {
            if (pathname.indexOf("/app-ui/home/notifications.aspx") >= 0) {
                return null;
            }
            if (pathname.indexOf("/app-ui/home/categories.aspx") >= 0) {
                return {
                    title: "Tin mới trong danh mục đang bật",
                    q: "",
                    filters: {},
                    emptyText: "Chưa có tin mới trong danh mục Home."
                };
            }
            if (pathname.indexOf("/app-ui/home/orders.aspx") >= 0) {
                return {
                    title: "Tin liên quan đơn mua",
                    q: "don mua",
                    filters: {},
                    emptyText: "Chưa có tin liên quan đơn mua."
                };
            }
            if (pathname.indexOf("/app-ui/home/appointments.aspx") >= 0) {
                return {
                    title: "Tin liên quan lịch hẹn",
                    q: "lich hen",
                    filters: {},
                    emptyText: "Chưa có tin liên quan lịch hẹn."
                };
            }
            if (pathname.indexOf("/app-ui/home/exchange-history.aspx") >= 0) {
                return {
                    title: "Tin liên quan lịch sử giao dịch",
                    q: "lich su giao dich",
                    filters: {},
                    emptyText: "Chưa có tin liên quan lịch sử giao dịch."
                };
            }
            if (pathname.indexOf("/app-ui/home/view-history.aspx") >= 0) {
                return {
                    title: "Tin bạn có thể xem tiếp",
                    q: "",
                    filters: {},
                    emptyText: "Chưa có tin mới để gợi ý."
                };
            }
            if (pathname.indexOf("/app-ui/home/my-reviews.aspx") >= 0) {
                return {
                    title: "Tin liên quan đánh giá",
                    q: "danh gia",
                    filters: {},
                    emptyText: "Chưa có tin liên quan đánh giá."
                };
            }
            if (pathname.indexOf("/app-ui/home/proposals.aspx") >= 0) {
                return {
                    title: "Tin liên quan đề xuất",
                    q: "de xuat",
                    filters: {},
                    emptyText: "Chưa có tin liên quan đề xuất."
                };
            }
            if (pathname.indexOf("/app-ui/home/feedback.aspx") >= 0) {
                return {
                    title: "Tin liên quan góp ý",
                    q: "dong gop y kien",
                    filters: {},
                    emptyText: "Chưa có tin liên quan góp ý."
                };
            }
            if (pathname.indexOf("/app-ui/home/benefit-consumer.aspx") >= 0) {
                return {
                    title: "Tin liên quan quyền tiêu dùng",
                    q: "quyen tieu dung",
                    filters: {},
                    emptyText: "Chưa có tin liên quan quyền tiêu dùng."
                };
            }
            if (pathname.indexOf("/app-ui/home/benefit-offers.aspx") >= 0) {
                return {
                    title: "Tin liên quan quyền ưu đãi",
                    q: "quyen uu dai",
                    filters: {},
                    emptyText: "Chưa có tin liên quan quyền ưu đãi."
                };
            }
            if (pathname.indexOf("/app-ui/home/benefit-labor.aspx") >= 0) {
                return {
                    title: "Tin liên quan hành vi lao động",
                    q: "hanh vi lao dong",
                    filters: {},
                    emptyText: "Chưa có tin liên quan hành vi lao động."
                };
            }
            if (pathname.indexOf("/app-ui/home/benefit-engagement.aspx") >= 0) {
                return {
                    title: "Tin liên quan chỉ số gắn kết",
                    q: "chi so gan ket",
                    filters: {},
                    emptyText: "Chưa có tin liên quan chỉ số gắn kết."
                };
            }
            if (pathname.indexOf("/app-ui/home/profile.aspx") >= 0
                || pathname.indexOf("/app-ui/home/settings.aspx") >= 0
                || pathname.indexOf("/app-ui/home/change-password.aspx") >= 0
                || pathname.indexOf("/app-ui/home/change-pin.aspx") >= 0) {
                return {
                    title: "Tin gợi ý cho tài khoản của bạn",
                    q: "",
                    filters: {},
                    emptyText: "Chưa có tin gợi ý mới."
                };
            }
            return null;
        }

        if (spaceCode === "batdongsan") {
            if (pathname.indexOf("/app-ui/batdongsan/mua-ban.aspx") >= 0) {
                return { title: "Tin mua bán mới", q: "", filters: { transaction_type: "mua_ban" }, emptyText: "Chưa có tin mua bán phù hợp." };
            }
            if (pathname.indexOf("/app-ui/batdongsan/cho-thue.aspx") >= 0) {
                return { title: "Tin cho thuê mới", q: "", filters: { transaction_type: "cho_thue" }, emptyText: "Chưa có tin cho thuê phù hợp." };
            }
            if (pathname.indexOf("/app-ui/batdongsan/du-an.aspx") >= 0) {
                return { title: "Dự án nổi bật", q: "du an bat dong san", filters: {}, emptyText: "Chưa có dự án phù hợp." };
            }
            if (pathname.indexOf("/app-ui/batdongsan/tham-khao-gia.aspx") >= 0) {
                return { title: "Tin tham khảo giá", q: "tham khao gia bat dong san", filters: {}, emptyText: "Chưa có dữ liệu tham khảo giá phù hợp." };
            }
            if (pathname.indexOf("/app-ui/batdongsan/vay-mua-nha.aspx") >= 0) {
                return { title: "Tin liên quan vay mua nhà", q: "vay mua nha", filters: {}, emptyText: "Chưa có tin liên quan vay mua nhà." };
            }
            if (pathname.indexOf("/app-ui/batdongsan/kinh-nghiem.aspx") >= 0) {
                return { title: "Tin kinh nghiệm mới", q: "kinh nghiem bat dong san", filters: {}, emptyText: "Chưa có tin kinh nghiệm phù hợp." };
            }
            return null;
        }

        if (spaceCode === "choxe") {
            if (pathname.indexOf("/app-ui/choxe/o-to.aspx") >= 0) {
                return { title: "Tin ô tô mới", q: "o to", filters: {}, emptyText: "Chưa có tin ô tô phù hợp." };
            }
            if (pathname.indexOf("/app-ui/choxe/xe-may.aspx") >= 0) {
                return { title: "Tin xe máy mới", q: "xe may", filters: {}, emptyText: "Chưa có tin xe máy phù hợp." };
            }
            if (pathname.indexOf("/app-ui/choxe/phu-tung.aspx") >= 0) {
                return { title: "Tin phụ tùng mới", q: "phu tung xe", filters: {}, emptyText: "Chưa có tin phụ tùng phù hợp." };
            }
            if (pathname.indexOf("/app-ui/choxe/dich-vu.aspx") >= 0) {
                return { title: "Tin dịch vụ xe", q: "dich vu xe", filters: {}, emptyText: "Chưa có tin dịch vụ xe phù hợp." };
            }
            return null;
        }

        if (spaceCode === "dienthoai-maytinh") {
            if (pathname.indexOf("/app-ui/dienthoai-maytinh/dien-thoai.aspx") >= 0) {
                return { title: "Tin điện thoại mới", q: "dien thoai", filters: {}, emptyText: "Chưa có tin điện thoại phù hợp." };
            }
            if (pathname.indexOf("/app-ui/dienthoai-maytinh/may-tinh.aspx") >= 0) {
                return { title: "Tin máy tính mới", q: "may tinh", filters: {}, emptyText: "Chưa có tin máy tính phù hợp." };
            }
            if (pathname.indexOf("/app-ui/dienthoai-maytinh/phu-kien.aspx") >= 0) {
                return { title: "Tin phụ kiện mới", q: "phu kien cong nghe", filters: {}, emptyText: "Chưa có tin phụ kiện phù hợp." };
            }
            return null;
        }

        return null;
    }

    function shouldInjectAccountSnapshot(spaceCode, pathname) {
        if (spaceCode !== "home") return false;
        return pathname.indexOf("/app-ui/home/profile.aspx") >= 0
            || pathname.indexOf("/app-ui/home/settings.aspx") >= 0
            || pathname.indexOf("/app-ui/home/change-password.aspx") >= 0
            || pathname.indexOf("/app-ui/home/change-pin.aspx") >= 0;
    }

    function buildGuestSnapshotHtml() {
        return '' +
            '<p class="app-account-runtime-copy">Bạn chưa đăng nhập, cần đăng nhập để mở thông tin tài khoản.</p>' +
            '<div class="app-page-actions">' +
            '  <a class="app-page-btn is-primary" href="' + buildLoginHref() + '">Đăng nhập</a>' +
            '</div>';
    }

    function mapSellerStatus(status) {
        var value = (status || "not_registered").toLowerCase();
        if (value === "approved") return "Đã duyệt gian hàng";
        if (value === "pending") return "Đang chờ duyệt gian hàng";
        return "Chưa đăng ký gian hàng";
    }

    function injectAccountSnapshot(session, spaceCode) {
        var pathname = getPathname();
        if (!shouldInjectAccountSnapshot(spaceCode, pathname)) return;
        var wrap = document.querySelector(".app-page-wrap");
        if (!wrap) return;
        if (wrap.querySelector("[data-runtime-account='1']")) return;

        var panel = document.createElement("article");
        panel.className = "app-page-card app-account-runtime";
        panel.setAttribute("data-runtime-account", "1");

        if (!session || session.auth_state !== "authenticated") {
            panel.innerHTML = '' +
                '<h2>Tài khoản</h2>' +
                buildGuestSnapshotHtml();
        } else {
            var user = session.user_summary || {};
            var displayName = escapeHtml(user.display_name || "Tài khoản");
            var phone = escapeHtml(user.phone || "Chưa cập nhật");
            var role = escapeHtml(user.role_label || "Khách hàng");
            var sellerStatus = escapeHtml(mapSellerStatus(session.seller_status));
            var avatar = user.avatar_url
                ? '<img class="app-account-runtime-avatar-img" src="' + escapeHtml(user.avatar_url) + '" alt="' + displayName + '" />'
                : '<span class="app-account-runtime-avatar-fallback">' + escapeHtml(user.avatar_fallback || "AH") + '</span>';
            var openShopHref = session.seller_status === "approved"
                ? decorateAppHref("/app-ui/gianhang/default.aspx?ui_mode=app")
                : decorateAppHref("/app-ui/auth/open-shop.aspx?ui_mode=app");

            panel.innerHTML = '' +
                '<h2>Tài khoản</h2>' +
                '<div class="app-account-runtime-head">' +
                '  <span class="app-account-runtime-avatar">' + avatar + '</span>' +
                '  <div class="app-account-runtime-meta">' +
                '    <strong>' + displayName + '</strong>' +
                '    <span>' + phone + '</span>' +
                '  </div>' +
                '</div>' +
                '<div class="app-account-runtime-grid">' +
                '  <div class="app-account-runtime-item"><span>Vai trò</span><strong>' + role + '</strong></div>' +
                '  <div class="app-account-runtime-item"><span>Gian hàng</span><strong>' + sellerStatus + '</strong></div>' +
                '</div>' +
                '<div class="app-page-actions">' +
                '  <a class="app-page-btn is-primary" href="' + decorateAppHref("/app-ui/home/profile.aspx?ui_mode=app") + '">Mở hồ sơ</a>' +
                '  <a class="app-page-btn" href="' + openShopHref + '">' + (session.seller_status === "approved" ? "Vào Aha Shop" : "Đăng ký gian hàng") + '</a>' +
                '</div>';
        }

        var marker = wrap.querySelector(".app-runtime-card");
        if (marker) {
            marker.insertAdjacentElement("afterend", panel);
        } else {
            var firstCard = wrap.querySelector(".app-page-card");
            if (firstCard) firstCard.insertAdjacentElement("afterend", panel);
            else wrap.appendChild(panel);
        }
    }

    function buildRuntimePreviewHref(spaceCode, context) {
        var params = new URLSearchParams();
        params.set("space", spaceCode || "home");
        params.set("q", context.q || "");
        params.set("page", "1");
        params.set("page_size", "4");
        params.set("sort", "newest");
        var filters = context.filters || {};
        Object.keys(filters).forEach(function (key) {
            if (filters[key]) params.set(key, filters[key]);
        });
        return "/app-ui/shared/runtime-search.ashx?" + params.toString();
    }

    function buildAppDetailHref(spaceCode, itemId) {
        if (!itemId) return "";
        var href = "/app-ui/" + encodeURIComponent(spaceCode) + "/detail.aspx?id=" + encodeURIComponent(itemId) + "&ui_mode=app";
        return appendQuery(href, "back_href", getCurrentAppHref());
    }

    function renderPreviewRows(host, spaceCode, items) {
        if (!host) return;
        var safeItems = Array.isArray(items) ? items : [];
        host.innerHTML = safeItems.map(function (item) {
            var title = escapeHtml(item && item.title ? item.title : "Tin đăng");
            var price = item && item.price ? String(item.price) : "";
            var location = item && item.location ? String(item.location) : "";
            var metaParts = [price, location].filter(function (part) { return part && part.trim(); });
            var sub = metaParts.length ? escapeHtml(metaParts.join(" • ")) : "Dữ liệu mới";
            var href = buildAppDetailHref(spaceCode, item && item.id ? item.id : "");
            if (!href) href = decorateAppHref(getContextualSearchHref(spaceCode));
            return '' +
                '<a class="app-page-link" href="' + href + '">' +
                '  <span>' + title + '<small>' + sub + '</small></span>' +
                '  <span>&rsaquo;</span>' +
                '</a>';
        }).join("");
    }

    function injectRuntimePreview(session, spaceCode) {
        if (spaceCode === "gianhang") return;
        var wrap = document.querySelector(".app-page-wrap");
        if (!wrap) return;
        if (wrap.querySelector("[data-runtime-preview='1']")) return;

        var pathname = getPathname();
        var context = getRuntimePreviewContext(spaceCode, pathname);
        if (!context) return;

        var panel = document.createElement("article");
        panel.className = "app-page-card app-runtime-preview";
        panel.setAttribute("data-runtime-preview", "1");
        panel.innerHTML = '' +
            '<div class="app-runtime-preview-head">' +
            '  <h2>' + escapeHtml(context.title) + '</h2>' +
            '  <a class="app-runtime-pill-link" href="' + decorateAppHref(getContextualSearchHref(spaceCode)) + '">Xem tất cả</a>' +
            '</div>' +
            '<p class="app-runtime-preview-copy">Đang tải dữ liệu...</p>' +
            '<div class="app-page-links app-runtime-preview-links"></div>';

        var marker = wrap.querySelector(".app-runtime-card");
        if (marker) {
            marker.insertAdjacentElement("afterend", panel);
        } else {
            var firstCard = wrap.querySelector(".app-page-card");
            if (firstCard) {
                firstCard.insertAdjacentElement("afterend", panel);
            } else {
                wrap.appendChild(panel);
            }
        }

        var copyNode = panel.querySelector(".app-runtime-preview-copy");
        var linksNode = panel.querySelector(".app-runtime-preview-links");
        if (!window.fetch) {
            if (copyNode) copyNode.textContent = "Trình duyệt hiện tại chưa hỗ trợ tải dữ liệu xem nhanh.";
            return;
        }

        fetch(buildRuntimePreviewHref(spaceCode, context), { credentials: "same-origin" })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (payload) {
                if (!payload) {
                    if (copyNode) copyNode.textContent = "Không thể tải dữ liệu lúc này.";
                    return;
                }
                if (!payload.ok && payload.reason === "unauthorized") {
                    if (copyNode) {
                        if (!session || session.auth_state !== "authenticated") {
                            copyNode.innerHTML = 'Bạn cần <a href="' + buildLoginHref() + '">đăng nhập</a> để xem dữ liệu đầy đủ.';
                        } else {
                            copyNode.textContent = "Phiên đăng nhập chưa đủ quyền để xem dữ liệu này.";
                        }
                    }
                    return;
                }
                var items = Array.isArray(payload.items) ? payload.items : [];
                if (!items.length) {
                    if (copyNode) copyNode.textContent = payload.summary_text || context.emptyText || "Chưa có dữ liệu phù hợp.";
                    if (linksNode) linksNode.innerHTML = "";
                    return;
                }
                if (copyNode) {
                    copyNode.textContent = payload.summary_text || ("Đang hiển thị " + items.length + " tin mới nhất.");
                }
                renderPreviewRows(linksNode, spaceCode, items);
            })
            .catch(function () {
                if (copyNode) copyNode.textContent = "Lỗi kết nối dữ liệu, vui lòng thử lại.";
            });
    }

    function decorateAnchor(node) {
        if (!node || !node.getAttribute) return;
        var href = node.getAttribute("href") || "";
        var decorated = decorateRuntimeHref(href);
        if (decorated === href) {
            decorated = decorateAppHref(href);
        }
        if (decorated && decorated !== href) node.setAttribute("href", decorated);
    }

    function prunePageExplainers() {
        var wrap = document.querySelector(".app-page-wrap");
        if (!wrap) return;
        var cards = wrap.querySelectorAll(".app-page-card");
        cards.forEach(function (card) {
            var titleNode = card.querySelector("h2");
            if (!titleNode) return;
            var title = (titleNode.textContent || "").trim().toLowerCase();
            if (title === "luồng thao tác") {
                card.remove();
            }
        });
    }

    function decorateRuntimeAnchors() {
        var root = document.querySelector(".app-page-wrap");
        if (!root) return;
        root.querySelectorAll("a[href]").forEach(decorateAnchor);
    }

    function tuneAppActions(spaceCode) {
        var root = document.querySelector(".app-page-wrap");
        if (!root) return;
        root.querySelectorAll(".app-page-actions .app-page-btn").forEach(function (btn) {
            var text = ((btn.textContent || "").trim() || "").toLowerCase();
            if (text.indexOf("tìm kiếm") >= 0 || text.indexOf("tim kiem") >= 0) {
                btn.setAttribute("href", decorateAppHref(getContextualSearchHref(spaceCode)));
            }
        });
    }

    function buildLoginHref() {
        var returnUrl = encodeURIComponent(getCurrentAppHref());
        return "/app-ui/auth/login.aspx?ui_mode=app&return_url=" + returnUrl;
    }

    function getPostTarget(session) {
        if (!window.AhaAppUi || typeof window.AhaAppUi.getPostTarget !== "function") {
            return { href: "/app-ui/auth/open-shop.aspx?ui_mode=app", label: "Đăng ký gian hàng" };
        }
        return window.AhaAppUi.getPostTarget(session);
    }

    function getContextualRuntimeHref(spaceCode) {
        var pathname = getPathname();
        if (pathname.indexOf("/notifications.aspx") >= 0) {
            if (spaceCode === "gianhang") {
                return "/app-ui/gianhang/notifications.aspx?ui_mode=app";
            }
            if (spaceCode === "batdongsan") return "/app-ui/batdongsan/notifications.aspx?ui_mode=app";
            if (spaceCode === "choxe") return "/app-ui/choxe/notifications.aspx?ui_mode=app";
            if (spaceCode === "dienthoai-maytinh") return "/app-ui/dienthoai-maytinh/notifications.aspx?ui_mode=app";
            return "/app-ui/home/notifications.aspx?ui_mode=app";
        }
        if (spaceCode === "home" && pathname.indexOf("/home/categories.aspx") >= 0) {
            return "/app-ui/home/search.aspx?ui_mode=app";
        }
        return "";
    }

    function shouldUseSellerGate(spaceCode) {
        return spaceCode === "gianhang";
    }

    function applyPrimaryAction(session, spaceCode) {
        var runtimeBtn = getRuntimePrimaryButton();
        if (!runtimeBtn) return;

        if (!session || session.auth_state !== "authenticated") {
            runtimeBtn.href = buildLoginHref();
            runtimeBtn.textContent = "Đăng nhập để tiếp tục";
            runtimeBtn.setAttribute("data-runtime-auth", "guest");
            return;
        }

        if (shouldUseSellerGate(spaceCode)) {
            var sellerStatus = (session.seller_status || "not_registered").toLowerCase();
            if (sellerStatus !== "approved") {
                var postTarget = getPostTarget(session);
                runtimeBtn.href = decorateRuntimeHref(postTarget.href || "/app-ui/auth/open-shop.aspx?ui_mode=app");
                runtimeBtn.textContent = postTarget.label || "Đăng ký gian hàng";
                runtimeBtn.setAttribute("data-runtime-auth", "seller-gated");
                return;
            }
        }

        var contextualHref = getContextualRuntimeHref(spaceCode);
        if (contextualHref) {
            runtimeBtn.href = decorateRuntimeHref(contextualHref);
            runtimeBtn.textContent = spaceCode === "gianhang" ? "Mở thông báo" : "Mở trung tâm thông báo";
            runtimeBtn.setAttribute("data-runtime-auth", "ok");
            return;
        }

        runtimeBtn.href = decorateRuntimeHref(runtimeBtn.getAttribute("href") || "");
        runtimeBtn.setAttribute("data-runtime-auth", "ok");
    }

    function buildRuntimeCard(session, runtimeHref, spaceCode) {
        var auth = !!(session && session.auth_state === "authenticated");
        var dotClass = auth ? "is-ok" : "is-warn";
        var user = session && session.user_summary ? session.user_summary : null;
        var userName = user ? (user.display_name || user.phone || "") : "";
        var sellerStatus = (session && session.seller_status) ? session.seller_status : "not_registered";
        var statusScope = spaceCode === "gianhang" ? (" • Gian hàng: " + sellerStatus) : "";
        var text = auth
            ? "Đang dùng phiên tài khoản hiện tại" + (userName ? ": " + userName : "") + statusScope + "."
            : "Bạn chưa đăng nhập, cần đăng nhập để thao tác dữ liệu thật.";
        var openRuntime = runtimeHref
            ? '<a class="app-runtime-pill-link" href="' + runtimeHref + '">Mở dữ liệu</a>'
            : "";
        return '' +
            '<article class="app-runtime-card">' +
            '  <div class="app-runtime-row">' +
            '    <span class="app-runtime-dot ' + dotClass + '"></span>' +
            '    <div class="app-runtime-copy">' +
            '      <h3 class="app-runtime-title">Trạng thái kết nối dữ liệu</h3>' +
            '      <p class="app-runtime-text">' + text + '</p>' +
            '    </div>' +
            openRuntime +
            '  </div>' +
            '</article>';
    }

    function injectRuntimeCard(session, spaceCode) {
        var wrap = document.querySelector(".app-page-wrap");
        var hero = document.querySelector(".app-page-hero");
        if (!wrap || !hero) return;
        if (wrap.querySelector(".app-runtime-card")) return;
        var runtimeBtn = getRuntimePrimaryButton();
        var runtimeHref = runtimeBtn ? runtimeBtn.getAttribute("href") : "";
        hero.insertAdjacentHTML("afterend", buildRuntimeCard(session, runtimeHref, spaceCode));
    }

    function enhanceCopy(session) {
        var copy = document.querySelector(".app-page-copy");
        if (!copy) return;
        if (copy.getAttribute("data-runtime-copy") === "1") return;
        if (!session || session.auth_state !== "authenticated") {
            copy.textContent = copy.textContent + " Đăng nhập để dùng đầy đủ dữ liệu.";
        }
        copy.setAttribute("data-runtime-copy", "1");
    }

    function bootstrap() {
        if (!document.querySelector(".app-page-wrap")) return;
        var session = getSession();
        var spaceCode = getSpaceCode();
        applyPrimaryAction(session, spaceCode);
        tuneAppActions(spaceCode);
        decorateRuntimeAnchors();
    }

    bootstrap();
})();
