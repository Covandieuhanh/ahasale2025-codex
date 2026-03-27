using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;

public partial class gianhang_don_mua : Page
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    private const string STATUS_FILTER_KEY = "gianhang_buyer_status_filter";

    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated || !info.CanAccessHome)
        {
            HomeSpaceAccess_cl.RedirectToHomeLogin(this, Request.RawUrl ?? "/gianhang/don-mua.aspx", "Vui lòng đăng nhập tài khoản gốc để xem đơn mua của gian hàng.");
            return;
        }

        if (!IsPostBack)
        {
            SetStatusFilter("all");
            SyncStatusDropdown();
            LoadNotice();
            BindPage(info);
        }
    }

    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        BindPage(RootAccount_cl.GetCurrentInfo());
    }

    protected void ddl_status_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetStatusFilter(ddl_status == null ? "all" : ddl_status.SelectedValue);
        SyncStatusDropdown();
        BindPage(RootAccount_cl.GetCurrentInfo());
    }

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        if (info == null || !info.IsAuthenticated)
            return;

        using (dbDataContext db = new dbDataContext())
        {
            List<GianHangBuyerOrder_cl.BuyerOrderRow> rows = GianHangBuyerOrder_cl.LoadOrders(
                db,
                info.AccountKey,
                GetStatusFilter(),
                txt_search == null ? string.Empty : (txt_search.Text ?? string.Empty),
                200);

            GianHangBuyerOrder_cl.BuyerOrderSummary summary = GianHangBuyerOrder_cl.BuildSummary(db, info.AccountKey);

            lit_summary_total.Text = summary.Total.ToString("#,##0");
            lit_summary_waiting.Text = summary.WaitingExchange.ToString("#,##0");
            lit_summary_exchanged.Text = summary.Exchanged.ToString("#,##0");
            lit_summary_delivered.Text = summary.Delivered.ToString("#,##0");
            lit_summary_cancelled.Text = summary.Cancelled.ToString("#,##0");

            ph_empty.Visible = rows.Count == 0;
            rp_orders.DataSource = rows;
            rp_orders.DataBind();
        }
    }

    private void LoadNotice()
    {
        string raw = GianHangBuyerOrder_cl.PullNotice(Session);
        if (string.IsNullOrWhiteSpace(raw))
        {
            ph_notice.Visible = false;
            return;
        }

        ph_notice.Visible = true;
        lit_notice.Text = Server.HtmlEncode(raw.Trim());
    }

    protected string ResolveImageUrl(object rawValue)
    {
        return GianHangStorefront_cl.ResolveImageUrl((rawValue ?? string.Empty).ToString());
    }

    protected string FormatCurrency(object rawValue)
    {
        decimal value = 0m;
        try
        {
            if (rawValue != null && rawValue != DBNull.Value)
                value = Convert.ToDecimal(rawValue, ViCulture);
        }
        catch
        {
            value = 0m;
        }

        return value.ToString("#,##0", ViCulture);
    }

    protected string FormatDate(object rawValue)
    {
        DateTime value;
        return DateTime.TryParse(Convert.ToString(rawValue, ViCulture), out value)
            ? value.ToString("dd/MM/yyyy HH:mm")
            : "--";
    }

    private string GetStatusFilter()
    {
        return GianHangBuyerOrder_cl.NormalizeStatusFilter((ViewState[STATUS_FILTER_KEY] ?? "all").ToString());
    }

    private void SetStatusFilter(string rawValue)
    {
        ViewState[STATUS_FILTER_KEY] = GianHangBuyerOrder_cl.NormalizeStatusFilter(rawValue);
    }

    private void SyncStatusDropdown()
    {
        if (ddl_status == null)
            return;

        string selected = GetStatusFilter();
        System.Web.UI.WebControls.ListItem item = ddl_status.Items.FindByValue(selected);
        if (item == null)
            return;

        ddl_status.ClearSelection();
        item.Selected = true;
    }
}
