using System;
using System.Linq;
using System.Web;

public static class ManagedHomeSpaceGuard_cl
{
    private const string GuardHandledKey = "__aha_managed_home_space_guard_handled";

    public static void TryHandleCurrentRequest(HttpContext ctx)
    {
        if (ctx == null || ctx.Request == null || ctx.Response == null || ctx.ApplicationInstance == null)
            return;

        if (ctx.Items[GuardHandledKey] is bool && (bool)ctx.Items[GuardHandledKey])
            return;

        string rawPath = GetRawPath(ctx);
        if (!IsGuardedRequestPath(rawPath))
            return;

        if (ShouldSkipForAdminSystemRequest(ctx, rawPath))
            return;

        string spaceCode = ResolveSpaceCode(rawPath);
        if (spaceCode == "")
            return;

        string returnUrl = NormalizeReturnUrl(ctx.Request.RawUrl, GetFallbackReturnUrl(spaceCode));
        bool requiresLegacyBootstrap = string.Equals(spaceCode, ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase);

        if (ShouldRememberRequestedReturnUrl(rawPath))
            HomeSpaceAccess_cl.RememberReturnUrl(ctx, returnUrl, GetFallbackReturnUrl(spaceCode));

        ctx.Items[GuardHandledKey] = true;

        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);

            string homeAccountKey = GianHangAdminBridge_cl.ReadHomeAccountFromSessionOrCookie();
            if (string.IsNullOrWhiteSpace(homeAccountKey))
            {
                if (requiresLegacyBootstrap)
                    GianHangAdminBridge_cl.ClearLegacyAdminSession();

                RedirectToHomeLogin(ctx, returnUrl, BuildLoginMessage(spaceCode));
                return;
            }

            taikhoan_tb homeAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == homeAccountKey);
            if (homeAccount == null)
            {
                if (requiresLegacyBootstrap)
                    GianHangAdminBridge_cl.ClearLegacyAdminSession();

                RedirectToHomeLogin(ctx, returnUrl, BuildLoginMessage(spaceCode));
                return;
            }

            if (!requiresLegacyBootstrap)
            {
                if (!SpaceAccess_cl.CanAccessSpace(db, homeAccount, spaceCode))
                {
                    RedirectToAccessPage(ctx, spaceCode, returnUrl);
                    return;
                }
                return;
            }

            string bootstrapHomeAccount;
            string deniedMessage;
            if (GianHangAdminBridge_cl.EnsureLegacyAdminSessionFromCurrentHome(db, out bootstrapHomeAccount, out deniedMessage))
                return;

            if (!string.IsNullOrWhiteSpace(bootstrapHomeAccount))
            {
                RedirectToAccessPage(ctx, spaceCode, returnUrl);
                return;
            }

            RedirectToHomeLogin(ctx, returnUrl, BuildLoginMessage(spaceCode));
        }
    }

    private static string GetRawPath(HttpContext ctx)
    {
        string rawUrl = ctx.Request.RawUrl ?? "";
        string path = rawUrl;
        int queryIndex = path.IndexOf('?');
        if (queryIndex >= 0)
            path = path.Substring(0, queryIndex);

        path = (path ?? "").Trim();
        if (path == "" && ctx.Request.Url != null)
            path = ctx.Request.Url.AbsolutePath ?? "";

        return (path ?? "").Trim().ToLowerInvariant();
    }

    private static bool IsGuardedRequestPath(string rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
            return false;

        string extension = VirtualPathUtility.GetExtension(rawPath) ?? "";
        if (extension != "" && !string.Equals(extension, ".aspx", StringComparison.OrdinalIgnoreCase))
            return false;

        if (rawPath.Equals("/gianhang/admin", StringComparison.OrdinalIgnoreCase))
            return true;
        if (rawPath.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
            return true;
        if (rawPath.Equals("/daugia/admin", StringComparison.OrdinalIgnoreCase))
            return true;
        if (rawPath.StartsWith("/daugia/admin/", StringComparison.OrdinalIgnoreCase))
            return true;
        if (rawPath.Equals("/event/admin", StringComparison.OrdinalIgnoreCase))
            return true;
        if (rawPath.StartsWith("/event/admin/", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static bool ShouldSkipForAdminSystemRequest(HttpContext ctx, string rawPath)
    {
        if (ctx == null || ctx.Request == null)
            return false;

        string normalizedPath = (rawPath ?? "").Trim().ToLowerInvariant();
        if (!IsSystemAdminMode(ctx.Request))
            return false;

        if (normalizedPath.Equals("/daugia/admin", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.StartsWith("/daugia/admin/", StringComparison.OrdinalIgnoreCase))
            return true;

        if (normalizedPath.Equals("/event/admin", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.StartsWith("/event/admin/", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static string ResolveSpaceCode(string rawPath)
    {
        if (rawPath.Equals("/gianhang/admin", StringComparison.OrdinalIgnoreCase)
            || rawPath.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.GianHangAdmin;

        if (rawPath.Equals("/daugia/admin", StringComparison.OrdinalIgnoreCase)
            || rawPath.StartsWith("/daugia/admin/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.DauGia;

        if (rawPath.Equals("/event/admin", StringComparison.OrdinalIgnoreCase)
            || rawPath.StartsWith("/event/admin/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.Event;

        return "";
    }

    private static string GetFallbackReturnUrl(string spaceCode)
    {
        switch (ModuleSpace_cl.Normalize(spaceCode))
        {
            case ModuleSpace_cl.GianHangAdmin:
                return "/gianhang/admin";
            case ModuleSpace_cl.DauGia:
                return "/daugia/admin";
            case ModuleSpace_cl.Event:
                return "/event/admin";
            default:
                return "/home/default.aspx";
        }
    }

    private static string NormalizeReturnUrl(string rawReturnUrl, string fallbackUrl)
    {
        return HomeSpaceAccess_cl.NormalizeReturnUrl(rawReturnUrl, fallbackUrl);
    }

    private static string BuildLoginMessage(string spaceCode)
    {
        switch (ModuleSpace_cl.Normalize(spaceCode))
        {
            case ModuleSpace_cl.GianHangAdmin:
                return "Vui lòng đăng nhập tài khoản Home để truy cập không gian /gianhang/admin.";
            case ModuleSpace_cl.DauGia:
                return "Vui lòng đăng nhập tài khoản Home để truy cập không gian /daugia/admin.";
            case ModuleSpace_cl.Event:
                return "Vui lòng đăng nhập tài khoản Home để truy cập không gian /event/admin.";
            default:
                return "Vui lòng đăng nhập tài khoản Home để tiếp tục.";
        }
    }

    private static void RedirectToAccessPage(HttpContext ctx, string spaceCode, string returnUrl)
    {
        string targetUrl = HomeSpaceAccess_cl.BuildAccessPageUrl(spaceCode, returnUrl);
        ctx.Response.Redirect(targetUrl, false);
        ctx.ApplicationInstance.CompleteRequest();
    }

    private static void RedirectToHomeLogin(HttpContext ctx, string returnUrl, string message)
    {
        HomeSpaceAccess_cl.RememberReturnUrl(ctx, returnUrl, GetFallbackReturnUrl(ResolveSpaceCode(GetRawPath(ctx))));

        if (ctx.Session != null)
            ctx.Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                string.IsNullOrWhiteSpace(message) ? "Vui lòng đăng nhập tài khoản Home để tiếp tục." : message,
                "1500",
                "warning");

        string targetUrl = "/dang-nhap?return_url=" + HttpUtility.UrlEncode(returnUrl);
        ctx.Response.Redirect(targetUrl, false);
        ctx.ApplicationInstance.CompleteRequest();
    }

    private static bool ShouldRememberRequestedReturnUrl(string rawPath)
    {
        string normalized = (rawPath ?? "").Trim().ToLowerInvariant();
        return normalized != "/gianhang/admin/login.aspx"
            && normalized != "/gianhang/admin/f5_ss_admin.aspx"
            && normalized != "/daugia/admin/login.aspx"
            && normalized != "/event/admin/login.aspx";
    }

    private static bool IsSystemAdminMode(HttpRequest request)
    {
        if (request == null)
            return false;

        string raw = (request.QueryString["system"] ?? "").Trim().ToLowerInvariant();
        return raw == "1" || raw == "true";
    }
}
