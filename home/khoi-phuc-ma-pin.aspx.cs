using System;
using System.Linq;
using System.Web.UI;

public partial class home_khoi_phuc_ma_pin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
            txt_phone.Focus();
        }
    }

    protected void btnSendOtp_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string rawPhone = txt_phone.Text ?? "";
            string phone = AccountAuth_cl.NormalizePhone(rawPhone);

            if (string.IsNullOrEmpty(phone))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập số điện thoại.", "Thông báo", true, "warning");
                return;
            }

            if (!AccountAuth_cl.IsValidPhone(phone))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại không hợp lệ.", "Thông báo", true, "warning");
                return;
            }

            AccountLoginInfo account = AccountAuth_cl.FindHomeAccountByPhone(db, phone);
            if (account != null && account.IsAmbiguous)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại đang trùng nhiều tài khoản home. Vui lòng liên hệ admin.", "Thông báo", true, "warning");
                return;
            }

            if (account == null || string.IsNullOrEmpty(account.TaiKhoan))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại không tồn tại trong hệ thống.", "Thông báo", true, "warning");
                return;
            }

            taikhoan_tb q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
            if (q == null || !PortalScope_cl.CanLoginHome(q.taikhoan, q.phanloai, q.permission))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản này không thuộc hệ home.", "Thông báo", true, "warning");
                return;
            }

            if (q.block == true)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản đã bị khóa.", "Thông báo", true, "warning");
                return;
            }

            int requestId;
            string error;
            bool usedFallback;
            string devOtp;

            bool sent = HomeOtp_cl.TrySendOtp(db, phone, q.taikhoan, HomeOtp_cl.TypePin,
                out requestId, out error, out usedFallback, out devOtp);

            if (!sent)
            {
                Helper_Tabler_cl.ShowModal(this.Page, error, "Thông báo", true, "warning");
                return;
            }

            if (requestId <= 0)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tạo được yêu cầu OTP. Vui lòng thử lại.", "Thông báo", true, "warning");
                return;
            }

            Session["home_otp_dev_code"] = usedFallback ? devOtp : "";
            Session["home_otp_dev_type"] = HomeOtp_cl.TypePin;

            Response.Redirect("/home/xac-nhan-otp.aspx?type=pin&id=" + requestId, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
