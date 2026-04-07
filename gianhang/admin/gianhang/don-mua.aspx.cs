using System;
using System.Collections.Generic;
using System.Linq;

public partial class gianhang_admin_gianhang_don_mua : System.Web.UI.Page
{
    private sealed class BuyerFlowRowView
    {
        public string DisplayId { get; set; }
        public string SourceInvoiceText { get; set; }
        public string OrderedAtText { get; set; }
        public string BuyerDisplay { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string StageHint { get; set; }
        public string TotalAmountText { get; set; }
        public bool HasLegacyMirror { get; set; }
        public string LegacyDetailUrl { get; set; }
        public string OrderDetailUrl { get; set; }
        public string PublicUrl { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();
    private string ownerAccountKey = string.Empty;

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string WaitUrl = "/gianhang/admin/gianhang/cho-thanh-toan.aspx";
    public string UtilityUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string PublicUrl = "/gianhang/public.aspx";
    public string LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
    public string SyncUrl = "/gianhang/admin/gianhang/don-mua.aspx?sync=1";
    public int SummaryTotal;
    public int SummaryWaiting;
    public int SummaryExchanged;
    public int SummaryDelivered;
    public int SummaryCancelled;
    public int SummaryMirrored;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q7_1");
        if (access == null)
            return;

        ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        if (string.Equals((Request.QueryString["sync"] ?? string.Empty).Trim(), "1", StringComparison.OrdinalIgnoreCase))
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey, true);
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        WaitUrl = GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl();
        UtilityUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        PublicUrl = GianHangRoutes_cl.BuildPublicUrl(ownerAccountKey);
        LegacyInvoiceUrl = GianHangRoutes_cl.BuildAdminLegacyInvoiceListUrl();
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl() + "?sync=1";

        if (!IsPostBack)
            ApplyQueryFilter();

        BindRows();
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

    private void BindRows()
    {
        string statusFilter = GianHangOrderCore_cl.NormalizeOrderStatusFilter(ddl_status.SelectedValue);
        string keyword = (txt_search.Text ?? string.Empty).Trim();

        GianHangOrderCore_cl.SellerOrderSummary summary = GianHangOrderCore_cl.BuildSellerOrderSummary(db, ownerAccountKey);
        SummaryTotal = summary.Total;
        SummaryWaiting = summary.WaitingExchange;
        SummaryExchanged = summary.Exchanged;
        SummaryDelivered = summary.Delivered;
        SummaryCancelled = summary.Cancelled;

        List<GianHangOrderCore_cl.SellerOrderRow> rows = GianHangOrderCore_cl.LoadSellerOrders(db, ownerAccountKey, statusFilter, keyword, 200);
        List<BuyerFlowRowView> viewRows = rows.Select(BuildViewRow).ToList();
        SummaryMirrored = viewRows.Count(p => p.HasLegacyMirror);

        ph_empty.Visible = viewRows.Count == 0;
        rp_orders.DataSource = viewRows;
        rp_orders.DataBind();
    }

    private BuyerFlowRowView BuildViewRow(GianHangOrderCore_cl.SellerOrderRow row)
    {
        long legacyInvoiceId = row == null ? 0L : GianHangWorkspaceLink_cl.ResolveLegacyInvoiceId(db, ownerAccountKey, row.SourceInvoiceId);
        string statusGroup = row == null ? string.Empty : (row.StatusGroup ?? string.Empty);
        string sourceKey = row == null ? string.Empty : row.SourceInvoiceId.ToString();
        return new BuyerFlowRowView
        {
            DisplayId = row == null ? "--" : (row.Id > 0 ? row.Id.ToString() : row.SourceInvoiceId.ToString()),
            SourceInvoiceText = sourceKey == string.Empty ? "0" : sourceKey,
            OrderedAtText = GianHangReport_cl.FormatDateTime(row == null ? null : row.OrderedAt),
            BuyerDisplay = row == null || string.IsNullOrWhiteSpace(row.BuyerDisplay) ? "Khách lẻ" : row.BuyerDisplay,
            StatusText = row == null ? "Không xác định" : (row.StatusText ?? "Không xác định"),
            StatusCss = ResolveStatusCss(statusGroup),
            StageHint = ResolveStageHint(statusGroup),
            TotalAmountText = GianHangReport_cl.FormatCurrency(row == null ? 0m : row.TotalAmount),
            HasLegacyMirror = legacyInvoiceId > 0,
            LegacyDetailUrl = legacyInvoiceId > 0 ? GianHangRoutes_cl.BuildAdminLegacyInvoiceDetailUrl(legacyInvoiceId) : string.Empty,
            OrderDetailUrl = OrdersUrl + "?keyword=" + Server.UrlEncode(sourceKey),
            PublicUrl = PublicUrl
        };
    }

    private static string ResolveStatusCss(string statusGroup)
    {
        switch ((statusGroup ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "cho-trao-doi":
                return "gh-admin-buyerflow__status gh-admin-buyerflow__status--waiting";
            case "da-trao-doi":
                return "gh-admin-buyerflow__status gh-admin-buyerflow__status--exchanged";
            case "da-giao":
                return "gh-admin-buyerflow__status gh-admin-buyerflow__status--delivered";
            case "da-huy":
                return "gh-admin-buyerflow__status gh-admin-buyerflow__status--cancelled";
            default:
                return "gh-admin-buyerflow__status gh-admin-buyerflow__status--pending";
        }
    }

    private static string ResolveStageHint(string statusGroup)
    {
        switch ((statusGroup ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "cho-trao-doi":
                return "Buyer đang ở bước chờ thanh toán / trao đổi.";
            case "da-trao-doi":
                return "Buyer đã hoàn tất trao đổi, đang chờ giao hoặc hậu kiểm.";
            case "da-giao":
                return "Buyer đã nhận hàng / hoàn tất vòng đơn mua.";
            case "da-huy":
                return "Buyer-flow đã kết thúc bằng thao tác hủy.";
            default:
                return "Buyer đã tạo đơn nhưng chưa vào bước trao đổi.";
        }
    }

    private void ApplyQueryFilter()
    {
        string keyword = (Request.QueryString["keyword"] ?? string.Empty).Trim();
        string status = (Request.QueryString["status"] ?? string.Empty).Trim();
        txt_search.Text = keyword;

        System.Web.UI.WebControls.ListItem selected = ddl_status.Items.FindByValue(GianHangOrderCore_cl.NormalizeOrderStatusFilter(status));
        if (selected != null)
        {
            ddl_status.ClearSelection();
            selected.Selected = true;
        }
    }

    private string BuildFilterUrl(bool clear)
    {
        if (clear)
            return GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl();

        List<string> parts = new List<string>();
        string keyword = (txt_search.Text ?? string.Empty).Trim();
        string status = GianHangOrderCore_cl.NormalizeOrderStatusFilter(ddl_status.SelectedValue);
        if (keyword != string.Empty)
            parts.Add("keyword=" + Server.UrlEncode(keyword));
        if (status != "all")
            parts.Add("status=" + Server.UrlEncode(status));

        return parts.Count == 0
            ? GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl()
            : (GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl() + "?" + string.Join("&", parts.ToArray()));
    }
}
