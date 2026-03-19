using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class();
    public string notifi;
    private config_social_media_table EnsureSocial()
    {
        config_social_media_table ob = db.config_social_media_tables.FirstOrDefault();
        if (ob != null)
            return ob;

        ob = new config_social_media_table
        {
            facebook = "",
            zalo = "",
            youtube = "",
            instagram = "",
            twitter = "",
            tiktok = "",
            wechat = "",
            linkedin = "",
            whatsapp = ""
        };
        db.config_social_media_tables.InsertOnSubmit(ob);
        db.SubmitChanges();
        return ob;
    }
    public void main()
    {
        config_social_media_table _ob = EnsureSocial();
        if (!IsPostBack)
        {
            TextBox1.Text = _ob.facebook;
            TextBox2.Text = _ob.zalo;
            TextBox3.Text = _ob.youtube;
            TextBox4.Text = _ob.instagram;
            TextBox5.Text = _ob.twitter;
            TextBox6.Text = _ob.tiktok;
            TextBox7.Text = _ob.wechat;
            TextBox8.Text = _ob.linkedin;
            TextBox9.Text = _ob.whatsapp;
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q1_4";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = "";if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
        string _url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
        string _kq = bcorn_class.check_login(Session["user"].ToString(), _cookie_user, _cookie_pass, _url, _quyen);
        if (_kq != "")//nếu có thông báo --> có lỗi --> reset --> bắt login lại
        {
            if (_kq == "baotri") Response.Redirect("/baotri.aspx");
            else
            {
                if (_kq == "1") Response.Redirect("/gianhang/admin/login.aspx");//hết Session, hết Cookie
                else
                {
                    if (_kq == "2")//k đủ quyền
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        Session["notifi"] = _kq; Session["user"] = "";
                        Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("/gianhang/admin/login.aspx");
                    }
                }
            }
        }
        #endregion

        main();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_4") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            config_social_media_table _ob = EnsureSocial();
            _ob.facebook = TextBox1.Text.Trim();
            _ob.zalo = TextBox2.Text.Trim();
            _ob.youtube = TextBox3.Text.Trim();
            _ob.instagram = TextBox4.Text.Trim();
            _ob.twitter = TextBox5.Text.Trim();
            _ob.tiktok = TextBox6.Text.Trim();
            _ob.wechat = TextBox7.Text.Trim();
            _ob.linkedin = TextBox8.Text.Trim();
            _ob.whatsapp = TextBox9.Text.Trim();
            db.SubmitChanges();

            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
            Response.Redirect("/gianhang/admin/cau-hinh-chung/link-social-media.aspx");
        }
    }
}
