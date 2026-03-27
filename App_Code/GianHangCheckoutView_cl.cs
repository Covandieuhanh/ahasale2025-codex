using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;

public static class GianHangCheckoutView_cl
{
    private const decimal VND_PER_QUYEN = 1000m;

    public sealed class CheckoutPageState
    {
        public bool ShouldRedirectToSellerList { get; set; }
        public string OrderId { get; set; }
        public string OrderItemCountText { get; set; }
        public List<GianHangOrderDetail_cl.OrderDetailRow> OrderDetails { get; set; }
        public string WaitAmountText { get; set; }
        public string ConfirmAmountText { get; set; }
        public bool ShowWaitingPlaceholder { get; set; }
        public bool ShowPaymentPlaceholder { get; set; }
        public bool PinEnabled { get; set; }
        public bool ConfirmEnabled { get; set; }
        public bool PollEnabled { get; set; }
        public string CardNoticeHtml { get; set; }
        public string SellerDisplayName { get; set; }
        public string BuyerDisplayName { get; set; }
    }

    public static CheckoutPageState BuildPageState(
        dbDataContext db,
        HttpSessionState session,
        string sellerAccount,
        int linkContextTtlMinutes)
    {
        sellerAccount = NormalizeText(sellerAccount).ToLowerInvariant();
        if (db == null || session == null || sellerAccount == string.Empty)
            return RedirectState();

        GianHangOrderRuntime_cl.OrderRuntime runtime = GianHangOrderRuntime_cl.ResolveLatestWaitingExchange(db, sellerAccount, false);
        if (runtime == null || (runtime.Order == null && runtime.Invoice == null))
            return RedirectState();

        string orderId = runtime == null ? string.Empty : runtime.OrderId;
        List<GianHangOrderDetail_cl.OrderDetailRow> detailRows =
            GianHangOrderDetail_cl.BuildOrderDetailRows(db, sellerAccount, runtime);

        decimal totalRights = GianHangCheckoutCore_cl.ConvertVndToRights(
            GianHangOrderRuntime_cl.ResolveTotalAmount(runtime));

        CheckoutPageState state = new CheckoutPageState
        {
            OrderId = orderId,
            OrderDetails = detailRows,
            OrderItemCountText = detailRows.Count.ToString("#,##0"),
            SellerDisplayName = GianHangOrderRuntime_cl.ResolveSellerDisplayName(db, runtime, sellerAccount, "Gian hàng")
        };

        GianHangCheckoutCore_cl.PaymentContext paymentContext =
            GianHangCheckoutCore_cl.LoadPaymentContext(
                db,
                session,
                sellerAccount,
                true,
                linkContextTtlMinutes,
                runtime);
        if (!string.IsNullOrEmpty(paymentContext.ErrorCode))
        {
            state.ShowWaitingPlaceholder = true;
            state.ShowPaymentPlaceholder = false;
            state.WaitAmountText = FormatQuyenWithVnd(totalRights);
            state.PinEnabled = true;
            state.ConfirmEnabled = true;
            state.PollEnabled = true;
            if (paymentContext.ErrorCode != "missing_key")
                state.CardNoticeHtml = GianHangCheckoutCore_cl.BuildContextErrorMessage(paymentContext.ErrorCode);
            return state;
        }

        state.ShowWaitingPlaceholder = false;
        state.ShowPaymentPlaceholder = true;
        state.PollEnabled = false;

        taikhoan_tb payerAccount = paymentContext.PayerAccount;
        The_PhatHanh_tb card = paymentContext.Card;
        int cardType = paymentContext.CardType;
        bool cardActive = card != null && card.TrangThai;
        string cardName = card == null ? string.Empty : NormalizeText(card.TenThe);

        state.BuyerDisplayName = BuildBuyerDisplayName(payerAccount, cardName);

        if (!cardActive)
        {
            state.ConfirmAmountText = FormatQuyenWithVnd(totalRights);
            state.PinEnabled = false;
            state.ConfirmEnabled = false;
            state.CardNoticeHtml = "Thẻ này đã bị khóa. Vui lòng liên hệ quản trị hoặc sử dụng thẻ khác.";
            return state;
        }

        if (cardType != 1 && cardType != 2)
        {
            state.ConfirmAmountText = FormatQuyenWithVnd(totalRights);
            state.PinEnabled = false;
            state.ConfirmEnabled = false;
            state.CardNoticeHtml = "Hiện tại chưa Trao đổi được bằng loại thẻ này.";
            return state;
        }

        GianHangOrderDetail_cl.DiscountSummary discountSummary =
            GianHangOrderDetail_cl.BuildDiscountSummary(db, sellerAccount, runtime);
        decimal discountRights = discountSummary.DiscountRights;
        bool hasDiscount = discountSummary.HasDiscount;

        if (cardType == 1)
            return BuildDiscountCardState(state, totalRights, discountRights, hasDiscount);

        return BuildConsumerCardState(state, payerAccount, totalRights, discountRights, hasDiscount);
    }

    private static CheckoutPageState RedirectState()
    {
        return new CheckoutPageState
        {
            ShouldRedirectToSellerList = true,
            OrderDetails = new List<GianHangOrderDetail_cl.OrderDetailRow>(),
            OrderItemCountText = "0"
        };
    }

    private static CheckoutPageState BuildDiscountCardState(
        CheckoutPageState state,
        decimal totalRights,
        decimal discountRights,
        bool hasDiscount)
    {
        if (state == null)
            state = new CheckoutPageState();

        if (!hasDiscount)
        {
            state.ConfirmAmountText = FormatQuyenWithVnd(totalRights);
            state.PinEnabled = false;
            state.ConfirmEnabled = false;
            state.CardNoticeHtml = "Thẻ ưu đãi chỉ áp dụng cho đơn có ưu đãi. Đơn này không có ưu đãi nên không thể Trao đổi bằng thẻ ưu đãi.";
            return state;
        }

        decimal consumerPart = totalRights - discountRights;
        if (consumerPart < 0m)
            consumerPart = 0m;

        state.ConfirmAmountText = FormatQuyenWithVnd(discountRights);
        state.PinEnabled = true;
        state.ConfirmEnabled = true;
        state.CardNoticeHtml =
            "Trao đổi bằng <b>Thẻ ưu đãi</b>: <b>" + discountRights.ToString("#,##0.##") + " Quyền</b>."
            + "<br/>Phần còn lại: <b>" + consumerPart.ToString("#,##0.##") + " Quyền</b> (trao đổi tiền mặt theo quy đổi).";
        return state;
    }

    private static CheckoutPageState BuildConsumerCardState(
        CheckoutPageState state,
        taikhoan_tb payerAccount,
        decimal totalRights,
        decimal discountRights,
        bool hasDiscount)
    {
        if (state == null)
            state = new CheckoutPageState();

        state.ConfirmAmountText = FormatQuyenWithVnd(totalRights);
        state.PinEnabled = true;
        state.ConfirmEnabled = true;

        decimal discountWallet = payerAccount == null ? 0m : (payerAccount.Vi1That_Evocher_30PhanTram ?? 0m);
        string html = "Tổng Trao đổi: <b>" + totalRights.ToString("#,##0.##") + " Quyền</b>.";

        if (hasDiscount)
        {
            decimal consumerPart = totalRights - discountRights;
            if (consumerPart < 0m)
                consumerPart = 0m;

            html += "<br/>Ưu đãi: <b>" + discountRights.ToString("#,##0.##") + " Quyền</b>."
                  + "<br/>Còn lại: <b>" + consumerPart.ToString("#,##0.##") + " Quyền</b>.";

            if (discountWallet >= discountRights)
            {
                html += "<br/><b>Sẽ trừ:</b> "
                      + "<br/><b>-" + discountRights.ToString("#,##0.##") + " Quyền ưu đãi</b>"
                      + "<br/><b>-" + consumerPart.ToString("#,##0.##") + " Quyền tiêu dùng</b>";
            }
            else
            {
                html += "<br/><b>Hồ sơ ưu đãi không đủ</b> → sẽ trừ 100% vào <b>Hồ sơ tiêu dùng</b>: <b>-" + totalRights.ToString("#,##0.##") + " Quyền tiêu dùng</b>.";
            }
        }
        else
        {
            html += "<br/><b>Sẽ trừ:</b> <b>-" + totalRights.ToString("#,##0.##") + " Quyền tiêu dùng</b>.";
        }

        state.CardNoticeHtml = html;
        return state;
    }

    private static string BuildBuyerDisplayName(taikhoan_tb payerAccount, string cardName)
    {
        string buyerName = payerAccount == null ? string.Empty : NormalizeText(payerAccount.hoten);
        if (buyerName == string.Empty && payerAccount != null)
            buyerName = NormalizeText(payerAccount.taikhoan);

        if (buyerName == string.Empty)
            buyerName = "Khách hàng";

        if (cardName == string.Empty)
            return buyerName;

        return buyerName + " (Đang dùng thẻ: " + cardName + ")";
    }

    private static string NormalizeText(string value)
    {
        return (value ?? string.Empty).Trim();
    }

    private static string FormatQuyenWithVnd(decimal rights)
    {
        decimal vnd = rights * VND_PER_QUYEN;
        return string.Format("{0:#,##0.##} Quyền (~{1:#,##0}đ)", rights, vnd);
    }
}
