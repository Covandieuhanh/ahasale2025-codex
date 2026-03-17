using System;
using System.Data.Linq.Mapping;

[Table(Name = "dbo.gianhang_storefront_config_table")]
public class gianhang_storefront_config_table
{
    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public long id { get; set; }

    [Column]
    public string id_chinhanh { get; set; }

    [Column]
    public string storefront_mode { get; set; }

    [Column]
    public string brand_note { get; set; }

    [Column]
    public string nav_home_text { get; set; }

    [Column]
    public string nav_booking_text { get; set; }

    [Column]
    public bool? quickstrip_visible { get; set; }

    [Column]
    public string quick_service_text { get; set; }

    [Column]
    public string quick_product_text { get; set; }

    [Column]
    public string quick_article_text { get; set; }

    [Column]
    public string quick_consult_text { get; set; }

    [Column]
    public string quick_booking_text { get; set; }

    [Column]
    public string hero_eyebrow { get; set; }

    [Column]
    public string hero_title { get; set; }

    [Column]
    public string hero_description { get; set; }

    [Column]
    public string hero_primary_text { get; set; }

    [Column]
    public string hero_primary_url { get; set; }

    [Column]
    public string hero_secondary_text { get; set; }

    [Column]
    public string hero_secondary_url { get; set; }

    [Column]
    public string hero_tertiary_text { get; set; }

    [Column]
    public string hero_tertiary_url { get; set; }

    [Column]
    public string hero_highlight_title { get; set; }

    [Column]
    public string hero_highlight_description { get; set; }

    [Column]
    public string hero_highlight_secondary_title { get; set; }

    [Column]
    public string hero_highlight_secondary_description { get; set; }

    [Column]
    public string hero_metric_service_text { get; set; }

    [Column]
    public string hero_metric_product_text { get; set; }

    [Column]
    public string hero_metric_article_text { get; set; }

    [Column]
    public string footer_description { get; set; }

    [Column]
    public string footer_chip_1 { get; set; }

    [Column]
    public string footer_chip_2 { get; set; }

    [Column]
    public string footer_chip_3 { get; set; }

    [Column]
    public string footer_chip_4 { get; set; }

    [Column]
    public string footer_nav_title { get; set; }

    [Column]
    public string footer_category_title { get; set; }

    [Column]
    public string footer_contact_title { get; set; }

    [Column]
    public string footer_bottom_primary_text { get; set; }

    [Column]
    public string footer_bottom_primary_url { get; set; }

    [Column]
    public string footer_bottom_secondary_text { get; set; }

    [Column]
    public string footer_bottom_secondary_url { get; set; }

    [Column]
    public DateTime? updated_at { get; set; }
}

[Table(Name = "dbo.gianhang_storefront_section_table")]
public class gianhang_storefront_section_table
{
    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public long id { get; set; }

    [Column]
    public string id_chinhanh { get; set; }

    [Column]
    public string section_key { get; set; }

    [Column]
    public string title { get; set; }

    [Column]
    public string subtitle { get; set; }

    [Column]
    public string description { get; set; }

    [Column]
    public string image { get; set; }

    [Column]
    public string badge_text { get; set; }

    [Column]
    public string item_label { get; set; }

    [Column]
    public string cta_text { get; set; }

    [Column]
    public string cta_url { get; set; }

    [Column]
    public string secondary_cta_text { get; set; }

    [Column]
    public string secondary_cta_url { get; set; }

    [Column]
    public string source_type { get; set; }

    [Column]
    public string source_value { get; set; }

    [Column]
    public string style_variant { get; set; }

    [Column]
    public int? rank { get; set; }

    [Column]
    public bool? is_visible { get; set; }

    [Column]
    public int? item_limit { get; set; }

    [Column]
    public string extra_json { get; set; }

    [Column]
    public DateTime? updated_at { get; set; }
}
