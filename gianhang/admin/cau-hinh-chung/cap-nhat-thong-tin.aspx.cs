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
    public string notifi;
    public void main()
    {
        config_thongtin_table _ob = db.config_thongtin_tables.First();

        if (!IsPostBack)
        {
            txt_tencongty.Text = _ob.tencongty;
            txt_slogan.Text = _ob.slogan;
            txt_diachi.Text = _ob.diachi;
            txt_link_googlemap.Text = _ob.link_googlemap;
            txt_hotline.Text = _ob.hotline;
            txt_zalo.Text = _ob.zalo;
            txt_email.Text = _ob.email;
            txt_masothue.Text = _ob.masothue;
        }

        if (_ob.icon != "")
        {
            Button2.Visible = true;
            Label2.Text = "<img src='" + _ob.icon + "' style='max-width: 80px' />";
        }
        else
        {
            Button2.Visible = false; Label2.Text = "";
        }
        if (_ob.apple_touch_icon != "")
        {
            Button3.Visible = true;
            Label3.Text = "<img src='" + _ob.apple_touch_icon + "' style='max-width: 80px' />";
        }
        else
        {
            Button3.Visible = false; Label3.Text = "";
        }
        if (_ob.logo != "")
        {
            Button4.Visible = true;
            Label4.Text = "<img src='" + _ob.logo + "' style='max-width: 80px' />";
        }
        else
        {
            Button4.Visible = false; Label4.Text = "";
        }
        if (_ob.logo1 != "")
        {
            Button5.Visible = true;
            Label5.Text = "<img src='" + _ob.logo1 + "' style='max-width: 80px' />";
        }
        else
        {
            Button5.Visible = false; Label5.Text = "";
        }
        if (_ob.logo_in_hoadon != "")
        {
            Button6.Visible = true;
            Label6.Text = "<img src='" + _ob.logo_in_hoadon + "' style='max-width: 80px' />";
        }
        else
        {
            Button6.Visible = false; Label6.Text = "";
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q1_3";
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
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            bool _checkloi = false;
            string _favicon = "", _icon_mobile = "", _logo = "", _logo1 = "", _logo_in_hoadon = "";

            if (!Directory.Exists(Server.MapPath("~/uploads/images/config/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/images/config/"));

            config_thongtin_table _ob = db.config_thongtin_tables.First();
            _favicon = _ob.icon; _icon_mobile = _ob.apple_touch_icon; _logo = _ob.logo; _logo1 = _ob.logo1; _logo_in_hoadon = _ob.logo_in_hoadon;


            if (FileUpload2.HasFile)//_favicon
            {
                string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.icon);//xóa ảnh cũ
                        _favicon = "/uploads/images/config/" + Guid.NewGuid() + _ext;
                        FileUpload2.SaveAs(Server.MapPath("~" + _favicon));//lưu ảnh mới
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh favicon quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh favicon không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }

            if (FileUpload3.HasFile)//_icon_mobile
            {
                string _ext = Path.GetExtension(FileUpload3.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload3.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.apple_touch_icon);//xóa ảnh cũ
                        _icon_mobile = "/uploads/images/config/" + Guid.NewGuid() + _ext;
                        FileUpload3.SaveAs(Server.MapPath("~" + _icon_mobile));//lưu ảnh mới
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh icon mobile quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh icon mobile không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }

            if (FileUpload4.HasFile)//_logo
            {
                string _ext = Path.GetExtension(FileUpload4.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload4.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.logo);//xóa ảnh cũ
                        _logo = "/uploads/images/config/" + Guid.NewGuid() + _ext;
                        FileUpload4.SaveAs(Server.MapPath("~" + _logo));//lưu ảnh mới
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh logo quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh logo không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }
            if (FileUpload5.HasFile)//_logo menu top
            {
                string _ext = Path.GetExtension(FileUpload5.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload5.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.logo);//xóa ảnh cũ
                        _logo1 = "/uploads/images/config/" + Guid.NewGuid() + _ext;
                        FileUpload5.SaveAs(Server.MapPath("~" + _logo1));//lưu ảnh mới
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh logo menu top quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh logo menu top không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }
            if (FileUpload6.HasFile)//_logo in hóa đơn
            {
                string _ext = Path.GetExtension(FileUpload6.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload6.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.logo_in_hoadon);//xóa ảnh cũ
                        _logo_in_hoadon = "/uploads/images/config/" + Guid.NewGuid() + _ext;
                        FileUpload6.SaveAs(Server.MapPath("~" + _logo_in_hoadon));//lưu ảnh mới
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh logo in hóa đơn quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh logo in hóa đơn không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }

            if (_checkloi == false)
            {
                cauhinhchung_class.update_thongtin( _favicon, _icon_mobile, _logo, _logo1, txt_tencongty.Text.Trim(),
                    txt_slogan.Text.Trim(), txt_diachi.Text.Trim(), txt_link_googlemap.Text.Trim(), txt_hotline.Text.Trim(),
                    txt_email.Text.Trim(), txt_masothue.Text.Trim(), txt_zalo.Text.Trim(), _logo_in_hoadon);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx");
            }

        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            config_thongtin_table _ob = db.config_thongtin_tables.First();
            file_folder_class.del_file(_ob.icon);//xóa ảnh cũ
            _ob.icon = "";
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
        }
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            config_thongtin_table _ob = db.config_thongtin_tables.First();
            file_folder_class.del_file(_ob.apple_touch_icon);//xóa ảnh cũ
            _ob.apple_touch_icon = "";
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
        }
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            config_thongtin_table _ob = db.config_thongtin_tables.First();
            file_folder_class.del_file(_ob.logo);//xóa ảnh cũ
            _ob.logo = "";
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
        }
    }
    protected void Button5_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            config_thongtin_table _ob = db.config_thongtin_tables.First();
            file_folder_class.del_file(_ob.logo1);//xóa ảnh cũ
            _ob.logo1 = "";
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa  thành công.", "4000", "warning"), true);
        }
    }
    protected void Button6_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            config_thongtin_table _ob = db.config_thongtin_tables.First();
            file_folder_class.del_file(_ob.logo_in_hoadon);//xóa ảnh cũ
            _ob.logo_in_hoadon = "";
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa  thành công.", "4000", "warning"), true);
        }
    }
}