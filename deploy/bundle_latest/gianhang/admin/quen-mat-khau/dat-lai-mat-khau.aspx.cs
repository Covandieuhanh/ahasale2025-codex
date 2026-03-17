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
    public string notifi, meta, user;
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
        if (Session["user_reset_pass_admin"] != null)
        {
            user = Session["user_reset_pass_admin"].ToString();
            if (tk_cl.exist_user(user))
            {

            }
            else
            {
                //Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản không tồn tại.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/quen-mat-khau/default.aspx");
            }
        }
        else
        {
            //Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/quen-mat-khau/default.aspx");
        }
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        //code = rd_cl.random_number(6);
        string _pass = txt_pass.Text;
        string _pass_1 = txt_pass_1.Text;
        if (_pass == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu mới.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_pass_1 == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập lại mật khẩu mới.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (_pass_1 != _pass)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Mật khẩu mới không trùng nhau.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    tk_cl.change_pass(user, _pass_1);

                    //lưu cookier với tên tài khoản để đăng nhập trong 1 năm
                    HttpCookie save_user_admin_aka_1 = new HttpCookie("save_user_admin_aka_1");
                    save_user_admin_aka_1.Value = encode_class.encrypt(user);
                    save_user_admin_aka_1.Expires = DateTime.Now.AddSeconds(365);
                    Response.Cookies.Add(save_user_admin_aka_1);

                    //lưu cookier với pass để đăng nhập trong 1 năm
                    HttpCookie save_pass_admin_aka_1 = new HttpCookie("save_pass_admin_aka_1");
                    save_pass_admin_aka_1.Value = encode_class.encode_md5(encode_class.encode_sha1(_pass));
                    save_pass_admin_aka_1.Expires = DateTime.Now.AddSeconds(365);
                    Response.Cookies.Add(save_pass_admin_aka_1);

                    Session["user"] = user;
                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đặt lại mật khẩu thành công.", "2000", "warning");
                    Session["user_reset_pass_admin"] = null;
                    Response.Redirect("/gianhang/admin");
                }
            }
        }
    }
}