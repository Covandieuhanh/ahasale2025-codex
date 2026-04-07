using System;
using System.Web;
using System.Web.UI;

public static class GianHangSystemAdminGuard_cl
{
    private const string UnknownSystemRouteFeature = "__gianhang_system_unknown_route__";
    private static readonly string[] LegacyFeatureFallbackOrder =
    {
        "shop_legacy_invoices",
        "shop_legacy_customers",
        "shop_legacy_content",
        "shop_legacy_finance",
        "shop_legacy_inventory",
        "shop_legacy_accounts",
        "shop_legacy_org",
        "shop_legacy_training",
        "shop_legacy_support"
    };

    private static string ResolveFeatureKey(string appRelativePath)
    {
        string path = (appRelativePath ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        if (!path.StartsWith("~/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        if (path == "~/gianhang/admin/login.aspx" || path == "~/gianhang/admin/f5_ss_admin.aspx")
            return string.Empty;

        if (path.StartsWith("~/gianhang/admin/quen-mat-khau/", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        if (path == "~/gianhang/admin/default.aspx"
            || path == "~/gianhang/admin/"
            || path == "~/gianhang/admin")
            return "shop_workspace";

        if (path.StartsWith("~/gianhang/admin/quan-ly-hoa-don/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_invoices";

        if (path.StartsWith("~/gianhang/admin/quan-ly-khach-hang/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/quan-ly-con-nguoi/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_customers";

        if (path.StartsWith("~/gianhang/admin/quan-ly-bai-viet/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/quan-ly-menu/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/quan-ly-module/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/cau-hinh-chung/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/cau-hinh-storefront/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/cai-dat-chung/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_content";

        if (path.StartsWith("~/gianhang/admin/quan-ly-thu-chi/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_finance";

        if (path.StartsWith("~/gianhang/admin/quan-ly-kho-hang/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/quan-ly-vat-tu/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/quan-ly-the-dich-vu/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_inventory";

        if (path.StartsWith("~/gianhang/admin/quan-ly-tai-khoan/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_accounts";

        if (path.StartsWith("~/gianhang/admin/quan-ly-he-thong/", StringComparison.OrdinalIgnoreCase)
            || path == "~/gianhang/admin/doi-chi-nhanh.aspx")
            return "shop_legacy_org";

        if (path.StartsWith("~/gianhang/admin/quan-ly-hoc-vien/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/quan-ly-giang-vien/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_training";

        if (path.StartsWith("~/gianhang/admin/quan-ly-thong-bao/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/gianhang/admin/yeu-cau-tu-van/", StringComparison.OrdinalIgnoreCase))
            return "shop_legacy_support";

        if (path.StartsWith("~/gianhang/admin/gianhang/", StringComparison.OrdinalIgnoreCase))
        {
            if (path == "~/gianhang/admin/gianhang/default.aspx"
                || path == "~/gianhang/admin/gianhang/trung-tam.aspx"
                || path == "~/gianhang/admin/gianhang/tien-ich.aspx"
                || path == "~/gianhang/admin/gianhang/tien-ich-co-cau.aspx"
                || path == "~/gianhang/admin/gianhang/tien-ich-quay-so.aspx")
                return "shop_workspace";

            if (path == "~/gianhang/admin/gianhang/bai-viet.aspx"
                || path == "~/gianhang/admin/gianhang/bai-viet-chi-tiet.aspx"
                || path == "~/gianhang/admin/gianhang/quan-ly-noi-dung.aspx"
                || path == "~/gianhang/admin/gianhang/trang-cong-khai.aspx"
                || path == "~/gianhang/admin/gianhang/trinh-bay.aspx")
                return "shop_legacy_content";

            if (path == "~/gianhang/admin/gianhang/khach-hang.aspx"
                || path == "~/gianhang/admin/gianhang/khach-hang-chi-tiet.aspx"
                || path == "~/gianhang/admin/gianhang/lich-hen.aspx"
                || path == "~/gianhang/admin/gianhang/lich-hen-chi-tiet.aspx")
                return "shop_legacy_customers";

            if (path == "~/gianhang/admin/gianhang/don-ban.aspx"
                || path == "~/gianhang/admin/gianhang/don-mua.aspx"
                || path == "~/gianhang/admin/gianhang/cho-thanh-toan.aspx"
                || path == "~/gianhang/admin/gianhang/hoa-don-dien-tu.aspx"
                || path == "~/gianhang/admin/gianhang/bao-cao.aspx")
                return "shop_legacy_invoices";

            if (path == "~/gianhang/admin/gianhang/san-pham.aspx"
                || path == "~/gianhang/admin/gianhang/san-pham-chi-tiet.aspx"
                || path == "~/gianhang/admin/gianhang/dich-vu.aspx"
                || path == "~/gianhang/admin/gianhang/dich-vu-chi-tiet.aspx"
                || path == "~/gianhang/admin/gianhang/gio-hang.aspx"
                || path == "~/gianhang/admin/gianhang/tao-giao-dich.aspx")
                return "shop_legacy_inventory";

            return UnknownSystemRouteFeature;
        }

        return UnknownSystemRouteFeature;
    }

    public static string ResolveSystemFeatureKey(HttpRequest request)
    {
        if (request == null)
            return string.Empty;

        return ResolveFeatureKey(request.AppRelativeCurrentExecutionFilePath ?? string.Empty);
    }

    private static string ResolveWorkspaceFallbackUrl(HttpRequest request)
    {
        if (AdminAccessGuard_cl.CanCurrentAdminAccessFeature("shop_workspace"))
            return WorkspaceContext_cl.AppendSystemAdminFlag("/gianhang/admin/default.aspx", request);

        for (int i = 0; i < LegacyFeatureFallbackOrder.Length; i++)
        {
            string featureKey = LegacyFeatureFallbackOrder[i];
            if (!AdminAccessGuard_cl.CanCurrentAdminAccessFeature(featureKey))
                continue;

            AdminFeatureRegistry_cl.FeatureDefinition feature = AdminFeatureRegistry_cl.Get(featureKey);
            string url = feature == null ? "" : (feature.DefaultUrl ?? "").Trim();
            if (string.IsNullOrWhiteSpace(url))
                continue;

            return WorkspaceContext_cl.AppendSystemAdminFlag(url, request);
        }

        return "/admin/default.aspx?mspace=gianhang";
    }

    public static bool EnsurePageAccess(Page page)
    {
        if (page == null || page.Request == null)
            return true;

        if (!WorkspaceContext_cl.IsSystemAdminMode(page.Request))
            return true;

        string featureKey = ResolveSystemFeatureKey(page.Request);
        if (string.IsNullOrWhiteSpace(featureKey))
            return true;

        if (string.Equals(featureKey, UnknownSystemRouteFeature, StringComparison.OrdinalIgnoreCase))
        {
            if (page.Session != null)
                page.Session["thongbao"] = thongbao_class.metro_notifi_onload(
                    "Thông báo",
                    "Route /gianhang/admin chưa được map quyền system-admin rõ ràng. Vui lòng liên hệ Super Admin để cập nhật policy.",
                    "2600",
                    "warning");
            page.Response.Redirect("/admin/default.aspx?mspace=gianhang", false);
            HttpContext current = HttpContext.Current;
            if (current != null && current.ApplicationInstance != null)
                current.ApplicationInstance.CompleteRequest();
            return false;
        }

        if (string.Equals(featureKey, "shop_workspace", StringComparison.OrdinalIgnoreCase)
            && !AdminAccessGuard_cl.CanCurrentAdminAccessFeature("shop_workspace"))
        {
            string fallbackUrl = ResolveWorkspaceFallbackUrl(page.Request);
            page.Response.Redirect(fallbackUrl, false);
            HttpContext fallbackContext = HttpContext.Current;
            if (fallbackContext != null && fallbackContext.ApplicationInstance != null)
                fallbackContext.ApplicationInstance.CompleteRequest();
            return false;
        }

        AdminAccessGuard_cl.RequireFeatureAccess(featureKey, "/admin/default.aspx?mspace=gianhang");
        return page.Response == null || !page.Response.IsRequestBeingRedirected;
    }

    public static bool EnsureHandlerAccess(HttpContext context)
    {
        if (context == null || context.Request == null)
            return false;

        if (!WorkspaceContext_cl.IsSystemAdminMode(context.Request))
            return true;

        string featureKey = ResolveSystemFeatureKey(context.Request);
        if (string.IsNullOrWhiteSpace(featureKey))
            return true;
        if (string.Equals(featureKey, UnknownSystemRouteFeature, StringComparison.OrdinalIgnoreCase))
            return false;

        return AdminAccessGuard_cl.CanCurrentAdminAccessFeature(featureKey);
    }
}
