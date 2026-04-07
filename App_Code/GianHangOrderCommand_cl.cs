using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GianHangOrderCommand_cl
{
    public sealed class OfflineCartLine
    {
        public string ProductId { get; set; }
        public string ReferenceId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public int PhanTramGiamGia { get; set; }

        public string HomePostId { get { return ReferenceId; } set { ReferenceId = value; } }
    }

    public sealed class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AlertType { get; set; }
        public bool UseHtmlMessage { get; set; }
        public bool ShouldRedirect { get; set; }
        public string RedirectUrl { get; set; }
        public string OrderId { get; set; }
    }

    public static CommandResult CreateOfflineOrder(
        dbDataContext db,
        string sellerAccount,
        IEnumerable<OfflineCartLine> rawLines,
        string choThanhToanUrl)
    {
        List<OfflineCartLine> cart = (rawLines ?? Enumerable.Empty<OfflineCartLine>())
            .Where(p => p != null)
            .Select(CloneCartLine)
            .ToList();

        if (db == null || string.IsNullOrWhiteSpace(sellerAccount))
            return Fail("Không xác định gian hàng hiện tại.");

        if (cart.Count == 0)
            return Fail("Giỏ hàng trống. Vui lòng thêm tin để tạo đơn.");

        Dictionary<string, GH_SanPham_tb> productCache = new Dictionary<string, GH_SanPham_tb>(StringComparer.OrdinalIgnoreCase);
        for (int i = cart.Count - 1; i >= 0; i--)
        {
            OfflineCartLine item = cart[i];
            item.SoLuong = ClampInt(item.SoLuong, 0, 999);
            item.PhanTramGiamGia = ClampInt(item.PhanTramGiamGia, 0, 50);
            if (item.SoLuong <= 0)
            {
                cart.RemoveAt(i);
                continue;
            }

            GH_SanPham_tb product = GianHangOrderCore_cl.FindSellableProduct(db, sellerAccount, item.ProductId);
            if (product == null)
                return Fail("Có tin trong giỏ đã bị xóa/ẩn hoặc không còn thuộc /gianhang này. ID: " + item.ProductId);

            item.ReferenceId = GianHangOrderWrite_cl.ResolveReferenceId(product);
            item.Name = product.ten ?? item.Name;
            item.Image = product.hinh_anh ?? item.Image;
            item.GiaBan = product.gia_ban ?? item.GiaBan;
            productCache[item.ProductId] = product;
        }

        if (cart.Count == 0)
            return Fail("Giỏ hàng không hợp lệ (SL = 0).");

        List<GianHangOrderWrite_cl.OrderLine> orderLines = new List<GianHangOrderWrite_cl.OrderLine>();
        foreach (OfflineCartLine item in cart)
        {
            GH_SanPham_tb product;
            if (!productCache.TryGetValue(item.ProductId, out product) || product == null)
                product = GianHangOrderCore_cl.FindSellableProduct(db, sellerAccount, item.ProductId);

            orderLines.Add(GianHangOrderWrite_cl.BuildLine(
                product,
                item.SoLuong,
                item.GiaBan,
                ClampInt(item.PhanTramGiamGia, 0, 50)));
        }

        DateTime now = AhaTime_cl.Now;
        GianHangOrderWrite_cl.CreateOrderResult writeResult = GianHangOrderWrite_cl.CreateOrderAndInvoice(db, new GianHangOrderWrite_cl.CreateOrderInput
        {
            SellerAccount = sellerAccount,
            BuyerAccount = string.Empty,
            CustomerName = string.Empty,
            CustomerPhone = string.Empty,
            CustomerAddress = string.Empty,
            Note = string.Empty,
            IsOnlineOrder = false,
            IsWaitingExchange = false,
            UpdateNativeCounters = false,
            CreatedAt = now,
            Lines = orderLines
        });
        if (writeResult == null)
            return Fail("Không thể tạo đơn trong /gianhang.");

        string orderId = GianHangOrderWrite_cl.ResolveOrderKey(writeResult);
        if (string.IsNullOrWhiteSpace(orderId))
            return Fail("Không thể tạo đơn trong /gianhang.");

        GianHangOrderRuntime_cl.OrderRuntime runtime = GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, sellerAccount, orderId);
        if (runtime == null)
            runtime = GianHangOrderRuntime_cl.CreateRuntime(sellerAccount, writeResult.LegacyOrder, writeResult.Invoice);

        string publicOrderId = GianHangOrderWrite_cl.ResolvePublicOrderId(writeResult);
        if (string.IsNullOrWhiteSpace(publicOrderId))
            publicOrderId = orderId;

        GianHangOrderRuntime_cl.OrderRuntime anotherPending = GianHangOrderRuntime_cl.ResolveAnotherWaitingExchangeForCommand(db, sellerAccount, orderId);
        string message;
        string waitingOrderIdForRedirect;
        if (anotherPending == null)
        {
            GianHangOrderRuntime_cl.PersistWaitingExchangeOpened(db, runtime);

            message = "Tạo đơn trong /gianhang thành công. ID: <b>" + publicOrderId + "</b>";
            waitingOrderIdForRedirect = string.IsNullOrWhiteSpace(runtime == null ? string.Empty : runtime.OrderId)
                ? orderId
                : runtime.OrderId;
        }
        else
        {
            message = "Bạn đang có 1 đơn chờ Trao đổi khác (ID: <b>" + anotherPending.OrderId + "</b>). Hệ thống giữ đơn mới nhưng sẽ chuyển sang phiên chờ Trao đổi đang hoạt động.";
            waitingOrderIdForRedirect = anotherPending.OrderId;
        }

        return new CommandResult
        {
            Success = true,
            Message = message,
            AlertType = "alert",
            UseHtmlMessage = true,
            ShouldRedirect = true,
            RedirectUrl = BuildWaitRedirectUrl(choThanhToanUrl, waitingOrderIdForRedirect),
            OrderId = publicOrderId
        };
    }

    public static CommandResult ActivateWaitingExchange(dbDataContext db, string sellerAccount, string orderId, string choThanhToanUrl)
    {
        GianHangOrderRuntime_cl.OrderRuntime runtime = GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, sellerAccount, orderId);
        string publicOrderId = GianHangOrderRuntime_cl.ResolvePublicOrderId(runtime);
        if (string.IsNullOrWhiteSpace(publicOrderId))
            publicOrderId = (orderId ?? string.Empty).Trim();
        if (runtime == null)
            return Fail("Đơn hàng không tồn tại hoặc không thuộc /gianhang này.");

        if (!CanActivateWaitingExchange(runtime))
            return Fail("Không thể kích hoạt chờ Trao đổi ở trạng thái hiện tại.");

        GianHangOrderRuntime_cl.OrderRuntime anotherPending = GianHangOrderRuntime_cl.ResolveAnotherWaitingExchangeForCommand(db, sellerAccount, orderId);
        if (anotherPending != null)
            return Fail("Bạn đang có 1 đơn chờ Trao đổi khác. ID đơn: " + anotherPending.OrderId);

        GianHangOrderRuntime_cl.PersistWaitingExchangeOpened(db, runtime);

        return new CommandResult
        {
            Success = true,
            ShouldRedirect = true,
            RedirectUrl = BuildWaitRedirectUrl(choThanhToanUrl, runtime.OrderId)
        };
    }

    public static CommandResult CancelWaitingExchange(dbDataContext db, string sellerAccount, string orderId)
    {
        GianHangOrderRuntime_cl.OrderRuntime runtime = GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, sellerAccount, orderId);
        if (runtime == null)
            return Fail("Đơn hàng không tồn tại hoặc không thuộc /gianhang này.");

        if (!CanCancelWaitingExchange(runtime))
            return Fail("Không thể hủy chờ Trao đổi ở trạng thái hiện tại.");

        GianHangOrderRuntime_cl.PersistWaitingExchangeCancelled(db, runtime);

        return Success("Đã hủy trạng thái chờ Trao đổi.");
    }

    public static CommandResult MarkDelivered(dbDataContext db, string sellerAccount, string orderId)
    {
        GianHangOrderRuntime_cl.OrderRuntime runtime = GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, sellerAccount, orderId);
        string publicOrderId = GianHangOrderRuntime_cl.ResolvePublicOrderId(runtime);
        if (string.IsNullOrWhiteSpace(publicOrderId))
            publicOrderId = (orderId ?? string.Empty).Trim();
        if (runtime == null)
            return Fail("Đơn hàng không tồn tại hoặc không thuộc /gianhang này.");

        if (!CanMarkDelivered(runtime))
            return Fail("Không thể xác nhận giao hàng ở trạng thái hiện tại.");

        return GianHangOrderLifecycle_cl.MarkDelivered(db, sellerAccount, runtime, publicOrderId);
    }

    public static CommandResult CancelOrder(dbDataContext db, string sellerAccount, string orderId)
    {
        GianHangOrderRuntime_cl.OrderRuntime runtime = GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, sellerAccount, orderId);
        string publicOrderId = GianHangOrderRuntime_cl.ResolvePublicOrderId(runtime);
        if (string.IsNullOrWhiteSpace(publicOrderId))
            publicOrderId = (orderId ?? string.Empty).Trim();
        if (runtime == null)
            return Fail("Đơn hàng không tồn tại hoặc không thuộc /gianhang này.");

        if (!CanCancelOrder(runtime))
            return Fail("Không thể hủy đơn hàng này ở trạng thái hiện tại.");

        return GianHangOrderLifecycle_cl.CancelOrder(db, sellerAccount, publicOrderId, runtime);
    }

    private static OfflineCartLine CloneCartLine(OfflineCartLine item)
    {
        return new OfflineCartLine
        {
            ProductId = item.ProductId,
            ReferenceId = item.ReferenceId,
            Name = item.Name,
            Image = item.Image,
            GiaBan = item.GiaBan,
            SoLuong = item.SoLuong,
            PhanTramGiamGia = item.PhanTramGiamGia
        };
    }

    private static int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private static bool CanActivateWaitingExchange(GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        return GianHangOrderRuntime_cl.CanOpenWaitingExchange(runtime);
    }

    private static bool CanCancelWaitingExchange(GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        return GianHangOrderRuntime_cl.CanCancelWaitingExchange(runtime);
    }

    private static bool CanMarkDelivered(GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        return GianHangOrderRuntime_cl.CanMarkDelivered(runtime);
    }

    private static bool CanCancelOrder(GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        return GianHangOrderRuntime_cl.CanCancelOrder(runtime);
    }

    private static CommandResult Fail(string message)
    {
        return new CommandResult
        {
            Success = false,
            Message = message,
            AlertType = "warning"
        };
    }

    private static CommandResult Success(string message)
    {
        return new CommandResult
        {
            Success = true,
            Message = message,
            AlertType = "success"
        };
    }

    private static string BuildWaitRedirectUrl(string baseUrl, string orderId)
    {
        string url = string.IsNullOrWhiteSpace(baseUrl) ? "/gianhang/cho-thanh-toan.aspx" : baseUrl.Trim();
        string safeOrderId = (orderId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(safeOrderId))
            return url;

        string join = url.Contains("?") ? "&" : "?";
        return url + join + "id=" + HttpUtility.UrlEncode(safeOrderId);
    }
}
