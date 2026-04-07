using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Configuration;
using System.Net;
using System.Net.Mail;


public class Helper_cl
{
    private static readonly object RuntimeCacheSync = new object();

    //Helper_Tabler_cl.ShowToast(this.Page, "Tài khoản không tồn tại.", "danger", true, 3000, "Lỗi");
    //Helper_Tabler_cl.ShowModal(this.Page,"Sai mật khẩu hoặc tài khoản.","Lỗi",true,"danger");
    //ctx.Session["ThongBao_Admin"] ="toast|Thông báo|Mật khẩu đã được thay đổi. Vui lòng đăng nhập lại.|warning|4000";
    //ctx.Session["ThongBao_Admin"] ="modal|Thông báo|Tài khoản đã bị khóa.|danger|0";
    //Helper_Tabler_cl.ShowThongBaoSession(this);//hiện thông báo nếu có

    #region hàm tạo phiên bản tự động cho link css và js, chống cache - OK
    private static string NormalizeStaticAssetPath(string virtualPath)
    {
        if (string.IsNullOrWhiteSpace(virtualPath))
            return virtualPath;

        string path = virtualPath.Trim();
        const string cssRoot1 = "~/css/";
        const string cssRoot2 = "~/Css/";
        const string cssRoot3 = "/css/";
        const string cssRoot4 = "/Css/";

        if (path.StartsWith(cssRoot1, StringComparison.OrdinalIgnoreCase))
            return "~/assetscss/" + path.Substring(cssRoot1.Length);
        if (path.StartsWith(cssRoot2, StringComparison.OrdinalIgnoreCase))
            return "~/assetscss/" + path.Substring(cssRoot2.Length);
        if (path.StartsWith(cssRoot3, StringComparison.OrdinalIgnoreCase))
            return "~/assetscss/" + path.Substring(cssRoot3.Length);
        if (path.StartsWith(cssRoot4, StringComparison.OrdinalIgnoreCase))
            return "~/assetscss/" + path.Substring(cssRoot4.Length);

        return path;
    }

    // Root version lấy từ appSettings[AssetVersion]; nếu không có, dùng timestamp Web.config để tự động đổi mỗi lần deploy.
    private static readonly string RootVersion = InitRootVersion();

    private static string InitRootVersion()
    {
        try
        {
            var fromConfig = ConfigurationManager.AppSettings["AssetVersion"];
            if (!string.IsNullOrEmpty(fromConfig))
                return fromConfig.Trim();

            // Nếu không set AssetVersion, tạo phiên bản ngẫu nhiên theo mỗi lần app khởi động (app pool recycle/deploy).
            return Guid.NewGuid().ToString("N");
        }
        catch { }

        return "0";
    }

    public static string VersionedUrl(string virtualPath)
    {
        string preferredPath = NormalizeStaticAssetPath(virtualPath);
        string customVersion = RootVersion;

        try
        {
            string physicalPath = HttpContext.Current.Server.MapPath(preferredPath);
            if (File.Exists(physicalPath))
            {
                long ticks = File.GetLastWriteTime(physicalPath).Ticks;
                string version = string.IsNullOrEmpty(customVersion) ? ticks.ToString() : (ticks + "-" + customVersion);
                return VirtualPathUtility.ToAbsolute(preferredPath) + "?v=" + version;
            }

            // fallback: giữ tương thích nếu đường dẫn cũ còn tồn tại.
            if (!string.Equals(preferredPath, virtualPath, StringComparison.OrdinalIgnoreCase))
            {
                string oldPhysicalPath = HttpContext.Current.Server.MapPath(virtualPath);
                if (File.Exists(oldPhysicalPath))
                {
                    long oldTicks = File.GetLastWriteTime(oldPhysicalPath).Ticks;
                    string oldVersion = string.IsNullOrEmpty(customVersion) ? oldTicks.ToString() : (oldTicks + "-" + customVersion);
                    return VirtualPathUtility.ToAbsolute(virtualPath) + "?v=" + oldVersion;
                }
            }
        }
        catch { }

        // fallback nếu file ko tồn tại
        string fallbackVersion = string.IsNullOrEmpty(customVersion) ? "0" : ("0-" + customVersion);
        return VirtualPathUtility.ToAbsolute(preferredPath) + "?v=" + fallbackVersion;
    }
    #endregion

    #region runtime cache
    public static T RuntimeCacheGet<T>(string key) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        return HttpRuntime.Cache[key] as T;
    }

    public static T RuntimeCacheGetOrAdd<T>(string key, int durationSeconds, Func<T> factory) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key is required.", "key");
        if (factory == null)
            throw new ArgumentNullException("factory");

        T cached = HttpRuntime.Cache[key] as T;
        if (cached != null)
            return cached;

        lock (RuntimeCacheSync)
        {
            cached = HttpRuntime.Cache[key] as T;
            if (cached != null)
                return cached;

            T created = factory();
            if (created == null)
                return null;

            if (durationSeconds < 1)
                durationSeconds = 1;

            HttpRuntime.Cache.Insert(
                key,
                created,
                null,
                DateTime.UtcNow.AddSeconds(durationSeconds),
                Cache.NoSlidingExpiration);

            return created;
        }
    }

    public static void RuntimeCacheRemove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        HttpRuntime.Cache.Remove(key);
    }
    #endregion

    #region upload image path guard
    public static bool IsMissingUploadFile(string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
            return false;

        string cleanPath = relativeUrl.Trim();
        if (cleanPath.Length == 0)
            return false;

        int q = cleanPath.IndexOf('?');
        if (q >= 0)
            cleanPath = cleanPath.Substring(0, q);

        int h = cleanPath.IndexOf('#');
        if (h >= 0)
            cleanPath = cleanPath.Substring(0, h);

        if (!cleanPath.StartsWith("/", StringComparison.Ordinal))
            cleanPath = "/" + cleanPath.TrimStart('/');

        if (!cleanPath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return false;
        // Runtime local/host có thể map path khác nhau (Docker/IIS/Mono),
        // nên không chặn ảnh theo File.Exists để tránh fallback sai.
        return false;
    }
    #endregion
    #region hàm Check quyền, dùng chung - OK
    public static bool check_quyen(string _quyen, params string[] list_quyen_kiemtra)
    {
        if (string.IsNullOrEmpty(_quyen))
            return false;

        // Tách chuỗi quyền thành danh sách
        var quyenList = _quyen.Split(',').Select(q => q.Trim()).ToList();

        // Duyệt qua các quyền cần kiểm tra
        foreach (var q in list_quyen_kiemtra)
        {
            if (quyenList.Contains(q))
                return true; // Có ít nhất 1 quyền hợp lệ
        }

        return false; // Không có quyền nào khớp
    }
    #endregion
    #region Hàm mã hóa và giải mã - OK
    //private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("YourEncryptionKey"); // Khóa mã hóa, hãy đảm bảo nó là một giá trị bí mật và an toàn

    private static readonly byte[] EncryptionKey = new byte[]// Khóa mã hóa, hãy đảm bảo nó là một giá trị bí mật và an toàn
{
    0x01, 0x45, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
};

    public static string mahoa_Hota(string plainText)
    {
        if (plainText == "")
            return "";
        else
        {
            byte[] encryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.Mode = CipherMode.CBC;
                // Tạo một vector khởi tạo ngẫu nhiên
                aes.GenerateIV();
                byte[] iv = aes.IV;
                // Mã hóa dữ liệu
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }
                // Kết hợp vector khởi tạo và dữ liệu mã hóa thành một chuỗi Base64
                byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
                Array.Copy(iv, 0, combinedBytes, 0, iv.Length);
                Array.Copy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);
                return Convert.ToBase64String(combinedBytes);
            }
        }
    }
    public static string giaima_Hota(string encryptedText)
    {
        try
        {
            if (encryptedText == "")
                return "";
            else
            {
                byte[] combinedBytes = Convert.FromBase64String(encryptedText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = EncryptionKey;
                    aes.Mode = CipherMode.CBC;

                    // Tách vector khởi tạo và dữ liệu mã hóa từ chuỗi Base64
                    byte[] iv = new byte[aes.BlockSize / 8];
                    byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];
                    Array.Copy(combinedBytes, 0, iv, 0, iv.Length);
                    Array.Copy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

                    // Giải mã dữ liệu
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);
                    using (MemoryStream ms = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {

                                return sr.ReadToEnd();

                            }
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
    }
    #endregion
    #region GỬI EMAIL
    // Random dùng chung để chọn autoX
    private static readonly Random _emailRandom = new Random();

    public static void SendMail(
        string toEmail,
        string subject,
        string body,
        bool isBodyHtml = false,
        int? fromIndex = null,
        string fromDisplayName = null,
        IEnumerable<Attachment> attachments = null,
        IEnumerable<string> ccEmails = null,
        IEnumerable<string> bccEmails = null,
        string replyTo = null
    )
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Email nhận không được để trống.", "toEmail");

        SendMail(
            new List<string> { toEmail },
            subject,
            body,
            isBodyHtml,
            fromIndex,
            fromDisplayName,
            attachments,
            ccEmails,
            bccEmails,
            replyTo
        );
    }

    public static void SendMail(
        IEnumerable<string> toEmails,
        string subject,
        string body,
        bool isBodyHtml = false,
        int? fromIndex = null,
        string fromDisplayName = null,
        IEnumerable<Attachment> attachments = null,
        IEnumerable<string> ccEmails = null,
        IEnumerable<string> bccEmails = null,
        string replyTo = null
    )
    {
        if (toEmails == null)
            throw new ArgumentNullException("toEmails");

        // Làm sạch danh sách email (trim, distinct, bỏ rỗng)
        var toList = toEmails
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!toList.Any())
            throw new ArgumentException("Danh sách email nhận đang rỗng.", "toEmails");

        // Đọc config
        string smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "";
        string smtpPortStr = ConfigurationManager.AppSettings["SmtpPort"] ?? "587";
        string emailPassword = ConfigurationManager.AppSettings["EmailPassword"] ?? "";
        string fromDisplayNameConfig = ConfigurationManager.AppSettings["EmailFromDisplayName"] ?? "Hotasoft.com";

        if (string.IsNullOrWhiteSpace(smtpServer))
            throw new InvalidOperationException("Chưa cấu hình SmtpServer trong web.config (appSettings).");

        if (string.IsNullOrWhiteSpace(emailPassword))
            throw new InvalidOperationException("Chưa cấu hình EmailPassword trong web.config (appSettings).");

        int smtpPort;
        if (!int.TryParse(smtpPortStr, out smtpPort))
        {
            smtpPort = 587;
        }

        // Nếu caller không truyền fromDisplayName thì dùng config (hoặc default)
        if (string.IsNullOrWhiteSpace(fromDisplayName))
        {
            fromDisplayName = fromDisplayNameConfig;
        }

        // Chọn tài khoản gửi: auto1@hotasoft.com ... auto19@hotasoft.com
        int idx;
        if (fromIndex.HasValue)
        {
            idx = fromIndex.Value; // bạn có thể truyền cố định 1..19 nếu muốn
        }
        else
        {
            lock (_emailRandom)
            {
                idx = _emailRandom.Next(1, 20); // [1, 19]
            }
        }

        string fromEmail = string.Format("auto{0}@hotasoft.com", idx);

        using (var message = new MailMessage())
        {
            // From
            message.From = new MailAddress(fromEmail, fromDisplayName, Encoding.UTF8);

            // To
            foreach (var to in toList)
            {
                message.To.Add(new MailAddress(to));
            }

            // CC
            if (ccEmails != null)
            {
                foreach (var cc in ccEmails.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    message.CC.Add(new MailAddress(cc.Trim()));
                }
            }

            // BCC
            if (bccEmails != null)
            {
                foreach (var bcc in bccEmails.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    message.Bcc.Add(new MailAddress(bcc.Trim()));
                }
            }

            // ReplyTo
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo.Trim()));
            }

            // Nội dung
            message.Subject = subject ?? "";
            message.Body = body ?? "";
            message.IsBodyHtml = isBodyHtml;
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;

            // Đính kèm (nếu có)
            if (attachments != null)
            {
                foreach (var att in attachments)
                {
                    if (att != null)
                        message.Attachments.Add(att);
                }
            }

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true; // thường port 587 dùng TLS
                client.Credentials = new NetworkCredential(fromEmail, emailPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 30000; // 30s

                client.Send(message);
            }
        }
    }
    #endregion




}
