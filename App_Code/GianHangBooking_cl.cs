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

    public static IQueryable<GH_DatLich_tb> QueryByStorefront(dbDataContext db, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        return db.GetTable<GH_DatLich_tb>().Where(p => p.shop_taikhoan == tk);
    }

    public static GH_SanPham_tb ResolvePublicService(dbDataContext db, string shopTaiKhoan, string rawServiceId)
    {
        EnsureSchema(db);
        int serviceId;
        if (db == null || !int.TryParse((rawServiceId ?? "").Trim(), out serviceId) || serviceId <= 0)
            return null;

        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        return GianHangProduct_cl.QueryPublicByStorefront(db, tk)
            .FirstOrDefault(p => p.id == serviceId && p.loai == GianHangProduct_cl.LoaiDichVu);
    }

    public static string ResolveServiceName(GH_DatLich_tb booking)
    {
        if (booking == null)
            return "--";

        string value = (booking.dich_vu ?? "").Trim();
        if (value != "")
            return value;

        if (booking.id_dichvu.HasValue)
            return "Dịch vụ #" + booking.id_dichvu.Value.ToString();

        return "--";
    }

    public static GH_DatLich_tb CreateBooking(dbDataContext db, string shopTaiKhoan, string tenKhach, string sdt, GH_SanPham_tb service, string dichVuText, DateTime? thoiGianHen, string ghiChu)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
            return null;

        string serviceName = (dichVuText ?? "").Trim();
        if (service != null && string.IsNullOrWhiteSpace(serviceName))
            serviceName = (service.ten ?? "").Trim();

        GH_DatLich_tb booking = new GH_DatLich_tb
        {
            shop_taikhoan = tk,
            ten_khach = (tenKhach ?? "").Trim(),
            sdt = (sdt ?? "").Trim(),
            dich_vu = serviceName,
            id_dichvu = service == null ? (int?)null : service.id,
            home_post_id = service == null ? (int?)null : service.id_baiviet,
            id_danhmuc = service == null ? null : (service.id_danhmuc ?? "").Trim(),
            thoi_gian_hen = thoiGianHen,
            ghi_chu = ghiChu ?? "",
            trang_thai = TrangThaiChoXacNhan,
            ngay_tao = AhaTime_cl.Now
        };

        db.GetTable<GH_DatLich_tb>().InsertOnSubmit(booking);
        db.SubmitChanges();
        return booking;
    }

    public static GH_DatLich_tb CreateBooking(dbDataContext db, string shopTaiKhoan, string tenKhach, string sdt, string dichVu, DateTime? thoiGianHen, string ghiChu)
    {
        return CreateBooking(db, shopTaiKhoan, tenKhach, sdt, null, dichVu, thoiGianHen, ghiChu);
    }

    public static bool UpdateStatus(dbDataContext db, long id, string shopTaiKhoan, string trangThai)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        var item = db.GetTable<GH_DatLich_tb>().FirstOrDefault(p => p.id == id && p.shop_taikhoan == tk);
        if (item == null)
            return false;

        item.trang_thai = trangThai;
        db.SubmitChanges();
        return true;
    }
}
