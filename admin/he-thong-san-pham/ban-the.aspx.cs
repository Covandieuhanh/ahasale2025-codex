using System;
using System.Collections.Generic;

public partial class admin_he_thong_san_pham_ban_the : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "sell";
        overrides["mode"] = "page";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/he-thong-san-pham/ban-san-pham.aspx", overrides);
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
