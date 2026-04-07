using System;
using System.Web.SessionState;

public static class GianHangCheckoutPage_cl
{
    public sealed class LoadResult
    {
        public bool StopRequest { get; set; }
        public bool ClearPaymentSession { get; set; }
        public bool UseOnloadDialog { get; set; }
        public string DialogTitle { get; set; }
        public string DialogMessage { get; set; }
        public string DialogType { get; set; }
        public bool ShouldRedirect { get; set; }
        public string RedirectUrl { get; set; }
        public GianHangCheckoutView_cl.CheckoutPageState PageState { get; set; }
    }

    public static LoadResult BuildLoadResult(
        dbDataContext db,
        HttpSessionState session,
        string sellerAccount,
        string rawCancelWait,
        string rawOrderId,
        string loginUrl,
        string choThanhToanUrl,
        string sellerDonBanUrl,
        int linkContextTtlMinutes)
    {
        if (db == null)
        {
            return new LoadResult
            {
                StopRequest = true,
                ShouldRedirect = true,
                RedirectUrl = sellerDonBanUrl ?? string.Empty
            };
        }

        GianHangCheckoutAction_cl.CancelWaitQueryResult cancelResult =
            GianHangCheckoutAction_cl.HandleCancelWaitQuery(
                db,
                rawCancelWait,
                sellerAccount,
                rawOrderId,
                loginUrl,
                choThanhToanUrl);
        if (cancelResult != null && cancelResult.Handled)
        {
            return new LoadResult
            {
                StopRequest = true,
                ClearPaymentSession = cancelResult.ClearPaymentSession,
                UseOnloadDialog = cancelResult.UseOnloadDialog,
                DialogTitle = cancelResult.DialogTitle,
                DialogMessage = cancelResult.DialogMessage,
                DialogType = cancelResult.DialogType,
                ShouldRedirect = cancelResult.ShouldRedirect,
                RedirectUrl = cancelResult.RedirectUrl
            };
        }

        GianHangCheckoutView_cl.CheckoutPageState state =
            GianHangCheckoutView_cl.BuildPageState(
                db,
                session,
                sellerAccount,
                linkContextTtlMinutes,
                rawOrderId);
        if (state == null || state.ShouldRedirectToSellerList)
        {
            return new LoadResult
            {
                StopRequest = true,
                ShouldRedirect = true,
                RedirectUrl = sellerDonBanUrl ?? string.Empty
            };
        }

        return new LoadResult
        {
            PageState = state
        };
    }

    public static GianHangOrderCommand_cl.CommandResult CancelCurrentWait(
        dbDataContext db,
        string sellerAccount,
        string currentOrderId)
    {
        return GianHangCheckoutAction_cl.CancelCurrentWait(db, sellerAccount, currentOrderId);
    }

    public static GianHangCheckoutCommand_cl.ExchangeResult ExecuteExchangeSafe(
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
        try
        {
            return GianHangCheckoutCommand_cl.ExecuteExchange(
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
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, sellerAccount, ex.StackTrace);
            return new GianHangCheckoutCommand_cl.ExchangeResult
            {
                Success = false,
                DialogTitle = "Thông báo",
                DialogMessage = "Có lỗi xảy ra trong quá trình xử lý trao đổi. Vui lòng thử lại.",
                DialogType = "alert"
            };
        }
    }
}
