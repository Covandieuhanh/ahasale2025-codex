using System;
using System.Globalization;
using System.Linq;
using System.Web;

public partial class gianhang_chi_tiet_san_pham : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        if (!IsPostBack)
            LoadProduct(info.AccountKey);
    }

    private void LoadProduct(string accountKey)
    {
        int id;
        if (string.IsNullOrWhiteSpace(accountKey) || !int.TryParse(Request.QueryString["id"], out id))
        {
            ShowNotFound();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);
            var sp = db.GetTable<GH_SanPham_tb>()
                .FirstOrDefault(p => p.id == id && p.shop_taikhoan == accountKey && (p.bin == null || p.bin == false));
            if (sp == null || GianHangProduct_cl.NormalizeLoai(sp.loai) != GianHangProduct_cl.LoaiSanPham)
            {
                ShowNotFound();
                return;
            }

            sp.luot_truy_cap = (sp.luot_truy_cap ?? 0) + 1;
            sp.ngay_cap_nhat = AhaTime_cl.Now;
            db.SubmitChanges();

            lb_name.Text = sp.ten ?? "";
            lb_desc.Text = sp.mo_ta ?? "";
            if ((sp.gia_ban ?? 0) <= 0)
                lb_price.Text = "Liên hệ";
            else
                lb_price.Text = (sp.gia_ban ?? 0).ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN"));

            img_product.ImageUrl = string.IsNullOrWhiteSpace(sp.hinh_anh) ? "/uploads/images/macdinh.jpg" : sp.hinh_anh;
            lit_content.Text = sp.noi_dung ?? "";
            lnk_back.NavigateUrl = "/gianhang/default.aspx";

            ViewState["gianhang_taikhoan"] = accountKey;
            ViewState["id"] = sp.id;
            pn_not_found.Visible = false;
            pn_content.Visible = true;
        }
    }

    private void ShowNotFound()
    {
        pn_not_found.Visible = true;
        pn_content.Visible = false;
    }

    protected void but_add_Click(object sender, EventArgs e)
    {
        RedirectToCart(false);
    }

    protected void but_order_Click(object sender, EventArgs e)
    {
        RedirectToCart(true);
    }

    private void RedirectToCart(bool directOrder)
    {
        string idRaw = ViewState["id"] != null ? ViewState["id"].ToString() : "";
        if (string.IsNullOrEmpty(idRaw))
            return;

        int soLuong = Number_cl.Check_Int((txt_soluong.Text ?? "1").Trim());
        if (soLuong <= 0)
            soLuong = 1;

        string returnUrl = "/gianhang/chi-tiet-san-pham.aspx?id=" + HttpUtility.UrlEncode(idRaw);
        string url = "/gianhang/don-ban.aspx?taodon=1&idsp=" + HttpUtility.UrlEncode(idRaw)
            + "&qty=" + HttpUtility.UrlEncode(soLuong.ToString())
            + "&return_url=" + HttpUtility.UrlEncode(returnUrl);
        Response.Redirect(url);
    }
}
