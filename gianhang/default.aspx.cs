using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

public partial class gianhang_Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
            BindPage(info);
    }

    private void BindPage(RootAccount_cl.RootAccountInfo info)
    {
        bool isActive = info != null && info.IsAuthenticated && info.CanAccessGianHang;

        ph_storefront_active.Visible = isActive;
        ph_access_shell.Visible = !isActive;

        if (isActive)
            BindStorefront(info);
        else
            BindAccessState(info);
    }

    private void BindStorefront(RootAccount_cl.RootAccountInfo info)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
                if (account == null)
                    return;

                GianHangPublic_cl.StorefrontContextInfo context = GianHangPublic_cl.ResolveContext(db, Request);
                GianHangStorefront_cl.StorefrontSummary summary = GianHangStorefront_cl.BuildSummary(db, account, Request.Url);
                List<GianHangStorefront_cl.ProductCardView> products = GianHangStorefront_cl.LoadProducts(db, info.AccountKey);
                List<GianHangStorefront_cl.ProductCardView> topProducts = GianHangStorefront_cl.LoadTopProducts(db, info.AccountKey, products);
                ApplyStorefrontConfig(db, account);

                lnk_storefront_profile_top.NavigateUrl = string.IsNullOrWhiteSpace(context.HomeUrl) ? summary.ProfileUrl : context.HomeUrl;
                lnk_storefront_profile_top.Text = "Mở trang công khai";

                lb_hero_title.Text = summary.StorefrontName;
                if (string.IsNullOrWhiteSpace(lb_hero_desc.Text))
                    lb_hero_desc.Text = "Không gian bán hàng dành cho khách quan tâm sản phẩm và dịch vụ của bạn.";

                lb_stat_products.Text = summary.ProductCount.ToString("#,##0");
                lb_stat_services.Text = summary.ServiceCount.ToString("#,##0");
                lb_stat_pending_orders.Text = summary.PendingOrderCount.ToString("#,##0");

                ph_top_products.Visible = topProducts.Count > 0;
                ph_empty_top_products.Visible = topProducts.Count == 0;
                rpt_top_products.DataSource = topProducts;
                rpt_top_products.DataBind();

                ph_empty_products.Visible = products.Count == 0;
                rp_products.DataSource = products;
                rp_products.DataBind();
            }
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            lb_hero_title.Text = "Gian hàng đối tác";
            if (string.IsNullOrWhiteSpace(lb_hero_desc.Text))
                lb_hero_desc.Text = "Dữ liệu đang tạm đồng bộ. Vui lòng tải lại sau ít giây.";

            lb_stat_products.Text = "0";
            lb_stat_services.Text = "0";
            lb_stat_pending_orders.Text = "0";

            ph_top_products.Visible = false;
            ph_empty_top_products.Visible = true;
            ph_empty_products.Visible = true;
            rpt_top_products.DataSource = new List<GianHangStorefront_cl.ProductCardView>();
            rpt_top_products.DataBind();
            rp_products.DataSource = new List<GianHangStorefront_cl.ProductCardView>();
            rp_products.DataBind();
        }
    }

    private void ApplyStorefrontConfig(dbDataContext db, taikhoan_tb account)
    {
        if (db == null || account == null)
            return;

        string accountKey = (account.taikhoan ?? string.Empty).Trim().ToLowerInvariant();
        string chiNhanhId = GianHangPublic_cl.ResolveChiNhanhIdForStoreAccount(db, account.taikhoan);
        gianhang_storefront_config_table config = GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);
        gianhang_storefront_section_table featuredProducts = GianHangStorefrontConfig_cl.GetSection(db, chiNhanhId, GianHangStorefrontConfig_cl.SectionFeaturedProducts);

        lb_hero_desc.Text = GianHangStorefrontConfig_cl.ResolveText(
            config.hero_description,
            "Không gian bán hàng dành cho khách quan tâm sản phẩm và dịch vụ của bạn.");

        lb_stat_products_label.Text = GianHangStorefrontConfig_cl.ResolveText(config.hero_metric_product_text, "Sản phẩm");
        lb_stat_services_label.Text = GianHangStorefrontConfig_cl.ResolveText(config.hero_metric_service_text, "Dịch vụ");
        lb_stat_pending_orders_label.Text = "Đơn đang chờ trao đổi";

        lb_top_products_title.Text = GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_title, "Top tin nổi bật");
        lb_top_products_sub.Text = GianHangStorefrontConfig_cl.ResolveText(config.hero_highlight_description, "Ưu tiên hiển thị các tin nổi bật của gian hàng.");

        string productsTitle = featuredProducts == null ? "" : GianHangStorefrontConfig_cl.ResolveText(featuredProducts.title, "");
        string productsDescription = featuredProducts == null ? "" : GianHangStorefrontConfig_cl.ResolveText(featuredProducts.description, "");

        lb_products_title.Text = string.IsNullOrWhiteSpace(productsTitle) ? "Danh sách sản phẩm và dịch vụ" : productsTitle;
        lb_products_sub.Text = string.IsNullOrWhiteSpace(productsDescription)
            ? "Danh sách sản phẩm và dịch vụ đang hoạt động."
            : productsDescription;

        ApplyHeroAction(lnk_storefront_primary_cta, config.hero_primary_text, config.hero_primary_url, accountKey, GianHangRoutes_cl.BuildPublicUrl(accountKey), false);
        ApplyHeroAction(lnk_storefront_secondary_cta, config.hero_secondary_text, config.hero_secondary_url, accountKey, GianHangRoutes_cl.BuildQuanLyTinUrl(), false);
        ApplyHeroAction(lnk_storefront_tertiary_cta, config.hero_tertiary_text, config.hero_tertiary_url, accountKey, GianHangRoutes_cl.BuildDonBanUrl(), false);
    }

    private static void ApplyHeroAction(System.Web.UI.WebControls.HyperLink link, string text, string rawUrl, string accountKey, string fallbackUrl, bool openNewTab)
    {
        if (link == null)
            return;

        string resolvedText = (text ?? string.Empty).Trim();
        if (resolvedText == string.Empty)
        {
            link.Visible = false;
            link.Text = string.Empty;
            link.NavigateUrl = string.Empty;
            return;
        }

        link.Visible = true;
        link.Text = resolvedText;
        link.NavigateUrl = GianHangPublic_cl.ResolveConfigUrl(rawUrl, accountKey, fallbackUrl);
        link.Target = openNewTab ? "_blank" : string.Empty;
    }

    private void BindAccessState(RootAccount_cl.RootAccountInfo info)
    {
        lit_account_key.Text = Server.HtmlEncode((info != null && info.IsAuthenticated ? info.AccountKey : "").Trim());

        string accessStatus = GianHangPolicy_cl.GetCurrentAccessStatus();
        lit_space_status.Text = Server.HtmlEncode(FormatStatus(accessStatus, info));

        CoreSpaceRequest_cl.SpaceRequestInfo latestRequest = GianHangPolicy_cl.GetCurrentLatestRequest();
        string reviewNote = latestRequest == null ? "" : (latestRequest.ReviewNote ?? "").Trim();

        ph_state_pending.Visible = latestRequest != null && string.Equals(latestRequest.RequestStatus, CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase);
        ph_state_blocked.Visible = !ph_state_pending.Visible && IsBlockedState(accessStatus, latestRequest);
        ph_state_request.Visible = !ph_state_pending.Visible && !ph_state_blocked.Visible;

        if (ph_state_pending.Visible)
        {
            lit_requested_at.Text = latestRequest.RequestedAt.HasValue
                ? latestRequest.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm")
                : "--";
            ph_review_note_pending.Visible = !string.IsNullOrEmpty(reviewNote);
            lit_review_note_pending.Text = Server.HtmlEncode(reviewNote);
        }
        else
        {
            lit_requested_at.Text = "--";
            ph_review_note_pending.Visible = false;
            lit_review_note_pending.Text = "";
        }

        if (ph_state_blocked.Visible)
        {
            ph_review_note_blocked.Visible = !string.IsNullOrEmpty(reviewNote);
            lit_review_note_blocked.Text = Server.HtmlEncode(reviewNote);
        }
        else
        {
            ph_review_note_blocked.Visible = false;
            lit_review_note_blocked.Text = "";
        }
    }

    protected void btn_request_open_Click(object sender, EventArgs e)
    {
        string message;
        bool ok = GianHangPolicy_cl.TryCreateRequestForCurrentAccount(out message);
        ShowMessage(message, ok ? "gianhang-state--success" : "gianhang-state--warning");
        BindPage(GianHangBootstrap_cl.GetCurrentInfo());
    }

    protected string BuildProductDetailUrl(object dataItem)
    {
        return GianHangStorefront_cl.BuildDetailUrl(dataItem as GianHangStorefront_cl.ProductCardView);
    }

    protected string BuildProductActionUrl(object dataItem)
    {
        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        return GianHangStorefront_cl.BuildActionUrl(dataItem as GianHangStorefront_cl.ProductCardView, info == null ? "" : info.AccountKey);
    }

    protected string BuildProductActionText(object dataItem)
    {
        return GianHangStorefront_cl.BuildActionText(dataItem as GianHangStorefront_cl.ProductCardView);
    }

    protected string BuildProductTypeLabel(object dataItem)
    {
        return GianHangStorefront_cl.BuildPostTypeLabel(dataItem as GianHangStorefront_cl.ProductCardView);
    }

    protected string BuildProductTypeCss(object dataItem)
    {
        return GianHangStorefront_cl.BuildPostTypeCss(dataItem as GianHangStorefront_cl.ProductCardView);
    }

    protected string FormatCurrency(object valueRaw)
    {
        decimal value = 0m;
        if (valueRaw != null && valueRaw != DBNull.Value)
        {
            try
            {
                value = Convert.ToDecimal(valueRaw);
            }
            catch
            {
                value = 0m;
            }
        }

        return GianHangStorefront_cl.FormatCurrency(value);
    }

    private void ShowMessage(string message, string cssModifier)
    {
        ph_message.Visible = !string.IsNullOrWhiteSpace(message);
        lit_message.Text = Server.HtmlEncode((message ?? "").Trim());
        box_message.Attributes["class"] = "gianhang-state " + (cssModifier ?? "gianhang-state--warning");
    }

    private static bool IsBlockedState(string accessStatus, CoreSpaceRequest_cl.SpaceRequestInfo latestRequest)
    {
        if (string.Equals(accessStatus, SpaceAccess_cl.StatusBlocked, StringComparison.OrdinalIgnoreCase)
            || string.Equals(accessStatus, SpaceAccess_cl.StatusRevoked, StringComparison.OrdinalIgnoreCase))
            return true;

        return latestRequest != null && string.Equals(latestRequest.RequestStatus, CoreSpaceRequest_cl.StatusRejected, StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatStatus(string accessStatus, RootAccount_cl.RootAccountInfo info)
    {
        if (info != null && info.CanAccessGianHang)
            return "Đang hoạt động";
        if (string.IsNullOrWhiteSpace(accessStatus))
            return "Chưa mở";

        string normalized = accessStatus.Trim().ToLowerInvariant();
        if (normalized == SpaceAccess_cl.StatusPending)
            return "Chờ duyệt";
        if (normalized == SpaceAccess_cl.StatusBlocked)
            return "Đang khóa";
        if (normalized == SpaceAccess_cl.StatusRevoked)
            return "Đã thu hồi";
        return normalized;
    }
}
