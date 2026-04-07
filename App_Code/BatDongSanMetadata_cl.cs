using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class BatDongSanMetadata_cl
{
    public sealed class PostMetadata
    {
        public long PostId { get; set; }
        public string ListingPurpose { get; set; }
        public string PropertyType { get; set; }
        public decimal AreaValue { get; set; }
        public decimal DepositAmount { get; set; }
        public int RentalTermMonths { get; set; }
        public int FloorCount { get; set; }
        public decimal LandWidth { get; set; }
        public decimal LandLength { get; set; }
        public string HouseDirection { get; set; }
        public string LegalStatus { get; set; }
        public string FurnishingStatus { get; set; }
        public int BedroomCount { get; set; }
        public int BathroomCount { get; set; }
        public string ProjectName { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string AddressLine { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public sealed class PublicListingRow
    {
        public long PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostSlug { get; set; }
        public string PostDescription { get; set; }
        public string PostImage { get; set; }
        public string PostOwner { get; set; }
        public DateTime? PostCreatedAt { get; set; }
        public decimal? PostPrice { get; set; }
        public string ListingPurpose { get; set; }
        public string PropertyType { get; set; }
        public decimal? AreaValue { get; set; }
        public decimal? DepositAmount { get; set; }
        public int? RentalTermMonths { get; set; }
        public int? FloorCount { get; set; }
        public decimal? LandWidth { get; set; }
        public decimal? LandLength { get; set; }
        public string HouseDirection { get; set; }
        public string LegalStatus { get; set; }
        public string FurnishingStatus { get; set; }
        public int? BedroomCount { get; set; }
        public int? BathroomCount { get; set; }
        public string ProjectName { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string AddressLine { get; set; }
    }

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null)
            return;

        db.ExecuteCommand(@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'BDS_PostMetadata_tb')
BEGIN
    CREATE TABLE dbo.BDS_PostMetadata_tb
    (
        PostId BIGINT NOT NULL PRIMARY KEY,
        ListingPurpose NVARCHAR(20) NOT NULL,
        PropertyType NVARCHAR(50) NOT NULL,
        AreaValue DECIMAL(18,2) NOT NULL CONSTRAINT DF_BDS_PostMetadata_AreaValue DEFAULT(0),
        DepositAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_BDS_PostMetadata_DepositAmount DEFAULT(0),
        RentalTermMonths INT NOT NULL CONSTRAINT DF_BDS_PostMetadata_RentalTermMonths DEFAULT(0),
        FloorCount INT NOT NULL CONSTRAINT DF_BDS_PostMetadata_FloorCount DEFAULT(0),
        LandWidth DECIMAL(18,2) NOT NULL CONSTRAINT DF_BDS_PostMetadata_LandWidth DEFAULT(0),
        LandLength DECIMAL(18,2) NOT NULL CONSTRAINT DF_BDS_PostMetadata_LandLength DEFAULT(0),
        HouseDirection NVARCHAR(50) NULL,
        LegalStatus NVARCHAR(150) NULL,
        FurnishingStatus NVARCHAR(150) NULL,
        BedroomCount INT NOT NULL CONSTRAINT DF_BDS_PostMetadata_BedroomCount DEFAULT(0),
        BathroomCount INT NOT NULL CONSTRAINT DF_BDS_PostMetadata_BathroomCount DEFAULT(0),
        ProjectName NVARCHAR(250) NULL,
        ProvinceName NVARCHAR(150) NULL,
        DistrictName NVARCHAR(150) NULL,
        WardName NVARCHAR(150) NULL,
        AddressLine NVARCHAR(500) NULL,
        CreatedAt DATETIME NULL,
        UpdatedAt DATETIME NULL
    );
END
");
        db.ExecuteCommand(@"
IF COL_LENGTH('dbo.BDS_PostMetadata_tb', 'DepositAmount') IS NULL
    ALTER TABLE dbo.BDS_PostMetadata_tb ADD DepositAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_BDS_PostMetadata_DepositAmount_Alter DEFAULT(0);
IF COL_LENGTH('dbo.BDS_PostMetadata_tb', 'RentalTermMonths') IS NULL
    ALTER TABLE dbo.BDS_PostMetadata_tb ADD RentalTermMonths INT NOT NULL CONSTRAINT DF_BDS_PostMetadata_RentalTermMonths_Alter DEFAULT(0);
IF COL_LENGTH('dbo.BDS_PostMetadata_tb', 'FloorCount') IS NULL
    ALTER TABLE dbo.BDS_PostMetadata_tb ADD FloorCount INT NOT NULL CONSTRAINT DF_BDS_PostMetadata_FloorCount_Alter DEFAULT(0);
IF COL_LENGTH('dbo.BDS_PostMetadata_tb', 'LandWidth') IS NULL
    ALTER TABLE dbo.BDS_PostMetadata_tb ADD LandWidth DECIMAL(18,2) NOT NULL CONSTRAINT DF_BDS_PostMetadata_LandWidth_Alter DEFAULT(0);
IF COL_LENGTH('dbo.BDS_PostMetadata_tb', 'LandLength') IS NULL
    ALTER TABLE dbo.BDS_PostMetadata_tb ADD LandLength DECIMAL(18,2) NOT NULL CONSTRAINT DF_BDS_PostMetadata_LandLength_Alter DEFAULT(0);
IF COL_LENGTH('dbo.BDS_PostMetadata_tb', 'HouseDirection') IS NULL
    ALTER TABLE dbo.BDS_PostMetadata_tb ADD HouseDirection NVARCHAR(50) NULL;
");
    }

    public static bool IsRealEstateCategory(dbDataContext db, string categoryId)
    {
        if (db == null)
            return false;

        string id = (categoryId ?? "").Trim();
        if (id == "")
            return false;

        var category = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == id);
        return IsRealEstateCategory(category == null ? "" : category.name, category == null ? "" : category.name_en);
    }

    public static string FindRealEstateCategoryId(dbDataContext db)
    {
        if (db == null)
            return "";

        var categories = db.DanhMuc_tbs
            .Where(p => p.bin == false && p.kyhieu_danhmuc == "web")
            .OrderBy(p => p.rank)
            .ToList();

        var matched = categories.FirstOrDefault(p => IsRealEstateCategory(p.name, p.name_en));
        return matched == null ? "" : matched.id.ToString();
    }

    public static string GetRealEstateCategoryIdsCsv(dbDataContext db)
    {
        if (db == null)
            return "";

        var ids = db.DanhMuc_tbs
            .Where(p => p.bin == false && p.kyhieu_danhmuc == "web")
            .ToList()
            .Where(p => IsRealEstateCategory(p.name, p.name_en))
            .Select(p => p.id.ToString())
            .Distinct()
            .ToArray();

        return string.Join(",", ids);
    }

    public static void Upsert(dbDataContext db, PostMetadata metadata)
    {
        if (db == null || metadata == null || metadata.PostId <= 0)
            return;

        EnsureSchemaSafe(db);

        string purpose = NormalizePurpose(metadata.ListingPurpose);
        string propertyType = NormalizePropertyType(metadata.PropertyType);
        decimal areaValue = metadata.AreaValue < 0 ? 0 : metadata.AreaValue;
        decimal depositAmount = metadata.DepositAmount < 0 ? 0 : metadata.DepositAmount;
        int rentalTermMonths = metadata.RentalTermMonths < 0 ? 0 : metadata.RentalTermMonths;
        int floorCount = metadata.FloorCount < 0 ? 0 : metadata.FloorCount;
        decimal landWidth = metadata.LandWidth < 0 ? 0 : metadata.LandWidth;
        decimal landLength = metadata.LandLength < 0 ? 0 : metadata.LandLength;
        string houseDirection = NullIfBlank(metadata.HouseDirection) as string;
        int bedroomCount = metadata.BedroomCount < 0 ? 0 : metadata.BedroomCount;
        int bathroomCount = metadata.BathroomCount < 0 ? 0 : metadata.BathroomCount;
        DateTime now = AhaTime_cl.Now;

        db.ExecuteCommand(@"
MERGE dbo.BDS_PostMetadata_tb AS target
USING (
        SELECT
        {0} AS PostId,
        {1} AS ListingPurpose,
        {2} AS PropertyType,
        {3} AS AreaValue,
        {4} AS DepositAmount,
        {5} AS RentalTermMonths,
        {6} AS FloorCount,
        {7} AS LandWidth,
        {8} AS LandLength,
        {9} AS HouseDirection,
        {10} AS LegalStatus,
        {11} AS FurnishingStatus,
        {12} AS BedroomCount,
        {13} AS BathroomCount,
        {14} AS ProjectName,
        {15} AS ProvinceName,
        {16} AS DistrictName,
        {17} AS WardName,
        {18} AS AddressLine,
        {19} AS StampNow
) AS source
ON target.PostId = source.PostId
WHEN MATCHED THEN
    UPDATE SET
        ListingPurpose = source.ListingPurpose,
        PropertyType = source.PropertyType,
        AreaValue = source.AreaValue,
        DepositAmount = source.DepositAmount,
        RentalTermMonths = source.RentalTermMonths,
        FloorCount = source.FloorCount,
        LandWidth = source.LandWidth,
        LandLength = source.LandLength,
        HouseDirection = source.HouseDirection,
        LegalStatus = source.LegalStatus,
        FurnishingStatus = source.FurnishingStatus,
        BedroomCount = source.BedroomCount,
        BathroomCount = source.BathroomCount,
        ProjectName = source.ProjectName,
        ProvinceName = source.ProvinceName,
        DistrictName = source.DistrictName,
        WardName = source.WardName,
        AddressLine = source.AddressLine,
        UpdatedAt = source.StampNow
WHEN NOT MATCHED THEN
    INSERT (PostId, ListingPurpose, PropertyType, AreaValue, DepositAmount, RentalTermMonths, FloorCount, LandWidth, LandLength, HouseDirection, LegalStatus, FurnishingStatus, BedroomCount, BathroomCount, ProjectName, ProvinceName, DistrictName, WardName, AddressLine, CreatedAt, UpdatedAt)
    VALUES (source.PostId, source.ListingPurpose, source.PropertyType, source.AreaValue, source.DepositAmount, source.RentalTermMonths, source.FloorCount, source.LandWidth, source.LandLength, source.HouseDirection, source.LegalStatus, source.FurnishingStatus, source.BedroomCount, source.BathroomCount, source.ProjectName, source.ProvinceName, source.DistrictName, source.WardName, source.AddressLine, source.StampNow, source.StampNow);",
            metadata.PostId,
            purpose,
            propertyType,
            areaValue,
            depositAmount,
            rentalTermMonths,
            floorCount,
            landWidth,
            landLength,
            houseDirection,
            NullIfBlank(metadata.LegalStatus),
            NullIfBlank(metadata.FurnishingStatus),
            bedroomCount,
            bathroomCount,
            NullIfBlank(metadata.ProjectName),
            NullIfBlank(metadata.ProvinceName),
            NullIfBlank(metadata.DistrictName),
            NullIfBlank(metadata.WardName),
            NullIfBlank(metadata.AddressLine),
            now);
    }

    public static PostMetadata GetByPostId(dbDataContext db, long postId)
    {
        if (db == null || postId <= 0)
            return null;

        EnsureSchemaSafe(db);

        return db.ExecuteQuery<PostMetadata>(@"
SELECT TOP 1
    PostId,
    ListingPurpose,
    PropertyType,
    AreaValue,
        DepositAmount,
        RentalTermMonths,
        FloorCount,
        LandWidth,
        LandLength,
        HouseDirection,
        LegalStatus,
        FurnishingStatus,
        BedroomCount,
        BathroomCount,
    ProjectName,
    ProvinceName,
    DistrictName,
    WardName,
    AddressLine,
    UpdatedAt
FROM dbo.BDS_PostMetadata_tb
WHERE PostId = {0}", postId).FirstOrDefault();
    }

    public static List<PublicListingRow> LoadPublicListings()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                EnsureSchemaSafe(db);
                return db.ExecuteQuery<PublicListingRow>(@"
SELECT
    post.id AS PostId,
    post.name AS PostTitle,
    post.name_en AS PostSlug,
    post.description AS PostDescription,
    post.image AS PostImage,
    post.nguoitao AS PostOwner,
    post.ngaytao AS PostCreatedAt,
    post.giaban AS PostPrice,
    meta.ListingPurpose,
    meta.PropertyType,
    meta.AreaValue,
    meta.DepositAmount,
    meta.RentalTermMonths,
    meta.FloorCount,
    meta.LandWidth,
    meta.LandLength,
    meta.HouseDirection,
    meta.LegalStatus,
    meta.FurnishingStatus,
    meta.BedroomCount,
    meta.BathroomCount,
    meta.ProjectName,
    meta.ProvinceName,
    meta.DistrictName,
    meta.WardName,
    meta.AddressLine
FROM dbo.BDS_PostMetadata_tb meta
INNER JOIN dbo.BaiViet_tb post ON post.id = meta.PostId
WHERE ISNULL(post.bin, 0) = 0
ORDER BY post.id DESC").ToList();
            }
        }
        catch
        {
            return new List<PublicListingRow>();
        }
    }

    public static string NormalizePurpose(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        return value == "rent" ? "rent" : "sale";
    }

    public static string NormalizePropertyType(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        switch (value)
        {
            case "apartment":
            case "house":
            case "land":
            case "townhouse":
            case "office":
            case "shophouse":
            case "warehouse":
            case "boarding-house":
            case "motel-room":
            case "business-premises":
                return value;
            default:
                return "apartment";
        }
    }

    public static decimal ParseArea(string raw)
    {
        string value = (raw ?? "").Trim().Replace(",", "");
        decimal result;
        if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            return 0;
        return result < 0 ? 0 : result;
    }

    public static decimal ParseDecimal(string raw)
    {
        string value = (raw ?? "").Trim().Replace(",", "");
        decimal result;
        if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            return 0;
        return result < 0 ? 0 : result;
    }

    public static bool IsRealEstateCategory(string name, string nameEn)
    {
        string normalizedName = BatDongSanService_cl.Slugify(name);
        string normalizedNameEn = BatDongSanService_cl.Slugify(nameEn);
        return normalizedName.Contains("bat-dong-san")
            || normalizedName.Contains("nha-dat")
            || normalizedNameEn.Contains("bat-dong-san")
            || normalizedNameEn.Contains("nha-dat");
    }

    private static object NullIfBlank(string value)
    {
        string normalized = (value ?? "").Trim();
        return normalized == "" ? null : (object)normalized;
    }
}
