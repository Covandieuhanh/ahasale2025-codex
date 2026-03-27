using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

public static class GianHangPosWorkflow_cl
{
    public sealed class OrderListPostbackState
    {
        public bool ShouldRebind { get; set; }
        public string StatusFilter { get; set; }
    }

    public static GianHangPosPage_cl.CreateModeBindingState BuildCreateModeState(
        dbDataContext db,
        string sellerAccount,
        string tradeTypeFilter,
        string keyword,
        int resultLimit,
        IEnumerable<GianHangPosCartSession_cl.CartItem> cart)
    {
        return GianHangPosPage_cl.BuildCreateModeState(
            db,
            sellerAccount,
            tradeTypeFilter,
            keyword,
            resultLimit,
            cart);
    }

    public static GianHangPosPage_cl.OrderListBindingState BuildOrderListState(
        dbDataContext db,
        string sellerAccount,
        string statusFilter,
        string keyword)
    {
        return GianHangPosPage_cl.BuildOrderListState(
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
        return GianHangPosPage_cl.HandlePassiveCreatePostback(
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
        return GianHangPosAction_cl.TryAddProductToCart(
            db,
            session,
            sessionKey,
            sellerAccount,
            productId,
            out warningMessage);
    }

    public static OrderListPostbackState HandleOrderListPostback(
        string currentStatus,
        string postedStatus)
    {
        string currentFilter = GianHangOrderCore_cl.NormalizeOrderStatusFilter(currentStatus);
        string nextFilter = GianHangOrderCore_cl.NormalizeOrderStatusFilter(postedStatus);

        return new OrderListPostbackState
        {
            ShouldRebind = !string.Equals(currentFilter, nextFilter, System.StringComparison.Ordinal),
            StatusFilter = string.Equals(currentFilter, nextFilter, System.StringComparison.Ordinal) ? currentFilter : nextFilter
        };
    }

    public static GianHangPosPage_cl.CartBindingState BuildCartStateFromSession(
        HttpSessionState session,
        string sessionKey)
    {
        return GianHangPosPage_cl.BuildCartState(
            GianHangPosCartSession_cl.GetCart(session, sessionKey));
    }

    public static GianHangPosPage_cl.CartBindingState ClearCartAndBuildState(
        HttpSessionState session,
        string sessionKey)
    {
        GianHangPosCartSession_cl.Clear(session, sessionKey);
        return BuildCartStateFromSession(session, sessionKey);
    }

    public static void ApplyCartCommand(
        HttpSessionState session,
        string sessionKey,
        string productId,
        string commandName,
        string qtyText,
        string discountText)
        {
        GianHangPosAction_cl.ApplyCartCommand(
            session,
            sessionKey,
            productId,
            commandName,
            qtyText,
            discountText);
    }

    public static GianHangPosPage_cl.CartBindingState ApplyCartCommandAndBuildState(
        HttpSessionState session,
        string sessionKey,
        string productId,
        string commandName,
        string qtyText,
        string discountText)
    {
        ApplyCartCommand(session, sessionKey, productId, commandName, qtyText, discountText);
        return BuildCartStateFromSession(session, sessionKey);
    }

    public static GianHangOrderCommand_cl.CommandResult CreateOfflineOrder(
        dbDataContext db,
        HttpSessionState session,
        string sessionKey,
        string sellerAccount,
        string choThanhToanUrl)
    {
        return GianHangPosAction_cl.CreateOfflineOrder(
            db,
            session,
            sessionKey,
            sellerAccount,
            choThanhToanUrl);
    }

    public static GianHangOrderCommand_cl.CommandResult ExecuteOrderListCommand(
        dbDataContext db,
        string sellerAccount,
        string commandName,
        string orderId)
    {
        return GianHangOrderListAction_cl.Execute(
            db,
            sellerAccount,
            commandName,
            orderId);
    }

    public static string BuildCreateModeEntryUrl(string productId, string returnUrl)
    {
        string url = GianHangRoutes_cl.BuildDonBanUrl() + "?taodon=1";
        string idsp = (productId ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(idsp))
            url += "&idsp=" + HttpUtility.UrlEncode(idsp);
        url += "&return_url=" + HttpUtility.UrlEncode(returnUrl ?? GianHangRoutes_cl.BuildDonBanUrl());
        return url;
    }
}
