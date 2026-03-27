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

    vattu_class vt_cl = new vattu_class();
    nhapvattu_class nvt_cl = new nhapvattu_class();
    nhapvattu_chitiet_class ctnvt_cl = new nhapvattu_chitiet_class();

    string_class str_cl = new string_class();
    public string user, user_parent, id = "", url_back, idhd;



    public void update_hoadon()
    {
        var q_hoadon = db.donnhap_vattu_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        var q_hoadon_chitiet = db.donnhap_vattu_chitiet_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());

        //update tổng tiền
        donnhap_vattu_table _ob_hoadon = q_hoadon.First();
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


    public void load_sp(string _id)
    {
        var q = db.donnhap_vattu_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        if (!IsPostBack)
        {
            donnhap_vattu_chitiet_table _ob = q.First();

            Panel2.Visible = true;

            txt_ngayban_sanpham.Text = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            txt_nsx.Text = _ob.nsx.Value.ToString("dd/MM/yyyy");
            txt_hsd.Text = _ob.hsd.Value.ToString("dd/MM/yyyy");
            txt_solo.Text = _ob.solo;
            txt_dvt.Text = _ob.dvt;
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



        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q13_5";
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
            if (ctnvt_cl.exist_id(id, user_parent))
            {
                var q = db.donnhap_vattu_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                donnhap_vattu_chitiet_table _ob = q.First();
                idhd = _ob.id_hoadon;
                url_back = "/gianhang/admin/quan-ly-vat-tu/chi-tiet-nhap-hang.aspx?id=" + idhd;
                string _kyhieu = _ob.kyhieu;

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


    protected void but_form_themsanpham_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q13_5") == "")
        {
            string _ten_sanpham = txt_tensanpham.Text.Trim();
            string _id_sanpham = vt_cl.return_id(_ten_sanpham);
            string _user_chotsale = "";

            string _gia = txt_gia_sanpham.Text.Trim().Replace(".", "");
            int _r1 = 0;
            int.TryParse(_gia, out _r1);//nếu là số nguyên thì gán cho _r

            string _ck = txt_chietkhau_sanpham.Text.Trim().Replace(".", "");
            int _r2 = 0;
            int.TryParse(_ck, out _r2);//nếu là số nguyên thì gán cho _r

            string _sl = txt_soluong_sanpham.Text.Trim().Replace(".", "");
            int _r3 = 0;
            int.TryParse(_sl, out _r3);//nếu là số nguyên thì gán cho _r

            //var q_chitiet_hoadon = db.donnhap_vattu_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_dvsp == _id_sanpham && p.id_hoadon == id);

            if (vt_cl.exist_id(_id_sanpham))
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
                            if (!tk_cl.exist_user_of_userparent(_user_chotsale, user_parent) && _user_chotsale != "")
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Nhân viên chốt sale không hợp lệ.", "4000", "warning"), true);
                            else
                            {
                                donnhap_vattu_chitiet_table _ob = db.donnhap_vattu_chitiet_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
                                if (_r3 < _ob.sl_daban)//SL mới k thể ít hơn sl đã bán của lô này     
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số lượng mới không được lớn hơn số lượng đã xuất của lô này.", "4000", "warning"), true);
                                else
                                {
                                    _ob.sl_conlai = _r3 - _ob.sl_daban;

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
                                    //_ob.ngaytao = DateTime.Parse(txt_ngayban_sanpham.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());


                                    _ob.nguoichot_dvsp = _user_chotsale;

                                    _ob.phantram_chotsale_dvsp = 0;
                                    _ob.tongtien_chotsale_dvsp = 0;

                                    _ob.nguoilam_dichvu = "";
                                    _ob.phantram_lamdichvu = 0;
                                    _ob.tongtien_lamdichvu = 0;

                                    _ob.dvt = txt_dvt.Text.Trim();
                                    _ob.nsx = DateTime.Parse(txt_nsx.Text);
                                    _ob.hsd = DateTime.Parse(txt_hsd.Text);
                                    _ob.solo = txt_solo.Text;


                                    db.SubmitChanges();



                                    //update_hoadon();
                                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                                    Response.Redirect(url_back);
                                }
                            }
                            //}

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


    //autocomplete
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers1(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.danhsach_vattu_tables.Where(p => p.tenvattu.Contains(prefixText) && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.tenvattu).ToList();
    }

    protected void txt_tensanpham_TextChanged(object sender, EventArgs e)
    {
        string _ten_sanpham = txt_tensanpham.Text.ToString();
        string _id_sanpham = vt_cl.return_id(_ten_sanpham);
        if (vt_cl.exist_id(_id_sanpham))
        {
            var q1 = db.danhsach_vattu_tables.Where(p => p.id.ToString() == _id_sanpham && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q1.Count() != 0)
            {
                txt_gia_sanpham.Text = q1.First().gianhap.Value.ToString("#,##0");
                txt_dvt.Text = q1.First().donvitinh_sp;
            }
        }
        else
        {
            txt_gia_sanpham.Text = "";
            txt_dvt.Text = "";
        }
    }
}
