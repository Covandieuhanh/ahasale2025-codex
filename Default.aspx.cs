using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", false);

            using (dbDataContext db = new dbDataContext())
            {
                // lấy tài khoản
                string _tk = Session["taikhoan_home"] as string;
                if (!string.IsNullOrEmpty(_tk))
                    ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
                else
                    ViewState["taikhoan"] = "";

                // META HOME
                var q = (from tk in db.CaiDatChung_tbs
                         where tk.phanloai_trang == "home"
                         select new
                         {
                             tk.lienket_chiase_title,
                             tk.lienket_chiase_description,
                             tk.lienket_chiase_image
                         }).FirstOrDefault();

                if (q != null)
                {
                    string title = q.lienket_chiase_title ?? "";
                    string description = q.lienket_chiase_description ?? "";
                    string imageRelativePath = q.lienket_chiase_image ?? "";

                    string imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, imageRelativePath);

                    string metaTags = string.Format(@"
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
                    ", title, description, title, description, imageUrl, Request.Url.AbsoluteUri, title, description, imageUrl);
                    literal_meta.Text = metaTags;
                }
            }
        }
    }
}