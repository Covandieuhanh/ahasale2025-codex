<%@ WebHandler Language="C#" Class="bat_dong_san_linked_image" %>

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

public class bat_dong_san_linked_image : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        int linkedId = ParseInt(context.Request["id"]);
        int index = ParseInt(context.Request["index"]);

        if (linkedId <= 0)
        {
            WriteFallback(context);
            return;
        }

        LinkedFeedStore_cl.LinkedPost row = null;
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                row = LinkedFeedStore_cl.GetById(db, linkedId);
            }
        }
        catch
        {
            row = null;
        }

        if (row == null)
        {
            WriteFallback(context);
            return;
        }

        var candidates = BatDongSanService_cl.ResolveLinkedRawGalleryUrls(row.ThumbnailUrl, row.GalleryCsv);
        if (candidates.Count == 0)
        {
            WriteFallback(context);
            return;
        }

        int safeIndex = index < 0 ? 0 : index;
        if (safeIndex >= candidates.Count)
            safeIndex = 0;

        for (int i = 0; i < candidates.Count; i++)
        {
            int current = (safeIndex + i) % candidates.Count;
            if (TryWriteCandidate(context, candidates[current], row.SourceUrl))
                return;
        }

        WriteFallback(context);
    }

    public bool IsReusable
    {
        get { return false; }
    }

    private static bool TryWriteCandidate(HttpContext context, string candidate, string sourceUrl)
    {
        string url = (candidate ?? "").Trim();
        if (url == "")
            return false;

        if (url.StartsWith("/"))
        {
            try
            {
                string localPath = context.Server.MapPath(url);
                if (!File.Exists(localPath))
                    return false;

                byte[] bytes = File.ReadAllBytes(localPath);
                if (bytes == null || bytes.Length == 0)
                    return false;

                context.Response.Clear();
                context.Response.ContentType = ResolveContentType(localPath);
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetMaxAge(TimeSpan.FromHours(12));
                context.Response.BinaryWrite(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36 AhaSaleImageProxy/1.0");
                    client.Headers.Add("Accept", "image/avif,image/webp,image/apng,image/*,*/*;q=0.8");

                    Uri sourceUri;
                    if (Uri.TryCreate((sourceUrl ?? "").Trim(), UriKind.Absolute, out sourceUri))
                        client.Headers.Add("Referer", sourceUri.Scheme + "://" + sourceUri.Host + "/");

                    byte[] bytes = client.DownloadData(url);
                    if (bytes == null || bytes.Length < 2048)
                        return false;

                    context.Response.Clear();
                    context.Response.ContentType = ResolveContentType(url);
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetMaxAge(TimeSpan.FromHours(2));
                    context.Response.BinaryWrite(bytes);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    private static void WriteFallback(HttpContext context)
    {
        try
        {
            string localPath = context.Server.MapPath(BatDongSanService_cl.DefaultBdsFallbackImage);
            if (File.Exists(localPath))
            {
                context.Response.Clear();
                context.Response.ContentType = ResolveContentType(localPath);
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetMaxAge(TimeSpan.FromHours(12));
                context.Response.BinaryWrite(File.ReadAllBytes(localPath));
                return;
            }
        }
        catch
        {
        }

        context.Response.StatusCode = 404;
        context.Response.End();
    }

    private static int ParseInt(string raw)
    {
        int value;
        return int.TryParse((raw ?? "").Trim(), out value) ? value : 0;
    }

    private static string ResolveContentType(string path)
    {
        string ext = Path.GetExtension(path ?? "").ToLowerInvariant();
        switch (ext)
        {
            case ".png": return "image/png";
            case ".webp": return "image/webp";
            case ".gif": return "image/gif";
            case ".svg": return "image/svg+xml";
            default: return "image/jpeg";
        }
    }
}
