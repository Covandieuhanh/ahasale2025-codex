using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_gio_hang : System.Web.UI.Page
{
    private const decimal TY_GIA_DONGA_VND = 1000m; // 1 Quyền tiêu dùng = 1000 VNĐ

    // ✅ Quy đổi VNĐ -> A (làm tròn lên 2 chữ số thập phân vì ví A là decimal(18,2))
    private decimal QuyDoi_VND_To_A(decimal vnd)
    {
        if (vnd <= 0) return 0m;
        decimal a = vnd / TY_GIA_DONGA_VND;
        return Math.Ceiling(a * 100m) / 100m;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string _tk = Session["taikhoan_home"] as string;

            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }

            show_main();
        }
    }

    #region main (HIỂN THỊ VNĐ)
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list_all = (from ob1 in db.GioHang_tbs.Where(p => p.taikhoan == ViewState["taikhoan"].ToString())
                            join ob2 in db.BaiViet_tbs on ob1.idsp equals ob2.id.ToString() into SanPhamGroup
                            from ob2 in SanPhamGroup.DefaultIfEmpty()
                            join ob3 in db.taikhoan_tbs on ob1.nguoiban_goc equals ob3.taikhoan into TaiKhoanGroup
                            from ob3 in TaiKhoanGroup.DefaultIfEmpty()
                            select new
                            {
                                ob1.id,
                                ob1.ngaythem,
                                ob2.image,
                                ob2.name,
                                ob2.name_en,
                                ob2.giaban, // ✅ VNĐ
                                ob1.soluong,

                                // ✅ % ưu đãi (null -> 0)
                                PhanTramUuDai = (ob2.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0),

                                // ✅ Thành tiền VNĐ
                                ThanhTien = ((ob2.giaban ?? 0m) * (ob1.soluong ?? 0)),

                                TenShop = ob3.ten_shop,
                            }).AsQueryable();

            list_all = list_all.OrderByDescending(p => p.ngaythem);

            Repeater1.DataSource = list_all;
            Repeater1.DataBind();
        }
    }

    #endregion

    protected void Button1_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            int _dem = 0;
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    string _id_giohang = lblData.Text;
                    var q = db.GioHang_tbs.FirstOrDefault(p => p.id.ToString() == _id_giohang);
                    if (q != null)
                    {
                        db.GioHang_tbs.DeleteOnSubmit(q);
                        _dem++;
                    }
                }
            }

            if (_dem > 0)
            {
                db.SubmitChanges();
                show_main();

                // ✅ đổi thông báo sang Tabler
                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
            }
            else
            {
                // ✅ đổi dialog sang Tabler
                Helper_Tabler_cl.ShowModal(this.Page, "Không có mục nào được chọn.", "Thông báo", true, "warning");
            }
        }
    }

    #region trao đổi (đặt hàng)

    public class BaiVietViewModel
    {
        public string id { get; set; }
        public DateTime? ngaythem { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public decimal? giaban { get; set; }   // ✅ VNĐ
        public int soluong { get; set; }
        public decimal? ThanhTien { get; set; } // ✅ VNĐ
        public string TenShop { get; set; }
        public string nguoiban_goc { get; set; }
        public string nguoiban_danglai { get; set; }
        public string idsp { get; set; }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            List<GioHang_tb> danhSachChon = new List<GioHang_tb>();
            List<string> danhSachID = new List<string>();

            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    danhSachID.Add(lblData.Text);
                }
            }

            if (danhSachID.Count > 0)
            {
                danhSachChon = db.GioHang_tbs
                                 .Where(p => danhSachID.Contains(p.id.ToString()))
                                 .ToList();

                var idspList = danhSachChon.Select(p => p.idsp).ToList();
                bool allStopped = !db.BaiViet_tbs.Any(p => idspList.Contains(p.id.ToString()) && p.bin == false);
                if (allStopped)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán", "Thông báo", true, "warning");
                    return;
                }

                var list_all = (from ob1 in danhSachChon
                                join ob2 in db.BaiViet_tbs on ob1.idsp equals ob2.id.ToString() into SanPhamGroup
                                from ob2 in SanPhamGroup.DefaultIfEmpty()
                                join ob3 in db.taikhoan_tbs on ob1.nguoiban_goc equals ob3.taikhoan into TaiKhoanGroup
                                from ob3 in TaiKhoanGroup.DefaultIfEmpty()
                                where ob2.bin != true
                                select new
                                {
                                    ob1.id,
                                    ob1.ngaythem,
                                    ob2.image,
                                    ob2.name,
                                    ob2.name_en,
                                    ob2.giaban, // ✅ VNĐ
                                    ob1.soluong,

                                    // ✅ % ưu đãi (null -> 0)
                                    PhanTramUuDai = (ob2.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0),

                                    // ✅ Thành tiền VNĐ
                                    ThanhTien = ((ob2.giaban ?? 0m) * (ob1.soluong ?? 0)),

                                    TenShop = ob3.ten_shop,
                                    ob1.nguoiban_goc,
                                    ob1.nguoiban_danglai,
                                    ob1.idsp
                                }).AsQueryable();


                var list_all_data = list_all.Select(x => new BaiVietViewModel
                {
                    id = x.id.ToString(),
                    ngaythem = x.ngaythem,
                    image = x.image,
                    name = x.name,
                    name_en = x.name_en,
                    giaban = x.giaban,
                    soluong = (int)(x.soluong ?? 0),
                    ThanhTien = x.ThanhTien,
                    TenShop = x.TenShop,
                    nguoiban_goc = x.nguoiban_goc,
                    nguoiban_danglai = x.nguoiban_danglai,
                    idsp = x.idsp,
                }).ToList();

                Session["danhSachGomNhom"] = list_all_data;

                Repeater2.DataSource = list_all.OrderBy(p => p.nguoiban_goc);
                Repeater2.DataBind();

                var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
                if (q_tk != null)
                {
                    txt_hoten_nguoinhan.Text = q_tk.hoten;
                    txt_sdt_nguoinhan.Text = q_tk.dienthoai;
                    txt_diachi_nguoinhan.Text = q_tk.diachi;
                }

                pn_dathang.Visible = !pn_dathang.Visible;
                up_dathang.Update();
            }
            else
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không có mục nào được chọn.", "Thông báo", true, "warning");
            }
        }
    }

    protected void but_close_form_dathang_Click(object sender, EventArgs e)
    {
        Session["danhSachGomNhom"] = null;
        pn_dathang.Visible = !pn_dathang.Visible;
    }

    protected void but_dathang_Click(object sender, EventArgs e)
    {
        if (Session["danhSachGomNhom"] == null) return;

        using (dbDataContext db = new dbDataContext())
        {
            var danhSachGomNhom = Session["danhSachGomNhom"] as List<BaiVietViewModel>;
            if (danhSachGomNhom == null || danhSachGomNhom.Count == 0) return;

            // ✅ Check sản phẩm còn bán
            var idspList = danhSachGomNhom.Select(p => p.idsp).ToList();
            bool allStopped = !db.BaiViet_tbs.Any(p => idspList.Contains(p.id.ToString()) && p.bin == false);
            if (allStopped)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Có sản phẩm đã ngừng bán", "Thông báo", true, "warning");
                return;
            }

            string tk = (ViewState["taikhoan"] ?? "").ToString();
            if (string.IsNullOrEmpty(tk)) return;

            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (q_tk == null) return;

            // ✅ Số dư 2 ví
            decimal soDuA = (q_tk.DongA ?? 0m);
            decimal soDuUuDaiA = (q_tk.Vi1That_Evocher_30PhanTram ?? 0m); // ví ưu đãi 30% (LoaiHoSo_Vi=2)


            // ✅ Gom theo người bán
            var groupedData = danhSachGomNhom
                .GroupBy(x => x.nguoiban_goc)
                .Select(g => new
                {
                    NguoiBan = g.Key,
                    BaiViets = g.ToList(),
                    TongThanhToan_TungDon_VND = g.Sum(item => item.ThanhTien ?? 0m)
                })
                .ToList();

            // ✅ Pre-check: đảm bảo nếu KHÔNG áp dụng ưu đãi thì Đồng A vẫn đủ cho TẤT CẢ đơn
            // (vì ưu đãi có thể không đủ => fallback về trừ 100% Đồng A)
            decimal tongA_ToiThieuCanCo = 0m;
            foreach (var group in groupedData)
            {
                decimal tongVND_TungDon = group.TongThanhToan_TungDon_VND;
                tongA_ToiThieuCanCo += QuyDoi_VND_To_A(tongVND_TungDon);
            }

            if (tongA_ToiThieuCanCo > soDuA)
            {
                // Thông báo theo tổng A (tính đúng như mỗi đơn một tỷ giá làm tròn)
                decimal tongThanhToan_VND = danhSachGomNhom.Sum(item => item.ThanhTien ?? 0m);
                Helper_Tabler_cl.ShowModal(
                    this.Page,
                    $"Quyền tiêu dùng của bạn không đủ để đặt các đơn này.<br/>" +
                    $"Cần <b>{tongA_ToiThieuCanCo:#,##0.##} A</b> (≈ {tongThanhToan_VND:#,##0}đ).",
                    "Thông báo",
                    true,
                    "danger"
                );
                return;
            }

            DateTime _now = AhaTime_cl.Now;

            foreach (var group in groupedData)
            {
                decimal tongVND_TungDon = group.TongThanhToan_TungDon_VND;

                // ✅ Tổng A của đơn (luôn theo tổng đơn)
                decimal canTraA_TungDon = QuyDoi_VND_To_A(tongVND_TungDon);

                // =========================================================
                // ✅ TÍNH ƯU ĐÃI TRÊN ĐƠN (cộng theo từng sản phẩm trong đơn)
                // =========================================================
                decimal tienUuDaiVND_TungDon = 0m;

                foreach (var item in group.BaiViets)
                {
                    // lấy % ưu đãi từ BaiViet
                    var sp = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == item.idsp && p.bin == false);
                    if (sp == null) continue;

                    int phanTramUuDai = 0;
                    if (sp.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
                        phanTramUuDai = sp.PhanTram_GiamGia_ThanhToan_BangEvoucher.Value;

                    if (phanTramUuDai < 0) phanTramUuDai = 0;
                    if (phanTramUuDai > 50) phanTramUuDai = 50;

                    // tính ưu đãi theo từng dòng: thanhtien dòng * %
                    decimal dongThanhTienVND = (item.ThanhTien ?? 0m);
                    if (dongThanhTienVND <= 0m) continue;

                    if (phanTramUuDai > 0)
                    {
                        tienUuDaiVND_TungDon += (dongThanhTienVND * phanTramUuDai / 100m);
                    }
                }

                decimal A_UuDai = 0m;
                if (tienUuDaiVND_TungDon > 0m)
                    A_UuDai = QuyDoi_VND_To_A(tienUuDaiVND_TungDon);

                bool apDungUuDai = (A_UuDai > 0m && soDuUuDaiA >= A_UuDai);

                decimal A_ConLai = 0m;
                if (apDungUuDai)
                {
                    A_ConLai = canTraA_TungDon - A_UuDai;
                    if (A_ConLai < 0m) A_ConLai = 0m;

                    // check đủ Đồng A cho phần còn lại
                    if (A_ConLai > soDuA)
                    {
                        Helper_Tabler_cl.ShowModal(
                            this.Page,
                            $"Bạn đủ ví ưu đãi nhưng Đồng A không đủ cho phần còn lại.<br/>" +
                            $"Cần thêm <b>{A_ConLai:#,##0.##} A</b>, bạn đang có <b>{soDuA:#,##0.##} A</b>.",
                            "Thông báo",
                            true,
                            "danger"
                        );
                        return;
                    }
                }
                else
                {
                    // fallback: trừ toàn bộ Đồng A
                    if (canTraA_TungDon > soDuA)
                    {
                        Helper_Tabler_cl.ShowModal(
                            this.Page,
                            $"Quyền tiêu dùng của bạn không đủ.<br/>" +
                            $"Cần <b>{canTraA_TungDon:#,##0.##} A</b> (≈ {tongVND_TungDon:#,##0}đ), bạn đang có <b>{soDuA:#,##0.##} A</b>.",
                            "Thông báo",
                            true,
                            "danger"
                        );
                        return;
                    }
                }

                // ===================== LƯU ĐƠN =====================
                DonHang_tb _ob = new DonHang_tb();
                _ob.ngaydat = _now;
                _ob.nguoimua = tk;
                _ob.nguoiban = group.NguoiBan;
                _ob.order_status = DonHangStateMachine_cl.Order_DaDat;
                _ob.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
                _ob.tongtien = tongVND_TungDon; // ✅ VNĐ

                _ob.hoten_nguoinhan = txt_hoten_nguoinhan.Text.Trim();
                _ob.sdt_nguoinhan = txt_sdt_nguoinhan.Text.Trim();
                _ob.diahchi_nguoinhan = txt_diachi_nguoinhan.Text.Trim();
                _ob.online_offline = true;
                _ob.chothanhtoan = false;
                DonHangStateMachine_cl.SyncLegacyStatus(_ob);

                db.DonHang_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                string _id_donhang = _ob.id.ToString();

                // ===================== CHI TIẾT + XÓA GIỎ =====================
                foreach (var item in group.BaiViets)
                {
                    DonHang_ChiTiet_tb _ob1 = new DonHang_ChiTiet_tb();
                    _ob1.id_donhang = _id_donhang;
                    _ob1.idsp = item.idsp;
                    _ob1.nguoiban_goc = _ob.nguoiban;
                    _ob1.nguoiban_danglai = item.nguoiban_danglai;
                    _ob1.soluong = item.soluong;

                    _ob1.giaban = item.giaban ?? 0m;                 // ✅ VNĐ
                    _ob1.thanhtien = (_ob1.giaban * item.soluong);   // ✅ VNĐ

                    // Nếu bảng chi tiết của bạn có field này như trang trước thì bật lại:
                    var sp = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == item.idsp && p.bin == false);
                    if (sp != null)
                    {
                        int pt = (sp.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0);
                        if (pt < 0) pt = 0; if (pt > 50) pt = 50;
                        _ob1.PhanTram_GiamGia_ThanhToan_BangEvoucher = pt;
                    }

                    db.DonHang_ChiTiet_tbs.InsertOnSubmit(_ob1);

                    GioHang_tb _ob9 = db.GioHang_tbs.First(p => p.id.ToString() == item.id);
                    db.GioHang_tbs.DeleteOnSubmit(_ob9);
                }

                // ===================== LỊCH SỬ & TRỪ 2 VÍ =====================
                if (apDungUuDai)
                {
                    // 1) Trừ ví ưu đãi (LoaiHoSo_Vi = 2)
                    LichSu_DongA_tb lsUuDai = new LichSu_DongA_tb();
                    lsUuDai.taikhoan = tk;
                    lsUuDai.dongA = A_UuDai;
                    lsUuDai.ngay = _now;
                    lsUuDai.CongTru = false;
                    lsUuDai.id_donhang = _id_donhang;
                    lsUuDai.LoaiHoSo_Vi = 2;
                    lsUuDai.ghichu =
                        $"Ưu đãi đơn {_id_donhang}: {A_UuDai:#,##0.##}A (≈ {tienUuDaiVND_TungDon:#,##0}đ, 1A={TY_GIA_DONGA_VND:#,##0}đ)";
                    db.LichSu_DongA_tbs.InsertOnSubmit(lsUuDai);

                    // trừ số dư ví ưu đãi
                    soDuUuDaiA -= A_UuDai;
                    q_tk.Vi1That_Evocher_30PhanTram = soDuUuDaiA;


                    // 2) Trừ ví Đồng A phần còn lại (LoaiHoSo_Vi = 1)
                    decimal conLaiVND = tongVND_TungDon - tienUuDaiVND_TungDon;

                    LichSu_DongA_tb lsA = new LichSu_DongA_tb();
                    lsA.taikhoan = tk;
                    lsA.dongA = A_ConLai; // ✅ đảm bảo A_UuDai + A_ConLai = canTraA_TungDon
                    lsA.ngay = _now;
                    lsA.CongTru = false;
                    lsA.id_donhang = _id_donhang;
                    lsA.LoaiHoSo_Vi = 1;
                    lsA.ghichu =
                        $"Tạm giữ Trao đổi đơn hàng số {_id_donhang} (phần còn lại): {A_ConLai:#,##0.##}A (≈ {conLaiVND:#,##0}đ, 1A={TY_GIA_DONGA_VND:#,##0}đ)";
                    db.LichSu_DongA_tbs.InsertOnSubmit(lsA);

                    // trừ số dư Đồng A
                    soDuA -= A_ConLai;
                    q_tk.DongA = soDuA;
                }
                else
                {
                    // Fallback: trừ 100% Đồng A (LoaiHoSo_Vi = 1)
                    LichSu_DongA_tb _ob2 = new LichSu_DongA_tb();
                    _ob2.taikhoan = tk;
                    _ob2.dongA = canTraA_TungDon;
                    _ob2.ngay = _now;
                    _ob2.CongTru = false;
                    _ob2.id_donhang = _id_donhang;
                    _ob2.ghichu =
                        $"Tạm giữ Trao đổi đơn hàng số {_id_donhang}: {canTraA_TungDon:#,##0.##}A (≈ {tongVND_TungDon:#,##0}đ, 1A={TY_GIA_DONGA_VND:#,##0}đ)";
                    _ob2.LoaiHoSo_Vi = 1;
                    db.LichSu_DongA_tbs.InsertOnSubmit(_ob2);

                    // trừ số dư Đồng A
                    soDuA -= canTraA_TungDon;
                    q_tk.DongA = soDuA;
                }

                // ===================== THÔNG BÁO =====================
                ThongBao_tb _ob4 = new ThongBao_tb();
                _ob4.id = Guid.NewGuid();
                _ob4.daxem = false;
                _ob4.nguoithongbao = tk;
                _ob4.nguoinhan = _ob.nguoiban;
                _ob4.link = "/home/don-ban.aspx";
                _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == tk).hoten
                                + " vừa đặt hàng đến bạn. ID đơn hàng: " + _id_donhang;
                _ob4.thoigian = AhaTime_cl.Now;
                _ob4.bin = false;
                db.ThongBao_tbs.InsertOnSubmit(_ob4);

                db.SubmitChanges();
            }

            show_main();
            up_main.Update();

            Session["danhSachGomNhom"] = null;
            pn_dathang.Visible = !pn_dathang.Visible;

            Helper_Tabler_cl.ShowModal(
                this.Page,
                "Đặt hàng thành công. Theo dõi đơn hàng tại mục <b>Đơn mua</b> của bạn.",
                "Thông báo",
                true,
                "success"
            );
        }
    }


    #endregion

    protected void Button3_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            int _dem = 0;
            foreach (RepeaterItem item in Repeater1.Items)
            {
                Label lblData = (Label)item.FindControl("lbID");
                TextBox txt_sl_1 = (TextBox)item.FindControl("txt_sl_1");

                if (txt_sl_1 != null && lblData != null)
                {
                    string _id_giohang = lblData.Text;
                    var q = db.GioHang_tbs.FirstOrDefault(p => p.id.ToString() == _id_giohang);
                    if (q != null)
                    {
                        int _sl = Number_cl.Check_Int(txt_sl_1.Text.Trim());
                        if (_sl > 0)
                        {
                            q.soluong = _sl;
                            _dem++;
                        }
                    }
                }
            }

            if (_dem > 0)
            {
                db.SubmitChanges();
                show_main();

                // ✅ đổi thông báo sang Tabler
                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
            }
            else
            {
                // ✅ đổi dialog sang Tabler
                Helper_Tabler_cl.ShowModal(this.Page, "Không có mục nào được chọn.", "Thông báo", true, "warning");
            }
        }
    }
}
