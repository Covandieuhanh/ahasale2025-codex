using System;
using System.Web.UI;

public partial class daugia_quan_ly : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string query = Request.Url == null ? "" : (Request.Url.Query ?? "");
        string target = "/daugia/admin/quan-ly";
        if (!string.IsNullOrWhiteSpace(query))
            target += query;
        Response.Redirect(target, true);
    }
}
