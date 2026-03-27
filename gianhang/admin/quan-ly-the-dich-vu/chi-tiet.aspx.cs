using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    thedichvu_class tdv_cl = new thedichvu_class();
    post_class po_cl = new post_class();
    data_khachhang_class dtkh_cl = new data_khachhang_class();
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class();
    string_class str_cl = new string_class();
    public string user, user_parent, id, sdt_kh;
    public Int64 sotien_conlai = 0;

    #region phân trang
    public int stt = 1, current_page = 1, show = 30, total_page = 1;
    List<string> list_id_split;
    #endregion

    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.id_thedichvu == id && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                            //join ob2 in db.web_post_tables.Where(p => p.phanloai == "ctdv").ToList() on ob1.id_dvsp.ToString() equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            ten_dichvu_sanpham = ob1.ten_dvsp_taithoidiemnay,
                            //gia = ob1.gia_dvsp_taithoidiemnay,
                            //soluong = ob1.soluong,
                            //thanhtien = ob1.thanhtien,
                            //chietkhau = ob1.chietkhau,
                            //tongtien_ck = ob1.tongtien_ck_dvsp,
                            //sauck = ob1.tongsauchietkhau,
                            //hinhanh = ob2.image,
                            danhgia_nguoilam_dichvu = ob1.danhgia_nhanvien_lamdichvu,
                            kyhieu = ob1.kyhieu,
                            danhgia_5sao_dv = ob1.danhgia_5sao_dv,

                            //phantramchot = ob1.phantram_chotsale_dvsp,
                            //tongtien_chot = ob1.tongtien_chotsale_dvsp.Value.ToString() == "0" ? "&nbsp;" : ob1.tongtien_chotsale_dvsp.Value.ToString("#,##0"),

                            phantramlam = ob1.phantram_lamdichvu,
                            tongtien_lam = ob1.tongtien_lamdichvu.Value.ToString() == "0" ? "&nbsp;" : ob1.tongtien_lamdichvu.Value.ToString("#,##0"),
                            tennguoilam = tk_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoilam_dichvu + "'>" + tk_cl.return_object(ob1.nguoilam_dichvu).hoten + "</a></div>",
                            tennguoichot = tk_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoichot_dvsp + "'>" + tk_cl.return_object(ob1.nguoichot_dvsp).hoten + "</a></div>",
                            ngayban = ob1.ngaytao,
                            id_thedichvu = ob1.id_thedichvu,
                        });

        //xử lý từ khóa
        //string _key = txt_search.Text.ToLower();
        //if (_key != "")
        //{
        //    var list_search = list_all.Where(p => p.ten_dichvu_sanpham.ToLower().Contains(_key)).ToList();
        //    list_all = list_all.Intersect(list_search).ToList();
        //}

        //sắp xếp
        list_all = list_all.OrderByDescending(p => p.ngayban).ToList();

        //main
        var list_split = list_all.ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;

        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }

    protected void but_xoathanhtoan_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q12_5") == "" || bcorn_class.check_quyen(user, "n12_5") == "")
        {
            var q_thanhtoan = db.thedichvu_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
            foreach (var t in q_thanhtoan)
            {
                if (Request.Form["check_lichsu_thanhtoan_" + t.id.ToString()] == "on")
                {
                    var q = db.thedichvu_lichsu_thanhtoan_tables.Where(p => p.id == t.id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        Int64 _sotien_thanhtoan = q.First().sotienthanhtoan.Value;
                        string _hinhthuc_thanhtoan = q.First().hinhthuc_thanhtoan;

                        var q_hoadon = db.thedichvu_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        thedichvu_table _ob = q_hoadon.First();
                        _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan - _sotien_thanhtoan;
                        _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;
                        db.SubmitChanges();

                        //xóa lịch sử
                        thedichvu_lichsu_thanhtoan_table _ob1 = q.First();
                        HoaDonThuChiSync_cl.DeleteForTheDichVuPayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                        db.thedichvu_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
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
        if (bcorn_class.check_quyen(user, "q12_5") == "" || bcorn_class.check_quyen(user, "n12_5") == "")
        {
            if (sotien_conlai == 0)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Đơn này đã thanh toán đủ.", "4000", "warning"), true);
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
                    var q = db.thedichvu_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    thedichvu_table _ob = q.First();
                    _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan + _st;
                    _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;

                    db.SubmitChanges();

                    var q1 = db.thedichvu_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    thedichvu_lichsu_thanhtoan_table _ob1 = new thedichvu_lichsu_thanhtoan_table();
                    _ob1.id_hoadon = id;
                    _ob1.sotienthanhtoan = _st;
                    _ob1.thoigian = DateTime.Parse(txt_ngaythanhtoan.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                    _ob1.user_parent = user_parent;
                    _ob1.hinhthuc_thanhtoan = ddl_hinhthuc_thanhtoan.SelectedValue.ToString();
                    _ob1.nguoithanhtoan = user;

                    _ob1.id_chinhanh = Session["chinhanh"].ToString();
                    db.thedichvu_lichsu_thanhtoan_tables.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                    HoaDonThuChiSync_cl.UpsertFromTheDichVuPayment(db, _ob, _ob1, user_parent);
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
        var q_thanhtoan = db.thedichvu_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
        Repeater2.DataSource = q_thanhtoan;
        Repeater2.DataBind();

        thedichvu_table _ob = db.thedichvu_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        sotien_conlai = _ob.sotien_conlai.Value;
        if (sotien_conlai == 0)
        {
            //var q_chitiet = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == id);
            //if (q_chitiet.Count() != 0)
            Label1.Text = "<span class='fg-red'><b>Đã thanh toán.</b></span>";
            //else
            //Label1.Text = "";
        }
        else
            Label1.Text = "<span class='fg-red'><b>Chưa thanh toán: " + sotien_conlai.ToString("#,##0") + "</b></span>";

        if (!IsPostBack)
            txt_sotien_thanhtoan_congno.Text = sotien_conlai.ToString("#,##0");
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
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (bcorn_class.check_quyen(user, "q12_3") == "" || bcorn_class.check_quyen(user, "n12_3") == "")
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                id = Request.QueryString["id"].ToString().Trim();
                if (tdv_cl.exist_id(id, user_parent))
                {
                    var q = db.thedichvu_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    thedichvu_table _ob = q.First();

                    if (bcorn_class.check_quyen(user, "q12_3") == "")
                    {
                        var list_dv = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                       select new { id = ob1.id, tendichvu = ob1.name, }
                                  );
                        DropDownList1.DataSource = list_dv;
                        DropDownList1.DataTextField = "tendichvu";
                        DropDownList1.DataValueField = "id";
                        DropDownList1.DataBind();
                        DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                        DropDownList1.SelectedIndex = po_cl.return_index_dv(_ob.iddv);
                    }
                    else//ngành của mình
                    {
                        if (_ob.id_nganh != Session["nganh"].ToString())
                        {
                            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để chỉnh sửa dữ liệu của ngành khác.", "false", "false", "OK", "alert", "");
                            Response.Redirect("/gianhang/admin");
                        }
                        var list_dv = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_nganh == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                       select new { id = ob1.id, tendichvu = ob1.name, }
                                  );
                        DropDownList1.DataSource = list_dv;
                        DropDownList1.DataTextField = "tendichvu";
                        DropDownList1.DataValueField = "id";
                        DropDownList1.DataBind();
                        DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                        DropDownList1.SelectedIndex = po_cl.return_index_dv_nganh(_ob.iddv);
                    }

                    if (!IsPostBack)
                    {
                        txt_ngaythanhtoan.Text = DateTime.Now.ToString();

                        txt_ngaytao.Text = _ob.ngaytao.Value.ToShortDateString();
                        txt_tenkhachhang.Text = _ob.tenkh;
                        txt_sdt.Text = _ob.sdt; sdt_kh = _ob.sdt;
                        txt_diachi.Text = _ob.diachi;
                        txt_ghichu.Text = _ob.ghichu;
                        txt_tenthe.Text = _ob.tenthe;
                        txt_sobuoi.Text = _ob.tongsoluong.ToString();
                        txt_giatrithe.Text = _ob.tongtien.Value.ToString("#,##0");
                        txt_hsd.Text = _ob.hsd.Value.ToShortDateString();
                        txt_mathe.Text = id;

                        ck_phantram_giathe.Checked = true;
                        if (_ob.chietkhau == 0)
                        {
                            if (_ob.tongtien_ck_hoadon != 0)
                            {
                                ck_tienmat_giathe.Checked = true;
                                txt_chietkhau_giatrithe.Text = _ob.tongtien_ck_hoadon.Value.ToString("#,##0");
                            }
                            else
                                ck_phantram_giathe.Checked = true;
                        }
                        else
                        {
                            ck_phantram_giathe.Checked = true;
                            txt_chietkhau_giatrithe.Text = _ob.chietkhau.ToString();
                        }

                        ck_phantram_chotsale.Checked = true;
                        if (_ob.phantram_chotsale == 0)
                        {
                            if (_ob.tongtien_chotsale_dvsp != 0)
                            {
                                ck_tienmat_chotsale.Checked = true;
                                txt_chietkhau_chotsale.Text = _ob.tongtien_chotsale_dvsp.Value.ToString("#,##0");
                            }
                            else
                                ck_phantram_chotsale.Checked = true;
                        }
                        else
                        {
                            ck_phantram_chotsale.Checked = true;
                            txt_chietkhau_chotsale.Text = _ob.phantram_chotsale.ToString();
                        }

                        var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                             select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                                 );

                        ddl_nhanvien_chotsale.DataSource = list_nhanvien;
                        ddl_nhanvien_chotsale.DataTextField = "tennhanvien";
                        ddl_nhanvien_chotsale.DataValueField = "username";
                        ddl_nhanvien_chotsale.DataBind();
                        ddl_nhanvien_chotsale.Items.Insert(0, new ListItem("Chọn", ""));
                        if (_ob.nguoichotsale != "")
                            ddl_nhanvien_chotsale.SelectedIndex = tk_cl.return_index(_ob.nguoichotsale);



                        main();//list sử dụng thẻ dv
                    }
                    reload_thanhtoan();
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


    public void reset_ss()
    {
        Session["index_sapxep_thedichvu"] = null;
        Session["current_page_thedichvu"] = null;
        Session["search_thedichvu"] = null;
        Session["show_thedichvu"] = null;
        Session["tungay_thedichvu"] = null;
        Session["denngay_thedichvu"] = null;

        Session["index_loc_hsd_thedichvu"] = null;
        Session["index_loc_nganh_thedichvu"] = null;
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q12_3") == "" || bcorn_class.check_quyen(user, "n12_3") == "")
        {
            string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
            string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
            string _ghichu = txt_ghichu.Text;
            string _diachi = txt_diachi.Text;
            string _ngaytao = txt_ngaytao.Text;
            string _tenthe = txt_tenthe.Text;
            string _hsd = txt_hsd.Text;

            string _userchot = ddl_nhanvien_chotsale.SelectedValue.ToString();
            string _iddv = DropDownList1.SelectedValue.ToString();

            string _ck = txt_chietkhau_chotsale.Text.Trim().Replace(".", "");//ck chốt sale
            int _r2 = 0;
            int.TryParse(_ck, out _r2); if (_r2 < 0) _r2 = 0;

            string _ck1 = txt_chietkhau_giatrithe.Text.Trim().Replace(".", "");//ck giá thẻ
            int _r4 = 0;
            int.TryParse(_ck1, out _r4); if (_r4 < 0) _r4 = 0;

            string _sb = txt_sobuoi.Text.Trim().Replace(".", "");//tong so luong, so buoi
            int _r3 = 0;
            int.TryParse(_sb, out _r3); if (_r3 < 0) _r3 = 0;

            string _tongtien = txt_giatrithe.Text.Trim().Replace(".", "");//gia tri the (tong tien)
            int _r5 = 0;
            int.TryParse(_tongtien, out _r5); if (_r5 < 0) _r5 = 0;

            if (dt_cl.check_date(_ngaytao) == false)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày tạo không hợp lệ", "4000", "warning"), true);
            else
            {
                if (_tenkhachhang == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên khách hàng.", "4000", "warning"), true);
                else
                {
                    if (_sdt == "")
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng SĐT khách hàng.", "4000", "warning"), true);
                    else
                    {
                        if (_tenthe == "")
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên thẻ dịch vụ.", "4000", "warning"), true);
                        else
                        {
                            if (_iddv == "")
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng chọn dịch vụ cho thẻ.", "4000", "warning"), true);
                            else
                            {
                                if (_r3 <= 0)
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số buổi không hợp lệ.", "4000", "warning"), true);
                                else
                                {
                                    if ((_r2 < 0 && ck_phantram_giathe.Checked == true) || (_r2 > 100 && ck_phantram_giathe.Checked == true))//nếu chọn % thì k đc <0 & >100 
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu giá thẻ không hợp lệ.", "4000", "warning"), true);
                                    else
                                    {
                                        if ((_r2 < 0 && ck_phantram_chotsale.Checked == true && _userchot != "") || (_r2 > 100 && ck_phantram_chotsale.Checked == true && _userchot != ""))//nếu chọn % thì k đc <0 & >100 
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu chốt sale không hợp lệ.", "4000", "warning"), true);
                                        else
                                        {
                                            thedichvu_table _ob = db.thedichvu_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
                                            _ob.ngaytao = DateTime.Parse(_ngaytao + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                                            _ob.nguoitao = Session["user"].ToString();
                                            _ob.tenthe = _tenthe;
                                            _ob.tenkh = _tenkhachhang;
                                            _ob.iddv = _iddv;
                                            _ob.ten_taithoidiemnay = "";
                                            if (po_cl.exist_id(_iddv))
                                                _ob.ten_taithoidiemnay = po_cl.return_object(_iddv).name;
                                            _ob.hsd = DateTime.Parse(_hsd);
                                            _ob.tongsoluong = _r3;
                                            _ob.sl_dalam = 0;
                                            _ob.sl_conlai = _r3;
                                            _ob.ghichu = _ghichu;
                                            _ob.sdt = _sdt;
                                            _ob.diachi = _diachi;

                                            _ob.tongtien = _r5;
                                            if (ck_phantram_giathe.Checked == true)//nếu ck chốt sale là %
                                            {
                                                _ob.chietkhau = _r4;
                                                _ob.tongsauchietkhau = _ob.tongtien - (_ob.tongtien * _r4 / 100);
                                                _ob.tongtien_ck_hoadon = _ob.tongtien - _ob.tongsauchietkhau;
                                            }
                                            else
                                            {
                                                _ob.chietkhau = 0;
                                                _ob.tongsauchietkhau = _ob.tongtien - _r4;
                                                _ob.tongtien_ck_hoadon = _r4;
                                            }

                                            _ob.sotien_dathanhtoan = 0;
                                            _ob.sotien_conlai = _ob.tongsauchietkhau;

                                            _ob.tennguoichot_hientai = "";
                                            _ob.nguoichotsale = _userchot;
                                            if (_userchot == "")
                                            {
                                                _ob.phantram_chotsale = 0;
                                                _ob.tongtien_chotsale_dvsp = 0;
                                            }
                                            else
                                            {
                                                _ob.tennguoichot_hientai = tk_cl.return_object(_userchot).hoten;
                                                if (ck_phantram_chotsale.Checked == true)//nếu ck chốt sale là %
                                                {
                                                    _ob.phantram_chotsale = _r2;
                                                    _ob.tongtien_chotsale_dvsp = _ob.tongsauchietkhau * _r2 / 100;
                                                }
                                                else
                                                {
                                                    _ob.phantram_chotsale = 0;
                                                    _ob.tongtien_chotsale_dvsp = _r2;
                                                }
                                            }

                                            db.SubmitChanges();

                                            var q_data = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString());
                                            if (q_data.Count() == 0 && _sdt != "")//chưa có sdt thì thêm vào
                                            {
                                                bspa_data_khachhang_table _ob1 = new bspa_data_khachhang_table();
                                                _ob1.tenkhachhang = _tenkhachhang;
                                                _ob1.diachi = _diachi;
                                                _ob1.magioithieu = "";
                                                _ob1.ngaytao = DateTime.Now;
                                                _ob1.nguoitao = user;
                                                _ob1.nguoichamsoc = "";
                                                _ob1.sdt = _sdt;
                                                _ob1.user_parent = user_parent;
                                                _ob1.matkhau = encode_class.encode_md5(encode_class.encode_sha1("12345678"));
                                                //_ob1.nhomkhachhang = DropDownList1.SelectedValue.ToString();

                                                //if (txt_ngaysinh.Text != "" && dt_cl.check_date(txt_ngaysinh.Text) == true)
                                                //    _ob1.ngaysinh = DateTime.Parse(txt_ngaysinh.Text);

                                                _ob.id_chinhanh = Session["chinhanh"].ToString();
                                                _ob1.capbac = ""; _ob1.vnd_tu_e_aha = 0; _ob1.sodiem_e_aha = 0; _ob1.solan_lencap = 0;
                                                db.bspa_data_khachhang_tables.InsertOnSubmit(_ob1);
                                                db.SubmitChanges();
                                            }
                                            reset_ss();
                                            //main();
                                            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm sản phẩm thành công.", "4000", "warning"), true);
                                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thẻ dịch vụ thành công.", "3000", "warning");
                                            Response.Redirect("/gianhang/admin/quan-ly-the-dich-vu/Default.aspx");
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    //autocomplete sđt
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.bspa_data_khachhang_tables.Where(p => p.sdt.Contains(prefixText) && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.sdt).ToList();
    }
    protected void txt_sdt_TextChanged(object sender, EventArgs e)
    {
        string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
        if (_sdt != "")
        {
            var q1 = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao);
            if (q1.Count() != 0)
            {
                txt_tenkhachhang.Text = q1.First().tenkhachhang;
                txt_diachi.Text = q1.First().diachi;
                //if (q1.First().ngaysinh != null)
                //    txt_ngaysinh.Text = q1.First().ngaysinh.Value.ToShortDateString();
            }

        }
    }
    //autocomplete tên kh
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers1(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.bspa_data_khachhang_tables.Where(p => p.tenkhachhang.Contains(prefixText) && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.tenkhachhang).ToList();
    }
    protected void txt_tenkhachhang_TextChanged(object sender, EventArgs e)
    {
        string _tenkh = txt_tenkhachhang.Text.Trim();
        if (_tenkh != "")
        {
            var q1 = db.bspa_data_khachhang_tables.Where(p => p.tenkhachhang == _tenkh && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao);
            if (q1.Count() != 0)
            {
                txt_sdt.Text = q1.First().sdt;
                txt_diachi.Text = q1.First().diachi;
                //if (q1.First().ngaysinh != null)
                //    txt_ngaysinh.Text = q1.First().ngaysinh.Value.ToShortDateString();
            }

        }
    }



}
