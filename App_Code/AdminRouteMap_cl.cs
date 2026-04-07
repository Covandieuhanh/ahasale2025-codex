using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class AdminRouteMap_cl
{
    public sealed class RouteDefinition
    {
        public string FeatureKey { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
    }

    private static readonly RouteDefinition[] Routes =
    {
        new RouteDefinition { FeatureKey = "login", Path = "/admin/login.aspx", Title = "Đăng nhập admin" },
        new RouteDefinition { FeatureKey = "admin_dashboard", Path = "/admin/default.aspx", Title = "Trang chủ admin" },
        new RouteDefinition { FeatureKey = "admin_accounts", Path = "/admin/quan-ly-tai-khoan/default.aspx", Title = "Quản lý tài khoản" },
        new RouteDefinition { FeatureKey = "admin_accounts", Path = "/admin/quan-ly-tai-khoan/them-moi.aspx", Title = "Quản lý tài khoản" },
        new RouteDefinition { FeatureKey = "admin_accounts", Path = "/admin/quan-ly-tai-khoan/bo-loc.aspx", Title = "Quản lý tài khoản" },
        new RouteDefinition { FeatureKey = "admin_accounts", Path = "/admin/quan-ly-tai-khoan/chinh-sua.aspx", Title = "Quản lý tài khoản" },
        new RouteDefinition { FeatureKey = "admin_accounts", Path = "/admin/quan-ly-tai-khoan/phan-quyen.aspx", Title = "Quản lý tài khoản" },
        new RouteDefinition { FeatureKey = "admin_otp", Path = "/admin/otp/default.aspx", Title = "Quản lý OTP" },
        new RouteDefinition { FeatureKey = "admin_feedback", Path = "/admin/quan-ly-gop-y/default.aspx", Title = "Quản lý góp ý" },
        new RouteDefinition { FeatureKey = "notifications", Path = "/admin/quan-ly-thong-bao/default.aspx", Title = "Quản lý thông báo" },
        new RouteDefinition { FeatureKey = "notifications", Path = "/admin/quan-ly-thong-bao/in.aspx", Title = "Quản lý thông báo" },
        new RouteDefinition { FeatureKey = "notifications", Path = "/admin/quan-ly-thong-bao/bo-loc.aspx", Title = "Quản lý thông báo" },
        new RouteDefinition { FeatureKey = "notifications", Path = "/admin/quan-ly-thong-bao/xuat-du-lieu.aspx", Title = "Quản lý thông báo" },
        new RouteDefinition { FeatureKey = "notifications", Path = "/admin/quan-ly-thong-bao/ban-in.aspx", Title = "Quản lý thông báo" },
        new RouteDefinition { FeatureKey = "consulting", Path = "/admin/yeu-cau-tu-van/default.aspx", Title = "Yêu cầu tư vấn" },
        new RouteDefinition { FeatureKey = "consulting", Path = "/admin/yeu-cau-tu-van/bo-loc.aspx", Title = "Yêu cầu tư vấn" },
        new RouteDefinition { FeatureKey = "consulting", Path = "/admin/yeu-cau-tu-van/xuat-du-lieu.aspx", Title = "Yêu cầu tư vấn" },
        new RouteDefinition { FeatureKey = "consulting", Path = "/admin/yeu-cau-tu-van/ban-in.aspx", Title = "Yêu cầu tư vấn" },
        new RouteDefinition { FeatureKey = "admin_company_shop_sync", Path = "/admin/tools/company-shop-sync.aspx", Title = "Đồng bộ shop công ty" },
        new RouteDefinition { FeatureKey = "admin_reindex_baiviet", Path = "/admin/tools/reindex-baiviet.aspx", Title = "Reindex bài viết" },
        new RouteDefinition { FeatureKey = "core_assets", Path = "/admin/trigger-seed-space.aspx", Title = "Seed không gian hệ thống" },
        new RouteDefinition { FeatureKey = "core_assets", Path = "/admin/lich-su-chuyen-diem/default.aspx", Title = "Lịch sử chuyển điểm" },
        new RouteDefinition { FeatureKey = "core_assets", Path = "/admin/lich-su-chuyen-diem/chuyen-diem.aspx", Title = "Lịch sử chuyển điểm" },
        new RouteDefinition { FeatureKey = "home_point_approval", Path = "/admin/duyet-yeu-cau-len-cap.aspx", Title = "Duyệt yêu cầu xác nhận hành vi" },
        new RouteDefinition { FeatureKey = "issue_cards", Path = "/admin/phat-hanh-the.aspx", Title = "Phát hành thẻ" },
        new RouteDefinition { FeatureKey = "issue_cards", Path = "/admin/phat-hanh-the/them-moi.aspx", Title = "Phát hành thẻ" },
        new RouteDefinition { FeatureKey = "tier_reference", Path = "/admin/motacapbac.aspx", Title = "Mô tả cấp bậc" },
        new RouteDefinition { FeatureKey = "tier_reference", Path = "/admin/MoTaCapBac.aspx", Title = "Mô tả cấp bậc" },
        new RouteDefinition { FeatureKey = "system_products", Path = "/admin/he-thong-san-pham/ban-san-pham.aspx", Title = "Bán sản phẩm" },
        new RouteDefinition { FeatureKey = "system_products", Path = "/admin/he-thong-san-pham/ban-the.aspx", Title = "Bán sản phẩm" },
        new RouteDefinition { FeatureKey = "system_products", Path = "/admin/he-thong-san-pham/chi-tiet-giao-dich.aspx", Title = "Bán sản phẩm" },
        new RouteDefinition { FeatureKey = "shop_workspace", Path = "/gianhang/admin", Title = "Trung tâm quản trị gian hàng" },
        new RouteDefinition { FeatureKey = "shop_workspace", Path = "/gianhang/admin/", Title = "Trung tâm quản trị gian hàng" },
        new RouteDefinition { FeatureKey = "shop_workspace", Path = "/gianhang/admin/default.aspx", Title = "Trung tâm quản trị gian hàng" },
        new RouteDefinition { FeatureKey = "shop_email", Path = "/admin/quan-ly-email-shop/default.aspx", Title = "Nội dung email gian hàng đối tác" },
        new RouteDefinition { FeatureKey = "home_gianhang_space", Path = "/admin/duyet-gian-hang-doi-tac.aspx", Title = "Duyệt không gian gian hàng" },
        new RouteDefinition { FeatureKey = "home_gianhang_space", Path = "/admin/duyet-quyen-gianhang.aspx", Title = "Duyệt không gian gian hàng" },
        new RouteDefinition { FeatureKey = "home_gianhang_space", Path = "/admin/duyet-quyen-gianhang-admin.aspx", Title = "Duyệt không gian gian hàng" },
        new RouteDefinition { FeatureKey = "shop_partner", Path = "/admin/duyet-shop-doi-tac.aspx", Title = "Duyệt gian hàng đối tác (Shop)" },
        new RouteDefinition { FeatureKey = "shop_level2", Path = "/admin/duyet-nang-cap-level2.aspx", Title = "Duyệt nâng cấp Level 2" },
        new RouteDefinition { FeatureKey = "daugia_workspace", Path = "/daugia/admin", Title = "Trung tâm quản trị đấu giá" },
        new RouteDefinition { FeatureKey = "daugia_workspace", Path = "/daugia/admin/", Title = "Trung tâm quản trị đấu giá" },
        new RouteDefinition { FeatureKey = "daugia_workspace", Path = "/daugia/admin/default.aspx", Title = "Trung tâm quản trị đấu giá" },
        new RouteDefinition { FeatureKey = "daugia_workspace", Path = "/daugia/admin/portal.aspx", Title = "Trung tâm quản trị đấu giá" },
        new RouteDefinition { FeatureKey = "event_workspace", Path = "/event/admin", Title = "Trung tâm quản trị sự kiện" },
        new RouteDefinition { FeatureKey = "event_workspace", Path = "/event/admin/", Title = "Trung tâm quản trị sự kiện" },
        new RouteDefinition { FeatureKey = "event_workspace", Path = "/event/admin/default.aspx", Title = "Trung tâm quản trị sự kiện" },
        new RouteDefinition { FeatureKey = "home_config", Path = "/admin/cai-dat-trang-chu/default.aspx", Title = "Cài đặt trang chủ" },
        new RouteDefinition { FeatureKey = "home_content", Path = "/admin/quan-ly-noi-dung-home/default.aspx", Title = "Nội dung trang chủ Home" },
        new RouteDefinition { FeatureKey = "home_bds_linked", Path = "/admin/quan-ly-noi-dung-home/bds-lien-ket-tin.aspx", Title = "BĐS - Liên kết tin" },
        new RouteDefinition { FeatureKey = "home_menu", Path = "/admin/quan-ly-menu/default.aspx", Title = "Quản lý menu" },
        new RouteDefinition { FeatureKey = "home_menu", Path = "/admin/quan-ly-menu/in.aspx", Title = "Quản lý menu" },
        new RouteDefinition { FeatureKey = "home_menu", Path = "/admin/quan-ly-menu/them-moi.aspx", Title = "Quản lý menu" },
        new RouteDefinition { FeatureKey = "home_menu", Path = "/admin/quan-ly-menu/bo-loc.aspx", Title = "Quản lý menu" },
        new RouteDefinition { FeatureKey = "home_menu", Path = "/admin/quan-ly-menu/chinh-sua.aspx", Title = "Quản lý menu" },
        new RouteDefinition { FeatureKey = "home_menu", Path = "/admin/quan-ly-menu/xuat-du-lieu.aspx", Title = "Quản lý menu" },
        new RouteDefinition { FeatureKey = "home_menu", Path = "/admin/quan-ly-menu/ban-in.aspx", Title = "Quản lý menu" },
        new RouteDefinition { FeatureKey = "home_posts", Path = "/admin/quan-ly-bai-viet/default.aspx", Title = "Quản lý bài viết" },
        new RouteDefinition { FeatureKey = "home_posts", Path = "/admin/quan-ly-bai-viet/in.aspx", Title = "Quản lý bài viết" },
        new RouteDefinition { FeatureKey = "home_posts", Path = "/admin/quan-ly-bai-viet/them-moi.aspx", Title = "Quản lý bài viết" },
        new RouteDefinition { FeatureKey = "home_posts", Path = "/admin/quan-ly-bai-viet/bo-loc.aspx", Title = "Quản lý bài viết" },
        new RouteDefinition { FeatureKey = "home_posts", Path = "/admin/quan-ly-bai-viet/chinh-sua.aspx", Title = "Quản lý bài viết" },
        new RouteDefinition { FeatureKey = "home_posts", Path = "/admin/quan-ly-bai-viet/xuat-du-lieu.aspx", Title = "Quản lý bài viết" },
        new RouteDefinition { FeatureKey = "home_posts", Path = "/admin/quan-ly-bai-viet/ban-in.aspx", Title = "Quản lý bài viết" },
        new RouteDefinition { FeatureKey = "home_banner", Path = "/admin/quan-ly-banner/default.aspx", Title = "Quản lý banner" },
        new RouteDefinition { FeatureKey = "home_banner", Path = "/admin/quan-ly-banner/them-moi.aspx", Title = "Quản lý banner" },
        new RouteDefinition { FeatureKey = "token_wallet", Path = "/admin/vi-token-diem/default.aspx", Title = "Ví token điểm" },
        new RouteDefinition { FeatureKey = "token_wallet", Path = "/admin/api/usdt-bridge-credit.aspx", Title = "API nạp USDT bridge" },
        new RouteDefinition { FeatureKey = "change_password", Path = "/admin/doi-mat-khau/default.aspx", Title = "Đổi mật khẩu" },
        new RouteDefinition { FeatureKey = "forgot_password", Path = "/admin/quen-mat-khau/default.aspx", Title = "Quên mật khẩu" },
        new RouteDefinition { FeatureKey = "forgot_password", Path = "/admin/khoi-phuc-mat-khau.aspx", Title = "Quên mật khẩu" }
    };

    private static string NormalizePath(string path)
    {
        return (path ?? "").Trim().ToLowerInvariant();
    }

    private static RouteDefinition GetDefinitionByPath(string path)
    {
        string normalized = NormalizePath(path);
        if (normalized == "")
            return null;

        return Routes.FirstOrDefault(item => NormalizePath(item.Path) == normalized);
    }

    private static string ResolveFeatureKeyByPrefix(string normalizedPath)
    {
        if (string.IsNullOrWhiteSpace(normalizedPath))
            return "";

        if (!normalizedPath.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return "";

        if (normalizedPath.StartsWith("/admin/quan-ly-tai-khoan/", StringComparison.OrdinalIgnoreCase))
            return "admin_accounts";
        if (normalizedPath.StartsWith("/admin/otp/", StringComparison.OrdinalIgnoreCase))
            return "admin_otp";
        if (normalizedPath.StartsWith("/admin/quan-ly-gop-y/", StringComparison.OrdinalIgnoreCase))
            return "admin_feedback";
        if (normalizedPath.StartsWith("/admin/quan-ly-thong-bao/", StringComparison.OrdinalIgnoreCase))
            return "notifications";
        if (normalizedPath.StartsWith("/admin/yeu-cau-tu-van/", StringComparison.OrdinalIgnoreCase))
            return "consulting";
        if (normalizedPath.StartsWith("/admin/quan-ly-noi-dung-home/", StringComparison.OrdinalIgnoreCase))
            return "home_content";
        if (normalizedPath.StartsWith("/admin/cai-dat-trang-chu/", StringComparison.OrdinalIgnoreCase))
            return "home_config";
        if (normalizedPath.StartsWith("/admin/quan-ly-menu/", StringComparison.OrdinalIgnoreCase))
            return "home_menu";
        if (normalizedPath.StartsWith("/admin/quan-ly-bai-viet/", StringComparison.OrdinalIgnoreCase))
            return "home_posts";
        if (normalizedPath.StartsWith("/admin/quan-ly-banner/", StringComparison.OrdinalIgnoreCase))
            return "home_banner";
        if (normalizedPath.StartsWith("/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase))
            return "core_assets";
        if (normalizedPath.StartsWith("/admin/he-thong-san-pham/", StringComparison.OrdinalIgnoreCase))
            return "system_products";
        if (normalizedPath.StartsWith("/admin/phat-hanh-the", StringComparison.OrdinalIgnoreCase))
            return "issue_cards";
        if (normalizedPath.StartsWith("/admin/quan-ly-email-shop/", StringComparison.OrdinalIgnoreCase))
            return "shop_email";
        if (normalizedPath.StartsWith("/admin/tools/reindex-baiviet", StringComparison.OrdinalIgnoreCase))
            return "admin_reindex_baiviet";
        if (normalizedPath.StartsWith("/admin/tools/company-shop-sync", StringComparison.OrdinalIgnoreCase))
            return "admin_company_shop_sync";
        if (normalizedPath.StartsWith("/admin/vi-token-diem/", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.StartsWith("/admin/api/usdt-bridge-credit.aspx", StringComparison.OrdinalIgnoreCase))
            return "token_wallet";

        if (normalizedPath == "/admin/duyet-yeu-cau-len-cap.aspx")
            return "home_point_approval";
        if (normalizedPath == "/admin/motacapbac.aspx")
            return "tier_reference";
        if (normalizedPath == "/admin/duyet-gian-hang-doi-tac.aspx"
            || normalizedPath == "/admin/duyet-quyen-gianhang.aspx"
            || normalizedPath == "/admin/duyet-quyen-gianhang-admin.aspx")
            return "home_gianhang_space";
        if (normalizedPath == "/admin/duyet-shop-doi-tac.aspx")
            return "shop_partner";
        if (normalizedPath == "/admin/duyet-nang-cap-level2.aspx")
            return "shop_level2";
        if (normalizedPath == "/admin/trigger-seed-space.aspx")
            return "core_assets";
        if (normalizedPath == "/admin/default.aspx")
            return "admin_dashboard";
        if (normalizedPath == "/admin/khoi-phuc-mat-khau.aspx"
            || normalizedPath.StartsWith("/admin/quen-mat-khau/", StringComparison.OrdinalIgnoreCase))
            return "forgot_password";
        if (normalizedPath.StartsWith("/admin/doi-mat-khau/", StringComparison.OrdinalIgnoreCase))
            return "change_password";

        return "";
    }

    public static string ResolveFeatureKey(string path)
    {
        string normalized = NormalizePath(path);
        RouteDefinition definition = GetDefinitionByPath(normalized);
        if (definition != null)
            return definition.FeatureKey ?? "";

        return ResolveFeatureKeyByPrefix(normalized);
    }

    public static string ResolveFeatureKey(HttpRequest request)
    {
        if (request == null || request.Url == null)
            return "";

        string path = request.Url.AbsolutePath ?? "";
        string normalized = NormalizePath(path);

        if (normalized.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase)
            || normalized == "/gianhang/admin"
            || normalized == "/gianhang/admin/")
        {
            string gianHangFeature = GianHangSystemAdminGuard_cl.ResolveSystemFeatureKey(request);
            if (!string.IsNullOrWhiteSpace(gianHangFeature)
                && !string.Equals(gianHangFeature, "__gianhang_system_unknown_route__", StringComparison.OrdinalIgnoreCase))
                return gianHangFeature;
        }

        string featureKey = ResolveFeatureKey(path);

        if (normalized.StartsWith("/admin/quan-ly-tai-khoan/", StringComparison.OrdinalIgnoreCase))
        {
            string scope = AdminDataScope_cl.ResolveEffectiveAccountScope(
                request.QueryString["scope"],
                request.QueryString["fscope"]);
            return AdminDataScope_cl.ResolveAccountFeatureByScope(scope);
        }

        if (normalized.StartsWith("/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase))
            return AdminDataScope_cl.ResolveTransferFeatureByTab(request.QueryString["tab"]);

        if (normalized == "/admin/default.aspx")
        {
            string section = (request.QueryString["section"] ?? "").Trim().ToLowerInvariant();
            if (section == "vi-token-diem")
                return "token_wallet";
            return "admin_dashboard";
        }

        return featureKey;
    }

    public static string ResolveTitle(HttpRequest request)
    {
        if (request == null || request.Url == null)
            return "Trang chủ admin";

        string path = request.Url.AbsolutePath ?? "";
        string normalized = NormalizePath(path);
        string title = ResolveTitle(path);

        if (normalized.StartsWith("/admin/quan-ly-tai-khoan/", StringComparison.OrdinalIgnoreCase))
        {
            string scope = AdminDataScope_cl.ResolveEffectiveAccountScope(
                request.QueryString["scope"],
                request.QueryString["fscope"]);
            if (scope == AdminDataScope_cl.AccountScopeAdmin) return "Quản lý tài khoản admin";
            if (scope == AdminDataScope_cl.AccountScopeHome) return "Quản lý tài khoản home";
            if (scope == AdminDataScope_cl.AccountScopeShop) return "Quản lý tài khoản gian hàng đối tác";
        }

        if (normalized.StartsWith("/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase))
        {
            string tab = AdminDataScope_cl.NormalizeTransferTab(request.QueryString["tab"]);
            if (tab == AdminDataScope_cl.TransferTabCustomer) return "Duyệt điểm hồ sơ Khách hàng";
            if (tab == AdminDataScope_cl.TransferTabDevelopment) return "Duyệt điểm hồ sơ Cộng tác phát triển";
            if (tab == AdminDataScope_cl.TransferTabEcosystem) return "Duyệt điểm hồ sơ Đồng hành hệ sinh thái";
            if (tab == AdminDataScope_cl.TransferTabShopOnly) return "Duyệt điểm / nghiệp vụ shop";
        }

        if (normalized == "/admin/default.aspx")
        {
            string section = (request.QueryString["section"] ?? "").Trim().ToLowerInvariant();
            if (section == "vi-token-diem")
                return "Ví token điểm";
        }

        return title;
    }

    public static string ResolveTitle(string path)
    {
        RouteDefinition definition = GetDefinitionByPath(path);
        if (definition != null)
            return definition.Title ?? "Trang chủ admin";

        string featureKey = ResolveFeatureKey(path);
        AdminFeatureRegistry_cl.FeatureDefinition feature = AdminFeatureRegistry_cl.Get(featureKey);
        if (feature != null && !string.IsNullOrWhiteSpace(feature.Title))
            return feature.Title;

        return "Trang chủ admin";
    }

    public static bool IsPathActive(HttpRequest request, params string[] paths)
    {
        if (request == null || request.Url == null || paths == null || paths.Length == 0)
            return false;

        string current = NormalizePath(request.Url.AbsolutePath);
        return paths.Any(path => current == NormalizePath(path));
    }

    public static bool IsTransferHistoryActive(HttpRequest request)
    {
        if (!IsPathActive(request, "/admin/lich-su-chuyen-diem/default.aspx", "/admin/lich-su-chuyen-diem/chuyen-diem.aspx"))
            return false;

        string tab = ((request.QueryString["tab"] ?? "")).Trim().ToLowerInvariant();
        return AdminDataScope_cl.NormalizeTransferTab(tab) == "";
    }

    public static bool IsPointApprovalActive(HttpRequest request)
    {
        if (IsPathActive(request, "/admin/duyet-yeu-cau-len-cap.aspx"))
            return true;

        if (!IsPathActive(request, "/admin/lich-su-chuyen-diem/default.aspx", "/admin/lich-su-chuyen-diem/chuyen-diem.aspx"))
            return false;

        string tab = AdminDataScope_cl.NormalizeTransferTab(request.QueryString["tab"]);
        return tab == AdminDataScope_cl.TransferTabCustomer
            || tab == AdminDataScope_cl.TransferTabDevelopment
            || tab == AdminDataScope_cl.TransferTabEcosystem;
    }

    public static bool IsShopPointApprovalActive(HttpRequest request)
    {
        if (!IsPathActive(request, "/admin/lich-su-chuyen-diem/default.aspx", "/admin/lich-su-chuyen-diem/chuyen-diem.aspx"))
            return false;

        return AdminDataScope_cl.NormalizeTransferTab(request.QueryString["tab"]) == AdminDataScope_cl.TransferTabShopOnly;
    }

    public static bool IsAccountScopeActive(HttpRequest request, string scope)
    {
        if (!IsPathActive(request,
            "/admin/quan-ly-tai-khoan/default.aspx",
            "/admin/quan-ly-tai-khoan/them-moi.aspx",
            "/admin/quan-ly-tai-khoan/bo-loc.aspx",
            "/admin/quan-ly-tai-khoan/chinh-sua.aspx",
            "/admin/quan-ly-tai-khoan/phan-quyen.aspx"))
            return false;

        string currentScope = AdminDataScope_cl.ResolveEffectiveAccountScope(
            request.QueryString["scope"],
            request.QueryString["fscope"]);
        string targetScope = AdminDataScope_cl.NormalizeAccountScope(scope);
        return string.Equals(currentScope, targetScope, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsTokenWalletActive(HttpRequest request)
    {
        if (IsPathActive(request, "/admin/vi-token-diem/default.aspx"))
            return true;

        if (!IsPathActive(request, "/admin/default.aspx"))
            return false;

        return string.Equals((request.QueryString["section"] ?? "").Trim(), "vi-token-diem", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFeatureActive(HttpRequest request, params string[] featureKeys)
    {
        if (request == null || featureKeys == null || featureKeys.Length == 0)
            return false;

        string currentFeature = ResolveFeatureKey(request);
        if (string.IsNullOrWhiteSpace(currentFeature))
            return false;

        foreach (string featureKey in featureKeys)
        {
            if (string.Equals((featureKey ?? "").Trim(), currentFeature, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    public static IList<RouteDefinition> GetAll()
    {
        return Routes;
    }
}
