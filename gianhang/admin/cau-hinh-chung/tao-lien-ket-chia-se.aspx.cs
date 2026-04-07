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
    private bool EnsureActionAccess(string requiredPermission)
    {
        return GianHangAdminPageGuard_cl.EnsureAccess(this, db, requiredPermission) != null;
    }
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
        if (!EnsureActionAccess("q1_1"))
            return;

        main();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!EnsureActionAccess("q1_1"))
            return;
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
        if (!EnsureActionAccess("q1_1"))
            return;
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
