using System;
using System.Web;

public static class GianHangWorkspaceUtility_cl
{
    private const string LegacyAppKey = "CoCau";
    private const string ScopedPrefix = "GianHang.CoCau.";

    public static string NormalizeWorkspaceKey(string workspaceKey)
    {
        return ((workspaceKey ?? string.Empty).Trim().ToLowerInvariant());
    }

    public static string ResolveWorkspaceKey(HttpRequest request)
    {
        string fromQuery = NormalizeWorkspaceKey((request == null ? string.Empty : (request["user"] ?? string.Empty)));
        if (fromQuery != string.Empty)
            return fromQuery;

        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null)
            return string.Empty;

        string fromParent = NormalizeWorkspaceKey((context.Session["user_parent"] ?? string.Empty) + string.Empty);
        if (fromParent != string.Empty)
            return fromParent;

        string fromHome = NormalizeWorkspaceKey((context.Session["taikhoan"] ?? string.Empty) + string.Empty);
        if (fromHome != string.Empty)
            return fromHome;

        return string.Empty;
    }

    public static int? GetCoCauIndex(string workspaceKey)
    {
        HttpApplicationState application = HttpContext.Current == null ? null : HttpContext.Current.Application;
        if (application == null)
            return null;

        string scopedKey = BuildScopedKey(workspaceKey);
        int parsed;
        if (scopedKey != string.Empty && TryParsePositive(application[scopedKey], out parsed))
            return parsed;

        if (TryParsePositive(application[LegacyAppKey], out parsed))
            return parsed;

        return null;
    }

    public static void SetCoCauIndex(string workspaceKey, int index)
    {
        if (index <= 0)
            return;

        HttpApplicationState application = HttpContext.Current == null ? null : HttpContext.Current.Application;
        if (application == null)
            return;

        string scopedKey = BuildScopedKey(workspaceKey);
        if (scopedKey == string.Empty)
            application[LegacyAppKey] = index.ToString();
        else
            application[scopedKey] = index.ToString();
    }

    public static void ClearCoCauIndex(string workspaceKey)
    {
        HttpApplicationState application = HttpContext.Current == null ? null : HttpContext.Current.Application;
        if (application == null)
            return;

        string scopedKey = BuildScopedKey(workspaceKey);
        if (scopedKey == string.Empty)
            application[LegacyAppKey] = null;
        else
            application[scopedKey] = null;
    }

    private static bool TryParsePositive(object raw, out int parsed)
    {
        parsed = 0;
        if (raw == null)
            return false;

        return int.TryParse((raw + string.Empty).Trim(), out parsed) && parsed > 0;
    }

    private static string BuildScopedKey(string workspaceKey)
    {
        string normalized = NormalizeWorkspaceKey(workspaceKey);
        return normalized == string.Empty ? string.Empty : (ScopedPrefix + normalized);
    }
}
