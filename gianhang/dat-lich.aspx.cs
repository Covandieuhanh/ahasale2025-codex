using System;
using System.Globalization;
using System.Linq;

public partial class gianhang_dat_lich : System.Web.UI.Page
{
    private string _gianHangTaiKhoan;

    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        _gianHangTaiKhoan = (info.AccountKey ?? "").Trim().ToLowerInvariant();
        ViewState["gianhang_taikhoan"] = _gianHangTaiKhoan;

        if (!IsPostBack)
        {
            InitGianHang();
            InitDefaults();
        }
    }

    private void InitGianHang()
    {
        if (string.IsNullOrEmpty(_gianHangTaiKhoan))
        {
            pn_not_found.Visible = true;
            pn_form.Visible = false;
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _gianHangTaiKhoan);
            if (acc == null)
            {
                pn_not_found.Visible = true;
                pn_form.Visible = false;
                return;
            }

            pn_not_found.Visible = false;
            pn_form.Visible = true;

            lb_gianhang_name.Text = string.IsNullOrWhiteSpace(acc.ten_shop) ? acc.taikhoan : acc.ten_shop;
            lb_gianhang_desc.Text = acc.motangan_shop ?? "";

            BindServices(db, _gianHangTaiKhoan);
        }
    }

    private void InitDefaults()
    {
        if (txt_ngay != null && string.IsNullOrWhiteSpace(txt_ngay.Text))
            txt_ngay.Text = DateTime.Now.ToString("yyyy-MM-dd");
        if (txt_gio != null && string.IsNullOrWhiteSpace(txt_gio.Text))
            txt_gio.Text = DateTime.Now.ToString("HH:mm");
    }

    private void BindServices(dbDataContext db, string gianHangTaiKhoan)
    {
        if (ddl_dichvu == null || db == null)
            return;

        var list = GianHangProduct_cl.QueryPublicByStorefront(db, gianHangTaiKhoan)
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
        string gianHangTaiKhoan = ((ViewState["gianhang_taikhoan"] ?? "").ToString() ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(gianHangTaiKhoan))
        {
            RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
            gianHangTaiKhoan = info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
            ViewState["gianhang_taikhoan"] = gianHangTaiKhoan;
        }

        if (string.IsNullOrEmpty(gianHangTaiKhoan))
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
            GH_SanPham_tb selectedService = GianHangBooking_cl.ResolvePublicService(
                db,
                gianHangTaiKhoan,
                ddl_dichvu == null ? "" : (ddl_dichvu.SelectedValue ?? ""));

            var booking = GianHangBooking_cl.CreateBooking(db, gianHangTaiKhoan, ten, sdt, selectedService, dichvu, thoiGianHen, ghichu);
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
        InitDefaults();
        txt_ghichu.Text = "";
    }
}
