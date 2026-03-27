using System;
using System.Linq;

public static class ShopStatus_cl
{
    public const byte StatusPending = 0;
    public const byte StatusApproved = 1;
    public const byte StatusRejected = 2;
    public const byte StatusRevoked = 3;

    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null)
            return;
        if (_schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            try
            {
                CoreSchemaMigration_cl.EnsureSchemaSafe(db);

                // Host không chạy được DDL, nên chỉ cập nhật dữ liệu khi cột đã tồn tại.
                if (!HasTrangThaiColumn(db))
                {
                    _schemaReady = true;
                    return;
                }

                bool hasCoreSpaceAccess = HasTable(db, "CoreSpaceAccess_tb");
                bool hasCoreSpaceRequest = HasTable(db, "CoreSpaceRequest_tb");

                if (hasCoreSpaceAccess && hasCoreSpaceRequest)
                {
                    // Đồng bộ cột legacy TrangThai_Shop theo luồng mới CoreSpace.
                    db.ExecuteCommand(@"
;WITH AccessLatest AS (
    SELECT LOWER(LTRIM(RTRIM(ISNULL(AccountKey, '')))) AS AccountKey,
           LOWER(LTRIM(RTRIM(ISNULL(AccessStatus, '')))) AS AccessStatus,
           ROW_NUMBER() OVER (PARTITION BY LOWER(LTRIM(RTRIM(ISNULL(AccountKey, '')))) ORDER BY ISNULL(IsPrimary, 0) DESC, Id DESC) AS rn
    FROM dbo.CoreSpaceAccess_tb
    WHERE LOWER(LTRIM(RTRIM(ISNULL(SpaceCode, '')))) = 'shop'
),
RequestLatest AS (
    SELECT LOWER(LTRIM(RTRIM(ISNULL(AccountKey, '')))) AS AccountKey,
           LOWER(LTRIM(RTRIM(ISNULL(RequestStatus, '')))) AS RequestStatus,
           ROW_NUMBER() OVER (PARTITION BY LOWER(LTRIM(RTRIM(ISNULL(AccountKey, '')))) ORDER BY ISNULL(RequestedAt, UpdatedAt) DESC, Id DESC) AS rn
    FROM dbo.CoreSpaceRequest_tb
    WHERE LOWER(LTRIM(RTRIM(ISNULL(SpaceCode, '')))) = 'shop'
),
Derived AS (
    SELECT tk.taikhoan,
           CASE
               WHEN al.AccessStatus = 'active' THEN {0}
               WHEN al.AccessStatus = 'pending' THEN {1}
               WHEN al.AccessStatus IN ('blocked', 'revoked') THEN {2}
               WHEN rl.RequestStatus = 'approved' THEN {0}
               WHEN rl.RequestStatus = 'pending' THEN {1}
               WHEN rl.RequestStatus = 'rejected' THEN {3}
               WHEN rl.RequestStatus = 'cancelled' THEN {2}
               WHEN LOWER(LTRIM(RTRIM(ISNULL(tk.phanloai, '')))) = N'gian hàng đối tác'
                    OR CHARINDEX('{4}', LOWER(ISNULL(tk.permission, ''))) > 0
                    THEN {0}
               ELSE NULL
           END AS NewStatus
    FROM dbo.taikhoan_tb tk
    LEFT JOIN AccessLatest al
        ON al.rn = 1
       AND al.AccountKey = LOWER(LTRIM(RTRIM(ISNULL(tk.taikhoan, ''))))
    LEFT JOIN RequestLatest rl
        ON rl.rn = 1
       AND rl.AccountKey = LOWER(LTRIM(RTRIM(ISNULL(tk.taikhoan, ''))))
)
UPDATE tk
SET TrangThai_Shop = CAST(d.NewStatus AS TINYINT)
FROM dbo.taikhoan_tb tk
INNER JOIN Derived d ON d.taikhoan = tk.taikhoan
WHERE d.NewStatus IS NOT NULL
  AND CONVERT(INT, tk.TrangThai_Shop) <> d.NewStatus;
", StatusApproved, StatusPending, StatusRevoked, StatusRejected, PortalScope_cl.ScopeShop);
                }
                else
                {
                    // Fallback an toàn khi host chưa có CoreSpace tables.
                    db.ExecuteCommand(@"
UPDATE tk
SET TrangThai_Shop = {0}
FROM dbo.taikhoan_tb tk
WHERE (
        LOWER(LTRIM(RTRIM(ISNULL(tk.phanloai, '')))) = N'gian hàng đối tác'
        OR CHARINDEX('{1}', LOWER(ISNULL(tk.permission, ''))) > 0
      )
  AND CONVERT(INT, tk.TrangThai_Shop) <> {0};
", StatusApproved, PortalScope_cl.ScopeShop);
                }

                _schemaReady = true;
            }
            catch
            {
                // Nếu lỗi schema thì không retry liên tục để tránh làm chậm trang.
                _schemaReady = true;
            }
        }
    }

    public static bool HasTrangThaiColumn(dbDataContext db)
    {
        if (db == null)
            return false;
        try
        {
            int flag = 0;
            foreach (int row in db.ExecuteQuery<int>(@"
SELECT CASE
    WHEN COL_LENGTH('dbo.taikhoan_tb', 'TrangThai_Shop') IS NULL THEN 0
    ELSE 1
END AS ok"))
            {
                flag = row;
                break;
            }
            return flag == 1;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsShopAccount(taikhoan_tb acc)
    {
        if (acc == null)
            return false;

        // Fallback legacy-only cho các nơi chưa có db context.
        return IsLegacyShopScope(acc) && acc.TrangThai_Shop == StatusApproved;
    }

    public static bool IsShopAccount(dbDataContext db, taikhoan_tb acc)
    {
        return IsShopApproved(db, acc);
    }

    public static bool IsShopApproved(taikhoan_tb acc)
    {
        if (acc == null)
            return false;

        if (acc.TrangThai_Shop == StatusApproved)
            return true;
        if (acc.TrangThai_Shop == StatusPending || acc.TrangThai_Shop == StatusRejected || acc.TrangThai_Shop == StatusRevoked)
            return false;

        return IsLegacyShopScope(acc);
    }

    public static bool IsShopApproved(dbDataContext db, taikhoan_tb acc)
    {
        return ResolveStatus(db, acc) == StatusApproved;
    }

    public static byte ResolveStatus(dbDataContext db, taikhoan_tb acc)
    {
        if (acc == null)
            return StatusRejected;

        if (db != null)
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);

            string key = (acc.taikhoan ?? "").Trim().ToLowerInvariant();
            if (key != "")
            {
                string accessStatus = (SpaceAccess_cl.GetSpaceStatus(db, key, ModuleSpace_cl.Shop) ?? "").Trim().ToLowerInvariant();
                if (accessStatus == SpaceAccess_cl.StatusActive)
                    return StatusApproved;
                if (accessStatus == SpaceAccess_cl.StatusPending)
                    return StatusPending;
                if (accessStatus == SpaceAccess_cl.StatusBlocked || accessStatus == SpaceAccess_cl.StatusRevoked)
                    return StatusRevoked;

                CoreSpaceRequest_cl.SpaceRequestInfo latestRequest = CoreSpaceRequest_cl.GetLatestRequest(db, key, ModuleSpace_cl.Shop);
                if (latestRequest != null)
                {
                    string requestStatus = (latestRequest.RequestStatus ?? "").Trim().ToLowerInvariant();
                    if (requestStatus == CoreSpaceRequest_cl.StatusApproved)
                        return StatusApproved;
                    if (requestStatus == CoreSpaceRequest_cl.StatusPending)
                        return StatusPending;
                    if (requestStatus == CoreSpaceRequest_cl.StatusRejected)
                        return StatusRejected;
                    if (requestStatus == CoreSpaceRequest_cl.StatusCancelled)
                        return StatusRevoked;
                }
            }
        }

        if (acc.TrangThai_Shop == StatusApproved)
            return StatusApproved;
        if (acc.TrangThai_Shop == StatusPending)
            return StatusPending;
        if (acc.TrangThai_Shop == StatusRevoked)
            return StatusRevoked;
        if (acc.TrangThai_Shop == StatusRejected)
            return StatusRejected;

        return IsLegacyShopScope(acc) ? StatusApproved : StatusRejected;
    }

    public static string GetStatusText(byte status)
    {
        switch (status)
        {
            case StatusApproved:
                return "Hoạt động";
            case StatusPending:
                return "Chờ duyệt";
            case StatusRejected:
                return "Bị từ chối";
            default:
                return "Đang khóa";
        }
    }

    public static string GetStatusText(dbDataContext db, taikhoan_tb acc)
    {
        return GetStatusText(ResolveStatus(db, acc));
    }

    private static bool IsLegacyShopScope(taikhoan_tb acc)
    {
        if (acc == null)
            return false;

        string scope = PortalScope_cl.ResolveScope(acc.taikhoan, acc.phanloai, acc.permission);
        return string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasTable(dbDataContext db, string tableName)
    {
        if (db == null)
            return false;

        string name = (tableName ?? "").Trim();
        if (name == "")
            return false;

        try
        {
            int count = db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = {0}",
                name).FirstOrDefault();
            return count > 0;
        }
        catch
        {
            return false;
        }
    }
}
