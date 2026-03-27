using System;
using System.Linq;
using System.Web;

public partial class home_don_chi_tiet : System.Web.UI.Page
{
    private const decimal VND_PER_A = 1000m;

    private string NormalizeMode(string raw)
    {
        string mode = (raw ?? "").Trim().ToLowerInvariant();
        return mode == "sell" ? "sell" : "buy";
    }

    private bool IsShopPortal()
    {
        return PortalRequest_cl.IsShopPortalRequest();
    }

    private string NormalizeReturnUrl(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        value = HttpUtility.UrlDecode(value) ?? "";
        value = value.Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        Uri absolute;
        if (Uri.TryCreate(value, UriKind.Absolute, out absolute))
        {
            if (Request.Url == null)
                return "";
            if (!string.Equals(absolute.Host, Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                return "";
            value = absolute.PathAndQuery;
        }

        if (!value.StartsWith("/", StringComparison.Ordinal))
            return "";
        if (value.StartsWith("//", StringComparison.Ordinal))
            return "";

        return value;
    }

    private string ResolveDefaultBackUrl(string mode)
    {
        if (mode == "sell")
            return IsShopPortal() ? "/shop/don-ban" : "/home/don-ban.aspx";
        return "/home/don-mua.aspx";
    }

    private string ResolveBackUrl()
    {
        string fromQuery = NormalizeReturnUrl((ViewState["return_url"] ?? "").ToString());
        if (!string.IsNullOrEmpty(fromQuery))
            return fromQuery;
        return ResolveDefaultBackUrl((ViewState["mode"] ?? "buy").ToString());
    }

    private void BindBackLinks()
    {
        string back = ResolveBackUrl();
        hl_back_top.NavigateUrl = back;
        hl_back_bottom.NavigateUrl = back;
    }

    private decimal ConvertToA(decimal vnd)
    {
        if (vnd <= 0m) return 0m;
        decimal a = vnd / VND_PER_A;
        return Math.Ceiling(a * 100m) / 100m;
    }

    private static string ResolveDisplayName(taikhoan_tb acc)
    {
        if (acc == null) return "";

        string name = (acc.ten_shop ?? "").Trim();
        if (!string.IsNullOrEmpty(name)) return name;

        name = (acc.hoten ?? "").Trim();
        if (!string.IsNullOrEmpty(name)) return name;

        return (acc.taikhoan ?? "").Trim();
    }

    private void LoadOrder()
    {
        string mode = (ViewState["mode"] ?? "buy").ToString();
        string orderId = (ViewState["order_id"] ?? "").ToString();
        string currentAccount = (ViewState["taikhoan"] ?? "").ToString();

        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(currentAccount))
        {
            Response.Redirect(ResolveBackUrl(), true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            DonHang_tb don = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == orderId);
            if (don == null)
            {
                Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Không tìm thấy đơn hàng.", "false", "false", "OK", "alert", "");
                Response.Redirect(ResolveBackUrl(), true);
                return;
            }

            bool authorized = mode == "sell"
                ? string.Equals(don.nguoiban ?? "", currentAccount, StringComparison.OrdinalIgnoreCase)
                : string.Equals(don.nguoimua ?? "", currentAccount, StringComparison.OrdinalIgnoreCase);

            if (!authorized)
            {
                Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không có quyền xem đơn hàng này.", "false", "false", "OK", "alert", "");
                Response.Redirect(ResolveBackUrl(), true);
                return;
            }

            DonHangStateMachine_cl.EnsureStateFields(don);
            string orderStatus = DonHangStateMachine_cl.GetOrderStatus(don);
            string exchangeStatus = DonHangStateMachine_cl.GetExchangeStatus(don);
            string legacyStatus = DonHangStateMachine_cl.ToLegacyStatus(orderStatus, exchangeStatus, don.online_offline);

            taikhoan_tb accSeller = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == don.nguoiban);
            taikhoan_tb accBuyer = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == don.nguoimua);

            lb_page_title.Text = mode == "sell" ? "Chi tiết đơn bán" : "Chi tiết đơn mua";
            lb_id.Text = don.id.ToString();
            lb_status.Text = legacyStatus;
            lb_loai.Text = (don.online_offline.HasValue && don.online_offline.Value == false) ? "Offline" : "Online";
            lb_ngaydat.Text = don.ngaydat.HasValue ? don.ngaydat.Value.ToString("dd/MM/yyyy HH:mm") : "--";
            lb_ten_shop.Text = ResolveDisplayName(accSeller);
            lb_nguoi_mua.Text = ResolveDisplayName(accBuyer);
            lb_nguoi_ban.Text = ResolveDisplayName(accSeller);
            lb_nguoinhan.Text = (don.hoten_nguoinhan ?? "").Trim();
            lb_sdt.Text = (don.sdt_nguoinhan ?? "").Trim();
            lb_diachi.Text = (don.diahchi_nguoinhan ?? "").Trim();

            decimal tongVnd = don.tongtien ?? 0m;
            lb_tong_vnd.Text = tongVnd.ToString("#,##0");
            lb_tong_a.Text = ConvertToA(tongVnd).ToString("#,##0.##");

            var details = (from ct in db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == orderId)
                           join sp in db.BaiViet_tbs on ct.idsp equals sp.id.ToString() into spGroup
                           from sp in spGroup.DefaultIfEmpty()
                           select new
                           {
                               ct.id,
                               ct.idsp,
                               name = sp != null ? sp.name : "",
                               name_en = sp != null ? sp.name_en : "",
                               image = sp != null ? sp.image : "/uploads/images/macdinh.jpg",
                               ct.giaban,
                               ct.soluong,
                               ct.thanhtien,
                               PhanTramUuDai = ct.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0
                           }).ToList();

            rp_chitiet.DataSource = details;
            rp_chitiet.DataBind();
            ph_empty.Visible = details.Count == 0;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string tkMaHoa = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (string.IsNullOrEmpty(tkMaHoa))
            {
                Response.Redirect(IsShopPortal() ? "/shop/login.aspx" : "/dang-nhap", true);
                return;
            }

            ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(tkMaHoa);
            ViewState["mode"] = NormalizeMode(Request.QueryString["mode"]);
            ViewState["order_id"] = (Request.QueryString["id"] ?? "").Trim();
            ViewState["return_url"] = NormalizeReturnUrl(Request.QueryString["return_url"] ?? "");

            if (string.IsNullOrEmpty((ViewState["order_id"] ?? "").ToString()))
            {
                Response.Redirect(ResolveBackUrl(), true);
                return;
            }

            BindBackLinks();
            LoadOrder();
        }
    }

    protected string BuildProductUrl(object nameEnObj, object idObj)
    {
        string id = (idObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(id))
            return "#";

        string slug = (nameEnObj ?? "").ToString().Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(slug))
            slug = "san-pham";
        return "/" + slug + "-" + id + ".html";
    }

    protected string FormatA(object vndObj)
    {
        decimal vnd = 0m;
        try { vnd = Convert.ToDecimal(vndObj); } catch { vnd = 0m; }
        return ConvertToA(vnd).ToString("0.00");
    }
}
