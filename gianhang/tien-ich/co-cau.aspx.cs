using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class tien_ich_co_cau : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Application["CoCau"] = TextBox1.Text;
        Label1.Text = "Cơ cấu thành công.";
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Application["CoCau"] = null;
        Label1.Text = "ĐÃ TẮT CƠ CẤU.";
    }
}