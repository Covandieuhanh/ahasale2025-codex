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
            db.SubmitChanges();

            txt_PinCu.Text = "";
            txt_PinMoi.Text = "";
            txt_NhapLaiPinMoi.Text = "";

            Helper_Tabler_cl.ShowModal(this.Page, "Đổi mã PIN thành công.", "Thông báo", true, "success");
        }
    }

    #region QUÊN PIN (giống Quên mật khẩu, nhưng gửi PIN mới)
    protected void but_show_form_quenpin_Click(object sender, EventArgs e)
    {
        pn_quenpin.Visible = true;
        txt_email_quenpin.Focus();
    }

    protected void but_close_form_quenpin_Click(object sender, EventArgs e)
    {
        txt_email_quenpin.Text = "";
        pn_quenpin.Visible = false;
        txt_PinCu.Focus();
    }

    protected void but_gui_pin_moi_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            String_cl str_cl = new String_cl();
            string _email = (txt_email_quenpin.Text ?? "").Trim().ToLower();

            if (str_cl.KiemTra_Email(_email) == false)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email không hợp lệ.", "Thông báo", true, "warning");
                return;
            }

            var q = db.taikhoan_tbs.FirstOrDefault(p => p.email != null && p.email.Trim().ToLower() == _email);
            if (q == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email này không tồn tại trong hệ thống.", "Thông báo", true, "warning");
                return;
            }

            // Giới hạn spam giống logic quên mk: mỗi 5 phút 1 lần
            // Tận dụng các field makhoiphuc/hsd_makhoiphuc bạn đang có (nếu bảng có sẵn).
            DateTime now = AhaTime_cl.Now;
            DateTime hsd = now.AddMinutes(5);

            if (q.hsd_makhoiphuc != null && q.hsd_makhoiphuc.Value >= now)
            {
                Helper_Tabler_cl.ShowModal(this.Page,
                    "Bạn vừa yêu cầu trước đó. Vui lòng thử lại sau khi hết hạn (5 phút).",
                    "Thông báo", true, "warning");
                return;
            }

            // Tạo PIN mới (6 chữ số)
            string pinMoi = TaoPinNgauNhien(4);

            // Cập nhật PIN mới vào DB (lưu hash)
            q.mapin_thanhtoan = PinSecurity_cl.HashPin(pinMoi);

            // Set throttle 5 phút (tái sử dụng field)
            q.makhoiphuc = Guid.NewGuid().ToString().ToLower(); // chỉ để ghi nhận yêu cầu
            q.hsd_makhoiphuc = hsd;

            db.SubmitChanges();

            // Gửi email
            string _tenmien = HttpContext.Current.Request.Url.Host.ToUpper();
            string _tieude = "Cấp lại mã PIN";

            string _noidung = "";
            _noidung += "<div style='color:red'>Ai đó đã yêu cầu cấp lại mã PIN của bạn tại " + _tenmien + "</div>";
            _noidung += "<div style='color:red'>Nếu không phải là bạn, vui lòng đăng nhập và đổi PIN ngay.<hr/></div>";
            _noidung += "<div>Tài khoản của bạn: <b>" + q.taikhoan + "</b></div>";
            _noidung += "<div>Mã PIN mới của bạn là: <b style='font-size:18px'>" + pinMoi + "</b></div>";
            _noidung += "<div>Khuyến nghị: Sau khi đăng nhập, hãy vào mục Đổi PIN để đặt lại PIN theo ý bạn.</div>";
            _noidung += "<div>Giới hạn yêu cầu tiếp theo sau: " + hsd.ToString("dd/MM/yyyy HH:mm") + "</div>";

            guiEmail_cl.SendEmail(_email, _tieude, _noidung, _tenmien, "");

            // đóng modal + thông báo
            txt_email_quenpin.Text = "";
            pn_quenpin.Visible = false;

            Helper_Tabler_cl.ShowModal(this.Page,
                "Chúng tôi đã gửi mã PIN mới vào email của bạn. Vui lòng kiểm tra cả Hộp thư rác.",
                "Thông báo", true, "success");
        }
    }

    private string TaoPinNgauNhien(int length)
    {
        return PinSecurity_cl.GenerateRandomNumericPin(length);
    }
    #endregion
}
