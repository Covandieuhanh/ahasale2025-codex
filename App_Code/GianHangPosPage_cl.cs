using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangPosPage_cl
{
    public const int ResultLimitStep = 20;

    public sealed class PassivePostbackState
    {
        public bool ShouldRebind { get; set; }
        public string TradeTypeFilter { get; set; }
        public int ResultLimit { get; set; }
    }

    public sealed class CartBindingState
    {
        public List<GianHangPosCartSession_cl.CartItem> Items { get; set; }
        public bool IsEmpty { get; set; }
        public string TotalVndText { get; set; }
        public string TotalRightsText { get; set; }
        public string TotalCountText { get; set; }
    }

    public sealed class CreateModeBindingState
    {
        public GianHangPosView_cl.SearchPanelState SearchPanel { get; set; }
        public CartBindingState Cart { get; set; }
    }

    public sealed class OrderListBindingState
    {
        public GianHangPosView_cl.OrderListPanelState Panel { get; set; }
        public GianHangOrderCore_cl.SellerOrderSummary Summary { get; set; }
    }

    public static int NormalizeResultLimit(int rawValue)
    {
        return GianHangPosCartSession_cl.ClampInt(rawValue <= 0 ? ResultLimitStep : rawValue, ResultLimitStep, 500);
    }

    public static int IncreaseResultLimit(int currentLimit)
    {
        return NormalizeResultLimit(NormalizeResultLimit(currentLimit) + ResultLimitStep);
    }

    public static PassivePostbackState HandlePassiveCreatePostback(
        string currentTradeType,
        string postedTradeType,
        bool searchMoreTriggered,
        int currentLimit)
    {
        string currentFilter = GianHangOrderCore_cl.NormalizeTradeTypeFilter(currentTradeType);
        string nextFilter = GianHangOrderCore_cl.NormalizeTradeTypeFilter(postedTradeType);
        int nextLimit = NormalizeResultLimit(currentLimit);
        bool shouldRebind = false;

        if (!string.Equals(currentFilter, nextFilter, StringComparison.Ordinal))
        {
            currentFilter = nextFilter;
            nextLimit = ResultLimitStep;
            shouldRebind = true;
        }

        if (searchMoreTriggered)
        {
            nextLimit = IncreaseResultLimit(nextLimit);
            shouldRebind = true;
        }

        return new PassivePostbackState
        {
            ShouldRebind = shouldRebind,
            TradeTypeFilter = currentFilter,
            ResultLimit = nextLimit
        };
    }

    public static CreateModeBindingState BuildCreateModeState(
        dbDataContext db,
        string sellerAccount,
        string tradeTypeFilter,
        string keyword,
        int resultLimit,
        IEnumerable<GianHangPosCartSession_cl.CartItem> cart)
    {
        List<GianHangPosCartSession_cl.CartItem> cartItems = (cart ?? Enumerable.Empty<GianHangPosCartSession_cl.CartItem>())
            .Where(item => item != null)
            .ToList();

        return new CreateModeBindingState
        {
            SearchPanel = GianHangPosView_cl.BuildSearchPanel(
                db,
                sellerAccount,
                tradeTypeFilter,
                keyword,
                NormalizeResultLimit(resultLimit)),
            Cart = BuildCartState(cartItems)
        };
    }

    public static OrderListBindingState BuildOrderListState(
        dbDataContext db,
        string sellerAccount,
        string statusFilter,
        string keyword)
    {
        GianHangPosView_cl.OrderListPanelState panel = GianHangPosView_cl.BuildOrderListPanel(
            db,
            sellerAccount,
            statusFilter,
            keyword);

        return new OrderListBindingState
        {
            Panel = panel,
            Summary = panel == null ? new GianHangOrderCore_cl.SellerOrderSummary() : (panel.Summary ?? new GianHangOrderCore_cl.SellerOrderSummary())
        };
    }

    public static CartBindingState BuildCartState(IEnumerable<GianHangPosCartSession_cl.CartItem> cart)
    {
        List<GianHangPosCartSession_cl.CartItem> items = (cart ?? Enumerable.Empty<GianHangPosCartSession_cl.CartItem>())
            .Where(item => item != null)
            .ToList();

        GianHangPosCartView_cl.CartSummaryState summary = GianHangPosCartView_cl.BuildSummary(
            items.Select(item => new GianHangPosCartView_cl.CartLine
            {
                GiaBan = item.GiaBan,
                SoLuong = item.SoLuong
            }));

        return new CartBindingState
        {
            Items = items,
            IsEmpty = items.Count == 0,
            TotalVndText = summary.TotalVndText ?? "0",
            TotalRightsText = summary.TotalRightsText ?? "0.00",
            TotalCountText = summary.TotalCountText ?? "0"
        };
    }

    public static bool TryAddProductToCart(
        dbDataContext db,
        string sellerAccount,
        string productId,
        List<GianHangPosCartSession_cl.CartItem> cart,
        out string warningMessage)
    {
        warningMessage = string.Empty;

        string safeProductId = (productId ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(safeProductId) || cart == null)
            return false;

        GH_SanPham_tb product = GianHangOrderCore_cl.FindSellableProduct(db, sellerAccount, safeProductId);
        if (product == null)
        {
            warningMessage = "Tin không tồn tại hoặc không thuộc không gian /gianhang của tài khoản này.";
            return false;
        }

        GianHangPosCartSession_cl.AddOrIncrement(cart, safeProductId, product);
        return true;
    }
}
