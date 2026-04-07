<%@ WebHandler Language="C#" Class="GianHangSupplyLookupHandler" %>

using System;
using System.Linq;
using System.Text;
using System.Web;

public class GianHangSupplyLookupHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    private sealed class LookupItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PriceText { get; set; }
        public string UnitText { get; set; }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = Encoding.UTF8;

        if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.AppendHeader("Allow", "GET");
            WriteJson(context, 405, false, "Method not allowed.", null);
            return;
        }

        if (!GianHangSystemAdminGuard_cl.EnsureHandlerAccess(context))
        {
            WriteJson(context, 403, false, "Bạn không có quyền truy cập endpoint này trong chế độ quản trị hệ thống.", null);
            return;
        }

        string currentUser = ((context.Session["user"] ?? "") + "").Trim();
        string branchId = ((context.Session["chinhanh"] ?? "") + "").Trim();
        if (currentUser == "" || branchId == "")
        {
            WriteJson(context, 401, false, "Phiên đăng nhập /gianhang/admin đã hết.", null);
            return;
        }

        string keywordRaw = ((context.Request.QueryString["q"] ?? "") + "").Trim();
        if (keywordRaw == "")
        {
            WriteJson(context, 200, true, "", null);
            return;
        }

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                string keyword = keywordRaw.ToLowerInvariant();
                var query = db.danhsach_vattu_tables
                    .Where(p => p.id_chinhanh == branchId && p.tenvattu.Contains(keywordRaw))
                    .OrderBy(p => p.tenvattu)
                    .Take(20)
                    .ToList();

                danhsach_vattu_table match = query
                    .FirstOrDefault(p => string.Equals(((p.tenvattu ?? "") + "").Trim(), keywordRaw, StringComparison.OrdinalIgnoreCase))
                    ?? query.FirstOrDefault(p => (((p.tenvattu ?? "") + "").ToLowerInvariant().Contains(keyword)));

                LookupItem item = match == null
                    ? null
                    : new LookupItem
                    {
                        Id = match.id.ToString(),
                        Name = ((match.tenvattu ?? "") + "").Trim(),
                        PriceText = ((match.gianhap ?? 0).ToString("#,##0")),
                        UnitText = ((match.donvitinh_sp ?? "") + "").Trim()
                    };

                WriteJson(context, 200, true, "", item);
            }
        }
        catch (Exception ex)
        {
            WriteJson(context, 500, false, "Lookup failed: " + ex.Message, null);
        }
    }

    public bool IsReusable { get { return false; } }

    private static void WriteJson(HttpContext context, int statusCode, bool ok, string message, LookupItem item)
    {
        context.Response.StatusCode = statusCode;
        StringBuilder sb = new StringBuilder();
        sb.Append("{\"ok\":").Append(ok ? "true" : "false");
        sb.Append(",\"message\":\"").Append(HttpUtility.JavaScriptStringEncode(message ?? "")).Append("\"");
        if (item == null)
        {
            sb.Append(",\"item\":null}");
            context.Response.Write(sb.ToString());
            return;
        }

        sb.Append(",\"item\":{");
        sb.Append("\"id\":\"").Append(HttpUtility.JavaScriptStringEncode(item.Id ?? "")).Append("\",");
        sb.Append("\"name\":\"").Append(HttpUtility.JavaScriptStringEncode(item.Name ?? "")).Append("\",");
        sb.Append("\"priceText\":\"").Append(HttpUtility.JavaScriptStringEncode(item.PriceText ?? "")).Append("\",");
        sb.Append("\"unitText\":\"").Append(HttpUtility.JavaScriptStringEncode(item.UnitText ?? "")).Append("\"}}");
        context.Response.Write(sb.ToString());
    }
}
