using System;
using System.Web;

public static class GianHangAdminContext_cl
{
    public static bool IsHomeManagedSession()
    {
        return ResolveCurrentHomeAccountKey() != "";
    }

    public static string ResolveCurrentHomeAccountKey()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return "";

        return Normalize((ctx.Session["gianhang_admin_home"] ?? "").ToString());
    }

    public static string ResolveCurrentOwnerAccountKey()
    {
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
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return "";

        string role = ((ctx.Session["gianhang_admin_role"] ?? "") + "").Trim();
        if (role != "")
            return role;

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
