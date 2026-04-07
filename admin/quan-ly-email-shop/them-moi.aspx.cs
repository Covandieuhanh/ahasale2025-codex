using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class admin_quan_ly_banner_them_moi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AdminAccessGuard_cl.EnsurePageAccess(this))
            return;

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "add";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-banner/Default.aspx", overrides);
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
