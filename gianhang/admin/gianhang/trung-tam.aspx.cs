using System;
using System.Web;

public partial class gianhang_admin_gianhang_trung_tam : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string WorkspaceDisplayName = "Trung tâm /gianhang";
    public string HomeAccountKey = "";
    public string OwnerDisplayName = "";
    public string ContactEmail = "Chưa cập nhật";
    public string AccountTypeLabel = "Tài khoản Home";
    public string RoleLabel = "Owner";
    public string AdminAccessText = "Đã mở quyền";
    public string PublicUrl = "/gianhang/public.aspx";
    public string PublicAbsoluteUrl = "/gianhang/public.aspx";
    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string StorefrontPreviewUrl = "/gianhang/admin/gianhang/trang-cong-khai.aspx";
    public string NativeCenterUrl = "/gianhang/tai-khoan/default.aspx";
    public string HomeProfileUrl = "/home/edit-info.aspx";
    public string ChangePasswordUrl = "/home/DoiMatKhau.aspx";
    public string AdminSettingsUrl = "/gianhang/admin/cai-dat-chung/default.aspx";
    public string ProfileSettingsUrl = "/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string ReportUrl = "/gianhang/admin/gianhang/bao-cao.aspx";
    public string RevenueText = "0 đ";
    public int ProductCount;
    public int ServiceCount;
    public int PendingOrderCount;
    public int BookingTotal;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        HomeAccountKey = ownerAccountKey;
        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        StorefrontPreviewUrl = GianHangRoutes_cl.BuildAdminWorkspaceStorefrontUrl();
        NativeCenterUrl = "/gianhang/tai-khoan/default.aspx";
        HomeProfileUrl = "/home/edit-info.aspx";
        ChangePasswordUrl = "/home/DoiMatKhau.aspx?return_url=" + HttpUtility.UrlEncode(GianHangRoutes_cl.BuildAdminWorkspaceAccountCenterUrl());
        AdminSettingsUrl = "/gianhang/admin/cai-dat-chung/default.aspx";
        ProfileSettingsUrl = "/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx";
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        ReportUrl = GianHangRoutes_cl.BuildAdminWorkspaceReportUrl();
        RoleLabel = string.IsNullOrWhiteSpace(access.RoleLabel) ? "Owner" : access.RoleLabel;

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccountKey);
        RootAccount_cl.RootAccountInfo ownerInfo = RootAccount_cl.GetInfo(db, ownerAccountKey);
        GianHangStorefront_cl.StorefrontSummary summary = GianHangStorefront_cl.BuildSummary(db, owner, Request.Url);
        GianHangReport_cl.Dashboard dashboard = GianHangReport_cl.BuildDashboard(
            db,
            ownerAccountKey,
            GianHangReport_cl.ResolveDateRange("", "", "", "all"));

        WorkspaceDisplayName = GianHangStorefront_cl.ResolveStorefrontName(owner);
        OwnerDisplayName = ResolveText(ownerInfo == null ? "" : ownerInfo.FullName, WorkspaceDisplayName);
        ContactEmail = ResolveText(RootAccount_cl.ResolveContactEmail(ownerInfo), "Chưa cập nhật");
        AccountTypeLabel = ResolveText(ownerInfo == null ? "" : ownerInfo.AccountType, "Tài khoản Home");
        AdminAccessText = ownerInfo != null && ownerInfo.CanAccessGianHangAdmin ? "Đã mở quyền" : "Chưa mở quyền";

        PublicUrl = summary.ProfileUrl;
        PublicAbsoluteUrl = ResolveText(summary.ProfileAbsoluteUrl, summary.ProfileUrl);

        ProductCount = summary.ProductCount;
        ServiceCount = summary.ServiceCount;
        PendingOrderCount = summary.PendingOrderCount;
        BookingTotal = dashboard.BookingTotal;
        RevenueText = GianHangReport_cl.FormatCurrency(dashboard.RevenueGross) + " đ";
    }

    private static string ResolveText(string value, string fallback)
    {
        string text = (value ?? "").Trim();
        return text == "" ? fallback : text;
    }
}
