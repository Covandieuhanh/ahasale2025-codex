using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public static class AdminManagementSpace_cl
{
    public const string SpaceAdmin = "admin";
    public const string SpaceHome = "home";
    public const string SpaceGianHang = "gianhang";
    public const string SpaceBatDongSan = "batdongsan";
    public const string SpaceDauGia = "daugia";
    public const string SpaceEvent = "event";
    public const string SpaceContent = "content";

    private const string SessionKey = "admin_management_space";

    public sealed class SpaceInfo
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string SwitchUrl { get; set; }
        public string LandingUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public static string NormalizeSpace(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        switch (value)
        {
            case SpaceAdmin:
            case SpaceHome:
            case SpaceGianHang:
            case SpaceBatDongSan:
            case SpaceDauGia:
            case SpaceEvent:
            case SpaceContent:
                return value;
            default:
                return "";
        }
    }

    public static string GetTitle(string space)
    {
        switch (NormalizeSpace(space))
        {
            case SpaceAdmin:
                return "Quản trị admin";
            case SpaceHome:
                return "Quản trị không gian Home";
            case SpaceGianHang:
                return "Quản trị không gian Gian hàng đối tác";
            case SpaceBatDongSan:
                return "Quản trị không gian Bất động sản";
            case SpaceDauGia:
                return "Quản trị không gian Đấu giá";
            case SpaceEvent:
                return "Quản trị không gian Sự kiện";
            case SpaceContent:
                return "Quản trị nội dung Website";
            default:
                return "Không gian quản trị";
        }
    }

    public static string GetSubtitle(string space)
    {
        switch (NormalizeSpace(space))
        {
            case SpaceAdmin:
                return "Quản lý dashboard admin, tài khoản admin, OTP, ví token điểm, góp ý, thông báo, tư vấn và các công cụ vận hành nội bộ của hệ quản trị.";
            case SpaceHome:
                return "Quản lý tài khoản Home, điểm, hành vi, phát hành thẻ và các thao tác lõi của hệ Home.";
            case SpaceGianHang:
                return "Quản lý mở không gian gian hàng, tài khoản shop, duyệt shop và vận hành gian hàng đối tác.";
            case SpaceBatDongSan:
                return "Đi tới trung tâm quản trị bất động sản và chỉ hiển thị các tab liên quan đến dữ liệu, feed và vận hành nội dung BĐS.";
            case SpaceDauGia:
                return "Đi tới trung tâm quản trị đấu giá và chỉ hiển thị các tab liên quan đến đấu giá.";
            case SpaceEvent:
                return "Đi tới trung tâm quản trị sự kiện và chỉ hiển thị các tab liên quan đến sự kiện.";
            case SpaceContent:
                return "Cập nhật giao diện, menu, banner, bài viết và các thông tin hiển thị trên website.";
            default:
                return "Chọn đúng không gian quản trị để chỉ thao tác trong phạm vi được phân quyền.";
        }
    }

    public static bool IsInternalAdminSpace(string space)
    {
        string normalized = NormalizeSpace(space);
        return normalized == SpaceAdmin
            || normalized == SpaceHome
            || normalized == SpaceGianHang
            || normalized == SpaceBatDongSan
            || normalized == SpaceContent;
    }

    public static string AppendSpaceToUrl(string url, string space)
    {
        string normalized = NormalizeSpace(space);
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(normalized))
            return url ?? "";

        string safeUrl = url ?? "";
        string separator = safeUrl.Contains("?") ? "&" : "?";
        if (safeUrl.IndexOf("mspace=", StringComparison.OrdinalIgnoreCase) >= 0)
            return safeUrl;

        return safeUrl + separator + "mspace=" + HttpUtility.UrlEncode(normalized);
    }

    public static bool CanAccessSpace(dbDataContext db, taikhoan_tb account, string space)
    {
        if (db == null || account == null)
            return false;

        string normalized = NormalizeSpace(space);
        if (string.IsNullOrWhiteSpace(normalized))
            return false;

        string tk = (account.taikhoan ?? "").Trim();
        string permissionRaw = account.permission ?? "";

        switch (normalized)
        {
            case SpaceAdmin:
                return AdminRolePolicy_cl.CanAccessAdminWorkspace(db, tk);

            case SpaceHome:
                return AdminRolePolicy_cl.IsSuperAdmin(tk)
                    || AdminRolePolicy_cl.CanAccessTransferHistory(db, tk)
                    || AdminRolePolicy_cl.CanManageHomeAccounts(db, tk)
                    || AdminRolePolicy_cl.CanReviewHomePointRequests(db, tk)
                    || AdminRolePolicy_cl.CanIssueCards(db, tk)
                    || AdminRolePolicy_cl.CanViewTierReference(db, tk)
                    || AdminRolePolicy_cl.CanSellSystemProducts(db, tk);

            case SpaceGianHang:
                return AdminRolePolicy_cl.IsSuperAdmin(tk)
                    || AdminRolePolicy_cl.CanManageShopAccounts(db, tk)
                    || AdminRolePolicy_cl.CanReviewShopPointRequests(db, tk)
                    || AdminRolePolicy_cl.CanApproveShopPartnerRegistration(db, tk)
                    || AdminRolePolicy_cl.CanApproveShopLevel2(db, tk)
                    || AdminRolePolicy_cl.CanManageShopOperations(db, tk);

            case SpaceBatDongSan:
                return AdminRolePolicy_cl.IsSuperAdmin(tk)
                    || AdminRolePolicy_cl.CanManageHomeBdsLinked(db, tk);

            case SpaceDauGia:
                return AdminRolePolicy_cl.IsSuperAdmin(tk)
                    || DauGiaPolicy_cl.CanAccessAdmin(db, tk, permissionRaw);

            case SpaceEvent:
                return AdminRolePolicy_cl.IsSuperAdmin(tk)
                    || EventPolicy_cl.CanViewAdminWorkspace(db, tk);

            case SpaceContent:
                return AdminRolePolicy_cl.IsSuperAdmin(tk)
                    || AdminRolePolicy_cl.CanManageHomeConfig(db, tk)
                    || AdminRolePolicy_cl.CanManageHomeContent(db, tk)
                    || AdminRolePolicy_cl.CanManageHomePosts(db, tk)
                    || AdminRolePolicy_cl.CanManageHomeMenu(db, tk)
                    || AdminRolePolicy_cl.CanManageHomeBanner(db, tk);

            default:
                return false;
        }
    }

    public static List<SpaceInfo> GetAllowedSpaces(dbDataContext db, taikhoan_tb account, HttpRequest request)
    {
        var result = new List<SpaceInfo>();
        if (db == null || account == null)
            return result;

        string current = GetCurrentSpace(db, account, request);
        string[] ordered = new[] { SpaceAdmin, SpaceHome, SpaceGianHang, SpaceBatDongSan, SpaceDauGia, SpaceEvent, SpaceContent };

        foreach (string space in ordered)
        {
            if (!CanAccessSpace(db, account, space))
                continue;

            string landingUrl = ResolveLandingUrl(db, account.taikhoan, space);
            result.Add(new SpaceInfo
            {
                Key = space,
                Title = GetTitle(space),
                Subtitle = GetSubtitle(space),
                SwitchUrl = landingUrl,
                LandingUrl = landingUrl,
                IsActive = string.Equals(current, space, StringComparison.OrdinalIgnoreCase)
            });
        }

        return result;
    }

    public static string ResolveDefaultSpace(dbDataContext db, taikhoan_tb account)
    {
        if (db == null || account == null)
            return "";

        string[] ordered = new[] { SpaceAdmin, SpaceHome, SpaceGianHang, SpaceBatDongSan, SpaceDauGia, SpaceEvent, SpaceContent };
        foreach (string space in ordered)
        {
            if (CanAccessSpace(db, account, space))
                return space;
        }

        return "";
    }

    public static string GetCurrentSpace(dbDataContext db, taikhoan_tb account, HttpRequest request)
    {
        if (db == null || account == null)
            return "";

        if (IsAdminDashboardRoute(request) && CanAccessSpace(db, account, SpaceAdmin))
        {
            SetCurrentSpace(SpaceAdmin);
            return SpaceAdmin;
        }

        string querySpace = request != null ? NormalizeSpace(request.QueryString["mspace"]) : "";
        if (CanAccessSpace(db, account, querySpace))
        {
            SetCurrentSpace(querySpace);
            return querySpace;
        }

        string sessionSpace = NormalizeSpace(GetSessionValue());
        if (CanAccessSpace(db, account, sessionSpace))
            return sessionSpace;

        string routeSpace = ResolveRouteSpace(request);
        if (CanAccessSpace(db, account, routeSpace))
        {
            SetCurrentSpace(routeSpace);
            return routeSpace;
        }

        string defaultSpace = ResolveDefaultSpace(db, account);
        SetCurrentSpace(defaultSpace);
        return defaultSpace;
    }

    public static string ResolveCurrentLandingUrl(dbDataContext db, taikhoan_tb account, HttpRequest request)
    {
        if (db == null || account == null)
            return "/admin/login.aspx";

        string current = GetCurrentSpace(db, account, request);
        return ResolveLandingUrl(db, account.taikhoan, current);
    }

    public static string ResolveLandingUrl(dbDataContext db, string taikhoan, string requestedSpace)
    {
        if (db == null || string.IsNullOrWhiteSpace(taikhoan))
            return "/admin/login.aspx";

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taikhoan);
        if (account == null)
            return "/admin/login.aspx";

        string space = NormalizeSpace(requestedSpace);
        if (!CanAccessSpace(db, account, space))
            space = ResolveDefaultSpace(db, account);

        if (string.IsNullOrWhiteSpace(space))
            return "/admin/login.aspx";

        switch (space)
        {
            case SpaceAdmin:
                return AppendSpaceToUrl("/admin/default.aspx", SpaceAdmin);

            case SpaceHome:
                return AppendSpaceToUrl("/admin/default.aspx", SpaceHome);

            case SpaceGianHang:
                return AppendSpaceToUrl("/admin/default.aspx", SpaceGianHang);

            case SpaceBatDongSan:
                return AppendSpaceToUrl("/admin/default.aspx", SpaceBatDongSan);

            case SpaceDauGia:
                return AppendSpaceToUrl("/admin/default.aspx", SpaceDauGia);

            case SpaceEvent:
                return AppendSpaceToUrl("/admin/default.aspx", SpaceEvent);

            case SpaceContent:
                return AppendSpaceToUrl("/admin/default.aspx", SpaceContent);

            default:
                return "/admin/default.aspx";
        }
    }

    public static bool EnsureSelectedSpaceRouteAccess(Page page)
    {
        if (page == null || page.Request == null)
            return true;

        HttpRequest request = page.Request;
        string routeSpace = ResolveRouteSpace(request);
        if (string.IsNullOrWhiteSpace(routeSpace))
            return true;

        string tk = AdminRolePolicy_cl.GetCurrentAdminUser();
        if (string.IsNullOrWhiteSpace(tk))
            return true;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (account == null)
                return true;

            string selected = GetCurrentSpace(db, account, request);
            if (string.IsNullOrWhiteSpace(selected) || string.IsNullOrWhiteSpace(routeSpace))
                return true;

            if (string.Equals(selected, routeSpace, StringComparison.OrdinalIgnoreCase))
                return true;

            string target = ResolveLandingUrl(db, tk, selected);
            if (string.IsNullOrWhiteSpace(target))
                return true;

            page.Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Bạn đang làm việc trong không gian quản trị khác. Hệ thống đã đưa bạn về đúng phạm vi được chọn.",
                "1500",
                "warning");

            page.Response.Redirect(target, false);
            HttpContext currentContext = HttpContext.Current;
            if (currentContext != null && currentContext.ApplicationInstance != null)
                currentContext.ApplicationInstance.CompleteRequest();
            return false;
        }
    }

    public static string ResolveRouteSpace(HttpRequest request)
    {
        if (request == null)
            return "";

        string path = GetNormalizedExecutionPath(request);
        if (string.IsNullOrWhiteSpace(path))
            return "";

        if (IsNeutralManagedRoute(path))
            return "";

        if (path.StartsWith("~/daugia/admin/", StringComparison.OrdinalIgnoreCase))
        {
            if (IsSystemAdminMode(request))
                return SpaceDauGia;
            return "";
        }

        if (path.StartsWith("~/event/admin/", StringComparison.OrdinalIgnoreCase))
        {
            if (IsSystemAdminMode(request))
                return SpaceEvent;
            return "";
        }

        if (!path.StartsWith("~/admin/", StringComparison.OrdinalIgnoreCase))
            return "";

        if (string.Equals(path, "~/admin/quan-ly-noi-dung-home/bds-lien-ket-tin.aspx", StringComparison.OrdinalIgnoreCase))
            return SpaceBatDongSan;

        if (path.StartsWith("~/admin/cai-dat-trang-chu/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-noi-dung-home/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-menu/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-bai-viet/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-banner/", StringComparison.OrdinalIgnoreCase))
            return SpaceContent;

        if (path.StartsWith("~/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
        {
            if (IsSystemAdminMode(request))
                return SpaceGianHang;
            return "";
        }

        if (path.StartsWith("~/admin/quan-ly-email-shop/", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/duyet-gian-hang-doi-tac.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/duyet-shop-doi-tac.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/duyet-nang-cap-level2.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/duyet-quyen-gianhang.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/duyet-quyen-gianhang-admin.aspx", StringComparison.OrdinalIgnoreCase))
            return SpaceGianHang;

        if (path.StartsWith("~/admin/quan-ly-tai-khoan/", StringComparison.OrdinalIgnoreCase))
        {
            string scope = AdminDataScope_cl.ResolveEffectiveAccountScope(
                request.QueryString["scope"],
                request.QueryString["fscope"]);
            if (scope == AdminDataScope_cl.AccountScopeAdmin)
                return SpaceAdmin;
            if (scope == AdminDataScope_cl.AccountScopeHome)
                return SpaceHome;
            if (scope == AdminDataScope_cl.AccountScopeShop)
                return SpaceGianHang;
            return "";
        }

        if (path.StartsWith("~/admin/otp/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-gop-y/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-thong-bao/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/yeu-cau-tu-van/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/vi-token-diem/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/tools/company-shop-sync.aspx", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/tools/reindex-baiviet.aspx", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/api/usdt-bridge-credit.aspx", StringComparison.OrdinalIgnoreCase))
            return SpaceAdmin;

        if (string.Equals(path, "~/admin/default.aspx", StringComparison.OrdinalIgnoreCase))
        {
            string querySpace = NormalizeSpace(request.QueryString["mspace"]);
            if (!string.IsNullOrWhiteSpace(querySpace))
                return querySpace;

            return SpaceAdmin;
        }

        if (path.StartsWith("~/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase))
        {
            string tab = AdminDataScope_cl.NormalizeTransferTab(request.QueryString["tab"]);
            if (tab == AdminDataScope_cl.TransferTabShopOnly)
                return SpaceGianHang;
            return SpaceHome;
        }

        if (string.Equals(path, "~/admin/duyet-yeu-cau-len-cap.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/motacapbac.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/phat-hanh-the.aspx", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/he-thong-san-pham/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/phat-hanh-the/", StringComparison.OrdinalIgnoreCase))
            return SpaceHome;

        return "";
    }

    private static string BuildSwitchUrl(string space)
    {
        return "/admin/default.aspx?mspace=" + HttpUtility.UrlEncode(NormalizeSpace(space));
    }

    private static bool IsNeutralAdminRoute(string path)
    {
        return string.Equals(path, "~/admin/login.aspx", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/doi-mat-khau/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quen-mat-khau/", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/khoi-phuc-mat-khau.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsNeutralManagedRoute(string path)
    {
        if (IsNeutralAdminRoute(path))
            return true;

        return string.Equals(path, "~/gianhang/admin/login.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/gianhang/admin/f5_ss_admin.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminDashboardRoute(HttpRequest request)
    {
        if (request == null)
            return false;

        string querySpace = NormalizeSpace(request.QueryString["mspace"]);
        if (!string.IsNullOrWhiteSpace(querySpace))
            return false;

        string path = GetNormalizedExecutionPath(request);
        return string.Equals(path, "~/admin/default.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSystemAdminMode(HttpRequest request)
    {
        if (request == null)
            return false;

        string raw = (request.QueryString["system"] ?? "").Trim().ToLowerInvariant();
        return raw == "1" || raw == "true";
    }

    private static string GetNormalizedExecutionPath(HttpRequest request)
    {
        if (request == null)
            return "";

        string path = (request.AppRelativeCurrentExecutionFilePath ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(path))
            return path;

        string absolute = (request.Path ?? "").Trim();
        if (string.IsNullOrWhiteSpace(absolute))
            return "";

        try
        {
            return VirtualPathUtility.ToAppRelative(absolute).Trim().ToLowerInvariant();
        }
        catch
        {
            return absolute.ToLowerInvariant();
        }
    }

    private static void SetCurrentSpace(string space)
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null)
            return;

        context.Session[SessionKey] = NormalizeSpace(space);
    }

    private static string GetSessionValue()
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null)
            return "";

        object value = context.Session[SessionKey];
        return value == null ? "" : value.ToString();
    }
}
