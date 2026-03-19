using System;
using System.Web;

public partial class home_quan_ly_thong_bao_default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("/home/quan-ly-tin/default.aspx", false);
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }
}

