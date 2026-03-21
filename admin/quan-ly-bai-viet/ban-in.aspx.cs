using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class admin_quan_ly_bai_viet_ban_in : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "print";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-bai-viet/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}

