using System;

public static class GianHangStorefrontSchema_cl
{
    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void EnsureSafe(dbDataContext db)
    {
        if (db == null || _schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            int originalTimeout = db.CommandTimeout;
            try
            {
                if (originalTimeout <= 0 || originalTimeout < 180)
                    db.CommandTimeout = 180;

                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.gianhang_storefront_config_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.gianhang_storefront_config_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [id_chinhanh] NVarChar(50) NULL,
        [storefront_mode] NVarChar(50) NULL,
        [brand_note] NVarChar(MAX) NULL,
        [nav_home_text] NVarChar(255) NULL,
        [nav_booking_text] NVarChar(255) NULL,
        [quickstrip_visible] Bit NULL,
        [quick_service_text] NVarChar(255) NULL,
        [quick_product_text] NVarChar(255) NULL,
        [quick_article_text] NVarChar(255) NULL,
        [quick_consult_text] NVarChar(255) NULL,
        [quick_booking_text] NVarChar(255) NULL,
        [hero_eyebrow] NVarChar(MAX) NULL,
        [hero_title] NVarChar(MAX) NULL,
        [hero_description] NVarChar(MAX) NULL,
        [hero_primary_text] NVarChar(255) NULL,
        [hero_primary_url] NVarChar(MAX) NULL,
        [hero_secondary_text] NVarChar(255) NULL,
        [hero_secondary_url] NVarChar(MAX) NULL,
        [hero_tertiary_text] NVarChar(255) NULL,
        [hero_tertiary_url] NVarChar(MAX) NULL,
        [hero_highlight_title] NVarChar(255) NULL,
        [hero_highlight_description] NVarChar(MAX) NULL,
        [hero_highlight_secondary_title] NVarChar(255) NULL,
        [hero_highlight_secondary_description] NVarChar(MAX) NULL,
        [hero_metric_service_text] NVarChar(255) NULL,
        [hero_metric_product_text] NVarChar(255) NULL,
        [hero_metric_article_text] NVarChar(255) NULL,
        [footer_description] NVarChar(MAX) NULL,
        [footer_chip_1] NVarChar(255) NULL,
        [footer_chip_2] NVarChar(255) NULL,
        [footer_chip_3] NVarChar(255) NULL,
        [footer_chip_4] NVarChar(255) NULL,
        [footer_nav_title] NVarChar(255) NULL,
        [footer_category_title] NVarChar(255) NULL,
        [footer_contact_title] NVarChar(255) NULL,
        [footer_bottom_primary_text] NVarChar(255) NULL,
        [footer_bottom_primary_url] NVarChar(MAX) NULL,
        [footer_bottom_secondary_text] NVarChar(255) NULL,
        [footer_bottom_secondary_url] NVarChar(MAX) NULL,
        [updated_at] DateTime NULL
    );
END

IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'id_chinhanh') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [id_chinhanh] NVarChar(50) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'storefront_mode') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [storefront_mode] NVarChar(50) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'brand_note') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [brand_note] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'nav_home_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [nav_home_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'nav_booking_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [nav_booking_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'quickstrip_visible') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [quickstrip_visible] Bit NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'quick_service_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [quick_service_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'quick_product_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [quick_product_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'quick_article_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [quick_article_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'quick_consult_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [quick_consult_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'quick_booking_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [quick_booking_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_eyebrow') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_eyebrow] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_title') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_title] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_description') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_description] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_primary_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_primary_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_primary_url') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_primary_url] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_secondary_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_secondary_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_secondary_url') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_secondary_url] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_tertiary_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_tertiary_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_tertiary_url') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_tertiary_url] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_highlight_title') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_highlight_title] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_highlight_description') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_highlight_description] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_highlight_secondary_title') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_highlight_secondary_title] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_highlight_secondary_description') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_highlight_secondary_description] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_metric_service_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_metric_service_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_metric_product_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_metric_product_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'hero_metric_article_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [hero_metric_article_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_description') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_description] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_chip_1') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_chip_1] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_chip_2') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_chip_2] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_chip_3') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_chip_3] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_chip_4') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_chip_4] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_nav_title') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_nav_title] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_category_title') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_category_title] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_contact_title') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_contact_title] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_bottom_primary_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_bottom_primary_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_bottom_primary_url') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_bottom_primary_url] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_bottom_secondary_text') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_bottom_secondary_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'footer_bottom_secondary_url') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [footer_bottom_secondary_url] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_config_table', 'updated_at') IS NULL ALTER TABLE dbo.gianhang_storefront_config_table ADD [updated_at] DateTime NULL;

IF OBJECT_ID('dbo.gianhang_storefront_section_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.gianhang_storefront_section_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [id_chinhanh] NVarChar(50) NULL,
        [section_key] NVarChar(100) NULL,
        [title] NVarChar(MAX) NULL,
        [subtitle] NVarChar(MAX) NULL,
        [description] NVarChar(MAX) NULL,
        [image] NVarChar(MAX) NULL,
        [badge_text] NVarChar(255) NULL,
        [item_label] NVarChar(255) NULL,
        [cta_text] NVarChar(255) NULL,
        [cta_url] NVarChar(MAX) NULL,
        [secondary_cta_text] NVarChar(255) NULL,
        [secondary_cta_url] NVarChar(MAX) NULL,
        [source_type] NVarChar(100) NULL,
        [source_value] NVarChar(MAX) NULL,
        [style_variant] NVarChar(100) NULL,
        [rank] Int NULL,
        [is_visible] Bit NULL,
        [item_limit] Int NULL,
        [extra_json] NVarChar(MAX) NULL,
        [updated_at] DateTime NULL
    );
END

IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'id_chinhanh') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [id_chinhanh] NVarChar(50) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'section_key') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [section_key] NVarChar(100) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'title') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [title] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'subtitle') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [subtitle] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'description') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [description] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'image') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [image] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'badge_text') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [badge_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'item_label') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [item_label] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'cta_text') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [cta_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'cta_url') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [cta_url] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'secondary_cta_text') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [secondary_cta_text] NVarChar(255) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'secondary_cta_url') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [secondary_cta_url] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'source_type') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [source_type] NVarChar(100) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'source_value') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [source_value] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'style_variant') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [style_variant] NVarChar(100) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'rank') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [rank] Int NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'is_visible') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [is_visible] Bit NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'item_limit') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [item_limit] Int NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'extra_json') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [extra_json] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.gianhang_storefront_section_table', 'updated_at') IS NULL ALTER TABLE dbo.gianhang_storefront_section_table ADD [updated_at] DateTime NULL;
");

                _schemaReady = true;
            }
            catch
            {
            }
            finally
            {
                db.CommandTimeout = originalTimeout;
            }
        }
    }

    public static void ResetCache()
    {
        lock (_schemaLock)
        {
            _schemaReady = false;
        }
    }
}
