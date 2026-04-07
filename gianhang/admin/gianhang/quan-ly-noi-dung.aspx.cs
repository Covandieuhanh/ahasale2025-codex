using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

public partial class gianhang_admin_gianhang_quan_ly_noi_dung : System.Web.UI.Page
{
    private sealed class ContentRowView
    {
        public int NativeId { get; set; }
        public string Name { get; set; }
        public string TypeLabel { get; set; }
        public string TypeCss { get; set; }
        public string PriceText { get; set; }
        public string VisibilityText { get; set; }
        public string VisibilityCss { get; set; }
        public string UpdatedAtText { get; set; }
        public string ImageUrl { get; set; }
        public string NativeEditUrl { get; set; }
        public string PublicUrl { get; set; }
        public string LegacyEditUrl { get; set; }
        public bool HasLegacyMirror { get; set; }
        public string LegacyMirrorText { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string NativeManageUrl = "/gianhang/quan-ly-tin/Default.aspx";
    public string NativeCreateUrl = "/gianhang/quan-ly-tin/Them.aspx";
    public string AdminContentUrl = "/gianhang/admin/quan-ly-bai-viet/Default.aspx";
    public string ArticleHubUrl = "/gianhang/admin/gianhang/bai-viet.aspx";
    public string ProductHubUrl = "/gianhang/admin/gianhang/san-pham.aspx";
    public string ServiceHubUrl = "/gianhang/admin/gianhang/dich-vu.aspx";
    public string PublicProductsUrl = "/gianhang/page/danh-sach-san-pham.aspx";
    public string PublicServicesUrl = "/gianhang/page/danh-sach-dich-vu.aspx";
    public string SyncUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx?sync=1";

    public int TotalItems;
    public int ProductCount;
    public int ServiceCount;
    public int HiddenCount;
    public int MirroredCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q4_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        if (string.Equals((Request.QueryString["sync"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase))
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerAccountKey, true);
            Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceContentUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        NativeManageUrl = GianHangRoutes_cl.BuildQuanLyTinUrl();
        NativeCreateUrl = "/gianhang/quan-ly-tin/Them.aspx";
        AdminContentUrl = GianHangRoutes_cl.BuildAdminLegacyArticlesUrl(string.Empty);
        ArticleHubUrl = GianHangRoutes_cl.BuildAdminWorkspaceArticlesUrl();
        ProductHubUrl = GianHangRoutes_cl.BuildAdminWorkspaceProductsUrl();
        ServiceHubUrl = GianHangRoutes_cl.BuildAdminWorkspaceServicesUrl();
        PublicProductsUrl = GianHangRoutes_cl.BuildProductsUrl(ownerAccountKey);
        PublicServicesUrl = GianHangRoutes_cl.BuildServicesUrl(ownerAccountKey);
        SyncUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl() + "?sync=1";

        if (!IsPostBack)
        {
            string type = (Request.QueryString["type"] ?? "all").Trim().ToLowerInvariant();
            if (ddl_type.Items.FindByValue(type) != null)
                ddl_type.SelectedValue = type;
            txt_search.Text = (Request.QueryString["keyword"] ?? "").Trim();
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
        string keyword = (txt_search.Text ?? "").Trim();
        string type = (ddl_type.SelectedValue ?? "all").Trim().ToLowerInvariant();

        IQueryable<GH_SanPham_tb> query = GianHangProduct_cl.QueryByStorefront(db, ownerAccountKey);
        TotalItems = query.Count();
        ProductCount = query.Count(p => (p.loai ?? "").Trim().ToLower() == GianHangProduct_cl.LoaiSanPham);
        ServiceCount = query.Count(p => (p.loai ?? "").Trim().ToLower() == GianHangProduct_cl.LoaiDichVu);
        HiddenCount = query.Count(p => p.bin == true);

        if (type == GianHangProduct_cl.LoaiSanPham || type == GianHangProduct_cl.LoaiDichVu)
            query = query.Where(p => p.loai == type);
        else if (type == "hidden")
            query = query.Where(p => p.bin == true);
        else if (type == "active")
            query = query.Where(p => p.bin == null || p.bin == false);

        int numericKeyword;
        bool hasNumericKeyword = int.TryParse(keyword, out numericKeyword);
        if (keyword != "")
        {
            query = query.Where(p =>
                (p.ten ?? "").Contains(keyword)
                || (p.mo_ta ?? "").Contains(keyword)
                || (hasNumericKeyword && p.id == numericKeyword));
        }

        List<GH_SanPham_tb> items = query
            .OrderByDescending(p => p.ngay_cap_nhat ?? p.ngay_tao)
            .ThenByDescending(p => p.id)
            .Take(160)
            .ToList();

        List<ContentRowView> rows = items.Select(item =>
        {
            long legacyId = GianHangWorkspaceLink_cl.ResolveLegacyProductId(db, ownerAccountKey, item.id);
            bool isService = string.Equals((item.loai ?? "").Trim(), GianHangProduct_cl.LoaiDichVu, StringComparison.OrdinalIgnoreCase);
            return new ContentRowView
            {
                NativeId = item.id,
                Name = string.IsNullOrWhiteSpace(item.ten) ? (isService ? "Dịch vụ chưa đặt tên" : "Sản phẩm chưa đặt tên") : item.ten.Trim(),
                TypeLabel = isService ? "Dịch vụ" : "Sản phẩm",
                TypeCss = isService ? "gh-admin-content__badge gh-admin-content__badge--service" : "gh-admin-content__badge gh-admin-content__badge--product",
                PriceText = (item.gia_ban ?? 0m) <= 0m ? "Liên hệ" : (item.gia_ban ?? 0m).ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) + " đ",
                VisibilityText = item.bin == true ? "Đang ẩn" : "Đang hiển thị",
                VisibilityCss = item.bin == true ? "gh-admin-content__badge gh-admin-content__badge--hidden" : "gh-admin-content__badge gh-admin-content__badge--visible",
                UpdatedAtText = (item.ngay_cap_nhat ?? item.ngay_tao).HasValue ? (item.ngay_cap_nhat ?? item.ngay_tao).Value.ToString("dd/MM/yyyy HH:mm") : "--",
                ImageUrl = GianHangStorefront_cl.ResolveImageUrl(item.hinh_anh),
                NativeEditUrl = "/gianhang/quan-ly-tin/Them.aspx?id=" + item.id.ToString(),
                PublicUrl = isService ? GianHangRoutes_cl.BuildXemDichVuUrl(item.id) : GianHangRoutes_cl.BuildXemSanPhamUrl(item.id),
                LegacyEditUrl = legacyId > 0 ? GianHangRoutes_cl.BuildAdminLegacyArticleEditUrl(legacyId, isService ? "dv" : "sp") : AdminContentUrl,
                HasLegacyMirror = legacyId > 0,
                LegacyMirrorText = legacyId > 0 ? ("Đã mirror #" + legacyId.ToString()) : "Chưa mirror"
            };
        }).ToList();

        MirroredCount = rows.Count(p => p.HasLegacyMirror);
        ph_empty.Visible = rows.Count == 0;
        rp_rows.DataSource = rows;
        rp_rows.DataBind();
    }

    private string BuildFilterUrl(bool clear)
    {
        if (clear)
            return GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();

        List<string> parts = new List<string>();
        string keyword = (txt_search.Text ?? "").Trim();
        string type = (ddl_type.SelectedValue ?? "all").Trim().ToLowerInvariant();
        if (keyword != "")
            parts.Add("keyword=" + Server.UrlEncode(keyword));
        if (type != "" && type != "all")
            parts.Add("type=" + Server.UrlEncode(type));
        return parts.Count == 0 ? GianHangRoutes_cl.BuildAdminWorkspaceContentUrl() : (GianHangRoutes_cl.BuildAdminWorkspaceContentUrl() + "?" + string.Join("&", parts.ToArray()));
    }
}
