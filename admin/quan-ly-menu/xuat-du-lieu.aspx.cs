using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class admin_quan_ly_menu_xuat_du_lieu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("home_menu", "/admin/default.aspx?mspace=content");

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "export";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-menu/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
