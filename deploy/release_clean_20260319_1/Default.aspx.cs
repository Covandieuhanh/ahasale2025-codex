using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    private const string DefaultMetaTitle = "AHA SALE";
    private const string DefaultMetaDescription = "Tìm sự vĩ đại của riêng bạn.";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Nếu đang ở chế độ shop và có phiên shop, ép về đăng nhập Home để tránh nhầm portal.
            if (!PortalActiveMode_cl.IsHomeActive() && PortalActiveMode_cl.HasShopCredential())
            {
                Response.Redirect("/dang-nhap", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", false);
            LoadPageMetaSafe();
        }
    }

    private void LoadPageMetaSafe()
    {
        try
        {
            SqlTransientGuard_cl.Execute(() =>
            {
                using (dbDataContext db = new dbDataContext())
                {
                    string _tk = Session["taikhoan_home"] as string;
                    if (!string.IsNullOrEmpty(_tk))
                        ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
                    else
                        ViewState["taikhoan"] = "";

                    var q = (from tk in db.CaiDatChung_tbs
                             where tk.phanloai_trang == "home"
                             select new
                             {
                                 tk.lienket_chiase_title,
                                 tk.lienket_chiase_description,
                                 tk.lienket_chiase_image
                             }).FirstOrDefault();

                    if (q == null)
                    {
                        ApplyMeta(DefaultMetaTitle, DefaultMetaDescription, "");
                        return;
                    }

                    ApplyMeta(
                        q.lienket_chiase_title ?? DefaultMetaTitle,
                        q.lienket_chiase_description ?? DefaultMetaDescription,
                        q.lienket_chiase_image ?? "");
                }
            });
        }
        catch (Exception ex)
        {
            ApplyMeta(DefaultMetaTitle, DefaultMetaDescription, "");
            Log_cl.Add_Log(ex.Message, "root_default_meta", ex.StackTrace);
        }
    }

    private void ApplyMeta(string title, string description, string imageRelativePath)
    {
        string safeTitle = title ?? "";
        string safeDescription = description ?? "";
        string safeImagePath = imageRelativePath ?? "";
        string imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, safeImagePath);

        literal_meta.Text = string.Format(@"
                        <title>{0}</title>
                        <meta name='description' content='{1}' />
                        <meta property='og:title' content='{2}' />
                        <meta property='og:description' content='{3}' />
                        <meta property='og:image' content='{4}' />
                        <meta property='og:type' content='website' />
                        <meta property='og:url' content='{5}' />
                        <meta name='twitter:card' content='summary_large_image' />
                        <meta name='twitter:title' content='{6}' />
                        <meta name='twitter:description' content='{7}' />
                        <meta name='twitter:image' content='{8}' />
                    ", safeTitle, safeDescription, safeTitle, safeDescription, imageUrl, Request.Url.AbsoluteUri, safeTitle, safeDescription, imageUrl);
    }
}
