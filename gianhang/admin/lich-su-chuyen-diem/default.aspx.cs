using System;
using System.Web;
using System.Web.UI;

public partial class gianhang_admin_lich_su_chuyen_diem_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, new dbDataContext(), "none");
        if (access == null)
            return;
    }
}
