using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class danh_sach_bai_viet : System.Web.UI.Page
{
    public string notifi = "", idbv = "", noidung = "", name_mn, name_mn_en, phanloai_menu, url_menu;
    public string title_web = "", des = "", image = "", gia = "";
    public string meta = "";

    private readonly dbDataContext db = new dbDataContext();

    public void opengraph(GH_BaiViet_tb article)
    {
        if (article == null)
            return;

        title_web = article.name ?? string.Empty;
        des = article.description ?? string.Empty;
        image = article.image ?? string.Empty;
        this.Title = title_web;
        string _title_op = "<meta property=\"og:title\" content=\"" + title_web + "\" />";
        string _image = "<meta property=\"og:image\" content=\"" + image + "\" />";
        string _description = "<meta name=\"description\" content=\"" + des + "\" />";
        string _description_op = "<meta property=\"og:description\" content=\"" + des + "\" />";
        meta = _title_op + _image + _description + _description_op;
    }

    private string ResolveStoreAccountKey()
    {
        return GianHangPublic_cl.ResolveCurrentStoreAccountKey(db, Request);
    }

    private void RedirectToStorefront(string message)
    {
        string storeAccountKey = ResolveStoreAccountKey();
        if (!string.IsNullOrWhiteSpace(message))
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", message, "false", "false", "OK", "alert", "");

        string targetUrl = GianHangRoutes_cl.BuildStorefrontUrl(storeAccountKey);

        Response.Redirect(targetUrl, false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    protected string BuildArticleUrl(object rawId)
    {
        return GianHangArticle_cl.BuildDetailUrl(rawId, ResolveStoreAccountKey());
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        idbv = (Request.QueryString["idbv"] ?? string.Empty).Trim();
        if (idbv == string.Empty)
        {
            RedirectToStorefront("Trang ban yeu cau khong hop le.");
            return;
        }

        string chiNhanhId = GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request);
        string storeAccountKey = ResolveStoreAccountKey();
        GianHangArticleView_cl.ArticleDetailPageState pageState = GianHangArticleView_cl.BuildDetailPageState(
            db,
            chiNhanhId,
            idbv,
            storeAccountKey);
        if (pageState == null || pageState.Article == null || pageState.Menu == null)
        {
            RedirectToStorefront("Trang ban yeu cau khong hop le.");
            return;
        }

        GH_BaiViet_tb article = pageState.Article;
        web_menu_table menu = pageState.Menu;
        name_mn = menu.name ?? string.Empty;
        name_mn_en = menu.name_en ?? string.Empty;
        phanloai_menu = menu.phanloai ?? string.Empty;
        url_menu = pageState.MenuUrl ?? GianHangArticle_cl.BuildListUrl(article.id_category, storeAccountKey);

        opengraph(article);
        noidung = article.content_post ?? string.Empty;
        Repeater2.DataSource = pageState.RelatedArticles;
        Repeater2.DataBind();
        if (pageState.RelatedArticles == null || pageState.RelatedArticles.Count == 0)
            Panel1.Visible = false;
    }
}
