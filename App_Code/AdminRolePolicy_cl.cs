using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public static class AdminRolePolicy_cl
{
    public sealed class ScopedAdminPresetInfo
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string[] PermissionCodes { get; set; }
    }

    public sealed class AdminHomeTabInfo
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Meaning { get; set; }
        public string ActionSummary { get; set; }
        public string GuardrailSummary { get; set; }
        public string ScopeLabel { get; set; }
        public string ScopeMeaning { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
    }

    public sealed class AdminObjectGroupInfo
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string ScopeLabel { get; set; }
        public string Meaning { get; set; }
        public string StatusLabel { get; set; }
        public string StatusHint { get; set; }
        public bool IsActive { get; set; }
    }

    public const string RoleHomeCustomer = PermissionProfile_cl.HoSoUuDai;
    public const string RoleHomeDevelopment = PermissionProfile_cl.HoSoLaoDong;
    public const string RoleHomeEcosystem = PermissionProfile_cl.HoSoGanKet;
    public const string RoleShopPartner = PermissionProfile_cl.HoSoShopOnly;
    public const string RoleHomeContent = HomeTextContent_cl.PermissionCode;
    public const string RoleHomeConfig = "home_config_admin";
    public const string RoleHomeBdsLinked = "home_bds_linked_admin";
    public const string RoleHomePosts = "home_posts_admin";
    public const string RoleHomeMenu = "home_menu_admin";
    public const string RoleHomeBanner = "home_banner_admin";
    public const string RoleLegacyConsumerAsset = PermissionProfile_cl.HoSoTieuDung;
    public const string RoleAdminOtp = "admin_otp";
    public const string RoleAdminTokenWallet = "admin_token_wallet";
    public const string RoleAdminFeedback = "admin_feedback";
    public const string RoleAdminNotification = "admin_notification";
    public const string RoleAdminConsulting = "admin_consulting";
    public const string RoleAdminCompanyShopSync = "admin_company_shop_sync";
    public const string RoleAdminReindexBaiViet = "admin_reindex_baiviet";
    public const string RoleHomeGianHangSpaceApproval = "home_gianhang_space_approval";
    public const string RoleShopPartnerApproval = "shop_partner_approval";
    public const string RoleShopLegacyInvoices = "shop_legacy_invoice_ops";
    public const string RoleShopLegacyCustomers = "shop_legacy_customer_ops";
    public const string RoleShopLegacyContent = "shop_legacy_content_ops";
    public const string RoleShopLegacyFinance = "shop_legacy_finance_ops";
    public const string RoleShopLegacyInventory = "shop_legacy_inventory_ops";
    public const string RoleShopLegacyAccounts = "shop_legacy_accounts_ops";
    public const string RoleShopLegacyOrg = "shop_legacy_org_ops";
    public const string RoleShopLegacyTraining = "shop_legacy_training_ops";
    public const string RoleShopLegacySupport = "shop_legacy_support_ops";
    public const string PresetHomeCustomer = "home_customer";
    public const string PresetHomeDevelopment = "home_development";
    public const string PresetHomeEcosystem = "home_ecosystem";
    public const string PresetShopPartner = "shop_partner";
    public const string PresetHomeContent = "home_content";

    private static readonly ScopedAdminPresetInfo[] ScopedAdminPresets =
    {
        new ScopedAdminPresetInfo
        {
            Key = PresetHomeCustomer,
            Label = "Admin khách hàng",
            PermissionCodes = new[] { RoleHomeCustomer }
        },
        new ScopedAdminPresetInfo
        {
            Key = PresetHomeDevelopment,
            Label = "Admin cộng tác phát triển",
            PermissionCodes = new[] { RoleHomeDevelopment }
        },
        new ScopedAdminPresetInfo
        {
            Key = PresetHomeEcosystem,
            Label = "Admin đồng hành hệ sinh thái",
            PermissionCodes = new[] { RoleHomeEcosystem }
        },
        new ScopedAdminPresetInfo
        {
            Key = PresetShopPartner,
            Label = "Admin gian hàng đối tác",
            PermissionCodes = new[] { RoleShopPartner }
        },
        new ScopedAdminPresetInfo
        {
            Key = PresetHomeContent,
            Label = "Admin nội dung Home",
            PermissionCodes = new[] { RoleHomeContent }
        }
    };


    public static string GetCurrentAdminUser()
    {
        if (HttpContext.Current == null)
            return "";

        HttpContext ctx = HttpContext.Current;
        string tkEnc = "";
        if (ctx.Session != null)
            tkEnc = ctx.Session["taikhoan"] as string;

        if (string.IsNullOrWhiteSpace(tkEnc) && ctx.Request != null && ctx.Request.Cookies != null)
        {
            HttpCookie ck = ctx.Request.Cookies["cookie_userinfo_admin_bcorn"];
            if (ck != null)
            {
                tkEnc = (ck["taikhoan"] ?? "").Trim();
                if (string.IsNullOrWhiteSpace(tkEnc))
                {
                    string raw = (ck.Value ?? "").Trim();
                    if (raw != "")
                    {
                        foreach (string segment in raw.Split('&'))
                        {
                            if (string.IsNullOrWhiteSpace(segment))
                                continue;

                            int idx = segment.IndexOf('=');
                            if (idx <= 0)
                                continue;

                            string key = (HttpUtility.UrlDecode(segment.Substring(0, idx)) ?? "").Trim();
                            if (!string.Equals(key, "taikhoan", StringComparison.OrdinalIgnoreCase))
                                continue;

                            tkEnc = (segment.Substring(idx + 1) ?? "").Trim();
                            tkEnc = HttpUtility.UrlDecode(tkEnc);
                            break;
                        }
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(tkEnc))
            return "";

        try
        {
            string decoded = mahoa_cl.giaima_Bcorn(tkEnc);
            if (!string.IsNullOrWhiteSpace(decoded) && ctx.Session != null)
                ctx.Session["taikhoan"] = tkEnc;
            return decoded;
        }
        catch
        {
            return tkEnc;
        }
    }

    public static bool IsSuperAdmin(string taikhoan)
    {
        return PermissionProfile_cl.IsRootAdmin(taikhoan);
    }

    public static bool HasRole(dbDataContext db, string taikhoan, string permissionCode)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return PermissionProfile_cl.HasPermission(db, taikhoan, permissionCode);
    }

    public static bool CanAccessAdminWorkspace(dbDataContext db, string taikhoan)
    {
        if (string.IsNullOrWhiteSpace(taikhoan))
            return false;

        if (IsSuperAdmin(taikhoan))
            return true;

        return CanManageAdminAccounts(db, taikhoan)
            || CanManageAdminOtp(db, taikhoan)
            || CanManageAdminTokenWallet(db, taikhoan)
            || CanManageAdminFeedback(db, taikhoan)
            || CanManageAdminNotification(db, taikhoan)
            || CanManageAdminConsulting(db, taikhoan)
            || CanManageAdminCompanyShopSync(db, taikhoan)
            || CanManageAdminReindexBaiViet(db, taikhoan);
    }

    public static IList<ScopedAdminPresetInfo> GetScopedAdminPresets()
    {
        return ScopedAdminPresets;
    }

    public static ScopedAdminPresetInfo GetScopedAdminPreset(string key)
    {
        string normalized = (key ?? "").Trim();
        if (normalized == "")
            return null;

        return ScopedAdminPresets.FirstOrDefault(p => string.Equals(p.Key, normalized, StringComparison.OrdinalIgnoreCase));
    }

    public static string DescribeAdminRoleSummary(string permissionRaw)
    {
        var labels = new List<string>();
        var permissionSet = new HashSet<string>(check_login_cl
            .NormalizePermissionTokensForDisplay(permissionRaw)
            .Where(token => !string.Equals(token, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(token, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(token, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase)), StringComparer.OrdinalIgnoreCase);

        foreach (ScopedAdminPresetInfo preset in ScopedAdminPresets)
        {
            if (preset.PermissionCodes != null && preset.PermissionCodes.Any(code => permissionSet.Contains(code)))
                labels.Add(preset.Label);
        }

        if (permissionSet.Contains(RoleLegacyConsumerAsset))
            labels.Add("Tài sản lõi (Super Admin)");

        return string.Join(", ", labels.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }

    public static string GetScopeDisplayLabel(string scope)
    {
        string normalized = PortalScope_cl.NormalizeScope(scope);
        if (normalized == PortalScope_cl.ScopeAdmin)
            return "Cổng Admin";
        if (normalized == PortalScope_cl.ScopeHome)
            return "Hệ Home";
        if (normalized == PortalScope_cl.ScopeShop)
            return "Hệ Shop";
        return "Chưa xác định";
    }

    public static string GetAdminRoleLabel(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        if (IsSuperAdmin(taikhoan))
            return "Super Admin";

        string presetKey = MatchScopedAdminPresetKey(permissionRaw);
        ScopedAdminPresetInfo preset = GetScopedAdminPreset(presetKey);
        if (preset != null)
            return preset.Label;

        string summary = DescribeAdminRoleSummary(permissionRaw);
        if (!string.IsNullOrWhiteSpace(summary))
            return summary;

        if (PortalScope_cl.ResolveScope(taikhoan, phanloai, permissionRaw) == PortalScope_cl.ScopeAdmin)
        {
            if (AccountType_cl.IsTreasury(phanloai))
                return "Tài khoản tổng";
            return "Nhân viên admin";
        }

        return "Tài khoản quản trị";
    }

    public static string GetAdminRoleDescription(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        if (IsSuperAdmin(taikhoan))
            return "Toàn quyền hệ thống. Duy nhất được thao tác trực tiếp với tiền, quyền, điểm và tài sản lõi.";

        string presetKey = MatchScopedAdminPresetKey(permissionRaw);
        switch (presetKey)
        {
            case PresetHomeCustomer:
                return "Quản lý hồ sơ tầng khách hàng Home và duyệt các yêu cầu điểm đúng phạm vi tầng khách hàng.";
            case PresetHomeDevelopment:
                return "Quản lý hồ sơ tầng cộng tác phát triển và duyệt các yêu cầu điểm đúng phạm vi tầng này.";
            case PresetHomeEcosystem:
                return "Quản lý hồ sơ tầng đồng hành hệ sinh thái và duyệt các yêu cầu điểm đúng phạm vi tầng này.";
            case PresetShopPartner:
                return "Quản lý tài khoản shop, nghiệp vụ shop và duyệt các yêu cầu điểm hoặc nghiệp vụ thuộc gian hàng đối tác.";
            case PresetHomeContent:
                return "Chỉ quản lý nội dung hiển thị trên Home: văn bản, menu, bài viết và banner.";
        }

        if (PortalScope_cl.ResolveScope(taikhoan, phanloai, permissionRaw) == PortalScope_cl.ScopeAdmin)
            return "Tài khoản admin nghiệp vụ. Chỉ xem tài sản lõi và thao tác trong đúng phạm vi đã được cấp.";

        return "Tài khoản quản trị đã đăng nhập vào cổng admin.";
    }

    public static string MatchScopedAdminPresetKey(string permissionRaw)
    {
        var permissionSet = new HashSet<string>(check_login_cl
            .NormalizePermissionTokensForDisplay(permissionRaw)
            .Where(token => !string.Equals(token, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(token, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(token, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase)), StringComparer.OrdinalIgnoreCase);

        foreach (ScopedAdminPresetInfo preset in ScopedAdminPresets)
        {
            if (preset.PermissionCodes == null)
                continue;

            if (preset.PermissionCodes.Length == permissionSet.Count
                && preset.PermissionCodes.All(code => permissionSet.Contains(code)))
                return preset.Key;
        }

        return "";
    }

    public static string ResolveScopedAdminPresetKey(dbDataContext db, string taikhoan)
    {
        if (string.IsNullOrWhiteSpace(taikhoan))
            return "";

        if (IsSuperAdmin(taikhoan))
            return "";

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taikhoan);
        if (account != null)
        {
            string matched = MatchScopedAdminPresetKey(account.permission);
            if (!string.IsNullOrWhiteSpace(matched))
                return matched;
        }

        foreach (ScopedAdminPresetInfo preset in ScopedAdminPresets)
        {
            if (preset.PermissionCodes != null && preset.PermissionCodes.Any(code => HasRole(db, taikhoan, code)))
                return preset.Key;
        }

        return "";
    }

    public static string ResolveAdminAccountManagementUrl(dbDataContext db, string taikhoan)
    {
        if (CanManageAdminAccounts(db, taikhoan))
            return "/admin/quan-ly-tai-khoan/Default.aspx?scope=admin&fscope=admin";

        return ResolveLandingUrl(db, taikhoan);
    }

    public static string ResolveHomeAccountManagementUrl(dbDataContext db, string taikhoan)
    {
        if (!CanManageHomeAccounts(db, taikhoan))
            return ResolveLandingUrl(db, taikhoan);

        string presetKey = ResolveScopedAdminPresetKey(db, taikhoan);
        var query = new List<string>
        {
            "scope=home",
            "fscope=home"
        };

        if (presetKey == PresetHomeCustomer || presetKey == PresetHomeDevelopment || presetKey == PresetHomeEcosystem)
            query.Add("frole=" + HttpUtility.UrlEncode(presetKey));

        return "/admin/quan-ly-tai-khoan/Default.aspx?" + string.Join("&", query);
    }

    public static string ResolveShopAccountManagementUrl(dbDataContext db, string taikhoan)
    {
        if (!CanManageShopAccounts(db, taikhoan))
            return ResolveLandingUrl(db, taikhoan);

        return "/admin/quan-ly-tai-khoan/Default.aspx?scope=shop&fscope=shop&frole=" + HttpUtility.UrlEncode(PresetShopPartner);
    }

    public static bool AccountMatchesScopedPreset(string presetKey, string resolvedScope, string phanLoai, int tier)
    {
        string normalizedPreset = (presetKey ?? "").Trim();
        string normalizedScope = PortalScope_cl.NormalizeScope(resolvedScope);

        if (string.IsNullOrWhiteSpace(normalizedPreset))
            return true;

        if (string.Equals(normalizedPreset, PresetShopPartner, StringComparison.OrdinalIgnoreCase))
            return string.Equals(normalizedScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);

        if (string.Equals(normalizedPreset, PresetHomeContent, StringComparison.OrdinalIgnoreCase))
            return string.Equals(normalizedScope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase);

        if (!string.Equals(normalizedScope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
            return false;

        switch (normalizedPreset.ToLowerInvariant())
        {
            case PresetHomeCustomer:
                return tier <= TierHome_cl.Tier1;
            case PresetHomeDevelopment:
                return tier == TierHome_cl.Tier2;
            case PresetHomeEcosystem:
                return tier >= TierHome_cl.Tier3;
            default:
                return false;
        }
    }

    public static bool CanManageAdminAccounts(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan);
    }

    public static bool CanManageAdminOtp(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan)
            || PermissionProfile_cl.HasPermission(db, taikhoan, RoleAdminOtp);
    }

    public static bool CanManageAdminTokenWallet(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan)
            || PermissionProfile_cl.HasPermission(db, taikhoan, RoleAdminTokenWallet);
    }

    public static bool CanManageAdminFeedback(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan)
            || PermissionProfile_cl.HasPermission(db, taikhoan, RoleAdminFeedback);
    }

    public static bool CanManageAdminNotification(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan)
            || PermissionProfile_cl.HasPermission(db, taikhoan, RoleAdminNotification);
    }

    public static bool CanManageAdminConsulting(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan)
            || PermissionProfile_cl.HasPermission(db, taikhoan, RoleAdminConsulting);
    }

    public static bool CanManageAdminCompanyShopSync(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan)
            || PermissionProfile_cl.HasPermission(db, taikhoan, RoleAdminCompanyShopSync);
    }

    public static bool CanManageAdminReindexBaiViet(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan)
            || PermissionProfile_cl.HasPermission(db, taikhoan, RoleAdminReindexBaiViet);
    }

    public static bool CanManageHomeAccounts(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return PermissionProfile_cl.HasAnyPermission(
            db,
            taikhoan,
            RoleHomeCustomer,
            RoleHomeDevelopment,
            RoleHomeEcosystem);
    }

    public static bool CanManageShopAccounts(dbDataContext db, string taikhoan)
    {
        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopOperations(dbDataContext db, string taikhoan)
    {
        return CanManageShopAccounts(db, taikhoan);
    }

    private static bool IsGranularShopApprovalMode(dbDataContext db, string taikhoan)
    {
        if (string.IsNullOrWhiteSpace(taikhoan))
            return false;

        return PermissionProfile_cl.HasAnyPermission(
            db,
            taikhoan,
            RoleHomeGianHangSpaceApproval,
            RoleShopPartnerApproval);
    }

    public static bool UsesGranularShopApprovalMode(dbDataContext db, string taikhoan)
    {
        return IsGranularShopApprovalMode(db, taikhoan);
    }

    public static bool CanManageHomeContent(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return HasRole(db, taikhoan, RoleHomeContent);
    }

    public static bool CanManageHomeConfig(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return HasRole(db, taikhoan, RoleHomeConfig);
    }

    public static bool CanManageHomePosts(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return HasRole(db, taikhoan, RoleHomePosts);
    }

    public static bool CanManageHomeMenu(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return HasRole(db, taikhoan, RoleHomeMenu);
    }

    public static bool CanManageHomeBanner(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return HasRole(db, taikhoan, RoleHomeBanner);
    }

    private static bool IsGranularHomeContentMode(dbDataContext db, string taikhoan)
    {
        if (string.IsNullOrWhiteSpace(taikhoan))
            return false;

        return PermissionProfile_cl.HasAnyPermission(
            db,
            taikhoan,
            RoleHomeBdsLinked);
    }

    public static bool CanManageHomeBdsLinked(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleHomeBdsLinked))
            return true;

        if (IsGranularHomeContentMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleHomeContent);
    }

    public static bool CanReviewHomePointRequests(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        return PermissionProfile_cl.HasAnyPermission(
            db,
            taikhoan,
            RoleHomeCustomer,
            RoleHomeDevelopment,
            RoleHomeEcosystem);
    }

    public static bool CanReviewCustomerPointRequests(dbDataContext db, string taikhoan)
    {
        return HasRole(db, taikhoan, RoleHomeCustomer);
    }

    public static bool CanReviewDevelopmentPointRequests(dbDataContext db, string taikhoan)
    {
        return HasRole(db, taikhoan, RoleHomeDevelopment);
    }

    public static bool CanReviewEcosystemPointRequests(dbDataContext db, string taikhoan)
    {
        return HasRole(db, taikhoan, RoleHomeEcosystem);
    }

    public static bool CanReviewShopPointRequests(dbDataContext db, string taikhoan)
    {
        return HasRole(db, taikhoan, RoleShopPartner);
    }

    private static bool IsGranularShopLegacyMode(dbDataContext db, string taikhoan)
    {
        if (string.IsNullOrWhiteSpace(taikhoan))
            return false;

        return PermissionProfile_cl.HasAnyPermission(
            db,
            taikhoan,
            RoleShopLegacyInvoices,
            RoleShopLegacyCustomers,
            RoleShopLegacyContent,
            RoleShopLegacyFinance,
            RoleShopLegacyInventory,
            RoleShopLegacyAccounts,
            RoleShopLegacyOrg,
            RoleShopLegacyTraining,
            RoleShopLegacySupport);
    }

    public static bool CanManageShopLegacyInvoices(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyInvoices))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacyCustomers(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyCustomers))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacyContent(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyContent))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacyFinance(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyFinance))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacyInventory(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyInventory))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacyAccounts(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyAccounts))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacyOrg(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyOrg))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacyTraining(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacyTraining))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanManageShopLegacySupport(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopLegacySupport))
            return true;

        if (IsGranularShopLegacyMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanViewTierReference(dbDataContext db, string taikhoan)
    {
        return CanReviewHomePointRequests(db, taikhoan);
    }

    public static bool CanAdjustCoreAssets(dbDataContext db, string taikhoan)
    {
        return IsSuperAdmin(taikhoan);
    }

    public static bool CanAccessTransferHistory(dbDataContext db, string taikhoan)
    {
        return CanAdjustCoreAssets(db, taikhoan);
    }

    public static bool CanIssueCards(dbDataContext db, string taikhoan)
    {
        return CanAdjustCoreAssets(db, taikhoan);
    }

    public static bool CanSellSystemProducts(dbDataContext db, string taikhoan)
    {
        return CanAdjustCoreAssets(db, taikhoan);
    }

    public static bool CanApproveShopPartnerRegistration(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleShopPartnerApproval))
            return true;

        if (IsGranularShopApprovalMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanApproveHomeGianHangSpace(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (HasRole(db, taikhoan, RoleHomeGianHangSpaceApproval))
            return true;

        if (IsGranularShopApprovalMode(db, taikhoan))
            return false;

        return HasRole(db, taikhoan, RoleShopPartner);
    }

    public static bool CanApproveShopLevel2(dbDataContext db, string taikhoan)
    {
        return CanAdjustCoreAssets(db, taikhoan);
    }

    public static bool CanAdjustHomeTierManually(dbDataContext db, string taikhoan)
    {
        return CanAdjustCoreAssets(db, taikhoan);
    }

    private static string GetAdminHomeActionSummary(string key)
    {
        switch ((key ?? "").Trim().ToLowerInvariant())
        {
            case "admin_dashboard":
                return "Đi vào landing tổng của khối admin để xem đúng các tab nội bộ đang được cấp theo không gian quản trị admin.";
            case "admin_accounts":
                return "Tạo tài khoản admin, gán preset 5 vai trò và rà đúng scope làm việc.";
            case "admin_otp":
                return "Theo dõi cấu hình OTP, lịch sử gửi mã và hỗ trợ reset xác thực theo đúng phạm vi admin.";
            case "admin_feedback":
                return "Xem, lọc và xử lý toàn bộ góp ý gửi về hệ quản trị.";
            case "admin_company_shop_sync":
                return "Đồng bộ dữ liệu shop công ty, rà luồng seed và làm sạch dữ liệu vận hành liên quan.";
            case "admin_reindex_baiviet":
                return "Chạy lại chỉ mục bài viết để đồng bộ kết quả tìm kiếm và feed hiển thị.";
            case "home_accounts":
                return "Xem hồ sơ, tìm kiếm, hỗ trợ truy cập cơ bản và rà dữ liệu người dùng Home.";
            case "home_point_approval":
            case "home_points_customer":
            case "home_points_development":
            case "home_points_ecosystem":
            case "shop_points":
                return "Mở phiếu chờ duyệt, duyệt hoặc từ chối theo đúng rule và phạm vi được cấp.";
            case "tier_reference":
                return "Rà mô tả cấp bậc, tiêu chí hành vi và nội dung tham chiếu để đối chiếu đúng khi xử lý nghiệp vụ Home.";
            case "shop_accounts":
                return "Rà hồ sơ shop, trạng thái vận hành, level 1/level 2 và thông tin gian hàng.";
            case "shop_workspace":
                return "Đi vào trung tâm quản trị gian hàng để thao tác đúng các công cụ vận hành của không gian gian hàng đối tác.";
            case "shop_legacy_invoices":
                return "Vào module hóa đơn legacy để xử lý đơn, in chứng từ và đối soát nghiệp vụ gian hàng.";
            case "shop_legacy_customers":
                return "Vào module khách hàng/lịch hẹn legacy để theo dõi hồ sơ và xử lý tương tác khách hàng.";
            case "shop_legacy_content":
                return "Vào module nội dung legacy để xử lý menu, bài viết và cấu hình hiển thị gian hàng.";
            case "shop_legacy_finance":
                return "Vào module thu/chi legacy để xử lý chứng từ tài chính nội bộ gian hàng.";
            case "shop_legacy_inventory":
                return "Vào module kho/vật tư legacy để xử lý nhập kho, tồn kho và vật tư.";
            case "shop_legacy_accounts":
                return "Vào module tài khoản/nhân sự legacy để quản lý nhân sự, phân quyền và dữ liệu tài khoản shop.";
            case "shop_legacy_org":
                return "Vào module cơ cấu hệ thống legacy để quản lý chi nhánh, ngành và phòng ban gian hàng.";
            case "shop_legacy_training":
                return "Vào module học viên/giảng viên legacy để vận hành đào tạo nội bộ gian hàng.";
            case "shop_legacy_support":
                return "Vào module hỗ trợ legacy để xử lý thông báo và yêu cầu tư vấn.";
            case "shop_level2":
                return "Xem danh sách yêu cầu, thẩm định và duyệt quyền dùng bộ công cụ level 2.";
            case "home_gianhang_space":
                return "Duyệt hoặc từ chối yêu cầu mở không gian /gianhang do tài khoản Home gửi lên.";
            case "shop_partner":
                return "Duyệt đăng ký gian hàng đối tác của shop và duyệt mở không gian /gianhang cho tài khoản Home.";
            case "shop_email":
                return "Cập nhật mẫu email vận hành và nội dung hệ thống gửi tới shop.";
            case "daugia_workspace":
                return "Đi vào trung tâm quản trị đấu giá để thao tác đúng các tab và công cụ đã được cấp cho không gian đấu giá.";
            case "event_workspace":
                return "Đi vào trung tâm quản trị sự kiện để thao tác đúng các tab và công cụ đã được cấp cho không gian sự kiện.";
            case "home_content":
                return "Sửa toàn bộ nội dung văn bản hiển thị trên Ahasale.vn và sẵn sàng mở rộng cho /home, /shop, /gianhang/admin, /daugia.";
            case "home_posts":
                return "Quản trị bài viết tổng quan và kho nội dung công khai cấp website.";
            case "home_menu":
                return "Quản lý menu, danh mục và điều hướng tổng quan của website.";
            case "home_banner":
                return "Quản lý banner và các khối hiển thị chiến dịch tổng quan của website.";
            case "home_config":
                return "Cấu hình các thiết lập lõi của Home như liên kết, maintenance và block hệ thống.";
            case "notifications":
                return "Lọc, xuất/in và theo dõi thông báo hệ thống theo luồng vận hành.";
            case "consulting":
                return "Lọc, xuất/in và xử lý các yêu cầu tư vấn phát sinh trên nền tảng.";
            case "system_products":
                return "Tạo giao dịch bán sản phẩm hệ thống và theo dõi kết quả xử lý.";
            case "issue_cards":
                return "Phát hành thẻ theo quy trình tài sản lõi và ghi nhận lịch sử phát hành.";
            case "core_assets":
                return "Đối soát lịch sử điểm, quyền và các giao dịch tài sản lõi toàn hệ thống.";
            case "token_wallet":
                return "Đối chiếu blockchain, xem bridge gần nhất và mở cấu hình ví/token.";
            default:
                return "Mở tab, xem dữ liệu và thao tác đúng phạm vi đã được cấp.";
        }
    }

    private static string GetAdminHomeGuardrailSummary(string key, bool isSuperAdmin)
    {
        switch ((key ?? "").Trim().ToLowerInvariant())
        {
            case "admin_dashboard":
                return "Landing của khối admin chỉ hiển thị các tab nội bộ của admin; không trộn tab Home, gian hàng, đấu giá hay sự kiện.";
            case "admin_accounts":
                return isSuperAdmin
                    ? "Có thể gán vai trò, nhưng mọi thay đổi cấu trúc quyền cần được kiểm soát như nghiệp vụ lõi."
                    : "Chỉ Super Admin mới được thay đổi cấu trúc vai trò hoặc cấp quyền admin.";
            case "admin_otp":
            case "admin_feedback":
            case "admin_company_shop_sync":
            case "admin_reindex_baiviet":
                return isSuperAdmin
                    ? "Khối này thuộc vận hành nội bộ admin. Chỉ thao tác trong phạm vi cấu hình, theo dõi và đồng bộ hệ thống."
                    : "Chỉ Super Admin mới được truy cập khối vận hành nội bộ của admin.";
            case "home_accounts":
                return "Chỉ thao tác trên dữ liệu Home thuộc scope phụ trách. Tiền, quyền và điểm chỉ được xem.";
            case "home_point_approval":
            case "home_points_customer":
            case "home_points_development":
            case "home_points_ecosystem":
            case "shop_points":
                return "Được duyệt/từ chối theo phiếu. Không nhập tay điểm và không cộng trừ trực tiếp tài sản lõi.";
            case "tier_reference":
                return "Đây là tab tham chiếu nghiệp vụ Home. Được xem mô tả cấp bậc nhưng không tự thay đổi điểm, quyền hay tài sản lõi.";
            case "shop_accounts":
            case "shop_level2":
            case "shop_partner":
            case "shop_workspace":
                return "Chỉ xử lý trạng thái nghiệp vụ shop và quyền mở /gianhang của Home. Không can thiệp trực tiếp tiền, quyền hoặc điểm của shop.";
            case "shop_legacy_invoices":
                return "Chỉ thao tác nghiệp vụ hóa đơn/đơn ở lớp legacy. Không can thiệp trực tiếp tiền, quyền hoặc điểm lõi.";
            case "shop_legacy_customers":
                return "Chỉ thao tác nghiệp vụ khách hàng/lịch hẹn ở lớp legacy. Không can thiệp trực tiếp tiền, quyền hoặc điểm lõi.";
            case "shop_legacy_content":
                return "Chỉ thao tác nội dung và cấu hình hiển thị ở lớp legacy. Không can thiệp tài sản lõi.";
            case "shop_legacy_finance":
                return "Chỉ thao tác chứng từ thu/chi nội bộ ở lớp legacy. Không can thiệp trực tiếp quyền hoặc điểm lõi.";
            case "shop_legacy_inventory":
                return "Chỉ thao tác kho/vật tư ở lớp legacy. Không can thiệp trực tiếp tài sản lõi nền tảng.";
            case "shop_legacy_accounts":
                return "Chỉ thao tác tài khoản/nhân sự ở lớp legacy. Không can thiệp trực tiếp tài sản lõi nền tảng.";
            case "shop_legacy_org":
                return "Chỉ thao tác cơ cấu chi nhánh-ngành-phòng ban ở lớp legacy. Không can thiệp tài sản lõi.";
            case "shop_legacy_training":
                return "Chỉ thao tác học viên/giảng viên ở lớp legacy. Không can thiệp tài sản lõi.";
            case "shop_legacy_support":
                return "Chỉ thao tác thông báo/tư vấn ở lớp legacy. Không can thiệp tài sản lõi.";
            case "home_gianhang_space":
                return "Khối này cấp quyền duyệt mở không gian /gianhang của tài khoản Home theo tab duyệt được cấp.";
            case "shop_email":
                return "Chỉ thay đổi nội dung vận hành hiển thị hoặc mẫu email; không đụng tài sản lõi.";
            case "daugia_workspace":
                return "Chỉ được dùng bộ công cụ đấu giá thuộc đúng không gian đấu giá; không can thiệp vào các khối admin khác.";
            case "event_workspace":
                return "Chỉ được dùng bộ công cụ sự kiện thuộc đúng không gian sự kiện; không can thiệp vào các khối admin khác.";
            case "home_content":
                return "Chỉ chỉnh sửa nội dung văn bản hiển thị; không quản lý menu, bài viết, banner tổng quan và không đụng tài sản lõi.";
            case "home_posts":
            case "home_menu":
            case "home_banner":
                return "Chỉ thao tác đúng tab tổng quan website đã được cấp; không tự động bao gồm các tab nội dung tổng quan còn lại.";
            case "home_config":
                return "Được cấu hình lõi hệ Home theo quyền home_config_admin; cần cẩn trọng vì có thể ảnh hưởng hiển thị toàn hệ thống.";
            case "notifications":
            case "consulting":
                return "Chỉ xử lý thông tin, không phát sinh quyền thao tác tài sản lõi.";
            case "system_products":
            case "issue_cards":
            case "core_assets":
            case "token_wallet":
                return "Khối này thuộc tài sản lõi. Chỉ Super Admin mới được truy cập và thao tác.";
            default:
                return isSuperAdmin
                    ? "Được thao tác theo tab đang cấp. Các thay đổi nhạy cảm vẫn cần kiểm soát và lưu vết."
                    : "Chỉ thao tác trong phạm vi tab đã cấp. Các thay đổi tài sản lõi vẫn do Super Admin kiểm soát.";
        }
    }

    public static string ResolveFeatureUrl(dbDataContext db, string taikhoan, string key)
    {
        switch ((key ?? "").Trim().ToLowerInvariant())
        {
            case "admin_accounts":
                return ResolveAdminAccountManagementUrl(db, taikhoan);
            case "home_accounts":
                return ResolveHomeAccountManagementUrl(db, taikhoan);
            case "home_point_approval":
                return ResolveHomePointApprovalUrl(db, taikhoan);
            case "shop_accounts":
                return ResolveShopAccountManagementUrl(db, taikhoan);
            default:
                AdminFeatureRegistry_cl.FeatureDefinition definition = AdminFeatureRegistry_cl.Get(key);
                return definition == null ? "" : (definition.DefaultUrl ?? "");
        }
    }

    private static void AddAdminHomeTab(
        List<AdminHomeTabInfo> items,
        HashSet<string> keys,
        HashSet<string> accessGroups,
        dbDataContext db,
        string taikhoan,
        string key)
    {
        AdminFeatureRegistry_cl.FeatureDefinition definition = AdminFeatureRegistry_cl.Get(key);
        if (definition == null || !keys.Add(definition.Key))
            return;

        string url = ResolveFeatureUrl(db, taikhoan, definition.Key);
        if (string.IsNullOrWhiteSpace(url))
            return;

        string accessGroup = AdminFeatureRegistry_cl.ResolveAccessGroupKey(definition.Key);
        if (!string.IsNullOrWhiteSpace(accessGroup) && !accessGroups.Add(accessGroup))
            return;

        bool isSuperAdmin = IsSuperAdmin(taikhoan);
        items.Add(new AdminHomeTabInfo
        {
            Key = definition.Key,
            Title = definition.Title,
            Meaning = definition.Meaning,
            ActionSummary = GetAdminHomeActionSummary(definition.Key),
            GuardrailSummary = GetAdminHomeGuardrailSummary(definition.Key, isSuperAdmin),
            ScopeLabel = definition.ScopeLabel,
            ScopeMeaning = definition.ScopeMeaning,
            Url = url,
            SortOrder = definition.SortOrder
        });
    }

    public static IList<AdminHomeTabInfo> BuildAdminHomeTabs(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        List<AdminHomeTabInfo> items = new List<AdminHomeTabInfo>();
        HashSet<string> keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> accessGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        bool isSuperAdmin = IsSuperAdmin(taikhoan);

        bool showLegacySystemProductTabs = !CompanyShop_cl.HideLegacyAdminSystemProduct();

        if (isSuperAdmin)
        {
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_dashboard");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_accounts");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_otp");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_feedback");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_company_shop_sync");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_reindex_baiviet");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_accounts");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_point_approval");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "tier_reference");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_workspace");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_invoices");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_customers");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_content");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_finance");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_inventory");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_accounts");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_org");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_training");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_support");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_accounts");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_points");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_level2");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_gianhang_space");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_partner");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_email");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "daugia_workspace");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "event_workspace");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_content");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_bds_linked");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_posts");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_menu");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_banner");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_config");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "notifications");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "consulting");
            if (showLegacySystemProductTabs)
            {
                AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "system_products");
                AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "issue_cards");
            }
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "core_assets");
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "token_wallet");
            return items.OrderBy(item => item.SortOrder).ToList();
        }

        if (CanAccessAdminWorkspace(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_dashboard");

        if (CanManageAdminAccounts(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_accounts");

        if (CanManageAdminOtp(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_otp");

        if (CanManageAdminFeedback(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_feedback");

        if (CanManageAdminCompanyShopSync(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_company_shop_sync");

        if (CanManageAdminReindexBaiViet(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "admin_reindex_baiviet");

        if (CanManageAdminNotification(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "notifications");

        if (CanManageAdminConsulting(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "consulting");

        if (CanManageAdminTokenWallet(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "token_wallet");

        if (CanManageHomeAccounts(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_accounts");

        if (CanReviewHomePointRequests(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_point_approval");

        if (CanViewTierReference(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "tier_reference");

        if (CanManageShopAccounts(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_accounts");

        bool canApproveHomeGianHangSpace = CanApproveHomeGianHangSpace(db, taikhoan);
        bool canApproveShopPartnerRegistration = CanApproveShopPartnerRegistration(db, taikhoan);
        bool useGranularShopApprovalTabs = UsesGranularShopApprovalMode(db, taikhoan);

        if (CanManageShopAccounts(db, taikhoan)
            || CanReviewShopPointRequests(db, taikhoan)
            || canApproveShopPartnerRegistration
            || canApproveHomeGianHangSpace
            || CanManageShopOperations(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_workspace");

        if (CanManageShopLegacyInvoices(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_invoices");

        if (CanManageShopLegacyCustomers(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_customers");

        if (CanManageShopLegacyContent(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_content");

        if (CanManageShopLegacyFinance(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_finance");

        if (CanManageShopLegacyInventory(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_inventory");

        if (CanManageShopLegacyAccounts(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_accounts");

        if (CanManageShopLegacyOrg(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_org");

        if (CanManageShopLegacyTraining(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_training");

        if (CanManageShopLegacySupport(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_legacy_support");

        if (CanManageShopOperations(db, taikhoan))
        {
            // Keep workspace entry available for shop operators in both legacy and granular modes.
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_workspace");
        }

        if (CanReviewShopPointRequests(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_points");

        if (canApproveHomeGianHangSpace && (useGranularShopApprovalTabs || !canApproveShopPartnerRegistration))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_gianhang_space");

        if (canApproveShopPartnerRegistration)
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_partner");

        if (CanApproveShopLevel2(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_level2");

        if (CanManageShopOperations(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "shop_email");

        if (CanManageHomeConfig(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_config");

        if (CanManageHomeContent(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_content");

        if (CanManageHomeBdsLinked(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_bds_linked");

        if (CanManageHomePosts(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_posts");

        if (CanManageHomeMenu(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_menu");

        if (CanManageHomeBanner(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "home_banner");

        if (DauGiaPolicy_cl.CanAccessAdmin(db, taikhoan, permissionRaw))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "daugia_workspace");

        if (EventPolicy_cl.CanViewAdminWorkspace(db, taikhoan))
            AddAdminHomeTab(items, keys, accessGroups, db, taikhoan, "event_workspace");

        return items.OrderBy(item => item.SortOrder).ToList();
    }

    public static HashSet<string> BuildAdminHomeTabKeySet(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        return new HashSet<string>(
            BuildAdminHomeTabs(db, taikhoan, phanloai, permissionRaw)
                .Select(item => (item.Key ?? "").Trim())
                .Where(item => item != ""),
            StringComparer.OrdinalIgnoreCase);
    }

    public static HashSet<string> BuildAdminHomeAccessGroupSet(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        return new HashSet<string>(
            BuildAdminHomeTabs(db, taikhoan, phanloai, permissionRaw)
                .Select(item => AdminFeatureRegistry_cl.ResolveAccessGroupKey(item.Key))
                .Where(item => !string.IsNullOrWhiteSpace(item)),
            StringComparer.OrdinalIgnoreCase);
    }

    public static IList<string> BuildAdminPolicyNotes(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        List<string> notes = new List<string>();

        if (IsSuperAdmin(taikhoan))
        {
            notes.Add("Bạn là Super Admin: được duy nhất quyền thao tác trực tiếp với tiền, quyền, điểm và tài sản lõi.");
            notes.Add("Bạn có thể tạo tài khoản admin mới, gán 5 vai trò vận hành và thay đổi cấu trúc phân quyền toàn hệ thống.");
            notes.Add("Mọi thao tác duyệt, phân phối hoặc can thiệp tài sản lõi nên được xem như nghiệp vụ cấp ngân hàng và cần lưu vết đầy đủ.");
            return notes;
        }

        notes.Add("Bạn chỉ được xem tiền, quyền và điểm. Không được cộng trừ trực tiếp tài sản lõi.");

        if (CanReviewCustomerPointRequests(db, taikhoan))
            notes.Add("Bạn được duyệt các yêu cầu điểm của tầng khách hàng Home theo đúng scope đã cấp.");
        if (CanReviewDevelopmentPointRequests(db, taikhoan))
            notes.Add("Bạn được duyệt các yêu cầu điểm của tầng cộng tác phát triển theo đúng scope đã cấp.");
        if (CanReviewEcosystemPointRequests(db, taikhoan))
            notes.Add("Bạn được duyệt các yêu cầu điểm của tầng đồng hành hệ sinh thái theo đúng scope đã cấp.");
        if (CanReviewShopPointRequests(db, taikhoan))
            notes.Add("Bạn được duyệt các yêu cầu điểm hoặc nghiệp vụ phát sinh của gian hàng đối tác theo đúng scope đã cấp.");
        if (CanManageHomeConfig(db, taikhoan))
            notes.Add("Bạn được cấu hình trang chủ (home_config) theo phạm vi cấp quyền, tách biệt với nhóm chỉnh nội dung văn bản.");
        if (CanManageHomeContent(db, taikhoan))
            notes.Add("Bạn chỉ được sửa nội dung văn bản hiển thị trên Ahasale.vn, không quản lý menu, banner, bài viết tổng quan và không can thiệp tài sản lõi.");
        if (CanManageHomePosts(db, taikhoan))
            notes.Add("Bạn được quản trị tab Bài viết tổng quan website theo quyền home_posts_admin, tách biệt khỏi các tab menu/banner.");
        if (CanManageHomeMenu(db, taikhoan))
            notes.Add("Bạn được quản trị tab Menu tổng quan website theo quyền home_menu_admin, không tự động có quyền tab bài viết/banner.");
        if (CanManageHomeBanner(db, taikhoan))
            notes.Add("Bạn được quản trị tab Banner tổng quan website theo quyền home_banner_admin, tách biệt khỏi các tab menu/bài viết.");
        if (notes.Count == 1)
            notes.Add("Tài khoản này chỉ thao tác được trong các tab đã hiện bên dưới. Nếu cần thêm quyền phải do Super Admin cấp.");

        return notes;
    }

    public static IList<AdminObjectGroupInfo> BuildAdminObjectGroups(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        bool isSuperAdmin = IsSuperAdmin(taikhoan);
        var items = new List<AdminObjectGroupInfo>();

        Action<string, string, string, string, bool, string, string> add = (key, title, scopeLabel, meaning, isActive, statusLabel, statusHint) =>
        {
            items.Add(new AdminObjectGroupInfo
            {
                Key = key,
                Title = title,
                ScopeLabel = scopeLabel,
                Meaning = meaning,
                IsActive = isActive,
                StatusLabel = statusLabel,
                StatusHint = statusHint
            });
        };

        add(
            "home_customer",
            "Tài khoản tầng khách hàng",
            "Hệ Home",
            "Phụ trách hồ sơ khách hàng Home và các yêu cầu điểm phát sinh từ tầng khách hàng.",
            isSuperAdmin || CanReviewCustomerPointRequests(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanReviewCustomerPointRequests(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có quyền giám sát và can thiệp nghiệp vụ khi cần."
                : (CanReviewCustomerPointRequests(db, taikhoan)
                    ? "Được duyệt phiếu điểm của tầng khách hàng theo đúng rule hệ thống."
                    : "Không hiện tab duyệt điểm khách hàng cho tài khoản này."));

        add(
            "home_development",
            "Tài khoản tầng cộng tác phát triển",
            "Hệ Home",
            "Phụ trách hồ sơ cộng tác phát triển và phiếu điểm của hành vi lao động.",
            isSuperAdmin || CanReviewDevelopmentPointRequests(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanReviewDevelopmentPointRequests(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có thể theo dõi toàn bộ luồng duyệt điểm cộng tác phát triển."
                : (CanReviewDevelopmentPointRequests(db, taikhoan)
                    ? "Được duyệt phiếu điểm đúng phạm vi cộng tác phát triển."
                    : "Tài khoản này không được duyệt điểm cộng tác phát triển."));

        add(
            "home_ecosystem",
            "Tài khoản tầng đồng hành hệ sinh thái",
            "Hệ Home",
            "Phụ trách hồ sơ đồng hành hệ sinh thái và các yêu cầu điểm thuộc luồng gắn kết.",
            isSuperAdmin || CanReviewEcosystemPointRequests(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanReviewEcosystemPointRequests(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có thể theo dõi toàn bộ luồng đồng hành hệ sinh thái."
                : (CanReviewEcosystemPointRequests(db, taikhoan)
                    ? "Được duyệt phiếu điểm của tầng đồng hành hệ sinh thái."
                    : "Không hiện tab duyệt điểm đồng hành hệ sinh thái cho tài khoản này."));

        bool canOperateShopPartnerGroup = isSuperAdmin
            || CanManageShopAccounts(db, taikhoan)
            || CanReviewShopPointRequests(db, taikhoan)
            || CanApproveHomeGianHangSpace(db, taikhoan)
            || CanApproveShopPartnerRegistration(db, taikhoan);

        add(
            "shop_partner",
            "Tài khoản gian hàng đối tác",
            "Hệ Shop",
            "Phụ trách tài khoản shop, nghiệp vụ shop và các yêu cầu điểm/nghiệp vụ của gian hàng đối tác.",
            canOperateShopPartnerGroup,
            isSuperAdmin ? "Super Admin giám sát" : (canOperateShopPartnerGroup ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin theo dõi toàn bộ luồng shop và level 2."
                : (canOperateShopPartnerGroup
                    ? "Được vận hành shop theo quyền được cấp (có thể là quyền tổng q2_5 hoặc quyền granular từng tab duyệt)."
                    : "Tài khoản này không được quản trị gian hàng đối tác."));

        add(
            "home_config",
            "Tài khoản cấu hình trang chủ",
            "Nội dung website",
            "Phụ trách cài đặt trang chủ và cấu hình hệ thống hiển thị lõi.",
            isSuperAdmin || CanManageHomeConfig(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanManageHomeConfig(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có thể cấu hình toàn bộ trang chủ và các tham số hệ thống liên quan."
                : (CanManageHomeConfig(db, taikhoan)
                    ? "Được cấu hình trang chủ theo quyền home_config_admin; không tự thao tác tài sản lõi."
                    : "Tài khoản này không có quyền cấu hình trang chủ."));

        add(
            "home_content",
            "Tài khoản quản lý nội dung web Ahasale.vn",
            "Nội dung văn bản",
            "Phụ trách nội dung văn bản hiển thị trên Ahasale.vn; không quản lý menu, banner hoặc bài viết tổng quan.",
            isSuperAdmin || CanManageHomeContent(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanManageHomeContent(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có thể chỉnh trực tiếp toàn bộ nội dung văn bản và đồng thời quản lý menu, bài viết, banner tổng quan của website."
                : (CanManageHomeContent(db, taikhoan)
                    ? "Được chỉnh nội dung văn bản hiển thị; không quản lý menu, bài viết, banner tổng quan và không đụng tiền, quyền, điểm."
                    : "Tài khoản này không có quyền sửa nội dung văn bản web."));

        add(
            "home_posts",
            "Tài khoản quản lý bài viết tổng quan",
            "Tổng quan website",
            "Phụ trách quản trị bài viết tổng quan của website.",
            isSuperAdmin || CanManageHomePosts(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanManageHomePosts(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có thể chỉnh toàn bộ bài viết tổng quan website."
                : (CanManageHomePosts(db, taikhoan)
                    ? "Được quản trị tab bài viết theo quyền home_posts_admin."
                    : "Tài khoản này không có quyền quản trị tab bài viết tổng quan."));

        add(
            "home_menu",
            "Tài khoản quản lý menu tổng quan",
            "Tổng quan website",
            "Phụ trách cấu hình menu và điều hướng tổng quan của website.",
            isSuperAdmin || CanManageHomeMenu(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanManageHomeMenu(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có thể chỉnh toàn bộ menu tổng quan website."
                : (CanManageHomeMenu(db, taikhoan)
                    ? "Được quản trị tab menu theo quyền home_menu_admin."
                    : "Tài khoản này không có quyền quản trị tab menu tổng quan."));

        add(
            "home_banner",
            "Tài khoản quản lý banner tổng quan",
            "Tổng quan website",
            "Phụ trách banner và khối hiển thị chiến dịch tổng quan của website.",
            isSuperAdmin || CanManageHomeBanner(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : (CanManageHomeBanner(db, taikhoan) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin có thể chỉnh toàn bộ banner tổng quan website."
                : (CanManageHomeBanner(db, taikhoan)
                    ? "Được quản trị tab banner theo quyền home_banner_admin."
                    : "Tài khoản này không có quyền quản trị tab banner tổng quan."));

        return items;
    }

    public static bool CanAccessHomeTierData(dbDataContext db, string taikhoan, int targetTier)
    {
        if (IsSuperAdmin(taikhoan))
            return true;

        if (targetTier >= TierHome_cl.Tier3)
            return PermissionProfile_cl.HasPermission(db, taikhoan, RoleHomeEcosystem);

        if (targetTier >= TierHome_cl.Tier2)
            return PermissionProfile_cl.HasPermission(db, taikhoan, RoleHomeDevelopment);

        return PermissionProfile_cl.HasPermission(db, taikhoan, RoleHomeCustomer);
    }

    public static string ResolveHomePointApprovalTab(dbDataContext db, string taikhoan)
    {
        if (IsSuperAdmin(taikhoan))
            return "";

        if (CanReviewDevelopmentPointRequests(db, taikhoan))
            return "lao-dong";

        if (CanReviewEcosystemPointRequests(db, taikhoan))
            return "gan-ket";

        if (CanReviewCustomerPointRequests(db, taikhoan))
            return "uu-dai";

        return "";
    }

    public static string ResolveHomePointApprovalUrl(dbDataContext db, string taikhoan)
    {
        string tab = ResolveHomePointApprovalTab(db, taikhoan);
        if (string.IsNullOrWhiteSpace(tab))
            return "/admin/duyet-yeu-cau-len-cap.aspx";

        return "/admin/lich-su-chuyen-diem/default.aspx?tab=" + HttpUtility.UrlEncode(tab);
    }

    public static string ResolvePrimaryWorkspaceUrl(dbDataContext db, string taikhoan)
    {
        if (string.IsNullOrWhiteSpace(taikhoan))
            return "/admin/login.aspx";

        if (IsSuperAdmin(taikhoan))
            return "/admin/default.aspx";

        string presetKey = ResolveScopedAdminPresetKey(db, taikhoan);
        if (string.Equals(presetKey, PresetHomeCustomer, StringComparison.OrdinalIgnoreCase)
            || string.Equals(presetKey, PresetHomeDevelopment, StringComparison.OrdinalIgnoreCase)
            || string.Equals(presetKey, PresetHomeEcosystem, StringComparison.OrdinalIgnoreCase))
            return ResolveHomeAccountManagementUrl(db, taikhoan);

        if (string.Equals(presetKey, PresetShopPartner, StringComparison.OrdinalIgnoreCase))
            return ResolveShopAccountManagementUrl(db, taikhoan);

        if (string.Equals(presetKey, PresetHomeContent, StringComparison.OrdinalIgnoreCase))
            return "/admin/quan-ly-noi-dung-home/default.aspx";

        string homePointApprovalUrl = ResolveHomePointApprovalUrl(db, taikhoan);
        if (!string.IsNullOrWhiteSpace(homePointApprovalUrl) && homePointApprovalUrl != "/admin/duyet-yeu-cau-len-cap.aspx")
            return homePointApprovalUrl;

        if (CanManageHomeAccounts(db, taikhoan))
            return ResolveHomeAccountManagementUrl(db, taikhoan);

        if (CanManageShopAccounts(db, taikhoan))
            return ResolveShopAccountManagementUrl(db, taikhoan);

        if (CanManageHomeConfig(db, taikhoan))
            return "/admin/cai-dat-trang-chu/default.aspx";

        if (CanManageHomeContent(db, taikhoan))
            return "/admin/quan-ly-noi-dung-home/default.aspx";

        if (CanManageHomeBdsLinked(db, taikhoan))
            return "/admin/quan-ly-noi-dung-home/bds-lien-ket-tin.aspx";

        if (CanManageHomePosts(db, taikhoan))
            return "/admin/quan-ly-bai-viet/default.aspx";

        if (CanManageHomeMenu(db, taikhoan))
            return "/admin/quan-ly-menu/default.aspx";

        if (CanManageHomeBanner(db, taikhoan))
            return "/admin/quan-ly-banner/default.aspx";

        return "/admin/default.aspx";
    }

    public static string ResolveLandingUrl(dbDataContext db, string taikhoan)
    {
        if (string.IsNullOrWhiteSpace(taikhoan))
            return "/admin/login.aspx";

        taikhoan_tb account = db != null ? db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taikhoan) : null;
        if (account == null)
            return "/admin/default.aspx";

        HttpRequest request = HttpContext.Current != null ? HttpContext.Current.Request : null;
        string landing = AdminManagementSpace_cl.ResolveCurrentLandingUrl(db, account, request);
        if (string.IsNullOrWhiteSpace(landing))
            return "/admin/default.aspx";
        return landing;
    }

    private static void RedirectDenied(string message, string fallbackUrl)
    {
        HttpContext context = HttpContext.Current;
        if (context == null)
            return;

        string safeFallbackUrl = (fallbackUrl ?? "").Trim();
        if (string.IsNullOrWhiteSpace(safeFallbackUrl) || string.Equals(safeFallbackUrl, "/admin/default.aspx", StringComparison.OrdinalIgnoreCase))
        {
            string tk = GetCurrentAdminUser();
            if (!string.IsNullOrWhiteSpace(tk))
            {
                try
                {
                    using (dbDataContext db = new dbDataContext())
                    {
                        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                        if (account != null)
                        {
                            string landing = AdminManagementSpace_cl.ResolveCurrentLandingUrl(db, account, context.Request);
                            if (!string.IsNullOrWhiteSpace(landing))
                                safeFallbackUrl = landing;
                        }
                    }
                }
                catch
                {
                    // fallback to the provided default route
                }
            }
        }

        if (string.IsNullOrWhiteSpace(safeFallbackUrl))
            safeFallbackUrl = "/admin/default.aspx";

        context.Session["thongbao"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            message,
            "1800",
            "warning");
        context.Response.Redirect(safeFallbackUrl);
    }

    public static void RequireSuperAdmin()
    {
        check_login_cl.check_login_admin("none", "none");
        HttpRequest request = HttpContext.Current != null ? HttpContext.Current.Request : null;
        string currentFeatureKey = AdminRouteMap_cl.ResolveFeatureKey(request);
        if (!string.IsNullOrWhiteSpace(currentFeatureKey) && AdminAccessGuard_cl.CanCurrentAdminAccessFeature(currentFeatureKey))
            return;

        string tk = GetCurrentAdminUser();
        if (IsSuperAdmin(tk))
            return;

        RedirectDenied("Chỉ Super Admin mới được thao tác chức năng này.", "/admin/default.aspx");
    }

    public static void RequireHomePointApprovalAdmin()
    {
        check_login_cl.check_login_admin("none", "none");

        using (dbDataContext db = new dbDataContext())
        {
            string tk = GetCurrentAdminUser();
            if (CanReviewHomePointRequests(db, tk))
                return;
        }

        RedirectDenied("Bạn không có quyền duyệt điểm của các hồ sơ Home.", "/admin/default.aspx");
    }

    public static void RequireShopOperationsManager()
    {
        check_login_cl.check_login_admin("none", "none");

        using (dbDataContext db = new dbDataContext())
        {
            string tk = GetCurrentAdminUser();
            if (CanManageShopOperations(db, tk))
                return;
        }

        RedirectDenied("Bạn không có quyền thao tác nghiệp vụ của gian hàng đối tác.", "/admin/default.aspx");
    }

    public static void RequireContentManager()
    {
        check_login_cl.check_login_admin("none", "none");
        HttpRequest request = HttpContext.Current != null ? HttpContext.Current.Request : null;
        string currentFeatureKey = AdminRouteMap_cl.ResolveFeatureKey(request);
        if (!string.IsNullOrWhiteSpace(currentFeatureKey) && AdminAccessGuard_cl.CanCurrentAdminAccessFeature(currentFeatureKey))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            string tk = GetCurrentAdminUser();
            if (CanManageHomeContent(db, tk))
                return;
        }

        RedirectDenied("Bạn không có quyền quản lý nội dung hiển thị trang Home.", "/admin/default.aspx");
    }

    public static bool EnsureMasterRouteAccess(Page page)
    {
        if (page == null || page.Request == null)
            return true;

        HttpRequest request = page.Request;
        string path = (request.AppRelativeCurrentExecutionFilePath ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("~/admin/", StringComparison.OrdinalIgnoreCase))
            return true;

        string tk = GetCurrentAdminUser();
        if (string.IsNullOrWhiteSpace(tk))
            return true;

        using (dbDataContext db = new dbDataContext())
        {
            if (IsSuperAdmin(tk))
                return true;

            string fallbackUrl = ResolveLandingUrl(db, tk);
            string denialMessage = null;

            if (IsHomeContentRoute(path))
            {
                if (CanManageHomeContent(db, tk))
                    return true;

                denialMessage = "Bạn chỉ được quản lý nội dung hiển thị trên trang Home.";
            }
            else if (IsHomeConfigRoute(path))
            {
                if (CanManageHomeConfig(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền cấu hình trang chủ.";
            }
            else if (IsHomePostsRoute(path))
            {
                if (CanManageHomePosts(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền quản trị tab Bài viết tổng quan website.";
            }
            else if (IsHomeMenuRoute(path))
            {
                if (CanManageHomeMenu(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền quản trị tab Menu tổng quan website.";
            }
            else if (IsHomeBannerRoute(path))
            {
                if (CanManageHomeBanner(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền quản trị tab Banner tổng quan website.";
            }
            else if (IsHomePointRoute(path, request))
            {
                if (CanAccessHomePointRoute(db, tk, request))
                    return true;

                denialMessage = "Bạn không có quyền duyệt điểm hoặc hồ sơ hành vi của tầng Home này.";
            }
            else if (IsShopOperationRoute(path))
            {
                if (CanManageShopOperations(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền thao tác nghiệp vụ của gian hàng đối tác.";
            }
            else if (IsAccountManagementRoute(path))
            {
                if (CanAccessAccountManagementRoute(db, tk, request))
                    return true;
                denialMessage = BuildAccountManagementDeniedMessage(request);
            }
            else if (IsAdminOtpRoute(path))
            {
                if (CanManageAdminOtp(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền quản lý OTP trong không gian Quản trị admin.";
            }
            else if (IsAdminTokenWalletRoute(path))
            {
                if (CanManageAdminTokenWallet(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền truy cập Ví token điểm trong không gian Quản trị admin.";
            }
            else if (IsAdminFeedbackRoute(path))
            {
                if (CanManageAdminFeedback(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền quản lý góp ý trong không gian Quản trị admin.";
            }
            else if (IsAdminNotificationRoute(path))
            {
                if (CanManageAdminNotification(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền quản lý thông báo trong không gian Quản trị admin.";
            }
            else if (IsAdminConsultingRoute(path))
            {
                if (CanManageAdminConsulting(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền quản lý yêu cầu tư vấn trong không gian Quản trị admin.";
            }
            else if (IsAdminCompanyShopSyncRoute(path))
            {
                if (CanManageAdminCompanyShopSync(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền dùng công cụ Đồng bộ shop công ty trong không gian Quản trị admin.";
            }
            else if (IsAdminReindexBaiVietRoute(path))
            {
                if (CanManageAdminReindexBaiViet(db, tk))
                    return true;

                denialMessage = "Bạn không có quyền chạy Reindex bài viết trong không gian Quản trị admin.";
            }
            else if (IsRootOnlyRoute(path))
            {
                denialMessage = "Chỉ Super Admin mới được thao tác chức năng này.";
            }

            if (string.IsNullOrWhiteSpace(denialMessage))
                return true;

            RedirectDenied(denialMessage, fallbackUrl);
            HttpContext current = HttpContext.Current;
            if (current != null && current.ApplicationInstance != null)
                current.ApplicationInstance.CompleteRequest();
            return false;
        }
    }

    private static bool IsHomeContentRoute(string path)
    {
        return path.StartsWith("~/admin/quan-ly-noi-dung-home/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHomeConfigRoute(string path)
    {
        return path.StartsWith("~/admin/cai-dat-trang-chu/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHomePostsRoute(string path)
    {
        return path.StartsWith("~/admin/quan-ly-bai-viet/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHomeMenuRoute(string path)
    {
        return path.StartsWith("~/admin/quan-ly-menu/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHomeBannerRoute(string path)
    {
        return path.StartsWith("~/admin/quan-ly-banner/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHomePointRoute(string path, HttpRequest request)
    {
        if (path.StartsWith("~/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase))
            return true;

        return string.Equals(path, "~/admin/duyet-yeu-cau-len-cap.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/motacapbac.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool CanAccessHomePointRoute(dbDataContext db, string taikhoan, HttpRequest request)
    {
        string path = (request.AppRelativeCurrentExecutionFilePath ?? "").Trim().ToLowerInvariant();
        if (!path.StartsWith("~/admin/lich-su-chuyen-diem/", StringComparison.OrdinalIgnoreCase))
            return CanReviewHomePointRequests(db, taikhoan);

        string tab = AdminDataScope_cl.NormalizeTransferTabExtended(request.QueryString["tab"]);
        if (string.IsNullOrWhiteSpace(tab))
            return CanReviewHomePointRequests(db, taikhoan)
                || CanReviewShopPointRequests(db, taikhoan)
                || CanManageShopAccounts(db, taikhoan);

        switch (tab)
        {
            case AdminDataScope_cl.TransferTabCustomer:
                return CanReviewCustomerPointRequests(db, taikhoan);
            case AdminDataScope_cl.TransferTabDevelopment:
                return CanReviewDevelopmentPointRequests(db, taikhoan);
            case AdminDataScope_cl.TransferTabEcosystem:
                return CanReviewEcosystemPointRequests(db, taikhoan);
            case AdminDataScope_cl.TransferTabShopOnly:
                return CanReviewShopPointRequests(db, taikhoan)
                    || CanManageShopAccounts(db, taikhoan);
            case AdminDataScope_cl.TransferTabCore:
                return CanAccessTransferHistory(db, taikhoan);
            default:
                return CanReviewHomePointRequests(db, taikhoan);
        }
    }

    private static bool IsShopOperationRoute(string path)
    {
        return path.StartsWith("~/admin/quan-ly-email-shop/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminOtpRoute(string path)
    {
        return path.StartsWith("~/admin/otp/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminTokenWalletRoute(string path)
    {
        return path.StartsWith("~/admin/vi-token-diem/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/api/usdt-bridge-credit.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminFeedbackRoute(string path)
    {
        return path.StartsWith("~/admin/quan-ly-gop-y/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminNotificationRoute(string path)
    {
        return path.StartsWith("~/admin/quan-ly-thong-bao/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminConsultingRoute(string path)
    {
        return path.StartsWith("~/admin/yeu-cau-tu-van/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminCompanyShopSyncRoute(string path)
    {
        return path.StartsWith("~/admin/tools/company-shop-sync.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdminReindexBaiVietRoute(string path)
    {
        return path.StartsWith("~/admin/tools/reindex-baiviet.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAccountManagementRoute(string path)
    {
        if (path.StartsWith("~/admin/quan-ly-tai-khoan/", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static string NormalizeAccountManagementRoleFilter(string raw)
    {
        string key = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(key))
            return "";

        if (string.Equals(key, "super_admin", StringComparison.OrdinalIgnoreCase))
            return "super_admin";

        ScopedAdminPresetInfo preset = GetScopedAdminPreset(key);
        return preset != null ? preset.Key : "";
    }

    private static string NormalizeAccountManagementScope(string raw)
    {
        string value = AdminDataScope_cl.NormalizeAccountScope(raw);
        if (string.IsNullOrWhiteSpace(value))
            return "";

        return value;
    }

    private static bool CanAccessAccountManagementRoleFilter(dbDataContext db, string taikhoan, string scope, string filterRole)
    {
        string normalizedScope = NormalizeAccountManagementScope(scope);
        string normalizedRole = NormalizeAccountManagementRoleFilter(filterRole);
        if (string.IsNullOrWhiteSpace(normalizedRole))
            return true;

        if (IsSuperAdmin(taikhoan))
            return true;

        string presetKey = ResolveScopedAdminPresetKey(db, taikhoan);
        if (string.IsNullOrWhiteSpace(presetKey))
            return false;

        if (string.Equals(normalizedScope, AdminDataScope_cl.AccountScopeHome, StringComparison.OrdinalIgnoreCase))
        {
            if (!(string.Equals(presetKey, PresetHomeCustomer, StringComparison.OrdinalIgnoreCase)
                || string.Equals(presetKey, PresetHomeDevelopment, StringComparison.OrdinalIgnoreCase)
                || string.Equals(presetKey, PresetHomeEcosystem, StringComparison.OrdinalIgnoreCase)))
                return false;

            return string.Equals(normalizedRole, presetKey, StringComparison.OrdinalIgnoreCase);
        }

        if (string.Equals(normalizedScope, AdminDataScope_cl.AccountScopeShop, StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(normalizedRole, PresetShopPartner, StringComparison.OrdinalIgnoreCase)
                && string.Equals(presetKey, PresetShopPartner, StringComparison.OrdinalIgnoreCase);
        }

        if (string.Equals(normalizedScope, AdminDataScope_cl.AccountScopeAdmin, StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(normalizedRole, PresetHomeContent, StringComparison.OrdinalIgnoreCase)
                && string.Equals(presetKey, PresetHomeContent, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static bool CanAccessAccountManagementRoute(dbDataContext db, string taikhoan, HttpRequest request)
    {
        string scope = NormalizeAccountManagementScope(
            AdminDataScope_cl.ResolveEffectiveAccountScope(
                request.QueryString["scope"],
                request.QueryString["fscope"]));
        string filterRole = request.QueryString["frole"];

        if (string.IsNullOrWhiteSpace(scope))
            return CanManageHomeAccounts(db, taikhoan)
                || CanManageShopAccounts(db, taikhoan)
                || CanManageAdminAccounts(db, taikhoan);

        if (scope == AdminDataScope_cl.AccountScopeHome)
            return CanManageHomeAccounts(db, taikhoan)
                && CanAccessAccountManagementRoleFilter(db, taikhoan, scope, filterRole);

        if (scope == AdminDataScope_cl.AccountScopeShop)
            return CanManageShopAccounts(db, taikhoan)
                && CanAccessAccountManagementRoleFilter(db, taikhoan, scope, filterRole);

        if (scope == AdminDataScope_cl.AccountScopeAdmin)
            return CanManageAdminAccounts(db, taikhoan)
                && CanAccessAccountManagementRoleFilter(db, taikhoan, scope, filterRole);

        return false;
    }

    private static string BuildAccountManagementDeniedMessage(HttpRequest request)
    {
        string scope = NormalizeAccountManagementScope(
            AdminDataScope_cl.ResolveEffectiveAccountScope(
                request.QueryString["scope"],
                request.QueryString["fscope"]));
        switch (scope)
        {
            case AdminDataScope_cl.AccountScopeAdmin:
                return "Bạn không có quyền truy cập khu quản trị tài khoản admin.";
            case AdminDataScope_cl.AccountScopeHome:
                return "Bạn không có quyền truy cập khu quản trị tài khoản Home của vai trò này.";
            case AdminDataScope_cl.AccountScopeShop:
                return "Bạn không có quyền truy cập khu quản trị tài khoản gian hàng đối tác.";
            default:
                return "Bạn không có quyền truy cập khu quản trị tài khoản này.";
        }
    }

    private static bool IsRootOnlyRoute(string path)
    {
        return path.StartsWith("~/admin/he-thong-san-pham/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/otp/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/vi-token-diem/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-gop-y/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-thong-bao/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/yeu-cau-tu-van/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/tools/", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/phat-hanh-the.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/duyet-nang-cap-level2.aspx", StringComparison.OrdinalIgnoreCase);
    }
}
