using System;
using System.Web;

public partial class home_logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string returnUrl = NormalizeLocalReturnUrl(Request.QueryString["return_url"]);
        if (string.IsNullOrEmpty(returnUrl))
        {
            string raw = (Request.QueryString["return_url"] ?? "").Trim();
            if (!string.IsNullOrEmpty(raw))
            {
                raw = HttpUtility.UrlDecode(raw) ?? raw;
                raw = raw.Trim();
                if (raw.StartsWith("/", StringComparison.Ordinal) && !raw.StartsWith("//", StringComparison.Ordinal))
                    returnUrl = raw;
            }
        }
        if (string.IsNullOrEmpty(returnUrl))
        {
            string sessionReturn = Session["home_logout_return"] as string;
            Session["home_logout_return"] = null;
            if (!string.IsNullOrEmpty(sessionReturn))
            {
                sessionReturn = sessionReturn.Trim();
                if (sessionReturn.StartsWith("/", StringComparison.Ordinal)
                    && !sessionReturn.StartsWith("//", StringComparison.Ordinal))
                {
                    returnUrl = sessionReturn;
                }
            }
        }

        Session["taikhoan_home"] = "";
        Session["matkhau_home"] = "";

        if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
            Response.Cookies["cookie_userinfo_home_bcorn"].Expires = DateTime.Now.AddDays(-1);

        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = "/home/default.aspx";

        Response.Redirect(returnUrl, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private string NormalizeLocalReturnUrl(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        value = HttpUtility.UrlDecode(value) ?? "";
        value = value.Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        Uri absolute;
        if (Uri.TryCreate(value, UriKind.Absolute, out absolute))
        {
            if (Request.Url == null)
                return "";

            if (!string.Equals(absolute.Host, Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                return "";

            value = absolute.PathAndQuery;
        }

        if (!value.StartsWith("/", StringComparison.Ordinal))
            return "";
        if (value.StartsWith("//", StringComparison.Ordinal))
            return "";

        return value;
    }
}
