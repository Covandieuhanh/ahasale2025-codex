using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class danh_sach_bai_viet : System.Web.UI.Page
{
    public string notifi = "", idmn = "", tenmn, mota;
    public string title_web = "", des = "";
    public string key = "";
    public string meta = "";

    private readonly dbDataContext db = new dbDataContext();
    public int stt = 1, current_page = 1, total_page = 1, show = 21;

    public void opengraph(string _idmn)
    {
        web_menu_table q = GianHangMenu_cl.FindById(db, ResolveCurrentChiNhanhId(), _idmn);
        if (q == null)
            return;

        title_web = q.name;
        des = q.description;
        this.Title = title_web;
        string _title_op = "<meta property=\"og:title\" content=\"" + title_web + "\" />";
        string _image = "<meta property=\"og:image\" content=\"" + (q.image ?? "") + "\" />";
        string _description = "<meta name=\"description\" content=\"" + des + "\" />";
        string _description_op = "<meta property=\"og:description\" content=\"" + des + "\" />";
        meta = _title_op + _image + _description + _description_op;
    }

    private string ResolveRequestedMenuId()
    {
        string requestedId = (Request.QueryString["idmn"] ?? string.Empty).Trim();
        if (requestedId != string.Empty)
            return requestedId;

        return GianHangArticle_cl.ResolveDefaultMenuId(db, ResolveCurrentChiNhanhId(), ResolveStoreAccountKey());
    }

    private string ResolveCurrentChiNhanhId()
    {
        return GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request);
    }

    private string ResolveStoreAccountKey()
    {
        return GianHangPublic_cl.ResolveCurrentStoreAccountKey(db, Request);
    }

    private int ResolveCurrentPage()
    {
        object currentPageSession = Session["current_page_home_baiviet"];
        if (currentPageSession == null)
            return 1;

        int pageNumber;
        if (!int.TryParse(currentPageSession.ToString(), out pageNumber))
            return 1;

        return pageNumber < 1 ? 1 : pageNumber;
    }

    private void RedirectToStorefront(string message, string storeAccountKey)
    {
        if (!string.IsNullOrWhiteSpace(message))
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", message, "false", "false", "OK", "alert", "");

        string targetUrl = GianHangRoutes_cl.BuildStorefrontUrl(storeAccountKey);

        Response.Redirect(targetUrl, false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    protected string BuildDetailUrl(object rawId)
    {
        return GianHangArticle_cl.BuildDetailUrl(rawId, ResolveStoreAccountKey());
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string storeAccountKey = ResolveStoreAccountKey();
        if (storeAccountKey == string.Empty)
        {
            RedirectToStorefront("Khong xac dinh duoc gian hang dang hoat dong.", storeAccountKey);
            return;
        }

        GianHangArticleView_cl.ArticleListPageState pageState = GianHangArticleView_cl.BuildListPageState(
            db,
            ResolveCurrentChiNhanhId(),
            storeAccountKey,
            ResolveRequestedMenuId(),
            txt_search.Text,
            ResolveCurrentPage(),
            show);
        if (pageState == null)
        {
            RedirectToStorefront("Danh muc bai viet khong hop le.", storeAccountKey);
            return;
        }

        web_menu_table menu = pageState.Menu;
        if (menu == null)
        {
            RedirectToStorefront("Danh muc bai viet khong hop le.", storeAccountKey);
            return;
        }

        idmn = menu.id.ToString();
        tenmn = menu.name;
        mota = menu.description;

        if (!IsPostBack)
            Session["current_page_home_baiviet"] = "1";

        main();
    }

    public void main()
    {
        opengraph(idmn);
        GianHangArticleView_cl.ArticleListPageState pageState = GianHangArticleView_cl.BuildListPageState(
            db,
            ResolveCurrentChiNhanhId(),
            ResolveStoreAccountKey(),
            idmn,
            txt_search.Text,
            ResolveCurrentPage(),
            show);
        if (pageState == null)
        {
            RedirectToStorefront("Danh muc bai viet khong hop le.", ResolveStoreAccountKey());
            return;
        }

        current_page = pageState.CurrentPage;
        total_page = pageState.TotalPage;
        but_xemtiep.Visible = pageState.CanLoadMore;
        but_quaylai.Visible = pageState.CanGoBack;
        stt = (show * current_page) - show + 1;

        Repeater1.DataSource = pageState.Items;
        Repeater1.DataBind();
    }

    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        Session["search_baiviet_home"] = txt_search.Text.Trim();
        Session["current_page_home_baiviet"] = "1";
        main();
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_home_baiviet"] = int.Parse(Session["current_page_home_baiviet"].ToString()) - 1;
        if (int.Parse(Session["current_page_home_baiviet"].ToString()) < 1)
            Session["current_page_home_baiviet"] = 1;
        main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_home_baiviet"] = int.Parse(Session["current_page_home_baiviet"].ToString()) + 1;
        if (int.Parse(Session["current_page_home_baiviet"].ToString()) > total_page)
            Session["current_page_home_baiviet"] = total_page;
        main();
    }
}
