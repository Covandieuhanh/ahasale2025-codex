using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public static class GlobalSpaceLauncher_cl
{
    public sealed class LauncherModel
    {
        public bool Visible { get; set; }
        public string AccountKey { get; set; }
        public string AccountName { get; set; }
        public string CurrentScope { get; set; }
        public string ItemsHtml { get; set; }
    }

    private sealed class CurrentContext
    {
        public string AccountKey { get; set; }
        public string Scope { get; set; }
        public bool IsManagedHomeSession { get; set; }
    }

    public static LauncherModel BuildCurrent(dbDataContext db, string rawPath)
    {
        LauncherModel model = new LauncherModel
        {
            Visible = false,
            AccountKey = "",
            AccountName = "",
            CurrentScope = "",
            ItemsHtml = ""
        };

        if (db == null)
            return model;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        CurrentContext context = ResolveCurrentContext();
        if (context == null || string.IsNullOrWhiteSpace(context.AccountKey))
            return model;

        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetInfo(db, context.AccountKey);
        if (info == null || !info.IsAuthenticated)
            return model;

        string currentPath = NormalizePath(rawPath);
        string currentSpace = ResolveSpaceByPath(currentPath);
        string selectedWorkspaceOwner = "";
        if (string.Equals(currentSpace, ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase))
            selectedWorkspaceOwner = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

        StringBuilder html = new StringBuilder();

        if (info.CanAccessHome)
        {
            AppendLink(
                html,
                "Truy cập không gian Home",
                "/",
                BuildApprovedHint(ModuleSpace_cl.Home),
                string.Equals(currentSpace, ModuleSpace_cl.Home, StringComparison.OrdinalIgnoreCase),
                false);
        }

        bool homeFlavor = context.IsManagedHomeSession
            || string.Equals(context.Scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase);

        if (homeFlavor)
            AppendHomeScopeEntries(db, html, info, currentSpace, selectedWorkspaceOwner);
        else
            AppendActiveEntries(db, html, info, currentSpace, selectedWorkspaceOwner);

        model.AccountKey = info.AccountKey ?? "";
        model.AccountName = string.IsNullOrWhiteSpace(info.FullName) ? (info.AccountKey ?? "") : info.FullName;
        model.CurrentScope = context.Scope ?? "";
        model.ItemsHtml = html.ToString();
        model.Visible = model.ItemsHtml.Length > 0;
        return model;
    }

    private static CurrentContext ResolveCurrentContext()
    {
        string rawPath = NormalizePath(CurrentPath());
        bool isGianHangAdminRequest = string.Equals(ResolveSpaceByPath(rawPath), ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase);

        if (isGianHangAdminRequest && GianHangAdminContext_cl.IsHomeManagedSession())
        {
            string managedHome = GianHangAdminContext_cl.ResolveDisplayAccountKey();
            if (!string.IsNullOrWhiteSpace(managedHome))
            {
                return new CurrentContext
                {
                    AccountKey = managedHome,
                    Scope = PortalScope_cl.ScopeHome,
                    IsManagedHomeSession = true
                };
            }
        }

        if (rawPath.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
        {
            string adminAccount = ReadEncodedAccount("taikhoan", "cookie_userinfo_admin_bcorn");
            if (!string.IsNullOrWhiteSpace(adminAccount))
            {
                return new CurrentContext
                {
                    AccountKey = adminAccount,
                    Scope = PortalScope_cl.ScopeAdmin,
                    IsManagedHomeSession = false
                };
            }
        }

        if (rawPath.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase))
        {
            string shopAccount = ReadEncodedAccount("taikhoan_shop", "cookie_userinfo_shop_bcorn");
            if (!string.IsNullOrWhiteSpace(shopAccount))
            {
                return new CurrentContext
                {
                    AccountKey = shopAccount,
                    Scope = PortalScope_cl.ScopeShop,
                    IsManagedHomeSession = false
                };
            }
        }

        string homeAccount = ReadEncodedAccount("taikhoan_home", "cookie_userinfo_home_bcorn");
        if (!string.IsNullOrWhiteSpace(homeAccount))
        {
            return new CurrentContext
            {
                AccountKey = homeAccount,
                Scope = PortalScope_cl.ScopeHome,
                IsManagedHomeSession = false
            };
        }

        string shopFallback = ReadEncodedAccount("taikhoan_shop", "cookie_userinfo_shop_bcorn");
        if (!string.IsNullOrWhiteSpace(shopFallback))
        {
            return new CurrentContext
            {
                AccountKey = shopFallback,
                Scope = PortalScope_cl.ScopeShop,
                IsManagedHomeSession = false
            };
        }

        string adminFallback = ReadEncodedAccount("taikhoan", "cookie_userinfo_admin_bcorn");
        if (!string.IsNullOrWhiteSpace(adminFallback))
        {
            return new CurrentContext
            {
                AccountKey = adminFallback,
                Scope = PortalScope_cl.ScopeAdmin,
                IsManagedHomeSession = false
            };
        }

        return null;
    }

    private static void AppendHomeScopeEntries(
        dbDataContext db,
        StringBuilder html,
        RootAccount_cl.RootAccountInfo info,
        string currentSpace,
        string selectedWorkspaceOwner)
    {
        if (db == null || html == null || info == null || string.IsNullOrWhiteSpace(info.AccountKey))
            return;

        AppendManagedSpaceEntry(db, html, info.AccountKey, ModuleSpace_cl.Shop, "shop đối tác chiến lược", info.CanAccessShop, "/shop/default.aspx", currentSpace);

        if (info.CanAccessAdmin)
        {
            AppendLink(
                html,
                "Truy cập không gian Admin",
                ResolveAdminUrl(db, info.AccountKey),
                BuildApprovedHint(ModuleSpace_cl.Admin),
                string.Equals(currentSpace, ModuleSpace_cl.Admin, StringComparison.OrdinalIgnoreCase),
                false);
        }

        AppendGianHangAdminWorkspaceLinks(db, html, info.AccountKey, currentSpace, selectedWorkspaceOwner);
        AppendOwnGianHangAdminRequestLink(db, html, info.AccountKey);
        AppendOwnGianHangSpaceEntry(db, html, info, currentSpace);
        AppendManagedSpaceEntry(db, html, info.AccountKey, ModuleSpace_cl.DauGia, "đấu giá", info.CanAccessDauGia, "/daugia/admin/", currentSpace);
        AppendManagedSpaceEntry(db, html, info.AccountKey, ModuleSpace_cl.Event, "sự kiện", info.CanAccessEvent, "/event/admin/", currentSpace);
    }

    private static void AppendActiveEntries(
        dbDataContext db,
        StringBuilder html,
        RootAccount_cl.RootAccountInfo info,
        string currentSpace,
        string selectedWorkspaceOwner)
    {
        if (db == null || html == null || info == null)
            return;

        if (info.CanAccessShop)
        {
            AppendLink(
                html,
                "Truy cập không gian Shop",
                "/shop/default.aspx",
                BuildApprovedHint(ModuleSpace_cl.Shop),
                string.Equals(currentSpace, ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase),
                false);
        }

        if (info.CanAccessGianHang)
        {
            AppendLink(
                html,
                "Truy cập không gian gian hàng đối tác",
                "/gianhang/",
                BuildApprovedHint(ModuleSpace_cl.GianHang),
                string.Equals(currentSpace, ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase),
                false);
        }

        if (info.CanAccessGianHangAdmin)
        {
            IList<GianHangAdminWorkspace_cl.WorkspaceInfo> workspaces = GianHangAdminWorkspace_cl.GetAvailableWorkspaces(db, info.AccountKey);
            if (workspaces != null && workspaces.Count > 0)
            {
                AppendGianHangAdminWorkspaceLinks(db, html, info.AccountKey, currentSpace, selectedWorkspaceOwner);
            }
            else
            {
                AppendLink(
                    html,
                    "Truy cập không gian quản trị gian hàng",
                    "/gianhang/admin/",
                    BuildApprovedHint(ModuleSpace_cl.GianHangAdmin),
                    string.Equals(currentSpace, ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase),
                    false);
            }
        }

        if (info.CanAccessDauGia)
        {
            AppendLink(
                html,
                "Truy cập không gian đấu giá",
                "/daugia/admin/",
                BuildApprovedHint(ModuleSpace_cl.DauGia),
                string.Equals(currentSpace, ModuleSpace_cl.DauGia, StringComparison.OrdinalIgnoreCase),
                false);
        }

        if (info.CanAccessEvent)
        {
            AppendLink(
                html,
                "Truy cập không gian sự kiện",
                "/event/admin/",
                BuildApprovedHint(ModuleSpace_cl.Event),
                string.Equals(currentSpace, ModuleSpace_cl.Event, StringComparison.OrdinalIgnoreCase),
                false);
        }

        if (info.CanAccessAdmin)
        {
            AppendLink(
                html,
                "Truy cập không gian Admin",
                ResolveAdminUrl(db, info.AccountKey),
                BuildApprovedHint(ModuleSpace_cl.Admin),
                string.Equals(currentSpace, ModuleSpace_cl.Admin, StringComparison.OrdinalIgnoreCase),
                false);
        }
    }

    private static void AppendGianHangAdminWorkspaceLinks(
        dbDataContext db,
        StringBuilder html,
        string accountKey,
        string currentSpace,
        string selectedWorkspaceOwner)
    {
        if (db == null || html == null || string.IsNullOrWhiteSpace(accountKey))
            return;

        IList<GianHangAdminWorkspace_cl.WorkspaceInfo> workspaces = GianHangAdminWorkspace_cl.GetAvailableWorkspaces(db, accountKey);
        if (workspaces == null)
            return;

        foreach (GianHangAdminWorkspace_cl.WorkspaceInfo workspace in workspaces)
        {
            if (workspace == null || string.IsNullOrWhiteSpace(workspace.OwnerAccountKey))
                continue;

            string ownerDisplay = (workspace.OwnerDisplayName ?? workspace.OwnerAccountKey ?? "").Trim();
            string roleLabel = (workspace.RoleLabel ?? "").Trim();
            string label = workspace.IsOwner
                ? "Quản trị gian hàng của bạn"
                : ("Quản trị gian hàng của " + ownerDisplay);
            string hint = workspace.IsOwner
                ? "Quản trị viên chính"
                : (roleLabel == "" ? "Đã tham gia quản trị" : roleLabel);
            bool active = string.Equals(currentSpace, ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase)
                && string.Equals((workspace.OwnerAccountKey ?? "").Trim(), (selectedWorkspaceOwner ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

            AppendLink(
                html,
                label,
                "/gianhang/admin?space_owner=" + HttpUtility.UrlEncode(workspace.OwnerAccountKey),
                hint,
                active,
                false);
        }
    }

    private static void AppendOwnGianHangAdminRequestLink(dbDataContext db, StringBuilder html, string accountKey)
    {
        if (db == null || html == null || string.IsNullOrWhiteSpace(accountKey))
            return;

        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, accountKey);
        if (account == null || SpaceAccess_cl.CanAccessGianHangAdmin(db, account))
            return;

        AppendLink(
            html,
            "Đăng ký quản trị nâng cao",
            "/home/mo-khong-gian.aspx?space=gianhang_admin&return_url=" + HttpUtility.UrlEncode("/gianhang/admin"),
            BuildRequestStatusHint(db, accountKey, ModuleSpace_cl.GianHangAdmin, BuildRequestFallbackHint(ModuleSpace_cl.GianHangAdmin)),
            false,
            true);
    }

    private static void AppendOwnGianHangSpaceEntry(
        dbDataContext db,
        StringBuilder html,
        RootAccount_cl.RootAccountInfo info,
        string currentSpace)
    {
        if (db == null || html == null || info == null || string.IsNullOrWhiteSpace(info.AccountKey))
            return;

        if (info.CanAccessGianHang)
        {
            AppendLink(
                html,
                "Truy cập gian hàng đối tác",
                "/gianhang/",
                BuildApprovedHint(ModuleSpace_cl.GianHang),
                string.Equals(currentSpace, ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase),
                false);
            return;
        }

        AppendLink(
            html,
            "Đăng ký gian hàng đối tác",
            "/home/dang-ky-gian-hang-doi-tac.aspx?return_url=" + HttpUtility.UrlEncode("/gianhang/"),
            BuildRequestStatusHint(db, info.AccountKey, ModuleSpace_cl.GianHang, BuildRequestFallbackHint(ModuleSpace_cl.GianHang)),
            false,
            true);
    }

    private static void AppendManagedSpaceEntry(
        dbDataContext db,
        StringBuilder html,
        string accountKey,
        string spaceCode,
        string labelLower,
        bool canAccess,
        string accessUrl,
        string currentSpace)
    {
        if (db == null || html == null || string.IsNullOrWhiteSpace(accountKey))
            return;

        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (normalizedSpace == "")
            return;

        if (canAccess)
        {
            AppendLink(
                html,
                "Truy cập không gian " + labelLower,
                accessUrl,
                BuildApprovedHint(normalizedSpace),
                string.Equals(currentSpace, normalizedSpace, StringComparison.OrdinalIgnoreCase),
                false);
            return;
        }

        AppendLink(
            html,
            "Đăng ký không gian " + labelLower,
            HomeSpaceAccess_cl.BuildAccessPageUrl(normalizedSpace, accessUrl),
            BuildRequestStatusHint(db, accountKey, normalizedSpace, BuildRequestFallbackHint(normalizedSpace)),
            false,
            true);
    }

    private static string BuildApprovedHint(string spaceCode)
    {
        string normalized = ModuleSpace_cl.Normalize(spaceCode);
        if (normalized == ModuleSpace_cl.GianHangAdmin)
            return "Quản trị nâng cao đã sẵn sàng";
        return "Đã sẵn sàng sử dụng";
    }

    private static string BuildRequestFallbackHint(string spaceCode)
    {
        string normalized = ModuleSpace_cl.Normalize(spaceCode);
        if (normalized == ModuleSpace_cl.GianHangAdmin)
            return "Chưa duyệt · Có thể gửi yêu cầu quản trị";
        return "Chưa duyệt · Có thể gửi yêu cầu";
    }

    private static string BuildRequestStatusHint(dbDataContext db, string accountKey, string spaceCode, string fallback)
    {
        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return fallback;

        CoreSpaceRequest_cl.SpaceRequestInfo latestRequest = CoreSpaceRequest_cl.GetLatestRequest(db, accountKey, spaceCode);
        if (latestRequest == null)
            return fallback;

        string status = (latestRequest.RequestStatus ?? "").Trim();
        if (string.Equals(status, CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase))
            return "Đang chờ duyệt · đã gửi yêu cầu";
        if (string.Equals(status, CoreSpaceRequest_cl.StatusRejected, StringComparison.OrdinalIgnoreCase))
            return "Yêu cầu gần nhất bị từ chối";
        if (string.Equals(status, CoreSpaceRequest_cl.StatusCancelled, StringComparison.OrdinalIgnoreCase))
            return "Yêu cầu gần nhất đã hủy";
        if (string.Equals(status, CoreSpaceRequest_cl.StatusApproved, StringComparison.OrdinalIgnoreCase))
            return "Yêu cầu gần nhất đã duyệt";

        return fallback;
    }

    private static void AppendLink(
        StringBuilder html,
        string label,
        string url,
        string hint,
        bool active,
        bool requestTone)
    {
        if (html == null || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(label))
            return;

        string css = "aha-space-launcher__link";
        if (active)
            css += " is-active";
        if (requestTone)
            css += " is-request";

        html.AppendFormat(
            @"<a class=""{0}"" href=""{1}""><span class=""aha-space-launcher__copy""><strong>{2}</strong><span>{3}</span></span><span class=""aha-space-launcher__chevron"">&rsaquo;</span></a>",
            css,
            HttpUtility.HtmlAttributeEncode(url),
            HttpUtility.HtmlEncode(label),
            HttpUtility.HtmlEncode(hint ?? ""));
    }

    private static string ResolveAdminUrl(dbDataContext db, string accountKey)
    {
        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return "/admin/default.aspx";

        return AdminRolePolicy_cl.ResolveLandingUrl(db, accountKey);
    }

    private static string ReadEncodedAccount(string sessionKey, string cookieName)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null)
            return "";

        string encoded = "";
        if (ctx.Session != null && !string.IsNullOrWhiteSpace(sessionKey))
            encoded = (ctx.Session[sessionKey] as string) ?? "";

        if (string.IsNullOrWhiteSpace(encoded) && ctx.Request != null && ctx.Request.Cookies != null && !string.IsNullOrWhiteSpace(cookieName))
        {
            HttpCookie cookie = ctx.Request.Cookies[cookieName];
            if (cookie != null)
                encoded = cookie["taikhoan"] ?? "";
        }

        return DecodeAccount(encoded);
    }

    private static string DecodeAccount(string encoded)
    {
        string value = (encoded ?? "").Trim();
        if (value == "")
            return "";

        try
        {
            return Normalize(mahoa_cl.giaima_Bcorn(value));
        }
        catch
        {
            return Normalize(value);
        }
    }

    private static string CurrentPath()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Request == null)
            return "";

        string raw = ctx.Request.RawUrl ?? "";
        int queryIndex = raw.IndexOf('?');
        if (queryIndex >= 0)
            raw = raw.Substring(0, queryIndex);

        if (raw == "" && ctx.Request.Url != null)
            raw = ctx.Request.Url.AbsolutePath ?? "";

        return raw ?? "";
    }

    private static string NormalizePath(string raw)
    {
        string value = (raw ?? "").Trim();
        if (value == "")
            return "";

        value = value.ToLowerInvariant();
        while (value.Length > 1 && value.EndsWith("/"))
            value = value.Substring(0, value.Length - 1);
        return value;
    }

    private static string ResolveSpaceByPath(string path)
    {
        string normalized = NormalizePath(path);
        if (normalized == "" || normalized == "/" || normalized == "/default.aspx")
            return ModuleSpace_cl.Home;
        if (normalized.StartsWith("/home/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.Home;

        string resolved = ModuleSpace_cl.ResolveByPath(normalized);
        if (resolved == "")
            return ModuleSpace_cl.Home;
        return resolved;
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
