using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class shop_khoi_phuc_mat_khau : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Legacy link-based reset deprecated; keep page as notice.
            UpdatePanel1.Visible = false;
            PlaceHolder1.Visible = true;
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng quay lại trang đăng nhập để nhận OTP.", "Thông báo", true, "warning");
        return;
    }
}
