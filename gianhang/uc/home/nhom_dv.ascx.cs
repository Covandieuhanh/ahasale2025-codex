using System;
using System.Linq;
using System.Web.UI;

public partial class uc_home_nhom_dv : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();
    private string currentChiNhanhId = "1";

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string BookingUrl = GianHangRoutes_cl.BuildBookingHubUrl(string.Empty, string.Empty);
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
            LoadStorefrontContext();
            LoadSectionCopy();
            BindData();
        }
    }

    private void LoadStorefrontContext()
    {
        GianHangPublic_cl.StorefrontContextInfo context = GianHangPublic_cl.ResolveContext(db, Request);
        currentChiNhanhId = (context.ChiNhanhId ?? string.Empty).Trim();
        if (currentChiNhanhId == string.Empty)
            currentChiNhanhId = GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request);
        BookingUrl = string.IsNullOrWhiteSpace(context.BookingUrl)
            ? GianHangRoutes_cl.BuildBookingHubUrl(context.AccountKey, Request.RawUrl)
            : context.BookingUrl;
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
        string chiNhanhId = currentChiNhanhId;
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
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, currentChiNhanhId, GianHangStorefrontConfig_cl.SectionServiceGroups);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == currentChiNhanhId);
        string fallback = "Cac nhom dich vu duoc sap xep gon theo nhu cau de khach chon nhanh va chuyen sang dat lich.";
        if (menu == null)
            return fallback;
        return GianHangStorefrontConfig_cl.ResolveText(menu.description, fallback);
    }

    public string ResolveGroupUrl(object idObject, object urlOtherObject)
    {
        return GianHangPublic_cl.BuildContextMenuUrl(db, Request, idObject, GianHangStorefront_cl.MenuTypeService, urlOtherObject);
    }
}
