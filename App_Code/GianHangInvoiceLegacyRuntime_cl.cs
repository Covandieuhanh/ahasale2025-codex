using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangInvoiceLegacyRuntime_cl
{
    public static Dictionary<string, int> LoadLegacySoldTotalsByReferences(dbDataContext db, string shopTaiKhoan, IEnumerable<string> referenceIds)
    {
        return GianHangLegacyOrder_cl.LoadLegacySoldTotalsByReferences(db, NormalizeAccount(shopTaiKhoan), referenceIds);
    }

    public static int CountPendingExchangeBySeller(dbDataContext db, string shopTaiKhoan)
    {
        return GianHangLegacyOrder_cl.CountPendingExchangeBySeller(db, NormalizeAccount(shopTaiKhoan));
    }

    public static DonHang_tb FindLegacyOrderByOrderKey(dbDataContext db, string shopTaiKhoan, string rawOrderKey)
    {
        string tk = NormalizeAccount(shopTaiKhoan);
        string orderKey = (rawOrderKey ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(tk) || string.IsNullOrWhiteSpace(orderKey))
            return null;

        GH_HoaDon_tb linkedInvoice = FindInvoiceByOrderKeyDirect(db, tk, orderKey);
        DonHang_tb orderFromInvoice = FindLegacyOrderByInvoice(db, linkedInvoice);
        if (orderFromInvoice != null)
            return orderFromInvoice;

        long orderId;
        if (!long.TryParse(orderKey, out orderId) || orderId <= 0)
            return null;

        DonHang_tb order = FindLegacyOrderByLegacyId(db, tk, orderId);
        if (order != null)
            DonHangStateMachine_cl.EnsureStateFields(order);
        return order;
    }

    public static DonHang_tb FindLegacyOrderByInvoice(dbDataContext db, GH_HoaDon_tb invoice)
    {
        if (db == null || invoice == null)
            return null;

        string sellerAccount = NormalizeAccount(invoice.shop_taikhoan);
        string legacyOrderId = (invoice.id_donhang ?? string.Empty).Trim();
        long parsedLegacyOrderId;
        if (string.IsNullOrWhiteSpace(sellerAccount)
            || !long.TryParse(legacyOrderId, out parsedLegacyOrderId)
            || parsedLegacyOrderId <= 0)
            return null;

        DonHang_tb order = FindLegacyOrderByLegacyId(db, sellerAccount, parsedLegacyOrderId);
        if (order != null)
            DonHangStateMachine_cl.EnsureStateFields(order);
        return order;
    }

    public static DonHang_tb FindLatestWaitingExchange(dbDataContext db, string shopTaiKhoan)
    {
        string tk = NormalizeAccount(shopTaiKhoan);
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return null;

        DonHang_tb order = GianHangLegacyOrder_cl.FindLatestWaitingExchange(db, tk);
        if (order != null)
            DonHangStateMachine_cl.EnsureStateFields(order);
        return order;
    }

    public static DonHang_tb FindAnotherWaitingExchange(dbDataContext db, string shopTaiKhoan, long excludedOrderId)
    {
        string tk = NormalizeAccount(shopTaiKhoan);
        if (db == null || string.IsNullOrWhiteSpace(tk) || excludedOrderId <= 0)
            return null;

        DonHang_tb order = GianHangLegacyOrder_cl.FindAnotherWaitingExchange(db, tk, excludedOrderId);
        if (order != null)
            DonHangStateMachine_cl.EnsureStateFields(order);
        return order;
    }

    public static GH_HoaDon_tb EnsureSnapshotForLegacyOrder(dbDataContext db, DonHang_tb order)
    {
        if (db == null || order == null || order.id <= 0)
            return null;

        string sellerAccount = NormalizeAccount(order.nguoiban);
        if (string.IsNullOrWhiteSpace(sellerAccount))
            return null;

        string orderId = order.id.ToString();
        GH_HoaDon_tb invoice = GianHangInvoice_cl.FindByShopAndOrderId(db, sellerAccount, orderId);
        if (invoice != null)
        {
            bool changedExisting = ApplyLegacyRuntimeFields(invoice, order);
            GianHangInvoice_cl.EnsureDetailSnapshots(db, invoice);
            if (changedExisting)
                db.SubmitChanges();
            return invoice;
        }

        List<DonHang_ChiTiet_tb> orderItems = GianHangLegacyOrder_cl.LoadOrderDetails(db, orderId);
        if (orderItems.Count == 0)
            return null;

        List<GianHangInvoice_cl.SnapshotLine> snapshotLines = GianHangInvoiceLegacySnapshot_cl.BuildSnapshotLinesFromLegacyOrder(db, sellerAccount, orderItems);
        if (snapshotLines.Count == 0)
            return null;

        invoice = GianHangInvoice_cl.CreateSnapshotFromOrder(
            db,
            sellerAccount,
            orderId,
            (order.nguoimua ?? string.Empty).Trim(),
            (order.hoten_nguoinhan ?? string.Empty).Trim(),
            (order.sdt_nguoinhan ?? string.Empty).Trim(),
            (order.diahchi_nguoinhan ?? string.Empty).Trim(),
            string.Empty,
            order.online_offline.HasValue && order.online_offline.Value == false,
            snapshotLines);
        if (invoice == null)
            return null;

        bool changed = ApplyLegacyRuntimeFields(invoice, order);
        GianHangInvoice_cl.EnsureDetailSnapshots(db, invoice);
        if (changed)
            db.SubmitChanges();
        return invoice;
    }

    public static int EnsureSnapshotsForSellerLegacyOrders(dbDataContext db, string shopTaiKhoan, int take)
    {
        string sellerAccount = NormalizeAccount(shopTaiKhoan);
        int safeTake = take <= 0 ? 200 : take;
        if (db == null || string.IsNullOrWhiteSpace(sellerAccount))
            return 0;

        List<DonHang_tb> orders = GianHangLegacyOrder_cl.QueryBySeller(db, sellerAccount)
            .OrderByDescending(p => p.id)
            .Take(safeTake)
            .ToList();
        if (orders.Count == 0)
            return 0;

        HashSet<string> existingOrderIds = new HashSet<string>(GianHangInvoice_cl.QueryByStorefront(db, sellerAccount)
            .Where(p => p.id_donhang != null && p.id_donhang != string.Empty)
            .Select(p => p.id_donhang)
            .ToList(), StringComparer.OrdinalIgnoreCase);

        int created = 0;
        for (int i = 0; i < orders.Count; i++)
        {
            DonHang_tb order = orders[i];
            string orderId = order.id.ToString();
            if (existingOrderIds.Contains(orderId))
                continue;

            GH_HoaDon_tb invoice = EnsureSnapshotForLegacyOrder(db, order);
            if (invoice == null)
                continue;

            existingOrderIds.Add(orderId);
            created++;
        }

        return created;
    }

    public static int EnsureSnapshotsForBuyerLegacyOrders(dbDataContext db, string buyerAccount, int take)
    {
        string buyerKey = NormalizeAccount(buyerAccount);
        int safeTake = take <= 0 ? 200 : take;
        if (db == null || string.IsNullOrWhiteSpace(buyerKey))
            return 0;

        List<DonHang_tb> orders = GianHangLegacyOrder_cl.QueryByBuyer(db, buyerKey)
            .OrderByDescending(p => p.id)
            .Take(safeTake)
            .ToList();
        if (orders.Count == 0)
            return 0;

        HashSet<string> existingOrderIds = new HashSet<string>(GianHangInvoice_cl.QueryByBuyer(db, buyerKey)
            .Where(p => p.id_donhang != null && p.id_donhang != string.Empty)
            .Select(p => p.id_donhang)
            .ToList(), StringComparer.OrdinalIgnoreCase);

        int created = 0;
        for (int i = 0; i < orders.Count; i++)
        {
            DonHang_tb order = orders[i];
            string orderId = order.id.ToString();
            if (existingOrderIds.Contains(orderId))
                continue;

            GH_HoaDon_tb invoice = EnsureSnapshotForLegacyOrder(db, order);
            if (invoice == null)
                continue;

            if (string.Equals(GianHangInvoice_cl.ResolveBuyerAccount(invoice, order), buyerKey, StringComparison.OrdinalIgnoreCase))
            {
                existingOrderIds.Add(orderId);
                created++;
            }
        }

        return created;
    }

    public static bool NeedsLegacyActionRuntime(GH_HoaDon_tb invoice)
    {
        return invoice == null;
    }

    public static bool NeedsLegacyBuyerRuntime(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return false;

        return string.IsNullOrWhiteSpace((invoice.ten_khach ?? string.Empty).Trim())
            && string.IsNullOrWhiteSpace((invoice.buyer_account ?? string.Empty).Trim());
    }

    public static bool NeedsLegacyStatusRuntime(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return false;

        string orderStatus = (invoice.order_status ?? string.Empty).Trim();
        string exchangeStatus = (invoice.exchange_status ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(orderStatus) || string.IsNullOrWhiteSpace(exchangeStatus);
    }

    public static void BackfillLegacyRuntimeFields(dbDataContext db, IEnumerable<GH_HoaDon_tb> invoices)
    {
        if (db == null || invoices == null)
            return;

        List<GH_HoaDon_tb> targets = invoices
            .Where(p => p != null && (NeedsLegacyStatusRuntime(p) || NeedsLegacyBuyerRuntime(p) || !p.is_offline.HasValue || string.IsNullOrWhiteSpace((p.trang_thai ?? string.Empty).Trim())))
            .ToList();
        if (targets.Count == 0)
            return;

        Dictionary<string, DonHang_tb> linkedOrders = LoadLegacyOrderMapByInvoices(db, targets);
        bool changed = false;

        for (int i = 0; i < targets.Count; i++)
        {
            GH_HoaDon_tb invoice = targets[i];
            DonHang_tb order = ResolveLegacyOrder(linkedOrders, invoice);
            if (order == null)
                continue;

            DonHangStateMachine_cl.EnsureStateFields(order);
            if (ApplyLegacyRuntimeFields(invoice, order))
                changed = true;
        }

        if (changed)
            db.SubmitChanges();
    }

    public static void BackfillLegacyRuntimeFields(dbDataContext db, GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return;

        BackfillLegacyRuntimeFields(db, new[] { invoice });
    }

    private static Dictionary<string, DonHang_tb> LoadLegacyOrderMapByInvoices(dbDataContext db, IEnumerable<GH_HoaDon_tb> invoices)
    {
        if (db == null || invoices == null)
            return new Dictionary<string, DonHang_tb>(StringComparer.OrdinalIgnoreCase);

        List<long> orderIds = invoices
            .Select(p => ParseLegacyOrderId(p == null ? string.Empty : p.id_donhang))
            .Where(p => p > 0)
            .Distinct()
            .ToList();
        if (orderIds.Count == 0)
            return new Dictionary<string, DonHang_tb>(StringComparer.OrdinalIgnoreCase);

        return GianHangLegacyOrder_cl.LoadMapByIds(db, orderIds);
    }

    private static GH_HoaDon_tb FindInvoiceByOrderKeyDirect(dbDataContext db, string shopTaiKhoan, string orderKey)
    {
        if (db == null || string.IsNullOrWhiteSpace(shopTaiKhoan) || string.IsNullOrWhiteSpace(orderKey))
            return null;

        IQueryable<GH_HoaDon_tb> query = db.GetTable<GH_HoaDon_tb>()
            .Where(p => p.shop_taikhoan == shopTaiKhoan);

        Guid invoiceGuid;
        if (Guid.TryParse(orderKey, out invoiceGuid))
        {
            GH_HoaDon_tb invoiceByGuid = query
                .Where(p => p.id_guide == invoiceGuid)
                .OrderByDescending(p => p.id)
                .FirstOrDefault();
            if (invoiceByGuid != null)
                return invoiceByGuid;
        }

        long invoiceId;
        if (long.TryParse(orderKey, out invoiceId))
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

    private static DonHang_tb FindLegacyOrderByLegacyId(dbDataContext db, string shopTaiKhoan, long orderId)
    {
        if (db == null || string.IsNullOrWhiteSpace(shopTaiKhoan) || orderId <= 0)
            return null;

        return GianHangLegacyOrder_cl.FindBySellerAndId(db, shopTaiKhoan, orderId);
    }

    private static DonHang_tb ResolveLegacyOrder(Dictionary<string, DonHang_tb> linkedOrders, GH_HoaDon_tb invoice)
    {
        if (linkedOrders == null || invoice == null)
            return null;

        string orderId = (invoice.id_donhang ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(orderId))
            return null;

        DonHang_tb order;
        return linkedOrders.TryGetValue(orderId, out order) ? order : null;
    }

    private static long ParseLegacyOrderId(string rawValue)
    {
        long parsed;
        return long.TryParse((rawValue ?? string.Empty).Trim(), out parsed) ? parsed : 0L;
    }

    private static string NormalizeAccount(string rawAccount)
    {
        return (rawAccount ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static bool ApplyLegacyRuntimeFields(GH_HoaDon_tb invoice, DonHang_tb order)
    {
        if (invoice == null || order == null)
            return false;

        bool changed = false;

        if (string.IsNullOrWhiteSpace((invoice.id_donhang ?? string.Empty).Trim()) && order.id > 0)
        {
            invoice.id_donhang = order.id.ToString();
            changed = true;
        }

        if (string.IsNullOrWhiteSpace((invoice.buyer_account ?? string.Empty).Trim()) && !string.IsNullOrWhiteSpace((order.nguoimua ?? string.Empty).Trim()))
        {
            invoice.buyer_account = order.nguoimua.Trim().ToLowerInvariant();
            changed = true;
        }

        if (string.IsNullOrWhiteSpace((invoice.ten_khach ?? string.Empty).Trim()))
        {
            string buyerName = (order.hoten_nguoinhan ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(buyerName))
            {
                invoice.ten_khach = buyerName;
                changed = true;
            }
        }

        if (string.IsNullOrWhiteSpace((invoice.sdt ?? string.Empty).Trim()))
        {
            string buyerPhone = (order.sdt_nguoinhan ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(buyerPhone))
            {
                invoice.sdt = buyerPhone;
                changed = true;
            }
        }

        if (string.IsNullOrWhiteSpace((invoice.dia_chi ?? string.Empty).Trim()))
        {
            string buyerAddress = (order.diahchi_nguoinhan ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(buyerAddress))
            {
                invoice.dia_chi = buyerAddress;
                changed = true;
            }
        }

        if (!invoice.tong_tien.HasValue && order.tongtien.HasValue)
        {
            invoice.tong_tien = order.tongtien.Value;
            changed = true;
        }

        if (!invoice.ngay_tao.HasValue && order.ngaytao.HasValue)
        {
            invoice.ngay_tao = order.ngaytao.Value;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace((invoice.order_status ?? string.Empty).Trim()))
        {
            invoice.order_status = DonHangStateMachine_cl.GetOrderStatus(order);
            changed = true;
        }

        if (string.IsNullOrWhiteSpace((invoice.exchange_status ?? string.Empty).Trim()))
        {
            invoice.exchange_status = DonHangStateMachine_cl.GetExchangeStatus(order);
            changed = true;
        }

        if (!invoice.is_offline.HasValue && order.online_offline.HasValue)
        {
            invoice.is_offline = order.online_offline.Value == false;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace((invoice.trang_thai ?? string.Empty).Trim()))
        {
            string statusGroup = GianHangInvoice_cl.ResolveOrderStatusGroup(invoice, order);
            if (statusGroup == "da-huy")
                invoice.trang_thai = GianHangInvoice_cl.TrangThaiHuy;
            else if (statusGroup == "da-trao-doi")
                invoice.trang_thai = GianHangInvoice_cl.TrangThaiDaThu;
            else
                invoice.trang_thai = GianHangInvoice_cl.TrangThaiMoi;
            changed = true;
        }

        return changed;
    }
}
