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
    nhaphang_class nh_cl = new nhaphang_class();
    datetime_class dt_cl = new datetime_class();
    post_class po_cl = new post_class();
    nhaphang_chitiet_class ctnh_cl = new nhaphang_chitiet_class();
    string_class str_cl = new string_class();
    public string user, user_parent, id = "", tongtien, tienbangchu, ngaytao, ten_kh, sdt_kh, diachi_kh, ghichu_kh, ck_hoadon, km1_ghichu, id_guide, nguoitao, ncc;
    public Int64 sotien_conlai = 0, sauck = 0;
    public int stt_tt = 1;

    #region phân trang
    public int stt = 1;
    List<string> list_id_split;
    #endregion

    public void update_hoadon()
    {
        var q_hoadon = db.donnhaphang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        var q_hoadon_chitiet = db.donnhaphang_chitiet_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());

        //update tổng tiền
        donnhaphang_table _ob_hoadon = q_hoadon.First();
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
        ck_hoadon = return_ck_hoadon(id);
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
        var q_thanhtoan = db.donnhaphang_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
        Repeater2.DataSource = q_thanhtoan;
        Repeater2.DataBind();

        donnhaphang_table _ob = db.donnhaphang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        sotien_conlai = _ob.sotien_conlai.Value;
        if (sotien_conlai == 0)
        {
            var q_chitiet = db.donnhaphang_chitiet_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());
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
        var q_hoadon = db.donnhaphang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
        donnhaphang_table _ob = q_hoadon.First();
        tongtien = _ob.tongtien.Value.ToString("#,##0");//hiển thị tổng tiền hóa đơn
        ngaytao = _ob.ngaytao.Value.ToString("dd/MM/yyyy HH:mm");
        ten_kh = _ob.tenkhachhang;
        sdt_kh = _ob.sdt;
        ncc = nh_cl.return_name_ncc(_ob.nhacungcap);
        nguoitao = tk_cl.return_object(_ob.nguoitao).hoten;
        diachi_kh = _ob.diachi;
        ghichu_kh = _ob.ghichu;
        ck_hoadon = return_ck_hoadon(id);
        sotien_conlai = _ob.sotien_conlai.Value;
        km1_ghichu = _ob.km1_ghichu;
        id_guide = _ob.id_guide.ToString();
        if (!IsPostBack)
        {
            txt_ngaythanhtoan.Text = DateTime.Now.ToString();
            txt_nsx.Text = DateTime.Now.ToString();
            txt_hsd.Text = DateTime.Now.AddYears(1).ToString();
            txt_solo.Text = nhaphang_class.return_maxid().ToString();
            txt_ngayban_sanpham.Text = DateTime.Now.ToString();

            var list_nhacungcap = (from ob1 in db.nhacungcap_nhaphang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                   select new { id = ob1.id, ten = ob1.ten, }
                      );
            DropDownList3.DataSource = list_nhacungcap;
            DropDownList3.DataTextField = "ten";
            DropDownList3.DataValueField = "id";
            DropDownList3.DataBind();
            DropDownList3.Items.Insert(0, new ListItem("Chọn", ""));
            DropDownList3.SelectedIndex = nh_cl.return_index_ncc(_ob.nhacungcap);

            txt_ghichu.Text = _ob.ghichu;

            txt_ngaytao.Text = _ob.ngaytao.Value.ToString("dd/MM/yyyy");

            ck_hd_phantram.Checked = true;
            if (_ob.chietkhau == 0)
            {
                if (_ob.tongtien_ck_hoadon != 0)
                {
                    ck_hd_tienmat.Checked = true;
                    txt_chietkhau_hoadon.Text = _ob.tongtien_ck_hoadon.Value.ToString("#,##0");
                }
                else
                    ck_hd_phantram.Checked = true;
            }
            else
            {
                ck_hd_phantram.Checked = true;
                txt_chietkhau_hoadon.Text = _ob.chietkhau.ToString();
            }

            //var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động").ToList()
            //                     select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
            //          );


            //ddl_nhanvien_chotsale_sanpham.DataSource = list_nhanvien;
            //ddl_nhanvien_chotsale_sanpham.DataTextField = "tennhanvien";
            //ddl_nhanvien_chotsale_sanpham.DataValueField = "username";
            //ddl_nhanvien_chotsale_sanpham.DataBind();
            //ddl_nhanvien_chotsale_sanpham.Items.Insert(0, new ListItem("Chọn", ""));

        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q11_4";
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

        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            if (nh_cl.exist_id(id, user_parent))
            {
                reload();
                update_hoadon();
                reload_thanhtoan();
                main();
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
    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.donnhaphang_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_hoadon == id&& p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                            //join ob2 in db.web_post_tables.Where(p => p.phanloai == "ctsp").ToList() on ob1.id_dvsp.ToString() equals ob2.id.ToString()
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
                            kyhieu = ob1.kyhieu,
                            ngayban = ob1.ngaytao,
                            dvt = ob1.dvt,
                            solo = ob1.solo,
                            nsx = ob1.nsx,
                            hsd = ob1.hsd,
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
        Session["current_page_chitietnhaphang"] = "1";

        main();

    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_chitietnhaphang"] = int.Parse(Session["current_page_chitietnhaphang"].ToString()) - 1;

        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_chitietnhaphang"] = int.Parse(Session["current_page_chitietnhaphang"].ToString()) + 1;

        main();
    }
    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q11_5") == "")
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.donnhaphang_chitiet_tables.Where(p => p.id.ToString() == _id&& p.id_chinhanh == Session["chinhanh"].ToString());
                    if (ctnh_cl.exist_id(_id, user_parent))
                    {
                        donnhaphang_chitiet_table _ob = q.First();

                        db.donnhaphang_chitiet_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();

                        //đoạn dưới đừng thắc mắc, khi xóa hết chi tiết thì xóa luôn lịch sử thanh toán
                        var q_hoadon_chitiet = db.donnhaphang_chitiet_tables.Where(p => p.id_hoadon == id&& p.id_chinhanh == Session["chinhanh"].ToString());
                        if (q_hoadon_chitiet.Count() == 0)
                        {
                            var q_thanhtoan = db.donnhaphang_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
                            foreach (var t in q_thanhtoan)
                            {
                                //xóa lịch sử
                                donnhaphang_lichsu_thanhtoan_table _ob1 = t;
                                HoaDonThuChiSync_cl.DeleteForNhapHangPayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                                db.donnhaphang_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
                                db.SubmitChanges();
                            }
                        }
                    }
                }
            }
            reload();
            update_hoadon();
            reload_thanhtoan();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

 
    public string return_ck_hoadon(string _id_hd)
    {
        string _kq = "";
        var q = db.donnhaphang_tables.Where(p => p.id.ToString() == _id_hd && p.id_chinhanh == Session["chinhanh"].ToString());
        donnhaphang_table _ob = q.First();

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

    protected void but_form_themsanpham_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q11_5") == "")
        {
            string _ten_sanpham = txt_tensanpham.Text.Trim();
            string _id_sanpham = po_cl.return_id(_ten_sanpham);
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

            //var q_chitiet_hoadon = db.donnhaphang_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_dvsp == _id_sanpham && p.id_hoadon == id);

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
                            if (!tk_cl.exist_user_of_userparent(_user_chotsale, user_parent) && _user_chotsale != "")
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Nhân viên chốt sale không hợp lệ.", "4000", "warning"), true);
                            else
                            {
                                donnhaphang_chitiet_table _ob = new donnhaphang_chitiet_table();
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

                                _ob.phantram_chotsale_dvsp = 0;
                                _ob.tongtien_chotsale_dvsp = 0;

                                _ob.nguoilam_dichvu = "";
                                _ob.phantram_lamdichvu = 0;
                                _ob.tongtien_lamdichvu = 0;

                                _ob.id_hoadon = id;
                                _ob.kyhieu = "sanpham";
                                _ob.user_parent = user_parent;
                                _ob.nguoitao = Session["user"].ToString();
                                _ob.danhgia_nhanvien_lamdichvu = "";

                                _ob.dvt = txt_dvt.Text.Trim();
                                _ob.nsx = DateTime.Parse(txt_nsx.Text);
                                _ob.hsd = DateTime.Parse(txt_hsd.Text);
                                _ob.solo = txt_solo.Text;
                                _ob.sl_daban = 0;
                                _ob.sl_conlai = _r3;
                                _ob.tennguoichot_hientai = "";
                                _ob.tennguoilam_hientai = "";
                                _ob.id_chinhanh = Session["chinhanh"].ToString();
                                db.donnhaphang_chitiet_tables.InsertOnSubmit(_ob);
                                db.SubmitChanges();
                                //k cần tăng sl trong kho, vì đã có thêm 1 lô mới

                                reload();
                                update_hoadon();
                                reload_thanhtoan();
                                main();

                                //reset
                                txt_tensanpham.Text = "";
                                txt_gia_sanpham.Text = ""; txt_soluong_sanpham.Text = "1"; txt_chietkhau_sanpham.Text = "0";
                                txt_dvt.Text = "";


                                main();
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Nhập thêm hàng thành công.", "4000", "warning"), true);
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
    protected void but_xoahoadon_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q11_6") == "")
        {
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
            var q = db.donnhaphang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (nh_cl.exist_id(id, user_parent))
            {
                nh_cl.del(id);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa hóa đơn thành công.", "2000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-kho-hang/don-nhap-hang.aspx");
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_form_edithoadon_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q11_5") == "")
        {
            string _ck = txt_chietkhau_hoadon.Text.Trim().Replace(".", "");
            int _r1 = 0;
            int.TryParse(_ck, out _r1);//nếu là số nguyên thì gán cho _r

            //string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
            //string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
            string _ghichu = txt_ghichu.Text;
            //string _diachi = txt_diachi.Text;
            string _ngaytao = txt_ngaytao.Text;
            if (dt_cl.check_date(_ngaytao) == false)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày tạo không hợp lệ", "4000", "warning"), true);
            else
            {
                if ((_r1 < 0 && ck_hd_phantram.Checked == true) || (_r1 > 100 && ck_hd_phantram.Checked == true))//nếu chọn % thì k đc <0 & >100 
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu hóa đơn không hợp lệ.", "4000", "warning"), true);
                else
                {
                    var q = db.donnhaphang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (nh_cl.exist_id(id, user_parent))
                    {
                        donnhaphang_table _ob = q.First();
                        _ob.ngaytao = DateTime.Parse(_ngaytao + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());

                        _ob.ghichu = _ghichu;

                        _ob.nhacungcap = DropDownList3.SelectedValue.ToString();

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
                        reload();
                        update_hoadon();
                        reload_thanhtoan();
                        main();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "4000", "warning"), true);
                    }
                }


            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_xoathanhtoan_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q11_8") == "")
        {
            var q_thanhtoan = db.donnhaphang_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
            foreach (var t in q_thanhtoan)
            {
                if (Request.Form["check_lichsu_thanhtoan_" + t.id.ToString()] == "on")
                {
                    var q = db.donnhaphang_lichsu_thanhtoan_tables.Where(p => p.id == t.id && p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        Int64 _sotien_thanhtoan = q.First().sotienthanhtoan.Value;
                        string _hinhthuc_thanhtoan = q.First().hinhthuc_thanhtoan;

                        var q_hoadon = db.donnhaphang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        donnhaphang_table _ob = q_hoadon.First();
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
                        donnhaphang_lichsu_thanhtoan_table _ob1 = q.First();
                        HoaDonThuChiSync_cl.DeleteForNhapHangPayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                        db.donnhaphang_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
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
        if (bcorn_class.check_quyen(user, "q11_8") == "")
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
                    var q = db.donnhaphang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    donnhaphang_table _ob = q.First();
                    _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan + _st;
                    _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;
                    if (ddl_hinhthuc_thanhtoan.SelectedValue.ToString() == "Tiền mặt")
                        _ob.thanhtoan_tienmat = _ob.thanhtoan_tienmat + _st;
                    if (ddl_hinhthuc_thanhtoan.SelectedValue.ToString() == "Chuyển khoản")
                        _ob.thanhtoan_chuyenkhoan = _ob.thanhtoan_chuyenkhoan + _st;
                    if (ddl_hinhthuc_thanhtoan.SelectedValue.ToString() == "Quẹt thẻ")
                        _ob.thanhtoan_quetthe = _ob.thanhtoan_quetthe + _st;
                    db.SubmitChanges();

                    var q1 = db.donnhaphang_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    donnhaphang_lichsu_thanhtoan_table _ob1 = new donnhaphang_lichsu_thanhtoan_table();
                    _ob1.id_hoadon = id;
                    _ob1.sotienthanhtoan = _st;
                    _ob1.thoigian = DateTime.Parse(txt_ngaythanhtoan.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                    _ob1.user_parent = user_parent;
                    _ob1.hinhthuc_thanhtoan = ddl_hinhthuc_thanhtoan.SelectedValue.ToString();
                    _ob1.nguoithanhtoan = user;
                    _ob1.id_chinhanh = Session["chinhanh"].ToString();
                    db.donnhaphang_lichsu_thanhtoan_tables.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                    HoaDonThuChiSync_cl.UpsertFromNhapHangPayment(db, _ob, _ob1);
                    db.SubmitChanges();

                    update_hoadon();
                    reload_thanhtoan();
                    main();
                    txt_sotien_thanhtoan_congno.Text = _ob.sotien_conlai.Value.ToString("#,##0");
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


    protected void txt_tensanpham_TextChanged(object sender, EventArgs e)
    {
        string _ten_sanpham = txt_tensanpham.Text.ToString();
        string _id_sanpham = po_cl.return_id(_ten_sanpham);
        if (po_cl.exist_id(_id_sanpham))
        {
            var q1 = db.web_post_tables.Where(p => p.id.ToString() == _id_sanpham && p.phanloai == "ctsp" && p.bin == false && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q1.Count() != 0)
            {
                txt_gia_sanpham.Text = q1.First().giavon_sanpham.Value.ToString("#,##0");
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
