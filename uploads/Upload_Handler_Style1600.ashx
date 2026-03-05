<%@ WebHandler Language="C#" Class="Upload_Handler_Style1600" %>

using System;
using System.Web;
using System.IO;
using System.Drawing;

public class Upload_Handler_Style1600 : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];

                // Kiểm tra loại tệp
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic" };
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("Định dạng ảnh không hợp lệ.");
                    return;
                }

                // Kiểm tra kích thước tệp
                int maxFileSize = 10 * 1024 * 1024; // 10 MB
                if (file.ContentLength > maxFileSize)
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("Vui lòng chọn file có kích thước nhỏ hơn 10 MB.");
                    return;
                }

                string tempFileName = "/uploads/img-temp/" + Guid.NewGuid() + fileExtension;
                string tempFilePath = context.Server.MapPath("~") + tempFileName;

                file.SaveAs(tempFilePath);

                string newFileName = "/uploads/img-handler/" + Guid.NewGuid() + fileExtension;
                string newFilePath = context.Server.MapPath("~") + newFileName;

                // Kiểm tra chiều rộng của hình ảnh
                using (Image image = Image.FromFile(tempFilePath))
                {
                    if (image.Width > 1600)
                    {
                        xulyanh_cl.ProcessAndSaveImage(tempFilePath, newFilePath, 1600, 100);
                    }
                    else
                    {
                        // Nếu chiều rộng nhỏ hơn 600, chỉ cần sao chép file mà không xử lý
                        File.Copy(tempFilePath, newFilePath);
                    }
                }

                File.Delete(tempFilePath);

                context.Response.StatusCode = 200;
                context.Response.Write(newFileName);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Lỗi upload.");
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write("Error uploading file: " + ex.Message);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }

}