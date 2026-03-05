using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_don_mua : System.Web.UI.Page
{
    DanhMuc_cl dm_cl = new DanhMuc_cl();
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    // ✅ TỶ GIÁ QUY ĐỔI: 1A = 1000 VNĐ
    private const decimal TY_GIA_A_VND = 1000m;
    private const string STATUS_FILTER_KEY = "status_filter_donmua_home";

    private class DonMuaRowVm
    {
        public long id { get; set; }
        public DateTime? ngaydat { get; set; }
        public string TenShop { get; set; }
        public string NguoiMua { get; set; }
        public string trangthai { get; set; }
        public decimal? tongtien { get; set; }
        public string hoten_nguoinhan { get; set; }
        public string sdt_nguoinhan { get; set; }
        public string diahchi_nguoinhan { get; set; }
        public bool? online_offline { get; set; }
        public bool? chothanhtoan { get; set; }
        public string status_group { get; set; }
        public bool show_huydon { get; set; }
        public bool show_danhan { get; set; }
    }

    // ✅ Quy đổi VNĐ -> A (làm tròn lên 2 số lẻ, luôn làm tròn lên để không thiếu)
    private decimal QuyDoi_VND_To_A(decimal vnd)
    {
        if (vnd <= 0) return 0m;
        decimal a = vnd / TY_GIA_A_VND;
        return Math.Ceiling(a * 100m) / 100m;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true); // yêu cầu đăng nhập

            string _tk = Session["taikhoan_home"] as string;

            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }

            set_dulieu_macdinh();
            show_main();
        }
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_donmua_home"] = "1";
        ViewState[STATUS_FILTER_KEY] = "all";
    }

    private string GetCurrentStatusFilter()
    {
        string key = (ViewState[STATUS_FILTER_KEY] ?? "all").ToString();
        switch (key)
        {
            case "da-dat":
            case "cho-trao-doi":
            case "da-trao-doi":
            case "da-giao":
            case "da-nhan":
            case "da-huy":
                return key;
            default:
                return "all";
        }
    }

    private string ResolveStatusGroup(string orderStatus, string exchangeStatus)
    {
        if (orderStatus == DonHangStateMachine_cl.Order_DaHuy) return "da-huy";
        if (orderStatus == DonHangStateMachine_cl.Order_DaNhan) return "da-nhan";
        if (orderStatus == DonHangStateMachine_cl.Order_DaGiao) return "da-giao";
        if (exchangeStatus == DonHangStateMachine_cl.Exchange_DaTraoDoi) return "da-trao-doi";
        if (exchangeStatus == DonHangStateMachine_cl.Exchange_ChoTraoDoi) return "cho-trao-doi";
        return "da-dat";
    }

    private string ResolveStatusFilterLabel(string key)
    {
        switch (key)
        {
            case "da-dat": return "Đã đặt/Chưa Trao đổi";
            case "cho-trao-doi": return "Chờ Trao đổi";
            case "da-trao-doi": return "Đã Trao đổi";
            case "da-giao": return "Đã giao";
            case "da-nhan": return "Đã nhận";
            case "da-huy": return "Đã hủy";
            default: return "Tất cả trạng thái";
        }
    }

    private void CloseChiTietModal()
    {
        Repeater2.DataSource = null;
        Repeater2.DataBind();
        ViewState["iddh"] = null;
        pn_chitiet.Visible = false;
        up_chitiet.Update();
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list_all_query = (from ob1 in db.DonHang_tbs.Where(p => p.nguoimua == ViewState["taikhoan"].ToString())
                                  join ob2 in db.taikhoan_tbs on ob1.nguoiban equals ob2.taikhoan into Group1
                                  from ob2 in Group1.DefaultIfEmpty()
                                  join ob3 in db.taikhoan_tbs on ob1.nguoimua equals ob3.taikhoan into Group2
                                  from ob3 in Group2.DefaultIfEmpty()
                                  select new
                                  {
                                      ob1.id,
                                      ob1.ngaydat,
                                      TenShop = ob2.ten_shop,
                                      NguoiMua = ob3.hoten,
                                      ob1.trangthai,
                                      ob1.order_status,
                                      ob1.exchange_status,
                                      ob1.tongtien,
                                      ob1.hoten_nguoinhan,
                                      ob1.sdt_nguoinhan,
                                      ob1.diahchi_nguoinhan,
                                      ob1.online_offline,
                                      ob1.chothanhtoan,
                                  }).AsQueryable();

            string _key = txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(_key))
                list_all_query = list_all_query.Where(p => p.TenShop.Contains(_key) || p.NguoiMua.Contains(_key) || p.id.ToString() == _key);
            else
            {
                string _key1 = txt_timkiem1.Text.Trim();
                if (!string.IsNullOrEmpty(_key1))
                    list_all_query = list_all_query.Where(p => p.TenShop.Contains(_key1) || p.NguoiMua.Contains(_key1) || p.id.ToString() == _key1);
            }

            var list_all = list_all_query
                .OrderByDescending(p => p.ngaydat)
                .ToList()
                .Select(p =>
                {
                    DonHang_tb dh = new DonHang_tb();
                    dh.trangthai = p.trangthai;
                    dh.order_status = p.order_status;
                    dh.exchange_status = p.exchange_status;
                    dh.online_offline = p.online_offline;

                    DonHangStateMachine_cl.EnsureStateFields(dh);
                    string orderStatus = DonHangStateMachine_cl.GetOrderStatus(dh);
                    string exchangeStatus = DonHangStateMachine_cl.GetExchangeStatus(dh);

                    return new DonMuaRowVm
                    {
                        id = p.id,
                        ngaydat = p.ngaydat,
                        TenShop = p.TenShop ?? "",
                        NguoiMua = p.NguoiMua ?? "",
                        trangthai = DonHangStateMachine_cl.ToLegacyStatus(orderStatus, exchangeStatus, p.online_offline),
                        tongtien = p.tongtien,
                        hoten_nguoinhan = p.hoten_nguoinhan,
                        sdt_nguoinhan = p.sdt_nguoinhan,
                        diahchi_nguoinhan = p.diahchi_nguoinhan,
                        online_offline = p.online_offline,
                        chothanhtoan = p.chothanhtoan,
                        status_group = ResolveStatusGroup(orderStatus, exchangeStatus),
                        show_huydon = DonHangStateMachine_cl.CanCancelOrder(dh),
                        show_danhan = DonHangStateMachine_cl.CanConfirmReceived(dh),
                    };
                })
                .ToList();

            string statusFilter = GetCurrentStatusFilter();
            if (statusFilter != "all")
                list_all = list_all.Where(p => p.status_group == statusFilter).ToList();

            lb_status_filter.Text = ResolveStatusFilterLabel(statusFilter);

            int _Tong_Record = list_all.Count;

            int show = 30; if (show <= 0) show = 30;
            int current_page = int.Parse(ViewState["current_page_donmua_home"].ToString());
            int total_page = number_of_page_class.return_total_page(_Tong_Record, show);
            if (current_page < 1) current_page = 1;
            else if (current_page > total_page) current_page = total_page;

            ViewState["total_page"] = total_page;

            if (current_page >= total_page)
            {
                but_xemtiep.Enabled = false;
                but_xemtiep1.Enabled = false;
            }
            else
            {
                but_xemtiep.Enabled = true;
                but_xemtiep1.Enabled = true;
            }
            if (current_page == 1)
            {
                but_quaylai.Enabled = false;
                but_quaylai1.Enabled = false;
            }
            else
            {
                but_quaylai.Enabled = true;
                but_quaylai1.Enabled = true;
            }

            var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();

            int stt = (show * current_page) - show + 1;
            int _s1 = stt + list_split.Count() - 1;
            if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            else lb_show.Text = "0-0/0";
            lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");

            Repeater1.DataSource = list_split;
            Repeater1.DataBind();
        }
    }

    protected void but_loc_trangthai_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        LinkButton button = (LinkButton)sender;
        ViewState[STATUS_FILTER_KEY] = (button.CommandArgument ?? "all").ToString();
        ViewState["current_page_donmua_home"] = 1;
        show_main();
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_donmua_home"] = int.Parse(ViewState["current_page_donmua_home"].ToString()) - 1;
        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_donmua_home"] = int.Parse(ViewState["current_page_donmua_home"].ToString()) + 1;
        show_main();
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_donmua_home"] = 1;
        show_main();
    }
    #endregion

    #region chi tiết đơn hàng
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _iddh = button.CommandArgument;

            var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == _iddh);
            if (q != null)
            {
                but_danhanhang.Visible = false;
                but_huydonhang.Visible = false;

                ViewState["iddh"] = _iddh;

                var q_ct = from ob1 in db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == _iddh)
                           join ob2 in db.BaiViet_tbs on ob1.idsp equals ob2.id.ToString() into SanPhamGroup
                           from ob2 in SanPhamGroup.DefaultIfEmpty()
                           select new
                           {
                               ob1.id,
                               ob1.id_donhang,
                               name = ob2 != null ? ob2.name : "",
                               ob2.name_en,
                               image = ob2 != null ? ob2.image : "",
                               PhanTram_GiamGia_ThanhToan_BangEvoucher = ob1.PhanTram_GiamGia_ThanhToan_BangEvoucher, // ✅ NEW
                               ob1.giaban,
                               ob1.soluong,
                               ob1.thanhtien,
                           };

                Repeater2.DataSource = q_ct;
                Repeater2.DataBind();

                pn_chitiet.Visible = !pn_chitiet.Visible;
                up_chitiet.Update();
            }
        }
    }

    protected void but_close_form_chitiet_Click(object sender, EventArgs e)
    {
        CloseChiTietModal();
    }

    protected void but_danhanhang_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string iddh = (ViewState["iddh"] ?? "").ToString();
            if (string.IsNullOrEmpty(iddh))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tìm thấy ID đơn hàng.", "Thông báo", true, "warning");
                return;
            }

            var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == iddh);
            if (q != null)
            {
                DonHangStateMachine_cl.EnsureStateFields(q);

                if (!DonHangStateMachine_cl.CanConfirmReceived(q))
                {
                    string msg = DonHangStateMachine_cl.IsTerminal(q)
                        ? "Không thể xác nhận đơn hàng ở trạng thái kết thúc."
                        : "Chỉ có thể xác nhận đã nhận hàng khi đơn ở trạng thái Đã giao.";
                    Helper_Tabler_cl.ShowModal(this.Page, msg, "Thông báo", true, "warning");
                    return;
                }

                DonHangStateMachine_cl.SetOrderStatus(q, DonHangStateMachine_cl.Order_DaNhan);
                DonHangStateMachine_cl.SetExchangeStatus(q, DonHangStateMachine_cl.Exchange_DaTraoDoi);

                if (q.online_offline == true)
                {
                    // ====== LẤY LỊCH SỬ TRỪ CỦA NGƯỜI MUA (CongTru=false) ======
                    var listTru = db.LichSu_DongA_tbs
                        .Where(x => x.id_donhang == iddh
                                && x.taikhoan == q.nguoimua
                                && x.CongTru == false)
                        .ToList();

                    if (listTru == null || listTru.Count == 0)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page,
                            "Không tìm thấy lịch sử trừ tiền của đơn này (CongTru=false). Không thể cộng tiền cho người bán.",
                            "Thông báo", true, "danger");
                        return;
                    }

                    decimal congViTieuDung = listTru
                        .Where(x => (x.LoaiHoSo_Vi ?? 1) == 1)
                        .Sum(x => x.dongA ?? 0m);

                    decimal congViUuDai30 = listTru
                        .Where(x => (x.LoaiHoSo_Vi ?? 1) == 2)
                        .Sum(x => x.dongA ?? 0m);

                    // ====== CỘNG CHO NGƯỜI BÁN (ĐỔI FIELD) ======
                    // DongA -> HoSo_TieuDung_ShopOnly
                    // DuVi1_Evocher_30PhanTram -> HoSo_UuDai_ShopOnly
                    var seller = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == q.nguoiban);
                    if (seller != null)
                    {
                        if (congViTieuDung > 0)
                            seller.HoSo_TieuDung_ShopOnly = (seller.HoSo_TieuDung_ShopOnly ?? 0m) + congViTieuDung;

                        if (congViUuDai30 > 0)
                            seller.HoSo_UuDai_ShopOnly = (seller.HoSo_UuDai_ShopOnly ?? 0m) + congViUuDai30;
                    }

                    // ====== LỊCH SỬ CỘNG TIỀN CHO NGƯỜI BÁN (THÊM TAG ĐỂ LỌC) ======
                    // Tag gợi ý: "|SHOPONLY|CREDIT_SELLER|"
                    DateTime now = AhaTime_cl.Now;

                    if (congViUuDai30 > 0)
                    {
                        LichSu_DongA_tb ls2 = new LichSu_DongA_tb();
                        ls2.taikhoan = q.nguoiban;
                        ls2.dongA = congViUuDai30;
                        ls2.ngay = now;
                        ls2.CongTru = true;
                        ls2.id_donhang = iddh;
                        ls2.ghichu = $"|SHOPONLY|CREDIT_SELLER| Bán đơn hàng số {iddh} (Hồ sơ ưu đãi ShopOnly)";
                        ls2.LoaiHoSo_Vi = 2;
                        db.LichSu_DongA_tbs.InsertOnSubmit(ls2);
                    }

                    if (congViTieuDung > 0)
                    {
                        LichSu_DongA_tb ls1 = new LichSu_DongA_tb();
                        ls1.taikhoan = q.nguoiban;
                        ls1.dongA = congViTieuDung;
                        ls1.ngay = now;
                        ls1.CongTru = true;
                        ls1.id_donhang = iddh;
                        ls1.ghichu = $"|SHOPONLY|CREDIT_SELLER| Bán đơn hàng số {iddh} (Hồ sơ tiêu dùng ShopOnly)";
                        ls1.LoaiHoSo_Vi = 1;
                        db.LichSu_DongA_tbs.InsertOnSubmit(ls1);
                    }

                    // ====== (GIỮ NGUYÊN) CẬP NHẬT GHI CHÚ LỊCH SỬ CỦA NGƯỜI MUA ======
                    var q_lichsu = db.LichSu_DongA_tbs.FirstOrDefault(p => p.id_donhang == iddh
                                                                       && p.taikhoan == q.nguoimua);
                    if (q_lichsu != null)
                        q_lichsu.ghichu = "Trao đổi đơn hàng số " + iddh;

                    // ====== (GIỮ NGUYÊN) CỘNG SỐ LƯỢNG ĐÃ BÁN ======
                    var q_ct = db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == iddh).ToList();
                    foreach (var item in q_ct)
                    {
                        var q_bv = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == item.idsp);
                        if (q_bv != null)
                        {
                            q_bv.soluong_daban = (q_bv.soluong_daban ?? 0) + item.soluong;
                        }
                    }
                }

                ThongBao_tb _ob4 = new ThongBao_tb();
                _ob4.id = Guid.NewGuid();
                _ob4.daxem = false;
                _ob4.nguoithongbao = ViewState["taikhoan"].ToString();
                _ob4.nguoinhan = q.nguoiban;
                _ob4.link = "/home/don-ban.aspx";
                _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).hoten
                             + " đã nhận đơn hàng số " + iddh;
                _ob4.thoigian = AhaTime_cl.Now;
                _ob4.bin = false;
                db.ThongBao_tbs.InsertOnSubmit(_ob4);

                db.SubmitChanges();

                show_main();
                up_main.Update();
                CloseChiTietModal();

                Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
            }
        }
    }

    protected void but_huydonhang_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string iddh = (ViewState["iddh"] ?? "").ToString();
            if (string.IsNullOrEmpty(iddh))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tìm thấy ID đơn hàng.", "Thông báo", true, "warning");
                return;
            }

            var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == iddh);
            if (q == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Đơn hàng không tồn tại.", "Thông báo", true, "warning");
                return;
            }

            DonHangStateMachine_cl.EnsureStateFields(q);

            if (!DonHangStateMachine_cl.CanCancelOrder(q))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không thể hủy đơn hàng này.", "Thông báo", true, "warning");
                return;
            }

            // ===== 1) LẤY LỊCH SỬ TRỪ (CongTru=false) =====
            var listTru = db.LichSu_DongA_tbs
                .Where(x => x.id_donhang == iddh && x.taikhoan == q.nguoimua && x.CongTru == false)
                .ToList();

            if (listTru == null || listTru.Count == 0)
            {
                Helper_Tabler_cl.ShowModal(
                    this.Page,
                    "Không tìm thấy lịch sử trừ tiền của đơn này (CongTru=false). Không thể hoàn tiền tự động.",
                    "Thông báo",
                    true,
                    "danger"
                );
                return;
            }

            // ===== 2) TÍNH TỔNG HOÀN THEO TỪNG Hồ sơ =====
            decimal hoanViTieuDung = listTru
                .Where(x => (x.LoaiHoSo_Vi ?? 1) == 1)
                .Sum(x => x.dongA ?? 0m);

            decimal hoanViUuDai30 = listTru
                .Where(x => (x.LoaiHoSo_Vi ?? 1) == 2)
                .Sum(x => x.dongA ?? 0m);

            decimal tongHoanA = hoanViTieuDung + hoanViUuDai30;

            // ===== 3) HOÀN VỀ ĐÚNG 2 Hồ sơ CỦA NGƯỜI MUA =====
            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == q.nguoimua);
            if (q_tk == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tìm thấy tài khoản người mua.", "Thông báo", true, "danger");
                return;
            }

            if (hoanViTieuDung > 0)
                q_tk.DongA = (q_tk.DongA ?? 0m) + hoanViTieuDung;

            // ✅ THEO Ý BẠN: HOÀN ƯU ĐÃI PHẢI VỀ TRƯỜNG MỚI (vì lúc trừ đã trừ trường mới)
            if (hoanViUuDai30 > 0)
                q_tk.Vi1That_Evocher_30PhanTram = (q_tk.Vi1That_Evocher_30PhanTram ?? 0m) + hoanViUuDai30;

            // ===== 4) UPDATE TRẠNG THÁI ĐƠN =====
            DonHangStateMachine_cl.SetOrderStatus(q, DonHangStateMachine_cl.Order_DaHuy);

            // ===== 5) LỊCH SỬ HOÀN (CongTru=true) =====
            DateTime now = AhaTime_cl.Now;

            if (hoanViUuDai30 > 0)
            {
                LichSu_DongA_tb lsHoan2 = new LichSu_DongA_tb();
                lsHoan2.taikhoan = q.nguoimua;
                lsHoan2.dongA = hoanViUuDai30;
                lsHoan2.ngay = now;
                lsHoan2.CongTru = true;
                lsHoan2.id_donhang = iddh;
                lsHoan2.LoaiHoSo_Vi = 2;
                lsHoan2.ghichu = $"Hoàn Hồ sơ ưu đãi đơn {iddh}: +{hoanViUuDai30:#,##0.##} Quyền ưu đãi";
                db.LichSu_DongA_tbs.InsertOnSubmit(lsHoan2);
            }

            if (hoanViTieuDung > 0)
            {
                LichSu_DongA_tb lsHoan1 = new LichSu_DongA_tb();
                lsHoan1.taikhoan = q.nguoimua;
                lsHoan1.dongA = hoanViTieuDung;
                lsHoan1.ngay = now;
                lsHoan1.CongTru = true;
                lsHoan1.id_donhang = iddh;
                lsHoan1.LoaiHoSo_Vi = 1;
                lsHoan1.ghichu = $"Hoàn Hồ sơ tiêu dùng đơn {iddh}: +{hoanViTieuDung:#,##0.##} Quyền tiêu dùng";
                db.LichSu_DongA_tbs.InsertOnSubmit(lsHoan1);
            }

            // ===== 6) THÔNG BÁO SHOP =====
            ThongBao_tb _ob4 = new ThongBao_tb();
            _ob4.id = Guid.NewGuid();
            _ob4.daxem = false;
            _ob4.nguoithongbao = ViewState["taikhoan"].ToString();
            _ob4.nguoinhan = q.nguoiban;
            _ob4.link = "/home/don-ban.aspx";
            _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).hoten
                         + " vừa hủy đơn. ID đơn hàng: " + iddh;
            _ob4.thoigian = now;
            _ob4.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(_ob4);

            db.SubmitChanges();

            // ===== 7) UI =====
            show_main();
            up_main.Update();
            CloseChiTietModal();

            string msg =
                "Hủy đơn thành công.<br/>" +
                $"ID đơn: <b>{iddh}</b><br/>" +
                $"Đã hoàn tổng: <b>{tongHoanA:#,##0.##} Quyền</b><br/>" +
                $"- Hồ sơ ưu đãi: <b>+{hoanViUuDai30:#,##0.##} Quyền ưu đãi</b><br/>" +
                $"- Hồ sơ tiêu dùng: <b>+{hoanViTieuDung:#,##0.##} Quyền tiêu dùng</b>";

            Helper_Tabler_cl.ShowModal(this.Page, msg, "Thông báo", true, "success");
        }
    }

    protected void but_danhanhang_row_Click(object sender, EventArgs e)
    {
        LinkButton button = (LinkButton)sender;
        ViewState["iddh"] = button.CommandArgument;
        but_danhanhang_Click(sender, EventArgs.Empty);
        if (pn_chitiet.Visible) CloseChiTietModal();
    }

    protected void but_huydonhang_row_Click(object sender, EventArgs e)
    {
        LinkButton button = (LinkButton)sender;
        ViewState["iddh"] = button.CommandArgument;
        but_huydonhang_Click(sender, EventArgs.Empty);
        if (pn_chitiet.Visible) CloseChiTietModal();
    }
    #endregion
}
