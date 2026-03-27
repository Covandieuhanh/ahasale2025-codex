using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class danh_sach_bai_viet : System.Web.UI.Page
{
    public string notifi = "", idbv = "", noidung = "", name_mn, name_mn_en, phanloai_menu, url_menu;
    public string title_web = "", des = "", image = "", gia = "";

    dbDataContext db = new dbDataContext();

    public string meta = "";
    public void opengraph(GH_SanPham_tb product)
    {
        title_web = product == null ? string.Empty : (product.ten ?? string.Empty);
        des = product == null ? string.Empty : (product.mo_ta ?? string.Empty);
        image = GianHangStorefront_cl.ResolveImageUrl(product == null ? string.Empty : product.hinh_anh);
        this.Title = title_web;
        string _title_op = "<meta property=\"og:title\" content=\"" + title_web + "\" />";
        string _image = "<meta property=\"og:image\" content=\"" + image + "\" />";
        string _description = "<meta name=\"description\" content=\"" + des + "\" />";
        string _description_op = "<meta property=\"og:description\" content=\"" + des + "\" />";
        meta = _title_op + _image + _description + _description_op;
    }

    public string _idmenu;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Request.QueryString["idbv"]))
        {
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect(GianHangRoutes_cl.BuildStorefrontUrl(string.Empty));
        }
        else
        {
            idbv = Request.QueryString["idbv"].ToString().Trim();
            int parsedId = 0;
            int.TryParse(idbv, out parsedId);
            GH_SanPham_tb product = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
            if (product == null || GianHangProduct_cl.NormalizeLoai(product.loai) != GianHangProduct_cl.LoaiSanPham)
            {
                Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect(GianHangRoutes_cl.BuildStorefrontUrl(string.Empty));
            }
            else
            {
                taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, product.shop_taikhoan);
                if (account == null || !SpaceAccess_cl.CanAccessGianHang(db, account))
                {
                    Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect(GianHangRoutes_cl.BuildStorefrontUrl((product.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant()));
                }
                else
                {
                    product.luot_truy_cap = (product.luot_truy_cap ?? 0) + 1;
                    product.ngay_cap_nhat = AhaTime_cl.Now;
                    db.SubmitChanges();

                    _idmenu = (product.id_danhmuc ?? string.Empty).Trim();
                    string chiNhanhId = GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request);
                    if (_idmenu == string.Empty || !GianHangMenu_cl.Exists(db, chiNhanhId, _idmenu))
                        _idmenu = GianHangStorefront_cl.ResolveDefaultMenuId(db, chiNhanhId, GianHangStorefront_cl.MenuTypeProduct);

                    web_menu_table menu = GianHangMenu_cl.FindById(db, chiNhanhId, _idmenu);
                    name_mn = menu == null ? string.Empty : (menu.name ?? string.Empty);
                    name_mn_en = menu == null ? string.Empty : (menu.name_en ?? string.Empty);
                    phanloai_menu = menu == null ? GianHangStorefront_cl.MenuTypeProduct : (menu.phanloai ?? GianHangStorefront_cl.MenuTypeProduct);
                    url_menu = GianHangPublic_cl.AppendUserToUrl("/gianhang/page/danh-sach-san-pham.aspx?idmn=" + HttpUtility.UrlEncode(_idmenu), (product.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant());

                    opengraph(product);
                    noidung = product.noi_dung ?? string.Empty;
                    gia = (product.gia_ban ?? 0m).ToString("#,##0");

                    var q2 = GianHangProduct_cl.QueryPublicByStorefront(db, (product.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant())
                        .Where(p => p.id != product.id
                                    && p.loai == GianHangProduct_cl.LoaiSanPham
                                    && ((_idmenu == string.Empty) || ((p.id_danhmuc ?? string.Empty).Trim() == _idmenu)))
                        .OrderByDescending(p => p.ngay_tao)
                        .Select(p => new
                        {
                            id = p.id,
                            image = p.hinh_anh,
                            name = p.ten ?? string.Empty,
                            description = p.mo_ta ?? string.Empty
                        })
                        .Take(9)
                        .ToList()
                        .Select(p => new
                        {
                            id = p.id,
                            image = GianHangStorefront_cl.ResolveImageUrl(p.image),
                            name = p.name,
                            description = p.description
                        })
                        .ToList();

                    Repeater2.DataSource = q2;
                    Repeater2.DataBind();
                    if (q2.Count == 0)
                        Panel1.Visible = false;
                }
            }
        }
    }

    protected string BuildRelatedDetailUrl(object rawId)
    {
        int parsedId;
        if (!int.TryParse(Convert.ToString(rawId), out parsedId) || parsedId <= 0)
            return "/gianhang/default.aspx";

        return "/gianhang/xem-san-pham.aspx?id=" + parsedId.ToString();
    }

    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        string returnUrl = "/gianhang/page/chi-tiet-san-pham.aspx?idbv=" + HttpUtility.UrlEncode(idbv);
        Response.Redirect(BuildCartActionUrl(false, returnUrl));
    }
    protected void but_dathang_Click(object sender, EventArgs e)
    {
        string returnUrl = "/gianhang/page/chi-tiet-san-pham.aspx?idbv=" + HttpUtility.UrlEncode(idbv);
        Response.Redirect(BuildCartActionUrl(true, returnUrl));
    }

    private string BuildCartActionUrl(bool checkoutNow, string returnUrl)
    {
        int parsedId = 0;
        int.TryParse((idbv ?? string.Empty).Trim(), out parsedId);
        GH_SanPham_tb product = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
        int routeId = product != null ? product.id : parsedId;
        string gianHangTaiKhoan = product == null ? string.Empty : ((product.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant());

        int quantity = 1;
        int.TryParse((txt_soluong_dv.Text ?? string.Empty).Trim(), out quantity);
        if (quantity <= 0)
            quantity = 1;
        if (quantity > 9999)
            quantity = 9999;

        string url = GianHangRoutes_cl.BuildCartUrl(gianHangTaiKhoan, returnUrl);
        url += (url.IndexOf('?') >= 0 ? "&" : "?") + "id=" + routeId.ToString() + "&qty=" + quantity.ToString();
        if (checkoutNow)
            url += "&focus=checkout";
        return url;
    }
}
