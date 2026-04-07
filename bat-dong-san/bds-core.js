(function () {
    var bootstrapNs = window.bootstrap || (window.bootstrap = {});
    var offcanvasInstances = [];

    function dispatchEventCompat(element, eventName) {
        if (!element) return;
        var evt;
        try {
            evt = new CustomEvent(eventName, { bubbles: true });
        } catch (e) {
            evt = document.createEvent("CustomEvent");
            evt.initCustomEvent(eventName, true, false, null);
        }
        element.dispatchEvent(evt);
    }

    function updateBodyState() {
        var hasOpen = !!document.querySelector(".offcanvas.show, .offcanvas.showing");
        if (!document.body) return;
        document.body.classList.toggle("offcanvas-open", hasOpen);
    }

    function Offcanvas(element) {
        this.element = element;
        if (offcanvasInstances.indexOf(this) < 0) {
            offcanvasInstances.push(this);
        }
    }

    Offcanvas.getInstance = function (element) {
        if (!element) return null;
        for (var i = 0; i < offcanvasInstances.length; i++) {
            if (offcanvasInstances[i].element === element) return offcanvasInstances[i];
        }
        return null;
    };

    Offcanvas.getOrCreateInstance = function (element) {
        return Offcanvas.getInstance(element) || new Offcanvas(element);
    };

    Offcanvas.prototype.show = function () {
        var element = this.element;
        if (!element) return;

        for (var i = 0; i < offcanvasInstances.length; i++) {
            var other = offcanvasInstances[i];
            if (other && other !== this) other.hide();
        }

        dispatchEventCompat(element, "show.bs.offcanvas");
        element.classList.remove("hiding");
        element.classList.add("showing");
        element.setAttribute("aria-modal", "true");
        element.setAttribute("role", "dialog");

        window.requestAnimationFrame(function () {
            element.classList.remove("showing");
            element.classList.add("show");
            updateBodyState();
            dispatchEventCompat(element, "shown.bs.offcanvas");
        });
    };

    Offcanvas.prototype.hide = function () {
        var element = this.element;
        if (!element) return;
        if (!element.classList.contains("show") && !element.classList.contains("showing")) {
            element.classList.remove("hiding");
            updateBodyState();
            return;
        }

        dispatchEventCompat(element, "hide.bs.offcanvas");
        element.classList.remove("showing");
        element.classList.remove("show");
        element.classList.add("hiding");
        element.removeAttribute("aria-modal");
        element.removeAttribute("role");

        window.setTimeout(function () {
            element.classList.remove("hiding");
            updateBodyState();
            dispatchEventCompat(element, "hidden.bs.offcanvas");
        }, 220);
    };

    bootstrapNs.Offcanvas = Offcanvas;

    function resolveTarget(trigger) {
        if (!trigger) return null;
        var selector = trigger.getAttribute("data-bs-target") || trigger.getAttribute("href") || "";
        if (!selector || selector.charAt(0) !== "#") return null;
        return document.querySelector(selector);
    }

    document.addEventListener("click", function (event) {
        var dismissBtn = event.target.closest("[data-bs-dismiss='offcanvas']");
        if (dismissBtn) {
            var offcanvas = dismissBtn.closest(".offcanvas");
            if (offcanvas) {
                event.preventDefault();
                Offcanvas.getOrCreateInstance(offcanvas).hide();
            }
            return;
        }

        var trigger = event.target.closest("[data-bs-toggle='offcanvas']");
        if (!trigger) return;

        var target = resolveTarget(trigger);
        if (!target) return;

        event.preventDefault();
        Offcanvas.getOrCreateInstance(target).show();
    });

    document.addEventListener("keydown", function (event) {
        if (event.key !== "Escape") return;
        var openCanvas = document.querySelector(".offcanvas.show, .offcanvas.showing");
        if (!openCanvas) return;
        Offcanvas.getOrCreateInstance(openCanvas).hide();
    });

    window.addEventListener("pageshow", function () {
        updateBodyState();
    });
})();
