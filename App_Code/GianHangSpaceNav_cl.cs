using System;
using System.Web;

public static class GianHangSpaceNav_cl
{
    public sealed class SpaceNavModel
    {
        public bool Visible { get; set; }
        public string StoreName { get; set; }
        public string AccountKey { get; set; }
        public string StatusText { get; set; }
        public string AvatarUrl { get; set; }
        public string PublicUrl { get; set; }
        public string HomeUrl { get; set; }
        public string ManageUrl { get; set; }
        public string OrdersUrl { get; set; }
        public string BookingUrl { get; set; }
        public string CustomersUrl { get; set; }
        public string ReportUrl { get; set; }
        public string Level2Url { get; set; }
        public bool ShowAdminUrl { get; set; }
        public string AdminUrl { get; set; }
        public string ActiveKey { get; set; }
    }

    public static SpaceNavModel BuildCurrent(HttpRequest request)
    {
        string path = request == null || request.Url == null
            ? string.Empty
            : (request.Url.AbsolutePath ?? string.Empty).Trim();

        if (!path.StartsWith("/gianhang", StringComparison.OrdinalIgnoreCase))
            return new SpaceNavModel();

        if (path.StartsWith("/gianhang/admin", StringComparison.OrdinalIgnoreCase))
            return new SpaceNavModel();

        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated || !info.CanAccessGianHang)
            return new SpaceNavModel();

        return CoreDb_cl.Use(db =>
        {
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
            GianHangStorefront_cl.StorefrontSummary summary = GianHangStorefront_cl.BuildSummary(db, account, request == null ? null : request.Url);

            return new SpaceNavModel
            {
                Visible = true,
                StoreName = (summary.StorefrontName ?? "").Trim(),
                AccountKey = (info.AccountKey ?? "").Trim(),
                StatusText = "Đang hoạt động",
                AvatarUrl = GianHangStorefront_cl.ResolveAvatarUrl(account),
                PublicUrl = summary.ProfileUrl,
                HomeUrl = "/gianhang/tai-khoan/default.aspx",
                ManageUrl = GianHangRoutes_cl.BuildOwnerContentUrl(),
                OrdersUrl = GianHangRoutes_cl.BuildOwnerOrdersUrl(),
                BookingUrl = GianHangRoutes_cl.BuildOwnerBookingsUrl(),
                CustomersUrl = GianHangRoutes_cl.BuildOwnerCustomersUrl(),
                ReportUrl = GianHangRoutes_cl.BuildOwnerReportsUrl(),
                Level2Url = GianHangRoutes_cl.BuildBridgeLevel2Url(),
                ShowAdminUrl = info.CanAccessGianHangAdmin,
                AdminUrl = "/gianhang/admin",
                ActiveKey = ResolveActiveKey(path)
            };
        });
    }

    private static string ResolveActiveKey(string path)
    {
        string normalized = (path ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized.StartsWith("/gianhang/quan-ly-tin"))
            return "manage";
        if (normalized.StartsWith("/gianhang/don-ban") || normalized.StartsWith("/gianhang/cho-thanh-toan"))
            return "orders";
        if (normalized.StartsWith("/gianhang/quan-ly-lich-hen") || normalized.StartsWith("/gianhang/dat-lich"))
            return "booking";
        if (normalized.StartsWith("/gianhang/khach-hang"))
            return "customers";
        if (normalized.StartsWith("/gianhang/bao-cao"))
            return "report";
        if (normalized.StartsWith("/gianhang/nang-cap-level2"))
            return "level2";
        if (normalized.StartsWith("/gianhang/tai-khoan"))
            return "home";
        if (normalized.StartsWith("/gianhang/public")
            || normalized.StartsWith("/gianhang/page/")
            || normalized.StartsWith("/gianhang/xem-san-pham")
            || normalized.StartsWith("/gianhang/xem-dich-vu")
            || normalized.StartsWith("/gianhang/giohang"))
            return "public";
        return string.Empty;
    }
}
