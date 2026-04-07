using System;

public static class AdminMenuPolicy_cl
{
    public sealed class MenuVisibility
    {
        public bool IsRoot { get; set; }
        public bool AdminDashboard { get; set; }
        public bool AdminAccount { get; set; }
        public bool AdminOtp { get; set; }
        public bool AdminTokenWallet { get; set; }
        public bool AdminGopY { get; set; }
        public bool AdminThongBao { get; set; }
        public bool AdminTuVan { get; set; }
        public bool AdminCompanyShopSync { get; set; }
        public bool AdminReindexBaiViet { get; set; }
        public bool TransferHistory { get; set; }
        public bool HomeAccount { get; set; }
        public bool ApproveHanhVi { get; set; }
        public bool IssueCard { get; set; }
        public bool TierDescription { get; set; }
        public bool SellProduct { get; set; }
        public bool ShopWorkspace { get; set; }
        public bool ShopAccount { get; set; }
        public bool ShopLegacyInvoices { get; set; }
        public bool ShopLegacyCustomers { get; set; }
        public bool ShopLegacyContent { get; set; }
        public bool ShopLegacyFinance { get; set; }
        public bool ShopLegacyInventory { get; set; }
        public bool ShopLegacyAccounts { get; set; }
        public bool ShopLegacyOrg { get; set; }
        public bool ShopLegacyTraining { get; set; }
        public bool ShopLegacySupport { get; set; }
        public bool HomeGianHangApproval { get; set; }
        public bool ShopPartnerApproval { get; set; }
        public bool ShopLevel2 { get; set; }
        public bool ShopEmailTemplate { get; set; }
        public bool ShopPointApproval { get; set; }
        public bool HomeContent { get; set; }
        public bool HomeTextContent { get; set; }
        public bool ContentMenu { get; set; }
        public bool ContentBaiViet { get; set; }
        public bool ContentBanner { get; set; }
        public bool BatDongSanLinked { get; set; }
        public bool DauGiaWorkspace { get; set; }
        public bool EventWorkspace { get; set; }
    }

    public static MenuVisibility Build(dbDataContext db, taikhoan_tb account, bool showLegacySystemProductMenu)
    {
        var model = new MenuVisibility();
        string tk = account != null ? (account.taikhoan ?? "").Trim() : "";
        if (account == null || string.IsNullOrWhiteSpace(tk))
            return model;

        model.IsRoot = AdminRolePolicy_cl.IsSuperAdmin(tk);
        string phanloai = account.phanloai ?? "";
        string permissionRaw = account.permission ?? "";
        var tabKeys = AdminRolePolicy_cl.BuildAdminHomeTabKeySet(db, tk, phanloai, permissionRaw);

        Func<string, bool> hasKey = key => tabKeys.Contains((key ?? "").Trim());

        model.AdminDashboard = hasKey("admin_dashboard");
        model.AdminAccount = hasKey("admin_accounts");
        model.AdminOtp = hasKey("admin_otp");
        model.AdminTokenWallet = hasKey("token_wallet");
        model.AdminGopY = hasKey("admin_feedback");
        model.AdminThongBao = hasKey("notifications");
        model.AdminTuVan = hasKey("consulting");
        model.AdminCompanyShopSync = hasKey("admin_company_shop_sync");
        model.AdminReindexBaiViet = hasKey("admin_reindex_baiviet");
        model.TransferHistory = hasKey("core_assets");
        model.HomeAccount = hasKey("home_accounts");
        model.ApproveHanhVi = hasKey("home_point_approval")
            || hasKey("home_points_customer")
            || hasKey("home_points_development")
            || hasKey("home_points_ecosystem");
        model.IssueCard = showLegacySystemProductMenu && hasKey("issue_cards");
        model.TierDescription = hasKey("tier_reference");
        model.SellProduct = showLegacySystemProductMenu && hasKey("system_products");
        model.ShopWorkspace = hasKey("shop_workspace");
        model.ShopAccount = hasKey("shop_accounts");
        model.ShopLegacyInvoices = hasKey("shop_legacy_invoices");
        model.ShopLegacyCustomers = hasKey("shop_legacy_customers");
        model.ShopLegacyContent = hasKey("shop_legacy_content");
        model.ShopLegacyFinance = hasKey("shop_legacy_finance");
        model.ShopLegacyInventory = hasKey("shop_legacy_inventory");
        model.ShopLegacyAccounts = hasKey("shop_legacy_accounts");
        model.ShopLegacyOrg = hasKey("shop_legacy_org");
        model.ShopLegacyTraining = hasKey("shop_legacy_training");
        model.ShopLegacySupport = hasKey("shop_legacy_support");
        model.HomeGianHangApproval = hasKey("home_gianhang_space");
        model.ShopPartnerApproval = hasKey("shop_partner");
        model.ShopLevel2 = hasKey("shop_level2");
        model.ShopEmailTemplate = hasKey("shop_email");
        model.ShopPointApproval = hasKey("shop_points");
        model.HomeContent = hasKey("home_config");
        model.HomeTextContent = hasKey("home_content");
        model.ContentMenu = hasKey("home_menu");
        model.ContentBaiViet = hasKey("home_posts");
        model.ContentBanner = hasKey("home_banner");
        model.BatDongSanLinked = hasKey("home_bds_linked");
        model.DauGiaWorkspace = hasKey("daugia_workspace");
        model.EventWorkspace = hasKey("event_workspace");

        return model;
    }
}
