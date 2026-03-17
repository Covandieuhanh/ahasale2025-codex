using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class uc_footer_uc : System.Web.UI.UserControl
{
    public string logo = string.Empty;
    public string tencongty = string.Empty;
    public string slogan = string.Empty;
    public string diachi = string.Empty;
    public string hotline = string.Empty;
    public string email = string.Empty;
    public string zalo = string.Empty;
    public string fb = string.Empty;
    public string ytb = string.Empty;
    public string ins = string.Empty;
    public string twitter = string.Empty;
    public string tiktok = string.Empty;
    public string wechat = string.Empty;
    public string linkedin = string.Empty;
    public string whatsapp = string.Empty;
    public string code_map = string.Empty;
    public int CurrentYear = DateTime.Now.Year;
    public string ServicesUrl = AhaShineHomeRoutes_cl.ServicesUrl;
    public string ProductsUrl = AhaShineHomeRoutes_cl.ProductsUrl;
    public string ArticlesUrl = AhaShineHomeRoutes_cl.ArticlesUrl;
    public string FooterDescription = string.Empty;
    public string FooterChip1 = string.Empty;
    public string FooterChip2 = string.Empty;
    public string FooterChip3 = string.Empty;
    public string FooterChip4 = string.Empty;
    public string FooterNavTitle = "Dieu huong nhanh";
    public string FooterCategoryTitle = "Danh muc tu admin";
    public string FooterContactTitle = "Lien he";
    public string FooterBottomPrimaryText = "Dang ky tu van";
    public string FooterBottomPrimaryUrl = "javascript:void(0)";
    public string FooterBottomPrimaryAttr = string.Empty;
    public string FooterBottomSecondaryText = "Dat lich ngay";
    public string FooterBottomSecondaryUrl = AhaShineHomeRoutes_cl.BookingUrl;
    public string NavHomeText = "Trang chu";
    public string NavBookingText = "Dat lich";
    public string QuickServiceText = "Dich vu";
    public string QuickProductText = "San pham";
    public string QuickArticleText = "Bai viet";

    private readonly dbDataContext db = new dbDataContext();
    private readonly menu_homeaka_class mn_cl = new menu_homeaka_class();

    protected void Page_Load(object sender, EventArgs e)
    {
        var storefrontConfig = GianHangStorefrontConfig_cl.GetConfig(db, AhaShineContext_cl.ResolveChiNhanhId());
        LoadInfo();
        LoadSocial();
        LoadEmbed();
        LoadStorefrontConfig(storefrontConfig);

        Repeater1.DataSource = mn_cl.return_list()
            .Where(p => p.id_parent == "0" && p.bin == false)
            .OrderBy(p => p.rank)
            .ThenBy(p => p.id);
        Repeater1.DataBind();
    }

    private void LoadInfo()
    {
        var info = db.config_thongtin_tables.FirstOrDefault();
        if (info == null)
            return;

        logo = (info.logo ?? string.Empty).Trim();
        tencongty = GianHangStorefrontConfig_cl.ResolveText(info.tencongty, "Gian hang doi tac");
        diachi = (info.diachi ?? string.Empty).Trim();
        hotline = (info.hotline ?? string.Empty).Trim();
        email = (info.email ?? string.Empty).Trim();
        slogan = (info.slogan ?? string.Empty).Trim();
    }

    private void LoadSocial()
    {
        var social = db.config_social_media_tables.FirstOrDefault();
        if (social == null)
            return;

        fb = social.facebook ?? string.Empty;
        zalo = social.zalo ?? string.Empty;
        ytb = social.youtube ?? string.Empty;
        ins = social.instagram ?? string.Empty;
        twitter = social.twitter ?? string.Empty;
        tiktok = social.tiktok ?? string.Empty;
        wechat = social.wechat ?? string.Empty;
        linkedin = social.linkedin ?? string.Empty;
        whatsapp = social.whatsapp ?? string.Empty;
    }

    private void LoadEmbed()
    {
        var embed = db.config_nhungma_tables.FirstOrDefault();
        if (embed != null)
            code_map = embed.nhungma_googlemaps ?? string.Empty;
    }

    private void LoadStorefrontConfig(gianhang_storefront_config_table storefrontConfig)
    {
        FooterDescription = GianHangStorefrontConfig_cl.ResolveText(
            storefrontConfig.footer_description,
            GianHangStorefrontConfig_cl.ResolveText(slogan, "Website gian hang duoc dong bo truc tiep tu /gianhang/admin de ban hang, dat lich va cham soc khach hang tren cung mot nen tang.")
        );
        FooterChip1 = (storefrontConfig.footer_chip_1 ?? string.Empty).Trim();
        FooterChip2 = (storefrontConfig.footer_chip_2 ?? string.Empty).Trim();
        FooterChip3 = (storefrontConfig.footer_chip_3 ?? string.Empty).Trim();
        FooterChip4 = (storefrontConfig.footer_chip_4 ?? string.Empty).Trim();
        FooterNavTitle = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.footer_nav_title, FooterNavTitle);
        FooterCategoryTitle = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.footer_category_title, FooterCategoryTitle);
        FooterContactTitle = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.footer_contact_title, FooterContactTitle);
        NavHomeText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.nav_home_text, NavHomeText);
        NavBookingText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.nav_booking_text, NavBookingText);
        QuickServiceText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_service_text, QuickServiceText);
        QuickProductText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_product_text, QuickProductText);
        QuickArticleText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.quick_article_text, QuickArticleText);
        FooterBottomPrimaryText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.footer_bottom_primary_text, FooterBottomPrimaryText);
        FooterBottomPrimaryUrl = ResolveStorefrontUrl(storefrontConfig.footer_bottom_primary_url, FooterBottomPrimaryUrl);
        FooterBottomPrimaryAttr = FooterBottomPrimaryUrl.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ? "onclick=\"show_hide_id_form_dangkytuvan()\"" : string.Empty;
        FooterBottomSecondaryText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.footer_bottom_secondary_text, FooterBottomSecondaryText);
        FooterBottomSecondaryUrl = ResolveStorefrontUrl(storefrontConfig.footer_bottom_secondary_url, FooterBottomSecondaryUrl);
    }

    private string ResolveStorefrontUrl(string rawUrl, string fallback)
    {
        string value = (rawUrl ?? string.Empty).Trim();
        if (value == string.Empty)
            return fallback;
        return GianHangStorefront_cl.NormalizeStandaloneUrl(value);
    }

    public string ResolveMenuUrl(object idObject, object phanloaiObject, object urlOtherObject)
    {
        return GianHangStorefront_cl.ResolveMenuUrl(idObject, phanloaiObject, urlOtherObject);
    }
}
