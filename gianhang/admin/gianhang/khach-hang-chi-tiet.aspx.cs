using System;
using System.Collections.Generic;
using System.Linq;

public partial class gianhang_admin_gianhang_khach_hang_chi_tiet : System.Web.UI.Page
{
    private sealed class OrderRowView
    {
        public string OrderIdText { get; set; }
        public string BuyerDisplay { get; set; }
        public string Phone { get; set; }
        public string AmountText { get; set; }
        public string OrderedAtText { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string WorkspaceOrdersUrl { get; set; }
    }

    private sealed class BookingRowView
    {
        public string BookingIdText { get; set; }
        public string ServiceName { get; set; }
        public string ScheduleText { get; set; }
        public string CreatedAtText { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string WorkspaceDetailUrl { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string PersonHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
    public string NativeDetailUrl = "/gianhang/khach-hang-chi-tiet.aspx";
    public string NativeCustomersUrl = "/gianhang/khach-hang.aspx";
    public string AdminDetailUrl = "/gianhang/admin/quan-ly-khach-hang/Default.aspx";

    public string CustomerTitle = "Khách hàng /gianhang";
    public string PhoneText = "--";
    public string BuyerAccountText = "--";
    public string LastInteractionText = "--";
    public string MirrorText = "Chưa định danh trong CRM admin";
    public string RevenueText = "0 đ";
    public bool HasDetail;
    public bool HasAdminMirror;
    public int OrderCount;
    public int BookingCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q9_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();

        string customerKey = (Request.QueryString["key"] ?? "").Trim();
        if (customerKey == "")
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        GianHangCustomer_cl.CustomerDetail detail = GianHangCustomer_cl.LoadCustomerDetail(db, ownerAccountKey, customerKey, 12, 12);
        if (detail == null)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        HasDetail = true;
        ph_empty.Visible = false;
        ph_content.Visible = true;

        CustomerTitle = string.IsNullOrWhiteSpace(detail.DisplayName) ? "Khách hàng /gianhang" : detail.DisplayName;
        PhoneText = string.IsNullOrWhiteSpace(detail.Phone) ? "--" : detail.Phone;
        BuyerAccountText = string.IsNullOrWhiteSpace(detail.BuyerAccount) ? "--" : detail.BuyerAccount;
        LastInteractionText = GianHangReport_cl.FormatDateTime(detail.LastInteractionAt);
        RevenueText = GianHangReport_cl.FormatCurrency(detail.RevenueTotal) + " đ";
        OrderCount = detail.OrderCount;
        BookingCount = detail.BookingCount;

        string personKeyword = PhoneText != "--" ? PhoneText : CustomerTitle;
        PersonHubUrl = GianHangRoutes_cl.BuildAdminLegacyPeopleHubUrl(personKeyword);
        NativeDetailUrl = GianHangRoutes_cl.BuildKhachHangChiTietUrl(detail.CustomerKey);
        NativeCustomersUrl = GianHangRoutes_cl.BuildKhachHangUrl();

        long legacyCustomerId = ResolveLegacyCustomerId((access.ChiNhanhId ?? "").Trim(), PhoneText);
        HasAdminMirror = legacyCustomerId > 0;
        AdminDetailUrl = HasAdminMirror
            ? GianHangRoutes_cl.BuildAdminLegacyCustomerDetailUrl(legacyCustomerId)
            : GianHangRoutes_cl.BuildAdminLegacyCustomersUrl(personKeyword);
        MirrorText = HasAdminMirror ? ("CRM admin #" + legacyCustomerId.ToString()) : "Chưa định danh trong CRM admin";

        rp_orders.DataSource = detail.RecentOrders.Select(item => new OrderRowView
        {
            OrderIdText = "#" + item.OrderId.ToString(),
            BuyerDisplay = string.IsNullOrWhiteSpace(item.BuyerDisplay) ? "Khách lẻ" : item.BuyerDisplay,
            Phone = string.IsNullOrWhiteSpace(item.Phone) ? "--" : item.Phone,
            AmountText = GianHangReport_cl.FormatCurrency(item.TotalAmount) + " đ",
            OrderedAtText = GianHangReport_cl.FormatDateTime(item.OrderedAt),
            StatusText = item.StatusText,
            StatusCss = ResolveOrderStatusCss(item.StatusGroup),
            WorkspaceOrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl() + "?keyword=" + Server.UrlEncode(item.OrderId.ToString())
        }).ToList();
        rp_orders.DataBind();
        ph_orders_empty.Visible = detail.RecentOrders == null || detail.RecentOrders.Count == 0;

        rp_bookings.DataSource = detail.RecentBookings.Select(item => new BookingRowView
        {
            BookingIdText = "#" + item.BookingId.ToString(),
            ServiceName = string.IsNullOrWhiteSpace(item.ServiceName) ? "--" : item.ServiceName,
            ScheduleText = GianHangReport_cl.FormatDateTime(item.ScheduleAt),
            CreatedAtText = GianHangReport_cl.FormatDateTime(item.CreatedAt),
            StatusText = item.StatusText,
            StatusCss = ResolveBookingStatusCss(item.StatusText),
            WorkspaceDetailUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingDetailUrl(item.BookingId)
        }).ToList();
        rp_bookings.DataBind();
        ph_bookings_empty.Visible = detail.RecentBookings == null || detail.RecentBookings.Count == 0;
    }

    private long ResolveLegacyCustomerId(string chiNhanhId, string phone)
    {
        string normalizedPhone = NormalizePhone(phone);
        if (chiNhanhId == "" || normalizedPhone == "")
            return 0L;

        bspa_data_khachhang_table customer = db.bspa_data_khachhang_tables.FirstOrDefault(p =>
            p.id_chinhanh == chiNhanhId &&
            (p.sdt ?? "").Replace(" ", "").Replace(".", "").Replace("-", "") == normalizedPhone);
        return customer == null ? 0L : customer.id;
    }

    private string ResolveOrderStatusCss(string statusGroup)
    {
        string value = (statusGroup ?? "").Trim().ToLowerInvariant();
        if (value == "waiting")
            return "gh-wc-badge gh-wc-badge--waiting";
        if (value == "success" || value == "done")
            return "gh-wc-badge gh-wc-badge--success";
        if (value == "cancelled")
            return "gh-wc-badge gh-wc-badge--muted";
        return "gh-wc-badge gh-wc-badge--active";
    }

    private string ResolveBookingStatusCss(string statusText)
    {
        string value = (statusText ?? "").Trim();
        if (value == GianHangBooking_cl.TrangThaiDaXacNhan)
            return "gh-wc-badge gh-wc-badge--active";
        if (value == GianHangBooking_cl.TrangThaiHoanThanh)
            return "gh-wc-badge gh-wc-badge--success";
        if (value == GianHangBooking_cl.TrangThaiHuy)
            return "gh-wc-badge gh-wc-badge--muted";
        return "gh-wc-badge gh-wc-badge--waiting";
    }

    private static string NormalizePhone(string raw)
    {
        return new string(((raw ?? "").Trim()).Where(char.IsDigit).ToArray());
    }
}
