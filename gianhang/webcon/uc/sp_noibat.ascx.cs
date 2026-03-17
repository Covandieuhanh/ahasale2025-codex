using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class uc_home_sp_noibat : System.Web.UI.UserControl
{
    dbDataContext db = new dbDataContext();
    public string name, des;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var q1 = db.web_menu_tables.Where(p => p.id.ToString() == "550" && p.bin == false && p.id_chinhanh ==AhaShineContext_cl.ResolveChiNhanhId());
            if (q1.Count() != 0)
            {
                // name = q1.First().name;
                des = q1.First().description;
            }

            var q = db.web_post_tables.Where(p => p.phanloai == "ctsp" && p.bin == false  && p.hienthi == true && p.id_chinhanh ==AhaShineContext_cl.ResolveChiNhanhId()).OrderBy(p => p.name);
            Repeater1.DataSource = q;
            Repeater1.DataBind();
        }
    }
}