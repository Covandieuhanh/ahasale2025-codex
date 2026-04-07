using System;
using System.Web;

public static class GianHangRoutes_cl
{
    public static string BuildOwnerHomeUrl()
    {
        return "/gianhang/default.aspx";
    }

    public static string BuildOwnerContentUrl()
    {
        return "/gianhang/quan-ly-tin/Default.aspx";
    }

    public static string BuildOwnerContentCreateUrl()
    {
        return "/gianhang/quan-ly-tin/Them.aspx";
    }

    public static string BuildOwnerOrdersUrl()
    {
        return "/gianhang/don-ban.aspx";
    }

    public static string BuildOwnerWaitUrl()
    {
        return "/gianhang/cho-thanh-toan.aspx";
    }

    public static string BuildOwnerCustomersUrl()
    {
        return "/gianhang/khach-hang.aspx";
    }

    public static string BuildOwnerCustomerDetailUrl(string customerKey)
    {
        string key = (customerKey ?? string.Empty).Trim();
        if (key == string.Empty)
            return BuildOwnerCustomersUrl();

        return BuildOwnerCustomersUrl() + "?key=" + HttpUtility.UrlEncode(key);
    }

    public static string BuildOwnerBookingsUrl()
    {
        return "/gianhang/quan-ly-lich-hen.aspx";
    }

    public static string BuildOwnerReportsUrl()
    {
        return "/gianhang/bao-cao.aspx";
    }

    public static string BuildOwnerAccountUrl()
    {
        return "/gianhang/tai-khoan/default.aspx";
    }

    public static string BuildOwnerElectronicInvoiceUrl(string rawId)
    {
        string safe = (rawId ?? string.Empty).Trim();
        if (safe == string.Empty)
            return "/gianhang/hoa-don-dien-tu.aspx";

        return "/gianhang/hoa-don-dien-tu.aspx?id=" + HttpUtility.UrlEncode(safe);
    }

    public static string BuildBridgeAdminAccessUrl()
    {
        return "/gianhang/mo-cong-cu-quan-tri.aspx";
    }

    public static string BuildBridgeLevel2Url()
    {
        return "/gianhang/nang-cap-level2.aspx";
    }

    public static string BuildPublicStorefrontUrl(string accountKey)
    {
        string normalized = NormalizeAccount(accountKey);
        if (normalized == string.Empty)
            return "/gianhang/public.aspx";

        return "/gianhang/public.aspx?user=" + HttpUtility.UrlEncode(normalized);
    }

    public static string BuildPublicProductsUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/page/danh-sach-san-pham.aspx", accountKey);
    }

    public static string BuildPublicServicesUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/page/danh-sach-dich-vu.aspx", accountKey);
    }

    public static string BuildPublicArticlesUrl(string accountKey)
    {
        return AppendUserToUrl("/gianhang/page/danh-sach-bai-viet.aspx", accountKey);
    }

    public static string BuildPublicProductDetailUrl(int itemId)
    {
        return "/gianhang/xem-san-pham.aspx?id=" + itemId.ToString();
    }

    public static string BuildPublicServiceDetailUrl(int itemId)
    {
        return "/gianhang/xem-dich-vu.aspx?id=" + itemId.ToString();
    }

    public static string BuildPublicBookingUrl(int itemId, string accountKey, string returnUrl)
    {
        string url = "/gianhang/datlich.aspx?id=" + itemId.ToString();
        url = AppendUserToUrl(url, accountKey);
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildPublicCartUrl(string accountKey, string returnUrl)
    {
        string url = "/gianhang/giohang.aspx";
        url = AppendUserToUrl(url, accountKey);
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildPublicRemoveCartItemUrl(string accountKey, int itemId, string returnUrl)
    {
        string url = "/gianhang/xoa_chitiet_giohang.aspx?id=" + itemId.ToString();
        url = AppendUserToUrl(url, accountKey);
        return AppendReturnUrl(url, returnUrl);
    }

    public static string BuildPublicUrl(string accountKey)
    {
        return BuildPublicStorefrontUrl(accountKey);
    }

    public static string BuildStorefrontUrl(string accountKey)
    {
        return BuildPublicStorefrontUrl(accountKey);
    }

    public static string BuildDashboardUrl()
    {
        return BuildOwnerHomeUrl();
    }

    public static string BuildLoginUrl(string returnUrl)
    {
        string normalized = NormalizeReturnUrl(returnUrl, BuildDashboardUrl());
        return "/dang-nhap?return_url=" + HttpUtility.UrlEncode(normalized);
    }

    public static string BuildDonBanUrl()
    {
        return BuildOwnerOrdersUrl();
    }

    public static string BuildAdminWorkspaceHubUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/default.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceAccountCenterUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/trung-tam.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceOrdersUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/don-ban.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceCreateUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/tao-giao-dich.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceWaitUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/cho-thanh-toan.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceBuyerFlowUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/don-mua.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceStorefrontUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/trang-cong-khai.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspacePresentationUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/trinh-bay.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceCartUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/gio-hang.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceElectronicInvoiceUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/hoa-don-dien-tu.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceUtilityUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/tien-ich.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceUtilityConfigUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/tien-ich-co-cau.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceUtilityDrawUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/tien-ich-quay-so.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceContentUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/quan-ly-noi-dung.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceArticlesUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/bai-viet.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceArticleDetailUrl(object articleId)
    {
        string safe = Convert.ToString(articleId) ?? string.Empty;
        safe = safe.Trim();
        if (safe == string.Empty)
            return BuildAdminWorkspaceArticlesUrl();

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/bai-viet-chi-tiet.aspx?id=" + HttpUtility.UrlEncode(safe), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceProductsUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/san-pham.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceProductDetailUrl(int itemId)
    {
        if (itemId <= 0)
            return BuildAdminWorkspaceProductsUrl();

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/san-pham-chi-tiet.aspx?id=" + itemId.ToString(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceServicesUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/dich-vu.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceServiceDetailUrl(int itemId)
    {
        if (itemId <= 0)
            return BuildAdminWorkspaceServicesUrl();

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/dich-vu-chi-tiet.aspx?id=" + itemId.ToString(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceCustomersUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/khach-hang.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceCustomerDetailUrl(string customerKey)
    {
        string key = (customerKey ?? string.Empty).Trim();
        if (key == string.Empty)
            return BuildAdminWorkspaceCustomersUrl();

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/khach-hang-chi-tiet.aspx?key=" + HttpUtility.UrlEncode(key), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceBookingsUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/lich-hen.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceBookingDetailUrl(long bookingId)
    {
        if (bookingId <= 0)
            return BuildAdminWorkspaceBookingsUrl();

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/lich-hen-chi-tiet.aspx?id=" + bookingId.ToString(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminWorkspaceReportUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/gianhang/bao-cao.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyInvoiceListUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyInvoiceCreateUrl(string tenKhach, string sdt, string idDatLich, string idNganh)
    {
        string url = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?q=add";
        if (!string.IsNullOrWhiteSpace(tenKhach))
            url += "&tenkh=" + HttpUtility.UrlEncode(tenKhach.Trim());
        if (!string.IsNullOrWhiteSpace(sdt))
            url += "&sdt=" + HttpUtility.UrlEncode(sdt.Trim());
        if (!string.IsNullOrWhiteSpace(idDatLich))
            url += "&id_datlich=" + HttpUtility.UrlEncode(idDatLich.Trim());
        if (!string.IsNullOrWhiteSpace(idNganh))
            url += "&idnganh=" + HttpUtility.UrlEncode(idNganh.Trim());
        return WorkspaceContext_cl.AppendSystemAdminFlag(url, HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyInvoiceDetailUrl(long invoiceId)
    {
        if (invoiceId <= 0)
            return BuildAdminLegacyInvoiceListUrl();

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + invoiceId.ToString(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyInvoiceProductStatsUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyInvoiceServiceStatsUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyCustomersUrl(string keyword)
    {
        string url = "/gianhang/admin/quan-ly-khach-hang/Default.aspx";
        if (!string.IsNullOrWhiteSpace(keyword))
            url += "?keyword=" + HttpUtility.UrlEncode(keyword.Trim());
        return WorkspaceContext_cl.AppendSystemAdminFlag(url, HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyCustomerDetailUrl(long customerId)
    {
        if (customerId <= 0)
            return BuildAdminLegacyCustomersUrl(string.Empty);

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + customerId.ToString(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyBookingsUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx", HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyBookingDetailUrl(long bookingId)
    {
        if (bookingId <= 0)
            return BuildAdminLegacyBookingsUrl();

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + bookingId.ToString(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyPeopleHubUrl(string keyword)
    {
        string url = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
        if (!string.IsNullOrWhiteSpace(keyword))
            url += "?keyword=" + HttpUtility.UrlEncode(keyword.Trim());
        return WorkspaceContext_cl.AppendSystemAdminFlag(url, HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyArticlesUrl(string postType)
    {
        string url = "/gianhang/admin/quan-ly-bai-viet/Default.aspx";
        if (!string.IsNullOrWhiteSpace(postType))
            url += "?pl=" + HttpUtility.UrlEncode(postType.Trim());
        return WorkspaceContext_cl.AppendSystemAdminFlag(url, HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildAdminLegacyArticleEditUrl(long articleId, string postType)
    {
        if (articleId <= 0)
            return BuildAdminLegacyArticlesUrl(postType);

        return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/quan-ly-bai-viet/edit.aspx?id=" + articleId.ToString(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    public static string BuildBaoCaoUrl()
    {
        return BuildOwnerReportsUrl();
    }

    public static string BuildKhachHangUrl()
    {
        return BuildOwnerCustomersUrl();
    }

    public static string BuildKhachHangChiTietUrl(string customerKey)
    {
        return BuildOwnerCustomerDetailUrl(customerKey);
    }

    public static string BuildQuanLyTinUrl()
    {
        return BuildOwnerContentUrl();
    }

    public static string BuildBuyerOrdersUrl()
    {
        return "/gianhang/don-mua.aspx";
    }

    public static string BuildElectronicInvoiceUrl(string rawId)
    {
        return BuildOwnerElectronicInvoiceUrl(rawId);
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
        return BuildOwnerWaitUrl();
    }

    public static string BuildBookingManagementUrl()
    {
        return BuildOwnerBookingsUrl();
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
        return BuildPublicProductDetailUrl(itemId);
    }

    public static string BuildXemDichVuUrl(int itemId)
    {
        return BuildPublicServiceDetailUrl(itemId);
    }

    public static string BuildDatLichPublicUrl(int itemId, string accountKey, string returnUrl)
    {
        return BuildPublicBookingUrl(itemId, accountKey, returnUrl);
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
        return BuildPublicCartUrl(accountKey, returnUrl);
    }

    public static string BuildRemoveCartItemUrl(string accountKey, int itemId, string returnUrl)
    {
        return BuildPublicRemoveCartItemUrl(accountKey, itemId, returnUrl);
    }

    public static string BuildServicesUrl(string accountKey)
    {
        return BuildPublicServicesUrl(accountKey);
    }

    public static string BuildProductsUrl(string accountKey)
    {
        return BuildPublicProductsUrl(accountKey);
    }

    public static string BuildArticlesUrl(string accountKey)
    {
        return BuildPublicArticlesUrl(accountKey);
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
