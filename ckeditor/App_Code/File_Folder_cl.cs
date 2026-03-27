using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for File_Folder_cl
/// </summary>
public class File_Folder_cl
{
    public static void del_file(string _link_file)
    {
        // Lấy đường dẫn tuyệt đối của tệp
        string absolutePath = HttpContext.Current.Server.MapPath("~" + _link_file);

        // Kiểm tra xem tệp có tồn tại không
        if (File.Exists(absolutePath))
        {
            try
            {
                // Xóa tệp
                File.Delete(absolutePath);
                // Ghi nhật ký (nếu cần) để biết tệp đã được xóa thành công
                // Ví dụ: Logger.Log($"File deleted: {absolutePath}");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ khi xóa tệp
                // Ví dụ: Logger.Log($"Error deleting file: {absolutePath}. Exception: {ex}");
            }
        }
        else
        {
            // Ghi nhật ký (nếu cần) để biết tệp không tồn tại
            // Ví dụ: Logger.Log($"File not found: {absolutePath}");
        }
    }
    //hàm sau kiểm tra xem đường dẫn của 1 file có tồn tại trên máy chủ không, bât kỳ máy chủ nào, chỉ cần truyền url
    private bool Check_File_All_Server(string url)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        catch
        {
            return false;
        }
    }

}