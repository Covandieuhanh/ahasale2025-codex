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
    private const string CANCEL_NOTICE_SESSION = "donmua_cancel_notice";

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

            if (TryHandleCancelRequest())
                return;

            set_dulieu_macdinh();
            show_main();
            ShowCancelNotice();
        }
    }

    private string GetCurrentHomeAccount()
    {
        string tk = (ViewState["taikhoan"] ?? "").ToString();
        if (string.IsNullOrEmpty(tk))
        {
            string enc = Session["taikhoan_home"] as string;
            if (!string.IsNullOrEmpty(enc))
                tk = mahoa_cl.giaima_Bcorn(enc);
        }
        return tk ?? "";
    }

    private bool TryHandleCancelRequest()
    {
        string cancel = (Request.QueryString["cancel"] ?? "").Trim();
        string iddh = (Request.QueryString["id"] ?? "").Trim();
        if (!string.Equals(cancel, "1", StringComparison.Ordinal) || string.IsNullOrEmpty(iddh))
            return false;

        using (dbDataContext db = new dbDataContext())
        {
            string msg, type;
            TryCancelOrderCore(db, iddh, out msg, out type);
            Session[CANCEL_NOTICE_SESSION] = (type ?? "warning") + "|" + (msg ?? "Không thể hủy đơn hàng này.");
        }

        Response.Redirect("/home/don-mua.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
        return true;
    }

    private void ShowCancelNotice()
    {
        string raw = Session[CANCEL_NOTICE_SESSION] as string;
        if (string.IsNullOrEmpty(raw))
            return;
        Session[CANCEL_NOTICE_SESSION] = null;

        string type = "warning";
        string msg = raw;
        int idx = raw.IndexOf('|');
        if (idx > 0)
        {
            type = raw.Substring(0, idx);
            msg = raw.Substring(idx + 1);
        }
        Helper_Tabler_cl.ShowModal(this.Page, msg, "Thông báo", true, type);
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

    private void SyncStatusFilterDropdown(string key)
    {
        if (ddl_status_filter == null) return;
        var item = ddl_status_filter.Items.FindByValue(key);
        if (item == null) return;
        ddl_status_filter.ClearSelection();
        item.Selected = true;
    }

    protected string BuildOrderDetailUrl(object orderIdObj)
    {
        string id = (orderIdObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(id))
            return "#";

        string back = "/home/don-mua.aspx";
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["id"] = id;
        query["mode"] = "buy";
        query["return_url"] = back;
        return "/home/don-chi-tiet.aspx?" + query.ToString();
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
            SyncStatusFilterDropdown(statusFilter);

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

    protected void ddl_status_filter_SelectedIndexChanged(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState[STATUS_FILTER_KEY] = (ddl_status_filter.SelectedValue ?? "all").ToString();
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
                        ls2.ghichu = string.Format("|SHOPONLY|CREDIT_SELLER| Bán đơn hàng số {0} (Hồ sơ ưu đãi ShopOnly)", iddh);
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
                        ls1.ghichu = string.Format("|SHOPONLY|CREDIT_SELLER| Bán đơn hàng số {0} (Hồ sơ tiêu dùng ShopOnly)", iddh);
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

                Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
            }
        }
    }

    protected void but_huydonhang_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string iddh = (ViewState["iddh"] ?? "").ToString();
            string msg;
            string type;
            bool ok = TryCancelOrderCore(db, iddh, out msg, out type);

            show_main();
            up_main.Update();

            Helper_Tabler_cl.ShowModal(this.Page, msg, "Thông báo", true, type);
        }
    }

    protected void but_danhanhang_row_Click(object sender, EventArgs e)
    {
        LinkButton button = (LinkButton)sender;
        ViewState["iddh"] = button.CommandArgument;
        but_danhanhang_Click(sender, EventArgs.Empty);
    }

    protected void but_huydonhang_row_Click(object sender, EventArgs e)
    {
        LinkButton button = (LinkButton)sender;
        ViewState["iddh"] = button.CommandArgument;
        but_huydonhang_Click(sender, EventArgs.Empty);
    }
    #endregion

    private bool TryCancelOrderCore(dbDataContext db, string iddh, out string message, out string type)
    {
        message = "Không thể hủy đơn hàng này.";
        type = "warning";

        string currentTk = GetCurrentHomeAccount();

        if (string.IsNullOrEmpty(iddh))
        {
            message = "Không tìm thấy ID đơn hàng.";
            return false;
        }

        var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == iddh);
        if (q == null)
        {
            message = "Đơn hàng không tồn tại.";
            return false;
        }

        DonHangStateMachine_cl.EnsureStateFields(q);

        if (!DonHangStateMachine_cl.CanCancelOrder(q))
        {
            message = "Không thể hủy đơn hàng này.";
            return false;
        }

        var listTru = db.LichSu_DongA_tbs
            .Where(x => x.id_donhang == iddh && x.taikhoan == q.nguoimua && x.CongTru == false)
            .ToList();

        if (listTru == null || listTru.Count == 0)
        {
            message = "Không tìm thấy lịch sử trừ tiền của đơn này (CongTru=false). Không thể hoàn tiền tự động.";
            type = "danger";
            return false;
        }

        decimal hoanViTieuDung = listTru
            .Where(x => (x.LoaiHoSo_Vi ?? 1) == 1)
            .Sum(x => x.dongA ?? 0m);

        decimal hoanViUuDai30 = listTru
            .Where(x => (x.LoaiHoSo_Vi ?? 1) == 2)
            .Sum(x => x.dongA ?? 0m);

        decimal tongHoanA = hoanViTieuDung + hoanViUuDai30;

        var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == q.nguoimua);
        if (q_tk == null)
        {
            message = "Không tìm thấy tài khoản người mua.";
            type = "danger";
            return false;
        }

        if (hoanViTieuDung > 0)
            q_tk.DongA = (q_tk.DongA ?? 0m) + hoanViTieuDung;

        if (hoanViUuDai30 > 0)
            q_tk.Vi1That_Evocher_30PhanTram = (q_tk.Vi1That_Evocher_30PhanTram ?? 0m) + hoanViUuDai30;

        DonHangStateMachine_cl.SetOrderStatus(q, DonHangStateMachine_cl.Order_DaHuy);

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
            lsHoan2.ghichu = string.Format("Hoàn Hồ sơ ưu đãi đơn {0}: +{1:#,##0.##} Quyền ưu đãi", iddh, hoanViUuDai30);
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
            lsHoan1.ghichu = string.Format("Hoàn Hồ sơ tiêu dùng đơn {0}: +{1:#,##0.##} Quyền tiêu dùng", iddh, hoanViTieuDung);
            db.LichSu_DongA_tbs.InsertOnSubmit(lsHoan1);
        }

        if (!string.IsNullOrEmpty(currentTk))
        {
            ThongBao_tb _ob4 = new ThongBao_tb();
            _ob4.id = Guid.NewGuid();
            _ob4.daxem = false;
            _ob4.nguoithongbao = currentTk;
            _ob4.nguoinhan = q.nguoiban;
            _ob4.link = "/home/don-ban.aspx";
            _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == currentTk).hoten
                         + " vừa hủy đơn. ID đơn hàng: " + iddh;
            _ob4.thoigian = now;
            _ob4.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(_ob4);
        }

        db.SubmitChanges();

        message =
            "Hủy đơn thành công.<br/>" +
            string.Format("ID đơn: <b>{0}</b><br/>", iddh) +
            string.Format("Đã hoàn tổng: <b>{0:#,##0.##} Quyền</b><br/>", tongHoanA) +
            string.Format("- Hồ sơ ưu đãi: <b>+{0:#,##0.##} Quyền ưu đãi</b><br/>", hoanViUuDai30) +
            string.Format("- Hồ sơ tiêu dùng: <b>+{0:#,##0.##} Quyền tiêu dùng</b>", hoanViTieuDung);
        type = "success";
        return true;
    }
}
