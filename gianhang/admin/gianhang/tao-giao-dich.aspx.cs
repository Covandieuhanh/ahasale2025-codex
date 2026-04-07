using System;
using System.Web;

public partial class gianhang_admin_gianhang_tao_giao_dich : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string BackUrl = "/gianhang/admin/default.aspx";
    public string WorkspaceHubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string UtilityUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string AdminInvoiceCreateUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?q=add";
    public string NativeOrderCreateUrl = "/gianhang/don-ban.aspx?taodon=1";
    public string ContextTag = "Tạo giao dịch từ /gianhang/admin";
    public string ContextDescription = "Chọn luồng tạo phù hợp với mục tiêu vận hành hiện tại.";
    public string CustomerDisplay = "Khách lẻ";
    public string PhoneDisplay = "--";
    public string BookingId = "";
    public string ProductId = "";
    public string QuantityText = "1";
    public string InvoiceTag = "Khuyến nghị";
    public string OrderTag = "Luồng native";
    public string InvoiceCardCss = " gh-createflow__card--recommended";
    public string OrderCardCss = "";
    public string OrderHint = "Hiện luồng native chưa nhận prefill khách/lịch hẹn như hóa đơn admin.";
    public string OrderButtonCss = "gh-createflow__btn--soft";

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? "").Trim().ToLowerInvariant();
        if (ownerAccountKey == "")
            return;

        WorkspaceHubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        UtilityUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        BackUrl = ResolveBackUrl();

        string tenKhach = ((Request.QueryString["tenkh"] ?? "") + "").Trim();
        string sdt = AccountAuth_cl.NormalizePhone((Request.QueryString["sdt"] ?? "") + "") ?? "";
        string idDatLich = ((Request.QueryString["id_datlich"] ?? "") + "").Trim();
        string idNganh = ((Request.QueryString["idnganh"] ?? "") + "").Trim();
        string idSp = ((Request.QueryString["idsp"] ?? "") + "").Trim();
        string qty = ((Request.QueryString["qty"] ?? "") + "").Trim();
        int safeQty = Number_cl.Check_Int(qty);
        if (safeQty <= 0)
            safeQty = 1;

        CustomerDisplay = tenKhach == "" ? "Khách lẻ" : tenKhach;
        PhoneDisplay = sdt == "" ? "--" : sdt;
        BookingId = idDatLich;
        ProductId = idSp;
        QuantityText = safeQty.ToString();

        ph_booking.Visible = BookingId != "";
        ph_product.Visible = ProductId != "";

        AdminInvoiceCreateUrl = BuildAdminInvoiceCreateUrl(tenKhach, sdt, idDatLich, idNganh);
        NativeOrderCreateUrl = BuildNativeOrderCreateUrl(idSp, safeQty);

        ApplyRecommendation(tenKhach, sdt, idDatLich, idSp);
    }

    private void ApplyRecommendation(string tenKhach, string sdt, string idDatLich, string idSp)
    {
        bool hasBooking = idDatLich != "";
        bool hasCustomer = tenKhach != "" || sdt != "";
        bool hasProduct = idSp != "";

        if (hasBooking)
        {
            ContextTag = "Khởi tạo từ lịch hẹn";
            ContextDescription = "Lịch hẹn đang mang theo bối cảnh khách và nghiệp vụ dịch vụ, nên nên vào hóa đơn admin trước để giữ liên kết CRM/lịch hẹn.";
            InvoiceTag = "Khuyến nghị mạnh";
            OrderTag = "Tùy chọn phụ";
            OrderHint = "Nếu vẫn muốn đi theo đơn native thì có thể mở, nhưng lịch hẹn hiện không được prefill sâu như hóa đơn admin.";
            return;
        }

        if (hasCustomer)
        {
            ContextTag = "Khởi tạo từ khách hàng";
            ContextDescription = "Đang có sẵn dữ liệu khách hàng, nên hóa đơn admin là luồng mượt hơn cho vận hành nội bộ. Đơn native phù hợp khi bạn muốn đi theo logic storefront.";
            InvoiceTag = "Khuyến nghị";
            OrderTag = "Luồng storefront";
            OrderHint = "Luồng native chưa gắn sẵn khách hàng như hóa đơn admin, nên sẽ phù hợp hơn khi tạo đơn storefront hoặc đơn hàng nhanh.";
            return;
        }

        if (hasProduct)
        {
            ContextTag = "Khởi tạo từ sản phẩm";
            ContextDescription = "Đang có sẵn sản phẩm đầu vào, nên vào đơn gian hàng native sẽ sát logic bán hàng storefront hơn. Hóa đơn admin vẫn dùng được nếu muốn xử lý vận hành tại quầy.";
            InvoiceTag = "Vận hành nội bộ";
            OrderTag = "Khuyến nghị";
            InvoiceCardCss = "";
            OrderCardCss = " gh-createflow__card--recommended";
            OrderButtonCss = "gh-createflow__btn--primary";
            OrderHint = "Sản phẩm và số lượng đầu vào sẽ được giữ trong luồng tạo đơn native.";
            return;
        }

        ContextTag = "Chọn theo mục đích";
        ContextDescription = "Nếu cần storefront/đơn native thì vào Đơn gian hàng. Nếu cần CRM, lịch hẹn, vận hành nội bộ thì vào Hóa đơn admin.";
        InvoiceTag = "Vận hành nội bộ";
        OrderTag = "Storefront / native";
        OrderCardCss = "";
        OrderButtonCss = "gh-createflow__btn--soft";
    }

    private string BuildAdminInvoiceCreateUrl(string tenKhach, string sdt, string idDatLich, string idNganh)
    {
        return GianHangRoutes_cl.BuildAdminLegacyInvoiceCreateUrl(tenKhach, sdt, idDatLich, idNganh);
    }

    private string BuildNativeOrderCreateUrl(string productId, int qty)
    {
        string url = GianHangPosWorkflow_cl.BuildCreateModeEntryUrl(productId, BackUrl);
        if (qty > 1)
            url += "&qty=" + qty.ToString();
        return url;
    }

    private string ResolveBackUrl()
    {
        string raw = (Request.QueryString["return_url"] ?? "").Trim();
        string decoded = HttpUtility.UrlDecode(raw) ?? "";
        decoded = decoded.Trim();
        if (decoded == "" || !decoded.StartsWith("/", StringComparison.Ordinal) || decoded.StartsWith("//", StringComparison.Ordinal))
            return GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        return decoded;
    }
}
