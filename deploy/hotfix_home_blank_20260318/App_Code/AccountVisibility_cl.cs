using System;
using System.Collections.Generic;
using System.Linq;

public static class AccountVisibility_cl
{
    public const string PostTypeProduct = "sanpham";
    public const string PostTypeService = "dichvu";
    public const string ShopPartnerType = "Gian hàng đối tác";
    private const string ShopScopeToken = PortalScope_cl.ScopeShop;
    private static bool _tradeTypeNormalized;
    private static readonly object _tradeTypeLock = new object();


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

        string tk = (taiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
            return false;

        ShopStatus_cl.EnsureSchemaSafe(db);
        EnsureTradeTypeNormalized(db);
        bool hasTrangThai = ShopStatus_cl.HasTrangThaiColumn(db);
        if (!hasTrangThai)
        {
            return db.taikhoan_tbs.Any(p =>
                p.taikhoan == tk
                && p.block != true);
        }

        return db.taikhoan_tbs.Any(p =>
            p.taikhoan == tk
            && (((p.phanloai == ShopPartnerType
                  || ((p.permission ?? "").ToLower()).Contains(ShopScopeToken))
                 && p.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                || (!(p.phanloai == ShopPartnerType
                      || ((p.permission ?? "").ToLower()).Contains(ShopScopeToken))
                    && p.block != true)));
    }

    public static IQueryable<BaiViet_tb> FilterVisibleProducts(dbDataContext db, IQueryable<BaiViet_tb> source)
    {
        if (db == null || source == null)
            return source;

        ShopStatus_cl.EnsureSchemaSafe(db);
        EnsureTradeTypeNormalized(db);
        bool hasTrangThai = ShopStatus_cl.HasTrangThaiColumn(db);
        if (!hasTrangThai)
        {
            return source.Where(p =>
                p.phanloai == PostTypeProduct
                && (p.bin == false || p.bin == null)
                && db.taikhoan_tbs.Any(acc =>
                    acc.taikhoan == p.nguoitao
                    && acc.block != true));
        }

        return source.Where(p =>
            p.phanloai == PostTypeProduct
            && (p.bin == false || p.bin == null)
            && db.taikhoan_tbs.Any(acc =>
                acc.taikhoan == p.nguoitao
                && (((acc.phanloai == ShopPartnerType
                      || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                     && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                    || (!(acc.phanloai == ShopPartnerType
                          || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                        && acc.block != true))));
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

        ShopStatus_cl.EnsureSchemaSafe(db);
        EnsureTradeTypeNormalized(db);
        bool hasTrangThai = ShopStatus_cl.HasTrangThaiColumn(db);
        if (!hasTrangThai)
        {
            return source.Where(p =>
                (p.phanloai == PostTypeProduct || p.phanloai == PostTypeService)
                && (p.bin == false || p.bin == null)
                && db.taikhoan_tbs.Any(acc =>
                    acc.taikhoan == p.nguoitao
                    && acc.block != true));
        }

        return source.Where(p =>
            (p.phanloai == PostTypeProduct || p.phanloai == PostTypeService)
            && (p.bin == false || p.bin == null)
            && db.taikhoan_tbs.Any(acc =>
                acc.taikhoan == p.nguoitao
                && (((acc.phanloai == ShopPartnerType
                      || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                     && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                    || (!(acc.phanloai == ShopPartnerType
                          || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                        && acc.block != true))));
    }

    public static BaiViet_tb FindVisibleTradePostById(dbDataContext db, string idsp)
    {
        if (db == null)
            return null;

        string id = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(id))
            return null;

        ShopStatus_cl.EnsureSchemaSafe(db);
        EnsureTradeTypeNormalized(db);
        bool hasTrangThai = ShopStatus_cl.HasTrangThaiColumn(db);
        if (!hasTrangThai)
        {
            return db.BaiViet_tbs.FirstOrDefault(p =>
                p.id.ToString() == id
                && (p.phanloai == PostTypeProduct || p.phanloai == PostTypeService)
                && (p.bin == false || p.bin == null)
                && db.taikhoan_tbs.Any(acc =>
                    acc.taikhoan == p.nguoitao
                    && acc.block != true));
        }

        return db.BaiViet_tbs.FirstOrDefault(p =>
            p.id.ToString() == id
            && (p.phanloai == PostTypeProduct || p.phanloai == PostTypeService)
            && (p.bin == false || p.bin == null)
            && db.taikhoan_tbs.Any(acc =>
                acc.taikhoan == p.nguoitao
                && (((acc.phanloai == ShopPartnerType
                      || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                     && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                    || (!(acc.phanloai == ShopPartnerType
                          || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                        && acc.block != true))));
    }

    public static BaiViet_tb FindVisibleProductById(dbDataContext db, string idsp)
    {
        if (db == null)
            return null;

        string id = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(id))
            return null;

        ShopStatus_cl.EnsureSchemaSafe(db);
        EnsureTradeTypeNormalized(db);
        bool hasTrangThai = ShopStatus_cl.HasTrangThaiColumn(db);
        if (!hasTrangThai)
        {
            return db.BaiViet_tbs.FirstOrDefault(p =>
                p.id.ToString() == id
                && p.phanloai == PostTypeProduct
                && (p.bin == false || p.bin == null)
                && db.taikhoan_tbs.Any(acc =>
                    acc.taikhoan == p.nguoitao
                    && acc.block != true));
        }

        return db.BaiViet_tbs.FirstOrDefault(p =>
            p.id.ToString() == id
            && p.phanloai == PostTypeProduct
            && (p.bin == false || p.bin == null)
            && db.taikhoan_tbs.Any(acc =>
                acc.taikhoan == p.nguoitao
                && (((acc.phanloai == ShopPartnerType
                      || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                     && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                    || (!(acc.phanloai == ShopPartnerType
                          || ((acc.permission ?? "").ToLower()).Contains(ShopScopeToken))
                        && acc.block != true))));
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
}
