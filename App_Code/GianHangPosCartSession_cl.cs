using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;

public static class GianHangPosCartSession_cl
{
    [Serializable]
    public sealed class CartItem
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

    public static List<CartItem> GetCart(HttpSessionState session, string sessionKey)
    {
        if (session == null)
            return new List<CartItem>();

        List<CartItem> cart = session[sessionKey] as List<CartItem>;
        if (cart == null)
        {
            cart = new List<CartItem>();
            session[sessionKey] = cart;
        }

        return cart;
    }

    public static void SaveCart(HttpSessionState session, string sessionKey, List<CartItem> cart)
    {
        if (session == null)
            return;

        session[sessionKey] = cart ?? new List<CartItem>();
    }

    public static void Clear(HttpSessionState session, string sessionKey)
    {
        SaveCart(session, sessionKey, new List<CartItem>());
    }

    public static int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static void ApplyInitialQuantity(List<CartItem> cart, string productId, int quantity)
    {
        string idsp = (productId ?? string.Empty).Trim();
        if (cart == null || idsp == string.Empty)
            return;

        CartItem item = cart.FirstOrDefault(x => string.Equals((x.ProductId ?? string.Empty).Trim(), idsp, StringComparison.OrdinalIgnoreCase));
        if (item == null)
            return;

        int safeQuantity = ClampInt(quantity, 1, 999);
        if (safeQuantity <= 1)
            return;

        item.SoLuong = ClampInt(item.SoLuong + safeQuantity - 1, 1, 999);
    }

    public static void AddOrIncrement(List<CartItem> cart, string productId, GH_SanPham_tb product)
    {
        if (cart == null || product == null)
            return;

        string idsp = (productId ?? string.Empty).Trim();
        if (idsp == string.Empty)
            idsp = product.id.ToString();

        CartItem existing = cart.FirstOrDefault(x => string.Equals((x.ProductId ?? string.Empty).Trim(), idsp, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            existing.SoLuong = ClampInt(existing.SoLuong + 1, 1, 999);
            if (string.IsNullOrEmpty((existing.ReferenceId ?? string.Empty).Trim()))
                existing.ReferenceId = GianHangOrderWrite_cl.ResolveReferenceId(product);
            existing.Name = product.ten ?? existing.Name;
            existing.Image = product.hinh_anh ?? existing.Image;
            existing.GiaBan = product.gia_ban ?? existing.GiaBan;
            return;
        }

        cart.Add(new CartItem
        {
            ProductId = idsp,
            ReferenceId = GianHangOrderWrite_cl.ResolveReferenceId(product),
            Name = product.ten ?? string.Empty,
            Image = product.hinh_anh ?? string.Empty,
            GiaBan = product.gia_ban ?? 0m,
            SoLuong = 1,
            PhanTramGiamGia = 0
        });
    }

    public static void ApplyItemCommand(List<CartItem> cart, string productId, string commandName, string qtyText, string discountText)
    {
        if (cart == null)
            return;

        string idsp = (productId ?? string.Empty).Trim();
        if (idsp == string.Empty)
            return;

        CartItem item = cart.FirstOrDefault(x => string.Equals((x.ProductId ?? string.Empty).Trim(), idsp, StringComparison.OrdinalIgnoreCase));
        if (item == null)
            return;

        string command = (commandName ?? string.Empty).Trim().ToLowerInvariant();
        if (command == "plus")
            item.SoLuong = ClampInt(item.SoLuong + 1, 1, 999);
        else if (command == "minus")
        {
            item.SoLuong = ClampInt(item.SoLuong - 1, 0, 999);
            if (item.SoLuong <= 0)
                cart.Remove(item);
        }
        else if (command == "remove")
            cart.Remove(item);
        else if (command == "updateqty")
        {
            int qty = Number_cl.Check_Int((qtyText ?? string.Empty).Trim());
            qty = ClampInt(qty, 0, 999);
            if (qty <= 0)
                cart.Remove(item);
            else
                item.SoLuong = qty;
        }
        else if (command == "updatediscount")
        {
            int percent = Number_cl.Check_Int((discountText ?? string.Empty).Trim());
            item.PhanTramGiamGia = ClampInt(percent, 0, 50);
        }
    }

    public static List<GianHangOrderCommand_cl.OfflineCartLine> BuildOfflineLines(IEnumerable<CartItem> cart)
    {
        return (cart ?? Enumerable.Empty<CartItem>())
            .Where(item => item != null)
            .Select(item => new GianHangOrderCommand_cl.OfflineCartLine
            {
                ProductId = item.ProductId,
                ReferenceId = item.ReferenceId,
                Name = item.Name,
                Image = item.Image,
                GiaBan = item.GiaBan,
                SoLuong = item.SoLuong,
                PhanTramGiamGia = item.PhanTramGiamGia
            })
            .ToList();
    }
}
