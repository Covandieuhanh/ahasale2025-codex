using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public static class AdvancedAdminOwnerGuard_cl
{
    public static string ResolveOwnerAdminUsername(string userParent)
    {
        return ShopLevel_cl.ResolveOwnerAdminUsername(userParent);
    }

    public static bool IsProtectedOwner(string targetUser, string userParent)
    {
        string target = (targetUser ?? "").Trim().ToLowerInvariant();
        string owner = ResolveOwnerAdminUsername(userParent);
        return !string.IsNullOrWhiteSpace(target)
            && !string.IsNullOrWhiteSpace(owner)
            && string.Equals(target, owner, StringComparison.OrdinalIgnoreCase);
    }

    public static bool CanManageProtectedOwner(string currentAdminUser, string userParent)
    {
        string adminUser = (currentAdminUser ?? "").Trim().ToLowerInvariant();
        if (string.Equals(adminUser, "admin", StringComparison.OrdinalIgnoreCase))
            return true;

        return string.Equals(adminUser, ResolveOwnerAdminUsername(userParent), StringComparison.OrdinalIgnoreCase);
    }

    public static bool EnsureCanManageTarget(Page page, string currentAdminUser, string targetUser, string userParent, string actionLabel, string redirectUrl)
    {
        if (!IsProtectedOwner(targetUser, userParent))
            return true;

        if (CanManageProtectedOwner(currentAdminUser, userParent))
            return true;

        HttpContext context = HttpContext.Current;
        if (context != null)
        {
            string label = string.IsNullOrWhiteSpace(actionLabel) ? "thực hiện thao tác này" : actionLabel.Trim().ToLowerInvariant();
            context.Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Chỉ owner admin mới được phép " + label + ".", "false", "false", "OK", "alert", "");
            string url = string.IsNullOrWhiteSpace(redirectUrl) ? "/gianhang/admin" : redirectUrl;
            page.Response.Redirect(url, false);
            if (context.ApplicationInstance != null)
                context.ApplicationInstance.CompleteRequest();
        }
        return false;
    }

    public static bool EnsureOwnerOnly(Page page, string currentAdminUser, string userParent, string actionLabel, string redirectUrl)
    {
        string adminUser = (currentAdminUser ?? "").Trim().ToLowerInvariant();
        if (string.Equals(adminUser, "admin", StringComparison.OrdinalIgnoreCase))
            return true;

        dbDataContext db = new dbDataContext();
        string ownerShop = ShopLevel_cl.ResolveOwnerShopAccountForAdmin(db, adminUser, userParent);
        bool isOwner = !string.IsNullOrWhiteSpace(ownerShop)
            && string.Equals(adminUser, ownerShop, StringComparison.OrdinalIgnoreCase);
        if (isOwner)
            return true;

        HttpContext context = HttpContext.Current;
        if (context != null)
        {
            string label = string.IsNullOrWhiteSpace(actionLabel) ? "truy cập khu vực này" : actionLabel.Trim().ToLowerInvariant();
            context.Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Chỉ chủ shop mới được phép " + label + ".", "false", "false", "OK", "alert", "");
            string url = string.IsNullOrWhiteSpace(redirectUrl) ? "/gianhang/admin" : redirectUrl;
            page.Response.Redirect(url, false);
            if (context.ApplicationInstance != null)
                context.ApplicationInstance.CompleteRequest();
        }

        return false;
    }
}
