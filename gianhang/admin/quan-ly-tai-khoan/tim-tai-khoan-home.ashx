<%@ WebHandler Language="C#" Class="GianHangAdminHomeAccountSearchHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public class GianHangAdminHomeAccountSearchHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    private sealed class SearchItem
    {
        public string AccountKey { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = Encoding.UTF8;

        if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
        {
            WriteJson(context, 405, false, "Method not allowed.", null);
            return;
        }

        string currentLegacyUser = ((context.Session["user"] ?? "") + "").Trim();
        if (currentLegacyUser == "")
        {
            WriteJson(context, 401, false, "Phiên đăng nhập /gianhang/admin đã hết.", null);
            return;
        }

        string ownerAccount = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (ownerAccount == "")
        {
            WriteJson(context, 403, false, "Không xác định được không gian hiện tại.", null);
            return;
        }

        string keywordRaw = (context.Request.QueryString["q"] ?? "").Trim();
        if (keywordRaw.Length < 2)
        {
            WriteJson(context, 200, true, "", new List<SearchItem>());
            return;
        }

        string keyword = keywordRaw.ToLowerInvariant();
        string phoneKeyword = AccountAuth_cl.NormalizePhone(keywordRaw);

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                CoreSchemaMigration_cl.EnsureSchemaSafe(db);

                List<taikhoan_tb> candidates = db.taikhoan_tbs
                    .Where(p =>
                        ((p.taikhoan ?? "").Contains(keyword))
                        || ((p.hoten ?? "").Contains(keywordRaw))
                        || ((p.email ?? "").Contains(keywordRaw))
                        || ((p.dienthoai ?? "").Contains(keywordRaw)))
                    .Take(50)
                    .ToList();

                List<SearchItem> items = candidates
                    .Where(p => p != null)
                    .Where(p => PortalScope_cl.CanLoginHome(p.taikhoan, p.phanloai, p.permission))
                    .Where(p => !string.Equals((p.taikhoan ?? "").Trim(), ownerAccount, StringComparison.OrdinalIgnoreCase))
                    .Where(p =>
                    {
                        string account = ((p.taikhoan ?? "") + "").Trim().ToLowerInvariant();
                        string fullName = ((p.hoten ?? "") + "").Trim();
                        string email = ((p.email ?? "") + "").Trim();
                        string phone = ((p.dienthoai ?? "") + "").Trim();
                        string normalizedPhone = AccountAuth_cl.NormalizePhone(phone);

                        return account.Contains(keyword)
                            || fullName.IndexOf(keywordRaw, StringComparison.OrdinalIgnoreCase) >= 0
                            || email.IndexOf(keywordRaw, StringComparison.OrdinalIgnoreCase) >= 0
                            || (!string.IsNullOrWhiteSpace(phoneKeyword) && normalizedPhone == phoneKeyword)
                            || phone.Contains(keywordRaw);
                    })
                    .Select(p => new SearchItem
                    {
                        AccountKey = ((p.taikhoan ?? "") + "").Trim(),
                        FullName = ((p.hoten ?? "") + "").Trim(),
                        Phone = ((p.dienthoai ?? "") + "").Trim(),
                        Email = ((p.email ?? "") + "").Trim()
                    })
                    .OrderBy(p => p.AccountKey)
                    .Take(8)
                    .ToList();

                WriteJson(context, 200, true, "", items);
            }
        }
        catch (Exception ex)
        {
            WriteJson(context, 500, false, "Không thể tìm tài khoản AhaSale: " + ex.Message, null);
        }
    }

    public bool IsReusable { get { return false; } }

    private static void WriteJson(HttpContext context, int statusCode, bool ok, string message, IList<SearchItem> items)
    {
        context.Response.StatusCode = statusCode;
        StringBuilder sb = new StringBuilder();
        sb.Append("{\"ok\":").Append(ok ? "true" : "false");
        sb.Append(",\"message\":\"").Append(HttpUtility.JavaScriptStringEncode(message ?? "")).Append("\"");
        sb.Append(",\"items\":[");
        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SearchItem item = items[i];
                if (i > 0) sb.Append(",");
                sb.Append("{");
                sb.Append("\"account\":\"").Append(HttpUtility.JavaScriptStringEncode(item.AccountKey ?? "")).Append("\",");
                sb.Append("\"name\":\"").Append(HttpUtility.JavaScriptStringEncode(item.FullName ?? "")).Append("\",");
                sb.Append("\"phone\":\"").Append(HttpUtility.JavaScriptStringEncode(item.Phone ?? "")).Append("\",");
                sb.Append("\"email\":\"").Append(HttpUtility.JavaScriptStringEncode(item.Email ?? "")).Append("\"");
                sb.Append("}");
            }
        }
        sb.Append("]}");
        context.Response.Write(sb.ToString());
    }
}
