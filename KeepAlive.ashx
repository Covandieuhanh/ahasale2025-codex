<%@ WebHandler Language="C#" Class="KeepAliveHandler" %>

using System;
using System.Web;

public class KeepAliveHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            LinkedFeedSync_cl.EnsureAutoSchedulerStarted();
            LinkedFeedSync_cl.TriggerAutoSyncInBackground();
        }
        catch { }
        context.Response.StatusCode = 204;
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();
        context.Response.ContentType = "text/plain";
    }

    public bool IsReusable { get { return true; } }
}
