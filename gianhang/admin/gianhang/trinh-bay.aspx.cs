using System;
using System.Linq;

public partial class gianhang_admin_gianhang_trinh_bay : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string PublicUrl = "/gianhang/public.aspx";
    public string PublicPreviewUrl = "/gianhang/admin/gianhang/trang-cong-khai.aspx";
    public string StorefrontPreviewUrl = "/gianhang/admin/gianhang/trang-cong-khai.aspx";
    public string StorefrontConfigUrl = "/gianhang/admin/cau-hinh-storefront/default.aspx";
    public string MenuUrl = "/gianhang/admin/quan-ly-menu/Default.aspx";
    public string SliderUrl = "/gianhang/admin/quan-ly-module/slide-anh/default.aspx";
    public string BrandSettingsUrl = "/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx";
    public string SocialSettingsUrl = "/gianhang/admin/cau-hinh-chung/link-social-media.aspx";
    public string EmbedSettingsUrl = "/gianhang/admin/cau-hinh-chung/nhung-ma-vao-website.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string ArticleHubUrl = "/gianhang/admin/gianhang/bai-viet.aspx";
    public string ArticleListUrl = "/gianhang/page/danh-sach-bai-viet.aspx";

    public int MenuCount;
    public int SliderCount;
    public int VisibleSectionCount;
    public int TotalSectionCount;
    public int ArticleCount;
    public string StorefrontModeLabel = "Tự động";
    public string BrandNoteStatus = "Chưa có";

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q1_3");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        PublicUrl = GianHangRoutes_cl.BuildPublicUrl(ownerAccountKey);
        PublicPreviewUrl = GianHangRoutes_cl.BuildAdminWorkspaceStorefrontUrl();
        StorefrontPreviewUrl = PublicPreviewUrl;
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        ArticleHubUrl = GianHangRoutes_cl.BuildAdminWorkspaceArticlesUrl();
        ArticleListUrl = GianHangRoutes_cl.BuildArticlesUrl(ownerAccountKey);

        string chiNhanhId = (access.ChiNhanhId ?? "").Trim();
        GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);

        MenuCount = db.web_menu_tables.Count(p => p.id_chinhanh == chiNhanhId);
        SliderCount = db.web_module_slider_tables.Count();
        VisibleSectionCount = GianHangStorefrontConfig_cl.GetVisibleSections(db, chiNhanhId).Count;
        TotalSectionCount = GianHangStorefrontConfig_cl.GetAllSections(db, chiNhanhId).Count;
        ArticleCount = GianHangArticle_cl.QueryPublicByChiNhanh(db, chiNhanhId, ownerAccountKey).Count();

        gianhang_storefront_config_table config = GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);
        string mode = (config.storefront_mode ?? "").Trim().ToLowerInvariant();
        if (mode == GianHangStorefrontConfig_cl.ModeHybrid)
            StorefrontModeLabel = "Kết hợp";
        else if (mode == GianHangStorefrontConfig_cl.ModeRetail)
            StorefrontModeLabel = "Bán lẻ";
        else if (mode == GianHangStorefrontConfig_cl.ModeService)
            StorefrontModeLabel = "Dịch vụ";
        else
            StorefrontModeLabel = "Tự động";

        BrandNoteStatus = string.IsNullOrWhiteSpace(config.brand_note) ? "Chưa có" : "Đã có";
    }
}
