using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage_Tabler_Tabler2 : System.Web.UI.MasterPage
{
    private static string NormalizeLocalReturnUrl(string raw)
    {
        string url = HttpUtility.UrlDecode(raw ?? "") ?? "";
        url = url.Trim();
        if (string.IsNullOrEmpty(url))
            return "/";
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            return "/";
        if (!url.StartsWith("/"))
            return "/";
        return url;
    }

    private string BuildAbsoluteUrl(string rawPath, string fallbackPath)
    {
        return PortalBranding_cl.BuildAbsoluteUrl(Request, rawPath, fallbackPath);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string currentPath = (Request == null || Request.Url == null) ? string.Empty : (Request.Url.AbsolutePath ?? string.Empty);
        string currentSpace = ModuleSpace_cl.ResolveByPath(currentPath);
        if (gianhang_space_nav != null)
            gianhang_space_nav.Visible = !string.Equals(currentSpace, ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase);

        // Giữ nguyên action hiện tại cho shop portal để không mất marker khi postback.
        if (PortalRequest_cl.IsShopPortalRequest() && form1 != null)
        {
            string rawUrl = (Request == null) ? "" : (Request.RawUrl ?? "");
            if (!string.IsNullOrEmpty(rawUrl))
                form1.Action = rawUrl;
        }

        if (!IsPostBack)
        {
            try
            {
                using (dbDataContext db = new dbDataContext())
                {

                    #region Favicon & icon mobile
                    string scopeKey = PortalBranding_cl.ResolveScopeKeyFromRequest(Request);
                    bool isShopPortal = scopeKey == PortalBranding_cl.ScopeShop;
                    PortalBranding_cl.ScopeBrandingSnapshot branding = PortalBranding_cl.LoadScopeBranding(db, scopeKey, true);

                    string fallbackIcon = PortalBranding_cl.ResolveDefaultIconPath(scopeKey);
                    string iconUrl = BuildAbsoluteUrl(PortalBranding_cl.ResolveFaviconPath(branding, scopeKey), PortalBranding_cl.DefaultHomeIconPath);
                    string appleTouchIconUrl = BuildAbsoluteUrl(PortalBranding_cl.ResolveHeaderLogoPath(branding, scopeKey), fallbackIcon);
                    string pwaIcon192Url = appleTouchIconUrl;
                    string pwaIcon512Url = appleTouchIconUrl;
                    string manifestPath = isShopPortal ? "/manifest-shop.ashx" : "/manifest.ashx";
                    string manifestUrl = string.Format("{0}://{1}{2}?v=20260324", Request.Url.Scheme, Request.Url.Authority, manifestPath);
                    string startupImageUrl = appleTouchIconUrl;
                    string appName = isShopPortal ? "AhaSale Shop" : "AhaSale";
                    string themeColor = isShopPortal ? "#ff5b2e" : "#10b981";

                    if (Context != null)
                        Context.Items["AhaHeaderCenterLogoUrl"] = appleTouchIconUrl;

                    string iconsHtml = string.Format(@"
                <!-- Favicon -->
                <link rel='icon' href='{0}' sizes='16x16' type='image/x-icon'>
                <link rel='icon' href='{1}' sizes='32x32' type='image/x-icon'>
                <link rel='icon' href='{2}' sizes='48x48' type='image/x-icon'>
                <link rel='shortcut icon' href='{2}' type='image/x-icon'>

                <!-- Apple Touch Icon -->
                <link rel='apple-touch-icon' href='{3}' sizes='180x180'>
                <link rel='apple-touch-icon' href='{4}' sizes='167x167'>
                <link rel='apple-touch-icon' href='{5}' sizes='152x152'>
                <link rel='apple-touch-icon' href='{6}' sizes='120x120'>
                <link rel='apple-touch-icon-precomposed' href='{3}'>

                <!-- Android Icons -->
                <link rel='icon' href='{7}' sizes='192x192'>
                <link rel='icon' href='{8}' sizes='512x512'>

                <!-- PWA -->
                <link rel='manifest' href='{9}'>
                <meta name='theme-color' content='{11}'>
                <meta name='apple-mobile-web-app-capable' content='yes'>
                <meta name='apple-mobile-web-app-status-bar-style' content='default'>
                <meta name='apple-mobile-web-app-title' content='{12}'>
                <meta name='mobile-web-app-capable' content='yes'>
                <meta name='application-name' content='{12}'>
                <meta name='msapplication-TileColor' content='{11}'>
                <meta name='msapplication-TileImage' content='{3}'>
                <link rel='apple-touch-startup-image' href='{10}'>
                ", iconUrl, iconUrl, iconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, pwaIcon192Url, pwaIcon512Url, manifestUrl, startupImageUrl, themeColor, appName);

                    literal_fav_icon.Text = iconsHtml;
                    #endregion
                    #region lưu nội dung thông báo nếu có
                    if (Session["thongbao_home"] != null)
                    {
                        ViewState["thongbao_home"] = Session["thongbao_home"].ToString();
                        Session["thongbao_home"] = null;

                    }
                    #endregion

                    string needLogoutRaw = (Request.QueryString["need_logout"] ?? "").Trim();
                    bool needLogout = needLogoutRaw == "1"
                        || needLogoutRaw.Equals("true", StringComparison.OrdinalIgnoreCase);
                    bool hasPendingModal = Session["home_modal2_msg"] != null
                        || Session["home_modal_msg"] != null
                        || needLogout;

                    #region kiểm tra trạng thái chờ Trao đổi (nếu có)

                    string _tk = PortalRequest_cl.GetCurrentAccount();
                    if (!string.IsNullOrEmpty(_tk) && !hasPendingModal) // nếu có đăng nhập
                    {
                        try
                        {
                            var q1 = db.DonHang_tbs.FirstOrDefault(p =>
                                p.nguoiban == _tk &&
                                (
                                    p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                                    || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                                ));
                            if (q1 != null)
                            {
                                bool isShopPortalRequest = PortalRequest_cl.IsShopPortalRequest();
                                Response.Redirect(isShopPortalRequest ? "/shop/cho-thanh-toan" : "/home/cho-thanh-toan.aspx");
                            }
                        }
                        catch (SqlException ex)
                        {
                            if (!IsMissingDonHangStatusColumnError(ex))
                                throw;
                        }
                    }
                    else//nếu k đăng nhập
                    {
                        _tk = "";
                    }
                    #endregion

                    // ✅ HIỆN MODAL NẾU TRANG TRƯỚC GỬI QUA
                    if (needLogout)
                    {
                        string returnUrl = NormalizeLocalReturnUrl(Request.QueryString["return_url"]);
                        if (!string.IsNullOrEmpty(returnUrl))
                            Session["home_logout_return"] = returnUrl;
                        else if (Session["home_logout_return"] != null)
                            returnUrl = NormalizeLocalReturnUrl(Session["home_logout_return"].ToString());

                        string logoutUrl = "/home/logout.aspx?return_url=" + HttpUtility.UrlEncode(returnUrl);

                        Helper_Tabler_cl.ShowModalTwoButtons(
                            this.Page,
                            "Bạn phải đăng xuất tài khoản để được phép thực hiện Đăng ký tài khoản",
                            "Thông báo",
                            true,
                            "warning",
                            "Đăng xuất",
                            logoutUrl,
                            "Để sau",
                            "/");
                    }
                    else if (Session["home_modal2_msg"] != null)
                    {
                        string msg = Session["home_modal2_msg"].ToString();
                        string title = (Session["home_modal2_title"] ?? "Thông báo").ToString();
                        string type = (Session["home_modal2_type"] ?? "warning").ToString();
                        string primaryText = (Session["home_modal2_primary_text"] ?? "Đăng xuất").ToString();
                        string primaryHref = (Session["home_modal2_primary_href"] ?? "").ToString();
                        string secondaryText = (Session["home_modal2_secondary_text"] ?? "Để sau").ToString();
                        string secondaryHref = (Session["home_modal2_secondary_href"] ?? "/home/default.aspx").ToString();

                        Session["home_modal2_msg"] = null;
                        Session["home_modal2_title"] = null;
                        Session["home_modal2_type"] = null;
                        Session["home_modal2_primary_text"] = null;
                        Session["home_modal2_primary_href"] = null;
                        Session["home_modal2_secondary_text"] = null;
                        Session["home_modal2_secondary_href"] = null;

                        Helper_Tabler_cl.ShowModalTwoButtons(
                            this.Page,
                            msg,
                            title,
                            true,
                            type,
                            primaryText,
                            primaryHref,
                            secondaryText,
                            secondaryHref);
                    }
                    else if (Session["home_modal_msg"] != null)
                    {
                        string msg = Session["home_modal_msg"].ToString();
                        string title = (Session["home_modal_title"] ?? "Thông báo").ToString();
                        string type = (Session["home_modal_type"] ?? "info").ToString();

                        // clear session để không bị hiện lại
                        Session["home_modal_msg"] = null;
                        Session["home_modal_title"] = null;
                        Session["home_modal_type"] = null;

                        Helper_Tabler_cl.ShowModal(this.Page, msg, title, true, type);
                    }

                    if (PortalRequest_cl.IsShopPortalRequest())
                    {
                        Helper_Tabler_cl.ShowThongBaoSessionShop(this.Page);
                    }
                }
            }
            catch (Exception ex)
            {
                Log_cl.Add_Log(ex.Message, "home_master", ex.StackTrace);
            }
        }
    }

    private static bool IsMissingDonHangStatusColumnError(SqlException ex)
    {
        if (ex == null)
            return false;

        string message = ex.Message ?? "";
        return message.IndexOf("Invalid column name 'exchange_status'", StringComparison.OrdinalIgnoreCase) >= 0
            || message.IndexOf("Invalid column name 'order_status'", StringComparison.OrdinalIgnoreCase) >= 0;
    }


}
