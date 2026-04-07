using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangAdminMenuPolicy_cl
{
    public sealed class MenuVisibility
    {
        public bool IsRoot { get; set; }
        public bool Dashboard { get; set; }
        public bool AdminAccount { get; set; }
        public bool Otp { get; set; }
        public bool TransferHistory { get; set; }
        public bool HomeAccount { get; set; }
        public bool ApproveHanhVi { get; set; }
        public bool IssueCard { get; set; }
        public bool TierDescription { get; set; }
        public bool SellProduct { get; set; }
        public bool ShopAccount { get; set; }
        public bool ShopApprove { get; set; }
        public bool ShopEmailTemplate { get; set; }
        public bool ContentHome { get; set; }
        public bool ContentHomeText { get; set; }
        public bool ContentMenu { get; set; }
        public bool ContentBaiViet { get; set; }
        public bool ContentBanner { get; set; }
        public bool ContentGopY { get; set; }
        public bool ContentThongBao { get; set; }
        public bool ContentTuVan { get; set; }
        public bool GroupAdmin { get; set; }
        public bool GroupHome { get; set; }
        public bool GroupShop { get; set; }
        public bool GroupContent { get; set; }
    }

    private const string PermissionManageAdminAccounts = "5";
    private const string PermissionLegacyGeneralAdmin = "1";
    private const string PermissionHomeContent = "q3_1";

    private static HashSet<string> ParsePermissionTokens(string permissionRaw)
    {
        var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(permissionRaw))
            return tokens;

        string[] arr = permissionRaw.Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string token in arr)
        {
            string t = (token ?? "").Trim();
            if (t != "")
                tokens.Add(t);
        }
        return tokens;
    }

    public static MenuVisibility Build(taikhoan_tb account, bool includeOtp)
    {
        var model = new MenuVisibility();
        bool isRoot = account != null && PermissionProfile_cl.IsRootAdmin(account.taikhoan);
        model.IsRoot = isRoot;
        if (isRoot)
        {
            model.Dashboard = true;
            model.AdminAccount = true;
            model.Otp = includeOtp;
            model.TransferHistory = true;
            model.HomeAccount = true;
            model.ApproveHanhVi = true;
            model.IssueCard = true;
            model.TierDescription = true;
            model.SellProduct = true;
            model.ShopAccount = true;
            model.ShopApprove = true;
            model.ShopEmailTemplate = true;
            model.ContentHome = true;
            model.ContentHomeText = true;
            model.ContentMenu = true;
            model.ContentBaiViet = true;
            model.ContentBanner = true;
            model.ContentGopY = true;
            model.ContentThongBao = true;
            model.ContentTuVan = true;
            model.GroupAdmin = true;
            model.GroupHome = true;
            model.GroupShop = true;
            model.GroupContent = true;
            return model;
        }

        HashSet<string> tokens = ParsePermissionTokens(account != null ? account.permission : "");
        bool legacyGeneral = tokens.Contains(PermissionLegacyGeneralAdmin);
        bool canManageAdminAccounts = tokens.Contains(PermissionManageAdminAccounts);
        bool canLegacyTransfer = PermissionProfile_cl.LegacyTieuDungPermissions.Any(code => tokens.Contains(code));
        bool canTieuDung = tokens.Contains(PermissionProfile_cl.HoSoTieuDung);
        bool canUuDai = tokens.Contains(PermissionProfile_cl.HoSoUuDai);
        bool canLaoDong = tokens.Contains(PermissionProfile_cl.HoSoLaoDong);
        bool canGanKet = tokens.Contains(PermissionProfile_cl.HoSoGanKet);
        bool canShopOnly = tokens.Contains(PermissionProfile_cl.HoSoShopOnly);
        bool canHomeContent = tokens.Contains(PermissionHomeContent);

        bool canApproveHanhVi = canUuDai || canLaoDong || canGanKet;
        bool canHomeAccount = canTieuDung || canApproveHanhVi;
        bool canTransferHistory = canLegacyTransfer || canTieuDung;
        bool showOtherContent = legacyGeneral || canHomeContent;

        model.Dashboard = false;
        model.AdminAccount = canManageAdminAccounts;
        model.Otp = includeOtp && legacyGeneral;
        model.TransferHistory = legacyGeneral || canTransferHistory;
        model.HomeAccount = legacyGeneral || canHomeAccount;
        model.ApproveHanhVi = legacyGeneral || canApproveHanhVi;
        model.IssueCard = legacyGeneral || canTieuDung;
        model.TierDescription = legacyGeneral || canApproveHanhVi;
        model.SellProduct = legacyGeneral || canTieuDung;
        model.ShopAccount = legacyGeneral || canShopOnly;
        model.ShopApprove = legacyGeneral || canShopOnly;
        model.ShopEmailTemplate = legacyGeneral || canShopOnly;
        model.ContentHome = false;
        model.ContentHomeText = showOtherContent;
        model.ContentMenu = showOtherContent;
        model.ContentBaiViet = showOtherContent;
        model.ContentBanner = showOtherContent;
        model.ContentGopY = showOtherContent;
        model.ContentThongBao = showOtherContent;
        model.ContentTuVan = showOtherContent;
        model.GroupAdmin = model.Dashboard || model.AdminAccount || model.Otp || model.TransferHistory;
        model.GroupHome = model.HomeAccount || model.ApproveHanhVi || model.IssueCard || model.TierDescription || model.SellProduct;
        model.GroupShop = model.ShopAccount || model.ShopApprove || model.ShopEmailTemplate;
        model.GroupContent = model.ContentHome || showOtherContent;
        return model;
    }
}
