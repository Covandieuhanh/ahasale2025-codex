using System;
using System.Web;
using System.Web.UI;

public static class GianHangAdminPageGuard_cl
{
    public sealed class AccessInfo
    {
        public string User { get; set; }
        public string OwnerAccountKey { get; set; }
        public string HomeAccountKey { get; set; }
        public string ChiNhanhId { get; set; }
        public string NganhId { get; set; }
        public string RoleLabel { get; set; }
    }

    public static AccessInfo EnsureAccess(Page page, dbDataContext db, string requiredPermission)
    {
        if (page == null || db == null)
            return null;

        string rawUrl = page.Request == null ? "/gianhang/admin" : ((page.Request.RawUrl ?? "/gianhang/admin").Trim());
        string homeAccountKey;
        string deniedMessage;
        if (!GianHangAdminBridge_cl.EnsureLegacyAdminSessionFromCurrentHome(db, out homeAccountKey, out deniedMessage))
        {
            if (!string.IsNullOrWhiteSpace(homeAccountKey) && !string.IsNullOrWhiteSpace(deniedMessage))
                HomeSpaceAccess_cl.RedirectToAccessPage(page, ModuleSpace_cl.GianHangAdmin, rawUrl);
            else
                HomeSpaceAccess_cl.RedirectToHomeLogin(page, rawUrl, "Vui lòng đăng nhập tài khoản Home để truy cập không gian /gianhang/admin.");
            return null;
        }

        HttpContext ctx = HttpContext.Current;
        string cookieUser = ctx != null && ctx.Request != null && ctx.Request.Cookies["save_user_admin_aka_1"] != null
            ? ctx.Request.Cookies["save_user_admin_aka_1"].Value
            : "";
        string cookiePass = ctx != null && ctx.Request != null && ctx.Request.Cookies["save_pass_admin_aka_1"] != null
            ? ctx.Request.Cookies["save_pass_admin_aka_1"].Value
            : "";

        if (ctx != null && ctx.Session != null)
        {
            if (ctx.Session["user"] == null)
                ctx.Session["user"] = "";
            if (ctx.Session["notifi"] == null)
                ctx.Session["notifi"] = "";
            if (((ctx.Session["user"] ?? "") + "").Trim() == "")
            {
                page.Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
                return null;
            }
        }

        string currentUrl = page.Request == null ? "" : page.Request.Url.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
        string loginResult = bcorn_class.check_login(
            ((ctx == null || ctx.Session == null ? "" : (ctx.Session["user"] ?? "") + "") ?? "").Trim(),
            cookieUser,
            cookiePass,
            currentUrl,
            string.IsNullOrWhiteSpace(requiredPermission) ? "none" : requiredPermission.Trim());

        if (loginResult != "")
        {
            if (loginResult == "baotri")
            {
                page.Response.Redirect("/baotri.aspx");
                return null;
            }

            if (loginResult == "1")
            {
                page.Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
                return null;
            }

            if (loginResult == "2")
            {
                string currentHomeAccountKey = GianHangAdminBridge_cl.ReadHomeAccountFromSessionOrCookie();
                if (!string.IsNullOrWhiteSpace(currentHomeAccountKey))
                {
                    GianHangAdminBridge_cl.ClearLegacyAdminSession();
                    HomeSpaceAccess_cl.RedirectToAccessPage(page, ModuleSpace_cl.GianHangAdmin, rawUrl);
                    return null;
                }

                if (ctx != null && ctx.Session != null)
                {
                    ctx.Session["notifi"] = thongbao_class.metro_dialog_onload(
                        "Thông báo",
                        "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.",
                        "false",
                        "false",
                        "OK",
                        "alert",
                        "");
                }
                page.Response.Redirect("/gianhang/admin");
                return null;
            }

            if (ctx != null && ctx.Session != null)
            {
                ctx.Session["notifi"] = loginResult;
                ctx.Session["user"] = "";
            }
            if (ctx != null && ctx.Response != null)
            {
                ExpireCookie(ctx.Response, "save_user_admin_aka_1");
                ExpireCookie(ctx.Response, "save_pass_admin_aka_1");
                ExpireCookie(ctx.Response, "save_url_admin_aka_1");
            }
            page.Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
            return null;
        }

        string ownerAccountKey = ((ctx == null || ctx.Session == null ? "" : (ctx.Session["user_parent"] ?? "") + "") ?? "").Trim();
        if (ownerAccountKey == "")
            ownerAccountKey = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

        if (string.IsNullOrWhiteSpace(ownerAccountKey))
        {
            HomeSpaceAccess_cl.RedirectToAccessPage(page, ModuleSpace_cl.GianHangAdmin, rawUrl);
            return null;
        }

        try
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey);
        }
        catch
        {
        }

        return new AccessInfo
        {
            User = ((ctx == null || ctx.Session == null ? "" : (ctx.Session["user"] ?? "") + "") ?? "").Trim(),
            OwnerAccountKey = ownerAccountKey,
            HomeAccountKey = homeAccountKey,
            ChiNhanhId = ((ctx == null || ctx.Session == null ? "" : (ctx.Session["chinhanh"] ?? "") + "") ?? "").Trim(),
            NganhId = ((ctx == null || ctx.Session == null ? "" : (ctx.Session["nganh"] ?? "") + "") ?? "").Trim(),
            RoleLabel = GianHangAdminContext_cl.ResolveCurrentRoleLabel()
        };
    }

    private static void ExpireCookie(HttpResponse response, string cookieName)
    {
        if (response == null || string.IsNullOrWhiteSpace(cookieName))
            return;

        HttpCookie cookie = new HttpCookie(cookieName);
        cookie.Expires = DateTime.Now.AddDays(-1);
        cookie.Value = "";
        cookie.Path = "/";
        response.Cookies.Set(cookie);
    }
}
