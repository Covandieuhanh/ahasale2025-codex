using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class admin_quan_ly_tai_khoan_them_moi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "add";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-tai-khoan/Default.aspx", overrides, "scope", "ftype", "fscope", "frole");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
