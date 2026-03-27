using System;

public partial class gianhang_khach_hang : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
        {
            ApplyFilter();
            BindPage(info);
        }
    }

    protected void btn_filter_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void btn_reset_Click(object sender, EventArgs e)
    {
        Response.Redirect(GianHangRoutes_cl.BuildKhachHangUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void ApplyFilter()
    {
        txt_search.Text = (Request.QueryString["q"] ?? "").Trim();
    }

    private string BuildFilterUrl()
    {
        string q = (txt_search.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(q))
            return GianHangRoutes_cl.BuildKhachHangUrl();
        return GianHangRoutes_cl.BuildKhachHangUrl() + "?q=" + Server.UrlEncode(q);
    }

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        string accountKey = info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(accountKey))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            var rows = GianHangCustomer_cl.LoadCustomers(db, accountKey, txt_search.Text, 300);
            ph_empty.Visible = rows.Count == 0;
            rp_customers.DataSource = rows;
            rp_customers.DataBind();
        }
    }

    protected string BuildDetailUrl(object customerKeyRaw)
    {
        string customerKey = (customerKeyRaw ?? "").ToString().Trim();
        if (string.IsNullOrWhiteSpace(customerKey))
            return GianHangRoutes_cl.BuildKhachHangUrl();

        return GianHangRoutes_cl.BuildKhachHangChiTietUrl(customerKey);
    }
}
