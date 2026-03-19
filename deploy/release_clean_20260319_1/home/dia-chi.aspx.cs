using System;
using System.Web;
using System.Web.UI.WebControls;

public partial class home_dia_chi : System.Web.UI.Page
{
    private string CurrentAccount()
    {
        return (ViewState["taikhoan"] ?? "").ToString();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string tkMaHoa = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (string.IsNullOrEmpty(tkMaHoa))
            {
                Response.Redirect("/dang-nhap", true);
                return;
            }

            ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(tkMaHoa);
            BindAddressBook();
        }
    }

    private void BindAddressBook()
    {
        string tk = CurrentAccount();
        if (string.IsNullOrEmpty(tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            var list = AddressHistory_cl.GetSavedAddresses(db, tk, 50, true);
            rpt_address_book.DataSource = list;
            rpt_address_book.DataBind();
            lbl_address_empty.Visible = (list == null || list.Count == 0);
        }
    }

    protected void AddressBook_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e == null) return;

        string command = (e.CommandName ?? "").Trim().ToLowerInvariant();
        if (command != "delete" && command != "edit" && command != "set-default")
            return;

        string tk = CurrentAccount();
        if (string.IsNullOrEmpty(tk))
            return;

        long id;
        if (!long.TryParse((e.CommandArgument ?? "").ToString(), out id))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            if (command == "delete")
            {
                AddressHistory_cl.DeleteAddress(db, tk, id);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã xoá địa chỉ.", null, true, 2000, "Thông báo");
                BindAddressBook();
                return;
            }

            if (command == "set-default")
            {
                AddressHistory_cl.SetDefaultAddress(db, tk, id);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã đặt làm mặc định.", null, true, 2000, "Thông báo");
                BindAddressBook();
                return;
            }

            if (command == "edit")
            {
                var item = AddressHistory_cl.FindById(db, tk, id);
                if (item != null)
                {
                    hf_edit_id.Value = item.Id.ToString();
                    txt_hoten.Text = item.HoTen ?? "";
                    txt_sdt.Text = item.Sdt ?? "";
                    txt_diachi_chitiet.Text = item.DiaChi ?? "";
                    hf_address_raw.Value = item.DiaChi ?? "";
                    chk_default.Checked = item.IsDefault;
                }
            }
        }
    }

    protected void btn_save_Click(object sender, EventArgs e)
    {
        string tk = CurrentAccount();
        if (string.IsNullOrEmpty(tk))
            return;

        string hoTen = (txt_hoten.Text ?? "").Trim();
        string sdt = (txt_sdt.Text ?? "").Trim();
        string tinh = (hf_tinh.Value ?? "").Trim();
        string quan = (hf_quan.Value ?? "").Trim();
        string phuong = (hf_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();

        if (hoTen.Length < 2)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập họ tên người nhận.", "Thông báo", true, "warning");
            return;
        }

        if (!IsValidPhone(sdt))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại không hợp lệ.", "Thông báo", true, "warning");
            return;
        }

        if (string.IsNullOrEmpty(tinh) || string.IsNullOrEmpty(quan) || string.IsNullOrEmpty(phuong))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn đầy đủ Tỉnh/Thành, Quận/Huyện và Phường/Xã.", "Thông báo", true, "warning");
            return;
        }

        if (chiTiet.Length < 4)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập địa chỉ chi tiết.", "Thông báo", true, "warning");
            return;
        }

        string fullAddress = AddressFormat_cl.BuildFullAddress(chiTiet, phuong, quan, tinh);

        long editId;
        long.TryParse((hf_edit_id.Value ?? "").Trim(), out editId);

        using (dbDataContext db = new dbDataContext())
        {
            if (editId > 0)
            {
                AddressHistory_cl.UpdateAddress(db, tk, editId, hoTen, sdt, fullAddress, chk_default.Checked);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã cập nhật địa chỉ.", null, true, 2000, "Thông báo");
            }
            else
            {
                AddressHistory_cl.UpsertAddress(db, tk, hoTen, sdt, fullAddress, chk_default.Checked);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã thêm địa chỉ.", null, true, 2000, "Thông báo");
            }
        }

        ResetForm();
        BindAddressBook();
    }

    protected void btn_reset_Click(object sender, EventArgs e)
    {
        ResetForm();
    }

    private void ResetForm()
    {
        hf_edit_id.Value = "";
        txt_hoten.Text = "";
        txt_sdt.Text = "";
        txt_diachi_chitiet.Text = "";
        hf_tinh.Value = "";
        hf_quan.Value = "";
        hf_phuong.Value = "";
        hf_address_raw.Value = "";
        txt_diachi_full.Text = "";
        chk_default.Checked = false;
    }

    private bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        string digits = System.Text.RegularExpressions.Regex.Replace(phone, "\\D+", "");
        return System.Text.RegularExpressions.Regex.IsMatch(digits, "^0\\d{9,10}$");
    }
}
