using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class shop_login : System.Web.UI.Page
{
    private static Control FindControlRecursive(Control root, string id)
    {
        if (root == null || string.IsNullOrEmpty(id))
            return null;

        if (string.Equals(root.ID, id, StringComparison.Ordinal))
            return root;

        foreach (Control child in root.Controls)
        {
            Control found = FindControlRecursive(child, id);
            if (found != null)
                return found;
        }

        return null;
    }

    private static void SetPanelVisible(Control root, string id, bool visible)
    {
        Panel pn = FindControlRecursive(root, id) as Panel;
        if (pn != null)
            pn.Visible = visible;
    }

    private static TextBox FindTextBox(Control root, string id)
    {
        return FindControlRecursive(root, id) as TextBox;
    }

    private bool TryReadCurrentShopCredential(out string tk, out string mk)
    {
        tk = "";
        mk = "";

        HttpCookie ck = Request.Cookies["cookie_userinfo_shop_bcorn"];
        if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]) && !string.IsNullOrEmpty(ck["matkhau"]))
        {
            tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
            mk = mahoa_cl.giaima_Bcorn(ck["matkhau"]);
            return !string.IsNullOrWhiteSpace(tk);
        }

        if (Session["taikhoan_shop"] != null && Session["matkhau_shop"] != null)
        {
            tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_shop"].ToString());
            mk = mahoa_cl.giaima_Bcorn(Session["matkhau_shop"].ToString());
            return !string.IsNullOrWhiteSpace(tk);
        }

        return false;
    }

    private bool IsValidActiveShopCredential(dbDataContext db, string tk, string mk, out taikhoan_tb acc)
    {
        acc = null;
        if (string.IsNullOrWhiteSpace(tk) || string.IsNullOrEmpty(mk))
            return false;

        acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc == null)
            return false;

        if (mk != (acc.matkhau ?? ""))
            return false;

        if (acc.block == true)
            return false;

        if (acc.hansudung != null && AhaTime_cl.Now > acc.hansudung.Value)
            return false;

        return PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        bool switchShopRequested = string.Equals((Request.QueryString["switch"] ?? "").Trim(), "shop", StringComparison.OrdinalIgnoreCase);
        if (switchShopRequested)
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);

        if (ph_switch_shop_mode != null)
            ph_switch_shop_mode.Visible = (!PortalActiveMode_cl.IsShopActive() && PortalActiveMode_cl.HasShopCredential());

        if (!IsPostBack)
        {
            string tk, mk;
            if (TryReadCurrentShopCredential(out tk, out mk))
            {
                using (dbDataContext db = new dbDataContext())
                {
                    taikhoan_tb acc;
                    bool validShopCredential = IsValidActiveShopCredential(db, tk, mk, out acc);
                    if (validShopCredential && PortalActiveMode_cl.IsShopActive())
                    {
                        if (AccountResetSecurity_cl.ShouldForceShopPassword(db, tk))
                            Response.Redirect("/shop/doi-mat-khau?force=1", false);
                        else
                            Response.Redirect("/shop/default.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    if (!validShopCredential)
                        check_login_cl.del_all_cookie_session_shop();
                }
            }

            string msg = Session["thongbao_shop"] as string;
            if (!string.IsNullOrEmpty(msg))
            {
                lb_msg.Text = "Vui lòng đăng nhập lại.";
                Session["thongbao_shop"] = "";
            }
        }
    }

    protected void but_login_Click(object sender, EventArgs e)
    {
        lb_msg.Text = "";

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                string loginEmail = AccountAuth_cl.NormalizeLoginId(txt_user.Text);
                string pass = (txt_pass.Text ?? "").Trim();

                if (loginEmail == "" || pass == "")
                {
                    lb_msg.Text = "Vui lòng nhập đủ email và mật khẩu.";
                    return;
                }

                if (loginEmail.IndexOf("@", StringComparison.Ordinal) < 0)
                {
                    lb_msg.Text = "Trang shop chỉ cho phép đăng nhập bằng email.";
                    return;
                }

                if (!AccountAuth_cl.IsValidEmail(loginEmail))
                {
                    lb_msg.Text = "Email đăng nhập không hợp lệ.";
                    return;
                }

                AccountLoginInfo account = AccountAuth_cl.FindShopAccountByEmail(db, loginEmail);
                if (account != null && account.IsAmbiguous)
                {
                    lb_msg.Text = "Email đang trùng nhiều tài khoản shop. Vui lòng liên hệ admin.";
                    return;
                }

                if (account == null || string.IsNullOrEmpty(account.TaiKhoan))
                {
                    lb_msg.Text = "Thông tin đăng nhập không tồn tại.";
                    return;
                }

                if (!AccountAuth_cl.IsPasswordValid(pass, account.MatKhau))
                {
                    lb_msg.Text = "Mật khẩu không đúng.";
                    return;
                }

                taikhoan_tb full = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
                if (full == null)
                {
                    lb_msg.Text = "Tài khoản không tồn tại.";
                    return;
                }

                if (full.block == true)
                {
                    lb_msg.Text = "Tài khoản shop chưa được duyệt hoặc đang bị khóa.";
                    return;
                }

                if (!PortalScope_cl.CanLoginShop(full.taikhoan, full.phanloai, full.permission))
                {
                    lb_msg.Text = "Tài khoản này không thuộc hệ shop.";
                    return;
                }

                if (PortalScope_cl.EnsureScope(full, PortalScope_cl.ScopeShop))
                    db.SubmitChanges();

                PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
                bool forceChangePasswordAfterLogin = AccountResetSecurity_cl.ShouldForceShopPassword(db, full.taikhoan);

                string tkMaHoa = mahoa_cl.mahoa_Bcorn(account.TaiKhoan);
                string mkMaHoa = mahoa_cl.mahoa_Bcorn(account.MatKhau);

                HttpCookie ck = new HttpCookie("cookie_userinfo_shop_bcorn");
                ck["taikhoan"] = tkMaHoa;
                ck["matkhau"] = mkMaHoa;
                ck.Expires = AhaTime_cl.Now.AddDays(7);
                ck.HttpOnly = true;
                ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                Response.Cookies.Add(ck);

                Session["taikhoan_shop"] = tkMaHoa;
                Session["matkhau_shop"] = mkMaHoa;

                if (forceChangePasswordAfterLogin)
                {
                    Session["thongbao_shop"] = thongbao_class.metro_notifi_onload("Thông báo", "Mật khẩu này là mật khẩu tạm thời. Vui lòng đổi lại ngay.", "1800", "warning");
                    Response.Redirect("/shop/doi-mat-khau?force=1", false);
                }
                else
                {
                    Response.Redirect("/shop/default.aspx", false);
                }
                Context.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "", ex.StackTrace);
            lb_msg.Text = "Có lỗi xảy ra, vui lòng thử lại.";
        }
    }

    #region khôi phục mật khẩu shop (email)
    protected void but_show_form_quenmk_Click(object sender, EventArgs e)
    {
        SetPanelVisible(this, "pn_khoiphuc", true);
        TextBox txtEmail = FindTextBox(this, "txt_email_khoiphuc");
        if (txtEmail != null)
            txtEmail.Focus();
    }

    protected void but_close_form_quenmk_Click(object sender, EventArgs e)
    {
        TextBox txtEmail = FindTextBox(this, "txt_email_khoiphuc");
        if (txtEmail != null)
            txtEmail.Text = "";
        SetPanelVisible(this, "pn_khoiphuc", false);
    }

    protected void but_nhanma_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            TextBox txtEmail = FindTextBox(this, "txt_email_khoiphuc");
            string emailRaw = txtEmail != null ? txtEmail.Text : "";
            string email = AccountAuth_cl.NormalizeLoginId(emailRaw);

            if (string.IsNullOrWhiteSpace(email) || !AccountAuth_cl.IsValidEmail(email))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email không hợp lệ.", "Thông báo", true, "warning");
                return;
            }

            AccountLoginInfo account = AccountAuth_cl.FindShopAccountByEmail(db, email);
            if (account != null && account.IsAmbiguous)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email đang trùng nhiều tài khoản shop. Vui lòng liên hệ admin.", "Thông báo", true, "warning");
                return;
            }

            if (account == null || string.IsNullOrEmpty(account.TaiKhoan))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email này không tồn tại trong hệ thống.", "Thông báo", true, "warning");
                return;
            }

            taikhoan_tb q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
            if (q == null || !PortalScope_cl.CanLoginShop(q.taikhoan, q.phanloai, q.permission))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản này không thuộc hệ shop.", "Thông báo", true, "warning");
                return;
            }

            if (q.block == true)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản shop chưa được duyệt hoặc đang bị khóa.", "Thông báo", true, "warning");
                return;
            }

            DateTime now = AhaTime_cl.Now;
            bool needNewToken = (q.hsd_makhoiphuc == null)
                                || (q.hsd_makhoiphuc.Value < now)
                                || string.IsNullOrEmpty(q.makhoiphuc);
            if (needNewToken)
            {
                q.makhoiphuc = Guid.NewGuid().ToString().ToLower();
                q.hsd_makhoiphuc = now.AddMinutes(5);
                db.SubmitChanges();
            }

            string token = (q.makhoiphuc ?? "").ToLower();
            DateTime expiry = q.hsd_makhoiphuc ?? now.AddMinutes(5);
            string resetLink = string.Format("{0}://{1}{2}",
                Request.Url.Scheme, Request.Url.Authority, "/shop/khoi-phuc-mat-khau.aspx?code=" + token);

            string domainName = HttpContext.Current.Request.Url.Host.ToUpper();
            string subject = "Khôi phục mật khẩu shop";
            string body = "<div style='color:red'>Ai đó đã yêu cầu đặt lại mật khẩu shop của bạn tại " + domainName + "</div>";
            body += "<div style='color:red'>Nếu không phải là bạn, vui lòng bỏ qua email này.<hr/></div>";
            body += "<div>Tài khoản của bạn: <b>" + q.taikhoan + "</b></div>";
            body += "<div><a href='" + resetLink + "'><b>Nhấp vào đây</b></a> để đặt lại mật khẩu của bạn.</div>";
            body += "<div>Thời gian hết hạn: " + expiry.ToString("dd/MM/yyyy HH:mm") + "</div>";

            guiEmail_cl.SendEmail(email, subject, body, domainName, "");

            if (txtEmail != null)
                txtEmail.Text = "";
            SetPanelVisible(this, "pn_khoiphuc", false);

            Helper_Tabler_cl.ShowModal(this.Page,
                "Chúng tôi đã gửi yêu cầu khôi phục vào email của bạn. Vui lòng kiểm tra cả Hộp thư rác.",
                "Thông báo", true, "warning");
        }
    }
    #endregion
}
