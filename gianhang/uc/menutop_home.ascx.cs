using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class uc_menu_top : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();
    private List<web_menu_table> menuCache = new List<web_menu_table>();
    private bool hasTransientDataIssue;

    public string DesktopMenuHtml = string.Empty;
    public string MobileMenuHtml = string.Empty;
    public string logo = string.Empty;
    public string avt = string.Empty;
    public string hoten = string.Empty;
    public string sodiem_eaha = "0";
    public string brandName = "Gian hàng";
    public string brandNote = string.Empty;
    public string hotline = string.Empty;
    public string HomeUrl = GianHangRoutes_cl.BuildStorefrontUrl(string.Empty);
    public string BookingUrl = GianHangRoutes_cl.BuildBookingHubUrl(string.Empty, string.Empty);
    public string CartUrl = GianHangRoutes_cl.BuildCartUrl(string.Empty, string.Empty);
    public string ServicesUrl = GianHangRoutes_cl.BuildServicesUrl(string.Empty);
    public string ProductsUrl = GianHangRoutes_cl.BuildProductsUrl(string.Empty);
    public string ArticlesUrl = GianHangRoutes_cl.BuildArticlesUrl(string.Empty);
    public string NavHomeText = "Trang chủ";
    public string NavBookingText = "Đặt lịch";
    public string QuickServiceText = "Dịch vụ";
    public string QuickProductText = "Sản phẩm";
    public string QuickArticleText = "Bài viết";
    public string QuickConsultText = "Tư vấn nhanh";
    public string QuickBookingText = "Lịch hẹn";
    public bool ShowQuickstrip = true;
    public bool IsSignedIn;
    public int sl_hangtronggio;
    public string accountLogin = string.Empty;
    public string spaceStatus = "Khách hàng";
    public string spacePublicUrl = "/gianhang/public.aspx";
    public string spaceManageUrl = "/gianhang/quan-ly-tin/Default.aspx";
    public string spaceOrdersUrl = "/gianhang/don-ban.aspx";
    public string spaceBookingUrl = "/gianhang/quan-ly-lich-hen.aspx";
    public string spaceCustomersUrl = "/gianhang/khach-hang.aspx";
    public string spaceReportUrl = "/gianhang/bao-cao.aspx";
    public string spaceLevel2Url = "/gianhang/nang-cap-level2.aspx";
    public string spaceHomeUrl = "/home/default.aspx";
    public string spaceAdminUrl = "/gianhang/admin";
    public bool showSpaceAdmin;

    private gianhang_storefront_config_table storefrontConfig;
    private string currentStoreAccount = string.Empty;
    private string currentChiNhanhId = "1";
    private string activeSpaceKey = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            LoadStorefrontContext();
            GianHangContext_cl.RememberStorefrontContext(Session, currentStoreAccount, currentChiNhanhId);
            storefrontConfig = TrySql(
                () => GianHangStorefrontConfig_cl.GetConfig(db, currentChiNhanhId),
                null);

            LoadBrand();
            LoadLabels();
            LoadCart();
            LoadAccount();
            LoadSpaceLinks();
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            hasTransientDataIssue = true;
            ApplyFallbackState();
        }
    }

    private void LoadStorefrontContext()
    {
        GianHangPublic_cl.StorefrontContextInfo context = TrySql(
            () => GianHangPublic_cl.ResolveContext(db, Request),
            new GianHangPublic_cl.StorefrontContextInfo());
        currentStoreAccount = (context.AccountKey ?? string.Empty).Trim().ToLowerInvariant();
        currentChiNhanhId = (context.ChiNhanhId ?? string.Empty).Trim();
        if (currentChiNhanhId == string.Empty)
            currentChiNhanhId = TrySql(() => GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request), "1");
        if (currentChiNhanhId == string.Empty)
            currentChiNhanhId = "1";
        HomeUrl = string.IsNullOrWhiteSpace(context.HomeUrl) ? GianHangRoutes_cl.BuildStorefrontUrl(currentStoreAccount) : context.HomeUrl;
        BookingUrl = string.IsNullOrWhiteSpace(context.BookingUrl) ? GianHangRoutes_cl.BuildBookingHubUrl(currentStoreAccount, Request.RawUrl) : context.BookingUrl;
        CartUrl = string.IsNullOrWhiteSpace(context.CartUrl) ? GianHangRoutes_cl.BuildCartUrl(currentStoreAccount, Request.RawUrl) : context.CartUrl;
        ServicesUrl = string.IsNullOrWhiteSpace(context.ServicesUrl) ? GianHangRoutes_cl.BuildServicesUrl(currentStoreAccount) : context.ServicesUrl;
        ProductsUrl = string.IsNullOrWhiteSpace(context.ProductsUrl) ? GianHangRoutes_cl.BuildProductsUrl(currentStoreAccount) : context.ProductsUrl;
        ArticlesUrl = string.IsNullOrWhiteSpace(context.ArticlesUrl) ? GianHangRoutes_cl.BuildArticlesUrl(currentStoreAccount) : context.ArticlesUrl;

        if (currentStoreAccount != string.Empty)
            GianHangCart_cl.RememberActiveStorefrontAccount(currentStoreAccount);
    }

    private void LoadBrand()
    {
        var config = TrySql(() => db.config_thongtin_tables.FirstOrDefault(), null);
        if (config == null)
            return;

        logo = (config.logo1 ?? string.Empty).Trim();
        if (logo == string.Empty)
            logo = (config.logo ?? string.Empty).Trim();

        brandName = GianHangStorefrontConfig_cl.ResolveText(config.tencongty, "Gian hàng đối tác");
        brandNote = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.brand_note, config.slogan);
        hotline = (config.hotline ?? string.Empty).Trim();
    }

    private void LoadLabels()
    {
        NavHomeText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.nav_home_text, NavHomeText);
        NavBookingText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.nav_booking_text, NavBookingText);
        QuickServiceText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_service_text, QuickServiceText);
        QuickProductText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_product_text, QuickProductText);
        QuickArticleText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_article_text, QuickArticleText);
        QuickConsultText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_consult_text, QuickConsultText);
        QuickBookingText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_booking_text, QuickBookingText);
        ShowQuickstrip = GianHangStorefrontConfig_cl.ResolveBool(storefrontConfig.quickstrip_visible, true);
    }

    private void LoadCart()
    {
        string gianHangTaiKhoan = GianHangCart_cl.GetActiveStorefrontAccount();
        DataTable giohang = null;
        try
        {
            giohang = GianHangCart_cl.GetCart(gianHangTaiKhoan, false);
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            hasTransientDataIssue = true;
            giohang = null;
        }
        if (giohang == null)
            return;

        foreach (DataRow row in giohang.Rows)
        {
            int soLuong;
            if (int.TryParse((row["soluong"] ?? "0").ToString(), out soLuong))
                sl_hangtronggio += soLuong;
        }
    }

    private void LoadAccount()
    {
        string currentUser = (Session["user_home"] ?? string.Empty).ToString().Trim();
        accountLogin = currentUser;
        hoten = currentUser;
        IsSignedIn = currentUser != string.Empty;
        if (!IsSignedIn)
            return;

        var customer = TrySql(
            () => db.bspa_data_khachhang_tables.FirstOrDefault(p => p.sdt == currentUser),
            null);
        if (customer == null)
            return;

        avt = (customer.anhdaidien ?? string.Empty).Trim();
        hoten = (customer.tenkhachhang ?? currentUser).Trim();
        if (hoten == string.Empty)
            hoten = currentUser;

        if (customer.sodiem_e_aha.HasValue)
            sodiem_eaha = customer.sodiem_e_aha.Value.ToString("#,##0.##");
    }

    private void LoadSpaceLinks()
    {
        GianHangSpaceNav_cl.SpaceNavModel model = TrySql(
            () => GianHangSpaceNav_cl.BuildCurrent(Request),
            new GianHangSpaceNav_cl.SpaceNavModel());

        if (model == null)
            model = new GianHangSpaceNav_cl.SpaceNavModel();

        if (model.Visible)
        {
            spacePublicUrl = ResolveDefault(model.PublicUrl, spacePublicUrl);
            spaceManageUrl = ResolveDefault(model.ManageUrl, spaceManageUrl);
            spaceOrdersUrl = ResolveDefault(model.OrdersUrl, spaceOrdersUrl);
            spaceBookingUrl = ResolveDefault(model.BookingUrl, spaceBookingUrl);
            spaceCustomersUrl = ResolveDefault(model.CustomersUrl, spaceCustomersUrl);
            spaceReportUrl = ResolveDefault(model.ReportUrl, spaceReportUrl);
            spaceLevel2Url = ResolveDefault(model.Level2Url, spaceLevel2Url);
            spaceHomeUrl = ResolveDefault(model.HomeUrl, spaceHomeUrl);
            spaceAdminUrl = ResolveDefault(model.AdminUrl, spaceAdminUrl);
            showSpaceAdmin = model.ShowAdminUrl;
            activeSpaceKey = (model.ActiveKey ?? string.Empty).Trim().ToLowerInvariant();

            string modelStatus = (model.StatusText ?? string.Empty).Trim();
            if (modelStatus != string.Empty)
                spaceStatus = modelStatus;

            if (avt == string.Empty && !string.IsNullOrWhiteSpace(model.AvatarUrl))
                avt = model.AvatarUrl.Trim();
            if (hoten == string.Empty && !string.IsNullOrWhiteSpace(model.StoreName))
                hoten = model.StoreName.Trim();
            if (accountLogin == string.Empty && !string.IsNullOrWhiteSpace(model.AccountKey))
                accountLogin = model.AccountKey.Trim();
            return;
        }

        string accountKey = (accountLogin ?? string.Empty).Trim().ToLowerInvariant();
        spacePublicUrl = GianHangRoutes_cl.BuildStorefrontUrl(accountKey);
        spaceManageUrl = "/gianhang/quan-ly-tin/Default.aspx";
        spaceOrdersUrl = "/gianhang/don-ban.aspx";
        spaceBookingUrl = "/gianhang/quan-ly-lich-hen.aspx";
        spaceCustomersUrl = "/gianhang/khach-hang.aspx";
        spaceReportUrl = "/gianhang/bao-cao.aspx";
        spaceLevel2Url = "/gianhang/nang-cap-level2.aspx";
        spaceHomeUrl = "/home/default.aspx";
        spaceAdminUrl = "/gianhang/admin";
        showSpaceAdmin = false;
        activeSpaceKey = ResolveSpaceKey(Request == null || Request.Url == null ? string.Empty : Request.Url.AbsolutePath);
    }

    private static string ResolveDefault(string value, string fallback)
    {
        string text = (value ?? string.Empty).Trim();
        return text == string.Empty ? fallback : text;
    }

    private static string ResolveSpaceKey(string path)
    {
        string normalized = (path ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized.StartsWith("/gianhang/quan-ly-tin"))
            return "manage";
        if (normalized.StartsWith("/gianhang/don-ban") || normalized.StartsWith("/gianhang/cho-thanh-toan"))
            return "orders";
        if (normalized.StartsWith("/gianhang/quan-ly-lich-hen") || normalized.StartsWith("/gianhang/dat-lich"))
            return "booking";
        if (normalized.StartsWith("/gianhang/khach-hang"))
            return "customers";
        if (normalized.StartsWith("/gianhang/bao-cao"))
            return "report";
        if (normalized.StartsWith("/gianhang/nang-cap-level2"))
            return "level2";
        if (normalized.StartsWith("/gianhang/admin"))
            return "admin";
        if (normalized.StartsWith("/gianhang/public")
            || normalized.StartsWith("/gianhang/page/")
            || normalized.StartsWith("/gianhang/xem-san-pham")
            || normalized.StartsWith("/gianhang/xem-dich-vu")
            || normalized.StartsWith("/gianhang/giohang"))
            return "public";
        return string.Empty;
    }

    public string GetSpaceLinkCss(string key)
    {
        string css = "storefront-account-space__link";
        string normalizedKey = (key ?? string.Empty).Trim().ToLowerInvariant();
        if (normalizedKey != string.Empty && normalizedKey == activeSpaceKey)
            css += " is-active";
        return css;
    }

    private bool HasChildren(string parentId)
    {
        string normalizedParent = (parentId ?? string.Empty).Trim();
        return menuCache.Any(p => p.id_parent == normalizedParent);
    }

    private string BuildDesktopMenu()
    {
        StringBuilder builder = new StringBuilder();
        foreach (var menu in menuCache.Where(p => p.id_parent == "0"))
        {
            if (HasChildren(menu.id.ToString()))
            {
                builder.Append("<li class='storefront-nav__item storefront-nav__item--dropdown'>");
                builder.Append("<details class='storefront-nav__details'>");
                builder.Append("<summary class='storefront-nav__link'>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</summary>");
                builder.Append("<div class='storefront-nav__panel'><div class='storefront-nav__panel-grid'>");
                builder.Append(BuildDesktopBranch(menu.id.ToString()));
                builder.Append("</div></div>");
                builder.Append("</details>");
                builder.Append("</li>");
            }
            else
            {
                builder.Append("<li class='storefront-nav__item'>");
                builder.Append("<a class='storefront-nav__link' href='");
                builder.Append(HttpUtility.HtmlAttributeEncode(ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other)));
                builder.Append("'>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</a></li>");
            }
        }

        return builder.ToString();
    }

    private string BuildDesktopBranch(string parentId)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var menu in menuCache.Where(p => p.id_parent == parentId))
        {
            string url = ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other);
            if (HasChildren(menu.id.ToString()))
            {
                builder.Append("<div class='storefront-nav__group'>");
                builder.Append("<a class='storefront-nav__group-title' href='");
                builder.Append(HttpUtility.HtmlAttributeEncode(url));
                builder.Append("'>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</a>");
                if (!string.IsNullOrWhiteSpace(menu.description))
                {
                    builder.Append("<div class='storefront-nav__group-note'>");
                    builder.Append(HttpUtility.HtmlEncode(menu.description));
                    builder.Append("</div>");
                }
                builder.Append("<div class='storefront-nav__group-links'>");
                builder.Append(BuildDesktopLeafLinks(menu.id.ToString()));
                builder.Append("</div></div>");
            }
            else
            {
                builder.Append("<a class='storefront-nav__solo' href='");
                builder.Append(HttpUtility.HtmlAttributeEncode(url));
                builder.Append("'>");
                builder.Append("<span class='storefront-nav__solo-title'>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</span>");
                if (!string.IsNullOrWhiteSpace(menu.description))
                {
                    builder.Append("<span class='storefront-nav__solo-note'>");
                    builder.Append(HttpUtility.HtmlEncode(menu.description));
                    builder.Append("</span>");
                }
                builder.Append("</a>");
            }
        }

        return builder.ToString();
    }

    private string BuildDesktopLeafLinks(string parentId)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var menu in menuCache.Where(p => p.id_parent == parentId))
        {
            string url = ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other);
            if (HasChildren(menu.id.ToString()))
            {
                builder.Append("<div class='storefront-nav__subgroup'>");
                builder.Append("<a class='storefront-nav__subgroup-title' href='");
                builder.Append(HttpUtility.HtmlAttributeEncode(url));
                builder.Append("'>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</a>");
                builder.Append("<div class='storefront-nav__subgroup-links'>");
                builder.Append(BuildDesktopLeafLinks(menu.id.ToString()));
                builder.Append("</div></div>");
            }
            else
            {
                builder.Append("<a class='storefront-nav__leaf' href='");
                builder.Append(HttpUtility.HtmlAttributeEncode(url));
                builder.Append("'>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</a>");
            }
        }

        return builder.ToString();
    }

    private string BuildMobileMenu(string parentId, int level)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var menu in menuCache.Where(p => p.id_parent == parentId))
        {
            string url = ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other);
            bool hasChildren = HasChildren(menu.id.ToString());
            builder.Append("<li class='storefront-mobile-nav__item storefront-mobile-nav__item--level-");
            builder.Append(level);
            builder.Append("'>");

            if (hasChildren)
            {
                builder.Append("<details class='storefront-mobile-nav__details'>");
                builder.Append("<summary>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</summary>");
                builder.Append("<div class='storefront-mobile-nav__children'>");
                builder.Append("<a class='storefront-mobile-nav__overview' href='");
                builder.Append(HttpUtility.HtmlAttributeEncode(url));
                builder.Append("'>Xem tất cả ");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</a>");
                builder.Append("<ul class='storefront-mobile-nav__list'>");
                builder.Append(BuildMobileMenu(menu.id.ToString(), level + 1));
                builder.Append("</ul></div></details>");
            }
            else
            {
                builder.Append("<a class='storefront-mobile-nav__link' href='");
                builder.Append(HttpUtility.HtmlAttributeEncode(url));
                builder.Append("'>");
                builder.Append(HttpUtility.HtmlEncode(menu.name));
                builder.Append("</a>");
            }

            builder.Append("</li>");
        }

        return builder.ToString();
    }

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        Session["user_home"] = string.Empty;
        app_cookie_policy_class.expire_cookie(HttpContext.Current, app_cookie_policy_class.home_user_cookie);
        app_cookie_policy_class.expire_cookie(HttpContext.Current, app_cookie_policy_class.home_pass_cookie);
        app_cookie_policy_class.expire_cookie(HttpContext.Current, app_cookie_policy_class.home_return_url_cookie);
        Session["notifi_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "2000", "light");
        Response.Redirect(HomeUrl);
    }

    private string ResolveMenuUrl(object idObject, object phanloaiObject, object urlOtherObject)
    {
        try
        {
            return GianHangPublic_cl.BuildContextMenuUrl(db, Request, idObject, phanloaiObject, urlOtherObject);
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            hasTransientDataIssue = true;
            return GianHangRoutes_cl.BuildStorefrontUrl(currentStoreAccount);
        }
    }

    private T TrySql<T>(Func<T> action, T fallback)
    {
        try
        {
            return SqlTransientGuard_cl.Execute(action, 3, 200);
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            hasTransientDataIssue = true;
            return fallback;
        }
    }

    private void ApplyFallbackState()
    {
        if (currentChiNhanhId == string.Empty)
            currentChiNhanhId = "1";
        if (brandName == string.Empty)
            brandName = "Gian hàng đối tác";
        if (brandNote == string.Empty)
            brandNote = "Đang đồng bộ dữ liệu, vui lòng tải lại sau vài giây.";
        if (HomeUrl == string.Empty)
            HomeUrl = GianHangRoutes_cl.BuildStorefrontUrl(currentStoreAccount);
        if (BookingUrl == string.Empty)
            BookingUrl = GianHangRoutes_cl.BuildBookingHubUrl(currentStoreAccount, string.Empty);
        if (CartUrl == string.Empty)
            CartUrl = GianHangRoutes_cl.BuildCartUrl(currentStoreAccount, string.Empty);
        if (ServicesUrl == string.Empty)
            ServicesUrl = GianHangRoutes_cl.BuildServicesUrl(currentStoreAccount);
        if (ProductsUrl == string.Empty)
            ProductsUrl = GianHangRoutes_cl.BuildProductsUrl(currentStoreAccount);
        if (ArticlesUrl == string.Empty)
            ArticlesUrl = GianHangRoutes_cl.BuildArticlesUrl(currentStoreAccount);
        menuCache = new List<web_menu_table>();

        if (hasTransientDataIssue && hotline == string.Empty)
            hotline = string.Empty;
    }
}
