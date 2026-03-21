using System;

public static class ModuleSpace_cl
{
    public const string Home = "home";
    public const string Shop = "shop";
    public const string Admin = "admin";
    public const string GianHangAdmin = "gianhang_admin";
    public const string DauGia = "daugia";

    public static string Normalize(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == Home) return Home;
        if (value == Shop) return Shop;
        if (value == Admin) return Admin;
        if (value == GianHangAdmin) return GianHangAdmin;
        if (value == DauGia) return DauGia;
        return "";
    }

    public static string ResolveByPath(string path)
    {
        string value = (path ?? "").Trim();
        if (value == "")
            return "";

        if (value.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return Admin;
        if (value.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase))
            return Shop;
        if (value.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
            return GianHangAdmin;
        if (value.StartsWith("/daugia/", StringComparison.OrdinalIgnoreCase))
            return DauGia;
        if (value.StartsWith("/home/", StringComparison.OrdinalIgnoreCase))
            return Home;

        return "";
    }
}

