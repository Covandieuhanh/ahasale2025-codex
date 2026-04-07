<%@ WebHandler Language="C#" Class="HomeSearchSuggestHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class HomeSearchSuggestHandler : IHttpHandler
{
    private const int SuggestCacheSeconds = 90;

    private sealed class SuggestItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string url { get; set; }
        public string source { get; set; }
    }

    private sealed class SuggestResponse
    {
        public bool ok { get; set; }
        public string message { get; set; }
        public List<SuggestItem> items { get; set; }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
        {
            WriteJson(context, new SuggestResponse
            {
                ok = false,
                message = "Method not allowed.",
                items = new List<SuggestItem>()
            }, 405);
            return;
        }

        string keyword = (context.Request.QueryString["q"] ?? "").Trim();
        string cat = (context.Request.QueryString["cat"] ?? "").Trim();
        string loc = (context.Request.QueryString["loc"] ?? "").Trim();
        int limit = ParseLimit(context.Request.QueryString["limit"]);

        if (keyword.Length < 2)
        {
            WriteJson(context, new SuggestResponse
            {
                ok = true,
                message = "Keyword too short.",
                items = new List<SuggestItem>()
            }, 200);
            return;
        }

        try
        {
            string cacheKey = string.Format(
                "home:search-suggest:{0}:{1}:{2}:{3}",
                keyword.ToLowerInvariant(),
                (cat ?? "").Trim().ToLowerInvariant(),
                (loc ?? "").Trim().ToLowerInvariant(),
                limit);

            SuggestResponse response = Helper_cl.RuntimeCacheGetOrAdd<SuggestResponse>(
                cacheKey,
                SuggestCacheSeconds,
                () => BuildResponse(keyword, cat, loc, limit));

            WriteJson(context, response ?? new SuggestResponse
            {
                ok = true,
                message = "No suggestion.",
                items = new List<SuggestItem>()
            }, 200);
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "home_search_suggest", ex.StackTrace);
            WriteJson(context, new SuggestResponse
            {
                ok = false,
                message = "Search suggest error.",
                items = new List<SuggestItem>()
            }, 500);
        }
    }

    public bool IsReusable { get { return false; } }

    private static int ParseLimit(string raw)
    {
        int n;
        if (!int.TryParse((raw ?? "").Trim(), out n)) n = 8;
        if (n < 1) n = 8;
        if (n > 12) n = 12;
        return n;
    }

    private static int ComputeClientOrderScore(SuggestItem item, string keyword, string keywordNormalized)
    {
        if (item == null) return 0;

        string title = (item.title ?? "").Trim();
        string safeKeyword = (keyword ?? "").Trim();
        string safeKeywordNormalized = (keywordNormalized ?? "").Trim();
        string titleNormalized = BaiVietSearchSchema_cl.NormalizeText(title);

        int score = 0;
        if (!string.IsNullOrWhiteSpace(safeKeyword))
        {
            if (string.Equals(title, safeKeyword, StringComparison.OrdinalIgnoreCase))
                score += 1200;
            else if (title.StartsWith(safeKeyword, StringComparison.OrdinalIgnoreCase))
                score += 900;
            else if (title.IndexOf(safeKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                score += 400;
        }

        if (!string.IsNullOrWhiteSpace(safeKeywordNormalized))
        {
            if (string.Equals(titleNormalized, safeKeywordNormalized, StringComparison.OrdinalIgnoreCase))
                score += 800;
            else if (titleNormalized.StartsWith(safeKeywordNormalized, StringComparison.OrdinalIgnoreCase))
                score += 500;
        }

        return score;
    }

    private static SuggestResponse BuildResponse(string keyword, string cat, string loc, int limit)
    {
        using (dbDataContext db = new dbDataContext())
        {
            BaiVietSearchSchema_cl.EnsureSchemaSafe(db);
            bool hasSearchColumns = BaiVietSearchSchema_cl.HasSearchColumns(db);

            var visible = AccountVisibility_cl.FilterVisibleTradePosts(db, db.BaiViet_tbs);

            if (!string.IsNullOrWhiteSpace(cat))
                visible = visible.Where(p => p.id_DanhMuc == cat || p.id_DanhMucCap2 == cat);

            if (!string.IsNullOrWhiteSpace(loc))
                visible = visible.Where(p => p.ThanhPho == loc);

            string keywordSlug = "";
            try
            {
                keywordSlug = (new string_class().replace_name_to_url(keyword) ?? "").Trim();
            }
            catch { keywordSlug = ""; }

            string keywordNormalized = hasSearchColumns ? BaiVietSearchSchema_cl.NormalizeText(keyword) : "";

            if (hasSearchColumns)
            {
                visible = visible.Where(p =>
                    (p.name != null && p.name.Contains(keyword)) ||
                    (p.description != null && p.description.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(keywordSlug) && p.name_en != null && p.name_en.Contains(keywordSlug)) ||
                    (!string.IsNullOrEmpty(keywordNormalized) && p.name_khongdau != null && p.name_khongdau.Contains(keywordNormalized)) ||
                    (!string.IsNullOrEmpty(keywordNormalized) && p.description_khongdau != null && p.description_khongdau.Contains(keywordNormalized)));
            }
            else
            {
                visible = visible.Where(p =>
                    (p.name != null && p.name.Contains(keyword)) ||
                    (p.description != null && p.description.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(keywordSlug) && p.name_en != null && p.name_en.Contains(keywordSlug)));
            }

            var rows = visible
                .OrderByDescending(p => (p.LuotTruyCap ?? 0))
                .ThenByDescending(p => p.ngaytao)
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.id_DanhMuc,
                    p.id_DanhMucCap2,
                    p.ThanhPho
                })
                .Take(limit * 6)
                .ToList();

            var catIds = rows
                .Select(r => !string.IsNullOrWhiteSpace(r.id_DanhMucCap2) ? r.id_DanhMucCap2 : r.id_DanhMuc)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();

            var catNames = db.DanhMuc_tbs
                .Where(dm => catIds.Contains(dm.id.ToString()))
                .ToDictionary(dm => dm.id.ToString(), dm => dm.name ?? "");

            var locIds = rows
                .Select(r => (r.ThanhPho ?? "").Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();

            var locNames = db.ThanhPhos
                .Where(tp => locIds.Contains(tp.id.ToString()))
                .ToDictionary(tp => tp.id.ToString(), tp => TinhThanhDisplay_cl.Format(tp.Ten));

            var items = new List<SuggestItem>();
            var dedupe = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var r in rows)
            {
                string title = (r.name ?? "").Trim();
                if (string.IsNullOrWhiteSpace(title))
                    continue;

                string itemKey = title.ToLowerInvariant();
                if (dedupe.Contains(itemKey))
                    continue;
                dedupe.Add(itemKey);

                string catId = !string.IsNullOrWhiteSpace(r.id_DanhMucCap2) ? r.id_DanhMucCap2 : (r.id_DanhMuc ?? "");
                string catName = catId != "" && catNames.ContainsKey(catId) ? (catNames[catId] ?? "") : "";
                string locId = (r.ThanhPho ?? "").Trim();
                string locName = locId != "" && locNames.ContainsKey(locId) ? (locNames[locId] ?? "") : "";

                items.Add(new SuggestItem
                {
                    id = r.id,
                    title = title,
                    categoryId = catId,
                    categoryName = catName,
                    locationId = locId,
                    locationName = locName,
                    url = AhaSearchRoutes_cl.BuildSearchUrl(title, catId, locId, locId, string.Empty, string.Empty, "all", "1"),
                    source = "suggest"
                });
            }

            items = items
                .OrderByDescending(x => ComputeClientOrderScore(x, keyword, keywordNormalized))
                .ThenBy(x => x.title)
                .Take(limit)
                .ToList();

            AppendPopularKeywordSuggestions(db, keyword, limit, items, dedupe);

            return new SuggestResponse
            {
                ok = true,
                message = items.Count == 0 ? "No suggestion." : "OK",
                items = items.Take(limit).ToList()
            };
        }
    }

    private static void AppendPopularKeywordSuggestions(dbDataContext db, string keyword, int limit, List<SuggestItem> items, HashSet<string> dedupe)
    {
        if (items == null || dedupe == null) return;
        if (items.Count >= limit) return;

        HomeTextContent_cl.TextContentItem content = HomeTextContent_cl.GetEffectiveByKey(db, HomeTextContent_cl.KeyPopularKeywords);
        List<HomeTextContent_cl.KeywordLineItem> popularItems = content != null && content.IsEnabled
            ? HomeTextContent_cl.ParseKeywordLines(content.TextContent)
            : new List<HomeTextContent_cl.KeywordLineItem>();

        if (popularItems.Count == 0) return;

        string normalizedKeyword = BaiVietSearchSchema_cl.NormalizeText(keyword ?? "");

        foreach (HomeTextContent_cl.KeywordLineItem line in popularItems)
        {
            string label = (line.Label ?? "").Trim();
            if (string.IsNullOrWhiteSpace(label)) continue;

            string normalizedLabel = BaiVietSearchSchema_cl.NormalizeText(label);
            bool isMatch =
                label.IndexOf(keyword ?? "", StringComparison.OrdinalIgnoreCase) >= 0 ||
                (!string.IsNullOrWhiteSpace(normalizedKeyword) && normalizedLabel.IndexOf(normalizedKeyword, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!isMatch) continue;

            string key = label.ToLowerInvariant();
            if (dedupe.Contains(key)) continue;
            dedupe.Add(key);

            string url = !string.IsNullOrWhiteSpace(line.Url)
                ? line.Url.Trim()
                : AhaSearchRoutes_cl.BuildSearchUrl(label, "", "", "", "", "", "all", "1");

            items.Add(new SuggestItem
            {
                id = 0,
                title = label,
                categoryId = "",
                categoryName = "Từ khóa phổ biến",
                locationId = "",
                locationName = "",
                url = url,
                source = "popular"
            });

            if (items.Count >= limit) break;
        }
    }

    private static void WriteJson(HttpContext context, SuggestResponse payload, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        var serializer = new JavaScriptSerializer();
        context.Response.Write(serializer.Serialize(payload));
    }
}
