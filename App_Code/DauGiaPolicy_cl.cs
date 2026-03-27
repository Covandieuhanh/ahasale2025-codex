using System;

public static class DauGiaPolicy_cl
{
    public const string StatusDraft = "draft";
    public const string StatusPendingApproval = "pending_approval";
    public const string StatusScheduled = "scheduled";
    public const string StatusLive = "live";
    public const string StatusReserved = "reserved";
    public const string StatusBuyerConfirmed = "buyer_confirmed";
    public const string StatusSellerConfirmed = "seller_confirmed";
    public const string StatusCompleted = "completed";
    public const string StatusExpired = "expired";
    public const string StatusCancelled = "cancelled";
    public const string StatusSettlementFailed = "settlement_failed";

    public const string SettlementManualFulfillment = "manual_fulfillment";
    public const string SettlementSystemTransfer = "system_transfer";

    public const string SourceManualAsset = "manual_asset";
    public const string SourceShopPost = "shop_post";
    public const string SourceHomeQuyenUuDai = "home_quyen_uu_dai";
    public const string SourceHomeQuyenTieuDung = "home_quyen_tieu_dung";

    public const string AdminPermissionCode = "daugia_admin";

    public static string NormalizeStatus(string status)
    {
        string normalized = (status ?? "").Trim().ToLowerInvariant();
        if (normalized == "")
            return StatusDraft;

        switch (normalized)
        {
            case StatusDraft:
            case StatusPendingApproval:
            case StatusScheduled:
            case StatusLive:
            case StatusReserved:
            case StatusBuyerConfirmed:
            case StatusSellerConfirmed:
            case StatusCompleted:
            case StatusExpired:
            case StatusCancelled:
            case StatusSettlementFailed:
                return normalized;
            default:
                return StatusDraft;
        }
    }

    public static string NormalizeSettlementMode(string settlementMode)
    {
        string normalized = (settlementMode ?? "").Trim().ToLowerInvariant();
        if (normalized == SettlementSystemTransfer)
            return SettlementSystemTransfer;
        return SettlementManualFulfillment;
    }

    public static string NormalizeSourceType(string sourceType)
    {
        string normalized = (sourceType ?? "").Trim().ToLowerInvariant();
        switch (normalized)
        {
            case SourceManualAsset:
            case SourceShopPost:
            case SourceHomeQuyenUuDai:
            case SourceHomeQuyenTieuDung:
                return normalized;
            default:
                return SourceManualAsset;
        }
    }

    public static bool RequiresSourceId(string sourceType)
    {
        return NormalizeSourceType(sourceType) != SourceManualAsset;
    }

    public static bool IsTerminalStatus(string status)
    {
        string normalized = NormalizeStatus(status);
        return normalized == StatusCompleted
            || normalized == StatusExpired
            || normalized == StatusCancelled
            || normalized == StatusSettlementFailed;
    }

    public static bool CanAccessPublic(string account)
    {
        // Route public cho phép truy cập không đăng nhập, nên luôn true.
        return true;
    }

    public static bool CanCreateAuction(string account)
    {
        string normalized = (account ?? "").Trim().ToLowerInvariant();
        return normalized != "";
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

    public static bool IsValidTransition(string fromStatus, string toStatus)
    {
        string from = NormalizeStatus(fromStatus);
        string to = NormalizeStatus(toStatus);
        if (from == to)
            return true;

        switch (from)
        {
            case StatusDraft:
                return to == StatusPendingApproval || to == StatusCancelled;
            case StatusPendingApproval:
                return to == StatusScheduled || to == StatusLive || to == StatusCancelled;
            case StatusScheduled:
                return to == StatusLive || to == StatusCancelled || to == StatusExpired;
            case StatusLive:
                return to == StatusReserved || to == StatusExpired || to == StatusCancelled;
            case StatusReserved:
                return to == StatusBuyerConfirmed || to == StatusSettlementFailed || to == StatusCancelled;
            case StatusBuyerConfirmed:
                return to == StatusSellerConfirmed || to == StatusSettlementFailed || to == StatusCancelled;
            case StatusSellerConfirmed:
                return to == StatusCompleted || to == StatusSettlementFailed;
            case StatusSettlementFailed:
                return to == StatusCancelled || to == StatusExpired;
            default:
                return false;
        }
    }
}
