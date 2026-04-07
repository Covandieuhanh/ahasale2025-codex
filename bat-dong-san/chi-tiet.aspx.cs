using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;

public partial class bat_dong_san_chi_tiet : BatDongSanPageBase
{
    protected BatDongSanService_cl.ListingItem Listing;

    protected void Page_Load(object sender, EventArgs e)
    {
        int id;
        if (!int.TryParse(Request.QueryString["id"], out id))
            id = 0;

        Listing = BatDongSanService_cl.GetListingById(id);
        if (Listing == null)
        {
            int linkedId;
            if (int.TryParse(Request.QueryString["linkedId"], out linkedId) && linkedId > 0)
                Listing = BuildFromLinked(linkedId);
        }

        phDetail.Visible = Listing != null;
        phEmpty.Visible = Listing == null;
        phRentFacts.Visible = Listing != null && string.Equals(Listing.Purpose, "rent", StringComparison.OrdinalIgnoreCase);
        phHouseFacts.Visible = Listing != null && string.Equals(Listing.PropertyType, "house", StringComparison.OrdinalIgnoreCase);
        phLandFacts.Visible = Listing != null && string.Equals(Listing.PropertyType, "land", StringComparison.OrdinalIgnoreCase);
        phProject.Visible = Listing != null && !string.IsNullOrWhiteSpace(Listing.ProjectName);
        phSourceAction.Visible = Listing != null && Listing.IsLinked && !string.IsNullOrWhiteSpace(Listing.LinkedSourceUrl);
        phCallAction.Visible = Listing != null && !Listing.IsLinked && !string.IsNullOrWhiteSpace(Listing.ContactPhone);
        phSellerRole.Visible = Listing != null && !string.IsNullOrWhiteSpace(Listing.SellerRole);

        phFactArea.Visible = Listing != null && Listing.AreaValue > 0;
        phFactUnitPrice.Visible = Listing != null && Listing.AreaValue > 0 && Listing.PriceValue > 0;
        phFactBedrooms.Visible = Listing != null && Listing.BedroomCount > 0;
        phFactBathrooms.Visible = Listing != null && Listing.BathroomCount > 0;
        phFactLegal.Visible = Listing != null && !string.IsNullOrWhiteSpace(Listing.LegalStatus);
        phFactFurnishing.Visible = Listing != null && !string.IsNullOrWhiteSpace(Listing.FurnishingStatus);
        phFactProject.Visible = Listing != null && !string.IsNullOrWhiteSpace(Listing.ProjectName);
        phFactDirection.Visible = Listing != null && !string.IsNullOrWhiteSpace(Listing.HouseDirection);

        if (Listing != null && !IsPostBack)
        {
            rptGallery.DataSource = Listing.Gallery ?? new List<string>();
            rptGallery.DataBind();

            var similar = BatDongSanService_cl.GetSimilarListings(Listing, 6);
            phSimilar.Visible = similar.Count > 0;
            rptSimilar.DataSource = similar;
            rptSimilar.DataBind();

            if (!string.IsNullOrWhiteSpace(Listing.ProjectName))
            {
                var sameProject = BatDongSanService_cl.GetListingsByProject(Listing.ProjectName, 6)
                    .Where(p => p.Id != Listing.Id)
                    .ToList();
                phProjectListings.Visible = sameProject.Count > 0;
                rptProjectListings.DataSource = sameProject;
                rptProjectListings.DataBind();
            }

            rptRelated.DataSource = BatDongSanService_cl.GetRelatedListings(Listing, 4);
            rptRelated.DataBind();
        }
    }

    protected string ResolveMainImage()
    {
        if (Listing == null)
            return BatDongSanService_cl.DefaultBdsFallbackImage;
        if (Listing.Gallery != null && Listing.Gallery.Count > 0)
            return Listing.Gallery[0];
        return string.IsNullOrWhiteSpace(Listing.ThumbnailUrl) ? BatDongSanService_cl.DefaultBdsFallbackImage : Listing.ThumbnailUrl;
    }

    protected string BuildPurposeLabel()
    {
        if (Listing == null)
            return "";
        return BatDongSanUiHelper_cl.BuildPurposeLabel(Listing.Purpose);
    }

    protected string BuildPurposeBadgeCss()
    {
        if (Listing == null)
            return "bg-secondary-lt text-secondary";
        return BatDongSanUiHelper_cl.BuildPurposeBadgeCss(Listing.Purpose);
    }

    protected string BuildCallHref()
    {
        if (Listing == null || Listing.IsLinked || string.IsNullOrWhiteSpace(Listing.ContactPhone))
            return "#";

        return "tel:" + HttpUtility.HtmlEncode(Listing.ContactPhone);
    }

    protected string ResolveBackToListingUrl()
    {
        if (Listing == null)
            return "/bat-dong-san";

        return string.Equals(Listing.Purpose, "rent", StringComparison.OrdinalIgnoreCase)
            ? "/bat-dong-san/cho-thue.aspx"
            : "/bat-dong-san/mua-ban.aspx";
    }

    protected string ResolveSourceUrl()
    {
        if (Listing == null || string.IsNullOrWhiteSpace(Listing.LinkedSourceUrl))
            return "#";
        return Listing.LinkedSourceUrl;
    }

    protected string RenderSourceChip()
    {
        if (Listing == null)
            return "";

        if (Listing.IsLinked)
        {
            string label = BatDongSanService_cl.ResolveLinkedSourceLabel(Listing.LinkedSource);
            return "<span class='bds-card-chip bds-card-chip-linked'>" + Server.HtmlEncode(label) + "</span>";
        }

        return "<span class='bds-card-chip bds-card-chip-native'>AhaLand</span>";
    }

    protected string RenderQuickHighlights()
    {
        if (Listing == null)
            return "";

        var parts = new List<string>();
        if (Listing.AreaValue > 0)
            parts.Add("<span class='bds-intent-pill'>Diện tích " + Server.HtmlEncode(BatDongSanService_cl.FormatArea(Listing.AreaValue)) + "</span>");
        if (Listing.BedroomCount > 0)
            parts.Add("<span class='bds-intent-pill'>" + Server.HtmlEncode(Listing.BedroomCount.ToString()) + " PN</span>");
        if (Listing.BathroomCount > 0)
            parts.Add("<span class='bds-intent-pill'>" + Server.HtmlEncode(Listing.BathroomCount.ToString()) + " WC</span>");
        if (!string.IsNullOrWhiteSpace(Listing.ProjectName))
            parts.Add("<span class='bds-intent-pill bds-intent-pill-soft'>" + Server.HtmlEncode(Listing.ProjectName) + "</span>");
        if (!string.IsNullOrWhiteSpace(Listing.PostedAgoText))
            parts.Add("<span class='bds-intent-pill bds-intent-pill-soft'>" + Server.HtmlEncode(Listing.PostedAgoText) + "</span>");

        return string.Join("", parts.ToArray());
    }

    protected string RenderDescription()
    {
        if (Listing == null)
            return "";

        string text = Listing.IsLinked
            ? BatDongSanService_cl.SanitizeLinkedText(Listing.Description ?? "", 4000, true)
            : (Listing.Description ?? "").Trim();

        if (text == "" && Listing.IsLinked)
            text = BuildLinkedFallbackDescription(Listing);

        if (text == "")
            return "";

        text = text.Replace("\r\n", "\n").Replace("\r", "\n");
        text = Regex.Replace(text, @"[ \t]+", " ");
        text = Regex.Replace(text, @"\n{3,}", "\n\n");
        text = text.Replace(" .", ".").Replace(" ;", ";").Replace(" :", ":").Replace(" ,", ",");

        return Server.HtmlEncode(text);
    }

    private BatDongSanService_cl.ListingItem BuildFromLinked(int linkedId)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var row = LinkedFeedStore_cl.GetById(db, linkedId);
            if (row == null)
                return null;

            string purpose = (row.Purpose ?? "").Trim().ToLowerInvariant() == "rent" ? "rent" : "sale";
            string propertyType = string.IsNullOrWhiteSpace(row.PropertyType) ? "apartment" : row.PropertyType.Trim().ToLowerInvariant();
            string propertyTypeLabel = propertyType == "land" ? "Đất nền"
                : (propertyType == "house" ? "Nhà phố"
                : (propertyType == "office" ? "Văn phòng"
                : (propertyType == "business-premises" ? "Mặt bằng" : "Căn hộ")));

            string rawText = (row.Title ?? "") + " " + (row.Summary ?? "") + " " + (row.AreaText ?? "") + " " + (row.PriceText ?? "");
            decimal area = ParseAreaValue(row.AreaText, rawText);
            int bedroomCount = ParseCount(rawText, @"(\d+)\s*(pn|phòng ngủ)");
            int bathroomCount = ParseCount(rawText, @"(\d+)\s*(wc|phòng tắm|toilet)");
            int floorCount = ParseCount(rawText, @"(\d+)\s*(tầng|lau|lầu)");
            string legal = ParseLegalStatus(rawText);
            decimal priceValue = ParsePriceValue(row.PriceText, rawText);
            string addressText = BuildAddressText(row);
            string direction = ParseHouseDirection(rawText);
            string furnishing = ParseFurnishingStatus(rawText);
            decimal landWidth = ParseLandDimension(rawText, 1);
            decimal landLength = ParseLandDimension(rawText, 2);

            return new BatDongSanService_cl.ListingItem
            {
                Id = 900000 + linkedId,
                Slug = BatDongSanService_cl.Slugify(row.Title),
                Title = row.Title,
                Purpose = purpose,
                PropertyType = propertyType,
                PropertyTypeLabel = propertyTypeLabel,
                Province = row.Province ?? "",
                District = row.District ?? "",
                Ward = "",
                AddressText = addressText,
                PriceValue = priceValue,
                PriceText = string.IsNullOrWhiteSpace(row.PriceText) ? "Liên hệ" : row.PriceText,
                AreaValue = area,
                LegalStatus = legal,
                FurnishingStatus = furnishing,
                BedroomCount = bedroomCount,
                BathroomCount = bathroomCount,
                FloorCount = floorCount,
                HouseDirection = direction,
                LandWidth = landWidth,
                LandLength = landLength,
                ProjectName = "",
                PostedAgoText = row.PublishedAt.ToString("dd/MM/yyyy HH:mm"),
                SellerName = "Tin liên kết",
                SellerRole = BatDongSanService_cl.ResolveLinkedSourceLabel(row.Source),
                ThumbnailUrl = BatDongSanService_cl.BuildLinkedImageProxyUrl(row.Id, 0),
                Description = BatDongSanService_cl.SanitizeLinkedText(row.Summary ?? "", 4000, true),
                ContactPhone = "",
                Gallery = ResolveLinkedGallery(row),
                IsLinked = true,
                LinkedSource = BatDongSanService_cl.ResolveLinkedSourceLabel(row.Source),
                LinkedSourceUrl = row.SourceUrl ?? ""
            };
        }
    }

    private string BuildLinkedFallbackDescription(BatDongSanService_cl.ListingItem item)
    {
        if (item == null)
            return "";

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(item.Title))
            parts.Add(item.Title.Trim());
        if (!string.IsNullOrWhiteSpace(item.PriceText))
            parts.Add("Giá: " + item.PriceText.Trim());
        if (item.AreaValue > 0)
            parts.Add("Diện tích: " + BatDongSanService_cl.FormatArea(item.AreaValue));
        if (!string.IsNullOrWhiteSpace(item.AddressText))
            parts.Add("Khu vực: " + item.AddressText.Trim());
        string sourceLabel = BatDongSanService_cl.ResolveLinkedSourceLabel(item.LinkedSource);
        if (!string.IsNullOrWhiteSpace(sourceLabel))
            parts.Add("Nguồn: " + sourceLabel);

        if (parts.Count == 0)
            return "Tin liên kết được đồng bộ từ nguồn ngoài. Vui lòng xem tại nguồn để đọc đầy đủ mô tả chi tiết.";

        return string.Join(". ", parts.ToArray()) + ". Vui lòng xem tại nguồn để đọc thêm chi tiết.";
    }

    private string BuildAddressText(LinkedFeedStore_cl.LinkedPost row)
    {
        string addr = ((row.District ?? "") + ", " + (row.Province ?? "")).Trim().Trim(',');
        if (!string.IsNullOrWhiteSpace(addr))
            return addr;

        string title = (row.Title ?? "");
        int idx = title.IndexOf(",");
        if (idx > 0)
            return title.Substring(idx + 1).Trim();
        return "";
    }

    private decimal ParseAreaValue(string areaText, string rawText)
    {
        string value = (areaText ?? "").Trim();
        if (value == "")
        {
            Match m = Regex.Match(rawText ?? "", @"(\d+[.,]?\d*)\s*(m2|m²)", RegexOptions.IgnoreCase);
            if (m.Success)
                value = m.Groups[1].Value;
        }
        return ParseDecimalLoose(value);
    }

    private decimal ParsePriceValue(string priceText, string rawText)
    {
        string input = (priceText ?? "").Trim();
        if (input == "")
            input = rawText ?? "";

        Match m = Regex.Match(input, @"(\d+[.,]?\d*)\s*(tỷ|ty|triệu|trieu|đ|vnd)", RegexOptions.IgnoreCase);
        if (!m.Success)
            return 0;

        decimal number = ParseDecimalLoose(m.Groups[1].Value);
        string unit = (m.Groups[2].Value ?? "").ToLowerInvariant();
        if (unit.Contains("tỷ") || unit.Contains("ty"))
            return number * 1000000000m;
        if (unit.Contains("triệu") || unit.Contains("trieu"))
            return number * 1000000m;
        return number;
    }

    private int ParseCount(string text, string pattern)
    {
        Match m = Regex.Match(text ?? "", pattern, RegexOptions.IgnoreCase);
        if (!m.Success)
            return 0;
        int n;
        return int.TryParse(m.Groups[1].Value, out n) ? n : 0;
    }

    private decimal ParseDecimalLoose(string raw)
    {
        string clean = Regex.Replace(raw ?? "", @"[^\d\.,]", "");
        if (clean == "")
            return 0;

        // Handle cases like 17,5 and 17.5
        clean = clean.Replace(",", ".");
        decimal n;
        if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out n))
            return n;
        return 0;
    }

    private string ParseLegalStatus(string text)
    {
        string lower = (text ?? "").ToLowerInvariant();
        if (lower.Contains("sổ hồng"))
            return "Sổ hồng riêng";
        if (lower.Contains("sổ đỏ"))
            return "Sổ đỏ";
        if (lower.Contains("pháp lý"))
            return "Có thông tin pháp lý";
        return "";
    }

    private string ParseHouseDirection(string text)
    {
        string lower = (text ?? "").ToLowerInvariant();
        if (lower.Contains("đông bắc") || lower.Contains("dong bac")) return "Đông Bắc";
        if (lower.Contains("đông nam") || lower.Contains("dong nam")) return "Đông Nam";
        if (lower.Contains("tây bắc") || lower.Contains("tay bac")) return "Tây Bắc";
        if (lower.Contains("tây nam") || lower.Contains("tay nam")) return "Tây Nam";
        if (lower.Contains("hướng đông") || lower.Contains("huong dong")) return "Đông";
        if (lower.Contains("hướng tây") || lower.Contains("huong tay")) return "Tây";
        if (lower.Contains("hướng nam") || lower.Contains("huong nam")) return "Nam";
        if (lower.Contains("hướng bắc") || lower.Contains("huong bac")) return "Bắc";
        return "";
    }

    private string ParseFurnishingStatus(string text)
    {
        string lower = (text ?? "").ToLowerInvariant();
        if (lower.Contains("full nội thất") || lower.Contains("day du noi that") || lower.Contains("đầy đủ nội thất"))
            return "Full nội thất";
        if (lower.Contains("nội thất cơ bản") || lower.Contains("noi that co ban"))
            return "Nội thất cơ bản";
        if (lower.Contains("trống") || lower.Contains("nha trong"))
            return "Nhà trống";
        if (lower.Contains("nội thất") || lower.Contains("noi that"))
            return "Có nội thất";
        return "";
    }

    private decimal ParseLandDimension(string text, int index)
    {
        Match m = Regex.Match(text ?? "", @"(\d+[.,]?\d*)\s*[xX]\s*(\d+[.,]?\d*)\s*m", RegexOptions.IgnoreCase);
        if (!m.Success)
            return 0;

        if (index == 1)
            return ParseDecimalLoose(m.Groups[1].Value);
        return ParseDecimalLoose(m.Groups[2].Value);
    }

    private List<string> ResolveLinkedGallery(LinkedFeedStore_cl.LinkedPost row)
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

    protected string FormatDepositAmount()
    {
        if (Listing == null || Listing.DepositAmount <= 0)
            return "Thỏa thuận";

        return BatDongSanService_cl.FormatMoneyCompact(Listing.DepositAmount, "");
    }

    protected string FormatRentalTerm()
    {
        if (Listing == null || Listing.RentalTermMonths <= 0)
            return "Linh hoạt";

        return Listing.RentalTermMonths.ToString() + " tháng";
    }

    protected string FormatFloorCount()
    {
        if (Listing == null || Listing.FloorCount <= 0)
            return "Chưa cập nhật";

        return Listing.FloorCount.ToString() + " tầng";
    }

    protected string FormatLandSize()
    {
        if (Listing == null || Listing.LandWidth <= 0 || Listing.LandLength <= 0)
            return "Chưa cập nhật";

        return Listing.LandWidth.ToString("0.##") + " x " + Listing.LandLength.ToString("0.##") + " m";
    }
}
