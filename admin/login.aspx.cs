using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default2 : System.Web.UI.Page
{
    private const string ViewRecover = "recover";

    private void DisablePageCaching()
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        Response.Cache.SetValidUntilExpires(false);
    }

    private string BuildLoginUrl()
    {
        return ResolveUrl("~/admin/login.aspx");
    }

    private string BuildForgotPasswordUrl()
    {
        return ResolveUrl("~/admin/quen-mat-khau/default.aspx");
    }

    private string ResolveSafeAdminReturnUrl(string rawUrl, string fallbackUrl)
    {
        return AdminFullPageRoute_cl.SanitizeAdminReturnUrl(rawUrl, fallbackUrl);
    }

    private void RememberRequestedReturnUrl()
    {
        string requested = ResolveSafeAdminReturnUrl(Request.QueryString["return_url"], "");
        if (!string.IsNullOrWhiteSpace(requested))
        {
            Session["url_back"] = requested;
            app_cookie_policy_class.persist_cookie(Context, app_cookie_policy_class.admin_return_url_cookie, requested, 1);
            app_cookie_policy_class.expire_cookie(Context, app_cookie_policy_class.home_return_url_cookie);
            return;
        }

        if (Session["url_back"] != null)
            return;

        string cookieBack = app_cookie_policy_class.read_cookie(Context, app_cookie_policy_class.admin_return_url_cookie);
        cookieBack = ResolveSafeAdminReturnUrl(cookieBack, "");
        if (!string.IsNullOrWhiteSpace(cookieBack))
            Session["url_back"] = cookieBack;
    }

    private void RedirectTo(string url)
    {
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private bool IsLocalAdminHost()
    {
        string host = "";
        try
        {
            host = (Request.Url.Host ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            host = "";
        }

        return host == "ahasale.local" || host == "localhost" || host == "127.0.0.1" || host == "::1";
    }

    private void EnsureLocalAdminAccount(dbDataContext db)
    {
        if (db == null)
            return;

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == "admin");
        bool created = false;
        if (acc == null)
        {
            acc = new taikhoan_tb
            {
                taikhoan = "admin",
                matkhau = "123",
                hoten = "Admin Local",
                ten = "ADMIN",
                hoten_khongdau = "admin",
                phanloai = "Nhân viên admin",
                ngaytao = AhaTime_cl.Now,
                nguoitao = "system",
                block = false,
                permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeAdmin),
                hansudung = AhaTime_cl.Now.AddYears(50),
                DongA = 0m
            };
            db.taikhoan_tbs.InsertOnSubmit(acc);
            created = true;
        }

        bool changed = created;
        if (string.IsNullOrWhiteSpace(acc.matkhau) || acc.matkhau != "123")
        {
            acc.matkhau = "123";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(acc.phanloai) || !string.Equals(acc.phanloai.Trim(), "Nhân viên admin", StringComparison.OrdinalIgnoreCase))
        {
            acc.phanloai = "Nhân viên admin";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(acc.hoten))
        {
            acc.hoten = "Admin Local";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(acc.ten))
        {
            acc.ten = "ADMIN";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(acc.hoten_khongdau))
        {
            acc.hoten_khongdau = "admin";
            changed = true;
        }

        if (!acc.hansudung.HasValue || acc.hansudung.Value < AhaTime_cl.Now.AddDays(1))
        {
            acc.hansudung = AhaTime_cl.Now.AddYears(50);
            changed = true;
        }

        if (acc.block ?? false)
        {
            acc.block = false;
            changed = true;
        }

        if (PortalScope_cl.EnsureScope(acc, PortalScope_cl.ScopeAdmin))
            changed = true;

        if (changed)
            db.SubmitChanges();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DisablePageCaching();
        RememberRequestedReturnUrl();

        string legacyView = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
        if (legacyView == ViewRecover)
        {
            RedirectTo(BuildForgotPasswordUrl());
            return;
        }

        if (!IsPostBack)
        {
            try
            {
                but_show_form_quenmk.NavigateUrl = BuildForgotPasswordUrl();
                #region THÔNG TIN TRANG
                using (dbDataContext db = new dbDataContext())
                {

                    var q = (from tk in db.CaiDatChung_tbs
                             where tk.phanloai_trang == "login"
                             select new { tk.thongtin_icon, tk.thongtin_apple_touch_icon, tk.lienket_chiase_title, tk.lienket_chiase_description, tk.lienket_chiase_image }).FirstOrDefault();

                    if (q != null)
                    {
                        string baseUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);

                        string iconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_icon);
                        string appleTouchIconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_apple_touch_icon);

                        string iconsHtml = string.Format(@"
                <!-- Favicon -->
                <link rel='icon' href='{0}' sizes='16x16' type='image/x-icon'>
                <link rel='icon' href='{1}' sizes='32x32' type='image/x-icon'>
                <link rel='icon' href='{2}' sizes='48x48' type='image/x-icon'>

                <!-- Apple Touch Icon -->
                <link rel='apple-touch-icon' href='{3}' sizes='180x180'>
                <link rel='apple-touch-icon' href='{4}' sizes='167x167'>
                <link rel='apple-touch-icon' href='{5}' sizes='152x152'>
                <link rel='apple-touch-icon' href='{6}' sizes='120x120'>

                <!-- Android Icons -->
                <link rel='icon' href='{7}' sizes='192x192'>
                <link rel='icon' href='{8}' sizes='144x144'>
                ", iconUrl, iconUrl, iconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, iconUrl, iconUrl);

                        string title = q.lienket_chiase_title;
                        string description = q.lienket_chiase_description;
                        string imageRelativePath = q.lienket_chiase_image;

                        // Tạo URL tuyệt đối cho hình ảnh
                        string imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, imageRelativePath);

                        string metaTags = string.Format(@"
                    <!-- Title -->
                    <title>{0}</title>

                    <!-- Meta Description -->
                    <meta name='description' content='{1}' />

                    <!-- Open Graph Meta Tags -->
                    <meta property='og:title' content='{2}' />
                    <meta property='og:description' content='{3}' />
                    <meta property='og:image' content='{4}' />
                    <meta property='og:type' content='website' />
                    <meta property='og:url' content='{5}' />

                    <!-- Twitter Card Meta Tags -->
                    <meta name='twitter:card' content='summary_large_image' />
                    <meta name='twitter:title' content='{6}' />
                    <meta name='twitter:description' content='{7}' />
                    <meta name='twitter:image' content='{8}' />
                ", title, description, title, description, imageUrl, Request.Url.AbsoluteUri, title, description, imageUrl);
                        //literal_fav_icon.Text = iconsHtml + metaTags;
                    }

                }
                #endregion
                #region KIỂM TRA ĐÃ ĐĂNG NHẬP HAY CHƯA
                // Lấy giá trị từ cookie
                string _tk = "";
                HttpCookie _ck = Request.Cookies["cookie_userinfo_admin_bcorn"];
                if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]))// Nếu có cookie, thì lấy giá trị từ cookie và giải mã chúng
                    _tk = mahoa_cl.giaima_Bcorn(_ck["taikhoan"]);
                else
                {
                    // Nếu không có cookie, thì kiểm tra session. Nếu có session, thì lấy giá trị từ session
                    if (Session["taikhoan"] != null)
                        _tk = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
                }
                if (taikhoan_cl.exist_taikhoan(_tk)) // nếu tài khoản tồn tại
                {
                    taikhoan_tb acc;
                    using (dbDataContext dbLogin = new dbDataContext())
                    {
                        acc = dbLogin.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                    }
                    bool canLoginAdmin = acc != null && PortalScope_cl.CanLoginAdmin(acc.taikhoan, acc.phanloai, acc.permission);
                    if (canLoginAdmin)
                    {
                        string _url_back = Session["url_back"] as string; // Sử dụng 'as' để tránh lỗi nếu 'url_back' là null
                        if (!string.IsNullOrEmpty(_url_back)) // Kiểm tra xem '_url_back' có hợp lệ hay không
                        {
                            string landingUrl = "/admin/default.aspx";
                            if (acc != null)
                            {
                                using (dbDataContext dbLanding = new dbDataContext())
                                {
                                    landingUrl = AdminRolePolicy_cl.ResolveLandingUrl(dbLanding, acc.taikhoan);
                                }
                            }
                            Response.Redirect(ResolveSafeAdminReturnUrl(_url_back, landingUrl), false);
                        }
                        else
                        {
                            string landingUrl = "/admin/default.aspx";
                            if (acc != null)
                            {
                                using (dbDataContext dbLanding = new dbDataContext())
                                {
                                    landingUrl = AdminRolePolicy_cl.ResolveLandingUrl(dbLanding, acc.taikhoan);
                                }
                            }
                            Response.Redirect(landingUrl, false);
                        }
                        Context.ApplicationInstance.CompleteRequest(); // Hoàn tất yêu cầu mà không ném 'ThreadAbortException'
                    }
                    else
                    {
                        check_login_cl.del_all_cookie_session_admin();
                        string scope = acc == null ? "" : PortalScope_cl.ResolveScope(acc.taikhoan, acc.phanloai, acc.permission);
                        string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang shop" : "AhaSale";
                        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".", "1800", "warning");
                    }
                }

                //đưa vào trong trang chủ của admin rồi tính kiểm tra tiếp tính hợp lệ của tài khoản
                #endregion
                #region lưu nội dung thông báo nếu có
                if (Session["thongbao"] != null)
                {
                    ViewState["thongbao"] = Session["thongbao"].ToString();
                    Session["thongbao"] = null;
                }
                #endregion
            }
            catch (Exception _ex)
            {
                if (SqlTransientGuard_cl.IsTransient(_ex))
                {
                    check_login_cl.del_all_cookie_session_admin();
                    ViewState["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Kết nối dữ liệu đang tạm thời gián đoạn. Vui lòng đăng nhập lại sau vài giây.", "1800", "warning");
                }

                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                {
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                }
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }


    protected void but_login_Click(object sender, EventArgs e)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                string _loginId = AccountAuth_cl.NormalizeLoginId(txt_user.Text);
                string _pass = txt_pass.Text ?? "";
                if (_loginId == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tài khoản/email/số điện thoại.", "5000", "warning"), true);
                else
                {
                    if (_pass == "")
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập mật khẩu.", "5000", "warning"), true);
                    else
                    {
                        if (IsLocalAdminHost()
                            && string.Equals(_loginId, "admin", StringComparison.OrdinalIgnoreCase)
                            && string.Equals(_pass, "123", StringComparison.Ordinal))
                        {
                            EnsureLocalAdminAccount(db);
                        }

                        AccountLoginInfo account = AccountAuth_cl.FindAccountByLoginId(db, _loginId, PortalScope_cl.ScopeAdmin);
                        if (account != null && account.IsAmbiguous)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Email hoặc số điện thoại đang trùng nhiều tài khoản. Vui lòng dùng tên tài khoản.", "3000", "warning"), true);
                            return;
                        }
                        if (account != null && !string.IsNullOrEmpty(account.TaiKhoan))
                        {
                            if (!AccountAuth_cl.IsPasswordValid(_pass, account.MatKhau))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu không đúng.", "2000", "warning"), true);
                                return;
                            }

                            if (account.Block)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản đã bị khóa.", "2500", "warning"), true);
                                return;
                            }

                            if (account.HanSuDung != null && AhaTime_cl.Now > account.HanSuDung.Value)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản đã hết hạn sử dụng.", "2500", "warning"), true);
                                return;
                            }

                            taikhoan_tb fullAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
                            if (fullAccount == null)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản không tồn tại.", "2000", "warning"), true);
                                return;
                            }

                            if (!PortalScope_cl.CanLoginAdmin(fullAccount.taikhoan, fullAccount.phanloai, fullAccount.permission))
                            {
                                string scope = PortalScope_cl.ResolveScope(fullAccount.taikhoan, fullAccount.phanloai, fullAccount.permission);
                                string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang shop" : "AhaSale";
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".", "2600", "warning"), true);
                                return;
                            }

                            if (PortalScope_cl.EnsureScope(fullAccount, PortalScope_cl.ScopeAdmin))
                            {
                                db.SubmitChanges();
                            }

                            string _taikhoan_mahoa = mahoa_cl.mahoa_Bcorn(account.TaiKhoan);
                            string _matkhau_mahoa = mahoa_cl.mahoa_Bcorn(account.MatKhau);
                            //lưu cookier với thông tin tài khoản để lưu giữ đăng nhập trong 7 ngày;
                            HttpCookie _ck = new HttpCookie("cookie_userinfo_admin_bcorn");
                            _ck["taikhoan"] = _taikhoan_mahoa;
                            _ck["matkhau"] = _matkhau_mahoa;
                            _ck.Expires = AhaTime_cl.Now.AddDays(7);
                            // Đặt thuộc tính HttpOnly để ngăn chặn truy cập từ mã JavaScript
                            _ck.HttpOnly = true;
                            // Đặt thuộc tính Secure để chỉ cho phép truyền cookie qua kết nối an toàn
                            _ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                            //chỉ định tên miền mà cookie được áp dụng. Bằng cách này, cookie chỉ được gửi đến máy chủ từ tên miền đã chỉ định, các miền con sẽ đc áp dụng theo
                            //_ck.Domain = "https://ahasale.vn";//bị ảnh hưởng khi ở localhost
                            Response.Cookies.Add(_ck);

                            //lưu session
                            Session["taikhoan"] = _taikhoan_mahoa;
                            Session["matkhau"] = _matkhau_mahoa;
                            Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "1000", "warning");

                            string _url_back = Convert.ToString(Session["url_back"]);

                            if (!string.IsNullOrEmpty(_url_back))
                            {
                                Response.Redirect(ResolveSafeAdminReturnUrl(_url_back, "/admin/default.aspx"), false);
                            }
                            else
                            {
                                string landingUrl = AdminRolePolicy_cl.ResolveLandingUrl(db, fullAccount.taikhoan);
                                Response.Redirect(landingUrl, false);
                            }

                            Context.ApplicationInstance.CompleteRequest();

                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản không tồn tại.", "2000", "warning"), true);

                    }
                }
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, "", _ex.StackTrace);
        }
    }

}
