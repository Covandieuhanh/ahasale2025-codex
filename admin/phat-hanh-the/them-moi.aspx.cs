using System;
using System.Collections.Generic;

public partial class admin_phat_hanh_the_them_moi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "add";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/phat-hanh-the.aspx", overrides);
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
