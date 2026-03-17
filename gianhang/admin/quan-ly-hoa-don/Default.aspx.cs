using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
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
    hoadon_class hd_cl = new hoadon_class();
    datetime_class dt_cl = new datetime_class();
    string_class str_cl = new string_class(); nganh_class ng_cl = new nganh_class();
    public string user, user_parent, show_add = "none";
    public Int64 doanhso_hoadon_sauck = 0, doanhso_dichvu = 0, doanhso_sanpham, doanhso_hoadon = 0, tong_congno = 0, tongtien_dathanhtoan = 0,
        dichvu_sauck = 0, sanpham_sauck = 0, sanpham_soluong = 0, dichvu_soluong = 0, hoadon_sl = 0,
        /*tong_tienmat = 0, tong_chuyenkhoan = 0,tong_quetthe = 0,*/ tongrieng_dichvu = 0, tongrieng_sanpham = 0;
    #region phân trang
    public int stt = 1, current_page = 1, show = 30, total_page = 1;
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
        if (bcorn_class.check_quyen(user, "q7_1") == "" || bcorn_class.check_quyen(user, "n7_1") == "")
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
                DropDownList5.DataSource = list_nganh;//lọc
                DropDownList5.DataTextField = "ten";
                DropDownList5.DataValueField = "id";
                DropDownList5.DataBind();
                DropDownList5.Items.Insert(0, new ListItem("Tất cả", ""));
                
                if (bcorn_class.check_quyen(user, "q7_1") == "")
                {
                }
                else
                {
                    DropDownList3.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList3.Enabled = false;
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }
                if (Session["index_loc_nganh_hoadon"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_hoadon"].ToString());
                else
                    Session["index_loc_nganh_hoadon"] = DropDownList5.SelectedIndex.ToString();

                var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                     select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                          );
                ddl_nhanvien_chamsoc.DataSource = list_nhanvien;
                ddl_nhanvien_chamsoc.DataTextField = "tennhanvien";
                ddl_nhanvien_chamsoc.DataValueField = "username";
                ddl_nhanvien_chamsoc.DataBind();
                ddl_nhanvien_chamsoc.Items.Insert(0, new ListItem("Chọn", ""));

                var list_nohmkhachhang = (from ob1 in db.nhomkhachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                          select new { id = ob1.id, tennhom = ob1.tennhom, }
                          );
                DropDownList1.DataSource = list_nohmkhachhang;
                DropDownList1.DataTextField = "tennhom";
                DropDownList1.DataValueField = "id";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));

                var q = db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());
                txt_ngaytao.Text = DateTime.Now.Date.ToString();

                if (Session["current_page_hoadon"] == null)//lưu giữ trang hiện tại
                    Session["current_page_hoadon"] = "1";

                if (Session["index_sapxep_hoadon"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                    Session["index_sapxep_hoadon"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_hoadon"].ToString());

                if (Session["search_hoadon"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_hoadon"].ToString();
                else
                    Session["search_hoadon"] = txt_search.Text;

                if (Session["show_hoadon"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_hoadon"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_hoadon"].ToString();

                if (Session["tungay_hoadon"] == null)
                {
                    txt_tungay.Text = "01/01/2023";
                    Session["tungay_hoadon"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_hoadon"].ToString();

                if (Session["denngay_hoadon"] == null)
                {
                    txt_denngay.Text = DateTime.Now.ToShortDateString();
                    Session["denngay_hoadon"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_hoadon"].ToString();

                if (Session["index_loc_thanhtoan_hoadon"] != null)//lưu lọc theo theo toán
                    ddl_locdulieu.SelectedIndex = int.Parse(Session["index_loc_thanhtoan_hoadon"].ToString());
                else
                    Session["index_loc_thanhtoan_hoadon"] = ddl_locdulieu.SelectedIndex.ToString();
                if (Session["index_loc_dvsp_hoadon"] != null)//lưu lọc theo dịch vụ-sản phẩm
                    ddl_loc2.SelectedIndex = int.Parse(Session["index_loc_dvsp_hoadon"].ToString());
                else
                    Session["index_loc_dvsp_hoadon"] = ddl_loc2.SelectedIndex.ToString();

                ap_dung_prefill_form_tao_hoa_don();
            }
            main();

            if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]))//khi ng dùng nhấn vào nút tạo hóa đơn từ menutop --> tạo nhanh
            {
                string _q = Request.QueryString["q"].ToString().Trim();
                if (_q == "add")
                    show_add = "block";
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion


    }

    private void ap_dung_prefill_form_tao_hoa_don()
    {
        string _sdt = datlich_class.chuanhoa_sdt(Request.QueryString["sdt"]);
        string _tenkh = (Request.QueryString["tenkh"] ?? "").Trim();
        string _id_nganh = (Request.QueryString["idnganh"] ?? "").Trim();
        string _id_datlich = (Request.QueryString["id_datlich"] ?? "").Trim();

        if (_sdt != "")
        {
            txt_sdt.Text = _sdt;
            var q_khach = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao);
            if (q_khach.Count() != 0)
                ap_dung_khachhang_vao_form_tao_hoa_don(q_khach.First());
        }

        if (_tenkh != "" && txt_tenkhachhang.Text.Trim() == "")
            txt_tenkhachhang.Text = _tenkh;

        if (_id_nganh != "")
            datlich_class.try_select_dropdown_value(DropDownList3, _id_nganh);

        if (_id_datlich != "" && txt_ghichu.Text.Trim() == "")
        {
            var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id_datlich && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q_lich.Count() != 0)
            {
                bspa_datlich_table _lich = q_lich.First();
                txt_ghichu.Text = datlich_lienket_class.dam_bao_ghi_chu_datlich(txt_ghichu.Text, _id_datlich, "Tạo từ", _lich.tendichvu_taithoidiemnay);
            }
        }
    }

    private void ap_dung_khachhang_vao_form_tao_hoa_don(bspa_data_khachhang_table _kh)
    {
        if (_kh == null)
            return;

        if (txt_tenkhachhang.Text.Trim() == "")
            txt_tenkhachhang.Text = _kh.tenkhachhang;
        if (txt_diachi.Text.Trim() == "")
            txt_diachi.Text = _kh.diachi;

        if (_kh.nguoichamsoc != "")
            datlich_class.try_select_dropdown_value(ddl_nhanvien_chamsoc, _kh.nguoichamsoc);
        if (_kh.nhomkhachhang != "")
            datlich_class.try_select_dropdown_value(DropDownList1, _kh.nhomkhachhang);
    }

    public void main()
    {
        //lấy dữ liệu_old
        //var list_all = (from ob1 in db.bspa_hoadon_tables.Where(p => p.user_parent == user_parent && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_hoadon"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_hoadon"].ToString()).Date).ToList()
        //                    //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
        //                select new
        //                {
        //                    id = ob1.id,
        //                    ngaytao = ob1.ngaytao,
        //                    tenkhachhang = ob1.tenkhachhang,
        //                    sdt = ob1.sdt,
        //                    ck_hoadon = ob1.chietkhau,
        //                    tongtien_ck = ob1.tongtien_ck_hoadon,
        //                    tongtien = ob1.tongtien,
        //                    tongsauchietkhau = ob1.tongsauchietkhau,
        //                    sotien_dathanhtoan = ob1.sotien_dathanhtoan,
        //                    sotien_conlai = ob1.sotien_conlai,
        //                    tienmat = ob1.thanhtoan_tienmat,
        //                    chuyenkhoan = ob1.thanhtoan_chuyenkhoan,
        //                    quetthe = ob1.thanhtoan_quetthe,
        //                    phanloai_hoadon = ob1.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
        //                    sl_dv = ob1.sl_dichvu,
        //                    sl_sp = ob1.sl_sanpham,
        //                    ds_dv = ob1.ds_dichvu,
        //                    ds_sp = ob1.ds_sanpham,
        //                    sauck_dv = ob1.sauck_dichvu,
        //                    sauck_sp = ob1.sauck_sanpham,
        //                });

        //lấy dữ liệu
        var list_all = (from ob1 in db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_hoadon"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_hoadon"].ToString()).Date).ToList()
                        join ob2 in db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id.ToString() equals ob2.id_hoadon
                        //join ob3 in db.bspa_lichsu_thanhtoan_tables.ToList() on ob1.id.ToString() equals ob3.id_hoadon  --> k đc, k có thanh toán là k hiện đơn
                        group ob2 by new
                        {
                            ob1.id,
                            ob1.id_guide,
                            ob1.ngaytao,
                            ob1.tenkhachhang,
                            ob1.sdt,
                            ob1.chietkhau,
                            ob1.tongtien_ck_hoadon,
                            ob1.tongtien,
                            ob1.tongsauchietkhau,
                            ob1.sotien_dathanhtoan,
                            ob1.sotien_conlai,
                            //ob1.thanhtoan_tienmat,
                            //ob1.thanhtoan_chuyenkhoan,
                            //ob1.thanhtoan_quetthe,
                            ob1.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
                            ob1.sl_dichvu,
                            ob1.sl_sanpham,
                            ob1.ds_dichvu,
                            ob1.ds_sanpham,
                            ob1.sauck_dichvu,
                            ob1.sauck_sanpham,
                            ob1.nguongoc,
                            ob1.id_nganh,
                        } into g
                        select new
                        {
                            id = g.Key.id,
                            id_guide = g.Key.id_guide,
                            ngaytao = g.Key.ngaytao,
                            tenkhachhang = g.Key.tenkhachhang,
                            sdt = g.Key.sdt,
                            ck_hoadon = g.Key.chietkhau,
                            tongtien_ck = g.Key.tongtien_ck_hoadon,
                            tongtien = g.Key.tongtien,
                            tongsauchietkhau = g.Key.tongsauchietkhau,
                            sotien_dathanhtoan = g.Key.sotien_dathanhtoan,
                            sotien_conlai = g.Key.sotien_conlai,
                            //tienmat = g.Key.thanhtoan_tienmat,
                            //chuyenkhoan = g.Key.thanhtoan_chuyenkhoan,
                            //quetthe = g.Key.thanhtoan_quetthe,
                            phanloai_hoadon = g.Key.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
                            sl_dv = g.Key.sl_dichvu,
                            sl_sp = g.Key.sl_sanpham,
                            ds_dv = g.Key.ds_dichvu,
                            ds_sp = g.Key.ds_sanpham,
                            sauck_dv = g.Key.sauck_dichvu,
                            sauck_sp = g.Key.sauck_sanpham,
                            tongtien_dichvu = g.Where(p => p.kyhieu == "dichvu").Sum(ob2 => ob2.tongsauchietkhau).Value,
                            tongtien_sanpham = g.Where(p => p.kyhieu == "sanpham").Sum(ob2 => ob2.tongsauchietkhau).Value,
                            nguongoc = g.Key.nguongoc,
                            id_nganh = g.Key.id_nganh,
                            tennganh=ng_cl.return_name(g.Key.id_nganh),
                        });
        //list_new: là các đơn vừa tạo xong, chưa có chi tiết nên nó k hiện đơn ở ql hóa đơn
        //var list_new = (from ob1 in db.bspa_hoadon_tables.Where(p => p.user_parent == user_parent && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_hoadon"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_hoadon"].ToString()).Date).ToList()

        //                select new
        //                {
        //                    id = ob1.id,
        //                    id_guide = ob1.id_guide,
        //                    ngaytao = ob1.ngaytao,
        //                    tenkhachhang = ob1.tenkhachhang,
        //                    sdt = ob1.sdt,
        //                    ck_hoadon = ob1.chietkhau,
        //                    tongtien_ck = ob1.tongtien_ck_hoadon,
        //                    tongtien = ob1.tongtien,
        //                    tongsauchietkhau = ob1.tongsauchietkhau,
        //                    sotien_dathanhtoan = ob1.sotien_dathanhtoan,
        //                    sotien_conlai = ob1.sotien_conlai,
        //                    tienmat = ob1.thanhtoan_tienmat,
        //                    chuyenkhoan = ob1.thanhtoan_chuyenkhoan,
        //                    quetthe = ob1.thanhtoan_quetthe,
        //                    phanloai_hoadon = ob1.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
        //                    sl_dv = ob1.sl_dichvu,
        //                    sl_sp = ob1.sl_sanpham,
        //                    ds_dv = ob1.ds_dichvu,
        //                    ds_sp = ob1.ds_sanpham,
        //                    sauck_dv = ob1.sauck_dichvu,
        //                    sauck_sp = ob1.sauck_sanpham,
        //                    tongtien_dichvu = Int64.Parse("0"),
        //                    tongtien_sanpham = Int64.Parse("0"),
        //                });
        //list_all = list_all.Union(list_new);

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key) || p.sdt == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //xử lý lọc dữ liệu
        if (ddl_locdulieu.SelectedValue.ToString() != "0")
        {
            switch (ddl_locdulieu.SelectedValue.ToString())
            {
                case ("1"): var list_1 = list_all.Where(p => p.sotien_conlai != 0).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                case ("2"): var list_2 = list_all.Where(p => p.sotien_conlai == 0).ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                default: break;
            }
        }
        if (ddl_loc2.SelectedValue.ToString() != "0")
        {
            switch (ddl_loc2.SelectedValue.ToString())
            {
                case ("1"): var list_1 = list_all.Where(p => p.phanloai_hoadon == "dichvu").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                case ("2"): var list_2 = list_all.Where(p => p.phanloai_hoadon == "dichvu" || p.phanloai_hoadon == "dichvusanpham").ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                case ("3"): var list_3 = list_all.Where(p => p.phanloai_hoadon == "sanpham").ToList(); list_all = list_all.Intersect(list_3).ToList(); break;
                case ("4"): var list_4 = list_all.Where(p => p.phanloai_hoadon == "sanpham" || p.phanloai_hoadon == "dichvusanpham").ToList(); list_all = list_all.Intersect(list_4).ToList(); break;
                default: break;
            }
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        //sắp xếp
        switch (Session["index_sapxep_hoadon"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
        }

        //TÍNH HÓA ĐƠN
        hoadon_sl = list_all.Count();
        doanhso_hoadon = list_all.Sum(p => p.tongtien).Value;
        doanhso_hoadon_sauck = list_all.Sum(p => p.tongsauchietkhau).Value;
        dichvu_soluong = list_all.Sum(p => p.sl_dv).Value;
        sanpham_soluong = list_all.Sum(p => p.sl_sp).Value;
        tongtien_dathanhtoan = list_all.Sum(p => p.sotien_dathanhtoan).Value;
        //tong_tienmat = list_all.Sum(p => p.tienmat).Value;
        //tong_chuyenkhoan = list_all.Sum(p => p.chuyenkhoan).Value;
        //tong_quetthe = list_all.Sum(p => p.quetthe).Value;
        tong_congno = list_all.Sum(p => p.sotien_conlai).Value;
        doanhso_dichvu = list_all.Sum(p => p.ds_dv).Value;
        doanhso_sanpham = list_all.Sum(p => p.ds_sp).Value;
        dichvu_sauck = list_all.Sum(p => p.sauck_dv).Value;
        sanpham_sauck = list_all.Sum(p => p.sauck_sp).Value;
        tongrieng_dichvu = list_all.Sum(p => p.tongtien_dichvu);
        tongrieng_sanpham = list_all.Sum(p => p.tongtien_sanpham);

        //foreach (var t in list_all)
        //{
        //    var q_dichvu = db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_hoadon == t.id.ToString() && p.kyhieu == "dichvu");
        //    if (q_dichvu.Count() != 0)
        //    {
        //        doanhso_dichvu = doanhso_dichvu + q_dichvu.Sum(p => p.thanhtien).Value;
        //        dichvu_sauck = dichvu_sauck + q_dichvu.Sum(p => p.tongsauchietkhau).Value;
        //    }
        //    var q_sanpham = db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_hoadon == t.id.ToString() && p.kyhieu == "sanpham");
        //    if (q_sanpham.Count() != 0)
        //    {
        //        doanhso_sanpham = doanhso_sanpham + q_sanpham.Sum(p => p.thanhtien).Value;
        //        sanpham_sauck = sanpham_sauck + q_sanpham.Sum(p => p.tongsauchietkhau).Value;
        //    }
        //}




        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_hoadon"].ToString());
        if (current_page > total_page)
            current_page = total_page;
        if (current_page >= total_page)
            but_xemtiep.Enabled = false;
        else
            but_xemtiep.Enabled = true;
        if (current_page == 1)
            but_quaylai.Enabled = false;
        else
            but_quaylai.Enabled = true;

        //main
        stt = (show * current_page) - show + 1;
        var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        Session["search_hoadon"] = txt_search.Text.Trim();
        Session["current_page_hoadon"] = "1";
        main();

    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_hoadon"] = int.Parse(Session["current_page_hoadon"].ToString()) - 1;
        if (int.Parse(Session["current_page_hoadon"].ToString()) < 1)
            Session["current_page_hoadon"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_hoadon"] = int.Parse(Session["current_page_hoadon"].ToString()) + 1;
        if (int.Parse(Session["current_page_hoadon"].ToString()) > total_page)
            Session["current_page_hoadon"] = total_page;
        main();
    }


    protected void Button3_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_2") == "" || bcorn_class.check_quyen(user, "n7_2") == "")
        {
            string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
            string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
            string _ghichu = txt_ghichu.Text;
            string _diachi = txt_diachi.Text;
            string _ngaytao = txt_ngaytao.Text;
            string _nganh = DropDownList3.SelectedValue.ToString();
            string _id_datlich = (Request.QueryString["id_datlich"] ?? "").Trim();

            //string _ck = txt_chietkhau_hoadon.Text.Trim().Replace(".", "");
            //int _r2 = 0;
            //int.TryParse(_ck, out _r2);//nếu là số nguyên thì gán cho _r
            if (_nganh == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng chọn ngành.", "4000", "warning"), true);
            else
            {
                if (dt_cl.check_date(_ngaytao) == false)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày tạo không hợp lệ", "4000", "warning"), true);
                else
                {
                    if (_tenkhachhang == "")
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên khách hàng.", "4000", "warning"), true);
                    else
                    {
                        //if ((_r2 < 0 && ck_hd_phantram.Checked == true) || (_r2 > 100 && ck_hd_phantram.Checked == true))//nếu chọn % thì k đc <0 & >100 
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu hóa đơn không hợp lệ.", "4000", "warning"), true);
                        //else
                        //{
                        bspa_hoadon_table _ob = new bspa_hoadon_table();
                        _ob.id_guide = Guid.NewGuid();
                        _ob.ngaytao = DateTime.Parse(_ngaytao + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                        _ob.tongtien = 0;
                        _ob.chietkhau = 0;
                        _ob.tongsauchietkhau = 0;
                        _ob.tenkhachhang = _tenkhachhang;
                        if (_id_datlich != "")
                        {
                            var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id_datlich && p.id_chinhanh == Session["chinhanh"].ToString());
                            if (q_lich.Count() != 0)
                                _ghichu = datlich_lienket_class.dam_bao_ghi_chu_datlich(_ghichu, _id_datlich, "Tạo từ", q_lich.First().tendichvu_taithoidiemnay);
                        }
                        _ob.ghichu = _ghichu;
                        _ob.sdt = _sdt;
                        _ob.diachi = _diachi;
                        _ob.sotien_dathanhtoan = 0;
                        _ob.thanhtoan_tienmat = 0; _ob.thanhtoan_chuyenkhoan = 0; _ob.thanhtoan_quetthe = 0;
                        _ob.sotien_conlai = 0;
                        _ob.sl_dichvu = 0; _ob.sl_sanpham = 0;
                        _ob.album = "";
                        _ob.user_parent = user_parent;
                        _ob.dichvu_hay_sanpham = "";
                        _ob.ds_dichvu = 0; _ob.ds_sanpham = 0; _ob.sauck_dichvu = 0; _ob.sauck_sanpham = 0;
                        _ob.tongtien_ck_hoadon = 0;
                        _ob.km1_ghichu = "";
                        _ob.nguoitao = Session["user"].ToString();
                        _ob.nguongoc = "App";
                        _ob.id_nganh = _nganh;
                        //if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
                        //{
                        //    _ob.chietkhau = _r2;
                        //    _ob.tongsauchietkhau = _ob.tongtien - (_ob.tongtien * _r2 / 100);
                        //    _ob.tongtien_ck_hoadon = _ob.tongtien - _ob.tongsauchietkhau;
                        //}
                        //else
                        //{
                        //    _ob.chietkhau = 0;
                        //    _ob.tongsauchietkhau = _ob.tongtien - _r2;
                        //    _ob.tongtien_ck_hoadon = _r2;
                        //}
                        _ob.id_chinhanh = Session["chinhanh"].ToString();
                        db.bspa_hoadon_tables.InsertOnSubmit(_ob);
                        db.SubmitChanges();
                        string _id_hoadon_moi = _ob.id.ToString();

                        if (_id_datlich != "")
                        {
                            datlich_lienket_class.dong_bo_vao_lich_hen(
                                db,
                                _id_datlich,
                                Session["chinhanh"].ToString(),
                                _id_hoadon_moi,
                                "",
                                "",
                                _ob.ngaytao,
                                "Đã tạo hóa đơn #" + _id_hoadon_moi,
                                false);
                            db.SubmitChanges();
                        }

                        var q_data = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString());
                        if (q_data.Count() == 0 && _sdt != "")//chưa có sdt thì thêm vào
                        {
                            bspa_data_khachhang_table _ob1 = new bspa_data_khachhang_table();
                            _ob1.tenkhachhang = _tenkhachhang;
                            _ob1.diachi = _diachi;
                            _ob1.magioithieu = "";
                            _ob1.ngaytao = DateTime.Now;
                            _ob1.nguoitao = user;
                            _ob1.nguoichamsoc = ddl_nhanvien_chamsoc.SelectedValue.ToString();
                            _ob1.sdt = _sdt;
                            _ob1.user_parent = user_parent;
                            _ob1.anhdaidien = "/uploads/images/macdinh.jpg";
                            _ob1.matkhau = "F5F65B64F300D3764E16E57663F3072F";//12345678
                            _ob1.nhomkhachhang = DropDownList1.SelectedValue.ToString();
                            //if (txt_ngaysinh.Text != "" && dt_cl.check_date(txt_ngaysinh.Text) == true)
                            //    _ob1.ngaysinh = DateTime.Parse(txt_ngaysinh.Text);
                            _ob1.id_chinhanh = Session["chinhanh"].ToString();
                            _ob1.capbac = ""; _ob1.vnd_tu_e_aha = 0; _ob1.sodiem_e_aha = 0; _ob1.solan_lencap = 0;
                            db.bspa_data_khachhang_tables.InsertOnSubmit(_ob1);

                            db.SubmitChanges();
                        }


                        //reset hết để bên trang quản lý nó hiển thị, chứ k là có lúc nó k hiện ra bài mới thêm,  tức ghê, kb do thằng nào
                        reset_ss();


                        //main();
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm sản phẩm thành công.", "4000", "warning"), true);
                        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Tạo hóa đơn thành công.", "2000", "warning");
                        Response.Redirect("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + _id_hoadon_moi + (_id_datlich != "" ? "&id_datlich=" + HttpUtility.UrlEncode(_id_datlich) : ""));
                        //}
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


    public void reset_ss()
    {
        Session["index_sapxep_hoadon"] = null;
        Session["current_page_hoadon"] = null;
        Session["search_hoadon"] = null;
        Session["show_hoadon"] = null;
        Session["tungay_hoadon"] = null;
        Session["denngay_hoadon"] = null;
        Session["index_loc_thanhtoan_hoadon"] = null;
        Session["index_loc_dvsp_hoadon"] = null;

        Session["index_loc_nganh_hoadon"] = null;
    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-hoa-don/Default.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_hoadon"] = DropDownList2.SelectedIndex;
        Session["current_page_hoadon"] = "1";
        Session["search_hoadon"] = txt_search.Text.Trim();
        Session["show_hoadon"] = txt_show.Text.Trim();

        Session["tungay_hoadon"] = txt_tungay.Text;
        Session["denngay_hoadon"] = txt_denngay.Text;

        Session["index_loc_thanhtoan_hoadon"] = ddl_locdulieu.SelectedIndex;
        Session["index_loc_dvsp_hoadon"] = ddl_loc2.SelectedIndex;

        Session["index_loc_nganh_hoadon"] = DropDownList5.SelectedIndex.ToString();

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_4") == ""|| bcorn_class.check_quyen(user, "n7_4") == "")
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (hd_cl.exist_id(_id, user_parent))
                    {
                        hd_cl.del(_id);
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }




    protected void Button5_Click(object sender, EventArgs e)
    {
        #region lấy dữ liệu
        var list_all = (from ob1 in db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_hoadon"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_hoadon"].ToString()).Date).ToList()
                        join ob2 in db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id.ToString() equals ob2.id_hoadon
                        //join ob3 in db.bspa_lichsu_thanhtoan_tables.ToList() on ob1.id.ToString() equals ob3.id_hoadon  --> k đc, k có thanh toán là k hiện đơn
                        group ob2 by new
                        {
                            ob1.id,
                            ob1.id_guide,
                            ob1.ngaytao,
                            ob1.tenkhachhang,
                            ob1.sdt,
                            ob1.chietkhau,
                            ob1.tongtien_ck_hoadon,
                            ob1.tongtien,
                            ob1.tongsauchietkhau,
                            ob1.sotien_dathanhtoan,
                            ob1.sotien_conlai,
                            //ob1.thanhtoan_tienmat,
                            //ob1.thanhtoan_chuyenkhoan,
                            //ob1.thanhtoan_quetthe,
                            ob1.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
                            ob1.sl_dichvu,
                            ob1.sl_sanpham,
                            ob1.ds_dichvu,
                            ob1.ds_sanpham,
                            ob1.sauck_dichvu,
                            ob1.sauck_sanpham,
                            ob1.nguongoc,
                            ob1.id_nganh,
                        } into g
                        select new
                        {
                            id = g.Key.id,
                            id_guide = g.Key.id_guide,
                            ngaytao = g.Key.ngaytao,
                            tenkhachhang = g.Key.tenkhachhang,
                            sdt = g.Key.sdt,
                            ck_hoadon = g.Key.chietkhau,
                            tongtien_ck = g.Key.tongtien_ck_hoadon,
                            tongtien = g.Key.tongtien,
                            tongsauchietkhau = g.Key.tongsauchietkhau,
                            sotien_dathanhtoan = g.Key.sotien_dathanhtoan,
                            sotien_conlai = g.Key.sotien_conlai,
                            //tienmat = g.Key.thanhtoan_tienmat,
                            //chuyenkhoan = g.Key.thanhtoan_chuyenkhoan,
                            //quetthe = g.Key.thanhtoan_quetthe,
                            phanloai_hoadon = g.Key.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
                            sl_dv = g.Key.sl_dichvu,
                            sl_sp = g.Key.sl_sanpham,
                            ds_dv = g.Key.ds_dichvu,
                            ds_sp = g.Key.ds_sanpham,
                            sauck_dv = g.Key.sauck_dichvu,
                            sauck_sp = g.Key.sauck_sanpham,
                            tongtien_dichvu = g.Where(p => p.kyhieu == "dichvu").Sum(ob2 => ob2.tongsauchietkhau).Value,
                            tongtien_sanpham = g.Where(p => p.kyhieu == "sanpham").Sum(ob2 => ob2.tongsauchietkhau).Value,
                            nguongoc = g.Key.nguongoc,
                            id_nganh = g.Key.id_nganh,
                        });

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key) || p.sdt == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //xử lý lọc dữ liệu
        if (ddl_locdulieu.SelectedValue.ToString() != "0")
        {
            switch (ddl_locdulieu.SelectedValue.ToString())
            {
                case ("1"): var list_1 = list_all.Where(p => p.sotien_conlai != 0).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                case ("2"): var list_2 = list_all.Where(p => p.sotien_conlai == 0).ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                default: break;
            }
        }
        if (ddl_loc2.SelectedValue.ToString() != "0")
        {
            switch (ddl_loc2.SelectedValue.ToString())
            {
                case ("1"): var list_1 = list_all.Where(p => p.phanloai_hoadon == "dichvu").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                case ("2"): var list_2 = list_all.Where(p => p.phanloai_hoadon == "dichvu" || p.phanloai_hoadon == "dichvusanpham").ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                case ("3"): var list_3 = list_all.Where(p => p.phanloai_hoadon == "sanpham").ToList(); list_all = list_all.Intersect(list_3).ToList(); break;
                case ("4"): var list_4 = list_all.Where(p => p.phanloai_hoadon == "sanpham" || p.phanloai_hoadon == "dichvusanpham").ToList(); list_all = list_all.Intersect(list_4).ToList(); break;
                default: break;
            }
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }
        //sắp xếp
        switch (Session["index_sapxep_hoadon"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
        }

        //TÍNH HÓA ĐƠN
        hoadon_sl = list_all.Count();
        doanhso_hoadon = list_all.Sum(p => p.tongtien).Value;
        doanhso_hoadon_sauck = list_all.Sum(p => p.tongsauchietkhau).Value;
        dichvu_soluong = list_all.Sum(p => p.sl_dv).Value;
        sanpham_soluong = list_all.Sum(p => p.sl_sp).Value;
        tongtien_dathanhtoan = list_all.Sum(p => p.sotien_dathanhtoan).Value;
        //tong_tienmat = list_all.Sum(p => p.tienmat).Value;
        //tong_chuyenkhoan = list_all.Sum(p => p.chuyenkhoan).Value;
        //tong_quetthe = list_all.Sum(p => p.quetthe).Value;
        tong_congno = list_all.Sum(p => p.sotien_conlai).Value;
        doanhso_dichvu = list_all.Sum(p => p.ds_dv).Value;
        doanhso_sanpham = list_all.Sum(p => p.ds_sp).Value;
        dichvu_sauck = list_all.Sum(p => p.sauck_dv).Value;
        sanpham_sauck = list_all.Sum(p => p.sauck_sp).Value;
        tongrieng_dichvu = list_all.Sum(p => p.tongtien_dichvu);
        tongrieng_sanpham = list_all.Sum(p => p.tongtien_sanpham);
        #endregion

        if (check_list_excel.Items.Count == 0)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
        else
        {
            // khởi tạo wb rỗng
            XSSFWorkbook wb = new XSSFWorkbook();

            // Tạo ra 1 sheet
            ISheet sheet = wb.CreateSheet();

            // Bắt đầu ghi lên sheet

            // Tạo row
            var row0 = sheet.CreateRow(0);
            // Merge lại row đầu 3 cột
            row0.CreateCell(0); // tạo ra cell trc khi merge
            CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 2);
            sheet.AddMergedRegion(cellMerge);
            row0.GetCell(0).SetCellValue("Dữ liệu của bạn xuất ngày " + DateTime.Now);

            //đếm xem có bao nhiêu cột đc chọn
            // Ghi tiêu đề cột ở row 1
            var row1 = sheet.CreateRow(1);
            int _socot = 0;
            for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
            {
                if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                {
                    //thì tạo cột tiêu đề
                    row1.CreateCell(_socot).SetCellValue(check_list_excel.Items[i].Text);
                    _socot = _socot + 1;
                }
            }

            // bắt đầu duyệt mảng và ghi tiếp tục
            int rowIndex = 2;
            foreach (var item in list_all)
            {
                // tao row mới
                var newRow = sheet.CreateRow(rowIndex);

                // set giá trị
                int _socot1 = 0;
                for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
                {
                    if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                    {
                        string _tencot = check_list_excel.Items[i].Value;
                        switch (_tencot)
                        {
                            //case "ngaysinh":
                            //    if (item.ngaysinh != null)
                            //    {
                            //        newRow.CreateCell(_socot1).SetCellValue(item.ngaysinh.Value.ToString("dd/MM/yyyy"));
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        newRow.CreateCell(_socot1).SetCellValue("");
                            //        break;
                            //    }
                            case "id": newRow.CreateCell(_socot1).SetCellValue(item.id); break;
                            case "ngaytao": newRow.CreateCell(_socot1).SetCellValue(item.ngaytao.Value.ToString("dd/MM/yyyy")); break;
                            case "nguongoc": newRow.CreateCell(_socot1).SetCellValue(item.nguongoc); break;
                            case "tenkhachhang": newRow.CreateCell(_socot1).SetCellValue(item.tenkhachhang); break;
                            case "sdt": newRow.CreateCell(_socot1).SetCellValue(item.sdt); break;

                            case "sl_dv": newRow.CreateCell(_socot1).SetCellValue(item.sl_dv.Value.ToString()); break;
                            case "tongtien_dichvu": newRow.CreateCell(_socot1).SetCellValue(item.tongtien_dichvu); break;
                            case "sl_sp": newRow.CreateCell(_socot1).SetCellValue(item.sl_sp.Value.ToString()); break;
                            case "tongtien_sanpham": newRow.CreateCell(_socot1).SetCellValue(item.tongtien_sanpham); break;
                            case "tongtien": newRow.CreateCell(_socot1).SetCellValue(item.tongtien.Value); break;
                            case "ck_hoadon": newRow.CreateCell(_socot1).SetCellValue(item.ck_hoadon.Value); break;
                            case "tongsauchietkhau": newRow.CreateCell(_socot1).SetCellValue(item.tongsauchietkhau.Value); break;
                            case "sotien_dathanhtoan": newRow.CreateCell(_socot1).SetCellValue(item.sotien_dathanhtoan.Value); break;
                            case "sotien_conlai": newRow.CreateCell(_socot1).SetCellValue(item.sotien_conlai.Value); break;
                            default: break;
                        }
                        _socot1 = _socot1 + 1;
                    }
                }

                // tăng index
                rowIndex++;
            }

            //tính tổng
            // Ghi tiêu đề cột ở row 1
            var row3 = sheet.CreateRow(rowIndex);//tạo dòng tiếp theo
            int _socot3 = 0;
            for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
            {
                if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                {
                    //thì ghi dữ liệu
                    string _tencot = check_list_excel.Items[i].Value;
                    switch (_tencot)
                    {
                        case "tongtien": row3.CreateCell(_socot3).SetCellValue(doanhso_hoadon); break;
                        case "tongsauchietkhau": row3.CreateCell(_socot3).SetCellValue(doanhso_hoadon_sauck); break;
                        case "sotien_dathanhtoan": row3.CreateCell(_socot3).SetCellValue(tongtien_dathanhtoan); break;
                        case "sotien_conlai": row3.CreateCell(_socot3).SetCellValue(tong_congno); break;
                        case "tongtien_dichvu": row3.CreateCell(_socot3).SetCellValue(tongrieng_dichvu); break;
                        case "tongtien_sanpham": row3.CreateCell(_socot3).SetCellValue(tongrieng_sanpham); break;
                        default: break;
                    }
                    _socot3 = _socot3 + 1;
                }
            }

            // xong hết thì save file lại
            string _filename = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filename + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filename + ".xlsx");

        }
    }
}
