(function () {
    function getUiScaffold() {
        return (window.AhaAppUiData && window.AhaAppUiData.gianhang) || {
            quickActions: [],
            statusTabs: [],
            metrics: [],
            listings: [],
            conversations: [],
            orders: [],
            appointments: [],
            reportSummary: null
        };
    }

    function buildEmptySnapshot(scaffold) {
        return {
            quickActions: scaffold.quickActions || [],
            statusTabs: scaffold.statusTabs || [],
            metrics: [
                { label: "Tin đăng hoạt động", value: "0" },
                { label: "Khách đang trao đổi", value: "0" },
                { label: "Lịch hẹn hôm nay", value: "0" }
            ],
            listings: [],
            conversations: [],
            orders: [],
            appointments: [],
            reportSummary: null
        };
    }

    function normalizeSnapshot(snapshot) {
        if (!snapshot) return null;
        return {
            quickActions: snapshot.quick_actions || snapshot.quickActions || [],
            statusTabs: snapshot.status_tabs || snapshot.statusTabs || [],
            metrics: snapshot.metrics || [],
            listings: snapshot.listings || [],
            conversations: snapshot.conversations || [],
            orders: snapshot.orders || [],
            appointments: snapshot.appointments || [],
            reportSummary: snapshot.report_summary || snapshot.reportSummary || null
        };
    }

    function rememberListings(items) {
        if (!items || !items.length) return;
        if (window.AhaAppUiAdapters && window.AhaAppUiAdapters.listing && typeof window.AhaAppUiAdapters.listing.rememberItems === "function") {
            window.AhaAppUiAdapters.listing.rememberItems(items);
        }
    }

    function fetchRuntime() {
        if (!window.fetch) return Promise.resolve(null);
        return fetch("/app-ui/shared/runtime-seller.ashx", { credentials: "same-origin" })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (payload) {
                if (!payload) return null;
                if (!payload.ok && payload.reason === "unauthorized") {
                    return { __unauthorized: true };
                }
                if (!payload.ok || !payload.snapshot) return null;
                var normalized = normalizeSnapshot(payload.snapshot);
                if (normalized && normalized.listings) {
                    rememberListings(normalized.listings);
                }
                return normalized;
            })
            .catch(function () { return null; });
    }

    function getSnapshotAsync() {
        var scaffold = getUiScaffold();
        var session = window.AhaAppUiAdapters && window.AhaAppUiAdapters.session
            ? window.AhaAppUiAdapters.session.getSession()
            : null;
        var sellerStatus = (session && session.seller_status) || "not_registered";
        var canUseSellerRuntime = sellerStatus === "approved";

        if (!canUseSellerRuntime) {
            return Promise.resolve(buildEmptySnapshot(scaffold));
        }

        return fetchRuntime().then(function (runtime) {
            if (runtime && runtime.__unauthorized) {
                return buildEmptySnapshot(scaffold);
            }
            if (!runtime) return buildEmptySnapshot(scaffold);
            return {
                quickActions: runtime.quickActions && runtime.quickActions.length ? runtime.quickActions : scaffold.quickActions,
                statusTabs: runtime.statusTabs && runtime.statusTabs.length ? runtime.statusTabs : scaffold.statusTabs,
                metrics: runtime.metrics && runtime.metrics.length ? runtime.metrics : buildEmptySnapshot(scaffold).metrics,
                listings: runtime.listings && runtime.listings.length ? runtime.listings : [],
                conversations: runtime.conversations && runtime.conversations.length ? runtime.conversations : [],
                orders: runtime.orders || [],
                appointments: runtime.appointments || [],
                reportSummary: runtime.reportSummary || null
            };
        });
    }

    window.AhaAppUiAdapters = window.AhaAppUiAdapters || {};
    window.AhaAppUiAdapters.seller = {
        getSnapshotAsync: getSnapshotAsync
    };
})();
