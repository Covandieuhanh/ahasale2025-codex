using System;
using System.Collections.Generic;
using System.Linq;

public static class CoreSpaceRequest_cl
{
    public const string StatusPending = "pending";
    public const string StatusApproved = "approved";
    public const string StatusRejected = "rejected";
    public const string StatusCancelled = "cancelled";

    public sealed class SpaceRequestInfo
    {
        public long Id { get; set; }
        public string AccountKey { get; set; }
        public string SpaceCode { get; set; }
        public string RequestStatus { get; set; }
        public string RequestSource { get; set; }
        public string RequestedBy { get; set; }
        public string RequestMetaJson { get; set; }
        public DateTime? RequestedAt { get; set; }
        public string ReviewNote { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public static SpaceRequestInfo GetLatestRequest(dbDataContext db, string accountKey, string spaceCode)
    {
        if (db == null)
            return null;

        string key = (accountKey ?? "").Trim();
        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (key == "" || normalizedSpace == "")
            return null;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        return db.ExecuteQuery<SpaceRequestInfo>(
                "SELECT TOP 1 Id, AccountKey, SpaceCode, RequestStatus, RequestSource, RequestedBy, RequestMetaJson, RequestedAt, ReviewNote, ReviewedBy, ReviewedAt, UpdatedAt " +
                "FROM dbo.CoreSpaceRequest_tb WHERE AccountKey = {0} AND SpaceCode = {1} ORDER BY RequestedAt DESC, Id DESC",
                key,
                normalizedSpace)
            .FirstOrDefault();
    }

    public static SpaceRequestInfo GetRequestById(dbDataContext db, long requestId)
    {
        if (db == null || requestId <= 0)
            return null;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        return db.ExecuteQuery<SpaceRequestInfo>(
                "SELECT TOP 1 Id, AccountKey, SpaceCode, RequestStatus, RequestSource, RequestedBy, RequestMetaJson, RequestedAt, ReviewNote, ReviewedBy, ReviewedAt, UpdatedAt " +
                "FROM dbo.CoreSpaceRequest_tb WHERE Id = {0}",
                requestId)
            .FirstOrDefault();
    }

    public static bool HasPendingRequest(dbDataContext db, string accountKey, string spaceCode)
    {
        SpaceRequestInfo info = GetLatestRequest(db, accountKey, spaceCode);
        return info != null && string.Equals(info.RequestStatus, StatusPending, StringComparison.OrdinalIgnoreCase);
    }

    public static bool TryCreateRequest(
        dbDataContext db,
        string accountKey,
        string spaceCode,
        string requestSource,
        string requestedBy,
        string requestMetaJson,
        out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Data connection is not available.";
            return false;
        }

        string key = (accountKey ?? "").Trim();
        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (key == "" || normalizedSpace == "")
        {
            message = "Account key or space code is invalid.";
            return false;
        }

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        SpaceAccess_cl.SpaceAccessRow access = SpaceAccess_cl.GetAccessRow(db, key, normalizedSpace);
        if (access != null && string.Equals(access.AccessStatus, SpaceAccess_cl.StatusActive, StringComparison.OrdinalIgnoreCase))
        {
            message = "This account already has access to the requested space.";
            return false;
        }

        if (HasPendingRequest(db, key, normalizedSpace))
        {
            message = "There is already a pending request for this space.";
            return false;
        }

        string requestMeta = (requestMetaJson ?? "").Trim();
        if (string.Equals(normalizedSpace, ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase))
        {
            int parsedPercent;
            if (!ShopPolicy_cl.TryParseCommissionPercent(requestMeta, out parsedPercent))
            {
                message = "Shop request requires a valid commission percent (0-100).";
                return false;
            }

            requestMeta = ShopPolicy_cl.BuildRequestMeta(parsedPercent);
        }

        db.ExecuteCommand(
            "INSERT INTO dbo.CoreSpaceRequest_tb (AccountKey, SpaceCode, RequestStatus, RequestSource, RequestedBy, RequestMetaJson, RequestedAt, UpdatedAt) " +
            "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
            key,
            normalizedSpace,
            StatusPending,
            (requestSource ?? "").Trim(),
            (requestedBy ?? "").Trim(),
            requestMeta,
            AhaTime_cl.Now,
            AhaTime_cl.Now);

        message = "The request has been created successfully.";
        return true;
    }

    public static bool TryCreateShopRequest(
        dbDataContext db,
        string accountKey,
        int commissionPercent,
        string requestSource,
        string requestedBy,
        out string message)
    {
        return TryCreateRequest(
            db,
            accountKey,
            ModuleSpace_cl.Shop,
            requestSource,
            requestedBy,
            ShopPolicy_cl.BuildRequestMeta(commissionPercent),
            out message);
    }

    public static bool ApproveRequest(dbDataContext db, long requestId, string reviewedBy, string reviewNote, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Data connection is not available.";
            return false;
        }

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        SpaceRequestInfo request = GetRequestById(db, requestId);

        if (request == null)
        {
            message = "The request could not be found.";
            return false;
        }

        if (!string.Equals(request.RequestStatus, StatusPending, StringComparison.OrdinalIgnoreCase))
        {
            message = "Only pending requests can be approved.";
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        db.ExecuteCommand(
            "UPDATE dbo.CoreSpaceRequest_tb SET RequestStatus = {1}, ReviewNote = {2}, ReviewedBy = {3}, ReviewedAt = {4}, UpdatedAt = {4} WHERE Id = {0}",
            requestId,
            StatusApproved,
            (reviewNote ?? "").Trim(),
            (reviewedBy ?? "").Trim(),
            now);

        SpaceAccess_cl.UpsertSpaceAccess(
            db,
            request.AccountKey,
            request.SpaceCode,
            SpaceAccess_cl.StatusActive,
            "request_approved",
            ModuleSpace_cl.Normalize(request.SpaceCode) == ModuleSpace_cl.Home,
            reviewedBy,
            now,
            "",
            request.Id);

        if (string.Equals(ModuleSpace_cl.Normalize(request.SpaceCode), ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase))
        {
            int requestedPercent = ShopPolicy_cl.ExtractRequestedCommissionPercent(request.RequestMetaJson, 0);
            ShopPolicy_cl.UpsertPolicy(db, request.AccountKey, requestedPercent, request.Id, reviewedBy, reviewNote);
        }

        message = "The request has been approved.";
        return true;
    }

    public static bool RejectRequest(dbDataContext db, long requestId, string reviewedBy, string reviewNote, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Data connection is not available.";
            return false;
        }

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        SpaceRequestInfo request = GetRequestById(db, requestId);

        if (request == null)
        {
            message = "The request could not be found.";
            return false;
        }

        if (!string.Equals(request.RequestStatus, StatusPending, StringComparison.OrdinalIgnoreCase))
        {
            message = "Only pending requests can be rejected.";
            return false;
        }

        db.ExecuteCommand(
            "UPDATE dbo.CoreSpaceRequest_tb SET RequestStatus = {1}, ReviewNote = {2}, ReviewedBy = {3}, ReviewedAt = {4}, UpdatedAt = {4} WHERE Id = {0}",
            requestId,
            StatusRejected,
            (reviewNote ?? "").Trim(),
            (reviewedBy ?? "").Trim(),
            AhaTime_cl.Now);

        message = "The request has been rejected.";
        return true;
    }

    public static bool CancelApprovedRequest(dbDataContext db, long requestId, string reviewedBy, string reviewNote, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Data connection is not available.";
            return false;
        }

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        SpaceRequestInfo request = GetRequestById(db, requestId);

        if (request == null)
        {
            message = "The request could not be found.";
            return false;
        }

        if (!string.Equals(request.RequestStatus, StatusApproved, StringComparison.OrdinalIgnoreCase))
        {
            message = "Only approved requests can be cancelled.";
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        db.ExecuteCommand(
            "UPDATE dbo.CoreSpaceRequest_tb SET RequestStatus = {1}, ReviewNote = {2}, ReviewedBy = {3}, ReviewedAt = {4}, UpdatedAt = {4} WHERE Id = {0}",
            requestId,
            StatusCancelled,
            (reviewNote ?? "").Trim(),
            (reviewedBy ?? "").Trim(),
            now);

        SpaceAccess_cl.DeleteSpaceAccess(db, request.AccountKey, request.SpaceCode);

        if (string.Equals(ModuleSpace_cl.Normalize(request.SpaceCode), ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase))
            ShopPolicy_cl.DeactivatePolicy(db, request.AccountKey, reviewedBy, reviewNote);

        message = "The approved request has been cancelled and access has been closed.";
        return true;
    }

    public static List<SpaceRequestInfo> LoadRequests(dbDataContext db, string accountKey, string spaceCode, string requestStatus)
    {
        if (db == null)
            return new List<SpaceRequestInfo>();

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        List<SpaceRequestInfo> list = db.ExecuteQuery<SpaceRequestInfo>(
                "SELECT Id, AccountKey, SpaceCode, RequestStatus, RequestSource, RequestedBy, RequestMetaJson, RequestedAt, ReviewNote, ReviewedBy, ReviewedAt, UpdatedAt FROM dbo.CoreSpaceRequest_tb")
            .ToList();

        string key = (accountKey ?? "").Trim();
        if (key != "")
            list = list.Where(p => string.Equals((p.AccountKey ?? "").Trim(), key, StringComparison.OrdinalIgnoreCase)).ToList();

        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (normalizedSpace != "")
            list = list.Where(p => string.Equals((p.SpaceCode ?? "").Trim(), normalizedSpace, StringComparison.OrdinalIgnoreCase)).ToList();

        string status = NormalizeStatus(requestStatus);
        if (status != "")
            list = list.Where(p => string.Equals((p.RequestStatus ?? "").Trim(), status, StringComparison.OrdinalIgnoreCase)).ToList();

        return list.OrderByDescending(p => p.RequestedAt).ThenByDescending(p => p.Id).ToList();
    }

    private static string NormalizeStatus(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == StatusPending) return StatusPending;
        if (value == StatusApproved) return StatusApproved;
        if (value == StatusRejected) return StatusRejected;
        if (value == StatusCancelled) return StatusCancelled;
        return "";
    }
}
