using System;
using System.Linq;

public static class GianHangBooking_cl
{
    public const string TrangThaiChoXacNhan = "Chưa xác nhận";
    public const string TrangThaiDaXacNhan = "Đã xác nhận";
    public const string TrangThaiHoanThanh = "Đã hoàn thành";
    public const string TrangThaiHuy = "Đã hủy";

    public static void EnsureSchema(dbDataContext db)
    {
        GianHangSchema_cl.EnsureSchemaSafe(db);
    }

    public static IQueryable<GH_DatLich_tb> QueryByShop(dbDataContext db, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        return db.GetTable<GH_DatLich_tb>().Where(p => (p.shop_taikhoan ?? "").Trim().ToLower() == tk);
    }

    public static GH_DatLich_tb CreateBooking(dbDataContext db, string shopTaiKhoan, string tenKhach, string sdt, string dichVu, DateTime? thoiGianHen, string ghiChu)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
            return null;

        GH_DatLich_tb booking = new GH_DatLich_tb
        {
            shop_taikhoan = tk,
            ten_khach = (tenKhach ?? "").Trim(),
            sdt = (sdt ?? "").Trim(),
            dich_vu = (dichVu ?? "").Trim(),
            thoi_gian_hen = thoiGianHen,
            ghi_chu = ghiChu ?? "",
            trang_thai = TrangThaiChoXacNhan,
            ngay_tao = AhaTime_cl.Now
        };

        db.GetTable<GH_DatLich_tb>().InsertOnSubmit(booking);
        db.SubmitChanges();
        return booking;
    }

    public static bool UpdateStatus(dbDataContext db, long id, string shopTaiKhoan, string trangThai)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        var item = db.GetTable<GH_DatLich_tb>().FirstOrDefault(p => p.id == id && (p.shop_taikhoan ?? "").Trim().ToLower() == tk);
        if (item == null)
            return false;

        item.trang_thai = trangThai;
        db.SubmitChanges();
        return true;
    }
}
