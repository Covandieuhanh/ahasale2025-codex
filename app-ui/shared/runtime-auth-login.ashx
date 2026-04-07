<%@ WebHandler Language="C#" Class="AppUiRuntimeAuthLoginHandler" %>

using System;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

public class AppUiRuntimeAuthLoginHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 405;
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Phương thức không hợp lệ."
            }));
            return;
        }

        string loginId = (context.Request["login_id"] ?? "").Trim();
        string password = context.Request["password"] ?? "";
        string returnUrlRaw = context.Request["return_url"] ?? "";
        string returnUrl = check_login_cl.NormalizeUnifiedReturnUrl(returnUrlRaw);
        if (string.IsNullOrWhiteSpace(returnUrl))
            returnUrl = "/app-ui/home/default.aspx?ui_mode=app";

        if (string.IsNullOrWhiteSpace(loginId))
        {
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Vui lòng nhập tài khoản / số điện thoại / email."
            }));
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Vui lòng nhập mật khẩu."
            }));
            return;
        }

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                AccountLoginInfo account = AccountAuth_cl.FindAccountByLoginId(db, loginId, PortalScope_cl.ScopeHome);
                if (account == null)
                    account = AccountAuth_cl.FindAccountByLoginId(db, loginId);

                if (account != null && account.IsAmbiguous)
                {
                    context.Response.Write(serializer.Serialize(new
                    {
                        ok = false,
                        message = "Thông tin đăng nhập đang trùng trên nhiều tài khoản. Vui lòng liên hệ admin để xử lý."
                    }));
                    return;
                }

                if (account == null || string.IsNullOrWhiteSpace(account.TaiKhoan))
                {
                    context.Response.Write(serializer.Serialize(new
                    {
                        ok = false,
                        message = "Thông tin đăng nhập không tồn tại."
                    }));
                    return;
                }

                if (!AccountAuth_cl.IsPasswordValid(password, account.MatKhau))
                {
                    context.Response.Write(serializer.Serialize(new
                    {
                        ok = false,
                        message = "Mật khẩu không đúng."
                    }));
                    return;
                }

                taikhoan_tb fullAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
                if (fullAccount == null)
                {
                    context.Response.Write(serializer.Serialize(new
                    {
                        ok = false,
                        message = "Thông tin đăng nhập không tồn tại."
                    }));
                    return;
                }

                if (fullAccount.block == true)
                {
                    context.Response.Write(serializer.Serialize(new
                    {
                        ok = false,
                        message = "Tài khoản đã bị khóa."
                    }));
                    return;
                }

                if (fullAccount.hansudung != null && AhaTime_cl.Now > fullAccount.hansudung.Value)
                {
                    context.Response.Write(serializer.Serialize(new
                    {
                        ok = false,
                        message = "Tài khoản đã hết hạn sử dụng."
                    }));
                    return;
                }

                // App UI đăng nhập theo scope Home để giữ trải nghiệm nhất quán.
                if (PortalScope_cl.EnsureScope(fullAccount, PortalScope_cl.ScopeHome))
                    db.SubmitChanges();

                SignInHome(context, account);

                string displayName = (fullAccount.hoten ?? "").Trim();
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = "Home " + (account.TaiKhoan ?? "").Trim();

                context.Response.Write(serializer.Serialize(new
                {
                    ok = true,
                    message = "Đăng nhập thành công.",
                    redirect_url = returnUrl,
                    user_summary = new
                    {
                        display_name = displayName,
                        username = "Home " + (account.TaiKhoan ?? "").Trim(),
                        phone = (account.TaiKhoan ?? "").Trim()
                    }
                }));
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_auth_login", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại."
            }));
        }
    }

    public bool IsReusable { get { return false; } }

    private static void SignInHome(HttpContext context, AccountLoginInfo account)
    {
        if (context == null || account == null)
            return;

        string tkEncoded = mahoa_cl.mahoa_Bcorn(account.TaiKhoan);
        string mkEncoded = mahoa_cl.mahoa_Bcorn(account.MatKhau);

        HttpCookie cookie = new HttpCookie("cookie_userinfo_home_bcorn");
        cookie["taikhoan"] = tkEncoded;
        cookie["matkhau"] = mkEncoded;
        cookie.Expires = AhaTime_cl.Now.AddDays(7);
        cookie.HttpOnly = true;
        cookie.Secure = AccountAuth_cl.ShouldUseSecureCookie(context.Request);
        context.Response.Cookies.Add(cookie);

        context.Session["taikhoan_home"] = tkEncoded;
        context.Session["matkhau_home"] = mkEncoded;

        PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
        check_login_cl.del_all_cookie_session_shop();
        check_login_cl.del_all_cookie_session_admin();
    }
}
