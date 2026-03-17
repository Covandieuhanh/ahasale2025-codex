using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;


public class sendmail_class
{

    public static void sendmail(string _smtp, int _port, string _titlemail, string _content, string _emailnhan, string _tennguoigui, string _att)
    {
        //vào link sau bật cho phép ứng dụng kém an toàn
        //https://myaccount.google.com/u/1/lesssecureapps?pageId=none

        _smtp = "host07.emailserver.vn";
        _port = 587;//"587 465"
        string _emailsend = "auto@ahashine.vn";
        string _pass = "AkaHalora!!";

       

        SmtpClient smtpClient = new SmtpClient(_smtp, _port);
        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new System.Net.NetworkCredential(_emailsend, _pass);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        MailMessage mailMessage = new MailMessage(_emailsend, _emailnhan);
        mailMessage.Subject = _titlemail;
        mailMessage.Body = _content;
        mailMessage.From = new MailAddress(_emailsend, _tennguoigui);
        mailMessage.IsBodyHtml = true;

        if (_att != "")
        {
            //đính kèm
            Attachment att = new Attachment(System.Web.HttpContext.Current.Server.MapPath("~" + _att));
            att.ContentDisposition.Inline = true;
            mailMessage.Attachments.Add(att);
        }

        smtpClient.Send(mailMessage);
        //smtpClient.Send(mailMessage1);
    }
}






