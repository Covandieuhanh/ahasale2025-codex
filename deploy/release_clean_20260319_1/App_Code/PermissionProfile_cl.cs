using System;
using System.Linq;

public static class PermissionProfile_cl
{
    public const string HoSoTieuDung = "q2_1";
    public const string HoSoUuDai = "q2_2";
    public const string HoSoLaoDong = "q2_3";
    public const string HoSoGanKet = "q2_4";
    public const string HoSoShopOnly = "q2_5";

    public static readonly string[] LegacyTieuDungPermissions =
    {
        "q1_1",
        "q1_2",
        "q1_3",
        "q1_4",
        "q1_5",
        "q1_6",
        "q1_7"
    };

    public static bool IsRootAdmin(string taikhoan)
    {
        return string.Equals((taikhoan ?? "").Trim(), "admin", StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasPermission(dbDataContext db, string taikhoan, string permissionCode)
    {
        if (db == null || string.IsNullOrWhiteSpace(taikhoan) || string.IsNullOrWhiteSpace(permissionCode))
            return false;

        return check_login_cl.CheckQuyen(db, taikhoan.Trim(), permissionCode.Trim());
    }

    public static bool HasAnyPermission(dbDataContext db, string taikhoan, params string[] permissionCodes)
    {
        if (db == null || string.IsNullOrWhiteSpace(taikhoan) || permissionCodes == null || permissionCodes.Length == 0)
            return false;

        string tk = taikhoan.Trim();
        return permissionCodes.Any(code => HasPermission(db, tk, code));
    }

    public static bool HasTieuDungPermissionOrLegacy(dbDataContext db, string taikhoan)
    {
        if (db == null || string.IsNullOrWhiteSpace(taikhoan))
            return false;

        if (IsRootAdmin(taikhoan))
            return true;

        if (HasPermission(db, taikhoan, HoSoTieuDung))
            return true;

        return LegacyTieuDungPermissions.Any(code => HasPermission(db, taikhoan, code));
    }
}
