using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_menu_in_a4_doc : System.Web.UI.Page
{
    private bool HasAnyPermission(string user, params string[] permissionKeys)
    {
        if (string.IsNullOrWhiteSpace(user) || permissionKeys == null)
            return false;

        for (int i = 0; i < permissionKeys.Length; i++)
        {
            string permissionKey = (permissionKeys[i] ?? "").Trim();
            if (permissionKey != "" && bcorn_class.check_quyen(user, permissionKey) == "")
                return true;
        }

        return false;
    }
    private void RedirectToAdminHome()
    {
        Response.Redirect(GianHangAdminBridge_cl.BuildAdminHomeUrl(HttpContext.Current));
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!GianHangSystemAdminGuard_cl.EnsurePageAccess(this))
            return;
        dbDataContext db = new dbDataContext();
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        string currentUser = (access.User ?? "").Trim();
        if (!HasAnyPermission(currentUser, "q4_1"))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
            return;
        }
    }
}
