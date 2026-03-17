using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class myweb_uc_metro_style_slider : System.Web.UI.UserControl
{
    dbDataContext db = new dbDataContext();    
    protected void Page_Load(object sender, EventArgs e)
    {
        Repeater1.DataSource = db.web_module_slider_tables.OrderBy(p => p.rank);
        Repeater1.DataBind();
    }
}