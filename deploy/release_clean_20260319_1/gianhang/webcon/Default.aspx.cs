using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    public string notifi, meta, title_web, tkchinhanh;
    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString["tkchinhanh"]))
        {
            tkchinhanh = Request.QueryString["tkchinhanh"].ToString().Trim();
            var q_cn = db.chinhanh_tables.Where(p => p.taikhoan_quantri == tkchinhanh);
            if (tk_cl.exist_user(tkchinhanh) && q_cn.Count() != 0)
            {
                Session["ten_tk_chinhanh"] = tkchinhanh;
                Session["id_chinhanh_webcon"] = q_cn.First().id;
                #region meta
                var q = db.config_lienket_chiase_tables;
                if (q.Count() != 0)
                {
                    #region thông tin trang web
                    title_web = q.First().title;
                    string _description = "<meta name=\"description\" content=\"" + q.First().description + "\" />";
                    #endregion

                    #region liên kết chia sẻ (Open Graphp)
                    string _title_op = "<meta property=\"og:title\" content=\"" + q.First().title + "\" />";
                    string _image = "<meta property=\"og:image\" content=\"" + q.First().image + "\" />";
                    string _description_op = "<meta property=\"og:description\" content=\"" + q.First().description + "\" />";
                    #endregion

                    meta = _title_op + _image + _description + _description_op;
                }
                #endregion
                if (!IsPostBack)
                {

                }
            }
            else
            {
                Response.Redirect("/");
            }
        }
        else
        {

            Response.Redirect("/");
        }


    }
}