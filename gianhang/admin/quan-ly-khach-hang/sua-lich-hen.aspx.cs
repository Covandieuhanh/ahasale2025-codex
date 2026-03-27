using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public class booking_workflow_step_item
{
    public string tieu_de { get; set; }
    public string mo_ta { get; set; }
    public string trang_thai { get; set; }
    public string css { get; set; }
    public string icon_css { get; set; }
}

public partial class admin_quan_ly_khach_hang_danh_sach_lich_hen : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    public string user, user_parent, id;
    public datlich_vanhanh_tongquan tongquan_vanhanh = new datlich_vanhanh_tongquan();
    public string url_ho_so_khach = "";
    public string url_tao_hoa_don = "";
    public string url_ban_thedv = "";
    public string url_sudung_thedv = "";
    public string ten_dichvu_hien_tai = "";
    public List<datlich_lienket_hoadon_item> list_hoadon_lienket = new List<datlich_lienket_hoadon_item>();
    public List<datlich_lienket_thedv_item> list_thedv_lienket = new List<datlich_lienket_thedv_item>();
    public khachhang_vanhanh_tongquan tongquan_crm = new khachhang_vanhanh_tongquan();
    public List<booking_workflow_step_item> list_buoc_vanhanh = new List<booking_workflow_step_item>();
    public string goi_y_workflow = "";
    public string url_goi_y_workflow = "";



    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login  
        string _quyen = "q10_3";
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
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            var q = db.bspa_datlich_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q.Count() != 0)
            {
                if (!IsPostBack)
                {
                    bspa_datlich_table _ob = q.First();

                    var list_dichvu = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                       select new { tendv = ob1.name, iddv = ob1.id, }
                           );
                    ddl_dichvu.DataSource = list_dichvu;
                    ddl_dichvu.DataTextField = "tendv";
                    ddl_dichvu.DataValueField = "iddv";
                    ddl_dichvu.DataBind();
                    ddl_dichvu.Items.Insert(0, new ListItem("Chọn dịch vụ", ""));
                    if (_ob.dichvu != "")
                        datlich_class.try_select_dropdown_value(ddl_dichvu, _ob.dichvu);

                    datlich_class.bind_gio_phut(ddl_giobatdau, ddl_phutbatdau, _ob.ngaydat);

                    var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                         select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                              );
                    ddl_nhanvien.DataSource = list_nhanvien;
                    ddl_nhanvien.DataTextField = "tennhanvien";
                    ddl_nhanvien.DataValueField = "username";
                    ddl_nhanvien.DataBind();
                    ddl_nhanvien.Items.Insert(0, new ListItem("Chọn nhân viên", ""));
                    if (_ob.nhanvien_thuchien != "")
                        datlich_class.try_select_dropdown_value(ddl_nhanvien, _ob.nhanvien_thuchien);

                    txt_ngaydat.Text = datlich_class.return_ngay_text(_ob.ngaydat.Value);

                    txt_tenkhachhang.Text = _ob.tenkhachhang;
                    txt_sdt.Text = _ob.sdt;
                    txt_ghichu.Text = _ob.ghichu;
                    txt_nguon.Text = _ob.nguongoc;
                    datlich_class.try_select_dropdown_value(ddl_trangthai, datlich_class.chuanhoa_trangthai(_ob.trangthai));
                }

                nap_tongquan_vanhanh(q.First());

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
        string _sdt = datlich_class.chuanhoa_sdt(txt_sdt.Text);
        txt_sdt.Text = _sdt;
        if (_sdt != "")
        {
            var q1 = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao);
            if (q1.Count() != 0)
            {
                txt_tenkhachhang.Text = q1.First().tenkhachhang;
                //txt_diachi.Text = q1.First().diachi;
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
                //txt_diachi.Text = q1.First().diachi;
            }

        }
    }


    public void reset_ss()
    {
        Session["index_sapxep_lichhen"] = null;
        Session["current_page_lichhen"] = null;
        Session["search_lichhen"] = null;
        Session["show_lichhen"] = null;
        Session["tungay_lichhen"] = null;
        Session["denngay_lichhen"] = null;

        Session["index_loc_trangthai_lichhen"] = null;
    }

    public string return_ngaygio_hienthi(DateTime? _value)
    {
        if (_value.HasValue == false)
            return "";
        return _value.Value.ToString("dd/MM/yyyy HH:mm");
    }

    public List<khachhang_vanhanh_timeline_item> return_timeline_datlich()
    {
        if (tongquan_crm == null || tongquan_crm.list_timeline == null)
            return new List<khachhang_vanhanh_timeline_item>();

        return tongquan_crm.list_timeline.Take(6).ToList();
    }

    private void nap_buoc_vanhanh(bspa_datlich_table _lich)
    {
        list_buoc_vanhanh = new List<booking_workflow_step_item>();
        if (_lich == null)
            return;

        string _trangthai = datlich_class.chuanhoa_trangthai(_lich.trangthai);
        bool _da_huy = _trangthai == datlich_class.trangthai_da_huy;
        bool _khong_den = _trangthai == datlich_class.trangthai_khong_den;
        bool _da_xacnhan = _trangthai == datlich_class.trangthai_da_xacnhan || _trangthai == datlich_class.trangthai_da_den;
        bool _da_den = _trangthai == datlich_class.trangthai_da_den;
        bool _co_giao_dich = list_hoadon_lienket.Count != 0 || list_thedv_lienket.Count != 0;
        bool _co_hoa_don = list_hoadon_lienket.Count != 0;
        bool _da_thanh_toan_xong = _co_hoa_don && list_hoadon_lienket.All(p => p.sotien_conlai <= 0);
        bool _congno = _co_hoa_don && list_hoadon_lienket.Any(p => p.sotien_conlai > 0);

        list_buoc_vanhanh.Add(new booking_workflow_step_item
        {
            tieu_de = "Đặt lịch",
            mo_ta = "Lịch #" + _lich.id + " từ " + ((_lich.nguongoc ?? "").Trim() != "" ? _lich.nguongoc.Trim() : "Trực tiếp"),
            trang_thai = "Hoàn tất",
            css = "booking-workflow__step--done",
            icon_css = "mif-calendar"
        });

        if (_da_huy)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Xác nhận",
                mo_ta = "Lịch đã hủy trước khi chốt vận hành.",
                trang_thai = "Dừng",
                css = "booking-workflow__step--blocked",
                icon_css = "mif-cancel"
            });
        }
        else if (_da_xacnhan)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Xác nhận",
                mo_ta = !string.IsNullOrWhiteSpace(_lich.nhanvien_thuchien) ? "Đã gán nhân viên " + datlich_class.return_ten_nguoitao_hienthi(_lich.nhanvien_thuchien) : "Lịch đã được xác nhận.",
                trang_thai = "Hoàn tất",
                css = "booking-workflow__step--done",
                icon_css = "mif-checkmark"
            });
        }
        else
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Xác nhận",
                mo_ta = "Cần chốt lịch với khách và gán nhân viên trước giờ hẹn.",
                trang_thai = "Đang chờ",
                css = "booking-workflow__step--current",
                icon_css = "mif-phone"
            });
        }

        if (_da_huy)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Check-in",
                mo_ta = "Không tiếp nhận vì lịch đã hủy.",
                trang_thai = "Dừng",
                css = "booking-workflow__step--blocked",
                icon_css = "mif-blocked"
            });
        }
        else if (_khong_den)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Check-in",
                mo_ta = "Khách không đến theo lịch đã xác nhận.",
                trang_thai = "No-show",
                css = "booking-workflow__step--blocked",
                icon_css = "mif-user-minus"
            });
        }
        else if (_da_den)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Check-in",
                mo_ta = "Khách đã đến chi nhánh và có thể xử lý dịch vụ.",
                trang_thai = "Hoàn tất",
                css = "booking-workflow__step--done",
                icon_css = "mif-enter"
            });
        }
        else
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Check-in",
                mo_ta = _da_xacnhan ? "Đang chờ khách đến hoặc staff tiếp nhận tại quầy." : "Cần xác nhận trước khi check-in.",
                trang_thai = _da_xacnhan ? "Sắp tới" : "Chưa mở",
                css = _da_xacnhan ? "booking-workflow__step--current" : "booking-workflow__step--pending",
                icon_css = "mif-user-check"
            });
        }

        if (_da_huy || _khong_den)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Xử lý dịch vụ",
                mo_ta = "Flow dịch vụ dừng vì lịch không được thực hiện.",
                trang_thai = "Dừng",
                css = "booking-workflow__step--blocked",
                icon_css = "mif-blocked"
            });
        }
        else if (_co_giao_dich)
        {
            string _mota_giaodich = _co_hoa_don ? "Đã mở " + list_hoadon_lienket.Count.ToString("#,##0") + " hóa đơn liên quan." : "Đã ghi nhận thẻ dịch vụ từ lịch này.";
            if (list_thedv_lienket.Count != 0)
                _mota_giaodich += " Có " + list_thedv_lienket.Count.ToString("#,##0") + " thẻ DV đi từ lịch.";

            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Xử lý dịch vụ",
                mo_ta = _mota_giaodich,
                trang_thai = "Đang xử lý",
                css = "booking-workflow__step--done",
                icon_css = "mif-briefcase"
            });
        }
        else
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Xử lý dịch vụ",
                mo_ta = _da_den ? "Tạo hóa đơn hoặc dùng thẻ để tiếp tục dịch vụ." : "Sẽ mở khi khách đến và staff bắt đầu xử lý.",
                trang_thai = _da_den ? "Cần thao tác" : "Chưa mở",
                css = _da_den ? "booking-workflow__step--current" : "booking-workflow__step--pending",
                icon_css = "mif-lab"
            });
        }

        if (_da_huy || _khong_den)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Thanh toán",
                mo_ta = "Không phát sinh thanh toán cho lịch này.",
                trang_thai = "Dừng",
                css = "booking-workflow__step--blocked",
                icon_css = "mif-blocked"
            });
        }
        else if (_da_thanh_toan_xong)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Thanh toán",
                mo_ta = "Các hóa đơn liên kết đã thanh toán đủ.",
                trang_thai = "Hoàn tất",
                css = "booking-workflow__step--done",
                icon_css = "mif-dollar2"
            });
        }
        else if (_congno)
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Thanh toán",
                mo_ta = "Đã phát sinh hóa đơn nhưng vẫn còn công nợ cần thu.",
                trang_thai = "Cần thu",
                css = "booking-workflow__step--current",
                icon_css = "mif-wallet"
            });
        }
        else
        {
            list_buoc_vanhanh.Add(new booking_workflow_step_item
            {
                tieu_de = "Thanh toán",
                mo_ta = _co_hoa_don ? "Đã có hóa đơn, chờ ghi nhận thanh toán." : "Chưa phát sinh hóa đơn hoặc thanh toán.",
                trang_thai = _co_hoa_don ? "Chờ thu" : "Chưa mở",
                css = _co_hoa_don ? "booking-workflow__step--current" : "booking-workflow__step--pending",
                icon_css = "mif-coins"
            });
        }

        goi_y_workflow = tongquan_crm != null ? (tongquan_crm.hanh_dong_goi_y ?? "") : "";
        url_goi_y_workflow = tongquan_crm != null ? (tongquan_crm.url_hanh_dong_goi_y ?? "") : "";
    }

    private void nap_tongquan_vanhanh(bspa_datlich_table _lich)
    {
        if (_lich == null)
            return;

        string _sdt = txt_sdt.Text.Trim() != "" ? txt_sdt.Text : _lich.sdt;
        string _tenkh = txt_tenkhachhang.Text.Trim() != "" ? txt_tenkhachhang.Text : _lich.tenkhachhang;
        string _id_dichvu = _lich.dichvu;
        if (ddl_dichvu != null && ddl_dichvu.Items.Count != 0 && string.IsNullOrWhiteSpace(ddl_dichvu.SelectedValue) == false)
            _id_dichvu = ddl_dichvu.SelectedValue;

        tongquan_vanhanh = datlich_vanhanh_class.tai_tongquan(db, _sdt, Session["chinhanh"].ToString(), _id_dichvu);
        tongquan_crm = khachhang_vanhanh_class.tai_tongquan(db, _sdt, Session["chinhanh"].ToString());
        ten_dichvu_hien_tai = datlich_class.return_ten_dichvu(db, _id_dichvu, Session["chinhanh"].ToString());
        list_hoadon_lienket = datlich_lienket_class.lay_ds_hoadon_lienket(db, _lich.id.ToString(), Session["chinhanh"].ToString());
        list_thedv_lienket = datlich_lienket_class.lay_ds_thedv_lienket(db, _lich.id.ToString(), Session["chinhanh"].ToString());

        string _id_nganh = tongquan_vanhanh.id_nganh_goiy;
        string _ten_url = HttpUtility.UrlEncode(_tenkh);
        string _sdt_url = HttpUtility.UrlEncode(datlich_class.chuanhoa_sdt(_sdt));
        string _id_lich_url = HttpUtility.UrlEncode(id);
        string _id_dichvu_url = HttpUtility.UrlEncode(_id_dichvu);
        string _id_nganh_url = HttpUtility.UrlEncode(_id_nganh);

        if (tongquan_vanhanh.co_hoso_khachhang)
            url_ho_so_khach = "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(tongquan_vanhanh.id_khachhang) + "&id_datlich=" + _id_lich_url;

        url_tao_hoa_don = "/gianhang/admin/gianhang/tao-giao-dich.aspx?"
            + "return_url=" + HttpUtility.UrlEncode(Request.RawUrl ?? "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx")
            + "&tenkh=" + _ten_url
            + "&sdt=" + _sdt_url
            + "&idnganh=" + _id_nganh_url
            + "&id_datlich=" + _id_lich_url;

        url_ban_thedv = "/gianhang/admin/quan-ly-the-dich-vu/Default.aspx?q=add"
            + "&tenkh=" + _ten_url
            + "&sdt=" + _sdt_url
            + "&iddv=" + _id_dichvu_url
            + "&idnganh=" + _id_nganh_url
            + "&id_datlich=" + _id_lich_url;

        if (tongquan_vanhanh.co_hoso_khachhang)
        {
            url_sudung_thedv = "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(tongquan_vanhanh.id_khachhang)
                + "&act=thedv&id_datlich=" + _id_lich_url;
        }

        nap_buoc_vanhanh(_lich);
    }

    protected void Button3_Click(object sender, EventArgs e)//thêm
    {
        if (bcorn_class.check_quyen(user, "q10_3") == "")
        {
            datlich_validate_result _kq = datlich_class.chuanhoa_du_lieu(
                txt_tenkhachhang.Text,
                txt_sdt.Text,
                txt_ngaydat.Text,
                ddl_giobatdau.SelectedValue,
                ddl_phutbatdau.SelectedValue,
                ddl_dichvu.SelectedValue,
                ddl_nhanvien.SelectedValue,
                txt_ghichu.Text,
                ddl_trangthai.SelectedValue,
                txt_nguon.Text,
                "Trực tiếp",
                true
            );

            if (_kq.hop_le == false)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", _kq.thongbao, "4000", "warning"), true);
            else
            {
                var q = db.bspa_datlich_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q.Count() != 0)
                {
                    string _loi_vanhanh = datlich_class.kiemtra_quy_tac_van_hanh(db, _kq.dulieu, Session["chinhanh"].ToString(), long.Parse(id), true);
                    if (_loi_vanhanh != "")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", _loi_vanhanh, "4000", "warning"), true);
                        return;
                    }

                    bspa_datlich_table _ob = q.First();
                    string _nhatky = khachhang_nhatky_class.tao_noidung_capnhat_lich(db, _ob, _kq.dulieu, id, Session["chinhanh"].ToString());
                    datlich_class.gan_du_lieu_vao_lich(db, _ob, _kq.dulieu, _ob.nguoitao, Session["chinhanh"].ToString(), true);
                    db.SubmitChanges();

                    if (_nhatky != "")
                    {
                        khachhang_nhatky_class.ghi_su_kien(db, _ob.sdt, Session["chinhanh"].ToString(), user, _nhatky, DateTime.Now);
                        db.SubmitChanges();
                    }

                    reset_ss();

                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                    Response.Redirect("/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx");
                }
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    //autocomplete dịch vụ
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers2(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.bspa_datlich_tables.Where(p => p.nguongoc.Contains(prefixText) && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.nguongoc).Distinct().ToList();
    }
}
