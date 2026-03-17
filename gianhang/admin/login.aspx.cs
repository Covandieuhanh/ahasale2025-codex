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
    public string notifi, meta;
    protected void Page_Load(object sender, EventArgs e)
    {
        //Session["user"] = ""; Session["title"] = "";
        //if (Request.Cookies["save_user_admin_aka_1"] != null)
        //    Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
        //if (Request.Cookies["save_pass_admin_aka_1"] != null)
        //    Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
        //if (Request.Cookies["save_url_admin_aka_1"] != null)
        //    Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
       // Response.Write(encode_class.encode_md5(encode_class.encode_sha1("12345678")));

        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = ""; Session["user_parent"] = "admin";

        if (Session["user"].ToString() != "")
            Response.Redirect("/gianhang/admin");

        if (!IsPostBack)
        {
            notifi = Session["notifi"].ToString();
            Session["notifi"] = "";

            var list_chinhanh = (from ob1 in db.chinhanh_tables.ToList()
                                 select new { id = ob1.id, ten = ob1.ten, }
                               );
            ddl_chinhanh.DataSource = list_chinhanh;
            ddl_chinhanh.DataTextField = "ten";
            ddl_chinhanh.DataValueField = "id";
            ddl_chinhanh.DataBind();
            ddl_chinhanh.Items.Insert(0, new ListItem("Chọn chi nhánh", ""));

            // Convenience for single-branch local deployments: avoid accidental empty branch on login.
            if (ddl_chinhanh.Items.Count == 2)
                ddl_chinhanh.SelectedIndex = 1;
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
    }


    protected void but_login_Click(object sender, EventArgs e)
    {
        string id_chinhanh = ddl_chinhanh.SelectedValue.ToString();
        string _user = txt_user.Text.Trim().ToLower();
        string _pass = txt_pass.Text;
        if (id_chinhanh == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn chi nhánh.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_user == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tài khoản.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (_pass == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    if (tk_cl.exist_user(_user))
                    {
                        taikhoan_table_2023 _ob = tk_cl.return_object(_user);
                        string _pass_mahoa = encode_class.encode_md5(encode_class.encode_sha1(_pass));
                        if (_ob.matkhau == _pass_mahoa)
                        {
                            if (_ob.trangthai == "Đang hoạt động")
                            {
                                if (_ob.id_chinhanh == id_chinhanh)
                                {
                                    //lưu cookier với tên tài khoản để đăng nhập trong 1 năm
                                    app_cookie_policy_class.persist_cookie(
                                        HttpContext.Current,
                                        app_cookie_policy_class.admin_user_cookie,
                                        encode_class.encrypt(_user),
                                        365
                                    );

                                    //lưu cookier với pass để đăng nhập trong 1 năm
                                    app_cookie_policy_class.persist_cookie(
                                        HttpContext.Current,
                                        app_cookie_policy_class.admin_pass_cookie,
                                        _pass_mahoa,
                                        365
                                    );

                                    Session["user"] = _user;
                                    Session["chinhanh"] = id_chinhanh;
                                    Session["nganh"] = tk_cl.return_object(_user).id_nganh;
                                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "2000", "warning");
                                    Response.Redirect("/gianhang/admin");
                                }
                                else
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Chi nhánh không đúng.", "false", "false", "OK", "alert", ""), true);
                            }
                            else
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);

                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Mật khẩu không đúng.", "false", "false", "OK", "alert", ""), true);

                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản không tồn tại.", "false", "false", "OK", "alert", ""), true);
                }
            }
        }
    }
}
