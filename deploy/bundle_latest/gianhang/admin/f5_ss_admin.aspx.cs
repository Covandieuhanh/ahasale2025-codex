using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class f5_ss : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null) Session["user"] = "";

        if (Session["user"].ToString() == "")
        {
            string _cookie_user = "";
            if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
            if (_cookie_user != "")//nếu có lưu cc
            {
                string _user_mahoa = _cookie_user;//lấy giá trị user cookie đã đc mã hóa
                string _user = encode_class.decrypt(_user_mahoa);//giải mã ra sẽ được usernamer
                if (tk_cl.exist_user(_user))//nếu user này tồn tại
                {
                    Session["user"] = _user;
                    Session["chinhanh"] = tk_cl.return_chinhanh(_user);
                    Session["nganh"] = tk_cl.return_object(_user).id_nganh;
                    Session["user_parent"] = "admin";
                    Response.Redirect("/gianhang/admin");
                }
            }
            else
            {
                Session["user"] = "";
                Session["chinhanh"] = "";
                Session["nganh"] = "";
                if (Request.Cookies["save_user_admin_aka_1"] != null)
                    Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                if (Request.Cookies["save_pass_admin_aka_1"] != null)
                    Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                if (Request.Cookies["save_url_admin_aka_1"] != null)
                    Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("/gianhang/admin/login.aspx");
            }
        }
        else
        {
            Response.Redirect("/gianhang/admin");
        }
    }
}
