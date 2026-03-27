using System;
using System.Linq;
using System.Web.UI;

public static class DauGiaPortalAccess_cl
{
    public static string EnsureSellerManagementAccess(Page page, dbDataContext db, string fallbackReturnUrl)
    {
        if (page == null || page.Response == null)
            return "";

        string returnUrl = page.Request == null
            ? (string.IsNullOrWhiteSpace(fallbackReturnUrl) ? "/daugia/admin" : fallbackReturnUrl)
            : (page.Request.RawUrl ?? (string.IsNullOrWhiteSpace(fallbackReturnUrl) ? "/daugia/admin" : fallbackReturnUrl));

        bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
        string accountKey = NormalizeAccount(PortalRequest_cl.GetCurrentAccount());
        if (accountKey == "")
        {
            if (isShopPortal)
            {
                if (page.Session != null)
                {
                    page.Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}",
                        "Thông báo",
                        "Vui lòng đăng nhập để quản lý đấu giá.",
                        "warning",
                        "1200");
                }

                page.Response.Redirect(check_login_cl.BuildShopLoginUrl(returnUrl), true);
                return "";
            }

            HomeSpaceAccess_cl.RedirectToHomeLogin(
                page,
                returnUrl,
                "Vui lòng đăng nhập tài khoản Home để quản lý đấu giá.");
            return "";
        }

        // Shop portal quản lý bằng phiên shop hiện có, không ép quyền HomeSpace.
        if (isShopPortal)
            return accountKey;

        taikhoan_tb homeAccount = db == null
            ? null
            : db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == accountKey);

        if (!SpaceAccess_cl.CanAccessDauGia(db, homeAccount))
        {
            HomeSpaceAccess_cl.RedirectToAccessPage(page, ModuleSpace_cl.DauGia, returnUrl);
            return "";
        }

        return accountKey;
    }

    public static bool CanSellerAccessInPortal(dbDataContext db, string sellerAccount, bool isShopPortal)
    {
        string accountKey = NormalizeAccount(sellerAccount);
        if (accountKey == "")
            return false;

        if (isShopPortal)
            return true;

        taikhoan_tb homeAccount = db == null
            ? null
            : db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == accountKey);
        return SpaceAccess_cl.CanAccessDauGia(db, homeAccount);
    }

    private static string NormalizeAccount(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
