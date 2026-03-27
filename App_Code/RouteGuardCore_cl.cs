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
        string currentSpace = ResolveCurrentSpace();
        return CoreDb_cl.Use(db =>
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);

            taikhoan_tb account = RootAccount_cl.GetCurrentEntity(db);
            if (account == null)
                return false;

            CoreAccessBootstrap_cl.EnsureAccountSeeded(db, account);
            return SpaceAccess_cl.CanAccessSpace(db, account, currentSpace);
        });
    }

    public static string GetCurrentDeniedMessage()
    {
        return SpaceAccess_cl.GetDeniedMessage(ResolveCurrentSpace());
    }
}
