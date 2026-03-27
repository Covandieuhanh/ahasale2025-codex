using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangOrderWrite_cl
{
    public sealed class OrderLine
    {
        public GH_SanPham_tb Product { get; set; }
        public string ReferenceId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }

        public decimal LineTotal
        {
            get { return Price * Quantity; }
        }
    }

    public sealed class CreateOrderInput
    {
        public string SellerAccount { get; set; }
        public string BuyerAccount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string Note { get; set; }
        public bool IsOnlineOrder { get; set; }
        public bool IsWaitingExchange { get; set; }
        public bool UpdateNativeCounters { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<OrderLine> Lines { get; set; }
    }

    public sealed class CreateOrderResult
    {
        public string LegacyOrderId { get; set; }
        public string OrderKey { get; set; }
        public string PublicOrderId { get; set; }
        public DonHang_tb LegacyOrder { get; set; }
        public GH_HoaDon_tb Invoice { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderLine> Lines { get; set; }
    }

    public static string ResolveReferenceId(GH_SanPham_tb product)
    {
        if (product == null)
            return string.Empty;

        return product.id.ToString();
    }

    public static OrderLine BuildLine(GH_SanPham_tb product, int quantity, decimal price, int discountPercent)
    {
        if (product == null)
            return null;

        int safeQuantity = quantity <= 0 ? 1 : quantity;
        int safeDiscount = discountPercent < 0 ? 0 : (discountPercent > 50 ? 50 : discountPercent);

        return new OrderLine
        {
            Product = product,
            ReferenceId = ResolveReferenceId(product),
            ProductName = product.ten ?? string.Empty,
            ProductImage = product.hinh_anh ?? string.Empty,
            ProductType = GianHangProduct_cl.NormalizeLoai(product.loai),
            Quantity = safeQuantity,
            Price = price < 0m ? 0m : price,
            DiscountPercent = safeDiscount
        };
    }

    public static CreateOrderResult CreateOrderAndInvoice(dbDataContext db, CreateOrderInput input)
    {
        if (db == null || input == null)
            return null;

        string sellerAccount = NormalizeAccount(input.SellerAccount);
        if (string.IsNullOrWhiteSpace(sellerAccount))
            return null;

        List<OrderLine> lines = NormalizeLines(input.Lines);
        if (lines.Count == 0)
            return null;

        decimal totalAmount = CalculateTotalAmount(lines);
        DonHang_tb order = GianHangLegacyOrder_cl.CreateOrder(db, new GianHangLegacyOrder_cl.CreateOrderInput
        {
            SellerAccount = sellerAccount,
            BuyerAccount = (input.BuyerAccount ?? string.Empty).Trim(),
            TotalAmount = totalAmount,
            CustomerName = (input.CustomerName ?? string.Empty).Trim(),
            CustomerPhone = (input.CustomerPhone ?? string.Empty).Trim(),
            CustomerAddress = (input.CustomerAddress ?? string.Empty).Trim(),
            IsOnlineOrder = input.IsOnlineOrder,
            IsWaitingExchange = input.IsWaitingExchange,
            CreatedAt = input.CreatedAt
        });
        if (order == null)
            return null;

        string orderId = order.id > 0 ? order.id.ToString() : string.Empty;
        for (int i = 0; i < lines.Count; i++)
        {
            OrderLine line = lines[i];
            if (line == null || line.Product == null || line.Quantity <= 0)
                continue;

            GianHangLegacyOrder_cl.AddOrderDetail(db, new GianHangLegacyOrder_cl.CreateOrderDetailInput
            {
                OrderId = orderId,
                ReferenceId = (line.ReferenceId ?? string.Empty).Trim(),
                SellerAccount = sellerAccount,
                NativeProductId = line.Product.id,
                Name = line.ProductName ?? string.Empty,
                Image = line.ProductImage ?? string.Empty,
                ProductType = line.ProductType ?? string.Empty,
                Quantity = line.Quantity,
                Price = line.Price,
                DiscountPercent = line.DiscountPercent
            });
        }

        if (input.UpdateNativeCounters)
            ApplyNativeCounters(lines, input.CreatedAt);

        GH_HoaDon_tb invoice = CreateInvoiceSnapshot(db, input, sellerAccount, orderId, lines);

        string orderKey = ResolveOrderKey(order, invoice);
        string publicOrderId = ResolvePublicOrderId(order, invoice);

        return new CreateOrderResult
        {
            LegacyOrderId = string.IsNullOrWhiteSpace(orderId)
                ? (invoice == null ? string.Empty : GianHangInvoice_cl.ResolveRuntimeOrderKey(invoice))
                : orderId,
            OrderKey = orderKey,
            PublicOrderId = publicOrderId,
            LegacyOrder = order,
            Invoice = invoice,
            TotalAmount = totalAmount,
            Lines = lines
        };
    }

    private static List<OrderLine> NormalizeLines(IEnumerable<OrderLine> lines)
    {
        return (lines ?? Enumerable.Empty<OrderLine>())
            .Where(p => p != null && p.Product != null && p.Quantity > 0)
            .ToList();
    }

    private static decimal CalculateTotalAmount(IEnumerable<OrderLine> lines)
    {
        decimal totalAmount = 0m;
        foreach (OrderLine line in (lines ?? Enumerable.Empty<OrderLine>()))
            totalAmount += line == null ? 0m : line.LineTotal;
        return totalAmount;
    }

    private static GH_HoaDon_tb CreateInvoiceSnapshot(
        dbDataContext db,
        CreateOrderInput input,
        string sellerAccount,
        string orderId,
        IEnumerable<OrderLine> lines)
    {
        return GianHangInvoice_cl.CreateSnapshotFromOrder(
            db,
            sellerAccount,
            orderId,
            (input.BuyerAccount ?? string.Empty).Trim(),
            (input.CustomerName ?? string.Empty).Trim(),
            (input.CustomerPhone ?? string.Empty).Trim(),
            (input.CustomerAddress ?? string.Empty).Trim(),
            (input.Note ?? string.Empty).Trim(),
            !input.IsOnlineOrder,
            BuildSnapshotLines(lines));
    }

    public static List<GianHangInvoice_cl.SnapshotLine> BuildSnapshotLines(IEnumerable<OrderLine> lines)
    {
        List<GianHangInvoice_cl.SnapshotLine> snapshots = new List<GianHangInvoice_cl.SnapshotLine>();
        foreach (OrderLine line in (lines ?? Enumerable.Empty<OrderLine>()))
        {
            if (line == null || line.Product == null || line.Quantity <= 0)
                continue;

            snapshots.Add(new GianHangInvoice_cl.SnapshotLine
            {
                ProductId = line.Product.id,
                ProductName = line.ProductName ?? string.Empty,
                ProductImage = line.ProductImage ?? string.Empty,
                ProductType = line.ProductType ?? string.Empty,
                Quantity = line.Quantity,
                Price = line.Price,
                LineTotal = line.LineTotal,
                DiscountPercent = line.DiscountPercent
            });
        }

        return snapshots;
    }

    public static string ResolveOrderKey(CreateOrderResult result)
    {
        if (result == null)
            return string.Empty;

        return ResolveOrderKey(result.LegacyOrder, result.Invoice);
    }

    public static string ResolvePublicOrderId(CreateOrderResult result)
    {
        if (result == null)
            return string.Empty;

        return ResolvePublicOrderId(result.LegacyOrder, result.Invoice);
    }

    public static string ResolveOrderKey(DonHang_tb legacyOrder, GH_HoaDon_tb invoice)
    {
        if (invoice != null)
        {
            string invoiceOrderId = GianHangInvoice_cl.ResolveRuntimeOrderKey(invoice);
            if (!string.IsNullOrWhiteSpace(invoiceOrderId))
                return invoiceOrderId;
        }

        if (legacyOrder != null && legacyOrder.id > 0)
            return legacyOrder.id.ToString();

        if (invoice != null)
            return invoice.id.ToString();

        return string.Empty;
    }

    public static string ResolvePublicOrderId(DonHang_tb legacyOrder, GH_HoaDon_tb invoice)
    {
        string publicOrderId = invoice == null
            ? string.Empty
            : GianHangInvoice_cl.ResolveOrderPublicId(invoice);
        if (!string.IsNullOrWhiteSpace(publicOrderId))
            return publicOrderId;

        return ResolveOrderKey(legacyOrder, invoice);
    }

    private static void ApplyNativeCounters(IEnumerable<OrderLine> lines, DateTime now)
    {
        foreach (OrderLine line in (lines ?? Enumerable.Empty<OrderLine>()))
        {
            if (line == null || line.Product == null || line.Quantity <= 0)
                continue;

            GH_SanPham_tb product = line.Product;
            int stock = product.so_luong_ton ?? 0;
            if (stock > 0)
                product.so_luong_ton = Math.Max(0, stock - line.Quantity);
            product.so_luong_da_ban = (product.so_luong_da_ban ?? 0) + line.Quantity;
            product.ngay_cap_nhat = now;
        }
    }

    private static string NormalizeAccount(string sellerAccount)
    {
        return (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
    }
}
