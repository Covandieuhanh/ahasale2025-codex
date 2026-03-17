using System;

public partial class home_tim_kiem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Page.Title = "Tìm kiếm";
        }
    }
}
