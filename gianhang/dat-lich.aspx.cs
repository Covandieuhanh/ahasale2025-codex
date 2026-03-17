using System;
using System.Globalization;
using System.Linq;

public partial class gianhang_dat_lich : System.Web.UI.Page
{
    private string _shop;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitShop();
            InitDefaults();
        }
    }

    private void InitShop()
    {
        _shop = (Request.QueryString["user"] ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(_shop))
            _shop = GianHangAuth_cl.ResolveShopAccount();

        if (string.IsNullOrEmpty(_shop))
        {
            pn_not_found.Visible = true;
            pn_form.Visible = false;
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _shop);
            if (acc == null)
            {
                pn_not_found.Visible = true;
                pn_form.Visible = false;
                return;
            }

            pn_not_found.Visible = false;
            pn_form.Visible = true;

            lb_shop_name.Text = string.IsNullOrWhiteSpace(acc.ten_shop) ? acc.taikhoan : acc.ten_shop;
            lb_shop_desc.Text = acc.motangan_shop ?? "";

            BindServices(db, _shop);
        }
    }

    private void InitDefaults()
    {
        if (txt_ngay != null && string.IsNullOrWhiteSpace(txt_ngay.Text))
            txt_ngay.Text = DateTime.Now.ToString("yyyy-MM-dd");
        if (txt_gio != null && string.IsNullOrWhiteSpace(txt_gio.Text))
            txt_gio.Text = DateTime.Now.ToString("HH:mm");
    }

    private void BindServices(dbDataContext db, string shop)
    {
        if (ddl_dichvu == null || db == null)
            return;

        var list = GianHangProduct_cl.QueryPublicByShop(db, shop)
            .ToList()
            .Where(p => GianHangProduct_cl.NormalizeLoai(p.loai) == GianHangProduct_cl.LoaiDichVu)
            .OrderBy(p => p.ten)
            .Select(p => new { p.id, p.ten })
            .ToList();

        ddl_dichvu.Items.Clear();
        ddl_dichvu.Items.Add(new System.Web.UI.WebControls.ListItem("Chọn dịch vụ", ""));
        foreach (var item in list)
            ddl_dichvu.Items.Add(new System.Web.UI.WebControls.ListItem(item.ten, item.id.ToString()));

        string idRaw = (Request.QueryString["id"] ?? "").Trim();
        int id;
        if (!string.IsNullOrEmpty(idRaw) && int.TryParse(idRaw, out id))
        {
            var selected = ddl_dichvu.Items.FindByValue(id.ToString());
            if (selected != null)
            {
                ddl_dichvu.ClearSelection();
                selected.Selected = true;
            }
        }
    }

    protected void but_submit_Click(object sender, EventArgs e)
    {
        string shop = (Request.QueryString["user"] ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(shop))
            shop = GianHangAuth_cl.ResolveShopAccount();

        if (string.IsNullOrEmpty(shop))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được gian hàng đối tác.", "Thông báo", true, "warning");
            return;
        }

        string ten = (txt_ten.Text ?? "").Trim();
        string sdt = (txt_sdt.Text ?? "").Trim();
        string dichvu = "";
        if (ddl_dichvu != null && !string.IsNullOrEmpty(ddl_dichvu.SelectedValue))
            dichvu = ddl_dichvu.SelectedItem != null ? ddl_dichvu.SelectedItem.Text : "";

        string dichvuKhac = txt_dichvu_khac != null ? (txt_dichvu_khac.Text ?? "").Trim() : "";
        if (!string.IsNullOrEmpty(dichvuKhac))
        {
            if (string.IsNullOrEmpty(dichvu))
                dichvu = dichvuKhac;
            else if (!dichvu.Equals(dichvuKhac, StringComparison.OrdinalIgnoreCase))
                dichvu = dichvu + " - " + dichvuKhac;
        }
        string ghichu = txt_ghichu.Text ?? "";

        if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(sdt))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập họ tên và số điện thoại.", "Thông báo", true, "warning");
            return;
        }

        DateTime? thoiGianHen = null;
        DateTime ngay;
        if (DateTime.TryParse(txt_ngay.Text, CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out ngay))
        {
            TimeSpan gio;
            if (TimeSpan.TryParse(txt_gio.Text, out gio))
                thoiGianHen = ngay.Date + gio;
            else
                thoiGianHen = ngay.Date;
        }

        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);
            var booking = GianHangBooking_cl.CreateBooking(db, shop, ten, sdt, dichvu, thoiGianHen, ghichu);
            if (booking == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không thể gửi lịch hẹn. Vui lòng thử lại.", "Thông báo", true, "warning");
                return;
            }
        }

        Helper_Tabler_cl.ShowToast(this.Page, "Đã gửi lịch hẹn. Gian hàng sẽ liên hệ xác nhận.", "success", true, 2500, "Thành công");
        txt_ten.Text = "";
        txt_sdt.Text = "";
        if (ddl_dichvu != null && ddl_dichvu.Items.Count > 0)
            ddl_dichvu.SelectedIndex = 0;
        if (txt_dichvu_khac != null)
            txt_dichvu_khac.Text = "";
        txt_ngay.Text = "";
        txt_gio.Text = "";
        txt_ghichu.Text = "";
    }
}
