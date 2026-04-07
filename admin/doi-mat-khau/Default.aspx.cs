using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class admin_doi_mat_khau_Default : System.Web.UI.Page
{
    private string CurrentTaiKhoan
    {
        get { return ViewState["current_taikhoan"] as string ?? ""; }
        set { ViewState["current_taikhoan"] = value ?? ""; }
    }

    private string ResolveBackUrl()
    {
        string fallback = "/admin/default.aspx";
        string raw = (Request.QueryString["return_url"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return fallback;

        string decoded = HttpUtility.UrlDecode(raw) ?? "";
        if (string.IsNullOrWhiteSpace(decoded))
            return fallback;

        if (!decoded.StartsWith("/", StringComparison.Ordinal))
            return fallback;

        if (decoded.StartsWith("//", StringComparison.Ordinal))
            return fallback;

        if (!decoded.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return fallback;

        return decoded;
    }

    private void ShowMessage(string text, string cssModifier)
    {
        pnMessage.Visible = !string.IsNullOrWhiteSpace(text);
        pnMessage.CssClass = "admin-password-message" + (string.IsNullOrWhiteSpace(cssModifier) ? "" : " " + cssModifier.Trim());
        litMessage.Text = Server.HtmlEncode(text ?? "");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("change_password", "/admin/login.aspx");

        if (!IsPostBack)
        {
            Session["title"] = "Đổi mật khẩu";

            string tkEnc = Session["taikhoan"] as string;
            if (string.IsNullOrWhiteSpace(tkEnc))
            {
                Response.Redirect("/admin/login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string taiKhoan = mahoa_cl.giaima_Bcorn(tkEnc);
            CurrentTaiKhoan = taiKhoan;
            lbTaiKhoan.Text = taiKhoan;
            hlBack.NavigateUrl = ResolveBackUrl();
        }
    }

    protected void butSave_Click(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("change_password", "/admin/login.aspx");

        string taiKhoan = CurrentTaiKhoan;
        if (string.IsNullOrWhiteSpace(taiKhoan))
        {
            string tkEnc = Session["taikhoan"] as string;
            if (!string.IsNullOrWhiteSpace(tkEnc))
                taiKhoan = mahoa_cl.giaima_Bcorn(tkEnc);
            CurrentTaiKhoan = taiKhoan;
            lbTaiKhoan.Text = taiKhoan;
        }

        string passOld = (txtPasswordOld.Text ?? "").Trim();
        string passNew = (txtPasswordNew.Text ?? "").Trim();
        string passConfirm = (txtPasswordConfirm.Text ?? "").Trim();

        if (string.IsNullOrWhiteSpace(passOld) || string.IsNullOrWhiteSpace(passNew) || string.IsNullOrWhiteSpace(passConfirm))
        {
            ShowMessage("Vui lòng nhập đầy đủ thông tin.", "error");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
            if (account == null)
            {
                ShowMessage("Không tìm thấy tài khoản hiện tại. Vui lòng tải lại trang.", "error");
                return;
            }

            if (!string.Equals(passOld, account.matkhau ?? "", StringComparison.Ordinal))
            {
                ShowMessage("Mật khẩu hiện tại không đúng.", "error");
                return;
            }

            if (!string.Equals(passNew, passConfirm, StringComparison.Ordinal))
            {
                ShowMessage("Mật khẩu mới không trùng nhau.", "error");
                return;
            }

            account.matkhau = passNew;
            db.SubmitChanges();
        }

        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
            Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);

        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "2000", "warning");
        Response.Redirect("/admin/login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }
}
