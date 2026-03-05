using System;
using System.Linq;
using System.Web;

public partial class home_page_ds_bai_viet : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            check_login_cl.check_login_home("none", "none", false);

            if (string.IsNullOrWhiteSpace(Request.QueryString["idmn"]))
            {
                Session["thongbao_home"] = thongbao_class.metro_dialog_onload(
                    "Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/");
                return;
            }

            string idmn = (Request.QueryString["idmn"] ?? "").Trim();
            hdnIdmn.Value = idmn;

            using (dbDataContext db = new dbDataContext())
            {
                var dm = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == idmn);
                if (dm == null)
                {
                    Session["thongbao_home"] = thongbao_class.metro_dialog_onload(
                        "Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/");
                    return;
                }

                string title = dm.name;
                string description = dm.description;
                string imageRelativePath = dm.image;
                string imageUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}{imageRelativePath}";

                literal_meta.Text = $@"
                    <title>{title}</title>
                    <meta name='description' content='{description}' />
                    <meta property='og:title' content='{title}' />
                    <meta property='og:description' content='{description}' />
                    <meta property='og:image' content='{imageUrl}' />
                    <meta property='og:type' content='website' />
                    <meta property='og:url' content='{Request.Url.AbsoluteUri}' />
                    <meta name='twitter:card' content='summary_large_image' />
                    <meta name='twitter:title' content='{title}' />
                    <meta name='twitter:description' content='{description}' />
                    <meta name='twitter:image' content='{imageUrl}' />
                ";

                // ✅ truyền idmn vào UC để UC lọc dữ liệu
                UcDanhChoBanMoiNhat.Idmn = idmn;
                UcDanhChoBanMoiNhat.TitleText = title; // nếu bạn muốn title card đổi theo danh mục
            }
        }
    }
}
