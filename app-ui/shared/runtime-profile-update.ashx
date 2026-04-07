<%@ WebHandler Language="C#" Class="AppUiRuntimeProfileUpdateHandler" %>

using System;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeProfileUpdateHandler : IHttpHandler
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

        try
        {
            RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
            if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
            {
                context.Response.StatusCode = 401;
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    message = "Bạn cần đăng nhập để cập nhật tài khoản."
                }));
                return;
            }

            string displayName = TrimTo((context.Request["display_name"] ?? "").Trim(), 120);
            string phone = TrimTo((context.Request["phone"] ?? "").Trim(), 30);
            string email = TrimTo((context.Request["email"] ?? "").Trim(), 120);
            string address = TrimTo((context.Request["address"] ?? "").Trim(), 220);
            string intro = TrimTo((context.Request["intro"] ?? "").Trim(), 500);

            if (string.IsNullOrWhiteSpace(displayName))
            {
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    message = "Vui lòng nhập họ và tên."
                }));
                return;
            }

            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
            {
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    message = "Email chưa đúng định dạng."
                }));
                return;
            }

            CoreDb_cl.Use(db =>
            {
                taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
                if (account == null)
                    throw new InvalidOperationException("ACCOUNT_NOT_FOUND");

                account.hoten = displayName;
                account.dienthoai = phone;
                account.email = email;
                account.diachi = address;
                account.gioithieu = intro;
                db.SubmitChanges();

                return 0;
            });

            context.Response.Write(serializer.Serialize(new
            {
                ok = true,
                message = "Đã lưu thông tin tài khoản."
            }));
        }
        catch (InvalidOperationException ex)
        {
            if (string.Equals(ex.Message, "ACCOUNT_NOT_FOUND", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 404;
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    message = "Không tìm thấy tài khoản để cập nhật."
                }));
                return;
            }
            throw;
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_profile_update", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Lỗi cập nhật tài khoản, vui lòng thử lại."
            }));
        }
    }

    public bool IsReusable { get { return false; } }

    private static string TrimTo(string value, int maxLength)
    {
        string safe = (value ?? "").Trim();
        if (maxLength <= 0 || safe.Length <= maxLength)
            return safe;
        return safe.Substring(0, maxLength).Trim();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            MailAddress addr = new MailAddress(email);
            return string.Equals(addr.Address, email, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
