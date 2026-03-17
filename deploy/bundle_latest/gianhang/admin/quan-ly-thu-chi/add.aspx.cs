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
    thuchi_class tc_cl = new thuchi_class(); nganh_class ng_cl = new nganh_class();
    public string user, user_parent, notifi;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

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
        if (bcorn_class.check_quyen(user, "q9_2") == "" || bcorn_class.check_quyen(user, "n9_2") == "")
        {
            if (!IsPostBack)
            {
                var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                  select new { id = ob1.id, ten = ob1.ten, }
                                      );
                DropDownList3.DataSource = list_nganh;//add
                DropDownList3.DataTextField = "ten";
                DropDownList3.DataValueField = "id";
                DropDownList3.DataBind();
                DropDownList3.Items.Insert(0, new ListItem("Tất cả", ""));

                DropDownList1.DataSource = list_nganh;//add nhóm thu chi
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataValueField = "id";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));

                if (bcorn_class.check_quyen(user, "q9_2") == "")
                {
                    var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                         select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                          );
                    ddl_nhanvien_nhantien.DataSource = list_nhanvien;
                    ddl_nhanvien_nhantien.DataTextField = "tennhanvien";
                    ddl_nhanvien_nhantien.DataValueField = "username";
                    ddl_nhanvien_nhantien.DataBind();
                    ddl_nhanvien_nhantien.Items.Insert(0, new ListItem("Chọn nhân viên", ""));
                }
                else
                {
                    var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_nganh == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                         select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                          );
                    ddl_nhanvien_nhantien.DataSource = list_nhanvien;
                    ddl_nhanvien_nhantien.DataTextField = "tennhanvien";
                    ddl_nhanvien_nhantien.DataValueField = "username";
                    ddl_nhanvien_nhantien.DataBind();
                    ddl_nhanvien_nhantien.Items.Insert(0, new ListItem("Chọn nhân viên", ""));

                    DropDownList3.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList3.Enabled = false;
                    DropDownList1.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList1.Enabled = false;
                }

                load_nhomthuchi();
                ddl_loaiphieu.SelectedIndex = 1;

                txt_ngaylap.Text = DateTime.Now.ToString("dd/MM/yyyy");



            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion

    }

    public void load_nhomthuchi()
    {
        if (bcorn_class.check_quyen(user, "q9_2") == "")
        {
            var list_nhomtc = (from ob1 in db.bspa_nhomthuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { id = ob1.id, tennhom = ob1.tennhom, }
                                );
            ddl_nhomtc.DataSource = list_nhomtc.OrderBy(p => p.tennhom);
            ddl_nhomtc.DataTextField = "tennhom";
            ddl_nhomtc.DataValueField = "id";
            ddl_nhomtc.DataBind();
            ddl_nhomtc.Items.Insert(0, new ListItem("Chọn", ""));
        }
        else
        {
            var list_nhomtc = (from ob1 in db.bspa_nhomthuchi_tables.Where(p => p.user_parent == user_parent && p.id_nganh == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { id = ob1.id, tennhom = ob1.tennhom, }
                                );
            ddl_nhomtc.DataSource = list_nhomtc.OrderBy(p => p.tennhom);
            ddl_nhomtc.DataTextField = "tennhom";
            ddl_nhomtc.DataValueField = "id";
            ddl_nhomtc.DataBind();
            ddl_nhomtc.Items.Insert(0, new ListItem("", ""));
        }

    }

    protected void but_form_themthuchi_Click(object sender, EventArgs e)
    {
        string _nganh = DropDownList3.SelectedValue.ToString();
        string _ngaylap = txt_ngaylap.Text;
        string _loaiphieu = ddl_loaiphieu.SelectedValue.ToString();
        string _idnhomtc = ddl_nhomtc.SelectedValue.ToString();
        string _noidung = txt_noidung.Text;
        string _nguoinhantien = ddl_nhanvien_nhantien.SelectedValue.ToString();
        string _sotien = txt_sotien.Text.Trim().Replace(".", "");
        int _r1 = 0;
        int.TryParse(_sotien, out _r1);//nếu là số nguyên thì gán cho _r

        if (_nganh == "")
            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng chọn ngành.", "4000", "warning");
        else
        {
            if (dt_cl.check_date(_ngaylap) == false)
                notifi = thongbao_class.metro_notifi_onload("Thông báo", "Ngày lập không hợp lệ", "4000", "warning");
            else
            {

                if (_r1 <= 0)
                    notifi = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng nhập số tiền.", "4000", "warning");
                else
                {
                    if (_noidung == "")
                        notifi = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng nhập nội dung.", "4000", "warning");
                    else
                    {
                        bspa_thuchi_table _ob = new bspa_thuchi_table();
                        _ob.user_parent = user_parent;
                        _ob.ngay = DateTime.Parse(_ngaylap + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                        _ob.thuchi = _loaiphieu;
                        if (_loaiphieu == "Chi")
                        {
                            _ob.duyet_phieuchi = "Chưa duyệt";//chờ duyệt
                            _ob.chophep_duyetvahuy_phieuchi = true;//cho phép duyệt hoặc hủy, chỉ 1 lần duy nhất

                            //gửi thông báo cho admin
                            thongbao_table _ob1 = new thongbao_table();
                            _ob1.id = Guid.NewGuid();
                            _ob1.daxem = false;//chưa xem
                            _ob1.nguoithongbao = db.chinhanh_tables.Where(p => p.id.ToString() == Session["chinhanh"].ToString()).First().taikhoan_quantri;
                            _ob1.nguoinhan = "admin";
                            _ob1.link = "/gianhang/admin/quan-ly-thu-chi/Default.aspx";
                            _ob1.noidung = tk_cl.return_object(user).hoten + " vừa lập phiếu chi và đang chờ duyệt.";
                            _ob1.thoigian = DateTime.Now;
                            _ob1.id_chinhanh = Session["chinhanh"].ToString();
                            db.thongbao_tables.InsertOnSubmit(_ob1);
                            db.SubmitChanges();
                        }
                        else
                        {
                            _ob.duyet_phieuchi = "Đã duyệt";
                            _ob.chophep_duyetvahuy_phieuchi = false;//cho phép duyệt hoặc hủy, chỉ áp dụng cho phiếu chi, kệ mẹ dòng này
                        }
                        _ob.noidung = _noidung;
                        _ob.sotien = _r1;
                        _ob.id_nhomthuchi = _idnhomtc;
                        _ob.nguoilapphieu = tk_cl.return_object(user).hoten;
                        _ob.nguoinhantien = _nguoinhantien;

                        _ob.id_nganh = _nganh;
                        _ob.id_chinhanh = Session["chinhanh"].ToString();
                        db.bspa_thuchi_tables.InsertOnSubmit(_ob);
                        db.SubmitChanges();


                        //thêm hình ảnh nếu có
                        //lưu fileupload  
                        if (FileUpload1.HasFile && FileUpload1.PostedFile != null)
                        {
                            HttpPostedFile uploadedFile = FileUpload1.PostedFile;
                            string _ext = Path.GetExtension(uploadedFile.FileName).ToLower();
                            if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png")
                            {
                                long _filesize = (uploadedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                                if (_filesize <= 1) //>1MB
                                {
                                    string _filename = "/uploads/images/thu-chi/" + Guid.NewGuid() + _ext;
                                    bspa_hinhanhthuchi_table _ob3 = new bspa_hinhanhthuchi_table();
                                    _ob3.user_parent = user_parent;
                                    _ob3.hinhanh = _filename;
                                    _ob3.id_thuchi = tc_cl.return_maxid(user_parent);

                                    _ob3.id_chinhanh = Session["chinhanh"].ToString();
                                    _ob3.id_nganh = _nganh;
                                    db.bspa_hinhanhthuchi_tables.InsertOnSubmit(_ob3);
                                    db.SubmitChanges();
                                    uploadedFile.SaveAs(Server.MapPath("~" + _filename));
                                }
                            }
                        }
                        //end lưu fileupload            

                        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Tạo thu chi thành công.", "4000", "warning");
                        Response.Redirect("/gianhang/admin/quan-ly-thu-chi/add.aspx");
                    }
                }
            }
        }
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        //chưa lấy đc id ngành, hàm này chưa chạy, đang ẩn
        string _tennhomdv = txt_tennhomdv.Text.Trim();
        string _id_nganh = DropDownList1.SelectedValue.ToString();

        if (_id_nganh == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng chọn ngành.", "4000", "warning"), true);
        else
        {
            if (_tennhomdv == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên nhóm thu chi.", "4000", "warning"), true);
            else
            {
                bspa_nhomthuchi_table _ob = new bspa_nhomthuchi_table();
                _ob.tennhom = _tennhomdv;
                _ob.user_parent = user_parent;

                //_ob.id_nganh = _nganh;

                _ob.id_chinhanh = Session["chinhanh"].ToString();
                db.bspa_nhomthuchi_tables.InsertOnSubmit(_ob);
                db.SubmitChanges();
                txt_tennhomdv.Text = "";
                load_nhomthuchi();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm nhóm thu chi thành công.", "4000", "warning"), true);
                //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm nhóm thu chi thành công.", "2000", "warning");
                //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
            }
        }
    }
}
