using System;
using System.Net;
using System.Net.Mail;

public static class EmailOtp_cl
{
    public static bool SendOtp(string email, string otp, string taiKhoan, out string error, out bool usedFallback)
    {
        error = "";
        usedFallback = false;

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                EmailOtpConfig cfg = OtpConfig_cl.GetEmailConfig(db);
                if (cfg == null || string.IsNullOrWhiteSpace(cfg.Host) || cfg.Port <= 0)
                {
                    error = "Chưa cấu hình Email OTP.";
                    return false;
                }

                if (cfg.DevMode)
                {
                    usedFallback = true;
                    Log_cl.Add_Log("[EMAIL OTP DEV] " + (email ?? "") + " - OTP " + otp, email ?? "", "");
                    return true;
                }

                string subject = (cfg.SubjectTemplate ?? "").Trim();
                if (string.IsNullOrEmpty(subject))
                    subject = "[AhaSale] Mã OTP của bạn";

                string body = (cfg.BodyTemplate ?? "").Trim();
                if (string.IsNullOrEmpty(body))
                    body = "Ma OTP cua ban la: {OTP}. Het han sau {EXPIRE} phut.";

                body = body.Replace("{OTP}", otp ?? "")
                           .Replace("{Account}", taiKhoan ?? "")
                           .Replace("{EXPIRE}", ShopOtp_cl.ExpireMinutes.ToString());

                MailMessage message = new MailMessage();
                message.To.Add(email);
                string fromAddress = string.IsNullOrWhiteSpace(cfg.FromEmail) ? "" : cfg.FromEmail.Trim();
                if (string.IsNullOrWhiteSpace(fromAddress))
                    fromAddress = "hotro@ahasale.vn";
                message.From = new MailAddress(fromAddress, cfg.FromName ?? "AhaSale");
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient(cfg.Host, cfg.Port))
                {
                    client.EnableSsl = cfg.UseSsl;
                    string smtpUser = string.IsNullOrWhiteSpace(cfg.Username) ? "" : cfg.Username.Trim();
                    if (string.IsNullOrWhiteSpace(smtpUser))
                        smtpUser = fromAddress;
                    if (!string.IsNullOrWhiteSpace(smtpUser))
                    {
                        client.Credentials = new NetworkCredential(smtpUser, cfg.Password ?? "");
                    }
                    client.Send(message);
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            Log_cl.Add_Log("[EMAIL OTP] " + ex.Message, email ?? "", ex.StackTrace);
            return false;
        }
    }
}
