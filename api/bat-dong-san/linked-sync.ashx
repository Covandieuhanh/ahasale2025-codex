<%@ WebHandler Language="C#" Class="api_bat_dong_san_linked_sync" %>

using System;
using System.Configuration;
using System.Web;

public class api_bat_dong_san_linked_sync : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (!IsAuthorized(context))
        {
            context.Response.StatusCode = 403;
            context.Response.Write("{\"ok\":false,\"message\":\"forbidden\"}");
            return;
        }

        try
        {
            string source = (context.Request["source"] ?? "").Trim();
            bool force = string.Equals((context.Request["force"] ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase);
            string message;
            if (force)
                message = LinkedFeedSync_cl.RunManualSync(source);
            else
            {
                LinkedFeedSync_cl.RunAutoSyncIfDue();
                message = "auto-sync checked";
            }

            context.Response.Write("{\"ok\":true,\"message\":\"" + EscapeJson(message) + "\"}");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write("{\"ok\":false,\"message\":\"" + EscapeJson(ex.Message) + "\"}");
        }
    }

    public bool IsReusable { get { return false; } }

    private static bool IsAuthorized(HttpContext context)
    {
        string configured = ConfigurationManager.AppSettings["BDS.LinkedSyncKey"];
        string incoming = (context.Request["key"] ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(configured))
            return string.Equals(configured.Trim(), incoming, StringComparison.Ordinal);

        string host = "";
        try
        {
            host = (context.Request.Url == null ? "" : context.Request.Url.Host).ToLowerInvariant();
        }
        catch { }
        return host.Contains("localhost") || host.Contains("127.0.0.1");
    }

    private static string EscapeJson(string value)
    {
        return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
