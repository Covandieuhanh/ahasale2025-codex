using System;

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
                db.ExecuteCommand(@"
IF COL_LENGTH('dbo.taikhoan_tb', 'TrangThai_Shop') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
        ADD TrangThai_Shop TINYINT NOT NULL
        CONSTRAINT DF_taikhoan_tb_TrangThai_Shop DEFAULT(0) WITH VALUES;
END");

                // Ưu tiên trạng thái mới nhất theo hồ sơ đăng ký gian hàng đối tác.
                db.ExecuteCommand(@"
;WITH Latest AS (
    SELECT dk.taikhoan,
           dk.TrangThai,
           ROW_NUMBER() OVER (PARTITION BY dk.taikhoan ORDER BY dk.NgayTao DESC, dk.ID DESC) AS rn
    FROM dbo.DangKy_GianHangDoiTac_tb dk
)
UPDATE tk
SET TrangThai_Shop = l.TrangThai
FROM dbo.taikhoan_tb tk
INNER JOIN Latest l ON l.taikhoan = tk.taikhoan AND l.rn = 1;
");

                // Legacy shop accounts không có hồ sơ đăng ký -> mặc định đã duyệt.
                db.ExecuteCommand(@"
UPDATE tk
SET TrangThai_Shop = {0}
FROM dbo.taikhoan_tb tk
WHERE NOT EXISTS (SELECT 1 FROM dbo.DangKy_GianHangDoiTac_tb dk WHERE dk.taikhoan = tk.taikhoan)
  AND (
        LOWER(LTRIM(RTRIM(ISNULL(tk.phanloai, '')))) = N'gian hàng đối tác'
        OR CHARINDEX('{1}', LOWER(ISNULL(tk.permission, ''))) > 0
      );
", StatusApproved, PortalScope_cl.ScopeShop);

                _schemaReady = true;
            }
            catch
            {
                // Nếu lỗi schema thì để lần sau thử lại.
            }
        }
    }

    public static bool IsShopAccount(taikhoan_tb acc)
    {
        if (acc == null)
            return false;
        string scope = PortalScope_cl.ResolveScope(acc.taikhoan, acc.phanloai, acc.permission);
        return string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsShopApproved(taikhoan_tb acc)
    {
        if (!IsShopAccount(acc))
            return false;
        return acc.TrangThai_Shop == StatusApproved;
    }
}
