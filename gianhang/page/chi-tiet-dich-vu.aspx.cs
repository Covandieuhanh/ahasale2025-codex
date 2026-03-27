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
    public void opengraph(GH_SanPham_tb service)
    {
        title_web = service == null ? string.Empty : (service.ten ?? string.Empty);
        des = service == null ? string.Empty : (service.mo_ta ?? string.Empty);
        image = GianHangStorefront_cl.ResolveImageUrl(service == null ? string.Empty : service.hinh_anh);
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
            GH_SanPham_tb service = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
            if (service == null || GianHangProduct_cl.NormalizeLoai(service.loai) != GianHangProduct_cl.LoaiDichVu)
            {
                Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect(GianHangRoutes_cl.BuildStorefrontUrl(string.Empty));
            }
            else
            {
                taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, service.shop_taikhoan);
                if (account == null || !SpaceAccess_cl.CanAccessGianHang(db, account))
                {
                    Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect(GianHangRoutes_cl.BuildStorefrontUrl((service.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant()));
                }
                else
                {
                    service.luot_truy_cap = (service.luot_truy_cap ?? 0) + 1;
                    service.ngay_cap_nhat = AhaTime_cl.Now;
                    db.SubmitChanges();

                    _idmenu = (service.id_danhmuc ?? string.Empty).Trim();
                    string chiNhanhId = GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request);
                    if (_idmenu == string.Empty || !GianHangMenu_cl.Exists(db, chiNhanhId, _idmenu))
                        _idmenu = GianHangStorefront_cl.ResolveDefaultMenuId(db, chiNhanhId, GianHangStorefront_cl.MenuTypeService);

                    web_menu_table menu = GianHangMenu_cl.FindById(db, chiNhanhId, _idmenu);
                    name_mn = menu == null ? string.Empty : (menu.name ?? string.Empty);
                    name_mn_en = menu == null ? string.Empty : (menu.name_en ?? string.Empty);
                    phanloai_menu = menu == null ? GianHangStorefront_cl.MenuTypeService : (menu.phanloai ?? GianHangStorefront_cl.MenuTypeService);
                    url_menu = GianHangPublic_cl.AppendUserToUrl("/gianhang/page/danh-sach-dich-vu.aspx?idmn=" + HttpUtility.UrlEncode(_idmenu), (service.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant());

                    opengraph(service);

                    noidung = service.noi_dung ?? string.Empty;
                    gia = (service.gia_ban ?? 0m).ToString("#,##0");

                    var q2 = GianHangProduct_cl.QueryPublicByStorefront(db, (service.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant())
                        .Where(p => p.id != service.id
                                    && p.loai == GianHangProduct_cl.LoaiDichVu
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

        return "/gianhang/xem-dich-vu.aspx?id=" + parsedId.ToString();
    }

    protected void but_dathang_Click(object sender, EventArgs e)
    {
        string returnUrl = "/gianhang/page/chi-tiet-dich-vu.aspx?idbv=" + HttpUtility.UrlEncode(idbv);
        int parsedId = 0;
        int.TryParse((idbv ?? string.Empty).Trim(), out parsedId);
        GH_SanPham_tb service = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
        int routeId = service != null ? service.id : parsedId;
        string gianHangTaiKhoan = service == null ? string.Empty : ((service.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant());

        string url = "/gianhang/datlich.aspx?id=" + routeId.ToString();
        url = GianHangRoutes_cl.AppendUserToUrl(url, gianHangTaiKhoan);
        url = GianHangRoutes_cl.AppendReturnUrl(url, returnUrl);
        Response.Redirect(url);
    }
}
