using System;
using System.Linq;

public static class ShopLevel_cl
{
    public const string TokenLevel2 = "shop_level2";
    public const int LevelBasic = 1;
    public const int LevelAdvanced = 2;

    public static bool HasAdvancedToken(string permissionRaw)
    {
        return PortalScope_cl.SplitPermissionTokens(permissionRaw)
            .Any(token => string.Equals((token ?? "").Trim(), TokenLevel2, StringComparison.OrdinalIgnoreCase));
    }

    public static int ResolveLevel(taikhoan_tb shopAccount)
    {
        if (shopAccount == null)
            return LevelBasic;

        return HasAdvancedToken(shopAccount.permission) ? LevelAdvanced : LevelBasic;
    }

    public static int ResolveLevel(dbDataContext db, string shopAccount)
    {
        string tk = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return LevelBasic;

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        return ResolveLevel(account);
    }

    public static bool IsAdvancedEnabled(dbDataContext db, string shopAccount)
    {
        return ResolveLevel(db, shopAccount) >= LevelAdvanced;
    }

    public static string ResolveLevelLabel(dbDataContext db, string shopAccount)
    {
        return IsAdvancedEnabled(db, shopAccount) ? "Level 2" : "Level 1";
    }

    public static bool GrantAdvanced(taikhoan_tb shopAccount)
    {
        if (shopAccount == null)
            return false;

        string normalized = PortalScope_cl.NormalizePermissionWithScope(shopAccount.permission, PortalScope_cl.ScopeShop);
        bool changed = !string.Equals(normalized, shopAccount.permission ?? "", StringComparison.Ordinal);
        shopAccount.permission = normalized;

        if (HasAdvancedToken(shopAccount.permission))
            return changed;

        string current = shopAccount.permission ?? "";
        shopAccount.permission = string.IsNullOrWhiteSpace(current)
            ? TokenLevel2
            : current + "," + TokenLevel2;
        return true;
    }

    public static bool RevokeAdvanced(taikhoan_tb shopAccount)
    {
        if (shopAccount == null || string.IsNullOrWhiteSpace(shopAccount.permission))
            return false;

        var tokens = PortalScope_cl.SplitPermissionTokens(shopAccount.permission)
            .Where(token => !string.Equals((token ?? "").Trim(), TokenLevel2, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        string next = string.Join(",", tokens);
        if (string.Equals(next, shopAccount.permission ?? "", StringComparison.Ordinal))
            return false;

        shopAccount.permission = next;
        return true;
    }

    public static string ResolveOwnerShopAccountForAdmin(dbDataContext db, string adminUser, string userParentHint)
    {
        string adminTk = (adminUser ?? "").Trim().ToLowerInvariant();
        string ownerTk = (userParentHint ?? "").Trim().ToLowerInvariant();

        if (ownerTk != "")
            return ownerTk;

        if (db != null && adminTk != "")
        {
            taikhoan_table_2023 adminAccount = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == adminTk);
            if (adminAccount != null && !string.IsNullOrWhiteSpace(adminAccount.user_parent))
                return adminAccount.user_parent.Trim().ToLowerInvariant();
        }

        return adminTk;
    }

    public static bool CanUseAdvancedAdmin(dbDataContext db, string adminUser, string userParentHint)
    {
        string adminTk = (adminUser ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(adminTk))
            return false;

        if (string.Equals(adminTk, "admin", StringComparison.OrdinalIgnoreCase))
            return true;

        string ownerShopTk = ResolveOwnerShopAccountForAdmin(db, adminTk, userParentHint);
        if (string.IsNullOrWhiteSpace(ownerShopTk))
            return false;

        taikhoan_tb shopAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ownerShopTk);
        if (shopAccount == null)
            return true;

        return ResolveLevel(shopAccount) >= LevelAdvanced;
    }

    public static string ResolveOwnerAdminUsername(string shopAccount)
    {
        return (shopAccount ?? "").Trim().ToLowerInvariant();
    }

    public static bool EnableAdvancedForShop(dbDataContext db, string shopAccount, out taikhoan_tb shop, out taikhoan_table_2023 ownerAdmin, out bool createdOwnerAdmin)
    {
        shop = null;
        ownerAdmin = null;
        createdOwnerAdmin = false;

        string shopTk = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(shopTk))
            return false;

        shop = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == shopTk);
        if (shop == null)
            return false;

        string ownerAdminTk = ResolveOwnerAdminUsername(shopTk);
        bool ownerExisted = db.taikhoan_table_2023s.Any(p => p.taikhoan == ownerAdminTk);
        bool permissionChanged = GrantAdvanced(shop);
        if (permissionChanged)
            db.SubmitChanges();

        ownerAdmin = AhaShineContext_cl.EnsureAdvancedAdminBootstrapForShop(db, shopTk);
        if (ownerAdmin != null)
        {
            try
            {
                GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(
                    db,
                    shopTk,
                    ownerAdmin.id_chinhanh,
                    ownerAdmin.id_nganh,
                    true);
            }
            catch
            {
            }
        }
        createdOwnerAdmin = !ownerExisted && ownerAdmin != null;
        return permissionChanged || ownerAdmin != null;
    }
}
