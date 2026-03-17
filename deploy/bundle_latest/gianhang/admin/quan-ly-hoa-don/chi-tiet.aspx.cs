using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    hoadon_class hd_cl = new hoadon_class();
    datetime_class dt_cl = new datetime_class();
    thedichvu_class tdv_cl = new thedichvu_class(); data_khachhang_class dtkh_cl = new data_khachhang_class();
    post_class po_cl = new post_class();
    hoadon_chitiet_class cthd_cl = new hoadon_chitiet_class(); string_class str_cl = new string_class();
    public string user, user_parent, id = "", tongtien, tienbangchu, ngaytao, ten_kh, sdt_kh, diachi_kh, ghichu_kh, ck_hoadon, km1_ghichu, id_guide, show_add = "none";
    public Int64 sotien_conlai = 0, sauck = 0;
    public int stt_tt = 1;
    public bool co_ngu_canh_datlich = false;
    public string id_datlich_lienket = "", ten_dichvu_datlich = "", ngay_datlich_lienket = "", ten_nhanvien_datlich = "", trangthai_datlich_lienket = "";
    public string url_quay_lai_datlich = "", url_ho_so_khach_datlich = "", url_ban_thedv_datlich = "", url_sudung_thedv_datlich = "";
    public string thongbao_datlich_thedv = "";
    public datlich_vanhanh_tongquan tongquan_datlich = new datlich_vanhanh_tongquan();
    public List<string> list_id_thedv_goiy = new List<string>();
    public string id_thedv_tu_dong_goiy = "";

    #region phân trang
    public int stt = 1;
    List<string> list_id_split, list_id_split_thedv;
    #endregion

    public void update_hoadon()
    {
        var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        var q_hoadon_chitiet = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());

        //update tổng tiền
        bspa_hoadon_table _ob_hoadon = q_hoadon.First();
        if (q_hoadon_chitiet.Count() != 0)
        {
            _ob_hoadon.tongtien = q_hoadon_chitiet.Sum(p => p.tongsauchietkhau);

            if (_ob_hoadon.tongtien_ck_hoadon.Value > 0)
            {
                if (_ob_hoadon.chietkhau.Value != 0) //ck theo %
                    _ob_hoadon.tongtien_ck_hoadon = _ob_hoadon.tongtien * _ob_hoadon.chietkhau / 100;
            }

            _ob_hoadon.tongsauchietkhau = _ob_hoadon.tongtien - _ob_hoadon.tongtien_ck_hoadon;
            _ob_hoadon.sotien_conlai = _ob_hoadon.tongsauchietkhau - _ob_hoadon.sotien_dathanhtoan;
        }
        else
        {
            _ob_hoadon.tongtien = 0;
            _ob_hoadon.chietkhau = 0;
            _ob_hoadon.tongtien_ck_hoadon = 0;
            _ob_hoadon.tongsauchietkhau = 0;
            _ob_hoadon.sotien_dathanhtoan = 0;
            _ob_hoadon.thanhtoan_tienmat = 0;
            _ob_hoadon.thanhtoan_chuyenkhoan = 0;
            _ob_hoadon.thanhtoan_quetthe = 0;
            _ob_hoadon.sotien_conlai = 0;
            _ob_hoadon.sl_dichvu = 0;
            _ob_hoadon.ds_dichvu = 0;
            _ob_hoadon.sauck_dichvu = 0;
            _ob_hoadon.sl_sanpham = 0;
            _ob_hoadon.ds_sanpham = 0;
            _ob_hoadon.sauck_sanpham = 0;
        }

        //phân loại đơn sản phẩm hay dịch vụ hay cả 2
        var q_dichvu = q_hoadon_chitiet.Where(p => p.kyhieu == "dichvu");
        var q_sanpham = q_hoadon_chitiet.Where(p => p.kyhieu == "sanpham");
        if (q_dichvu.Count() == 0)
        {
            if (q_sanpham.Count() != 0)
                _ob_hoadon.dichvu_hay_sanpham = "sanpham";
        }
        else
        {
            if (q_sanpham.Count() == 0)
                _ob_hoadon.dichvu_hay_sanpham = "dichvu";
            else
                _ob_hoadon.dichvu_hay_sanpham = "dichvusanpham";
        }

        //update sl dv sp
        if (q_hoadon_chitiet.Where(p => p.kyhieu == "dichvu").Count() != 0)
        {
            _ob_hoadon.sl_dichvu = q_hoadon_chitiet.Where(p => p.kyhieu == "dichvu").Sum(p => p.soluong);
            _ob_hoadon.ds_dichvu = q_hoadon_chitiet.Where(p => p.kyhieu == "dichvu").Sum(p => p.thanhtien);
            _ob_hoadon.sauck_dichvu = q_hoadon_chitiet.Where(p => p.kyhieu == "dichvu").Sum(p => p.tongsauchietkhau);
        }
        else
        {
            _ob_hoadon.sl_dichvu = 0;
            _ob_hoadon.ds_dichvu = 0;
            _ob_hoadon.sauck_dichvu = 0;
        }

        if (q_hoadon_chitiet.Where(p => p.kyhieu == "sanpham").Count() != 0)
        {
            _ob_hoadon.sl_sanpham = q_hoadon_chitiet.Where(p => p.kyhieu == "sanpham").Sum(p => p.soluong);
            _ob_hoadon.ds_sanpham = q_hoadon_chitiet.Where(p => p.kyhieu == "sanpham").Sum(p => p.thanhtien);
            _ob_hoadon.sauck_sanpham = q_hoadon_chitiet.Where(p => p.kyhieu == "sanpham").Sum(p => p.tongsauchietkhau);
        }
        else
        {
            _ob_hoadon.sl_sanpham = 0;
            _ob_hoadon.ds_sanpham = 0;
            _ob_hoadon.sauck_sanpham = 0;
        }
        // db.SubmitChanges();

        tongtien = _ob_hoadon.tongtien.Value.ToString("#,##0");
        //ck_hoadon = return_ck_hoadon(id);
        sauck = _ob_hoadon.tongsauchietkhau.Value;
        if (sauck < 0)
            tienbangchu = "Âm " + number_class.number_to_text_unlimit(sauck.ToString().Replace("-", ""));
        else
            tienbangchu = number_class.number_to_text_unlimit(sauck.ToString().Replace("-", ""));


        db.SubmitChanges();
    }

    public void reload_thanhtoan()
    {
        //form thanh toán
        var q_thanhtoan = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
        Repeater2.DataSource = q_thanhtoan;
        Repeater2.DataBind();

        bspa_hoadon_table _ob = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        sotien_conlai = _ob.sotien_conlai.Value;
        if (sotien_conlai == 0)
        {
            var q_chitiet = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q_chitiet.Count() != 0)
                Label1.Text = "<span class='fg-red'><b>Đã thanh toán.</b></span>";
            else
                Label1.Text = "";
        }
        else
            Label1.Text = "<span class='fg-red'><b>Chưa thanh toán: " + sotien_conlai.ToString("#,##0") + "</b></span>";

        if (!IsPostBack)
            txt_sotien_thanhtoan_congno.Text = sotien_conlai.ToString("#,##0");
    }
    public void reload()
    {
        var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        bspa_hoadon_table _ob = q_hoadon.First();
        tongtien = _ob.tongtien.Value.ToString("#,##0");//hiển thị tổng tiền hóa đơn
        ngaytao = _ob.ngaytao.Value.ToString("dd/MM/yyyy HH:mm");
        ten_kh = _ob.tenkhachhang;
        sdt_kh = _ob.sdt;
        diachi_kh = _ob.diachi;
        ghichu_kh = _ob.ghichu;
        //ck_hoadon = return_ck_hoadon(id);
        sotien_conlai = _ob.sotien_conlai.Value;
        km1_ghichu = _ob.km1_ghichu;
        id_guide = _ob.id_guide.ToString();
        if (!IsPostBack)
        {
            txt_ngaythanhtoan.Text = DateTime.Now.ToString();
            txt_ngayban.Text = DateTime.Now.ToString();
            txt_ngayban_sanpham.Text = DateTime.Now.ToString();
            txt_ngayban_thedv.Text = DateTime.Now.ToString();

            txt_tenkhachhang.Text = _ob.tenkhachhang;
            txt_sdt.Text = _ob.sdt;
            txt_ghichu.Text = _ob.ghichu;
            txt_diachi.Text = _ob.diachi;
            txt_ngaytao.Text = _ob.ngaytao.Value.ToString("dd/MM/yyyy");

            ck_hd_phantram.Checked = true; ck_hd_phantram1.Checked = true;
            if (_ob.chietkhau == 0)
            {
                if (_ob.tongtien_ck_hoadon != 0)
                {
                    ck_hd_tienmat.Checked = true; ck_hd_tienmat1.Checked = true;
                    txt_chietkhau_hoadon.Text = _ob.tongtien_ck_hoadon.Value.ToString("#,##0");
                    txt_chietkhau_hoadon1.Text = _ob.tongtien_ck_hoadon.Value.ToString("#,##0");
                }
                else
                {
                    ck_hd_phantram.Checked = true; ck_hd_phantram1.Checked = true;
                }
            }
            else
            {
                ck_hd_phantram.Checked = true; ck_hd_phantram1.Checked = true;
                txt_chietkhau_hoadon.Text = _ob.chietkhau.ToString();
                txt_chietkhau_hoadon1.Text = _ob.chietkhau.ToString();
            }

            //dịch vụ
            //var list_dichvu = (from ob1 in db.bspa_dichvu_tables.Where(p => p.user_parent == user_parent).ToList()
            //                   select new { id = ob1.id, tendichvu = ob1.tendichvu, }
            //          );
            //ddl_dichvu.DataSource = list_dichvu.OrderBy(p => p.tendichvu);
            //ddl_dichvu.DataTextField = "tendichvu";
            //ddl_dichvu.DataValueField = "id";
            //ddl_dichvu.DataBind();
            //ddl_dichvu.Items.Insert(0, new ListItem("Chọn", ""));

            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.user_parent == user_parent && p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                      );
            ddl_nhanvien_chotsale.DataSource = list_nhanvien;
            ddl_nhanvien_chotsale.DataTextField = "tennhanvien";
            ddl_nhanvien_chotsale.DataValueField = "username";
            ddl_nhanvien_chotsale.DataBind();
            ddl_nhanvien_chotsale.Items.Insert(0, new ListItem("Chọn", ""));

            ddl_nhanvien_lamdichvu.DataSource = list_nhanvien;
            ddl_nhanvien_lamdichvu.DataTextField = "tennhanvien";
            ddl_nhanvien_lamdichvu.DataValueField = "username";
            ddl_nhanvien_lamdichvu.DataBind();
            ddl_nhanvien_lamdichvu.Items.Insert(0, new ListItem("Chọn", ""));

            ddl_nhanvien_lamdichvu_thedv.DataSource = list_nhanvien;
            ddl_nhanvien_lamdichvu_thedv.DataTextField = "tennhanvien";
            ddl_nhanvien_lamdichvu_thedv.DataValueField = "username";
            ddl_nhanvien_lamdichvu_thedv.DataBind();
            ddl_nhanvien_lamdichvu_thedv.Items.Insert(0, new ListItem("Chọn", ""));

            //end dịch vụ

            //sản phẩm
            //var list_sanpham = (from ob1 in db.bspa_sanpham_tables.Where(p => p.user_parent == user_parent).ToList()
            //                    select new { id = ob1.id, tensanpham = ob1.tensanpham, }
            //          );
            //ddl_sanpham.DataSource = list_sanpham.OrderBy(p => p.tensanpham);
            //ddl_sanpham.DataTextField = "tensanpham";
            //ddl_sanpham.DataValueField = "id";
            //ddl_sanpham.DataBind();
            //ddl_sanpham.Items.Insert(0, new ListItem("Chọn", ""));

            ddl_nhanvien_chotsale_sanpham.DataSource = list_nhanvien;
            ddl_nhanvien_chotsale_sanpham.DataTextField = "tennhanvien";
            ddl_nhanvien_chotsale_sanpham.DataValueField = "username";
            ddl_nhanvien_chotsale_sanpham.DataBind();
            ddl_nhanvien_chotsale_sanpham.Items.Insert(0, new ListItem("Chọn", ""));

        }

        //thẻ dịch vụ còn hạn
        var list_thedv = (from ob1 in db.thedichvu_tables.Where(p => p.sdt == sdt_kh && p.hsd.Value.Date >= DateTime.Now.Date && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                              //join ob2 in db.web_menu_tables.ToList() on bv.id_category equals mn.id.ToString()
                          select new
                          {
                              id = ob1.id,
                              tenkhachhang = ob1.tenkh,
                              sdt = ob1.sdt,
                              ngaytao = ob1.ngaytao,
                              tenthe = ob1.tenthe,
                              tendv = ob1.ten_taithoidiemnay,
                              hsd = ob1.hsd,
                              sobuoi = ob1.tongsoluong,
                              sl_dalam = ob1.sl_dalam,
                              sl_conlai = ob1.sl_conlai,
                              tongtien = ob1.tongtien,
                              ck_hoadon = ob1.chietkhau,
                              tongtien_ck = ob1.tongtien_ck_hoadon,
                              tongsauchietkhau = ob1.tongsauchietkhau,
                              sotien_dathanhtoan = ob1.sotien_dathanhtoan,
                              sotien_conlai = ob1.sotien_conlai,
                              phantramchot = ob1.phantram_chotsale,
                              tongtien_chot = ob1.tongtien_chotsale_dvsp,
                              tennguoichot = tk_cl.exist_user_of_userparent(ob1.nguoichotsale, user_parent) == false ? ob1.nguoichotsale : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoichotsale + "'>" + tk_cl.return_object(ob1.nguoichotsale).hoten + "</a></div>",
                          }).ToList();
        var list_split_thedv = list_thedv.ToList();
        list_id_split_thedv = new List<string>();
        foreach (var t in list_split_thedv)
        {
            list_id_split_thedv.Add("check_thedv_" + t.id);
        }
        Repeater3.DataSource = list_thedv;
        Repeater3.DataBind();
    }

    private void nap_ngu_canh_datlich_hoadon()
    {
        string _id_datlich = (Request.QueryString["id_datlich"] ?? "").Trim();
        if (_id_datlich == "")
        {
            var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q_hoadon.Count() != 0)
                _id_datlich = datlich_lienket_class.lay_id_datlich_tu_ghichu(q_hoadon.First().ghichu);
        }

        if (_id_datlich == "")
            return;

        var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id_datlich && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q_lich.Count() == 0)
            return;

        bspa_datlich_table _lich = q_lich.First();
        string _sdt_lich = datlich_class.chuanhoa_sdt(_lich.sdt);
        if (sdt_kh != "" && _sdt_lich != "" && datlich_class.chuanhoa_sdt(sdt_kh) != _sdt_lich)
            return;

        co_ngu_canh_datlich = true;
        id_datlich_lienket = _id_datlich;
        ten_dichvu_datlich = _lich.tendichvu_taithoidiemnay;
        if (ten_dichvu_datlich == "")
            ten_dichvu_datlich = datlich_class.return_ten_dichvu(db, _lich.dichvu, Session["chinhanh"].ToString());

        ngay_datlich_lienket = _lich.ngaydat.HasValue ? _lich.ngaydat.Value.ToString("dd/MM/yyyy HH:mm") : "";
        ten_nhanvien_datlich = datlich_class.return_ten_nguoitao_hienthi(_lich.nhanvien_thuchien);
        trangthai_datlich_lienket = _lich.trangthai ?? "";
        url_quay_lai_datlich = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + HttpUtility.UrlEncode(id_datlich_lienket);
        tongquan_datlich = datlich_vanhanh_class.tai_tongquan(db, _sdt_lich, Session["chinhanh"].ToString(), _lich.dichvu);

        if (tongquan_datlich.co_hoso_khachhang)
        {
            url_ho_so_khach_datlich = "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(tongquan_datlich.id_khachhang);
            url_sudung_thedv_datlich = url_ho_so_khach_datlich + "&act=thedv&id_datlich=" + HttpUtility.UrlEncode(id_datlich_lienket);
        }

        string _ten_url = HttpUtility.UrlEncode(ten_kh);
        string _sdt_url = HttpUtility.UrlEncode(_sdt_lich);
        string _id_nganh_url = HttpUtility.UrlEncode(tongquan_datlich.id_nganh_goiy);
        string _id_dichvu_url = HttpUtility.UrlEncode(_lich.dichvu);
        url_ban_thedv_datlich = "/gianhang/admin/quan-ly-the-dich-vu/Default.aspx?q=add"
            + "&tenkh=" + _ten_url
            + "&sdt=" + _sdt_url
            + "&iddv=" + _id_dichvu_url
            + "&idnganh=" + _id_nganh_url
            + "&id_datlich=" + HttpUtility.UrlEncode(id_datlich_lienket);

        if (!string.IsNullOrWhiteSpace(_lich.dichvu))
        {
            DateTime _today = DateTime.Now.Date;
            list_id_thedv_goiy = db.thedichvu_tables
                .Where(p =>
                    p.sdt == _sdt_lich &&
                    p.id_chinhanh == Session["chinhanh"].ToString() &&
                    p.iddv == _lich.dichvu &&
                    (p.sl_conlai ?? 0) > 0 &&
                    (p.hsd.HasValue == false || p.hsd.Value.Date >= _today))
                .OrderBy(p => p.hsd)
                .ThenByDescending(p => p.ngaytao)
                .Select(p => p.id.ToString())
                .ToList();
        }

        if (list_id_thedv_goiy.Count == 1)
            id_thedv_tu_dong_goiy = list_id_thedv_goiy[0];

        if (list_id_thedv_goiy.Count == 0)
            thongbao_datlich_thedv = "Chưa có thẻ còn hiệu lực khớp với dịch vụ của lịch hẹn này.";
        else if (list_id_thedv_goiy.Count == 1)
            thongbao_datlich_thedv = "Đã tự gợi ý 1 thẻ phù hợp để staff tiêu buổi ngay trong hóa đơn.";
        else
            thongbao_datlich_thedv = "Có " + list_id_thedv_goiy.Count.ToString("#,##0") + " thẻ phù hợp. Staff chọn 1 thẻ rồi thêm vào hóa đơn.";
    }

    private void ap_dung_ngu_canh_datlich_vao_form()
    {
        if (co_ngu_canh_datlich == false)
            return;

        var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == id_datlich_lienket && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q_lich.Count() == 0)
            return;

        bspa_datlich_table _lich = q_lich.First();
        if (_lich.ngaydat.HasValue)
        {
            txt_ngayban.Text = _lich.ngaydat.Value.ToString("dd/MM/yyyy");
            txt_ngayban_thedv.Text = _lich.ngaydat.Value.ToString("dd/MM/yyyy");
        }

        if (!string.IsNullOrWhiteSpace(_lich.nhanvien_thuchien))
        {
            datlich_class.try_select_dropdown_value(ddl_nhanvien_lamdichvu, _lich.nhanvien_thuchien);
            datlich_class.try_select_dropdown_value(ddl_nhanvien_lamdichvu_thedv, _lich.nhanvien_thuchien);
        }

        if (ten_dichvu_datlich != "" && txt_tendichvu.Text.Trim() == "")
        {
            txt_tendichvu.Text = ten_dichvu_datlich;
            if (!string.IsNullOrWhiteSpace(_lich.dichvu))
            {
                var q_dichvu = db.web_post_tables.Where(p => p.id.ToString() == _lich.dichvu && p.phanloai == "ctdv" && p.bin == false && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q_dichvu.Count() != 0)
                {
                    txt_gia.Text = q_dichvu.First().giaban_dichvu.Value.ToString("#,##0");
                    txt_chietkhau_chotsale.Text = q_dichvu.First().phantram_chotsale_dichvu.Value.ToString();
                    txt_chietkhau_lamdichvu.Text = q_dichvu.First().phantram_lamdichvu.Value.ToString();
                }
            }
        }
    }

    public string return_thedv_checked_attr(string _id)
    {
        if (id_thedv_tu_dong_goiy == _id)
            return "checked='checked'";
        return "";
    }

    public string return_thedv_row_class(string _id)
    {
        if (list_id_thedv_goiy.Contains(_id))
            return "bg-lightGreen";
        return "";
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
        if (bcorn_class.check_quyen(user, "q7_1") == "" || bcorn_class.check_quyen(user, "n7_1") == "")
        {

            //chỉnh trạng thái thông báo thành đã xem (nếu đc truy cập từ thanh thông báo)
            if (!string.IsNullOrWhiteSpace(Request.QueryString["idtb"]))
            {
                string _idtb = Request.QueryString["idtb"].ToString().Trim();
                var q_tb = db.thongbao_tables.Where(p => p.id.ToString() == _idtb && p.nguoinhan == Session["user"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q_tb.Count() != 0)
                {
                    thongbao_table _ob = q_tb.First();
                    _ob.daxem = true;//đã đọc
                    db.SubmitChanges();
                }
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                id = Request.QueryString["id"].ToString().Trim();
                if (hd_cl.exist_id(id, user_parent))
                {
                    if (bcorn_class.check_quyen(user, "q7_1") == "")//neu la quyen cap chi nhanh
                    {

                    }
                    else//neu la quyen cap nganh
                    {
                        if (hd_cl.return_object(id).id_nganh != Session["nganh"].ToString())
                        {
                            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để xem ngành khác.", "false", "false", "OK", "alert", "");
                            Response.Redirect("/gianhang/admin");
                        }
                    }

                    reload();
                    update_hoadon();
                    reload_thanhtoan();
                    main();
                    nap_ngu_canh_datlich_hoadon();
                    if (!IsPostBack)
                        ap_dung_ngu_canh_datlich_vao_form();

                    if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]))//khi ng dùng nhấn vào nút tạo hóa đơn từ menutop --> tạo nhanh
                    {
                        string _q = Request.QueryString["q"].ToString().Trim();
                        if (_q == "add")
                            show_add = "block";
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

        //if (!IsPostBack)
        //{
        //    foreach (var t in db.bspa_lichsu_thanhtoan_tables)
        //    {
        //        string _id_hd = t.id_hoadon;
        //        var bc = db.bspa_hoadon_tables.Where(p => p.id.ToString() == _id_hd);
        //        bspa_lichsu_thanhtoan_table _ob = t;
        //        if (bc.Count() != 0)
        //            _ob.id_nganh = bc.First().id_nganh;
        //        db.SubmitChanges();
        //    }
        //}

    }
    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.web_post_tables.Where(p => (p.phanloai == "ctdv" && p.id_chinhanh == Session["chinhanh"].ToString()) || (p.phanloai == "ctsp" && p.id_chinhanh == Session["chinhanh"].ToString())).ToList() on ob1.id_dvsp.ToString() equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            ten_dichvu_sanpham = ob1.ten_dvsp_taithoidiemnay,
                            gia = ob1.gia_dvsp_taithoidiemnay,
                            soluong = ob1.soluong,
                            thanhtien = ob1.thanhtien,
                            chietkhau = ob1.chietkhau,
                            tongtien_ck = ob1.tongtien_ck_dvsp,
                            sauck = ob1.tongsauchietkhau,
                            //hinhanh = ob2.image,
                            danhgia_nguoilam_dichvu = ob1.danhgia_nhanvien_lamdichvu,
                            kyhieu = ob1.kyhieu,
                            danhgia_5sao_dv = ob1.danhgia_5sao_dv,

                            phantramchot = ob1.phantram_chotsale_dvsp,
                            tongtien_chot = ob1.tongtien_chotsale_dvsp.Value.ToString() == "0" ? "&nbsp;" : ob1.tongtien_chotsale_dvsp.Value.ToString("#,##0"),

                            phantramlam = ob1.phantram_lamdichvu,
                            tongtien_lam = ob1.tongtien_lamdichvu.Value.ToString() == "0" ? "&nbsp;" : ob1.tongtien_lamdichvu.Value.ToString("#,##0"),
                            tennguoilam = tk_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoilam_dichvu + "'>" + tk_cl.return_object(ob1.nguoilam_dichvu).hoten + "</a></div>",
                            tennguoichot = tk_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoichot_dvsp + "'>" + tk_cl.return_object(ob1.nguoichot_dvsp).hoten + "</a></div>",
                            ngayban = ob1.ngaytao,
                            id_thedichvu = ob1.id_thedichvu,
                            gia_hienthi_khidung_thedv = ob1.gia_hienthi_khidung_thedv,//để hiển thị giá khi sd thẻ dịch vụ, hiện cho có
                        });

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.ten_dichvu_sanpham.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //sắp xếp
        list_all = list_all.OrderBy(p => p.ngayban).ToList();

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
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_chitiethoadon"] = "1";

        main();

    }
    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_chitiethoadon"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_chitiethoadon"] = int.Parse(Session["current_page_chitiethoadon"].ToString()) - 1;

        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_chitiethoadon"] = int.Parse(Session["current_page_chitiethoadon"].ToString()) + 1;

        main();
    }
    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_4") == "" || bcorn_class.check_quyen(user, "n7_4") == "")
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (cthd_cl.exist_id(_id, user_parent))
                    {
                        bspa_hoadon_chitiet_table _ob = q.First();

                        //update số lượng
                        if (_ob.kyhieu == "sanpham")
                        {
                            //tăng sl trong kho sp lên lại
                            po_cl.tang_soluong_sanpham(_ob.id_dvsp, _ob.soluong.Value);
                        }

                        if (_ob.id_thedichvu != null)//bonbap
                        {
                            //công trừ số buổi của thẻ
                            var q_thedv = db.thedichvu_tables.Where(p => p.id.ToString() == _ob.id_thedichvu && p.id_chinhanh == Session["chinhanh"].ToString());
                            thedichvu_table _ob_thedv = q_thedv.First();
                            _ob_thedv.sl_dalam = _ob_thedv.sl_dalam - 1;
                            _ob_thedv.sl_conlai = _ob_thedv.tongsoluong - _ob_thedv.sl_dalam;
                            db.SubmitChanges();
                        }

                        db.bspa_hoadon_chitiet_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();

                        //đoạn dưới đừng thắc mắc, khi xóa hết chi tiết thì xóa luôn lịch sử thanh toán
                        var q_hoadon_chitiet = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        if (q_hoadon_chitiet.Count() == 0)
                        {
                            var q_thanhtoan = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
                            foreach (var t in q_thanhtoan)
                            {
                                //xóa lịch sử
                                bspa_lichsu_thanhtoan_table _ob1 = t;
                                HoaDonThuChiSync_cl.DeleteForInvoicePayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                                db.bspa_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
                                db.SubmitChanges();
                            }
                        }
                    }
                }
            }
            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning");
            Response.Redirect("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + id);
            //reload();
            //update_hoadon();
            //reload_thanhtoan();
            //main();
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    public string return_ck_dvsp(string _id_cthd)
    {
        string _kq = "";
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == _id_cthd && p.id_chinhanh == Session["chinhanh"].ToString());
        bspa_hoadon_chitiet_table _ob = q.First();

        if (_ob.tongtien_ck_dvsp.Value > 0)
        {
            string _tongtien = _ob.tongtien_ck_dvsp.Value.ToString("#,##0").Replace(".", ",");
            if (_ob.chietkhau.Value != 0)
                _kq = _kq + "<div><span data-role='hint' data-hint-position='top' data-hint-text='" + _tongtien + "'>" + _ob.chietkhau.Value + "%</span> </div>";
            else
                _kq = _kq + "<div><span>" + _ob.tongtien_ck_dvsp.Value.ToString("#,##0") + "</span> </div>";
        }
        else
            _kq = _kq + "<div>" + "&nbsp;" + "</div>";
        return _kq;
    }
    public string return_ck_hoadon(string _id_hd)
    {
        string _kq = "";
        var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == _id_hd && p.id_chinhanh == Session["chinhanh"].ToString());
        bspa_hoadon_table _ob = q.First();

        if (_ob.chietkhau == 0)
        {
            if (_ob.tongtien_ck_hoadon != 0)
            {
                _kq = _kq + "<div>" + _ob.tongtien_ck_hoadon.Value.ToString("#,##0") + "</div>";
                return _kq;
            }
            else
                return "0%";
        }
        else
        {
            _kq = _kq + "<div><span data-role='hint' data-hint-position='top' data-hint-text='" + _ob.tongtien_ck_hoadon.Value.ToString("#,##0") + "'>" + _ob.chietkhau.Value + "%</span> </div>";
            return _kq;
        }
    }
    protected void but_form_themdichvu_Click(object sender, EventArgs e)
    {

        if (bcorn_class.check_quyen(user, "q7_2") == "" || bcorn_class.check_quyen(user, "n7_2") == "")
        {
            //string _id_dichvu = ddl_dichvu.SelectedValue.ToString();
            string _ten_dichvu = txt_tendichvu.Text.Trim();
            string _id_dichvu = po_cl.return_id(_ten_dichvu);

            string _user_chotsale = ddl_nhanvien_chotsale.SelectedValue.ToString();
            string _user_lamdichvu = ddl_nhanvien_lamdichvu.SelectedValue.ToString();

            string _gia = txt_gia.Text.Trim().Replace(".", "");
            int _r1 = 0;
            int.TryParse(_gia, out _r1);//nếu là số nguyên thì gán cho _r

            string _ck = txt_chietkhau.Text.Trim().Replace(".", "");
            int _r2 = 0;
            int.TryParse(_ck, out _r2);//nếu là số nguyên thì gán cho _r

            string _sl = txt_soluong.Text.Trim().Replace(".", "");
            int _r3 = 0;
            int.TryParse(_sl, out _r3);//nếu là số nguyên thì gán cho _r

            string _chietkhau_chotsale = txt_chietkhau_chotsale.Text.Trim().Replace(".", "");
            int _r4 = 0;
            int.TryParse(_chietkhau_chotsale, out _r4);

            string _phantram_lam = txt_chietkhau_lamdichvu.Text.Trim().Replace(".", "");
            int _r5 = 0;
            int.TryParse(_phantram_lam, out _r5);



            var q_chitiet_hoadon = db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_dvsp == _id_dichvu && p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());

            if (po_cl.exist_id(_id_dichvu))
            {
                if (_r1 < 0)//giá k đc = 0            
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Giá dịch vụ không hợp lệ.", "4000", "warning"), true);
                else
                {
                    if (_r3 < 1)//số lượng     
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số lượng không hợp lệ.", "4000", "warning"), true);
                    else
                    {
                        if ((_r2 < 0 && ck_dv_phantram.Checked == true) || (_r2 > 100 && ck_dv_phantram.Checked == true))//nếu chọn % thì k đc <0 & >100 
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu dịch vụ không hợp lệ.", "4000", "warning"), true);
                        else
                        {
                            if ((_r4 < 0 && ck_dv_phantram_chotsale.Checked == true) || (_r4 > 100 && ck_dv_phantram_chotsale.Checked == true))//nếu chọn % thì k đc <0 & >100
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu cho nhân viên chốt sale không hợp lệ.", "4000", "warning"), true);
                            else
                            {
                                if ((_r5 < 0 && ck_dv_phantram_chotsale_lamdv.Checked == true) || (_r5 > 100 && ck_dv_phantram_chotsale_lamdv.Checked == true))//nếu chọn % thì k đc <0 & >100
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu cho nhân viên thực hiện không hợp lệ.", "4000", "warning"), true);
                                else
                                {
                                    if (!tk_cl.exist_user_of_userparent(_user_chotsale, user_parent) && _user_chotsale != "")
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Nhân viên chốt sale không hợp lệ.", "4000", "warning"), true);
                                    else
                                    {
                                        //if (_user_lamdichvu == "")
                                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng chọn nhân viên thực hiện.", "4000", "warning"), true);
                                        //if (q_chitiet_hoadon.Count() != 0)
                                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Dịch vụ này đã có trong hóa đơn. Thay vì thêm mới, hãy chỉnh sửa nó và lưu lại.", "4000", "warning"), true);
                                        //else
                                        //{
                                        //else
                                        //{
                                        bspa_hoadon_chitiet_table _ob = new bspa_hoadon_chitiet_table();
                                        _ob.id_dvsp = _id_dichvu;
                                        _ob.ten_dvsp_taithoidiemnay = _ten_dichvu;
                                        _ob.gia_dvsp_taithoidiemnay = _r1;
                                        _ob.soluong = _r3;
                                        _ob.thanhtien = _r1 * _r3;
                                        _ob.hinhanh_hientai = "";
                                        if (ck_dv_phantram.Checked == true)//nếu ck chốt sale là %
                                        {
                                            _ob.chietkhau = _r2;
                                            _ob.tongsauchietkhau = _ob.thanhtien - (_ob.thanhtien * _r2 / 100);
                                            _ob.tongtien_ck_dvsp = _ob.thanhtien - _ob.tongsauchietkhau;
                                        }
                                        else
                                        {
                                            _ob.chietkhau = 0;
                                            _ob.tongsauchietkhau = _ob.thanhtien - _r2;
                                            _ob.tongtien_ck_dvsp = _r2;
                                        }
                                        _ob.ngaytao = DateTime.Parse(txt_ngayban.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());

                                        _ob.nguoichot_dvsp = _user_chotsale;
                                        _ob.tennguoichot_hientai = "";
                                        if (_user_chotsale == "")
                                        {
                                            _ob.phantram_chotsale_dvsp = 0;
                                            _ob.tongtien_chotsale_dvsp = 0;
                                        }
                                        else
                                        {
                                            _ob.tennguoichot_hientai = tk_cl.return_object(_user_chotsale).hoten;
                                            if (ck_dv_phantram_chotsale.Checked == true)//nếu ck chốt sale là %
                                            {
                                                _ob.phantram_chotsale_dvsp = _r4;
                                                _ob.tongtien_chotsale_dvsp = _ob.tongsauchietkhau * _r4 / 100;
                                            }
                                            else
                                            {
                                                _ob.phantram_chotsale_dvsp = 0;
                                                _ob.tongtien_chotsale_dvsp = _r4;
                                            }
                                        }

                                        _ob.nguoilam_dichvu = _user_lamdichvu;

                                        if (_user_lamdichvu == "")
                                        {
                                            _ob.phantram_lamdichvu = 0;
                                            _ob.tongtien_lamdichvu = 0;
                                            _ob.tennguoilam_hientai = "";
                                        }
                                        else
                                        {
                                            _ob.tennguoilam_hientai = tk_cl.return_object(_user_lamdichvu).hoten;
                                            if (ck_dv_phantram_chotsale_lamdv.Checked == true)//nếu ck thực hiện là %
                                            {
                                                _ob.phantram_lamdichvu = _r5;
                                                _ob.tongtien_lamdichvu = _ob.tongsauchietkhau * _r5 / 100;
                                            }
                                            else
                                            {
                                                _ob.phantram_lamdichvu = 0;
                                                _ob.tongtien_lamdichvu = _r5;
                                            }
                                        }

                                        _ob.id_hoadon = id;
                                        _ob.kyhieu = "dichvu";
                                        _ob.user_parent = user_parent;
                                        _ob.nguoitao = Session["user"].ToString();
                                        _ob.danhgia_nhanvien_lamdichvu = txt_danhgia_dichvu.Text.Trim();
                                        _ob.danhgia_5sao_dv = Request.Form["danhgia_5sao_nhanvien_dv"];

                                        _ob.id_chinhanh = Session["chinhanh"].ToString();
                                        _ob.id_nganh = hd_cl.return_object(id).id_nganh;
                                        db.bspa_hoadon_chitiet_tables.InsertOnSubmit(_ob);
                                        db.SubmitChanges();

                                        if (co_ngu_canh_datlich)
                                        {
                                            var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                                            if (q_hoadon.Count() != 0)
                                                q_hoadon.First().ghichu = datlich_lienket_class.dam_bao_ghi_chu_datlich(q_hoadon.First().ghichu, id_datlich_lienket, "Tạo từ", ten_dichvu_datlich);

                                            datlich_lienket_class.dong_bo_vao_lich_hen(
                                                db,
                                                id_datlich_lienket,
                                                Session["chinhanh"].ToString(),
                                                id,
                                                "",
                                                _user_lamdichvu,
                                                _ob.ngaytao,
                                                "Đã thêm dịch vụ vào hóa đơn #" + id,
                                                true);
                                            db.SubmitChanges();
                                        }

                                        reload();
                                        update_hoadon();
                                        reload_thanhtoan();
                                        main();

                                        //reset
                                        //ddl_dichvu.SelectedIndex = 0;
                                        txt_tendichvu.Text = "";
                                        txt_gia.Text = ""; txt_soluong.Text = "1"; txt_chietkhau.Text = "0"; ddl_nhanvien_chotsale.SelectedIndex = 0; txt_chietkhau_chotsale.Text = "0";
                                        ddl_nhanvien_lamdichvu.SelectedIndex = 0; txt_chietkhau_lamdichvu.Text = "0"; txt_danhgia_dichvu.Text = "";


                                        dtkh_cl.tinhtong_chitieu_update_capbac(sdt_kh);
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm dịch vụ vào hóa đơn thành công.", "4000", "warning"), true);
                                        //}
                                        //}

                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Dịch vụ không tồn tại.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_form_themsanpham_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_2") == "" || bcorn_class.check_quyen(user, "n7_2") == "")
        {

            string _ten_sanpham = txt_tensanpham.Text.Trim();
            string _id_sanpham = po_cl.return_id(_ten_sanpham);

            string _user_chotsale = ddl_nhanvien_chotsale_sanpham.SelectedValue.ToString();


            string _gia = txt_gia_sanpham.Text.Trim().Replace(".", "");
            int _r1 = 0;
            int.TryParse(_gia, out _r1);//nếu là số nguyên thì gán cho _r

            string _ck = txt_chietkhau_sanpham.Text.Trim().Replace(".", "");
            int _r2 = 0;
            int.TryParse(_ck, out _r2);//nếu là số nguyên thì gán cho _r

            string _sl = txt_soluong_sanpham.Text.Trim().Replace(".", "");
            int _r3 = 0;
            int.TryParse(_sl, out _r3);//nếu là số nguyên thì gán cho _r

            string _phantram_chot = txt_chietkhau_chotsale_sanpham.Text.Trim().Replace(".", "");
            int _r4 = 0;
            int.TryParse(_phantram_chot, out _r4);


            var q_chitiet_hoadon = db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_dvsp == _id_sanpham && p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());

            if (po_cl.exist_id(_id_sanpham))
            {
                if (_r1 < 0)//giá k đc = 0            
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Giá sản phẩm không hợp lệ.", "4000", "warning"), true);
                else
                {
                    if (_r3 < 1)//số lượng     
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số lượng không hợp lệ.", "4000", "warning"), true);
                    else
                    {
                        if ((_r2 < 0 && ck_sp_phantram.Checked == true) || (_r2 > 100 && ck_sp_phantram.Checked == true))//nếu chọn % thì k đc <0 & >100 
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu sản phẩm không hợp lệ.", "4000", "warning"), true);
                        else
                        {
                            if ((_r4 < 0 && ck_sp_phantram_chotsale.Checked == true) || (_r4 > 100 && ck_sp_phantram_chotsale.Checked == true))//nếu chọn % thì k đc <0 & >100
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu cho nhân viên chốt sale không hợp lệ.", "4000", "warning"), true);
                            else
                            {
                                if (!tk_cl.exist_user_of_userparent(_user_chotsale, user_parent) && _user_chotsale != "")
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Nhân viên chốt sale không hợp lệ.", "4000", "warning"), true);
                                else
                                {
                                    //if (q_chitiet_hoadon.Count() != 0)
                                    //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Sản phẩm này đã có trong hóa đơn. Thay vì thêm mới, hãy chỉnh sửa nó và lưu lại.", "4000", "warning"), true);
                                    //else
                                    //{
                                    if (!po_cl.check_sl_ton_sanpham(_id_sanpham, _r3))//nếu k đủ hàng để xuất
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng trong kho không đủ để xuất", "false", "false", "OK", "alert", ""), true);
                                    else
                                    {
                                        bspa_hoadon_chitiet_table _ob = new bspa_hoadon_chitiet_table();
                                        _ob.id_dvsp = _id_sanpham;
                                        _ob.ten_dvsp_taithoidiemnay = _ten_sanpham;
                                        _ob.gia_dvsp_taithoidiemnay = _r1;
                                        _ob.soluong = _r3;
                                        _ob.thanhtien = _r1 * _r3;
                                        _ob.hinhanh_hientai = "";
                                        if (ck_sp_phantram.Checked == true)//nếu ck  là %
                                        {
                                            _ob.chietkhau = _r2;
                                            _ob.tongsauchietkhau = _ob.thanhtien - (_ob.thanhtien * _r2 / 100);
                                            _ob.tongtien_ck_dvsp = _ob.thanhtien - _ob.tongsauchietkhau;
                                        }
                                        else
                                        {
                                            _ob.chietkhau = 0;
                                            _ob.tongsauchietkhau = _ob.thanhtien - _r2;
                                            _ob.tongtien_ck_dvsp = _r2;
                                        }
                                        _ob.ngaytao = DateTime.Parse(txt_ngayban_sanpham.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());


                                        _ob.nguoichot_dvsp = _user_chotsale;

                                        if (_user_chotsale == "")
                                        {
                                            _ob.phantram_chotsale_dvsp = 0;
                                            _ob.tongtien_chotsale_dvsp = 0;
                                        }
                                        else
                                        {
                                            _ob.tennguoichot_hientai = tk_cl.return_object(_user_chotsale).hoten;
                                            if (ck_sp_phantram_chotsale.Checked == true)//nếu ck chốt sale là %
                                            {
                                                _ob.phantram_chotsale_dvsp = _r4;
                                                _ob.tongtien_chotsale_dvsp = _ob.tongsauchietkhau * _r4 / 100;
                                            }
                                            else
                                            {
                                                _ob.phantram_chotsale_dvsp = 0;
                                                _ob.tongtien_chotsale_dvsp = _r4;
                                            }
                                        }

                                        _ob.nguoilam_dichvu = "";
                                        _ob.phantram_lamdichvu = 0;
                                        _ob.tongtien_lamdichvu = 0;

                                        _ob.id_hoadon = id;
                                        _ob.kyhieu = "sanpham";
                                        _ob.user_parent = user_parent;
                                        _ob.nguoitao = Session["user"].ToString();
                                        _ob.danhgia_nhanvien_lamdichvu = "";


                                        _ob.id_chinhanh = Session["chinhanh"].ToString();
                                        _ob.id_nganh = hd_cl.return_object(id).id_nganh;
                                        db.bspa_hoadon_chitiet_tables.InsertOnSubmit(_ob);
                                        db.SubmitChanges();
                                        //giảm số lượng trong kho
                                        po_cl.giam_soluong_sanpham(_id_sanpham, _r3);

                                        reload();
                                        update_hoadon();
                                        reload_thanhtoan();
                                        main();

                                        //reset
                                        txt_tensanpham.Text = "";
                                        txt_gia_sanpham.Text = ""; txt_soluong_sanpham.Text = "1"; txt_chietkhau_sanpham.Text = "0"; ddl_nhanvien_chotsale_sanpham.SelectedIndex = 0; txt_chietkhau_chotsale_sanpham.Text = "0";


                                        //main();
                                        dtkh_cl.tinhtong_chitieu_update_capbac(sdt_kh);
                                        //hd_cl.tinhtoan_diemthuong_eaha(sdt_kh);
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm sản phẩm vào hóa đơn thành công.", "4000", "warning"), true);
                                    }
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Sản phẩm không hợp lệ.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_xoahoadon_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_4") == "" || bcorn_class.check_quyen(user, "n7_4") == "")
        {
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
            var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (hd_cl.exist_id(id, user_parent))
            {
                hd_cl.del(id);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa hóa đơn thành công.", "2000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-hoa-don/Default.aspx");
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_form_edithoadon_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_3") == "" || bcorn_class.check_quyen(user, "n7_3") == "")
        {
            string _ck = txt_chietkhau_hoadon.Text.Trim().Replace(".", "");
            int _r1 = 0;
            int.TryParse(_ck, out _r1);//nếu là số nguyên thì gán cho _r

            string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
            string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
            string _ghichu = txt_ghichu.Text;
            string _diachi = txt_diachi.Text;
            string _ngaytao = txt_ngaytao.Text;
            if (co_ngu_canh_datlich)
                _ghichu = datlich_lienket_class.dam_bao_ghi_chu_datlich(_ghichu, id_datlich_lienket, "Tạo từ", ten_dichvu_datlich);
            if (dt_cl.check_date(_ngaytao) == false)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày tạo không hợp lệ", "4000", "warning"), true);
            else
            {
                if (_tenkhachhang == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên khách hàng.", "4000", "warning"), true);
                else
                {
                    if ((_r1 < 0 && ck_hd_phantram.Checked == true) || (_r1 > 100 && ck_hd_phantram.Checked == true))//nếu chọn % thì k đc <0 & >100 
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu hóa đơn không hợp lệ.", "4000", "warning"), true);
                    else
                    {
                        var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        if (hd_cl.exist_id(id, user_parent))
                        {
                            bspa_hoadon_table _ob = q.First();
                            _ob.ngaytao = DateTime.Parse(_ngaytao + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                            _ob.tenkhachhang = _tenkhachhang;
                            _ob.ghichu = _ghichu;
                            _ob.diachi = _diachi;
                            _ob.sdt = _sdt;


                            if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
                            {
                                _ob.chietkhau = _r1;
                                _ob.tongsauchietkhau = _ob.tongtien - (_ob.tongtien * _r1 / 100);
                                _ob.tongtien_ck_hoadon = _ob.tongtien - _ob.tongsauchietkhau;
                            }
                            else
                            {
                                _ob.chietkhau = 0;
                                _ob.tongsauchietkhau = _ob.tongtien - _r1;
                                _ob.tongtien_ck_hoadon = _r1;
                            }

                            _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;

                            db.SubmitChanges();
                            //reload();
                            //update_hoadon();
                            //reload_thanhtoan();
                            //main();
                            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "4000", "warning"), true);
                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "2000", "warning");
                            Response.Redirect("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + id);
                        }
                    }
                }

            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void but_xoathanhtoan_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_6") == "" || bcorn_class.check_quyen(user, "n7_6") == "")
        {
            var q_thanhtoan = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
            foreach (var t in q_thanhtoan)
            {
                if (Request.Form["check_lichsu_thanhtoan_" + t.id.ToString()] == "on")
                {
                    var q = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id == t.id && p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        Int64 _sotien_thanhtoan = q.First().sotienthanhtoan.Value;
                        string _hinhthuc_thanhtoan = q.First().hinhthuc_thanhtoan;

                        var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        bspa_hoadon_table _ob = q_hoadon.First();
                        _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan - _sotien_thanhtoan;
                        _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;
                        if (_hinhthuc_thanhtoan == "Tiền mặt")
                            _ob.thanhtoan_tienmat = _ob.thanhtoan_tienmat - _sotien_thanhtoan;
                        if (_hinhthuc_thanhtoan == "Chuyển khoản")
                            _ob.thanhtoan_chuyenkhoan = _ob.thanhtoan_chuyenkhoan - _sotien_thanhtoan;
                        if (_hinhthuc_thanhtoan == "Quẹt thẻ")
                            _ob.thanhtoan_quetthe = _ob.thanhtoan_quetthe - _sotien_thanhtoan;
                        db.SubmitChanges();



                        //xóa lịch sử
                        bspa_lichsu_thanhtoan_table _ob1 = q.First();
                        HoaDonThuChiSync_cl.DeleteForInvoicePayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                        db.bspa_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
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
        if (bcorn_class.check_quyen(user, "q7_5") == "" || bcorn_class.check_quyen(user, "n7_5") == "")
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
                    var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    bspa_hoadon_table _ob = q.First();
                    _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan + _st;
                    _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;
                    if (ddl_hinhthuc_thanhtoan.SelectedValue.ToString() == "Tiền mặt")
                        _ob.thanhtoan_tienmat = _ob.thanhtoan_tienmat + _st;
                    if (ddl_hinhthuc_thanhtoan.SelectedValue.ToString() == "Chuyển khoản")
                        _ob.thanhtoan_chuyenkhoan = _ob.thanhtoan_chuyenkhoan + _st;
                    if (ddl_hinhthuc_thanhtoan.SelectedValue.ToString() == "Quẹt thẻ")
                        _ob.thanhtoan_quetthe = _ob.thanhtoan_quetthe + _st;
                    db.SubmitChanges();

                    var q1 = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    bspa_lichsu_thanhtoan_table _ob1 = new bspa_lichsu_thanhtoan_table();
                    _ob1.id_nganh = Session["nganh"].ToString();
                    _ob1.id_hoadon = id;
                    _ob1.sotienthanhtoan = _st;
                    _ob1.thoigian = DateTime.Parse(txt_ngaythanhtoan.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                    _ob1.user_parent = user_parent;
                    _ob1.hinhthuc_thanhtoan = ddl_hinhthuc_thanhtoan.SelectedValue.ToString();
                    _ob1.nguoithanhtoan = user;

                    _ob1.id_chinhanh = Session["chinhanh"].ToString();
                    _ob1.id_nganh = hd_cl.return_object(id).id_nganh;
                    db.bspa_lichsu_thanhtoan_tables.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                    HoaDonThuChiSync_cl.UpsertFromInvoicePayment(db, _ob, _ob1);
                    db.SubmitChanges();

                    string _id_datlich_thanh_toan = datlich_lienket_class.lay_id_datlich_tu_ghichu(_ob.ghichu);
                    if (_id_datlich_thanh_toan != "")
                    {
                        string _ghi_chu_thanh_toan = "Đã thu " + _st.ToString("#,##0") + " đ bằng " + ddl_hinhthuc_thanhtoan.SelectedValue.ToString() + " cho HĐ #" + id;
                        if ((_ob.sotien_conlai ?? 0) <= 0)
                            _ghi_chu_thanh_toan = "Đã thanh toán đủ HĐ #" + id;
                        else
                            _ghi_chu_thanh_toan += ", còn nợ " + (_ob.sotien_conlai ?? 0).ToString("#,##0") + " đ";

                        datlich_lienket_class.dong_bo_vao_lich_hen(
                            db,
                            _id_datlich_thanh_toan,
                            Session["chinhanh"].ToString(),
                            id,
                            "",
                            "",
                            _ob1.thoigian,
                            _ghi_chu_thanh_toan,
                            false);
                        db.SubmitChanges();
                    }

                    update_hoadon();
                    reload_thanhtoan();
                    main();
                    txt_sotien_thanhtoan_congno.Text = _ob.sotien_conlai.Value.ToString("#,##0");

                    dtkh_cl.tinhtong_chitieu_update_capbac(sdt_kh);//cập nhật điểm eha

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

    //autocomplete
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers1(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.web_post_tables.Where(p => p.name.Contains(prefixText) && p.phanloai == "ctsp" && p.bin == false && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.name).ToList();
    }

    //autocomplete dịch vụ
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.web_post_tables.Where(p => p.name.Contains(prefixText) && p.phanloai == "ctdv" && p.bin == false && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.name).ToList();
    }

    protected void txt_tendichvu_TextChanged(object sender, EventArgs e)
    {
        string _ten_dichvu = txt_tendichvu.Text.ToString();
        string _id_dichvu = po_cl.return_id(_ten_dichvu);
        if (po_cl.exist_id(_id_dichvu))
        {
            var q1 = db.web_post_tables.Where(p => p.id.ToString() == _id_dichvu && p.phanloai == "ctdv" && p.bin == false && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q1.Count() != 0)
            {
                txt_gia.Text = q1.First().giaban_dichvu.Value.ToString("#,##0");
                txt_chietkhau_chotsale.Text = q1.First().phantram_chotsale_dichvu.Value.ToString();
                txt_chietkhau_lamdichvu.Text = q1.First().phantram_lamdichvu.Value.ToString();
            }
        }
        else
        {
            txt_gia.Text = "";
            txt_chietkhau_chotsale.Text = "";
            txt_chietkhau_lamdichvu.Text = "";
        }
    }

    protected void txt_tensanpham_TextChanged(object sender, EventArgs e)
    {
        string _ten_sanpham = txt_tensanpham.Text.ToString();
        string _id_sanpham = po_cl.return_id(_ten_sanpham);
        if (po_cl.exist_id(_id_sanpham))
        {
            var q1 = db.web_post_tables.Where(p => p.id.ToString() == _id_sanpham && p.phanloai == "ctsp" && p.bin == false && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q1.Count() != 0)
            {
                txt_gia_sanpham.Text = q1.First().giaban_sanpham.Value.ToString("#,##0");
                txt_chietkhau_chotsale_sanpham.Text = q1.First().phantram_chotsale_sanpham.Value.ToString();
            }
        }
        else
        {
            txt_gia_sanpham.Text = "";
            txt_chietkhau_chotsale_sanpham.Text = "";
        }
    }


    //thêm dv bằng thẻ dv
    protected void Button1_Click(object sender, EventArgs e)
    {


        string _id_thedv = "";
        int _count = 0;
        if (bcorn_class.check_quyen(user, "q7_2") == "" || bcorn_class.check_quyen(user, "n7_2") == "")
        {
            //đảm bảo rằng chỉ chọn 1 lần 1 thẻ dv
            for (int i = 0; i < list_id_split_thedv.Count; i++)
            {
                if (Request.Form[list_id_split_thedv[i]] == "on")
                {
                    string _id = list_id_split_thedv[i].Replace("check_thedv_", "");
                    var q = db.thedichvu_tables.Where(p => p.id.ToString() == _id && p.sdt == sdt_kh && p.hsd.Value.Date >= DateTime.Now.Date && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (tdv_cl.exist_id(_id, user_parent))
                    {
                        _id_thedv = _id;
                        _count = _count + 1;
                    }
                }
            }

            if (_count > 1 || _count == 0)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn 1 thẻ dịch vụ.", "false", "false", "OK", "alert", ""), true);
            else
            {
                var q_thedv = db.thedichvu_tables.Where(p => p.id.ToString() == _id_thedv && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q_thedv.First().sl_conlai <= 0)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Thẻ này đã sử dụng hết số buổi.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    string _ten_dichvu = tdv_cl.return_object(_id_thedv).ten_taithoidiemnay;
                    string _id_dichvu = tdv_cl.return_object(_id_thedv).iddv;
                    Int64 _gia_1buoi = tdv_cl.return_object(_id_thedv).tongsauchietkhau.Value / tdv_cl.return_object(_id_thedv).tongsoluong.Value;

                    string _user_lamdichvu = ddl_nhanvien_lamdichvu_thedv.SelectedValue.ToString();

                    int _r1 = 0;//giá
                    int _r3 = 1;//sl


                    string _phantram_lam = txt_chietkhau_lamdichvu_thedv.Text.Trim().Replace(".", "");
                    int _r5 = 0;
                    int.TryParse(_phantram_lam, out _r5);

                    //var q_chitiet_hoadon = db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_dvsp == _id_dichvu && p.id_hoadon == id);

                    if (po_cl.exist_id(_id_dichvu))
                    {
                        if (_r1 < 0)//giá k đc = 0            
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Giá dịch vụ không hợp lệ.", "4000", "warning"), true);
                        else
                        {
                            if (_r3 < 1)//số lượng     
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số lượng không hợp lệ.", "4000", "warning"), true);
                            else
                            {

                                if ((_r5 < 0 && ck_dv_phantram_lamdv_thedv.Checked == true) || (_r5 > 100 && ck_dv_phantram_lamdv_thedv.Checked == true))//nếu chọn % thì k đc <0 & >100
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu cho nhân viên thực hiện không hợp lệ.", "4000", "warning"), true);
                                else
                                {

                                    bspa_hoadon_chitiet_table _ob = new bspa_hoadon_chitiet_table();
                                    _ob.id_thedichvu = _id_thedv;//đánh dấu dv này là sử dụng thẻ dv
                                    _ob.gia_hienthi_khidung_thedv = _gia_1buoi;
                                    _ob.id_dvsp = _id_dichvu;
                                    _ob.ten_dvsp_taithoidiemnay = _ten_dichvu;
                                    _ob.gia_dvsp_taithoidiemnay = _r1;
                                    _ob.soluong = _r3;
                                    _ob.thanhtien = _r1 * _r3;
                                    _ob.hinhanh_hientai = "";
                                    _ob.chietkhau = 0;
                                    _ob.tongsauchietkhau = 0;
                                    _ob.tongtien_ck_dvsp = 0;
                                    _ob.ngaytao = DateTime.Parse(txt_ngayban_thedv.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());

                                    _ob.nguoichot_dvsp = "";
                                    _ob.tennguoichot_hientai = "";
                                    _ob.phantram_chotsale_dvsp = 0;
                                    _ob.tongtien_chotsale_dvsp = 0;

                                    _ob.nguoilam_dichvu = _user_lamdichvu;

                                    if (_user_lamdichvu == "")
                                    {
                                        _ob.phantram_lamdichvu = 0;
                                        _ob.tongtien_lamdichvu = 0;
                                        _ob.tennguoilam_hientai = "";
                                    }
                                    else
                                    {
                                        _ob.tennguoilam_hientai = tk_cl.return_object(_user_lamdichvu).hoten;
                                        if (ck_dv_phantram_lamdv_thedv.Checked == true)//nếu ck thực hiện là %
                                        {
                                            _ob.phantram_lamdichvu = _r5;
                                            _ob.tongtien_lamdichvu = _gia_1buoi * _r5 / 100;
                                        }
                                        else
                                        {
                                            _ob.phantram_lamdichvu = 0;
                                            _ob.tongtien_lamdichvu = _r5;
                                        }
                                    }

                                    _ob.id_hoadon = id;
                                    _ob.kyhieu = "dichvu";
                                    _ob.user_parent = user_parent;
                                    _ob.nguoitao = Session["user"].ToString();
                                    _ob.danhgia_nhanvien_lamdichvu = txt_danhgia_dichvu_lamdv.Text.Trim();
                                    _ob.danhgia_5sao_dv = Request.Form["danhgia_5sao_nhanvien_dv_lamdv"];

                                    _ob.id_chinhanh = Session["chinhanh"].ToString();
                                    db.bspa_hoadon_chitiet_tables.InsertOnSubmit(_ob);
                                    db.SubmitChanges();

                                    if (co_ngu_canh_datlich)
                                    {
                                        var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                                        if (q_hoadon.Count() != 0)
                                            q_hoadon.First().ghichu = datlich_lienket_class.dam_bao_ghi_chu_datlich(q_hoadon.First().ghichu, id_datlich_lienket, "Sử dụng thẻ từ", ten_dichvu_datlich);
                                    }


                                    //công trừ số buổi của thẻ

                                    thedichvu_table _ob_thedv = q_thedv.First();
                                    _ob_thedv.sl_dalam = _ob_thedv.sl_dalam + 1;
                                    _ob_thedv.sl_conlai = _ob_thedv.tongsoluong - _ob_thedv.sl_dalam;
                                    if (co_ngu_canh_datlich)
                                    {
                                        datlich_lienket_class.dong_bo_vao_lich_hen(
                                            db,
                                            id_datlich_lienket,
                                            Session["chinhanh"].ToString(),
                                            id,
                                            _id_thedv,
                                            _user_lamdichvu,
                                            _ob.ngaytao,
                                            "Đã dùng thẻ DV #" + _id_thedv + " trong hóa đơn #" + id,
                                            true);
                                    }
                                    db.SubmitChanges();

                                    reload();
                                    update_hoadon();
                                    reload_thanhtoan();
                                    ddl_nhanvien_lamdichvu_thedv.SelectedIndex = 0; txt_chietkhau_lamdichvu_thedv.Text = "0"; txt_danhgia_dichvu_lamdv.Text = "";
                                    main();
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Sử dụng thẻ dịch vụ thành công.", "4000", "warning"), true);
                                    //}
                                    //}

                                }
                            }



                        }

                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Dịch vụ không tồn tại.", "4000", "warning"), true);
                }
            }

        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void but_form_edithoadon1_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_3") == "" || bcorn_class.check_quyen(user, "n7_3") == "")
        {
            string _ck = txt_chietkhau_hoadon1.Text.Trim().Replace(".", "");
            int _r1 = 0;
            int.TryParse(_ck, out _r1);//nếu là số nguyên thì gán cho _r

            string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
            string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
            string _ghichu = txt_ghichu.Text;
            string _diachi = txt_diachi.Text;
            string _ngaytao = txt_ngaytao.Text;
            if (co_ngu_canh_datlich)
                _ghichu = datlich_lienket_class.dam_bao_ghi_chu_datlich(_ghichu, id_datlich_lienket, "Tạo từ", ten_dichvu_datlich);
            if (dt_cl.check_date(_ngaytao) == false)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày tạo không hợp lệ", "4000", "warning"), true);
            else
            {
                if (_tenkhachhang == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên khách hàng.", "4000", "warning"), true);
                else
                {
                    if ((_r1 < 0 && ck_hd_phantram1.Checked == true) || (_r1 > 100 && ck_hd_phantram1.Checked == true))//nếu chọn % thì k đc <0 & >100 
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu hóa đơn không hợp lệ.", "4000", "warning"), true);
                    else
                    {
                        var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        if (hd_cl.exist_id(id, user_parent))
                        {
                            bspa_hoadon_table _ob = q.First();
                            _ob.ngaytao = DateTime.Parse(_ngaytao + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                            _ob.tenkhachhang = _tenkhachhang;
                            _ob.ghichu = _ghichu;
                            _ob.diachi = _diachi;
                            _ob.sdt = _sdt;


                            if (ck_hd_phantram1.Checked == true)//nếu ck chốt sale là %
                            {
                                _ob.chietkhau = _r1;
                                _ob.tongsauchietkhau = _ob.tongtien - (_ob.tongtien * _r1 / 100);
                                _ob.tongtien_ck_hoadon = _ob.tongtien - _ob.tongsauchietkhau;
                            }
                            else
                            {
                                _ob.chietkhau = 0;
                                _ob.tongsauchietkhau = _ob.tongtien - _r1;
                                _ob.tongtien_ck_hoadon = _r1;
                            }

                            _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;

                            db.SubmitChanges();
                            //reload();
                            //update_hoadon();
                            //reload_thanhtoan();
                            //main();
                            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "4000", "warning"), true);
                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "2000", "warning");
                            Response.Redirect("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + id);
                        }
                    }
                }

            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
}
