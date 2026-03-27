using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_DoiMatKhau : System.Web.UI.Page
{
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
            db.SubmitChanges();

            bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
            if (isShopPortal)
            {
                Session["taikhoan_shop"] = "";
                Session["matkhau_shop"] = "";
                if (Request.Cookies["cookie_userinfo_shop_bcorn"] != null)
                    Response.Cookies["cookie_userinfo_shop_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
            }
            else
            {
                Session["taikhoan_home"] = "";
                Session["matkhau_home"] = "";
                if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
                    Response.Cookies["cookie_userinfo_home_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
            }

            Helper_Tabler_cl.ShowModal(this.Page, "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "Thông báo", true, "warning");
            Response.Redirect(isShopPortal ? "/shop/login.aspx" : "/dang-nhap");
        }
    }

}
