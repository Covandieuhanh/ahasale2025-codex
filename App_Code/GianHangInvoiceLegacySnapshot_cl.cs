using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangInvoiceLegacySnapshot_cl
{
    public static bool EnsureDetailSnapshots(
        dbDataContext db,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> details,
        string sellerAccount)
    {
        if (db == null || invoice == null || invoice.id <= 0)
            return false;

        List<GH_HoaDon_ChiTiet_tb> safeDetails = details ?? new List<GH_HoaDon_ChiTiet_tb>();
        if (HasCompleteDetailSnapshots(safeDetails))
            return false;

        string orderId = (invoice.id_donhang ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(orderId))
            return false;

        List<DonHang_ChiTiet_tb> orderItems = LoadLegacyOrderItemsByInvoice(db, invoice);
        if (orderItems.Count == 0)
            return false;

        return BackfillInvoiceDetailsFromLegacy(
            db,
            invoice,
            safeDetails,
            orderItems,
            (sellerAccount ?? string.Empty).Trim().ToLowerInvariant());
    }

    public static List<DonHang_ChiTiet_tb> LoadLegacyOrderItemsByInvoice(
        dbDataContext db,
        GH_HoaDon_tb invoice)
    {
        if (db == null || invoice == null)
            return new List<DonHang_ChiTiet_tb>();

        string orderId = (invoice.id_donhang ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(orderId))
            return new List<DonHang_ChiTiet_tb>();

        return GianHangLegacyOrder_cl.LoadOrderDetails(db, orderId);
    }

    public static bool BackfillInvoiceDetailsFromLegacy(
        dbDataContext db,
        GH_HoaDon_tb invoice,
        List<GH_HoaDon_ChiTiet_tb> details,
        List<DonHang_ChiTiet_tb> orderItems,
        string sellerAccount)
    {
        if (db == null || invoice == null || orderItems == null || orderItems.Count == 0)
            return false;

        string normalizedSeller = (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
        Dictionary<string, GH_SanPham_tb> nativeMap = LoadNativeProductMapByOrderItems(db, normalizedSeller, orderItems);
        Dictionary<int, GH_HoaDon_ChiTiet_tb> detailMapByProductId = (details ?? new List<GH_HoaDon_ChiTiet_tb>())
            .Where(p => p != null && p.id_sanpham.HasValue)
            .GroupBy(p => p.id_sanpham.Value)
            .ToDictionary(p => p.Key, p => p.OrderByDescending(x => x.id).First());

        bool changed = BackfillOrderItemSnapshotsFromSources(
            orderItems,
            nativeMap,
            detailMapByProductId,
            null);

        changed = BackfillDetailSnapshotsFromSources(
            db,
            invoice,
            orderItems,
            details ?? new List<GH_HoaDon_ChiTiet_tb>(),
            detailMapByProductId,
            nativeMap,
            null);
        if (HasCompleteDetailSnapshots(details))
            return changed;

        Dictionary<string, BaiViet_tb> legacyMap = LoadLegacyPostMapByOrderItems(db, orderItems, nativeMap, details, detailMapByProductId);
        if (legacyMap.Count > 0)
        {
            changed = BackfillOrderItemSnapshotsFromSources(
                orderItems,
                nativeMap,
                detailMapByProductId,
                legacyMap) || changed;

            changed = BackfillDetailSnapshotsFromSources(
                db,
                invoice,
                orderItems,
                details ?? new List<GH_HoaDon_ChiTiet_tb>(),
                detailMapByProductId,
                nativeMap,
                legacyMap) || changed;
        }

        return changed;
    }

    public static Dictionary<string, GH_SanPham_tb> LoadNativeProductMapByOrderItems(
        dbDataContext db,
        string sellerAccount,
        IEnumerable<DonHang_ChiTiet_tb> orderItems)
    {
        Dictionary<string, GH_SanPham_tb> map = new Dictionary<string, GH_SanPham_tb>(StringComparer.OrdinalIgnoreCase);
        if (db == null || orderItems == null || string.IsNullOrWhiteSpace(sellerAccount))
            return map;

        List<int> referenceIds = orderItems
            .Select(p =>
            {
                int parsed;
                return int.TryParse((p == null ? string.Empty : (p.idsp ?? string.Empty).Trim()), out parsed) ? parsed : 0;
            })
            .Where(p => p > 0)
            .Distinct()
            .ToList();
        List<int> nativeIds = orderItems
            .Where(p => p != null && (p.gh_product_id ?? 0) > 0)
            .Select(p => p.gh_product_id.Value)
            .Distinct()
            .ToList();
        if (referenceIds.Count == 0 && nativeIds.Count == 0)
            return map;

        foreach (GH_SanPham_tb product in GianHangProduct_cl.QueryByStorefront(db, sellerAccount)
            .Where(p => nativeIds.Contains(p.id)
                        || referenceIds.Contains(p.id)
                        || (p.id_baiviet.HasValue && referenceIds.Contains(p.id_baiviet.Value)))
            .ToList())
        {
            string nativeKey = product.id.ToString();
            if (!map.ContainsKey(nativeKey))
                map[nativeKey] = product;

            if (product.id_baiviet.HasValue)
            {
                string referenceKey = product.id_baiviet.Value.ToString();
                if (!map.ContainsKey(referenceKey))
                    map[referenceKey] = product;
            }
        }

        return map;
    }

    public static Dictionary<string, BaiViet_tb> LoadLegacyPostMapByOrderItems(
        dbDataContext db,
        IEnumerable<DonHang_ChiTiet_tb> orderItems,
        Dictionary<string, GH_SanPham_tb> nativeMap,
        List<GH_HoaDon_ChiTiet_tb> details,
        Dictionary<int, GH_HoaDon_ChiTiet_tb> detailMapByProductId)
    {
        List<string> referenceIds = (orderItems ?? Enumerable.Empty<DonHang_ChiTiet_tb>())
            .Where(p => RequiresLegacyPostLookup(p, nativeMap, details, detailMapByProductId))
            .Select(p => p == null ? string.Empty : (p.idsp ?? string.Empty).Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (referenceIds.Count == 0)
            return new Dictionary<string, BaiViet_tb>(StringComparer.OrdinalIgnoreCase);

        return GianHangLegacyPost_cl.LoadMapByReferenceIds(db, referenceIds);
    }

    public static bool RequiresLegacyPostLookup(
        DonHang_ChiTiet_tb item,
        Dictionary<string, GH_SanPham_tb> nativeMap,
        List<GH_HoaDon_ChiTiet_tb> details,
        Dictionary<int, GH_HoaDon_ChiTiet_tb> detailMapByProductId)
    {
        if (item == null)
            return false;

        bool hasSnapshot = HasOrderItemSnapshot(item);
        bool hasDiscountSnapshot = item.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue;
        GH_SanPham_tb nativeProduct = ResolveNativeProduct(nativeMap, item);
        GH_HoaDon_ChiTiet_tb detail = null;
        if (detailMapByProductId != null && detailMapByProductId.Count > 0)
        {
            int productId = item.gh_product_id ?? 0;
            if (productId <= 0 && nativeProduct != null)
                productId = nativeProduct.id;

            if (productId > 0)
                detailMapByProductId.TryGetValue(productId, out detail);
        }

        if (!NeedsLegacyPostForDetailBackfill(item, nativeProduct, detail))
            return false;

        if (nativeProduct == null)
            return !hasSnapshot || !hasDiscountSnapshot;

        return !hasDiscountSnapshot && GianHangProduct_cl.ResolveDiscountPercent(nativeProduct) <= 0;
    }

    public static GH_SanPham_tb ResolveNativeProduct(Dictionary<string, GH_SanPham_tb> nativeMap, DonHang_ChiTiet_tb item)
    {
        if (nativeMap == null || item == null)
            return null;

        GH_SanPham_tb nativeProduct;
        string nativeKey = ((item.gh_product_id ?? 0) > 0 ? item.gh_product_id.Value.ToString() : string.Empty);
        if (!string.IsNullOrWhiteSpace(nativeKey) && nativeMap.TryGetValue(nativeKey, out nativeProduct))
            return nativeProduct;

        string referenceKey = (item.idsp ?? string.Empty).Trim();
        return !string.IsNullOrWhiteSpace(referenceKey) && nativeMap.TryGetValue(referenceKey, out nativeProduct)
            ? nativeProduct
            : null;
    }

    public static BaiViet_tb ResolveLegacyPost(Dictionary<string, BaiViet_tb> legacyMap, DonHang_ChiTiet_tb item)
    {
        if (legacyMap == null || item == null)
            return null;

        BaiViet_tb legacyPost;
        string referenceKey = (item.idsp ?? string.Empty).Trim();
        return !string.IsNullOrWhiteSpace(referenceKey) && legacyMap.TryGetValue(referenceKey, out legacyPost)
            ? legacyPost
            : null;
    }

    public static bool HasOrderItemSnapshot(DonHang_ChiTiet_tb item)
    {
        if (item == null)
            return false;
        if ((item.gh_product_id ?? 0) > 0)
            return true;
        if (!string.IsNullOrWhiteSpace((item.gh_name ?? string.Empty).Trim()))
            return true;
        if (!string.IsNullOrWhiteSpace((item.gh_image ?? string.Empty).Trim()))
            return true;
        return !string.IsNullOrWhiteSpace((item.gh_type ?? string.Empty).Trim());
    }

    public static string ResolveDetailName(DonHang_ChiTiet_tb item, GH_SanPham_tb nativeProduct, BaiViet_tb legacyPost)
    {
        string snapshotName = item == null ? string.Empty : ((item.gh_name ?? string.Empty).Trim());
        if (!string.IsNullOrWhiteSpace(snapshotName))
            return snapshotName;
        if (nativeProduct != null && !string.IsNullOrWhiteSpace((nativeProduct.ten ?? string.Empty).Trim()))
            return nativeProduct.ten.Trim();
        return legacyPost == null ? string.Empty : ((legacyPost.name ?? string.Empty).Trim());
    }

    public static string ResolveDetailImage(DonHang_ChiTiet_tb item, GH_SanPham_tb nativeProduct, BaiViet_tb legacyPost)
    {
        string snapshotImage = item == null ? string.Empty : ((item.gh_image ?? string.Empty).Trim());
        if (!string.IsNullOrWhiteSpace(snapshotImage))
            return snapshotImage;
        if (nativeProduct != null && !string.IsNullOrWhiteSpace((nativeProduct.hinh_anh ?? string.Empty).Trim()))
            return nativeProduct.hinh_anh.Trim();
        return legacyPost == null ? string.Empty : ((legacyPost.image ?? string.Empty).Trim());
    }

    public static string ResolveDetailType(DonHang_ChiTiet_tb item, GH_SanPham_tb nativeProduct, BaiViet_tb legacyPost)
    {
        string snapshotType = item == null ? string.Empty : ((item.gh_type ?? string.Empty).Trim());
        if (!string.IsNullOrWhiteSpace(snapshotType))
            return snapshotType;
        if (nativeProduct != null)
            return GianHangProduct_cl.NormalizeLoai(nativeProduct.loai);
        if (legacyPost == null)
            return string.Empty;

        string legacyType = (legacyPost.phanloai ?? string.Empty).Trim().ToLowerInvariant();
        return legacyType == AccountVisibility_cl.PostTypeService
            ? GianHangProduct_cl.LoaiDichVu
            : GianHangProduct_cl.LoaiSanPham;
    }

    public static int ResolveDetailDiscountPercent(DonHang_ChiTiet_tb item, GH_SanPham_tb nativeProduct, BaiViet_tb legacyPost)
    {
        int percent = item == null ? 0 : (item.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0);
        if (percent <= 0)
            percent = GianHangProduct_cl.ResolveDiscountPercent(nativeProduct);
        if (percent <= 0 && legacyPost != null)
            percent = legacyPost.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0;
        if (percent < 0) percent = 0;
        if (percent > 50) percent = 50;
        return percent;
    }

    public static List<GianHangInvoice_cl.SnapshotLine> BuildSnapshotLinesFromLegacyOrder(
        dbDataContext db,
        string sellerAccount,
        List<DonHang_ChiTiet_tb> orderItems)
    {
        Dictionary<string, GH_SanPham_tb> nativeMap = LoadNativeProductMapByOrderItems(db, sellerAccount, orderItems);
        List<GianHangInvoice_cl.SnapshotLine> lines = new List<GianHangInvoice_cl.SnapshotLine>();
        List<DonHang_ChiTiet_tb> anchoredItems = new List<DonHang_ChiTiet_tb>();
        HashSet<string> legacyLookupKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        bool hasGianHangAnchor = false;

        for (int i = 0; i < orderItems.Count; i++)
        {
            DonHang_ChiTiet_tb item = orderItems[i];
            if (item == null)
                continue;

            GH_SanPham_tb nativeProduct = ResolveNativeProduct(nativeMap, item);
            bool hasNativeAnchor = nativeProduct != null || HasOrderItemSnapshot(item);
            hasGianHangAnchor = hasGianHangAnchor || hasNativeAnchor;
            if (!hasNativeAnchor)
                continue;

            int quantity = item.soluong.GetValueOrDefault() <= 0 ? 1 : item.soluong.GetValueOrDefault();
            decimal price = item.giaban ?? 0m;
            decimal lineTotal = item.thanhtien ?? (price * quantity);
            int productId = item.gh_product_id.GetValueOrDefault();
            if (productId <= 0 && nativeProduct != null)
                productId = nativeProduct.id;

            GianHangInvoice_cl.SnapshotLine line = new GianHangInvoice_cl.SnapshotLine
            {
                ProductId = productId,
                ProductName = ResolveDetailName(item, nativeProduct, null),
                ProductImage = ResolveDetailImage(item, nativeProduct, null),
                ProductType = ResolveDetailType(item, nativeProduct, null),
                Quantity = quantity,
                Price = price,
                LineTotal = lineTotal,
                DiscountPercent = ResolveDetailDiscountPercent(item, nativeProduct, null)
            };
            lines.Add(line);
            anchoredItems.Add(item);

            if (NeedsLegacyPostForDetailBackfill(item, nativeProduct, null)
                && RequiresLegacyPostLookup(item, nativeMap, null, null))
            {
                string key = (item.idsp ?? string.Empty).Trim();
                if (key != string.Empty)
                    legacyLookupKeys.Add(key);
            }
        }

        if (legacyLookupKeys.Count > 0 && lines.Count > 0)
        {
            Dictionary<string, BaiViet_tb> legacyMap = GianHangLegacyPost_cl.LoadMapByReferenceIds(db, legacyLookupKeys.ToList());
            if (legacyMap.Count > 0)
            {
                for (int i = 0; i < anchoredItems.Count && i < lines.Count; i++)
                {
                    DonHang_ChiTiet_tb item = anchoredItems[i];
                    GianHangInvoice_cl.SnapshotLine line = lines[i];
                    if (item == null || line == null)
                        continue;

                    string key = (item.idsp ?? string.Empty).Trim();
                    BaiViet_tb legacyPost;
                    if (key == string.Empty || !legacyMap.TryGetValue(key, out legacyPost) || legacyPost == null)
                        continue;

                    GH_SanPham_tb nativeProduct = ResolveNativeProduct(nativeMap, item);
                    if (string.IsNullOrWhiteSpace(line.ProductName))
                        line.ProductName = ResolveDetailName(item, nativeProduct, legacyPost);
                    if (string.IsNullOrWhiteSpace(line.ProductImage))
                        line.ProductImage = ResolveDetailImage(item, nativeProduct, legacyPost);
                    if (string.IsNullOrWhiteSpace(line.ProductType))
                        line.ProductType = ResolveDetailType(item, nativeProduct, legacyPost);
                    if (line.DiscountPercent <= 0)
                        line.DiscountPercent = ResolveDetailDiscountPercent(item, nativeProduct, legacyPost);
                }
            }
        }

        return hasGianHangAnchor ? lines : new List<GianHangInvoice_cl.SnapshotLine>();
    }

    public static bool NeedsLegacyPostForDetailBackfill(
        DonHang_ChiTiet_tb item,
        GH_SanPham_tb nativeProduct,
        GH_HoaDon_ChiTiet_tb detail)
    {
        if (item == null)
            return false;

        int detailDiscountPercent = 0;
        if (detail != null && detail.phan_tram_uu_dai.HasValue)
        {
            detailDiscountPercent = detail.phan_tram_uu_dai.Value;
            if (detailDiscountPercent < 0)
                detailDiscountPercent = 0;
            if (detailDiscountPercent > 50)
                detailDiscountPercent = 50;
        }

        if (!string.IsNullOrWhiteSpace((detail == null ? string.Empty : (detail.ten_sanpham ?? string.Empty)).Trim())
            && !string.IsNullOrWhiteSpace((detail == null ? string.Empty : (detail.hinh_anh ?? string.Empty)).Trim())
            && !string.IsNullOrWhiteSpace((detail == null ? string.Empty : (detail.loai ?? string.Empty)).Trim())
            && detailDiscountPercent > 0)
            return false;

        if (!string.IsNullOrWhiteSpace(ResolveDetailName(item, nativeProduct, null))
            && !string.IsNullOrWhiteSpace(ResolveDetailImage(item, nativeProduct, null))
            && !string.IsNullOrWhiteSpace(ResolveDetailType(item, nativeProduct, null))
            && ResolveDetailDiscountPercent(item, nativeProduct, null) > 0)
            return false;

        if (string.IsNullOrWhiteSpace(ResolveDetailName(item, nativeProduct, null)))
            return true;
        if (string.IsNullOrWhiteSpace(ResolveDetailImage(item, nativeProduct, null)))
            return true;
        if (string.IsNullOrWhiteSpace(ResolveDetailType(item, nativeProduct, null)))
            return true;

        return ResolveDetailDiscountPercent(item, nativeProduct, null) <= 0;
    }

    private static bool BackfillDetailSnapshotsFromSources(
        dbDataContext db,
        GH_HoaDon_tb invoice,
        List<DonHang_ChiTiet_tb> orderItems,
        List<GH_HoaDon_ChiTiet_tb> details,
        Dictionary<int, GH_HoaDon_ChiTiet_tb> detailMapByProductId,
        Dictionary<string, GH_SanPham_tb> nativeMap,
        Dictionary<string, BaiViet_tb> legacyMap)
    {
        bool changed = false;
        for (int i = 0; i < orderItems.Count; i++)
        {
            DonHang_ChiTiet_tb item = orderItems[i];
            if (item == null)
                continue;

            GH_HoaDon_ChiTiet_tb detail = null;
            GH_SanPham_tb nativeProduct = ResolveNativeProduct(nativeMap, item);
            if (nativeProduct != null)
                detailMapByProductId.TryGetValue(nativeProduct.id, out detail);
            if (detail == null && i < details.Count)
                detail = details[i];
            BaiViet_tb legacyPost = NeedsLegacyPostForDetailBackfill(item, nativeProduct, detail)
                ? ResolveLegacyPost(legacyMap, item)
                : null;
            if (detail == null)
            {
                detail = new GH_HoaDon_ChiTiet_tb
                {
                    id_hoadon = invoice.id
                };
                db.GetTable<GH_HoaDon_ChiTiet_tb>().InsertOnSubmit(detail);
                details.Add(detail);
                if (nativeProduct != null)
                    detailMapByProductId[nativeProduct.id] = detail;
                changed = true;
            }

            if (!detail.id_sanpham.HasValue && nativeProduct != null)
            {
                detail.id_sanpham = nativeProduct.id;
                changed = true;
            }

            string resolvedName = ResolveDetailName(item, nativeProduct, legacyPost);
            if (string.IsNullOrWhiteSpace((detail.ten_sanpham ?? string.Empty).Trim()) && !string.IsNullOrWhiteSpace(resolvedName))
            {
                detail.ten_sanpham = resolvedName;
                changed = true;
            }

            string resolvedImage = ResolveDetailImage(item, nativeProduct, legacyPost);
            if (string.IsNullOrWhiteSpace((detail.hinh_anh ?? string.Empty).Trim()) && !string.IsNullOrWhiteSpace(resolvedImage))
            {
                detail.hinh_anh = resolvedImage;
                changed = true;
            }

            string resolvedType = ResolveDetailType(item, nativeProduct, legacyPost);
            if (string.IsNullOrWhiteSpace((detail.loai ?? string.Empty).Trim()) && !string.IsNullOrWhiteSpace(resolvedType))
            {
                detail.loai = resolvedType;
                changed = true;
            }

            int quantity = item.soluong.GetValueOrDefault() <= 0 ? 1 : item.soluong.GetValueOrDefault();
            if (!detail.so_luong.HasValue || detail.so_luong.Value <= 0)
            {
                detail.so_luong = quantity;
                changed = true;
            }

            decimal price = item.giaban ?? 0m;
            if (!detail.gia.HasValue)
            {
                detail.gia = price;
                changed = true;
            }

            decimal lineTotal = item.thanhtien ?? (price * quantity);
            if (!detail.thanh_tien.HasValue)
            {
                detail.thanh_tien = lineTotal;
                changed = true;
            }

            int discountPercent = ResolveDetailDiscountPercent(item, nativeProduct, legacyPost);
            if (!detail.phan_tram_uu_dai.HasValue && discountPercent > 0)
            {
                detail.phan_tram_uu_dai = discountPercent;
                changed = true;
            }
        }

        return changed;
    }

    private static bool BackfillOrderItemSnapshotsFromSources(
        List<DonHang_ChiTiet_tb> orderItems,
        Dictionary<string, GH_SanPham_tb> nativeMap,
        Dictionary<int, GH_HoaDon_ChiTiet_tb> detailMapByProductId,
        Dictionary<string, BaiViet_tb> legacyMap)
    {
        if (orderItems == null || orderItems.Count == 0)
            return false;

        bool changed = false;
        for (int i = 0; i < orderItems.Count; i++)
        {
            DonHang_ChiTiet_tb item = orderItems[i];
            if (item == null)
                continue;

            GH_SanPham_tb nativeProduct = ResolveNativeProduct(nativeMap, item);
            GH_HoaDon_ChiTiet_tb detail = null;
            if (detailMapByProductId != null && nativeProduct != null)
                detailMapByProductId.TryGetValue(nativeProduct.id, out detail);

            BaiViet_tb legacyPost = NeedsLegacyPostForDetailBackfill(item, nativeProduct, detail)
                ? ResolveLegacyPost(legacyMap, item)
                : null;

            if ((item.gh_product_id ?? 0) <= 0 && nativeProduct != null)
            {
                item.gh_product_id = nativeProduct.id;
                changed = true;
            }

            string resolvedName = ResolveDetailName(item, nativeProduct, legacyPost);
            if (string.IsNullOrWhiteSpace((item.gh_name ?? string.Empty).Trim()) && !string.IsNullOrWhiteSpace(resolvedName))
            {
                item.gh_name = resolvedName.Trim();
                changed = true;
            }

            string resolvedImage = ResolveDetailImage(item, nativeProduct, legacyPost);
            if (string.IsNullOrWhiteSpace((item.gh_image ?? string.Empty).Trim()) && !string.IsNullOrWhiteSpace(resolvedImage))
            {
                item.gh_image = resolvedImage.Trim();
                changed = true;
            }

            string resolvedType = ResolveDetailType(item, nativeProduct, legacyPost);
            if (string.IsNullOrWhiteSpace((item.gh_type ?? string.Empty).Trim()) && !string.IsNullOrWhiteSpace(resolvedType))
            {
                item.gh_type = resolvedType.Trim();
                changed = true;
            }

            if (!item.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
            {
                int resolvedPercent = ResolveDetailDiscountPercent(item, nativeProduct, legacyPost);
                if (resolvedPercent > 0)
                {
                    item.PhanTram_GiamGia_ThanhToan_BangEvoucher = resolvedPercent;
                    changed = true;
                }
            }
        }

        return changed;
    }

    public static bool HasCompleteDetailSnapshots(List<GH_HoaDon_ChiTiet_tb> details)
    {
        if (details == null || details.Count == 0)
            return false;

        for (int i = 0; i < details.Count; i++)
        {
            if (!HasCompleteDetailSnapshot(details[i]))
                return false;
        }

        return true;
    }

    public static bool HasCompleteDetailSnapshot(GH_HoaDon_ChiTiet_tb detail)
    {
        if (detail == null)
            return false;

        return !string.IsNullOrWhiteSpace((detail.ten_sanpham ?? string.Empty).Trim())
            && !string.IsNullOrWhiteSpace((detail.hinh_anh ?? string.Empty).Trim())
            && !string.IsNullOrWhiteSpace((detail.loai ?? string.Empty).Trim())
            && detail.so_luong.HasValue
            && detail.gia.HasValue
            && detail.thanh_tien.HasValue;
    }
}
