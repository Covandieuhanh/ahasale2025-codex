using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangInvoice_cl
{
    public sealed class SnapshotLine
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        public int DiscountPercent { get; set; }
    }

    public const string TrangThaiMoi = "Chờ thanh toán";
    public const string TrangThaiDaThu = "Đã thanh toán";
    public const string TrangThaiHuy = "Đã hủy";

    public static void EnsureSchema(dbDataContext db)
    {
        GianHangSchema_cl.EnsureSchemaSafe(db);
    }

    public static IQueryable<GH_HoaDon_tb> QueryByStorefront(dbDataContext db, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        return db.GetTable<GH_HoaDon_tb>().Where(p => p.shop_taikhoan == tk);
    }

    public static IQueryable<GH_HoaDon_tb> QueryByBuyer(dbDataContext db, string buyerAccount)
    {
        EnsureSchema(db);
        string tk = (buyerAccount ?? string.Empty).Trim().ToLowerInvariant();
        return db.GetTable<GH_HoaDon_tb>().Where(p => p.buyer_account == tk);
    }

    public static int EnsureStorefrontInvoiceSnapshots(dbDataContext db, string shopTaiKhoan, int snapshotTake)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return 0;

        return GianHangInvoiceLegacyRuntime_cl.EnsureSnapshotsForSellerLegacyOrders(
            db,
            tk,
            snapshotTake <= 0 ? 500 : snapshotTake);
    }

    public static int EnsureBuyerInvoiceSnapshots(dbDataContext db, string buyerAccount, int snapshotTake)
    {
        EnsureSchema(db);
        string tk = (buyerAccount ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return 0;

        return GianHangInvoiceLegacyRuntime_cl.EnsureSnapshotsForBuyerLegacyOrders(
            db,
            tk,
            snapshotTake <= 0 ? 500 : snapshotTake);
    }

    public static void PrepareInvoicesWithRuntime(dbDataContext db, IEnumerable<GH_HoaDon_tb> invoices)
    {
        if (db == null || invoices == null)
            return;

        List<GH_HoaDon_tb> safeInvoices = invoices
            .Where(p => p != null)
            .ToList();
        if (safeInvoices.Count == 0)
            return;

        GianHangInvoiceLegacyRuntime_cl.BackfillLegacyRuntimeFields(db, safeInvoices);
        EnsureDetailSnapshots(db, safeInvoices);
    }

    public static List<GH_HoaDon_tb> LoadStorefrontInvoicesWithRuntime(
        dbDataContext db,
        string shopTaiKhoan,
        int snapshotTake,
        int invoiceTake)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return new List<GH_HoaDon_tb>();

        EnsureStorefrontInvoiceSnapshots(db, tk, snapshotTake);
        List<GH_HoaDon_tb> invoices = QueryByStorefront(db, tk)
            .OrderByDescending(p => p.id)
            .Take(invoiceTake <= 0 ? 500 : invoiceTake)
            .ToList();
        PrepareInvoicesWithRuntime(db, invoices);
        return invoices;
    }

    public static List<GH_HoaDon_tb> LoadBuyerInvoicesWithRuntime(
        dbDataContext db,
        string buyerAccount,
        int snapshotTake,
        int invoiceTake)
    {
        EnsureSchema(db);
        string tk = (buyerAccount ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return new List<GH_HoaDon_tb>();

        EnsureBuyerInvoiceSnapshots(db, tk, snapshotTake);
        List<GH_HoaDon_tb> invoices = QueryByBuyer(db, tk)
            .OrderByDescending(p => p.id)
            .Take(invoiceTake <= 0 ? 500 : invoiceTake)
            .ToList();
        PrepareInvoicesWithRuntime(db, invoices);
        return invoices;
    }

    public static Dictionary<int, int> LoadSoldTotalsByProductIds(
        dbDataContext db,
        string shopTaiKhoan,
        IEnumerable<int> productIds,
        int snapshotTake)
    {
        EnsureSchema(db);

        Dictionary<int, int> map = new Dictionary<int, int>();
        List<int> ids = (productIds ?? Enumerable.Empty<int>())
            .Where(p => p > 0)
            .Distinct()
            .ToList();
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || tk == string.Empty || ids.Count == 0)
            return map;

        EnsureStorefrontInvoiceSnapshots(db, tk, snapshotTake <= 0 ? 1000 : snapshotTake);
        List<GH_HoaDon_tb> invoices = QueryByStorefront(db, tk)
            .Where(p => (p.order_status ?? string.Empty).Trim() != DonHangStateMachine_cl.Order_DaHuy)
            .ToList();
        if (invoices.Count == 0)
            return map;

        PrepareInvoicesWithRuntime(db, invoices);

        List<long> invoiceIds = invoices.Select(p => p.id).Distinct().ToList();
        if (invoiceIds.Count == 0)
            return map;

        return db.GetTable<GH_HoaDon_ChiTiet_tb>()
            .Where(p => invoiceIds.Contains(p.id_hoadon)
                        && p.id_sanpham.HasValue
                        && ids.Contains(p.id_sanpham.Value))
            .GroupBy(p => p.id_sanpham.Value)
            .Select(g => new { id = g.Key, total = g.Sum(x => (int?)x.so_luong) ?? 0 })
            .ToList()
            .ToDictionary(p => p.id, p => p.total);
    }

    public static Dictionary<int, int> LoadSoldTotalsByProductMap(
        dbDataContext db,
        string shopTaiKhoan,
        IDictionary<int, string> productReferenceMap,
        int snapshotTake)
    {
        Dictionary<int, int> map = new Dictionary<int, int>();
        if (productReferenceMap == null || productReferenceMap.Count == 0)
            return map;

        map = LoadSoldTotalsByProductIds(db, shopTaiKhoan, productReferenceMap.Keys, snapshotTake);

        List<string> unresolvedLegacyIds = new List<string>();
        foreach (KeyValuePair<int, string> pair in productReferenceMap)
        {
            string legacyReferenceId = (pair.Value ?? string.Empty).Trim();
            if (legacyReferenceId == string.Empty)
                continue;

            int sold;
            if (!map.TryGetValue(pair.Key, out sold) || sold <= 0)
                unresolvedLegacyIds.Add(legacyReferenceId);
        }

        if (unresolvedLegacyIds.Count == 0)
            return map;

        Dictionary<string, int> legacyMap = GianHangInvoiceLegacyRuntime_cl.LoadLegacySoldTotalsByReferences(db, shopTaiKhoan, unresolvedLegacyIds);
        foreach (KeyValuePair<int, string> pair in productReferenceMap)
        {
            string legacyReferenceId = (pair.Value ?? string.Empty).Trim();
            if (legacyReferenceId == string.Empty)
                continue;

            int sold;
            if ((!map.TryGetValue(pair.Key, out sold) || sold <= 0) && legacyMap.TryGetValue(legacyReferenceId, out sold))
                map[pair.Key] = sold;
        }

        return map;
    }

    public static int CountWaitingExchangeByStorefront(dbDataContext db, string shopTaiKhoan, int snapshotTake, int invoiceTake)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || tk == string.Empty)
            return 0;

        List<GH_HoaDon_tb> invoices = LoadStorefrontInvoicesWithRuntime(
            db,
            tk,
            snapshotTake <= 0 ? 500 : snapshotTake,
            invoiceTake <= 0 ? 500 : invoiceTake);
        if (invoices.Count == 0)
            return GianHangInvoiceLegacyRuntime_cl.CountPendingExchangeBySeller(db, tk);

        int count = 0;
        for (int i = 0; i < invoices.Count; i++)
        {
            if (ResolveOrderStatusGroup(invoices[i], null) == "cho-trao-doi")
                count++;
        }

        return count;
    }

    public static GH_HoaDon_tb FindByPublicKeyOrId(dbDataContext db, string rawValue)
    {
        EnsureSchema(db);
        string value = (rawValue ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return null;

        Guid guide;
        if (Guid.TryParse(value, out guide))
        {
            GH_HoaDon_tb byGuide = db.GetTable<GH_HoaDon_tb>().FirstOrDefault(p => p.id_guide == guide);
            if (byGuide != null)
                return byGuide;
        }

        long id;
        if (long.TryParse(value, out id))
        {
            return db.GetTable<GH_HoaDon_tb>().FirstOrDefault(p => p.id == id || p.id_donhang == value);
        }

        return db.GetTable<GH_HoaDon_tb>().FirstOrDefault(p => p.id_donhang == value);
    }

    public static GH_HoaDon_tb FindByShopAndOrderId(dbDataContext db, string shopTaiKhoan, string orderId)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        string id = (orderId ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(tk) || string.IsNullOrWhiteSpace(id))
            return null;

        return db.GetTable<GH_HoaDon_tb>()
            .Where(p => p.shop_taikhoan == tk
                        && p.id_donhang == id)
            .OrderByDescending(p => p.id)
            .FirstOrDefault();
    }

    public static GH_HoaDon_tb FindLatestWaitingExchange(dbDataContext db, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return null;

        return db.GetTable<GH_HoaDon_tb>()
            .Where(p => p.shop_taikhoan == tk)
            .OrderByDescending(p => p.id)
            .Take(500)
            .ToList()
            .FirstOrDefault(p =>
                DonHangStateMachine_cl.NormalizeExchangeStatus(p.exchange_status)
                == DonHangStateMachine_cl.Exchange_ChoTraoDoi);
    }

    public static GH_HoaDon_tb EnsureInvoiceByOrderKey(dbDataContext db, string shopTaiKhoan, string rawOrderKey)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        string orderKey = (rawOrderKey ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(tk) || string.IsNullOrWhiteSpace(orderKey))
            return null;

        GH_HoaDon_tb invoice = ResolveInvoiceByOrderKey(db, tk, orderKey);
        if (invoice == null)
        {
            DonHang_tb legacyOrder = GianHangInvoiceLegacyRuntime_cl.FindLegacyOrderByOrderKey(db, tk, orderKey);
            if (legacyOrder != null)
                invoice = GianHangInvoiceLegacyRuntime_cl.EnsureSnapshotForLegacyOrder(db, legacyOrder);
        }

        if (invoice != null)
        {
            GianHangInvoiceLegacyRuntime_cl.BackfillLegacyRuntimeFields(db, invoice);
            EnsureDetailSnapshots(db, invoice);
        }

        return invoice;
    }

    public static bool NeedsLegacyRuntimeForCommand(GH_HoaDon_tb invoice)
    {
        return invoice == null;
    }

    public static DonHang_tb ResolveLegacyOrderForRuntime(
        dbDataContext db,
        string shopTaiKhoan,
        GH_HoaDon_tb invoice,
        string rawOrderKey)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        string orderKey = (rawOrderKey ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return null;

        DonHang_tb order = null;
        if (invoice != null)
            order = GianHangInvoiceLegacyRuntime_cl.FindLegacyOrderByInvoice(db, invoice);

        if (order == null && !string.IsNullOrWhiteSpace(orderKey))
            order = GianHangInvoiceLegacyRuntime_cl.FindLegacyOrderByOrderKey(db, tk, orderKey);

        return order;
    }

    public static GH_HoaDon_tb FindLatestWaitingExchangeWithSnapshot(dbDataContext db, string shopTaiKhoan, int snapshotTake)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return null;

        EnsureStorefrontInvoiceSnapshots(db, tk, snapshotTake <= 0 ? 500 : snapshotTake);
        GH_HoaDon_tb invoice = FindLatestWaitingExchange(db, tk);
        if (invoice != null)
        {
            PrepareInvoicesWithRuntime(db, new[] { invoice });
            return invoice;
        }

        DonHang_tb legacyOrder = GianHangInvoiceLegacyRuntime_cl.FindLatestWaitingExchange(db, tk);
        if (legacyOrder == null)
            return null;

        return GianHangInvoiceLegacyRuntime_cl.EnsureSnapshotForLegacyOrder(db, legacyOrder);
    }

    public static GH_HoaDon_tb FindAnotherWaitingExchangeWithSnapshot(dbDataContext db, string shopTaiKhoan, string excludedOrderId, int snapshotTake)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        string excluded = (excludedOrderId ?? string.Empty).Trim();
        long excludedId;
        bool hasExcludedId = long.TryParse(excluded, out excludedId) && excludedId > 0;
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return null;

        EnsureStorefrontInvoiceSnapshots(db, tk, snapshotTake <= 0 ? 500 : snapshotTake);
        List<GH_HoaDon_tb> invoices = QueryByStorefront(db, tk)
            .OrderByDescending(p => p.id)
            .Take(snapshotTake <= 0 ? 500 : snapshotTake)
            .ToList()
            .Where(p => DonHangStateMachine_cl.NormalizeExchangeStatus(p.exchange_status)
                        == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
            .ToList();
        PrepareInvoicesWithRuntime(db, invoices);

        for (int i = 0; i < invoices.Count; i++)
        {
            GH_HoaDon_tb invoice = invoices[i];
            string invoiceOrderKey = ResolveInvoiceOrderKey(invoice);
            if (string.Equals(invoiceOrderKey, excluded, StringComparison.OrdinalIgnoreCase))
                continue;

            return invoice;
        }

        DonHang_tb legacyOrder = hasExcludedId
            ? GianHangInvoiceLegacyRuntime_cl.FindAnotherWaitingExchange(db, tk, excludedId)
            : GianHangInvoiceLegacyRuntime_cl.FindLatestWaitingExchange(db, tk);
        if (legacyOrder == null)
            return null;

        return GianHangInvoiceLegacyRuntime_cl.EnsureSnapshotForLegacyOrder(db, legacyOrder);
    }

    public static List<GH_HoaDon_ChiTiet_tb> GetDetails(dbDataContext db, long idHoaDon)
    {
        EnsureSchema(db);
        GH_HoaDon_tb invoice = db.GetTable<GH_HoaDon_tb>().FirstOrDefault(p => p.id == idHoaDon);
        EnsureDetailSnapshots(db, invoice);
        return QueryDetails(db, idHoaDon);
    }

    public static List<GH_HoaDon_ChiTiet_tb> GetDetails(dbDataContext db, GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return new List<GH_HoaDon_ChiTiet_tb>();

        EnsureDetailSnapshots(db, invoice);
        return QueryDetails(db, invoice.id);
    }

    public static List<GH_HoaDon_ChiTiet_tb> GetDetailsByOrderId(dbDataContext db, string shopTaiKhoan, string orderId)
    {
        GH_HoaDon_tb invoice = FindByShopAndOrderId(db, shopTaiKhoan, orderId);
        if (invoice == null)
            return new List<GH_HoaDon_ChiTiet_tb>();

        EnsureDetailSnapshots(db, invoice);
        return QueryDetails(db, invoice.id);
    }

    public static List<GH_HoaDon_ChiTiet_tb> GetDetailsByOrderKey(dbDataContext db, string shopTaiKhoan, string orderKey)
    {
        GH_HoaDon_tb invoice = EnsureInvoiceByOrderKey(db, shopTaiKhoan, orderKey);
        if (invoice == null)
            return new List<GH_HoaDon_ChiTiet_tb>();

        return GetDetails(db, invoice);
    }

    public static void EnsureDetailSnapshots(dbDataContext db, GH_HoaDon_tb invoice)
    {
        if (db == null || invoice == null || invoice.id <= 0)
            return;

        List<GH_HoaDon_ChiTiet_tb> details = QueryDetails(db, invoice.id);
        bool changed = GianHangInvoiceLegacySnapshot_cl.EnsureDetailSnapshots(
            db,
            invoice,
            details,
            (invoice.shop_taikhoan ?? string.Empty).Trim());

        if (changed)
            db.SubmitChanges();
    }

    public static int EnsureDetailSnapshots(dbDataContext db, IEnumerable<GH_HoaDon_tb> invoices)
    {
        if (db == null || invoices == null)
            return 0;

        int touched = 0;
        HashSet<long> seen = new HashSet<long>();
        foreach (GH_HoaDon_tb invoice in invoices)
        {
            if (invoice == null || invoice.id <= 0 || !seen.Add(invoice.id))
                continue;

            EnsureDetailSnapshots(db, invoice);
            touched++;
        }

        return touched;
    }

    public static GH_HoaDon_tb CreateInvoice(dbDataContext db,
        string shopTaiKhoan,
        string tenKhach,
        string sdt,
        string diaChi,
        string ghiChu,
        int idSanPham,
        int soLuong)
    {
        string error;
        var items = new List<GianHangCartItem>
        {
            new GianHangCartItem { Id = idSanPham, SoLuong = soLuong }
        };

        return CreateInvoiceFromCart(db, shopTaiKhoan, tenKhach, sdt, diaChi, ghiChu, items, "", out error);
    }

    public static GH_HoaDon_tb CreateInvoiceFromCart(dbDataContext db,
        string shopTaiKhoan,
        string tenKhach,
        string sdt,
        string diaChi,
        string ghiChu,
        IEnumerable<GianHangCartItem> items,
        string buyerAccount,
        out string error)
    {
        error = "";
        EnsureSchema(db);

        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
        {
            error = "Không xác định gian hàng.";
            return null;
        }

        var list = (items ?? new List<GianHangCartItem>()).Where(p => p != null).ToList();
        if (list.Count == 0)
        {
            error = "Giỏ hàng trống.";
            return null;
        }

        var ids = list.Select(p => p.Id).Distinct().ToList();
        var products = db.GetTable<GH_SanPham_tb>()
            .Where(p => ids.Contains(p.id) && p.shop_taikhoan == tk && (p.bin == null || p.bin == false))
            .ToList();

        if (products.Count != ids.Count)
        {
            error = "Sản phẩm không tồn tại.";
            return null;
        }

        decimal total = 0m;
        var productLookup = products.ToDictionary(p => p.id, p => p);
        var lineItems = new List<GH_HoaDon_ChiTiet_tb>();

        foreach (var item in list)
        {
            if (!productLookup.ContainsKey(item.Id))
            {
                error = "Sản phẩm không tồn tại.";
                return null;
            }

            var sp = productLookup[item.Id];
            string loai = GianHangProduct_cl.NormalizeLoai(sp.loai);
            if (loai == GianHangProduct_cl.LoaiDichVu)
            {
                error = "Dịch vụ không thể đặt qua giỏ hàng.";
                return null;
            }

            int qty = item.SoLuong <= 0 ? 1 : item.SoLuong;
            if (qty > 9999) qty = 9999;

            int ton = sp.so_luong_ton ?? 0;
            if (ton > 0 && qty > ton)
            {
                error = "Số lượng vượt tồn kho.";
                return null;
            }

            decimal gia = sp.gia_ban ?? 0m;
            decimal thanhTien = gia * qty;
            total += thanhTien;

            lineItems.Add(new GH_HoaDon_ChiTiet_tb
            {
                id_sanpham = sp.id,
                ten_sanpham = sp.ten,
                hinh_anh = sp.hinh_anh,
                loai = loai,
                so_luong = qty,
                gia = gia,
                thanh_tien = thanhTien,
                phan_tram_uu_dai = GianHangProduct_cl.ResolveDiscountPercent(sp)
            });
        }

        GH_HoaDon_tb hoaDon = new GH_HoaDon_tb
        {
            id_guide = Guid.NewGuid(),
            shop_taikhoan = tk,
            id_donhang = string.Empty,
            buyer_account = (buyerAccount ?? string.Empty).Trim().ToLowerInvariant(),
            ten_khach = (tenKhach ?? "").Trim(),
            sdt = (sdt ?? "").Trim(),
            dia_chi = diaChi ?? "",
            ghi_chu = ghiChu ?? "",
            tong_tien = total,
            trang_thai = TrangThaiMoi,
            order_status = DonHangStateMachine_cl.Order_DaDat,
            exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi,
            is_offline = false,
            ngay_tao = AhaTime_cl.Now
        };

        db.GetTable<GH_HoaDon_tb>().InsertOnSubmit(hoaDon);
        db.SubmitChanges();
        EnsureStableOrderId(db, hoaDon);

        foreach (var line in lineItems)
        {
            line.id_hoadon = hoaDon.id;
            db.GetTable<GH_HoaDon_ChiTiet_tb>().InsertOnSubmit(line);

            var sp = productLookup[line.id_sanpham ?? 0];
            int ton = sp.so_luong_ton ?? 0;
            if (ton > 0)
                sp.so_luong_ton = ton - (line.so_luong ?? 0);
            sp.so_luong_da_ban = (sp.so_luong_da_ban ?? 0) + (line.so_luong ?? 0);
            sp.ngay_cap_nhat = AhaTime_cl.Now;
        }

        db.SubmitChanges();

        foreach (var sp in productLookup.Values)
            GianHangProduct_cl.TrySyncToHome(db, sp);

        return hoaDon;
    }

    public static GH_HoaDon_tb CreateSnapshotFromOrder(
        dbDataContext db,
        string shopTaiKhoan,
        string idDonHang,
        string buyerAccount,
        string tenKhach,
        string sdt,
        string diaChi,
        string ghiChu,
        bool isOffline,
        IEnumerable<SnapshotLine> lines)
    {
        EnsureSchema(db);

        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(tk))
            return null;

        List<SnapshotLine> safeLines = (lines ?? Enumerable.Empty<SnapshotLine>())
            .Where(p => p != null && p.Quantity > 0)
            .ToList();
        if (safeLines.Count == 0)
            return null;

        decimal total = 0m;
        for (int i = 0; i < safeLines.Count; i++)
            total += safeLines[i].LineTotal;

        GH_HoaDon_tb hoaDon = new GH_HoaDon_tb
        {
            id_guide = Guid.NewGuid(),
            shop_taikhoan = tk,
            id_donhang = (idDonHang ?? string.Empty).Trim(),
            buyer_account = (buyerAccount ?? string.Empty).Trim().ToLowerInvariant(),
            ten_khach = (tenKhach ?? string.Empty).Trim(),
            sdt = (sdt ?? string.Empty).Trim(),
            dia_chi = (diaChi ?? string.Empty).Trim(),
            ghi_chu = (ghiChu ?? string.Empty).Trim(),
            tong_tien = total,
            trang_thai = TrangThaiMoi,
            order_status = DonHangStateMachine_cl.Order_DaDat,
            exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi,
            is_offline = isOffline,
            ngay_tao = AhaTime_cl.Now
        };

        db.GetTable<GH_HoaDon_tb>().InsertOnSubmit(hoaDon);
        db.SubmitChanges();
        EnsureStableOrderId(db, hoaDon);

        for (int i = 0; i < safeLines.Count; i++)
        {
            SnapshotLine line = safeLines[i];
            GH_HoaDon_ChiTiet_tb detail = new GH_HoaDon_ChiTiet_tb
            {
                id_hoadon = hoaDon.id,
                id_sanpham = line.ProductId,
                ten_sanpham = line.ProductName,
                hinh_anh = (line.ProductImage ?? string.Empty).Trim(),
                loai = (line.ProductType ?? string.Empty).Trim(),
                so_luong = line.Quantity,
                gia = line.Price,
                thanh_tien = line.LineTotal,
                phan_tram_uu_dai = line.DiscountPercent < 0 ? 0 : (line.DiscountPercent > 50 ? 50 : line.DiscountPercent)
            };
            db.GetTable<GH_HoaDon_ChiTiet_tb>().InsertOnSubmit(detail);
        }

        db.SubmitChanges();
        return hoaDon;
    }

    private static void EnsureStableOrderId(dbDataContext db, GH_HoaDon_tb invoice)
    {
        if (db == null || invoice == null || invoice.id <= 0)
            return;

        string orderId = (invoice.id_donhang ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(orderId))
            return;

        invoice.id_donhang = invoice.id.ToString();
        db.SubmitChanges();
    }

    public static bool UpdateStatus(dbDataContext db, long idHoaDon, string shopTaiKhoan, string trangThai)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        var hd = db.GetTable<GH_HoaDon_tb>().FirstOrDefault(p => p.id == idHoaDon && p.shop_taikhoan == tk);
        if (hd == null)
            return false;

        hd.trang_thai = trangThai;
        db.SubmitChanges();
        return true;
    }

    public static bool SyncOrderLifecycleByOrderId(dbDataContext db, string shopTaiKhoan, string orderId, string orderStatus, string exchangeStatus, string invoiceStatus)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? string.Empty).Trim().ToLowerInvariant();
        string id = (orderId ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(tk) || string.IsNullOrWhiteSpace(id))
            return false;

        GH_HoaDon_tb invoice = db.GetTable<GH_HoaDon_tb>()
            .Where(p => p.shop_taikhoan == tk && p.id_donhang == id)
            .OrderByDescending(p => p.id)
            .FirstOrDefault();
        if (invoice == null)
            return false;

        if (!string.IsNullOrWhiteSpace(orderStatus))
            invoice.order_status = orderStatus.Trim();
        if (!string.IsNullOrWhiteSpace(exchangeStatus))
            invoice.exchange_status = exchangeStatus.Trim();
        if (!string.IsNullOrWhiteSpace(invoiceStatus))
            invoice.trang_thai = invoiceStatus.Trim();

        db.SubmitChanges();
        return true;
    }

    public static string ResolveOrderStatusGroup(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        string orderStatus = (invoice == null ? string.Empty : (invoice.order_status ?? string.Empty)).Trim();
        string exchangeStatus = (invoice == null ? string.Empty : (invoice.exchange_status ?? string.Empty)).Trim();

        if (string.IsNullOrWhiteSpace(orderStatus) && linkedOrder != null)
            orderStatus = DonHangStateMachine_cl.GetOrderStatus(linkedOrder);
        if (string.IsNullOrWhiteSpace(exchangeStatus) && linkedOrder != null)
            exchangeStatus = DonHangStateMachine_cl.GetExchangeStatus(linkedOrder);

        string normalizedOrderStatus = DonHangStateMachine_cl.NormalizeOrderStatus(orderStatus);
        string normalizedExchangeStatus = DonHangStateMachine_cl.NormalizeExchangeStatus(exchangeStatus);

        if (normalizedOrderStatus == DonHangStateMachine_cl.Order_DaHuy)
            return "da-huy";
        if (normalizedExchangeStatus == DonHangStateMachine_cl.Exchange_DaTraoDoi)
            return "da-trao-doi";
        if (normalizedExchangeStatus == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
            return "cho-trao-doi";
        if (normalizedOrderStatus == DonHangStateMachine_cl.Order_DaGiao
            || normalizedOrderStatus == DonHangStateMachine_cl.Order_DaNhan)
            return "da-giao";
        return "da-dat";
    }

    public static string ResolveOrderStatusGroup(GH_HoaDon_tb invoice)
    {
        return ResolveOrderStatusGroup(invoice, null);
    }

    public static string ResolveOrderStatusText(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        switch (ResolveOrderStatusGroup(invoice, linkedOrder))
        {
            case "cho-trao-doi":
                return "Chờ Trao đổi";
            case "da-trao-doi":
                return "Đã Trao đổi";
            case "da-giao":
                return "Đã giao";
            case "da-huy":
                return "Đã hủy";
            default:
                return "Đã đặt";
        }
    }

    public static string ResolveOrderStatusText(GH_HoaDon_tb invoice)
    {
        return ResolveOrderStatusText(invoice, null);
    }

    public static string ResolveOrderStatusCss(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        switch (ResolveOrderStatusGroup(invoice, linkedOrder))
        {
            case "cho-trao-doi":
                return "gh-report-status gh-report-status-warning";
            case "da-trao-doi":
                return "gh-report-status gh-report-status-info";
            case "da-giao":
                return "gh-report-status gh-report-status-success";
            case "da-huy":
                return "gh-report-status gh-report-status-danger";
            default:
                return "gh-report-status gh-report-status-neutral";
        }
    }

    public static string ResolveOrderStatusCss(GH_HoaDon_tb invoice)
    {
        return ResolveOrderStatusCss(invoice, null);
    }

    public static string ResolveOrderPublicId(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        string orderId = (invoice == null ? string.Empty : (invoice.id_donhang ?? string.Empty)).Trim();
        if (!string.IsNullOrWhiteSpace(orderId))
            return orderId;

        if (linkedOrder != null && linkedOrder.id > 0)
            return linkedOrder.id.ToString();

        return invoice == null ? string.Empty : invoice.id.ToString();
    }

    public static string ResolveOrderPublicId(GH_HoaDon_tb invoice)
    {
        return ResolveOrderPublicId(invoice, null);
    }

    public static string ResolveRuntimeOrderKey(GH_HoaDon_tb invoice)
    {
        return ResolveInvoiceOrderKey(invoice);
    }

    public static bool ResolveIsOffline(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (invoice != null && invoice.is_offline.HasValue)
            return invoice.is_offline.Value;

        return linkedOrder != null && linkedOrder.online_offline.HasValue && linkedOrder.online_offline.Value == false;
    }

    public static bool ResolveIsOffline(GH_HoaDon_tb invoice)
    {
        return ResolveIsOffline(invoice, null);
    }

    public static decimal ResolveTotalAmount(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (invoice != null && invoice.tong_tien.HasValue)
            return invoice.tong_tien.Value;

        return linkedOrder == null ? 0m : (linkedOrder.tongtien ?? 0m);
    }

    public static decimal ResolveTotalAmount(GH_HoaDon_tb invoice)
    {
        return ResolveTotalAmount(invoice, null);
    }

    public static string ResolveBuyerAccount(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        string buyerAccount = (invoice == null ? string.Empty : (invoice.buyer_account ?? string.Empty)).Trim();
        if (!string.IsNullOrWhiteSpace(buyerAccount))
            return buyerAccount.ToLowerInvariant();

        buyerAccount = (linkedOrder == null ? string.Empty : (linkedOrder.nguoimua ?? string.Empty)).Trim();
        return string.IsNullOrWhiteSpace(buyerAccount) ? string.Empty : buyerAccount.ToLowerInvariant();
    }

    public static string ResolveBuyerAccount(GH_HoaDon_tb invoice)
    {
        return ResolveBuyerAccount(invoice, null);
    }

    public static string ResolveBuyerDisplay(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        string buyerName = (invoice == null ? string.Empty : (invoice.ten_khach ?? string.Empty)).Trim();
        if (!string.IsNullOrWhiteSpace(buyerName))
            return buyerName;

        buyerName = (linkedOrder == null ? string.Empty : (linkedOrder.hoten_nguoinhan ?? string.Empty)).Trim();
        if (!string.IsNullOrWhiteSpace(buyerName))
            return buyerName;

        string buyerAccount = ResolveBuyerAccount(invoice, linkedOrder);
        return string.IsNullOrWhiteSpace(buyerAccount) ? "Khách lẻ" : buyerAccount;
    }

    public static string ResolveBuyerDisplay(GH_HoaDon_tb invoice)
    {
        return ResolveBuyerDisplay(invoice, null);
    }

    public static string ResolveSellerAccount(GH_HoaDon_tb invoice, DonHang_tb linkedOrder, string fallbackAccount)
    {
        string sellerAccount = (invoice == null ? string.Empty : (invoice.shop_taikhoan ?? string.Empty)).Trim();
        if (!string.IsNullOrWhiteSpace(sellerAccount))
            return sellerAccount.ToLowerInvariant();

        sellerAccount = (linkedOrder == null ? string.Empty : (linkedOrder.nguoiban ?? string.Empty)).Trim();
        if (!string.IsNullOrWhiteSpace(sellerAccount))
            return sellerAccount.ToLowerInvariant();

        sellerAccount = (fallbackAccount ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(sellerAccount) ? string.Empty : sellerAccount.ToLowerInvariant();
    }

    public static string ResolveSellerAccount(GH_HoaDon_tb invoice, string fallbackAccount)
    {
        return ResolveSellerAccount(invoice, null, fallbackAccount);
    }

    public static string ResolveSellerDisplayName(dbDataContext db, GH_HoaDon_tb invoice, DonHang_tb linkedOrder, string fallbackAccount, string fallbackLabel)
    {
        string sellerAccount = ResolveSellerAccount(invoice, linkedOrder, fallbackAccount);
        if (db != null && !string.IsNullOrWhiteSpace(sellerAccount))
        {
            taikhoan_tb seller = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == sellerAccount);
            if (seller != null)
            {
                string display = (seller.ten_shop ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(display))
                    return display;

                display = (seller.hoten ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(display))
                    return display;
            }
        }

        if (!string.IsNullOrWhiteSpace(sellerAccount))
            return sellerAccount;

        fallbackLabel = (fallbackLabel ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(fallbackLabel) ? "Gian hàng" : fallbackLabel;
    }

    public static string ResolveSellerDisplayName(dbDataContext db, GH_HoaDon_tb invoice, string fallbackAccount, string fallbackLabel)
    {
        return ResolveSellerDisplayName(db, invoice, null, fallbackAccount, fallbackLabel);
    }

    public static bool CanOpenWaitingExchange(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (linkedOrder != null)
            return DonHangStateMachine_cl.CanActivateChoTraoDoi(linkedOrder);

        return ResolveOrderStatusGroup(invoice) == "da-dat";
    }

    public static bool CanOpenWaitingExchange(GH_HoaDon_tb invoice)
    {
        return CanOpenWaitingExchange(invoice, null);
    }

    public static bool CanCancelWaitingExchange(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (linkedOrder != null)
            return DonHangStateMachine_cl.CanCancelChoTraoDoi(linkedOrder);

        return ResolveOrderStatusGroup(invoice) == "cho-trao-doi";
    }

    public static bool CanCancelWaitingExchange(GH_HoaDon_tb invoice)
    {
        return CanCancelWaitingExchange(invoice, null);
    }

    public static bool CanCancelOrder(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (linkedOrder != null)
            return DonHangStateMachine_cl.CanCancelOrder(linkedOrder);

        string statusGroup = ResolveOrderStatusGroup(invoice);
        return statusGroup != "da-trao-doi"
            && statusGroup != "da-giao"
            && statusGroup != "da-huy";
    }

    public static bool CanCancelOrder(GH_HoaDon_tb invoice)
    {
        return CanCancelOrder(invoice, null);
    }

    public static bool CanMarkDelivered(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (linkedOrder != null)
            return DonHangStateMachine_cl.CanMarkDelivered(linkedOrder);

        return ResolveOrderStatusGroup(invoice) == "da-dat";
    }

    public static bool CanMarkDelivered(GH_HoaDon_tb invoice)
    {
        return CanMarkDelivered(invoice, null);
    }

    public static bool IsWaitingExchange(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (linkedOrder != null)
            return DonHangStateMachine_cl.IsChoTraoDoi(linkedOrder);

        return ResolveOrderStatusGroup(invoice) == "cho-trao-doi";
    }

    public static bool IsWaitingExchange(GH_HoaDon_tb invoice)
    {
        return IsWaitingExchange(invoice, null);
    }

    public static bool CanExecuteExchange(GH_HoaDon_tb invoice, DonHang_tb linkedOrder)
    {
        if (linkedOrder != null)
            return DonHangStateMachine_cl.CanExecuteExchange(linkedOrder);

        return invoice != null && IsWaitingExchange(invoice);
    }

    public static bool CanExecuteExchange(GH_HoaDon_tb invoice)
    {
        return CanExecuteExchange(invoice, null);
    }

    private static List<GH_HoaDon_ChiTiet_tb> QueryDetails(dbDataContext db, long idHoaDon)
    {
        return db.GetTable<GH_HoaDon_ChiTiet_tb>()
            .Where(p => p.id_hoadon == idHoaDon)
            .OrderBy(p => p.id)
            .ToList();
    }

    public static GH_HoaDon_tb ResolveInvoiceByOrderKey(dbDataContext db, string shopTaiKhoan, string orderKey)
    {
        Guid invoiceGuid;
        bool hasGuidKey = Guid.TryParse(orderKey, out invoiceGuid);
        long invoiceId;
        bool hasNumericKey = long.TryParse(orderKey, out invoiceId);

        IQueryable<GH_HoaDon_tb> query = QueryByStorefront(db, shopTaiKhoan);
        if (hasGuidKey)
        {
            GH_HoaDon_tb invoiceByGuid = query
                .Where(p => p.id_guide == invoiceGuid)
                .OrderByDescending(p => p.id)
                .FirstOrDefault();
            if (invoiceByGuid != null)
                return invoiceByGuid;
        }

        if (hasNumericKey)
        {
            GH_HoaDon_tb invoiceById = query
                .Where(p => p.id == invoiceId || (p.id_donhang ?? string.Empty).Trim() == orderKey)
                .OrderByDescending(p => p.id)
                .FirstOrDefault();
            if (invoiceById != null)
                return invoiceById;
        }

        return query
            .Where(p => (p.id_donhang ?? string.Empty).Trim() == orderKey)
            .OrderByDescending(p => p.id)
            .FirstOrDefault();
    }

    private static string ResolveInvoiceOrderKey(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return string.Empty;

        string orderId = (invoice.id_donhang ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(orderId))
            return orderId;

        if (invoice.id_guide != Guid.Empty)
            return invoice.id_guide.ToString();

        return invoice.id.ToString();
    }

    private static string NormalizeAccount(string rawAccount)
    {
        return (rawAccount ?? string.Empty).Trim().ToLowerInvariant();
    }

}

public class GianHangCartItem
{
    public int Id { get; set; }
    public int SoLuong { get; set; }
}
