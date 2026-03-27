using System;
using System.Web;

public static class GianHangAuth_cl
{
    public static string ResolveShopAccount()
    {
        HttpContext context = HttpContext.Current;
        if (context == null)
            return "";

        return ResolveRootAccountForGianHang(context);
    }

    private static string ResolveRootAccountForGianHang(HttpContext context)
    {
        string path = context == null || context.Request == null
            ? string.Empty
            : (context.Request.Path ?? string.Empty).Trim().ToLowerInvariant();
        if (path == string.Empty || !path.StartsWith("/gianhang", StringComparison.Ordinal))
            return string.Empty;

        string rootAccount = (PortalRequest_cl.GetCurrentAccount() ?? string.Empty).Trim().ToLowerInvariant();
        return rootAccount;
    }
}
