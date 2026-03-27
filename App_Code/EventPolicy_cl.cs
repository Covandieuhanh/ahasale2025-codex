using System;
using System.Linq;

public static class EventPolicy_cl
{
    public const string StatusDraft = "draft";
    public const string StatusPendingApproval = "pending_approval";
    public const string StatusActive = "active";
    public const string StatusPaused = "paused";
    public const string StatusEnded = "ended";
    public const string StatusArchived = "archived";

    public const string CampaignTypeVoucherTier = "voucher_tier";
    public const string CampaignTypeSalaryBonusTier = "salary_bonus_tier";

    public const string AdminPermissionCode = "event_admin";
    public const string HomePermissionCode = "event_admin_home";
    public const string RoleOwner = "event_owner";
    public const string RoleDesigner = "event_designer";
    public const string RoleApprover = "event_approver";
    public const string RoleOperator = "event_operator";
    public const string RoleViewer = "event_viewer";

    public static string NormalizeStatus(string status)
    {
        string normalized = (status ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return StatusDraft;

        switch (normalized)
        {
            case StatusDraft:
            case StatusPendingApproval:
            case StatusActive:
            case StatusPaused:
            case StatusEnded:
            case StatusArchived:
                return normalized;
            default:
                return StatusDraft;
        }
    }

    public static string NormalizeCampaignType(string campaignType)
    {
        string normalized = (campaignType ?? "").Trim().ToLowerInvariant();
        if (normalized == CampaignTypeSalaryBonusTier)
            return CampaignTypeSalaryBonusTier;
        return CampaignTypeVoucherTier;
    }

    public static bool CanAccessAdmin(dbDataContext db, string account, string permissionRaw)
    {
        string normalized = (account ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return false;

        if (PermissionProfile_cl.IsRootAdmin(normalized))
            return true;

        if (db != null && check_login_cl.CheckQuyen(db, normalized, AdminPermissionCode))
            return true;
        return false;
    }

    public static bool CanAccessHomeBuilder(dbDataContext db, string account, string permissionRaw)
    {
        string normalized = (account ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return false;

        if (db != null)
        {
            taikhoan_tb homeAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == normalized);
            if (homeAccount != null && PortalScope_cl.CanLoginHome(homeAccount.taikhoan, homeAccount.phanloai, homeAccount.permission))
                return SpaceAccess_cl.CanAccessEvent(db, homeAccount);
        }

        // Fallback cho trường hợp cần dùng chung token event_admin.
        if (db != null && check_login_cl.CheckQuyen(db, normalized, AdminPermissionCode))
            return true;

        return false;
    }

    public static bool CanViewAdminWorkspace(dbDataContext db, string account)
    {
        string normalized = (account ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return false;

        if (CanAccessAdmin(db, normalized, ""))
            return true;

        if (CanAccessHomeBuilder(db, normalized, ""))
            return true;

        if (db != null && check_login_cl.CheckQuyen(db, normalized,
            RoleOwner + "," + RoleDesigner + "," + RoleApprover + "," + RoleOperator + "," + RoleViewer))
            return true;

        return false;
    }

    public static bool CanPublishProgram(dbDataContext db, string account)
    {
        string normalized = (account ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return false;

        if (CanAccessAdmin(db, normalized, ""))
            return true;

        if (CanAccessHomeBuilder(db, normalized, ""))
            return true;

        if (db != null && check_login_cl.CheckQuyen(db, normalized, RoleOwner + "," + RoleApprover))
            return true;

        return false;
    }

    public static bool CanOperateProgram(dbDataContext db, string account)
    {
        string normalized = (account ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return false;

        if (CanAccessAdmin(db, normalized, ""))
            return true;

        if (CanAccessHomeBuilder(db, normalized, ""))
            return true;

        if (db != null && check_login_cl.CheckQuyen(db, normalized, RoleOwner + "," + RoleOperator + "," + RoleDesigner))
            return true;

        return false;
    }

    public static bool IsValidTransition(string fromStatus, string toStatus)
    {
        string from = NormalizeStatus(fromStatus);
        string to = NormalizeStatus(toStatus);
        if (from == to)
            return true;

        switch (from)
        {
            case StatusDraft:
                return to == StatusPendingApproval || to == StatusActive || to == StatusArchived;
            case StatusPendingApproval:
                return to == StatusActive || to == StatusDraft || to == StatusArchived;
            case StatusActive:
                return to == StatusPaused || to == StatusEnded || to == StatusArchived;
            case StatusPaused:
                return to == StatusActive || to == StatusEnded || to == StatusArchived;
            case StatusEnded:
                return to == StatusArchived;
            default:
                return false;
        }
    }

    public static int NormalizeCapOccurrence(int capOccurrence)
    {
        if (capOccurrence <= 0)
            return 10;
        if (capOccurrence > 200)
            return 200;
        return capOccurrence;
    }

    public static double NormalizePercent(double percent, double fallback)
    {
        if (double.IsNaN(percent) || double.IsInfinity(percent) || percent <= 0)
            return fallback;
        if (percent > 100)
            return 100;
        return percent;
    }

    public static int GetEffectiveOccurrence(int occurrence, int capOccurrence)
    {
        int cap = NormalizeCapOccurrence(capOccurrence);
        int value = occurrence <= 0 ? 1 : occurrence;
        if (value > cap)
            value = cap;
        return value;
    }

    public static double GetRateRatio(int occurrence, double stepPercent, double maxPercent, int capOccurrence)
    {
        double step = NormalizePercent(stepPercent, 5);
        double max = NormalizePercent(maxPercent, 50);
        if (max < step)
            max = step;

        int effectiveOccurrence = GetEffectiveOccurrence(occurrence, capOccurrence);
        double ratePercent = effectiveOccurrence * step;
        if (ratePercent > max)
            ratePercent = max;

        return ratePercent / 100d;
    }
}
