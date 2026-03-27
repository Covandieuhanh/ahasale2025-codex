using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangCustomer_cl
{
    public sealed class CustomerRow
    {
        public string CustomerKey { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string BuyerAccount { get; set; }
        public int OrderCount { get; set; }
        public int BookingCount { get; set; }
        public decimal RevenueTotal { get; set; }
        public DateTime? LastInteractionAt { get; set; }
    }

    public sealed class CustomerOrderRow
    {
        public long OrderId { get; set; }
        public string BuyerDisplay { get; set; }
        public string Phone { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? OrderedAt { get; set; }
        public string StatusText { get; set; }
        public string StatusGroup { get; set; }
    }

    public sealed class CustomerBookingRow
    {
        public long BookingId { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string ServiceName { get; set; }
        public DateTime? ScheduleAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string StatusText { get; set; }
    }

    public sealed class CustomerDetail
    {
        public string CustomerKey { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string BuyerAccount { get; set; }
        public int OrderCount { get; set; }
        public int BookingCount { get; set; }
        public decimal RevenueTotal { get; set; }
        public DateTime? LastInteractionAt { get; set; }
        public List<CustomerOrderRow> RecentOrders { get; set; }
        public List<CustomerBookingRow> RecentBookings { get; set; }
    }

    private sealed class Aggregate
    {
        public string CustomerKey { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string BuyerAccount { get; set; }
        public int OrderCount { get; set; }
        public int BookingCount { get; set; }
        public decimal RevenueTotal { get; set; }
        public DateTime? LastInteractionAt { get; set; }
    }

    public static List<CustomerRow> LoadCustomers(dbDataContext db, string accountKey, string keyword, int take)
    {
        string tk = (accountKey ?? "").Trim().ToLowerInvariant();
        string search = (keyword ?? "").Trim().ToLowerInvariant();
        int safeTake = Math.Max(1, take);

        if (db == null || string.IsNullOrWhiteSpace(tk))
            return new List<CustomerRow>();

        Dictionary<string, Aggregate> map = BuildAggregateMap(db, tk);

        IEnumerable<CustomerRow> query = map.Values.Select(p => new CustomerRow
        {
            CustomerKey = p.CustomerKey,
            DisplayName = string.IsNullOrWhiteSpace(p.DisplayName) ? "Khách lẻ" : p.DisplayName,
            Phone = string.IsNullOrWhiteSpace(p.Phone) ? "--" : p.Phone,
            BuyerAccount = string.IsNullOrWhiteSpace(p.BuyerAccount) ? "" : p.BuyerAccount,
            OrderCount = p.OrderCount,
            BookingCount = p.BookingCount,
            RevenueTotal = p.RevenueTotal,
            LastInteractionAt = p.LastInteractionAt
        });

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                ((p.DisplayName ?? "").ToLowerInvariant().Contains(search)) ||
                ((p.Phone ?? "").ToLowerInvariant().Contains(search)) ||
                ((p.BuyerAccount ?? "").ToLowerInvariant().Contains(search)));
        }

        return query
            .OrderByDescending(p => p.LastInteractionAt ?? DateTime.MinValue)
            .ThenBy(p => p.DisplayName)
            .Take(safeTake)
            .ToList();
    }

    public static int CountCustomers(dbDataContext db, string accountKey)
    {
        string tk = (accountKey ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return 0;

        return BuildAggregateMap(db, tk).Count;
    }

    public static CustomerDetail LoadCustomerDetail(dbDataContext db, string accountKey, string customerKey, int orderTake, int bookingTake)
    {
        string tk = (accountKey ?? "").Trim().ToLowerInvariant();
        string key = (customerKey ?? "").Trim();
        if (db == null || string.IsNullOrWhiteSpace(tk) || string.IsNullOrWhiteSpace(key))
            return null;

        Dictionary<string, Aggregate> map = BuildAggregateMap(db, tk);
        Aggregate current;
        if (!map.TryGetValue(key, out current))
            return null;

        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.LoadStorefrontInvoicesWithRuntime(db, tk, 500, 2000);
        List<CustomerOrderRow> orders = invoices
            .Where(p => string.Equals(BuildCustomerKey(p.sdt, p.buyer_account, p.ten_khach), key, StringComparison.OrdinalIgnoreCase))
            .Take(Math.Max(1, orderTake))
            .Select(p => new CustomerOrderRow
            {
                OrderId = ParseOrderId(p),
                BuyerDisplay = PickValue((p.ten_khach ?? "").Trim(), (p.buyer_account ?? "").Trim(), "Khách lẻ"),
                Phone = string.IsNullOrWhiteSpace(NormalizePhone(p.sdt)) ? "--" : NormalizePhone(p.sdt),
                TotalAmount = p.tong_tien ?? 0m,
                OrderedAt = p.ngay_tao,
                StatusText = GianHangInvoice_cl.ResolveOrderStatusText(p, null),
                StatusGroup = ResolveOrderStatusGroup(p)
            })
            .ToList();

        List<CustomerBookingRow> bookings = GianHangBooking_cl.QueryByStorefront(db, tk)
            .OrderByDescending(p => p.id)
            .Take(2000)
            .ToList()
            .Where(p => string.Equals(BuildCustomerKey(p.sdt, "", p.ten_khach), key, StringComparison.OrdinalIgnoreCase))
            .Take(Math.Max(1, bookingTake))
            .Select(p => new CustomerBookingRow
            {
                BookingId = p.id,
                DisplayName = PickValue((p.ten_khach ?? "").Trim(), "Khách đặt lịch"),
                Phone = string.IsNullOrWhiteSpace(NormalizePhone(p.sdt)) ? "--" : NormalizePhone(p.sdt),
                ServiceName = GianHangBooking_cl.ResolveServiceName(p),
                ScheduleAt = p.thoi_gian_hen,
                CreatedAt = p.ngay_tao,
                StatusText = string.IsNullOrWhiteSpace(p.trang_thai) ? GianHangBooking_cl.TrangThaiChoXacNhan : p.trang_thai.Trim()
            })
            .ToList();

        return new CustomerDetail
        {
            CustomerKey = current.CustomerKey,
            DisplayName = string.IsNullOrWhiteSpace(current.DisplayName) ? "Khách lẻ" : current.DisplayName,
            Phone = string.IsNullOrWhiteSpace(current.Phone) ? "--" : current.Phone,
            BuyerAccount = string.IsNullOrWhiteSpace(current.BuyerAccount) ? "" : current.BuyerAccount,
            OrderCount = current.OrderCount,
            BookingCount = current.BookingCount,
            RevenueTotal = current.RevenueTotal,
            LastInteractionAt = current.LastInteractionAt,
            RecentOrders = orders,
            RecentBookings = bookings
        };
    }

    private static Dictionary<string, Aggregate> BuildAggregateMap(dbDataContext db, string accountKey)
    {
        Dictionary<string, Aggregate> map = new Dictionary<string, Aggregate>(StringComparer.OrdinalIgnoreCase);

        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.LoadStorefrontInvoicesWithRuntime(db, accountKey, 500, 2000);

        foreach (GH_HoaDon_tb invoice in invoices)
        {
            string key = BuildCustomerKey(invoice.sdt, invoice.buyer_account, invoice.ten_khach);
            if (string.IsNullOrWhiteSpace(key))
                continue;

            Aggregate item;
            if (!map.TryGetValue(key, out item))
            {
                item = new Aggregate { CustomerKey = key };
                map[key] = item;
            }

            item.DisplayName = PickValue(item.DisplayName, (invoice.ten_khach ?? "").Trim(), (invoice.buyer_account ?? "").Trim(), "Khách lẻ");
            item.Phone = PickValue(item.Phone, NormalizePhone(invoice.sdt));
            item.BuyerAccount = PickValue(item.BuyerAccount, (invoice.buyer_account ?? "").Trim());
            item.OrderCount++;
            item.RevenueTotal += invoice.tong_tien ?? 0m;
            item.LastInteractionAt = MaxDate(item.LastInteractionAt, invoice.ngay_tao);
        }

        List<GH_DatLich_tb> bookings = GianHangBooking_cl.QueryByStorefront(db, accountKey)
            .OrderByDescending(p => p.id)
            .Take(2000)
            .ToList();

        foreach (GH_DatLich_tb booking in bookings)
        {
            string key = BuildCustomerKey(booking.sdt, "", booking.ten_khach);
            if (string.IsNullOrWhiteSpace(key))
                continue;

            Aggregate item;
            if (!map.TryGetValue(key, out item))
            {
                item = new Aggregate { CustomerKey = key };
                map[key] = item;
            }

            item.DisplayName = PickValue(item.DisplayName, (booking.ten_khach ?? "").Trim(), "Khách đặt lịch");
            item.Phone = PickValue(item.Phone, NormalizePhone(booking.sdt));
            item.BookingCount++;
            item.LastInteractionAt = MaxDate(item.LastInteractionAt, booking.ngay_tao ?? booking.thoi_gian_hen);
        }

        return map;
    }

    private static string BuildCustomerKey(string phoneRaw, string buyerAccountRaw, string nameRaw)
    {
        string phone = NormalizePhone(phoneRaw);
        if (!string.IsNullOrWhiteSpace(phone))
            return "phone:" + phone;

        string buyerAccount = (buyerAccountRaw ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(buyerAccount))
            return "account:" + buyerAccount;

        string name = (nameRaw ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(name))
            return "name:" + name;

        return "";
    }

    private static string ResolveOrderStatusGroup(GH_HoaDon_tb invoice)
    {
        string statusGroup = GianHangInvoice_cl.ResolveOrderStatusGroup(invoice);
        if (statusGroup == "da-huy")
            return "cancelled";
        if (statusGroup == "cho-trao-doi")
            return "waiting";
        if (statusGroup == "da-trao-doi")
            return "success";
        if (statusGroup == "da-giao")
            return "done";
        return "active";
    }

    private static string NormalizePhone(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "";

        char[] chars = value.Where(char.IsDigit).ToArray();
        return new string(chars);
    }

    private static string PickValue(params string[] values)
    {
        foreach (string value in values)
        {
            string current = (value ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(current))
                return current;
        }

        return "";
    }

    private static DateTime? MaxDate(DateTime? current, DateTime? next)
    {
        if (!current.HasValue)
            return next;
        if (!next.HasValue)
            return current;
        return current.Value >= next.Value ? current : next;
    }

    private static long ParseOrderId(GH_HoaDon_tb invoice)
    {
        long parsed;
        string raw = GianHangInvoice_cl.ResolveOrderPublicId(invoice);
        return long.TryParse((raw ?? string.Empty).Trim(), out parsed) && parsed > 0
            ? parsed
            : (invoice == null ? 0L : invoice.id);
    }
}
