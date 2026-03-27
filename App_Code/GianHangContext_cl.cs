using System;
using System.Web.SessionState;
using System.Web.UI;

public static class GianHangContext_cl
{
    public const string SessionChiNhanhKey = "id_chinhanh_gianhang";
    public const string SessionTaiKhoanKey = "ten_tk_gianhang";
    public const string LegacySessionChiNhanhKey = "id_chinhanh_webcon";
    public const string LegacySessionTaiKhoanKey = "ten_tk_chinhanh";

    public static RootAccount_cl.RootAccountInfo EnsureCurrentAccess(Page page)
    {
        if (page == null)
            return null;

        string returnUrl = page.Request == null ? "/gianhang" : (page.Request.RawUrl ?? "/gianhang");
        string accountKey = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        if (accountKey == "")
        {
            if (page.Session != null)
            {
                page.Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                    "Thông báo",
                    "Vui lòng đăng nhập tài khoản Home để truy cập không gian /gianhang.",
                    "1500",
                    "warning");
            }

            page.Response.Redirect(GianHangRoutes_cl.BuildLoginUrl(returnUrl), true);
            return null;
        }

        GianHangBootstrap_cl.EnsureReady();
        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated)
        {
            if (page.Session != null)
            {
                page.Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                    "Thông báo",
                    "Phiên đăng nhập Home không còn hợp lệ. Vui lòng đăng nhập lại để tiếp tục.",
                    "1500",
                    "warning");
            }

            page.Response.Redirect(GianHangRoutes_cl.BuildLoginUrl(returnUrl), true);
            return null;
        }

        if (!info.CanAccessGianHang)
        {
            HomeSpaceAccess_cl.RedirectToAccessPage(page, ModuleSpace_cl.GianHang, returnUrl);
            return null;
        }

        return info;
    }

    public static string GetCurrentAccountKey(Page page)
    {
        RootAccount_cl.RootAccountInfo info = EnsureCurrentAccess(page);
        return info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
    }

    public static taikhoan_tb GetCurrentAccountEntity(dbDataContext db, Page page)
    {
        RootAccount_cl.RootAccountInfo info = EnsureCurrentAccess(page);
        if (info == null)
            return null;

        return RootAccount_cl.GetByAccountKey(db, info.AccountKey);
    }

    public static void RememberStorefrontContext(HttpSessionState session, string accountKey, string chiNhanhId)
    {
        if (session == null)
            return;

        string normalizedAccount = (accountKey ?? string.Empty).Trim().ToLowerInvariant();
        string normalizedChiNhanhId = string.IsNullOrWhiteSpace(chiNhanhId) ? "1" : chiNhanhId.Trim();

        session[SessionTaiKhoanKey] = normalizedAccount;
        session[SessionChiNhanhKey] = normalizedChiNhanhId;

        // Giai doan chuyen tiep: giu khoa cu de cac luong chua doi van chay.
        session[LegacySessionTaiKhoanKey] = normalizedAccount;
        session[LegacySessionChiNhanhKey] = normalizedChiNhanhId;
        session["chinhanh"] = normalizedChiNhanhId;
    }

    public static string ResolveSessionChiNhanhId(HttpSessionState session)
    {
        if (session == null)
            return "1";

        object raw = session[SessionChiNhanhKey]
            ?? session[LegacySessionChiNhanhKey]
            ?? session["chinhanh"]
            ?? "1";

        string value = (raw ?? "1").ToString().Trim();
        return string.IsNullOrWhiteSpace(value) ? "1" : value;
    }

    public static string ResolveSessionTaiKhoan(HttpSessionState session)
    {
        if (session == null)
            return string.Empty;

        object raw = session[SessionTaiKhoanKey]
            ?? session[LegacySessionTaiKhoanKey]
            ?? string.Empty;

        return (raw ?? string.Empty).ToString().Trim().ToLowerInvariant();
    }
}
