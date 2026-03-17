using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string notifi, id,user, user_parent;
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    giangvien_class gv_cl = new giangvien_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class nganh_cl = new nganh_class();

    public void main()
    {
        giangvien_table _ob = db.giangvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        if (!IsPostBack)
        {
            var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                              select new { id = ob1.id, ten = ob1.ten, }
                               );
            DropDownList5.DataSource = list_nganh;
            DropDownList5.DataTextField = "ten";
            DropDownList5.DataValueField = "id";
            DropDownList5.DataBind();
            DropDownList5.Items.Insert(0, new ListItem("Chọn", ""));
            DropDownList5.SelectedIndex = nganh_cl.return_index(_ob.chuyenmon);
            if (bcorn_class.check_quyen(user, "q15_3") == "")//neu la quyen cap chi nhanh
            {
                
            }
            else//neu la quyen cap nganh
            {
                if (_ob.chuyenmon != Session["nganh"].ToString())
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để chỉnh sửa Chuyên gia ngành khác.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }
                DropDownList5.Enabled = false;
            }


            txt_hoten.Text = _ob.hoten;
            txt_ngaysinh.Text = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
            DropDownList1.SelectedIndex = _ob.trangthai == "Đang giảng dạy" ? 0 : 1;
            txt_email.Text = _ob.email;
            txt_dienthoai.Text = _ob.dienthoai;
            txt_zalo.Text = _ob.zalo;
            txt_facebook.Text = _ob.facebook;

            txt_sobuoi_lythuyet.Text = _ob.sobuoi_lythuyet.Value.ToString();
            txt_sobuoi_thuchanh.Text = _ob.sobuoi_thuchanh.Value.ToString();
            txt_sobuoi_trogiang.Text = _ob.sobuoi_trogiang.Value.ToString();
            txt_danhgia.Text = _ob.danhgiagiangvien;
            if (_ob.goidaotao == "Cơ bản")
                DropDownList2.SelectedIndex = 0;
            else
            {
                if (_ob.goidaotao == "Nâng cao")
                    DropDownList2.SelectedIndex = 1;
                else
                    DropDownList2.SelectedIndex = 2;
            }
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
        user_parent = "admin";
        if (bcorn_class.check_quyen(user, "q15_3") == "" || bcorn_class.check_quyen(user, "n15_3") == "")
        {
                if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    id = Request.QueryString["id"].ToString().Trim();
                    if (gv_cl.exist_id(id))
                    {
                        main();
                    }
                    else
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion

        
    }

    protected void button1_Click1(object sender, EventArgs e)
    {
        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _ngaysinh = txt_ngaysinh.Text;
        string _email = txt_email.Text.ToLower().Trim();
        string _sdt = txt_dienthoai.Text.Trim();
        string _zalo = txt_zalo.Text.Trim();
        string _facebook = txt_facebook.Text.ToLower().Trim();

        string _nganhhoc = DropDownList5.SelectedValue.ToString();

        string _sobuoi_lt = txt_sobuoi_lythuyet.Text.Trim().Replace(".", "").Replace(",", ""); int _r1 = 0; int.TryParse(_sobuoi_lt, out _r1); if (_r1 < 0) _r1 = 0;
        string _sobuoi_th = txt_sobuoi_thuchanh.Text.Trim().Replace(".", "").Replace(",", ""); int _r2 = 0; int.TryParse(_sobuoi_th, out _r2); if (_r2 < 0) _r2 = 0;
        string _sobuoi_tg = txt_sobuoi_trogiang.Text.Trim().Replace(".", "").Replace(",", ""); int _r3 = 0; int.TryParse(_sobuoi_tg, out _r3); if (_r3 < 0) _r3 = 0;

        if (_fullname == "")
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "warning", "");
        else
        {
            if (_nganhhoc == "")
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn ngành học", "false", "false", "OK", "warning", "");
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
                        giangvien_table _ob1 = db.giangvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();

                        bool _checkloi = false;
                        string _avt = _ob1.anhdaidien;

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


                        _ob1.hoten = _fullname;
                        _ob1.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                        _ob1.anhdaidien = _avt;
                        _ob1.trangthai = DropDownList1.SelectedValue.ToString();
                        _ob1.email = _email;
                        _ob1.dienthoai = _sdt;
                        _ob1.zalo = _zalo;
                        _ob1.facebook = _facebook;
                        if (_ngaysinh != "")
                            _ob1.ngaysinh = DateTime.Parse(_ngaysinh);
                        else
                            _ob1.ngaysinh = null;
                        _ob1.chuyenmon = _nganhhoc;
                        _ob1.sobuoi_lythuyet = _r1; _ob1.sobuoi_thuchanh = _r2; _ob1.sobuoi_trogiang = _r2;
                        _ob1.danhgiagiangvien = txt_danhgia.Text.Trim();
                        _ob1.goidaotao = DropDownList2.SelectedValue.ToString();

                        if (_checkloi == false)
                        {
                            db.SubmitChanges();
                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                            Response.Redirect("/gianhang/admin/quan-ly-giang-vien/Default.aspx");
                        }

                    }
                }
            }
        }
    }
    protected void Button2_Click(object sender, EventArgs e)//xóa ảnh đại diện
    {
        giangvien_table _ob = db.giangvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        if (_ob.anhdaidien != "/uploads/images/macdinh.jpg")
        {
            file_folder_class.del_file(_ob.anhdaidien);//xóa ảnh cũ
            _ob.anhdaidien = "/uploads/images/macdinh.jpg";
        }
        db.SubmitChanges();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh đại diện thành công.", "4000", "warning"), true);
    }
}