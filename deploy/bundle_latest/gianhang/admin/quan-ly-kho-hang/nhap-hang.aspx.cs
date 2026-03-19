using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_menu_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    post_class po_cl = new post_class();
    datetime_class dt_cl = new datetime_class();
    nhaphang_class nh_cl = new nhaphang_class();
    menu_class mn_cl = new menu_class();
    public string act_nhaphang, user, user_parent;
    DataTable giohang_nhaphang_admin = new DataTable();
    List<web_post_table> list_all = new List<web_post_table>();
    public string notifi = "", tongtien_text = "";
    public Int64 tongtien = 0, sauchietkhau = 0, ds_dv = 0, ds_sp = 0, tongtien_ck_hoadon = 0;
    public int sl_hangtronggio = 0, sl_sp = 0, sl_dv = 0;
    public string tien(string _abc)
    {
        if (string.IsNullOrWhiteSpace(_abc))
            return "0";
        if (!Int64.TryParse(_abc, out Int64 _value))
            return "0";
        return _value.ToString("#,##0");
    }
    public string Safe(object value)
    {
        return value == null ? "" : value.ToString();
    }

    public string SafeNumber(object value)
    {
        return value == null ? "0" : value.ToString();
    }
    public string return_tenmn(string _idmn)
    {
        return mn_cl.return_object(_idmn).name;
    }

    #region phân trang
    public int stt = 1;
    List<string> list_id_split;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {

        #region Check_Login  
        string _quyen = "q11_2";
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

        main();

        if (!IsPostBack)
        {
            txt_ngaynhap.Text = DateTime.Now.Date.ToString();

            var list_nhacungcap = (from ob1 in db.nhacungcap_nhaphang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                   select new { id = ob1.id, ten = ob1.ten, }
                      );
            DropDownList3.DataSource = list_nhacungcap;
            DropDownList3.DataTextField = "ten";
            DropDownList3.DataValueField = "id";
            DropDownList3.DataBind();
            DropDownList3.Items.Insert(0, new ListItem("Chọn", ""));

            if (Session["chietkhau_nhaphang_admin"] != null)
            {
                txt_chietkhau.Text = tien(Session["chietkhau_nhaphang_admin"].ToString());
            }
            else
                Session["chietkhau_nhaphang_admin"] = txt_chietkhau.Text;

            if (Session["loai_chietkhau_nhaphang_admin"] != null)
            {
                if (Session["loai_chietkhau_nhaphang_admin"].ToString() == "phantram")
                    ck_hd_phantram.Checked = true;
                else
                    ck_hd_tienmat.Checked = true;
            }
            else
            {
                ck_hd_phantram.Checked = true;
                Session["loai_chietkhau_nhaphang_admin"] = "phantram";
            }
        }

        load_giohang();

        if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]))//khi ng dùng nhấn vào nút tạo hóa đơn từ menutop --> tạo nhanh
        {
            string _q = Request.QueryString["q"].ToString().Trim();
            if (_q == "nh")
                act_nhaphang = "active";
        }




    }

    public void load_giohang()
    {
        if (Session["giohang_nhaphang_admin"] == null)
        {
            //Nếu chưa có giỏ hàng, tạo giỏ hàng thông qua DataTable với các cột sau
            giohang_nhaphang_admin.Columns.Add("ID");
            giohang_nhaphang_admin.Columns.Add("Name");

            giohang_nhaphang_admin.Columns.Add("Price");
            giohang_nhaphang_admin.Columns.Add("soluong");
            giohang_nhaphang_admin.Columns.Add("thanhtien");
            giohang_nhaphang_admin.Columns.Add("loai_chietkhau");
            giohang_nhaphang_admin.Columns.Add("chietkhau");
            giohang_nhaphang_admin.Columns.Add("tongtien_ck_hoadon");
            giohang_nhaphang_admin.Columns.Add("sauck");

            giohang_nhaphang_admin.Columns.Add("img");
            giohang_nhaphang_admin.Columns.Add("nsx");
            giohang_nhaphang_admin.Columns.Add("hsd");
            giohang_nhaphang_admin.Columns.Add("solo");
            giohang_nhaphang_admin.Columns.Add("dvt");

            giohang_nhaphang_admin.Columns.Add("kyhieu");//là sanpham hoặc dichvu
                                                         //Sau khi tạo xong thì lưu lại vào session
            Session["giohang_nhaphang_admin"] = giohang_nhaphang_admin;
        }
        else
        {
            tongtien = 0;
            //Lấy thông tin giỏ hàng từ Session["giohang"]
            giohang_nhaphang_admin = Session["giohang_nhaphang_admin"] as DataTable;
            foreach (DataRow row in giohang_nhaphang_admin.Rows) // Duyệt từng dòng (DataRow) trong DataTable
            {
                tongtien = tongtien + int.Parse(row["sauck"].ToString());
                if (row["kyhieu"].ToString() == "dichvu")
                {
                    sl_dv = sl_dv + 1;
                    ds_dv = ds_dv + int.Parse(row["sauck"].ToString());
                }
                if (row["kyhieu"].ToString() == "sanpham")
                {
                    sl_sp = sl_sp + 1;
                    ds_sp = ds_sp + int.Parse(row["sauck"].ToString());
                }
            }
        }

        if (Session["giohang_nhaphang_admin"] != null)
            sl_hangtronggio = giohang_nhaphang_admin.Rows.Count;
        if (sl_hangtronggio > 0)
        {
            but_save.Visible = true;
            but_huygiohang.Visible = true;
        }

        //==> sauchietkhau chiết khấu
        if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
            sauchietkhau = tongtien - (tongtien / 100 * int.Parse(Session["chietkhau_nhaphang_admin"].ToString()));
        else
            sauchietkhau = tongtien - int.Parse(Session["chietkhau_nhaphang_admin"].ToString());

        tongtien_text = sauchietkhau.ToString();


        //Hiện thị thông tin giỏ hàng
        Repeater2.DataSource = giohang_nhaphang_admin;
        Repeater2.DataBind();
    }

    public void main()
    {
        list_all = db.web_post_tables.Where(p => p.phanloai == "ctsp" && p.bin == false && p.id_chinhanh == Session["chinhanh"].ToString()).ToList();
        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.name.ToLower().Contains(_key) || p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        list_all = list_all.OrderBy(p => p.name).ToList();

        Repeater1.DataSource = list_all;
        Repeater1.DataBind();
    }

    #region autopostback
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        Session["search_nhaphang"] = txt_search.Text.Trim();
        Session["current_page_nhaphang"] = "1";
        main();
    }

    #endregion



    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        giohang_nhaphang_admin = Session["giohang_nhaphang_admin"] as DataTable;
        foreach (var t in list_all)
        {
            string _idsp = t.id.ToString().Replace("slsp_", "");//lấy idsp

            string _soluong = Request.Form["slsp_" + t.id.ToString()];//lấy số lượng của sp đó
            //xử lý số lượng
            int _sl = 0;
            int.TryParse(_soluong, out _sl);
            if (_sl <= 0 || _sl > 9999)
                _sl = 0;

            string _gianhap = Request.Form["giavon_" + t.id.ToString()].Replace(".", "");//lấy giá nhập
            //xử lý giá nhập
            Int64 _gia = 0;
            Int64.TryParse(_gianhap, out _gia);
            if (_gia <= 0)
                _gia = 0;

            string _chietkhau = Request.Form["ck_" + t.id.ToString()].Replace(".", "");//lấy giá nhập
            //xử lý ck
            Int64 _ck = 0;
            Int64.TryParse(_chietkhau, out _ck);
            if (_ck <= 0)
                _ck = 0;

            string _nsx = Request.Form["nsx_" + t.id.ToString()];
            string _hsd = Request.Form["hsd_" + t.id.ToString()];
            string _solo = Request.Form["solo_" + t.id.ToString()];
            string _dvt = Request.Form["dvt_" + t.id.ToString()];



            //nếu >0 thì thêm vào giỏ hàng
            if (_sl > 0 && po_cl.exist_id(_idsp))
            {

                string _phanloai = po_cl.return_object(_idsp).phanloai;
                string _kyhieu = "sanpham";
                //Int64 _gia = po_cl.return_object(_idsp).giavon_sanpham.Value;

                string _ten_sp = po_cl.return_object(_idsp).name;

                //Kiểm tra xem đã có sản phẩm trong giỏ hàng chưa ?
                //Nếu chưa thì thêm bản ghi mới vào giỏ hàng với số lượng soluong là request số lương
                //Nếu có thì tăng soluong
                bool isExisted = false;
                foreach (DataRow dr in giohang_nhaphang_admin.Rows)
                {
                    if (dr["ID"].ToString() == _idsp)
                    {
                        dr["soluong"] = int.Parse(dr["soluong"].ToString()) + _sl;
                        dr["thanhtien"] = (Int64.Parse(dr["Price"].ToString()) * int.Parse(dr["soluong"].ToString())).ToString();

                        if (Request.Form["loai_ck_" + t.id.ToString()] == "phantram")//nếu ck  là %
                        {
                            if (_ck > 100)//phần trăm k đc lớn hơn 100
                                _ck = 0;

                            dr["loai_chietkhau"] = "phantram";
                            dr["chietkhau"] = _ck;
                            dr["sauck"] = Int64.Parse(dr["thanhtien"].ToString()) - (Int64.Parse(dr["thanhtien"].ToString()) * _ck / 100);
                            dr["tongtien_ck_hoadon"] = Int64.Parse(dr["thanhtien"].ToString()) - Int64.Parse(dr["sauck"].ToString());
                        }
                        else
                        {
                            if (_ck > Int64.Parse(dr["thanhtien"].ToString()))//ck tiền thì k đc lớn hơn số tiền
                                _ck = 0;
                            dr["loai_chietkhau"] = "tienmat";
                            dr["chietkhau"] = 0;
                            dr["sauck"] = _gia * _sl - _ck;
                            dr["tongtien_ck_hoadon"] = _ck;
                        }

                        isExisted = true;
                        break;
                    }
                }
                if (!isExisted)//Chưa có sản phẩm trong giỏ hàng
                {
                    DataRow dr = giohang_nhaphang_admin.NewRow();
                    dr["ID"] = _idsp;
                    dr["Name"] = _ten_sp;
                    dr["Price"] = _gia;
                    dr["soluong"] = _sl;
                    dr["thanhtien"] = _gia * _sl;
                    dr["img"] = po_cl.return_object(_idsp).image;
                    dr["nsx"] = _nsx;
                    dr["hsd"] = _hsd;
                    dr["solo"] = _solo;
                    dr["kyhieu"] = _kyhieu;
                    dr["dvt"] = _dvt;

                    if (Request.Form["loai_ck_" + t.id.ToString()] == "phantram")//nếu ck  là %
                    {
                        if (_ck > 100)//phần trăm k đc lớn hơn 100
                            _ck = 0;
                        dr["loai_chietkhau"] = "phantram";
                        dr["chietkhau"] = _ck;
                        dr["sauck"] = Int64.Parse(dr["thanhtien"].ToString()) - (Int64.Parse(dr["thanhtien"].ToString()) * _ck / 100);
                        dr["tongtien_ck_hoadon"] = Int64.Parse(dr["thanhtien"].ToString()) - Int64.Parse(dr["sauck"].ToString());
                    }
                    else
                    {
                        if (_ck > Int64.Parse(dr["thanhtien"].ToString()))//ck tiền thì k đc lớn hơn số tiền
                            _ck = 0;
                        dr["loai_chietkhau"] = "tienmat";
                        dr["chietkhau"] = 0;
                        dr["sauck"] = _gia * _sl - _ck;
                        dr["tongtien_ck_hoadon"] = _ck;
                    }

                    giohang_nhaphang_admin.Rows.Add(dr);
                }
                //Lưu lại thông tin giỏ hàng mới nhất
                Session["giohang_nhaphang_admin"] = giohang_nhaphang_admin;
            }
        }
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm vào giỏ thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx");
    }


    protected void but_capnhat_Click(object sender, EventArgs e)
    {
        string _ck_hd = txt_chietkhau.Text.Trim().Replace(".", "");
        int _r1 = 0;
        int.TryParse(_ck_hd, out _r1);//nếu là số nguyên thì gán cho _r

        if ((_r1 < 0 && ck_hd_phantram.Checked == true) || (_r1 > 100 && ck_hd_phantram.Checked == true))//nếu chọn % thì k đc <0 & >100 
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu không hợp lệ.", "4000", "warning"), true);
        else
        {
            Session["chietkhau_nhaphang_admin"] = _r1;
            if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
                Session["loai_chietkhau_nhaphang_admin"] = "phantram";
            else
                Session["loai_chietkhau_nhaphang_admin"] = "tienmat";

            foreach (DataRow row in giohang_nhaphang_admin.Rows) // Duyệt từng dòng (DataRow) trong DataTable
            {
                Int64 _soluong;
                Int64.TryParse(Request.Form["sl_" + row["ID"].ToString()], out _soluong);
                if (_soluong <= 0)
                    _soluong = 1;

                Int64 _gianhap;
                string _g = Request.Form["gianhap_" + row["ID"].ToString()].Replace(".", "");
                Int64.TryParse(_g, out _gianhap);
                if (_gianhap <= 0)
                    _gianhap = 0;

                string _chietkhau = Request.Form["ck_gh_" + row["ID"].ToString()].Replace(".", "");//lấy giá nhập                                                                  //xử lý ck
                Int64 _ck = 0;
                Int64.TryParse(_chietkhau, out _ck);
                if (_ck <= 0)
                    _ck = 0;

                string _nsx = Request.Form["nsx_gh_" + row["ID"].ToString()];
                string _hsd = Request.Form["hsd_gh_" + row["ID"].ToString()];
                string _solo = Request.Form["solo_gh_" + row["ID"].ToString()];
                string _dvt = Request.Form["dvt_gh_" + row["ID"].ToString()];

                row["Price"] = _gianhap;
                row["soluong"] = _soluong;
                row["thanhtien"] = _soluong * _gianhap;
                row["nsx"] = _nsx;
                row["hsd"] = _hsd;
                row["solo"] = _solo;
                row["dvt"] = _dvt;

                if (Request.Form["loai_ck_gh_" + row["ID"].ToString()] == "phantram")//nếu ck  là %
                {
                    row["loai_chietkhau"] = "phantram";
                    row["chietkhau"] = _ck;
                    row["sauck"] = Int64.Parse(row["thanhtien"].ToString()) - (Int64.Parse(row["thanhtien"].ToString()) * _ck / 100);
                    row["tongtien_ck_hoadon"] = Int64.Parse(row["thanhtien"].ToString()) - Int64.Parse(row["sauck"].ToString());
                }
                else
                {
                    row["loai_chietkhau"] = "tienmat";
                    row["chietkhau"] = 0;
                    row["sauck"] = _gianhap * _soluong - _ck;
                    row["tongtien_ck_hoadon"] = _ck;
                }
            }
            load_giohang();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật đơn nhập hàng thành công", "4000", "warning"), true);
            //Session["notifi"] = "<script>function openNotify(){var notify = Metro.notify;notify.setup({timeout: 4000,});notify.create('Cập nhật đơn nhập hàng thành công.', 'Thông báo', {cls: 'warning'});}window.onload = function () {openNotify();};</script>";
            //Response.Redirect("/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx");
        }


    }

    protected void but_huygiohang_Click(object sender, EventArgs e)
    {
        Session["chietkhau_nhaphang_admin"] = null;
        Session["giohang_nhaphang_admin"] = null;
        Session["notifi"] = "<script>function openNotify(){var notify = Metro.notify;notify.setup({timeout: 4000,});notify.create('Hủy đơn nhập hàng thành công.', 'Thông báo', {cls: 'light'});}window.onload = function () {openNotify();};</script>";
        Response.Redirect("/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx");
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        string _ngaynhap = txt_ngaynhap.Text;
        string _ghichu = txt_ghichu.Text;
        if (tongtien == 0)
            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Đơn hàng của bạn đang trống.", "4000", "warning");
        else
        {
            //TẠO HÓA ĐƠN
            Guid _idguide = Guid.NewGuid();
            donnhaphang_table _ob = new donnhaphang_table();
            _ob.id_guide = _idguide;
            _ob.ngaytao = DateTime.Parse(_ngaynhap + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
            _ob.tongtien = tongtien;

            if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
            {
                _ob.chietkhau = int.Parse(Session["chietkhau_nhaphang_admin"].ToString());
                _ob.tongsauchietkhau = _ob.tongtien - (_ob.tongtien * _ob.chietkhau / 100);
                _ob.tongtien_ck_hoadon = _ob.tongtien - _ob.tongsauchietkhau;
            }
            else
            {
                _ob.chietkhau = 0;
                _ob.tongsauchietkhau = _ob.tongtien - int.Parse(Session["chietkhau_nhaphang_admin"].ToString());
                _ob.tongtien_ck_hoadon = int.Parse(Session["chietkhau_nhaphang_admin"].ToString());
            }



            _ob.tenkhachhang = "";
            _ob.ghichu = _ghichu;
            _ob.sdt = "";
            _ob.diachi = "";
            _ob.sotien_dathanhtoan = 0;
            _ob.thanhtoan_tienmat = 0; _ob.thanhtoan_chuyenkhoan = 0; _ob.thanhtoan_quetthe = 0;
            _ob.sotien_conlai = tongtien;
            _ob.sl_dichvu = sl_dv;
            _ob.sl_sanpham = sl_sp;
            _ob.album = "";
            _ob.user_parent = "admin";


            if (sl_dv == 0)
            {
                if (sl_sp != 0)
                    _ob.dichvu_hay_sanpham = "sanpham";
            }
            else
            {
                if (sl_sp == 0)
                    _ob.dichvu_hay_sanpham = "dichvu";
                else
                    _ob.dichvu_hay_sanpham = "dichvusanpham";
            }

            _ob.ds_dichvu = ds_dv;
            _ob.ds_sanpham = ds_sp;
            _ob.sauck_dichvu = ds_dv;
            _ob.sauck_sanpham = ds_sp;


            _ob.km1_ghichu = "";
            _ob.nguoitao = Session["user"].ToString();
            _ob.nguongoc = "";

            _ob.nhacungcap = DropDownList3.SelectedValue.ToString();
            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.donnhaphang_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();

            string _idhd = nh_cl.return_id_bang_idguide(_idguide.ToString());

            foreach (DataRow row in giohang_nhaphang_admin.Rows) // Duyệt từng dòng (DataRow) trong DataTable
            {
                donnhaphang_chitiet_table _ob2 = new donnhaphang_chitiet_table();
                _ob2.id_hoadon = _idhd;
                _ob2.id_dvsp = row["ID"].ToString();
                _ob2.kyhieu = row["kyhieu"].ToString();
                _ob2.soluong = int.Parse(row["soluong"].ToString());
                _ob2.sl_daban = 0;
                _ob2.sl_conlai = int.Parse(row["soluong"].ToString());
                _ob2.thanhtien = Int64.Parse(row["thanhtien"].ToString());
                _ob2.chietkhau = int.Parse(row["chietkhau"].ToString());
                _ob2.tongtien_ck_dvsp = Int64.Parse(row["tongtien_ck_hoadon"].ToString());
                _ob2.gia_dvsp_taithoidiemnay = int.Parse(row["Price"].ToString());
                _ob2.tongsauchietkhau = Int64.Parse(row["sauck"].ToString());
                _ob2.ten_dvsp_taithoidiemnay = row["Name"].ToString();
                _ob2.hinhanh_hientai = "";
                _ob2.ngaytao = DateTime.Now;
                _ob2.nguoichot_dvsp = "";
                _ob2.tennguoichot_hientai = "";
                _ob2.phantram_chotsale_dvsp = 0;
                _ob2.tongtien_chotsale_dvsp = 0;
                _ob2.nguoilam_dichvu = "";
                _ob2.tennguoilam_hientai = "";
                _ob2.phantram_lamdichvu = 0;
                _ob2.tongtien_lamdichvu = 0;

                _ob2.user_parent = "admin";
                _ob2.nguoitao = Session["user"].ToString();
                _ob2.danhgia_nhanvien_lamdichvu = "";

                _ob2.nsx = DateTime.Parse(row["nsx"].ToString());
                _ob2.hsd = DateTime.Parse(row["hsd"].ToString());
                _ob2.solo = row["solo"].ToString();
                _ob2.dvt = row["dvt"].ToString();
                _ob2.id_chinhanh = Session["chinhanh"].ToString();
                db.donnhaphang_chitiet_tables.InsertOnSubmit(_ob2);
                db.SubmitChanges();
            }
            reset_ss();
            Session["giohang_nhaphang_admin"] = null;
            Session["chietkhau_nhaphang_admin"] = null;
            Session["loai_chietkhau_nhaphang_admin"] = null;

            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Nhập hàng thành công.", "4000", "warning");
            Response.Redirect("/gianhang/admin/quan-ly-kho-hang/don-nhap-hang.aspx");
        }

    }
    public void reset_ss()
    {
        Session["index_sapxep_donnhaphang"] = null;
        Session["current_page_donnhaphang"] = null;
        Session["search_donnhaphang"] = null;
        Session["show_donnhaphang"] = null;
        Session["tungay_donnhaphang"] = null;
        Session["denngay_donnhaphang"] = null;
        Session["index_loc_thanhtoan_donnhaphang"] = null;

    }
}
