using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage_Tabler_Tabler2 : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Giữ nguyên action hiện tại cho shop portal để không mất marker khi postback.
        if (PortalRequest_cl.IsShopPortalRequest() && form1 != null)
        {
            string rawUrl = (Request == null) ? "" : (Request.RawUrl ?? "");
            if (!string.IsNullOrEmpty(rawUrl))
                form1.Action = rawUrl;
        }

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

                string _tk = PortalRequest_cl.GetCurrentAccount();
                if (!string.IsNullOrEmpty(_tk)) // nếu có đăng nhập
                {
                    var q1 = db.DonHang_tbs.FirstOrDefault(p =>
                        p.nguoiban == _tk &&
                        (
                            p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                            || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                        ));
                    if (q1 != null)
                    {
                        bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
                        Response.Redirect(isShopPortal ? "/shop/cho-thanh-toan" : "/home/cho-thanh-toan.aspx");
                    }
                }
                else//nếu k đăng nhập
                {
                    _tk = "";
                }
                #endregion

                // ✅ HIỆN MODAL NẾU TRANG TRƯỚC GỬI QUA
                if (Session["home_modal_msg"] != null)
                {
                    string msg = Session["home_modal_msg"].ToString();
                    string title = (Session["home_modal_title"] ?? "Thông báo").ToString();
                    string type = (Session["home_modal_type"] ?? "info").ToString();

                    // clear session để không bị hiện lại
                    Session["home_modal_msg"] = null;
                    Session["home_modal_title"] = null;
                    Session["home_modal_type"] = null;

                    Helper_Tabler_cl.ShowModal(this.Page, msg, title, true, type);
                }
            }
        }
    }


}
