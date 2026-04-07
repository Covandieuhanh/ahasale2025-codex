using System;
using System.Globalization;
using System.Text;

public static class DonHangStateMachine_cl
{
    public const string Order_DaDat = "Đã đặt";
    public const string Order_DaGiao = "Đã giao";
    public const string Order_DaNhan = "Đã nhận";
    public const string Order_DaHuy = "Đã hủy";

    public const string Exchange_ChuaTraoDoi = "Chưa Trao đổi";
    public const string Exchange_ChoTraoDoi = "Chờ Trao đổi";
    public const string Exchange_DaTraoDoi = "Đã Trao đổi";

    public static string NormalizeOrderStatus(string value)
    {
        string token = CanonicalizeStatusToken(value);
        if (token == "da-giao" || token == "dagiao") return Order_DaGiao;
        if (token == "da-nhan" || token == "danhan") return Order_DaNhan;
        if (token == "da-huy" || token == "dahuy") return Order_DaHuy;
        return Order_DaDat;
    }

    public static string NormalizeExchangeStatus(string value)
    {
        string token = CanonicalizeStatusToken(value);
        if (token == "cho-trao-doi" || token == "chotraodoi") return Exchange_ChoTraoDoi;
        if (token == "da-trao-doi" || token == "datraodoi") return Exchange_DaTraoDoi;
        return Exchange_ChuaTraoDoi;
    }

    public static string InferOrderStatusFromLegacy(string legacyStatus)
    {
        string normalized = NormalizeOrderStatus(legacyStatus);
        if (normalized == Order_DaGiao) return Order_DaGiao;
        if (normalized == Order_DaNhan) return Order_DaNhan;
        if (normalized == Order_DaHuy) return Order_DaHuy;
        return Order_DaDat;
    }

    public static string InferExchangeStatusFromLegacy(string legacyStatus)
    {
        string normalizedExchange = NormalizeExchangeStatus(legacyStatus);
        if (normalizedExchange == Exchange_ChoTraoDoi) return Exchange_ChoTraoDoi;
        if (normalizedExchange == Exchange_DaTraoDoi) return Exchange_DaTraoDoi;
        if (NormalizeOrderStatus(legacyStatus) == Order_DaNhan) return Exchange_DaTraoDoi;
        return Exchange_ChuaTraoDoi;
    }

    private static string CanonicalizeStatusToken(string value)
    {
        string raw = (value ?? string.Empty).Trim();
        if (raw == string.Empty)
            return string.Empty;

        string lowered = RemoveDiacritics(raw).ToLowerInvariant();
        lowered = lowered.Replace('_', '-').Replace(' ', '-');
        while (lowered.Contains("--"))
            lowered = lowered.Replace("--", "-");
        return lowered.Trim('-');
    }

    private static string RemoveDiacritics(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        string normalized = input.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder(normalized.Length);
        for (int i = 0; i < normalized.Length; i++)
        {
            char c = normalized[i];
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D');
    }

    public static string GetOrderStatus(DonHang_tb donHang)
    {
        if (donHang == null) return Order_DaDat;

        string raw = donHang.order_status;
        if (string.IsNullOrWhiteSpace(raw))
            return NormalizeOrderStatus(InferOrderStatusFromLegacy(donHang.trangthai));

        return NormalizeOrderStatus(raw);
    }

    public static string GetExchangeStatus(DonHang_tb donHang)
    {
        if (donHang == null) return Exchange_ChuaTraoDoi;

        string raw = donHang.exchange_status;
        if (string.IsNullOrWhiteSpace(raw))
            return NormalizeExchangeStatus(InferExchangeStatusFromLegacy(donHang.trangthai));

        return NormalizeExchangeStatus(raw);
    }

    public static void EnsureStateFields(DonHang_tb donHang)
    {
        if (donHang == null) return;

        donHang.order_status = GetOrderStatus(donHang);
        donHang.exchange_status = GetExchangeStatus(donHang);
        SyncLegacyStatus(donHang);
    }

    public static void SetOrderStatus(DonHang_tb donHang, string orderStatus)
    {
        if (donHang == null) return;

        EnsureStateFields(donHang);
        donHang.order_status = NormalizeOrderStatus(orderStatus);
        SyncLegacyStatus(donHang);
    }

    public static void SetExchangeStatus(DonHang_tb donHang, string exchangeStatus)
    {
        if (donHang == null) return;

        EnsureStateFields(donHang);
        donHang.exchange_status = NormalizeExchangeStatus(exchangeStatus);
        SyncLegacyStatus(donHang);
    }

    public static void SyncLegacyStatus(DonHang_tb donHang)
    {
        if (donHang == null) return;

        string order = GetOrderStatus(donHang);
        string exchange = GetExchangeStatus(donHang);
        donHang.trangthai = ToLegacyStatus(order, exchange, donHang.online_offline);
    }

    public static string ToLegacyStatus(string orderStatus, string exchangeStatus, bool? onlineOffline)
    {
        orderStatus = NormalizeOrderStatus(orderStatus);
        exchangeStatus = NormalizeExchangeStatus(exchangeStatus);

        if (orderStatus == Order_DaHuy) return Order_DaHuy;
        if (orderStatus == Order_DaNhan) return Order_DaNhan;
        if (exchangeStatus == Exchange_DaTraoDoi) return Exchange_DaTraoDoi;
        if (orderStatus == Order_DaGiao) return Order_DaGiao;
        if (exchangeStatus == Exchange_ChoTraoDoi) return Exchange_ChoTraoDoi;

        bool isOffline = onlineOffline.HasValue && onlineOffline.Value == false;
        return isOffline ? Exchange_ChuaTraoDoi : Order_DaDat;
    }

    public static bool IsTerminal(DonHang_tb donHang)
    {
        if (donHang == null) return true;

        string order = GetOrderStatus(donHang);
        string exchange = GetExchangeStatus(donHang);

        return order == Order_DaNhan
            || order == Order_DaHuy
            || exchange == Exchange_DaTraoDoi;
    }

    public static bool CanMarkDelivered(DonHang_tb donHang)
    {
        if (donHang == null) return false;
        if (IsTerminal(donHang)) return false;

        return GetOrderStatus(donHang) == Order_DaDat
            && GetExchangeStatus(donHang) != Exchange_ChoTraoDoi;
    }

    public static bool CanConfirmReceived(DonHang_tb donHang)
    {
        if (donHang == null) return false;
        if (IsTerminal(donHang)) return false;

        return GetOrderStatus(donHang) == Order_DaGiao;
    }

    public static bool CanCancelOrder(DonHang_tb donHang)
    {
        if (donHang == null) return false;

        string order = GetOrderStatus(donHang);
        string exchange = GetExchangeStatus(donHang);

        if (order == Order_DaGiao || order == Order_DaNhan || order == Order_DaHuy)
            return false;

        if (exchange == Exchange_DaTraoDoi)
            return false;

        return true;
    }

    public static bool CanActivateChoTraoDoi(DonHang_tb donHang)
    {
        if (donHang == null) return false;
        if (IsTerminal(donHang)) return false;

        return GetOrderStatus(donHang) == Order_DaDat
            && GetExchangeStatus(donHang) == Exchange_ChuaTraoDoi;
    }

    public static bool CanCancelChoTraoDoi(DonHang_tb donHang)
    {
        if (donHang == null) return false;
        if (IsTerminal(donHang)) return false;

        return GetExchangeStatus(donHang) == Exchange_ChoTraoDoi;
    }

    public static bool CanExecuteExchange(DonHang_tb donHang)
    {
        if (donHang == null) return false;
        if (IsTerminal(donHang)) return false;

        return GetExchangeStatus(donHang) == Exchange_ChoTraoDoi;
    }

    public static bool IsChoTraoDoi(DonHang_tb donHang)
    {
        if (donHang == null) return false;
        return GetExchangeStatus(donHang) == Exchange_ChoTraoDoi;
    }
}
