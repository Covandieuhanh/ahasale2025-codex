using System;
using System.Collections.Generic;

public partial class admin_lich_su_chuyen_diem_chuyen_diem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "transfer";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/lich-su-chuyen-diem/Default.aspx", overrides);
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
