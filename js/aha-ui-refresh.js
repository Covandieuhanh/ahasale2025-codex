(function () {
    "use strict";

    function setVhUnit() {
        var vh = window.innerHeight * 0.01;
        document.documentElement.style.setProperty("--aha-vh", vh + "px");
    }

    function markWideTables(root) {
        var scope = root || document;
        var tables = scope.querySelectorAll("table");
        for (var i = 0; i < tables.length; i++) {
            var table = tables[i];
            var parent = table.parentElement;
            if (!parent) continue;
            if (table.closest(".bcorn-fix-title-table-container")) continue;
            if (parent.classList.contains("table-responsive")) continue;

            var parentWidth = parent.clientWidth || 0;
            var tableWidth = table.scrollWidth || 0;
            if (tableWidth > parentWidth + 8) {
                parent.classList.add("aha-table-wrap");
            } else {
                parent.classList.remove("aha-table-wrap");
            }
        }
    }

    function normalizeAdminTables(root) {
        if (!document.body || !document.body.classList.contains("admin-shell")) {
            return;
        }

        var scope = root || document;
        var tables = scope.querySelectorAll(".admin-page table");
        for (var i = 0; i < tables.length; i++) {
            var table = tables[i];
            if (!table || table.classList.contains("no-admin-unify")) continue;

            table.classList.add("aha-admin-table");

            var parent = table.parentElement;
            if (!parent) continue;

            if (!parent.classList.contains("aha-admin-table-shell")) {
                var tag = parent.tagName;
                var shouldWrap = (
                    tag !== "TD" &&
                    tag !== "TH" &&
                    !parent.classList.contains("table-responsive") &&
                    !parent.classList.contains("bcorn-fix-title-table-container")
                );

                if (shouldWrap) {
                    var wrapper = document.createElement("div");
                    wrapper.className = "aha-admin-table-shell";
                    parent.insertBefore(wrapper, table);
                    wrapper.appendChild(table);
                    parent = wrapper;
                }
            }

            if (parent.classList) {
                parent.classList.add("aha-admin-table-shell");
            }
        }
    }

    function normalizeAdminComponents(root) {
        if (!document.body || !document.body.classList.contains("admin-shell")) {
            return;
        }

        var scope = root || document;

        var toolbars = scope.querySelectorAll(".admin-page #menutop-tool-bc");
        for (var i = 0; i < toolbars.length; i++) {
            toolbars[i].classList.add("aha-admin-toolbar");
        }

        var toolbarSearches = scope.querySelectorAll(".admin-page #timkiem-fixtop-bc");
        for (var j = 0; j < toolbarSearches.length; j++) {
            toolbarSearches[j].classList.add("aha-admin-toolbar-search");
        }

        var sections = scope.querySelectorAll(".admin-page > .p-3");
        for (var k = 0; k < sections.length; k++) {
            sections[k].classList.add("aha-admin-section");
        }

        var legacySections = scope.querySelectorAll(".admin-page > div[class*='pt-3'][class*='pl-3']");
        for (var l = 0; l < legacySections.length; l++) {
            legacySections[l].classList.add("aha-admin-section");
        }

        var tableWraps = scope.querySelectorAll(".admin-page .bcorn-fix-title-table-container, .admin-page .table-responsive");
        for (var m = 0; m < tableWraps.length; m++) {
            tableWraps[m].classList.add("aha-admin-grid");
        }

        var tableRows = scope.querySelectorAll(".admin-page .dropdown-button > .button.small.bg-transparent");
        for (var n = 0; n < tableRows.length; n++) {
            tableRows[n].classList.add("aha-row-actions-trigger");
        }

        var selects = scope.querySelectorAll(".admin-page select[data-role='select'], .admin-page .select select");
        for (var p = 0; p < selects.length; p++) {
            selects[p].classList.add("aha-admin-select");
        }

        var modalHeads = scope.querySelectorAll("div[style*='z-index: 1041'][style*='height: 52px']");
        for (var q = 0; q < modalHeads.length; q++) {
            var headWrap = modalHeads[q];
            headWrap.classList.add("aha-admin-modal-head-wrap");

            var headCard = headWrap.querySelector("div[style*='max-width']");
            if (headCard) {
                headCard.classList.add("aha-admin-modal-head-card");
            }

            var closeButton = headWrap.querySelector("a[title='Đóng']");
            if (closeButton) {
                closeButton.classList.add("aha-admin-modal-close");
            }
        }

        var modalOverlays = scope.querySelectorAll("div[style*='z-index: 1040'][style*='background-image: url']");
        for (var r = 0; r < modalOverlays.length; r++) {
            var overlay = modalOverlays[r];
            overlay.classList.add("aha-admin-modal-overlay");

            var dialog = overlay.querySelector("div[style*='max-width']");
            if (dialog) {
                dialog.classList.add("aha-admin-modal-dialog");
            }

            var body = overlay.querySelector(".bg-white.border.bd-transparent");
            if (body) {
                body.classList.add("aha-admin-modal-body");
            }
        }

        var progressMasks = scope.querySelectorAll("div[id*='UpdateProgress'] .bg-dark.fixed-top.h-100.w-100");
        for (var s = 0; s < progressMasks.length; s++) {
            progressMasks[s].classList.add("aha-admin-progress-mask");
        }

        var progressAtoms = scope.querySelectorAll("div[id*='UpdateProgress'] .activity-atom");
        for (var t = 0; t < progressAtoms.length; t++) {
            progressAtoms[t].classList.add("aha-admin-progress-atom");
        }
    }

    function normalizeAdminStatusPills(root) {
        if (!document.body || !document.body.classList.contains("admin-shell")) {
            return;
        }

        var scope = root || document;
        var selectors = [
            ".admin-page .status-pill",
            ".admin-page .aha-admin-status",
            ".admin-page .bridge-inline-badge",
            ".admin-page .yc-kpi",
            ".admin-page .bcorn-fix-title-table .button.mini",
            ".admin-page .table .button.mini"
        ].join(",");

        var items = scope.querySelectorAll(selectors);
        for (var i = 0; i < items.length; i++) {
            var el = items[i];
            if (!el || el.classList.contains("mota-item")) continue;
            if (el.closest(".mota-tabs")) continue;

            var txt = (el.textContent || "").replace(/\s+/g, " ").trim().toLowerCase();
            if (!txt) continue;
            if (txt.length > 80) continue;

            el.classList.add("aha-status-pill");
            el.classList.remove(
                "aha-status-pending",
                "aha-status-approved",
                "aha-status-rejected",
                "aha-status-neutral",
                "aha-tag-khachhang",
                "aha-tag-congtac",
                "aha-tag-donghanh",
                "aha-tag-shop",
                "aha-tag-admin",
                "aha-tag-tong"
            );

            if (/ch[ơo]\s*duy[ệe]t|pending|đang chờ|đang xử lý|processing/.test(txt)) {
                el.classList.add("aha-status-pending");
            } else if (/đã duyệt|hoạt động|thành công|approved|active|^in$/.test(txt)) {
                el.classList.add("aha-status-approved");
            } else if (/từ chối|hủy|huỷ|khóa|khóa thẻ|lỗi|fail|rejected|inactive|^out$/.test(txt)) {
                el.classList.add("aha-status-rejected");
            } else {
                el.classList.add("aha-status-neutral");
            }

            if (/khách hàng/.test(txt)) {
                el.classList.add("aha-tag-khachhang");
            } else if (/cộng tác phát triển/.test(txt)) {
                el.classList.add("aha-tag-congtac");
            } else if (/đồng hành hệ sinh thái/.test(txt)) {
                el.classList.add("aha-tag-donghanh");
            } else if (/gian hàng đối tác/.test(txt)) {
                el.classList.add("aha-tag-shop");
            } else if (/nhân viên admin/.test(txt)) {
                el.classList.add("aha-tag-admin");
            } else if (/tài khoản tổng/.test(txt)) {
                el.classList.add("aha-tag-tong");
            }
        }
    }

    function revealBlocks(root) {
        if (document.body && document.body.classList.contains("admin-shell")) {
            return;
        }

        var scope = root || document;
        var selector = [
            ".card",
            ".panel",
            ".box",
            ".tile",
            ".bcorn-fix-title-table-container",
            ".content-inner > div"
        ].join(",");

        var blocks = scope.querySelectorAll(selector);
        if (!blocks.length) return;

        var reduceMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
        if (reduceMotion || !("IntersectionObserver" in window)) {
            for (var i = 0; i < blocks.length; i++) {
                blocks[i].classList.add("is-visible");
            }
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    observer.unobserve(entry.target);
                }
            });
        }, {
            rootMargin: "0px 0px -8% 0px",
            threshold: 0.08
        });

        for (var j = 0; j < blocks.length; j++) {
            var block = blocks[j];
            if (!block.classList.contains("aha-reveal")) {
                block.classList.add("aha-reveal");
            }
            observer.observe(block);
        }
    }

    function hideStuckUpdateProgress() {
        if (!(window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)) {
            return;
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (!prm) {
            return;
        }

        if (typeof prm.get_isInAsyncPostBack === "function" && prm.get_isInAsyncPostBack()) {
            return;
        }

        var holders = document.querySelectorAll("div[id*='UpdateProgress']");
        for (var i = 0; i < holders.length; i++) {
            var holder = holders[i];
            if (!holder || !holder.style) continue;

            var style = window.getComputedStyle ? window.getComputedStyle(holder) : null;
            if (!style) continue;

            if (style.display !== "none" && style.visibility !== "hidden") {
                holder.style.display = "none";
            }
        }
    }

    function markDeviceInput() {
        var coarse = window.matchMedia && window.matchMedia("(pointer: coarse)").matches;
        document.body.classList.toggle("aha-touch", !!coarse);
    }

    function syncSearchInputs(root) {
        var scope = root || document;
        var inputs = scope.querySelectorAll("[data-sync-key]");
        if (!inputs.length) return;

        var groups = {};
        for (var i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            var key = input.getAttribute("data-sync-key");
            if (!key) continue;

            if (!groups[key]) groups[key] = [];
            groups[key].push(input);
        }

        for (var groupKey in groups) {
            if (!Object.prototype.hasOwnProperty.call(groups, groupKey)) continue;
            var group = groups[groupKey];
            if (!group || !group.length) continue;

            var initial = "";
            for (var j = 0; j < group.length; j++) {
                var val = (group[j].value || "").trim();
                if (val) {
                    initial = group[j].value;
                    break;
                }
            }

            if (initial) {
                for (var k = 0; k < group.length; k++) {
                    if (group[k].value !== initial) {
                        group[k].value = initial;
                    }
                }
            }

            for (var m = 0; m < group.length; m++) {
                var current = group[m];
                if (current.getAttribute("data-sync-bound") === "1") continue;

                current.setAttribute("data-sync-bound", "1");
                current.addEventListener("input", function () {
                    var syncKey = this.getAttribute("data-sync-key");
                    if (!syncKey || !groups[syncKey]) return;

                    var peers = groups[syncKey];
                    for (var n = 0; n < peers.length; n++) {
                        if (peers[n] !== this && peers[n].value !== this.value) {
                            peers[n].value = this.value;
                        }
                    }
                });
            }
        }
    }

    function bindEnterToAction(root) {
        var scope = root || document;
        var fields = scope.querySelectorAll("[data-enter-click]");
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            if (field.getAttribute("data-enter-bound") === "1") continue;

            field.setAttribute("data-enter-bound", "1");
            field.addEventListener("keydown", function (event) {
                var key = event.key || event.keyCode;
                if (key !== "Enter" && key !== 13) return;

                event.preventDefault();
                var targetId = this.getAttribute("data-enter-click");
                if (!targetId) return;

                var target = document.getElementById(targetId);
                if (target && typeof target.click === "function") {
                    target.click();
                }
            });
        }
    }

    var compactStorageKey = "aha_admin_compact_mode";

    function getStoredCompactMode() {
        try {
            return window.localStorage.getItem(compactStorageKey);
        } catch (err) {
            return null;
        }
    }

    function setStoredCompactMode(value) {
        try {
            if (value === null) {
                window.localStorage.removeItem(compactStorageKey);
            } else {
                window.localStorage.setItem(compactStorageKey, value ? "1" : "0");
            }
        } catch (err) { }
    }

    function setMenuTitles(compact) {
        var links = document.querySelectorAll("#side-menu li a");
        for (var i = 0; i < links.length; i++) {
            var cap = links[i].querySelector(".caption");
            if (!cap) continue;
            var text = (cap.textContent || "").trim();
            if (!text) continue;

            if (compact) {
                links[i].setAttribute("title", text);
            } else {
                links[i].removeAttribute("title");
            }
        }
    }

    function applyAdminCompactMode(forceCompact) {
        if (!document.body || !document.body.classList.contains("admin-shell")) return;

        var toggle = document.getElementById("admin-ui-density-toggle");
        var mobile = window.innerWidth < 992;
        if (mobile) {
            document.body.classList.remove("admin-smart-compact");
            if (toggle) {
                toggle.classList.remove("active");
                toggle.setAttribute("aria-pressed", "false");
            }
            setMenuTitles(false);
            return;
        }

        var stored = getStoredCompactMode();
        var compact = false;

        if (typeof forceCompact === "boolean") {
            compact = forceCompact;
        } else {
            // Keep fixed default menu mode to avoid tab label flicker on page navigation.
            compact = false;
        }

        document.body.classList.toggle("admin-smart-compact", compact);
        setMenuTitles(compact);

        if (toggle) {
            toggle.classList.toggle("active", compact);
            toggle.setAttribute("aria-pressed", compact ? "true" : "false");
            toggle.setAttribute("title", compact ? "Mở rộng menu" : "Thu gọn menu");
        }
    }

    function bindAdminCompactToggle(root) {
        var scope = root || document;
        var toggle = scope.getElementById ? scope.getElementById("admin-ui-density-toggle") : document.getElementById("admin-ui-density-toggle");
        if (!toggle || toggle.getAttribute("data-compact-bound") === "1") return;

        toggle.setAttribute("data-compact-bound", "1");
        toggle.addEventListener("click", function (event) {
            event.preventDefault();
            var next = !document.body.classList.contains("admin-smart-compact");
            setStoredCompactMode(next);
            applyAdminCompactMode(next);
        });
    }

    function initAdminSmartLayout(root) {
        if (!document.body || !document.body.classList.contains("admin-shell")) return;
        bindAdminCompactToggle(root);
        applyAdminCompactMode();
    }

    function init(root) {
        setVhUnit();
        markDeviceInput();
        syncSearchInputs(root);
        bindEnterToAction(root);
        initAdminSmartLayout(root);
        normalizeAdminComponents(root);
        normalizeAdminStatusPills(root);
        normalizeAdminTables(root);
        markWideTables(root);
        revealBlocks(root);
        hideStuckUpdateProgress();
    }

    function refreshLayout(root) {
        setVhUnit();
        markDeviceInput();
        syncSearchInputs(root);
        bindEnterToAction(root);
        initAdminSmartLayout(root);
        normalizeAdminComponents(root);
        normalizeAdminStatusPills(root);
        normalizeAdminTables(root);
        markWideTables(root);
        hideStuckUpdateProgress();
    }

    document.addEventListener("DOMContentLoaded", function () {
        init(document);
    });

    window.addEventListener("load", function () {
        hideStuckUpdateProgress();
    });

    var resizeTimer = null;
    window.addEventListener("resize", function () {
        if (resizeTimer) {
            window.clearTimeout(resizeTimer);
        }
        resizeTimer = window.setTimeout(function () {
            refreshLayout(document);
        }, 120);
    });

    if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            init(document);
            window.setTimeout(hideStuckUpdateProgress, 120);
        });
    }
})();
