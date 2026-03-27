using System;

public static class ModuleSpace_cl
{
    public const string Home = "home";
    public const string GianHang = "gianhang";
    public const string Shop = "shop";
    public const string Admin = "admin";
    public const string GianHangAdmin = "gianhang_admin";
    public const string DauGia = "daugia";
    public const string Event = "event";

    public static string Normalize(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == Home) return Home;
        if (value == GianHang) return GianHang;
        if (value == Shop) return Shop;
        if (value == Admin) return Admin;
        if (value == GianHangAdmin) return GianHangAdmin;
        if (value == DauGia) return DauGia;
        if (value == Event) return Event;
        return "";
    }

    public static string ResolveByPath(string path)
    {
        string value = (path ?? "").Trim();
        if (value == "")
            return "";

        if (value.Equals("/admin", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return Admin;
        if (value.Equals("/shop", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase))
            return Shop;
        if (value.Equals("/gianhang/admin", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
            return GianHangAdmin;
        if (value.StartsWith("/gianhang/", StringComparison.OrdinalIgnoreCase) || value.Equals("/gianhang", StringComparison.OrdinalIgnoreCase))
            return GianHang;
        if (value.Equals("/daugia", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/daugia/", StringComparison.OrdinalIgnoreCase))
            return DauGia;
        if (value.Equals("/event", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/event/", StringComparison.OrdinalIgnoreCase))
            return Event;
        if (value.Equals("/home", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/home/", StringComparison.OrdinalIgnoreCase))
            return Home;

        return "";
    }
}
