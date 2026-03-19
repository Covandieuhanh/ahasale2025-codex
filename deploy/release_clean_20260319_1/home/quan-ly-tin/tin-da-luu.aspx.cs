using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class tin_da_luu : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            check_login_cl.check_login_home("none", "none", false);

            // Lấy tin_theo_doi từ user hoặc session
            string tin_theo_doi = "true";
            UcDanhChoBanMoiNhat.tin_theo_doi = tin_theo_doi; // đổi từ tin_theo_doi sang tin_theo_doi
            UcDanhChoBanMoiNhat.TitleText = "Tin đã theo dõi"; // đổi title nếu muốn
        }
    }
}
