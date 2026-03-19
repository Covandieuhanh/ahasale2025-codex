using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class chitiettintuc : System.Web.UI.Page
{
    public string notifi, meta, title_web, url_back;
    dbDataContext db = new dbDataContext();
    poss_class_homeaka po_cl = new poss_class_homeaka();
    string_class str_cl = new string_class();
    taikhoan_class tk_cl = new taikhoan_class();
    number_class nb_cl = new number_class();
    DataTable giohang = new DataTable();
    hoadon_home_class hd_cl = new hoadon_home_class();

    public string tongtien_text = "";
    public Int64 tongtien = 0, sauchietkhau = 0, ds_dv = 0, ds_sp = 0;
    public int sl_hangtronggio = 0, chietkhau = 0, sl_sp = 0, sl_dv = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        AhaShineContext_cl.EnsureContext();
        Session["ten_tk_chinhanh"] = AhaShineContext_cl.UserParent;
        Session["id_chinhanh_webcon"] = AhaShineContext_cl.ResolveChiNhanhId();

        #region meta
        var q = db.config_lienket_chiase_tables;
        if (q.Count() != 0)
        {
            #region thông tin trang web
            title_web = q.First().title;
            string _description = "<meta name=\"description\" content=\"" + q.First().description + "\" />";
            #endregion

            #region liên kết chia sẻ (Open Graphp)
            string _title_op = "<meta property=\"og:title\" content=\"" + "Giỏ hàng của bạn" + "\" />";
            string _image = "<meta property=\"og:image\" content=\"" + q.First().image + "\" />";
            string _description_op = "<meta property=\"og:description\" content=\"" + "" + "\" />";
            #endregion

            meta = _title_op + _image + _description + _description_op;
        }
        #endregion

        if (!IsPostBack)//tùy chọn hiển thị ngày giờ
        {

        }

        if (Request.Cookies[app_cookie_policy_class.home_return_url_cookie] != null)
            url_back = AhaShineHomeRoutes_cl.NormalizeReturnUrl(Request.Cookies[app_cookie_policy_class.home_return_url_cookie].Value);
        else
            url_back = AhaShineHomeRoutes_cl.HomeUrl;

        if (Session["giohang"] == null)
        {
            //Nếu chưa có giỏ hàng, tạo giỏ hàng thông qua DataTable với các cột sau
            giohang.Columns.Add("ID");
            giohang.Columns.Add("Name");
            giohang.Columns.Add("Price");
            giohang.Columns.Add("soluong");
            giohang.Columns.Add("thanhtien");
            giohang.Columns.Add("img");

            giohang.Columns.Add("kyhieu");//là sanpham hoặc dichvu
            //Sau khi tạo xong thì lưu lại vào session
            Session["giohang"] = giohang;
        }
        else
        {
            //Lấy thông tin giỏ hàng từ Session["giohang"]
            giohang = Session["giohang"] as DataTable;
            foreach (DataRow row in giohang.Rows) // Duyệt từng dòng (DataRow) trong DataTable
            {
                tongtien = tongtien + int.Parse(row["thanhtien"].ToString());
                if (row["kyhieu"].ToString() == "dichvu")
                {
                    sl_dv = sl_dv + 1;
                    ds_dv = ds_dv + int.Parse(row["thanhtien"].ToString());
                }
                if (row["kyhieu"].ToString() == "sanpham")
                {
                    sl_sp = sl_sp + 1;
                    ds_sp = ds_sp + int.Parse(row["thanhtien"].ToString());
                }
            }
        }

        if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            string _soluong = Request.QueryString["sl"];
            int _sl = 1;
            if (!String.IsNullOrWhiteSpace(Request.QueryString["sl"]))
            {
                int.TryParse(_soluong, out _sl);
                if (_sl <= 0 || _sl > 9999)
                    _sl = 1;
            }
            string _id = Request.QueryString["id"];
            if (po_cl.exist_id(_id))
            {
                string _phanloai = po_cl.return_object(_id).phanloai;
                string _kyhieu = "";
                Int64 _gia = 0;
                if (_phanloai == "ctdv")//nếu đây là dịch vụ
                {
                    _gia = po_cl.return_object(_id).giaban_dichvu.Value;
                    _kyhieu = "dichvu";
                    //nếu là dichvu thì k cho thêm vào giỏ này sản phẩm. Tách riêng ra.
                    Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Mã sản phẩm không tồn tại.", "false", "false", "OK", "success", "");
                    Response.Redirect(url_back);
                }
                else
                {
                    if (_phanloai == "ctsp")//nếu đây là sản phẩm
                    {
                        _gia = po_cl.return_object(_id).giaban_sanpham.Value;
                        _kyhieu = "sanpham";
                    }
                    else
                        Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);//nếu là dạng bài viết thì k thêm vào giỏ hàng
                }


                string _ten_spdv = po_cl.return_object(_id).name;
                //Kiểm tra xem đã có sản phẩm trong giỏ hàng chưa ?
                //Nếu chưa thì thêm bản ghi mới vào giỏ hàng với số lượng soluong là 1
                //Nếu có thì tăng soluong lên 1
                bool isExisted = false;
                foreach (DataRow dr in giohang.Rows)
                {
                    if (dr["ID"].ToString() == _id)
                    {
                        dr["soluong"] = int.Parse(dr["soluong"].ToString()) + _sl;
                        dr["thanhtien"] = (Int64.Parse(dr["Price"].ToString()) * (int.Parse(dr["soluong"].ToString()))).ToString();
                        isExisted = true;
                        break;
                    }
                }
                if (!isExisted)//Chưa có sản phẩm trong giỏ hàng
                {
                    DataRow dr = giohang.NewRow();
                    dr["ID"] = _id;
                    dr["Name"] = _ten_spdv;
                    dr["Price"] = _gia;
                    dr["soluong"] = _sl;
                    dr["thanhtien"] = _gia * _sl;
                    dr["img"] = po_cl.return_object(_id).image;
                    dr["kyhieu"] = _kyhieu;//là sanpham hoặc dichvu
                    giohang.Rows.Add(dr);
                }
                //Lưu lại thông tin giỏ hàng mới nhất vào session["giohang"]
                Session["giohang"] = giohang;
                if (!String.IsNullOrWhiteSpace(Request.QueryString["dh"]))
                {
                    string _dh = Request.QueryString["dh"].ToLower();
                    if (_dh != "ok")
                    {
                        Session["notifi_home"] = thongbao_class.metro_dialog_2but_onload("Thông báo", "Thêm hàng vào giỏ thành công.", "false", "false", "Tiếp tục chọn mặt hàng", "secondary", "", "Xem giỏ hàng", "success", AhaShineHomeRoutes_cl.CartUrl);
                        Response.Redirect(url_back);
                    }
                    else
                    {
                        Response.Redirect(AhaShineHomeRoutes_cl.CartUrl);//để reset các thông số tránh trường hợp ng ta tải lại trang sẽ tăng thêm số lượng
                    }
                }
                else
                {
                    Session["notifi_home"] = thongbao_class.metro_dialog_2but_onload("Thông báo", "Thêm hàng vào giỏ thành công.", "false", "false", "Tiếp tục chọn mặt hàng", "secondary", "", "Xem giỏ hàng", "success", AhaShineHomeRoutes_cl.CartUrl);
                    Response.Redirect(url_back);
                }

            }
        }

        if (Session["giohang"] != null)
            sl_hangtronggio = giohang.Rows.Count;
        if (sl_hangtronggio > 0)
        {
            but_save.Visible = true;
            but_huygiohang.Visible = true;
        }

        //==>sauchietkhau chiết khấu
        sauchietkhau = tongtien - (tongtien / 100 * chietkhau);
        tongtien_text = sauchietkhau.ToString();

        //Hiện thị thông tin giỏ hàng
        Repeater1.DataSource = giohang;
        Repeater1.DataBind();
    }
    protected void but_capnhat_Click(object sender, EventArgs e)
    {
        foreach (DataRow row in giohang.Rows) // Duyệt từng dòng (DataRow) trong DataTable
        {
            Int64 _soluong;
            Int64.TryParse(Request.Form["sl_" + row["ID"].ToString()], out _soluong);
            if (_soluong <= 0)
                _soluong = 1;
            row["soluong"] = _soluong;
            row["thanhtien"] = _soluong * int.Parse(row["Price"].ToString());
            //Response.Write(_soluong + " ");
        }
        Session["notifi_home"] = "<script>function openNotify(){var notify = Metro.notify;notify.setup({timeout: 4000,});notify.create('Cập nhật giỏ hàng thành công.', 'Thông báo', {cls: 'light'});}window.onload = function () {openNotify();};</script>";
        Response.Redirect(AhaShineHomeRoutes_cl.CartUrl);
    }

    protected void but_huygiohang_Click(object sender, EventArgs e)
    {
        Session["giohang"] = null;
        Session["notifi_home"] = "<script>function openNotify(){var notify = Metro.notify;notify.setup({timeout: 4000,});notify.create('Hủy giỏ hàng thành công.', 'Thông báo', {cls: 'light'});}window.onload = function () {openNotify();};</script>";
        Response.Redirect(AhaShineHomeRoutes_cl.CartUrl);
    }

    protected void but_dathang_Click(object sender, EventArgs e)
    {


        string _tenkh = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "");
        string _diachi = txt_diachi.Text;
        string _ghichu = txt_ghichu.Text;
        if (tongtien == 0)
            notifi = "<script>function openDialog() {Metro.dialog.create({title: 'Thông báo',content: '<div>Giỏ hàng của bạn không có gì. Vui lòng thêm sản phẩm vào giỏ trước khi thanh toán.</div>',closeButton: false,overlayClickClose: false,actions: [{caption: 'OK',cls: 'js-dialog-close info'}]});}window.onload = function () {openDialog();};</script>";
        else
        {
            if (_tenkh == "" || _sdt == "" || _diachi == "")
            {
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng điền đầy đủ thông tin.", "false", "false", "OK", "primary", "");
            }
            else
            {
                foreach (DataRow row1 in giohang.Rows) // Duyệt từng dòng (DataRow) trong DataTable, để xem kho có đủ xuất không
                {
                    if (!po_cl.check_sl_ton_sanpham(row1["ID"].ToString(), int.Parse(row1["soluong"].ToString())))//nếu 1 trong số sp k đủ hàng để xuất
                    {
                        Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Hiện tại chúng tôi không đáp ứng được đơn hàng này.<br/>Vui lòng thử lại sau.", "false", "false", "OK", "success", "");
                        Response.Redirect(url_back);
                    }
                    else
                    {
                        //TẠO HÓA ĐƠN
                        Guid _idguide = Guid.NewGuid();
                        bspa_hoadon_table _ob = new bspa_hoadon_table();
                        _ob.id_guide = _idguide;
                        _ob.ngaytao = DateTime.Now;
                        _ob.tongtien = tongtien;
                        _ob.chietkhau = chietkhau;
                        _ob.tongsauchietkhau = sauchietkhau;
                        _ob.tenkhachhang = _tenkh;
                        _ob.ghichu = _ghichu;
                        _ob.sdt = _sdt;
                        _ob.diachi = _diachi;
                        _ob.sotien_dathanhtoan = 0;
                        _ob.thanhtoan_tienmat = 0; _ob.thanhtoan_chuyenkhoan = 0; _ob.thanhtoan_quetthe = 0;
                        _ob.sotien_conlai = tongtien;
                        _ob.sl_dichvu = sl_dv;
                        _ob.sl_sanpham = sl_sp;
                        _ob.album = "";
                        _ob.user_parent = AhaShineContext_cl.UserParent;
                        _ob.id_chinhanh = AhaShineContext_cl.ResolveChiNhanhId();
                        _ob.id_nganh = "";

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

                        _ob.tongtien_ck_hoadon = 0;
                        _ob.km1_ghichu = "";
                        _ob.nguoitao = "admin";
                        _ob.nguongoc = "Web";

                        db.bspa_hoadon_tables.InsertOnSubmit(_ob);
                        db.SubmitChanges();

                        string _idhd = hd_cl.return_id_bang_idguide(_idguide.ToString().ToUpper());

                        foreach (DataRow row in giohang.Rows) // Duyệt từng dòng (DataRow) trong DataTable
                        {
                            bspa_hoadon_chitiet_table _ob2 = new bspa_hoadon_chitiet_table();
                            _ob2.id_hoadon = _idhd;
                            _ob2.id_dvsp = row["ID"].ToString();
                            _ob2.kyhieu = row["kyhieu"].ToString();

                            _ob2.soluong = int.Parse(row["soluong"].ToString());
                            //giảm số lượng
                            po_cl.giam_soluong_sanpham(_ob2.id_dvsp, _ob2.soluong.Value);

                            _ob2.thanhtien = Int64.Parse(row["thanhtien"].ToString());
                            _ob2.chietkhau = 0;
                            _ob2.tongtien_ck_dvsp = 0;
                            _ob2.gia_dvsp_taithoidiemnay = int.Parse(row["Price"].ToString());
                            _ob2.tongsauchietkhau = _ob2.thanhtien;
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
                            _ob2.id_chinhanh = AhaShineContext_cl.ResolveChiNhanhId();
                            _ob2.id_nganh = "";
                            _ob2.user_parent = AhaShineContext_cl.UserParent;
                            _ob2.nguoitao = "admin";
                            _ob2.danhgia_nhanvien_lamdichvu = "";

                            db.bspa_hoadon_chitiet_tables.InsertOnSubmit(_ob2);
                            db.SubmitChanges();
                        }
                        Session["giohang"] = null;

                        //thông báo trên app
                        thongbao_table _ob1 = new thongbao_table();
                        _ob1.id = _idguide;
                        _ob1.daxem = false;//chưa xem
                        _ob1.nguoithongbao = "admin";
                        _ob1.nguoinhan = "admin";
                        _ob1.link = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + _idhd;
                        _ob1.noidung = _tenkh + " vừa tạo đơn hàng tại website";
                        _ob1.thoigian = DateTime.Now;
                        db.thongbao_tables.InsertOnSubmit(_ob1);
                        db.SubmitChanges();

                        //thông báo qua email                
                        //string _mail_captren = tk_cl.return_object("admin").email;
                        //string _lists_email = /*_mailboss + "," +*/ _mail_captren;
                        //if (_lists_email != "")
                        //{
                        //    string[] _list = _lists_email.Split(',');
                        //    for (int _i = 0; _i < _list.Length; _i++)
                        //    {
                        //        string _tieudemail_mail = _tenkh + " vừa đặt hàng tại halora.vn.";
                        //        string _emailnhan = _list[_i].Trim();

                        //        string _noidung_mail = "";
                        //        _noidung_mail = _noidung_mail + "<h3>THÔNG TIN ĐƠN HÀNG</h3>";
                        //        _noidung_mail = _noidung_mail + "<div>MÃ ĐƠN HÀNG: <b>" + _idhd + "</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>TÊN KHÁCH HÀNG: <b>" + _tenkh + "</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>SỐ ĐIỆN THOẠI: <b>" + _sdt + "</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>ĐỊA CHỈ GIAO HÀNG: <b>" + _diachi + "</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>TỔNG TIỀN: <b>" + tongtien + "</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>CHIẾT KHẤU: <b>" + chietkhau + "%</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>TỔNG SAU CHIẾT KHẤU: <b>" + sauchietkhau.ToString("#,##0") + "</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>SỐ TIỀN BẰNG CHỮ: <b>" + number_class.number_to_text_unlimit(tongtien_text) + " đồng.</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div>GHI CHÚ: <b>" + txt_ghichu.Text + "</b></div>";
                        //        _noidung_mail = _noidung_mail + "<div style='color:red'>Lưu ý: Đây là thư tự động được gửi từ hệ thống app.halora.vn. Không phản hồi mail này, xin cám ơn.</div>";
                        //        sendmail_class.sendmail("smtp.gmail.com", 587, _tieudemail_mail,
                        //            _noidung_mail, _emailnhan, "app.halora.vn", "");
                        //    }
                        //}

                        Session["notifi_home"] = thongbao_class.metro_dialog_onload("Đặt đơn thành công", "Mã đơn: <b>" + _idhd + "</b><br/>Xin cám ơn và hẹn gặp lại.", "false", "false", "OK", "success", "");
                        Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
                    }
                }


            }
        }
    }
    public string tien(string _abc)
    {
        return Int64.Parse(_abc).ToString("#,##0");
    }
}
