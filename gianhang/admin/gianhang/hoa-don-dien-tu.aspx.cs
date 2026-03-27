using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

public partial class gianhang_admin_gianhang_hoa_don_dien_tu : System.Web.UI.Page
{
    private sealed class InvoiceRowView
    {
        public string DisplayId { get; set; }
        public string GuideText { get; set; }
        public string CreatedAtText { get; set; }
        public string CustomerDisplay { get; set; }
        public string CustomerPhone { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string TotalText { get; set; }
        public bool HasLegacyMirror { get; set; }
        public string LegacyUrl { get; set; }
        public string PublicUrl { get; set; }
        public string PreviewUrl { get; set; }
        public string PreviewKey { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    private string ownerAccountKey = string.Empty;

    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";
    public string BuyerFlowUrl = "/gianhang/admin/gianhang/don-mua.aspx";
    public int SummaryTotal;
    public int SummaryPending;
    public int SummaryPaid;
    public int SummaryMirrored;

    public string PreviewPublicId = string.Empty;
    public string PreviewGuideId = string.Empty;
    public string PreviewCustomer = string.Empty;
    public string PreviewPhone = string.Empty;
    public string PreviewCreatedAt = string.Empty;
    public string PreviewTotal = string.Empty;
    public string PreviewPublicUrl = string.Empty;
    public string PreviewLegacyUrl = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        BuyerFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl();
        LegacyInvoiceUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang";

        if (!IsPostBack)
            txt_search.Text = (Request.QueryString["keyword"] ?? string.Empty).Trim();

        BindInvoices();
    }

    protected void btn_filter_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(false), false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    protected void btn_clear_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(true), false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    protected string FormatLineMoney(object raw)
    {
        decimal value = 0m;
        if (raw != null && raw != DBNull.Value)
        {
            try
            {
                value = Convert.ToDecimal(raw, ViCulture);
            }
            catch
            {
                value = 0m;
            }
        }
        return value.ToString("#,##0.##", ViCulture) + " đ";
    }

    private void BindInvoices()
    {
        string keyword = (txt_search.Text ?? string.Empty).Trim().ToLowerInvariant();
        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.LoadStorefrontInvoicesWithRuntime(db, ownerAccountKey, 300, 300);
        SummaryTotal = invoices.Count;
        SummaryPaid = invoices.Count(p => string.Equals((p.trang_thai ?? string.Empty).Trim(), GianHangInvoice_cl.TrangThaiDaThu, StringComparison.OrdinalIgnoreCase));
        SummaryPending = invoices.Count(p => !string.Equals((p.trang_thai ?? string.Empty).Trim(), GianHangInvoice_cl.TrangThaiDaThu, StringComparison.OrdinalIgnoreCase)
                                          && !string.Equals((p.trang_thai ?? string.Empty).Trim(), GianHangInvoice_cl.TrangThaiHuy, StringComparison.OrdinalIgnoreCase));

        List<InvoiceRowView> rows = new List<InvoiceRowView>();
        List<GH_HoaDon_tb> filteredInvoices = new List<GH_HoaDon_tb>();
        foreach (GH_HoaDon_tb invoice in invoices)
        {
            string previewKey = ResolvePreviewKey(invoice);
            string haystack = string.Join(" ",
                invoice.id.ToString(),
                invoice.id_donhang ?? string.Empty,
                invoice.id_guide.HasValue ? invoice.id_guide.Value.ToString() : string.Empty,
                invoice.ten_khach ?? string.Empty,
                invoice.sdt ?? string.Empty).ToLowerInvariant();
            if (keyword != string.Empty && !haystack.Contains(keyword))
                continue;

            long legacyId = GianHangWorkspaceLink_cl.ResolveLegacyInvoiceId(db, ownerAccountKey, invoice.id);
            filteredInvoices.Add(invoice);
            rows.Add(new InvoiceRowView
            {
                DisplayId = GianHangInvoice_cl.ResolveOrderPublicId(invoice),
                GuideText = invoice.id_guide.HasValue ? invoice.id_guide.Value.ToString() : (previewKey ?? string.Empty),
                CreatedAtText = invoice.ngay_tao.HasValue ? invoice.ngay_tao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                CustomerDisplay = string.IsNullOrWhiteSpace(invoice.ten_khach) ? "Khách lẻ" : invoice.ten_khach.Trim(),
                CustomerPhone = (invoice.sdt ?? string.Empty).Trim(),
                StatusText = GianHangInvoice_cl.ResolveOrderStatusText(invoice),
                StatusCss = ResolveStatusCss(invoice),
                TotalText = GianHangReport_cl.FormatCurrency(GianHangInvoice_cl.ResolveTotalAmount(invoice)) + " đ",
                HasLegacyMirror = legacyId > 0,
                LegacyUrl = legacyId > 0 ? "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + legacyId.ToString() : string.Empty,
                PublicUrl = GianHangRoutes_cl.BuildElectronicInvoiceUrl(previewKey),
                PreviewUrl = GianHangRoutes_cl.BuildAdminWorkspaceElectronicInvoiceUrl() + "?id=" + Server.UrlEncode(previewKey) + (keyword == string.Empty ? string.Empty : "&keyword=" + Server.UrlEncode(keyword)),
                PreviewKey = previewKey
            });
        }

        SummaryMirrored = rows.Count(p => p.HasLegacyMirror);
        ph_empty.Visible = rows.Count == 0;
        rp_invoices.DataSource = rows;
        rp_invoices.DataBind();

        string selectedKey = ResolveSelectedPreviewKey(filteredInvoices);
        if (selectedKey == string.Empty)
        {
            ph_preview.Visible = false;
            ph_preview_legacy.Visible = false;
            ph_no_preview.Visible = true;
            rp_lines.DataSource = null;
            rp_lines.DataBind();
            return;
        }

        GianHangInvoicePrint_cl.InvoicePrintState state = GianHangInvoicePrint_cl.BuildState(db, selectedKey);
        GH_HoaDon_tb nativeInvoice = GianHangInvoice_cl.FindByPublicKeyOrId(db, selectedKey);
        if (state == null || nativeInvoice == null || !string.Equals((nativeInvoice.shop_taikhoan ?? string.Empty).Trim(), ownerAccountKey, StringComparison.OrdinalIgnoreCase))
        {
            ph_preview.Visible = false;
            ph_preview_legacy.Visible = false;
            ph_no_preview.Visible = true;
            rp_lines.DataSource = null;
            rp_lines.DataBind();
            return;
        }

        PreviewPublicId = state.PublicId ?? string.Empty;
        PreviewGuideId = string.IsNullOrWhiteSpace(state.GuideId) ? selectedKey : state.GuideId;
        PreviewCustomer = state.CustomerName ?? string.Empty;
        PreviewPhone = state.CustomerPhone ?? string.Empty;
        PreviewCreatedAt = state.CreatedAtText ?? string.Empty;
        PreviewTotal = (state.AfterDiscountText ?? state.TotalText ?? "0") + " đ";
        PreviewPublicUrl = GianHangRoutes_cl.BuildElectronicInvoiceUrl(selectedKey);

        long previewLegacyId = GianHangWorkspaceLink_cl.ResolveLegacyInvoiceId(db, ownerAccountKey, nativeInvoice.id);
        PreviewLegacyUrl = previewLegacyId > 0 ? "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + previewLegacyId.ToString() : string.Empty;
        ph_preview.Visible = true;
        ph_preview_legacy.Visible = previewLegacyId > 0;
        ph_no_preview.Visible = false;
        rp_lines.DataSource = state.Lines ?? new List<GianHangInvoicePrint_cl.InvoiceLine>();
        rp_lines.DataBind();
    }

    private string BuildFilterUrl(bool clear)
    {
        if (clear)
            return GianHangRoutes_cl.BuildAdminWorkspaceElectronicInvoiceUrl();

        string keyword = (txt_search.Text ?? string.Empty).Trim();
        if (keyword == string.Empty)
            return GianHangRoutes_cl.BuildAdminWorkspaceElectronicInvoiceUrl();
        return GianHangRoutes_cl.BuildAdminWorkspaceElectronicInvoiceUrl() + "?keyword=" + Server.UrlEncode(keyword);
    }

    private string ResolveSelectedPreviewKey(List<GH_HoaDon_tb> filteredInvoices)
    {
        string requested = (Request.QueryString["id"] ?? string.Empty).Trim();
        if (requested != string.Empty)
        {
            GH_HoaDon_tb invoice = GianHangInvoice_cl.FindByPublicKeyOrId(db, requested);
            if (invoice != null && string.Equals((invoice.shop_taikhoan ?? string.Empty).Trim(), ownerAccountKey, StringComparison.OrdinalIgnoreCase))
                return requested;
        }

        GH_HoaDon_tb first = filteredInvoices.FirstOrDefault();
        return ResolvePreviewKey(first);
    }

    private static string ResolvePreviewKey(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return string.Empty;
        if (invoice.id_guide.HasValue)
            return invoice.id_guide.Value.ToString();
        string orderId = (invoice.id_donhang ?? string.Empty).Trim();
        if (orderId != string.Empty)
            return orderId;
        return invoice.id.ToString();
    }

    private static string ResolveStatusCss(GH_HoaDon_tb invoice)
    {
        string status = (invoice == null ? string.Empty : (invoice.trang_thai ?? string.Empty)).Trim();
        if (string.Equals(status, GianHangInvoice_cl.TrangThaiDaThu, StringComparison.OrdinalIgnoreCase))
            return "gh-admin-einv__badge--paid";
        if (string.Equals(status, GianHangInvoice_cl.TrangThaiHuy, StringComparison.OrdinalIgnoreCase))
            return "gh-admin-einv__badge--cancelled";
        return "gh-admin-einv__badge--new";
    }
}
