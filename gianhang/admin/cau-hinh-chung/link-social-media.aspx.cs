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

    private bool HasPermission(string permissionKey)
    {
        string currentUser = GianHangAdminContext_cl.ResolveDisplayAccountKey();
        if (string.IsNullOrEmpty(currentUser) || string.IsNullOrEmpty(permissionKey))
            return false;

        return bcorn_class.check_quyen(currentUser, permissionKey) != "2";
    }

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
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q1_4");
        if (access == null)
            return;

        main();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!HasPermission("q1_4"))
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
