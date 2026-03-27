using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class admin_uc_admin_space_launcher_uc : System.Web.UI.UserControl
{
    public string ButtonCssClass { get; set; }
    public string WrapperCssClass { get; set; }

    protected string ToggleButtonCssClass { get; private set; }
    protected string WrapperCssClassResolved { get; private set; }
    protected string LauncherRootId { get; private set; }
    protected string DrawerClientId { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        string buttonCss = (ButtonCssClass ?? string.Empty).Trim();
        if (buttonCss == string.Empty)
            buttonCss = "app-bar-item";
        ToggleButtonCssClass = buttonCss + " aha-admin-space-launcher__toggle";

        string wrapperCss = (WrapperCssClass ?? string.Empty).Trim();
        WrapperCssClassResolved = wrapperCss == string.Empty ? string.Empty : (" " + wrapperCss);

        LauncherRootId = ClientID + "_root";
        DrawerClientId = ClientID + "_drawer";

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                string taiKhoan = AdminRolePolicy_cl.GetCurrentAdminUser();
                if (string.IsNullOrWhiteSpace(taiKhoan))
                {
                    phLauncher.Visible = false;
                    return;
                }

                taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
                if (account == null)
                {
                    phLauncher.Visible = false;
                    return;
                }

                List<AdminManagementSpace_cl.SpaceInfo> spaces = AdminManagementSpace_cl.GetAllowedSpaces(db, account, Request);
                if (spaces == null || spaces.Count == 0)
                {
                    phLauncher.Visible = false;
                    return;
                }

                string currentSpace = AdminManagementSpace_cl.GetCurrentSpace(db, account, Request);
                litCurrentSpaceTitle.Text = HttpUtility.HtmlEncode(AdminManagementSpace_cl.GetTitle(currentSpace));
                litAdminDisplayName.Text = HttpUtility.HtmlEncode(BuildAdminDisplayName(account));
                litAdminRole.Text = HttpUtility.HtmlEncode(BuildAdminRole(db, account));
                litItems.Text = BuildItemsHtml(spaces);
                phLauncher.Visible = true;
            }
        }
        catch (Exception ex)
        {
            phLauncher.Visible = false;
            Log_cl.Add_Log(ex.Message, "admin_space_launcher_uc", ex.StackTrace);
        }
    }

    private static string BuildAdminDisplayName(taikhoan_tb account)
    {
        if (account == null)
            return "Tài khoản admin";

        string displayName = (account.hoten ?? "").Trim();
        if (displayName == string.Empty)
            displayName = (account.taikhoan ?? "").Trim();
        if (displayName == string.Empty)
            displayName = "Tài khoản admin";

        return displayName;
    }

    private static string BuildAdminRole(dbDataContext db, taikhoan_tb account)
    {
        if (account == null)
            return "Vai trò quản trị";

        string displayName = BuildAdminDisplayName(account);
        string roleLabel = AdminRolePolicy_cl.GetAdminRoleLabel(db, account.taikhoan, account.phanloai, account.permission);
        if (string.IsNullOrWhiteSpace(roleLabel))
            return "Vai trò quản trị";

        return roleLabel;
    }

    private static string BuildItemsHtml(List<AdminManagementSpace_cl.SpaceInfo> spaces)
    {
        var html = new StringBuilder();
        foreach (AdminManagementSpace_cl.SpaceInfo item in spaces)
        {
            if (item == null)
                continue;

            string css = "aha-admin-space-launcher__link";
            if (item.IsActive)
                css += " is-active";

            html.Append("<a class='")
                .Append(css)
                .Append("' href='")
                .Append(HttpUtility.HtmlAttributeEncode(item.SwitchUrl ?? "#"))
                .Append("'>")
                .Append("<span class='aha-admin-space-launcher__copy'>")
                .Append("<strong>")
                .Append(HttpUtility.HtmlEncode(item.Title ?? string.Empty))
                .Append("</strong>")
                .Append("<span>")
                .Append(HttpUtility.HtmlEncode(item.Subtitle ?? string.Empty))
                .Append("</span>")
                .Append("</span>")
                .Append("<span class='aha-admin-space-launcher__chevron' aria-hidden='true'>&rsaquo;</span>")
                .Append("</a>");
        }

        return html.ToString();
    }
}
