using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class danh_sach_bai_viet : System.Web.UI.Page
{
    public string notifi = "", idmn = "", tenmn, mota;
    public string title_web = "", des = "";
    public string key = "";
    public string meta = "";

    dbDataContext db = new dbDataContext();
    public int stt = 1, current_page = 1, total_page = 1, show = 21;
    List<string> list_id_split;

    private sealed class NativeProductListItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public decimal? giaban { get; set; }
        public DateTime? ngaytao { get; set; }
        public string loai { get; set; }
    }

    public void opengraph(string _idmn)
    {
        web_menu_table q = GianHangMenu_cl.FindById(db, ResolveCurrentChiNhanhId(), _idmn);
        if (q == null)
            return;

        title_web = q.name;
        des = q.description;
        this.Title = title_web;
        string _title_op = "<meta property=\"og:title\" content=\"" + title_web + "\" />";
        string _image = "<meta property=\"og:image\" content=\"" + (q.image ?? "") + "\" />";
        string _description = "<meta name=\"description\" content=\"" + des + "\" />";
        string _description_op = "<meta property=\"og:description\" content=\"" + des + "\" />";
        meta = _title_op + _image + _description + _description_op;
    }

    public string return_name_category(string _idmn)
    {
        web_menu_table q = GianHangMenu_cl.FindById(db, ResolveCurrentChiNhanhId(), _idmn);
        return q == null ? string.Empty : (q.name ?? string.Empty);
    }

    private string ResolveCurrentChiNhanhId()
    {
        return GianHangPublic_cl.ResolveCurrentChiNhanhId(db, Request);
    }

    private string ResolveRequestedMenuId()
    {
        string requestedId = (Request.QueryString["idmn"] ?? string.Empty).Trim();
        if (requestedId != string.Empty)
            return requestedId;

        string chiNhanhId = ResolveCurrentChiNhanhId();
        return GianHangStorefront_cl.ResolveDefaultMenuId(db, chiNhanhId, GianHangStorefront_cl.MenuTypeProduct);
    }

    private string ResolveStoreAccountKey()
    {
        return GianHangPublic_cl.ResolveCurrentStoreAccountKey(db, Request);
    }

    private web_menu_table ResolveValidMenu(string storeAccountKey)
    {
        web_menu_table menu = ResolveMenuById(ResolveRequestedMenuId(), GianHangStorefront_cl.MenuTypeProduct);
        if (menu != null)
            return menu;

        string chiNhanhId = ResolveCurrentChiNhanhId();
        menu = ResolveMenuById(GianHangStorefront_cl.ResolveDefaultMenuId(db, chiNhanhId, GianHangStorefront_cl.MenuTypeProduct), GianHangStorefront_cl.MenuTypeProduct);
        if (menu != null)
            return menu;

        if (!string.IsNullOrWhiteSpace(storeAccountKey))
        {
            string firstMenuId = GianHangProduct_cl.QueryPublicByStorefront(db, storeAccountKey)
                .Select(p => new
                {
                    loai = p.loai,
                    id_danhmuc = p.id_danhmuc
                })
                .ToList()
                .Where(p => (p.loai ?? string.Empty) == GianHangProduct_cl.LoaiSanPham
                            && (p.id_danhmuc ?? string.Empty).Trim() != string.Empty)
                .Select(p => (p.id_danhmuc ?? string.Empty).Trim())
                .FirstOrDefault();

            menu = ResolveMenuById(firstMenuId, GianHangStorefront_cl.MenuTypeProduct);
            if (menu != null)
                return menu;
        }

        return null;
    }

    private web_menu_table ResolveMenuById(string rawMenuId, string expectedType)
    {
        string menuId = (rawMenuId ?? string.Empty).Trim();
        web_menu_table menu = GianHangMenu_cl.FindById(db, ResolveCurrentChiNhanhId(), menuId);
        if (menu == null)
            return null;

        string menuType = (menu.phanloai ?? string.Empty).Trim().ToLowerInvariant();
        string normalizedExpected = (expectedType ?? string.Empty).Trim().ToLowerInvariant();
        if (menuType != normalizedExpected)
            return null;

        return menu;
    }

    private void RedirectToStorefront(string message, string storeAccountKey)
    {
        if (!string.IsNullOrWhiteSpace(message))
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", message, "false", "false", "OK", "alert", "");

        string targetUrl = GianHangRoutes_cl.BuildStorefrontUrl(storeAccountKey);

        Response.Redirect(targetUrl, false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string storeAccountKey = ResolveStoreAccountKey();
        if (storeAccountKey == string.Empty)
        {
            RedirectToStorefront("Khong xac dinh duoc gian hang dang hoat dong.", storeAccountKey);
            return;
        }

        web_menu_table menu = ResolveValidMenu(storeAccountKey);
        if (menu == null)
        {
            RedirectToStorefront("Danh muc san pham khong hop le.", storeAccountKey);
            return;
        }

        idmn = menu.id.ToString();
        tenmn = menu.name;
        mota = menu.description;
        if (!IsPostBack)
            Session["current_page_home_sanpham"] = "1";

        main();
    }

    protected string BuildDetailUrl(object rawId)
    {
        int parsedId;
        if (!int.TryParse(Convert.ToString(rawId), out parsedId) || parsedId <= 0)
            return "/gianhang/default.aspx";

        return "/gianhang/xem-san-pham.aspx?id=" + parsedId.ToString();
    }

    protected string BuildCartUrl(object rawId)
    {
        return BuildProductActionUrl(rawId, false);
    }

    protected string BuildCheckoutUrl(object rawId)
    {
        return BuildProductActionUrl(rawId, true);
    }

    private string BuildProductActionUrl(object rawId, bool checkoutNow)
    {
        int parsedId;
        if (!int.TryParse(Convert.ToString(rawId), out parsedId) || parsedId <= 0)
            return "/gianhang/default.aspx";

        GH_SanPham_tb product = GianHangPublicOrder_cl.ResolvePublicProductByAnyId(db, null, parsedId);
        int routeId = product != null ? product.id : parsedId;
        string gianHangTaiKhoan = product == null ? ResolveStoreAccountKey() : ((product.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant());
        string returnUrl = Request.RawUrl ?? GianHangRoutes_cl.BuildProductsUrl(gianHangTaiKhoan);

        string url = GianHangRoutes_cl.BuildCartUrl(gianHangTaiKhoan, returnUrl);
        url += (url.IndexOf('?') >= 0 ? "&" : "?") + "id=" + routeId.ToString() + "&qty=1";
        if (checkoutNow)
            url += "&focus=checkout";
        return url;
    }

    public void main()
    {
        opengraph(idmn);
        string storeAccountKey = ResolveStoreAccountKey();
        List<NativeProductListItem> list_all = GianHangProduct_cl.QueryPublicByStorefront(db, storeAccountKey)
            .Where(p => (p.id_danhmuc ?? string.Empty).Trim() == idmn
                        && (p.id_danhmuc ?? string.Empty).Trim() != string.Empty)
            .Select(p => new NativeProductListItem
            {
                id = p.id,
                name = p.ten ?? string.Empty,
                image = p.hinh_anh,
                description = p.mo_ta ?? string.Empty,
                giaban = p.gia_ban,
                ngaytao = p.ngay_tao,
                loai = p.loai
            })
            .ToList()
            .Where(p => (p.loai ?? string.Empty) == GianHangProduct_cl.LoaiSanPham)
            .OrderByDescending(p => p.ngaytao)
            .ToList();

        for (int i = 0; i < list_all.Count; i++)
            list_all[i].image = GianHangStorefront_cl.ResolveImageUrl(list_all[i].image);

        string _key = (txt_search.Text ?? string.Empty).Trim().ToLowerInvariant();
        if (_key != string.Empty)
        {
            list_all = list_all.Where(p =>
                    (p.name ?? string.Empty).ToLowerInvariant().Contains(_key)
                    || p.id.ToString() == _key
                    || (p.description ?? string.Empty).ToLowerInvariant().Contains(_key))
                .ToList();
        }

        total_page = number_of_page_class.return_total_page(list_all.Count, show);
        current_page = int.Parse(Session["current_page_home_sanpham"].ToString());
        if (current_page > total_page)
            current_page = total_page;
        if (current_page < 1)
            current_page = 1;

        but_xemtiep.Visible = current_page < total_page;
        but_quaylai.Visible = current_page > 1;

        stt = (show * current_page) - show + 1;
        List<NativeProductListItem> list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
        list_id_split = new List<string>(list_split.Count);
        foreach (NativeProductListItem t in list_split)
            list_id_split.Add("check_" + t.id);

        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }

    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        Session["search_baiviet_home"] = txt_search.Text.Trim();
        Session["current_page_home_sanpham"] = "1";
        main();
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_home_sanpham"] = int.Parse(Session["current_page_home_sanpham"].ToString()) - 1;
        if (int.Parse(Session["current_page_home_sanpham"].ToString()) < 1)
            Session["current_page_home_sanpham"] = 1;
        main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_home_sanpham"] = int.Parse(Session["current_page_home_sanpham"].ToString()) + 1;
        if (int.Parse(Session["current_page_home_sanpham"].ToString()) > total_page)
            Session["current_page_home_sanpham"] = total_page;
        main();
    }
}
