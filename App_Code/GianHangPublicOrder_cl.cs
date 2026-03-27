using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public static class GianHangPublicOrder_cl
{
    public sealed class CheckoutInput
    {
        public string StoreAccountKey { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string Note { get; set; }
        public DataTable CartTable { get; set; }
        public RootAccount_cl.RootAccountInfo BuyerInfo { get; set; }
    }

    public sealed class CheckoutResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public long OrderId { get; set; }
        public long InvoiceId { get; set; }
        public string InvoicePublicKey { get; set; }
        public string StoreAccountKey { get; set; }
        public string BuyerAccountKey { get; set; }
    }

    private sealed class CartLine
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public static taikhoan_tb ResolveStoreAccount(dbDataContext db, string requestedUser, int? incomingItemId)
    {
        if (db == null)
            return null;

        string accountKey = (requestedUser ?? string.Empty).Trim().ToLowerInvariant();
        if (accountKey == string.Empty && incomingItemId.HasValue && incomingItemId.Value > 0)
        {
            GH_SanPham_tb item = ResolvePublicProductByAnyId(db, null, incomingItemId.Value);
            if (item != null)
                accountKey = (item.shop_taikhoan ?? string.Empty).Trim().ToLowerInvariant();
        }

        if (accountKey == string.Empty)
            return null;

        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, accountKey);
        if (account == null)
            return null;

        return SpaceAccess_cl.CanAccessGianHang(db, account) ? account : null;
    }

    public static GH_SanPham_tb ResolvePublicProductByAnyId(dbDataContext db, string shopAccountKey, int rawProductId)
    {
        if (db == null || rawProductId <= 0)
            return null;

        GianHangProduct_cl.EnsureSchema(db);
        string seller = (shopAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        IQueryable<GH_SanPham_tb> query = db.GetTable<GH_SanPham_tb>().Where(p => p.bin == null || p.bin == false);
        if (seller != string.Empty)
            query = query.Where(p => p.shop_taikhoan == seller);

        GH_SanPham_tb product = query.FirstOrDefault(p => p.id == rawProductId || p.id_baiviet == rawProductId);
        if (product != null)
            return product;

        string referenceKey = rawProductId.ToString();
        Dictionary<string, int> nativeMap = GianHangLegacyPost_cl.ResolveNativeIdByReferenceIds(
            db,
            new List<string> { referenceKey });
        int nativeId;
        if (nativeMap != null
            && nativeMap.TryGetValue(referenceKey, out nativeId)
            && nativeId > 0)
        {
            return query.FirstOrDefault(p => p.id == nativeId);
        }

        return null;
    }

    public static CheckoutResult CreateOrderFromCart(dbDataContext db, CheckoutInput input)
    {
        CheckoutResult result = new CheckoutResult
        {
            Success = false,
            ErrorMessage = "Không thể tạo đơn hàng.",
            StoreAccountKey = string.Empty,
            BuyerAccountKey = string.Empty
        };

        if (db == null || input == null)
            return result;

        string sellerAccount = (input.StoreAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (sellerAccount == string.Empty)
        {
            result.ErrorMessage = "Không xác định gian hàng đối tác.";
            return result;
        }

        GianHangProduct_cl.EnsureSchema(db);

        List<CartLine> cartLines = ParseCart(input.CartTable);
        if (cartLines.Count == 0)
        {
            result.ErrorMessage = "Giỏ hàng đang trống.";
            return result;
        }

        string customerName = (input.CustomerName ?? string.Empty).Trim();
        string customerPhone = NormalizePhone(input.CustomerPhone);
        string customerAddress = (input.CustomerAddress ?? string.Empty).Trim();
        string note = (input.Note ?? string.Empty).Trim();
        if (customerName == string.Empty || customerPhone == string.Empty || customerAddress == string.Empty)
        {
            result.ErrorMessage = "Vui lòng điền đầy đủ họ tên, số điện thoại và địa chỉ.";
            return result;
        }

        List<int> ids = cartLines.Select(p => p.ProductId).Distinct().ToList();
        Dictionary<int, GH_SanPham_tb> productMap = db.GetTable<GH_SanPham_tb>()
            .Where(p => ids.Contains(p.id) && p.shop_taikhoan == sellerAccount && (p.bin == null || p.bin == false))
            .ToList()
            .ToDictionary(p => p.id, p => p);

        if (productMap.Count != ids.Count)
        {
            result.ErrorMessage = "Có sản phẩm không còn tồn tại hoặc không còn thuộc gian hàng này.";
            return result;
        }

        DateTime now = AhaTime_cl.Now;
        List<GianHangOrderWrite_cl.OrderLine> orderLines = new List<GianHangOrderWrite_cl.OrderLine>();
        foreach (CartLine line in cartLines)
        {
            GH_SanPham_tb product;
            if (!productMap.TryGetValue(line.ProductId, out product))
            {
                result.ErrorMessage = "Có sản phẩm không hợp lệ trong giỏ.";
                return result;
            }

            if (GianHangProduct_cl.NormalizeLoai(product.loai) != GianHangProduct_cl.LoaiSanPham)
            {
                result.ErrorMessage = "Dịch vụ không thể thanh toán qua giỏ hàng sản phẩm.";
                return result;
            }

            int stock = product.so_luong_ton ?? 0;
            if (stock > 0 && line.Quantity > stock)
            {
                result.ErrorMessage = "Số lượng vượt quá tồn kho của sản phẩm: " + (product.ten ?? ("#" + product.id));
                return result;
            }

            orderLines.Add(GianHangOrderWrite_cl.BuildLine(
                product,
                line.Quantity,
                product.gia_ban ?? 0m,
                GianHangProduct_cl.ResolveDiscountPercent(product)));
        }

        string buyerAccount = "";
        RootAccount_cl.RootAccountInfo buyerInfo = input.BuyerInfo;
        if (buyerInfo != null && buyerInfo.IsAuthenticated)
            buyerAccount = (buyerInfo.AccountKey ?? string.Empty).Trim();

        GianHangOrderWrite_cl.CreateOrderResult writeResult = GianHangOrderWrite_cl.CreateOrderAndInvoice(db, new GianHangOrderWrite_cl.CreateOrderInput
        {
            SellerAccount = sellerAccount,
            BuyerAccount = buyerAccount,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CustomerAddress = customerAddress,
            Note = note,
            IsOnlineOrder = true,
            IsWaitingExchange = false,
            UpdateNativeCounters = true,
            CreatedAt = now
            ,
            Lines = orderLines
        });
        if (writeResult == null)
        {
            result.ErrorMessage = "Không thể tạo đơn hàng.";
            return result;
        }

        GH_HoaDon_tb invoice = writeResult.Invoice;
        string orderId = GianHangOrderWrite_cl.ResolveOrderKey(writeResult);
        if (string.IsNullOrWhiteSpace(orderId))
        {
            result.ErrorMessage = "Không thể tạo đơn hàng.";
            return result;
        }

        string publicOrderId = GianHangOrderWrite_cl.ResolvePublicOrderId(writeResult);
        if (string.IsNullOrWhiteSpace(publicOrderId))
            publicOrderId = orderId;

        if (sellerAccount != string.Empty)
        {
            ThongBao_tb sellerNotice = new ThongBao_tb();
            sellerNotice.id = Guid.NewGuid();
            sellerNotice.daxem = false;
            sellerNotice.nguoithongbao = buyerAccount == string.Empty ? "guest" : buyerAccount;
            sellerNotice.nguoinhan = sellerAccount;
            sellerNotice.link = GianHangRoutes_cl.BuildDonBanUrl();
            sellerNotice.noidung = customerName + " vừa tạo đơn hàng tại /gianhang. ID đơn: " + publicOrderId;
            sellerNotice.thoigian = now;
            sellerNotice.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(sellerNotice);
        }

        if (buyerAccount != string.Empty && !string.Equals(buyerAccount, sellerAccount, StringComparison.OrdinalIgnoreCase))
        {
            ThongBao_tb buyerNotice = new ThongBao_tb();
            buyerNotice.id = Guid.NewGuid();
            buyerNotice.daxem = false;
            buyerNotice.nguoithongbao = sellerAccount;
            buyerNotice.nguoinhan = buyerAccount;
            buyerNotice.link = GianHangRoutes_cl.BuildBuyerOrdersUrl();
            buyerNotice.noidung = "Bạn vừa tạo đơn tại /gianhang. ID đơn: " + publicOrderId;
            buyerNotice.thoigian = now;
            buyerNotice.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(buyerNotice);
        }

        db.SubmitChanges();

        string emailErr;
        if (invoice != null)
            GianHangEmailNotify_cl.TryNotifyInvoice(db, invoice, GianHangEmailTemplate_cl.CodeOrderCreated, "", out emailErr);

        result.Success = true;
        result.ErrorMessage = string.Empty;
        long parsedOrderId;
        result.OrderId = long.TryParse(orderId, out parsedOrderId)
            ? parsedOrderId
            : (invoice == null ? 0L : invoice.id);
        result.InvoiceId = invoice == null ? 0L : invoice.id;
        result.InvoicePublicKey = invoice == null || !invoice.id_guide.HasValue
            ? string.Empty
            : invoice.id_guide.Value.ToString();
        result.StoreAccountKey = sellerAccount;
        result.BuyerAccountKey = buyerAccount;
        return result;
    }

    private static List<CartLine> ParseCart(DataTable cartTable)
    {
        List<CartLine> list = new List<CartLine>();
        if (cartTable == null)
            return list;

        foreach (DataRow row in cartTable.Rows)
        {
            int id;
            int quantity;
            if (!TryParseInt(row["ID"], out id) || id <= 0)
                continue;

            if (!TryParseInt(row["soluong"], out quantity))
                quantity = 1;

            if (quantity <= 0)
                quantity = 1;
            if (quantity > 9999)
                quantity = 9999;

            list.Add(new CartLine
            {
                ProductId = id,
                Quantity = quantity
            });
        }

        return list;
    }

    private static bool TryParseInt(object raw, out int value)
    {
        return int.TryParse((raw ?? string.Empty).ToString().Trim(), out value);
    }

    private static string NormalizePhone(string raw)
    {
        string value = (raw ?? string.Empty).Trim();
        value = value.Replace(" ", "").Replace(".", "").Replace("-", "");
        return value;
    }
}
