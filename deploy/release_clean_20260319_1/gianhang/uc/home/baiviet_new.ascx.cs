using System;
using System.Linq;
using System.Web.UI;

public partial class uc_home_baiviet_new : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string SectionLabel = "Noi dung ho tro ban hang";
    public string SectionTitle = "Bai viet moi duoc dung nhu lop noi dung tang niem tin va ho tro chuyen doi.";
    public string SectionDescription = string.Empty;
    public string SectionBadgeText = "SEO, niem tin va tu van";
    public string ItemLabel = "Bai viet dan dat";
    public string PrimaryButtonText = "Doc chi tiet";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
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
        string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhId();
        int itemLimit = GianHangStorefrontConfig_cl.ResolveItemLimit(section, 6);

        var articles = db.web_post_tables
            .Where(p => p.phanloai == "ctbv" && p.bin == false && p.hienthi == true && p.id_chinhanh == chiNhanhId)
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
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, AhaShineContext_cl.ResolveChiNhanhId(), GianHangStorefrontConfig_cl.SectionFeaturedArticles);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == AhaShineContext_cl.ResolveChiNhanhId());
        string fallback = "SEO, niem tin va tu van duoc ghep vao cung storefront de khach du thong tin truoc khi ra quyet dinh.";
        if (menu == null)
            return fallback;
        return GianHangStorefrontConfig_cl.ResolveText(menu.description, fallback);
    }
}
