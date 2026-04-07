using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public static class GianHangStorefront_cl
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    public const string MenuTypeArticle = "dsbv";
    public const string MenuTypeProduct = "dssp";
    public const string MenuTypeService = "dsdv";

    public sealed class StorefrontSummary
    {
        public string StorefrontName { get; set; }
        public string AccountKey { get; set; }
        public string AvatarUrl { get; set; }
        public string ProfileUrl { get; set; }
        public string ProfileAbsoluteUrl { get; set; }
        public int ProductCount { get; set; }
        public int ServiceCount { get; set; }
        public int PendingOrderCount { get; set; }
    }

    public sealed class ProductCardView
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        public string Name { get; set; }
        public string SlugRaw { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Views { get; set; }
        public string PostType { get; set; }
        public int SoldCount { get; set; }

        public int HomePostId { get { return ReferenceId; } set { ReferenceId = value; } }
    }

    private sealed class ProductCardRaw
    {
        public int Id { get; set; }
        public int? ReferenceId { get; set; }
        public string Name { get; set; }
        public string SlugRaw { get; set; }
        public string ImageUrlRaw { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Views { get; set; }
        public string PostTypeRaw { get; set; }
        public int SoldCount { get; set; }
    }

    public static StorefrontSummary BuildSummary(dbDataContext db, taikhoan_tb account, Uri requestUrl)
    {
        if (db == null || account == null)
            return new StorefrontSummary();

        string accountKey = (account.taikhoan ?? "").Trim().ToLowerInvariant();
        IQueryable<GH_SanPham_tb> source = BuildProductSource(db, accountKey);
        int productCount = source.Count(p => p.loai == GianHangProduct_cl.LoaiSanPham);
        int serviceCount = source.Count(p => p.loai == GianHangProduct_cl.LoaiDichVu);

        return new StorefrontSummary
        {
            StorefrontName = ResolveStorefrontName(account),
            AccountKey = accountKey,
            AvatarUrl = ResolveAvatarUrl(account),
            ProfileUrl = ResolveProfileUrl(account),
            ProfileAbsoluteUrl = BuildAbsoluteUrl(requestUrl, ResolveProfileUrl(account)),
            ProductCount = productCount,
            ServiceCount = serviceCount,
            PendingOrderCount = GetPendingOrdersCompat(db, accountKey)
        };
    }

    public static List<ProductCardView> LoadProducts(dbDataContext db, string accountKey)
    {
        string key = (accountKey ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(key))
            return new List<ProductCardView>();

        List<ProductCardRaw> rawList = BuildProductSource(db, key)
            .OrderByDescending(p => p.ngay_tao)
            .Select(p => new ProductCardRaw
            {
                Id = p.id,
                ReferenceId = p.id_baiviet ?? p.id,
                Name = p.ten,
                SlugRaw = p.ten,
                ImageUrlRaw = p.hinh_anh,
                Price = p.gia_ban,
                CreatedAt = p.ngay_tao,
                Views = p.luot_truy_cap ?? 0,
                PostTypeRaw = p.loai,
                SoldCount = p.so_luong_da_ban ?? 0
            })
            .ToList();

        List<ProductCardView> list = new List<ProductCardView>(rawList.Count);
        for (int i = 0; i < rawList.Count; i++)
        {
            ProductCardRaw item = rawList[i];
            string loai = GianHangProduct_cl.NormalizeLoai(item.PostTypeRaw);
            list.Add(new ProductCardView
            {
                Id = item.Id,
                ReferenceId = item.ReferenceId ?? item.Id,
                Name = item.Name,
                SlugRaw = item.SlugRaw,
                ImageUrl = ResolveImageUrl(item.ImageUrlRaw),
                Price = item.Price,
                CreatedAt = item.CreatedAt,
                Views = item.Views,
                PostType = loai == GianHangProduct_cl.LoaiDichVu
                    ? AccountVisibility_cl.PostTypeService
                    : AccountVisibility_cl.PostTypeProduct,
                SoldCount = item.SoldCount
            });
        }

        ApplySoldCount(db, key, list);
        return list;
    }

    public static List<ProductCardView> LoadTopProducts(dbDataContext db, string accountKey, IList<ProductCardView> products)
    {
        if (products == null || products.Count == 0)
            return new List<ProductCardView>();

        List<ProductCardView> ordered = products
            .OrderByDescending(p => p.SoldCount)
            .ThenByDescending(p => p.Views)
            .Take(5)
            .ToList();

        if (ordered.All(p => p.SoldCount == 0))
        {
            ordered = products
                .OrderByDescending(p => p.Views)
                .Take(5)
                .ToList();
        }

        return ordered;
    }

    public static string ResolveStorefrontName(taikhoan_tb account)
    {
        if (account == null)
            return "Gian hàng";

        if (!string.IsNullOrWhiteSpace(account.ten_shop))
            return account.ten_shop.Trim();
        if (!string.IsNullOrWhiteSpace(account.hoten))
            return account.hoten.Trim();
        return (account.taikhoan ?? "Gian hàng").Trim();
    }

    public static string ResolveAvatarUrl(taikhoan_tb account)
    {
        string avatar = account == null ? "" : (account.logo_shop ?? "").Trim();
        if (string.IsNullOrWhiteSpace(avatar))
            avatar = account == null ? "" : (account.anhdaidien ?? "").Trim();
        if (string.IsNullOrWhiteSpace(avatar))
            return "/uploads/images/macdinh.jpg";
        return ResolveImageUrl(avatar);
    }

    public static string ResolveProfileUrl(taikhoan_tb account)
    {
        string accountKey = account == null ? "" : (account.taikhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(accountKey))
            return GianHangRoutes_cl.BuildPublicUrl(string.Empty);
        return GianHangRoutes_cl.BuildPublicUrl(accountKey);
    }

    public static string ResolveImageUrl(string imageRaw)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        string image = (imageRaw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(image))
            return fallback;
        if (image.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(image, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        if (image.StartsWith("~/", StringComparison.Ordinal))
            image = image.Substring(1);
        if (!image.StartsWith("/", StringComparison.Ordinal))
            image = "/" + image;

        return image;
    }

    public static string BuildDetailUrl(ProductCardView item)
    {
        if (item == null)
            return GianHangRoutes_cl.BuildDashboardUrl();

        if (IsService(item))
            return GianHangRoutes_cl.BuildChiTietDichVuUrl(item.Id);

        return GianHangRoutes_cl.BuildChiTietSanPhamUrl(item.Id);
    }

    public static string BuildActionUrl(ProductCardView item, string accountKey)
    {
        if (item == null)
            return GianHangRoutes_cl.BuildDashboardUrl();

        return GianHangRoutes_cl.BuildTaoDonUrl(item.Id, GianHangRoutes_cl.BuildDashboardUrl());
    }

    public static string BuildActionText(ProductCardView item)
    {
        return "Tạo đơn";
    }

    public static bool IsService(ProductCardView item)
    {
        string postType = item == null ? "" : (item.PostType ?? "").Trim();
        return string.Equals(postType, AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase);
    }

    public static string BuildPostTypeLabel(ProductCardView item)
    {
        return IsService(item) ? "Dịch vụ" : "Sản phẩm";
    }

    public static string BuildPostTypeCss(ProductCardView item)
    {
        return IsService(item) ? "gianhang-product-badge gianhang-product-badge-service" : "gianhang-product-badge gianhang-product-badge-product";
    }

    public static string FormatCurrency(decimal? value)
    {
        return (value ?? 0m).ToString("#,##0.##", ViCulture);
    }

    public static string ResolveDefaultMenuId(dbDataContext db, string chiNhanhId, string menuType)
    {
        string normalizedType = (menuType ?? string.Empty).Trim().ToLowerInvariant();
        if (normalizedType == string.Empty)
            return string.Empty;

        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty)
            return string.Empty;

        web_menu_table menu = db.web_menu_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId
                        && (p.bin == false || p.bin == null)
                        && p.phanloai == normalizedType)
            .OrderBy(p => p.rank ?? 9999)
            .ThenBy(p => p.id)
            .FirstOrDefault();
        if (menu != null)
            return menu.id.ToString();

        menu = db.web_menu_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId
                        && (p.bin == false || p.bin == null))
            .ToList()
            .FirstOrDefault(p => string.Equals((p.phanloai ?? string.Empty).Trim(), normalizedType, StringComparison.OrdinalIgnoreCase));

        return menu == null ? string.Empty : menu.id.ToString();
    }

    public static string ResolveDefaultMenuId(string menuType)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string chiNhanhId = GianHangPublic_cl.ResolveCurrentChiNhanhId(db, HttpContext.Current == null ? null : HttpContext.Current.Request);
            return ResolveDefaultMenuId(db, chiNhanhId, menuType);
        }
    }

    public static string ResolveMenuUrl(object idObject, object phanloaiObject, object urlOtherObject)
    {
        string menuId = (idObject ?? string.Empty).ToString().Trim();
        string menuType = (phanloaiObject ?? string.Empty).ToString().Trim().ToLowerInvariant();
        string externalUrl = (urlOtherObject ?? string.Empty).ToString().Trim();

        switch (menuType)
        {
            case MenuTypeArticle:
                return GianHangRoutes_cl.BuildArticlesUrl(string.Empty) + "?idmn=" + HttpUtility.UrlEncode(menuId);
            case MenuTypeProduct:
                return GianHangRoutes_cl.BuildProductsUrl(string.Empty) + "?idmn=" + HttpUtility.UrlEncode(menuId);
            case MenuTypeService:
                return GianHangRoutes_cl.BuildServicesUrl(string.Empty) + "?idmn=" + HttpUtility.UrlEncode(menuId);
        }

        if (!string.IsNullOrWhiteSpace(externalUrl))
            return NormalizeStandaloneUrl(externalUrl);

        return GianHangRoutes_cl.BuildStorefrontUrl(string.Empty);
    }

    public static string NormalizeStandaloneUrl(string rawUrl)
    {
        string value = (rawUrl ?? string.Empty).Trim();
        if (value == string.Empty)
            return GianHangRoutes_cl.BuildStorefrontUrl(string.Empty);

        value = GianHangStorefrontConfig_cl.NormalizeStorefrontUrl(value, string.Empty);

        if (value.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("#", StringComparison.Ordinal))
            return value;

        Uri absoluteUrl;
        if (Uri.TryCreate(value, UriKind.Absolute, out absoluteUrl))
            return absoluteUrl.AbsoluteUri;

        if (value.StartsWith("~/", StringComparison.Ordinal))
            value = value.Substring(1);

        if (!value.StartsWith("/", StringComparison.Ordinal))
            value = "/" + value;

        if (value.StartsWith("/gianhang/", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/home/", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return value;

        if (value.StartsWith("/page/", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/tai-khoan/", StringComparison.OrdinalIgnoreCase)
            || value.Equals("/datlich.aspx", StringComparison.OrdinalIgnoreCase)
            || value.Equals("/giohang.aspx", StringComparison.OrdinalIgnoreCase)
            || value.Equals("/xoa_chitiet_giohang.aspx", StringComparison.OrdinalIgnoreCase))
            return "/gianhang" + value;

        return value;
    }

    private static IQueryable<GH_SanPham_tb> BuildProductSource(dbDataContext db, string accountKey)
    {
        return GianHangProduct_cl.QueryPublicByStorefront(db, accountKey);
    }

    private static void ApplySoldCount(dbDataContext db, string accountKey, IList<ProductCardView> products)
    {
        if (db == null || products == null || products.Count == 0)
            return;

        GianHangInvoice_cl.EnsureSchema(db);
        List<int> nativeProductIds = products
            .Select(p => p.Id)
            .ToList()
            .Distinct()
            .ToList();
        Dictionary<int, string> productReferenceMap = new Dictionary<int, string>();
        for (int i = 0; i < products.Count; i++)
        {
            ProductCardView item = products[i];
            if (item == null || productReferenceMap.ContainsKey(item.Id))
                continue;

            productReferenceMap[item.Id] = item.ReferenceId > 0 && item.ReferenceId != item.Id
                ? item.ReferenceId.ToString()
                : string.Empty;
        }

        Dictionary<int, int> nativeSoldMap = GianHangInvoice_cl.LoadSoldTotalsByProductMap(db, accountKey, productReferenceMap, 1000);

        for (int i = 0; i < products.Count; i++)
        {
            ProductCardView item = products[i];
            int sold = 0;
            int nativeSold;
            if (nativeSoldMap.TryGetValue(item.Id, out nativeSold))
                sold += nativeSold;

            item.SoldCount = sold;
        }
    }

    private static int GetPendingOrdersCompat(dbDataContext db, string accountKey)
    {
        return GianHangInvoice_cl.CountWaitingExchangeByStorefront(db, accountKey, 500, 500);
    }

    private static string BuildAbsoluteUrl(Uri requestUrl, string relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
            return "";
        if (requestUrl == null)
            return relativeUrl;

        return requestUrl.GetLeftPart(UriPartial.Authority) + relativeUrl;
    }
}
