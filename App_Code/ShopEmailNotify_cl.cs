public static class ShopEmailNotify_cl
{
    public sealed class TemplateMailData
    {
        public string ShopName;
        public string ShopEmail;
        public string OrderCode;
        public string CustomerName;
        public string Total;
        public string OrderStatus;
        public string OrderUrl;
        public string CreatedAt;
        public string Message;
    }

    public static bool TryNotifyOrder(dbDataContext db, DonHang_tb order, string templateCode, string message, out string error)
    {
        error = string.Empty;
        return false;
    }

    public static bool TrySendTemplate(dbDataContext db, string templateCode, TemplateMailData data, out string error)
    {
        error = string.Empty;
        return false;
    }
}
