using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public static class BatDongSanSearch_cl
{
    private sealed class LocationAlias
    {
        public string Token { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }

    private sealed class SearchIntent
    {
        public string CleanKeyword { get; set; }
        public string Purpose { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public decimal? AreaMin { get; set; }
        public decimal? AreaMax { get; set; }
    }

    public static BatDongSanService_cl.ListingQuery NormalizeQuery(BatDongSanService_cl.ListingQuery query)
    {
        query = query ?? new BatDongSanService_cl.ListingQuery();
        SearchIntent intent = ParseIntent(query.Keyword);
        bool hasInferredIntent =
            !string.IsNullOrWhiteSpace(intent.Purpose) ||
            !string.IsNullOrWhiteSpace(intent.Province) ||
            !string.IsNullOrWhiteSpace(intent.District) ||
            !string.IsNullOrWhiteSpace(intent.PropertyType) ||
            !string.IsNullOrWhiteSpace(intent.Bedrooms) ||
            intent.PriceMin.HasValue ||
            intent.PriceMax.HasValue ||
            intent.AreaMin.HasValue ||
            intent.AreaMax.HasValue;

        return new BatDongSanService_cl.ListingQuery
        {
            Purpose = PreferExplicit(query.Purpose, intent.Purpose),
            Keyword = ResolveKeyword(query.Keyword, intent.CleanKeyword, hasInferredIntent),
            Province = PreferExplicit(query.Province, intent.Province),
            District = PreferExplicit(query.District, intent.District),
            PropertyType = PreferExplicit(query.PropertyType, intent.PropertyType),
            Bedrooms = PreferExplicit(query.Bedrooms, intent.Bedrooms),
            LegalStatus = query.LegalStatus,
            FurnishingStatus = query.FurnishingStatus,
            Project = query.Project,
            PriceMin = query.PriceMin ?? intent.PriceMin,
            PriceMax = query.PriceMax ?? intent.PriceMax,
            AreaMin = query.AreaMin ?? intent.AreaMin,
            AreaMax = query.AreaMax ?? intent.AreaMax,
            HouseDirection = query.HouseDirection,
            Sort = query.Sort
        };
    }

    public static List<BatDongSanService_cl.ListingItem> QueryUnifiedListings(BatDongSanService_cl.ListingQuery query)
    {
        BatDongSanService_cl.ListingQuery normalized = NormalizeQuery(query);

        IEnumerable<BatDongSanService_cl.ListingItem> items = BatDongSanService_cl.QueryListings(normalized);
        items = items.Concat(GetLinkedListings());
        items = ApplyFilters(items, normalized);

        string keyword = (normalized.Keyword ?? "").Trim();
        if (keyword != "")
        {
            string slugKeyword = BatDongSanService_cl.Slugify(keyword).Replace("-", " ").Trim();
            items = items
                .Select(x => new { Item = x, Score = ComputeRelevanceScore(x, normalized, slugKeyword) })
                .OrderByDescending(x => x.Item.VisualPriority)
                .ThenByDescending(x => x.Score)
                .ThenByDescending(x => x.Item.Id)
                .Select(x => x.Item);
        }
        else
        {
            items = ApplySort(items, normalized.Sort);
        }

        return items
            .GroupBy(x => (x.IsLinked ? "linked:" : "native:") + x.Id.ToString(CultureInfo.InvariantCulture))
            .Select(g => g.First())
            .ToList();
    }

    public static List<BatDongSanService_cl.FilterOption> GetUnifiedProvinceOptions(string purpose)
    {
        return QueryUnifiedListings(new BatDongSanService_cl.ListingQuery { Purpose = purpose })
            .Where(x => !string.IsNullOrWhiteSpace(x.Province))
            .GroupBy(x => (x.Province ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => new BatDongSanService_cl.FilterOption
            {
                Label = g.Key,
                Value = BatDongSanService_cl.Slugify(g.Key),
                Note = g.Count().ToString("#,##0") + " tin"
            })
            .ToList();
    }

    public static List<BatDongSanService_cl.FilterOption> GetUnifiedDistrictOptions(string purpose, string province)
    {
        return QueryUnifiedListings(new BatDongSanService_cl.ListingQuery
            {
                Purpose = purpose,
                Province = (province ?? "").Trim()
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.District))
            .GroupBy(x => (x.District ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => new BatDongSanService_cl.FilterOption
            {
                Label = g.Key,
                Value = BatDongSanService_cl.Slugify(g.Key),
                Note = g.Count().ToString("#,##0") + " tin"
            })
            .ToList();
    }

    public static List<BatDongSanService_cl.FilterOption> GetUnifiedPropertyTypeOptions(string purpose, string province, string district)
    {
        return QueryUnifiedListings(new BatDongSanService_cl.ListingQuery
            {
                Purpose = purpose,
                Province = (province ?? "").Trim(),
                District = (district ?? "").Trim()
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.PropertyType))
            .GroupBy(x => (x.PropertyType ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g =>
            {
                BatDongSanService_cl.ListingItem first = g.FirstOrDefault();
                string label = first == null ? g.Key : (first.PropertyTypeLabel ?? "").Trim();
                if (label == "")
                    label = g.Key;
                return new BatDongSanService_cl.FilterOption
                {
                    Label = label,
                    Value = g.Key,
                    Note = g.Count().ToString("#,##0") + " tin"
                };
            })
            .ToList();
    }

    private static IEnumerable<BatDongSanService_cl.ListingItem> ApplyFilters(IEnumerable<BatDongSanService_cl.ListingItem> items, BatDongSanService_cl.ListingQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.Purpose))
            items = items.Where(p => string.Equals(p.Purpose, query.Purpose, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(query.Province))
            items = items.Where(p => BatDongSanService_cl.Slugify(p.Province) == query.Province.Trim().ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(query.District))
            items = items.Where(p => BatDongSanService_cl.Slugify(p.District) == query.District.Trim().ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(query.PropertyType))
            items = items.Where(p => BatDongSanService_cl.PropertyTypeMatches(p, query.PropertyType));

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

        return items;
    }

    private static IEnumerable<BatDongSanService_cl.ListingItem> ApplySort(IEnumerable<BatDongSanService_cl.ListingItem> items, string sort)
    {
        switch ((sort ?? "").Trim().ToLowerInvariant())
        {
            case "price_asc":
                return items.OrderByDescending(p => p.VisualPriority).ThenBy(p => p.PriceValue <= 0 ? decimal.MaxValue : p.PriceValue).ThenByDescending(p => p.Id);
            case "price_desc":
                return items.OrderByDescending(p => p.VisualPriority).ThenByDescending(p => p.PriceValue).ThenByDescending(p => p.Id);
            case "area_desc":
                return items.OrderByDescending(p => p.VisualPriority).ThenByDescending(p => p.AreaValue).ThenByDescending(p => p.Id);
            default:
                return items.OrderByDescending(p => p.VisualPriority).ThenByDescending(p => p.Id);
        }
    }

    private static int ComputeRelevanceScore(BatDongSanService_cl.ListingItem item, BatDongSanService_cl.ListingQuery query, string slugKeyword)
    {
        int score = item.IsLinked ? 0 : 20;
        string title = BatDongSanService_cl.Slugify(item.Title).Replace("-", " ");
        string project = BatDongSanService_cl.Slugify(item.ProjectName).Replace("-", " ");
        string address = BatDongSanService_cl.Slugify(item.AddressText).Replace("-", " ");
        string district = BatDongSanService_cl.Slugify(item.District).Replace("-", " ");
        string province = BatDongSanService_cl.Slugify(item.Province).Replace("-", " ");
        string propertyType = (item.PropertyType ?? "").Trim().ToLowerInvariant();
        bool isDongNai = province == "dong nai";
        bool isBienHoa = district == "bien hoa";

        if (slugKeyword != "")
        {
            if (title.Contains(slugKeyword)) score += 80;
            if (project.Contains(slugKeyword)) score += 40;
            if (address.Contains(slugKeyword)) score += 25;
            if (district.Contains(slugKeyword) || province.Contains(slugKeyword)) score += 20;
        }

        if (!string.IsNullOrWhiteSpace(query.Purpose) && string.Equals((item.Purpose ?? "").Trim(), query.Purpose.Trim(), StringComparison.OrdinalIgnoreCase))
            score += 24;
        if (!string.IsNullOrWhiteSpace(query.Province) && BatDongSanService_cl.Slugify(item.Province) == query.Province)
            score += 48;
        if (!string.IsNullOrWhiteSpace(query.District) && BatDongSanService_cl.Slugify(item.District) == query.District)
            score += 44;
        if (!string.IsNullOrWhiteSpace(query.PropertyType) && BatDongSanService_cl.PropertyTypeMatches(item, query.PropertyType))
            score += 36;

        if (query.Province == "dong-nai" && isDongNai)
            score += 35;
        if (query.District == "bien-hoa" && isBienHoa)
            score += 35;
        if (query.Province == "dong-nai" && isBienHoa)
            score += 10;

        if (query.PropertyType == "land" && title.Contains("dat"))
            score += 12;
        if (query.PropertyType == "warehouse" && (title.Contains("kho") || title.Contains("xuong")))
            score += 12;
        if (query.PropertyType == "business-premises" && (title.Contains("mat bang") || title.Contains("kiot") || title.Contains("shop")))
            score += 12;
        if (query.PropertyType == "boarding-house" && (title.Contains("phong tro") || title.Contains("nha tro")))
            score += 12;
        if (query.PropertyType == "house" && (title.Contains("nha") || title.Contains("biet thu") || title.Contains("villa")))
            score += 10;

        if (item.PriceValue > 0) score += 8;
        if (item.AreaValue > 0) score += 6;
        score += item.VisualPriority * 8;
        if (item.BedroomCount > 0) score += 3;
        if (item.IsLinked && isDongNai) score += 8;
        if (item.IsLinked && isBienHoa) score += 8;

        return score;
    }

    private static List<BatDongSanService_cl.ListingItem> GetLinkedListings()
    {
        using (dbDataContext db = new dbDataContext())
        {
            LinkedFeedStore_cl.EnsureSchema(db);
            List<LinkedFeedStore_cl.LinkedPost> rows = LinkedFeedStore_cl.GetActiveForSearch(db, 5000);
            return rows
                .Where(HasUsableLinkedImage)
                .Select(MapLinkedToListing)
                .Where(x => x != null)
                .ToList();
        }
    }

    private static bool HasUsableLinkedImage(LinkedFeedStore_cl.LinkedPost row)
    {
        if (row == null)
            return false;
        return BatDongSanService_cl.ResolveLinkedRawGalleryUrls(row.ThumbnailUrl, row.GalleryCsv).Count > 0;
    }

    private static BatDongSanService_cl.ListingItem MapLinkedToListing(LinkedFeedStore_cl.LinkedPost row)
    {
        if (row == null || string.IsNullOrWhiteSpace(row.Title))
            return null;

        string purpose = InferPurpose(row);
        string propertyType = InferPropertyType(row);
        decimal areaValue = ParseAreaText(row.AreaText);
        decimal priceValue = ParsePriceText(row.PriceText, purpose);
        int bedrooms = InferBedrooms(row);
        string district = (row.District ?? "").Trim();
        string province = (row.Province ?? "").Trim();

        return new BatDongSanService_cl.ListingItem
        {
            Id = row.Id,
            Slug = BatDongSanService_cl.Slugify(row.Title),
            Title = row.Title.Trim(),
            Purpose = purpose,
            PropertyType = propertyType,
            PropertyTypeLabel = BuildPropertyTypeLabel(propertyType),
            Province = province,
            District = district,
            Ward = "",
            AddressText = BuildAddressText(district, province),
            PriceValue = priceValue,
            PriceText = string.IsNullOrWhiteSpace(row.PriceText) ? "Liên hệ" : row.PriceText.Trim(),
            AreaValue = areaValue,
            DepositAmount = 0,
            RentalTermMonths = 0,
            FloorCount = 0,
            LandWidth = 0,
            LandLength = 0,
            LegalStatus = "Theo nguồn liên kết",
            FurnishingStatus = "Theo nguồn liên kết",
            BedroomCount = bedrooms,
            BathroomCount = 0,
            HouseDirection = "",
            ProjectName = "",
            PostedAgoText = BuildPostedAgoText(row.PublishedAt),
            SellerName = BatDongSanService_cl.ResolveLinkedSourceLabel(row.Source),
            SellerRole = "Tin liên kết",
            ThumbnailUrl = BatDongSanService_cl.BuildLinkedImageProxyUrl(row.Id, 0),
            VisualPriority = BatDongSanService_cl.ComputeLinkedVisualPriority(row.ThumbnailUrl, row.GalleryCsv),
            Description = BuildLinkedDescription(row),
            ContactPhone = "",
            Gallery = BuildLinkedProxyGallery(row),
            IsLinked = true,
            LinkedSource = BatDongSanService_cl.ResolveLinkedSourceLabel(row.Source),
            LinkedSourceUrl = row.SourceUrl
        };
    }

    private static List<string> BuildLinkedProxyGallery(LinkedFeedStore_cl.LinkedPost row)
    {
        if (row == null)
            return new List<string> { BatDongSanService_cl.DefaultBdsFallbackImage };

        List<string> raw = BatDongSanService_cl.ResolveLinkedRawGalleryUrls(row.ThumbnailUrl, row.GalleryCsv);
        if (raw.Count == 0)
            return new List<string> { BatDongSanService_cl.DefaultBdsFallbackImage };

        return raw
            .Take(8)
            .Select((x, index) => BatDongSanService_cl.BuildLinkedImageProxyUrl(row.Id, index))
            .ToList();
    }

    private static string BuildLinkedDescription(LinkedFeedStore_cl.LinkedPost row)
    {
        string summary = BatDongSanService_cl.SanitizeLinkedText(row.Summary ?? "", 600, true);
        if (summary != "")
            return summary;

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(row.Title))
            parts.Add(row.Title.Trim());
        if (!string.IsNullOrWhiteSpace(row.PriceText))
            parts.Add("Giá: " + row.PriceText.Trim());
        if (!string.IsNullOrWhiteSpace(row.AreaText))
            parts.Add("Diện tích: " + row.AreaText.Trim());

        string location = BuildAddressText((row.District ?? "").Trim(), (row.Province ?? "").Trim());
        if (location != "")
            parts.Add("Khu vực: " + location);

        string sourceLabel = BatDongSanService_cl.ResolveLinkedSourceLabel(row.Source);
        if (!string.IsNullOrWhiteSpace(sourceLabel))
            parts.Add("Nguồn: " + sourceLabel);

        if (parts.Count == 0)
            return "Tin liên kết được đồng bộ từ nguồn ngoài và lọc theo logic bất động sản của AhaLand.";

        return string.Join(". ", parts.ToArray()) + ".";
    }

    private static string InferPurpose(LinkedFeedStore_cl.LinkedPost row)
    {
        string purpose = (row.Purpose ?? "").Trim().ToLowerInvariant();
        if (purpose == "rent" || purpose == "sale")
            return purpose;

        string text = BatDongSanService_cl.Slugify((row.Title ?? "") + " " + (row.Summary ?? ""));
        if (text.Contains("cho-thue") || Regex.IsMatch(text, @"(^| )thue( |$)"))
            return "rent";
        return "sale";
    }

    private static string InferPropertyType(LinkedFeedStore_cl.LinkedPost row)
    {
        string raw = (row.PropertyType ?? "").Trim().ToLowerInvariant();
        string text = BatDongSanService_cl.Slugify((row.Title ?? "") + " " + (row.Summary ?? ""));
        string fromText = InferPropertyTypeFromText(text);
        string fromRaw = raw == "" ? "" : BatDongSanMetadata_cl.NormalizePropertyType(raw);

        if (fromText != "")
        {
            if (fromRaw == "")
                return fromText;

            if (fromRaw == "land" && fromText != "land")
                return fromText;

            if ((fromRaw == "apartment" || fromRaw == "house") && fromText == "business-premises")
                return fromText;

            if (fromRaw == "apartment" && fromText == "house")
                return fromText;

            return fromRaw;
        }

        if (fromRaw != "")
            return fromRaw;

        return "house";
    }

    private static string InferPropertyTypeFromText(string text)
    {
        string normalized = (text ?? "").Trim();
        if (normalized == "")
            return "";

        if (normalized.Contains("can-ho") || normalized.Contains("chung-cu") || normalized.Contains("studio") || normalized.Contains("penthouse") || normalized.Contains("duplex"))
            return "apartment";
        if (normalized.Contains("van-phong"))
            return "office";
        if (normalized.Contains("mat-bang") || normalized.Contains("shophouse") || normalized.Contains("shop") || normalized.Contains("kiot") || normalized.Contains("kiosk") || normalized.Contains("cua-hang"))
            return "business-premises";
        if (normalized.Contains("kho-xuong") || normalized.Contains("nha-xuong") || normalized.Contains("kho-bai") || normalized.Contains("kho") || normalized.Contains("xuong"))
            return "warehouse";
        if (normalized.Contains("phong-tro") || normalized.Contains("nha-tro") || normalized.Contains("phong-cho-thue"))
            return "boarding-house";
        if (normalized.Contains("biet-thu") || normalized.Contains("villa") || normalized.Contains("nha-pho") || normalized.Contains("nha-rieng") || normalized.Contains("nha-nguyen-can") || Regex.IsMatch(normalized, @"(^|-)nha(-|$)"))
            return "house";
        if (normalized.Contains("dat-nen") || normalized.Contains("dat-tho-cu") || normalized.Contains("dat-sao") || normalized.Contains("dat-vuon") || normalized.Contains("dat-cong-nghiep") || Regex.IsMatch(normalized, @"(^|-)dat(-|$)"))
            return "land";

        return "";
    }

    private static int InferBedrooms(LinkedFeedStore_cl.LinkedPost row)
    {
        string text = ((row.Title ?? "") + " " + (row.Summary ?? "")).ToLowerInvariant();
        Match m = Regex.Match(text, @"(\d+)\s*(pn|phong ngu|phòng ngủ)");
        int value;
        if (m.Success && int.TryParse(m.Groups[1].Value, out value))
            return value;
        return 0;
    }

    private static decimal ParseAreaText(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        Match m = Regex.Match(value, @"(\d+(?:[\.,]\d+)?)");
        if (!m.Success)
            return 0;
        decimal result;
        if (!decimal.TryParse(m.Groups[1].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            return 0;
        return result < 0 ? 0 : result;
    }

    private static decimal ParsePriceText(string raw, string purpose)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == "")
            return 0;

        Match m = Regex.Match(value, @"(\d+(?:[\.,]\d+)?)");
        if (!m.Success)
            return 0;

        decimal number;
        if (!decimal.TryParse(m.Groups[1].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
            return 0;

        if (value.Contains("ty"))
            return number * 1000000000m;
        if (value.Contains("trieu"))
            return number * 1000000m;
        if (value.Contains("nghin"))
            return number * 1000m;
        return number;
    }

    private static string BuildPropertyTypeLabel(string propertyType)
    {
        switch ((propertyType ?? "").Trim().ToLowerInvariant())
        {
            case "house":
                return "Nhà phố";
            case "land":
                return "Đất nền";
            case "office":
                return "Văn phòng";
            case "business-premises":
                return "Mặt bằng";
            case "townhouse":
                return "Nhà liền kề";
            case "warehouse":
                return "Kho/Xưởng";
            case "boarding-house":
                return "Phòng trọ";
            default:
                return "Căn hộ";
        }
    }

    private static string BuildAddressText(string district, string province)
    {
        if (district != "" && province != "")
            return district + ", " + province;
        if (province != "")
            return province;
        return "Theo nguồn liên kết";
    }

    private static string BuildPostedAgoText(DateTime date)
    {
        TimeSpan diff = AhaTime_cl.Now - date;
        if (diff.TotalMinutes < 60)
            return Math.Max(1, (int)diff.TotalMinutes).ToString(CultureInfo.InvariantCulture) + " phút trước";
        if (diff.TotalHours < 24)
            return Math.Max(1, (int)diff.TotalHours).ToString(CultureInfo.InvariantCulture) + " giờ trước";
        return Math.Max(1, (int)diff.TotalDays).ToString(CultureInfo.InvariantCulture) + " ngày trước";
    }

    private static SearchIntent ParseIntent(string keyword)
    {
        SearchIntent intent = new SearchIntent();
        string raw = (keyword ?? "").Trim();
        if (raw == "")
        {
            intent.CleanKeyword = "";
            return intent;
        }

        string normalized = " " + BatDongSanService_cl.Slugify(raw).Replace("-", " ") + " ";
        normalized = SimplifyAdministrativePrefixes(normalized);
        string working = normalized;

        if (ContainsAny(working, " cho thue ", " thue "))
        {
            intent.Purpose = "rent";
            working = ReplaceAny(working, " cho thue ", " thue ");
        }
        else if (ContainsAny(working, " mua ban ", " ban "))
        {
            intent.Purpose = "sale";
            working = ReplaceAny(working, " mua ban ", " ban ");
        }

        if (ContainsAny(working, " can ho ", " chung cu ", " studio ", " penthouse ", " duplex "))
        {
            intent.PropertyType = "apartment";
            working = ReplaceAny(working, " can ho ", " chung cu ", " studio ", " penthouse ", " duplex ");
        }
        else if (ContainsAny(working, " nha pho ", " nha rieng ", " biet thu ", " nha nguyen can ", " nha cap 4 ", " nha ", " villa "))
        {
            intent.PropertyType = "house";
            working = ReplaceAny(working, " nha pho ", " nha rieng ", " biet thu ", " nha nguyen can ", " nha cap 4 ", " nha ", " villa ");
        }
        else if (ContainsAny(working, " dat nen ", " dat tho cu ", " dat sao ", " dat vuon ", " dat mau ", " dat cong nghiep ", " dat "))
        {
            intent.PropertyType = "land";
            working = ReplaceAny(working, " dat nen ", " dat tho cu ", " dat sao ", " dat vuon ", " dat mau ", " dat cong nghiep ", " dat ");
        }
        else if (ContainsAny(working, " van phong "))
        {
            intent.PropertyType = "office";
            working = ReplaceAny(working, " van phong ");
        }
        else if (ContainsAny(working, " mat bang ", " shophouse ", " shop ", " kiot ", " kiosk ", " cua hang "))
        {
            intent.PropertyType = "business-premises";
            working = ReplaceAny(working, " mat bang ", " shophouse ", " shop ", " kiot ", " kiosk ", " cua hang ");
        }
        else if (ContainsAny(working, " kho xuong ", " nha xuong ", " kho bai ", " kho ", " xuong "))
        {
            intent.PropertyType = "warehouse";
            working = ReplaceAny(working, " kho xuong ", " nha xuong ", " kho bai ", " kho ", " xuong ");
        }
        else if (ContainsAny(working, " phong tro ", " nha tro ", " phong cho thue ", " phong "))
        {
            intent.PropertyType = "boarding-house";
            working = ReplaceAny(working, " phong tro ", " nha tro ", " phong cho thue ", " phong ");
        }

        Match bedroomMatch = Regex.Match(working, @"(\d+)\s*(pn|phong ngu)");
        if (bedroomMatch.Success)
        {
            intent.Bedrooms = bedroomMatch.Groups[1].Value;
            working = working.Replace(bedroomMatch.Value, " ");
        }

        ParsePriceIntent(ref working, intent);
        ParseAreaIntent(ref working, intent);
        ParseLocationIntent(ref working, intent);

        string clean = Regex.Replace(working, @"\s+", " ").Trim();
        intent.CleanKeyword = clean;
        return intent;
    }

    private static void ParsePriceIntent(ref string working, SearchIntent intent)
    {
        working = NormalizePriceTokens(working);

        Match compactRange = Regex.Match(working, @"(\d+(?:[\.,]\d+)?)\s*(ty|trieu|nghin)\s+(?:den|toi|\-)\s+(\d+(?:[\.,]\d+)?)\s*(ty|trieu|nghin)");
        if (compactRange.Success)
        {
            intent.PriceMin = ConvertMoney(compactRange.Groups[1].Value, compactRange.Groups[2].Value);
            intent.PriceMax = ConvertMoney(compactRange.Groups[3].Value, compactRange.Groups[4].Value);
            working = working.Replace(compactRange.Value, " ");
            return;
        }

        Match between = Regex.Match(working, @"(?:tu|tren)\s+(\d+(?:[\.,]\d+)?)\s*(ty|trieu|nghin)?\s+(?:den|toi)\s+(\d+(?:[\.,]\d+)?)\s*(ty|trieu|nghin)?");
        if (between.Success)
        {
            intent.PriceMin = ConvertMoney(between.Groups[1].Value, between.Groups[2].Value);
            intent.PriceMax = ConvertMoney(between.Groups[3].Value, between.Groups[4].Value);
            working = working.Replace(between.Value, " ");
            return;
        }

        Match exact = Regex.Match(working, @"(\d+(?:[\.,]\d+)?)\s*(ty|trieu|nghin)");
        if (exact.Success)
        {
            decimal exactValue = ConvertMoney(exact.Groups[1].Value, exact.Groups[2].Value);
            if (exactValue > 0)
            {
                intent.PriceMin = exactValue;
                intent.PriceMax = exactValue;
                working = working.Replace(exact.Value, " ");
                return;
            }
        }

        Match max = Regex.Match(working, @"(?:duoi|toi da|khong qua)\s+(\d+(?:[\.,]\d+)?)\s*(ty|trieu|nghin)?");
        if (max.Success)
        {
            intent.PriceMax = ConvertMoney(max.Groups[1].Value, max.Groups[2].Value);
            working = working.Replace(max.Value, " ");
            return;
        }

        Match min = Regex.Match(working, @"(?:tren|tu)\s+(\d+(?:[\.,]\d+)?)\s*(ty|trieu|nghin)?");
        if (min.Success)
        {
            intent.PriceMin = ConvertMoney(min.Groups[1].Value, min.Groups[2].Value);
            working = working.Replace(min.Value, " ");
        }
    }

    private static void ParseAreaIntent(ref string working, SearchIntent intent)
    {
        working = NormalizeAreaTokens(working);

        Match dimensions = Regex.Match(working, @"(\d+(?:[\.,]\d+)?)\s*x\s*(\d+(?:[\.,]\d+)?)");
        if (dimensions.Success)
        {
            decimal width = ConvertDecimal(dimensions.Groups[1].Value);
            decimal length = ConvertDecimal(dimensions.Groups[2].Value);
            decimal area = width * length;
            if (area > 0)
            {
                intent.AreaMin = area;
                intent.AreaMax = area;
                working = working.Replace(dimensions.Value, " ");
                return;
            }
        }

        Match widthLength = Regex.Match(working, @"ngang\s*(\d+(?:[\.,]\d+)?)\s*(?:m|met)?\s*dai\s*(\d+(?:[\.,]\d+)?)\s*(?:m|met)?");
        if (widthLength.Success)
        {
            decimal width = ConvertDecimal(widthLength.Groups[1].Value);
            decimal length = ConvertDecimal(widthLength.Groups[2].Value);
            decimal area = width * length;
            if (area > 0)
            {
                intent.AreaMin = area;
                intent.AreaMax = area;
                working = working.Replace(widthLength.Value, " ");
                return;
            }
        }

        Match between = Regex.Match(working, @"(?:tu)\s+(\d+(?:[\.,]\d+)?)\s*m2?\s+(?:den|toi)\s+(\d+(?:[\.,]\d+)?)\s*m2?");
        if (between.Success)
        {
            intent.AreaMin = ConvertDecimal(between.Groups[1].Value);
            intent.AreaMax = ConvertDecimal(between.Groups[2].Value);
            working = working.Replace(between.Value, " ");
            return;
        }

        Match max = Regex.Match(working, @"(?:duoi|nho hon)\s+(\d+(?:[\.,]\d+)?)\s*m2?");
        if (max.Success)
        {
            intent.AreaMax = ConvertDecimal(max.Groups[1].Value);
            working = working.Replace(max.Value, " ");
            return;
        }

        Match min = Regex.Match(working, @"(?:tren)\s+(\d+(?:[\.,]\d+)?)\s*m2?");
        if (min.Success)
        {
            intent.AreaMin = ConvertDecimal(min.Groups[1].Value);
            working = working.Replace(min.Value, " ");
            return;
        }

        Match exact = Regex.Match(working, @"(\d+(?:[\.,]\d+)?)\s*(m2|m|met)");
        if (exact.Success)
        {
            decimal area = ConvertDecimal(exact.Groups[1].Value);
            if (area > 0)
            {
                intent.AreaMin = area;
                intent.AreaMax = area;
                working = working.Replace(exact.Value, " ");
            }
        }
    }

    private static void ParseLocationIntent(ref string working, SearchIntent intent)
    {
        foreach (LocationAlias alias in GetPriorityLocationAliases())
        {
            string token = " " + BatDongSanService_cl.Slugify(alias.Token).Replace("-", " ") + " ";
            if (!working.Contains(token))
                continue;

            if (string.IsNullOrWhiteSpace(intent.Province) && !string.IsNullOrWhiteSpace(alias.Province))
                intent.Province = BatDongSanService_cl.Slugify(alias.Province);
            if (string.IsNullOrWhiteSpace(intent.District) && !string.IsNullOrWhiteSpace(alias.District))
                intent.District = BatDongSanService_cl.Slugify(alias.District);

            working = working.Replace(token, " ");
        }

        foreach (string province in GetProvinceCandidates())
        {
            string token = " " + BatDongSanService_cl.Slugify(province).Replace("-", " ") + " ";
            if (!working.Contains(token))
                continue;
            intent.Province = BatDongSanService_cl.Slugify(province);
            working = working.Replace(token, " ");
            break;
        }

        foreach (string district in GetDistrictCandidates())
        {
            string token = " " + BatDongSanService_cl.Slugify(district).Replace("-", " ") + " ";
            if (!working.Contains(token))
                continue;
            intent.District = BatDongSanService_cl.Slugify(district);
            working = working.Replace(token, " ");
            break;
        }

        if (string.IsNullOrWhiteSpace(intent.Province) && !string.IsNullOrWhiteSpace(intent.District))
        {
            string inferredProvince = InferProvinceFromDistrict(intent.District);
            if (!string.IsNullOrWhiteSpace(inferredProvince))
                intent.Province = inferredProvince;
        }
    }

    private static IEnumerable<string> GetProvinceCandidates()
    {
        HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (BatDongSanService_cl.ListingItem item in BatDongSanService_cl.QueryListings(new BatDongSanService_cl.ListingQuery()))
        {
            if (!string.IsNullOrWhiteSpace(item.Province))
                names.Add(item.Province.Trim());
        }

        using (dbDataContext db = new dbDataContext())
        {
            foreach (LinkedFeedStore_cl.LinkedPost row in LinkedFeedStore_cl.GetActiveForSearch(db, 5000))
            {
                if (!string.IsNullOrWhiteSpace(row.Province))
                    names.Add(row.Province.Trim());
            }
        }

        names.Add("Đồng Nai");
        names.Add("TP.HCM");
        names.Add("Hà Nội");
        return names.OrderByDescending(x => x.Length).ToList();
    }

    private static IEnumerable<string> GetDistrictCandidates()
    {
        HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (BatDongSanService_cl.ListingItem item in BatDongSanService_cl.QueryListings(new BatDongSanService_cl.ListingQuery()))
        {
            if (!string.IsNullOrWhiteSpace(item.District))
                names.Add(item.District.Trim());
        }

        using (dbDataContext db = new dbDataContext())
        {
            foreach (LinkedFeedStore_cl.LinkedPost row in LinkedFeedStore_cl.GetActiveForSearch(db, 5000))
            {
                if (!string.IsNullOrWhiteSpace(row.District))
                    names.Add(row.District.Trim());
            }
        }

        names.Add("Biên Hòa");
        names.Add("Long Thành");
        names.Add("Nhơn Trạch");
        names.Add("Trảng Bom");
        names.Add("Vĩnh Cửu");
        names.Add("Cẩm Mỹ");
        names.Add("Xuân Lộc");
        names.Add("Định Quán");
        names.Add("Tân Phú");
        names.Add("Thống Nhất");
        names.Add("Long Khánh");
        return names.OrderByDescending(x => x.Length).ToList();
    }

    private static IEnumerable<LocationAlias> GetPriorityLocationAliases()
    {
        return new[]
        {
            new LocationAlias { Token = "TP.HCM", Province = "TP.HCM", District = "" },
            new LocationAlias { Token = "TPHCM", Province = "TP.HCM", District = "" },
            new LocationAlias { Token = "Hồ Chí Minh", Province = "TP.HCM", District = "" },
            new LocationAlias { Token = "Ho Chi Minh", Province = "TP.HCM", District = "" },
            new LocationAlias { Token = "Sài Gòn", Province = "TP.HCM", District = "" },
            new LocationAlias { Token = "Sai Gon", Province = "TP.HCM", District = "" },
            new LocationAlias { Token = "HCM", Province = "TP.HCM", District = "" },
            new LocationAlias { Token = "TP Biên Hòa", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "thanh pho bien hoa", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Biên Hòa", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Long Thành", Province = "Đồng Nai", District = "Long Thành" },
            new LocationAlias { Token = "Nhơn Trạch", Province = "Đồng Nai", District = "Nhơn Trạch" },
            new LocationAlias { Token = "Trảng Bom", Province = "Đồng Nai", District = "Trảng Bom" },
            new LocationAlias { Token = "Vĩnh Cửu", Province = "Đồng Nai", District = "Vĩnh Cửu" },
            new LocationAlias { Token = "Cẩm Mỹ", Province = "Đồng Nai", District = "Cẩm Mỹ" },
            new LocationAlias { Token = "Xuân Lộc", Province = "Đồng Nai", District = "Xuân Lộc" },
            new LocationAlias { Token = "Định Quán", Province = "Đồng Nai", District = "Định Quán" },
            new LocationAlias { Token = "Tân Phú", Province = "Đồng Nai", District = "Tân Phú" },
            new LocationAlias { Token = "Thống Nhất", Province = "Đồng Nai", District = "Thống Nhất" },
            new LocationAlias { Token = "Long Khánh", Province = "Đồng Nai", District = "Long Khánh" },
            new LocationAlias { Token = "Trảng Dài", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Tam Phước", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Tân Hiệp", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Long Bình Tân", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Hóa An", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "An Bình", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Phước Tân", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Bửu Long", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Hiệp Hòa", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Hố Nai", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Long Bình", Province = "Đồng Nai", District = "Biên Hòa" },
            new LocationAlias { Token = "Sông Trầu", Province = "Đồng Nai", District = "Trảng Bom" },
            new LocationAlias { Token = "Phước An", Province = "Đồng Nai", District = "Nhơn Trạch" },
            new LocationAlias { Token = "Long Đức", Province = "Đồng Nai", District = "Long Thành" }
        }
        .OrderByDescending(x => (x.Token ?? "").Length)
        .ToList();
    }

    private static string InferProvinceFromDistrict(string districtSlug)
    {
        switch ((districtSlug ?? "").Trim().ToLowerInvariant())
        {
            case "bien-hoa":
            case "long-thanh":
            case "nhon-trach":
            case "trang-bom":
            case "vinh-cuu":
            case "cam-my":
            case "xuan-loc":
            case "dinh-quan":
            case "tan-phu":
            case "thong-nhat":
            case "long-khanh":
                return "dong-nai";
            default:
                return "";
        }
    }

    private static bool ContainsAny(string value, params string[] tokens)
    {
        return tokens.Any(t => value.Contains(t));
    }

    private static string ReplaceAny(string value, params string[] tokens)
    {
        string output = value;
        foreach (string token in tokens)
            output = output.Replace(token, " ");
        return output;
    }

    private static decimal ConvertMoney(string numberRaw, string unitRaw)
    {
        decimal number = ConvertDecimal(numberRaw);
        string unit = (unitRaw ?? "").Trim().ToLowerInvariant();
        if (unit == "ty")
            return number * 1000000000m;
        if (unit == "trieu")
            return number * 1000000m;
        if (unit == "nghin")
            return number * 1000m;
        return number;
    }

    private static string NormalizePriceTokens(string value)
    {
        string output = value ?? "";
        output = Regex.Replace(output, @"(\d+)\s*tr\/thang", "$1 trieu ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+)\s*tr\/th", "$1 trieu ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+)\s*trieu\/thang", "$1 trieu ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+)\s*trieu\/th", "$1 trieu ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*tr\b", "$1 trieu", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*ty\b", "$1 ty", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*k\b", "$1 nghin", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+)\s*ty\s+(\d{1,3})(?!\d)", delegate (Match m)
        {
            string tail = m.Groups[2].Value;
            string number = tail.Length >= 3 ? m.Groups[1].Value + "." + tail : m.Groups[1].Value + "." + tail.PadLeft(3, '0');
            return number + " ty";
        }, RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"\s+", " ").Trim();
        return " " + output + " ";
    }

    private static string NormalizeAreaTokens(string value)
    {
        string output = value ?? "";
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*met vuong", "$1 m2", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*met\b", "$1 m", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*m²", "$1 m2", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*m2", "$1 m2", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"(\d+(?:[\.,]\d+)?)\s*x\s*(\d+(?:[\.,]\d+)?)", "$1 x $2", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"\s+", " ").Trim();
        return " " + output + " ";
    }

    private static decimal ConvertDecimal(string raw)
    {
        decimal value;
        if (!decimal.TryParse((raw ?? "").Trim().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            return 0;
        return value < 0 ? 0 : value;
    }

    private static string SimplifyAdministrativePrefixes(string value)
    {
        string output = value ?? "";
        output = Regex.Replace(output, @"\b(tp|tp\.|thanh pho)\s+", " ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"\b(q|q\.|quan)\s+", " ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"\b(h|h\.|huyen)\s+", " ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"\b(p|p\.|phuong)\s+", " ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"\b(x|x\.|xa)\s+", " ", RegexOptions.IgnoreCase);
        output = Regex.Replace(output, @"\s+", " ").Trim();
        return " " + output + " ";
    }

    private static string PreferExplicit(string explicitValue, string inferredValue)
    {
        string explicitNormalized = (explicitValue ?? "").Trim();
        if (explicitNormalized != "")
            return explicitNormalized;
        return (inferredValue ?? "").Trim();
    }

    private static string ResolveKeyword(string rawKeyword, string cleanedKeyword, bool hasInferredIntent)
    {
        string cleaned = (cleanedKeyword ?? "").Trim();
        if (cleaned != "")
            return cleaned;
        if (hasInferredIntent)
            return "";
        return (rawKeyword ?? "").Trim();
    }
}
