using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangBuyerOrder_cl
{
    public const string NoticeSessionKey = "gianhang_buyer_order_notice";

    public sealed class BuyerOrderRow
    {
        public long InvoiceId { get; set; }
        public string OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string StoreAccount { get; set; }
        public string StoreName { get; set; }
        public string StatusGroup { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public decimal TotalAmount { get; set; }
        public string FirstItemName { get; set; }
        public string FirstItemImage { get; set; }
        public int FirstItemQty { get; set; }
        public int TotalItems { get; set; }
        public string PublicUrl { get; set; }
    }

    public sealed class BuyerOrderSummary
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int WaitingExchange { get; set; }
        public int Exchanged { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
    }

    public static string NormalizeStatusFilter(string rawFilter)
    {
        string filter = (rawFilter ?? string.Empty).Trim().ToLowerInvariant();
        switch (filter)
        {
            case "da-dat":
            case "cho-trao-doi":
            case "da-trao-doi":
            case "da-giao":
            case "da-huy":
                return filter;
            default:
                return "all";
        }
    }

    public static string ResolveStatusFilterLabel(string rawFilter)
    {
        switch (NormalizeStatusFilter(rawFilter))
        {
            case "da-dat":
                return "Đã đặt";
            case "cho-trao-doi":
                return "Chờ trao đổi";
            case "da-trao-doi":
                return "Đã trao đổi";
            case "da-giao":
                return "Đã giao";
            case "da-huy":
                return "Đã hủy";
            default:
                return "Tất cả";
        }
    }

    public static BuyerOrderSummary BuildSummary(IList<BuyerOrderRow> rows)
    {
        IList<BuyerOrderRow> list = rows ?? new List<BuyerOrderRow>();
        return new BuyerOrderSummary
        {
            Total = list.Count,
            Pending = list.Count(p => p.StatusGroup == "da-dat"),
            WaitingExchange = list.Count(p => p.StatusGroup == "cho-trao-doi"),
            Exchanged = list.Count(p => p.StatusGroup == "da-trao-doi"),
            Delivered = list.Count(p => p.StatusGroup == "da-giao"),
            Cancelled = list.Count(p => p.StatusGroup == "da-huy")
        };
    }

    public static BuyerOrderSummary BuildSummary(dbDataContext db, string buyerAccount)
    {
        string accountKey = NormalizeAccount(buyerAccount);
        BuyerOrderSummary summary = new BuyerOrderSummary();
        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return summary;

        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.LoadBuyerInvoicesWithRuntime(
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

    public static void SetNotice(System.Web.SessionState.HttpSessionState session, string message)
    {
        if (session == null)
            return;

        session[NoticeSessionKey] = (message ?? string.Empty).Trim();
    }

    public static string PullNotice(System.Web.SessionState.HttpSessionState session)
    {
        if (session == null)
            return string.Empty;

        string raw = Convert.ToString(session[NoticeSessionKey]) ?? string.Empty;
        session[NoticeSessionKey] = null;
        return raw.Trim();
    }

    public static List<BuyerOrderRow> LoadOrders(
        dbDataContext db,
        string buyerAccount,
        string statusFilter,
        string keyword,
        int take)
    {
        string accountKey = NormalizeAccount(buyerAccount);
        string normalizedFilter = NormalizeStatusFilter(statusFilter);
        string safeKeyword = (keyword ?? string.Empty).Trim().ToLowerInvariant();
        int safeTake = take <= 0 ? 100 : take;

        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return new List<BuyerOrderRow>();

        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.LoadBuyerInvoicesWithRuntime(
            db,
            accountKey,
            Math.Max(safeTake * 2, 500),
            Math.Max(safeTake * 2, 5000));
        Dictionary<long, List<GH_HoaDon_ChiTiet_tb>> detailMap = LoadInvoiceDetailMap(db, invoices);
        Dictionary<string, taikhoan_tb> sellerMap = LoadSellerMap(db, invoices);

        List<BuyerOrderRow> rows = new List<BuyerOrderRow>(invoices.Count);
        for (int i = 0; i < invoices.Count; i++)
        {
            GH_HoaDon_tb invoice = invoices[i];
            string statusGroup = GianHangInvoice_cl.ResolveOrderStatusGroup(invoice);
            if (normalizedFilter != "all" && statusGroup != normalizedFilter)
                continue;

            taikhoan_tb seller = ResolveSeller(sellerMap, invoice);
            string storeName = ResolveStoreName(invoice, seller);
            string publicOrderId = GianHangInvoice_cl.ResolveOrderPublicId(invoice);
            string runtimeOrderKey = GianHangInvoice_cl.ResolveRuntimeOrderKey(invoice);
            string haystack = string.Join(" ",
                invoice.id.ToString(),
                publicOrderId,
                runtimeOrderKey,
                invoice.shop_taikhoan ?? string.Empty,
                storeName,
                invoice.ten_khach ?? string.Empty).ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(safeKeyword) && !haystack.Contains(safeKeyword))
                continue;

            List<GH_HoaDon_ChiTiet_tb> details;
            detailMap.TryGetValue(invoice.id, out details);
            GH_HoaDon_ChiTiet_tb firstDetail = details == null ? null : details.FirstOrDefault();

            rows.Add(new BuyerOrderRow
            {
                InvoiceId = invoice.id,
                OrderId = publicOrderId,
                CreatedAt = invoice.ngay_tao,
                StoreAccount = (invoice.shop_taikhoan ?? string.Empty).Trim(),
                StoreName = storeName,
                StatusGroup = statusGroup,
                StatusText = GianHangInvoice_cl.ResolveOrderStatusText(invoice),
                StatusCss = GianHangInvoice_cl.ResolveOrderStatusCss(invoice),
                TotalAmount = GianHangInvoice_cl.ResolveTotalAmount(invoice),
                FirstItemName = firstDetail == null ? "Đơn hàng /gianhang" : (firstDetail.ten_sanpham ?? "Sản phẩm"),
                FirstItemImage = firstDetail == null ? string.Empty : (firstDetail.hinh_anh ?? string.Empty),
                FirstItemQty = firstDetail == null ? 0 : (firstDetail.so_luong ?? 0),
                TotalItems = details == null ? 0 : details.Sum(p => p.so_luong ?? 0),
                PublicUrl = GianHangRoutes_cl.BuildStorefrontUrl(invoice.shop_taikhoan)
            });
        }

        if (rows.Count > safeTake)
            return rows.Take(safeTake).ToList();

        return rows;
    }

    private static Dictionary<long, List<GH_HoaDon_ChiTiet_tb>> LoadInvoiceDetailMap(dbDataContext db, IList<GH_HoaDon_tb> invoices)
    {
        GianHangInvoice_cl.EnsureSchema(db);
        List<long> invoiceIds = invoices == null
            ? new List<long>()
            : invoices.Select(p => p.id).Distinct().ToList();
        if (invoiceIds.Count == 0)
            return new Dictionary<long, List<GH_HoaDon_ChiTiet_tb>>();

        return db.GetTable<GH_HoaDon_ChiTiet_tb>()
            .Where(p => invoiceIds.Contains(p.id_hoadon))
            .OrderBy(p => p.id)
            .ToList()
            .GroupBy(p => p.id_hoadon)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private static Dictionary<string, taikhoan_tb> LoadSellerMap(dbDataContext db, IList<GH_HoaDon_tb> invoices)
    {
        List<string> sellerKeys = invoices == null
            ? new List<string>()
            : invoices
                .Select(p => NormalizeAccount(p.shop_taikhoan))
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        if (sellerKeys.Count == 0)
            return new Dictionary<string, taikhoan_tb>(StringComparer.OrdinalIgnoreCase);

        return db.taikhoan_tbs
            .Where(p => sellerKeys.Contains(p.taikhoan))
            .ToList()
            .ToDictionary(p => NormalizeAccount(p.taikhoan), p => p, StringComparer.OrdinalIgnoreCase);
    }

    private static taikhoan_tb ResolveSeller(Dictionary<string, taikhoan_tb> sellerMap, GH_HoaDon_tb invoice)
    {
        if (sellerMap == null || invoice == null)
            return null;

        taikhoan_tb seller;
        return sellerMap.TryGetValue(NormalizeAccount(invoice.shop_taikhoan), out seller) ? seller : null;
    }

    private static string ResolveStoreName(GH_HoaDon_tb invoice, taikhoan_tb seller)
    {
        if (seller != null)
        {
            string name = (seller.ten_shop ?? string.Empty).Trim();
            if (name != string.Empty)
                return name;

            name = (seller.hoten ?? string.Empty).Trim();
            if (name != string.Empty)
                return name;
        }

        string fallback = invoice == null ? string.Empty : (invoice.shop_taikhoan ?? string.Empty).Trim();
        return fallback == string.Empty ? "Gian hàng đối tác" : fallback;
    }

    private static string NormalizeAccount(string rawAccount)
    {
        return (rawAccount ?? string.Empty).Trim().ToLowerInvariant();
    }
}
