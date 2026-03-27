using System;
using System.Configuration;
using System.Linq;

public static class CompanyShop_cl
{
    public const string ProductTypePublic = "sanpham";
    public const string ProductTypeInternal = "sanpham_noibo";

    private const string AppSettingSpecialAccount = "CompanyShop.SpecialAccount";
    private const string DefaultSpecialAccount = "shop_cong_ty";
    private const string AppSettingSpecialPassword = "CompanyShop.SpecialPassword";
    private const string DefaultSpecialPassword = "123123";
    private const string AppSettingSpecialCommission = "CompanyShop.DefaultCommissionPercent";
    private const string AppSettingHideLegacyAdminSystemProduct = "CompanyShop.HideLegacyAdminSystemProduct";

    public static string GetSpecialAccount()
    {
        string configured = "";
        try
        {
            configured = ConfigurationManager.AppSettings[AppSettingSpecialAccount];
        }
        catch
        {
            configured = "";
        }

        configured = (configured ?? "").Trim().ToLowerInvariant();
        if (configured == "")
            configured = DefaultSpecialAccount;

        return configured;
    }

    public static string GetSpecialPassword()
    {
        string configured = "";
        try
        {
            configured = ConfigurationManager.AppSettings[AppSettingSpecialPassword];
        }
        catch
        {
            configured = "";
        }

        configured = (configured ?? "").Trim();
        if (configured == "")
            configured = DefaultSpecialPassword;
        return configured;
    }

    public static int GetDefaultCommissionPercent()
    {
        string configured = "";
        try
        {
            configured = ConfigurationManager.AppSettings[AppSettingSpecialCommission];
        }
        catch
        {
            configured = "";
        }

        int value;
        if (!int.TryParse((configured ?? "").Trim(), out value))
            value = 0;
        return ClampPlatformSharePercent(value);
    }

    public static bool HideLegacyAdminSystemProduct()
    {
        string raw = "";
        try
        {
            raw = ConfigurationManager.AppSettings[AppSettingHideLegacyAdminSystemProduct];
        }
        catch
        {
            raw = "";
        }

        raw = (raw ?? "").Trim().ToLowerInvariant();
        if (raw == "")
            return true;
        return raw == "1" || raw == "true" || raw == "yes" || raw == "on";
    }

    public static bool IsSpecialAccount(string taiKhoan)
    {
        string tk = (taiKhoan ?? "").Trim().ToLowerInvariant();
        if (tk == "")
            return false;

        return string.Equals(tk, GetSpecialAccount(), StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCompanyShopAccount(dbDataContext db, string taiKhoan)
    {
        if (db == null)
            return false;

        string tk = (taiKhoan ?? "").Trim().ToLowerInvariant();
        if (tk == "")
            return false;
        if (!IsSpecialAccount(tk))
            return false;

        CompanyShopBootstrap_cl.EnsureSpecialShopReady(db);

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc == null)
            return false;
        ShopStatus_cl.EnsureSchemaSafe(db);
        if (!ShopStatus_cl.IsShopApproved(db, acc))
            return false;

        return SpaceAccess_cl.CanAccessShop(db, acc);
    }

    public static bool IsCurrentPortalCompanyShop(dbDataContext db)
    {
        string tk = ResolveCurrentPortalAccount();
        return IsCompanyShopAccount(db, tk);
    }

    public static string ResolveCurrentPortalAccount()
    {
        string tk = PortalRequest_cl.GetCurrentAccount();
        return (tk ?? "").Trim().ToLowerInvariant();
    }

    public static bool IsInternalProductType(string phanloai)
    {
        return string.Equals((phanloai ?? "").Trim(), ProductTypeInternal, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPublicProductType(string phanloai)
    {
        return string.Equals((phanloai ?? "").Trim(), ProductTypePublic, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsManagedProductType(string phanloai)
    {
        string value = (phanloai ?? "").Trim().ToLowerInvariant();
        return value == ProductTypePublic || value == ProductTypeInternal;
    }

    public static string NormalizeProductType(string rawType, bool allowInternalType)
    {
        if (!allowInternalType)
            return ProductTypePublic;

        string value = (rawType ?? "").Trim().ToLowerInvariant();
        if (value == "internal" || value == ProductTypeInternal)
            return ProductTypeInternal;

        return ProductTypePublic;
    }

    public static string BuildProductTypeLabel(string phanloai)
    {
        return IsInternalProductType(phanloai) ? "Nội bộ" : "Công khai";
    }

    public static int ClampPlatformSharePercent(int value)
    {
        if (value < 0) return 0;
        if (value > 100) return 100;
        return value;
    }

    public static int GetPlatformSharePercent(BaiViet_tb post)
    {
        if (post == null)
            return 0;

        long raw = post.banhang_thuong ?? 0;
        int value;
        if (raw > int.MaxValue) value = int.MaxValue;
        else if (raw < int.MinValue) value = int.MinValue;
        else value = (int)raw;

        return ClampPlatformSharePercent(value);
    }

    public static void SetPlatformSharePercent(BaiViet_tb post, int value)
    {
        if (post == null)
            return;

        post.banhang_thuong = ClampPlatformSharePercent(value);
    }
}
