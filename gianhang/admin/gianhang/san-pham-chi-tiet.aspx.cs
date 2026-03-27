using System;

public partial class gianhang_admin_gianhang_san_pham_chi_tiet : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string ProductsUrl = "/gianhang/admin/gianhang/san-pham.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string ItemName = "Sản phẩm /gianhang";
    public string DescriptionText = "Chưa có mô tả ngắn.";
    public string ContentHtml = "Chưa có nội dung chi tiết.";
    public string ImageUrl = "/images/no_image.png";
    public string PriceText = "Liên hệ";
    public string CategoryText = "Chưa gắn danh mục";
    public string VisibilityText = "Đang hiển thị";
    public string VisibilityCss = "gh-item-badge gh-item-badge--visible";
    public string BridgeText = "Chưa bridge sang admin";
    public string StockText = "Tồn: 0";
    public string DiscountText = "Chưa đặt ưu đãi";
    public string UpdatedAtText = "--";
    public string NativeEditUrl = "/gianhang/quan-ly-tin/Default.aspx";
    public string AdminEditUrl = "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp";
    public string PublicDetailUrl = "/gianhang/page/danh-sach-san-pham.aspx";
    public string CreateOrderUrl = "/gianhang/don-ban.aspx?taodon=1";

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q4_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        ProductsUrl = GianHangRoutes_cl.BuildAdminWorkspaceProductsUrl();
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();

        int nativeId;
        if (!int.TryParse((Request.QueryString["id"] ?? string.Empty).Trim(), out nativeId) || nativeId <= 0)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        GianHangAdminWorkspaceCatalog_cl.CatalogDetail detail = GianHangAdminWorkspaceCatalog_cl.LoadDetail(db, ownerAccountKey, GianHangProduct_cl.LoaiSanPham, nativeId);
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
        CreateOrderUrl = GianHangRoutes_cl.BuildTaoDonUrl(detail.NativeId, Request.RawUrl ?? GianHangRoutes_cl.BuildAdminWorkspaceProductDetailUrl(detail.NativeId));
    }
}
