using System;
using System.Globalization;
using System.Linq;
using System.Web;

public partial class gianhang_quan_ly_tin_Them : System.Web.UI.Page
{
    private readonly DanhMuc_cl _danhMuc = new DanhMuc_cl();

    private static string NormalizeProvinceName(string value)
    {
        string name = (value ?? "").Trim();
        if (string.IsNullOrEmpty(name))
            return "";

        if (name.StartsWith("Tỉnh ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(5);
        else if (name.StartsWith("Thành phố ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(10);
        else if (name.StartsWith("TP. ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(4);
        else if (name.StartsWith("TP ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(3);

        return name.Trim();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;

        ViewState["gianhang_taikhoan"] = (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (!IsPostBack)
        {
            txt_giaban.Text = "0";
            LoadDanhMuc();
            LoadForEditIfNeeded();
        }
    }

    private string CurrentAccountKey()
    {
        return ((ViewState["gianhang_taikhoan"] ?? "").ToString() ?? "").Trim().ToLowerInvariant();
    }

    private void LoadDanhMuc()
    {
        _danhMuc.Show_DanhMuc(2, 3, ddl_danhmuc, false, "web", "135");
    }

    private int? CurrentEditId()
    {
        int id;
        if (int.TryParse((Request.QueryString["id"] ?? "").Trim(), out id) && id > 0)
            return id;
        return null;
    }

    private void LoadForEditIfNeeded()
    {
        int? editId = CurrentEditId();
        if (!editId.HasValue)
            return;

        using (dbDataContext db = new dbDataContext())
        {
            GH_SanPham_tb item = GianHangProduct_cl.GetById(db, editId.Value, CurrentAccountKey());
            if (item == null)
            {
                Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Không tìm thấy tin cần chỉnh sửa.", "1200", "warning");
                Response.Redirect("/gianhang/quan-ly-tin/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            lb_form_title.Text = "Cập nhật tin";
            but_submit.Text = "Cập nhật";
            txt_name.Text = item.ten ?? "";
            txt_description.Text = item.mo_ta ?? "";
            txt_noidung.Text = item.noi_dung ?? "";
            txt_link_fileupload.Text = item.hinh_anh ?? "";
            txt_giaban.Text = string.Format("{0:#,##0}", item.gia_ban ?? 0m);
            int phanTramUuDai = GianHangProduct_cl.ResolveDiscountPercent(item);
            txt_phantram_uu_dai.Text = phanTramUuDai.ToString();
            hf_tinh.Value = item.dia_chi_tinh ?? "";
            hf_quan.Value = item.dia_chi_quan ?? "";
            hf_phuong.Value = item.dia_chi_phuong ?? "";
            txt_diachi_chitiet.Text = item.dia_chi_chi_tiet ?? "";
            hf_address_raw.Value = item.dia_diem ?? "";
            chk_hidden.Checked = item.bin == true;

            string loai = GianHangProduct_cl.NormalizeLoai(item.loai);
            if (ddl_loai.Items.FindByValue(loai) != null)
                ddl_loai.SelectedValue = loai;

            string idDanhMuc = (item.id_danhmuc ?? "").Trim();
            if (!string.IsNullOrEmpty(idDanhMuc) && ddl_danhmuc.Items.FindByValue(idDanhMuc) != null)
                ddl_danhmuc.SelectedValue = idDanhMuc;

            if (!string.IsNullOrWhiteSpace(item.hinh_anh))
            {
                string safeUrl = HttpUtility.HtmlEncode(item.hinh_anh);
                lit_uploaded_main.Text = "<div class='small text-muted mb-1'>Ảnh hiện tại</div><img src='" + safeUrl + "' alt='' />";
            }
        }
    }

    protected void but_submit_Click(object sender, EventArgs e)
    {
        string name = (txt_name.Text ?? "").Trim();
        string description = (txt_description.Text ?? "").Trim();
        string content = (txt_noidung.Text ?? "").Trim();
        string image = (txt_link_fileupload.Text ?? "").Trim();
        string tinh = (hf_tinh.Value ?? "").Trim();
        string quan = (hf_quan.Value ?? "").Trim();
        string phuong = (hf_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();
        string diaDiem = AddressFormat_cl.BuildFullAddress(chiTiet, phuong, quan, tinh);
        if (string.IsNullOrWhiteSpace(diaDiem))
            diaDiem = (hf_address_raw.Value ?? "").Trim();
        string diaChiTinh = NormalizeProvinceName(tinh);
        string loai = (ddl_loai.SelectedValue ?? GianHangProduct_cl.LoaiSanPham).Trim().ToLowerInvariant();
        string idDanhMuc = (ddl_danhmuc.SelectedValue ?? "").Trim();

        decimal giaBan;
        if (!decimal.TryParse((txt_giaban.Text ?? "0").Replace(".", "").Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out giaBan))
            giaBan = 0m;
        int phanTramUuDai;
        if (!int.TryParse((txt_phantram_uu_dai.Text ?? "0").Trim(), out phanTramUuDai))
            phanTramUuDai = 0;
        phanTramUuDai = GianHangProduct_cl.ClampDiscountPercent(phanTramUuDai);

        if (string.IsNullOrWhiteSpace(name))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập tên tin.", "Thông báo", true, "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(idDanhMuc))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn danh mục.", "Thông báo", true, "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(image))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng tải ảnh chính.", "Thông báo", true, "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(tinh) || string.IsNullOrWhiteSpace(quan) || string.IsNullOrWhiteSpace(phuong))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn đầy đủ Tỉnh/Thành, Quận/Huyện và Phường/Xã.", "Thông báo", true, "warning");
            return;
        }

        if (chiTiet.Length < 4)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập địa chỉ chi tiết.", "Thông báo", true, "warning");
            return;
        }

        int? editId = CurrentEditId();

        using (dbDataContext db = new dbDataContext())
        {
            GH_SanPham_tb saved = GianHangProduct_cl.Save(
                db,
                editId,
                CurrentAccountKey(),
                name,
                description,
                content,
                image,
                giaBan,
                0,
                0,
                phanTramUuDai,
                loai,
                idDanhMuc,
                diaDiem,
                diaChiTinh,
                quan,
                phuong,
                chiTiet,
                chk_hidden.Checked);

            if (saved == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không thể lưu tin. Vui lòng thử lại.", "Thông báo", true, "warning");
                return;
            }
        }

        Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            editId.HasValue ? "Đã cập nhật tin thành công." : "Đã đăng tin mới thành công.",
            "1200",
            "success");

        Response.Redirect("/gianhang/quan-ly-tin/Default.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }
}
