using System;
using System.Web;

public static class PortalActiveMode_cl
{
    public const string ModeHome = "home";
    public const string ModeShop = "shop";

    private const string SessionKey = "portal_active_mode";
    private const string CookieName = "cookie_portal_active_mode_bcorn";

    private static string Normalize(string raw)
    {
        string s = (raw ?? "").Trim().ToLowerInvariant();
        if (s == ModeShop) return ModeShop;
        return ModeHome;
    }

    private static string ReadSession(string key)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null || string.IsNullOrEmpty(key))
            return "";

        object raw = ctx.Session[key];
        return raw == null ? "" : (raw.ToString() ?? "");
    }

    private static string ReadCookie(string cookieName, string key)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Request == null || string.IsNullOrEmpty(cookieName) || string.IsNullOrEmpty(key))
            return "";

        HttpCookie ck = ctx.Request.Cookies[cookieName];
        if (ck == null)
            return "";

        return ck[key] ?? "";
    }

    public static bool HasHomeCredential()
    {
        return !string.IsNullOrEmpty(ReadSession("taikhoan_home"))
            || !string.IsNullOrEmpty(ReadCookie("cookie_userinfo_home_bcorn", "taikhoan"));
    }

    public static bool HasShopCredential()
    {
        return !string.IsNullOrEmpty(ReadSession("taikhoan_shop"))
            || !string.IsNullOrEmpty(ReadCookie("cookie_userinfo_shop_bcorn", "taikhoan"));
    }

    public static string GetCurrentMode()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null)
            return ModeHome;

        try
        {
            if (ctx.Session != null)
            {
                string rawSession = (ctx.Session[SessionKey] as string ?? "").Trim();
                if (!string.IsNullOrEmpty(rawSession))
                    return Normalize(rawSession);
            }
        }
        catch
        {
        }

        HttpCookie ck = ctx.Request == null ? null : ctx.Request.Cookies[CookieName];
        if (ck != null)
        {
            string rawCookie = (ck["mode"] ?? "").Trim();
            if (!string.IsNullOrEmpty(rawCookie))
                return Normalize(rawCookie);
        }

        return ModeHome;
    }

    public static bool IsHomeActive()
    {
        return string.Equals(GetCurrentMode(), ModeHome, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsShopActive()
    {
        return string.Equals(GetCurrentMode(), ModeShop, StringComparison.OrdinalIgnoreCase);
    }

    public static void SetMode(string mode)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null)
            return;

        string normalized = Normalize(mode);

        try
        {
            if (ctx.Session != null)
                ctx.Session[SessionKey] = normalized;
        }
        catch
        {
        }

        HttpCookie ck = new HttpCookie(CookieName);
        ck["mode"] = normalized;
        ck.Expires = AhaTime_cl.Now.AddDays(30);
        ck.HttpOnly = true;
        ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(ctx.Request);
        ctx.Response.Cookies.Add(ck);
    }
}
