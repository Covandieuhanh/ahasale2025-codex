using System;
using System.Web;

public static class GianHangRoutes_cl
{
    public static string BuildPublicUrl(string accountKey)
    {
        return BuildStorefrontUrl(accountKey);
    }

    public static string BuildStorefrontUrl(string accountKey)
    {
        string normalized = NormalizeAccount(accountKey);
        if (normalized == string.Empty)
            return "/gianhang/public.aspx";

        return "/gianhang/public.aspx?user=" + HttpUtility.UrlEncode(normalized);
    }

    public static string BuildDashboardUrl()
    {
        return "/gianhang/default.aspx";
    }

    public static string BuildLoginUrl(string returnUrl)
    {
        string normalized = NormalizeReturnUrl(returnUrl, BuildDashboardUrl());
        return "/dang-nhap?return_url=" + HttpUtility.UrlEncode(normalized);
    }

    public static string BuildDonBanUrl()
    {
        return "/gianhang/don-ban.aspx";
    }

    public static string BuildAdminWorkspaceHubUrl()
    {
        return "/gianhang/admin/gianhang/default.aspx";
    }

    public static string BuildAdminWorkspaceAccountCenterUrl()
    {
        return "/gianhang/admin/gianhang/trung-tam.aspx";
    }

    public static string BuildAdminWorkspaceOrdersUrl()
    {
        return "/gianhang/admin/gianhang/don-ban.aspx";
    }

    public static string BuildAdminWorkspaceCreateUrl()
    {
        return "/gianhang/admin/gianhang/tao-giao-dich.aspx";
    }

    public static string BuildAdminWorkspaceWaitUrl()
    {
        return "/gianhang/admin/gianhang/cho-thanh-toan.aspx";
    }

    public static string BuildAdminWorkspaceBuyerFlowUrl()
    {
        return "/gianhang/admin/gianhang/don-mua.aspx";
    }

    public static string BuildAdminWorkspaceStorefrontUrl()
    {
        return "/gianhang/admin/gianhang/trang-cong-khai.aspx";
    }

    public static string BuildAdminWorkspacePresentationUrl()
    {
        return "/gianhang/admin/gianhang/trinh-bay.aspx";
    }

    public static string BuildAdminWorkspaceCartUrl()
    {
        return "/gianhang/admin/gianhang/gio-hang.aspx";
    }

    public static string BuildAdminWorkspaceElectronicInvoiceUrl()
    {
        return "/gianhang/admin/gianhang/hoa-don-dien-tu.aspx";
    }

    public static string BuildAdminWorkspaceUtilityUrl()
    {
        return "/gianhang/admin/gianhang/tien-ich.aspx";
    }

    public static string BuildAdminWorkspaceUtilityConfigUrl()
    {
        return "/gianhang/admin/gianhang/tien-ich-co-cau.aspx";
    }

    public static string BuildAdminWorkspaceUtilityDrawUrl()
    {
        return "/gianhang/admin/gianhang/tien-ich-quay-so.aspx";
    }

    public static string BuildAdminWorkspaceContentUrl()
    {
        return "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    }

    public static string BuildAdminWorkspaceArticlesUrl()
    {
        return "/gianhang/admin/gianhang/bai-viet.aspx";
    }

    public static string BuildAdminWorkspaceArticleDetailUrl(object articleId)
    {
        string safe = Convert.ToString(articleId) ?? string.Empty;
        safe = safe.Trim();
        if (safe == string.Empty)
            return BuildAdminWorkspaceArticlesUrl();

        return "/gianhang/admin/gianhang/bai-viet-chi-tiet.aspx?id=" + HttpUtility.UrlEncode(safe);
    }

    public static string BuildAdminWorkspaceProductsUrl()
    {
        return "/gianhang/admin/gianhang/san-pham.aspx";
    }

    public static string BuildAdminWorkspaceProductDetailUrl(int itemId)
    {
        if (itemId <= 0)
            return BuildAdminWorkspaceProductsUrl();

        return "/gianhang/admin/gianhang/san-pham-chi-tiet.aspx?id=" + itemId.ToString();
    }

    public static string BuildAdminWorkspaceServicesUrl()
    {
        return "/gianhang/admin/gianhang/dich-vu.aspx";
    }

    public static string BuildAdminWorkspaceServiceDetailUrl(int itemId)
    {
        if (itemId <= 0)
            return BuildAdminWorkspaceServicesUrl();

        return "/gianhang/admin/gianhang/dich-vu-chi-tiet.aspx?id=" + itemId.ToString();
    }

    public static string BuildAdminWorkspaceCustomersUrl()
    {
        return "/gianhang/admin/gianhang/khach-hang.aspx";
    }

    public static string BuildAdminWorkspaceCustomerDetailUrl(string customerKey)
    {
        string key = (customerKey ?? string.Empty).Trim();
        if (key == string.Empty)
            return BuildAdminWorkspaceCustomersUrl();

        return "/gianhang/admin/gianhang/khach-hang-chi-tiet.aspx?key=" + HttpUtility.UrlEncode(key);
    }

    public static string BuildAdminWorkspaceBookingsUrl()
    {
        return "/gianhang/admin/gianhang/lich-hen.aspx";
    }

    public static string BuildAdminWorkspaceBookingDetailUrl(long bookingId)
    {
        if (bookingId <= 0)
            return BuildAdminWorkspaceBookingsUrl();

        return "/gianhang/admin/gianhang/lich-hen-chi-tiet.aspx?id=" + bookingId.ToString();
    }

    public static string BuildAdminWorkspaceReportUrl()
    {
        return "/gianhang/admin/gianhang/bao-cao.aspx";
    }

    public static string BuildBaoCaoUrl()
    {
        return "/gianhang/bao-cao.aspx";
    }

    public static string BuildKhachHangUrl()
    {
        return "/gianhang/khach-hang.aspx";
    }

    public static string BuildKhachHangChiTietUrl(string customerKey)
    {
        string key = (customerKey ?? string.Empty).Trim();
        if (key == string.Empty)
            return BuildKhachHangUrl();

        return BuildKhachHangUrl() + "?key=" + HttpUtility.UrlEncode(key);
    }

    public static string BuildQuanLyTinUrl()
    {
        return "/gianhang/quan-ly-tin/Default.aspx";
    }

    public static string BuildBuyerOrdersUrl()
    {
        return "/gianhang/don-mua.aspx";
    }

    public static string BuildElectronicInvoiceUrl(string rawId)
    {
        string safe = (rawId ?? string.Empty).Trim();
        if (safe == string.Empty)
            return "/gianhang/hoa-don-dien-tu.aspx";
        return "/gianhang/hoa-don-dien-tu.aspx?id=" + HttpUtility.UrlEncode(safe);
    }

    public static string BuildUtilityConfigUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/tien-ich/co-cau.aspx", accountKey);
    }

    public static string BuildUtilityDrawUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/tien-ich/quayso.aspx", accountKey);
    }

    // Compatibility alias for legacy call sites.
    public static string ChoThanhToanUrl()
    {
        return "/gianhang/cho-thanh-toan.aspx";
    }

    public static string BuildBookingManagementUrl()
    {
        return "/gianhang/quan-ly-lich-hen.aspx";
    }

    public static string BuildChiTietSanPhamUrl(int itemId)
    {
        return "/gianhang/chi-tiet-san-pham.aspx?id=" + itemId.ToString();
    }

    public static string BuildChiTietDichVuUrl(int itemId)
    {
        return "/gianhang/chi-tiet-dich-vu.aspx?id=" + itemId.ToString();
    }

    public static string BuildXemSanPhamUrl(int itemId)
    {
        return "/gianhang/xem-san-pham.aspx?id=" + itemId.ToString();
    }

    public static string BuildXemDichVuUrl(int itemId)
    {
        return "/gianhang/xem-dich-vu.aspx?id=" + itemId.ToString();
    }

    public static string BuildDatLichPublicUrl(int itemId, string accountKey, string returnUrl)
    {
        string url = "/gianhang/datlich.aspx?id=" + itemId.ToString();
        url = AppendUserToUrl(url, accountKey);
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildDatLichUrl(int itemId, string returnUrl)
    {
        string url = "/gianhang/dat-lich.aspx?id=" + itemId.ToString();
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildTaoDonUrl(int itemId, string returnUrl)
    {
        string url = BuildDonBanUrl() + "?taodon=1&idsp=" + itemId.ToString();
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildPublicCartUrl(string accountKey, int itemId, int quantity, string returnUrl, bool focusCheckout)
    {
        int safeQty = quantity <= 0 ? 1 : quantity;
        string url = "/gianhang/giohang.aspx?id=" + itemId.ToString() + "&qty=" + safeQty.ToString();
        if (focusCheckout)
            url += "&focus=checkout";
        url = AppendUserToUrl(url, accountKey);
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildHomeSpaceUrl()
    {
        return "/home/default.aspx";
    }

    public static string BuildCartUrl(string accountKey, string returnUrl)
    {
        string url = BuildStorefrontUrl(string.Empty).Replace("/public.aspx", "/giohang.aspx");
        url = AppendUserToUrl(url, accountKey);
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildRemoveCartItemUrl(string accountKey, int itemId, string returnUrl)
    {
        string url = "/gianhang/xoa_chitiet_giohang.aspx?id=" + itemId.ToString();
        url = AppendUserToUrl(url, accountKey);
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildServicesUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/page/danh-sach-dich-vu.aspx", accountKey);
    }

    public static string BuildProductsUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/page/danh-sach-san-pham.aspx", accountKey);
    }

    public static string BuildArticlesUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/page/danh-sach-bai-viet.aspx", accountKey);
    }

    public static string BuildBookingHubUrl(string accountKey, string returnUrl)
    {
        return AppendReturnUrl(BuildServicesUrl(accountKey), returnUrl);
    }

    public static string BuildServiceCategoryUrl(string accountKey, string menuId)
    {
        string url = "/gianhang/page/danh-sach-dich-vu.aspx";
        if (!string.IsNullOrWhiteSpace(menuId))
            url += "?idmn=" + HttpUtility.UrlEncode(menuId.Trim());
        return AppendUserToUrl(url, accountKey);
    }

    public static string BuildProductCategoryUrl(string accountKey, string menuId)
    {
        string url = "/gianhang/page/danh-sach-san-pham.aspx";
        if (!string.IsNullOrWhiteSpace(menuId))
            url += "?idmn=" + HttpUtility.UrlEncode(menuId.Trim());
        return AppendUserToUrl(url, accountKey);
    }

    public static string NormalizeReturnUrl(string rawReturnUrl, string fallbackUrl)
    {
        string normalized = (rawReturnUrl ?? string.Empty).Trim();
        if (normalized == string.Empty)
            normalized = fallbackUrl ?? BuildStorefrontUrl(string.Empty);

        if (!normalized.StartsWith("/", StringComparison.Ordinal))
            return fallbackUrl ?? BuildStorefrontUrl(string.Empty);

        if (normalized.StartsWith("//", StringComparison.Ordinal))
            return fallbackUrl ?? BuildStorefrontUrl(string.Empty);

        return normalized;
    }

    public static string AppendUserToUrl(string rawUrl, string accountKey)
    {
        string url = (rawUrl ?? string.Empty).Trim();
        string normalized = NormalizeAccount(accountKey);
        if (url == string.Empty || normalized == string.Empty)
            return url;

        if (url.IndexOf("user=", StringComparison.OrdinalIgnoreCase) >= 0)
            return url;

        int hashIndex = url.IndexOf('#');
        string hash = string.Empty;
        if (hashIndex >= 0)
        {
            hash = url.Substring(hashIndex);
            url = url.Substring(0, hashIndex);
        }

        url += (url.IndexOf('?') >= 0 ? "&" : "?") + "user=" + HttpUtility.UrlEncode(normalized);
        return url + hash;
    }

    public static string AppendReturnUrl(string rawUrl, string returnUrl)
    {
        string url = (rawUrl ?? string.Empty).Trim();
        string normalizedReturnUrl = NormalizeReturnUrl(returnUrl, string.Empty);
        if (url == string.Empty || normalizedReturnUrl == string.Empty)
            return url;

        if (url.IndexOf("return_url=", StringComparison.OrdinalIgnoreCase) >= 0)
            return url;

        url += (url.IndexOf('?') >= 0 ? "&" : "?") + "return_url=" + HttpUtility.UrlEncode(normalizedReturnUrl);
        return url;
    }

    private static string NormalizeAccount(string accountKey)
    {
        return (accountKey ?? string.Empty).Trim().ToLowerInvariant();
    }
}
