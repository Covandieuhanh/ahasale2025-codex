using System;
using System.Linq;
using System.Web;

public static class PortalBranding_cl
{
    public const string ScopeHome = "home";
    public const string ScopeShop = "shop";
    public const string ScopeAdmin = "admin";
    public const string ScopeBatDongSan = "batdongsan";

    public const string DefaultHomeIconPath = "/uploads/images/favicon.png";
    public const string DefaultShopIconPath = "/uploads/images/logo-aha.png";
    public const string DefaultAdminIconPath = "/uploads/images/favicon.png";
    public const string DefaultBatDongSanIconPath = DefaultHomeIconPath;

    public sealed class ScopeBrandingSnapshot
    {
        public string IconPath { get; set; }
        public string AppleTouchIconPath { get; set; }
        public string LogoPath { get; set; }
        public string LogoAltPath { get; set; }
    }

    public static string NormalizeScopeKey(string rawScope)
    {
        string scope = (rawScope ?? string.Empty).Trim().ToLowerInvariant();
        if (scope == ScopeShop)
            return ScopeShop;
        if (scope == ScopeAdmin)
            return ScopeAdmin;
        if (scope == ScopeBatDongSan)
            return ScopeBatDongSan;
        return ScopeHome;
    }

    public static string ResolveScopeKeyFromRequest(HttpRequest request)
    {
        string path = request == null || request.Url == null ? string.Empty : (request.Url.AbsolutePath ?? string.Empty).Trim();
        if (path.StartsWith("/bat-dong-san", StringComparison.OrdinalIgnoreCase))
            return ScopeBatDongSan;
        if (path.StartsWith("/gianhang", StringComparison.OrdinalIgnoreCase))
            return ScopeShop;
        return path.StartsWith("/shop", StringComparison.OrdinalIgnoreCase) ? ScopeShop : ScopeHome;
    }

    public static string ResolveDefaultIconPath(string scopeKey)
    {
        switch (NormalizeScopeKey(scopeKey))
        {
            case ScopeShop:
                return DefaultShopIconPath;
            case ScopeAdmin:
                return DefaultAdminIconPath;
            case ScopeBatDongSan:
                return DefaultBatDongSanIconPath;
            default:
                return DefaultHomeIconPath;
        }
    }

    public static bool IsAbsoluteHttpUrl(string value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
    }

    public static string NormalizeIconPath(string rawPath, string fallbackPath)
    {
        string path = (rawPath ?? string.Empty).Trim();
        string fallback = (fallbackPath ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(path))
            path = fallback;

        if (string.IsNullOrEmpty(path))
            path = DefaultHomeIconPath;

        if (path.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            path = string.IsNullOrEmpty(fallback) ? DefaultHomeIconPath : fallback;

        if (path.StartsWith("~/", StringComparison.Ordinal))
            path = path.Substring(1);

        if (IsAbsoluteHttpUrl(path))
            return path;

        if (!path.StartsWith("/", StringComparison.Ordinal))
            path = "/" + path.TrimStart('/');

        return path;
    }

    public static string BuildAbsoluteUrl(HttpRequest request, string rawPath, string fallbackPath)
    {
        string path = NormalizeIconPath(rawPath, fallbackPath);
        if (IsAbsoluteHttpUrl(path) || request == null || request.Url == null)
            return path;

        return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, path);
    }

    public static ScopeBrandingSnapshot LoadScopeBranding(dbDataContext db, string scopeKey, bool fallbackToHome)
    {
        if (db == null)
            return new ScopeBrandingSnapshot();

        string normalizedScope = NormalizeScopeKey(scopeKey);
        var row = (from tk in db.CaiDatChung_tbs
                   where tk.phanloai_trang == normalizedScope
                   select new
                   {
                       tk.thongtin_icon,
                       tk.thongtin_apple_touch_icon,
                       tk.thongtin_logo,
                       tk.thongtin_logo1
                   }).FirstOrDefault();

        if (row == null && fallbackToHome && normalizedScope != ScopeHome)
        {
            row = (from tk in db.CaiDatChung_tbs
                   where tk.phanloai_trang == ScopeHome
                   select new
                   {
                       tk.thongtin_icon,
                       tk.thongtin_apple_touch_icon,
                       tk.thongtin_logo,
                       tk.thongtin_logo1
                   }).FirstOrDefault();
        }

        if (row == null)
            return new ScopeBrandingSnapshot();

        return new ScopeBrandingSnapshot
        {
            IconPath = row.thongtin_icon,
            AppleTouchIconPath = row.thongtin_apple_touch_icon,
            LogoPath = row.thongtin_logo,
            LogoAltPath = row.thongtin_logo1
        };
    }

    public static string ResolveHeaderLogoPath(ScopeBrandingSnapshot snapshot, string scopeKey)
    {
        string fallback = ResolveDefaultIconPath(scopeKey);
        if (snapshot == null)
            return fallback;

        return FirstValidIconPath(
            snapshot.AppleTouchIconPath,
            snapshot.LogoPath,
            snapshot.LogoAltPath,
            snapshot.IconPath,
            fallback);
    }

    public static string ResolveFaviconPath(ScopeBrandingSnapshot snapshot, string scopeKey)
    {
        string fallback = ResolveDefaultIconPath(scopeKey);
        if (snapshot == null)
            return fallback;

        return FirstValidIconPath(
            snapshot.IconPath,
            snapshot.AppleTouchIconPath,
            snapshot.LogoPath,
            snapshot.LogoAltPath,
            fallback);
    }

    private static string FirstValidIconPath(params string[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            string value = (values[i] ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(value) && !value.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                return NormalizeIconPath(value, string.Empty);
        }

        return DefaultHomeIconPath;
    }
}
