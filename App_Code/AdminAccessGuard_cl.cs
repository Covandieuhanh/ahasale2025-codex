using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public static class AdminAccessGuard_cl
{
    private const string RequestAccessCacheKey = "__admin_access_guard_feature_cache";

    private static IDictionary<string, bool> GetRequestAccessCache()
    {
        HttpContext context = HttpContext.Current;
        if (context == null)
            return null;

        object raw = context.Items[RequestAccessCacheKey];
        IDictionary<string, bool> cache = raw as IDictionary<string, bool>;
        if (cache != null)
            return cache;

        cache = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        context.Items[RequestAccessCacheKey] = cache;
        return cache;
    }

    private static string BuildRequestCacheEntryKey(string accountKey, string featureKey)
    {
        return (accountKey ?? "").Trim().ToLowerInvariant() + "|" + (featureKey ?? "").Trim().ToLowerInvariant();
    }

    private static bool TryGetCachedAccessDecision(string accountKey, string featureKey, out bool allowed)
    {
        allowed = false;
        IDictionary<string, bool> cache = GetRequestAccessCache();
        if (cache == null)
            return false;

        string key = BuildRequestCacheEntryKey(accountKey, featureKey);
        return cache.TryGetValue(key, out allowed);
    }

    private static void CacheAccessDecision(string accountKey, string featureKey, bool allowed)
    {
        IDictionary<string, bool> cache = GetRequestAccessCache();
        if (cache == null)
            return;

        string key = BuildRequestCacheEntryKey(accountKey, featureKey);
        cache[key] = allowed;
    }

    private static string NormalizeFallbackUrl(string featureKey, string fallbackUrl)
    {
        string safe = (fallbackUrl ?? "").Trim();
        if (string.IsNullOrWhiteSpace(safe))
            return "";

        // Backward-compatible fix: old pages may still pass legacy "mspace=core".
        if (safe.IndexOf("mspace=core", StringComparison.OrdinalIgnoreCase) >= 0)
            safe = safe.Replace("mspace=core", "mspace=" + AdminManagementSpace_cl.SpaceHome);

        return safe;
    }

    private static void RedirectDenied(Page page, string message, string fallbackUrl)
    {
        HttpContext currentContext = HttpContext.Current;
        HttpResponse response = page != null ? page.Response : (currentContext != null ? currentContext.Response : null);
        if (page != null && page.Session != null)
            page.Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                message,
                "1800",
                "warning");
        else if (currentContext != null && currentContext.Session != null)
            currentContext.Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                message,
                "1800",
                "warning");

        if (response == null)
            return;

        string safeTarget = NormalizeFallbackUrl("", fallbackUrl);
        if (string.IsNullOrWhiteSpace(safeTarget))
            safeTarget = "/admin/default.aspx";
        response.Redirect(safeTarget, false);

        if (currentContext != null && currentContext.ApplicationInstance != null)
            currentContext.ApplicationInstance.CompleteRequest();
    }

    private static bool TryRedirectCanonicalQuery(Page page, HttpRequest request)
    {
        if (request == null)
            return false;

        string path = request.Url != null ? (request.Url.AbsolutePath ?? "") : "";
        if (string.Equals(path, "/admin/default.aspx", StringComparison.OrdinalIgnoreCase))
        {
            string mspace = (request.QueryString["mspace"] ?? "").Trim();
            if (string.Equals(mspace, "core", StringComparison.OrdinalIgnoreCase))
            {
                string target = "/admin/default.aspx?mspace=" + HttpUtility.UrlEncode(AdminManagementSpace_cl.SpaceHome);
                HttpContext mspaceContext = HttpContext.Current;
                HttpResponse mspaceResponse = page != null ? page.Response : (mspaceContext != null ? mspaceContext.Response : null);
                if (mspaceResponse != null)
                {
                    mspaceResponse.Redirect(target, false);
                    if (mspaceContext != null && mspaceContext.ApplicationInstance != null)
                        mspaceContext.ApplicationInstance.CompleteRequest();
                    return true;
                }
            }
        }

        string canonicalUrl;
        if (!AdminDataScope_cl.TryBuildCanonicalAdminQueryUrl(request, out canonicalUrl))
            return false;

        HttpContext currentContext = HttpContext.Current;
        HttpResponse response = page != null ? page.Response : (currentContext != null ? currentContext.Response : null);
        if (response == null || string.IsNullOrWhiteSpace(canonicalUrl))
            return false;

        response.Redirect(canonicalUrl, false);
        if (currentContext != null && currentContext.ApplicationInstance != null)
            currentContext.ApplicationInstance.CompleteRequest();
        return true;
    }

    private static bool IsGuardableAdminRequest(HttpRequest request)
    {
        if (request == null)
            return false;

        string path = (request.AppRelativeCurrentExecutionFilePath ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (path == "~/admin/login.aspx")
            return false;
        if (path == "~/admin/quen-mat-khau/default.aspx")
            return false;
        if (path == "~/admin/khoi-phuc-mat-khau.aspx")
            return false;
        if (path == "~/admin/api/usdt-bridge-credit.aspx")
            return false;

        return path.StartsWith("~/admin/", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildDeniedMessage(string featureKey)
    {
        AdminFeatureRegistry_cl.FeatureDefinition definition = AdminFeatureRegistry_cl.Get(featureKey);
        if (definition == null || string.IsNullOrWhiteSpace(definition.Title))
            return "Bạn không có quyền truy cập chức năng quản trị này.";

        return "Bạn không có quyền truy cập chức năng \"" + definition.Title + "\" trong không gian quản trị hiện tại.";
    }

    private static string BuildUnknownRouteMessage()
    {
        return "Route quản trị này chưa được map quyền rõ ràng. Vui lòng liên hệ Super Admin để cập nhật policy trước khi sử dụng.";
    }

    private static bool HasFeatureAccess(dbDataContext db, taikhoan_tb account, string featureKey)
    {
        if (db == null || account == null || string.IsNullOrWhiteSpace(featureKey))
            return false;

        if (string.Equals((featureKey ?? "").Trim(), "change_password", StringComparison.OrdinalIgnoreCase))
            return true;

        string tk = (account.taikhoan ?? "").Trim();
        if (AdminRolePolicy_cl.IsSuperAdmin(tk))
            return true;

        string requiredGroup = AdminFeatureRegistry_cl.ResolveAccessGroupKey(featureKey);
        if (string.IsNullOrWhiteSpace(requiredGroup))
            return false;

        HashSet<string> allowedGroups = AdminRolePolicy_cl.BuildAdminHomeAccessGroupSet(
            db,
            account.taikhoan,
            account.phanloai ?? "",
            account.permission ?? "");

        return allowedGroups.Contains(requiredGroup);
    }

    public static bool CanCurrentAdminAccessFeature(string featureKey)
    {
        string normalizedFeatureKey = (featureKey ?? "").Trim();
        if (normalizedFeatureKey == "")
            return false;

        string tk = AdminRolePolicy_cl.GetCurrentAdminUser();
        if (string.IsNullOrWhiteSpace(tk))
            return false;

        bool cachedAllowed;
        if (TryGetCachedAccessDecision(tk, normalizedFeatureKey, out cachedAllowed))
            return cachedAllowed;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (account == null)
                return false;

            bool allowed = HasFeatureAccess(db, account, normalizedFeatureKey);
            CacheAccessDecision(tk, normalizedFeatureKey, allowed);
            return allowed;
        }
    }

    public static bool EnsurePageAccess(Page page)
    {
        if (page == null || page.Request == null)
            return true;

        HttpRequest request = page.Request;
        if (!IsGuardableAdminRequest(request))
            return true;

        if (TryRedirectCanonicalQuery(page, request))
            return false;

        string featureKey = AdminRouteMap_cl.ResolveFeatureKey(request);
        string tk = AdminRolePolicy_cl.GetCurrentAdminUser();
        if (string.IsNullOrWhiteSpace(tk))
            return true;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (account == null)
                return true;

            if (string.IsNullOrWhiteSpace(featureKey))
            {
                if (AdminRolePolicy_cl.IsSuperAdmin((account.taikhoan ?? "").Trim()))
                    return true;

                string unknownRouteFallbackUrl = AdminManagementSpace_cl.ResolveCurrentLandingUrl(db, account, request);
                RedirectDenied(page, BuildUnknownRouteMessage(), NormalizeFallbackUrl("", unknownRouteFallbackUrl));
                return false;
            }

            bool allowed;
            if (!TryGetCachedAccessDecision(tk, featureKey, out allowed))
            {
                allowed = HasFeatureAccess(db, account, featureKey);
                CacheAccessDecision(tk, featureKey, allowed);
            }

            if (allowed)
                return true;

            string fallbackUrl = AdminManagementSpace_cl.ResolveCurrentLandingUrl(db, account, request);
            RedirectDenied(page, BuildDeniedMessage(featureKey), NormalizeFallbackUrl(featureKey, fallbackUrl));
            return false;
        }
    }

    public static void RequireFeatureAccess(string featureKey, string fallbackUrl)
    {
        string normalizedFeatureKey = (featureKey ?? "").Trim();
        if (normalizedFeatureKey == "")
            return;

        check_login_cl.check_login_admin("none", "none");

        HttpContext context = HttpContext.Current;
        if (context != null && TryRedirectCanonicalQuery(context.Handler as Page, context.Request))
            return;

        string tk = AdminRolePolicy_cl.GetCurrentAdminUser();
        if (string.IsNullOrWhiteSpace(tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (account == null)
                return;

            bool allowed;
            if (!TryGetCachedAccessDecision(tk, normalizedFeatureKey, out allowed))
            {
                allowed = HasFeatureAccess(db, account, normalizedFeatureKey);
                CacheAccessDecision(tk, normalizedFeatureKey, allowed);
            }

            if (allowed)
                return;

            RedirectDenied(
                HttpContext.Current != null ? HttpContext.Current.Handler as Page : null,
                BuildDeniedMessage(normalizedFeatureKey),
                NormalizeFallbackUrl(normalizedFeatureKey, fallbackUrl));
        }
    }
}
