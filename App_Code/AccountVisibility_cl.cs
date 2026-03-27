using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class AccountVisibility_cl
{
    public const string PostTypeProduct = "sanpham";
    public const string PostTypeService = "dichvu";
    public const string ShopPartnerType = "Gian hàng đối tác";
    private const string ShopScopeToken = PortalScope_cl.ScopeShop;
    private const string VisibleSellerCacheKey = "__ahasale_visible_seller_cache";
    private static bool _tradeTypeNormalized;
    private static readonly object _tradeTypeLock = new object();

    private sealed class VisibleSellerCache
    {
        public List<string> ExactKeys { get; set; }
        public HashSet<string> NormalizedKeys { get; set; }
    }

    public static bool IsBlocked(taikhoan_tb account)
    {
        return account != null && account.block == true;
    }

    public static bool CanToggleHomeLock(taikhoan_tb account)
    {
        if (account == null)
            return false;
        if (PermissionProfile_cl.IsRootAdmin(account.taikhoan))
            return false;
        return PortalScope_cl.CanLoginHome(account.taikhoan, account.phanloai, account.permission);
    }

    public static bool CanLoginHomeByPhone(taikhoan_tb account)
    {
        if (account == null)
            return false;
        if (!PortalScope_cl.CanLoginHome(account.taikhoan, account.phanloai, account.permission))
            return false;

        bool byPhoneColumn = AccountAuth_cl.IsValidPhone(account.dienthoai);
        bool byLegacyUsername = AccountAuth_cl.IsValidPhone(account.taikhoan);
        return byPhoneColumn || byLegacyUsername;
    }

    public static bool IsSellerVisible(dbDataContext db, string taiKhoan)
    {
        if (db == null)
            return false;

        string tk = NormalizeAccountKey(taiKhoan);
        if (string.IsNullOrEmpty(tk))
            return false;

        VisibleSellerCache cache = GetVisibleSellerCache(db);
        return cache.NormalizedKeys.Contains(tk);
    }

    public static IQueryable<BaiViet_tb> FilterVisibleProducts(dbDataContext db, IQueryable<BaiViet_tb> source)
    {
        if (db == null || source == null)
            return source;

        List<string> allowedKeys = GetVisibleSellerCache(db).ExactKeys;
        if (allowedKeys.Count == 0)
            return source.Where(p => false);

        return source.Where(p =>
            p.phanloai == PostTypeProduct
            && (p.bin == false || p.bin == null)
            && allowedKeys.Contains(p.nguoitao));
    }

    public static bool IsVisibleTradeType(string phanLoai)
    {
        string type = NormalizeTradeType(phanLoai);
        return type == PostTypeProduct || type == PostTypeService;
    }

    public static bool IsProductPost(BaiViet_tb post)
    {
        return post != null && string.Equals(NormalizeTradeType(post.phanloai), PostTypeProduct, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsServicePost(BaiViet_tb post)
    {
        return post != null && string.Equals(NormalizeTradeType(post.phanloai), PostTypeService, StringComparison.OrdinalIgnoreCase);
    }

    public static IQueryable<BaiViet_tb> FilterVisibleTradePosts(dbDataContext db, IQueryable<BaiViet_tb> source)
    {
        if (db == null || source == null)
            return source;

        List<string> allowedKeys = GetVisibleSellerCache(db).ExactKeys;
        if (allowedKeys.Count == 0)
            return source.Where(p => false);

        return source.Where(p =>
            (p.phanloai == PostTypeProduct || p.phanloai == PostTypeService)
            && (p.bin == false || p.bin == null)
            && allowedKeys.Contains(p.nguoitao));
    }

    public static BaiViet_tb FindVisibleTradePostById(dbDataContext db, string idsp)
    {
        if (db == null)
            return null;

        string id = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(id))
            return null;

        List<string> allowedKeys = GetVisibleSellerCache(db).ExactKeys;
        if (allowedKeys.Count == 0)
            return null;

        return db.BaiViet_tbs.FirstOrDefault(p =>
            p.id.ToString() == id
            && (p.phanloai == PostTypeProduct || p.phanloai == PostTypeService)
            && (p.bin == false || p.bin == null)
            && allowedKeys.Contains(p.nguoitao));
    }

    public static BaiViet_tb FindVisibleProductById(dbDataContext db, string idsp)
    {
        if (db == null)
            return null;

        string id = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(id))
            return null;

        List<string> allowedKeys = GetVisibleSellerCache(db).ExactKeys;
        if (allowedKeys.Count == 0)
            return null;

        return db.BaiViet_tbs.FirstOrDefault(p =>
            p.id.ToString() == id
            && p.phanloai == PostTypeProduct
            && (p.bin == false || p.bin == null)
            && allowedKeys.Contains(p.nguoitao));
    }

    public static int LockLegacyHomeAccountsWithoutPhone(dbDataContext db, out int hiddenPostCount)
    {
        hiddenPostCount = 0;
        if (db == null)
            return 0;

        List<taikhoan_tb> allAccounts = db.taikhoan_tbs.ToList();
        List<string> targets = allAccounts
            .Where(CanToggleHomeLock)
            .Where(p => p.block != true)
            .Where(p => !CanLoginHomeByPhone(p))
            .Select(p => (p.taikhoan ?? "").Trim().ToLowerInvariant())
            .Where(p => p != "")
            .Distinct()
            .ToList();

        if (targets.Count == 0)
            return 0;

        hiddenPostCount = db.BaiViet_tbs.Count(p =>
            (p.phanloai == PostTypeProduct || p.phanloai == PostTypeService)
            && p.bin == false
            && targets.Contains((p.nguoitao ?? "").Trim().ToLower()));

        foreach (taikhoan_tb acc in allAccounts)
        {
            string tk = (acc.taikhoan ?? "").Trim().ToLowerInvariant();
            if (targets.Contains(tk))
                acc.block = true;
        }

        db.SubmitChanges();
        return targets.Count;
    }

    public static string NormalizeTradeType(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(value))
            return "";

        if (CompanyShop_cl.IsInternalProductType(value))
            return CompanyShop_cl.ProductTypeInternal;

        string compact = value.Replace(" ", "").Replace("_", "").Replace("-", "");
        if (compact == "dichvu" || compact == "dịchvụ" || value == "service")
            return PostTypeService;
        if (compact == "sanpham" || compact == "sảnphẩm" || value == "product")
            return PostTypeProduct;

        return value;
    }

    public static void EnsureTradeTypeNormalized(dbDataContext db)
    {
        if (db == null || _tradeTypeNormalized)
            return;

        lock (_tradeTypeLock)
        {
            if (_tradeTypeNormalized)
                return;

            try
            {
                db.ExecuteCommand(@"
UPDATE dbo.BaiViet_tb
SET phanloai = 'dichvu'
WHERE phanloai IS NOT NULL
  AND REPLACE(REPLACE(REPLACE(LOWER(LTRIM(RTRIM(phanloai))), ' ', ''), '_', ''), '-', '') IN ('dichvu', N'dịchvụ', 'service');");

                db.ExecuteCommand(@"
UPDATE dbo.BaiViet_tb
SET phanloai = 'sanpham'
WHERE phanloai IS NOT NULL
  AND REPLACE(REPLACE(REPLACE(LOWER(LTRIM(RTRIM(phanloai))), ' ', ''), '_', ''), '-', '') IN ('sanpham', N'sảnphẩm', 'product');");

                _tradeTypeNormalized = true;
            }
            catch
            {
                // nếu lỗi schema thì tránh retry liên tục gây chậm
                _tradeTypeNormalized = true;
            }
        }
    }

    private static VisibleSellerCache GetVisibleSellerCache(dbDataContext db)
    {
        HttpContext context = HttpContext.Current;
        if (context != null)
        {
            VisibleSellerCache cached = context.Items[VisibleSellerCacheKey] as VisibleSellerCache;
            if (cached != null)
                return cached;
        }

        ShopStatus_cl.EnsureSchemaSafe(db);
        EnsureTradeTypeNormalized(db);
        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        bool hasTrangThai = ShopStatus_cl.HasTrangThaiColumn(db);
        HashSet<string> gianHangActiveKeys = GetActiveGianHangKeys(db);
        HashSet<string> shopActiveKeys = GetActiveShopKeys(db);
        List<string> exactKeys = new List<string>();
        HashSet<string> normalizedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (taikhoan_tb account in db.taikhoan_tbs.ToList())
        {
            if (!IsAccountVisibleForTradeFeed(account, hasTrangThai, gianHangActiveKeys, shopActiveKeys))
                continue;

            string exactKey = (account.taikhoan ?? "").Trim();
            string normalizedKey = NormalizeAccountKey(account.taikhoan);
            if (exactKey == "" || normalizedKey == "")
                continue;

            if (!normalizedKeys.Contains(normalizedKey))
            {
                normalizedKeys.Add(normalizedKey);
                exactKeys.Add(exactKey);
            }
        }

        VisibleSellerCache cache = new VisibleSellerCache
        {
            ExactKeys = exactKeys,
            NormalizedKeys = normalizedKeys
        };

        if (context != null)
            context.Items[VisibleSellerCacheKey] = cache;

        return cache;
    }

    private static HashSet<string> GetActiveGianHangKeys(dbDataContext db)
    {
        List<string> keys = db.ExecuteQuery<string>(
                "SELECT AccountKey FROM dbo.CoreSpaceAccess_tb WHERE SpaceCode = {0} AND AccessStatus = {1}",
                ModuleSpace_cl.GianHang,
                SpaceAccess_cl.StatusActive)
            .Select(NormalizeAccountKey)
            .Where(p => p != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new HashSet<string>(keys, StringComparer.OrdinalIgnoreCase);
    }

    private static HashSet<string> GetActiveShopKeys(dbDataContext db)
    {
        List<string> keys = db.ExecuteQuery<string>(
                "SELECT AccountKey FROM dbo.CoreSpaceAccess_tb WHERE SpaceCode = {0} AND AccessStatus = {1}",
                ModuleSpace_cl.Shop,
                SpaceAccess_cl.StatusActive)
            .Select(NormalizeAccountKey)
            .Where(p => p != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new HashSet<string>(keys, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsAccountVisibleForTradeFeed(taikhoan_tb account, bool hasTrangThai, HashSet<string> gianHangActiveKeys, HashSet<string> shopActiveKeys)
    {
        if (account == null)
            return false;

        string accountKey = NormalizeAccountKey(account.taikhoan);
        if (accountKey == "")
            return false;

        bool isShop = IsShopAccount(account, accountKey, shopActiveKeys);
        if (isShop)
        {
            if (shopActiveKeys != null && shopActiveKeys.Contains(accountKey))
                return true;

            if (!hasTrangThai)
                return account.block != true;

            return account.TrangThai_Shop == ShopStatus_cl.StatusApproved;
        }

        if (account.block == true)
            return false;

        if (!PortalScope_cl.CanLoginHome(account.taikhoan, account.phanloai, account.permission))
            return true;

        return gianHangActiveKeys.Contains(accountKey);
    }

    private static bool IsShopAccount(taikhoan_tb account, string normalizedAccountKey, HashSet<string> shopActiveKeys)
    {
        if (account == null)
            return false;

        if (!string.IsNullOrEmpty(normalizedAccountKey) && shopActiveKeys != null && shopActiveKeys.Contains(normalizedAccountKey))
            return true;

        if (account.TrangThai_Shop == ShopStatus_cl.StatusApproved)
            return true;

        return string.Equals((account.phanloai ?? "").Trim(), ShopPartnerType, StringComparison.OrdinalIgnoreCase)
               || ((account.permission ?? "").ToLowerInvariant()).Contains(ShopScopeToken);
    }

    private static string NormalizeAccountKey(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
