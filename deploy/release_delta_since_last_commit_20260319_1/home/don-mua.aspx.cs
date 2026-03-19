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
        public string NguoiBan { get; set; }
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
        public bool is_completed { get; set; }
        public bool show_review { get; set; }
        public string review_url { get; set; }
        public int pending_review { get; set; }
        public string first_item_name { get; set; }
        public string first_item_image { get; set; }
        public int first_item_qty { get; set; }
        public decimal? first_item_price { get; set; }
        public int total_items { get; set; }
        public bool show_rebuy { get; set; }
        public string rebuy_label { get; set; }
        public string rebuy_url { get; set; }
        public string more_items_label { get; set; }
    }

    private class DonMuaQueryRow
    {
        public long id { get; set; }
        public DateTime? ngaydat { get; set; }
        public string TenShop { get; set; }
        public string NguoiBan { get; set; }
        public string NguoiMua { get; set; }
        public string trangthai { get; set; }
        public string order_status { get; set; }
        public string exchange_status { get; set; }
        public decimal? tongtien { get; set; }
        public string hoten_nguoinhan { get; set; }
        public string sdt_nguoinhan { get; set; }
        public string diahchi_nguoinhan { get; set; }
        public bool? online_offline { get; set; }
        public bool? chothanhtoan { get; set; }
    }

    private class OrderItemRow
    {
        public string id_donhang { get; set; }
        public string idsp { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int? soluong { get; set; }
        public decimal? giaban { get; set; }
        public string phanloai { get; set; }
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
            ApplyQueryFilters();
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

    private static bool IsCompletedExchange(string orderStatus, string exchangeStatus)
    {
        if (string.Equals(exchangeStatus, DonHangStateMachine_cl.Exchange_DaTraoDoi, StringComparison.OrdinalIgnoreCase))
            return true;
        if (string.Equals(orderStatus, DonHangStateMachine_cl.Order_DaNhan, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    private string BuildOrderReviewUrl(long id)
    {
        string back = "/home/don-mua.aspx";
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["id"] = id.ToString();
        query["mode"] = "buy";
        query["review"] = "1";
        query["return_url"] = back;
        return "/home/don-chi-tiet.aspx?" + query.ToString();
    }

    private static bool IsServiceType(string phanloai)
    {
        return string.Equals((phanloai ?? "").Trim(), AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase);
    }

    private string BuildRebuyUrl(string idsp, string sellerAccount, bool isService)
    {
        string safeId = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(safeId))
            return "#";

        string seller = (sellerAccount ?? "").Trim();
        string returnUrl = HttpUtility.UrlEncode("/home/don-mua.aspx");

        if (isService)
        {
            string url = "/home/dat-lich.aspx?id=" + HttpUtility.UrlEncode(safeId);
            if (!string.IsNullOrWhiteSpace(seller))
                url += "&user=" + HttpUtility.UrlEncode(seller);
            url += "&return_url=" + returnUrl;
            return url;
        }

        string orderUrl = "/home/trao-doi.aspx?idsp=" + HttpUtility.UrlEncode(safeId) + "&qty=1";
        if (!string.IsNullOrWhiteSpace(seller))
            orderUrl += "&user_bancheo=" + HttpUtility.UrlEncode(seller);
        orderUrl += "&return_url=" + returnUrl;
        return orderUrl;
    }

    private string ResolveImagePath(string imageRaw)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        string image = (imageRaw ?? "").Trim();
        if (string.IsNullOrEmpty(image))
            return fallback;

        if (image.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(image, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        if (image.StartsWith("~/", StringComparison.Ordinal))
            image = image.Substring(1);
        if (!image.StartsWith("/", StringComparison.Ordinal))
            image = "/" + image;

        if (Helper_cl.IsMissingUploadFile(image))
            return fallback;

        return image;
    }

    protected string FormatVnd(object valueRaw)
    {
        decimal value = 0m;
        try
        {
            if (valueRaw != null && valueRaw != DBNull.Value)
                value = Convert.ToDecimal(valueRaw);
        }
        catch
        {
            value = 0m;
        }
        return value.ToString("#,##0");
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

    private bool HasColumn(dbDataContext db, string table, string column)
    {
        try
        {
            string sql = "SELECT COL_LENGTH('" + table + "', '" + column + "')";
            int? len = db.ExecuteQuery<int?>(sql).FirstOrDefault();
            return len.HasValue;
        }
        catch
        {
            return false;
        }
    }

    private bool HasOrderStateColumns(dbDataContext db)
    {
        bool hasOrder = HasColumn(db, "dbo.DonHang_tb", "order_status");
        bool hasExchange = HasColumn(db, "dbo.DonHang_tb", "exchange_status");
        return hasOrder && hasExchange;
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

    private string NormalizeStatusKey(string raw)
    {
        string key = (raw ?? "").Trim().ToLowerInvariant();
        switch (key)
        {
            case "all":
            case "da-dat":
            case "cho-trao-doi":
            case "da-trao-doi":
            case "da-giao":
            case "da-nhan":
            case "da-huy":
                return key;
            default:
                return "";
        }
    }

    private void ApplyQueryFilters()
    {
        string status = NormalizeStatusKey(Request.QueryString["status"]);
        if (!string.IsNullOrEmpty(status))
        {
            ViewState[STATUS_FILTER_KEY] = status;
            ViewState["current_page_donmua_home"] = "1";
        }

        string q = (Request.QueryString["q"] ?? "").Trim();
        if (!string.IsNullOrEmpty(q))
        {
            if (txt_timkiem1 != null) txt_timkiem1.Text = q;
            if (txt_timkiem != null) txt_timkiem.Text = q;
            ViewState["current_page_donmua_home"] = "1";
        }
    }

    protected string GetCurrentStatusFilter()
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

    protected string GetCurrentSearchQuery()
    {
        string q = (Request.QueryString["q"] ?? "").Trim();
        if (!string.IsNullOrEmpty(q)) return q;
        q = (txt_timkiem1 != null ? (txt_timkiem1.Text ?? "") : "").Trim();
        if (!string.IsNullOrEmpty(q)) return q;
        q = (txt_timkiem != null ? (txt_timkiem.Text ?? "") : "").Trim();
        return q;
    }

    protected string BuildStatusTabUrl(string statusKey)
    {
        string key = NormalizeStatusKey(statusKey);
        if (string.IsNullOrEmpty(key))
            key = "all";

        var qs = HttpUtility.ParseQueryString(string.Empty);
        qs["status"] = key;

        string q = GetCurrentSearchQuery();
        if (!string.IsNullOrWhiteSpace(q))
            qs["q"] = q;

        return "/home/don-mua.aspx?" + qs.ToString();
    }

    protected string GetMobileTabClass(string statusKey)
    {
        string current = GetCurrentStatusFilter();
        string key = NormalizeStatusKey(statusKey);
        if (string.IsNullOrEmpty(key))
            key = "all";
        return string.Equals(current, key, StringComparison.OrdinalIgnoreCase)
            ? "mobile-tab active"
            : "mobile-tab";
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            bool hasStateCols = HasOrderStateColumns(db);

            IQueryable<DonMuaQueryRow> list_all_query;
            if (hasStateCols)
            {
                list_all_query = (from ob1 in db.DonHang_tbs.Where(p => p.nguoimua == ViewState["taikhoan"].ToString())
                                  join ob2 in db.taikhoan_tbs on ob1.nguoiban equals ob2.taikhoan into Group1
                                  from ob2 in Group1.DefaultIfEmpty()
                                  join ob3 in db.taikhoan_tbs on ob1.nguoimua equals ob3.taikhoan into Group2
                                  from ob3 in Group2.DefaultIfEmpty()
                                  select new DonMuaQueryRow
                                  {
                                      id = ob1.id,
                                      ngaydat = ob1.ngaydat,
                                      TenShop = ob2.ten_shop,
                                      NguoiBan = ob1.nguoiban,
                                      NguoiMua = ob3.hoten,
                                      trangthai = ob1.trangthai,
                                      order_status = ob1.order_status,
                                      exchange_status = ob1.exchange_status,
                                      tongtien = ob1.tongtien,
                                      hoten_nguoinhan = ob1.hoten_nguoinhan,
                                      sdt_nguoinhan = ob1.sdt_nguoinhan,
                                      diahchi_nguoinhan = ob1.diahchi_nguoinhan,
                                      online_offline = ob1.online_offline,
                                      chothanhtoan = ob1.chothanhtoan,
                                  }).AsQueryable();
            }
            else
            {
                list_all_query = (from ob1 in db.DonHang_tbs.Where(p => p.nguoimua == ViewState["taikhoan"].ToString())
                                  join ob2 in db.taikhoan_tbs on ob1.nguoiban equals ob2.taikhoan into Group1
                                  from ob2 in Group1.DefaultIfEmpty()
                                  join ob3 in db.taikhoan_tbs on ob1.nguoimua equals ob3.taikhoan into Group2
                                  from ob3 in Group2.DefaultIfEmpty()
                                  select new DonMuaQueryRow
                                  {
                                      id = ob1.id,
                                      ngaydat = ob1.ngaydat,
                                      TenShop = ob2.ten_shop,
                                      NguoiBan = ob1.nguoiban,
                                      NguoiMua = ob3.hoten,
                                      trangthai = ob1.trangthai,
                                      order_status = null,
                                      exchange_status = null,
                                      tongtien = ob1.tongtien,
                                      hoten_nguoinhan = ob1.hoten_nguoinhan,
                                      sdt_nguoinhan = ob1.sdt_nguoinhan,
                                      diahchi_nguoinhan = ob1.diahchi_nguoinhan,
                                      online_offline = ob1.online_offline,
                                      chothanhtoan = ob1.chothanhtoan,
                                  }).AsQueryable();
            }

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
                    bool isCompleted = IsCompletedExchange(orderStatus, exchangeStatus);

                    return new DonMuaRowVm
                    {
                        id = p.id,
                        ngaydat = p.ngaydat,
                        TenShop = p.TenShop ?? "",
                        NguoiBan = p.NguoiBan ?? "",
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
                        is_completed = isCompleted,
                    };
                })
                .ToList();

            string statusFilter = GetCurrentStatusFilter();
            if (statusFilter != "all")
                list_all = list_all.Where(p => p.status_group == statusFilter).ToList();

            lb_status_filter.Text = ResolveStatusFilterLabel(statusFilter);
            SyncStatusFilterDropdown(statusFilter);

            string currentTk = GetCurrentHomeAccount();
            if (list_all.Count > 0 && !string.IsNullOrWhiteSpace(currentTk))
            {
                var orderIds = list_all.Select(p => p.id.ToString()).Distinct().ToList();
                var detailRows = (from ct in db.DonHang_ChiTiet_tbs
                                  join sp in db.BaiViet_tbs on ct.idsp equals sp.id.ToString() into spGroup
                                  from sp in spGroup.DefaultIfEmpty()
                                  where orderIds.Contains(ct.id_donhang)
                                  select new
                                  {
                                      ct.id_donhang,
                                      ct.idsp,
                                      name = sp != null ? sp.name : "",
                                      image = sp != null ? sp.image : "/uploads/images/macdinh.jpg",
                                      ct.soluong,
                                      ct.giaban,
                                      phanloai = sp != null ? sp.phanloai : ""
                                  }).ToList()
                                  .Select(p => new OrderItemRow
                                  {
                                      id_donhang = p.id_donhang,
                                      idsp = p.idsp,
                                      name = p.name,
                                      image = p.image,
                                      soluong = p.soluong,
                                      giaban = p.giaban,
                                      phanloai = p.phanloai
                                  }).ToList();

                var orderItemMap = detailRows
                    .Where(p => !string.IsNullOrWhiteSpace(p.id_donhang) && !string.IsNullOrWhiteSpace(p.idsp))
                    .GroupBy(p => p.id_donhang)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToList()
                    );

                var allItemIds = orderItemMap.SelectMany(kv => kv.Value.Select(x => (x.idsp ?? "").Trim())).Distinct().ToList();
                var reviewedSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (allItemIds.Count > 0)
                {
                    reviewedSet = new HashSet<string>(
                        db.DanhGiaBaiViets
                          .Where(dg => dg.TaiKhoanDanhGia == currentTk && allItemIds.Contains(dg.idBaiViet))
                          .Select(dg => dg.idBaiViet)
                          .ToList(),
                        StringComparer.OrdinalIgnoreCase
                    );
                }

                foreach (var row in list_all)
                {
                    List<OrderItemRow> items;
                    if (!orderItemMap.TryGetValue(row.id.ToString(), out items))
                        items = new List<OrderItemRow>();

                    int pending = items.Count(item => !reviewedSet.Contains(((item.idsp ?? "").ToString().Trim())));
                    row.pending_review = pending;
                    row.show_review = row.is_completed && pending > 0;
                    row.review_url = row.show_review ? BuildOrderReviewUrl(row.id) : "";

                    row.total_items = items.Count;
                    if (items.Count > 0)
                    {
                        var first = items[0];
                        row.first_item_name = (first.name ?? "").ToString();
                        row.first_item_image = ResolveImagePath((first.image ?? "").ToString());
                        row.first_item_qty = first.soluong == null ? 1 : Convert.ToInt32(first.soluong);
                        row.first_item_price = first.giaban;
                        bool isService = items.Any(x => IsServiceType((x.phanloai ?? "").ToString()));
                        row.show_rebuy = row.is_completed;
                        row.rebuy_label = isService ? "Đặt lại" : "Mua lại";
                        string idsp = (first.idsp ?? "").ToString();
                        row.rebuy_url = row.show_rebuy ? BuildRebuyUrl(idsp, row.NguoiBan, isService) : "";
                        row.more_items_label = items.Count > 1 ? ("và " + (items.Count - 1).ToString() + " sản phẩm khác") : "";
                    }
                    else
                    {
                        row.first_item_name = "Không có sản phẩm";
                        row.first_item_image = "/uploads/images/macdinh.jpg";
                        row.first_item_qty = 0;
                        row.first_item_price = 0m;
                        row.show_rebuy = false;
                        row.rebuy_label = "";
                        row.rebuy_url = "";
                        row.more_items_label = "";
                    }
                }
            }

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
            rp_orders_mobile.DataSource = list_split;
            rp_orders_mobile.DataBind();
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

                    DateTime now = AhaTime_cl.Now;
                    if (congViUuDai30 > 0)
                    {
                        ShopOnlyLedger_cl.AddSellerCreditFromOrder(
                            db,
                            q.nguoiban,
                            iddh,
                            congViUuDai30,
                            2,
                            string.Format("Bán đơn hàng số {0} (Hồ sơ ưu đãi ShopOnly)", iddh));
                    }

                    if (congViTieuDung > 0)
                    {
                        ShopOnlyLedger_cl.AddSellerCreditFromOrder(
                            db,
                            q.nguoiban,
                            iddh,
                            congViTieuDung,
                            1,
                            string.Format("Bán đơn hàng số {0} (Hồ sơ tiêu dùng ShopOnly)", iddh));
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

        string _emailErr;
        ShopEmailNotify_cl.TryNotifyOrder(db, q, ShopEmailTemplate_cl.CodeOrderCancelled, "Đơn hàng đã bị hủy.", out _emailErr);

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
