using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_left_uc : System.Web.UI.UserControl
{
    public string loi, tuvan;

    public string MenuActive(params string[] urls)
    {
        string currentUrl = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
        foreach (string url in urls)
        {
            if (currentUrl == (url ?? "").ToLower().Trim())
                return "active";
        }
        return "";
    }

    public string MenuActiveTaiKhoanScope(string scope)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/quan-ly-tai-khoan/default.aspx")
            return "";

        string currentScope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
        string targetScope = (scope ?? "").Trim().ToLowerInvariant();
        if (currentScope == targetScope)
            return "active";
        return "";
    }

    private static string ResolveTitle(string url)
    {
        switch ((url ?? "").ToLower().Trim())
        {
            case "/admin/quan-ly-menu/default.aspx":
                return "Quản lý menu";
            case "/admin/quan-ly-bai-viet/default.aspx":
            case "/admin/quan-ly-bai-viet/in.aspx":
                return "Quản lý bài viết";
            case "/admin/quan-ly-banner/default.aspx":
                return "Quản lý banner";
            case "/admin/quan-ly-gop-y/default.aspx":
                return "Quản lý góp ý";
            case "/admin/quan-ly-thong-bao/default.aspx":
            case "/admin/quan-ly-thong-bao/in.aspx":
                return "Quản lý thông báo";
            case "/admin/yeu-cau-tu-van/default.aspx":
                return "Yêu cầu tư vấn";
            case "/admin/lich-su-chuyen-diem/default.aspx":
                return "Lịch sử chuyển điểm";
            case "/admin/cai-dat-trang-chu/default.aspx":
                return "Cài đặt trang chủ";
            case "/admin/quan-ly-tai-khoan/default.aspx":
                return "Quản lý tài khoản";
            case "/admin/duyet-yeu-cau-len-cap.aspx":
                return "Duyệt yêu cầu lên cấp";
            case "/admin/duyet-gian-hang-doi-tac.aspx":
                return "Duyệt gian hàng đối tác";
            case "/admin/phat-hanh-the.aspx":
                return "Phát hành thẻ";
            case "/admin/motacapbac.aspx":
                return "Mô tả cấp bậc";
            case "/admin/he-thong-san-pham/ban-san-pham.aspx":
                return "Bán sản phẩm";
            default:
                return "Trang chủ admin";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string _url = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
                string title = ResolveTitle(_url);
                if (_url == "/admin/quan-ly-tai-khoan/default.aspx")
                {
                    string scope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
                    if (scope == "admin") title = "Quản lý tài khoản admin";
                    else if (scope == "home") title = "Quản lý tài khoản home";
                    else if (scope == "shop") title = "Quản lý tài khoản shop";
                }
                Session["title"] = title;

                using (dbDataContext db = new dbDataContext())
                {
                    #region ĐẾM LỖI HỆ THỐNG CHƯA XỬ LÝ
                    int q_loi = db.Log_tbs.Count(p => p.trangthai == "Chưa sửa" && p.bin == false);
                    if (q_loi < 100)
                        loi = q_loi.ToString();
                    else
                        loi = "99+";
                    #endregion
                }
            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }
}
