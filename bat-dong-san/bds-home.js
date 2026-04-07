(function () {
    function splitCandidates(raw) {
        if (!raw) return [];
        return raw.split("|")
            .map(function (x) { return (x || "").trim(); })
            .filter(function (x) { return x.length > 0; });
    }

    function attachLinkedThumbFallback(img) {
        if (!img) return;

        var fallback = img.getAttribute("data-fallback") || "/uploads/images/bat-dong-san/bds-placeholder.jpg";
        var candidates = splitCandidates(img.getAttribute("data-gallery"));
        var current = img.getAttribute("src") || "";
        if (current) candidates.unshift(current);

        var unique = [];
        for (var i = 0; i < candidates.length; i++) {
            if (unique.indexOf(candidates[i]) < 0) unique.push(candidates[i]);
        }

        var index = unique.indexOf(current);
        if (index < 0) index = 0;

        img.onerror = function () {
            index++;
            if (index < unique.length) {
                img.src = unique[index];
                return;
            }
            img.onerror = null;
            img.src = fallback;
        };
    }

    function initLinkedThumbFallback() {
        var images = document.querySelectorAll("img[data-role='linked-thumb']");
        for (var i = 0; i < images.length; i++) {
            attachLinkedThumbFallback(images[i]);
        }
    }

    function initRegionOverviewTabs() {
        var root = document.getElementById("regionOverviewBlock");
        if (!root) return;

        var typeButtons = root.querySelectorAll("[data-type-tab]");
        var cards = root.querySelectorAll("[data-region-card]");

        var state = {
            type: "all"
        };

        function getCount(card, type) {
            var key = "data-total-" + type;
            var raw = card.getAttribute(key) || "0";
            var n = parseInt(raw, 10);
            return isNaN(n) ? 0 : n;
        }

        function getPurposeHref(card, type) {
            var saleRaw = card.getAttribute("data-sale-" + type) || "0";
            var rentRaw = card.getAttribute("data-rent-" + type) || "0";
            var saleCount = parseInt(saleRaw, 10);
            var rentCount = parseInt(rentRaw, 10);
            if (isNaN(saleCount)) saleCount = 0;
            if (isNaN(rentCount)) rentCount = 0;
            return rentCount > saleCount ? "/bat-dong-san/cho-thue.aspx" : "/bat-dong-san/mua-ban.aspx";
        }

        function refresh() {
            for (var j = 0; j < typeButtons.length; j++) {
                var t = typeButtons[j].getAttribute("data-type-tab");
                typeButtons[j].classList.toggle("active", t === state.type);
            }

            for (var k = 0; k < cards.length; k++) {
                var card = cards[k];
                var count = getCount(card, state.type);
                var countNode = card.querySelector("[data-region-count]");
                if (countNode) countNode.textContent = count.toLocaleString("vi-VN");

                var province = card.getAttribute("data-province") || "";
                var href = getPurposeHref(card, state.type);
                var query = [];
                if (province) query.push("province=" + encodeURIComponent(province));
                if (state.type && state.type !== "all") query.push("propertyType=" + encodeURIComponent(state.type));
                card.setAttribute("href", href + (query.length ? "?" + query.join("&") : ""));
            }
        }

        for (var i = 0; i < typeButtons.length; i++) {
            typeButtons[i].addEventListener("click", function () {
                state.type = this.getAttribute("data-type-tab") || "all";
                refresh();
            });
        }

        refresh();
    }

    function initHeroSearchModeSelect() {
        var hidden = document.getElementById("bdsSearchMode");
        var input = document.getElementById("bdsKeyword");
        var select = document.getElementById("bdsSearchModeSelect");
        if (!hidden || !select) return;

        var placeholders = {
            sale: "VD: BÃ¡n nhÃ  BÃ¬nh Tháº¡nh dÆ°á»›i 5 tá»·",
            rent: "VD: ThuÃª cÄƒn há»™ BiÃªn HÃ²a 2 phÃ²ng ngá»§",
            project: "VD: Dá»± Ã¡n cÄƒn há»™ Thá»§ Äá»©c"
        };

        function apply(mode) {
            hidden.value = mode;
            if (select.value !== mode) select.value = mode;
            if (input) input.setAttribute("placeholder", placeholders[mode] || placeholders.sale);
        }

        select.addEventListener("change", function () {
            apply(select.value || "sale");
        });

        apply(hidden.value || "sale");
    }

    function initShortcutButtons() {
        var buttons = document.querySelectorAll("[data-bds-dev-message]");
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].addEventListener("click", function (e) {
                e.preventDefault();
                var message = this.getAttribute("data-bds-dev-message") || "TÃ­nh nÄƒng nÃ y Ä‘ang trong giai Ä‘oáº¡n phÃ¡t triá»ƒn.";
                if (window.show_toast) {
                    show_toast(message, "warning", 2800, true, "Báº¥t Ä‘á»™ng sáº£n");
                } else {
                    alert(message);
                }
            });
        }
    }

    function initBdsStickySearch() {
        var wrap = document.getElementById("bdsSearchWrap");
        if (!wrap) return;
        var spacer = document.getElementById("bdsSearchWrapSpacer");
        var ticking = false;
        var isFixed = false;
        var headerOffset = -1;

        function syncHeaderOffset() {
            var header = document.querySelector(".bds-topbar");
            if (!header) return;
            var headerInner = header.querySelector(".bds-topbar-inner");
            var rect = headerInner ? headerInner.getBoundingClientRect() : header.getBoundingClientRect();
            if (!rect || !rect.height) return;

            var offset = Math.round(rect.height);
            if (offset < 56) offset = 56;
            if (offset > 96) offset = 96;
            if (offset <= 0 || offset === headerOffset) return;
            headerOffset = offset;
            document.documentElement.style.setProperty("--aha-home-sticky-top", offset + "px");
        }

        function isOverlayOpen() {
            return !!document.querySelector(".offcanvas.show, .modal.show");
        }

        function getStickyTop() {
            var topOffset = headerOffset > 0 ? headerOffset : 72;
            var stickyGap = parseInt(getComputedStyle(document.documentElement).getPropertyValue("--bds-search-sticky-gap"), 10) || 0;
            return topOffset + stickyGap;
        }

        function getAnchorTop() {
            var anchor = spacer || wrap;
            return anchor.getBoundingClientRect().top + window.scrollY;
        }

        function setFixed(nextFixed) {
            if (nextFixed === isFixed) {
                if (nextFixed && spacer) spacer.style.height = wrap.offsetHeight + "px";
                return;
            }
            isFixed = nextFixed;
            wrap.classList.toggle("is-sticky-fixed", nextFixed);
            if (spacer) spacer.style.height = nextFixed ? (wrap.offsetHeight + "px") : "0px";
        }

        function update() {
            if (!document.body.contains(wrap)) return;
            if (isOverlayOpen()) return;

            var stickyTop = getStickyTop();
            var triggerY = getAnchorTop() - stickyTop;
            var currentY = window.scrollY || window.pageYOffset || 0;
            setFixed(currentY >= triggerY);
        }

        function queueUpdate() {
            if (ticking) return;
            ticking = true;
            window.requestAnimationFrame(function () {
                ticking = false;
                update();
            });
        }

        syncHeaderOffset();
        window.addEventListener("scroll", queueUpdate, { passive: true });
        window.addEventListener("resize", function () {
            syncHeaderOffset();
            queueUpdate();
        });
        window.addEventListener("load", function () {
            syncHeaderOffset();
            queueUpdate();
        });
        document.addEventListener("shown.bs.offcanvas", queueUpdate);
        document.addEventListener("hidden.bs.offcanvas", queueUpdate);

        if ("ResizeObserver" in window) {
            var headerEl = document.querySelector(".bds-topbar");
            if (headerEl) {
                var ro = new ResizeObserver(function () {
                    syncHeaderOffset();
                    queueUpdate();
                });
                ro.observe(headerEl);
            }
        }

        queueUpdate();
    }

    function init() {
        initLinkedThumbFallback();
        initRegionOverviewTabs();
        initHeroSearchModeSelect();
        initBdsStickySearch();
        initShortcutButtons();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
