using System;
using System.Collections.Generic;
using System.Linq;

public partial class gianhang_admin_gianhang_don_ban : System.Web.UI.Page
{
    private sealed class AdminOrderRowView
    {
        public string DisplayId { get; set; }
        public string ActionOrderId { get; set; }
        public long SourceInvoiceId { get; set; }
        public string OrderedAtText { get; set; }
        public string BuyerDisplay { get; set; }
        public string BuyerHint { get; set; }
        public string TypeLabel { get; set; }
        public string TypeCss { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string TotalAmountText { get; set; }
        public bool HasLegacyMirror { get; set; }
        public string LegacyDetailUrl { get; set; }
        public bool CanOpenWait { get; set; }
        public bool CanCancelWait { get; set; }
        public bool CanCancelOrder { get; set; }
        public bool CanMarkDelivered { get; set; }
        public bool IsWaitingExchange { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();
    private string ownerAccountKey = "";

    public string CreateOrderUrl = "/gianhang/don-ban.aspx?taodon=1";
    public string CreateFlowUrl = "/gianhang/admin/gianhang/tao-giao-dich.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string WaitExchangeUrl = "/gianhang/admin/gianhang/cho-thanh-toan.aspx";
    public string BuyerFlowUrl = "/gianhang/admin/gianhang/don-mua.aspx";
    public string UtilityUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
    public string SyncUrl = "/gianhang/admin/gianhang/don-ban.aspx?sync=1";
    public int SummaryTotal;
    public int SummaryPending;
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

        ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        if (string.Equals((Request.QueryString["sync"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase))
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey, true);
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        CreateOrderUrl = GianHangRoutes_cl.AppendReturnUrl("/gianhang/don-ban.aspx?taodon=1", Request.RawUrl ?? GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl());
        CreateFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceCreateUrl() + "?return_url=" + Server.UrlEncode(Request.RawUrl ?? GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl());
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        WaitExchangeUrl = GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl();
        BuyerFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl();
        UtilityUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl() + "?sync=1";

        if (!IsPostBack)
            ApplyQueryFilter();

        BindOrders();
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

    protected void rp_orders_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        string orderId = ((e.CommandArgument ?? "") + "").Trim();
        if (orderId == "")
            return;

        GianHangOrderCommand_cl.CommandResult result = GianHangPosWorkflow_cl.ExecuteOrderListCommand(db, ownerAccountKey, e.CommandName, orderId);
        if (result != null && result.ShouldRedirect && !string.IsNullOrWhiteSpace(result.RedirectUrl))
        {
            Response.Redirect(result.RedirectUrl, false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        BindOrders();
        Helper_Tabler_cl.ShowModal(this.Page, result == null ? "Có lỗi xảy ra khi cập nhật đơn." : result.Message, "Thông báo", true, result == null ? "warning" : (result.AlertType ?? "alert"));
    }

    private void BindOrders()
    {
        string statusFilter = GianHangOrderCore_cl.NormalizeOrderStatusFilter(ddl_status.SelectedValue);
        string keyword = (txt_search.Text ?? "").Trim();

        GianHangOrderCore_cl.SellerOrderSummary summary = GianHangOrderCore_cl.BuildSellerOrderSummary(db, ownerAccountKey);
        SummaryTotal = summary.Total;
        SummaryPending = summary.Pending;
        SummaryWaiting = summary.WaitingExchange;
        SummaryExchanged = summary.Exchanged;
        SummaryDelivered = summary.Delivered;
        SummaryCancelled = summary.Cancelled;

        List<GianHangOrderCore_cl.SellerOrderRow> rows = GianHangOrderCore_cl.LoadSellerOrders(db, ownerAccountKey, statusFilter, keyword, 300);
        List<AdminOrderRowView> viewRows = rows.Select(BuildViewRow).ToList();
        SummaryMirrored = viewRows.Count(p => p.HasLegacyMirror);

        ph_no_orders.Visible = viewRows.Count == 0;
        rp_orders.DataSource = viewRows;
        rp_orders.DataBind();
    }

    private AdminOrderRowView BuildViewRow(GianHangOrderCore_cl.SellerOrderRow row)
    {
        long legacyInvoiceId = row == null ? 0L : GianHangWorkspaceLink_cl.ResolveLegacyInvoiceId(db, ownerAccountKey, row.SourceInvoiceId);
        return new AdminOrderRowView
        {
            DisplayId = row == null ? "--" : (row.Id > 0 ? row.Id.ToString() : row.SourceInvoiceId.ToString()),
            ActionOrderId = row == null ? "" : (row.Id > 0 ? row.Id.ToString() : row.SourceInvoiceId.ToString()),
            SourceInvoiceId = row == null ? 0L : row.SourceInvoiceId,
            OrderedAtText = GianHangReport_cl.FormatDateTime(row == null ? null : row.OrderedAt),
            BuyerDisplay = row == null || string.IsNullOrWhiteSpace(row.BuyerDisplay) ? "Khách lẻ" : row.BuyerDisplay,
            BuyerHint = row == null || !row.IsOffline ? "Đơn online từ storefront" : "Đơn tạo thủ công / tại quầy",
            TypeLabel = row != null && row.IsOffline ? "Offline" : "Online",
            TypeCss = row != null && row.IsOffline ? "gh-admin-badge--offline" : "gh-admin-badge--online",
            StatusText = row == null ? "Không xác định" : (row.StatusText ?? "Không xác định"),
            StatusCss = ResolveStatusCss(row == null ? "" : row.StatusGroup),
            TotalAmountText = GianHangReport_cl.FormatCurrency(row == null ? 0m : row.TotalAmount),
            HasLegacyMirror = legacyInvoiceId > 0,
            LegacyDetailUrl = legacyInvoiceId > 0 ? "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + legacyInvoiceId.ToString() : "",
            CanOpenWait = row != null && row.CanOpenWait,
            CanCancelWait = row != null && row.CanCancelWait,
            CanCancelOrder = row != null && row.CanCancelOrder,
            CanMarkDelivered = row != null && row.CanMarkDelivered,
            IsWaitingExchange = row != null && row.IsWaitingExchange
        };
    }

    private static string ResolveStatusCss(string statusGroup)
    {
        switch ((statusGroup ?? "").Trim().ToLowerInvariant())
        {
            case "cho-trao-doi":
                return "gh-admin-badge--waiting";
            case "da-trao-doi":
                return "gh-admin-badge--exchanged";
            case "da-giao":
                return "gh-admin-badge--delivered";
            case "da-huy":
                return "gh-admin-badge--cancelled";
            default:
                return "gh-admin-badge--pending";
        }
    }

    private void ApplyQueryFilter()
    {
        string status = (Request.QueryString["status"] ?? "").Trim();
        string keyword = (Request.QueryString["keyword"] ?? "").Trim();

        System.Web.UI.WebControls.ListItem selectedStatus = ddl_status.Items.FindByValue(GianHangOrderCore_cl.NormalizeOrderStatusFilter(status));
        if (selectedStatus != null)
        {
            ddl_status.ClearSelection();
            selectedStatus.Selected = true;
        }

        txt_search.Text = keyword;
    }

    private string BuildFilterUrl(bool clear)
    {
        if (clear)
            return GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();

        List<string> parts = new List<string>();
        string status = GianHangOrderCore_cl.NormalizeOrderStatusFilter(ddl_status.SelectedValue);
        string keyword = (txt_search.Text ?? "").Trim();
        if (status != "all")
            parts.Add("status=" + Server.UrlEncode(status));
        if (keyword != "")
            parts.Add("keyword=" + Server.UrlEncode(keyword));

        return parts.Count == 0
            ? GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl()
            : (GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl() + "?" + string.Join("&", parts.ToArray()));
    }
}
