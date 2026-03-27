using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

public partial class shop_default : System.Web.UI.Page
{
    private const string ShopSpacePublic = "public";
    private const string ShopSpaceInternal = "internal";

    private string ResolveShopSpace(bool isCompanyShop)
    {
        if (!isCompanyShop)
            return ShopSpacePublic;

        string raw = (Request.QueryString["space"] ?? "").Trim().ToLowerInvariant();
        return raw == ShopSpaceInternal ? ShopSpaceInternal : ShopSpacePublic;
    }

    private bool IsInternalSpace()
    {
        return string.Equals((ViewState["shop_space"] ?? ShopSpacePublic).ToString(), ShopSpaceInternal, StringComparison.OrdinalIgnoreCase);
    }

    protected string GetSpaceMenuCss(string space)
    {
        string expected = (space ?? "").Trim().ToLowerInvariant();
        string current = (ViewState["shop_space"] ?? ShopSpacePublic).ToString().Trim().ToLowerInvariant();
        return expected == current ? "menu-item menu-item-active" : "menu-item";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.Equals((Request.QueryString["switch"] ?? "").Trim(), "shop", StringComparison.OrdinalIgnoreCase))
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);

        check_login_cl.check_login_shop("none", "none", true);

        if (!IsPostBack)
        {
            string tk = ResolveCurrentShopAccount();
            if (string.IsNullOrEmpty(tk))
            {
                Response.Redirect("/shop/login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                if (acc == null || !PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission))
                {
                    check_login_cl.del_all_cookie_session_shop();
                    Response.Redirect("/shop/login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                bool isCompanyShop = CompanyShop_cl.IsCompanyShopAccount(db, tk);
                ph_menu_ban_san_pham.Visible = isCompanyShop;
                ph_menu_company_space.Visible = isCompanyShop;
                ph_switch_to_home.Visible = PortalActiveMode_cl.HasHomeCredential();
                lnk_switch_to_home.NavigateUrl = "/dang-nhap?switch=home";
                string space = ResolveShopSpace(isCompanyShop);
                ViewState["shop_space"] = space;

                lnk_space_public.NavigateUrl = "/shop/default.aspx?space=" + ShopSpacePublic;
                lnk_space_internal.NavigateUrl = "/shop/default.aspx?space=" + ShopSpaceInternal;
                lnk_space_public.CssClass = GetSpaceMenuCss(ShopSpacePublic);
                lnk_space_internal.CssClass = GetSpaceMenuCss(ShopSpaceInternal);

                if (isCompanyShop && space == ShopSpaceInternal)
                {
                    lb_space_hero_title.Text = "Không gian 2: Sản phẩm nội bộ";
                    lb_space_hero_desc.Text = "Dùng riêng cho sản phẩm nội bộ của công ty. Mọi thao tác bán nội bộ chạy trong shop portal.";
                    lb_space_product_title.Text = "Danh sách sản phẩm nội bộ";
                    lb_space_product_desc.Text = "Không gian này chỉ hiển thị sản phẩm nội bộ và thao tác bán nội bộ.";
                }
                else
                {
                    lb_space_hero_title.Text = "Không gian 1: Gian hàng công khai";
                    lb_space_hero_desc.Text = "Giống shop thường: quản lý sản phẩm công khai, đơn bán, khách hàng và thanh toán.";
                    lb_space_product_title.Text = "Sản phẩm của gian hàng";
                    lb_space_product_desc.Text = "Trang chủ shop chỉ hiển thị sản phẩm do chính tài khoản shop này đăng.";
                }

                BindHeaderAndStats(db, acc, isCompanyShop, space);
                BindProducts(db, tk, isCompanyShop, space);
            }
        }
    }

    private string ResolveCurrentShopAccount()
    {
        string tk = "";
        string encodedSession = Session["taikhoan_shop"] as string;
        if (!string.IsNullOrEmpty(encodedSession))
        {
            tk = mahoa_cl.giaima_Bcorn(encodedSession);
        }
        else
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_shop_bcorn"];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
        }

        return (tk ?? "").Trim().ToLower();
    }

    private void BindHeaderAndStats(dbDataContext db, taikhoan_tb acc, bool isCompanyShop, string space)
    {
        string displayName = string.IsNullOrWhiteSpace(acc.hoten) ? acc.taikhoan : acc.hoten.Trim();
        string avatar = string.IsNullOrWhiteSpace(acc.anhdaidien) ? "/uploads/images/macdinh.jpg" : acc.anhdaidien.Trim();
        string publicPath = ShopSlug_cl.GetPublicUrl(db, acc);
        string fullPublicUrl = Request.Url.GetLeftPart(UriPartial.Authority) + publicPath;

        lb_taikhoan.Text = acc.taikhoan;
        lb_hoten.Text = displayName;
        lb_hoten_short.Text = displayName;
        lb_phanloai.Text = string.IsNullOrWhiteSpace(acc.phanloai) ? "Gian hàng đối tác" : acc.phanloai;
        lb_trangthai.Text = acc.block == true ? "Đang khóa" : "Hoạt động";
        img_avatar.ImageUrl = avatar;

        if (isCompanyShop)
            lb_phanloai.Text = lb_phanloai.Text + " • Shop công ty • " + (space == ShopSpaceInternal ? "Không gian nội bộ" : "Không gian công khai");

        lb_public_path.Text = publicPath;

        lnk_public_shop.NavigateUrl = publicPath;
        lnk_public_shop.Text = "Xem trang công khai";
        lnk_public_shop_top.NavigateUrl = publicPath;
        lnk_public_shop_top.Text = fullPublicUrl;
    }

    private void BindProducts(dbDataContext db, string tk, bool isCompanyShop, string space)
    {
        var source = db.BaiViet_tbs
            .Where(p => p.nguoitao == tk && p.bin == false);

        if (isCompanyShop)
        {
            if (space == ShopSpaceInternal)
                source = source.Where(p => p.phanloai == CompanyShop_cl.ProductTypeInternal);
            else
                source = source.Where(p => p.phanloai == CompanyShop_cl.ProductTypePublic);
        }
        else
            source = source.Where(p => p.phanloai == CompanyShop_cl.ProductTypePublic);

        var products = source
            .OrderByDescending(p => p.ngaytao)
            .Select(p => new
            {
                p.id,
                p.name,
                p.name_en,
                p.image,
                p.giaban,
                p.ngaytao,
                LuotTruyCap = (p.LuotTruyCap ?? 0),
                KenhRaw = p.phanloai,
                PhanTram_ChiTra_ChoSan = p.banhang_thuong
            })
            .ToList();

        rp_products.DataSource = products;
        rp_products.DataBind();

        ph_empty_products.Visible = products.Count == 0;

        int totalProducts = products.Count;
        int totalViews = products.Sum(x => x.LuotTruyCap);
        int totalSold = db.DonHang_ChiTiet_tbs
            .Where(p => p.nguoiban_goc == tk || p.nguoiban_danglai == tk)
            .Select(p => (int?)p.soluong)
            .ToList()
            .Sum() ?? 0;
        int pendingOrders = GetPendingOrdersCompat(db, tk);

        lb_total_products.Text = totalProducts.ToString("#,##0");
        lb_total_views.Text = totalViews.ToString("#,##0");
        lb_total_sold.Text = totalSold.ToString("#,##0");
        lb_pending_orders.Text = pendingOrders.ToString("#,##0");
    }

    protected string BuildProductChannelLabel(object raw)
    {
        string phanloai = (raw ?? "").ToString();
        return CompanyShop_cl.IsInternalProductType(phanloai) ? "Nội bộ" : "Công khai";
    }

    protected string BuildProductChannelCss(object raw)
    {
        string phanloai = (raw ?? "").ToString();
        if (CompanyShop_cl.IsInternalProductType(phanloai))
            return "product-badge product-badge-internal";
        return "product-badge product-badge-public";
    }

    protected string BuildSellActionUrl(object idRaw, object productTypeRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);

        string phanloai = (productTypeRaw ?? "").ToString();
        if (CompanyShop_cl.IsInternalProductType(phanloai))
            return "/shop/noi-bo/ban-san-pham?view=sell&idsp=" + id.ToString();

        return BuildCreateOrderUrl(idRaw);
    }

    protected string BuildSellActionText(object productTypeRaw)
    {
        string phanloai = (productTypeRaw ?? "").ToString();
        if (CompanyShop_cl.IsInternalProductType(phanloai))
            return "Bán nội bộ";
        return "Tạo đơn";
    }

    protected string ResolveProductImage(object imageRaw)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        string image = (imageRaw ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(image))
            return fallback;

        if (image.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(image, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        if (image.StartsWith("~/", StringComparison.Ordinal))
            image = image.Substring(1);
        if (!image.StartsWith("/", StringComparison.Ordinal))
            image = "/" + image;

        if (IsMissingUploadFile(image))
            return fallback;

        return image;
    }

    private bool IsMissingUploadFile(string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
            return false;
        if (!relativeUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return false;

        string cleanPath = relativeUrl;
        int q = cleanPath.IndexOf('?');
        if (q >= 0)
            cleanPath = cleanPath.Substring(0, q);

        try
        {
            string physical = Server.MapPath("~" + cleanPath);
            if (string.IsNullOrEmpty(physical))
                return false;
            return !File.Exists(physical);
        }
        catch
        {
            return false;
        }
    }

    protected string FormatCurrency(object valueRaw)
    {
        decimal value = 0m;
        if (valueRaw != null && valueRaw != DBNull.Value)
        {
            try
            {
                value = Convert.ToDecimal(valueRaw, CultureInfo.InvariantCulture);
            }
            catch
            {
                string raw = Convert.ToString(valueRaw, CultureInfo.InvariantCulture);
                if (!decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                    && !decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                {
                    value = 0m;
                }
            }
        }

        return value.ToString("#,##0.##", CultureInfo.GetCultureInfo("vi-VN"));
    }

    protected string BuildCreateOrderUrl(object idRaw)
    {
        int id;
        int.TryParse((idRaw ?? "").ToString(), out id);

        string baseUrl = "/shop/don-ban?taodon=1";
        if (id > 0)
            baseUrl += "&idsp=" + id.ToString();

        baseUrl += "&return_url=" + HttpUtility.UrlEncode("/shop/default.aspx");
        return baseUrl;
    }

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        check_login_cl.del_all_cookie_session_shop();
        Response.Redirect("/shop/login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private int GetPendingOrdersCompat(dbDataContext db, string tk)
    {
        try
        {
            return db.DonHang_tbs.Count(p =>
                p.nguoiban == tk &&
                (
                    p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                    || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                ));
        }
        catch (SqlException ex)
        {
            if (!IsMissingDonHangStatusColumnError(ex))
                throw;

            return db.DonHang_tbs.Count(p =>
                p.nguoiban == tk &&
                p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi);
        }
    }

    private static bool IsMissingDonHangStatusColumnError(SqlException ex)
    {
        if (ex == null)
            return false;

        string message = ex.Message ?? "";
        return message.IndexOf("Invalid column name 'exchange_status'", StringComparison.OrdinalIgnoreCase) >= 0
            || message.IndexOf("Invalid column name 'order_status'", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
