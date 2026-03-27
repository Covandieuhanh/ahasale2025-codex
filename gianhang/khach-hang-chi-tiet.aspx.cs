using System;
using System.Web;

public partial class gianhang_khach_hang_chi_tiet : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
            BindPage(info);
    }

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        string accountKey = info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
        string customerKey = (Request.QueryString["key"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(accountKey) || string.IsNullOrWhiteSpace(customerKey))
        {
            ShowNotFound();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            GianHangCustomer_cl.CustomerDetail detail = GianHangCustomer_cl.LoadCustomerDetail(db, accountKey, customerKey, 20, 20);
            if (detail == null)
            {
                ShowNotFound();
                return;
            }

            ph_detail.Visible = true;
            ph_not_found.Visible = false;

            lb_name.Text = HttpUtility.HtmlEncode(detail.DisplayName);
            lb_phone.Text = HttpUtility.HtmlEncode(detail.Phone);
            lb_buyer_account.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(detail.BuyerAccount) ? "--" : detail.BuyerAccount);
            lb_order_count.Text = detail.OrderCount.ToString("#,##0");
            lb_booking_count.Text = detail.BookingCount.ToString("#,##0");
            lb_revenue_total.Text = GianHangReport_cl.FormatCurrency(detail.RevenueTotal) + " đ";
            lb_last_interaction.Text = GianHangReport_cl.FormatDateTime(detail.LastInteractionAt);
            lb_last_interaction_kpi.Text = GianHangReport_cl.FormatDateTime(detail.LastInteractionAt);

            lnk_back_customers.NavigateUrl = "/gianhang/khach-hang.aspx";
            lnk_back_orders.NavigateUrl = "/gianhang/don-ban.aspx";
            lnk_back_bookings.NavigateUrl = "/gianhang/quan-ly-lich-hen.aspx";

            string phoneDigits = (detail.Phone ?? "").Trim();
            ph_call.Visible = phoneDigits != "--" && phoneDigits != "";
            if (ph_call.Visible)
                lnk_call.NavigateUrl = "tel:" + phoneDigits;

            ph_empty_orders.Visible = detail.RecentOrders == null || detail.RecentOrders.Count == 0;
            rp_orders.DataSource = detail.RecentOrders;
            rp_orders.DataBind();

            ph_empty_bookings.Visible = detail.RecentBookings == null || detail.RecentBookings.Count == 0;
            rp_bookings.DataSource = detail.RecentBookings;
            rp_bookings.DataBind();
        }
    }

    private void ShowNotFound()
    {
        ph_detail.Visible = false;
        ph_not_found.Visible = true;
        lnk_not_found.NavigateUrl = "/gianhang/khach-hang.aspx";
    }

    protected string ResolveOrderStatusCss(object raw)
    {
        string status = (raw ?? "").ToString().Trim().ToLowerInvariant();
        switch (status)
        {
            case "success":
                return "gh-chip gh-chip-success";
            case "waiting":
                return "gh-chip gh-chip-warning";
            case "cancelled":
                return "gh-chip gh-chip-muted";
            case "done":
                return "gh-chip gh-chip-info";
            default:
                return "gh-chip gh-chip-active";
        }
    }

    protected string ResolveBookingStatusCss(object raw)
    {
        string status = (raw ?? "").ToString().Trim();
        if (status == GianHangBooking_cl.TrangThaiHoanThanh)
            return "gh-chip gh-chip-success";
        if (status == GianHangBooking_cl.TrangThaiDaXacNhan)
            return "gh-chip gh-chip-info";
        if (status == GianHangBooking_cl.TrangThaiHuy)
            return "gh-chip gh-chip-muted";
        return "gh-chip gh-chip-warning";
    }
}
