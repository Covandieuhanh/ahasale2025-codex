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
    datetime_class dt_cl = new datetime_class();
    data_khachhang_class dtkh_cl = new data_khachhang_class();

    public string user, user_parent, homnay;
    public Int64 chitieu_dichvu = 0, chitieu_sanpham = 0, chitieu_tong = 0, chitieu_tb = 0, doanhso_dv = 0, doanhso_sp = 0, doanhso_tong = 0;
    public Int64 chiphi_codinh, sotien_dathanhtoan_hoadon, tongthu_tu_thuchi, tongdoanhso, tongdoanhthu, tongchi_tu_thuchi, tongchiphi, tongloinhuan;
    public Int64 tongcongno = 0;
    public int tonghoadon = 0, tonglichhen = 0, tongkhachmoi = 0;
    public string tongloinhuan_css = "fg-green";
    public double phantram_dv = 0, phantram_sp = 0, phantram_tong = 0;
    public lichhen_dieuphoi_tongquan dieu_phoi_homnay = new lichhen_dieuphoi_tongquan();

    public List<lichhen_dieuphoi_nhanvien_item> return_top_nhanvien_dieu_phoi()
    {
        if (dieu_phoi_homnay == null || dieu_phoi_homnay.list_nhanvien == null)
            return new List<lichhen_dieuphoi_nhanvien_item>();

        return dieu_phoi_homnay.list_nhanvien.Take(4).ToList();
    }

    public List<lichhen_dieuphoi_canhbao_item> return_top_canhbao_dieu_phoi()
    {
        if (dieu_phoi_homnay == null || dieu_phoi_homnay.list_canhbao == null)
            return new List<lichhen_dieuphoi_canhbao_item>();

        return dieu_phoi_homnay.list_canhbao.Take(4).ToList();
    }

    public void get_ngay()
    {
        if (!IsPostBack)
        {
            if (Session["tungay_thongke_home"] == null)
            {
                txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToShortDateString();
                Session["tungay_thongke_home"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_thongke_home"].ToString();

            if (Session["denngay_thongke_home"] == null)
            {
                txt_denngay.Text = DateTime.Now.ToShortDateString();
                Session["denngay_thongke_home"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_thongke_home"].ToString();
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
        user = Session["user"].ToString();
        user_parent = "admin";
        dieu_phoi_homnay = lichhen_dieuphoi_class.tai_tongquan_homnay(db, Session["chinhanh"].ToString());

        Repeater4.DataSource = db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());
        Repeater4.DataBind();

        //if (!IsPostBack)
        //{
        //    foreach (var t in db.bspa_data_khachhang_tables)
        //        dtkh_cl.tinhtong_chitieu_update_capbac(t.sdt);
        //}

        //homnay = dt_cl.return_thuvietnam(DateTime.Now) + ", ngày " + DateTime.Now.Day + " tháng " + ddl_thang.SelectedValue.ToString() + " năm " + Session["nam_thongke_home"].ToString();
        get_ngay();
        thongke_tonghop();
        dichvu_banchay();
        sanpham_banchay();
        thongke_lamdichvu();
    }


    #region thống kê doanh số, doanh thu, công nợ
    public Int64 tinhdoanhso_hoadon_nganh(string _id_nganh)//SP-DV từ hóa đơn
    {
        get_ngay();
        var q = db.bspa_hoadon_tables.Where(p => p.id_nganh == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.tongsauchietkhau).Value;
    }
    public Int64 tinhdoanhthu_hoadon_nganh(string _id_nganh)//SP-DV từ hóa đơn
    {
        get_ngay();
        var q = db.bspa_hoadon_tables.Where(p => p.id_nganh == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.sotien_dathanhtoan).Value;
    }
    public Int64 tinhcongno_hoadon_nganh(string _id_nganh)//SP-DV từ hóa đơn
    {
        get_ngay();
        var q = db.bspa_hoadon_tables.Where(p => p.id_nganh == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.sotien_conlai).Value;
    }
    public Int64 tinhdoanhso_thedichvu_nganh(string _id_nganh)//thẻ dịch vụ
    {
        get_ngay();
        var q = db.thedichvu_tables.Where(p => p.id_nganh == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.tongsauchietkhau).Value;
    }
    public Int64 tinhdoanhthu_thedichvu_nganh(string _id_nganh)//thẻ dịch vụ
    {
        get_ngay();
        var q = db.thedichvu_tables.Where(p => p.id_nganh == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.sotien_dathanhtoan).Value;
    }
    public Int64 tinhcongno_thedichvu_nganh(string _id_nganh)//thẻ dịch vụ
    {
        get_ngay();
        var q = db.thedichvu_tables.Where(p => p.id_nganh == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.sotien_conlai).Value;
    }
    public Int64 tinhdoanhso_hocvien_nganh(string _id_nganh)//thành viên
    {
        get_ngay();
        var q = db.hocvien_tables.Where(p => p.nganhhoc == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.hocphi).Value;
    }
    public Int64 tinhdoanhthu_hocvien_nganh(string _id_nganh)//thành viên
    {
        get_ngay();
        var q = db.hocvien_tables.Where(p => p.nganhhoc == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.sotien_dathanhtoan).Value;
    }
    public Int64 tinhcongno_hocvien_nganh(string _id_nganh)//thành viên
    {
        get_ngay();
        var q = db.hocvien_tables.Where(p => p.nganhhoc == _id_nganh && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList();
        return q.Sum(p => p.sotien_conlai).Value;
    }
    
    public void thongke_tonghop()
    {
        get_ngay();
        string _chinhanh = Session["chinhanh"].ToString();
        DateTime _from = DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date;
        DateTime _to = DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date;

        var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id_chinhanh == _chinhanh && p.ngaytao != null && p.ngaytao.Value.Date >= _from && p.ngaytao.Value.Date <= _to);
        var q_thedv = db.thedichvu_tables.Where(p => p.id_chinhanh == _chinhanh && p.ngaytao != null && p.ngaytao.Value.Date >= _from && p.ngaytao.Value.Date <= _to);
        var q_hocvien = db.hocvien_tables.Where(p => p.id_chinhanh == _chinhanh && p.ngaytao != null && p.ngaytao.Value.Date >= _from && p.ngaytao.Value.Date <= _to);

        tonghoadon = q_hoadon.Count();
        tongdoanhso = (q_hoadon.Sum(p => (long?)p.tongsauchietkhau) ?? 0)
                      + (q_thedv.Sum(p => (long?)p.tongsauchietkhau) ?? 0)
                      + (q_hocvien.Sum(p => (long?)p.hocphi) ?? 0);

        tongdoanhthu = (q_hoadon.Sum(p => (long?)p.sotien_dathanhtoan) ?? 0)
                       + (q_thedv.Sum(p => (long?)p.sotien_dathanhtoan) ?? 0)
                       + (q_hocvien.Sum(p => (long?)p.sotien_dathanhtoan) ?? 0);

        tongcongno = (q_hoadon.Sum(p => (long?)p.sotien_conlai) ?? 0)
                     + (q_thedv.Sum(p => (long?)p.sotien_conlai) ?? 0)
                     + (q_hocvien.Sum(p => (long?)p.sotien_conlai) ?? 0);

        var q_thuchi = db.bspa_thuchi_tables.Where(p => p.id_chinhanh == _chinhanh && p.duyet_phieuchi == "Đã duyệt" && p.ngay != null && p.ngay.Value.Date >= _from && p.ngay.Value.Date <= _to);
        tongthu_tu_thuchi = q_thuchi.Where(p => p.thuchi == "Thu").Sum(p => (long?)p.sotien) ?? 0;
        tongchi_tu_thuchi = q_thuchi.Where(p => p.thuchi == "Chi").Sum(p => (long?)p.sotien) ?? 0;
        tongloinhuan = tongthu_tu_thuchi - tongchi_tu_thuchi;
        tongloinhuan_css = tongloinhuan >= 0 ? "fg-green" : "fg-red";

        tongkhachmoi = db.bspa_data_khachhang_tables.Where(p => p.id_chinhanh == _chinhanh && p.ngaytao != null && p.ngaytao.Value.Date >= _from && p.ngaytao.Value.Date <= _to).Count();

        tonglichhen = db.bspa_datlich_tables.Where(p => p.id_chinhanh == _chinhanh &&
            ((p.ngaydat != null && p.ngaydat.Value.Date >= _from && p.ngaydat.Value.Date <= _to) ||
             (p.ngaydat == null && p.ngaytao != null && p.ngaytao.Value.Date >= _from && p.ngaytao.Value.Date <= _to))
        ).Count();
    }

#endregion

    #region thống kê làm dịch vụ

    public void thongke_lamdichvu()
    {
        var q = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                 join ob2 in db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList() on ob1.taikhoan.ToString() equals ob2.nguoilam_dichvu
                 group ob2 by new { ob1.taikhoan, ob1.hoten } into g
                 select new
                 {
                     username = g.Key.taikhoan,
                     fullname = g.Key.hoten,
                     soluong = g.Count(),
                 });
        Repeater3.DataSource = q.OrderByDescending(p => p.soluong).Take(5);
        Repeater3.DataBind();
    }

    public string return_fullname(string _username)
    {
        taikhoan_table_2023 _ob = tk_cl.return_object(_username);
        return _ob.hoten;
    }

    #endregion

    public void dichvu_banchay()
    {
        var list_all = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.kyhieu == "dichvu" && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList() on ob1.id.ToString() equals ob2.id_dvsp
                        group ob2 by new { ob1.id, ob1.name } into g
                        select new
                        {
                            id = g.Key.id,
                            tendvsp = g.Key.name,
                            tongsl = g.Sum(ob2 => ob2.soluong).Value,
                        });


        list_all = list_all.OrderByDescending(p => p.tongsl).Take(5).ToList();
        Repeater1.DataSource = list_all;
        Repeater1.DataBind();
    }
    public void sanpham_banchay()
    {
        var list_all = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctsp" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.kyhieu == "sanpham" && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thongke_home"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thongke_home"].ToString()).Date).ToList() on ob1.id.ToString() equals ob2.id_dvsp
                        group ob2 by new { ob1.id, ob1.name } into g
                        select new
                        {
                            id = g.Key.id,
                            tendvsp = g.Key.name,
                            tongsl = g.Sum(ob2 => ob2.soluong).Value,
                        });


        list_all = list_all.OrderByDescending(p => p.tongsl).Take(5).ToList();
        Repeater2.DataSource = list_all;
        Repeater2.DataBind();
    }

    public void reset_ss()
    {
        Session["tungay_thongke_home"] = null;
        Session["denngay_thongke_home"] = null;
    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "2000", "warning");
        Response.Redirect("/gianhang/admin");
    }
    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["tungay_thongke_home"] = txt_tungay.Text;
        Session["denngay_thongke_home"] = txt_denngay.Text;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "2000", "warning");
        Response.Redirect("/gianhang/admin");
    }

    #region chọn ngày nhanh
    protected void but_homqua_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        txt_denngay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_homnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.ToString();
        txt_denngay.Text = DateTime.Now.Date.ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuantruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaycuoituan().ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunamtruoc(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinamtruoc(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quytruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quynay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }


    #endregion
}
