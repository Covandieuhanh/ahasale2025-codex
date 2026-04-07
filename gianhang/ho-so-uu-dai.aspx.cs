using System;
using System.Web;
using System.Web.UI;

public partial class gianhang_ho_so_uu_dai : Page
{
    private const int WalletType = 2;
    private const int PageSize = 20;
    private const decimal VND_PER_A = 1000m;

    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
        {
            txt_search.Text = (Request.QueryString["q"] ?? string.Empty).Trim();
            BindPage(info);
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildSelfUrl(txt_search.Text, 1), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void btn_clear_Click(object sender, EventArgs e)
    {
        Response.Redirect("/gianhang/ho-so-uu-dai.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        int currentPage = ResolveCurrentPage();
        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);
            GianHangLedger_cl.HistoryPage history = GianHangLedger_cl.LoadHistoryPage(db, info.AccountKey, WalletType, txt_search.Text, currentPage, PageSize);
            int totalPage = number_of_page_class.return_total_page(history.TotalCount, PageSize);
            if (totalPage < 1)
                totalPage = 1;

            if (currentPage > totalPage && history.TotalCount > 0)
            {
                Response.Redirect(BuildSelfUrl(txt_search.Text, totalPage), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            lit_balance_a.Text = history.Balance.UuDai.ToString("#,##0.##") + " A";
            lit_balance_vnd.Text = (history.Balance.UuDai * VND_PER_A).ToString("#,##0") + " đ";
            lit_total_count.Text = history.TotalCount.ToString("#,##0");
            lit_page_state.Text = string.Format("Trang {0}/{1}", currentPage, totalPage);
            litPager.Text = HomePager_cl.RenderPager(Request, currentPage, totalPage);
            rp_history.DataSource = history.Items;
            rp_history.DataBind();
            ph_empty.Visible = history.Items.Count == 0;
        }
    }

    protected string BuildBadgeCss(object isCreditRaw)
    {
        return (ToBool(isCreditRaw) ? "gh-wallet-item-badge gh-wallet-item-badge--credit" : "gh-wallet-item-badge gh-wallet-item-badge--debit");
    }

    protected string BuildBadgeText(object isCreditRaw)
    {
        return ToBool(isCreditRaw) ? "Ghi có" : "Ghi nợ";
    }

    protected string BuildAmountCss(object isCreditRaw)
    {
        return ToBool(isCreditRaw) ? "gh-wallet-amount-value gh-wallet-amount-value--credit" : "gh-wallet-amount-value gh-wallet-amount-value--debit";
    }

    protected string BuildSignedAmount(object amountRaw, object isCreditRaw)
    {
        decimal amount = ToDecimal(amountRaw);
        string sign = ToBool(isCreditRaw) ? "+" : "-";
        return sign + amount.ToString("#,##0.##") + " A";
    }

    protected string BuildOrderLabel(object publicOrderIdRaw, object orderIdRaw)
    {
        string publicOrderId = (publicOrderIdRaw ?? string.Empty).ToString().Trim();
        string orderId = (orderIdRaw ?? string.Empty).ToString().Trim();
        if (publicOrderId != string.Empty)
            return Server.HtmlEncode(publicOrderId);
        if (orderId != string.Empty)
            return Server.HtmlEncode(orderId);
        return "--";
    }

    protected string BuildBuyerLabel(object buyerAccountRaw)
    {
        string buyer = (buyerAccountRaw ?? string.Empty).ToString().Trim();
        return buyer == string.Empty ? "--" : Server.HtmlEncode(buyer);
    }

    protected string CleanNote(object noteRaw)
    {
        string note = (noteRaw ?? string.Empty).ToString().Trim();
        note = note.Replace(GianHangLedger_cl.TagCreditSeller, string.Empty)
                   .Replace(GianHangLedger_cl.TagDebitSeller, string.Empty)
                   .Replace(GianHangLedger_cl.TagRoot, string.Empty)
                   .Trim();
        return note == string.Empty ? "Giao dịch hồ sơ quyền ưu đãi gian hàng" : Server.HtmlEncode(note);
    }

    protected string FormatDate(object valueRaw)
    {
        DateTime value;
        if (valueRaw != null && DateTime.TryParse(valueRaw.ToString(), out value))
            return value.ToString("dd/MM/yyyy HH:mm");
        return "--";
    }

    private int ResolveCurrentPage()
    {
        int page;
        if (!int.TryParse(Request.QueryString["page"], out page) || page < 1)
            page = 1;
        return page;
    }

    private string BuildSelfUrl(string keyword, int page)
    {
        string url = "/gianhang/ho-so-uu-dai.aspx";
        System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
        string safeKeyword = (keyword ?? string.Empty).Trim();
        if (safeKeyword != string.Empty)
            parts.Add("q=" + HttpUtility.UrlEncode(safeKeyword));
        if (page > 1)
            parts.Add("page=" + page.ToString());
        if (parts.Count == 0)
            return url;
        return url + "?" + string.Join("&", parts.ToArray());
    }

    private static bool ToBool(object valueRaw)
    {
        bool value;
        return valueRaw != null && bool.TryParse(valueRaw.ToString(), out value) && value;
    }

    private static decimal ToDecimal(object valueRaw)
    {
        decimal value;
        return valueRaw != null && decimal.TryParse(valueRaw.ToString(), out value) ? value : 0m;
    }
}
