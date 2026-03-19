<%@ WebHandler Language="C#" Class="Upload_Handler_TimelineJpg" %>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

public class Upload_Handler_TimelineJpg : IHttpHandler
{
    private const int MaxImageSizeBytes = 20 * 1024 * 1024; // 20 MB
    private const int MaxWidth = 1600;
    private const long JpegQuality = 85L;

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
                context.Response.Write("Vui lòng chọn file ảnh.");
                return;
            }

            HttpPostedFile file = context.Request.Files[0];
            string ext = (Path.GetExtension(file.FileName) ?? "").Trim().ToLowerInvariant();
            bool isImageExt = MediaFile_cl.IsImageExtension(ext);
            bool isImageContent = (file.ContentType ?? "").StartsWith("image/", StringComparison.OrdinalIgnoreCase);

            if (!isImageExt && !isImageContent)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Định dạng không hợp lệ. Vui lòng chọn file ảnh.");
                return;
            }

            if (file.ContentLength > MaxImageSizeBytes)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Ảnh quá lớn. Vui lòng chọn ảnh nhỏ hơn 20 MB.");
                return;
            }

            string tempFolder = "img-temp";
            string outputFolder = "img-timeline";
            string tempName = Guid.NewGuid().ToString("N") + (string.IsNullOrEmpty(ext) ? ".tmp" : ext);
            string outputName = Guid.NewGuid().ToString("N") + ".jpg";

            string tempRelative = BuildRelativePath(tempFolder, tempName);
            string outputRelative = BuildRelativePath(outputFolder, outputName);
            string tempPath = ToAbsolutePath(context, tempRelative);
            string outputPath = ToAbsolutePath(context, outputRelative);

            string tempDir = Path.GetDirectoryName(tempPath);
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            file.SaveAs(tempPath);

            ConvertToJpeg(tempPath, outputPath);

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

    private static void ConvertToJpeg(string sourcePath, string destinationPath)
    {
        using (Image image = Image.FromFile(sourcePath))
        {
            int width = image.Width;
            int height = image.Height;
            if (width > MaxWidth && width > 0)
            {
                int newHeight = (int)Math.Round(height * (MaxWidth / (double)width));
                using (Bitmap resized = new Bitmap(image, new Size(MaxWidth, newHeight)))
                {
                    SaveJpeg(resized, destinationPath);
                }
            }
            else
            {
                SaveJpeg(image, destinationPath);
            }
        }
    }

    private static void SaveJpeg(Image image, string destinationPath)
    {
        ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders()
            .FirstOrDefault(c => c.MimeType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase));

        if (jpegCodec == null)
        {
            image.Save(destinationPath, ImageFormat.Jpeg);
            return;
        }

        using (var encoderParams = new EncoderParameters(1))
        {
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, JpegQuality);
            image.Save(destinationPath, jpegCodec, encoderParams);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
