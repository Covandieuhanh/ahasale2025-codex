using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Linq;

public static class ShopEmailNotify_cl
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

    public static bool TryNotifyOrder(dbDataContext db, DonHang_tb order, string templateCode, string message, out string error)
    {
        error = "";
        if (db == null || order == null) return false;

        taikhoan_tb seller = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == order.nguoiban);
        if (seller == null) return false;

        string shopEmail = ResolveShopEmail(seller);
        if (string.IsNullOrWhiteSpace(shopEmail)) return false;

        taikhoan_tb buyer = null;
        if (!string.IsNullOrWhiteSpace(order.nguoimua))
            buyer = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == order.nguoimua);

        string orderStatus = DonHangStateMachine_cl.GetOrderStatus(order);
        string orderUrl = BuildOrderDetailUrl(seller, order.id.ToString());

        TemplateMailData ctx = new TemplateMailData
        {
            ShopName = ResolveShopName(seller),
            ShopEmail = shopEmail,
            OrderCode = order.id.ToString(),
            CustomerName = ResolveCustomerName(order, buyer),
            Total = FormatCurrency(order.tongtien),
            OrderStatus = orderStatus,
            OrderUrl = orderUrl,
            CreatedAt = FormatDate(order.ngaydat),
            Message = message ?? ""
        };

        return TrySendTemplate(db, templateCode, ctx, out error);
    }

    public static bool TrySendTemplate(dbDataContext db, string templateCode, TemplateMailData ctx, out string error)
    {
        error = "";
        if (db == null || ctx == null) return false;

        string code = ShopEmailTemplate_cl.NormalizeCode(templateCode);
        if (!ShopEmailTemplate_cl.IsValidCode(code)) return false;

        ShopEmailTemplate_cl.TemplateItem tpl = ShopEmailTemplate_cl.GetEffectiveByCode(db, code);
        if (tpl == null) return false;
        if (!tpl.IsActive) return false;

        string subject = ApplyTokens(tpl.Subject, ctx);
        string body = ApplyTokens(tpl.Body, ctx);

        return SendTextMail(db, ctx.ShopEmail, subject, body, out error);
    }

    private static string ApplyTokens(string input, TemplateMailData ctx)
    {
        if (string.IsNullOrEmpty(input)) return "";

        return input
            .Replace("{SHOP_NAME}", ctx.ShopName ?? "")
            .Replace("{SHOP_EMAIL}", ctx.ShopEmail ?? "")
            .Replace("{ORDER_CODE}", ctx.OrderCode ?? "")
            .Replace("{CUSTOMER_NAME}", ctx.CustomerName ?? "")
            .Replace("{TOTAL}", ctx.Total ?? "")
            .Replace("{ORDER_STATUS}", ctx.OrderStatus ?? "")
            .Replace("{ORDER_URL}", ctx.OrderUrl ?? "")
            .Replace("{CREATED_AT}", ctx.CreatedAt ?? "")
            .Replace("{MESSAGE}", ctx.Message ?? "");
    }

    private static string ResolveShopEmail(taikhoan_tb seller)
    {
        if (seller == null) return "";
        string email = (seller.email ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(email)) return email;
        return (seller.email_shop ?? "").Trim();
    }

    private static string ResolveShopName(taikhoan_tb seller)
    {
        if (seller == null) return "";
        if (!string.IsNullOrWhiteSpace(seller.ten_shop)) return seller.ten_shop.Trim();
        if (!string.IsNullOrWhiteSpace(seller.hoten)) return seller.hoten.Trim();
        return (seller.taikhoan ?? "").Trim();
    }

    private static string ResolveCustomerName(DonHang_tb order, taikhoan_tb buyer)
    {
        if (buyer != null && !string.IsNullOrWhiteSpace(buyer.hoten))
            return buyer.hoten.Trim();

        if (order != null && !string.IsNullOrWhiteSpace(order.hoten_nguoinhan))
            return order.hoten_nguoinhan.Trim();

        if (order != null && !string.IsNullOrWhiteSpace(order.nguoimua))
            return order.nguoimua.Trim();

        return "Khách hàng";
    }

    private static string FormatCurrency(decimal? value)
    {
        decimal v = value ?? 0m;
        return string.Format("{0:#,##0}đ", v);
    }

    private static string FormatDate(DateTime? value)
    {
        if (!value.HasValue) return "";
        return value.Value.ToString("dd/MM/yyyy HH:mm");
    }

    private static string BuildOrderDetailUrl(taikhoan_tb seller, string orderId)
    {
        string id = (orderId ?? "").Trim();
        if (string.IsNullOrEmpty(id)) return "";

        string scope = PortalScope_cl.ResolveScope(seller.taikhoan, seller.phanloai, seller.permission);
        bool isShop = string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);

        string listUrl = isShop ? "/shop/don-ban" : "/home/don-ban.aspx";
        string detailBase = isShop ? "/shop/don-chi-tiet" : "/home/don-chi-tiet.aspx";

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["id"] = id;
        query["mode"] = "sell";
        query["return_url"] = listUrl;

        string relative = detailBase + "?" + query.ToString();
        return BuildAbsoluteUrl(relative);
    }

    private static string BuildAbsoluteUrl(string relativeUrl)
    {
        string path = relativeUrl ?? "";
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Request == null || ctx.Request.Url == null)
            return path;

        string baseUrl = ctx.Request.Url.GetLeftPart(UriPartial.Authority);
        if (string.IsNullOrWhiteSpace(baseUrl)) return path;
        if (!path.StartsWith("/")) path = "/" + path;
        return baseUrl + path;
    }

    private static bool SendTextMail(dbDataContext db, string toEmail, string subject, string body, out string error)
    {
        error = "";
        if (string.IsNullOrWhiteSpace(toEmail)) return false;

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
                // ignore
            }

            EmailOtpConfig cfg = OtpConfig_cl.GetEmailConfig(db);
            if (cfg == null || string.IsNullOrWhiteSpace(cfg.Host) || cfg.Port <= 0)
            {
                error = "Chưa cấu hình Email OTP.";
                return false;
            }

            if (cfg.DevMode)
            {
                Log_cl.Add_Log("[SHOP EMAIL DEV] " + (toEmail ?? "") + " | " + (subject ?? ""), toEmail ?? "", "");
                return true;
            }

            string fromAddress = string.IsNullOrWhiteSpace(cfg.FromEmail) ? "" : cfg.FromEmail.Trim();
            if (string.IsNullOrWhiteSpace(fromAddress))
                fromAddress = "hotro@ahasale.vn";

            string fromName = string.IsNullOrWhiteSpace(cfg.FromName) ? "AhaSale" : cfg.FromName.Trim();

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(fromAddress, fromName, Encoding.UTF8);
                message.To.Add(toEmail.Trim());
                message.Subject = subject ?? "";
                message.Body = body ?? "";
                message.IsBodyHtml = false;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                using (SmtpClient client = new SmtpClient(cfg.Host, cfg.Port))
                {
                    client.EnableSsl = cfg.UseSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;

                    string smtpUser = string.IsNullOrWhiteSpace(cfg.Username) ? "" : cfg.Username.Trim();
                    if (string.IsNullOrWhiteSpace(smtpUser))
                        smtpUser = fromAddress;
                    if (!string.IsNullOrWhiteSpace(smtpUser))
                    {
                        client.Credentials = new NetworkCredential(smtpUser, cfg.Password ?? "");
                    }

                    client.Send(message);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            string detail = ex.Message ?? "";
            if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                detail = detail + " | " + ex.InnerException.Message;
            error = string.IsNullOrWhiteSpace(detail) ? "Failure sending mail." : detail;
            Log_cl.Add_Log("[SHOP EMAIL] " + ex.Message, toEmail ?? "", ex.StackTrace);
            return false;
        }
    }
}
