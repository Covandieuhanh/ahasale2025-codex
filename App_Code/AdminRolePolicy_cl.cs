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
    public const string RoleLegacyConsumerAsset = PermissionProfile_cl.HoSoTieuDung;
    public const string RoleAdminOtp = "admin_otp";
    public const string RoleAdminTokenWallet = "admin_token_wallet";
    public const string RoleAdminFeedback = "admin_feedback";
    public const string RoleAdminNotification = "admin_notification";
    public const string RoleAdminConsulting = "admin_consulting";
    public const string RoleAdminCompanyShopSync = "admin_company_shop_sync";
    public const string RoleAdminReindexBaiViet = "admin_reindex_baiviet";
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

    public static bool CanManageHomeContent(dbDataContext db, string taikhoan)
    {
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
            case "home_gianhang_space":
                return "Khối này cấp quyền vào không gian /gianhang của tài khoản Home. Chỉ Super Admin mới được thao tác.";
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
                return isSuperAdmin
                    ? "Đây là khối tổng quan website. Chỉ Super Admin mới được thay đổi để tránh lệch cấu trúc hiển thị toàn hệ thống."
                    : "Chỉ Super Admin mới được thay đổi menu, bài viết và banner tổng quan của website.";
            case "home_config":
                return isSuperAdmin
                    ? "Được cấu hình lõi hệ Home. Cần cẩn trọng vì có thể ảnh hưởng hiển thị toàn hệ thống."
                    : "Chỉ Super Admin mới được thay đổi cấu hình lõi của Home.";
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

    public static IList<AdminHomeTabInfo> BuildAdminHomeTabs(dbDataContext db, string taikhoan, string phanloai, string permissionRaw)
    {
        List<AdminHomeTabInfo> items = new List<AdminHomeTabInfo>();
        HashSet<string> keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        bool isSuperAdmin = IsSuperAdmin(taikhoan);

        Action<string, string, string, string, string, string, int> add = (key, title, meaning, scopeLabel, scopeMeaning, url, sortOrder) =>
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(url) || !keys.Add(key))
                return;

            items.Add(new AdminHomeTabInfo
            {
                Key = key,
                Title = title,
                Meaning = meaning,
                ActionSummary = GetAdminHomeActionSummary(key),
                GuardrailSummary = GetAdminHomeGuardrailSummary(key, isSuperAdmin),
                ScopeLabel = scopeLabel,
                ScopeMeaning = scopeMeaning,
                Url = url,
                SortOrder = sortOrder
            });
        };

        bool showLegacySystemProductTabs = !CompanyShop_cl.HideLegacyAdminSystemProduct();

        if (isSuperAdmin)
        {
            add("admin_dashboard", "Trang chủ admin", "Landing tổng của khối quản trị admin.", "Cổng Admin", "Điểm vào chính của không gian quản trị admin.", "/admin/default.aspx", 5);
            add("admin_accounts", "Quản lý tài khoản admin", "Tạo tài khoản admin, gán 5 vai trò và kiểm soát đúng cổng vận hành.", "Cổng Admin", "Toàn bộ tài khoản admin và cấu trúc phân quyền nội bộ.", "/admin/quan-ly-tai-khoan/Default.aspx?scope=admin&fscope=admin", 10);
            add("admin_otp", "Quản lý OTP", "Quản trị cấu hình OTP, nhật ký gửi mã và hỗ trợ xác thực cho các tài khoản trong hệ.", "Cổng Admin", "Khối OTP nội bộ của hệ quản trị.", "/admin/otp/default.aspx", 15);
            add("admin_feedback", "Quản lý góp ý", "Theo dõi, lọc và xử lý các góp ý phát sinh gửi vào hệ quản trị.", "Cổng Admin", "Toàn bộ luồng góp ý của nền tảng.", "/admin/quan-ly-gop-y/default.aspx", 18);
            add("admin_company_shop_sync", "Đồng bộ shop công ty", "Đồng bộ, làm sạch và rà soát dữ liệu shop công ty dùng cho các tác vụ nội bộ.", "Cổng Admin", "Công cụ nội bộ dành cho vận hành admin.", "/admin/tools/company-shop-sync.aspx", 19);
            add("admin_reindex_baiviet", "Reindex bài viết", "Chạy lại chỉ mục nội dung để đồng bộ tìm kiếm và các feed bài viết trên website.", "Cổng Admin", "Công cụ vận hành chỉ mục nội dung.", "/admin/tools/reindex-baiviet.aspx", 19);
            add("home_accounts", "Quản lý tài khoản home", "Rà soát hồ sơ Home, tìm kiếm và xử lý dữ liệu người dùng cá nhân.", "Hệ Home", "Toàn bộ tài khoản Home trên các tầng khách hàng, cộng tác phát triển và đồng hành hệ sinh thái.", "/admin/quan-ly-tai-khoan/Default.aspx?scope=home&fscope=home", 20);
            add("home_point_approval", "Duyệt yêu cầu xác nhận hành vi", "Mở đúng tab duyệt hành vi/điểm của hệ Home theo vai trò hiện tại.", "Hệ Home", "Khối duyệt yêu cầu hành vi và các hồ sơ điểm liên quan của hệ Home.", ResolveHomePointApprovalUrl(db, taikhoan), 30);
            add("tier_reference", "Mô tả cấp bậc", "Tra cứu mô tả cấp bậc, tiêu chí hành vi và nội dung tham chiếu của hệ Home.", "Hệ Home", "Khối tham chiếu cấp bậc và nội dung hành vi của hệ Home.", "/admin/MoTaCapBac.aspx", 52);
            add("shop_workspace", "Trung tâm quản trị gian hàng", "Đi vào portal quản trị của không gian gian hàng đối tác.", "Hệ Gian hàng", "Điểm vào chính của không gian quản trị gian hàng đối tác.", "/gianhang/admin/default.aspx?system=1", 55);
            add("shop_accounts", "Quản lý tài khoản gian hàng đối tác", "Theo dõi hồ sơ shop, cấp vận hành và trạng thái gian hàng đối tác.", "Hệ Shop", "Toàn bộ tài khoản shop và thông tin vận hành shop.", "/admin/quan-ly-tai-khoan/Default.aspx?scope=shop&fscope=shop", 60);
            add("shop_points", "Duyệt điểm / nghiệp vụ shop", "Duyệt các yêu cầu điểm hoặc nghiệp vụ gắn với gian hàng đối tác.", "Hệ Shop", "Chỉ các hồ sơ điểm và nghiệp vụ thuộc gian hàng đối tác.", "/admin/lich-su-chuyen-diem/default.aspx?tab=shop-only", 70);
            add("shop_level2", "Duyệt nâng cấp Level 2", "Duyệt yêu cầu mở quyền dùng /gianhang/admin cho shop.", "Hệ Shop", "Các shop yêu cầu nâng cấp lên bộ công cụ quản trị Level 2.", "/admin/duyet-nang-cap-level2.aspx", 80);
            add("home_gianhang_space", "Duyệt không gian gian hàng", "Duyệt yêu cầu mở không gian /gianhang của tài khoản Home.", "Hệ Gian hàng", "Các tài khoản Home gửi yêu cầu mở không gian /gianhang để tự sở hữu gian hàng của mình.", "/admin/duyet-gian-hang-doi-tac.aspx", 90);
            add("shop_partner", "Duyệt gian hàng đối tác (Shop)", "Duyệt yêu cầu mở không gian /shop và áp dụng % chiết khấu mặc định cho shop.", "Hệ Shop", "Các tài khoản Home gửi yêu cầu mở /shop kèm % chiết khấu để chạy chính sách chia % của nền tảng.", "/admin/duyet-shop-doi-tac.aspx", 100);
            add("shop_email", "Nội dung email gian hàng đối tác", "Sửa nội dung email hệ thống gửi tới shop trong luồng vận hành.", "Hệ Shop", "Các mẫu email nội bộ liên quan đến shop và gian hàng.", "/admin/quan-ly-email-shop/default.aspx", 110);
            add("daugia_workspace", "Trung tâm quản trị đấu giá", "Đi vào portal quản trị của không gian đấu giá.", "Hệ Đấu giá", "Điểm vào chính của không gian quản trị đấu giá.", "/daugia/admin/default.aspx?system=1", 112);
            add("event_workspace", "Trung tâm quản trị sự kiện", "Đi vào portal quản trị của không gian sự kiện.", "Hệ Sự kiện", "Điểm vào chính của không gian quản trị sự kiện.", "/event/admin/default.aspx?system=1", 114);
            add("home_content", "Nội dung trang chủ Home", "Sửa nội dung văn bản hiển thị trên Ahasale.vn.", "Nội dung văn bản", "Các text, nhãn, CTA, mô tả và nội dung văn bản hiển thị trên Ahasale.vn; về sau có thể mở rộng cho /home, /shop, /gianhang/admin và /daugia.", "/admin/quan-ly-noi-dung-home/default.aspx", 120);
            add("home_posts", "Quản lý bài viết", "Quản trị bài viết tổng quan cấp website.", "Tổng quan website", "Kho bài viết, tin tức và nội dung công khai tổng quan của website.", "/admin/quan-ly-bai-viet/default.aspx", 130);
            add("home_menu", "Quản lý menu", "Cấu hình menu và điều hướng tổng quan của website.", "Tổng quan website", "Các menu, danh mục và nhóm điều hướng tổng quan của website.", "/admin/quan-ly-menu/default.aspx", 140);
            add("home_banner", "Quản lý banner", "Quản lý banner và khối chiến dịch tổng quan của website.", "Tổng quan website", "Các banner và vùng hiển thị chiến dịch tổng quan của website.", "/admin/quan-ly-banner/default.aspx", 150);
            add("home_config", "Cài đặt trang chủ", "Cấu hình tổng cho Home như liên kết, bảo trì và khối hiển thị lõi.", "Cổng Admin", "Cấu hình hệ thống của trang Home và các thiết lập lõi.", "/admin/cai-dat-trang-chu/default.aspx", 160);
            add("notifications", "Quản lý thông báo", "Điều phối thông báo hệ thống và trạng thái gửi nhận.", "Cổng Admin", "Thông báo hệ thống toàn nền tảng.", "/admin/quan-ly-thong-bao/default.aspx", 170);
            add("consulting", "Yêu cầu tư vấn", "Tổng hợp và xử lý luồng liên hệ, yêu cầu tư vấn từ người dùng.", "Cổng Admin", "Các yêu cầu tư vấn phát sinh trên toàn nền tảng.", "/admin/yeu-cau-tu-van/default.aspx", 180);
            if (showLegacySystemProductTabs)
            {
                add("system_products", "Bán sản phẩm", "Thao tác bán các sản phẩm/thẻ của hệ thống từ cổng admin.", "Tài sản lõi", "Các nghiệp vụ bán sản phẩm hệ thống có ảnh hưởng tới tài sản lõi.", "/admin/he-thong-san-pham/ban-the.aspx", 190);
                add("issue_cards", "Phát hành thẻ", "Phát hành thẻ theo đúng quy trình tài sản lõi của hệ thống.", "Tài sản lõi", "Các giao dịch phát hành thẻ từ cổng admin.", "/admin/phat-hanh-the/them-moi.aspx", 200);
            }
            add("core_assets", "Lịch sử chuyển điểm", "Rà soát chuyển điểm, tài sản lõi và các giao dịch nội bộ toàn hệ thống.", "Tài sản lõi", "Tiền, quyền, điểm và toàn bộ giao dịch tài sản lõi.", "/admin/lich-su-chuyen-diem/default.aspx?tab=tieu-dung", 210);
            add("token_wallet", "Ví token điểm", "Theo dõi số dư blockchain, đối chiếu điểm A nội bộ và mở cấu hình bridge tài sản lõi.", "Tài sản lõi", "Khối ví/token bridge chỉ dành cho Super Admin.", "/admin/vi-token-diem/default.aspx", 220);
            return items.OrderBy(item => item.SortOrder).ToList();
        }

        if (CanAccessAdminWorkspace(db, taikhoan))
            add("admin_dashboard", "Trang chủ admin", "Landing tổng của khối quản trị admin.", "Cổng Admin", "Điểm vào chính của không gian quản trị admin.", "/admin/default.aspx", 5);

        if (CanManageAdminAccounts(db, taikhoan))
            add("admin_accounts", "Quản lý tài khoản admin", "Tạo tài khoản admin, gán 5 vai trò và kiểm soát đúng cổng vận hành.", "Cổng Admin", "Toàn bộ tài khoản admin và cấu trúc phân quyền nội bộ.", ResolveAdminAccountManagementUrl(db, taikhoan), 10);

        if (CanManageAdminOtp(db, taikhoan))
            add("admin_otp", "Quản lý OTP", "Quản trị cấu hình OTP, nhật ký gửi mã và hỗ trợ xác thực cho các tài khoản trong hệ.", "Cổng Admin", "Khối OTP nội bộ của hệ quản trị.", "/admin/otp/default.aspx", 15);

        if (CanManageAdminFeedback(db, taikhoan))
            add("admin_feedback", "Quản lý góp ý", "Theo dõi, lọc và xử lý các góp ý phát sinh gửi vào hệ quản trị.", "Cổng Admin", "Toàn bộ luồng góp ý của nền tảng.", "/admin/quan-ly-gop-y/default.aspx", 18);

        if (CanManageAdminCompanyShopSync(db, taikhoan))
            add("admin_company_shop_sync", "Đồng bộ shop công ty", "Đồng bộ, làm sạch và rà soát dữ liệu shop công ty dùng cho các tác vụ nội bộ.", "Cổng Admin", "Công cụ nội bộ dành cho vận hành admin.", "/admin/tools/company-shop-sync.aspx", 19);

        if (CanManageAdminReindexBaiViet(db, taikhoan))
            add("admin_reindex_baiviet", "Reindex bài viết", "Chạy lại chỉ mục nội dung để đồng bộ tìm kiếm và các feed bài viết trên website.", "Cổng Admin", "Công cụ vận hành chỉ mục nội dung.", "/admin/tools/reindex-baiviet.aspx", 19);

        if (CanManageAdminNotification(db, taikhoan))
            add("notifications", "Quản lý thông báo", "Điều phối thông báo hệ thống và trạng thái gửi nhận.", "Cổng Admin", "Thông báo hệ thống toàn nền tảng.", "/admin/quan-ly-thong-bao/default.aspx", 170);

        if (CanManageAdminConsulting(db, taikhoan))
            add("consulting", "Yêu cầu tư vấn", "Tổng hợp và xử lý luồng liên hệ, yêu cầu tư vấn từ người dùng.", "Cổng Admin", "Các yêu cầu tư vấn phát sinh trên toàn nền tảng.", "/admin/yeu-cau-tu-van/default.aspx", 180);

        if (CanManageAdminTokenWallet(db, taikhoan))
            add("token_wallet", "Ví token điểm", "Theo dõi số dư blockchain, đối chiếu điểm A nội bộ và mở cấu hình bridge tài sản lõi.", "Tài sản lõi", "Khối ví/token bridge chỉ dành cho Super Admin.", "/admin/vi-token-diem/default.aspx", 220);

        if (CanManageHomeAccounts(db, taikhoan))
            add("home_accounts", "Quản lý tài khoản home", "Rà soát hồ sơ Home, tìm kiếm và xử lý dữ liệu người dùng cá nhân.", "Hệ Home", "Các tài khoản Home nằm trong phạm vi vai trò bạn đang phụ trách.", ResolveHomeAccountManagementUrl(db, taikhoan), 20);

        if (CanReviewHomePointRequests(db, taikhoan))
            add("home_point_approval", "Duyệt yêu cầu xác nhận hành vi", "Mở đúng tab duyệt hành vi/điểm của hệ Home theo vai trò hiện tại.", "Hệ Home", "Khối duyệt yêu cầu hành vi và các hồ sơ điểm liên quan của hệ Home.", ResolveHomePointApprovalUrl(db, taikhoan), 30);

        if (CanViewTierReference(db, taikhoan))
            add("tier_reference", "Mô tả cấp bậc", "Tra cứu mô tả cấp bậc, tiêu chí hành vi và nội dung tham chiếu của hệ Home.", "Hệ Home", "Khối tham chiếu cấp bậc và nội dung hành vi của hệ Home.", "/admin/MoTaCapBac.aspx", 52);

        if (CanManageShopAccounts(db, taikhoan))
            add("shop_accounts", "Quản lý tài khoản gian hàng đối tác", "Theo dõi hồ sơ shop, cấp vận hành và trạng thái gian hàng đối tác.", "Hệ Shop", "Các tài khoản shop nằm trong phạm vi quản trị gian hàng đối tác.", ResolveShopAccountManagementUrl(db, taikhoan), 60);

        if (CanManageShopAccounts(db, taikhoan)
            || CanReviewShopPointRequests(db, taikhoan)
            || CanApproveShopPartnerRegistration(db, taikhoan)
            || CanManageShopOperations(db, taikhoan))
            add("shop_workspace", "Trung tâm quản trị gian hàng", "Đi vào portal quản trị của không gian gian hàng đối tác.", "Hệ Gian hàng", "Điểm vào chính của không gian quản trị gian hàng đối tác.", "/gianhang/admin/default.aspx?system=1", 55);

        if (CanReviewShopPointRequests(db, taikhoan))
            add("shop_points", "Duyệt điểm / nghiệp vụ shop", "Duyệt các yêu cầu điểm hoặc nghiệp vụ gắn với gian hàng đối tác.", "Hệ Shop", "Chỉ các hồ sơ điểm và nghiệp vụ thuộc gian hàng đối tác.", "/admin/lich-su-chuyen-diem/default.aspx?tab=shop-only", 70);

        if (CanApproveShopPartnerRegistration(db, taikhoan))
        {
            add("home_gianhang_space", "Duyệt không gian gian hàng", "Duyệt yêu cầu mở không gian /gianhang của tài khoản Home.", "Hệ Gian hàng", "Các tài khoản Home gửi yêu cầu mở không gian /gianhang để tự sở hữu gian hàng của mình.", "/admin/duyet-gian-hang-doi-tac.aspx", 80);
            add("shop_partner", "Duyệt gian hàng đối tác (Shop)", "Duyệt yêu cầu mở không gian /shop và áp dụng % chiết khấu mặc định cho shop.", "Hệ Shop", "Các tài khoản Home gửi yêu cầu mở /shop kèm % chiết khấu để chạy chính sách chia % của nền tảng.", "/admin/duyet-shop-doi-tac.aspx", 85);
        }

        if (CanApproveShopLevel2(db, taikhoan))
            add("shop_level2", "Duyệt nâng cấp Level 2", "Duyệt yêu cầu mở quyền dùng /gianhang/admin cho shop.", "Hệ Shop", "Các shop yêu cầu nâng cấp lên bộ công cụ quản trị Level 2.", "/admin/duyet-nang-cap-level2.aspx", 87);

        if (CanManageShopOperations(db, taikhoan))
            add("shop_email", "Nội dung email gian hàng đối tác", "Sửa nội dung email hệ thống gửi tới shop trong luồng vận hành.", "Hệ Shop", "Các mẫu email nội bộ liên quan đến shop và gian hàng.", "/admin/quan-ly-email-shop/default.aspx", 80);

        if (CanManageHomeContent(db, taikhoan))
        {
            add("home_config", "Cài đặt trang chủ", "Cấu hình tổng cho Home như liên kết, bảo trì và khối hiển thị lõi.", "Nội dung website", "Cấu hình hệ thống và các thiết lập lõi của khu vực Home.", "/admin/cai-dat-trang-chu/default.aspx", 88);
            add("home_content", "Nội dung trang chủ Home", "Sửa nội dung văn bản hiển thị trên Ahasale.vn.", "Nội dung văn bản", "Các text, nhãn, CTA, mô tả và nội dung văn bản hiển thị trên Ahasale.vn; về sau có thể mở rộng cho /home, /shop, /gianhang/admin và /daugia.", "/admin/quan-ly-noi-dung-home/default.aspx", 90);
        }

        if (DauGiaPolicy_cl.CanAccessAdmin(db, taikhoan, permissionRaw))
            add("daugia_workspace", "Trung tâm quản trị đấu giá", "Đi vào portal quản trị của không gian đấu giá.", "Hệ Đấu giá", "Điểm vào chính của không gian quản trị đấu giá.", "/daugia/admin/default.aspx?system=1", 112);

        if (EventPolicy_cl.CanViewAdminWorkspace(db, taikhoan))
            add("event_workspace", "Trung tâm quản trị sự kiện", "Đi vào portal quản trị của không gian sự kiện.", "Hệ Sự kiện", "Điểm vào chính của không gian quản trị sự kiện.", "/event/admin/default.aspx?system=1", 114);

        return items.OrderBy(item => item.SortOrder).ToList();
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
        if (CanManageHomeContent(db, taikhoan))
            notes.Add("Bạn chỉ được sửa nội dung văn bản hiển thị trên Ahasale.vn, không quản lý menu, banner, bài viết tổng quan và không can thiệp tài sản lõi.");
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

        add(
            "shop_partner",
            "Tài khoản gian hàng đối tác",
            "Hệ Shop",
            "Phụ trách tài khoản shop, nghiệp vụ shop và các yêu cầu điểm/nghiệp vụ của gian hàng đối tác.",
            isSuperAdmin || CanManageShopAccounts(db, taikhoan) || CanReviewShopPointRequests(db, taikhoan),
            isSuperAdmin ? "Super Admin giám sát" : ((CanManageShopAccounts(db, taikhoan) || CanReviewShopPointRequests(db, taikhoan)) ? "Được phụ trách" : "Không phụ trách"),
            isSuperAdmin
                ? "Super Admin theo dõi toàn bộ luồng shop và level 2."
                : ((CanManageShopAccounts(db, taikhoan) || CanReviewShopPointRequests(db, taikhoan))
                    ? "Được quản trị shop và duyệt nghiệp vụ shop trong phạm vi được cấp."
                    : "Tài khoản này không được quản trị gian hàng đối tác."));

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

        if (CanManageHomeContent(db, taikhoan))
            return "/admin/quan-ly-noi-dung-home/default.aspx";

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

        context.Session["thongbao"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            message,
            "1800",
            "warning");
        context.Response.Redirect(string.IsNullOrWhiteSpace(fallbackUrl) ? "/admin/default.aspx" : fallbackUrl);
    }

    public static void RequireSuperAdmin()
    {
        check_login_cl.check_login_admin("none", "none");
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

        string tab = (request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(tab))
            return CanReviewHomePointRequests(db, taikhoan)
                || CanReviewShopPointRequests(db, taikhoan)
                || CanManageShopAccounts(db, taikhoan);

        switch (tab)
        {
            case "uu-dai":
                return CanReviewCustomerPointRequests(db, taikhoan);
            case "lao-dong":
                return CanReviewDevelopmentPointRequests(db, taikhoan);
            case "gan-ket":
                return CanReviewEcosystemPointRequests(db, taikhoan);
            case "shop-only":
                return CanReviewShopPointRequests(db, taikhoan)
                    || CanManageShopAccounts(db, taikhoan);
            case "tieu-dung":
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
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(value))
            return "";

        if (value == "admin" || value == PortalScope_cl.ScopeAdmin)
            return "admin";

        if (value == "home" || value == PortalScope_cl.ScopeHome)
            return "home";

        if (value == "shop" || value == PortalScope_cl.ScopeShop)
            return "shop";

        return "";
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

        if (string.Equals(normalizedScope, "home", StringComparison.OrdinalIgnoreCase))
        {
            if (!(string.Equals(presetKey, PresetHomeCustomer, StringComparison.OrdinalIgnoreCase)
                || string.Equals(presetKey, PresetHomeDevelopment, StringComparison.OrdinalIgnoreCase)
                || string.Equals(presetKey, PresetHomeEcosystem, StringComparison.OrdinalIgnoreCase)))
                return false;

            return string.Equals(normalizedRole, presetKey, StringComparison.OrdinalIgnoreCase);
        }

        if (string.Equals(normalizedScope, "shop", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(normalizedRole, PresetShopPartner, StringComparison.OrdinalIgnoreCase)
                && string.Equals(presetKey, PresetShopPartner, StringComparison.OrdinalIgnoreCase);
        }

        if (string.Equals(normalizedScope, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(normalizedRole, PresetHomeContent, StringComparison.OrdinalIgnoreCase)
                && string.Equals(presetKey, PresetHomeContent, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static bool CanAccessAccountManagementRoute(dbDataContext db, string taikhoan, HttpRequest request)
    {
        string scope = NormalizeAccountManagementScope(request.QueryString["scope"]);
        string filterRole = request.QueryString["frole"];

        if (string.IsNullOrWhiteSpace(scope))
            return CanManageHomeAccounts(db, taikhoan)
                || CanManageShopAccounts(db, taikhoan)
                || CanManageAdminAccounts(db, taikhoan);

        if (scope == "home")
            return CanManageHomeAccounts(db, taikhoan)
                && CanAccessAccountManagementRoleFilter(db, taikhoan, scope, filterRole);

        if (scope == "shop")
            return CanManageShopAccounts(db, taikhoan)
                && CanAccessAccountManagementRoleFilter(db, taikhoan, scope, filterRole);

        if (scope == "admin")
            return CanManageAdminAccounts(db, taikhoan)
                && CanAccessAccountManagementRoleFilter(db, taikhoan, scope, filterRole);

        return false;
    }

    private static string BuildAccountManagementDeniedMessage(HttpRequest request)
    {
        string scope = NormalizeAccountManagementScope(request.QueryString["scope"]);
        switch (scope)
        {
            case "admin":
                return "Bạn không có quyền truy cập khu quản trị tài khoản admin.";
            case "home":
                return "Bạn không có quyền truy cập khu quản trị tài khoản Home của vai trò này.";
            case "shop":
                return "Bạn không có quyền truy cập khu quản trị tài khoản gian hàng đối tác.";
            default:
                return "Bạn không có quyền truy cập khu quản trị tài khoản này.";
        }
    }

    private static bool IsRootOnlyRoute(string path)
    {
        return path.StartsWith("~/admin/cai-dat-trang-chu/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/he-thong-san-pham/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/otp/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/vi-token-diem/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-gop-y/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-thong-bao/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/yeu-cau-tu-van/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-bai-viet/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-menu/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/quan-ly-banner/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("~/admin/tools/", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/phat-hanh-the.aspx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(path, "~/admin/duyet-nang-cap-level2.aspx", StringComparison.OrdinalIgnoreCase);
    }
}
