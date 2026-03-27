using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web;

public partial class gianhang_quan_ly_lich_hen : System.Web.UI.Page
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
        {
            ApplyQueryFilter();
            HandleAction(info);
            BindPage(info);
        }
    }

    private void HandleAction(RootAccount_cl.RootAccountInfo info)
    {
        string action = (Request.QueryString["action"] ?? "").Trim().ToLowerInvariant();
        long bookingId;
        if (!long.TryParse((Request.QueryString["id"] ?? "").Trim(), out bookingId))
            return;

        if (action != "confirm" && action != "done" && action != "cancel")
            return;

        string gianHangTaiKhoan = info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(gianHangTaiKhoan))
            return;

        string nextStatus = "";
        string notice = "";
        switch (action)
        {
            case "confirm":
                nextStatus = GianHangBooking_cl.TrangThaiDaXacNhan;
                notice = "Đã xác nhận lịch hẹn #" + bookingId.ToString() + ".";
                break;
            case "done":
                nextStatus = GianHangBooking_cl.TrangThaiHoanThanh;
                notice = "Đã chuyển lịch hẹn #" + bookingId.ToString() + " sang trạng thái hoàn thành.";
                break;
            case "cancel":
                nextStatus = GianHangBooking_cl.TrangThaiHuy;
                notice = "Đã hủy lịch hẹn #" + bookingId.ToString() + ".";
                break;
            default:
                return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            if (!GianHangBooking_cl.UpdateStatus(db, bookingId, gianHangTaiKhoan, nextStatus))
                return;
        }

        Session["gianhang_booking_notice"] = notice;
        Response.Redirect(BuildReturnUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        string gianHangAccount = info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(gianHangAccount))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb gianHang = RootAccount_cl.GetByAccountKey(db, gianHangAccount);
            if (gianHang == null)
                return;

            lb_gianhang_name.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(gianHang.ten_shop) ? gianHang.taikhoan : gianHang.ten_shop.Trim());

            IQueryable<GH_DatLich_tb> query = GianHangBooking_cl.QueryByStorefront(db, gianHangAccount);

            string search = (txt_search.Text ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = "%" + search + "%";
                query = query.Where(p =>
                    SqlMethods.Like(p.ten_khach ?? "", keyword) ||
                    SqlMethods.Like(p.sdt ?? "", keyword) ||
                    SqlMethods.Like(p.dich_vu ?? "", keyword));
            }

            string status = (ddl_status.SelectedValue ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => (p.trang_thai ?? "").Trim() == status);

            List<GH_DatLich_tb> bookings = query
                .OrderByDescending(p => p.ngay_tao)
                .ThenByDescending(p => p.id)
                .ToList();

            int total = bookings.Count;
            int pending = 0;
            int confirmed = 0;
            int done = 0;
            foreach (GH_DatLich_tb booking in bookings)
            {
            string trangThaiLichHen = (booking.trang_thai ?? "").Trim();
            if (trangThaiLichHen == GianHangBooking_cl.TrangThaiChoXacNhan || trangThaiLichHen == "")
                pending++;
            else if (trangThaiLichHen == GianHangBooking_cl.TrangThaiDaXacNhan)
                confirmed++;
            else if (trangThaiLichHen == GianHangBooking_cl.TrangThaiHoanThanh)
                done++;
        }

            lb_total.Text = total.ToString("#,##0");
            lb_pending.Text = pending.ToString("#,##0");
            lb_confirmed.Text = confirmed.ToString("#,##0");
            lb_done.Text = done.ToString("#,##0");

            ph_empty.Visible = bookings.Count == 0;

            List<object> views = new List<object>(bookings.Count);
            foreach (GH_DatLich_tb booking in bookings)
            {
                string trangThaiLichHen = string.IsNullOrWhiteSpace(booking.trang_thai)
                    ? GianHangBooking_cl.TrangThaiChoXacNhan
                    : booking.trang_thai.Trim();

                views.Add(new
                {
                    booking.id,
                    booking.ten_khach,
                    booking.sdt,
                    dich_vu = GianHangBooking_cl.ResolveServiceName(booking),
                    thoi_gian_hen_text = FormatDateTime(booking.thoi_gian_hen),
                    trang_thai = trangThaiLichHen,
                    status_css = ResolveStatusCss(trangThaiLichHen),
                    ghi_chu_hien_thi = string.IsNullOrWhiteSpace(booking.ghi_chu) ? "--" : booking.ghi_chu,
                    show_confirm = CanShowConfirm(trangThaiLichHen),
                    show_done = CanShowDone(trangThaiLichHen),
                    show_cancel = CanShowCancel(trangThaiLichHen),
                    url_confirm = BuildActionUrl("confirm", booking.id),
                    url_done = BuildActionUrl("done", booking.id),
                    url_cancel = BuildActionUrl("cancel", booking.id)
                });
            }

            rp_bookings.DataSource = views;
            rp_bookings.DataBind();
        }

        string notice = (Session["gianhang_booking_notice"] ?? "").ToString();
        ph_notice.Visible = !string.IsNullOrWhiteSpace(notice);
        lb_notice.Text = notice;
        Session["gianhang_booking_notice"] = "";
    }

    protected void btn_filter_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildReturnUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void btn_reset_Click(object sender, EventArgs e)
    {
        Response.Redirect(GianHangRoutes_cl.BuildBookingManagementUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void ApplyQueryFilter()
    {
        if (txt_search != null)
            txt_search.Text = (Request.QueryString["q"] ?? "").Trim();

        if (ddl_status != null)
        {
            string status = (Request.QueryString["status"] ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(status))
            {
                System.Web.UI.WebControls.ListItem item = ddl_status.Items.FindByValue(status);
                if (item != null)
                {
                    ddl_status.ClearSelection();
                    item.Selected = true;
                }
            }
        }
    }

    private string BuildReturnUrl()
    {
        string query = (txt_search.Text ?? "").Trim();
        string status = ddl_status == null ? "" : (ddl_status.SelectedValue ?? "").Trim();

        List<string> parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(query))
            parts.Add("q=" + HttpUtility.UrlEncode(query));
        if (!string.IsNullOrWhiteSpace(status))
            parts.Add("status=" + HttpUtility.UrlEncode(status));

        if (parts.Count == 0)
            return GianHangRoutes_cl.BuildBookingManagementUrl();

        return GianHangRoutes_cl.BuildBookingManagementUrl() + "?" + string.Join("&", parts.ToArray());
    }

    private string BuildActionUrl(string action, long id)
    {
        string baseUrl = BuildReturnUrl();
        string separator = baseUrl.Contains("?") ? "&" : "?";
        return baseUrl + separator + "action=" + HttpUtility.UrlEncode(action) + "&id=" + id.ToString();
    }

    private static string FormatDateTime(DateTime? value)
    {
        if (!value.HasValue)
            return "--";
        return value.Value.ToString("dd/MM/yyyy HH:mm", ViCulture);
    }

    private static string ResolveStatusCss(string status)
    {
        string value = (status ?? "").Trim();
        if (value == GianHangBooking_cl.TrangThaiDaXacNhan)
            return "gh-booking-status gh-booking-status-confirmed";
        if (value == GianHangBooking_cl.TrangThaiHoanThanh)
            return "gh-booking-status gh-booking-status-done";
        if (value == GianHangBooking_cl.TrangThaiHuy)
            return "gh-booking-status gh-booking-status-cancelled";
        return "gh-booking-status gh-booking-status-pending";
    }

    private static bool CanShowConfirm(string status)
    {
        string value = (status ?? "").Trim();
        return value == "" || value == GianHangBooking_cl.TrangThaiChoXacNhan;
    }

    private static bool CanShowDone(string status)
    {
        return string.Equals((status ?? "").Trim(), GianHangBooking_cl.TrangThaiDaXacNhan, StringComparison.OrdinalIgnoreCase);
    }

    private static bool CanShowCancel(string status)
    {
        string value = (status ?? "").Trim();
        return value != GianHangBooking_cl.TrangThaiHoanThanh && value != GianHangBooking_cl.TrangThaiHuy;
    }
}
