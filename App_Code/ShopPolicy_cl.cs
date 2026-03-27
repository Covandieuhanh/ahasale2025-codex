using System;
using System.Linq;
using System.Text.RegularExpressions;

public static class ShopPolicy_cl
{
    public const int MinCommissionPercent = 0;
    public const int MaxCommissionPercent = 100;
    public const string PolicyStatusActive = "active";
    public const string PolicyStatusInactive = "inactive";

    private static readonly Regex CommissionRegex = new Regex(@"(?:^|;)\s*commission_percent\s*=\s*(\d{1,3})\s*(?:;|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static int ClampCommissionPercent(int value)
    {
        if (value < MinCommissionPercent) return MinCommissionPercent;
        if (value > MaxCommissionPercent) return MaxCommissionPercent;
        return value;
    }

    public static bool TryParseCommissionPercent(string raw, out int value)
    {
        value = 0;
        string text = (raw ?? string.Empty).Trim();
        if (text == string.Empty)
            return false;

        Match match = CommissionRegex.Match(text + ";");
        if (match.Success && match.Groups.Count >= 2)
            text = match.Groups[1].Value;

        int parsed;
        if (!int.TryParse(text, out parsed))
            return false;
        if (parsed < MinCommissionPercent || parsed > MaxCommissionPercent)
            return false;

        value = parsed;
        return true;
    }

    public static string BuildRequestMeta(int commissionPercent)
    {
        return "commission_percent=" + ClampCommissionPercent(commissionPercent).ToString();
    }

    public static int ExtractRequestedCommissionPercent(string requestMeta, int fallbackValue)
    {
        string meta = (requestMeta ?? string.Empty).Trim();
        if (meta == string.Empty)
            return ClampCommissionPercent(fallbackValue);

        Match match = CommissionRegex.Match(meta + ";");
        if (!match.Success || match.Groups.Count < 2)
            return ClampCommissionPercent(fallbackValue);

        int parsed;
        if (!int.TryParse(match.Groups[1].Value, out parsed))
            return ClampCommissionPercent(fallbackValue);

        return ClampCommissionPercent(parsed);
    }

    public static bool TryGetActivePolicyPercent(dbDataContext db, string accountKey, out int percent)
    {
        percent = 0;
        if (db == null)
            return false;

        string account = (accountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (account == string.Empty)
            return false;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        int? value = db.ExecuteQuery<int?>(
            "SELECT TOP 1 CommissionPercent " +
            "FROM dbo.CoreShopPolicy_tb " +
            "WHERE AccountKey = {0} AND PolicyStatus = {1} " +
            "ORDER BY UpdatedAt DESC, Id DESC",
            account,
            PolicyStatusActive).FirstOrDefault();

        if (!value.HasValue)
            return false;

        percent = ClampCommissionPercent(value.Value);
        return true;
    }

    public static int ResolveCommissionPercentForSeller(dbDataContext db, string accountKey, int fallbackPercent)
    {
        int fromPolicy;
        if (TryGetActivePolicyPercent(db, accountKey, out fromPolicy))
            return fromPolicy;
        return ClampCommissionPercent(fallbackPercent);
    }

    public static void UpsertPolicy(
        dbDataContext db,
        string accountKey,
        int commissionPercent,
        long? approvedRequestId,
        string approvedBy,
        string note)
    {
        if (db == null)
            return;

        string account = (accountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (account == string.Empty)
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        DateTime now = AhaTime_cl.Now;
        int percent = ClampCommissionPercent(commissionPercent);
        string reviewer = (approvedBy ?? string.Empty).Trim();
        string reviewNote = (note ?? string.Empty).Trim();
        long? requestId = approvedRequestId.HasValue && approvedRequestId.Value > 0 ? approvedRequestId : null;

        int updated;
        if (requestId.HasValue)
        {
            updated = db.ExecuteCommand(
                "UPDATE dbo.CoreShopPolicy_tb " +
                "SET CommissionPercent = {1}, PolicyStatus = {2}, ApprovedRequestId = {3}, ApprovedBy = {4}, ApprovedAt = {5}, Note = {6}, UpdatedAt = {5} " +
                "WHERE AccountKey = {0}",
                account,
                percent,
                PolicyStatusActive,
                requestId.Value,
                reviewer,
                now,
                reviewNote);
        }
        else
        {
            updated = db.ExecuteCommand(
                "UPDATE dbo.CoreShopPolicy_tb " +
                "SET CommissionPercent = {1}, PolicyStatus = {2}, ApprovedRequestId = NULL, ApprovedBy = {3}, ApprovedAt = {4}, Note = {5}, UpdatedAt = {4} " +
                "WHERE AccountKey = {0}",
                account,
                percent,
                PolicyStatusActive,
                reviewer,
                now,
                reviewNote);
        }

        if (updated > 0)
            return;

        if (requestId.HasValue)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreShopPolicy_tb " +
                "(AccountKey, CommissionPercent, PolicyStatus, ApprovedRequestId, ApprovedBy, ApprovedAt, Note, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {5}, {5})",
                account,
                percent,
                PolicyStatusActive,
                requestId.Value,
                reviewer,
                now,
                reviewNote);
        }
        else
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreShopPolicy_tb " +
                "(AccountKey, CommissionPercent, PolicyStatus, ApprovedRequestId, ApprovedBy, ApprovedAt, Note, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, NULL, {3}, {4}, {5}, {4}, {4})",
                account,
                percent,
                PolicyStatusActive,
                reviewer,
                now,
                reviewNote);
        }
    }

    public static void DeactivatePolicy(
        dbDataContext db,
        string accountKey,
        string approvedBy,
        string note)
    {
        if (db == null)
            return;

        string account = (accountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (account == string.Empty)
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        DateTime now = AhaTime_cl.Now;
        db.ExecuteCommand(
            "UPDATE dbo.CoreShopPolicy_tb " +
            "SET PolicyStatus = {1}, ApprovedBy = {2}, ApprovedAt = {3}, Note = {4}, UpdatedAt = {3} " +
            "WHERE AccountKey = {0}",
            account,
            PolicyStatusInactive,
            (approvedBy ?? string.Empty).Trim(),
            now,
            (note ?? string.Empty).Trim());
    }
}
