using System;

public static class GianHangSchema_cl
{
    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null)
            return;
        if (_schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            try
            {
                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.GH_SanPham_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_SanPham_tb
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        shop_taikhoan NVARCHAR(100) NOT NULL,
        ten NVARCHAR(MAX) NULL,
        mo_ta NVARCHAR(MAX) NULL,
        noi_dung NVARCHAR(MAX) NULL,
        hinh_anh NVARCHAR(MAX) NULL,
        gia_ban DECIMAL(18,2) NULL,
        gia_von BIGINT NULL,
        loai NVARCHAR(20) NULL,
        id_danhmuc NVARCHAR(50) NULL,
        bin BIT NULL,
        ngay_tao DATETIME NULL,
        ngay_cap_nhat DATETIME NULL,
        id_baiviet INT NULL,
        so_luong_ton INT NULL,
        so_luong_da_ban INT NULL,
        luot_truy_cap INT NULL
    );
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'id_baiviet') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD id_baiviet INT NULL;
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'so_luong_ton') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD so_luong_ton INT NULL;
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'so_luong_da_ban') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD so_luong_da_ban INT NULL;
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'luot_truy_cap') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD luot_truy_cap INT NULL;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_SanPham_shop' AND object_id = OBJECT_ID('dbo.GH_SanPham_tb')
)
BEGIN
    CREATE INDEX IX_GH_SanPham_shop ON dbo.GH_SanPham_tb(shop_taikhoan, bin, loai);
END

IF OBJECT_ID('dbo.GH_DatLich_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_DatLich_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        shop_taikhoan NVARCHAR(100) NOT NULL,
        ten_khach NVARCHAR(200) NULL,
        sdt NVARCHAR(30) NULL,
        dich_vu NVARCHAR(200) NULL,
        thoi_gian_hen DATETIME NULL,
        ghi_chu NVARCHAR(MAX) NULL,
        trang_thai NVARCHAR(30) NULL,
        ngay_tao DATETIME NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_DatLich_shop' AND object_id = OBJECT_ID('dbo.GH_DatLich_tb')
)
BEGIN
    CREATE INDEX IX_GH_DatLich_shop ON dbo.GH_DatLich_tb(shop_taikhoan, trang_thai, thoi_gian_hen);
END

IF OBJECT_ID('dbo.GH_HoaDon_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_HoaDon_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        shop_taikhoan NVARCHAR(100) NOT NULL,
        ten_khach NVARCHAR(200) NULL,
        sdt NVARCHAR(30) NULL,
        dia_chi NVARCHAR(MAX) NULL,
        ghi_chu NVARCHAR(MAX) NULL,
        tong_tien DECIMAL(18,2) NULL,
        trang_thai NVARCHAR(30) NULL,
        ngay_tao DATETIME NULL
    );
END

IF OBJECT_ID('dbo.GH_HoaDon_ChiTiet_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_HoaDon_ChiTiet_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        id_hoadon BIGINT NOT NULL,
        id_sanpham INT NULL,
        ten_sanpham NVARCHAR(MAX) NULL,
        so_luong INT NULL,
        gia DECIMAL(18,2) NULL,
        thanh_tien DECIMAL(18,2) NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_HoaDon_shop' AND object_id = OBJECT_ID('dbo.GH_HoaDon_tb')
)
BEGIN
    CREATE INDEX IX_GH_HoaDon_shop ON dbo.GH_HoaDon_tb(shop_taikhoan, ngay_tao, trang_thai);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_HoaDonCT_hoadon' AND object_id = OBJECT_ID('dbo.GH_HoaDon_ChiTiet_tb')
)
BEGIN
    CREATE INDEX IX_GH_HoaDonCT_hoadon ON dbo.GH_HoaDon_ChiTiet_tb(id_hoadon);
END
");

                _schemaReady = true;
            }
            catch
            {
                // Nếu lỗi schema thì để lần sau thử lại.
            }
        }
    }
}
