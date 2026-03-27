using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

public static class GianHangPublic_cl
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    public sealed class StorefrontContextInfo
    {
        public string AccountKey { get; set; }
        public string ChiNhanhId { get; set; }
        public string ReturnUrl { get; set; }
        public string HomeUrl { get; set; }
        public string BookingUrl { get; set; }
        public string CartUrl { get; set; }
        public string ServicesUrl { get; set; }
        public string ProductsUrl { get; set; }
        public string ArticlesUrl { get; set; }
    }

    public sealed class PublicSummary
    {
        public string AccountKey { get; set; }
        public string StoreName { get; set; }
        public string OwnerName { get; set; }
        public string AvatarUrl { get; set; }
        public string CoverUrl { get; set; }
        public string Description { get; set; }
        public string PublicUrl { get; set; }
        public string PublicAbsoluteUrl { get; set; }
        public int ProductCount { get; set; }
        public int ServiceCount { get; set; }
        public int TotalViews { get; set; }
        public int TotalSold { get; set; }
    }

    public sealed class PublicProductView
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        public string ShopAccountKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Views { get; set; }
        public int SoldCount { get; set; }
        public string ProductType { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsService { get; set; }
        public string ActionLabel { get; set; }

        public int HomePostId { get { return ReferenceId; } set { ReferenceId = value; } }
    }

    private sealed class ProductRaw
    {
        public int id { get; set; }
        public int? reference_id { get; set; }
        public string shop_taikhoan { get; set; }
        public string ten { get; set; }
        public string mo_ta { get; set; }
        public string hinh_anh { get; set; }
        public decimal? gia_ban { get; set; }
        public DateTime? ngay_tao { get; set; }
        public int luot_truy_cap { get; set; }
        public int so_luong_da_ban { get; set; }
        public string loai { get; set; }
        public string id_danhmuc { get; set; }
    }

    public static string ResolveRequestedAccountKey(string requestedUser)
    {
        string accountKey = (requestedUser ?? string.Empty).Trim().ToLowerInvariant();
        if (accountKey != string.Empty)
            return accountKey;

        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
        if (info != null && info.IsAuthenticated && info.CanAccessGianHang)
            return (info.AccountKey ?? string.Empty).Trim().ToLowerInvariant();

        return string.Empty;
    }

    public static taikhoan_tb ResolveStoreAccount(dbDataContext db, string requestedUser)
    {
        if (db == null)
            return null;

        string accountKey = ResolveRequestedAccountKey(requestedUser);
        if (accountKey == string.Empty)
            return null;

        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, accountKey);
        if (account == null)
            return null;

        if (!SpaceAccess_cl.CanAccessGianHang(db, account))
            return null;

        return account;
    }

    public static StorefrontContextInfo ResolveContext(dbDataContext db, HttpRequest request)
    {
        string accountKey = ResolveCurrentStoreAccountKey(db, request);
        string returnUrl = ResolveReturnUrl(request == null ? null : request.RawUrl, BuildStorefrontUrl(accountKey));

        return new StorefrontContextInfo
        {
            AccountKey = accountKey,
            ChiNhanhId = ResolveCurrentChiNhanhId(db, request),
            ReturnUrl = returnUrl,
            HomeUrl = BuildStorefrontUrl(accountKey),
            BookingUrl = BuildContextBookingUrl(accountKey, returnUrl),
            CartUrl = BuildContextCartUrl(accountKey, returnUrl),
            ServicesUrl = GianHangRoutes_cl.BuildServicesUrl(accountKey),
            ProductsUrl = GianHangRoutes_cl.BuildProductsUrl(accountKey),
            ArticlesUrl = GianHangRoutes_cl.BuildArticlesUrl(accountKey)
        };
    }

    public static string ResolveCurrentStoreAccountKey(dbDataContext db, HttpRequest request)
    {
        string requestedUser = request == null || request.QueryString == null
            ? string.Empty
            : (request.QueryString["user"] ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(requestedUser))
            return requestedUser.Trim().ToLowerInvariant();

        GH_SanPham_tb requestedItem = ResolveRequestItem(db, request);
        if (requestedItem != null && !string.IsNullOrWhiteSpace(requestedItem.shop_taikhoan))
            return requestedItem.shop_taikhoan.Trim().ToLowerInvariant();

        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
        if (!IsPublicFacingRequest(request))
        {
            if (info != null && info.IsAuthenticated && info.CanAccessGianHang && !string.IsNullOrWhiteSpace(info.AccountKey))
                return info.AccountKey.Trim().ToLowerInvariant();
        }

        string activeShop = GianHangCart_cl.GetActiveStorefrontAccount();
        if (!string.IsNullOrWhiteSpace(activeShop))
            return activeShop.Trim().ToLowerInvariant();

        if (info != null && info.IsAuthenticated && info.CanAccessGianHang && !string.IsNullOrWhiteSpace(info.AccountKey))
            return info.AccountKey.Trim().ToLowerInvariant();

        return string.Empty;
    }

    public static string ResolveCurrentChiNhanhId(dbDataContext db, HttpRequest request, string fallback = null)
    {
        string accountKey = ResolveCurrentStoreAccountKey(db, request);
        if (db != null && !string.IsNullOrWhiteSpace(accountKey))
            return ResolveChiNhanhIdForStoreAccount(db, accountKey, fallback);

        string explicitFallback = (fallback ?? string.Empty).Trim();
        if (explicitFallback != string.Empty)
            return explicitFallback;

        return "1";
    }

    public static string ResolveChiNhanhIdForStoreAccount(dbDataContext db, string accountKey, string fallback = null)
    {
        string explicitFallback = (fallback ?? string.Empty).Trim();
        if (explicitFallback != string.Empty)
            return explicitFallback;

        string normalizedAccount = (accountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (db != null && normalizedAccount != string.Empty)
        {
            taikhoan_table_2023 directAccount = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == normalizedAccount);
            if (directAccount != null && !string.IsNullOrWhiteSpace(directAccount.id_chinhanh))
                return directAccount.id_chinhanh.Trim();

            taikhoan_table_2023 childAccount = db.taikhoan_table_2023s
                .FirstOrDefault(p => p.user_parent == normalizedAccount && p.id_chinhanh != null && p.id_chinhanh != "");
            if (childAccount != null && !string.IsNullOrWhiteSpace(childAccount.id_chinhanh))
                return childAccount.id_chinhanh.Trim();

            chinhanh_table chiNhanh = db.chinhanh_tables.FirstOrDefault(p => p.taikhoan_quantri == normalizedAccount);
            if (chiNhanh != null)
                return chiNhanh.id.ToString();
        }

        HttpContext context = HttpContext.Current;
        if (context != null && context.Session != null)
        {
            string sessionFallback = GianHangContext_cl.ResolveSessionChiNhanhId(context.Session);
            if (sessionFallback != string.Empty)
                return sessionFallback;
        }

        return "1";
    }

    public static string AppendUserToUrl(string rawUrl, string accountKey)
    {
        string url = (rawUrl ?? string.Empty).Trim();
        string normalizedAccount = (accountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (url == string.Empty || normalizedAccount == string.Empty)
            return url;
        if (IsStaticUrl(url))
            return url;
        Uri absoluteUrl;
        if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUrl))
            return url;
        if (!url.StartsWith("/", StringComparison.Ordinal))
            return url;
        if (url.IndexOf("user=", StringComparison.OrdinalIgnoreCase) >= 0)
            return url;

        int hashIndex = url.IndexOf('#');
        string hash = string.Empty;
        if (hashIndex >= 0)
        {
            hash = url.Substring(hashIndex);
            url = url.Substring(0, hashIndex);
        }

        url += (url.IndexOf('?') >= 0 ? "&" : "?") + "user=" + HttpUtility.UrlEncode(normalizedAccount);
        return url + hash;
    }

    public static string BuildContextMenuUrl(dbDataContext db, HttpRequest request, object idObject, object phanloaiObject, object urlOtherObject)
    {
        string accountKey = ResolveCurrentStoreAccountKey(db, request);
        string url = GianHangStorefront_cl.ResolveMenuUrl(idObject, phanloaiObject, urlOtherObject);
        return AppendUserToUrl(url, accountKey);
    }

    public static string BuildContextStorefrontUrl(dbDataContext db, HttpRequest request)
    {
        return BuildStorefrontUrl(ResolveCurrentStoreAccountKey(db, request));
    }

    public static string BuildContextBookingUrl(dbDataContext db, HttpRequest request)
    {
        StorefrontContextInfo context = ResolveContext(db, request);
        return context.BookingUrl;
    }

    public static string BuildContextCartUrl(dbDataContext db, HttpRequest request)
    {
        StorefrontContextInfo context = ResolveContext(db, request);
        return context.CartUrl;
    }

    public static PublicSummary BuildSummary(dbDataContext db, taikhoan_tb account, Uri requestUrl)
    {
        if (db == null || account == null)
            return new PublicSummary();

        string accountKey = (account.taikhoan ?? string.Empty).Trim().ToLowerInvariant();
        List<GH_SanPham_tb> products = GianHangProduct_cl.QueryPublicByStorefront(db, accountKey).ToList();
        int productCount = products.Count(p => GianHangProduct_cl.NormalizeLoai(p.loai) == GianHangProduct_cl.LoaiSanPham);
        int serviceCount = products.Count(p => GianHangProduct_cl.NormalizeLoai(p.loai) == GianHangProduct_cl.LoaiDichVu);
        Dictionary<int, int> soldMap = BuildNativeSoldCountMap(db, accountKey, products.Select(p => p.id));
        int totalSold = 0;
        for (int i = 0; i < products.Count; i++)
        {
            GH_SanPham_tb product = products[i];
            int nativeSold;
            if (!soldMap.TryGetValue(product.id, out nativeSold))
                nativeSold = product.so_luong_da_ban ?? 0;
            totalSold += nativeSold;
        }
        string publicUrl = BuildStorefrontUrl(accountKey);

        return new PublicSummary
        {
            AccountKey = accountKey,
            StoreName = GianHangStorefront_cl.ResolveStorefrontName(account),
            OwnerName = string.IsNullOrWhiteSpace(account.hoten) ? accountKey : account.hoten.Trim(),
            AvatarUrl = GianHangStorefront_cl.ResolveAvatarUrl(account),
            CoverUrl = GianHangStorefront_cl.ResolveImageUrl((account.anhbia_shop ?? string.Empty).Trim()),
            Description = ResolveDescription(account),
            PublicUrl = publicUrl,
            PublicAbsoluteUrl = BuildAbsoluteUrl(requestUrl, publicUrl),
            ProductCount = productCount,
            ServiceCount = serviceCount,
            TotalViews = products.Sum(p => p.luot_truy_cap ?? 0),
            TotalSold = totalSold
        };
    }

    public static List<PublicProductView> LoadProducts(dbDataContext db, string shopAccountKey)
    {
        string accountKey = (shopAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || accountKey == string.Empty)
            return new List<PublicProductView>();

        List<ProductRaw> rawProducts = GianHangProduct_cl.QueryPublicByStorefront(db, accountKey)
            .OrderByDescending(p => p.ngay_tao)
            .Select(p => new ProductRaw
            {
                id = p.id,
                reference_id = p.id_baiviet ?? p.id,
                shop_taikhoan = p.shop_taikhoan,
                ten = p.ten,
                mo_ta = p.mo_ta,
                hinh_anh = p.hinh_anh,
                gia_ban = p.gia_ban,
                ngay_tao = p.ngay_tao,
                luot_truy_cap = p.luot_truy_cap ?? 0,
                so_luong_da_ban = p.so_luong_da_ban ?? 0,
                loai = p.loai,
                id_danhmuc = p.id_danhmuc
            })
            .ToList();

        Dictionary<string, string> categoryMap = ResolveCategoryMap(db, rawProducts);
        Dictionary<int, int> soldMap = BuildNativeSoldCountMap(db, accountKey, rawProducts.Select(p => p.id));
        List<PublicProductView> list = new List<PublicProductView>(rawProducts.Count);
        for (int i = 0; i < rawProducts.Count; i++)
        {
            ProductRaw item = rawProducts[i];
            string normalizedType = GianHangProduct_cl.NormalizeLoai(item.loai);
            string categoryName = string.Empty;
            string categoryId = (item.id_danhmuc ?? string.Empty).Trim();
            int soldCount;
            if (!soldMap.TryGetValue(item.id, out soldCount))
                soldCount = item.so_luong_da_ban;
            if (categoryId != string.Empty)
                categoryMap.TryGetValue(categoryId, out categoryName);

            list.Add(new PublicProductView
            {
                Id = item.id,
                ReferenceId = item.reference_id ?? item.id,
                ShopAccountKey = (item.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant(),
                Name = item.ten ?? string.Empty,
                Description = item.mo_ta ?? string.Empty,
                ImageUrl = GianHangStorefront_cl.ResolveImageUrl(item.hinh_anh),
                Price = item.gia_ban,
                CreatedAt = item.ngay_tao,
                Views = item.luot_truy_cap,
                SoldCount = soldCount,
                ProductType = normalizedType,
                CategoryId = categoryId,
                CategoryName = categoryName ?? string.Empty,
                IsService = normalizedType == GianHangProduct_cl.LoaiDichVu,
                ActionLabel = normalizedType == GianHangProduct_cl.LoaiDichVu ? "Đặt lịch" : "Trao đổi"
            });
        }

        return list;
    }

    public static List<PublicProductView> LoadTopProducts(IList<PublicProductView> products, int take)
    {
        if (products == null || products.Count == 0)
            return new List<PublicProductView>();

        int safeTake = take <= 0 ? 6 : take;
        List<PublicProductView> ordered = products
            .OrderByDescending(p => p.SoldCount)
            .ThenByDescending(p => p.Views)
            .ThenByDescending(p => p.CreatedAt ?? DateTime.MinValue)
            .Take(safeTake)
            .ToList();

        if (ordered.All(p => p.SoldCount <= 0))
        {
            ordered = products
                .OrderByDescending(p => p.Views)
                .ThenByDescending(p => p.CreatedAt ?? DateTime.MinValue)
                .Take(safeTake)
                .ToList();
        }

        return ordered;
    }

    public static GH_SanPham_tb GetPublicItemById(dbDataContext db, int id)
    {
        if (db == null || id <= 0)
            return null;

        GianHangProduct_cl.EnsureSchema(db);
        return db.GetTable<GH_SanPham_tb>()
            .FirstOrDefault(p => p.id == id && (p.bin == null || p.bin == false));
    }

    public static GH_SanPham_tb GetPublicItemById(dbDataContext db, int id, string normalizedType)
    {
        GH_SanPham_tb item = GetPublicItemById(db, id);
        if (item == null)
            return null;

        if ((normalizedType ?? string.Empty).Trim().ToLowerInvariant() == string.Empty)
            return item;

        return GianHangProduct_cl.NormalizeLoai(item.loai) == normalizedType ? item : null;
    }

    public static string BuildStorefrontUrl(string accountKey)
    {
        string normalized = (accountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized == string.Empty)
            return GianHangRoutes_cl.BuildPublicUrl(string.Empty);

        return GianHangRoutes_cl.BuildPublicUrl(normalized);
    }

    public static string BuildDetailUrl(PublicProductView item)
    {
        if (item == null)
            return GianHangRoutes_cl.BuildPublicUrl(string.Empty);

        if (item.IsService)
            return GianHangRoutes_cl.BuildXemDichVuUrl(item.Id);

        return GianHangRoutes_cl.BuildXemSanPhamUrl(item.Id);
    }

    public static string BuildBookingUrl(PublicProductView item, string returnUrlRaw)
    {
        if (item == null)
            return "#";

        return BuildBookingUrl(item.Id, item.ShopAccountKey, returnUrlRaw);
    }

    public static string BuildBookingUrl(int itemId, string shopAccountKey, string returnUrlRaw)
    {
        if (itemId <= 0)
            return "#";

        string returnUrl = ResolveReturnUrl(returnUrlRaw, BuildStorefrontUrl(shopAccountKey));
        return GianHangRoutes_cl.BuildDatLichPublicUrl(itemId, shopAccountKey, returnUrl);
    }

    public static string BuildExchangeUrl(PublicProductView item, string returnUrlRaw)
    {
        if (item == null)
            return "#";
        if (item.IsService)
            return BuildBookingUrl(item, returnUrlRaw);

        return BuildExchangeUrl(item.Id, item.ShopAccountKey, returnUrlRaw);
    }

    public static string BuildExchangeUrl(int publicProductId, string shopAccountKey, string returnUrlRaw)
    {
        if (publicProductId <= 0)
            return "#";

        string returnUrl = ResolveReturnUrl(returnUrlRaw, BuildStorefrontUrl(shopAccountKey));
        return GianHangRoutes_cl.BuildPublicCartUrl(shopAccountKey, publicProductId, 1, returnUrl, true);
    }

    public static string BuildAddCartUrl(PublicProductView item, string returnUrlRaw)
    {
        if (item == null || item.IsService)
            return "#";

        return BuildAddCartUrl(item.Id, item.ShopAccountKey, returnUrlRaw);
    }

    public static string BuildAddCartUrl(int publicProductId, string shopAccountKey, string returnUrlRaw)
    {
        if (publicProductId <= 0)
            return "#";

        string returnUrl = ResolveReturnUrl(returnUrlRaw, BuildStorefrontUrl(shopAccountKey));
        return GianHangRoutes_cl.BuildPublicCartUrl(shopAccountKey, publicProductId, 1, returnUrl, false);
    }

    public static string BuildTypeLabel(string rawType)
    {
        return GianHangProduct_cl.NormalizeLoai(rawType) == GianHangProduct_cl.LoaiDichVu ? "Dịch vụ" : "Sản phẩm";
    }

    public static string BuildTypeCss(string rawType)
    {
        return GianHangProduct_cl.NormalizeLoai(rawType) == GianHangProduct_cl.LoaiDichVu
            ? "card-badge card-badge-service"
            : "card-badge card-badge-product";
    }

    public static string FormatCurrency(decimal? value)
    {
        return (value ?? 0m).ToString("#,##0.##", ViCulture);
    }

    public static string ResolveConfigUrl(string rawUrl, string accountKey, string fallbackUrl)
    {
        string value = GianHangStorefrontConfig_cl.NormalizeStorefrontUrl(rawUrl, accountKey);
        if (string.IsNullOrWhiteSpace(value))
            value = fallbackUrl ?? string.Empty;
        return value;
    }

    private static string ResolveDescription(taikhoan_tb account)
    {
        if (account == null)
            return string.Empty;

        string description = (account.motangan_shop ?? string.Empty).Trim();
        if (description != string.Empty)
            return description;

        if (!string.IsNullOrWhiteSpace(account.hoten))
            return "Không gian gian hàng công khai của " + account.hoten.Trim() + ".";

        return "Không gian gian hàng công khai được mở rộng từ tài khoản Home.";
    }

    private static Dictionary<int, int> BuildNativeSoldCountMap(dbDataContext db, string accountKey, IEnumerable<int> productIds)
    {
        Dictionary<int, int> map = new Dictionary<int, int>();
        if (db == null || string.IsNullOrWhiteSpace(accountKey) || productIds == null)
            return map;

        GianHangInvoice_cl.EnsureSchema(db);
        List<int> ids = productIds
            .Where(p => p > 0)
            .Distinct()
            .ToList();
        if (ids.Count == 0)
            return map;

        List<long> invoiceIds = GianHangInvoice_cl.QueryByStorefront(db, accountKey)
            .Where(p => (p.order_status ?? string.Empty).Trim() != DonHangStateMachine_cl.Order_DaHuy)
            .Select(p => p.id)
            .ToList();
        if (invoiceIds.Count == 0)
            return map;

        return db.GetTable<GH_HoaDon_ChiTiet_tb>()
            .Where(p => invoiceIds.Contains(p.id_hoadon)
                        && p.id_sanpham.HasValue
                        && ids.Contains(p.id_sanpham.Value))
            .GroupBy(p => p.id_sanpham.Value)
            .Select(g => new { id = g.Key, total = g.Sum(x => (int?)x.so_luong) ?? 0 })
            .ToList()
            .ToDictionary(p => p.id, p => p.total);
    }

    private static string BuildAbsoluteUrl(Uri requestUrl, string relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
            return string.Empty;
        if (requestUrl == null)
            return relativeUrl;

        return requestUrl.GetLeftPart(UriPartial.Authority) + relativeUrl;
    }

    private static string ResolveReturnUrl(string returnUrlRaw, string fallbackUrl)
    {
        return GianHangRoutes_cl.NormalizeReturnUrl(returnUrlRaw, fallbackUrl ?? BuildStorefrontUrl(string.Empty));
    }

    private static string BuildContextBookingUrl(string accountKey, string returnUrl)
    {
        return GianHangRoutes_cl.BuildBookingHubUrl(accountKey, returnUrl);
    }

    private static string BuildContextCartUrl(string accountKey, string returnUrl)
    {
        return GianHangRoutes_cl.BuildCartUrl(accountKey, returnUrl);
    }

    private static string BuildContextListUrl(string baseUrl, string accountKey)
    {
        return GianHangRoutes_cl.AppendUserToUrl(baseUrl, accountKey);
    }

    private static GH_SanPham_tb ResolveRequestItem(dbDataContext db, HttpRequest request)
    {
        if (db == null || request == null || request.QueryString == null)
            return null;

        int parsedId;
        string rawId = (request.QueryString["id"] ?? string.Empty).Trim();
        if (int.TryParse(rawId, out parsedId) && parsedId > 0)
        {
            GH_SanPham_tb item = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
            if (item != null)
                return item;
        }

        string rawLegacyId = (request.QueryString["idbv"] ?? string.Empty).Trim();
        if (int.TryParse(rawLegacyId, out parsedId) && parsedId > 0)
            return GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);

        return null;
    }

    private static bool IsStaticUrl(string rawUrl)
    {
        string url = (rawUrl ?? string.Empty).Trim();
        if (url == string.Empty)
            return true;

        return url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("#", StringComparison.Ordinal);
    }

    private static bool IsPublicFacingRequest(HttpRequest request)
    {
        string path = request == null ? string.Empty : (request.Path ?? string.Empty).Trim().ToLowerInvariant();
        if (path == string.Empty)
            return false;

        if (path.StartsWith("/gianhang/page/", StringComparison.OrdinalIgnoreCase))
            return true;

        return path == "/gianhang/public.aspx"
            || path == "/gianhang/giohang.aspx"
            || path == "/gianhang/datlich.aspx"
            || path == "/gianhang/xem-san-pham.aspx"
            || path == "/gianhang/xem-dich-vu.aspx"
            || path == "/gianhang/xoa_chitiet_giohang.aspx"
            || path == "/gianhang/hoa-don-dien-tu.aspx"
            || path == "/gianhang/baotri.aspx";
    }

    private static Dictionary<string, string> ResolveCategoryMap(dbDataContext db, IList<ProductRaw> products)
    {
        Dictionary<string, string> categoryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (db == null || products == null || products.Count == 0)
            return categoryMap;

        List<string> categoryIds = products
            .Select(p => (p.id_danhmuc ?? string.Empty).Trim())
            .Where(p => p != string.Empty)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        List<int> danhMucIds = new List<int>();
        for (int i = 0; i < categoryIds.Count; i++)
        {
            int id;
            if (int.TryParse(categoryIds[i], out id))
                danhMucIds.Add(id);
        }

        List<DanhMuc_tb> danhMucs = db.DanhMuc_tbs.Where(p => danhMucIds.Contains(p.id)).ToList();
        for (int i = 0; i < danhMucs.Count; i++)
        {
            string key = danhMucs[i].id.ToString();
            string name = (danhMucs[i].name ?? string.Empty).Trim();
            if (!categoryMap.ContainsKey(key))
                categoryMap[key] = name;
        }

        List<string> missingIds = categoryIds.Where(p => !categoryMap.ContainsKey(p)).ToList();
        if (missingIds.Count > 0)
        {
            List<web_menu_table> menus = db.web_menu_tables
                .Where(p => missingIds.Contains(p.id.ToString()) && (p.bin == false || p.bin == null))
                .ToList();

            for (int i = 0; i < menus.Count; i++)
            {
                string key = menus[i].id.ToString();
                if (!categoryMap.ContainsKey(key))
                    categoryMap[key] = (menus[i].name ?? string.Empty).Trim();
            }
        }

        return categoryMap;
    }
}
