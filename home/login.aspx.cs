using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_login : System.Web.UI.Page
{
    private const string SessionReturnUrlKey = "url_back_home";
    private const string ReturnUrlCookieName = app_cookie_policy_class.home_return_url_cookie;
    private const string LoginSubmitHandledKey = "__home_login_submit_handled";

    protected void Page_Load(object sender, EventArgs e)
    {
        HandleSwitchModeRequest();
        CaptureRequestedReturnUrlFromRequest();

        if (this.Form != null && Request != null)
        {
            string rawUrl = ResolveRequestedPathAndQuery();
            if (string.IsNullOrWhiteSpace(rawUrl))
                rawUrl = "/home/login.aspx";
            this.Form.Action = rawUrl;
            CaptureReturnUrlFromAction(rawUrl);
        }

        CaptureReturnUrlFromQuery();
        EnsureReturnUrlState();

        PlaceHolder phSwitchHomeMode = FindControlRecursive(this, "ph_switch_home_mode") as PlaceHolder;
        if (phSwitchHomeMode != null)
            phSwitchHomeMode.Visible = (!PortalActiveMode_cl.IsHomeActive() && PortalActiveMode_cl.HasHomeCredential());

        if (TryRedirectAuthenticatedHomeUserFromLogin())
            return;

        if (ShouldHandleDirectLoginSubmit())
        {
            ProcessLoginSubmit();
            return;
        }

        if (!IsPostBack)
        {
            if (ShouldAutoRedirectAuthenticatedUser() && TryAutoRedirectWhenAlreadyAuthenticated())
                return;

            txt_user.Focus();
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        if (this.Form != null)
            CaptureReturnUrlFromAction(this.Form.Action ?? "");

        EnsureReturnUrlState();
        base.OnPreRender(e);
    }

    protected void but_login_Click(object sender, EventArgs e)
    {
        ProcessLoginSubmit();
    }

    private void ProcessLoginSubmit()
    {
        if (Context != null && Context.Items[LoginSubmitHandledKey] is bool && (bool)Context.Items[LoginSubmitHandledKey])
            return;

        if (Context != null)
            Context.Items[LoginSubmitHandledKey] = true;

        try
        {
            CaptureReturnUrlFromPostedFields();

            using (dbDataContext db = new dbDataContext())
            {
                string loginId = (txt_user.Text ?? "").Trim();
                string pass = txt_pass.Text ?? "";

                if (string.IsNullOrEmpty(loginId))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập tài khoản / số điện thoại / email.", "Thông báo", true, "warning");
                    return;
                }

                if (string.IsNullOrEmpty(pass))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập mật khẩu.", "Thông báo", true, "warning");
                    return;
                }

                string preferredScope = ResolvePreferredScopeFromReturnUrl();
                AccountLoginInfo account = null;
                if (!string.IsNullOrEmpty(preferredScope))
                    account = AccountAuth_cl.FindAccountByLoginId(db, loginId, preferredScope);
                if (account == null && string.IsNullOrEmpty(preferredScope))
                    account = AccountAuth_cl.FindAccountByLoginId(db, loginId, PortalScope_cl.ScopeHome);
                if (account == null)
                    account = AccountAuth_cl.FindAccountByLoginId(db, loginId);

                if (account != null && account.IsAmbiguous)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Thông tin đăng nhập đang trùng trên nhiều tài khoản. Vui lòng liên hệ admin để xử lý.", "Thông báo", true, "warning");
                    return;
                }
                if (account == null || string.IsNullOrEmpty(account.TaiKhoan))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Thông tin đăng nhập không tồn tại.", "Thông báo", true, "warning");
                    return;
                }

                if (!AccountAuth_cl.IsPasswordValid(pass, account.MatKhau))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Mật khẩu không đúng.", "Thông báo", true, "warning");
                    return;
                }

                taikhoan_tb fullAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
                if (fullAccount == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Thông tin đăng nhập không tồn tại.", "Thông báo", true, "warning");
                    return;
                }

                if (fullAccount.block == true)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản đã bị khóa.", "Thông báo", true, "warning");
                    return;
                }

                if (fullAccount.hansudung != null && AhaTime_cl.Now > fullAccount.hansudung.Value)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản của bạn đã hết hạn sử dụng.", "Thông báo", true, "warning");
                    return;
                }

                string baseScope = PortalScope_cl.ResolveScope(fullAccount.taikhoan, fullAccount.phanloai, fullAccount.permission);
                if (string.IsNullOrEmpty(baseScope))
                    baseScope = PortalScope_cl.ScopeHome;

                string scope = baseScope;
                bool canAccessShopSpace = SpaceAccess_cl.CanAccessShop(db, fullAccount);
                if (string.Equals(preferredScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase) && canAccessShopSpace)
                    scope = PortalScope_cl.ScopeShop;

                if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
                {
                    ShopStatus_cl.EnsureSchemaSafe(db);
                    if (!ShopStatus_cl.IsShopApproved(db, fullAccount) && !canAccessShopSpace)
                    {
                        string msg = (fullAccount.TrangThai_Shop == ShopStatus_cl.StatusPending)
                            ? "Tài khoản gian hàng đối tác chưa được duyệt."
                            : "Tài khoản gian hàng đối tác đang bị khóa.";
                        Helper_Tabler_cl.ShowModal(this.Page, msg, "Thông báo", true, "warning");
                        return;
                    }
                }

                if (string.Equals(baseScope, scope, StringComparison.OrdinalIgnoreCase)
                    && PortalScope_cl.EnsureScope(fullAccount, scope))
                    db.SubmitChanges();

                SignInByScope(scope, account);

                string resolvedReturnBeforeRedirect = ReadRequestedReturnUrl() ?? "";
                string normalizedReturnBeforeRedirect = NormalizeLocalReturnUrl(resolvedReturnBeforeRedirect);
                string redirectUrl = ResolvePostLoginUrl(db, scope, fullAccount.taikhoan);
                ClearReturnUrlState();
                Response.Redirect(redirectUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "", ex.StackTrace);
            Helper_Tabler_cl.ShowModal(this.Page, "Có lỗi xảy ra. Vui lòng thử lại.", "Thông báo", true, "warning");
        }
    }

    private bool ShouldHandleDirectLoginSubmit()
    {
        if (Request == null)
            return false;

        if (!string.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            return false;

        if (IsMicrosoftAjaxAsyncPost())
            return false;

        string submitValue = "";
        if (but_login != null)
            submitValue = (Request.Form[but_login.UniqueID] ?? "").Trim();

        if (submitValue == "")
            submitValue = (Request.Form["ctl00$main$but_login"] ?? "").Trim();

        if (submitValue == "")
            return false;

        string userValue = "";
        if (txt_user != null)
            userValue = (Request.Form[txt_user.UniqueID] ?? "").Trim();

        if (userValue == "")
            userValue = (Request.Form["ctl00$main$txt_user"] ?? "").Trim();

        return userValue != "";
    }

    private bool IsMicrosoftAjaxAsyncPost()
    {
        string asyncForm = (Request.Form["__ASYNCPOST"] ?? "").Trim();
        if (string.Equals(asyncForm, "true", StringComparison.OrdinalIgnoreCase))
            return true;

        string ajaxHeader = (Request.Headers["X-MicrosoftAjax"] ?? "").Trim();
        return string.Equals(ajaxHeader, "Delta=true", StringComparison.OrdinalIgnoreCase);
    }

    private void HandleSwitchModeRequest()
    {
        string switchMode = (Request.QueryString["switch"] ?? "").Trim().ToLowerInvariant();
        if (switchMode == "home")
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
            check_login_cl.del_all_cookie_session_shop();
            return;
        }

        if (switchMode == "shop")
        {
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
            check_login_cl.del_all_cookie_session_home();
        }
    }

    private bool TryAutoRedirectWhenAlreadyAuthenticated()
    {
        string preferredScope = ResolvePreferredScopeFromReturnUrl();

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account;

            if (string.Equals(preferredScope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase)
                && TryResolveCredential(db, "cookie_userinfo_home_bcorn", "taikhoan_home", "matkhau_home", PortalScope_cl.ScopeHome, out account))
            {
                PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
                check_login_cl.del_all_cookie_session_shop();
                check_login_cl.del_all_cookie_session_admin();
                Session["home_modal_msg"] = "Bạn đã đăng nhập. Vui lòng đăng xuất để đăng nhập tài khoản khác.";
                Session["home_modal_title"] = "Thông báo";
                Session["home_modal_type"] = "warning";
                string url = ResolvePostLoginUrl(db, PortalScope_cl.ScopeHome, account.taikhoan);
                ClearReturnUrlState();
                Response.Redirect(url, false);
                Context.ApplicationInstance.CompleteRequest();
                return true;
            }

            if (string.Equals(preferredScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase)
                && TryResolveCredential(db, "cookie_userinfo_shop_bcorn", "taikhoan_shop", "matkhau_shop", PortalScope_cl.ScopeShop, out account))
            {
                PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
                check_login_cl.del_all_cookie_session_home();
                check_login_cl.del_all_cookie_session_admin();
                Session["home_modal_msg"] = "Bạn đã đăng nhập. Vui lòng đăng xuất để đăng nhập tài khoản khác.";
                Session["home_modal_title"] = "Thông báo";
                Session["home_modal_type"] = "warning";
                string url = ResolvePostLoginUrl(db, PortalScope_cl.ScopeShop, account.taikhoan);
                ClearReturnUrlState();
                Response.Redirect(url, false);
                Context.ApplicationInstance.CompleteRequest();
                return true;
            }

            if (string.Equals(preferredScope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase)
                && TryResolveCredential(db, "cookie_userinfo_admin_bcorn", "taikhoan", "matkhau", PortalScope_cl.ScopeAdmin, out account))
            {
                PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
                check_login_cl.del_all_cookie_session_home();
                check_login_cl.del_all_cookie_session_shop();
                Session["home_modal_msg"] = "Bạn đã đăng nhập. Vui lòng đăng xuất để đăng nhập tài khoản khác.";
                Session["home_modal_title"] = "Thông báo";
                Session["home_modal_type"] = "warning";
                string url = ResolvePostLoginUrl(db, PortalScope_cl.ScopeAdmin, account.taikhoan);
                ClearReturnUrlState();
                Response.Redirect(url, false);
                Context.ApplicationInstance.CompleteRequest();
                return true;
            }
        }

        return false;
    }

    private bool TryRedirectAuthenticatedHomeUserFromLogin()
    {
        taikhoan_tb account;
        using (dbDataContext db = new dbDataContext())
        {
            if (!TryResolveCredential(db, "cookie_userinfo_home_bcorn", "taikhoan_home", "matkhau_home", PortalScope_cl.ScopeHome, out account))
                return false;
        }

        string targetUrl = Request == null ? "/dang-nhap" : (Request.RawUrl ?? "/dang-nhap");
        string logoutUrl = "/home/logout.aspx?return_url=" + HttpUtility.UrlEncode(targetUrl);
        string redirectUrl = "/home/default.aspx?need_logout=1&return_url=" + HttpUtility.UrlEncode(targetUrl);

        Session["home_logout_return"] = targetUrl;
        Session["home_modal2_msg"] = "Bạn phải đăng xuất tài khoản Home hiện tại để được phép truy cập phần Đăng nhập.";
        Session["home_modal2_title"] = "Thông báo";
        Session["home_modal2_type"] = "warning";
        Session["home_modal2_primary_text"] = "Đăng xuất";
        Session["home_modal2_primary_href"] = logoutUrl;
        Session["home_modal2_secondary_text"] = "Về trang chủ";
        Session["home_modal2_secondary_href"] = "/home/default.aspx";

        ClearReturnUrlState();
        Response.Redirect(redirectUrl, false);
        Context.ApplicationInstance.CompleteRequest();
        return true;
    }

    private bool TryResolveCredential(
        dbDataContext db,
        string cookieName,
        string sessionUserKey,
        string sessionPassKey,
        string expectedScope,
        out taikhoan_tb account)
    {
        account = null;

        string tk;
        string mk;
        if (!TryReadCredential(cookieName, sessionUserKey, sessionPassKey, out tk, out mk))
            return false;

        taikhoan_tb found = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (found == null)
            return false;

        if (!AccountAuth_cl.IsPasswordValid(mk, found.matkhau))
            return false;

        if (found.block == true)
            return false;

        if (found.hansudung != null && AhaTime_cl.Now > found.hansudung.Value)
            return false;

        string resolvedScope = PortalScope_cl.ResolveScope(found.taikhoan, found.phanloai, found.permission);
        bool matchExpectedScope = string.Equals(resolvedScope, expectedScope, StringComparison.OrdinalIgnoreCase);
        if (!matchExpectedScope && string.Equals(expectedScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            matchExpectedScope = SpaceAccess_cl.CanAccessShop(db, found);
        if (!matchExpectedScope)
            return false;

        if (string.Equals(expectedScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
        {
            ShopStatus_cl.EnsureSchemaSafe(db);
            if (!ShopStatus_cl.IsShopApproved(db, found) && !SpaceAccess_cl.CanAccessShop(db, found))
                return false;
        }

        account = found;
        return true;
    }

    private bool TryReadCredential(string cookieName, string sessionUserKey, string sessionPassKey, out string tk, out string mk)
    {
        tk = "";
        mk = "";
        string tkEncoded = "";
        string mkEncoded = "";

        try
        {
            HttpCookie ck = Request.Cookies[cookieName];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]) && !string.IsNullOrEmpty(ck["matkhau"]))
            {
                tkEncoded = ck["taikhoan"];
                mkEncoded = ck["matkhau"];
            }
            else if (Session[sessionUserKey] != null && Session[sessionPassKey] != null)
            {
                tkEncoded = Session[sessionUserKey].ToString();
                mkEncoded = Session[sessionPassKey].ToString();
            }

            if (string.IsNullOrEmpty(tkEncoded) || string.IsNullOrEmpty(mkEncoded))
                return false;

            tk = mahoa_cl.giaima_Bcorn(tkEncoded);
            mk = mahoa_cl.giaima_Bcorn(mkEncoded);
            return !string.IsNullOrWhiteSpace(tk);
        }
        catch
        {
            return false;
        }
    }

    private void SignInByScope(string scope, AccountLoginInfo account)
    {
        string tkEncoded = mahoa_cl.mahoa_Bcorn(account.TaiKhoan);
        string mkEncoded = mahoa_cl.mahoa_Bcorn(account.MatKhau);

        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
        {
            WriteAuthCookie("cookie_userinfo_shop_bcorn", tkEncoded, mkEncoded);
            Session["taikhoan_shop"] = tkEncoded;
            Session["matkhau_shop"] = mkEncoded;
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
            check_login_cl.del_all_cookie_session_home();
            check_login_cl.del_all_cookie_session_admin();
            return;
        }

        if (string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
        {
            WriteAuthCookie("cookie_userinfo_admin_bcorn", tkEncoded, mkEncoded);
            Session["taikhoan"] = tkEncoded;
            Session["matkhau"] = mkEncoded;
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
            check_login_cl.del_all_cookie_session_home();
            check_login_cl.del_all_cookie_session_shop();
            return;
        }

        WriteAuthCookie("cookie_userinfo_home_bcorn", tkEncoded, mkEncoded);
        Session["taikhoan_home"] = tkEncoded;
        Session["matkhau_home"] = mkEncoded;
        PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
        check_login_cl.del_all_cookie_session_shop();
        check_login_cl.del_all_cookie_session_admin();
    }

    private void WriteAuthCookie(string cookieName, string tkEncoded, string mkEncoded)
    {
        HttpCookie ck = new HttpCookie(cookieName);
        ck["taikhoan"] = tkEncoded;
        ck["matkhau"] = mkEncoded;
        ck.Expires = AhaTime_cl.Now.AddDays(7);
        ck.HttpOnly = true;
        ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
        Response.Cookies.Add(ck);
    }

    private string ResolvePostLoginUrl(dbDataContext db, string scope, string taiKhoan)
    {
        string fallback = ResolveScopeFallbackUrl(db, scope, taiKhoan);
        string stored = ReadRequestedReturnUrl();
        string normalized = NormalizeLocalReturnUrl(stored);

        if (IsReturnUrlAllowedForScope(scope, normalized))
            return normalized;

        return fallback;
    }

    private string ResolveScopeFallbackUrl(dbDataContext db, string scope, string taiKhoan)
    {
        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            return "/shop/default.aspx";

        if (string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
        {
            string landing = AdminRolePolicy_cl.ResolveLandingUrl(db, taiKhoan);
            if (string.IsNullOrWhiteSpace(landing) || landing.IndexOf("login", StringComparison.OrdinalIgnoreCase) >= 0)
                return "/admin/default.aspx";
            return landing;
        }

        string requested = NormalizeLocalReturnUrl(ReadRequestedReturnUrl());
        string requestedPath = ExtractPath(requested).ToLowerInvariant();
        if (IsExactOrUnderPath(requestedPath, "/gianhang/admin")
            || IsExactOrUnderPath(requestedPath, "/event/admin"))
            return requested;

        if (requestedPath.StartsWith("/daugia/admin", StringComparison.Ordinal)
            && requested.IndexOf("shop_portal=1", StringComparison.OrdinalIgnoreCase) < 0)
            return requested;

        return "/home/default.aspx";
    }

    private void CaptureReturnUrlFromQuery()
    {
        string raw = ResolveRawReturnUrlFromRequest();
        if (!string.IsNullOrWhiteSpace(raw))
        {
            string normalized = NormalizeLocalReturnUrl(raw);
            if (!string.IsNullOrEmpty(normalized))
            {
                PersistReturnUrlState(normalized);
                return;
            }
        }

        string existing = NormalizeLocalReturnUrl((hid_return_url == null ? "" : hid_return_url.Value) ?? "");
        if (string.IsNullOrWhiteSpace(existing))
            existing = NormalizeLocalReturnUrl(Convert.ToString(Session[SessionReturnUrlKey]));
        if (string.IsNullOrWhiteSpace(existing))
            existing = NormalizeLocalReturnUrl(ReadReturnUrlCookie());

        if (!string.IsNullOrWhiteSpace(existing))
        {
            PersistReturnUrlState(existing);
            return;
        }
    }

    private void CaptureRequestedReturnUrlFromRequest()
    {
        string normalized = NormalizeLocalReturnUrl(ResolveRawReturnUrlFromRequest());
        if (!string.IsNullOrWhiteSpace(normalized))
            PersistReturnUrlState(normalized);
    }

    private void EnsureReturnUrlState()
    {
        string normalized = NormalizeLocalReturnUrl(ReadRequestedReturnUrl());
        if (string.IsNullOrWhiteSpace(normalized))
            normalized = NormalizeLocalReturnUrl(ResolveRawReturnUrlFromRequest());

        if (string.IsNullOrWhiteSpace(normalized))
            return;

        PersistReturnUrlState(normalized);
    }

    private void CaptureReturnUrlFromAction(string rawActionUrl)
    {
        string normalized = NormalizeLocalReturnUrl(ExtractReturnUrlFromPathAndQuery(rawActionUrl));
        if (string.IsNullOrWhiteSpace(normalized))
            return;

        PersistReturnUrlState(normalized);
    }

    private bool ShouldAutoRedirectAuthenticatedUser()
    {
        string raw = ResolveRawReturnUrlFromRequest();
        string normalized = NormalizeLocalReturnUrl(raw);
        return !string.IsNullOrEmpty(normalized);
    }

    private string ResolvePreferredScopeFromReturnUrl()
    {
        string raw = ResolveExplicitReturnUrlFromCurrentRequest();
        string normalized = NormalizeLocalReturnUrl(raw);
        if (string.IsNullOrEmpty(normalized))
            return "";

        string path = ExtractPath(normalized).ToLowerInvariant();
        if (path.StartsWith("/shop/", StringComparison.Ordinal))
            return PortalScope_cl.ScopeShop;
        if (path.StartsWith("/admin/", StringComparison.Ordinal))
            return PortalScope_cl.ScopeAdmin;
        if (IsExactOrUnderPath(path, "/gianhang/admin")
            || IsExactOrUnderPath(path, "/event/admin"))
            return PortalScope_cl.ScopeHome;
        if (path.StartsWith("/daugia/admin", StringComparison.Ordinal))
        {
            if (normalized.IndexOf("shop_portal=1", StringComparison.OrdinalIgnoreCase) >= 0)
                return PortalScope_cl.ScopeShop;
            return PortalScope_cl.ScopeHome;
        }
        if (path == "/" || path.StartsWith("/home/", StringComparison.Ordinal))
            return PortalScope_cl.ScopeHome;

        return "";
    }

    private string ResolveExplicitReturnUrlFromCurrentRequest()
    {
        string raw = Request == null ? "" : (Request.QueryString["return_url"] ?? Request.QueryString["returnUrl"] ?? "");
        if (!string.IsNullOrWhiteSpace(raw))
            return raw;

        foreach (string candidate in EnumerateRequestPathAndQueryCandidates())
        {
            raw = ExtractReturnUrlFromPathAndQuery(candidate);
            if (!string.IsNullOrWhiteSpace(raw))
                return raw;
        }

        return "";
    }

    private string ReadRequestedReturnUrl()
    {
        string shadowValue = (Request.Form["home_return_url_shadow"] ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(shadowValue))
            return shadowValue;

        if (hid_return_url != null && !string.IsNullOrWhiteSpace(hid_return_url.Value))
            return hid_return_url.Value;

        string formValue = (Request.Form["ctl00$main$hid_return_url"] ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(formValue))
            return formValue;

        string stored = Convert.ToString(Session[SessionReturnUrlKey]);
        if (!string.IsNullOrWhiteSpace(stored))
            return stored;

        string cookieValue = ReadReturnUrlCookie();
        if (!string.IsNullOrWhiteSpace(cookieValue))
            return cookieValue;

        string queryRaw = ResolveRawReturnUrlFromRequest();
        if (!string.IsNullOrWhiteSpace(queryRaw))
            return queryRaw;

        return "";
    }

    protected string RenderRequestedReturnUrl()
    {
        string normalized = NormalizeLocalReturnUrl(ReadRequestedReturnUrl());
        if (!string.IsNullOrWhiteSpace(normalized))
            return normalized;

        return NormalizeLocalReturnUrl(ResolveRawReturnUrlFromRequest());
    }

    private string ResolveRawReturnUrlFromRequest()
    {
        string raw = Request.QueryString["return_url"] ?? Request.QueryString["returnUrl"] ?? "";
        if (!string.IsNullOrWhiteSpace(raw))
            return raw;

        foreach (string candidate in EnumerateRequestPathAndQueryCandidates())
        {
            raw = ExtractReturnUrlFromPathAndQuery(candidate);
            if (!string.IsNullOrWhiteSpace(raw))
                return raw;
        }

        raw = ReadReturnUrlCookie();

        return raw ?? "";
    }

    private void CaptureReturnUrlFromPostedFields()
    {
        string normalized = NormalizeLocalReturnUrl(Request.Form["home_return_url_shadow"] ?? "");
        if (string.IsNullOrWhiteSpace(normalized))
            normalized = NormalizeLocalReturnUrl(Request.Form["ctl00$main$hid_return_url"] ?? "");
        if (string.IsNullOrWhiteSpace(normalized) && hid_return_url != null)
            normalized = NormalizeLocalReturnUrl(Request.Form[hid_return_url.UniqueID] ?? "");

        if (!string.IsNullOrWhiteSpace(normalized))
            PersistReturnUrlState(normalized);
    }

    private string ResolveRequestedPathAndQuery()
    {
        foreach (string candidate in EnumerateRequestPathAndQueryCandidates())
        {
            string value = (candidate ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return "";
    }

    private string[] EnumerateRequestPathAndQueryCandidates()
    {
        string[] candidates = new[]
        {
            ReadServerVariable("HTTP_X_ORIGINAL_URL"),
            ReadServerVariable("HTTP_X_REWRITE_URL"),
            ReadServerVariable("UNENCODED_URL"),
            ReadServerVariable("REQUEST_URI"),
            Request == null || Request.Url == null ? "" : Request.Url.PathAndQuery,
            Request == null ? "" : Request.RawUrl,
            this.Form == null ? "" : this.Form.Action
        };

        return candidates
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private string ReadServerVariable(string key)
    {
        if (Request == null || Request.ServerVariables == null || string.IsNullOrWhiteSpace(key))
            return "";

        try
        {
            return (Request.ServerVariables[key] ?? "").Trim();
        }
        catch
        {
            return "";
        }
    }

    private string ReadReturnUrlCookie()
    {
        if (Request == null || Request.Cookies == null)
            return "";

        HttpCookie cookie = Request.Cookies[ReturnUrlCookieName];
        if (cookie == null)
            return "";

        return NormalizeLocalReturnUrl(cookie.Value);
    }

    private void PersistReturnUrlState(string normalized)
    {
        string safeValue = NormalizeLocalReturnUrl(normalized);
        if (string.IsNullOrWhiteSpace(safeValue))
        {
            ClearReturnUrlState();
            return;
        }

        Session[SessionReturnUrlKey] = safeValue;
        if (hid_return_url != null)
            hid_return_url.Value = safeValue;

        app_cookie_policy_class.persist_cookie(HttpContext.Current, ReturnUrlCookieName, safeValue, 1);
    }

    private void ClearReturnUrlState()
    {
        Session.Remove(SessionReturnUrlKey);
        if (hid_return_url != null)
            hid_return_url.Value = "";

        app_cookie_policy_class.expire_cookie(HttpContext.Current, ReturnUrlCookieName);
        app_cookie_policy_class.expire_cookie(HttpContext.Current, app_cookie_policy_class.admin_return_url_cookie);
    }

    private string ExtractReturnUrlFromPathAndQuery(string rawPathAndQuery)
    {
        string value = (rawPathAndQuery ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "";

        int questionMark = value.IndexOf('?');
        if (questionMark < 0 || questionMark >= value.Length - 1)
            return "";

        string query = value.Substring(questionMark + 1);
        var parsed = HttpUtility.ParseQueryString(query);
        return parsed["return_url"] ?? parsed["returnUrl"] ?? "";
    }

    private bool IsReturnUrlAllowedForScope(string scope, string normalizedUrl)
    {
        if (string.IsNullOrEmpty(normalizedUrl))
            return false;

        string path = ExtractPath(normalizedUrl).ToLowerInvariant();
        if (path.StartsWith("/dang-nhap", StringComparison.Ordinal)
            || path.StartsWith("/home/login", StringComparison.Ordinal)
            || path.StartsWith("/shop/login", StringComparison.Ordinal)
            || path.StartsWith("/admin/login", StringComparison.Ordinal)
            || path.StartsWith("/gianhang/admin/login", StringComparison.Ordinal))
            return false;

        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
        {
            if (path.StartsWith("/shop/", StringComparison.Ordinal))
                return true;
            if (path.StartsWith("/daugia/admin", StringComparison.Ordinal))
                return normalizedUrl.IndexOf("shop_portal=1", StringComparison.OrdinalIgnoreCase) >= 0;
            return false;
        }

        if (string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
        {
            if (path.StartsWith("/admin/", StringComparison.Ordinal)
                || IsExactOrUnderPath(path, "/gianhang/admin")
                || IsExactOrUnderPath(path, "/event/admin"))
                return true;
            if (path.StartsWith("/daugia/admin", StringComparison.Ordinal))
                return normalizedUrl.IndexOf("shop_portal=1", StringComparison.OrdinalIgnoreCase) < 0;
            return false;
        }

        if (IsExactOrUnderPath(path, "/gianhang/admin")
            || IsExactOrUnderPath(path, "/event/admin"))
            return true;

        if (path.StartsWith("/daugia/admin", StringComparison.Ordinal))
            return normalizedUrl.IndexOf("shop_portal=1", StringComparison.OrdinalIgnoreCase) < 0;

        if (path.StartsWith("/shop/", StringComparison.Ordinal)
            || path.StartsWith("/admin/", StringComparison.Ordinal))
            return false;

        return true;
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

        bool looksAbsoluteWebUrl =
            value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

        Uri absolute;
        if (looksAbsoluteWebUrl && Uri.TryCreate(value, UriKind.Absolute, out absolute))
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
            || lower.StartsWith("/home/login")
            || lower.StartsWith("/login")
            || lower.StartsWith("/shop/login")
            || lower.StartsWith("/shop/dang-nhap")
            || lower.StartsWith("/admin/login")
            || lower.StartsWith("/gianhang/admin/login"))
            return "";

        return value;
    }

    private string ExtractPath(string pathAndQuery)
    {
        string raw = (pathAndQuery ?? "").Trim();
        int questionMark = raw.IndexOf('?');
        if (questionMark >= 0)
            return raw.Substring(0, questionMark);
        return raw;
    }

    private bool IsExactOrUnderPath(string actualPath, string basePath)
    {
        string actual = (actualPath ?? "").Trim();
        string baseValue = (basePath ?? "").Trim();
        if (actual == "" || baseValue == "")
            return false;

        return string.Equals(actual, baseValue, StringComparison.Ordinal)
            || actual.StartsWith(baseValue + "/", StringComparison.Ordinal);
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
