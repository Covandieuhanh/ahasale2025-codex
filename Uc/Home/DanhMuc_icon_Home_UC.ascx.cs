using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Uc_Home_DanhMuc_icon_Home_UC : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (dbDataContext db = new dbDataContext())
            {
                var danhMucList = db.DanhMuc_tbs
                    .Where(p => p.id_parent == "134" && p.bin == false)
                    .OrderBy(d => d.rank)
                    .ToList();

                Repeater1.DataSource = danhMucList;
                Repeater1.DataBind();
            }
        }
    }

}