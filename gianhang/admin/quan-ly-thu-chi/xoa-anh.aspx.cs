using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class quan_ly_ban_hang_thuong_quy_cap_duoi : System.Web.UI.Page
{
    public string id;
    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();
    hinhanh_thuchi_class hatc_cl = new hinhanh_thuchi_class();
    public string user, user_parent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!GianHangSystemAdminGuard_cl.EnsurePageAccess(this))
            return;

        user = GianHangAdminContext_cl.ResolveDisplayAccountKey();
        if (string.IsNullOrWhiteSpace(user))
        {
            Response.Redirect(GianHangAdminBridge_cl.ResolveSessionRecoveryUrl(HttpContext.Current, "/gianhang/admin"));
            return;
        }
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            if (hatc_cl.exist_id(id, user_parent))
            {
                string _idtc = hatc_cl.return_object(id).id_thuchi;
                if (!string.IsNullOrWhiteSpace(Request.QueryString["_link"]))
                {
                    string _link = Request.QueryString["link"].ToString().ToLower();
                    if (File.Exists(Server.MapPath("~" + _link)))
                        File.Delete(Server.MapPath("~" + _link));
                }
                hatc_cl.del(id);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa ảnh thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-thu-chi/edit.aspx?id="+ _idtc);
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/Default.aspx");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/Default.aspx");
        }
    }
}
