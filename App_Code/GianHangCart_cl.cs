using System;
using System.Data;
using System.Linq;
using System.Web;

public static class GianHangCart_cl
{
    private const string SessionPrefix = "gh_cart_";
    private const string ActiveShopSessionKey = "__gh_cart_active_shop";

    private static string NormalizeStorefrontAccount(string accountKey)
    {
        return (accountKey ?? "").Trim().ToLowerInvariant();
    }

    private static string SessionKey(string accountKey)
    {
        return SessionPrefix + NormalizeStorefrontAccount(accountKey);
    }

    public static string GetActiveStorefrontAccount()
    {
        var ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return "";

        return NormalizeStorefrontAccount((ctx.Session[ActiveShopSessionKey] ?? "").ToString());
    }

    public static void RememberActiveStorefrontAccount(string accountKey)
    {
        var ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return;

        string normalized = NormalizeStorefrontAccount(accountKey);
        if (normalized == "")
            return;

        ctx.Session[ActiveShopSessionKey] = normalized;
    }

    // Compatibility aliases for older call sites during migration.
    public static string GetActiveShop()
    {
        return GetActiveStorefrontAccount();
    }

    public static void RememberActiveShop(string shop)
    {
        RememberActiveStorefrontAccount(shop);
    }

    private static DataTable CreateSchema()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Price", typeof(decimal));
        dt.Columns.Add("soluong", typeof(int));
        dt.Columns.Add("thanhtien", typeof(decimal));
        dt.Columns.Add("img", typeof(string));
        dt.Columns.Add("kyhieu", typeof(string));
        return dt;
    }

    public static DataTable GetCart(string shopTaiKhoan, bool createIfMissing = true)
    {
        string normalizedShop = NormalizeStorefrontAccount(shopTaiKhoan);
        if (normalizedShop == "")
            normalizedShop = GetActiveStorefrontAccount();

        if (normalizedShop == "")
            return null;

        string key = SessionKey(normalizedShop);
        var ctx = HttpContext.Current;
        if (ctx == null)
            return null;

        DataTable cart = ctx.Session[key] as DataTable;
        if (cart == null && createIfMissing)
        {
            cart = CreateSchema();
            ctx.Session[key] = cart;
        }

        if (cart != null)
            RememberActiveStorefrontAccount(normalizedShop);

        return cart;
    }

    public static void Clear(string shopTaiKhoan)
    {
        var ctx = HttpContext.Current;
        if (ctx == null)
            return;

        string normalizedShop = NormalizeStorefrontAccount(shopTaiKhoan);
        if (normalizedShop == "")
            normalizedShop = GetActiveStorefrontAccount();
        if (normalizedShop == "")
            return;

        ctx.Session[SessionKey(normalizedShop)] = null;
    }

    public static bool AddItem(dbDataContext db, string shopTaiKhoan, int idSanPham, int soLuong, out string error)
    {
        error = "";
        if (db == null)
        {
            error = "Không thể kết nối dữ liệu.";
            return false;
        }

        string tk = NormalizeStorefrontAccount(shopTaiKhoan);
        if (string.IsNullOrEmpty(tk))
        {
            error = "Không xác định gian hàng đối tác.";
            return false;
        }

        if (soLuong <= 0)
            soLuong = 1;
        if (soLuong > 9999)
            soLuong = 9999;

        GianHangSchema_cl.EnsureSchemaSafe(db);
        IQueryable<GH_SanPham_tb> productQuery = db.GetTable<GH_SanPham_tb>()
            .Where(p => p.shop_taikhoan == tk && p.bin != true);
        GH_SanPham_tb sp = productQuery.FirstOrDefault(p => p.id == idSanPham || (p.id_baiviet ?? 0) == idSanPham);
        if (sp == null)
        {
            string referenceKey = idSanPham.ToString();
            var nativeMap = GianHangLegacyPost_cl.ResolveNativeIdByReferenceIds(db, new System.Collections.Generic.List<string> { referenceKey });
            int nativeId;
            if (nativeMap != null
                && nativeMap.TryGetValue(referenceKey, out nativeId)
                && nativeId > 0)
            {
                sp = productQuery.FirstOrDefault(p => p.id == nativeId);
            }
        }

        if (sp == null)
        {
            error = "Sản phẩm không tồn tại.";
            return false;
        }

        string loai = GianHangProduct_cl.NormalizeLoai(sp.loai);
        if (loai == GianHangProduct_cl.LoaiDichVu)
        {
            error = "Dịch vụ cần đặt lịch riêng.";
            return false;
        }

        int ton = sp.so_luong_ton ?? 0;
        if (ton > 0 && soLuong > ton)
        {
            error = "Số lượng vượt quá tồn kho.";
            return false;
        }

        DataTable cart = GetCart(tk, true);
        if (cart == null)
        {
            error = "Không thể khởi tạo giỏ hàng.";
            return false;
        }

        RememberActiveStorefrontAccount(tk);

        foreach (DataRow row in cart.Rows)
        {
            if (Convert.ToInt32(row["ID"]) == sp.id)
            {
                int current = Convert.ToInt32(row["soluong"]);
                int next = current + soLuong;
                if (ton > 0 && next > ton)
                {
                    error = "Số lượng vượt quá tồn kho.";
                    return false;
                }

                row["soluong"] = next;
                row["thanhtien"] = (sp.gia_ban ?? 0m) * next;
                return true;
            }
        }

        DataRow dr = cart.NewRow();
        dr["ID"] = sp.id;
        dr["Name"] = sp.ten ?? "";
        dr["Price"] = sp.gia_ban ?? 0m;
        dr["soluong"] = soLuong;
        dr["thanhtien"] = (sp.gia_ban ?? 0m) * soLuong;
        dr["img"] = string.IsNullOrWhiteSpace(sp.hinh_anh) ? "/uploads/images/macdinh.jpg" : sp.hinh_anh;
        dr["kyhieu"] = "sanpham";
        cart.Rows.Add(dr);
        return true;
    }

    public static bool RemoveItem(string shopTaiKhoan, int idSanPham)
    {
        DataTable cart = GetCart(shopTaiKhoan, false);
        if (cart == null)
            return false;

        foreach (DataRow row in cart.Rows)
        {
            if (Convert.ToInt32(row["ID"]) == idSanPham)
            {
                row.Delete();
                cart.AcceptChanges();
                return true;
            }
        }
        return false;
    }

    public static void UpdateQuantities(string shopTaiKhoan, HttpRequest request)
    {
        DataTable cart = GetCart(shopTaiKhoan, false);
        if (cart == null || request == null)
            return;

        foreach (DataRow row in cart.Rows)
        {
            string key = "sl_" + row["ID"].ToString();
            string raw = request.Form[key];
            if (raw == null)
                continue;

            int qty;
            if (!int.TryParse(raw.Replace(".", "").Replace(",", ""), out qty))
                qty = 1;
            if (qty <= 0)
                qty = 1;
            if (qty > 9999)
                qty = 9999;

            row["soluong"] = qty;
            row["thanhtien"] = Convert.ToDecimal(row["Price"]) * qty;
        }
        cart.AcceptChanges();
    }

    public static decimal CalculateTotal(string shopTaiKhoan)
    {
        DataTable cart = GetCart(shopTaiKhoan, false);
        if (cart == null)
            return 0m;

        decimal total = 0m;
        foreach (DataRow row in cart.Rows)
        {
            total += Convert.ToDecimal(row["thanhtien"]);
        }
        return total;
    }

    public static int CalculateTotalQuantity(string shopTaiKhoan)
    {
        DataTable cart = GetCart(shopTaiKhoan, false);
        if (cart == null)
            return 0;

        int total = 0;
        foreach (DataRow row in cart.Rows)
        {
            int quantity;
            if (!int.TryParse((row["soluong"] ?? "0").ToString(), out quantity))
                quantity = 0;
            if (quantity > 0)
                total += quantity;
        }

        return total;
    }
}
