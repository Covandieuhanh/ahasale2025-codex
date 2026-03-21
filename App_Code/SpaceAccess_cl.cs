using System;
using System.Collections.Generic;
using System.Linq;

public static class SpaceAccess_cl
{
    public const string StatusActive = "active";
    public const string StatusPending = "pending";
    public const string StatusBlocked = "blocked";
    public const string StatusRevoked = "revoked";

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
                return CanAccessHome(account);
            case ModuleSpace_cl.Shop:
                return CanAccessShop(account);
            case ModuleSpace_cl.Admin:
                return CanAccessAdmin(account);
            case ModuleSpace_cl.GianHangAdmin:
                return CanAccessGianHangAdmin(account);
            case ModuleSpace_cl.DauGia:
                return CanAccessDauGia(account);
            default:
                return false;
        }
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
            case ModuleSpace_cl.Shop:
                return "Tai khoan chua du quyen vao khong gian shop.";
            case ModuleSpace_cl.Admin:
                return "Tai khoan chua du quyen vao khong gian admin.";
            case ModuleSpace_cl.GianHangAdmin:
                return "Tai khoan chua du quyen vao khong gian gian hang admin.";
            case ModuleSpace_cl.DauGia:
                return "Tai khoan chua du quyen vao khong gian dau gia.";
            case ModuleSpace_cl.Home:
                return "Tai khoan chua du quyen vao khong gian home.";
            default:
                return "Tai khoan chua du quyen truy cap.";
        }
    }
}

