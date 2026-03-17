using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public static class AdvancedAdminAccessGate_cl
{
    private static void Redirect(Page page, string url)
    {
        HttpContext context = HttpContext.Current;
        if (page == null || page.Response == null)
            return;

        page.Response.Redirect(url, false);
        if (context != null && context.ApplicationInstance != null)
            context.ApplicationInstance.CompleteRequest();
    }

    public static bool EnsurePageAccess(Page page)
    {
        if (page == null || HttpContext.Current == null)
            return false;

        HttpContext context = HttpContext.Current;
        if (!host_compat_policy_class.is_allowed_request_host(context.Request))
        {
            Redirect(page, "/");
            return false;
        }

        string contextWarning;
        if (!AhaShineContext_cl.TryEnsureContext(out contextWarning))
        {
            if (context.Session["notifi"] == null || string.IsNullOrWhiteSpace((context.Session["notifi"] ?? "").ToString()))
                context.Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", contextWarning, "2200", "warning");
        }

        string adminUser = ((context.Session["user"] ?? "").ToString() ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(adminUser))
        {
            Redirect(page, "/gianhang/admin/f5_ss_admin.aspx");
            return false;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_table_2023 adminAccount = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == adminUser);
            string userParentHint = adminAccount == null ? AhaShineContext_cl.UserParent : adminAccount.user_parent;
            if (!ShopLevel_cl.CanUseAdvancedAdmin(db, adminUser, userParentHint))
            {
                context.Session["user"] = "";
                context.Session["chinhanh"] = "";
                context.Session["nganh"] = "";
                context.Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Shop này đang ở Level 1 nên chưa dùng được bộ công cụ /gianhang/admin.", "2200", "warning");
                Redirect(page, "/gianhang/admin/login.aspx");
                return false;
            }

            string deniedLabel;
            if (!AdvancedAdminPermission_cl.CanAccessRoute(adminUser, context.Request.Url.AbsolutePath, out deniedLabel))
            {
                string moduleLabel = string.IsNullOrWhiteSpace(deniedLabel) ? "module này" : deniedLabel.ToLowerInvariant();
                context.Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn chưa được cấp quyền truy cập " + moduleLabel + ".", "false", "false", "OK", "alert", "");
                Redirect(page, "/gianhang/admin");
                return false;
            }
        }

        return true;
    }
}
