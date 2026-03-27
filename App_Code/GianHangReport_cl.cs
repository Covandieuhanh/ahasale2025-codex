using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class GianHangReport_cl
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public sealed class ReportRange
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? ToExclusive { get; set; }
        public string MonthText { get; set; }
        public bool IsAllTime { get; set; }
    }

    public sealed class Dashboard
    {
        public int ProductCount { get; set; }
        public int ServiceCount { get; set; }
        public int TotalViews { get; set; }
        public decimal RevenueGross { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int WaitingExchange { get; set; }
        public int Exchanged { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
        public int TotalSold { get; set; }
        public int BookingTotal { get; set; }
        public int BookingPending { get; set; }
        public int BookingConfirmed { get; set; }
        public int BookingDone { get; set; }
        public int BookingCancelled { get; set; }
        public List<RecentOrderRow> RecentOrders { get; set; }
        public List<RecentBookingRow> RecentBookings { get; set; }
    }

    public sealed class RecentOrderRow
    {
        public long Id { get; set; }
        public DateTime? OrderedAt { get; set; }
        public string BuyerDisplay { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public sealed class RecentBookingRow
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string ServiceName { get; set; }
        public DateTime? BookingTime { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
    }

    public static ReportRange ResolveDateRange(string monthText, string fromText, string toText, string rangeRaw)
    {
        ReportRange range = new ReportRange();

        string rangeFlag = (rangeRaw ?? "").Trim().ToLowerInvariant();
        if (rangeFlag == "all")
        {
            range.IsAllTime = true;
            return range;
        }

        DateTime parsed;
        if (TryParseDate(fromText, out parsed))
            range.FromDate = parsed.Date;

        if (TryParseDate(toText, out parsed))
            range.ToDate = parsed.Date;

        if (range.FromDate.HasValue || range.ToDate.HasValue)
        {
            range.MonthText = "";
        }
        else if (TryParseMonth(monthText, out parsed))
        {
            range.MonthText = parsed.ToString("yyyy-MM");
            range.FromDate = new DateTime(parsed.Year, parsed.Month, 1);
            range.ToDate = range.FromDate.Value.AddMonths(1).AddDays(-1);
            range.ToExclusive = range.FromDate.Value.AddMonths(1);
            return range;
        }

        if (range.FromDate.HasValue && range.ToDate.HasValue && range.FromDate.Value > range.ToDate.Value)
        {
            DateTime swap = range.FromDate.Value;
            range.FromDate = range.ToDate;
            range.ToDate = swap;
        }

        if (range.ToDate.HasValue)
            range.ToExclusive = range.ToDate.Value.AddDays(1);

        if (!range.FromDate.HasValue && !range.ToDate.HasValue)
        {
            DateTime now = AhaTime_cl.Now;
            range.FromDate = new DateTime(now.Year, now.Month, 1);
            range.ToDate = range.FromDate.Value.AddMonths(1).AddDays(-1);
            range.ToExclusive = range.FromDate.Value.AddMonths(1);
            range.MonthText = range.FromDate.Value.ToString("yyyy-MM");
        }

        return range;
    }

    public static string BuildRangeLabel(ReportRange range)
    {
        if (range == null || range.IsAllTime)
            return "Khoảng thời gian: Toàn thời gian";

        if (!string.IsNullOrWhiteSpace(range.MonthText))
        {
            DateTime monthValue;
            if (DateTime.TryParseExact(range.MonthText, "yyyy-MM", Invariant, DateTimeStyles.None, out monthValue))
                return string.Format("Khoảng thời gian: Tháng {0:MM/yyyy}", monthValue);
        }

        string fromText = range.FromDate.HasValue ? range.FromDate.Value.ToString("dd/MM/yyyy") : "...";
        string toText = range.ToDate.HasValue ? range.ToDate.Value.ToString("dd/MM/yyyy") : "...";
        return string.Format("Khoảng thời gian: {0} - {1}", fromText, toText);
    }

    public static Dashboard BuildDashboard(dbDataContext db, string accountKey, ReportRange range)
    {
        Dashboard result = new Dashboard
        {
            RecentOrders = new List<RecentOrderRow>(),
            RecentBookings = new List<RecentBookingRow>()
        };

        string tk = (accountKey ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return result;

        IQueryable<GH_SanPham_tb> posts = GianHangProduct_cl.QueryPublicByStorefront(db, tk);

        result.ProductCount = posts.Count(p => (p.loai ?? "").Trim().ToLower() == GianHangProduct_cl.LoaiSanPham);
        result.ServiceCount = posts.Count(p => (p.loai ?? "").Trim().ToLower() == GianHangProduct_cl.LoaiDichVu);
        result.TotalViews = posts.Sum(p => (int?)p.luot_truy_cap) ?? 0;

        IQueryable<GH_HoaDon_tb> invoiceQuery = GianHangInvoice_cl.QueryByStorefront(db, tk);
        if (range != null && !range.IsAllTime)
        {
            if (range.FromDate.HasValue)
                invoiceQuery = invoiceQuery.Where(p => p.ngay_tao >= range.FromDate.Value);
            if (range.ToExclusive.HasValue)
                invoiceQuery = invoiceQuery.Where(p => p.ngay_tao < range.ToExclusive.Value);
        }

        List<GH_HoaDon_tb> invoices = invoiceQuery.OrderByDescending(p => p.id).ToList();
        GianHangInvoice_cl.PrepareInvoicesWithRuntime(db, invoices);
        result.TotalOrders = invoices.Count;

        foreach (GH_HoaDon_tb invoice in invoices)
        {
            string statusGroup = GianHangInvoice_cl.ResolveOrderStatusGroup(invoice);
            switch (statusGroup)
            {
                case "cho-trao-doi":
                    result.WaitingExchange++;
                    break;
                case "da-trao-doi":
                    result.Exchanged++;
                    break;
                case "da-giao":
                    result.Delivered++;
                    break;
                case "da-huy":
                    result.Cancelled++;
                    break;
                default:
                    result.PendingOrders++;
                    break;
            }
        }

        if (invoices.Count > 0)
        {
            List<long> invoiceIds = invoices.Select(p => p.id).ToList();
            List<GH_HoaDon_ChiTiet_tb> details = db.GetTable<GH_HoaDon_ChiTiet_tb>()
                .Where(p => invoiceIds.Contains(p.id_hoadon))
                .ToList();

            result.RevenueGross = invoices.Sum(p => p.tong_tien ?? 0m);
            result.TotalSold = details.Sum(p => p.so_luong ?? 0);
        }

        result.RecentOrders = invoices
            .Take(8)
            .Select(p => new RecentOrderRow
            {
                Id = p.id,
                OrderedAt = p.ngay_tao,
                BuyerDisplay = GianHangInvoice_cl.ResolveBuyerDisplay(p),
                StatusText = GianHangInvoice_cl.ResolveOrderStatusText(p),
                StatusCss = GianHangInvoice_cl.ResolveOrderStatusCss(p),
                TotalAmount = GianHangInvoice_cl.ResolveTotalAmount(p)
            })
            .ToList();

        IQueryable<GH_DatLich_tb> bookingQuery = GianHangBooking_cl.QueryByStorefront(db, tk);
        if (range != null && !range.IsAllTime)
        {
            if (range.FromDate.HasValue)
                bookingQuery = bookingQuery.Where(p => p.ngay_tao >= range.FromDate.Value);
            if (range.ToExclusive.HasValue)
                bookingQuery = bookingQuery.Where(p => p.ngay_tao < range.ToExclusive.Value);
        }

        List<GH_DatLich_tb> bookings = bookingQuery.OrderByDescending(p => p.ngay_tao).ThenByDescending(p => p.id).ToList();
        result.BookingTotal = bookings.Count;
        foreach (GH_DatLich_tb booking in bookings)
        {
            string status = (booking.trang_thai ?? "").Trim();
            if (status == GianHangBooking_cl.TrangThaiDaXacNhan)
                result.BookingConfirmed++;
            else if (status == GianHangBooking_cl.TrangThaiHoanThanh)
                result.BookingDone++;
            else if (status == GianHangBooking_cl.TrangThaiHuy)
                result.BookingCancelled++;
            else
                result.BookingPending++;
        }

        result.RecentBookings = bookings
            .Take(8)
            .Select(p => new RecentBookingRow
            {
                Id = p.id,
                CustomerName = string.IsNullOrWhiteSpace(p.ten_khach) ? "Khách lẻ" : p.ten_khach,
                Phone = p.sdt ?? "",
                ServiceName = GianHangBooking_cl.ResolveServiceName(p),
                BookingTime = p.thoi_gian_hen,
                StatusText = string.IsNullOrWhiteSpace(p.trang_thai) ? GianHangBooking_cl.TrangThaiChoXacNhan : p.trang_thai,
                StatusCss = ResolveBookingStatusCss(p.trang_thai)
            })
            .ToList();

        return result;
    }

    public static string FormatDateTime(DateTime? value)
    {
        if (!value.HasValue)
            return "--";
        return value.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("vi-VN"));
    }

    public static string FormatCurrency(decimal value)
    {
        return value.ToString("#,##0");
    }

    private static bool TryParseDate(string text, out DateTime date)
    {
        date = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(text))
            return false;
        string[] formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy" };
        return DateTime.TryParseExact(text.Trim(), formats, Invariant, DateTimeStyles.None, out date);
    }

    private static bool TryParseMonth(string text, out DateTime date)
    {
        date = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(text))
            return false;
        string[] formats = new[] { "yyyy-MM", "MM/yyyy" };
        return DateTime.TryParseExact(text.Trim(), formats, Invariant, DateTimeStyles.None, out date);
    }

    private static string ResolveBookingStatusCss(string rawStatus)
    {
        string status = (rawStatus ?? "").Trim();
        if (status == GianHangBooking_cl.TrangThaiDaXacNhan)
            return "gh-report-status gh-report-status-info";
        if (status == GianHangBooking_cl.TrangThaiHoanThanh)
            return "gh-report-status gh-report-status-success";
        if (status == GianHangBooking_cl.TrangThaiHuy)
            return "gh-report-status gh-report-status-danger";
        return "gh-report-status gh-report-status-warning";
    }
}
