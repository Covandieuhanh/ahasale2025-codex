using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangInvoice_cl
{
    public const string TrangThaiMoi = "Chờ thanh toán";
    public const string TrangThaiDaThu = "Đã thanh toán";
    public const string TrangThaiHuy = "Đã hủy";

    public static void EnsureSchema(dbDataContext db)
    {
        GianHangSchema_cl.EnsureSchemaSafe(db);
    }

    public static IQueryable<GH_HoaDon_tb> QueryByShop(dbDataContext db, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        return db.GetTable<GH_HoaDon_tb>().Where(p => (p.shop_taikhoan ?? "").Trim().ToLower() == tk);
    }

    public static List<GH_HoaDon_ChiTiet_tb> GetDetails(dbDataContext db, long idHoaDon)
    {
        EnsureSchema(db);
        return db.GetTable<GH_HoaDon_ChiTiet_tb>().Where(p => p.id_hoadon == idHoaDon).ToList();
    }

    public static GH_HoaDon_tb CreateInvoice(dbDataContext db,
        string shopTaiKhoan,
        string tenKhach,
        string sdt,
        string diaChi,
        string ghiChu,
        int idSanPham,
        int soLuong)
    {
        string error;
        var items = new List<GianHangCartItem>
        {
            new GianHangCartItem { Id = idSanPham, SoLuong = soLuong }
        };

        return CreateInvoiceFromCart(db, shopTaiKhoan, tenKhach, sdt, diaChi, ghiChu, items, out error);
    }

    public static GH_HoaDon_tb CreateInvoiceFromCart(dbDataContext db,
        string shopTaiKhoan,
        string tenKhach,
        string sdt,
        string diaChi,
        string ghiChu,
        IEnumerable<GianHangCartItem> items,
        out string error)
    {
        error = "";
        EnsureSchema(db);

        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
        {
            error = "Không xác định gian hàng đối tác.";
            return null;
        }

        var list = (items ?? new List<GianHangCartItem>()).Where(p => p != null).ToList();
        if (list.Count == 0)
        {
            error = "Giỏ hàng trống.";
            return null;
        }

        var ids = list.Select(p => p.Id).Distinct().ToList();
        var products = db.GetTable<GH_SanPham_tb>()
            .Where(p => ids.Contains(p.id) && (p.shop_taikhoan ?? "").Trim().ToLower() == tk && p.bin != true)
            .ToList();

        if (products.Count != ids.Count)
        {
            error = "Sản phẩm không tồn tại.";
            return null;
        }

        decimal total = 0m;
        var productLookup = products.ToDictionary(p => p.id, p => p);
        var lineItems = new List<GH_HoaDon_ChiTiet_tb>();

        foreach (var item in list)
        {
            if (!productLookup.ContainsKey(item.Id))
            {
                error = "Sản phẩm không tồn tại.";
                return null;
            }

            var sp = productLookup[item.Id];
            string loai = GianHangProduct_cl.NormalizeLoai(sp.loai);
            if (loai == GianHangProduct_cl.LoaiDichVu)
            {
                error = "Dịch vụ không thể đặt qua giỏ hàng.";
                return null;
            }

            int qty = item.SoLuong <= 0 ? 1 : item.SoLuong;
            if (qty > 9999) qty = 9999;

            int ton = sp.so_luong_ton ?? 0;
            if (ton > 0 && qty > ton)
            {
                error = "Số lượng vượt tồn kho.";
                return null;
            }

            decimal gia = sp.gia_ban ?? 0m;
            decimal thanhTien = gia * qty;
            total += thanhTien;

            lineItems.Add(new GH_HoaDon_ChiTiet_tb
            {
                id_sanpham = sp.id,
                ten_sanpham = sp.ten,
                so_luong = qty,
                gia = gia,
                thanh_tien = thanhTien
            });
        }

        GH_HoaDon_tb hoaDon = new GH_HoaDon_tb
        {
            shop_taikhoan = tk,
            ten_khach = (tenKhach ?? "").Trim(),
            sdt = (sdt ?? "").Trim(),
            dia_chi = diaChi ?? "",
            ghi_chu = ghiChu ?? "",
            tong_tien = total,
            trang_thai = TrangThaiMoi,
            ngay_tao = AhaTime_cl.Now
        };

        db.GetTable<GH_HoaDon_tb>().InsertOnSubmit(hoaDon);
        db.SubmitChanges();

        foreach (var line in lineItems)
        {
            line.id_hoadon = hoaDon.id;
            db.GetTable<GH_HoaDon_ChiTiet_tb>().InsertOnSubmit(line);

            var sp = productLookup[line.id_sanpham ?? 0];
            int ton = sp.so_luong_ton ?? 0;
            if (ton > 0)
                sp.so_luong_ton = ton - (line.so_luong ?? 0);
            sp.so_luong_da_ban = (sp.so_luong_da_ban ?? 0) + (line.so_luong ?? 0);
            sp.ngay_cap_nhat = AhaTime_cl.Now;
        }

        db.SubmitChanges();

        foreach (var sp in productLookup.Values)
            GianHangProduct_cl.SyncToHome(db, sp);

        return hoaDon;
    }

    public static bool UpdateStatus(dbDataContext db, long idHoaDon, string shopTaiKhoan, string trangThai)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        var hd = db.GetTable<GH_HoaDon_tb>().FirstOrDefault(p => p.id == idHoaDon && (p.shop_taikhoan ?? "").Trim().ToLower() == tk);
        if (hd == null)
            return false;

        hd.trang_thai = trangThai;
        db.SubmitChanges();
        return true;
    }
}

public class GianHangCartItem
{
    public int Id { get; set; }
    public int SoLuong { get; set; }
}
