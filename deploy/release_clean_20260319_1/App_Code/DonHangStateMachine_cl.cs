using System;

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
        if (value == Order_DaGiao) return Order_DaGiao;
        if (value == Order_DaNhan) return Order_DaNhan;
        if (value == Order_DaHuy) return Order_DaHuy;
        return Order_DaDat;
    }

    public static string NormalizeExchangeStatus(string value)
    {
        if (value == Exchange_ChoTraoDoi) return Exchange_ChoTraoDoi;
        if (value == Exchange_DaTraoDoi) return Exchange_DaTraoDoi;
        return Exchange_ChuaTraoDoi;
    }

    public static string InferOrderStatusFromLegacy(string legacyStatus)
    {
        if (legacyStatus == Order_DaGiao) return Order_DaGiao;
        if (legacyStatus == Order_DaNhan) return Order_DaNhan;
        if (legacyStatus == Order_DaHuy) return Order_DaHuy;
        return Order_DaDat;
    }

    public static string InferExchangeStatusFromLegacy(string legacyStatus)
    {
        if (legacyStatus == Exchange_ChoTraoDoi) return Exchange_ChoTraoDoi;
        if (legacyStatus == Exchange_DaTraoDoi) return Exchange_DaTraoDoi;
        if (legacyStatus == Order_DaNhan) return Exchange_DaTraoDoi;
        return Exchange_ChuaTraoDoi;
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
