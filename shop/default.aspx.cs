using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

public partial class shop_default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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

                BindHeaderAndStats(db, acc);
                BindProducts(db, tk);
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

    private void BindHeaderAndStats(dbDataContext db, taikhoan_tb acc)
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

        lb_public_path.Text = publicPath;

        lnk_public_shop.NavigateUrl = publicPath;
        lnk_public_shop.Text = "Xem trang công khai";
        lnk_public_shop_top.NavigateUrl = publicPath;
        lnk_public_shop_top.Text = fullPublicUrl;
    }

    private void BindProducts(dbDataContext db, string tk)
    {
        var products = db.BaiViet_tbs
            .Where(p => p.nguoitao == tk && p.bin == false && p.phanloai == "sanpham")
            .OrderByDescending(p => p.ngaytao)
            .Select(p => new
            {
                p.id,
                p.name,
                p.name_en,
                p.image,
                p.giaban,
                p.ngaytao,
                LuotTruyCap = (p.LuotTruyCap ?? 0)
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
        int pendingOrders = db.DonHang_tbs.Count(p =>
            p.nguoiban == tk &&
            (
                p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
            ));

        lb_total_products.Text = totalProducts.ToString("#,##0");
        lb_total_views.Text = totalViews.ToString("#,##0");
        lb_total_sold.Text = totalSold.ToString("#,##0");
        lb_pending_orders.Text = pendingOrders.ToString("#,##0");
    }

    protected string ResolveProductImage(object imageRaw)
    {
        string image = (imageRaw ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(image))
            return "/uploads/images/macdinh.jpg";
        return image;
    }

    protected string FormatCurrency(object valueRaw)
    {
        decimal value;
        decimal.TryParse(Convert.ToString(valueRaw, CultureInfo.InvariantCulture), out value);
        return value.ToString("#,##0.##");
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
}
