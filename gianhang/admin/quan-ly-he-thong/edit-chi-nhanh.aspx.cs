using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class();
    chinhanh_class cn_cl = new chinhanh_class();
    nhomthuchi_class ntc_cl = new nhomthuchi_class(); nganh_class ng_cl = new nganh_class();
    public string user, user_parent, notifi, id;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

    private bool HasPermission(string permissionKey)
    {
        string currentUser = (user ?? "").Trim();
        if (string.IsNullOrEmpty(currentUser) || string.IsNullOrEmpty(permissionKey))
            return false;

        return bcorn_class.check_quyen(currentUser, permissionKey) != "2";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q16_0");
        if (access == null)
            return;

        #region Check id
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;

        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            if (cn_cl.exist_id(id))
            {
                chinhanh_table _ob = cn_cl.return_object(id);

                if (!IsPostBack)
                {
                    txt_tenchinhanh.Text = _ob.ten;
                    TextBox1.Text = _ob.slogan;
                    TextBox2.Text = _ob.diachi;
                    TextBox3.Text = _ob.email;
                    TextBox4.Text = _ob.sdt;

                    if (_ob.logo_hoadon !=null)
                    {
                        Button2.Visible = true;
                        Label2.Text = "<img src='" + _ob.logo_hoadon + "' style='max-width: 150px' />";
                    }
                    else
                    {
                        Button2.Visible = false;
                        Label2.Text = "";
                    }
                    if (_ob.logo_footer != null)
                    {
                        Button1.Visible = true;
                        Label1.Text = "<img src='" + _ob.logo_footer + "' style='max-width: 150px' />";
                    }
                    else
                    {
                        Button1.Visible = false;
                        Label1.Text = "";
                    }
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/Default.aspx");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/Default.aspx");
        }

        #endregion
    }





    protected void but_form_themthuchi_Click(object sender, EventArgs e)
    {
        if (!HasPermission("q16_0"))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
            return;
        }

        string _ten_cn = txt_tenchinhanh.Text;
        string _slogan = TextBox1.Text;
        string _diachi = TextBox2.Text;
        string _email = TextBox3.Text;
        string _sdt = TextBox4.Text;

        if (_ten_cn == "")
            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng nhập tên Chi nhánh.", "4000", "warning");
        else
        {
            var q = db.chinhanh_tables.Where(p => p.id.ToString() == id);
            chinhanh_table _ob = q.First();
            _ob.ten = _ten_cn;
            _ob.slogan = _slogan; _ob.diachi = _diachi; _ob.email = _email; _ob.sdt = _sdt;
            bool _checkloi = false;
            string _img1 = "", _img2 = "";

            if (FileUpload1.HasFile)//logo chân trang
            {
                string _ext = Path.GetExtension(FileUpload1.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload1.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.logo_footer);//xóa ảnh cũ
                        _img1 = "/uploads/images/" + Guid.NewGuid() + _ext;
                        FileUpload1.SaveAs(Server.MapPath("~" + _img1));//lưu ảnh mới
                        _ob.logo_footer = _img1;
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước Logo chân trang quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Logo chân trang không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }
            if (FileUpload2.HasFile)//logo hóa đơn
            {
                string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        file_folder_class.del_file(_ob.logo_hoadon);//xóa ảnh cũ
                        _img2 = "/uploads/images/" + Guid.NewGuid() + _ext;
                        FileUpload2.SaveAs(Server.MapPath("~" + _img2));//lưu ảnh mới
                        _ob.logo_hoadon = _img2;
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước Logo hóa đơn quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Logo hóa đơn không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }
            if (_checkloi == false)
            {
                db.SubmitChanges();
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-he-thong/chi-nhanh.aspx");
            }

        }

    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        if (!HasPermission("q16_0"))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
            return;
        }
    
        var q = db.chinhanh_tables.Where(p => p.id.ToString() == id);
        chinhanh_table _ob = q.First();
        file_folder_class.del_file(_ob.logo_hoadon);//xóa ảnh cũ
        _ob.logo_hoadon = "";
        db.SubmitChanges();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa logo hóa đơn thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-he-thong/edit-chi-nhanh.aspx?id="+ id);
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!HasPermission("q16_0"))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
            return;
        }

        var q = db.chinhanh_tables.Where(p => p.id.ToString() == id);
        chinhanh_table _ob = q.First();
        file_folder_class.del_file(_ob.logo_footer);//xóa ảnh cũ
        _ob.logo_footer = "";
        db.SubmitChanges();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa logo chân trang thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-he-thong/edit-chi-nhanh.aspx?id=" + id);
    }
}
