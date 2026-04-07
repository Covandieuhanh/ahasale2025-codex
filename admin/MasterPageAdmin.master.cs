using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_MasterPageAdmin : System.Web.UI.MasterPage
{
    private void AppendBodyClass(string className)
    {
        if (bodyAdmin == null || string.IsNullOrWhiteSpace(className))
            return;

        string current = (bodyAdmin.Attributes["class"] ?? "").Trim();
        string[] parts = current.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Contains(className, StringComparer.OrdinalIgnoreCase))
            return;

        bodyAdmin.Attributes["class"] = string.IsNullOrWhiteSpace(current)
            ? className
            : current + " " + className;
    }

    private static string ToCssSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var buffer = new List<char>(value.Length);
        foreach (char ch in value.Trim().ToLowerInvariant())
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
                buffer.Add(ch);
            else if (ch == '-' || ch == '_' || ch == ' ')
                buffer.Add('-');
        }

        string slug = new string(buffer.ToArray());
        while (slug.Contains("--"))
            slug = slug.Replace("--", "-");
        return slug.Trim('-');
    }

    private void ApplyRouteDisplayMode()
    {
        string routeView = (Request.QueryString["view"] ?? "").Trim();
        string currentPath = (Request.Url.AbsolutePath ?? "").Trim().ToLowerInvariant();

        if (routeView != "")
        {
            AppendBodyClass("admin-route-fullview");
            AppendBodyClass("admin-fullpage-view");
        }

        string viewSlug = ToCssSlug(routeView);
        if (!string.IsNullOrWhiteSpace(viewSlug))
            AppendBodyClass("admin-route-view-" + viewSlug);

        if (currentPath == "")
            return;

        if (IsDirectFullPageRoute(currentPath))
        {
            AppendBodyClass("admin-route-fullview");
            AppendBodyClass("admin-fullpage-view");
            AppendBodyClass("admin-route-path-" + ToCssSlug(currentPath.Replace("/", "-")));
        }
    }

    private static bool IsDirectFullPageRoute(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        return path == "/admin/doi-mat-khau/default.aspx"
            || path == "/admin/quen-mat-khau/default.aspx"
            || path == "/admin/vi-token-diem/default.aspx"
            || path == "/admin/he-thong-san-pham/ban-the.aspx"
            || path == "/admin/he-thong-san-pham/chi-tiet-giao-dich.aspx"
            || path == "/admin/lich-su-chuyen-diem/chuyen-diem.aspx"
            || path == "/admin/phat-hanh-the/them-moi.aspx"
            || path == "/admin/quan-ly-banner/them-moi.aspx"
            || path == "/admin/quan-ly-bai-viet/them-moi.aspx"
            || path == "/admin/quan-ly-bai-viet/bo-loc.aspx"
            || path == "/admin/quan-ly-bai-viet/chinh-sua.aspx"
            || path == "/admin/quan-ly-bai-viet/xuat-du-lieu.aspx"
            || path == "/admin/quan-ly-bai-viet/ban-in.aspx"
            || path == "/admin/quan-ly-menu/them-moi.aspx"
            || path == "/admin/quan-ly-menu/bo-loc.aspx"
            || path == "/admin/quan-ly-menu/chinh-sua.aspx"
            || path == "/admin/quan-ly-menu/xuat-du-lieu.aspx"
            || path == "/admin/quan-ly-menu/ban-in.aspx"
            || path == "/admin/quan-ly-tai-khoan/them-moi.aspx"
            || path == "/admin/quan-ly-tai-khoan/bo-loc.aspx"
            || path == "/admin/quan-ly-tai-khoan/chinh-sua.aspx"
            || path == "/admin/quan-ly-tai-khoan/phan-quyen.aspx"
            || path == "/admin/quan-ly-thong-bao/bo-loc.aspx"
            || path == "/admin/quan-ly-thong-bao/xuat-du-lieu.aspx"
            || path == "/admin/quan-ly-thong-bao/ban-in.aspx"
            || path == "/admin/yeu-cau-tu-van/bo-loc.aspx"
            || path == "/admin/yeu-cau-tu-van/xuat-du-lieu.aspx"
            || path == "/admin/yeu-cau-tu-van/ban-in.aspx";
    }

    private bool TryRedirectLegacyTopView()
    {
        string topView = (Request.QueryString["topview"] ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(topView))
            return false;

        string safeBackUrl = AdminFullPageRoute_cl.SanitizeAdminReturnUrl(Request.RawUrl, "/admin/default.aspx");
        string redirectUrl = safeBackUrl;

        if (topView == "change-password")
            redirectUrl = "/admin/doi-mat-khau/default.aspx?return_url=" + HttpUtility.UrlEncode(safeBackUrl);

        Response.Redirect(redirectUrl, false);
        Context.ApplicationInstance.CompleteRequest();
        return true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (TryRedirectLegacyTopView())
            return;

        ApplyRouteDisplayMode();

        if (!AdminRolePolicy_cl.EnsureMasterRouteAccess(this.Page))
            return;

        if (!AdminManagementSpace_cl.EnsureSelectedSpaceRouteAccess(this.Page))
            return;

        if (!AdminAccessGuard_cl.EnsurePageAccess(this.Page))
            return;

        if (!IsPostBack)
        {
            try
            {
                using (dbDataContext db = new dbDataContext())
                {
                    #region Favicon & icon mobile
                    var q = (from tk in db.CaiDatChung_tbs
                             where tk.phanloai_trang == "admin"
                             select new { tk.thongtin_icon, tk.thongtin_apple_touch_icon }).FirstOrDefault();

                    if (q != null)
                    {
                        string baseUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);

                        string iconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_icon);
                        string appleTouchIconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_apple_touch_icon);

                        string iconsHtml = string.Format(@"
                <!-- Favicon -->
                <link rel='icon' href='{0}' sizes='16x16' type='image/x-icon'>
                <link rel='icon' href='{1}' sizes='32x32' type='image/x-icon'>
                <link rel='icon' href='{2}' sizes='48x48' type='image/x-icon'>

                <!-- Apple Touch Icon -->
                <link rel='apple-touch-icon' href='{3}' sizes='180x180'>
                <link rel='apple-touch-icon' href='{4}' sizes='167x167'>
                <link rel='apple-touch-icon' href='{5}' sizes='152x152'>
                <link rel='apple-touch-icon' href='{6}' sizes='120x120'>

                <!-- Android Icons -->
                <link rel='icon' href='{7}' sizes='192x192'>
                <link rel='icon' href='{8}' sizes='144x144'>
                ", iconUrl, iconUrl, iconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, iconUrl, iconUrl);

                        literal_fav_icon.Text = iconsHtml;
                    }
                    #endregion
                    #region lưu nội dung thông báo nếu có
                    if (Session["thongbao"] != null)
                    {
                        ViewState["thongbao"] = Session["thongbao"].ToString();
                        Session["thongbao"] = null;
                    }
                    #endregion
                }
            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                {
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                }
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }
}
