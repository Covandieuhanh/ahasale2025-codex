using System;

public static class GianHangBootstrap_cl
{
    public const string BasePath = "/gianhang";
    public const string SpaceCode = ModuleSpace_cl.GianHang;

    public static void EnsureReady()
    {
        CoreAccessBootstrap_cl.EnsureCurrentAccountSeeded();
    }

    public static RootAccount_cl.RootAccountInfo GetCurrentInfo()
    {
        EnsureReady();
        return RootAccount_cl.GetCurrentInfo();
    }

    public static string BuildUrl(string relativePath)
    {
        string path = (relativePath ?? "").Trim();
        if (path == "")
            return BasePath;
        if (!path.StartsWith("/", StringComparison.Ordinal))
            path = "/" + path;
        return BasePath + path;
    }

    public static SpaceAccess_cl.SpaceAccessRow GetAccessRow(dbDataContext db, string accountKey)
    {
        return SpaceAccess_cl.GetAccessRow(db, accountKey, SpaceCode);
    }

    public static CoreSpaceRequest_cl.SpaceRequestInfo GetLatestRequest(dbDataContext db, string accountKey)
    {
        return CoreSpaceRequest_cl.GetLatestRequest(db, accountKey, SpaceCode);
    }
}
