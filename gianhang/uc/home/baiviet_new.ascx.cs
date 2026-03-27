using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class uc_home_baiviet_new : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();
    private string currentChiNhanhId = "1";

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string SectionLabel = "Nội dung hỗ trợ bán hàng";
    public string SectionTitle = "Bài viết mới giúp tăng niềm tin và hỗ trợ khách ra quyết định.";
    public string SectionDescription = string.Empty;
    public string SectionBadgeText = "SEO, niềm tin và tư vấn";
    public string ItemLabel = "Bài viết nổi bật";
    public string PrimaryButtonText = "Đọc chi tiết";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            currentChiNhanhId = GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request);
            LoadSectionCopy();
            BindData();
        }
    }

    private void LoadSectionCopy()
    {
        var section = ResolveSection();
        SectionLabel = GianHangStorefrontConfig_cl.ResolveText(section.subtitle, SectionLabel);
        SectionTitle = GianHangStorefrontConfig_cl.ResolveText(section.title, SectionTitle);
        SectionDescription = GianHangStorefrontConfig_cl.ResolveText(section.description, ResolveMenuDescription());
        SectionBadgeText = GianHangStorefrontConfig_cl.ResolveText(section.badge_text, SectionBadgeText);
        ItemLabel = GianHangStorefrontConfig_cl.ResolveText(section.item_label, ItemLabel);
        PrimaryButtonText = GianHangStorefrontConfig_cl.ResolveText(section.cta_text, PrimaryButtonText);
    }

    private void BindData()
    {
        var section = ResolveSection();
        string chiNhanhId = currentChiNhanhId;
        int itemLimit = GianHangStorefrontConfig_cl.ResolveItemLimit(section, 6);
        string storeAccountKey = GianHangPublic_cl.ResolveCurrentStoreAccountKey(db, Request);

        var articles = GianHangArticle_cl.QueryPublicByChiNhanh(db, chiNhanhId, storeAccountKey)
            .OrderByDescending(p => p.ngaytao)
            .ToList();

        if (articles.Count == 0)
        {
            Visible = false;
            return;
        }

        if (itemLimit > 0)
            articles = articles.Take(itemLimit).ToList();

        Repeater1.DataSource = articles.Take(1);
        Repeater1.DataBind();
        Repeater2.DataSource = articles.Skip(1);
        Repeater2.DataBind();
    }

    private gianhang_storefront_section_table ResolveSection()
    {
        if (SectionConfig == null)
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, currentChiNhanhId, GianHangStorefrontConfig_cl.SectionFeaturedArticles);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == currentChiNhanhId);
        string fallback = "Nội dung tư vấn được hiển thị cùng gian hàng để khách có đủ thông tin trước khi ra quyết định.";
        if (menu == null)
            return fallback;
        return GianHangStorefrontConfig_cl.ResolveText(menu.description, fallback);
    }

    protected string BuildArticleUrl(object rawId)
    {
        GianHangPublic_cl.StorefrontContextInfo context = GianHangPublic_cl.ResolveContext(db, Request);
        return GianHangArticle_cl.BuildDetailUrl(rawId, context.AccountKey);
    }
}
