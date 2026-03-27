using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string notifi, user, user_parent;
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    giangvien_class gv_cl = new giangvien_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class ng_cl = new nganh_class();
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
        if (bcorn_class.check_quyen(user, "q14_2") == "" || bcorn_class.check_quyen(user, "n14_2") == "")
        {
            if (!IsPostBack)
            {
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
                }
                else
                {
                    var list_gv = (from ob1 in db.giangvien_tables.Where(p => p.trangthai == "Đang giảng dạy" && p.chuyenmon== Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                   select new { id = ob1.id, ten = ob1.hoten, }
                                    );
                    DropDownList1.DataSource = list_gv;
                    DropDownList1.DataTextField = "ten";
                    DropDownList1.DataValueField = "id";
                    DropDownList1.DataBind();
                    DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                    DropDownList1.SelectedIndex = gv_cl.return_index_nganh(Session["nganh"].ToString());
                }

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
        if (bcorn_class.check_quyen(user, "q14_2") == "")
        {

        }
        else
        {
            DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
            DropDownList5.Enabled = false;
        }

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
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn ngành", "false", "false", "OK", "warning", "");
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
                            hocvien_table _ob1 = new hocvien_table();

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

                            string _anh_capbang = "";
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

                            #region chỉ dành riêng khi tạo
                            _ob1.nguoitao = Session["user"].ToString();
                            _ob1.ngaytao = DateTime.Now;
                            _ob1.id_chinhanh = Session["chinhanh"].ToString();
                            #endregion

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
                            _ob1.sotien_dathanhtoan = 0;
                            _ob1.sotien_conlai = _r4;
                            if (_checkloi == false)
                            {
                                db.hocvien_tables.InsertOnSubmit(_ob1);
                                db.SubmitChanges();
                                GianHangAdminPersonHub_cl.SyncSourcePhoneState(db, user_parent, "", _sdt, _fullname, user);
                                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm thành viên thành công.", "4000", "warning");
                                Response.Redirect("/gianhang/admin/quan-ly-hoc-vien/Default.aspx");
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
