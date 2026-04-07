using System;
using System.Collections.Generic;

public partial class admin_he_thong_san_pham_chi_tiet_giao_dich : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("system_products", "/admin/default.aspx");

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "detail";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/he-thong-san-pham/ban-san-pham.aspx", overrides);
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
