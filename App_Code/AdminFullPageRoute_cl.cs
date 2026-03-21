using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;

public static class AdminFullPageRoute_cl
{
    public const string TransferMarkerKey = "__admin_fullpage_transfer";

    public static string BuildTargetUrl(HttpRequest request, string targetPath, IDictionary<string, string> overrides, params string[] keepKeys)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (request != null && keepKeys != null)
        {
            foreach (string key in keepKeys)
            {
                string safeKey = (key ?? "").Trim();
                if (safeKey == "")
                    continue;

                string value = (request.QueryString[safeKey] ?? "").Trim();
                if (value != "")
                    query[safeKey] = value;
            }
        }

        if (overrides != null)
        {
            foreach (KeyValuePair<string, string> pair in overrides)
            {
                string key = (pair.Key ?? "").Trim();
                if (key == "")
                    continue;

                string value = (pair.Value ?? "").Trim();
                if (value == "")
                    query.Remove(key);
                else
                    query[key] = value;
            }
        }

        string basePath = targetPath ?? "";
        string queryText = query.ToString();
        return queryText == "" ? basePath : basePath + "?" + queryText;
    }

    public static void Transfer(Page page, string targetUrl)
    {
        if (page == null)
            return;

        HttpContext context = HttpContext.Current;
        if (context != null)
            context.Items[TransferMarkerKey] = true;

        page.Server.Transfer(targetUrl, true);
    }

    public static bool IsTransferredRequest(HttpContext context)
    {
        if (context == null || context.Items == null)
            return false;

        object marker = context.Items[TransferMarkerKey];
        return marker is bool && (bool)marker;
    }

    public static string SanitizeAdminReturnUrl(string rawUrl, string fallbackUrl)
    {
        string fallback = string.IsNullOrWhiteSpace(fallbackUrl) ? "/admin/default.aspx" : fallbackUrl.Trim();
        string raw = (rawUrl ?? "").Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return fallback;

        string decoded = HttpUtility.UrlDecode(raw) ?? "";
        if (string.IsNullOrWhiteSpace(decoded))
            return fallback;

        if (!decoded.StartsWith("/", StringComparison.Ordinal) || decoded.StartsWith("//", StringComparison.Ordinal))
            return fallback;

        string path = decoded;
        string queryText = "";
        int queryIndex = decoded.IndexOf('?');
        if (queryIndex >= 0)
        {
            path = decoded.Substring(0, queryIndex);
            queryText = queryIndex < decoded.Length - 1 ? decoded.Substring(queryIndex + 1) : "";
        }

        if (!path.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return fallback;

        if (path.Equals("/admin/login.aspx", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/admin/doi-mat-khau/default.aspx", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/admin/quen-mat-khau/default.aspx", StringComparison.OrdinalIgnoreCase))
            return fallback;

        NameValueCollection query = HttpUtility.ParseQueryString(queryText);
        query.Remove("topview");
        query.Remove("return_url");

        string legacyView = (query["view"] ?? "").Trim().ToLowerInvariant();
        if (legacyView == "recover")
            query.Remove("view");

        string cleanQuery = query.ToString();
        return string.IsNullOrWhiteSpace(cleanQuery) ? path : path + "?" + cleanQuery;
    }

    public static bool HasLegacyTopView(HttpRequest request)
    {
        if (request == null)
            return false;

        return !string.IsNullOrWhiteSpace((request.QueryString["topview"] ?? "").Trim());
    }
}
