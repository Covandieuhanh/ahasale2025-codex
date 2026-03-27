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
    public string HomeUrl = GianHangRoutes_cl.BuildStorefrontUrl(string.Empty);
    public string BookingUrl = GianHangRoutes_cl.BuildBookingHubUrl(string.Empty, string.Empty);
    public string CartUrl = GianHangRoutes_cl.BuildCartUrl(string.Empty, string.Empty);
    public string ServicesUrl = GianHangRoutes_cl.BuildServicesUrl(string.Empty);
    public string ProductsUrl = GianHangRoutes_cl.BuildProductsUrl(string.Empty);
    public string ArticlesUrl = GianHangRoutes_cl.BuildArticlesUrl(string.Empty);
    public string FooterDescription = string.Empty;
    public string FooterChip1 = string.Empty;
    public string FooterChip2 = string.Empty;
    public string FooterChip3 = string.Empty;
    public string FooterChip4 = string.Empty;
    public string FooterNavTitle = "Điều hướng nhanh";
    public string FooterCategoryTitle = "Danh mục từ quản trị";
    public string FooterContactTitle = "Liên hệ";
    public string FooterBottomPrimaryText = "Đăng ký tư vấn";
    public string FooterBottomPrimaryUrl = "javascript:void(0)";
    public string FooterBottomPrimaryAttr = string.Empty;
    public string FooterBottomSecondaryText = "Đặt lịch ngay";
    public string FooterBottomSecondaryUrl = GianHangRoutes_cl.BuildBookingHubUrl(string.Empty, string.Empty);
    public string NavHomeText = "Trang chủ";
    public string NavBookingText = "Đặt lịch";
    public string QuickServiceText = "Dịch vụ";
    public string QuickProductText = "Sản phẩm";
    public string QuickArticleText = "Bài viết";
    private string currentStoreAccount = string.Empty;

    private readonly dbDataContext db = new dbDataContext();
    private string currentChiNhanhId = "1";
    private bool hasTransientDataIssue;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            LoadStorefrontContext();
            var storefrontConfig = TrySql(() => GianHangStorefrontConfig_cl.GetConfig(db, currentChiNhanhId), null);
            LoadInfo();
            LoadSocial();
            LoadEmbed();
            LoadStorefrontConfig(storefrontConfig);

            Repeater1.DataSource = TrySql(
                () => GianHangMenu_cl.LoadAll(db, currentChiNhanhId)
                    .Where(p => p.id_parent == "0" && p.bin == false)
                    .ToList(),
                new System.Collections.Generic.List<web_menu_table>());
            Repeater1.DataBind();
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            hasTransientDataIssue = true;
            ApplyFallbackState();
            Repeater1.DataSource = null;
            Repeater1.DataBind();
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
    }

    private void LoadInfo()
    {
        var info = TrySql(() => db.config_thongtin_tables.FirstOrDefault(), null);
        if (info == null)
            return;

        logo = (info.logo ?? string.Empty).Trim();
        tencongty = GianHangStorefrontConfig_cl.ResolveText(info.tencongty, "Gian hàng đối tác");
        diachi = (info.diachi ?? string.Empty).Trim();
        hotline = (info.hotline ?? string.Empty).Trim();
        email = (info.email ?? string.Empty).Trim();
        slogan = (info.slogan ?? string.Empty).Trim();
    }

    private void LoadSocial()
    {
        var social = TrySql(() => db.config_social_media_tables.FirstOrDefault(), null);
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
        var embed = TrySql(() => db.config_nhungma_tables.FirstOrDefault(), null);
        if (embed != null)
            code_map = embed.nhungma_googlemaps ?? string.Empty;
    }

    private void LoadStorefrontConfig(gianhang_storefront_config_table storefrontConfig)
    {
        FooterDescription = GianHangStorefrontConfig_cl.ResolveText(
            storefrontConfig.footer_description,
            GianHangStorefrontConfig_cl.ResolveText(slogan, "Thông tin gian hàng được đồng bộ để phục vụ bán hàng, đặt lịch và chăm sóc khách hàng trên cùng một nền tảng.")
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
        FooterBottomSecondaryUrl = ResolveStorefrontUrl(storefrontConfig.footer_bottom_secondary_url, BookingUrl);
    }

    private string ResolveStorefrontUrl(string rawUrl, string fallback)
    {
        string value = (rawUrl ?? string.Empty).Trim();
        if (value == string.Empty)
            return fallback;
        return GianHangPublic_cl.AppendUserToUrl(GianHangStorefront_cl.NormalizeStandaloneUrl(value), currentStoreAccount);
    }

    public string ResolveMenuUrl(object idObject, object phanloaiObject, object urlOtherObject)
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
        if (tencongty == string.Empty)
            tencongty = "Gian hàng đối tác";
        if (FooterDescription == string.Empty)
            FooterDescription = "Dữ liệu chân trang đang được đồng bộ. Vui lòng tải lại sau vài giây.";
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

        if (hasTransientDataIssue && slogan == string.Empty)
            slogan = string.Empty;
    }
}
