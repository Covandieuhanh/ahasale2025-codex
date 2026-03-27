using System;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;

public partial class gianhang_datlich_public : System.Web.UI.Page
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            LoadPageData();
    }

    private void LoadPageData()
    {
        int requestedId = 0;
        int.TryParse((Request.QueryString["id"] ?? string.Empty).Trim(), out requestedId);

        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);

            string requestedUser = (Request.QueryString["user"] ?? string.Empty).Trim();
            string storeAccountKey = (requestedUser ?? string.Empty).Trim().ToLowerInvariant();
            GH_SanPham_tb requestedService = null;

            if (requestedId > 0)
            {
                requestedService = GianHangPublic_cl.GetPublicItemById(db, requestedId, GianHangProduct_cl.LoaiDichVu);
                if (requestedService == null)
                {
                    ShowNotFound();
                    return;
                }
                if (storeAccountKey == string.Empty)
                    storeAccountKey = (requestedService.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant();
            }

            if (storeAccountKey == string.Empty)
            {
                taikhoan_tb currentOwner = GianHangPublic_cl.ResolveStoreAccount(db, "");
                if (currentOwner != null)
                    storeAccountKey = (currentOwner.taikhoan ?? string.Empty).Trim().ToLowerInvariant();
            }

            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, storeAccountKey);
            if (account == null || !SpaceAccess_cl.CanAccessGianHang(db, account))
            {
                ShowNotFound();
                return;
            }

            ViewState["gianhang_public_booking_account"] = storeAccountKey;
            ViewState["gianhang_public_return_url"] = ResolveReturnUrl(storeAccountKey, requestedId);

            lb_store_name.Text = HttpUtility.HtmlEncode(GianHangStorefront_cl.ResolveStorefrontName(account));
            lb_store_desc.Text = HttpUtility.HtmlEncode((account.motangan_shop ?? string.Empty).Trim());
            lnk_back.NavigateUrl = (ViewState["gianhang_public_return_url"] ?? GianHangPublic_cl.BuildStorefrontUrl(storeAccountKey)).ToString();

            BindServiceList(db, storeAccountKey, requestedService);
            PrefillCustomer(db);
            InitDefaults();

            pn_form.Visible = true;
            pn_not_found.Visible = false;
        }
    }

    private void BindServiceList(dbDataContext db, string storeAccountKey, GH_SanPham_tb requestedService)
    {
        ddl_service.Items.Clear();
        ddl_service.Items.Add(new ListItem("Chọn dịch vụ", ""));

        foreach (GH_SanPham_tb item in GianHangProduct_cl.QueryPublicByStorefront(db, storeAccountKey))
        {
            if (GianHangProduct_cl.NormalizeLoai(item.loai) != GianHangProduct_cl.LoaiDichVu)
                continue;

            ddl_service.Items.Add(new ListItem(item.ten ?? ("Dịch vụ #" + item.id.ToString()), item.id.ToString()));
        }

        if (requestedService != null)
        {
            ListItem selected = ddl_service.Items.FindByValue(requestedService.id.ToString());
            if (selected != null)
            {
                ddl_service.ClearSelection();
                selected.Selected = true;
            }
        }
    }

    private void PrefillCustomer(dbDataContext db)
    {
        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated || !info.CanAccessHome)
            return;

        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
        if (account == null || !SpaceAccess_cl.CanAccessHome(db, account))
            return;

        if (string.IsNullOrWhiteSpace(txt_customer_name.Text))
            txt_customer_name.Text = (account.hoten ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(txt_customer_phone.Text))
            txt_customer_phone.Text = (account.taikhoan ?? string.Empty).Trim();
    }

    private void InitDefaults()
    {
        if (string.IsNullOrWhiteSpace(txt_booking_date.Text))
            txt_booking_date.Text = DateTime.Now.ToString("yyyy-MM-dd");
        if (string.IsNullOrWhiteSpace(txt_booking_time.Text))
            txt_booking_time.Text = DateTime.Now.ToString("HH:mm");
    }

    protected void btn_submit_Click(object sender, EventArgs e)
    {
        string storeAccountKey = ((ViewState["gianhang_public_booking_account"] ?? string.Empty).ToString() ?? string.Empty).Trim().ToLowerInvariant();
        if (storeAccountKey == string.Empty)
        {
            ShowMessage("Không xác định được gian hàng cần đặt lịch.", false);
            return;
        }

        string customerName = (txt_customer_name.Text ?? string.Empty).Trim();
        string customerPhone = (txt_customer_phone.Text ?? string.Empty).Trim();
        string notes = (txt_notes.Text ?? string.Empty).Trim();
        string serviceName = string.Empty;

        if (ddl_service != null && ddl_service.SelectedItem != null && !string.IsNullOrWhiteSpace(ddl_service.SelectedValue))
            serviceName = ddl_service.SelectedItem.Text;

        if (customerName == string.Empty || customerPhone == string.Empty || serviceName == string.Empty)
        {
            ShowMessage("Vui lòng nhập họ tên, số điện thoại và chọn dịch vụ.", false);
            return;
        }

        DateTime? bookingAt = null;
        DateTime bookingDate;
        if (DateTime.TryParse(txt_booking_date.Text, ViCulture, DateTimeStyles.None, out bookingDate))
        {
            TimeSpan bookingTime;
            if (TimeSpan.TryParse(txt_booking_time.Text, out bookingTime))
                bookingAt = bookingDate.Date + bookingTime;
            else
                bookingAt = bookingDate.Date;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, storeAccountKey);
            if (account == null || !SpaceAccess_cl.CanAccessGianHang(db, account))
            {
                ShowMessage("Gian hàng này hiện chưa sẵn sàng để nhận lịch.", false);
                return;
            }

            GH_SanPham_tb selectedService = GianHangBooking_cl.ResolvePublicService(db, storeAccountKey, ddl_service == null ? "" : (ddl_service.SelectedValue ?? ""));
            GH_DatLich_tb booking = GianHangBooking_cl.CreateBooking(db, storeAccountKey, customerName, customerPhone, selectedService, serviceName, bookingAt, notes);
            if (booking == null)
            {
                ShowMessage("Không thể gửi lịch hẹn. Vui lòng thử lại.", false);
                return;
            }
        }

        ShowMessage("Đã gửi lịch hẹn thành công. Gian hàng sẽ liên hệ xác nhận sớm.", true);
        txt_customer_name.Text = string.Empty;
        txt_customer_phone.Text = string.Empty;
        txt_notes.Text = string.Empty;
        if (ddl_service != null && ddl_service.Items.Count > 0)
            ddl_service.SelectedIndex = 0;
        InitDefaults();
    }

    private string ResolveReturnUrl(string storeAccountKey, int serviceId)
    {
        string returnUrl = (Request.QueryString["return_url"] ?? string.Empty).Trim();
        if (returnUrl != string.Empty)
            return returnUrl;

        if (serviceId > 0)
            return GianHangRoutes_cl.BuildXemDichVuUrl(serviceId);

        return GianHangPublic_cl.BuildStorefrontUrl(storeAccountKey);
    }

    private void ShowMessage(string message, bool success)
    {
        pn_message.Visible = !string.IsNullOrWhiteSpace(message);
        lit_message.Text = HttpUtility.HtmlEncode((message ?? string.Empty).Trim());
        box_message.Attributes["class"] = success ? "message message-success" : "message message-warning";
    }

    private void ShowNotFound()
    {
        pn_form.Visible = false;
        pn_not_found.Visible = true;
    }
}
