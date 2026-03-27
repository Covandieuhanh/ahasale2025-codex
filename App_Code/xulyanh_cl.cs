using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

public class xulyanh_cl
{
    public static bool IsImageFile(string fileExtension)
    {
        // Kiểm tra xem phần mở rộng của tệp tin có phải là của hình ảnh hay không
        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".heic" }; // Thêm các định dạng hình ảnh khác nếu cần
        return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    public static void ProcessAndSaveImage(string sourcePath, string destinationPath, int newWidth, int quality)
    {
        try
        {
            using (Bitmap original = new Bitmap(sourcePath))
            {
                int newHeight = (int)((double)original.Height / original.Width * newWidth);
                ImageFormat outputFormat = GetImageFormatFromFileName(destinationPath);
                bool keepTransparency = SupportsTransparency(outputFormat);
                PixelFormat pixelFormat = keepTransparency ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;

                using (Bitmap resized = new Bitmap(newWidth, newHeight, pixelFormat))
                {
                    using (Graphics graphics = Graphics.FromImage(resized))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.Clear(keepTransparency ? Color.Transparent : Color.White);

                        using (ImageAttributes imageAttributes = new ImageAttributes())
                        {
                            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                            graphics.DrawImage(
                                original,
                                new Rectangle(0, 0, newWidth, newHeight),
                                0,
                                0,
                                original.Width,
                                original.Height,
                                GraphicsUnit.Pixel,
                                imageAttributes);
                        }
                    }

                    // Kiểm tra xem đường dẫn có hợp lệ và có quyền ghi không
                    if (IsValidPath(destinationPath))
                    {
                        // Lưu ảnh đã thay đổi kích thước dưới định dạng được xác định với chất lượng
                        SaveImageWithQuality(resized, destinationPath, outputFormat, quality);
                    }
                    else
                    {
                        // Xử lý lỗi đường dẫn không hợp lệ hoặc không có quyền ghi
                        Console.WriteLine("Đường dẫn không hợp lệ hoặc không có quyền ghi.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Xử lý và ghi log lỗi
            Console.WriteLine("Lỗi: " + ex.Message);
        }
    }

    private static void SaveImageWithQuality(Bitmap image, string destinationPath, ImageFormat format, int quality)
    {
        EncoderParameters encoderParameters = null;
        ImageCodecInfo encoder = GetEncoder(format);

        // Nếu định dạng là JPEG, sử dụng EncoderParameters để đặt chất lượng
        if (format == ImageFormat.Jpeg)
        {
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
        }

        if (encoder != null && encoderParameters != null)
        {
            image.Save(destinationPath, encoder, encoderParameters);
            return;
        }

        image.Save(destinationPath, format);
    }

    private static ImageFormat GetImageFormatFromFileName(string fileName)
    {
        string extension = (Path.GetExtension(fileName) ?? string.Empty).ToLower();

        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                return ImageFormat.Jpeg;

            case ".png":
                return ImageFormat.Png;

            // Thêm các định dạng khác nếu cần

            default:
                // Mặc định là JPEG nếu không xác định được định dạng
                return ImageFormat.Jpeg;
        }
    }

    private static bool IsValidPath(string path)
    {
        // Kiểm tra xem đường dẫn có hợp lệ và có quyền ghi không
        try
        {
            using (FileStream fs = File.OpenWrite(path))
            {
                fs.Close();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    // Hàm này trả về thông tin về encoder cho định dạng hình ảnh được chỉ định
    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        // Lấy danh sách tất cả các encoder có sẵn trong hệ thống
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

        // Duyệt qua từng encoder để tìm encoder cho định dạng hình ảnh được chỉ định
        foreach (ImageCodecInfo codec in codecs)
        {
            // Nếu tìm thấy encoder cho định dạng hình ảnh, trả về thông tin của nó
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }

        // Nếu không tìm thấy encoder cho định dạng hình ảnh, trả về null
        return null;
    }

    private static bool SupportsTransparency(ImageFormat format)
    {
        return format.Guid == ImageFormat.Png.Guid
            || format.Guid == ImageFormat.Gif.Guid
            || format.Guid == ImageFormat.Tiff.Guid;
    }
}
