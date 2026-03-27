using System;
using System.Web;
using System.Web.UI;

public partial class Uc_Shared_SpaceLauncher_uc : System.Web.UI.UserControl
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
        ToggleButtonCssClass = buttonCss + " aha-space-launcher__toggle";

        string wrapperCss = (WrapperCssClass ?? string.Empty).Trim();
        WrapperCssClassResolved = wrapperCss == string.Empty ? string.Empty : (" " + wrapperCss);

        LauncherRootId = ClientID + "_root";
        DrawerClientId = ClientID + "_drawer";

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                GlobalSpaceLauncher_cl.LauncherModel model = GlobalSpaceLauncher_cl.BuildCurrent(db, Request.RawUrl);
                if (model == null || !model.Visible || string.IsNullOrWhiteSpace(model.ItemsHtml))
                {
                    phLauncher.Visible = false;
                    return;
                }

                phLauncher.Visible = true;
                litAccountName.Text = HttpUtility.HtmlEncode(ResolveAccountName(model));
                litAccountKey.Text = HttpUtility.HtmlEncode(model.AccountKey ?? string.Empty);
                litItems.Text = model.ItemsHtml ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            phLauncher.Visible = false;
            Log_cl.Add_Log(ex.Message, "space_launcher_uc", ex.StackTrace);
        }
    }

    private static string ResolveAccountName(GlobalSpaceLauncher_cl.LauncherModel model)
    {
        if (model == null)
            return string.Empty;

        string accountName = (model.AccountName ?? string.Empty).Trim();
        string accountKey = (model.AccountKey ?? string.Empty).Trim();
        if (accountName == string.Empty)
            return accountKey;
        return accountName;
    }
}
