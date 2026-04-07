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
    public string notifi, id;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q5_1");
        if (access == null)
            return;

        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            var q = db.web_module_slider_tables.Where(p => p.id.ToString() == id);
            if (q.Count() != 0)
            {
                if (!IsPostBack)
                {
                    web_module_slider_table _ob = q.First();
                    Label2.Text = "<img src='" + _ob.img + "' style='max-width: 150px' />";
                    txt_rank.Text = _ob.rank.ToString();
                    txt_title_link1.Text = _ob.but_title;
                    txt_link1.Text = _ob.link;
                }

            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/quan-ly-module/slide-anh/default.aspx");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/quan-ly-module/slide-anh/default.aspx");
        }
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

        var q = db.web_module_slider_tables.Where(p => p.id.ToString() == id);
        if (q.Count() != 0)
        {
            web_module_slider_table _ob = q.First();
            _ob.link = txt_link1.Text.Trim().ToLower();
            _ob.rank = _rank;
            _ob.but_title = txt_title_link1.Text.Trim();

            if (FileUpload2.HasFile)
            {
                string _ext = System.IO.Path.GetExtension(FileUpload2.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        string _filename = "/uploads/images/slide/" + Guid.NewGuid() + _ext;
                        FileUpload2.SaveAs(Server.MapPath("~" + _filename));
                        file_folder_class.del_file(_ob.img);//xóa ảnh cũ
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
            }

            if (_checkloi == false)
            {
                db.SubmitChanges();
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-module/slide-anh/edit.aspx?id=" + id);
            }

        }
    }
}
