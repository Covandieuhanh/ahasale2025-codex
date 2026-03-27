using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class uc_home_sp_noibat : System.Web.UI.UserControl
{
    private readonly dbDataContext db = new dbDataContext();
    private string currentChiNhanhId = "1";

    private sealed class FeaturedProductView
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image_raw { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public decimal? giaban { get; set; }
    }

    public gianhang_storefront_section_table SectionConfig { get; set; }
    public string SectionLabel = "Sản phẩm chủ lực";
    public string SectionTitle = "Sản phẩm được trình bày như một danh mục nổi bật để chốt đơn nhanh.";
    public string SectionDescription = string.Empty;
    public string SectionBadgeText = "Danh mục bán nhanh";
    public string ItemLabel = "Sản phẩm nổi bật";
    public string PrimaryButtonText = "Đặt hàng";
    public string SecondaryButtonText = "Thêm vào giỏ";

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
            .Where(p => p.loai == GianHangProduct_cl.LoaiSanPham);
        if (!string.IsNullOrWhiteSpace(sourceMenuId))
            query = query.Where(p => (p.id_danhmuc ?? string.Empty).Trim() == sourceMenuId);

        var featured = query
            .OrderByDescending(p => p.so_luong_da_ban ?? 0)
            .ThenByDescending(p => p.luot_truy_cap ?? 0)
            .ThenByDescending(p => p.ngay_tao)
            .Select(p => new FeaturedProductView
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
                .Where(p => p.loai == GianHangProduct_cl.LoaiSanPham)
                .OrderByDescending(p => p.so_luong_da_ban ?? 0)
                .ThenByDescending(p => p.luot_truy_cap ?? 0)
                .ThenByDescending(p => p.ngay_tao)
                .Select(p => new FeaturedProductView
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
            SectionConfig = GianHangStorefrontConfig_cl.GetSection(db, currentChiNhanhId, GianHangStorefrontConfig_cl.SectionFeaturedProducts);
        return SectionConfig;
    }

    private string ResolveMenuDescription()
    {
        string menuId = GianHangStorefrontConfig_cl.ResolveSectionSourceValue(ResolveSection());
        var menu = db.web_menu_tables.FirstOrDefault(p => p.id.ToString() == menuId && p.bin == false && p.id_chinhanh == currentChiNhanhId);
        string fallback = "Mỗi thẻ tập trung vào 3 yếu tố chính: hình ảnh, giá bán và hành động thêm giỏ hoặc đặt hàng.";
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

        GH_SanPham_tb product = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
        if (product != null)
            return "/gianhang/xem-san-pham.aspx?id=" + product.id.ToString();

        return "/gianhang/page/chi-tiet-san-pham.aspx?idbv=" + HttpUtility.UrlEncode(normalizedId);
    }

    protected string FormatPrice(object rawPrice)
    {
        decimal price;
        if (!decimal.TryParse(Convert.ToString(rawPrice), out price))
            price = 0m;
        return price.ToString("#,##0");
    }

    protected string BuildCartUrl(object rawId)
    {
        return BuildProductActionUrl(rawId, false);
    }

    protected string BuildCheckoutUrl(object rawId)
    {
        return BuildProductActionUrl(rawId, true);
    }

    private string BuildProductActionUrl(object rawId, bool checkoutNow)
    {
        string id = Convert.ToString(rawId);
        string normalizedId = (id ?? string.Empty).Trim();
        int parsedId = 0;
        int.TryParse(normalizedId, out parsedId);

        GH_SanPham_tb product = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
        int routeId = product != null ? product.id : parsedId;
        string gianHangTaiKhoan = product == null ? string.Empty : ((product.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant());
        string returnUrl = Request.RawUrl ?? GianHangRoutes_cl.BuildStorefrontUrl(gianHangTaiKhoan);

        string url = GianHangRoutes_cl.BuildCartUrl(gianHangTaiKhoan, returnUrl);
        url += (url.IndexOf('?') >= 0 ? "&" : "?") + "id=" + routeId.ToString() + "&qty=1";
        if (checkoutNow)
            url += "&focus=checkout";
        return url;
    }
}
