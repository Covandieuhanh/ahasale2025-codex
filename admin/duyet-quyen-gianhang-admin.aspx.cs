using System;

public partial class admin_duyet_quyen_gianhang_admin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("/admin/duyet-gian-hang-doi-tac.aspx", true);
    }
}
