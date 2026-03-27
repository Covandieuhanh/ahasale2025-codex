using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
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
        gianhang_storefront_section_table section = db.gianhang_storefront_section_tables
            .FirstOrDefault(p => p.id_chinhanh == normalizedChiNhanhId && p.section_key == normalizedKey);
        if (section != null)
            return section;

        return db.gianhang_storefront_section_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId)
            .ToList()
            .FirstOrDefault(p => string.Equals((p.section_key ?? string.Empty).Trim(), normalizedKey, StringComparison.OrdinalIgnoreCase));
    }

    public static List<gianhang_storefront_section_table> GetVisibleSections(dbDataContext db, string chiNhanhId)
    {
        EnsureDefaults(db, chiNhanhId);
        string normalizedChiNhanhId = NormalizeChiNhanhId(chiNhanhId);
        return db.gianhang_storefront_section_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId && p.is_visible == true)
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

    public static bool ResolveBool(object rawValue, bool defaultValue)
    {
        if (rawValue == null || rawValue == DBNull.Value)
            return defaultValue;

        bool boolValue;
        if (bool.TryParse(rawValue.ToString(), out boolValue))
            return boolValue;

        int intValue;
        if (int.TryParse(rawValue.ToString(), out intValue))
            return intValue != 0;

        return defaultValue;
    }

    public static void EnsureDefaults(dbDataContext db, string chiNhanhId)
    {
        if (db == null)
            return;

        GianHangStorefrontSchema_cl.EnsureSafe(db);
        string normalizedChiNhanhId = NormalizeChiNhanhId(chiNhanhId);

        using (dbDataContext ensureDb = new dbDataContext())
        {
            GianHangStorefrontSchema_cl.EnsureSafe(ensureDb);
            EnsureDefaultsCore(ensureDb, normalizedChiNhanhId);
        }
    }

    private static void EnsureDefaultsCore(dbDataContext db, string normalizedChiNhanhId)
    {
        var config = db.gianhang_storefront_config_tables.FirstOrDefault(p => p.id_chinhanh == normalizedChiNhanhId);
        if (config == null)
        {
            config = new gianhang_storefront_config_table
            {
                id_chinhanh = normalizedChiNhanhId,
                storefront_mode = ModeAuto,
                brand_note = "Gian hàng được quản trị từ khu vận hành riêng.",
                nav_home_text = "Trang chủ",
                nav_booking_text = "Trang công khai",
                quickstrip_visible = true,
                quick_service_text = "Dịch vụ",
                quick_product_text = "Sản phẩm",
                quick_article_text = "Bài viết",
                quick_consult_text = "Tư vấn nhanh",
                quick_booking_text = "Trang công khai",
                hero_eyebrow = "Không gian gian hàng",
                hero_title = "",
                hero_description = "",
                hero_primary_text = "Xem trang công khai",
                hero_primary_url = "/gianhang/public.aspx",
                hero_secondary_text = "Quản lý tin đăng",
                hero_secondary_url = "/gianhang/quan-ly-tin/Default.aspx",
                hero_tertiary_text = "Theo dõi đơn bán",
                hero_tertiary_url = "/gianhang/don-ban.aspx",
                hero_highlight_title = "Top sản phẩm",
                hero_highlight_description = "Ưu tiên theo bán chạy và lượt xem để nhận ra mặt hàng đang tạo doanh thu.",
                hero_highlight_secondary_title = "Danh sách tin công khai",
                hero_highlight_secondary_description = "Tất cả sản phẩm và dịch vụ đang hiển thị trên trang công khai.",
                hero_metric_service_text = "Dịch vụ đang hiển thị",
                hero_metric_product_text = "Sản phẩm có thể bán trực tiếp",
                hero_metric_article_text = "Nội dung hỗ trợ chuyển đổi",
                footer_description = "",
                footer_chip_1 = "Đồng bộ nội dung",
                footer_chip_2 = "Đặt lịch",
                footer_chip_3 = "Bán hàng",
                footer_chip_4 = "Chăm sóc khách",
                footer_nav_title = "Điều hướng nhanh",
                footer_category_title = "Danh mục từ quản trị",
                footer_contact_title = "Liên hệ",
                footer_bottom_primary_text = "Đăng ký tư vấn",
                footer_bottom_primary_url = "javascript:void(0)",
                footer_bottom_secondary_text = "Đặt lịch ngay",
                footer_bottom_secondary_url = GianHangRoutes_cl.BuildServicesUrl(string.Empty),
                updated_at = DateTime.Now
            };
            db.gianhang_storefront_config_tables.InsertOnSubmit(config);
            SubmitChangesSafe(db);
        }

        bool configChanged = NormalizeLegacyConfig(config);
        if (configChanged)
        {
            config.updated_at = DateTime.Now;
            SubmitChangesSafe(db);
        }

        EnsureSection(db, normalizedChiNhanhId, SectionServiceGroups, "Danh mục dịch vụ", "Nhóm dịch vụ được tổ chức rõ ràng trên trang công khai.", "Các nhóm dịch vụ được sắp xếp sẵn để đưa lên trang công khai khi cần.", "Danh mục", "Nhóm dịch vụ", "Xem trang công khai", "/gianhang/public.aspx", "Quản lý dịch vụ", "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv", SectionServiceGroups, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeService), 10, true, 6);
        EnsureSection(db, normalizedChiNhanhId, SectionFeaturedServices, "Dịch vụ cần đẩy mạnh", "Dịch vụ chủ lực sẽ hiển thị nổi bật để khách dễ xem và đặt lịch.", "Khi đăng dịch vụ trong khu quản trị, nội dung sẽ được đồng bộ sang trang công khai.", "Nguồn hiển thị", "Dịch vụ chủ lực", "Quản lý dịch vụ", "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv", "Xem trang công khai", "/gianhang/public.aspx", SectionFeaturedServices, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeService), 20, true, 8);
        EnsureSection(db, normalizedChiNhanhId, SectionFeaturedProducts, "Sản phẩm chủ lực", "Sản phẩm và dịch vụ hiển thị được lấy từ dữ liệu quản trị của gian hàng.", "Khối này đại diện cho nguồn hiển thị công khai dùng chung của gian hàng.", "Nguồn hiển thị", "Tin công khai", "Mở trang công khai", "/gianhang/public.aspx", "Quản lý sản phẩm", "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp", SectionFeaturedProducts, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeProduct), 30, true, 8);
        EnsureSection(db, normalizedChiNhanhId, SectionFeaturedArticles, "Nội dung hỗ trợ chuyển đổi", "Bài viết tư vấn hỗ trợ SEO, tăng niềm tin và chuyển đổi trên trang công khai.", "Bài viết được quản trị tại khu vận hành và hiển thị trên trang công khai.", "Nội dung", "Bài viết", "Quản lý bài viết", "/gianhang/admin/quan-ly-bai-viet/Default.aspx", "Xem trang công khai", "/gianhang/public.aspx", SectionFeaturedArticles, GianHangStorefront_cl.ResolveDefaultMenuId(GianHangStorefront_cl.MenuTypeArticle), 40, true, 6);
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
            SubmitChangesSafe(db);
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
        changed = NormalizeLegacySection(section) || changed;
        if (changed)
        {
            section.updated_at = DateTime.Now;
            SubmitChangesSafe(db);
        }
    }

    private static void SubmitChangesSafe(dbDataContext db)
    {
        try
        {
            db.SubmitChanges();
        }
        catch (ChangeConflictException)
        {
            try
            {
                foreach (ObjectChangeConflict conflict in db.ChangeConflicts)
                    conflict.Resolve(RefreshMode.OverwriteCurrentValues);
            }
            catch
            {
            }
        }
        catch (SqlException ex)
        {
            if (!IsIgnorableBootstrapSql(ex))
                throw;
        }
    }

    private static bool IsIgnorableBootstrapSql(SqlException ex)
    {
        if (ex == null)
            return false;

        foreach (SqlError error in ex.Errors)
        {
            if (error == null)
                continue;

            if (error.Number == 2601 || error.Number == 2627)
                return true;
        }

        return false;
    }

    public static string NormalizeChiNhanhId(string chiNhanhId)
    {
        string normalized = (chiNhanhId ?? string.Empty).Trim();
        return normalized == string.Empty ? "1" : normalized;
    }

    public static string ResolveText(string value, string fallback)
    {
        string normalized = NormalizeLegacyCopy((value ?? string.Empty).Trim());
        if (normalized != string.Empty)
            return normalized;
        return NormalizeLegacyCopy((fallback ?? string.Empty).Trim());
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
                return "Gian hàng thiên sản phẩm";
            case ModeService:
                return "Gian hàng thiên dịch vụ";
            default:
                return "Gian hàng kết hợp sản phẩm và dịch vụ";
        }
    }

    public static string ResolveModeDescription(string resolvedMode)
    {
        switch ((resolvedMode ?? string.Empty).Trim().ToLowerInvariant())
        {
            case ModeRetail:
                return "Trang ưu tiên danh mục sản phẩm, bán hàng và giỏ hàng, nhưng vẫn giữ đường dẫn đặt lịch và tư vấn khi cần.";
            case ModeService:
                return "Trang ưu tiên dịch vụ và lịch hẹn, nhưng vẫn sẵn sàng mở rộng sang kênh bán sản phẩm khi được cập nhật.";
            default:
                return "Trang được cân bằng để vừa chốt lịch, vừa chốt đơn, phù hợp mô hình gian hàng vận hành trên AhaSale.";
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

    public static string NormalizeStorefrontUrl(string rawUrl, string accountKey)
    {
        string value = (rawUrl ?? string.Empty).Trim();
        if (value == string.Empty)
            return string.Empty;

        return value;
    }

    private static bool NormalizeLegacyConfig(gianhang_storefront_config_table config)
    {
        if (config == null)
            return false;

        bool changed = false;
        config.brand_note = NormalizeLegacyField(config.brand_note, ref changed);
        config.nav_home_text = NormalizeLegacyField(config.nav_home_text, ref changed);
        config.nav_booking_text = NormalizeLegacyField(config.nav_booking_text, ref changed);
        config.quick_service_text = NormalizeLegacyField(config.quick_service_text, ref changed);
        config.quick_product_text = NormalizeLegacyField(config.quick_product_text, ref changed);
        config.quick_article_text = NormalizeLegacyField(config.quick_article_text, ref changed);
        config.quick_consult_text = NormalizeLegacyField(config.quick_consult_text, ref changed);
        config.quick_booking_text = NormalizeLegacyField(config.quick_booking_text, ref changed);
        config.hero_eyebrow = NormalizeLegacyField(config.hero_eyebrow, ref changed);
        config.hero_title = NormalizeLegacyField(config.hero_title, ref changed);
        config.hero_description = NormalizeLegacyField(config.hero_description, ref changed);
        config.hero_primary_text = NormalizeLegacyField(config.hero_primary_text, ref changed);
        config.hero_secondary_text = NormalizeLegacyField(config.hero_secondary_text, ref changed);
        config.hero_tertiary_text = NormalizeLegacyField(config.hero_tertiary_text, ref changed);
        config.hero_highlight_title = NormalizeLegacyField(config.hero_highlight_title, ref changed);
        config.hero_highlight_description = NormalizeLegacyField(config.hero_highlight_description, ref changed);
        config.hero_highlight_secondary_title = NormalizeLegacyField(config.hero_highlight_secondary_title, ref changed);
        config.hero_highlight_secondary_description = NormalizeLegacyField(config.hero_highlight_secondary_description, ref changed);
        config.hero_metric_service_text = NormalizeLegacyField(config.hero_metric_service_text, ref changed);
        config.hero_metric_product_text = NormalizeLegacyField(config.hero_metric_product_text, ref changed);
        config.hero_metric_article_text = NormalizeLegacyField(config.hero_metric_article_text, ref changed);
        config.footer_description = NormalizeLegacyField(config.footer_description, ref changed);
        config.footer_chip_1 = NormalizeLegacyField(config.footer_chip_1, ref changed);
        config.footer_chip_2 = NormalizeLegacyField(config.footer_chip_2, ref changed);
        config.footer_chip_3 = NormalizeLegacyField(config.footer_chip_3, ref changed);
        config.footer_chip_4 = NormalizeLegacyField(config.footer_chip_4, ref changed);
        config.footer_nav_title = NormalizeLegacyField(config.footer_nav_title, ref changed);
        config.footer_category_title = NormalizeLegacyField(config.footer_category_title, ref changed);
        config.footer_contact_title = NormalizeLegacyField(config.footer_contact_title, ref changed);
        config.footer_bottom_primary_text = NormalizeLegacyField(config.footer_bottom_primary_text, ref changed);
        config.footer_bottom_secondary_text = NormalizeLegacyField(config.footer_bottom_secondary_text, ref changed);
        return changed;
    }

    private static bool NormalizeLegacySection(gianhang_storefront_section_table section)
    {
        if (section == null)
            return false;

        bool changed = false;
        section.subtitle = NormalizeLegacyField(section.subtitle, ref changed);
        section.title = NormalizeLegacyField(section.title, ref changed);
        section.description = NormalizeLegacyField(section.description, ref changed);
        section.badge_text = NormalizeLegacyField(section.badge_text, ref changed);
        section.item_label = NormalizeLegacyField(section.item_label, ref changed);
        section.cta_text = NormalizeLegacyField(section.cta_text, ref changed);
        section.secondary_cta_text = NormalizeLegacyField(section.secondary_cta_text, ref changed);
        return changed;
    }

    private static string NormalizeLegacyField(string value, ref bool changed)
    {
        string normalized = NormalizeLegacyCopy(value);
        if (!string.Equals(normalized, value ?? string.Empty, StringComparison.Ordinal))
            changed = true;
        return normalized;
    }

    private static string NormalizeLegacyCopy(string value)
    {
        string text = (value ?? string.Empty).Trim();
        switch (text)
        {
            case "Storefront /gianhang duoc quan tri tu /gianhang/admin.":
                return "Gian hàng được quản trị từ khu vận hành riêng.";
            case "Trang chu":
                return "Trang chủ";
            case "Trang cong khai":
                return "Trang công khai";
            case "Dich vu":
                return "Dịch vụ";
            case "San pham":
                return "Sản phẩm";
            case "Bai viet":
                return "Bài viết";
            case "Tu van nhanh":
                return "Tư vấn nhanh";
            case "Storefront /gianhang":
                return "Không gian gian hàng";
            case "Quan ly tin dang":
                return "Quản lý tin đăng";
            case "Theo doi don ban":
                return "Theo dõi đơn bán";
            case "Top san pham":
                return "Top sản phẩm";
            case "Uu tien theo ban chay va luot xem de nhin ra mat hang dang tao doanh thu.":
                return "Ưu tiên theo bán chạy và lượt xem để nhận ra mặt hàng đang tạo doanh thu.";
            case "Danh sach san pham cong khai":
                return "Danh sách tin công khai";
            case "Tat ca san pham va dich vu dang hien thi ra storefront /gianhang.":
                return "Tất cả sản phẩm và dịch vụ đang hiển thị trên trang công khai.";
            case "Dich vu dang hien thi":
                return "Dịch vụ đang hiển thị";
            case "San pham co the ban truc tiep":
                return "Sản phẩm có thể bán trực tiếp";
            case "Noi dung ho tro chuyen doi":
                return "Nội dung hỗ trợ chuyển đổi";
            case "Dong bo noi dung":
                return "Đồng bộ nội dung";
            case "Dat lich":
                return "Đặt lịch";
            case "Ban hang":
                return "Bán hàng";
            case "Cham soc khach":
                return "Chăm sóc khách";
            case "Dieu huong nhanh":
                return "Điều hướng nhanh";
            case "Danh muc tu admin":
                return "Danh mục từ quản trị";
            case "Lien he":
                return "Liên hệ";
            case "Dang ky tu van":
                return "Đăng ký tư vấn";
            case "Dat lich ngay":
                return "Đặt lịch ngay";
            case "Danh muc dich vu":
                return "Danh mục dịch vụ";
            case "Nhom dich vu duoc to chuc ro rang tren storefront /gianhang.":
                return "Nhóm dịch vụ được tổ chức rõ ràng trên trang công khai.";
            case "Cac nhom dich vu duoc sap xep de khi can thi co the dua len trang cong khai ma khong can sua code.":
                return "Các nhóm dịch vụ được sắp xếp sẵn để đưa lên trang công khai khi cần.";
            case "Danh muc":
                return "Danh mục";
            case "Nhom dich vu":
                return "Nhóm dịch vụ";
            case "Xem trang cong khai":
                return "Xem trang công khai";
            case "Quan ly dich vu":
                return "Quản lý dịch vụ";
            case "Dich vu can day manh":
                return "Dịch vụ cần đẩy mạnh";
            case "Dich vu chu luc se hien tren /gianhang cung cach card voi san pham.":
                return "Dịch vụ chủ lực sẽ hiển thị nổi bật để khách dễ xem và đặt lịch.";
            case "Khi dang dich vu trong /gianhang/admin, noi dung duoc dong bo sang feed cong khai de /gianhang va /home cung doc.":
                return "Khi đăng dịch vụ trong khu quản trị, nội dung sẽ được đồng bộ sang trang công khai.";
            case "Feed cong khai":
                return "Nguồn hiển thị";
            case "Dich vu chu luc":
                return "Dịch vụ chủ lực";
            case "Xem storefront":
                return "Xem trang công khai";
            case "San pham chu luc":
                return "Sản phẩm chủ lực";
            case "San pham va dich vu hien thi tren /gianhang tu du lieu quan tri trong /gianhang/admin.":
                return "Sản phẩm và dịch vụ hiển thị được lấy từ dữ liệu quản trị của gian hàng.";
            case "Khoi nay dai dien cho feed cong khai goc. /gianhang, /home va ve sau cac space khac se cung doc tu mot nguon hien thi.":
                return "Khối này đại diện cho nguồn hiển thị công khai dùng chung của gian hàng.";
            case "Nguon hien thi goc":
                return "Nguồn hiển thị";
            case "San pham cong khai":
                return "Tin công khai";
            case "Mo /gianhang":
                return "Mở trang công khai";
            case "Quan ly san pham":
                return "Quản lý sản phẩm";
            case "Bai viet tu van ho tro SEO, niem tin va chuyen doi tren trang cong khai /gianhang.":
                return "Bài viết tư vấn hỗ trợ SEO, tăng niềm tin và chuyển đổi trên trang công khai.";
            case "Neu can dang bai viet de lam noi dung dan dat, van quan tri trong /gianhang/admin va hien tren feed cong khai.":
                return "Bài viết được quản trị tại khu vận hành và hiển thị trên trang công khai.";
            case "Noi dung":
                return "Nội dung";
            case "Quan ly bai viet":
                return "Quản lý bài viết";
            case "Website gian hang duoc dong bo truc tiep tu /gianhang/admin de ban hang, dat lich va cham soc khach hang tren cung mot nen tang.":
                return "Thông tin gian hàng được đồng bộ để phục vụ bán hàng, đặt lịch và chăm sóc khách hàng trên cùng một nền tảng.";
            case "Gian hang doi tac":
                return "Gian hàng đối tác";
            case "Du lieu footer dang duoc dong bo. Vui long tai lai sau vai giay.":
                return "Dữ liệu chân trang đang được đồng bộ. Vui lòng tải lại sau vài giây.";
            case "Moi the card tap trung vao dung 3 thu: hinh anh, gia ban va hanh dong them gio hoac dat hang.":
                return "Mỗi thẻ tập trung vào 3 yếu tố chính: hình ảnh, giá bán và hành động thêm giỏ hoặc đặt hàng.";
            case "Muc nay dong vai tro nhu khu vuc best-seller cho dich vu: nhin nhanh, hieu nhanh va chuyen doi ngay.":
                return "Khu vực này đóng vai trò như nhóm dịch vụ nổi bật: nhìn nhanh, hiểu nhanh và chuyển đổi ngay.";
            default:
                return text;
        }
    }
}
