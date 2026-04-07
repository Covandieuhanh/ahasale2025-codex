using System;
using System.Collections.Generic;

public partial class admin_yeu_cau_tu_van_bo_loc : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AdminAccessGuard_cl.EnsurePageAccess(this))
            return;

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "filter";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/yeu-cau-tu-van/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
