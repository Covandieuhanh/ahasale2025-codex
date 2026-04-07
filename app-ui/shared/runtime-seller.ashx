<%@ WebHandler Language="C#" Class="AppUiRuntimeSellerHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeSellerHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        try
        {
            RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
            if (info == null || !info.IsAuthenticated || !info.CanAccessGianHang || string.IsNullOrWhiteSpace(info.AccountKey))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new
                {
                    ok = false,
                    reason = "unauthorized",
                    snapshot = (object)null
                }));
                return;
            }

            string accountKey = (info.AccountKey ?? "").Trim().ToLowerInvariant();
            object snapshot = CoreDb_cl.Use(db => BuildSnapshot(db, accountKey));
            context.Response.Write(new JavaScriptSerializer().Serialize(new
            {
                ok = true,
                source = "runtime",
                snapshot = snapshot
            }));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_seller", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write("{\"ok\":false,\"reason\":\"error\",\"snapshot\":null}");
        }
    }

    public bool IsReusable { get { return false; } }

    private static object BuildSnapshot(dbDataContext db, string accountKey)
    {
        if (db == null || string.IsNullOrWhiteSpace(accountKey))
        {
            return new
            {
                quick_actions = new object[0],
                status_tabs = new object[0],
                metrics = new object[0],
                listings = new object[0],
                conversations = new object[0]
            };
        }

        IQueryable<GH_SanPham_tb> postsQuery = GianHangProduct_cl.QueryByStorefront(db, accountKey);
        List<GH_SanPham_tb> posts = postsQuery
            .OrderByDescending(p => p.ngay_cap_nhat ?? p.ngay_tao)
            .ThenByDescending(p => p.id)
            .Take(120)
            .ToList();

        int activeCount = posts.Count(p => p.bin == null || p.bin == false);
        int hiddenCount = posts.Count - activeCount;
        int customerCount = GianHangCustomer_cl.CountCustomers(db, accountKey);

        var dashboard = GianHangReport_cl.BuildDashboard(
            db,
            accountKey,
            GianHangReport_cl.ResolveDateRange("", "", "", "all"));

        List<object> listings = posts.Select(MapListing).Cast<object>().ToList();

        List<object> conversations = GianHangCustomer_cl.LoadCustomers(db, accountKey, "", 14)
            .Select(customer => new
            {
                name = string.IsNullOrWhiteSpace(customer.DisplayName) ? "Khách hàng" : customer.DisplayName,
                topic = BuildCustomerTopic(customer),
                state = "Cần phản hồi",
                time = FormatAgo(customer.LastInteractionAt)
            })
            .Cast<object>()
            .ToList();

        List<object> orders = (dashboard.RecentOrders ?? new List<GianHangReport_cl.RecentOrderRow>())
            .Select(order => new
            {
                id = order.Id,
                buyer = string.IsNullOrWhiteSpace(order.BuyerDisplay) ? "Khách lẻ" : order.BuyerDisplay,
                ordered_at = order.OrderedAt.HasValue ? order.OrderedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--",
                status = string.IsNullOrWhiteSpace(order.StatusText) ? "Đang xử lý" : order.StatusText,
                amount = order.TotalAmount.ToString("#,##0") + " đ"
            })
            .Cast<object>()
            .ToList();

        List<object> appointments = (dashboard.RecentBookings ?? new List<GianHangReport_cl.RecentBookingRow>())
            .Select(booking => new
            {
                id = booking.Id,
                customer = string.IsNullOrWhiteSpace(booking.CustomerName) ? "Khách đặt lịch" : booking.CustomerName,
                service = string.IsNullOrWhiteSpace(booking.ServiceName) ? "Dịch vụ" : booking.ServiceName,
                booking_time = booking.BookingTime.HasValue ? booking.BookingTime.Value.ToString("dd/MM/yyyy HH:mm") : "--",
                status = string.IsNullOrWhiteSpace(booking.StatusText) ? "Chưa xác nhận" : booking.StatusText
            })
            .Cast<object>()
            .ToList();

        return new
        {
            quick_actions = new[]
            {
                new { label = "Đăng tin", tone = "primary", href = "/app-ui/gianhang/post.aspx?ui_mode=app" },
                new { label = "Quản lý tin", tone = "primary", href = "/app-ui/gianhang/listing.aspx?ui_mode=app" },
                new { label = "Tạo đơn", tone = "primary", href = "/app-ui/gianhang/create-order.aspx?ui_mode=app" },
                new { label = "Chờ thanh toán", tone = "neutral", href = "/app-ui/gianhang/pending-payment.aspx?ui_mode=app" },
                new { label = "Lịch hẹn", tone = "neutral", href = "/app-ui/gianhang/status.aspx?ui_mode=app&tab=" + HttpUtility.UrlEncode("Lịch hẹn") },
                new { label = "Khách hàng", tone = "neutral", href = "/app-ui/gianhang/conversations.aspx?ui_mode=app" },
                new { label = "Mở quản trị", tone = "neutral", href = "/app-ui/gianhang/actions.aspx?ui_mode=app" }
            },
            status_tabs = new[] { "Tất cả", "Chờ duyệt", "Đang bán", "Cần sửa", "Đã ẩn" },
            metrics = new[]
            {
                new { label = "Tin đăng hoạt động", value = activeCount.ToString("#,##0") },
                new { label = "Khách đang trao đổi", value = customerCount.ToString("#,##0") },
                new { label = "Lịch hẹn hôm nay", value = dashboard.BookingPending.ToString("#,##0") }
            },
            listings = listings,
            conversations = conversations,
            orders = orders,
            appointments = appointments,
            report_summary = new
            {
                revenue_gross = dashboard.RevenueGross.ToString("#,##0") + " đ",
                total_orders = dashboard.TotalOrders.ToString("#,##0"),
                pending_orders = dashboard.PendingOrders.ToString("#,##0"),
                total_sold = dashboard.TotalSold.ToString("#,##0"),
                booking_total = dashboard.BookingTotal.ToString("#,##0"),
                booking_pending = dashboard.BookingPending.ToString("#,##0")
            }
        };
    }

    private static object MapListing(GH_SanPham_tb post)
    {
        bool isHidden = post != null && post.bin == true;
        string status = isHidden ? "Đã ẩn" : "Đang bán";
        string statusTone = isHidden ? "pending" : "live";
        string category = ResolveCategory(post);
        string summary = (post == null ? "" : (post.mo_ta ?? "").Trim());
        if (summary.Length > 140)
            summary = summary.Substring(0, 140).Trim() + "...";

        int views = post == null ? 0 : (post.luot_truy_cap ?? 0);
        DateTime? updated = post == null ? (DateTime?)null : (post.ngay_cap_nhat ?? post.ngay_tao);
        string postType = post == null ? "" : ((post.loai ?? "").Trim().ToLowerInvariant());

        return new
        {
            id = "runtime-gh-" + (post == null ? 0 : post.id),
            title = post == null ? "Tin gian hàng" : ((post.ten ?? "").Trim()),
            status = status,
            statusTone = statusTone,
            category = category,
            stat = "Lượt xem " + views.ToString("#,##0"),
            updatedAt = updated.HasValue ? ("Cập nhật " + updated.Value.ToString("dd/MM/yyyy HH:mm")) : "Cập nhật --",
            summary = string.IsNullOrWhiteSpace(summary) ? "Tin đang được quản lý trong không gian gian hàng." : summary,
            publishTargets = post != null && post.id_baiviet.HasValue
                ? new[] { "Gian hàng", "Home" }
                : new[] { "Gian hàng" },
            reviewNotes = isHidden
                ? new[] { "Tin đang ẩn, bật lại khi sẵn sàng." }
                : new[] { "Theo dõi lead mới và phản hồi nhanh." },
            leads = new[] { "Theo dõi trao đổi từ khách hàng quan tâm." },
            quickActions = new[] { "Sửa nội dung", "Đẩy tin", isHidden ? "Hiển thị lại tin" : "Tạm ẩn tin" },
            publishHistory = new[] { "Đồng bộ dữ liệu từ gian hàng." },
            checklist = new[] { "Kiểm tra giá", "Kiểm tra ảnh", "Theo dõi phản hồi" }
        };
    }

    private static string ResolveCategory(GH_SanPham_tb post)
    {
        string postType = post == null ? "" : ((post.loai ?? "").Trim().ToLowerInvariant());
        if (postType == GianHangProduct_cl.LoaiDichVu)
            return "Dịch vụ";
        if (postType == GianHangProduct_cl.LoaiSanPham)
            return "Sản phẩm";
        return "Tin gian hàng";
    }

    private static string BuildCustomerTopic(GianHangCustomer_cl.CustomerRow customer)
    {
        if (customer == null)
            return "Khách hàng mới cần theo dõi.";

        List<string> parts = new List<string>();
        if (customer.OrderCount > 0)
            parts.Add(customer.OrderCount.ToString("#,##0") + " đơn");
        if (customer.BookingCount > 0)
            parts.Add(customer.BookingCount.ToString("#,##0") + " lịch hẹn");
        if (parts.Count == 0)
            return "Khách hàng mới cần theo dõi.";
        return "Đang có " + string.Join(" và ", parts.ToArray()) + ".";
    }

    private static string FormatAgo(DateTime? value)
    {
        if (!value.HasValue)
            return "--";

        TimeSpan diff = AhaTime_cl.Now - value.Value;
        if (diff.TotalMinutes < 1)
            return "vừa xong";
        if (diff.TotalMinutes < 60)
            return ((int)Math.Floor(diff.TotalMinutes)).ToString() + " phút trước";
        if (diff.TotalHours < 24)
            return ((int)Math.Floor(diff.TotalHours)).ToString() + " giờ trước";
        return ((int)Math.Floor(diff.TotalDays)).ToString() + " ngày trước";
    }
}
