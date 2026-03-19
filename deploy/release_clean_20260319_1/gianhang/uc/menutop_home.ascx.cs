using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class uc_menu_top : System.Web.UI.UserControl
{
    private readonly menu_homeaka_class mn_cl = new menu_homeaka_class();
    private readonly dbDataContext db = new dbDataContext();
    private List<web_menu_table> menuCache = new List<web_menu_table>();

    public string DesktopMenuHtml = string.Empty;
    public string MobileMenuHtml = string.Empty;
    public string logo = string.Empty;
    public string avt = string.Empty;
    public string hoten = string.Empty;
    public string sodiem_eaha = "0";
    public string brandName = "Gian hang";
    public string brandNote = string.Empty;
    public string hotline = string.Empty;
    public string ServicesUrl = AhaShineHomeRoutes_cl.ServicesUrl;
    public string ProductsUrl = AhaShineHomeRoutes_cl.ProductsUrl;
    public string ArticlesUrl = AhaShineHomeRoutes_cl.ArticlesUrl;
    public string NavHomeText = "Trang chu";
    public string NavBookingText = "Dat lich";
    public string QuickServiceText = "Dich vu";
    public string QuickProductText = "San pham";
    public string QuickArticleText = "Bai viet";
    public string QuickConsultText = "Tu van nhanh";
    public string QuickBookingText = "Lich hen";
    public bool ShowQuickstrip = true;
    public bool IsSignedIn;
    public int sl_hangtronggio;

    private gianhang_storefront_config_table storefrontConfig;

    protected void Page_Load(object sender, EventArgs e)
    {
        AhaShineContext_cl.EnsureContext();
        storefrontConfig = GianHangStorefrontConfig_cl.GetConfig(db, AhaShineContext_cl.ResolveChiNhanhId());
        menuCache = mn_cl.return_list()
            .Where(p => p.bin == false)
            .OrderBy(p => p.rank)
            .ThenBy(p => p.id)
            .ToList();

        LoadBrand();
        LoadLabels();
        LoadCart();
        LoadAccount();

        DesktopMenuHtml = BuildDesktopMenu();
        MobileMenuHtml = BuildMobileMenu("0", 0);
    }

    private void LoadBrand()
    {
        var config = db.config_thongtin_tables.FirstOrDefault();
        if (config == null)
            return;

        logo = (config.logo1 ?? string.Empty).Trim();
        if (logo == string.Empty)
            logo = (config.logo ?? string.Empty).Trim();

        brandName = GianHangStorefrontConfig_cl.ResolveText(config.tencongty, "Gian hang doi tac");
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
        ShowQuickstrip = storefrontConfig.quickstrip_visible ?? true;
    }

    private void LoadCart()
    {
        var giohang = Session["giohang"] as DataTable;
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
        IsSignedIn = currentUser != string.Empty;
        if (!IsSignedIn)
            return;

        var customer = db.bspa_data_khachhang_tables.FirstOrDefault(p => p.sdt == currentUser);
        if (customer == null)
            return;

        avt = (customer.anhdaidien ?? string.Empty).Trim();
        hoten = (customer.tenkhachhang ?? currentUser).Trim();
        if (hoten == string.Empty)
            hoten = currentUser;

        if (customer.sodiem_e_aha.HasValue)
            sodiem_eaha = customer.sodiem_e_aha.Value.ToString("#,##0.##");
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
                builder.Append(HttpUtility.HtmlAttributeEncode(GianHangStorefront_cl.ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other)));
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
            string url = GianHangStorefront_cl.ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other);
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
            string url = GianHangStorefront_cl.ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other);
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
            string url = GianHangStorefront_cl.ResolveMenuUrl(menu.id, menu.phanloai, menu.url_other);
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
                builder.Append("'>Xem tat ca ");
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
        Session["notifi_home"] = thongbao_class.metro_notifi_onload("Thong bao", "Dang xuat thanh cong.", "2000", "light");
        Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
    }
}
