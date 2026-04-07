using System;
using System.Web;

public static class WorkspaceContext_cl
{
    private const string SessionKey = "workspace_context_current";

    public sealed class WorkspaceSessionContext
    {
        public string WorkspaceKey { get; set; }
        public string OwnerAccountKey { get; set; }
        public string HomeAccountKey { get; set; }
        public string LegacyUser { get; set; }
        public string RoleLabel { get; set; }
        public string Source { get; set; }
        public string ChiNhanhId { get; set; }
        public string NganhId { get; set; }
        public bool IsManagedWorkspace { get; set; }
        public bool IsSystemAdminMode { get; set; }
    }

    public static WorkspaceSessionContext GetCurrent()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return null;

        return ctx.Session[SessionKey] as WorkspaceSessionContext;
    }

    public static void SetCurrent(WorkspaceSessionContext context)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return;

        if (context == null)
        {
            ctx.Session.Remove(SessionKey);
            return;
        }

        ctx.Session[SessionKey] = context;
    }

    public static void ClearCurrent()
    {
        SetCurrent(null);
    }

    public static bool IsCurrentWorkspace(string workspaceKey)
    {
        WorkspaceSessionContext context = GetCurrent();
        if (context == null)
            return false;

        return string.Equals(Normalize(context.WorkspaceKey), Normalize(workspaceKey), StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsSystemAdminMode(HttpRequest request)
    {
        if (request != null)
        {
            string queryValue = (request.QueryString["system"] ?? "").Trim();
            if (string.Equals(queryValue, "1", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        WorkspaceSessionContext context = GetCurrent();
        return context != null && context.IsSystemAdminMode;
    }

    public static string AppendSystemAdminFlag(string url, HttpRequest request)
    {
        string safeUrl = (url ?? "").Trim();
        if (safeUrl == "")
            return "";

        if (!IsSystemAdminMode(request))
            return safeUrl;

        if (safeUrl.IndexOf("system=", StringComparison.OrdinalIgnoreCase) >= 0)
            return safeUrl;

        string separator = safeUrl.Contains("?") ? "&" : "?";
        return safeUrl + separator + "system=1";
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
