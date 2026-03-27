using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangOrderDetail_cl
{
    private sealed class ResolvedDetailContext
    {
        public string OrderId { get; set; }
        public GH_HoaDon_tb Invoice { get; set; }
        public List<GH_HoaDon_ChiTiet_tb> InvoiceDetails { get; set; }
        public List<DonHang_ChiTiet_tb> LegacyOrderItems { get; set; }
    }

    public sealed class OrderDetailRow
    {
        public long id { get; set; }
        public string id_donhang { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public decimal? giaban { get; set; }
        public int? soluong { get; set; }
        public decimal? thanhtien { get; set; }
        public int PhanTramUuDai { get; set; }
    }

    public sealed class DiscountSummary
    {
        public decimal DiscountAmountVnd { get; set; }
        public decimal DiscountRights { get; set; }
        public bool HasDiscount { get; set; }
    }

    public static List<OrderDetailRow> BuildOrderDetailRows(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            (orderId ?? string.Empty).Trim(),
            null,
            null,
            orderItems,
            true);
        if (context.InvoiceDetails.Count > 0)
            return BuildOrderDetailRowsFromInvoice(db, sellerAccount, context.InvoiceDetails);

        return GianHangOrderDetailLegacy_cl.BuildOrderDetailRows(db, sellerAccount, context.OrderId, context.LegacyOrderItems);
    }

    public static List<OrderDetailRow> BuildOrderDetailRows(
        dbDataContext db,
        string sellerAccount,
        GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            runtime == null ? string.Empty : runtime.OrderId,
            runtime == null ? null : runtime.Invoice,
            null,
            null,
            true);
        return BuildOrderDetailRows(
            db,
            sellerAccount,
            context.OrderId,
            context.Invoice,
            context.InvoiceDetails,
            context.LegacyOrderItems);
    }

    public static List<OrderDetailRow> BuildOrderDetailRows(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            ResolveOrderKey(invoice),
            invoice,
            null,
            null,
            true);
        return BuildOrderDetailRows(db, sellerAccount, context.OrderId, context.Invoice, context.InvoiceDetails, null);
    }

    public static List<OrderDetailRow> BuildOrderDetailRows(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            ResolveOrderKey(invoice),
            invoice,
            invoiceDetails,
            null,
            true);
        return BuildOrderDetailRows(db, sellerAccount, context.OrderId, context.Invoice, context.InvoiceDetails, null);
    }

    public static List<OrderDetailRow> BuildOrderDetailRowsFromInvoiceSnapshot(
        dbDataContext db,
        string sellerAccount,
        IEnumerable<GH_HoaDon_ChiTiet_tb> invoiceDetails)
    {
        return BuildOrderDetailRowsFromInvoice(db, sellerAccount, invoiceDetails);
    }

    public static List<OrderDetailRow> BuildOrderDetailRows(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            orderId,
            invoice,
            null,
            orderItems,
            true);
        return BuildOrderDetailRows(
            db,
            sellerAccount,
            context.OrderId,
            context.Invoice,
            context.InvoiceDetails,
            context.LegacyOrderItems);
    }

    public static List<OrderDetailRow> BuildOrderDetailRows(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            orderId,
            invoice,
            invoiceDetails,
            orderItems,
            true);
        if (context.InvoiceDetails.Count > 0)
            return BuildOrderDetailRowsFromInvoice(db, sellerAccount, context.InvoiceDetails);

        return GianHangOrderDetailLegacy_cl.BuildOrderDetailRows(
            db,
            sellerAccount,
            context.OrderId,
            context.LegacyOrderItems);
    }

    public static void ApplySoldCountAfterExchange(
        dbDataContext db,
        string sellerAccount,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            ResolveLegacyOrderId(orderItems),
            null,
            null,
            orderItems,
            true);
        if (context.InvoiceDetails.Count > 0)
        {
            ApplySoldCountAfterExchange(db, sellerAccount, context.Invoice, context.InvoiceDetails, context.LegacyOrderItems);
            return;
        }

        GianHangOrderDetailLegacy_cl.ApplySoldCountAfterExchange(db, sellerAccount, context.LegacyOrderItems);
    }

    public static void ApplySoldCountAfterExchange(
        dbDataContext db,
        string sellerAccount,
        GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            runtime == null ? string.Empty : runtime.OrderId,
            runtime == null ? null : runtime.Invoice,
            null,
            null,
            true);
        ApplySoldCountAfterExchange(db, sellerAccount, context.Invoice, context.InvoiceDetails, context.LegacyOrderItems);
    }

    public static void ApplySoldCountAfterExchange(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            ResolveOrderKey(invoice),
            invoice,
            null,
            null,
            true);
        ApplySoldCountAfterExchange(db, sellerAccount, context.Invoice, context.InvoiceDetails, null);
    }

    public static void ApplySoldCountAfterExchange(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails)
    {
        ApplySoldCountAfterExchange(db, sellerAccount, invoice, invoiceDetails, null);
    }

    public static void ApplySoldCountAfterExchangeFromInvoiceSnapshot(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails)
    {
        ApplySoldCountAfterExchange(db, sellerAccount, invoice, invoiceDetails, null);
    }

    public static void ApplySoldCountAfterExchange(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            ResolveOrderKey(invoice),
            invoice,
            null,
            orderItems,
            true);
        ApplySoldCountAfterExchange(db, sellerAccount, context.Invoice, context.InvoiceDetails, context.LegacyOrderItems);
    }

    public static void ApplySoldCountAfterExchange(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        List<GH_HoaDon_ChiTiet_tb> safeInvoiceDetails = invoiceDetails ?? new List<GH_HoaDon_ChiTiet_tb>();
        if (safeInvoiceDetails.Count > 0)
        {
            List<int> productIds = safeInvoiceDetails
                .Where(p => p.id_sanpham.HasValue && p.id_sanpham.Value > 0)
                .Select(p => p.id_sanpham.Value)
                .Distinct()
                .ToList();
            if (productIds.Count > 0)
            {
                Dictionary<int, GH_SanPham_tb> nativeMap = GianHangProduct_cl.QueryByStorefront(db, sellerAccount)
                    .Where(p => productIds.Contains(p.id))
                    .ToDictionary(p => p.id, p => p);
                DateTime now = AhaTime_cl.Now;
                for (int i = 0; i < safeInvoiceDetails.Count; i++)
                {
                    GH_HoaDon_ChiTiet_tb detail = safeInvoiceDetails[i];
                    if (!detail.id_sanpham.HasValue)
                        continue;

                    GH_SanPham_tb nativeProduct;
                    if (!nativeMap.TryGetValue(detail.id_sanpham.Value, out nativeProduct) || nativeProduct == null)
                        continue;

                    nativeProduct.so_luong_da_ban = (nativeProduct.so_luong_da_ban ?? 0) + (detail.so_luong ?? 0);
                    nativeProduct.ngay_cap_nhat = now;
                }
                return;
            }
        }

        ApplySoldCountAfterExchange(db, sellerAccount, orderItems);
    }

    public static DiscountSummary BuildDiscountSummary(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            (orderId ?? string.Empty).Trim(),
            null,
            null,
            orderItems,
            true);
        if (context.InvoiceDetails.Count > 0)
            return BuildDiscountSummary(db, sellerAccount, context.OrderId, context.Invoice, context.InvoiceDetails, context.LegacyOrderItems);

        return GianHangOrderDetailLegacy_cl.BuildDiscountSummary(db, sellerAccount, context.OrderId, context.LegacyOrderItems);
    }

    public static DiscountSummary BuildDiscountSummary(
        dbDataContext db,
        string sellerAccount,
        GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            runtime == null ? string.Empty : runtime.OrderId,
            runtime == null ? null : runtime.Invoice,
            null,
            null,
            true);
        return BuildDiscountSummary(
            db,
            sellerAccount,
            context.OrderId,
            context.Invoice,
            context.InvoiceDetails,
            context.LegacyOrderItems);
    }

    public static DiscountSummary BuildDiscountSummary(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            ResolveOrderKey(invoice),
            invoice,
            null,
            null,
            true);
        return BuildDiscountSummary(db, sellerAccount, context.OrderId, context.Invoice, context.InvoiceDetails, null);
    }

    public static DiscountSummary BuildDiscountSummary(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            ResolveOrderKey(invoice),
            invoice,
            invoiceDetails,
            null,
            true);
        return BuildDiscountSummary(db, sellerAccount, context.OrderId, context.Invoice, context.InvoiceDetails, null);
    }

    public static DiscountSummary BuildDiscountSummaryFromInvoiceSnapshot(
        dbDataContext db,
        string sellerAccount,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails)
    {
        return BuildDiscountSummary(db, sellerAccount, invoice, invoiceDetails);
    }

    private static string ResolveOrderKey(GH_HoaDon_tb invoice)
    {
        return GianHangInvoice_cl.ResolveRuntimeOrderKey(invoice);
    }

    public static List<GH_HoaDon_ChiTiet_tb> EnsureInvoiceDetailsForOrder(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice)
    {
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails = LoadInvoiceDetailsForOrder(db, sellerAccount, orderId, invoice);
        if (invoiceDetails.Count > 0)
            return invoiceDetails;

        GH_HoaDon_tb ensuredInvoice = invoice ?? GianHangInvoice_cl.EnsureInvoiceByOrderKey(db, sellerAccount, orderId);
        if (ensuredInvoice != null)
        {
            GianHangInvoice_cl.EnsureDetailSnapshots(db, ensuredInvoice);
            return LoadInvoiceDetailsForOrder(db, sellerAccount, orderId, ensuredInvoice);
        }

        if (db == null || invoice == null)
            return invoiceDetails;

        GianHangInvoice_cl.EnsureDetailSnapshots(db, invoice);
        return LoadInvoiceDetailsForOrder(db, sellerAccount, orderId, invoice);
    }

    public static DiscountSummary BuildDiscountSummary(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            orderId,
            invoice,
            null,
            orderItems,
            true);
        return BuildDiscountSummary(
            db,
            sellerAccount,
            context.OrderId,
            context.Invoice,
            context.InvoiceDetails,
            context.LegacyOrderItems);
    }

    public static DiscountSummary BuildDiscountSummary(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        ResolvedDetailContext context = ResolveDetailContext(
            db,
            sellerAccount,
            orderId,
            invoice,
            invoiceDetails,
            orderItems,
            true);
        if (context.InvoiceDetails.Count > 0)
        {
            DiscountSummary summary = new DiscountSummary();
            for (int i = 0; i < context.InvoiceDetails.Count; i++)
            {
                GH_HoaDon_ChiTiet_tb detail = context.InvoiceDetails[i];
                int percent = ResolveInvoiceDetailDiscountPercent(detail);
                decimal lineTotal = detail == null ? 0m : (detail.thanh_tien ?? 0m);
                if (lineTotal <= 0m || percent <= 0)
                    continue;

                summary.DiscountAmountVnd += lineTotal * percent / 100m;
            }
            summary.DiscountRights = GianHangCheckoutCore_cl.ConvertVndToRights(summary.DiscountAmountVnd);
            summary.HasDiscount = summary.DiscountAmountVnd > 0m && summary.DiscountRights > 0m;
            return summary;
        }

        return GianHangOrderDetailLegacy_cl.BuildDiscountSummary(db, sellerAccount, context.OrderId, context.LegacyOrderItems);
    }

    private static ResolvedDetailContext ResolveDetailContext(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails,
        List<DonHang_ChiTiet_tb> orderItems,
        bool includeLegacyItems)
    {
        string resolvedOrderId = (orderId ?? string.Empty).Trim();
        if (resolvedOrderId == string.Empty)
        {
            if (invoice != null)
                resolvedOrderId = ResolveOrderKey(invoice);
            if (resolvedOrderId == string.Empty)
                resolvedOrderId = ResolveLegacyOrderId(orderItems);
        }

        GH_HoaDon_tb resolvedInvoice = invoice;
        if (resolvedInvoice == null && db != null && !string.IsNullOrWhiteSpace(sellerAccount) && !string.IsNullOrWhiteSpace(resolvedOrderId))
            resolvedInvoice = GianHangInvoice_cl.EnsureInvoiceByOrderKey(db, sellerAccount, resolvedOrderId);

        List<GH_HoaDon_ChiTiet_tb> resolvedInvoiceDetails = invoiceDetails ?? EnsureInvoiceDetailsForOrder(
            db,
            sellerAccount,
            resolvedOrderId,
            resolvedInvoice);

        List<DonHang_ChiTiet_tb> resolvedLegacyOrderItems = orderItems;
        if (includeLegacyItems)
        {
            resolvedLegacyOrderItems = ResolveLegacyOrderItems(
                db,
                sellerAccount,
                resolvedOrderId,
                resolvedInvoice,
                resolvedInvoiceDetails,
                orderItems);

            if (TryBackfillInvoiceDetailsFromLegacy(
                db,
                sellerAccount,
                resolvedOrderId,
                resolvedInvoice,
                resolvedInvoiceDetails,
                resolvedLegacyOrderItems))
            {
                resolvedInvoiceDetails = LoadInvoiceDetailsForOrder(
                    db,
                    sellerAccount,
                    resolvedOrderId,
                    resolvedInvoice);
            }

            if (resolvedInvoiceDetails != null && resolvedInvoiceDetails.Count > 0)
                resolvedLegacyOrderItems = null;
        }

        return new ResolvedDetailContext
        {
            OrderId = resolvedOrderId,
            Invoice = resolvedInvoice,
            InvoiceDetails = resolvedInvoiceDetails ?? new List<GH_HoaDon_ChiTiet_tb>(),
            LegacyOrderItems = resolvedLegacyOrderItems
        };
    }

    private static bool TryBackfillInvoiceDetailsFromLegacy(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails,
        List<DonHang_ChiTiet_tb> legacyOrderItems)
    {
        if (db == null || invoice == null)
            return false;

        List<GH_HoaDon_ChiTiet_tb> safeInvoiceDetails = invoiceDetails ?? new List<GH_HoaDon_ChiTiet_tb>();
        if (safeInvoiceDetails.Count > 0)
            return false;

        List<DonHang_ChiTiet_tb> safeLegacyOrderItems = legacyOrderItems ?? new List<DonHang_ChiTiet_tb>();
        if (safeLegacyOrderItems.Count == 0)
            return false;

        string runtimeSellerAccount = (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
        if (runtimeSellerAccount == string.Empty)
            runtimeSellerAccount = (invoice.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (runtimeSellerAccount == string.Empty)
            return false;

        bool changed = GianHangInvoiceLegacySnapshot_cl.BackfillInvoiceDetailsFromLegacy(
            db,
            invoice,
            safeInvoiceDetails,
            safeLegacyOrderItems,
            runtimeSellerAccount);
        if (changed)
            db.SubmitChanges();

        return changed;
    }

    private static string ResolveLegacyOrderId(List<DonHang_ChiTiet_tb> orderItems)
    {
        if (orderItems == null || orderItems.Count == 0 || orderItems[0] == null)
            return string.Empty;

        return (orderItems[0].id_donhang ?? string.Empty).Trim();
    }

    private static List<OrderDetailRow> BuildOrderDetailRowsFromInvoice(
        dbDataContext db,
        string sellerAccount,
        IEnumerable<GH_HoaDon_ChiTiet_tb> invoiceDetails)
    {
        List<GH_HoaDon_ChiTiet_tb> details = (invoiceDetails ?? Enumerable.Empty<GH_HoaDon_ChiTiet_tb>()).ToList();
        List<OrderDetailRow> rows = new List<OrderDetailRow>(details.Count);
        if (details.Count == 0)
            return rows;

        List<int> productIds = details
            .Where(p => p.id_sanpham.HasValue && p.id_sanpham.Value > 0)
            .Select(p => p.id_sanpham.Value)
            .Distinct()
            .ToList();

        Dictionary<int, GH_SanPham_tb> nativeMap = productIds.Count == 0
            ? new Dictionary<int, GH_SanPham_tb>()
            : GianHangProduct_cl.QueryByStorefront(db, sellerAccount)
                .Where(p => productIds.Contains(p.id))
                .ToDictionary(p => p.id, p => p);

        for (int i = 0; i < details.Count; i++)
        {
            GH_HoaDon_ChiTiet_tb detail = details[i];
            GH_SanPham_tb nativeProduct = null;
            if (detail.id_sanpham.HasValue)
                nativeMap.TryGetValue(detail.id_sanpham.Value, out nativeProduct);

            rows.Add(new OrderDetailRow
            {
                id = detail.id,
                id_donhang = string.Empty,
                name = !string.IsNullOrWhiteSpace(detail.ten_sanpham)
                    ? (detail.ten_sanpham ?? string.Empty)
                    : (nativeProduct == null ? string.Empty : (nativeProduct.ten ?? string.Empty)),
                image = !string.IsNullOrWhiteSpace(detail.hinh_anh)
                    ? (detail.hinh_anh ?? string.Empty)
                    : (nativeProduct == null ? string.Empty : (nativeProduct.hinh_anh ?? string.Empty)),
                giaban = detail.gia,
                soluong = detail.so_luong,
                thanhtien = detail.thanh_tien,
                PhanTramUuDai = ResolveInvoiceDetailDiscountPercent(detail)
            });
        }

        return rows;
    }

    public static List<GH_HoaDon_ChiTiet_tb> LoadInvoiceDetailsForOrder(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice)
    {
        if (db == null)
            return new List<GH_HoaDon_ChiTiet_tb>();

        if (invoice != null)
            return GianHangInvoice_cl.GetDetails(db, invoice);

        if (string.IsNullOrWhiteSpace(sellerAccount) || string.IsNullOrWhiteSpace(orderId))
            return new List<GH_HoaDon_ChiTiet_tb>();

        return GianHangInvoice_cl.GetDetailsByOrderKey(db, sellerAccount, orderId);
    }

    // Compatibility shim for older call sites that still resolve the shorter helper name
    // during App_Code recompilation on Mono.
    public static List<GH_HoaDon_ChiTiet_tb> LoadInvoiceDetails(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice)
    {
        return LoadInvoiceDetailsForOrder(db, sellerAccount, orderId, invoice);
    }

    public static Dictionary<int, GH_HoaDon_ChiTiet_tb> LoadInvoiceDetailMapByOrderId(
        dbDataContext db,
        string sellerAccount,
        string orderId)
    {
        Dictionary<int, GH_HoaDon_ChiTiet_tb> map = new Dictionary<int, GH_HoaDon_ChiTiet_tb>();
        if (db == null || string.IsNullOrWhiteSpace(sellerAccount) || string.IsNullOrWhiteSpace(orderId))
            return map;

        GH_HoaDon_tb invoice = GianHangInvoice_cl.EnsureInvoiceByOrderKey(db, sellerAccount, orderId);
        if (invoice == null)
            return map;

        foreach (GH_HoaDon_ChiTiet_tb detail in GianHangInvoice_cl.GetDetails(db, invoice.id)
            .Where(p => p.id_sanpham.HasValue)
            .OrderByDescending(p => p.id))
        {
            int productId = detail.id_sanpham.Value;
            if (!map.ContainsKey(productId))
                map[productId] = detail;
        }

        return map;
    }

    private static List<DonHang_ChiTiet_tb> ResolveLegacyOrderItems(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        if (orderItems != null)
            return orderItems;

        List<GH_HoaDon_ChiTiet_tb> safeInvoiceDetails = invoiceDetails ?? new List<GH_HoaDon_ChiTiet_tb>();
        if (safeInvoiceDetails.Count > 0)
            return null;

        return GianHangOrderDetailLegacy_cl.LoadLegacyOrderItemsIfNeeded(
            db,
            sellerAccount,
            safeInvoiceDetails,
            orderId,
            invoice);
    }

    private static int ResolveInvoiceDetailDiscountPercent(GH_HoaDon_ChiTiet_tb invoiceDetail)
    {
        if (invoiceDetail == null || !invoiceDetail.phan_tram_uu_dai.HasValue)
            return 0;

        int percent = invoiceDetail.phan_tram_uu_dai.Value;
        if (percent < 0) percent = 0;
        if (percent > 50) percent = 50;
        return percent;
    }
}
