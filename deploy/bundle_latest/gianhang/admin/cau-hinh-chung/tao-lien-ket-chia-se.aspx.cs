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
    private config_lienket_chiase_table EnsureLienKet()
    {
        config_lienket_chiase_table ob = db.config_lienket_chiase_tables.FirstOrDefault();
        if (ob != null)
            return ob;

        ob = new config_lienket_chiase_table
        {
            title = "",
            description = "",
            image = ""
        };
        db.config_lienket_chiase_tables.InsertOnSubmit(ob);
        db.SubmitChanges();
        return ob;
    }
    public void main()
    {
        config_lienket_chiase_table _ob = EnsureLienKet();

        if (!IsPostBack)
        {
            txt_title.Text = _ob.title;
            txt_description.Text = _ob.description;
        }

        if (_ob.image != "")
        {
            Button2.Visible = true;
            Label2.Text = "<img src='" + _ob.image + "' style='max-width: 150px' />";
        }
        else
        {
            Button2.Visible = false;
            Label2.Text = "";
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q1_1";
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
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_1") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            bool _checkloi = false;
            string _img = "";

            if (!Directory.Exists(Server.MapPath("~/uploads/images/config/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/images/config/"));

            config_lienket_chiase_table _ob = EnsureLienKet();
            _img = _ob.image;

            if (FileUpload2.HasFile)//nếu có ảnh thu nhỏ đc chọn
            {
                string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.image);//xóa ảnh cũ
                        _img = "/uploads/images/config/" + Guid.NewGuid() + _ext;
                        FileUpload2.SaveAs(Server.MapPath("~" + _img));//lưu ảnh mới
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh thu nhỏ quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh thu nhỏ không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }
            if (_checkloi == false)
            {
                cauhinhchung_class.update_lienket_chiase(txt_title.Text.Trim(), txt_description.Text.Trim(), _img);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/cau-hinh-chung/tao-lien-ket-chia-se.aspx");
            }
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_1") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            config_lienket_chiase_table _ob = EnsureLienKet();
            file_folder_class.del_file(_ob.image);//xóa ảnh cũ
            _ob.image = "";
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thu nhỏ thành công.", "4000", "warning"), true);
        }
    }
}
