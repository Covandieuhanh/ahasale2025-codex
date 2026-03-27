using System;
using System.Linq;
using System.Web;

public partial class shop_login : System.Web.UI.Page
{
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
}
