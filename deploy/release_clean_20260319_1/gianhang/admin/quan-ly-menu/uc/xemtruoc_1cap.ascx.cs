using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_website_quan_ly_menu_uc_xemtruoc_1cap : System.Web.UI.UserControl
{    
    menu_class mn_cl = new menu_class();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Repeater1.DataSource = mn_cl.return_list().Where(p => p.id_parent == "0" && p.bin == false&& p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.rank);
            Repeater1.DataBind();
        }
    }
}