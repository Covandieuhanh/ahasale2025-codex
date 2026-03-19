using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Uc_Home_DanhMuc_icon_Home_UC : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                SqlTransientGuard_cl.Execute(() =>
                {
                    using (dbDataContext db = new dbDataContext())
                    {
                        // Ưu tiên lấy danh mục con của root "Danh mục" (kyhieu=web).
                        var root = db.DanhMuc_tbs.FirstOrDefault(p =>
                            p.id_level == 1
                            && p.bin == false
                            && p.kyhieu_danhmuc == "web"
                            && (
                                (p.name != null && p.name.Trim().ToLower() == "danh mục")
                                || (p.name_en != null && (p.name_en.Trim().ToLower() == "danh-muc" || p.name_en.Trim().ToLower() == "danhmuc"))
                            ));

                        IQueryable<DanhMuc_tb> query;
                        if (root != null)
                        {
                            query = db.DanhMuc_tbs
                                .Where(p => p.id_parent == root.id.ToString() && p.bin == false && p.kyhieu_danhmuc == "web");
                        }
                        else
                        {
                            // Fallback: nếu không có root "Danh mục" thì lấy các mục level 2.
                            query = db.DanhMuc_tbs
                                .Where(p => p.id_level == 2 && p.bin == false && p.kyhieu_danhmuc == "web");
                        }

                        var danhMucList = query
                            .OrderBy(d => d.rank)
                            .ToList();

                        Repeater1.DataSource = danhMucList;
                        Repeater1.DataBind();
                    }
                });
            }
            catch (Exception ex)
            {
                Repeater1.DataSource = new List<object>();
                Repeater1.DataBind();
                Log_cl.Add_Log(ex.Message, "home_categories", ex.StackTrace);
            }
        }
    }

    protected string ResolveCategoryImage(object imageRaw)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        string image = (imageRaw ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(image))
            return fallback;

        if (image.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(image, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        if (image.StartsWith("~/", StringComparison.Ordinal))
            image = image.Substring(1);
        if (!image.StartsWith("/", StringComparison.Ordinal))
            image = "/" + image;

        if (IsMissingUploadFile(image))
            return fallback;

        return image;
    }

    private bool IsMissingUploadFile(string relativeUrl)
    {
        return Helper_cl.IsMissingUploadFile(relativeUrl);
    }
}
