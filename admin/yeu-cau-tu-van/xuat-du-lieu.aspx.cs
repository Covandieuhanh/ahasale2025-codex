using System;
using System.Collections.Generic;

public partial class admin_yeu_cau_tu_van_xuat_du_lieu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "export";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/yeu-cau-tu-van/Default.aspx", overrides, "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
