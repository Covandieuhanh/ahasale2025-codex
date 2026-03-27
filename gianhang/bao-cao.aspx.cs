using System;

public partial class gianhang_bao_cao : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
        {
            InitDefaultRange();
            ApplyQueryFilter();
            BindPage(info);
        }
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

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        string accountKey = info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(accountKey))
            return;

        GianHangReport_cl.ReportRange range = GianHangReport_cl.ResolveDateRange(
            txt_month.Text,
            txt_from.Text,
            txt_to.Text,
            Request.QueryString["range"]);

        lb_range_label.Text = GianHangReport_cl.BuildRangeLabel(range);

        using (dbDataContext db = new dbDataContext())
        {
            GianHangReport_cl.Dashboard report = GianHangReport_cl.BuildDashboard(db, accountKey, range);

            lb_revenue_gross.Text = GianHangReport_cl.FormatCurrency(report.RevenueGross) + " đ";
            lb_total_orders.Text = report.TotalOrders.ToString("#,##0");
            lb_waiting_exchange.Text = report.WaitingExchange.ToString("#,##0");
            lb_total_sold.Text = report.TotalSold.ToString("#,##0");
            lb_total_products.Text = report.ProductCount.ToString("#,##0");
            lb_total_services.Text = report.ServiceCount.ToString("#,##0");
            lb_total_views.Text = report.TotalViews.ToString("#,##0");
            lb_booking_total.Text = report.BookingTotal.ToString("#,##0");

            lb_group_pending.Text = report.PendingOrders.ToString("#,##0");
            lb_group_wait.Text = report.WaitingExchange.ToString("#,##0");
            lb_group_exchanged.Text = report.Exchanged.ToString("#,##0");
            lb_group_delivered.Text = report.Delivered.ToString("#,##0");
            lb_group_cancelled.Text = report.Cancelled.ToString("#,##0");

            lb_booking_pending.Text = report.BookingPending.ToString("#,##0");
            lb_booking_confirmed.Text = report.BookingConfirmed.ToString("#,##0");
            lb_booking_done.Text = report.BookingDone.ToString("#,##0");
            lb_booking_cancelled.Text = report.BookingCancelled.ToString("#,##0");

            ph_empty_orders.Visible = report.RecentOrders.Count == 0;
            rp_recent_orders.DataSource = report.RecentOrders;
            rp_recent_orders.DataBind();

            ph_empty_bookings.Visible = report.RecentBookings.Count == 0;
            rp_recent_bookings.DataSource = report.RecentBookings;
            rp_recent_bookings.DataBind();
        }
    }

    private void InitDefaultRange()
    {
        if (txt_month == null || txt_from == null || txt_to == null)
            return;

        DateTime now = AhaTime_cl.Now;
        txt_month.Text = now.ToString("yyyy-MM");
        txt_from.Text = "";
        txt_to.Text = "";
    }

    private void ApplyQueryFilter()
    {
        string month = (Request.QueryString["month"] ?? "").Trim();
        string from = (Request.QueryString["from"] ?? "").Trim();
        string to = (Request.QueryString["to"] ?? "").Trim();
        string range = (Request.QueryString["range"] ?? "").Trim().ToLowerInvariant();

        if (range == "all")
        {
            txt_month.Text = "";
            txt_from.Text = "";
            txt_to.Text = "";
            return;
        }

        if (!string.IsNullOrWhiteSpace(month))
            txt_month.Text = month;
        if (!string.IsNullOrWhiteSpace(from))
            txt_from.Text = from;
        if (!string.IsNullOrWhiteSpace(to))
            txt_to.Text = to;
    }

    private string BuildFilterUrl(bool allTime)
    {
        if (allTime)
            return GianHangRoutes_cl.BuildBaoCaoUrl() + "?range=all";

        string month = (txt_month.Text ?? "").Trim();
        string from = (txt_from.Text ?? "").Trim();
        string to = (txt_to.Text ?? "").Trim();

        System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrWhiteSpace(month))
            parts.Add("month=" + Server.UrlEncode(month));
        if (!string.IsNullOrWhiteSpace(from))
            parts.Add("from=" + Server.UrlEncode(from));
        if (!string.IsNullOrWhiteSpace(to))
            parts.Add("to=" + Server.UrlEncode(to));

        if (parts.Count == 0)
            return GianHangRoutes_cl.BuildBaoCaoUrl();

        return GianHangRoutes_cl.BuildBaoCaoUrl() + "?" + string.Join("&", parts.ToArray());
    }
}
