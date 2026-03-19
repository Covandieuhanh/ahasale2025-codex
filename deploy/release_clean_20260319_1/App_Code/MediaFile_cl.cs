using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public static class MediaFile_cl
{
    private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".jpe", ".png", ".gif", ".webp", ".svg", ".heic", ".heif",
        ".bmp", ".tif", ".tiff", ".avif", ".jfif"
    };

    private static readonly HashSet<string> VideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".mov", ".webm", ".m4v", ".ogv", ".3gp", ".avi", ".mkv"
    };

    private static readonly HashSet<string> ImageProcessExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".jpe", ".png", ".gif", ".bmp", ".tif", ".tiff"
    };

    public static string GetExtension(string mediaUrl)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl))
            return "";

        string input = mediaUrl.Trim();
        Uri uri;
        if (Uri.TryCreate(input, UriKind.Absolute, out uri))
            input = uri.AbsolutePath;

        int q = input.IndexOf('?');
        if (q >= 0)
            input = input.Substring(0, q);

        string ext = Path.GetExtension(input) ?? "";
        return ext.Trim().ToLowerInvariant();
    }

    public static bool IsImage(string mediaUrl)
    {
        return ImageExtensions.Contains(GetExtension(mediaUrl));
    }

    public static bool IsVideo(string mediaUrl)
    {
        return VideoExtensions.Contains(GetExtension(mediaUrl));
    }

    public static bool IsSupportedMedia(string mediaUrl)
    {
        return IsImage(mediaUrl) || IsVideo(mediaUrl);
    }

    public static bool IsProcessableImage(string mediaUrl)
    {
        return ImageProcessExtensions.Contains(GetExtension(mediaUrl));
    }

    public static bool IsProcessableImageExtension(string ext)
    {
        return ImageProcessExtensions.Contains((ext ?? "").Trim().ToLowerInvariant());
    }

    public static bool IsImageExtension(string ext)
    {
        return ImageExtensions.Contains((ext ?? "").Trim().ToLowerInvariant());
    }

    public static bool IsVideoExtension(string ext)
    {
        return VideoExtensions.Contains((ext ?? "").Trim().ToLowerInvariant());
    }

    public static string GetSafeUrl(string url)
    {
        return HttpUtility.HtmlAttributeEncode((url ?? "").Trim());
    }

    public static string GetSafeText(string text)
    {
        return HttpUtility.HtmlEncode((text ?? "").Trim());
    }

    public static string GetVideoMime(string mediaUrlOrExt)
    {
        string ext = mediaUrlOrExt;
        if (!string.IsNullOrEmpty(ext) && ext.Contains("/"))
            ext = GetExtension(mediaUrlOrExt);
        else if (!string.IsNullOrEmpty(ext) && ext.StartsWith(".", StringComparison.Ordinal))
            ext = ext.ToLowerInvariant();
        else
            ext = GetExtension(mediaUrlOrExt);

        switch (ext)
        {
            case ".webm": return "video/webm";
            case ".ogv": return "video/ogg";
            case ".mov": return "video/quicktime";
            case ".m4v": return "video/x-m4v";
            case ".3gp": return "video/3gpp";
            case ".avi": return "video/x-msvideo";
            case ".mkv": return "video/x-matroska";
            default: return "video/mp4";
        }
    }
}
