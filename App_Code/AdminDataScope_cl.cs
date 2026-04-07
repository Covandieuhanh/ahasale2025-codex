using System;
using System.Collections.Generic;
using System.Web;

public static class AdminDataScope_cl
{
    public const string AccountScopeAdmin = "admin";
    public const string AccountScopeHome = "home";
    public const string AccountScopeShop = "shop";

    public const string TransferTabCustomer = "uu-dai";
    public const string TransferTabDevelopment = "lao-dong";
    public const string TransferTabEcosystem = "gan-ket";
    public const string TransferTabShopOnly = "shop-only";
    public const string TransferTabCore = "tieu-dung";

    public static string NormalizeAccountScope(string scopeRaw)
    {
        string scope = (scopeRaw ?? "").Trim().ToLowerInvariant();
        if (scope == PortalScope_cl.ScopeAdmin)
            return AccountScopeAdmin;
        if (scope == PortalScope_cl.ScopeHome)
            return AccountScopeHome;
        if (scope == PortalScope_cl.ScopeShop)
            return AccountScopeShop;

        if (scope == AccountScopeAdmin
            || scope == AccountScopeHome
            || scope == AccountScopeShop)
            return scope;
        return "";
    }

    public static string ResolveAccountFeatureByScope(string scopeRaw)
    {
        string scope = NormalizeAccountScope(scopeRaw);
        if (scope == AccountScopeHome)
            return "home_accounts";
        if (scope == AccountScopeShop)
            return "shop_accounts";
        return "admin_accounts";
    }

    public static string ResolveEffectiveAccountScope(string scopeRaw, string filterScopeRaw)
    {
        string scope = NormalizeAccountScope(scopeRaw);
        if (!string.IsNullOrWhiteSpace(scope))
            return scope;

        return NormalizeAccountScope(filterScopeRaw);
    }

    public static string NormalizeTransferTab(string tabRaw)
    {
        string tab = (tabRaw ?? "").Trim().ToLowerInvariant();
        if (tab == TransferTabCustomer
            || tab == TransferTabDevelopment
            || tab == TransferTabEcosystem
            || tab == TransferTabShopOnly)
            return tab;
        return "";
    }

    public static string NormalizeTransferTabExtended(string tabRaw)
    {
        string tab = (tabRaw ?? "").Trim().ToLowerInvariant();
        if (tab == TransferTabCore)
            return TransferTabCore;

        return NormalizeTransferTab(tab);
    }

    public static string ResolveTransferFeatureByTab(string tabRaw)
    {
        string tab = NormalizeTransferTab(tabRaw);
        if (tab == TransferTabShopOnly)
            return "shop_points";
        if (tab == TransferTabCustomer
            || tab == TransferTabDevelopment
            || tab == TransferTabEcosystem)
            return "home_point_approval";
        return "core_assets";
    }

    private static bool IsAccountScopeRoute(string path)
    {
        return (path ?? "").StartsWith("/admin/quan-ly-tai-khoan/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTransferTabRoute(string path)
    {
        return (path ?? "").StartsWith("/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsOtpScopeRoute(string path)
    {
        return (path ?? "").StartsWith("/admin/otp/", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildCanonicalQueryString(HttpRequest request, IDictionary<string, string> overrides)
    {
        if (request == null)
            return "";

        var pairs = new List<KeyValuePair<string, string>>();
        var handledKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string keyObj in request.QueryString.AllKeys)
        {
            string key = keyObj ?? "";
            if (string.IsNullOrWhiteSpace(key))
                continue;

            if (overrides != null && overrides.ContainsKey(key))
            {
                if (!handledKeys.Contains(key))
                {
                    pairs.Add(new KeyValuePair<string, string>(key, overrides[key] ?? ""));
                    handledKeys.Add(key);
                }
                continue;
            }

            string[] values = request.QueryString.GetValues(key);
            if (values == null || values.Length == 0)
            {
                pairs.Add(new KeyValuePair<string, string>(key, ""));
                continue;
            }

            for (int i = 0; i < values.Length; i++)
                pairs.Add(new KeyValuePair<string, string>(key, values[i] ?? ""));
        }

        if (overrides != null)
        {
            foreach (var entry in overrides)
            {
                if (string.IsNullOrWhiteSpace(entry.Key))
                    continue;
                if (handledKeys.Contains(entry.Key))
                    continue;

                pairs.Add(new KeyValuePair<string, string>(entry.Key, entry.Value ?? ""));
            }
        }

        var encoded = new List<string>();
        foreach (var pair in pairs)
            encoded.Add(HttpUtility.UrlEncode(pair.Key) + "=" + HttpUtility.UrlEncode(pair.Value ?? ""));

        return string.Join("&", encoded.ToArray());
    }

    public static bool TryBuildCanonicalAdminQueryUrl(HttpRequest request, out string canonicalUrl)
    {
        canonicalUrl = "";
        if (request == null || request.Url == null)
            return false;

        string path = (request.Url.AbsolutePath ?? "").Trim();
        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (IsAccountScopeRoute(path))
        {
            string rawScope = (request.QueryString["scope"] ?? "").Trim();
            string rawFilterScope = (request.QueryString["fscope"] ?? "").Trim();
            string normalizedScope = NormalizeAccountScope(rawScope);
            string normalizedFilterScope = NormalizeAccountScope(rawFilterScope);
            string effective = ResolveEffectiveAccountScope(rawScope, rawFilterScope);

            if (string.IsNullOrWhiteSpace(effective))
                return false;

            bool needsNormalize =
                !string.Equals(normalizedScope, effective, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(normalizedFilterScope, effective, StringComparison.OrdinalIgnoreCase);

            if (!needsNormalize)
                return false;

            var overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            overrides["scope"] = effective;
            overrides["fscope"] = effective;

            string query = BuildCanonicalQueryString(request, overrides);
            canonicalUrl = path + (string.IsNullOrWhiteSpace(query) ? "" : "?" + query);
            return true;
        }

        if (IsTransferTabRoute(path))
        {
            string rawTab = (request.QueryString["tab"] ?? "").Trim();
            string normalizedTab = NormalizeTransferTabExtended(rawTab);
            if (string.IsNullOrWhiteSpace(normalizedTab))
                normalizedTab = TransferTabCore;

            if (string.Equals(rawTab, normalizedTab, StringComparison.OrdinalIgnoreCase))
                return false;

            var overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            overrides["tab"] = normalizedTab;

            string query = BuildCanonicalQueryString(request, overrides);
            canonicalUrl = path + (string.IsNullOrWhiteSpace(query) ? "" : "?" + query);
            return true;
        }

        if (IsOtpScopeRoute(path))
        {
            string rawScope = (request.QueryString["scope"] ?? "").Trim();
            string normalizedScope = NormalizeAccountScope(rawScope);
            string effective = string.Equals(normalizedScope, AccountScopeShop, StringComparison.OrdinalIgnoreCase)
                ? AccountScopeShop
                : AccountScopeHome;

            if (string.Equals(rawScope, effective, StringComparison.OrdinalIgnoreCase))
                return false;

            var overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            overrides["scope"] = effective;

            string query = BuildCanonicalQueryString(request, overrides);
            canonicalUrl = path + (string.IsNullOrWhiteSpace(query) ? "" : "?" + query);
            return true;
        }

        return false;
    }
}
