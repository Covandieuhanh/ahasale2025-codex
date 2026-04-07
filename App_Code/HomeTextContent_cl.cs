using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class HomeTextContent_cl
{
    public const string PermissionCode = "q3_1";

    public const string KeyPopularKeywords = "home.popular_keywords";
    public const string KeyAboutLong = "home.about_long";
    public const string KeyFooterContactEmail = "home.footer.contact_email";
    public const string KeyFooterContactHotline = "home.footer.contact_hotline";
    public const string KeyFooterContactAddress = "home.footer.contact_address";
    public const string KeyFooterLegalLine = "home.footer.legal_line";
    public const string KeyFooterPolicyUsageText = "home.footer.policy_usage_text";
    public const string KeyFooterPolicyUsageUrl = "home.footer.policy_usage_url";
    public const string KeyFooterSocialLinkedin = "home.footer.social_linkedin";
    public const string KeyFooterSocialYoutube = "home.footer.social_youtube";
    public const string KeyFooterSocialFacebook = "home.footer.social_facebook";

    private static readonly object SyncRoot = new object();
    private static bool _schemaEnsured;
    private const int EffectiveCacheSeconds = 120;

    public sealed class TextContentItem
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string TextContent { get; set; }
        public bool IsEnabled { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsBuiltIn { get; set; }
    }

    public sealed class BuiltInTextItem
    {
        public string Key { get; private set; }
        public string Title { get; private set; }
        public string DefaultText { get; private set; }

        public BuiltInTextItem(string key, string title, string defaultText)
        {
            Key = key;
            Title = title;
            DefaultText = defaultText;
        }
    }

    public sealed class KeywordLineItem
    {
        public string Label { get; set; }
        public string Url { get; set; }
    }

    public static List<BuiltInTextItem> GetBuiltInItems()
    {
        return new List<BuiltInTextItem>
        {
            new BuiltInTextItem(
                KeyPopularKeywords,
                "Các từ khóa phổ biến",
                string.Join("\n", new[]
                {
                    "iPhone 12",
                    "iPhone 14 Pro Max",
                    "Điện thoại iPhone cũ",
                    "Điện thoại Samsung cũ",
                    "Máy quay cũ",
                    "Loa cũ",
                    "Điện thoại cũ",
                    "iPhone 12 Mini",
                    "iPhone 14 Plus",
                    "Dàn karaoke cũ",
                    "Máy tính để bàn giá rẻ",
                    "Micro cũ",
                    "Máy tính để bàn cũ",
                    "Macbook",
                    "iPhone 12 Pro",
                    "iPhone 14 Pro",
                    "Tivi cũ giá rẻ",
                    "Ống kính (lens) cũ",
                    "Tai nghe cũ",
                    "Máy tính bảng cũ",
                    "iPhone 12 Pro Max",
                    "Samsung S25 Edge",
                    "iPhone 16e",
                    "Máy ảnh cũ",
                    "Amply",
                    "Laptop cũ"
                })),
            new BuiltInTextItem(
                KeyAboutLong,
                "AhaSale - Chợ Mua Bán, Rao Vặt Trực Tuyến Hàng Đầu Của Người Việt",
                string.Join("\n\n", new[]
                {
                    "AhaSale chính thức gia nhập thị trường Việt Nam vào đầu năm 2012, với mục đích tạo ra cho bạn một kênh rao vặt trung gian, kết nối người mua với người bán lại với nhau bằng những Trao đổi cực kỳ đơn giản, tiện lợi, nhanh chóng, an toàn, mang đến hiệu quả bất ngờ.",
                    "Đến nay, AhaSale tự hào là Website rao vặt được ưa chuộng hàng đầu Việt Nam. Hàng ngàn món hời từ Bất động sản, Nhà cửa, Xe cộ, Đồ điện tử, Thú cưng, Vật dụng cá nhân..., đến tìm việc làm, thông tin tuyển dụng, các dịch vụ - du lịch được đăng tin, rao bán trên AhaSale.",
                    "Với AhaSale, bạn có thể dễ dàng mua bán, trao đổi bất cứ một loại mặt hàng nào, dù đó là đồ cũ hay đồ mới với nhiều lĩnh vực:",
                    "Bất động sản: Cho thuê, Mua bán nhà đất, căn hộ chung cư, văn phòng mặt bằng kinh doanh, phòng trọ đa dạng về diện tích, vị trí.",
                    "Phương tiện đi lại: Mua bán ô tô, xe máy có đủ biển cao, giá cả hợp lý, giấy tờ đầy đủ.",
                    "Đồ dùng cá nhân: Quần áo, giày dép, túi xách, đồng hồ... đa phong cách, hợp thời trang.",
                    "Đồ điện tử: Điện thoại di động, máy tính bảng, laptop, tivi, loa, amply..., đồ điện gia dụng: máy giặt, tủ lạnh, máy lạnh điều hòa... với rất nhiều nhãn hiệu, kích thước khác nhau.",
                    "Vật nuôi, thú cưng: đa dạng thú cưng loại cảnh, chó, chim, mèo, cá, hamster... giá cực tốt.",
                    "Tuyển dụng, việc làm: Hàng triệu công việc hấp dẫn, phù hợp tại Việc Làm Tốt - Kênh tuyển dụng hàng hiệu quả, uy tín được phát triển bởi AhaSale.",
                    "Dịch vụ, du lịch: Khách sạn, vé máy bay, vé tàu, vé xe, tour du lịch và các voucher du lịch... uy tín, chất lượng.",
                    "Đồ ăn, thực phẩm: Các món ăn được chế biến thơm ngon, hấp dẫn, thực phẩm tươi sống, an toàn và giá cả hợp lý.",
                    "Và còn rất nhiều mặt hàng khác nữa đã và đang được rao bán tại AhaSale.",
                    "Mỗi người trong chúng ta đều có những sản phẩm đã qua sử dụng và không còn dùng tới nữa. Vậy còn chần chừ gì nữa mà không để nó trở nên giá trị hơn với người khác.",
                    "Không những thế, website ahasale.vn còn cung cấp cho bạn thông tin về giá cả các mặt hàng đang bán để bạn có thể tham khảo. Đồng thời, thông qua Blog kinh nghiệm, AhaSale sẽ tư vấn, chia sẻ cho bạn những thông tin bổ ích.",
                    "Chúc các bạn có những trải nghiệm mua bán tuyệt vời trên AhaSale."
                })),
            new BuiltInTextItem(
                KeyFooterContactEmail,
                "Footer - Email liên hệ",
                "ahasale.vn@gmail.com"),
            new BuiltInTextItem(
                KeyFooterContactHotline,
                "Footer - Số CSKH",
                "0868.877.686"),
            new BuiltInTextItem(
                KeyFooterContactAddress,
                "Footer - Địa chỉ công ty",
                "Số 46/3, đường Võ Thị Sáu, khu phố Gò Me, Phường Trấn Biên, Tỉnh Đồng Nai, Việt Nam"),
            new BuiltInTextItem(
                KeyFooterLegalLine,
                "Footer - Dòng pháp lý cuối trang",
                "CÔNG TY CỔ PHẦN ĐÀO TẠO AHA SALE - Người đại diện theo pháp luật: Trần Đức Cường; GPKDKD: 3603907499 do Sở tài chính tỉnh Đồng Nai cấp ngày 29/03/2023;"),
            new BuiltInTextItem(
                KeyFooterPolicyUsageText,
                "Footer - Tên link chính sách sử dụng",
                "Chính sách sử dụng"),
            new BuiltInTextItem(
                KeyFooterPolicyUsageUrl,
                "Footer - Link chính sách sử dụng",
                "/home/noi-dung-footer.aspx?slug=quy-che-hoat-dong-san"),
            new BuiltInTextItem(
                KeyFooterSocialLinkedin,
                "Footer - Link LinkedIn",
                "#"),
            new BuiltInTextItem(
                KeyFooterSocialYoutube,
                "Footer - Link YouTube",
                "#"),
            new BuiltInTextItem(
                KeyFooterSocialFacebook,
                "Footer - Link Facebook",
                "#")
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
        return (key ?? "").Trim().ToLowerInvariant();
    }

    public static List<TextContentItem> GetAll(dbDataContext db)
    {
        EnsureSchema(db);

        var builtInMap = GetBuiltInItems().ToDictionary(x => x.Key, x => x, StringComparer.OrdinalIgnoreCase);
        var rows = new List<TextContentItem>();

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT content_key, title, text_content, is_enabled, updated_by, updated_at
FROM dbo.Home_Text_Content_tb
ORDER BY content_key";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string key = (reader["content_key"] as string ?? "").Trim();
                    rows.Add(new TextContentItem
                    {
                        Key = key,
                        Title = (reader["title"] as string ?? "").Trim(),
                        TextContent = reader["text_content"] as string ?? "",
                        IsEnabled = reader["is_enabled"] != DBNull.Value && Convert.ToBoolean(reader["is_enabled"]),
                        UpdatedBy = (reader["updated_by"] as string ?? "").Trim(),
                        UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"]),
                        IsBuiltIn = builtInMap.ContainsKey(key)
                    });
                }
            }
        }

        foreach (BuiltInTextItem builtIn in GetBuiltInItems())
        {
            if (rows.Any(x => string.Equals(x.Key, builtIn.Key, StringComparison.OrdinalIgnoreCase)))
                continue;

            rows.Add(new TextContentItem
            {
                Key = builtIn.Key,
                Title = builtIn.Title,
                TextContent = builtIn.DefaultText,
                IsEnabled = true,
                UpdatedBy = "",
                UpdatedAt = null,
                IsBuiltIn = true
            });
        }

        return rows.OrderByDescending(x => x.IsBuiltIn).ThenBy(x => x.Key).ToList();
    }

    public static TextContentItem GetByKey(dbDataContext db, string key)
    {
        EnsureSchema(db);

        string contentKey = NormalizeKey(key);
        if (!IsValidKey(contentKey)) return null;

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 content_key, title, text_content, is_enabled, updated_by, updated_at
FROM dbo.Home_Text_Content_tb
WHERE content_key = @key";
            cmd.Parameters.AddWithValue("@key", contentKey);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read()) return null;
                return new TextContentItem
                {
                    Key = (reader["content_key"] as string ?? "").Trim(),
                    Title = (reader["title"] as string ?? "").Trim(),
                    TextContent = reader["text_content"] as string ?? "",
                    IsEnabled = reader["is_enabled"] != DBNull.Value && Convert.ToBoolean(reader["is_enabled"]),
                    UpdatedBy = (reader["updated_by"] as string ?? "").Trim(),
                    UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"]),
                    IsBuiltIn = GetBuiltInItems().Any(x => string.Equals(x.Key, contentKey, StringComparison.OrdinalIgnoreCase))
                };
            }
        }
    }

    public static TextContentItem GetEffectiveByKey(dbDataContext db, string key)
    {
        string contentKey = NormalizeKey(key);
        if (string.IsNullOrEmpty(contentKey))
            return null;

        string cacheKey = "home_text_content_effective:" + contentKey;
        TextContentItem cached = Helper_cl.RuntimeCacheGet<TextContentItem>(cacheKey);
        if (cached != null)
            return CloneTextContentItem(cached);

        BuiltInTextItem builtIn = GetBuiltInItems().FirstOrDefault(x => string.Equals(x.Key, contentKey, StringComparison.OrdinalIgnoreCase));
        TextContentItem dbRow = GetByKey(db, contentKey);

        if (dbRow == null)
        {
            TextContentItem fallback = new TextContentItem
            {
                Key = contentKey,
                Title = builtIn != null ? builtIn.Title : contentKey,
                TextContent = builtIn != null ? builtIn.DefaultText : "",
                IsEnabled = true,
                IsBuiltIn = builtIn != null
            };
            Helper_cl.RuntimeCacheGetOrAdd<TextContentItem>(cacheKey, EffectiveCacheSeconds, () => CloneTextContentItem(fallback));
            return fallback;
        }

        if (!dbRow.IsEnabled)
        {
            TextContentItem disabled = new TextContentItem
            {
                Key = dbRow.Key,
                Title = string.IsNullOrWhiteSpace(dbRow.Title) && builtIn != null ? builtIn.Title : dbRow.Title,
                TextContent = "",
                IsEnabled = false,
                UpdatedBy = dbRow.UpdatedBy,
                UpdatedAt = dbRow.UpdatedAt,
                IsBuiltIn = dbRow.IsBuiltIn
            };
            Helper_cl.RuntimeCacheGetOrAdd<TextContentItem>(cacheKey, EffectiveCacheSeconds, () => CloneTextContentItem(disabled));
            return disabled;
        }

        if (string.IsNullOrWhiteSpace(dbRow.TextContent) && builtIn != null)
            dbRow.TextContent = builtIn.DefaultText;

        if (string.IsNullOrWhiteSpace(dbRow.Title) && builtIn != null)
            dbRow.Title = builtIn.Title;

        Helper_cl.RuntimeCacheGetOrAdd<TextContentItem>(cacheKey, EffectiveCacheSeconds, () => CloneTextContentItem(dbRow));
        return dbRow;
    }

    public static void Upsert(dbDataContext db, string key, string title, string textContent, bool isEnabled, string updatedBy)
    {
        EnsureSchema(db);

        string contentKey = NormalizeKey(key);
        if (!IsValidKey(contentKey))
            throw new InvalidOperationException("Mã vị trí nội dung không hợp lệ.");

        string safeTitle = (title ?? "").Trim();
        if (safeTitle.Length > 250) safeTitle = safeTitle.Substring(0, 250);

        string safeText = (textContent ?? "").Trim();
        string safeUpdatedBy = (updatedBy ?? "").Trim();
        if (safeUpdatedBy.Length > 50) safeUpdatedBy = safeUpdatedBy.Substring(0, 50);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
MERGE dbo.Home_Text_Content_tb AS target
USING (SELECT @key AS content_key) AS source
ON target.content_key = source.content_key
WHEN MATCHED THEN
    UPDATE SET
        title = @title,
        text_content = @text_content,
        is_enabled = @is_enabled,
        updated_by = @updated_by,
        updated_at = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (content_key, title, text_content, is_enabled, updated_by, updated_at, created_at)
    VALUES (@key, @title, @text_content, @is_enabled, @updated_by, GETDATE(), GETDATE());";

            cmd.Parameters.AddWithValue("@key", contentKey);
            cmd.Parameters.AddWithValue("@title", (object)safeTitle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@text_content", (object)safeText ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_enabled", isEnabled);
            cmd.Parameters.AddWithValue("@updated_by", (object)safeUpdatedBy ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        Helper_cl.RuntimeCacheRemove("home_text_content_effective:" + contentKey);
    }

    private static TextContentItem CloneTextContentItem(TextContentItem item)
    {
        if (item == null)
            return null;

        return new TextContentItem
        {
            Key = item.Key,
            Title = item.Title,
            TextContent = item.TextContent,
            IsEnabled = item.IsEnabled,
            UpdatedBy = item.UpdatedBy,
            UpdatedAt = item.UpdatedAt,
            IsBuiltIn = item.IsBuiltIn
        };
    }

    public static List<KeywordLineItem> ParseKeywordLines(string rawText)
    {
        var list = new List<KeywordLineItem>();
        string text = (rawText ?? "").Replace("\r\n", "\n");
        string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string lineRaw in lines)
        {
            string line = (lineRaw ?? "").Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string label = line;
            string url = "#";
            int idx = line.IndexOf('|');
            if (idx >= 0)
            {
                label = line.Substring(0, idx).Trim();
                string right = line.Substring(idx + 1).Trim();
                if (!string.IsNullOrEmpty(right))
                {
                    if (IsSafePublicUrl(right))
                        url = right;
                }
            }

            if (string.IsNullOrWhiteSpace(label)) continue;

            list.Add(new KeywordLineItem
            {
                Label = label,
                Url = url
            });
        }

        return list;
    }

    public static string RenderPlainTextAsHtml(string plainText)
    {
        string raw = (plainText ?? "").Trim();
        if (string.IsNullOrEmpty(raw)) return "";

        string normalized = raw.Replace("\r\n", "\n");
        string[] blocks = normalized.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

        StringBuilder sb = new StringBuilder();
        foreach (string block in blocks)
        {
            string one = (block ?? "").Trim();
            if (string.IsNullOrEmpty(one)) continue;
            one = HttpUtility.HtmlEncode(one);
            one = one.Replace("\n", "<br/>");
            sb.Append("<p>");
            sb.Append(one);
            sb.Append("</p>");
        }

        return sb.ToString();
    }

    public static string GetGuideText(string key)
    {
        string normalized = NormalizeKey(key);
        if (normalized == KeyPopularKeywords)
            return "Mỗi dòng là 1 từ khóa. Có thể ghi: Tên từ khóa|/link-noi-bo-hoac-link-ngoai. Ví dụ: iPhone cũ|/tim-kiem?kw=iphone";

        if (normalized == KeyAboutLong)
            return "Nhập nội dung mô tả dài dạng văn bản. Mỗi đoạn cách nhau 1 dòng trống.";

        if (normalized == KeyFooterContactEmail)
            return "Nhập email hiển thị tại footer. Ví dụ: cskh@ahasale.vn";

        if (normalized == KeyFooterContactHotline)
            return "Nhập số CSKH hiển thị tại footer.";

        if (normalized == KeyFooterContactAddress)
            return "Nhập địa chỉ hiển thị tại footer.";

        if (normalized == KeyFooterLegalLine)
            return "Nhập dòng pháp lý cuối trang footer (văn bản thường).";

        if (normalized == KeyFooterPolicyUsageText)
            return "Nhập tên hiển thị của link chính sách cuối footer.";

        if (normalized == KeyFooterPolicyUsageUrl)
            return "Nhập link chính sách cuối footer. Chấp nhận /link-noi-bo hoặc https://...";

        if (normalized == KeyFooterSocialLinkedin || normalized == KeyFooterSocialYoutube || normalized == KeyFooterSocialFacebook)
            return "Nhập URL đầy đủ của mạng xã hội (https://...) hoặc để # nếu chưa dùng.";

        return "Nhập nội dung văn bản hiển thị trên trang chủ Home.";
    }

    public static string GetDefaultTitle(string key)
    {
        string normalized = NormalizeKey(key);
        BuiltInTextItem item = GetBuiltInItems().FirstOrDefault(x => string.Equals(x.Key, normalized, StringComparison.OrdinalIgnoreCase));
        if (item == null) return normalized;
        return item.Title;
    }

    private static bool IsSafePublicUrl(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string url = input.Trim();
        if (url.StartsWith("/", StringComparison.Ordinal)) return true;
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) return true;
        if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
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
IF OBJECT_ID('dbo.Home_Text_Content_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Home_Text_Content_tb
    (
        content_key NVARCHAR(100) NOT NULL PRIMARY KEY,
        title NVARCHAR(250) NULL,
        text_content NVARCHAR(MAX) NULL,
        is_enabled BIT NOT NULL CONSTRAINT DF_Home_Text_Content_tb_is_enabled DEFAULT(1),
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL,
        created_at DATETIME NOT NULL CONSTRAINT DF_Home_Text_Content_tb_created_at DEFAULT(GETDATE())
    );
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

            foreach (BuiltInTextItem builtIn in GetBuiltInItems())
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Home_Text_Content_tb WHERE content_key = @key)
BEGIN
    INSERT INTO dbo.Home_Text_Content_tb
    (
        content_key, title, text_content, is_enabled, updated_by, updated_at, created_at
    )
    VALUES
    (
        @key, @title, @text_content, 1, 'system_seed', GETDATE(), GETDATE()
    );
END;";

                cmd.Parameters.AddWithValue("@key", builtIn.Key);
                cmd.Parameters.AddWithValue("@title", builtIn.Title);
                cmd.Parameters.AddWithValue("@text_content", builtIn.DefaultText);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
