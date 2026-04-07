using System;

public partial class gianhang_admin_gianhang_dich_vu_chi_tiet : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string ServicesUrl = "/gianhang/admin/gianhang/dich-vu.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string ItemName = "Dịch vụ /gianhang";
    public string DescriptionText = "Chưa có mô tả ngắn.";
    public string ContentHtml = "Chưa có nội dung chi tiết.";
    public string ImageUrl = "/images/no_image.png";
    public string PriceText = "Liên hệ";
    public string CategoryText = "Chưa gắn danh mục";
    public string VisibilityText = "Đang hiển thị";
    public string VisibilityCss = "gh-item-badge gh-item-badge--visible";
    public string BridgeText = "Chưa bridge sang admin";
    public string StockText = "Không theo tồn kho";
    public string DiscountText = "Chưa đặt ưu đãi";
    public string UpdatedAtText = "--";
    public string NativeEditUrl = "/gianhang/quan-ly-tin/Default.aspx";
    public string AdminEditUrl = "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv";
    public string PublicDetailUrl = "/gianhang/page/danh-sach-dich-vu.aspx";
    public string BookingUrl = "/gianhang/datlich.aspx";

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q4_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        ServicesUrl = GianHangRoutes_cl.BuildAdminWorkspaceServicesUrl();
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        AdminEditUrl = GianHangRoutes_cl.BuildAdminLegacyArticlesUrl("dv");

        int nativeId;
        if (!int.TryParse((Request.QueryString["id"] ?? string.Empty).Trim(), out nativeId) || nativeId <= 0)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        GianHangAdminWorkspaceCatalog_cl.CatalogDetail detail = GianHangAdminWorkspaceCatalog_cl.LoadDetail(db, ownerAccountKey, GianHangProduct_cl.LoaiDichVu, nativeId);
        if (detail == null)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        ph_empty.Visible = false;
        ph_content.Visible = true;

        ItemName = detail.Name;
        DescriptionText = detail.Description;
        ContentHtml = string.IsNullOrWhiteSpace(detail.ContentHtml) ? "Chưa có nội dung chi tiết." : detail.ContentHtml;
        ImageUrl = detail.ImageUrl;
        PriceText = detail.PriceText;
        CategoryText = detail.CategoryText;
        VisibilityText = detail.VisibilityText;
        VisibilityCss = detail.VisibilityCss;
        BridgeText = detail.BridgeText;
        StockText = detail.StockText;
        DiscountText = detail.DiscountText;
        UpdatedAtText = detail.UpdatedAtText;
        NativeEditUrl = detail.NativeEditUrl;
        AdminEditUrl = detail.AdminEditUrl;
        PublicDetailUrl = detail.PublicUrl;
        BookingUrl = GianHangRoutes_cl.BuildDatLichPublicUrl(detail.NativeId, ownerAccountKey, Request.RawUrl ?? GianHangRoutes_cl.BuildAdminWorkspaceServiceDetailUrl(detail.NativeId));
    }
}
