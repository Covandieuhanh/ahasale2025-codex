using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangStorefrontConfig_cl
{
    public const string ModeAuto = "auto";
    public const string ModeHybrid = "hybrid";
    public const string ModeRetail = "retail";
    public const string ModeService = "service";

    public const string SectionServiceGroups = "service-groups";
    public const string SectionFeaturedServices = "featured-services";
    public const string SectionFeaturedProducts = "featured-products";
    public const string SectionFeaturedArticles = "featured-articles";

    public static readonly string[] DefaultSectionOrder =
    {
        SectionServiceGroups,
        SectionFeaturedServices,
        SectionFeaturedProducts,
        SectionFeaturedArticles
    };

    public static gianhang_storefront_config_table GetConfig(dbDataContext db)
    {
        return GetConfig(db, AhaShineContext_cl.ResolveChiNhanhId());
    }

    public static gianhang_storefront_config_table GetConfig(dbDataContext db, string chiNhanhId)
    {
        EnsureDefaults(db, chiNhanhId);
        return db.gianhang_storefront_config_tables.First(p => p.id_chinhanh == NormalizeChiNhanhId(chiNhanhId));
    }

    public static gianhang_storefront_section_table GetSection(dbDataContext db, string chiNhanhId, string sectionKey)
    {
        EnsureDefaults(db, chiNhanhId);
        string normalizedChiNhanhId = NormalizeChiNhanhId(chiNhanhId);
        string normalizedKey = (sectionKey ?? string.Empty).Trim().ToLowerInvariant();
        return db.gianhang_storefront_section_tables.FirstOrDefault(p => p.id_chinhanh == normalizedChiNhanhId && (p.section_key ?? string.Empty).Trim().ToLower() == normalizedKey);
    }

    public static List<gianhang_storefront_section_table> GetVisibleSections(dbDataContext db, string chiNhanhId)
    {
        EnsureDefaults(db, chiNhanhId);
        string normalizedChiNhanhId = NormalizeChiNhanhId(chiNhanhId);
        return db.gianhang_storefront_section_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId && (p.is_visible ?? false))
            .OrderBy(p => p.rank ?? 9999)
            .ThenBy(p => p.id)
            .ToList();
    }

    public static List<gianhang_storefront_section_table> GetAllSections(dbDataContext db, string chiNhanhId)
    {
        EnsureDefaults(db, chiNhanhId);
        string normalizedChiNhanhId = NormalizeChiNhanhId(chiNhanhId);
        return db.gianhang_storefront_section_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId)
            .OrderBy(p => p.rank ?? 9999)
            .ThenBy(p => p.id)
            .ToList();
    }

    public static void EnsureDefaults(dbDataContext db, string chiNhanhId)
    {
        if (db == null)
            return;

        GianHangStorefrontSchema_cl.EnsureSafe(db);

        string normalizedChiNhanhId = NormalizeChiNhanhId(chiNhanhId);
        var config = db.gianhang_storefront_config_tables.FirstOrDefault(p => p.id_chinhanh == normalizedChiNhanhId);
        if (config == null)
        {
            config = new gianhang_storefront_config_table
            {
                id_chinhanh = normalizedChiNhanhId,
                storefront_mode = ModeAuto,
                brand_note = "Storefront /shop duoc quan tri tu /gianhang/admin.",
                nav_home_text = "Trang chu",
                nav_booking_text = "Trang cong khai",
                quickstrip_visible = true,
                quick_service_text = "Dich vu",
                quick_product_text = "San pham",
                quick_article_text = "Bai viet",
                quick_consult_text = "Tu van nhanh",
                quick_booking_text = "Trang cong khai",
                hero_eyebrow = "Storefront gian hang doi tac",
                hero_title = "",
                hero_description = "",
                hero_primary_text = "Xem trang cong khai",
                hero_primary_url = "/shop/public.aspx",
                hero_secondary_text = "Quan ly tin dang",
                hero_secondary_url = "/shop/quan-ly-tin",
                hero_tertiary_text = "Theo doi don ban",
                hero_tertiary_url = "/shop/don-ban",
                hero_highlight_title = "Top san pham",
                hero_highlight_description = "Uu tien theo ban chay va luot xem de nhin ra mat hang dang tao doanh thu.",
                hero_highlight_secondary_title = "Danh sach san pham cong khai",
                hero_highlight_secondary_description = "Tat ca san pham va dich vu dang hien thi ra storefront /shop.",
                hero_metric_service_text = "Dich vu dang hien thi",
                hero_metric_product_text = "San pham co the ban truc tiep",
                hero_metric_article_text = "Noi dung ho tro chuyen doi",
                footer_description = "",
                footer_chip_1 = "Dong bo noi dung",
                footer_chip_2 = "Dat lich",
                footer_chip_3 = "Ban hang",
                footer_chip_4 = "Cham soc khach",
                footer_nav_title = "Dieu huong nhanh",
                footer_category_title = "Danh muc tu admin",
                footer_contact_title = "Lien he",
                footer_bottom_primary_text = "Dang ky tu van",
                footer_bottom_primary_url = "javascript:void(0)",
                footer_bottom_secondary_text = "Dat lich ngay",
                footer_bottom_secondary_url = AhaShineHomeRoutes_cl.BookingUrl,
                updated_at = DateTime.Now
            };
            db.gianhang_storefront_config_tables.InsertOnSubmit(config);
            db.SubmitChanges();
        }

        EnsureSection(db, normalizedChiNhanhId, SectionServiceGroups, "Danh muc dich vu", "Nhom dich vu duoc to chuc ro rang tren storefront /shop.", "Cac nhom dich vu duoc sap xep de khi can thi co the dua len trang cong khai ma khong can sua code.", "Danh muc", "Nhom dich vu", "Xem trang cong khai", "/shop/public.aspx", "Quan ly dich vu", "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv", SectionServiceGroups, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeService), 10, true, 6);
        EnsureSection(db, normalizedChiNhanhId, SectionFeaturedServices, "Dich vu can day manh", "Dich vu chu luc se hien tren /shop cung cach card voi san pham.", "Khi dang dich vu trong /gianhang/admin, noi dung duoc dong bo sang feed cong khai de /shop va /home cung doc.", "Feed cong khai", "Dich vu chu luc", "Quan ly dich vu", "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv", "Xem storefront", "/shop/public.aspx", SectionFeaturedServices, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeService), 20, true, 8);
        EnsureSection(db, normalizedChiNhanhId, SectionFeaturedProducts, "San pham chu luc", "San pham va dich vu hien thi tren /shop tu du lieu quan tri trong /gianhang/admin.", "Khoi nay dai dien cho feed cong khai goc. /shop, /home va ve sau /gianhang se cung doc tu mot nguon hien thi.", "Nguon hien thi goc", "San pham cong khai", "Mo /shop", "/shop/public.aspx", "Quan ly san pham", "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp", SectionFeaturedProducts, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeProduct), 30, true, 8);
        EnsureSection(db, normalizedChiNhanhId, SectionFeaturedArticles, "Noi dung ho tro chuyen doi", "Bai viet tu van ho tro SEO, niem tin va chuyen doi tren trang cong khai /shop.", "Neu can dang bai viet de lam noi dung dan dat, van quan tri trong /gianhang/admin va hien tren feed cong khai.", "Noi dung", "Bai viet", "Quan ly bai viet", "/gianhang/admin/quan-ly-bai-viet/Default.aspx", "Xem storefront", "/shop/public.aspx", SectionFeaturedArticles, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeArticle), 40, true, 6);
    }

    private static void EnsureSection(
        dbDataContext db,
        string chiNhanhId,
        string sectionKey,
        string subtitle,
        string title,
        string description,
        string badgeText,
        string itemLabel,
        string ctaText,
        string ctaUrl,
        string secondaryCtaText,
        string secondaryCtaUrl,
        string sourceType,
        string sourceValue,
        int rank,
        bool isVisible,
        int itemLimit)
    {
        var section = db.gianhang_storefront_section_tables.FirstOrDefault(p => p.id_chinhanh == chiNhanhId && p.section_key == sectionKey);
        if (section == null)
        {
            section = new gianhang_storefront_section_table
            {
                id_chinhanh = chiNhanhId,
                section_key = sectionKey,
                subtitle = subtitle,
                title = title,
                description = description,
                badge_text = badgeText,
                item_label = itemLabel,
                cta_text = ctaText,
                cta_url = ctaUrl,
                secondary_cta_text = secondaryCtaText,
                secondary_cta_url = secondaryCtaUrl,
                source_type = sourceType,
                source_value = sourceValue,
                rank = rank,
                is_visible = isVisible,
                item_limit = itemLimit,
                updated_at = DateTime.Now
            };
            db.gianhang_storefront_section_tables.InsertOnSubmit(section);
            db.SubmitChanges();
            return;
        }

        bool changed = false;
        if (string.IsNullOrWhiteSpace(section.source_type))
        {
            section.source_type = sourceType;
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(section.source_value))
        {
            section.source_value = sourceValue;
            changed = true;
        }
        if (!section.rank.HasValue)
        {
            section.rank = rank;
            changed = true;
        }
        if (!section.is_visible.HasValue)
        {
            section.is_visible = isVisible;
            changed = true;
        }
        if (!section.item_limit.HasValue)
        {
            section.item_limit = itemLimit;
            changed = true;
        }
        if (changed)
        {
            section.updated_at = DateTime.Now;
            db.SubmitChanges();
        }
    }

    public static string NormalizeChiNhanhId(string chiNhanhId)
    {
        string normalized = (chiNhanhId ?? string.Empty).Trim();
        return normalized == string.Empty ? "1" : normalized;
    }

    public static string ResolveText(string value, string fallback)
    {
        string normalized = (value ?? string.Empty).Trim();
        return normalized == string.Empty ? (fallback ?? string.Empty) : normalized;
    }

    public static string ResolveSectionSourceValue(gianhang_storefront_section_table section)
    {
        if (section == null)
            return string.Empty;

        string configured = (section.source_value ?? string.Empty).Trim();
        if (configured != string.Empty)
            return configured;

        switch ((section.source_type ?? string.Empty).Trim().ToLowerInvariant())
        {
            case SectionServiceGroups:
            case SectionFeaturedServices:
                return GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeService);
            case SectionFeaturedProducts:
                return GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeProduct);
            case SectionFeaturedArticles:
                return GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeArticle);
            default:
                return string.Empty;
        }
    }

    public static int ResolveItemLimit(gianhang_storefront_section_table section, int fallback)
    {
        if (section != null && section.item_limit.HasValue && section.item_limit.Value > 0)
            return section.item_limit.Value;
        return fallback;
    }

    public static string ResolveStorefrontMode(string configuredMode, int serviceCount, int productCount)
    {
        string normalized = (configuredMode ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized == ModeHybrid || normalized == ModeRetail || normalized == ModeService)
            return normalized;

        if (serviceCount > 0 && productCount > 0)
            return ModeHybrid;
        if (productCount > 0)
            return ModeRetail;
        return ModeService;
    }

    public static string ResolveModeLabel(string resolvedMode)
    {
        switch ((resolvedMode ?? string.Empty).Trim().ToLowerInvariant())
        {
            case ModeRetail:
                return "Retail-first storefront";
            case ModeService:
                return "Service-first storefront";
            default:
                return "Hybrid retail + service";
        }
    }

    public static string ResolveModeDescription(string resolvedMode)
    {
        switch ((resolvedMode ?? string.Empty).Trim().ToLowerInvariant())
        {
            case ModeRetail:
                return "Trang uu tien catalog, ban hang va gio hang, nhung van giu duong dan dat lich va tu van neu can.";
            case ModeService:
                return "Trang uu tien dieu huong dich vu va lich hen, nhung van san sang de bo sung kenh ban san pham khi admin cap nhat.";
            default:
                return "Trang duoc can bang de vua chot lich, vua chot don, phu hop mo hinh gian hang doi tac van hanh tren AhaSale.";
        }
    }

    public static string ResolveSectionSummary(gianhang_storefront_section_table section)
    {
        if (section == null)
            return string.Empty;

        string summary = ResolveText(section.title, section.section_key);
        string source = ResolveText(section.source_type, "manual");
        return summary + " | " + source;
    }
}
