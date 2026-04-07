<%@ WebHandler Language="C#" Class="GianHangInvoiceLookupHandler" %>

using System;
using System.Linq;
using System.Text;
using System.Web;

public class GianHangInvoiceLookupHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    private sealed class LookupItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CareUser { get; set; }
        public string GroupId { get; set; }
        public string PriceText { get; set; }
        public string SalePercentText { get; set; }
        public string PerformerPercentText { get; set; }
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

        string mode = ((context.Request.QueryString["mode"] ?? "") + "").Trim().ToLowerInvariant();
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
                LookupItem item = null;
                switch (mode)
                {
                    case "customer-phone":
                        item = LookupCustomerByPhone(db, branchId, keywordRaw);
                        break;
                    case "customer-name":
                        item = LookupCustomerByName(db, branchId, keywordRaw);
                        break;
                    case "service":
                        item = LookupPost(db, branchId, keywordRaw, "ctdv");
                        break;
                    case "product":
                        item = LookupPost(db, branchId, keywordRaw, "ctsp");
                        break;
                }

                WriteJson(context, 200, true, "", item);
            }
        }
        catch (Exception ex)
        {
            WriteJson(context, 500, false, "Lookup failed: " + ex.Message, null);
        }
    }

    public bool IsReusable { get { return false; } }

    private static LookupItem LookupCustomerByPhone(dbDataContext db, string branchId, string keywordRaw)
    {
        string phone = NormalizePhone(keywordRaw);
        var query = db.bspa_data_khachhang_tables
            .Where(p => p.id_chinhanh == branchId && (p.sdt == keywordRaw || p.sdt.Contains(keywordRaw)))
            .OrderByDescending(p => p.ngaytao)
            .Take(20)
            .ToList();

        bspa_data_khachhang_table match = query
            .FirstOrDefault(p => NormalizePhone(p.sdt) == phone)
            ?? query.FirstOrDefault(p => ((p.sdt ?? "") + "").Contains(keywordRaw));

        return ToLookupItem(match);
    }

    private static LookupItem LookupCustomerByName(dbDataContext db, string branchId, string keywordRaw)
    {
        string keyword = keywordRaw.ToLowerInvariant();
        var query = db.bspa_data_khachhang_tables
            .Where(p => p.id_chinhanh == branchId && p.tenkhachhang.Contains(keywordRaw))
            .OrderByDescending(p => p.ngaytao)
            .Take(20)
            .ToList();

        bspa_data_khachhang_table match = query
            .FirstOrDefault(p => string.Equals(((p.tenkhachhang ?? "") + "").Trim(), keywordRaw, StringComparison.OrdinalIgnoreCase))
            ?? query.FirstOrDefault(p => (((p.tenkhachhang ?? "") + "").ToLowerInvariant().Contains(keyword)));

        return ToLookupItem(match);
    }

    private static LookupItem LookupPost(dbDataContext db, string branchId, string keywordRaw, string postType)
    {
        string keyword = keywordRaw.ToLowerInvariant();
        var query = db.web_post_tables
            .Where(p => p.id_chinhanh == branchId && p.phanloai == postType && p.bin == false && p.name.Contains(keywordRaw))
            .OrderBy(p => p.name)
            .Take(20)
            .ToList();

        web_post_table match = query
            .FirstOrDefault(p => string.Equals(((p.name ?? "") + "").Trim(), keywordRaw, StringComparison.OrdinalIgnoreCase))
            ?? query.FirstOrDefault(p => (((p.name ?? "") + "").ToLowerInvariant().Contains(keyword)));

        if (match == null)
            return null;

        return new LookupItem
        {
            Id = match.id.ToString(),
            Name = ((match.name ?? "") + "").Trim(),
            PriceText = postType == "ctdv"
                ? ((match.giaban_dichvu ?? 0).ToString("#,##0"))
                : ((match.giaban_sanpham ?? 0).ToString("#,##0")),
            SalePercentText = postType == "ctdv"
                ? ((match.phantram_chotsale_dichvu ?? 0).ToString())
                : ((match.phantram_chotsale_sanpham ?? 0).ToString()),
            PerformerPercentText = postType == "ctdv"
                ? ((match.phantram_lamdichvu ?? 0).ToString())
                : ""
        };
    }

    private static LookupItem ToLookupItem(bspa_data_khachhang_table item)
    {
        if (item == null)
            return null;

        return new LookupItem
        {
            Id = item.id.ToString(),
            Name = ((item.tenkhachhang ?? "") + "").Trim(),
            Phone = ((item.sdt ?? "") + "").Trim(),
            Address = ((item.diachi ?? "") + "").Trim(),
            CareUser = ((item.nguoichamsoc ?? "") + "").Trim(),
            GroupId = ((item.nhomkhachhang ?? "") + "").Trim()
        };
    }

    private static string NormalizePhone(string phone)
    {
        return ((phone ?? "") + "")
            .Replace(" ", "")
            .Replace(".", "")
            .Replace("-", "")
            .Replace("+", "")
            .Trim();
    }

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
        sb.Append("\"phone\":\"").Append(HttpUtility.JavaScriptStringEncode(item.Phone ?? "")).Append("\",");
        sb.Append("\"address\":\"").Append(HttpUtility.JavaScriptStringEncode(item.Address ?? "")).Append("\",");
        sb.Append("\"careUser\":\"").Append(HttpUtility.JavaScriptStringEncode(item.CareUser ?? "")).Append("\",");
        sb.Append("\"groupId\":\"").Append(HttpUtility.JavaScriptStringEncode(item.GroupId ?? "")).Append("\",");
        sb.Append("\"priceText\":\"").Append(HttpUtility.JavaScriptStringEncode(item.PriceText ?? "")).Append("\",");
        sb.Append("\"salePercentText\":\"").Append(HttpUtility.JavaScriptStringEncode(item.SalePercentText ?? "")).Append("\",");
        sb.Append("\"performerPercentText\":\"").Append(HttpUtility.JavaScriptStringEncode(item.PerformerPercentText ?? "")).Append("\"}}");
        context.Response.Write(sb.ToString());
    }
}
