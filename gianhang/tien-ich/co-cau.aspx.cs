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
        int index;
        if (!int.TryParse((TextBox1.Text ?? string.Empty).Trim(), out index) || index <= 0)
        {
            Label1.Text = "Vui lòng nhập một số thứ tự hợp lệ lớn hơn 0.";
            return;
        }

        GianHangWorkspaceUtility_cl.SetCoCauIndex(GianHangWorkspaceUtility_cl.ResolveWorkspaceKey(Request), index);
        Label1.Text = "Cơ cấu thành công.";
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        GianHangWorkspaceUtility_cl.ClearCoCauIndex(GianHangWorkspaceUtility_cl.ResolveWorkspaceKey(Request));
        Label1.Text = "ĐÃ TẮT CƠ CẤU.";
    }
}