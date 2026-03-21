using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class admin_quan_ly_menu_chinh_sua : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var overrides = new Dictionary<string, string>();
        overrides["view"] = "edit";
        overrides["id"] = (Request.QueryString["id"] ?? "").Trim();
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/quan-ly-menu/Default.aspx", overrides, "id", "bin");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
