using System;
using System.Linq;
using System.Web;

public static class GianHangAdminBridge_cl
{
    public static void ClearLegacyAdminSession()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null)
            return;

        if (ctx.Session != null)
        {
            ctx.Session["user"] = "";
            ctx.Session["chinhanh"] = "";
            ctx.Session["nganh"] = "";
            ctx.Session["user_parent"] = "";
            ctx.Session["gianhang_admin_owner"] = "";
            ctx.Session["gianhang_admin_home"] = "";
            ctx.Session["gianhang_admin_mode"] = "";
            ctx.Session["gianhang_admin_role"] = "";
        }

        GianHangAdminWorkspace_cl.ClearWorkspaceSelection();

        if (ctx.Response == null)
            return;

        ExpireCookie(ctx, "save_user_admin_aka_1");
        ExpireCookie(ctx, "save_pass_admin_aka_1");
        ExpireCookie(ctx, "save_url_admin_aka_1");
    }

    public static string ReadHomeAccountFromSessionOrCookie()
    {
        string tkEnc = "";
        HttpContext ctx = HttpContext.Current;
        if (ctx != null && ctx.Session != null)
        {
            string bridgedHome = ((ctx.Session["gianhang_admin_home"] ?? "") + "").Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(bridgedHome))
                return bridgedHome;

            tkEnc = (ctx.Session["taikhoan_home"] as string) ?? "";
        }

        if (tkEnc == "" && ctx != null && ctx.Request != null && ctx.Request.Cookies != null)
        {
            HttpCookie ck = ctx.Request.Cookies["cookie_userinfo_home_bcorn"];
            if (ck != null)
                tkEnc = ReadCookieValue(ck, "taikhoan");
        }

        if (tkEnc == "")
        {
            string portalAccount = PortalRequest_cl.GetCurrentAccount();
            if (portalAccount != "")
                return portalAccount.Trim().ToLowerInvariant();
            return "";
        }

        tkEnc = tkEnc.Replace(" ", "+");

        try
        {
            return (mahoa_cl.giaima_Bcorn(tkEnc) ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            string portalAccount = PortalRequest_cl.GetCurrentAccount();
            if (portalAccount != "")
                return portalAccount.Trim().ToLowerInvariant();
            return "";
        }
    }

    private static string ReadCookieValue(HttpCookie cookie, string key)
    {
        if (cookie == null || string.IsNullOrWhiteSpace(key))
            return "";

        string direct = (cookie[key] ?? "").Trim();
        if (direct != "")
            return direct;

        string raw = (cookie.Value ?? "").Trim();
        if (raw == "")
            return "";

        foreach (string segment in raw.Split('&'))
        {
            if (string.IsNullOrWhiteSpace(segment))
                continue;

            int idx = segment.IndexOf('=');
            if (idx <= 0)
                continue;

            string partKey = (HttpUtility.UrlDecode(segment.Substring(0, idx)) ?? "").Trim();
            if (!string.Equals(partKey, key, StringComparison.OrdinalIgnoreCase))
                continue;

            string partValue = (segment.Substring(idx + 1) ?? "").Trim();
            if (partValue.IndexOf('%') >= 0)
                partValue = Uri.UnescapeDataString(partValue);

            return partValue.Trim();
        }

        return "";
    }

    public static bool TryBootstrapLegacyAdminFromHome(
        dbDataContext db,
        out string homeAccount,
        out string deniedMessage)
    {
        return EnsureLegacyAdminSessionFromCurrentHome(db, out homeAccount, out deniedMessage);
    }

    public static bool EnsureLegacyAdminSessionFromCurrentHome(
        dbDataContext db,
        out string homeAccount,
        out string deniedMessage)
    {
        homeAccount = ReadHomeAccountFromSessionOrCookie();
        deniedMessage = "";

        if (db == null || homeAccount == "")
        {
            ClearLegacyAdminSession();
            return false;
        }

        string currentHomeAccount = homeAccount;
        taikhoan_tb home = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == currentHomeAccount);
        if (home == null)
        {
            ClearLegacyAdminSession();
            return false;
        }

        if (!GianHangAdminWorkspace_cl.HasAnyWorkspace(db, currentHomeAccount)
            && !SpaceAccess_cl.CanAccessGianHangAdmin(db, home))
        {
            deniedMessage = "Bạn chưa được mở quyền hoặc tham gia không gian /gianhang/admin.";
            ClearLegacyAdminSession();
            return false;
        }

        GianHangAdminWorkspace_cl.WorkspaceInfo workspace;
        return GianHangAdminWorkspace_cl.TryBootstrapSelectedWorkspace(db, currentHomeAccount, out workspace, out deniedMessage);
    }

    public static bool IsHomeManagedLegacyAdmin(dbDataContext db, taikhoan_table_2023 adminAcc, out taikhoan_tb homeAccount)
    {
        homeAccount = null;
        if (db == null || adminAcc == null)
            return false;

        string adminUser = (adminAcc.taikhoan ?? "").Trim().ToLowerInvariant();
        if (adminUser == "")
            return false;

        taikhoan_tb linked = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == adminUser);
        if (linked == null)
            return false;

        string scope = PortalScope_cl.ResolveScope(linked.taikhoan, linked.phanloai, linked.permission);
        if (!string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
            return false;

        homeAccount = linked;
        return true;
    }

    private static void ExpireCookie(HttpContext ctx, string cookieName)
    {
        if (ctx == null || ctx.Response == null || string.IsNullOrWhiteSpace(cookieName))
            return;

        HttpCookie cookie = new HttpCookie(cookieName);
        cookie.Expires = DateTime.Now.AddDays(-1);
        cookie.Value = "";
        cookie.Path = "/";
        ctx.Response.Cookies.Set(cookie);
    }
}
