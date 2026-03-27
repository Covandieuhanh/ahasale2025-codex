<%@ WebHandler Language="C#" Class="manifest_shop" %>

using System;
using System.Linq;
using System.Text;
using System.Web;

public class manifest_shop : IHttpHandler
{
    private static string NormalizeIconPath(string rawPath)
    {
        return PortalBranding_cl.NormalizeIconPath(rawPath, PortalBranding_cl.DefaultShopIconPath);
    }

    private static string ResolveIconMime(string iconPath)
    {
        string lower = (iconPath ?? "").Trim().ToLowerInvariant();
        if (lower.EndsWith(".jpg") || lower.EndsWith(".jpeg"))
            return "image/jpeg";
        if (lower.EndsWith(".png"))
            return "image/png";
        if (lower.EndsWith(".webp"))
            return "image/webp";
        if (lower.EndsWith(".svg"))
            return "image/svg+xml";
        if (lower.EndsWith(".gif"))
            return "image/gif";
        return "image/png";
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/manifest+json";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        string iconPath = PortalBranding_cl.DefaultShopIconPath;
        string appName = "AhaSale Shop";
        string shortName = "AhaSale Shop";
        string themeColor = "#ff5b2e";

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.CaiDatChung_tbs
                    .Where(p => p.phanloai_trang == "shop")
                    .Select(p => new { p.thongtin_apple_touch_icon, p.thongtin_icon, p.thongtin_logo, p.thongtin_logo1 })
                    .FirstOrDefault();

                if (q == null)
                {
                    q = db.CaiDatChung_tbs
                        .Where(p => p.phanloai_trang == "home")
                        .Select(p => new { p.thongtin_apple_touch_icon, p.thongtin_icon, p.thongtin_logo, p.thongtin_logo1 })
                        .FirstOrDefault();
                }

                if (q != null)
                    iconPath = NormalizeIconPath(PortalBranding_cl.ResolveHeaderLogoPath(
                        new PortalBranding_cl.ScopeBrandingSnapshot
                        {
                            AppleTouchIconPath = q.thongtin_apple_touch_icon,
                            IconPath = q.thongtin_icon,
                            LogoPath = q.thongtin_logo,
                            LogoAltPath = q.thongtin_logo1
                        },
                        PortalBranding_cl.ScopeShop));
                else
                    iconPath = NormalizeIconPath(iconPath);
            }
        }
        catch
        {
            iconPath = NormalizeIconPath(iconPath);
        }

        string iconType = ResolveIconMime(iconPath);

        string json = string.Format(
            "{{\"name\":\"{0}\",\"short_name\":\"{1}\",\"description\":\"Gian hàng đối tác AhaSale\",\"start_url\":\"/shop/\",\"scope\":\"/shop/\",\"display\":\"standalone\",\"background_color\":\"#ffffff\",\"theme_color\":\"{2}\",\"icons\":[{{\"src\":\"{3}\",\"sizes\":\"192x192\",\"type\":\"{4}\",\"purpose\":\"any maskable\"}},{{\"src\":\"{3}\",\"sizes\":\"512x512\",\"type\":\"{4}\",\"purpose\":\"any maskable\"}}]}}",
            HttpUtility.JavaScriptStringEncode(appName),
            HttpUtility.JavaScriptStringEncode(shortName),
            themeColor,
            iconPath,
            iconType
        );

        context.Response.Write(json);
    }

    public bool IsReusable
    {
        get { return true; }
    }
}
