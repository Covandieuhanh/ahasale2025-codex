using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class home_dat_lai_ma_pin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
            BindRequest();
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        int requestId;
        if (!int.TryParse(hd_request_id.Value, out requestId) || requestId <= 0)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Yêu cầu không hợp lệ.", "Thông báo", true, "warning");
            return;
        }

        string newPin = (txt_new_pin.Text ?? "").Trim();
        string newPinRepeat = (txt_new_pin_repeat.Text ?? "").Trim();

        if (string.IsNullOrEmpty(newPin))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập mã PIN mới.", "Thông báo", true, "warning");
            return;
        }

        if (!string.Equals(newPin, newPinRepeat, StringComparison.Ordinal))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Mã PIN mới không trùng nhau.", "Thông báo", true, "warning");
            return;
        }

        if (!PinSecurity_cl.IsValidPinFormat(newPin))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Mã PIN mới phải gồm đúng 4 chữ số.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            HomeOtpRequestInfo info;
            string error;
            if (!HomeOtp_cl.TryGetOtpInfo(db, requestId, HomeOtp_cl.TypePin, out info, out error))
            {
                Helper_Tabler_cl.ShowModal(this.Page, error, "Thông báo", true, "warning");
                return;
            }

            if (info.ExpiresAt < AhaTime_cl.Now)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Mã OTP đã hết hạn. Vui lòng gửi lại.", "Thông báo", true, "warning");
                return;
            }

            if (info.Status != HomeOtp_cl.StatusVerified)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Mã OTP chưa được xác thực.", "Thông báo", true, "warning");
                return;
            }

            taikhoan_tb q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == info.TaiKhoan);
            if (q == null || !PortalScope_cl.CanLoginHome(q.taikhoan, q.phanloai, q.permission))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản không hợp lệ.", "Thông báo", true, "warning");
                return;
            }

            if (q.block == true)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản đã bị khóa.", "Thông báo", true, "warning");
                return;
            }

            q.mapin_thanhtoan = PinSecurity_cl.HashPin(newPin);
            string clearError;
            AccountResetSecurity_cl.ClearForceHomePin(db, q.taikhoan, out clearError);
            db.SubmitChanges();

            string consumeError;
            string consumedTk;
            HomeOtp_cl.TryConsumeVerifiedOtp(db, requestId, HomeOtp_cl.TypePin, out consumedTk, out consumeError);
        }

        ClearHomeLoginState();
        ShowSuccessThenRedirectToLogin("Đặt lại mã PIN thành công. Vui lòng đăng nhập lại.");
    }

    private void BindRequest()
    {
        int requestId;
        if (!int.TryParse(Request.QueryString["rid"], out requestId) || requestId <= 0)
        {
            ShowError("Yêu cầu không hợp lệ.");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            HomeOtpRequestInfo info;
            string error;
            if (!HomeOtp_cl.TryGetOtpInfo(db, requestId, HomeOtp_cl.TypePin, out info, out error))
            {
                ShowError(error);
                return;
            }

            if (info.ExpiresAt < AhaTime_cl.Now)
            {
                ShowError("Mã OTP đã hết hạn. Vui lòng gửi lại.");
                return;
            }

            if (info.Status != HomeOtp_cl.StatusVerified)
            {
                ShowError("Mã OTP chưa được xác thực.");
                return;
            }

            hd_request_id.Value = requestId.ToString();
            lit_phone.Text = HomeOtp_cl.MaskPhone(info.Phone);
            pn_form.Visible = true;
        }
    }

    private void ShowError(string message)
    {
        pn_error.Visible = true;
        lit_error.Text = message;
        pn_form.Visible = false;
    }

    private void ClearHomeLoginState()
    {
        Session["taikhoan_home"] = "";
        Session["matkhau_home"] = "";
        if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
        {
            HttpCookie cookie = new HttpCookie("cookie_userinfo_home_bcorn");
            cookie.Expires = AhaTime_cl.Now.AddDays(-1);
            cookie.Path = "/";
            Response.Cookies.Add(cookie);
        }
    }

    private void ShowSuccessThenRedirectToLogin(string message)
    {
        Helper_Tabler_cl.ShowModal(this.Page, message, "Thông báo", false, "success");

        string js = @"
(function() {
    function bindRedirect() {
        var modal = document.getElementById('dynamicModal');
        if (!modal) {
            setTimeout(bindRedirect, 100);
            return;
        }

        function redirectToLogin() {
            window.location.replace('/dang-nhap');
        }

        var okButton = modal.querySelector('.btn-ok');
        var closeButton = modal.querySelector('.btn-close');

        if (okButton) okButton.addEventListener('click', redirectToLogin, { once: true });
        if (closeButton) closeButton.addEventListener('click', redirectToLogin, { once: true });
    }

    bindRedirect();
})();";

        ScriptManager.RegisterStartupScript(
            this.Page,
            this.GetType(),
            "home_reset_pin_redirect_" + Guid.NewGuid().ToString("N"),
            js,
            true);
    }
}
