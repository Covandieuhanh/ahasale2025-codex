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
    nganh_class ng_cl = new nganh_class();
    public string user, user_parent, show_add = "none";
    public Int64 doanhso_hoadon_sauck = 0, doanhso_hoadon = 0, tong_congno = 0, tongtien_dathanhtoan = 0, hoadon_sl = 0;
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
        if (bcorn_class.check_quyen(user, "q12_1") == "" || bcorn_class.check_quyen(user, "n12_1") == "")
        {
            if (!IsPostBack)
            {
                var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                  select new { id = ob1.id, ten = ob1.ten, }
                                   );
                DropDownList5.DataSource = list_nganh;
                DropDownList5.DataTextField = "ten";
                DropDownList5.DataValueField = "id";
                DropDownList5.DataBind();
                DropDownList5.Items.Insert(0, new ListItem("Tất cả", ""));
                DropDownList3.DataSource = list_nganh;
                DropDownList3.DataTextField = "ten";
                DropDownList3.DataValueField = "id";
                DropDownList3.DataBind();
                DropDownList3.Items.Insert(0, new ListItem("Chọn", ""));

                if (bcorn_class.check_quyen(user, "q12_1") == "")
                {
                    var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                         select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                          );
                    ddl_nhanvien_chotsale.DataSource = list_nhanvien;
                    ddl_nhanvien_chotsale.DataTextField = "tennhanvien";
                    ddl_nhanvien_chotsale.DataValueField = "username";
                    ddl_nhanvien_chotsale.DataBind();
                    ddl_nhanvien_chotsale.Items.Insert(0, new ListItem("Chọn", ""));

                    var list_dv = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                   select new { id = ob1.id, tendichvu = ob1.name, }
                              );
                    DropDownList1.DataSource = list_dv;
                    DropDownList1.DataTextField = "tendichvu";
                    DropDownList1.DataValueField = "id";
                    DropDownList1.DataBind();
                    DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                }
                else
                {
                    var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_nganh == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                         select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                          );
                    ddl_nhanvien_chotsale.DataSource = list_nhanvien;
                    ddl_nhanvien_chotsale.DataTextField = "tennhanvien";
                    ddl_nhanvien_chotsale.DataValueField = "username";
                    ddl_nhanvien_chotsale.DataBind();
                    ddl_nhanvien_chotsale.Items.Insert(0, new ListItem("Chọn", ""));

                    var list_dv = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_nganh == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                   select new { id = ob1.id, tendichvu = ob1.name, }
                              );
                    DropDownList1.DataSource = list_dv;
                    DropDownList1.DataTextField = "tendichvu";
                    DropDownList1.DataValueField = "id";
                    DropDownList1.DataBind();
                    DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));

                    //show ngành khi lọc
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;

                    //show ngành khi thêm
                    DropDownList3.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList3.Enabled = false;
                }

                txt_ngaytao.Text = DateTime.Now.Date.ToString();
                txt_hsd.Text = DateTime.Now.AddYears(1).Date.ToString();

                if (Session["current_page_thedichvu"] == null)//lưu giữ trang hiện tại
                    Session["current_page_thedichvu"] = "1";

                if (Session["index_sapxep_thedichvu"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                    Session["index_sapxep_thedichvu"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_thedichvu"].ToString());

                if (Session["search_thedichvu"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_thedichvu"].ToString();
                else
                    Session["search_thedichvu"] = txt_search.Text;

                if (Session["show_thedichvu"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_thedichvu"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_thedichvu"].ToString();

                if (Session["tungay_thedichvu"] == null)
                {
                    txt_tungay.Text = "01/01/2023";
                    Session["tungay_thedichvu"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_thedichvu"].ToString();

                if (Session["denngay_thedichvu"] == null)
                {
                    txt_denngay.Text = DateTime.Now.ToShortDateString();
                    Session["denngay_thedichvu"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_thedichvu"].ToString();

                if (Session["index_loc_hsd_thedichvu"] != null)//lưu lọc hsd
                    ddl_loc_hsd.SelectedIndex = int.Parse(Session["index_loc_hsd_thedichvu"].ToString());
                else
                    Session["index_loc_hsd_thedichvu"] = ddl_loc_hsd.SelectedIndex.ToString();

                if (Session["index_loc_nganh_thedichvu"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_thedichvu"].ToString());
                else
                    Session["index_loc_nganh_thedichvu"] = DropDownList5.SelectedIndex.ToString();

                load_nganh();
                ap_dung_prefill_form_ban_thedv();
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

    private void ap_dung_prefill_form_ban_thedv()
    {
        string _sdt = datlich_class.chuanhoa_sdt(Request.QueryString["sdt"]);
        string _tenkh = (Request.QueryString["tenkh"] ?? "").Trim();
        string _id_nganh = (Request.QueryString["idnganh"] ?? "").Trim();
        string _id_dichvu = (Request.QueryString["iddv"] ?? "").Trim();
        string _id_datlich = (Request.QueryString["id_datlich"] ?? "").Trim();

        if (_sdt != "")
        {
            txt_sdt.Text = _sdt;
            var q_khach = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao);
            if (q_khach.Count() != 0)
                ap_dung_khachhang_vao_form_ban_thedv(q_khach.First());
        }

        if (_tenkh != "" && txt_tenkhachhang.Text.Trim() == "")
            txt_tenkhachhang.Text = _tenkh;

        if (_id_nganh != "")
            datlich_class.try_select_dropdown_value(DropDownList3, _id_nganh);

        if (_id_dichvu != "")
        {
            datlich_class.try_select_dropdown_value(DropDownList1, _id_dichvu);
            var q_dichvu = db.web_post_tables.Where(p => p.id.ToString() == _id_dichvu && p.id_chinhanh == Session["chinhanh"].ToString() && p.bin == false);
            if (q_dichvu.Count() != 0 && txt_tenthe.Text.Trim() == "")
                txt_tenthe.Text = q_dichvu.First().name;
        }

        if (_id_datlich != "" && txt_ghichu.Text.Trim() == "")
        {
            var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id_datlich && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q_lich.Count() != 0)
            {
                bspa_datlich_table _lich = q_lich.First();
                txt_ghichu.Text = datlich_lienket_class.dam_bao_ghi_chu_datlich(txt_ghichu.Text, _id_datlich, "Bán từ", _lich.tendichvu_taithoidiemnay);
            }
        }
    }

    private void ap_dung_khachhang_vao_form_ban_thedv(bspa_data_khachhang_table _kh)
    {
        if (_kh == null)
            return;

        if (txt_tenkhachhang.Text.Trim() == "")
            txt_tenkhachhang.Text = _kh.tenkhachhang;
        if (txt_diachi.Text.Trim() == "")
            txt_diachi.Text = _kh.diachi;
    }
    public string check_hsd(string _hsd)
    {
        if (DateTime.Now.Date > DateTime.Parse(_hsd).Date)
            return "<span class='fg-red'>" + _hsd + "</span>";
        else
            return _hsd;
    }
    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.thedichvu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_thedichvu"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_thedichvu"].ToString()).Date).ToList()
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
                            id_nganh = ob1.id_nganh,
                        }).ToList();


        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key) || p.sdt.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        if (ddl_loc_hsd.SelectedValue.ToString() != "0")
        {
            switch (ddl_loc_hsd.SelectedValue.ToString())
            {
                case ("1"): var list_2 = list_all.Where(p => p.hsd.Value.Date >= DateTime.Now.Date).ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                case ("2"): var list_3 = list_all.Where(p => p.hsd.Value.Date < DateTime.Now.Date).ToList(); list_all = list_all.Intersect(list_3).ToList(); break;
                default: break;
            }
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        //TÍNH THOỐNỐNG KÊ
        hoadon_sl = list_all.Count();
        doanhso_hoadon = list_all.Sum(p => p.tongtien).Value;
        doanhso_hoadon_sauck = list_all.Sum(p => p.tongsauchietkhau).Value;
        tongtien_dathanhtoan = list_all.Sum(p => p.sotien_dathanhtoan).Value;
        tong_congno = list_all.Sum(p => p.sotien_conlai).Value;

        //sắp xếp
        switch (Session["index_sapxep_thedichvu"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_thedichvu"].ToString());
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
        Session["search_thedichvu"] = txt_search.Text.Trim();
        Session["current_page_thedichvu"] = "1";
        main();

    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_thedichvu"] = int.Parse(Session["current_page_thedichvu"].ToString()) - 1;
        if (int.Parse(Session["current_page_thedichvu"].ToString()) < 1)
            Session["current_page_thedichvu"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_thedichvu"] = int.Parse(Session["current_page_thedichvu"].ToString()) + 1;
        if (int.Parse(Session["current_page_thedichvu"].ToString()) > total_page)
            Session["current_page_thedichvu"] = total_page;
        main();
    }


    protected void Button3_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q12_2") == "" || bcorn_class.check_quyen(user, "n12_2") == "")
        {
            string _nganh = DropDownList3.SelectedValue.ToString();
            string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
            string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
            string _ghichu = txt_ghichu.Text;
            string _diachi = txt_diachi.Text;
            string _ngaytao = txt_ngaytao.Text;
            string _tenthe = txt_tenthe.Text;
            string _hsd = txt_hsd.Text;
            string _id_datlich = (Request.QueryString["id_datlich"] ?? "").Trim();

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


            if (_nganh == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng chọn ngành", "4000", "warning"), true);
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
                                                thedichvu_table _ob = new thedichvu_table();
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
                                                if (_id_datlich != "")
                                                    _ghichu = datlich_lienket_class.dam_bao_ghi_chu_datlich(_ghichu, _id_datlich, "Bán từ", _ob.ten_taithoidiemnay);
                                                _ob.ghichu = _ghichu;
                                                _ob.sdt = _sdt;
                                                _ob.diachi = _diachi;
                                                _ob.id_nganh = _nganh;

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

                                                _ob.id_chinhanh = Session["chinhanh"].ToString();
                                                db.thedichvu_tables.InsertOnSubmit(_ob);
                                                db.SubmitChanges();
                                                string _id_thedv_moi = _ob.id.ToString();

                                                if (_id_datlich != "")
                                                {
                                                    datlich_lienket_class.dong_bo_vao_lich_hen(
                                                        db,
                                                        _id_datlich,
                                                        Session["chinhanh"].ToString(),
                                                        "",
                                                        _id_thedv_moi,
                                                        "",
                                                        _ob.ngaytao,
                                                        "Đã bán thẻ DV #" + _id_thedv_moi,
                                                        false);
                                                    db.SubmitChanges();
                                                }

                                                var q_data = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == Session["chinhanh"].ToString());
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
                                                    _ob1.anhdaidien = "/uploads/images/macdinh.jpg";
                                                    _ob1.matkhau = "F5F65B64F300D3764E16E57663F3072F";//12345678
                                                    _ob1.nhomkhachhang = "";

                                                    //if (txt_ngaysinh.Text != "" && dt_cl.check_date(txt_ngaysinh.Text) == true)
                                                    //    _ob1.ngaysinh = DateTime.Parse(txt_ngaysinh.Text);
                                                    _ob1.id_chinhanh = Session["chinhanh"].ToString();
                                                    _ob1.capbac = ""; _ob1.vnd_tu_e_aha = 0; _ob1.sodiem_e_aha = 0;
                                                    _ob1.solan_lencap = 0;
                                                    db.bspa_data_khachhang_tables.InsertOnSubmit(_ob1);
                                                    db.SubmitChanges();
                                                }


                                                //reset hết để bên trang quản lý nó hiển thị, chứ k là có lúc nó k hiện ra bài mới thêm,  tức ghê, kb do thằng nào
                                                reset_ss();


                                                //main();
                                                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm sản phẩm thành công.", "4000", "warning"), true);


                                                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Bán thẻ dịch vụ thành công.", "2000", "warning");
                                                string _id_khach = dtkh_cl.layid_tu_sdt(_sdt);
                                                if (_id_datlich != "" && _id_khach != "")
                                                    Response.Redirect("/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(_id_khach) + "&act=thedv&id_datlich=" + HttpUtility.UrlEncode(_id_datlich));
                                                else
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
        Session["index_sapxep_thedichvu"] = null;
        Session["current_page_thedichvu"] = null;
        Session["search_thedichvu"] = null;
        Session["show_thedichvu"] = null;
        Session["tungay_thedichvu"] = null;
        Session["denngay_thedichvu"] = null;

        Session["index_loc_hsd_thedichvu"] = null;
        Session["index_loc_nganh_thedichvu"] = null;
    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-the-dich-vu/Default.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_thedichvu"] = DropDownList2.SelectedIndex;
        Session["current_page_thedichvu"] = "1";
        Session["search_thedichvu"] = txt_search.Text.Trim();
        Session["show_thedichvu"] = txt_show.Text.Trim();

        Session["tungay_thedichvu"] = txt_tungay.Text;
        Session["denngay_thedichvu"] = txt_denngay.Text;

        Session["index_loc_hsd_thedichvu"] = ddl_loc_hsd.SelectedIndex;
        Session["index_loc_nganh_thedichvu"] = DropDownList5.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q12_4") == "" || bcorn_class.check_quyen(user, "n12_2") == "")
        {
            int _count = 0, _the_dadung = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.thedichvu_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (tdv_cl.exist_id(_id, user_parent))
                    {
                        thedichvu_table _ob = q.First();
                        if (_ob.tongsoluong != _ob.sl_conlai)
                            _the_dadung = _the_dadung + 1;//đếm xem có bn thẻ đã dùng

                        tdv_cl.del(_id);
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                if (_the_dadung > 0)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thẻ dịch vụ đã sử dụng, không xóa được.", "4000", "warning"), true);
                else
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

    public void load_nganh()
    {

        var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                          select new { id = ob1.id, ten = ob1.ten, }
                               );
        DropDownList3.DataSource = list_nganh;
        DropDownList3.DataTextField = "ten";
        DropDownList3.DataValueField = "id";
        DropDownList3.DataBind();
        DropDownList3.Items.Insert(0, new ListItem("Chọn", ""));
        if (bcorn_class.check_quyen(user, "q12_2") == "")
        {

        }
        else
        {
            DropDownList3.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
            DropDownList3.Enabled = false;
        }

    }

}
