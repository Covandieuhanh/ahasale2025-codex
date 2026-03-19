using System;
using System.Web.UI;

public partial class shop_xac_nhan_otp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
            BindRequest();
        }
    }

    protected void btnVerify_Click(object sender, EventArgs e)
    {
        int requestId;
        if (!int.TryParse(hd_request_id.Value, out requestId) || requestId <= 0)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Yêu cầu không hợp lệ.", "Thông báo", true, "warning");
            return;
        }

        string otpType = (hd_request_type.Value ?? "").Trim().ToLowerInvariant();
        string otp = (txt_otp.Text ?? "").Trim();

        using (dbDataContext db = new dbDataContext())
        {
            string error;
            if (!ShopOtp_cl.VerifyOtp(db, requestId, otp, otpType, out error))
            {
                Helper_Tabler_cl.ShowModal(this.Page, error, "Thông báo", true, "warning");
                return;
            }
        }

        RedirectToNextStep(otpType, requestId);
    }

    private void BindRequest()
    {
        string rawType = (Request.QueryString["type"] ?? "").Trim().ToLowerInvariant();
        string otpType = NormalizeOtpType(rawType);
        int requestId;

        if (string.IsNullOrEmpty(otpType) || !int.TryParse(Request.QueryString["id"], out requestId) || requestId <= 0)
        {
            ShowError("Yêu cầu không hợp lệ.");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            ShopOtpRequestInfo info;
            string error;
            if (!ShopOtp_cl.TryGetOtpInfo(db, requestId, otpType, out info, out error))
            {
                ShowError(error);
                return;
            }

            DateTime now = AhaTime_cl.Now;
            if (info.ExpiresAt < now)
            {
                ShowError("Mã OTP đã hết hạn. Vui lòng gửi lại.");
                return;
            }

            if (info.Status == ShopOtp_cl.StatusSendFailed)
            {
                string warn = Session["shop_otp_send_warning"] as string;
                if (string.IsNullOrEmpty(warn))
                    warn = "Hệ thống chưa gửi được OTP. Vui lòng nhận OTP từ admin.";
                pn_send_warning.Visible = true;
                lit_send_warning.Text = warn;
            }

            if (info.Status == ShopOtp_cl.StatusLocked)
            {
                ShowError("Mã OTP đã bị khóa do nhập sai nhiều lần. Vui lòng gửi lại.");
                return;
            }

            if (info.Status == ShopOtp_cl.StatusConsumed)
            {
                ShowError("Mã OTP đã được sử dụng. Vui lòng gửi lại.");
                return;
            }

            hd_request_id.Value = requestId.ToString();
            hd_request_type.Value = otpType;
            lit_email.Text = ShopOtp_cl.MaskPhone(info.Phone);
            pn_form.Visible = true;

            hl_resend.NavigateUrl = "/shop/login.aspx";

            string devOtp = Session["shop_otp_dev_code"] as string;
            string devType = Session["shop_otp_dev_type"] as string;
            if (!string.IsNullOrEmpty(devOtp) && string.Equals(devType, otpType, StringComparison.OrdinalIgnoreCase))
            {
                pn_dev_otp.Visible = true;
                lit_dev_otp.Text = devOtp;
            }
            Session["shop_otp_dev_code"] = null;
            Session["shop_otp_dev_type"] = null;
            Session["shop_otp_send_warning"] = null;
        }
    }

    private void ShowError(string message)
    {
        pn_error.Visible = true;
        lit_error.Text = message;
        pn_form.Visible = false;
    }

    private static string NormalizeOtpType(string raw)
    {
        if (string.Equals(raw, "password", StringComparison.OrdinalIgnoreCase))
            return ShopOtp_cl.TypePassword;
        return "";
    }

    private void RedirectToNextStep(string otpType, int requestId)
    {
        string url = "/shop/dat-lai-mat-khau.aspx?rid=" + requestId;
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }
}
