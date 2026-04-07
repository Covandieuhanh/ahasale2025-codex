using System;

public partial class gianhang_admin_gianhang_san_pham : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string ContentUrl = "/gianhang/admin/gianhang/quan-ly-noi-dung.aspx";
    public string CompanionUrl = "/gianhang/admin/gianhang/dich-vu.aspx";
    public string PublicListUrl = "/gianhang/page/danh-sach-san-pham.aspx";
    public string AdminLegacyUrl = "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp";
    public string NativeManageUrl = "/gianhang/quan-ly-tin/Default.aspx";

    public int TotalCount;
    public int VisibleCount;
    public int HiddenCount;
    public int MirroredCount;
    public int DiscountedCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q4_1");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        ContentUrl = GianHangRoutes_cl.BuildAdminWorkspaceContentUrl();
        CompanionUrl = GianHangRoutes_cl.BuildAdminWorkspaceServicesUrl();
        PublicListUrl = GianHangRoutes_cl.BuildProductsUrl(ownerAccountKey);
        AdminLegacyUrl = GianHangRoutes_cl.BuildAdminLegacyArticlesUrl("sp");
        NativeManageUrl = GianHangRoutes_cl.BuildQuanLyTinUrl();

        if (!IsPostBack)
        {
            txt_search.Text = (Request.QueryString["keyword"] ?? "").Trim();
            string visibility = (Request.QueryString["visibility"] ?? "all").Trim().ToLowerInvariant();
            if (ddl_visibility.Items.FindByValue(visibility) != null)
                ddl_visibility.SelectedValue = visibility;
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
        GianHangAdminWorkspaceCatalog_cl.CatalogPayload payload = GianHangAdminWorkspaceCatalog_cl.LoadByType(
            db,
            ownerAccountKey,
            GianHangProduct_cl.LoaiSanPham,
            txt_search.Text,
            ddl_visibility.SelectedValue,
            180);

        TotalCount = payload.TotalCount;
        VisibleCount = payload.VisibleCount;
        HiddenCount = payload.HiddenCount;
        MirroredCount = payload.MirroredCount;
        DiscountedCount = payload.DiscountedCount;

        ph_empty.Visible = payload.Rows.Count == 0;
        rp_rows.DataSource = payload.Rows;
        rp_rows.DataBind();
    }

    private string BuildFilterUrl(bool clear)
    {
        string baseUrl = GianHangRoutes_cl.BuildAdminWorkspaceProductsUrl();
        if (clear)
            return baseUrl;

        string keyword = (txt_search.Text ?? "").Trim();
        string visibility = (ddl_visibility.SelectedValue ?? "all").Trim().ToLowerInvariant();
        System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
        if (keyword != "")
            parts.Add("keyword=" + Server.UrlEncode(keyword));
        if (visibility != "" && visibility != "all")
            parts.Add("visibility=" + Server.UrlEncode(visibility));
        return parts.Count == 0 ? baseUrl : (baseUrl + "?" + string.Join("&", parts.ToArray()));
    }
}
