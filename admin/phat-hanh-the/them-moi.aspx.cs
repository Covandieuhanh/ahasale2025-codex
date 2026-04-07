using System;
using System.Collections.Generic;

public partial class admin_phat_hanh_the_them_moi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AdminAccessGuard_cl.EnsurePageAccess(this))
            return;

        if (CompanyShop_cl.HideLegacyAdminSystemProduct())
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Luồng phát hành/bán thẻ đã chuyển sang không gian /shop của tài khoản shop công ty.",
                "2200",
                "warning");
            Response.Redirect("/admin/default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "add";
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/phat-hanh-the.aspx", overrides);
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
