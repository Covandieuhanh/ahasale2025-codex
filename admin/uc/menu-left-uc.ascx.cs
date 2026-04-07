using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_left_uc : System.Web.UI.UserControl
{
    public string loi, tuvan;
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

    private void BuildLeftMenuPermissionFlags(dbDataContext db, taikhoan_tb account)
    {
        bool showLegacySystemProductMenu = !CompanyShop_cl.HideLegacyAdminSystemProduct();
        AdminMenuPolicy_cl.MenuVisibility visibility = AdminMenuPolicy_cl.Build(db, account, showLegacySystemProductMenu);

        SetFlag("left_is_root", visibility.IsRoot);
        SetFlag("left_admin_dashboard", visibility.AdminDashboard);
        SetFlag("left_admin_account", visibility.AdminAccount);
        SetFlag("left_admin_otp", visibility.AdminOtp);
        SetFlag("left_admin_token_wallet", visibility.AdminTokenWallet);
        SetFlag("left_admin_gopy", visibility.AdminGopY);
        SetFlag("left_admin_thongbao", visibility.AdminThongBao);
        SetFlag("left_admin_tuvan", visibility.AdminTuVan);
        SetFlag("left_admin_company_shop_sync", visibility.AdminCompanyShopSync);
        SetFlag("left_admin_reindex_baiviet", visibility.AdminReindexBaiViet);
        SetFlag("left_transfer_history", visibility.TransferHistory);
        SetFlag("left_home_account", visibility.HomeAccount);
        SetFlag("left_home_approve_hanhvi", visibility.ApproveHanhVi);
        SetFlag("left_home_issue_card", visibility.IssueCard);
        SetFlag("left_home_tier_desc", visibility.TierDescription);
        SetFlag("left_home_sell_product", visibility.SellProduct);
        SetFlag("left_shop_workspace", visibility.ShopWorkspace);
        SetFlag("left_shop_account", visibility.ShopAccount);
        SetFlag("left_shop_legacy_invoices", visibility.ShopLegacyInvoices);
        SetFlag("left_shop_legacy_customers", visibility.ShopLegacyCustomers);
        SetFlag("left_shop_legacy_content", visibility.ShopLegacyContent);
        SetFlag("left_shop_legacy_finance", visibility.ShopLegacyFinance);
        SetFlag("left_shop_legacy_inventory", visibility.ShopLegacyInventory);
        SetFlag("left_shop_legacy_accounts", visibility.ShopLegacyAccounts);
        SetFlag("left_shop_legacy_org", visibility.ShopLegacyOrg);
        SetFlag("left_shop_legacy_training", visibility.ShopLegacyTraining);
        SetFlag("left_shop_legacy_support", visibility.ShopLegacySupport);
        SetFlag("left_shop_approve_home_space", visibility.HomeGianHangApproval);
        SetFlag("left_shop_approve_partner", visibility.ShopPartnerApproval);
        SetFlag("left_shop_level2", visibility.ShopLevel2);
        SetFlag("left_shop_email_template", visibility.ShopEmailTemplate);
        SetFlag("left_shop_point_approval", visibility.ShopPointApproval);
        SetFlag("left_content_home", visibility.HomeContent);
        SetFlag("left_content_home_text", visibility.HomeTextContent);
        SetFlag("left_batdongsan_linked", visibility.BatDongSanLinked);
        SetFlag("left_content_menu", visibility.ContentMenu);
        SetFlag("left_content_baiviet", visibility.ContentBaiViet);
        SetFlag("left_content_banner", visibility.ContentBanner);
        SetFlag("left_daugia_workspace", visibility.DauGiaWorkspace);
        SetFlag("left_event_workspace", visibility.EventWorkspace);

        // Super Admin luôn nhìn thấy đầy đủ tab trong mọi không gian quản trị.
        if (visibility.IsRoot)
        {
            string[] allFlagKeys =
            {
                "left_admin_dashboard",
                "left_admin_account",
                "left_admin_otp",
                "left_admin_token_wallet",
                "left_admin_gopy",
                "left_admin_thongbao",
                "left_admin_tuvan",
                "left_admin_company_shop_sync",
                "left_admin_reindex_baiviet",
                "left_transfer_history",
                "left_home_account",
                "left_home_approve_hanhvi",
                "left_home_issue_card",
                "left_home_tier_desc",
                "left_home_sell_product",
                "left_shop_workspace",
                "left_shop_account",
                "left_shop_legacy_invoices",
                "left_shop_legacy_customers",
                "left_shop_legacy_content",
                "left_shop_legacy_finance",
                "left_shop_legacy_inventory",
                "left_shop_legacy_accounts",
                "left_shop_legacy_org",
                "left_shop_legacy_training",
                "left_shop_legacy_support",
                "left_shop_approve_home_space",
                "left_shop_approve_partner",
                "left_shop_level2",
                "left_shop_email_template",
                "left_shop_point_approval",
                "left_content_home",
                "left_content_home_text",
                "left_batdongsan_linked",
                "left_content_menu",
                "left_content_baiviet",
                "left_content_banner",
                "left_daugia_workspace",
                "left_event_workspace"
            };

            foreach (string key in allFlagKeys)
                SetFlag(key, true);
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
    public bool ShowLeftShopLegacyInvoices() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_invoices"); }
    public bool ShowLeftShopLegacyCustomers() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_customers"); }
    public bool ShowLeftShopLegacyContent() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_content"); }
    public bool ShowLeftShopLegacyFinance() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_finance"); }
    public bool ShowLeftShopLegacyInventory() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_inventory"); }
    public bool ShowLeftShopLegacyAccounts() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_accounts"); }
    public bool ShowLeftShopLegacyOrg() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_org"); }
    public bool ShowLeftShopLegacyTraining() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_training"); }
    public bool ShowLeftShopLegacySupport() { return IsManagementSpaceGianHang() && GetFlag("left_shop_legacy_support"); }
    public bool ShowLeftGianHangApproval() { return IsManagementSpaceGianHang() && GetFlag("left_shop_approve_home_space"); }
    public bool ShowLeftShopPartnerApproval() { return IsManagementSpaceGianHang() && GetFlag("left_shop_approve_partner"); }
    public bool ShowLeftShopLevel2() { return IsManagementSpaceGianHang() && GetFlag("left_shop_level2"); }
    public bool ShowLeftShopEmailTemplate() { return IsManagementSpaceGianHang() && GetFlag("left_shop_email_template"); }
    public bool ShowLeftShopPointApproval() { return IsManagementSpaceGianHang() && GetFlag("left_shop_point_approval"); }
    public bool ShowLeftBatDongSanWorkspace() { return IsManagementSpaceBatDongSan() && GetFlag("left_batdongsan_linked"); }
    public bool ShowLeftContentMenu() { return IsManagementSpaceContent() && GetFlag("left_content_menu"); }
    public bool ShowLeftContentBaiViet() { return IsManagementSpaceContent() && GetFlag("left_content_baiviet"); }
    public bool ShowLeftContentBanner() { return IsManagementSpaceContent() && GetFlag("left_content_banner"); }
    public bool ShowLeftBatDongSanLinked() { return IsManagementSpaceBatDongSan() && GetFlag("left_batdongsan_linked"); }
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

    public bool ShowLeftGroupShop() { return ShowLeftShopWorkspace() || ShowLeftShopAccount() || ShowLeftShopLegacyInvoices() || ShowLeftShopLegacyCustomers() || ShowLeftShopLegacyContent() || ShowLeftShopLegacyFinance() || ShowLeftShopLegacyInventory() || ShowLeftShopLegacyAccounts() || ShowLeftShopLegacyOrg() || ShowLeftShopLegacyTraining() || ShowLeftShopLegacySupport() || ShowLeftGianHangApproval() || ShowLeftShopPartnerApproval() || ShowLeftShopLevel2() || ShowLeftShopEmailTemplate() || ShowLeftShopPointApproval(); }
    public bool ShowLeftGroupBatDongSan() { return ShowLeftBatDongSanLinked(); }
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
        return GetFeatureUrl("admin_dashboard", AdminManagementSpace_cl.SpaceAdmin, "/admin/default.aspx");
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
        return GetFeatureUrl("admin_otp", AdminManagementSpace_cl.SpaceAdmin, "/admin/otp/default.aspx");
    }

    public string GetAdminTokenWalletUrl()
    {
        return GetFeatureUrl("token_wallet", AdminManagementSpace_cl.SpaceAdmin, "/admin/vi-token-diem/default.aspx");
    }

    public string GetAdminGopYUrl() { return GetFeatureUrl("admin_feedback", AdminManagementSpace_cl.SpaceAdmin, "/admin/quan-ly-gop-y/default.aspx"); }
    public string GetAdminThongBaoUrl() { return GetFeatureUrl("notifications", AdminManagementSpace_cl.SpaceAdmin, "/admin/quan-ly-thong-bao/default.aspx"); }
    public string GetAdminTuVanUrl() { return GetFeatureUrl("consulting", AdminManagementSpace_cl.SpaceAdmin, "/admin/yeu-cau-tu-van/default.aspx"); }
    public string GetAdminCompanyShopSyncUrl() { return GetFeatureUrl("admin_company_shop_sync", AdminManagementSpace_cl.SpaceAdmin, "/admin/tools/company-shop-sync.aspx"); }
    public string GetAdminReindexBaiVietUrl() { return GetFeatureUrl("admin_reindex_baiviet", AdminManagementSpace_cl.SpaceAdmin, "/admin/tools/reindex-baiviet.aspx"); }

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
    public string GetContentBaiVietUrl() { return GetFeatureUrl("home_posts", AdminManagementSpace_cl.SpaceContent, "/admin/quan-ly-bai-viet/default.aspx"); }
    public string GetContentBannerUrl() { return GetFeatureUrl("home_banner", AdminManagementSpace_cl.SpaceContent, "/admin/quan-ly-banner/default.aspx"); }
    public string MenuActive(params string[] urls)
    {
        return AdminRouteMap_cl.IsPathActive(HttpContext.Current.Request, urls) ? "active" : "";
    }

    public string MenuActiveTransferHistory()
    {
        return AdminRouteMap_cl.IsTransferHistoryActive(Request) ? "active" : "";
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

    public string MenuActiveTokenWallet()
    {
        return AdminRouteMap_cl.IsTokenWalletActive(Request) ? "active" : "";
    }

    public string MenuActiveFeature(params string[] featureKeys)
    {
        return AdminRouteMap_cl.IsFeatureActive(Request, featureKeys) ? "active" : "";
    }

    public string MenuActiveBatDongSanWorkspace()
    {
        return IsManagementSpaceBatDongSan() ? "active" : "";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                Session["title"] = AdminRouteMap_cl.ResolveTitle(Request);

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
                    BuildLeftMenuPermissionFlags(db, account);
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
