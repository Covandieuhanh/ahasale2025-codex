using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;

public static class GianHangCheckoutCore_cl
{
    public sealed class PaymentContext
    {
        public GianHangOrderRuntime_cl.OrderRuntime WaitingRuntime { get; set; }
        public string WaitingOrderId { get; set; }
        public taikhoan_tb PayerAccount { get; set; }
        public The_PhatHanh_tb Card { get; set; }
        public int CardType { get; set; }
        public string ErrorCode { get; set; }
    }

    public static PaymentContext LoadPaymentContext(
        dbDataContext db,
        HttpSessionState session,
        string sellerAccount,
        bool requireFreshLink,
        int linkContextTtlMinutes,
        GianHangOrderRuntime_cl.OrderRuntime prefetchedRuntime = null)
    {
        PaymentContext context = new PaymentContext
        {
            CardType = 0,
            ErrorCode = string.Empty
        };

        GianHangOrderRuntime_cl.OrderRuntime runtime = prefetchedRuntime;
        if (runtime == null || (runtime.Order == null && runtime.Invoice == null))
            runtime = GianHangOrderRuntime_cl.ResolveLatestWaitingExchange(db, sellerAccount, false);

        context.WaitingRuntime = runtime;
        context.WaitingOrderId = runtime == null ? string.Empty : (runtime.OrderId ?? string.Empty).Trim();
        if (runtime == null || string.IsNullOrWhiteSpace(context.WaitingOrderId))
        {
            context.ErrorCode = "missing_order";
            return context;
        }

        Guid keyGuid;
        if (!WalletPaymentSession_cl.TryGetCardKey(session, out keyGuid))
        {
            context.ErrorCode = "missing_key";
            return context;
        }

        if (requireFreshLink && !WalletPaymentSession_cl.IsFresh(session, linkContextTtlMinutes))
        {
            context.ErrorCode = "expired_key";
            return context;
        }

        context.Card = db.The_PhatHanh_tbs.FirstOrDefault(p => p.idGuide == keyGuid);
        if (context.Card == null)
        {
            context.ErrorCode = "invalid_key";
            return context;
        }

        string payerCode = (context.Card.taikhoan ?? "").Trim();
        context.PayerAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == payerCode);
        if (context.PayerAccount == null)
        {
            context.ErrorCode = "missing_payer";
            return context;
        }

        context.CardType = context.Card.LoaiThe;

        string orderIdInSession = WalletPaymentSession_cl.GetOrderId(session);
        if (!string.IsNullOrEmpty(orderIdInSession) && !string.Equals(orderIdInSession, context.WaitingOrderId, StringComparison.Ordinal))
        {
            context.ErrorCode = "order_mismatch";
            return context;
        }

        WalletPaymentSession_cl.Set(
            session,
            context.PayerAccount.taikhoan,
            context.CardType,
            context.Card.TenThe ?? string.Empty,
            keyGuid,
            context.Card.TrangThai,
            context.WaitingOrderId);

        return context;
    }

    public static string BuildContextErrorMessage(string contextError)
    {
        if (contextError == "expired_key")
            return "Phiên trao đổi đã hết hạn. Vui lòng quét link trao đổi lại.";
        if (contextError == "invalid_key")
            return "Link trao đổi không còn hợp lệ. Vui lòng quét lại thẻ.";
        if (contextError == "missing_payer")
            return "Tài khoản trao đổi không tồn tại. Vui lòng kiểm tra lại thẻ.";
        if (contextError == "order_mismatch")
            return "Đơn chờ trao đổi đã thay đổi. Vui lòng quét lại link trao đổi.";
        return "Vui lòng quét link trao đổi để tiếp tục.";
    }

    public static decimal ConvertVndToRights(decimal vnd)
    {
        if (vnd <= 0m) return 0m;
        decimal rights = vnd / 1000m;
        return Math.Ceiling(rights * 100m) / 100m;
    }
}
