using System.Collections.Generic;
using System.Web.SessionState;

public static class GianHangPosAction_cl
{
    public static bool TryAddProductToCart(
        dbDataContext db,
        HttpSessionState session,
        string sessionKey,
        string sellerAccount,
        string productId,
        out string warningMessage)
    {
        warningMessage = string.Empty;
        if (db == null || session == null || string.IsNullOrWhiteSpace(sessionKey))
            return false;

        List<GianHangPosCartSession_cl.CartItem> cart = GianHangPosCartSession_cl.GetCart(session, sessionKey);
        if (!GianHangPosPage_cl.TryAddProductToCart(db, sellerAccount, productId, cart, out warningMessage))
            return false;

        GianHangPosCartSession_cl.SaveCart(session, sessionKey, cart);
        return true;
    }

    public static void ApplyCartCommand(
        HttpSessionState session,
        string sessionKey,
        string productId,
        string commandName,
        string qtyText,
        string discountText)
    {
        if (session == null || string.IsNullOrWhiteSpace(sessionKey))
            return;

        List<GianHangPosCartSession_cl.CartItem> cart = GianHangPosCartSession_cl.GetCart(session, sessionKey);
        GianHangPosCartSession_cl.ApplyItemCommand(cart, productId, commandName, qtyText, discountText);
        GianHangPosCartSession_cl.SaveCart(session, sessionKey, cart);
    }

    public static GianHangOrderCommand_cl.CommandResult CreateOfflineOrder(
        dbDataContext db,
        HttpSessionState session,
        string sessionKey,
        string sellerAccount,
        string choThanhToanUrl)
    {
        if (session == null || string.IsNullOrWhiteSpace(sessionKey))
        {
            return new GianHangOrderCommand_cl.CommandResult
            {
                Success = false,
                Message = "Phiên giỏ hàng không hợp lệ.",
                AlertType = "warning"
            };
        }

        List<GianHangPosCartSession_cl.CartItem> cart = GianHangPosCartSession_cl.GetCart(session, sessionKey);
        List<GianHangOrderCommand_cl.OfflineCartLine> lines = GianHangPosCartSession_cl.BuildOfflineLines(cart);
        GianHangOrderCommand_cl.CommandResult result =
            GianHangOrderCommand_cl.CreateOfflineOrder(db, sellerAccount, lines, choThanhToanUrl);

        if (result != null && result.Success)
            GianHangPosCartSession_cl.Clear(session, sessionKey);

        return result;
    }
}
