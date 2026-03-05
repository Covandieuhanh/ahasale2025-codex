using System;
using System.Linq;
using System.Web;

public partial class shop_login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string tk = GetCurrentShopAccount();
            if (!string.IsNullOrEmpty(tk))
            {
                using (dbDataContext db = new dbDataContext())
                {
                    taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                    if (acc != null && PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission))
                    {
                        Response.Redirect("/shop/default.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }

                check_login_cl.del_all_cookie_session_shop();
            }

            string msg = Session["thongbao_shop"] as string;
            if (!string.IsNullOrEmpty(msg))
            {
                lb_msg.Text = "Vui lòng đăng nhập lại.";
                Session["thongbao_shop"] = "";
            }
        }
    }

    private string GetCurrentShopAccount()
    {
        string tk = "";
        HttpCookie ck = Request.Cookies["cookie_userinfo_shop_bcorn"];
        if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
        {
            tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
        }
        else if (Session["taikhoan_shop"] != null)
        {
            tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_shop"].ToString());
        }
        return (tk ?? "").Trim();
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

                Response.Redirect("/shop/default.aspx", false);
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
