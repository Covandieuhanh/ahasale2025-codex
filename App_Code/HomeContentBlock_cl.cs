using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

public static class HomeContentBlock_cl
{
    public const string PermissionCode = "q3_1";

    public const string KeyFooter = "home.footer";
    public const string KeyPopularKeywords = "home.popular_keywords";
    public const string KeyAboutLong = "home.about_long";

    private static readonly object SyncRoot = new object();
    private static bool _schemaEnsured;

    public sealed class BlockItem
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string HtmlContent { get; set; }
        public bool IsEnabled { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsBuiltIn { get; set; }
    }

    public sealed class BuiltInBlock
    {
        public BuiltInBlock(string key, string title)
        {
            Key = key;
            Title = title;
        }

        public string Key { get; private set; }
        public string Title { get; private set; }
    }

    public static IReadOnlyList<BuiltInBlock> GetBuiltInBlocks()
    {
        return new[]
        {
            new BuiltInBlock(KeyFooter, "Footer trang chủ Home"),
            new BuiltInBlock(KeyPopularKeywords, "Khối Từ khóa phổ biến"),
            new BuiltInBlock(KeyAboutLong, "Khối Giới thiệu dài")
        };
    }

    public static bool IsValidKey(string key)
    {
        string normalized = NormalizeKey(key);
        if (string.IsNullOrEmpty(normalized)) return false;
        return Regex.IsMatch(normalized, @"^[a-zA-Z0-9_.-]{3,100}$");
    }

    public static string NormalizeKey(string key)
    {
        return (key ?? "").Trim();
    }

    public static string SanitizeHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return "";

        string output = html;
        output = Regex.Replace(output, @"<\s*script[^>]*?>[\s\S]*?<\s*/\s*script\s*>", "", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"on[a-z]+\s*=\s*(['""]).*?\1", "", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"on[a-z]+\s*=\s*[^\s>]+", "", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"javascript\s*:", "", RegexOptions.IgnoreCase);
        return output.Trim();
    }

    public static string GetEnabledHtmlByKeyOrEmpty(dbDataContext db, string key)
    {
        BlockItem item = GetByKey(db, key);
        if (item == null) return "";
        if (!item.IsEnabled) return "";
        return SanitizeHtml(item.HtmlContent);
    }

    public static BlockItem GetByKey(dbDataContext db, string key)
    {
        EnsureSchema(db);

        string blockKey = NormalizeKey(key);
        if (!IsValidKey(blockKey))
            return null;

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 block_key, title, html_content, is_enabled, updated_by, updated_at
FROM dbo.Home_Content_Block_tb
WHERE block_key = @key";
            cmd.Parameters.AddWithValue("@key", blockKey);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read()) return null;

                return new BlockItem
                {
                    Key = reader["block_key"] as string ?? "",
                    Title = reader["title"] as string ?? "",
                    HtmlContent = reader["html_content"] as string ?? "",
                    IsEnabled = reader["is_enabled"] != DBNull.Value && Convert.ToBoolean(reader["is_enabled"]),
                    UpdatedBy = reader["updated_by"] as string ?? "",
                    UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"])
                };
            }
        }
    }

    public static List<BlockItem> GetAll(dbDataContext db)
    {
        EnsureSchema(db);

        var builtInMap = GetBuiltInBlocks().ToDictionary(x => x.Key, x => x.Title, StringComparer.OrdinalIgnoreCase);
        var rows = new List<BlockItem>();

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT block_key, title, html_content, is_enabled, updated_by, updated_at
FROM dbo.Home_Content_Block_tb
ORDER BY block_key";
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string key = reader["block_key"] as string ?? "";
                    rows.Add(new BlockItem
                    {
                        Key = key,
                        Title = (reader["title"] as string ?? "").Trim(),
                        HtmlContent = reader["html_content"] as string ?? "",
                        IsEnabled = reader["is_enabled"] != DBNull.Value && Convert.ToBoolean(reader["is_enabled"]),
                        UpdatedBy = reader["updated_by"] as string ?? "",
                        UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"]),
                        IsBuiltIn = builtInMap.ContainsKey(key)
                    });
                }
            }
        }

        foreach (BuiltInBlock block in GetBuiltInBlocks())
        {
            if (rows.Any(x => string.Equals(x.Key, block.Key, StringComparison.OrdinalIgnoreCase)))
                continue;

            rows.Add(new BlockItem
            {
                Key = block.Key,
                Title = block.Title,
                HtmlContent = "",
                IsEnabled = true,
                UpdatedBy = "",
                UpdatedAt = null,
                IsBuiltIn = true
            });
        }

        return rows
            .OrderByDescending(x => x.IsBuiltIn)
            .ThenBy(x => x.Key)
            .ToList();
    }

    public static void Upsert(dbDataContext db, string key, string title, string htmlContent, bool isEnabled, string updatedBy)
    {
        EnsureSchema(db);

        string blockKey = NormalizeKey(key);
        if (!IsValidKey(blockKey))
            throw new InvalidOperationException("Mã vị trí không hợp lệ.");

        string cleanTitle = (title ?? "").Trim();
        if (cleanTitle.Length > 250)
            cleanTitle = cleanTitle.Substring(0, 250);

        string cleanHtml = (htmlContent ?? "").Trim();
        string cleanUpdatedBy = (updatedBy ?? "").Trim();
        if (cleanUpdatedBy.Length > 50)
            cleanUpdatedBy = cleanUpdatedBy.Substring(0, 50);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
MERGE dbo.Home_Content_Block_tb AS target
USING (SELECT @key AS block_key) AS source
ON target.block_key = source.block_key
WHEN MATCHED THEN
    UPDATE SET
        title = @title,
        html_content = @html_content,
        is_enabled = @is_enabled,
        updated_by = @updated_by,
        updated_at = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (block_key, title, html_content, is_enabled, updated_by, updated_at, created_at)
    VALUES (@key, @title, @html_content, @is_enabled, @updated_by, GETDATE(), GETDATE());";

            cmd.Parameters.AddWithValue("@key", blockKey);
            cmd.Parameters.AddWithValue("@title", (object)cleanTitle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@html_content", (object)cleanHtml ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_enabled", isEnabled);
            cmd.Parameters.AddWithValue("@updated_by", (object)cleanUpdatedBy ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
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
IF OBJECT_ID('dbo.Home_Content_Block_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Home_Content_Block_tb
    (
        block_key NVARCHAR(100) NOT NULL PRIMARY KEY,
        title NVARCHAR(250) NULL,
        html_content NVARCHAR(MAX) NULL,
        is_enabled BIT NOT NULL CONSTRAINT DF_Home_Content_Block_tb_is_enabled DEFAULT(1),
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL,
        created_at DATETIME NOT NULL CONSTRAINT DF_Home_Content_Block_tb_created_at DEFAULT(GETDATE())
    );
END;";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _schemaEnsured = true;
        }
    }
}
