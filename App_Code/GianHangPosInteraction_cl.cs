using System.Web.SessionState;

public static class GianHangPosInteraction_cl
{
    public sealed class CreateOrderActionResult
    {
        public GianHangOrderCommand_cl.CommandResult Command { get; set; }
        public GianHangPosPage_cl.CartBindingState Cart { get; set; }
    }

    public static GianHangPosPage_cl.CreateModeBindingState BuildCreateModeState(
        dbDataContext db,
        string sellerAccount,
        string tradeTypeFilter,
        string keyword,
        int resultLimit,
        HttpSessionState session,
        string sessionKey)
    {
        return GianHangPosWorkflow_cl.BuildCreateModeState(
            db,
            sellerAccount,
            tradeTypeFilter,
            keyword,
            resultLimit,
            GianHangPosCartSession_cl.GetCart(session, sessionKey));
    }

    public static GianHangPosPage_cl.OrderListBindingState BuildOrderListState(
        dbDataContext db,
        string sellerAccount,
        string statusFilter,
        string keyword)
    {
        return GianHangPosWorkflow_cl.BuildOrderListState(
            db,
            sellerAccount,
            statusFilter,
            keyword);
    }

    public static GianHangPosPage_cl.PassivePostbackState HandleCreatePostback(
        string currentTradeType,
        string postedTradeType,
        bool searchMoreTriggered,
        int currentLimit)
    {
        return GianHangPosWorkflow_cl.HandleCreatePostback(
            currentTradeType,
            postedTradeType,
            searchMoreTriggered,
            currentLimit);
    }

    public static bool TryAddProductToCart(
        dbDataContext db,
        HttpSessionState session,
        string sessionKey,
        string sellerAccount,
        string productId,
        out string warningMessage)
    {
        return GianHangPosWorkflow_cl.TryAddProductToCart(
            db,
            session,
            sessionKey,
            sellerAccount,
            productId,
            out warningMessage);
    }

    public static GianHangPosWorkflow_cl.OrderListPostbackState HandleOrderListPostback(
        string currentStatus,
        string postedStatus)
    {
        return GianHangPosWorkflow_cl.HandleOrderListPostback(currentStatus, postedStatus);
    }

    public static GianHangPosPage_cl.CartBindingState HandleCartCommand(
        HttpSessionState session,
        string sessionKey,
        string productId,
        string commandName,
        string qtyText,
        string discountText)
    {
        return GianHangPosWorkflow_cl.ApplyCartCommandAndBuildState(
            session,
            sessionKey,
            productId,
            commandName,
            qtyText,
            discountText);
    }

    public static GianHangPosPage_cl.CartBindingState ClearCart(
        HttpSessionState session,
        string sessionKey)
    {
        return GianHangPosWorkflow_cl.ClearCartAndBuildState(session, sessionKey);
    }

    public static CreateOrderActionResult CreateOfflineOrder(
        dbDataContext db,
        HttpSessionState session,
        string sessionKey,
        string sellerAccount,
        string choThanhToanUrl)
    {
        GianHangOrderCommand_cl.CommandResult command =
            GianHangPosWorkflow_cl.CreateOfflineOrder(
                db,
                session,
                sessionKey,
                sellerAccount,
                choThanhToanUrl);

        return new CreateOrderActionResult
        {
            Command = command,
            Cart = GianHangPosWorkflow_cl.BuildCartStateFromSession(session, sessionKey)
        };
    }
}
