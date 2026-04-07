using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

public static class LinkedFeedStore_cl
{
    public sealed class LinkedSource
    {
        public int Id { get; set; }
        public string SourceKey { get; set; }
        public string SourceLabel { get; set; }
        public string ListingUrl { get; set; }
        public string DetailToken { get; set; }
        public string SourceType { get; set; }
        public string LegalMode { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }

    public sealed class LinkedPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Source { get; set; }
        public string SourceUrl { get; set; }
        public string Purpose { get; set; }
        public string PropertyType { get; set; }
        public string PriceText { get; set; }
        public string AreaText { get; set; }
        public string ThumbnailUrl { get; set; }
        public string GalleryCsv { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class FeedLog
    {
        public DateTime RanAt { get; set; }
        public string SourceKey { get; set; }
        public string SourceLabel { get; set; }
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Expired { get; set; }
        public bool IsSuccess { get; set; }
        public string StatusText { get; set; }
    }

    public static void EnsureSchema(dbDataContext db)
    {
        if (db == null) return;

        db.ExecuteCommand(@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='BDS_LinkedPosts_tb')
BEGIN
    CREATE TABLE dbo.BDS_LinkedPosts_tb
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(300) NOT NULL,
        Summary NVARCHAR(1000) NULL,
        Source NVARCHAR(50) NOT NULL,
        SourceUrl NVARCHAR(500) NOT NULL,
        Purpose NVARCHAR(10) NULL,
        PropertyType NVARCHAR(50) NULL,
        PriceText NVARCHAR(100) NULL,
        AreaText NVARCHAR(50) NULL,
        ThumbnailUrl NVARCHAR(500) NULL,
        GalleryCsv NVARCHAR(MAX) NULL,
        PublishedAt DATETIME NOT NULL,
        Province NVARCHAR(150) NULL,
        District NVARCHAR(150) NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT('active'),
        CreatedAt DATETIME NOT NULL,
        UpdatedAt DATETIME NOT NULL
    );
    CREATE UNIQUE INDEX IX_BDS_LinkedPosts_SourceUrl ON dbo.BDS_LinkedPosts_tb(SourceUrl);
END
");
        db.ExecuteCommand(@"
IF COL_LENGTH('dbo.BDS_LinkedPosts_tb', 'Purpose') IS NULL
    ALTER TABLE dbo.BDS_LinkedPosts_tb ADD Purpose NVARCHAR(10) NULL;
IF COL_LENGTH('dbo.BDS_LinkedPosts_tb', 'PropertyType') IS NULL
    ALTER TABLE dbo.BDS_LinkedPosts_tb ADD PropertyType NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.BDS_LinkedPosts_tb', 'PriceText') IS NULL
    ALTER TABLE dbo.BDS_LinkedPosts_tb ADD PriceText NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.BDS_LinkedPosts_tb', 'AreaText') IS NULL
    ALTER TABLE dbo.BDS_LinkedPosts_tb ADD AreaText NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.BDS_LinkedPosts_tb', 'ThumbnailUrl') IS NULL
    ALTER TABLE dbo.BDS_LinkedPosts_tb ADD ThumbnailUrl NVARCHAR(500) NULL;
IF COL_LENGTH('dbo.BDS_LinkedPosts_tb', 'GalleryCsv') IS NULL
    ALTER TABLE dbo.BDS_LinkedPosts_tb ADD GalleryCsv NVARCHAR(MAX) NULL;
");

        db.ExecuteCommand(@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='BDS_LinkedFeedLogs_tb')
BEGIN
    CREATE TABLE dbo.BDS_LinkedFeedLogs_tb
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        RanAt DATETIME NOT NULL,
        SourceKey NVARCHAR(50) NOT NULL,
        SourceLabel NVARCHAR(150) NOT NULL,
        Created INT NOT NULL,
        Updated INT NOT NULL,
        Expired INT NOT NULL,
        IsSuccess BIT NOT NULL,
        StatusText NVARCHAR(500) NULL
    );
END
");

        db.ExecuteCommand(@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='BDS_LinkedSources_tb')
BEGIN
    CREATE TABLE dbo.BDS_LinkedSources_tb
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SourceKey NVARCHAR(80) NOT NULL,
        SourceLabel NVARCHAR(200) NOT NULL,
        ListingUrl NVARCHAR(700) NOT NULL,
        DetailToken NVARCHAR(120) NULL,
        SourceType NVARCHAR(40) NOT NULL DEFAULT('html'),
        LegalMode NVARCHAR(40) NOT NULL DEFAULT('robots-only'),
        IsActive BIT NOT NULL DEFAULT(1),
        SortOrder INT NOT NULL DEFAULT(100),
        CreatedAt DATETIME NOT NULL DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL DEFAULT(GETDATE())
    );
    CREATE UNIQUE INDEX IX_BDS_LinkedSources_SourceKey ON dbo.BDS_LinkedSources_tb(SourceKey);
END
");
        SeedDefaultSources(db);
    }

    public sealed class UpsertItem
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Source { get; set; }
        public string SourceUrl { get; set; }
        public string Purpose { get; set; }
        public string PropertyType { get; set; }
        public string PriceText { get; set; }
        public string AreaText { get; set; }
        public string ThumbnailUrl { get; set; }
        public string GalleryCsv { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }

    public static void UpsertPosts(dbDataContext db, IEnumerable<UpsertItem> items, out int created, out int updated)
    {
        created = 0;
        updated = 0;
        if (db == null || items == null) return;

        var sanitized = items
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Title) && !string.IsNullOrWhiteSpace(x.SourceUrl))
            .ToList();

        foreach (var item in sanitized)
        {
            DateTime now = AhaTime_cl.Now;
            string sourceUrl = FitLen((item.SourceUrl ?? "").Trim(), 500);
            string title = FitLen((item.Title ?? "").Trim(), 300);
            string summary = NormalizeForSummary(FitLen(item.Summary ?? "", 1000));
            string source = FitLen(NormalizeSourceKey(item.Source), 50);
            string purpose = FitLen((item.Purpose ?? "").Trim(), 10);
            string propertyType = FitLen((item.PropertyType ?? "").Trim(), 50);
            string priceText = FitLen((item.PriceText ?? "").Trim(), 100);
            string areaText = FitLen((item.AreaText ?? "").Trim(), 50);
            string thumbnailUrl = FitLen((item.ThumbnailUrl ?? "").Trim(), 500);
            string galleryCsv = item.GalleryCsv ?? "";
            string province = FitLen(NormalizeProvinceName(item.Province), 150);
            string district = FitLen(NormalizeDistrictName(item.District), 150);

            bool exists = db.ExecuteQuery<int>("SELECT COUNT(1) FROM dbo.BDS_LinkedPosts_tb WHERE SourceUrl = {0}", sourceUrl).FirstOrDefault() > 0;
            db.ExecuteCommand(@"
MERGE dbo.BDS_LinkedPosts_tb AS target
USING (SELECT {0} AS SourceUrl) AS src
    ON target.SourceUrl = src.SourceUrl
WHEN MATCHED THEN
    UPDATE SET
        Title = {1},
        Summary = {2},
        Source = {3},
        Purpose = {4},
        PropertyType = {5},
        PriceText = {6},
        AreaText = {7},
        ThumbnailUrl = {8},
        GalleryCsv = {9},
        PublishedAt = {10},
        Province = {11},
        District = {12},
        Status = 'active',
        UpdatedAt = {13}
WHEN NOT MATCHED THEN
    INSERT (Title, Summary, Source, SourceUrl, Purpose, PropertyType, PriceText, AreaText, ThumbnailUrl, GalleryCsv, PublishedAt, Province, District, Status, CreatedAt, UpdatedAt)
    VALUES ({1}, {2}, {3}, {0}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, 'active', {13}, {13});
", sourceUrl, title, summary, source, purpose, propertyType, priceText, areaText, thumbnailUrl, galleryCsv, item.PublishedAt, province, district, now);

            if (exists) updated++;
            else created++;
        }
    }

    private static string FitLen(string value, int maxLen)
    {
        string v = (value ?? "").Trim();
        if (maxLen <= 0 || v.Length <= maxLen)
            return v;
        return v.Substring(0, maxLen);
    }

    private static string NormalizeForSummary(string value)
    {
        string v = value ?? "";
        v = System.Text.RegularExpressions.Regex.Replace(v, @"\s+", " ").Trim();
        v = v.Replace(" .", ".").Replace(" ,", ",").Replace(" ;", ";").Replace(" :", ":");
        return v;
    }

    private static string NormalizeSourceKey(string value)
    {
        return (value ?? "").Trim().ToLowerInvariant();
    }

    private static string NormalizeProvinceName(string value)
    {
        string normalized = BatDongSanService_cl.Slugify(value ?? "").Replace("-", " ").Trim();
        if (normalized == "")
            return "";
        if (normalized == "dong nai")
            return "Đồng Nai";
        if (normalized == "nghe an")
            return "Nghệ An";
        if (normalized == "ha noi")
            return "Hà Nội";
        if (normalized == "da nang")
            return "Đà Nẵng";
        if (normalized == "tp hcm" || normalized == "tphcm" || normalized == "ho chi minh" || normalized == "sai gon" || normalized == "hcm")
            return "TP.HCM";
        return (value ?? "").Trim();
    }

    private static string NormalizeDistrictName(string value)
    {
        string normalized = BatDongSanService_cl.Slugify(value ?? "").Replace("-", " ").Trim();
        if (normalized == "")
            return "";
        switch (normalized)
        {
            case "bien hoa":
                return "Biên Hòa";
            case "long thanh":
                return "Long Thành";
            case "nhon trach":
                return "Nhơn Trạch";
            case "trang bom":
                return "Trảng Bom";
            case "vinh cuu":
                return "Vĩnh Cửu";
            case "cam my":
                return "Cẩm Mỹ";
            case "xuan loc":
                return "Xuân Lộc";
            case "dinh quan":
                return "Định Quán";
            case "tan phu":
                return "Tân Phú";
            case "thong nhat":
                return "Thống Nhất";
            case "long khanh":
                return "Long Khánh";
            default:
                return (value ?? "").Trim();
        }
    }

    public static List<LinkedPost> GetLatest(dbDataContext db, int take)
    {
        EnsureSchema(db);
        int safeTake = take <= 0 ? 20 : (take > 5000 ? 5000 : take);
        string sql = @"
SELECT TOP " + safeTake.ToString() + @"
    Id, Title, Summary, Source, SourceUrl, Purpose, PropertyType, PriceText, AreaText, ThumbnailUrl, GalleryCsv, PublishedAt, Province, District, CreatedAt
FROM dbo.BDS_LinkedPosts_tb
WHERE Status = 'active'
  AND ISNULL(LTRIM(RTRIM(Province)), '') <> ''
  AND (
        (Source = 'raovat-net' AND SourceUrl LIKE '%/xem/%')
        OR (Source = 'alonhadat' AND SourceUrl LIKE '%.html%')
        OR (Source = 'rongbay' AND SourceUrl LIKE '%.html%')
        OR (Source NOT IN ('raovat-net','alonhadat','rongbay'))
      )
ORDER BY PublishedAt DESC, Id DESC";
        return db.ExecuteQuery<LinkedPost>(sql).ToList();
    }

    public static List<LinkedPost> GetActiveForSearch(dbDataContext db, int take)
    {
        EnsureSchema(db);
        int safeTake = take <= 0 ? 500 : (take > 5000 ? 5000 : take);
        string sql = @"
SELECT TOP " + safeTake.ToString() + @"
    Id, Title, Summary, Source, SourceUrl, Purpose, PropertyType, PriceText, AreaText, ThumbnailUrl, GalleryCsv, PublishedAt, Province, District, CreatedAt
FROM dbo.BDS_LinkedPosts_tb
WHERE Status = 'active'
  AND ISNULL(LTRIM(RTRIM(Province)), '') <> ''
  AND (
        (Source = 'raovat-net' AND SourceUrl LIKE '%/xem/%')
        OR (Source = 'alonhadat' AND SourceUrl LIKE '%.html%')
        OR (Source = 'rongbay' AND SourceUrl LIKE '%.html%')
        OR (Source NOT IN ('raovat-net','alonhadat','rongbay'))
      )
ORDER BY PublishedAt DESC, Id DESC";
        return db.ExecuteQuery<LinkedPost>(sql).ToList();
    }

    public static int GetActiveCount(dbDataContext db)
    {
        EnsureSchema(db);
        return db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.BDS_LinkedPosts_tb
WHERE Status = 'active'
  AND ISNULL(LTRIM(RTRIM(Province)), '') <> ''
  AND (
        (Source = 'raovat-net' AND SourceUrl LIKE '%/xem/%')
        OR (Source = 'alonhadat' AND SourceUrl LIKE '%.html%')
        OR (Source = 'rongbay' AND SourceUrl LIKE '%.html%')
        OR (Source NOT IN ('raovat-net','alonhadat','rongbay'))
      )").FirstOrDefault();
    }

    public static List<LinkedPost> GetPagedActive(dbDataContext db, int page, int pageSize)
    {
        EnsureSchema(db);
        int safePage = page <= 0 ? 1 : page;
        int safeSize = pageSize <= 0 ? 12 : (pageSize > 60 ? 60 : pageSize);
        int start = (safePage - 1) * safeSize + 1;
        int finish = safePage * safeSize;

        string sql = @"
WITH src AS (
    SELECT
        Id, Title, Summary, Source, SourceUrl, Purpose, PropertyType, PriceText, AreaText, ThumbnailUrl, GalleryCsv, PublishedAt, Province, District, CreatedAt,
        ROW_NUMBER() OVER (ORDER BY PublishedAt DESC, Id DESC) AS rn
    FROM dbo.BDS_LinkedPosts_tb
    WHERE Status = 'active'
      AND ISNULL(LTRIM(RTRIM(Province)), '') <> ''
      AND (
            (Source = 'raovat-net' AND SourceUrl LIKE '%/xem/%')
            OR (Source = 'alonhadat' AND SourceUrl LIKE '%.html%')
            OR (Source = 'rongbay' AND SourceUrl LIKE '%.html%')
            OR (Source NOT IN ('raovat-net','alonhadat','rongbay'))
          )
)
SELECT Id, Title, Summary, Source, SourceUrl, Purpose, PropertyType, PriceText, AreaText, ThumbnailUrl, GalleryCsv, PublishedAt, Province, District, CreatedAt
FROM src
WHERE rn BETWEEN " + start.ToString() + " AND " + finish.ToString() + @"
ORDER BY rn";

        return db.ExecuteQuery<LinkedPost>(sql).ToList();
    }

    public static LinkedPost GetById(dbDataContext db, int id)
    {
        EnsureSchema(db);
        return db.ExecuteQuery<LinkedPost>(@"
SELECT TOP 1 Id, Title, Summary, Source, SourceUrl, PublishedAt, Province, District, CreatedAt
    , Purpose, PropertyType, PriceText, AreaText, ThumbnailUrl, GalleryCsv
FROM dbo.BDS_LinkedPosts_tb
WHERE Id = {0}
  AND ISNULL(LTRIM(RTRIM(Province)), '') <> ''
  AND (
        (Source = 'raovat-net' AND SourceUrl LIKE '%/xem/%')
        OR (Source = 'alonhadat' AND SourceUrl LIKE '%.html%')
        OR (Source = 'rongbay' AND SourceUrl LIKE '%.html%')
        OR (Source NOT IN ('raovat-net','alonhadat','rongbay'))
      )", id).FirstOrDefault();
    }

    public static int DeactivateMissingLocationPosts(dbDataContext db)
    {
        EnsureSchema(db);
        return db.ExecuteCommand(@"
UPDATE dbo.BDS_LinkedPosts_tb
SET Status = 'inactive', UpdatedAt = GETDATE()
WHERE Status = 'active'
  AND ISNULL(LTRIM(RTRIM(Province)), '') = ''
");
    }

    public static void SaveLog(LinkedFeedSync_cl.FeedLog log)
    {
        if (log == null) return;
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                EnsureSchema(db);
                db.ExecuteCommand(@"
INSERT INTO dbo.BDS_LinkedFeedLogs_tb (RanAt, SourceKey, SourceLabel, Created, Updated, Expired, IsSuccess, StatusText)
VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
                    log.RanAt,
                    (log.SourceKey ?? ""),
                    (log.SourceLabel ?? ""),
                    log.Created,
                    log.Updated,
                    log.Expired,
                    log.IsSuccess,
                    (log.StatusText ?? ""));
            }
        }
        catch { }
    }

    public static List<LinkedFeedSync_cl.FeedLog> GetRecentLogs(dbDataContext db, int take)
    {
        EnsureSchema(db);
        int safeTake = take <= 0 ? 20 : (take > 200 ? 200 : take);
        string sql = @"
SELECT TOP " + safeTake.ToString() + @" RanAt, SourceKey, SourceLabel, Created, Updated, Expired, IsSuccess,
    ISNULL(StatusText, '') AS StatusText
FROM dbo.BDS_LinkedFeedLogs_tb
ORDER BY RanAt DESC, Id DESC";
        return db.ExecuteQuery<LinkedFeedSync_cl.FeedLog>(sql).ToList();
    }

    public static List<LinkedFeedSync_cl.FeedLog> TryGetRecentLogsFast(dbDataContext db, int take)
    {
        if (db == null)
            return new List<LinkedFeedSync_cl.FeedLog>();

        try
        {
            int safeTake = take <= 0 ? 20 : (take > 200 ? 200 : take);
            string sql = @"
SELECT TOP " + safeTake.ToString() + @" RanAt, SourceKey, SourceLabel, Created, Updated, Expired, IsSuccess,
    ISNULL(StatusText, '') AS StatusText
FROM dbo.BDS_LinkedFeedLogs_tb
ORDER BY RanAt DESC, Id DESC";
            return db.ExecuteQuery<LinkedFeedSync_cl.FeedLog>(sql).ToList();
        }
        catch
        {
            return new List<LinkedFeedSync_cl.FeedLog>();
        }
    }

    public static int ExpireOldPosts(dbDataContext db, int ttlDays)
    {
        EnsureSchema(db);
        int days = ttlDays <= 0 ? 14 : (ttlDays > 180 ? 180 : ttlDays);
        DateTime threshold = AhaTime_cl.Now.AddDays(-days);
        return db.ExecuteCommand(@"
UPDATE dbo.BDS_LinkedPosts_tb
SET Status = 'expired', UpdatedAt = GETDATE()
WHERE Status = 'active'
  AND PublishedAt < {0}", threshold);
    }

    public static int ResolveLinkedTtlDays()
    {
        try
        {
            string raw = ConfigurationManager.AppSettings["BDS.LinkedPostTtlDays"];
            int n;
            if (int.TryParse((raw ?? "").Trim(), out n))
            {
                if (n < 3) return 3;
                if (n > 180) return 180;
                return n;
            }
        }
        catch { }
        return 14;
    }

    public static List<LinkedSource> GetActiveSources(dbDataContext db)
    {
        EnsureSchema(db);
        return db.ExecuteQuery<LinkedSource>(@"
SELECT Id, SourceKey, SourceLabel, ListingUrl, DetailToken, SourceType, LegalMode, IsActive, SortOrder
FROM dbo.BDS_LinkedSources_tb
WHERE IsActive = 1
ORDER BY SortOrder ASC, Id ASC").ToList();
    }

    public static List<LinkedSource> TryGetActiveSourcesFast(dbDataContext db)
    {
        if (db == null)
            return new List<LinkedSource>();

        try
        {
            return db.ExecuteQuery<LinkedSource>(@"
SELECT Id, SourceKey, SourceLabel, ListingUrl, DetailToken, SourceType, LegalMode, IsActive, SortOrder
FROM dbo.BDS_LinkedSources_tb
WHERE IsActive = 1
ORDER BY SortOrder ASC, Id ASC").ToList();
        }
        catch
        {
            return new List<LinkedSource>();
        }
    }

    public static List<LinkedSource> GetAllSources(dbDataContext db)
    {
        EnsureSchema(db);
        return db.ExecuteQuery<LinkedSource>(@"
SELECT Id, SourceKey, SourceLabel, ListingUrl, DetailToken, SourceType, LegalMode, IsActive, SortOrder
FROM dbo.BDS_LinkedSources_tb
ORDER BY SortOrder ASC, Id ASC").ToList();
    }

    public static void UpsertSource(dbDataContext db, string key, string label, string listingUrl, string detailToken, int sortOrder, bool isActive, string legalMode, string sourceType)
    {
        if (db == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(listingUrl))
            return;

        db.ExecuteCommand(@"
IF EXISTS (SELECT 1 FROM dbo.BDS_LinkedSources_tb WHERE SourceKey = {0})
BEGIN
    UPDATE dbo.BDS_LinkedSources_tb
    SET
        SourceLabel = {1},
        ListingUrl = {2},
        DetailToken = {3},
        SourceType = {7},
        LegalMode = {4},
        IsActive = {5},
        SortOrder = {6},
        UpdatedAt = GETDATE()
    WHERE SourceKey = {0};
END
ELSE
BEGIN
    INSERT INTO dbo.BDS_LinkedSources_tb (SourceKey, SourceLabel, ListingUrl, DetailToken, SourceType, LegalMode, IsActive, SortOrder, CreatedAt, UpdatedAt)
    VALUES ({0}, {1}, {2}, {3}, {7}, {4}, {5}, {6}, GETDATE(), GETDATE());
END
", key.Trim().ToLowerInvariant(), label ?? key, listingUrl ?? "", detailToken ?? "", legalMode ?? "robots-only", isActive, sortOrder, string.IsNullOrWhiteSpace(sourceType) ? "html" : sourceType.Trim().ToLowerInvariant());
    }

    private static void SeedDefaultSources(dbDataContext db)
    {
        if (db == null) return;

        UpsertSource(db, "alonhadat", "Alonhadat", "https://alonhadat.com.vn/nha-dat/can-ban/nha-dat-1.html", ".html", 10);
        UpsertSource(db, "rongbay", "Rongbay", "https://rongbay.com/nha-dat.html", ".html", 20);
        UpsertSource(db, "raovat-net", "Raovat.net", "https://raovat.net/", "/xem/", 30);
    }

    private static void UpsertSource(dbDataContext db, string key, string label, string listingUrl, string detailToken, int sortOrder)
    {
        if (db == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(listingUrl))
            return;

        db.ExecuteCommand(@"
IF EXISTS (SELECT 1 FROM dbo.BDS_LinkedSources_tb WHERE SourceKey = {0})
BEGIN
    UPDATE dbo.BDS_LinkedSources_tb
    SET
        SourceLabel = CASE WHEN ISNULL(SourceLabel,'') = '' THEN {1} ELSE SourceLabel END,
        ListingUrl = CASE WHEN ISNULL(ListingUrl,'') = '' THEN {2} ELSE ListingUrl END,
        DetailToken = CASE WHEN ISNULL(DetailToken,'') = '' THEN {3} ELSE DetailToken END,
        SortOrder = CASE WHEN SortOrder <= 0 THEN {4} ELSE SortOrder END,
        UpdatedAt = GETDATE()
    WHERE SourceKey = {0};
END
ELSE
BEGIN
    INSERT INTO dbo.BDS_LinkedSources_tb (SourceKey, SourceLabel, ListingUrl, DetailToken, SourceType, LegalMode, IsActive, SortOrder, CreatedAt, UpdatedAt)
    VALUES ({0}, {1}, {2}, {3}, 'html', 'robots-only', 1, {4}, GETDATE(), GETDATE());
END
", key.Trim().ToLowerInvariant(), label ?? key, listingUrl ?? "", detailToken ?? "", sortOrder);
    }
}
