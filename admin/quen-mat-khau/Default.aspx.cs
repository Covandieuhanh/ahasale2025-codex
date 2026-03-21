using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class admin_quen_mat_khau_Default : System.Web.UI.Page
{
    private void ShowMessage(string text, string cssModifier)
    {
        pnMessage.Visible = !string.IsNullOrWhiteSpace(text);
        pnMessage.CssClass = "admin-forgot-message" + (string.IsNullOrWhiteSpace(cssModifier) ? "" : " " + cssModifier.Trim());
        litMessage.Text = Server.HtmlEncode(text ?? "");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            hlBack.NavigateUrl = "/admin/login.aspx";
            if (Session["thongbao"] != null)
            {
                ViewState["thongbao"] = Session["thongbao"].ToString();
                Session["thongbao"] = null;
            }
        }
    }

    protected void butSend_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            String_cl str_cl = new String_cl();
            string email = (txtEmail.Text ?? "").Trim().ToLowerInvariant();
            if (!str_cl.KiemTra_Email(email))
            {
                ShowMessage("Email không hợp lệ.", "error");
                return;
            }

            var scopedAccounts = db.taikhoan_tbs
                .Where(p => (p.email ?? "").Trim().ToLower() == email)
                .ToList()
                .Where(p => PortalScope_cl.CanLoginAdmin(p.taikhoan, p.phanloai, p.permission))
                .ToList();

            if (scopedAccounts.Count > 1)
            {
                ShowMessage("Email này đang trùng nhiều tài khoản trong hệ admin. Vui lòng liên hệ quản trị để xử lý.", "error");
                return;
            }

            taikhoan_tb account = scopedAccounts.FirstOrDefault();
            if (account == null)
            {
                ShowMessage("Không tìm thấy tài khoản admin nào ứng với email này.", "error");
                return;
            }

            string tenMien = HttpContext.Current.Request.Url.Host.ToUpperInvariant();
            string tieuDe = "Khôi phục mật khẩu";
            string ma = Guid.NewGuid().ToString().ToLowerInvariant();
            string linkKhoiPhuc = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "/admin/khoi-phuc-mat-khau.aspx?code=" + ma);
            DateTime hsd = AhaTime_cl.Now.AddMinutes(5);

            string noiDung = "<div style='color:red'>Ai đó đã yêu cầu đặt lại mật khẩu của bạn tại " + tenMien + "</div>";
            noiDung += "<div style='color:red'>Nếu không phải là bạn, vui lòng bỏ qua email này và không phải làm gì cả. Tài khoản của bạn vẫn được an toàn.<hr/></div>";
            noiDung += "<div>Tài khoản của bạn: <b>" + account.taikhoan + "</b></div>";
            noiDung += "<div><a href='" + linkKhoiPhuc + "'><b>Nhấp vào đây</b></a> để đặt lại mật khẩu của bạn.</div>";
            noiDung += "<div>Thời gian hết hạn: " + hsd.ToString("dd/MM/yyyy HH:mm") + "'</div>";

            if (account.hsd_makhoiphuc == null || account.hsd_makhoiphuc.Value < AhaTime_cl.Now)
            {
                account.makhoiphuc = ma;
                account.hansudung = null;
                account.hsd_makhoiphuc = hsd;
                db.SubmitChanges();

                guiEmail_cl.SendEmail(email, tieuDe, noiDung, tenMien, "");
            }

            txtEmail.Text = "";
            Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã gửi yêu cầu khôi phục vào email của bạn. Vui lòng kiểm tra hộp thư đến hoặc thư rác.", "2500", "warning");
            Response.Redirect("/admin/login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
