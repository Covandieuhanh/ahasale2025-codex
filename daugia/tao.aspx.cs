using System;
using System.Web.UI;

public partial class daugia_tao : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string query = Request.Url == null ? "" : (Request.Url.Query ?? "");
        string target = "/daugia/admin/tao";
        if (!string.IsNullOrWhiteSpace(query))
            target += query;
        Response.Redirect(target, true);
    }
}
