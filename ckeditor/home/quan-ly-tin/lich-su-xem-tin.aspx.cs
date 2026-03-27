using System;
using System.Linq;
using System.Web;

public partial class home_lich_su_xem_tin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            check_login_cl.check_login_home("none", "none", false);
            string lich_su_xem_tin = "true";
            UcDanhChoBanMoiNhat.lich_su_xem_tin = lich_su_xem_tin; // đổi từ tin_theo_doi sang tin_theo_doi
            UcDanhChoBanMoiNhat.TitleText = "Lịch sử xem tin"; // đổi title nếu muốn
        }
    }
}
