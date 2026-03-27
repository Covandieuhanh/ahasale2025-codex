using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class ShopSlug_cl
{
    private static bool? _hasSlugShopColumnCache;

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        string normalized = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < normalized.Length; i++)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(normalized[i]);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(normalized[i]);
            }
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string ToSlug(string raw)
    {
        string s = RemoveDiacritics((raw ?? "").Trim().ToLower());
        s = s.Replace('\u0111', 'd');
        s = Regex.Replace(s, @"[^a-z0-9]+", "-");
        s = Regex.Replace(s, @"-+", "-").Trim('-');
        if (string.IsNullOrEmpty(s)) s = "shop";
        if (s.Length > 90) s = s.Substring(0, 90).Trim('-');
        if (string.IsNullOrEmpty(s)) s = "shop";
        return s;
    }

    public static bool HasSlugShopColumn(dbDataContext db)
    {
        if (db == null) return false;
        if (_hasSlugShopColumnCache.HasValue) return _hasSlugShopColumnCache.Value;

        try
        {
            int count = db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'taikhoan_tb' AND COLUMN_NAME = 'slug_shop'")
                .FirstOrDefault();
            _hasSlugShopColumnCache = count > 0;
        }
        catch
        {
            _hasSlugShopColumnCache = false;
        }

        return _hasSlugShopColumnCache.Value;
    }

    public static bool IsShopAccount(dbDataContext db, taikhoan_tb acc)
    {
        if (acc == null) return false;
        string scope = PortalScope_cl.ResolveScope(acc.taikhoan, acc.phanloai, acc.permission);
        if (!string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            return false;
        if (acc.block == true)
            return false;
        return true;
    }

    private static string ReadSlug(dbDataContext db, string taiKhoan)
    {
        if (db == null || string.IsNullOrEmpty(taiKhoan) || !HasSlugShopColumn(db))
            return "";

        try
        {
            string slug = db.ExecuteQuery<string>(
                "SELECT TOP 1 LOWER(LTRIM(RTRIM(ISNULL(slug_shop, '')))) FROM dbo.taikhoan_tb WHERE taikhoan = {0}",
                taiKhoan).FirstOrDefault();
            return (slug ?? "").Trim();
        }
        catch
        {
            return "";
        }
    }

    private static void WriteSlug(dbDataContext db, string taiKhoan, string slug)
    {
        if (db == null || string.IsNullOrEmpty(taiKhoan) || !HasSlugShopColumn(db))
            return;

        db.ExecuteCommand(
            "UPDATE dbo.taikhoan_tb SET slug_shop = {0} WHERE taikhoan = {1}",
            slug,
            taiKhoan);
    }

    private static bool SlugExists(dbDataContext db, string slug, string exceptTaiKhoan)
    {
        if (db == null || string.IsNullOrEmpty(slug) || !HasSlugShopColumn(db))
            return false;

        string except = (exceptTaiKhoan ?? "").Trim().ToLower();
        return db.ExecuteQuery<int>(
            "SELECT TOP 1 1 FROM dbo.taikhoan_tb WHERE LOWER(LTRIM(RTRIM(ISNULL(slug_shop, '')))) = {0} AND LOWER(LTRIM(RTRIM(ISNULL(taikhoan, '')))) <> {1}",
            slug,
            except).Any();
    }

    private static string BuildBaseSlug(taikhoan_tb acc)
    {
        if (acc == null) return "shop";

        string baseText = (acc.ten_shop ?? "").Trim();
        if (string.IsNullOrEmpty(baseText))
            baseText = (acc.hoten ?? "").Trim();
        if (string.IsNullOrEmpty(baseText))
            baseText = (acc.email_shop ?? "").Trim();
        if (string.IsNullOrEmpty(baseText))
            baseText = (acc.email ?? "").Trim();
        if (string.IsNullOrEmpty(baseText))
            baseText = (acc.taikhoan ?? "").Trim();

        if (baseText.Contains("@"))
            baseText = baseText.Split('@')[0];

        return ToSlug(baseText);
    }

    public static string EnsureSlugForShop(dbDataContext db, taikhoan_tb acc)
    {
        if (db == null || acc == null) return "";
        if (!HasSlugShopColumn(db)) return "";
        if (!IsShopAccount(db, acc)) return "";

        string currentSlug = ReadSlug(db, acc.taikhoan);
        if (!string.IsNullOrEmpty(currentSlug))
            return currentSlug;

        string baseSlug = BuildBaseSlug(acc);
        string candidate = baseSlug;
        int suffix = 0;
        while (SlugExists(db, candidate, acc.taikhoan))
        {
            suffix++;
            candidate = baseSlug + "-" + suffix.ToString();
            if (candidate.Length > 90)
                candidate = candidate.Substring(0, 90).Trim('-');
        }

        WriteSlug(db, acc.taikhoan, candidate);
        return candidate;
    }

    public static taikhoan_tb FindApprovedShopBySlug(dbDataContext db, string slugRaw)
    {
        if (db == null || !HasSlugShopColumn(db)) return null;
        string slug = ToSlug(slugRaw);
        if (string.IsNullOrEmpty(slug)) return null;

        string tk = db.ExecuteQuery<string>(
            "SELECT TOP 1 taikhoan FROM dbo.taikhoan_tb WHERE LOWER(LTRIM(RTRIM(ISNULL(slug_shop, '')))) = {0}",
            slug).FirstOrDefault();

        if (string.IsNullOrEmpty(tk)) return null;

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc == null) return null;
        if (!IsShopAccount(db, acc)) return null;

        return acc;
    }

    public static string GetPublicUrl(dbDataContext db, taikhoan_tb acc)
    {
        if (acc == null || string.IsNullOrEmpty(acc.taikhoan))
            return "/";

        if (IsShopAccount(db, acc))
        {
            string slug = EnsureSlugForShop(db, acc);
            if (!string.IsNullOrEmpty(slug))
                return "/shop/cong-khai/" + slug;
        }

        return "/" + (acc.taikhoan ?? "").Trim().ToLower() + ".info";
    }

    public static string GetPublicUrlByTaiKhoan(dbDataContext db, string taiKhoan)
    {
        string tk = (taiKhoan ?? "").Trim().ToLower();
        if (string.IsNullOrEmpty(tk)) return "/";

        if (db == null) return "/" + tk + ".info";

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc == null) return "/" + tk + ".info";

        return GetPublicUrl(db, acc);
    }
}
