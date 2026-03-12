using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

public partial class shop_public : System.Web.UI.Page
{
    private const string HomeLoginFlag = "home_login";

    private class ShopPublicProductView
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string image { get; set; }
        public decimal? giaban { get; set; }
        public DateTime? ngaytao { get; set; }
        public int LuotTruyCap { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string AreaRaw { get; set; }
        public string AreaLabel { get; set; }
        public long DateTicks { get; set; }
        public int SoldCount { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            LoadShopPublicPage();
    }

    private void LoadShopPublicPage()
    {
        string queryShopSlug = (Request.QueryString["shop_slug"] ?? "").Trim().ToLowerInvariant();
        string queryUser = (Request.QueryString["user"] ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(queryShopSlug) && string.IsNullOrEmpty(queryUser))
        {
            RedirectToRoot();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb shop = null;

            if (!string.IsNullOrEmpty(queryShopSlug))
            {
                shop = ShopSlug_cl.FindApprovedShopBySlug(db, queryShopSlug);
            }
            else
            {
                shop = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == queryUser);
                if (shop != null && !ShopSlug_cl.IsShopAccount(db, shop))
                    shop = null;
            }

            if (shop == null)
            {
                RedirectToRoot();
                return;
            }
            if (shop.block == true)
            {
                RedirectToRoot();
                return;
            }

            string canonicalPath = ShopSlug_cl.GetPublicUrl(db, shop);
            string requestPath = (Request.Url.AbsolutePath ?? "").Trim().ToLowerInvariant();
            if (!requestPath.Equals(canonicalPath, StringComparison.OrdinalIgnoreCase)
                && !requestPath.EndsWith("/shop/public.aspx", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect(canonicalPath, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string currentHomeAccount = GetCurrentHomeAccount(db);
            if (ShouldStartHomeLogin())
            {
                if (string.IsNullOrEmpty(currentHomeAccount))
                {
                    Session["url_back_home"] = BuildAbsoluteUrl(canonicalPath).ToLowerInvariant();
                    Response.Redirect(BuildHomeLoginUrl(canonicalPath), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Response.Redirect(canonicalPath, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            BindShopSummary(db, shop, canonicalPath, currentHomeAccount);
            BindShopProducts(db, shop.taikhoan);
        }
    }

    private void BindShopSummary(dbDataContext db, taikhoan_tb shop, string canonicalPath, string currentHomeAccount)
    {
        ViewState["shop_taikhoan"] = (shop.taikhoan ?? "").Trim().ToLowerInvariant();

        string displayName = (shop.ten_shop ?? "").Trim();
        if (string.IsNullOrEmpty(displayName))
            displayName = (shop.hoten ?? "").Trim();
        if (string.IsNullOrEmpty(displayName))
            displayName = (shop.taikhoan ?? "").Trim();

        string ownerName = (shop.hoten ?? "").Trim();
        if (string.IsNullOrEmpty(ownerName))
            ownerName = (shop.taikhoan ?? "").Trim();

        string avatar = (shop.anhdaidien ?? "").Trim();
        if (string.IsNullOrEmpty(avatar))
            avatar = "/uploads/images/macdinh.jpg";

        string slug = ShopSlug_cl.EnsureSlugForShop(db, shop);
        if (string.IsNullOrEmpty(slug))
            slug = (shop.taikhoan ?? "").Trim().ToLowerInvariant();

        var visibleProducts = AccountVisibility_cl.FilterVisibleProducts(
            db,
            db.BaiViet_tbs.Where(p => p.nguoitao == shop.taikhoan));

        int totalProducts = visibleProducts.Count();
        int totalViews = visibleProducts
            .Select(p => (int?)p.LuotTruyCap)
            .ToList()
            .Sum() ?? 0;

        int totalSold = db.DonHang_ChiTiet_tbs
            .Where(p => p.nguoiban_goc == shop.taikhoan || p.nguoiban_danglai == shop.taikhoan)
            .Select(p => (int?)p.soluong)
            .ToList()
            .Sum() ?? 0;

        int pendingOrders = GetPendingOrdersCompat(db, shop.taikhoan);

        img_avatar.ImageUrl = avatar;
        lb_shop_name.Text = displayName;
        lb_shop_slug.Text = slug;
        lb_owner_name.Text = ownerName;
        lb_total_products.Text = totalProducts.ToString("#,##0");
        lb_total_views.Text = totalViews.ToString("#,##0");
        lb_total_sold.Text = totalSold.ToString("#,##0");
        lb_pending_orders.Text = pendingOrders.ToString("#,##0");
        lb_public_url.Text = Request.Url.GetLeftPart(UriPartial.Authority) + canonicalPath;

        if (!string.IsNullOrEmpty(currentHomeAccount))
        {
            lnk_home_login.Text = "Vào trang Home";
            lnk_home_login.NavigateUrl = "/home/default.aspx";
            lnk_home_login.ToolTip = "Đã đăng nhập Home: " + currentHomeAccount;
        }
        else
        {
            lnk_home_login.Text = "Đăng nhập Home để trao đổi";
            lnk_home_login.NavigateUrl = BuildHomeLoginUrl(canonicalPath);
            lnk_home_login.ToolTip = "Đăng nhập Home để thao tác trao đổi";
        }
    }

    private void BindShopProducts(dbDataContext db, string taiKhoanShop)
    {
        var rawProducts = db.BaiViet_tbs
            .Where(p => p.nguoitao == taiKhoanShop)
            .Where(p => db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true))
            .Where(p => p.bin == false && p.phanloai == "sanpham")
            .OrderByDescending(p => p.ngaytao)
            .Select(p => new
            {
                p.id,
                p.name,
                p.name_en,
                p.image,
                p.giaban,
                p.ngaytao,
                LuotTruyCap = (p.LuotTruyCap ?? 0),
                CategoryId = p.id_DanhMuc,
                AreaRaw = p.ThanhPho,
                SoldCount = (p.soluong_daban ?? 0)
            })
            .ToList();

        var categoryIds = rawProducts
            .Select(p => (p.CategoryId ?? "").Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .ToList();

        var categoryIdInts = new List<int>();
        foreach (var idRaw in categoryIds)
        {
            int idInt;
            if (int.TryParse(idRaw, out idInt))
                categoryIdInts.Add(idInt);
        }

        var categoryMap = db.DanhMuc_tbs
            .Where(dm => categoryIdInts.Contains(dm.id))
            .ToList()
            .ToDictionary(dm => dm.id.ToString(), dm => (dm.name ?? "").Trim());

        var products = rawProducts.Select(p =>
        {
            string categoryName = "";
            string categoryKey = (p.CategoryId ?? "").Trim();
            if (!string.IsNullOrEmpty(categoryKey) && categoryMap.ContainsKey(categoryKey))
                categoryName = categoryMap[categoryKey];

            string areaLabel = "";
            string areaRaw = (p.AreaRaw ?? "").Trim();
            if (!string.IsNullOrEmpty(areaRaw))
                areaLabel = TinhThanhDisplay_cl.Format(areaRaw);

            long dateTicks = p.ngaytao.HasValue ? p.ngaytao.Value.Ticks : 0;

            return new ShopPublicProductView
            {
                id = p.id,
                name = p.name,
                name_en = p.name_en,
                image = p.image,
                giaban = p.giaban,
                ngaytao = p.ngaytao,
                LuotTruyCap = p.LuotTruyCap,
                CategoryId = categoryKey,
                CategoryName = categoryName,
                AreaRaw = areaRaw.ToLowerInvariant(),
                AreaLabel = string.IsNullOrEmpty(areaLabel) ? "Không rõ" : areaLabel,
                DateTicks = dateTicks,
                SoldCount = p.SoldCount
            };
        }).ToList();

        rp_products.DataSource = products;
        rp_products.DataBind();

        bool hasProducts = products.Count > 0;
        ph_products.Visible = hasProducts;
        ph_empty_products.Visible = !hasProducts;

        BuildFilterOptions(products, categoryMap);
    }

    private void BuildFilterOptions(List<ShopPublicProductView> products, Dictionary<string, string> categoryMap)
    {
        var sbSuggest = new StringBuilder();
        sbSuggest.Append("<datalist id='shopSuggest'>");
        foreach (string name in products.Select(p => (string)(p.name ?? "")).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct())
        {
            sbSuggest.Append("<option value=\"");
            sbSuggest.Append(HttpUtility.HtmlAttributeEncode(name));
            sbSuggest.Append("\"></option>");
        }
        sbSuggest.Append("</datalist>");
        lit_shop_suggest.Text = sbSuggest.ToString();

        var sbCat = new StringBuilder();
        sbCat.Append("<option value=\"\">Tất cả danh mục</option>");
        foreach (var kv in categoryMap.OrderBy(k => k.Value))
        {
            sbCat.Append("<option value=\"");
            sbCat.Append(HttpUtility.HtmlAttributeEncode(kv.Key));
            sbCat.Append("\">");
            sbCat.Append(HttpUtility.HtmlEncode(kv.Value));
            sbCat.Append("</option>");
        }
        lit_category_options.Text = sbCat.ToString();

        var areas = products
            .Select(p => (string)p.AreaRaw)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        var sbArea = new StringBuilder();
        sbArea.Append("<option value=\"\">Tất cả khu vực</option>");
        foreach (var area in areas)
        {
            string label = TinhThanhDisplay_cl.Format(area);
            sbArea.Append("<option value=\"");
            sbArea.Append(HttpUtility.HtmlAttributeEncode(area.ToLowerInvariant()));
            sbArea.Append("\">");
            sbArea.Append(HttpUtility.HtmlEncode(label));
            sbArea.Append("</option>");
        }
        lit_area_options.Text = sbArea.ToString();
    }

    private void RedirectToRoot()
    {
        Response.Redirect("/", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private bool ShouldStartHomeLogin()
    {
        string q1 = (Request.QueryString[HomeLoginFlag] ?? "").Trim();
        if (q1 == "1" || q1.Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;

        string rawUrl = (Request.RawUrl ?? "").Trim();
        if (!string.IsNullOrEmpty(rawUrl)
            && rawUrl.IndexOf(HomeLoginFlag + "=1", StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        string q2 = (Request.Url == null ? "" : Request.Url.Query ?? "").Trim();
        if (!string.IsNullOrEmpty(q2))
        {
            var parsed = HttpUtility.ParseQueryString(q2.TrimStart('?'));
            string value = (parsed[HomeLoginFlag] ?? "").Trim();
            if (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private string BuildHomeLoginUrl(string canonicalPath)
    {
        string path = (canonicalPath ?? "").Trim();
        if (string.IsNullOrEmpty(path))
            path = (Request.Url == null ? "/" : (Request.Url.AbsolutePath ?? "/")).Trim();
        if (string.IsNullOrEmpty(path))
            path = "/";

        return "/dang-nhap?return_url=" + HttpUtility.UrlEncode(path);
    }

    private string BuildAbsoluteUrl(string relativePath)
    {
        string path = (relativePath ?? "").Trim();
        if (string.IsNullOrEmpty(path))
            path = "/";
        return Request.Url.GetLeftPart(UriPartial.Authority) + path;
    }

    private string GetCurrentHomeAccount(dbDataContext db)
    {
        string tkEncrypted = Session["taikhoan_home"] as string;
        if (string.IsNullOrEmpty(tkEncrypted))
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_home_bcorn"];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                tkEncrypted = ck["taikhoan"];
        }

        if (string.IsNullOrEmpty(tkEncrypted))
            return "";

        string tk = "";
        try
        {
            tk = mahoa_cl.giaima_Bcorn(tkEncrypted);
        }
        catch
        {
            tk = "";
        }

        tk = (tk ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
            return "";

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc == null)
            return "";
        if (acc.block == true)
            return "";

        if (!PortalScope_cl.CanLoginHome(acc.taikhoan, acc.phanloai, acc.permission))
            return "";

        return acc.taikhoan;
    }

    private int GetPendingOrdersCompat(dbDataContext db, string tk)
    {
        try
        {
            return db.DonHang_tbs.Count(p =>
                p.nguoiban == tk &&
                (
                    p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                    || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                ));
        }
        catch (SqlException ex)
        {
            if (!IsMissingDonHangStatusColumnError(ex))
                throw;

            return db.DonHang_tbs.Count(p =>
                p.nguoiban == tk &&
                p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi);
        }
    }

    private static bool IsMissingDonHangStatusColumnError(SqlException ex)
    {
        if (ex == null)
            return false;

        string message = ex.Message ?? "";
        return message.IndexOf("Invalid column name 'exchange_status'", StringComparison.OrdinalIgnoreCase) >= 0
            || message.IndexOf("Invalid column name 'order_status'", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    protected string ResolveProductImage(object imageRaw)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        string image = (imageRaw ?? "").ToString().Trim();
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

        if (IsMissingUploadFile(image))
            return fallback;

        return image;
    }

    private bool IsMissingUploadFile(string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
            return false;
        if (!relativeUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return false;

        string cleanPath = relativeUrl;
        int q = cleanPath.IndexOf('?');
        if (q >= 0)
            cleanPath = cleanPath.Substring(0, q);

        try
        {
            string physical = Server.MapPath("~" + cleanPath);
            if (string.IsNullOrEmpty(physical))
                return false;
            return !File.Exists(physical);
        }
        catch
        {
            return false;
        }
    }

    protected string ResolveProductUrl(object idRaw, object nameEnRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);
        if (id <= 0) return "#";

        string slug = (nameEnRaw ?? "").ToString().Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(slug))
            slug = "san-pham";

        return "/" + slug + "-" + id.ToString() + ".html";
    }

    protected string BuildExchangeActionUrl(object idRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);
        if (id <= 0)
            return "#";

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["idsp"] = id.ToString();
        query["qty"] = "1";

        string shopAccount = (ViewState["shop_taikhoan"] ?? "").ToString().Trim();
        if (!string.IsNullOrEmpty(shopAccount))
            query["user_bancheo"] = shopAccount;

        query["return_url"] = (Request.RawUrl ?? "/");
        return "/home/trao-doi.aspx?" + query.ToString();
    }

    protected string BuildAddCartActionUrl(object idRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);
        if (id <= 0)
            return "#";

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["idsp"] = id.ToString();
        query["qty"] = "1";

        string shopAccount = (ViewState["shop_taikhoan"] ?? "").ToString().Trim();
        if (!string.IsNullOrEmpty(shopAccount))
            query["user_bancheo"] = shopAccount;

        query["return_url"] = (Request.RawUrl ?? "/");
        return "/home/them-vao-gio.aspx?" + query.ToString();
    }

    protected string FormatCurrency(object valueRaw)
    {
        decimal value = 0m;
        if (valueRaw != null && valueRaw != DBNull.Value)
        {
            try
            {
                value = Convert.ToDecimal(valueRaw, CultureInfo.InvariantCulture);
            }
            catch
            {
                string raw = Convert.ToString(valueRaw, CultureInfo.InvariantCulture);
                if (!decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                    && !decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                {
                    value = 0m;
                }
            }
        }

        return value.ToString("#,##0.##", CultureInfo.GetCultureInfo("vi-VN"));
    }

    protected string FormatDate(object dateRaw)
    {
        DateTime date;
        if (DateTime.TryParse(Convert.ToString(dateRaw, CultureInfo.InvariantCulture), out date))
            return date.ToString("dd/MM/yyyy HH:mm");
        return "--";
    }
}
