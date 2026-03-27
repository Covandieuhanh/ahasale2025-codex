using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_left_uc : System.Web.UI.UserControl
{
    public string loi, tuvan;

    private bool GetFlag(string key)
    {
        object v = ViewState[key];
        if (v == null) return false;
        bool b;
        return bool.TryParse(v.ToString(), out b) && b;
    }

    private void SetFlag(string key, bool value)
    {
        ViewState[key] = value ? "true" : "false";
    }

    private void BuildLeftMenuPermissionFlags(taikhoan_tb account)
    {
        string tk = account != null ? (account.taikhoan ?? "").Trim() : "";
        bool isRoot = account != null && AdminRolePolicy_cl.IsSuperAdmin(tk);
        SetFlag("left_is_root", isRoot);

        bool showAdminDashboard = false;
        bool showAdminAccount = false;
        bool showAdminOtp = false;
        bool showAdminTokenWallet = false;
        bool showAdminGopY = false;
        bool showAdminThongBao = false;
        bool showAdminTuVan = false;
        bool showAdminCompanyShopSync = false;
        bool showAdminReindexBaiViet = false;
        bool showTransferHistory = false;
        bool showHomeAccount = false;
        bool showApproveHanhVi = false;
        bool showIssueCard = false;
        bool showTierDescription = false;
        bool showSellProduct = false;
        bool showShopWorkspace = false;
        bool showShopAccount = false;
        bool showShopApprove = false;
        bool showShopLevel2 = false;
        bool showShopEmailTemplate = false;
        bool showShopPointApproval = false;
        bool showHomeSettings = false;
        bool showHomeTextContent = false;
        bool showContentMenu = false;
        bool showContentBaiViet = false;
        bool showContentBanner = false;
        bool showDauGiaWorkspace = false;
        bool showEventWorkspace = false;
        bool showLegacySystemProductMenu = !CompanyShop_cl.HideLegacyAdminSystemProduct();

        if (account != null && !string.IsNullOrWhiteSpace(tk))
        {
            using (dbDataContext db = new dbDataContext())
            {
                string phanloai = account != null ? (account.phanloai ?? "") : "";
                string permissionRaw = account != null ? (account.permission ?? "") : "";
                var tabKeys = new HashSet<string>(
                    AdminRolePolicy_cl.BuildAdminHomeTabs(db, tk, phanloai, permissionRaw)
                        .Select(item => (item.Key ?? "").Trim()),
                    StringComparer.OrdinalIgnoreCase);

                Func<string, bool> hasKey = key => tabKeys.Contains((key ?? "").Trim());

                showAdminDashboard = hasKey("admin_dashboard");
                showAdminAccount = hasKey("admin_accounts");
                showAdminOtp = hasKey("admin_otp");
                showAdminTokenWallet = hasKey("token_wallet");
                showAdminGopY = hasKey("admin_feedback");
                showAdminThongBao = hasKey("notifications");
                showAdminTuVan = hasKey("consulting");
                showAdminCompanyShopSync = hasKey("admin_company_shop_sync");
                showAdminReindexBaiViet = hasKey("admin_reindex_baiviet");
                showTransferHistory = hasKey("core_assets");
                showHomeAccount = hasKey("home_accounts");
                showApproveHanhVi = hasKey("home_point_approval")
                    || hasKey("home_points_customer")
                    || hasKey("home_points_development")
                    || hasKey("home_points_ecosystem");
                showIssueCard = showLegacySystemProductMenu && hasKey("issue_cards");
                showTierDescription = hasKey("tier_reference");
                showSellProduct = showLegacySystemProductMenu && hasKey("system_products");
                showShopWorkspace = hasKey("shop_workspace");
                showShopAccount = hasKey("shop_accounts");
                showShopApprove = hasKey("home_gianhang_space") || hasKey("shop_partner");
                showShopLevel2 = hasKey("shop_level2");
                showShopEmailTemplate = hasKey("shop_email");
                showShopPointApproval = hasKey("shop_points");
                showHomeSettings = hasKey("home_config");
                showHomeTextContent = hasKey("home_content");
                showContentMenu = hasKey("home_menu");
                showContentBaiViet = hasKey("home_posts");
                showContentBanner = hasKey("home_banner");
                showDauGiaWorkspace = hasKey("daugia_workspace");
                showEventWorkspace = hasKey("event_workspace");
            }
        }

        SetFlag("left_admin_dashboard", showAdminDashboard);
        SetFlag("left_admin_account", showAdminAccount);
        SetFlag("left_admin_otp", showAdminOtp);
        SetFlag("left_admin_token_wallet", showAdminTokenWallet);
        SetFlag("left_admin_gopy", showAdminGopY);
        SetFlag("left_admin_thongbao", showAdminThongBao);
        SetFlag("left_admin_tuvan", showAdminTuVan);
        SetFlag("left_admin_company_shop_sync", showAdminCompanyShopSync);
        SetFlag("left_admin_reindex_baiviet", showAdminReindexBaiViet);
        SetFlag("left_transfer_history", showTransferHistory);
        SetFlag("left_home_account", showHomeAccount);
        SetFlag("left_home_approve_hanhvi", showApproveHanhVi);
        SetFlag("left_home_issue_card", showIssueCard);
        SetFlag("left_home_tier_desc", showTierDescription);
        SetFlag("left_home_sell_product", showSellProduct);
        SetFlag("left_shop_workspace", showShopWorkspace);
        SetFlag("left_shop_account", showShopAccount);
        SetFlag("left_shop_approve", showShopApprove);
        SetFlag("left_shop_level2", showShopLevel2);
        SetFlag("left_shop_email_template", showShopEmailTemplate);
        SetFlag("left_shop_point_approval", showShopPointApproval);
        SetFlag("left_content_home", showHomeSettings);
        SetFlag("left_content_home_text", showHomeTextContent);
        SetFlag("left_content_menu", showContentMenu);
        SetFlag("left_content_baiviet", showContentBaiViet);
        SetFlag("left_content_banner", showContentBanner);
        SetFlag("left_daugia_workspace", showDauGiaWorkspace);
        SetFlag("left_event_workspace", showEventWorkspace);
    }

    private bool IsRootAdminCurrent()
    {
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc))
            return false;

        string tk = "";
        try { tk = mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { tk = tkEnc; }
        return PermissionProfile_cl.IsRootAdmin(tk);
    }

    private bool CanManageHomeContentCurrent()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                return AdminRolePolicy_cl.CanManageHomeContent(db, AdminRolePolicy_cl.GetCurrentAdminUser());
            }
        }
        catch
        {
            return false;
        }
    }

    public bool ShowHomeLandingSettingsTab()
    {
        return IsManagementSpaceContent() && GetFlag("left_content_home");
    }

    public bool ShowHomeLandingContentTab()
    {
        return IsManagementSpaceContent() && GetFlag("left_content_home_text");
    }

    private string GetScopedUrl(string url, string space)
    {
        return AdminManagementSpace_cl.AppendSpaceToUrl(url, space);
    }

    private string GetCurrentManagementSpace()
    {
        object value = ViewState["admin_management_space"];
        string space = value == null ? "" : value.ToString();
        space = AdminManagementSpace_cl.NormalizeSpace(space);
        return string.IsNullOrWhiteSpace(space) ? AdminManagementSpace_cl.SpaceAdmin : space;
    }

    public bool IsManagementSpaceAdmin() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceAdmin; }
    public bool IsManagementSpaceHome() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceHome; }
    public bool IsManagementSpaceGianHang() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceGianHang; }
    public bool IsManagementSpaceDauGia() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceDauGia; }
    public bool IsManagementSpaceEvent() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceEvent; }
    public bool IsManagementSpaceContent() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceContent; }

    public bool ShowLeftAdminDashboard() { return IsManagementSpaceAdmin() && GetFlag("left_admin_dashboard"); }
    public bool ShowLeftAdminAccount() { return IsManagementSpaceAdmin() && GetFlag("left_admin_account"); }
    public bool ShowLeftAdminOtp() { return IsManagementSpaceAdmin() && GetFlag("left_admin_otp"); }
    public bool ShowLeftAdminTokenWallet() { return IsManagementSpaceAdmin() && GetFlag("left_admin_token_wallet"); }
    public bool ShowLeftAdminGopY() { return IsManagementSpaceAdmin() && GetFlag("left_admin_gopy"); }
    public bool ShowLeftAdminThongBao() { return IsManagementSpaceAdmin() && GetFlag("left_admin_thongbao"); }
    public bool ShowLeftAdminTuVan() { return IsManagementSpaceAdmin() && GetFlag("left_admin_tuvan"); }
    public bool ShowLeftAdminCompanyShopSync() { return IsManagementSpaceAdmin() && GetFlag("left_admin_company_shop_sync"); }
    public bool ShowLeftAdminReindexBaiViet() { return IsManagementSpaceAdmin() && GetFlag("left_admin_reindex_baiviet"); }
    public bool ShowLeftTransferHistory() { return IsManagementSpaceHome() && GetFlag("left_transfer_history"); }
    public bool ShowLeftHomeAccount() { return IsManagementSpaceHome() && GetFlag("left_home_account"); }
    public bool ShowLeftApproveHanhVi() { return IsManagementSpaceHome() && GetFlag("left_home_approve_hanhvi"); }
    public bool ShowLeftIssueCard() { return IsManagementSpaceHome() && GetFlag("left_home_issue_card"); }
    public bool ShowLeftTierDescription() { return IsManagementSpaceHome() && GetFlag("left_home_tier_desc"); }
    public bool ShowLeftSellProduct() { return IsManagementSpaceHome() && GetFlag("left_home_sell_product"); }
    public bool ShowLeftShopWorkspace() { return IsManagementSpaceGianHang() && GetFlag("left_shop_workspace"); }
    public bool ShowLeftShopAccount() { return IsManagementSpaceGianHang() && GetFlag("left_shop_account"); }
    public bool ShowLeftShopApprove() { return IsManagementSpaceGianHang() && GetFlag("left_shop_approve"); }
    public bool ShowLeftShopLevel2() { return IsManagementSpaceGianHang() && GetFlag("left_shop_level2"); }
    public bool ShowLeftShopEmailTemplate() { return IsManagementSpaceGianHang() && GetFlag("left_shop_email_template"); }
    public bool ShowLeftShopPointApproval() { return IsManagementSpaceGianHang() && GetFlag("left_shop_point_approval"); }
    public bool ShowLeftContentMenu() { return IsManagementSpaceContent() && GetFlag("left_content_menu"); }
    public bool ShowLeftContentBaiViet() { return IsManagementSpaceContent() && GetFlag("left_content_baiviet"); }
    public bool ShowLeftContentBanner() { return IsManagementSpaceContent() && GetFlag("left_content_banner"); }
    public bool ShowLeftDauGiaWorkspace() { return IsManagementSpaceDauGia() && GetFlag("left_daugia_workspace"); }
    public bool ShowLeftEventWorkspace() { return IsManagementSpaceEvent() && GetFlag("left_event_workspace"); }

    public bool ShowLeftGroupAdmin()
    {
        return ShowLeftAdminDashboard() || ShowLeftAdminAccount() || ShowLeftAdminOtp() || ShowLeftAdminTokenWallet()
            || ShowLeftAdminGopY() || ShowLeftAdminThongBao() || ShowLeftAdminTuVan()
            || ShowLeftAdminCompanyShopSync() || ShowLeftAdminReindexBaiViet();
    }

    public bool ShowLeftGroupHomeSpace()
    {
        return ShowLeftTransferHistory() || ShowLeftHomeAccount() || ShowLeftApproveHanhVi() || ShowLeftIssueCard()
            || ShowLeftTierDescription() || ShowLeftSellProduct();
    }

    public bool ShowLeftGroupShop() { return ShowLeftShopWorkspace() || ShowLeftShopAccount() || ShowLeftShopApprove() || ShowLeftShopLevel2() || ShowLeftShopEmailTemplate() || ShowLeftShopPointApproval(); }
    public bool ShowLeftGroupContent() { return ShowHomeLandingSettingsTab() || ShowHomeLandingContentTab() || ShowLeftContentMenu() || ShowLeftContentBaiViet() || ShowLeftContentBanner(); }
    public bool ShowLeftGroupDauGia() { return ShowLeftDauGiaWorkspace(); }
    public bool ShowLeftGroupEvent() { return ShowLeftEventWorkspace(); }

    private string GetCurrentAdminAccount()
    {
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc))
            return "";

        try { return mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { return tkEnc; }
    }

    public string GetApproveHomePointUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveHomePointApprovalUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceHome);
            }
        }

        return GetScopedUrl("/admin/duyet-yeu-cau-len-cap.aspx", AdminManagementSpace_cl.SpaceHome);
    }

    public string GetHomeAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveHomeAccountManagementUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceHome);
            }
        }

        return GetScopedUrl("/admin/quan-ly-tai-khoan/Default.aspx?scope=home&fscope=home", AdminManagementSpace_cl.SpaceHome);
    }

    public string GetAdminDashboardUrl()
    {
        return GetScopedUrl("/admin/default.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveAdminAccountManagementUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceAdmin);
            }
        }

        return GetScopedUrl("/admin/quan-ly-tai-khoan/Default.aspx?scope=admin&fscope=admin", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminOtpUrl()
    {
        return GetScopedUrl("/admin/otp/default.aspx?scope=home", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminTokenWalletUrl()
    {
        return GetScopedUrl("/admin/vi-token-diem/default.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminGopYUrl() { return GetScopedUrl("/admin/quan-ly-gop-y/default.aspx", AdminManagementSpace_cl.SpaceAdmin); }
    public string GetAdminThongBaoUrl() { return GetScopedUrl("/admin/quan-ly-thong-bao/default.aspx", AdminManagementSpace_cl.SpaceAdmin); }
    public string GetAdminTuVanUrl() { return GetScopedUrl("/admin/yeu-cau-tu-van/default.aspx", AdminManagementSpace_cl.SpaceAdmin); }
    public string GetAdminCompanyShopSyncUrl() { return GetScopedUrl("/admin/tools/company-shop-sync.aspx", AdminManagementSpace_cl.SpaceAdmin); }
    public string GetAdminReindexBaiVietUrl() { return GetScopedUrl("/admin/tools/reindex-baiviet.aspx", AdminManagementSpace_cl.SpaceAdmin); }

    public string GetShopWorkspaceUrl()
    {
        return GetScopedUrl("/gianhang/admin/default.aspx?system=1", AdminManagementSpace_cl.SpaceGianHang);
    }

    public string GetShopAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveShopAccountManagementUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceGianHang);
            }
        }

        return GetScopedUrl("/admin/quan-ly-tai-khoan/Default.aspx?scope=shop&fscope=shop&frole=shop_partner", AdminManagementSpace_cl.SpaceGianHang);
    }

    public string GetDauGiaAdminUrl()
    {
        return GetScopedUrl("/daugia/admin/default.aspx?system=1", AdminManagementSpace_cl.SpaceDauGia);
    }

    public string GetEventAdminUrl()
    {
        return GetScopedUrl("/event/admin/default.aspx?system=1", AdminManagementSpace_cl.SpaceEvent);
    }

    public string GetHomeTransferHistoryUrl() { return GetScopedUrl("/admin/lich-su-chuyen-diem/default.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetHomeIssueCardUrl() { return GetScopedUrl("/admin/phat-hanh-the/them-moi.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetHomeTierDescriptionUrl() { return GetScopedUrl("/admin/MoTaCapBac.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetHomeSellProductUrl() { return GetScopedUrl("/admin/he-thong-san-pham/ban-the.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetShopPointApprovalUrl() { return GetScopedUrl("/admin/lich-su-chuyen-diem/default.aspx?tab=shop-only", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetShopEmailTemplateUrl() { return GetScopedUrl("/admin/quan-ly-email-shop/default.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetGianHangApprovalUrl() { return GetScopedUrl("/admin/duyet-gian-hang-doi-tac.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetShopPartnerApprovalUrl() { return GetScopedUrl("/admin/duyet-shop-doi-tac.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetShopLevel2ApprovalUrl() { return GetScopedUrl("/admin/duyet-nang-cap-level2.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetContentSettingsUrl() { return GetScopedUrl("/admin/cai-dat-trang-chu/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentHomeTextUrl() { return GetScopedUrl("/admin/quan-ly-noi-dung-home/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentMenuUrl() { return GetScopedUrl("/admin/quan-ly-menu/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBaiVietUrl() { return GetScopedUrl("/admin/quan-ly-bai-viet/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBannerUrl() { return GetScopedUrl("/admin/quan-ly-banner/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string MenuActive(params string[] urls)
    {
        string currentUrl = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
        foreach (string url in urls)
        {
            if (currentUrl == (url ?? "").ToLower().Trim())
                return "active";
        }
        return "";
    }

    public string MenuActiveTransferHistory()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/lich-su-chuyen-diem/default.aspx"
            && currentUrl != "/admin/lich-su-chuyen-diem/chuyen-diem.aspx")
            return "";

        string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
        if (tab == "uu-dai" || tab == "lao-dong" || tab == "gan-ket")
            return "";

        return "active";
    }

    public string MenuActivePointApproval()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl == "/admin/duyet-yeu-cau-len-cap.aspx")
            return "active";
        if (currentUrl != "/admin/lich-su-chuyen-diem/default.aspx"
            && currentUrl != "/admin/lich-su-chuyen-diem/chuyen-diem.aspx")
            return "";

        string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
        if (tab == "uu-dai" || tab == "lao-dong" || tab == "gan-ket")
            return "active";

        return "";
    }

    public string MenuActiveShopPointApproval()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/lich-su-chuyen-diem/default.aspx"
            && currentUrl != "/admin/lich-su-chuyen-diem/chuyen-diem.aspx")
            return "";

        string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
        return tab == "shop-only" ? "active" : "";
    }

    public string MenuActiveTaiKhoanScope(string scope)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/quan-ly-tai-khoan/default.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/them-moi.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/bo-loc.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/chinh-sua.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/phan-quyen.aspx")
            return "";

        string currentScope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
        string targetScope = (scope ?? "").Trim().ToLowerInvariant();
        if (currentScope == targetScope)
            return "active";
        return "";
    }

    public string MenuActiveTokenWallet()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl == "/admin/vi-token-diem/default.aspx")
            return "active";
        if (currentUrl != "/admin/default.aspx")
            return "";

        string section = (Request.QueryString["section"] ?? "").Trim().ToLowerInvariant();
        return section == "vi-token-diem" ? "active" : "";
    }

    private static string ResolveTitle(string url)
    {
        string normalized = (url ?? "").ToLower().Trim();
        switch (normalized)
        {
            case "/admin/quan-ly-menu/default.aspx":
            case "/admin/quan-ly-menu/them-moi.aspx":
            case "/admin/quan-ly-menu/bo-loc.aspx":
            case "/admin/quan-ly-menu/chinh-sua.aspx":
            case "/admin/quan-ly-menu/xuat-du-lieu.aspx":
            case "/admin/quan-ly-menu/ban-in.aspx":
                return "Quản lý menu";
            case "/admin/quan-ly-bai-viet/default.aspx":
            case "/admin/quan-ly-bai-viet/in.aspx":
            case "/admin/quan-ly-bai-viet/them-moi.aspx":
            case "/admin/quan-ly-bai-viet/bo-loc.aspx":
            case "/admin/quan-ly-bai-viet/chinh-sua.aspx":
            case "/admin/quan-ly-bai-viet/xuat-du-lieu.aspx":
            case "/admin/quan-ly-bai-viet/ban-in.aspx":
                return "Quản lý bài viết";
            case "/admin/quan-ly-banner/default.aspx":
            case "/admin/quan-ly-banner/them-moi.aspx":
                return "Quản lý banner";
            case "/admin/quan-ly-gop-y/default.aspx":
                return "Quản lý góp ý";
            case "/admin/quan-ly-thong-bao/default.aspx":
            case "/admin/quan-ly-thong-bao/in.aspx":
            case "/admin/quan-ly-thong-bao/bo-loc.aspx":
            case "/admin/quan-ly-thong-bao/xuat-du-lieu.aspx":
            case "/admin/quan-ly-thong-bao/ban-in.aspx":
                return "Quản lý thông báo";
            case "/admin/yeu-cau-tu-van/default.aspx":
            case "/admin/yeu-cau-tu-van/bo-loc.aspx":
            case "/admin/yeu-cau-tu-van/xuat-du-lieu.aspx":
            case "/admin/yeu-cau-tu-van/ban-in.aspx":
                return "Yêu cầu tư vấn";
            case "/admin/tools/company-shop-sync.aspx":
                return "Đồng bộ shop công ty";
            case "/admin/tools/reindex-baiviet.aspx":
                return "Reindex bài viết";
            case "/admin/lich-su-chuyen-diem/default.aspx":
            case "/admin/lich-su-chuyen-diem/chuyen-diem.aspx":
                return "Lịch sử chuyển điểm";
            case "/admin/cai-dat-trang-chu/default.aspx":
                return "Cài đặt trang chủ";
            case "/admin/quan-ly-noi-dung-home/default.aspx":
                return "Nội dung trang chủ Home";
            case "/admin/quan-ly-tai-khoan/Default.aspx":
            case "/admin/quan-ly-tai-khoan/them-moi.aspx":
            case "/admin/quan-ly-tai-khoan/bo-loc.aspx":
            case "/admin/quan-ly-tai-khoan/chinh-sua.aspx":
            case "/admin/quan-ly-tai-khoan/phan-quyen.aspx":
                return "Quản lý tài khoản";
            case "/admin/duyet-yeu-cau-len-cap.aspx":
                return "Duyệt yêu cầu xác nhận hành vi";
            case "/admin/duyet-gian-hang-doi-tac.aspx":
                return "Duyệt không gian gian hàng";
            case "/admin/duyet-shop-doi-tac.aspx":
                return "Duyệt gian hàng đối tác (Shop)";
            case "/admin/otp/default.aspx":
                return "Quản lý OTP";
            case "/admin/phat-hanh-the.aspx":
            case "/admin/phat-hanh-the/them-moi.aspx":
                return "Phát hành thẻ";
            case "/admin/doi-mat-khau/default.aspx":
                return "Đổi mật khẩu";
            case "/admin/quen-mat-khau/default.aspx":
            case "/admin/khoi-phuc-mat-khau.aspx":
                return "Quên mật khẩu";
            case "/admin/vi-token-diem/default.aspx":
                return "Ví token điểm";
            case "/admin/motacapbac.aspx":
                return "Mô tả cấp bậc";
            case "/admin/he-thong-san-pham/ban-san-pham.aspx":
            case "/admin/he-thong-san-pham/ban-the.aspx":
            case "/admin/he-thong-san-pham/chi-tiet-giao-dich.aspx":
                return "Bán sản phẩm";
            default:
                return "Trang chủ admin";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string _url = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
                string title = ResolveTitle(_url);
                if (_url == "/admin/quan-ly-tai-khoan/Default.aspx")
                {
                    string scope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
                    if (scope == "admin") title = "Quản lý tài khoản admin";
                    else if (scope == "home") title = "Quản lý tài khoản home";
                    else if (scope == "shop") title = "Quản lý tài khoản gian hàng đối tác";
                }
                else if (_url == "/admin/lich-su-chuyen-diem/default.aspx")
                {
                    string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
                    if (tab == "uu-dai") title = "Duyệt điểm hồ sơ Khách hàng";
                    else if (tab == "lao-dong") title = "Duyệt điểm hồ sơ Cộng tác phát triển";
                    else if (tab == "gan-ket") title = "Duyệt điểm hồ sơ Đồng hành hệ sinh thái";
                    else if (tab == "shop-only") title = "Duyệt điểm / nghiệp vụ shop";
                }
                Session["title"] = title;

                using (dbDataContext db = new dbDataContext())
                {
                    string tkEnc = Session["taikhoan"] as string;
                    string tk = "";
                    if (!string.IsNullOrEmpty(tkEnc))
                    {
                        try { tk = mahoa_cl.giaima_Bcorn(tkEnc); }
                        catch { tk = tkEnc; }
                    }
                    taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk);
                    BuildLeftMenuPermissionFlags(account);
                    if (account != null)
                    {
                        string currentSpace = AdminManagementSpace_cl.GetCurrentSpace(db, account, Request);
                        ViewState["admin_management_space"] = currentSpace;
                        ViewState["admin_role_label"] = AdminRolePolicy_cl.GetAdminRoleLabel(db, account.taikhoan, account.phanloai, account.permission);
                        ViewState["admin_scope_label"] = AdminManagementSpace_cl.GetTitle(currentSpace);
                    }
                    else
                    {
                        ViewState["admin_role_label"] = "Trang quản trị";
                        ViewState["admin_scope_label"] = "Cổng Admin";
                    }

                    #region ĐẾM LỖI HỆ THỐNG CHƯA XỬ LÝ
                    int q_loi = db.Log_tbs.Count(p => p.trangthai == "Chưa sửa" && p.bin == false);
                    if (q_loi < 100)
                        loi = q_loi.ToString();
                    else
                        loi = "99+";
                    #endregion
                }
            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }
}
