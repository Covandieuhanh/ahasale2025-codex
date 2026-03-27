using System;

public static class GianHangOrderListAction_cl
{
    public static GianHangOrderCommand_cl.CommandResult Execute(
        dbDataContext db,
        string sellerAccount,
        string commandName,
        string orderId)
    {
        string command = (commandName ?? string.Empty).Trim().ToLowerInvariant();
        string safeOrderId = (orderId ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(sellerAccount) || string.IsNullOrWhiteSpace(safeOrderId))
        {
            return new GianHangOrderCommand_cl.CommandResult
            {
                Success = false,
                Message = "Thiếu thông tin đơn hàng để thao tác.",
                AlertType = "warning"
            };
        }

        switch (command)
        {
            case "openwait":
                return GianHangOrderCommand_cl.ActivateWaitingExchange(
                    db,
                    sellerAccount,
                    safeOrderId,
                    GianHangCheckoutPortal_cl.ChoThanhToanUrl());

            case "cancelwait":
                return GianHangOrderCommand_cl.CancelWaitingExchange(db, sellerAccount, safeOrderId);

            case "deliver":
                return GianHangOrderCommand_cl.MarkDelivered(db, sellerAccount, safeOrderId);

            case "cancelorder":
                return GianHangOrderCommand_cl.CancelOrder(db, sellerAccount, safeOrderId);

            default:
                return new GianHangOrderCommand_cl.CommandResult
                {
                    Success = false,
                    Message = "Thao tác không hợp lệ.",
                    AlertType = "warning"
                };
        }
    }
}
