using System;

public static class GianHangCheckoutAction_cl
{
    public sealed class CancelWaitQueryResult
    {
        public bool Handled { get; set; }
        public bool ShouldRedirect { get; set; }
        public string RedirectUrl { get; set; }
        public bool ClearPaymentSession { get; set; }
        public bool UseOnloadDialog { get; set; }
        public string DialogTitle { get; set; }
        public string DialogMessage { get; set; }
        public string DialogType { get; set; }
    }

    public static CancelWaitQueryResult HandleCancelWaitQuery(
        dbDataContext db,
        string rawCancelWait,
        string sellerAccount,
        string rawOrderId,
        string loginUrl,
        string choThanhToanUrl)
    {
        if (!string.Equals((rawCancelWait ?? string.Empty).Trim(), "1", StringComparison.Ordinal))
            return new CancelWaitQueryResult { Handled = false };

        if (string.IsNullOrWhiteSpace(sellerAccount))
        {
            return new CancelWaitQueryResult
            {
                Handled = true,
                ShouldRedirect = true,
                RedirectUrl = loginUrl ?? string.Empty
            };
        }

        string orderId = (rawOrderId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(orderId))
        {
            return BuildOnloadRedirect(
                choThanhToanUrl,
                "Thiếu mã đơn hợp lệ để hủy chờ Trao đổi.");
        }

        GianHangOrderRuntime_cl.OrderRuntime runtime =
            GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, sellerAccount, orderId);
        if (runtime == null || string.IsNullOrWhiteSpace((runtime.OrderId ?? string.Empty).Trim()))
        {
            return BuildOnloadRedirect(
                choThanhToanUrl,
                "Không tìm thấy đơn hàng cần hủy hoặc bạn không có quyền thao tác đơn này.");
        }

        GianHangOrderCommand_cl.CommandResult result =
            GianHangOrderCommand_cl.CancelWaitingExchange(db, sellerAccount, runtime.OrderId);
        if (!result.Success)
        {
            return BuildOnloadRedirect(
                choThanhToanUrl,
                "Đơn hàng không còn ở trạng thái chờ Trao đổi.");
        }

        return new CancelWaitQueryResult
        {
            Handled = true,
            ShouldRedirect = true,
            RedirectUrl = GianHangCheckoutPortal_cl.SellerDonBanUrl(),
            ClearPaymentSession = true
        };
    }

    public static GianHangOrderCommand_cl.CommandResult CancelCurrentWait(
        dbDataContext db,
        string sellerAccount,
        string currentOrderId)
    {
        string orderId = (currentOrderId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(orderId))
            return BuildAlertResult("Không tìm thấy đơn hàng chờ Trao đổi để hủy.");

        GianHangOrderRuntime_cl.OrderRuntime runtime =
            GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, sellerAccount, orderId);
        if (runtime == null || string.IsNullOrWhiteSpace((runtime.OrderId ?? string.Empty).Trim()))
            return BuildAlertResult("Không tìm thấy đơn hàng chờ Trao đổi để hủy.");

        GianHangOrderCommand_cl.CommandResult result =
            GianHangOrderCommand_cl.CancelWaitingExchange(db, sellerAccount, runtime.OrderId);
        if (result.Success && string.IsNullOrWhiteSpace(result.RedirectUrl))
        {
            result.ShouldRedirect = true;
            result.RedirectUrl = GianHangCheckoutPortal_cl.SellerDonBanUrl();
        }

        return result;
    }

    private static CancelWaitQueryResult BuildOnloadRedirect(string redirectUrl, string message)
    {
        return new CancelWaitQueryResult
        {
            Handled = true,
            ShouldRedirect = true,
            RedirectUrl = redirectUrl ?? string.Empty,
            UseOnloadDialog = true,
            DialogTitle = "Thông báo",
            DialogMessage = message ?? string.Empty,
            DialogType = "alert"
        };
    }

    private static GianHangOrderCommand_cl.CommandResult BuildAlertResult(string message)
    {
        return new GianHangOrderCommand_cl.CommandResult
        {
            Success = false,
            Message = message ?? string.Empty,
            AlertType = "alert"
        };
    }
}
