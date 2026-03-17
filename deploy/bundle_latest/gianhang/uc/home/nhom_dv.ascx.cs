using System;
using System.Linq;
using System.Web.UI;

public partial class uc_home_nhom_dv : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string SectionLabel = "Danh muc dich vu";
    public string SectionTitle = "Nhom dich vu duoc to chuc nhu mot catalog ro rang.";
    public string SectionDescription = string.Empty;
    public string SectionBadgeText = "Di thang vao nhu cau chinh";
    public string ItemLabel = "Nhom cham soc";
    public string PrimaryButtonText = "Xem danh muc";
    public string SecondaryButtonText = "Dat lich";

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
        string sourceValue = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(section);
        int itemLimit = GianHangStorefrontConfig_cl.ResolveItemLimit(section, 6);

        var query = db.web_menu_tables
            .Where(p => p.id_parent == sourceValue && p.bin == false && p.id_chinhanh == chiNhanhId)
            .OrderBy(p => p.rank)
            .ThenBy(p => p.id)
            .AsQueryable();

        if (itemLimit > 0)
            query = query.Take(itemLimit);

        var items = query.ToList();
        if (items.Count == 0)
        {
            Visible = false;
            return;
        }

        Repeater1.DataSource = items;
        Repeater1.DataBind();
    }

    private gianhang_storefront_section_table ResolveSection()
    {
        if (SectionConfig == null)
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, AhaShineContext_cl.ResolveChiNhanhId(), GianHangStorefrontConfig_cl.SectionServiceGroups);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == AhaShineContext_cl.ResolveChiNhanhId());
        string fallback = "Cac nhom dich vu duoc sap xep gon theo nhu cau de khach chon nhanh va chuyen sang dat lich.";
        if (menu == null)
            return fallback;
        return GianHangStorefrontConfig_cl.ResolveText(menu.description, fallback);
    }

    public string ResolveGroupUrl(object idObject, object urlOtherObject)
    {
        return GianHangStorefront_cl.ResolveMenuUrl(idObject, GianHangStorefront_cl.MenuTypeService, urlOtherObject);
    }
}
