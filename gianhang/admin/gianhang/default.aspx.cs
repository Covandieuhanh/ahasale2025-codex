using System;
using System.Collections.Generic;
using System.Linq;

public partial class gianhang_admin_gianhang_default : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string WorkspaceDisplayName = "Không gian /gianhang";
    public string PublicUrl = "/gianhang/public.aspx";
    public string AccountCenterUrl = "/gianhang/admin/gianhang/trung-tam.aspx";
    public string PresentationUrl = "/gianhang/admin/gianhang/trinh-bay.aspx";
    public string ArticlesUrl = "/gianhang/admin/gianhang/bai-viet.aspx";
    public string ProductsUrl = "/gianhang/admin/gianhang/san-pham.aspx";
    public string ServicesUrl = "/gianhang/admin/gianhang/dich-vu.aspx";
    public string StorefrontUrl = "/gianhang/admin/gianhang/trang-cong-khai.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string ReportUrl = "/gianhang/admin/gianhang/bao-cao.aspx";
    public string UtilityUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string CreateFlowUrl = "/gianhang/admin/gianhang/tao-giao-dich.aspx";
    public string WaitUrl = "/gianhang/admin/gianhang/cho-thanh-toan.aspx";
    public string BuyerFlowUrl = "/gianhang/admin/gianhang/don-mua.aspx";
    public string CartUrl = "/gianhang/admin/gianhang/gio-hang.aspx";
    public string ElectronicInvoiceUrl = "/gianhang/admin/gianhang/hoa-don-dien-tu.aspx";
    public string LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
    public string PersonHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
    public string SyncUrl = "/gianhang/admin/gianhang/default.aspx?sync=1";
    public int ProductCount;
    public int ServiceCount;
    public int TotalOrders;
    public int WaitingExchange;
    public int BookingTotal;
    public int CustomerCount;
    public int LegacyMirroredInvoiceCount;
    public decimal RevenueGross;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        if (string.Equals((Request.QueryString["sync"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase))
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey, true);
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceHubUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        AccountCenterUrl = GianHangRoutes_cl.BuildAdminWorkspaceAccountCenterUrl();
        PresentationUrl = GianHangRoutes_cl.BuildAdminWorkspacePresentationUrl();
        ArticlesUrl = GianHangRoutes_cl.BuildAdminWorkspaceArticlesUrl();
        ProductsUrl = GianHangRoutes_cl.BuildAdminWorkspaceProductsUrl();
        ServicesUrl = GianHangRoutes_cl.BuildAdminWorkspaceServicesUrl();
        StorefrontUrl = GianHangRoutes_cl.BuildAdminWorkspaceStorefrontUrl();
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        ReportUrl = GianHangRoutes_cl.BuildAdminWorkspaceReportUrl();
        UtilityUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        CreateFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceCreateUrl();
        WaitUrl = GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl();
        BuyerFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl();
        CartUrl = GianHangRoutes_cl.BuildAdminWorkspaceCartUrl();
        ElectronicInvoiceUrl = GianHangRoutes_cl.BuildAdminWorkspaceElectronicInvoiceUrl();
        PublicUrl = GianHangRoutes_cl.BuildPublicUrl(ownerAccountKey);
        LegacyInvoiceUrl = GianHangRoutes_cl.BuildAdminLegacyInvoiceListUrl();
        PersonHubUrl = GianHangRoutes_cl.BuildAdminLegacyPeopleHubUrl(string.Empty);
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl() + "?sync=1";

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccountKey);
        string display = owner == null ? "" : ((owner.ten_shop ?? "").Trim());
        if (display == "")
            display = owner == null ? "" : ((owner.hoten ?? "").Trim());
        if (display == "")
            display = ownerAccountKey;
        WorkspaceDisplayName = display;

        GianHangReport_cl.ReportRange allTimeRange = GianHangReport_cl.ResolveDateRange("", "", "", "all");
        GianHangReport_cl.Dashboard report = GianHangReport_cl.BuildDashboard(db, ownerAccountKey, allTimeRange);
        ProductCount = report.ProductCount;
        ServiceCount = report.ServiceCount;
        TotalOrders = report.TotalOrders;
        WaitingExchange = report.WaitingExchange;
        BookingTotal = report.BookingTotal;
        RevenueGross = report.RevenueGross;
        CustomerCount = GianHangCustomer_cl.CountCustomers(db, ownerAccountKey);
        LegacyMirroredInvoiceCount = db.bspa_hoadon_tables
            .Where(p => p.user_parent == ownerAccountKey && p.id_chinhanh == access.ChiNhanhId)
            .ToList()
            .Count(p => GianHangWorkspaceLink_cl.IsWorkspaceMirrorSource((p.nguongoc ?? "") + ""));

        List<GianHangCustomer_cl.CustomerRow> customers = GianHangCustomer_cl.LoadCustomers(db, ownerAccountKey, "", 6);
        ph_no_customers.Visible = customers.Count == 0;
        rp_recent_customers.DataSource = customers;
        rp_recent_customers.DataBind();
    }
}
