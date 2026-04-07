using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_website_quan_ly_menu_xemtruoc : System.Web.UI.Page
{
   dbDataContext db = new dbDataContext();
    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q5_1");
        if (access == null)
            return;
    }

}
