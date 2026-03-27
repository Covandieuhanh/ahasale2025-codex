using System;

public static class AhaShineHomeRoutes_cl
{
    public const string BasePath = "/gianhang";
    public static string HomeUrl { get { return BasePath + "/default.aspx"; } }
    public static string BookingUrl { get { return BasePath + "/datlich.aspx"; } }
    public static string CartUrl { get { return BasePath + "/giohang.aspx"; } }
    public static string RemoveCartItemUrl { get { return BasePath + "/xoa_chitiet_giohang.aspx"; } }
    public static string ServicesUrl { get { return BasePath + "/page/danh-sach-dich-vu.aspx"; } }
    public static string ProductsUrl { get { return BasePath + "/page/danh-sach-san-pham.aspx"; } }
    public static string ArticlesUrl { get { return BasePath + "/page/danh-sach-bai-viet.aspx"; } }
    public static string AccountUrl { get { return BasePath + "/tai-khoan/default.aspx"; } }
    public static string ChangePasswordUrl { get { return BasePath + "/tai-khoan/doi-mat-khau.aspx"; } }

    public static bool ShouldSkipReturnUrl(string path)
    {
        string normalized = (path ?? "").Trim().ToLowerInvariant();
        return normalized == "/gianhang/admin/login.aspx"
            || normalized == "/gianhang/admin/quen-mat-khau/default.aspx"
            || normalized == "/gianhang/admin/quen-mat-khau/nhap-ma-khoi-phuc.aspx"
            || normalized == "/gianhang/admin/quen-mat-khau/dat-lai-mat-khau.aspx"
            || normalized == "/gianhang/giohang.aspx"
            || normalized == "/gianhang/xoa_chitiet_giohang.aspx";
    }

    public static string NormalizeReturnUrl(string rawUrl)
    {
        if (string.IsNullOrWhiteSpace(rawUrl))
            return HomeUrl;

        string url = rawUrl.Trim();
        string lower = url.ToLowerInvariant();

        if (lower == "/" || lower == "/default.aspx")
            return HomeUrl;

        if (lower.StartsWith("/datlich.aspx") || lower.StartsWith("/dat-lich.aspx"))
            return BasePath + url;

        if (lower.StartsWith("/page/") || lower.StartsWith("/chi-tiet-") || lower.StartsWith("/hoa-don-dien-tu"))
            return BasePath + url;

        if (lower.StartsWith("/giohang"))
            return CartUrl;

        if (lower.StartsWith("/xoa_chitiet_giohang.aspx"))
            return CartUrl;

        if (lower.StartsWith("/tai-khoan/"))
            return BasePath + url;

        if (lower.StartsWith(BasePath + "/xoa_chitiet_giohang.aspx"))
            return CartUrl;

        if (lower.StartsWith(BasePath + "/giohang.aspx"))
            return CartUrl;

        if (lower.StartsWith(BasePath))
            return url;

        return HomeUrl;
    }
}
