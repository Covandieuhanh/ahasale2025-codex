using System;
using System.Globalization;
using System.Linq;

public partial class gianhang_chi_tiet_dich_vu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string id = Request.QueryString["id"] ?? "";
        if (!string.IsNullOrWhiteSpace(id))
        {
            Response.Redirect("/gianhang/page/chi-tiet-dich-vu.aspx?id=" + id);
            return;
        }
        Response.Redirect("/gianhang/Default.aspx");
        return;
        if (!IsPostBack)
            LoadService();
    }

    private void LoadService()
    {
        string tk = (Request.QueryString["user"] ?? "").Trim().ToLowerInvariant();
        int id;
        if (string.IsNullOrEmpty(tk) || !int.TryParse(Request.QueryString["id"], out id))
        {
            ShowNotFound();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);
            var sp = db.GetTable<GH_SanPham_tb>()
                .FirstOrDefault(p => p.id == id && (p.shop_taikhoan ?? "").Trim().ToLower() == tk && p.bin != true);
            if (sp == null || GianHangProduct_cl.NormalizeLoai(sp.loai) != GianHangProduct_cl.LoaiDichVu)
            {
                ShowNotFound();
                return;
            }

            sp.luot_truy_cap = (sp.luot_truy_cap ?? 0) + 1;
            sp.ngay_cap_nhat = AhaTime_cl.Now;
            db.SubmitChanges();
            GianHangProduct_cl.SyncToHome(db, sp);

            lb_name.Text = sp.ten ?? "";
            lb_desc.Text = sp.mo_ta ?? "";
            if ((sp.gia_ban ?? 0) <= 0)
                lb_price.Text = "Liên hệ";
            else
                lb_price.Text = (sp.gia_ban ?? 0).ToString("#,##0", CultureInfo.InvariantCulture);

            img_service.ImageUrl = string.IsNullOrWhiteSpace(sp.hinh_anh) ? "/uploads/images/macdinh.jpg" : sp.hinh_anh;
            lit_content.Text = sp.noi_dung ?? "";
            lnk_back.NavigateUrl = "/gianhang/default.aspx?user=" + tk;

            ViewState["shop"] = tk;
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

    protected void but_book_Click(object sender, EventArgs e)
    {
        string tk = ViewState["shop"] as string ?? "";
        string idRaw = ViewState["id"] != null ? ViewState["id"].ToString() : "";
        if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(idRaw))
            return;

        Response.Redirect("/gianhang/dat-lich.aspx?user=" + tk + "&id=" + idRaw);
    }
}
