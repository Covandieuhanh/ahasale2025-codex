using System;
using System.Web.UI;

public partial class gianhang_tai_khoan_default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
            BindPage(info);
    }

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
            if (account == null)
            {
                Response.Redirect("/gianhang/default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            GianHangStorefront_cl.StorefrontSummary summary = GianHangStorefront_cl.BuildSummary(db, account, Request.Url);
            GianHangReport_cl.ReportRange range = GianHangReport_cl.ResolveDateRange("", "", "", "all");
            GianHangReport_cl.Dashboard dashboard = GianHangReport_cl.BuildDashboard(db, info.AccountKey, range);

            img_avatar.ImageUrl = GianHangStorefront_cl.ResolveAvatarUrl(account);
            lit_store_name.Text = Server.HtmlEncode(summary.StorefrontName);
            lit_account_key.Text = Server.HtmlEncode(info.AccountKey);
            lit_space_status.Text = info.CanAccessGianHang ? "Đang hoạt động" : "Chưa mở quyền";
            lit_contact_email.Text = Server.HtmlEncode(ResolveTextOrFallback(RootAccount_cl.ResolveContactEmail(info), "Chưa có email"));

            lit_full_name.Text = Server.HtmlEncode(ResolveTextOrFallback(info.FullName, info.AccountKey));
            lit_account_type.Text = Server.HtmlEncode(ResolveTextOrFallback(info.AccountType, "Tài khoản Home"));
            lit_email.Text = Server.HtmlEncode(ResolveTextOrFallback(info.Email, "Chưa cập nhật"));
            lit_gianhang_email.Text = Server.HtmlEncode(ResolveTextOrFallback(info.ShopEmail, "Dùng chung email Home"));
            lit_profile_url.Text = Server.HtmlEncode(summary.ProfileAbsoluteUrl);
            lit_admin_access.Text = info.CanAccessGianHangAdmin ? "Đã mở quyền" : "Chưa mở quyền";

            lit_product_count.Text = summary.ProductCount.ToString("#,##0");
            lit_service_count.Text = summary.ServiceCount.ToString("#,##0");
            lit_pending_orders.Text = summary.PendingOrderCount.ToString("#,##0");
            lit_booking_total.Text = dashboard.BookingTotal.ToString("#,##0");
            lit_revenue_gross.Text = GianHangReport_cl.FormatCurrency(dashboard.RevenueGross);
            lit_total_views.Text = dashboard.TotalViews.ToString("#,##0");
            lit_exchange_delivery.Text = string.Format("{0:#,##0} / {1:#,##0}", dashboard.Exchanged, dashboard.Delivered);
            lit_booking_progress.Text = string.Format("{0:#,##0} / {1:#,##0}", dashboard.BookingConfirmed, dashboard.BookingDone);

            lnk_storefront.NavigateUrl = summary.ProfileUrl;
            lnk_edit_home_profile.NavigateUrl = "/home/edit-info.aspx";
            lnk_change_password.NavigateUrl = "/home/DoiMatKhau.aspx?return_url=%2Fgianhang%2Fdefault.aspx";
            lnk_manage_posts.NavigateUrl = "/gianhang/quan-ly-tin/Default.aspx";
            lnk_orders.NavigateUrl = "/gianhang/don-ban.aspx";
            lnk_bookings.NavigateUrl = "/gianhang/quan-ly-lich-hen.aspx";
            lnk_customers.NavigateUrl = "/gianhang/khach-hang.aspx";
            lnk_report.NavigateUrl = "/gianhang/bao-cao.aspx";

            ph_admin_link.Visible = info.CanAccessGianHangAdmin;
            lnk_admin.NavigateUrl = "/gianhang/admin";
            ph_admin_request_link.Visible = !info.CanAccessGianHangAdmin;
            lnk_request_admin.NavigateUrl = "/gianhang/mo-cong-cu-quan-tri.aspx";
        }
    }

    private static string ResolveTextOrFallback(string value, string fallback)
    {
        string text = (value ?? "").Trim();
        return text == "" ? fallback : text;
    }
}
