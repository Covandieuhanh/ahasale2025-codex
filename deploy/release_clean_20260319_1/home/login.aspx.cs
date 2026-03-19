using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool switchHomeRequested = string.Equals((Request.QueryString["switch"] ?? "").Trim(), "home", StringComparison.OrdinalIgnoreCase);
        if (switchHomeRequested)
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
            check_login_cl.del_all_cookie_session_shop();
        }
        // Ensure postback keeps the friendly URL (/dang-nhap) when this page is rewritten.
        if (this.Form != null && Request != null)
        {
            string rawUrl = Request.RawUrl ?? "/home/login.aspx";
            this.Form.Action = rawUrl;
        }

        CaptureReturnUrlFromQuery();

        PlaceHolder phSwitchHomeMode = FindControlRecursive(this, "ph_switch_home_mode") as PlaceHolder;
        if (phSwitchHomeMode != null)
            phSwitchHomeMode.Visible = (!PortalActiveMode_cl.IsHomeActive() && PortalActiveMode_cl.HasHomeCredential());

        if (!IsPostBack)
        {
            #region KIỂM TRA ĐÃ ĐĂNG NHẬP HAY CHƯA (GIỮ LOGIC)
            string _tk = "";
            HttpCookie _ck = Request.Cookies["cookie_userinfo_home_bcorn"];
            if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]))
                _tk = mahoa_cl.giaima_Bcorn(_ck["taikhoan"]);
            else
            {
                if (Session["taikhoan_home"] != null)
                    _tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_home"].ToString());
            }

            if (taikhoan_cl.exist_taikhoan(_tk))
            {
                bool validHomeCredential = false;
                using (dbDataContext db = new dbDataContext())
                {
                    taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                    bool canLoginHome = acc != null && PortalScope_cl.CanLoginHome(acc.taikhoan, acc.phanloai, acc.permission);
                    validHomeCredential = canLoginHome;
                    if (canLoginHome)
                    {
                        PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
                        check_login_cl.del_all_cookie_session_shop();

                        Session["home_modal_msg"] = "Bạn đã đăng nhập. Vui lòng đăng xuất để đăng nhập tài khoản khác.";
                        Session["home_modal_title"] = "Thông báo";
                        Session["home_modal_type"] = "warning";

                        // Luồng home: luôn về trang chủ sau đăng nhập/auto-redirect.
                        Session["url_back_home"] = "";
                        Response.Redirect("/home/default.aspx", false);

                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }

                if (!validHomeCredential)
                {
                    check_login_cl.del_all_cookie_session_home();
                    string scope = "";
                    using (dbDataContext dbScope = new dbDataContext())
                    {
                        taikhoan_tb accScope = dbScope.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                        if (accScope != null)
                            scope = PortalScope_cl.ResolveScope(accScope.taikhoan, accScope.phanloai, accScope.permission);
                    }
                    string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang gian hàng đối tác" : "trang admin";
                    if (scope == PortalScope_cl.ScopeShop)
                    {
                        Session["ThongBao_Shop"] = "modal|Thông báo|Tài khoản này chỉ được phép đăng nhập ở trang gian hàng đối tác.|warning|0";
                        Response.Redirect("/shop/login.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    Session["thongbao_home"] = "modal|Thông báo|Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".|warning|2500";
                }
            }
            #endregion

            // Focus cho đẹp
            txt_user.Focus();
        }
    }

    protected void but_login_Click(object sender, EventArgs e)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                string _phone = AccountAuth_cl.NormalizePhone(txt_user.Text);
                string _pass = txt_pass.Text ?? "";
                string rawUser = (txt_user.Text ?? "").Trim();

                if (string.IsNullOrEmpty(_phone))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập số điện thoại.", "Thông báo", true, "warning");
                    return;
                }

                if (rawUser.Contains("@"))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Trang home chỉ cho phép đăng nhập bằng số điện thoại.", "Thông báo", true, "warning");
                    return;
                }

                if (!AccountAuth_cl.IsValidPhone(_phone))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại không hợp lệ.", "Thông báo", true, "warning");
                    return;
                }

                if (string.IsNullOrEmpty(_pass))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập mật khẩu.", "Thông báo", true, "warning");
                    return;
                }

                AccountLoginInfo account = AccountAuth_cl.FindHomeAccountByPhone(db, _phone);
                if (account != null && account.IsAmbiguous)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại đang trùng trên nhiều tài khoản home. Vui lòng liên hệ admin để xử lý.", "Thông báo", true, "warning");
                    return;
                }
                if (account == null || string.IsNullOrEmpty(account.TaiKhoan))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Thông tin đăng nhập không tồn tại.", "Thông báo", true, "warning");
                    return;
                }

                if (!AccountAuth_cl.IsPasswordValid(_pass, account.MatKhau))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Mật khẩu không đúng.", "Thông báo", true, "warning");
                    return;
                }

                if (account.Block)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản đã bị khóa.", "Thông báo", true, "warning");
                    return;
                }

                if (account.HanSuDung != null && AhaTime_cl.Now > account.HanSuDung.Value)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản của bạn đã hết hạn sử dụng.", "Thông báo", true, "warning");
                    return;
                }

                taikhoan_tb fullAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
                if (fullAccount == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Thông tin đăng nhập không tồn tại.", "Thông báo", true, "warning");
                    return;
                }

                if (!PortalScope_cl.CanLoginHome(fullAccount.taikhoan, fullAccount.phanloai, fullAccount.permission))
                {
                    string scope = PortalScope_cl.ResolveScope(fullAccount.taikhoan, fullAccount.phanloai, fullAccount.permission);
                    if (scope == PortalScope_cl.ScopeShop)
                    {
                        Session["ThongBao_Shop"] = "modal|Thông báo|Tài khoản này chỉ được phép đăng nhập ở trang gian hàng đối tác.|warning|0";
                        Response.Redirect("/shop/login.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    string targetPortal = "trang admin";
                    Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".", "Thông báo", true, "warning");
                    return;
                }

                if (PortalScope_cl.EnsureScope(fullAccount, PortalScope_cl.ScopeHome))
                {
                    db.SubmitChanges();
                }

                // ✅ Đăng nhập OK (GIỮ LOGIC)
                string _taikhoan_mahoa = mahoa_cl.mahoa_Bcorn(account.TaiKhoan);
                string _matkhau_mahoa = mahoa_cl.mahoa_Bcorn(account.MatKhau);

                HttpCookie _ck = new HttpCookie("cookie_userinfo_home_bcorn");
                _ck["taikhoan"] = _taikhoan_mahoa;
                _ck["matkhau"] = _matkhau_mahoa;
                _ck.Expires = AhaTime_cl.Now.AddDays(7);
                _ck.HttpOnly = true;
                _ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                Response.Cookies.Add(_ck);

                Session["taikhoan_home"] = _taikhoan_mahoa;
                Session["matkhau_home"] = _matkhau_mahoa;
                PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
                check_login_cl.del_all_cookie_session_shop();

                // Luồng home: luôn về trang chủ sau đăng nhập thành công.
                Session["url_back_home"] = "";
                Response.Redirect("/home/default.aspx", false);

                Context.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, "", _ex.StackTrace);
            Helper_Tabler_cl.ShowModal(this.Page, "Có lỗi xảy ra. Vui lòng thử lại.", "Thông báo", true, "warning");
        }
    }

    private void CaptureReturnUrlFromQuery()
    {
        string raw = Request.QueryString["return_url"] ?? Request.QueryString["returnUrl"] ?? "";
        string normalized = NormalizeLocalReturnUrl(raw);
        if (!string.IsNullOrEmpty(normalized))
            Session["url_back_home"] = normalized.ToLowerInvariant();
    }

    private string NormalizeLocalReturnUrl(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        value = HttpUtility.UrlDecode(value) ?? "";
        value = value.Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        Uri absolute;
        if (Uri.TryCreate(value, UriKind.Absolute, out absolute))
        {
            if (Request.Url == null)
                return "";

            if (!string.Equals(absolute.Host, Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                return "";

            value = absolute.PathAndQuery;
        }

        if (!value.StartsWith("/", StringComparison.Ordinal))
            return "";
        if (value.StartsWith("//", StringComparison.Ordinal))
            return "";

        string lower = value.ToLowerInvariant();
        if (lower.StartsWith("/dang-nhap")
            || lower.StartsWith("/home/login.aspx")
            || lower.StartsWith("/shop/dang-nhap")
            || lower.StartsWith("/shop/login.aspx"))
            return "";

        return value;
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
}
