using System;
using System.Web.UI;

public partial class gianhang_uc_space_nav : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        BindNav();
    }

    private void BindNav()
    {
        GianHangSpaceNav_cl.SpaceNavModel model = GianHangSpaceNav_cl.BuildCurrent(Request);
        ph_nav.Visible = model != null && model.Visible;
        if (!ph_nav.Visible)
            return;

        lit_store_name.Text = Server.HtmlEncode(model.StoreName ?? "");
        lit_account_key.Text = Server.HtmlEncode(model.AccountKey ?? "");
        lit_status.Text = Server.HtmlEncode(model.StatusText ?? "");
        img_avatar.ImageUrl = string.IsNullOrWhiteSpace(model.AvatarUrl)
            ? "/uploads/images/macdinh.jpg"
            : model.AvatarUrl;

        lnk_public.NavigateUrl = model.PublicUrl ?? GianHangRoutes_cl.BuildPublicStorefrontUrl(string.Empty);
        lnk_home.NavigateUrl = model.HomeUrl ?? "/gianhang/tai-khoan/default.aspx";
        lnk_manage.NavigateUrl = model.ManageUrl ?? GianHangRoutes_cl.BuildOwnerContentUrl();
        lnk_orders.NavigateUrl = model.OrdersUrl ?? GianHangRoutes_cl.BuildOwnerOrdersUrl();
        lnk_booking.NavigateUrl = model.BookingUrl ?? GianHangRoutes_cl.BuildOwnerBookingsUrl();
        lnk_customers.NavigateUrl = model.CustomersUrl ?? GianHangRoutes_cl.BuildOwnerCustomersUrl();
        lnk_report.NavigateUrl = model.ReportUrl ?? GianHangRoutes_cl.BuildOwnerReportsUrl();
        lnk_level2.NavigateUrl = model.Level2Url ?? GianHangRoutes_cl.BuildBridgeLevel2Url();

        ph_admin.Visible = model.ShowAdminUrl;
        lnk_admin.NavigateUrl = model.AdminUrl ?? "/gianhang/admin";

        ApplyActiveState(model.ActiveKey);
    }

    private void ApplyActiveState(string activeKey)
    {
        MarkActive(lnk_public, activeKey == "public");
        MarkActive(lnk_home, activeKey == "home");
        MarkActive(lnk_manage, activeKey == "manage");
        MarkActive(lnk_orders, activeKey == "orders");
        MarkActive(lnk_booking, activeKey == "booking");
        MarkActive(lnk_customers, activeKey == "customers");
        MarkActive(lnk_report, activeKey == "report");
        MarkActive(lnk_level2, activeKey == "level2");
    }

    private static void MarkActive(System.Web.UI.WebControls.HyperLink link, bool active)
    {
        if (link == null)
            return;

        string css = (link.CssClass ?? "").Trim();
        css = css.Replace(" is-active", "").Trim();
        if (active)
            css = (css + " is-active").Trim();
        link.CssClass = css;
    }
}
