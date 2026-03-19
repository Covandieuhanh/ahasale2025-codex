<%@ WebHandler Language="C#" Class="Upload_Handler_Style1" %>

using System;
using System.Drawing;
using System.IO;
using System.Web;

public class Upload_Handler_Style1 : IHttpHandler
{
    private const int MaxImageSizeBytes = 20 * 1024 * 1024; // 20 MB
    private const int MaxVideoSizeBytes = 80 * 1024 * 1024; // 80 MB (video ngắn)

    private static string BuildRelativePath(string folder, string fileName)
    {
        return "/uploads/" + folder.Trim('/') + "/" + fileName;
    }

    private static string ToAbsolutePath(HttpContext context, string relativePath)
    {
        string root = context.Server.MapPath("~").TrimEnd('/', '\\');
        string rel = (relativePath ?? "").TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(root, rel);
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        try
        {
            if (context.Request.Files.Count <= 0)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Vui lòng chọn file.");
                return;
            }

            HttpPostedFile file = context.Request.Files[0];
            string ext = (Path.GetExtension(file.FileName) ?? "").Trim().ToLowerInvariant();
            bool isImage = MediaFile_cl.IsImageExtension(ext);
            bool isVideo = MediaFile_cl.IsVideoExtension(ext);

            if (!isImage && !isVideo)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Định dạng không hợp lệ. Hỗ trợ ảnh và video ngắn.");
                return;
            }

            int maxSize = isVideo ? MaxVideoSizeBytes : MaxImageSizeBytes;
            if (file.ContentLength > maxSize)
            {
                context.Response.StatusCode = 400;
                context.Response.Write(isVideo
                    ? "Video quá lớn. Vui lòng chọn video nhỏ hơn 80 MB."
                    : "Ảnh quá lớn. Vui lòng chọn ảnh nhỏ hơn 20 MB.");
                return;
            }

            string tempFolder = isVideo ? "media-temp" : "img-temp";
            string outputFolder = isVideo ? "media-handler" : "img-handler";
            string tempName = Guid.NewGuid().ToString("N") + ext;
            string outputName = Guid.NewGuid().ToString("N") + ext;

            string tempRelative = BuildRelativePath(tempFolder, tempName);
            string outputRelative = BuildRelativePath(outputFolder, outputName);
            string tempPath = ToAbsolutePath(context, tempRelative);
            string outputPath = ToAbsolutePath(context, outputRelative);

            string tempDir = Path.GetDirectoryName(tempPath);
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            file.SaveAs(tempPath);

            if (isVideo)
            {
                File.Copy(tempPath, outputPath, true);
            }
            else
            {
                bool saved = false;
                if (MediaFile_cl.IsProcessableImageExtension(ext))
                {
                    try
                    {
                        using (Image image = Image.FromFile(tempPath))
                        {
                            if (image.Width > 600)
                                xulyanh_cl.ProcessAndSaveImage(tempPath, outputPath, 600, 100);
                            else
                                File.Copy(tempPath, outputPath, true);
                            saved = true;
                        }
                    }
                    catch
                    {
                        saved = false;
                    }
                }

                if (!saved)
                    File.Copy(tempPath, outputPath, true);
            }

            if (File.Exists(tempPath))
                File.Delete(tempPath);

            context.Response.StatusCode = 200;
            context.Response.Write(outputRelative);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write("Lỗi upload: " + ex.Message);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
