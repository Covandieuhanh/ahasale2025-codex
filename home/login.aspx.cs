using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class home_login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure postback keeps the friendly URL (/dang-nhap) when this page is rewritten.
        if (this.Form != null && Request != null)
        {
            string rawUrl = Request.RawUrl ?? "/home/login.aspx";
            this.Form.Action = rawUrl;
        }

        CaptureReturnUrlFromQuery();

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
                using (dbDataContext db = new dbDataContext())
                {
                    taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                    bool canLoginHome = acc != null && PortalScope_cl.CanLoginHome(acc.taikhoan, acc.phanloai, acc.permission);
                    if (canLoginHome)
                    {
                        string _url_back = Session["url_back_home"] as string;
                        if (!string.IsNullOrEmpty(_url_back))
                            Response.Redirect(_url_back, false);
                        else
                            Response.Redirect("/home/quan-ly-tin/default.aspx", false);

                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }

                check_login_cl.del_all_cookie_session_home();
                string scope = "";
                using (dbDataContext dbScope = new dbDataContext())
                {
                    taikhoan_tb accScope = dbScope.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                    if (accScope != null)
                        scope = PortalScope_cl.ResolveScope(accScope.taikhoan, accScope.phanloai, accScope.permission);
                }
                string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang shop" : "trang admin";
                Session["thongbao_home"] = "modal|Thông báo|Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".|warning|2500";
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

                if (string.IsNullOrEmpty(_phone))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập số điện thoại.", "Thông báo", true, "warning");
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
                    string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang shop" : "trang admin";
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

                string _url_back = Session["url_back_home"]?.ToString();
                if (!string.IsNullOrEmpty(_url_back))
                    Response.Redirect(_url_back, false);
                else
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

    #region khôi phục mật khẩu
    protected void but_show_form_quenmk_Click(object sender, EventArgs e)
    {
        pn_khoiphuc.Visible = true;
        txt_email_khoiphuc.Focus();
    }

    protected void but_close_form_quenmk_Click(object sender, EventArgs e)
    {
        txt_email_khoiphuc.Text = "";
        pn_khoiphuc.Visible = false;
    }

    protected void but_nhanma_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            String_cl str_cl = new String_cl();
            string _email = txt_email_khoiphuc.Text.Trim().ToLower();

            if (str_cl.KiemTra_Email(_email) == false)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email không hợp lệ.", "Thông báo", true, "warning");
                return;
            }

            var scopedAccounts = db.taikhoan_tbs
                .Where(p => (p.email ?? "").Trim().ToLower() == _email)
                .ToList()
                .Where(p => PortalScope_cl.CanLoginHome(p.taikhoan, p.phanloai, p.permission))
                .ToList();

            if (scopedAccounts.Count > 1)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email này đang trùng nhiều tài khoản trong hệ AhaSale. Vui lòng liên hệ quản trị để xử lý.", "Thông báo", true, "warning");
                return;
            }

            taikhoan_tb q = scopedAccounts.FirstOrDefault();
            if (q == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Email này không tồn tại trong hệ thống.", "Thông báo", true, "warning");
                return;
            }

            // THÔNG BÁO QUA EMAIL (GIỮ LOGIC)
            List<string> emailAddresses = new List<string> { _email };

            string _tenmien = HttpContext.Current.Request.Url.Host.ToUpper();
            string _tieude = "Khôi phục mật khẩu";

            string _ma = Guid.NewGuid().ToString().ToLower();
            string _link_khoiphuc = $"{Request.Url.Scheme}://{Request.Url.Authority}{"/home/khoi-phuc-mat-khau.aspx?code=" + _ma}";
            DateTime _hsd = AhaTime_cl.Now.AddMinutes(5);

            string _noidung = "<div style='color:red'>Ai đó đã yêu cầu đặt lại mật khẩu của bạn tại " + _tenmien + "</div>";
            _noidung += "<div style='color:red'>Nếu không phải là bạn, vui lòng bỏ qua email này.<hr/></div>";
            _noidung += "<div>Tài khoản của bạn: <b>" + q.taikhoan + "</b></div>";
            _noidung += "<div><a href='" + _link_khoiphuc + "'><b>Nhấp vào đây</b></a> để đặt lại mật khẩu của bạn.</div>";
            _noidung += "<div>Thời gian hết hạn: " + _hsd.ToString("dd/MM/yyyy HH:mm") + "</div>";

            if (q.hsd_makhoiphuc == null || q.hsd_makhoiphuc.Value < AhaTime_cl.Now)
            {
                q.makhoiphuc = _ma;
                q.hansudung = null;
                q.hsd_makhoiphuc = _hsd;
                db.SubmitChanges();

                foreach (var _email_nhan in emailAddresses)
                    guiEmail_cl.SendEmail(_email_nhan, _tieude, _noidung, _tenmien, "");
            }

            txt_email_khoiphuc.Text = "";
            pn_khoiphuc.Visible = false;

            Helper_Tabler_cl.ShowModal(this.Page,
                "Chúng tôi đã gửi yêu cầu khôi phục vào email của bạn. Vui lòng kiểm tra cả Hộp thư rác và làm theo hướng dẫn.",
                "Thông báo", true, "warning");
        }
    }
    #endregion

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
}
