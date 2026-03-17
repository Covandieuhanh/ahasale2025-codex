using System;
using System.Linq;
using System.Web.UI;

public partial class uc_home_dv_noibat : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string SectionLabel = "Dich vu nen uu tien chot";
    public string SectionTitle = "Dich vu chu luc duoc day len nhu cac san pham ban chay.";
    public string SectionDescription = string.Empty;
    public string SectionBadgeText = "CTA kep: xem chi tiet hoac dat lich";
    public string ItemLabel = "Dich vu chu luc";
    public string PrimaryButtonText = "Xem chi tiet";
    public string SecondaryButtonText = "Dat lich ngay";

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
            .Where(p => p.phanloai == "ctdv" && p.bin == false && p.noibat == true && p.hienthi == true && p.id_chinhanh == chiNhanhId)
            .OrderBy(p => p.name)
            .ToList();

        if (featured.Count == 0)
        {
            featured = db.web_post_tables
                .Where(p => p.phanloai == "ctdv" && p.bin == false && p.hienthi == true && p.id_chinhanh == chiNhanhId)
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
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, AhaShineContext_cl.ResolveChiNhanhId(), GianHangStorefrontConfig_cl.SectionFeaturedServices);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == AhaShineContext_cl.ResolveChiNhanhId());
        string fallback = "Muc nay dong vai tro nhu khu vuc best-seller cho dich vu: nhin nhanh, hieu nhanh va chuyen doi ngay.";
        if (menu == null)
            return fallback;
        return GianHangStorefrontConfig_cl.ResolveText(menu.description, fallback);
    }
}
