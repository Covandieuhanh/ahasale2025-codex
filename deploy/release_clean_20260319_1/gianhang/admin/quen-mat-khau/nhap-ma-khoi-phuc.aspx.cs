using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default2 : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    random_class rd_cl = new random_class();
    public string notifi, meta, user, email;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"].ToString() != "")
        {
            if (Request.Cookies["save_url_admin_aka_1"] != null)
                Response.Redirect(Request.Cookies["save_url_admin_aka_1"].Value);
            else
                Response.Redirect("/gianhang/admin");
        }

        if (!IsPostBack)
        {
            notifi = Session["notifi"].ToString();
            Session["notifi"] = "";
        }
        #region meta
        var q = db.config_thongtin_tables;
        if (q.Count() != 0)
        {
            string _icon = "<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"" + q.First().icon + "\" />";
            string _appletouch = "<link rel=\"apple-touch-icon\" href=\"" + q.First().apple_touch_icon + "\" />";
            meta = _icon;
        }
        #endregion
        if (!string.IsNullOrWhiteSpace(Request.QueryString["user"]))
        {
            user = Request.QueryString["user"].ToString().Trim();
            if (tk_cl.exist_user(user))
            {
                email = tk_cl.return_object(user).email;
                if (email == "")
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản này chưa cập nhật email.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin/quen-mat-khau/default.aspx");
                }
                else
                    email = "***" + email.Substring(3, email.Length-3);
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản không tồn tại.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/quen-mat-khau/default.aspx");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/quen-mat-khau/default.aspx");
        }
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        //code = rd_cl.random_number(6);
        string _makhoiphuc = txt_makhoiphuc.Text.Trim().ToLower();
        if (_makhoiphuc == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mã khôi phục.", "false", "false", "OK", "alert", ""), true);
        else
        {
            taikhoan_table_2023 _ob = tk_cl.return_object(user);
            if (_ob.makhoiphuc != txt_makhoiphuc.Text.Trim())
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Mã khôi phục không đúng.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (_ob.hsd_makhoiphuc.Value.AddMinutes(5) < DateTime.Now)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Mã khôi phục đã hết hạn.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    Session["user_reset_pass_admin"] = user;
                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xác nhận mã khôi phục thành công.", "4000", "warning");
                    Response.Redirect("/gianhang/admin/quen-mat-khau/dat-lai-mat-khau.aspx");
                }
            }
        }
    }
}