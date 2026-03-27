using System;
using System.Collections.Generic;
using System.Linq;

public static class CompanyShopBootstrap_cl
{
    private const string MapTableName = "CoreCompanyShopProductMap_tb";
    private const string SystemProductTableName = "SanPham_Aha_tb";
    private const string ShopPostTableName = "BaiViet_tb";
    private const string CategoryTableName = "DanhMuc_tb";
    private const string PolicyTableName = "CoreShopPolicy_tb";
    private const string SyncSource = "company_shop_bootstrap";
    private static readonly object SyncLock = new object();
    private static readonly object WarmupLock = new object();
    private static DateTime _lastSyncAtUtc = DateTime.MinValue;
    private static DateTime _lastWarmupAttemptUtc = DateTime.MinValue;
    private static readonly TimeSpan SyncInterval = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan WarmupRetryInterval = TimeSpan.FromSeconds(20);
    private static bool _runtimeWarmedUp;

    private sealed class CompanyProductMapRow
    {
        public int SystemProductId { get; set; }
        public int ShopPostId { get; set; }
    }

    public sealed class CompanyShopSyncReport
    {
        public string AccountKey { get; set; }
        public bool AccountCreated { get; set; }
        public int SystemProductCount { get; set; }
        public int InternalProductCount { get; set; }
        public int MapCount { get; set; }
        public int SpecialProductHandlerCount { get; set; }
        public bool HasHomeAccess { get; set; }
        public bool HasShopAccess { get; set; }
        public bool HasActivePolicy { get; set; }
        public int PolicyPercent { get; set; }
    }

    public static taikhoan_tb EnsureSpecialShopReady(dbDataContext db)
    {
        bool created;
        return EnsureSpecialShopReady(db, out created);
    }

    public static void EnsureRuntimeWarmup()
    {
        if (_runtimeWarmedUp)
            return;

        if (_lastWarmupAttemptUtc != DateTime.MinValue
            && (DateTime.UtcNow - _lastWarmupAttemptUtc) < WarmupRetryInterval)
            return;

        lock (WarmupLock)
        {
            if (_runtimeWarmedUp)
                return;

            if (_lastWarmupAttemptUtc != DateTime.MinValue
                && (DateTime.UtcNow - _lastWarmupAttemptUtc) < WarmupRetryInterval)
                return;

            _lastWarmupAttemptUtc = DateTime.UtcNow;
            try
            {
                using (dbDataContext db = new dbDataContext())
                {
                    EnsureSystemCatalogMirrored(db);

                    int activePercent;
                    bool hasPolicy = ShopPolicy_cl.TryGetActivePolicyPercent(db, CompanyShop_cl.GetSpecialAccount(), out activePercent);
                    bool hasInternalProducts = !HasTable(db, SystemProductTableName)
                        || db.BaiViet_tbs.Any(p => p.nguoitao == CompanyShop_cl.GetSpecialAccount() && p.phanloai == CompanyShop_cl.ProductTypeInternal);

                    _runtimeWarmedUp = hasPolicy && hasInternalProducts;
                }
            }
            catch
            {
                _runtimeWarmedUp = false;
            }
        }
    }

    public static taikhoan_tb EnsureSpecialShopReady(dbDataContext db, out bool created)
    {
        created = false;
        if (db == null)
            return null;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        ShopStatus_cl.EnsureSchemaSafe(db);

        string accountKey = CompanyShop_cl.GetSpecialAccount();
        if (string.IsNullOrWhiteSpace(accountKey))
            return null;

        accountKey = accountKey.Trim().ToLowerInvariant();
        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == accountKey);

        if (account == null)
        {
            account = new taikhoan_tb();
            account.taikhoan = accountKey;
            account.matkhau = CompanyShop_cl.GetSpecialPassword();
            account.hoten = "AhaSale Company";
            account.ten = "AHA SALE";
            account.hoten_khongdau = "aha-sale-company";
            account.phanloai = AccountVisibility_cl.ShopPartnerType;
            account.permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeShop);
            account.block = false;
            account.nguoitao = "system";
            account.ngaytao = AhaTime_cl.Now;
            account.hansudung = AhaTime_cl.Now.AddYears(50);
            account.DongA = 0m;
            account.ten_shop = "AhaSale Company";
            account.motangan_shop = "Gian hàng công ty AhaSale cho các sản phẩm hệ thống.";
            account.anhdaidien = "/uploads/images/macdinh.jpg";
            account.logo_shop = "/uploads/images/macdinh.jpg";
            account.TrangThai_Shop = ShopStatus_cl.StatusApproved;
            db.taikhoan_tbs.InsertOnSubmit(account);
            db.SubmitChanges();
            created = true;
        }

        bool changed = false;
        if (string.IsNullOrWhiteSpace(account.matkhau))
        {
            account.matkhau = CompanyShop_cl.GetSpecialPassword();
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.hoten))
        {
            account.hoten = "AhaSale Company";
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.ten))
        {
            account.ten = "AHA SALE";
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.hoten_khongdau))
        {
            account.hoten_khongdau = "aha-sale-company";
            changed = true;
        }
        if (!string.Equals((account.phanloai ?? "").Trim(), AccountVisibility_cl.ShopPartnerType, StringComparison.OrdinalIgnoreCase))
        {
            account.phanloai = AccountVisibility_cl.ShopPartnerType;
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.permission) || !PortalScope_cl.CanLoginShop(account.taikhoan, account.phanloai, account.permission))
        {
            account.permission = PortalScope_cl.NormalizePermissionWithScope(account.permission, PortalScope_cl.ScopeShop);
            changed = true;
        }
        if (account.block == true)
        {
            account.block = false;
            changed = true;
        }
        if (!account.hansudung.HasValue || account.hansudung.Value < AhaTime_cl.Now.AddYears(5))
        {
            account.hansudung = AhaTime_cl.Now.AddYears(50);
            changed = true;
        }
        if (account.TrangThai_Shop != ShopStatus_cl.StatusApproved)
        {
            account.TrangThai_Shop = ShopStatus_cl.StatusApproved;
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.ten_shop))
        {
            account.ten_shop = string.IsNullOrWhiteSpace(account.hoten) ? accountKey : account.hoten.Trim();
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.motangan_shop))
        {
            account.motangan_shop = "Gian hàng công ty AhaSale cho các sản phẩm hệ thống.";
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.email))
        {
            account.email = accountKey + "@ahasale.vn";
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.email_shop))
        {
            account.email_shop = account.email;
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.sdt_shop) && !string.IsNullOrWhiteSpace(account.dienthoai))
        {
            account.sdt_shop = account.dienthoai;
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.anhdaidien))
        {
            account.anhdaidien = "/uploads/images/macdinh.jpg";
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(account.logo_shop))
        {
            account.logo_shop = account.anhdaidien;
            changed = true;
        }

        if (changed)
            db.SubmitChanges();

        SpaceAccess_cl.UpsertSpaceAccess(
            db,
            accountKey,
            ModuleSpace_cl.Home,
            SpaceAccess_cl.StatusActive,
            SyncSource,
            true,
            "system",
            AhaTime_cl.Now,
            "",
            null);

        SpaceAccess_cl.UpsertSpaceAccess(
            db,
            accountKey,
            ModuleSpace_cl.Shop,
            SpaceAccess_cl.StatusActive,
            SyncSource,
            false,
            "system",
            AhaTime_cl.Now,
            "",
            null);

        EnsureCompanyPolicy(db, accountKey);

        ShopSlug_cl.EnsureSlugForShop(db, account);
        return account;
    }

    public static void EnsureSystemCatalogMirrored(dbDataContext db)
    {
        if (db == null)
            return;

        if (!ShouldRunSync(false))
            return;

        bool created;
        taikhoan_tb account = EnsureSpecialShopReady(db, out created);
        if (account == null)
            return;

        if (!ShouldRunSync(created))
            return;

        lock (SyncLock)
        {
            if (!ShouldRunSync(created))
                return;

            try
            {
                RunManualSync(db, account, created);
                _lastSyncAtUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                string actor = account == null ? "system" : (account.taikhoan ?? "system");
                Log_cl.Add_Log("CompanyShop sync failed: " + ex.Message, actor, ex.StackTrace);
            }
        }
    }

    public static CompanyShopSyncReport RunManualSync(dbDataContext db)
    {
        bool created;
        taikhoan_tb account = EnsureSpecialShopReady(db, out created);
        return RunManualSync(db, account, created);
    }

    private static CompanyShopSyncReport RunManualSync(dbDataContext db, taikhoan_tb account, bool created)
    {
        if (db == null)
            throw new ArgumentNullException("db");
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
            throw new InvalidOperationException("Không khởi tạo được tài khoản shop công ty.");

        SyncSystemProductsToInternalPosts(db, account.taikhoan);

        int policyPercent = 0;
        bool hasPolicy = ShopPolicy_cl.TryGetActivePolicyPercent(db, account.taikhoan, out policyPercent);
        string normalizedAccount = (account.taikhoan ?? "").Trim().ToLowerInvariant();

        CompanyShopSyncReport report = new CompanyShopSyncReport();
        report.AccountKey = normalizedAccount;
        report.AccountCreated = created;
        report.SystemProductCount = HasTable(db, SystemProductTableName) ? db.SanPham_Aha_tbs.Count() : 0;
        report.InternalProductCount = db.BaiViet_tbs.Count(p => p.nguoitao == normalizedAccount && p.phanloai == CompanyShop_cl.ProductTypeInternal);
        report.MapCount = HasMapTable(db)
            ? db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM dbo." + MapTableName + " WHERE AccountKey = {0}",
                normalizedAccount).FirstOrDefault()
            : 0;
        report.SpecialProductHandlerCount = ShopSpecialProduct_cl.CountActiveHandlers(db, normalizedAccount);
        report.HasHomeAccess = SpaceAccess_cl.GetSpaceStatus(db, normalizedAccount, ModuleSpace_cl.Home) == SpaceAccess_cl.StatusActive;
        report.HasShopAccess = SpaceAccess_cl.GetSpaceStatus(db, normalizedAccount, ModuleSpace_cl.Shop) == SpaceAccess_cl.StatusActive;
        report.HasActivePolicy = hasPolicy;
        report.PolicyPercent = policyPercent;
        return report;
    }

    private static bool ShouldRunSync(bool force)
    {
        if (force)
            return true;
        if (_lastSyncAtUtc == DateTime.MinValue)
            return true;
        return (DateTime.UtcNow - _lastSyncAtUtc) >= SyncInterval;
    }

    private static void SyncSystemProductsToInternalPosts(dbDataContext db, string accountKey)
    {
        if (db == null)
            return;
        if (!HasTable(db, SystemProductTableName) || !HasTable(db, ShopPostTableName))
            return;

        string seller = (accountKey ?? "").Trim().ToLowerInvariant();
        if (seller == "")
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        List<SanPham_Aha_tb> systemProducts = db.SanPham_Aha_tbs
            .OrderBy(p => p.id)
            .ToList();
        if (systemProducts.Count == 0)
            return;

        int defaultCategoryId = 0;
        if (HasTable(db, CategoryTableName))
        {
            defaultCategoryId = db.DanhMuc_tbs
                .OrderBy(p => p.id)
                .Select(p => p.id)
                .FirstOrDefault();
        }
        string defaultCategory = defaultCategoryId > 0 ? defaultCategoryId.ToString() : "";

        List<CompanyProductMapRow> mapRows = new List<CompanyProductMapRow>();
        if (HasMapTable(db))
        {
            mapRows = db.ExecuteQuery<CompanyProductMapRow>(
                    "SELECT SystemProductId, ShopPostId FROM dbo." + MapTableName + " WHERE AccountKey = {0}",
                    seller)
                .ToList();
        }

        Dictionary<int, int> mapBySystemId = mapRows
            .Where(p => p != null)
            .GroupBy(p => p.SystemProductId)
            .ToDictionary(p => p.Key, p => p.First().ShopPostId);

        DateTime now = AhaTime_cl.Now;
        for (int i = 0; i < systemProducts.Count; i++)
        {
            SanPham_Aha_tb sys = systemProducts[i];
            if (sys == null)
                continue;

            int systemId = sys.id;
            int mappedPostId = mapBySystemId.ContainsKey(systemId) ? mapBySystemId[systemId] : 0;
            string marker = BuildMarker(systemId);

            BaiViet_tb post = null;
            if (mappedPostId > 0)
            {
                post = db.BaiViet_tbs.FirstOrDefault(p => p.id == mappedPostId && p.nguoitao == seller);
            }

            if (post == null)
            {
                post = db.BaiViet_tbs.FirstOrDefault(p =>
                    p.nguoitao == seller
                    && p.name_en == marker
                    && p.phanloai == CompanyShop_cl.ProductTypeInternal);
            }

            bool isNew = false;
            if (post == null)
            {
                post = new BaiViet_tb();
                isNew = true;
                db.BaiViet_tbs.InsertOnSubmit(post);
            }

            string productName = (sys.TenSanPham ?? "").Trim();
            if (productName == "")
                productName = "Sản phẩm hệ thống #" + systemId.ToString();

            post.name = productName;
            post.name_en = marker;
            if (string.IsNullOrWhiteSpace(post.id_DanhMuc))
                post.id_DanhMuc = defaultCategory;
            if (post.id_DanhMucCap2 == null)
                post.id_DanhMucCap2 = "";
            if (string.IsNullOrWhiteSpace(post.content_post))
                post.content_post = "Sản phẩm hệ thống do công ty AhaSale phát hành.";
            post.description = "Đồng bộ từ danh mục sản phẩm hệ thống (ID " + systemId.ToString() + ").";
            if (string.IsNullOrWhiteSpace(post.image))
                post.image = "/uploads/images/macdinh.jpg";
            post.bin = false;
            if (!post.ngaytao.HasValue)
                post.ngaytao = now;
            post.nguoitao = seller;
            if (!post.noibat.HasValue)
                post.noibat = false;
            post.giaban = Convert.ToDecimal(sys.GiaBan ?? 0L);
            if (!post.giavon.HasValue)
                post.giavon = 0L;
            if (!post.soluong_tonkho.HasValue)
                post.soluong_tonkho = 0;
            if (!post.soluong_daban.HasValue)
                post.soluong_daban = 0;
            if (!post.chotsale_thuong.HasValue)
                post.chotsale_thuong = 0;
            post.phanloai = CompanyShop_cl.ProductTypeInternal;
            if (!post.banhang_phantram_hoac_tien.HasValue)
                post.banhang_phantram_hoac_tien = true;
            if (!post.chotsale_phantram_hoac_tien.HasValue)
                post.chotsale_phantram_hoac_tien = true;
            if (!post.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
                post.PhanTram_GiamGia_ThanhToan_BangEvoucher = 0;

            int platformPercent = 0;
            if (sys.PhanTramChoSan.HasValue)
                platformPercent = CompanyShop_cl.ClampPlatformSharePercent((int)Math.Round(sys.PhanTramChoSan.Value, MidpointRounding.AwayFromZero));
            CompanyShop_cl.SetPlatformSharePercent(post, platformPercent);

            db.SubmitChanges();
            ShopToAhaShinePostSync_cl.SyncTradePost(db, post);

            if (HasMapTable(db))
            {
                UpsertMap(db, seller, systemId, post.id);
            }

            ShopSpecialProduct_cl.EnsureLegacyDefaultHandlerForMirroredProduct(
                db,
                seller,
                systemId,
                post.id,
                productName,
                "system");

            if (isNew)
            {
                mapBySystemId[systemId] = post.id;
            }
        }
    }

    private static void UpsertMap(dbDataContext db, string accountKey, int systemProductId, int shopPostId)
    {
        if (db == null || !HasMapTable(db))
            return;

        DateTime now = AhaTime_cl.Now;
        int updated = db.ExecuteCommand(
            "UPDATE dbo." + MapTableName + " SET ShopPostId = {2}, UpdatedAt = {3} WHERE AccountKey = {0} AND SystemProductId = {1}",
            accountKey,
            systemProductId,
            shopPostId,
            now);

        if (updated > 0)
            return;

        db.ExecuteCommand(
            "INSERT INTO dbo." + MapTableName + " (AccountKey, SystemProductId, ShopPostId, CreatedAt, UpdatedAt) VALUES ({0}, {1}, {2}, {3}, {3})",
            accountKey,
            systemProductId,
            shopPostId,
            now);
    }

    private static bool HasMapTable(dbDataContext db)
    {
        return HasTable(db, MapTableName);
    }

    private static bool HasTable(dbDataContext db, string tableName)
    {
        if (db == null)
            return false;
        if (string.IsNullOrWhiteSpace(tableName))
            return false;

        try
        {
            int count = db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}",
                tableName).FirstOrDefault();
            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    private static void EnsureCompanyPolicy(dbDataContext db, string accountKey)
    {
        if (db == null)
            return;

        string account = (accountKey ?? "").Trim().ToLowerInvariant();
        if (account == "" || !HasTable(db, PolicyTableName))
            return;

        int percent = CompanyShop_cl.GetDefaultCommissionPercent();
        ShopPolicy_cl.UpsertPolicy(
            db,
            account,
            percent,
            null,
            "system",
            "Auto bootstrap company shop.");

        int activePercent;
        if (ShopPolicy_cl.TryGetActivePolicyPercent(db, account, out activePercent))
            return;

        DateTime now = AhaTime_cl.Now;
        db.ExecuteCommand(
            "INSERT INTO dbo.CoreShopPolicy_tb " +
            "(AccountKey, CommissionPercent, PolicyStatus, ApprovedRequestId, ApprovedBy, ApprovedAt, Note, CreatedAt, UpdatedAt) " +
            "VALUES ({0}, {1}, {2}, NULL, {3}, {4}, {5}, {4}, {4})",
            account,
            percent,
            ShopPolicy_cl.PolicyStatusActive,
            "system",
            now,
            "Auto bootstrap company shop (fallback).");
    }

    private static string BuildMarker(int systemProductId)
    {
        return "company-system-product-" + systemProductId.ToString();
    }
}
