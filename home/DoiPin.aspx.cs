using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class home_DoiPin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string _tk = Session["taikhoan_home"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }

            txt_PinCu.Focus();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!IsPostBack) return;

        // giữ value sau postback (UpdatePanel)
        if (!string.IsNullOrEmpty(txt_PinCu.Text))
            txt_PinCu.Attributes["value"] = txt_PinCu.Text;

        if (!string.IsNullOrEmpty(txt_PinMoi.Text))
            txt_PinMoi.Attributes["value"] = txt_PinMoi.Text;

        if (!string.IsNullOrEmpty(txt_NhapLaiPinMoi.Text))
            txt_NhapLaiPinMoi.Attributes["value"] = txt_NhapLaiPinMoi.Text;
    }

    protected void btnDoiPin_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string pinCu = txt_PinCu.Text.Trim();
            string pinMoi1 = txt_PinMoi.Text.Trim();
            string pinMoi2 = txt_NhapLaiPinMoi.Text.Trim();

            if (string.IsNullOrEmpty(pinCu) || string.IsNullOrEmpty(pinMoi1) || string.IsNullOrEmpty(pinMoi2))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập đầy đủ thông tin.", "Thông báo", true, "warning");
                return;
            }

            if (ViewState["taikhoan"] == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng tải lại trang.", "Thông báo", true, "warning");
                return;
            }

            string taiKhoan = ViewState["taikhoan"].ToString();

            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
            if (q == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng tải lại trang.", "Thông báo", true, "warning");
                return;
            }

            if (pinMoi1 != pinMoi2)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Mã PIN mới không trùng nhau.", "Thông báo", true, "warning");
                return;
            }

            if (!PinSecurity_cl.IsValidPinFormat(pinMoi1))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Mã PIN mới phải gồm đúng 4 chữ số.", "Thông báo", true, "warning");
                return;
            }

            if (!PinSecurity_cl.VerifyAndUpgrade(q, pinCu))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Mã PIN hiện tại không đúng.", "Thông báo", true, "warning");
                return;
            }

            q.mapin_thanhtoan = PinSecurity_cl.HashPin(pinMoi1);
            string clearError;
            AccountResetSecurity_cl.ClearForceHomePin(db, q.taikhoan, out clearError);
            db.SubmitChanges();

            txt_PinCu.Text = "";
            txt_PinMoi.Text = "";
            txt_NhapLaiPinMoi.Text = "";

            Helper_Tabler_cl.ShowModal(this.Page, "Đổi mã PIN thành công.", "Thông báo", true, "success");
        }
    }

}
