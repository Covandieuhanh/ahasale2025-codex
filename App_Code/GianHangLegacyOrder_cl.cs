using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

public static class GianHangLegacyOrder_cl
{
    public sealed class CreateOrderInput
    {
        public string SellerAccount { get; set; }
        public string BuyerAccount { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public bool IsOnlineOrder { get; set; }
        public bool IsWaitingExchange { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class CreateOrderDetailInput
    {
        public string OrderId { get; set; }
        public string ReferenceId { get; set; }
        public string SellerAccount { get; set; }
        public int? NativeProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
    }

    public static IQueryable<DonHang_tb> QueryBySeller(dbDataContext db, string sellerAccount)
    {
        string seller = NormalizeAccount(sellerAccount);
        if (db == null || seller == string.Empty)
            return Enumerable.Empty<DonHang_tb>().AsQueryable();

        return db.DonHang_tbs.Where(p => p.nguoiban == seller);
    }

    public static DonHang_tb FindBySellerAndId(dbDataContext db, string sellerAccount, long orderId)
    {
        if (orderId <= 0)
            return null;

        return QueryBySeller(db, sellerAccount).FirstOrDefault(p => p.id == orderId);
    }

    public static IQueryable<DonHang_tb> QueryByBuyer(dbDataContext db, string buyerAccount)
    {
        string buyer = NormalizeAccount(buyerAccount);
        if (db == null || buyer == string.Empty)
            return Enumerable.Empty<DonHang_tb>().AsQueryable();

        return db.DonHang_tbs.Where(p => (p.nguoimua ?? string.Empty).Trim().ToLower() == buyer);
    }

    public static List<DonHang_tb> LoadByIds(dbDataContext db, IEnumerable<long> orderIds)
    {
        List<long> numericIds = (orderIds ?? Enumerable.Empty<long>())
            .Where(p => p > 0)
            .Distinct()
            .ToList();
        if (db == null || numericIds.Count == 0)
            return new List<DonHang_tb>();

        List<DonHang_tb> orders = db.DonHang_tbs.Where(p => numericIds.Contains(p.id)).ToList();
        for (int i = 0; i < orders.Count; i++)
            DonHangStateMachine_cl.EnsureStateFields(orders[i]);
        return orders;
    }

    public static Dictionary<string, DonHang_tb> LoadMapByIds(dbDataContext db, IEnumerable<long> orderIds)
    {
        return LoadByIds(db, orderIds)
            .Where(p => p != null && p.id > 0)
            .ToDictionary(p => p.id.ToString(), p => p, StringComparer.OrdinalIgnoreCase);
    }

    public static DonHang_tb FindLatestWaitingExchange(dbDataContext db, string sellerAccount)
    {
        return QueryBySeller(db, sellerAccount)
            .Where(p =>
                ((p.exchange_status ?? string.Empty).Trim() == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                || (string.IsNullOrWhiteSpace(p.exchange_status)
                    && (p.trangthai ?? string.Empty).Trim() == DonHangStateMachine_cl.Exchange_ChoTraoDoi))
            .OrderByDescending(p => p.id)
            .FirstOrDefault();
    }

    public static DonHang_tb FindAnotherWaitingExchange(dbDataContext db, string sellerAccount, long excludedOrderId)
    {
        return QueryBySeller(db, sellerAccount)
            .Where(p =>
                p.id != excludedOrderId
                && (((p.exchange_status ?? string.Empty).Trim() == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                    || (string.IsNullOrWhiteSpace(p.exchange_status)
                        && (p.trangthai ?? string.Empty).Trim() == DonHangStateMachine_cl.Exchange_ChoTraoDoi)))
            .OrderByDescending(p => p.id)
            .FirstOrDefault();
    }

    public static int CountPendingExchangeBySeller(dbDataContext db, string sellerAccount)
    {
        IQueryable<DonHang_tb> query = QueryBySeller(db, sellerAccount);
        try
        {
            return query.Count(p =>
                p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi));
        }
        catch (SqlException ex)
        {
            if (!IsMissingStatusColumnError(ex))
                throw;

            return query.Count(p => p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi);
        }
    }

    public static Dictionary<string, int> LoadLegacySoldTotalsByReferences(
        dbDataContext db,
        string sellerAccount,
        IEnumerable<string> referenceIds)
    {
        Dictionary<string, int> map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        string seller = NormalizeAccount(sellerAccount);
        List<string> ids = (referenceIds ?? Enumerable.Empty<string>())
            .Select(p => (p ?? string.Empty).Trim())
            .Where(p => p != string.Empty)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (db == null || seller == string.Empty || ids.Count == 0)
            return map;

        foreach (var row in db.DonHang_ChiTiet_tbs
            .Where(p => (p.nguoiban_goc == seller || p.nguoiban_danglai == seller)
                        && (!p.gh_product_id.HasValue || p.gh_product_id.Value <= 0)
                        && ids.Contains(p.idsp))
            .GroupBy(p => p.idsp)
            .Select(g => new { Id = g.Key, Total = g.Sum(x => (int?)x.soluong) ?? 0 })
            .ToList())
        {
            string key = (row.Id ?? string.Empty).Trim();
            if (key != string.Empty)
                map[key] = row.Total;
        }

        return map;
    }

    public static DonHang_tb CreateOrder(dbDataContext db, CreateOrderInput input)
    {
        if (db == null || input == null)
            return null;

        DonHang_tb order = new DonHang_tb
        {
            ngaydat = input.CreatedAt,
            nguoiban = NormalizeAccount(input.SellerAccount),
            nguoimua = (input.BuyerAccount ?? string.Empty).Trim(),
            tongtien = input.TotalAmount,
            hoten_nguoinhan = (input.CustomerName ?? string.Empty).Trim(),
            sdt_nguoinhan = (input.CustomerPhone ?? string.Empty).Trim(),
            diahchi_nguoinhan = (input.CustomerAddress ?? string.Empty).Trim(),
            online_offline = input.IsOnlineOrder,
            chothanhtoan = input.IsWaitingExchange
        };

        DonHangStateMachine_cl.SetOrderStatus(order, DonHangStateMachine_cl.Order_DaDat);
        DonHangStateMachine_cl.SetExchangeStatus(
            order,
            input.IsWaitingExchange ? DonHangStateMachine_cl.Exchange_ChoTraoDoi : DonHangStateMachine_cl.Exchange_ChuaTraoDoi);
        DonHangStateMachine_cl.SyncLegacyStatus(order);

        db.DonHang_tbs.InsertOnSubmit(order);
        db.SubmitChanges();
        return order;
    }

    public static DonHang_ChiTiet_tb AddOrderDetail(dbDataContext db, CreateOrderDetailInput input)
    {
        if (db == null || input == null)
            return null;

        int quantity = input.Quantity <= 0 ? 1 : input.Quantity;
        decimal price = input.Price < 0m ? 0m : input.Price;

        DonHang_ChiTiet_tb detail = new DonHang_ChiTiet_tb
        {
            id_donhang = (input.OrderId ?? string.Empty).Trim(),
            idsp = (input.ReferenceId ?? string.Empty).Trim(),
            nguoiban_goc = NormalizeAccount(input.SellerAccount),
            nguoiban_danglai = string.Empty,
            gh_product_id = input.NativeProductId,
            gh_name = (input.Name ?? string.Empty).Trim(),
            gh_image = (input.Image ?? string.Empty).Trim(),
            gh_type = (input.ProductType ?? string.Empty).Trim(),
            soluong = quantity,
            giaban = price,
            thanhtien = price * quantity,
            PhanTram_GiamGia_ThanhToan_BangEvoucher = ClampInt(input.DiscountPercent, 0, 50)
        };

        db.DonHang_ChiTiet_tbs.InsertOnSubmit(detail);
        return detail;
    }

    public static List<DonHang_ChiTiet_tb> LoadOrderDetails(dbDataContext db, string orderId)
    {
        string safeOrderId = (orderId ?? string.Empty).Trim();
        if (db == null || safeOrderId == string.Empty)
            return new List<DonHang_ChiTiet_tb>();

        return db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == safeOrderId).ToList();
    }

    private static string NormalizeAccount(string sellerAccount)
    {
        return (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static bool IsMissingStatusColumnError(SqlException ex)
    {
        if (ex == null)
            return false;

        string message = ex.Message ?? string.Empty;
        return message.IndexOf("Invalid column name 'exchange_status'", StringComparison.OrdinalIgnoreCase) >= 0
            || message.IndexOf("Invalid column name 'order_status'", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
