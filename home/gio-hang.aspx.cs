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

    private void BindCheckoutSummary(List<BaiVietViewModel> items)
    {
        if (items == null || items.Count == 0)
        {
            lb_checkout_items.Text = "0";
            lb_checkout_total_vnd.Text = "0";
            lb_checkout_total_vnd_footer.Text = "0";
            lb_checkout_total_a.Text = "0";
            lb_checkout_total_a_footer.Text = "0";
            return;
        }

        int tongSanPham = items.Sum(x => x.soluong);
        decimal tongVnd = items.Sum(x => x.ThanhTien ?? 0m);
        decimal tongA = QuyDoi_VND_To_A(tongVnd);
        string tongAText = tongA.ToString("#,##0.##");

        lb_checkout_items.Text = tongSanPham.ToString("#,##0");
        lb_checkout_total_vnd.Text = tongVnd.ToString("#,##0");
        lb_checkout_total_vnd_footer.Text = tongVnd.ToString("#,##0");
        lb_checkout_total_a.Text = tongAText;
        lb_checkout_total_a_footer.Text = tongAText;
    }

    private int ParseQty(string raw)
    {
        int qty = Number_cl.Check_Int((raw ?? "").Trim());
        if (qty < 1) qty = 1;
        if (qty > 999) qty = 999;
        return qty;
    }

    private string CurrentAccount()
    {
        return ((ViewState["taikhoan"] ?? "").ToString() ?? "").Trim();
    }

    private bool IsCheckoutStepRequest()
    {
        return string.Equals((Request.QueryString["step"] ?? "").Trim(), "2", StringComparison.Ordinal);
    }

    private List<long> ParseSelectedIdsFromQuery()
    {
        string raw = (Request.QueryString["sel"] ?? "").Trim();
        if (string.IsNullOrEmpty(raw))
            return new List<long>();

        return raw
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                long id;
                return long.TryParse((x ?? "").Trim(), out id) ? id : 0;
            })
            .Where(id => id > 0)
            .Distinct()
            .ToList();
    }

    private Dictionary<long, int> ParseQtyMapFromQuery()
    {
        Dictionary<long, int> map = new Dictionary<long, int>();
        string raw = (Request.QueryString["qty"] ?? "").Trim();
        if (string.IsNullOrEmpty(raw))
            return map;

        string[] pairs = raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string pair in pairs)
        {
            if (string.IsNullOrWhiteSpace(pair)) continue;
            string[] parts = pair.Split('-');
            if (parts.Length != 2) continue;

            long id;
            int qty;
            if (!long.TryParse((parts[0] ?? "").Trim(), out id) || id <= 0) continue;
            if (!int.TryParse((parts[1] ?? "").Trim(), out qty)) continue;

            map[id] = ParseQty(qty.ToString());
        }

        return map;
    }

    private sealed class CartRowInput
    {
        public long CartId { get; set; }
        public int Qty { get; set; }
    }

    private List<CartRowInput> ReadCartRowsFromMainRepeater()
    {
        List<CartRowInput> rows = new List<CartRowInput>();

        foreach (RepeaterItem item in Repeater1.Items)
        {
            Label lblData = (Label)item.FindControl("lbID");
            TextBox txtQty = (TextBox)item.FindControl("txt_sl_1");
            if (lblData == null || txtQty == null) continue;

            long cartId;
            if (!long.TryParse((lblData.Text ?? "").Trim(), out cartId) || cartId <= 0)
                continue;

            int qty = ParseQty(txtQty.Text);
            txtQty.Text = qty.ToString();

            rows.Add(new CartRowInput
            {
                CartId = cartId,
                Qty = qty
            });
        }

        return rows;
    }

    private int SyncGridCartQuantity(dbDataContext db, List<CartRowInput> rows)
    {
        if (rows == null || rows.Count == 0)
            return 0;

        List<long> ids = rows.Select(x => x.CartId).Distinct().ToList();
        Dictionary<long, GioHang_tb> cartMap = db.GioHang_tbs
            .Where(p => ids.Contains(p.id))
            .ToDictionary(p => p.id);

        int changed = 0;
        foreach (CartRowInput row in rows)
        {
            GioHang_tb q;
            if (!cartMap.TryGetValue(row.CartId, out q))
                continue;

            if ((q.soluong ?? 0) != row.Qty)
            {
                q.soluong = row.Qty;
                changed++;
            }
        }

        return changed;
    }

    private string BuildReceiverAddress()
    {
        string tinh = (hf_tinh.Value ?? "").Trim();
        string quan = (hf_quan.Value ?? "").Trim();
        string phuong = (hf_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();

        string full = AddressFormat_cl.BuildFullAddress(chiTiet, phuong, quan, tinh);
        txt_diachi_nguoinhan.Text = full;
        return full;
    }

    private bool ValidateReceiverAddress()
    {
        string tinh = (hf_tinh.Value ?? "").Trim();
        string quan = (hf_quan.Value ?? "").Trim();
        string phuong = (hf_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();

        if (string.IsNullOrEmpty(tinh) || string.IsNullOrEmpty(quan) || string.IsNullOrEmpty(phuong))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn đầy đủ Tỉnh/Thành, Quận/Huyện và Phường/Xã.", "Thông báo", true, "warning");
            return false;
        }

        if (chiTiet.Length < 4)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập địa chỉ chi tiết.", "Thông báo", true, "warning");
            return false;
        }

        return true;
    }

    private void SyncCheckoutQuantityFromRepeater(dbDataContext db, List<BaiVietViewModel> danhSach)
    {
        if (danhSach == null || danhSach.Count == 0)
            return;

        Dictionary<string, BaiVietViewModel> vmMap = danhSach.ToDictionary(x => x.id, x => x);
        List<long> ids = vmMap.Keys
            .Select(x =>
            {
                long id;
                return long.TryParse((x ?? "").Trim(), out id) ? id : 0;
            })
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        Dictionary<long, GioHang_tb> cartMap = db.GioHang_tbs
            .Where(p => ids.Contains(p.id))
            .ToDictionary(p => p.id);

        foreach (RepeaterItem item in Repeater2.Items)
        {
            Label lblData = (Label)item.FindControl("lbID");
            TextBox txtQty = (TextBox)item.FindControl("txt_sl_2");
            if (lblData == null || txtQty == null) continue;

            string idGioHang = (lblData.Text ?? "").Trim();
            if (string.IsNullOrEmpty(idGioHang)) continue;

            BaiVietViewModel vm;
            if (!vmMap.TryGetValue(idGioHang, out vm))
                continue;

            long idInt;
            if (!long.TryParse(idGioHang, out idInt) || idInt <= 0)
                continue;

            GioHang_tb q;
            if (!cartMap.TryGetValue(idInt, out q))
                continue;

            if (vm == null) continue;

            int qty = ParseQty(txtQty.Text);
            txtQty.Text = qty.ToString();

            vm.soluong = qty;
            decimal gia = vm.giaban ?? 0m;
            vm.ThanhTien = gia * qty;
            q.soluong = qty;
        }
    }

    private void BindSavedAddresses(dbDataContext db, string taiKhoan)
    {
        if (db == null)
        {
            pnl_saved_address.Visible = false;
            return;
        }

        var list = AddressHistory_cl.GetRecentAddresses(db, taiKhoan, 5, true);
        if (list != null && list.Count > 0)
        {
            rpt_saved_address.DataSource = list;
            rpt_saved_address.DataBind();
            pnl_saved_address.Visible = true;
        }
        else
        {
            pnl_saved_address.Visible = false;
        }
    }

    protected void SavedAddress_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e == null)
            return;

        string command = (e.CommandName ?? "").Trim().ToLowerInvariant();
        if (command != "delete" && command != "set-default")
            return;

        string tk = (ViewState["taikhoan"] ?? "").ToString();
        if (string.IsNullOrEmpty(tk))
            return;

        long id;
        if (!long.TryParse((e.CommandArgument ?? "").ToString(), out id))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            if (command == "delete")
            {
                AddressHistory_cl.DeleteAddress(db, tk, id);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã xoá địa chỉ.", null, true, 2000, "Thông báo");
            }
            else if (command == "set-default")
            {
                AddressHistory_cl.SetDefaultAddress(db, tk, id);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã đặt làm mặc định.", null, true, 2000, "Thông báo");
            }

            BindSavedAddresses(db, tk);
        }
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
            if (IsCheckoutStepRequest())
            {
                List<long> selectedIds = ParseSelectedIdsFromQuery();
                Dictionary<long, int> qtyMap = ParseQtyMapFromQuery();
                using (dbDataContext db = new dbDataContext())
                {
                    if (!TryBuildCheckoutStep(db, selectedIds, qtyMap, true))
                    {
                        pn_step1.Visible = true;
                        pn_dathang.Visible = false;
                        show_main();
                    }
                    else
                    {
                        pn_step1.Visible = false;
                        pn_dathang.Visible = true;
                    }
                }
            }
            else
            {
                pn_step1.Visible = true;
                pn_dathang.Visible = false;
                show_main();
            }
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
                            where ob2 != null
                                  && ob2.bin != true
                                  && db.taikhoan_tbs.Any(acc => acc.taikhoan == ob2.nguoitao && acc.block != true)
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
        public int PhanTramUuDai { get; set; }
        public int soluong { get; set; }
        public decimal? ThanhTien { get; set; } // ✅ VNĐ
        public string TenShop { get; set; }
        public string nguoiban_goc { get; set; }
        public string nguoiban_danglai { get; set; }
        public string idsp { get; set; }
    }

    private bool TryBuildCheckoutStep(dbDataContext db, List<long> selectedCartIds, Dictionary<long, int> qtyMap, bool showEmptyAlert)
    {
        if (selectedCartIds == null || selectedCartIds.Count == 0)
        {
            if (showEmptyAlert)
                Helper_Tabler_cl.ShowModal(this.Page, "Không có mục nào được chọn.", "Thông báo", true, "warning");
            return false;
        }

        string tk = CurrentAccount();
        if (string.IsNullOrEmpty(tk))
            return false;

        if (qtyMap == null)
            qtyMap = new Dictionary<long, int>();

        var selectedRows = (from g in db.GioHang_tbs
                            where selectedCartIds.Contains(g.id) && g.taikhoan == tk
                            join p in db.BaiViet_tbs on g.idsp equals p.id.ToString()
                            join seller in db.taikhoan_tbs on p.nguoitao equals seller.taikhoan
                            join shop in db.taikhoan_tbs on g.nguoiban_goc equals shop.taikhoan into ShopGroup
                            from shop in ShopGroup.DefaultIfEmpty()
                            where p.bin != true
                                  && p.phanloai == "sanpham"
                                  && seller.block != true
                            select new
                            {
                                g.id,
                                g.ngaythem,
                                g.idsp,
                                g.soluong,
                                g.nguoiban_goc,
                                g.nguoiban_danglai,
                                p.image,
                                p.name,
                                p.name_en,
                                p.giaban,
                                PhanTramUuDai = (p.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0),
                                TenShop = (shop == null ? "" : shop.ten_shop)
                            }).ToList();

        if (selectedRows.Count == 0)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không tìm thấy sản phẩm còn hoạt động trong giỏ.", "Thông báo", true, "warning");
            return false;
        }

        List<BaiVietViewModel> list_all_data = selectedRows
            .Select(ob =>
            {
                int soLuong = qtyMap.ContainsKey(ob.id)
                    ? ParseQty(qtyMap[ob.id].ToString())
                    : ParseQty((ob.soluong ?? 1).ToString());

                int phanTramUuDai = ob.PhanTramUuDai;
                if (phanTramUuDai < 0) phanTramUuDai = 0;
                if (phanTramUuDai > 50) phanTramUuDai = 50;

                decimal giaBan = ob.giaban ?? 0m;

                return new BaiVietViewModel
                {
                    id = ob.id.ToString(),
                    ngaythem = ob.ngaythem,
                    image = string.IsNullOrWhiteSpace(ob.image) ? "/uploads/images/macdinh.jpg" : ob.image,
                    name = ob.name,
                    name_en = ob.name_en,
                    giaban = ob.giaban,
                    PhanTramUuDai = phanTramUuDai,
                    soluong = soLuong,
                    ThanhTien = giaBan * soLuong,
                    TenShop = ob.TenShop ?? "",
                    nguoiban_goc = ob.nguoiban_goc,
                    nguoiban_danglai = ob.nguoiban_danglai,
                    idsp = (ob.idsp ?? "").Trim()
                };
            })
            .OrderBy(x => x.nguoiban_goc)
            .ThenByDescending(x => x.ngaythem)
            .ToList();

        if (list_all_data.Count == 0)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán", "Thông báo", true, "warning");
            return false;
        }

        Session["danhSachGomNhom"] = list_all_data;
        BindCheckoutSummary(list_all_data);
        Repeater2.DataSource = list_all_data;
        Repeater2.DataBind();

        var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (q_tk != null)
        {
            txt_hoten_nguoinhan.Text = q_tk.hoten;
            txt_sdt_nguoinhan.Text = q_tk.dienthoai;
            string diaChi = q_tk.diachi ?? "";
            txt_diachi_nguoinhan.Text = diaChi;
            txt_diachi_chitiet.Text = diaChi;
            hf_address_raw.Value = diaChi;
        }

        BindSavedAddresses(db, tk);

        pn_step1.Visible = false;
        pn_dathang.Visible = true;
        return true;
    }

    protected void but_close_form_dathang_Click(object sender, EventArgs e)
    {
        Session["danhSachGomNhom"] = null;
        BindCheckoutSummary(null);
        pn_dathang.Visible = false;
    }

    protected void but_dathang_Click(object sender, EventArgs e)
    {
        if (Session["danhSachGomNhom"] == null) return;

        using (dbDataContext db = new dbDataContext())
        {
            var danhSachGomNhom = Session["danhSachGomNhom"] as List<BaiVietViewModel>;
            if (danhSachGomNhom == null || danhSachGomNhom.Count == 0) return;

            SyncCheckoutQuantityFromRepeater(db, danhSachGomNhom);
            Session["danhSachGomNhom"] = danhSachGomNhom;
            BindCheckoutSummary(danhSachGomNhom);

            // ✅ Check sản phẩm còn bán
            var idspList = danhSachGomNhom.Select(p => p.idsp).ToList();
            bool allStopped = !db.BaiViet_tbs.Any(p =>
                idspList.Contains(p.id.ToString())
                && p.bin == false
                && p.phanloai == "sanpham"
                && db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true));
            if (allStopped)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Có sản phẩm đã ngừng bán", "Thông báo", true, "warning");
                return;
            }

            string tk = (ViewState["taikhoan"] ?? "").ToString();
            if (string.IsNullOrEmpty(tk)) return;

            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (q_tk == null) return;

            if (!ValidateReceiverAddress())
                return;

            string diaChiGiaoHang = BuildReceiverAddress();
            AddressHistory_cl.UpsertAddress(db, tk, (txt_hoten_nguoinhan.Text ?? "").Trim(), (txt_sdt_nguoinhan.Text ?? "").Trim(), diaChiGiaoHang);

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
                    "Quyền tiêu dùng của bạn không đủ để đặt các đơn này.<br/>" +
                    string.Format("Cần <b>{0:#,##0.##} A</b> (≈ {1:#,##0}đ).", tongA_ToiThieuCanCo, tongThanhToan_VND),
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
                    var sp = AccountVisibility_cl.FindVisibleProductById(db, item.idsp);
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
                            "Bạn đủ ví ưu đãi nhưng Đồng A không đủ cho phần còn lại.<br/>" +
                            string.Format("Cần thêm <b>{0:#,##0.##} A</b>, bạn đang có <b>{1:#,##0.##} A</b>.", A_ConLai, soDuA),
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
                            "Quyền tiêu dùng của bạn không đủ.<br/>" +
                            string.Format("Cần <b>{0:#,##0.##} A</b> (≈ {1:#,##0}đ), bạn đang có <b>{2:#,##0.##} A</b>.", canTraA_TungDon, tongVND_TungDon, soDuA),
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
                _ob.diahchi_nguoinhan = diaChiGiaoHang;
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
                    var sp = AccountVisibility_cl.FindVisibleProductById(db, item.idsp);
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
                        string.Format("Ưu đãi đơn {0}: {1:#,##0.##}A (≈ {2:#,##0}đ, 1A={3:#,##0}đ)", _id_donhang, A_UuDai, tienUuDaiVND_TungDon, TY_GIA_DONGA_VND);
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
                        string.Format("Tạm giữ Trao đổi đơn hàng số {0} (phần còn lại): {1:#,##0.##}A (≈ {2:#,##0}đ, 1A={3:#,##0}đ)", _id_donhang, A_ConLai, conLaiVND, TY_GIA_DONGA_VND);
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
                        string.Format("Tạm giữ Trao đổi đơn hàng số {0}: {1:#,##0.##}A (≈ {2:#,##0}đ, 1A={3:#,##0}đ)", _id_donhang, canTraA_TungDon, tongVND_TungDon, TY_GIA_DONGA_VND);
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

            Session["danhSachGomNhom"] = null;
            BindCheckoutSummary(null);
            pn_dathang.Visible = false;

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
            List<CartRowInput> rows = ReadCartRowsFromMainRepeater();
            int _dem = SyncGridCartQuantity(db, rows);

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
