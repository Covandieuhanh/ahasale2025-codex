using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_don_ban : System.Web.UI.Page
{
    DanhMuc_cl dm_cl = new DanhMuc_cl();
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    // ✅ TỶ GIÁ QUY ĐỔI: 1A = 1000 VNĐ
    private const decimal TY_GIA_A_VND = 1000m;

    // ===================== OFFLINE POS CART (Session) =====================
    [Serializable]
    public class OfflineCartItem
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal GiaBan { get; set; }   // VNĐ

        public int SoLuong { get; set; }          // 1..999
        public int PhanTramGiamGia { get; set; }  // 0..50 (sửa được)
    }

    private const string OFFLINE_CART_SESSION_KEY = "offline_pos_cart_home_don_ban";
    private const string STATUS_FILTER_KEY = "status_filter_donban_home";

    private string GetRequestQueryValue(string key)
    {
        if (string.IsNullOrEmpty(key))
            return "";

        string[] queryValues = Request.QueryString.GetValues(key);
        if (queryValues != null)
        {
            for (int i = 0; i < queryValues.Length; i++)
            {
                string item = (queryValues[i] ?? "").Trim();
                if (!string.IsNullOrEmpty(item))
                    return item;
            }
        }

        string value = (Request.QueryString[key] ?? "").Trim();
        if (!string.IsNullOrEmpty(value))
        {
            string[] splitByComma = value.Split(',');
            for (int i = 0; i < splitByComma.Length; i++)
            {
                string item = (splitByComma[i] ?? "").Trim();
                if (!string.IsNullOrEmpty(item))
                    return item;
            }
        }

        string rawUrl = Request.RawUrl ?? "";
        int qPos = rawUrl.IndexOf('?');
        if (qPos < 0 || qPos >= rawUrl.Length - 1)
            return "";

        var query = HttpUtility.ParseQueryString(rawUrl.Substring(qPos + 1));
        value = (query[key] ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        string[] rawSplitByComma = value.Split(',');
        for (int i = 0; i < rawSplitByComma.Length; i++)
        {
            string item = (rawSplitByComma[i] ?? "").Trim();
            if (!string.IsNullOrEmpty(item))
                return item;
        }

        return value;
    }

    private bool IsCreateStandaloneMode()
    {
        string raw = GetRequestQueryValue("taodon");
        return raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    protected string GetCreateOrderWrapperCssClass()
    {
        return IsCreateStandaloneMode()
            ? "pos-standalone-page"
            : "modal modal-blur show";
    }

    protected string GetCreateOrderDialogStyle()
    {
        return IsCreateStandaloneMode()
            ? "max-width: 1180px;"
            : "max-width: 1100px;";
    }

    private string ResolveDonBanListUrl()
    {
        return PortalRequest_cl.IsShopPortalRequest() ? "/shop/don-ban" : "/home/don-ban.aspx";
    }

    private void EnsurePortalLogin()
    {
        if (PortalRequest_cl.IsShopPortalRequest())
            check_login_cl.check_login_shop("none", "none", true);
        else
            check_login_cl.check_login_home("none", "none", true);
    }

    private string EnsureCurrentAccountInViewState()
    {
        string tk = (ViewState["taikhoan"] ?? "").ToString().Trim().ToLowerInvariant();
        if (!string.IsNullOrEmpty(tk))
            return tk;

        string tkEncrypted = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (string.IsNullOrEmpty(tkEncrypted))
            return "";

        tk = mahoa_cl.giaima_Bcorn(tkEncrypted);
        tk = (tk ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrEmpty(tk))
            ViewState["taikhoan"] = tk;

        return tk;
    }

    private string NormalizeReturnUrl(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        value = HttpUtility.UrlDecode(value) ?? "";
        value = value.Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        Uri absolute;
        if (Uri.TryCreate(value, UriKind.Absolute, out absolute))
        {
            if (Request.Url == null)
                return "";
            if (!string.Equals(absolute.Host, Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                return "";
            value = absolute.PathAndQuery;
        }

        if (!value.StartsWith("/", StringComparison.Ordinal))
            return "";
        if (value.StartsWith("//", StringComparison.Ordinal))
            return "";

        return value;
    }

    private string ResolveCreateModeBackUrl()
    {
        string fromQuery = NormalizeReturnUrl(GetRequestQueryValue("return_url"));
        if (!string.IsNullOrEmpty(fromQuery))
            return fromQuery;

        return ResolveDonBanListUrl();
    }

    private string BuildCreateOrderEntryUrl(string productId)
    {
        string url = PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/don-ban?taodon=1"
            : "/home/don-ban.aspx?taodon=1";

        string idsp = (productId ?? "").Trim();
        if (!string.IsNullOrEmpty(idsp))
            url += "&idsp=" + HttpUtility.UrlEncode(idsp);

        url += "&return_url=" + HttpUtility.UrlEncode(ResolveDonBanListUrl());
        return url;
    }

    protected string BuildOrderDetailUrl(object orderIdObj)
    {
        string id = (orderIdObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(id))
            return "#";

        string back = ResolveDonBanListUrl();
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["id"] = id;
        query["mode"] = "sell";
        query["return_url"] = back;

        string basePath = PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/don-chi-tiet"
            : "/home/don-chi-tiet.aspx";
        return basePath + "?" + query.ToString();
    }

    private class DonBanRowVm
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
        public bool show_dagiaohang { get; set; }
        public bool show_chothanhtoan { get; set; }
        public bool show_huychothanhtoan { get; set; }
    }

    private IQueryable<DonHang_tb> QueryDonChoTraoDoiBySeller(dbDataContext db, string sellerAccount)
    {
        return db.DonHang_tbs.Where(p =>
            p.nguoiban == sellerAccount &&
            (
                p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
            )
        );
    }

    private List<OfflineCartItem> GetCart()
    {
        var cart = Session[OFFLINE_CART_SESSION_KEY] as List<OfflineCartItem>;
        if (cart == null)
        {
            cart = new List<OfflineCartItem>();
            Session[OFFLINE_CART_SESSION_KEY] = cart;
        }
        return cart;
    }

    private void ClearCart()
    {
        Session[OFFLINE_CART_SESSION_KEY] = new List<OfflineCartItem>();
    }

    private int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private void BindCartUI()
    {
        var cart = GetCart();
        ph_no_cart.Visible = (cart.Count == 0);

        RepeaterCart.DataSource = cart.ToList();
        RepeaterCart.DataBind();
        decimal totalVnd = 0m;
        foreach (var it in cart)
        {
            totalVnd += (it.GiaBan * it.SoLuong);
        }

        lb_cart_total_vnd.Text = totalVnd.ToString("#,##0");
        lb_cart_total_a.Text = (totalVnd / TY_GIA_A_VND).ToString("0.00");


        lb_cart_err.Text = "";
    }

    private void BindSearchResults(string keyword)
    {
        using (dbDataContext db = new dbDataContext())
        {
            keyword = (keyword ?? "").Trim();

            var q = db.BaiViet_tbs.Where(p =>
                p.bin == false &&
                p.phanloai == "sanpham" &&
                p.nguoitao == ViewState["taikhoan"].ToString());

            if (!string.IsNullOrEmpty(keyword))
            {
                q = q.Where(p => p.name.Contains(keyword));
            }

            var list = q.OrderByDescending(p => p.ngaytao)
                .Select(p => new
                {
                    p.id,
                    p.image,
                    p.name,
                    p.giaban
                })
                .Take(30)
                .ToList();

            RepeaterSearch.DataSource = list;
            RepeaterSearch.DataBind();

            ph_no_search.Visible = (list.Count == 0);
        }
    }

    private bool AddProductToCartById(string idsp, bool showWarningWhenMissing)
    {
        string productId = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(productId))
            return false;

        using (dbDataContext db = new dbDataContext())
        {
            var sp = db.BaiViet_tbs.FirstOrDefault(x =>
                x.id.ToString() == productId &&
                x.bin == false &&
                x.phanloai == "sanpham" &&
                x.nguoitao == ViewState["taikhoan"].ToString());

            if (sp == null)
            {
                if (showWarningWhenMissing)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm không tồn tại hoặc không thuộc shop.", "Thông báo", true, "warning");
                }
                return false;
            }

            var cart = GetCart();
            var exist = cart.FirstOrDefault(x => x.ProductId == productId);
            if (exist != null)
            {
                exist.SoLuong = ClampInt(exist.SoLuong + 1, 1, 999);
            }
            else
            {
                int pt = sp.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0;
                pt = ClampInt(pt, 0, 50);

                cart.Add(new OfflineCartItem
                {
                    ProductId = productId,
                    Name = sp.name ?? "",
                    Image = sp.image ?? "",
                    GiaBan = sp.giaban ?? 0,
                    SoLuong = 1,
                    PhanTramGiamGia = pt
                });
            }

            Session[OFFLINE_CART_SESSION_KEY] = cart;
            return true;
        }
    }

    bool IsDuyetGianHangDoiTac()
    {
        string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (string.IsNullOrEmpty(_tk)) return false;

        string tk = mahoa_cl.giaima_Bcorn(_tk);

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk);
            if (acc != null && PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission))
                return true;

            // ✅ điều kiện: đã có bản ghi duyệt thành công TrangThai = 1
            return db.DangKy_GianHangDoiTac_tbs.Any(x => x.taikhoan == tk && x.TrangThai == 1);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        bool createMode = IsCreateStandaloneMode();
        up_main.Visible = !createMode;

        EnsurePortalLogin();
        EnsureCurrentAccountInViewState();

        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();

            // ✅ CHỈ CHO VÀO NẾU ĐÃ ĐƯỢC DUYỆT GIAN HÀNG ĐỐI TÁC
            if (!IsDuyetGianHangDoiTac())
            {
                // set modal để trang chủ hiển thị
                Session["home_modal_msg"] = "Tính năng này chỉ dành cho tài khoản đã đăng ký gian hàng đối tác thành công.";
                Session["home_modal_title"] = "Chưa đủ điều kiện";
                Session["home_modal_type"] = "warning"; // success/info/warning/danger

                Response.Redirect(PortalRequest_cl.IsShopPortalRequest() ? "/shop/default.aspx" : "~/", true);
                return;
            }

            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();

            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }

            if (createMode)
            {
                pn_taodon.Visible = true;
                txt_sp_search.Text = "";
                BindSearchResults("");
                BindCartUI();

                string idsp = GetRequestQueryValue("idsp");
                if (!string.IsNullOrEmpty(idsp))
                {
                    AddProductToCartById(idsp, false);
                    BindCartUI();
                }

                return;
            }

            set_dulieu_macdinh();
            show_main();
        }
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_donban_home"] = "1";
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

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list_all_query = (from ob1 in db.DonHang_tbs.Where(p => p.nguoiban == ViewState["taikhoan"].ToString())
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
                    bool isOffline = p.online_offline.HasValue && p.online_offline.Value == false;

                    return new DonBanRowVm
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
                        show_dagiaohang = DonHangStateMachine_cl.CanMarkDelivered(dh),
                        show_chothanhtoan = isOffline && DonHangStateMachine_cl.CanActivateChoTraoDoi(dh),
                        show_huychothanhtoan = isOffline && DonHangStateMachine_cl.CanCancelChoTraoDoi(dh),
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
            int current_page = int.Parse(ViewState["current_page_donban_home"].ToString());
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

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();
        ViewState["current_page_donban_home"] = int.Parse(ViewState["current_page_donban_home"].ToString()) - 1;
        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();
        ViewState["current_page_donban_home"] = int.Parse(ViewState["current_page_donban_home"].ToString()) + 1;
        show_main();
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        EnsurePortalLogin();
        ViewState["current_page_donban_home"] = 1;
        show_main();
    }

    protected void but_loc_trangthai_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();
        LinkButton button = (LinkButton)sender;
        ViewState[STATUS_FILTER_KEY] = (button.CommandArgument ?? "all").ToString();
        ViewState["current_page_donban_home"] = 1;
        show_main();
    }

    protected void ddl_status_filter_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnsurePortalLogin();
        ViewState[STATUS_FILTER_KEY] = (ddl_status_filter.SelectedValue ?? "all").ToString();
        ViewState["current_page_donban_home"] = 1;
        show_main();
    }
    #endregion

    #region chi tiết đơn hàng
    protected void but_dagiaohang_Click(object sender, EventArgs e)
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
                if (!DonHangStateMachine_cl.CanMarkDelivered(q))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Không thể xác nhận giao hàng ở trạng thái hiện tại.", "Thông báo", true, "warning");
                    return;
                }

                DonHangStateMachine_cl.SetOrderStatus(q, DonHangStateMachine_cl.Order_DaGiao);

                ThongBao_tb _ob4 = new ThongBao_tb();
                _ob4.id = Guid.NewGuid();
                _ob4.daxem = false;
                _ob4.nguoithongbao = ViewState["taikhoan"].ToString();
                _ob4.nguoinhan = q.nguoimua;
                _ob4.link = "/home/don-mua.aspx";
                _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).ten_shop
                             + " vừa xác nhận đã giao hàng. ID đơn hàng: " + ViewState["iddh"].ToString();
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
                show_main();
                up_main.Update();

                Helper_Tabler_cl.ShowModal(this.Page, "Không thể hủy đơn hàng này.", "Thông báo", true, "warning");
                return;
            }

            // Offline (nguoimua="") thì không có hoàn Hồ sơ
            if (string.IsNullOrEmpty(q.nguoimua))
            {
                DonHangStateMachine_cl.SetOrderStatus(q, DonHangStateMachine_cl.Order_DaHuy);
                db.SubmitChanges();

                show_main();
                up_main.Update();

                Helper_Tabler_cl.ShowModal(this.Page, "Đã hủy đơn Offline (không phát sinh hoàn Hồ sơ).", "Thông báo", true, "success");
                return;
            }

            // ===== 1) LẤY LỊCH SỬ TRỪ TIỀN LÚC ĐẶT HÀNG (CongTru=false)
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

            // ===== 3) CỘNG TIỀN VỀ ĐÚNG 2 Hồ sơ CỦA NGƯỜI MUA =====
            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == q.nguoimua);
            if (q_tk == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tìm thấy tài khoản người mua.", "Thông báo", true, "danger");
                return;
            }

            if (hoanViTieuDung > 0)
                q_tk.DongA = (q_tk.DongA ?? 0m) + hoanViTieuDung;

            // ✅ Hoàn về ví ưu đãi trường MỚI
            if (hoanViUuDai30 > 0)
                q_tk.Vi1That_Evocher_30PhanTram = (q_tk.Vi1That_Evocher_30PhanTram ?? 0m) + hoanViUuDai30;


            // ===== 4) UPDATE TRẠNG THÁI =====
            DonHangStateMachine_cl.SetOrderStatus(q, DonHangStateMachine_cl.Order_DaHuy);

            // ===== 5) GHI LỊCH SỬ HOÀN (CongTru=true)
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
                lsHoan2.ghichu = string.Format("Hoàn Hồ sơ ưu đãi 30% đơn {0}: +{1:#,##0.##} Quyền ưu đãi", iddh, hoanViUuDai30);

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

            // ===== 6) THÔNG BÁO CHO NGƯỜI MUA =====
            ThongBao_tb tb = new ThongBao_tb();
            tb.id = Guid.NewGuid();
            tb.daxem = false;
            tb.nguoithongbao = ViewState["taikhoan"].ToString();
            tb.nguoinhan = q.nguoimua;
            tb.link = "/home/don-mua.aspx";
            tb.noidung = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).ten_shop
                       + " vừa hủy đơn. ID đơn hàng: " + iddh;
            tb.thoigian = now;
            tb.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(tb);

            db.SubmitChanges();

            // ===== 7) UI =====
            show_main();
            up_main.Update();

            string msg =
                "Hủy đơn thành công.<br/>" +
                string.Format("ID đơn: <b>{0}</b><br/>", iddh) +
                string.Format("Đã hoàn tổng: <b>{0:#,##0.##} Quyền</b><br/>", tongHoanA) +
                string.Format("- Hồ sơ ưu đãi 30%: <b>+{0:#,##0.##} Quyền ưu đãi</b><br/>", hoanViUuDai30) +
                string.Format("- Hồ sơ tiêu dùng: <b>+{0:#,##0.##} Quyền tiêu dùng</b>", hoanViTieuDung);

            Helper_Tabler_cl.ShowModal(this.Page, msg, "Thông báo", true, "success");
        }
    }

    #endregion

    #region tạo đơn offline (POS)
    protected void but_show_form_taodon_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();
        Response.Redirect(BuildCreateOrderEntryUrl(""), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_close_form_taodon_Click(object sender, EventArgs e)
    {
        if (IsCreateStandaloneMode())
        {
            Response.Redirect(ResolveCreateModeBackUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        // Đóng modal. (Không clear giỏ để tránh mất thao tác)
        pn_taodon.Visible = false;
    }

    protected void txt_sp_search_TextChanged(object sender, EventArgs e)
    {
        EnsurePortalLogin();

        BindSearchResults(txt_sp_search.Text.Trim());
        BindCartUI();

        up_taodon.Update();
    }

    protected void RepeaterSearch_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        EnsurePortalLogin();

        if (e.CommandName != "add") return;

        string idsp = (e.CommandArgument ?? "").ToString();
        if (string.IsNullOrEmpty(idsp)) return;

        AddProductToCartById(idsp, true);

        BindSearchResults(txt_sp_search.Text.Trim());
        BindCartUI();
        up_taodon.Update();
    }

    protected void RepeaterCart_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        EnsurePortalLogin();

        string idsp = (e.CommandArgument ?? "").ToString();
        if (string.IsNullOrEmpty(idsp)) return;

        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.ProductId == idsp);
        if (item == null)
        {
            BindCartUI();
            up_taodon.Update();
            return;
        }

        if (e.CommandName == "plus")
        {
            item.SoLuong = ClampInt(item.SoLuong + 1, 1, 999);
        }
        else if (e.CommandName == "minus")
        {
            item.SoLuong = ClampInt(item.SoLuong - 1, 0, 999);
            if (item.SoLuong <= 0) cart.Remove(item);
        }
        else if (e.CommandName == "remove")
        {
            cart.Remove(item);
        }
        else if (e.CommandName == "updateqty")
        {
            TextBox txt = e.Item.FindControl("txt_qty") as TextBox;
            int qty = Number_cl.Check_Int(((txt != null ? txt.Text : "0")).Trim());
            qty = ClampInt(qty, 0, 999);

            if (qty <= 0) cart.Remove(item);
            else item.SoLuong = qty;
        }
        else if (e.CommandName == "updatediscount")
        {
            TextBox txt = e.Item.FindControl("txt_discount") as TextBox;
            int pt = Number_cl.Check_Int(((txt != null ? txt.Text : "0")).Trim());
            pt = ClampInt(pt, 0, 50);
            item.PhanTramGiamGia = pt;
        }

        Session[OFFLINE_CART_SESSION_KEY] = cart;

        BindCartUI();
        up_taodon.Update();
    }

    protected void but_clear_cart_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();

        ClearCart();
        BindCartUI();
        up_taodon.Update();
    }

    protected void but_taodon_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();

        var cart = GetCart();
        if (cart == null || cart.Count == 0)
        {
            lb_cart_err.Text = "Giỏ hàng trống. Vui lòng thêm sản phẩm.";
            BindCartUI();
            up_taodon.Update();
            return;
        }

        // chuẩn hóa dữ liệu giỏ
        foreach (var it in cart.ToList())
        {
            it.SoLuong = ClampInt(it.SoLuong, 0, 999);
            it.PhanTramGiamGia = ClampInt(it.PhanTramGiamGia, 0, 50);
            if (it.SoLuong <= 0) cart.Remove(it);
        }

        if (cart.Count == 0)
        {
            lb_cart_err.Text = "Giỏ hàng không hợp lệ (SL = 0).";
            BindCartUI();
            up_taodon.Update();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            // re-check sản phẩm + chốt giá theo DB (chuyên nghiệp, tránh sai)
            foreach (var it in cart)
            {
                var sp = db.BaiViet_tbs.FirstOrDefault(x =>
                    x.id.ToString() == it.ProductId &&
                    x.bin == false &&
                    x.phanloai == "sanpham" &&
                    x.nguoitao == ViewState["taikhoan"].ToString());

                if (sp == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page,
                        "Có sản phẩm trong giỏ đã bị xóa/ẩn hoặc không thuộc shop. Vui lòng kiểm tra lại. ID: " + it.ProductId,
                        "Thông báo", true, "warning");
                    return;
                }

                it.Name = sp.name ?? it.Name;
                it.Image = sp.image ?? it.Image;
                it.GiaBan = sp.giaban ?? it.GiaBan;
            }

            decimal tongThanhToan = 0m;
            foreach (var it in cart)
                tongThanhToan += (it.GiaBan * it.SoLuong);

        

            DateTime _now = AhaTime_cl.Now;

            DonHang_tb dh = new DonHang_tb();
            dh.ngaydat = _now;
            dh.nguoimua = "";
            dh.nguoiban = ViewState["taikhoan"].ToString();
            dh.order_status = DonHangStateMachine_cl.Order_DaDat;
            dh.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
            dh.tongtien = tongThanhToan;
            dh.hoten_nguoinhan = "";
            dh.sdt_nguoinhan = "";
            dh.diahchi_nguoinhan = "";
            dh.online_offline = false;
            dh.chothanhtoan = false;
            DonHangStateMachine_cl.SyncLegacyStatus(dh);

            db.DonHang_tbs.InsertOnSubmit(dh);
            db.SubmitChanges();

            string id_donhang = dh.id.ToString();

            foreach (var it in cart)
            {
                DonHang_ChiTiet_tb ct = new DonHang_ChiTiet_tb();
                ct.id_donhang = id_donhang;
                ct.idsp = it.ProductId;
                ct.nguoiban_goc = dh.nguoiban;
                ct.nguoiban_danglai = "";
                ct.soluong = it.SoLuong;
                ct.giaban = Convert.ToInt64(it.GiaBan);
                ct.thanhtien = Convert.ToInt64(it.GiaBan * it.SoLuong);


                // ✅ POS: lưu % ưu đãi theo giỏ (user sửa được)
                ct.PhanTram_GiamGia_ThanhToan_BangEvoucher = ClampInt(it.PhanTramGiamGia, 0, 50);

                db.DonHang_ChiTiet_tbs.InsertOnSubmit(ct);
            }

            db.SubmitChanges();

            // clear cart + đóng modal
            ClearCart();
            BindCartUI();

            if (IsCreateStandaloneMode())
            {
                Session["thongbao_home"] = thongbao_class.metro_dialog_onload(
                    "Thông báo",
                    "Tạo đơn Offline thành công. ID: <b>" + id_donhang + "</b>",
                    "false", "false", "OK", "alert", ""
                );

                if (PortalRequest_cl.IsShopPortalRequest())
                {
                    string sellerAccount = (ViewState["taikhoan"] ?? "").ToString();
                    var donChoKhac = QueryDonChoTraoDoiBySeller(db, sellerAccount)
                        .FirstOrDefault(p => p.id.ToString() != id_donhang);

                    if (donChoKhac == null)
                    {
                        DonHangStateMachine_cl.SetExchangeStatus(dh, DonHangStateMachine_cl.Exchange_ChoTraoDoi);
                        dh.chothanhtoan = true;
                        db.SubmitChanges();
                    }
                    else
                    {
                        Session["thongbao_home"] = thongbao_class.metro_dialog_onload(
                            "Thông báo",
                            "Bạn đang có 1 đơn chờ Trao đổi khác (ID: <b>" + donChoKhac.id + "</b>). Hệ thống chuyển bạn đến trang chờ trao đổi của đơn đang hoạt động.",
                            "false", "false", "OK", "alert", ""
                        );
                    }

                    Response.Redirect("/shop/cho-thanh-toan", false);
                }
                else
                {
                    Response.Redirect(ResolveCreateModeBackUrl(), false);
                }
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // refresh main
            show_main();
            up_main.Update();
            pn_taodon.Visible = false;
            Helper_Tabler_cl.ShowModal(this.Page, "Tạo đơn Offline thành công. ID: <b>" + id_donhang + "</b>", "Thông báo", true, "success");
        }
    }
    #endregion

    protected void but_chothanhtoan_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _iddh = button.CommandArgument;

            var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == _iddh);
            if (q != null)
            {
                DonHangStateMachine_cl.EnsureStateFields(q);
                if (!DonHangStateMachine_cl.CanActivateChoTraoDoi(q))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Không thể kích hoạt chờ Trao đổi ở trạng thái hiện tại.", "Thông báo", true, "warning");
                    return;
                }

                var q1 = QueryDonChoTraoDoiBySeller(db, ViewState["taikhoan"].ToString())
                    .FirstOrDefault(p => p.id.ToString() != _iddh);
                if (q1 != null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Bạn đang có 1 đơn đang chờ Trao đổi khác. ID đơn: " + q1.id.ToString(), "Thông báo", true, "warning");
                    return;
                }

                DonHangStateMachine_cl.SetExchangeStatus(q, DonHangStateMachine_cl.Exchange_ChoTraoDoi);
                q.chothanhtoan = true;
                db.SubmitChanges();
                show_main();

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                    "window.location.href='" + (PortalRequest_cl.IsShopPortalRequest() ? "/shop/cho-thanh-toan" : "/home/cho-thanh-toan.aspx") + "';", true);
            }
        }
    }


    protected void but_huychothanhtoan_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _iddh = button.CommandArgument;

            var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == _iddh);
            if (q != null)
            {
                DonHangStateMachine_cl.EnsureStateFields(q);
                if (!DonHangStateMachine_cl.CanCancelChoTraoDoi(q))
                {
                    show_main();
                    Helper_Tabler_cl.ShowModal(this.Page, "Không thể hủy chờ Trao đổi ở trạng thái hiện tại.", "Thông báo", true, "warning");
                    return;
                }

                DonHangStateMachine_cl.SetExchangeStatus(q, DonHangStateMachine_cl.Exchange_ChuaTraoDoi);
                q.chothanhtoan = false;
                db.SubmitChanges();
                show_main();

                Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
            }
        }
    }

    protected void but_dagiaohang_row_Click(object sender, EventArgs e)
    {
        LinkButton button = (LinkButton)sender;
        ViewState["iddh"] = button.CommandArgument;
        but_dagiaohang_Click(sender, EventArgs.Empty);
    }

    protected void but_huydonhang_row_Click(object sender, EventArgs e)
    {
        LinkButton button = (LinkButton)sender;
        ViewState["iddh"] = button.CommandArgument;
        but_huydonhang_Click(sender, EventArgs.Empty);
    }

}
