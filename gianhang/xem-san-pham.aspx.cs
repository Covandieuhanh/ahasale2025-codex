using System;
using System.Web;

public partial class gianhang_xem_san_pham : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string id = (Request.QueryString["id"] ?? string.Empty).Trim();
        if (id == string.Empty)
        {
            Response.Redirect("/gianhang/default.aspx", false);
            if (Context != null && Context.ApplicationInstance != null)
                Context.ApplicationInstance.CompleteRequest();
            return;
        }

        string targetUrl = "/gianhang/page/chi-tiet-san-pham.aspx?idbv=" + HttpUtility.UrlEncode(id);
        string userKey = (Request.QueryString["u"] ?? string.Empty).Trim();
        if (userKey != string.Empty)
            targetUrl = GianHangRoutes_cl.AppendUserToUrl(targetUrl, userKey);

        Response.Redirect(targetUrl, false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }
}
