using System;
using System.Linq;

public partial class gianhang_admin_gianhang_trang_cong_khai : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string WorkspaceDisplayName = "Không gian /gianhang";
    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string PublicUrl = "/gianhang/public.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string CustomersUrl = "/gianhang/admin/gianhang/khach-hang.aspx";
    public string BookingsUrl = "/gianhang/admin/gianhang/lich-hen.aspx";
    public string ProductsUrl = "/gianhang/page/danh-sach-san-pham.aspx";
    public string ServicesUrl = "/gianhang/page/danh-sach-dich-vu.aspx";
    public string ArticlesUrl = "/gianhang/page/danh-sach-bai-viet.aspx";
    public int ProductCount;
    public int ServiceCount;
    public int ArticleCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        PublicUrl = GianHangRoutes_cl.BuildPublicUrl(ownerAccountKey);
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        CustomersUrl = GianHangRoutes_cl.BuildAdminWorkspaceCustomersUrl();
        BookingsUrl = GianHangRoutes_cl.BuildAdminWorkspaceBookingsUrl();
        ProductsUrl = GianHangRoutes_cl.BuildProductsUrl(ownerAccountKey);
        ServicesUrl = GianHangRoutes_cl.BuildServicesUrl(ownerAccountKey);
        ArticlesUrl = GianHangRoutes_cl.BuildArticlesUrl(ownerAccountKey);

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccountKey);
        string display = owner == null ? string.Empty : ((owner.ten_shop ?? "").Trim());
        if (display == "")
            display = owner == null ? string.Empty : ((owner.hoten ?? "").Trim());
        if (display == "")
            display = ownerAccountKey;
        WorkspaceDisplayName = display;

        IQueryable<GH_SanPham_tb> allContent = GianHangProduct_cl.QueryByStorefront(db, ownerAccountKey);
        ProductCount = allContent.Count(p => (p.loai ?? "").Trim().ToLower() == GianHangProduct_cl.LoaiSanPham && (p.bin == null || p.bin == false));
        ServiceCount = allContent.Count(p => (p.loai ?? "").Trim().ToLower() == GianHangProduct_cl.LoaiDichVu && (p.bin == null || p.bin == false));

        ArticleCount = GianHangArticle_cl.QueryPublicByChiNhanh(db, access.ChiNhanhId, ownerAccountKey).Count();
    }
}
