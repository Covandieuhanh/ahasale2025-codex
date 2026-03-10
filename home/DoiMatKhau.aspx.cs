using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_DoiMatKhau : System.Web.UI.Page
{
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
            "home_password_changed_redirect_" + Guid.NewGuid().ToString("N"),
            js,
            true);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true); //check tài khoản, có chuyển hướng. YÊU CẦU ĐĂNG NHẬP.

            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();

            if (!string.IsNullOrEmpty(_tk))//nếu có khách đăng nhập
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
            { }

            txt_MatKhauCu.Focus();
        }
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!IsPostBack) return;

        if (!string.IsNullOrEmpty(txt_MatKhauCu.Text))
            txt_MatKhauCu.Attributes["value"] = txt_MatKhauCu.Text;

        if (!string.IsNullOrEmpty(txt_MatKhauMoi.Text))
            txt_MatKhauMoi.Attributes["value"] = txt_MatKhauMoi.Text;

        if (!string.IsNullOrEmpty(txt_NhapLaiMatKhauMoi.Text))
            txt_NhapLaiMatKhauMoi.Attributes["value"] = txt_NhapLaiMatKhauMoi.Text;
    }
	    protected void btnDoiMatKhau_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _pass_old = txt_MatKhauCu.Text.Trim();
            string _pass_1 = txt_MatKhauMoi.Text.Trim();
            string _pass_2 = txt_NhapLaiMatKhauMoi.Text.Trim();

            if (string.IsNullOrEmpty(_pass_old) || string.IsNullOrEmpty(_pass_1) || string.IsNullOrEmpty(_pass_2))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập đầy đủ thông tin.", "Thông báo", true, "warning");
                return;
            }

            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng tải lại trang.", "Thông báo", true, "warning");
                return;
            }

            if (_pass_old != q.matkhau)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Mật khẩu hiện tại không đúng.", "Thông báo", true, "warning");
                return;
            }

            if (_pass_1 != _pass_2)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Mật khẩu mới không trùng nhau.", "Thông báo", true, "warning");
                return;
            }

            q.matkhau = _pass_1;

            bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
            string clearError;
            if (isShopPortal)
                AccountResetSecurity_cl.ClearForceShopPassword(db, q.taikhoan, out clearError);
            else
                AccountResetSecurity_cl.ClearForceHomePassword(db, q.taikhoan, out clearError);

            db.SubmitChanges();

            if (isShopPortal)
            {
                Session["taikhoan_shop"] = "";
                Session["matkhau_shop"] = "";
                if (Request.Cookies["cookie_userinfo_shop_bcorn"] != null)
                {
                    HttpCookie cookie = new HttpCookie("cookie_userinfo_shop_bcorn");
                    cookie.Expires = AhaTime_cl.Now.AddDays(-1);
                    cookie.Path = "/";
                    Response.Cookies.Add(cookie);
                }

                Helper_Tabler_cl.ShowModal(this.Page, "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "Thông báo", false, "success");
                string shopJs = @"
(function() {
    function bindRedirect() {
        var modal = document.getElementById('dynamicModal');
        if (!modal) {
            setTimeout(bindRedirect, 100);
            return;
        }

        function redirectToLogin() {
            window.location.replace('/shop/login.aspx');
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
                    "shop_password_changed_redirect_" + Guid.NewGuid().ToString("N"),
                    shopJs,
                    true);
            }
            else
            {
                ClearHomeLoginState();
                ShowSuccessThenRedirectToLogin("Đổi mật khẩu thành công. Vui lòng đăng nhập lại.");
            }
        }
    }

}
