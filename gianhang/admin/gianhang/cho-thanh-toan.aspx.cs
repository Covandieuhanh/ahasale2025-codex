using System;
using System.Collections.Generic;
using System.Linq;

public partial class gianhang_admin_gianhang_cho_thanh_toan : System.Web.UI.Page
{
    private sealed class WaitOrderView
    {
        public string DisplayId { get; set; }
        public string SourceInvoiceText { get; set; }
        public string OrderedAtText { get; set; }
        public string BuyerDisplay { get; set; }
        public string BuyerHint { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string TotalAmountText { get; set; }
        public bool HasLegacyMirror { get; set; }
        public string LegacyDetailUrl { get; set; }
        public string ActionOrderId { get; set; }
        public bool CanCancelWait { get; set; }
        public bool CanMarkDelivered { get; set; }
        public bool CanCancelOrder { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();
    private string ownerAccountKey = string.Empty;

    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string BuyerFlowUrl = "/gianhang/admin/gianhang/don-mua.aspx";
    public string UtilityUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string NativeWaitUrl = "/gianhang/cho-thanh-toan.aspx";
    public string LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
    public string SyncUrl = "/gianhang/admin/gianhang/cho-thanh-toan.aspx?sync=1";
    public int SummaryWaiting;
    public int SummaryMirrored;
    public int SummaryCanCancel;
    public int SummaryCanDeliver;

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
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        BuyerFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl();
        UtilityUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        NativeWaitUrl = GianHangRoutes_cl.AppendReturnUrl("/gianhang/cho-thanh-toan.aspx", Request.RawUrl ?? GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl());
        LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl() + "?sync=1";

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
        string orderId = ((e.CommandArgument ?? string.Empty) + string.Empty).Trim();
        if (orderId == string.Empty)
            return;

        GianHangOrderCommand_cl.CommandResult result = GianHangPosWorkflow_cl.ExecuteOrderListCommand(db, ownerAccountKey, e.CommandName, orderId);
        if (result != null && result.ShouldRedirect && !string.IsNullOrWhiteSpace(result.RedirectUrl))
        {
            Response.Redirect(result.RedirectUrl, false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        BindOrders();
        Helper_Tabler_cl.ShowModal(this.Page, result == null ? "Có lỗi xảy ra khi cập nhật trạng thái chờ thanh toán." : result.Message, "Thông báo", true, result == null ? "warning" : (result.AlertType ?? "alert"));
    }

    private void BindOrders()
    {
        string keyword = (txt_search.Text ?? string.Empty).Trim();
        List<GianHangOrderCore_cl.SellerOrderRow> rows = GianHangOrderCore_cl.LoadSellerOrders(db, ownerAccountKey, "cho-trao-doi", keyword, 160);
        List<WaitOrderView> viewRows = rows.Select(BuildViewRow).ToList();

        SummaryWaiting = viewRows.Count;
        SummaryMirrored = viewRows.Count(p => p.HasLegacyMirror);
        SummaryCanCancel = viewRows.Count(p => p.CanCancelWait);
        SummaryCanDeliver = viewRows.Count(p => p.CanMarkDelivered);

        ph_no_orders.Visible = viewRows.Count == 0;
        rp_orders.DataSource = viewRows;
        rp_orders.DataBind();
    }

    private WaitOrderView BuildViewRow(GianHangOrderCore_cl.SellerOrderRow row)
    {
        long legacyInvoiceId = row == null ? 0L : GianHangWorkspaceLink_cl.ResolveLegacyInvoiceId(db, ownerAccountKey, row.SourceInvoiceId);
        return new WaitOrderView
        {
            DisplayId = row == null ? "--" : (row.Id > 0 ? row.Id.ToString() : row.SourceInvoiceId.ToString()),
            SourceInvoiceText = row == null ? "0" : row.SourceInvoiceId.ToString(),
            OrderedAtText = GianHangReport_cl.FormatDateTime(row == null ? null : row.OrderedAt),
            BuyerDisplay = row == null || string.IsNullOrWhiteSpace(row.BuyerDisplay) ? "Khách lẻ" : row.BuyerDisplay,
            BuyerHint = "Buyer đã vào bước chờ thanh toán / trao đổi",
            StatusText = row == null ? "Không xác định" : (row.StatusText ?? "Không xác định"),
            StatusCss = ResolveStatusCss(row == null ? string.Empty : row.StatusGroup),
            TotalAmountText = GianHangReport_cl.FormatCurrency(row == null ? 0m : row.TotalAmount),
            HasLegacyMirror = legacyInvoiceId > 0,
            LegacyDetailUrl = legacyInvoiceId > 0 ? "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + legacyInvoiceId.ToString() : string.Empty,
            ActionOrderId = row == null ? string.Empty : (row.Id > 0 ? row.Id.ToString() : row.SourceInvoiceId.ToString()),
            CanCancelWait = row != null && row.CanCancelWait,
            CanMarkDelivered = row != null && row.CanMarkDelivered,
            CanCancelOrder = row != null && row.CanCancelOrder
        };
    }

    private static string ResolveStatusCss(string statusGroup)
    {
        switch ((statusGroup ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "cho-trao-doi":
                return "gh-admin-wait__badge--waiting";
            case "da-trao-doi":
                return "gh-admin-wait__badge--success";
            case "da-giao":
                return "gh-admin-wait__badge--done";
            case "da-huy":
                return "gh-admin-wait__badge--cancelled";
            default:
                return "gh-admin-wait__badge--pending";
        }
    }

    private void ApplyQueryFilter()
    {
        txt_search.Text = (Request.QueryString["keyword"] ?? string.Empty).Trim();
    }

    private string BuildFilterUrl(bool clear)
    {
        if (clear)
            return GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl();

        string keyword = (txt_search.Text ?? string.Empty).Trim();
        if (keyword == string.Empty)
            return GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl();

        return GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl() + "?keyword=" + Server.UrlEncode(keyword);
    }
}
