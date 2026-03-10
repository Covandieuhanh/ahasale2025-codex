using System;
using System.Web;

public partial class home_noi_dung_footer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        try
        {
            string slug = (Request.QueryString["slug"] ?? "").Trim();
            if (string.IsNullOrWhiteSpace(slug))
            {
                Response.Redirect("/", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                HomeFooterArticle_cl.FooterArticleItem article = HomeFooterArticle_cl.GetEnabledBySlug(db, slug);
                if (article == null)
                {
                    Response.Redirect("/", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                string title = string.IsNullOrWhiteSpace(article.DisplayName) ? "Nội dung AhaSale" : article.DisplayName;
                lit_title.Text = Server.HtmlEncode(title);
                lit_body_content.Text = HomeFooterArticle_cl.RenderBodyAsHtml(article.BodyContent);
                lit_updated_at.Text = article.UpdatedAt.HasValue
                    ? article.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm")
                    : "Chưa cập nhật";

                Title = title + " | AhaSale";
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan_home"] as string;
            if (!string.IsNullOrEmpty(_tk))
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);

            Response.Redirect("/", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
