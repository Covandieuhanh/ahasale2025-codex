using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_slider_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    public string notifi = "", url_back;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q5_1");
        if (access == null)
            return;

        url_back = GianHangAdminBridge_cl.ResolvePreferredAdminRedirectUrl(HttpContext.Current, "", "/gianhang/admin");
    }

    protected void but_add_Click(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q5_1");
        if (access == null)
            return;

        if (!Directory.Exists(Server.MapPath("~/uploads/images/slide/")))
            Directory.CreateDirectory(Server.MapPath("~/uploads/images/slide/"));
        bool _checkloi = false;

        string _rank_text = txt_rank.Text.Trim();//lấy thứ tự từ ô text
        int _rank = 0;
        int.TryParse(_rank_text, out _rank);//nếu là số nguyên dương thì gán cho _rank
        if (_rank <= 0)//nếu k phải là số thì trả về 0 hoặc nhập vô số âm thì trả về 0 cho đẹp ^^, chơi số nguyên dương thôi
            _rank = 1;


        web_module_slider_table _ob = new web_module_slider_table();
        if (FileUpload2.HasFile)
        {
            _ob.link = txt_link1.Text.Trim().ToLower();
            _ob.rank = _rank;
            _ob.but_title = txt_title_link1.Text.Trim();
            _ob.img = "";
            string _ext = System.IO.Path.GetExtension(FileUpload2.FileName).ToLower();
            if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
            {
                //byte - kb - mb  ContentLength trra về byte của file
                long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                if (_filesize <= 1) //>1MB
                {
                    string _filename = "/uploads/images/slide/" + Guid.NewGuid() + _ext;
                    FileUpload2.SaveAs(Server.MapPath("~" + _filename));
                    _ob.img = _filename;
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }
            else
            {
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh không đúng định dạng.", "false", "false", "OK", "alert", "");
                _checkloi = true;
            }
            if (_checkloi == false)
            {
                db.web_module_slider_tables.InsertOnSubmit(_ob);
                db.SubmitChanges();
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm ảnh thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-module/slide-anh/add.aspx");
            }
        }
        else
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn ảnh.", "false", "false", "OK", "alert", "");



    }

}
