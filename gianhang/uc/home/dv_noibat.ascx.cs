using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class uc_home_dv_noibat : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();
    private string currentChiNhanhId = "1";

    private sealed class FeaturedServiceView
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image_raw { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public decimal? giaban { get; set; }
    }

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string SectionLabel = "Dịch vụ nên ưu tiên giới thiệu";
    public string SectionTitle = "Dịch vụ chủ lực được làm nổi bật để khách dễ đặt lịch và trao đổi.";
    public string SectionDescription = string.Empty;
    public string SectionBadgeText = "Xem chi tiết hoặc đặt lịch ngay";
    public string ItemLabel = "Dịch vụ chủ lực";
    public string PrimaryButtonText = "Xem chi tiết";
    public string SecondaryButtonText = "Đặt lịch ngay";

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
        SecondaryButtonText = GianHangStorefrontConfig_cl.ResolveText(section.secondary_cta_text, SecondaryButtonText);
    }

    private void BindData()
    {
        var section = ResolveSection();
        int itemLimit = GianHangStorefrontConfig_cl.ResolveItemLimit(section, 8);
        string storeAccountKey = GianHangPublic_cl.ResolveCurrentStoreAccountKey(db, Request);
        if (string.IsNullOrWhiteSpace(storeAccountKey))
        {
            Visible = false;
            return;
        }

        string sourceMenuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(section);
        IQueryable<GH_SanPham_tb> query = GianHangProduct_cl.QueryPublicByStorefront(db, storeAccountKey)
            .Where(p => p.loai == GianHangProduct_cl.LoaiDichVu);
        if (!string.IsNullOrWhiteSpace(sourceMenuId))
            query = query.Where(p => (p.id_danhmuc ?? string.Empty).Trim() == sourceMenuId);

        var featured = query
            .OrderByDescending(p => p.so_luong_da_ban ?? 0)
            .ThenByDescending(p => p.luot_truy_cap ?? 0)
            .ThenByDescending(p => p.ngay_tao)
            .Select(p => new FeaturedServiceView
            {
                id = p.id,
                name = p.ten ?? string.Empty,
                image_raw = p.hinh_anh,
                description = p.mo_ta ?? string.Empty,
                giaban = p.gia_ban
            })
            .ToList();

        if (featured.Count == 0 && !string.IsNullOrWhiteSpace(sourceMenuId))
        {
            featured = GianHangProduct_cl.QueryPublicByStorefront(db, storeAccountKey)
                .Where(p => p.loai == GianHangProduct_cl.LoaiDichVu)
                .OrderByDescending(p => p.so_luong_da_ban ?? 0)
                .ThenByDescending(p => p.luot_truy_cap ?? 0)
                .ThenByDescending(p => p.ngay_tao)
                .Select(p => new FeaturedServiceView
                {
                    id = p.id,
                    name = p.ten ?? string.Empty,
                    image_raw = p.hinh_anh,
                    description = p.mo_ta ?? string.Empty,
                    giaban = p.gia_ban
                })
                .ToList();
        }

        for (int i = 0; i < featured.Count; i++)
            featured[i].image = GianHangStorefront_cl.ResolveImageUrl(featured[i].image_raw);

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
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, currentChiNhanhId, GianHangStorefrontConfig_cl.SectionFeaturedServices);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == currentChiNhanhId);
        string fallback = "Khu vực này đóng vai trò như nhóm dịch vụ nổi bật: nhìn nhanh, hiểu nhanh và chuyển đổi ngay.";
        if (menu == null)
            return fallback;
        return GianHangStorefrontConfig_cl.ResolveText(menu.description, fallback);
    }

    protected string BuildDetailUrl(object rawId)
    {
        string id = Convert.ToString(rawId);
        string normalizedId = (id ?? string.Empty).Trim();
        int parsedId = 0;
        int.TryParse(normalizedId, out parsedId);

        GH_SanPham_tb service = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
        if (service != null)
            return "/gianhang/xem-dich-vu.aspx?id=" + service.id.ToString();

        return "/gianhang/page/chi-tiet-dich-vu.aspx?idbv=" + HttpUtility.UrlEncode(normalizedId);
    }

    protected string BuildBookingUrl(object rawId)
    {
        string id = Convert.ToString(rawId);
        string normalizedId = (id ?? string.Empty).Trim();
        int parsedId = 0;
        int.TryParse(normalizedId, out parsedId);

        GH_SanPham_tb service = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
        int routeId = service != null ? service.id : parsedId;
        string gianHangTaiKhoan = service == null ? string.Empty : ((service.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant());
        string returnUrl = Request.RawUrl ?? GianHangRoutes_cl.BuildStorefrontUrl(gianHangTaiKhoan);

        string url = "/gianhang/datlich.aspx?id=" + routeId.ToString();
        url = GianHangRoutes_cl.AppendUserToUrl(url, gianHangTaiKhoan);
        url = GianHangRoutes_cl.AppendReturnUrl(url, returnUrl);
        return url;
    }

    protected string FormatPrice(object rawPrice)
    {
        decimal price;
        if (!decimal.TryParse(Convert.ToString(rawPrice), out price))
            price = 0m;
        return price.ToString("#,##0");
    }
}
