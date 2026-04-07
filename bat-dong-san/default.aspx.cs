using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Web.Caching;

public partial class bat_dong_san_default : BatDongSanPageBase
{
    protected sealed class RegionOverviewItem
    {
        public string ProvinceKey { get; set; }
        public string ProvinceName { get; set; }
        public string HeroImageUrl { get; set; }
        public bool IsHero { get; set; }
        public int SaleAll { get; set; }
        public int SaleApartment { get; set; }
        public int SaleHouse { get; set; }
        public int SaleBusiness { get; set; }
        public int SaleLand { get; set; }
        public int RentAll { get; set; }
        public int RentApartment { get; set; }
        public int RentHouse { get; set; }
        public int RentBusiness { get; set; }
        public int RentLand { get; set; }
    }

    private sealed class LinkedScoreRow
    {
        public LinkedFeedStore_cl.LinkedPost Post { get; set; }
        public int DongNaiPriority { get; set; }
        public int ImagePriority { get; set; }
        public int VisualScore { get; set; }
        public int Score { get; set; }
    }

    protected BatDongSanService_cl.SummaryInfo Summary = new BatDongSanService_cl.SummaryInfo();
    protected string LinkedDebugInfo = "";
    protected int LinkedCurrentPage = 1;
    protected int LinkedTotalPages = 1;
    protected int LinkedTotalItems = 0;
    private const int LinkedPageSize = 12;
    private const int PreferredDongNaiRatioPercent = 90;
    private const int RegionOverviewCacheMinutes = 5;
    private const string RegionOverviewCacheKey = "bds.region-overview.top5.v5";
    protected List<RegionOverviewItem> RegionOverview = new List<RegionOverviewItem>();
    private static readonly Dictionary<string, string> ProvinceAliasToCanonical = BuildProvinceAliasMap();
    private static readonly Dictionary<string, string> ProvinceCanonicalLabels = BuildProvinceLabelMap();
    private static readonly Dictionary<string, string> ProvinceHeroImageMap = BuildProvinceHeroImageMap();
    private static readonly string[] FallbackRegionOrder = new[] { "dong-nai", "tp-ho-chi-minh", "ha-noi", "binh-duong", "da-nang" };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            hfSearchMode.Value = "sale";
            BindPage();
        }
    }

    private void BindPage()
    {
        Summary = BatDongSanService_cl.GetSummary();

        rptFeaturedListings.DataSource = BatDongSanService_cl.GetFeaturedListings("sale", 6);
        rptFeaturedListings.DataBind();

        RegionOverview = BuildRegionOverviewCached();
        rptRegionOverview.DataSource = RegionOverview;
        rptRegionOverview.DataBind();

        using (dbDataContext db = new dbDataContext())
        {
            bool debugLinked = string.Equals((Request.QueryString["debugLinked"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase);
            int page;
            if (!int.TryParse((Request.QueryString["linkedPage"] ?? "").Trim(), out page) || page <= 0)
                page = 1;

            LinkedFeedStore_cl.ExpireOldPosts(db, LinkedFeedStore_cl.ResolveLinkedTtlDays());
            LinkedFeedStore_cl.DeactivateMissingLocationPosts(db);

            var all = LinkedFeedStore_cl.GetLatest(db, 2000);
            var pool = all.ToList();

            var scoredBase = pool
                .Where(HasRenderableLinkedImage)
                .Select(x => new LinkedScoreRow
                {
                    Post = x,
                    DongNaiPriority = ComputeLinkedDongNaiPriority(x),
                    ImagePriority = ComputeLinkedImagePriority(x),
                    VisualScore = ComputeLinkedVisualScore(x),
                    Score = ComputeLinkedQualityScore(x)
                });

            var scored = scoredBase
                .Where(x => x.Score >= 15)
                .OrderByDescending(x => x.DongNaiPriority)
                .ThenByDescending(x => x.ImagePriority)
                .ThenByDescending(x => x.VisualScore)
                .ThenByDescending(x => x.Score)
                .ThenByDescending(x => x.Post.PublishedAt)
                .ToList();

            if (scored.Count < 50)
            {
                scored = scoredBase
                    .Where(x => x.Score >= 5)
                    .OrderByDescending(x => x.DongNaiPriority)
                    .ThenByDescending(x => x.ImagePriority)
                    .ThenByDescending(x => x.VisualScore)
                    .ThenByDescending(x => x.Score)
                    .ThenByDescending(x => x.Post.PublishedAt)
                    .ToList();
            }

            if (scored.Count < 50)
            {
                scored = scoredBase
                    .OrderByDescending(x => x.DongNaiPriority)
                    .ThenByDescending(x => x.ImagePriority)
                    .ThenByDescending(x => x.VisualScore)
                    .ThenByDescending(x => x.Score)
                    .ThenByDescending(x => x.Post.PublishedAt)
                    .ToList();
            }

            var imageFirst = BuildLinkedRankedList(scored.Where(x => x.ImagePriority >= 3));
            var imageLast = BuildLinkedRankedList(scored.Where(x => x.ImagePriority < 3));

            var ranked = imageFirst
                .Concat(imageLast)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .ToList();

            LinkedTotalItems = ranked.Count;
            LinkedTotalPages = Math.Max(1, (int)Math.Ceiling(LinkedTotalItems / (double)LinkedPageSize));
            LinkedCurrentPage = Math.Min(Math.Max(1, page), LinkedTotalPages);

            var linked = ranked
                .Skip((LinkedCurrentPage - 1) * LinkedPageSize)
                .Take(LinkedPageSize)
                .ToList();

            if (debugLinked)
            {
                phLinkedDebug.Visible = true;
                LinkedDebugInfo = "Linked debug | all=" + all.Count
                    + " | pool=" + pool.Count
                    + " | scored(>=15)=" + scored.Count
                    + " | image-first=" + imageFirst.Count
                    + " | image-last=" + imageLast.Count
                    + " | ranked=" + ranked.Count
                    + " | page=" + LinkedCurrentPage + "/" + LinkedTotalPages
                    + " | final=" + linked.Count;
            }

            phLinked.Visible = linked.Count > 0;
            rptLinked.DataSource = linked;
            rptLinked.DataBind();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string mode = (hfSearchMode.Value ?? "").Trim().ToLowerInvariant();
        string target = "/bat-dong-san/mua-ban.aspx";
        if (mode == "rent")
            target = "/bat-dong-san/cho-thue.aspx";
        else if (mode == "project")
            target = "/bat-dong-san/du-an.aspx";

        if (mode == "project")
        {
            var projectQuery = new List<string>();
            if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
                projectQuery.Add("keyword=" + Server.UrlEncode(txtKeyword.Text.Trim()));
            if (projectQuery.Count > 0)
                target += "?" + string.Join("&", projectQuery.ToArray());
        }
        else
        {
            BatDongSanService_cl.ListingQuery query = BatDongSanSearch_cl.NormalizeQuery(new BatDongSanService_cl.ListingQuery
            {
                Purpose = mode == "rent" ? "rent" : "sale",
                Keyword = txtKeyword.Text.Trim()
            });
            target = BatDongSanListingFilterHelper_cl.BuildFilterUrl(target, query);
        }

        Response.Redirect(target, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected string BuildListingMeta(object itemObj)
    {
        return BatDongSanService_cl.BuildListingMeta(itemObj as BatDongSanService_cl.ListingItem);
    }

    protected string RenderLinkedSummary(object summaryObj)
    {
        string text = BatDongSanService_cl.SanitizeLinkedText((summaryObj ?? "").ToString(), 210, false);
        if (string.IsNullOrWhiteSpace(text))
            text = "Xem táº¡i nguá»“n Ä‘á»ƒ Ä‘á»c mÃ´ táº£ chi tiáº¿t cá»§a tin liÃªn káº¿t nÃ y.";
        return Server.HtmlEncode(text);
    }

    protected string BuildLinkedLocation(object districtObj, object provinceObj)
    {
        string district = (districtObj ?? "").ToString().Trim();
        string province = (provinceObj ?? "").ToString().Trim();

        if (district != "" && province != "")
            return Server.HtmlEncode(district + ", " + province);
        if (district != "")
            return Server.HtmlEncode(district);
        if (province != "")
            return Server.HtmlEncode(province);
        return "Äang cáº­p nháº­t vá»‹ trÃ­";
    }

    protected string RenderLinkedSourceLabel(object sourceObj)
    {
        return Server.HtmlEncode(BatDongSanService_cl.ResolveLinkedSourceLabel((sourceObj ?? "").ToString()));
    }

    protected string BuildRegionCardCss(object itemObj)
    {
        RegionOverviewItem item = itemObj as RegionOverviewItem;
        if (item == null)
            return "region-card";
        return item.IsHero
            ? "region-card region-card--hero"
            : "region-card";
    }

    protected string BuildRegionCardStyle(object itemObj)
    {
        RegionOverviewItem item = itemObj as RegionOverviewItem;
        string image = ResolveRegionHeroImage(item == null ? "" : item.HeroImageUrl);
        return "background-image:linear-gradient(180deg, rgba(15,23,42,.15) 0%, rgba(15,23,42,.75) 100%), url('" + Server.HtmlEncode(image) + "');";
    }

    private string ResolveRegionHeroImage(string configured)
    {
        string path = (configured ?? "").Trim();
        string resolved = FindFirstExistingRegionImage(BuildRegionImageCandidates(path));
        if (resolved != "")
            return resolved;

        resolved = FindFirstExistingRegionImage(BuildRegionImageCandidates("/uploads/images/bds-region/default"));
        if (resolved != "")
            return resolved;

        return "/uploads/images/bds-region/default.svg";
    }

    private static IEnumerable<string> BuildRegionImageCandidates(string inputPath)
    {
        string raw = (inputPath ?? "").Trim();
        if (raw == "")
            yield break;

        string noQuery = raw;
        int q = noQuery.IndexOf('?');
        if (q >= 0) noQuery = noQuery.Substring(0, q);
        int h = noQuery.IndexOf('#');
        if (h >= 0) noQuery = noQuery.Substring(0, h);
        noQuery = noQuery.Trim();
        if (noQuery == "")
            yield break;

        yield return noQuery;

        int dot = noQuery.LastIndexOf('.');
        int slash = noQuery.LastIndexOf('/');
        bool hasExt = dot > slash;
        string root = hasExt ? noQuery.Substring(0, dot) : noQuery;
        string[] exts = { ".jpg", ".jpeg", ".webp", ".png", ".svg" };
        for (int i = 0; i < exts.Length; i++)
            yield return root + exts[i];
    }

    private string FindFirstExistingRegionImage(IEnumerable<string> candidates)
    {
        if (candidates == null)
            return "";

        foreach (string candidate in candidates)
        {
            string path = (candidate ?? "").Trim();
            if (path == "" || !path.StartsWith("/"))
                continue;
            try
            {
                string full = Server.MapPath(path);
                if (File.Exists(full))
                    return path;
            }
            catch
            {
            }
        }
        return "";
    }

    protected string ResolveLinkedThumb(object itemObj)
    {
        if (itemObj == null)
            return BatDongSanService_cl.DefaultBdsFallbackImage;

        string galleryCsv = ReadStringProp(itemObj, "GalleryCsv");
        string thumb = ReadStringProp(itemObj, "ThumbnailUrl");
        int linkedId = ReadIntProp(itemObj, "Id");
        List<string> raw = BatDongSanService_cl.ResolveLinkedRawGalleryUrls(thumb, galleryCsv);
        if (linkedId > 0 && raw.Count > 0)
            return BatDongSanService_cl.BuildLinkedImageProxyUrl(linkedId, 0);
        return BatDongSanService_cl.ResolveLinkedThumbnail(thumb, galleryCsv);
    }

    protected string BuildLinkedGalleryData(object itemObj)
    {
        if (itemObj == null)
            return "";

        string thumb = ReadStringProp(itemObj, "ThumbnailUrl");
        string galleryCsv = ReadStringProp(itemObj, "GalleryCsv");
        int linkedId = ReadIntProp(itemObj, "Id");
        List<string> raw = BatDongSanService_cl.ResolveLinkedRawGalleryUrls(thumb, galleryCsv);
        if (linkedId <= 0 || raw.Count == 0)
        {
            List<string> fallback = BatDongSanService_cl.ResolveLinkedGalleryUrls(thumb, galleryCsv);
            if (fallback.Count == 1 && string.Equals(fallback[0], BatDongSanService_cl.DefaultBdsFallbackImage, StringComparison.OrdinalIgnoreCase))
                return "";
            return HttpUtility.HtmlAttributeEncode(string.Join("|", fallback.ToArray()));
        }

        List<string> list = raw
            .Take(8)
            .Select((x, index) => BatDongSanService_cl.BuildLinkedImageProxyUrl(linkedId, index))
            .ToList();

        if (list.Count == 0)
            return "";

        return HttpUtility.HtmlAttributeEncode(string.Join("|", list.ToArray()));
    }

    private static string ReadStringProp(object itemObj, string propName)
    {
        try
        {
            PropertyInfo p = itemObj.GetType().GetProperty(propName);
            if (p == null)
                return "";
            object v = p.GetValue(itemObj, null);
            return v == null ? "" : v.ToString().Trim();
        }
        catch
        {
            return "";
        }
    }

    private static int ReadIntProp(object itemObj, string propName)
    {
        try
        {
            PropertyInfo p = itemObj.GetType().GetProperty(propName);
            if (p == null)
                return 0;
            object v = p.GetValue(itemObj, null);
            if (v == null)
                return 0;
            int n;
            return int.TryParse(v.ToString(), out n) ? n : 0;
        }
        catch
        {
            return 0;
        }
    }

    private static bool IsUsableThumb(string url)
    {
        string v = (url ?? "").Trim();
        if (v == "")
            return false;
        if (v.Equals(BatDongSanService_cl.DefaultBdsFallbackImage, StringComparison.OrdinalIgnoreCase)
            || v.Equals("/uploads/images/macdinh.jpg", StringComparison.OrdinalIgnoreCase))
            return false;

        string lower = v.ToLowerInvariant();
        if (lower.Contains("avatar") || lower.Contains("profile") || lower.Contains("logo") || lower.Contains("icon") || lower.Contains("banner") || lower.Contains("favicon"))
            return false;

        try
        {
            var ctx = HttpContext.Current;
            if (ctx != null && ctx.Server != null)
            {
                if (v.StartsWith("/"))
                {
                    string physical = ctx.Server.MapPath(v);
                    if (!File.Exists(physical))
                        return false;
                    long bytes = new FileInfo(physical).Length;
                    return bytes >= 12000;
                }
                if (v.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || v.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private bool HasUsableLinkedImage(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return false;

        if (IsUsableThumb(post.ThumbnailUrl))
            return true;

        string csv = (post.GalleryCsv ?? "").Trim();
        if (csv == "")
            return false;

        string[] arr = csv.Split('|');
        for (int i = 0; i < arr.Length; i++)
        {
            if (IsUsableThumb(arr[i]))
                return true;
        }

        return false;
    }

    private bool HasRenderableLinkedImage(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return false;

        return BatDongSanService_cl.ComputeLinkedVisualPriority(post.ThumbnailUrl, post.GalleryCsv) > 0;
    }

    private int ComputeLinkedQualityScore(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return 0;

        int score = 0;
        int imagePriority = ComputeLinkedImagePriority(post);
        if (imagePriority >= 2) score += 40;
        else if (imagePriority == 1) score += 20;
        if (!string.IsNullOrWhiteSpace(post.Title)) score += 20;
        if (!string.IsNullOrWhiteSpace(post.Summary)) score += 10;
        if (!string.IsNullOrWhiteSpace(post.PriceText)) score += 20;
        if (!string.IsNullOrWhiteSpace(post.AreaText)) score += 15;
        string provinceKey = NormalizeRegionProvinceKey(post.Province);
        if (!string.IsNullOrWhiteSpace(post.Province)) score += 10;
        if (!string.IsNullOrWhiteSpace(post.District)) score += 8;
        if (!string.IsNullOrWhiteSpace(post.Purpose)) score += 5;
        if (!string.IsNullOrWhiteSpace(post.PropertyType)) score += 5;
        if (provinceKey == "dong-nai") score += 60;
        if (IsDongNaiFocusSource(post.Source)) score += 40;

        string gallery = (post.GalleryCsv ?? "").Trim();
        if (gallery != "")
        {
            int imageCount = gallery.Split('|').Count(x => IsUsableThumb(x));
            if (imageCount >= 3) score += 15;
            else if (imageCount >= 1) score += 8;
        }

        string inspect = ((post.Title ?? "") + " " + (post.Summary ?? "")).ToLowerInvariant();
        if (inspect.Contains("toÃ n quá»‘c") || inspect.Contains("toan quoc")) score -= 30;
        if (provinceKey != "dong-nai") score -= 8;

        return score;
    }

    private List<LinkedFeedStore_cl.LinkedPost> BuildLinkedPageSlice(
        List<LinkedFeedStore_cl.LinkedPost> preferred,
        List<LinkedFeedStore_cl.LinkedPost> semiPreferred,
        List<LinkedFeedStore_cl.LinkedPost> fallback,
        int page,
        int pageSize)
    {
        preferred = preferred ?? new List<LinkedFeedStore_cl.LinkedPost>();
        semiPreferred = semiPreferred ?? new List<LinkedFeedStore_cl.LinkedPost>();
        fallback = fallback ?? new List<LinkedFeedStore_cl.LinkedPost>();

        int safePage = page <= 0 ? 1 : page;
        int preferredQuota = (int)Math.Ceiling(pageSize * (PreferredDongNaiRatioPercent / 100.0));
        int previousSlots = (safePage - 1) * pageSize;

        int preferredStart = Math.Min(previousSlots, preferred.Count);
        int preferredTakenBefore = preferredStart;

        int nonPreferredSlotsBefore = previousSlots - preferredTakenBefore;
        if (nonPreferredSlotsBefore < 0)
            nonPreferredSlotsBefore = 0;

        int semiStart = Math.Min(nonPreferredSlotsBefore, semiPreferred.Count);
        int semiTakenBefore = semiStart;

        int fallbackStart = nonPreferredSlotsBefore - semiTakenBefore;
        if (fallbackStart < 0)
            fallbackStart = 0;
        if (fallbackStart > fallback.Count)
            fallbackStart = fallback.Count;

        var result = new List<LinkedFeedStore_cl.LinkedPost>();

        for (int i = preferredStart; i < preferred.Count && result.Count < preferredQuota; i++)
            result.Add(preferred[i]);

        for (int i = semiStart; i < semiPreferred.Count && result.Count < pageSize; i++)
            result.Add(semiPreferred[i]);

        for (int i = fallbackStart; i < fallback.Count && result.Count < pageSize; i++)
            result.Add(fallback[i]);

        if (result.Count < pageSize)
        {
            for (int i = preferredStart + (result.Count > preferredQuota ? preferredQuota : Math.Min(preferredQuota, preferred.Count - preferredStart)); i < preferred.Count && result.Count < pageSize; i++)
            {
                if (!result.Any(x => x.Id == preferred[i].Id))
                    result.Add(preferred[i]);
            }
        }

        return result
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .Take(pageSize)
            .ToList();
    }

    private int ComputeLinkedDongNaiPriority(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return 0;

        string provinceKey = NormalizeRegionProvinceKey(post.Province);
        string district = (post.District ?? "").Trim().ToLowerInvariant();
        string purpose = (post.Purpose ?? "").Trim().ToLowerInvariant();

        bool isCommercial = purpose == "rent" || purpose == "sale" || purpose == "";
        if (provinceKey == "dong-nai" && IsDongNaiFocusSource(post.Source) && isCommercial)
            return 4;
        if (provinceKey == "dong-nai" && isCommercial)
            return 3;
        if (district.Contains("biÃªn hÃ²a") || district.Contains("bien hoa") || district.Contains("long thÃ nh") || district.Contains("long thanh") || district.Contains("nhÆ¡n tráº¡ch") || district.Contains("nhon trach"))
            return 2;
        if (IsDongNaiFocusSource(post.Source))
            return 1;
        return 0;
    }

    private bool IsDongNaiFocusSource(string source)
    {
        string key = (source ?? "").Trim().ToLowerInvariant();
        return key.Contains("dong-nai") || key.Contains("bien-hoa");
    }

    private int ComputeLinkedImagePriority(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return 0;

        string thumb = (post.ThumbnailUrl ?? "").Trim();
        string gallery = (post.GalleryCsv ?? "").Trim();
        int imageCount = CountUsableImages(post);

        bool hasThumb = IsUsableThumb(thumb);
        bool thumbLocal = hasThumb && thumb.StartsWith("/uploads/images/linked-bds/", StringComparison.OrdinalIgnoreCase);
        bool likelySourceLogo = IsLikelySourceLogoImage(post);
        int visualPriority = BatDongSanService_cl.ComputeLinkedVisualPriority(post.ThumbnailUrl, post.GalleryCsv);

        if (likelySourceLogo && imageCount <= 1)
            return 1;
        if (visualPriority >= 3)
            return 4;
        if (thumbLocal && imageCount >= 3)
            return 4;
        if (thumbLocal && imageCount >= 1)
            return 3;
        if (visualPriority >= 1)
            return 1;
        if (!string.IsNullOrWhiteSpace(thumb) || !string.IsNullOrWhiteSpace(gallery))
            return 1;
        return 0;
    }

    private int ComputeLinkedVisualScore(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return 0;

        int score = 0;
        int imageCount = CountUsableImages(post);
        if (imageCount >= 4) score += 25;
        else if (imageCount >= 2) score += 15;
        else if (imageCount >= 1) score += 8;

        string thumb = (post.ThumbnailUrl ?? "").Trim();
        if (thumb.StartsWith("/uploads/images/linked-bds/", StringComparison.OrdinalIgnoreCase))
            score += 20;

        if (IsLikelySourceLogoImage(post))
            score -= 25;

        return score;
    }

    private List<LinkedFeedStore_cl.LinkedPost> BuildLinkedRankedList(IEnumerable<LinkedScoreRow> scoredItems)
    {
        var rows = (scoredItems ?? Enumerable.Empty<LinkedScoreRow>())
            .OrderByDescending(x => x.DongNaiPriority)
            .ThenByDescending(x => x.ImagePriority)
            .ThenByDescending(x => x.VisualScore)
            .ThenByDescending(x => x.Score)
            .ThenByDescending(x => x.Post.PublishedAt)
            .ToList();

        var preferred = rows.Where(x => x.DongNaiPriority >= 3).Select(x => x.Post);
        var semiPreferred = rows.Where(x => x.DongNaiPriority == 2).Select(x => x.Post);
        var fallback = rows.Where(x => x.DongNaiPriority <= 1).Select(x => x.Post);

        return preferred
            .Concat(semiPreferred)
            .Concat(fallback)
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .ToList();
    }

    private int CountUsableImages(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return 0;

        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string thumb = (post.ThumbnailUrl ?? "").Trim();
        if (IsUsableThumb(thumb))
            set.Add(thumb);

        string gallery = (post.GalleryCsv ?? "").Trim();
        if (gallery != "")
        {
            string[] arr = gallery.Split('|');
            for (int i = 0; i < arr.Length; i++)
            {
                string url = (arr[i] ?? "").Trim();
                if (!IsUsableThumb(url))
                    continue;
                set.Add(url);
            }
        }
        return set.Count;
    }

    private bool IsLikelySourceLogoImage(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return false;

        string thumb = (post.ThumbnailUrl ?? "").Trim().ToLowerInvariant();
        if (thumb == "")
            return false;

        string[] bad = new[]
        {
            "logo", "favicon", "avatar", "default-user", "placeholder",
            "rongbay", "raovat", "alonhadat", "muaban.net/logo", "icon"
        };

        for (int i = 0; i < bad.Length; i++)
        {
            if (thumb.Contains(bad[i]))
                return true;
        }

        return false;
    }

    protected bool HasLinkedPager()
    {
        return LinkedTotalPages > 1;
    }

    protected string BuildLinkedPageUrl(int page)
    {
        int p = page <= 0 ? 1 : page;
        var query = System.Web.HttpUtility.ParseQueryString(Request.QueryString.ToString());
        query.Set("linkedPage", p.ToString());
        string q = query.ToString();
        return "/bat-dong-san" + (string.IsNullOrWhiteSpace(q) ? "" : "?" + q);
    }

    protected List<int> GetLinkedPageNumbers()
    {
        var pages = new List<int>();
        if (LinkedTotalPages <= 1)
            return pages;

        int maxButtons = 10;
        int start = ((LinkedCurrentPage - 1) / maxButtons) * maxButtons + 1;
        int end = Math.Min(LinkedTotalPages, start + maxButtons - 1);
        for (int i = start; i <= end; i++)
            pages.Add(i);
        return pages;
    }

    private List<RegionOverviewItem> BuildRegionOverviewCached()
    {
        object cached = HttpRuntime.Cache[RegionOverviewCacheKey];
        var list = cached as List<RegionOverviewItem>;
        if (list != null && list.Count > 0)
            return CloneRegionOverview(list);

        var fresh = BuildRegionOverviewCore();
        HttpRuntime.Cache.Insert(
            RegionOverviewCacheKey,
            CloneRegionOverview(fresh),
            null,
            DateTime.UtcNow.AddMinutes(RegionOverviewCacheMinutes),
            Cache.NoSlidingExpiration);
        return fresh;
    }

    private List<RegionOverviewItem> BuildRegionOverviewCore()
    {
        var counters = new Dictionary<string, RegionOverviewItem>(StringComparer.OrdinalIgnoreCase);

        var selfListings = BatDongSanService_cl.GetLiveListings();
        for (int i = 0; i < selfListings.Count; i++)
            AccumulateRegionCount(counters, selfListings[i].Province, selfListings[i].Purpose, selfListings[i].PropertyType);

        using (dbDataContext db = new dbDataContext())
        {
            LinkedFeedStore_cl.ExpireOldPosts(db, LinkedFeedStore_cl.ResolveLinkedTtlDays());
            var linked = LinkedFeedStore_cl.GetActiveForSearch(db, 5000);
            for (int i = 0; i < linked.Count; i++)
            {
                var row = linked[i];
                AccumulateRegionCount(counters, row.Province, row.Purpose, row.PropertyType);
            }
        }

        var ranked = counters.Values
            .Where(x => (x.SaleAll + x.RentAll) > 0)
            .OrderByDescending(x => string.Equals(x.ProvinceKey, "dong-nai", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
            .ThenByDescending(x => x.SaleAll + x.RentAll)
            .ThenByDescending(x => string.Equals(x.ProvinceKey, "dong-nai", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
            .ThenByDescending(x => x.SaleAll)
            .ThenBy(x => x.ProvinceName)
            .Take(5)
            .ToList();

        if (ranked.Count < 5)
        {
            for (int i = 0; i < FallbackRegionOrder.Length; i++)
            {
                string k = FallbackRegionOrder[i];
                if (ranked.Any(x => string.Equals(x.ProvinceKey, k, StringComparison.OrdinalIgnoreCase)))
                    continue;
                ranked.Add(CreateRegionCounter(k));
                if (ranked.Count >= 5)
                    break;
            }
        }

        if (ranked.Count > 0)
            ranked[0].IsHero = true;

        return ranked;
    }

    private List<RegionOverviewItem> CloneRegionOverview(List<RegionOverviewItem> source)
    {
        if (source == null)
            return new List<RegionOverviewItem>();

        return source.Select(x => new RegionOverviewItem
        {
            ProvinceKey = x.ProvinceKey,
            ProvinceName = x.ProvinceName,
            HeroImageUrl = x.HeroImageUrl,
            IsHero = x.IsHero,
            SaleAll = x.SaleAll,
            SaleApartment = x.SaleApartment,
            SaleHouse = x.SaleHouse,
            SaleBusiness = x.SaleBusiness,
            SaleLand = x.SaleLand,
            RentAll = x.RentAll,
            RentApartment = x.RentApartment,
            RentHouse = x.RentHouse,
            RentBusiness = x.RentBusiness,
            RentLand = x.RentLand
        }).ToList();
    }

    private void AccumulateRegionCount(Dictionary<string, RegionOverviewItem> target, string provinceName, string purpose, string propertyType)
    {
        if (target == null)
            return;

        string key = NormalizeRegionProvinceKey(provinceName);
        if (key == "")
            return;

        RegionOverviewItem item;
        if (!target.TryGetValue(key, out item))
        {
            item = CreateRegionCounter(key);
            target[key] = item;
        }

        string p = ((purpose ?? "").Trim().ToLowerInvariant() == "rent") ? "rent" : "sale";
        string type = (propertyType ?? "").Trim().ToLowerInvariant();
        string bucket = "apartment";
        if (type == "house") bucket = "house";
        else if (type == "land") bucket = "land";
        else if (type == "office" || type == "business-premises") bucket = "business";

        if (p == "rent")
        {
            item.RentAll++;
            if (bucket == "apartment") item.RentApartment++;
            else if (bucket == "house") item.RentHouse++;
            else if (bucket == "land") item.RentLand++;
            else item.RentBusiness++;
        }
        else
        {
            item.SaleAll++;
            if (bucket == "apartment") item.SaleApartment++;
            else if (bucket == "house") item.SaleHouse++;
            else if (bucket == "land") item.SaleLand++;
            else item.SaleBusiness++;
        }
    }

    private string NormalizeRegionProvinceKey(string provinceName)
    {
        string raw = (provinceName ?? "").Trim();
        if (raw == "")
            return "";

        string slug = BatDongSanService_cl.Slugify(raw);
        if (slug == "")
            return "";

        string canonical;
        if (ProvinceAliasToCanonical.TryGetValue(slug, out canonical))
            return canonical;

        return slug;
    }

    private RegionOverviewItem CreateRegionCounter(string key)
    {
        string canonical = (key ?? "").Trim().ToLowerInvariant();
        string label;
        if (!ProvinceCanonicalLabels.TryGetValue(canonical, out label))
            label = ToRegionLabel(canonical);

        string hero;
        if (!ProvinceHeroImageMap.TryGetValue(canonical, out hero))
            hero = "/uploads/images/bds-region/default.svg";

        return new RegionOverviewItem
        {
            ProvinceKey = canonical,
            ProvinceName = label,
            HeroImageUrl = hero
        };
    }

    protected int GetRegionDisplayCount(object itemObj)
    {
        RegionOverviewItem item = itemObj as RegionOverviewItem;
        if (item == null)
            return 0;
        return item.SaleAll + item.RentAll;
    }

    private string ToRegionLabel(string slug)
    {
        string[] parts = (slug ?? "").Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return "Khu vá»±c";
        for (int i = 0; i < parts.Length; i++)
        {
            string p = parts[i];
            parts[i] = p.Length <= 1 ? p.ToUpperInvariant() : char.ToUpperInvariant(p[0]) + p.Substring(1);
        }
        return string.Join(" ", parts);
    }

    private static void AddProvinceAliases(Dictionary<string, string> map, string canonical, params string[] aliases)
    {
        if (map == null || string.IsNullOrWhiteSpace(canonical))
            return;
        for (int i = 0; i < aliases.Length; i++)
        {
            string key = BatDongSanService_cl.Slugify(aliases[i] ?? "");
            if (key == "")
                continue;
            map[key] = canonical;
        }
    }

    private static Dictionary<string, string> BuildProvinceAliasMap()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        AddProvinceAliases(map, "an-giang", "An Giang", "Tá»‰nh An Giang");
        AddProvinceAliases(map, "ba-ria-vung-tau", "BÃ  Rá»‹a - VÅ©ng TÃ u", "BÃ  Rá»‹a VÅ©ng TÃ u", "VÅ©ng TÃ u", "Tá»‰nh BÃ  Rá»‹a - VÅ©ng TÃ u");
        AddProvinceAliases(map, "bac-giang", "Báº¯c Giang", "Tá»‰nh Báº¯c Giang");
        AddProvinceAliases(map, "bac-kan", "Báº¯c Káº¡n", "Báº¯c Cáº¡n", "Tá»‰nh Báº¯c Káº¡n");
        AddProvinceAliases(map, "bac-lieu", "Báº¡c LiÃªu", "Tá»‰nh Báº¡c LiÃªu");
        AddProvinceAliases(map, "bac-ninh", "Báº¯c Ninh", "Tá»‰nh Báº¯c Ninh");
        AddProvinceAliases(map, "ben-tre", "Báº¿n Tre", "Tá»‰nh Báº¿n Tre");
        AddProvinceAliases(map, "binh-dinh", "BÃ¬nh Äá»‹nh", "Tá»‰nh BÃ¬nh Äá»‹nh");
        AddProvinceAliases(map, "binh-duong", "BÃ¬nh DÆ°Æ¡ng", "Tá»‰nh BÃ¬nh DÆ°Æ¡ng");
        AddProvinceAliases(map, "binh-phuoc", "BÃ¬nh PhÆ°á»›c", "Tá»‰nh BÃ¬nh PhÆ°á»›c");
        AddProvinceAliases(map, "binh-thuan", "BÃ¬nh Thuáº­n", "Tá»‰nh BÃ¬nh Thuáº­n");
        AddProvinceAliases(map, "ca-mau", "CÃ  Mau", "Tá»‰nh CÃ  Mau");
        AddProvinceAliases(map, "cao-bang", "Cao Báº±ng", "Tá»‰nh Cao Báº±ng");
        AddProvinceAliases(map, "dak-lak", "Äáº¯k Láº¯k", "Dak Lak", "Tá»‰nh Äáº¯k Láº¯k");
        AddProvinceAliases(map, "dak-nong", "Äáº¯k NÃ´ng", "Dak Nong", "Tá»‰nh Äáº¯k NÃ´ng");
        AddProvinceAliases(map, "dien-bien", "Äiá»‡n BiÃªn", "Tá»‰nh Äiá»‡n BiÃªn");
        AddProvinceAliases(map, "dong-nai", "Äá»“ng Nai", "Tá»‰nh Äá»“ng Nai");
        AddProvinceAliases(map, "dong-thap", "Äá»“ng ThÃ¡p", "Tá»‰nh Äá»“ng ThÃ¡p");
        AddProvinceAliases(map, "gia-lai", "Gia Lai", "Tá»‰nh Gia Lai");
        AddProvinceAliases(map, "ha-giang", "HÃ  Giang", "Tá»‰nh HÃ  Giang");
        AddProvinceAliases(map, "ha-nam", "HÃ  Nam", "Tá»‰nh HÃ  Nam");
        AddProvinceAliases(map, "ha-tinh", "HÃ  TÄ©nh", "Tá»‰nh HÃ  TÄ©nh");
        AddProvinceAliases(map, "hai-duong", "Háº£i DÆ°Æ¡ng", "Tá»‰nh Háº£i DÆ°Æ¡ng");
        AddProvinceAliases(map, "hau-giang", "Háº­u Giang", "Tá»‰nh Háº­u Giang");
        AddProvinceAliases(map, "hoa-binh", "HÃ²a BÃ¬nh", "Tá»‰nh HÃ²a BÃ¬nh");
        AddProvinceAliases(map, "hung-yen", "HÆ°ng YÃªn", "Tá»‰nh HÆ°ng YÃªn");
        AddProvinceAliases(map, "khanh-hoa", "KhÃ¡nh HÃ²a", "Tá»‰nh KhÃ¡nh HÃ²a");
        AddProvinceAliases(map, "kien-giang", "KiÃªn Giang", "Tá»‰nh KiÃªn Giang");
        AddProvinceAliases(map, "kon-tum", "Kon Tum", "Tá»‰nh Kon Tum");
        AddProvinceAliases(map, "lai-chau", "Lai ChÃ¢u", "Tá»‰nh Lai ChÃ¢u");
        AddProvinceAliases(map, "lam-dong", "LÃ¢m Äá»“ng", "Tá»‰nh LÃ¢m Äá»“ng");
        AddProvinceAliases(map, "lang-son", "Láº¡ng SÆ¡n", "Tá»‰nh Láº¡ng SÆ¡n");
        AddProvinceAliases(map, "lao-cai", "LÃ o Cai", "Tá»‰nh LÃ o Cai");
        AddProvinceAliases(map, "long-an", "Long An", "Tá»‰nh Long An");
        AddProvinceAliases(map, "nam-dinh", "Nam Äá»‹nh", "Tá»‰nh Nam Äá»‹nh");
        AddProvinceAliases(map, "nghe-an", "Nghá»‡ An", "Tá»‰nh Nghá»‡ An");
        AddProvinceAliases(map, "ninh-binh", "Ninh BÃ¬nh", "Tá»‰nh Ninh BÃ¬nh");
        AddProvinceAliases(map, "ninh-thuan", "Ninh Thuáº­n", "Tá»‰nh Ninh Thuáº­n");
        AddProvinceAliases(map, "phu-tho", "PhÃº Thá»", "Tá»‰nh PhÃº Thá»");
        AddProvinceAliases(map, "phu-yen", "PhÃº YÃªn", "Tá»‰nh PhÃº YÃªn");
        AddProvinceAliases(map, "quang-binh", "Quáº£ng BÃ¬nh", "Tá»‰nh Quáº£ng BÃ¬nh");
        AddProvinceAliases(map, "quang-nam", "Quáº£ng Nam", "Tá»‰nh Quáº£ng Nam");
        AddProvinceAliases(map, "quang-ngai", "Quáº£ng NgÃ£i", "Tá»‰nh Quáº£ng NgÃ£i");
        AddProvinceAliases(map, "quang-ninh", "Quáº£ng Ninh", "Tá»‰nh Quáº£ng Ninh");
        AddProvinceAliases(map, "quang-tri", "Quáº£ng Trá»‹", "Tá»‰nh Quáº£ng Trá»‹");
        AddProvinceAliases(map, "soc-trang", "SÃ³c TrÄƒng", "Tá»‰nh SÃ³c TrÄƒng");
        AddProvinceAliases(map, "son-la", "SÆ¡n La", "Tá»‰nh SÆ¡n La");
        AddProvinceAliases(map, "tay-ninh", "TÃ¢y Ninh", "Tá»‰nh TÃ¢y Ninh");
        AddProvinceAliases(map, "thai-binh", "ThÃ¡i BÃ¬nh", "Tá»‰nh ThÃ¡i BÃ¬nh");
        AddProvinceAliases(map, "thai-nguyen", "ThÃ¡i NguyÃªn", "Tá»‰nh ThÃ¡i NguyÃªn");
        AddProvinceAliases(map, "thanh-hoa", "Thanh HÃ³a", "Tá»‰nh Thanh HÃ³a");
        AddProvinceAliases(map, "thua-thien-hue", "Thá»«a ThiÃªn Huáº¿", "Tá»‰nh Thá»«a ThiÃªn Huáº¿", "Huáº¿");
        AddProvinceAliases(map, "tien-giang", "Tiá»n Giang", "Tá»‰nh Tiá»n Giang");
        AddProvinceAliases(map, "tra-vinh", "TrÃ  Vinh", "Tá»‰nh TrÃ  Vinh");
        AddProvinceAliases(map, "tuyen-quang", "TuyÃªn Quang", "Tá»‰nh TuyÃªn Quang");
        AddProvinceAliases(map, "vinh-long", "VÄ©nh Long", "Tá»‰nh VÄ©nh Long");
        AddProvinceAliases(map, "vinh-phuc", "VÄ©nh PhÃºc", "Tá»‰nh VÄ©nh PhÃºc");
        AddProvinceAliases(map, "yen-bai", "YÃªn BÃ¡i", "Tá»‰nh YÃªn BÃ¡i");

        AddProvinceAliases(map, "can-tho", "Cáº§n ThÆ¡", "ThÃ nh phá»‘ Cáº§n ThÆ¡", "TP Cáº§n ThÆ¡");
        AddProvinceAliases(map, "da-nang", "ÄÃ  Náºµng", "ThÃ nh phá»‘ ÄÃ  Náºµng", "TP ÄÃ  Náºµng", "Danang");
        AddProvinceAliases(map, "hai-phong", "Háº£i PhÃ²ng", "ThÃ nh phá»‘ Háº£i PhÃ²ng", "TP Háº£i PhÃ²ng");
        AddProvinceAliases(map, "ha-noi", "HÃ  Ná»™i", "ThÃ nh phá»‘ HÃ  Ná»™i", "TP HÃ  Ná»™i", "Hanoi");
        AddProvinceAliases(map, "tp-ho-chi-minh", "TP.HCM", "TP HCM", "TPHCM", "HCM", "Há»“ ChÃ­ Minh", "ThÃ nh phá»‘ Há»“ ChÃ­ Minh", "SÃ i GÃ²n", "Sai Gon");

        // Chuáº©n hÃ³a thÃªm alias viáº¿t táº¯t phá»• biáº¿n
        AddProvinceAliases(map, "ba-ria-vung-tau", "BRVT");
        AddProvinceAliases(map, "thua-thien-hue", "TT Huáº¿", "TTHue");
        AddProvinceAliases(map, "dak-lak", "Dac Lac");
        AddProvinceAliases(map, "dak-nong", "Dac Nong");

        return map;
    }

    private static Dictionary<string, string> BuildProvinceLabelMap()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        map["an-giang"] = "An Giang";
        map["ba-ria-vung-tau"] = "BÃ  Rá»‹a - VÅ©ng TÃ u";
        map["bac-giang"] = "Báº¯c Giang";
        map["bac-kan"] = "Báº¯c Káº¡n";
        map["bac-lieu"] = "Báº¡c LiÃªu";
        map["bac-ninh"] = "Báº¯c Ninh";
        map["ben-tre"] = "Báº¿n Tre";
        map["binh-dinh"] = "BÃ¬nh Äá»‹nh";
        map["binh-duong"] = "BÃ¬nh DÆ°Æ¡ng";
        map["binh-phuoc"] = "BÃ¬nh PhÆ°á»›c";
        map["binh-thuan"] = "BÃ¬nh Thuáº­n";
        map["ca-mau"] = "CÃ  Mau";
        map["cao-bang"] = "Cao Báº±ng";
        map["dak-lak"] = "Äáº¯k Láº¯k";
        map["dak-nong"] = "Äáº¯k NÃ´ng";
        map["dien-bien"] = "Äiá»‡n BiÃªn";
        map["dong-nai"] = "Äá»“ng Nai";
        map["dong-thap"] = "Äá»“ng ThÃ¡p";
        map["gia-lai"] = "Gia Lai";
        map["ha-giang"] = "HÃ  Giang";
        map["ha-nam"] = "HÃ  Nam";
        map["ha-tinh"] = "HÃ  TÄ©nh";
        map["hai-duong"] = "Háº£i DÆ°Æ¡ng";
        map["hau-giang"] = "Háº­u Giang";
        map["hoa-binh"] = "HÃ²a BÃ¬nh";
        map["hung-yen"] = "HÆ°ng YÃªn";
        map["khanh-hoa"] = "KhÃ¡nh HÃ²a";
        map["kien-giang"] = "KiÃªn Giang";
        map["kon-tum"] = "Kon Tum";
        map["lai-chau"] = "Lai ChÃ¢u";
        map["lam-dong"] = "LÃ¢m Äá»“ng";
        map["lang-son"] = "Láº¡ng SÆ¡n";
        map["lao-cai"] = "LÃ o Cai";
        map["long-an"] = "Long An";
        map["nam-dinh"] = "Nam Äá»‹nh";
        map["nghe-an"] = "Nghá»‡ An";
        map["ninh-binh"] = "Ninh BÃ¬nh";
        map["ninh-thuan"] = "Ninh Thuáº­n";
        map["phu-tho"] = "PhÃº Thá»";
        map["phu-yen"] = "PhÃº YÃªn";
        map["quang-binh"] = "Quáº£ng BÃ¬nh";
        map["quang-nam"] = "Quáº£ng Nam";
        map["quang-ngai"] = "Quáº£ng NgÃ£i";
        map["quang-ninh"] = "Quáº£ng Ninh";
        map["quang-tri"] = "Quáº£ng Trá»‹";
        map["soc-trang"] = "SÃ³c TrÄƒng";
        map["son-la"] = "SÆ¡n La";
        map["tay-ninh"] = "TÃ¢y Ninh";
        map["thai-binh"] = "ThÃ¡i BÃ¬nh";
        map["thai-nguyen"] = "ThÃ¡i NguyÃªn";
        map["thanh-hoa"] = "Thanh HÃ³a";
        map["thua-thien-hue"] = "Thá»«a ThiÃªn Huáº¿";
        map["tien-giang"] = "Tiá»n Giang";
        map["tra-vinh"] = "TrÃ  Vinh";
        map["tuyen-quang"] = "TuyÃªn Quang";
        map["vinh-long"] = "VÄ©nh Long";
        map["vinh-phuc"] = "VÄ©nh PhÃºc";
        map["yen-bai"] = "YÃªn BÃ¡i";
        map["can-tho"] = "Cáº§n ThÆ¡";
        map["da-nang"] = "ÄÃ  Náºµng";
        map["hai-phong"] = "Háº£i PhÃ²ng";
        map["ha-noi"] = "HÃ  Ná»™i";
        map["tp-ho-chi-minh"] = "TP.HCM";
        return map;
    }

    private static string ToDisplayNameFromCanonical(string canonical)
    {
        string[] parts = (canonical ?? "").Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            string p = parts[i];
            parts[i] = p.Length <= 1 ? p.ToUpperInvariant() : char.ToUpperInvariant(p[0]) + p.Substring(1);
        }
        return string.Join(" ", parts);
    }

    private static Dictionary<string, string> BuildProvinceHeroImageMap()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in ProvinceCanonicalLabels)
        {
            map[kv.Key] = "/uploads/images/bds-region/" + kv.Key;
        }
        map["default"] = "/uploads/images/bds-region/default";
        return map;
    }
}
