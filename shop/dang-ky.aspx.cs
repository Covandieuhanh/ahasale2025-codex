using System;
using System.Web;

public partial class shop_dang_ky : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string requestedReturnUrl = Request.QueryString["return_url"] ?? Request.QueryString["returnUrl"] ?? "";
        requestedReturnUrl = HttpUtility.UrlDecode(requestedReturnUrl ?? "") ?? "";
        string returnUrl = check_login_cl.NormalizeUnifiedReturnUrl(requestedReturnUrl);
        if (string.IsNullOrWhiteSpace(returnUrl))
            returnUrl = "/shop/default.aspx";

        string target = "/home/mo-khong-gian.aspx?space=shop&return_url=" + HttpUtility.UrlEncode(returnUrl);
        Response.Redirect(target, true);
    }
}
