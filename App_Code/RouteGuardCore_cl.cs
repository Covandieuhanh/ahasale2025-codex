using System;

public static class RouteGuardCore_cl
{
    public static string ResolveCurrentSpace()
    {
        if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null)
            return ModuleSpace_cl.Home;

        string path = System.Web.HttpContext.Current.Request.Url == null
            ? ""
            : (System.Web.HttpContext.Current.Request.Url.AbsolutePath ?? "");

        string resolved = ModuleSpace_cl.ResolveByPath(path);
        return resolved == "" ? ModuleSpace_cl.Home : resolved;
    }

    public static bool CanCurrentAccountAccessCurrentPath()
    {
        taikhoan_tb account = RootAccount_cl.GetCurrentEntity();
        if (account == null)
            return false;

        string currentSpace = ResolveCurrentSpace();
        return SpaceAccess_cl.CanAccessSpace(account, currentSpace);
    }

    public static string GetCurrentDeniedMessage()
    {
        return SpaceAccess_cl.GetDeniedMessage(ResolveCurrentSpace());
    }
}
