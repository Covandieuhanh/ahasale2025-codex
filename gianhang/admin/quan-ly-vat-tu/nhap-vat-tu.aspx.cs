using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
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
    vattu_class vattu_cl = new vattu_class();
    datetime_class dt_cl = new datetime_class();
    nhapvattu_class vt_cl = new nhapvattu_class();

    
    menu_class mn_cl = new menu_class();
    public string act_nhaphang, user, user_parent;
    DataTable giohang_nhapvattu_admin = new DataTable();
    List<danhsach_vattu_table> list_all = new List<danhsach_vattu_table>();
    public string notifi = "", tongtien_text = "";
    public Int64 tongtien = 0, sauchietkhau = 0, ds_dv = 0, ds_sp = 0, tongtien_ck_hoadon = 0;
    public int sl_hangtronggio = 0, sl_sp = 0, sl_dv = 0;
    public string tien(string _abc)
    {
        return Int64.Parse(_abc).ToString("#,##0");
    }
    public string return_tenmn(string _idmn)
    {
        return mn_cl.return_object(_idmn).name;
    }

    #region phân trang
    public int stt = 1;
    List<string> list_id_split;
    #endregion

    private bool EnsureActionAccess(string requiredPermission)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, requiredPermission);
        if (access == null)
            return false;

        user = (access.User ?? "").Trim();
        if (string.IsNullOrWhiteSpace(user_parent))
            user_parent = access.OwnerAccountKey;
        return true;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q13_2");
        if (access == null)
            return;
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;

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

            if (Session["chietkhau_nhapvattu_admin"] != null)
            {
                txt_chietkhau.Text = tien(Session["chietkhau_nhapvattu_admin"].ToString());
            }
            else
                Session["chietkhau_nhapvattu_admin"] = txt_chietkhau.Text;

            if (Session["loai_chietkhau_nhapvattu_admin"] != null)
            {
                if (Session["loai_chietkhau_nhapvattu_admin"].ToString() == "phantram")
                    ck_hd_phantram.Checked = true;
                else
                    ck_hd_tienmat.Checked = true;
            }
            else
            {
                ck_hd_phantram.Checked = true;
                Session["loai_chietkhau_nhapvattu_admin"] = "phantram";
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
        if (Session["giohang_nhapvattu_admin"] == null)
        {
            //Nếu chưa có giỏ hàng, tạo giỏ hàng thông qua DataTable với các cột sau
            giohang_nhapvattu_admin.Columns.Add("ID");
            giohang_nhapvattu_admin.Columns.Add("Name");

            giohang_nhapvattu_admin.Columns.Add("Price");
            giohang_nhapvattu_admin.Columns.Add("soluong");
            giohang_nhapvattu_admin.Columns.Add("thanhtien");
            giohang_nhapvattu_admin.Columns.Add("loai_chietkhau");
            giohang_nhapvattu_admin.Columns.Add("chietkhau");
            giohang_nhapvattu_admin.Columns.Add("tongtien_ck_hoadon");
            giohang_nhapvattu_admin.Columns.Add("sauck");

            giohang_nhapvattu_admin.Columns.Add("img");
            giohang_nhapvattu_admin.Columns.Add("nsx");
            giohang_nhapvattu_admin.Columns.Add("hsd");
            giohang_nhapvattu_admin.Columns.Add("solo");
            giohang_nhapvattu_admin.Columns.Add("dvt");

            giohang_nhapvattu_admin.Columns.Add("kyhieu");//là sanpham hoặc dichvu
                                                         //Sau khi tạo xong thì lưu lại vào session
            Session["giohang_nhapvattu_admin"] = giohang_nhapvattu_admin;
        }
        else
        {
            tongtien = 0;
            //Lấy thông tin giỏ hàng từ Session["giohang"]
            giohang_nhapvattu_admin = Session["giohang_nhapvattu_admin"] as DataTable;
            foreach (DataRow row in giohang_nhapvattu_admin.Rows) // Duyệt từng dòng (DataRow) trong DataTable
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

        if (Session["giohang_nhapvattu_admin"] != null)
            sl_hangtronggio = giohang_nhapvattu_admin.Rows.Count;
        if (sl_hangtronggio > 0)
        {
            but_save.Visible = true;
            but_huygiohang.Visible = true;
        }

        //==> sauchietkhau chiết khấu
        if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
            sauchietkhau = tongtien - (tongtien / 100 * int.Parse(Session["chietkhau_nhapvattu_admin"].ToString()));
        else
            sauchietkhau = tongtien - int.Parse(Session["chietkhau_nhapvattu_admin"].ToString());

        tongtien_text = sauchietkhau.ToString();


        //Hiện thị thông tin giỏ hàng
        Repeater2.DataSource = giohang_nhapvattu_admin;
        Repeater2.DataBind();
    }

    public void main()
    {
        list_all = db.danhsach_vattu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList();
        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenvattu.ToLower().Contains(_key) || p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        list_all = list_all.OrderBy(p => p.tenvattu).ToList();

        Repeater1.DataSource = list_all;
        Repeater1.DataBind();
    }

    #region autopostback
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    protected void but_search_Click(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    private void ApplySearchState()
    {
        Session["current_page_nhapvattu"] = "1";

        main();
    }

    #endregion



    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        giohang_nhapvattu_admin = Session["giohang_nhapvattu_admin"] as DataTable;
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
            if (_sl > 0 && vattu_cl.exist_id(_idsp))
            {

                string _phanloai = vattu_cl.return_object(_idsp).id_nhom;
                string _kyhieu = "sanpham";
                //Int64 _gia = po_cl.return_object(_idsp).giavon_sanpham.Value;

                string _ten_sp = vattu_cl.return_object(_idsp).tenvattu;

                //Kiểm tra xem đã có sản phẩm trong giỏ hàng chưa ?
                //Nếu chưa thì thêm bản ghi mới vào giỏ hàng với số lượng soluong là request số lương
                //Nếu có thì tăng soluong
                bool isExisted = false;
                foreach (DataRow dr in giohang_nhapvattu_admin.Rows)
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
                    DataRow dr = giohang_nhapvattu_admin.NewRow();
                    dr["ID"] = _idsp;
                    dr["Name"] = _ten_sp;
                    dr["Price"] = _gia;
                    dr["soluong"] = _sl;
                    dr["thanhtien"] = _gia * _sl;
                    dr["img"] = vattu_cl.return_object(_idsp).image;
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

                    giohang_nhapvattu_admin.Rows.Add(dr);
                }
                //Lưu lại thông tin giỏ hàng mới nhất
                Session["giohang_nhapvattu_admin"] = giohang_nhapvattu_admin;
            }
        }
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm vào giỏ thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx");
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
            Session["chietkhau_nhapvattu_admin"] = _r1;
            if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
                Session["loai_chietkhau_nhapvattu_admin"] = "phantram";
            else
                Session["loai_chietkhau_nhapvattu_admin"] = "tienmat";

            foreach (DataRow row in giohang_nhapvattu_admin.Rows) // Duyệt từng dòng (DataRow) trong DataTable
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
        Session["chietkhau_nhapvattu_admin"] = null;
        Session["giohang_nhapvattu_admin"] = null;
        Session["notifi"] = "<script>function openNotify(){var notify = Metro.notify;notify.setup({timeout: 4000,});notify.create('Hủy đơn nhập hàng thành công.', 'Thông báo', {cls: 'light'});}window.onload = function () {openNotify();};</script>";
        Response.Redirect("/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx");
    }

    public void reset_ss()
    {
        Session["index_sapxep_donnhapvattu"] = null;
        Session["current_page_donnhapvattu"] = null;
        Session["search_donnhapvattu"] = null;
        Session["show_donnhapvattu"] = null;
        Session["tungay_donnhapvattu"] = null;
        Session["denngay_donnhapvattu"] = null;
        Session["index_loc_thanhtoan_donnhapvattu"] = null;

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!EnsureActionAccess("q13_2"))
            return;

        string _ngaynhap = txt_ngaynhap.Text;
        string _ghichu = txt_ghichu.Text;
        if (tongtien == 0)
            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Đơn hàng của bạn đang trống.", "4000", "warning");
        else
        {
            //TẠO HÓA ĐƠN
            Guid _idguide = Guid.NewGuid();
            donnhap_vattu_table _ob = new donnhap_vattu_table();
            _ob.id_guide = _idguide;
            _ob.ngaytao = DateTime.Parse(_ngaynhap + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
            _ob.tongtien = tongtien;

            if (ck_hd_phantram.Checked == true)//nếu ck chốt sale là %
            {
                _ob.chietkhau = int.Parse(Session["chietkhau_nhapvattu_admin"].ToString());
                _ob.tongsauchietkhau = _ob.tongtien - (_ob.tongtien * _ob.chietkhau / 100);
                _ob.tongtien_ck_hoadon = _ob.tongtien - _ob.tongsauchietkhau;
            }
            else
            {
                _ob.chietkhau = 0;
                _ob.tongsauchietkhau = _ob.tongtien - int.Parse(Session["chietkhau_nhapvattu_admin"].ToString());
                _ob.tongtien_ck_hoadon = int.Parse(Session["chietkhau_nhapvattu_admin"].ToString());
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
            _ob.user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();


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
            _ob.nguoitao = user;
            _ob.nguongoc = "";

            _ob.nhacungcap = DropDownList3.SelectedValue.ToString();

            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.donnhap_vattu_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();

            string _idhd = vt_cl.return_id_bang_idguide(_idguide.ToString());

            foreach (DataRow row in giohang_nhapvattu_admin.Rows) // Duyệt từng dòng (DataRow) trong DataTable
            {
                donnhap_vattu_chitiet_table _ob2 = new donnhap_vattu_chitiet_table();
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

                _ob2.user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
                _ob2.nguoitao = user;
                _ob2.danhgia_nhanvien_lamdichvu = "";

                _ob2.nsx = DateTime.Parse(row["nsx"].ToString());
                _ob2.hsd = DateTime.Parse(row["hsd"].ToString());
                _ob2.solo = row["solo"].ToString();
                _ob2.dvt = row["dvt"].ToString();

                _ob2.id_chinhanh = Session["chinhanh"].ToString();
                db.donnhap_vattu_chitiet_tables.InsertOnSubmit(_ob2);
                db.SubmitChanges();
            }
            reset_ss();
            Session["giohang_nhapvattu_admin"] = null;
            Session["chietkhau_nhapvattu_admin"] = null;
            Session["loai_chietkhau_nhapvattu_admin"] = null;

            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Nhập vật tư thành công.", "4000", "warning");
            Response.Redirect("/gianhang/admin/quan-ly-vat-tu/vat-tu-da-nhap.aspx");
        }

    }
}
