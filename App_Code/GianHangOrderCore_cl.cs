using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangOrderCore_cl
{
    private sealed class SellablePostRaw
    {
        public int Id { get; set; }
        public int? ReferenceId { get; set; }
        public string ImageRaw { get; set; }
        public string Name { get; set; }
        public decimal GiaBan { get; set; }
        public string PostTypeRaw { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public sealed class SellerOrderRow
    {
        public long Id { get; set; }
        public long SourceInvoiceId { get; set; }
        public DateTime? OrderedAt { get; set; }
        public string BuyerDisplay { get; set; }
        public string OrderStatus { get; set; }
        public string ExchangeStatus { get; set; }
        public string StatusGroup { get; set; }
        public string StatusText { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsOffline { get; set; }
        public bool CanOpenWait { get; set; }
        public bool CanCancelWait { get; set; }
        public bool CanCancelOrder { get; set; }
        public bool CanMarkDelivered { get; set; }
        public bool IsWaitingExchange { get; set; }
    }

    public sealed class SellerOrderSummary
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int WaitingExchange { get; set; }
        public int Exchanged { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
    }

    public sealed class SellablePostCard
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public decimal GiaBan { get; set; }
        public string PostType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int DiscountPercent { get; set; }

        // Legacy-friendly aliases so existing WebForms bindings can stay stable
        public int HomePostId { get { return ReferenceId; } set { ReferenceId = value; } }
        public int id { get { return Id; } set { Id = value; } }
        public int id_baiviet { get { return ReferenceId; } set { ReferenceId = value; } }
        public string image { get { return Image; } set { Image = value; } }
        public string name { get { return Name; } set { Name = value; } }
        public decimal giaban { get { return GiaBan; } set { GiaBan = value; } }
        public string phanloai { get { return PostType; } set { PostType = value; } }
    }

    public static string NormalizeTradeTypeFilter(string rawFilter)
    {
        string filter = (rawFilter ?? "").Trim().ToLowerInvariant();
        if (filter == "product" || filter == "service")
            return filter;
        return "all";
    }

    public static IQueryable<GH_SanPham_tb> QuerySellableProducts(dbDataContext db, string sellerAccount)
    {
        string accountKey = (sellerAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return Enumerable.Empty<GH_SanPham_tb>().AsQueryable();

        return GianHangProduct_cl.QueryPublicByStorefront(db, accountKey);
    }

    public static List<SellablePostCard> LoadSellablePosts(
        dbDataContext db,
        string sellerAccount,
        string tradeTypeFilter,
        string keyword,
        int limitPlusOne)
    {
        string normalizedFilter = NormalizeTradeTypeFilter(tradeTypeFilter);
        string safeKeyword = (keyword ?? "").Trim();
        int safeLimit = Math.Max(1, limitPlusOne);

        IQueryable<GH_SanPham_tb> query = QuerySellableProducts(db, sellerAccount);

        if (normalizedFilter == "product")
            query = query.Where(p => p.loai == GianHangProduct_cl.LoaiSanPham);
        else if (normalizedFilter == "service")
            query = query.Where(p => p.loai == GianHangProduct_cl.LoaiDichVu);

        if (!string.IsNullOrWhiteSpace(safeKeyword))
            query = query.Where(p => (p.ten ?? "").Contains(safeKeyword));

        List<SellablePostRaw> rawRows = query
            .OrderByDescending(p => p.ngay_cap_nhat ?? p.ngay_tao)
            .Select(p => new SellablePostRaw
            {
                Id = p.id,
                ReferenceId = p.id_baiviet ?? p.id,
                ImageRaw = p.hinh_anh,
                Name = p.ten,
                GiaBan = p.gia_ban ?? 0m,
                PostTypeRaw = p.loai,
                CreatedAt = p.ngay_cap_nhat ?? p.ngay_tao
            })
            .Take(safeLimit)
            .ToList();

        List<SellablePostCard> rows = new List<SellablePostCard>(rawRows.Count);
        for (int i = 0; i < rawRows.Count; i++)
        {
            SellablePostRaw item = rawRows[i];
            rows.Add(new SellablePostCard
            {
                Id = item.Id,
                ReferenceId = item.ReferenceId ?? item.Id,
                Image = item.ImageRaw,
                Name = item.Name,
                GiaBan = item.GiaBan,
                PostType = GianHangProduct_cl.NormalizeLoai(item.PostTypeRaw),
                CreatedAt = item.CreatedAt,
                DiscountPercent = 0
            });
        }

        return rows;
    }

    public static GH_SanPham_tb FindSellableProduct(dbDataContext db, string sellerAccount, string productId)
    {
        string accountKey = (sellerAccount ?? "").Trim().ToLowerInvariant();
        string safeProductId = (productId ?? "").Trim();
        if (db == null || string.IsNullOrWhiteSpace(accountKey) || string.IsNullOrWhiteSpace(safeProductId))
            return null;

        int parsedId;
        if (!int.TryParse(safeProductId, out parsedId) || parsedId <= 0)
            return null;

        IQueryable<GH_SanPham_tb> query = QuerySellableProducts(db, accountKey);
        GH_SanPham_tb product = query.FirstOrDefault(p => p.id == parsedId || p.id_baiviet == parsedId);
        if (product != null)
            return product;

        string referenceKey = parsedId.ToString();
        Dictionary<string, int> nativeMap = GianHangLegacyPost_cl.ResolveNativeIdByReferenceIds(
            db,
            new List<string> { referenceKey });
        int nativeId;
        if (nativeMap != null
            && nativeMap.TryGetValue(referenceKey, out nativeId)
            && nativeId > 0)
        {
            return query.FirstOrDefault(p => p.id == nativeId);
        }

        return null;
    }

    public static string NormalizeOrderStatusFilter(string rawFilter)
    {
        string filter = (rawFilter ?? "").Trim().ToLowerInvariant();
        switch (filter)
        {
            case "cho-trao-doi":
            case "da-trao-doi":
            case "da-huy":
            case "da-giao":
            case "da-dat":
                return filter;
            default:
                return "all";
        }
    }

    public static SellerOrderSummary BuildSellerOrderSummary(dbDataContext db, string sellerAccount)
    {
        string accountKey = (sellerAccount ?? "").Trim().ToLowerInvariant();
        SellerOrderSummary summary = new SellerOrderSummary();
        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return summary;

        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.LoadStorefrontInvoicesWithRuntime(
            db,
            accountKey,
            500,
            5000);

        summary.Total = invoices.Count;
        for (int i = 0; i < invoices.Count; i++)
        {
            switch (GianHangInvoice_cl.ResolveOrderStatusGroup(invoices[i], null))
            {
                case "cho-trao-doi":
                    summary.WaitingExchange++;
                    break;
                case "da-trao-doi":
                    summary.Exchanged++;
                    break;
                case "da-giao":
                    summary.Delivered++;
                    break;
                case "da-huy":
                    summary.Cancelled++;
                    break;
                default:
                    summary.Pending++;
                    break;
            }
        }

        return summary;
    }

    public static List<SellerOrderRow> LoadSellerOrders(
        dbDataContext db,
        string sellerAccount,
        string statusFilter,
        string keyword,
        int take)
    {
        string accountKey = (sellerAccount ?? "").Trim().ToLowerInvariant();
        string normalizedFilter = NormalizeOrderStatusFilter(statusFilter);
        string safeKeyword = (keyword ?? "").Trim().ToLowerInvariant();
        int safeTake = Math.Max(1, take);

        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return new List<SellerOrderRow>();

        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.LoadStorefrontInvoicesWithRuntime(
            db,
            accountKey,
            Math.Max(safeTake * 2, 500),
            Math.Max(safeTake * 2, 5000));

        List<SellerOrderRow> rows = new List<SellerOrderRow>();
        for (int i = 0; i < invoices.Count; i++)
        {
            GH_HoaDon_tb invoice = invoices[i];
            string statusGroup = GianHangInvoice_cl.ResolveOrderStatusGroup(invoice);
            if (normalizedFilter != "all" && statusGroup != normalizedFilter)
                continue;

            string buyerDisplay = GianHangInvoice_cl.ResolveBuyerDisplay(invoice);
            string actionOrderKey = GianHangInvoice_cl.ResolveOrderPublicId(invoice);
            bool hasActionOrderKey = !string.IsNullOrWhiteSpace((actionOrderKey ?? string.Empty).Trim());
            if (!string.IsNullOrWhiteSpace(safeKeyword))
            {
                string haystack = string.Join(" ",
                    invoice.id.ToString(),
                    invoice.buyer_account ?? "",
                    invoice.ten_khach ?? "",
                    buyerDisplay ?? "").ToLowerInvariant();
                if (!haystack.Contains(safeKeyword))
                    continue;
            }

            rows.Add(new SellerOrderRow
            {
                Id = ParseOrderRowId(invoice),
                SourceInvoiceId = invoice.id,
                OrderedAt = invoice.ngay_tao,
                BuyerDisplay = buyerDisplay,
                OrderStatus = ResolveOrderStatus(invoice),
                ExchangeStatus = ResolveExchangeStatus(invoice),
                StatusGroup = statusGroup,
                StatusText = GianHangInvoice_cl.ResolveOrderStatusText(invoice),
                TotalAmount = GianHangInvoice_cl.ResolveTotalAmount(invoice),
                IsOffline = GianHangInvoice_cl.ResolveIsOffline(invoice),
                CanOpenWait = hasActionOrderKey && GianHangInvoice_cl.CanOpenWaitingExchange(invoice),
                CanCancelWait = hasActionOrderKey && GianHangInvoice_cl.CanCancelWaitingExchange(invoice),
                CanCancelOrder = hasActionOrderKey && GianHangInvoice_cl.CanCancelOrder(invoice),
                CanMarkDelivered = hasActionOrderKey && GianHangInvoice_cl.CanMarkDelivered(invoice),
                IsWaitingExchange = GianHangInvoice_cl.IsWaitingExchange(invoice)
            });
        }

        if (rows.Count > safeTake)
            return rows.Take(safeTake).ToList();

        return rows;
    }
    private static string ResolveOrderStatus(GH_HoaDon_tb invoice)
    {
        string value = (invoice == null ? "" : (invoice.order_status ?? "")).Trim();
        if (!string.IsNullOrWhiteSpace(value))
            return value;
        return DonHangStateMachine_cl.Order_DaDat;
    }

    private static string ResolveExchangeStatus(GH_HoaDon_tb invoice)
    {
        string value = (invoice == null ? "" : (invoice.exchange_status ?? "")).Trim();
        if (!string.IsNullOrWhiteSpace(value))
            return value;
        return DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
    }

    private static long ParseOrderRowId(GH_HoaDon_tb invoice)
    {
        string raw = GianHangInvoice_cl.ResolveOrderPublicId(invoice);
        long value;
        if (long.TryParse((raw ?? string.Empty).Trim(), out value) && value > 0)
            return value;

        return invoice == null ? 0L : invoice.id;
    }

}
