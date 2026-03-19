using System;
using System.Linq;
using System.Web.UI;

public partial class uc_home_sp_noibat : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string SectionLabel = "San pham chu luc";
    public string SectionTitle = "San pham duoc trinh bay nhu mot catalog co the chot don ngay.";
    public string SectionDescription = string.Empty;
    public string SectionBadgeText = "Catalog ban nhanh";
    public string ItemLabel = "San pham noi bat";
    public string PrimaryButtonText = "Dat hang";
    public string SecondaryButtonText = "Them vao gio";

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
        SecondaryButtonText = GianHangStorefrontConfig_cl.ResolveText(section.secondary_cta_text, SecondaryButtonText);
    }

    private void BindData()
    {
        var section = ResolveSection();
        string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhId();
        int itemLimit = GianHangStorefrontConfig_cl.ResolveItemLimit(section, 8);

        var featured = db.web_post_tables
            .Where(p => p.phanloai == "ctsp" && p.bin == false && p.noibat == true && p.hienthi == true && p.id_chinhanh == chiNhanhId)
            .OrderBy(p => p.name)
            .ToList();

        if (featured.Count == 0)
        {
            featured = db.web_post_tables
                .Where(p => p.phanloai == "ctsp" && p.bin == false && p.hienthi == true && p.id_chinhanh == chiNhanhId)
                .OrderBy(p => p.name)
                .ToList();
        }

        if (itemLimit > 0)
            featured = featured.Take(itemLimit).ToList();

        if (featured.Count == 0)
        {
            Visible = false;
            return;
        }

        Repeater1.DataSource = featured;
        Repeater1.DataBind();
    }

    private gianhang_storefront_section_table ResolveSection()
    {
        if (SectionConfig == null)
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, AhaShineContext_cl.ResolveChiNhanhId(), GianHangStorefrontConfig_cl.SectionFeaturedProducts);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == AhaShineContext_cl.ResolveChiNhanhId());
        string fallback = "Moi the card tap trung vao dung 3 thu: hinh anh, gia ban va hanh dong them gio hoac dat hang.";
        if (menu == null)
            return fallback;
        return GianHangStorefrontConfig_cl.ResolveText(menu.description, fallback);
    }
}
