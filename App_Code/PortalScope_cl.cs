using System;
using System.Collections.Generic;
using System.Linq;

public static class PortalScope_cl
{
    public const string ScopeAdmin = "portal_admin";
    public const string ScopeHome = "portal_home";
    public const string ScopeShop = "portal_shop";
    private const string AccountTypeAdminStaff = "Nhân viên admin";
    private const string AccountTypeShopPartner = "Gian hàng đối tác";

    private static bool IsRootAdmin(string taikhoan)
    {
        return string.Equals((taikhoan ?? "").Trim(), "admin", StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeScope(string scope)
    {
        string s = (scope ?? "").Trim().ToLowerInvariant();
        if (s == ScopeAdmin) return ScopeAdmin;
        if (s == ScopeHome) return ScopeHome;
        if (s == ScopeShop) return ScopeShop;
        return "";
    }

    public static IEnumerable<string> SplitPermissionTokens(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Enumerable.Empty<string>();

        return raw
            .Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => (x ?? "").Trim())
            .Where(x => x != "");
    }

    private static bool IsScopeToken(string token)
    {
        string t = NormalizeScope(token);
        return t == ScopeAdmin || t == ScopeHome || t == ScopeShop;
    }

    public static string GetExplicitScope(string permissionRaw)
    {
        bool hasAdmin = false;
        bool hasHome = false;
        bool hasShop = false;

        foreach (string token in SplitPermissionTokens(permissionRaw))
        {
            string t = NormalizeScope(token);
            if (t == ScopeAdmin) hasAdmin = true;
            if (t == ScopeHome) hasHome = true;
            if (t == ScopeShop) hasShop = true;
        }

        if (hasAdmin && !hasHome && !hasShop) return ScopeAdmin;
        if (hasHome && !hasAdmin && !hasShop) return ScopeHome;
        if (hasShop && !hasAdmin && !hasHome) return ScopeShop;

        // Nếu dữ liệu cũ lẫn nhiều scope, ưu tiên admin > shop > home.
        if (hasAdmin) return ScopeAdmin;
        if (hasShop) return ScopeShop;
        if (hasHome) return ScopeHome;

        return "";
    }

    private static bool IsAdminPermissionToken(string token)
    {
        string t = (token ?? "").Trim().ToLowerInvariant();
        if (t == "5") return true;
        if (t.StartsWith("q1_")) return true;
        if (t.StartsWith("q2_")) return true;
        return false;
    }

    private static bool HasAnyAdminPermission(string permissionRaw)
    {
        return SplitPermissionTokens(permissionRaw).Any(IsAdminPermissionToken);
    }

    private static bool IsShopPartnerType(string phanloai)
    {
        return string.Equals((phanloai ?? "").Trim(), AccountTypeShopPartner, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminStaffType(string phanloai)
    {
        return string.Equals((phanloai ?? "").Trim(), AccountTypeAdminStaff, StringComparison.OrdinalIgnoreCase);
    }

    public static bool ContainsAnyAdminPermission(string permissionRaw)
    {
        return HasAnyAdminPermission(permissionRaw);
    }

    public static string InferLegacyScope(string taikhoan, string phanloai, string permissionRaw)
    {
        // Fallback cho dữ liệu rất cũ chưa có scope token.
        if (IsRootAdmin(taikhoan)) return ScopeAdmin;
        if (HasAnyAdminPermission(permissionRaw)) return ScopeAdmin;
        if (IsShopPartnerType(phanloai)) return ScopeShop;
        if (AccountType_cl.IsTreasury(phanloai) || IsAdminStaffType(phanloai)) return ScopeAdmin;
        return ScopeHome;
    }

    public static string ResolveScope(string taikhoan, string phanloai, string permissionRaw)
    {
        // Chuẩn hóa mạnh cho dữ liệu cũ:
        // - Gian hàng đối tác luôn đi hệ shop (trừ khi là admin theo quyền/root).
        // - Admin quyền/root luôn đi hệ admin.
        if (IsRootAdmin(taikhoan)) return ScopeAdmin;
        if (HasAnyAdminPermission(permissionRaw)) return ScopeAdmin;
        if (IsShopPartnerType(phanloai)) return ScopeShop;

        string explicitScope = GetExplicitScope(permissionRaw);
        if (explicitScope != "") return explicitScope;
        return InferLegacyScope(taikhoan, phanloai, permissionRaw);
    }

    public static bool CanLoginAdmin(string taikhoan, string phanloai, string permissionRaw)
    {
        return string.Equals(ResolveScope(taikhoan, phanloai, permissionRaw), ScopeAdmin, StringComparison.OrdinalIgnoreCase);
    }

    public static bool CanLoginHome(string taikhoan, string phanloai, string permissionRaw)
    {
        return string.Equals(ResolveScope(taikhoan, phanloai, permissionRaw), ScopeHome, StringComparison.OrdinalIgnoreCase);
    }

    public static bool CanLoginShop(string taikhoan, string phanloai, string permissionRaw)
    {
        return string.Equals(ResolveScope(taikhoan, phanloai, permissionRaw), ScopeShop, StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizePermissionWithScope(string permissionRaw, string scope)
    {
        string normalizedScope = NormalizeScope(scope);
        List<string> tokens = SplitPermissionTokens(permissionRaw)
            .Where(t => !IsScopeToken(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedScope != "")
            tokens.Add(normalizedScope);

        return string.Join(",", tokens);
    }

    public static bool EnsureScope(taikhoan_tb account, string scope)
    {
        if (account == null) return false;

        bool changed = false;
        string current = account.permission ?? "";
        string next = NormalizePermissionWithScope(current, scope);
        if (!string.Equals(current, next, StringComparison.Ordinal))
        {
            account.permission = next;
            changed = true;
        }

        string targetScope = NormalizeScope(scope);
        if (targetScope == "")
            targetScope = ResolveScope(account.taikhoan, account.phanloai, next);

        if (!string.Equals(targetScope, ScopeShop, StringComparison.OrdinalIgnoreCase))
        {
            if (CleanupShopDataOutsideShopScope(account))
                changed = true;
        }

        return changed;
    }

    public static bool CleanupShopDataOutsideShopScope(taikhoan_tb account)
    {
        if (account == null) return false;

        bool changed = false;

        if (!string.IsNullOrEmpty(account.logo_shop)) { account.logo_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.anhbia_shop)) { account.anhbia_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.ten_shop)) { account.ten_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.sdt_shop)) { account.sdt_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.email_shop)) { account.email_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.link_zalo_shop)) { account.link_zalo_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.motangan_shop)) { account.motangan_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.diachi_shop)) { account.diachi_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.linkfb_shop)) { account.linkfb_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.youtube_shop)) { account.youtube_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.tiktok_shop)) { account.tiktok_shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.Ten_FB_Shop)) { account.Ten_FB_Shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.Ten_Youtube_Shop)) { account.Ten_Youtube_Shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.Ten_TikTok_Shop)) { account.Ten_TikTok_Shop = null; changed = true; }
        if (!string.IsNullOrEmpty(account.Ten_Zalo_Shop)) { account.Ten_Zalo_Shop = null; changed = true; }
        if ((account.HoSo_TieuDung_ShopOnly ?? 0m) != 0m) { account.HoSo_TieuDung_ShopOnly = 0m; changed = true; }
        if ((account.HoSo_UuDai_ShopOnly ?? 0m) != 0m) { account.HoSo_UuDai_ShopOnly = 0m; changed = true; }
        if ((account.ChiPhanTram_BanDichVu_ChoSan ?? 0) != 0) { account.ChiPhanTram_BanDichVu_ChoSan = 0; changed = true; }

        return changed;
    }
}
