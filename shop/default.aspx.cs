using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                SetControlVisible("ph_menu_ban_san_pham", isCompanyShop);
                SetControlVisible("ph_menu_company_space", isCompanyShop);
                SetControlVisible("ph_switch_to_home", PortalActiveMode_cl.HasHomeCredential());
                SetHyperLinkNavigateUrl("lnk_switch_to_home", "/dang-nhap?switch=home");
                string space = ResolveShopSpace(isCompanyShop);
                ViewState["shop_space"] = space;

                SetHyperLinkNavigateUrl("lnk_space_public", "/shop/default.aspx?space=" + ShopSpacePublic);
                SetHyperLinkNavigateUrl("lnk_space_internal", "/shop/default.aspx?space=" + ShopSpaceInternal);
                SetWebControlCssClass("lnk_space_public", GetSpaceMenuCss(ShopSpacePublic));
                SetWebControlCssClass("lnk_space_internal", GetSpaceMenuCss(ShopSpaceInternal));

                if (isCompanyShop && space == ShopSpaceInternal)
                {
                    SetLabelText("lb_space_hero_title", "Không gian 2: Sản phẩm nội bộ");
                    SetLabelText("lb_space_hero_desc", "Dùng riêng cho sản phẩm nội bộ của công ty. Mọi thao tác bán nội bộ chạy trong shop portal.");
                    SetLabelText("lb_space_product_title", "Danh sách sản phẩm nội bộ");
                    SetLabelText("lb_space_product_desc", "Không gian này chỉ hiển thị sản phẩm nội bộ và thao tác bán nội bộ.");
                }
                else
                {
                    SetLabelText("lb_space_hero_title", "Không gian 1: Gian hàng công khai");
                    SetLabelText("lb_space_hero_desc", "Giống shop thường: quản lý sản phẩm công khai, đơn bán, khách hàng và trao đổi.");
                    SetLabelText("lb_space_product_title", "Sản phẩm của gian hàng");
                    SetLabelText("lb_space_product_desc", "Trang chủ shop chỉ hiển thị sản phẩm do chính tài khoản shop này đăng.");
                }

                string shopAccount = string.IsNullOrWhiteSpace(acc.taikhoan) ? tk : acc.taikhoan.Trim();
                BindHeaderAndStats(db, acc, isCompanyShop, space);
                BindProducts(db, shopAccount, isCompanyShop, space);
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

        return (tk ?? "").Trim();
    }

    private void BindHeaderAndStats(dbDataContext db, taikhoan_tb acc, bool isCompanyShop, string space)
    {
        string displayName = string.IsNullOrWhiteSpace(acc.hoten) ? acc.taikhoan : acc.hoten.Trim();
        string avatar = string.IsNullOrWhiteSpace(acc.anhdaidien) ? "/uploads/images/macdinh.jpg" : acc.anhdaidien.Trim();
        string publicPath = ShopSlug_cl.GetPublicUrl(db, acc);
        string fullPublicUrl = Request.Url.GetLeftPart(UriPartial.Authority) + publicPath;

        SetLabelText("lb_taikhoan", acc.taikhoan);
        SetLabelText("lb_hoten", displayName);
        SetLabelText("lb_hoten_short", displayName);

        string phanLoaiText = string.IsNullOrWhiteSpace(acc.phanloai) ? "Gian hàng đối tác" : acc.phanloai;
        if (isCompanyShop)
            phanLoaiText += " • Shop công ty • " + (space == ShopSpaceInternal ? "Không gian nội bộ" : "Không gian công khai");
        SetLabelText("lb_phanloai", phanLoaiText);
        SetLabelText("lb_trangthai", acc.block == true ? "Đang khóa" : "Hoạt động");
        SetImageUrl("img_avatar", avatar);

        SetLabelText("lb_public_path", publicPath);

        SetHyperLinkNavigateUrl("lnk_public_shop", publicPath);
        SetHyperLinkText("lnk_public_shop", "Xem trang công khai");
        SetHyperLinkNavigateUrl("lnk_public_shop_top", publicPath);
        SetHyperLinkText("lnk_public_shop_top", fullPublicUrl);
    }

    private void BindProducts(dbDataContext db, string tk, bool isCompanyShop, string space)
    {
        IQueryable<BaiViet_tb> source = db.BaiViet_tbs.Where(p => p.nguoitao == tk);

        // Public space must use the same visibility filter as shop public page
        // to avoid product/price mismatch between two views.
        if (isCompanyShop && space == ShopSpaceInternal)
        {
            source = source.Where(p => p.bin == false && p.phanloai == CompanyShop_cl.ProductTypeInternal);
        }
        else
        {
            source = AccountVisibility_cl.FilterVisibleProducts(db, source);
        }

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

        BindRepeater("rp_products", products);
        SetControlVisible("ph_empty_products", products.Count == 0);

        int totalProducts = products.Count;
        int totalViews = products.Sum(x => x.LuotTruyCap);
        int totalSold = db.DonHang_ChiTiet_tbs
            .Where(p => p.nguoiban_goc == tk || p.nguoiban_danglai == tk)
            .Select(p => (int?)p.soluong)
            .ToList()
            .Sum() ?? 0;
        int pendingOrders = GetPendingOrdersCompat(db, tk);

        SetLabelText("lb_total_products", totalProducts.ToString("#,##0"));
        SetLabelText("lb_total_views", totalViews.ToString("#,##0"));
        SetLabelText("lb_total_sold", totalSold.ToString("#,##0"));
        SetLabelText("lb_pending_orders", pendingOrders.ToString("#,##0"));
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

    private void SetControlVisible(string id, bool visible)
    {
        Control ctl = FindControlById(id);
        if (ctl != null)
            ctl.Visible = visible;
    }

    private void SetHyperLinkNavigateUrl(string id, string url)
    {
        HyperLink ctl = FindControlById(id) as HyperLink;
        if (ctl != null)
            ctl.NavigateUrl = url ?? "";
    }

    private void SetHyperLinkText(string id, string text)
    {
        HyperLink ctl = FindControlById(id) as HyperLink;
        if (ctl != null)
            ctl.Text = text ?? "";
    }

    private void SetLabelText(string id, string text)
    {
        Label ctl = FindControlById(id) as Label;
        if (ctl != null)
            ctl.Text = text ?? "";
    }

    private void SetImageUrl(string id, string imageUrl)
    {
        Image ctl = FindControlById(id) as Image;
        if (ctl != null)
            ctl.ImageUrl = imageUrl ?? "";
    }

    private void BindRepeater(string id, object dataSource)
    {
        Repeater ctl = FindControlById(id) as Repeater;
        if (ctl != null)
        {
            ctl.DataSource = dataSource;
            ctl.DataBind();
        }
    }

    private void SetWebControlCssClass(string id, string cssClass)
    {
        WebControl ctl = FindControlById(id) as WebControl;
        if (ctl != null)
            ctl.CssClass = cssClass ?? "";
    }

    private Control FindControlById(string id)
    {
        return FindControlRecursive(this, id);
    }

    private static Control FindControlRecursive(Control root, string id)
    {
        if (root == null || string.IsNullOrEmpty(id))
            return null;

        if (string.Equals(root.ID, id, StringComparison.Ordinal))
            return root;

        foreach (Control child in root.Controls)
        {
            Control found = FindControlRecursive(child, id);
            if (found != null)
                return found;
        }

        return null;
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
