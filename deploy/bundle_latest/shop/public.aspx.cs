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
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    private string _cachedReturnUrlEncoded;
    private string _cachedShopAccount;
    private string _cachedShopAccountEncoded;

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
        public string PostType { get; set; }
        public bool IsService { get; set; }
        public string ExchangeLabel { get; set; }
    }

    private sealed class ShopPublicProductRaw
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string image { get; set; }
        public decimal? giaban { get; set; }
        public DateTime? ngaytao { get; set; }
        public int LuotTruyCap { get; set; }
        public string CategoryId { get; set; }
        public string AreaRaw { get; set; }
        public int SoldCount { get; set; }
        public string PostType { get; set; }
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
            ShopStatus_cl.EnsureSchemaSafe(db);
            AccountVisibility_cl.EnsureTradeTypeNormalized(db);
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
            if (!ShopStatus_cl.IsShopApproved(shop))
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

            ApplyStorefrontConfig(db, shop, canonicalPath);
            var rawProducts = LoadShopProductsRaw(db, shop.taikhoan);
            BindShopSummary(db, shop, canonicalPath, currentHomeAccount, rawProducts);
            BindShopProducts(db, rawProducts);
        }
    }

    private void ApplyStorefrontConfig(dbDataContext db, taikhoan_tb shop, string canonicalPath)
    {
        if (db == null || shop == null)
            return;

        string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, shop.taikhoan);

        gianhang_storefront_config_table config = GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);
        gianhang_storefront_section_table featuredProducts = GianHangStorefrontConfig_cl.GetSection(db, chiNhanhId, GianHangStorefrontConfig_cl.SectionFeaturedProducts);

        lb_public_hero_title.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_title, "Trang công khai gian hàng đối tác"));
        lb_public_hero_sub.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_description, "Trang công khai cho phép xem tự do. Nếu muốn thao tác trao đổi, vui lòng đăng nhập tài khoản Home."));
        lb_public_top_title.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_title, "Top tin nổi bật"));
        lb_public_top_sub.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_description, "Ưu tiên theo bán chạy và lượt xem."));
        lb_public_products_title.Text = HttpUtility.HtmlEncode(featuredProducts != null
            ? GianHangStorefrontConfig_cl.ResolveText(featuredProducts.title, "Sản phẩm & dịch vụ của gian hàng đối tác")
            : "Sản phẩm & dịch vụ của gian hàng đối tác");
        lb_public_products_sub.Text = HttpUtility.HtmlEncode(featuredProducts != null
            ? GianHangStorefrontConfig_cl.ResolveText(featuredProducts.description, "Danh sách này là public, lấy trực tiếp từ tài khoản gian hàng đối tác hiện tại.")
            : "Danh sách này là public, lấy trực tiếp từ tài khoản gian hàng đối tác hiện tại.");

        string primaryText = (config.hero_primary_text ?? "").Trim();
        string primaryUrl = ResolveShopConfigUrl((config.hero_primary_url ?? "").Trim(), canonicalPath);
        lnk_public_primary_cta.Visible = !string.IsNullOrWhiteSpace(primaryText);
        lnk_public_primary_cta.Text = HttpUtility.HtmlEncode(primaryText);
        lnk_public_primary_cta.NavigateUrl = primaryUrl;

        string scrollText = string.IsNullOrWhiteSpace(config.hero_secondary_text) ? "Xem sản phẩm" : config.hero_secondary_text.Trim();
        lnk_public_scroll_cta.Text = HttpUtility.HtmlEncode(scrollText);

        string topCtaText = featuredProducts != null && !string.IsNullOrWhiteSpace(featuredProducts.cta_text)
            ? featuredProducts.cta_text.Trim()
            : "Xem tất cả";
        string topCtaUrl = featuredProducts != null
            ? ResolveShopConfigUrl(featuredProducts.cta_url, "#shop-products")
            : "#shop-products";
        lnk_public_top_cta.Text = HttpUtility.HtmlEncode(topCtaText);
        lnk_public_top_cta.NavigateUrl = topCtaUrl;
    }

    private void BindShopSummary(dbDataContext db, taikhoan_tb shop, string canonicalPath, string currentHomeAccount, List<ShopPublicProductRaw> rawProducts)
    {
        ViewState["shop_taikhoan"] = (shop.taikhoan ?? "").Trim().ToLowerInvariant();

        string displayName = (shop.ten_shop ?? "").Trim();
        if (string.IsNullOrEmpty(displayName))
            displayName = (shop.taikhoan ?? "").Trim();

        string ownerName = (shop.hoten ?? "").Trim();
        if (string.IsNullOrEmpty(ownerName))
            ownerName = (shop.taikhoan ?? "").Trim();

        string logo = (shop.logo_shop ?? "").Trim();
        string avatar = ResolveShopImage(logo, "/uploads/images/macdinh.jpg");

        string cover = ResolveShopImage((shop.anhbia_shop ?? "").Trim(), "");
        bool hasCover = !string.IsNullOrEmpty(cover);
        if (hasCover)
        {
            pn_cover.Visible = true;
            pn_cover.Attributes["style"] = "background-image: url('" + HttpUtility.HtmlAttributeEncode(cover) + "');";
        }
        else
        {
            pn_cover.Visible = false;
        }

        if (hero_section != null)
            hero_section.Attributes["class"] = hasCover ? "hero has-cover" : "hero";

        string slug = ShopSlug_cl.EnsureSlugForShop(db, shop);
        if (string.IsNullOrEmpty(slug))
            slug = (shop.taikhoan ?? "").Trim().ToLowerInvariant();

        int totalProducts = rawProducts == null ? 0 : rawProducts.Count;
        int totalViews = rawProducts == null ? 0 : rawProducts.Sum(p => p.LuotTruyCap);

        int totalSold = db.DonHang_ChiTiet_tbs
            .Where(p => p.nguoiban_goc == shop.taikhoan || p.nguoiban_danglai == shop.taikhoan)
            .Sum(p => (int?)p.soluong) ?? 0;

        int pendingOrders = GetPendingOrdersCompat(db, shop.taikhoan);

        img_avatar.ImageUrl = avatar;
        lb_shop_name.Text = HttpUtility.HtmlEncode(displayName);
        lb_shop_slug.Text = HttpUtility.HtmlEncode(slug);
        lb_owner_name.Text = HttpUtility.HtmlEncode(ownerName);
        lb_total_products.Text = totalProducts.ToString("#,##0");
        lb_total_views.Text = totalViews.ToString("#,##0");
        lb_total_sold.Text = totalSold.ToString("#,##0");
        lb_pending_orders.Text = pendingOrders.ToString("#,##0");
        lb_public_url.Text = HttpUtility.HtmlEncode(Request.Url.GetLeftPart(UriPartial.Authority) + canonicalPath);

        string desc = (shop.motangan_shop ?? "").Trim();
        if (!string.IsNullOrEmpty(desc))
        {
            ph_shop_desc.Visible = true;
            lb_shop_desc.Text = HttpUtility.HtmlEncode(desc);
        }
        else
        {
            ph_shop_desc.Visible = false;
            lb_shop_desc.Text = "";
        }

        pn_public_stats.Visible = totalProducts > 0 || totalViews > 0 || totalSold > 0 || pendingOrders > 0;

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

    private List<ShopPublicProductRaw> LoadShopProductsRaw(dbDataContext db, string taiKhoanShop)
    {
        return db.BaiViet_tbs
            .Where(p => p.nguoitao == taiKhoanShop
                        && (p.bin == false || p.bin == null)
                        && (p.phanloai == AccountVisibility_cl.PostTypeProduct || p.phanloai == AccountVisibility_cl.PostTypeService))
            .OrderByDescending(p => p.ngaytao)
            .Select(p => new ShopPublicProductRaw
            {
                id = p.id,
                name = p.name,
                name_en = p.name_en,
                image = p.image,
                giaban = p.giaban,
                ngaytao = p.ngaytao,
                LuotTruyCap = (p.LuotTruyCap ?? 0),
                CategoryId = p.id_DanhMuc,
                AreaRaw = p.ThanhPho,
                SoldCount = (p.soluong_daban ?? 0),
                PostType = p.phanloai
            })
            .ToList();
    }

    private void BindShopProducts(dbDataContext db, List<ShopPublicProductRaw> rawProducts)
    {
        if (rawProducts == null)
            rawProducts = new List<ShopPublicProductRaw>();

        var categoryIds = rawProducts
            .Select(p => (p.CategoryId ?? "").Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .ToList();

        var categoryIdInts = new List<int>(categoryIds.Count);
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

        var missingCategoryIds = categoryIds
            .Where(id => !categoryMap.ContainsKey(id))
            .ToList();
        if (missingCategoryIds.Count > 0)
        {
            var menuFallback = db.web_menu_tables
                .Where(mn => missingCategoryIds.Contains(mn.id.ToString()) && (mn.bin == false || mn.bin == null))
                .ToList()
                .GroupBy(mn => mn.id.ToString())
                .ToDictionary(g => g.Key, g => (g.First().name ?? "").Trim());

            foreach (var pair in menuFallback)
            {
                if (!categoryMap.ContainsKey(pair.Key))
                    categoryMap[pair.Key] = pair.Value;
            }
        }

        var areaLabelCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var products = new List<ShopPublicProductView>(rawProducts.Count);
        foreach (var p in rawProducts)
        {
            bool isService = string.Equals((p.PostType ?? "").Trim(), AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase);
            string categoryName = "";
            string categoryKey = (p.CategoryId ?? "").Trim();
            if (!string.IsNullOrEmpty(categoryKey))
                categoryMap.TryGetValue(categoryKey, out categoryName);

            string areaLabel = "";
            string areaRaw = (p.AreaRaw ?? "").Trim();
            if (!string.IsNullOrEmpty(areaRaw))
            {
                if (!areaLabelCache.TryGetValue(areaRaw, out areaLabel))
                {
                    areaLabel = TinhThanhDisplay_cl.Format(areaRaw);
                    areaLabelCache[areaRaw] = areaLabel;
                }
            }

            long dateTicks = p.ngaytao.HasValue ? p.ngaytao.Value.Ticks : 0;

            products.Add(new ShopPublicProductView
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
                SoldCount = p.SoldCount,
                PostType = p.PostType,
                IsService = isService,
                ExchangeLabel = isService ? "Đặt lịch" : "Trao đổi"
            });
        }

        rp_products.DataSource = products;
        rp_products.DataBind();

        var topProducts = products
            .OrderByDescending(p => p.SoldCount)
            .ThenByDescending(p => p.LuotTruyCap)
            .ThenByDescending(p => p.DateTicks)
            .Take(6)
            .ToList();

        ph_top_products.Visible = topProducts.Count > 0;
        rp_top_products.DataSource = topProducts;
        rp_top_products.DataBind();

        bool hasProducts = products.Count > 0;
        ph_products.Visible = hasProducts;
        ph_empty_products.Visible = !hasProducts;

        BuildFilterOptions(products, categoryMap, areaLabelCache);
    }

    private void BuildFilterOptions(List<ShopPublicProductView> products, Dictionary<string, string> categoryMap, Dictionary<string, string> areaLabelCache)
    {
        var sbSuggest = new StringBuilder();
        sbSuggest.Append("<datalist id='shopSuggest'>");
        var nameSet = new HashSet<string>(StringComparer.Ordinal);
        foreach (var product in products)
        {
            string name = (product.name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
                continue;
            if (!nameSet.Add(name))
                continue;

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

        var areaSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var product in products)
        {
            string area = (product.AreaRaw ?? "").Trim();
            if (string.IsNullOrWhiteSpace(area))
                continue;
            areaSet.Add(area);
        }

        var areas = areaSet.OrderBy(s => s).ToList();

        var sbArea = new StringBuilder();
        sbArea.Append("<option value=\"\">Tất cả khu vực</option>");
        foreach (var area in areas)
        {
            string label;
            if (areaLabelCache == null || !areaLabelCache.TryGetValue(area, out label))
            {
                label = TinhThanhDisplay_cl.Format(area);
                if (areaLabelCache != null)
                    areaLabelCache[area] = label;
            }
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

    private string ResolveShopConfigUrl(string raw, string fallback)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return fallback ?? "";
        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            return value;
        return fallback ?? "";
    }

    private string GetCurrentHomeAccount(dbDataContext db)
    {
        if (!PortalActiveMode_cl.IsHomeActive())
            return "";

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

    private Dictionary<string, bool> GetUploadMissingCache()
    {
        try
        {
            if (Context == null || Context.Items == null)
                return new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            const string cacheKey = "__aha_missing_upload_cache_shop_public";
            var cached = Context.Items[cacheKey] as Dictionary<string, bool>;
            if (cached == null)
            {
                cached = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                Context.Items[cacheKey] = cached;
            }
            return cached;
        }
        catch
        {
            return new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private string ResolveShopImage(string imageRaw, string fallback)
    {
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
            var cache = GetUploadMissingCache();
            bool cachedMissing;
            if (cache.TryGetValue(cleanPath, out cachedMissing))
                return cachedMissing;

            string physical = Server.MapPath("~" + cleanPath);
            if (string.IsNullOrEmpty(physical))
                return false;
            bool missing = !File.Exists(physical);
            cache[cleanPath] = missing;
            return missing;
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

    protected string BuildExchangeActionUrl(object idRaw, object isServiceRaw, object nameEnRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);
        if (id <= 0)
            return "#";

        bool isService = false;
        if (isServiceRaw != null && isServiceRaw != DBNull.Value)
        {
            if (isServiceRaw is bool)
            {
                isService = (bool)isServiceRaw;
            }
            else
            {
                string raw = (isServiceRaw ?? "").ToString();
                if (raw == "1")
                    isService = true;
                else if (raw == "0")
                    isService = false;
                else
                    bool.TryParse(raw, out isService);
            }
        }

        if (isService)
        {
            return BuildBookingUrl(idRaw);
        }

        string returnUrl = GetReturnUrlEncoded();
        string shopAccountEncoded = GetShopAccountEncoded();
        string url = "/home/trao-doi.aspx?idsp=" + id.ToString() + "&qty=1";
        if (!string.IsNullOrEmpty(shopAccountEncoded))
            url += "&user_bancheo=" + shopAccountEncoded;
        url += "&return_url=" + returnUrl;
        return url;
    }

    protected string BuildBookingUrl(object idRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);
        if (id <= 0)
            return "#";

        string returnUrl = GetReturnUrlEncoded();
        string shopAccountEncoded = GetShopAccountEncoded();
        string url = "/shop/dat-lich.aspx?id=" + id.ToString();
        if (!string.IsNullOrWhiteSpace(shopAccountEncoded))
            url += "&user=" + shopAccountEncoded;
        url += "&return_url=" + returnUrl;
        return url;
    }

    protected string BuildAddCartActionUrl(object idRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);
        if (id <= 0)
            return "#";

        string returnUrl = GetReturnUrlEncoded();
        string shopAccountEncoded = GetShopAccountEncoded();
        string url = "/home/them-vao-gio.aspx?idsp=" + id.ToString() + "&qty=1";
        if (!string.IsNullOrEmpty(shopAccountEncoded))
            url += "&user_bancheo=" + shopAccountEncoded;
        url += "&return_url=" + returnUrl;
        return url;
    }

    private string GetShopAccount()
    {
        if (_cachedShopAccount == null)
            _cachedShopAccount = (ViewState["shop_taikhoan"] ?? "").ToString().Trim();
        return _cachedShopAccount;
    }

    private string GetShopAccountEncoded()
    {
        string account = GetShopAccount();
        if (string.IsNullOrEmpty(account))
            return "";
        if (_cachedShopAccountEncoded == null)
            _cachedShopAccountEncoded = HttpUtility.UrlEncode(account);
        return _cachedShopAccountEncoded;
    }

    private string GetReturnUrlEncoded()
    {
        if (_cachedReturnUrlEncoded == null)
            _cachedReturnUrlEncoded = HttpUtility.UrlEncode(Request.RawUrl ?? "/");
        return _cachedReturnUrlEncoded;
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

        return value.ToString("#,##0.##", ViCulture);
    }

    protected string FormatDate(object dateRaw)
    {
        DateTime date;
        if (DateTime.TryParse(Convert.ToString(dateRaw, CultureInfo.InvariantCulture), out date))
            return date.ToString("dd/MM/yyyy HH:mm");
        return "--";
    }
}
