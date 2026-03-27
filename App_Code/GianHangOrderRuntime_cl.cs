using System;
using System.Linq;

public static class GianHangOrderRuntime_cl
{
    public sealed class OrderRuntime
    {
        public string SellerAccount { get; set; }
        public DonHang_tb Order { get; set; }
        public GH_HoaDon_tb Invoice { get; set; }

        public bool HasOrder
        {
            get { return Order != null; }
        }

        public string OrderId
        {
            get
            {
                string invoiceOrderId = GianHangInvoice_cl.ResolveRuntimeOrderKey(Invoice);
                if (!string.IsNullOrWhiteSpace(invoiceOrderId))
                    return invoiceOrderId;

                if (Order != null)
                    return Order.id.ToString();

                return Invoice == null ? string.Empty : Invoice.id.ToString();
            }
        }
    }

    private static bool HasInvoice(OrderRuntime runtime)
    {
        return runtime != null && runtime.Invoice != null;
    }

    private static DonHang_tb EnsureLoadedLegacyOrder(OrderRuntime runtime)
    {
        if (runtime == null || runtime.Order == null)
            return null;

        DonHangStateMachine_cl.EnsureStateFields(runtime.Order);
        return runtime.Order;
    }

    private static string MapLegacyStatusesToGroup(string orderStatus, string exchangeStatus)
    {
        string normalizedOrderStatus = DonHangStateMachine_cl.NormalizeOrderStatus(orderStatus);
        string normalizedExchangeStatus = DonHangStateMachine_cl.NormalizeExchangeStatus(exchangeStatus);

        if (normalizedOrderStatus == DonHangStateMachine_cl.Order_DaHuy)
            return "da-huy";
        if (normalizedExchangeStatus == DonHangStateMachine_cl.Exchange_DaTraoDoi)
            return "da-trao-doi";
        if (normalizedExchangeStatus == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
            return "cho-trao-doi";
        if (normalizedOrderStatus == DonHangStateMachine_cl.Order_DaGiao
            || normalizedOrderStatus == DonHangStateMachine_cl.Order_DaNhan)
            return "da-giao";
        return "da-dat";
    }

    public static OrderRuntime ResolveByOrderKey(dbDataContext db, string sellerAccount, string rawOrderKey)
    {
        return ResolveByOrderKey(db, sellerAccount, rawOrderKey, false);
    }

    public static OrderRuntime ResolveByOrderKeyForCommand(dbDataContext db, string sellerAccount, string rawOrderKey)
    {
        return ResolveByOrderKey(db, sellerAccount, rawOrderKey, true);
    }

    public static OrderRuntime ResolveByOrderKey(dbDataContext db, string sellerAccount, string rawOrderKey, bool requireLegacyOrder)
    {
        string shopAccount = NormalizeAccount(sellerAccount);
        string orderKey = (rawOrderKey ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(shopAccount) || string.IsNullOrWhiteSpace(orderKey))
            return null;

        GH_HoaDon_tb invoice = GianHangInvoice_cl.EnsureInvoiceByOrderKey(db, shopAccount, orderKey);

        DonHang_tb order = null;
        bool shouldLoadLegacyOrder = invoice == null
            || (requireLegacyOrder && GianHangInvoice_cl.NeedsLegacyRuntimeForCommand(invoice));

        if (shouldLoadLegacyOrder)
            order = GianHangInvoice_cl.ResolveLegacyOrderForRuntime(db, shopAccount, invoice, orderKey);

        if (order == null && invoice == null)
            return null;

        if (order != null)
            DonHangStateMachine_cl.EnsureStateFields(order);

        return new OrderRuntime
        {
            SellerAccount = shopAccount,
            Order = order,
            Invoice = invoice
        };
    }

    public static OrderRuntime ResolveLatestWaitingExchange(dbDataContext db, string sellerAccount)
    {
        return ResolveLatestWaitingExchange(db, sellerAccount, false);
    }

    public static OrderRuntime ResolveLatestWaitingExchangeForCommand(dbDataContext db, string sellerAccount)
    {
        return ResolveLatestWaitingExchange(db, sellerAccount, true);
    }

    public static OrderRuntime ResolveLatestWaitingExchange(dbDataContext db, string sellerAccount, bool requireLegacyOrder)
    {
        string shopAccount = NormalizeAccount(sellerAccount);
        if (db == null || string.IsNullOrWhiteSpace(shopAccount))
            return null;

        GH_HoaDon_tb waitingInvoice = GianHangInvoice_cl.FindLatestWaitingExchangeWithSnapshot(db, shopAccount, 500);

        if (waitingInvoice != null)
        {
            OrderRuntime runtimeFromInvoice = ResolveByOrderKey(
                db,
                shopAccount,
                GianHangInvoice_cl.ResolveRuntimeOrderKey(waitingInvoice),
                requireLegacyOrder);
            if (runtimeFromInvoice != null)
                return runtimeFromInvoice;
        }

        return null;
    }

    public static OrderRuntime ResolveAnotherWaitingExchange(dbDataContext db, string sellerAccount, string excludedOrderId)
    {
        return ResolveAnotherWaitingExchange(db, sellerAccount, excludedOrderId, false);
    }

    public static OrderRuntime ResolveAnotherWaitingExchangeForCommand(dbDataContext db, string sellerAccount, string excludedOrderId)
    {
        return ResolveAnotherWaitingExchange(db, sellerAccount, excludedOrderId, true);
    }

    public static OrderRuntime ResolveAnotherWaitingExchange(dbDataContext db, string sellerAccount, string excludedOrderId, bool requireLegacyOrder)
    {
        string shopAccount = NormalizeAccount(sellerAccount);
        string excluded = (excludedOrderId ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(shopAccount))
            return null;

        GH_HoaDon_tb waitingInvoice = GianHangInvoice_cl.FindAnotherWaitingExchangeWithSnapshot(db, shopAccount, excluded, 500);
        if (waitingInvoice == null)
            return null;

        OrderRuntime runtimeFromInvoice = ResolveByOrderKey(
            db,
            shopAccount,
            GianHangInvoice_cl.ResolveRuntimeOrderKey(waitingInvoice),
            requireLegacyOrder);
        if (runtimeFromInvoice == null)
            return null;
        if (string.Equals(runtimeFromInvoice.OrderId, excluded, StringComparison.OrdinalIgnoreCase))
            return null;

        return runtimeFromInvoice;
    }

    public static bool SyncInvoiceLifecycle(dbDataContext db, OrderRuntime runtime, string invoiceStatus)
    {
        if (db == null || runtime == null)
            return false;

        string sellerAccount = NormalizeAccount(runtime.SellerAccount);
        if (string.IsNullOrWhiteSpace(sellerAccount))
            sellerAccount = runtime.Order == null ? string.Empty : NormalizeAccount(runtime.Order.nguoiban);
        if (string.IsNullOrWhiteSpace(sellerAccount))
            sellerAccount = runtime.Invoice == null ? string.Empty : NormalizeAccount(runtime.Invoice.shop_taikhoan);
        if (string.IsNullOrWhiteSpace(sellerAccount))
            return false;

        string orderStatus = runtime.Invoice == null
            ? string.Empty
            : (runtime.Invoice.order_status ?? string.Empty);
        if (string.IsNullOrWhiteSpace(orderStatus) && runtime.Order != null)
            orderStatus = runtime.Order.order_status ?? string.Empty;

        string exchangeStatus = runtime.Invoice == null
            ? string.Empty
            : (runtime.Invoice.exchange_status ?? string.Empty);
        if (string.IsNullOrWhiteSpace(exchangeStatus) && runtime.Order != null)
            exchangeStatus = runtime.Order.exchange_status ?? string.Empty;
        string orderId = runtime.OrderId;

        if (!string.IsNullOrWhiteSpace(orderId))
        {
            return GianHangInvoice_cl.SyncOrderLifecycleByOrderId(
                db,
                sellerAccount,
                orderId,
                orderStatus,
                exchangeStatus,
                invoiceStatus);
        }

        if (runtime.Invoice == null)
            return false;

        if (!string.IsNullOrWhiteSpace(orderStatus))
            runtime.Invoice.order_status = orderStatus.Trim();
        if (!string.IsNullOrWhiteSpace(exchangeStatus))
            runtime.Invoice.exchange_status = exchangeStatus.Trim();
        if (!string.IsNullOrWhiteSpace(invoiceStatus))
            runtime.Invoice.trang_thai = invoiceStatus.Trim();
        db.SubmitChanges();
        return true;
    }

    public static void EnsureLegacyState(OrderRuntime runtime)
    {
        if (runtime == null || runtime.Order == null)
            return;

        DonHangStateMachine_cl.EnsureStateFields(runtime.Order);
    }

    public static bool CanExecuteExchange(OrderRuntime runtime)
    {
        if (runtime == null)
            return false;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.CanExecuteExchange(runtime.Invoice);

        return runtime.Order != null && DonHangStateMachine_cl.CanExecuteExchange(runtime.Order);
    }

    public static bool CanOpenWaitingExchange(OrderRuntime runtime)
    {
        if (runtime == null)
            return false;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.CanOpenWaitingExchange(runtime.Invoice);

        return runtime.Order != null && DonHangStateMachine_cl.CanActivateChoTraoDoi(runtime.Order);
    }

    public static bool CanCancelWaitingExchange(OrderRuntime runtime)
    {
        if (runtime == null)
            return false;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.CanCancelWaitingExchange(runtime.Invoice);

        return runtime.Order != null && DonHangStateMachine_cl.CanCancelChoTraoDoi(runtime.Order);
    }

    public static bool CanMarkDelivered(OrderRuntime runtime)
    {
        if (runtime == null)
            return false;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.CanMarkDelivered(runtime.Invoice);

        return runtime.Order != null && DonHangStateMachine_cl.CanMarkDelivered(runtime.Order);
    }

    public static bool CanCancelOrder(OrderRuntime runtime)
    {
        if (runtime == null)
            return false;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.CanCancelOrder(runtime.Invoice);

        return runtime.Order != null && DonHangStateMachine_cl.CanCancelOrder(runtime.Order);
    }

    public static string ResolveSellerCreditAccount(OrderRuntime runtime, string fallbackAccount)
    {
        if (runtime == null)
            return GianHangInvoice_cl.ResolveSellerAccount((GH_HoaDon_tb)null, fallbackAccount);

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveSellerAccount(runtime.Invoice, fallbackAccount);

        return GianHangInvoice_cl.ResolveSellerAccount(null, runtime.Order, fallbackAccount);
    }

    public static void ApplyExchangeSuccessRuntime(OrderRuntime runtime, taikhoan_tb payerAccount)
    {
        if (runtime == null || payerAccount == null)
            return;

        DonHang_tb order = runtime.Order;
        if (order != null)
        {
            DonHangStateMachine_cl.EnsureStateFields(order);
            DonHangStateMachine_cl.SetExchangeStatus(order, DonHangStateMachine_cl.Exchange_DaTraoDoi);
            order.nguoimua = payerAccount.taikhoan;
            order.hoten_nguoinhan = payerAccount.hoten;
            order.sdt_nguoinhan = payerAccount.dienthoai;
            order.diahchi_nguoinhan = payerAccount.diachi;
            order.chothanhtoan = false;
        }

        GH_HoaDon_tb invoice = runtime.Invoice;
        if (invoice != null)
        {
            if (!string.IsNullOrWhiteSpace(payerAccount.taikhoan))
                invoice.buyer_account = payerAccount.taikhoan.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace((invoice.ten_khach ?? string.Empty).Trim()))
                invoice.ten_khach = !string.IsNullOrWhiteSpace(payerAccount.hoten) ? payerAccount.hoten.Trim() : (payerAccount.taikhoan ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace((invoice.sdt ?? string.Empty).Trim()))
                invoice.sdt = (payerAccount.dienthoai ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace((invoice.dia_chi ?? string.Empty).Trim()))
                invoice.dia_chi = (payerAccount.diachi ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace((invoice.order_status ?? string.Empty).Trim()))
                invoice.order_status = DonHangStateMachine_cl.Order_DaDat;
            invoice.exchange_status = DonHangStateMachine_cl.Exchange_DaTraoDoi;
            invoice.trang_thai = GianHangInvoice_cl.TrangThaiDaThu;
        }
    }

    public static void PersistExchangeSuccess(
        dbDataContext db,
        string sellerAccount,
        OrderRuntime runtime,
        taikhoan_tb payerAccount)
    {
        if (db == null || runtime == null || payerAccount == null)
            return;

        EnsureLoadedLegacyOrder(runtime);
        ApplyExchangeSuccessRuntime(runtime, payerAccount);

        GH_HoaDon_tb invoice = runtime.Invoice;
        if (invoice == null)
            GianHangOrderDetail_cl.ApplySoldCountAfterExchange(db, sellerAccount, runtime);
        else
            GianHangOrderDetail_cl.ApplySoldCountAfterExchange(db, sellerAccount, invoice, GianHangInvoice_cl.GetDetails(db, invoice));

        db.SubmitChanges();
        SyncInvoiceLifecycle(db, runtime, GianHangInvoice_cl.TrangThaiDaThu);
    }

    public static void ApplyWaitingExchangeOpened(OrderRuntime runtime)
    {
        if (runtime == null)
            return;

        DonHang_tb order = runtime.Order;
        if (order != null)
        {
            DonHangStateMachine_cl.EnsureStateFields(order);
            DonHangStateMachine_cl.SetExchangeStatus(order, DonHangStateMachine_cl.Exchange_ChoTraoDoi);
            order.chothanhtoan = true;
        }

        GH_HoaDon_tb invoice = runtime.Invoice;
        if (invoice != null)
        {
            if (string.IsNullOrWhiteSpace((invoice.order_status ?? string.Empty).Trim()))
                invoice.order_status = DonHangStateMachine_cl.Order_DaDat;
            invoice.exchange_status = DonHangStateMachine_cl.Exchange_ChoTraoDoi;
            invoice.trang_thai = GianHangInvoice_cl.TrangThaiMoi;
        }
    }

    public static void PersistWaitingExchangeOpened(dbDataContext db, OrderRuntime runtime)
    {
        if (db == null || runtime == null)
            return;

        EnsureLoadedLegacyOrder(runtime);
        ApplyWaitingExchangeOpened(runtime);
        db.SubmitChanges();
        SyncInvoiceLifecycle(db, runtime, GianHangInvoice_cl.TrangThaiMoi);
    }

    public static void ApplyWaitingExchangeCancelled(OrderRuntime runtime)
    {
        if (runtime == null)
            return;

        DonHang_tb order = runtime.Order;
        if (order != null)
        {
            DonHangStateMachine_cl.EnsureStateFields(order);
            DonHangStateMachine_cl.SetExchangeStatus(order, DonHangStateMachine_cl.Exchange_ChuaTraoDoi);
            order.chothanhtoan = false;
        }

        GH_HoaDon_tb invoice = runtime.Invoice;
        if (invoice != null)
        {
            if (string.IsNullOrWhiteSpace((invoice.order_status ?? string.Empty).Trim()))
                invoice.order_status = DonHangStateMachine_cl.Order_DaDat;
            invoice.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
            invoice.trang_thai = GianHangInvoice_cl.TrangThaiMoi;
        }
    }

    public static void PersistWaitingExchangeCancelled(dbDataContext db, OrderRuntime runtime)
    {
        if (db == null || runtime == null)
            return;

        EnsureLoadedLegacyOrder(runtime);
        ApplyWaitingExchangeCancelled(runtime);
        db.SubmitChanges();
        SyncInvoiceLifecycle(db, runtime, GianHangInvoice_cl.TrangThaiMoi);
    }

    public static void ApplyDelivered(OrderRuntime runtime)
    {
        if (runtime == null)
            return;

        DonHang_tb order = runtime.Order;
        if (order != null)
        {
            DonHangStateMachine_cl.EnsureStateFields(order);
            DonHangStateMachine_cl.SetOrderStatus(order, DonHangStateMachine_cl.Order_DaGiao);
        }

        GH_HoaDon_tb invoice = runtime.Invoice;
        if (invoice != null)
        {
            invoice.order_status = DonHangStateMachine_cl.Order_DaGiao;
            if (string.IsNullOrWhiteSpace((invoice.exchange_status ?? string.Empty).Trim()))
                invoice.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
            invoice.trang_thai = GianHangInvoice_cl.TrangThaiDaThu;
        }
    }

    public static void PersistDelivered(dbDataContext db, OrderRuntime runtime)
    {
        if (db == null || runtime == null)
            return;

        EnsureLoadedLegacyOrder(runtime);
        ApplyDelivered(runtime);
        db.SubmitChanges();
        SyncInvoiceLifecycle(db, runtime, GianHangInvoice_cl.TrangThaiDaThu);
    }

    public static void ApplyCancelled(OrderRuntime runtime)
    {
        if (runtime == null)
            return;

        DonHang_tb order = runtime.Order;
        if (order != null)
        {
            DonHangStateMachine_cl.EnsureStateFields(order);
            DonHangStateMachine_cl.SetOrderStatus(order, DonHangStateMachine_cl.Order_DaHuy);
        }

        GH_HoaDon_tb invoice = runtime.Invoice;
        if (invoice != null)
        {
            invoice.order_status = DonHangStateMachine_cl.Order_DaHuy;
            invoice.trang_thai = GianHangInvoice_cl.TrangThaiHuy;
        }
    }

    public static void PersistCancelled(dbDataContext db, OrderRuntime runtime)
    {
        if (db == null || runtime == null)
            return;

        EnsureLoadedLegacyOrder(runtime);
        ApplyCancelled(runtime);
        db.SubmitChanges();
        SyncInvoiceLifecycle(db, runtime, GianHangInvoice_cl.TrangThaiHuy);
    }

    public static string ResolvePublicOrderId(OrderRuntime runtime)
    {
        if (runtime == null)
            return string.Empty;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveOrderPublicId(runtime.Invoice);

        return runtime.Order == null ? string.Empty : runtime.Order.id.ToString();
    }

    public static string ResolveOrderStatusGroup(OrderRuntime runtime)
    {
        if (runtime == null)
            return string.Empty;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveOrderStatusGroup(runtime.Invoice);

        if (runtime.Order == null)
            return string.Empty;

        return MapLegacyStatusesToGroup(
            DonHangStateMachine_cl.GetOrderStatus(runtime.Order),
            DonHangStateMachine_cl.GetExchangeStatus(runtime.Order));
    }

    public static string ResolveOrderStatusText(OrderRuntime runtime)
    {
        if (runtime == null)
            return string.Empty;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveOrderStatusText(runtime.Invoice);

        string statusGroup = ResolveOrderStatusGroup(runtime);
        switch (statusGroup)
        {
            case "cho-trao-doi":
                return "Chờ Trao đổi";
            case "da-trao-doi":
                return "Đã Trao đổi";
            case "da-giao":
                return "Đã giao";
            case "da-huy":
                return "Đã hủy";
            default:
                return "Đã đặt";
        }
    }

    public static string ResolveOrderStatusCss(OrderRuntime runtime)
    {
        if (runtime == null)
            return string.Empty;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveOrderStatusCss(runtime.Invoice);

        string statusGroup = ResolveOrderStatusGroup(runtime);
        switch (statusGroup)
        {
            case "cho-trao-doi":
                return "gh-report-status gh-report-status-warning";
            case "da-trao-doi":
                return "gh-report-status gh-report-status-info";
            case "da-giao":
                return "gh-report-status gh-report-status-success";
            case "da-huy":
                return "gh-report-status gh-report-status-danger";
            default:
                return "gh-report-status gh-report-status-neutral";
        }
    }

    public static bool ResolveIsOffline(OrderRuntime runtime)
    {
        if (runtime == null)
            return false;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveIsOffline(runtime.Invoice);

        return runtime.Order != null && runtime.Order.online_offline.HasValue && runtime.Order.online_offline.Value == false;
    }

    public static decimal ResolveTotalAmount(OrderRuntime runtime)
    {
        if (runtime == null)
            return 0m;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveTotalAmount(runtime.Invoice);

        return runtime.Order == null ? 0m : (runtime.Order.tongtien ?? 0m);
    }

    public static OrderRuntime CreateRuntime(string sellerAccount, DonHang_tb order, GH_HoaDon_tb invoice)
    {
        if (order == null && invoice == null)
            return null;

        return new OrderRuntime
        {
            SellerAccount = NormalizeAccount(sellerAccount),
            Order = order,
            Invoice = invoice
        };
    }

    public static string ResolveBuyerAccount(OrderRuntime runtime)
    {
        if (runtime == null)
            return string.Empty;

        if (HasInvoice(runtime))
            return GianHangInvoice_cl.ResolveBuyerAccount(runtime.Invoice);

        return GianHangInvoice_cl.ResolveBuyerAccount(null, runtime.Order);
    }

    public static string ResolveSellerDisplayName(dbDataContext db, OrderRuntime runtime, string fallbackAccount, string fallbackLabel)
    {
        if (runtime == null)
            return fallbackLabel ?? string.Empty;

        if (HasInvoice(runtime))
        {
            return GianHangInvoice_cl.ResolveSellerDisplayName(
                db,
                runtime.Invoice,
                fallbackAccount,
                fallbackLabel);
        }

        return GianHangInvoice_cl.ResolveSellerDisplayName(
            db,
            null,
            runtime.Order,
            fallbackAccount,
            fallbackLabel);
    }

    private static string NormalizeAccount(string sellerAccount)
    {
        return (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
    }

}
