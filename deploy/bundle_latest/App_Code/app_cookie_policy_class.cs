using System;
using System.Configuration;
using System.Reflection;
using System.Web;

public static class app_cookie_policy_class
{
    public const string admin_user_cookie = "save_user_admin_aka_1";
    public const string admin_pass_cookie = "save_pass_admin_aka_1";
    public const string admin_return_url_cookie = "save_url_admin_aka_1";

    public const string home_user_cookie = "save_sdt_home_aka";
    public const string home_pass_cookie = "save_pass_home_aka";
    public const string home_return_url_cookie = "save_url_home_aka";

    public const string shop_user_cookie = "save_sdt_shop_aka";
    public const string shop_pass_cookie = "save_pass_shop_aka";
    public const string shop_return_url_cookie = "save_url_shop_aka";

    public static void persist_cookie(HttpContext context, string name, string value, int ttl_days)
    {
        if (context == null || context.Response == null || string.IsNullOrWhiteSpace(name))
            return;

        HttpCookie cookie = new HttpCookie(name);
        cookie.Value = value ?? "";
        cookie.Expires = DateTime.Now.AddDays(ttl_days);
        cookie.Path = "/";

        apply_policy(cookie, context.Request);
        context.Response.Cookies.Add(cookie);
    }

    public static void expire_cookie(HttpContext context, string name)
    {
        if (context == null || context.Response == null || string.IsNullOrWhiteSpace(name))
            return;

        HttpCookie cookie = new HttpCookie(name);
        cookie.Value = "";
        cookie.Expires = DateTime.Now.AddDays(-1);
        cookie.Path = "/";

        apply_policy(cookie, context.Request);
        context.Response.Cookies.Add(cookie);
    }

    private static void apply_policy(HttpCookie cookie, HttpRequest request)
    {
        if (cookie == null)
            return;

        cookie.HttpOnly = true;
        cookie.Secure = should_use_secure_cookie(request);
        try_set_same_site(cookie, read_same_site_mode());
    }

    private static bool should_use_secure_cookie(HttpRequest request)
    {
        string force_secure = ConfigurationManager.AppSettings["ForceSecureCookies"];
        if (!string.IsNullOrWhiteSpace(force_secure))
        {
            if (force_secure.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (force_secure.Trim().Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (request == null)
            return true;

        if (request.IsSecureConnection)
            return true;

        string forwarded_proto = request.Headers["X-Forwarded-Proto"];
        if (!string.IsNullOrWhiteSpace(forwarded_proto) &&
            forwarded_proto.IndexOf("https", StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        return false;
    }

    private static string read_same_site_mode()
    {
        string same_site = ConfigurationManager.AppSettings["CookieSameSiteMode"];
        if (string.IsNullOrWhiteSpace(same_site))
            return "Lax";

        same_site = same_site.Trim();
        if (same_site.Equals("Strict", StringComparison.OrdinalIgnoreCase) ||
            same_site.Equals("Lax", StringComparison.OrdinalIgnoreCase) ||
            same_site.Equals("None", StringComparison.OrdinalIgnoreCase))
            return same_site;

        return "Lax";
    }

    private static void try_set_same_site(HttpCookie cookie, string same_site_mode)
    {
        PropertyInfo prop = typeof(HttpCookie).GetProperty("SameSite");
        if (prop == null || prop.CanWrite == false)
            return;

        try
        {
            object enum_value = Enum.Parse(prop.PropertyType, same_site_mode, true);
            prop.SetValue(cookie, enum_value, null);
        }
        catch
        {
        }
    }
}
