using System;
using System.Web;

public static class GianHangAuth_cl
{
    public static string ResolveShopAccount()
    {
        string tk = "";
        string encodedSession = HttpContext.Current.Session["taikhoan_shop"] as string;
        if (!string.IsNullOrEmpty(encodedSession))
        {
            tk = mahoa_cl.giaima_Bcorn(encodedSession);
        }
        else
        {
            HttpCookie ck = HttpContext.Current.Request.Cookies["cookie_userinfo_shop_bcorn"];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
        }

        return (tk ?? "").Trim().ToLowerInvariant();
    }
}
