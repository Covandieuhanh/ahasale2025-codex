using System;
using System.Web;

public static class PortalRequest_cl
{
    private static HttpRequest CurrentRequest()
    {
        return (HttpContext.Current == null) ? null : HttpContext.Current.Request;
    }

    private static string ReadSession(string key)
    {
        if (HttpContext.Current == null || HttpContext.Current.Session == null || string.IsNullOrEmpty(key))
            return "";

        object raw = HttpContext.Current.Session[key];
        return raw == null ? "" : (raw.ToString() ?? "");
    }

    private static string ReadCookieValue(string cookieName, string key)
    {
        HttpRequest req = CurrentRequest();
        if (req == null || string.IsNullOrEmpty(cookieName) || string.IsNullOrEmpty(key))
            return "";

        HttpCookie ck = req.Cookies[cookieName];
        if (ck == null)
            return "";

        return ck[key] ?? "";
    }

    public static bool IsShopPortalRequest()
    {
        HttpRequest req = CurrentRequest();
        if (req == null)
            return false;

        string marker = (req.QueryString["shop_portal"] ?? "").Trim();
        if (string.Equals(marker, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(marker, "true", StringComparison.OrdinalIgnoreCase))
            return true;

        string path = (req.Url == null ? "" : (req.Url.AbsolutePath ?? "")).Trim();
        return path.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetCurrentAccountEncrypted()
    {
        if (IsShopPortalRequest())
        {
            if (!PortalActiveMode_cl.IsShopActive())
                return "";

            string tkShop = ReadSession("taikhoan_shop");
            if (!string.IsNullOrEmpty(tkShop))
                return tkShop;

            tkShop = ReadCookieValue("cookie_userinfo_shop_bcorn", "taikhoan");
            if (!string.IsNullOrEmpty(tkShop))
                return tkShop;
            return "";
        }

        if (!PortalActiveMode_cl.IsHomeActive())
            return "";

        string tkHome = ReadSession("taikhoan_home");
        if (!string.IsNullOrEmpty(tkHome))
            return tkHome;

        return ReadCookieValue("cookie_userinfo_home_bcorn", "taikhoan");
    }

    public static string GetCurrentAccount()
    {
        string tkEnc = GetCurrentAccountEncrypted();
        if (string.IsNullOrEmpty(tkEnc))
            return "";

        try
        {
            return mahoa_cl.giaima_Bcorn(tkEnc) ?? "";
        }
        catch
        {
            return "";
        }
    }
}
