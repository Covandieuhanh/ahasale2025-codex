using System;

public partial class admin_duyet_quyen_gianhang : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("home_gianhang_space", "/admin/default.aspx?mspace=gianhang");
        Response.Redirect("/admin/duyet-gian-hang-doi-tac.aspx", true);
    }
}
