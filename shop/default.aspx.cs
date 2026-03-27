using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class shop_default : System.Web.UI.Page
{
    private const string ShopSpacePublic = "public";
    private const string ShopSpaceInternal = "internal";
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    private readonly Dictionary<string, Control> _controlCache = new Dictionary<string, Control>(StringComparer.Ordinal);
    private static readonly string ShopDefaultReturnUrlEncoded = HttpUtility.UrlEncode("/shop/default.aspx");
    private string _cachedShopAccount;
    private string _cachedShopAccountEncoded;

    protected string ResolveShopBrandLogoUrl()
    {
        string contextLogo = PortalBranding_cl.NormalizeIconPath(
            Convert.ToString(Context == null ? null : Context.Items["AhaHeaderCenterLogoUrl"]),
            PortalBranding_cl.DefaultShopIconPath);

        if (!string.IsNullOrWhiteSpace(contextLogo))
            return contextLogo;

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                PortalBranding_cl.ScopeBrandingSnapshot branding = PortalBranding_cl.LoadScopeBranding(db, PortalBranding_cl.ScopeShop, true);
                return PortalBranding_cl.ResolveHeaderLogoPath(branding, PortalBranding_cl.ScopeShop);
            }
        }
        catch
        {
            return PortalBranding_cl.DefaultShopIconPath;
        }
    }

    private class ShopProductSummary
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string image { get; set; }
        public decimal? giaban { get; set; }
        public DateTime? ngaytao { get; set; }
        public int LuotTruyCap { get; set; }
        public string KenhRaw { get; set; }
        public int SoldCount { get; set; }
    }

    private string ResolveShopSpace(bool isCompanyShop)
    {
        if (!isCompanyShop)
            return ShopSpacePublic;

        string raw = (Request.QueryString["space"] ?? "").Trim().ToLowerInvariant();
        return raw == ShopSpaceInternal ? ShopSpaceInternal : ShopSpacePublic;
    }

    private bool IsInternalSpace()
    {
        return string.Equals((ViewState["shop_space"] ?? ShopSpacePublic).ToString(), ShopSpaceInternal, StringComparison.OrdinalIgnoreCase);
    }

    protected string GetSpaceMenuCss(string space)
    {
        string expected = (space ?? "").Trim().ToLowerInvariant();
        string current = (ViewState["shop_space"] ?? ShopSpacePublic).ToString().Trim().ToLowerInvariant();
        return expected == current ? "menu-item menu-item-active" : "menu-item";
    }

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
                if (acc == null || !SpaceAccess_cl.CanAccessShop(db, acc))
                {
                    check_login_cl.del_all_cookie_session_shop();
                    Response.Redirect("/shop/login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                bool isCompanyShop = CompanyShop_cl.IsCompanyShopAccount(db, tk);
                if (isCompanyShop)
                    CompanyShopBootstrap_cl.EnsureSystemCatalogMirrored(db);
                SetControlVisible("ph_menu_ban_san_pham", isCompanyShop);
                SetControlVisible("ph_menu_company_space", isCompanyShop);
                SetControlVisible("ph_switch_to_home", PortalActiveMode_cl.HasHomeCredential());
                SetHyperLinkNavigateUrl("lnk_switch_to_home", "/dang-nhap?switch=home");
                string space = ResolveShopSpace(isCompanyShop);
                ViewState["shop_space"] = space;

                SetHyperLinkNavigateUrl("lnk_space_public", "/shop/default.aspx?space=" + ShopSpacePublic);
                SetHyperLinkNavigateUrl("lnk_space_internal", "/shop/default.aspx?space=" + ShopSpaceInternal);
                SetWebControlCssClass("lnk_space_public", GetSpaceMenuCss(ShopSpacePublic));
                SetWebControlCssClass("lnk_space_internal", GetSpaceMenuCss(ShopSpaceInternal));

                if (isCompanyShop && space == ShopSpaceInternal)
                {
                    SetLabelText("lb_space_hero_title", "Không gian 2: Sản phẩm nội bộ");
                    SetLabelText("lb_space_hero_desc", "Dùng riêng cho sản phẩm nội bộ của công ty. Mọi thao tác bán nội bộ chạy trong cổng gian hàng đối tác.");
                    SetLabelText("lb_space_product_title", "Danh sách sản phẩm nội bộ");
                    SetLabelText("lb_space_product_desc", "Không gian này chỉ hiển thị sản phẩm nội bộ và thao tác bán nội bộ.");
                }
                else
                {
                    SetLabelText("lb_space_hero_title", "Không gian 1: Gian hàng công khai");
                    SetLabelText("lb_space_hero_desc", "Giống gian hàng đối tác thường: quản lý sản phẩm công khai, đơn bán, khách hàng và trao đổi.");
                    SetLabelText("lb_space_product_title", "Sản phẩm của gian hàng đối tác");
                    SetLabelText("lb_space_product_desc", "Trang chủ gian hàng đối tác chỉ hiển thị sản phẩm do chính tài khoản gian hàng đối tác này đăng.");
                }

                string shopAccount = string.IsNullOrWhiteSpace(acc.taikhoan) ? tk : acc.taikhoan.Trim();
                ShopToAhaShinePostSync_cl.SyncTradePostsForShopThrottled(db, shopAccount, 3);
                BindHeaderAndStats(db, acc, isCompanyShop, space);
                BindLevelActions(db, acc);
                ApplyStorefrontConfig(db, acc, isCompanyShop, space);
                BindProducts(db, shopAccount, isCompanyShop, space);
            }
        }
    }

    private string ResolveCurrentShopAccount()
    {
        if (_cachedShopAccount != null)
            return _cachedShopAccount;

        string tk = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim();
        if (!string.IsNullOrEmpty(tk))
        {
            _cachedShopAccount = tk;
            return _cachedShopAccount;
        }

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

        _cachedShopAccount = (tk ?? "").Trim();
        return _cachedShopAccount;
    }

    private string ResolveCurrentShopAccountEncoded()
    {
        string account = ResolveCurrentShopAccount();
        if (string.IsNullOrEmpty(account))
            return "";
        if (_cachedShopAccountEncoded == null)
            _cachedShopAccountEncoded = HttpUtility.UrlEncode(account);
        return _cachedShopAccountEncoded;
    }

    private void BindHeaderAndStats(dbDataContext db, taikhoan_tb acc, bool isCompanyShop, string space)
    {
        string displayName = string.IsNullOrWhiteSpace(acc.hoten) ? acc.taikhoan : acc.hoten.Trim();
        string avatar = string.IsNullOrWhiteSpace(acc.anhdaidien) ? "/uploads/images/macdinh.jpg" : acc.anhdaidien.Trim();
        string publicPath = ShopSlug_cl.GetPublicUrl(db, acc);
        string fullPublicUrl = Request.Url.GetLeftPart(UriPartial.Authority) + publicPath;

        SetLabelText("lb_taikhoan", acc.taikhoan);
        SetLabelText("lb_shop_brand_title", "Shop Portal");
        SetLabelText("lb_hoten", displayName);
        SetLabelText("lb_hoten_short", displayName);

        string phanLoaiText = string.IsNullOrWhiteSpace(acc.phanloai) ? "Gian hàng đối tác" : acc.phanloai;
        if (isCompanyShop)
            phanLoaiText += " • Shop công ty • " + (space == ShopSpaceInternal ? "Không gian nội bộ" : "Không gian công khai");
        SetLabelText("lb_phanloai", phanLoaiText);
        string shopStatusText = ShopStatus_cl.GetStatusText(db, acc);
        SetLabelText("lb_trangthai", shopStatusText);
        SetImageUrl("img_avatar", avatar);

        SetLabelText("lb_public_path", publicPath);

        SetHyperLinkNavigateUrl("lnk_public_shop", publicPath);
        SetHyperLinkText("lnk_public_shop", "Xem trang công khai");
        SetHyperLinkNavigateUrl("lnk_public_shop_top", publicPath);
        SetHyperLinkText("lnk_public_shop_top", fullPublicUrl);
    }

    private void BindLevelActions(dbDataContext db, taikhoan_tb acc)
    {
        if (db == null || acc == null)
            return;

        bool hasAdvanced = ShopLevel_cl.IsAdvancedEnabled(db, acc.taikhoan);
        if (hasAdvanced)
            AhaShineContext_cl.EnsureAdvancedAdminBootstrapForShop(db, acc.taikhoan);
        SetLabelText("lb_shop_level", hasAdvanced ? "Level 2" : "Level 1");
        SetLabelText("lb_shop_level_hint", hasAdvanced ? "Đã mở công cụ quản trị nâng cao tại /gianhang/admin" : "Đang dùng bộ công cụ cơ bản trong /shop");
        SetControlVisible("ph_menu_advanced_tools", hasAdvanced);
        SetControlVisible("ph_menu_advanced_upgrade", !hasAdvanced);

        ShopLevel2Request_cl.Level2RequestInfo latestRequest = null;
        if (!hasAdvanced)
            latestRequest = ShopLevel2Request_cl.GetLatestRequest(db, acc.taikhoan);

        bool pendingRequest = !hasAdvanced && latestRequest != null && latestRequest.TrangThai == ShopLevel2Request_cl.StatusPending;
        bool rejectedRequest = !hasAdvanced && latestRequest != null && latestRequest.TrangThai == ShopLevel2Request_cl.StatusRejected;

        SetControlVisible("ph_level2_pending_notice", pendingRequest);
        SetControlVisible("ph_level2_reject_notice", rejectedRequest);

        if (pendingRequest)
        {
            string pendingTime = latestRequest.NgayTao.HasValue
                ? latestRequest.NgayTao.Value.ToString("dd/MM/yyyy HH:mm")
                : "--";
            SetLabelText("lb_level2_pending_time", pendingTime);
        }
        else
        {
            SetLabelText("lb_level2_pending_time", "--");
        }

        if (rejectedRequest)
        {
            string note = string.IsNullOrWhiteSpace(latestRequest.GhiChuAdmin) ? "Admin chưa ghi chú." : latestRequest.GhiChuAdmin.Trim();
            SetLabelText("lb_level2_reject_note", note);
        }
        else
        {
            SetLabelText("lb_level2_reject_note", "Admin chưa ghi chú.");
        }
    }

    private void ApplyStorefrontConfig(dbDataContext db, taikhoan_tb acc, bool isCompanyShop, string space)
    {
        if (db == null || acc == null)
            return;

        string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, acc.taikhoan);

        gianhang_storefront_config_table config = GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);
        gianhang_storefront_section_table featuredProducts = GianHangStorefrontConfig_cl.GetSection(db, chiNhanhId, GianHangStorefrontConfig_cl.SectionFeaturedProducts);

        string publicUrl = ShopSlug_cl.GetPublicUrl(db, acc);
        bool usePublicConfig = !(isCompanyShop && string.Equals(space, ShopSpaceInternal, StringComparison.OrdinalIgnoreCase));

        string heroTitleFallback = string.Equals(space, ShopSpaceInternal, StringComparison.OrdinalIgnoreCase)
            ? "Không gian 2: Sản phẩm nội bộ"
            : "Không gian 1: Gian hàng công khai";
        string heroDescFallback = string.Equals(space, ShopSpaceInternal, StringComparison.OrdinalIgnoreCase)
            ? "Dùng riêng cho sản phẩm nội bộ của công ty. Mọi thao tác bán nội bộ chạy trong cổng gian hàng đối tác."
            : "Giống gian hàng đối tác thường: quản lý sản phẩm công khai, đơn bán, khách hàng và trao đổi.";

        string heroTitle = usePublicConfig
            ? GianHangStorefrontConfig_cl.ResolveText(config.hero_title, heroTitleFallback)
            : heroTitleFallback;
        string heroDesc = usePublicConfig
            ? GianHangStorefrontConfig_cl.ResolveText(config.hero_description, heroDescFallback)
            : heroDescFallback;

        string topTitle = usePublicConfig
            ? GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_title, "Top sản phẩm")
            : "Top sản phẩm";
        string topDesc = usePublicConfig
            ? GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_description, "Ưu tiên theo bán chạy, nếu chưa có sẽ theo lượt xem.")
            : "Ưu tiên theo bán chạy, nếu chưa có sẽ theo lượt xem.";

        string productTitleFallback = string.Equals(space, ShopSpaceInternal, StringComparison.OrdinalIgnoreCase)
            ? "Danh sách sản phẩm nội bộ"
            : "Sản phẩm của gian hàng đối tác";
        string productDescFallback = string.Equals(space, ShopSpaceInternal, StringComparison.OrdinalIgnoreCase)
            ? "Không gian này chỉ hiển thị sản phẩm nội bộ và thao tác bán nội bộ."
            : "Trang chủ gian hàng đối tác chỉ hiển thị sản phẩm do chính tài khoản gian hàng đối tác này đăng.";

        string productTitle = usePublicConfig && featuredProducts != null
            ? GianHangStorefrontConfig_cl.ResolveText(featuredProducts.title, productTitleFallback)
            : productTitleFallback;
        string productDesc = usePublicConfig && featuredProducts != null
            ? GianHangStorefrontConfig_cl.ResolveText(featuredProducts.description, productDescFallback)
            : productDescFallback;

        SetLabelText("lb_space_hero_title", heroTitle);
        SetLabelText("lb_space_hero_desc", heroDesc);
        SetLabelText("lb_top_products_title", topTitle);
        SetLabelText("lb_top_products_desc", topDesc);
        SetLabelText("lb_space_product_title", productTitle);
        SetLabelText("lb_space_product_desc", productDesc);

        string primaryText = usePublicConfig ? (config.hero_primary_text ?? "").Trim() : "";
        string primaryUrl = usePublicConfig ? (config.hero_primary_url ?? "").Trim() : "";
        string secondaryText = usePublicConfig ? (config.hero_secondary_text ?? "").Trim() : "";
        string secondaryUrl = usePublicConfig ? (config.hero_secondary_url ?? "").Trim() : "";

        if (string.IsNullOrWhiteSpace(primaryUrl))
            primaryUrl = publicUrl;
        primaryUrl = ResolveShopConfigUrl(primaryUrl, publicUrl);
        secondaryUrl = ResolveShopConfigUrl(secondaryUrl, "/shop/quan-ly-tin");

        SetHyperLinkText("lnk_shop_primary_cta", primaryText);
        SetHyperLinkNavigateUrl("lnk_shop_primary_cta", primaryUrl);
        SetControlVisible("lnk_shop_primary_cta", !string.IsNullOrWhiteSpace(primaryText));

        SetHyperLinkText("lnk_shop_secondary_cta", secondaryText);
        SetHyperLinkNavigateUrl("lnk_shop_secondary_cta", secondaryUrl);
        SetControlVisible("lnk_shop_secondary_cta", !string.IsNullOrWhiteSpace(secondaryText));
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

    private void BindProducts(dbDataContext db, string tk, bool isCompanyShop, string space)
    {
        IQueryable<BaiViet_tb> source = db.BaiViet_tbs.Where(p => p.nguoitao == tk);

        // Public space must use the same visibility filter as shop public page
        // to avoid product/price mismatch between two views.
        if (isCompanyShop && space == ShopSpaceInternal)
        {
            source = source.Where(p => (p.bin == false || p.bin == null) && p.phanloai == CompanyShop_cl.ProductTypeInternal);
        }
        else
        {
            source = source.Where(p =>
                (p.bin == false || p.bin == null)
                && (p.phanloai == AccountVisibility_cl.PostTypeProduct || p.phanloai == AccountVisibility_cl.PostTypeService));
        }

        var products = source
            .OrderByDescending(p => p.ngaytao)
            .Select(p => new ShopProductSummary
            {
                id = p.id,
                name = p.name,
                name_en = p.name_en,
                image = p.image,
                giaban = p.giaban,
                ngaytao = p.ngaytao,
                LuotTruyCap = (p.LuotTruyCap ?? 0),
                KenhRaw = p.phanloai,
                SoldCount = (p.soluong_daban ?? 0)
            })
            .ToList();

        BindRepeater("rp_products", products);
        SetControlVisible("ph_empty_products", products.Count == 0);

        BindTopProducts(db, tk, products);
    }

    private void BindTopProducts(dbDataContext db, string tk, List<ShopProductSummary> products)
    {
        if (db == null || products == null || products.Count == 0)
        {
            SetControlVisible("ph_top_products", false);
            SetControlVisible("ph_empty_top_products", true);
            return;
        }

        var productIds = new List<string>(products.Count);
        foreach (var product in products)
            productIds.Add(product.id.ToString());
        var soldMap = db.DonHang_ChiTiet_tbs
            .Where(p => (p.nguoiban_goc == tk || p.nguoiban_danglai == tk)
                        && productIds.Contains(p.idsp))
            .GroupBy(p => p.idsp)
            .Select(g => new { idsp = g.Key, total = g.Sum(x => (int?)x.soluong) ?? 0 })
            .ToList()
            .Where(p => !string.IsNullOrWhiteSpace(p.idsp))
            .ToDictionary(p => p.idsp, p => p.total);

        foreach (var product in products)
        {
            string key = product.id.ToString();
            int sold;
            if (soldMap.TryGetValue(key, out sold))
                product.SoldCount = sold;
        }

        var ordered = products
            .OrderByDescending(p => p.SoldCount)
            .ThenByDescending(p => p.LuotTruyCap)
            .Take(5)
            .ToList();

        if (ordered.All(p => p.SoldCount == 0))
        {
            ordered = products
                .OrderByDescending(p => p.LuotTruyCap)
                .Take(5)
                .ToList();
        }

        var view = ordered.Select(p => new
        {
            Id = p.id,
            Name = p.name,
            Image = ResolveProductImage(p.image),
            Sold = p.SoldCount,
            Views = p.LuotTruyCap
        }).ToList();

        bool hasTop = view.Count > 0;
        SetControlVisible("ph_top_products", hasTop);
        SetControlVisible("ph_empty_top_products", !hasTop);
        BindRepeater("rpt_top_products", view);
    }

    protected string BuildProductChannelLabel(object raw)
    {
        string phanloai = (raw ?? "").ToString();
        return CompanyShop_cl.IsInternalProductType(phanloai) ? "Nội bộ" : "Công khai";
    }

    protected string BuildProductChannelCss(object raw)
    {
        string phanloai = (raw ?? "").ToString();
        if (CompanyShop_cl.IsInternalProductType(phanloai))
            return "product-badge product-badge-internal";
        return "product-badge product-badge-public";
    }

    protected string BuildSellActionUrl(object idRaw, object nameEnRaw, object nameRaw, object productTypeRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);

        string phanloai = (productTypeRaw ?? "").ToString();
        if (CompanyShop_cl.IsInternalProductType(phanloai))
            return "/shop/noi-bo/ban-san-pham?view=sell&idsp=" + id.ToString();

        if (string.Equals(phanloai.Trim(), AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase))
            return BuildBookingUrl(idRaw);

        return BuildCreateOrderUrl(idRaw);
    }

    protected string BuildSellActionText(object productTypeRaw)
    {
        string phanloai = (productTypeRaw ?? "").ToString();
        if (CompanyShop_cl.IsInternalProductType(phanloai))
            return "Bán nội bộ";
        if (string.Equals(phanloai.Trim(), AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase))
            return "Đặt lịch";
        return "Tạo đơn";
    }

    protected bool IsServicePost(object productTypeRaw)
    {
        string phanloai = (productTypeRaw ?? "").ToString();
        return string.Equals(phanloai.Trim(), AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase);
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

            const string cacheKey = "__aha_missing_upload_cache_shop";
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

    private void BindRepeater(string id, object dataSource)
    {
        Repeater ctl = FindControlById(id) as Repeater;
        if (ctl != null)
        {
            ctl.DataSource = dataSource;
            ctl.DataBind();
        }
    }

    private void SetWebControlCssClass(string id, string cssClass)
    {
        WebControl ctl = FindControlById(id) as WebControl;
        if (ctl != null)
            ctl.CssClass = cssClass ?? "";
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

    protected string BuildCreateOrderUrl(object idRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);

        string baseUrl = "/shop/don-ban?taodon=1";
        if (id > 0)
            baseUrl += "&idsp=" + id.ToString();

        baseUrl += "&return_url=" + ShopDefaultReturnUrlEncoded;
        return baseUrl;
    }

    protected string BuildBookingUrl(object idRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);

        string shopAccountEncoded = ResolveCurrentShopAccountEncoded();
        var sb = new StringBuilder("/shop/dat-lich.aspx?");
        bool hasParam = false;

        if (id > 0)
        {
            sb.Append("id=").Append(id.ToString());
            hasParam = true;
        }

        if (!string.IsNullOrWhiteSpace(shopAccountEncoded))
        {
            if (hasParam)
                sb.Append("&");
            sb.Append("user=").Append(shopAccountEncoded);
            hasParam = true;
        }

        if (hasParam)
            sb.Append("&");
        sb.Append("return_url=").Append(ShopDefaultReturnUrlEncoded);
        return sb.ToString();
    }

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        check_login_cl.del_all_cookie_session_shop();
        Response.Redirect("/shop/login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
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
}
