using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class HomeFooterArticle_cl
{
    public const string GroupSupport = "footer.support";
    public const string GroupAbout = "footer.about";

    private static readonly object SyncRoot = new object();
    private static bool _schemaEnsured;

    public sealed class FooterArticleItem
    {
        public int Id { get; set; }
        public string ContentKey { get; set; }
        public string GroupKey { get; set; }
        public string DisplayName { get; set; }
        public string Slug { get; set; }
        public string TargetUrl { get; set; }
        public string BodyContent { get; set; }
        public int SortOrder { get; set; }
        public bool IsEnabled { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsBuiltIn { get; set; }
    }

    public sealed class BuiltInFooterItem
    {
        public string ContentKey { get; private set; }
        public string GroupKey { get; private set; }
        public string DisplayName { get; private set; }
        public int SortOrder { get; private set; }
        public string DefaultBodyContent { get; private set; }

        public BuiltInFooterItem(string contentKey, string groupKey, string displayName, int sortOrder, string defaultBodyContent)
        {
            ContentKey = contentKey;
            GroupKey = groupKey;
            DisplayName = displayName;
            SortOrder = sortOrder;
            DefaultBodyContent = defaultBodyContent;
        }
    }

    public static List<BuiltInFooterItem> GetBuiltInItems()
    {
        return new List<BuiltInFooterItem>
        {
            new BuiltInFooterItem("footer.support.help_center", GroupSupport, "Trung tâm trợ giúp", 10, "Đây là nội dung Trung tâm trợ giúp. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.support.safe_trade", GroupSupport, "An toàn mua bán", 20, "Đây là nội dung An toàn mua bán. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.support.contact_support", GroupSupport, "Liên hệ hỗ trợ", 30, "Đây là nội dung Liên hệ hỗ trợ. Bạn có thể sửa tại trang quản trị nội dung Home."),

            new BuiltInFooterItem("footer.about.intro", GroupAbout, "Giới thiệu", 10, "Đây là nội dung Giới thiệu. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.about.policy", GroupAbout, "Quy chế hoạt động sàn", 20, "Đây là nội dung Quy chế hoạt động sàn. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.about.privacy", GroupAbout, "Chính sách bảo mật", 30, "Đây là nội dung Chính sách bảo mật. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.about.dispute", GroupAbout, "Giải quyết tranh chấp", 40, "Đây là nội dung Giải quyết tranh chấp. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.about.recruit", GroupAbout, "Tuyển dụng", 50, "Đây là nội dung Tuyển dụng. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.about.media", GroupAbout, "Truyền thông", 60, "Đây là nội dung Truyền thông. Bạn có thể sửa tại trang quản trị nội dung Home."),
            new BuiltInFooterItem("footer.about.blog", GroupAbout, "Blog", 70, "Đây là nội dung Blog. Bạn có thể sửa tại trang quản trị nội dung Home.")
        };
    }

    public static string NormalizeGroupKey(string groupKey)
    {
        string g = (groupKey ?? "").Trim().ToLowerInvariant();
        if (g == GroupSupport) return GroupSupport;
        if (g == GroupAbout) return GroupAbout;
        return GroupSupport;
    }

    public static bool IsValidGroupKey(string groupKey)
    {
        string g = NormalizeGroupKey(groupKey);
        return g == GroupSupport || g == GroupAbout;
    }

    public static string NormalizeContentKey(string contentKey)
    {
        return (contentKey ?? "").Trim().ToLowerInvariant();
    }

    public static bool IsValidContentKey(string contentKey)
    {
        string normalized = NormalizeContentKey(contentKey);
        if (string.IsNullOrWhiteSpace(normalized)) return false;
        return Regex.IsMatch(normalized, @"^[a-z0-9_.-]{5,120}$");
    }

    public static string ToSlug(string raw)
    {
        string s = RemoveDiacritics((raw ?? "").Trim().ToLowerInvariant());
        s = s.Replace('\u0111', 'd');
        s = Regex.Replace(s, @"[^a-z0-9]+", "-");
        s = Regex.Replace(s, @"-+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(s)) s = "noi-dung";
        if (s.Length > 160) s = s.Substring(0, 160).Trim('-');
        if (string.IsNullOrWhiteSpace(s)) s = "noi-dung";
        return s;
    }

    public static string BuildContentKey(string groupKey, string nameOrSlug)
    {
        string g = NormalizeGroupKey(groupKey);
        string token = ToSlug(nameOrSlug).Replace('-', '_');
        if (token.Length > 70) token = token.Substring(0, 70).Trim('_');
        if (string.IsNullOrWhiteSpace(token)) token = "item";
        return g + "." + token;
    }

    public static List<FooterArticleItem> GetAll(dbDataContext db)
    {
        EnsureSchema(db);
        var builtInMap = GetBuiltInItems().ToDictionary(x => x.ContentKey, x => x, StringComparer.OrdinalIgnoreCase);
        var list = new List<FooterArticleItem>();

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT id, content_key, group_key, display_name, slug, target_url, body_content, sort_order, is_enabled, updated_by, updated_at
FROM dbo.Home_Footer_Article_tb
ORDER BY group_key, sort_order, id";
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string contentKey = (reader["content_key"] as string ?? "").Trim().ToLowerInvariant();
                    list.Add(new FooterArticleItem
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        ContentKey = contentKey,
                        GroupKey = NormalizeGroupKey(reader["group_key"] as string ?? ""),
                        DisplayName = (reader["display_name"] as string ?? "").Trim(),
                        Slug = (reader["slug"] as string ?? "").Trim().ToLowerInvariant(),
                        TargetUrl = (reader["target_url"] as string ?? "").Trim(),
                        BodyContent = (reader["body_content"] as string ?? "").Trim(),
                        SortOrder = reader["sort_order"] == DBNull.Value ? 0 : Convert.ToInt32(reader["sort_order"]),
                        IsEnabled = reader["is_enabled"] != DBNull.Value && Convert.ToBoolean(reader["is_enabled"]),
                        UpdatedBy = (reader["updated_by"] as string ?? "").Trim(),
                        UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"]),
                        IsBuiltIn = builtInMap.ContainsKey(contentKey)
                    });
                }
            }
        }

        return list;
    }

    public static List<FooterArticleItem> GetEnabledByGroup(dbDataContext db, string groupKey)
    {
        string group = NormalizeGroupKey(groupKey);
        return GetAll(db)
            .Where(x => x.IsEnabled && string.Equals(x.GroupKey, group, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToList();
    }

    public static FooterArticleItem GetByContentKey(dbDataContext db, string contentKey)
    {
        string key = NormalizeContentKey(contentKey);
        if (!IsValidContentKey(key)) return null;
        return GetAll(db).FirstOrDefault(x => string.Equals(x.ContentKey, key, StringComparison.OrdinalIgnoreCase));
    }

    public static FooterArticleItem GetEnabledBySlug(dbDataContext db, string slugRaw)
    {
        string slug = ToSlug(slugRaw);
        if (string.IsNullOrWhiteSpace(slug)) return null;

        EnsureSchema(db);
        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 id, content_key, group_key, display_name, slug, target_url, body_content, sort_order, is_enabled, updated_by, updated_at
FROM dbo.Home_Footer_Article_tb
WHERE slug = @slug AND is_enabled = 1
ORDER BY id";
            cmd.Parameters.AddWithValue("@slug", slug);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read()) return null;
                return new FooterArticleItem
                {
                    Id = Convert.ToInt32(reader["id"]),
                    ContentKey = (reader["content_key"] as string ?? "").Trim().ToLowerInvariant(),
                    GroupKey = NormalizeGroupKey(reader["group_key"] as string ?? ""),
                    DisplayName = (reader["display_name"] as string ?? "").Trim(),
                    Slug = (reader["slug"] as string ?? "").Trim().ToLowerInvariant(),
                    TargetUrl = (reader["target_url"] as string ?? "").Trim(),
                    BodyContent = (reader["body_content"] as string ?? "").Trim(),
                    SortOrder = reader["sort_order"] == DBNull.Value ? 0 : Convert.ToInt32(reader["sort_order"]),
                    IsEnabled = true,
                    UpdatedBy = (reader["updated_by"] as string ?? "").Trim(),
                    UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"]),
                    IsBuiltIn = GetBuiltInItems().Any(x => string.Equals(x.ContentKey, (reader["content_key"] as string ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                };
            }
        }
    }

    public static void Upsert(
        dbDataContext db,
        string contentKey,
        string groupKey,
        string displayName,
        string slugInput,
        string targetUrl,
        string bodyContent,
        int sortOrder,
        bool isEnabled,
        string updatedBy)
    {
        EnsureSchema(db);

        string safeGroup = NormalizeGroupKey(groupKey);
        string safeDisplayName = (displayName ?? "").Trim();
        if (safeDisplayName.Length > 250) safeDisplayName = safeDisplayName.Substring(0, 250);
        if (string.IsNullOrWhiteSpace(safeDisplayName))
            throw new InvalidOperationException("Tên hiển thị không được để trống.");

        string safeKey = NormalizeContentKey(contentKey);
        if (!IsValidContentKey(safeKey))
            throw new InvalidOperationException("Mã nội dung footer không hợp lệ.");

        string slugBase = ToSlug(string.IsNullOrWhiteSpace(slugInput) ? safeDisplayName : slugInput);
        string safeSlug = EnsureUniqueSlug(db, slugBase, safeKey);
        string safeTargetUrl = NormalizeTargetUrl(targetUrl);
        string safeBodyContent = (bodyContent ?? "").Trim();
        string safeUpdatedBy = (updatedBy ?? "").Trim();
        if (safeUpdatedBy.Length > 50) safeUpdatedBy = safeUpdatedBy.Substring(0, 50);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
MERGE dbo.Home_Footer_Article_tb AS target
USING (SELECT @content_key AS content_key) AS source
ON target.content_key = source.content_key
WHEN MATCHED THEN
    UPDATE SET
        group_key = @group_key,
        display_name = @display_name,
        slug = @slug,
        target_url = @target_url,
        body_content = @body_content,
        sort_order = @sort_order,
        is_enabled = @is_enabled,
        updated_by = @updated_by,
        updated_at = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (content_key, group_key, display_name, slug, target_url, body_content, sort_order, is_enabled, updated_by, updated_at, created_at)
    VALUES (@content_key, @group_key, @display_name, @slug, @target_url, @body_content, @sort_order, @is_enabled, @updated_by, GETDATE(), GETDATE());";

            cmd.Parameters.AddWithValue("@content_key", safeKey);
            cmd.Parameters.AddWithValue("@group_key", safeGroup);
            cmd.Parameters.AddWithValue("@display_name", safeDisplayName);
            cmd.Parameters.AddWithValue("@slug", safeSlug);
            cmd.Parameters.AddWithValue("@target_url", (object)safeTargetUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@body_content", (object)safeBodyContent ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sort_order", sortOrder);
            cmd.Parameters.AddWithValue("@is_enabled", isEnabled);
            cmd.Parameters.AddWithValue("@updated_by", (object)safeUpdatedBy ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static string ResolveLinkUrl(FooterArticleItem item)
    {
        if (item == null) return "#";
        if (!string.IsNullOrWhiteSpace(item.TargetUrl))
            return item.TargetUrl;
        if (!string.IsNullOrWhiteSpace(item.Slug))
            return "/home/noi-dung-footer.aspx?slug=" + HttpUtility.UrlEncode(item.Slug);
        return "#";
    }

    public static string RenderBodyAsHtml(string bodyText)
    {
        string raw = (bodyText ?? "").Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return "<p>Nội dung đang được cập nhật.</p>";

        string normalized = raw.Replace("\r\n", "\n");
        string[] blocks = normalized.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

        StringBuilder sb = new StringBuilder();
        foreach (string block in blocks)
        {
            string line = (block ?? "").Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            string safe = HttpUtility.HtmlEncode(line).Replace("\n", "<br/>");
            sb.Append("<p>");
            sb.Append(safe);
            sb.Append("</p>");
        }

        string output = sb.ToString();
        if (string.IsNullOrWhiteSpace(output))
            return "<p>Nội dung đang được cập nhật.</p>";
        return output;
    }

    private static string NormalizeTargetUrl(string targetUrl)
    {
        string url = (targetUrl ?? "").Trim();
        if (string.IsNullOrWhiteSpace(url)) return "";
        if (url.StartsWith("/", StringComparison.Ordinal)) return url;
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) return url;
        if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return url;
        return "";
    }

    private static string EnsureUniqueSlug(dbDataContext db, string slugBase, string exceptContentKey)
    {
        string baseSlug = ToSlug(slugBase);
        string candidate = baseSlug;
        int suffix = 1;

        while (SlugExists(db, candidate, exceptContentKey))
        {
            suffix++;
            candidate = baseSlug + "-" + suffix.ToString(CultureInfo.InvariantCulture);
            if (candidate.Length > 160)
                candidate = candidate.Substring(0, 160).Trim('-');
        }

        return candidate;
    }

    private static bool SlugExists(dbDataContext db, string slug, string exceptContentKey)
    {
        string safeSlug = (slug ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(safeSlug)) return false;
        string exceptKey = NormalizeContentKey(exceptContentKey);

        EnsureSchema(db);
        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 1
FROM dbo.Home_Footer_Article_tb
WHERE slug = @slug
  AND content_key <> @except_key";
            cmd.Parameters.AddWithValue("@slug", safeSlug);
            cmd.Parameters.AddWithValue("@except_key", exceptKey);
            conn.Open();
            object value = cmd.ExecuteScalar();
            return value != null;
        }
    }

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        string normalized = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < normalized.Length; i++)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(normalized[i]);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(normalized[i]);
            }
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static void EnsureSchema(dbDataContext db)
    {
        if (_schemaEnsured) return;

        lock (SyncRoot)
        {
            if (_schemaEnsured) return;

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
IF OBJECT_ID('dbo.Home_Footer_Article_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Home_Footer_Article_tb
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        content_key NVARCHAR(120) NOT NULL,
        group_key NVARCHAR(50) NOT NULL,
        display_name NVARCHAR(250) NOT NULL,
        slug NVARCHAR(180) NOT NULL,
        target_url NVARCHAR(500) NULL,
        body_content NVARCHAR(MAX) NULL,
        sort_order INT NOT NULL CONSTRAINT DF_Home_Footer_Article_tb_sort_order DEFAULT(0),
        is_enabled BIT NOT NULL CONSTRAINT DF_Home_Footer_Article_tb_is_enabled DEFAULT(1),
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL,
        created_at DATETIME NOT NULL CONSTRAINT DF_Home_Footer_Article_tb_created_at DEFAULT(GETDATE())
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Home_Footer_Article_tb_content_key'
      AND object_id = OBJECT_ID(N'dbo.Home_Footer_Article_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Home_Footer_Article_tb_content_key
        ON dbo.Home_Footer_Article_tb(content_key);
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Home_Footer_Article_tb_slug'
      AND object_id = OBJECT_ID(N'dbo.Home_Footer_Article_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Home_Footer_Article_tb_slug
        ON dbo.Home_Footer_Article_tb(slug);
END;";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            SeedBuiltInIfMissing(db);
            _schemaEnsured = true;
        }
    }

    private static void SeedBuiltInIfMissing(dbDataContext db)
    {
        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            conn.Open();
            foreach (BuiltInFooterItem builtIn in GetBuiltInItems())
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Home_Footer_Article_tb WHERE content_key = @content_key)
BEGIN
    INSERT INTO dbo.Home_Footer_Article_tb
    (
        content_key, group_key, display_name, slug, target_url, body_content, sort_order, is_enabled, updated_by, updated_at, created_at
    )
    VALUES
    (
        @content_key, @group_key, @display_name, @slug, '', @body_content, @sort_order, 1, 'system_seed', GETDATE(), GETDATE()
    );
END;";

                cmd.Parameters.AddWithValue("@content_key", builtIn.ContentKey);
                cmd.Parameters.AddWithValue("@group_key", builtIn.GroupKey);
                cmd.Parameters.AddWithValue("@display_name", builtIn.DisplayName);
                cmd.Parameters.AddWithValue("@slug", ToSlug(builtIn.DisplayName));
                cmd.Parameters.AddWithValue("@body_content", builtIn.DefaultBodyContent);
                cmd.Parameters.AddWithValue("@sort_order", builtIn.SortOrder);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
