<%@ WebHandler Language="C#" Class="AppUiRuntimeSearchHandler" %>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeSearchHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        string space = ((context.Request["space"] ?? "home").Trim() ?? "home").ToLowerInvariant();
        string q = (context.Request["q"] ?? "").Trim();
        int page = ParseInt(context.Request["page"], 1);
        int pageSize = ParseInt(context.Request["page_size"], 20);

        try
        {
            object payload;
            if (space == "batdongsan")
                payload = BuildBatDongSanPayload(q, page, pageSize, context.Request);
            else if (space == "choxe")
                payload = BuildVerticalPayload(space, q, page, pageSize, context.Request, "product", "Xe", "xe");
            else if (space == "dienthoai-maytinh")
                payload = BuildVerticalPayload(space, q, page, pageSize, context.Request, "product", "Công nghệ", "dien thoai");
            else
                payload = BuildHomePayload(q, page, pageSize, context.Request);

            context.Response.Write(new JavaScriptSerializer().Serialize(payload));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_search", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write("{\"ok\":false,\"items\":[],\"total_items\":0,\"summary_text\":\"Lỗi tải dữ liệu.\"}");
        }
    }

    public bool IsReusable { get { return false; } }

    private sealed class HomeFeedEntry
    {
        public string Key { get; set; }
        public DateTime SortAt { get; set; }
        public object Item { get; set; }
    }

    private sealed class NativeHomeSeed
    {
        public string Key { get; set; }
        public string Identity { get; set; }
        public DateTime SortAt { get; set; }
        public object Item { get; set; }
    }

    private static object BuildHomePayload(string q, int page, int pageSize, HttpRequest request)
    {
        int safePage = page <= 0 ? 1 : page;
        int safeSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 50);

        List<HomeFeedEntry> feed = new List<HomeFeedEntry>();
        HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> seenIdentity = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        Action<string, string, DateTime, object> addFeed = (key, identityKey, sortAt, item) =>
        {
            string safeKey = (key ?? "").Trim();
            if (safeKey == "" || seen.Contains(safeKey))
                return;
            string safeIdentity = (identityKey ?? "").Trim();
            if (safeIdentity != "" && seenIdentity.Contains(safeIdentity))
                return;
            seen.Add(safeKey);
            if (safeIdentity != "")
                seenIdentity.Add(safeIdentity);
            feed.Add(new HomeFeedEntry
            {
                Key = safeKey,
                SortAt = sortAt,
                Item = item
            });
        };

        try
        {
            SearchRequestModel input = SearchRequestMapper.FromHttpRequest(request);
            input.Keyword = q;
            input.SearchType = "all";
            input.Page = 1;
            input.Size = 240;

            SearchService service = new SearchService();
            SearchResultPresenter presenter = new SearchResultPresenter();
            SearchResultModel result = presenter.Present(service.Search(input));

            foreach (SearchResultItemModel item in (result.Items ?? new List<SearchResultItemModel>()))
            {
                if (item == null || item.Id <= 0) continue;
                string source = InferHomeRuntimeSource(item);
                string badge = ResolveHomeBadge(source);
                addFeed(
                    "search:" + item.Id.ToString(),
                    BuildHomeSearchIdentity(item, source),
                    item.CreatedAt ?? DateTime.MinValue,
                    MapHomeItem(item, source, badge));
            }

            if (string.IsNullOrWhiteSpace(q))
            {
                AppendHomeSeedByKeyword(service, presenter, input, "xe", "choxe", "Aha Xe", addFeed);
                AppendHomeSeedByKeyword(service, presenter, input, "dien thoai", "dienthoai-maytinh", "Aha Tech", addFeed);
            }
        }
        catch
        {
            // Host có thể chưa đồng bộ đủ search runtime; Home app vẫn nên có feed native.
        }

        AppendNativeHomePosts(q, addFeed);

        List<LinkedFeedStore_cl.LinkedPost> linkedRows = CoreDb_cl.Use(db => LinkedFeedStore_cl.GetActiveForSearch(db, 600));
        IEnumerable<LinkedFeedStore_cl.LinkedPost> linkedFiltered = linkedRows;
        if (!string.IsNullOrWhiteSpace(q))
        {
            string keyword = Normalize(q);
            linkedFiltered = linkedFiltered.Where(p =>
                Normalize((p.Title ?? "") + " " + (p.Summary ?? "") + " " + (p.PropertyType ?? "") + " " + (p.Province ?? "") + " " + (p.District ?? "")).Contains(keyword));
        }

        foreach (LinkedFeedStore_cl.LinkedPost row in linkedFiltered
            .OrderByDescending(p => p.PublishedAt)
            .ThenByDescending(p => p.Id))
        {
            if (row == null) continue;
            string linkedIdentity = !string.IsNullOrWhiteSpace(row.SourceUrl)
                ? "bds-url:" + Normalize(row.SourceUrl)
                : ("bds-id:" + row.Id.ToString());
            addFeed(
                "bds:" + row.Id.ToString(),
                linkedIdentity,
                row.PublishedAt,
                MapBatDongSanItem(row));
        }

        feed = feed
            .OrderByDescending(x => x.SortAt)
            .ThenByDescending(x => x.Key)
            .ToList();

        if (string.IsNullOrWhiteSpace(q))
            feed = BlendHomeFeed(feed);

        int totalItems = feed.Count;
        List<object> pageItems = feed
            .Skip((safePage - 1) * safeSize)
            .Take(safeSize)
            .Select(x => x.Item)
            .ToList();

        string summary = string.IsNullOrWhiteSpace(q)
            ? ("Home tổng hợp: " + totalItems.ToString() + " tin từ toàn nền tảng.")
            : ("Home tổng hợp: " + totalItems.ToString() + " tin phù hợp từ khóa \"" + q.Trim() + "\".");

        return new
        {
            ok = true,
            source = "runtime-home-aggregate",
            items = pageItems,
            page = safePage,
            page_size = safeSize,
            total_items = totalItems,
            total_pages = safeSize <= 0 ? 1 : (int)Math.Ceiling((double)totalItems / safeSize),
            summary_text = summary,
            page_title = string.IsNullOrWhiteSpace(q) ? "Aha Sale - Tổng hợp" : ("Aha Sale - " + q.Trim())
        };
    }

    private static void AppendNativeHomePosts(string q, Action<string, string, DateTime, object> addFeed)
    {
        if (addFeed == null)
            return;

        List<NativeHomeSeed> rows = CoreDb_cl.Use(db =>
        {
            IQueryable<BaiViet_tb> query = db.BaiViet_tbs.Where(p =>
                (p.bin == null || p.bin == false)
                && (p.phanloai == "sanpham" || p.phanloai == "dichvu"));

            string keyword = (q ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    (p.name ?? "").Contains(keyword)
                    || (p.name_en ?? "").Contains(keyword)
                    || (p.description ?? "").Contains(keyword));
            }

            var rawRows = query
                .OrderByDescending(p => p.ngaytao)
                .ThenByDescending(p => p.id)
                .Take(string.IsNullOrWhiteSpace(keyword) ? 180 : 120)
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.name_en,
                    p.description,
                    p.image,
                    p.giaban,
                    p.ngaytao,
                    p.id_DanhMuc,
                    p.id_DanhMucCap2,
                    p.ThanhPho,
                    p.phanloai
                })
                .ToList();

            Dictionary<string, string> categoryNames = db.DanhMuc_tbs
                .ToDictionary(p => p.id.ToString(), p => (p.name ?? "").Trim(), StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> locationNames = db.ThanhPhos
                .ToDictionary(p => p.id.ToString(), p => TinhThanhDisplay_cl.Format(p.Ten), StringComparer.OrdinalIgnoreCase);

            return rawRows.Select(p =>
            {
                string categoryId = !string.IsNullOrWhiteSpace(p.id_DanhMucCap2) ? p.id_DanhMucCap2 : (p.id_DanhMuc ?? "");
                string categoryName = categoryNames.ContainsKey(categoryId) ? categoryNames[categoryId] : "";
                string locationKey = (p.ThanhPho ?? "").Trim();
                string locationName = locationNames.ContainsKey(locationKey) ? locationNames[locationKey] : "";
                string source = InferHomeRuntimeSourceByText((p.name ?? "") + " " + (p.description ?? ""), categoryName);
                string badge = ResolveHomeBadge(source);
                return new NativeHomeSeed
                {
                    Key = "native:" + p.id.ToString(),
                    Identity = "native-id:" + p.id.ToString(),
                    SortAt = p.ngaytao ?? DateTime.MinValue,
                    Item = new
                    {
                        id = "runtime-home-" + p.id.ToString(),
                        runtime_id = p.id,
                        runtime_source = source,
                        title = (p.name ?? "").Trim(),
                        summary = (p.description ?? "").Trim(),
                        image = ResolveImage(p.image),
                        category = string.IsNullOrWhiteSpace(categoryName) ? (p.phanloai == "dichvu" ? "Dịch vụ" : "Sản phẩm") : categoryName,
                        location = locationName,
                        price = FormatPrice(p.giaban ?? 0m),
                        meta = p.ngaytao.HasValue ? ("Cập nhật " + p.ngaytao.Value.ToString("dd/MM/yyyy")) : "Tin từ ahasale.vn",
                        badge = badge,
                        detail_url = ""
                    }
                };
            }).ToList();
        });

        foreach (dynamic row in rows)
        {
            addFeed(
                row.Key,
                row.Identity,
                row.SortAt,
                row.Item);
        }
    }

    private static List<HomeFeedEntry> BlendHomeFeed(List<HomeFeedEntry> feed)
    {
        if (feed == null || feed.Count <= 1)
            return feed ?? new List<HomeFeedEntry>();

        string[] preferredOrder = new[] { "home", "batdongsan", "choxe", "dienthoai-maytinh" };
        Dictionary<string, Queue<HomeFeedEntry>> buckets = preferredOrder.ToDictionary(
            key => key,
            key => new Queue<HomeFeedEntry>(),
            StringComparer.OrdinalIgnoreCase);

        List<HomeFeedEntry> remaining = new List<HomeFeedEntry>();
        foreach (HomeFeedEntry entry in feed)
        {
            string source = GetFeedRuntimeSource(entry);
            Queue<HomeFeedEntry> bucket;
            if (buckets.TryGetValue(source, out bucket))
                bucket.Enqueue(entry);
            else
                remaining.Add(entry);
        }

        List<HomeFeedEntry> blended = new List<HomeFeedEntry>();
        bool advanced = true;
        while (advanced)
        {
            advanced = false;
            for (int i = 0; i < preferredOrder.Length; i += 1)
            {
                Queue<HomeFeedEntry> bucket = buckets[preferredOrder[i]];
                if (bucket.Count <= 0)
                    continue;

                blended.Add(bucket.Dequeue());
                advanced = true;
            }
        }

        blended.AddRange(remaining);
        return blended;
    }

    private static string GetFeedRuntimeSource(HomeFeedEntry entry)
    {
        if (entry == null || entry.Item == null)
            return "home";

        var prop = entry.Item.GetType().GetProperty("runtime_source");
        if (prop == null)
            return "home";

        string value = ((prop.GetValue(entry.Item, null) ?? "").ToString()).Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(value) ? "home" : value;
    }

    private static void AppendHomeSeedByKeyword(
        SearchService service,
        SearchResultPresenter presenter,
        SearchRequestModel baseInput,
        string keyword,
        string runtimeSource,
        string badge,
        Action<string, string, DateTime, object> addFeed)
    {
        if (service == null || presenter == null || baseInput == null || addFeed == null)
            return;

        SearchRequestModel seed = new SearchRequestModel
        {
            Keyword = keyword,
            SearchType = "product",
            CategoryId = baseInput.CategoryId,
            LocationId = baseInput.LocationId,
            ProvinceId = baseInput.ProvinceId,
            DistrictId = baseInput.DistrictId,
            WardId = baseInput.WardId,
            Page = 1,
            Size = 120
        };

        List<SearchResultItemModel> scopedSeeds = (presenter.Present(service.Search(seed)).Items ?? new List<SearchResultItemModel>())
            .Where(item => item != null && item.Id > 0)
            .ToList();

        // Nếu keyword seed không trả dữ liệu (hay gặp ở Xe/Công nghệ),
        // mở rộng truy vấn và lọc theo không gian để Home vẫn có đủ luồng.
        if (scopedSeeds.Count == 0
            && (string.Equals(runtimeSource, "choxe", StringComparison.OrdinalIgnoreCase)
                || string.Equals(runtimeSource, "dienthoai-maytinh", StringComparison.OrdinalIgnoreCase)))
        {
            SearchRequestModel broad = new SearchRequestModel
            {
                Keyword = string.Empty,
                SearchType = string.Empty,
                CategoryId = baseInput.CategoryId,
                LocationId = baseInput.LocationId,
                ProvinceId = baseInput.ProvinceId,
                DistrictId = baseInput.DistrictId,
                WardId = baseInput.WardId,
                Page = 1,
                Size = 180
            };

            scopedSeeds = (presenter.Present(service.Search(broad)).Items ?? new List<SearchResultItemModel>())
                .Where(item => item != null
                    && item.Id > 0
                    && MatchesHomeSeedRuntimeSource(item, runtimeSource))
                .Take(24)
                .ToList();
        }

        foreach (SearchResultItemModel item in scopedSeeds)
        {
            addFeed(
                "seed-" + runtimeSource + ":" + item.Id.ToString(),
                BuildHomeSearchIdentity(item, runtimeSource),
                item.CreatedAt ?? DateTime.MinValue,
                MapHomeItem(item, runtimeSource, badge));
        }
    }

    private static string InferHomeRuntimeSource(SearchResultItemModel item)
    {
        string category = Normalize(item == null ? "" : (item.CategoryName ?? ""));
        string title = Normalize(item == null ? "" : (item.Title ?? ""));
        return InferHomeRuntimeSourceByText(title, category);
    }

    private static string InferHomeRuntimeSourceByText(string titleRaw, string categoryRaw)
    {
        string category = Normalize(categoryRaw);
        string title = Normalize(titleRaw);
        string haystack = (category + " " + title).Trim();

        if (haystack.Contains("bat dong san") || haystack.Contains("nha ") || haystack.Contains("dat nen") || haystack.Contains("can ho"))
            return "batdongsan";
        if (haystack.Contains("xe") || haystack.Contains("o to") || haystack.Contains("xe may") || haystack.Contains("vinfast"))
            return "choxe";
        if (haystack.Contains("dien thoai") || haystack.Contains("may tinh") || haystack.Contains("laptop") || haystack.Contains("iphone"))
            return "dienthoai-maytinh";
        return "home";
    }

    private static bool MatchesHomeSeedRuntimeSource(SearchResultItemModel item, string runtimeSource)
    {
        if (item == null || string.IsNullOrWhiteSpace(runtimeSource))
            return false;

        string source = runtimeSource.Trim().ToLowerInvariant();
        string text = Normalize((item.Title ?? "") + " " + (item.CategoryName ?? "") + " " + (item.Summary ?? ""));
        if (source == "choxe")
        {
            string[] tokens = new[] {
                "xe", "o to", "oto", "xe may", "suv", "sedan", "toyota", "honda", "kia", "ford", "yamaha", "vinfast", "mazda", "hyundai"
            };
            return tokens.Any(token => text.Contains(token));
        }
        if (source == "dienthoai-maytinh")
        {
            string[] tokens = new[] {
                "dien thoai", "iphone", "samsung", "xiaomi", "laptop", "macbook", "may tinh", "ipad", "pc", "ssd", "ram"
            };
            return tokens.Any(token => text.Contains(token));
        }
        return string.Equals(InferHomeRuntimeSource(item), source, StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveHomeBadge(string runtimeSource)
    {
        if (string.Equals(runtimeSource, "batdongsan", StringComparison.OrdinalIgnoreCase))
            return "Aha Land";
        if (string.Equals(runtimeSource, "choxe", StringComparison.OrdinalIgnoreCase))
            return "Aha Xe";
        if (string.Equals(runtimeSource, "dienthoai-maytinh", StringComparison.OrdinalIgnoreCase))
            return "Aha Tech";
        return "Aha Sale";
    }

    private static string BuildHomeSearchIdentity(SearchResultItemModel item, string runtimeSource)
    {
        if (item == null)
            return "";

        string source = string.IsNullOrWhiteSpace(runtimeSource) ? "home" : runtimeSource.Trim().ToLowerInvariant();
        string detailUrl = Normalize(item.DetailUrl);
        if (detailUrl != "")
            return "search-url:" + source + ":" + detailUrl;

        if (item.Id > 0)
            return "search-id:" + source + ":" + item.Id.ToString(CultureInfo.InvariantCulture);

        string title = Normalize(item.Title);
        string category = Normalize(item.CategoryName);
        string location = Normalize(item.LocationName);
        string price = item.Price.ToString("0.##", CultureInfo.InvariantCulture);
        string image = Normalize(item.ImageUrl);
        return "search-fallback:" + source + ":" + title + "|" + category + "|" + location + "|" + price + "|" + image;
    }

    private static object BuildBatDongSanPayload(string q, int page, int pageSize, HttpRequest request)
    {
        string transactionType = (request["transaction_type"] ?? "").Trim().ToLowerInvariant();
        string propertyType = (request["property_type"] ?? "").Trim();
        string priceBucket = (request["price_bucket"] ?? "").Trim().ToLowerInvariant();
        int areaMin = ParseInt(request["area_min"], 0);
        string sort = (request["sort"] ?? "newest").Trim().ToLowerInvariant();

        List<LinkedFeedStore_cl.LinkedPost> rows = CoreDb_cl.Use(db => LinkedFeedStore_cl.GetActiveForSearch(db, 400));
        IEnumerable<LinkedFeedStore_cl.LinkedPost> filtered = rows;

        if (!string.IsNullOrWhiteSpace(q))
        {
            string keyword = Normalize(q);
            filtered = filtered.Where(p => Normalize((p.Title ?? "") + " " + (p.Summary ?? "") + " " + (p.PropertyType ?? "") + " " + (p.Province ?? "") + " " + (p.District ?? "")).Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(propertyType))
        {
            string expected = Normalize(propertyType);
            filtered = filtered.Where(p => Normalize(p.PropertyType).Contains(expected));
        }

        if (transactionType == "mua_ban")
            filtered = filtered.Where(p => !string.Equals((p.Purpose ?? "").Trim(), "rent", StringComparison.OrdinalIgnoreCase));
        else if (transactionType == "cho_thue")
            filtered = filtered.Where(p => string.Equals((p.Purpose ?? "").Trim(), "rent", StringComparison.OrdinalIgnoreCase));

        if (areaMin > 0)
            filtered = filtered.Where(p => ParseArea(p.AreaText) >= areaMin);

        if (priceBucket == "duoi_3_ty")
            filtered = filtered.Where(p => ParsePriceMillion(p.PriceText) > 0 && ParsePriceMillion(p.PriceText) < 3000m);
        else if (priceBucket == "tren_3_ty")
            filtered = filtered.Where(p => ParsePriceMillion(p.PriceText) >= 3000m);

        filtered = SortBatDongSan(filtered, sort);
        List<LinkedFeedStore_cl.LinkedPost> all = filtered.ToList();

        int safePage = page <= 0 ? 1 : page;
        int safeSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 50);
        List<LinkedFeedStore_cl.LinkedPost> slice = all.Skip((safePage - 1) * safeSize).Take(safeSize).ToList();

        return new
        {
            ok = true,
            source = "runtime",
            items = slice.Select(MapBatDongSanItem).ToList(),
            page = safePage,
            page_size = safeSize,
            total_items = all.Count,
            total_pages = safeSize <= 0 ? 1 : (int)Math.Ceiling((double)all.Count / safeSize),
            summary_text = "Tìm thấy " + all.Count.ToString() + " tin bất động sản phù hợp.",
            page_title = string.IsNullOrWhiteSpace(q) ? "Aha Land" : ("Aha Land: " + q)
        };
    }

    private static object BuildVerticalPayload(
        string space,
        string q,
        int page,
        int pageSize,
        HttpRequest request,
        string forcedSearchType,
        string verticalLabel,
        string defaultKeyword)
    {
        int safePage = page <= 0 ? 1 : page;
        int safeSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 50);

        SearchRequestModel input = SearchRequestMapper.FromHttpRequest(request);
        string safeSpace = string.IsNullOrWhiteSpace(space) ? "home" : space.Trim().ToLowerInvariant();
        input.Keyword = string.IsNullOrWhiteSpace(q) ? defaultKeyword : q.Trim();
        input.SearchType = AhaSearchRoutes_cl.NormalizeSearchType(forcedSearchType);
        input.Page = 1;
        input.Size = 200;

        SearchService service = new SearchService();
        SearchResultPresenter presenter = new SearchResultPresenter();
        SearchResultModel result = presenter.Present(service.Search(input));

        List<SearchResultItemModel> scoped = (result.Items ?? new List<SearchResultItemModel>())
            .Where(item => item != null
                && item.Id > 0
                && string.Equals(InferHomeRuntimeSource(item), safeSpace, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Nếu vertical chưa có dữ liệu theo keyword mặc định, mở rộng truy vấn theo toàn nền tảng rồi lọc lại theo không gian.
        if (scoped.Count == 0 && string.IsNullOrWhiteSpace(q))
        {
            input.Keyword = string.Empty;
            input.SearchType = string.Empty;
            SearchResultModel broadResult = presenter.Present(service.Search(input));
            scoped = (broadResult.Items ?? new List<SearchResultItemModel>())
                .Where(item => item != null
                    && item.Id > 0
                    && string.Equals(InferHomeRuntimeSource(item), safeSpace, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        List<object> source = scoped
            .Select(item => MapVerticalItem(item, safeSpace, verticalLabel))
            .Cast<object>()
            .ToList();
        List<object> slice = source.Skip((safePage - 1) * safeSize).Take(safeSize).ToList();

        string summaryKeyword = string.IsNullOrWhiteSpace(q) ? "mặc định" : q.Trim();
        return new
        {
            ok = true,
            source = "runtime",
            items = slice,
            page = safePage,
            page_size = safeSize,
            total_items = source.Count,
            total_pages = safeSize <= 0 ? 1 : (int)Math.Ceiling((double)source.Count / safeSize),
            summary_text = "Tìm thấy " + source.Count.ToString() + " tin " + verticalLabel + " phù hợp với từ khóa \"" + summaryKeyword + "\".",
            page_title = string.IsNullOrWhiteSpace(q) ? ("Aha " + verticalLabel) : ("Aha " + verticalLabel + ": " + q.Trim())
        };
    }

    private static object MapHomeItem(SearchResultItemModel item, string runtimeSource, string badge)
    {
        return new
        {
            id = "runtime-home-" + item.Id.ToString(),
            runtime_id = item.Id,
            runtime_source = string.IsNullOrWhiteSpace(runtimeSource) ? "home" : runtimeSource,
            title = item.Title ?? "",
            summary = item.Summary ?? "",
            image = ResolveImage(item.ImageUrl),
            category = string.IsNullOrWhiteSpace(item.CategoryName) ? "Tin đăng" : item.CategoryName,
            location = item.LocationName ?? "",
            price = FormatPrice(item.Price),
            meta = item.CreatedAt.HasValue ? ("Cập nhật " + item.CreatedAt.Value.ToString("dd/MM/yyyy")) : "Tin từ nền tảng hiện tại",
            badge = string.IsNullOrWhiteSpace(badge) ? "Aha Sale" : badge,
            detail_url = item.DetailUrl ?? ""
        };
    }

    private static object MapVerticalItem(SearchResultItemModel item, string space, string verticalLabel)
    {
        string safeSpace = string.IsNullOrWhiteSpace(space) ? "home" : space.Trim().ToLowerInvariant();
        string fallbackCategory = string.Equals(safeSpace, "choxe", StringComparison.OrdinalIgnoreCase)
            ? "Xe"
            : "Điện thoại, máy tính";
        return new
        {
            id = "runtime-" + safeSpace + "-" + item.Id.ToString(),
            runtime_id = item.Id,
            runtime_source = safeSpace,
            title = item.Title ?? "",
            summary = item.Summary ?? "",
            image = ResolveImage(item.ImageUrl),
            category = string.IsNullOrWhiteSpace(item.CategoryName) ? fallbackCategory : item.CategoryName,
            location = item.LocationName ?? "",
            price = FormatPrice(item.Price),
            meta = item.CreatedAt.HasValue ? ("Cập nhật " + item.CreatedAt.Value.ToString("dd/MM/yyyy")) : "Tin từ nền tảng hiện tại",
            badge = "Aha " + verticalLabel,
            detail_url = item.DetailUrl ?? ""
        };
    }

    private static object MapBatDongSanItem(LinkedFeedStore_cl.LinkedPost item)
    {
        string district = (item.District ?? "").Trim();
        string province = (item.Province ?? "").Trim();
        string location = district != "" && province != "" ? (district + ", " + province) : (district != "" ? district : province);
        string summary = BatDongSanService_cl.SanitizeLinkedText(item.Summary ?? "", 600, true);
        if (string.IsNullOrWhiteSpace(summary))
            summary = (item.Title ?? "").Trim();
        return new
        {
            id = "runtime-bds-" + item.Id.ToString(),
            runtime_id = item.Id,
            runtime_source = "batdongsan",
            title = item.Title ?? "",
            summary = summary,
            image = ResolveImage(item.ThumbnailUrl),
            category = string.IsNullOrWhiteSpace(item.PropertyType) ? "Bất động sản" : item.PropertyType,
            location = location,
            price = string.IsNullOrWhiteSpace(item.PriceText) ? "Liên hệ" : item.PriceText,
            meta = string.IsNullOrWhiteSpace(item.AreaText) ? "Tin liên kết" : item.AreaText,
            badge = string.Equals((item.Purpose ?? "").Trim(), "rent", StringComparison.OrdinalIgnoreCase) ? "Cho thuê" : "Mua bán",
            detail_url = item.SourceUrl ?? ""
        };
    }

    private static List<object> ApplyVerticalHeuristic(List<object> items, string verticalLabel)
    {
        if (items == null || items.Count == 0)
            return new List<object>();

        List<string> tokens;
        if (string.Equals(verticalLabel, "Xe", StringComparison.OrdinalIgnoreCase))
        {
            tokens = new List<string> {
                "xe", "ô tô", "o to", "oto", "xe máy", "xe may", "suv", "sedan", "honda", "toyota", "kia", "ford", "yamaha", "vinfast"
            };
        }
        else
        {
            tokens = new List<string> {
                "điện thoại", "dien thoai", "iphone", "samsung", "xiaomi", "laptop", "macbook", "máy tính", "may tinh", "ipad", "pc", "ssd", "ram"
            };
        }

        return items
            .Where(item =>
            {
                object titleObj = item.GetType().GetProperty("title") != null ? item.GetType().GetProperty("title").GetValue(item, null) : null;
                object categoryObj = item.GetType().GetProperty("category") != null ? item.GetType().GetProperty("category").GetValue(item, null) : null;
                object summaryObj = item.GetType().GetProperty("summary") != null ? item.GetType().GetProperty("summary").GetValue(item, null) : null;
                string text = Normalize((titleObj ?? "").ToString() + " " + (categoryObj ?? "").ToString() + " " + (summaryObj ?? "").ToString());
                return tokens.Any(token => text.Contains(token));
            })
            .ToList();
    }

    private static IEnumerable<LinkedFeedStore_cl.LinkedPost> SortBatDongSan(IEnumerable<LinkedFeedStore_cl.LinkedPost> items, string sort)
    {
        switch (sort)
        {
            case "price_asc":
                return items.OrderBy(p => ParsePriceMillion(p.PriceText)).ThenByDescending(p => p.PublishedAt);
            case "price_desc":
                return items.OrderByDescending(p => ParsePriceMillion(p.PriceText)).ThenByDescending(p => p.PublishedAt);
            case "area_desc":
                return items.OrderByDescending(p => ParseArea(p.AreaText)).ThenByDescending(p => p.PublishedAt);
            default:
                return items.OrderByDescending(p => p.PublishedAt).ThenByDescending(p => p.Id);
        }
    }

    private static string ResolveImage(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "/app-ui/assets/placeholder-media.svg";
        if (value.StartsWith("~/", StringComparison.Ordinal))
            value = value.Substring(1);
        if (!value.StartsWith("/", StringComparison.Ordinal) && !value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            value = "/" + value;
        return value;
    }

    private static string FormatPrice(decimal price)
    {
        if (price <= 0) return "Liên hệ";
        return price.ToString("#,##0") + " đ";
    }

    private static string Normalize(string text)
    {
        return (text ?? "")
            .Trim()
            .ToLowerInvariant()
            .Replace("đ", "d");
    }

    private static int ParseArea(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        string digits = new string(text.TakeWhile(c => char.IsDigit(c) || c == ' ').ToArray()).Trim();
        int value;
        return int.TryParse(digits, out value) ? value : 0;
    }

    private static decimal ParsePriceMillion(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0m;
        string normalized = Normalize(text).Replace(",", ".");
        decimal value = 0m;
        string token = new string(normalized.TakeWhile(c => char.IsDigit(c) || c == '.' || c == ' ').ToArray()).Trim();
        decimal.TryParse(token, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
        if (normalized.Contains("ty")) return value * 1000m;
        if (normalized.Contains("trieu")) return value;
        return value;
    }

    private static int ParseInt(string raw, int fallback)
    {
        int value;
        return int.TryParse((raw ?? "").Trim(), out value) ? value : fallback;
    }
}
