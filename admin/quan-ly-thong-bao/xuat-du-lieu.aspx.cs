using System;
using System.Collections.Generic;

public partial class admin_quan_ly_thong_bao_xuat_du_lieu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AdminAccessGuard_cl.EnsurePageAccess(this))
            return;

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "export";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-thong-bao/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
