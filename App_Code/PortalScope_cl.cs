using System;
using System.Collections.Generic;
using System.Linq;

public static class PortalScope_cl
{
    public const string ScopeAdmin = "portal_admin";
    public const string ScopeHome = "portal_home";
    public const string ScopeShop = "portal_shop";

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

    public static bool ContainsAnyAdminPermission(string permissionRaw)
    {
        return HasAnyAdminPermission(permissionRaw);
    }

    public static string InferLegacyScope(string taikhoan, string phanloai, string permissionRaw)
    {
        // Không dùng phanloai legacy để quyết định cổng đăng nhập.
        // Cổng đăng nhập được quyết định bởi scope token trong permission.
        // Fallback legacy chỉ dùng cho dữ liệu rất cũ chưa có scope.
        if (IsRootAdmin(taikhoan)) return ScopeAdmin;
        if (HasAnyAdminPermission(permissionRaw)) return ScopeAdmin;
        return ScopeHome;
    }

    public static string ResolveScope(string taikhoan, string phanloai, string permissionRaw)
    {
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

        string current = account.permission ?? "";
        string next = NormalizePermissionWithScope(current, scope);
        if (string.Equals(current, next, StringComparison.Ordinal))
            return false;

        account.permission = next;
        return true;
    }
}
