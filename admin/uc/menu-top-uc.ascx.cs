using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_top_uc : System.Web.UI.UserControl
{
    private readonly Dictionary<string, string> _featureUrlCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

    private void BuildMenuPermissionFlags(dbDataContext db, taikhoan_tb account)
    {
        bool showLegacySystemProductMenu = !CompanyShop_cl.HideLegacyAdminSystemProduct();
        AdminMenuPolicy_cl.MenuVisibility visibility = AdminMenuPolicy_cl.Build(db, account, showLegacySystemProductMenu);

        SetFlag("menu_is_root", visibility.IsRoot);
        SetFlag("menu_admin_dashboard", visibility.AdminDashboard);
        SetFlag("menu_admin_account", visibility.AdminAccount);
        SetFlag("menu_admin_otp", visibility.AdminOtp);
        SetFlag("menu_admin_token_wallet", visibility.AdminTokenWallet);
        SetFlag("menu_admin_gopy", visibility.AdminGopY);
        SetFlag("menu_admin_thongbao", visibility.AdminThongBao);
        SetFlag("menu_admin_tuvan", visibility.AdminTuVan);
        SetFlag("menu_admin_company_shop_sync", visibility.AdminCompanyShopSync);
        SetFlag("menu_admin_reindex_baiviet", visibility.AdminReindexBaiViet);
        SetFlag("menu_transfer_history", visibility.TransferHistory);
        SetFlag("menu_home_account", visibility.HomeAccount);
        SetFlag("menu_home_approve_hanhvi", visibility.ApproveHanhVi);
        SetFlag("menu_home_issue_card", visibility.IssueCard);
        SetFlag("menu_home_tier_desc", visibility.TierDescription);
        SetFlag("menu_home_sell_product", visibility.SellProduct);
        SetFlag("menu_shop_workspace", visibility.ShopWorkspace);
        SetFlag("menu_shop_account", visibility.ShopAccount);
        SetFlag("menu_shop_legacy_invoices", visibility.ShopLegacyInvoices);
        SetFlag("menu_shop_legacy_customers", visibility.ShopLegacyCustomers);
        SetFlag("menu_shop_legacy_content", visibility.ShopLegacyContent);
        SetFlag("menu_shop_legacy_finance", visibility.ShopLegacyFinance);
        SetFlag("menu_shop_legacy_inventory", visibility.ShopLegacyInventory);
        SetFlag("menu_shop_legacy_accounts", visibility.ShopLegacyAccounts);
        SetFlag("menu_shop_legacy_org", visibility.ShopLegacyOrg);
        SetFlag("menu_shop_legacy_training", visibility.ShopLegacyTraining);
        SetFlag("menu_shop_legacy_support", visibility.ShopLegacySupport);
        SetFlag("menu_shop_approve_home_space", visibility.HomeGianHangApproval);
        SetFlag("menu_shop_approve_partner", visibility.ShopPartnerApproval);
        SetFlag("menu_shop_level2", visibility.ShopLevel2);
        SetFlag("menu_shop_email_template", visibility.ShopEmailTemplate);
        SetFlag("menu_shop_point_approval", visibility.ShopPointApproval);
        SetFlag("menu_content_home", visibility.HomeContent);
        SetFlag("menu_content_home_text", visibility.HomeTextContent);
        SetFlag("menu_batdongsan_linked", visibility.BatDongSanLinked);
        SetFlag("menu_content_menu", visibility.ContentMenu);
        SetFlag("menu_content_baiviet", visibility.ContentBaiViet);
        SetFlag("menu_content_banner", visibility.ContentBanner);
        SetFlag("menu_daugia_workspace", visibility.DauGiaWorkspace);
        SetFlag("menu_event_workspace", visibility.EventWorkspace);

        // Super Admin luôn nhìn thấy đầy đủ tab trong mọi không gian quản trị.
        if (visibility.IsRoot)
        {
            string[] allFlagKeys =
            {
                "menu_admin_dashboard",
                "menu_admin_account",
                "menu_admin_otp",
                "menu_admin_token_wallet",
                "menu_admin_gopy",
                "menu_admin_thongbao",
                "menu_admin_tuvan",
                "menu_admin_company_shop_sync",
                "menu_admin_reindex_baiviet",
                "menu_transfer_history",
                "menu_home_account",
                "menu_home_approve_hanhvi",
                "menu_home_issue_card",
                "menu_home_tier_desc",
                "menu_home_sell_product",
                "menu_shop_workspace",
                "menu_shop_account",
                "menu_shop_legacy_invoices",
                "menu_shop_legacy_customers",
                "menu_shop_legacy_content",
                "menu_shop_legacy_finance",
                "menu_shop_legacy_inventory",
                "menu_shop_legacy_accounts",
                "menu_shop_legacy_org",
                "menu_shop_legacy_training",
                "menu_shop_legacy_support",
                "menu_shop_approve_home_space",
                "menu_shop_approve_partner",
                "menu_shop_level2",
                "menu_shop_email_template",
                "menu_shop_point_approval",
                "menu_content_home",
                "menu_content_home_text",
                "menu_batdongsan_linked",
                "menu_content_menu",
                "menu_content_baiviet",
                "menu_content_banner",
                "menu_daugia_workspace",
                "menu_event_workspace"
            };

            foreach (string key in allFlagKeys)
                SetFlag(key, true);
        }
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
    public bool IsManagementSpaceBatDongSan() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceBatDongSan; }
    public bool IsManagementSpaceDauGia() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceDauGia; }
    public bool IsManagementSpaceEvent() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceEvent; }
    public bool IsManagementSpaceContent() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceContent; }

    public bool ShowMenuAdminDashboard() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_dashboard"); }
    public bool ShowMenuAdminAccount() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_account"); }
    public bool ShowMenuAdminOtp() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_otp"); }
    public bool ShowMenuAdminTokenWallet() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_token_wallet"); }
    public bool ShowMenuAdminGopY() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_gopy"); }
    public bool ShowMenuAdminThongBao() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_thongbao"); }
    public bool ShowMenuAdminTuVan() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_tuvan"); }
    public bool ShowMenuAdminCompanyShopSync() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_company_shop_sync"); }
    public bool ShowMenuAdminReindexBaiViet() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_reindex_baiviet"); }
    public bool ShowMenuTransferHistory() { return IsManagementSpaceHome() && GetFlag("menu_transfer_history"); }
    public bool ShowMenuHomeAccount() { return IsManagementSpaceHome() && GetFlag("menu_home_account"); }
    public bool ShowMenuApproveHanhVi() { return IsManagementSpaceHome() && GetFlag("menu_home_approve_hanhvi"); }
    public bool ShowMenuIssueCard() { return IsManagementSpaceHome() && GetFlag("menu_home_issue_card"); }
    public bool ShowMenuTierDescription() { return IsManagementSpaceHome() && GetFlag("menu_home_tier_desc"); }
    public bool ShowMenuSellProduct() { return IsManagementSpaceHome() && GetFlag("menu_home_sell_product"); }
    public bool ShowMenuShopWorkspace() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_workspace"); }
    public bool ShowMenuShopAccount() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_account"); }
    public bool ShowMenuShopLegacyInvoices() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_invoices"); }
    public bool ShowMenuShopLegacyCustomers() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_customers"); }
    public bool ShowMenuShopLegacyContent() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_content"); }
    public bool ShowMenuShopLegacyFinance() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_finance"); }
    public bool ShowMenuShopLegacyInventory() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_inventory"); }
    public bool ShowMenuShopLegacyAccounts() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_accounts"); }
    public bool ShowMenuShopLegacyOrg() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_org"); }
    public bool ShowMenuShopLegacyTraining() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_training"); }
    public bool ShowMenuShopLegacySupport() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_legacy_support"); }
    public bool ShowMenuGianHangApproval() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_approve_home_space"); }
    public bool ShowMenuShopPartnerApproval() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_approve_partner"); }
    public bool ShowMenuShopLevel2() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_level2"); }
    public bool ShowMenuShopEmailTemplate() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_email_template"); }
    public bool ShowMenuShopPointApproval() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_point_approval"); }
    public bool ShowMenuHomeContent() { return IsManagementSpaceContent() && GetFlag("menu_content_home"); }
    public bool ShowMenuHomeTextContent() { return IsManagementSpaceContent() && GetFlag("menu_content_home_text"); }
    public bool ShowMenuBatDongSanWorkspace() { return IsManagementSpaceBatDongSan() && GetFlag("menu_batdongsan_linked"); }
    public bool ShowMenuContentMenu() { return IsManagementSpaceContent() && GetFlag("menu_content_menu"); }
    public bool ShowMenuContentBaiViet() { return IsManagementSpaceContent() && GetFlag("menu_content_baiviet"); }
    public bool ShowMenuContentBanner() { return IsManagementSpaceContent() && GetFlag("menu_content_banner"); }
    public bool ShowMenuBatDongSanLinked() { return IsManagementSpaceBatDongSan() && GetFlag("menu_batdongsan_linked"); }
    public bool ShowMenuDauGiaWorkspace() { return IsManagementSpaceDauGia() && GetFlag("menu_daugia_workspace"); }
    public bool ShowMenuEventWorkspace() { return IsManagementSpaceEvent() && GetFlag("menu_event_workspace"); }

    public bool ShowMenuGroupAdmin()
    {
        return ShowMenuAdminDashboard() || ShowMenuAdminAccount() || ShowMenuAdminOtp() || ShowMenuAdminTokenWallet()
            || ShowMenuAdminGopY() || ShowMenuAdminThongBao() || ShowMenuAdminTuVan()
            || ShowMenuAdminCompanyShopSync() || ShowMenuAdminReindexBaiViet();
    }

    public bool ShowMenuGroupHomeSpace()
    {
        return ShowMenuTransferHistory() || ShowMenuHomeAccount() || ShowMenuApproveHanhVi()
            || ShowMenuIssueCard() || ShowMenuTierDescription() || ShowMenuSellProduct();
    }

    public bool ShowMenuGroupShop() { return ShowMenuShopWorkspace() || ShowMenuShopAccount() || ShowMenuShopLegacyInvoices() || ShowMenuShopLegacyCustomers() || ShowMenuShopLegacyContent() || ShowMenuShopLegacyFinance() || ShowMenuShopLegacyInventory() || ShowMenuShopLegacyAccounts() || ShowMenuShopLegacyOrg() || ShowMenuShopLegacyTraining() || ShowMenuShopLegacySupport() || ShowMenuGianHangApproval() || ShowMenuShopPartnerApproval() || ShowMenuShopLevel2() || ShowMenuShopEmailTemplate() || ShowMenuShopPointApproval(); }
    public bool ShowMenuGroupBatDongSan() { return ShowMenuBatDongSanLinked(); }
    public bool ShowMenuGroupContent() { return ShowMenuHomeContent() || ShowMenuHomeTextContent() || ShowMenuContentMenu() || ShowMenuContentBaiViet() || ShowMenuContentBanner(); }
    public bool ShowMenuGroupDauGia() { return ShowMenuDauGiaWorkspace(); }
    public bool ShowMenuGroupEvent() { return ShowMenuEventWorkspace(); }

    public bool ShowQuickCreateAccounts()
    {
        if (IsManagementSpaceAdmin())
            return ShowMenuAdminAccount();

        if (IsManagementSpaceHome())
            return ShowMenuHomeAccount();

        if (IsManagementSpaceGianHang())
            return ShowMenuShopAccount();

        return false;
    }

    public bool ShowQuickCreateOperations()
    {
        return IsManagementSpaceHome() && (ShowMenuIssueCard() || ShowMenuSellProduct());
    }

    public bool ShowQuickCreateContent()
    {
        return IsManagementSpaceContent() && (ShowMenuContentMenu() || ShowMenuContentBaiViet() || ShowMenuContentBanner());
    }

    public bool ShowQuickCreateMenu()
    {
        return ShowQuickCreateAccounts() || ShowQuickCreateOperations() || ShowQuickCreateContent();
    }

    public string GetAdminHomeUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
                if (account != null)
                    return AdminManagementSpace_cl.ResolveCurrentLandingUrl(db, account, Request);
            }
        }

        return "/admin/login.aspx";
    }

    private string GetScopedUrl(string url, string space)
    {
        return AdminManagementSpace_cl.AppendSpaceToUrl(url, space);
    }

    private string GetFeatureUrl(string featureKey, string space, string fallbackUrl)
    {
        string cacheKey = string.Format("{0}|{1}|{2}", featureKey ?? "", space ?? "", fallbackUrl ?? "");
        string cachedUrl;
        if (_featureUrlCache.TryGetValue(cacheKey, out cachedUrl))
            return cachedUrl;

        string resolvedUrl = "";
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                resolvedUrl = AdminRolePolicy_cl.ResolveFeatureUrl(db, taiKhoan, featureKey);
            }
        }

        if (string.IsNullOrWhiteSpace(resolvedUrl))
        {
            AdminFeatureRegistry_cl.FeatureDefinition definition = AdminFeatureRegistry_cl.Get(featureKey);
            if (definition != null)
                resolvedUrl = definition.DefaultUrl ?? "";
        }

        if (string.IsNullOrWhiteSpace(resolvedUrl))
            resolvedUrl = fallbackUrl ?? "";

        string finalUrl = GetScopedUrl(resolvedUrl, space);
        _featureUrlCache[cacheKey] = finalUrl;
        return finalUrl;
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

    public string GetAdminDashboardUrl()
    {
        return GetFeatureUrl("admin_dashboard", AdminManagementSpace_cl.SpaceAdmin, "/admin/default.aspx");
    }

    public string GetAdminOtpUrl()
    {
        return GetFeatureUrl("admin_otp", AdminManagementSpace_cl.SpaceAdmin, "/admin/otp/default.aspx");
    }

    public string GetAdminTokenWalletUrl()
    {
        return GetFeatureUrl("token_wallet", AdminManagementSpace_cl.SpaceAdmin, "/admin/vi-token-diem/default.aspx");
    }

    public string GetAdminGopYUrl()
    {
        return GetFeatureUrl("admin_feedback", AdminManagementSpace_cl.SpaceAdmin, "/admin/quan-ly-gop-y/default.aspx");
    }

    public string GetAdminThongBaoUrl()
    {
        return GetFeatureUrl("notifications", AdminManagementSpace_cl.SpaceAdmin, "/admin/quan-ly-thong-bao/default.aspx");
    }

    public string GetAdminTuVanUrl()
    {
        return GetFeatureUrl("consulting", AdminManagementSpace_cl.SpaceAdmin, "/admin/yeu-cau-tu-van/default.aspx");
    }

    public string GetAdminCompanyShopSyncUrl()
    {
        return GetFeatureUrl("admin_company_shop_sync", AdminManagementSpace_cl.SpaceAdmin, "/admin/tools/company-shop-sync.aspx");
    }

    public string GetAdminReindexBaiVietUrl()
    {
        return GetFeatureUrl("admin_reindex_baiviet", AdminManagementSpace_cl.SpaceAdmin, "/admin/tools/reindex-baiviet.aspx");
    }

    public string GetAdminAccountCreateUrl()
    {
        return GetScopedUrl("/admin/quan-ly-tai-khoan/them-moi.aspx?scope=admin&fscope=admin", AdminManagementSpace_cl.SpaceAdmin);
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

    public string GetShopWorkspaceUrl()
    {
        return GetFeatureUrl("shop_workspace", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/default.aspx?system=1");
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
        return GetFeatureUrl("daugia_workspace", AdminManagementSpace_cl.SpaceDauGia, "/daugia/admin/default.aspx?system=1");
    }

    public string GetEventAdminUrl()
    {
        return GetFeatureUrl("event_workspace", AdminManagementSpace_cl.SpaceEvent, "/event/admin/default.aspx?system=1");
    }

    public string GetHomeAccountCreateUrl()
    {
        return GetScopedUrl("/admin/quan-ly-tai-khoan/them-moi.aspx?scope=home&fscope=home", AdminManagementSpace_cl.SpaceHome);
    }

    public string GetShopAccountCreateUrl()
    {
        return GetScopedUrl("/admin/quan-ly-tai-khoan/them-moi.aspx?scope=shop&fscope=shop&frole=shop_partner", AdminManagementSpace_cl.SpaceGianHang);
    }

    public string GetHomeTransferHistoryUrl() { return GetFeatureUrl("core_assets", AdminManagementSpace_cl.SpaceHome, "/admin/lich-su-chuyen-diem/default.aspx"); }
    public string GetHomeIssueCardUrl() { return GetFeatureUrl("issue_cards", AdminManagementSpace_cl.SpaceHome, "/admin/phat-hanh-the/them-moi.aspx"); }
    public string GetHomeTierDescriptionUrl() { return GetFeatureUrl("tier_reference", AdminManagementSpace_cl.SpaceHome, "/admin/MoTaCapBac.aspx"); }
    public string GetHomeSellProductUrl() { return GetFeatureUrl("system_products", AdminManagementSpace_cl.SpaceHome, "/admin/he-thong-san-pham/ban-the.aspx"); }
    public string GetShopPointApprovalUrl() { return GetFeatureUrl("shop_points", AdminManagementSpace_cl.SpaceGianHang, "/admin/lich-su-chuyen-diem/default.aspx?tab=shop-only"); }
    public string GetShopEmailTemplateUrl() { return GetFeatureUrl("shop_email", AdminManagementSpace_cl.SpaceGianHang, "/admin/quan-ly-email-shop/default.aspx"); }
    public string GetShopLegacyInvoicesUrl() { return GetFeatureUrl("shop_legacy_invoices", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang&system=1"); }
    public string GetShopLegacyCustomersUrl() { return GetFeatureUrl("shop_legacy_customers", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-khach-hang/Default.aspx?system=1"); }
    public string GetShopLegacyContentUrl() { return GetFeatureUrl("shop_legacy_content", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-bai-viet/Default.aspx?system=1"); }
    public string GetShopLegacyFinanceUrl() { return GetFeatureUrl("shop_legacy_finance", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-thu-chi/Default.aspx?system=1"); }
    public string GetShopLegacyInventoryUrl() { return GetFeatureUrl("shop_legacy_inventory", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-kho-hang/Default.aspx?system=1"); }
    public string GetShopLegacyAccountsUrl() { return GetFeatureUrl("shop_legacy_accounts", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-tai-khoan/Default.aspx?system=1"); }
    public string GetShopLegacyOrgUrl() { return GetFeatureUrl("shop_legacy_org", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-he-thong/chi-nhanh.aspx?system=1"); }
    public string GetShopLegacyTrainingUrl() { return GetFeatureUrl("shop_legacy_training", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-hoc-vien/Default.aspx?system=1"); }
    public string GetShopLegacySupportUrl() { return GetFeatureUrl("shop_legacy_support", AdminManagementSpace_cl.SpaceGianHang, "/gianhang/admin/quan-ly-thong-bao/Default.aspx?system=1"); }
    public string GetGianHangApprovalUrl() { return GetFeatureUrl("home_gianhang_space", AdminManagementSpace_cl.SpaceGianHang, "/admin/duyet-gian-hang-doi-tac.aspx"); }
    public string GetShopPartnerApprovalUrl() { return GetFeatureUrl("shop_partner", AdminManagementSpace_cl.SpaceGianHang, "/admin/duyet-shop-doi-tac.aspx"); }
    public string GetShopLevel2ApprovalUrl() { return GetFeatureUrl("shop_level2", AdminManagementSpace_cl.SpaceGianHang, "/admin/duyet-nang-cap-level2.aspx"); }
    public string GetContentSettingsUrl() { return GetFeatureUrl("home_config", AdminManagementSpace_cl.SpaceContent, "/admin/cai-dat-trang-chu/default.aspx"); }
    public string GetContentHomeTextUrl() { return GetFeatureUrl("home_content", AdminManagementSpace_cl.SpaceContent, "/admin/quan-ly-noi-dung-home/default.aspx"); }
    public string GetBatDongSanWorkspaceUrl() { return GetScopedUrl("/admin/default.aspx", AdminManagementSpace_cl.SpaceBatDongSan); }
    public string GetBatDongSanLinkedUrl() { return GetFeatureUrl("home_bds_linked", AdminManagementSpace_cl.SpaceBatDongSan, "/admin/quan-ly-noi-dung-home/bds-lien-ket-tin.aspx"); }
    public string GetContentMenuUrl() { return GetFeatureUrl("home_menu", AdminManagementSpace_cl.SpaceContent, "/admin/quan-ly-menu/default.aspx"); }
    public string GetContentMenuCreateUrl() { return GetScopedUrl("/admin/quan-ly-menu/them-moi.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBaiVietUrl() { return GetFeatureUrl("home_posts", AdminManagementSpace_cl.SpaceContent, "/admin/quan-ly-bai-viet/default.aspx"); }
    public string GetContentBaiVietCreateUrl() { return GetScopedUrl("/admin/quan-ly-bai-viet/them-moi.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBannerUrl() { return GetFeatureUrl("home_banner", AdminManagementSpace_cl.SpaceContent, "/admin/quan-ly-banner/default.aspx"); }
    public string GetContentBannerCreateUrl() { return GetScopedUrl("/admin/quan-ly-banner/them-moi.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string MenuActive(params string[] urls)
    {
        return AdminRouteMap_cl.IsPathActive(HttpContext.Current.Request, urls) ? "active" : "";
    }

    public string MenuActiveTransferHistory()
    {
        return AdminRouteMap_cl.IsTransferHistoryActive(Request) ? "active" : "";
    }

    public string MenuActiveTokenWallet()
    {
        return AdminRouteMap_cl.IsTokenWalletActive(Request) ? "active" : "";
    }

    public string MenuActivePointApproval()
    {
        return AdminRouteMap_cl.IsPointApprovalActive(Request) ? "active" : "";
    }

    public string MenuActiveShopPointApproval()
    {
        return AdminRouteMap_cl.IsShopPointApprovalActive(Request) ? "active" : "";
    }

    public string MenuActiveTaiKhoanScope(string scope)
    {
        return AdminRouteMap_cl.IsAccountScopeActive(Request, scope) ? "active" : "";
    }

    public string MenuActiveFeature(params string[] featureKeys)
    {
        return AdminRouteMap_cl.IsFeatureActive(Request, featureKeys) ? "active" : "";
    }

    public string MenuActiveBatDongSanWorkspace()
    {
        return IsManagementSpaceBatDongSan() ? "active" : "";
    }

    private string GetCurrentAdminAccount()
    {
        string taiKhoanMaHoa = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(taiKhoanMaHoa))
            return "";

        return mahoa_cl.giaima_Bcorn(taiKhoanMaHoa);
    }

    protected string BuildCurrentPageUrl()
    {
        var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
        query.Remove("topview");
        query.Remove("return_url");
        string queryString = query.ToString();
        return Request.Url.AbsolutePath + (string.IsNullOrEmpty(queryString) ? "" : "?" + queryString);
    }

    private string BuildChangePasswordUrl()
    {
        string currentUrl = AdminFullPageRoute_cl.SanitizeAdminReturnUrl(BuildCurrentPageUrl(), "/admin/default.aspx");
        return ResolveUrl("~/admin/doi-mat-khau/default.aspx?return_url=" + HttpUtility.UrlEncode(currentUrl));
    }

    public string GetNotificationListUrl()
    {
        return ResolveUrl("~/admin/quan-ly-thong-bao/default.aspx");
    }

    private void RedirectTo(string url)
    {
        ScriptManager sm = ScriptManager.GetCurrent(Page);
        if (sm != null && sm.IsInAsyncPostBack)
        {
            string safeUrl = (url ?? "").Replace("'", "\\'");
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "window.location='" + safeUrl + "';", true);
            return;
        }

        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void ApplyAccountRouteLinks()
    {
        but_show_form_doimatkhau.NavigateUrl = BuildChangePasswordUrl();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region vô hiệu hóa timer trên một số trang có CKEditor
                ApplyMenuTimerSafe();
                #endregion


                ViewState["title"] = AdminRouteMap_cl.ResolveTitle(Request);

                ViewState["sapxep_thongbao"] = "1";//mặc định sx thông báo theo mới nhất lên đầu
                but_sapxep_moinhat.CssClass = "info small rounded";

                using (dbDataContext db = new dbDataContext())
                {
                    show_soluong_thongbao(db);
                    lay_thongtin_nguoidung(db);
                }

                ApplyAccountRouteLinks();

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

    private void ApplyMenuTimerSafe()
    {
        try
        {
            string url = (Request.Url != null ? Request.Url.AbsolutePath : "").ToLowerInvariant();
            bool disableTimer = url == "/admin/quan-ly-bai-viet/default.aspx"
                                || url == "/admin/motacapbac.aspx"
                                || url == "/admin/MoTaCapBac.aspx".ToLowerInvariant();

            Timer timer = FindControl("Timer1") as Timer;
            if (timer != null)
                timer.Enabled = !disableTimer;
        }
        catch
        {
            // ignore timer failures (Mono doesn't implement Timer.Visible)
        }
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                show_soluong_thongbao(db);
            }
        }
        catch
        {
            // ignore timer refresh failures
        }
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        string _tk = Session["taikhoan"] as string;
        if (!string.IsNullOrEmpty(_tk))
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);
            try
            {
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                if (q == null) return;

                ViewState["hoten"] = q.hoten;
                ViewState["anhdaidien"] = q.anhdaidien;
                ViewState["email"] = q.email;
                ViewState["taikhoan"] = _tk;
                ViewState["admin_role_label"] = AdminRolePolicy_cl.GetAdminRoleLabel(db, q.taikhoan, q.phanloai, q.permission);
                ViewState["admin_role_description"] = AdminRolePolicy_cl.GetAdminRoleDescription(db, q.taikhoan, q.phanloai, q.permission);
                string currentSpace = AdminManagementSpace_cl.GetCurrentSpace(db, q, Request);
                ViewState["admin_management_space"] = currentSpace;
                ViewState["admin_scope_label"] = AdminManagementSpace_cl.GetTitle(currentSpace);

                // Hiển thị nhãn theo hệ đăng nhập admin.
                if (AccountType_cl.IsTreasury(q.phanloai))
                {
                    ViewState["phanloai"] = "Tài khoản tổng";
                }
                else
                {
                    ViewState["phanloai"] = "Nhân viên admin";
                }


                ViewState["DongA"] = (q.DongA ?? 0).ToString("#,##0");
                BuildMenuPermissionFlags(db, q);
                // =============================
            }
            catch (Exception _ex)
            {
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }


    #region thông báo
    public void show_soluong_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                lb_sl_thongbao.Text = "0";
                return;
            }

            // Đếm số lượng thông báo chưa đọc
            int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == taiKhoan && p.daxem == false && p.bin == false);

            // Cập nhật nhãn hiển thị số lượng thông báo
            if (soLuongThongBaoChuaDoc < 100)
                lb_sl_thongbao.Text = soLuongThongBaoChuaDoc.ToString();
            else
                lb_sl_thongbao.Text = "99+";
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

    public void show_noidung_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                Repeater1.DataSource = new object[0];
                Repeater1.DataBind();
                ph_empty_thongbao.Visible = true;
                return;
            }

            var list_all = (from ob1 in db.ThongBao_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                            from ob2 in senderGroup.DefaultIfEmpty()
                            where ob1.nguoinhan == taiKhoan && ob1.bin == false
                            select new
                            {
                                ob1.id, // id thông báo
                                avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                                    ? "/uploads/images/macdinh.jpg"
                                    : ob2.anhdaidien,
                                daxem = ob1.daxem,
                                noidung = ob1.noidung ?? "",
                                thoigian = ob1.thoigian,
                                link = (ob1.link == null || ob1.link == "")
                                    ? "/admin/default.aspx?"
                                    : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                            }).AsQueryable();

            if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")//lọc ra chưa đọc, mới nhất lên đầu
                list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian).Take(20);
            else//sx theo mới nhất lên đầu
                list_all = list_all.OrderByDescending(p => p.thoigian).Take(20);

            var result = list_all.ToList();
            // Gán dữ liệu cho Repeater
            Repeater1.DataSource = result;
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = result.Count == 0;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            Repeater1.DataSource = new object[0];
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = true;
        }
    }
    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            ViewState["sapxep_thongbao"] = "1";
            but_sapxep_moinhat.CssClass = "info small rounded";
            but_sapxep_chuadoc.CssClass = "light small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
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

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            ViewState["sapxep_thongbao"] = "2";
            but_sapxep_moinhat.CssClass = "light small rounded";
            but_sapxep_chuadoc.CssClass = "info small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
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

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
            UpdatePanel2.Update();
            // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString()), "1000", "warning"), true);
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
    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = false;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
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

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
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
    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.bin = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
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
    #endregion

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
            Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "2000", "warning");
        Response.Redirect("/admin/login.aspx");
    }
    #region ĐỔI MẬT KHẨU
    #endregion 

}
