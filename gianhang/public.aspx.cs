using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

public partial class gianhang_public : System.Web.UI.Page
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            LoadPageData();
    }

    private void LoadPageData()
    {
        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);

            taikhoan_tb account = GianHangPublic_cl.ResolveStoreAccount(db, Request.QueryString["user"]);
            if (account == null)
            {
                taikhoan_tb pendingHomeAccount = ResolvePendingReviewAccount(db);
                if (pendingHomeAccount != null)
                {
                    BindPendingReview(pendingHomeAccount);
                    return;
                }

                RedirectToDefault();
                return;
            }

            string accountKey = (account.taikhoan ?? string.Empty).Trim().ToLowerInvariant();
            ViewState["gianhang_public_account"] = accountKey;

            ph_public_active.Visible = true;
            ph_pending_review.Visible = false;

            GianHangPublic_cl.PublicSummary summary = GianHangPublic_cl.BuildSummary(db, account, Request.Url);
            List<GianHangPublic_cl.PublicProductView> products = GianHangPublic_cl.LoadProducts(db, accountKey);
            List<GianHangPublic_cl.PublicProductView> topProducts = GianHangPublic_cl.LoadTopProducts(products, 6);

            ApplyStorefrontConfig(db, account, accountKey);
            BindSummary(summary);
            BindProducts(products, topProducts);
            BindCurrentHomeState(db, accountKey);
        }
    }

    private taikhoan_tb ResolvePendingReviewAccount(dbDataContext db)
    {
        if (db == null || Request == null || Request.QueryString == null)
            return null;

        string requestedUser = (Request.QueryString["user"] ?? string.Empty).Trim().ToLowerInvariant();
        if (requestedUser == string.Empty)
            return null;

        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, requestedUser);
        if (account == null)
            return null;

        bool isHome = PortalScope_cl.CanLoginHome(account.taikhoan, account.phanloai, account.permission);
        if (!isHome)
            return null;

        if (SpaceAccess_cl.CanAccessGianHang(db, account))
            return null;

        return account;
    }

    private void BindPendingReview(taikhoan_tb account)
    {
        ph_public_active.Visible = false;
        ph_pending_review.Visible = true;

        string accountKey = account == null ? string.Empty : ((account.taikhoan ?? string.Empty).Trim().ToLowerInvariant());
        string ownerName = account == null
            ? string.Empty
            : (!string.IsNullOrWhiteSpace(account.hoten) ? account.hoten.Trim() : accountKey);

        lb_pending_store_name.Text = HttpUtility.HtmlEncode(ownerName);
        lb_pending_store_account.Text = HttpUtility.HtmlEncode(accountKey);
        lnk_pending_home.NavigateUrl = "/gianhang/tai-khoan/default.aspx";
    }

    private void ApplyStorefrontConfig(dbDataContext db, taikhoan_tb account, string accountKey)
    {
        string chiNhanhId = GianHangPublic_cl.ResolveChiNhanhIdForStoreAccount(db, account.taikhoan);
        gianhang_storefront_config_table config = GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);
        gianhang_storefront_section_table featuredProducts = GianHangStorefrontConfig_cl.GetSection(db, chiNhanhId, GianHangStorefrontConfig_cl.SectionFeaturedProducts);

        lb_public_hero_title.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_title, "Trang công khai"));
        lb_public_hero_sub.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_description, "Khám phá sản phẩm, dịch vụ và các tiện ích đang hoạt động của gian hàng."));
        lb_public_top_title.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_title, "Top tin nổi bật"));
        lb_public_top_sub.Text = HttpUtility.HtmlEncode(GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_description, "Ưu tiên theo số lượng bán và lượt xem."));

        string productsTitle = featuredProducts == null ? "" : GianHangStorefrontConfig_cl.ResolveText(featuredProducts.title, "");
        string productsDescription = featuredProducts == null ? "" : GianHangStorefrontConfig_cl.ResolveText(featuredProducts.description, "");
        lb_public_products_title.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(productsTitle) ? "Sản phẩm & dịch vụ công khai" : productsTitle);
        lb_public_products_sub.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(productsDescription) ? "Danh sách sản phẩm và dịch vụ đang hoạt động." : productsDescription);

        string primaryText = (config.hero_primary_text ?? string.Empty).Trim();
        string primaryUrl = GianHangPublic_cl.ResolveConfigUrl((config.hero_primary_url ?? string.Empty).Trim(), accountKey, GianHangPublic_cl.BuildStorefrontUrl(accountKey));
        lnk_public_primary_cta.Visible = !string.IsNullOrWhiteSpace(primaryText);
        lnk_public_primary_cta.Text = HttpUtility.HtmlEncode(primaryText);
        lnk_public_primary_cta.NavigateUrl = primaryUrl;

        string topCtaText = featuredProducts != null && !string.IsNullOrWhiteSpace(featuredProducts.cta_text)
            ? featuredProducts.cta_text.Trim()
            : "Xem tất cả";
        string topCtaUrl = featuredProducts != null
            ? GianHangPublic_cl.ResolveConfigUrl(featuredProducts.cta_url, accountKey, "#gianhang-products")
            : "#gianhang-products";

        lnk_public_top_cta.Text = HttpUtility.HtmlEncode(topCtaText);
        lnk_public_top_cta.NavigateUrl = topCtaUrl;
    }

    private void BindSummary(GianHangPublic_cl.PublicSummary summary)
    {
        string cover = (summary.CoverUrl ?? string.Empty).Trim();
        bool hasCover = !string.IsNullOrEmpty(cover) && !cover.EndsWith("/uploads/images/macdinh.jpg", StringComparison.OrdinalIgnoreCase);
        pn_cover.Visible = hasCover;
        if (hasCover && hero_cover != null)
            hero_cover.Attributes["style"] = "background-image: url('" + HttpUtility.HtmlAttributeEncode(cover) + "');";

        if (hero_section != null)
            hero_section.Attributes["class"] = hasCover ? "hero has-cover" : "hero";

        img_avatar.ImageUrl = summary.AvatarUrl;
        lb_store_name.Text = HttpUtility.HtmlEncode(summary.StoreName);
        lb_store_name_banner.Text = HttpUtility.HtmlEncode(summary.StoreName);
        lb_store_account.Text = HttpUtility.HtmlEncode(summary.AccountKey);
        lb_owner_name.Text = HttpUtility.HtmlEncode(summary.OwnerName);
        lb_total_products.Text = summary.ProductCount.ToString("#,##0");
        lb_total_services.Text = summary.ServiceCount.ToString("#,##0");
        lb_total_views.Text = summary.TotalViews.ToString("#,##0");
        lb_total_sold.Text = summary.TotalSold.ToString("#,##0");

        string description = (summary.Description ?? string.Empty).Trim();
        ph_store_desc.Visible = description != string.Empty;
        lb_store_desc.Text = HttpUtility.HtmlEncode(description);
    }

    private void BindProducts(IList<GianHangPublic_cl.PublicProductView> products, IList<GianHangPublic_cl.PublicProductView> topProducts)
    {
        List<GianHangPublic_cl.PublicProductView> safeProducts = products == null
            ? new List<GianHangPublic_cl.PublicProductView>()
            : new List<GianHangPublic_cl.PublicProductView>(products);
        List<GianHangPublic_cl.PublicProductView> safeTopProducts = topProducts == null
            ? new List<GianHangPublic_cl.PublicProductView>()
            : new List<GianHangPublic_cl.PublicProductView>(topProducts);

        rp_top_products.DataSource = safeTopProducts;
        rp_top_products.DataBind();
        ph_top_products.Visible = safeTopProducts.Count > 0;
        ph_empty_top_products.Visible = safeTopProducts.Count == 0;

        rp_products.DataSource = safeProducts;
        rp_products.DataBind();
        ph_products.Visible = safeProducts.Count > 0;
        ph_empty_products.Visible = safeProducts.Count == 0;

        BindCategoryOptions(safeProducts);
        lit_product_count.Text = safeProducts.Count.ToString("#,##0");
    }

    private void BindCurrentHomeState(dbDataContext db, string storeAccountKey)
    {
        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
        bool isAuthenticated = info != null && info.IsAuthenticated && info.CanAccessHome;
        bool isOwner = isAuthenticated && string.Equals((info.AccountKey ?? string.Empty).Trim(), storeAccountKey, StringComparison.OrdinalIgnoreCase);

        if (isAuthenticated)
        {
            if (isOwner && info.CanAccessGianHang)
            {
                lnk_home_login.Text = "Vào gian hàng của tôi";
                lnk_home_login.NavigateUrl = GianHangRoutes_cl.BuildDashboardUrl();
            }
            else
            {
                lnk_home_login.Text = "Vào trang Home";
                lnk_home_login.NavigateUrl = "/gianhang/tai-khoan/default.aspx";
            }
        }
        else
        {
            lnk_home_login.Text = "Đăng nhập tài khoản để trao đổi";
            lnk_home_login.NavigateUrl = GianHangRoutes_cl.BuildLoginUrl(Request.RawUrl ?? GianHangPublic_cl.BuildStorefrontUrl(storeAccountKey));
        }

        ph_owner_actions.Visible = isOwner;
        if (isOwner)
        {
            lnk_owner_dashboard.NavigateUrl = GianHangRoutes_cl.BuildDashboardUrl();
            lnk_owner_manage.NavigateUrl = GianHangRoutes_cl.BuildQuanLyTinUrl();
        }
    }

    private void BindCategoryOptions(IList<GianHangPublic_cl.PublicProductView> products)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder discovery = new StringBuilder();
        sb.Append("<option value=\"\">Tất cả danh mục</option>");

        SortedDictionary<string, string> categories = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (products != null)
        {
            for (int i = 0; i < products.Count; i++)
            {
                string categoryId = (products[i].CategoryId ?? string.Empty).Trim();
                string categoryName = (products[i].CategoryName ?? string.Empty).Trim();
                if (categoryId == string.Empty || categoryName == string.Empty)
                    continue;
                if (!categories.ContainsKey(categoryId))
                    categories.Add(categoryId, categoryName);
            }
        }

        foreach (KeyValuePair<string, string> pair in categories)
        {
            sb.Append("<option value=\"");
            sb.Append(HttpUtility.HtmlAttributeEncode(pair.Key));
            sb.Append("\">");
            sb.Append(HttpUtility.HtmlEncode(pair.Value));
            sb.Append("</option>");

            discovery.Append("<a href=\"#gianhang-products\" class=\"discovery-chip\" data-discovery-category=\"");
            discovery.Append(HttpUtility.HtmlAttributeEncode(pair.Key));
            discovery.Append("\">");
            discovery.Append(HttpUtility.HtmlEncode(pair.Value));
            discovery.Append("</a>");
        }

        lit_category_options.Text = sb.ToString();
        lit_category_discovery.Text = discovery.ToString();
    }

    protected string BuildDetailUrl(object dataItem)
    {
        return GianHangPublic_cl.BuildDetailUrl(dataItem as GianHangPublic_cl.PublicProductView);
    }

    protected string BuildExchangeActionUrl(object dataItem)
    {
        return GianHangPublic_cl.BuildExchangeUrl(dataItem as GianHangPublic_cl.PublicProductView, Request.RawUrl ?? GianHangRoutes_cl.BuildPublicUrl(string.Empty));
    }

    protected string BuildAddCartActionUrl(object dataItem)
    {
        return GianHangPublic_cl.BuildAddCartUrl(dataItem as GianHangPublic_cl.PublicProductView, Request.RawUrl ?? GianHangRoutes_cl.BuildPublicUrl(string.Empty));
    }

    protected string BuildTypeCss(object rawType)
    {
        return GianHangPublic_cl.BuildTypeCss(Convert.ToString(rawType, ViCulture));
    }

    protected string BuildTypeLabel(object rawType)
    {
        return GianHangPublic_cl.BuildTypeLabel(Convert.ToString(rawType, ViCulture));
    }

    protected string FormatCurrency(object valueRaw)
    {
        decimal value = 0m;
        if (valueRaw != null && valueRaw != DBNull.Value)
        {
            try
            {
                value = Convert.ToDecimal(valueRaw, ViCulture);
            }
            catch
            {
                value = 0m;
            }
        }

        return value.ToString("#,##0.##", ViCulture);
    }

    protected string FormatDate(object valueRaw)
    {
        DateTime value;
        if (DateTime.TryParse(Convert.ToString(valueRaw, ViCulture), out value))
            return value.ToString("dd/MM/yyyy HH:mm");
        return "--";
    }

    protected string EncodeAttr(object raw)
    {
        return HttpUtility.HtmlAttributeEncode(Convert.ToString(raw, ViCulture));
    }

    protected string BuildTypeFilterValue(object rawType)
    {
        string normalized = GianHangProduct_cl.NormalizeLoai(Convert.ToString(rawType, ViCulture));
        return normalized == GianHangProduct_cl.LoaiDichVu ? "service" : "product";
    }

    protected bool ShowAddCart(object dataItem)
    {
        GianHangPublic_cl.PublicProductView item = dataItem as GianHangPublic_cl.PublicProductView;
        return item != null && !item.IsService && item.Id > 0;
    }

    private void RedirectToDefault()
    {
        Response.Redirect(GianHangRoutes_cl.BuildDashboardUrl(), false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }
}
