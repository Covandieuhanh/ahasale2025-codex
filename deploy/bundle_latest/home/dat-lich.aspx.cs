using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;

public partial class home_dat_lich : System.Web.UI.Page
{
    private const string BookingSource = "Home Public";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (txt_ngay != null)
            {
                txt_ngay.Attributes["type"] = "date";
                txt_ngay.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (!IsPostBack)
            {
                txt_ngay.Text = DateTime.Now.ToString("yyyy-MM-dd");
                datlich_class.bind_gio_phut(ddl_gio, ddl_phut, DateTime.Now.AddHours(1));
                BindBookingPage();
            }
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            ShowWarning(AhaShineContext_cl.BuildTransientWarningMessage());
            ph_success.Visible = false;
            lnk_back.NavigateUrl = ResolveReturnUrlFallback();
            lnk_back.Text = "Quay lại";
        }
    }

    protected void but_datlich_Click(object sender, EventArgs e)
    {
        try
        {
            string shopAccount = ResolveCurrentShopAccount();
            if (string.IsNullOrWhiteSpace(shopAccount))
            {
                ShowWarning("Không xác định được gian hàng để đặt lịch.");
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                string selectedServiceId = (hf_service_id.Value ?? "").Trim();
                if (string.IsNullOrWhiteSpace(selectedServiceId))
                {
                    ShowWarning("Vui lòng chọn dịch vụ từ danh sách gợi ý.");
                    BindBookingPage();
                    return;
                }

                BaiViet_tb service = ResolveService(db, shopAccount, selectedServiceId);
                if (service == null)
                {
                    ShowWarning("Dịch vụ không tồn tại hoặc không còn hiển thị.");
                    BindBookingPage();
                    return;
                }

                string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, shopAccount);
                web_post_table mirroredService = ShopToAhaShinePostSync_cl.SyncTradePost(db, service);
                string bookingServiceId = mirroredService == null ? service.id.ToString() : mirroredService.id.ToString();
                string ngayInput = NormalizeDateInput(txt_ngay.Text);
                string gio = (ddl_gio.SelectedValue ?? "").Trim();
                string phut = (ddl_phut.SelectedValue ?? "").Trim();

                datlich_validate_result result = datlich_class.chuanhoa_du_lieu(
                    txt_hoten.Text,
                    txt_sdt.Text,
                    ngayInput,
                    gio,
                    phut,
                    bookingServiceId,
                    "",
                    txt_ghichu.Text,
                    datlich_class.trangthai_chua_xacnhan,
                    BookingSource,
                    BookingSource,
                    false
                );

                if (!result.hop_le)
                {
                    ShowWarning(result.thongbao);
                    BindBookingPage();
                    return;
                }

                string businessRule = datlich_class.kiemtra_quy_tac_van_hanh(db, result.dulieu, chiNhanhId, null, true);
                if (!string.IsNullOrWhiteSpace(businessRule))
                {
                    ShowWarning(businessRule);
                    BindBookingPage();
                    return;
                }

                bspa_datlich_table booking = new bspa_datlich_table();
                datlich_class.gan_du_lieu_vao_lich(db, booking, result.dulieu, BookingSource, chiNhanhId, false);
                db.bspa_datlich_tables.InsertOnSubmit(booking);
                db.SubmitChanges();

                TryNotifyAdvancedPortal(db, shopAccount, result.dulieu.tenkhachhang, booking.id);

                string successUrl = BuildSuccessUrl(shopAccount, service.id.ToString(), booking.id.ToString());
                Response.Redirect(successUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            ShowWarning(AhaShineContext_cl.BuildTransientWarningMessage());
        }
    }

    private void BindBookingPage()
    {
        string shopAccount = ResolveCurrentShopAccount();
        string serviceId = (Request.QueryString["id"] ?? "").Trim();
        string returnUrl = ResolveReturnUrl();

        lnk_back.NavigateUrl = returnUrl;
        lnk_back.Text = "Quay lại";

        if (Request.QueryString["ok"] == "1" && !string.IsNullOrWhiteSpace(Request.QueryString["booking_id"]))
        {
            ph_success.Visible = true;
            lb_success.Text = "Đặt lịch thành công. Mã lịch hẹn: #" + HttpUtility.HtmlEncode(Request.QueryString["booking_id"]);
        }
        else
        {
            ph_success.Visible = false;
            lb_success.Text = "";
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb shop = ResolveShop(db, shopAccount, serviceId);
            if (shop == null)
            {
                pn_form.Visible = false;
                ShowWarning("Không tìm thấy gian hàng hoặc dịch vụ công khai để đặt lịch.");
                return;
            }

            shopAccount = (shop.taikhoan ?? "").Trim().ToLowerInvariant();
            hf_shop_account.Value = shopAccount;
            lb_shop_name.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(shop.ten_shop) ? shop.taikhoan : shop.ten_shop.Trim());
            BindServiceInput(db, shopAccount, serviceId);
            AutoFillCustomerInfo(db, AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, shopAccount));
        }
    }

    private void BindServiceInput(dbDataContext db, string shopAccount, string serviceId)
    {
        lb_service_name.Text = "Chọn dịch vụ";
        txt_service_search.Text = "";
        hf_service_id.Value = "";

        if (db == null || string.IsNullOrWhiteSpace(shopAccount) || string.IsNullOrWhiteSpace(serviceId))
            return;

        BaiViet_tb service = ResolveService(db, shopAccount, serviceId);
        if (service == null)
            return;

        string name = (service.name ?? "").Trim();
        txt_service_search.Text = name;
        hf_service_id.Value = service.id.ToString();
        lb_service_name.Text = HttpUtility.HtmlEncode(name);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ServiceSuggestItem> SearchServices(string term, string shopAccount)
    {
        List<ServiceSuggestItem> results = new List<ServiceSuggestItem>();
        string keyword = (term ?? "").Trim();
        string tk = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(tk))
            return results;

        if (keyword.Length > 80)
            keyword = keyword.Substring(0, 80);

        using (dbDataContext db = new dbDataContext())
        {
            results = db.BaiViet_tbs
                .Where(p => p.nguoitao == tk
                            && (p.bin == false || p.bin == null)
                            && (p.phanloai ?? "").Trim() == AccountVisibility_cl.PostTypeService
                            && p.name != null
                            && p.name.Contains(keyword))
                .OrderByDescending(p => p.ngaytao)
                .Take(12)
                .Select(p => new ServiceSuggestItem
                {
                    id = p.id.ToString(),
                    name = (p.name ?? "").Trim()
                })
                .ToList();
        }

        return results;
    }

    public class ServiceSuggestItem
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    private taikhoan_tb ResolveShop(dbDataContext db, string shopAccount, string serviceId)
    {
        string tk = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(tk))
            return db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);

        BaiViet_tb service = ResolveService(db, "", serviceId);
        if (service == null || string.IsNullOrWhiteSpace(service.nguoitao))
            return null;

        return db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == service.nguoitao);
    }

    private BaiViet_tb ResolveService(dbDataContext db, string shopAccount, string serviceId)
    {
        int id;
        if (db == null || !int.TryParse((serviceId ?? "").Trim(), out id))
            return null;

        var query = db.BaiViet_tbs.Where(p =>
            p.id == id
            && (p.bin == false || p.bin == null)
            && (p.phanloai ?? "").Trim() == AccountVisibility_cl.PostTypeService);

        string tk = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(tk))
            query = query.Where(p => p.nguoitao == tk);

        return query.FirstOrDefault();
    }

    private string ResolveCurrentShopAccount()
    {
        string fromQuery = (Request.QueryString["user"] ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(fromQuery))
            return fromQuery;

        string currentPortalAccount = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        return currentPortalAccount;
    }

    private void AutoFillCustomerInfo(dbDataContext db, string chiNhanhId)
    {
        if (!string.IsNullOrWhiteSpace(txt_hoten.Text) || !string.IsNullOrWhiteSpace(txt_sdt.Text))
            return;

        string phone = "";
        if (Session["user_home"] != null)
            phone = (Session["user_home"] ?? "").ToString().Trim();

        if (string.IsNullOrWhiteSpace(phone) && Request.Cookies["save_sdt_home_aka"] != null)
        {
            try
            {
                phone = encode_class.decrypt(Request.Cookies["save_sdt_home_aka"].Value);
            }
            catch
            {
                phone = "";
            }
        }

        phone = datlich_class.chuanhoa_sdt(phone);
        if (string.IsNullOrWhiteSpace(phone))
            return;

        txt_sdt.Text = phone;
        var kh = db.bspa_data_khachhang_tables
            .Where(p => p.sdt == phone && p.id_chinhanh == chiNhanhId)
            .OrderByDescending(p => p.ngaytao)
            .FirstOrDefault();
        if (kh != null && string.IsNullOrWhiteSpace(txt_hoten.Text))
            txt_hoten.Text = kh.tenkhachhang;
    }

    private string NormalizeDateInput(string raw)
    {
        DateTime parsed;
        if (DateTime.TryParseExact((raw ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            return parsed.ToString("dd/MM/yyyy");

        return (raw ?? "").Trim();
    }

    private string ResolveReturnUrl()
    {
        string raw = (Request.QueryString["return_url"] ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(raw) && raw.StartsWith("/", StringComparison.Ordinal) && !raw.StartsWith("//", StringComparison.Ordinal))
            return raw;

        string shopAccount = ResolveCurrentShopAccount();
        if (!string.IsNullOrWhiteSpace(shopAccount))
        {
            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb shop = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == shopAccount);
                if (shop != null)
                    return ShopSlug_cl.GetPublicUrl(db, shop);
            }
        }

        return ResolveReturnUrlFallback();
    }

    private string ResolveReturnUrlFallback()
    {
        string shopAccount = ResolveCurrentShopAccount();
        if (!string.IsNullOrWhiteSpace(shopAccount))
            return "/home/Default.aspx?user=" + HttpUtility.UrlEncode(shopAccount);
        return "/";
    }

    private string BuildSuccessUrl(string shopAccount, string serviceId, string bookingId)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrWhiteSpace(shopAccount))
            query["user"] = shopAccount;
        if (!string.IsNullOrWhiteSpace(serviceId))
            query["id"] = serviceId;
        query["return_url"] = ResolveReturnUrl();
        query["ok"] = "1";
        query["booking_id"] = bookingId;
        return "/home/dat-lich.aspx?" + query.ToString();
    }

    private void TryNotifyAdvancedPortal(dbDataContext db, string shopAccount, string customerName, long bookingId)
    {
        try
        {
            thongbao_table notice = new thongbao_table();
            notice.id = Guid.NewGuid();
            notice.daxem = false;
            notice.nguoithongbao = BookingSource;
            notice.nguoinhan = shopAccount;
            notice.link = "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx";
            notice.noidung = (customerName ?? "Khách") + " vừa đặt lịch #" + bookingId.ToString();
            notice.thoigian = DateTime.Now;
            db.thongbao_tables.InsertOnSubmit(notice);
            db.SubmitChanges();
        }
        catch
        {
        }
    }

    private void ShowWarning(string message)
    {
        ph_warning.Visible = !string.IsNullOrWhiteSpace(message);
        lb_warning.Text = message ?? "";
    }
}
