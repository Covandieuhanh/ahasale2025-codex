using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class gianhang_Default : System.Web.UI.Page
{
    public string meta = string.Empty;
    public string title_web = "Gian hang";
    public string BrandName = "Gian hang doi tac";
    public string BrandSlogan = string.Empty;
    public string HeroEyebrow = string.Empty;
    public string HeroTitle = string.Empty;
    public string HeroDescription = string.Empty;
    public string HeroPrimaryText = string.Empty;
    public string HeroPrimaryUrl = AhaShineHomeRoutes_cl.ServicesUrl;
    public string HeroSecondaryText = string.Empty;
    public string HeroSecondaryUrl = AhaShineHomeRoutes_cl.ProductsUrl;
    public string HeroTertiaryText = string.Empty;
    public string HeroTertiaryUrl = AhaShineHomeRoutes_cl.BookingUrl;
    public string StagePrimaryTitle = string.Empty;
    public string StagePrimaryDescription = string.Empty;
    public string StageSecondaryTitle = string.Empty;
    public string StageSecondaryDescription = string.Empty;
    public string MetricServiceText = string.Empty;
    public string MetricProductText = string.Empty;
    public string MetricArticleText = string.Empty;
    public int ServiceCount;
    public int ProductCount;
    public int ArticleCount;
    public int MenuCount;

    private readonly dbDataContext db = new dbDataContext();
    private string chiNhanhId = "1";
    private gianhang_storefront_config_table storefrontConfig;
    private string shareDescription = string.Empty;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        AhaShineContext_cl.EnsureContext();
        chiNhanhId = AhaShineContext_cl.ResolveChiNhanhId();
        storefrontConfig = GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);
        BuildHomeSections();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadBrand();
        LoadMeta();
        LoadCounts();
        ResolveHero();
    }

    private void LoadBrand()
    {
        var info = db.config_thongtin_tables.FirstOrDefault();
        if (info == null)
            return;

        BrandName = GianHangStorefrontConfig_cl.ResolveText(info.tencongty, "Gian hang doi tac");
        BrandSlogan = (info.slogan ?? string.Empty).Trim();
    }

    private void LoadMeta()
    {
        var shareConfig = db.config_lienket_chiase_tables.FirstOrDefault();
        if (shareConfig == null)
        {
            title_web = BrandName;
            return;
        }

        string shareTitle = (shareConfig.title ?? string.Empty).Trim();
        shareDescription = (shareConfig.description ?? string.Empty).Trim();
        string shareImage = (shareConfig.image ?? string.Empty).Trim();

        title_web = shareTitle == string.Empty ? BrandName : shareTitle;

        string titleTag = shareTitle == string.Empty ? string.Empty : "<meta property=\"og:title\" content=\"" + shareTitle + "\" />";
        string imageTag = shareImage == string.Empty ? string.Empty : "<meta property=\"og:image\" content=\"" + shareImage + "\" />";
        string descriptionContent = shareDescription == string.Empty ? BrandSlogan : shareDescription;
        string descriptionTag = descriptionContent == string.Empty ? string.Empty : "<meta name=\"description\" content=\"" + descriptionContent + "\" />";
        string ogDescriptionTag = descriptionContent == string.Empty ? string.Empty : "<meta property=\"og:description\" content=\"" + descriptionContent + "\" />";
        meta = titleTag + imageTag + descriptionTag + ogDescriptionTag;
    }

    private void LoadCounts()
    {
        var visiblePosts = db.web_post_tables.Where(p => p.bin == false && p.hienthi == true && p.id_chinhanh == chiNhanhId);
        ServiceCount = visiblePosts.Count(p => p.phanloai == "ctdv");
        ProductCount = visiblePosts.Count(p => p.phanloai == "ctsp");
        ArticleCount = visiblePosts.Count(p => p.phanloai == "ctbv");
        MenuCount = db.web_menu_tables.Count(p => p.bin == false && p.id_chinhanh == chiNhanhId);
    }

    private void ResolveHero()
    {
        string resolvedMode = GianHangStorefrontConfig_cl.ResolveStorefrontMode(storefrontConfig.storefront_mode, ServiceCount, ProductCount);
        string resolvedModeLabel = GianHangStorefrontConfig_cl.ResolveModeLabel(resolvedMode);
        string resolvedModeDescription = GianHangStorefrontConfig_cl.ResolveModeDescription(resolvedMode);

        HeroEyebrow = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_eyebrow, "Website gian hang doi tac");
        HeroTitle = GianHangStorefrontConfig_cl.ResolveText(
            storefrontConfig.hero_title,
            BrandName + " duoc van hanh tren mot storefront can bang giua dich vu, san pham va cham soc khach hang."
        );

        string heroFallbackDescription = GianHangStorefrontConfig_cl.ResolveText(shareDescription, BrandSlogan);
        if (heroFallbackDescription == string.Empty)
        {
            heroFallbackDescription = "Toan bo noi dung, menu, slider, dich vu, san pham va bai viet deu duoc dong bo truc tiep tu /gianhang/admin.";
        }
        HeroDescription = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_description, heroFallbackDescription);

        HeroPrimaryText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_primary_text, "Kham pha dich vu");
        HeroPrimaryUrl = ResolveConfigUrl(storefrontConfig.hero_primary_url, AhaShineHomeRoutes_cl.ServicesUrl);
        HeroSecondaryText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_secondary_text, "Mua san pham");
        HeroSecondaryUrl = ResolveConfigUrl(storefrontConfig.hero_secondary_url, AhaShineHomeRoutes_cl.ProductsUrl);
        HeroTertiaryText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_tertiary_text, "Dat lich ngay");
        HeroTertiaryUrl = ResolveConfigUrl(storefrontConfig.hero_tertiary_url, AhaShineHomeRoutes_cl.BookingUrl);

        StagePrimaryTitle = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_highlight_title, resolvedModeLabel);
        StagePrimaryDescription = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_highlight_description, resolvedModeDescription);
        StageSecondaryTitle = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_highlight_secondary_title, MenuCount.ToString("#,##0") + " diem dieu huong");
        StageSecondaryDescription = GianHangStorefrontConfig_cl.ResolveText(
            storefrontConfig.hero_highlight_secondary_description,
            "Menu, danh muc va noi dung duoc lay truc tiep tu cau hinh gian hang hien tai."
        );

        MetricServiceText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_metric_service_text, "Dich vu dang hien thi");
        MetricProductText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_metric_product_text, "San pham co the ban truc tiep");
        MetricArticleText = GianHangStorefrontConfig_cl.ResolveText(storefrontConfig.hero_metric_article_text, "Noi dung ho tro chuyen doi");
    }

    private string ResolveConfigUrl(string configuredValue, string fallback)
    {
        string value = (configuredValue ?? string.Empty).Trim();
        if (value == string.Empty)
            return fallback;
        return GianHangStorefront_cl.NormalizeStandaloneUrl(value);
    }

    private void BuildHomeSections()
    {
        plHomeSections.Controls.Clear();
        foreach (var section in GianHangStorefrontConfig_cl.GetVisibleSections(db, chiNhanhId))
        {
            Control control = CreateSectionControl(section);
            if (control == null)
                continue;

            Panel wrapper = new Panel();
            wrapper.CssClass = "home-fade-up";
            wrapper.Controls.Add(control);
            plHomeSections.Controls.Add(wrapper);
        }
    }

    private Control CreateSectionControl(gianhang_storefront_section_table section)
    {
        string sourceType = (section.source_type ?? section.section_key ?? string.Empty).Trim().ToLowerInvariant();
        string controlPath = string.Empty;
        switch (sourceType)
        {
            case GianHangStorefrontConfig_cl.SectionServiceGroups:
                controlPath = "~/gianhang/uc/home/nhom_dv.ascx";
                break;
            case GianHangStorefrontConfig_cl.SectionFeaturedServices:
                controlPath = "~/gianhang/uc/home/dv_noibat.ascx";
                break;
            case GianHangStorefrontConfig_cl.SectionFeaturedProducts:
                controlPath = "~/gianhang/uc/home/sp_noibat.ascx";
                break;
            case GianHangStorefrontConfig_cl.SectionFeaturedArticles:
                controlPath = "~/gianhang/uc/home/baiviet_new.ascx";
                break;
            default:
                return null;
        }

        Control control = LoadControl(controlPath);
        control.ID = "ucSection_" + section.id;
        var property = control.GetType().GetProperty("SectionConfig");
        if (property != null && property.CanWrite)
            property.SetValue(control, section, null);
        return control;
    }
}
