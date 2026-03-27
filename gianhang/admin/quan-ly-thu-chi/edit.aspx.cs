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
    thuchi_class tc_cl = new thuchi_class();
    nhomthuchi_class ntc_cl = new nhomthuchi_class(); nganh_class ng_cl = new nganh_class();
    public string user, user_parent, notifi, id;
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
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (bcorn_class.check_quyen(user, "q9_3") == "" || bcorn_class.check_quyen(user, "n9_3") == "")
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                id = Request.QueryString["id"].ToString().Trim();
                if (tc_cl.exist_id(id, user_parent))
                {
                    bspa_thuchi_table _ob = tc_cl.return_object(id);
                    if (HoaDonThuChiSync_cl.IsAutoSystemEntry(_ob))
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Phiếu thu/chi tự động không cho phép chỉnh sửa.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin/quan-ly-thu-chi/Default.aspx");
                    }

                    if (bcorn_class.check_quyen(user, "q9_3") == "")//neu la quyen cap chi nhanh
                    {

                    }
                    else//neu la quyen cap nganh
                    {
                        if (_ob.id_nganh != Session["nganh"].ToString())
                        {
                            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để chỉnh sửa ngành khác.", "false", "false", "OK", "alert", "");
                            Response.Redirect("/gianhang/admin");
                        }
                    }

                    if (!IsPostBack)
                    {
                        if (bcorn_class.check_quyen(user, "q9_3") == "")
                        {
                            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                                  );
                            ddl_nhanvien_nhantien.DataSource = list_nhanvien;
                            ddl_nhanvien_nhantien.DataTextField = "tennhanvien";
                            ddl_nhanvien_nhantien.DataValueField = "username";
                            ddl_nhanvien_nhantien.DataBind();
                            ddl_nhanvien_nhantien.Items.Insert(0, new ListItem("Chọn nhân viên", ""));
                            if (_ob.nguoinhantien != "")
                                ddl_nhanvien_nhantien.SelectedIndex = tk_cl.return_index(_ob.nguoinhantien);

                            var list_nhomtc = (from ob1 in db.bspa_nhomthuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                               select new { id = ob1.id, tennhom = ob1.tennhom, }
                            );
                            ddl_nhomtc.DataSource = list_nhomtc.OrderBy(p => p.tennhom);
                            ddl_nhomtc.DataTextField = "tennhom";
                            ddl_nhomtc.DataValueField = "id";
                            ddl_nhomtc.DataBind();
                            ddl_nhomtc.Items.Insert(0, new ListItem("", ""));
                            if (_ob.id_nhomthuchi != "")
                                ddl_nhomtc.SelectedIndex = ntc_cl.return_index(_ob.id_nhomthuchi, user_parent);
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
                            if (_ob.nguoinhantien != "")
                                ddl_nhanvien_nhantien.SelectedIndex = tk_cl.return_index_nganh(_ob.nguoinhantien);

                            DropDownList3.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                            DropDownList3.Enabled = false;

                            var list_nhomtc = (from ob1 in db.bspa_nhomthuchi_tables.Where(p => p.user_parent == user_parent && p.id_nganh == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                               select new { id = ob1.id, tennhom = ob1.tennhom, }
                            );
                            ddl_nhomtc.DataSource = list_nhomtc.OrderBy(p => p.tennhom);
                            ddl_nhomtc.DataTextField = "tennhom";
                            ddl_nhomtc.DataValueField = "id";
                            ddl_nhomtc.DataBind();
                            ddl_nhomtc.Items.Insert(0, new ListItem("", ""));
                            if (_ob.id_nhomthuchi != "")
                                ddl_nhomtc.SelectedIndex = ntc_cl.return_index_nganh(_ob.id_nhomthuchi, user_parent);
                        }




                        if (_ob.thuchi == "Thu")
                            ddl_loaiphieu.SelectedIndex = 0;
                        else
                            ddl_loaiphieu.SelectedIndex = 1;

                        txt_ngaylap.Text = _ob.ngay.Value.ToString();
                        txt_noidung.Text = _ob.noidung;
                        txt_sotien.Text = _ob.sotien.Value.ToString("#,##0");

                        var q_hinhanh = db.bspa_hinhanhthuchi_tables.Where(p => p.user_parent == user_parent && p.id_thuchi == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        if (q_hinhanh.Count() != 0)
                        {
                            Repeater2.DataSource = q_hinhanh;
                            Repeater2.DataBind();
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
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion



    }





    protected void but_form_themthuchi_Click(object sender, EventArgs e)
    {
        string _nganh = DropDownList3.SelectedValue.ToString();
        string _ngaylap = txt_ngaylap.Text;
        string _nguoinhantien = ddl_nhanvien_nhantien.SelectedValue.ToString();
        string _loaiphieu = ddl_loaiphieu.SelectedValue.ToString();
        string _idnhomtc = ddl_nhomtc.SelectedValue.ToString();
        string _noidung = txt_noidung.Text;
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
                        var q = db.bspa_thuchi_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        bspa_thuchi_table _ob = q.First();
                        if (_ob.duyet_phieuchi == "Đã duyệt" && _ob.thuchi == "Chi")
                            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Không thể chỉnh sửa phiếu chi đã duyệt.", "4000", "warning");
                        else
                        {

                            _ob.ngay = DateTime.Parse(_ngaylap + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                            _ob.thuchi = _loaiphieu;
                            _ob.noidung = _noidung;
                            _ob.sotien = _r1;
                            _ob.id_nhomthuchi = _idnhomtc;
                            _ob.nguoinhantien = _nguoinhantien;
                            _ob.id_nganh = _nganh;
                            db.SubmitChanges();


                            //thêm hình ảnh nếu có
                            //lưu fileupload  
                            if (FileUpload1.HasFile)
                            {
                                foreach (HttpPostedFile uploadedFile in FileUpload1.PostedFiles)
                                {
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
                                            _ob3.id_thuchi = id;

                                            _ob3.id_chinhanh = Session["chinhanh"].ToString();
                                            db.bspa_hinhanhthuchi_tables.InsertOnSubmit(_ob3);
                                            db.SubmitChanges();
                                            uploadedFile.SaveAs(Server.MapPath("~" + _filename));
                                        }
                                    }
                                }
                            }
                            //end lưu fileupload            

                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                            Response.Redirect("/gianhang/admin/quan-ly-thu-chi/Default.aspx");
                        }
                    }
                }
            }
        }

    }
}
