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
    post_class po_cl = new post_class();
    hoadon_chitiet_class cthd_cl = new hoadon_chitiet_class();
    string_class str_cl = new string_class();
    public string user, user_parent, id = "", active_dv, active_tdv, active_sp, url_back, idhd, danhgia_5sao_dv, danhgia_5sao_dv_thedv, sdt_kh;
    #region phân trang
    public int stt = 1;
    List<string> list_id_split, list_id_split_thedv;
    #endregion


    public void update_hoadon()
    {
        var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == idhd && p.id_chinhanh == Session["chinhanh"].ToString());
        var q_hoadon_chitiet = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == idhd && p.id_chinhanh == Session["chinhanh"].ToString());

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


        db.SubmitChanges();
    }

    public void load_dv(string _id)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        if (!IsPostBack)
        {
            bspa_hoadon_chitiet_table _ob = q.First();
            Panel1.Visible = true;
            Panel2.Visible = false;
            Panel3.Visible = false;
            active_dv = "class=\"active\"";
            active_sp = "class=\"disabled\"";
            active_tdv = "class=\"disabled\"";
            txt_ngayban.Text = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            txt_tendichvu.Text = _ob.ten_dvsp_taithoidiemnay;
            txt_gia.Text = _ob.gia_dvsp_taithoidiemnay.Value.ToString("#,##0");
            txt_soluong.Text = _ob.soluong.Value.ToString("#,##0");
            danhgia_5sao_dv = _ob.danhgia_5sao_dv;

            #region ck dvsp
            ck_dv_phantram.Checked = true;
            if (_ob.chietkhau == 0)
            {
                if (_ob.tongtien_ck_dvsp != 0)
                {
                    ck_dv_tienmat.Checked = true;
                    txt_chietkhau.Text = _ob.tongtien_ck_dvsp.Value.ToString("#,##0");
                }
                else
                    ck_dv_phantram.Checked = true;
            }
            else
            {
                ck_dv_phantram.Checked = true;
                txt_chietkhau.Text = _ob.chietkhau.ToString();
            }
            #endregion
            #region ck chốt sale
            ck_dv_phantram_chotsale.Checked = true;
            if (_ob.phantram_chotsale_dvsp == 0)
            {
                if (_ob.tongtien_chotsale_dvsp != 0)
                {
                    ck_dv_tienmat_chotsale.Checked = true;
                    txt_chietkhau_chotsale.Text = _ob.tongtien_chotsale_dvsp.Value.ToString("#,##0");
                }
                else
                    ck_dv_phantram_chotsale.Checked = true;
            }
            else
            {
                ck_dv_phantram_chotsale.Checked = true;
                txt_chietkhau_chotsale.Text = _ob.phantram_chotsale_dvsp.ToString();
            }
            #endregion
            #region ck làm dịch vụ
            ck_dv_phantram_chotsale_lamdv.Checked = true;
            if (_ob.phantram_lamdichvu == 0)
            {
                if (_ob.tongtien_lamdichvu != 0)
                {
                    ck_dv_tienmat_chotsale_lamdv.Checked = true;
                    txt_chietkhau_lamdichvu.Text = _ob.tongtien_lamdichvu.Value.ToString("#,##0");
                }
                else
                    ck_dv_phantram_chotsale_lamdv.Checked = true;
            }
            else
            {
                ck_dv_phantram_chotsale_lamdv.Checked = true;
                txt_chietkhau_lamdichvu.Text = _ob.phantram_lamdichvu.ToString();
            }
            #endregion

            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                             );
            ddl_nhanvien_chotsale.DataSource = list_nhanvien;
            ddl_nhanvien_chotsale.DataTextField = "tennhanvien";
            ddl_nhanvien_chotsale.DataValueField = "username";
            ddl_nhanvien_chotsale.DataBind();
            ddl_nhanvien_chotsale.Items.Insert(0, new ListItem("Chọn", ""));
            if (_ob.nguoichot_dvsp != "")
                ddl_nhanvien_chotsale.SelectedIndex = tk_cl.return_index(_ob.nguoichot_dvsp);

            ddl_nhanvien_lamdichvu.DataSource = list_nhanvien;
            ddl_nhanvien_lamdichvu.DataTextField = "tennhanvien";
            ddl_nhanvien_lamdichvu.DataValueField = "username";
            ddl_nhanvien_lamdichvu.DataBind();
            ddl_nhanvien_lamdichvu.Items.Insert(0, new ListItem("Chọn", ""));
            if (_ob.nguoilam_dichvu != "")
                ddl_nhanvien_lamdichvu.SelectedIndex = tk_cl.return_index(_ob.nguoilam_dichvu);

            txt_danhgia_dichvu.Text = _ob.danhgia_nhanvien_lamdichvu;
        }
    }
    public void load_sp(string _id)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        if (!IsPostBack)
        {
            bspa_hoadon_chitiet_table _ob = q.First();
            Panel1.Visible = false;
            Panel2.Visible = true;
            Panel3.Visible = false;
            active_sp = "class=\"active\"";
            active_dv = "class=\"disabled\"";
            active_tdv = "class=\"disabled\"";
            txt_ngayban_sanpham.Text = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            txt_tensanpham.Text = _ob.ten_dvsp_taithoidiemnay;
            txt_gia_sanpham.Text = _ob.gia_dvsp_taithoidiemnay.Value.ToString("#,##0");
            txt_soluong_sanpham.Text = _ob.soluong.Value.ToString("#,##0");

            #region ck sp
            ck_sp_phantram.Checked = true;
            if (_ob.chietkhau == 0)
            {
                if (_ob.tongtien_ck_dvsp != 0)
                {
                    ck_sp_tienmat.Checked = true;
                    txt_chietkhau_sanpham.Text = _ob.tongtien_ck_dvsp.Value.ToString("#,##0");
                }
                else
                    ck_sp_phantram.Checked = true;
            }
            else
            {
                ck_sp_phantram.Checked = true;
                txt_chietkhau_sanpham.Text = _ob.chietkhau.ToString();
            }
            #endregion
            #region ck chốt sale
            ck_sp_phantram_chotsale.Checked = true;
            if (_ob.phantram_chotsale_dvsp == 0)
            {
                if (_ob.tongtien_chotsale_dvsp != 0)
                {
                    ck_sp_tienmat_chotsale.Checked = true;
                    txt_chietkhau_chotsale_sanpham.Text = _ob.tongtien_chotsale_dvsp.Value.ToString("#,##0");
                }
                else
                    ck_sp_phantram_chotsale.Checked = true;
            }
            else
            {
                ck_sp_phantram_chotsale.Checked = true;
                txt_chietkhau_chotsale_sanpham.Text = _ob.phantram_chotsale_dvsp.ToString();
            }
            #endregion

            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                             );
            ddl_nhanvien_chotsale_sanpham.DataSource = list_nhanvien;
            ddl_nhanvien_chotsale_sanpham.DataTextField = "tennhanvien";
            ddl_nhanvien_chotsale_sanpham.DataValueField = "username";
            ddl_nhanvien_chotsale_sanpham.DataBind();
            ddl_nhanvien_chotsale_sanpham.Items.Insert(0, new ListItem("Chọn", ""));
            if (_ob.nguoichot_dvsp != "")
                ddl_nhanvien_chotsale_sanpham.SelectedIndex = tk_cl.return_index(_ob.nguoichot_dvsp);
        }
    }
    public void load_tdv(string _id)
    {
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

        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        if (!IsPostBack)
        {
            bspa_hoadon_chitiet_table _ob = q.First();
            Panel1.Visible = false;
            Panel2.Visible = false;
            Panel3.Visible = true;
            active_sp = "class=\"disabled\"";
            active_dv = "class=\"disabled\"";
            active_tdv = "class=\"active\"";
            txt_ngayban_thedv.Text = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            danhgia_5sao_dv_thedv = _ob.danhgia_5sao_dv;
            #region ck làm dịch vụ
            ck_dv_phantram_lamdv_thedv.Checked = true;
            if (_ob.phantram_lamdichvu == 0)
            {
                if (_ob.tongtien_lamdichvu != 0)
                {
                    ck_dv_tienmat_lamdv_thedv.Checked = true;
                    txt_chietkhau_lamdichvu_thedv.Text = _ob.tongtien_lamdichvu.Value.ToString("#,##0");
                }
                else
                    ck_dv_phantram_lamdv_thedv.Checked = true;
            }
            else
            {
                ck_dv_phantram_lamdv_thedv.Checked = true;
                txt_chietkhau_lamdichvu_thedv.Text = _ob.phantram_lamdichvu.ToString();
            }
            #endregion
            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                             );
            ddl_nhanvien_lamdichvu_thedv.DataSource = list_nhanvien;
            ddl_nhanvien_lamdichvu_thedv.DataTextField = "tennhanvien";
            ddl_nhanvien_lamdichvu_thedv.DataValueField = "username";
            ddl_nhanvien_lamdichvu_thedv.DataBind();
            ddl_nhanvien_lamdichvu_thedv.Items.Insert(0, new ListItem("Chọn", ""));
            if (_ob.nguoilam_dichvu != "")
                ddl_nhanvien_lamdichvu_thedv.SelectedIndex = tk_cl.return_index(_ob.nguoilam_dichvu);

            txt_danhgia_dichvu_lamdv.Text = _ob.danhgia_nhanvien_lamdichvu;
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
        if (bcorn_class.check_quyen(user, "q7_1") == "" || bcorn_class.check_quyen(user, "n7_1") == "")
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                id = Request.QueryString["id"].ToString().Trim();
                if (cthd_cl.exist_id(id, user_parent))
                {
                    if (bcorn_class.check_quyen(user, "q7_1") == "")//neu la quyen cap chi nhanh
                    {

                    }
                    else//neu la quyen cap nganh
                    {
                        if (cthd_cl.return_object(id).id_nganh != Session["nganh"].ToString())
                        {
                            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để xem ngành khác.", "false", "false", "OK", "alert", "");
                            Response.Redirect("/gianhang/admin");
                        }
                    }

                    var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    bspa_hoadon_chitiet_table _ob = q.First();
                    idhd = _ob.id_hoadon;
                    url_back = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + idhd;
                    var q_hd = db.bspa_hoadon_tables.Where(p => p.id.ToString() == idhd && p.id_chinhanh == Session["chinhanh"].ToString());
                    sdt_kh = q_hd.First().sdt;
                    string _kyhieu = _ob.kyhieu;
                    if (_kyhieu == "dichvu")
                    {
                        if (_ob.id_thedichvu != null)
                            load_tdv(id);
                        else
                            load_dv(id);
                    }
                    else
                        load_sp(id);
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

    protected void but_form_themdichvu_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q7_3") == "" || bcorn_class.check_quyen(user, "n7_3") == "")
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
                                        bspa_hoadon_chitiet_table _ob = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
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

                                        //_ob.id_hoadon = id;
                                        //_ob.kyhieu = "dichvu";
                                        //_ob.user_parent = user_parent;
                                        _ob.nguoitao = Session["user"].ToString();
                                        _ob.danhgia_nhanvien_lamdichvu = txt_danhgia_dichvu.Text.Trim();
                                        _ob.danhgia_5sao_dv = Request.Form["danhgia_5sao_nhanvien_dv"];

                                        db.SubmitChanges();

                                        update_hoadon();
                                        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                                        Response.Redirect(url_back);

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
        if (bcorn_class.check_quyen(user, "q7_3") == "" || bcorn_class.check_quyen(user, "n7_3") == "")
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


            //var q_chitiet_hoadon = db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_dvsp == _id_sanpham && p.id_hoadon == id);

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
                                    bspa_hoadon_chitiet_table _ob = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
                                    int _sl_old = _ob.soluong.Value;
                                    int _sl_chenhlech = _r3 - _sl_old;//sl mới - sl cũ
                                    if (!po_cl.check_sl_ton_sanpham(_id_sanpham, _sl_chenhlech) && _r3 > _sl_old)//nếu k đủ hàng để xuất
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số lượng trong kho không đủ để xuất", "false", "false", "OK", "alert", ""), true);
                                    else
                                    {
                                        //tăng kho sl cũ
                                        po_cl.tang_soluong_sanpham(_id_sanpham, _ob.soluong.Value);
                                        //giảm kho với sl mới
                                        po_cl.giam_soluong_sanpham(_id_sanpham, _r3);

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

                                        //_ob.id_hoadon = id;
                                        //_ob.kyhieu = "sanpham";
                                        //_ob.user_parent = user_parent;
                                        _ob.nguoitao = Session["user"].ToString();
                                        _ob.danhgia_nhanvien_lamdichvu = "";
                                        db.SubmitChanges();



                                        //update_hoadon();
                                        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                                        Response.Redirect(url_back);
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
    //thêm dv bằng thẻ dv
    protected void Button1_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Chức năng đang được cập nhật. Vui lòng chờ hoặc xóa và thêm lại mục này.", "false", "false", "OK", "alert", ""), true);
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
}
