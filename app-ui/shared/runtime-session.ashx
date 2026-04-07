<%@ WebHandler Language="C#" Class="AppUiRuntimeSessionHandler" %>

using System;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

public class AppUiRuntimeSessionHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/javascript; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string json = serializer.Serialize(BuildPayload());
        context.Response.Write("window.AhaAppUiRuntimeSession=" + json + ";");
    }

    public bool IsReusable { get { return false; } }

    private static object BuildPayload()
    {
        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
        {
            return new
            {
                auth_state = "guest",
                seller_status = "not_registered",
                available_spaces = new string[0],
                user_summary = new
                {
                    display_name = "Khách",
                    username = "",
                    phone = "",
                    role_label = "Khách",
                    avatar_url = "/app-ui/assets/placeholder-media.svg",
                    avatar_fallback = "KH"
                }
            };
        }

        return CoreDb_cl.Use(db =>
        {
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
            string sellerStatus = ResolveSellerStatus(db, info);
            string displayName = ResolveDisplayName(info);
            string phone = (info.AccountKey ?? "").Trim();
            string avatarUrl = GianHangStorefront_cl.ResolveAvatarUrl(account);
            string[] spaces = RootAccount_cl.GetAvailableSpaces(info).ToArray();

            return new
            {
                auth_state = "authenticated",
                seller_status = sellerStatus,
                available_spaces = spaces,
                user_summary = new
                {
                    display_name = displayName,
                    username = string.IsNullOrWhiteSpace(info.AccountKey) ? "" : ("Home " + info.AccountKey.Trim()),
                    phone = phone,
                    email = RootAccount_cl.ResolveContactEmail(info),
                    role_label = ResolveRoleLabel(info, sellerStatus),
                    avatar_url = avatarUrl,
                    avatar_fallback = BuildAvatarFallback(displayName, phone)
                }
            };
        });
    }

    private static string ResolveSellerStatus(dbDataContext db, RootAccount_cl.RootAccountInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.AccountKey))
            return "not_registered";

        if (info.CanAccessGianHang)
            return "approved";

        CoreSpaceRequest_cl.SpaceRequestInfo latest = CoreSpaceRequest_cl.GetLatestRequest(db, info.AccountKey, ModuleSpace_cl.GianHang);
        if (latest != null && string.Equals((latest.RequestStatus ?? "").Trim(), CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase))
            return "pending";

        return "not_registered";
    }

    private static string ResolveDisplayName(RootAccount_cl.RootAccountInfo info)
    {
        if (info == null)
            return "Khách";

        string fullName = (info.FullName ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(fullName))
            return fullName;

        string accountKey = (info.AccountKey ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(accountKey))
            return "Home " + accountKey;

        return "Tài khoản";
    }

    private static string ResolveRoleLabel(RootAccount_cl.RootAccountInfo info, string sellerStatus)
    {
        if (info == null)
            return "Khách";

        if (string.Equals(sellerStatus, "approved", StringComparison.OrdinalIgnoreCase))
            return "Đối tác";

        if (info.CanAccessHome)
            return "Khách hàng";

        return "Tài khoản";
    }

    private static string BuildAvatarFallback(string displayName, string accountKey)
    {
        string value = (displayName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            value = (accountKey ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "AH";

        string[] parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpperInvariant();
        if (parts[0].Length >= 2)
            return parts[0].Substring(0, 2).ToUpperInvariant();
        return parts[0].ToUpperInvariant();
    }
}
