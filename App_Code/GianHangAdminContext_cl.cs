using System;
using System.Web;

public static class GianHangAdminContext_cl
{
    public static bool IsHomeManagedSession()
    {
        if (WorkspaceContext_cl.IsCurrentWorkspace(GianHangWorkspacePolicy_cl.WorkspaceKey))
        {
            WorkspaceContext_cl.WorkspaceSessionContext current = WorkspaceContext_cl.GetCurrent();
            if (current != null && current.IsManagedWorkspace)
                return true;
        }

        return ResolveCurrentHomeAccountKey() != "";
    }

    public static string ResolveCurrentHomeAccountKey()
    {
        WorkspaceContext_cl.WorkspaceSessionContext current = WorkspaceContext_cl.GetCurrent();
        if (current != null && WorkspaceContext_cl.IsCurrentWorkspace(GianHangWorkspacePolicy_cl.WorkspaceKey))
        {
            string homeKey = Normalize(current.HomeAccountKey);
            if (homeKey != "")
                return homeKey;
        }

        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return "";

        return Normalize((ctx.Session["gianhang_admin_home"] ?? "").ToString());
    }

    public static string ResolveCurrentOwnerAccountKey()
    {
        WorkspaceContext_cl.WorkspaceSessionContext current = WorkspaceContext_cl.GetCurrent();
        if (current != null && WorkspaceContext_cl.IsCurrentWorkspace(GianHangWorkspacePolicy_cl.WorkspaceKey))
        {
            string ownerKey = Normalize(current.OwnerAccountKey);
            if (ownerKey != "")
                return ownerKey;
        }

        HttpContext ctx = HttpContext.Current;
        if (ctx != null && ctx.Session != null)
        {
            string owner = Normalize((ctx.Session["gianhang_admin_owner"] ?? "").ToString());
            if (owner != "")
                return owner;
        }

        string fromSession = Normalize(AhaShineContext_cl.ResolveUserParent());
        if (fromSession != "")
            return fromSession;

        return "";
    }

    public static string ResolveCurrentLegacyUser()
    {
        WorkspaceContext_cl.WorkspaceSessionContext current = WorkspaceContext_cl.GetCurrent();
        if (current != null && WorkspaceContext_cl.IsCurrentWorkspace(GianHangWorkspacePolicy_cl.WorkspaceKey))
        {
            string legacyUser = Normalize(current.LegacyUser);
            if (legacyUser != "")
                return legacyUser;
        }

        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return "";

        return Normalize((ctx.Session["user"] ?? "").ToString());
    }

    public static string ResolveCurrentMode()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return "";

        return Normalize((ctx.Session["gianhang_admin_mode"] ?? "").ToString());
    }

    public static string ResolveCurrentRoleLabel()
    {
        WorkspaceContext_cl.WorkspaceSessionContext current = WorkspaceContext_cl.GetCurrent();
        if (current != null && WorkspaceContext_cl.IsCurrentWorkspace(GianHangWorkspacePolicy_cl.WorkspaceKey))
        {
            string currentRole = (current.RoleLabel ?? "").Trim();
            if (currentRole != "")
                return currentRole;
        }

        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return "";

        string sessionRole = ((ctx.Session["gianhang_admin_role"] ?? "") + "").Trim();
        if (sessionRole != "")
            return sessionRole;

        string mode = ResolveCurrentMode();
        if (mode == "owner")
            return "Chủ không gian";
        if (mode == "member")
            return "Nhân viên";

        return "";
    }

    public static string ResolveDisplayAccountKey()
    {
        if (IsHomeManagedSession())
            return ResolveCurrentHomeAccountKey();

        HttpContext ctx = HttpContext.Current;
        string taiKhoanMaHoa = ctx != null && ctx.Session != null ? (ctx.Session["taikhoan"] as string) : "";
        if (string.IsNullOrWhiteSpace(taiKhoanMaHoa))
            return "";

        try
        {
            return Normalize(mahoa_cl.giaima_Bcorn(taiKhoanMaHoa));
        }
        catch
        {
            return Normalize(taiKhoanMaHoa);
        }
    }

    public static taikhoan_tb ResolveDisplayAccount(dbDataContext db)
    {
        if (db == null)
            return null;

        string accountKey = ResolveDisplayAccountKey();
        if (accountKey == "")
            return null;

        return RootAccount_cl.GetByAccountKey(db, accountKey);
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
