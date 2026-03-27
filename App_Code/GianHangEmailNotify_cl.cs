using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

public static class GianHangEmailNotify_cl
{
    public sealed class TemplateMailData
    {
        public string ShopName { get; set; }
        public string ShopEmail { get; set; }
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string Total { get; set; }
        public string OrderStatus { get; set; }
        public string OrderUrl { get; set; }
        public string CreatedAt { get; set; }
        public string Message { get; set; }
    }

    public static bool TryNotifyInvoice(dbDataContext db, GH_HoaDon_tb invoice, string templateCode, string message, out string error)
    {
        error = string.Empty;
        if (db == null || invoice == null)
            return false;

        string sellerAccount = (invoice.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(sellerAccount))
            return false;

        taikhoan_tb seller = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == sellerAccount);
        if (seller == null)
            return false;

        string shopEmail = ResolveShopEmail(seller);
        if (string.IsNullOrWhiteSpace(shopEmail))
            return false;

        taikhoan_tb buyer = null;
        string buyerAccount = (invoice.buyer_account ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(buyerAccount))
            buyer = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == buyerAccount);

        TemplateMailData ctx = new TemplateMailData
        {
            ShopName = ResolveShopName(seller),
            ShopEmail = shopEmail,
            OrderCode = GianHangInvoice_cl.ResolveOrderPublicId(invoice),
            CustomerName = ResolveCustomerName(invoice, buyer),
            Total = string.Format("{0:#,##0} VND", invoice.tong_tien ?? 0m),
            OrderStatus = GianHangInvoice_cl.ResolveOrderStatusText(invoice),
            OrderUrl = BuildOrderDetailUrl(invoice),
            CreatedAt = invoice.ngay_tao.HasValue ? invoice.ngay_tao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
            Message = message ?? string.Empty
        };

        return TrySendTemplate(db, templateCode, ctx, out error);
    }

    public static bool TrySendTemplate(dbDataContext db, string templateCode, TemplateMailData ctx, out string error)
    {
        error = string.Empty;
        if (db == null || ctx == null)
            return false;

        string code = GianHangEmailTemplate_cl.NormalizeCode(templateCode);
        if (!GianHangEmailTemplate_cl.IsValidCode(code))
            return false;

        GianHangEmailTemplate_cl.TemplateItem tpl = GianHangEmailTemplate_cl.GetEffectiveByCode(db, code);
        if (tpl == null || !tpl.IsActive)
            return false;

        string toEmail = (ctx.ShopEmail ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(toEmail))
            return false;

        string subject = RenderTemplate(tpl.Subject, ctx);
        string bodyHtml = RenderBodyAsHtml(RenderTemplate(tpl.Body, ctx));

        return TrySendEmail(toEmail, subject, bodyHtml, out error);
    }

    private static string RenderTemplate(string template, TemplateMailData ctx)
    {
        string value = template ?? string.Empty;
        value = value.Replace("{SHOP_NAME}", ctx.ShopName ?? string.Empty);
        value = value.Replace("{SHOP_EMAIL}", ctx.ShopEmail ?? string.Empty);
        value = value.Replace("{ORDER_CODE}", ctx.OrderCode ?? string.Empty);
        value = value.Replace("{CUSTOMER_NAME}", ctx.CustomerName ?? string.Empty);
        value = value.Replace("{TOTAL}", ctx.Total ?? string.Empty);
        value = value.Replace("{ORDER_STATUS}", ctx.OrderStatus ?? string.Empty);
        value = value.Replace("{ORDER_URL}", ctx.OrderUrl ?? string.Empty);
        value = value.Replace("{CREATED_AT}", ctx.CreatedAt ?? string.Empty);
        value = value.Replace("{MESSAGE}", ctx.Message ?? string.Empty);
        return value;
    }

    private static string RenderBodyAsHtml(string body)
    {
        string value = body ?? string.Empty;
        if (value.IndexOf("<html", StringComparison.OrdinalIgnoreCase) >= 0
            || value.IndexOf("<body", StringComparison.OrdinalIgnoreCase) >= 0
            || value.IndexOf("<br", StringComparison.OrdinalIgnoreCase) >= 0
            || value.IndexOf("<p", StringComparison.OrdinalIgnoreCase) >= 0)
            return value;

        string encoded = HttpUtility.HtmlEncode(value);
        encoded = encoded.Replace("\r\n", "\n").Replace("\r", "\n");
        encoded = encoded.Replace("\n", "<br/>");
        return encoded;
    }

    private static bool TrySendEmail(string toEmail, string subject, string bodyHtml, out string error)
    {
        error = string.Empty;

        try
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls;
            }
            catch
            {
            }

            string smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? string.Empty;
            string smtpPortRaw = ConfigurationManager.AppSettings["SmtpPort"] ?? "587";
            string smtpPassword = ConfigurationManager.AppSettings["EmailPassword"] ?? string.Empty;
            string fromDisplayName = ConfigurationManager.AppSettings["EmailFromDisplayName"] ?? "AhaSale Gian Hang";
            string fromAddress = ConfigurationManager.AppSettings["EmailFromAddress"] ?? "hotro@ahasale.vn";
            string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(smtpServer))
            {
                error = "Chưa cấu hình SmtpServer.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(smtpPassword))
            {
                error = "Chưa cấu hình EmailPassword.";
                return false;
            }

            int smtpPort;
            if (!int.TryParse(smtpPortRaw, out smtpPort) || smtpPort <= 0)
                smtpPort = 587;

            if (string.IsNullOrWhiteSpace(smtpUsername))
                smtpUsername = fromAddress;

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(fromAddress, fromDisplayName, Encoding.UTF8);
                message.To.Add(toEmail);
                message.Subject = subject ?? string.Empty;
                message.Body = bodyHtml ?? string.Empty;
                message.IsBodyHtml = true;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Timeout = 30000;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.Send(message);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            string detail = ex.Message ?? string.Empty;
            if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                detail = detail + " | " + ex.InnerException.Message;
            error = string.IsNullOrWhiteSpace(detail) ? "Failure sending mail." : detail;
            Log_cl.Add_Log("[GIANHANG EMAIL] " + ex.Message, toEmail ?? string.Empty, ex.StackTrace);
            return false;
        }
    }

    private static string ResolveShopEmail(taikhoan_tb seller)
    {
        if (seller == null) return string.Empty;
        string email = (seller.email ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(email))
            return email;
        return (seller.email_shop ?? string.Empty).Trim();
    }

    private static string ResolveShopName(taikhoan_tb seller)
    {
        if (seller == null) return string.Empty;
        if (!string.IsNullOrWhiteSpace(seller.ten_shop))
            return seller.ten_shop.Trim();
        if (!string.IsNullOrWhiteSpace(seller.hoten))
            return seller.hoten.Trim();
        return (seller.taikhoan ?? string.Empty).Trim();
    }

    private static string ResolveCustomerName(GH_HoaDon_tb invoice, taikhoan_tb buyer)
    {
        if (buyer != null && !string.IsNullOrWhiteSpace(buyer.hoten))
            return buyer.hoten.Trim();
        if (invoice != null && !string.IsNullOrWhiteSpace(invoice.ten_khach))
            return invoice.ten_khach.Trim();
        if (invoice != null && !string.IsNullOrWhiteSpace(invoice.buyer_account))
            return invoice.buyer_account.Trim();
        return "Khách hàng";
    }

    private static string BuildOrderDetailUrl(string orderId)
    {
        string safeId = (orderId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(safeId))
            return string.Empty;

        string relative = "/gianhang/don-ban.aspx?return_url=%2fgianhang%2fdefault.aspx";
        return BuildAbsoluteUrl(relative);
    }

    private static string BuildOrderDetailUrl(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return string.Empty;

        string publicId = GianHangInvoice_cl.ResolveOrderPublicId(invoice);
        string relative = "/gianhang/don-ban.aspx?return_url=%2fgianhang%2fdefault.aspx";
        if (!string.IsNullOrWhiteSpace(publicId))
            relative += "&invoice=" + System.Web.HttpUtility.UrlEncode(publicId);
        return BuildAbsoluteUrl(relative);
    }

    private static string BuildAbsoluteUrl(string relativeUrl)
    {
        string path = relativeUrl ?? string.Empty;
        System.Web.HttpContext ctx = System.Web.HttpContext.Current;
        if (ctx == null || ctx.Request == null || ctx.Request.Url == null)
            return path;

        string baseUrl = ctx.Request.Url.GetLeftPart(UriPartial.Authority);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return path;
        if (!path.StartsWith("/"))
            path = "/" + path;
        return baseUrl + path;
    }
}
