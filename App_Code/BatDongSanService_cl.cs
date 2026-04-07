using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public static class BatDongSanService_cl
{
    public const string DefaultBdsFallbackImage = "/uploads/images/bat-dong-san/bds-placeholder.jpg";

    public sealed class SummaryInfo
    {
        public int TotalListings { get; set; }
        public int SaleCount { get; set; }
        public int RentCount { get; set; }
        public int ProjectCount { get; set; }
    }

    public sealed class FilterOption
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Note { get; set; }
    }

    public sealed class ListingItem
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Purpose { get; set; }
        public string PropertyType { get; set; }
        public string PropertyTypeLabel { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string AddressText { get; set; }
        public decimal PriceValue { get; set; }
        public string PriceText { get; set; }
        public decimal AreaValue { get; set; }
        public decimal DepositAmount { get; set; }
        public int RentalTermMonths { get; set; }
        public int FloorCount { get; set; }
        public decimal LandWidth { get; set; }
        public decimal LandLength { get; set; }
        public string LegalStatus { get; set; }
        public string FurnishingStatus { get; set; }
        public int BedroomCount { get; set; }
        public int BathroomCount { get; set; }
        public string HouseDirection { get; set; }
        public string ProjectName { get; set; }
        public string PostedAgoText { get; set; }
        public string SellerName { get; set; }
        public string SellerRole { get; set; }
        public string ThumbnailUrl { get; set; }
        public int VisualPriority { get; set; }
        public string Description { get; set; }
        public string ContactPhone { get; set; }
        public List<string> Gallery { get; set; }
        public bool IsLinked { get; set; }
        public string LinkedSource { get; set; }
        public string LinkedSourceUrl { get; set; }
    }

    public sealed class ProjectItem
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string TypeLabel { get; set; }
        public string PriceHint { get; set; }
        public int ListingCount { get; set; }
    }

    public sealed class ListingQuery
    {
        public string Purpose { get; set; }
        public string Keyword { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public string LegalStatus { get; set; }
        public string FurnishingStatus { get; set; }
        public string Project { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public decimal? AreaMin { get; set; }
        public decimal? AreaMax { get; set; }
        public string HouseDirection { get; set; }
        public string Sort { get; set; }
    }

    private static readonly List<ListingItem> SeedListings = BuildSeedListings();
    private static readonly List<ProjectItem> SeedProjects = BuildSeedProjects();

    public static SummaryInfo GetSummary()
    {
        IEnumerable<ListingItem> seed = ShouldUseSeedListings() ? SeedListings : Enumerable.Empty<ListingItem>();
        List<ListingItem> items = LoadDatabaseListings()
            .Concat(seed)
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .ToList();

        return new SummaryInfo
        {
            TotalListings = items.Count,
            SaleCount = items.Count(p => string.Equals(p.Purpose, "sale", StringComparison.OrdinalIgnoreCase)),
            RentCount = items.Count(p => string.Equals(p.Purpose, "rent", StringComparison.OrdinalIgnoreCase)),
            ProjectCount = SeedProjects.Count
        };
    }

    public static List<FilterOption> GetFeaturedRegions()
    {
        return new List<FilterOption>
        {
            new FilterOption { Label = "TP.HCM", Value = "tp-ho-chi-minh", Note = "Táº­p trung mua bÃ¡n vÃ  cho thuÃª dÃ y nháº¥t" },
            new FilterOption { Label = "HÃ  Ná»™i", Value = "ha-noi", Note = "Nhu cáº§u cÄƒn há»™, nhÃ  phá»‘, vÄƒn phÃ²ng cao" },
            new FilterOption { Label = "BÃ¬nh DÆ°Æ¡ng", Value = "binh-duong", Note = "Máº¡nh vá» cÄƒn há»™, nhÃ  phá»‘, kho xÆ°á»Ÿng" },
            new FilterOption { Label = "Äá»“ng Nai", Value = "dong-nai", Note = "PhÃ¹ há»£p Ä‘áº¥t ná»n, nhÃ  phá»‘, dá»± Ã¡n vá»‡ tinh" }
        };
    }

    public static List<FilterOption> GetFeaturedPropertyTypes()
    {
        return new List<FilterOption>
        {
            new FilterOption { Label = "CÄƒn há»™", Value = "apartment", Note = "Listing giÃ u dá»¯ liá»‡u theo dá»± Ã¡n" },
            new FilterOption { Label = "NhÃ  phá»‘", Value = "house", Note = "PhÃ¹ há»£p logic bÃ¡n á»Ÿ thá»±c vÃ  Ä‘áº§u tÆ°" },
            new FilterOption { Label = "Äáº¥t ná»n", Value = "land", Note = "Nháº¥n máº¡nh phÃ¡p lÃ½ vÃ  diá»‡n tÃ­ch" },
            new FilterOption { Label = "VÄƒn phÃ²ng", Value = "office", Note = "Táº­p trung vÃ o vá»‹ trÃ­, diá»‡n tÃ­ch, bÃ n giao" },
            new FilterOption { Label = "Máº·t báº±ng", Value = "business-premises", Note = "DÃ¹ng cho nhu cáº§u thuÃª kinh doanh" }
        };
    }

    public static List<ProjectItem> GetFeaturedProjects()
    {
        return SeedProjects.ToList();
    }

    public static bool PropertyTypeMatches(string filterType, string actualType)
    {
        string filter = (filterType ?? "").Trim().ToLowerInvariant();
        string actual = (actualType ?? "").Trim().ToLowerInvariant();

        if (filter == "")
            return true;
        if (actual == "")
            return false;
        if (string.Equals(filter, actual, StringComparison.OrdinalIgnoreCase))
            return true;

        switch (filter)
        {
            case "house":
                return actual == "townhouse";
            case "business-premises":
                return actual == "shophouse";
            case "boarding-house":
                return actual == "motel-room";
            default:
                return false;
        }
    }

    public static bool PropertyTypeMatches(ListingItem item, string filterType)
    {
        if (item == null)
            return false;

        if (PropertyTypeMatches(filterType, item.PropertyType))
            return true;

        string filter = (filterType ?? "").Trim().ToLowerInvariant();
        if (filter == "")
            return true;

        string title = Slugify(item.Title).Replace("-", " ");
        string description = Slugify(item.Description).Replace("-", " ");
        string propertyTypeLabel = Slugify(item.PropertyTypeLabel).Replace("-", " ");
        string haystack = (title + " " + description + " " + propertyTypeLabel).Trim();

        switch (filter)
        {
            case "house":
                return haystack.Contains(" nha ")
                    || haystack.StartsWith("nha ")
                    || haystack.Contains(" nha pho ")
                    || haystack.Contains(" biet thu ")
                    || haystack.Contains(" villa ")
                    || haystack.Contains(" nha rieng ")
                    || haystack.Contains(" nha nguyen can ");
            case "apartment":
                return haystack.Contains(" can ho ")
                    || haystack.Contains(" chung cu ")
                    || haystack.Contains(" studio ")
                    || haystack.Contains(" penthouse ")
                    || haystack.Contains(" duplex ");
            case "land":
                return haystack.Contains(" dat ")
                    || haystack.Contains(" dat nen ")
                    || haystack.Contains(" tho cu ")
                    || haystack.Contains(" dat vuon ")
                    || haystack.Contains(" dat sao ");
            case "business-premises":
                return haystack.Contains(" mat bang ")
                    || haystack.Contains(" shophouse ")
                    || haystack.Contains(" shop ")
                    || haystack.Contains(" kiot ")
                    || haystack.Contains(" kiosk ")
                    || haystack.Contains(" cua hang ");
            case "warehouse":
                return haystack.Contains(" kho ")
                    || haystack.Contains(" xuong ")
                    || haystack.Contains(" kho xuong ")
                    || haystack.Contains(" nha xuong ")
                    || haystack.Contains(" kho bai ");
            case "boarding-house":
                return haystack.Contains(" phong tro ")
                    || haystack.Contains(" nha tro ")
                    || haystack.Contains(" phong cho thue ");
            default:
                return false;
        }
    }

    public static string ResolveLinkedSourceLabel(string sourceKey)
    {
        string key = (sourceKey ?? "").Trim().ToLowerInvariant();
        if (key == "")
            return "Tin liÃªn káº¿t";

        switch (key)
        {
            case "muaban":
                return "Muaban";
            case "muaban-thue-bien-hoa":
                return "Muaban thuÃª BiÃªn HÃ²a";
            case "muaban-thue-dong-nai":
                return "Muaban thuÃª Äá»“ng Nai";
            case "muaban-thue-nha-bien-hoa":
                return "Muaban thuÃª nhÃ  BiÃªn HÃ²a";
            case "muaban-thue-can-ho-bien-hoa":
                return "Muaban thuÃª cÄƒn há»™ BiÃªn HÃ²a";
            case "muaban-thue-van-phong-bien-hoa":
                return "Muaban thuÃª vÄƒn phÃ²ng BiÃªn HÃ²a";
            case "muaban-thue-kho-bien-hoa":
                return "Muaban thuÃª kho/xÆ°á»Ÿng BiÃªn HÃ²a";
            case "muaban-ban-bien-hoa":
                return "Muaban bÃ¡n BiÃªn HÃ²a";
            case "muaban-ban-dong-nai":
                return "Muaban bÃ¡n Äá»“ng Nai";
            case "muaban-ban-nha-bien-hoa":
                return "Muaban bÃ¡n nhÃ  BiÃªn HÃ²a";
            case "muaban-ban-can-ho-bien-hoa":
            case "muaban-ban-can-ho-bien-hoa-v2":
                return "Muaban bÃ¡n cÄƒn há»™ BiÃªn HÃ²a";
            case "muaban-ban-dat-bien-hoa":
                return "Muaban bÃ¡n Ä‘áº¥t BiÃªn HÃ²a";
            case "batdongsan-ban-bien-hoa":
                return "Batdongsan bÃ¡n BiÃªn HÃ²a";
            case "batdongsan-ban-dat-bien-hoa":
                return "Batdongsan bÃ¡n Ä‘áº¥t BiÃªn HÃ²a";
            case "batdongsan-thue-bien-hoa":
                return "Batdongsan thuÃª BiÃªn HÃ²a";
            case "batdongsan-thue-nha-bien-hoa":
                return "Batdongsan thuÃª nhÃ  BiÃªn HÃ²a";
            case "batdongsanviet":
                return "Batdongsanviet";
            case "alonhadat":
                return "Alonhadat";
            case "rongbay":
                return "Rongbay";
            case "raovat-net":
                return "Raovat.net";
            case "homedy":
                return "Homedy";
            case "nhadat247":
                return "Nhadat247";
            case "dothi":
                return "Dothi";
            case "dothi-thue":
                return "Dothi thuÃª";
            case "nhadatcanban":
                return "Nhadatcanban";
            case "mogi":
                return "Mogi";
            case "bds123-ban":
                return "BDS123 bÃ¡n";
            case "bds123-thue":
                return "BDS123 thuÃª";
            case "phongtro123":
                return "Phongtro123";
            case "thuecanho123":
                return "Thuecanho123";
            case "nhadatso":
                return "Nhadatso";
            default:
                return string.IsNullOrWhiteSpace(sourceKey) ? "Tin liÃªn káº¿t" : sourceKey.Trim();
        }
    }

    public static string ResolveLinkedThumbnail(string thumbnailUrl, string galleryCsv)
    {
        List<string> gallery = ResolveLinkedGalleryUrls(thumbnailUrl, galleryCsv);
        return gallery.Count > 0 ? gallery[0] : DefaultBdsFallbackImage;
    }

    public static string BuildLinkedImageProxyUrl(int linkedId, int index)
    {
        if (linkedId <= 0)
            return DefaultBdsFallbackImage;

        int safeIndex = index < 0 ? 0 : index;
        return "/bat-dong-san/linked-image.ashx?id="
            + linkedId.ToString(CultureInfo.InvariantCulture)
            + "&index="
            + safeIndex.ToString(CultureInfo.InvariantCulture);
    }

    public static List<string> ResolveLinkedGalleryUrls(string thumbnailUrl, string galleryCsv)
    {
        var images = new List<string>();
        TryAddLinkedImage(images, thumbnailUrl);

        string csv = (galleryCsv ?? "").Trim();
        if (csv != "")
        {
            string[] parts = csv.Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                TryAddLinkedImage(images, parts[i]);
                if (images.Count >= 8)
                    break;
            }
        }

        if (images.Count == 0)
            images.Add(DefaultBdsFallbackImage);

        return images;
    }

    public static List<string> ResolveLinkedRawGalleryUrls(string thumbnailUrl, string galleryCsv)
    {
        var images = new List<string>();
        TryAddLinkedRawImage(images, thumbnailUrl);

        string csv = (galleryCsv ?? "").Trim();
        if (csv != "")
        {
            string[] parts = csv.Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                TryAddLinkedRawImage(images, parts[i]);
                if (images.Count >= 12)
                    break;
            }
        }

        return images;
    }

    public static int ComputeLinkedVisualPriority(string thumbnailUrl, string galleryCsv)
    {
        List<string> localOrUsable = ResolveLinkedGalleryUrls(thumbnailUrl, galleryCsv)
            .Where(x => !IsBdsFallbackImage(x))
            .ToList();
        if (localOrUsable.Any(x => IsLocalImagePath(x)))
            return 3;
        if (localOrUsable.Count > 0 || ResolveLinkedRawGalleryUrls(thumbnailUrl, galleryCsv).Count > 0)
            return 1;

        return 0;
    }

    public static bool IsBdsFallbackImage(string url)
    {
        string value = (url ?? "").Trim();
        return value == ""
            || value.Equals(DefaultBdsFallbackImage, StringComparison.OrdinalIgnoreCase)
            || value.Equals("/uploads/images/macdinh.jpg", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsLocalImagePath(string url)
    {
        string value = (url ?? "").Trim();
        return value.StartsWith("/", StringComparison.OrdinalIgnoreCase)
            && !value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            && !value.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    public static ProjectItem GetProjectBySlug(string slug)
    {
        string normalized = (slug ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return null;

        return SeedProjects.FirstOrDefault(p => string.Equals((p.Slug ?? "").Trim(), normalized, StringComparison.OrdinalIgnoreCase));
    }

    public static string BuildProjectUrl(ProjectItem item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Slug))
            return "/bat-dong-san/du-an";

        return "/bat-dong-san/du-an/" + item.Slug.Trim().ToLowerInvariant();
    }

    public static List<ListingItem> GetListingsByProject(string projectName, int take)
    {
        IEnumerable<ListingItem> items = GetAllListings();
        string normalized = (projectName ?? "").Trim();
        if (normalized != "")
        {
            items = items.Where(p => string.Equals((p.ProjectName ?? "").Trim(), normalized, StringComparison.OrdinalIgnoreCase));
        }

        return items
            .OrderByDescending(p => p.Id)
            .Take(take)
            .ToList();
    }

    public static List<FilterOption> GetDistrictOptions(string province)
    {
        IEnumerable<ListingItem> items = GetAllListings();
        if (!string.IsNullOrWhiteSpace(province))
            items = items.Where(p => Slugify(p.Province) == province.Trim().ToLowerInvariant());

        return items
            .Where(p => !string.IsNullOrWhiteSpace(p.District))
            .GroupBy(p => p.District.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key)
            .Select(g => new FilterOption
            {
                Label = g.Key,
                Value = Slugify(g.Key),
                Note = g.Count().ToString("#,##0") + " tin"
            })
            .ToList();
    }

    public static List<FilterOption> GetLegalStatusOptions()
    {
        return GetAllListings()
            .Where(p => !string.IsNullOrWhiteSpace(p.LegalStatus))
            .GroupBy(p => p.LegalStatus.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key)
            .Select(g => new FilterOption
            {
                Label = g.Key,
                Value = g.Key,
                Note = g.Count().ToString("#,##0") + " tin"
            })
            .ToList();
    }

    public static List<FilterOption> GetFurnishingOptions()
    {
        return GetAllListings()
            .Where(p => !string.IsNullOrWhiteSpace(p.FurnishingStatus))
            .GroupBy(p => p.FurnishingStatus.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key)
            .Select(g => new FilterOption
            {
                Label = g.Key,
                Value = g.Key,
                Note = g.Count().ToString("#,##0") + " tin"
            })
            .ToList();
    }

    public static List<FilterOption> GetProjectOptions()
    {
        return GetAllListings()
            .Where(p => !string.IsNullOrWhiteSpace(p.ProjectName))
            .GroupBy(p => p.ProjectName.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key)
            .Select(g => new FilterOption
            {
                Label = g.Key,
                Value = g.Key,
                Note = g.Count().ToString("#,##0") + " tin"
            })
            .ToList();
    }

    public static List<FilterOption> GetDirectionOptions()
    {
        string[] directions = new[] { "ÄÃ´ng", "TÃ¢y", "Nam", "Báº¯c", "ÄÃ´ng Nam", "ÄÃ´ng Báº¯c", "TÃ¢y Nam", "TÃ¢y Báº¯c" };
        return directions.Select(d => new FilterOption { Label = d, Value = d }).ToList();
    }

    public static List<ListingItem> GetFeaturedListings(string purpose, int take)
    {
        return QueryListings(new ListingQuery
        {
            Purpose = purpose,
            Sort = "newest"
        }).Take(take).ToList();
    }

    public static List<ListingItem> GetLiveListings()
    {
        return LoadDatabaseListings();
    }

    public static List<ListingItem> QueryListings(ListingQuery query)
    {
        IEnumerable<ListingItem> items = GetAllListings();
        query = query ?? new ListingQuery();

        if (!string.IsNullOrWhiteSpace(query.Purpose))
            items = items.Where(p => string.Equals(p.Purpose, query.Purpose, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            string keyword = query.Keyword.Trim().ToLowerInvariant();
            items = items.Where(p =>
                ContainsText(p.Title, keyword)
                || ContainsText(p.ProjectName, keyword)
                || ContainsText(p.District, keyword)
                || ContainsText(p.Province, keyword)
                || ContainsText(p.AddressText, keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.Province))
            items = items.Where(p => Slugify(p.Province) == query.Province.Trim().ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(query.District))
            items = items.Where(p => Slugify(p.District) == query.District.Trim().ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(query.PropertyType))
            items = items.Where(p => PropertyTypeMatches(p, query.PropertyType));

        if (!string.IsNullOrWhiteSpace(query.Bedrooms))
        {
            int bedrooms;
            if (int.TryParse(query.Bedrooms, out bedrooms) && bedrooms > 0)
                items = items.Where(p => p.BedroomCount >= bedrooms);
        }

        if (!string.IsNullOrWhiteSpace(query.LegalStatus))
            items = items.Where(p => string.Equals((p.LegalStatus ?? "").Trim(), query.LegalStatus.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(query.FurnishingStatus))
            items = items.Where(p => string.Equals((p.FurnishingStatus ?? "").Trim(), query.FurnishingStatus.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(query.Project))
            items = items.Where(p => string.Equals((p.ProjectName ?? "").Trim(), query.Project.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(query.HouseDirection))
            items = items.Where(p => string.Equals((p.HouseDirection ?? "").Trim(), query.HouseDirection.Trim(), StringComparison.OrdinalIgnoreCase));

        if (query.PriceMin.HasValue)
            items = items.Where(p => p.PriceValue >= query.PriceMin.Value);

        if (query.PriceMax.HasValue && query.PriceMax.Value > 0)
            items = items.Where(p => p.PriceValue <= query.PriceMax.Value);

        if (query.AreaMin.HasValue)
            items = items.Where(p => p.AreaValue >= query.AreaMin.Value);

        if (query.AreaMax.HasValue && query.AreaMax.Value > 0)
            items = items.Where(p => p.AreaValue <= query.AreaMax.Value);

        switch ((query.Sort ?? "").Trim().ToLowerInvariant())
        {
            case "price_asc":
                items = items.OrderByDescending(p => p.VisualPriority).ThenBy(p => p.PriceValue).ThenByDescending(p => p.Id);
                break;
            case "price_desc":
                items = items.OrderByDescending(p => p.VisualPriority).ThenByDescending(p => p.PriceValue).ThenByDescending(p => p.Id);
                break;
            case "area_desc":
                items = items.OrderByDescending(p => p.VisualPriority).ThenByDescending(p => p.AreaValue).ThenByDescending(p => p.Id);
                break;
            default:
                items = items.OrderByDescending(p => p.VisualPriority).ThenByDescending(p => p.Id);
                break;
        }

        return items.ToList();
    }

    public static ListingItem GetListingById(int id)
    {
        return GetAllListings().FirstOrDefault(p => p.Id == id);
    }

    public static List<ListingItem> GetRelatedListings(ListingItem current, int take)
    {
        if (current == null)
            return new List<ListingItem>();

        return GetAllListings()
            .Where(p => p.Id != current.Id
                && string.Equals(p.Purpose, current.Purpose, StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.Province, current.Province, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(p => p.Id)
            .Take(take)
            .ToList();
    }

    public static List<ListingItem> GetSimilarListings(ListingItem current, int take)
    {
        if (current == null)
            return new List<ListingItem>();

        return GetAllListings()
            .Where(p => p.Id != current.Id
                && string.Equals(p.Purpose, current.Purpose, StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.PropertyType, current.PropertyType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.Province, current.Province, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(p => p.Id)
            .Take(take)
            .ToList();
    }

    public static string BuildListingUrl(ListingItem item)
    {
        if (item == null)
            return "/bat-dong-san";

        if (item.IsLinked)
            return "/bat-dong-san/chi-tiet.aspx?linkedId=" + item.Id.ToString(CultureInfo.InvariantCulture);

        // DÃ¹ng .aspx Ä‘á»ƒ Ä‘áº£m báº£o cháº¡y trÃªn mÃ´i trÆ°á»ng chÆ°a báº­t rewrite (localhost/mono)
        return "/bat-dong-san/chi-tiet.aspx?id=" + item.Id.ToString(CultureInfo.InvariantCulture);
    }

    public static string FormatArea(decimal areaValue)
    {
        return areaValue.ToString("#,##0.##", CultureInfo.InvariantCulture) + " mÂ²";
    }

    public static string FormatPricePerSquareMeter(ListingItem item)
    {
        if (item == null || item.AreaValue <= 0 || item.PriceValue <= 0)
            return "-";

        decimal unitPrice = item.PriceValue / item.AreaValue;
        if (string.Equals(item.Purpose, "rent", StringComparison.OrdinalIgnoreCase))
            return unitPrice.ToString("#,##0", CultureInfo.InvariantCulture) + " Ä‘/mÂ²/thÃ¡ng";

        return unitPrice.ToString("#,##0", CultureInfo.InvariantCulture) + " Ä‘/mÂ²";
    }

    public static string BuildListingMeta(ListingItem item)
    {
        if (item == null)
            return "";

        var parts = new List<string>();
        parts.Add(item.PropertyTypeLabel);
        if (item.BedroomCount > 0)
            parts.Add(item.BedroomCount.ToString(CultureInfo.InvariantCulture) + " PN");
        if (item.BathroomCount > 0)
            parts.Add(item.BathroomCount.ToString(CultureInfo.InvariantCulture) + " WC");
        if (item.AreaValue > 0)
            parts.Add(FormatArea(item.AreaValue));
        if (item.FloorCount > 0 && string.Equals(item.PropertyType, "house", StringComparison.OrdinalIgnoreCase))
            parts.Add(item.FloorCount.ToString(CultureInfo.InvariantCulture) + " táº§ng");
        if (item.LandWidth > 0 && item.LandLength > 0 && string.Equals(item.PropertyType, "land", StringComparison.OrdinalIgnoreCase))
            parts.Add(item.LandWidth.ToString("0.##", CultureInfo.InvariantCulture) + " x " + item.LandLength.ToString("0.##", CultureInfo.InvariantCulture) + " m");

        return string.Join(" â€¢ ", parts.ToArray());
    }

    public static string SanitizeLinkedText(string raw, int maxChars, bool preserveLineBreaks)
    {
        string text = (raw ?? "").Trim();
        if (text == "")
            return "";

        text = HttpUtility.HtmlDecode(text);
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");
        text = Regex.Replace(text, @"<[^>]+>", " ", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\\[nrt]", " ", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\\?""@context\\?""\s*:\s*\\?""https?:\\?/\\?/schema\.org\\?""", " ", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\{[^\{\}]{0,1200}@context[^\{\}]{0,1200}\}", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        text = Regex.Replace(text, @"\s*[,;:\.\-]{8,}\s*", " ", RegexOptions.IgnoreCase);

        string[] scriptTokens = new[]
        {
            "window.",
            "document.",
            "function(",
            "analytics.",
            "analyticsload",
            "datalayer",
            "gtag(",
            "googletag",
            "sentry.",
            "snippet_version",
            "__writekey",
            "<iframe",
            "base64,",
            "iconapp",
            "script",
            "@context",
            "schema.org",
            "breadcrumblist",
            "realestatelisting",
            "itemlistelement",
            "tÃ i khoáº£n xin chÃ o",
            "tai khoan xin chao",
            "táº¡o tÃ i khoáº£n Ä‘Äƒng nháº­p",
            "tao tai khoan dang nhap",
            "xin chÃ o, khÃ¡ch",
            "xin chao, khach"
        };

        string lower = text.ToLowerInvariant();
        int cutIndex = -1;
        for (int i = 0; i < scriptTokens.Length; i++)
        {
            int idx = lower.IndexOf(scriptTokens[i], StringComparison.Ordinal);
            if (idx >= 0 && (cutIndex < 0 || idx < cutIndex))
                cutIndex = idx;
        }

        if (cutIndex >= 0)
            text = text.Substring(0, cutIndex);

        if (preserveLineBreaks)
        {
            string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var cleanLines = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = Regex.Replace(lines[i], @"[ \t]+", " ").Trim();
                if (line == "")
                    continue;
                if (LooksLikeGarbageLine(line))
                    continue;
                if (LooksLikeStructuredDataLine(line))
                    continue;
                cleanLines.Add(line);
            }

            text = string.Join("\n", cleanLines.ToArray());
            text = Regex.Replace(text, @"\n{3,}", "\n\n");
        }
        else
        {
            text = Regex.Replace(text, @"\s+", " ").Trim();
        }

        text = Regex.Replace(text, @"[\{\}\[\]]+", " ");
        text = text.Replace(" .", ".").Replace(" ,", ",").Replace(" ;", ";").Replace(" :", ":");
        text = text.Trim(' ', ',', ';', '-', '|');

        if (LooksLikeGarbageLine(text) || LooksLikeStructuredDataLine(text))
            return "";

        if (maxChars > 0 && text.Length > maxChars)
            text = text.Substring(0, maxChars).Trim() + "...";

        return text;
    }

    private static bool LooksLikeGarbageLine(string value)
    {
        string text = (value ?? "").Trim();
        if (text == "")
            return true;

        if (Regex.IsMatch(text, @"^[\}\{\]\[;:,\.\-=_'""`~!@#$%^&*()+/\\|<>?]+$"))
            return true;

        string lower = text.ToLowerInvariant();
        return lower.Contains("window.")
            || lower.Contains("document.")
            || lower.Contains("analytics.")
            || lower.Contains("function(")
            || lower.Contains("datalayer")
            || lower.Contains("gtag(")
            || lower.Contains("googletag")
            || lower.Contains("sentry.")
            || lower.Contains("base64,")
            || lower.Contains("<iframe");
    }

    private static bool LooksLikeStructuredDataLine(string value)
    {
        string text = (value ?? "").Trim();
        if (text == "")
            return true;

        string lower = text.ToLowerInvariant();
        if (lower.Contains("@context")
            || lower.Contains("schema.org")
            || lower.Contains("breadcrumblist")
            || lower.Contains("realestatelisting")
            || lower.Contains("itemlistelement")
            || lower.Contains("\"@type\"")
            || lower.Contains("\"@id\""))
            return true;

        int braceCount = text.Count(ch => ch == '{' || ch == '}' || ch == '[' || ch == ']');
        int colonCount = text.Count(ch => ch == ':');
        int quoteCount = text.Count(ch => ch == '"' || ch == '\'');
        if (braceCount >= 2 && colonCount >= 2 && quoteCount >= 4)
            return true;

        return false;
    }

    public static string FormatMoneyCompact(decimal value, string suffix)
    {
        if (value <= 0)
            return "-";

        if (value >= 1000000000m)
            return (value / 1000000000m).ToString("0.##", CultureInfo.InvariantCulture) + " tá»·" + suffix;

        if (value >= 1000000m)
            return (value / 1000000m).ToString("0.##", CultureInfo.InvariantCulture) + " triá»‡u" + suffix;

        return value.ToString("#,##0", CultureInfo.InvariantCulture) + " Ä‘" + suffix;
    }

    public static string Slugify(string input)
    {
        string value = (input ?? "").Trim().ToLowerInvariant();
        value = value.Replace("Ä‘", "d");
        string[] source = new string[] { "Ã ", "Ã¡", "áº¡", "áº£", "Ã£", "Ã¢", "áº§", "áº¥", "áº­", "áº©", "áº«", "Äƒ", "áº±", "áº¯", "áº·", "áº³", "áºµ", "Ã¨", "Ã©", "áº¹", "áº»", "áº½", "Ãª", "á»", "áº¿", "á»‡", "á»ƒ", "á»…", "Ã¬", "Ã­", "á»‹", "á»‰", "Ä©", "Ã²", "Ã³", "á»", "á»", "Ãµ", "Ã´", "á»“", "á»‘", "á»™", "á»•", "á»—", "Æ¡", "á»", "á»›", "á»£", "á»Ÿ", "á»¡", "Ã¹", "Ãº", "á»¥", "á»§", "Å©", "Æ°", "á»«", "á»©", "á»±", "á»­", "á»¯", "á»³", "Ã½", "á»µ", "á»·", "á»¹" };
        string[] target = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y" };
        for (int i = 0; i < source.Length; i++)
            value = value.Replace(source[i], target[i]);

        while (value.Contains("  "))
            value = value.Replace("  ", " ");

        value = value.Replace("/", " ").Replace(",", " ").Replace(".", " ").Replace("(", " ").Replace(")", " ");
        while (value.Contains("  "))
            value = value.Replace("  ", " ");

        return value.Trim().Replace(" ", "-");
    }

    private static bool ContainsText(string value, string keyword)
    {
        string left = Slugify(value).Replace("-", " ");
        string right = Slugify(keyword).Replace("-", " ");
        if (right == "")
            return true;
        return left.Contains(right);
    }

    private static void TryAddLinkedImage(List<string> images, string candidate)
    {
        string normalized = NormalizeLinkedImageUrl(candidate);
        if (normalized == "")
            return;
        if (images.Contains(normalized))
            return;
        images.Add(normalized);
    }

    private static void TryAddLinkedRawImage(List<string> images, string candidate)
    {
        string normalized = NormalizeLinkedRawImageUrl(candidate);
        if (normalized == "")
            return;
        if (images.Contains(normalized))
            return;
        images.Add(normalized);
    }

    private static string NormalizeLinkedImageUrl(string candidate)
    {
        string url = (candidate ?? "").Trim();
        if (url == "" || url.Equals(DefaultBdsFallbackImage, StringComparison.OrdinalIgnoreCase) || url.Equals("/uploads/images/macdinh.jpg", StringComparison.OrdinalIgnoreCase))
            return "";

        string lower = url.ToLowerInvariant();
        string[] blockedTokens = new[]
        {
            "avatar",
            "profile",
            "logo",
            "icon",
            "banner",
            "favicon",
            "placeholder",
            "default-user"
        };

        for (int i = 0; i < blockedTokens.Length; i++)
        {
            if (lower.Contains(blockedTokens[i]))
                return "";
        }

        if (url.StartsWith("/"))
        {
            try
            {
                HttpContext ctx = HttpContext.Current;
                if (ctx == null || ctx.Server == null)
                    return "";

                string physical = ctx.Server.MapPath(url);
                if (!File.Exists(physical))
                    return "";
            }
            catch
            {
                return "";
            }

            return url;
        }

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return url;

        return "";
    }

    private static string NormalizeLinkedRawImageUrl(string candidate)
    {
        string url = (candidate ?? "").Trim();
        if (url == "" || url.Equals(DefaultBdsFallbackImage, StringComparison.OrdinalIgnoreCase) || url.Equals("/uploads/images/macdinh.jpg", StringComparison.OrdinalIgnoreCase))
            return "";

        string lower = url.ToLowerInvariant();
        string[] blockedTokens = new[]
        {
            "avatar",
            "profile",
            "logo",
            "icon",
            "banner",
            "favicon",
            "placeholder",
            "default-user"
        };

        for (int i = 0; i < blockedTokens.Length; i++)
        {
            if (lower.Contains(blockedTokens[i]))
                return "";
        }

        if (url.StartsWith("/"))
            return url;

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return url;

        return "";
    }

    private static List<ListingItem> LoadDatabaseListings()
    {
        try
        {
            List<BatDongSanMetadata_cl.PublicListingRow> rows = BatDongSanMetadata_cl.LoadPublicListings();
            return rows.Select(MapPublicRowToListing).Where(p => p != null).ToList();
        }
        catch
        {
            return new List<ListingItem>();
        }
    }

    private static List<ListingItem> GetAllListings()
    {
        IEnumerable<ListingItem> seed = ShouldUseSeedListings() ? SeedListings : Enumerable.Empty<ListingItem>();
        return LoadDatabaseListings()
            .Concat(seed)
            .GroupBy(p => p.Id)
            .Select(g => g.OrderByDescending(x => x.Id).First())
            .ToList();
    }

    private static bool ShouldUseSeedListings()
    {
        try
        {
            var context = System.Web.HttpContext.Current;
            string host = "";
            if (context != null && context.Request != null && context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Host))
            {
                host = context.Request.Url.Host.ToLowerInvariant();
                if (host.Contains("localhost") || host.Contains("127.0.0.1"))
                    return true;
            }

            string setting = System.Configuration.ConfigurationManager.AppSettings["BDS.UseSeedListings"];
            if (!string.IsNullOrEmpty(setting))
            {
                bool enabled;
                if (bool.TryParse(setting, out enabled))
                    return enabled;
            }
        }
        catch
        {
        }

        return false;
    }

    private static ListingItem MapPublicRowToListing(BatDongSanMetadata_cl.PublicListingRow row)
    {
        if (row == null || row.PostId <= 0)
            return null;

        string purpose = BatDongSanMetadata_cl.NormalizePurpose(row.ListingPurpose);
        string propertyType = BatDongSanMetadata_cl.NormalizePropertyType(row.PropertyType);
        string propertyTypeLabel = BuildPropertyTypeLabel(propertyType);
        int bedroomCount = row.BedroomCount ?? 0;
        int bathroomCount = row.BathroomCount ?? 0;
        decimal priceValue = row.PostPrice ?? 0;
        decimal areaValue = row.AreaValue ?? 0;
        decimal depositAmount = row.DepositAmount ?? 0;
        int rentalTermMonths = row.RentalTermMonths ?? 0;
        int floorCount = row.FloorCount ?? 0;
        decimal landWidth = row.LandWidth ?? 0;
        decimal landLength = row.LandLength ?? 0;

        return new ListingItem
        {
            Id = (int)row.PostId,
            Slug = Slugify((row.PostSlug ?? "").Trim() == "" ? row.PostTitle : row.PostSlug),
            Title = (row.PostTitle ?? "").Trim(),
            Purpose = purpose,
            PropertyType = propertyType,
            PropertyTypeLabel = propertyTypeLabel,
            Province = (row.ProvinceName ?? "").Trim(),
            District = (row.DistrictName ?? "").Trim(),
            Ward = (row.WardName ?? "").Trim(),
            AddressText = (row.AddressLine ?? "").Trim(),
            PriceValue = priceValue,
            PriceText = BuildPriceText(purpose, priceValue),
            AreaValue = areaValue,
            DepositAmount = depositAmount,
            RentalTermMonths = rentalTermMonths,
            FloorCount = floorCount,
            LandWidth = landWidth,
            LandLength = landLength,
            HouseDirection = string.IsNullOrWhiteSpace(row.HouseDirection) ? "" : row.HouseDirection.Trim(),
            LegalStatus = string.IsNullOrWhiteSpace(row.LegalStatus) ? "ChÆ°a cáº­p nháº­t phÃ¡p lÃ½" : row.LegalStatus.Trim(),
            FurnishingStatus = string.IsNullOrWhiteSpace(row.FurnishingStatus) ? "ChÆ°a cáº­p nháº­t ná»™i tháº¥t" : row.FurnishingStatus.Trim(),
            BedroomCount = bedroomCount,
            BathroomCount = bathroomCount,
            ProjectName = (row.ProjectName ?? "").Trim(),
            PostedAgoText = BuildPostedAgoText(row.PostCreatedAt),
            SellerName = string.IsNullOrWhiteSpace(row.PostOwner) ? "AhaSale" : row.PostOwner.Trim(),
            SellerRole = "Tin tá»« AhaSale Home",
            ThumbnailUrl = string.IsNullOrWhiteSpace(row.PostImage) ? "/uploads/images/macdinh.jpg" : row.PostImage.Trim(),
            VisualPriority = string.IsNullOrWhiteSpace(row.PostImage) ? 0 : 3,
            Description = string.IsNullOrWhiteSpace(row.PostDescription) ? "Tin báº¥t Ä‘á»™ng sáº£n Ä‘Æ°á»£c Ä‘Äƒng tá»« luá»“ng Home vÃ  Ä‘Ã£ map sang metadata BÄS." : row.PostDescription.Trim(),
            ContactPhone = "",
            Gallery = new List<string> { string.IsNullOrWhiteSpace(row.PostImage) ? "/uploads/images/macdinh.jpg" : row.PostImage.Trim() }
        };
    }

    private static string BuildPropertyTypeLabel(string propertyType)
    {
        switch ((propertyType ?? "").Trim().ToLowerInvariant())
        {
            case "house":
                return "NhÃ  phá»‘";
            case "land":
                return "Äáº¥t ná»n";
            case "office":
                return "VÄƒn phÃ²ng";
            case "business-premises":
                return "Máº·t báº±ng";
            case "townhouse":
                return "NhÃ  liá»n ká»";
            default:
                return "CÄƒn há»™";
        }
    }

    private static string BuildPriceText(string purpose, decimal priceValue)
    {
        if (priceValue <= 0)
            return "LiÃªn há»‡";

        if (string.Equals(purpose, "rent", StringComparison.OrdinalIgnoreCase))
            return priceValue.ToString("#,##0", CultureInfo.InvariantCulture) + " Ä‘/thÃ¡ng";

        if (priceValue >= 1000000000m)
            return (priceValue / 1000000000m).ToString("0.##", CultureInfo.InvariantCulture) + " tá»·";

        return priceValue.ToString("#,##0", CultureInfo.InvariantCulture) + " Ä‘";
    }

    private static string BuildPostedAgoText(DateTime? date)
    {
        if (!date.HasValue)
            return "Má»›i Ä‘Äƒng";

        TimeSpan diff = AhaTime_cl.Now - date.Value;
        if (diff.TotalMinutes < 60)
            return Math.Max(1, (int)diff.TotalMinutes).ToString(CultureInfo.InvariantCulture) + " phÃºt trÆ°á»›c";
        if (diff.TotalHours < 24)
            return Math.Max(1, (int)diff.TotalHours).ToString(CultureInfo.InvariantCulture) + " giá» trÆ°á»›c";
        return Math.Max(1, (int)diff.TotalDays).ToString(CultureInfo.InvariantCulture) + " ngÃ y trÆ°á»›c";
    }

    private static List<ProjectItem> BuildSeedProjects()
    {
        return new List<ProjectItem>
        {
            new ProjectItem { Slug = "celesta-rise", Name = "Celesta Rise", Location = "NhÃ  BÃ¨, TP.HCM", TypeLabel = "CÄƒn há»™", PriceHint = "Tá»« 3,2 tá»·", ListingCount = 14 },
            new ProjectItem { Slug = "the-rivana", Name = "The Rivana", Location = "Thuáº­n An, BÃ¬nh DÆ°Æ¡ng", TypeLabel = "CÄƒn há»™", PriceHint = "Tá»« 2,1 tá»·", ListingCount = 9 },
            new ProjectItem { Slug = "aqua-city", Name = "Aqua City", Location = "BiÃªn HÃ²a, Äá»“ng Nai", TypeLabel = "NhÃ  phá»‘ / Biá»‡t thá»±", PriceHint = "Tá»« 6,8 tá»·", ListingCount = 11 }
        };
    }

    private static List<ListingItem> BuildSeedListings()
    {
        return new List<ListingItem>
        {
            new ListingItem
            {
                Id = 1006,
                Slug = "ban-can-ho-2pn-celesta-rise-view-song",
                Title = "BÃ¡n cÄƒn há»™ 2PN Celesta Rise view sÃ´ng, ná»™i tháº¥t cÆ¡ báº£n Ä‘áº¹p",
                Purpose = "sale",
                PropertyType = "apartment",
                PropertyTypeLabel = "CÄƒn há»™",
                Province = "TP.HCM",
                District = "NhÃ  BÃ¨",
                Ward = "PhÆ°á»›c Kiá»ƒn",
                AddressText = "Celesta Rise, PhÆ°á»›c Kiá»ƒn, NhÃ  BÃ¨, TP.HCM",
                PriceValue = 3450000000m,
                PriceText = "3,45 tá»·",
                AreaValue = 78m,
                LegalStatus = "HÄMB / Ä‘ang chá» sá»•",
                FurnishingStatus = "Ná»™i tháº¥t cÆ¡ báº£n",
                BedroomCount = 2,
                BathroomCount = 2,
                ProjectName = "Celesta Rise",
                PostedAgoText = "35 phÃºt trÆ°á»›c",
                SellerName = "Ngá»c Lan",
                SellerRole = "MÃ´i giá»›i chuyÃªn dá»± Ã¡n",
                ThumbnailUrl = "/uploads/images/macdinh.jpg",
                Description = "CÄƒn há»™ bá»‘ trÃ­ thoÃ¡ng, ban cÃ´ng rá»™ng, phÃ¹ há»£p gia Ä‘Ã¬nh tráº» muá»‘n á»Ÿ tháº­t hoáº·c Ä‘áº§u tÆ° cho thuÃª. Tin máº«u dÃ¹ng Ä‘á»ƒ dá»±ng logic listing-first cho /bat-dong-san.",
                ContactPhone = "0901 234 567",
                Gallery = new List<string> { "/uploads/images/macdinh.jpg", "/uploads/images/macdinh.jpg", "/uploads/images/macdinh.jpg" }
            },
            new ListingItem
            {
                Id = 1005,
                Slug = "ban-nha-pho-1-tret-2-lau-hiep-binh-phuoc",
                Title = "BÃ¡n nhÃ  phá»‘ 1 trá»‡t 2 láº§u gáº§n Pháº¡m VÄƒn Äá»“ng, phÃ¡p lÃ½ rÃµ rÃ ng",
                Purpose = "sale",
                PropertyType = "house",
                PropertyTypeLabel = "NhÃ  phá»‘",
                Province = "TP.HCM",
                District = "Thá»§ Äá»©c",
                Ward = "Hiá»‡p BÃ¬nh PhÆ°á»›c",
                AddressText = "Hiá»‡p BÃ¬nh PhÆ°á»›c, Thá»§ Äá»©c, TP.HCM",
                PriceValue = 7250000000m,
                PriceText = "7,25 tá»·",
                AreaValue = 82m,
                FloorCount = 3,
                LegalStatus = "Sá»• há»“ng riÃªng",
                FurnishingStatus = "HoÃ n thiá»‡n",
                BedroomCount = 4,
                BathroomCount = 4,
                ProjectName = "",
                PostedAgoText = "1 giá» trÆ°á»›c",
                SellerName = "Háº£i Nam",
                SellerRole = "ChÃ­nh chá»§",
                ThumbnailUrl = "/uploads/images/macdinh.jpg",
                Description = "NhÃ  hoÃ n thiá»‡n Ä‘áº¹p, háº»m xe hÆ¡i, phÃ¹ há»£p á»Ÿ káº¿t há»£p lÃ m vÄƒn phÃ²ng nhá». ÄÃ¢y lÃ  tin máº«u Ä‘á»ƒ mÃ´ phá»ng nhÃ³m nhÃ  phá»‘ trong phase 1.",
                ContactPhone = "0938 222 999",
                Gallery = new List<string> { "/uploads/images/macdinh.jpg", "/uploads/images/macdinh.jpg" }
            },
            new ListingItem
            {
                Id = 1004,
                Slug = "ban-lo-dat-tho-cu-gan-san-bay-long-thanh",
                Title = "BÃ¡n lÃ´ Ä‘áº¥t thá»• cÆ° gáº§n sÃ¢n bay Long ThÃ nh, ngang Ä‘áº¹p dá»… Ä‘áº§u tÆ°",
                Purpose = "sale",
                PropertyType = "land",
                PropertyTypeLabel = "Äáº¥t ná»n",
                Province = "Äá»“ng Nai",
                District = "Long ThÃ nh",
                Ward = "BÃ¬nh SÆ¡n",
                AddressText = "BÃ¬nh SÆ¡n, Long ThÃ nh, Äá»“ng Nai",
                PriceValue = 2180000000m,
                PriceText = "2,18 tá»·",
                AreaValue = 110m,
                LandWidth = 5m,
                LandLength = 22m,
                LegalStatus = "Sá»• riÃªng",
                FurnishingStatus = "KhÃ´ng Ã¡p dá»¥ng",
                BedroomCount = 0,
                BathroomCount = 0,
                ProjectName = "",
                PostedAgoText = "2 giá» trÆ°á»›c",
                SellerName = "Táº¥n PhÃ¡t",
                SellerRole = "NhÃ  Ä‘áº§u tÆ°",
                ThumbnailUrl = "/uploads/images/macdinh.jpg",
                Description = "LÃ´ Ä‘áº¥t vuÃ´ng vá»©c, Ä‘Æ°á»ng Ã´ tÃ´, phÃ¹ há»£p giá»¯ tÃ i sáº£n trung háº¡n. Metadata loáº¡i Ä‘áº¥t, phÃ¡p lÃ½, diá»‡n tÃ­ch lÃ  dá»¯ liá»‡u báº¯t buá»™c cho listing BÄS.",
                ContactPhone = "0977 888 123",
                Gallery = new List<string> { "/uploads/images/macdinh.jpg" }
            },
            new ListingItem
            {
                Id = 1003,
                Slug = "cho-thue-can-ho-the-rivana-full-noi-that",
                Title = "Cho thuÃª cÄƒn há»™ The Rivana full ná»™i tháº¥t, vÃ o á»Ÿ ngay",
                Purpose = "rent",
                PropertyType = "apartment",
                PropertyTypeLabel = "CÄƒn há»™",
                Province = "BÃ¬nh DÆ°Æ¡ng",
                District = "Thuáº­n An",
                Ward = "VÄ©nh PhÃº",
                AddressText = "The Rivana, VÄ©nh PhÃº, Thuáº­n An, BÃ¬nh DÆ°Æ¡ng",
                PriceValue = 14500000m,
                PriceText = "14,5 triá»‡u/thÃ¡ng",
                AreaValue = 72m,
                DepositAmount = 29000000m,
                RentalTermMonths = 12,
                LegalStatus = "Äá»§ Ä‘iá»u kiá»‡n giao dá»‹ch",
                FurnishingStatus = "Full ná»™i tháº¥t",
                BedroomCount = 2,
                BathroomCount = 2,
                ProjectName = "The Rivana",
                PostedAgoText = "20 phÃºt trÆ°á»›c",
                SellerName = "TrÃºc Vy",
                SellerRole = "ChuyÃªn viÃªn cho thuÃª",
                ThumbnailUrl = "/uploads/images/macdinh.jpg",
                Description = "CÄƒn há»™ phÃ¹ há»£p chuyÃªn gia, ná»™i tháº¥t Ä‘áº§y Ä‘á»§, cÃ³ thá»ƒ dá»n vÃ o á»Ÿ ngay. Tin máº«u cho nhÃ¡nh cho thuÃª trong /bat-dong-san.",
                ContactPhone = "0916 555 888",
                Gallery = new List<string> { "/uploads/images/macdinh.jpg", "/uploads/images/macdinh.jpg" }
            },
            new ListingItem
            {
                Id = 1002,
                Slug = "cho-thue-van-phong-mat-tien-nguyen-van-linh",
                Title = "Cho thuÃª vÄƒn phÃ²ng máº·t tiá»n Nguyá»…n VÄƒn Linh, bÃ n giao hoÃ n thiá»‡n",
                Purpose = "rent",
                PropertyType = "office",
                PropertyTypeLabel = "VÄƒn phÃ²ng",
                Province = "TP.HCM",
                District = "Quáº­n 7",
                Ward = "TÃ¢n PhÃº",
                AddressText = "Nguyá»…n VÄƒn Linh, TÃ¢n PhÃº, Quáº­n 7, TP.HCM",
                PriceValue = 32000000m,
                PriceText = "32 triá»‡u/thÃ¡ng",
                AreaValue = 95m,
                DepositAmount = 64000000m,
                RentalTermMonths = 24,
                LegalStatus = "Há»£p Ä‘á»“ng thuÃª thÆ°Æ¡ng máº¡i",
                FurnishingStatus = "HoÃ n thiá»‡n cÆ¡ báº£n",
                BedroomCount = 0,
                BathroomCount = 2,
                ProjectName = "",
                PostedAgoText = "55 phÃºt trÆ°á»›c",
                SellerName = "Minh Khang Office",
                SellerRole = "ÄÆ¡n vá»‹ cho thuÃª",
                ThumbnailUrl = "/uploads/images/macdinh.jpg",
                Description = "Máº·t báº±ng vÄƒn phÃ²ng sÃ¡ng, dá»… set up thÆ°Æ¡ng hiá»‡u, phÃ¹ há»£p SME. DÃ¹ng Ä‘á»ƒ mÃ´ phá»ng nhÃ³m office/business-premises trÃªn listing.",
                ContactPhone = "0908 666 777",
                Gallery = new List<string> { "/uploads/images/macdinh.jpg", "/uploads/images/macdinh.jpg" }
            },
            new ListingItem
            {
                Id = 1001,
                Slug = "cho-thue-mat-bang-khu-do-thi-aqua-city",
                Title = "Cho thuÃª máº·t báº±ng kinh doanh gáº§n Aqua City, trá»¥c khÃ¡ch Ä‘Ã´ng",
                Purpose = "rent",
                PropertyType = "business-premises",
                PropertyTypeLabel = "Máº·t báº±ng",
                Province = "Äá»“ng Nai",
                District = "BiÃªn HÃ²a",
                Ward = "Long HÆ°ng",
                AddressText = "Long HÆ°ng, BiÃªn HÃ²a, Äá»“ng Nai",
                PriceValue = 22000000m,
                PriceText = "22 triá»‡u/thÃ¡ng",
                AreaValue = 120m,
                DepositAmount = 44000000m,
                RentalTermMonths = 12,
                LegalStatus = "Há»£p Ä‘á»“ng thuÃª",
                FurnishingStatus = "BÃ n giao thÃ´",
                BedroomCount = 0,
                BathroomCount = 1,
                ProjectName = "Aqua City",
                PostedAgoText = "3 giá» trÆ°á»›c",
                SellerName = "Äá»©c TrÆ°á»ng",
                SellerRole = "Khai thÃ¡c máº·t báº±ng",
                ThumbnailUrl = "/uploads/images/macdinh.jpg",
                Description = "Máº·t báº±ng phÃ¹ há»£p cafÃ©, showroom mini hoáº·c vÄƒn phÃ²ng giao dá»‹ch. ÄÃ¢y lÃ  dá»¯ liá»‡u máº«u cho nhÃ³m business-premises.",
                ContactPhone = "0981 345 678",
                Gallery = new List<string> { "/uploads/images/macdinh.jpg" }
            }
        };
    }
}
