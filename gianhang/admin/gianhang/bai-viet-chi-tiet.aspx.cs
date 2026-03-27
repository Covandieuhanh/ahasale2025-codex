using System;

public partial class gianhang_admin_gianhang_bai_viet_chi_tiet : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string ArticlesUrl = "/gianhang/admin/gianhang/bai-viet.aspx";
    public string StorefrontUrl = "/gianhang/admin/gianhang/trang-cong-khai.aspx";
    public string ArticleTitle = "Bài viết /gianhang";
    public string DescriptionText = "Chưa có mô tả ngắn.";
    public string ContentHtml = "Chưa có nội dung chi tiết.";
    public string ImageUrl = "/images/no_image.png";
    public string VisibilityText = "Đang hiển thị";
    public string VisibilityCss = "gh-article-detail-badge gh-article-detail-badge--visible";
    public string MirrorText = "Đang dùng native record";
    public string UpdatedAtText = "--";
    public string AdminEditUrl = "/gianhang/admin/quan-ly-bai-viet/Default.aspx";
    public string PublicDetailUrl = "/gianhang/page/danh-sach-bai-viet.aspx";
    public string PublicListUrl = "/gianhang/page/danh-sach-bai-viet.aspx";
    public string MenuTitle = "Bài viết storefront";

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q4_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        ArticlesUrl = GianHangRoutes_cl.BuildAdminWorkspaceArticlesUrl();
        StorefrontUrl = GianHangRoutes_cl.BuildAdminWorkspaceStorefrontUrl();
        PublicListUrl = GianHangRoutes_cl.BuildArticlesUrl(ownerAccountKey);

        string articleId = (Request.QueryString["id"] ?? string.Empty).Trim();
        if (articleId == string.Empty)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        GH_BaiViet_tb article = GianHangArticle_cl.FindPublicById(db, (access.ChiNhanhId ?? string.Empty).Trim(), articleId, ownerAccountKey);
        if (article == null)
        {
            ph_empty.Visible = true;
            ph_content.Visible = false;
            return;
        }

        ph_empty.Visible = false;
        ph_content.Visible = true;

        ArticleTitle = string.IsNullOrWhiteSpace(article.name) ? "Bài viết /gianhang" : article.name.Trim();
        DescriptionText = string.IsNullOrWhiteSpace(article.description) ? "Chưa có mô tả ngắn." : article.description.Trim();
        ContentHtml = string.IsNullOrWhiteSpace(article.content_post) ? "Chưa có nội dung chi tiết." : article.content_post;
        ImageUrl = GianHangStorefront_cl.ResolveImageUrl(article.image);
        VisibilityText = (article.hienthi == false || article.bin == true) ? "Đang ẩn" : "Đang hiển thị";
        VisibilityCss = (article.hienthi == false || article.bin == true)
            ? "gh-article-detail-badge gh-article-detail-badge--hidden"
            : "gh-article-detail-badge gh-article-detail-badge--visible";
        UpdatedAtText = (article.ngay_cap_nhat ?? article.ngaytao).HasValue ? (article.ngay_cap_nhat ?? article.ngaytao).Value.ToString("dd/MM/yyyy HH:mm") : "--";

        long routeId = GianHangArticle_cl.ResolveRouteId(article);
        PublicDetailUrl = GianHangArticle_cl.BuildDetailUrl(routeId, ownerAccountKey);
        AdminEditUrl = article.legacy_post_id.HasValue && article.legacy_post_id.Value > 0
            ? ("/gianhang/admin/quan-ly-bai-viet/edit.aspx?id=" + article.legacy_post_id.Value.ToString())
            : "/gianhang/admin/quan-ly-bai-viet/Default.aspx";
        MirrorText = article.legacy_post_id.HasValue && article.legacy_post_id.Value > 0
            ? ("Đã mirror #" + article.legacy_post_id.Value.ToString())
            : "Đang dùng native record";

        web_menu_table menu = GianHangMenu_cl.FindById(db, (access.ChiNhanhId ?? string.Empty).Trim(), (article.id_category ?? string.Empty).Trim());
        if (menu != null && !string.IsNullOrWhiteSpace(menu.name))
            MenuTitle = menu.name.Trim();
    }
}
