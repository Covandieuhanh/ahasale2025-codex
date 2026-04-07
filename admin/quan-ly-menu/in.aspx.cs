using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_menu_in : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("home_menu", "/admin/default.aspx?mspace=content");
    }
}
