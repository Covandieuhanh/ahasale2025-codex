using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using static OfficeOpenXml.ExcelErrorValue;
using System.IO;
using System.Collections.Generic;
using System.Web;
public class guiEmail_cl
    // THÊM ĐOẠN COADE SAU VÀO WEB CONFIG ĐỂ BẢO MẬT
//    <configuration>
//  < appSettings >
//    < add key = "SmtpServer" value = "host.emailserver.vn" />
//    < add key = "SmtpPort" value = "587" />
//    < add key = "EmailPassword" value = "9qCdpF6w58SqR6Dt" />
//  </ appSettings >
//</ configuration >
{
    // Tạo đối tượng Random tĩnh để sử dụng lại, tránh việc tạo lại nhiều lần
    private static readonly Random Random = new Random();

    public static void SendEmail(string toAddress, string subject, string body, string senderName, string attachmentPath = "")
    {
        string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
        int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        string password = ConfigurationManager.AppSettings["EmailPassword"];

        //int randomNumber = Random.Next(1, 21);
        string fromAddress = $"hotro@ahasale.vn";

        try
        {
            using (SmtpClient smtpClient = new SmtpClient(smtpServer, port))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(fromAddress, password);

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(fromAddress, senderName);
                    mailMessage.To.Add(toAddress);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    // Đặt mã hóa cho nội dung và tiêu đề
                    mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

                    // Thêm thông tin chân trang
                    string footer = "<div style='color:red'>Lưu ý: Đây là email được gửi tự động từ hệ thống. Vui lòng không phản hồi mail này.</div>";
                    mailMessage.Body += footer;

                    if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                    {
                        using (Attachment attachment = new Attachment(attachmentPath))
                        {
                            mailMessage.Attachments.Add(attachment);
                            smtpClient.Send(mailMessage);
                        }
                    }
                    else
                    {
                        smtpClient.Send(mailMessage);
                    }
                }
            }
        }
        catch (Exception _ex)
        {
            string _tk = HttpContext.Current.Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
}


//SỬ DỤNG 
//string _tieude = "quyết định bổ nhiệm";
//string _noidung = "Từ ngày 01/08/2024 bổ nhiệm ông Ngô Quang Bôn làm GĐ Marketing";
//string _ten_nguoigui = Request.Url.Host.ToUpper();
//string _link_dinhkem = "";

//List<string> emailAddresses = new List<string>
//        {
//            "bcorn.net@gmail.com",
//            "mkt.tcsmoitruong@gmail.com"
//        };
////ADD EMAIL
////List<string> emailAddresses = new List<string>();
////// Lấy các địa chỉ email từ các TextBox và thêm vào danh sách
////if (!string.IsNullOrEmpty(txtEmail1.Text))
////{
////    emailAddresses.Add(txtEmail1.Text);
////}

//foreach (var _email_nhan in emailAddresses)
//{
//    try
//    {
//        guiEmail_cl.SendEmail(_email_nhan, _tieude, _noidung, _ten_nguoigui, _link_dinhkem);
//        Label1.Text += $"Email sent successfully to {_email_nhan}!<br />";
//    }
//    catch (Exception ex)
//    {
//        Label1.Text += $"Error sending email to {_email_nhan}: {ex.Message}<br />";
//    }
//}