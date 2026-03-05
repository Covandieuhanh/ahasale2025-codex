using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPageHome : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (dbDataContext db = new dbDataContext())
            {


                #region Favicon & icon mobile
                var q = (from tk in db.CaiDatChung_tbs
                         where tk.phanloai_trang == "home"
                         select new { tk.thongtin_icon, tk.thongtin_apple_touch_icon }).FirstOrDefault();

                if (q != null)
                {
                    string baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";

                    string iconUrl = $"{baseUrl}{q.thongtin_icon}";
                    string appleTouchIconUrl = $"{baseUrl}{q.thongtin_apple_touch_icon}";

                    string iconsHtml = $@"
                <!-- Favicon -->
                <link rel='icon' href='{iconUrl}' sizes='16x16' type='image/x-icon'>
                <link rel='icon' href='{iconUrl}' sizes='32x32' type='image/x-icon'>
                <link rel='icon' href='{iconUrl}' sizes='48x48' type='image/x-icon'>

                <!-- Apple Touch Icon -->
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='180x180'>
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='167x167'>
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='152x152'>
                <link rel='apple-touch-icon' href='{appleTouchIconUrl}' sizes='120x120'>

                <!-- Android Icons -->
                <link rel='icon' href='{iconUrl}' sizes='192x192'>
                <link rel='icon' href='{iconUrl}' sizes='144x144'>
                ";

                    literal_fav_icon.Text = iconsHtml;
                }
                #endregion
                #region lưu nội dung thông báo nếu có
                if (Session["thongbao_home"] != null)
                {
                    ViewState["thongbao_home"] = Session["thongbao_home"].ToString();
                    Session["thongbao_home"] = null;

                }
                #endregion

                #region kiểm tra trạng thái chờ Trao đổi (nếu có)

                string _tk = Session["taikhoan_home"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // nếu có đăng nhập
                {
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                    var q1 = db.DonHang_tbs.FirstOrDefault(p =>
                        p.nguoiban == _tk &&
                        (
                            p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                            || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                        ));
                    if (q1 != null)
                    {
                        Response.Redirect("/home/cho-thanh-toan.aspx");
                    }
                }
                else//nếu k đăng nhập
                {
                    _tk = "";
                }
                #endregion
            }
        }
    }

}
