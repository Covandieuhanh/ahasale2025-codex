using System;
using System.Linq;

public partial class shop_dat_lai_mat_khau : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
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

        string pass = (txt_pass.Text ?? "").Trim();
        string pass2 = (txt_pass_confirm.Text ?? "").Trim();
        if (string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(pass2))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập đủ mật khẩu.", "Thông báo", true, "warning");
            return;
        }
        if (!string.Equals(pass, pass2, StringComparison.Ordinal))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Mật khẩu nhập lại không khớp.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string tk;
            string error;
            if (!ShopOtp_cl.TryConsumeVerifiedOtp(db, requestId, ShopOtp_cl.TypePassword, out tk, out error))
            {
                Helper_Tabler_cl.ShowModal(this.Page, error, "Thông báo", true, "warning");
                return;
            }

            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (q == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tìm thấy tài khoản.", "Thông báo", true, "warning");
                return;
            }

            q.matkhau = pass;
            string clearError;
            AccountResetSecurity_cl.ClearForceShopPassword(db, q.taikhoan, out clearError);
            db.SubmitChanges();
        }

        // Logout shop and redirect to login
        Session["taikhoan_shop"] = "";
        Session["matkhau_shop"] = "";
        if (Request.Cookies["cookie_userinfo_shop_bcorn"] != null)
        {
            DateTime now = AhaTime_cl.Now;
            Response.Cookies["cookie_userinfo_shop_bcorn"].Expires = now.AddDays(-1);
        }

        Session["ThongBao_Shop"] = "toast|Thông báo|Đổi mật khẩu thành công. Vui lòng đăng nhập lại.|warning|1200";
        Response.Redirect("/shop/login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
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
            ShopOtpRequestInfo info;
            string error;
            if (!ShopOtp_cl.TryGetOtpInfo(db, requestId, ShopOtp_cl.TypePassword, out info, out error))
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

            if (info.Status != ShopOtp_cl.StatusVerified)
            {
                ShowError("Bạn cần xác thực OTP trước khi đặt lại mật khẩu.");
                return;
            }

            hd_request_id.Value = requestId.ToString();
            pn_form.Visible = true;
            hl_back.NavigateUrl = "/shop/login.aspx";
        }
    }

    private void ShowError(string message)
    {
        pn_error.Visible = true;
        lit_error.Text = message;
        pn_form.Visible = false;
    }
}
