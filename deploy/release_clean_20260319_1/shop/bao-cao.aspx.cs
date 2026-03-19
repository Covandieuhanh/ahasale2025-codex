using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class shop_bao_cao : System.Web.UI.Page
{
    private readonly Dictionary<string, Control> _controlCache = new Dictionary<string, Control>(StringComparer.Ordinal);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.Equals((Request.QueryString["switch"] ?? "").Trim(), "shop", StringComparison.OrdinalIgnoreCase))
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);

        check_login_cl.check_login_shop("none", "none", true);

        if (!IsPostBack)
        {
            string tk = ResolveCurrentShopAccount();
            if (string.IsNullOrEmpty(tk))
            {
                Response.Redirect("/shop/login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                ShopStatus_cl.EnsureSchemaSafe(db);
                AccountVisibility_cl.EnsureTradeTypeNormalized(db);

                taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                if (acc == null || !PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission))
                {
                    check_login_cl.del_all_cookie_session_shop();
                    Response.Redirect("/shop/login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                bool isCompanyShop = CompanyShop_cl.IsCompanyShopAccount(db, tk);
                SetControlVisible("ph_menu_ban_san_pham", isCompanyShop);
                SetControlVisible("ph_menu_company_space", isCompanyShop);
                SetControlVisible("ph_switch_to_home", PortalActiveMode_cl.HasHomeCredential());
                SetHyperLinkNavigateUrl("lnk_switch_to_home", "/dang-nhap?switch=home");
                SetHyperLinkNavigateUrl("lnk_space_public", "/shop/default.aspx?space=public");
                SetHyperLinkNavigateUrl("lnk_space_internal", "/shop/default.aspx?space=internal");

                BindHeader(db, acc);
                InitDefaultRange();
                LoadReport(db, acc);
            }
        }
    }

    protected void btn_filter_Click(object sender, EventArgs e)
    {
        string tk = ResolveCurrentShopAccount();
        if (string.IsNullOrEmpty(tk))
        {
            Response.Redirect("/shop/login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            ShopStatus_cl.EnsureSchemaSafe(db);
            AccountVisibility_cl.EnsureTradeTypeNormalized(db);

            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc == null || !PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission))
            {
                check_login_cl.del_all_cookie_session_shop();
                Response.Redirect("/shop/login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            bool isCompanyShop = CompanyShop_cl.IsCompanyShopAccount(db, tk);
            SetControlVisible("ph_menu_ban_san_pham", isCompanyShop);
            SetControlVisible("ph_menu_company_space", isCompanyShop);
            SetControlVisible("ph_switch_to_home", PortalActiveMode_cl.HasHomeCredential());
            SetHyperLinkNavigateUrl("lnk_switch_to_home", "/dang-nhap?switch=home");
            SetHyperLinkNavigateUrl("lnk_space_public", "/shop/default.aspx?space=public");
            SetHyperLinkNavigateUrl("lnk_space_internal", "/shop/default.aspx?space=internal");

            BindHeader(db, acc);
            LoadReport(db, acc);
        }
    }

    protected void btn_clear_Click(object sender, EventArgs e)
    {
        Response.Redirect("/shop/bao-cao.aspx?range=all", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        check_login_cl.del_all_cookie_session_shop();
        Response.Redirect("/shop/login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void BindHeader(dbDataContext db, taikhoan_tb acc)
    {
        string displayName = string.IsNullOrWhiteSpace(acc.hoten) ? acc.taikhoan : acc.hoten.Trim();
        string avatar = string.IsNullOrWhiteSpace(acc.anhdaidien) ? "/uploads/images/macdinh.jpg" : acc.anhdaidien.Trim();
        string publicPath = ShopSlug_cl.GetPublicUrl(db, acc);
        string fullPublicUrl = Request.Url.GetLeftPart(UriPartial.Authority) + publicPath;

        SetLabelText("lb_taikhoan", acc.taikhoan);
        SetLabelText("lb_hoten", displayName);
        SetLabelText("lb_hoten_short", displayName);
        SetLabelText("lb_public_path", publicPath);
        SetImageUrl("img_avatar", avatar);

        string phanLoaiText = string.IsNullOrWhiteSpace(acc.phanloai) ? "Gian hàng đối tác" : acc.phanloai;
        SetLabelText("lb_phanloai", phanLoaiText);
        string shopStatusText = acc.TrangThai_Shop == ShopStatus_cl.StatusApproved
            ? "Hoạt động"
            : (acc.TrangThai_Shop == ShopStatus_cl.StatusPending ? "Chờ duyệt" : "Đang khóa");
        SetLabelText("lb_trangthai", shopStatusText);

        SetHyperLinkNavigateUrl("lnk_public_shop", publicPath);
        SetHyperLinkText("lnk_public_shop", "Xem trang công khai");
        SetHyperLinkNavigateUrl("lnk_public_shop_top", publicPath);
        SetHyperLinkText("lnk_public_shop_top", fullPublicUrl);
    }

    private void InitDefaultRange()
    {
        if (txt_from == null || txt_to == null || txt_month == null) return;

        DateTime now = AhaTime_cl.Now;
        txt_month.Text = now.ToString("yyyy-MM");
        txt_from.Text = "";
        txt_to.Text = "";
    }

    private void LoadReport(dbDataContext db, taikhoan_tb acc)
    {
        string tk = acc != null ? (acc.taikhoan ?? "").Trim() : "";
        if (string.IsNullOrEmpty(tk)) return;

        DateTime? fromDate;
        DateTime? toDate;
        DateTime? toExclusive;
        bool isAllTime;
        string monthText;
        ResolveDateRange(out fromDate, out toDate, out toExclusive, out monthText, out isAllTime);

        string rangeLabel = BuildRangeLabel(fromDate, toDate, monthText, isAllTime);
        SetLabelText("lb_range_label", rangeLabel);

        var productsQuery = db.BaiViet_tbs.Where(p => p.nguoitao == tk && (p.bin == false || p.bin == null));
        var productStats = productsQuery
            .GroupBy(p => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Views = g.Sum(p => (int?)p.LuotTruyCap) ?? 0
            })
            .FirstOrDefault();

        int totalProducts = productStats == null ? 0 : productStats.Total;
        int totalViews = productStats == null ? 0 : productStats.Views;

        var orders = GetOrdersInRange(db, tk, fromDate, toExclusive);
        int totalOrders = orders.Count;
        int pendingOrders = 0;
        int groupDat = 0;
        int groupCho = 0;
        int groupDa = 0;
        int groupGiao = 0;
        int groupNhan = 0;
        int groupHuy = 0;

        foreach (var order in orders)
        {
            string exchangeStatus = DonHangStateMachine_cl.GetExchangeStatus(order);
            if (exchangeStatus == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                pendingOrders++;

            string orderStatus = DonHangStateMachine_cl.GetOrderStatus(order);
            string group = ResolveStatusGroup(orderStatus, exchangeStatus);

            switch (group)
            {
                case "da-huy":
                    groupHuy++;
                    break;
                case "da-nhan":
                    groupNhan++;
                    break;
                case "da-giao":
                    groupGiao++;
                    break;
                case "da-trao-doi":
                    groupDa++;
                    break;
                case "cho-trao-doi":
                    groupCho++;
                    break;
                default:
                    groupDat++;
                    break;
            }
        }

        decimal revenueGross = 0m;
        decimal revenueCompletedA = 0m;
        int totalSold = 0;

        if (orders.Count > 0)
        {
            var idStrings = new List<string>(orders.Count);
            foreach (var order in orders)
                idStrings.Add(order.id.ToString());

            var detailRows = db.DonHang_ChiTiet_tbs
                .Where(p => (p.nguoiban_goc == tk || p.nguoiban_danglai == tk) && idStrings.Contains(p.id_donhang))
                .Select(p => new { p.id_donhang, p.thanhtien, p.giaban, p.soluong })
                .ToList();

            revenueGross = detailRows.Sum(p => (p.thanhtien ?? ((p.giaban ?? 0m) * (p.soluong ?? 0))));
            totalSold = detailRows.Sum(p => p.soluong ?? 0);
        }

        if (isAllTime && acc != null)
        {
            revenueCompletedA = acc.HoSo_TieuDung_ShopOnly ?? 0m;
        }
        else
        {
            var ledger = db.LichSu_DongA_tbs.Where(x =>
                x.taikhoan == tk
                && x.LoaiHoSo_Vi == 1
                && (
                    (x.ghichu != null && x.ghichu.Contains(ShopOnlyLedger_cl.TagRoot))
                    || (x.id_donhang != null && x.id_donhang != "")
                ));

            if (fromDate.HasValue)
                ledger = ledger.Where(x => x.ngay >= fromDate.Value);
            if (toExclusive.HasValue)
                ledger = ledger.Where(x => x.ngay < toExclusive.Value);

            var ledgerSums = ledger
                .GroupBy(x => x.CongTru)
                .Select(g => new { CongTru = g.Key, Sum = g.Sum(x => (decimal?)x.dongA) ?? 0m })
                .ToList();

            decimal addA = ledgerSums.Where(x => x.CongTru == true).Select(x => x.Sum).FirstOrDefault();
            decimal subA = ledgerSums.Where(x => x.CongTru == false).Select(x => x.Sum).FirstOrDefault();
            revenueCompletedA = addA - subA;
        }

        decimal responseRate = 100m;
        if (totalOrders > 0)
        {
            int responded = totalOrders - pendingOrders;
            if (responded < 0) responded = 0;
            responseRate = Math.Round((responded * 100m) / totalOrders, 1);
        }

        SetLabelText("lb_revenue_gross", revenueGross.ToString("#,##0") + " đ");
        SetLabelText("lb_revenue_completed", revenueCompletedA.ToString("#,##0.##") + " A");
        SetLabelText("lb_total_orders", totalOrders.ToString("#,##0"));
        SetLabelText("lb_pending_orders", pendingOrders.ToString("#,##0"));
        SetLabelText("lb_total_sold", totalSold.ToString("#,##0"));
        SetLabelText("lb_response_rate", responseRate.ToString("0.#") + "%");
        SetLabelText("lb_total_products", totalProducts.ToString("#,##0"));
        SetLabelText("lb_total_views", totalViews.ToString("#,##0"));

        SetLabelText("lb_group_dat", groupDat.ToString("#,##0"));
        SetLabelText("lb_group_cho", groupCho.ToString("#,##0"));
        SetLabelText("lb_group_da", groupDa.ToString("#,##0"));
        SetLabelText("lb_group_giao", groupGiao.ToString("#,##0"));
        SetLabelText("lb_group_nhan", groupNhan.ToString("#,##0"));
        SetLabelText("lb_group_huy", groupHuy.ToString("#,##0"));
    }

    private void ResolveDateRange(out DateTime? fromDate, out DateTime? toDate, out DateTime? toExclusive, out string monthText, out bool isAllTime)
    {
        fromDate = null;
        toDate = null;
        toExclusive = null;
        monthText = "";
        isAllTime = false;

        string range = (Request.QueryString["range"] ?? "").Trim().ToLowerInvariant();
        if (range == "all")
        {
            isAllTime = true;
            return;
        }

        DateTime parsed;
        bool hasFromInput = txt_from != null && !string.IsNullOrWhiteSpace(txt_from.Text);
        bool hasToInput = txt_to != null && !string.IsNullOrWhiteSpace(txt_to.Text);

        if (hasFromInput && TryParseDate(txt_from.Text, out parsed))
            fromDate = parsed.Date;

        if (hasToInput && TryParseDate(txt_to.Text, out parsed))
            toDate = parsed.Date;

        if (fromDate.HasValue || toDate.HasValue)
        {
            monthText = "";
        }
        else if (txt_month != null && TryParseMonth(txt_month.Text, out parsed))
        {
            monthText = txt_month.Text;
            fromDate = new DateTime(parsed.Year, parsed.Month, 1);
            toDate = fromDate.Value.AddMonths(1).AddDays(-1);
            toExclusive = fromDate.Value.AddMonths(1);
            return;
        }

        if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
        {
            DateTime swap = fromDate.Value;
            fromDate = toDate;
            toDate = swap;
        }

        if (toDate.HasValue)
            toExclusive = toDate.Value.AddDays(1);

        if (!fromDate.HasValue && !toDate.HasValue && !isAllTime)
        {
            DateTime now = AhaTime_cl.Now;
            fromDate = new DateTime(now.Year, now.Month, 1);
            toDate = fromDate.Value.AddMonths(1).AddDays(-1);
            toExclusive = fromDate.Value.AddMonths(1);
        }
    }

    private static bool TryParseDate(string text, out DateTime date)
    {
        date = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(text)) return false;
        string value = text.Trim();
        string[] formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy" };
        return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    private static bool TryParseMonth(string text, out DateTime date)
    {
        date = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(text)) return false;
        string value = text.Trim();
        string[] formats = new[] { "yyyy-MM", "MM/yyyy" };
        return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    private string BuildRangeLabel(DateTime? fromDate, DateTime? toDate, string monthText, bool isAllTime)
    {
        if (isAllTime)
            return "Khoảng thời gian: Toàn thời gian";

        if (!string.IsNullOrWhiteSpace(monthText))
        {
            DateTime monthValue;
            if (DateTime.TryParseExact(monthText, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out monthValue))
                return string.Format("Khoảng thời gian: Tháng {0:MM/yyyy}", monthValue);
        }

        if (fromDate.HasValue && toDate.HasValue)
        {
            DateTime first = new DateTime(fromDate.Value.Year, fromDate.Value.Month, 1);
            DateTime last = first.AddMonths(1).AddDays(-1);
            if (fromDate.Value.Date == first && toDate.Value.Date == last)
                return string.Format("Khoảng thời gian: Tháng {0:MM/yyyy}", first);
        }

        string fromText = fromDate.HasValue ? fromDate.Value.ToString("dd/MM/yyyy") : "...";
        string toText = toDate.HasValue ? toDate.Value.ToString("dd/MM/yyyy") : "...";
        return string.Format("Khoảng thời gian: {0} - {1}", fromText, toText);
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

    private List<DonHang_tb> GetOrdersInRange(dbDataContext db, string tk, DateTime? fromDate, DateTime? toExclusive)
    {
        IQueryable<DonHang_tb> q = db.DonHang_tbs.Where(p => p.nguoiban == tk);
        if (fromDate.HasValue)
            q = q.Where(p => p.ngaydat >= fromDate.Value);
        if (toExclusive.HasValue)
            q = q.Where(p => p.ngaydat < toExclusive.Value);

        return q.ToList();
    }

    private string ResolveCurrentShopAccount()
    {
        string tk = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim();
        if (!string.IsNullOrEmpty(tk))
            return tk;

        tk = "";
        string encodedSession = Session["taikhoan_shop"] as string;
        if (!string.IsNullOrEmpty(encodedSession))
        {
            tk = mahoa_cl.giaima_Bcorn(encodedSession);
        }
        else
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_shop_bcorn"];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
        }

        return (tk ?? "").Trim();
    }

    private void SetLabelText(string id, string text)
    {
        Label ctl = FindControlById(id) as Label;
        if (ctl != null)
            ctl.Text = text ?? "";
    }

    private void SetImageUrl(string id, string imageUrl)
    {
        Image ctl = FindControlById(id) as Image;
        if (ctl != null)
            ctl.ImageUrl = imageUrl ?? "";
    }

    private void SetControlVisible(string id, bool visible)
    {
        Control ctl = FindControlById(id);
        if (ctl != null)
            ctl.Visible = visible;
    }

    private void SetHyperLinkNavigateUrl(string id, string url)
    {
        HyperLink ctl = FindControlById(id) as HyperLink;
        if (ctl != null)
            ctl.NavigateUrl = url ?? "";
    }

    private void SetHyperLinkText(string id, string text)
    {
        HyperLink ctl = FindControlById(id) as HyperLink;
        if (ctl != null)
            ctl.Text = text ?? "";
    }

    private Control FindControlById(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        Control cached;
        if (_controlCache.TryGetValue(id, out cached))
            return cached;

        Control found = FindControlRecursive(this, id);
        if (found != null)
            _controlCache[id] = found;
        return found;
    }

    private static Control FindControlRecursive(Control root, string id)
    {
        if (root == null || string.IsNullOrEmpty(id))
            return null;

        if (string.Equals(root.ID, id, StringComparison.Ordinal))
            return root;

        foreach (Control child in root.Controls)
        {
            Control found = FindControlRecursive(child, id);
            if (found != null)
                return found;
        }

        return null;
    }
}
