using System;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

public static class GianHangCheckoutPortal_cl
{
    public const int LinkContextTtlMinutes = 30;

    public static string SellerDonBanUrl()
    {
        return GianHangRoutes_cl.BuildDonBanUrl();
    }

    public static string ChoThanhToanUrl()
    {
        return GianHangRoutes_cl.ChoThanhToanUrl();
    }

    public static string LoginUrl()
    {
        return GianHangRoutes_cl.BuildLoginUrl(GianHangRoutes_cl.ChoThanhToanUrl());
    }

    public static string ResolvePortalDialogClass(string defaultCssClass)
    {
        return "warning";
    }

    public static void SetDialogOnload(HttpSessionState session, string title, string message, string defaultCssClass)
    {
        if (session == null)
            return;

        session["thongbao_home"] = thongbao_class.metro_dialog_onload(
            title ?? "Thông báo",
            message ?? string.Empty,
            "false", "false", "OK", ResolvePortalDialogClass(defaultCssClass), "");
    }

    public static void SetExchangeSuccessNotice(HttpSessionState session, string message)
    {
        if (session == null)
            return;

        session["thongbao_home"] = thongbao_class.metro_dialog_onload(
            "Thông báo",
            message ?? string.Empty,
            "false", "false", "OK", ResolvePortalDialogClass("success"), "");
    }

    public static void ShowDialog(Page page, string title, string message, string defaultCssClass)
    {
        if (page == null)
            return;

        ScriptManager.RegisterStartupScript(
            page,
            page.GetType(),
            Guid.NewGuid().ToString(),
            thongbao_class.metro_dialog(
                title ?? "Thông báo",
                message ?? string.Empty,
                "false", "false", "OK",
                ResolvePortalDialogClass(defaultCssClass),
                ""),
            true);
    }

    public static void ApplyExchangeResult(Page page, HttpSessionState session, GianHangCheckoutCommand_cl.ExchangeResult result)
    {
        if (page == null || result == null)
            return;

        if (!string.IsNullOrWhiteSpace(result.SuccessNotice))
        {
            SetExchangeSuccessNotice(session, result.SuccessNotice);
        }
        else if (!string.IsNullOrWhiteSpace(result.DialogMessage))
        {
            if (result.UseOnloadDialog)
                SetDialogOnload(session, result.DialogTitle, result.DialogMessage, result.DialogType);
            else
                ShowDialog(page, result.DialogTitle, result.DialogMessage, result.DialogType);
        }

        if (result.ShouldRedirect && !string.IsNullOrWhiteSpace(result.RedirectUrl))
        {
            ScriptManager.RegisterStartupScript(
                page,
                page.GetType(),
                Guid.NewGuid().ToString(),
                "window.location.href='" + result.RedirectUrl + "';",
                true);
        }
    }

    public static void ClearPaymentSession(HttpSessionState session)
    {
        WalletPaymentSession_cl.Clear(session);
    }

    public static string GetClientIp()
    {
        if (HttpContext.Current == null || HttpContext.Current.Request == null)
            return string.Empty;

        string ip = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(ip))
        {
            string[] ips = ip.Split(',');
            if (ips.Length > 0)
                return ips[0].Trim();
        }

        return (HttpContext.Current.Request.UserHostAddress ?? string.Empty).Trim();
    }
}
