using System;

public static class GianHangPolicy_cl
{
    public static bool CanAccess(string account)
    {
        account = (account ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(account))
            return false;

        return CoreDb_cl.Use(db =>
        {
            taikhoan_tb entity = RootAccount_cl.GetByAccountKey(db, account);
            return SpaceAccess_cl.CanAccessGianHang(db, entity);
        });
    }

    public static bool CanCurrentAccountAccess()
    {
        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        return info != null && info.IsAuthenticated && info.CanAccessGianHang;
    }

    public static string GetCurrentAccessStatus()
    {
        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated)
            return "";

        return CoreDb_cl.Use(db => SpaceAccess_cl.GetSpaceStatus(db, info.AccountKey, GianHangBootstrap_cl.SpaceCode));
    }

    public static CoreSpaceRequest_cl.SpaceRequestInfo GetCurrentLatestRequest()
    {
        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated)
            return null;

        return CoreDb_cl.Use(db => GianHangBootstrap_cl.GetLatestRequest(db, info.AccountKey));
    }

    public static bool TryCreateRequestForCurrentAccount(out string userMessage)
    {
        userMessage = GianHangNotify_cl.BuildErrorMessage("");

        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated)
        {
            userMessage = GianHangNotify_cl.BuildNeedHomeMessage();
            return false;
        }

        if (!info.CanAccessHome)
        {
            userMessage = GianHangNotify_cl.BuildNeedHomeMessage();
            return false;
        }

        if (info.CanAccessGianHang)
        {
            userMessage = GianHangNotify_cl.BuildAlreadyActiveMessage();
            return false;
        }

        string resolvedMessage = userMessage;
        bool created = CoreDb_cl.Use(db =>
        {
            taikhoan_tb entity = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
            if (entity == null)
            {
                resolvedMessage = GianHangNotify_cl.BuildNeedHomeMessage();
                return false;
            }

            string message;
            bool ok = CoreSpaceRequest_cl.TryCreateRequest(
                db,
                info.AccountKey,
                GianHangBootstrap_cl.SpaceCode,
                "gianhang",
                info.AccountKey,
                null,
                out message);

            if (ok)
            {
                resolvedMessage = GianHangNotify_cl.BuildRequestCreatedMessage();
                return true;
            }

            string latestStatus = SpaceAccess_cl.GetSpaceStatus(db, info.AccountKey, GianHangBootstrap_cl.SpaceCode);
            if (string.Equals(latestStatus, SpaceAccess_cl.StatusBlocked, StringComparison.OrdinalIgnoreCase))
            {
                resolvedMessage = GianHangNotify_cl.BuildBlockedMessage();
                return false;
            }

            if (string.Equals(latestStatus, SpaceAccess_cl.StatusRevoked, StringComparison.OrdinalIgnoreCase))
            {
                resolvedMessage = GianHangNotify_cl.BuildRevokedMessage();
                return false;
            }

            CoreSpaceRequest_cl.SpaceRequestInfo latestRequest = GianHangBootstrap_cl.GetLatestRequest(db, info.AccountKey);
            if (latestRequest != null && string.Equals(latestRequest.RequestStatus, CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase))
            {
                resolvedMessage = GianHangNotify_cl.BuildPendingMessage();
                return false;
            }

            resolvedMessage = string.IsNullOrWhiteSpace(message)
                ? GianHangNotify_cl.BuildErrorMessage("")
                : GianHangNotify_cl.BuildWarningMessage(message);
            return false;
        });

        userMessage = resolvedMessage;
        return created;
    }
}
