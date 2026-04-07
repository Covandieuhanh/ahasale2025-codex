using System;
using System.Linq;
using System.Web;

public static class GianHangWorkspacePolicy_cl
{
    public const string WorkspaceKey = "gianhang_admin";

    public static bool CanAccessHomeManagedWorkspace(dbDataContext db, string homeAccountKey, string ownerAccountKey)
    {
        string homeKey = Normalize(homeAccountKey);
        string ownerKey = Normalize(ownerAccountKey);
        if (db == null || homeKey == "" || ownerKey == "")
            return false;

        taikhoan_tb homeAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == homeKey);
        if (homeAccount != null && SpaceAccess_cl.CanAccessGianHangAdmin(db, homeAccount))
            return true;

        return GianHangAdminWorkspace_cl.HasAnyWorkspace(db, homeKey)
            || GianHangAdminWorkspace_cl.HasAnyWorkspace(db, ownerKey);
    }

    public static WorkspaceContext_cl.WorkspaceSessionContext BuildContext(GianHangAdminPageGuard_cl.AccessInfo access, HttpRequest request)
    {
        if (access == null)
            return null;

        return new WorkspaceContext_cl.WorkspaceSessionContext
        {
            WorkspaceKey = WorkspaceKey,
            OwnerAccountKey = Normalize(access.OwnerAccountKey),
            HomeAccountKey = Normalize(access.HomeAccountKey),
            LegacyUser = Normalize(access.User),
            RoleLabel = (access.RoleLabel ?? "").Trim(),
            Source = "gianhang_admin_guard",
            ChiNhanhId = (access.ChiNhanhId ?? "").Trim(),
            NganhId = (access.NganhId ?? "").Trim(),
            IsManagedWorkspace = true,
            IsSystemAdminMode = WorkspaceContext_cl.IsSystemAdminMode(request)
        };
    }

    public static string BuildWorkspaceHubUrl()
    {
        return WorkspaceContext_cl.AppendSystemAdminFlag(GianHangRoutes_cl.BuildAdminWorkspaceHubUrl(), HttpContext.Current != null ? HttpContext.Current.Request : null);
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
