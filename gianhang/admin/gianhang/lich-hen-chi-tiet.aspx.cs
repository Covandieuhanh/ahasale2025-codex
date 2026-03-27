using System;
using System.Linq;

public partial class gianhang_admin_gianhang_lich_hen_chi_tiet : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string PersonHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
    public string AdminDetailUrl = "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx";
    public string NativeBookingsUrl = "/gianhang/quan-ly-lich-hen.aspx";
    public string PublicServiceUrl = "/gianhang/page/danh-sach-dich-vu.aspx";

    public string CustomerName = "Lịch hẹn /gianhang";
    public string PhoneText = "--";
    public string ServiceName = "--";
    public string ScheduleText = "--";
    public string CreatedAtText = "--";
    public string StatusText = "--";
    public string StatusCss = "gh-wb-badge gh-wb-badge--waiting";
    public string NoteText = "Chưa có ghi chú.";
    public string MirrorText = "Chưa mirror";
    public bool HasDetail;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q9_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        NativeBookingsUrl = GianHangRoutes_cl.BuildBookingManagementUrl();

        long nativeId;
        if (!long.TryParse((Request.QueryString["id"] ?? "").Trim(), out nativeId) || nativeId <= 0)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        GH_DatLich_tb booking = GianHangBooking_cl.QueryByStorefront(db, ownerAccountKey).FirstOrDefault(p => p.id == nativeId);
        if (booking == null)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        ph_empty.Visible = false;
        ph_content.Visible = true;
        HasDetail = true;

        CustomerName = string.IsNullOrWhiteSpace(booking.ten_khach) ? "Khách đặt lịch" : booking.ten_khach.Trim();
        PhoneText = string.IsNullOrWhiteSpace(booking.sdt) ? "--" : booking.sdt.Trim();
        ServiceName = GianHangBooking_cl.ResolveServiceName(booking);
        ScheduleText = GianHangReport_cl.FormatDateTime(booking.thoi_gian_hen);
        CreatedAtText = GianHangReport_cl.FormatDateTime(booking.ngay_tao);
        StatusText = string.IsNullOrWhiteSpace(booking.trang_thai) ? GianHangBooking_cl.TrangThaiChoXacNhan : booking.trang_thai.Trim();
        StatusCss = ResolveStatusCss(StatusText);
        NoteText = string.IsNullOrWhiteSpace(booking.ghi_chu) ? "Chưa có ghi chú." : booking.ghi_chu.Trim();

        long legacyId = GianHangWorkspaceLink_cl.ResolveLegacyBookingId(db, ownerAccountKey, booking.id);
        if (legacyId > 0)
        {
            AdminDetailUrl = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + legacyId.ToString();
            MirrorText = "Lịch hẹn admin #" + legacyId.ToString();
        }

        string personKeyword = PhoneText != "--" ? PhoneText : CustomerName;
        PersonHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx?keyword=" + Server.UrlEncode(personKeyword);
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl() + "?keyword=" + Server.UrlEncode(personKeyword);

        if (booking.id_dichvu.HasValue && booking.id_dichvu.Value > 0)
            PublicServiceUrl = GianHangRoutes_cl.AppendUserToUrl(GianHangRoutes_cl.BuildXemDichVuUrl(booking.id_dichvu.Value), ownerAccountKey);
    }

    private string ResolveStatusCss(string status)
    {
        if (status == GianHangBooking_cl.TrangThaiDaXacNhan)
            return "gh-wb-badge gh-wb-badge--active";
        if (status == GianHangBooking_cl.TrangThaiHoanThanh)
            return "gh-wb-badge gh-wb-badge--success";
        if (status == GianHangBooking_cl.TrangThaiHuy)
            return "gh-wb-badge gh-wb-badge--muted";
        return "gh-wb-badge gh-wb-badge--waiting";
    }
}
