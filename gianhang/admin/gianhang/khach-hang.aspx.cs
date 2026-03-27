using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

public partial class gianhang_admin_gianhang_khach_hang : System.Web.UI.Page
{
    private sealed class CustomerRowView
    {
        public string CustomerKey { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string BuyerAccount { get; set; }
        public string OrderCountText { get; set; }
        public string BookingCountText { get; set; }
        public string RevenueText { get; set; }
        public string LastInteractionText { get; set; }
        public string NativeDetailUrl { get; set; }
        public string AdminDetailUrl { get; set; }
        public string PersonHubUrl { get; set; }
        public bool HasAdminMirror { get; set; }
        public string MirrorText { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string NativeCustomersUrl = "/gianhang/khach-hang.aspx";
    public string AdminCustomersUrl = "/gianhang/admin/quan-ly-khach-hang/Default.aspx";
    public string PersonHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string SyncUrl = "/gianhang/admin/gianhang/khach-hang.aspx?sync=1";

    public int TotalCustomers;
    public int WithPhoneCount;
    public int AdminMirrorCount;
    public int TotalOrderCount;
    public int TotalBookingCount;
    public decimal RevenueTotal;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q9_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        if (string.Equals((Request.QueryString["sync"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase))
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey, true);
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        NativeCustomersUrl = GianHangRoutes_cl.BuildKhachHangUrl();
        AdminCustomersUrl = "/gianhang/admin/quan-ly-khach-hang/Default.aspx";
        PersonHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl() + "?sync=1";

        if (!IsPostBack)
            txt_search.Text = (Request.QueryString["keyword"] ?? "").Trim();

        BindRows(ownerAccountKey, (access.ChiNhanhId ?? "").Trim());
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

    private void BindRows(string ownerAccountKey, string chiNhanhId)
    {
        string keyword = (txt_search.Text ?? "").Trim();
        List<GianHangCustomer_cl.CustomerRow> customers = GianHangCustomer_cl.LoadCustomers(db, ownerAccountKey, keyword, 160);

        Dictionary<string, long> legacyByPhone = LoadLegacyCustomerMap(chiNhanhId);
        TotalCustomers = customers.Count;
        WithPhoneCount = customers.Count(p => NormalizePhone(p.Phone) != "");
        TotalOrderCount = customers.Sum(p => p.OrderCount);
        TotalBookingCount = customers.Sum(p => p.BookingCount);
        RevenueTotal = customers.Sum(p => p.RevenueTotal);

        List<CustomerRowView> rows = customers.Select(item =>
        {
            string phone = NormalizePhone(item.Phone);
            long legacyCustomerId = 0;
            bool hasLegacy = phone != "" && legacyByPhone.TryGetValue(phone, out legacyCustomerId) && legacyCustomerId > 0;
            return new CustomerRowView
            {
                CustomerKey = item.CustomerKey,
                DisplayName = item.DisplayName,
                Phone = string.IsNullOrWhiteSpace(phone) ? "--" : phone,
                BuyerAccount = string.IsNullOrWhiteSpace(item.BuyerAccount) ? "--" : item.BuyerAccount,
                OrderCountText = item.OrderCount.ToString("#,##0"),
                BookingCountText = item.BookingCount.ToString("#,##0"),
                RevenueText = GianHangReport_cl.FormatCurrency(item.RevenueTotal) + " đ",
                LastInteractionText = GianHangReport_cl.FormatDateTime(item.LastInteractionAt),
                NativeDetailUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomerDetailUrl(item.CustomerKey),
                AdminDetailUrl = hasLegacy ? ("/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + legacyCustomerId.ToString()) : (AdminCustomersUrl + "?keyword=" + Server.UrlEncode(phone != "" ? phone : item.DisplayName)),
                PersonHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx?keyword=" + Server.UrlEncode(phone != "" ? phone : item.DisplayName),
                HasAdminMirror = hasLegacy,
                MirrorText = hasLegacy ? ("CRM admin #" + legacyCustomerId.ToString()) : "Chưa định danh trong CRM admin"
            };
        }).ToList();

        AdminMirrorCount = rows.Count(p => p.HasAdminMirror);
        ph_empty.Visible = rows.Count == 0;
        rp_rows.DataSource = rows;
        rp_rows.DataBind();
    }

    private Dictionary<string, long> LoadLegacyCustomerMap(string chiNhanhId)
    {
        Dictionary<string, long> map = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(chiNhanhId))
            return map;

        List<bspa_data_khachhang_table> rows = db.bspa_data_khachhang_tables.Where(p => p.id_chinhanh == chiNhanhId).ToList();
        for (int i = 0; i < rows.Count; i++)
        {
            bspa_data_khachhang_table item = rows[i];
            string phone = NormalizePhone(item.sdt);
            if (phone == "" || map.ContainsKey(phone))
                continue;
            map[phone] = item.id;
        }
        return map;
    }

    private static string NormalizePhone(string raw)
    {
        string digits = new string(((raw ?? "").Trim()).Where(char.IsDigit).ToArray());
        return digits.Trim();
    }

    private string BuildFilterUrl(bool clear)
    {
        if (clear)
            return GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();

        string keyword = (txt_search.Text ?? "").Trim();
        if (keyword == "")
            return GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        return GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl() + "?keyword=" + Server.UrlEncode(keyword);
    }
}
