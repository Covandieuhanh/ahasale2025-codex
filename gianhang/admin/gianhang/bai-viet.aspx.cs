using System;
using System.Collections.Generic;
using System.Linq;

public partial class gianhang_admin_gianhang_bai_viet : System.Web.UI.Page
{
    private sealed class ArticleRowView
    {
        public long ArticleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VisibilityText { get; set; }
        public string VisibilityCss { get; set; }
        public string MirrorText { get; set; }
        public string UpdatedAtText { get; set; }
        public string AdminEditUrl { get; set; }
        public string PublicDetailUrl { get; set; }
        public string PublicListUrl { get; set; }
        public string WorkspaceDetailUrl { get; set; }
        public bool HasMirror { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();

    public string AdminArticleUrl = "/gianhang/admin/quan-ly-bai-viet/Default.aspx";
    public string PublicArticlesUrl = "/gianhang/page/danh-sach-bai-viet.aspx";
    public string StorefrontPreviewUrl = "/gianhang/admin/gianhang/trang-cong-khai.aspx";

    public int TotalArticles;
    public int VisibleArticles;
    public int HiddenArticles;
    public int MirroredArticles;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q4_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        AdminArticleUrl = GianHangRoutes_cl.BuildAdminLegacyArticlesUrl(string.Empty);
        PublicArticlesUrl = GianHangRoutes_cl.BuildArticlesUrl(ownerAccountKey);
        StorefrontPreviewUrl = GianHangRoutes_cl.BuildAdminWorkspaceStorefrontUrl();

        if (!IsPostBack)
            txt_search.Text = (Request.QueryString["keyword"] ?? "").Trim();

        BindRows(access, ownerAccountKey);
    }

    protected void btn_filter_Click(object sender, EventArgs e)
    {
        string keyword = (txt_search.Text ?? "").Trim();
        string url = GianHangRoutes_cl.BuildAdminWorkspaceArticlesUrl();
        if (keyword != "")
            url += "?keyword=" + Server.UrlEncode(keyword);
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void btn_clear_Click(object sender, EventArgs e)
    {
        Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceArticlesUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void BindRows(GianHangAdminPageGuard_cl.AccessInfo access, string ownerAccountKey)
    {
        string keyword = (txt_search.Text ?? "").Trim().ToLowerInvariant();
        List<GH_BaiViet_tb> source = GianHangArticle_cl.QueryPublicByChiNhanh(db, access.ChiNhanhId, ownerAccountKey)
            .OrderByDescending(p => p.ngay_cap_nhat ?? p.ngaytao)
            .ThenByDescending(p => p.id)
            .Take(160)
            .ToList();

        TotalArticles = source.Count;
        VisibleArticles = source.Count(p => p.hienthi == null || p.hienthi == true);
        HiddenArticles = source.Count(p => p.hienthi == false || p.bin == true);

        if (keyword != "")
        {
            source = source.Where(p =>
                ((p.name ?? "").Trim().ToLowerInvariant().Contains(keyword))
                || ((p.description ?? "").Trim().ToLowerInvariant().Contains(keyword))
                || p.id.ToString() == keyword
                || ((p.legacy_post_id.HasValue ? p.legacy_post_id.Value.ToString() : "") == keyword))
                .ToList();
        }

        List<ArticleRowView> rows = source.Select(item =>
        {
            long routeId = GianHangArticle_cl.ResolveRouteId(item);
            bool hasMirror = item.legacy_post_id.HasValue && item.legacy_post_id.Value > 0;
            return new ArticleRowView
            {
                ArticleId = routeId,
                Name = string.IsNullOrWhiteSpace(item.name) ? "Bài viết chưa đặt tên" : item.name.Trim(),
                Description = string.IsNullOrWhiteSpace(item.description) ? "Chưa có mô tả ngắn." : item.description.Trim(),
                ImageUrl = GianHangStorefront_cl.ResolveImageUrl(item.image),
                VisibilityText = (item.hienthi == false || item.bin == true) ? "Đang ẩn" : "Đang hiển thị",
                VisibilityCss = (item.hienthi == false || item.bin == true) ? "gh-article-badge gh-article-badge--hidden" : "gh-article-badge gh-article-badge--visible",
                MirrorText = hasMirror ? ("Đã mirror #" + item.legacy_post_id.Value.ToString()) : "Đang dùng native record",
                UpdatedAtText = (item.ngay_cap_nhat ?? item.ngaytao).HasValue ? (item.ngay_cap_nhat ?? item.ngaytao).Value.ToString("dd/MM/yyyy HH:mm") : "--",
                AdminEditUrl = hasMirror ? GianHangRoutes_cl.BuildAdminLegacyArticleEditUrl(item.legacy_post_id.Value, string.Empty) : AdminArticleUrl,
                PublicDetailUrl = GianHangArticle_cl.BuildDetailUrl(routeId, ownerAccountKey),
                PublicListUrl = PublicArticlesUrl,
                WorkspaceDetailUrl = GianHangRoutes_cl.BuildAdminWorkspaceArticleDetailUrl(routeId),
                HasMirror = hasMirror
            };
        }).ToList();

        MirroredArticles = rows.Count(p => p.HasMirror);
        ph_empty.Visible = rows.Count == 0;
        rp_rows.DataSource = rows;
        rp_rows.DataBind();
    }
}
