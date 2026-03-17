using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string notifi, id, user, user_parent, sdt_kh;
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    hocvien_class hv_cl = new hocvien_class();
    giangvien_class gv_cl = new giangvien_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class nganh_cl = new nganh_class();
    data_khachhang_class dtkh_cl = new data_khachhang_class();
    public Int64 sotien_conlai = 0;

    public void main()
    {
        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        if (!IsPostBack)
        {
            txt_ngaythanhtoan.Text = DateTime.Now.ToShortDateString();

            if (bcorn_class.check_quyen(user, "q14_2") == "")
            {
                var list_gv = (from ob1 in db.giangvien_tables.Where(p => p.trangthai == "Đang giảng dạy" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { id = ob1.id, ten = ob1.hoten, }
                                );
                DropDownList1.DataSource = list_gv;
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataValueField = "id";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                DropDownList1.SelectedIndex = gv_cl.return_index(_ob.id_giangvien);
            }
            else
            {
                if (_ob.nganhhoc != Session["nganh"].ToString())
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để chỉnh sửa thành viên ngành khác.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }

                var list_gv = (from ob1 in db.giangvien_tables.Where(p => p.trangthai == "Đang giảng dạy" && p.chuyenmon == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                   select new { id = ob1.id, ten = ob1.hoten, }
                                    );
                DropDownList1.DataSource = list_gv;
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataValueField = "id";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                DropDownList1.SelectedIndex = gv_cl.return_index_nganh(_ob.id_giangvien);
            }

            var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                              select new { id = ob1.id, ten = ob1.ten, }
                               );
            DropDownList5.DataSource = list_nganh;
            DropDownList5.DataTextField = "ten";
            DropDownList5.DataValueField = "id";
            DropDownList5.DataBind();
            DropDownList5.Items.Insert(0, new ListItem("Chọn", ""));
            DropDownList5.SelectedIndex = nganh_cl.return_index(_ob.nganhhoc);
            if (bcorn_class.check_quyen(user, "q14_3") == "")
            {

            }
            else
            {
                DropDownList5.Enabled = false;
            }


            txt_hoten.Text = _ob.hoten;
            txt_ngaysinh.Text = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";

            txt_email.Text = _ob.email;
            txt_dienthoai.Text = _ob.dienthoai; sdt_kh = _ob.dienthoai;
            txt_zalo.Text = _ob.zalo;
            txt_facebook.Text = _ob.facebook;
            txt_sobuoi_lythuyet.Text = _ob.sobuoi_lythuyet.Value.ToString();
            txt_sobuoi_thuchanh.Text = _ob.sobuoi_thuchanh.Value.ToString();
            txt_sobuoi_trogiang.Text = _ob.sobuoi_trogiang.Value.ToString();
            if (_ob.goidaotao == "Cơ bản")
                DropDownList2.SelectedIndex = 0;
            else
            {
                if (_ob.goidaotao == "Nâng cao")
                    DropDownList2.SelectedIndex = 1;
                else
                    DropDownList2.SelectedIndex = 2;
            }
            if (_ob.capbang == "Chưa cấp bằng")
                DropDownList4.SelectedIndex = 0;
            else
                DropDownList4.SelectedIndex = 1;
            txt_ngaycapbang.Text = _ob.ngaycapbang != null ? _ob.ngaycapbang.Value.ToString("dd/MM/yyyy") : "";
            if (_ob.xeploai == "0")
                DropDownList3.SelectedIndex = 0;
            else
            {
                if (_ob.goidaotao == "A")
                    DropDownList2.SelectedIndex = 1;
                {
                    if (_ob.goidaotao == "B")
                        DropDownList2.SelectedIndex = 2;
                    else
                        DropDownList2.SelectedIndex = 3;
                }
            }
            //txt_nganhhoc.Text = _ob.nganhhoc;
            txt_hocphi.Text = _ob.hocphi.Value.ToString("#,##0");
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
        if (_ob.anh_capbang != "")
        {
            Button3.Visible = true;
            Label3.Text = "<img src='" + _ob.anh_capbang + "' style='max-width: 100px' />";
        }
        else
        {
            Button3.Visible = false;
            Label3.Text = "";
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
        if (bcorn_class.check_quyen(user, "q14_3") == "" || bcorn_class.check_quyen(user, "n14_3") == "")
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                id = Request.QueryString["id"].ToString().Trim();
                if (hv_cl.exist_id(id))
                {
                    main();
                    reload_thanhtoan();
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

    //autocomplete ngành học
    //[System.Web.Script.Services.ScriptMethod()]
    //[System.Web.Services.WebMethod]
    //public static List<string> SearchCustomers(string prefixText, int count)
    //{
    //    dbDataContext db1 = new dbDataContext();
    //    return db1.giangvien_tables.Where(p => p.chuyenmon.Contains(prefixText)).Select(p => p.chuyenmon).ToList();
    //}

    protected void button1_Click1(object sender, EventArgs e)
    {
        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _ngaysinh = txt_ngaysinh.Text;
        string _ngaycapbang = txt_ngaycapbang.Text;
        string _email = txt_email.Text.ToLower().Trim();
        string _sdt = txt_dienthoai.Text.Trim();
        string _zalo = txt_zalo.Text.Trim();
        string _facebook = txt_facebook.Text.ToLower().Trim();
        //string _nganhhoc = txt_nganhhoc.Text.Trim();
        string _nganhhoc = DropDownList5.SelectedValue.ToString();
        string _idgiangvien = DropDownList1.SelectedValue.ToString();

        string _sobuoi_lt = txt_sobuoi_lythuyet.Text.Trim().Replace(".", "").Replace(",", ""); int _r1 = 0; int.TryParse(_sobuoi_lt, out _r1); if (_r1 < 0) _r1 = 0;
        string _sobuoi_th = txt_sobuoi_thuchanh.Text.Trim().Replace(".", "").Replace(",", ""); int _r2 = 0; int.TryParse(_sobuoi_th, out _r2); if (_r2 < 0) _r2 = 0;
        string _sobuoi_tg = txt_sobuoi_trogiang.Text.Trim().Replace(".", "").Replace(",", ""); int _r3 = 0; int.TryParse(_sobuoi_tg, out _r3); if (_r3 < 0) _r3 = 0;
        string _hocphi = txt_hocphi.Text.Trim().Replace(".", "").Replace(",", ""); int _r4 = 0; int.TryParse(_hocphi, out _r4); if (_r4 < 0) _r4 = 0;

        if (_fullname == "")
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "warning", "");
        else
        {
            if (_nganhhoc == "")
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn ngành học", "false", "false", "OK", "warning", "");
            else
            {
                if (_idgiangvien == "")
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn Chuyên gia.", "false", "false", "OK", "warning", "");
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
                            hocvien_table _ob1 = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();

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

                            string _anh_capbang = _ob1.anh_capbang;
                            if (FileUpload3.HasFile)//nếu có ảnh thu nhỏ đc chọn
                            {
                                string _ext = Path.GetExtension(FileUpload3.FileName).ToLower();
                                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                                {
                                    //byte - kb - mb  ContentLength trra về byte của file
                                    long _filesize = (FileUpload3.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                                    if (_filesize <= 1) //>1MB
                                    {
                                        _anh_capbang = "/uploads/images/cap-bang/" + Guid.NewGuid() + _ext;
                                        FileUpload3.SaveAs(Server.MapPath("~" + _anh_capbang));//lưu ảnh mới
                                    }
                                    else
                                    {
                                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh cấp bằng quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                                        _checkloi = true;
                                    }
                                }
                                else
                                {
                                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh cấp bằng không đúng định dạng.", "false", "false", "OK", "alert", "");
                                    _checkloi = true;
                                }
                            }

                            _ob1.hoten = _fullname;
                            _ob1.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                            _ob1.anhdaidien = _avt;
                            _ob1.anh_capbang = _anh_capbang;
                            if (_ngaysinh != "")
                                _ob1.ngaysinh = DateTime.Parse(_ngaysinh);
                            else
                                _ob1.ngaysinh = null;

                            _ob1.email = _email;
                            _ob1.dienthoai = _sdt;
                            _ob1.zalo = _zalo;
                            _ob1.facebook = _facebook;

                            _ob1.sobuoi_lythuyet = _r1; _ob1.sobuoi_thuchanh = _r2; _ob1.sobuoi_trogiang = _r2;
                            _ob1.goidaotao = DropDownList2.SelectedValue.ToString();

                            _ob1.nganhhoc = _nganhhoc;
                            _ob1.id_giangvien = _idgiangvien;
                            _ob1.tengiangvien_hientai = gv_cl.return_object(_ob1.id_giangvien).hoten;
                            _ob1.xeploai = DropDownList3.SelectedValue.ToString();
                            _ob1.capbang = DropDownList4.SelectedValue.ToString();
                            if (_ngaycapbang != "")
                                _ob1.ngaycapbang = DateTime.Parse(_ngaycapbang);
                            else
                                _ob1.ngaycapbang = null;

                            _ob1.hocphi = _r4;

                            var q_tt = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString());
                            if (q_tt.Count() != 0)
                            {
                                _ob1.sotien_dathanhtoan = q_tt.Sum(p => p.sotienthanhtoan);
                                _ob1.sotien_conlai = _ob1.hocphi - _ob1.sotien_dathanhtoan;
                            }
                            else
                            {
                                _ob1.sotien_conlai = _ob1.hocphi - _ob1.sotien_dathanhtoan;
                            }

                            if (_checkloi == false)
                            {
                                db.SubmitChanges();
                                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                                Response.Redirect("/gianhang/admin/quan-ly-hoc-vien/Default.aspx");
                            }

                        }
                    }
                }
            }
        }
    }

    protected void Button2_Click(object sender, EventArgs e)//xóa ảnh đại diện
    {
        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        if (_ob.anhdaidien != "/uploads/images/macdinh.jpg")
        {
            file_folder_class.del_file(_ob.anhdaidien);//xóa ảnh cũ
            _ob.anhdaidien = "/uploads/images/macdinh.jpg";
        }
        db.SubmitChanges();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh đại diện thành công.", "4000", "warning"), true);
    }
    protected void Button3_Click(object sender, EventArgs e)//xóa ảnh câp bằng
    {
        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        file_folder_class.del_file(_ob.anh_capbang);//xóa ảnh cũ
        _ob.anh_capbang = "";
        db.SubmitChanges();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh cấp bằng thành công.", "4000", "warning"), true);
    }


    protected void but_xoathanhtoan_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q14_5") == "" || bcorn_class.check_quyen(user, "q14_5") == "")
        {
            var q_thanhtoan = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
            foreach (var t in q_thanhtoan)
            {
                if (Request.Form["check_lichsu_thanhtoan_" + t.id.ToString()] == "on")
                {
                    var q = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id == t.id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        Int64 _sotien_thanhtoan = q.First().sotienthanhtoan.Value;
                        string _hinhthuc_thanhtoan = q.First().hinhthuc_thanhtoan;

                        var q_hoadon = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        hocvien_table _ob = q_hoadon.First();
                        _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan - _sotien_thanhtoan;
                        _ob.sotien_conlai = _ob.hocphi - _ob.sotien_dathanhtoan;
                        db.SubmitChanges();

                        //xóa lịch sử
                        hocvien_lichsu_thanhtoan_table _ob1 = q.First();
                        HoaDonThuChiSync_cl.DeleteForHocVienPayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                        db.hocvien_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
                        db.SubmitChanges();
                    }
                }
            }

            reload_thanhtoan();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_thanhtoan_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q14_5") == "" || bcorn_class.check_quyen(user, "n14_5") == "")
        {
            if (sotien_conlai == 0)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "thành viên này đã thanh toán đủ.", "4000", "warning"), true);
            else
            {
                string _sotien = txt_sotien_thanhtoan_congno.Text.Trim().Replace(".", "");
                Int64 _st = 0;
                Int64.TryParse(_sotien, out _st);//nếu là số nguyên thì gán cho _st
                if (_st < 0)//nếu k phải là số hoặc nhập vô số âm thì trả về 0
                    _st = 0;
                if (_st > 0)
                {
                    //if (_st > sotien_conlai)
                    //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số tiền thanh toán không được lớn hơn số tiền còn thiếu.", "4000", "warning"), true);
                    //else
                    //{
                    var q = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    hocvien_table _ob = q.First();
                    _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan + _st;
                    _ob.sotien_conlai = _ob.hocphi - _ob.sotien_dathanhtoan;
                    db.SubmitChanges();

                    var q1 = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    hocvien_lichsu_thanhtoan_table _ob1 = new hocvien_lichsu_thanhtoan_table();
                    _ob1.id_hocvien = id;
                    _ob1.sotienthanhtoan = _st;
                    _ob1.thoigian = DateTime.Parse(txt_ngaythanhtoan.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                    _ob1.hinhthuc_thanhtoan = ddl_hinhthuc_thanhtoan.SelectedValue.ToString();
                    _ob1.nguoithanhtoan = user;
                    db.hocvien_lichsu_thanhtoan_tables.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                    HoaDonThuChiSync_cl.UpsertFromHocVienPayment(db, _ob, _ob1, user_parent);
                    db.SubmitChanges();

                    reload_thanhtoan();

                    txt_sotien_thanhtoan_congno.Text = _ob.sotien_conlai.Value.ToString("#,##0");
                    dtkh_cl.tinhtong_chitieu_update_capbac(sdt_kh);//cập nhật điểm eha

                    //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thanh toán thành công.", "4000", "warning");
                    //Response.Redirect("/gianhang/admin/quan-ly-the-dich-vu/Default.aspx");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thanh toán thành công.", "4000", "warning"), true);
                    //}
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số tiền thanh toán không hợp lệ", "4000", "warning"), true);

            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    public void reload_thanhtoan()
    {
        //form thanh toán
        var q_thanhtoan = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
        Repeater2.DataSource = q_thanhtoan;
        Repeater2.DataBind();

        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        sotien_conlai = _ob.sotien_conlai.Value;
        if (sotien_conlai == 0)
        {
            //var q_chitiet = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == id);
            //if (q_chitiet.Count() != 0)
            Label1.Text = "<span class='fg-red'><b>Đã thanh toán.</b></span>";
            //else
            // Label1.Text = "";
        }
        else
            Label1.Text = "<span class='fg-red'><b>Chưa thanh toán: " + sotien_conlai.ToString("#,##0") + "</b></span>";

        if (!IsPostBack)
            txt_sotien_thanhtoan_congno.Text = sotien_conlai.ToString("#,##0");
    }
}