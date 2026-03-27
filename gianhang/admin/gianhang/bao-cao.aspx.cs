using System;
using System.Collections.Generic;
using System.Linq;

public partial class gianhang_admin_gianhang_bao_cao : System.Web.UI.Page
{
    private sealed class RecentOrderView
    {
        public string DisplayId { get; set; }
        public string OrderedAtText { get; set; }
        public string BuyerDisplay { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string TotalAmountText { get; set; }
        public string DetailUrl { get; set; }
    }

    private sealed class RecentBookingView
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string ServiceName { get; set; }
        public string BookingTimeText { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string DetailUrl { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();
    private string ownerAccountKey = "";
    private string branchId = "";

    public string PublicUrl = "/gianhang/public.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string WaitUrl = "/gianhang/admin/gianhang/cho-thanh-toan.aspx";
    public string BuyerFlowUrl = "/gianhang/admin/gianhang/don-mua.aspx";
    public string UtilityUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
    public string LegacyProductStatsUrl = "/gianhang/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx";
    public string LegacyServiceStatsUrl = "/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx";
    public string SyncUrl = "/gianhang/admin/gianhang/bao-cao.aspx?sync=1";
    public string RangeLabel = "Khoảng thời gian: Toàn thời gian";
    public decimal RevenueGross;
    public int TotalOrders;
    public int WaitingExchange;
    public int DeliveredOrders;
    public int ProductCount;
    public int ServiceCount;
    public int TotalViews;
    public int BookingTotal;
    public int LegacyInvoiceCount;
    public decimal LegacyRevenueGross;
    public int LegacyMirroredInvoiceCount;
    public decimal LegacyMirroredRevenueGross;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        branchId = (access.ChiNhanhId ?? "").Trim();
        if (ownerAccountKey == "")
            return;

        if (string.Equals((Request.QueryString["sync"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase))
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey, true);
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceReportUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        PublicUrl = GianHangRoutes_cl.BuildPublicUrl(ownerAccountKey);
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        WaitUrl = GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl();
        BuyerFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl();
        UtilityUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
        LegacyProductStatsUrl = "/gianhang/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx";
        LegacyServiceStatsUrl = "/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx";
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceReportUrl() + "?sync=1";

        if (!IsPostBack)
            ApplyQueryFilter();

        BindPage();
    }

    protected void btn_filter_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(false), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void btn_clear_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(true), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void BindPage()
    {
        GianHangReport_cl.ReportRange range = GianHangReport_cl.ResolveDateRange(
            txt_month.Text,
            txt_from.Text,
            txt_to.Text,
            Request.QueryString["range"]);

        RangeLabel = GianHangReport_cl.BuildRangeLabel(range);
        GianHangReport_cl.Dashboard report = GianHangReport_cl.BuildDashboard(db, ownerAccountKey, range);

        RevenueGross = report.RevenueGross;
        TotalOrders = report.TotalOrders;
        WaitingExchange = report.WaitingExchange;
        DeliveredOrders = report.Delivered;
        ProductCount = report.ProductCount;
        ServiceCount = report.ServiceCount;
        TotalViews = report.TotalViews;
        BookingTotal = report.BookingTotal;

        BindLegacySummary(range);

        List<RecentOrderView> orderRows = report.RecentOrders
            .Select(BuildOrderView)
            .ToList();
        ph_empty_orders.Visible = orderRows.Count == 0;
        rp_recent_orders.DataSource = orderRows;
        rp_recent_orders.DataBind();

        List<RecentBookingView> bookingRows = report.RecentBookings
            .Select(BuildBookingView)
            .ToList();
        ph_empty_bookings.Visible = bookingRows.Count == 0;
        rp_recent_bookings.DataSource = bookingRows;
        rp_recent_bookings.DataBind();
    }

    private void BindLegacySummary(GianHangReport_cl.ReportRange range)
    {
        IQueryable<bspa_hoadon_table> query = db.bspa_hoadon_tables.Where(p =>
            p.user_parent == ownerAccountKey &&
            p.id_chinhanh == branchId);

        if (range != null && !range.IsAllTime)
        {
            if (range.FromDate.HasValue)
            {
                DateTime from = range.FromDate.Value.Date;
                query = query.Where(p => p.ngaytao != null && p.ngaytao.Value >= from);
            }

            if (range.ToExclusive.HasValue)
            {
                DateTime toExclusive = range.ToExclusive.Value;
                query = query.Where(p => p.ngaytao != null && p.ngaytao.Value < toExclusive);
            }
        }

        List<bspa_hoadon_table> rows = query.ToList();
        LegacyInvoiceCount = rows.Count;
        LegacyRevenueGross = rows.Sum(p => Convert.ToDecimal(p.tongsauchietkhau ?? 0));

        List<bspa_hoadon_table> mirrored = rows
            .Where(p => GianHangWorkspaceLink_cl.IsWorkspaceMirrorSource((p.nguongoc ?? "") + ""))
            .ToList();
        LegacyMirroredInvoiceCount = mirrored.Count;
        LegacyMirroredRevenueGross = mirrored.Sum(p => Convert.ToDecimal(p.tongsauchietkhau ?? 0));
    }

    private RecentOrderView BuildOrderView(GianHangReport_cl.RecentOrderRow row)
    {
        long legacyInvoiceId = row == null ? 0L : GianHangWorkspaceLink_cl.ResolveLegacyInvoiceId(db, ownerAccountKey, row.Id);
        return new RecentOrderView
        {
            DisplayId = row == null ? "--" : row.Id.ToString(),
            OrderedAtText = GianHangReport_cl.FormatDateTime(row == null ? null : row.OrderedAt),
            BuyerDisplay = row == null || string.IsNullOrWhiteSpace(row.BuyerDisplay) ? "Khách lẻ" : row.BuyerDisplay,
            StatusText = row == null ? "Không xác định" : (row.StatusText ?? "Không xác định"),
            StatusCss = ResolveOrderStatusCss(row == null ? "" : row.StatusCss),
            TotalAmountText = GianHangReport_cl.FormatCurrency(row == null ? 0m : row.TotalAmount),
            DetailUrl = legacyInvoiceId > 0
                ? "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + legacyInvoiceId.ToString()
                : (OrdersUrl + "?keyword=" + Server.UrlEncode(row == null ? "" : row.Id.ToString()))
        };
    }

    private RecentBookingView BuildBookingView(GianHangReport_cl.RecentBookingRow row)
    {
        long legacyBookingId = row == null ? 0L : GianHangWorkspaceLink_cl.ResolveLegacyBookingId(db, ownerAccountKey, row.Id);
        return new RecentBookingView
        {
            CustomerName = row == null || string.IsNullOrWhiteSpace(row.CustomerName) ? "Khách đặt lịch" : row.CustomerName,
            Phone = row == null || string.IsNullOrWhiteSpace(row.Phone) ? "--" : row.Phone,
            ServiceName = row == null || string.IsNullOrWhiteSpace(row.ServiceName) ? "Dịch vụ /gianhang" : row.ServiceName,
            BookingTimeText = GianHangReport_cl.FormatDateTime(row == null ? null : row.BookingTime),
            StatusText = row == null ? "Không xác định" : (row.StatusText ?? "Không xác định"),
            StatusCss = ResolveBookingStatusCss(row == null ? "" : row.StatusCss),
            DetailUrl = legacyBookingId > 0
                ? "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + legacyBookingId.ToString()
                : "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx"
        };
    }

    private static string ResolveOrderStatusCss(string rawCss)
    {
        string value = (rawCss ?? "").Trim().ToLowerInvariant();
        if (value.Contains("muted"))
            return "gh-admin-report__badge--cancelled";
        if (value.Contains("red"))
            return "gh-admin-report__badge--waiting";
        if (value.Contains("green"))
            return "gh-admin-report__badge--success";
        if (value.Contains("azure"))
            return "gh-admin-report__badge--pending";
        return "gh-admin-report__badge--done";
    }

    private static string ResolveBookingStatusCss(string rawCss)
    {
        string value = (rawCss ?? "").Trim().ToLowerInvariant();
        if (value.Contains("green"))
            return "gh-admin-report__badge--booking-done";
        if (value.Contains("blue"))
            return "gh-admin-report__badge--booking-confirmed";
        return "gh-admin-report__badge--booking-pending";
    }

    private void ApplyQueryFilter()
    {
        string month = (Request.QueryString["month"] ?? "").Trim();
        string from = (Request.QueryString["from"] ?? "").Trim();
        string to = (Request.QueryString["to"] ?? "").Trim();
        string range = (Request.QueryString["range"] ?? "").Trim().ToLowerInvariant();

        DateTime now = AhaTime_cl.Now;
        txt_month.Text = now.ToString("yyyy-MM");
        txt_from.Text = "";
        txt_to.Text = "";

        if (range == "all")
        {
            txt_month.Text = "";
            return;
        }

        if (month != "")
            txt_month.Text = month;
        if (from != "")
            txt_from.Text = from;
        if (to != "")
            txt_to.Text = to;
    }

    private string BuildFilterUrl(bool allTime)
    {
        if (allTime)
            return GianHangRoutes_cl.BuildAdminWorkspaceReportUrl() + "?range=all";

        List<string> parts = new List<string>();
        string month = (txt_month.Text ?? "").Trim();
        string from = (txt_from.Text ?? "").Trim();
        string to = (txt_to.Text ?? "").Trim();

        if (month != "")
            parts.Add("month=" + Server.UrlEncode(month));
        if (from != "")
            parts.Add("from=" + Server.UrlEncode(from));
        if (to != "")
            parts.Add("to=" + Server.UrlEncode(to));

        return parts.Count == 0
            ? GianHangRoutes_cl.BuildAdminWorkspaceReportUrl()
            : (GianHangRoutes_cl.BuildAdminWorkspaceReportUrl() + "?" + string.Join("&", parts.ToArray()));
    }
}
