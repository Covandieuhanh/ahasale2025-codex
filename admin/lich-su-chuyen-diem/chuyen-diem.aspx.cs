using System;
using System.Collections.Generic;

public partial class admin_lich_su_chuyen_diem_chuyen_diem : System.Web.UI.Page
{
    private const string TabTieuDung = AdminDataScope_cl.TransferTabCore;

    private string NormalizeTabForRoute(string raw)
    {
        string tab = (raw ?? "").Trim().ToLowerInvariant();
        if (tab == TabTieuDung)
            return TabTieuDung;

        string normalized = AdminDataScope_cl.NormalizeTransferTab(tab);
        return string.IsNullOrWhiteSpace(normalized) ? TabTieuDung : normalized;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AdminAccessGuard_cl.EnsurePageAccess(this))
            return;

        var overrides = new Dictionary<string, string>();
        overrides["view"] = "transfer";
        overrides["tab"] = NormalizeTabForRoute(Request.QueryString["tab"]);
        string target = AdminFullPageRoute_cl.BuildTargetUrl(Request, "/admin/lich-su-chuyen-diem/Default.aspx", overrides, "tab");
        AdminFullPageRoute_cl.Transfer(this, target);
    }
}
