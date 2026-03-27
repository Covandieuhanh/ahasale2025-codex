using System.Web.SessionState;

public static class GianHangCheckoutInteraction_cl
{
    public sealed class CancelWaitActionResult
    {
        public bool Success { get; set; }
        public bool ClearPaymentSession { get; set; }
        public bool ShouldRedirect { get; set; }
        public string RedirectUrl { get; set; }
        public string DialogTitle { get; set; }
        public string DialogMessage { get; set; }
        public string DialogType { get; set; }
    }

    public static CancelWaitActionResult CancelCurrentWait(
        dbDataContext db,
        string sellerAccount,
        string currentOrderId,
        string sellerDonBanUrl)
    {
        GianHangOrderCommand_cl.CommandResult command =
            GianHangCheckoutPage_cl.CancelCurrentWait(db, sellerAccount, currentOrderId);

        if (command == null)
        {
            return new CancelWaitActionResult
            {
                Success = false,
                DialogTitle = "Thông báo",
                DialogMessage = "Có lỗi xảy ra. Vui lòng thử lại.",
                DialogType = "alert"
            };
        }

        if (!command.Success)
        {
            return new CancelWaitActionResult
            {
                Success = false,
                DialogTitle = "Thông báo",
                DialogMessage = command.Message,
                DialogType = command.AlertType
            };
        }

        return new CancelWaitActionResult
        {
            Success = true,
            ClearPaymentSession = true,
            ShouldRedirect = true,
            RedirectUrl = string.IsNullOrWhiteSpace(command.RedirectUrl) ? sellerDonBanUrl : command.RedirectUrl
        };
    }

    public static GianHangCheckoutCommand_cl.ExchangeResult ExecuteExchange(
        dbDataContext db,
        HttpSessionState session,
        string sellerAccount,
        string orderId,
        string inputPin,
        string sellerDonBanUrl,
        string choThanhToanUrl,
        string buyerDonMuaUrl,
        string loginUrl,
        string clientIp,
        int linkContextTtlMinutes)
    {
        return GianHangCheckoutPage_cl.ExecuteExchangeSafe(
            db,
            session,
            sellerAccount,
            orderId,
            inputPin,
            sellerDonBanUrl,
            choThanhToanUrl,
            buyerDonMuaUrl,
            loginUrl,
            clientIp,
            linkContextTtlMinutes);
    }
}
