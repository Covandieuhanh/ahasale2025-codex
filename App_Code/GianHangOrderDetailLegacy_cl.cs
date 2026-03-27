using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangOrderDetailLegacy_cl
{
    private sealed class OrderItemSnapshotContext
    {
        public Dictionary<string, GH_SanPham_tb> NativeMap { get; set; }
        public Dictionary<int, GH_HoaDon_ChiTiet_tb> InvoiceDetailMap { get; set; }
        public Dictionary<string, BaiViet_tb> LegacyMap { get; set; }
    }

    public static List<GianHangOrderDetail_cl.OrderDetailRow> BuildOrderDetailRows(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        List<GianHangOrderDetail_cl.OrderDetailRow> rows = new List<GianHangOrderDetail_cl.OrderDetailRow>();
        if (orderItems == null || orderItems.Count == 0)
            return rows;

        string resolvedOrderId = ResolveOrderId(orderId, orderItems);
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails = GianHangOrderDetail_cl.EnsureInvoiceDetailsForOrder(
            db,
            sellerAccount,
            resolvedOrderId,
            null);
        if (invoiceDetails.Count > 0)
        {
            return GianHangOrderDetail_cl.BuildOrderDetailRowsFromInvoiceSnapshot(db, sellerAccount, invoiceDetails);
        }

        OrderItemSnapshotContext context = BuildSnapshotContext(db, sellerAccount, resolvedOrderId, orderItems, true);

        foreach (DonHang_ChiTiet_tb item in orderItems)
        {
            GH_SanPham_tb nativeProduct = GianHangInvoiceLegacySnapshot_cl.ResolveNativeProduct(
                context == null ? null : context.NativeMap,
                item);
            GH_HoaDon_ChiTiet_tb invoiceDetail = ResolveInvoiceDetail(
                item,
                context == null ? null : context.NativeMap,
                context == null ? null : context.InvoiceDetailMap);

            rows.Add(new GianHangOrderDetail_cl.OrderDetailRow
            {
                id = item.id,
                id_donhang = item.id_donhang,
                name = ResolveRowName(item, nativeProduct, invoiceDetail),
                image = ResolveRowImage(item, nativeProduct, invoiceDetail),
                giaban = item.giaban,
                soluong = item.soluong,
                thanhtien = item.thanhtien,
                PhanTramUuDai = ResolveOrderLineDiscountPercent(
                    item,
                    nativeProduct,
                    invoiceDetail,
                    context == null ? null : context.LegacyMap)
            });
        }

        return rows;
    }

    public static void ApplySoldCountAfterExchange(
        dbDataContext db,
        string sellerAccount,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        if (db == null || orderItems == null || orderItems.Count == 0)
            return;

        string orderId = ResolveOrderId(string.Empty, orderItems);
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails = GianHangOrderDetail_cl.EnsureInvoiceDetailsForOrder(
            db,
            sellerAccount,
            orderId,
            null);
        if (invoiceDetails.Count > 0)
        {
            GianHangOrderDetail_cl.ApplySoldCountAfterExchangeFromInvoiceSnapshot(
                db,
                sellerAccount,
                null,
                invoiceDetails);
            return;
        }

        Dictionary<string, GH_SanPham_tb> nativeMap = GianHangInvoiceLegacySnapshot_cl.LoadNativeProductMapByOrderItems(
            db,
            sellerAccount,
            orderItems);
        DateTime now = AhaTime_cl.Now;

        foreach (DonHang_ChiTiet_tb item in orderItems)
        {
            GH_SanPham_tb nativeProduct = GianHangInvoiceLegacySnapshot_cl.ResolveNativeProduct(
                nativeMap,
                item);

            if (nativeProduct != null)
            {
                nativeProduct.so_luong_da_ban = (nativeProduct.so_luong_da_ban ?? 0) + item.soluong;
                nativeProduct.ngay_cap_nhat = now;
            }
        }
    }

    public static GianHangOrderDetail_cl.DiscountSummary BuildDiscountSummary(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        GianHangOrderDetail_cl.DiscountSummary summary = new GianHangOrderDetail_cl.DiscountSummary();
        if (orderItems == null || orderItems.Count == 0)
            return summary;

        string resolvedOrderId = ResolveOrderId(orderId, orderItems);
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails = GianHangOrderDetail_cl.EnsureInvoiceDetailsForOrder(
            db,
            sellerAccount,
            resolvedOrderId,
            null);
        if (invoiceDetails.Count > 0)
        {
            GH_HoaDon_tb invoice = GianHangInvoice_cl.EnsureInvoiceByOrderKey(db, sellerAccount, resolvedOrderId);
            return GianHangOrderDetail_cl.BuildDiscountSummaryFromInvoiceSnapshot(
                db,
                sellerAccount,
                invoice,
                invoiceDetails);
        }

        OrderItemSnapshotContext context = BuildSnapshotContext(db, sellerAccount, resolvedOrderId, orderItems, true);

        foreach (DonHang_ChiTiet_tb item in orderItems)
        {
            GH_SanPham_tb nativeProduct = GianHangInvoiceLegacySnapshot_cl.ResolveNativeProduct(
                context == null ? null : context.NativeMap,
                item);
            int percent = ResolveOrderLineDiscountPercent(
                item,
                nativeProduct,
                ResolveInvoiceDetail(
                    item,
                    context == null ? null : context.NativeMap,
                    context == null ? null : context.InvoiceDetailMap),
                context == null ? null : context.LegacyMap);
            decimal lineTotal = item.thanhtien ?? 0m;
            if (lineTotal <= 0m || percent <= 0)
                continue;

            summary.DiscountAmountVnd += lineTotal * percent / 100m;
        }

        summary.DiscountRights = GianHangCheckoutCore_cl.ConvertVndToRights(summary.DiscountAmountVnd);
        summary.HasDiscount = summary.DiscountAmountVnd > 0m && summary.DiscountRights > 0m;
        return summary;
    }

    public static List<DonHang_ChiTiet_tb> LoadLegacyOrderItemsIfNeeded(
        dbDataContext db,
        string sellerAccount,
        List<GH_HoaDon_ChiTiet_tb> invoiceDetails,
        string orderId,
        GH_HoaDon_tb invoice)
    {
        if (invoiceDetails != null && invoiceDetails.Count > 0)
            return new List<DonHang_ChiTiet_tb>();

        if (db == null)
            return new List<DonHang_ChiTiet_tb>();

        string legacyOrderId = (orderId ?? string.Empty).Trim();
        if (legacyOrderId == string.Empty && invoice != null)
            legacyOrderId = (invoice.id_donhang ?? string.Empty).Trim();
        if (legacyOrderId == string.Empty)
            return new List<DonHang_ChiTiet_tb>();

        string runtimeSellerAccount = !string.IsNullOrWhiteSpace(sellerAccount)
            ? sellerAccount
            : (invoice == null ? string.Empty : (invoice.shop_taikhoan ?? string.Empty));
        GH_HoaDon_tb ensuredInvoice = invoice ?? GianHangInvoice_cl.EnsureInvoiceByOrderKey(db, runtimeSellerAccount, legacyOrderId);
        if (ensuredInvoice != null)
        {
            List<GH_HoaDon_ChiTiet_tb> ensuredDetails = GianHangInvoice_cl.GetDetails(db, ensuredInvoice);
            if (ensuredDetails.Count > 0)
                return new List<DonHang_ChiTiet_tb>();
        }

        return GianHangLegacyOrder_cl.LoadOrderDetails(db, legacyOrderId);
    }

    public static Dictionary<string, BaiViet_tb> LoadLegacyPostMapByIds(
        dbDataContext db,
        IEnumerable<string> rawIds)
    {
        if (rawIds == null)
            return new Dictionary<string, BaiViet_tb>(StringComparer.OrdinalIgnoreCase);

        List<string> referenceIds = rawIds
            .Select(p => (p ?? string.Empty).Trim())
            .Where(p => p != string.Empty)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        return GianHangLegacyPost_cl.LoadMapByReferenceIds(db, referenceIds);
    }

    public static void BackfillOrderItemSnapshots(
        dbDataContext db,
        IEnumerable<DonHang_ChiTiet_tb> orderItems,
        Dictionary<string, GH_SanPham_tb> nativeMap,
        Dictionary<int, GH_HoaDon_ChiTiet_tb> invoiceDetailMap,
        Dictionary<string, BaiViet_tb> legacyMap)
    {
        if (db == null || orderItems == null)
            return;

        bool changed = false;
        foreach (DonHang_ChiTiet_tb item in orderItems)
        {
            if (item == null)
                continue;

            string itemKey = (item.idsp ?? string.Empty).Trim();
            GH_SanPham_tb nativeProduct = null;
            if (nativeMap != null && itemKey != string.Empty)
                nativeMap.TryGetValue(itemKey, out nativeProduct);

            GH_HoaDon_ChiTiet_tb invoiceDetail = ResolveInvoiceDetail(item, nativeMap, invoiceDetailMap);

            BaiViet_tb legacyPost = null;
            if (legacyMap != null && itemKey != string.Empty)
                legacyMap.TryGetValue(itemKey, out legacyPost);

            if ((item.gh_product_id ?? 0) <= 0 && nativeProduct != null)
            {
                item.gh_product_id = nativeProduct.id;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(item.gh_name))
            {
                string resolvedName = !string.IsNullOrWhiteSpace(invoiceDetail == null ? string.Empty : invoiceDetail.ten_sanpham)
                    ? invoiceDetail.ten_sanpham
                    : (nativeProduct != null ? (nativeProduct.ten ?? string.Empty) : (legacyPost == null ? string.Empty : (legacyPost.name ?? string.Empty)));
                if (!string.IsNullOrWhiteSpace(resolvedName))
                {
                    item.gh_name = resolvedName.Trim();
                    changed = true;
                }
            }

            if (string.IsNullOrWhiteSpace(item.gh_image))
            {
                string resolvedImage = !string.IsNullOrWhiteSpace(invoiceDetail == null ? string.Empty : invoiceDetail.hinh_anh)
                    ? invoiceDetail.hinh_anh
                    : (nativeProduct != null ? (nativeProduct.hinh_anh ?? string.Empty) : (legacyPost == null ? string.Empty : (legacyPost.image ?? string.Empty)));
                if (!string.IsNullOrWhiteSpace(resolvedImage))
                {
                    item.gh_image = resolvedImage.Trim();
                    changed = true;
                }
            }

            if (string.IsNullOrWhiteSpace(item.gh_type))
            {
                string resolvedType = !string.IsNullOrWhiteSpace(invoiceDetail == null ? string.Empty : invoiceDetail.loai)
                    ? invoiceDetail.loai
                    : (nativeProduct != null ? GianHangProduct_cl.NormalizeLoai(nativeProduct.loai) : string.Empty);
                if (!string.IsNullOrWhiteSpace(resolvedType))
                {
                    item.gh_type = resolvedType.Trim();
                    changed = true;
                }
            }

            if (!item.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
            {
                int resolvedPercent = ResolveInvoiceDetailDiscountPercent(invoiceDetail);
                if (resolvedPercent <= 0)
                    resolvedPercent = GianHangProduct_cl.ResolveDiscountPercent(nativeProduct);
                if (resolvedPercent <= 0 && legacyPost != null)
                    resolvedPercent = ResolveOrderLineDiscountPercent(item, null, legacyPost);

                if (resolvedPercent > 0)
                {
                    item.PhanTram_GiamGia_ThanhToan_BangEvoucher = resolvedPercent;
                    changed = true;
                }
            }
        }

        if (changed)
            db.SubmitChanges();
    }

    public static int ResolveOrderLineDiscountPercent(DonHang_ChiTiet_tb orderItem, GH_SanPham_tb nativeProduct, BaiViet_tb legacyPost)
    {
        int percent = orderItem.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0;
        if (percent <= 0)
            percent = GianHangProduct_cl.ResolveDiscountPercent(nativeProduct);
        if (percent <= 0 && legacyPost != null)
            percent = legacyPost.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0;

        if (percent < 0) percent = 0;
        if (percent > 50) percent = 50;
        return percent;
    }

    public static int ResolveOrderLineDiscountPercent(
        DonHang_ChiTiet_tb orderItem,
        GH_SanPham_tb nativeProduct,
        GH_HoaDon_ChiTiet_tb invoiceDetail,
        Dictionary<string, BaiViet_tb> legacyMap)
    {
        if (orderItem == null)
            return 0;

        if (orderItem.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
        {
            int snapshotPercent = orderItem.PhanTram_GiamGia_ThanhToan_BangEvoucher.Value;
            if (snapshotPercent < 0) snapshotPercent = 0;
            if (snapshotPercent > 50) snapshotPercent = 50;
            return snapshotPercent;
        }

        int invoicePercent = ResolveInvoiceDetailDiscountPercent(invoiceDetail);
        if (invoicePercent > 0)
            return invoicePercent;

        int nativePercent = GianHangProduct_cl.ResolveDiscountPercent(nativeProduct);
        if (nativePercent > 0)
            return nativePercent;

        if (legacyMap == null || legacyMap.Count == 0)
            return 0;

        BaiViet_tb legacyPost;
        legacyMap.TryGetValue((orderItem.idsp ?? string.Empty).Trim(), out legacyPost);
        return ResolveOrderLineDiscountPercent(orderItem, nativeProduct, legacyPost);
    }

    public static GH_HoaDon_ChiTiet_tb ResolveInvoiceDetail(
        DonHang_ChiTiet_tb orderItem,
        Dictionary<string, GH_SanPham_tb> nativeMap,
        Dictionary<int, GH_HoaDon_ChiTiet_tb> invoiceDetailMap)
    {
        if (orderItem == null || invoiceDetailMap == null || invoiceDetailMap.Count == 0)
            return null;

        int nativeProductId = orderItem.gh_product_id ?? 0;
        if (nativeProductId <= 0 && nativeMap != null)
        {
            GH_SanPham_tb nativeProduct;
            if (nativeMap.TryGetValue((orderItem.idsp ?? string.Empty).Trim(), out nativeProduct) && nativeProduct != null)
                nativeProductId = nativeProduct.id;
        }

        if (nativeProductId <= 0)
            return null;

        GH_HoaDon_ChiTiet_tb detail;
        return invoiceDetailMap.TryGetValue(nativeProductId, out detail) ? detail : null;
    }

    public static List<string> CollectLegacyLookupIds(
        IEnumerable<DonHang_ChiTiet_tb> orderItems,
        Dictionary<string, GH_SanPham_tb> nativeMap,
        Dictionary<int, GH_HoaDon_ChiTiet_tb> invoiceDetailMap,
        bool includeLegacyForDiscount)
    {
        List<string> ids = new List<string>();
        HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (orderItems == null)
            return ids;

        foreach (DonHang_ChiTiet_tb item in orderItems)
        {
            if (item == null)
                continue;

            GH_SanPham_tb nativeProduct = null;
            if (nativeMap != null)
                nativeMap.TryGetValue((item.idsp ?? string.Empty).Trim(), out nativeProduct);

            GH_HoaDon_ChiTiet_tb invoiceDetail = ResolveInvoiceDetail(item, nativeMap, invoiceDetailMap);
            if (!RequiresLegacyPostLookup(item, nativeProduct, invoiceDetail, includeLegacyForDiscount))
                continue;

            string key = (item.idsp ?? string.Empty).Trim();
            if (key == string.Empty || !seen.Add(key))
                continue;

            ids.Add(key);
        }

        return ids;
    }

    public static string ResolveSnapshotName(DonHang_ChiTiet_tb orderItem)
    {
        return orderItem == null ? string.Empty : ((orderItem.gh_name ?? string.Empty).Trim());
    }

    public static string ResolveSnapshotImage(DonHang_ChiTiet_tb orderItem)
    {
        return orderItem == null ? string.Empty : ((orderItem.gh_image ?? string.Empty).Trim());
    }

    private static OrderItemSnapshotContext BuildSnapshotContext(
        dbDataContext db,
        string sellerAccount,
        string orderId,
        List<DonHang_ChiTiet_tb> orderItems,
        bool includeLegacyForDiscount)
    {
        OrderItemSnapshotContext context = new OrderItemSnapshotContext
        {
            NativeMap = GianHangInvoiceLegacySnapshot_cl.LoadNativeProductMapByOrderItems(db, sellerAccount, orderItems),
            InvoiceDetailMap = GianHangOrderDetail_cl.LoadInvoiceDetailMapByOrderId(db, sellerAccount, orderId),
            LegacyMap = new Dictionary<string, BaiViet_tb>(StringComparer.OrdinalIgnoreCase)
        };

        BackfillOrderItemSnapshots(db, orderItems, context.NativeMap, context.InvoiceDetailMap, null);

        List<string> legacyLookupIds = CollectLegacyLookupIds(orderItems, context.NativeMap, context.InvoiceDetailMap, includeLegacyForDiscount);
        if (legacyLookupIds.Count > 0)
            context.LegacyMap = LoadLegacyPostMapByIds(db, legacyLookupIds);

        if (context.LegacyMap.Count > 0)
            BackfillOrderItemSnapshots(db, orderItems, context.NativeMap, context.InvoiceDetailMap, context.LegacyMap);
        return context;
    }

    private static string ResolveOrderId(string orderId, List<DonHang_ChiTiet_tb> orderItems)
    {
        string resolved = (orderId ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(resolved))
            return resolved;

        if (orderItems == null || orderItems.Count == 0 || orderItems[0] == null)
            return string.Empty;

        return (orderItems[0].id_donhang ?? string.Empty).Trim();
    }

    private static string ResolveRowName(DonHang_ChiTiet_tb orderItem, GH_SanPham_tb nativeProduct, GH_HoaDon_ChiTiet_tb invoiceDetail)
    {
        string snapshotName = ResolveSnapshotName(orderItem);
        if (snapshotName != string.Empty)
            return snapshotName;
        if (invoiceDetail != null && !string.IsNullOrWhiteSpace(invoiceDetail.ten_sanpham))
            return invoiceDetail.ten_sanpham ?? string.Empty;
        return nativeProduct == null ? string.Empty : (nativeProduct.ten ?? string.Empty);
    }

    private static string ResolveRowImage(DonHang_ChiTiet_tb orderItem, GH_SanPham_tb nativeProduct, GH_HoaDon_ChiTiet_tb invoiceDetail)
    {
        string snapshotImage = ResolveSnapshotImage(orderItem);
        if (snapshotImage != string.Empty)
            return snapshotImage;
        if (invoiceDetail != null && !string.IsNullOrWhiteSpace(invoiceDetail.hinh_anh))
            return invoiceDetail.hinh_anh ?? string.Empty;
        return nativeProduct == null ? string.Empty : (nativeProduct.hinh_anh ?? string.Empty);
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

    private static bool HasGianHangSnapshot(DonHang_ChiTiet_tb orderItem)
    {
        if (orderItem == null)
            return false;
        if ((orderItem.gh_product_id ?? 0) > 0)
            return true;
        if (!string.IsNullOrWhiteSpace(orderItem.gh_name))
            return true;
        if (!string.IsNullOrWhiteSpace(orderItem.gh_image))
            return true;
        return !string.IsNullOrWhiteSpace(orderItem.gh_type);
    }

    private static bool HasInvoiceDetailSnapshot(GH_HoaDon_ChiTiet_tb invoiceDetail)
    {
        if (invoiceDetail == null)
            return false;
        if (!string.IsNullOrWhiteSpace(invoiceDetail.ten_sanpham))
            return true;
        if (!string.IsNullOrWhiteSpace(invoiceDetail.hinh_anh))
            return true;
        return !string.IsNullOrWhiteSpace(invoiceDetail.loai);
    }

    private static bool RequiresLegacyPostLookup(
        DonHang_ChiTiet_tb orderItem,
        GH_SanPham_tb nativeProduct,
        GH_HoaDon_ChiTiet_tb invoiceDetail,
        bool includeLegacyForDiscount)
    {
        if (orderItem == null)
            return false;
        if (!HasGianHangSnapshot(orderItem) && !HasInvoiceDetailSnapshot(invoiceDetail))
            return true;
        if (!includeLegacyForDiscount)
            return false;
        if (orderItem.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
            return false;
        if (invoiceDetail != null && invoiceDetail.phan_tram_uu_dai.HasValue)
            return false;
        return GianHangProduct_cl.ResolveDiscountPercent(nativeProduct) <= 0;
    }
}
