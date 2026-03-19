using System;
using System.Linq;
using System.Web;

public partial class home_them_vao_gio : System.Web.UI.Page
{
    private const decimal VND_PER_A = 1000m;

    private int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
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

    private string ResolveDefaultBackUrl()
    {
        if (PortalRequest_cl.IsShopPortalRequest())
            return "/shop/default.aspx";
        return "/";
    }

    private string ResolveBackUrl()
    {
        string fromQuery = NormalizeReturnUrl((ViewState["return_url"] ?? "").ToString());
        if (!string.IsNullOrEmpty(fromQuery))
            return fromQuery;
        return ResolveDefaultBackUrl();
    }

    private void BindBackLinks()
    {
        string back = ResolveBackUrl();
        hl_back_top.NavigateUrl = back;
        hl_back_bottom.NavigateUrl = back;
    }

    private decimal ToA(decimal vnd)
    {
        if (vnd <= 0m) return 0m;
        decimal a = vnd / VND_PER_A;
        return Math.Ceiling(a * 100m) / 100m;
    }

    private void UpdateTotalLabels()
    {
        int soLuong = Number_cl.Check_Int((txt_soluong.Text ?? "").Trim());
        soLuong = ClampInt(soLuong, 1, 999);
        txt_soluong.Text = soLuong.ToString();

        decimal giaVnd = 0m;
        if (ViewState["gia_vnd"] != null)
            decimal.TryParse(ViewState["gia_vnd"].ToString(), out giaVnd);

        decimal tongVnd = giaVnd * soLuong;
        lb_tong_vnd.Text = tongVnd.ToString("#,##0");
        lb_tong_a.Text = ToA(tongVnd).ToString("0.00");
    }

    private bool LoadProduct()
    {
        string idsp = (ViewState["idsp"] ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(idsp))
            return false;

        using (dbDataContext db = new dbDataContext())
        {
            BaiViet_tb sp = AccountVisibility_cl.FindVisibleProductById(db, idsp);
            if (sp == null)
            {
                Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Sản phẩm đã ngừng bán hoặc không tồn tại.", "false", "false", "OK", "alert", "");
                Response.Redirect(ResolveBackUrl(), true);
                return false;
            }

            decimal giaVnd = sp.giaban ?? 0m;
            ViewState["gia_vnd"] = giaVnd;
            lb_ten_sp.Text = sp.name ?? "";
            lb_gia_vnd.Text = giaVnd.ToString("#,##0.##");
            lb_gia_a.Text = ToA(giaVnd).ToString("0.00");

            string image = string.IsNullOrWhiteSpace(sp.image) ? "/uploads/images/macdinh.jpg" : sp.image.Trim();
            img_product.Src = image;
        }

        UpdateTotalLabels();
        return true;
    }

    private string ResolveNguoiBanDangLai(string nguoiBanGoc)
    {
        string userBanCheo = (ViewState["user_bancheo"] ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(userBanCheo))
            return "";
        if (string.Equals(nguoiBanGoc ?? "", userBanCheo, StringComparison.OrdinalIgnoreCase))
            return "";

        using (dbDataContext db = new dbDataContext())
        {
            if (!AccountVisibility_cl.IsSellerVisible(db, userBanCheo))
                return "";
        }

        return userBanCheo;
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
                Response.Redirect(PortalRequest_cl.IsShopPortalRequest() ? "/shop/login.aspx" : "/dang-nhap", true);
                return;
            }

            ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(tkMaHoa);
            ViewState["idsp"] = (Request.QueryString["idsp"] ?? "").Trim();
            ViewState["user_bancheo"] = (Request.QueryString["user_bancheo"] ?? "").Trim();
            ViewState["return_url"] = NormalizeReturnUrl(Request.QueryString["return_url"] ?? "");

            int qty = Number_cl.Check_Int((Request.QueryString["qty"] ?? "1").Trim());
            qty = ClampInt(qty, 1, 999);
            txt_soluong.Text = qty.ToString();

            BindBackLinks();
            if (!LoadProduct())
                return;
        }
    }

    protected void txt_soluong_TextChanged(object sender, EventArgs e)
    {
        UpdateTotalLabels();
    }

    protected void but_xacnhan_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        string tk = (ViewState["taikhoan"] ?? "").ToString();
        string idsp = (ViewState["idsp"] ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(idsp))
        {
            Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Thiếu thông tin thao tác giỏ hàng.", "false", "false", "OK", "alert", "");
            Response.Redirect(ResolveBackUrl(), true);
            return;
        }

        int soLuong = Number_cl.Check_Int((txt_soluong.Text ?? "").Trim());
        soLuong = ClampInt(soLuong, 1, 999);
        txt_soluong.Text = soLuong.ToString();

        using (dbDataContext db = new dbDataContext())
        {
            BaiViet_tb sp = AccountVisibility_cl.FindVisibleProductById(db, idsp);
            if (sp == null)
            {
                Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Sản phẩm đã ngừng bán hoặc không tồn tại.", "false", "false", "OK", "alert", "");
                Response.Redirect(ResolveBackUrl(), true);
                return;
            }

            string nguoiBanDangLai = ResolveNguoiBanDangLai(sp.nguoitao);

            GioHang_tb q = db.GioHang_tbs.FirstOrDefault(p => p.idsp == idsp && p.taikhoan == tk);
            if (q != null)
            {
                q.soluong = q.soluong + soLuong;
                q.ngaythem = AhaTime_cl.Now;
                if (!string.IsNullOrEmpty(nguoiBanDangLai))
                    q.nguoiban_danglai = nguoiBanDangLai;
            }
            else
            {
                GioHang_tb ob = new GioHang_tb();
                ob.ngaythem = AhaTime_cl.Now;
                ob.taikhoan = tk;
                ob.idsp = idsp;
                ob.soluong = soLuong;
                ob.nguoiban_goc = sp.nguoitao;
                ob.nguoiban_danglai = nguoiBanDangLai;
                db.GioHang_tbs.InsertOnSubmit(ob);
            }

            db.SubmitChanges();
        }

        Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Đã thêm sản phẩm vào giỏ hàng.", "false", "false", "OK", "alert", "");
        Response.Redirect(ResolveBackUrl(), true);
    }
}
