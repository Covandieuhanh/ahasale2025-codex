using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string notifi, url_back, user, user_parent;
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    random_class rd_cl = new random_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class ng_cl = new nganh_class();
    public string txt_oldpass = "", txt_pass1 = "", txt_pass2 = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "none";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = ""; if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
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
        #region Check quyen theo nganh
        user = Session["user"].ToString();
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (bcorn_class.check_quyen(user, "q2_5") == "" || bcorn_class.check_quyen(user, "n2_5") == "")
        {
            if (Request.Cookies["save_url_admin_aka_1"] != null)
                url_back = Request.Cookies["save_url_admin_aka_1"].Value;
            else
                url_back = "/gianhang/admin";
            if (!IsPostBack)
            {
                load_nganh();
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion  
    }
    public void load_nganh()
    {
        var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                          select new { id = ob1.id, ten = ob1.ten, }
                               );
        DropDownList5.DataSource = list_nganh;
        DropDownList5.DataTextField = "ten";
        DropDownList5.DataValueField = "id";
        DropDownList5.DataBind();
        DropDownList5.Items.Insert(0, new ListItem("Chọn", ""));
        if (bcorn_class.check_quyen(user, "q2_5") == "")
        {
        }
        else
        {
            DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
            DropDownList5.Enabled = false;
        }

    }
    protected void button1_Click1(object sender, EventArgs e)
    {
        string _user = txt_taikhoan.Text.Trim().ToLower();
        string _pass = txt_matkhau.Text.Trim();
        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _ngaysinh = txt_ngaysinh.Text;
        string _email = txt_email.Text.ToLower().Trim();
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


        if (_user == "")
            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng nhập tài khoản.", "4000", "warning");
        else
        {
            if (tk_cl.exist_user(_user))
                notifi = thongbao_class.metro_notifi_onload("Thông báo", "Tài khoản đã tồn tại. Vui lòng chọn tên khác.", "4000", "warning");
            else
            {
                if (!regex_class.check_user_invalid(_user) || _user.Length < 5 || _user.Length > 30)
                    notifi = thongbao_class.metro_notifi_onload("Thông báo", "Tài khoản không hợp lệ. Tài khoản hợp lệ phải có độ dài từ 5-30 ký tự và chỉ chứa các chữ cái từ a-z hoặc số từ 0-9.", "10000", "warning");
                else
                {
                    if (!tk_cl.check_name_invalid(_user))//thỏa mãn a-x 0-9 nhưng trúng mấy cái tên mình k cho phép, vd như admin
                        notifi = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng chọn tên tài khoản khác.", "4000", "warning");
                    else
                    {
                        if (_pass == "")
                            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng nhập mật khẩu.", "4000", "warning");
                        else
                        {
                            if (_fullname == "")
                                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "warning", "");
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
                                        if (_email != "" && tk_cl.exist_email(_email))
                                            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Email này đã được đăng ký cho một tài khoản khác", "false", "false", "OK", "alert", "");
                                        else
                                        {
                                            if (_hsd_tk != "" && dt_cl.check_date(_hsd_tk) == false)//nếu có nhập mới kiểm tra
                                                notifi = thongbao_class.metro_notifi_onload("Thông báo", "Hạn sử dụng tài khoản không hợp lệ", "4000", "warning");
                                            else
                                            {
                                                if (_r1 < 0)//lương k đc < 0
                                                    notifi = thongbao_class.metro_notifi_onload("Thông báo", "Lương cơ bản không hợp lệ.", "4000", "warning");
                                                else
                                                {
                                                    if (_r2 < 0 || _r2 > 31)
                                                        notifi = thongbao_class.metro_notifi_onload("Thông báo", "Số ngày công không hợp lệ.", "4000", "warning");
                                                    else
                                                    {
                                                        taikhoan_table_2023 _ob1 = new taikhoan_table_2023();

                                                        bool _checkloi = false;
                                                        string _avt = "/uploads/images/macdinh.jpg";

                                                        if (FileUpload2.HasFile)//nếu có ảnh thu nhỏ đc chọn
                                                        {
                                                            string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                                                            if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                                                            {
                                                                //byte - kb - mb  ContentLength trra về byte của file
                                                                long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                                                                if (_filesize <= 1) //>1MB
                                                                {
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
                                                        #region chỉ dành riêng khi tạo
                                                        _ob1.taikhoan = _user;
                                                        _ob1.permission = "";
                                                        _ob1.matkhau = encode_class.encode_md5(encode_class.encode_sha1(_pass));
                                                        _ob1.makhoiphuc = rd_cl.random_number(6);
                                                        _ob1.hsd_makhoiphuc = DateTime.Parse("01/01/1991");
                                                        _ob1.nguoitao = Session["user"].ToString();
                                                        _ob1.ngaytao = DateTime.Now;
                                                        #endregion
                                                        _ob1.id_chinhanh = Session["chinhanh"].ToString();
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

                                                        //new aka
                                                        _ob1.user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

                                                        if (_checkloi == false)
                                                        {
                                                            db.taikhoan_table_2023s.InsertOnSubmit(_ob1);
                                                            db.SubmitChanges();
                                                            GianHangAdminPersonHub_cl.SyncSourcePhoneState(db, user_parent, "", _sdt, _fullname, user);
                                                            GianHangAdminWorkspace_cl.SyncLegacySourceAccess(db, user_parent, _user, DropDownList1.SelectedValue.ToString() == "Đang hoạt động");
                                                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã tạo hồ sơ nội bộ thành công. Tiếp theo, hãy phân quyền và liên kết tài khoản Home cho hồ sơ này nhé.", "4000", "warning");
                                                            Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + _user);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
    protected void but_form_themnhomthuchi_Click(object sender, EventArgs e)
    {
        string _tennhom = txt_tennhom.Text.Trim();

        if (_tennhom == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên ngành.", "4000", "warning"), true);
        else
        {

            nganh_table _ob = new nganh_table();
            _ob.ten = _tennhom;
            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.nganh_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_tennhom.Text = "";
            load_nganh();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm ngành thành công.", "4000", "warning"), true);
            //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm nhóm khách hàng thành công.", "2000", "warning");
            //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
        }
    }
}
