using System;
using System.Collections.Generic;
using System.Linq;

public static class SpaceAccess_cl
{
    public const string StatusActive = "active";
    public const string StatusPending = "pending";
    public const string StatusBlocked = "blocked";
    public const string StatusRevoked = "revoked";

    public sealed class SpaceAccessRow
    {
        public long Id { get; set; }
        public string AccountKey { get; set; }
        public string SpaceCode { get; set; }
        public string AccessStatus { get; set; }
        public string AccessSource { get; set; }
        public bool IsPrimary { get; set; }
        public long? ApprovedRequestId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string LockedReason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private sealed class SpaceAccessRowRaw
    {
        public long Id { get; set; }
        public string AccountKey { get; set; }
        public string SpaceCode { get; set; }
        public string AccessStatus { get; set; }
        public string AccessSource { get; set; }
        public int? IsPrimaryValue { get; set; }
        public long? ApprovedRequestId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string LockedReason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public static IEnumerable<string> SplitLegacyPermission(string raw)
    {
        return PortalScope_cl.SplitPermissionTokens(raw)
            .Select(p => (p ?? "").Trim().ToLowerInvariant())
            .Where(p => p != "");
    }

    public static bool HasLegacyPermission(string permissionRaw, params string[] permissionCodes)
    {
        HashSet<string> set = new HashSet<string>(SplitLegacyPermission(permissionRaw), StringComparer.OrdinalIgnoreCase);
        if (set.Contains("all"))
            return true;

        if (permissionCodes == null || permissionCodes.Length == 0)
            return false;

        for (int i = 0; i < permissionCodes.Length; i++)
        {
            string code = (permissionCodes[i] ?? "").Trim();
            if (code == "")
                continue;

            if (set.Contains(code))
                return true;
        }

        return false;
    }

    public static bool CanAccessHome(taikhoan_tb account)
    {
        if (account == null)
            return false;

        return PortalScope_cl.CanLoginHome(account.taikhoan, account.phanloai, account.permission);
    }

    public static bool CanAccessShop(taikhoan_tb account)
    {
        if (account == null)
            return false;

        return PortalScope_cl.CanLoginShop(account.taikhoan, account.phanloai, account.permission);
    }

    public static bool CanAccessGianHang(taikhoan_tb account)
    {
        if (account == null)
            return false;

        return HasLegacyPermission(account.permission, "portal_gianhang", "gianhang", "mod_gianhang");
    }

    public static bool CanAccessAdmin(taikhoan_tb account)
    {
        if (account == null)
            return false;

        return PortalScope_cl.CanLoginAdmin(account.taikhoan, account.phanloai, account.permission);
    }

    public static bool CanAccessDauGia(taikhoan_tb account)
    {
        if (account == null)
            return false;

        return HasLegacyPermission(account.permission, "portal_daugia", "daugia", "mod_daugia");
    }

    public static bool CanAccessEvent(taikhoan_tb account)
    {
        if (account == null)
            return false;

        return HasLegacyPermission(
            account.permission,
            EventPolicy_cl.HomePermissionCode,
            EventPolicy_cl.AdminPermissionCode,
            EventPolicy_cl.RoleOwner,
            EventPolicy_cl.RoleDesigner,
            EventPolicy_cl.RoleApprover,
            EventPolicy_cl.RoleOperator,
            EventPolicy_cl.RoleViewer,
            "portal_event",
            "event",
            "mod_event");
    }

    public static bool CanAccessGianHangAdmin(taikhoan_tb account)
    {
        if (account == null)
            return false;

        return HasLegacyPermission(account.permission, "portal_gianhang_admin", "gianhang_admin", "mod_gianhang_admin");
    }

    public static bool CanAccessSpace(taikhoan_tb account, string spaceCode)
    {
        switch (ModuleSpace_cl.Normalize(spaceCode))
        {
            case ModuleSpace_cl.Home:
            case ModuleSpace_cl.BatDongSan:
                return CanAccessHome(account);
            case ModuleSpace_cl.GianHang:
                return CanAccessGianHang(account);
            case ModuleSpace_cl.Shop:
                return CanAccessShop(account);
            case ModuleSpace_cl.Admin:
                return CanAccessAdmin(account);
            case ModuleSpace_cl.GianHangAdmin:
                return CanAccessGianHangAdmin(account);
            case ModuleSpace_cl.DauGia:
                return CanAccessDauGia(account);
            case ModuleSpace_cl.Event:
                return CanAccessEvent(account);
            default:
                return false;
        }
    }

    public static IList<string> GetLegacyAccessSpaces(taikhoan_tb account)
    {
        List<string> spaces = new List<string>();
        if (account == null)
            return spaces;

        if (CanAccessHome(account)) spaces.Add(ModuleSpace_cl.Home);
        if (CanAccessHome(account)) spaces.Add(ModuleSpace_cl.BatDongSan);
        if (CanAccessGianHang(account)) spaces.Add(ModuleSpace_cl.GianHang);
        if (CanAccessShop(account)) spaces.Add(ModuleSpace_cl.Shop);
        if (CanAccessAdmin(account)) spaces.Add(ModuleSpace_cl.Admin);
        if (CanAccessGianHangAdmin(account)) spaces.Add(ModuleSpace_cl.GianHangAdmin);
        if (CanAccessDauGia(account)) spaces.Add(ModuleSpace_cl.DauGia);
        if (CanAccessEvent(account)) spaces.Add(ModuleSpace_cl.Event);
        return spaces;
    }

    public static bool CanAccessHome(dbDataContext db, taikhoan_tb account)
    {
        return ResolveAccess(db, account, ModuleSpace_cl.Home, delegate { return CanAccessHome(account); });
    }

    public static bool CanAccessShop(dbDataContext db, taikhoan_tb account)
    {
        return ResolveAccess(db, account, ModuleSpace_cl.Shop, delegate { return CanAccessShop(account); });
    }

    public static bool CanAccessGianHang(dbDataContext db, taikhoan_tb account)
    {
        return ResolveAccess(db, account, ModuleSpace_cl.GianHang, delegate { return CanAccessGianHang(account); });
    }

    public static bool CanAccessAdmin(dbDataContext db, taikhoan_tb account)
    {
        return ResolveAccess(db, account, ModuleSpace_cl.Admin, delegate { return CanAccessAdmin(account); });
    }

    public static bool CanAccessDauGia(dbDataContext db, taikhoan_tb account)
    {
        return ResolveAccess(db, account, ModuleSpace_cl.DauGia, delegate { return CanAccessDauGia(account); });
    }

    public static bool CanAccessGianHangAdmin(dbDataContext db, taikhoan_tb account)
    {
        return ResolveAccess(db, account, ModuleSpace_cl.GianHangAdmin, delegate { return CanAccessGianHangAdmin(account); });
    }

    public static bool CanAccessEvent(dbDataContext db, taikhoan_tb account)
    {
        return ResolveAccess(db, account, ModuleSpace_cl.Event, delegate { return CanAccessEvent(account); });
    }

    public static bool CanAccessSpace(dbDataContext db, taikhoan_tb account, string spaceCode)
    {
        switch (ModuleSpace_cl.Normalize(spaceCode))
        {
            case ModuleSpace_cl.Home:
            case ModuleSpace_cl.BatDongSan:
                return CanAccessHome(db, account);
            case ModuleSpace_cl.GianHang:
                return CanAccessGianHang(db, account);
            case ModuleSpace_cl.Shop:
                return CanAccessShop(db, account);
            case ModuleSpace_cl.Admin:
                return CanAccessAdmin(db, account);
            case ModuleSpace_cl.GianHangAdmin:
                return CanAccessGianHangAdmin(db, account);
            case ModuleSpace_cl.DauGia:
                return CanAccessDauGia(db, account);
            case ModuleSpace_cl.Event:
                return CanAccessEvent(db, account);
            default:
                return false;
        }
    }

    public static string GetSpaceStatus(dbDataContext db, string accountKey, string spaceCode)
    {
        if (db == null)
            return "";

        string key = (accountKey ?? "").Trim();
        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (key == "" || normalizedSpace == "")
            return "";

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        return db.ExecuteQuery<string>(
                "SELECT TOP 1 AccessStatus FROM dbo.CoreSpaceAccess_tb WHERE AccountKey = {0} AND SpaceCode = {1} ORDER BY IsPrimary DESC, Id DESC",
                key,
                normalizedSpace)
            .FirstOrDefault() ?? "";
    }

    public static SpaceAccessRow GetAccessRow(dbDataContext db, string accountKey, string spaceCode)
    {
        if (db == null)
            return null;

        string key = (accountKey ?? "").Trim();
        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (key == "" || normalizedSpace == "")
            return null;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        SpaceAccessRowRaw raw = db.ExecuteQuery<SpaceAccessRowRaw>(
                "SELECT TOP 1 Id, AccountKey, SpaceCode, AccessStatus, AccessSource, CAST(IsPrimary AS INT) AS IsPrimaryValue, ApprovedRequestId, ApprovedBy, ApprovedAt, LockedReason, CreatedAt, UpdatedAt " +
                "FROM dbo.CoreSpaceAccess_tb WHERE AccountKey = {0} AND SpaceCode = {1} ORDER BY IsPrimary DESC, Id DESC",
                key,
                normalizedSpace)
            .FirstOrDefault();

        if (raw == null)
            return null;

        return new SpaceAccessRow
        {
            Id = raw.Id,
            AccountKey = raw.AccountKey,
            SpaceCode = raw.SpaceCode,
            AccessStatus = raw.AccessStatus,
            AccessSource = raw.AccessSource,
            IsPrimary = raw.IsPrimaryValue.HasValue && raw.IsPrimaryValue.Value != 0,
            ApprovedRequestId = raw.ApprovedRequestId,
            ApprovedBy = raw.ApprovedBy,
            ApprovedAt = raw.ApprovedAt,
            LockedReason = raw.LockedReason,
            CreatedAt = raw.CreatedAt,
            UpdatedAt = raw.UpdatedAt
        };
    }

    public static void UpsertSpaceAccess(
        dbDataContext db,
        string accountKey,
        string spaceCode,
        string accessStatus,
        string accessSource,
        bool isPrimary,
        string approvedBy,
        DateTime? approvedAt,
        string lockedReason,
        long? approvedRequestId)
    {
        if (db == null)
            return;

        string key = (accountKey ?? "").Trim();
        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        string status = NormalizeStatus(accessStatus);
        if (key == "" || normalizedSpace == "")
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        DateTime now = AhaTime_cl.Now;
        int isPrimaryValue = isPrimary ? 1 : 0;
        string accessSourceValue = (accessSource ?? "").Trim();
        string approvedByValue = (approvedBy ?? "").Trim();
        string lockedReasonValue = (lockedReason ?? "").Trim();

        if (isPrimary)
        {
            db.ExecuteCommand(
                "UPDATE dbo.CoreSpaceAccess_tb SET IsPrimary = 0, UpdatedAt = {1} WHERE AccountKey = {0} AND IsPrimary = 1",
                key,
                now);
        }

        int affected = 0;
        if (approvedRequestId.HasValue && approvedAt.HasValue)
        {
            affected = db.ExecuteCommand(
                "UPDATE dbo.CoreSpaceAccess_tb " +
                "SET AccessStatus = {2}, AccessSource = {3}, IsPrimary = {4}, ApprovedRequestId = {5}, ApprovedBy = {6}, ApprovedAt = {7}, LockedReason = {8}, UpdatedAt = {9} " +
                "WHERE AccountKey = {0} AND SpaceCode = {1}",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedRequestId.Value,
                approvedByValue,
                approvedAt.Value,
                lockedReasonValue,
                now);
        }
        else if (approvedRequestId.HasValue)
        {
            affected = db.ExecuteCommand(
                "UPDATE dbo.CoreSpaceAccess_tb " +
                "SET AccessStatus = {2}, AccessSource = {3}, IsPrimary = {4}, ApprovedRequestId = {5}, ApprovedBy = {6}, ApprovedAt = NULL, LockedReason = {7}, UpdatedAt = {8} " +
                "WHERE AccountKey = {0} AND SpaceCode = {1}",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedRequestId.Value,
                approvedByValue,
                lockedReasonValue,
                now);
        }
        else if (approvedAt.HasValue)
        {
            affected = db.ExecuteCommand(
                "UPDATE dbo.CoreSpaceAccess_tb " +
                "SET AccessStatus = {2}, AccessSource = {3}, IsPrimary = {4}, ApprovedRequestId = NULL, ApprovedBy = {5}, ApprovedAt = {6}, LockedReason = {7}, UpdatedAt = {8} " +
                "WHERE AccountKey = {0} AND SpaceCode = {1}",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedByValue,
                approvedAt.Value,
                lockedReasonValue,
                now);
        }
        else
        {
            affected = db.ExecuteCommand(
                "UPDATE dbo.CoreSpaceAccess_tb " +
                "SET AccessStatus = {2}, AccessSource = {3}, IsPrimary = {4}, ApprovedRequestId = NULL, ApprovedBy = {5}, ApprovedAt = NULL, LockedReason = {6}, UpdatedAt = {7} " +
                "WHERE AccountKey = {0} AND SpaceCode = {1}",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedByValue,
                lockedReasonValue,
                now);
        }

        if (affected > 0)
            return;

        if (approvedRequestId.HasValue && approvedAt.HasValue)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreSpaceAccess_tb (AccountKey, SpaceCode, AccessStatus, AccessSource, IsPrimary, ApprovedRequestId, ApprovedBy, ApprovedAt, LockedReason, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedRequestId.Value,
                approvedByValue,
                approvedAt.Value,
                lockedReasonValue,
                now,
                now);
        }
        else if (approvedRequestId.HasValue)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreSpaceAccess_tb (AccountKey, SpaceCode, AccessStatus, AccessSource, IsPrimary, ApprovedRequestId, ApprovedBy, ApprovedAt, LockedReason, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, NULL, {7}, {8}, {9})",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedRequestId.Value,
                approvedByValue,
                lockedReasonValue,
                now,
                now);
        }
        else if (approvedAt.HasValue)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreSpaceAccess_tb (AccountKey, SpaceCode, AccessStatus, AccessSource, IsPrimary, ApprovedRequestId, ApprovedBy, ApprovedAt, LockedReason, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, NULL, {5}, {6}, {7}, {8}, {9})",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedByValue,
                approvedAt.Value,
                lockedReasonValue,
                now,
                now);
        }
        else
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreSpaceAccess_tb (AccountKey, SpaceCode, AccessStatus, AccessSource, IsPrimary, ApprovedRequestId, ApprovedBy, ApprovedAt, LockedReason, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, NULL, {5}, NULL, {6}, {7}, {8})",
                key,
                normalizedSpace,
                status,
                accessSourceValue,
                isPrimaryValue,
                approvedByValue,
                lockedReasonValue,
                now,
                now);
        }
    }

    public static void DeleteSpaceAccess(dbDataContext db, string accountKey, string spaceCode)
    {
        if (db == null)
            return;

        string key = (accountKey ?? "").Trim();
        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (key == "" || normalizedSpace == "")
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        db.ExecuteCommand(
            "DELETE FROM dbo.CoreSpaceAccess_tb WHERE AccountKey = {0} AND SpaceCode = {1}",
            key,
            normalizedSpace);
    }

    public static bool CanAccessPath(taikhoan_tb account, string path)
    {
        string spaceCode = ModuleSpace_cl.ResolveByPath(path);
        if (spaceCode == "")
            return false;

        return CanAccessSpace(account, spaceCode);
    }

    public static string GetDeniedMessage(string spaceCode)
    {
        switch (ModuleSpace_cl.Normalize(spaceCode))
        {
            case ModuleSpace_cl.GianHang:
                return "Tai khoan chua du quyen vao khong gian gian hang.";
            case ModuleSpace_cl.Shop:
                return "Tai khoan chua du quyen vao khong gian shop.";
            case ModuleSpace_cl.Admin:
                return "Tai khoan chua du quyen vao khong gian admin.";
            case ModuleSpace_cl.GianHangAdmin:
                return "Tai khoan chua du quyen vao khong gian gian hang admin.";
            case ModuleSpace_cl.DauGia:
                return "Tai khoan chua du quyen vao khong gian dau gia.";
            case ModuleSpace_cl.Event:
                return "Tai khoan chua du quyen vao khong gian event admin.";
            case ModuleSpace_cl.Home:
            case ModuleSpace_cl.BatDongSan:
                return "Tai khoan chua du quyen vao khong gian home.";
            default:
                return "Tai khoan chua du quyen truy cap.";
        }
    }

    private static bool ResolveAccess(dbDataContext db, taikhoan_tb account, string spaceCode, Func<bool> legacyFallback)
    {
        if (account == null)
            return false;

        string key = (account.taikhoan ?? "").Trim();
        if (key == "")
            return false;

        string status = GetSpaceStatus(db, key, spaceCode);
        if (status == StatusActive)
            return true;
        if (status == StatusPending || status == StatusBlocked || status == StatusRevoked)
            return false;

        if (IsManagedHomeSpace(spaceCode))
            return false;

        return legacyFallback != null && legacyFallback();
    }

    private static bool IsManagedHomeSpace(string spaceCode)
    {
        switch (ModuleSpace_cl.Normalize(spaceCode))
        {
            case ModuleSpace_cl.GianHang:
            case ModuleSpace_cl.GianHangAdmin:
            case ModuleSpace_cl.DauGia:
            case ModuleSpace_cl.Event:
                return true;
            default:
                return false;
        }
    }

    private static string NormalizeStatus(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == StatusPending) return StatusPending;
        if (value == StatusBlocked) return StatusBlocked;
        if (value == StatusRevoked) return StatusRevoked;
        return StatusActive;
    }
}
