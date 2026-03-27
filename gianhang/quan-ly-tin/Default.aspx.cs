using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class gianhang_quan_ly_tin_Default : Page
{
    private sealed class PostRowView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string PostType { get; set; }
        public bool IsHidden { get; set; }
        public string CategoryName { get; set; }
        public string PriceText { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedAtText { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        ViewState["gianhang_taikhoan"] = (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (!IsPostBack)
            BindPosts();
    }

    private string CurrentAccountKey()
    {
        return ((ViewState["gianhang_taikhoan"] ?? "").ToString() ?? "").Trim().ToLowerInvariant();
    }

    private string CurrentKeyword()
    {
        return (txt_timkiem.Text ?? "").Trim();
    }

    private string CurrentLoai()
    {
        string value = (ddl_loai.SelectedValue ?? "all").Trim().ToLowerInvariant();
        if (value == GianHangProduct_cl.LoaiSanPham || value == GianHangProduct_cl.LoaiDichVu)
            return value;
        return "all";
    }

    private string CurrentTrangThai()
    {
        string value = (ddl_trangthai.SelectedValue ?? "all").Trim().ToLowerInvariant();
        if (value == "active" || value == "hidden")
            return value;
        return "all";
    }

    private void BindPosts()
    {
        string accountKey = CurrentAccountKey();
        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);

            IQueryable<GH_SanPham_tb> source = GianHangProduct_cl.QueryByStorefront(db, accountKey);

            string loai = CurrentLoai();
            if (loai != "all")
                source = source.Where(p => p.loai == loai);

            string trangThai = CurrentTrangThai();
            if (trangThai == "active")
                source = source.Where(p => p.bin == null || p.bin == false);
            else if (trangThai == "hidden")
                source = source.Where(p => p.bin == true);

            string keyword = CurrentKeyword();
            int idKeyword;
            bool hasIdKeyword = int.TryParse(keyword, out idKeyword);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                source = source.Where(p =>
                    (p.ten ?? "").Contains(keyword)
                    || (p.mo_ta ?? "").Contains(keyword)
                    || (hasIdKeyword && p.id == idKeyword));
            }

            List<PostRowView> rows = (from sp in source
                                      join dm in db.DanhMuc_tbs on sp.id_danhmuc equals dm.id.ToString() into dmGroup
                                      from dm in dmGroup.DefaultIfEmpty()
                                      orderby sp.ngay_cap_nhat descending, sp.id descending
                                      select new
                                      {
                                          Id = sp.id,
                                          Name = sp.ten ?? "",
                                          Description = sp.mo_ta ?? "",
                                          ImageUrlRaw = sp.hinh_anh,
                                          PostType = sp.loai,
                                          IsHidden = sp.bin == true,
                                          CategoryName = dm != null ? (dm.name ?? "") : "",
                                          Price = sp.gia_ban ?? 0m,
                                          UpdatedAt = sp.ngay_cap_nhat ?? sp.ngay_tao
                                      }).ToList()
                                      .Select(sp => new PostRowView
                                      {
                                          Id = sp.Id,
                                          Name = sp.Name,
                                          Description = sp.Description,
                                          ImageUrl = GianHangStorefront_cl.ResolveImageUrl(sp.ImageUrlRaw),
                                          PostType = (sp.PostType ?? "").Trim().ToLowerInvariant(),
                                          IsHidden = sp.IsHidden,
                                          CategoryName = sp.CategoryName,
                                          PriceText = sp.Price <= 0m ? "Liên hệ" : sp.Price.ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) + " đ",
                                          UpdatedAt = sp.UpdatedAt,
                                          UpdatedAtText = sp.UpdatedAt.HasValue ? sp.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--"
                                      }).ToList();

            int total = GianHangProduct_cl.QueryByStorefront(db, accountKey).Count();
            int active = GianHangProduct_cl.QueryByStorefront(db, accountKey).Count(p => p.bin == null || p.bin == false);
            int hidden = total - active;
            int products = GianHangProduct_cl.QueryByStorefront(db, accountKey).Count(p => p.loai == GianHangProduct_cl.LoaiSanPham);
            int services = GianHangProduct_cl.QueryByStorefront(db, accountKey).Count(p => p.loai == GianHangProduct_cl.LoaiDichVu);

            lb_total.Text = total.ToString("#,##0");
            lb_active.Text = active.ToString("#,##0");
            lb_hidden.Text = hidden.ToString("#,##0");
            lb_products.Text = products.ToString("#,##0");
            lb_services.Text = services.ToString("#,##0");
            lb_result_summary.Text = rows.Count == 0
                ? "Không có tin phù hợp với bộ lọc hiện tại."
                : string.Format("Hiển thị {0} tin phù hợp.", rows.Count);

            ph_empty.Visible = rows.Count == 0;
            rpt_posts.DataSource = rows;
            rpt_posts.DataBind();
        }
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        BindPosts();
    }

    protected void ddl_loai_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindPosts();
    }

    protected void ddl_trangthai_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindPosts();
    }

    protected void but_refresh_Click(object sender, EventArgs e)
    {
        BindPosts();
    }

    protected void rpt_posts_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (!string.Equals(e.CommandName, "toggle", StringComparison.OrdinalIgnoreCase))
            return;

        int id;
        if (!int.TryParse((e.CommandArgument ?? "").ToString(), out id))
            return;

        string accountKey = CurrentAccountKey();
        using (dbDataContext db = new dbDataContext())
        {
            bool ok = GianHangProduct_cl.ToggleVisibility(db, id, accountKey);
            Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                ok ? "Đã cập nhật trạng thái hiển thị của tin." : "Không thể cập nhật trạng thái của tin.",
                "1200",
                ok ? "success" : "warning");
        }

        BindPosts();
    }
}
