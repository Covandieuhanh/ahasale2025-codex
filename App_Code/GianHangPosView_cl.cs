using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangPosView_cl
{
    public sealed class SearchPanelState
    {
        public List<GianHangOrderCore_cl.SellablePostCard> Rows { get; set; }
        public bool HasMore { get; set; }
        public bool IsEmpty { get; set; }
        public string NoteText { get; set; }
        public string SummaryText { get; set; }
    }

    public sealed class OrderListPanelState
    {
        public GianHangOrderCore_cl.SellerOrderSummary Summary { get; set; }
        public List<GianHangOrderCore_cl.SellerOrderRow> Rows { get; set; }
        public bool IsEmpty { get; set; }
        public string FilterLabel { get; set; }
        public string SummaryText { get; set; }
    }

    public static SearchPanelState BuildSearchPanel(
        dbDataContext db,
        string sellerAccount,
        string tradeTypeFilter,
        string keyword,
        int limit)
    {
        int safeLimit = limit <= 0 ? 20 : limit;
        string normalizedFilter = GianHangOrderCore_cl.NormalizeTradeTypeFilter(tradeTypeFilter);
        string filterLabel = ResolveTradeTypeFilterLabel(normalizedFilter);
        string safeKeyword = (keyword ?? string.Empty).Trim();

        List<GianHangOrderCore_cl.SellablePostCard> rows = GianHangOrderCore_cl.LoadSellablePosts(
            db,
            sellerAccount,
            normalizedFilter,
            safeKeyword,
            safeLimit + 1);

        bool hasMore = rows.Count > safeLimit;
        List<GianHangOrderCore_cl.SellablePostCard> visibleRows = hasMore ? rows.Take(safeLimit).ToList() : rows;

        return new SearchPanelState
        {
            Rows = visibleRows,
            HasMore = hasMore,
            IsEmpty = visibleRows.Count == 0,
            NoteText = string.Format("Đang lọc: {0}. Gõ tên để tìm nhanh, bấm Thêm để đưa vào giỏ.", filterLabel),
            SummaryText = visibleRows.Count == 0
                ? "Không có kết quả phù hợp."
                : string.Format("Hiển thị {0} tin đầu tiên theo bộ lọc {1}.", visibleRows.Count, filterLabel.ToLowerInvariant())
        };
    }

    public static OrderListPanelState BuildOrderListPanel(
        dbDataContext db,
        string sellerAccount,
        string statusFilter,
        string keyword)
    {
        string normalizedFilter = GianHangOrderCore_cl.NormalizeOrderStatusFilter(statusFilter);
        string filterLabel = ResolveOrderStatusFilterLabel(normalizedFilter);
        string safeKeyword = (keyword ?? string.Empty).Trim();

        GianHangOrderCore_cl.SellerOrderSummary summary = GianHangOrderCore_cl.BuildSellerOrderSummary(db, sellerAccount);
        List<GianHangOrderCore_cl.SellerOrderRow> rows = GianHangOrderCore_cl.LoadSellerOrders(db, sellerAccount, normalizedFilter, safeKeyword, 300);

        return new OrderListPanelState
        {
            Summary = summary,
            Rows = rows,
            IsEmpty = rows.Count == 0,
            FilterLabel = filterLabel,
            SummaryText = rows.Count == 0
                ? "Chưa có đơn phù hợp với bộ lọc hiện tại."
                : string.Format("Hiển thị {0} đơn trong không gian /gianhang theo bộ lọc {1}.", rows.Count, filterLabel.ToLowerInvariant())
        };
    }

    public static string ResolveTradeTypeFilterLabel(string tradeTypeFilter)
    {
        switch ((tradeTypeFilter ?? "all").Trim().ToLowerInvariant())
        {
            case "product": return "Sản phẩm";
            case "service": return "Dịch vụ";
            default: return "Tất cả";
        }
    }

    public static string ResolveOrderStatusFilterLabel(string filterKey)
    {
        switch ((filterKey ?? "all").Trim().ToLowerInvariant())
        {
            case "da-dat": return "Đã đặt";
            case "cho-trao-doi": return "Chờ Trao đổi";
            case "da-trao-doi": return "Đã Trao đổi";
            case "da-giao": return "Đã giao";
            case "da-huy": return "Đã hủy";
            default: return "Tất cả trạng thái";
        }
    }
}
