using System;
using System.Web.SessionState;

public static class WalletPaymentSession_cl
{
    public const string KeyPayer = "taikhoan_thanhtoan_home";
    public const string KeyLoaiThe = "loaithanhtoan_home";
    public const string KeyTenThe = "tenthe_thanhtoan_home";
    public const string KeyCardGuid = "key_thanhtoan_home";
    public const string KeyCardStatus = "trangthai_the_home";

    public const string KeyOrderId = "id_donhang_thanhtoan_home";
    public const string KeyVerifiedUtcTicks = "thoigian_xacnhan_link_thanhtoan_utc_ticks_home";
    public const string KeyPendingSpace = "space_thanhtoan_pending_home";
    public const string KeyPendingSeller = "seller_thanhtoan_pending_home";
    public const string KeyPendingOrderId = "id_donhang_pending_home";
    public const string KeyPendingUtcTicks = "thoigian_pending_thanhtoan_utc_ticks_home";

    public static void Clear(HttpSessionState session)
    {
        if (session == null) return;

        session[KeyPayer] = null;
        session[KeyLoaiThe] = null;
        session[KeyTenThe] = null;
        session[KeyCardGuid] = null;
        session[KeyCardStatus] = null;
        session[KeyOrderId] = null;
        session[KeyVerifiedUtcTicks] = null;
    }

    public static void ClearPending(HttpSessionState session)
    {
        if (session == null) return;

        session[KeyPendingSpace] = null;
        session[KeyPendingSeller] = null;
        session[KeyPendingOrderId] = null;
        session[KeyPendingUtcTicks] = null;
    }

    public static void Set(HttpSessionState session, string payer, int loaiThe, string tenThe, Guid keyGuid, bool cardStatus, string orderId)
    {
        if (session == null) return;

        session[KeyPayer] = payer;
        session[KeyLoaiThe] = loaiThe;
        session[KeyTenThe] = tenThe;
        session[KeyCardGuid] = keyGuid.ToString();
        session[KeyCardStatus] = cardStatus;
        session[KeyOrderId] = orderId;
        session[KeyVerifiedUtcTicks] = DateTime.UtcNow.Ticks.ToString();
    }

    public static void PreparePending(HttpSessionState session, string spaceCode, string sellerAccount, string orderId)
    {
        if (session == null) return;

        session[KeyPendingSpace] = (spaceCode ?? string.Empty).Trim().ToLowerInvariant();
        session[KeyPendingSeller] = (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
        session[KeyPendingOrderId] = (orderId ?? string.Empty).Trim();
        session[KeyPendingUtcTicks] = DateTime.UtcNow.Ticks.ToString();
    }

    public static bool TryGetCardKey(HttpSessionState session, out Guid keyGuid)
    {
        keyGuid = Guid.Empty;
        if (session == null || session[KeyCardGuid] == null) return false;

        string keyStr = (session[KeyCardGuid] ?? string.Empty).ToString().Trim();
        if (string.IsNullOrEmpty(keyStr)) return false;

        return Guid.TryParse(keyStr, out keyGuid);
    }

    public static bool IsFresh(HttpSessionState session, int ttlMinutes)
    {
        if (session == null || session[KeyVerifiedUtcTicks] == null) return false;

        long ticks;
        if (!long.TryParse((session[KeyVerifiedUtcTicks] ?? string.Empty).ToString(), out ticks))
            return false;

        DateTime verifiedUtc;
        try
        {
            verifiedUtc = new DateTime(ticks, DateTimeKind.Utc);
        }
        catch
        {
            return false;
        }

        return DateTime.UtcNow <= verifiedUtc.AddMinutes(ttlMinutes);
    }

    public static string GetOrderId(HttpSessionState session)
    {
        if (session == null || session[KeyOrderId] == null) return string.Empty;
        return (session[KeyOrderId] ?? string.Empty).ToString().Trim();
    }

    public static bool TryGetPending(HttpSessionState session, int ttlMinutes, out string spaceCode, out string sellerAccount, out string orderId)
    {
        spaceCode = string.Empty;
        sellerAccount = string.Empty;
        orderId = string.Empty;

        if (session == null || session[KeyPendingUtcTicks] == null)
            return false;

        long ticks;
        if (!long.TryParse((session[KeyPendingUtcTicks] ?? string.Empty).ToString(), out ticks))
            return false;

        DateTime preparedUtc;
        try
        {
            preparedUtc = new DateTime(ticks, DateTimeKind.Utc);
        }
        catch
        {
            return false;
        }

        if (DateTime.UtcNow > preparedUtc.AddMinutes(ttlMinutes))
            return false;

        spaceCode = (session[KeyPendingSpace] ?? string.Empty).ToString().Trim().ToLowerInvariant();
        sellerAccount = (session[KeyPendingSeller] ?? string.Empty).ToString().Trim().ToLowerInvariant();
        orderId = (session[KeyPendingOrderId] ?? string.Empty).ToString().Trim();
        return !string.IsNullOrEmpty(spaceCode) || !string.IsNullOrEmpty(sellerAccount) || !string.IsNullOrEmpty(orderId);
    }
}
