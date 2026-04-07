using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class gianhang_admin_gianhang_lich_hen : System.Web.UI.Page
{
    private sealed class BookingRowView
    {
        public long NativeId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string ServiceName { get; set; }
        public string CreatedAtText { get; set; }
        public string ScheduleText { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public bool HasAdminMirror { get; set; }
        public string AdminDetailUrl { get; set; }
        public string WorkspaceDetailUrl { get; set; }
        public string AdminMirrorText { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string NativeBookingsUrl = "/gianhang/quan-ly-lich-hen.aspx";
    public string AdminBookingsUrl = "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string SyncUrl = "/gianhang/admin/gianhang/lich-hen.aspx?sync=1";

    public int TotalBookings;
    public int PendingCount;
    public int ConfirmedCount;
    public int DoneCount;
    public int CancelledCount;
    public int MirroredCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q9_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        if (string.Equals((Request.QueryString["sync"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase))
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey, true);
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        NativeBookingsUrl = GianHangRoutes_cl.BuildBookingManagementUrl();
        AdminBookingsUrl = GianHangRoutes_cl.BuildAdminLegacyBookingsUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl() + "?sync=1";

        if (!IsPostBack)
        {
            txt_search.Text = (Request.QueryString["keyword"] ?? "").Trim();
            string status = (Request.QueryString["status"] ?? "all").Trim();
            if (ddl_status.Items.FindByValue(status) != null)
                ddl_status.SelectedValue = status;
        }

        BindRows(ownerAccountKey);
    }

    protected void btn_filter_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(false), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void btn_clear_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(true), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void BindRows(string ownerAccountKey)
    {
        string keyword = (txt_search.Text ?? "").Trim().ToLowerInvariant();
        string status = (ddl_status.SelectedValue ?? "all").Trim();

        List<GH_DatLich_tb> rows = GianHangBooking_cl.QueryByStorefront(db, ownerAccountKey)
            .OrderByDescending(p => p.ngay_tao ?? p.thoi_gian_hen)
            .ThenByDescending(p => p.id)
            .Take(240)
            .ToList();

        if (keyword != "")
        {
            rows = rows.Where(p =>
                ((p.ten_khach ?? "").Trim().ToLowerInvariant().Contains(keyword))
                || ((p.sdt ?? "").Trim().ToLowerInvariant().Contains(keyword))
                || ((p.dich_vu ?? "").Trim().ToLowerInvariant().Contains(keyword))
                || p.id.ToString().Contains(keyword)).ToList();
        }

        if (status != "all")
            rows = rows.Where(p => string.Equals((p.trang_thai ?? GianHangBooking_cl.TrangThaiChoXacNhan).Trim(), status, StringComparison.OrdinalIgnoreCase)).ToList();

        List<BookingRowView> viewRows = rows.Select(item =>
        {
            long legacyId = GianHangWorkspaceLink_cl.ResolveLegacyBookingId(db, ownerAccountKey, item.id);
            string statusText = string.IsNullOrWhiteSpace(item.trang_thai) ? GianHangBooking_cl.TrangThaiChoXacNhan : item.trang_thai.Trim();
            return new BookingRowView
            {
                NativeId = item.id,
                CustomerName = string.IsNullOrWhiteSpace(item.ten_khach) ? "Khách đặt lịch" : item.ten_khach.Trim(),
                Phone = string.IsNullOrWhiteSpace(item.sdt) ? "--" : item.sdt.Trim(),
                ServiceName = GianHangBooking_cl.ResolveServiceName(item),
                CreatedAtText = GianHangReport_cl.FormatDateTime(item.ngay_tao),
                ScheduleText = GianHangReport_cl.FormatDateTime(item.thoi_gian_hen),
                StatusText = statusText,
                StatusCss = ResolveStatusCss(statusText),
                HasAdminMirror = legacyId > 0,
                AdminDetailUrl = legacyId > 0 ? GianHangRoutes_cl.BuildAdminLegacyBookingDetailUrl(legacyId) : AdminBookingsUrl,
                WorkspaceDetailUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingDetailUrl(item.id),
                AdminMirrorText = legacyId > 0 ? ("Lịch hẹn admin #" + legacyId.ToString()) : "Chưa mirror"
            };
        }).ToList();

        TotalBookings = viewRows.Count;
        PendingCount = viewRows.Count(p => p.StatusText == GianHangBooking_cl.TrangThaiChoXacNhan);
        ConfirmedCount = viewRows.Count(p => p.StatusText == GianHangBooking_cl.TrangThaiDaXacNhan);
        DoneCount = viewRows.Count(p => p.StatusText == GianHangBooking_cl.TrangThaiHoanThanh);
        CancelledCount = viewRows.Count(p => p.StatusText == GianHangBooking_cl.TrangThaiHuy);
        MirroredCount = viewRows.Count(p => p.HasAdminMirror);

        ph_empty.Visible = viewRows.Count == 0;
        rp_rows.DataSource = viewRows;
        rp_rows.DataBind();
    }

    private string ResolveStatusCss(string statusText)
    {
        if (statusText == GianHangBooking_cl.TrangThaiDaXacNhan)
            return "gh-admin-bookings__badge gh-admin-bookings__badge--confirmed";
        if (statusText == GianHangBooking_cl.TrangThaiHoanThanh)
            return "gh-admin-bookings__badge gh-admin-bookings__badge--done";
        if (statusText == GianHangBooking_cl.TrangThaiHuy)
            return "gh-admin-bookings__badge gh-admin-bookings__badge--cancelled";
        return "gh-admin-bookings__badge gh-admin-bookings__badge--pending";
    }

    private string BuildFilterUrl(bool clear)
    {
        if (clear)
            return GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();

        List<string> parts = new List<string>();
        string keyword = (txt_search.Text ?? "").Trim();
        string status = (ddl_status.SelectedValue ?? "all").Trim();
        if (keyword != "")
            parts.Add("keyword=" + Server.UrlEncode(keyword));
        if (status != "" && status != "all")
            parts.Add("status=" + Server.UrlEncode(status));
        return parts.Count == 0 ? GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl() : (GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl() + "?" + string.Join("&", parts.ToArray()));
    }
}
