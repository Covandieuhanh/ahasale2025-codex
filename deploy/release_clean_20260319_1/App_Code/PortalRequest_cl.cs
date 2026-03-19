using System;
using System.Web;

public static class PortalRequest_cl
{
    private const string CurrentAccountEncryptedKey = "__aha_current_account_enc";
    private const string CurrentAccountKey = "__aha_current_account";
    private const string IsShopPortalKey = "__aha_is_shop_portal";

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

        if (HttpContext.Current != null && HttpContext.Current.Items != null)
        {
            object cached = HttpContext.Current.Items[IsShopPortalKey];
            if (cached is bool)
                return (bool)cached;
        }

        string marker = (req.QueryString["shop_portal"] ?? "").Trim();
        if (string.Equals(marker, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(marker, "true", StringComparison.OrdinalIgnoreCase))
        {
            if (HttpContext.Current != null && HttpContext.Current.Items != null)
                HttpContext.Current.Items[IsShopPortalKey] = true;
            return true;
        }

        string path = (req.Url == null ? "" : (req.Url.AbsolutePath ?? "")).Trim();
        bool isShop = path.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase);
        if (HttpContext.Current != null && HttpContext.Current.Items != null)
            HttpContext.Current.Items[IsShopPortalKey] = isShop;
        return isShop;
    }

    public static string GetCurrentAccountEncrypted()
    {
        if (HttpContext.Current != null && HttpContext.Current.Items != null)
        {
            object cachedEnc = HttpContext.Current.Items[CurrentAccountEncryptedKey];
            if (cachedEnc is string)
                return (string)cachedEnc;
        }

        if (IsShopPortalRequest())
        {
            if (!PortalActiveMode_cl.IsShopActive())
            {
                CacheEnc("");
                return "";
            }

            string tkShop = ReadSession("taikhoan_shop");
            if (!string.IsNullOrEmpty(tkShop))
            {
                CacheEnc(tkShop);
                return tkShop;
            }

            tkShop = ReadCookieValue("cookie_userinfo_shop_bcorn", "taikhoan");
            if (!string.IsNullOrEmpty(tkShop))
            {
                CacheEnc(tkShop);
                return tkShop;
            }
            CacheEnc("");
            return "";
        }

        if (!PortalActiveMode_cl.IsHomeActive())
        {
            CacheEnc("");
            return "";
        }

        string tkHome = ReadSession("taikhoan_home");
        if (!string.IsNullOrEmpty(tkHome))
        {
            CacheEnc(tkHome);
            return tkHome;
        }

        string tkCookie = ReadCookieValue("cookie_userinfo_home_bcorn", "taikhoan");
        CacheEnc(tkCookie);
        return tkCookie;
    }

    public static string GetCurrentAccount()
    {
        if (HttpContext.Current != null && HttpContext.Current.Items != null)
        {
            object cached = HttpContext.Current.Items[CurrentAccountKey];
            if (cached is string)
                return (string)cached;
        }

        string tkEnc = GetCurrentAccountEncrypted();
        if (string.IsNullOrEmpty(tkEnc))
        {
            CacheAccount("");
            return "";
        }

        try
        {
            string tk = mahoa_cl.giaima_Bcorn(tkEnc) ?? "";
            CacheAccount(tk);
            return tk;
        }
        catch
        {
            CacheAccount("");
            return "";
        }
    }

    private static void CacheEnc(string value)
    {
        if (HttpContext.Current != null && HttpContext.Current.Items != null)
            HttpContext.Current.Items[CurrentAccountEncryptedKey] = value ?? "";
    }

    private static void CacheAccount(string value)
    {
        if (HttpContext.Current != null && HttpContext.Current.Items != null)
            HttpContext.Current.Items[CurrentAccountKey] = value ?? "";
    }
}
