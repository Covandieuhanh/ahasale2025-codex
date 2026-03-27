<%@ WebHandler Language="C#" Class="manifest_home" %>

using System;
using System.Web;

public class manifest_home : IHttpHandler
{
    private static string ResolveIconMime(string iconPath)
    {
        string lower = (iconPath ?? string.Empty).Trim().ToLowerInvariant();
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

        string iconPath = PortalBranding_cl.DefaultHomeIconPath;
        string appName = "AhaSale";
        string shortName = "AhaSale";
        string themeColor = "#10b981";

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                PortalBranding_cl.ScopeBrandingSnapshot branding = PortalBranding_cl.LoadScopeBranding(db, PortalBranding_cl.ScopeHome, true);
                iconPath = PortalBranding_cl.NormalizeIconPath(
                    PortalBranding_cl.ResolveHeaderLogoPath(branding, PortalBranding_cl.ScopeHome),
                    PortalBranding_cl.DefaultHomeIconPath);
            }
        }
        catch
        {
            iconPath = PortalBranding_cl.NormalizeIconPath(iconPath, PortalBranding_cl.DefaultHomeIconPath);
        }

        string iconType = ResolveIconMime(iconPath);

        string json = string.Format(
            "{{\"name\":\"{0}\",\"short_name\":\"{1}\",\"description\":\"AhaSale.vn\",\"start_url\":\"/\",\"scope\":\"/\",\"display\":\"standalone\",\"background_color\":\"#ffffff\",\"theme_color\":\"{2}\",\"icons\":[{{\"src\":\"{3}\",\"sizes\":\"192x192\",\"type\":\"{4}\",\"purpose\":\"any maskable\"}},{{\"src\":\"{3}\",\"sizes\":\"512x512\",\"type\":\"{4}\",\"purpose\":\"any maskable\"}}]}}",
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
