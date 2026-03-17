using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class shop_login : System.Web.UI.Page
{
    private string _cachedReturnUrl;
    private void DisablePageCaching()
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        Response.Cache.SetValidUntilExpires(false);
    }

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

    private string ResolveReturnUrl()
    {
        if (!string.IsNullOrEmpty(_cachedReturnUrl))
            return _cachedReturnUrl;

        string safe = check_login_cl.NormalizeShopReturnUrl(Request.QueryString["return_url"]);
        if (string.IsNullOrEmpty(safe))
            safe = "/shop/default.aspx";

        _cachedReturnUrl = safe;
        return _cachedReturnUrl;
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

        ShopStatus_cl.EnsureSchemaSafe(db);
        acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk && p.matkhau == mk);
        if (acc == null)
            return false;

        if (!ShopStatus_cl.IsShopApproved(acc))
            return false;

        DateTime now = AhaTime_cl.Now;
        if (acc.hansudung != null && now > acc.hansudung.Value)
            return false;

        return PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DisablePageCaching();

        bool switchShopRequested = string.Equals((Request.QueryString["switch"] ?? "").Trim(), "shop", StringComparison.OrdinalIgnoreCase);
        if (switchShopRequested)
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
            check_login_cl.del_all_cookie_session_home();
        }

        if (ph_switch_shop_mode != null)
            ph_switch_shop_mode.Visible = (!PortalActiveMode_cl.IsShopActive() && PortalActiveMode_cl.HasShopCredential());

        if (!IsPostBack)
        {
            string tk, mk;
            if (TryReadCurrentShopCredential(out tk, out mk))
            {
                try
                {
                    using (dbDataContext db = new dbDataContext())
                    {
                        taikhoan_tb acc;
                        bool validShopCredential = IsValidActiveShopCredential(db, tk, mk, out acc);
                        if (validShopCredential && PortalActiveMode_cl.IsShopActive())
                        {
                            check_login_cl.del_all_cookie_session_home();
                            if (AccountResetSecurity_cl.ShouldForceShopPassword(db, tk))
                                Response.Redirect("/shop/doi-mat-khau?force=1", false);
                            else
                                Response.Redirect(ResolveReturnUrl(), false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }

                        if (!validShopCredential)
                            check_login_cl.del_all_cookie_session_shop();
                    }
                }
                catch (Exception ex)
                {
                    if (!SqlTransientGuard_cl.IsTransient(ex))
                        throw;

                    check_login_cl.del_all_cookie_session_shop();
                    lb_msg.Text = "Kết nối dữ liệu đang tạm thời gián đoạn. Vui lòng đăng nhập lại sau vài giây.";
                }
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
                    lb_msg.Text = "Trang gian hàng đối tác chỉ cho phép đăng nhập bằng email.";
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
                    lb_msg.Text = "Email đang trùng nhiều tài khoản gian hàng đối tác. Vui lòng liên hệ admin.";
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

                ShopStatus_cl.EnsureSchemaSafe(db);
                if (!ShopStatus_cl.IsShopApproved(full))
                {
                    lb_msg.Text = (full.TrangThai_Shop == ShopStatus_cl.StatusPending)
                        ? "Tài khoản gian hàng đối tác chưa được duyệt."
                        : "Tài khoản gian hàng đối tác đang bị khóa.";
                    return;
                }

                if (!PortalScope_cl.CanLoginShop(full.taikhoan, full.phanloai, full.permission))
                {
                    lb_msg.Text = "Tài khoản này không thuộc hệ gian hàng đối tác.";
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
                DateTime now = AhaTime_cl.Now;
                ck.Expires = now.AddDays(7);
                ck.HttpOnly = true;
                ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                Response.Cookies.Add(ck);

                Session["taikhoan_shop"] = tkMaHoa;
                Session["matkhau_shop"] = mkMaHoa;
                check_login_cl.del_all_cookie_session_home();

                if (forceChangePasswordAfterLogin)
                {
                    Session["ThongBao_Shop"] = "toast|Thông báo|Mật khẩu này là mật khẩu tạm thời. Vui lòng đổi lại ngay.|warning|1800";
                    Response.Redirect("/shop/doi-mat-khau?force=1", false);
                }
                else
                {
                    Response.Redirect(ResolveReturnUrl(), false);
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

    #region khôi phục mật khẩu gian hàng đối tác (email)
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
                Helper_Tabler_cl.ShowModal(this.Page, "Email đang trùng nhiều tài khoản gian hàng đối tác. Vui lòng liên hệ admin.", "Thông báo", true, "warning");
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
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản này không thuộc hệ gian hàng đối tác.", "Thông báo", true, "warning");
                return;
            }

            ShopStatus_cl.EnsureSchemaSafe(db);
            if (!ShopStatus_cl.IsShopApproved(q))
            {
                string msg = (q.TrangThai_Shop == ShopStatus_cl.StatusPending)
                    ? "Tài khoản gian hàng đối tác chưa được duyệt."
                    : "Tài khoản gian hàng đối tác đang bị khóa.";
                Helper_Tabler_cl.ShowModal(this.Page, msg, "Thông báo", true, "warning");
                return;
            }

            int requestId;
            string error;
            bool usedFallback;
            string devOtp;

            bool sent = ShopOtp_cl.TrySendEmailOtp(db, email, q.taikhoan, ShopOtp_cl.TypePassword,
                out requestId, out error, out usedFallback, out devOtp);

            if (requestId <= 0)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tạo được OTP. Vui lòng thử lại.", "Thông báo", true, "warning");
                return;
            }

            if (!sent)
            {
                Session["shop_otp_send_warning"] = string.IsNullOrEmpty(error)
                    ? "Hệ thống chưa gửi được OTP. Vui lòng nhận OTP từ admin."
                    : error;
            }
            else
            {
                Session["shop_otp_send_warning"] = null;
            }

            Session["shop_otp_dev_code"] = usedFallback ? devOtp : "";
            Session["shop_otp_dev_type"] = ShopOtp_cl.TypePassword;

            if (txtEmail != null)
                txtEmail.Text = "";
            SetPanelVisible(this, "pn_khoiphuc", false);

            Response.Redirect("/shop/xac-nhan-otp.aspx?type=password&id=" + requestId, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
    #endregion
}
