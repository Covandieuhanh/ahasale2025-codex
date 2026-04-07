using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class admin_quan_ly_bai_viet_them_moi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AdminAccessGuard_cl.EnsurePageAccess(this))
            return;

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "add";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-bai-viet/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
