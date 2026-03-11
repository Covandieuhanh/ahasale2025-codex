<%@ WebHandler Language="C#" Class="ContactLeadHandler" %>

using System;
using System.Linq;
using System.Web;

public class ContactLeadHandler : IHttpHandler
{
    private const string SourcePrefix = "home_profile:";

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
        {
            WriteJson(context, false, "Method not allowed.", 405);
            return;
        }

        string user = (context.Request.Form["user"] ?? "").Trim().ToLowerInvariant();
        string name = (context.Request.Form["name"] ?? "").Trim();
        string phone = NormalizePhone(context.Request.Form["phone"]);
        string email = (context.Request.Form["email"] ?? "").Trim();
        string message = (context.Request.Form["message"] ?? "").Trim();

        if (string.IsNullOrWhiteSpace(user))
        {
            WriteJson(context, false, "Không xác định được hồ sơ nhận liên hệ.", 400);
            return;
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            WriteJson(context, false, "Vui lòng nhập họ tên.", 400);
            return;
        }
        if (string.IsNullOrWhiteSpace(phone))
        {
            WriteJson(context, false, "Vui lòng nhập số điện thoại.", 400);
            return;
        }
        if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
        {
            WriteJson(context, false, "Email chưa đúng định dạng.", 400);
            return;
        }

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == user && p.block != true);
                if (acc == null)
                {
                    WriteJson(context, false, "Hồ sơ không tồn tại.", 404);
                    return;
                }

                data_yeucau_tuvan_table ob = new data_yeucau_tuvan_table();
                ob.ten = name;
                ob.sdt = phone;
                ob.noidung = BuildContactNote(email, message);
                ob.bin = false;
                ob.ngay = AhaTime_cl.Now;
                ob.kyhieu_nguon = SourcePrefix + user;
                db.data_yeucau_tuvan_tables.InsertOnSubmit(ob);
                db.SubmitChanges();
            }
        }
        catch (Exception ex)
        {
            WriteJson(context, false, "Lỗi gửi liên hệ: " + ex.Message, 500);
            return;
        }

        WriteJson(context, true, "OK", 200);
    }

    public bool IsReusable { get { return false; } }

    private static string NormalizePhone(string phone)
    {
        return (phone ?? "").Replace(" ", "").Replace(".", "").Replace("+", "").Replace("-", "");
    }

    private static string BuildContactNote(string email, string message)
    {
        string note = "";
        if (!string.IsNullOrWhiteSpace(email))
            note += "Email: " + email.Trim();
        if (!string.IsNullOrWhiteSpace(message))
        {
            if (note.Length > 0) note += "\n";
            note += "Noi dung: " + message.Trim();
        }
        return note.Trim();
    }

    private static void WriteJson(HttpContext context, bool ok, string message, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        string safe = HttpUtility.JavaScriptStringEncode(message ?? "");
        context.Response.Write("{\"ok\":" + (ok ? "true" : "false") + ",\"message\":\"" + safe + "\"}");
    }
}
