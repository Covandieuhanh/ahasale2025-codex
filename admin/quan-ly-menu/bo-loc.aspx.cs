using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class admin_quan_ly_menu_bo_loc : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "filter";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-menu/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
