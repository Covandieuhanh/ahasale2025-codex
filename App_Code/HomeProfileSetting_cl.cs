using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public static class HomeProfileSetting_cl
{
    private static readonly object SyncRoot = new object();
    private static bool _schemaEnsured;

    private static readonly string[] AllowedTemplates = new[] { "classic", "pro", "creator" };

    public sealed class ProfileSettings
    {
        public string TemplateKey { get; set; }
        public string AccentColor { get; set; }
        public bool ShowContact { get; set; }
        public bool ShowSocial { get; set; }
        public bool ShowReviews { get; set; }
        public bool ShowShop { get; set; }
        public bool ShowProducts { get; set; }
        public string SocialOrderPersonal { get; set; }
        public string SocialOrderShop { get; set; }
    }

    public static ProfileSettings GetSettings(dbDataContext db, string taiKhoan, bool isShop)
    {
        EnsureSchema(db);

        ProfileSettings defaults = GetDefaults(isShop);
        if (string.IsNullOrWhiteSpace(taiKhoan))
            return defaults;

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 template_key, accent_color, show_contact, show_social, show_reviews, show_shop, show_products, social_order_personal, social_order_shop
FROM dbo.Home_Profile_Setting_tb
WHERE taikhoan = @tk";
            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    return defaults;

                ProfileSettings settings = new ProfileSettings
                {
                    TemplateKey = NormalizeTemplate(reader["template_key"] as string),
                    AccentColor = NormalizeAccent(reader["accent_color"] as string),
                    ShowContact = reader["show_contact"] != DBNull.Value && Convert.ToBoolean(reader["show_contact"]),
                    ShowSocial = reader["show_social"] == DBNull.Value || Convert.ToBoolean(reader["show_social"]),
                    ShowReviews = reader["show_reviews"] == DBNull.Value || Convert.ToBoolean(reader["show_reviews"]),
                    ShowShop = reader["show_shop"] != DBNull.Value && Convert.ToBoolean(reader["show_shop"]),
                    ShowProducts = reader["show_products"] != DBNull.Value && Convert.ToBoolean(reader["show_products"]),
                    SocialOrderPersonal = NormalizeOrder(reader["social_order_personal"] as string),
                    SocialOrderShop = NormalizeOrder(reader["social_order_shop"] as string)
                };

                if (string.IsNullOrEmpty(settings.TemplateKey))
                    settings.TemplateKey = defaults.TemplateKey;
                if (string.IsNullOrEmpty(settings.AccentColor))
                    settings.AccentColor = defaults.AccentColor;

                if (!isShop)
                {
                    settings.ShowShop = false;
                    settings.ShowProducts = false;
                }

                return settings;
            }
        }
    }

    public static void Upsert(dbDataContext db, string taiKhoan, ProfileSettings settings, string updatedBy, bool isShop)
    {
        EnsureSchema(db);
        if (string.IsNullOrWhiteSpace(taiKhoan))
            return;

        ProfileSettings normalized = NormalizeSettings(settings, isShop);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
MERGE dbo.Home_Profile_Setting_tb AS target
USING (SELECT @tk AS taikhoan) AS source
ON target.taikhoan = source.taikhoan
WHEN MATCHED THEN UPDATE SET
    template_key = @template_key,
    accent_color = @accent_color,
    show_contact = @show_contact,
    show_social = @show_social,
    show_reviews = @show_reviews,
    show_shop = @show_shop,
    show_products = @show_products,
    social_order_personal = @social_order_personal,
    social_order_shop = @social_order_shop,
    updated_at = @updated_at,
    updated_by = @updated_by
WHEN NOT MATCHED THEN
    INSERT (taikhoan, template_key, accent_color, show_contact, show_social, show_reviews, show_shop, show_products, social_order_personal, social_order_shop, updated_at, updated_by)
    VALUES (@tk, @template_key, @accent_color, @show_contact, @show_social, @show_reviews, @show_shop, @show_products, @social_order_personal, @social_order_shop, @updated_at, @updated_by);";

            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            cmd.Parameters.AddWithValue("@template_key", normalized.TemplateKey ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@accent_color", normalized.AccentColor ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@show_contact", normalized.ShowContact);
            cmd.Parameters.AddWithValue("@show_social", normalized.ShowSocial);
            cmd.Parameters.AddWithValue("@show_reviews", normalized.ShowReviews);
            cmd.Parameters.AddWithValue("@show_shop", normalized.ShowShop);
            cmd.Parameters.AddWithValue("@show_products", normalized.ShowProducts);
            cmd.Parameters.AddWithValue("@social_order_personal", normalized.SocialOrderPersonal ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@social_order_shop", normalized.SocialOrderShop ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@updated_at", AhaTime_cl.Now);
            cmd.Parameters.AddWithValue("@updated_by", (updatedBy ?? "").Trim());

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    private static ProfileSettings NormalizeSettings(ProfileSettings settings, bool isShop)
    {
        ProfileSettings defaults = GetDefaults(isShop);
        if (settings == null)
            return defaults;

        ProfileSettings normalized = new ProfileSettings
        {
            TemplateKey = NormalizeTemplate(settings.TemplateKey),
            AccentColor = NormalizeAccent(settings.AccentColor),
            ShowContact = settings.ShowContact,
            ShowSocial = settings.ShowSocial,
            ShowReviews = settings.ShowReviews,
            ShowShop = settings.ShowShop,
            ShowProducts = settings.ShowProducts,
            SocialOrderPersonal = NormalizeOrder(settings.SocialOrderPersonal),
            SocialOrderShop = NormalizeOrder(settings.SocialOrderShop)
        };

        if (string.IsNullOrEmpty(normalized.TemplateKey))
            normalized.TemplateKey = defaults.TemplateKey;
        if (string.IsNullOrEmpty(normalized.AccentColor))
            normalized.AccentColor = defaults.AccentColor;

        if (!isShop)
        {
            normalized.ShowShop = false;
            normalized.ShowProducts = false;
        }

        return normalized;
    }

    private static ProfileSettings GetDefaults(bool isShop)
    {
        return new ProfileSettings
        {
            TemplateKey = "classic",
            AccentColor = "#22c55e",
            ShowContact = true,
            ShowSocial = true,
            ShowReviews = true,
            ShowShop = false,
            ShowProducts = false,
            SocialOrderPersonal = "",
            SocialOrderShop = ""
        };
    }

    private static string NormalizeTemplate(string key)
    {
        string normalized = (key ?? "").Trim().ToLowerInvariant();
        foreach (string template in AllowedTemplates)
        {
            if (string.Equals(template, normalized, StringComparison.OrdinalIgnoreCase))
                return template;
        }
        return "";
    }

    private static string NormalizeAccent(string color)
    {
        string raw = (color ?? "").Trim();
        if (Regex.IsMatch(raw, "^#([0-9a-fA-F]{6})$"))
            return raw.ToLowerInvariant();
        return "";
    }

    private static string NormalizeOrder(string value)
    {
        string raw = (value ?? "").Trim();
        if (string.IsNullOrEmpty(raw))
            return "";

        raw = Regex.Replace(raw, "[^0-9,]", "");
        raw = Regex.Replace(raw, ",{2,}", ",");
        return raw.Trim(',');
    }

    private static void EnsureSchema(dbDataContext db)
    {
        if (_schemaEnsured || db == null)
            return;

        lock (SyncRoot)
        {
            if (_schemaEnsured) return;

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Home_Profile_Setting_tb')
BEGIN
    CREATE TABLE dbo.Home_Profile_Setting_tb (
        taikhoan NVARCHAR(50) NOT NULL PRIMARY KEY,
        template_key NVARCHAR(30) NULL,
        accent_color NVARCHAR(20) NULL,
        show_contact BIT NULL,
        show_social BIT NULL,
        show_reviews BIT NULL,
        show_shop BIT NULL,
        show_products BIT NULL,
        social_order_personal NVARCHAR(MAX) NULL,
        social_order_shop NVARCHAR(MAX) NULL,
        updated_at DATETIME NULL,
        updated_by NVARCHAR(50) NULL
    )
END
IF COL_LENGTH('dbo.Home_Profile_Setting_tb', 'social_order_personal') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Profile_Setting_tb ADD social_order_personal NVARCHAR(MAX) NULL
END
IF COL_LENGTH('dbo.Home_Profile_Setting_tb', 'social_order_shop') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Profile_Setting_tb ADD social_order_shop NVARCHAR(MAX) NULL
END";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _schemaEnsured = true;
        }
    }
}
