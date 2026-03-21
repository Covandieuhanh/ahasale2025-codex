using System;
using System.Collections.Generic;

public partial class admin_yeu_cau_tu_van_ban_in : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "print";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/yeu-cau-tu-van/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
