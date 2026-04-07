using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class gianhang_taikhoan_edit : System.Web.UI.Page
{
    public string notifi, user, user_parent, url_back, current_user;
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    datetime_class dt_cl = new datetime_class();
    chinhanh_class cn_cl = new chinhanh_class();
    nganh_class ng_cl = new nganh_class();
    public string txt_oldpass = "", txt_pass1 = "", txt_pass2 = ""; nganh_class nganh_cl = new nganh_class();

    private bool HasAnyPermission(params string[] permissionKeys)
    {
        string actor = (current_user ?? "").Trim();
        if (string.IsNullOrEmpty(actor))
            return false;

        foreach (string permissionKey in permissionKeys)
        {
            if (!string.IsNullOrEmpty(permissionKey) && bcorn_class.check_quyen(actor, permissionKey) == "")
                return true;
        }

        return false;
    }

    private bool IsCurrentRootAdmin()
    {
        return string.Equals((current_user ?? "").Trim(), "admin", StringComparison.OrdinalIgnoreCase);
    }

    private void RedirectToAdminHome()
    {
        Response.Redirect(GianHangAdminBridge_cl.BuildAdminHomeUrl(HttpContext.Current));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        string qsUser = (Request.QueryString["user"] ?? "").Trim();
        user = qsUser;
        current_user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission("q2_3", "n2_3") || string.Equals(user, current_user, StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(qsUser))
            {
                user = qsUser;
                if (tk_cl.exist_user(user))
                {
                    if (user == "admin" && !IsCurrentRootAdmin())
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể chỉnh sửa tài khoản admin.", "false", "false", "OK", "alert", "");
                        RedirectToAdminHome();
                    }
                    else
                    {

                        main();
                        url_back = GianHangAdminBridge_cl.ResolvePreferredAdminRedirectUrl(HttpContext.Current, "", "/gianhang/admin");

                    }
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    RedirectToAdminHome();
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                RedirectToAdminHome();
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
        }
        #endregion


    }
    //protected void Page_InitComplete(object sender, EventArgs e)
    //{

    //}
    public void main()
    {
        string chinhanhId = (Session["chinhanh"] ?? "").ToString();
        taikhoan_table_2023 _ob = db.taikhoan_table_2023s
            .FirstOrDefault(p => p.taikhoan == user && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId));
        if (_ob == null)
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Không tìm thấy tài khoản cần chỉnh sửa.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
            return;
        }
        if (!IsPostBack)
        {
            var list_nganh = (from ob1 in db.nganh_tables.Where(p => string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId).ToList()
                              select new { id = ob1.id, ten = ob1.ten, }
                                  );
            DropDownList5.DataSource = list_nganh;
            DropDownList5.DataTextField = "ten";
            DropDownList5.DataValueField = "id";
            DropDownList5.DataBind();
            DropDownList5.Items.Insert(0, new ListItem("Chọn", ""));
            DropDownList5.SelectedIndex = nganh_cl.return_index(_ob.id_nganh);

            if (HasAnyPermission("q3_5"))
            {
            }
            else
            {
                DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                DropDownList5.Enabled = false;
            }

            txt_hoten.Text = _ob.hoten;
            txt_ngaysinh.Text = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
            DropDownList1.SelectedIndex = _ob.trangthai == "Đang hoạt động" ? 0 : 1;
            txt_email.Text = _ob.email; txt_dienthoai.Text = _ob.dienthoai; txt_zalo.Text = _ob.zalo; txt_facebook.Text = _ob.facebook;
            txt_hansudung_taikhoan.Text = _ob.hansudung != null ? _ob.hansudung.Value.ToString("dd/MM/yyyy") : "";
            txt_luong.Text = (_ob.luongcoban ?? 0).ToString("#,##0");
            txt_songaycong.Text = (_ob.songaycong ?? 0).ToString();
        }
        if (_ob.anhdaidien != "")
        {
            Button2.Visible = true;
            Label2.Text = "<img src='" + _ob.anhdaidien + "' style='max-width: 100px' />";
        }
        else
        {
            Button2.Visible = false;
            Label2.Text = "";
        }
    }
    protected void button1_Click1(object sender, EventArgs e)
    {
        if (!(HasAnyPermission("q2_3", "n2_3") || string.Equals(user, current_user, StringComparison.OrdinalIgnoreCase)))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
            return;
        }

        taikhoan_table_2023 _ob = tk_cl.return_object(user);
        if (_ob == null)
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Không tìm thấy tài khoản cần cập nhật.", "false", "false", "OK", "alert", "");
            return;
        }
        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _ngaysinh = txt_ngaysinh.Text;
        string _email = txt_email.Text.ToLower().Trim();
        string _email_old = _ob.email;
        string _sdt = txt_dienthoai.Text.Trim();
        string _zalo = txt_zalo.Text.Trim();
        string _facebook = txt_facebook.Text.ToLower().Trim();
        string _hsd_tk = txt_hansudung_taikhoan.Text;

        string _idnganh = DropDownList5.SelectedValue.ToString();

        string _luong = txt_luong.Text.Trim().Replace(".", "").Replace(",", "");
        int _r1 = 0;
        int.TryParse(_luong, out _r1);//nếu là số nguyên thì gán cho _r

        string _ngaycong = txt_songaycong.Text.Trim();
        int _r2 = 0;
        int.TryParse(_ngaycong, out _r2);//nếu là số nguyên thì gán cho _r


        if (_fullname == "")
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "false", "false", "OK", "warning", "");
        else
        {
            if (_ngaysinh != "" && dt_cl.check_date(_ngaysinh) == false)//nếu có nhập mới kiểm tra
                notifi = thongbao_class.metro_notifi_onload("Thông báo", "Ngày sinh không hợp lệ.", "4000", "warning");
            else
            {
                if (!regex_class.check_email_invalid(_email) && _email != "")//nếu có nhập mail thì kiểm tra định dạng mail
                    notifi = thongbao_class.metro_notifi_onload("Thông báo", "Email không hợp lệ.", "4000", "warning");
                else
                {
                    if (_email != "" && tk_cl.exist_email_old(_email_old, _email) && _email != _email_old)
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Email này đã được đăng ký cho một tài khoản khác", "false", "false", "OK", "alert", "");
                    else
                    {
                        if (_hsd_tk != "" && dt_cl.check_date(_hsd_tk) == false)//nếu có nhập mới kiểm tra
                            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Hạn sử dụng tài khoản không hợp lệ", "4000", "warning");
                        else
                        {
                            if (_r1 < 0)//lương k đc < 0
                                notifi = thongbao_class.metro_notifi_onload("Thông báo", "Lương cơ bản không hợp lệ.", "2000", "warning");
                            else
                            {
                                if (_r2 < 0 || _r2 > 31)
                                    notifi = thongbao_class.metro_notifi_onload("Thông báo", "Số ngày công không hợp lệ.", "4000", "warning");
                                else
                                {
                                    string chinhanhId = (Session["chinhanh"] ?? "").ToString();
                                    taikhoan_table_2023 _ob1 = db.taikhoan_table_2023s
                                        .FirstOrDefault(p => p.taikhoan == user && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId));
                                    if (_ob1 == null)
                                    {
                                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Không tìm thấy tài khoản cần cập nhật.", "false", "false", "OK", "alert", "");
                                        return;
                                    }

                                    string _oldPhone = (_ob1.dienthoai ?? "").Trim();
                                    bool _checkloi = false;
                                    string _avt = "";
                                    _avt = _ob1.anhdaidien;

                                    if (FileUpload2.HasFile)//nếu có ảnh thu nhỏ đc chọn
                                    {
                                        string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                                        if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                                        {
                                            //byte - kb - mb  ContentLength trra về byte của file
                                            long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                                            if (_filesize <= 1) //>1MB
                                            {
                                                if (_ob1.anhdaidien != "/uploads/images/macdinh.jpg")
                                                    file_folder_class.del_file(_ob1.anhdaidien);//xóa ảnh cũ
                                                _avt = "/uploads/images/avatar/" + Guid.NewGuid() + _ext;
                                                FileUpload2.SaveAs(Server.MapPath("~" + _avt));//lưu ảnh mới
                                            }
                                            else
                                            {
                                                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh đại diện quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                                                _checkloi = true;
                                            }
                                        }
                                        else
                                        {
                                            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh đại diện không đúng định dạng.", "false", "false", "OK", "alert", "");
                                            _checkloi = true;
                                        }
                                    }
                                    _ob1.id_nganh = _idnganh;
                                    _ob1.hoten = _fullname;
                                    _ob1.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                                    _ob1.anhdaidien = _avt;
                                    _ob1.trangthai = DropDownList1.SelectedValue.ToString();
                                    _ob1.email = _email; _ob1.dienthoai = _sdt; _ob1.zalo = _zalo; _ob1.facebook = _facebook;
                                    if (_ngaysinh != "")
                                        _ob1.ngaysinh = DateTime.Parse(_ngaysinh);
                                    else
                                        _ob1.ngaysinh = null;
                                    if (_hsd_tk != "")
                                        _ob1.hansudung = DateTime.Parse(_hsd_tk);
                                    else
                                        _ob1.hansudung = null;

                                    _ob1.luongcoban = _r1;
                                    _ob1.songaycong = _r2;
                                    if (_r1 != 0 && _r2 != 0)
                                        _ob1.luongngay = _r1 / _r2;
                                    else
                                        _ob1.luongngay = 0;

                                    if (_checkloi == false)
                                    {
                                        db.SubmitChanges();
                                        GianHangAdminPersonHub_cl.SyncSourcePhoneState(db, user_parent, _oldPhone, _sdt, _fullname, user);
                                        GianHangAdminWorkspace_cl.SyncLegacySourceAccess(db, user_parent, user, DropDownList1.SelectedValue.ToString() == "Đang hoạt động");
                                        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                                        Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + user);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
    protected void Button2_Click(object sender, EventArgs e)//xóa ảnh đại diện
    {
        if (HasAnyPermission("q2_3", "n2_3"))
        {
            taikhoan_table_2023 _ob = db.taikhoan_table_2023s.Where(p => p.taikhoan == user && p.id_chinhanh == Session["chinhanh"].ToString()).First();
            if (_ob.anhdaidien != "/uploads/images/macdinh.jpg")
            {
                file_folder_class.del_file(_ob.anhdaidien);//xóa ảnh cũ
                _ob.anhdaidien = "/uploads/images/macdinh.jpg";
            }
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh đại diện thành công.", "4000", "warning"), true);

        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
        }
    }
}
